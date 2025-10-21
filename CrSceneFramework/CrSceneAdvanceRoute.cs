#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public sealed class CrSceneAdvanceRoute : ACrSceneRoute
    {
        public static CrSceneAdvanceRoute Default;

        static CrSceneAdvanceRoute()
        {
            Default = new CrSceneAdvanceRoute();
        }

        public override void Follow(ACrSceneContext crSceneContext)
        {
            crSceneContext.CurrentCrSceneGroup.Advance(crSceneContext);
        }
    }
}