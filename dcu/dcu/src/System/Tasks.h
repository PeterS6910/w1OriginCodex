#ifndef _TASKS_H
#define _TASKS_H

#include "System\Core\system_LPC122x.h"
#include "baseTypes.h"

#define SINGLE_TICK     1      // single tick in ms

int32 Tasks_Register(DVoid2Void task,uint32 priority);
void Tasks_Run();
//void Tasks_IncrementCounter();

void Tasks_Unregister(DVoid2Void task);
#ifndef BOOTLOADER
void Tasks_UnregisterAll();
#endif

#endif