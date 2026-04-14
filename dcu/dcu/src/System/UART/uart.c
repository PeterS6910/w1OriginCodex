#pragma diag_suppress=Pa082
/*****************************************************************************
 *   uart.c:  UART API file for NXP LPC11xx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.08.21  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#include "System\LPC122x.h"
#include "System\Core\system_LPC122x.h"
#include "System\Core\ipr.h"
#include "uart.h"
#include "System\Timer\timer16.h"
#include "System\Timer\sysTick.h"
#include "System\baseTypes.h"
#include "System\GPIO\gpio.h"
#include "stdarg.h"
#include "IO\Outputs.h"

#define RS485_ENABLED		0
#define TX_INTERRUPT		0		/* 0 if TX uses polling, 1 interrupt driven. */
#define MODEM_TEST			0

#define DIR485CR                18              /* Port id for card reader rs485 direction */

/* RS485 mode definition. */
#define RS485_NMMEN		(0x1 << 0)
#define RS485_RXDIS		(0x1 << 1)
#define RS485_AADEN		(0x1 << 2)
#define RS485_SEL		(0x1 << 3)
#define RS485_DCTRL		(0x1 << 4)
#define RS485_OINV		(0x1 << 5)

#define IMPLICIT_BAUDRATE       9600
#define TX_FLUSH_COUNT          16

volatile uint32 _UART0_Status;
volatile uint32 _UART0_RxCount = 0;
volatile byte  _UART0_RxBuffer[RX_BUFSIZE]; 
//= { 0xFE, 0xEF, 0x01, 0x30, 0x01, 0x00, 0x23, 0x5D, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x02, 0x01 };
//FE EF 01 30 01 00 23 5D 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 02 01
volatile uint32 _UART0_TxCount = 0;
volatile byte  _UART0_TxBuffer[TX_BUFSIZE];
volatile uint32 _UART0_Timestamp;

#ifndef BOOTLOADER
volatile uint32 _UART1_Status;
volatile uint32 _UART1_RxCount = 0;
volatile byte  _UART1_RxBuffer[RX_BUFSIZE];
volatile uint32 _UART1_TxCount = 0;
volatile byte  _UART1_TxBuffer[TX_BUFSIZE];
volatile uint32 _UART1_Timestamp;
#endif

#ifdef DEBUG
/*volatile uint32 _uart1LastReChange = 0;
volatile uint32 _uart1LastReElapsed = 0;

uint32  UART_GetLastUart1ReChange() {
    uint32 val = _uart1LastReElapsed;
    _uart1LastReElapsed = 0;
    return val;
}*/
#endif


volatile uint32 _uartActualBaudrate[2];

static __INLINE void EnableReceiveInterrupt(uint32 port) {
#ifndef BOOTLOADER
    if (port == 0)
#endif
        LPC_UART0->IER = IER_RXLIE | IER_RBRIE | IER_THREIE;
#ifndef BOOTLOADER
    else
        LPC_UART1->IER = IER_RXLIE | IER_RBRIE | IER_THREIE;
#endif
}

static __INLINE void DisableReceiveInterrupt(uint32 port) {
#ifndef BOOTLOADER
    if (port == 0)
#endif
        LPC_UART0->IER = IER_RXLIE;
#ifndef BOOTLOADER
    else
        LPC_UART1->IER = IER_RXLIE;
#endif
}

/* FWD declaration */
void UART_FlushData(uint32 port);

#ifndef BOOTLOADER
uint32 UART_GetBaudRate(uint32 port) {
    switch(port) {
        case CR_PORT:
        case COMM_PORT:
            return _uartActualBaudrate[port];
        default:
            return 0;
    }
}
#endif

/*
volatile int _allReceivedData = 0;
volatile int _allFaultyReceivedData = 0;
volatile int _allUartInterruptCalls = 0;
*/

/*****************************************************************************
** Function name:		UART0_IRQHandler
**
** Descriptions:		UART interrupt handler
**
** parameters:			None
** Returned value:		None
** 
*****************************************************************************/
#ifndef LON

