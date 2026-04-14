namespace Contal.Cgp.NCAS.NodeDataProtocol
{
    public enum NodeErrorCode
    {
        Ok = 0,                     /* configuration ok */
        Error = 1,
        IndexOutOfRange = 2,            /* input/output id is greater than available inputs/outputs count */
        IndexAlreadyUsed = 3,           /* input/output already used for other functionality */
        InvalidParameter = 4,            /* in case some of the parameters does not fit ;) */
        UnknownActuator = 5,            /* returned when actuator id higher than highest possible */
        InvalidES = 6,                  /* returned when request to set electric strike that is not defined */
        UnknownSensor = 7,              /* returned when sensor id higher than highest possible */
        UnknownTimming = 8,
        UnknownDelay = 9,
        OutputNotAssigned = 10,          /* if the DSM tries to use output that is not assigned */
        OutputNotConfigured = 11,        /* if the DSM tries to use output that is not configured */
        SensorActivated = 12,                  /* sensor in active state */
        SensorNotActivated = 13,               /* sensor in not active state */
        SensorIntrusion = 14,
        SensorSabotage = 15,                  /* sensor in intrusion state */
        SensorNotChanged = 16,
        SensorNotConfigured = 17,
        PrepareSectorError = 18,
        CopyRamToFlashError = 19,
        ErasePageError = 20,
        BufferTooSmall = 21,
        CRIIsOffline = 22,
        ElectricStrikeNotConfigured = 23,
        OpenDoorNotConfigured = 24,
        DoorLockedNotConfigured = 25,
        DSMSettingsLocked = 26,
        DoorOpenedNotSupported = 27,
        DoorLockedNotSupported = 28,
        DoorFullyOpenedNotSupported = 29,
        CardReaderOutOfRange = 30,
        OutOfAccessCommandRange = 31,
        ImplicitCodeNotDefined = 32,
        CardReaderNotAssigned = 33,

        AlreadyUsedAsSensor = 34,
        AlreadyUsedAsAgSource = 35,
        NotAssignedAsSpecialOutpu = 36,
        NotAssignedAsSensor = 37,
        OutputBlocked = 38,

        ElectricStrikeOppositeNotConfigured = 39,

        NotImplemented = 0xED,
        InvalidDataLength = 0xEE,
        ObsoleteCommand = 0xFE,
        UnknownCommand = 0xFF
    }
}
