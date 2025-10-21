using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal partial class ACardReaderSettings
    {
        private class InputForForcedSecurityLevelChangedListener : IInputChangedListener
        {
            private readonly ACardReaderSettings _cardReaderSettings;
            private readonly Guid _guidCardReader;

            public InputForForcedSecurityLevelChangedListener(ACardReaderSettings cardReaderSettings)
            {
                _cardReaderSettings = cardReaderSettings;
                _guidCardReader = _cardReaderSettings.Id;
            }

            public override int GetHashCode()
            {
                return _guidCardReader.GetHashCode();
            }

            public bool Equals(IInputChangedListener other)
            {
                var otherListener = other as InputForForcedSecurityLevelChangedListener;

                return otherListener != null
                       && _guidCardReader.Equals(
                           otherListener._cardReaderSettings.Id);
            }

            public void OnInputChanged(
                Guid guidInput,
                State state)
            {
                _cardReaderSettings.InputForForcedSecurityLevelChanged(
                    guidInput,
                    state);
            }
        }
    }
}
