#include "clspGeneric.h"
#include "System\dmHandler.h"
#include <stdlib.h>
#include "System\constants.h"

#ifndef BOOTLOADER
#define MAX_QUEUE_LENGTH        32
#define MAX_LLQUEUE_LENGTH      8
#else
#define MAX_QUEUE_LENGTH        16
#define MAX_LLQUEUE_LENGTH      4
#endif


// suggested logical address defined by DIP switch PINs - relevant is considered address <1, 31>
volatile uint32 _clspSuggestedAddress = 0;
// logical address assigned by master - after successful assignment is equal to _clspSuggestedAddress
volatile uint32 _clspLogicalAddress = 0;
volatile bool _clspIsAssigned = false;

volatile ProtocolId_t _clspMasterProtocol;

#ifndef BOOTLOADER
volatile bool _clspInConflict = false;
#else
// if set and suggestedAddress is set, the logicalAddress is pre-set for the logicalAddress directly
volatile int32 _clspForceAddressAssignment = 0;
volatile uint32 _forcedAddressValue = 0x00;
#endif

// used for retries
volatile ClspFrame_t* _clspFrameForPossibleRetry = null;

// queue for application frames for transmission
volatile ClspFrame_t* _clspTxQueue[MAX_QUEUE_LENGTH];
volatile uint32 _clspTxCount = 0;

// queue for link layer only frames for transmission
volatile ClspFrame_t* _clspTxLLQueue[MAX_LLQUEUE_LENGTH];
volatile uint32 _clspTxLLCount = 0;




/************************************************************************************************************
 * EVENTS
 ************************************************************************************************************/
#define MAX_CLSP_EVENT_HANDLERS     4

DEFINE_EVENT(DOnNodeAssignmentChanged, _eventOnNodeAssignmentChanged)
DEFINE_EVENT(DOnFrameReceived, _eventOnFrameReceived)

#ifndef BOOTLOADER
DEFINE_EVENT(DOnNodeAddressConflicted, _eventOnNodeAddressConflicted)
#endif

/***************************************************************************
 * @summary Binds function to an event of NODE ASSIGNED
 *          Occurs every time the node has 0-NotAssigned logical address and received
 *          address assignment from the master
 * @param onNodeAssigned - functional pointer
 ***************************************************************************/
DEFINE_BIND_EVENT(Clsp_BindOnNodeAssignmentChanged, DOnNodeAssignmentChanged, _eventOnNodeAssignmentChanged)
#ifndef BOOTLOADER
DEFINE_BIND_EVENT(Clsp_BindOnNodeAddressConflicted, DOnNodeAddressConflicted, _eventOnNodeAddressConflicted)
#endif

DEFINE_BIND_EVENT(Clsp_BindOnFrameReceived, DOnFrameReceived, _eventOnFrameReceived)



/************************************************************************************************************
 * FUNCTIONS
 ************************************************************************************************************/
#ifdef DEBUG
uint32 Clsp_GetTxCount() {
    return _clspTxCount;
}
#endif

bool Clsp_EnqueueLLFrame(ClspFrame_t* frame)
{
    bool ret = false;
    if (frame != null)
    {
        if (_clspTxLLCount >= MAX_LLQUEUE_LENGTH) 
        {
            Delete(frame);
        }
        else
        {
            _clspTxLLQueue[_clspTxLLCount++] = frame;
            //_clsp485TxLLCount++;
        }
    }
    return ret;
}

