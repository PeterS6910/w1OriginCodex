#pragma once
#include "System\resultCodes.h"
#include "System\baseTypes.h"

#define OVERFLOWING_TIME    0x7FFFFFFF

#ifndef BOOTLOADER
void SysTick_Handler(void);
#else
void SysTick_HandlerBoot(void);
#endif

void SysTick_InitMainTimer();

//void SysTick_RegisterCriticalTask(void (*f)(void));

uint32 SysTick_GetTickCount();

uint32 SysTick_GetElapsedTime(uint32 pastTickCount);
//uint32_t SysTick_GetPreciseElapsedTime(uint32_t pastTickCount);

#ifndef BOOTLOADER
void SysTick_Delay(uint32 delayInMs);

uint32 SysTick_GetHour();
uint32 SysTick_GetMinute();
uint32 SysTick_GetSecond();

Result_e SysTick_SetTime(uint32 hour, uint32 minute, uint32 second);
#endif