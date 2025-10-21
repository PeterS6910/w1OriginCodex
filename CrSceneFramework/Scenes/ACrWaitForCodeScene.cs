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
    public abstract class ACrWaitForCodeScene : CrBaseScene
    {
        protected readonly int MinDigits;
        protected readonly int MaxDigits;
        protected readonly int CodeLength;

        private readonly ACrSceneRoute _homeRoute;

        protected ACrWaitForCodeScene(
            int minDigits,
            int maxDigits,
            int codeLength,
            ACrSceneRoute homeRoute)
        {
            MinDigits = minDigits;
            MaxDigits = maxDigits;
            CodeLength = codeLength;

            _homeRoute = homeRoute;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.Up)
                _homeRoute.Follow(crSceneContext);
        }
    }
}
