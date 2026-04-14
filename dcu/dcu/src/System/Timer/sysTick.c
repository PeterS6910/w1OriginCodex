#include "sysTick.h"
#include "timer.h"
#include "System\LPC122x.h"

volatile uint32 _sysTickCount;

#ifndef BOOTLOADER
volatile uint32 _overflowCount;
volatile uint32 _milisecondsPerSecondCounter = 0;
volatile uint32 _secondOfTheDay = 0;
#endif

#define SECONDS_PER_DAY 86400

/*
#define MAX_BASIC_TIMER_TASKS 4

typedef void(*PFunction)(void);

PFunction _criticalTasks[MAX_BASIC_TIMER_TASKS];

void SysTick_RegisterCriticalTask(void (*f)(void)) {
    if (f == 0)
        return;
    
    for(int i=0;i<MAX_BASIC_TIMER_TASKS; i++)
        if (_criticalTasks[i] == 0)
        {
            _criticalTasks[i] = f;
            break;
        }
}
*/


/**
 * Interrupt handler
 */
#ifndef BOOTLOADER
void SysTick_Handler(void) 
#else
void SysTick_HandlerBoot(void) 
#endif
{
    _sysTickCount++;
    if (_sysTickCount > OVERFLOWING_TIME) {
        _sysTickCount = 0;
#ifndef BOOTLOADER
        _overflowCount++;
#endif
    }
    
#ifndef BOOTLOADER
    //if (_sysTickCount % 10 == 0) {
        _milisecondsPerSecondCounter++;
        
        if (_milisecondsPerSecondCounter >= 1000) {
            _milisecondsPerSecondCounter=0;
            if (_secondOfTheDay < SECONDS_PER_DAY - 1)
                _secondOfTheDay++;
            else
                _secondOfTheDay = 0;
        }
        
        // handling of the critical tasks
        /*
        for(int i=0;i<MAX_BASIC_TIMER_TASKS; i++) {
            if (_criticalTasks[i] != 0)
                _criticalTasks[i]();
            else 
                break;
        }*/
    //}
#endif
}


/**
 * Initializes and starts the sysTick timer supported by Cortex ARM processors
 */
void SysTick_InitMainTimer() {
    _sysTickCount = 0;// 0x7FFFE4A7; just testing 7s before overflow
    
#ifndef BOOTLOADER
    _overflowCount = 0; //2;
#endif

    /*for(int i=0;i<MAX_BASIC_TIMER_TASKS; i++) {
        _criticalTasks[i] = 0;
    }*/
    
    SysTick_Config(ONE_MS_TIME_INTERVAL); //ONE_TENTH_MS_TIME_INTERVAL);
}


/**
 *
 */
uint32 SysTick_GetTickCount() {
    return _sysTickCount;
}

/**
 *
 */
uint32 SysTick_GetElapsedTime(uint32 pastTickCount) 
{
    uint32 ret = 0;
    if (pastTickCount <= OVERFLOWING_TIME)
    {
        uint32 actual = _sysTickCount; // SNAPSHOT
        if (actual != pastTickCount)
        {
            if (pastTickCount < actual) 
                ret = (actual - pastTickCount); // / 10;
            else
                ret = ((OVERFLOWING_TIME - pastTickCount)+actual +1); // / 10;
        }
    }
    return ret;
}

/*
uint32 SysTick_GetPreciseElapsedTime(uint32 pastTickCount) {
    if (_sysTickCount == pastTickCount)
        return 0;
    
    uint32 actual = _sysTickCount;
    if (pastTickCount <= actual) 
        return (actual - pastTickCount) ;
    else
        return ((0xFFFFFFFF - pastTickCount)+actual );
}
*/

#ifndef BOOTLOADER
/**
 *
 */
void SysTick_Delay(uint32 delayInMs) {
    if (delayInMs == 0 ||
        delayInMs > OVERFLOWING_TIME)
        return;
    
    uint32 stc = SysTick_GetTickCount();
    uint32 elapsed;
    do {
        for(uint32 j=0;j<0x0FFF;j++)   // 0xFFF = 4096 is around 3900, which is number of one-cycle instruction to be fitted into one tenth of MS
        	__ASM("NOP"); // this is intentional , DO NOT WITHDRAW
        elapsed = SysTick_GetElapsedTime(stc);
    }
    while(elapsed < delayInMs);
       
}

uint32 SysTick_GetHour() {
    return (_secondOfTheDay / 3600);
}

uint32 SysTick_GetMinute() {
    return (_secondOfTheDay %  3600) / 60;
}

uint32 SysTick_GetSecond() {
    return _secondOfTheDay % 60;
}

Result_e SysTick_SetTime(uint32 hour, uint32 minute, uint32 second)
{
    if (hour > 23)
        return R_InvalidParameter;
    if (minute > 59)
        return R_InvalidParameter;
    if (second > 59)
        return R_InvalidParameter;
    
    _secondOfTheDay = (hour * 3600) + (minute * 60) + second;
    
    return R_Ok;
}
#endif
