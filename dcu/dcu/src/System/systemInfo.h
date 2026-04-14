#pragma once

#include "System\baseTypes.h"
#include "System\resultCodes.h"

typedef enum 
{
    SysInfoNone = 0x00,
    SysInfoNoISP = 0x01,
    SysInfoCRP1 = 0x02,
    SysInfoCRP2 = 0x03,
    SysInfoCRP3 = 0x04,
            
} SysInfo_CRPLevel_e;

void SysInfo_Init(uint32 inputCount, uint32 outputCount);

void SysInfo_SetInputCount(uint32 inputCount);

void SysInfo_SetOutputCount(uint32 outputCount);

Result_e SysInfo_SetCRPLevel(SysInfo_CRPLevel_e level);

uint32 SysInfo_GetCRPLevel();

uint32 SysInfo_GetInputCount();

uint32 SysInfo_GetOutputCount();

void SysInfo_SignalAddressConflict(bool doSignal);

void SysInfo_SignalConnectionDown(bool doSignal);

void SysInfo_StopSignalling();

void SysInfo_RestartDCUDelayed();

void SysInfo_ResetToBootloaderDelayed();
void SysInfo_ResetToApplicationDelayed();

void SysInfo_DelayTask(DVoid2Void task, uint32 restartDelay);

#ifndef BOOTLOADER
void SysInfo_GetBootloaderVersion(uint32* version,uint32* revision);
#endif
