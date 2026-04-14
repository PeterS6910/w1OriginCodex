#ifndef BOOTLOADER

//#define CR_DEBUG_VERBOSE // ONLY FOR TRACING OF THE MESSAGES SENT AND RECEIVED 

#include "CrCommunicator.h"
#include <stdint.h>
#include "System\UART\uart.h"
#include "System\baseTypes.h"
#include "System\Timer\timer16.h"
#include "System\Timer\timer32.h"
#include "System\Timer\sysTick.h"
#include "CrProtocol.h"
#include "CardReader.h"
#include "CrQueue.h"
#include "System\baseTypes.h"
#include "System\LPC122x.h"
#include "System\tasks.h"
#include "Crypto\hashes.h"
#include "System\GPIO\gpio.h"

#include "System\dmHandler.h"

#define INVALID_TIMESTAMP   0xFFFFFFFF

enum {
    CRRP_WAIT_FOR_PREAMBLE,
    CRRP_WAIT_FOR_HEADER,
    CRRP_WAIT_FOR_DATA,
    
    CRRP_MAX_UINT = 0xFFFFFFFF
} _readingPhase ;


#define CR_CCA_CHAR_TIME_4800               16667 // in usec 1/(4800/8)
#define CR_CCA_CHAR_TIME_9600               8333 // in usec 1/(9600/8) 
#define CR_CCA_CHAR_TIME_19200              417 // in usec 1/(19200/8) 
#define CR_CCA_CHAR_TIME_38400              208 // in usec 1/(38400/8) 
#define CR_CCA_CHAR_TIME_57600              139// in usec 1/(57600/8) 
#define CR_CCA_CHAR_TIME_115200             70 // in usec 1/(115200/8) 

#define CR_COMM_INITIALIZED             0x01
#define CR_COMM_OLD_BAUDRATE            0x02

#define CR_MESSAGE_RETRIED_BY_TIMEOUT    0x80
#define CR_MESSAGE_RETRIED_BY_NACK       0x40


volatile uint32 _crCommFlags = 0;

volatile CardReader_t _cardReaders[CR_MAX_COUNT];

// preallocate Optional data here instead of dynamically
//volatile byte _crResponseMessageOptionalData[CR_MAX_OPTIONAL_DATA_LENGTH];
volatile CRReceivedMessage_t _crResponseMessage;

volatile uint32 _receivedCheckSumHigh;
volatile uint32 _receivedCheckSumLow;
//volatile uint32 _calculatedCheckSum;

volatile uint32 _remainingHeaderBytes = 0;
volatile uint32 _remainingDataBytes = 0;

volatile uint32 _lastRequestTimeout = 0;
//volatile bool _lastRequestInProgress = false;
volatile uint32 _lastRequestTimestamp = 0; 
volatile uint32 _lastCRAddress = 0;

//#define ROTATE_OVER_BAUDRATES

/**
 * DELEGATES
 */
void CrCommunicator_OnResponseRecieved();

DEFINE_EVENT( DCROnlineStateChanged, _eventOnlineStateChanged)
DEFINE_EVENT( DCRKeyPressed, _eventKeyPressed)
DEFINE_EVENT( DCRRawDataReceived, _eventRawDataReceived)
DEFINE_EVENT( DCRGeneralEvent, _eventTamperReturn )
DEFINE_EVENT( DCRGeneralEvent, _eventCountryCodeConfirmed )
//DEFINE_EVENT( DCROptionalDataReceived, _eventCardSwiped)

//volatile DCROptionalDataReceived CR_CodeSpecified;
//volatile DCRGeneralEvent CR_CodeTimeout;


DEFINE_BIND_EVENT( CrCommunicator_BindOnlineStateChanged, DCROnlineStateChanged , _eventOnlineStateChanged)
DEFINE_BIND_EVENT( CrCommunicator_BindRawDataReceived, DCRRawDataReceived, _eventRawDataReceived)
DEFINE_BIND_EVENT( CrCommunicator_BindKeyPressed, DCRKeyPressed, _eventKeyPressed)
DEFINE_BIND_EVENT( CrCommunicator_BindTamperReturn, DCRGeneralEvent, _eventTamperReturn)
DEFINE_BIND_EVENT( CrCommunicator_BindCountryCodeConfirmed, DCRGeneralEvent, _eventCountryCodeConfirmed)
//DEFINE_BIND_EVENT( CrCommunicator_BindCardSwiped, DCROptionalDataReceived , _eventCardSwiped )

#define CR_WAITING_FOR_RESPONSE_AFTER_TRANSMISSION              -255
#define CR_TRANSMISSION_AFTER_TIMEOUT                           -1

// contains either special states as negative numbers, like CR_WAITING_FOR_RESPONSE_AFTER_TRANSMISSION
// or time of last response as positive number (SysTick_GetTickCount())
volatile int32 _crPollingState = CR_TRANSMISSION_AFTER_TIMEOUT; // implicit value must be like this, so the TransmitTask will emit the first GET_PROTOCOL_VERSION commands at the beggining


/***************************************************************************
 *
 ***************************************************************************/
CardReader_t* CrCommunicator_GetCR(uint32 crAddress) {
    if (crAddress < 1 || crAddress > CR_MAX_COUNT) {
        //NOP();
        return null;
    }
    else
        return (CardReader_t*)&(_cardReaders[crAddress-1]);
}

/***************************************************************************
 *
 ***************************************************************************/
#ifdef DEBUG

bool CR_SetEnabled(uint32 crAddress, bool enable) {    
    CardReader_t* cr = CrCommunicator_GetCR(crAddress);
    if (cr == null)
        return false;
    
    cr->isEnabled = enable;
    
    return true;
}
#endif



/***************************************************************************
 *
 ***************************************************************************/
bool CR_ChangeOnlineState(CardReader_t* cr, bool isOnline) 
{
    if (null == cr)
        return false;
    
    if (cr->isOnline == isOnline) {
#ifdef DEBUG
    	NOP();
#endif
        return false;
    }
    else {
        cr->isOnline = isOnline;
       
        if (null != _eventOnlineStateChanged) {
            BEGIN_CALL_EVENT(_eventOnlineStateChanged)
                cr,isOnline
            END_CALL_EVENT
        }
        
        //if (isOnline)
        //    CR_SetTime(cr);
        
        return true;
    }
}

/***************************************************************************
 *
 ***************************************************************************/
void CR_SetInitialValues(CardReader_t* cr, bool isFirst) 
{
    if (cr == null)
        return;
				
    
    cr->lastRequestRetryCount = 0;
    cr->protocolVersionHigh = 0;
    cr->protocolVersionLow = 0;
    cr->firmwareVersionHigh = 0;
    cr->firmwareVersionLow = 0;
    cr->hardwareVersion = CRHW_Unknown;
    cr->inSabotage = false;
    cr->inUpgrade = false;
    cr->lastUpgradeDPN = -1;
    //cr->speedNegotiated = false;
    cr->timeDisabled = false;
    
    /*if (_crCommFlags & CR_COMM_OLD_BAUDRATE)   
    {
        cr->currentBaudRate = FIRST_COMMON_CR_BAUDRATE;
    }
    else
        cr->currentBaudRate = FIXED_COMMON_CR_BAUDRATE;
    
    cr->requestedBaudRate = 0;*/
	
#ifdef CR_DEBUG_VERBOSE
    for(int j=0;j<MAX_CRMC_TRACE_COUNT;j++) {
        cr->messageTrace[j].messageCode = CRMC_UNKNOWN;       
        //cr->messageTrace[j].baudRate = 0;
        cr->messageTrace[j].time = 0;
        cr->messageTrace[j].requestMessageCode = CRMC_UNKNOWN;
        cr->messageTrace[j].requestAddress =0;
        cr->messageTrace[j].address =0;
    }
    cr->messageTraceCount = 0;
#endif
 
    if (!isFirst) {
        CR_ChangeOnlineState(cr,false);
        
        if (null != cr->lastRequest) 
        {
            Delete(cr->lastRequest);
            cr->lastRequest = null;
        }
                
        CrQueue_Clear(cr->address);
    }
}

/***************************************************************************
 *
 ***************************************************************************/
void CR_SetState(CardReader_t* cr,CRState_e state) {
    if (null == cr)
        return;
    
    uint32 previousState;
    
    if (cr->state == state)
        return;
    else {
        previousState = cr->state;
        cr->state = state;
    }
    
    switch(state) {
        case CRS_NOT_RESPONDING:
            CR_SetInitialValues(cr,false);
            
            break;
        case CRS_MAIN_LOOP:
            CR_ChangeOnlineState(cr,true);
            break;
        case CRS_NOT_INITIALIZED:
            
            if (previousState != CRS_NOT_RESPONDING) {
                CR_SetInitialValues(cr,false);
            }
            
            break;
        case CRS_IN_SETTING_LOOP:
            break;
    }
}

/***************************************************************************
 *
 ***************************************************************************/
CRMessage_t* ConceptRequest(uint32 address,CRMessageCode_t messageCode,uint32 optionalDataLength,byte* optionalData) 
{
    // upper level calls should ensure this    
    //if (address <= 0 || address > 0x0F)
        //return null;
    
    if (optionalData == null) 
        optionalDataLength = 0;
    else {
        if (optionalDataLength > CR_MAX_OPTIONAL_DATA_LENGTH) 
        {
            // this failsafe handling caused more problem 
            // using 0 might be a better choice
            //optionalDataLength = CR_MAX_OPTIONAL_DATA_LENGTH;
            optionalDataLength = 0;
        }
    }
    
    CRMessage_t* request = CrQueue_AllocRequest(optionalDataLength);
    if (request == null)
        // possibly out of memory
        return null;
   
    request->crAddress = address;
    request->messageCode = messageCode;
    // request->optionalDataLength = optionalDataLength; // should be done by CrQueue_AllocRequest
    
    if (optionalDataLength > 0) {
        CopySafe(optionalData,optionalDataLength,request->optionalData,request->optionalDataLength,optionalDataLength);
    }
    
    return request;
}

#if DEBUG
uint32 _crFlashUploadRetries = 0;
#endif

