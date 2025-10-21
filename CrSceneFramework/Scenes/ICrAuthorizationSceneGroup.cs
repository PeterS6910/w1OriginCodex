#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public interface ICrAuthorizationSceneGroup
    {
        void OnCodeSpecified(
            ACrSceneContext crSceneContext,
            string codeData);

        void OnCardSwiped(
            ACrSceneContext crSceneContext,
            string cardData,
            int cardSystemNumber);

        void Cancel(ACrSceneContext crSceneContext);

        bool SupportsCancelBySpecialKey
        {
            get;
        }
    }
}
