/******************************************************************************
 * @file:    system_LPC122x.c
 * @purpose: CMSIS Cortex-M0 Device Peripheral Access Layer Source File
 *           for the NXP LPC122x Device Series 
 * @version: V1.0
 * @date:    26. Nov. 2008
 *----------------------------------------------------------------------------
 *
 * Copyright (C) 2008 ARM Limited. All rights reserved.
 *
 * ARM Limited (ARM) is supplying this software for use with Cortex-M3 
 * processor based microcontrollers.  This file can be freely distributed 
 * within development tools that are supporting such ARM based processors. 
 *
 * THIS SOFTWARE IS PROVIDED "AS IS".  NO WARRANTIES, WHETHER EXPRESS, IMPLIED
 * OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE APPLY TO THIS SOFTWARE.
 * ARM SHALL NOT, IN ANY CIRCUMSTANCES, BE LIABLE FOR SPECIAL, INCIDENTAL, OR
 * CONSEQUENTIAL DAMAGES, FOR ANY REASON WHATSOEVER.
 *
 ******************************************************************************/
#include <stdint.h>
#include "System\LPC122x.h"
#include "System\GPIO\gpio.h"
#include "System\constants.h"
#include "System\Timer\sysTick.h"
#include "System\Timer\timer32.h"
#include "System\UART\uart.h"
#include "System\Core\ipr.h"
#include "System\Core\exceptionHandling.h"
#include "System\watchdog.h"
#include "IO\Inputs.h"
#include "Communication\Clsp485\clsp485.h"
#include "Communication\ClspLon\Driver\LdvSci.h"

#define CLOCK_SETUP           1

#define SYS_PLL_SETUP         1
#define SYS_PLLSRCSEL_Val     0x00000001
#define SYS_PLL_MSEL_Val      0x00000002
#define SYS_PLL_PSEL_Val      0x00000001
#define SYS_AHB_DIV_Val       0x01                      /* 1 through 255, 0 will disable the output. */

/*----------------------------------------------------------------------------
  Define clocks
 *----------------------------------------------------------------------------*/
#define XTAL        (13000000UL)        /* Oscillator frequency               */
#define OSC_CLK     (      XTAL)        /* Main oscillator frequency          */
#define IRC_OSC     ( 4000000UL)        /* Internal RC oscillator frequency   */
#define WDT_OSC     (  250000UL)        /* WDT oscillator frequency           */

/*----------------------------------------------------------------------------
  Clock Variable definitions
 *----------------------------------------------------------------------------*/
uint32_t _clockSource = IRC_OSC;
uint32_t _systemFrequency = IRC_OSC; /*!< System Clock Frequency (Core Clock)  */
uint32_t _systemAHBFrequency = IRC_OSC;

/**
 * Misc. clock generation modules
 *
 * @param  none
 * @return none
 *
 * @brief  Setup the microcontroller system.
 *         Initialize the System and update the SystemFrequency variable.
 */
void SystemPLL_Setup(void)
{
    uint32_t regVal;
    
    LPC_SYSCON->PRESETCTRL  &= ~0x00008000;                 /* Disable 1-Cycle Read Mode */
    
    _clockSource = OSC_CLK;
    LPC_SYSCON->SYSPLLCLKSEL = 0x01;    /* Select system OSC */
//  LPC_SYSCON->SYSPLLCLKUEN = 0x01;                 /* Update clock source */
    LPC_SYSCON->SYSPLLCLKUEN = 0x00;                 /* toggle Update register once */
    LPC_SYSCON->SYSPLLCLKUEN = 0x01;
    while (!(LPC_SYSCON->SYSPLLCLKUEN & 0x01)); /* Wait until updated */

    regVal = LPC_SYSCON->SYSPLLCTRL;
    regVal &= ~0x1FF;
    LPC_SYSCON->SYSPLLCTRL = (regVal | (SYS_PLL_PSEL_Val << 5) | SYS_PLL_MSEL_Val);
  
    /* Enable main system clock, main system clock bit 7 in PDRUNCFG. */
    LPC_SYSCON->PDRUNCFG &= ~(0x1 << 7);
    while (!(LPC_SYSCON->SYSPLLSTAT & 0x01));   /* Wait until it's locked */

    LPC_SYSCON->MAINCLKSEL = 0x03;  /* Select PLL clock output */
//  LPC_SYSCON->MAINCLKUEN = 0x01;                /* Update MCLK clock source */
    LPC_SYSCON->MAINCLKUEN = 0x00;                /* Toggle update register once */
    LPC_SYSCON->MAINCLKUEN = 0x01;
    while (!(LPC_SYSCON->MAINCLKUEN & 0x01));   /* Wait until updated */

    LPC_SYSCON->SYSAHBCLKDIV = SYS_AHB_DIV_Val;   /* SYS AHB clock, 0 will disable output */

    /* If the SYS PLL output is selected as the main clock. Even if SYS PLL is 
    configured and enabled, it doesn't mean it will be selected as the MAIN clock 
    source. Be careful with MAINCLKSEL value. If SYS PLL is not selected, System
    Frequence should be the same as either IRC, external OSC(SYS), or WDT OSC clock. */
    _systemFrequency = _clockSource * (SYS_PLL_MSEL_Val + 1);

    _systemAHBFrequency = (uint32_t)(_systemFrequency / SYS_AHB_DIV_Val);
    return;
}

