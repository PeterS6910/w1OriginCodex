/*****************************************************************************
 *   timer32.c:  32-bit Timer C file for NXP LPC1xxx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.08.20  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#include "System\LPC122x.h"
#include "System\Core\system_LPC122x.h"
#include "System\Core\ipr.h"
#include "timer32.h"
#include "timer.h"

#include "System\watchdog.h"
#include "System\ADC\adc.h"
#include "IO\Outputs.h"
#include "Communication\ClspLon\Driver\LdvSci.h"


//volatile uint32_t _timer32_0_counter = 0;
//volatile uint32_t _timer32_1_counter = 0;
////volatile uint32_t _timer32_0_capture = 0;
//volatile uint32_t _timer32_1_capture = 0;
//volatile uint32_t _timer32_0_period = 0;
//volatile uint32_t _timer32_1_period = 0;

//#define MAX_BASIC_TIMER_TASKS 4
//
//typedef void(*PFunction)(void);
//
//PFunction _timer32_0_Tasks[MAX_BASIC_TIMER_TASKS];
//PFunction _timer32_1_Tasks[MAX_BASIC_TIMER_TASKS];
//
//void Timer32_RegisterTimerTask(uint8_t timerNum,void (*f)(void)) {
//    if (f == 0)
//        return;
//    
//    if (timerNum == 0) {
//    
//        for(int i=0;i<MAX_BASIC_TIMER_TASKS; i++)
//            if (_timer32_0_Tasks[i] == 0)
//            {
//                _timer32_0_Tasks[i] = f;
//                break;
//            }
//    
//    }
//    else if (timerNum == 1) {
//         for(int i=0;i<MAX_BASIC_TIMER_TASKS; i++)
//            if (_timer32_1_Tasks[i] == 0)
//            {
//                _timer32_1_Tasks[i] = f;
//                break;
//            }
//    }
//}

/*****************************************************************************
** Function name:		Timer32_DelayMs
**
** Descriptions:		Start the timer delay in milo seconds
**						until elapsed
**
** parameters:			timer number, Delay value in milo second			 
** 						
** Returned value:		None
** 
*****************************************************************************/
void Timer32_DelayMs(uint8_t timer_num, uint32_t delayInMs)
{
    if (timer_num == 0)
    {
        /* setup timer #0 for delay */
        LPC_TMR32B0->TCR = 0x02;		/* reset timer */
        LPC_TMR32B0->PR  = 0x00;		/* set prescaler to zero */
        LPC_TMR32B0->MR0 = delayInMs * ONE_MS_TIME_INTERVAL;
        LPC_TMR32B0->IR  = 0xff;		/* reset all interrrupts */
        LPC_TMR32B0->MCR = 0x04;		/* stop timer on match */
        LPC_TMR32B0->TCR = 0x01;		/* start timer */
        
        /* wait until delay time has elapsed */
        while (LPC_TMR32B0->TCR & 0x01);
    }
    else if (timer_num == 1)
    {
        /* setup timer #1 for delay */
        LPC_TMR32B1->TCR = 0x02;		/* reset timer */
        LPC_TMR32B1->PR  = 0x00;		/* set prescaler to zero */
        LPC_TMR32B1->MR0 = delayInMs * ONE_MS_TIME_INTERVAL;
        LPC_TMR32B1->IR  = 0xff;		/* reset all interrrupts */
        LPC_TMR32B1->MCR = 0x04;		/* stop timer on match */
        LPC_TMR32B1->TCR = 0x01;		/* start timer */
        
        /* wait until delay time has elapsed */
        while (LPC_TMR32B1->TCR & 0x01);
    }
    return;
}

/******************************************************************************
** Function name:		CT32B0_IRQHandler
**
** Descriptions:		Timer/Counter 0 interrupt handler
**						executes each 10ms @ 60 MHz CPU Clock
**
** parameters:			None
** Returned value:		None
** 
******************************************************************************/
//#ifndef BOOTLOADER
//void CT32B0_IRQHandler(void)
//{
//    if (LPC_TMR32B0->IR & 0x01)
//    {  
//        LPC_TMR32B0->IR = 1;				/* clear interrupt flag */
//        
//        for(int i=0;i<MAX_BASIC_TIMER_TASKS;i++)
//            if (0 != _timer32_0_Tasks[i])
//                _timer32_0_Tasks[i]();
//        
//    }
//    if (LPC_TMR32B0->IR & (0x1 << 4))
//    {  
//        LPC_TMR32B0->IR = 0x1 << 4;			/* clear interrupt flag */
//    }
//    return;
//}
//#endif

