#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public abstract class ACrSceneRoute : IInstanceProvider<ACrSceneRoute>
    {
        internal ACrSceneRoute()
        {
        }

        public abstract void Follow(ACrSceneContext crSceneContext);

        public ACrSceneRoute Instance
        {
            get { return this; }
        }
    }
}