namespace Contal.Cgp.Globals
{
    public enum CardReaderSceneType : byte
    {
        Unknown = 0xFF,
        
        AlarmPanelState = 0xFE,

        DoorUnlocked = 2,
        DoorLocked = 4,
        WaitingForCardPin = 8,
        WaitingForCard = 9,
        WaitingForCode = 10,
        WaitingForPIN = 11,
        Rejected = 12,
        OutOfOrder = 13,
        AlarmAreaSet = 14,
        AlarmAreaUnset = 15,
        AlarmAreaSetCard = 16,
        AlarmAreaSetCardPin = 17,
        AlarmAreaSetPIN = 18,
        AlarmAreaSetCode = 19,
        AlarmAreaUnsetCard = 20,
        AlarmAreaUnsetCardPin = 21,
        AlarmAreaUnsetPIN = 22,
        AlarmAreaUnsetCode = 23,
        AlarmAreaSetSucceeded = 24,
        AlarmAreaSetFailed = 25,
        AlarmAreaUnconditionalSet = 26,
        AlarmAreaUnconditionalSetSucceeded = 27,
        AlarmAreaUnconditionalSetFailed = 28, // RG_RFA Unhandled, only called SetCommand

        //Command for card reader
        EnterToMenuAlarmAreasCard = 30,
        EnterToMenuAlarmAreasCardPin = 31,
        EnterToMenuAlarmAreasPin = 32,
        EnterToMenuAlarmAreasCode = 33, // RG_RFA Unhandled
        EnterToSetUnsetMenuAlarmAreaCode = 34,
        MenuAlarmAreaSet = 35,
        MenuAlarmAreaUnset = 36,
        MenuAlarmAreaSetCode = 37,
        MenuAlarmAreaUnsetCode = 38,
        MenuAlarmAreaUnconditionalSet = 39,
        MenuAlarmAreaSetSucceeded = 40,
        MenuAlarmAreaSetFailed = 41,
        MenuAlarmAreaUnconditionalSetSucceeded = 42,
        MenuAlarmAreaUnconditionalSetFailed = 43,
        MenuAlarmAreaUnsetSucceeded = 44,
        MenuAlarmAreaUnsetFailed = 45,
        WaitingForEmergencyCode = 46,
        NoAccessToThisMenu = 47,
        NoAvailableItemsInMenu=48, // RG_RFA Unhandled
        NoAlarmAreasToSetUnset = 49,
        NotUsedInDoorEnvironment=50, // RG_RFA Unhandled
        MenuAlarmAreaSetAll = 51,
        MenuAlarmAreaUnsetAll = 52,
        EnterToMenuSensorsCard = 53,
        EnterToMenuSensorsCardPin = 54,
        EnterToMenuSensorsPin = 55,
        MenuSensorsForAlarmArea = 57,
        MenuSensor = 58,
        MenuBlockInputTemporarilyAreaUnset = 59,
        MenuBlockInputPermanently = 60,
        MenuUnblockInput = 61,
        MenuAcknowledgeInputAlarm = 62,
        MenuSensors = 63,
        MenuAlarmStateForAlarmArea = 64,
        MenuAlarmStateAcknowledge = 65,
        MenuAlarmStateAcknowledgeAndBlock = 66,
        MenuAlarmStateBlock = 67,
        MenuBlockInputTemporarilySensorStateNormal = 68,
        MenuAlarmAreasEnterPIN = 69,
        //end commands for card reader
        Accepted = 70, // RG_RFA Unhandled, used in PC
        MainMenu = 71, // RG_RFA Unhandled, only assigned
        QueryDbStamp = 72, // RG_RFA Unhandled, 
        ValidateCardSystem = 73, // RG_RFA Unhandled
        SetCardSystem = 74, // RG_RFA Unhandled
        WaitingForCodeOrCard = 75,
        WaitingForCodeOrCardPin = 76,
        AlarmAreaUnsetEntry = 77,
        AlarmAreaUnsetExit = 78,
        AlarmAreaAlarm = 79,

        MenuEventlogsCard = 80, // RG_RFA Unhandled
        MenuEventlogsCardPIN = 81, // RG_RFA Unhandled
        MenuEventlogsCode = 82,
        MenuEventlogsPIN = 91,

        MenuFunctionKey1 = 83, // RG_RFA Unhandled
        MenuFunctionKey2 = 90, // RG_RFA Unhandled
        MenuFunctionKeyCard = 84, // RG_RFA Unhandled
        MenuFunctionKeyCardPIN = 85, // RG_RFA Unhandled
        MenuFunctionKeyPIN = 86,
        MenuFunctionKeyCodeOrCard = 87, // RG_RFA Unhandled
        MenuFunctionKeyCodeOrCardPin = 88, // RG_RFA Unhandled
        MenuFunctionKeyCode = 89, // RG_RFA Unhandled

        AlarmAreaUnsetTimeBuyingImplicit = 92,
        AlarmAreaUnsetTimeBuying = 93,
        AlarmAreaUnsetTimeBuyingWaitForResult = 94,
        AlarmAreaUnsetTimeBuyingSucceeded = 95,
        AlarmAreaUnsetTimeBuyingFailed = 96,
        AlarmAreaUnsetAllTimeBuying = 97,
    }

}
