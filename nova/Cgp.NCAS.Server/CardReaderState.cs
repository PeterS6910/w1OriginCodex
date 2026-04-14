using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server
{
    public class CardReaderState
    {
        private readonly Guid _cardReaderGuid;
        private OnlineState _onlineState;
        private DateTime? _dateTimeLastCardReaderOnlineStateChanged;

        private DateTime? _dateTimeLastLastCardChanged;

        private DateTime? _dateTimeLastCardReaderCommandChanged;
        private bool _isUpgrading;

        public OnlineState OnlineState
        {
            get
            {
                return _onlineState == OnlineState.Online
                       && _isUpgrading
                    ? OnlineState.Upgrading
                    : _onlineState;
            }
        }

        public string ProtocolVersion
        {
            get;
            private set;
        }

        public string FirmwareVersion
        {
            get;
            private set;
        }

        public string LastCard
        {
            get;
            private set;
        }

        public string ProtocolMajor
        {
            get;
            private set;
        }

        public CardReaderSceneType CardReaderCommand
        {
            get;
            private set;
        }

        public CardReaderState(Beans.CardReader cardReader, DateTime? dateTime, bool online)
        {
            CardReaderCommand = CardReaderSceneType.Unknown;
            SetOnlineState(
                dateTime,
                online
                    ? OnlineState.Online
                    : OnlineState.Offline);

            if (cardReader == null)
                return;

            _cardReaderGuid = cardReader.IdCardReader;
            ChangeSettingsForAlarmCROffline(cardReader, false);
        }

        public bool SetOnlineState(DateTime? dateTime, OnlineState onlineState)
        {
            if (dateTime != null
                && _dateTimeLastCardReaderOnlineStateChanged != null
                && dateTime < _dateTimeLastCardReaderOnlineStateChanged)
            {
                return false;
            }

            _dateTimeLastCardReaderOnlineStateChanged = dateTime;
            _onlineState = onlineState;

            if (onlineState == OnlineState.Unknown
                || onlineState == OnlineState.Offline)
            {
                SetCardReaderCommand(dateTime, CardReaderSceneType.Unknown);
            }

            return true;
        }

        public bool SetBlockedState(
            DateTime blockedStateChangedDateTime,
            bool isBlocked)
        {
            if (_blockedStateChangedDateTime != null && _blockedStateChangedDateTime >= blockedStateChangedDateTime)
                return false;

            _blockedStateChangedDateTime = blockedStateChangedDateTime;
            IsBlocked = isBlocked;

            return true;
        }

        public void SetStateUpgrading(bool isUpgrading)
        {
            _isUpgrading = isUpgrading;
        }

        public void SetCardReaderInformation(
            string protocolVersion, 
            string firmvareVersion, 
            string hardwareVersion, 
            string protocolMajor)
        {
            ProtocolVersion = protocolVersion;
            FirmwareVersion = firmvareVersion;

            byte crHV;
            Byte.TryParse(
                hardwareVersion, 
                out crHV);

            ProtocolMajor = protocolMajor;
        }

        public void SetLastCard(DateTime? dateTime, string lastCard)
        {
            if (dateTime != null
                && _dateTimeLastLastCardChanged != null
                && dateTime < _dateTimeLastLastCardChanged)
            {
                return;
            }

            _dateTimeLastLastCardChanged = dateTime;
            LastCard = lastCard;
        }

        public bool SetCardReaderCommand(
            DateTime? dateTime,
            CardReaderSceneType cardReaderCommand)
        {
            if (dateTime != null
                && _dateTimeLastCardReaderCommandChanged != null
                && dateTime < _dateTimeLastCardReaderCommandChanged)
            {
                return false;
            }

            _dateTimeLastCardReaderCommandChanged = dateTime;
            CardReaderCommand = cardReaderCommand;

            return true;
        }

        private bool? _enabledAlarmCROffline;
        private bool? _blockAlarmCROffline;
        private byte? _blockAlarmCROfflineObjectType;
        private Guid? _blockAlarmCROfflineObjectId;

        private DateTime? _blockedStateChangedDateTime;

        public bool IsBlocked
        {
            get;
            private set;
        }

        public void ChangeSettingsForAlarmCROffline(Beans.CardReader cardReader, bool updateAlarms)
        {
            bool? actEnabledAlarmCROffline = cardReader.AlarmOffline;

            bool? actBlockAlarmCROffline = cardReader.BlockAlarmOffline;
            byte? actBlockAlarmCROfflineObjectType = cardReader.ObjBlockAlarmOfflineObjectType;
            Guid? actBlockAlarmCROfflineObjectId = cardReader.ObjBlockAlarmOfflineId;

            if (_enabledAlarmCROffline == actEnabledAlarmCROffline
                && _blockAlarmCROffline == actBlockAlarmCROffline
                && _blockAlarmCROfflineObjectType == actBlockAlarmCROfflineObjectType
                && _blockAlarmCROfflineObjectId == actBlockAlarmCROfflineObjectId)
            {
                return;
            }

            _enabledAlarmCROffline = actEnabledAlarmCROffline;

            _blockAlarmCROffline = actBlockAlarmCROffline;
            _blockAlarmCROfflineObjectType = actBlockAlarmCROfflineObjectType;
            _blockAlarmCROfflineObjectId = actBlockAlarmCROfflineObjectId;

            if (updateAlarms)
                ChangeAlarmCROffline(
                    null,
                    null);
        }

        public void ChangeAlarmCROffline(ObjectType? objectType, Guid? objectGuid)
        {
            if (objectType != null && objectGuid != null)
            {
                if (_blockAlarmCROffline == null)
                {
                    if (!CCUAlarms.Singleton.IsRelevantObjectForBlockingAlarmCROffline(
                            objectType.Value,
                            objectGuid.Value))
                    {
                        return;
                    }
                }
                else
                {
                    if (!_blockAlarmCROffline.Value ||
                        (byte?) objectType != _blockAlarmCROfflineObjectType ||
                        objectGuid != _blockAlarmCROfflineObjectId)
                    {
                        return;
                    }
                }
            }

            var cardReader = CardReaders.Singleton.GetById(_cardReaderGuid);
            if (cardReader == null)
                return;

            var ccu = CCUConfigurationHandler.GetCCUFromCR(cardReader);

            if (ccu == null)
                return;

            CCUAlarms.UpdateAlarmCrOfflineDueCcuOffline(
                cardReader,
                CCUConfigurationHandler.Singleton.GetCCUState(ccu.IdCCU));
        }

        public bool IsBlockedAlarmCROffline()
        {
            if (_blockAlarmCROffline == null)
                return CCUAlarms.Singleton.IsBlockedAlarmCROffline();

            if (!_blockAlarmCROffline.Value
                || _blockAlarmCROfflineObjectType == null
                || _blockAlarmCROfflineObjectId == null)
            {
                return false;
            }

            switch (_blockAlarmCROfflineObjectType.Value)
            {
                case (byte)ObjectType.DailyPlan:

                    return TimeAxis.Singleton.GetActualStatusDP(_blockAlarmCROfflineObjectId.Value) == TimeAxis.ON;

                case (byte)ObjectType.TimeZone:

                    return TimeAxis.Singleton.GetActualStatusTZ(_blockAlarmCROfflineObjectId.Value) == TimeAxis.ON;

                default:

                    return false;
            }
        }
    }
}