#pragma once
#include "System\baseTypes.h"

#define UID_LENGTH  16

typedef enum
{
    IAP_CmdSuccess = 0,
    IAP_InvalidCommand = 1,
    IAP_SrcAddrError = 2,
    IAP_DstAddrError = 3,
    IAP_SrcAddrNotMapped = 4,
    IAP_DstAddrNotMapped = 5,
    IAP_CountError = 6,
    IAP_InvalidSector = 7,
    IAP_SectorNotBlank = 8,
    IAP_SectorNotPrepared = 9,
    IAP_CompareError = 10,
    IAP_Busy = 11,
    IAP_InvalidParameter = 0xffffffff         /* added for more error checking */
} IAP_StatusCodes_e;

byte* IAP_ReadSerialNumber() ;

IAP_StatusCodes_e IAP_PrepareSector(uint32 startSecNum, uint32 endSecNum);
IAP_StatusCodes_e IAP_CopyRAMToFlash(uint32 dstAddress, uint32 srcAddress, uint32 number);
IAP_StatusCodes_e IAP_EraseSector(uint32 startSecNum, uint32 endSecNum);

IAP_StatusCodes_e IAP_ErasePage(uint32 startPage, uint32 endPage);