/***************************************************************************
 *
 ***************************************************************************/
bool CrCommunicator_ConceptAndEnqueue(uint32 address,CRMessageCode_t messageCode,uint32 optionalDataLength,byte* optionalData,bool onTop,bool shiftIfFull) 
{
    if (messageCode == CRMC_UNKNOWN)
        return false;
    
    CardReader_t* cr = CrCommunicator_GetCR(address);
    if (cr == null) 
    {
#ifdef DEBUG
    	NOP();
#endif
        return false;
    }
    
    if (messageCode == CRMC_FLASH_UPLOAD) 
    {
        int32 dpn = *((uint16*)optionalData); // must be int for next comparation
        
        if (dpn <= cr->lastUpgradeDPN) 
        {
#ifdef DEBUG
        	NOP();
            _crFlashUploadRetries++;
            dpn += cr->lastUpgradeDPN;
#endif
            
            return true; // don't return false here, because it's a partly correct state
        }
        else
            cr->lastUpgradeDPN = dpn;
    }
    
    
    
    switch(messageCode) {
        case CRMC_STACKED_MESSAGE:
            if (optionalDataLength < 2 || optionalDataLength > CR_MAX_OPTIONAL_DATA_LENGTH)
                return false;
            else 
            {
                uint32 subMessageCount = optionalData[0];
                uint32 reachedPosition = subMessageCount+1; // skip the header about [count of submessages][submessageLength1]..[submessageLengthN]
                
                bool succeeded = true;
                
                // sequential conception of the messages 
                for(int i = 0;i<subMessageCount;i++) {
                    int32 subMessageLength = optionalData[i+1];
                    
                    if (subMessageLength <= 0) 
                    {
                        // this CAN happen according the specification on master side
                        //
                        // continue to ensure the rest of messages being parsed
                    	NOP();
                        continue;
                    }
                    
                    if (reachedPosition + subMessageLength <= optionalDataLength) 
                    {
                        bool enqueueSuccess = CrCommunicator_ConceptAndEnqueue(
                        
                        //CRMessage_t* request = ConceptRequest(
                            address,//address
                            (CRMessageCode_t)optionalData[reachedPosition],//messageCode
                            subMessageLength - 1, // -1 stands for encapsulated messageCode
                            &(optionalData[reachedPosition+1]), // +1 stands for encapsulated messageCode
                            onTop, shiftIfFull
                            );

                        if (!enqueueSuccess)
                            succeeded = false;
                        
                        reachedPosition += subMessageLength; // subMessageLength includes +1 for messageCode
                    }
                    else {
                        succeeded = false;
                        break;
                    }
                }
                
                return succeeded;
            }
            break;
        case CRMC_ALARM_AREA_TMP_UNSET:
        case CRMC_ALARM_AREA_RESERVED:
        case CRMC_ALARM_AREA_IN_ALARM:
            {
                int32 optAccessParam = -1;
                if (optionalDataLength > 0) 
                    optAccessParam = optionalData[0];
                
                if (!CR_AccessCommand(cr,messageCode,optAccessParam))
                    return false;
                else 
                    return true;
             
            }
     
            break;
        case CRMC_SET_PRESENTATION_MASK:
            {
                if (optionalDataLength < CR_SET_PRESENTATION_MASK_ODL)
                    return false;
                
                bool isChanged = false;
                for(int i=0;i<CR_SET_PRESENTATION_MASK_ODL;i++) 
                {
                    if (cr->upperSystemPresentationMask[i] != optionalData[i])
                    {
                        isChanged = true;
                        cr->upperSystemPresentationMask[i] = optionalData[i];
                    }
                }
                
                if (isChanged) {
                    // this way cannot be done, as it produces call of ConceptAndEnqueu again
                    return CR_SetPresentationMask(cr);
                }
                else
                    // true, because no change made to presentation mask, so it would not be applied on CR anyway
                    return true;
            }
            break;
        case CRMC_SET_COUNTRY_CODE:
            {
                if (optionalDataLength < CR_SET_COUNTRY_CODE_ODL) 
                    return false;
                
                bool isChanged = optionalData[1] != cr->language;
                
                if (isChanged) {
                    cr->language = optionalData[1];
                    return CR_SetCountryCode(cr);
                }
                else
                    // true, because no change made to country code, so it would not be applied on CR anyway
                    return true;
            }
            break;    
#if DEBUG
        case CRMC_PUT_ANNOUNCMENT:
            NOP();
#endif
        default:
            {
                CRMessage_t* request = ConceptRequest(address,messageCode,optionalDataLength,optionalData);   
                if (request == null)
                    return false;
               
                bool enqueueSuccess =  CrQueue_Enqueue(address,request,onTop,shiftIfFull);
       
                return enqueueSuccess;
            }
            break;
    }
    
}

/***************************************************************************
 *
 ***************************************************************************/
int32 CrCommunicator_PHDParsing(byte* buffer, uint32 size,uint32 timestamp) {
    //uint32 tc = SysTick_GetTickCount();
    
    unsigned char volatile* formerBuffer = buffer;
    
    bool continueProcessing;
    do 
    {
        continueProcessing = false;
        int32 sizeToSearch = size - (buffer - formerBuffer);
        
        switch(_readingPhase) {
            case CRRP_WAIT_FOR_PREAMBLE:
                {
                    int32 start = -1;
                    for(int i=0;i<sizeToSearch;i++) {
                        if (buffer[i] == 0xFF) {
                           // possibly a PREAMBLE
                           start = i;
                           // increments buffer pointer
                           _readingPhase = CRRP_WAIT_FOR_HEADER;
                           _remainingHeaderBytes = CR_HEADER_LENGTH;
                                    
                           if ((sizeToSearch-start-CR_PREAMBLE_LENGTH)>0) {
                                buffer = buffer+start+CR_PREAMBLE_LENGTH;
                                continueProcessing = true;
                           }
                           
                           break;
                           
                        }
                    }
                    
                    if (start == -1)
                        return 0;
                }
                break;
            case CRRP_WAIT_FOR_HEADER:
                switch(_remainingHeaderBytes) {
                    case 5:
                        if (buffer[0] >0 && buffer[0] <= 0x0F)
                        {            
                            _crResponseMessage.crAddress = buffer[0];
                            //_calculatedCheckSum = _crResponseMessage.crAddress;
                        }
                        else {
                            // invalid start of the header - possibly another preamble
                            _readingPhase = CRRP_WAIT_FOR_PREAMBLE;
                        }
                            
                        break;
                    case 4:   
                        _crResponseMessage.optionalDataLength = buffer[0];
                        //_calculatedCheckSum += _crResponseMessage.optionalDataLength;
                        break;
                    case 3:
                        _crResponseMessage.messageCode = (CRMessageCode_t)buffer[0];
                        //_calculatedCheckSum += _crResponseMessage.messageCode;
                        break;
                    case 2:
                        _receivedCheckSumHigh = buffer[0];
                        break;
                    case 1:
                        
                        _receivedCheckSumLow = buffer[0];
                        
                        if (_crResponseMessage.optionalDataLength > 0 && 
                            _crResponseMessage.optionalDataLength <= CR_MAX_OPTIONAL_DATA_LENGTH) {
                            _remainingDataBytes = _crResponseMessage.optionalDataLength;
                            _readingPhase = CRRP_WAIT_FOR_DATA;
                        }
                        else {
                            _readingPhase = CRRP_WAIT_FOR_PREAMBLE;
                            
                            //if (_crResponseMessage.optionalDataLength > CR_MAX_OPTIONAL_DATA_LENGTH)
                            //    _crResponseMessage.optionalDataLength = 0;
                            
                            bool isArithmetic = true;
                            CardReader_t* cr = CrCommunicator_GetCR(_crResponseMessage.crAddress);
                            if (cr != null && cr->protocolVersionHigh >= CR_FIRST_CCR_PROTO)
                                isArithmetic = false;
                            
                            bool isChecksumValid = false;
                            uint32 calculatedCheckSum = false;
                            if (isArithmetic) 
                            {
                                calculatedCheckSum = 0xFFFF & ((_crResponseMessage.crAddress + _crResponseMessage.optionalDataLength + _crResponseMessage.messageCode)*2);
                                
                                uint32 arithmeticReceivedChecksum = 0xFFFF & (_receivedCheckSumLow + (_receivedCheckSumHigh << 8));
                                
                               
                                if (calculatedCheckSum == arithmeticReceivedChecksum)
                                    isChecksumValid = true;                               
                                
                            }
                            else 
                            {
                                // this block also optimizes usage of the stack by tmpHeader variable
                                
                                byte tmpHeader[CR_HEADER_LENGTH-1] = { _crResponseMessage.crAddress, _crResponseMessage.optionalDataLength, _crResponseMessage.messageCode, _receivedCheckSumHigh };
                                
                                calculatedCheckSum =  Crc8(tmpHeader,0,CR_HEADER_LENGTH-1);
                                
                                if (calculatedCheckSum == _receivedCheckSumLow) 
                                    isChecksumValid = true;
                            }
                            
                            // HANDLE THE ACTUAL RESPONSE
                            CrCommunicator_OnResponseRecieved(isChecksumValid);
                        }
                        break;

                }
                
                _remainingHeaderBytes--;
                
                if (sizeToSearch > 1) {
                    buffer++;
                    continueProcessing = true;
                }
                
                break;
            case CRRP_WAIT_FOR_DATA:
                //_calculatedCheckSum += buffer[0];
                
                //if (_remainingDataBytes ==  _crResponseMessage.optionalDataLength) {
                       
                //}
                
                _crResponseMessage.optionalData[_crResponseMessage.optionalDataLength - _remainingDataBytes] = buffer[0];

                
                _remainingDataBytes--;
                
                if (_remainingDataBytes == 0) {
                   _readingPhase = CRRP_WAIT_FOR_PREAMBLE;
                   
                    bool isArithmetic = true;
                    uint32 calculatedCheckSum;
                    bool isChecksumValid = false;
                    CardReader_t* cr = CrCommunicator_GetCR(_crResponseMessage.crAddress);
                    if (cr != null && cr->protocolVersionHigh >= CR_FIRST_CCR_PROTO && _crResponseMessage.messageCode != CRMC_PROTOCOL_VERSION)
                        isArithmetic = false;

                    if (isArithmetic) {
                       calculatedCheckSum = _crResponseMessage.crAddress + _crResponseMessage.optionalDataLength + _crResponseMessage.messageCode;
                       for(int32 i=0;i<_crResponseMessage.optionalDataLength;i++)
                           calculatedCheckSum += _crResponseMessage.optionalData[i];
                       
                       calculatedCheckSum = 0xFFFF & (2 * calculatedCheckSum);
                       
                                 
                       uint32 arithmeticReceivedChecksum = 0xFFFF & (_receivedCheckSumLow + (_receivedCheckSumHigh << 8));
                      
                        if (calculatedCheckSum == arithmeticReceivedChecksum)
                            isChecksumValid = true;
                        
                    }
                    else {
                        byte header[] = { _crResponseMessage.crAddress, _crResponseMessage.optionalDataLength, _crResponseMessage.messageCode, _receivedCheckSumHigh };
                                
                        calculatedCheckSum =  Crc8(header,0,4);
                        
                        if (calculatedCheckSum == _receivedCheckSumLow) {
                            isChecksumValid = true;
                        }
                    }
                    
                    CrCommunicator_OnResponseRecieved(isChecksumValid);
                }

                if (sizeToSearch > 1) {                    
                    buffer++;
                    continueProcessing = true;
                }
                
                break;
        }
    }
    while(continueProcessing);
    
    return 0;
}
/***************************************************************************
 *
 ***************************************************************************/
