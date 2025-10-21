using System;

using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public sealed class CrSceneGroupExitRoute : ACrSceneRoute
    {
        private readonly ACrSceneGroup _crSceneGroup;

        private readonly IInstanceProvider<ACrSceneRoute> _parentSceneRouteProvider;

        public CrSceneGroupExitRoute(
            [NotNull]
            ACrSceneGroup crSceneGroup,
            [NotNull]
            IInstanceProvider<ACrSceneRoute> parentSceneRouteProvider)
        {
            _crSceneGroup = crSceneGroup;
            _parentSceneRouteProvider = parentSceneRouteProvider;
        }

        public ACrSceneGroup SceneGroup
        {
            get { return _crSceneGroup; }
        }

        public event Action<ACrSceneContext> BeforeExit;

        public override void Follow(ACrSceneContext crSceneContext)
        {
            if (!ReferenceEquals(
                _crSceneGroup,
                crSceneContext.CurrentCrSceneGroup)
                || !_crSceneGroup.IsActive)
            {
                throw new Exception();
            }

            if (BeforeExit != null)
                BeforeExit(crSceneContext);

            crSceneContext.ExitCurrentCrSceneGroup(_crSceneGroup);

            ACrSceneRoute nextRoute = 
                _parentSceneRouteProvider.Instance
                    ?? CrSceneGroupReturnRoute.Default;

            nextRoute.Follow(crSceneContext);
        }
    }
}