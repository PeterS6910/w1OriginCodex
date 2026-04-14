#pragma once
#include "System\baseTypes.h"
#include "IO\InputsEnums.h"

typedef enum 
{
    Ext12I = 0,
    Ext8I_4O = 1,
    Ext4I_8O = 2,
    Ext8O = 3,
    ExtUnknown = 0xfe,
    ExtNone = 0xff
} ExtensionBoard_e;

ExtensionBoard_e GetExtBoardType();

void IOExp_SetLed(uint32 ledID, uint32 isOn);

void IOExp_SetLedState(uint32 ledId, InputState_e state);

void IOExp_SetOutput(uint32 outputID, uint32 isOn);

InputState_e IOExp_ReadFuse();
InputState_e IOExp_ReadPower();

void InitIOExpansion();