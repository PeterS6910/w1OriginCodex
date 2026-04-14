/*****************************************************************************
 *   gpio.c:  GPIO C file for NXP LPC11xx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.07.20  ver 1.00    Prelimnary version, first Release
 *
*****************************************************************************/
#include "System\LPC122x.h"			/* LPC11xx Peripheral Registers */
#include "gpio.h"
#include "System\baseTypes.h"
#include "Communication\ClspLon\Driver\LdvSci.h"

/* Shadow registers used to prevent chance of read-modify-write errors */
/* Ultra-conservative approach... */
//volatile uint32_t _GPIOShadowPort2;



/*****************************************************************************
** Function name:		GPIOInit
**
** Descriptions:		Initialize GPIO, install the
**						GPIO interrupt handler
**
** parameters:			None
** Returned value:		true or false, return false if the VIC table
**						is full and GPIO interrupt handler can be
**						installed.
** 
*****************************************************************************/
void GPIO_Init(void)
{
    /* Enable AHB clock to the GPIO domain. */
    // this should be set by default anyway
//    LPC_SYSCON->SYSAHBCLKCTRL |= (uint32_t)(1 << 29);     /* GPIO2 - probably not used on this one */
    LPC_SYSCON->SYSAHBCLKCTRL |= (uint32_t)(1 << 30);     /* GPIO1 */
    LPC_SYSCON->SYSAHBCLKCTRL |= (uint32_t)(1 << 31);     /* GPIO0 */
    
    LPC_GPIO0->MASK = 0x00;
    LPC_GPIO1->MASK = 0x00;
//    LPC_GPIO2->MASK = 0x00;
    
//    /* Set up NVIC when I/O pins are configured as external interrupts. */
    NVIC_EnableIRQ(EINT0_IRQn);
//    NVIC_SetPriority(EINT0_IRQn, 1);
//    
//    NVIC_EnableIRQ(EINT1_IRQn);
//    NVIC_EnableIRQ(EINT2_IRQn);
//    NVIC_EnableIRQ(EINT3_IRQn);
    
    return;
}

/*****************************************************************************
** Function name:		GPIOSetDir
**
** Descriptions:		Set the direction in GPIO port
**
** parameters:			port num, bit position, direction (1 out, 0 input)
** Returned value:		None
** 
*****************************************************************************/
void GPIO_SetDir(uint32_t portNum, uint32_t bitPosi, uint32_t dir)
{
    /* if DIR is OUT(1), but GPIOx_DIR is not set, set DIR
    to OUT(1); if DIR is IN(0), but GPIOx_DIR is set, clr
    DIR to IN(0). All the other cases are ignored. 
    On port3(bit 0 through 3 only), no error protection if 
    bit value is out of range. */
    switch (portNum)
    {
        case PORT0:
            if (!(LPC_GPIO0->DIR & (0x1 << bitPosi)) && (dir == 1))
                LPC_GPIO0->DIR |= (0x1 << bitPosi);
            else if ((LPC_GPIO0->DIR & (0x1 << bitPosi)) && (dir == 0))
                LPC_GPIO0->DIR &= ~(0x1 << bitPosi);	  
            break;
        case PORT1:
            if (!(LPC_GPIO1->DIR & (0x1 << bitPosi)) && (dir == 1))
                LPC_GPIO1->DIR |= (0x1 << bitPosi);
            else if ((LPC_GPIO1->DIR & (0x1 << bitPosi)) && (dir == 0))
                LPC_GPIO1->DIR &= ~(0x1 << bitPosi);	  
            break;
/*
        case PORT2:
            if (!(LPC_GPIO2->DIR & (0x1<<bitPosi)) && (dir == 1))
                LPC_GPIO2->DIR |= (0x1 << bitPosi);
            else if ((LPC_GPIO2->DIR & (0x1 << bitPosi)) && (dir == 0))
                LPC_GPIO2->DIR &= ~(0x1 << bitPosi);	  
            break;
*/       
        default:
            break;
    }
    return;
}

