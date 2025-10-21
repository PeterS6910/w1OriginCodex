
#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrWaitForPinScene : ACrWaitForCodeScene
    {
        private readonly bool _alwaysRequireConfirmation;

        public CrWaitForPinScene(
            int minDigits,
            int maxDigits,
            int pinLength,
            bool alwaysRequireConfirmation,
            ACrSceneRoute homeRoute)
            : base(
                minDigits,
                maxDigits,
                pinLength,
                homeRoute)
        {
            _alwaysRequireConfirmation = alwaysRequireConfirmation;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForPIN(
                cardReader,
                MinDigits,
                MaxDigits,
                CodeLength,
                _alwaysRequireConfirmation);

            return true;
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForPIN(
                cardReader,
                MinDigits,
                MaxDigits,
                CodeLength,
                _alwaysRequireConfirmation);
        }
    }
}
