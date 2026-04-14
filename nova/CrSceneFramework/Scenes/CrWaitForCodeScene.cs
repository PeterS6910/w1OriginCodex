
#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrWaitForCodeScene : ACrWaitForCodeScene
    {
        public CrWaitForCodeScene(
            int minDigits,
            int maxDigits,
            int maxCodeLength,
            ACrSceneRoute homeRoute)
            : base(
                minDigits,
                maxDigits,
                maxCodeLength,
                homeRoute)
        {
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForCode(
                cardReader
                //,CodeLength
                );

            return true;
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForCode(
                cardReader
                //,CodeLength
                );
        }
    }
}
