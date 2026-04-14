#include "System\Tasks.h"
#include "System\Timer\sysTick.h"
#include "System\dmHandler.h"
#include "System\systemInfo.h"
#include "ClspAPI.h"
#include "Crypto\hashes.h"
#include "IO\Inputs.h"

typedef enum
{
    DU_Idle,
    DU_UpdateDomain,
    DU_ConfirmDomain,
    DU_DomainConfirmed
        
} ClspAPI_DomainUpdate_e;

typedef struct
{
    ClspAPI_DomainUpdate_e state;
    
    uint8 domainIndex;              // domain id
    LonDomain domain;               // domain config structure
    
    bool lastResult;                // indicates whether last update was confirmed
} ClspAPI_DomainUpdate_t;


void UpdateDomainTask();
ClspFrame_t* ClspLon_InternalCreateFrame(LonAPI_AddressType_e addressType, uint32 messageCode, uint32 messageParam, uint32 optionalDataLength);

volatile ClspAPI_DomainUpdate_t _domainContext;

LonApiError ClspLon_UpdateDomain(const LonCtrl_Context_t* lonContext)
{
    _domainContext.domainIndex = lonContext->domainIdxToSet;
    
    memcpy((void*)(_domainContext.domain.Id), lonContext->requestedDomainId, lonContext->requestedDomainIdLength);
    
    _domainContext.domain.InvalidIdLength = lonContext->requestedDomainIdLength & LON_DOMAIN_ID_LENGTH_MASK;
    _domainContext.domain.Subnet = lonContext->requestedSubnet;
    _domainContext.domain.NodeClone = (1 << LON_DOMAIN_NONCLONE_SHIFT) | lonContext->requestedNodeId;
    
    memset((void*)(_domainContext.domain.Key), 0xff, LON_AUTHENTICATION_KEY_LENGTH);
    
    return LonUpdateDomainConfig(_domainContext.domainIndex, (const LonDomain*)&(_domainContext.domain));
}




LonApiError ClspLon_BroadcastUniqueId(uint32 suggestedAddress, ProtocolId_t protocolId)
{
    ClspFrame_t* frame = ClspLon_InternalCreateFrame(LonAPI_Broadcast, NODE_NOTIFIED, 0, LON_UNIQUE_ID_LENGTH + 1);
    
    if (frame != null)
    {
        LonUniqueId uniqueId;
        LonGetUniqueId(&uniqueId);

        frame->messageParam = suggestedAddress;
        frame->optionalData[0] = protocolId;
        memcpy((void*)(&(frame->optionalData[1])), uniqueId, LON_UNIQUE_ID_LENGTH);
        
        //Clsp_EnqueueLLFrame(frame);
        LonApiError result = ClspLon_ComposeByFrameAndSend(frame);
        Delete(frame);
        
        return result;
    }
    else
    {
        // TODO maybe something better
        return LonApiTxBufIsFull;
    }
}

LonApiError ClspLon_SendAlive()
{
    ClspFrame_t* frame = ClspLon_InternalCreateFrame(LonAPI_Unicast, ALIVE, 0, 0);
    
    if (frame != null)
    {
//        Clsp_EnqueueLLFrame(frame);
//        return LonApiNoError;
        LonApiError result = ClspLon_ComposeByFrameAndSend(frame);
        Delete(frame);
        return result;
    }
    else
    {
        // TODO maybe something better
        return LonApiTxBufIsFull;
    }
}

LonApiError ClspLon_EnqueueAlive()
{
    ClspFrame_t* frame = ClspLon_InternalCreateFrame(LonAPI_Unicast, ALIVE, 0, 0);
    
    if (frame != null)
    {
        Clsp_EnqueueLLFrame(frame);
        return LonApiNoError;
    }
    else
    {
        // TODO maybe something better
        return LonApiTxBufIsFull;
    }
}