#ifndef BOOTLOADER
void UART0_IRQHandler(void)
#else
void UART0_IRQHandlerBoot(void)
#endif // BOOTLOADER
{
    //_allUartInterruptCalls++;
    
    uint32 IIRValue, LSRValue;
    volatile uint32 Dummy;
    //Dummy = Dummy;
    
    IIRValue = LPC_UART0->IIR;
    
    IIRValue >>= 1;			/* skip pending bit in IIR */
    IIRValue &= 0x07;			/* check bit 1~3(shifter 0~2), interrupt identification */
    if (IIRValue == IIR_RLS)		/* Receive Line Status */
    {
        LSRValue = LPC_UART0->LSR;
        /* Receive Line Status */
        if (LSRValue & (LSR_OE | LSR_PE | LSR_FE | LSR_RXFE | LSR_BI))
        {
            /* There are errors or break interrupt */
            /* Read LSR will clear the interrupt */
            _UART0_Status = LSRValue;
            
            if (LSRValue & LSR_RDR) {
                Dummy = LPC_UART0->RBR;	/* Dummy read on RX to clear 
                                    interrupt, then bail out */
                //Dummy *= Dummy + 10;
            }
            
            //_allReceivedData++;
            //_allFaultyReceivedData++;
            
            
            return;
        }
        if (LSRValue & LSR_RDR)	/* Receive Data Ready */			
        {
            /* If no error on RLS, normal ready, save into the data buffer. */
            /* Note: read RBR will clear the interrupt */
    
            while (LPC_UART0->LSR & LSR_RDR)
            {        
                uint32 oneByte = LPC_UART0->RBR;
                
                if (_UART0_RxCount < RX_BUFSIZE - 1)
                    _UART0_RxBuffer[_UART0_RxCount++] = oneByte;
                
                /* Store the time */
                _UART0_Timestamp = SysTick_GetTickCount();    
            }
        }
    }
    else if (IIRValue == IIR_RDA)	/* Receive Data Available */
    {
        while (LPC_UART0->LSR & LSR_RDR)
        {
            /* Receive Data Available */
            uint32 oneByte = LPC_UART0->RBR;
            //_allReceivedData++;
            
            if (_UART0_RxCount < RX_BUFSIZE - 1)
                _UART0_RxBuffer[_UART0_RxCount++] = oneByte;
            
            /* Store the time */
            _UART0_Timestamp = SysTick_GetTickCount();    
        }
    }
    else if (IIRValue == IIR_CTI)	/* Character timeout indicator */
    {
        /* Character Time-out indicator */
        _UART0_Status |= 0x100;		/* Bit 9 as the CTI error */
    }    
    else if (IIRValue == IIR_THRE)  /* Transmitter Holding Register Empty. */
    {
        UART_FlushData(COMM_PORT);
    }
    
    //return;
}
#endif // LON

#ifndef BOOTLOADER
void UART1_IRQHandler(void)
{
    uint32 IIRValue, LSRValue;
    volatile uint32 Dummy;
    //Dummy = Dummy;
    
    IIRValue = LPC_UART1->IIR;          /* this also clears the interrupt */
    
    IIRValue >>= 1;			/* skip pending bit in IIR */
    IIRValue &= 0x07;			/* check bit 1~3, interrupt identification */
    if (IIRValue == IIR_RLS)		/* Receive Line Status */
    {
        LSRValue = LPC_UART1->LSR;
        /* Receive Line Status */
        if (LSRValue & (LSR_OE | LSR_PE | LSR_FE | LSR_RXFE | LSR_BI))
        {
            /* There are errors or break interrupt */
            /* Read LSR will clear the interrupt */
            _UART1_Status = LSRValue;
            if (LSRValue & LSR_RDR) {
                Dummy = LPC_UART1->RBR;	/* Dummy read on RX to clear 
                                    interrupt, then bail out */
                //Dummy *= Dummy + 10;
            }
            
            return;
        }
        if (LSRValue & LSR_RDR)	/* Receive Data Ready */			
        {
            /* If no error on RLS, normal ready, save into the data buffer. */
            /* Note: read RBR will clear the interrupt */
            while (LPC_UART1->LSR & LSR_RDR)
            {
                /* Receive Data Available */
                uint32 oneByte = LPC_UART1->RBR;
                //_allReceivedData++;
                
                if (_UART1_RxCount < RX_BUFSIZE - 1)
                    _UART1_RxBuffer[_UART1_RxCount++] = oneByte;
                
                /* Store the time */
                _UART1_Timestamp = SysTick_GetTickCount();    
            }
        }
    }
    else if (IIRValue == IIR_RDA)	/* Receive Data Available */
    {
        while (LPC_UART1->LSR & LSR_RDR)
        {
            /* Receive Data Available */
            uint32 oneByte = LPC_UART1->RBR;
            //_allReceivedData++;
            
            if (_UART1_RxCount < RX_BUFSIZE - 1)
                _UART1_RxBuffer[_UART1_RxCount++] = oneByte;
            
            /* Store the time */
            _UART1_Timestamp = SysTick_GetTickCount();    
        }
    }
    else if (IIRValue == IIR_CTI)	/* Character timeout indicator */
    {
        /* Character Time-out indicator */
        _UART1_Status |= 0x100;		/* Bit 9 as the CTI error */
    }    
    else if (IIRValue == IIR_THRE)  /* Transmitter Holding Register Empty. */
    {
        UART_FlushData(CR_PORT);
    }
    
    //return;
}