bool Clsp_EnqueueFrame(ClspFrame_t* frame,bool onTop) 
{
    if (frame == null)
        return false;
    
    if (!_clspIsAssigned || _clspTxCount >= MAX_QUEUE_LENGTH)
    {
        Delete(frame);
        return false;
    }
    
    if (onTop) {
         // shift the values
        for(int i = _clspTxCount - 1; i > 0; i--) 
        {
            _clspTxQueue[i] = _clspTxQueue[i - 1];
        }
        
        _clspTxQueue[0] = frame;
        _clspTxCount++;
    }
    else 
    {
        if (_clspTxCount == 0 && _clspFrameForPossibleRetry != null)
        {
            // this should eliminate situation, when retry is going to be transmitted
            // and the retried request has the same response as the retried response
            
            bool isSame = true;
            if (frame->address != _clspFrameForPossibleRetry->address ||
                frame->messageCode != _clspFrameForPossibleRetry->messageCode ||
                frame->messageParam != _clspFrameForPossibleRetry->messageParam || 
                frame->optionalDataLength != _clspFrameForPossibleRetry->optionalDataLength)
            {
                isSame = false;
            }
            else
            {
                for (int i = 0; i < frame->optionalDataLength; i++)
                {
                    if (frame->optionalData[i] != _clspFrameForPossibleRetry->optionalData[i])
                    {
                        isSame = false;
                        break;
                    }
                }
            }    
            
            if (isSame) 
            {
                Delete(frame); 
                return true;
            }
        }

#ifndef LON        
        // now try to use stacking if possible
        bool stacked = false;

        if (_clspTxCount > 0) 
        {
            ClspFrame_t* previousFrame = (ClspFrame_t*)_clspTxQueue[_clspTxCount - 1];
            
            if (null != previousFrame && previousFrame->address == frame->address)
            {
                if (previousFrame->messageCode != STACKED_FRAME && (previousFrame->flags & CLSP_FRAME_STACKABLE)) 
                {    
                    // +6 stands for messageCode,messageParam,optionalDataLength for both messages 
                    uint32 newLength = previousFrame->optionalDataLength + frame->optionalDataLength + 6;
                    
                    // don't allow stacking over max frame length
                    if (newLength <= CLSP_MAX_STACKABLE_SIZE) {
                    
                        ClspFrame_t* newFrame = Clsp_InternalCreateFrame(frame->address, STACKED_FRAME, 2, newLength, true); 
                        if (newFrame!= null) 
                        {
                            // copying first frame data
                            newFrame->optionalData[0] = previousFrame->messageCode;
                            newFrame->optionalData[1] = previousFrame->messageParam;
                            newFrame->optionalData[2] = previousFrame->optionalDataLength;
                            
                            if (previousFrame->optionalDataLength > 0)
                                CopySafe(previousFrame->optionalData,previousFrame->optionalDataLength, ((newFrame->optionalData) + 3), 
                                         newFrame->optionalDataLength - 3, previousFrame->optionalDataLength);
                            
                            newFrame->optionalData[3+previousFrame->optionalDataLength] = frame->messageCode;
                            newFrame->optionalData[3+previousFrame->optionalDataLength + 1] = frame->messageParam;
                            newFrame->optionalData[3+previousFrame->optionalDataLength + 2] = frame->optionalDataLength;     
                            
                            if (frame->optionalDataLength > 0)
                                CopySafe(frame->optionalData,frame->optionalDataLength,
                                         ((newFrame->optionalData) + 3 + previousFrame->optionalDataLength + 3),
                                         newFrame->optionalDataLength - 6 - previousFrame->optionalDataLength,
                                         frame->optionalDataLength);
                            
                            //former previousFrame is replaced in the queue and both new and previous frame destroyed
                            _clspTxQueue[_clspTxCount-1] = newFrame;
                            Delete(previousFrame);
                            Delete(frame);
                            
                            stacked = true;
                        }
                    }
                    
                }
                else 
                {
                    if (previousFrame->messageCode == STACKED_FRAME) 
                    {
                         // +3 stands for messageCode,messageParam,optionalDataLength for one added message
                        uint32 newLength = previousFrame->optionalDataLength + frame->optionalDataLength + 3;
                        
#ifdef DEBUG
                        if (frame->messageParam == 0x60)
                            __ASM("NOP");
#endif
                        
                        // don't allow stacking over max frame length
                        if (newLength <= CLSP_MAX_STACKABLE_SIZE) 
                        {
                            ClspFrame_t* newFrame = Clsp_InternalCreateFrame(frame->address, STACKED_FRAME, previousFrame->messageParam + 1, newLength, true); 
                            if (newFrame != null) 
                            {    
                                CopySafe(previousFrame->optionalData, previousFrame->optionalDataLength, newFrame->optionalData, 
                                         newFrame->optionalDataLength, previousFrame->optionalDataLength);
                                
                                newFrame->optionalData[previousFrame->optionalDataLength] = frame->messageCode;
                                newFrame->optionalData[previousFrame->optionalDataLength + 1] = frame->messageParam;
                                newFrame->optionalData[previousFrame->optionalDataLength + 2] = frame->optionalDataLength;     
                                
                                if (frame->optionalDataLength > 0)
                                    CopySafe(frame->optionalData,frame->optionalDataLength,
                                         ((newFrame->optionalData) + previousFrame->optionalDataLength + 3),
                                         newFrame->optionalDataLength - 3 - previousFrame->optionalDataLength ,
                                         frame->optionalDataLength);
                                
                                //former previousFrame is replaced in the queue and both new and previous frame destroyed
                                _clspTxQueue[_clspTxCount - 1] = newFrame;
                                Delete(previousFrame);
                                Delete(frame);
                                
                                stacked = true;
                                
#ifdef DEBUG
                                if (newFrame->messageParam == 0xB) {
                                	__ASM("NOP");
                                    newFrame->messageParam = 0xB;
                                }
#endif
                            }
                        }
                    }
                }
            }
        }
#endif // LON
        // if stacked, the former previousFrame is replaced in the queue and both actual and previous frame destroyed
#ifndef LON
        if (!stacked) 
#endif
        {
            _clspTxQueue[_clspTxCount] = frame;
            _clspTxCount++;
        }
    }
    
    return true;
}