void CrCommunicator_SendPollingMessage(uint32 address) {
    if (CrQueue_Count(address) == 0 
           /* &&
                        (_lastRequest == null ||
                         (_lastRequest != null && 
                          (_lastRequest->crAddress != (address) ||
                           (_lastRequest->crAddress == (address) && _lastRequest->crAddress != ACK_GET_EVENT ) ) ) )*/
        )
	CrCommunicator_ConceptAndEnqueue(address,CRMC_ACK_GET_EVENT,0,null,false,false);
    
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_SetTime(CardReader_t* cr) {
    if (null == cr ||
        cr->timeDisabled)
	return false;
    
    byte params[5];

    params[0] = 0x30; // positioning, not relevant to Premium
    params[1] = 0x02; // 24h format, AM
    params[2] = ToBcdByte(SysTick_GetHour());
    params[3] = ToBcdByte(SysTick_GetMinute());
    params[4] = ToBcdByte(SysTick_GetSecond());
    
    
    CRMessage_t* message = ConceptRequest(
        cr->address,
		CRMC_SET_TIME,
		5,
		params);
    

    if (null == message)
        return false;
    
    return CrQueue_Enqueue(cr->address,message,false,false);
}

/***************************************************************************
 * SHOULD NOT CALL CONCEPT AND ENQUEUE
 ***************************************************************************/
bool CR_SetPresentationMask(CardReader_t* cr) {
    if (null == cr)
        return false;
    
    CRMessage_t* request = 
        ConceptRequest(
            cr->address,
            CRMC_SET_PRESENTATION_MASK,
            CR_SET_PRESENTATION_MASK_ODL,
            cr->upperSystemPresentationMask);   
    
    if (request == null)
        return false;
                   
    return CrQueue_Enqueue(cr->address,request,false,false);
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_SetCountryCode(CardReader_t* cr) {
	byte params[CR_SET_COUNTRY_CODE_ODL];

	params[0] = 0;
	params[1] = cr->language;
    
    CRMessage_t* request = 
        ConceptRequest(cr->address,CRMC_SET_COUNTRY_CODE,CR_SET_COUNTRY_CODE_ODL,params);   
    if (request == null)
        return false;
                   
    return CrQueue_Enqueue(cr->address,request,false,false);
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_ValidateCommandSuitable(CardReader_t* cr) {
    if (null == cr)
        return false;
    else
        return (cr->state == CRS_MAIN_LOOP);
}

/***************************************************************************
 *
 ***************************************************************************/
/*
void CR_WaitForCard(CardReader_t* cr) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return;
    
	byte params[1];

	params[0] = 0x3;

	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_WAITING_FOR_CARD,
		1,
		params,
        false,false);
}*/

/***************************************************************************
 *
 ***************************************************************************/
bool CR_AccessCommand(CardReader_t* cr,CRMessageCode_t accessCommand,int32 optionalParameter) {
    if (cr==null || !cr->isOnline)
        return false;
    
    byte rawParams[] = { 0, 0 };
    
    bool isAccessCommand = ((accessCommand & 0x40) == 0x40) || ((accessCommand & 0x50) == 0x50);
    
    // marks the types of messages that are actually consisting of several submessages
    // not yet supported on CR, but will be full part of CR protocol in future
    bool virtualMessage = false;
    CRMessageCode_t originalAccessCommand = accessCommand;
    
    bool applyMasterMessage = true;
    
    if (accessCommand == CRMC_ALARM_AREA_TMP_UNSET ||
        // this condition for temporal backward compatibility
        accessCommand == CRMC_ALARM_AREA_RESERVED ||
        accessCommand == CRMC_ALARM_AREA_IN_ALARM)
    {
        virtualMessage = true;
        accessCommand = CRMC_ALARM_AREA_IS_SET;
        
        if (optionalParameter > 0 &&
            ((optionalParameter & CRATUF_NO_MASTER_CMD) == CRATUF_NO_MASTER_CMD))
            applyMasterMessage = false;
    }

    
    uint32 paramCount;
    if (optionalParameter >= 0) {
        if (isAccessCommand) 
        {
            // 1(textPart)+2(symbolPart)
            rawParams[0] = 3; // all visible parts of access code
            
            if (virtualMessage) {
                paramCount = 1;
            }
            else {
                rawParams[1] = optionalParameter;
                paramCount = 2; 
            }
        }
        else 
        {
            rawParams[0] = optionalParameter;
            paramCount = 1;
        }
    }
    else {
        if (isAccessCommand) 
        {
            // 1(textPart)+2(symbolPart)
            rawParams[0] = 3;
            paramCount = 1;
        }
        else 
        {
            paramCount = 0;
        }
    }
    
    CRMessage_t* message = null;
    if (applyMasterMessage) 
    {
        message = ConceptRequest(
            cr->address,
            accessCommand,
            paramCount,
            rawParams
        );
        
        if (message == null)
            return false;
        
        // deallocates the message even if problem
        bool enqueueResult = CrQueue_Enqueue(cr->address,message,false,false);
        if (!virtualMessage || !enqueueResult)
            return enqueueResult;
    }
     
    // additional submessages
    switch(originalAccessCommand) {
        case CRMC_ALARM_AREA_IS_SET:
            // this is a temporary solution until default/empty DISPLAY_INFO_TAB after every 0x4* and 0x5*
            // command is not supported on CCR
            CR_DisplayInfoTab(cr, 
                CRDITI_NO_CHANGE, 0, 
                CRDITI_EMPTY, 0, 
                CRDITI_EMPTY, 0,
                CRDITI_NO_CHANGE, 0);
            return true;
        case CRMC_ALARM_AREA_TMP_UNSET:
        case CRMC_ALARM_AREA_RESERVED:
            {
                bool isEntry = ((optionalParameter & CRATUF_ENTRY) == CRATUF_ENTRY);
            
                if (optionalParameter > 0 &&
                    (optionalParameter & CRATUF_INDICATORS) == CRATUF_INDICATORS) 
                {
                    if (isEntry) {
                        if (!CR_PeripheralMode(cr,CRIM_LOW_FREQUENCY,CRIM_INTACT,CRIM_LOW_FREQUENCY,CRIM_INTACT))
                            return false;
                    }
                    else
                    {
                        if (!CR_PeripheralMode(cr,CRIM_LOW_FREQUENCY,CRIM_INTACT,CRIM_LOW_FREQUENCY,CRIM_INTACT))
                            return false;
                    }
                }
                
                if (optionalParameter > 0 &&
                    (optionalParameter & CRATUF_TEXT) == CRATUF_TEXT) 
                {
                    if (isEntry) {
                        if (!CR_DisplayText(cr,0,14,"entry",5))
                            return false;
                    }
                    else
                    {
                        if (!CR_DisplayText(cr,0,14,"exit",4))
                            return false;
                    }
                }
                
                if (optionalParameter > 0 &&
                    (optionalParameter & CRATUF_NO_MASTER_CMD) == CRATUF_NO_MASTER_CMD) 
                {
                    if ((optionalParameter & CRATUF_CARD_AVAIL) == CRATUF_CARD_AVAIL)
                        CR_DisplayInfoTab(cr, 
                            CRDITI_NO_CHANGE, 0, 
                            CRDITI_CARD, 1, 
                            CRDITI_SHIELD_UP_DOWN, isEntry ? 1 : 0,
                            CRDITI_NO_CHANGE, 0);

                    else
                        CR_DisplayInfoTab(cr, 
                            CRDITI_NO_CHANGE, 0, 
                            CRDITI_EMPTY, 0, 
                            CRDITI_SHIELD_UP_DOWN, isEntry ? 1 : 0,
                            CRDITI_NO_CHANGE, 0);
                    
                    
                }
                else {
                    CR_DisplayInfoTab(cr, 
                        CRDITI_NO_CHANGE, 0, 
                        CRDITI_CARD, 1,
                        CRDITI_EMPTY, 0,
                        CRDITI_NO_CHANGE, 0);
                }
                
                return true;    
            }
            break;
        case CRMC_ALARM_AREA_IN_ALARM:
            if (!CR_PeripheralMode(cr,CRIM_HIGH_FREQUENCY,CRIM_INTACT,CRIM_HIGH_FREQUENCY,CRIM_INTACT))
                return false;
            
            if (optionalParameter > 0 &&
                    (optionalParameter & CRATUF_NO_MASTER_CMD) == CRATUF_NO_MASTER_CMD) 
                {
                    CR_DisplayInfoTab(cr, 
                        CRDITI_NO_CHANGE, 0, 
                        CRDITI_NO_CHANGE, 0, 
                        CRDITI_SHIELD_UP_DOWN, 1,
                        CRDITI_NO_CHANGE, 0);
                    
                    
                }
            
            return true;
            
            //break;
        default:
            return false;
            
    }

    

}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_RawAccessCommand(uint32 crAddress, CRMessageCode_t accessCommand,int32 optionalParameter) {
    CardReader_t* cr = CrCommunicator_GetCR(crAddress);
    if (cr == null)
        return false;
    else
        return CR_AccessCommand(cr, accessCommand, optionalParameter);
}

/***************************************************************************
 *
 ***************************************************************************/
/*bool CR_AccessCommandParametric(CardReader_t* cr,CRMessageCode_t accessCommand,uint32 optionalParameter) {
    if (cr==null || !cr->isOnline)
        return false;
    
    uint32 paramCount = 2;
    if ((accessCommand & 0x40 != 0x40) &&
        (accessCommand & 0x50 != 0x50))
        paramCount = 1;
   
    CRMessage_t* message = null;
    if (paramCount == 2) 
    {
        byte params[] = { 3, 0 };
        params[1] = optionalParameter;

    
        message = ConceptRequest(
            cr->address,
		    accessCommand,
    		2,
	    	params
        );
    }
    
    if (paramCount == 1) {
        byte param0 = optionalParameter;
        
        message = ConceptRequest(
            cr->address,
		    accessCommand,
    		1,
	    	&param0
        );
    }
    
    if (message == null)
        return false;
    
    return CrQueue_Enqueue(cr->address,message,false,false);
}*/

/***************************************************************************
 *
 ***************************************************************************/
/*bool CR_RawAccessCommandParametric(uint32 crAddress,CRMessageCode_t accessCommand, uint32 optionalParameter) {
    CardReader_t* cr = CrCommunicator_GetCR(crAddress);
    if (cr == null)
        return false;
    else
        return CR_AccessCommandParametric(cr,accessCommand,optionalParameter);
}*/

/***************************************************************************
 *
 ***************************************************************************/
bool CR_DisplayInfoTab(CardReader_t* cr, 
    CRDITIndex_e icon1, bool enabled1, 
    CRDITIndex_e icon2, bool enabled2, 
    CRDITIndex_e icon3, bool enabled3, 
    CRDITIndex_e icon4, bool enabled4
    ) 
{
    // validates also if cr is null
    if (!CR_ValidateCommandSuitable(cr))
        return false;
    
    if (!CR_HasDisplay(cr))
        return true; // fake true, as the non-display CRs would not even recognize it
    
    if (cr->protocolVersionHigh < CR_FIRST_CCR_PROTO)
        return true; // fake true, as the BCRs would not even recognize it
    
    byte params[8];

    params[0] = icon1;
    params[1] = enabled1;
    params[2] = icon2;
    params[3] = enabled2;
    params[4] = icon3;
    params[5] = enabled3;
    params[6] = icon4;
    params[7] = enabled4;

    
    return 
        CrCommunicator_ConceptAndEnqueue(
            cr->address,
            CRMC_DISPLAY_INFO_TAB,
            8,
            params,
            false,false);
    
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_RawDisplayInfoTab(uint32 crAddress, 
    CRDITIndex_e icon1, bool enabled1, 
    CRDITIndex_e icon2, bool enabled2, 
    CRDITIndex_e icon3, bool enabled3, 
    CRDITIndex_e icon4, bool enabled4
    ) 
{
    CardReader_t* cr = CrCommunicator_GetCR(crAddress);
    
    if (cr == null)
        return false;
    else
        return CR_DisplayInfoTab(cr,icon1,enabled1,icon2,enabled2,icon3,enabled3,icon4,enabled4);
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_PeripheralMode(CardReader_t* cr, CRIndicatorMode_e redLedIM,CRIndicatorMode_e greenLedIM,CRIndicatorMode_e buzzerIM,CRIndicatorMode_e externalBuzzerIM) 
{
    // validates also if cr is null
    if (!CR_ValidateCommandSuitable(cr))
        return false;
    
    byte params[2];

    params[0] = (redLedIM & 0x0F) << 4;
    params[0] |= (greenLedIM & 0x0F);
    
    params[1] = (externalBuzzerIM & 0x0F) << 4;
    params[1] |= (buzzerIM & 0x0F);

    return 
        CrCommunicator_ConceptAndEnqueue(
            cr->address,
            CRMC_PUT_ANNOUNCMENT,
            2,
            params,
            false,false);
    
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_RawPeripheralMode(uint32 crAddress, CRIndicatorMode_e redLedIM,CRIndicatorMode_e greenLedIM,CRIndicatorMode_e buzzerIM,CRIndicatorMode_e externalBuzzerIM) 
{
    CardReader_t* cr = CrCommunicator_GetCR(crAddress);
    
    if (cr == null)
        return false;
    else
        return CR_PeripheralMode(cr,redLedIM,greenLedIM, buzzerIM,externalBuzzerIM);
}


/***************************************************************************
 *
 ***************************************************************************/
#ifdef TEST_MODE_VIA_CR
bool CR_SingleKeyMode(CardReader_t* cr,bool enable) {
    if (!CR_ValidateCommandSuitable(cr))
        return false;
    
    return CrCommunicator_ConceptAndEnqueue(
        cr->address,
        enable ? CRMC_SINGLE_KEY_MODE_ENABLE : CRMC_SINGLE_KEY_MODE_DISABLE,
        0,
        null,
        false,false);
 
}
#endif


/***************************************************************************
 *
 ***************************************************************************/
bool CR_ClearDisplay(CardReader_t* cr) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return false;
    
    byte params[] = { 0xFF };

    return CrCommunicator_ConceptAndEnqueue(
            cr->address,
            CRMC_CLEAR_DISPLAY,
            1,
            params,
    false,false);
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_DisplayText(CardReader_t* cr,uint32 left,uint32 top,char* text,uint32 textLength) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return false;
    
    if (null == text ||
        0 == textLength)
        return false;
    
    if (textLength > 28)
        textLength = 28;
    
  	byte params[30]; // max text length + 2params 
        
    params[0] = ((left & 0x0F) << 4) | (top & 0x0F);
    params[1] = 1;
    
    for(int i=0;i<textLength;i++)
        params[2+i] = text[i];
    
    return CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_DISPLAY_TEXT,
		textLength + 2,
		params,
        false,false);
}

/***************************************************************************
 *
 ***************************************************************************/
/*
bool CRStringDecoding(byte* input,uint32 inputSize,uint32 index,uint32 length,byte* outData,uint32* outDataLength)
{
    if (null == input ||
        null == outData ||
        outDataLength == null ||
        *outDataLength <= 0) {
        return false;
    }

    if (index >= inputSize) {
        *outDataLength = 0;    
        return false;
    }

    if (index + length > inputSize)
        length = inputSize - index + 1;

    int pinKeyCode = 0;
    int count = 0;
    for (int i = index; i < index+length; i++)
    {
        pinKeyCode = (input[i] & 0xF0) >> 4;
        if (pinKeyCode >= 0 && pinKeyCode <= 9)
        {
            outData[count] = pinKeyCode;
        }
        else
        {
            break;
        }
        
        count++;
        if (count >= *outDataLength)
            break;

        pinKeyCode = input[i] & 0x0F;
        if (pinKeyCode >= 0 && pinKeyCode <= 9)
        {
            outData[count] = pinKeyCode;
        }
        else
        {
            break;
        }
        
        count++;
        if (count >= *outDataLength)
            break;
        
    }
    
    *outDataLength = count;

    return true;
}*/

/***************************************************************************
 * @return true, if the SET_COMMUNICATION_LINE_MODE was enqueued
 *         false, if there was no change in speed necessary or available
 ***************************************************************************/
bool CrCommunicator_SendSetCommLineMode(CardReader_t* cr) {
    if (null == cr)
        return false;
    
    /*
    if (!cr->speedNegotiated) {
        uint32 maxBaudrate;
        if (cr->protocolVersionHigh < CR_FIRST_CCR_PROTO)
            maxBaudrate = FIRST_COMMON_CR_BAUDRATE;
        else
            maxBaudrate = THIRD_COMMON_CR_BAUDRATE;
            
        // POSSIBLY THE NEAR END OF IN_SETTING_LOOP STATE
        if (cr->currentBaudRate != maxBaudrate) {
            byte baudrateParams[2];
            
            baudrateParams[0] = CrProtocol_GetBaudRateEnum(maxBaudrate);
            
            baudrateParams[1] = 2; // NORMAL MODE - NOT SLOW
            
            cr->requestedBaudRate = maxBaudrate;
            
            CrCommunicator_ConceptAndEnqueue(cr->address,CRMC_SET_COMMUNICATION_LINE_MODE,2,baudrateParams,false,false);
            
            return true;
        }
        else {
            cr->speedNegotiated = true;       
            
            return false;
        }
    }
    else */
    
        return false;
}


/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeProtocolVersion(CardReader_t* cr) {
    
    uint32 protocolVersionHigh =  _crResponseMessage.optionalData[0];
    if ((protocolVersionHigh & 0x80) > 0) 
    {
        cr->inUpgrade = true;
        protocolVersionHigh &= 0x7F;
    }
    
    cr->protocolVersionHigh = FromBcdByte(protocolVersionHigh);
    cr->protocolVersionLow = FromBcdByte(_crResponseMessage.optionalData[1]);
    
    if (_crResponseMessage.optionalDataLength > 2) {
        cr->firmwareVersionHigh = FromBcdByte(_crResponseMessage.optionalData[2]);
        
        if (_crResponseMessage.optionalDataLength > 3) { 
            cr->firmwareVersionLow = FromBcdByte(_crResponseMessage.optionalData[3]);
            
            if (_crResponseMessage.optionalDataLength > 4)
                cr->hardwareVersion = (CRHWVersion_e)_crResponseMessage.optionalData[4];
        }
            
    }
    
    
    switch(cr->state) { 
        case CRS_NOT_RESPONDING:
                cr->inSabotage = false;

                CR_SetState(cr,CRS_NOT_INITIALIZED);
                
                //if (cr->protocolVersionHigh < CR_FIRST_CCR_PROTO)
                    CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
                //else
                //    if (!CrCommunicator_SendSetCommLineMode(cr))
                //        CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);

                break;
        case CRS_NOT_INITIALIZED:
                CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
                break;
    }   
}




/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeCrIdentity(CardReader_t* cr)
{
    cr->firmwareVersionHigh = FromBcdByte(_crResponseMessage.optionalData[0]);
    cr->firmwareVersionLow = FromBcdByte(_crResponseMessage.optionalData[1]);
    cr->hardwareVersion = (CRHWVersion_e)_crResponseMessage.optionalData[2];

    // necessary for getting the RSP_ACK
    //if (_crCommFlags & CR_COMM_OLD_BAUDRATE) 
    //{
    //    if (!CrCommunicator_SendSetCommLineMode(cr))
    //        CrCommunicator_SendPollingMessage(cr->address);
    //}
    //else {
        // FIXED SPEED
    CrCommunicator_SendPollingMessage(cr->address);
    //}
}

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeReset(CardReader_t* cr) {
    if (cr->state != CRS_NOT_INITIALIZED)
    	CR_SetState(cr,CRS_NOT_INITIALIZED);

    CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
}

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeCrNotInitialized(CardReader_t* cr) {
    if (null == cr->lastRequest) {
        //NOP();
        return;
    }
    
    bool sendSetCrInitialized = false;
     bool switchToInSettingLoop = true;

    
    switch (cr->lastRequest->messageCode)
    {
        case CRMC_SET_TIME:
            //sendSetCrInitialized = true;
            if (cr->state != CRS_MAIN_LOOP)
                CR_SetCountryCode(cr);
            else
            {
                // NO need to do the MAIN_LOOP->IN_SETTING_LOOP state switch
                sendSetCrInitialized = true;
                switchToInSettingLoop = false;
            }
            
            break;
        case CRMC_SET_COUNTRY_CODE:
            if (cr->state != CRS_MAIN_LOOP)
                CR_SetPresentationMask(cr);
            else 
            {
                // CRMC_SET_CR_INITIALIZED must precede the event !!!
                CrCommunicator_ConceptAndEnqueue(_crResponseMessage.crAddress,CRMC_SET_CR_INITIALIZED,0,null,false,false);
                    
                SAFE_CALL_EVENT(_eventCountryCodeConfirmed)
                        cr
                END_CALL_EVENT
                       
                switchToInSettingLoop = false;
            }
	        //sendSetCrInitialized = true; 
            break;
        case CRMC_SET_PRESENTATION_MASK:
            if (cr->state != CRS_MAIN_LOOP)
                // this command should end the IN_SETTING_LOOP sequence
                sendSetCrInitialized = true;
            else
            {
                // NO need to do the MAIN_LOOP->IN_SETTING_LOOP state switch
                sendSetCrInitialized = true;
                switchToInSettingLoop = false;
            }
            break;
        default:
            // implicit - should be started as first in the InSettingLoop state
            if (cr->state != CRS_IN_SETTING_LOOP)
            {
                
                if (!CR_SetTime(cr)) {
                    sendSetCrInitialized = true;
                }

                // do not call this, because it overrides settings from the upper system layers                
                //CR_SetCountryCode(cr);
            }
            else
                sendSetCrInitialized = true;
                    
            
            break;
    }
    
    if (sendSetCrInitialized) {
        CrCommunicator_ConceptAndEnqueue(_crResponseMessage.crAddress,CRMC_SET_CR_INITIALIZED,0,null,false,false);
    }

    if (switchToInSettingLoop)
        CR_SetState(cr,CRS_IN_SETTING_LOOP);

}

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeCommunicationModeChanged(CardReader_t* cr) {
    /*if (cr->requestedBaudRate > 0) {
        // if this response or RSP_ACK received, the requested baud rate has been confirmed
        cr->currentBaudRate = cr->requestedBaudRate;
        cr->requestedBaudRate = 0;
        cr->speedNegotiated = true;
    }*/
    
    if (cr->protocolVersionHigh < CR_FIRST_CCR_PROTO) {
        // necessary for getting the RSP_ACK
         CrCommunicator_SendPollingMessage(cr->address);  
    }
    else {
        CR_SetState(cr,CRS_IN_SETTING_LOOP);
        if (!CR_SetTime(cr))
            CrCommunicator_ConceptAndEnqueue(_crResponseMessage.crAddress,CRMC_SET_CR_INITIALIZED,0,null,false,false);
    }
    
     
}

/***************************************************************************
 *
 ***************************************************************************/
/*void AnalyzeCardData(CardReader_t* cr) 
{
    uint32 cardLength = _crResponseMessage.optionalDataLength*2;
    
    byte* cardSerial = 
        (byte*)New(cardLength);
        
    if (CRStringDecoding(_crResponseMessage.optionalData,_crResponseMessage.optionalDataLength,
                     0,_crResponseMessage.optionalDataLength,
                     cardSerial,&cardLength)) 
    {
        BEGIN_CALL_EVENT(_eventCardSwiped)
            cr,cardSerial,cardLength
        END_CALL_EVENT
    }
    
    Delete(cardSerial);

}*/

/***************************************************************************
 *
 ***************************************************************************/
//int counter = 0;

void AnalyzeKeyPressed(CardReader_t* cr) 
{
    if (null == cr)
        return;
    
    if (_crResponseMessage.optionalDataLength == 0)
        return;
        
    byte keyCode = _crResponseMessage.optionalData[0];
    /*counter++;
    char param[] = { counter / 10 + '0' , counter % 10 + '0' };
            
                CR_DisplayText(cr,1,1,param,2);*/
    
    BEGIN_CALL_EVENT(_eventKeyPressed)
        cr,keyCode
    END_CALL_EVENT
}

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeDataReceived(CardReader_t* cr) {
    if (null == cr)
        return;
    
    BEGIN_CALL_EVENT(_eventRawDataReceived)
        cr,_crResponseMessage.messageCode,(byte*)_crResponseMessage.optionalData,_crResponseMessage.optionalDataLength
    END_CALL_EVENT
}

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeDataReceived2(CardReader_t* cr,CRMessageCode_t messageCode,byte* optionalData,uint32 optionalDataLength) {
    if (null != cr)
    {
        BEGIN_CALL_EVENT(_eventRawDataReceived)
            cr,messageCode,optionalData,optionalDataLength
        END_CALL_EVENT
    }
}

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeRspAck(CardReader_t* cr) {
    if (_crResponseMessage.optionalDataLength > 0)
    {
       uint32 flags = _crResponseMessage.optionalData[0];
    }
    
    switch(cr->state)
    {
        case CRS_NOT_INITIALIZED:
                CR_SetState(cr,CRS_IN_SETTING_LOOP);
                CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
                break;
        case CRS_IN_SETTING_LOOP:

            //if (_crCommFlags & CR_COMM_OLD_BAUDRATE &&
            //    cr->lastRequest->messageCode == CRMC_SET_COMMUNICATION_LINE_MODE) {
            //    AnalyzeCommunicationModeChanged(cr);
            //    return;
            //}

            if (cr->hardwareVersion == 0) 
                 CrCommunicator_ConceptAndEnqueue(cr->address,CRMC_GET_CR_IDENTITY,0,null,false,false);
            else
            {

                //if (_crCommFlags & CR_COMM_OLD_BAUDRATE) {
                //    if (CrCommunicator_SendSetCommLineMode(cr))
                //        return;
                //}

                CR_SetState(cr,CRS_MAIN_LOOP);
                CrCommunicator_SendPollingMessage(cr->address);

            }
         
            break;
    }
}

#define CR_DELAYED_SACK_COUNT   20

byte _crDelayedSACKS[CR_DELAYED_SACK_COUNT];
uint32 _crDelayedSACKCount = 0;

/***************************************************************************
 *
 ***************************************************************************/
void AnalyzeRspServiceAck(CardReader_t* cr) {
    switch(cr->state) {
        case CRS_NOT_RESPONDING:
        case CRS_NOT_INITIALIZED:
            cr->inUpgrade = true;
            
            CR_SetState(cr,CRS_MAIN_LOOP);
            CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
            break;
        case CRS_MAIN_LOOP:
            if (!cr->inUpgrade) {
                CR_SetState(cr,CRS_NOT_INITIALIZED); // this will create OnlineStateChanged event to false
                cr->inUpgrade = true;
                CR_SetState(cr,CRS_MAIN_LOOP); // this will create OnlineStateChanged event to true
                CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
            }
            
           
        
                
            if (_crResponseMessage.optionalDataLength > 0)
            {
                
                if (cr->lastRequest->flags != CR_MESSAGE_RETRIED_BY_NACK) 
                {
                    if (_crResponseMessage.optionalData[0] != 0xFA
                        || cr->lastRequest->flags == CR_MESSAGE_RETRIED_BY_TIMEOUT
                        ) 
                    {
                        AnalyzeDataReceived(cr);
                    }
        /*
                        if (cr->lastRequest->messageCode != CRMC_FLASH_UPLOAD)
                            NOP();
                    
                    
                        if (MemoryLoad() < 20) {
                            if (_crDelayedSACKCount > 0) 
                            {
                                
                                
                                
                                AnalyzeDataReceived2(cr,CRMC_SERVICE_ACK,_crDelayedSACKS,1);
                                //if (_crDelayedSACKS > 1)
                                //    AnalyzeDataReceived2(cr,CRMC_SERVICE_ACK,_crDelayedSACKS+1,1);
                        
                                for (int i=0;i<_crDelayedSACKCount-1;i++) 
                                    _crDelayedSACKS[i]=_crDelayedSACKS[i+1];
                                
                                _crDelayedSACKS[_crDelayedSACKCount-1] = _crResponseMessage.optionalData[0];
                                
                            }
                            else
                                AnalyzeDataReceived(cr);
                        }
                        else {
                            if (_crDelayedSACKCount < CR_DELAYED_SACK_COUNT) 
                            {
                                _crDelayedSACKS[_crDelayedSACKCount++] = _crResponseMessage.optionalData[0];
                            }
                            else
                                NOP();
                        }
                    }*/
                }
#ifdef DEBUG
                else
                	NOP();
#endif
            }
            else {
                /*if (_crDelayedSACKCount > 0) 
                {
                    if (MemoryLoad() < 20) 
                    {
                        AnalyzeDataReceived2(cr,CRMC_SERVICE_ACK,_crDelayedSACKS,1);
                    
                        for (int i=0;i<_crDelayedSACKCount-1;i++) 
                            _crDelayedSACKS[i]=_crDelayedSACKS[i+1];
                        _crDelayedSACKCount --;
                    }
                }
                else {
                    if (cr->upgradeStarted && MemoryLoad() < 20) 
                    {
                        cr->upgradeStarted = false;
                        byte readyToFlash=0x0F;
                        AnalyzeDataReceived2(cr,CRMC_SERVICE_ACK,&readyToFlash,1);
                    }
                }*/
                //_crLastSACKresolution = -1;
                
            }
            break;
    }
}


#ifdef CR_DEBUG_VERBOSE

/***************************************************************************
 *
 ***************************************************************************/
void CR_Trace(
    CardReader_t* cr,
    CRMessageCode_t messageCode,
    CRTraceRecordType_e traceType,
    uint32 address,
    uint32 requestAddress,
    uint32 requestAddress2,
    CRMessageCode_t requestMessageCode) 
{
    if (cr->messageTraceCount < MAX_CRMC_TRACE_COUNT) 
    {       
        cr->messageTraceCount++;
    }
    else {
        for(int i=1;i<MAX_CRMC_TRACE_COUNT;i++)
            cr->messageTrace[i-1] = cr->messageTrace[i];
    }
      
    cr->messageTrace[cr->messageTraceCount-1].messageCode = messageCode;
    //cr->messageTrace[cr->messageTraceCount].baudRate = baudRate;
    cr->messageTrace[cr->messageTraceCount-1].type = traceType;
    cr->messageTrace[cr->messageTraceCount-1].address = address;
    cr->messageTrace[cr->messageTraceCount-1].time = SysTick_GetTickCount();
    cr->messageTrace[cr->messageTraceCount-1].requestAddress = requestAddress;
    cr->messageTrace[cr->messageTraceCount-1].requestAddress2 = requestAddress2;
    cr->messageTrace[cr->messageTraceCount-1].requestMessageCode = requestMessageCode;  
}
#endif

//#define CR_DISTORTION_TEST

/***************************************************************************
 *
 ***************************************************************************/
void CrCommunicator_OnResponseRecieved(bool isChecksumValid) 
{
#ifdef CR_DISTORTION_TEST
    uint32 tc = SysTick_GetTickCount();
    if (tc % 2 == 1) 
    {
        // fake distortion
    	NOP();
        return;
    }
#endif
    
    uint32 elapsedTimeFromRequest = 0;
    if(_crPollingState == CR_WAITING_FOR_RESPONSE_AFTER_TRANSMISSION &&
        _crResponseMessage.crAddress == _lastCRAddress)
    {
        elapsedTimeFromRequest = SysTick_GetElapsedTime(_lastRequestTimestamp);
    }
    else
    {
#ifdef CR_DEBUG_VERBOSE
        CardReader_t* cr = CrCommunicator_GetCR(_crResponseMessage.crAddress);
        CR_Trace(
                 cr,
                 _crResponseMessage.messageCode,
                 CRTR_RESPONSE_TOO_LATE,
                 _crResponseMessage.crAddress,
                 _lastCRAddress,
                 cr->lastRequest->crAddress,
                 cr->lastRequest->messageCode);
#endif
        // this means , that the response arived already late

#ifdef DEBUG
        NOP();
#ifdef CR_DEBUG_VERBOSE
        uint32 test = _crResponseMessage.messageCode + cr->lastRequest->crAddress;
#endif
#endif
        return;
    }   
    
    if (elapsedTimeFromRequest >= _lastRequestTimeout) 
    {
        // too late
        //NOP();
        //UART_Debug(COMM_PORT,"response arived late\r\n");
#ifdef CR_DEBUG_VERBOSE
        CardReader_t* cr = CrCommunicator_GetCR(_crResponseMessage.crAddress);
        CR_Trace(
                 cr,
                 _crResponseMessage.messageCode,
                 CRTR_RESPONSE_TOO_LATE,
                 _crResponseMessage.crAddress,
                 _lastCRAddress,
                 cr->lastRequest->crAddress,
                 cr->lastRequest->messageCode);    
#endif
        _crPollingState = SysTick_GetTickCount();
        return;
    }
    
    // unlocks the transmit task
    // if anything received, also identifies the time slot, where a new request can begin
    _crPollingState = SysTick_GetTickCount();
    
          
    if (!isChecksumValid) {
        //UART_Debug(COMM_PORT,"invalid checksum\r\n");
        //DeallocLastRequest(false);
#ifdef CR_DEBUG_VERBOSE
        CardReader_t* cr = CrCommunicator_GetCR(_crResponseMessage.crAddress);
        CR_Trace(
                 cr,
                 _crResponseMessage.messageCode,
                 CRTR_RESPONSE_SCRAMBLED,
                 _crResponseMessage.crAddress,
                 _lastCRAddress,
                 cr->lastRequest->crAddress,
                 cr->lastRequest->messageCode);
#endif

        CrCommunicator_ConceptAndEnqueue(_lastCRAddress,CRMC_NACK,0,null,true,false);
        return;
    }

    CardReader_t* cr = CrCommunicator_GetCR(_crResponseMessage.crAddress);
    if (cr == null) {
        return;
    }
    else {
        cr->lastRequestRetryCount = 0;
    }
    
#ifdef CR_DEBUG_VERBOSE
    CR_Trace(cr,
             _crResponseMessage.messageCode,
             CRTR_RESPONSE,
             _crResponseMessage.crAddress,
             _lastCRAddress,
             _lastCRAddress,
             cr->lastRequest->messageCode);
#endif
    
    


    // failsafe mechanism for virtually unexpected occurence of message within specified states
    switch(cr->state) {
            case CRS_NOT_RESPONDING:
                    switch(_crResponseMessage.messageCode) {
                            case CRMC_PROTOCOL_VERSION:
                                    break;
                            default:
                                    CR_SetState(cr,CRS_NOT_INITIALIZED);
                                    CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
                                    break;
                    }
                    break;
            case CRS_NOT_INITIALIZED:
                    switch(_crResponseMessage.messageCode) {
                            case CRMC_RSP_ACK:
                            case CRMC_RSP_NACK:
                            case CRMC_CR_NOT_INITIALIZED:
                            case CRMC_PROTOCOL_VERSION:
                            case CRMC_RSP_RESET:
                            case CRMC_CR_IDENTITY:
                            case CRMC_COMMUNICATION_MODE_CHANGED:
                            case CRMC_SERVICE_ACK:
                                    break;
                            default:
                                    CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
                                    break;
                    }
                    break;
            case CRS_IN_SETTING_LOOP:
                    switch(_crResponseMessage.messageCode) {
                            // every one of these special codes MUST produce at least POLLING response on it's own
                            // so they cannot remain without response in their specific handlers
                            case CRMC_RSP_ACK:
                            case CRMC_RSP_NACK:
                            case CRMC_CR_NOT_INITIALIZED:
                            case CRMC_COMMUNICATION_MODE_CHANGED:
                            case CRMC_CR_IDENTITY:
                            case CRMC_PROTOCOL_VERSION:
                            case CRMC_RSP_RESET:
                                    break;
                            case CRMC_SABOTAGE_ACTIVE:
                            case CRMC_SABOTAGE_PASSIVE:                              
                                    CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress); // must be , because otherwise the polling will fall apart
                                    break;
                            default:
                                    CR_SetState(cr,CRS_MAIN_LOOP);
                                    CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);

                    }
                    break;
    }
    
    switch(_crResponseMessage.messageCode) {
        case CRMC_RSP_ACK:
            AnalyzeRspAck(cr);
            break;
        case CRMC_RSP_NACK:
            if (null != cr->lastRequest) 
            {
                //cr->lastRequestRetryCount = 0;
                cr->lastRequest->flags = CR_MESSAGE_RETRIED_BY_NACK;
              
                CrQueue_Enqueue(cr->address,cr->lastRequest,true,false);
                // to prevent deallocation on next RequestBegin call
                // if Enqueue fails, it should deallocate the lastRequest inside
                cr->lastRequest = null;
            }
            else
                CrCommunicator_SendPollingMessage(_crResponseMessage.crAddress);
            break;
        case CRMC_SERVICE_ACK:
            AnalyzeRspServiceAck(cr);    
            break;    
        case CRMC_PROTOCOL_VERSION:
                AnalyzeProtocolVersion(cr);
                break;
        /*case CRMC_COMMUNICATION_MODE_CHANGED:
                if (_crCommFlags & CR_COMM_OLD_BAUDRATE)
                    AnalyzeCommunicationModeChanged(cr);                     
                break;*/

        case CRMC_CR_IDENTITY:
                AnalyzeCrIdentity(cr);
                break;
        case CRMC_RSP_RESET:
                AnalyzeReset(cr);
                break;
        case CRMC_CR_NOT_INITIALIZED:
                AnalyzeCrNotInitialized(cr);
                break;
        case CRMC_CODE_DATA:
                AnalyzeDataReceived(cr);
                break;
        case CRMC_CODE_TIMEOUT:
                AnalyzeDataReceived(cr);
                break;
        case CRMC_CARD_DATA:
                //AnalyzeCardData(cr);
                AnalyzeDataReceived(cr);
                break;
                
        case CRMC_SABOTAGE_ACTIVE:
                if (!cr->inSabotage) {
                    cr->inSabotage = true;
                    AnalyzeDataReceived(cr);
                }
                break;
                
        case CRMC_SABOTAGE_PASSIVE:
                if (cr->inSabotage) {
                    cr->inSabotage = false;
                    
                    if (cr->isOnline) {
                        if (cr->state == CRS_MAIN_LOOP) 
                        {
                            // this is a return from tamper throughout normal usage
                            CrCommunicator_ConceptAndEnqueue(cr->address,CRMC_SET_CR_INITIALIZED,0,null,true,false);
                        }
                        
                        // do not call this when cr->isOnline is false, because the 
                        // cronlinestatechanged event will provide equivalent state reporting
                        BEGIN_CALL_EVENT(_eventTamperReturn)
                            cr
                        END_CALL_EVENT
                    }
                    
                    AnalyzeDataReceived(cr);
                }
                break;
        case CRMC_SINGLE_KEY_PRESSED:
                
                AnalyzeKeyPressed(cr);
                AnalyzeDataReceived(cr);
                break;
                    
        default:
                AnalyzeDataReceived(cr);
                break;
    }        
}

/***************************************************************************
 *
 ***************************************************************************/
void CrCommunicator_ReceiveTask() {

    /*switch(_readingPhase) {
        case CRRP_WAIT_FOR_PREAMBLE:
            UART_Receive(CR_PORT,CR_PREAMBLE_LENGTH,CrCommunicator_PHDParsing);        
            break;
        case CRRP_WAIT_FOR_HEADER:
        case CRRP_WAIT_FOR_DATA:
            UART_Receive(CR_PORT,1,CrCommunicator_PHDParsing);
            break;
    }*/
    
    UART_Receive(CR_PORT,1,CrCommunicator_PHDParsing);
}


/***************************************************************************
 * starts transmission of the request over the RS485 line
 * scales the baudrate according the CR settings 
 ***************************************************************************/ 
void CrCommunicator_RequestBegin(CardReader_t* cr,CRMessage_t* request) {
    if (null == request || null == cr) {
        //NOP();
        return;
    }
    
    //EnterCriticalSection();
       
    byte rawRequestData[CR_MAX_FRAME_LENGTH];
    uint32 outBufferSize = CR_MAX_FRAME_LENGTH;
       
    uint32 requestSize = CR_PREAMBLE_N_HEADER_LENGTH+request->optionalDataLength;
    
    if (outBufferSize < requestSize) {
        //LeaveCriticalSection();
#ifdef DEBUG
    	NOP();
        
        // just to keep the values of optimisation
        if (outBufferSize -2 < requestSize)
            NOP();
#endif
        return;
    }
    
    rawRequestData[0] = 0xFF;
    rawRequestData[1] = request->crAddress;
    rawRequestData[2] = request->optionalDataLength;
    rawRequestData[3] = request->messageCode;
    
    if (request->optionalDataLength > 0) 
    {       
        for(int i=0;i<request->optionalDataLength;i++) {
            rawRequestData[CR_PREAMBLE_N_HEADER_LENGTH+i] = request->optionalData[i];
        }  
    }
    
    bool isArithemticChecksum = true;
    if (cr->protocolVersionHigh >= CR_FIRST_CCR_PROTO &&
        request->messageCode != CRMC_GET_PROTOCOL_VERSION)
        isArithemticChecksum = false;
    
    if (isArithemticChecksum) 
    {
        uint32 calculatedChecksum = request->crAddress + request->optionalDataLength +
            request->messageCode;
        
        if (request->optionalDataLength > 0) {       
            for(int i=0;i<request->optionalDataLength;i++) {
                calculatedChecksum += request->optionalData[i];
            }  
        }
    
        calculatedChecksum = calculatedChecksum*2;
        
        rawRequestData[4] = (calculatedChecksum & 0xFF00) >> 8;
        rawRequestData[5] = (calculatedChecksum & 0x00FF);
    }
    else {
        rawRequestData[4] = 0xE3; // preset for CRC8
        if (request->optionalDataLength > 0)
            rawRequestData[4] = Crc8(request->optionalData,0,request->optionalDataLength);
        
        uint32 headerChecksum = Crc8(rawRequestData,CR_PREAMBLE_LENGTH,CR_HEADER_LENGTH-1);
        
        rawRequestData[5] = headerChecksum & 0xFF;
    }
    
    switch(request->messageCode) {
        case CRMC_ACK_GET_EVENT:
            if (_crCommFlags & CR_COMM_OLD_BAUDRATE)
                _lastRequestTimeout = TCR_ACK_GET_EVENT_TIMEOUT;
            else
                _lastRequestTimeout = CCR_ACK_GET_EVENT_TIMEOUT;
            break;
        case CRMC_SET_PRESENTATION_MASK:
            _lastRequestTimeout = CR_PRESENTATION_MASK_TIMEOUT;
            break;
        case CRMC_MENU_SYSTEM:
        case CRMC_PUT_ANNOUNCMENT:
            _lastRequestTimeout = CR_MENU_COMMANDS_TIMEOUT;
            break;
        case CRMC_DISPLAY_TEXT:
            _lastRequestTimeout = CR_DISPLAY_TEXT_TIMEOUT;
            break;
        case CRMC_GET_PROTOCOL_VERSION:
            if (_crCommFlags & CR_COMM_OLD_BAUDRATE)
                _lastRequestTimeout = TCR_GET_PROTOCOL_VERSION_TIMEOUT;
            else
               _lastRequestTimeout = CCR_GET_PROTOCOL_VERSION_TIMEOUT; 
            break;
        case CRMC_SET_CARD_SYS:    
        case  CRMC_VALIDATE_CARD_SYS:
            _lastRequestTimeout = CR_SMART_COMMANDS_TIMEOUT;
            break; 
        case CRMC_FLASH_UPLOAD:
            _lastRequestTimeout = CR_FLASHING_COMMANDS_TIMEOUT;
            break;
        default:
            if (_crCommFlags & CR_COMM_OLD_BAUDRATE)
                _lastRequestTimeout = TCR_DEFAULT_COMMAND_TIMEOUT;
            else
                _lastRequestTimeout = CCR_DEFAULT_COMMAND_TIMEOUT;
            break;
    }   
  
#ifndef BOOTLOADER
    //if (_crCommFlags & CR_COMM_OLD_BAUDRATE &&
    //    UART_GetBaudRate(CR_PORT) != FIRST_COMMON_CR_BAUDRATE) {
    //    UART_Init(CR_PORT,FIRST_COMMON_CR_BAUDRATE);
    //}
#endif
    
    // in the _lastRequestTimeout there should be count for the lasting of the transmission itself
    uint32 charTime;
    
    switch(UART_GetBaudRate(CR_PORT)) {
        case 9600:
            charTime = CR_CCA_CHAR_TIME_9600;
            break;
        case 19200:
            charTime = CR_CCA_CHAR_TIME_19200;
            break;            
        case 38400:
            charTime = CR_CCA_CHAR_TIME_38400;
            break;
        case 57600:
            charTime = CR_CCA_CHAR_TIME_57600;
            break;
        case 115200:
            charTime = CR_CCA_CHAR_TIME_115200;
            break;
        case 4800:
        default:   
            charTime = CR_CCA_CHAR_TIME_4800;
            break;
    }
    
    uint32 expectedTransmissionTime = requestSize*charTime;
    expectedTransmissionTime = expectedTransmissionTime / 1000 + (expectedTransmissionTime % 1000 > 0 ? 1 : 0); // to transform usec into ms      

    
    uint32 beginSendTc = SysTick_GetTickCount();
    UART_Send(CR_PORT,rawRequestData,requestSize);
    uint32 elapsed = SysTick_GetElapsedTime(beginSendTc);
    
    _lastRequestTimestamp = SysTick_GetTickCount();  
    
    if (elapsed < expectedTransmissionTime)
        _lastRequestTimeout += (expectedTransmissionTime - elapsed);
    
    
#ifdef CR_DEBUG_VERBOSE
    if (request->messageCode != CRMC_GET_PROTOCOL_VERSION)
    {
        CR_Trace(cr,
                 request->messageCode,
                 CRTR_REQUEST,
                 request->crAddress,
                 0,
                 0,
                 request->messageCode);
    }
#endif
    
    // DEALLOCATION of previous request in post procesing
    // checks null inside
    if (null != cr->lastRequest)
        Delete(cr->lastRequest);
    
    cr->lastRequest = request;    
    _lastCRAddress = cr->address;
      

}

/***************************************************************************
 *
 ***************************************************************************/
void CrCommunicator_CheckReceptionTimeoutTask() {
    if (_crPollingState == CR_WAITING_FOR_RESPONSE_AFTER_TRANSMISSION)
    {
        uint32 elapsedTimeFromRequest =  SysTick_GetElapsedTime(_lastRequestTimestamp);
    
        // CHECKS TIMEOUTS
        if (elapsedTimeFromRequest >= _lastRequestTimeout)
        {
            
            
            // THIS MEANS THAT REQUEST TIMED OUT
            CardReader_t* cr = CrCommunicator_GetCR(_lastCRAddress);
            if (null == cr) {
#ifdef DEBUG
            	NOP();
#endif
                return;
            }
            
#ifdef CR_DEBUG_VERBOSE
            CR_Trace(
                     cr,
                     cr->lastRequest->messageCode,
                     CRTR_TIMEOUT,
                     _lastCRAddress,
                     0,
                     0,
                     cr->lastRequest->messageCode
                     );
#endif
            
            if (cr->lastRequest == null)
            {
                // possibly a result of Out of memory
                // _crPollingState = CR_TRANSMISSION_AFTER_TIMEOUT;
                return;
            }
                        
            if (cr->lastRequest->messageCode == CRMC_RESET) {
                CR_SetState(cr,CRS_NOT_RESPONDING);
                //DeallocLastRequest(false);                
            }
            else {
            
                if (cr->lastRequest->messageCode == CRMC_GET_PROTOCOL_VERSION) {

/*
#ifdef ROTATE_OVER_BAUDRATES
                    if (_maxLineBaudRate > FIRST_COMMON_CR_BAUDRATE) {
                    
                        // ROUND ROBIN OVER BAUDRATES
                        switch(cr->currentBaudRate) {
                            case 4800:
                                cr->currentBaudRate = SECOND_COMMON_CR_BAUDRATE;
                                break;
                            case 9600:
                                cr->currentBaudRate = SECOND_COMMON_CR_BAUDRATE;
                                break;
                            case 19200:
                                cr->currentBaudRate = FIRST_COMMON_CR_BAUDRATE;
                                break;
                            default:
                                cr->currentBaudRate = FIRST_COMMON_CR_BAUDRATE;
                                break;
                        }
                    }
#endif
*/
                    
                    CR_SetState(cr,CRS_NOT_RESPONDING);
                    //DeallocLastRequest(false);
                }
                else {
            
                    if (cr->lastRequestRetryCount < MAX_RETRY_COUNT) 
                    {    
                        cr->lastRequestRetryCount++;
                        
                        cr->lastRequest->flags = CR_MESSAGE_RETRIED_BY_TIMEOUT;
                                                
                        CrQueue_Enqueue(cr->lastRequest->crAddress,cr->lastRequest,true,false);
                        // TO PREVENT DEALLOCATION ON CrCommunicator_RequestBegin 
                        // if Enqueue fails, it should deallocate the lastRequest inside
                        cr->lastRequest = null;
                        
                    }
                    else 
                    {
                        
                        // cr->lastRequestRetryCount = 0; no need to be done here, done by CR_SetState
                        
                        CR_SetState(cr,CRS_NOT_RESPONDING); // the cr->lastRequest will be nulled by this call            
                        //DeallocLastRequest(false);
                    }
                }
            }

            // means timeout - request ended gracefuly
            // to ensure atomitcity, call it at the end of timeout handling
            
             _crPollingState = CR_TRANSMISSION_AFTER_TIMEOUT;
        }
    }
}

uint32 _lastSetTimeRefresh = 0;

/***************************************************************************
 *
 ***************************************************************************/
void CrCommunicator_TransmitTask() 
{
    uint32 delayBetweenSuccessfulPolling = CCR_DELAY_BETWEEN_SUCCESSFUL_POLLING;
    if (_crCommFlags & CR_COMM_OLD_BAUDRATE)
        delayBetweenSuccessfulPolling = TCR_DELAY_BETWEEN_SUCCESSFUL_POLLING;
    
   
    // start transmission either after timeout
    // or after previous successful reception ,ensuring the minimal gap because of RE/ reswitching    
    if (_crPollingState == CR_TRANSMISSION_AFTER_TIMEOUT 
        || (_crPollingState >= 0 && SysTick_GetElapsedTime(_crPollingState) >= delayBetweenSuccessfulPolling)) 
    {
        
        // ROUND ROBIN
        uint32 nextCrAddress;
        if (_lastCRAddress == CR_MAX_COUNT)
            nextCrAddress = 1;
        else
            nextCrAddress = _lastCRAddress+1;
        
        CardReader_t* cr = CrCommunicator_GetCR(nextCrAddress);
        if (cr == null) {
#ifdef DEBUG
        	NOP();
#endif
            return;
        }
        
/*#ifdef DEBUG
        if (!cr->isEnabled) {
            // ROUND ROBIN FOR DEBUG
            if (nextCrAddress == 1)
                cr = CrCommunicator_GetCR(2);
            else
                cr = CrCommunicator_GetCR(1);

            if (!cr->isEnabled)
                return;
        }
#endif  */
       
        
        CRMessage_t* message = CrQueue_Pop(cr->address);
         
        if (null == message) {
            // IDLE messages
            switch(cr->state) {
                case CRS_NOT_RESPONDING:
                    {
                        byte protoVer[2];
                        protoVer[0] = 0x03;
                        protoVer[1] = 0x00;
                        message = ConceptRequest(cr->address,CRMC_GET_PROTOCOL_VERSION,2,protoVer);
                    }
                    break;
                case CRS_MAIN_LOOP:
                    /*
                    if (SysTick_GetMinute() == 1 &&
                        SysTick_GetElapsedTime(_lastSetTimeRefresh) > 60000) {
                        _lastSetTimeRefresh = SysTick_GetTickCount();
                        message = ConceptSetTime(cr);
                    }*/
                
                    if (null == message)
                        message = ConceptRequest(cr->address,CRMC_ACK_GET_EVENT,0,null);
                    break;
            }
        }
        
        if (message!=null) {
        
            CrCommunicator_RequestBegin(cr,message); // this call will also confirm nextAddress as _lastCRAddress
        
            _crPollingState = CR_WAITING_FOR_RESPONSE_AFTER_TRANSMISSION; 
        }
    }
}

/***************************************************************************
 * SHOULD NOT BE CALLED ANYWHERE ELSE BUT MAIN
 ***************************************************************************/
void CrCommunicator_Init() {
    if (!(_crCommFlags & CR_COMM_INITIALIZED)) {
        
        int32 separateSpeeds = GPIO_GetValue(PORT0, 27); // DIP6
        separateSpeeds = GPIO_GetValue(PORT0, 27); // DIP6 - no filtertime etc - lookup only on boot

        if (separateSpeeds >= 0 && separateSpeeds == 0) // inverted value
            _crCommFlags |= CR_COMM_OLD_BAUDRATE;
        
        if (_crCommFlags & CR_COMM_OLD_BAUDRATE)   
            UART_Init(CR_PORT,FIRST_COMMON_CR_BAUDRATE);
        else
            UART_Init(CR_PORT,FIXED_COMMON_CR_BAUDRATE);

        
        for(int i=0;i< CR_MAX_COUNT;i++) {
            _cardReaders[i].address = i+1;           
            _cardReaders[i].isOnline = false;
            _cardReaders[i].state = CRS_NOT_RESPONDING;
            _cardReaders[i].lastRequest = null;
            _cardReaders[i].language = 0; // stands for Neutral/English language
            
#ifdef DEBUG
            //if (i == 1)
            _cardReaders[i].isEnabled = true;
#endif
            
            for (int j=0;j<CR_SET_PRESENTATION_MASK_ODL;j++)
                _cardReaders[i].upperSystemPresentationMask[j] = 0xFF;
           
            CR_SetInitialValues((CardReader_t*)&(_cardReaders[i]),true);
            
        }
        
        //_crResponseMessage.optionalData = (byte*)_crResponseMessageOptionalData;           
        
        Tasks_Register(CrCommunicator_TransmitTask,10);
        Tasks_Register(CrCommunicator_ReceiveTask,9);
        Tasks_Register(CrCommunicator_CheckReceptionTimeoutTask,10);
        

        _crCommFlags |= CR_COMM_INITIALIZED;
    }
    
    
    
}

/***************************************************************************
 *
 ***************************************************************************/
void CrCommunicator_FireOnlineEvents() 
{   
    for(int i=0;i<CR_MAX_COUNT;i++) 
    {
        CardReader_t* cr = (CardReader_t*)&(_cardReaders[i]);
        if (cr->isOnline
#ifdef DEBUG
            && cr->isEnabled
#endif
           )
        {
            BEGIN_CALL_EVENT( _eventOnlineStateChanged )
                cr,cr->isOnline
            END_CALL_EVENT
        }
#ifdef DEBUG
        else {
        	asm("NOP");
                //NOP();
            if (cr->isOnline)
            	asm("NOP");
              //NOP();
        }
#endif
        
    }
}

/***************************************************************************
 *
 ***************************************************************************/
bool CR_HasDisplay(CardReader_t* cr) {
    if (null == cr)
        return false;
    
    switch (cr->hardwareVersion)
    {
        case CRHW_ProximityFull:
        case CRHW_ASPMotorolaProximityFull:
        case CRHW_SmartFull :
        case CRHW_SmartQTouchKeyboard:
        case CRHW_HIDProximityFull:
        case CRHW_ComboSmartProximityFull:
        case CRHW_ProximityLiteCCR:
        case CRHW_Proximity8LineLcd:
        case CRHW_ProximityPremiumCCR:
        case CRHW_SmartLiteCCR:
        case CRHW_SmartQTouchCCR:
        case CRHW_Smart8LineLCD:
        case CRHW_SmartPremiumCCR:
        case CRHW_HIDProximity8LineLCDFull:
            return true;
        default:
            return false;
    }
}


/***************************************************************************
 *
 ***************************************************************************/
/*
void CR_WaitForPIN(CardReader_t* cr,uint32 pinLength) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return;
    
    if (pinLength < 4)
        pinLength = 4;
    else
        if (pinLength>8)
            pinLength = 8;
    
	byte params[2] = { 0x3, pinLength };

	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_WAITING_FOR_PIN_CODE,
		2,
		params,
        false,false);
}*/

/***************************************************************************
 *
 ***************************************************************************/
/*
void CR_WaitForGIN(CardReader_t* cr,uint32 ginLength) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return;
    
    if (ginLength < 4)
        ginLength = 4;
    else
        if (ginLength>8)
            ginLength = 8;
    
	byte params[2] = { 0x3, ginLength };

	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_WAITING_FOR_GIN_CODE,
		2,
		params,
        false,false);
}*/

/***************************************************************************
 *
 ***************************************************************************/
/*
void CR_Accepted(CardReader_t* cr) 
{

    if (!CR_ValidateCommandSuitable(cr))
        return;
    
	byte params[2] = { 0x3 };
    
	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_ACCEPT,
		1,
		params,
        false,false);
}
*/

/***************************************************************************
 *
 ***************************************************************************/
/*void CR_Rejected(CardReader_t* cr) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return;
    
	byte params[2] = { 0x3 };
    
	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_REJECT_REQUEST,
		1,
		params,
        false,false);
}*/

/***************************************************************************
 *
 ***************************************************************************/
/*
void CR_DisplaySymbol(CardReader_t* cr,uint32 left,uint32 top,uint32 symbolId) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return;
    
	byte params[3] = { left, top, symbolId };
    
	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_DISPLAY_SYMBOL,
		3,
		params,
        false,false);
}*/

/***************************************************************************
 *
 ***************************************************************************/
/*
void CR_DisplayDefaultText(CardReader_t* cr,uint32 left,uint32 top,uint32 textId) 
{
    if (!CR_ValidateCommandSuitable(cr))
        return;
    
	byte params[3] = { 
        ((left & 0x0F) << 4) | (top & 0x0F),
        0,
        textId };
    
	CrCommunicator_ConceptAndEnqueue(
		cr->address,
		CRMC_DISPLAY_DEFAULT_TEXT,
		3,
		params,
        false);
}*/


#endif // BOOTLOADER