void UART_DebugExtended(uint32 port, char* string, uint32 ownBaudRate)
{
#ifdef NO_CR_DEBUG 
    switch(port) {
        case CR_PORT:
        case COMM_PORT:
            {
                uint32 oldBaudrate = _uartActualBaudrate[port];        
                
                if (ownBaudRate <=0)
                    ownBaudRate = 115200;
                
                if (oldBaudrate != ownBaudRate)
                     UART_Init(port, ownBaudRate);
                
                UART_Send(port, (byte*)string, strlen(string));
                
                if (oldBaudrate > 0)
                    UART_Init(port, oldBaudrate);
            }
            break;
    }
#endif
}

#ifdef NO_CR_DEBUG
static void TimeStamp()
{
    UART_Debug("\n");
    
    char buffer[32];
    
    itoa(SysTick_GetHour(), buffer, sizeof(buffer), true);
    UART_Debug(buffer);
    UART_Debug(":");

    itoa(SysTick_GetMinute(), buffer, sizeof(buffer), true);
    UART_Debug(buffer);
    UART_Debug(":");
    
    itoa(SysTick_GetSecond(), buffer, sizeof(buffer), true);
    UART_Debug(buffer);
    UART_Debug(" ");
}
#endif

void UART_DebugT(char* string)
{
#ifdef NO_CR_DEBUG    
    TimeStamp();
    UART_Debug(string);
#endif
}

void UART_Debug(char* string) {
#ifdef NO_CR_DEBUG    
    UART_DebugExtended(1, string, 115200);
#endif
}

void UART_DebugChar(char oneChar)
{
#ifdef NO_CR_DEBUG
    char str[1] = { oneChar };
    UART_Debug(str);
#endif
}

void UART_DebugArray(uint8* array, int length)
{
#ifdef NO_CR_DEBUG
    for (int i = 0; i < length; i++)
    {
        UART_DebugHex(array[i]);
        UART_Debug(" ");
    }
#endif
}

void UART_DebugHex(uint32 value)
{
#ifdef NO_CR_DEBUG
    char hexChars[] = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
    char sByte[3];
    sByte[0] = hexChars[(value >> 4) & 0x0f];
    sByte[1] = hexChars[value & 0x0f];
    sByte[2] = 0;
    UART_Debug(sByte);
#endif
}
#endif // BOOTLOADER

