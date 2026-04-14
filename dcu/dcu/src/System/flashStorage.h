#pragma once

#include "System\baseTypes.h"

#ifndef BOOTLOADER
typedef struct
{
    uint32_t testData;
} StorableData_t;

extern volatile StorableData_t _storableData;

void FS_LoadData();
void FS_SaveData();
#endif 