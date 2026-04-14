using System.Collections.Generic;
using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
using CrSceneFrameworkCF.Scenes;
#else
using CrSceneFramework.Scenes;
#endif

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public abstract class ACrAuthorizationSceneGroup : ACrSequentialSceneGroup, ICrAuthorizationSceneGroup
    {
        protected readonly AAuthorizationProcessBase AuthorizationProcess;

        protected readonly CrSceneGroupExitRoute GrantedGroupExitRoute;
        protected readonly CrSceneGroupExitRoute RejectedGroupExitRoute;
        protected readonly CrSceneGroupExitRoute RedundantGroupExitRoute;
        protected readonly CrSceneGroupExitRoute CancelledGroupExitRoute;

        protected ACrAuthorizationSceneGroup(
            [NotNull] AAuthorizationProcessBase authorizationProcess,
            [NotNull] IInstanceProvider<ACrSceneRoute> grantedRouteProvider,
            [NotNull] IInstanceProvider<ACrSceneRoute> rejectedRouteProvider,
            [NotNull] IInstanceProvider<ACrSceneRoute> redundantRouteProvider,
            [NotNull] IInstanceProvider<ACrSceneRoute> cancelledRouteProvider,
            [NotNull] IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(parentDefaultRouteProvider)
        {
            AuthorizationProcess = authorizationProcess;

            GrantedGroupExitRoute = 
                new CrSceneGroupExitRoute(
                    this,
                    grantedRouteProvider);

            RejectedGroupExitRoute = 
                new CrSceneGroupExitRoute(
                    this,
                    rejectedRouteProvider);

            RedundantGroupExitRoute = 
                new CrSceneGroupExitRoute(
                    this,
                    redundantRouteProvider);

            CancelledGroupExitRoute = 
                new CrSceneGroupExitRoute(
                    this,
                    cancelledRouteProvider);
        }

        protected abstract ICrScene GetSceneWaitingForCode();

        protected abstract ICrScene GetSceneWaitingForCard();

        protected override IEnumerable<ICrScene> Scenes
        {
            get
            {
                AuthorizationProcess.Reset();

                ICrScene crScene;

                if (AuthorizationProcess.AcceptsCode)
                {
                    crScene = 
                        new CrAuthorizeByCodeSceneDecorator(
                            GetSceneWaitingForCode(), 
                            this);

                    if (!AuthorizationProcess.AcceptsCard)
                    {
                        yield return crScene;
                        yield break;
                    }
                }
                else
                {
                    if (!AuthorizationProcess.AcceptsCard)
                        yield break;

                    crScene = GetSceneWaitingForCard();
                }

                yield return new CrAuthorizeByCardSceneDecorator(
                    crScene,
                    this);

                if (AuthorizationProcess.IsPinRequired)
                    yield return new CrAuthorizeByCodeSceneDecorator(
                        GetSceneWaitingForPin(),
                        this);
            }
        }

        protected abstract ICrScene GetSceneWaitingForPin();

        #region ICrAuthorizeSceneRouteFollower Members

        private void FollowRoute(ACrSceneContext crSceneContext)
        {
            CrSceneGroupExitRoute exitRoute = null;

            switch (AuthorizationProcess.AuthorizationProcessState)
            {
                case AuthorizationProcessState.Undecided:

                    CrSceneAdvanceRoute.Default.Follow(crSceneContext);
                    return;

                case AuthorizationProcessState.Granted:

                    exitRoute = GrantedGroupExitRoute;
                    break;

                case AuthorizationProcessState.Rejected:

                    exitRoute = RejectedGroupExitRoute;
                    break;

                case AuthorizationProcessState.Redundant:

                    exitRoute = RedundantGroupExitRoute;
                    break;

                case AuthorizationProcessState.Cancelled:

                    exitRoute = CancelledGroupExitRoute;
                    break;
            }

            (exitRoute ?? DefaultGroupExitRoute).Follow(crSceneContext);
        }

        void ICrAuthorizationSceneGroup.OnCardSwiped(
            ACrSceneContext crSceneContext,
            string cardData,
            int cardSystemNumber)
        {
            if (!AuthorizationProcess.AcceptsCard)
                return;

            AuthorizationProcess.OnCardSwiped(
                cardData,
                cardSystemNumber);

            FollowRoute(crSceneContext);
        }

        public void Cancel(ACrSceneContext crSceneContext)
        {
            AuthorizationProcess.Cancel();

            FollowRoute(crSceneContext);
        }

        public virtual bool SupportsCancelBySpecialKey
        {
            get { return true; }
        }

        void ICrAuthorizationSceneGroup.OnCodeSpecified(
            ACrSceneContext crSceneContext, 
            string codeData)
        {
            AuthorizationProcess.OnCodeSpecified(codeData);

            FollowRoute(crSceneContext);
        }

        #endregion
    }
}
