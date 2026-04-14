#pragma once

#include "System\constants.h"
#include "System\resultCodes.h"
#include "System\baseTypes.h"

#define UNCONFIGURED_IO 0xff

#define DSM_ACTUATOR_COUNT          5
#define DSM_SENSOR_COUNT            3
#define DSM_TIMMING_COUNT           4
#define DSM_DELAYS_COUNT            4
#define DSM_SPECIAL_OUTPUT_COUNT    3
#define DSM_AG_SRC_COUNT            2
#define DSM_READER_COUNT            2



typedef enum
{
    DEStandard = 0,
    DERotating = 1,
    DEStandardWithLocking = 2,
    DEStandardWithMaxOpened = 3,
    DEMinimal = 4
} DoorEnviromentType_e;


/* Actuator enumeration */
typedef enum
{
    ElectricStrike = 0,                 /* electric strike */
    ElectricStrikeOpposite = 1,         /* electric strike opposite */
    ExtraElectricStrike = 2,            /* extra electric strike */
    ExtraElectricStrikeOpposite = 3,    /* extra electric strike opposite */
    AlarmBypass = 4                     /* alarm bypass */
} DSMActuators_e;

typedef enum
{
    StrikeTypeLevel = 0,
    StrikeTypePulse = 1
} DSMStrikeType_e;

/* Sensor enumeration */
typedef enum
{
    DoorLocked = 0,
    DoorOpened = 1,
    DoorFullyOpened = 2
} DSMSensors_e;

/* Timming configuration enumeration */
typedef enum
{
    MaxUnlockTime = 0,          /* Maximal time that the lock will be held open */
    MaxOpenTime = 1,            /* Max time for the door to be open */
    DoorPreAjarTime = 2,        /* Specifies when the pre ajar warning starts */
    SireneAjarDelay = 3          /* ??? */
} DSMDoorTimming_e;

/* Delays configuration enumeration */
typedef enum
{
    DelayBeforeUnlock = 0,  /* delay before the electric strike activated after unlock APAS is '1' */
    DelayBeforeLock,        /* delay before the electric strike deactivated after unlock APAS is '0' */
    DelayBeforeClose,       /* delay off for door sensor */
    DelayBeforeIntrusion    /* ??? */
} DSMDoorDelays_e;

/* DSM machine states */
typedef enum
{
    StateLocked,                   /* Idle state, lock and door is closed */
    StateUnlocking,
    StateUnlocked,               /* Lock is opened, door is still closed */
    StateDoorOpened,             /* Lock is closed, door is opened */
    StateLocking,                /* Closing, delay handling -> back to idle */
    StateIntrusion
} DSMMachineStates_e;

/* Possible states signaled to upper layer (CCU) */
typedef enum
{
    SignalLocked = 0,
    SignalUnlocking = 1,
    SignalUnlocked = 2,
    SignalDoorOpened = 3,
    SignalLocking = 4,
    SignalIntrusion = 5,
    SignalSabotage = 6,
    SignalPreAjar = 7,
    SignalAjar = 8,
    SignalNone = 0xff
} DSMSignals_e;

typedef enum
{
    AGS_InternalPushButton = 0,
    AGS_ExternalPushButton = 1,
    AGS_CardReader = 2,
    AGS_None = 0xff
} DSMAccessGrantedSource_e;

typedef enum
{
    InfoNone = 0xff,
    InfoApasRestored = 0,
    InfoInterruptedAccess = 1,
    InfoNormalAccess = 2,
    InfoAccessViolated = 3,
    InfoViolationReturn = 4
} DSMSignalInfo_e;

typedef enum
{
    SpecialDOAjar = 0,
    SpecialDOIntrusion = 1,
    SpecialDOSabotage = 2
} DSMSpecialDOType_e;

typedef enum 
{
    DSM_NormalCard = 0,
    DSM_EmergencyCard = 1
} DSMAGSourceCard_e;

#define DSM_CR_SUPPRESSED           (1 << 0)            /* Mark when the CR is suppressed and not suppossed to receive messages from DSM */
#define DSM_CR_MSG_SET              (1 << 1)            /* Mark that implicit CR message is ready */
#define DSM_CR_SKIP_IMPLICIT_CMD    (1 << 2)
#define DSM_CR_SKIP_OPT_MSG         (1 << 3)
#define DSM_CR_LOCKED               (1 << 4)            /* Mark that implicit code for CR is AlarmArea set or Locked */

