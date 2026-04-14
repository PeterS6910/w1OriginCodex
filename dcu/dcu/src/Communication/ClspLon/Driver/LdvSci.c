#ifdef LON

/*
 *  Filename: LdvSci.c
 *
 *  Description:  This file contains the ARM7 ShortStack SCI driver 
 *  implementation to interface with a ShortStack Micro Server.
 *
 * Copyright (c) Echelon Corporation 2008.  All rights reserved.
 *
 * This file is Example Software as defined in the Software
 * License Agreement that governs its use.
 *
 * ECHELON MAKES NO REPRESENTATION, WARRANTY, OR CONDITION OF
 * ANY KIND, EXPRESS, IMPLIED, STATUTORY, OR OTHERWISE OR IN
 * ANY COMMUNICATION WITH YOU, INCLUDING, BUT NOT LIMITED TO,
 * ANY IMPLIED WARRANTIES OF MERCHANTABILITY, SATISFACTORY
 * QUALITY, FITNESS FOR ANY PARTICULAR PURPOSE, 
 * NONINFRINGEMENT, AND THEIR EQUIVALENTS.
 */

//#define PRINT_LINK_LAYER

#include "Communication\ClspLon\Others\Processor.h"
#include "Communication\ClspLon\LonPlatform.h"
#include "LdvSci.h"

// TODO if this works then create TIME32 handler somewhere else
#include "System\ADC\adc.h"

#ifndef XPRESSO
	#include "intrinsics.h" /* required for __enable_interrupt, __disable_interrupt */
#endif

#include "System\baseTypes.h"
#include "System\LPC122x.h"
#include "System\GPIO\gpio.h"
#include "System\Timer\timer.h"
#include "System\Timer\sysTick.h"
#include "System\Timer\timer32.h"
#include "System\watchdog.h"

/*
 * Interrupt priority definitions
 * Define all the interrupts at the highest priority.
 * We don't want any of the interrupts to pre-empt others.
 * If they do, they can generate certain race conditions where
 * we may report receive timeouts even when they don't occur
 * and as a result reset the driver and the Micro Server.
 * 7 is the highest priority
 */

/* 
 * Define the baud rate for the usart.
*/ 
#if (SS_BAUD_RATE == 0)
    #define USART_BAUD_RATE             76800
#elif (SS_BAUD_RATE == 1)
    #define USART_BAUD_RATE             9600
#elif (SS_BAUD_RATE == 2)
    #define USART_BAUD_RATE             38400
#endif

LdvSysTxBuffer              _lonTxBuffer[LDV_TXBUFCOUNT];
LdvSysRxBuffer              _lonRxBuffer[LDV_RXBUFCOUNT];
volatile LdvDriverStatus    _lonDriverStatus;

/* counters for debug purposes */
LonUbits32 _nRxErrors = 0;
LonUbits32 _nRxTimeout = 0;
LonUbits32 _nTxBufUnavailable = 0;

/* Utility Timers */
unsigned _nUtilityUpTimer[4] = {0}; /* 4 utility timers that count up */
unsigned _nUtilityDownTimer[4] = {0}; /* 4 utility timers that count down */
#define MAX_UNSIGNED    0xFFFFFFFF

/* 
 * Intrinsic functions used to enable and disable global interrupts.
 * The interrupts should be disabled before accessing any shared date,
 * and enabled afterwards.
 */

extern void OnLonDriverWokeUp();
extern void OnLonDriverReset();
extern void OnResetAsserted();

void EnableInterrupts()
{
    GPIO_IntEnable(PORT0, CTS);
    NVIC_EnableIRQ(UART0_IRQn);
    //Timer32_Enable(1);
    _lonDriverStatus.TimerEnabled = true;
    
    GPIO_IntEnable(PORT0, NEURON_RESET);
}

void DisableInterrupts()
{
    GPIO_IntDisable(PORT0, CTS);
    NVIC_DisableIRQ(UART0_IRQn);
    //Timer32_Disable(1);
    _lonDriverStatus.TimerEnabled = false;
    GPIO_IntDisable(PORT0, NEURON_RESET);
}

void LonDriverWokeUp()
{
    OnLonDriverWokeUp();
}

void LonDriverReset()
{
    OnLonDriverReset();
}

/*
 * Forward declarations for the interrupt handler functions.
*/
//extern void CtsInterruptHandler(void);
//extern void ResetInterruptHandler(void);

volatile uint32 _resetAssCnt = 0;
volatile uint32 _resetDeassCnt = 0;

void ResetInterruptHandler(void)
{
    if (!CHECK_RESET_ASSERTED())
    {
        //UART_DebugT("Reset asserted\n");
        OnResetAsserted();
        
        _resetAssCnt++;
    }
    else
    {
        //UART_DebugT("Reset deasserted\n");
        //
        _resetDeassCnt++;
    }
}

/*
 * Internal function used to reset the Micro Server.
 */
void ResetMicroServer(void) 
{
    //UART_DebugT("ResetMicroServer\n");
    //printf("RMS\n");
    
    LonDriverReset();       // my added call to notify upper layer that the driver is down
    
    /* Configure the Neuron Reset line as output so, we can drive it - only temporarily */
    GPIO_SetDir(PORT0, NEURON_RESET, 1);
    /* Assert the reset line to reset the Micro Server */
    GPIO_SetValue(PORT0, NEURON_RESET, 0);
    /* Wait for some time. */
    SleepMs(50);
    /* Configure the reset line back as input */
    GPIO_SetDir(PORT0, NEURON_RESET, 0);
    /* Pull up the reset line so its deasserted */
    LPC_IOCON->PIO0_29 |= (1 << 4); // this should be by default anyway
    /* Wait for some time. The Micro Server takes approximately 200 ms to reset */
    SleepMs(200);
}

