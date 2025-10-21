using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrAuthorizeByCodeSceneDecorator : CrAuthorizeBaseSceneDecorator
    {
        public CrAuthorizeByCodeSceneDecorator(
            [NotNull] 
            ICrScene decoratedScene,
            [NotNull] 
            ICrAuthorizationSceneGroup crAuthorizationSceneGroup)
            : base(decoratedScene, crAuthorizationSceneGroup)
        {
        }

        public override void OnCodeSpecified(
            ACrSceneContext crSceneContext,
            string codeData)
        {
            CrAuthorizationSceneGroup.OnCodeSpecified(
                crSceneContext, 
                codeData);
        }

        public override void OnCodeTimedOut(ACrSceneContext crSceneContext)
        {
            CrAuthorizationSceneGroup.Cancel(crSceneContext);
        }
    }
}
