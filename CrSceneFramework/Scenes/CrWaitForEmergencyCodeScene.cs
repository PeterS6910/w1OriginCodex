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
    public class CrWaitForEmergencyCodeScene : ACrWaitForCodeScene
    {
        public CrWaitForEmergencyCodeScene(
            int minDigits,
            int maxDigits,
            int emergencyCodeLength,
            ACrSceneRoute homeRoute)
            : base(
                minDigits,
                maxDigits,
                emergencyCodeLength,
                homeRoute)
        {
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForEmergencyCode(
                cardReader,
                MinDigits,
                MaxDigits,
                CodeLength,
                true);

            return true;
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.AccessCommands.WaitingForEmergencyCode(
                cardReader,
                MinDigits,
                MaxDigits,
                CodeLength,
                true);
        }
    }
}
