using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;
using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal class NoAccessToThisMenuSceneGroup : CrSimpleSceneGroup
    {
        private class Scene : CrBaseScene
        {
            private readonly CrSceneGroupExitRoute _exitRoute;

            public Scene(CrSceneGroupExitRoute exitRoute)
            {
                _exitRoute = exitRoute;
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var cardReader = crSceneContext.CardReader;

                cardReader.AccessCommands.NoAccessToThisMenu(cardReader);

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

        public NoAccessToThisMenuSceneGroup()
            : this(new DelayedInitReference<ICrScene>())
        {
        }

        private NoAccessToThisMenuSceneGroup(DelayedInitReference<ICrScene> sceneProvider)
            : base(
                sceneProvider,
                CrSceneAdvanceRoute.Default)
        {
            sceneProvider.Instance = new Scene(DefaultGroupExitRoute);
        }
    }
}
