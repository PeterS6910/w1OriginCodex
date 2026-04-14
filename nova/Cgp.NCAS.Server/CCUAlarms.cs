using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Server
{
    internal sealed class CCUAlarms : ASingleton<CCUAlarms>
    {
        private CCUAlarms() : base(null)
        {
            TimeAxis.Singleton.TimeZoneStateChanged += TZStateChanged;
            TimeAxis.Singleton.DailyPlanStateChanged += DPStateChanged;
        }

        public void Init()
        {
            ChangeSettingsForBlockAlarmCCUClockUnsynchronized();
            ChangeSettingsForBlockAlarmCCUUnconfigured();
            ChangeSettingsForBlockAlarmCCUOffline();
            ChangeSettingsForBlockAlarmDCUOffline();
            ChangeSettingsForBlockAlarmCROffline();
        }

        private bool _blockAlarmCCUClockUnsynchronized;
        private byte? _blockAlarmCCUClockUnsynchronizedObjectType;
        private Guid? _blockAlarmCCUClockUnsynchronizedObjectId;

        public void ChangeSettingsForBlockAlarmCCUClockUnsynchronized()
        {
            var actBlockAlarmCCUClockUnsynchronized =
                DevicesAlarmSettings.Singleton.BlockAlarmCCUClockUnsynchronized();

            var actBlockAlarmCCUClockUnsynchronizedObjectType =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCCUClockUnsynchronizedObjectType();

            var actBlockAlarmCCUClockUnsynchronizedObjectId =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCCUClockUnsynchronizedId();

            if (_blockAlarmCCUClockUnsynchronized == actBlockAlarmCCUClockUnsynchronized &&
                    _blockAlarmCCUClockUnsynchronizedObjectType == actBlockAlarmCCUClockUnsynchronizedObjectType &&
                    _blockAlarmCCUClockUnsynchronizedObjectId == actBlockAlarmCCUClockUnsynchronizedObjectId)
                return;

            _blockAlarmCCUClockUnsynchronized = actBlockAlarmCCUClockUnsynchronized;
            _blockAlarmCCUClockUnsynchronizedObjectType = actBlockAlarmCCUClockUnsynchronizedObjectType;
            _blockAlarmCCUClockUnsynchronizedObjectId = actBlockAlarmCCUClockUnsynchronizedObjectId;

            ChangeAlarmsCCUClockUnsynchronizedToNormal(null, null);
        }

        public bool IsBlockedAlarmCCUClockUnsynchronized()
        {
            if (_blockAlarmCCUClockUnsynchronized &&
                    _blockAlarmCCUClockUnsynchronizedObjectType != null &&
                    _blockAlarmCCUClockUnsynchronizedObjectId != null)
                switch (_blockAlarmCCUClockUnsynchronizedObjectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return TimeAxis.Singleton
                            .GetActualStatusDP(_blockAlarmCCUClockUnsynchronizedObjectId.Value) == TimeAxis.ON;

                    case (byte)ObjectType.TimeZone:
                        return TimeAxis.Singleton
                            .GetActualStatusTZ(_blockAlarmCCUClockUnsynchronizedObjectId.Value) == TimeAxis.ON;
                }

            return false;
        }

        private void ChangeAlarmsCCUClockUnsynchronizedToNormal(
            ObjectType? objectType,
            Guid? objectGuid)
        {
            if (!_blockAlarmCCUClockUnsynchronized)
                return;

            if (objectType != null && objectGuid != null)
                if (_blockAlarmCCUClockUnsynchronizedObjectType != (byte?)objectType ||
                        _blockAlarmCCUClockUnsynchronizedObjectId != objectGuid)
                    return;

            if (!IsBlockedAlarmCCUClockUnsynchronized())
                return;

            try
            {
                NcasServer.GetAlarmsQueue().TryRunOnAlarmsOwner(
                    serverAlarmsOwner =>
                        serverAlarmsOwner.StopAlarmsForAlarmType(AlarmType.CCU_ClockUnsynchronized));
            }
            catch { }
        }

        public NCASServer NcasServer
        {
            get
            {
                if (_ncasServer == null)
                    _ncasServer = CCUConfigurationHandler.Singleton.NcasServer;

                return _ncasServer;
            }
        }

        private bool _blockAlarmCCUUnconfigured;
        private byte? _blockAlarmCCUUnconfiguredObjectType;
        private Guid? _blockAlarmCCUUnconfiguredObjectId;

        public void ChangeSettingsForBlockAlarmCCUUnconfigured()
        {
            var actBlockAlarmCCUUnconfigured =
                DevicesAlarmSettings.Singleton.BlockAlarmCCUUnconfigured();

            var actBlockAlarmCCUUnconfiguredObjectType =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCCUUnconfiguredObjectType();

            var actBlockAlarmCCUUnconfiguredObjectId =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCCUUnconfiguredId();

            if (_blockAlarmCCUUnconfigured == actBlockAlarmCCUUnconfigured &&
                    _blockAlarmCCUUnconfiguredObjectType == actBlockAlarmCCUUnconfiguredObjectType &&
                    _blockAlarmCCUUnconfiguredObjectId == actBlockAlarmCCUUnconfiguredObjectId)
                return;

            _blockAlarmCCUUnconfigured = actBlockAlarmCCUUnconfigured;
            _blockAlarmCCUUnconfiguredObjectType = actBlockAlarmCCUUnconfiguredObjectType;
            _blockAlarmCCUUnconfiguredObjectId = actBlockAlarmCCUUnconfiguredObjectId;

            ChangeAlarmsCCUUnconfigured(null, null);
        }

        public bool IsBlockedAlarmCCUUnconfigured()
        {
            if (_blockAlarmCCUUnconfigured &&
                    _blockAlarmCCUUnconfiguredObjectType != null &&
                    _blockAlarmCCUUnconfiguredObjectId != null)
                switch (_blockAlarmCCUUnconfiguredObjectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return TimeAxis.Singleton
                            .GetActualStatusDP(_blockAlarmCCUUnconfiguredObjectId.Value) == TimeAxis.ON;

                    case (byte)ObjectType.TimeZone:
                        return TimeAxis.Singleton
                            .GetActualStatusTZ(_blockAlarmCCUUnconfiguredObjectId.Value) == TimeAxis.ON;
                }

            return false;
        }

        private void ChangeAlarmsCCUUnconfigured(ObjectType? objectType, Guid? objectGuid)
        {
            if (!_blockAlarmCCUUnconfigured)
                return;

            if (objectType != null && objectGuid != null)
                if (_blockAlarmCCUUnconfiguredObjectType != (byte?)objectType ||
                        _blockAlarmCCUUnconfiguredObjectId != objectGuid)
                    return;

            if (IsBlockedAlarmCCUUnconfigured())
                CCUConfigurationHandler.Singleton.ForEachCCUSettings(
                    ccuSettings => ccuSettings.DeleteAlarmCCUUnconfigured());
            else
                CCUConfigurationHandler.Singleton.ForEachCCUSettings(
                    ccuSettings => ccuSettings.UpdateAlarmCcuUnconfigured());
        }

        private bool _blockAlarmCCUOffline;
        private byte? _blockAlarmCCUOfflineObjectType;
        private Guid? _blockAlarmCCUOfflineObjectId;

        public void ChangeSettingsForBlockAlarmCCUOffline()
        {
            var actBlockAlarmCCUOffline =
                DevicesAlarmSettings.Singleton.BlockAlarmCCUOffline();

            var actBlockAlarmCCUOfflineObjectType =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCCUOfflineObjectType();

            var actBlockAlarmCCUOfflineObjectId =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCCUOfflineId();

            if (_blockAlarmCCUOffline == actBlockAlarmCCUOffline &&
                    _blockAlarmCCUOfflineObjectType == actBlockAlarmCCUOfflineObjectType &&
                    _blockAlarmCCUOfflineObjectId == actBlockAlarmCCUOfflineObjectId)
                return;

            _blockAlarmCCUOffline = actBlockAlarmCCUOffline;
            _blockAlarmCCUOfflineObjectType = actBlockAlarmCCUOfflineObjectType;
            _blockAlarmCCUOfflineObjectId = actBlockAlarmCCUOfflineObjectId;

            ChangeAlarmsCCUOffline(null, null);
        }

        public bool IsRelevantObjectForBlockingAlarmCCUOffline(ObjectType objectType, Guid objectGuid)
        {
            return
                _blockAlarmCCUOffline &&
                _blockAlarmCCUOfflineObjectType != null &&
                _blockAlarmCCUOfflineObjectId != null &&
                _blockAlarmCCUOfflineObjectType.Value == (byte)objectType &&
                _blockAlarmCCUOfflineObjectId.Value == objectGuid;
        }

        public bool IsBlockedAlarmCCUOffline()
        {
            if (_blockAlarmCCUOffline &&
                    _blockAlarmCCUOfflineObjectType != null &&
                    _blockAlarmCCUOfflineObjectId != null)
                switch (_blockAlarmCCUOfflineObjectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return TimeAxis.Singleton
                            .GetActualStatusDP(_blockAlarmCCUOfflineObjectId.Value) == TimeAxis.ON;

                    case (byte)ObjectType.TimeZone:
                        return TimeAxis.Singleton
                            .GetActualStatusTZ(_blockAlarmCCUOfflineObjectId.Value) == TimeAxis.ON;
                }

            return false;
        }

        private static void ChangeAlarmsCCUOffline(ObjectType? objectType, Guid? objectGuid)
        {
            CCUConfigurationHandler.Singleton.ForEachCCUSettings(
                ccuSettings => 
                    ccuSettings.ChangeAlarmCCUOffline(
                        objectType,
                        objectGuid));
        }

        private bool _blockAlarmDCUOffline;
        private byte? _blockAlarmDCUOfflineObjectType;
        private Guid? _blockAlarmDCUOfflineObjectId;

        public void ChangeSettingsForBlockAlarmDCUOffline()
        {
            var actBlockAlarmDCUOffline =
                DevicesAlarmSettings.Singleton.BlockAlarmDCUOffline();

            var actBlockAlarmDCUOfflineObjectType =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmDCUOfflineObjectType();

            var actBlockAlarmDCUOfflineObjectId =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmDCUOfflineId();

            if (_blockAlarmDCUOffline == actBlockAlarmDCUOffline &&
                    _blockAlarmDCUOfflineObjectType == actBlockAlarmDCUOfflineObjectType &&
                    _blockAlarmDCUOfflineObjectId == actBlockAlarmDCUOfflineObjectId)
                return;

            _blockAlarmDCUOffline = actBlockAlarmDCUOffline;
            _blockAlarmDCUOfflineObjectType = actBlockAlarmDCUOfflineObjectType;
            _blockAlarmDCUOfflineObjectId = actBlockAlarmDCUOfflineObjectId;

            ChangeAlarmDcuOfflineDueCcuOffline(null, null);
        }

        private void AlarmStateChanged(ObjectType objectType, Guid objectGuid)
        {
            ChangeAlarmsCCUClockUnsynchronizedToNormal(objectType, objectGuid);
            ChangeAlarmsCCUUnconfigured(objectType, objectGuid);
            ChangeAlarmsCCUOffline(objectType, objectGuid);
            ChangeAlarmDcuOfflineDueCcuOffline(objectType, objectGuid);
            ChangeAlarmsCROffline(objectType, objectGuid);
        }

        private void TZStateChanged(
            Guid objectGuid,
            byte state)
        {
            AlarmStateChanged(ObjectType.TimeZone, objectGuid);
        }

        private void DPStateChanged(
            Guid objectGuid,
            byte state)
        {
            AlarmStateChanged(ObjectType.DailyPlan, objectGuid);
        }

        public void ChangeSettingsForAlarmDCUOffline(DCU dcu)
        {
            CCUSettings ccuSettings;

            DCUState dcuState =
                CCUConfigurationHandler.Singleton.GetDcuState(
                    dcu,
                    out ccuSettings);

            if (dcuState != null)
                dcuState.ChangeSettingsForAlarmDCUOffline(
                    dcu,
                    true);
        }

        public bool IsRelevantObjectForBlockingAlarmDCUOffline(
            ObjectType objectType,
            Guid objectGuid)
        {
            return
                _blockAlarmDCUOffline &&
                _blockAlarmDCUOfflineObjectType != null &&
                _blockAlarmDCUOfflineObjectId != null &&
                _blockAlarmDCUOfflineObjectType.Value == (byte)objectType &&
                _blockAlarmDCUOfflineObjectId.Value == objectGuid;
        }

        public bool IsBlockedAlarmDCUOffline()
        {
            if (_blockAlarmDCUOffline &&
                    _blockAlarmDCUOfflineObjectType != null &&
                    _blockAlarmDCUOfflineObjectId != null)
                switch (_blockAlarmDCUOfflineObjectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return TimeAxis.Singleton
                            .GetActualStatusDP(_blockAlarmDCUOfflineObjectId.Value) == TimeAxis.ON;

                    case (byte)ObjectType.TimeZone:
                        return TimeAxis.Singleton
                            .GetActualStatusTZ(_blockAlarmDCUOfflineObjectId.Value) == TimeAxis.ON;
                }

            return false;
        }

        public static bool IsBlockedAlarmDCUOffline(DCU dcu)
        {
            CCUSettings ccuSettings;

            DCUState dcuState =
                CCUConfigurationHandler.Singleton.GetDcuState(
                    dcu,
                    out ccuSettings);

            return
                dcuState != null &&
                dcuState.IsBlockedAlarmDCUOffline();
        }

        private static void ChangeAlarmDcuOfflineDueCcuOffline(
            ObjectType? objectType,
            Guid? objectGuid)
        {
            CCUConfigurationHandler.Singleton.ForEachCCUSettings(
                ccuSettings => 
                    ccuSettings.ChangeAlarmDcuOfflineDueCcuOffline(
                        objectType,
                        objectGuid));
        }

        private bool _blockAlarmCROffline;
        private byte? _blockAlarmCROfflineObjectType;
        private Guid? _blockAlarmCROfflineObjectId;

        private NCASServer _ncasServer;

        public void ChangeSettingsForBlockAlarmCROffline()
        {
            var actBlockAlarmCROffline =
                DevicesAlarmSettings.Singleton.BlockAlarmCROffline();

            var actBlockAlarmCROfflineObjectType =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCROfflineObjectType();

            var actBlockAlarmCROfflineObjectId =
                DevicesAlarmSettings.Singleton.ObjBlockAlarmCROfflineId();

            if (_blockAlarmCROffline == actBlockAlarmCROffline &&
                    _blockAlarmCROfflineObjectType == actBlockAlarmCROfflineObjectType &&
                    _blockAlarmCROfflineObjectId == actBlockAlarmCROfflineObjectId)
                return;

            _blockAlarmCROffline = actBlockAlarmCROffline;
            _blockAlarmCROfflineObjectType = actBlockAlarmCROfflineObjectType;
            _blockAlarmCROfflineObjectId = actBlockAlarmCROfflineObjectId;

            ChangeAlarmsCROffline(null, null);
        }

        public static void ChangeSettingsForAlarmCROffline(CardReader cardRader)
        {
            if (cardRader == null)
                return;

            CardReaderState cardReaderState = 
                CCUConfigurationHandler.Singleton
                    .GetCardReaderState(cardRader.IdCardReader);

            if (cardReaderState != null)
                cardReaderState.ChangeSettingsForAlarmCROffline(
                    cardRader,
                    true);
        }

        public bool IsRelevantObjectForBlockingAlarmCROffline(
            ObjectType objectType,
            Guid objectGuid)
        {
            return
                _blockAlarmCROffline &&
                _blockAlarmCROfflineObjectType != null &&
                _blockAlarmCROfflineObjectId != null &&
                _blockAlarmCROfflineObjectType.Value == (byte)objectType &&
                _blockAlarmCROfflineObjectId.Value == objectGuid;
        }

        public bool IsBlockedAlarmCROffline()
        {
            if (_blockAlarmCROffline &&
                    _blockAlarmCROfflineObjectType != null &&
                    _blockAlarmCROfflineObjectId != null)
                switch (_blockAlarmCROfflineObjectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return TimeAxis.Singleton
                            .GetActualStatusDP(_blockAlarmCROfflineObjectId.Value) == TimeAxis.ON;

                    case (byte)ObjectType.TimeZone:
                        return TimeAxis.Singleton
                            .GetActualStatusTZ(_blockAlarmCROfflineObjectId.Value) == TimeAxis.ON;
                }

            return false;
        }

        public static bool IsBlockedAlarmCROffline(CardReader cardRader)
        {
            CardReaderState cardReaderState = 
                CCUConfigurationHandler.Singleton.GetCardReaderState(cardRader);

            return
                cardReaderState != null &&
                cardReaderState.IsBlockedAlarmCROffline();
        }

        private static void ChangeAlarmsCROffline(
            ObjectType? objectType, 
            Guid? objectGuid)
        {
            CCUConfigurationHandler.Singleton.ForEachCCUSettings(
                ccuSettings => 
                    ccuSettings.ChangeAlarmsCROffline(
                        objectType, 
                        objectGuid));
        }

        public static void UpdateAlarmsDcuOfflineDueCcuOffline()
        {
            var dcus =
                DCUs.Singleton.List();

            foreach (var dcu in dcus)
            {
                var ccuOnlineState = CCUConfigurationHandler.Singleton.GetCCUState(dcu.CCU.IdCCU);

                UpdateAlarmDcuOfflineDueCcuOffline(
                    dcu,
                    ccuOnlineState);
            }
        }

        public static void UpdateAlarmDcuOfflineDueCcuOffline(
            DCU dcu,
            CCUOnlineState ccuOnlineState)
        {
            UpdateAlarmDcuOfflineDueCcuOffline(
                dcu,
                ccuOnlineState != CCUOnlineState.Online,
                IsBlockedAlarmDCUOffline(dcu));
        }

        private static void UpdateAlarmDcuOfflineDueCcuOffline(
            DCU dcu,
            bool isCcuOffline,
            bool isBlockedAlarm)
        {
            if (dcu == null || dcu.CCU == null)
                return;

            if (!dcu.CCU.IsConfigured)
                return;

            if (!isCcuOffline
                || isBlockedAlarm
                || (dcu.AlarmOffline != null
                    ? !(bool) dcu.AlarmOffline
                    : !DevicesAlarmSettings.Singleton.AlarmDcuOffline()))
            {
                DcuOfflineDueCcuOfflineServerAlarm.StopAlarm(dcu.IdDCU);
                return;
            }

            DcuOfflineDueCcuOfflineServerAlarm.AddAlarm(dcu);
        }

        public static void UpdateAlarmsCrOfflineDueCcuOffline()
        {
            var cardReaders =
                CardReaders.Singleton.List();

            foreach (var cardReader in cardReaders)
            {
                var ccuOnlineState = CCUConfigurationHandler.Singleton.GetCCUState(
                    CCUConfigurationHandler.GetCCUGuidFromCR(cardReader));

                UpdateAlarmCrOfflineDueCcuOffline(
                    cardReader,
                    ccuOnlineState);
            }
        }

        public static void UpdateAlarmCrOfflineDueCcuOffline(
            CardReader cardReader,
            CCUOnlineState ccuOnlineState)
        {
            UpdateAlarmCrOfflineDueCcuOffline(
                cardReader,
                ccuOnlineState != CCUOnlineState.Online,
                IsBlockedAlarmCROffline(cardReader));
        }

        public static void UpdateAlarmCrOfflineDueCcuOffline(
            CardReader cardReader,
            bool isCcuOffline,
            bool isBlockedAlarm)
        {
            if (cardReader == null)
                return;

            var ccu = CCUConfigurationHandler.GetCCUFromCR(cardReader);

            if (ccu == null)
                return;

            if (!ccu.IsConfigured)
                return;

            if (!isCcuOffline
                || isBlockedAlarm
                || (cardReader.AlarmOffline != null
                    ? !(bool) cardReader.AlarmOffline
                    : !DevicesAlarmSettings.Singleton.AlarmCrOffline()))
            {
                CrOfflineDueCcuOfflineServerAlarm.StopAlarm(cardReader.IdCardReader);
                return;
            }

            CrOfflineDueCcuOfflineServerAlarm.AddAlarm(
                ccu,
                cardReader);
        }

        public static void CreateDataChannelTransferAlarm(
            CCU ccu,
            string streamName,
            string errorMessage)
        {
            if (ccu == null)
                return;

            var alarmParameters =
                new List<AlarmParameter>
                {
                    new AlarmParameter(
                        ParameterType.FileName,
                        streamName)
                };

            Singleton.NcasServer.GetAlarmsQueue().AddAlarm(
                new ServerAlarm(
                    new ServerAlarmCore(
                        new Alarm(
                            new AlarmKey(
                                AlarmType.CCU_DataChannelDistrupted,
                                new IdAndObjectType(
                                    ccu.IdCCU,
                                    ObjectType.CCU),
                                alarmParameters),
                            AlarmState.Alarm),
                        string.Format(
                            "{0} : {1}",
                            AlarmType.CCU_DataChannelDistrupted,
                            ccu.ToString()),
                        ccu.ToString(),
                        String.Format(
                            "TCP data channel sending file failed: {0} {1}, error: {2}",
                            ccu.ToString(),
                            streamName,
                            errorMessage))));
        }

        public static void StopDataChannelTransferAlarm(
            Guid idCcu,
            string streamName)
        {
            var alarmParameters =
                new List<AlarmParameter>
                {
                    new AlarmParameter(
                        ParameterType.FileName,
                        streamName)
                };

            Singleton.NcasServer.GetAlarmsQueue().TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarm(
                        new AlarmKey(
                            AlarmType.CCU_DataChannelDistrupted,
                            new IdAndObjectType(
                                idCcu,
                                ObjectType.CCU),
                            alarmParameters)));
        }

        public static void StopDataChannelTransferAlarms(
            Guid idCcu)
        {
            Singleton.NcasServer.GetAlarmsQueue().TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarmsForAlarmType(
                        AlarmType.CCU_DataChannelDistrupted,
                        Enumerable.Repeat(
                            new IdAndObjectType(
                                idCcu,
                                ObjectType.CCU),
                            1)));
        }

        public static void CreateAlarmCcuClockUnsynchronized(
            Guid idCcu,
            string ccuIpAddress,
            DateTime dateTimeCcu,
            DateTime dateTimeServer)
        {
            if (Singleton.IsBlockedAlarmCCUClockUnsynchronized())
                return;

            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return;

            CcuClockUnsynchronizedServerAlarm.AddAlarm(
                ccu,
                dateTimeCcu,
                dateTimeServer);
        }

        public static void StopAlarmCcuClockUnsynchronized(Guid idCcu)
        {
            if (Singleton.IsBlockedAlarmCCUClockUnsynchronized())
                return;

            CcuClockUnsynchronizedServerAlarm.StopAlarm(idCcu);
        }
    }
}
