#include "CrQueue.h"
#include "System\baseTypes.h"
#include "CrProtocol.h"
#include "System\LPC122x.h"
#include <stdlib.h>

#define CR_MAX_REQUEST_COUNT 10    // at least 8 for 8 line commands of menu and some more at the time

#include "System\dmHandler.h"


typedef CRMessage_t* PCRMessage_t;

volatile PCRMessage_t _crRequests[CR_MAX_COUNT][CR_MAX_REQUEST_COUNT];

volatile uint32 _crActualSizes[CR_MAX_COUNT] = { 0, 0 };

/***************************************************************************
 *
 ***************************************************************************/
uint32 CrQueue_Count(uint32 crAddress) {   
    if (crAddress < 1 || crAddress > CR_MAX_COUNT)
        return 0;
    else
        return _crActualSizes[crAddress-1];
}

/***************************************************************************
 *
 ***************************************************************************/
void CrQueue_Clear(uint32 crAddress) 
{   
    if (crAddress < 1 || crAddress > CR_MAX_COUNT)
        return;
    
    uint32* actualSize = (uint32*)&(_crActualSizes[crAddress-1]);
    PCRMessage_t* queue = (PCRMessage_t*)&(_crRequests[crAddress-1]);
    
    for(int i=0;i<*actualSize;i++) {
        if (null != queue[i]) 
        {
            Delete(queue[i]);
            queue[i] = null;
        }
    }
    
    *actualSize = 0;  
}

#ifdef DEBUG
uint32 _crMaxQueueCount = 0;
#endif

/***************************************************************************
 *
 ***************************************************************************/
bool CrQueue_Enqueue(uint32 crAddress, CRMessage_t* message,bool onTop,bool shiftIfFull)  {
    if (null == message)
        return false;
    
    if (crAddress < 1 || crAddress > CR_MAX_COUNT) 
    {
        Delete(message);
        return false;
    }
    
    PCRMessage_t* queue = (PCRMessage_t*)&(_crRequests[crAddress-1]);
    uint32* actualSize = (uint32*)&(_crActualSizes[crAddress-1]);
       
    if (onTop) {
        if (*actualSize < CR_MAX_REQUEST_COUNT) 
        {
            *actualSize += 1;
            
        }
        else
        {
            if (!shiftIfFull) 
            {
                // message is NOT null at this moment
                Delete(message);
                return false;
            }
            else
                Delete(queue[*actualSize-1]);
        }
        
        // shifting bytes
        for(int i=*actualSize-1;i>=1;i--) {
            queue[i] = queue[i-1];
        }
              
        queue[0] = message;      
    }
    else {
        if (*actualSize < CR_MAX_REQUEST_COUNT) {
            queue[*actualSize] = message;  
            *actualSize += 1;
        }
        else {
            if (shiftIfFull) {
                // position 0 will be cleared
                // remember deallocate
                if (*actualSize > 0)
                {
                    Delete(queue[0]);
                }
                
                for(int32 i=0;i<*actualSize-1;i++)
                    queue[i] = queue[i+1];
            
                queue[*actualSize-1] = message;
            }
            else {
                // message is NOT null at this moment
                Delete(message);
                // means queue is full
                return false;
            }
        }
    }
    
#ifdef DEBUG
    
    if (*actualSize > _crMaxQueueCount)
            _crMaxQueueCount = *actualSize;
#endif

 
    return true;
}

/***************************************************************************
 *
 ***************************************************************************/
CRMessage_t* CrQueue_Pop(uint32 crAddress) 
{
    if (crAddress < 1 || crAddress > CR_MAX_COUNT)
        return null;
    
    PCRMessage_t* queue=(PCRMessage_t*)&(_crRequests[crAddress-1]);  
    uint32* actualSize = (uint32*)&(_crActualSizes[crAddress-1]);
    
    if (*actualSize == 0) {
        return null;
    }
    
    CRMessage_t* crMessage = queue[0];
    
    if (*actualSize > 1) {
        for(int i=0;i<*actualSize-1;i++) {
            queue[i] = queue[i+1];
        }
        *actualSize -= 1;
        queue[*actualSize] = null;
    }
    else {
        queue[0] = null;
        *actualSize = 0;
    }
      
    return crMessage;
};

/***************************************************************************
 *
 ***************************************************************************/
CRMessage_t* CrQueue_AllocRequest(uint32 odLength) 
{   
    if (odLength > CR_MAX_OPTIONAL_DATA_LENGTH)
        odLength = CR_MAX_OPTIONAL_DATA_LENGTH;
    
    // DO NOT count sizeof of whole CRMessage_t, because the typedef preallocates the optionalData to CR_MAX_OPTIONAL_DATA_LENGTH
    uint32 structSize = sizeof(CRMessage_t) + odLength;

    // after this operation, the buffer should contain zeroes    
    CRMessage_t* request = (CRMessage_t*) New(structSize);
                
    if (request == null)      
        // out of memory
        return null;

    request->optionalDataLength = odLength;
       
    //_allocCount+= sizeof(CRMessage_t)+odLength; 
   
    return request;
}



