using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.CCU
{
    class DevicesAlarmSettings
    {
        private static volatile DevicesAlarmSettings _singleton;
        private static readonly object _syncRoot = new object();

        private DevicesAlarmSetting _deviceAlarmSettings;

        public static DevicesAlarmSettings Singleton
        {
            get
            {
                if (null != _singleton)
                    return _singleton;

                lock (_syncRoot)
                    if (_singleton == null)
                        _singleton = new DevicesAlarmSettings();

                return _singleton;
            }
        }

        public void Unconfigure()
        {
            _deviceAlarmSettings = null;
        }

        public void LoadDevicesAlarmSettings()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void DevicesAlarmSettings.LoadDevicesAlarmSettings()");

            var listGuidDas = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.DevicesAlarmSetting);

            if (listGuidDas == null) 
                return;

            var idDas = listGuidDas.FirstOrDefault();

            if (idDas == Guid.Empty)
                return;

            //CCU alarms settings
            bool oldAlarmCCUTamperSabotage = AlarmCCUTamperSabotage;
            bool oldBlockAlarmCCUTamperSabotage = BlockAlarmCCUTamperSabotage;
            byte? oldObjBlockAlarmCCUTamperSabotageObjectType = ObjBlockAlarmCCUTamperSabotageObjectType;
            Guid? oldObjBlockAlarmCCUTamperSabotageId = ObjBlockAlarmCCUTamperSabotageId;
            bool oldEventlogDuringBlockAlarmCCUTamperSabotage = EventlogDuringBlockAlarmCCUTamperSabotage;

            bool oldAlarmPrimaryPowerMissing = AlarmPrimaryPowerMissing;
            bool oldBlockAlarmPrimaryPowerMissing = BlockAlarmPrimaryPowerMissing;
            byte? oldObjBlockAlarmPrimaryPowerMissingObjectType = ObjBlockAlarmPrimaryPowerMissingObjectType;
            Guid? oldObjBlockAlarmPrimaryPowerMissingId = ObjBlockAlarmPrimaryPowerMissingId;
            bool oldEventlogDuringBlockAlarmPrimaryPowerMissing = EventlogDuringBlockAlarmPrimaryPowerMissing;

            bool oldAlarmBatteryIsLow = AlarmBatteryIsLow;
            bool oldBlockAlarmBatteryIsLow = BlockAlarmBatteryIsLow;
            byte? oldObjBlockAlarmBatteryIsLowObjectType = ObjBlockAlarmBatteryIsLowObjectType;
            Guid? oldObjBlockAlarmBatteryIsLowId = ObjBlockAlarmBatteryIsLowId;
            bool oldEventlogDuringBlockAlarmBatteryIsLow = EventlogDuringBlockAlarmBatteryIsLow;

            bool oldAlarmUpsOutputFuse = AlarmUpsOutputFuse;
            bool oldBlockAlarmUpsOutputFuse = BlockAlarmUpsOutputFuse;
            byte? oldObjBlockAlarmUpsOutputFuseObjectType = ObjBlockAlarmUpsOutputFuseObjectType;
            Guid? oldObjBlockAlarmUpsOutputFuseId = ObjBlockAlarmUpsOutputFuseId;
            bool oldEventlogDuringBlockAlarmUpsOutputFuse = EventlogDuringBlockAlarmUpsOutputFuse;

            bool oldAlarmUpsBatteryFault = AlarmUpsBatteryFault;
            bool oldBlockAlarmUpsBatteryFault = BlockAlarmUpsBatteryFault;
            byte? oldObjBlockAlarmUpsBatteryFaultObjectType = ObjBlockAlarmUpsBatteryFaultObjectType;
            Guid? oldObjBlockAlarmUpsBatteryFaultId = ObjBlockAlarmUpsBatteryFaultId;
            bool oldEventlogDuringBlockAlarmUpsBatteryFault = EventlogDuringBlockAlarmUpsBatteryFault;

            bool oldAlarmUpsBatteryFuse = AlarmUpsBatteryFuse;
            bool oldBlockAlarmUpsBatteryFuse = BlockAlarmUpsBatteryFuse;
            byte? oldObjBlockAlarmUpsBatteryFuseObjectType = ObjBlockAlarmUpsBatteryFuseObjectType;
            Guid? oldObjBlockAlarmUpsBatteryFuseId = ObjBlockAlarmUpsBatteryFuseId;
            bool oldEventlogDuringBlockAlarmUpsBatteryFuse = EventlogDuringBlockAlarmUpsBatteryFuse;

            bool oldAlarmUpsOvertemperature = AlarmUpsOvertemperature;
            bool oldBlockAlarmUpsOvertemperature = BlockAlarmUpsOvertemperature;
            byte? oldObjBlockAlarmUpsOvertemperatureObjectType = ObjBlockAlarmUpsOvertemperatureObjectType;
            Guid? oldObjBlockAlarmUpsOvertemperatureId = ObjBlockAlarmUpsOvertemperatureId;
            bool oldEventlogDuringBlockAlarmUpsOvertemperature = EventlogDuringBlockAlarmUpsOvertemperature;

            bool oldAlarmUpsTamperSabotage = AlarmUpsTamperSabotage;
            bool oldBlockAlarmUpsTamperSabotage = BlockAlarmUpsTamperSabotage;
            byte? oldObjBlockAlarmUpsTamperSabotageObjectType = ObjBlockAlarmUpsTamperSabotageObjectType;
            Guid? oldObjBlockAlarmUpsTamperSabotageId = ObjBlockAlarmUpsTamperSabotageId;
            bool oldEventlogDuringBlockAlarmUpsTamperSabotage = EventlogDuringBlockAlarmUpsTamperSabotage;

            bool oldAlarmFuseOnExtensionBoard = AlarmFuseOnExtensionBoard;
            bool oldBlockAlarmFuseOnExtensionBoard = BlockAlarmFuseOnExtensionBoard;
            byte? oldObjBlockAlarmFuseOnExtensionBoardObjectType = ObjBlockAlarmFuseOnExtensionBoardObjectType;
            Guid? oldObjBlockAlarmFuseOnExtensionBoardId = ObjBlockAlarmFuseOnExtensionBoardId;
            bool oldEventlogDuringBlockAlarmFuseOnExtensionBoard = EventlogDuringBlockAlarmFuseOnExtensionBoard;

            bool oldAlarmCcuCatUnreachable = AlarmCcuCatUnreachable;

            bool oldAlarmCcuTransferToArcTimedOut = AlarmCcuTransferToArcTimedOut;

            //DCU alarm settings
            bool oldAlarmDcuOffline = AlarmDcuOffline;
            bool oldBlockAlarmDcuOffline = BlockAlarmDcuOffline;
            byte? oldObjBlockAlarmDcuOfflineObjectType = ObjBlockAlarmDcuOfflineObjectType;
            Guid? oldObjBlockAlarmDcuOfflineId = ObjBlockAlarmDcuOfflineId;

            bool oldAlarmDCUTamperSabotage = AlarmDCUTamperSabotage;
            bool oldBlockAlarmDCUTamperSabotage = BlockAlarmDCUTamperSabotage;
            byte? oldObjBlockAlarmDCUTamperSabotageObjectType = ObjBlockAlarmDCUTamperSabotageObjectType;
            Guid? oldObjBlockAlarmDCUTamperSabotageId = ObjBlockAlarmDCUTamperSabotageId;
            bool oldEventlogDuringBlockAlarmDCUTamperSabotage = EventlogDuringBlockAlarmDCUTamperSabotage;

            //Door environment alarm settings
            bool oldAlarmDEDoorAjar = AlarmDEDoorAjar;
            bool oldBlockAlarmDEDoorAjar = BlockAlarmDEDoorAjar;
            byte? oldObjBlockAlarmDEDoorAjarObjectType = ObjBlockAlarmDEDoorAjarObjectType;
            Guid? oldObjBlockAlarmDEDoorAjarId = ObjBlockAlarmDEDoorAjarId;

            bool oldAlarmDEIntrusion = AlarmDEIntrusion;
            bool oldBlockAlarmDEIntrusion = BlockAlarmDEIntrusion;
            byte? oldObjBlockAlarmDEIntrusionObjectType = ObjBlockAlarmDEIntrusionObjectType;
            Guid? oldObjBlockAlarmDEIntrusionId = ObjBlockAlarmDEIntrusionId;

            bool oldAlarmDESabotage = AlarmDESabotage;
            bool oldBlockAlarmDESabotage = BlockAlarmDESabotage;
            byte? oldObjBlockAlarmDESabotageObjectType = ObjBlockAlarmDESabotageObjectType;
            Guid? oldObjBlockAlarmDESabotageId = ObjBlockAlarmDESabotageId;

            //Card reader alarm settings
            bool oldAlarmCrOffline = AlarmCrOffline;
            bool oldBlockAlarmCrOffline = BlockAlarmCrOffline;
            byte? oldObjBlockAlarmCrOfflineObjectType = ObjBlockAlarmCrOfflineObjectType;
            Guid? oldObjBlockAlarmCrOfflineId = ObjBlockAlarmCrOfflineId;
                
            bool oldAlarmCRTamperSabotage = AlarmCRTamperSabotage;
            bool oldBlockAlarmCRTamperSabotage = BlockAlarmCRTamperSabotage;
            byte? oldObjBlockAlarmCRTamperSabotageObjectType = ObjBlockAlarmCRTamperSabotageObjectType;
            Guid? oldObjBlockAlarmCRTamperSabotageId = ObjBlockAlarmCRTamperSabotageId;
            bool oldEventlogDuringBlockAlarmCRTamperSabotage = EventlogDuringBlockAlarmCRTamperSabotage;

            bool oldAlarmCrAccessDenied = AlarmCrAccessDenied;
            bool oldBlockAlarmCrAccessDenied = BlockAlarmCrAccessDenied;
            byte? oldObjBlockAlarmCrAccessDeniedObjectType = ObjBlockAlarmCrAccessDeniedObjectType;
            Guid? oldObjBlockAlarmCrAccessDeniedId = ObjBlockAlarmCrAccessDeniedId;
            bool oldEventlogDuringBlockAlarmCrAccessDenied = EventlogDuringBlockAlarmCrAccessDenied;

            bool oldAlarmCrUnknownCard = AlarmCrUnknownCard;
            bool oldBlockAlarmCrUnknownCard = BlockAlarmCrUnknownCard;
            byte? oldObjBlockAlarmCrUnknownCardObjectType = ObjBlockAlarmCrUnknownCardObjectType;
            Guid? oldObjBlockAlarmCrUnknownCardId = ObjBlockAlarmCrUnknownCardId;
            bool oldEventlogDuringBlockAlarmCrUnknownCard = EventlogDuringBlockAlarmCrUnknownCard;

            bool oldAlarmCrCardBlockedOrInactive = AlarmCrCardBlockedOrInactive;
            bool oldBlockAlarmCrCardBlockedOrInactive = BlockAlarmCrCardBlockedOrInactive;
            byte? oldObjBlockAlarmCrCardBlockedOrInactiveObjectType = ObjBlockAlarmCrCardBlockedOrInactiveObjectType;
            Guid? oldObjBlockAlarmCrCardBlockedOrInactiveId = ObjBlockAlarmCrCardBlockedOrInactiveId;
            bool oldEventlogDuringBlockAlarmCrCardBlockedOrInactive = EventlogDuringBlockAlarmCrCardBlockedOrInactive;

            bool oldAlarmCrInvalidPin = AlarmCrInvalidPin;
            bool oldBlockAlarmCrInvalidPin = BlockAlarmCrInvalidPin;
            byte? oldObjBlockAlarmCrInvalidPinObjectType = ObjBlockAlarmCrInvalidPinObjectType;
            Guid? oldObjBlockAlarmCrInvalidPinId = ObjBlockAlarmCrInvalidPinId;
            bool oldEventlogDuringBlockAlarmCrInvalidPin = EventlogDuringBlockAlarmCrInvalidPin;

            bool oldAlarmCrInvalidGin = AlarmCrInvalidGin;
            bool oldBlockAlarmCrInvalidGin = BlockAlarmCrInvalidGin;
            byte? oldObjBlockAlarmCrInvalidGinObjectType = ObjBlockAlarmCrInvalidGinObjectType;
            Guid? oldObjBlockAlarmCrInvalidGinId = ObjBlockAlarmCrInvalidGinId;
            bool oldEventlogDuringBlockAlarmCrInvalidGin = EventlogDuringBlockAlarmCrInvalidGin;

            bool oldAlarmCrInvalidEmergencyCode = AlarmCrInvalidEmergencyCode;
            bool oldBlockAlarmCrInvalidEmergencyCode = BlockAlarmCrInvalidEmergencyCode;
            byte? oldObjBlockAlarmCrInvalidEmergencyCodeObjectType = ObjBlockAlarmCrInvalidEmergencyCodeObjectType;
            Guid? oldObjBlockAlarmCrInvalidEmergencyCodeId = ObjBlockAlarmCrInvalidEmergencyCodeId;
            bool oldEventlogDuringBlockAlarmCrInvalidEmergencyCode = EventlogDuringBlockAlarmCrInvalidEmergencyCode;

            bool oldAlarmCrAccessPermitted = AlarmCrAccessPermitted;
            bool oldBlockAlarmCrAccessPermitted = BlockAlarmCrAccessPermitted;
            byte? oldObjBlockAlarmCrAccessPermittedObjectType = ObjBlockAlarmCrAccessPermittedObjectType;
            Guid? oldObjBlockAlarmCrAccessPermittedId = ObjBlockAlarmCrAccessPermittedId;

            bool oldInvalidPinRetriesLimitdEnabled = InvalidPinRetriesLimitEnabled;

            bool oldAlarmInvalidPinRetriesLimitReached = AlarmInvalidPinRetriesLimitReached;
            bool oldBlockAlarmInvalidPinRetriesLimitReached = BlockAlarmInvalidPinRetriesLimitReached;
            byte? oldObjBlockAlarmInvalidPinRetriesLimitReachedObjectType =
                ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType;
            Guid? oldObjBlockAlarmInvalidPinRetriesLimitReachedId =
                ObjBlockAlarmInvalidPinRetriesLimitReachedId;

            byte oldInvalidPinRetriesLimitReachedTimeout = InvalidPinRetriesLimitReachedTimeout;

            bool oldInvalidGinRetriesLimitdEnabled = InvalidGinRetriesLimitEnabled;

            bool oldAlarmInvalidGinRetriesLimitReached = AlarmInvalidGinRetriesLimitReached;
            bool oldBlockAlarmInvalidGinRetriesLimitReached = BlockAlarmInvalidGinRetriesLimitReached;
            byte? oldObjBlockAlarmInvalidGinRetriesLimitReachedObjectType =
                ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType;
            Guid? oldObjBlockAlarmInvalidGinRetriesLimitReachedId =
                ObjBlockAlarmInvalidGinRetriesLimitReachedId;

            bool oldAlarmAreaSetByOnOffObjectFailed = AlarmAreaSetByOnOffObjectFailed;
            bool oldBlockAlarmAreaSetByOnOffObjectFailed = BlockAlarmAreaSetByOnOffObjectFailed;
            byte? oldObjBlockAlarmAreaSetByOnOffObjectFailedObjectType =
                ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType;
            Guid? oldObjBlockAlarmAreaSetByOnOffObjectFailedId =
                ObjBlockAlarmAreaSetByOnOffObjectFailedId;

            _deviceAlarmSettings = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.DevicesAlarmSetting, idDas) as DB.DevicesAlarmSetting;

            //CCU alarms settings
            if (oldAlarmCCUTamperSabotage != AlarmCCUTamperSabotage || oldBlockAlarmCCUTamperSabotage != BlockAlarmCCUTamperSabotage ||
                oldObjBlockAlarmCCUTamperSabotageObjectType != ObjBlockAlarmCCUTamperSabotageObjectType ||
                oldObjBlockAlarmCCUTamperSabotageId != ObjBlockAlarmCCUTamperSabotageId ||
                oldEventlogDuringBlockAlarmCCUTamperSabotage != EventlogDuringBlockAlarmCCUTamperSabotage)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CCU_TamperSabotage,
                    AlarmCCUTamperSabotage,
                    BlockAlarmCCUTamperSabotage,
                    ObjBlockAlarmCCUTamperSabotageId,
                    ObjBlockAlarmCCUTamperSabotageObjectType,
                    EventlogDuringBlockAlarmCCUTamperSabotage);
            }

            if (oldAlarmPrimaryPowerMissing != AlarmPrimaryPowerMissing || oldBlockAlarmPrimaryPowerMissing != BlockAlarmPrimaryPowerMissing ||
                oldObjBlockAlarmPrimaryPowerMissingObjectType != ObjBlockAlarmPrimaryPowerMissingObjectType ||
                oldObjBlockAlarmPrimaryPowerMissingId != ObjBlockAlarmPrimaryPowerMissingId ||
                oldEventlogDuringBlockAlarmPrimaryPowerMissing != EventlogDuringBlockAlarmPrimaryPowerMissing)
            {

                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CCU_PrimaryPowerMissing,
                    AlarmPrimaryPowerMissing,
                    BlockAlarmPrimaryPowerMissing,
                    ObjBlockAlarmPrimaryPowerMissingId,
                    ObjBlockAlarmPrimaryPowerMissingObjectType,
                    EventlogDuringBlockAlarmPrimaryPowerMissing);
            }

            if (oldAlarmBatteryIsLow != AlarmBatteryIsLow || oldBlockAlarmBatteryIsLow != BlockAlarmBatteryIsLow ||
                oldObjBlockAlarmBatteryIsLowObjectType != ObjBlockAlarmBatteryIsLowObjectType ||
                oldObjBlockAlarmBatteryIsLowId != ObjBlockAlarmBatteryIsLowId ||
                oldEventlogDuringBlockAlarmBatteryIsLow != EventlogDuringBlockAlarmBatteryIsLow)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CCU_BatteryLow,
                    AlarmBatteryIsLow,
                    BlockAlarmBatteryIsLow,
                    ObjBlockAlarmBatteryIsLowId,
                    ObjBlockAlarmBatteryIsLowObjectType,
                    EventlogDuringBlockAlarmBatteryIsLow);
            }

            if (oldAlarmUpsOutputFuse != AlarmUpsOutputFuse
                || oldBlockAlarmUpsOutputFuse != BlockAlarmUpsOutputFuse
                || oldObjBlockAlarmUpsOutputFuseObjectType != ObjBlockAlarmUpsOutputFuseObjectType
                || oldObjBlockAlarmUpsOutputFuseId != ObjBlockAlarmUpsOutputFuseId
                || oldEventlogDuringBlockAlarmUpsOutputFuse != EventlogDuringBlockAlarmUpsOutputFuse)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_Ups_OutputFuse,
                    AlarmUpsOutputFuse,
                    BlockAlarmUpsOutputFuse,
                    ObjBlockAlarmUpsOutputFuseId,
                    ObjBlockAlarmUpsOutputFuseObjectType,
                    EventlogDuringBlockAlarmUpsOutputFuse);
            }

            if (oldAlarmUpsBatteryFault != AlarmUpsBatteryFault
                || oldBlockAlarmUpsBatteryFault != BlockAlarmUpsBatteryFault
                || oldObjBlockAlarmUpsBatteryFaultObjectType != ObjBlockAlarmUpsBatteryFaultObjectType
                || oldObjBlockAlarmUpsBatteryFaultId != ObjBlockAlarmUpsBatteryFaultId
                || oldEventlogDuringBlockAlarmUpsBatteryFault != EventlogDuringBlockAlarmUpsBatteryFault)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_Ups_BatteryFault,
                    AlarmUpsBatteryFault,
                    BlockAlarmUpsBatteryFault,
                    ObjBlockAlarmUpsBatteryFaultId,
                    ObjBlockAlarmUpsBatteryFaultObjectType,
                    EventlogDuringBlockAlarmUpsBatteryFault);
            }

            if (oldAlarmUpsBatteryFuse != AlarmUpsBatteryFuse
                || oldBlockAlarmUpsBatteryFuse != BlockAlarmUpsBatteryFuse
                || oldObjBlockAlarmUpsBatteryFuseObjectType != ObjBlockAlarmUpsBatteryFuseObjectType
                || oldObjBlockAlarmUpsBatteryFuseId != ObjBlockAlarmUpsBatteryFuseId
                || oldEventlogDuringBlockAlarmUpsBatteryFuse != EventlogDuringBlockAlarmUpsBatteryFuse)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_Ups_BatteryFuse,
                    AlarmUpsBatteryFuse,
                    BlockAlarmUpsBatteryFuse,
                    ObjBlockAlarmUpsBatteryFuseId,
                    ObjBlockAlarmUpsBatteryFuseObjectType,
                    EventlogDuringBlockAlarmUpsBatteryFuse);
            }

            if (oldAlarmUpsOvertemperature != AlarmUpsOvertemperature
                || oldBlockAlarmUpsOvertemperature != BlockAlarmUpsOvertemperature
                || oldObjBlockAlarmUpsOvertemperatureObjectType != ObjBlockAlarmUpsOvertemperatureObjectType
                || oldObjBlockAlarmUpsOvertemperatureId != ObjBlockAlarmUpsOvertemperatureId
                || oldEventlogDuringBlockAlarmUpsOvertemperature != EventlogDuringBlockAlarmUpsOvertemperature)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_Ups_Overtemperature,
                    AlarmUpsOvertemperature,
                    BlockAlarmUpsOvertemperature,
                    ObjBlockAlarmUpsOvertemperatureId,
                    ObjBlockAlarmUpsOvertemperatureObjectType,
                    EventlogDuringBlockAlarmUpsOvertemperature);
            }

            if (oldAlarmUpsTamperSabotage != AlarmUpsTamperSabotage
                || oldBlockAlarmUpsTamperSabotage != BlockAlarmUpsTamperSabotage
                || oldObjBlockAlarmUpsTamperSabotageObjectType != ObjBlockAlarmUpsTamperSabotageObjectType
                || oldObjBlockAlarmUpsTamperSabotageId != ObjBlockAlarmUpsTamperSabotageId
                || oldEventlogDuringBlockAlarmUpsTamperSabotage != EventlogDuringBlockAlarmUpsTamperSabotage)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_Ups_TamperSabotage,
                    AlarmUpsTamperSabotage,
                    BlockAlarmUpsTamperSabotage,
                    ObjBlockAlarmUpsTamperSabotageId,
                    ObjBlockAlarmUpsTamperSabotageObjectType,
                    EventlogDuringBlockAlarmUpsTamperSabotage);
            }

            if (oldAlarmFuseOnExtensionBoard != AlarmFuseOnExtensionBoard ||
                oldBlockAlarmFuseOnExtensionBoard != BlockAlarmFuseOnExtensionBoard ||
                oldObjBlockAlarmFuseOnExtensionBoardObjectType != ObjBlockAlarmFuseOnExtensionBoardObjectType ||
                oldObjBlockAlarmFuseOnExtensionBoardId != ObjBlockAlarmFuseOnExtensionBoardId ||
                oldEventlogDuringBlockAlarmFuseOnExtensionBoard != EventlogDuringBlockAlarmFuseOnExtensionBoard)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CCU_ExtFuse,
                    AlarmFuseOnExtensionBoard,
                    BlockAlarmFuseOnExtensionBoard,
                    ObjBlockAlarmFuseOnExtensionBoardId,
                    ObjBlockAlarmFuseOnExtensionBoardObjectType,
                    EventlogDuringBlockAlarmFuseOnExtensionBoard);
            }

            if (oldAlarmCcuCatUnreachable != AlarmCcuCatUnreachable)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_CatUnreachable,
                    AlarmCcuCatUnreachable,
                    false,
                    null,
                    null,
                    true);
            }

            if (oldAlarmCcuTransferToArcTimedOut != AlarmCcuTransferToArcTimedOut)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.Ccu_TransferToArcTimedOut,
                    AlarmCcuTransferToArcTimedOut,
                    false,
                    null,
                    null,
                    true);
            }

            //DCU alarms settings
            if (oldAlarmDcuOffline != AlarmDcuOffline
                || oldBlockAlarmDcuOffline != BlockAlarmDcuOffline
                || oldObjBlockAlarmDcuOfflineId != ObjBlockAlarmDcuOfflineId
                || oldObjBlockAlarmDcuOfflineObjectType != ObjBlockAlarmDcuOfflineObjectType)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.DCU_Offline,
                    AlarmDcuOffline,
                    BlockAlarmDcuOffline,
                    ObjBlockAlarmDcuOfflineId,
                    ObjBlockAlarmDcuOfflineObjectType,
                    true);
            }

            if (oldAlarmDCUTamperSabotage != AlarmDCUTamperSabotage ||
                oldBlockAlarmDCUTamperSabotage != BlockAlarmDCUTamperSabotage ||
                oldObjBlockAlarmDCUTamperSabotageObjectType != ObjBlockAlarmDCUTamperSabotageObjectType ||
                oldObjBlockAlarmDCUTamperSabotageId != ObjBlockAlarmDCUTamperSabotageId ||
                oldEventlogDuringBlockAlarmDCUTamperSabotage != EventlogDuringBlockAlarmDCUTamperSabotage)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.DCU_TamperSabotage,
                    AlarmDCUTamperSabotage,
                    BlockAlarmDCUTamperSabotage,
                    ObjBlockAlarmDCUTamperSabotageId,
                    ObjBlockAlarmDCUTamperSabotageObjectType,
                    EventlogDuringBlockAlarmDCUTamperSabotage);
            }

            //Door environment alarm settings
            if (oldAlarmDEDoorAjar != AlarmDEDoorAjar || oldBlockAlarmDEDoorAjar != BlockAlarmDEDoorAjar ||
                oldObjBlockAlarmDEDoorAjarObjectType != ObjBlockAlarmDEDoorAjarObjectType ||
                oldObjBlockAlarmDEDoorAjarId != ObjBlockAlarmDEDoorAjarId)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.DoorEnvironment_DoorAjar,
                    AlarmDEDoorAjar,
                    BlockAlarmDEDoorAjar,
                    ObjBlockAlarmDEDoorAjarId,
                    ObjBlockAlarmDEDoorAjarObjectType,
                    true);
            }

            if (oldAlarmDEIntrusion != AlarmDEIntrusion || oldBlockAlarmDEIntrusion != BlockAlarmDEIntrusion ||
                oldObjBlockAlarmDEIntrusionObjectType != ObjBlockAlarmDEIntrusionObjectType ||
                oldObjBlockAlarmDEIntrusionId != ObjBlockAlarmDEIntrusionId)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.DoorEnvironment_Intrusion,
                    AlarmDEIntrusion,
                    BlockAlarmDEIntrusion,
                    ObjBlockAlarmDEIntrusionId,
                    ObjBlockAlarmDEIntrusionObjectType,
                    true);
            }

            if (oldAlarmDESabotage != AlarmDESabotage || oldBlockAlarmDESabotage != BlockAlarmDESabotage ||
                oldObjBlockAlarmDESabotageObjectType != ObjBlockAlarmDESabotageObjectType ||
                oldObjBlockAlarmDESabotageId != ObjBlockAlarmDESabotageId)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.DoorEnvironment_Sabotage,
                    AlarmDESabotage,
                    BlockAlarmDESabotage,
                    ObjBlockAlarmDESabotageId,
                    ObjBlockAlarmDESabotageObjectType,
                    true);
            }

            //Card reader alarm settings
            if (oldAlarmCrOffline != AlarmCrOffline
                || oldBlockAlarmCrOffline != BlockAlarmCrOffline
                || oldObjBlockAlarmCrOfflineId != ObjBlockAlarmCrOfflineId
                || oldObjBlockAlarmCrOfflineObjectType != ObjBlockAlarmCrOfflineObjectType)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_Offline,
                    AlarmCrOffline,
                    BlockAlarmCrOffline,
                    ObjBlockAlarmCrOfflineId,
                    ObjBlockAlarmCrOfflineObjectType,
                    true);
            }

            if (oldAlarmCRTamperSabotage != AlarmCRTamperSabotage || oldBlockAlarmCRTamperSabotage != BlockAlarmCRTamperSabotage ||
                oldObjBlockAlarmCRTamperSabotageObjectType != ObjBlockAlarmCRTamperSabotageObjectType ||
                oldObjBlockAlarmCRTamperSabotageId != ObjBlockAlarmCRTamperSabotageId ||
                oldEventlogDuringBlockAlarmCRTamperSabotage != EventlogDuringBlockAlarmCRTamperSabotage)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_TamperSabotage,
                    AlarmCRTamperSabotage,
                    BlockAlarmCRTamperSabotage,
                    ObjBlockAlarmCRTamperSabotageId,
                    ObjBlockAlarmCRTamperSabotageObjectType,
                    EventlogDuringBlockAlarmCRTamperSabotage);
            }

            if (oldAlarmCrAccessDenied != AlarmCrAccessDenied
                || oldBlockAlarmCrAccessDenied != BlockAlarmCrAccessDenied
                || oldObjBlockAlarmCrAccessDeniedId != ObjBlockAlarmCrAccessDeniedId
                || oldObjBlockAlarmCrAccessDeniedObjectType != ObjBlockAlarmCrAccessDeniedObjectType
                || oldEventlogDuringBlockAlarmCrAccessDenied != EventlogDuringBlockAlarmCrAccessDenied)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_AccessDenied,
                    AlarmCrAccessDenied,
                    BlockAlarmCrAccessDenied,
                    ObjBlockAlarmCrAccessDeniedId,
                    ObjBlockAlarmCrAccessDeniedObjectType,
                    EventlogDuringBlockAlarmCrAccessDenied);
            }

            if (oldAlarmCrUnknownCard != AlarmCrUnknownCard
                || oldBlockAlarmCrUnknownCard != BlockAlarmCrUnknownCard
                || oldObjBlockAlarmCrUnknownCardId != ObjBlockAlarmCrUnknownCardId
                || oldObjBlockAlarmCrUnknownCardObjectType != ObjBlockAlarmCrUnknownCardObjectType
                || oldEventlogDuringBlockAlarmCrUnknownCard != EventlogDuringBlockAlarmCrUnknownCard)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_UnknownCard,
                    AlarmCrUnknownCard,
                    BlockAlarmCrUnknownCard,
                    ObjBlockAlarmCrUnknownCardId,
                    ObjBlockAlarmCrUnknownCardObjectType,
                    EventlogDuringBlockAlarmCrUnknownCard);
            }

            if (oldAlarmCrCardBlockedOrInactive != AlarmCrCardBlockedOrInactive
                || oldBlockAlarmCrCardBlockedOrInactive != BlockAlarmCrCardBlockedOrInactive
                || oldObjBlockAlarmCrCardBlockedOrInactiveId != ObjBlockAlarmCrCardBlockedOrInactiveId
                || oldObjBlockAlarmCrCardBlockedOrInactiveObjectType != ObjBlockAlarmCrCardBlockedOrInactiveObjectType
                || oldEventlogDuringBlockAlarmCrCardBlockedOrInactive != EventlogDuringBlockAlarmCrCardBlockedOrInactive)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_CardBlockedOrInactive,
                    AlarmCrCardBlockedOrInactive,
                    BlockAlarmCrCardBlockedOrInactive,
                    ObjBlockAlarmCrCardBlockedOrInactiveId,
                    ObjBlockAlarmCrCardBlockedOrInactiveObjectType,
                    EventlogDuringBlockAlarmCrCardBlockedOrInactive);
            }

            if (oldAlarmCrInvalidPin != AlarmCrInvalidPin
                || oldBlockAlarmCrInvalidPin != BlockAlarmCrInvalidPin
                || oldObjBlockAlarmCrInvalidPinId != ObjBlockAlarmCrInvalidPinId
                || oldObjBlockAlarmCrInvalidPinObjectType != ObjBlockAlarmCrInvalidPinObjectType
                || oldEventlogDuringBlockAlarmCrInvalidPin != EventlogDuringBlockAlarmCrInvalidPin)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_InvalidPIN,
                    AlarmCrInvalidPin,
                    BlockAlarmCrInvalidPin,
                    ObjBlockAlarmCrInvalidPinId,
                    ObjBlockAlarmCrInvalidPinObjectType,
                    EventlogDuringBlockAlarmCrInvalidPin);
            }

            if (oldAlarmCrInvalidGin != AlarmCrInvalidGin
                || oldBlockAlarmCrInvalidGin != BlockAlarmCrInvalidGin
                || oldObjBlockAlarmCrInvalidGinId != ObjBlockAlarmCrInvalidGinId
                || oldObjBlockAlarmCrInvalidGinObjectType != ObjBlockAlarmCrInvalidGinObjectType
                || oldEventlogDuringBlockAlarmCrInvalidGin != EventlogDuringBlockAlarmCrInvalidGin)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_InvalidCode,
                    AlarmCrInvalidGin,
                    BlockAlarmCrInvalidGin,
                    ObjBlockAlarmCrInvalidGinId,
                    ObjBlockAlarmCrInvalidGinObjectType,
                    EventlogDuringBlockAlarmCrInvalidGin);
            }

            if (oldAlarmCrInvalidEmergencyCode != AlarmCrInvalidEmergencyCode
                || oldBlockAlarmCrInvalidEmergencyCode != BlockAlarmCrInvalidEmergencyCode
                || oldObjBlockAlarmCrInvalidEmergencyCodeId != ObjBlockAlarmCrInvalidEmergencyCodeId
                || oldObjBlockAlarmCrInvalidEmergencyCodeObjectType != ObjBlockAlarmCrInvalidEmergencyCodeObjectType
                || oldEventlogDuringBlockAlarmCrInvalidEmergencyCode != EventlogDuringBlockAlarmCrInvalidEmergencyCode)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_InvalidEmergencyCode,
                    AlarmCrInvalidEmergencyCode,
                    BlockAlarmCrInvalidEmergencyCode,
                    ObjBlockAlarmCrInvalidEmergencyCodeId,
                    ObjBlockAlarmCrInvalidEmergencyCodeObjectType,
                    EventlogDuringBlockAlarmCrInvalidEmergencyCode);
            }

            if (oldAlarmCrAccessPermitted != AlarmCrAccessPermitted
                || oldBlockAlarmCrAccessPermitted != BlockAlarmCrAccessPermitted
                || oldObjBlockAlarmCrAccessPermittedId != ObjBlockAlarmCrAccessPermittedId
                || oldObjBlockAlarmCrAccessPermittedObjectType != ObjBlockAlarmCrAccessPermittedObjectType)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_AccessPermitted,
                    AlarmCrAccessPermitted,
                    BlockAlarmCrAccessPermitted,
                    ObjBlockAlarmCrAccessPermittedId,
                    ObjBlockAlarmCrAccessPermittedObjectType,
                    true);
            }

            if (oldInvalidPinRetriesLimitdEnabled != InvalidPinRetriesLimitEnabled
                && !InvalidPinRetriesLimitEnabled)
            {
                CardPinAccessManager.Singleton.Reset();
            }

            if (oldAlarmInvalidPinRetriesLimitReached != AlarmInvalidPinRetriesLimitReached
                || oldBlockAlarmInvalidPinRetriesLimitReached != BlockAlarmInvalidPinRetriesLimitReached
                || oldObjBlockAlarmInvalidPinRetriesLimitReachedId != ObjBlockAlarmInvalidPinRetriesLimitReachedId
                ||oldObjBlockAlarmInvalidPinRetriesLimitReachedObjectType != ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached,
                    AlarmInvalidPinRetriesLimitReached,
                    BlockAlarmInvalidPinRetriesLimitReached,
                    ObjBlockAlarmInvalidPinRetriesLimitReachedId,
                    ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType,
                    true);
            }

            if (oldInvalidPinRetriesLimitReachedTimeout != InvalidPinRetriesLimitReachedTimeout)
                CardPinAccessManager.Singleton.SetInvalidPinRetriesLimitReachedTimeout(InvalidPinRetriesLimitReachedTimeout);

            if (oldInvalidGinRetriesLimitdEnabled != InvalidGinRetriesLimitEnabled
                && !InvalidGinRetriesLimitEnabled)
            {
                CardReaders.Singleton.StopTimeoutAndResetInvalidGinRetriesLimitReached();
            }

            if (oldAlarmInvalidGinRetriesLimitReached != AlarmInvalidGinRetriesLimitReached
                || oldBlockAlarmInvalidGinRetriesLimitReached != BlockAlarmInvalidGinRetriesLimitReached
                || oldObjBlockAlarmInvalidGinRetriesLimitReachedId != ObjBlockAlarmInvalidGinRetriesLimitReachedId
                || oldObjBlockAlarmInvalidGinRetriesLimitReachedObjectType != ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached,
                    AlarmInvalidGinRetriesLimitReached,
                    BlockAlarmInvalidGinRetriesLimitReached,
                    ObjBlockAlarmInvalidGinRetriesLimitReachedId,
                    ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType,
                    true);
            }

            if (oldAlarmAreaSetByOnOffObjectFailed != AlarmAreaSetByOnOffObjectFailed
                || oldBlockAlarmAreaSetByOnOffObjectFailed != BlockAlarmAreaSetByOnOffObjectFailed
                || oldObjBlockAlarmAreaSetByOnOffObjectFailedId !=
                ObjBlockAlarmAreaSetByOnOffObjectFailedId
                || oldObjBlockAlarmAreaSetByOnOffObjectFailedObjectType !=
                ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType)
            {
                BlockedAlarmsManager.Singleton.ConfigureGeneralBlocking(
                    AlarmType.AlarmArea_SetByOnOffObjectFailed,
                    AlarmAreaSetByOnOffObjectFailed,
                    BlockAlarmAreaSetByOnOffObjectFailed,
                    ObjBlockAlarmAreaSetByOnOffObjectFailedId,
                    ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType,
                    true);
            }

            CatAlarmsManager.Singleton.ConfigureGeneralAlarmArcs(AlarmTypeAndIdAlarmArcs);
        }

        // Alarms for CCU

        public bool AlarmCCUTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCCUTamperSabotage;

                return true;
            }
        }

        public bool BlockAlarmCCUTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCCUTamperSabotage;

                return false;
            }
        }

        public byte? ObjBlockAlarmCCUTamperSabotageObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCCUTamperSabotageObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCCUTamperSabotageId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCCUTamperSabotageId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCCUTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCCUTamperSabotage;

                return false;
            }
        }

        public bool AlarmPrimaryPowerMissing
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmPrimaryPowerMissing;

                return true;
            }
        }

        public bool BlockAlarmPrimaryPowerMissing
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmPrimaryPowerMissing;

                return false;
            }
        }

        public byte? ObjBlockAlarmPrimaryPowerMissingObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmPrimaryPowerMissingObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmPrimaryPowerMissingId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmPrimaryPowerMissingId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmPrimaryPowerMissing
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmPrimaryPowerMissing;

                return false;
            }
        }

        public bool AlarmBatteryIsLow
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmBatteryIsLow;

                return true;
            }
        }

        public bool BlockAlarmBatteryIsLow
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmBatteryIsLow;

                return false;
            }
        }

        public byte? ObjBlockAlarmBatteryIsLowObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmBatteryIsLowObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmBatteryIsLowId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmBatteryIsLowId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmBatteryIsLow
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmBatteryIsLow;

                return false;
            }
        }

        public bool AlarmUpsOutputFuse
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmUpsOutputFuse;

                return true;
            }
        }

        public bool BlockAlarmUpsOutputFuse
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmUpsOutputFuse;

                return false;
            }
        }

        public byte? ObjBlockAlarmUpsOutputFuseObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsOutputFuseObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmUpsOutputFuseId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsOutputFuseId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmUpsOutputFuse
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmUpsOutputFuse;

                return false;
            }
        }

        public bool AlarmUpsBatteryFault
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmUpsBatteryFault;

                return true;
            }
        }

        public bool BlockAlarmUpsBatteryFault
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmUpsBatteryFault;

                return false;
            }
        }

        public byte? ObjBlockAlarmUpsBatteryFaultObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsBatteryFaultObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmUpsBatteryFaultId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsBatteryFaultId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmUpsBatteryFault
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmUpsBatteryFault;

                return false;
            }
        }

        public bool AlarmUpsBatteryFuse
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmUpsBatteryFuse;

                return true;
            }
        }

        public bool BlockAlarmUpsBatteryFuse
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmUpsBatteryFuse;

                return false;
            }
        }

        public byte? ObjBlockAlarmUpsBatteryFuseObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsBatteryFuseObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmUpsBatteryFuseId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsBatteryFuseId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmUpsBatteryFuse
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmUpsBatteryFuse;

                return false;
            }
        }

        public bool AlarmUpsOvertemperature
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmUpsOvertemperature;

                return true;
            }
        }

        public bool BlockAlarmUpsOvertemperature
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmUpsOvertemperature;

                return false;
            }
        }

        public byte? ObjBlockAlarmUpsOvertemperatureObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsOvertemperatureObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmUpsOvertemperatureId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsOvertemperatureId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmUpsOvertemperature
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmUpsOvertemperature;

                return false;
            }
        }

        public bool AlarmUpsTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmUpsTamperSabotage;

                return true;
            }
        }

        public bool BlockAlarmUpsTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmUpsTamperSabotage;

                return false;
            }
        }

        public byte? ObjBlockAlarmUpsTamperSabotageObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsTamperSabotageObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmUpsTamperSabotageId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmUpsTamperSabotageId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmUpsTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmUpsTamperSabotage;

                return false;
            }
        }

        public bool AlarmFuseOnExtensionBoard
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmFuseOnExtensionBoard;

                return true;
            }
        }

        public bool AlarmCcuCatUnreachable
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCcuCatUnreachable;

                return true;
            }
        }

        public bool AlarmCcuTransferToArcTimedOut
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCcuTransferToArcTimedOut;

                return true;
            }
        }

        public bool BlockAlarmFuseOnExtensionBoard
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmFuseOnExtensionBoard;

                return false;
            }
        }

        public byte? ObjBlockAlarmFuseOnExtensionBoardObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmFuseOnExtensionBoardObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmFuseOnExtensionBoardId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmFuseOnExtensionBoardId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmFuseOnExtensionBoard
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmFuseOnExtensionBoard;

                return false;
            }
        }

        // Alarms for DCU

        public bool AlarmDcuOffline
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmDCUOffline;

                return true;
            }
        }

        public bool BlockAlarmDcuOffline
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmDCUOffline;

                return false;
            }
        }

        public byte? ObjBlockAlarmDcuOfflineObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDCUOfflineObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmDcuOfflineId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDCUOfflineId;

                return null;
            }
        }

        public bool AlarmDCUTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmDCUTamperSabotage;

                return true;
            }
        }

        public bool BlockAlarmDCUTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmDCUTamperSabotage;

                return false;
            }
        }

        public byte? ObjBlockAlarmDCUTamperSabotageObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDCUTamperSabotageObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmDCUTamperSabotageId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDCUTamperSabotageId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmDCUTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmDCUTamperSabotage;

                return false;
            }
        }

        // Alarms for door environment

        public bool AlarmDEDoorAjar
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmDEDoorAjar;
                return true;
            }
        }

        public bool BlockAlarmDEDoorAjar
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmDEDoorAjar;

                return false;
            }
        }

        public byte? ObjBlockAlarmDEDoorAjarObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDEDoorAjarObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmDEDoorAjarId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDEDoorAjarId;

                return null;
            }
        }

        public bool AlarmDEIntrusion
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmDEIntrusion;
                return true;
            }
        }

        public bool BlockAlarmDEIntrusion
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmDEIntrusion;

                return false;
            }
        }

        public byte? ObjBlockAlarmDEIntrusionObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDEIntrusionObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmDEIntrusionId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDEIntrusionId;

                return null;
            }
        }

        public bool AlarmDESabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmDESabotage;
                return true;
            }
        }

        public bool BlockAlarmDESabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmDESabotage;

                return false;
            }
        }

        public byte? ObjBlockAlarmDESabotageObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDESabotageObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmDESabotageId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmDESabotageId;

                return null;
            }
        }

        public DB.SecurityLevel SecurityLevelForEnterToMenu
        {
            get
            {
                return _deviceAlarmSettings != null
                    ? (DB.SecurityLevel)_deviceAlarmSettings.SecurityLevelForEnterToMenu
                    : DB.SecurityLevel.CardPIN;
            }
        }

        public int GinLengthForEnterToMenu
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.GinLengthForEnterToMenu
                        : 0;
            }
        }

        public Guid GuidSecurityTimeZoneForEnterToMenu
        {
            get
            {
                return _deviceAlarmSettings != null
                    ? _deviceAlarmSettings.GuidSecurityTimeZoneForEnterToMenu
                    : Guid.Empty;
            }
        }

        public Guid GuidSecurityDailyPlanForEnterToMenu
        {
            get
            {
                return _deviceAlarmSettings != null
                    ? _deviceAlarmSettings.GuidSecurityDailyPlanForEnterToMenu
                    : Guid.Empty;
            }
        }

        public string GinForEnterToMenu
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.GinForEnterToMenu
                        : string.Empty;
            }
        }
        // Alarms for card reader

        public bool AlarmCrOffline
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCROffline;

                return true;
            }
        }

        public bool BlockAlarmCrOffline
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCROffline;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrOfflineObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCROfflineObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrOfflineId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCROfflineId;

                return null;
            }
        }

        public bool AlarmCRTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCRTamperSabotage;

                return true;
            }
        }

        public bool BlockAlarmCRTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCRTamperSabotage;

                return false;
            }
        }

        public byte? ObjBlockAlarmCRTamperSabotageObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCRTamperSabotageObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCRTamperSabotageId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCRTamperSabotageId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCRTamperSabotage
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCRTamperSabotage;

                return false;
            }
        }

        public bool AlarmCrAccessDenied
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrAccessDenied;

                return true;
            }
        }

        public bool BlockAlarmCrAccessDenied
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrAccessDenied;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrAccessDeniedObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrAccessDeniedObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrAccessDeniedId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrAccessDeniedId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCrAccessDenied
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCrAccessDenied;

                return false;
            }
        }

        public bool AlarmCrAccessPermitted
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrAccessPermitted;

                return true;
            }
        }

        public bool BlockAlarmCrAccessPermitted
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrAccessPermitted;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrAccessPermittedObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrAccessPermittedObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrAccessPermittedId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrAccessPermittedId;

                return null;
            }
        }

        public bool AlarmCrCardBlockedOrInactive
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrCardBlockedOrInactive;

                return true;
            }
        }

        public bool BlockAlarmCrCardBlockedOrInactive
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrCardBlockedOrInactive;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrCardBlockedOrInactiveObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrCardBlockedOrInactiveObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrCardBlockedOrInactiveId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrCardBlockedOrInactiveId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCrCardBlockedOrInactive
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCrCardBlockedOrInactive;

                return false;
            }
        }

        public bool AlarmCrInvalidEmergencyCode
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrInvalidEmergencyCode;

                return true;
            }
        }

        public bool BlockAlarmCrInvalidEmergencyCode
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrInvalidEmergencyCode;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrInvalidEmergencyCodeObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrInvalidEmergencyCodeObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrInvalidEmergencyCodeId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrInvalidEmergencyCodeId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCrInvalidEmergencyCode
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCrInvalidEmergencyCode;

                return false;
            }
        }
       
        public bool AlarmCrInvalidGin
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrInvalidGin;

                return true;
            }
        }

        public bool BlockAlarmCrInvalidGin
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrInvalidGin;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrInvalidGinObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrInvalidGinObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrInvalidGinId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrInvalidGinId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCrInvalidGin
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCrInvalidGin;

                return false;
            }
        }

        public bool AlarmCrInvalidPin
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrInvalidPin;

                return true;
            }
        }

        public bool BlockAlarmCrInvalidPin
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrInvalidPin;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrInvalidPinObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrInvalidPinObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrInvalidPinId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrInvalidPinId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCrInvalidPin
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCrInvalidPin;

                return false;
            }
        }

        public bool AlarmCrUnknownCard
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AlarmCrUnknownCard;

                return true;
            }
        }

        public bool BlockAlarmCrUnknownCard
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.BlockAlarmCrUnknownCard;

                return false;
            }
        }

        public byte? ObjBlockAlarmCrUnknownCardObjectType
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrUnknownCardObjectType;

                return null;
            }
        }

        public Guid? ObjBlockAlarmCrUnknownCardId
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.ObjBlockAlarmCrUnknownCardId;

                return null;
            }
        }

        public bool EventlogDuringBlockAlarmCrUnknownCard
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.EventlogDuringBlockAlarmCrUnknownCard;

                return false;
            }
        }

        // Reporting

        public bool AllowAAToCRsReporting
        {
            get
            {
                if (_deviceAlarmSettings != null)
                    return _deviceAlarmSettings.AllowAAToCRsReporting;

                return false;
            }
            set
            {
                if (_deviceAlarmSettings != null)
                    _deviceAlarmSettings.AllowAAToCRsReporting = value;
            }
        }

        public bool AlarmInvalidPinRetriesLimitReached
        {
            get
            {
                return
                    _deviceAlarmSettings == null
                    || _deviceAlarmSettings.AlarmInvalidPinRetriesLimitReached;
            }
        }

        public bool BlockAlarmInvalidPinRetriesLimitReached
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                    && _deviceAlarmSettings.BlockAlarmInvalidPinRetriesLimitReached;
            }
        }

        public byte? ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType
                        : null;
            }
        }

        public Guid? ObjBlockAlarmInvalidPinRetriesLimitReachedId
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.ObjBlockAlarmInvalidPinRetriesLimitReachedId
                        : null;
            }
        }

        public bool AlarmInvalidGinRetriesLimitReached
        {
            get
            {
                return
                    _deviceAlarmSettings == null
                    || _deviceAlarmSettings.AlarmInvalidGinRetriesLimitReached;
            }
        }

        public bool BlockAlarmInvalidGinRetriesLimitReached
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                    && _deviceAlarmSettings.BlockAlarmInvalidGinRetriesLimitReached;
            }
        }

        public byte? ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType
                        : null;
            }
        }

        public Guid? ObjBlockAlarmInvalidGinRetriesLimitReachedId
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.ObjBlockAlarmInvalidGinRetriesLimitReachedId
                        : null;
            }
        }

        public bool AlarmAreaSetByOnOffObjectFailed
        {
            get
            {
                return
                    _deviceAlarmSettings == null
                    || _deviceAlarmSettings.AlarmAreaSetByOnOffObjectFailed;
            }
        }

        public bool BlockAlarmAreaSetByOnOffObjectFailed
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                    && _deviceAlarmSettings.BlockAlarmAreaSetByOnOffObjectFailed;
            }
        }

        public byte? ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType
                        : null;
            }
        }

        public Guid? ObjBlockAlarmAreaSetByOnOffObjectFailedId
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.ObjBlockAlarmAreaSetByOnOffObjectFailedId
                        : null;
            }
        }

        public bool InvalidPinRetriesLimitEnabled
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                    && _deviceAlarmSettings.InvalidPinRetriesLimitEnabled;
            }
        }

        public byte InvalidPinRetriesCount
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.InvalidPinRetriesCount
                        : (byte)3;
            }
        }

        public byte InvalidPinRetriesLimitReachedTimeout
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.InvalidPinRetriesLimitReachedTimeout
                        : (byte)5;
            }
        }

        public bool InvalidGinRetriesLimitEnabled
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                    && _deviceAlarmSettings.InvalidGinRetriesLimitEnabled;
            }
        }

        public byte InvalidGinRetriesCount
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.InvalidGinRetriesCount
                        : (byte)3;
            }
        }

        public byte InvalidGinRetriesLimitReachedTimeout
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.InvalidGinRetriesLimitReachedTimeout
                        : (byte)5;
            }
        }

        public ICollection<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs
        {
            get
            {
                return
                    _deviceAlarmSettings != null
                        ? _deviceAlarmSettings.AlarmTypeAndIdAlarmArcs
                        : null;
            }
        }
    }
}