/*
 *  Function: SuspendSci
 *  Function to suspend the SCI driver.
 */
LonBool SuspendSci(void)
{
    /* Make sure that the transmitter and receiver are idle.
     * Also, make sure we haven't asserted the RTS line to begin any new transmission. */
    if (_lonDriverStatus.TxState == LdvTxIdle  &&  _lonDriverStatus.RxState == LdvRxIdle  &&  CHECK_RTS_DEASSERTED())
    {
        /* Deassert the HRDY to tell the mircoserver to stop sending data. */
        DEASSERT_HRDY();
        /* Wait for the time it takes to transfer 2 bytes of packet header (approximately 2 ms).
         * The Micro Server may have started sending the packet before seeing the HRDY deasserted. */
        SleepMs(2);
        /* Check if the receiver is still idle */
        if (_lonDriverStatus.RxState == LdvRxIdle)
        {
            /* Receiver is idle. We can safely suspend the driver */
            DISABLE_TX_INT();
            DISABLE_RX_INT();
            _lonDriverStatus.DriverState = LdvDriverSleep;       /* Put the driver into sleep mode */
            DEASSERT_RTS();
            return TRUE;
        }
        /* Else receiver is not idle.
         * Don't suspend the driver, but don't reassert the HRDY
         * so that it can be suspended the next time.
         * If it doesn't need to be suspended, the HRDY will automatically
         * get asserted when there is an empty receive buffer */
    }
    return FALSE;
}


/*
 *  Function: ResumeSci
 *  Function to resume the SCI driver.
 */
void ResumeSci(void)
{
    /* Make sure that the driver was actually suspended
     * before resuming it */
    if (_lonDriverStatus.DriverState == LdvDriverSleep)
    {
        unsigned int i;
        
        _lonDriverStatus.DriverState = LdvDriverNormal;
        /* Clear interrupts if any, by reading the status register */
        uint32 IIRValue, LSRValue;
        IIRValue = LPC_UART0->IIR;
        LSRValue = LPC_UART0->LSR;
        // dummy operation to make sure compiler does not do some magic
        IIRValue = LSRValue;
        LSRValue = IIRValue;        
            
        ENABLE_RX_INT();        /* Enable receive interrupt */
        /* Resume Neuron if receive buffer is not full */
        for (i = 0; i < LDV_RXBUFCOUNT; i ++) 
        {
            if(_lonRxBuffer[i].State == LdvRxBufferEmpty)
            {
                ASSERT_HRDY();
                break;
            }
        }
    }
}

/* 
 * Internal helper function to increment the various indices in a cyclic manner 
 */
static void CyclicIncrement(LdvIndexType index)
{
    switch (index)
    {
        case LdvIndexRxBufferReady:
            if (++_lonDriverStatus.RxBufferReadyIndex >= LDV_RXBUFCOUNT)
                _lonDriverStatus.RxBufferReadyIndex = 0;
            break;
        case LdvIndexRxBufferReceive:
            if (++_lonDriverStatus.RxBufferReceiveIndex >= LDV_RXBUFCOUNT)
                _lonDriverStatus.RxBufferReceiveIndex = 0;
            break;
        case LdvIndexTxBufferEmpty:
            if (++_lonDriverStatus.TxBufferEmptyIndex >= LDV_TXBUFCOUNT)
                _lonDriverStatus.TxBufferEmptyIndex = 0;
            break;
        case LdvIndexTxBufferTransmit:
            if (++_lonDriverStatus.TxBufferTransmitIndex >= LDV_TXBUFCOUNT)
                _lonDriverStatus.TxBufferTransmitIndex = 0;
            break;
        default:
            break;
    }
}

#ifdef PRINT_LINK_LAYER
static void PrintData(LonByte* pData, unsigned length, LonBool bTransmit, int bufIdx)
{
    char hexChars[] = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
    char sData[256];
    char sByte[4];
    unsigned i;
    if (bTransmit) 
        strcpy(sData, "Transmitted : ");
    else
        strcpy(sData, "Received    : ");

    for (i = 0; i < length; i ++)
    {
        //sprintf(sByte, "%X ", pData[i]);
        sByte[0] = hexChars[((pData[i] >> 4) & 0x0f)];
        sByte[1] = hexChars[(pData[i] & 0x0f)];
        sByte[2] = ' ';
        sByte[3] = 0;
        strcat(sData, sByte);
    }
    strcat(sData, "\n");
    
    //UART_DebugExtended(1, sData, 115200);
    //UART_DebugT(sData);
    printf("%s\n", sData);
}
#endif

/*
 * Function: LdvInit
 * Initialize the serial driver.
 * 
 * Remarks:
 * This function is called to initialize the serial driver.
 * Previously named ldv_init.
 */
