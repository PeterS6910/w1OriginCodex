#ifndef __DIPSWITCH_H
#define __DIPSWITCH_H

#include "System\baseTypes.h"

#define DIP_COUNT   5

//void InputLeds_Task();

void DIP_Init();

int32 DIP_GetFrom(uint32 index);

#endif