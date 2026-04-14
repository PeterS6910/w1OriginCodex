#pragma once
#include "CrProtocol.h"
#include "System\baseTypes.h"

#ifdef CR_DEBUG_VERBOSE 
    #define MAX_CRMC_TRACE_COUNT 5
#endif

typedef enum CRHWVersion
{
    CRHW_Unknown = 0x00,
    CRHW_ProximityFull = 0xA0, //0xA0 - BCR Proximity (Full version)
    CRHW_ProximityMini = 0xA1, //0xA1 - BCR Proximity (Mini version)
    //CRHW_ProximityFullUSB = 0xA2,//0xA2 - BCR Proximity (USB, Full version)
    //CRHW_ProximityMiniUSB = 0xA3, //0xA3 - BCR Proximity (USB, Mini version)

        CRHW_ASPMotorolaProximityFull = 0xB0, //0xB0 - BCR Proximity (ASP Motorola card support, Full version)
        CRHW_ASPMotorolaProximityMini = 0xB1, //0xB1 - BCR Proximity (ASP Motorola card support, Mini version)

        CRHW_SmartFull = 0xC0, //0xC0 - BCR Smart/Mifare (Full version)
        CRHW_SmartQTouchKeyboard = 0xC1, //0xC1 - BCR Smart/Mifare QTOUCH (Siedle)
        //CRHW_SmartFullUSB = 0xC2, //0xC2 - BCR Smart/Mifare (Mini version)
        //CRHW_SmartMiniUSB = 0xC3, //0xC3 - BCR Smart/Mifare (USB, Full version)

        CRHW_HIDProximityFull = 0xD0, //0xD0 - BCR HID Prox II (Full version)
        CRHW_HIDProximityMini = 0xD1, //0xD1 - BCR HID Prox II (Mini version)
        //CRHW_HIDProximityMiniUSB = 0xD2, //0xD2 - BCR HID Prox II (USB, Mini version)

        CRHW_ComboSmartProximityFull = 0xE2, //0xE2 - BCR Combo Smart/Mifare & Proximity (Full version)

        CRHW_ProximityMiniCCR = 0x24, //0x24 - CCR Proximity Slim
        CRHW_ProximityLiteCCR = 0x25, //0x25 - CCR Proximity Lite
        CRHW_Proximity8LineLcd = 0x28, //0x28 - CACR Proximity (Full version)
        CRHW_ProximityPremiumCCR = 0x29, //0x29 - CCR Proximity Premium
        CRHW_SmartMiniCCR = 0x44,  //0x44 - CCR Smart/Mifare Slim
        CRHW_SmartLiteCCR = 0x45, //0x45 - CCR Smart/Mifare Lite
        CRHW_SmartQTouchCCR = 0x46, //0x46 - CCR Smart/Mifare QTOUCH with wiegand interface
        CRHW_Smart8LineLCD = 0x48, //0x48 - CACR Smart/Mifare (Full version)       
        CRHW_SmartPremiumCCR = 0x49, //0x49 - CCR Smart/Mifare Premium
        CRHW_HIDProximity8LineLCDMini = 0x53, //0x53 - CACR HID prox II (Mini version)
        CRHW_HIDProximity8LineLCDFull = 0x58, //0x58 - CACR HID prox II (Full version)
        CRHW_BCROemWiegandInterface = 0x89, //0x89 - BCR OEM interface (Wiegand card interface)
        
    CRHW_MAX_UINT = 0xFFFFFFFF

} CRHWVersion_e;

//#define CR_DEBUG_VERBOSE

#ifdef CR_DEBUG_VERBOSE
typedef enum {
    CRTR_REQUEST = 1,
    CRTR_RESPONSE = 2,
    CRTR_TIMEOUT= 3,
    CRTR_RESPONSE_TOO_LATE = 4,
    CRTR_RESPONSE_TOO_LATE2 = 5,
    CRTR_RESPONSE_SCRAMBLED = 6
}
CRTraceRecordType_e;

typedef struct {
    uint32 address:2;
    uint32 requestAddress:2;
    uint32 requestAddress2:2;
    
    CRMessageCode_t messageCode;
    //uint32 baudRate;
    CRTraceRecordType_e type;  
    CRMessageCode_t requestMessageCode;
    uint32 time;
} CRCommTrace_t;
#endif


typedef struct {
    CRState_e state;
       
    uint32 address;
    
    bool timeDisabled;
    
    bool isOnline;
    
#ifdef DEBUG
    bool isEnabled;
#endif
    
    bool inSabotage;
    bool inUpgrade;
    int32 lastUpgradeDPN;
    
    //bool speedNegotiated;
    
    uint32 lastRequestRetryCount;
        
    //uint32 currentBaudRate;
    //uint32 requestedBaudRate;

    uint32 protocolVersionHigh;
    uint32 protocolVersionLow;
    uint32 firmwareVersionHigh;
    uint32 firmwareVersionLow;

    CRHWVersion_e hardwareVersion;
    uint32 language;
    
    // thould be at offset dividable by 4
    CRMessage_t* lastRequest;
    
#ifdef CR_DEBUG_VERBOSE    
    CRCommTrace_t messageTrace[MAX_CRMC_TRACE_COUNT];
    uint8 messageTraceCount;
#endif
   
    //bool inSabotage;
    byte upperSystemPresentationMask[CR_SET_PRESENTATION_MASK_ODL];
    
} CardReader_t;