/*****************************************************************************
** Function name:		UARTSend
**
** Descriptions:		Send a block of data to the UART 0 port based
**				on the data length
**
** parameters:		buffer pointer, and data length
** Returned value:	None
** 
*****************************************************************************/
void UART_Send(uint32 port, byte* data, uint32 length)
{
    uint32 i = 0;

    if (length == 0)
        return;
    
#ifndef BOOTLOADER
    switch(port) {
        case CR_PORT:
        case COMM_PORT:
            break;      
        default:
            return;
    }
#else
    port = COMM_PORT;
#endif
    
    if (_uartActualBaudrate[port] == 0)
        UART_Init(port, IMPLICIT_BAUDRATE);

#ifndef BOOTLOADER    
    if (port == COMM_PORT)
#endif
    {
        /* Exit if there is not space to store the data */
        if (_UART0_TxCount + length >= TX_BUFSIZE) {
#ifdef DEBUG
        	__ASM("NOP");
#endif
            return;
        }
        /* Copy data to the tx buffer */
        for (i = 0; i < length; i++)
        {
            _UART0_TxBuffer[i + _UART0_TxCount] = data[i];
        }
        _UART0_TxCount += length;
        
        UART_FlushData(port);
        
        return;
    }
    
#ifndef BOOTLOADER
    if (port == CR_PORT)
    {
        /* Exit if there is not space to store the data */
        if (_UART1_TxCount + length >= TX_BUFSIZE) {
#ifdef DEBUG
        	__ASM("NOP");
#endif
            return;
        }
        
        /* Copy data to the tx buffer */
        for (i = 0; i < length; i++)
        {
            _UART1_TxBuffer[i + _UART1_TxCount] = data[i];
        }
        _UART1_TxCount += length;
        
        UART_FlushData(port);
                
        return;
    }
#endif
    
}

void UART_FlushData(uint32 port)
{   
    //DisableReceiveInterrupt(port);	
#ifndef BOOTLOADER
    if (port == CR_PORT)
        NVIC_DisableIRQ(UART1_IRQn);
    else if (port == COMM_PORT)
#endif
        NVIC_DisableIRQ(UART0_IRQn);
    
    uint32 i;
    uint32 txRaceCondition;
    
    LPC_UART_TypeDef* uartHandler;
    volatile uint32* txCount;
    volatile byte* txBuffer;
    
#ifndef BOOTLOADER
    if (port == CR_PORT)
    {
        uartHandler = LPC_UART1;
        txCount = &_UART1_TxCount;
        txBuffer = _UART1_TxBuffer;
    }
    if (port == COMM_PORT)
#endif
    {
        uartHandler = LPC_UART0;
        txCount = &_UART0_TxCount;
        txBuffer = _UART0_TxBuffer;
    }
    
    /* Check if thr is empty */
    if (uartHandler->LSR & LSR_THRE)
    {
        /* store data into tx fifo */
        uint32 dataToSend = (*txCount > TX_FLUSH_COUNT) ? TX_FLUSH_COUNT : *txCount;
        /* if there are data to send -> raise the direction signal */
        if (dataToSend > 0)
        {
#ifndef BOOTLOADER
            if (port == CR_PORT)
            {
                GPIO_SetValue(PORT0, DIR485CR, 1);
#ifdef DEBUG
                //_uart1LastReChange = SysTick_GetTickCount();
#endif // DEBUG
            }
#endif // BOOTLOADER
            
            for (i = 0; i < dataToSend; i++)
            {
                txRaceCondition = 0;
                while (!(uartHandler->LSR & LSR_THRE))
                {
                    if (txRaceCondition == 0x7FFF) 
                        break;
                    else
                        txRaceCondition++;
                }
                
                uartHandler->THR = txBuffer[i];
            }
            /* shift the data in the transmit buffer if neccessary */
            if (*txCount > TX_FLUSH_COUNT)
            {
                for (i = dataToSend; i < *txCount; i++)
                {
                    txBuffer[i - dataToSend] = txBuffer[i];
                }
            }
            *txCount -= dataToSend;
            
        }
        /* transmittion over -> lower the direction signal */
#ifndef BOOTLOADER
        else     
        {
            if (port == CR_PORT) 
            {
                txRaceCondition = 0;
                /* Wait until data are gone even from TSR and then change direction */
                while (!(uartHandler->LSR & LSR_TEMT))
                {
                    if (txRaceCondition == 0x7FFF) {
                        txRaceCondition = 0xFFFF;
                        break;
                    }
                    else
                        txRaceCondition++;
                }
                
                /*
                switch(_uartActualBaudrate[CR_PORT]) {
                    case 19200:
                        for(int i=0;i<0x7FF;i++)
                            asm("NOP");
                        break;
                    case 4800:
                       for(int i=0;i<0xFFF;i++)
                            asm("NOP");

                        break;
                }*/
                 
                GPIO_SetValue(PORT0, DIR485CR, 0);
#ifdef DEBUG
                
                //_uart1LastReElapsed = SysTick_GetElapsedTime(_uart1LastReChange);
                //_uart1LastReChange = SysTick_GetTickCount();
                
                txRaceCondition *= txRaceCondition + 3;
#endif // DEBUG
                
                
            }
        }
#endif // BOOTLOADER
    }
    else
    {
    	__ASM("NOP");
    }
    
//    EnableReceiveInterrupt(port);
#ifndef BOOTLOADER
    if (port == CR_PORT)
        NVIC_EnableIRQ(UART1_IRQn);
    else if (port == COMM_PORT)
#endif
        NVIC_EnableIRQ(UART0_IRQn);
}


