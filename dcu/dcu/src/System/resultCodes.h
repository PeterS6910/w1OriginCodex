#pragma once

/* Results returned by Sensor & Actuator settings routines */
typedef enum
{
    R_Ok = 0,                     /* configuration ok */
    R_Error = 1,
    R_IndexOutOfRange = 2,            /* input/output id is greater than available inputs/outputs count */
    R_IndexAlreadyUsed = 3,           /* input/output already used for other functionality */
    R_InvalidParameter = 4,            /* in case some of the parameters does not fit ;) */
    R_UnknownActuator = 5,            /* returned when actuator id higher than highest possible */
    R_InvalidES = 6,                  /* returned when request to set electric strike that is not defined */
    R_UnknownSensor = 7,              /* returned when sensor id higher than highest possible */
    R_UnknownTimming = 8,
    R_UnknownDelay = 9,
    R_OutputNotAssigned = 10,          /* if the DSM tries to use output that is not assigned */
    R_OutputNotConfigured = 11,        /* if the DSM tries to use output that is not configured */
    R_SensorActivated = 12,                  /* sensor in active state */
    R_SensorNotActivated = 13,               /* sensor in not active state */
    R_SensorIntrusion = 14,
    R_SensorSabotage = 15,                  /* sensor in intrusion state */
    R_SensorNotChanged = 16,
    R_SensorNotConfigured = 17,
    R_PrepareSectorError = 18,
    R_CopyRamToFlashError = 19,
    R_ErasePageError = 20,
    R_BufferTooSmall = 21,
    R_CRIsOffline = 22,
    R_ElectricStrikeNotConfigured = 23,
    R_OpenDoorNotConfigured = 24,
    R_DoorLockedNotConfigured = 25, 
    R_DSMSettingsLocked = 26,
    R_DoorOpenedNotSupported = 27,
    R_DoorLockedNotSupported = 28,
    R_DoorFullyOpenedNotSupported = 29,
    R_CROutOfRange = 30,
    R_OutOfAccessCmdRange = 31,
    R_ImplicitCodeNotDefined = 32,
    R_CardReaderNotAssigned = 33,

    R_InvalidDataLength = 0xEE,
    R_UnknownCommand = 0xFF,
    R_32bitFiller = 0xFFFFFFFF
} Result_e;