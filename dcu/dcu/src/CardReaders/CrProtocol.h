#pragma once
#include "System\baseTypes.h"

#define CR_MAX_COUNT                    2


    // FOR SEPARATE SPEEDS - BCR
    #define FIRST_COMMON_CR_BAUDRATE        4800
    //#define SECOND_COMMON_CR_BAUDRATE       19200
    #define THIRD_COMMON_CR_BAUDRATE        38400

    // FOR FIXED SPEEDS - CCR
    #define FIXED_COMMON_CR_BAUDRATE    38400

#define CR_MAX_OPTIONAL_DATA_LENGTH     96 // LEAVE IT to the CR PROTOCOL MAXIMUM (96)
                                           // even if the maximum able to be tranfserred by LON is 40B-4B CR_CLSP_HEADER
                                            // CAUSE THERE CAN BE LOCALLY TRANSFERRED MESSAGES BIGGER THAN THOSE ENCAPS VIA CLSP




#define CR_PREAMBLE_LENGTH              1
#define CR_HEADER_LENGTH                5
#define CR_MAX_FRAME_LENGTH             CR_PREAMBLE_LENGTH + CR_HEADER_LENGTH + CR_MAX_OPTIONAL_DATA_LENGTH
#define CR_PREAMBLE_N_HEADER_LENGTH     6

#define CR_SET_PRESENTATION_MASK_ODL     13
#define CR_SET_COUNTRY_CODE_ODL          2

#define CR_FIRST_CCR_PROTO              3


