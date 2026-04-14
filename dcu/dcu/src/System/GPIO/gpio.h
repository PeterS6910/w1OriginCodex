/*****************************************************************************
 *   gpio.h:  Header file for NXP LPC1xxx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.09.01  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#ifndef __GPIO_H 
#define __GPIO_H

#include "System\baseTypes.h"

#define PORT0			0
#define PIO0            0
#define PORT1			1
#define PIO1            1
#define PORT2			2
#define PIO2            2

#define GPIO_OUTPUT  1
#define GPIO_INPUT   0

void GPIO_IRQHandler(void);

void GPIO_Init(void);

void GPIO_SetDir(uint32 portNum, uint32 bitPosi, uint32 dir);

void GPIO_SetValue(uint32 portNum, uint32 bitPosi, uint32 bitVal);

int32 GPIO_GetValue(uint32 portNum, uint32 bitPosi);

void GPIO_SetInterrupt(uint32 portNum, uint32 bitPosi, uint32 sense,
		uint32 single, uint32 event);

void GPIO_IntEnable(uint32 portNum, uint32 bitPosi);

void GPIO_IntDisable(uint32 portNum, uint32 bitPosi);
        uint32 GPIOIntStatus(uint32 portNum, uint32 bitPosi);

uint32 GPIO_IntStatus(uint32 portNum, uint32 bitPosi);

void GPIO_IntClear(uint32 portNum, uint32 bitPosi);

void PIO0_IRQHandlerBoot(void);

#endif /* end __GPIO_H */
/*****************************************************************************
**                            End Of File
******************************************************************************/