#define DSM_CR_LOCKED_WITH_EXCEPTION    0x30

/* DSM Assigned CR structure */
typedef struct
{
    uint32 address;
    uint32 flags;              /* bit 0 - is suppressed, bit 1 - cr message set, bit 2 - skip implicit cmd, bit 3 - skip opt. message, 4 - CR locked or AA set */
    uint32 implicitCode;        /* the code that will be sent to CR when DSM is Locked state */
    int32 optionalParameter;         /* optional parameter for the implicit code, -1 when not used */
    bool intrusionOnlyViaLed;

    uint32 msgCode;
    uint32 optDataLength;
    uint8* optData;
} DSMAssignedCR_t;

/* DSM configuration structure */
typedef struct
{
    uint8 locked : 1;                           /* marks that DSM is running and no change to basic configuration is allowed */

    uint8 actuator[DSM_ACTUATOR_COUNT];
    uint8 sensor[DSM_SENSOR_COUNT];
    uint8 agSources[DSM_AG_SRC_COUNT];

    uint8 specialOutput[DSM_SPECIAL_OUTPUT_COUNT];

    DSMAssignedCR_t cardReaders[2];
//    uint32 assignedCRs;                       /* Assigned card readers that can receive raw commands (binary) */
//    uint32 activeCRs;                     /* Marks which card readers are able to receive raw commands (some CRs can be supressed) */

    uint32 timming[DSM_TIMMING_COUNT];
    uint32 delay[DSM_DELAYS_COUNT];
    uint8 invokeIntrusionAlarm : 1;

    uint32 usedOutputs;                       /* mark used outputs */
    uint8 usedSpecialOutputs[MAX_OUTPUT_COUNT];
    uint32 usedInputs;                        /* mark used inputs */

    uint8 ccuAsAgSource : 1;                  /* source of the Access Granted signal is CCU / button */
    uint8 preAjarConfigured : 1;              /* indicates that the pre ajar has been configured and should be used */

    /* Special outputs alarms */
    uint8 doorAjarAlarmEnabled : 1;
    uint8 intrusionAlarmEnabled : 1;
    uint8 sabotageAlarmEnabled : 1;

} DSMConfiguration_t;

typedef struct
{
    DoorEnviromentType_e enviromentType;

    DSMMachineStates_e state;               /* current state of the machine */

    uint8 signalAccessGranted : 1;        /* this can be either signal from CCU or signal from pushbutton */
    DSMAccessGrantedSource_e agSource;    /* source of access granted signal */

    uint8 preAjarActive : 1;              /* indicates active pre ajar sequence */
    uint8 ajarActive : 1;                 /* indicates active ajar warning */
    uint32 timer;                         /* helper timer for different stuff */

    uint8 forceUnlocked : 1;
    uint8 forceUnlockedByStart : 1;       /* marks that the force unlock was called before DSM start */
    uint8 locksOpen : 1;
    uint8 intrusion : 1;
    uint8 sabotaged : 1;                  /* DSM is sabotaged */
    uint8 pushButtonSabotaged : 1;        /* Signal that one or more pushbuttons were sabotaged */
    uint8 sabotagedSensor[DSM_SENSOR_COUNT];

    uint8 doorOpened : 1;                 /* indicate that the door is currently open */
    uint8 doorLocked : 1;                 /* Indicate that the door is currently locked */
    uint8 intrusionStarted : 1;           /* indicate that intrusion begun */
    uint8 intrusionSignalStarted : 1;     /* intrusion signal started (500ms) */

    uint32 intrusionTimer;                /* after timer runs over delay before intrusion time -> intrusion state */
    uint32 intrusionSignalTimer;          /* 500 ms timer for intrusion signal */

    uint8 doorAjarAlarmOutActivated : 1;     /* Marks that the output for ajar is activated */
    uint8 intrusionAlarmOutActivated : 1;   /* marks that output for intrusion is activated */
    uint8 sabotageAlarmOutActivated : 1;    /* makrs that output for saboteda is activated */
    
    uint8 doorOpenedAtStop : 1;             /* door was opened when DSM was stopped */
    uint32 stopTime;                        /* marks time when DSM was stopped, is used in case door was opened before stop,
                                                and DSM is started withing time limit -> DSM goes to door opened and not intrusion */
    
} DSMMachine_t;