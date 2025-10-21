using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    internal class ImplicitCodeScene : CrSceneDecorator
    {
        private readonly ACardReaderSettings _cardReaderSettings;

        public ImplicitCodeScene(
            [NotNull]
            ACardReaderSettings cardReaderSettings,
            ICrScene decoratedScene)
            : base(decoratedScene)
        {
            _cardReaderSettings = cardReaderSettings;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var doorEnvironmentAdapter = _cardReaderSettings.DoorEnvironmentAdapter;

            if (doorEnvironmentAdapter != null)
                doorEnvironmentAdapter.LooseCardReaderIfSuppressed();

            return base.OnEntered(crSceneContext);
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            var doorEnvironmentAdapter = _cardReaderSettings.DoorEnvironmentAdapter;

            if (doorEnvironmentAdapter != null)
                doorEnvironmentAdapter.LooseCardReaderIfSuppressed();

            base.OnReturned(crSceneContext);
        }

        public override void OnDescending(ACrSceneContext crSceneContext)
        {
            var doorEnvironmentAdapter = _cardReaderSettings.DoorEnvironmentAdapter;

            if (doorEnvironmentAdapter != null)
                doorEnvironmentAdapter.SuppressCardReader();

            base.OnDescending(crSceneContext);
        }
    }
}
