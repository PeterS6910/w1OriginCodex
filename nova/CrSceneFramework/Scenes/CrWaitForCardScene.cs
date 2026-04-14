#if COMPACT_FRAMEWORK
using Contal.Drivers.CardReader;
#else
using Contal.Drivers.CardReader;
#endif

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrWaitForCardScene : CrBaseScene
    {
        private readonly ACrSceneRoute _homeRoute;

        public CrWaitForCardScene(
            ACrSceneRoute homeRoute)
        {
            _homeRoute = homeRoute;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.Up)
                if (_homeRoute != null)
                    _homeRoute.Follow(crSceneContext);
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForCard(cardReader);

            return true;
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForCard(cardReader);
        }
    }
}
