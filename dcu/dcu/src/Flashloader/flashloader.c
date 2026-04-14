#include <stdint.h>

#include "System\constants.h"
#include "flashloader.h"
#include "System\IAP\iap.h"

#include "System\GPIO\gpio.h"
#include "System\lpc122x.h"
#include "System\Timer\sysTick.h"

volatile uint32 _destinationAddress;
volatile uint32 _nextPage;

volatile uint32 _partialChecksum;

volatile uint32 _lastUpgradeActivity;

//void flash_RefreshActivity()
//{
//    _lastUpgradeActivity = SysTick_GetTickCount();
//}

uint32 flash_TimeFromLastActivity()
{
    return SysTick_GetElapsedTime(_lastUpgradeActivity);
}

uint32 flash_getLastChecksum()
{
    return _partialChecksum & 0xFFFF;
}

int32_t flash_CheckApplication()
{
    //flash_RefreshActivity();
    _lastUpgradeActivity = SysTick_GetTickCount();
    
    uint32* appLength = (uint32*)APPLICATION_LENGTH_ADDRESS;
    uint32* appChecksum = (uint32*)APPLICATION_CHECKSUM_ADDRESS;
    uint8* application = (uint8*)APPLICATION_BASE_ADDRESS + 8;
    
    
    uint32 maxSize = LAST_FLASH_ADDRESS - APPLICATION_BASE_ADDRESS;
    if (*appLength > maxSize)
        return -1;
       
    //uint32 x = 4;
    uint32 i;
    uint32 calcChecksum = 0;
    
    for (i = 0; i < *appLength; i++)
    {
        calcChecksum += application[i];
    }
    
    if (calcChecksum == *appChecksum)
        return 0;
    else 
        return -1;
}

//Result_e flash_WriteToAddress(uint32 address, uint8* dataToWrite, uint32 length)
//{
//    flash_RefreshActivity();
//    
//    if (length > FLASH_MAX_BUFFER)
//        return BufferTooSmall;
//    
//    uint32 startSector, endSector;
//    uint32 writeBuffer[FLASH_MAX_BUFFER];
//    
//    startSector = address / 0x1000;
//    endSector = (address + length - 1) / 0x1000;
//    
//    memcpy(writeBuffer, dataToWrite, length);
//    
//    Result_e result = Ok;
//    
//    EnterCriticalSection();
//        
//    if (IAP_PrepareSector(startSector, endSector) != IAP_CmdSuccess)
//        result = PrepareSectorError;
//        
//    if (result == Ok)
//    {
//        if (IAP_CopyRAMToFlash(address, (uint32)writeBuffer, length) != IAP_CmdSuccess)
//            result = CopyRamToFlashError; 
//    }
//    
//    LeaveCriticalSection();
//    
//    return result;
//}

Result_e flash_WriteData(uint32 blockNumber, uint8* dataToWrite, uint32 length)
{
    //flash_RefreshActivity();
    _lastUpgradeActivity = SysTick_GetTickCount();
    
    uint32 destinationAddr = APPLICATION_BASE_ADDRESS;
    destinationAddr += blockNumber * UPGRADE_CHUNK_SIZE;
    
    if (destinationAddr + length >= LAST_FLASH_ADDRESS)
        return R_CopyRamToFlashError;
    
    /* Shift all because at the first 8 bytes will come app. length & checksum */
//    if (blockNumber == 0)
//        destinationAddr += 8;
    
    
    //Result_e result = flash_WriteToAddress(destinationAddr, dataToWrite, length);
    
    //flash_RefreshActivity();
    _lastUpgradeActivity = SysTick_GetTickCount();
    
    if (length > FLASH_MAX_BUFFER)
        return R_BufferTooSmall;
    
    uint32 startSector, endSector;
    uint32 writeBuffer[FLASH_MAX_BUFFER];
    
    startSector = destinationAddr / 0x1000;
    endSector = (destinationAddr + length - 1) / 0x1000;
    
    memcpy(writeBuffer, dataToWrite, length);
    
    Result_e result = R_Ok;
    
    EnterCriticalSection();
        
    if (IAP_PrepareSector(startSector, endSector) != IAP_CmdSuccess)
        result = R_PrepareSectorError;
        
    if (result == R_Ok)
    {
        if (IAP_CopyRAMToFlash(destinationAddr, (uint32)writeBuffer, length) != IAP_CmdSuccess)
            result = R_CopyRamToFlashError; 
    }
    
    LeaveCriticalSection();
    
    uint8* data = (uint8*)destinationAddr;
    
    _partialChecksum = 0;
    uint32 i;
    for (i = 0; i < length; i++)
        _partialChecksum += data[i];
    
    return result;
}

Result_e flash_ErasePage(uint32 page)
{
    //flash_RefreshActivity();
    _lastUpgradeActivity = SysTick_GetTickCount();
    
    if (page >= 128)
        return R_ErasePageError;
    
    EnterCriticalSection();
    
    uint32 sector = page / 8;
    
    Result_e result = R_Ok;
    
    if (IAP_PrepareSector(sector, sector) != IAP_CmdSuccess)
        result = R_PrepareSectorError;
    
    if (result == R_Ok)
    {
        if (IAP_ErasePage(page, page) != IAP_CmdSuccess)
            result = R_ErasePageError;
    }
    
    LeaveCriticalSection();
    
    return result;
}
