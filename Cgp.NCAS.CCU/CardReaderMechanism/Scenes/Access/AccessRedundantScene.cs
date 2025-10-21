using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access
{
    internal class AccessRedundantScene : CrBaseScene
    {
        private readonly ICcuSceneGroup _sceneGroup;

        public AccessRedundantScene(ICcuSceneGroup sceneGroup)
        {
            _sceneGroup = sceneGroup;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            _sceneGroup
                .CardReaderSettings
                .DoorEnvironmentAdapter.SuppressCardReader();

            var cardReader = crSceneContext.CardReader;

            cardReader.DisplayCommands.DisplaySymbol(
                cardReader,
                CRSymbolClass.HardDefinedPicture,
                (byte)CrIconSymbol.Accepted,
                false,
                82,
                12,
                0,
                0);

            TimerManager.Static.StartTimeout(
                CcuCore.CARD_READER_REJECT_DELAY,
                crSceneContext,
                OnTimeout);

            return true;
        }

        private bool OnTimeout(TimerCarrier timerCarrier)
        {
            var crSceneContext = (ACrSceneContext)timerCarrier.Data;

            crSceneContext.PlanDelayedRouteFollowing(
                this,
                _sceneGroup,
                _sceneGroup.DefaultGroupExitRoute);

            return true;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.No)
                _sceneGroup.DefaultGroupExitRoute.Follow(crSceneContext);
        }
    }
}