volatile uint32 _outputsDelay;
volatile uint32 _wdDelay;

#ifndef BOOTLOADER
void CT32B1_IRQHandler(void)
#else
void CT32B1_IRQHandlerBoot(void)
#endif
{
    /* Clear the interrupt */
    if (LPC_TMR32B1->IR & 0x01)
        LPC_TMR32B1->IR = 1;			// clear interrupt flag 
    if (LPC_TMR32B1->IR & (0x1 << 4))
        LPC_TMR32B1->IR = 0x1 << 4;			// clear interrupt flag 
    
    // handle ADC timming
    // TODO beware that this can be delayed due to LON driver -> ADC can be delayed
    if (_adcContext.timer != 0)
    {
        if (--_adcContext.timer == 0)
        {
            ADC_StartNewConversion();
        }
    }
    
#ifdef LON
    OneMsTimerHandler();
#endif
    
#ifndef BOOTLOADER
    if (++_outputsDelay > 5)
    {
        _outputsDelay = 0;
        Outputs_Task();
    }
    
    if (++_wdDelay > 20)
    {
        _wdDelay = 0;
        WD_MainThread();
    }
#endif
}


/******************************************************************************
** Function name:		enable_timer
**
** Descriptions:		Enable timer
**
** parameters:			timer number: 0 or 1
** Returned value:		None
** 
******************************************************************************/
void Timer32_Enable(uint8_t timer_num)
{
    if (timer_num == 0)
    {
        LPC_TMR32B0->TCR = 1;
    }
    else
    {
        LPC_TMR32B1->TCR = 1;
    }
    return;
}

/******************************************************************************
** Function name:		disable_timer
**
** Descriptions:		Disable timer
**
** parameters:			timer number: 0 or 1
** Returned value:		None
** 
******************************************************************************/
void Timer32_Disable(uint8_t timer_num)
{
    if (timer_num == 0)
    {
        LPC_TMR32B0->TCR = 0;
    }
    else
    {
        LPC_TMR32B1->TCR = 0;
    }
    return;
}

/******************************************************************************
** Function name:		reset_timer
**
** Descriptions:		Reset timer
**
** parameters:			timer number: 0 or 1
** Returned value:		None
** 
******************************************************************************/
void Timer32_Reset(uint8_t timer_num)
{
    uint32_t regVal;
    
    if (timer_num == 0)
    {
        regVal = LPC_TMR32B0->TCR;
        regVal |= 0x02;
        LPC_TMR32B0->TCR = regVal;
    }
    else
    {
        regVal = LPC_TMR32B1->TCR;
        regVal |= 0x02;
        LPC_TMR32B1->TCR = regVal;
    }
    return;
}

/******************************************************************************
** Function name:		init_timer
**
** Descriptions:		Initialize timer, set timer interval, reset timer,
**						install timer interrupt handler
**
** parameters:			timer number and timer interval
** Returned value:		None
** 
******************************************************************************/
void Timer32_Init(uint8_t timer_num, uint32_t timerInterval) 
{
    if (timer_num == 0)
    {
//        for(int i=0;i<MAX_BASIC_TIMER_TASKS;i++)
//            _timer32_0_Tasks[i] = 0;
        
        /* Some of the I/O pins need to be clearfully planned if
        you use below module because JTAG and TIMER CAP/MAT pins are muxed. */
        LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 9);
       
        LPC_TMR32B0->MR0 = timerInterval;
        
        /* Capture 0 on rising edge, interrupt enable. */
        LPC_TMR32B0->CCR = (0x1 << 0) | (0x1 << 2);
        LPC_TMR32B0->MCR = 3;			/* Interrupt and Reset on MR0 */
        
        /* Enable the TIMER0 Interrupt */
        NVIC_EnableIRQ(TIMER_32_0_IRQn);
        // lower priority than SysTick
        NVIC_SetPriority(TIMER_32_0_IRQn, TIMER32_PRIO);
        
    }
    /* timer 1 reserved for LON */
    else if (timer_num == 1)
    {
        LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 10);         // enable clock for 32 bit timer 1
        LPC_TMR32B1->MR0 = timerInterval;               // 1 ms
        LPC_TMR32B1->CCR = (0x1 << 0) | (0x1 << 2);     // capture on rising edge & enable interrupt
        LPC_TMR32B1->MCR = 3;			                // Interrupt and Reset on MR0 
        NVIC_EnableIRQ(TIMER_32_1_IRQn);
        NVIC_SetPriority(TIMER_32_1_IRQn, 1);
    }
    return;
}

/******************************************************************************
**                            End Of File
******************************************************************************/