typedef enum 
{
        CRMC_GET_EVENT = 0x00,
        CRMC_ACK_GET_EVENT = 0x01,
        CRMC_NACK = 0x02,
        CRMC_GET_CR_IDENTITY = 0x03,
        CRMC_GET_PROTOCOL_VERSION = 0x04,
        CRMC_RESET = 0x10,
        CRMC_GREEN_LED_MODE = 0x11,
        CRMC_RED_LED_MODE = 0x12,
        CRMC_BUZZER_MODE = 0x14,
        CRMC_EXTERNAL_BUZZER_MODE = 0x15,
        CRMC_KBD_LIGHT_CTRL = 0x16,
        CRMC_DISPLAY_ON = 0x20,
        CRMC_DISPLAY_OFF = 0x21,
        CRMC_CLEAR_DISPLAY = 0x22,
        CRMC_TEXT_INVERSION = 0x23,
        CRMC_SCROLL_DISPLAY = 0x24,
        CRMC_DISPLAY_DEFAULT_TEXT = 0x25,
        CRMC_DISPLAY_TEXT = 0x26,
        CRMC_REAL_TIME_PRESENTATION_ENABLE = 0x27,
        CRMC_REAL_TIME_PRESENTATION_DISABLE = 0x28,
        CRMC_DISPLAY_SYMBOL = 0x29,
        CRMC_LINE_SCROLL = 0x2A,
        CRMC_DISPLAY_INFO_TAB = 0x2B,
        
        CRMC_SET_TIME = 0x30,
        CRMC_SET_MAXIMUM_COMMUNICATION_RETRY = 0x31,
        CRMC_SET_COUNTRY_CODE = 0x32,
        CRMC_SET_PRESENTATION_MASK = 0x33,
        CRMC_SINGLE_KEY_MODE_ENABLE = 0x38,
        CRMC_SINGLE_KEY_MODE_DISABLE = 0x39,
        CRMC_SET_COMMUNICATION_LINE_MODE = 0x3A,
        CRMC_SPECIFIC_SETTING1 = 0x3B,
        CRMC_SPECIFIC_SETTING2 = 0x3C,
        CRMC_SET_CR_INITIALIZED = 0x3F,
        CRMC_OUT_OF_ORDER = 0x40,
        CRMC_DOOR_UNLOCKED = 0x41,
        CRMC_DOOR_LOCKED = 0x42,
        CRMC_WAITING_FOR_CARD = 0x43,
        CRMC_WAITING_FOR_GIN_CODE = 0x44,
        CRMC_WAITING_FOR_PIN_CODE = 0x45,
        CRMC_REJECT_REQUEST = 0x46,
        CRMC_ACCEPT = 0x47,
        CRMC_DOOR_OPEN = 0x48,
        CRMC_DOOR_LOCKING = 0x49,
        CRMC_WARNING_DOOR_OPENED_TOO_LONG = 0x4A,
        CRMC_ALARM_DOOR_OPENED_TOO_LONG = 0x4B,
        CRMC_ALARM_INTRUSION = 0x4C,
        CRMC_SET_ALARM_AREA = 0x50,
        CRMC_UNSET_ALARM_AREA = 0x51,
        CRMC_ALARM_AREA_IS_SET = 0x52,
        CRMC_ALARM_AREA_IS_UNSET = 0x53,
        CRMC_FN_BOX = 0x54,
        CRMC_UNCONDITIONAL_SET = 0x58,
        CRMC_UNACKNOWLEDGED_ALARMS = 0x59,
        
        // virtual command, not yet supported by CCRs
        CRMC_ALARM_AREA_TMP_UNSET = 0x5C,
       
       // virtual command, not yet supported by CCRs
        // reserved but also used for backward compatibility
        CRMC_ALARM_AREA_RESERVED = 0x5D,
        
        // virtual command, not yet supported by CCRs
        CRMC_ALARM_AREA_IN_ALARM = 0x5E,

        
        CRMC_MENU_SYSTEM = 0x60,
        CRMC_PUT_ANNOUNCMENT = 0x61,
     
        
        CRMC_RSP_ACK = 0x81,
        CRMC_RSP_NACK = 0x82,
        CRMC_SABOTAGE_ACTIVE = 0x90,
        CRMC_SABOTAGE_PASSIVE = 0x91,
        CRMC_RSP_RESET = 0x92,
        CRMC_CR_NOT_INITIALIZED = 0x93,
        CRMC_CR_IDENTITY = 0x94,
        CRMC_COMMUNICATION_MODE_CHANGED = 0x95,
        CRMC_PROTOCOL_VERSION = 0x96,
        CRMC_CARD_DATA = 0xA0,
        CRMC_CODE_DATA = 0xA1,
        CRMC_CODE_TIMEOUT = 0xA2,
        CRMC_SINGLE_KEY_PRESSED = 0xA3,
        CRMC_FNBOX_TIMEOUT = 0xA4  ,
        
        CRMC_SET_ENCRYPTION_KEY = 0x3B,
        CRMC_ENCRYPTED_COMMAND = 0x3C,
        CRMC_ACK_SET_KEY = 0x97,
        CRMC_ENCRYPTED_RESPONSE = 0x98,
        CRMC_CONF_CMD1 = 0x68,
        CRMC_CONF_CMD2 = 0x69,
        CRMC_CONF_CMD3 = 0x6A,
        CRMC_CONF_CMD4 = 0x6B,
        CRMC_CONF_CMD5 = 0x6C,
        CRMC_CONF_CMD6 = 0x6D,
        CRMC_CONF_CMD7 = 0x6E,
        CRMC_CONF_CMD9 = 0x6F,
        CRMC_CONF_RES1 = 0xB0,
        CRMC_CONF_RES2 = 0xB1,
        CRMC_CONF_RES3 = 0xB2,
        CRMC_CONF_RES4 = 0xB3,
        CRMC_CONF_RES5 = 0xB4,
        CRMC_CONF_RES6 = 0xB5,
        CRMC_CONF_RES7 = 0xB6,
        CRMC_CONF_RES9 = 0xB7,
        CRMC_CONF_RES10 = 0xB8,
        CRMC_CONF_RES11 = 0xB9,
        CRMC_CONF_RES12 = 0xBA,
        CRMC_CONF_RES13 = 0xBB,
        CRMC_CONF_RES14 = 0xBC,
        CRMC_CONF_RES15 = 0xBD,
        CRMC_CONF_RES16 = 0xBE,
        CRMC_CONF_RES17 = 0xBF,

        CRMC_SET_CARD_SYS = 0x69,
        CRMC_VALIDATE_CARD_SYS = 0x6A,
        CRMC_QUERY_DB_STAMP = 0x6C,
        CRMC_CONF_ACK = 0xB2,

        CRMC_SET_LOW_MENU_BUTTONS = 0x3E,
        
        CRMC_ENTER_FLASH_SERVICE = 0x78,
        CRMC_FLASH_UPLOAD = 0x79,

        CRMC_SERVICE_ACK = 0xF8,
        
        CRMC_STACKED_MESSAGE = 0xFD,
        CRMC_UNKNOWN = 0xFE

 } CRMessageCode_t;


