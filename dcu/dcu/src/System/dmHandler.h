#pragma once
#include "baseTypes.h"

/// if this preprocessor directive defined, the dmHandler is using 
/// malloc/free instead of internal implementation
#define DM_HANDLER_MALLOC

void* New(uint32 size);
bool Delete(void* pointer);


#ifndef BOOTLOADER
uint32 AvailableSpace(void* pointer);

uint32 MemoryLoad();
#endif

