#pragma once

#include "System\constants.h"

#define SPECIAL_INPUT_COUNT 11
#define UNREAD_VALUE        0xff

typedef enum
{
    Normal = 0,
    Alarm = 1,
    Short = 2,
    Break = 3,
    Unknown = 0xff,
        
} InputState_e;

typedef struct 
{
#ifndef BOOTLOADER
    uint8 tamper : 1;
    //uint8 dip1 : 1;
    //uint8 dip2 : 1;
    uint8 address : 5;
    uint8 dip6 : 1;
    uint8 fuse: 1;
    uint8 extFuse : 1;
    uint8 extPower : 1;
#else
    uint32 address;
#endif
} SpecialInputState_t;

typedef enum
{
    IT_DIGITAL = 0,
    IT_BALANCED = 1,
    IT_SPECIAL = 2,
    IT_UNCONFIGURED = 0xFFFFFFFF,
        
} InputType_e;