void ClspLon_CheckDIPChange(LonCtrl_Context_t* lonContext)
{
    // check DIP change
    if (_inputChanged & SI_ADDRESS_BIT) 
    {
        _inputChanged &= ~SI_ADDRESS_BIT;
        lonContext->suggestedAddress = _specialInputState.address;
        lonContext->configStatus = LonCS_SendAddressRequest;
        lonContext->online = false;
        
        _clspSuggestedAddress = _specialInputState.address;
        
        ClspLon_ForceNodeRelease(lonContext);
    }    
}

void ClspLon_CheckTamper()
{
    if (_inputChanged & SI_TAMPER_BIT)
    {
    }
}






ClspFrame_t* ClspLon_InternalCreateFrame(LonAPI_AddressType_e addressType, uint32 messageCode, uint32 messageParam, uint32 optionalDataLength)
{
    if (optionalDataLength > CLSP_MAX_OPTIONAL_DATA_LENGTH)
        optionalDataLength = CLSP_MAX_OPTIONAL_DATA_LENGTH;
    
    uint32 structSize = sizeof(ClspFrame_t) - CLSP_MAX_OPTIONAL_DATA_LENGTH + optionalDataLength;
   
    // the buffer SHOULD be zeroed after this operation
    ClspFrame_t* ret =  (ClspFrame_t*)New(structSize); 
    
    if (ret != null) 
    { 
        ret->flags |= addressType;
        ret->flags |= CLSP_FRAME_ADDRESS_SET;
        ret->messageCode = messageCode;
        ret->messageParam = messageParam;
        //ret->sequenceNumber = 0;
        ret->optionalDataLength = optionalDataLength;
        
        //ret->flags = CLSP_FRAME_CHECKSUM_VALID;
        //if (stackable)
        //    ret->flags |= CLSP_FRAME_STACKABLE;
    }
    
    return ret;
}

LonApiError ClspLon_ComposeByFrameAndSend(ClspFrame_t* frame)
{
    if (frame == null)
        return LonApiInvalidParameter;
    
    byte rawResponse[CLSP_MAX_FRAME_LENGTH];
    uint32 rawResponseLength;
    
    rawResponse[0] = frame->messageParam;
    rawResponse[1] = frame->flags;
    
    if (null == frame->optionalData && frame->optionalDataLength > 0)
        frame->optionalDataLength = 0;
          
    if (frame->optionalDataLength > CLSP_MAX_OPTIONAL_DATA_LENGTH)
        frame->optionalDataLength = CLSP_MAX_OPTIONAL_DATA_LENGTH;
        
    if (frame->optionalDataLength > 0 && frame->optionalData != null) 
    {
        for (int i = 0; i < frame->optionalDataLength; i++)
            rawResponse[i + LONCTRL_LON_HEADER_LENGTH] = frame->optionalData[i];                
    }
    
    rawResponseLength = LONCTRL_LON_HEADER_LENGTH + frame->optionalDataLength;
    
    LonSendSubnetNode dAddress;
 
    dAddress.Type = LonAddressSubnetNode;
    dAddress.RepeatRetry = 3 | (LonRpt192 << LON_SENDBCAST_REPEAT_TIMER_SHIFT);
    dAddress.RsvdTransmit = LonTx96;
    
    if (!(frame->flags & CLSP_FRAME_ADDRESS_SET))
        frame->flags |= LonAPI_Unicast;
    
    LonServiceType lonServiceType = LonServiceAcknowledged;
    
    switch (frame->flags & CLSP_FRAME_ADDRESS_TYPE)
    {
        case LonAPI_Broadcast:
            dAddress.Subnet = LONCTRL_CONFIG_SUBNET;
            dAddress.DomainNode = (LONCTRL_CONFIG_DOMAIN_ID << LON_SENDSN_DOMAIN_SHIFT) | 
                (LONCTR_MASTER_ADDRESS & LON_SENDSN_NODE_MASK);        
            break;
            
        case LonAPI_Unicast:
            if (frame->messageCode == ALIVE)
                lonServiceType = LonServiceUnacknowledged;
            
            dAddress.Subnet = LONCTRL_WORKING_SUBNET;
            dAddress.DomainNode = (LONCTRL_DEFAULT_DOMAIN_ID << LON_SENDSN_DOMAIN_SHIFT) | 
                (LONCTR_MASTER_ADDRESS & LON_SENDSN_NODE_MASK);
            break;
    }
    
    return LonSendMsg(LonMtIndexDcuMessage, false, lonServiceType, 
               false, (const LonSendAddress*)&dAddress, frame->messageCode, rawResponse, rawResponseLength);    
}

