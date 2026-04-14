/*****************************************************************************
 *   timer16.c:  Timer C file for NXP LPC11xx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.08.20  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#include "System\LPC122x.h"
#include "timer16.h"
#include "System\Core\system_LPC122x.h"
#include "System\Core\ipr.h"
#include "timer.h"

volatile uint32_t _timer16_0_counter = 0;
volatile uint32_t _timer16_1_counter = 0;
volatile uint32_t _timer16_0_capture = 0;
volatile uint32_t _timer16_1_capture = 0;
volatile uint32_t _timer16_0_period = 0;
volatile uint32_t _timer16_1_period = 0;

/*****************************************************************************
** Function name:		delayMs
**
** Descriptions:		Start the timer delay in milo seconds
**						until elapsed
**
** parameters:			timer number, Delay value in milo second			 
** 						
** Returned value:		None
** 
*****************************************************************************/
void Timer16_DelayMs(uint32_t delayInMs)
{
    for (int i = 0; i < delayInMs; i++) 
    {
        /*
        * setup timer #0 for delay
        */
        LPC_TMR16B0->TCR = 0x02;		/* reset timer */
        LPC_TMR16B0->PR  = 0x00;		/* set prescaler to zero */
        LPC_TMR16B0->MR0 = ONE_MS_TIME_INTERVAL; // is 39 000 which fits in 16 timer
        LPC_TMR16B0->IR  = 0xff;		/* reset all interrrupts */
        LPC_TMR16B0->MCR = 0x04;		/* stop timer on match */
        LPC_TMR16B0->TCR = 0x01;		/* start timer */
        /* wait until delay time has elapsed */
        while (LPC_TMR16B0->TCR & 0x01);
    }
    
    return;
}

/******************************************************************************
** Function name:		CT16B0_IRQHandler
**
** Descriptions:		Timer/Counter 0 interrupt handler
**						executes each 10ms @ 60 MHz CPU Clock
**
** parameters:			None
** Returned value:		None
** 
******************************************************************************/
#ifndef BOOTLOADER
void CT16B0_IRQHandler(void)
{  
    if ( LPC_TMR16B0->IR & 0x1 )
    {
        LPC_TMR16B0->IR = 1;			/* clear interrupt flag */
        _timer16_0_counter++;
    }
    if ( LPC_TMR16B0->IR & (0x1 << 4) )
    {
        LPC_TMR16B0->IR = 0x1 << 4;		/* clear interrupt flag */
        _timer16_0_capture++;
    }
  return;
}

/******************************************************************************
** Function name:		CT16B1_IRQHandler
**
** Descriptions:		Timer/Counter 1 interrupt handler
**						executes each 10ms @ 60 MHz CPU Clock
**
** parameters:			None
** Returned value:		None
** 
******************************************************************************/
void CT16B1_IRQHandler(void)
{
    if (LPC_TMR16B1->IR & 0x1)
    {  
        LPC_TMR16B1->IR = 1;			/* clear interrupt flag */
        _timer16_1_counter++;
    }
    if (LPC_TMR16B1->IR & (0x1 << 4) )
    {
        LPC_TMR16B1->IR = 0x1 << 4;		/* clear interrupt flag */
        _timer16_1_capture++;
    }
    return;
}
#endif

/******************************************************************************
** Function name:		enable_timer
**
** Descriptions:		Enable timer
**
** parameters:			timer number: 0 or 1
** Returned value:		None
** 
******************************************************************************/
void Timer16_Enable(uint8_t timerNum)
{
    if (timerNum == 0)
    {
        LPC_TMR16B0->TCR = 1;
    }
    else
    {
        LPC_TMR16B1->TCR = 1;
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
void Timer16_Disable(uint8_t timerNum)
{
    if (timerNum == 0)
    {
        LPC_TMR16B0->TCR = 0;
    }
    else
    {
        LPC_TMR16B1->TCR = 0;
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
void Timer16_Reset(uint8_t timerNum)
{
    uint32_t regVal;
    
    if ( timerNum == 0 )
    {
        regVal = LPC_TMR16B0->TCR;
        regVal |= 0x02;
        LPC_TMR16B0->TCR = regVal;
    }
    else
    {
        regVal = LPC_TMR16B1->TCR;
        regVal |= 0x02;
        LPC_TMR16B1->TCR = regVal;
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
void Timer16_Init(uint8_t timerNum, uint16_t timerInterval) 
{
	if (timerNum == 0)
	{
		/* Some of the I/O pins need to be clearfully planned if
		you use below module because JTAG and TIMER CAP/MAT pins are muxed. */
		LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 7);
		
		_timer16_0_counter = 0;
		_timer16_0_capture = 0;
		LPC_TMR16B0->MR0 = timerInterval;

		/* Capture 0 on rising edge, interrupt enable. */
		LPC_TMR16B0->CCR = (0x1 << 0) | (0x1 << 2);

		LPC_TMR16B0->MCR = 3;				/* Interrupt and Reset on MR0 and MR1 */
		
		/* Enable the TIMER0 Interrupt */
		NVIC_EnableIRQ(TIMER_16_0_IRQn);
                NVIC_SetPriority(TIMER_16_0_IRQn,TIMER16_PRIO);
	}
	else if (timerNum == 1)
	{
		/* Some of the I/O pins need to be clearfully planned if
		you use below module because JTAG and TIMER CAP/MAT pins are muxed. */
		LPC_SYSCON->SYSAHBCLKCTRL |= (1<<8);
		
		_timer16_1_counter = 0;
		_timer16_1_capture = 0;
		LPC_TMR16B1->MR0 = timerInterval;
#if TIMER_MATCH
		LPC_TMR16B1->EMR &= ~(0xFF << 4);
		LPC_TMR16B1->EMR |= ((0x3 << 4) | (0x3 << 6) | (0x3 << 8));
#else
		/* Capture 0 on rising edge, interrupt enable. */
		LPC_TMR16B1->CCR = (0x1 << 0) | (0x1 << 2);
#endif
		LPC_TMR16B1->MCR = 3;				/* Interrupt and Reset on MR0 and MR1 */
		
		/* Enable the TIMER1 Interrupt */
		NVIC_EnableIRQ(TIMER_16_1_IRQn);
                NVIC_SetPriority(TIMER_16_1_IRQn,TIMER16_PRIO);
	}
	return;
}

/******************************************************************************
**                            End Of File
******************************************************************************/
