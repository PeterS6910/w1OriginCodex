using System;
using System.Collections.Generic;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(314)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DevicesAlarmSetting : IDbObject
    {
        [LwSerialize]
        public Guid IdDevicesAlarmSetting { get; set; }

        // Alarms for CCU
        [LwSerialize]
        public bool AlarmCCUTamperSabotage { get; set; }
        [LwSerialize]
        public bool BlockAlarmCCUTamperSabotage { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCCUTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCCUTamperSabotageId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCCUTamperSabotage { get; set; }

        [LwSerialize]
        public bool AlarmPrimaryPowerMissing { get; set; }
        [LwSerialize]
        public bool BlockAlarmPrimaryPowerMissing { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmPrimaryPowerMissingObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmPrimaryPowerMissingId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmPrimaryPowerMissing { get; set; }

        [LwSerialize]
        public bool AlarmBatteryIsLow { get; set; }
        [LwSerialize]
        public bool BlockAlarmBatteryIsLow { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmBatteryIsLowObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmBatteryIsLowId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmBatteryIsLow { get; set; }

        [LwSerialize]
        public  bool AlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public bool BlockAlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsOutputFuseObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsOutputFuseId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmUpsOutputFuse { get; set; }

        [LwSerialize]
        public bool AlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public bool BlockAlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsBatteryFaultObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsBatteryFaultId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmUpsBatteryFault { get; set; }

        [LwSerialize]
        public bool AlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public bool BlockAlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsBatteryFuseObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsBatteryFuseId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmUpsBatteryFuse { get; set; }

        [LwSerialize]
        public bool AlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public bool BlockAlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsOvertemperatureObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsOvertemperatureId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmUpsOvertemperature { get; set; }

        [LwSerialize]
        public bool AlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public bool BlockAlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsTamperSabotageId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmUpsTamperSabotage { get; set; }

        [LwSerialize]
        public bool AlarmFuseOnExtensionBoard { get; set; }
        [LwSerialize]
        public bool BlockAlarmFuseOnExtensionBoard { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmFuseOnExtensionBoardObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmFuseOnExtensionBoardId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmFuseOnExtensionBoard { get; set; }

        [LwSerialize]
        public virtual bool AlarmCcuCatUnreachable { get; set; }

        [LwSerialize]
        public virtual bool AlarmCcuTransferToArcTimedOut { get; set; }

        // Alarms for DCU
        [LwSerialize]
        public bool AlarmDCUOffline { get; set; }
        [LwSerialize]
        public bool BlockAlarmDCUOffline { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmDCUOfflineObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmDCUOfflineId { get; set; }

        [LwSerialize]
        public bool AlarmDCUTamperSabotage { get; set; }
        [LwSerialize]
        public bool BlockAlarmDCUTamperSabotage { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmDCUTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmDCUTamperSabotageId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmDCUTamperSabotage { get; set; }

        // Alarms for door environment
        [LwSerialize]
        public bool AlarmDEIntrusion { get; set; }
        [LwSerialize]
        public bool BlockAlarmDEIntrusion { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmDEIntrusionObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmDEIntrusionId { get; set; }

        [LwSerialize]
        public bool AlarmDEDoorAjar { get; set; }
        [LwSerialize]
        public bool BlockAlarmDEDoorAjar { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmDEDoorAjarObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmDEDoorAjarId { get; set; }

        [LwSerialize]
        public bool AlarmDESabotage { get; set; }
        [LwSerialize]
        public bool BlockAlarmDESabotage { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmDESabotageObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmDESabotageId { get; set; }

        // Card reader - security level for enter to menu
        [LwSerialize]
        public byte SecurityLevelForEnterToMenu { get; set; }
        [LwSerialize]
        public string GinForEnterToMenu { get; set; }
        [LwSerialize]
        public byte GinLengthForEnterToMenu { get; set; }
        [LwSerialize]
        public Guid GuidSecurityDailyPlanForEnterToMenu { get; set; }
        [LwSerialize]
        public Guid GuidSecurityTimeZoneForEnterToMenu { get; set; }

        // Alarms for card reader
        [LwSerialize]
        public bool AlarmCROffline { get; set; }
        [LwSerialize]
        public bool BlockAlarmCROffline { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCROfflineObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCROfflineId { get; set; }

        [LwSerialize]
        public bool AlarmCRTamperSabotage { get; set; }
        [LwSerialize]
        public bool BlockAlarmCRTamperSabotage { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCRTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCRTamperSabotageId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCRTamperSabotage { get; set; }

        [LwSerialize]
        public  bool AlarmCrAccessDenied { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrAccessDenied { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrAccessDeniedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrAccessDeniedId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCrAccessDenied { get; set; }

        [LwSerialize]
        public bool AlarmCrUnknownCard { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrUnknownCard { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrUnknownCardObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrUnknownCardId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCrUnknownCard { get; set; }

        [LwSerialize]
        public bool AlarmCrCardBlockedOrInactive { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrCardBlockedOrInactive { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrCardBlockedOrInactiveObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrCardBlockedOrInactiveId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCrCardBlockedOrInactive { get; set; }

        [LwSerialize]
        public bool AlarmCrInvalidPin { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrInvalidPin { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrInvalidPinObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrInvalidPinId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCrInvalidPin { get; set; }

        [LwSerialize]
        public bool AlarmInvalidPinRetriesLimitReached { get; set; }
        [LwSerialize]
        public bool BlockAlarmInvalidPinRetriesLimitReached { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmInvalidPinRetriesLimitReachedId { get; set; }

        [LwSerialize]
        public bool AlarmCrInvalidGin { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrInvalidGin { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrInvalidGinObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrInvalidGinId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCrInvalidGin { get; set; }

        [LwSerialize]
        public bool AlarmCrInvalidEmergencyCode { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrInvalidEmergencyCode { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrInvalidEmergencyCodeObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrInvalidEmergencyCodeId { get; set; }
        [LwSerialize]
        public bool EventlogDuringBlockAlarmCrInvalidEmergencyCode { get; set; }

        [LwSerialize]
        public bool AlarmCrAccessPermitted { get; set; }
        [LwSerialize]
        public bool BlockAlarmCrAccessPermitted { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCrAccessPermittedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCrAccessPermittedId { get; set; }

        [LwSerialize]
        public bool AllowAAToCRsReporting { get; set; }

        [LwSerialize]
        public bool AlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public bool BlockAlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmInvalidGinRetriesLimitReachedId { get; set; }

        [LwSerialize]
        public virtual bool AlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmAreaSetByOnOffObjectFailedId { get; set; }

        [LwSerialize]
        public bool InvalidPinRetriesLimitEnabled { get; set; }
        [LwSerialize]
        public byte InvalidPinRetriesCount { get; set; }
        [LwSerialize]
        public byte InvalidPinRetriesLimitReachedTimeout { get; set; }

        [LwSerialize]
        public bool InvalidGinRetriesLimitEnabled { get; set; }
        [LwSerialize]
        public byte InvalidGinRetriesCount { get; set; }
        [LwSerialize]
        public byte InvalidGinRetriesLimitReachedTimeout { get; set; }

        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public Guid GetGuid()
        {
            return IdDevicesAlarmSetting;
        }

        public Cgp.Globals.ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.DevicesAlarmSetting;
        }
    }
}
