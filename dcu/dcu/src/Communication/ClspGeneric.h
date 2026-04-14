#pragma once

#include "System\baseTypes.h"
#include "Communication\ClspLon\Driver\LdvSci.h"

#define CLSP_MAX_FRAME_LENGTH                               89
#define CLSP_MAX_OPTIONAL_DATA_LENGTH                       80
#define CLSP_MAX_STACKABLE_SIZE                             64 // this is just a quick fix for problems with distorted stacked frames
#define CLSP_MAX_ENCDATA_LENGTH                             78
#define CLSP_PREAMBLE_LENGTH                                2
#define CLSP_HEADER_LENGTH                                  7

#define CLSP_FRAME_CHECKSUM_VALID       0x80
#define CLSP_FRAME_CHECKSUM_INVALID     0x7F
#define CLSP_FRAME_STACKABLE            0x40
#define CLSP_FRAME_ADDRESS_TYPE         (3 << 0)    // 2 bits for address types: broadcast, unicast, etc... 
#define CLSP_FRAME_ADDRESS_SET          (1 << 2)    // marks that the address has been set, if not then it will be set just prior sending

#define CLSP_FRAME_PREFIX_LENGTH        CLSP_PREAMBLE_LENGTH + CLSP_HEADER_LENGTH

typedef enum MessageCode_e {
    POLLING = 0x00,
    ALIVE = 0x01,
    RESEND_REQUIRED = 0x02,
    NODE_LOOKUP = 0x03,
    NODE_NOTIFIED = 0x04,
    NODE_ASSIGNED = 0x05,
    NODE_ASSIGNMENT_CONFIRMED = 0x06,
    NODE_ADDRESS_CONFLICT = 0x07,
    NODE_LOOKUP_CONFLICT = 0x08,
    
    STACKED_FRAME = 0x1F,
    
    PROTOCOL_FIRST_ID = 0x20,
    PROTOCOL_LAST_ID = 0xDF

} MessageCode_t;

typedef enum ProtocolId_e{
    PROTO_UPLOADER = 0,
    PROTO_ACCESS = 1,
    PROTO_CARDREADER = 2,
    
    PROTO_TESTING = 0x10,
        
    PROTO_MAX_ID = 0xBF,
    
    PROTO_INVALID = 0xFF,
        
    PROTO_FILLING = 0xFFFFFFFF // make the enum 32bit
} ProtocolId_t;

typedef struct ClspFrame {
    // the order of fields in the structure follows the CLSP485 protocol field order
    byte address;
    byte messageCode;
    byte messageParam;
    byte sequenceNumber;
    byte optionalDataLength;
    //byte dataChecksum;
    //byte headerChecksum;
    // end of header-only fields
    
    // contains information, whether checksum valid, or whether is stackable
    byte flags;   

    byte optionalData[CLSP_MAX_OPTIONAL_DATA_LENGTH];   // it's MANDATORY that this field will be aligned to N*4B in the memory
} ClspFrame_t;

extern volatile uint32 _clspSuggestedAddress;
extern volatile uint32 _clspLogicalAddress;
extern volatile bool _clspIsAssigned;

extern volatile ProtocolId_t _clspMasterProtocol;

// used for retries
extern volatile ClspFrame_t* _clspFrameForPossibleRetry;

#ifndef BOOTLOADER
extern volatile bool _clspInConflict;
#else
// if set and suggestedAddress is set, the logicalAddress is pre-set for the logicalAddress directly
extern volatile int32 _clspForceAddressAssignment;
extern volatile uint32 _forcedAddressValue;
#endif


typedef void(*DOnNodeAssignmentChanged)(uint32 suggestedAddress, bool isAssigned);

typedef ClspFrame_t*(*DOnFrameReceived)(ClspFrame_t* frame, ProtocolId_t protocol, bool* isTopPriotity);

/**
 * @summary Binds a function to an event of Node assigned
 * @param onNodeAssigned - NON NULL functional pointer
 */
DECLARE_BIND_EVENT(Clsp_BindOnNodeAssignmentChanged, DOnNodeAssignmentChanged)

/**
 * @summary Binds a function to an event of Frame received 
 * @param onFrameReceived - NON NULL functional pointer
 * @param onFrameReceived(masterFrame) - frame received from the master node
 * @param OUT onFrameReceived(responseFrame) - response frame to be either enqueued or directly transmitted to master
 * @param OUT onFrameReceived(isTopPriority) - indicates, whether the responseFrame will enqueued(FALSE) or directly transmitted(TRUE)
 */
DECLARE_BIND_EVENT(Clsp_BindOnFrameReceived, DOnFrameReceived)

#ifndef BOOTLOADER

typedef void(*DOnNodeAddressConflicted)(uint32 suggestedAddress);

DECLARE_BIND_EVENT(Clsp_BindOnNodeAddressConflicted, DOnNodeAddressConflicted)

#endif



bool Clsp_EnqueueLLFrame(ClspFrame_t* frame);
bool Clsp_EnqueueFrame(ClspFrame_t* frame,bool onTop);

void Clsp_ClearQueues();

ClspFrame_t* Clsp_PopLinkLayerQueue();
ClspFrame_t* Clsp_PopQueue();

ClspFrame_t* Clsp_CreateFrame(ProtocolId_t protocol, uint32 command, uint32 optionalDataLength, bool stackable);
ClspFrame_t* Clsp_InternalCreateFrame(uint32 address, uint32 messageCode, uint32 messageParam, uint32 optionalDataLength, bool stackable);

#ifndef BOOTLOADER

uint32 Clsp_GetLogicalAddress();
uint32 Clsp_GetSuggestedAddress();

bool Clsp_IsAssigned();
bool Clsp_IsInConflict();

#else

void Clsp_SetForceLogicalAddress();

#endif  // BOOTLOADER


void FireNodeAddressConflicted(uint32 address);
void FireNodeAssignmentChanged(uint32 address, bool state);
void FireOnFrameReceived(ClspFrame_t* receivedFrame, ProtocolId_t protocolId, bool isTopPriority);

#ifdef DEBUG
uint32 Clsp_GetTxCount();
#endif