//volatile int _allSizeDirect = 0;

void UART_Receive(uint32 port, uint32 triggerCount, DUartDataHandler dataHandler) {

    if (null == dataHandler || 
        port != CR_PORT && port != COMM_PORT)
        return;
    
    if (triggerCount < 1)
        triggerCount = 1;
    
    uint32 countSnapshot;
    uint32* rxCount;
    byte* rxBuffer;
    uint32 currentTimestamp;

#ifndef BOOTLOADER    
    switch(port) {

        case CR_PORT:
            if (_uartActualBaudrate[port] == 0)
                UART_Init(port, IMPLICIT_BAUDRATE);    
            countSnapshot = _UART1_RxCount;
    
            rxCount = (uint32*)&_UART1_RxCount;
            rxBuffer = (byte*)_UART1_RxBuffer;
            currentTimestamp = _UART1_Timestamp;
            break;

        case COMM_PORT:
#endif
            if (_uartActualBaudrate[port] == 0)
                UART_Init(port, IMPLICIT_BAUDRATE);    
            countSnapshot = _UART0_RxCount;
            rxCount = (uint32*)&_UART0_RxCount;
            rxBuffer = (byte*)_UART0_RxBuffer;
            currentTimestamp = _UART0_Timestamp;
#ifndef BOOTLOADER
            break;
        default:
            return;
    }
#endif
    
    //if (countSnapshot == 0) // OPTIM - NO NEED FOR THIS CONDITION, AS IN NEXT CONDITION triggerCount will be alway >=1
    //    return;
        
    if (countSnapshot >= triggerCount) {
        
        if (dataHandler(rxBuffer, countSnapshot, currentTimestamp) == 0) {
            
            // ENTER CRITICAL SECTION
            DisableReceiveInterrupt(port);
            
            if (*rxCount > countSnapshot) 
            {
                // otherwise leave the RX buffer to be filled from current position
                //*previousCount = countSnapshot;
                
                uint32 diff= *rxCount - countSnapshot;
                
                for (int i=0;i<diff;i++)
                    rxBuffer[i] = rxBuffer[countSnapshot+i];
                
                //_allSizeDirect += countSnapshot;
                *rxCount = diff;
                
            }
            else {
                //_allSizeDirect += *rxCount;
                *rxCount = 0;
            }
            
            // enable rcv interrupt
            EnableReceiveInterrupt(port);
            // LEAVE CRITICAL SECTION
        }
        else {
            // UART Count is not shifted, therefore we're waiting for further data   
           
            
            // anti-hanging condition
            // if there is no space to store data, there's no reason to wait more
            if (countSnapshot == RX_BUFSIZE) {
                *rxCount = 0;
            }
        }
    }
    
}

