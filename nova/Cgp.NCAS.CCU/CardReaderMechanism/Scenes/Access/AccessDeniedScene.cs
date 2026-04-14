using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access
{
    internal class AccessDeniedScene : RejectedScene
    {
        private readonly ICcuSceneGroup _sceneGroup;

        public AccessDeniedScene(
            ICcuSceneGroup sceneGroup)
            : base(sceneGroup.DefaultGroupExitRoute)
        {
            _sceneGroup = sceneGroup;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            _sceneGroup
                .CardReaderSettings
                .DoorEnvironmentAdapter.SuppressCardReader();

            return base.OnEntered(crSceneContext);
        }
    }
}