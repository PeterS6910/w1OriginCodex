namespace Contal.Cgp.NCAS.Definitions
{
    #region DSM related
    public enum DoorEnvironmentState : byte
    {
        Unknown = 0xff,
        Locked = 0,
        Unlocking = 1,
        Unlocked = 2,
        Opened = 3,
        Locking = 4,
        Intrusion = 5,
        Sabotage = 6,
        AjarPrewarning = 7,
        Ajar = 8
    }

    public enum DoorEnvironmentStateDetail : byte
    {
        None = 0xff,
        APASRestored = 0,
        InterruptedAccess = 1,
        NormalAccess = 2,
        AccessViolated = 3,
        ViolationReturn = 4
    }

    public enum DoorEnvironmentAccessTrigger : byte
    {
        InternalPushButton = 0,
        ExternalPushButton = 1,
        InternalCardReader = 2,
        ExternalCardReader = 3,
        CardReader = 4,
        None = 0xff
    }

    public enum DoorEnviromentType : byte
    {
        Standard = 0,
        Rotating = 1,
        StandardWithLocking = 2,
        StandardWithMaxOpened = 3,
        Minimal = 4,
        Turnstile = 5
    }

    public enum ActuatorType
    {
        ElectricStrike = 0,
        ElectricStrikeOpposite = 1,
        ExtraElectricStrike = 2,
        ExtraElectricStrikeOpposite = 3
    }

    public enum DsmAccessGrantedSeverity
    {
        NormalCard = 0,
        EmergencyCode = 1,
        ForceUnlockedToForceLocked = 2
    }

    public enum PushButtonType
    {
        Internal = 1,
        External = 2
    }

    public enum SensorType
    {
        DoorLocked = 0,
        DoorOpened = 1,
        DoorFullyOpened = 2
    }

    public enum SpecialOutputType
    {
        Ajar = 0,
        Intrusion = 1,
        Sabotage = 2
    }

    public enum OutputType
    {
        Level = 0,
        Frequency = 1,
        Impulse = 2
    }

    public enum StrikeType
    {
        Level = 0,
        Impulse = 1
    }
    #endregion

    public enum DeviceType
    {
        CAT12CE = 0,
        CCU = 1,
        DCU = 2,
        CR = 3,
        Client = 4,
        Other = 5
    }

    public enum InputType:byte
    {
        Unconfigured = 0xFF,
        DI = 0,
        BSI = 1,
        MBSI = 2,
        AI = 3
    }
    

//#if ! COMPACT_FRAMEWORK
    public enum MainBoardVariant
    {
        Unknown = -1,
        CAT12CE = 0,
        CCU12 = 1,
        CCU40 = 2,
        CCU0_ECHELON = 3,
        CCU05 = 4,
        CAT12CE_AUDIO = 5,
        CCR_PLUS = 6,
        CCU0_RS485 = 7
    }
    //#endif

    #region NodeDataProtocol related


    public enum CRPLevel
    {
        None = 0,
        NoISP = 1,
        CRP1 = 2,
        CRP2 = 3,
        CRP3 = 4
    }



    public enum SpecialInput
    {
        Tamper = 7,
        Fuse = 8,
        ExtensionBoardFuse = 9,
        ExtensionBoardPower = 10
    }

    public enum GeneratedCardType
    {
        MifareCSN,
        MifareSector
    }
    #endregion
}