void UART_CalculateFDRValues(uint32 iBaudrate, uint32 iPCLK, 
                             uint32* divAddVal, uint32* mulVal, uint32* pDLest) 
{ 
    const float fFractionalTable[72] =
    {
        1.000, 1.250, 1.500, 1.750,
        1.067, 1.267, 1.533, 1.769,
        1.071, 1.273, 1.538, 1.778,
        1.077, 1.286, 1.545, 1.786,
        1.083, 1.300, 1.556, 1.800,
        1.091, 1.308, 1.571, 1.818,
        1.100, 1.333, 1.583, 1.833,
        1.111, 1.357, 1.600, 1.846,
        1.125, 1.364, 1.615, 1.857,
        1.133, 1.375, 1.625, 1.867,
        1.143, 1.385, 1.636, 1.875,
        1.154, 1.400, 1.643, 1.889,
        1.167, 1.417, 1.667, 1.900,
        1.182, 1.429, 1.692, 1.909,
        1.200, 1.444, 1.700, 1.917,
        1.214, 1.455, 1.714, 1.923,
        1.222, 1.462, 1.727, 1.929,
        1.231, 1.467, 1.733, 1.933
    };
    
    const byte iDivisionAddTable[72] =
    {
        0, 1, 1, 3,
        1, 4, 8, 10,
        1, 3, 7, 7,
        1, 2, 6, 11,
        1, 3, 5, 4,
        1, 4, 4, 9,
        1, 1, 7, 5,
        1, 5, 3, 11,
        1, 4, 8, 6,
        2, 3, 5, 13,
        1, 5, 7, 7,
        2, 2, 9, 8,
        1, 5, 2, 9,
        2, 3, 9, 10,
        1, 4, 7, 11,
        3, 5, 5, 12,
        2, 6, 8, 13,
        3, 7, 11, 14
    };

    const byte iMultipleTable[72] =
    {
        1, 4, 2, 4,
        15, 15, 15, 13,
        14, 11, 13, 9,
        13, 7, 11, 14,
        12, 10, 9, 5,
        11, 13, 7, 11,
        10, 3, 12, 6,
        9, 14, 5, 13,
        8, 11, 13, 7,
        15, 8, 8, 15,
        7, 13, 11, 8,
        13, 5, 14, 9,
        6, 12, 3, 10,
        11, 7, 13, 11,
        5, 9, 10, 12,
        14, 11, 7, 13,
        9, 13, 11, 14,
        13, 15, 15, 15,
    };
    
    // find FR estimated
    float frEstimated;
    for (frEstimated = 1.1; frEstimated < 1.9; frEstimated += 0.1) 
    {
        int iDLest = (int)(iPCLK / (16 * iBaudrate * frEstimated));
        float frTested = (float)iPCLK / (16 * iBaudrate * iDLest);
        
        if (frTested > 1.1 && frTested < 1.9)
        {
            frEstimated = frTested;
            *pDLest = iDLest;
            break;
        }
    }
    
    // find closest table value
    float fSmallestDiff = 10; // starting with impossibly high difference
    int iPos = 0;

    for (int i = 0; i < 72; i++) 
    {
        float fNewDiff = fFractionalTable[i] - frEstimated;
        if (fNewDiff < 0) 
            fNewDiff = -fNewDiff; // abs value

        if (fNewDiff < fSmallestDiff)              
        {
            iPos = i;
            fSmallestDiff = fNewDiff;
        }
    }
    
    // return table values 
    *divAddVal = iDivisionAddTable[iPos];
    *mulVal = iMultipleTable[iPos];
}

void UART_ClearInterrupts(LPC_UART_TypeDef* uartHandler)
{
    /* Read to clear the line status. */
    uint32 regVal = uartHandler->LSR;
    
    /* Ensure a clean start, no data in either TX or RX FIFO. */
    while (uartHandler->LSR & (LSR_THRE | LSR_TEMT) != (LSR_THRE | LSR_TEMT));
    while (uartHandler->LSR & LSR_RDR)
        regVal = uartHandler->RBR;	/* Dump data from RX FIFO */
}

