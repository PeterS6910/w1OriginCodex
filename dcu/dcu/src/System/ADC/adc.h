/*****************************************************************************
 *   adc.h:  Header file for NXP LPC11xx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.07.19  ver 1.00    Prelimnary version, first Release
 *
******************************************************************************/
#ifndef __ADC_H 
#define __ADC_H

#include "System\constants.h"
#include "System\baseTypes.h"

#define ADC_DEBUG			1

#define ADC_OFFSET		0x10
#define ADC_INDEX		4

#define ADC_DONE		0x80000000
#define ADC_OVERRUN		0x40000000
#define ADC_ADINT		0x00010000

#define SEL0            0x04
#define SEL1            0x05
#define SEL2            0x06

#define ADC_MAX_VALUE   1023

#define ADC_NUM		    4           /* for DCU     */
#define ADC_CLK	        1000000		/* set to 1Mhz */

#define ADC_CHANNEL_CNT 4
#define ADC_START_CONVERSION    (1 << 24)

typedef enum 
{
    ADC_Reference = 0,
    ADC_Base14 = 1,
    ADC_Ext14 = 2,
    ADC_Ext58 = 3,
    ADC_Ext912 = 4
} ADC_ChannelSource_e;

typedef struct
{
    uint32 timer;
    uint32 currentChannel;
    ADC_ChannelSource_e channelSource;
} ADC_Context_t;

extern volatile uint32 _ADCValue[MAX_INPUT_COUNT];
extern volatile ADC_Context_t _adcContext;

uint32 ADC_IsValid();

void ADC_Init(uint32 ADC_Clk);
//uint32 ADC_Read(uint8_t channelNum);
//void ADC_ReadAll();
void ADC_StartNewConversion();

#ifndef RS485 // this conditioning marks old 1.7.approach
#ifndef LON
void ADC_Task();
#endif
#endif

#ifdef DEBUG_GENERATOR
void ADC_RandomTask();
void ADC_InitGenerator(uint32 minValue, uint32 maxValue);
#endif // DEBUG_GENERATOR

#endif /* end __ADC_H */
/*****************************************************************************
**                            End Of File
******************************************************************************/
