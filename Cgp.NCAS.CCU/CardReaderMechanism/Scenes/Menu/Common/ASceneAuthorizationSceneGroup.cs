using System.Collections.Generic;

using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

using CardReader = Contal.Drivers.CardReader.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal abstract class ASceneAuthorizationSceneGroup<TSceneAuthorizationSceneGroup> :
        ACrSequentialSceneGroup,
        ICcuSceneGroup
        where TSceneAuthorizationSceneGroup : ASceneAuthorizationSceneGroup<TSceneAuthorizationSceneGroup>
    {
        protected abstract class AAuthorizationSceneGroup : ACrAuthorizationSceneGroup
        {
            public TSceneAuthorizationSceneGroup ParentSceneGroup
            {
                get;
                private set;
            }

            private class RejectedRouteProvider : AFactory<ACrSceneRoute>
            {
                private readonly TSceneAuthorizationSceneGroup _sceneAuthorizationSceneGroup;

                public RejectedRouteProvider(
                    TSceneAuthorizationSceneGroup sceneAuthorizationSceneGroup)
                {
                    _sceneAuthorizationSceneGroup = sceneAuthorizationSceneGroup;
                }

                protected override ACrSceneRoute CreateInstance()
                {
                    return
                        new RejectedSceneGroup(
                            _sceneAuthorizationSceneGroup.DefaultGroupExitRoute).EnterRoute;
                }
            }

            protected abstract class AAuthorizationScene : CrBaseScene
            {
                protected readonly AAuthorizationSceneGroup SceneGroup;

                protected AAuthorizationScene(
                    AAuthorizationSceneGroup sceneGroup)
                {
                    SceneGroup = sceneGroup;
                }

                public override bool OnEntered(ACrSceneContext crSceneContext)
                {
                    Show(crSceneContext.CardReader);
                    return true;
                }

                private void Show(CardReader cardReader)
                {
                    ShowInternal(cardReader);

                    cardReader.MenuCommands.SetBottomMenuButtons(
                        cardReader,
                        GetBottomMenu());
                }

                protected virtual CRBottomMenu GetBottomMenu()
                {
                    return new CRBottomMenu
                    {
                        Button1 = CRMenuButtonLook.No,
                        Button1ReturnCode = CRSpecialKey.No,
                        Button2 = CRMenuButtonLook.Clear,
                        Button3 = CRMenuButtonLook.Clear,
                        Button4 = CRMenuButtonLook.Clear
                    };
                }

                protected abstract void ShowInternal(CardReader cardReader);

                public override void OnReturned(ACrSceneContext crSceneContext)
                {
                    Show(crSceneContext.CardReader);
                }
            }

            protected abstract class AAuthorizationSceneForcedGinCodeLedPresentation : AAuthorizationScene
            {
                protected AAuthorizationSceneForcedGinCodeLedPresentation(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                public override bool OnEntered(ACrSceneContext crSceneContext)
                {
                    var cardReader = crSceneContext.CardReader;

                    if (!cardReader.SlGinCodeLedPresentation)
                    {
                        cardReader.SlGinCodeLedPresentation = true;
                        cardReader.ApplyPresentationMask();
                        cardReader.SlGinCodeLedPresentation = false;
                    }

                    return base.OnEntered(crSceneContext);
                }

                public override void OnExiting(ACrSceneContext crSceneContext)
                {
                    var cardReader = crSceneContext.CardReader;

                    if (!cardReader.SlGinCodeLedPresentation)
                    {
                        cardReader.ApplyPresentationMask();
                    }

                    base.OnExiting(crSceneContext);
                }
            }

            protected abstract class AWaitingForCardScene : AAuthorizationScene
            {
                private ITimer _timeout;

                private readonly object _timeoutLock = new object();

                protected AWaitingForCardScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                private void StartTimeout(ACrSceneContext crSceneContext)
                {
                    lock (_timeoutLock)
                    {
                        if (_timeout != null)
                            _timeout.StopTimer();

                        _timeout = TimerManager.Static.StartTimeout(
                            CardReaderConstants.AUTHORIZATIONDELAY,
                            new TimeoutData(
                                SceneGroup,
                                crSceneContext),
                            OnTimeout);
                    }
                }

                private class TimeoutData
                {
                    private readonly AAuthorizationSceneGroup _sceneGroup;
                    private readonly ACrSceneContext _crSceneContext;

                    private readonly ICrScene _requiredScene;

                    public TimeoutData(AAuthorizationSceneGroup sceneGroup,
                        ACrSceneContext crSceneContext)
                    {
                        _sceneGroup = sceneGroup;
                        _crSceneContext = crSceneContext;

                        _requiredScene = sceneGroup.CurrentCrScene;
                    }

                    public void OnTimeout()
                    {
                        _crSceneContext.PlanDelayedRouteFollowing(
                            _requiredScene,
                            _sceneGroup,
                            _sceneGroup.CancelledGroupExitRoute);
                    }
                }

                public override bool OnEntered(ACrSceneContext crSceneContext)
                {
                    var result =  base.OnEntered(crSceneContext);

                    if (result)
                        StartTimeout(crSceneContext);

                    return result;
                }

                private bool OnTimeout(TimerCarrier timerCarrier)
                {
                    lock (_timeoutLock)
                    {
                        ((TimeoutData)timerCarrier.Data).OnTimeout();

                        _timeout = null;
                    }

                    return true;
                }

                private void StopTimeout()
                {
                    lock (_timeoutLock)
                        if (_timeout != null)
                        {
                            _timeout.StopTimer();
                            _timeout = null;
                        }
                }

                public override void OnAdvancing(ACrSceneContext crSceneContext)
                {
                    StopTimeout();
                    base.OnAdvancing(crSceneContext);
                }

                public override void OnDescending(ACrSceneContext crSceneContext)
                {
                    StopTimeout();
                    base.OnDescending(crSceneContext);
                }

                public override void OnReturned(ACrSceneContext crSceneContext)
                {
                    StartTimeout(crSceneContext);
                    base.OnReturned(crSceneContext);
                }

                public override void OnExiting(ACrSceneContext crSceneContext)
                {
                    StopTimeout();
                    base.OnExiting(crSceneContext);
                }
            }

            protected AAuthorizationSceneGroup(
                TSceneAuthorizationSceneGroup parentSceneGroup)
                : this(
                    parentSceneGroup,
                    new RejectedRouteProvider(parentSceneGroup))
            {
            }

            private AAuthorizationSceneGroup(
                TSceneAuthorizationSceneGroup parentSceneGroup,
                IInstanceProvider<ACrSceneRoute> rejectedParentRouteProvider)
                : base(
                    parentSceneGroup.SceneAuthorizationProcess,
                    CrSceneAdvanceRoute.Default,
                    rejectedParentRouteProvider,
                    CrSceneAdvanceRoute.Default,
                    parentSceneGroup.DefaultGroupExitRoute,
                    CrSceneAdvanceRoute.Default)
            {
                ParentSceneGroup = parentSceneGroup;
            }
        }

        public ASceneAuthorizationProcessBase SceneAuthorizationProcess
        {
            get;
            private set;
        }

        protected ASceneAuthorizationSceneGroup(
            ASceneAuthorizationProcessBase sceneAuthorizationProcess,
            [NotNull] IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            ACardReaderSettings cardReaderSettings)
            : base(parentDefaultRouteProvider)
        {
            CardReaderSettings = cardReaderSettings;
            SceneAuthorizationProcess = sceneAuthorizationProcess;
        }

        protected abstract ICrScene CreateAuthorizedScene();

        protected override IEnumerable<ICrScene> Scenes
        {
            get
            {
                yield return
                    new CrSceneGroupWrapperScene<AAuthorizationSceneGroup>(
                        CreateInnerAuthorizationSceneGroup());

                switch (SceneAuthorizationProcess.AuthorizationProcessState)
                {
                    case AuthorizationProcessState.Granted:
                    case AuthorizationProcessState.Undecided:

                        yield return CreateAuthorizedScene();
                        break;
                }
            }
        }

        protected abstract AAuthorizationSceneGroup CreateInnerAuthorizationSceneGroup();

        public ACardReaderSettings CardReaderSettings
        {
            get;
            private set;
        }
    }
}
