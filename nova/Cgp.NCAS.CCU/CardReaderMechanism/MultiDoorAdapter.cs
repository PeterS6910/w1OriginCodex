using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access;
using Contal.Drivers.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal class MultiDoorAdapter : IDoorEnvironmentAdapter
    {
        internal class AuthorizationProcessClass : AAccessAuthorizationProcess
        {
            private readonly MultiDoorAdapter _multiDoorAdapter;

            public AuthorizationProcessClass(MultiDoorAdapter multiDoorAdapter)
            {
                _multiDoorAdapter = multiDoorAdapter;
            }

            protected override bool IsRedundant
            {
                get { return _multiDoorAdapter.IsRedundant; }
            }

            public override ACardReaderSettings CardReaderSettings
            {
                get { return _multiDoorAdapter._cardReaderSettings; }
            }

            protected override bool AuthorizeByCardInternal()
            {
                return AuthorizeInternal(
                    _multiDoorAdapter.HasAccessMultiDoor(AccessData));
            }

            protected override bool AuthorizeByPersonInternal()
            {
                return AuthorizeInternal(
                    _multiDoorAdapter.HasAccessMultiDoor(AccessData));
            }

            private bool AuthorizeInternal(
                ICollection<Guid> elementGuids)
            {
                if (elementGuids == null)
                    return false;

                _multiDoorAdapter.SetAccessGrantedElementGuids(elementGuids);

                return true;
            }

            protected override void OnReset()
            {
                base.OnReset();
                _multiDoorAdapter.ClearAccessGrantedElementGuids();
            }
        }

        public bool IsRedundant
        {
            get
            {
                return
                    _accessGrantedElementGuids == null
                        ? MultiDoors.Singleton.IsUnlocked(_idMultiDoor)
                        : _accessGrantedElementGuids.All(
                            guidElement => MultiDoorElements.Singleton.IsUnlocked(guidElement));

            }
        }

        private void SetAccessGrantedElementGuids(ICollection<Guid> elementGuids)
        {
            _accessGrantedElementGuids = elementGuids;
        }

        private void ClearAccessGrantedElementGuids()
        {
            _accessGrantedElementGuids = null;
        }

        public bool IsCardReaderSuppressed
        {
            get { return MultiDoors.Singleton.IsCardReaderSuppressed(_idMultiDoor); }
        }

        public MultiDoorAdapter(
            ACardReaderSettings cardReaderSettings,
            Guid idMultiDoor)
        {
            _cardReaderSettings = cardReaderSettings;
            _idMultiDoor = idMultiDoor;
        }

        public bool IsAssociatedWithDoorEnvironment(Guid idDoorEnvironment)
        {
            return false;
        }

        public bool IsAssociatedWithMultiDoor(Guid idMultiDoor)
        {
           return _idMultiDoor.Equals(idMultiDoor);
        }

        public bool IsCrStartScheduled
        {
            get { return MultiDoors.Singleton.IsCrStartScheduled(_idMultiDoor); }
        }

        public void OnAccessDenied(
            Guid guidCard,
            string cardNumber,
            string message)
        {
        }

        public void OnAccessGranted(
            AccessDataBase accessData)
        {
            MultiDoors.Singleton.OnAccessGranted(
                _idMultiDoor,
                _accessGrantedElementGuids,
                _cardReaderSettings.Id,
                accessData,
                _cardReaderSettings.CardReader);
        }

        public void ClearAccessGrantedVariables()
        {
            ClearAccessGrantedElementGuids();
        }

        public void AttachEvents()
        {
        }

        public void DetachEvents()
        {
        }

        public void SetImplicitCrCode(
            CRMessage implicitCrMessage,
            IList<CRMessage> followingMessages,
            bool intrusionOnlyViaLed)
        {
            MultiDoors.Singleton.SetImplicitCrCode(
                _idMultiDoor,
                implicitCrMessage,
                followingMessages,
                intrusionOnlyViaLed,
                _cardReaderSettings.CardReader);
        }

        public void LooseCardReaderIfSuppressed()
        {
            MultiDoors.Singleton.LooseCardReaderIfSuppressed(
                _idMultiDoor,
                _cardReaderSettings.CardReader);
        }

        public void ForceLooseCardReader()
        {
            MultiDoors.Singleton.LooseCardReader(
                _idMultiDoor,
                _cardReaderSettings.CardReader);
        }

        public void SuppressCardReader()
        {
            MultiDoors.Singleton.SuppressCardReader(_idMultiDoor);
        }

        private ICollection<Guid> _accessGrantedElementGuids;

        private readonly Guid _idMultiDoor;

        private readonly ACardReaderSettings _cardReaderSettings;

        public AAccessAuthorizationProcess CreateAccessAuthorizationProcess()
        {
            return new AuthorizationProcessClass(this);
        }

        public void OnCrOnlineStateChanged(bool isOnline)
        {
            MultiDoors.Singleton.OnCrOnlineStateChanged(
                _idMultiDoor,
                isOnline);
        }

        public bool HasAccess(AccessDataBase accessData, Guid idCardReader)
        {
            return false;
        }

        public ICollection<Guid> HasAccessMultiDoor(AccessDataBase accessData)
        {
            return MultiDoors.Singleton.HasAccessMultiDoor(
                accessData,
                _idMultiDoor);
        }
    }
}