using System.Collections.Generic;

using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public abstract class ACrSequentialSceneGroup : ACrSceneGroup
    {
        private IEnumerator<ICrScene> _sceneEnumerator;

        private bool _currentSceneValid;

        protected abstract IEnumerable<ICrScene> Scenes
        {
            get;
        }

        protected ACrSequentialSceneGroup(
            [NotNull]
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(parentDefaultRouteProvider)
        {
        }

        protected sealed override void Enter(ACrSceneContext crSceneContext)
        {
            _sceneEnumerator = Scenes.GetEnumerator();

            Advance(crSceneContext);
        }

        protected override ICrScene CurrentCrScene
        {
            get
            {
                return _currentSceneValid
                    ? _sceneEnumerator.Current
                    : null;
            }
        }

        protected override bool TryAdvance(ACrSceneContext crSceneContext)
        {
            if (_sceneEnumerator.MoveNext())
            {
                _currentSceneValid = true;

                do
                    if (_sceneEnumerator.Current.OnEntered(crSceneContext))
                        return true;
                while (_sceneEnumerator.MoveNext());
            }

            _currentSceneValid = false;
            return false;
        }
    }
}