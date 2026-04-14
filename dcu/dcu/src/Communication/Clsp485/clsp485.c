#pragma diag_suppress=Pa082
#include "Communication\ClspGeneric.h"
#include "Clsp485.h"
#include "System\baseTypes.h"
#include "System\UART\uart.h"

#include "System\Timer\sysTick.h"

#include "Crypto\hashes.h"
#include "System\IAP\iap.h"
#include "System\GPIO\gpio.h"

//#include "IO\Outputs.h"
#include "IO\Inputs.h"

#include "System\Tasks.h"

#include "Crypto\cryptoaes.h"
#include "Crypto\hashes.h"

//#define DEBUG_COMM

#include "System\dmHandler.h"
#include "IO\Outputs.h"



#define ENCRYPTION_FLAG                         0x80
#define ENCRYPTION_CLEAR                        0x7F

// Crc8 checksum of the internal LPCxx processor UID
volatile int32 _uidChecksum = -1;
volatile int32 _maxNodeLookupSequence = -1;

//to distinct no communication from master from some time
volatile int32 _clspLastFrameReceivedTimestamp = -1;

// if the request from master was encrypted the next communication will be encrypted as well
volatile bool _clspIsEncrypted = false;

#ifdef DEBUG
volatile int32 _clspLastReceptionTime = -1;
#endif

int32 _clspNodeNotifiedTime = -1;




/************************************************************************************************************/

volatile uint32 _clsp485RemainingHeaderBytes = 0;
volatile uint32 _clsp485RemainingDataBytes = 0;

//volatile uint32 _errorRate = 100;

// defines latency of DCU between received master frame and transmitted response
// in ms
volatile uint32 _clspResponseLatency = 0;

#ifdef DEBUG
volatile uint32 _clspResponseTimeout = 0;
#endif

// maximum time, that node can remain unpolled by master 
// if this time elapses, node becomes released from addressing scheme
volatile uint32 _maxUnpolledTime = 3; // must be defined, until no NODE_LOOKUP received

volatile int32 _clspAppealForTransmissionSQNumber = -1;


volatile byte* _aesKey = null;
volatile byte* _aesIV = null;

// reading states from the UART
enum {
    CLRP_WaitForPreamble,
    CLRP_WaitForUnclearHeader,
    CLRP_WaitForHeader,
    CLRP_WaitForData,
    CLRP_FILLER = 0xFFFFFFFF
} _clspReadingPhase ;


volatile ClspFrame_t _clspReceivedFrame; // = { 0,0,0,0,0,0,0,0,null};
volatile uint32 _clspReceivedDataChecksum;

//volatile byte _clsp485masterFrameOptionalData[CLSP_MAX_OPTIONAL_DATA_LENGTH];

#ifndef BOOTLOADER
#define MAX_QUEUE_LENGTH        32
#define MAX_LLQUEUE_LENGTH      8
#else
#define MAX_QUEUE_LENGTH        16
#define MAX_LLQUEUE_LENGTH      4
#endif

volatile int32 _clspPreviousFrameSQ = -1;

#ifdef DEBUG
volatile int32 _clspPreviousFrameMC = -1;
#endif

// pre-definition
void Clsp485_OnFrameReceived();


/***************************************************************************
 *
 ***************************************************************************/
