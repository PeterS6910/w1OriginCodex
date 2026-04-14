#include "flashStorage.h"
#include "System\IAP\iap.h"
#include "LPC122x.h"
#include "System\baseTypes.h"

#ifndef BOOTLOADER

#define STORAGE_BASE_ADDRESS    0xF000
#define STORAGE_DATA_CHUNK      256

volatile StorableData_t _storableData;

void FS_memcpy(uint8_t* source, uint8_t* destination, uint32_t length)
{
    uint32_t i;
    for (i = 0; i < length; i++)
    {
        destination[i] = source[i];
    }
}

void FS_memclear(uint8_t* destination, uint32_t length)
{
    uint32_t i;
    for (i = 0; i < length; i++)
        destination[i] = 0;
}

void FS_LoadData()
{
    uint32_t* flashPointer = (uint32_t*)STORAGE_BASE_ADDRESS;
    uint32_t* dataPointer = (uint32_t*)&_storableData;
    uint32_t dataSize = sizeof(_storableData);
 
    uint32_t i;
    for (i = 0; i < dataSize; i++)
    {
        dataPointer[i] = flashPointer[i];
    }
}

void FS_SaveData()
{
    uint8_t buffer[STORAGE_DATA_CHUNK];    
    
    uint32_t flashPointer = STORAGE_BASE_ADDRESS;
    
    uint8_t* dataPointer;
    uint32_t dataSize = sizeof(_storableData);
    
    dataPointer = (uint8_t*)&_storableData;
    
    EnterCriticalSection();
    IAP_PrepareSector(15, 15);
    IAP_EraseSector(15, 15);
    
    while (dataSize > 0)
    {
        FS_memclear(buffer, STORAGE_DATA_CHUNK);
        
        uint32_t dataToCopy;
        
        dataToCopy = (dataSize > STORAGE_DATA_CHUNK) ? STORAGE_DATA_CHUNK : dataSize;
        FS_memcpy(dataPointer, buffer, dataToCopy);
        
        IAP_PrepareSector(15, 15);
        IAP_CopyRAMToFlash(flashPointer, (uint32_t)buffer, dataToCopy);
        
        dataPointer += dataToCopy;
        dataSize -= dataToCopy;
    }
    
    LeaveCriticalSection();
}

#endif