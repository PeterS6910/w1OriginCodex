using System;
using System.Collections.Generic;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    public enum SecurityLevel : byte
    {
        Unlocked = 0,
        Code = 1,
        CodeOrCard = 2,
        CodeOrCardPin = 3,
        Card = 4,
        CardPIN = 5,
        Locked = 6,
        SecurityTimeZoneSecurityDailyPlan = 9
    }

    [LwSerialize(329)]
    public enum SecurityLevelForSpecialKey
    {
        None = 0,
        Code = 1,
        CodeOrCard = 2,
        CodeOrCardPin = 3,
        CARD = 4,
        CARDPIN = 5
    }

    [LwSerialize(323)]
    public enum ObjectAction
    {
        Activate, Deactivate, ActivateDeactivate
    }

    [LwSerialize(318)]
    public enum FunctionKeySymbol
    {
        F1, F2, Lighting, DoorBell, Info
    }

    [LwSerialize(317)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class FunctionKey
    {
        [LwSerialize]
        public bool isEnable { get; set; }
        [LwSerialize]
        public SecurityLevelForSpecialKey SecurityLevel { get; set; }
        [LwSerialize]
        public string GIN { get; set; }
        [LwSerialize]
        public Guid IdOutput { get; set; }
        [LwSerialize]
        public ObjectAction ObjectAction { get; set; }
        [LwSerialize]
        public Guid IdTimeZoneOrDailyPlan { get; set; }
        [LwSerialize]
        public bool IsUsedTimeZone { get; set; }
        [LwSerialize]
        public string Text { get; set; }
        [LwSerialize]
        public FunctionKeySymbol Symbol { get; set; }
    }

    [LwSerialize(309)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CardReader : IDbObject
    {
        [LwSerialize]
        public Guid IdCardReader { get; set; }
        [LwSerialize]
        public string GIN { get; set; }
        [LwSerialize]
        public byte SecurityLevel { get; set; }

        [LwSerialize]
        public bool IsEmergencyCode { get; set; }
        [LwSerialize]
        public string EmergencyCode { get; set; }
        [LwSerialize]
        public byte EmergencyCodeLength { get; set; }
        [LwSerialize]
        public bool IsForcedSecurityLevel { get; set; }
        [LwSerialize]
        public byte ForcedSecurityLevel { get; set; }

        private Guid _guidSecurityDailyPlan = Guid.Empty;
        [LwSerialize]
        public Guid GuidSecurityDailyPlan { get { return _guidSecurityDailyPlan; } set { _guidSecurityDailyPlan = value; } }
        private Guid _guidSecurityTimeZone = Guid.Empty;
        [LwSerialize]
        public Guid GuidSecurityTimeZone { get { return _guidSecurityTimeZone; } set { _guidSecurityTimeZone = value; } }
        private Guid _guidDCU = Guid.Empty;
        [LwSerialize]
        public Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        private Guid _guidCCU = Guid.Empty;
        [LwSerialize]
        public Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        [LwSerialize]
        public string Port { get; set; }
        [LwSerialize]
        public byte Address { get; set; }
        [LwSerialize]
        public byte CardReaderHardware { get; set; }

        [LwSerialize]
        public bool? AlarmAccessDenied { get; set; }
        [LwSerialize]
        public bool? BlockAlarmAccessDenied { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmAccessDeniedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmAccessDeniedId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmAccessDenied { get; set; }

        [LwSerialize]
        public bool? AlarmUnknownCard { get; set; }
        [LwSerialize]
        public bool? BlockAlarmUnknownCard { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUnknownCardObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUnknownCardId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmUnknownCard { get; set; }

        [LwSerialize]
        public bool? AlarmCardBlockedOrInactive { get; set; }
        [LwSerialize]
        public bool? BlockAlarmCardBlockedOrInactive { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmCardBlockedOrInactiveObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmCardBlockedOrInactiveId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmCardBlockedOrInactive { get; set; }

        [LwSerialize]
        public bool? AlarmInvalidPIN { get; set; }
        [LwSerialize]
        public bool? BlockAlarmInvalidPin { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmInvalidPinObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmInvalidPinId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmInvalidPin { get; set; }

        [LwSerialize]
        public bool? AlarmInvalidGIN { get; set; }
        [LwSerialize]
        public bool? BlockAlarmInvalidGin { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmInvalidGinObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmInvalidGinId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmInvalidGin { get; set; }

        [LwSerialize]
        public bool? AlarmInvalidEmergencyCode { get; set; }
        [LwSerialize]
        public bool? BlockAlarmInvalidEmergencyCode { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmInvalidEmergencyCodeObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmInvalidEmergencyCodeId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmInvalidEmergencyCode { get; set; }

        [LwSerialize]
        public bool? AlarmAccessPermitted { get; set; }
        [LwSerialize]
        public bool? BlockAlarmAccessPermitted { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmAccessPermittedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmAccessPermittedId { get; set; }

        [LwSerialize]
        public bool? AlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public bool? BlockAlarmInvalidGinRetriesLimitReached { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmInvalidGinRetriesLimitReachedId { get; set; }

        [LwSerialize]
        public byte CRLanguage { get; set; }
        [LwSerialize]
        public bool CardAppliedLED { get; set; }
        [LwSerialize]
        public bool CardAppliedKeyboardLight { get; set; }
        [LwSerialize]
        public bool CardAppliedInternalBuzzer { get; set; }
        [LwSerialize]
        public bool CardAppliedExternalBuzzer { get; set; }
        [LwSerialize]
        public bool TamperLED { get; set; }
        [LwSerialize]
        public bool TamperKeyboardLight { get; set; }
        [LwSerialize]
        public bool TamperInternalBuzzer { get; set; }
        [LwSerialize]
        public bool TamperExternalBuzzer { get; set; }
        [LwSerialize]
        public bool ResetLED { get; set; }
        [LwSerialize]
        public bool ResetKeyboardLight { get; set; }
        [LwSerialize]
        public bool ResetInternalBuzzer { get; set; }
        [LwSerialize]
        public bool ResetExternalBuzzer { get; set; }
        [LwSerialize]
        public bool KeyPressedLED { get; set; }
        [LwSerialize]
        public bool KeyPressedKeyboardLight { get; set; }
        [LwSerialize]
        public bool KeyPressedInternalBuzzer { get; set; }
        [LwSerialize]
        public bool KeyPressedExternalBuzzer { get; set; }
        [LwSerialize]
        public bool InternalBuzzerKillswitch { get; set; }
        [LwSerialize]
        public bool SlCodeLedPresentation { get; set; }

        private ObjectType _onOffObjectObjectType;
        [LwSerialize]
        public ObjectType OnOffObjectObjectType { get { return _onOffObjectObjectType; } set { _onOffObjectObjectType = value; } }
        [LwSerialize]
        public Guid? OnOffObjectId { get; set; }

        [LwSerialize]
        public byte? SLForEnterToMenu { get; set; }
        [LwSerialize]
        public bool UseAccessGinForEnterToMenu { get; set; }
        [LwSerialize]
        public string GinForEnterToMenu { get; set; }
        [LwSerialize]
        public Guid GuidSecurityDailyPlanForEnterToMenu { get; set; }
        [LwSerialize]
        public Guid GuidSecurityTimeZoneForEnterToMenu { get; set; }

        [LwSerialize]
        public bool? AlarmOffline { get; set; }
        [LwSerialize]
        public bool? BlockAlarmOffline { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmOfflineObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmOfflineId { get; set; }

        [LwSerialize]
        public bool? AlarmTamper { get; set; }
        [LwSerialize]
        public bool? BlockAlarmTamper { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmTamperObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmTamperId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmTamper { get; set; }

        [LwSerialize]
        public FunctionKey FunctionKey1 { get; set; }
        [LwSerialize]
        public FunctionKey FunctionKey2 { get; set; }

        [LwSerialize]
        public byte? QueryDbStamp { get; set; }
        private Guid _guidSpecialOutputForTamper = Guid.Empty;
        [LwSerialize]
        public Guid GuidSpecialOutputForTamper { get { return _guidSpecialOutputForTamper; } set { _guidSpecialOutputForTamper = value; } }
        private Guid _guidSpecialOutputForOffline = Guid.Empty;
        [LwSerialize]
        public Guid GuidSpecialOutputForOffline { get { return _guidSpecialOutputForOffline; } set { _guidSpecialOutputForOffline = value; } }

        [LwSerialize]
        public bool? InvalidGinRetriesLimitEnabled { get; set; }
        [LwSerialize]
        public bool? InvalidPinRetriesLimitEnabled { get; set; }

        [LwSerialize]
        public List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        [LwSerialize]
        public virtual bool DisableScreensaver { get; set; }

        public Guid GetGuid()
        {
            return IdCardReader;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.CardReader;
        }

        public override string ToString()
        {
            return "CR" + Address;
        }
    }
}