bool Clsp485_ComposeByFrameAndSend(ClspFrame_t* frame) 
{
    if (frame == null)
        return false;
    
    byte rawResponse[CLSP_MAX_FRAME_LENGTH];
    uint32 rawResponseLength;
    
    rawResponse[0] = 0xEF;
    rawResponse[1] = 0xFE;
    rawResponse[2] = frame->address;
    rawResponse[3] = frame->messageCode;
    rawResponse[4] = frame->messageParam;
    rawResponse[5] = frame->sequenceNumber;
    
    if (null == frame->optionalData && frame->optionalDataLength > 0)
        frame->optionalDataLength = 0;
          
    if (frame->messageCode != RESEND_REQUIRED &&       // do not encrypt "resend frame"
        _clspIsEncrypted &&
        _aesKey != null && _aesIV != null)
    {
        AES_IVInit((byte*)_aesIV);
        AES_KeyInit((byte*)_aesKey);
        
        if (frame->optionalDataLength > CLSP_MAX_ENCDATA_LENGTH)
            frame->optionalDataLength = CLSP_MAX_ENCDATA_LENGTH;
        
        byte dataToEncrypt[CLSP_MAX_OPTIONAL_DATA_LENGTH];
        uint32 encryptedSize = 0;
        
        dataToEncrypt[0] = frame->messageParam;
        
        if (frame->optionalDataLength > 0 && frame->optionalData != null) 
        {
            for (int i = 0; i < frame->optionalDataLength; i++)
                dataToEncrypt[1 + i] = frame->optionalData[i];
        }    
        
        byte* encryptedData = AES_Encrypt(dataToEncrypt, frame->optionalDataLength + 1, &encryptedSize);

        rawResponse[3] |= ENCRYPTION_FLAG;
        rawResponse[4] = encryptedData[0];  
        rawResponse[6] = encryptedSize - 1;

        if (encryptedSize > 0)
        {
            for (int i = 1; i < encryptedSize; i++)
                rawResponse[(i - 1) + CLSP_FRAME_PREFIX_LENGTH] = encryptedData[i];
        }
        
        rawResponseLength = CLSP_FRAME_PREFIX_LENGTH + encryptedSize - 1;
    }
    else
    {
        if (frame->optionalDataLength > CLSP_MAX_OPTIONAL_DATA_LENGTH)
            frame->optionalDataLength = CLSP_MAX_OPTIONAL_DATA_LENGTH;
        
        rawResponse[6] = frame->optionalDataLength;
            
        if (frame->optionalDataLength > 0 && frame->optionalData != null) 
        {
            for (int i = 0; i < frame->optionalDataLength; i++)
                rawResponse[i + CLSP_FRAME_PREFIX_LENGTH] = frame->optionalData[i];                
        }
        
        rawResponseLength = CLSP_FRAME_PREFIX_LENGTH + frame->optionalDataLength;
    }
    
    rawResponse[5] = _clspAppealForTransmissionSQNumber;
    
#ifdef DEBUG
    if (rawResponse[3] == STACKED_FRAME)
        __ASM("NOP");
#endif
    
    if (frame->optionalDataLength > 0)
        rawResponse[7] = Crc8((byte*)rawResponse, CLSP_FRAME_PREFIX_LENGTH, frame->optionalDataLength); // DATA CHECKSUM
    else
        rawResponse[7] = 0xE3; // NO DATA IS MARKED BY CRC8 PRESET, WHICH IS 0xE3
    
    rawResponse[8] = Crc8((byte*)rawResponse, CLSP_PREAMBLE_LENGTH, CLSP_HEADER_LENGTH - 1); // HEADER CHECKSUM
    
    UART_Send(COMM_PORT, rawResponse, rawResponseLength);
    
    return true;
    
}


/***************************************************************************
 *
 ***************************************************************************/
