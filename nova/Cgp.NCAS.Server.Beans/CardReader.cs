using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.NCAS.Server.Beans
{

    public enum DCUHWVersion : byte
    {
        Unknown = 0x00,
        RS485 = 0x01,
        Echelon = 0x02
    }

    [Serializable]
    [LwSerialize(329)]
    public enum SecurityLevelForSpecialKey
    {
        [Name("None")]
        None = 0,
        [Name("CODE")]
        CODE = 1,
        [Name("CODEORCARD")]
        CODEORCARD = 2,
        [Name("CODEORCARDPIN")]
        CODEORCARDPIN = 3,
        [Name("CARD")]
        CARD = 4,
        [Name("CARDPIN")]
        CARDPIN = 5,
    }

    [Serializable]
    [LwSerialize(323)]
    public enum ObjectAction
    {
        Activate, Deactivate, ActivateDeactivate 
    }

    [Serializable]
    [LwSerialize(318)]
    public enum FunctionKeySymbol
    {
        F1, F2, Lighting, DoorBell, Info 
    }

    [Serializable]
    [LwSerialize(317)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class FunctionKey
    {
        [LwSerialize]
        public virtual bool isEnable { get; set; }
        [LwSerialize]
        public virtual SecurityLevelForSpecialKey SecurityLevel { get; set; }
        [LwSerialize]
        public virtual string GIN { get; set; }
        
        public virtual int GinLenght { get; set; }
        [LwSerialize]
        public virtual Guid IdOutput { get; set; }
        [LwSerialize]
        public virtual ObjectAction ObjectAction { get; set; }
        [LwSerialize]
        public virtual Guid IdTimeZoneOrDailyPlan { get; set; }
        [LwSerialize]
        public bool IsUsedTimeZone { get; set; }
        [LwSerialize]
        public virtual string Text { get; set; }
        [LwSerialize]
        public virtual FunctionKeySymbol Symbol { get; set; }
    }

    [Serializable]
    [LwSerialize(309)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class CardReader : 
        AOrmObjectWithVersion, 
        IOrmObjectWithAlarmInstructions, 
        IEquatable<CardReader>,
        IGetDcu,
        ICardReaderObject
    {
        public const string COLUMNIDCARDREADER = "IdCardReader";
        public const string COLUMNNAME = "Name";
        public const string COLUMNGIN = "GIN";
        public const string COLUMNGINLENGTH = "GinLength";
        public const string COLUMNSECURITYLEVEL = "SecurityLevel";
        public const string COLUMNISEMERGENCYCODE = "IsEmergencyCode";
        public const string COLUMNEMERGENCYCODE = "EmergencyCode";
        public const string COLUMNEMERGENCYCODELENGTH = "EmergencyCodeLength";
        public const string COLUMNISFORCEDSECURITYLEVEL = "IsForcedSecurityLevel";
        public const string COLUMNFORCEDSECURITYLEVEL = "ForcedSecurityLevel";
        public const string COLUMNSECURITYDAILYPLAN = "SecurityDailyPlan";
        public const string COLUMNGUIDSECURITYDAILYPLAN = "GuidSecurityDailyPlan";
        public const string COLUMNSECURITYTIMEZONE = "SecurityTimeZone";
        public const string COLUMNGUIDSECURITYTIMEZONE = "GuidSecurityTimeZone";
        //public const string COLUMNTIMEZONE = "TimeZone";
        public const string COLUMNDCU = "DCU";
        public const string COLUMNGUIDDCU = "GuidDCU";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNAACARDREADERS = "AACardReaders";
        public const string COLUMNONOFFOBJECTOBJECTTYPE = "OnOffObjectObjectType";
        public const string COLUMNONOFFOBJECTTYPE = "OnOffObjectType";
        public const string COLUMNONOFFOBJECTID = "OnOffObjectId";
        public const string COLUMNONOFFOBJECT = "OnOffObject";
        public const string COLUMNCCU = "CCU";
        public const string COLUMNGUIDCCU = "GuidCCU";
        public const string COLUMNPORT = "Port";
        public const string COLUMNADDRESS = "Address";
        public const string COLUMNONLINESTATE = "OnlineState";
        public const string COLUMNUPGRADEPROGRESS = "UpgradeProgress";
        public const string COLUMNSELECTUPGRADE = "Select";
        public const string COLUMNUSED = "Used";

        public const string COLUMNALARMACCESSDENIED = "AlarmAccessDenied";
        public const string COLUMN_BLOCK_ALARM_ACCESS_DENIED = "BlockAlarmAccessDenied";
        public const string COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED_OBJECT_TYPE = "ObjBlockAlarmAccessDeniedObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED_ID = "ObjBlockAlarmAccessDeniedId";
        public const string COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED = "ObjBlockAlarmAccessDenied";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_ACCESS_DENIED = "EventlogDuringBlockAlarmAccessDenied";

        public const string COLUMNALARMUNKNOWNCARD = "AlarmUnknownCard";
        public const string COLUMN_BLOCK_ALARM_UNKNOWN_CARD = "BlockAlarmUnknownCard";
        public const string COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD_OBJECT_TYPE = "ObjBlockAlarmUnknownCardObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD_ID = "ObjBlockAlarmUnknownCardId";
        public const string COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD = "ObjBlockAlarmUnknownCard";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_UNKNOWN_CARD = "EventlogDuringBlockAlarmUnknownCard";

        public const string COLUMNALARMCARDBLOCKEDORINACTIVE = "AlarmCardBlockedOrInactive";
        public const string COLUMN_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE = "BlockAlarmCardBlockedOrInactive";
        public const string COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE_OBJECT_TYPE = "ObjBlockAlarmCardBlockedOrInactiveObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE_ID = "ObjBlockAlarmCardBlockedOrInactiveId";
        public const string COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE = "ObjBlockAlarmCardBlockedOrInactive";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE = "EventlogDuringBlockAlarmCardBlockedOrInactive";

        public const string COLUMNALARMINVALIDPIN = "AlarmInvalidPIN";
        public const string COLUMN_BLOCK_ALARM_INVALID_PIN = "BlockAlarmInvalidPin";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_OBJECT_TYPE = "ObjBlockAlarmInvalidPinObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_ID = "ObjBlockAlarmInvalidPinId";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN = "ObjBlockAlarmInvalidPin";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_PIN = "EventlogDuringBlockAlarmInvalidPin";

        public const string COLUMNALARMINVALIDGIN = "AlarmInvalidGIN";
        public const string COLUMN_BLOCK_ALARM_INVALID_GIN = "BlockAlarmInvalidGin";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_OBJECT_TYPE = "ObjBlockAlarmInvalidGinObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_ID = "ObjBlockAlarmInvalidGinId";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN = "ObjBlockAlarmInvalidGin";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_GIN = "EventlogDuringBlockAlarmInvalidGin";

        public const string COLUMNALARMINVALIDEMERGENCYCODE = "AlarmInvalidEmergencyCode";
        public const string COLUMN_BLOCK_ALARM_INVALID_EMERGENCY_CODE = "BlockAlarmInvalidEmergencyCode";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE_OBJECT_TYPE = "ObjBlockAlarmInvalidEmergencyCodeObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE_ID = "ObjBlockAlarmInvalidEmergencyCodeId";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE = "ObjBlockAlarmInvalidEmergencyCode";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_EMERGENCY_CODE = "EventlogDuringBlockAlarmInvalidEmergencyCode";

        public const string COLUMNALARMACCESSPERMITTED = "AlarmAccessPermitted";
        public const string COLUMN_BLOCK_ALARM_ACCESS_PERMITTED = "BlockAlarmAccessPermitted";
        public const string COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED_OBJECT_TYPE = "ObjBlockAlarmAccessPermittedObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED_ID = "ObjBlockAlarmAccessPermittedId";
        public const string COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED = "ObjBlockAlarmAccessPermitted";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_ACCESS_PERMITTED = "EventlogDuringBlockAlarmAccessPermitted";

        public const string COLUMNCRLANGUAGE = "CRLanguage";
        public const string COLUMNCARDAPPLIEDLED = "CardAppliedLED";
        public const string COLUMNCARDAPPLIEDKEYBOARDLIGHT = "CardAppliedKeyboardLight";
        public const string COLUMNCARDAPPLIEDINERNALBUZZER = "CardAppliedInternalBuzzer";
        public const string COLUMNCARDAPPLIEDEXTERNALBUZER = "CardAppliedExternalBuzzer";
        public const string COLUMNTAMPERLED = "TamperLED";
        public const string COLUMNTAMPERKEYBOARDLIGHT = "TamperKeyboardLight";
        public const string COLUMNTAMPERINERNALBUZZER = "TamperInternalBuzzer";
        public const string COLUMNTAMPEREXTERNALBUZER = "TamperExternalBuzzer";
        public const string COLUMNRESETLED = "ResetLED";
        public const string COLUMNRESETKEYBOARDLIGHT = "ResetKeyboardLight";
        public const string COLUMNRESETINERNALBUZZER = "ResetInternalBuzzer";
        public const string COLUMNRESETEXTERNALBUZER = "ResetExternalBuzzer";
        public const string COLUMNKEYPRESSEDLED = "KeyPressedLED";
        public const string COLUMNKEYPRESSEDKEYBOARDLIGHT = "KeyPressedKeyboardLight";
        public const string COLUMNKEYPRESSEDINERNALBUZZER = "KeyPressedInternalBuzzer";
        public const string COLUMNKEYPRESSEDEXTERNALBUZER = "KeyPressedExternalBuzzer";
        public const string COLUMNINTERNALBUZERKILLSWITCH = "InternalBuzzerKillswitch";
        public const string COLUMN_SL_CODE_LED_PRESENTATION = "SlCodeLedPresentation";
        public const string COLUMNCARDREADERHARDWARE = "CardReaderHardware";
        public const string COLUMNSECURITYLEVELDSM = "SecurityLevelDSM";
        public const string COLUMNSLFORENTERTOMENU = "SLForEnterToMenu";
        public const string COLUMN_USE_ACCESS_GIN_FOR_ENTER_TO_MENU = "UseAccessGinForEnterToMenu";
        public const string COLUMN_GIN_FOR_ENTER_TO_MENU = "GinForEnterToMenu";
        public const string COLUMN_GIN_LENGTH_FOR_ENTER_TO_MENU = "GinLengthForEnterToMenu";
        public const string COLUMN_SECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU = "SecurityDailyPlanForEnterToMenu";
        public const string COLUMN_GUID_SSECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU = "GuidSecurityDailyPlanForEnterToMenu";
        public const string COLUMN_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU = "SecurityTimeZoneForEnterToMenu";
        public const string COLUMN_GUID_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU = "GuidSecurityTimeZoneForEnterToMenu";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNACTUALSTATE = "ActualState";

        public const string COLUMNALARMOFFLINE = "AlarmOffline";
        public const string COLUMN_BLOCK_ALARM_OFFLINE = "BlockAlarmOffline";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE_OBJECT_TYPE = "ObjBlockAlarmOfflineObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE_ID = "ObjBlockAlarmOfflineId";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE = "ObjBlockAlarmOffline";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_OFFLINE = "EventlogDuringBlockAlarmOffline";

        public const string COLUMNALARMTAMPER = "AlarmTamper";
        public const string COLUMN_BLOCK_ALARM_TAMPER = "BlockAlarmTamper";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER_OBJECT_TYPE = "ObjBlockAlarmTamperObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER_ID = "ObjBlockAlarmTamperId";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER = "ObjBlockAlarmTamper";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_TAMPER = "EventlogDuringBlockAlarmTamper";

        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string COLUMNENABLEPARENTINFULLNAME = "EnableParentInFullName";
        public const string COLUMNQUERYDBSTAMP = "QueryDbStamp";
        public const string COLUMNSPECIALOUTPUTFORTAMPER = "SpecialOutputForTamper";
        public const string COLUMNGUIDSPECIALOUTPUTFORTAMPER = "GuidSpecialOutputForTamper";
        public const string COLUMNSPECIALOUTPUTFOROFFLINE = "SpecialOutputForOffline";
        public const string COLUMNGUIDSPECIALOUTPUTFOROFFLINE = "GuidSpecialOutputForOffline";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMNEVENTLOG = "Eventlog";
        public const string COLUMNFUNCTIONKEY1 = "FunctionKey1";
        public const string COLUMNFUNCTIONKEY2 = "FunctionKey2";

        public const string COLUMN_INVALID_GIN_RETRIES_LIMIT_ENABLED = "InvalidGinRetriesLimitEnabled";
        public const string COLUMN_INVALID_PIN_RETRIES_LIMIT_ENABLED = "InvalidPinRetriesLimitEnabled";

        public const string COLUMN_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED = "AlarmInvalidGinRetriesLimitReached";
        public const string COLUMN_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED = "BlockAlarmInvalidGinRetriesLimitReached";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_OBJECT_TYPE = "ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_ID = "ObjBlockAlarmInvalidGinRetriesLimitReachedId";

        public const string COLUMN_CARD_READER_ALARM_ARCS = "CardReaderAlarmArcs";
        public const string COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS = "AlarmTypeAndIdAlarmArcs";

        public const string COLUMN_DISABLE_SCREENSAVER = "DisableScreensaver";
        public const string COLUMN_REPORT_EVENTS_TO_TIMETEC = "ReportEventsToTimetec";

        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdCardReader { get; set; }
        public virtual string Name { get; set; }
        [LwSerialize]
        public virtual string GIN { get; set; }

        public virtual byte GinLength { get; set; }
        [LwSerialize]
        public virtual byte SecurityLevel { get; set; }
        [LwSerialize]
        public virtual bool IsEmergencyCode { get; set; }
        [LwSerialize]
        public virtual string EmergencyCode { get; set; }
        [LwSerialize]
        public virtual byte EmergencyCodeLength { get; set; }
        [LwSerialize]
        public virtual bool IsForcedSecurityLevel { get; set; }
        [LwSerialize]
        public virtual byte ForcedSecurityLevel { get; set; }
        public virtual SecurityDailyPlan SecurityDailyPlan { get; set; }
        private Guid _guidSecurityDailyPlan = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSecurityDailyPlan { get { return _guidSecurityDailyPlan; } set { _guidSecurityDailyPlan = value; } }
        public virtual SecurityTimeZone SecurityTimeZone { get; set; }
        private Guid _guidSecurityTimeZone = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSecurityTimeZone { get { return _guidSecurityTimeZone; } set { _guidSecurityTimeZone = value; } }
        public virtual DCU DCU { get; set; }
        private Guid _guidDCU = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        public virtual CCU CCU { get; set; }
        private Guid _guidCCU = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        [LwSerialize]
        public virtual string Port { get; set; }
        [LwSerialize]
        public virtual byte Address { get; set; }
        [LwSerialize]
        public virtual byte CardReaderHardware { get; set; }

        [LwSerialize]
        public virtual bool? AlarmAccessDenied { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmAccessDenied { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmAccessDeniedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmAccessDeniedId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmAccessDenied { get; set; }

        [LwSerialize]
        public virtual bool? AlarmUnknownCard { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmUnknownCard { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUnknownCardObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUnknownCardId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmUnknownCard { get; set; }

        [LwSerialize]
        public virtual bool? AlarmCardBlockedOrInactive { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmCardBlockedOrInactive { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmCardBlockedOrInactiveObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmCardBlockedOrInactiveId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmCardBlockedOrInactive { get; set; }

        [LwSerialize]
        public virtual bool? AlarmInvalidPIN { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmInvalidPin { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmInvalidPinObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmInvalidPinId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmInvalidPin { get; set; }

        [LwSerialize]
        public virtual bool? AlarmInvalidGIN { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmInvalidGin { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmInvalidGinObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmInvalidGinId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmInvalidGin { get; set; }

        [LwSerialize]
        public virtual bool? AlarmInvalidEmergencyCode { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmInvalidEmergencyCode { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmInvalidEmergencyCodeObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmInvalidEmergencyCodeId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmInvalidEmergencyCode { get; set; }

        [LwSerialize]
        public virtual bool? AlarmAccessPermitted { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmAccessPermitted { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmAccessPermittedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmAccessPermittedId { get; set; }

        [LwSerialize]
        public virtual bool? AlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmInvalidGinRetriesLimitReachedId { get; set; }

        [LwSerialize]
        public virtual byte CRLanguage { get; set; }
        [LwSerialize]
        public virtual bool CardAppliedLED { get; set; }
        [LwSerialize]
        public virtual bool CardAppliedKeyboardLight { get; set; }
        [LwSerialize]
        public virtual bool CardAppliedInternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool CardAppliedExternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool TamperLED { get; set; }
        [LwSerialize]
        public virtual bool TamperKeyboardLight { get; set; }
        [LwSerialize]
        public virtual bool TamperInternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool TamperExternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool ResetLED { get; set; }
        [LwSerialize]
        public virtual bool ResetKeyboardLight { get; set; }
        [LwSerialize]
        public virtual bool ResetInternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool ResetExternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool KeyPressedLED { get; set; }
        [LwSerialize]
        public virtual bool KeyPressedKeyboardLight { get; set; }
        [LwSerialize]
        public virtual bool KeyPressedInternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool KeyPressedExternalBuzzer { get; set; }
        [LwSerialize]
        public virtual bool InternalBuzzerKillswitch { get; set; }
        [LwSerialize]
        public virtual bool SlCodeLedPresentation { get; set; }

        [LwSerialize]
        public virtual FunctionKey FunctionKey1 { get; set; }
        [LwSerialize]
        public virtual FunctionKey FunctionKey2 { get; set; }

        public virtual string Description { get; set; }
        public virtual ICollection<AACardReader> AACardReaders { get; set; }
        public virtual string OnOffObjectType { get; set; }
        private ObjectType _onOffObjectObjectType;
        [LwSerialize]
        public virtual ObjectType OnOffObjectObjectType { get { return _onOffObjectObjectType; } set { _onOffObjectObjectType = value; } }
        [LwSerialize]
        public virtual Guid? OnOffObjectId { get; set; }
        private AOnOffObject _onOffObject;
        public virtual AOnOffObject OnOffObject { get { return _onOffObject; } set { _onOffObject = value; } }
        [LwSerialize]
        public virtual byte? SLForEnterToMenu { get; set; }

        [LwSerialize]
        public virtual bool UseAccessGinForEnterToMenu { get; set; }

        [LwSerialize]
        public virtual string GinForEnterToMenu { get; set; }

        public virtual byte GinLengthForEnterToMenu { get; set; }

        public virtual SecurityDailyPlan SecurityDailyPlanForEnterToMenu { get; set; }
        [LwSerialize]
        public virtual Guid GuidSecurityDailyPlanForEnterToMenu { get; set; }

        public virtual SecurityTimeZone SecurityTimeZoneForEnterToMenu { get; set; }
        [LwSerialize]
        public virtual Guid GuidSecurityTimeZoneForEnterToMenu { get; set; }

        public virtual byte ObjectType { get; set; }

        [LwSerialize]
        public virtual bool? AlarmOffline { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmOffline { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmOfflineObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmOfflineId { get; set; }

        [LwSerialize]
        public virtual bool? AlarmTamper { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmTamper { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmTamperObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmTamperId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmTamper { get; set; }

        public virtual Guid CkUnique { get; set; }
        public virtual bool EnableParentInFullName { get; set; }
        [LwSerialize]
        public virtual byte? QueryDbStamp { get; set; }
        public virtual Output SpecialOutputForTamper { get; set; }
        private Guid _guidSpecialOutputForTamper = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSpecialOutputForTamper { get { return _guidSpecialOutputForTamper; } set { _guidSpecialOutputForTamper = value; } }
        public virtual Output SpecialOutputForOffline { get; set; }
        private Guid _guidSpecialOutputForOffline = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSpecialOutputForOffline { get { return _guidSpecialOutputForOffline; } set { _guidSpecialOutputForOffline = value; } }
        public virtual string LocalAlarmInstruction { get; set; }

        [LwSerialize]
        public virtual bool? InvalidGinRetriesLimitEnabled { get; set; }
        [LwSerialize]
        public virtual bool? InvalidPinRetriesLimitEnabled { get; set; }

        public virtual ICollection<CardReaderAlarmArc> CardReaderAlarmArcs { get; set; }
        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        [LwSerialize]
        public virtual bool DisableScreensaver { get; set; }

        public virtual TimetecSetting ReportEventsToTimetec { get; set; }

        public override int GetHashCode()
        {
            return 
                IdCardReader.Equals(Guid.Empty)
                    ? base.GetHashCode()
                    : IdCardReader.GetHashCode();
        }

        public virtual bool Equals(CardReader other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return
                other != null &&
                !IdCardReader.Equals(Guid.Empty) &&
                IdCardReader.Equals(other.IdCardReader);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CardReader);
        }

        public override string ToString()
        {
            var result = string.Empty;
            if (EnableParentInFullName)
            {
                if (CCU != null)
                {
                    result += CCU + StringConstants.SLASHWITHSPACES;
                }
                else if (DCU != null)
                {
                    result += DCU + StringConstants.SLASHWITHSPACES;
                }
            }
            result += Name;

            return result;
        }

        public CardReader()
        {
            IsEmergencyCode = false;
            IsForcedSecurityLevel = false;
            CRLanguage = (byte)Contal.Drivers.CardReader.CRLanguage.English;
            CardAppliedLED = true;
            CardAppliedKeyboardLight = true;
            CardAppliedInternalBuzzer = true;
            CardAppliedExternalBuzzer = true;
            TamperLED = true;
            TamperKeyboardLight = true;
            TamperInternalBuzzer = true;
            TamperExternalBuzzer = true;
            ResetLED = true;
            ResetKeyboardLight = true;
            ResetInternalBuzzer = true;
            ResetExternalBuzzer = true;
            KeyPressedLED = true;
            KeyPressedKeyboardLight = true;
            KeyPressedInternalBuzzer = true;
            KeyPressedExternalBuzzer = true;
            SlCodeLedPresentation = true;
            GIN = string.Empty;
            GinLength = 0;
            ObjectType = (byte)Cgp.Globals.ObjectType.CardReader;
            CkUnique = Guid.NewGuid();
            EnableParentInFullName = Support.EnableParentInFullName;
            Port = string.Empty;
        }

        public override bool Compare(object obj)
        {
            var cardReader = obj as CardReader;

            return 
                cardReader != null && 
                cardReader.IdCardReader == IdCardReader;
        }

        public virtual bool IsOnline()
        {
            return false;
        }

        public virtual void PrepareToSend()
        {
            GuidCCU = 
                CCU != null
                    ? CCU.IdCCU
                    : Guid.Empty;

            GuidDCU = 
                DCU != null
                    ? DCU.IdDCU
                    : Guid.Empty;

            GuidSecurityDailyPlan = 
                SecurityDailyPlan != null
                    ? SecurityDailyPlan.IdSecurityDailyPlan
                    : Guid.Empty;

            GuidSecurityTimeZone = 
                SecurityTimeZone != null
                    ? SecurityTimeZone.IdSecurityTimeZone
                    : Guid.Empty;

            if (OnOffObject != null)
                OnOffObjectObjectType = OnOffObject.GetObjectType();

            GuidSpecialOutputForOffline = 
                SpecialOutputForOffline != null
                    ? SpecialOutputForOffline.IdOutput
                    : Guid.Empty;

            GuidSpecialOutputForTamper = 
                SpecialOutputForTamper != null
                    ? SpecialOutputForTamper.IdOutput
                    : Guid.Empty;

            GuidSecurityDailyPlanForEnterToMenu = SecurityDailyPlanForEnterToMenu != null
                ? SecurityDailyPlanForEnterToMenu.IdSecurityDailyPlan
                : Guid.Empty;

            GuidSecurityTimeZoneForEnterToMenu = SecurityTimeZoneForEnterToMenu != null
                ? SecurityTimeZoneForEnterToMenu.IdSecurityTimeZone
                : Guid.Empty;

            AlarmTypeAndIdAlarmArcs = CardReaderAlarmArcs == null || CardReaderAlarmArcs.Count == 0
                ? null
                : new List<AlarmTypeAndIdAlarmArc>(
                    CardReaderAlarmArcs.Select(
                        cardReaderAlarmArc =>
                            new AlarmTypeAndIdAlarmArc(
                                (AlarmType)cardReaderAlarmArc.AlarmType,
                                cardReaderAlarmArc.IdAlarmArc)));
        }

        public override string GetIdString()
        {
            return IdCardReader.ToString();
        }

        public override object GetId()
        {
            return IdCardReader;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CardReaderModifyObj(this);
        }

        public virtual IEnumerable<ICardReaderObject> GetChildObjects()
        {
            yield break;
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.CardReader;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public virtual DCU GetDcu()
        {
            return DCU;
        }
    }

    [Serializable]
    public class CardReaderShort : IShortObject
    {
        public const string COLUMN_ID_CARD_READER = "IdCardReader";
        public const string COLUMN_FULL_NAME = "FullName";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_ONLINE_STATE = "OnlineState";
        public const string COLUMN_STRING_ONLINE_STATE = "StringOnlineState";
        public const string COLUMN_SECURITY_LEVEL_DSM = "SecurityLevelDSM";
        public const string COLUMN_IS_USED_IN_DOOR_ENVIRONMENT = "UsedInDoorEnvironment";
        public const string COLUMN_ACTUAL_STATE = "ActualState";
        public const string COLUMN_SECURITY_LEVEL = "SecurityLevel";
        public const string COLUMN_CR_COMMAND = "CardReaderSceneType";
        public const string COLUMN_IS_BLOCKED = "IsBlocked";
        public const string COLUMN_BLOCKED_STATE = "BlockedState";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdCardReader { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public OnlineState OnlineState { get; set; }
        public string StringOnlineState { get; set; }
        public bool UsedInDoorEnvironment { get; set; }
        public string SecurityLevelDSM { get; set; }
        public CardReaderSceneType CardReaderSceneType { get; set; }
        public string ActualState { get; set; }
        public byte SecurityLevel { get; set; }
        public bool IsBlocked { get; set; }
        public Image BlockedState { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }
        public byte SLForEnterToMenu { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public CardReaderShort(CardReader cr)
        {
            IdCardReader = cr.IdCardReader;
            Name = cr.Name;
            FullName = cr.ToString();
            SecurityLevel = cr.SecurityLevel;
            Description = cr.Description;
            SLForEnterToMenu = cr.SLForEnterToMenu.HasValue ? cr.SLForEnterToMenu.Value : (byte)0xFF;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.CardReader; } }

        public string GetSubTypeImageString(object value)
        {
            if (value is OnlineState)
            {
                try
                {
                    if ((OnlineState)value == OnlineState.Online) return ObjectType.CardReader.ToString();
                    return ObjTypeHelper.CardReaderBlocked;
                }
                catch { }
            }
            return string.Empty;
        }

        #endregion

        #region IShortObject Members

        public object Id { get { return IdCardReader; } }

        #endregion
    }


    [Serializable]
    public class CardReaderModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.CardReader; }
        }

        public CardReaderModifyObj(CardReader cardReader)
        {
            Id = cardReader.IdCardReader;
            FullName = cardReader.ToString();
            Description = cardReader.Description;
        }
    }

    public enum SecurityLevel : byte
    {
        [Name("Unlocked")]
        Unlocked = 0,
        [Name("CODE")]
        CODE = 1,
        [Name("CODEORCARD")]
        CODEORCARD = 2,
        [Name("CODEORCARDPIN")]
        CODEORCARDPIN = 3,
        [Name("CARD")]
        CARD = 4,
        [Name("CARDPIN")]
        CARDPIN = 5,
        [Name("Locked")]
        Locked = 6,
        //NOT SUPPORTED IN THIS VERSION
        //[Name("ToggleCard")]
        //ToggleCard = 7,
        //[Name("ToggleCardPIN")]
        //ToggleCardPIN = 8,
        [Name("SecurityTimeZoneSecurityDailyPlan")]
        SecurityTimeZoneSecurityDailyPlan = 9
    }

    public class SecurityLevelStates
    {
        private readonly SecurityLevel _value;
        public SecurityLevel Value
        {
            get { return _value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        public SecurityLevelStates(SecurityLevel value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<SecurityLevelStates> GetCardStatesList(LocalizationHelper localizationHelper)
        {
            IList<SecurityLevelStates> list = new List<SecurityLevelStates>();
            var fieldsInfo = typeof(SecurityLevel).GetFields();
            foreach (var fieldInfo in fieldsInfo)
            {
                var attribs =
                    fieldInfo.GetCustomAttributes(typeof(NameAttribute), false) as NameAttribute[];

                if (attribs != null && attribs.Length > 0)
                    list.Add(
                        new SecurityLevelStates(
                            (SecurityLevel)fieldInfo.GetValue(fieldInfo),
                            localizationHelper.GetString("SecurityLevelStates_" + attribs[0].Name)));
            }

            return list;
        }

        public static IList<SecurityLevelStates> GetCardStatesListSmallCR(LocalizationHelper localizationHelper)
        {
            IList<SecurityLevelStates> list = new List<SecurityLevelStates>();
            var fieldsInfo = typeof(SecurityLevel).GetFields();
            foreach (var fieldInfo in fieldsInfo)
            {
                var attribs =
                    fieldInfo.GetCustomAttributes(typeof(NameAttribute), false) as NameAttribute[];

                if (attribs != null &&
                    attribs.Length > 0 &&
                    !attribs[0].Name.Contains("PIN") &&
                    !attribs[0].Name.Contains("GIN"))
                {
                    list.Add(
                        new SecurityLevelStates(
                            (SecurityLevel)fieldInfo.GetValue(fieldInfo),
                            localizationHelper.GetString("SecurityLevelStates_" + attribs[0].Name)));
                }
            }

            return list;
        }

        public static SecurityLevelStates GetSecurityLevelState(LocalizationHelper localizationHelper, IList<SecurityLevelStates> listSecurityLevelStates, byte securityLevelState)
        {
            if (listSecurityLevelStates == null)
            {
                return GetSecurityLevelState(localizationHelper, securityLevelState);
            }
            foreach (var listSecurityLevelState in listSecurityLevelStates)
            {
                if ((byte)listSecurityLevelState.Value == securityLevelState)
                    return listSecurityLevelState;
            }

            return null;
        }

        public static SecurityLevelStates GetSecurityLevelState(LocalizationHelper localizationHelper, byte cardState)
        {
            var fieldsInfo = typeof(SecurityLevel).GetFields();
            foreach (var fieldInfo in fieldsInfo)
            {
                var attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs != null && attribs.Length > 0)
                    if ((byte)fieldInfo.GetValue(fieldInfo) == cardState)
                        return
                            new SecurityLevelStates(
                                (SecurityLevel)fieldInfo.GetValue(fieldInfo),
                                localizationHelper.GetString("SecurityLevelStates_" + attribs[0].Name));
            }

            return null;
        }
    }

    public class CRUpgradeProgressChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CRUpgradeProgressChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<CRUpgradeState> _upgradeProgressChanged;

        public static CRUpgradeProgressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CRUpgradeProgressChangedHandler();
                    }

                return _singleton;
            }
        }

        public CRUpgradeProgressChangedHandler()
            : base("CRUpgradeProgressChangedHandler")
        {
        }

        public void RegisterUpgradeProgressChanged(Action<CRUpgradeState> stateChanged)
        {
            _upgradeProgressChanged += stateChanged;
        }

        public void UnregisterUpgradeProgressChanged(Action<CRUpgradeState> stateChanged)
        {
            _upgradeProgressChanged -= stateChanged;
        }

        public void RunEvent(CRUpgradeState upgradeState)
        {
            if (_upgradeProgressChanged != null)
                _upgradeProgressChanged(upgradeState);
        }
    }

    [Serializable]
    public class CRUpgradeState
    {
        public Guid CCUGuid = Guid.Empty;
        public Guid? DCUGuid = Guid.Empty;
        public byte[] CardReaderAddresses = { };
        public byte? UpgradeResult = null;
        public int? Percents = null;
        public byte? UnpackErrorCode = null;

        public CRUpgradeState(Guid ccuGuid, Guid? dcuGuid, byte[] cardReaderAddresses, byte? upgradeResult, int? percents, byte? unpackErrorCode)
        {
            CCUGuid = ccuGuid;
            DCUGuid = dcuGuid;
            CardReaderAddresses = cardReaderAddresses;
            UpgradeResult = upgradeResult;
            Percents = percents;
            UnpackErrorCode = unpackErrorCode;
        }
    }

    public class StateChangedCardReaderHandler : ARemotingCallbackHandler
    {
        private static volatile StateChangedCardReaderHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte, Guid> _stateChanged;

        public static StateChangedCardReaderHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new StateChangedCardReaderHandler();
                    }

                return _singleton;
            }
        }

        public StateChangedCardReaderHandler()
            : base("StateChangedCardReaderHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte, Guid> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte, Guid> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State, Guid parent)
        {
            _stateChanged?.Invoke(id, State, parent);
        }
    }

    public class TamperChangedCardReaderHandler : ARemotingCallbackHandler
    {
        private static volatile TamperChangedCardReaderHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, bool, Guid> _tamperStateChanged;

        public static TamperChangedCardReaderHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new TamperChangedCardReaderHandler();
                    }

                return _singleton;
            }
        }

        public TamperChangedCardReaderHandler()
            : base("TamperChangedCardReaderHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, bool, Guid> tamperStateChanged)
        {
            _tamperStateChanged += tamperStateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, bool, Guid> tamperStateChanged)
        {
            _tamperStateChanged -= tamperStateChanged;
        }

        public void RunEvent(Guid id, bool isTamper, Guid parent)
        {
            if (_tamperStateChanged != null)
                _tamperStateChanged(id, isTamper, parent);
        }
    }

    public class CommandChangedCardReaderHandler : ARemotingCallbackHandler
    {
        private static volatile CommandChangedCardReaderHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _commandChanged;

        public static CommandChangedCardReaderHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CommandChangedCardReaderHandler();
                    }

                return _singleton;
            }
        }

        public CommandChangedCardReaderHandler()
            : base("CommandChangedCardReaderHandler")
        {
        }

        public void RegisterCommandChanged(Action<Guid, byte> commandChanged)
        {
            _commandChanged += commandChanged;
        }

        public void UnregisterCommandChanged(Action<Guid, byte> commandChanged)
        {
            _commandChanged -= commandChanged;
        }

        public void RunEvent(Guid id, byte State)
        {
            if (_commandChanged != null)
                _commandChanged(id, State);
        }
    }

    public class BlockedStateChangedCardReaderHandler : ARemotingCallbackHandler
    {
        private static volatile BlockedStateChangedCardReaderHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, bool> _commandChanged;

        public static BlockedStateChangedCardReaderHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new BlockedStateChangedCardReaderHandler();
                    }

                return _singleton;
            }
        }

        public BlockedStateChangedCardReaderHandler()
            : base("BlockedStateChangedCardReaderHandler")
        {
        }

        public void RegisterBlockedStateChanged(Action<Guid, bool> commandChanged)
        {
            _commandChanged += commandChanged;
        }

        public void UnregisterBlockedStateChanged(Action<Guid, bool> commandChanged)
        {
            _commandChanged -= commandChanged;
        }

        public void RunEvent(Guid id, bool blockedState)
        {
            _commandChanged?.Invoke(id, blockedState);
        }
    }
}