void LdvInit(void) 
{
    //UART_DebugT("LdvInit\n");
    
    LonUbits8 i;
    
    _lonDriverStatus.DriverState            = LdvDriverSleep;
    _lonDriverStatus.RxState                = LdvRxIdle;
    _lonDriverStatus.RxBufferReceiveIndex   = 0;
    _lonDriverStatus.RxBufferReadyIndex     = 0;
    _lonDriverStatus.TxState                = LdvTxIdle;
    _lonDriverStatus.TxBufferTransmitIndex  = 0;
    _lonDriverStatus.TxBufferEmptyIndex     = 0;
    _lonDriverStatus.pTxMsg                 = 0;
    _lonDriverStatus.DrvWakeupTime          = LDV_DRVWAKEUPTIME;
    _lonDriverStatus.RxTimeout              = 0;
    _lonDriverStatus.KeepAliveTimeout       = LDV_KEEPALIVETIMEOUT;
    _lonDriverStatus.PutMsgTimeout          = 0;

    /* Mark all receiver buffers as empty */
    for (i = 0; i < LDV_RXBUFCOUNT; i++)
        _lonRxBuffer[i].State = LdvRxBufferEmpty;

    /* Mark all transmit buffers as empty */
    for (i = 0; i < LDV_TXBUFCOUNT; i++)
        _lonTxBuffer[i].State = LdvTxBufferEmpty;
        
    /* Configure the RTS line as output */
    GPIO_SetDir(PORT0, RTS, 1);
    DEASSERT_RTS();
    
    /* Configure the CTS line as input */
    GPIO_SetDir(PORT0, CTS, 0);
    
    /* Pull up the CTS line so its deasserted */
    LPC_IOCON->PIO0_7 |= (1 << 4);
    
    /* Configure the Host Ready and the Baud Rate selector lines as output */
    GPIO_SetDir(PORT0, HRDY, 1);
    DEASSERT_HRDY();
    
    /* Configure the Neuron Reset line as input */
    GPIO_SetDir(PORT0, NEURON_RESET, 0);
    /* Pull up the reset line so its deasserted */
    LPC_IOCON->PIO0_29 |= (1 << 4);
                                  
    /* Enable PIO interrupt for CTS */
    GPIO_SetInterrupt(PORT0, CTS, 0, 1, 0);
    GPIO_IntEnable(PORT0, CTS);
    
    /* Enable PIO interrupt for Reset */
    GPIO_SetInterrupt(PORT0, NEURON_RESET, 0, 1, 0);
    GPIO_IntEnable(PORT0, NEURON_RESET);
    
    /* Enable periodic timer interrupt */
    Timer32_Init(1, ONE_MS_TIME_INTERVAL);
    Timer32_Enable(1);
    
    /* Enable Global interrupts */
    EnableInterrupts();
}

/*
 * Function: LdvReset
 * Reset the serial driver.
 * 
 * Remarks:
 * This function is called to reset the serial driver.
 * The API resets the serial driver whenever it receives an uplink reset 
 * message from the Micro Server. A reset may also be required in cases when 
 * the driver detects some error in transmission or goes out of sync with the
 * Micro Server.
 * The driver should drop all pending receive and transmit transactions and go 
 * back to the initial state.
 */
void LdvReset(void) 
{
    DISABLE_TX_INT();
    DISABLE_RX_INT();
    DEASSERT_RTS();
    DEASSERT_HRDY();
    _lonDriverStatus.DriverState = LdvDriverSleep;       /* Put the driver into sleep mode to begin with */
    _lonDriverStatus.DrvWakeupTime = LDV_DRVWAKEUPTIME;  /* Actual reset will happen when the driver wakes up */
    
    _lonDriverStatus.SuspendTransmittion = true;
    _lonDriverStatus.SuspendReceive = true;
}

/*
 * Function: LdvGetMsg
 * Gets an incoming message from the serial driver.
 *
 * Parameters:
 * ppMsg - pointer to the transmit buffer pointer that contains the incoming 
 * message.
 *
 * Returns:
 * <LonApiError> - LonApiNoError if a message is available and was successfully 
 *                 returned, an appropriate error code, otherwise.
 *
 * Remarks:
 * This function gets an incoming message from the serial driver.
 * Note that the caller has a pointer into the driver memory upon a successful
 * return from the call. The caller must free the memory to the driver later 
 * by calling <LdvReleaseMsg>. 
 * Previously named ldv_get_msg.
 */
LonApiError LdvGetMsg(LonSmipMsg **ppMsg)
{
    LonApiError result = LonApiRxMsgNotAvailable;
    LonUbits8 i;
    
    for (i = 0; i < LDV_RXBUFCOUNT; i++)
    {
        DisableInterrupts();

        if (_lonRxBuffer[_lonDriverStatus.RxBufferReadyIndex].State == LdvRxBufferReady)
        {
            _lonRxBuffer[_lonDriverStatus.RxBufferReadyIndex].State = LdvRxBufferProcessing;
            *ppMsg = (LonSmipMsg*)_lonRxBuffer[_lonDriverStatus.RxBufferReadyIndex].Data;

            EnableInterrupts();

            CyclicIncrement(LdvIndexRxBufferReady);
            result = LonApiNoError;
            break;
        }

        EnableInterrupts();

        CyclicIncrement(LdvIndexRxBufferReady);
    }
    
    return result;
}

/*
 * Function: LdvReleaseMsg
 * Releases a message buffer back to the serial driver.
 *
 * Parameters:
 * pMsg - pointer to the message buffer to be released.
 *
 * Remarks:
 * This function releases a message buffer back to the serial driver.
 * Note that the driver assumes that upon return, the memory pointed 
 * to by *pMsg* has been returned to the driver. Therefore, the caller 
 * must not use this memory anymore.
 * Previously named ldv_release_msg.
 */
