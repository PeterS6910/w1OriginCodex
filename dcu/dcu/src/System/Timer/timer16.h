/*****************************************************************************
 *   timer16.h:  Header file for NXP LPC1xxx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.08.20  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#ifndef __TIMER16_H 
#define __TIMER16_H

/* The test is either MAT_OUT or CAP_IN. Default is MAT_OUT. */
#define TIMER_MATCH		0


#define EMC0	4
#define EMC1	6
#define EMC2	8
#define EMC3	10

///* For 16-bit timer, make sure that TIME_INTERVAL should be no
//greater than 0xFFFF. */
//#define TIMER_INTERVAL
///* depending on the SystemFrequency and SystemAHBFrequency setting, 
//if SystemFrequency = 60Mhz, SystemAHBFrequency = 1/4 SystemAHBFrequency, 
//10mSec = 150.000-1 counts */

void Timer16_DelayMs(uint32_t delayInMs);
void CT16B0_IRQHandler(void);
void CT16B1_IRQHandler(void);
void Timer16_Enable(uint8_t timer_num);
void Timer16_Disable(uint8_t timer_num);
void Timer16_Reset(uint8_t timer_num);
void Timer16_Init(uint8_t timer_num, uint16_t timerInterval);
void Timer16_InitPWM(uint8_t timer_num, uint32_t period, uint8_t match_enable, uint8_t cap_enabled);
void Timer16_SetMatchPWM(uint8_t timer_num, uint8_t match_nr, uint32_t value);

#endif /* end __TIMER16_H */
/*****************************************************************************
**                            End Of File
******************************************************************************/
