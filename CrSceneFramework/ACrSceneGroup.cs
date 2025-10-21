using JetBrains.Annotations;

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
    public abstract class ACrSceneGroup : ICrSceneGroup
    {
        private sealed class CrSceneGroupEnterRoute : ACrSceneRoute
        {
            private readonly ACrSceneGroup _crSceneGroup;

            public CrSceneGroupEnterRoute(ACrSceneGroup crSceneGroup)
            {
                _crSceneGroup = crSceneGroup;
            }

            public override void Follow(ACrSceneContext crSceneContext)
            {
                crSceneContext.EnterCrSceneGroup(_crSceneGroup);
            }
        }

        protected ACrSceneGroup(
            [NotNull]
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
        {
            DefaultGroupExitRoute =
                new CrSceneGroupExitRoute(
                    this,
                    parentDefaultRouteProvider);

            EnterRoute = new CrSceneGroupEnterRoute(this);
        }

        public ACrSceneRoute EnterRoute
        {
            get;
            private set;
        }

        void ICrSceneGroup.OnReturned(ACrSceneContext crSceneContext)
        {
            CurrentCrScene.OnReturned(crSceneContext);
        }

        protected abstract ICrScene CurrentCrScene
        {
            get;
        }

        public CrSceneGroupExitRoute DefaultGroupExitRoute
        {
            get;
            private set;
        }

        public bool IsCurrentScene(ICrScene requiredScene)
        {
            return ReferenceEquals(
                CurrentCrScene,
                requiredScene);
        }

        public bool IsActive
        {
            get;
            private set;
        }

        public virtual bool IsTopMost
        {
            get { return false; }
        }

        void ICrSceneGroup.OnEntered(ACrSceneContext crSceneContext)
        {
            IsActive = true;

            BeforeEnter(crSceneContext);
            Enter(crSceneContext);
        }

        protected virtual void BeforeEnter(ACrSceneContext crSceneContext)
        {
            
        }

        protected abstract void Enter(ACrSceneContext crSceneContext);

        public void OnCodeTimedOut(ACrSceneContext crSceneContext)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnCodeTimedOut(crSceneContext);
        }

        void ICrSceneGroup.OnMenuCancelled(
            ACrSceneContext aCrSceneContext,
            bool byOtherCommand)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnMenuCancelled(
                    aCrSceneContext,
                    byOtherCommand);
        }

        void ICrSceneGroup.OnMenuTimeOut(ACrSceneContext crSceneContext)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnMenuTimedOut(crSceneContext);
        }

        void ICrSceneGroup.OnMenuItemSelected(
            ACrSceneContext aCrSceneContext,
            int itemIndex)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnMenuItemSelected(
                    aCrSceneContext,
                    itemIndex);
        }

        void ICrSceneGroup.OnFnBoxTimedOut(ACrSceneContext crSceneContext)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnFnBoxTimedOut(crSceneContext);
        }

        void ICrSceneGroup.OnExiting(ACrSceneContext crSceneContext)
        {
            IsActive = false;

            var currentCrScene = CurrentCrScene;

            if (currentCrScene != null)
                currentCrScene.OnExiting(crSceneContext);

            AfterExit(crSceneContext);
        }

        protected virtual void AfterExit(ACrSceneContext crSceneContext)
        {
        }

        void ICrSceneGroup.OnDescending(ACrSceneContext crSceneContext)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnDescending(crSceneContext);
        }

        void ICrSceneGroup.OnCardSwiped(
            ACrSceneContext aCrSceneContext,
            string cardData,
            int cardSystemNumber)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnCardSwiped(
                    aCrSceneContext,
                    cardData,
                    cardSystemNumber);
        }

        void ICrSceneGroup.OnCodeSpecified(
            ACrSceneContext aCrSceneContext,
            string codeData)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnCodeSpecified(
                    aCrSceneContext,
                    codeData);
        }

        protected abstract bool TryAdvance(ACrSceneContext crSceneContext);

        protected void Advance(ACrSceneContext crSceneContext)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnAdvancing(crSceneContext);

            if (!TryAdvance(crSceneContext))
                DefaultGroupExitRoute.Follow(crSceneContext);
        }

        void ICrSceneGroup.Advance(ACrSceneContext crSceneContext)
        {
            Advance(crSceneContext);
        }

        void ICrSceneGroup.OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnSpecialKeyPressed(
                    crSceneContext,
                    specialKey);
        }

        void ICrSceneGroup.OnNumericKeyPressed(
            ACrSceneContext crSceneContext,
            byte numeric)
        {
            if (CurrentCrScene != null)
                CurrentCrScene.OnNumericKeyPressed(
                    crSceneContext,
                    numeric);
        }
    }
}