LonApiError ClspLon_CreateFrameFromData(uint32 code, const uint8* data, uint32 dataLength, ClspFrame_t* frame)
{
    frame->messageCode = code;
    frame->messageParam = data[0];
    frame->flags = data[1];
    
    frame->optionalDataLength = dataLength - LONCTRL_LON_HEADER_LENGTH;
    if (frame->optionalDataLength > CLSP_MAX_OPTIONAL_DATA_LENGTH)
        frame->optionalDataLength = CLSP_MAX_OPTIONAL_DATA_LENGTH;
    
    memcpy((void*)(frame->optionalData), &(data[LONCTRL_LON_HEADER_LENGTH]), frame->optionalDataLength);     
    
    return LonApiNoError;
}











void OnLonDomainConfigReceived(const LonDomain* const pDomain, const LonBool success)
{    
    if (success)
    {
        uint32 domainLength = pDomain->InvalidIdLength & LON_DOMAIN_ID_LENGTH_MASK;
        
#ifdef NO_CR_DEBUG        
        UART_DebugT("<< Domain Config Received\n");
        
        UART_Debug("\tDomain ID: ");
        UART_DebugArray((uint8*)(pDomain->Id), domainLength);
        UART_Debug("\n");
        
        UART_Debug("\tSubnet ID: ");
        UART_DebugHex(pDomain->Subnet);
        UART_Debug("\n");
        
        UART_Debug("\tNode ID: ");
        UART_DebugHex((uint8)(pDomain->NodeClone & LON_DOMAIN_NODE_MASK));
        UART_Debug("\n");
#endif
        
        if (_domainContext.state != DU_ConfirmDomain)
            return;
        
        // check if correct 
        bool isCorrect = true;
                    
        // check domain length
        if (domainLength != _domainContext.domain.InvalidIdLength)
            isCorrect = false;

        // check domain id bytes, also check if is still correct since the id length might be different
        if (isCorrect)
        {
            for (int i = 0; i < domainLength; i++)
            {
                if (_domainContext.domain.Id[i] != pDomain->Id[i])
                {
                    isCorrect = false;
                    break;
                }
            }
        }
        
        // check subnet
        if (_domainContext.domain.Subnet != pDomain->Subnet)
            isCorrect = false;
        
        // check node id
        if (_domainContext.domain.NodeClone != pDomain->NodeClone)
            isCorrect = false;   
        
        _domainContext.lastResult = isCorrect;
        _domainContext.state = DU_DomainConfirmed;
    }
}

void ClspLon_ForceNodeRelease(LonCtrl_Context_t* lonContext) 
{
    _clspIsAssigned = false;
    
    lonContext->assignedAddress = 0;
    lonContext->online = false;    
    
    lonContext->sendStatus = LonSS_Idle;
    if (null != lonContext->frameToSend) 
    {
        Delete((void*)(lonContext->frameToSend));
        lonContext->frameToSend = null;
    }
    
    Clsp_ClearQueues();

#ifndef BOOTLOADER
    SysInfo_SignalConnectionDown(true);
    FireNodeAssignmentChanged(lonContext->suggestedAddress, false);
#endif
}

volatile uint8 _debugBuffer[10][80];
volatile uint32 _debugIndex = 0;

void ClspLon_DebugTask()
{
    
}