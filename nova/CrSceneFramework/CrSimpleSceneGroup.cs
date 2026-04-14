using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public class CrSimpleSceneGroup : ACrSceneGroup
    {
        private readonly IInstanceProvider<ICrScene> _sceneProvider;
        private ICrScene _currentCrScene;

        public CrSimpleSceneGroup(
            [NotNull]
            IInstanceProvider<ICrScene> sceneProvider,
            [NotNull]
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(parentDefaultRouteProvider)
        {
            _sceneProvider = sceneProvider;
        }

        protected override ICrScene CurrentCrScene
        {
            get
            {
                return _currentCrScene;
            }
        }

        protected sealed override void Enter(ACrSceneContext crSceneContext)
        {
            _currentCrScene = null;

            Advance(crSceneContext);
        }

        protected override bool TryAdvance(ACrSceneContext crSceneContext)
        {
            if (_currentCrScene == null)
            {
                _currentCrScene = _sceneProvider.Instance;

                if (_currentCrScene.OnEntered(crSceneContext))
                    return true;
            }

            _currentCrScene = null;

            return false;
        }
    }
}