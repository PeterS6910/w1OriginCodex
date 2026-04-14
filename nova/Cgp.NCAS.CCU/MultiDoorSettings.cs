using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Drivers.CardReader;
using Contal.Drivers.LPC3250;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal class MultiDoorSettings : AQueuedStateAndSettingsObject<MultiDoorSettings, DB.MultiDoor>
    {
        private readonly ICollection<Guid> _guidElements = new HashSet<Guid>();

        private int _doorTimeOpen;
        private int _doorTimeUnlock;
        private int _doorTimePreAlarm;

        private Guid _idCardReader;

        private CRMessage _implicitCrMessage;

        private CRMessage _followingMessages;

        private readonly object _cardReaderSupressedLock = new object();
        private bool _cardReaderSupressed;

        private readonly Guid _idMultiDoor;

        public MultiDoorSettings(Guid idMultiDoor)
            : base (idMultiDoor)
        {
            IsCrStartScheduled = true;
            _idMultiDoor = idMultiDoor;
        }

        public bool IsForceUnlocked
        {
            get;
            private set;
        }

        public bool IsUnlocked
        {
            get
            {
                return
                    _guidElements
                        .All(guidElement => 
                            MultiDoorElements.Singleton.IsUnlocked(guidElement));
            }
        }

        public bool IsCrStartScheduled
        {
            get;
            private set;
        }

        protected override void ConfigureInternal(DB.MultiDoor multiDoor)
        {
            bool timingsChanged =
                ConfigurationState == ConfigurationState.ConfiguringExisting
                && (
                    _doorTimeOpen != multiDoor.DoorTimeOpen ||
                    _doorTimeUnlock != multiDoor.DoorTimeUnlock ||
                    _doorTimePreAlarm != multiDoor.DoorTimePreAlarm);

            _doorTimeOpen = multiDoor.DoorTimeOpen;
            _doorTimeUnlock = multiDoor.DoorTimeUnlock;
            _doorTimePreAlarm = multiDoor.DoorTimePreAlarm;

            if (timingsChanged)
                foreach (var guidElement in _guidElements)
                    MultiDoorElements.Singleton.OnTimingsChanged(
                        guidElement,
                        multiDoor);

            if (!multiDoor.CardReaderId.Equals(_idCardReader))
            {
                if (_idCardReader != Guid.Empty)
                    CardReaders.Singleton.DetachMultiDoorAdapter(
                        _idCardReader,
                        _idMultiDoor);

                _idCardReader = multiDoor.CardReaderId;
            }
        }

        protected override void ApplyHwSetup(DB.MultiDoor dbObject)
        {
            if (!IsCrStartScheduled)
                return;

            IsCrStartScheduled = false;

            CardReader cardReader;

            var implicCrCodeParams =
                CardReaders.Singleton
                    .GetCurrentImplicitCrCodeParams(
                        _idCardReader,
                        out cardReader);

            SetImplicitCrCode(
                implicCrCodeParams.ImplicitCrMessage,
                implicCrCodeParams.FollowingMessages,
                implicCrCodeParams.IsGinOrVariations,
                cardReader);

            CardReaders.Singleton.EnterRootScene(_idCardReader);
        }

        protected override void UnconfigureInternal(DB.MultiDoor newMultiDoor)
        {
            if (newMultiDoor == null)
                CardReaders.Singleton.DetachMultiDoorAdapter(
                    _idCardReader,
                    _idMultiDoor);
            else
                IsCrStartScheduled = _idCardReader != newMultiDoor.CardReaderId;
        }

        public void RemoveMultiDoorElement(Guid guidMultiDoorElement)
        {
            _guidElements.Remove(guidMultiDoorElement);
        }

        public void OnAccessGranted(
            ICollection<Guid> accessGrantedElementGuids,
            Guid guidCardReader,
            AccessDataBase accessData,
            CardReader cardReader)
        {
            if (cardReader == null)
                return;

            if (accessGrantedElementGuids == null)
                accessGrantedElementGuids = _guidElements;

            bool anyElementHasBeenUnlocked = false;

            foreach (var guidElement in accessGrantedElementGuids)
                if (MultiDoorElements.Singleton.OnAccessGranted(
                    guidElement,
                    guidCardReader,
                    accessData,
                    cardReader.Address))
                {
                    anyElementHasBeenUnlocked = true;
                }

            lock (_cardReaderSupressedLock)
            {
                if (anyElementHasBeenUnlocked)
                {
                    _showCommandAccepted = true;

                    if (!_cardReaderSupressed)
                        cardReader.AccessCommands.Accepted(cardReader);
                }
                else
                {
                    _showCommandAlarmAreaIsSet = true;

                    if (!_cardReaderSupressed)
                        cardReader.AlarmAreaCommands.AlarmAreaIsSet(cardReader);
                }
            }

            NativeTimerManager.StartTimeout(
                CcuCore.CARD_READER_REJECT_DELAY,
                cardReader,
                OnAccessGrantedTimeout);
        }

        private bool _showCommandAccepted;
        private bool _showCommandAlarmAreaIsSet;

        private bool OnAccessGrantedTimeout(NativeTimer timer)
        {
            lock (_cardReaderSupressedLock)
            {
                _showCommandAccepted = false;
                _showCommandAlarmAreaIsSet = false;

                if (_cardReaderSupressed)
                    return true;

                if (timer == null)
                    return true;

                var cardReader = timer.Data as CardReader;

                if (cardReader == null)
                    return true;

                SendStoredImplicitCrCode(cardReader);
            }

            return true;
        }

        public void OnMultiDoorElementAdded(Guid guidMultiDoorElement)
        {
            _guidElements.Add(guidMultiDoorElement);
        }

        public void OnMultiDoorElementRemoved(Guid guidMultiDoorElement)
        {
            _guidElements.Remove(guidMultiDoorElement);
        }

        public void SendAllStates()
        {
            foreach (var guidElement in _guidElements)
                MultiDoorElements.Singleton.SendAllStates(guidElement);
        }

        public void SetForceUnlocked(bool isForceUnlocked)
        {
            IsForceUnlocked = isForceUnlocked;

            foreach (var guidElement in _guidElements)
                MultiDoorElements.Singleton.SetForceUnlocked(guidElement, isForceUnlocked);
        }

        protected override MultiDoorSettings This
        {
            get { return this; }
        }

        public void SetImplicitCrCode(
            [NotNull] CRMessage implicitCrMessage,
            IList<CRMessage> followingMessages,
            bool intrusionOnlyViaLed,
            CardReader cardReader)
        {
            if (cardReader == null)
                return;

            _implicitCrMessage = implicitCrMessage;

            _followingMessages =
                CRControlCommands.StackedMessage(
                    followingMessages,
                    cardReader.Address);

            if (!_cardReaderSupressed)
                SendStoredImplicitCrCode(cardReader);

            MultiDoors.Singleton.SetForceUnlocked(
                _idMultiDoor,
                _implicitCrMessage.MessageCode == CRMessageCode.DOOR_UNLOCKED);
        }

        private void SendStoredImplicitCrCode(CardReader cardReader)
        {
            if (cardReader == null)
                return;

            cardReader.ParentCommunicator.SendMessage(_implicitCrMessage);

            if (_followingMessages != null)
            {
                cardReader.ParentCommunicator.SendMessage(_followingMessages);
            }
        }

        public void SuppressCardReader()
        {
            lock (_cardReaderSupressedLock)
                _cardReaderSupressed = true;
        }

        public void LooseCardReaderIfSuppressed(CardReader cardReader)
        {
            lock (_cardReaderSupressedLock)
            {
                if (!_cardReaderSupressed)
                    return;

                LooseCardReader(cardReader);
            }
        }

        public void LooseCardReader(CardReader cardReader)
        {
            lock (_cardReaderSupressedLock)
            {
                _cardReaderSupressed = false;

                if (_showCommandAccepted)
                {
                    cardReader.AccessCommands.Accepted(cardReader);
                    return;
                }

                if (_showCommandAlarmAreaIsSet)
                {
                    cardReader.AlarmAreaCommands.AlarmAreaIsSet(cardReader);
                    return;
                }

                SendStoredImplicitCrCode(cardReader);
            }
        }

        public bool IsCardReaderSuppressed
        {
            get
            {
                lock (_cardReaderSupressedLock)
                    return _cardReaderSupressed;
            }
        }

        public void OnCrOnlineStateChanged(bool isOnline)
        {
            if (isOnline)
                return;

            lock (_cardReaderSupressedLock)
                _cardReaderSupressed = false;
        }

        public ICollection<Guid> HasAccessMultiDoor(AccessDataBase accessData)
        {
            return CardAccessRightsManager.Singleton.HasAccessMultiDoor(
                accessData,
                _idMultiDoor,
                _guidElements);
        }
    }
}
