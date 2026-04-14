#ifdef LON

#include "System\Timer\sysTick.h"
#include "System\Tasks.h"
#include "IO\Inputs.h"
#include "System\dmHandler.h"
#include "System\systemInfo.h"
#include "System\watchdog.h"

#include "LonControl.h"
#include "Communication\ClspLon\Driver\LdvSci.h"
#include "Communication\ClspLon\ClspAPI.h"
#include "Communication\ClspGeneric.h"
#include "Communication\communication.h"

volatile LonCtrl_Context_t _lonContext;


void LonCtrl_Task();
void LonCtrl_SendTask();


//volatile uint32 _tmpTimer;
//volatile uint32 _lastRFCChange = 0;
//
//volatile uint32 _resetCnt = 0;
//volatile uint32 _mre = 0;

void LonCtrl_Init(ProtocolId_t masterProtocol)
{
    WD_Kick();
    UART_Init(0, 38400);
    //UART_Init(0, 76800);
    //UART_Init(0, 153600);
#ifdef NO_CR_DEBUG
    WD_Kick();
    UART_Init(1, 115200);
#endif
    _lonContext.masterProtocol = masterProtocol;
    
    _lonContext.error = false;
    _lonContext.initialized = false;
    _lonContext.basicConfigDone = false;
    _lonContext.readyForCommands = false;
    
    //_lastRFCChange = 1;
    
    _lonContext.waitForReply = false;
    //_lonContext.configStatus = LonCS_QueryStatus;
    //_lonContext.configStatus = LonCS_GoUnconfigured;
    _lonContext.maxUnpolledTime = LONCTRL_UNPOLLED_TIMEOUT;
    _lonContext.startTime = SysTick_GetTickCount();
    _lonContext.suggestedAddress = _specialInputState.address;
    _lonContext.assignedAddress = 0;
    _lonContext.online = false;
    
    _clspSuggestedAddress = _specialInputState.address;
    
    _lonContext.timeToWait = 2000;
    
    // clear send
    _lonContext.sendStatus = LonSS_Idle;
    if (null != _lonContext.frameToSend) 
    {
        Delete((void*)(_lonContext.frameToSend));
        _lonContext.frameToSend = null;
    }
    Clsp_ClearQueues();
    
    _clspIsAssigned = false;
    
    _lonContext.domainIdxToSet = LONCTRL_DEFAULT_DOMAIN_ID;
    _lonContext.requestedSubnet = LONCTRL_DEFAULT_SUBNET;
    _lonContext.requestedNodeId = LONCTRL_DEFAULT_NODEID;
    _lonContext.requestedDomainId = LONCTRL_DEFAULT_DOMAIN;
    _lonContext.requestedDomainIdLength = LONCTRL_DEFAULT_DOMAIN_LEN;
    _lonContext.online = false;
    
    _lonContext.configStatus = LonCS_UpdateConfigToDefault;
    
    //_lonContext.configStatus = LonCS_Done;
    
#ifndef BOOTLOADER
    SysInfo_SignalConnectionDown(true);
#endif
    
    Tasks_Register(LonCtrl_Task, 1);
}