/**
 * Initialize the system
 *
 * @param  none
 * @return none
 *
 * @brief  Setup the microcontroller system.
 *         Initialize the System and update the SystemFrequency variable.
 */
void SystemInit(void)
{
    uint32_t i;

#if 1
    /* First, below lines are for debugging only. For future release, WDT is 
    enabled by bootrom, thus, unless a feed to WDT continuously, or WDT timeout 
    will occur. If it's happen, WDT interrupt will be pending until a INT_CLEAR
    is applied. Below logic is to prevent system from going to the WDT interrupt
    during debugging. 
    Second, all the peripheral clocks seem to be enabled by bootrom, it's
    not consistent with the UM. In below lines, only SYS, ROM, RAM, FLASHREG,
    FLASHARRAY, and I2C are enabled per UM dated July 14th. */
    
    /* watchdog configuration */
    LPC_WDT->MOD = 0x00;  
    LPC_WDT->FEED = 0xAA;         /* Feeding sequence */
    LPC_WDT->FEED = 0x55;
        
    /* Select IRC as the clock source for the watchdog */
    LPC_WDT->CLKSEL &= ~0x03;
    /* Enable only interrupt after watchdog counter reaches zero */
    
#if (NO_WATCHDOG || DEBUG)
    LPC_WDT->MOD &= ~(1 << 1);
#else
    LPC_WDT->MOD |= (1 << 1);
#endif
    
    /* Set the counter value */    
    //LPC_WDT->TC = 0x2DC6C0; // 12 000 000 / 4 (hopefully 1 sec)
    LPC_WDT->TC = 0x5B8D80; // 2 sec
    //LPC_WDT->TC = 0x00ffff;
    LPC_WDT->WARNINT = 0;// 512;
    
    /* Enable watchdog */
    LPC_WDT->MOD |= 0x01;
    LPC_WDT->FEED = 0xAA;
    LPC_WDT->FEED = 0x55;
    
//#ifndef BOOTLOADER
//    NVIC_EnableIRQ(WDT_IRQn);
//    NVIC_SetPriority(WDT_IRQn, 0);
//#else
//    NVIC_DisableIRQ(WDT_IRQn);
//#endif
    
    
    LPC_SYSCON->SYSMEMREMAP = 0x2;                /* remap to internal flash */
    
    NVIC->ICPR[0] |= 0xFFFFFFFF; 
    LPC_SYSCON->SYSAHBCLKCTRL = 0x00000001F | (1 << 15);
#endif   

#if (CLOCK_SETUP)                       /* Clock Setup */
    /* bit 0 default is crystal bypass, 
    bit1 0=0~20Mhz crystal input, 1=15~50Mhz crystal input. */
    LPC_SYSCON->SYSOSCCTRL = 0x00;
    
    /* main system OSC run is cleared, bit 5 in PDRUNCFG register */
    LPC_SYSCON->PDRUNCFG &= ~(0x1 << 5);
    /* Wait 200us for OSC to be stablized, no status 
    indication, dummy wait. */
    for ( i = 0; i < 0x100; i++ );

    SystemPLL_Setup();  

#endif  /* endif CLOCK_SETUP */

    /* System clock to the IOCON needs to be enabled or
    most of the I/O related peripherals won't work. */
    LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 16);
    
    GPIO_Init();
    
    //LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 15);
    
    return;
}

void System_RequestUpgrade()
{
    uint32_t* requestAddress = (uint32_t*)APP_SWITCH_ADDR;
    *requestAddress = RUN_UPGRADE;
}

/**
 * Setup systick and UART0 addresses so bootloader handlers can be used
 */
