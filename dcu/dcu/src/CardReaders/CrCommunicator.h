#pragma once
#include "System\baseTypes.h"

#include "CardReader.h"

#define TCR_DEFAULT_COMMAND_TIMEOUT         60      // ms
#define TCR_GET_PROTOCOL_VERSION_TIMEOUT    60      // ms - let it have higher for TCR
#define TCR_ACK_GET_EVENT_TIMEOUT           60      // ms

#define CCR_DEFAULT_COMMAND_TIMEOUT         40      // ms
#define CCR_GET_PROTOCOL_VERSION_TIMEOUT    40      // ms
#define CCR_ACK_GET_EVENT_TIMEOUT           40      // ms

#define CR_PRESENTATION_MASK_TIMEOUT        400     //ms
#define CR_MENU_COMMANDS_TIMEOUT            200     //ms
#define CR_DISPLAY_TEXT_TIMEOUT             100     //ms
#define CR_SMART_COMMANDS_TIMEOUT           150     //ms
#define CR_FLASHING_COMMANDS_TIMEOUT           300     //ms

#define CCR_DELAY_BETWEEN_SUCCESSFUL_POLLING          15       // ms 
#define TCR_DELAY_BETWEEN_SUCCESSFUL_POLLING          40       // ms 

//#define MINIMAL_POLLING_TIME_PER_TCR    30      // ms
//#define MINIMAL_POLLING_TIME_PER_CCR    10      // ms

#define MAX_RETRY_COUNT                 10


/********************************************************************************************
 * delegate definitions
 ********************************************************************************************/
typedef void(*DCROnlineStateChanged)(CardReader_t*,bool);
typedef void(*DCROptionalDataReceived)(CardReader_t* cr,byte* cardData,uint32 cardDataLength);
typedef void(*DCRGeneralEvent)(CardReader_t* cr);
typedef void(*DCRRawDataReceived)(CardReader_t* cr,uint32 messageCode,byte* optionalData,uint32 optionalDataLength);
typedef void(*DCRKeyPressed)(CardReader_t* cr,uint32 keyCode);

//extern volatile DCROptionalDataReceived CR_CodeSpecified;
//extern volatile DCRGeneralEvent CR_CodeTimeout;

/**
 * @returns CR according to it's address or NULL if irelevant
 */
CardReader_t* CrCommunicator_GetCR(uint32 crAddress);

void CrCommunicator_Init();

void CrCommunicator_FireOnlineEvents();

bool CrCommunicator_ConceptAndEnqueue(uint32 address,CRMessageCode_t messageCode,uint32 optionalDataLength,byte* optionalData,bool onTop,bool shiftIfFull);

#ifdef DEBUG
bool CR_SetEnabled(uint32 crAddress, bool enable);
#endif

bool CR_DisplayText(CardReader_t* cr,uint32 left,uint32 top,char* text,uint32 textLength);
bool CR_ClearDisplay(CardReader_t* cr);
bool CR_SetTime(CardReader_t* cr);

bool CR_SetPresentationMask(CardReader_t* cr);
bool CR_SetCountryCode(CardReader_t* cr);

bool CR_RawAccessCommand(uint32 crAddress, CRMessageCode_t accessCommand,int32 optionalParameter);
bool CR_AccessCommand(CardReader_t* cr, CRMessageCode_t accessCommand,int32 optionalParameter);

//bool CR_AccessCommandParametric(CardReader_t* cr,CRMessageCode_t accessCommand,uint32 optionalParameter);
//bool CR_RawAccessCommandParametric(uint32 crAddress,CRMessageCode_t accessCommand, uint32 optionalParameter);

bool CR_RawPeripheralMode(uint32 crAddress, CRIndicatorMode_e redLedIM,CRIndicatorMode_e greenLedIM,CRIndicatorMode_e buzzerIM,CRIndicatorMode_e externalBuzzerIM);
bool CR_PeripheralMode(CardReader_t* cr, CRIndicatorMode_e redLedIM,CRIndicatorMode_e greenLedIM,CRIndicatorMode_e buzzerIM,CRIndicatorMode_e externalBuzzerIM);

#ifndef XPRESSO
bool CR_RawDisplayInfoTab(uint32 crAddress,
    uint32 icon1, bool enabled1,
    uint32 icon2, bool enabled2,
    uint32 icon3, bool enabled3,
    uint32 icon4, bool enabled4
    );

bool CR_DisplayInfoTab(CardReader_t* cr,
    uint32 icon1, bool enabled1,
    uint32 icon2, bool enabled2,
    uint32 icon3, bool enabled3,
    uint32 icon4, bool enabled4
    );
#else
bool CR_RawDisplayInfoTab(uint32 crAddress, 
	CRDITIndex_e icon1, bool enabled1,
	CRDITIndex_e icon2, bool enabled2,
	CRDITIndex_e icon3, bool enabled3,
	CRDITIndex_e icon4, bool enabled4
    );

bool CR_DisplayInfoTab(CardReader_t* cr, 
	CRDITIndex_e icon1, bool enabled1,
	CRDITIndex_e icon2, bool enabled2,
	CRDITIndex_e icon3, bool enabled3,
	CRDITIndex_e icon4, bool enabled4
    );
#endif

#ifdef TEST_MODE_VIA_CR
bool CR_SingleKeyMode(CardReader_t* cr,bool enable);
#endif

bool CR_HasDisplay(CardReader_t* cr);

/**
 * Events binding
 */
DECLARE_BIND_EVENT( CrCommunicator_BindOnlineStateChanged, DCROnlineStateChanged )
DECLARE_BIND_EVENT( CrCommunicator_BindKeyPressed, DCRKeyPressed )
DECLARE_BIND_EVENT( CrCommunicator_BindRawDataReceived, DCRRawDataReceived )
DECLARE_BIND_EVENT( CrCommunicator_BindTamperReturn, DCRGeneralEvent )
DECLARE_BIND_EVENT( CrCommunicator_BindCountryCodeConfirmed, DCRGeneralEvent )
//DECLARE_BIND_EVENT( CrCommunicator_BindCardSwiped, DCROptionalDataReceived )

