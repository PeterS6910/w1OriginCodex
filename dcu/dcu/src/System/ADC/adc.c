/*****************************************************************************
 *   adc.c:  ADC C file for NXP LPC11xx Family Microprocessors
 *
 *   Copyright(C) 2008, NXP Semiconductor
 *   All rights reserved.
 *
 *   History
 *   2008.07.20  ver 1.00    Prelimnary version, first Release
 *
*****************************************************************************/
#include "System\LPC122x.h"			/* LPC11xx Peripheral Registers */
#include "System\GPIO\gpio.h"
#include "System\Timer\sysTick.h"
#include "System\Core\system_LPC122x.h"
#include "adc.h"
#include "System\constants.h"
#ifndef BOOTLOADER
#include "Testing\testing.h"
#endif

volatile uint32 _ADCValue[MAX_INPUT_COUNT];

volatile uint32 _adcTimer = 0;
volatile uint32 _adcState = 0;
volatile uint32 _referenceValue = 0;
volatile bool _adcValid = false;

volatile ADC_Context_t _adcContext;

/*****************************************************************************
** Function name:		ADC_SelectInputs
**
** Descriptions:		Select either inputs or the reference as the source for ADC
**
** parameters:			bool whether inputs selected
** Returned value:		None
** 
*****************************************************************************/
void ADC_SelectInputs(ADC_ChannelSource_e isSelected)
{
    switch (isSelected)
    {
        case ADC_Reference: // reference
            GPIO_SetValue(PORT0, SEL0, 0);
            GPIO_SetValue(PORT0, SEL1, 0);
            GPIO_SetValue(PORT0, SEL2, 0);
            break;
            
        case ADC_Base14: // 1 - 4 base
            GPIO_SetValue(PORT0, SEL0, 1);
            GPIO_SetValue(PORT0, SEL1, 0);
            GPIO_SetValue(PORT0, SEL2, 0);
            break;
           
        case ADC_Ext14: // 1 - 4 ext
            GPIO_SetValue(PORT0, SEL0, 0);
            GPIO_SetValue(PORT0, SEL1, 1);
            GPIO_SetValue(PORT0, SEL2, 0);
            break;
        
        case ADC_Ext58: // 5 - 8 ext
            GPIO_SetValue(PORT0, SEL0, 0);
            GPIO_SetValue(PORT0, SEL1, 0);
            GPIO_SetValue(PORT0, SEL2, 1);
            break;
            
        case ADC_Ext912: // 9 - 12 ext
            GPIO_SetValue(PORT0, SEL0, 0);
            GPIO_SetValue(PORT0, SEL1, 1);
            GPIO_SetValue(PORT0, SEL2, 1);
            break;
       
    }
    
    _adcContext.timer = 1;  // this will be cleared in timer32, ADC next adc conversion will be started from there
}

#ifdef DEBUG_GENERATOR

typedef struct
{
    
    
    uint32 lastChange;
    uint32 nextChange;
    bool enabled;
} ADC_RandomContext_t;

extern volatile bool _testADCGenEnabled;

volatile ADC_RandomContext_t _adcRandomContext[4];
volatile bool _adcIsGeneratorInit = false;
volatile uint32 _adcGenMinInterval;
volatile uint32 _adcGenMaxInterval;

void ADC_InitGenerator(uint32 minValue, uint32 maxValue)
{
    _adcGenMinInterval = minValue;
    _adcGenMaxInterval = maxValue;
    
    _adcIsGeneratorInit = true;
}

void ADC_RandomTask()
{
    if (!_adcIsGeneratorInit)
        ADC_InitGenerator(1000, 30000);
    
    for (uint32 i = 0; i < 4; i++)
    {
        uint32 elapsed = SysTick_GetElapsedTime(_adcRandomContext[i].lastChange);
        if (elapsed > _adcRandomContext[i].nextChange)
        {
            _adcRandomContext[i].lastChange = SysTick_GetTickCount();

            if (_ADCValue[i] != 0)
                _ADCValue[i] = 0;
            else
                _ADCValue[i] = 1023;

            _adcRandomContext[i].nextChange = Test_GetRandomValue(_adcGenMinInterval, _adcGenMaxInterval);
        }
    }
}

#endif // DEBUG_GENERATOR


