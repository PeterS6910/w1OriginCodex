using Contal.Drivers.CardReader;

using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrAuthorizeBaseSceneDecorator : CrSceneDecorator
    {
        protected readonly ICrAuthorizationSceneGroup CrAuthorizationSceneGroup;

        public CrAuthorizeBaseSceneDecorator(
            [NotNull] ICrScene decoratedScene,
            [NotNull] ICrAuthorizationSceneGroup crAuthorizationSceneGroup)
            : base(decoratedScene)
        {
            CrAuthorizationSceneGroup = crAuthorizationSceneGroup;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.No
                && CrAuthorizationSceneGroup.SupportsCancelBySpecialKey)
            {
                CrAuthorizationSceneGroup.Cancel(crSceneContext);
                return;
            }

            base.OnSpecialKeyPressed(crSceneContext,
                specialKey);
        }
    }
}
