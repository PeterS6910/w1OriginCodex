using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal class CrSceneGroupWrapperScene<TCrSceneGroup> : CrBaseScene
        where TCrSceneGroup : ACrSceneGroup
    {
        public CrSceneGroupWrapperScene(
            TCrSceneGroup innerSceneGroup)
        {
            InnerSceneGroup = innerSceneGroup;
        }

        public TCrSceneGroup InnerSceneGroup
        {
            get;
            private set;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            InnerSceneGroup.EnterRoute.Follow(crSceneContext);

            return true;
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            InnerSceneGroup.EnterRoute.Follow(crSceneContext);
        }
    }
}