/***************************************************************************
 *
 ***************************************************************************/
ClspFrame_t* Clsp_PopLinkLayerQueue() 
{
    if (_clspTxLLCount == 0)
        return null;
    else 
    {
        ClspFrame_t* ret = (ClspFrame_t*)_clspTxLLQueue[0];
    
        for(int32 i = 0; i < _clspTxLLCount - 1; i++)
        {
            _clspTxLLQueue[i] = _clspTxLLQueue[i + 1];
        }
        
        for(int32 i = _clspTxLLCount - 1; i < MAX_LLQUEUE_LENGTH; i++)
            _clspTxLLQueue[i] = null;
        
        _clspTxLLCount--;
        
        return ret;
    }
}

/***************************************************************************
 *
 ***************************************************************************/
ClspFrame_t* Clsp_PopQueue() 
{
    if (_clspTxCount == 0)
        return null;
    else 
    {
        ClspFrame_t* ret = (ClspFrame_t*)_clspTxQueue[0];
        
        for(int32 i = 0; i < _clspTxCount - 1; i++)
        {
            _clspTxQueue[i] = _clspTxQueue[i + 1];
        }
        
        for(int32 i = _clspTxCount - 1; i < MAX_QUEUE_LENGTH; i++)
            _clspTxQueue[i] = null;
        
        _clspTxCount--;
        
        return ret;
    }
}

void Clsp_ClearQueues()
{
    // clear application TX Queue
    for(int32 i = 0; i < _clspTxCount; i++) 
    {
        if (_clspTxQueue[i] != null) {
            Delete((ClspFrame_t*)_clspTxQueue[i]);
            _clspTxQueue[i] = null;    
        }
    }
    _clspTxCount = 0;
    
    // clear link layer TX queue
    for(int32 i = 0; i < _clspTxLLCount; i++) 
    {
        if (_clspTxLLQueue[i] != null) {
            Delete((ClspFrame_t*)_clspTxLLQueue[i]);
            _clspTxLLQueue[i] = null; 
        }
    }
    _clspTxLLCount = 0;
}

/***************************************************************************
 *
 ***************************************************************************/
