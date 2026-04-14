using System;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public sealed class CrSceneGroupReturnRoute : ACrSceneRoute
    {
        public static CrSceneGroupReturnRoute Default;

        static CrSceneGroupReturnRoute()
        {
            Default = new CrSceneGroupReturnRoute();
        }

        public override void Follow(ACrSceneContext crSceneContext)
        {
            crSceneContext.CurrentCrSceneGroup.OnReturned(crSceneContext);
        }
    }
}