void System_PrepareForBootloader()
{
#ifdef BOOTLOADER
	#ifndef XPRESSO
    	__disable_interrupt();
	#else
    	__disable_irq();
	#endif
    
    // systick
    uint32_t* systickHandler = (uint32_t*)BOOT_SYSTICK_HANDLER;
    *systickHandler = (uint32_t)&SysTick_HandlerBoot;    
    
    uint32_t* systickSwitch = (uint32_t*)SYSTICK_SWITCH_ADDR;
    *systickSwitch = (uint32_t)BOOT_SYSTICK_HANDLER;
       
    // uart
    uint32_t* uartHandler = (uint32_t*)BOOT_UART_HANDLER;
    *uartHandler = (uint32_t)&UART0_IRQHandlerBoot;   
    
    uint32_t* uartSwitch = (uint32_t*)UART_SWITCH_ADDR;
    *uartSwitch = (uint32_t)BOOT_UART_HANDLER;
    
    // timer32
    uint32* timer32Handler = (uint32*)BOOT_TIMER_HANDLER;
    *timer32Handler = (uint32)&CT32B1_IRQHandlerBoot;
    
    uint32* timer32Switch = (uint32*)TIMER_SWITCH_ADDR;
    *timer32Switch= (uint32)BOOT_TIMER_HANDLER;
    
    // pio0 (cts)
    uint32* ctsHandler = (uint32*)BOOT_CTS_HANDLER;
    *ctsHandler = (uint32)&PIO0_IRQHandlerBoot;
    
    uint32* ctsSwitch = (uint32*)CTS_SWITCH_ADDR;
    *ctsSwitch = (uint32)BOOT_CTS_HANDLER;
    
    // hardfault
    uint32* hardfaultHandler = (uint32*)BOOT_HARDFAULT_HANDLER;
    *hardfaultHandler = (uint32)&HardFault_HandlerBoot;
    
    uint32* hardfaultSwitch = (uint32*)HARDFAULT_SWITCH_ADDR;
    *hardfaultSwitch = (uint32)BOOT_HARDFAULT_HANDLER;
    
    // wdt 
//    uint32* wdtHandler = (uint32*)BOOT_WDT_HANDLER;
//    *wdtHandler = (uint32)&WDT_IRQHandlerBoot;
//    
//    uint32* wdtSwitch = (uint32*)WATCHDOG_SWITCH_ADDR;
//    *wdtSwitch = (uint32)BOOT_WDT_HANDLER;
    
//    if (System_CheckForceSetAddress())
//    {
//        Clsp_SetForceLogicalAddress();
//        System_ClearForceSetAddress();
//    }
#endif
}

/** 
 * Setup systick and UART0 addresses so DCU main app handlers can be used 
 */
void System_PrepareForApplication()
{
	#ifndef XPRESSO
    	__disable_interrupt();
	#else
    	__disable_irq();
	#endif
    
    uint32_t* systickSwitch;
    systickSwitch = (uint32_t*)SYSTICK_SWITCH_ADDR;
    *systickSwitch = (uint32_t)DCU_SYSTICK_HANDLER;
    
    uint32_t* uartSwitch;
    uartSwitch = (uint32_t*)UART_SWITCH_ADDR;
    *uartSwitch = (uint32_t)DCU_UART_HANDLER; 
    
    uint32* timer32Switch = (uint32*)TIMER_SWITCH_ADDR;
    *timer32Switch= (uint32)DCU_TIMER_HANDLER;
    
    uint32* ctsSwitch = (uint32*)CTS_SWITCH_ADDR;
    *ctsSwitch = (uint32)DCU_CTS_HANDLER;
}



#ifndef LON
/**
 * Request the logical address to be force-set to suggested address. 
 */
void System_RequestForceAddressSet()
{
    uint32_t* forceSetAddr = (uint32_t*)FORCE_SET_ADDR;
    *forceSetAddr = FORCE_SET_ADDRESS;
    uint32_t* forceAddr = (uint32_t*)FORCE_ADDRESS_ADDR;
    *forceAddr = _specialInputState.address;
}

/**
 * Cancel the force set address request (called from bootloader after address force set already)
 */
void System_ClearForceSetAddress()
{
    uint32_t* forceSetAddr = (uint32_t*)FORCE_SET_ADDR;
    *forceSetAddr = 0;
}

/** 
 * Check if force set address request issued 
 * 
 * @return 1 if request issued, 0 otherwise
 */
uint32_t System_CheckForceSetAddress()
{
    uint32_t* forceSetAddr = (uint32_t*)FORCE_SET_ADDR;
    if (*forceSetAddr == FORCE_SET_ADDRESS)
        return 1;
    else
        return 0;
}
#endif // LON