typedef enum {
    CRS_NOT_RESPONDING,
    CRS_NOT_INITIALIZED,
    CRS_IN_SETTING_LOOP,
    CRS_MAIN_LOOP,
        
        // this value is for having the enum 4B    
        CRS_MAX_UINT =     0xFFFFFFFF

} CRState_e;

typedef struct CRReceivedMessage {
    byte crAddress;
    byte optionalDataLength;
    CRMessageCode_t messageCode;
    byte optionalData[CR_MAX_OPTIONAL_DATA_LENGTH];
} CRReceivedMessage_t;

typedef struct CRMessage {
    byte flags;
    byte crAddress;
    byte optionalDataLength;
    CRMessageCode_t messageCode;
    byte optionalData[0];
} CRMessage_t;


uint32 FromBcdByte(uint32 value);
uint32 ToBcdByte(uint32 value);

enum CRBaudRate
{
        BR1200 = 0x1,
        BR2400 = 0x2,
        BR4800 = 0x03,
        BR9600 = 0x04,
        BR19200 = 0x05,
        BR38400 = 0x06,
        BR57600 = 0x07,
        BR115200 = 0x08,
};

typedef enum CRBaudRate CRBaudRate_e;

uint32 CrProtocol_GetBaudRate(CRBaudRate_e input);
CRBaudRate_e CrProtocol_GetBaudRateEnum(uint32 input);

enum CRIndicatorMode
{
    CRIM_OFF = 0,
    CRIM_ON = 1,
    CRIM_CLICK = 2,
    CRIM_SHORT_PULSE = 3,
    CRIM_LONG_PULSE = 4,
    CRIM_ULTRA_LOW_FREQUENCY = 5,
    CRIM_LOW_FREQUENCY = 6,
    CRIM_HIGH_FREQUENCY = 7,
    CRIM_INTACT = 0x0F
};

typedef enum CRIndicatorMode CRIndicatorMode_e;

// icon indexes for DISPLAY_INFO_TAB command
typedef enum
{
        CRDITI_EMPTY  =                   0x00,
        CRDITI_NO_CHANGE =               0x80,
        // OPENED if ON , CLOSED if OFF
        CRDITI_DOORS_OPENED_CLOSE =      0x01,
        CRDITI_SHIELD_UP_DOWN =         0x02,
        CRDITI_FACE_ON_OFF =            0x03,
        CRDITI_CARD =                    0x04,
        CRDITI_CLOSE_DOOR =             0x05,
            
            // this value is for having the enum 4B    
        CRDITI_MAX_UINT =     0xFFFFFFFF
} CRDITIndex_e;

// bit flags for the virtual CRMC_ALARM_AREA_TMP_UNSET command
typedef enum
{
    CRATUF_ENTRY = 0x01,
    CRATUF_INDICATORS = 0x02,
    CRATUF_TEXT = 0x04,
    CRATUF_CARD_AVAIL = 0x08,
    CRATUF_NO_MASTER_CMD = 0x80
}
CRATUFlag_e;
