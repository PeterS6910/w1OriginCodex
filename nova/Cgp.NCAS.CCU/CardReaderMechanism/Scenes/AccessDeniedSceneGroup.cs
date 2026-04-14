using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    internal class RejectedScene : CrBaseScene
    {
        private readonly CrSceneGroupExitRoute _exitRoute;

        public RejectedScene(
            CrSceneGroupExitRoute exitRoute)
        {
            _exitRoute = exitRoute;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            
            cardReader.AccessCommands.Rejected(cardReader);

            TimerManager.Static.StartTimeout(
                CcuCore.CARD_READER_REJECT_DELAY,
                crSceneContext,
                OnTimeout);

            return true;
        }

        private bool OnTimeout(TimerCarrier timerCarrier)
        {
            var crSceneContext = ((ACrSceneContext)timerCarrier.Data);

            crSceneContext.PlanDelayedRouteFollowing(
                this,
                _exitRoute.SceneGroup,
                _exitRoute);

            return true;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.No)
                _exitRoute.Follow(crSceneContext);
        }
    }

    internal class RejectedSceneGroup : CrSimpleSceneGroup
    {
        public RejectedSceneGroup(
            [NotNull] IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
            : this(
                new DelayedInitReference<ICrScene>(),
                defaultRouteProvider)
        {
        }

        private RejectedSceneGroup(
            DelayedInitReference<ICrScene> sceneProvider,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(sceneProvider, parentDefaultRouteProvider)
        {
            sceneProvider.Instance = new RejectedScene(DefaultGroupExitRoute);
        }
    }
}