void LonCtrl_Task()
{
    uint32 elapsed;
    
    if (!_lonContext.initialized)
    {
        if (LonInit() != LonApiNoError)
            _lonContext.error = true;
        else
            _lonContext.initialized = true;
    }
    else
    {   
        LonEventHandler();
#ifndef BOOTLOADER
        WD_Kick();
#endif
        if (!_lonContext.readyForCommands)
            return;
        
        // if the basic intialization is done, start checking DIP value changes
        if (_lonContext.basicConfigDone)
            ClspLon_CheckDIPChange((LonCtrl_Context_t*)&_lonContext);
        
        elapsed = SysTick_GetElapsedTime(_lonContext.startTime);
        if (elapsed > _lonContext.timeToWait)
        {
            _lonContext.startTime = SysTick_GetTickCount();
            
            _lonContext.timeToWait = 100;
            
            switch (_lonContext.configStatus)
            {                    
                case LonCS_UpdateConfigToDefault:
                    if (ClspLon_UpdateDomain((const LonCtrl_Context_t*)&_lonContext) == LonApiNoError)
                    {
                        // prepare for active
                        _lonContext.domainIdxToSet = LONCTRL_CONFIG_DOMAIN_ID;
                        _lonContext.requestedSubnet = LONCTRL_CONFIG_SUBNET;
                        _lonContext.requestedNodeId = LONCTRL_CONFIG_NODEID;
                        _lonContext.requestedDomainId = LONCTRL_CONFIG_DOMAIN;
                        _lonContext.requestedDomainIdLength = LONCTRL_CONFIG_DOMAIN_ID;
                        
                        _lonContext.configStatus = LonCS_UpdateActiveToDefault;
                        
#ifdef NO_CR_DEBUG
                        UART_DebugT(">> Update C domain\n");
#endif
                    }
                    break;
                
                case LonCS_UpdateActiveToDefault:
                    if (ClspLon_UpdateDomain((const LonCtrl_Context_t*)&_lonContext) == LonApiNoError)
                    {
                        _lonContext.configStatus = LonCS_GoConfigured;
#ifdef NO_CR_DEBUG
                        UART_DebugT(">> Update A domain\n");
#endif
                    }
                    break;
                
                case LonCS_GoConfigured:
                    if (LonGoConfigured() == LonApiNoError)
                    {
#ifdef NO_CR_DEBUG
                        UART_DebugT(">> Go configured sent\n"); 
#endif
                        _lonContext.configStatus = LonCS_WaitForReply;
                    }
                    break;
                    
                    
                    
                    
                case LonCS_SendAddressRequest:
                    if (_lonContext.suggestedAddress != 0)
                    {                    
                        if (ClspLon_BroadcastUniqueId(_lonContext.suggestedAddress, _lonContext.masterProtocol) == LonApiNoError)
                        {
#ifdef NO_CR_DEBUG
                            UART_DebugT("Address request sent\n");
#endif
                            _lonContext.configStatus = LonCS_WaitForAddress;
                            _lonContext.timer = SysTick_GetTickCount();
                        }
                    }
                    break;
                    
                case LonCS_WaitForAddress:
                    elapsed = SysTick_GetElapsedTime(_lonContext.timer);
                    if (elapsed > LONCTRL_ADDRESS_REQ_TIMEOUT)
                    {
                        _lonContext.configStatus = LonCS_SendAddressRequest;                   
                    }
                    break;
                   
                    
                    
                    
                    
                case LonCS_UpdateDomainToSet:
                    if (ClspLon_UpdateDomain((const LonCtrl_Context_t*)&_lonContext) == LonApiNoError)
                    {
#ifdef NO_CR_DEBUG
                        UART_DebugT(">> Update A domain\n");
#endif
                        _lonContext.configStatus = LonCS_SendAlive;
                    }
                    break;
                    
                case LonCS_SendAlive:
                    if (ClspLon_SendAlive() == LonApiNoError)
                    {
#ifdef NO_CR_DEBUG
                        UART_DebugT(">> Alive sent\n");  
#endif     
                        _lonContext.configStatus = LonCS_WaitForConfirm;
                    }
                    break;
                    
                case LonCS_WaitForConfirm:
                    elapsed = SysTick_GetElapsedTime(_lonContext.timer);
                    if (elapsed > LONCTRL_ADDRESS_REQ_TIMEOUT)
                    {
                        _lonContext.configStatus = LonCS_SendAddressRequest;                   
                    }
                    break;
                    
                    
                    
                    
                    
                case LonCS_Done:
                    elapsed = SysTick_GetElapsedTime(_lonContext.pollingTimer);
                    if (elapsed > _lonContext.maxUnpolledTime)
                    {
#ifdef NO_CR_DEBUG
                        UART_DebugT("Unpolled for too long\n");
#endif     
                        _lonContext.configStatus = LonCS_SendAddressRequest;
                        
                        _lonContext.assignedAddress = 0;
                        _lonContext.online = false;
                        
                        ClspLon_ForceNodeRelease((LonCtrl_Context_t*)&_lonContext);
#ifndef BOOTLOADER
                        SysInfo_SignalConnectionDown(true);
#endif
                    }
                    else if (_lonContext.online)
                    {
                        LonCtrl_SendTask();
                        _lonContext.timeToWait = 0;
                    }
                    
//                    elapsed = SysTick_GetElapsedTime(_tmpTimer);
//                    if (elapsed > 5)
//                    {
////                        _tmpTimer = SysTick_GetTickCount();
//                        
//                        _clspIsAssigned = 1;
//                        
//                        ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_IO_TEST_OK, 0, false);
//                        if (frame != null)
//                            Clsp_EnqueueFrame(frame, false);
//                    }
                    
                    break;
                
            }
        }
    }
}

