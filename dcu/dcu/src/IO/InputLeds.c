#include "System\GPIO\gpio.h"
#include "InputLeds.h"
#include "System\Timer\sysTick.h"
#include "System\systemInfo.h"
#include "IO\IOExpansion.h"
#include "System\tasks.h"

volatile InputState_e _infoLed[MAX_INPUT_COUNT];

volatile LEDStateDescriptor_t _shortTimer;
volatile LEDStateDescriptor_t _breakTimer;
volatile LEDStateDescriptor_t _unknownTimer;

volatile bool _disableAutoNotification = true;

bool InputLeds_InternalSetLed(uint32 ledId, bool isOn) 
{
    if (ledId >= SysInfo_GetInputCount())
        return false;
    
    if (ledId < BASE_INPUT_COUNT)
        GPIO_SetValue(PORT1, 2 + ledId, !(isOn & 0x01));
    else
        IOExp_SetLed(ledId - BASE_INPUT_COUNT, isOn);
    
    return true;        
}

bool InputLeds_SetLed(uint32 ledId, bool isOn) {
    if (!_disableAutoNotification)
        return false;
    else
        return InputLeds_InternalSetLed(ledId,isOn);
    
}

void InputLeds_SetState(uint32 ledId, InputState_e state)
{
    if (ledId >= SysInfo_GetInputCount())
        return;
    
    _infoLed[ledId] = state;
    
    if (_disableAutoNotification) 
        return;
    
    switch (state)
    {
        case Normal:
            InputLeds_InternalSetLed(ledId, 0);
            break;
        case Alarm:
            InputLeds_InternalSetLed(ledId, 1);
            break;
        case Short:
            //if (ledId < BASE_INPUT_COUNT)
                InputLeds_InternalSetLed(ledId, _shortTimer.isOn);
            //else
            //    IOExp_SetLedState(ledId - BASE_INPUT_COUNT, Short);
            break;
        case Break:
            //if (ledId < BASE_INPUT_COUNT)
                InputLeds_InternalSetLed(ledId, _breakTimer.isOn);
            //else
            //    IOExp_SetLedState(ledId - BASE_INPUT_COUNT, Break);
            break;
        case Unknown:
            InputLeds_InternalSetLed(ledId, _unknownTimer.isOn);
    }
}

bool InputLeds_DisableAutoNotification() {
    if (_disableAutoNotification)
        return false;
    
    _disableAutoNotification = true;

    for(int32 i=0;i<SysInfo_GetInputCount();i++)
        InputLeds_InternalSetLed(i,false);
    
    return true;
}

bool InputLeds_EnableAutoNotification() {
    if (!_disableAutoNotification)
        return false;
    
    _disableAutoNotification = false;
    
    for (uint32 i = 0; i < SysInfo_GetInputCount(); i++)
    {
        InputLeds_SetState(i, _infoLed[i]);
    }
    
    return true;
}

void InputsLeds_UpdateLEDs(InputState_e state)
{
    if (_disableAutoNotification)
        return;
    
    for (int32 i = 0; i < SysInfo_GetInputCount(); i++)
    {
        if (_infoLed[i] == state)
        {
            switch (state)
            {
                case Short:
                    //if (i < BASE_INPUT_COUNT)
                        InputLeds_InternalSetLed(i, _shortTimer.isOn);
                    break;
                case Break:
                    //if (i < BASE_INPUT_COUNT)
                        InputLeds_InternalSetLed(i, _breakTimer.isOn);
                    break;
                case Unknown:
                    InputLeds_InternalSetLed(i, _unknownTimer.isOn);
                    break;
            }
            
        }
    }
}

void InputLeds_Task()
{
    uint32 elapsed;
    
    elapsed = SysTick_GetElapsedTime(_shortTimer.changeTime);
    if (elapsed >= SHORT_TIME)
    {
        _shortTimer.changeTime = SysTick_GetTickCount();
        _shortTimer.isOn = !_shortTimer.isOn;
        InputsLeds_UpdateLEDs(Short);
    }
    
    elapsed = SysTick_GetElapsedTime(_breakTimer.changeTime);
    if (elapsed >= BREAK_TIME)
    {
        _breakTimer.changeTime = SysTick_GetTickCount();
        _breakTimer.isOn = !_breakTimer.isOn;
        InputsLeds_UpdateLEDs(Break);
    }
    
    elapsed = SysTick_GetElapsedTime(_unknownTimer.changeTime);
    if (elapsed >= UNKNOWN_TIME)
    {
        _unknownTimer.changeTime = SysTick_GetTickCount();
        _unknownTimer.isOn = !_unknownTimer.isOn;
        InputsLeds_UpdateLEDs(Unknown);
    }
}

void InputLeds_Init() 
{
    for(int i=2;i<=5;i++) {
        GPIO_SetDir(PORT1, i, GPIO_OUTPUT);  
        // switch the LEDs off
        GPIO_SetValue(PORT1, i, 1);
    
    }
    /*
    GPIO_SetDir(PORT1, 2, GPIO_OUTPUT);
    GPIO_SetDir(PORT1, 3, GPIO_OUTPUT);
    GPIO_SetDir(PORT1, 4, GPIO_OUTPUT);
    GPIO_SetDir(PORT1, 5, GPIO_OUTPUT);
    GPIO_SetValue(PORT1, 2, 1);
    GPIO_SetValue(PORT1, 3, 1);
    GPIO_SetValue(PORT1, 4, 1);
    GPIO_SetValue(PORT1, 5, 1);*/
    
    int32 i;
    for (i = 0; i < SysInfo_GetInputCount(); i++) 
        InputLeds_SetState(i, Unknown);
    
    _shortTimer.changeTime = SysTick_GetTickCount();
    _shortTimer.isOn = 0;
    _breakTimer.changeTime = SysTick_GetTickCount();
    _breakTimer.isOn = 0;
    _unknownTimer.changeTime = SysTick_GetTickCount();
    _unknownTimer.isOn = 0;
    
    Tasks_Register(InputLeds_Task, 10);
    
    InputLeds_EnableAutoNotification();
}
