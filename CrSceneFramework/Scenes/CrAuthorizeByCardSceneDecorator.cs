using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrAuthorizeByCardSceneDecorator : CrAuthorizeBaseSceneDecorator
    {
        public CrAuthorizeByCardSceneDecorator(
            [NotNull]
            ICrScene decoratedScene,
            [NotNull]
            ICrAuthorizationSceneGroup crAuthorizationSceneGroup)
            : base(decoratedScene, crAuthorizationSceneGroup)
        {
        }

        public override void OnCardSwiped(
            ACrSceneContext crSceneContext,
            string cardData,
            int cardSystemNumber)
        {
            CrAuthorizationSceneGroup.OnCardSwiped(
                crSceneContext,
                cardData,
                cardSystemNumber);
        }
    }
}