/*****************************************************************************
** Function name:		UARTInit
**
** Descriptions:		Initialize UART0 port, setup pin select,
**				clock, parity, stop bits, FIFO, etc.
**
** parameters:			Port id (0 or 1)
**                              UART baudrate
** Returned value:		None
** 
*****************************************************************************/
void UART_Init(uint32 port, uint32 baudrate)
{
    LPC_UART_TypeDef* uartHandler;
    
    if (baudrate == 0)
        baudrate = IMPLICIT_BAUDRATE;
    
    uint32 regVal;
    uint32 tmpVal;
    
#ifndef BOOTLOADER
    if (port == COMM_PORT)
#endif
    {
        uartHandler = LPC_UART0;
        // wait for tx buffer empty
        
        while (!(uartHandler->LSR & LSR_THRE))
        {
            tmpVal++;
            if (tmpVal > 1000000)
                break;
        }
                
        _uartActualBaudrate[port] = baudrate;
        _UART0_RxCount = 0;
        
        NVIC_DisableIRQ(UART0_IRQn);
        
        LPC_IOCON->PIO0_1 &= ~0x07;
        LPC_IOCON->PIO0_1 |= 0x02;  /* RXD0 */
        LPC_IOCON->PIO0_2 &= ~0x07;
        LPC_IOCON->PIO0_2 |= 0x02;  /* TXD0 */
        
        LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 12);
        LPC_SYSCON->UART0CLKDIV = 0x1;    
        
        regVal = LPC_SYSCON->UART0CLKDIV;
        
#ifndef LON
         /* Below is the direction control setting. */
        LPC_IOCON->PIO0_0 &= ~0x07;	
        LPC_IOCON->PIO0_0 |= 0x02;		/* UART_DIR or UART_RTS(HB1) as direction control. */
        uartHandler->RS485CTRL |= RS485_DCTRL; // enable auto direction control
        uartHandler->RS485CTRL |= RS485_OINV;  // invert direction control fnc
#endif
    }
#ifndef BOOTLOADER
    else
    {
        if (port == CR_PORT) 
        {
            uartHandler = LPC_UART1;

            while (!(uartHandler->LSR & LSR_THRE))
            {
                tmpVal++;
                if (tmpVal > 1000000)
                    break;
            }
            
            _uartActualBaudrate[port] = baudrate;
            _UART1_RxCount = 0;
            
            NVIC_DisableIRQ(UART1_IRQn);
            
            LPC_IOCON->PIO0_8 &= ~0x07;
            LPC_IOCON->PIO0_8 |= 0x02;  /* RXD1 */
            LPC_IOCON->PIO0_9 &= ~0x07;
            LPC_IOCON->PIO0_9 |= 0x02;  /* TXD1 */
            
            LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 13);
            LPC_SYSCON->UART1CLKDIV = 0x1;     
            
            regVal = LPC_SYSCON->UART1CLKDIV;
            
            // direction port for UART1 (rs485)
            GPIO_SetDir(PORT0, DIR485CR, GPIO_OUTPUT);
        }
        else
            return;
    }
#endif
    
    uartHandler->LCR = 0x83;             /* 8 bits, no Parity, 1 Stop bit */
    
    uint32 iDLest, iDivAddVal, iMulVal;
    
    UART_CalculateFDRValues(baudrate, _systemAHBFrequency / regVal, &iDivAddVal,
                            &iMulVal, &iDLest);
    
    uint32 dlm = (iDLest >> 8);
    uint32 dll = (iDLest & 0xff);
    uint32 fdr = iDivAddVal | (iMulVal << 4);
    
    uartHandler->DLM = dlm;							
    uartHandler->DLL = dll;
    uartHandler->FDR = fdr;       /* fractional divider register */
    uartHandler->LCR = 0x03;		/* DLAB = 0 */
    uartHandler->FCR = 0x07;		/* Enable and reset TX and RX FIFO. */
    
    /* Read to clear the line status. */
    regVal = uartHandler->LSR;
    
    /* Ensure a clean start, no data in either TX or RX FIFO. */
    while (uartHandler->LSR & (LSR_THRE | LSR_TEMT) != (LSR_THRE | LSR_TEMT));
    while (uartHandler->LSR & LSR_RDR)
        regVal = uartHandler->RBR;	/* Dump data from RX FIFO */
    
   
    
    /* Enable the UART Interrupt */
#ifndef BOOTLOADER
    if (port == COMM_PORT) 
#endif
    {
        NVIC_EnableIRQ(UART0_IRQn);
        // lower prio than systick
        NVIC_SetPriority(UART0_IRQn, UART_PRIO);
    }
#ifndef BOOTLOADER
    
    else
    {
        NVIC_EnableIRQ(UART1_IRQn);
        // lower prio than systick
        NVIC_SetPriority(UART1_IRQn, UART_PRIO);
    }
#endif
    
    EnableReceiveInterrupt(port);	
    
    //return;
}

/******************************************************************************
**                            End Of File
******************************************************************************/

#pragma diag_default=Pa082