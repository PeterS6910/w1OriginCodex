#pragma once
#include "System\resultCodes.h"
#include "System\baseTypes.h"

uint32 flash_TimeFromLastActivity();

uint32 flash_getLastChecksum();

int32 flash_CheckApplication();

Result_e flash_WriteData(uint32 blockNumber, byte* dataToWrite, uint32 length);

Result_e flash_ErasePage(uint32 page);

void flash_UpgradeTask();