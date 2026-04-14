#pragma once
#include "System\baseTypes.h"
#include "Clsp485\clsp485.h"

/* Possible event callbacks that can be reported */
#define COMM_ON_INPUT_CHANGE            (1 << 0)
#define COMM_ON_SPECIAL_INPUT_CHANGE    (1 << 1)
#define COMM_ON_OUTPUT_CHANGE           (1 << 2)
#define COMM_ON_DIP_CHANGE              (1 << 3)
#define COMM_ON_DSM_CHANGE              (1 << 4)
#define COMM_ON_OUTPUT_LOGIC_CHANGE     (1 << 5)
#define COMM_ON_CR_ONLINE_STATE_CHANGE  (1 << 6)
#define COMM_ON_CR_DATA_RECEIVED        (1 << 7)

/* DSM configuration */
#define COMM_SET_PUSHBUTTON         0x10
#define COMM_UNSET_PUSHBUTTON       0x11
#define COMM_SET_SENSOR             0x12
#define COMM_UNSET_SENSOR           0x13
#define COMM_SET_EL_STRIKE          0x14
#define COMM_SET_BYPAS_ALARM        0x15
#define COMM_UNSET_ACTUATOR         0x16
#define COMM_ENABLE_ALARMS          0x17
#define COMM_SET_TIMMINGS           0x18
#define COMM_SET_SPECIAL_OUTPUT     0x19
#define COMM_UNSET_SPECIAL_OUTPUT   0x1A
#define COMM_START_DSM              0x1B
#define COMM_STOP_DSM               0x1C
#define COMM_SIGNAL_AG              0x1D
#define COMM_INVOKE_INTRUSION_ALARM 0x1E
#define COMM_FORCE_UNLOCKED         0x1F

/* Read commands */
//#define COMM_READ_INPUT_CNT         0x20
//#define COMM_READ_OUTPUT_CNT        0x21
#define COMM_READ_DEVICE_INFO       0x20
#define COMM_READ_FW_VERSION        0x22
#define COMM_READ_MEMORY_LOAD       0x23
#define COMM_DEBUG_SEQ_REQUEST      0x24

/* Input configuration */
#define COMM_UNSET_INPUT            0x30
#define COMM_SET_BSI_LEVELS         0x31
#define COMM_SET_BSI_PARAMS         0x32
#define COMM_SET_DI_PARAMS          0x33
#define COMM_BIND_OUT_TO_IN         0x34
#define COMM_UNBIND_OUT_TO_IN       0x35
#define COMM_REMAP_BSI              0x36
#define COMM_REMAP_DI               0x37

/* Output configuration */
#define COMM_SET_REPORTED_OUTPUT    0x38
#define COMM_OUTPUT_LEVEL           0x39
#define COMM_OUTPUT_FREQUENCY       0x3A
#define COMM_OUTPUT_PULSE           0x3B
#define COMM_ACTIVATE_OUTPUT        0x3C
#define COMM_UNSET_REPORTED_OUTPUT  0x3D
#define COMM_FORCE_OUTPUT_OFF       0x3E
#define COMM_SET_BLOCKED_OUTPUT_EX  0x3F

#define COMM_SET_REPORTED_OUT_LOGIC     0xC0
#define COMM_UNSET_REPORTED_OUT_LOGIC   0xC1
#define COMM_SET_REPORTED_OUT_LOGIC_EX  0xC2
#define COMM_SET_REPORTED_OUTPUT_EX 0xC3

#define COMM_SET_REPORTED_INPUTS    0xC4
#define COMM_UNSET_REPORTED_INPUTS  0xC5
#define COMM_SET_REPORTED_INPUTS_EX 0xC6

/* Cr commands */
#define COMM_CR_REQUEST             0x40