/*****************************************************************************
** Function name:		GPIOSetValue
**
** Descriptions:		Set/clear a bitvalue in a specific bit position
**						in GPIO portX(X is the port number.)
**
** parameters:			port num, bit position, bit value
** Returned value:		None
** 
*****************************************************************************/
void GPIO_SetValue(uint32_t portNum, uint32_t bitPosi, uint32_t bitVal)
{
    /* if bitVal is 1, the bitPosi bit is set in the GPIOShadowPortx. Then
    * GPIOShadowPortx is written to the I/O port register. */
    switch (portNum)
    {
        case PORT0:
            if (bitVal)
            	LPC_GPIO0->SET = (1 << bitPosi);
            else
            	LPC_GPIO0->CLR = (1 << bitPosi);
            
            /* Use of shadow prevents bit operation error if the read value
            * (external hardware state) of a pin differs from the I/O latch
            * value. A potential side effect is that other GPIO code in this
            * project that is not aware of the shadow will have its GPIO
            * state overwritten.
            */
            //LPC_GPIO0->OUT = _GPIOShadowPort0;
            break;
        case PORT1:
            if (bitVal)
            	LPC_GPIO1->SET = (1 << bitPosi);
            else
            	LPC_GPIO1->CLR = (1 << bitPosi);
            
            //LPC_GPIO1->OUT = _GPIOShadowPort1;
            break;
/*
        case PORT2:
            if (bitVal)
                _GPIOShadowPort2 |= (1 << bitPosi);
            else
                _GPIOShadowPort2 &= ~(1 << bitPosi);
            
            LPC_GPIO2->OUT = _GPIOShadowPort2;
            break;
*/
        //default:
        //    break;
    }
    return;
}

int32 GPIO_GetValue(uint32 portNum, uint32 bitPosi)
{
    int32 val = -1;
    switch (portNum) 
    {
        case PORT0:
            if (LPC_GPIO0->PIN & (1 << bitPosi)) 
                val = 1;
            else
                val = 0;
            break;
        case PORT1:
            if (LPC_GPIO1->PIN & (1 << bitPosi)) 
                val = 1;
            else
                val = 0;
            break;
/*
        case PORT2:
            if (LPC_GPIO2->PIN & (1 << bitPosi)) return 1;
            else return 0;
*/
    }
    
    return val;
    
   /* switch (portNum) 
    {
        case PORT0:
            if (LPC_GPIO0->PIN & (1 << bitPosi)) return 1;
            else return 0;
        case PORT1:
            if (LPC_GPIO1->PIN & (1 << bitPosi)) return 1;
            else return 0;

        case PORT2:
            if (LPC_GPIO2->PIN & (1 << bitPosi)) return 1;
            else return 0;

        default:
            return -1;       
    }*/
}

//#ifndef BOOTLOADER
//#if !BOOTLOADER && !LON
/*****************************************************************************
** Function name:		GPIOSetInterrupt
**
** Descriptions:		Set interrupt sense, event, etc.
**						edge or level, 0 is edge, 1 is level
**						single or double edge, 0 is single, 1 is double 
**						active high or low, etc.
**
** parameters:			port num, bit position, sense, single/doube, polarity
** Returned value:		None
** 
*****************************************************************************/
void GPIO_SetInterrupt(uint32_t portNum, uint32_t bitPosi, uint32_t sense,
			uint32_t single, uint32_t event)
{
    switch (portNum)
    {
        case PORT0:
            if (sense == 0)
            {
                LPC_GPIO0->IS &= ~(0x1 << bitPosi);
                /* single or double only applies when sense is 0(edge trigger). */
                if (single == 0)
                    LPC_GPIO0->IBE &= ~(0x1 << bitPosi);
                else
                    LPC_GPIO0->IBE |= (0x1 << bitPosi);
            }
            else
            {
                LPC_GPIO0->IS |= (0x1 << bitPosi);
            }
            
            if (event == 0)
                LPC_GPIO0->IEV &= ~(0x1 << bitPosi);
            else
                LPC_GPIO0->IEV |= (0x1 << bitPosi);
            
            break;
        case PORT1:
            if (sense == 0)
            {
                LPC_GPIO1->IS &= ~(0x1 << bitPosi);
                /* single or double only applies when sense is 0(edge trigger). */
                if (single == 0)
                  LPC_GPIO1->IBE &= ~(0x1 << bitPosi);
                else
                    LPC_GPIO1->IBE |= (0x1 << bitPosi);
            }
            else 
            {
                LPC_GPIO1->IS |= (0x1 << bitPosi);
            }
            
            if (event == 0)
                LPC_GPIO1->IEV &= ~(0x1 << bitPosi);
            else
                LPC_GPIO1->IEV |= (0x1 << bitPosi);  

            break;
/*
        case PORT2:
            if (sense == 0)
            {
                LPC_GPIO2->IS &= ~(0x1 << bitPosi);
                // single or double only applies when sense is 0(edge trigger). 
                if (single == 0)
                  LPC_GPIO2->IBE &= ~(0x1 << bitPosi);
                else
                  LPC_GPIO2->IBE |= (0x1 << bitPosi);
            }
            else 
            {
                LPC_GPIO2->IS |= (0x1 << bitPosi);
            }
            
            if (event == 0)
                LPC_GPIO2->IEV &= ~(0x1 << bitPosi);
            else
                LPC_GPIO2->IEV |= (0x1 << bitPosi);  
            
            break;
*/
        default:
           break;
    }
    return;
}