void LdvReleaseMsg(const LonSmipMsg *pMsg)
{
    LonUbits8 i;
    const LonSmipMsg *pTMsg;
    
    for (i = 0; i < LDV_RXBUFCOUNT; i++) 
    {
        DisableInterrupts();

        pTMsg = (const LonSmipMsg *)_lonRxBuffer[i].Data;
        if (pTMsg == pMsg)
        {
            /* Found the message to be released in the buffer */
            _lonRxBuffer[i].State = LdvRxBufferEmpty;
            EnableInterrupts();
            
            /* Resume ShortStack Micro Server if driver is in normal state */
            if (_lonDriverStatus.DriverState == LdvDriverNormal)
                ASSERT_HRDY();
            break;
        }
        
        EnableInterrupts();
    }
}

/*
 * Function: LdvAllocateMsg
 * Allocates a transmit buffer from the serial driver.
 *
 * Parameters:
 * ppMsg - pointer to the transmit buffer pointer that will be returned.
 *
 * Returns:
 * <LonApiError> - LonApiNoError if the message was successfully allocated, 
 *                 an appropriate error code, otherwise.
 *
 * Remarks:
 * This function allocates a transmit buffer from the serial driver.
 * Note that the caller has a pointer into the driver memory upon a successful
 * return from the call. The caller must free the memory to the driver later 
 * by calling <LdvPutMsg>. 
 * Previously named ldv_allocate_msg.
 */
LonApiError LdvAllocateMsg(LonSmipMsg **ppMsg)
{
    LonApiError result = LonApiTxBufIsFull;
    LonUbits8 i;
    LonUbits8 *pMsg;
    
    for (i = 0; i < LDV_TXBUFCOUNT; i++)
    {
        DisableInterrupts();
        
        if (_lonTxBuffer[_lonDriverStatus.TxBufferEmptyIndex].State == LdvTxBufferEmpty)
        {
            /* Found an empty message buffer */
            _lonTxBuffer[_lonDriverStatus.TxBufferEmptyIndex].State = LdvTxBufferFilling;
            pMsg = _lonTxBuffer[_lonDriverStatus.TxBufferEmptyIndex].Data;
            *ppMsg = (LonSmipMsg*)pMsg;

            EnableInterrupts();

            CyclicIncrement(LdvIndexTxBufferEmpty);
            result = LonApiNoError;
            break;
        }
        
        EnableInterrupts();
        
        CyclicIncrement(LdvIndexTxBufferEmpty);
    }
    
    if (result != LonApiNoError)
        _nTxBufUnavailable++;
    
    return result;
}

/*
 * Function: LdvPutMsg
 * Sends a message downlink.
 *
 * Parameters:
 * pMsg - pointer to the message that will be sent.
 *
 * Remarks:
 * This function sends a message downlink. The message must have been allocated
 * from the transmit buffer using <LdvAllocateMsg>. Note that the driver assumes
 * that upon return, the memory pointed to by *pMsg* has been returned to 
 * the driver. Therefore, the caller must not use this memory anymore.
 * Previously named ldv_put_msg.
 */
void LdvPutMsg(const LonSmipMsg* pMsg)
{
    LonUbits8 i;
    const LonSmipMsg *pTMsg;
   
    for (i = 0; i < LDV_TXBUFCOUNT; i++)
    {
        DisableInterrupts();

        pTMsg = (const LonSmipMsg*)_lonTxBuffer[i].Data;
        if (pTMsg == pMsg) 
        {
            /* Found the message to be transmitted */
            _lonTxBuffer[i].State = LdvTxBufferReady;
            EnableInterrupts();

            break;
        }

        EnableInterrupts();
    }
}

/*
 * Function: LdvPutMsgBlocking
 * Sends a message downlink using a blocking call.
 *
 * Parameters:
 * pMsg - pointer to the message that will be sent.
 *
 * Returns:
 * <LonApiError> - LonApiNoError if the message was successfully sent, 
 *                 an appropriate error code, otherwise.
 *
 * Remarks:
 * This function sends a message downlink without allocating a transmit buffer 
 * from the serial driver. Note that this function blocks until the driver 
 * completes transmitting the message pointed to by *pMsg*. This function is 
 * typically used to send the initialization messages to the Micro Server.
 * Previously named ldv_put_msg_init.
 */
LonApiError LdvPutMsgBlocking(const LonSmipMsg* pMsg)
{
    LonApiError result = LonApiNoError;
    
    _lonDriverStatus.PutMsgTimeout = LDV_PUTMSGTIMEOUT;
    
    while (TRUE)
    {    
        DisableInterrupts();

        if (_lonDriverStatus.pTxMsg == 0)
        {
            /* There are no messages being transmitted, so transmit this message */
            _lonDriverStatus.pTxMsg = (const LonByte *)pMsg;
            EnableInterrupts();
            break;
        }
        
        /* Check the timer */
        if (_lonDriverStatus.PutMsgTimeout == 0)
        {
            /* The timer has expired. Declare the Micro Server as unresponsive */
            EnableInterrupts();
            result = LonApiMicroServerUnresponsive;
            break;
        }
        
        EnableInterrupts();
        
        /* If we got here, it means there are pending messages.
         * Transmit them first and then try to transmit pMsg. */
        LdvFlushMsgs();
//        UpdateWatchDogTimer();
#ifndef BOOTLOADER
        WD_Kick();
#endif
    }

    /* 
     * Our message is now in the driver.
     * Wait for the transmission to be finished.
     */
    while (_lonDriverStatus.pTxMsg == (const LonByte*)pMsg)
    {
        LdvFlushMsgs();
//        UpdateWatchDogTimer();
#ifndef BOOTLOADER
        WD_Kick();
#endif
        
        /* Check the timer */
        DisableInterrupts();
    
        if (_lonDriverStatus.PutMsgTimeout == 0)
        {
            /* The timer has expired. Declare the Micro Server as unresponsive */
            EnableInterrupts();

            result = LonApiMicroServerUnresponsive;
            break;
        }
        
        EnableInterrupts();
    }
    
    return result;
}

