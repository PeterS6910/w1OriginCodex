using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Globals
{
    public enum ElectricStrikeState : byte
    {
        Locked = 0,
        Locking = 1,
        Unlocked = 2,
        Unlocking = 3
    }

    [LwSerialize(213)]
    public enum BlockingType : byte
    {
        NotBlocked = 0,
        ForcefullyBlocked = 1,
        ForcefullySet = 2,
        BlockedByObject = 3
    }

    public enum UnpackPackageFailedCode : byte
    {
        UnsupportedHeaderFormat = 0,
        UnpackFailed = 1,
        GetChecksumFailed = 2,
        ChecksumDoesNotMatch = 3,
        Other = 4
    }

    public enum RequiredLicenceProperties : byte
    {
        DoorEnvironmentCount = 0,
        Graphics = 1,
        CCU40MaxDsm = 2,
        CAT12CEMaxDsm = 3,
        CCU05MaxDsm = 4,
        Cat12ComboCount = 5,
        TimetecMaster = 6
    }

    public enum AlarmAreaActionResult : byte
    {
        Success = 0,
        FailedCCUOffline = 1,
        FailedInputInAlarm = 2,
        FailedNoImplicitManager = 3,
        SetUnsetNotConfirm = 4,
        FailedTimeBuyingNotEnabled = 5,
        FailedAlarmAreaNotFound = 6,
        FailedInsufficientRights = 7,
        FailedDueError = 8,
        FailedTimeAlreadyBought = 9,
        FailedTotalBoughtTimeReached = 10,
        FailedBoughtTimeDurationExceeded = 11,
        FailedTimeToBuyIsZero = 12,
        FailedTimeBuyingNotAvaible = 13
    }

    public enum MultiDoorType : byte
    {
        MultiDoor = 0,
        Elevator = 1
    }

    [LwSerialize(216)]
    public enum AlarmAreaAutomaticActivationMode : byte
    {
        TryToSetAreaUntilNotCalm = 0,
        UnconditionalSet = 1
    }

    [LwSerialize(333)]
    public enum SensorBlockingType : byte
    {
        Unblocked = 0,
        BlockTemporarilyUntilAlarmAreaUnset = 1,
        BlockTemporarilyUntilSensorStateNormal = 2,
        BlockPermanently = 3
    }

    [LwSerialize(334)]
    public enum SensorPurpose : byte
    {
        BurglaryAlarm = 0,
        FireAlarm = 1,
        SprinklerAlarm = 2,
        WaterAlarm = 3,
        HeatAlarm = 4,
        GasAlarm = 5,
        HoldUpAlarm = 6,
        PanicAlarm = 7,
        FreezeAlarm = 8
    }

    [LwSerialize(335)]
    public enum ApbzCardReaderEntryExitBy : byte
    {
        AccessPermitted = 0,
        NormalAccess = 1,
        AccessInterupted = 2
    }

    [LwSerialize(337)]
    public enum TimeBuyingMatrixState
    {
        O4AA_ON_AND_O4TBA_ON,
        O4AA_ON_AND_O4TBA_OFF,
        O4AA_OFF_AND_O4TBA_ON,
        O4AA_OFF_AND_O4TBA_OFF,
        O4TBA_ON,
        O4TBA_OFF
    }
}