/*****************************************************************************
** Function name:		GPIOIntEnable
**
** Descriptions:		Enable Interrupt Mask for a port pin.
**
** parameters:			port num, bit position
** Returned value:		None
** 
*****************************************************************************/
void GPIO_IntEnable(uint32_t portNum, uint32_t bitPosi)
{
    switch (portNum)
    {
        case PORT0:
            LPC_GPIO0->IE |= (0x1 << bitPosi); 
            break;
        case PORT1:
            LPC_GPIO1->IE |= (0x1 << bitPosi);	
            break;
/*
        case PORT2:
            LPC_GPIO2->IE |= (0x1 << bitPosi);	    
            break;
*/
        default:
            break;
    }
    return;
}

/*****************************************************************************
** Function name:		GPIOIntDisable
**
** Descriptions:		Disable Interrupt Mask for a port pin.
**
** parameters:			port num, bit position
** Returned value:		None
** 
*****************************************************************************/
void GPIO_IntDisable(uint32_t portNum, uint32_t bitPosi)
{
    switch (portNum)
    {
        case PORT0:
            LPC_GPIO0->IE &= ~(0x1 << bitPosi); 
            break;
        case PORT1:
            LPC_GPIO1->IE &= ~(0x1 << bitPosi);	
            break;
/*
        case PORT2:
            LPC_GPIO2->IE &= ~(0x1 << bitPosi);	    
            break;
*/
        default:
            break;
    }
    return;
}

/*****************************************************************************
** Function name:		GPIOIntStatus
**
** Descriptions:		Get Interrupt status for a port pin.
**
** parameters:			port num, bit position
** Returned value:		None
** 
*****************************************************************************/
uint32_t GPIO_IntStatus(uint32_t portNum, uint32_t bitPosi)
{
    uint32_t regVal = 0;
    
    switch (portNum)
    {
        case PORT0:
            if (LPC_GPIO0->MIS & (0x1 << bitPosi))
                regVal = 1;
            break;
        case PORT1:
            if (LPC_GPIO1->MIS & (0x1 << bitPosi))
                regVal = 1;	
            break;
/*
        case PORT2:
            if (LPC_GPIO2->MIS & (0x1 << bitPosi))
                regVal = 1;		    
            break;
*/
        default:
            break;
    }
    return (regVal);
}

/*****************************************************************************
** Function name:		GPIOIntClear
**
** Descriptions:		Clear Interrupt for a port pin.
**
** parameters:			port num, bit position
** Returned value:		None
** 
*****************************************************************************/
void GPIO_IntClear(uint32_t portNum, uint32_t bitPosi)
{
    switch (portNum)
    {
        case PORT0:
            LPC_GPIO0->IC |= (0x1 << bitPosi); 
            break;
        case PORT1:
            LPC_GPIO1->IC |= (0x1 << bitPosi);	
            break;
/*
        case PORT2:
            LPC_GPIO2->IC |= (0x1 << bitPosi);	    
            break;
*/
        default:
            break;
    }
    /* Recommended by datasheet to add two nops in case of edge detection (LON in DCU) */
    __ASM("nop");
    __ASM("nop");
    
    return;
}

#ifndef BOOTLOADER
void PIO0_IRQHandler(void)
#else
void PIO0_IRQHandlerBoot(void)
#endif
{
    uint32_t regVal;
    
    regVal = GPIO_IntStatus(PORT0, CTS);
    if (regVal)
    {
        GPIO_IntClear(PORT0, CTS);
        
#ifdef LON
        CtsInterruptHandler();
#endif
    }
    
    regVal = GPIO_IntStatus(PORT0, NEURON_RESET);
    if (regVal)
    {
        GPIO_IntClear(PORT0, NEURON_RESET);
#ifdef LON
        ResetInterruptHandler();
#endif
    }
    return;
}
//#endif
/******************************************************************************
**                            End Of File
******************************************************************************/