ClspFrame_t* Clsp_InternalCreateFrame(uint32 address, uint32 messageCode, uint32 messageParam, uint32 optionalDataLength, bool stackable) 
{
    if (optionalDataLength > CLSP_MAX_OPTIONAL_DATA_LENGTH)
        optionalDataLength = CLSP_MAX_OPTIONAL_DATA_LENGTH;
    
    uint32 structSize = sizeof(ClspFrame_t) - CLSP_MAX_OPTIONAL_DATA_LENGTH + optionalDataLength;
   
    // the buffer SHOULD be zeroed after this operation
    ClspFrame_t* ret =  (ClspFrame_t*)New(structSize); 
    
/*#ifdef USE_MALLOC
         calloc(structSize,1);
        //memset(ret,0,sizeof(Clsp485Frame_t)+optionalDataLength);
#else
         New(structSize); 
#endif*/
    if (ret != null) 
    { 
        ret->address = address;
        ret->messageCode = messageCode;
        ret->messageParam = messageParam;
        ret->sequenceNumber = 0;
        ret->optionalDataLength = optionalDataLength;
        
        //ret->dataChecksum = 0;
        //ret->headerChecksum = 0;
        ret->flags = CLSP_FRAME_CHECKSUM_VALID;
        
        if (stackable)
            ret->flags |= CLSP_FRAME_STACKABLE;
    }
    // else out of memory
    
    return ret;
}

/***************************************************************************
 *
 ***************************************************************************/
ClspFrame_t* Clsp_CreateFrame(ProtocolId_t protocol, uint32 command, uint32 optionalDataLength, bool stackable) 
{
    ClspFrame_t* ret = null;
    if (_clspIsAssigned && command <= PROTO_MAX_ID) // command can be from 0x00 to 0xBF
    {
        ret = Clsp_InternalCreateFrame(_clspLogicalAddress, PROTOCOL_FIRST_ID + (uint32)protocol, command, optionalDataLength, stackable);
    }
#ifdef DEBUG
    else
    {
        if (command == 0x60)
        	__ASM("NOP");
        command *= command;
    }
#endif
    return ret;
}

#ifndef BOOTLOADER
void FireNodeAddressConflicted(uint32 address)
{
    BEGIN_CALL_EVENT(_eventOnNodeAddressConflicted)
        address
    END_CALL_EVENT
}
#endif

void FireNodeAssignmentChanged(uint32 address, bool state)
{
    BEGIN_CALL_EVENT(_eventOnNodeAssignmentChanged)
        address, state
    END_CALL_EVENT
}

void FireOnFrameReceived(ClspFrame_t* receivedFrame, ProtocolId_t protocolId, bool isTopPriority)
{
    ClspFrame_t* responseFrame = null;
    
    for (int i = 0; i < MAX_EVENT_HANDLERS; i++) 
    {
        if (_eventOnFrameReceived[i] != null) 
        {            
            responseFrame = (ClspFrame_t*)
                (_eventOnFrameReceived[i])(
                    receivedFrame,
                    protocolId,
                    &isTopPriority
                );               
            
            // Enqueue the response from OnFrameReceived, if any relevant
            if (responseFrame != null ) 
                Clsp_EnqueueFrame(responseFrame, isTopPriority); 
        }
    }
}

#ifdef BOOTLOADER
#ifndef LON
void Clsp485_SetForceLogicalAddress()
{
    _clspForceAddressAssignment = 1;
    uint32* forceAddr = (uint32*)FORCE_ADDRESS_ADDR;
    _forcedAddressValue = *forceAddr;
}
#endif
#else

uint32 Clsp_GetLogicalAddress() 
{
    return _clspLogicalAddress;
}
uint32 Clsp_GetSuggestedAddress() 
{
    return _clspSuggestedAddress;
}

bool Clsp_IsAssigned() 
{
    return _clspIsAssigned;
}

bool Clsp_IsInConflict() 
{
    return _clspInConflict;
}
#endif  // BOOTLOADER