///// HANDLERS


void OnLonGoConfiguredReceived()
{
    _lonContext.waitForReply = false;
#ifdef NO_CR_DEBUG
    UART_DebugT("<< Go Configured received\n");
#endif
    _lonContext.basicConfigDone = true;
    _lonContext.configStatus = LonCS_SendAddressRequest;
}

//volatile uint32 _onResetCnt = 0;
//volatile uint32 _lastCalled = 0;

void OnResetAsserted()
{
    _lonContext.readyForCommands = false;
    //_lastRFCChange = 5;
//    _lastCalled = 1;
}

void OnResetOccurred(const LonResetNotification* const pResetNotification)
{    
//    _onResetCnt++;
//    _lastCalled = 2;
    
    if (pResetNotification->Version != LON_LINK_LAYER_PROTOCOL_VERSION)
        _lonContext.error = true;
    else if (!LON_GET_ATTRIBUTE((*pResetNotification), LON_RESET_INITIALIZED))
    {
        _lonContext.initialized = false; /* This will result in a re-initialization of the Micro Server */
#ifdef NO_CR_DEBUG
        UART_DebugT(" REINITIALIZATION\n");
#endif
    }
    else
    {       
        _lonContext.readyForCommands = false;
        //_lastRFCChange = 3;
        _lonContext.driverOnline = false;
        
//        _lonContext.waitForReply = false;
        
        //if (_lonContext.configStatus < LonCS_Done)
            //_lonContext.configStatus = LonCS_QueryStatus;
            //_lonContext.configStatus = LonCS_GoUnconfigured;
    }
    
    //_resetCnt++;
#ifdef NO_CR_DEBUG    
    UART_DebugT("<< Reset Occurred, cause: ");
#endif
    switch (pResetNotification->ResetCause)
    {
        case 0x01:
#ifdef NO_CR_DEBUG
            UART_Debug("Power up");
#endif
            break;
        case 0x02:
        case 0x0A:
        case 0x12:
#ifdef NO_CR_DEBUG
            UART_Debug("External");
#endif
            _lonContext.timeToWait = 2000;
            _lonContext.startTime = SysTick_GetTickCount();
            break;
        case 0x0C:
#ifdef NO_CR_DEBUG
            UART_Debug("Watchdog");
#endif
            break;
        case 0x14:
#ifdef NO_CR_DEBUG
            UART_Debug("Software");
#endif
            _lonContext.timeToWait = 2000;
            _lonContext.startTime = SysTick_GetTickCount();
            break;
        default:
#ifdef NO_CR_DEBUG
            UART_DebugHex(pResetNotification->ResetCause);
#endif
            break;
    }
#ifdef NO_CR_DEBUG
    UART_Debug("\n");
#endif
}

void OnLonDriverWokeUp()
{
#ifdef NO_CR_DEBUG
    UART_DebugT("On driver woke up\n");
#endif    
    _lonContext.readyForCommands = true;
    //_lastRFCChange = 4;
    _lonContext.driverOnline = true;
    _lonContext.waitForReply = false;
    
    _lonContext.beforeStartMark = SysTick_GetTickCount();
}

void OnLonDriverReset()
{
    // called from interrupt
#ifdef NO_CR_DEBUG
    UART_DebugT("On driver reset\n");
#endif
    
    _lonContext.driverOnline = false;
    _lonContext.waitForReply = false;
//    _lonContext.initialized = false;
}




///////////////////////////////////////////////////////////////////////////////
// MESSAGE PROCESSING
///////////////////////////////////////////////////////////////////////////////

#if DEBUG
volatile uint32 _sendTimeoutCnt = 0;
#endif // DEBUG

