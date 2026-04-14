#include "systemInfo.h"
#include "System\constants.h"

#include "System\Timer\sysTick.h"
#include "System\GPIO\gpio.h"
#include "System\Tasks.h"
#include "IO\InputLeds.h"
#include "System\LPC122x.h"
#include "System\IAP\iap.h"
#include "System\UART\uart.h"
#include "System\I2C\i2c.h"

#ifndef BOOTLOADER
#include "app_version.h"
#else
#include "boot_version.h"
#endif

#define SYSINFO_CRP_ADDRESS 0x000002FC
#define SYSINFO_NOISP   0x4E697370
#define SYSINFO_CRP1    0x12345678
#define SYSINFO_CRP2    0x87654321
#define SYSINFO_CRP3    0x43218765

volatile uint32 _inputCount;
volatile uint32 _outputCount;

void SysInfo_Init(uint32 inputCount, uint32 outputCount)
{
    _inputCount = inputCount;
    _outputCount = outputCount;
}

void SysInfo_SetInputCount(uint32 inputCount)
{
    if (_inputCount < inputCount)
        _inputCount = inputCount;
}

void SysInfo_SetOutputCount(uint32 outputCount)
{
    if (_outputCount < outputCount)
        _outputCount = outputCount;
}

Result_e SysInfo_SetCRPLevel(SysInfo_CRPLevel_e level)
{
    uint32 value = 0x00;
    
    switch (level)
    {
        case SysInfoNoISP:
            value = SYSINFO_NOISP;
            break;
        case SysInfoCRP1:
            value = SYSINFO_CRP1;
            break;
        case SysInfoCRP2:
            value = SYSINFO_CRP2;
            break;
        case SysInfoCRP3:
            value = SYSINFO_CRP3;
            break;
        case SysInfoNone:
        default:
            value = 0x00000000;
            break;
    }
    
    IAP_PrepareSector(0, 0);
    IAP_StatusCodes_e result = IAP_CopyRAMToFlash(SYSINFO_CRP_ADDRESS, (uint32)(&value), 4);
    
    return (result == IAP_CmdSuccess ? R_Ok : R_Error);
}

uint32 SysInfo_GetCRPLevel()
{
    uint32* crpLevel = (uint32*)SYSINFO_CRP_ADDRESS;
    
    switch (*crpLevel)
    {
        case SYSINFO_NOISP:
            return 1;
        case SYSINFO_CRP1:
            return 2;
        case SYSINFO_CRP2:
            return 3;
        case SYSINFO_CRP3:
            return 4;
        default:    // none
            return 0;
    }
}

uint32 SysInfo_GetInputCount()
{
    return _inputCount;
}

uint32 SysInfo_GetOutputCount()
{
    return _outputCount;
}

volatile uint32 _signalStartTime = 0;
volatile uint32 _signalLedID = 2;
volatile uint32 _signalState = 0;

volatile bool _ledInitialized = 0;
volatile bool _signallingAddressConflict = 0;
volatile bool _signallingConnectionDown = 0;

void SysInfo_InitLEDs()
{
    _signalStartTime = SysTick_GetTickCount();
    
    for (int i=2;i<=5;i++)
    {
        GPIO_SetDir(PORT1, i, 1);
        GPIO_SetValue(PORT1, i, 1);
    }
      
    /*
    GPIO_SetDir(PORT1, 2, 1);
    GPIO_SetDir(PORT1, 3, 1);
    GPIO_SetDir(PORT1, 4, 1);
    GPIO_SetDir(PORT1, 5, 1);
    
    GPIO_SetValue(PORT1, 2, 1);
    GPIO_SetValue(PORT1, 3, 1);
    GPIO_SetValue(PORT1, 4, 1);
    GPIO_SetValue(PORT1, 5, 1);*/
    
    _ledInitialized = 1;
}

#ifndef BOOTLOADER
void SysInfo_SignalConnectionDownTask()
{
    uint32 elapsed = SysTick_GetElapsedTime(_signalStartTime);
    
    if (elapsed > 500)
    {
        GPIO_SetValue(PORT1, _signalLedID, 1);
        
        _signalLedID++;
        
        if (_signalLedID > 5)
            _signalLedID = 2;
        
        GPIO_SetValue(PORT1, _signalLedID, 0);
        
        _signalStartTime = SysTick_GetTickCount();
    }
}
#endif

void SysInfo_SwitchAll(uint32 value)
{
    for (int i=2;i<=5;i++)
    {
        GPIO_SetValue(PORT1, i, value);
    }
    /*GPIO_SetValue(PORT1, 2, value);
    GPIO_SetValue(PORT1, 3, value);
    GPIO_SetValue(PORT1, 4, value);
    GPIO_SetValue(PORT1, 5, value);*/
}

void SysInfo_SignalAddressConflictTask()
{
    uint32 elapsed = SysTick_GetElapsedTime(_signalStartTime);
    
    if (_signalState < 6)
    {
        if (elapsed > 100)
        {
            if (_signalState % 2)
                SysInfo_SwitchAll(1);
            else
                SysInfo_SwitchAll(0);
            
            _signalState++;
            
            _signalStartTime = SysTick_GetTickCount();
        }
    }
    else
    {
        SysInfo_SwitchAll(1);   // turn off 
        
        if (elapsed > 1000)
        {
            _signalState = 0;
            
            _signalStartTime = SysTick_GetTickCount();
        }
    }
}

