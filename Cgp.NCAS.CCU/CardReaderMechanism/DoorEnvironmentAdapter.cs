using System;
using System.Collections.Generic;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Drivers.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal class DoorEnvironmentAdapter :
        IDoorEnvironmentAdapter,
        DoorEnvironments.IStateChangedHandler,
        IEquatable<DoorEnvironments.IStateChangedHandler>
    {
        private bool _areEventsAttached;

        private readonly Guid _idDoorEnvironment;

        public DoorEnvironmentAdapter(
            ACardReaderSettings cardReaderSettings,
            Guid idDoorEnvironment)
        {
            CardReaderSettings = cardReaderSettings;
            _idDoorEnvironment = idDoorEnvironment;

            AttachEvents();
        }

        private DoorEnvironmentState State
        {
            get
            {
                return
                    DoorEnvironments.Singleton
                        .GetDoorEnviromentState(_idDoorEnvironment);
            }
        }

        public bool IsAssociatedWithDoorEnvironment(Guid idDoorEnvironment)
        {
            return _idDoorEnvironment.Equals(idDoorEnvironment);
        }

        public bool IsAssociatedWithMultiDoor(Guid idMultiDoor)
        {
            return false;
        }

        public bool IsCrStartScheduled
        {
            get
            {
                return DoorEnvironments.Singleton.IsDsmStartRequired(_idDoorEnvironment);
            }
        }

        public ACardReaderSettings CardReaderSettings
        {
            get;
            private set;
        }

        public bool IsCardReaderSuppressed
        {
            get
            {
                lock (_isSuppressedLock)
                    return _isSuppressed;
            }
        }

        public void OnAccessDenied(
            Guid guidCard,
            string cardNumber,
            string message)
        {
            Events.ProcessEvent(
                new EventDsmAccessRestricted(
                    _idDoorEnvironment,
                    CardReaderSettings.Id,
                    guidCard,
                    cardNumber,
                    message));
        }

        public void OnAccessGranted(
            AccessDataBase accessData)
        {
            DoorEnvironments.Singleton.OnAccessGranted(
                _idDoorEnvironment,
                CardReaderSettings.Id,
                accessData,
                CardReaderSettings.Address);
        }

        public void AttachEvents()
        {
            if (_areEventsAttached)
                return;

            DoorEnvironments.Singleton.AddStateChangedHandler(
                _idDoorEnvironment,
                this);

            _areEventsAttached = true;
        }

        public void ClearAccessGrantedVariables()
        {
            DoorEnvironments.Singleton.ClearAccessGrantedVariables(_idDoorEnvironment);
        }

        private bool _isSuppressed;

        private readonly object _isSuppressedLock = new object();

        public void LooseCardReaderIfSuppressed()
        {
            lock (_isSuppressedLock)
            {
                if (!_isSuppressed)
                    return;

                _isSuppressed = false;

                DoorEnvironments.Singleton.LooseCardReader(
                    _idDoorEnvironment,
                    CardReaderSettings.Address);
            }
        }

        public void ForceLooseCardReader()
        {
            lock (_isSuppressedLock)
            {
                _isSuppressed = false;

                DoorEnvironments.Singleton.LooseCardReader(
                    _idDoorEnvironment,
                    CardReaderSettings.Address);
            }
        }

        public void DetachEvents()
        {
            if (!_areEventsAttached)
                return;

            DoorEnvironments.Singleton.RemoveStateChangedHandler(
                _idDoorEnvironment,
                this);

            _areEventsAttached = false;
        }

        public void SetImplicitCrCode(
            CRMessage implicitCrMessage,
            IList<CRMessage> followingMessages,
            bool intrusionOnlyViaLed)
        {
            DoorEnvironments.Singleton.SetImplicitCrCode(
                _idDoorEnvironment,
                CardReaderSettings.Address,
                implicitCrMessage,
                followingMessages,
                intrusionOnlyViaLed);
        }

        public void SuppressCardReader()
        {
            lock (_isSuppressedLock)
            {
                if (_isSuppressed)
                    return;

                _isSuppressed = true;

                DoorEnvironments.Singleton.SuppressCardReader(
                    _idDoorEnvironment,
                    CardReaderSettings.Address);
            }
        }

        public AAccessAuthorizationProcess CreateAccessAuthorizationProcess()
        {
            return new AuthorizationProcessClass(this);
        }

        public void OnCrOnlineStateChanged(bool isOnline)
        {
            if (isOnline)
                return;

            lock (_isSuppressedLock)
                _isSuppressed = false;
        }

        public bool HasAccess(
            AccessDataBase accessData,
            Guid idCardReader)
        {
            return CardAccessRightsManager.Singleton.HasAccess(
                accessData,
                CardReaderSettings.Id,
                _idDoorEnvironment,
                CardReaderSettings.CardReaderDb.GuidDCU);
        }

        public ICollection<Guid> HasAccessMultiDoor(AccessDataBase accessData)
        {
            return null;
        }

        internal class AuthorizationProcessClass : AAccessAuthorizationProcess
        {
            private readonly DoorEnvironmentAdapter _doorEnvironmentAdapter;

            public AuthorizationProcessClass(DoorEnvironmentAdapter doorEnvironmentAdapter)
            {
                _doorEnvironmentAdapter = doorEnvironmentAdapter;
            }

            protected override bool IsRedundant
            {
                get
                {
                    var state = _doorEnvironmentAdapter.State;

                    return state == DoorEnvironmentState.Unlocked ||
                           state == DoorEnvironmentState.Unlocking ||
                           state == DoorEnvironmentState.Opened;
                }
            }

            public override ACardReaderSettings CardReaderSettings
            {
                get { return _doorEnvironmentAdapter.CardReaderSettings; }
            }

            protected override bool AuthorizeByCardInternal()
            {
                if (!_doorEnvironmentAdapter.HasAccess(
                    AccessData,
                    CardReaderSettings.Id))
                {
                    return false;
                }

                return 
                    IsRedundant 
                    || AntiPassBackZones.Singleton.HasAccess(
                        AccessData.IdCard,
                        CardReaderSettings.Id);
            }

            protected override bool AuthorizeByPersonInternal()
            {
                return _doorEnvironmentAdapter.HasAccess(
                    AccessData,
                    CardReaderSettings.Id);
            }
        }

        void DoorEnvironments.IStateChangedHandler.Execute(DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            CardReaderSettings.DoorEnvironmentStateChanged(doorEnvironmentStateChangedArgs);
        }

        public override int GetHashCode()
        {
            return CardReaderSettings.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DoorEnvironments.IStateChangedHandler);
        }

        public bool Equals(DoorEnvironments.IStateChangedHandler other)
        {
            var otherAdapter = other as DoorEnvironmentAdapter;

            return 
                otherAdapter != null
                && otherAdapter.CardReaderSettings.Id == CardReaderSettings.Id;
        }
    }
}