int32 Clsp485_PHDParsing(byte* buffer, uint32 size, uint32 timestamp) 
{  
    int32 start = -1;
        
    //_allSize += size;
    
    //uint32 tc = SysTick_GetTickCount();
    
    unsigned char volatile* formerBuffer = buffer;
    
    bool continueProcessing;
    do 
    {
        continueProcessing = false;
        int32 sizeToSearch = size - (buffer - formerBuffer);
        //uint32 address;
        
        switch(_clspReadingPhase) 
        {
            case CLRP_WaitForPreamble: 
                for(int i = 0; i < sizeToSearch - 1; i++) 
                {
                    if (buffer[i] == 0xFE && buffer[i + 1] == 0xEF) 
                    {
                       // possibly a PREAMBLE
                       start = i;
                       
                       // increments buffer pointer
                       _clspReadingPhase = CLRP_WaitForHeader;
                       _clsp485RemainingHeaderBytes = CLSP_HEADER_LENGTH;
                                
                       if ((sizeToSearch - start - CLSP_PREAMBLE_LENGTH) > 0) 
                       {
                            buffer = buffer + start + CLSP_PREAMBLE_LENGTH;
                            continueProcessing = true;
                       }
                       
                       break;
                    }
                }
                
                if (start == -1) 
                {
                    if (buffer[sizeToSearch - 1] == 0xFE)
                        return 1;       // this means, that the UART_RxBuffer would not be cleared/shifted of actual data
                                        // it will be waiting for new data , and they will together with old data be parsed by this function again
                    else
                        return 0;       // the conformant UART Rx bytes that had been pushed to this function will be cleared
                }
                
                break;
                
            case CLRP_WaitForHeader:
                switch(_clsp485RemainingHeaderBytes) 
                {
                    case 7:
                        {
                            uint32 address = buffer[0];
                            
                            if (address == 0                                                   // FRAME IS INTENDED FOR MASTER
                                || address > CLSP485_BCAST                                      // INVALID OR DISTORTED ADDRESS IN HEADER
                                || (_clspLogicalAddress == 0 && address != CLSP485_BCAST)           // UNICAST MESSAGE, WHEN NODE HAS NO ADDRESS ASIGNED
                                || (_clspLogicalAddress != 0 && address != _clspLogicalAddress && address != CLSP485_BCAST)         // UNICAST ADDRESS NOT MEANT TO THIS NODE
                                                                                                                                    // BUT ALLOW BCAST TO PASS TO RECEIVE EVEN SO
                                    )
                            {  
                                _clspReadingPhase = CLRP_WaitForPreamble;
                            }
                            else 
                            {    
                                _clspReceivedFrame.address = address;
                            }
                        }
                        break;
                    case 6:
                        _clspReceivedFrame.messageCode = buffer[0];
                        break;
                    case 5:
                        _clspReceivedFrame.messageParam = buffer[0];
                        break;                    
                    case 4:
                        _clspReceivedFrame.sequenceNumber = buffer[0];
                        break;
                    case 3:   
                        _clspReceivedFrame.optionalDataLength = buffer[0];
                        break;
                    case 2:
                        _clspReceivedDataChecksum = buffer[0];
                        break;
                    case 1:
                        {
                            uint32 headerChecksum = buffer[0];
                            uint32 countedCrc;
                            
                            {
                                byte tmpHeader[CLSP_HEADER_LENGTH - 1] = { 
                                    _clspReceivedFrame.address,
                                    _clspReceivedFrame.messageCode,
                                    _clspReceivedFrame.messageParam,
                                    _clspReceivedFrame.sequenceNumber,
                                    _clspReceivedFrame.optionalDataLength,
                                    _clspReceivedDataChecksum };
                               
                                countedCrc = Crc8(tmpHeader,0,CLSP_HEADER_LENGTH - 1);
                            }
                            
                            
                            
                            if (countedCrc != headerChecksum) 
                            {
                                 // _clspReadingPhase = WaitForPreamble; let it be done later, so during the debug you'll see actual readingPhase
                                
                                _clspReceivedFrame.flags &= CLSP_FRAME_CHECKSUM_INVALID;


#ifdef DEBUG
                                _clspLastReceptionTime = timestamp;
#endif
                                
                                Clsp485_OnFrameReceived();
                                
                                _clspReadingPhase = CLRP_WaitForPreamble; 
                            }
                            else 
                            {
                                _clspReceivedFrame.flags |= CLSP_FRAME_CHECKSUM_VALID;
                            
                                if (_clspReceivedFrame.optionalDataLength > 0 && 
                                    _clspReceivedFrame.optionalDataLength <= CLSP_MAX_OPTIONAL_DATA_LENGTH) 
                                {
                                    _clsp485RemainingDataBytes = _clspReceivedFrame.optionalDataLength;
                                    _clspReadingPhase = CLRP_WaitForData;
                                }
                                else 
                                {
                                     // _clspReadingPhase = WaitForPreamble; let it be done later, so during the debug you'll see actual readingPhase
                                    
#ifdef DEBUG
                                    _clspLastReceptionTime = timestamp;
#endif
                                    
                                    // HANDLE THE ACTUAL RESPONSE
                                    Clsp485_OnFrameReceived();
                                    
                                    _clspReadingPhase = CLRP_WaitForPreamble; 
                                }
                            }
                            
                        }
                        break;

                }
                
                _clsp485RemainingHeaderBytes--;
                
                if (sizeToSearch > 1) 
                {
                    buffer++;
                    continueProcessing = true;
                }
                        
                break;   
                
            case CLRP_WaitForData:            

                _clspReceivedFrame.optionalData[_clspReceivedFrame.optionalDataLength - _clsp485RemainingDataBytes] = buffer[0];
                
                _clsp485RemainingDataBytes--;
                
                if (_clsp485RemainingDataBytes == 0) 
                {
                   // _clspReadingPhase = WaitForPreamble; let it be done later, so during the debug you'll see actual readingPhase
                         
                   if (Crc8((byte*)_clspReceivedFrame.optionalData, 0, _clspReceivedFrame.optionalDataLength) != _clspReceivedDataChecksum)
                       _clspReceivedFrame.flags &= CLSP_FRAME_CHECKSUM_INVALID;
                   else
                       _clspReceivedFrame.flags |= CLSP_FRAME_CHECKSUM_VALID;
                   
#ifdef DEBUG
                   _clspLastReceptionTime = timestamp;
#endif
                   
                   Clsp485_OnFrameReceived();
                   
                   _clspReadingPhase = CLRP_WaitForPreamble;
                }
                
                if (sizeToSearch > 1)
                {
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
void Clsp485_AppealForTransmission(int32 sequenceNumber) {
    _clspLastFrameReceivedTimestamp = SysTick_GetTickCount();
    _clspAppealForTransmissionSQNumber = sequenceNumber;
}

#ifdef DEBUG
uint32 _clspNodeAssignmentCount = 0;
#endif

void Clsp485_UidChecksumRandomize() {
    // RANDOMIZE UID CHECKSUM
    
    uint32 tc = SysTick_GetTickCount();
    tc = tc % UID_LENGTH;
    byte* uid = IAP_ReadSerialNumber();
    
    if (uid != null) {
        _uidChecksum = (byte)((_uidChecksum + uid[tc]) % (_maxNodeLookupSequence + 1));
        _clspNodeNotifiedTime = -1;
    }
}

/***************************************************************************
 *
 ***************************************************************************/
//volatile uint32 chckErrorCount = 0;


void Clsp485_OnFrameReceived() 
{
    //_allSize = 0;
    
    if ((_clspReceivedFrame.flags & CLSP_FRAME_CHECKSUM_VALID) == 0)   
    {        
        if (_clspLogicalAddress != 0 && _clspLogicalAddress == _clspReceivedFrame.address) {   
            Clsp_EnqueueLLFrame(Clsp_InternalCreateFrame(_clspLogicalAddress, RESEND_REQUIRED, _clspReceivedFrame.messageCode, 0, false));
           
            Clsp485_AppealForTransmission(_clspReceivedFrame.sequenceNumber);
        }
        // else
            // address is distorted or irelevant    
                       
        return;
    }
    
    /* If the MSB is set -> encrypted communication */
    if (_clspReceivedFrame.messageCode & ENCRYPTION_FLAG)
    {
        _clspIsEncrypted = true;
        /* Clear the MSB so the message code can be read properly */
        _clspReceivedFrame.messageCode &= ENCRYPTION_CLEAR;
    }
    else
    {
        _clspIsEncrypted = false;
    }
    
    if (_clspIsEncrypted && _aesKey != null && _aesIV != null)
    {
        byte dataToDecrypt[CLSP_MAX_OPTIONAL_DATA_LENGTH];
        
        dataToDecrypt[0] = _clspReceivedFrame.messageParam;
        
        if (_clspReceivedFrame.optionalDataLength > 0)
        {
            for (int i = 0; i < _clspReceivedFrame.optionalDataLength; i++)
                dataToDecrypt[i + 1] = _clspReceivedFrame.optionalData[i];
        }
        
        AES_IVInit((byte*)_aesIV);
        AES_KeyInit((byte*)_aesKey);
        
        uint32 plainSize = 0;
        
        byte* decryptedData = AES_Decrypt(dataToDecrypt, _clspReceivedFrame.optionalDataLength + 1, &plainSize);
        
        ClspFrame_t* tmpFrame = Clsp_InternalCreateFrame(_clspReceivedFrame.address, _clspReceivedFrame.messageCode, 
                                               _clspReceivedFrame.messageParam, plainSize - 1, false);
        tmpFrame->messageParam = decryptedData[0];
        tmpFrame->sequenceNumber = _clspReceivedFrame.sequenceNumber;
        tmpFrame->optionalDataLength = plainSize - 1;
        
        for (int i = 0; i < plainSize - 1; i++)
            tmpFrame->optionalData[i] = decryptedData[i + 1];
        
        //_clspReceivedFrame = tmpFrame;  
        _clspReceivedFrame.address = tmpFrame->address;
        _clspReceivedFrame.messageCode = tmpFrame->messageCode;
        _clspReceivedFrame.messageParam = tmpFrame->messageParam;
        //_clspReceivedFrame.sequenceNumber = tmpFrame->sequenceNumber;
        _clspReceivedFrame.optionalDataLength = tmpFrame->optionalDataLength;
        //_clspReceivedFrame.dataChecksum = tmpFrame->dataChecksum;
        //_clspReceivedFrame.headerChecksum = tmpFrame->headerChecksum;
        //_clspReceivedFrame.flags = tmpFrame->flags;
        for (int i = 0; i < tmpFrame->optionalDataLength; i++)
            _clspReceivedFrame.optionalData[i] = tmpFrame->optionalData[i];
        
        Delete((ClspFrame_t*)tmpFrame);
    }
    
    ClspFrame_t* responseFrame = null;
    byte* uid = null;
    
    if (_clspReceivedFrame.address == CLSP485_BCAST) 
    {
        bool equal = true;
        
        switch(_clspReceivedFrame.messageCode) 
        {
            case NODE_LOOKUP:    
                if (_clspReceivedFrame.optionalDataLength >= 3) 
                {
                    _clspResponseLatency = _clspReceivedFrame.optionalData[0];
#ifdef DEBUG
                    _clspResponseTimeout = _clspReceivedFrame.optionalData[1];
#endif
                    _maxUnpolledTime = 1000 * _clspReceivedFrame.optionalData[2];
                }    
                
                if (_clspReceivedFrame.optionalDataLength >= 4) 
                {
                    int32 mnls = _clspReceivedFrame.optionalData[3];
                    if (mnls != _maxNodeLookupSequence) 
                    {
                        byte* uid = IAP_ReadSerialNumber();
                        
                        if (uid != null) 
                        {
                            _uidChecksum = Crc8(uid, 0, UID_LENGTH);
                            
                            if (mnls < 0xFF) 
                            {
                                _uidChecksum = _uidChecksum % (mnls + 1);
                            }
                            
                            //_uidChecksum = 0x18;
                        }
                        
                        _maxNodeLookupSequence = mnls;
                    }
                }
                
                if (_clspLogicalAddress != 0 || // IF NEW NODE LOOKUP RECEIVED AND LOGICAL ADDRESS ALREADY ASSIGNED, IGNORE
                    _clspSuggestedAddress == 0 || // IF DIP SWITCH ADDRESS HAD NOT BEEN SET YET, IGNORE
                    _uidChecksum != _clspReceivedFrame.messageParam)  // IF THIS SQNUMBER IS NOT RELEVANT TO UID CHECKSUM, WAIT MORE
                                                                      // THEREFORE IGNORE THIS FRAME
                {
                    return;         
                }
                                
                uid = IAP_ReadSerialNumber();
                if (null != uid) 
                {
                    responseFrame = Clsp_InternalCreateFrame(CLSP485_MASTER, NODE_NOTIFIED, _clspSuggestedAddress, UID_LENGTH+2, false);
                    
                    CopyAll(uid,UID_LENGTH,responseFrame->optionalData,UID_LENGTH);
                    responseFrame->optionalData[UID_LENGTH] = _clspMasterProtocol;
                    responseFrame->optionalData[UID_LENGTH+1] = _uidChecksum;
                    
                    Clsp_EnqueueLLFrame(responseFrame);
                    
                    Clsp485_AppealForTransmission(_clspReceivedFrame.sequenceNumber);
                    
                    if (_clspNodeNotifiedTime < 0)
                        _clspNodeNotifiedTime = SysTick_GetTickCount();
                }

                return;
                
            case NODE_ASSIGNED:               
                uid = IAP_ReadSerialNumber();
                if (null == uid ||
                    _clspLogicalAddress != 0)
                    return;
                               
                equal = true;
                // COMPARE UID               
                for(int i = 0; i < UID_LENGTH; i++) 
                {
                    if (_clspReceivedFrame.optionalData[i] != uid[i]) 
                    {    
                        equal = false;
                        break;
                    }
                }
                
                if (!equal) 
                {
                    if (_clspReceivedFrame.optionalDataLength >= 18) {
                        int32 foreignUidChecksum = _clspReceivedFrame.optionalData[17];
                        
                        if (foreignUidChecksum == _uidChecksum)
                             // RANDOMIZE UID CHECKSUM
                            Clsp485_UidChecksumRandomize(); 
                    }
                    
                    // POSSIBLY NODE_ASSIGNMENT FOR DIFFERENT NODE
                    return;
                }

                //////////////////////////////////////////////////////////////////////////////////////////                
                // NODE HAS BEEN ASSIGNED AN ADDRESS BUT REMAINS OFFLINE UNTIL NOT SUCCESSFULY POLLED/////
#ifndef BOOTLOADER
                _clspInConflict = false;
#endif
                _clspLogicalAddress = _clspReceivedFrame.messageParam;
                
                _clspNodeNotifiedTime = -1;
                //////////////////////////////////////////////////////////////////////////////////////////                
               
                // continue with responding of an usual response
                return;
                //break;
            case NODE_LOOKUP_CONFLICT:
                if (_clspReceivedFrame.messageParam == _uidChecksum)
                {
                    Clsp485_UidChecksumRandomize();
                }
                return;
#ifndef BOOTLOADER                
            case NODE_ADDRESS_CONFLICT:
                if (!_clspIsAssigned &&
                    _clspSuggestedAddress == _clspReceivedFrame.messageParam) 
                {
                    _clspNodeNotifiedTime = -1;
                    if (!_clspInConflict) 
                    {
                        _clspInConflict = true;
                        
                        FireNodeAddressConflicted(_clspSuggestedAddress);
                        

                    }
                }

                return;
#endif                
            default:
                // if it's broadcast, and not recognized command, do not respond further
                return;        
        }
    }
    
    // these two states are handled here :
    // when MASTER sends RESEND_REQUIRED after invalid cheksum from this node
    // OR Master did not received previous response from the node and therefore retransmission will take place
       
    if(_clspReceivedFrame.messageCode == RESEND_REQUIRED)
    {
        // if resend is required , executing OnFrameReceived has no meaning, as the retransmittion will take place
        Clsp485_AppealForTransmission(_clspReceivedFrame.sequenceNumber);
        return;
    }
    
    bool resendRequired = false;
    if (_clspReceivedFrame.sequenceNumber == _clspPreviousFrameSQ)
    {
        // this means, that previous response from this node was most possibly not received, as the sequencenumber should increment itself
        // anytime a response has been delivered to master
        
        // executing of OnFrameReceived has no meaning either, as the retransmittion will take place
        //Clsp485_AppealForTransmission(_clspReceivedFrame.sequenceNumber);
        resendRequired = true;
        //return;
    }
    
    // this branch means, that NEITHER retransmission requested by master, NEITHER retransmission assumed by slave is taking place
    // therefore _clspFrameForPossibleRetry can be destroyed    
    if (!resendRequired && _clspFrameForPossibleRetry != null)
    {
        Delete((ClspFrame_t*)_clspFrameForPossibleRetry);
        _clspFrameForPossibleRetry = null;
    } 
    
    
    // FOR UNICAST POLLING/REQUESTS ONLY     
    if (_clspReceivedFrame.messageCode == NODE_ASSIGNMENT_CONFIRMED && !_clspIsAssigned) 
    {
        _clspIsAssigned = true;   
        
#ifdef DEBUG
            _clspNodeAssignmentCount++;
#endif
        
        // posponed event on OnNodeAssigned until master did not declared the online state of this node
        FireNodeAssignmentChanged(_clspLogicalAddress, true);
    }    
        
    // Record actual Sequence number for next request comparation
    _clspPreviousFrameSQ = _clspReceivedFrame.sequenceNumber;
#ifdef DEBUG
    _clspPreviousFrameMC = _clspReceivedFrame.messageCode;
#endif
    
    
    // 1. appeal for transmission for unicast messages should be invoked even if NO response frame
    //    has been generated by the OnFrameReceived event/handler
    // 2. the appeal should be planned before any OnFrameReceived execution, as they can take unexpected time
    //    so _lastMasterFrameReceivedTime is relevant only here
    Clsp485_AppealForTransmission(_clspReceivedFrame.sequenceNumber);
    
   
    bool isTopPriority = false;
    responseFrame = null;
    if (_clspIsAssigned &&
        ((_clspReceivedFrame.messageCode >= PROTOCOL_FIRST_ID && _clspReceivedFrame.messageCode <= PROTOCOL_LAST_ID)
        || _clspReceivedFrame.messageCode == NODE_ASSIGNMENT_CONFIRMED)) 
    {
        // IF IT'S A LINK LAYER MESSAGE CODE, DO NOT ENTER CALL _eventOnFrameReceived HOOK
        // IF IT'S A LINK LAYER, NO ENQUEUE WILL BE MADE - E.G. FOR RESEND REQUIRED
        
        ProtocolId_t proto = PROTO_INVALID;
                
        if (_clspReceivedFrame.messageCode >= PROTOCOL_FIRST_ID && _clspReceivedFrame.messageCode <= PROTOCOL_LAST_ID)
            proto = (ProtocolId_t)((int)_clspReceivedFrame.messageCode - (int)PROTOCOL_FIRST_ID);
        
        FireOnFrameReceived((ClspFrame_t*)&_clspReceivedFrame, proto, isTopPriority);
    }    
    
    /// JUST A TEST
    /// if ((SysTick_GetTickCount() % 10) == 2)
    ///    SysTick_Delay(_clspResponseTimeout + 3);
    
}

#ifndef BOOTLOADER
void InvokeNodeReleasedEvent() 
{
    /*for(int i=0;i<MAX_CLSP_EVENT_HANDLERS;i++) 
    {
        if (_eventOnNodeAssignmentChanged[i] != null)
            (_eventOnNodeAssignmentChanged[i])(_clspSuggestedAddress,false);
    }*/
    
    FireNodeAssignmentChanged(_clspSuggestedAddress, false);  
}
#endif

/***************************************************************************
 *
 ***************************************************************************/
void Clsp485_ForceNodeRelease() 
{
    _clspLogicalAddress = 0;
    _clspIsAssigned = false;
#ifndef BOOTLOADER
    _clspInConflict = false;
#endif
    _clspAppealForTransmissionSQNumber = -1;
    _clspPreviousFrameSQ = -1;
#ifdef DEBUG
    _clspPreviousFrameMC = -1;
#endif
    
    if (null != _clspFrameForPossibleRetry) 
    {
        Delete((void*)_clspFrameForPossibleRetry);
        _clspFrameForPossibleRetry = null;
    }
    
    Clsp_ClearQueues();
    
    //LeaveCriticalSection();

#ifndef BOOTLOADER
    InvokeNodeReleasedEvent();
#endif
}

/***************************************************************************
 *
 ***************************************************************************/
void Clsp485_ReceiveTask() { 
#ifdef DEBUG
    //_clspLogicalAddress = 7;
    //_clspSuggestedAddress = 7;
#endif
    
#ifdef BOOTLOADER
    if (_clspForceAddressAssignment > 0 &&
        _forcedAddressValue > CLSP485_MASTER && _forcedAddressValue < CLSP485_BCAST)
    {
        _clspForceAddressAssignment = 0; // implicit value, if anything goes wrong, 
                                     // so next call of ReceiveTask would not try to 
                                     // obtain forced logical address
        
        byte* uid = IAP_ReadSerialNumber();
        if (null != uid) 
        {
            ClspFrame_t* notifiedFrame = Clsp_InternalCreateFrame(_clspLogicalAddress, NODE_NOTIFIED, _clspSuggestedAddress, UID_LENGTH + 2, false);
                
            if (null != notifiedFrame) 
            {
                _uidChecksum = Crc8(uid, 0, UID_LENGTH);
                
                CopyAll(uid, UID_LENGTH, notifiedFrame->optionalData, UID_LENGTH + 2);               
                notifiedFrame->optionalData[UID_LENGTH] = _clspMasterProtocol;
                notifiedFrame->optionalData[UID_LENGTH + 1] = _uidChecksum;
                
                if (Clsp_EnqueueLLFrame(notifiedFrame)) 
                {
                    _clspSuggestedAddress = _forcedAddressValue;    /* to block the change after DIP switch is read */
                    _clspLogicalAddress = _forcedAddressValue;
                    _clspForceAddressAssignment = -1;
                }
                
                // NO APPEAL FOR TRANSMISSION HERE !!!!!!
                // IT's NOT INVOKED BY POLLING MECHANISM, THEREFORE THE TIMING OF THE APPEAL WOULD NOT BE CORRECT
            }
        }
    }  
    // SCANNING OF THE SUGGESTED ADDRESS BY DIP SWITCH
    else 
#endif
    {    
        if (_inputChanged & SI_ADDRESS_BIT)
        {
            uint32 suggestedAddress = _specialInputState.address;
            _inputChanged &= ~SI_ADDRESS_BIT;
            
            if (suggestedAddress != _clspSuggestedAddress)
            {
                _clspSuggestedAddress = suggestedAddress;           
                
                if (_clspLogicalAddress != 0)
                {                                                      
                    Clsp485_ForceNodeRelease();            
                }
#ifndef BOOTLOADER                
                else 
                {
                    // this is branch where Node was assigned, and was switched from non-zero to zero DIP/Suggested address
                    _clspInConflict = false;
                    

                    InvokeNodeReleasedEvent();
                }
#endif                
                
            }
        }           
        
        if (_maxUnpolledTime > 0 &&
            _clspLastFrameReceivedTimestamp >= 0 &&
            _clspLogicalAddress != 0 &&
            SysTick_GetElapsedTime(_clspLastFrameReceivedTimestamp) > _maxUnpolledTime)
        {
            // IF NO MASTER IS POLLING THIS NODE, REVERT BACK TO NOT ASSIGNED STATE
            Clsp485_ForceNodeRelease();
        }          //*/
    }
    
    if (_clspLogicalAddress == 0 &&
        _clspNodeNotifiedTime >= 0 &&
          SysTick_GetElapsedTime(_clspNodeNotifiedTime) >= 1000) 
    {
        Clsp485_UidChecksumRandomize();
    }
        
    /*switch(_clspReadingPhase) 
    {
        case CLRP_WaitForPreamble:*/
            UART_Receive(COMM_PORT, 1, Clsp485_PHDParsing);
            /*break;
        case WaitForHeader:
            UART_Receive(COMM_PORT, 1, Clsp485_PHDParsing);
            break;
        case WaitForData:
            UART_Receive(COMM_PORT, 1, Clsp485_PHDParsing);
            break;
        default:
            UART_Receive(COMM_PORT,1,Clsp485_PHDParsing);
            break;
    }*/
}

/***************************************************************************
 *
 ***************************************************************************/
void Clsp485_TransmitTask() 
{   
    if (_clspAppealForTransmissionSQNumber < 0 ||                                       // if no master frame had been received after last response
        SysTick_GetElapsedTime(_clspLastFrameReceivedTimestamp) < _clspResponseLatency) // the tx/response latency is not fullfiled yet
        return;
    
    
    /*if (SysTick_GetElapsedTime(_clspLastFrameReceivedTimestamp) > _clspResponseTimeout) {
        asm("NOP");
    }*/
    
#ifdef DEBUG
    // THIS PART IS HIGHLY EXPERIMENTAL !!!
    
    int32 actualTime = SysTick_GetTickCount();
    
    // check, if appeal is not out of date
    // check only if the actual time is not equal to last request time
    if (_clspLastFrameReceivedTimestamp != actualTime &&
        _clspLastReceptionTime >= 0) {
        if (_clspLastFrameReceivedTimestamp < actualTime) {
            if (_clspLastReceptionTime > _clspLastFrameReceivedTimestamp && _clspLastReceptionTime <=actualTime)
            {
                _clspAppealForTransmissionSQNumber = -1; 
                // this means, that between the appeal was made and actual TransmitTask was called
                // some RELEVANT request has been received
                return;
            }
        }
        else 
        {
            if ((_clspLastReceptionTime > _clspLastFrameReceivedTimestamp) ||
                (_clspLastReceptionTime >= 0 && _clspLastReceptionTime <= actualTime)) 
            {
                _clspAppealForTransmissionSQNumber = -1; 
                // this means, that between the appeal was made and actual TransmitTask was called
                // some RELEVANT request has been received
                return;
            }
        }
    }
    // THIS PART IS HIGHLY EXPERIMENTAL !!!
#endif
   
    
    ClspFrame_t* responseFrame = Clsp_PopLinkLayerQueue();
       
    if (responseFrame != null) 
    {
        // All most-top priority link-layer frames for transmission are handled here
        // e.g. RESEND_REQUIRED, NODE_NOTIFIED
       
        if (responseFrame->messageCode == RESEND_REQUIRED && !_clspIsAssigned) 
        {
            Delete(responseFrame);
            responseFrame = null;
            // RESEND REQUIRED HAS NO MEANING IF THE NODE LOST LOGICAL ADDRESS
            _clspAppealForTransmissionSQNumber = -1;
            return;
        }
               
       
        Clsp485_ComposeByFrameAndSend(responseFrame);
        
        // DESTROY Link layer frames right after transmission, as retry has no meaning for them
        Delete(responseFrame);
        
        responseFrame = null;
    }
    else 
    {
        // Application frames
        if (_clspFrameForPossibleRetry != null) 
        {    
            
            Clsp485_ComposeByFrameAndSend((ClspFrame_t*)_clspFrameForPossibleRetry);
            
            // do not null the _clspFrameForPossibleRetry as it can be retried again
        }
        else 
        {
            if (_clspIsAssigned)
                responseFrame = Clsp_PopQueue();
                            
            if (responseFrame == null) 
            {
                // if nothing in the queue, respond will ALIVE frame
                
                byte helperBuffer[sizeof(ClspFrame_t)-CLSP_MAX_OPTIONAL_DATA_LENGTH];
                ClspFrame_t* aliveFrame = (ClspFrame_t*)helperBuffer;
                
                aliveFrame->address = _clspLogicalAddress;
                aliveFrame->messageCode = ALIVE;
                aliveFrame->messageParam = 0;
                //aliveFrame->sequenceNumber = _clspAppealForTransmissionSQNumber; // no need to do it here, done later
                aliveFrame->optionalDataLength = 0;
                
                Clsp485_ComposeByFrameAndSend(aliveFrame); //Clsp485_ComposeResponse(_clspLogicalAddress, ALIVE, 0x0, _clspAppealForTransmissionSQNumber, 0, 0, null);
            }
            else 
            {               
                _clspFrameForPossibleRetry = responseFrame;
                
                Clsp485_ComposeByFrameAndSend(responseFrame);
                
                // DO NOT destroy frame here, it'll be destroy, when relevant NON-RESEND-REQUIRED will be received
                // it'll be received by the _clspFrameForPossibleRetry pointer
                
                ////Delete(responseFrame);
            }
        }
    }
    
    //SendResponse(_clspAppealForTransmissionSQNumber);
    
    _clspAppealForTransmissionSQNumber = -1;
}

/***************************************************************************
 *
 ***************************************************************************/
void Clsp485_Init(ProtocolId_t masterProtocol, byte* key, byte* iv) 
{
     UART_Init(COMM_PORT, CLSP485_DEFAULT_SPEED);
     
     if (masterProtocol <= PROTO_MAX_ID)
         _clspMasterProtocol = masterProtocol;
     else
         _clspMasterProtocol = PROTO_MAX_ID;
     
     _aesKey = key;
     _aesIV = iv;
     
     //_clspReceivedFrame.optionalData = (byte*)&_clsp485masterFrameOptionalData;
     
     Tasks_Register(Clsp485_ReceiveTask,8); // the priority for receive must be little bit higher, so every ReceiveTask call is done before transmit task
     Tasks_Register(Clsp485_TransmitTask,9); 
}



#pragma diag_default=Pa082