/*
 * Function: LdvFlushMsgs
 * Complete pending transmissions.
 *
 * Remarks:
 * This function must be called during the idle loop to complete pending 
 * transmissions.
 * Previously named ldv_flush_msgs.
 */
void LdvFlushMsgs(void)
{
    LonUbits8 i;

    if (CHECK_CTS_DEASSERTED()) /* If CTS is asserted, there is already a message being transmitted */
    {
        DisableInterrupts();

        if (_lonDriverStatus.TxState == LdvTxIdle && _lonDriverStatus.DriverState != LdvDriverSleep) 
        {
            /* The driver is awake and is ready to transmit */
            if (_lonDriverStatus.pTxMsg != 0) 
            {
                /* There is already a message that needs to be transmitted.
                 * Assert the RTS line and then the interrupt routine will handle the transfer */
                ASSERT_RTS();
                EnableInterrupts();
            }
            else
            {
                /* The driver doesn't have any pending messages to transmit.
                 * Time to pull out any buffered messages */
                for (i = 0; i < LDV_TXBUFCOUNT; i++) 
                {
                    DisableInterrupts();

                    if (_lonTxBuffer[_lonDriverStatus.TxBufferTransmitIndex].State == LdvTxBufferReady) 
                    {
                        /* Found a buffer that is ready for transmission */
                        _lonDriverStatus.pTxMsg = _lonTxBuffer[_lonDriverStatus.TxBufferTransmitIndex].Data;
                        EnableInterrupts();

                        CyclicIncrement(LdvIndexTxBufferTransmit);
                        /* Assert the RTS line and then the interrupt routine will handle the transfer */
                        ASSERT_RTS();
                        break;
                    }
                    EnableInterrupts();

                    CyclicIncrement(LdvIndexTxBufferTransmit);
                }
            }
        }
        else
            EnableInterrupts();
    }
}