/* Report commands */
#define COMM_ACK                    0x50
#define COMM_NACK                   0x51
#define COMM_INPUT_CHANGED          0x52
#define COMM_SPECIAL_INPUT_CHANGED  0x53
#define COMM_DIP_CHANGED            0x54
#define COMM_OUTPUT_CHANGED         0x55
#define COMM_DSM_CHANGED            0x56
#define COMM_DEVICE_INFO            0x57
//#define COMM_INPUT_CNT              0x57
//#define COMM_OUTPUT_CNT             0x58
#define COMM_OUTPUT_LOGIC_STATE     0x59
#define COMM_CR_ONLINE_STATE_CHANGED      0x60
#define COMM_CR_DATA_RECEIVED         0x61
#define COMM_FW_VERSION             0x62
#define COMM_MEMORY_LOAD            0x63
#define COMM_DEBUG_SEQ_RESPONSE     0x64
#define COMM_MEMORY_RESTORE         0x65

/* Miscellanous settings */
#define COMM_SET_TIME               0x80
#define COMM_RESTART_DCU            0x81
#define COMM_IO_TEST_PROGRESS       0x82
#define COMM_IO_TEST_TIMEOUT        0x83
#define COMM_START_IO_TEST          0x84
#define COMM_IO_TEST_OK             0x85
#define COMM_SET_CRP_LEVEL          0x86

#ifdef DEBUG_GENERATOR
#define COMM_TOGGLE_CR_GENERATOR    0x90
#define COMM_TOGGLE_ADC_GENERATOR   0x91
#endif

/* DSM continued */
#define COMM_SET_CRS                0xA0
#define COMM_SUPPRESS_CR            0xA1
#define COMM_LOOSE_CR                0xA2
#define COMM_SET_IMPLICIT_CODE      0xA3

/* Upgrade commands */
#define COMM_RESET_TO_BOOTLOADER    0x00
#define COMM_RESET_TO_APPLICATION   0x01
#define COMM_START_UPGRADE          0x02
#define COMM_WRITE_DATA             0x03
#define COMM_UPGRADE_DONE           0x04
#define COMM_SEND_NEXT_DATA         0x05
#define COMM_RESEND_LAST_DATA       0x06
#define COMM_ERASE_NEXT_PAGE        0x07
#define COMM_ERASE_OK               0x08
#define COMM_ERASE_ERROR            0x09
#define COMM_READY_FOR_UPGRADE      0x0A
#define COMM_WRITE_CHECKSUM         0x0B
#define COMM_WRITE_APP_LENGTH       0x0C
#define COMM_CTRL_DATA_OK           0x0D
#define COMM_CTRL_DATA_ERROR        0x0E
#define COMM_REQUEST_UPGRADE        0x0F
#define COMM_IN_BOOTLOADER          0x10
#define COMM_READ_BASE_ADDRESS      0x11
#define COMM_BASE_ADDRESS           0x12

typedef enum 
{
    Comm_IndexOutOfRange = 1,
    Comm_IndexAlreadyUsed = 2,
    Comm_WrongArgument = 3,
    Comm_UnknownActuator = 4
} comm_ResultCodes_e;

/** 
 * @summary Register functions callbacks for events that should be reported to CCU
 * @param callbacks - use defines (see above) for the required callbacks 
 */
//#ifndef BOOTLOADER
//void Comm_RegisterCallbacks(uint32 callbacks);
//#endif

void Comm_FireMemoryWarning(uint32 allocated, uint32 seq);

void Comm_FireMemoryRestore(uint32 allocated);

/**
 * @summary Process command from the master frame 
 * @param command - command specifying requested operation
 * @param data - optional data further specifying the operation
 */
//void Comm_ProcessCommand(uint32_t command, uint8_t* data);

//volatile void Comm_OnNodeAssigned(uint32_t logicalAddress);

//volatile void Comm_OnNodeReleased();

//Clsp485Frame_t* Comm_OnFrameReceived(Clsp485Frame_t* masterFrame, ProtocolId_t protocol, BOOL* isTopPriority);

void Comm_Init();