/*****************************************************************************
** Function name:		ADCInit
**
** Descriptions:		initialize ADC channel
**
** parameters:			ADC clock rate
** Returned value:		None
** 
*****************************************************************************/
void ADC_Init(uint32 ADC_Clk)
{
    uint32 i;
    
    /* Disable Power down bit to the ADC block. */  
    LPC_SYSCON->PDRUNCFG &= ~(0x1 << 4);
    
    /* Enable AHB clock to the ADC. */
    LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 14);
    
    for (i = 0; i < ADC_NUM; i++)
    {
       _ADCValue[i] = 0x0;
    }
    
    /* ADC IN0 */
    LPC_IOCON->PIO0_30 = 0x03; // AD0 func, analog mode, no-pullup
    /* ADC IN1 */
    LPC_IOCON->PIO0_31 = 0x03;    
    /* ADC IN2 */
    LPC_IOCON->PIO1_0 = 0x02;
    /* ADC IN3 */
    LPC_IOCON->PIO1_1 = 0x02;
    
    LPC_ADC->CR = 
        (0x01 << 0) |       /* SEL=1,select channel 0~7 on ADC0 */
        ((_systemAHBFrequency / ADC_Clk - 1) << 8) |  /* CLKDIV = Fpclk / 1000000 - 1 */ 
        (0 << 16) | 		/* BURST = 0, no BURST, software controlled */
        (0 << 17) |  		/* CLKS = 0, 11 clocks/10 bits */
        (1 << 21) |  		/* PDN = 1, normal operation */
        (0 << 22) |  		/* TEST1:0 = 00 */
        (0 << 24) |  		/* START = 0 A/D conversion stops */
        (0 << 27);		/* EDGE = 0 (CAP/MAT singal falling,trigger A/D conversion) */ 
    
    NVIC_EnableIRQ(ADC_IRQn);
    LPC_ADC->INTEN = 0x100;//FF;		// Enable all interrupts 
    
    /* Initialize select pins (special for the DCU) */
    GPIO_SetDir(PORT0, SEL0, GPIO_OUTPUT);
    GPIO_SetDir(PORT0, SEL1, GPIO_OUTPUT);
    GPIO_SetDir(PORT0, SEL2, GPIO_OUTPUT);
    /* Select reference on ADC0 channel */
    ADC_SelectInputs(ADC_Reference);
    
    return;
}

/*****************************************************************************
** Function name:		ADCRead
**
** Descriptions:		Read ADC channel
**
** parameters:			Channel number
** Returned value:		Value read, if interrupt driven, return channel #
** 
*****************************************************************************/
//volatile uint32 _adcCnt = 0;
//volatile uint32 _adcMaxCnt = 0;
//uint32 ADC_Read(uint32 channelNum)
//{
//    uint32 regVal, ADC_Data;
//
//    /* channel number is 0 through 4 */
//    if (channelNum >= ADC_NUM)
//    {
//        channelNum = 0;		/* reset channel number to 0 */
//    }
//    
//    LPC_ADC->CR &= 0xFFFFFF00;
//    LPC_ADC->CR |= (1 << 24) | (1 << channelNum);	/* switch channel,start A/D convert */
//    
//    while (1)			                            /* wait until end of A/D convert */
//    {
//        //regVal = *(volatile unsigned long *)(LPC_ADC_BASE 
//        //        + ADC_OFFSET + ADC_INDEX * channelNum);
//        /* read result of A/D conversion */
//        //if (regVal & ADC_DONE)
//        if (LPC_ADC->GDR & (uint32)(1 << 31))  /* check if channel is done */
//        {
//            break;
//        }
//        
//        _adcCnt++;
//    }	
//        
//    if (_adcCnt > _adcMaxCnt)
//        _adcMaxCnt = _adcCnt;
//    _adcCnt = 0;
//    
//    LPC_ADC->CR &= 0xF8FFFFFF;	/* stop ADC now */    
//    if (LPC_ADC->GDR & (1 << 30))	/* save data when it's not overrun, otherwise, return zero */
//    {
//        return (0);
//    }
//    
//    regVal = LPC_ADC->GDR;
//    ADC_Data = (regVal >> 6) & 0x3FF;
//
//    return (ADC_Data);	/* return A/D conversion value */
//}

uint32 ADC_IsValid()
{
    return _adcValid;
}

volatile uint32 _readingNow = 0;

