using System;
using System.Collections.Generic;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

#if COMPACT_FRAMEWORK
using Contal.Drivers.CardReader;
#else
using Contal.Drivers.CardReader;
#endif

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public abstract class ACrSceneContext 
    {
        public abstract CardReader CardReader
        {
            get;
        }

        private readonly Stack<ICrSceneGroup> _sceneGroupStack =
            new Stack<ICrSceneGroup>();
        
        private readonly object _contextSync = new object();

        private const long RestartDelay = 1000;

        private abstract class ARouteFollowRequest : IProcessingQueueRequest<ACrSceneContext>
        {
            public void Execute(ACrSceneContext crSceneContext)
            {
                lock (crSceneContext._contextSync)
                {
                    try
                    {
                        var route = GetInstance(crSceneContext);

                        if (route != null)
                            route.Follow(crSceneContext);
                    }
                    catch (Exception ex)
                    {
                        HandledExceptionAdapter.Examine(ex);
                        crSceneContext.PlanRestart();
                    }
                }
            }

            public void OnError(ACrSceneContext param, Exception error)
            {
            }

            protected abstract ACrSceneRoute GetInstance(ACrSceneContext crSceneContext);
        }

        private class CurrentSceneRouteFollowRequest
            : ARouteFollowRequest
        {
            private readonly ICrScene _requiredScene;
            private readonly ICrSceneGroup _requiredSceneGroup;
            private readonly ACrSceneRoute _routeToFollow;

            public CurrentSceneRouteFollowRequest(
                ICrScene requiredScene,
                ICrSceneGroup requiredSceneGroup,
                ACrSceneRoute routeToFollow)
            {
                _requiredScene = requiredScene;
                _requiredSceneGroup = requiredSceneGroup;
                _routeToFollow = routeToFollow;
            }

            protected override ACrSceneRoute GetInstance(ACrSceneContext context)
            {
                var currentCrSceneGroup = context.CurrentCrSceneGroup;

                return ReferenceEquals(
                        currentCrSceneGroup,
                        _requiredSceneGroup)
                       && currentCrSceneGroup.IsCurrentScene(_requiredScene)
                    ? _routeToFollow
                    : null;
            }
        }

        private class TopMostSceneGroupExitRouteFollowRequest 
            : ARouteFollowRequest
        {
            protected override ACrSceneRoute GetInstance(ACrSceneContext context)
            {
                var currentCrSceneGroup = context.CurrentCrSceneGroup;

                return currentCrSceneGroup != null && currentCrSceneGroup.IsTopMost
                    ? currentCrSceneGroup.DefaultGroupExitRoute
                    : null;
            }
        }

        private readonly ThreadPoolQueue<IProcessingQueueRequest<ACrSceneContext>, ACrSceneContext> _processingQueue;

        protected ACrSceneContext()
        {
            _processingQueue =
                new ThreadPoolQueue<IProcessingQueueRequest<ACrSceneContext>, ACrSceneContext>(
                    ThreadPoolGetter.Get(),
                    this);
        }

        private void ExecCardReaderEvent(Action action)
        {
            lock (_contextSync)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                    PlanRestart();
                }
            }
        }

        internal ICrSceneGroup CurrentCrSceneGroup
        {
            get
            {
                return 
                    _sceneGroupStack.Count != 0
                        ? _sceneGroupStack.Peek()
                        : null;
            }
        }

        public void CardReaderCodeTimedOut()
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnCodeTimedOut(this));
        }

        public abstract long ShowInfoDelay
        {
            get;
        }

        public void CardReaderCodeSpecified(string codeData)
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnCodeSpecified(
                    this,
                    codeData));
        }

        public void CardReaderMenuCancelled(bool byOtherCommand)
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnMenuCancelled(
                    this,
                    byOtherCommand));
        }

        public void CardReaderMenuTimedOut()
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnMenuTimeOut(this));
        }

        public void CardReaderMenuItemSelected(
            int itemIndex,
            string itemText)
        {
        }

        public void CardReaderMenuItemSelected(int itemReturnCode)
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnMenuItemSelected(
                    this,
                    itemReturnCode));
        }

        public void CardReaderFnBoxTimedOut()
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnFnBoxTimedOut(this));
        }

        public void CardReaderNumericKeyPressed(byte numeric)
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnNumericKeyPressed(
                    this,
                    numeric));
        }

        public void CardReaderSpecialKeyPressed(CRSpecialKey specialKey)
        {
            ExecCardReaderEvent(
                () => CurrentCrSceneGroup.OnSpecialKeyPressed(
                    this,
                    specialKey));
        }

        public void PlanDelayedRouteFollowing(
            ICrScene requiredScene,
            ICrSceneGroup requiredSceneGroup,
            ACrSceneRoute routeToFollow)
        {
            _processingQueue.Enqueue(
                new CurrentSceneRouteFollowRequest(
                    requiredScene,
                    requiredSceneGroup,
                    routeToFollow));
        }

        private void PlanRestart()
        {
            TimerManager.Static.StartTimeout(
                RestartDelay,
                OnStartTimeout);
        }

        private bool OnStartTimeout(TimerCarrier timerCarrier)
        {
            EnterRootScene();

            return true;
        }

        public void CardReaderCardSwiped(
            string cardData,
            int cardSystemNumber)
        {
            ExecCardReaderEvent(
                () =>
                {
                    NotifyCardSwiped(
                        cardData,
                        cardSystemNumber);

                    CurrentCrSceneGroup.OnCardSwiped(
                        this,
                        cardData,
                        cardSystemNumber);
                });
        }

        protected virtual void NotifyCardSwiped(
            string cardData,
            int cardSystemNumber)
        {
        }

        public void EnterRootScene()
        {
            if (CardReader == null)
                return;

            _processingQueue.ClearAndBlock();
            _processingQueue.WaitUntilIdle();

            lock (_contextSync)
            {
                _processingQueue.Unblock();

                ExitAllScenesInternal();

                RootSceneGroupInstance.EnterRoute.Follow(this);
            }
        }

        public void ExitAllScenes()
        {
            if (CardReader == null)
                return;

            lock (_contextSync)
                ExitAllScenesInternal();
        }

        private void ExitAllScenesInternal()
        {
            CardReader.StopMenuIfRunning(
                CardReader,
                true);

            while (_sceneGroupStack.Count > 0)
                ExitCurrentCrSceneGroup(_sceneGroupStack.Peek());
        }

        internal void EnterCrSceneGroup(ICrSceneGroup crSceneGroup)
        {
            if (crSceneGroup.IsActive)
                throw new Exception();

            if (_sceneGroupStack.Count > 0)
                _sceneGroupStack.Peek().OnDescending(this);

            _sceneGroupStack.Push(crSceneGroup);

            crSceneGroup.OnEntered(this);
        }

        internal void ExitCurrentCrSceneGroup(ICrSceneGroup crSceneGroup)
        {
            _sceneGroupStack.Pop();

            crSceneGroup.OnExiting(this);
        }

        protected abstract ACrSceneGroup RootSceneGroupInstance
        {
            get;
        }

        public void UpdateTopMostScene()
        {
            _processingQueue.Enqueue(new TopMostSceneGroupExitRouteFollowRequest());
        }

        public void Dispose()
        {
            _processingQueue.Dispose();
        }
    }
}