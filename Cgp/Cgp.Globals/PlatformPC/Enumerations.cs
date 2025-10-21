
namespace Contal.Cgp.Globals
{
    public enum ReadWritePermissionState
    {
        None,
        Read,
        Write,
        ReadWrite
    }
    public enum UpgradeStage
    {
        Finished = 0,
        Transfering = 1,
        Upgrading = 2,
        TransferFailed = 3,
        UpgradeFailed = 4,
        NonDefined = 5
    }
    public enum CCUFileTransferPurpose : byte
    {
        CCUUpgrade = 0,
        DCUUpgrade = 1,
        CEUpgrade = 2,
        CRUpgrade = 3
    }
    public enum ActionUpgrade
    {
        BEGIN_UPGRADE = 1,
        FINALIZE_UPGRADE = 2
    };

    public enum ActionResultUpgrade
    {
        Unkonwn = -1,
        Success = 0,
        Started = 1,
        ChecksumInvalid = 2,
        FlashingError = 3,
        SignatureFileError = 4,
        RegistryBackupError = 5,
        InvalidCeImageFile = 6,
        ImageFileNotHaveBuildNumber = 7,
        ImageFileNoExist = 8,
        AfterAllRetryActionTimeout = 9,
        IsJustRunning = 10,
        StillRunning = 11,                 //finalize try again later 
        BeginUpgradeMustBeCallFirst = 12
    };

    public enum RequiredLicenceProperties
    {
        Edition = 0,
        ConnectionCount = 1,
        CISIntegration = 2,
        OfflineImport = 3,
        MajorVersion = 4,
        IdManagement = 5,
        MaxSubsiteCount = 6
    }

    public enum CcuConfigurationOptions
    {
        Enable = 0,
        Disable = 1,
        EnableNewPassword = 2,
        EnableVerifyPassword = 3,
        DiableServerSettingPwd = 4
    }

    public enum AlarmTypeBuzzer
    {
        AlarmNotAcknowledged = 0,
        Alarm = 1,
        NotAcknowledged = 2,
        NotSelected = 3
    }

    public enum TUpsOnlineState
    {
        Unknown,
        Online,
        Offline
    }

    public enum TUpsAlarm
    {
        OutputFuse,
        OutputPowerOutOfTolerance,
        PrimaryPowerMissing,
        BatteryFault,
        BatteryEmpty,
        BatteryFuse,
        Overtemperature,
        Tamper,
    }
}
