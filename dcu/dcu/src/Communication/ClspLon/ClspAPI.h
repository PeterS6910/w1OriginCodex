#pragma once
#include "Communication\ClspLon\Driver\LdvSci.h"
#include "Communication\ClspGeneric.h"

#define LONCTRL_DEFAULT_DOMAIN_ID   0
#define LONCTRL_DEFAULT_DOMAIN      "A"
#define LONCTRL_DEFAULT_DOMAIN_LEN  1
#define LONCTRL_DEFAULT_SUBNET      255
#define LONCTRL_WORKING_SUBNET      1
#define LONCTRL_DEFAULT_NODEID      127

#define LONCTRL_CONFIG_DOMAIN_ID    1
#define LONCTRL_CONFIG_DOMAIN       "C"
#define LONCTRL_CONFIG_DOMAIN_LEN   1
#define LONCTRL_CONFIG_SUBNET       1
#define LONCTRL_CONFIG_NODEID       127

#define LONCTR_MASTER_ADDRESS       0

#define LONCTRL_ADDRESS_REQ_TIMEOUT         5000
#define LONCTRL_UNPOLLED_TIMEOUT            5000
#define LONCTRL_POLLING_DELAY_MULTIPLIER    5

#define LONCTRL_FRAME_RETRY_CNT             5

#define LONCTRL_LON_HEADER_LENGTH           2
#define LONCTRL_MSG_CONFIRM_TIMEOUT         1000


typedef enum
{
    LonCS_QueryStatus = 0,
    LonCS_WaitForStatus,
    
    //LonCS_GoUnconfigured,
    //LonCS_QueryDomainForDefault,
    LonCS_UpdateConfigToDefault,
    LonCS_UpdateActiveToDefault,
    
    LonCS_SendAddressRequest,
    LonCS_WaitForAddressChange,
    LonCS_WaitForAddress,
    LonCS_UpdateDomainToSet,
    //LonCS_QueryDomainForSet,
    LonCS_SendAlive,
    LonCS_WaitForConfirm,
    LonCS_GoConfigured,
    
    LonCS_Idle,
    LonCS_WaitForReply,
        
    LonCS_Done
} LonCtrl_ConfigStatus_e;

typedef enum
{
    LonSS_Idle = 0,
    LonSS_Send,
    LonSS_WaitForResult,
    LonSS_MessageOk,
    LonSS_MessageFailed,
    LonSS_MessageDelete
} LonCtrl_SendStatus_e;

typedef enum
{
    LonAPI_Ok,
    LonAPI_Error,
    LonAPI_Pending,
    LonAPI_Timeout
} LonAPI_Result_e;

typedef enum
{
    LonAPI_Broadcast = 0,
    LonAPI_Unicast = 1
} LonAPI_AddressType_e;

typedef struct
{
    ProtocolId_t masterProtocol;
    
    bool error;
    bool initialized;           // basic initialization of lon driver 
    bool basicConfigDone;       // default domains set
    bool online;                // address assigned by master
    
    uint32 startTime;           // timer for command delay
    
    uint32 timer;               // generic timer
    uint32 timeToWait;          // next command delay
    
    uint32 maxUnpolledTime;     // default 5 sec, updated when node assigned
    uint32 pollingTimer;        // polling timeout timer
    uint32 pollingDelayStart;   // marks the time the polling request was received and is used to delay polling response
    uint32 pollingDelay;        // polling delay calculated by position in the polling broacast
    
    
    bool readyForCommands;      // indicate that driver is ready to accept commands
    bool driverOnline;          // can be awake or sleep
    uint32 beforeStartMark;
    bool waitForReply;          // wait for command reply
    
    LonCtrl_ConfigStatus_e configStatus;
    
    uint32 suggestedAddress;
    uint32 assignedAddress;
    
    // domain settings
    uint8 domainIdxToSet;
    uint8 requestedSubnet;
    uint8 requestedNodeId;
    uint8* requestedDomainId;
    uint32 requestedDomainIdLength;

    // send structures
    ClspFrame_t* frameToSend;
    uint32 retryCount;
    uint32 sendTimer;
    uint32 sendDelay;
    LonCtrl_SendStatus_e sendStatus;
    
    
} LonCtrl_Context_t;

LonApiError ClspLon_UpdateDomain(const LonCtrl_Context_t* lonContext);

LonApiError ClspLon_BroadcastUniqueId(uint32 suggestedAddress, ProtocolId_t protocolId);

LonApiError ClspLon_SendAlive();

LonApiError ClspLon_EnqueueAlive();

LonApiError ClspLon_CreateFrameFromData(uint32 code, const uint8* data, uint32 dataLength, ClspFrame_t* frame);

LonApiError ClspLon_ComposeByFrameAndSend(ClspFrame_t* frame);

void ClspLon_CheckDIPChange(LonCtrl_Context_t* lonContext);

void ClspLon_ForceNodeRelease(LonCtrl_Context_t* lonContext);