#ifndef BOOTLOADER
void SysInfo_SignalAddressConflict(bool doSignal)
{
    if (doSignal)
    {
        if (!_signallingAddressConflict)
        {
            if (!_ledInitialized)
                SysInfo_InitLEDs();

            if (_signallingConnectionDown)
                SysInfo_SignalConnectionDown(0);

            InputLeds_DisableAutoNotification();
            
            Tasks_Register(SysInfo_SignalAddressConflictTask, 10);
            
            _signallingAddressConflict = 1;
        }
    }
    else 
    {
        if (_signallingAddressConflict)
        {
            InputLeds_EnableAutoNotification();
            
            Tasks_Unregister(SysInfo_SignalAddressConflictTask);
            
            _signallingAddressConflict = 0; 
        }
    }
}

#endif

#ifndef BOOTLOADER
void SysInfo_SignalConnectionDown(bool doSignal)
{
    if (doSignal)
    {
        if (_signallingConnectionDown)  // signalling already so no change required
            return;
        
        if (!_ledInitialized)
            SysInfo_InitLEDs();
        
        if (_signallingAddressConflict)
            SysInfo_SignalAddressConflict(0);
        
        InputLeds_DisableAutoNotification();
        
        _signalState = 0;
        Tasks_Register(SysInfo_SignalConnectionDownTask, 10);
        
        _signallingConnectionDown = 1;
    }
    else if (_signallingConnectionDown)
    {
        InputLeds_EnableAutoNotification();
        
        Tasks_Unregister(SysInfo_SignalConnectionDownTask);
        
        _signallingConnectionDown = 0;
    }
}
#endif

void SysInfo_StopSignalling()
{
    if (_signallingConnectionDown)
        SysInfo_SignalConnectionDown(0);
    if (_signallingAddressConflict)
        SysInfo_SignalAddressConflict(0);
}


volatile uint32 _taskDelayStarted;
volatile uint32 _taskDelay = 500;
volatile DVoid2Void _taskToDelay = null;

void SysInfo_DelayingTask()
{
    uint32 elapsed = SysTick_GetElapsedTime(_taskDelayStarted);
    if (elapsed > _taskDelay)
    {
        Tasks_Unregister(SysInfo_DelayingTask);
        if (_taskToDelay != null)
            _taskToDelay();
    }
}

void SysInfo_RestartDCUDelayed()
{
    NVIC_SystemReset();
}

extern void StartBootloader();
extern void StartApplication();

void SysInfo_DisableAllInterrupts()
{
#ifndef XPRESSO
	__disable_interrupt();
#else
    __disable_irq();
#endif
    
    // clear and stop ADC
    uint32 regVal = LPC_ADC->GDR;
    LPC_ADC->CR &= 0xF8FFFFFF;	/* stop ADC now */
    NVIC_DisableIRQ(ADC_IRQn);
    
    // clear UART 0 & 1
    UART_ClearInterrupts(LPC_UART0);
    NVIC_DisableIRQ(UART0_IRQn);
    UART_ClearInterrupts(LPC_UART1);
    NVIC_DisableIRQ(UART1_IRQn);
    
    // clear CB32B1 (lon timer)
    if (LPC_TMR32B1->IR & 0x01)
        LPC_TMR32B1->IR = 1;			// clear interrupt flag 
    if (LPC_TMR32B1->IR & (0x1 << 4))
        LPC_TMR32B1->IR = 0x1 << 4;			// clear interrupt flag 
    NVIC_DisableIRQ(TIMER_32_1_IRQn);
    
    // stop I2C
    I2C_ClearFlag(I2C_I2CONSET_AA | I2C_I2CONSET_SI | I2C_I2CONSET_STA | I2C_I2CONSET_I2EN);
    I2C_DeInit();
    NVIC_DisableIRQ(I2C_IRQn);
    
    // clear GPIO
    LPC_GPIO0->IC |= 0xffffffff;
    LPC_GPIO1->IC |= 0xffffffff;
    NVIC_DisableIRQ(EINT0_IRQn);
}

void SysInfo_ResetToBootloaderDelayed()
{
    SysInfo_DisableAllInterrupts();
    StartBootloader();
}

void SysInfo_ResetToApplicationDelayed()
{
    //System_PrepareForApplication();
    //StartApplication();
    SysInfo_DisableAllInterrupts();
    //__disable_interrupt();
    StartBootloader();
}

void SysInfo_DelayTask(DVoid2Void task, uint32 restartDelay)
{
    _taskDelayStarted = SysTick_GetTickCount();
    _taskDelay = restartDelay;
    _taskToDelay = task;
    Tasks_Register(SysInfo_DelayingTask, 10);
}

#ifndef BOOTLOADER
void SysInfo_GetBootloaderVersion(uint32* version,uint32* revision) {
    if (null != version && null != revision) 
    {       
        uint32* bootVerLocation = (uint32*)0x3ffc;
        uint32 bootVersion = *bootVerLocation;
        
        if ((((bootVersion & 0xFF000000) >> 24) == (bootVersion & 0x00FF)) && 
            (((bootVersion & 0x00FF0000) >> 8) == (bootVersion & 0xFF00)))
        {
            *version = ((bootVersion & 0xFF00) >> 8);
            *revision = (bootVersion & 0xFF);
        }
        else
        {
            *version = 0;
            *revision = 0;
        }
    }   
}
#endif