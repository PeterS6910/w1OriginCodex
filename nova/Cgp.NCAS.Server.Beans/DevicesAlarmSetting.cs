using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(314)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class DevicesAlarmSetting : AOrmObjectWithVersion
    {
        public const string COLUMNIDDEVICESALARMSETTING = "IdDevicesAlarmSetting";
        public const string COLUMNNAME = "Name";

        public const string COLUMNALARMCCUOFFLINE = "AlarmCCUOffline";
        public const string COLUMN_BLOCK_ALARM_CCU_OFFLINE = "BlockAlarmCCUOffline";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_OFFLINE_OBJECT_TYPE = "ObjBlockAlarmCCUOfflineObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_OFFLINE_ID = "ObjBlockAlarmCCUOfflineId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_OFFLINE = "ObjBlockAlarmCCUOffline";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CCU_OFFLINE = "EventlogDuringBlockAlarmCCUOffline";

        public const string COLUMNALARMCCUUNCONFIGURED = "AlarmCCUUnconfigured";
        public const string COLUMN_BLOCK_ALARM_CCU_UNCONFIGURED = "BlockAlarmCCUUnconfigured";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_UNCONFIGURED_OBJECT_TYPE = "ObjBlockAlarmCCUUnconfiguredObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_UNCONFIGURED_ID = "ObjBlockAlarmCCUUnconfiguredId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_UNCONFIGURED = "ObjBlockAlarmCCUUnconfigured";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CCU_UNCONFIGURED = "EventlogDuringBlockAlarmCCUUnconfigured";

        public const string COLUMNALARMCCUCLOCKUNSYNCHRONIZED = "AlarmCCUClockUnsynchronized";
        public const string COLUMN_BLOCK_ALARM_CCU_CLOCK_UNSYNCHRONIZED = "BlockAlarmCCUClockUnsynchronized";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_CLOCK_UNSYNCHRONIZED_OBJECT_TYPE = "ObjBlockAlarmCCUClockUnsynchronizedObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_CLOCK_UNSYNCHRONIZED_ID = "ObjBlockAlarmCCUClockUnsynchronizedId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_CLOCK_UNSYNCHRONIZED = "ObjBlockAlarmCCUClockUnsynchronized";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CCU_CLOCK_UNSYNCHRONIZED = "EventlogDuringBlockAlarmCCUClockUnsynchronized";

        public const string COLUMNALARMCCUTAMPERSABOTAGE = "AlarmCCUTamperSabotage";
        public const string COLUMN_BLOCK_ALARM_CCU_TAMPER_SABOTAGE = "BlockAlarmCCUTamperSabotage";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_TAMPER_SABOTAGE_OBJECT_TYPE = "ObjBlockAlarmCCUTamperSabotageObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_TAMPER_SABOTAGE_ID = "ObjBlockAlarmCCUTamperSabotageId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CCU_TAMPER_SABOTAGE = "ObjBlockAlarmCCUTamperSabotage";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CCU_TAMPER_SABOTAGE = "EventlogDuringBlockAlarmCCUTamperSabotage";

        public const string COLUMNALARMRRIMARYPOWERMISSING = "AlarmPrimaryPowerMissing";
        public const string COLUMN_BLOCK_ALARM_PRIMARY_POWER_MISSING = "BlockAlarmPrimaryPowerMissing";
        public const string COLUMN_OBJ_BLOCK_ALARM_PRIMARY_POWER_MISSING_OBJECT_TYPE = "ObjBlockAlarmPrimaryPowerMissingObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_PRIMARY_POWER_MISSING_ID = "ObjBlockAlarmPrimaryPowerMissingId";
        public const string COLUMN_OBJ_BLOCK_ALARM_PRIMARY_POWER_MISSING = "ObjBlockAlarmPrimaryPowerMissing";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_PRIMARY_POWER_MISSING = "EventlogDuringBlockAlarmPrimaryPowerMissing";

        public const string COLUMNALARMBATTERYISLOW = "AlarmBatteryIsLow";
        public const string COLUMN_BLOCK_ALARM_BATTERY_IS_LOW = "BlockAlarmBatteryIsLow";
        public const string COLUMN_OBJ_BLOCK_ALARM_BATTERY_IS_LOW_OBJECT_TYPE = "ObjBlockAlarmBatteryIsLowObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_BATTERY_IS_LOW_ID = "ObjBlockAlarmBatteryIsLowId";
        public const string COLUMN_OBJ_BLOCK_ALARM_BATTERY_IS_LOW = "ObjBlockAlarmBatteryIsLow";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_BATTERY_IS_LOW = "EventlogDuringBlockAlarmBatteryIsLow";

        public const string COLUMNALARMFUSEONEXTENSIONBOARD = "AlarmFuseOnExtensionBoard";
        public const string COLUMN_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD = "BlockAlarmFuseOnExtensionBoard";
        public const string COLUMN_OBJ_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD_OBJECT_TYPE = "ObjBlockAlarmFuseOnExtensionBoardObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD_ID = "ObjBlockAlarmFuseOnExtensionBoardId";
        public const string COLUMN_OBJ_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD = "ObjBlockAlarmFuseOnExtensionBoard";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD = "EventlogDuringBlockAlarmFuseOnExtensionBoard";

        public const string COLUMN_ALARM_CCU_CAT_UNREACHABLE = "AlarmCcuCatUnreachable";

        public const string COLUMN_ALARM_CCU_TRANSFER_TO_ARC_TIMED_OUT = "AlarmCcuTransferToArcTimedOut";

        public const string COLUMNALARMDCUOFFLINE = "AlarmDCUOffline";
        public const string COLUMN_BLOCK_ALARM_DCU_OFFLINE = "BlockAlarmDCUOffline";
        public const string COLUMN_OBJ_BLOCK_ALARM_DCU_OFFLINE_OBJECT_TYPE = "ObjBlockAlarmDCUOfflineObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_DCU_OFFLINE_ID = "ObjBlockAlarmDCUOfflineId";
        public const string COLUMN_OBJ_BLOCK_ALARM_DCU_OFFLINE = "ObjBlockAlarmDCUOffline";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DCU_OFFLINE = "EventlogDuringBlockAlarmDCUOffline";

        public const string COLUMNALARMDCUTAMPERSABOTAGE = "AlarmDCUTamperSabotage";
        public const string COLUMN_BLOCK_ALARM_DCU_TAMPER_SABOTAGE = "BlockAlarmDCUTamperSabotage";
        public const string COLUMN_OBJ_BLOCK_ALARM_DCU_TAMPER_SABOTAGE_OBJECT_TYPE = "ObjBlockAlarmDCUTamperSabotageObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_DCU_TAMPER_SABOTAGE_ID = "ObjBlockAlarmDCUTamperSabotageId";
        public const string COLUMN_OBJ_BLOCK_ALARM_DCU_TAMPER_SABOTAGE = "ObjBlockAlarmDCUTamperSabotage";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DCU_TAMPER_SABOTAGE = "EventlogDuringBlockAlarmDCUTamperSabotage";

        public const string COLUMN_ALARM_DE_INTRUSION = "AlarmDEIntrusion";
        public const string COLUMN_BLOCK_ALARM_DE_INTRUSION = "BlockAlarmDEIntrusion";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_INTRUSION_OBJECT_TYPE = "ObjBlockAlarmDEIntrusionObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_INTRUSION_ID = "ObjBlockAlarmDEIntrusionId";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_INTRUSION = "ObjBlockAlarmDEIntrusion";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DE_INTRUSION = "EventlogDuringBlockAlarmDEIntrusion";

        public const string COLUMN_ALARM_DE_DOOR_AJAR = "AlarmDEDoorAjar";
        public const string COLUMN_BLOCK_ALARM_DE_DOOR_AJAR = "BlockAlarmDEDoorAjar";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_DOOR_AJAR_OBJECT_TYPE = "ObjBlockAlarmDEDoorAjarObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_DOOR_AJAR_ID = "ObjBlockAlarmDEDoorAjarId";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_DOOR_AJAR = "ObjBlockAlarmDEDoorAjar";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DE_DOOR_AJAR = "EventlogDuringBlockAlarmDEDoorAjar";

        public const string COLUMN_ALARM_DE_SABOTAGE = "AlarmDESabotage";
        public const string COLUMN_BLOCK_ALARM_DE_SABOTAGE = "BlockAlarmDESabotage";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_SABOTAGE_OBJECT_TYPE = "ObjBlockAlarmDESabotageObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_SABOTAGE_ID = "ObjBlockAlarmDESabotageId";
        public const string COLUMN_OBJ_BLOCK_ALARM_DE_SABOTAGE = "ObjBlockAlarmDESabotage";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DE_SABOTAGE = "EventlogDuringBlockAlarmDESabotage";

        public const string COLUMN_SECURITY_LEVEL_FOR_ENTER_TO_MENU = "SecurityLevelForEnterToMenu";
        public const string COLUMN_GIN_FOR_ENTER_TO_MENU = "GinForEnterToMenu";
        public const string COLUMN_GIN_LENGTH_FOR_ENTER_TO_MENU = "GinLengthForEnterToMenu";
        public const string COLUMN_SECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU = "SecurityDailyPlanForEnterToMenu";
        public const string COLUMN_GUID_SSECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU = "GuidSecurityDailyPlanForEnterToMenu";
        public const string COLUMN_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU = "SecurityTimeZoneForEnterToMenu";
        public const string COLUMN_GUID_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU = "GuidSecurityTimeZoneForEnterToMenu";

        public const string COLUMNALARMCROFFLINE = "AlarmCROffline";
        public const string COLUMN_BLOCK_ALARM_CR_OFFLINE = "BlockAlarmCROffline";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_OFFLINE_OBJECT_TYPE = "ObjBlockAlarmCROfflineObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_OFFLINE_ID = "ObjBlockAlarmCROfflineId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_OFFLINE = "ObjBlockAlarmCROffline";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_OFFLINE = "EventlogDuringBlockAlarmCROffline";

        public const string COLUMNALARMCRTAMPERSABOTAGE = "AlarmCRTamperSabotage";
        public const string COLUMN_BLOCK_ALARM_CR_TAMPER_SABOTAGE = "BlockAlarmCRTamperSabotage";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_TAMPER_SABOTAGE_OBJECT_TYPE = "ObjBlockAlarmCRTamperSabotageObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_TAMPER_SABOTAGE_ID = "ObjBlockAlarmCRTamperSabotageId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_TAMPER_SABOTAGE = "ObjBlockAlarmCRTamperSabotage";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_TAMPER_SABOTAGE = "EventlogDuringBlockAlarmCRTamperSabotage";

        public const string COLUMNALARMCRACCESSDENIED = "AlarmCrAccessDenied";
        public const string COLUMN_BLOCK_ALARM_CR_ACCESS_DENIED = "BlockAlarmCrAccessDenied";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_ACCESS_DENIED_OBJECT_TYPE = "ObjBlockAlarmCrAccessDeniedObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_ACCESS_DENIED_ID = "ObjBlockAlarmCrAccessDeniedId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_ACCESS_DENIED = "ObjBlockAlarmCrAccessDenied";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_ACCESS_DENIED = "EventlogDuringBlockAlarmCrAccessDenied";

        public const string COLUMNALARMCRUNKNOWNCARD = "AlarmCrUnknownCard";
        public const string COLUMN_BLOCK_ALARM_CR_UNKNOWN_CARD = "BlockAlarmCrUnknownCard";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_UNKNOWN_CARD_OBJECT_TYPE = "ObjBlockAlarmCrUnknownCardObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_UNKNOWN_CARD_ID = "ObjBlockAlarmCrUnknownCardId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_UNKNOWN_CARD = "ObjBlockAlarmCrUnknownCard";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_UNKNOWN_CARD = "EventlogDuringBlockAlarmCrUnknownCard";

        public const string COLUMNALRMCRCARDBLOCKEDORINACTIVE = "AlarmCrCardBlockedOrInactive";
        public const string COLUMN_BLOCK_ALARM_CR_CARD_BLOCKED_OR_INACTIVE = "BlockAlarmCrCardBlockedOrInactive";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_CARD_BLOCKED_OR_INACTIVE_OBJECT_TYPE = "ObjBlockAlarmCrCardBlockedOrInactiveObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_CARD_BLOCKED_OR_INACTIVE_ID = "ObjBlockAlarmCrCardBlockedOrInactiveId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_CARD_BLOCKED_OR_INACTIVE = "ObjBlockAlarmCrCardBlockedOrInactive";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_CARD_BLOCKED_OR_INACTIVE = "EventlogDuringBlockAlarmCrCardBlockedOrInactive";

        public const string COLUMNALARMCRINVALIDPIN = "AlarmCrInvalidPin";
        public const string COLUMN_BLOCK_ALARM_CR_INVALID_PIN = "BlockAlarmCrInvalidPin";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_PIN_OBJECT_TYPE = "ObjBlockAlarmCrInvalidPinObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_PIN_ID = "ObjBlockAlarmCrInvalidPinId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_PIN = "ObjBlockAlarmCrInvalidPin";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_INVALID_PIN = "EventlogDuringBlockAlarmCrInvalidPin";

        public const string COLUMNALARMCRINVALIDGIN = "AlarmCrInvalidGin";
        public const string COLUMN_BLOCK_ALARM_CR_INVALID_GIN = "BlockAlarmCrInvalidGin";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_GIN_OBJECT_TYPE = "ObjBlockAlarmCrInvalidGinObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_GIN_ID = "ObjBlockAlarmCrInvalidGinId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_GIN = "ObjBlockAlarmCrInvalidGin";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_INVALID_GIN = "EventlogDuringBlockAlarmCrInvalidGin";

        public const string COLUMNALARMCRINVALIDEMERGENCYCODE = "AlarmCrInvalidEmergencyCode";
        public const string COLUMN_BLOCK_ALARM_CR_INVALID_EMERGENCY_CODE = "BlockAlarmCrInvalidEmergencyCode";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_EMERGENCY_CODE_OBJECT_TYPE = "ObjBlockAlarmCrInvalidEmergencyCodeObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_EMERGENCY_CODE_ID = "ObjBlockAlarmCrInvalidEmergencyCodeId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_INVALID_EMERGENCY_CODE = "ObjBlockAlarmCrInvalidEmergencyCode";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CR_INVALID_EMERGENCY_CODE = "EventlogDuringBlockAlarmCrInvalidEmergencyCode";

        public const string COLUMNALARMCRACCESSPERMITTED = "AlarmCrAccessPermitted";
        public const string COLUMN_BLOCK_ALARM_CR_ACCESS_PERMITTED = "BlockAlarmCrAccessPermitted";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_ACCESS_PERMITTED_OBJECT_TYPE = "ObjBlockAlarmCrAccessPermittedObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_ACCESS_PERMITTED_ID = "ObjBlockAlarmCrAccessPermittedId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CR_ACCESS_PERMITTED = "ObjBlockAlarmCrAccessPermitted";

        public const string COLUMNALARMCCUOFFLINEPRESENATIONGROUP = "AlarmCCUOfflinePresentationGroup";
        public const string COLUMNALARMCCUUNCONFIGUREDPRESENTATIONGROUP = "AlarmCCUUnconfiguredPresentationGroup";
        public const string COLUMNALARMCCUCLOCKUNSYNCHRONIZEDPRESENTATIONGROUP = "AlarmCCUClockUnsynchronizedPresentationGroup";
        public const string COLUMNALARMCCUTAMPERSABOTAGEPRESENATIONGROUP = "AlarmCCUTamperSabotagePresentationGroup";
        public const string COLUMNALARMCCUPRIMARYPOWERMISSINGPRESENATIONGROUP = "AlarmCCUPrimaryPowerMissingPresentationGroup";
        public const string COLUMNALARMCCUBATTERYISLOWPRESENATIONGROUP = "AlarmCCUBatteryIsLowPresentationGroup";
        public const string COLUMNALARMCCUFUSEONEXTENSIONBOARDPRESENATIONGROUP = "AlarmCCUFuseOnExtensionBoardPresentationGroup";
        public const string COLUMN_ALARM_CCU_CAT_UNREACHABLE_PRESENATION_GROUP = "AlarmCcuCatUnreachablePresentationGroup";
        public const string COLUMN_ALARM_CCU_TRANSFER_TO_ARC_TIMED_OUT_PRESENATION_GROUP = "AlarmCcuTransferToArcTimedOutPresentationGroup";

        public const string COLUMNALARMDCUOFFLINEPRESENATIONGROUP = "AlarmDCUOfflinePresentationGroup";
        public const string COLUMNALARMDCUTAMPERSABOTAGEPRESENATIONGROUP = "AlarmDCUTamperSabotagePresentationGroup";
        public const string COLUMNALARMDEDOORAJARPRESENATIONGROUP = "AlarmDEDoorAjarPresentationGroup";
        public const string COLUMNALARMDEINTRUSIONPRESENATIONGROUP = "AlarmDEIntrusionPresentationGroup";
        public const string COLUMNALARMDESABOTAGEPRESENATIONGROUP = "AlarmDESabotagePresentationGroup";

        public const string COLUMNALARMCROFFLINEPRESENATIONGROUP = "AlarmCROfflinePresentationGroup";
        public const string COLUMNALARMCRTAMPERSABOTAGEPRESENATIONGROUP = "AlarmCRTamperSabotagePresentationGroup";
        public const string COLUMNALARMCRACCESSDENIEDPRESENATIONGROUP = "AlarmCRAccessDeniedPresentationGroup";
        public const string COLUMNALARMCRUNKNOWNCARDPRESENATIONGROUP = "AlarmCRUnknownCardPresentationGroup";
        public const string COLUMNALARMCRCARDBLOCKEDORINACTIVEPRESENATIONGROUP = "AlarmCRCardBlockedOrInactivePresentationGroup";
        public const string COLUMNALARMCRINVALIDPINPRESENATIONGROUP = "AlarmCRInvalidPINPresentationGroup";
        public const string COLUMNALARMCRINVALIDGINPRESENATIONGROUP = "AlarmCRInvalidGINPresentationGroup";
        public const string COLUMNALARMCRINVALIDEMERGENCYCODEPRESENATIONGROUP = "AlarmCRInvalidEmergencyCodePresentationGroup";
        public const string COLUMNALARMCRACCESSPERMITTEDPRESENATIONGROUP = "AlarmCRAccessPermittedPresentationGroup";

        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNALLOWALARMAREAREPOSRTINGTOITSCARDREADERS = "AllowAAToCRsReporting";

        public const string COLUMN_INVALID_PIN_RETRIES_LIMIT_ENABLED = "InvalidPinRetriesLimitEnabled";
        public const string COLUMN_INVALID_PIN_RETRIES_COUNT = "InvalidPinRetriesCount";
        public const string COLUMN_INVALID_PIN_RETRIES_LIMIT_REACHED_TIMEOUT = "InvalidPinRetriesLimitReachedTimeout";

        public const string COLUMN_ALARM_INVALID_PIN_RETRIES_LIMIT_REACHED = "AlarmInvalidPinRetriesLimitReached";
        public const string COLUMN_BLOCK_ALARM_INVALID_PIN_RETRIES_LIMIT_REACHED = "BlockAlarmInvalidPinRetriesLimitReached";

        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_RETRIES_LIMIT_REACHED_OBJECT_TYPE =
            "ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType";

        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_RETRIES_LIMIT_REACHED_ID =
            "ObjBlockAlarmInvalidPinRetriesLimitReachedId";

        public const string COLUMN_PG_ALARM_INVALID_PIN_RETRIES_LIMIT_REACHED =
            "PgAlarmInvalidPinRetriesLimitReached";

        public const string COLUMN_INVALID_GIN_RETRIES_LIMIT_ENABLED = "InvalidGinRetriesLimitEnabled";
        public const string COLUMN_INVALID_GIN_RETRIES_COUNT = "InvalidGinRetriesCount";
        public const string COLUMN_INVALID_GIN_RETRIES_LIMIT_REACHED_TIMEOUT = "InvalidGinRetriesLimitReachedTimeout";

        public const string COLUMN_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED = "AlarmInvalidGinRetriesLimitReached";
        public const string COLUMN_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED = "BlockAlarmInvalidGinRetriesLimitReached";

        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_OBJECT_TYPE =
            "ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType";

        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_ID =
            "ObjBlockAlarmInvalidGinRetriesLimitReachedId";

        public const string COLUMN_PG_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED =
            "PgAlarmInvalidGinRetriesLimitReached";

        public const string COLUMN_DEVICES_ALARM_SETTING_ALARM_ARCS = "DevicesAlarmSettingAlarmArcs";
        public const string COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS = "AlarmTypeAndIdAlarmArcs";

        public const string COLUMN_ALARM_AREA_ALARM_PRESENTATION_GROUP = "AlarmAreaAlarmPresentationGroup";
        public const string COLUMN_ALARM_AREA_SET_BY_ON_OFF_OBJECT_FAILED_PRESENTATION_GROUP = "AlarmAreaSetByOnOffObjectFailedPresentationGroup";
        public const string COLUMN_SENSOR_ALARM_PRESENTATION_GROUP = "SensorAlarmPresentationGroup";
        public const string COLUMN_SENSOR_TAMPER_ALARM_PRESENTATION_GROUP = "SensorTamperAlarmPresentationGroup";

        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdDevicesAlarmSetting { get; set; }
        public virtual string Name { get; set; }

        // Alarms for CCU
        public virtual bool AlarmCCUOffline { get; set; }
        public virtual bool BlockAlarmCCUOffline { get; set; }
        public virtual byte? ObjBlockAlarmCCUOfflineObjectType { get; set; }
        public virtual Guid? ObjBlockAlarmCCUOfflineId { get; set; }

        public virtual bool AlarmCCUUnconfigured { get; set; }
        public virtual bool BlockAlarmCCUUnconfigured { get; set; }
        public virtual byte? ObjBlockAlarmCCUUnconfiguredObjectType { get; set; }
        public virtual Guid? ObjBlockAlarmCCUUnconfiguredId { get; set; }

        public virtual bool AlarmCCUClockUnsynchronized { get; set; }
        public virtual bool BlockAlarmCCUClockUnsynchronized { get; set; }
        public virtual byte? ObjBlockAlarmCCUClockUnsynchronizedObjectType { get; set; }
        public virtual Guid? ObjBlockAlarmCCUClockUnsynchronizedId { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCCUTamperSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCCUTamperSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCCUTamperSabotageObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCCUTamperSabotageId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCCUTamperSabotage { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmPrimaryPowerMissing { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmPrimaryPowerMissing { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmPrimaryPowerMissingObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmPrimaryPowerMissingId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmPrimaryPowerMissing { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmBatteryIsLow { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmBatteryIsLow { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmBatteryIsLowObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmBatteryIsLowId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmBatteryIsLow { get; set; }

        [LwSerialize]
        public virtual bool AlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsOutputFuseObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsOutputFuseId { get; set; }
        [LwSerialize]
        public virtual bool EventlogDuringBlockAlarmUpsOutputFuse { get; set; }

        [LwSerialize]
        public virtual bool AlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsBatteryFaultObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsBatteryFaultId { get; set; }
        [LwSerialize]
        public virtual bool EventlogDuringBlockAlarmUpsBatteryFault { get; set; }

        [LwSerialize]
        public virtual bool AlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsBatteryFuseObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsBatteryFuseId { get; set; }
        [LwSerialize]
        public virtual bool EventlogDuringBlockAlarmUpsBatteryFuse { get; set; }

        [LwSerialize]
        public virtual bool AlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsOvertemperatureObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsOvertemperatureId { get; set; }
        [LwSerialize]
        public virtual bool EventlogDuringBlockAlarmUpsOvertemperature { get; set; }

        [LwSerialize]
        public virtual bool AlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsTamperSabotageId { get; set; }
        [LwSerialize]
        public virtual bool EventlogDuringBlockAlarmUpsTamperSabotage { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmFuseOnExtensionBoard { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmFuseOnExtensionBoard { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmFuseOnExtensionBoardObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmFuseOnExtensionBoardId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmFuseOnExtensionBoard { get; set; }

        [LwSerialize]
        public virtual bool AlarmCcuCatUnreachable { get; set; }

        [LwSerialize]
        public virtual bool AlarmCcuTransferToArcTimedOut { get; set; }

        // Alarms for DCU
        [LwSerialize]
        public virtual bool AlarmDCUOffline { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmDCUOffline { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmDCUOfflineObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmDCUOfflineId { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmDCUTamperSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmDCUTamperSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmDCUTamperSabotageObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmDCUTamperSabotageId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmDCUTamperSabotage { get; set; }

        // Alarms for door environment
        [LwSerializeAttribute()]
        public virtual bool AlarmDEIntrusion { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmDEIntrusion { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmDEIntrusionObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmDEIntrusionId { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmDEDoorAjar { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmDEDoorAjar { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmDEDoorAjarObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmDEDoorAjarId { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmDESabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmDESabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmDESabotageObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmDESabotageId { get; set; }

        // Card reader - security level for enter to menu
        [LwSerialize]
        public virtual byte SecurityLevelForEnterToMenu { get; set; }

        [LwSerialize]
        public virtual string GinForEnterToMenu { get; set; }
        [LwSerialize]
        public virtual byte GinLengthForEnterToMenu { get; set; }

        public virtual SecurityDailyPlan SecurityDailyPlanForEnterToMenu { get; set; }
        [LwSerialize]
        public virtual Guid GuidSecurityDailyPlanForEnterToMenu { get; set; }

        public virtual SecurityTimeZone SecurityTimeZoneForEnterToMenu { get; set; }
        [LwSerialize]
        public virtual Guid GuidSecurityTimeZoneForEnterToMenu { get; set; }

        // Alarms for card reader
        [LwSerialize]
        public virtual bool AlarmCROffline { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmCROffline { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmCROfflineObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmCROfflineId { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCRTamperSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCRTamperSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCRTamperSabotageObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCRTamperSabotageId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCRTamperSabotage { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrAccessDenied { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrAccessDenied { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrAccessDeniedObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrAccessDeniedId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCrAccessDenied { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrUnknownCard { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrUnknownCard { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrUnknownCardObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrUnknownCardId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCrUnknownCard { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrCardBlockedOrInactive { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrCardBlockedOrInactive { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrCardBlockedOrInactiveObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrCardBlockedOrInactiveId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCrCardBlockedOrInactive { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrInvalidPin { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrInvalidPin { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrInvalidPinObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrInvalidPinId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCrInvalidPin { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrInvalidGin { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrInvalidGin { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrInvalidGinObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrInvalidGinId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCrInvalidGin { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrInvalidEmergencyCode { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrInvalidEmergencyCode { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrInvalidEmergencyCodeObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrInvalidEmergencyCodeId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool EventlogDuringBlockAlarmCrInvalidEmergencyCode { get; set; }

        [LwSerializeAttribute()]
        public virtual bool AlarmCrAccessPermitted { get; set; }
        [LwSerializeAttribute()]
        public virtual bool BlockAlarmCrAccessPermitted { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmCrAccessPermittedObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmCrAccessPermittedId { get; set; }

        [LwSerialize]
        public virtual bool AlarmInvalidPinRetriesLimitReached { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmInvalidPinRetriesLimitReached { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmInvalidPinRetriesLimitReachedId { get; set; }

        [LwSerialize]
        public virtual bool AlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmInvalidGinRetriesLimitReachedId { get; set; }

        // Presentation groups for CCUs
        public virtual PresentationGroup AlarmCCUOfflinePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCCUUnconfiguredPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCCUClockUnsynchronizedPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCCUTamperSabotagePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCCUPrimaryPowerMissingPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCCUBatteryIsLowPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuUpsOutputFusePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuUpsBatteryFaultPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuUpsBatteryFusePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuUpsOvertemperaturePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuUpsTamperSabotagePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCCUFuseOnExtensionBoardPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuCatUnreachablePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCcuTransferToArcTimedOutPresentationGroup { get; set; }

        // Presentation groups for DCUs
        public virtual PresentationGroup AlarmDCUOfflinePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmDCUTamperSabotagePresentationGroup { get; set; }

        // Presentation groups for door environment
        public virtual PresentationGroup AlarmDEDoorAjarPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmDEIntrusionPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmDESabotagePresentationGroup { get; set; }

        // Presentation group for CRs
        public virtual PresentationGroup AlarmCROfflinePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRTamperSabotagePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRAccessDeniedPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRUnknownCardPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRCardBlockedOrInactivePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRInvalidPINPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRInvalidGINPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRInvalidEmergencyCodePresentationGroup { get; set; }
        public virtual PresentationGroup AlarmCRAccessPermittedPresentationGroup { get; set; }
        public virtual PresentationGroup PgAlarmInvalidPinRetriesLimitReached { get; set; }
        public virtual PresentationGroup PgAlarmInvalidGinRetriesLimitReached { get; set; }

        public virtual string Description { get; set; }

        [LwSerialize]
        public virtual bool AllowAAToCRsReporting { get; set; }

        [LwSerialize]
        public virtual bool InvalidPinRetriesLimitEnabled { get; set; }
        [LwSerialize]
        public virtual byte InvalidPinRetriesCount { get; set; }
        [LwSerialize]
        public virtual byte InvalidPinRetriesLimitReachedTimeout { get; set; }

        [LwSerialize]
        public virtual bool InvalidGinRetriesLimitEnabled { get; set; }
        [LwSerialize]
        public virtual byte InvalidGinRetriesCount { get; set; }
        [LwSerialize]
        public virtual byte InvalidGinRetriesLimitReachedTimeout { get; set; }

        public virtual ICollection<DevicesAlarmSettingAlarmArc> DevicesAlarmSettingAlarmArcs { get; set; }
        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        [LwSerialize]
        public virtual bool AlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual bool BlockAlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmAreaSetByOnOffObjectFailedId { get; set; }

        // Presentation groups for AAs
        public virtual PresentationGroup AlarmAreaAlarmPresentationGroup { get; set; }
        public virtual PresentationGroup AlarmAreaSetByOnOffObjectFailedPresentationGroup { get; set; }
        public virtual PresentationGroup SensorAlarmPresentationGroup { get; set; }
        public virtual PresentationGroup SensorTamperAlarmPresentationGroup { get; set; }


        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DevicesAlarmSetting)
            {
                return (obj as DevicesAlarmSetting).IdDevicesAlarmSetting == IdDevicesAlarmSetting;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdDevicesAlarmSetting.ToString();
        }

        public override object GetId()
        {
            return IdDevicesAlarmSetting;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.DevicesAlarmSetting;
        }

        public virtual void PrepareToSend()
        {
            GuidSecurityDailyPlanForEnterToMenu = SecurityDailyPlanForEnterToMenu != null
                ? SecurityDailyPlanForEnterToMenu.IdSecurityDailyPlan
                : Guid.Empty;

            GuidSecurityTimeZoneForEnterToMenu = SecurityTimeZoneForEnterToMenu != null
                ? SecurityTimeZoneForEnterToMenu.IdSecurityTimeZone
                : Guid.Empty;

            AlarmTypeAndIdAlarmArcs = DevicesAlarmSettingAlarmArcs == null || DevicesAlarmSettingAlarmArcs.Count == 0
                ? null
                : new List<AlarmTypeAndIdAlarmArc>(
                    DevicesAlarmSettingAlarmArcs.Select(
                        devicesAlarmSettingAlarmArc =>
                            new AlarmTypeAndIdAlarmArc(
                                (AlarmType) devicesAlarmSettingAlarmArc.AlarmType,
                                devicesAlarmSettingAlarmArc.IdAlarmArc)));
        }
    }
}