void OnTxReady()
{
    bool txDone = false;
    
    while ((LPC_UART0->LSR & LSR_THRE) && !txDone)
    {
        // check that there is no serious problem with rx
        if (_lonDriverStatus.SuspendTransmittion)
            return;
        
        /* If data needs to be transmitted, make sure that the CTS line is still asserted */
        if (_lonDriverStatus.TxState != LdvTxDone  &&  CHECK_CTS_DEASSERTED())
        {
            /* Something unusual happened (such as Neuron reset) */
            /* Reset the Micro Server and start over. Once the Micro Server resets, 
               the uplink reset message will come in and it will reset this driver also  */
            
            // this sometimes happens after transmittion, because of the way thre interrupt is called
            if (_lonDriverStatus.TxState == LdvTxIdle)
                return;
            
//            UART_Debug("OnTxReady, TxState: ");
//            UART_DebugHex(_lonDriverStatus.TxState);
//            UART_Debug("\n");
//            
//            if (txDone)
//                UART_Debug("txDone\n");
            
            ResetMicroServer();
            
            txDone = true;
        }
        else
        {
            const LonByte* pTM = _lonDriverStatus.pTxMsg;

            switch (_lonDriverStatus.TxState)
            {
                case LdvTxIdle:
                    /* Transmit length */
                    if (pTM != 0)
                    {
                        for (uint32 i = 0; i < LDV_TXBUFCOUNT; i++)
                        {
                            /* See if this message belongs to any buffer.
                             * If found and the buffer is ready to transmit, change its state */
                            if ((_lonTxBuffer[i].Data == pTM) && (_lonTxBuffer[i].State == LdvTxBufferReady))
                            {
                                _lonTxBuffer[i].State = LdvTxBufferTransmitting;
                                break;
                            }
                        }
                        _lonDriverStatus.TxNextChar = 0;
                        _lonDriverStatus.TxPayloadLen = pTM[0];   /* The first byte in the message is the payload length */
                        _lonDriverStatus.TxState = LdvTxCmd;
                        
                        LPC_UART0->THR = pTM[_lonDriverStatus.TxNextChar++];
                    }
                    break;
        
                case LdvTxCmd:
                    /* Transmit command */
//                    UART_Debug("Command to send: ");
//                    UART_DebugHex(pTM[DriverStatus.TxNextChar]);
//                    UART_Debug("\n");
                    
                    LPC_UART0->THR = pTM[_lonDriverStatus.TxNextChar++];

                    /* Two byte header has been sent */
                    /* First, check to see if info byte needs to be sent */
                    if (pTM[_lonDriverStatus.TxNextChar - 1] == (LonNiNv | LON_NV_ESCAPE_SEQUENCE))
                    {
                        _lonDriverStatus.TxState = LdvTxHandShake; /* As soon as the CTS is deasserted, we will assert RTS to send the info */
                        _lonDriverStatus.TxNextState = LdvTxInfo_1;
                        DISABLE_TX_INT();                   /* Disable transmit interrupt */
                        txDone = true;
                    }
                    /* Check if there is payload to be sent */
                    else if (_lonDriverStatus.TxPayloadLen != 0) 
                    {
                        _lonDriverStatus.TxState = LdvTxHandShake; /* As soon as the CTS is deasserted, we will assert RTS to send the payload */
                        _lonDriverStatus.TxNextState = LdvTxPayload;
                        DISABLE_TX_INT();                   /* Disable transmit interrupt */
                        txDone = true;
                    }
                    else
                    {    
                        _lonDriverStatus.TxState = LdvTxDone;
                    }                   
                    break;
                    
                case LdvTxInfo_1:
                    /* Transmit the first info byte */
                    LPC_UART0->THR = ((LonSicb*) ((LonSmipMsg*) pTM)->Payload)->NvMessage.Index;
                    _lonDriverStatus.TxState = LdvTxInfo_2;
                    break;
                    
                case LdvTxInfo_2:
                    /* Transmit the second info byte */
                    /* For now, this byte is 0x00 */
                    LPC_UART0->THR = 0x00;
                   
                    /* Info bytes have been sent */
                    /* Check if there is payload to be sent */
                    if (_lonDriverStatus.TxPayloadLen != 0) 
                    {
                        _lonDriverStatus.TxState = LdvTxHandShake; /* As soon as the CTS is deasserted, we will assert RTS to send the payload */
                        _lonDriverStatus.TxNextState = LdvTxPayload;
                        DISABLE_TX_INT();                   /* Disable transmit interrupt */
                        txDone = true;
                    }
                    else
                    {    
                        _lonDriverStatus.TxState = LdvTxDone;
                    }
                    
                    break;
        
                case LdvTxPayload:
                    /* Keep transmitting payload */
                    LPC_UART0->THR = pTM[_lonDriverStatus.TxNextChar++];    

                    if (--_lonDriverStatus.TxPayloadLen == 0)
                    {
                        _lonDriverStatus.TxState = LdvTxDone;
                    }
                    break;
        
                case LdvTxDone:
                
                    #ifdef PRINT_LINK_LAYER
                    PrintData((LonByte*) pTM, _lonDriverStatus.TxNextChar, 1, 0);
                    #endif
                    
                    for (uint32 i = 0; i < LDV_TXBUFCOUNT; i ++) 
                    {
                        /* Find the buffer which was being transmitted if any, and if found, change its state to empty */
                        if (_lonTxBuffer[i].State == LdvTxBufferTransmitting)
                        {
                            _lonTxBuffer[i].State = LdvTxBufferEmpty;
                            break;
                        }
                    }
                    _lonDriverStatus.pTxMsg = 0;
                    DISABLE_TX_INT();        /* We are done transmitting, disable transmit interrupt */
                    _lonDriverStatus.TxState = LdvTxIdle;
                    
                    txDone = true;
                    break;
        
                default:
                    break;
            } 
            /* Restart the keep-alive timer */
            _lonDriverStatus.KeepAliveTimeout = LDV_KEEPALIVETIMEOUT;
        }
        
        if (!txDone && !_lonDriverStatus.SuspendTransmittion)
        {
            uint32 deadlockCnt = 0;
            while (!(LPC_UART0->LSR & LSR_THRE))
            {
                if (!deadlockCnt > 1000)
                    return;
            }
        }
    }
}

/* 
 * Interrupt handler function for the CTS line
 */
void CtsInterruptHandler(void)
{
    /* CTS line has changed since there is no other cause for this interrupt to occur */
    if (CHECK_CTS_ASSERTED())
    {
        /* Micro Server is ready to receive data */
        ENABLE_TX_INT();    /* This will trigger the transmission to begin */
        DEASSERT_RTS();     /* No need to keep RTS asserted any more */
        
        OnTxReady();
    }
    else /* CTS line has been deasserted */
    {
        if (_lonDriverStatus.TxState == LdvTxHandShake)
        {
            /* The header has been fully transmitted and info/payload needs to be sent */
            ASSERT_RTS();            /* Request to send info or payload */
            if (_lonDriverStatus.TxNextState == LdvTxInfo_1)
                _lonDriverStatus.TxState = LdvTxInfo_1;
            else if (_lonDriverStatus.TxNextState == LdvTxPayload)
                _lonDriverStatus.TxState = LdvTxPayload;
        }       
    }
    /* Else end of transmission of a packet, nothing needs to be done */
}




/* 
 * Interrupt handler function for the receiver and transmitter interrupts
 */