void LonCtrl_SendTask()
{
    LonApiError result;
    
    uint32 elapsed;
    bool continueProcessing;

    do 
    {
        continueProcessing = false;
        
        switch (_lonContext.sendStatus)
        {
            case LonSS_Idle:
                if (!_lonContext.readyForCommands)
                {
                    continueProcessing = false;
                    break;
                }
                
                // check if there is any link layer message
                _lonContext.frameToSend = Clsp_PopLinkLayerQueue();
           
                // if not link layer check regular one
                if (_lonContext.frameToSend == null)
                    _lonContext.frameToSend = Clsp_PopQueue();
                
                if (_lonContext.frameToSend != null) 
                {
                    _lonContext.sendStatus = LonSS_Send;
                    _lonContext.retryCount = 0;
                    
                    continueProcessing = true;
                }
                break;
                
            case LonSS_Send:
                if (!_lonContext.readyForCommands)
                {
                    continueProcessing = false;
                    break;
                }
                
                if (_lonContext.frameToSend->messageParam != 0)
                {
//                    UART_Debug("sm: ");
//                    UART_DebugHex(_lonContext.frameToSend->messageParam);
//                    UART_Debug("\n");
                }
                else
                {
//                    UART_Debug(".");
                }
                
                result = ClspLon_ComposeByFrameAndSend(_lonContext.frameToSend);
                
                if (result == LonApiNoError)
                {
                    _lonContext.sendStatus = LonSS_WaitForResult;
                    _lonContext.sendTimer = SysTick_GetTickCount();
                }            
                else
                {
//                    UART_DebugT("Send error: ");
//                    UART_DebugHex(result);
//                    UART_Debug("\n");
                }
                break;
                
            case LonSS_WaitForResult:
                elapsed = SysTick_GetElapsedTime(_lonContext.sendTimer);
                if (elapsed > LONCTRL_MSG_CONFIRM_TIMEOUT)
                {
                    //UART_DebugT("Message timeout\n");
                    _lonContext.sendStatus = LonSS_MessageFailed;
                }
                break;
            
            case LonSS_MessageOk:
                _lonContext.sendStatus = LonSS_MessageDelete;
                break;
                
            case LonSS_MessageFailed:
                if (_lonContext.retryCount < LONCTRL_FRAME_RETRY_CNT)
                {
                    _lonContext.retryCount++;
                    _lonContext.sendStatus = LonSS_Send;
#ifdef DEBUG
                    _sendTimeoutCnt++;
#endif
                }
                else
                {
                    //_mre++;
                    //UART_DebugT("!!! MRE\n");
                    _lonContext.sendStatus = LonSS_MessageDelete;
                }
                break;
                
            case LonSS_MessageDelete:
                Delete(_lonContext.frameToSend);
                _lonContext.frameToSend = null;
                _lonContext.sendStatus = LonSS_Idle;
                
                _lonContext.sendDelay = SysTick_GetTickCount();
                continueProcessing = true;
                break;
        }

    } while (continueProcessing);
}

void OnLonMsgCompleted(const unsigned tag, const LonBool success)
{
    if (tag == LonMtIndexDcuMessage && _lonContext.sendStatus == LonSS_WaitForResult)
    {
        if (success)
        {
            // check if this was unicast message, if so refresh polling timer
            // DISABLED - this caused unintended behaviour, when master app was down, however
            // lonCoprocessor responding on LinkLayer, thus keeping this DCU always online
            //if ((_lonContext.frameToSend->flags & CLSP_FRAME_ADDRESS_TYPE) == LonAPI_Unicast)
            //    _lonContext.pollingTimer = SysTick_GetTickCount();
            
            _lonContext.sendStatus = LonSS_MessageOk;
        }
        else
            _lonContext.sendStatus = LonSS_MessageFailed;
    }
}





/*
 * Task for polling delay, when polling request is received, delay time is calculated and this task is started.
 * When delay elapses, polling is sent and this task is unregistered 
 */
static void DelayedPollingTask()
{
    uint32 elapsed = SysTick_GetElapsedTime(_lonContext.pollingDelayStart);
    
    if (elapsed >= _lonContext.pollingDelay)
    {
        ClspLon_EnqueueAlive();        
        Tasks_Unregister(DelayedPollingTask);
    }
}

