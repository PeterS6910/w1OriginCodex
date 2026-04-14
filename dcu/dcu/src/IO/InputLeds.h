#ifndef __INPUTLEDS_H
#define __INPUTLEDS_H

#include "InputsEnums.h"
#include "System\baseTypes.h"

#define UNKNOWN_TIME    500

#define SHORT_TIME      100     //104 if combined with i2c frequency handling
#define BREAK_TIME      200     //209 if combined with i2c frequency handling

typedef struct 
{
    uint32 changeTime;
    bool isOn;
} LEDStateDescriptor_t;

void InputLeds_Init();

bool InputLeds_SetLed(uint32 ledId, bool isOn);
void InputLeds_SetState(uint32 ledId, InputState_e state);

bool InputLeds_DisableAutoNotification();
bool InputLeds_EnableAutoNotification();

#endif