void OnRxData(/*uint8 rxChar*/)
{
    uint32 i;
    bool rxDone = false;

    while ((LPC_UART0->LSR & LSR_RDR) && !rxDone)
    {
//        if (_lonDriverStatus.SuspendReceive)
//            return;
        
        uint8 rxChar = LPC_UART0->RBR;
        
        if (_lonDriverStatus.DriverState == LdvDriverNormal)
        {
            /* Driver is in good shape, handle the data */
            switch (_lonDriverStatus.RxState) 
            {
                case LdvRxIdle:
                    /* Length byte has arrived */
                    /* Initially assume no receive buffer is available */
                    _lonDriverStatus.RxState = LdvRxIgnore;
                    
                    /* rxChar contains the length byte, total payload size is length byte + 1 byte for the command */
                    _lonDriverStatus.RxPayloadLen = rxChar + 1; 
                    _lonDriverStatus.RxTimeout = LDV_RXTIMEOUT;
        
                    if (rxChar < (LDV_RXBUFSIZE - sizeof(LonSmipHdr)))
                    {
                        /* Find an empty buffer to store this data */
                        for (i = 0; i < LDV_RXBUFCOUNT; i++)
                        {
                            LonByte rcvIndex = _lonDriverStatus.RxBufferReceiveIndex;
                            if (_lonRxBuffer[rcvIndex].State == LdvRxBufferEmpty)
                            {
                                /* Found an empty buffer */
                                _lonDriverStatus.RxNextFree = 0;                        /* Initialize the buffer receive counter */
                                _lonDriverStatus.RxState = LdvRxPayload;                /* Change the driver receive state */
                                _lonRxBuffer[rcvIndex].State = LdvRxBufferReceiving;    /* Change the buffer state */
                                _lonRxBuffer[rcvIndex].Data[_lonDriverStatus.RxNextFree++] = rxChar;  /* Store the data in the buffer */
                                
                                break;
                            }
                            CyclicIncrement(LdvIndexRxBufferReceive);
                        }
                    }
                    break;
        
                case LdvRxPayload:
                {
                    /* Payload is arriving, keep pushing data in the receive buffer */
                    LonByte rcvIndex = _lonDriverStatus.RxBufferReceiveIndex;
                    _lonRxBuffer[rcvIndex].Data[_lonDriverStatus.RxNextFree++] = rxChar;
                    /* Check if all the bytes have been received */
                    if (--_lonDriverStatus.RxPayloadLen == 0)
                    {
                        /* All bytes have been received */               
                        #ifdef PRINT_LINK_LAYER
                        PrintData((LonByte*) &_lonRxBuffer[rcvIndex].Data[0], _lonDriverStatus.RxNextFree, 0, rcvIndex);
                        #endif
                        
                        _lonDriverStatus.RxState = LdvRxIdle;
                        _lonRxBuffer[rcvIndex].State = LdvRxBufferReady;
                        _lonDriverStatus.RxTimeout = 0;
                        CyclicIncrement(LdvIndexRxBufferReceive);
        
                        /* Check if there is still an empty receive buffer */
                        for (i = 0; i < LDV_RXBUFCOUNT; i++) 
                            if (_lonRxBuffer[i].State == LdvRxBufferEmpty)
                                /* There is an empty receive buffer, we are done */
                                break;
                        /* If there is no empty receive buffer, quench the neuron */
                        if (i == LDV_RXBUFCOUNT)
                            DEASSERT_HRDY();    
                    }
                    else
                    {
                        _lonDriverStatus.RxTimeout = LDV_RXTIMEOUT;
                    }
                    break;
                }
                    
                case LdvRxIgnore:
                    /* There was no receive buffer available to store this message */
                    /* Keep ignoring till the full message has arrived */
                    if (--_lonDriverStatus.RxPayloadLen == 0) 
                    {
                        _lonDriverStatus.RxState = LdvRxIdle;
                        _lonDriverStatus.RxTimeout = 0;
                    }
                    else
                    {
                        _lonDriverStatus.RxTimeout = LDV_RXTIMEOUT;
                    }
                    break;
        
                default:
                    break;
            }
                /* Restart the keep-alive timer */
            _lonDriverStatus.KeepAliveTimeout = LDV_KEEPALIVETIMEOUT;
        }
        else
        {
            rxDone = true;
        }   
    }
}

#ifdef LON
#ifndef BOOTLOADER
void UART0_IRQHandler(void)
#else
void UART0_IRQHandlerBoot(void)
#endif
{
    uint32 uart0Status;
    uint32 IIRValue, LSRValue;
    volatile uint32 Dummy;
    
    IIRValue = LPC_UART0->IIR;
    
    IIRValue >>= 1;			        /* skip pending bit in IIR */
    IIRValue &= 0x07;			    /* check bit 1~3(shifter 0~2), interrupt identification */

    switch (IIRValue)
    {
        /* Receive Line Status */
        case IIR_RLS:		
            LSRValue = LPC_UART0->LSR;
            /* Receive Line Status */
            if (LSRValue & (LSR_OE | LSR_PE | LSR_FE | LSR_RXFE | LSR_BI))
            {
                /* There are errors or break interrupt */
                /* Read LSR will clear the interrupt */
                uart0Status = LSRValue;
                
                if (LSRValue & LSR_RDR) {
                    Dummy = LPC_UART0->RBR;	/* Dummy read on RX to clear 
                                        interrupt, then bail out */
                }
                
//                if (LSRValue & LSR_OE)
//                    UART_Debug("Overrun ");
//                if (LSRValue & LSR_PE)
//                    UART_Debug("Parity ");
//                if (LSRValue & LSR_FE)
//                    UART_Debug("Framing ");
//                if (LSRValue & LSR_RXFE)
//                    UART_Debug("RX Framing ");
//                if (LSRValue & LSR_BI)
//                    UART_Debug("BI ");
//                
//                UART_Debug("error on: ");
//                UART_DebugHex(Dummy);
//                UART_Debug("\n");
                
                _nRxErrors++;
                ResetMicroServer();
                
                return;
            }
            
            if (LSRValue & LSR_RDR)	/* Receive Data Ready */			
            {
                /* If no error on RLS, normal ready, save into the data buffer. */
                /* Note: read RBR will clear the interrupt */                
                OnRxData();
            }
            break;
    
        /* Receive Data Available */
        case IIR_RDA:	
            OnRxData();
            break;

        /* Character timeout indicator */
        case IIR_CTI:	
            uart0Status |= 0x100;		/* Bit 9 as the CTI error */
            break;
        
        /* Transmitter Holding Register Empty. */
        case IIR_THRE:
            OnTxReady();
            break;
    }
}
#endif // LON