void OnLonMsgArrived(const LonReceiveAddress* const pAddress, const LonCorrelator correlator, 
                   const LonBool priority, const LonServiceType serviceType, const LonBool authenticated, 
                   const LonByte code, const LonByte* const pData, const unsigned dataLength)
{
    //return;
    
    ProtocolId_t proto = PROTO_INVALID;
    //bool suspendResult;
    //uint32 deadlockCnt = 0;
    
    ClspFrame_t recvFrame;    
    
    bool isPollingForMe = false;
    int i;
    
    switch ((pAddress->DomainFormat & LON_RECEIVEADDRESS_FORMAT_MASK))
    {
        case LonReceiveDestinationAddressBroadcast:
            if (_lonContext.online && code == POLLING)
            {
                // check if this polling request is also for this node
                for (i = 0; i < dataLength; i++)
                {
                    if (_lonContext.assignedAddress != 0 && pData[i] == _lonContext.assignedAddress)
                    {
                        isPollingForMe = true;
                        break;
                    }
                }
                
                if (isPollingForMe)
                {
                    _lonContext.pollingDelayStart = SysTick_GetTickCount();
                    _lonContext.pollingDelay = LONCTRL_POLLING_DELAY_MULTIPLIER * i;
                    _lonContext.pollingTimer = SysTick_GetTickCount();
                    Tasks_Register(DelayedPollingTask, 5);                 
                }
            }
            break;

        case LonReceiveDestinationAddressSubnetNode:
            if (code == NODE_ASSIGNMENT_CONFIRMED)
            {
                _clspIsAssigned = true;
                _lonContext.configStatus = LonCS_Done;
                _lonContext.pollingTimer = SysTick_GetTickCount();
#ifndef BOOTLOADER
                SysInfo_SignalConnectionDown(false);
#endif
                FireNodeAssignmentChanged(_lonContext.assignedAddress, true);
            }
            else if (_lonContext.online)
            {
                ClspLon_CreateFrameFromData(code, pData, dataLength, &recvFrame);
                
                if (recvFrame.messageCode >= PROTOCOL_FIRST_ID && recvFrame.messageCode <= PROTOCOL_LAST_ID) {
                    proto = (ProtocolId_t)((int)recvFrame.messageCode - (int)PROTOCOL_FIRST_ID);
                    
                    // application unicast also revives the polling state
                    _lonContext.pollingTimer = SysTick_GetTickCount();
                }
                
                // try to suspend driver
//                do 
//                {
//                    suspendResult = SuspendSci();
//                    
//                    if (++deadlockCnt > 1000)
//                    {
//                        UART_DebugT("Suspend failed after 1000 cnt\n");
//                        break;
//                    }
//                } while (!suspendResult);
                
                //suspendResult = SuspendSci();
                SuspendSci();
                // process message
                FireOnFrameReceived(&recvFrame, proto, false);
                // resume driver
                ResumeSci();
            }
            break;
            
        case LonReceiveDestinationAddressUniqueId:
            if ((pAddress->DomainFormat & LON_RECEIVEADDRESS_DOMAIN_MASK) >> LON_RECEIVEADDRESS_DOMAIN_SHIFT == 1)
            {
                switch (code)
                {
                    case NODE_ASSIGNED:
                        _lonContext.domainIdxToSet = 0;
                        _lonContext.requestedSubnet = LONCTRL_WORKING_SUBNET;
                        _lonContext.requestedNodeId = _lonContext.suggestedAddress;
                        _lonContext.requestedDomainId = LONCTRL_DEFAULT_DOMAIN;
                        _lonContext.requestedDomainIdLength = LONCTRL_DEFAULT_DOMAIN_LEN;
                        
                        _lonContext.configStatus = LonCS_UpdateDomainToSet;
                        _lonContext.assignedAddress = _lonContext.suggestedAddress;
                        _lonContext.online = true;
                        
                        // updatte unpolled timeout
                        if (dataLength > 2)
                            _lonContext.maxUnpolledTime = pData[2] * 1000;
#ifndef BOOTLOADER
                        SysInfo_SignalAddressConflict(false);
#endif
                        break;
                    case NODE_ADDRESS_CONFLICT:
                        _lonContext.configStatus = LonCS_WaitForAddressChange;
#ifndef BOOTLOADER
                        SysInfo_SignalAddressConflict(true);
                        FireNodeAddressConflicted(_clspSuggestedAddress);
#endif
                        _lonContext.online = false;
                        break;
                }
            }
            
            break;
    }
}




#endif // LON