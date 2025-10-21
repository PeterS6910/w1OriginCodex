using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.Globals
{
    [Serializable]
    [LwSerialize(704)]
    public enum AlarmState : byte
    {
        Alarm = 0,
        Normal = 1
    }

    [Serializable]
    [LwSerialize(705)]
    public enum AlarmType : byte
    {
        GeneralAlarm = 0,

        ICCU_SendingOfObjectStateFailed = 0x30,
        ICCU_PortAlreadyUsed = 0x31,

        CCU_Offline = 0x40,
        CCU_TamperSabotage = 0x41,
        CCU_Unconfigured = 0x42,
        CCU_ClockUnsynchronized = 0x43,
        CCU_OutdatedFirmware = 0x44,
        CCU_PrimaryPowerMissing = 0x45,
        CCU_BatteryLow = 0x46,
        Ccu_Ups_OutputFuse = 0x23,
        Ccu_Ups_BatteryFault = 0x24,
        Ccu_Ups_BatteryFuse = 0x25,
        Ccu_Ups_Overtemperature = 0x26,
        Ccu_Ups_TamperSabotage = 0x27,
        CCU_ExtFuse = 0x47,
        CCU_DataChannelDistrupted = 0x48,
        CCU_CoprocessorFailure = 0x49,
        CCU_HighMemoryLoad = 0x20,
        CCU_FilesystemProblem = 0x21,
        CCU_SdCardNotFound = 0x22,
        Ccu_CatUnreachable = 0x28,
        Ccu_TransferToArcTimedOut = 0x29,

        DCU_Offline = 0x50,
        DCU_TamperSabotage = 0x51,
        DCU_Unconfigured = 0x52,
        DCU_WaitingForUpgrade = 0x53,
        DCU_OutdatedFirmware = 0x54,
        DCU_Offline_Due_CCU_Offline = 0x55,

        Input_Alarm = 0x61,
        Input_Tamper = 0x62,
        Output_Alarm = 0x65,
        Sensor_Alarm = 0x66,
        Sensor_Tamper_Alarm = 0x67,

        CardReader_Offline = 0x70,
        CardReader_TamperSabotage = 0x71,
        CardReader_AccessDenied = 0x72,
        CardReader_UnknownCard = 0x73,
        CardReader_CardBlockedOrInactive = 0x74,
        CardReader_InvalidPIN = 0x75,
        CardReader_InvalidCode = 0x76,
        CardReader_AccessPermitted = 0x77,
        CardReader_InvalidEmergencyCode = 0x78,
        CardReader_Offline_Due_CCU_Offline = 0x79,


        DoorEnvironment_Intrusion = 0x81,
        DoorEnvironment_DoorAjar = 0x82,
        DoorEnvironment_Sabotage = 0x83,

        CardReader_Invalid_Pin_Retries_Limit_Reached = 0x84,
        CardReader_Invalid_Code_Retries_Limit_Reached = 0x85,

        AlarmArea_Alarm = 0x91, // stands also for B-alarm
        AlarmArea_AAlarm = 0x92,
        AlarmArea_ReportingToCR = 0x93,
        AlarmArea_SetByOnOffObjectFailed = 0x94

        // nove alarmy pridavaj do jednotlivych kategorii     

    }

    [Serializable]
    [LwSerialize(706)]
    public enum ParameterType : byte
    {
        DateTimeCcu = 0,
        DateTimeServer = 1,
        FileName = 2,
        CardNumber = 3,
        CatIpAddress = 4,
        ArcName = 5
    }

    [Serializable]
    public enum AlarmPriority : byte
    {
        Critical = 0,
        High = 1,
        Medium = 2,
        Low = 3
    }
}