/*
 * Interrupt handler function for the periodic interval timer
 */
//#ifndef BOOTLOADER
//void CT32B1_IRQHandler(void)
//#else
//void CT32B1_IRQHandlerBoot(void)
//#endif

void OneMsTimerHandler()
{    
    if (!_lonDriverStatus.TimerEnabled)
        return;
    
    LonUbits8 i;

    
    
    /* First, handle the utility timers */
    for (int i = 0; i < 4; i++)
    {
        if (_nUtilityUpTimer[i] < MAX_UNSIGNED)
            _nUtilityUpTimer[i]++;
        if (_nUtilityDownTimer[i])
            _nUtilityDownTimer[i]--;
    }
    
    /* Check the receiver timer */
    if (_lonDriverStatus.RxTimeout != 0) 
    {
        /* It means this timer is set */
        /* Decrement one unit and see if it is supposed to expire */
        if (--_lonDriverStatus.RxTimeout == 0) 
        {
            /* Receive timer has expired! */
            _nRxTimeout++;
            /* The driver and the Micro Server may have become out of sync */
            /* Reset the Micro Server and start over. Once the Micro Server resets, 
               the uplink reset message will come in and it will reset this driver also  */
            //UART_DebugT("Rx timeout\n");
            
            _lonDriverStatus.SuspendTransmittion = true;
            _lonDriverStatus.SuspendReceive = true;
            

            // CLEAR TEST START
            _lonDriverStatus.TxState        = LdvTxIdle;
            _lonDriverStatus.RxState        = LdvRxIdle;
            
            /* Abort ongoing receive transactions if any */
            for (i = 0; i < LDV_RXBUFCOUNT; i ++) 
                if (_lonRxBuffer[i].State == LdvRxBufferReceiving)
                    _lonRxBuffer[i].State = LdvRxBufferEmpty;
            /* Restart ongoing transmit transactions if any */
            for (i = 0; i < LDV_TXBUFCOUNT; i ++) 
                if (_lonTxBuffer[i].State == LdvTxBufferTransmitting)
                    _lonTxBuffer[i].State = LdvTxBufferReady;
            // CLEAR TEST END
            
            
            ResetMicroServer();
        }
    } 

    /* Check the driver sleep timer */
    if (_lonDriverStatus.DrvWakeupTime != 0) 
    {
        /* It means this timer is set */
        /* Decrement one unit and see if it is supposed to expire */
        if (--_lonDriverStatus.DrvWakeupTime == 0) 
        {
            /* The timer has expired. Wake up the driver */
            _lonDriverStatus.DriverState    = LdvDriverNormal;
            _lonDriverStatus.TxState        = LdvTxIdle;
            _lonDriverStatus.RxState        = LdvRxIdle;
            
            _lonDriverStatus.SuspendTransmittion = false;
            _lonDriverStatus.SuspendReceive = false;
            
            /* Abort ongoing receive transactions if any */
            for (i = 0; i < LDV_RXBUFCOUNT; i ++) 
                if (_lonRxBuffer[i].State == LdvRxBufferReceiving)
                    _lonRxBuffer[i].State = LdvRxBufferEmpty;
            /* Restart ongoing transmit transactions if any */
            for (i = 0; i < LDV_TXBUFCOUNT; i ++) 
                if (_lonTxBuffer[i].State == LdvTxBufferTransmitting)
                    _lonTxBuffer[i].State = LdvTxBufferReady;

            /* Clear interrupts if any, by reading the status register */
            uint32 IIRValue, LSRValue;
            IIRValue = LPC_UART0->IIR;
            LSRValue = LPC_UART0->LSR;
            // dummy operation to make sure compiler does not do some magic
            IIRValue = LSRValue;
            LSRValue = IIRValue;        
                        
            ENABLE_RX_INT();        /* Enable receive interrupt */
            ASSERT_HRDY();          /* Resume Neuron */
            
            LonDriverWokeUp();      // my added event to notify upper layer that driver is up & ready
        }
    }
    
    /* Check the keep-alive timer */
    if (_lonDriverStatus.KeepAliveTimeout != 0)
    {
        if (--_lonDriverStatus.KeepAliveTimeout == 0)
        {
            /* Keep-alive timer has expired. Deassert and then assert the HRDY */
            DEASSERT_HRDY();
            SleepMs(1);
            ASSERT_HRDY();
            /* Restart the timer */
            _lonDriverStatus.KeepAliveTimeout = LDV_KEEPALIVETIMEOUT;
        }
    }
    
    /* Check the put message blocking timer */
    if (_lonDriverStatus.PutMsgTimeout != 0)
    {
        --_lonDriverStatus.PutMsgTimeout;
    }
}

#endif // LON
