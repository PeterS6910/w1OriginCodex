/*****************************************************************************
 *   timer32.h:  Header file for NXP LPC1xxx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.08.20  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#ifndef __TIMER32_H 
#define __TIMER32_H

#include "System\Core\system_LPC122x.h"

/* The test is either MAT_OUT or CAP_IN. Default is MAT_OUT. */
#define TIMER_MATCH		0

#define EMC0	4
#define EMC1	6
#define EMC2	8
#define EMC3	10

#define MATCH0	(1<<0)
#define MATCH1	(1<<1)
#define MATCH2	(1<<2)
#define MATCH3	(1<<3)

#define LON_TIME_INTERVAL (_systemAHBFrequency / 1000 - 1)  // every 1 ms
/* depending on the SystemFrequency and SystemAHBFrequency setting, 
if SystemFrequency = 60Mhz, SystemAHBFrequency = 1/4 SystemFrequency, 
10mSec = 150.000-1 counts */

void Timer32_RegisterTimerTask(uint8_t timerNum, void (*f)(void));

void Timer32_DelayMs(uint8_t timer_num, uint32_t delayInMs);
void CT32B0_IRQHandler(void);
void CT32B1_IRQHandlerBoot(void);
//void CT32B1_IRQHandler(void);
void Timer32_Enable(uint8_t timer_num);
void Timer32_Disable(uint8_t timer_num);
void Timer32_Reset(uint8_t timer_num);
void Timer32_Init(uint8_t timer_num, uint32_t timerInterval);
void Timer32_InitPWM(uint8_t timer_num, uint32_t period, uint8_t match_enable);
void Timer32_SetMatchPWM(uint8_t timer_num, uint8_t match_nr, uint32_t value);

#endif /* end __TIMER32_H */
/*****************************************************************************
**                            End Of File
******************************************************************************/