#pragma diag_suppress=Pa082
/*****************************************************************************
** Function name:		ADC_ReadAll
**
** Descriptions:		Read all inputs on DCU and adjust their value to Ref.
**
** parameters:			None
** Returned value:		None
** 
*****************************************************************************/
//void ADC_ReadAll() 
//{
//    uint32 elapsedTime = 0;
//    uint32 i = 0;
//    
//    switch (_adcState)
//    {
//        /* Select reference */
//        case 0:     
//            _readingNow = 0xff;
//            ADC_SelectInputs(_readingNow);
//            _adcTimer = SysTick_GetTickCount();
//            _adcState = 1;
//            break;
//            
//        /* Wait 1 ms */
//        case 1:
//            elapsedTime = SysTick_GetElapsedTime(_adcTimer);
//            if (elapsedTime > 1)
//                _adcState = 2;
//            break;
//            
//        /* Read reference & select first input */
//        case 2:
//            _readingNow = 0;
//            //_referenceValue = ADC_Read(0);
//            _adcState = 3;
//            break;
//            
//        case 3:
//            // select required inputs
//            ADC_SelectInputs(_readingNow);
//            _adcTimer = SysTick_GetTickCount();
//            _adcState = 4;
//            break;
//            
//        /* Wait 1 ms */
//        case 4:
//            elapsedTime = SysTick_GetElapsedTime(_adcTimer);
//            if (elapsedTime > 1)
//                _adcState = 5;
//            break;
//            
//        /* Read all inputs */
//        case 5:
//            for (i = 0; i < ADC_NUM; i++)
//            {
//                uint32 valueIndex = i + (ADC_NUM * _readingNow);
//                _ADCValue[valueIndex] = ADC_Read(i);
//                float tmpValue = (float)((_ADCValue[valueIndex] * ADC_MAX_VALUE) / _referenceValue);
//                _ADCValue[valueIndex] = (uint32)tmpValue;
//            }        
//            
//            _readingNow++;
//            if (_readingNow > 3)
//            {
//                _adcState = 0;
//                //_adcValid = true;
//            }
//            else
//            {
//                _adcState = 3;
//            }
//            break;
//    }
//}
//#pragma diag_default=Pa082
//

#ifndef RS485 // this conditioning marks old 1.7.approach
#ifndef LON
void ADC_Task()
{
    ADC_ReadAll();
}
#endif
#endif

void ADC_StartNewConversion()
{
    // select channel 0
    LPC_ADC->CR &= 0xFFFFFF00;      // clear all channels 7:0
    LPC_ADC->CR |= ADC_START_CONVERSION | (1 << 0);	/* select channel 0 & start conversion */
}

#ifndef BOOTLOADER
void ADC_IRQHandler()
{
    uint32 regVal = LPC_ADC->GDR;
    uint32 currentChannel = (regVal >> 24) & 0x07;
    uint32 resultValue = (regVal >> 6) & 0x3FF;
    
    LPC_ADC->CR &= 0xF8FFFFFF;	/* stop ADC now */    
    
    if (_adcContext.channelSource == ADC_Reference)
        _referenceValue = resultValue; 
    else
    {
#ifdef DEBUG_GENERATOR
        if (!_testADCGenEnabled)
        {
#endif
            uint32 valueIndex = currentChannel + (ADC_CHANNEL_CNT * (_adcContext.channelSource - 1));
            _ADCValue[valueIndex] = resultValue;
            
            // adjust with regards to reference value
            if (_referenceValue != 0)
            {
                float tmpValue = (float)((_ADCValue[valueIndex] * ADC_MAX_VALUE) / _referenceValue);
                _ADCValue[valueIndex] = (uint32)tmpValue;
            }
#ifdef DEBUG_GENERATOR                
        }
#endif
    }
    
    // prepare next conversion
    if (currentChannel >= (ADC_CHANNEL_CNT - 1) || _adcContext.channelSource == ADC_Reference)
    {
        // select next channel and start conversion
        _adcContext.channelSource++;
        if (_adcContext.channelSource > ADC_Ext912)
        {
            _adcContext.channelSource = ADC_Reference;
            _adcValid = true;
        }
        ADC_SelectInputs(_adcContext.channelSource);
    }
    else
    {
        currentChannel++;
        // select 
        LPC_ADC->CR &= 0xFFFFFF00;      // clear all channels 7:0
        LPC_ADC->CR |= ADC_START_CONVERSION | (1 << currentChannel);	/* switch channel,start A/D convert */
    }
}
#endif
/*********************************************************************************
**                            End Of File
*********************************************************************************/
