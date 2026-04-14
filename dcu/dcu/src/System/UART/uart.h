/*****************************************************************************
 *   uart.h:  Header file for NXP LPC1xxx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.08.21  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#ifndef __UART_H
#define __UART_H

#include "System\LPC122x.h"
#include "System\baseTypes.h"

#define IER_RBRIE	(1 << 0)
#define IER_THREIE	(1 << 1)
#define IER_RXLIE	(1 << 2)

#define IIR_PEND	0x01
#define IIR_RLS		0x03
#define IIR_RDA		0x02
#define IIR_CTI		0x06
#define IIR_THRE	0x01

#define LSR_RDR		0x01
#define LSR_OE		0x02
#define LSR_PE		0x04
#define LSR_FE		0x08
#define LSR_BI		0x10
#define LSR_THRE	0x20
#define LSR_TEMT	0x40
#define LSR_RXFE	0x80

#define RX_BUFSIZE	256
#define TX_BUFSIZE      256

#define CR_PORT         1
#define COMM_PORT       0

#ifndef BOOTLOADER

#ifndef LON
void UART0_IRQHandler(void);
#endif

void UART1_IRQHandler(void);

#else   // for bootloader

void UART0_IRQHandlerBoot(void);

#endif

typedef int32 (*DUartDataHandler)(byte* buffer, uint32 size, uint32 timestamp);

extern volatile byte  _UART0_RxBuffer[RX_BUFSIZE];
extern volatile uint32 _UART0_RxCount;

void UART_DisableUART();

void UART_EnableUART();

void UART_ClearInterrupts(LPC_UART_TypeDef* uartHandler);

void UART_Init(uint32 port, uint32 baudrate);

unsigned char volatile* UART_GetTxBuffer();

void UART_Send(uint32 port, byte* data, uint32 length);

void UART_Receive(uint32 port, uint32 triggerCount, DUartDataHandler dataHandler);

#ifndef BOOTLOADER
uint32 UART_GetBaudRate(uint32 port);
#endif


void UART_DebugT(char* string);
void UART_Debug(char* string);
void UART_DebugExtended(uint32 port, char* string, uint32 ownBaudRate);
void UART_DebugHex(uint32 value);
void UART_DebugChar(char oneChar);
void UART_DebugArray(uint8* array, int length);

//uint32 UART_GetLastUart1ReChange();


#endif /* end __UART_H */
/*****************************************************************************
**                            End Of File
******************************************************************************/
