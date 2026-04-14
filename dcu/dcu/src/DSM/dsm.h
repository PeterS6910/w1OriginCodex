#pragma once

#include "dsmConfig.h"
#include "CardReaders\CrProtocol.h"
 
void DSM_HostStateChanged(uint32 suggestedAddress, bool isAssigned);

void DSM_SendCRRawCmd(uint32 cr, CRMessageCode_t cmd, int32 param);

//void DSM_SendCRRawCmd(uint32 cr, CRMessageCode_t cmd);

Result_e DSM_SuppressCR(uint32 cr);

Result_e DSM_LooseCR(uint32 cr);

Result_e DSM_AssignCRs(uint8* data);

Result_e DSM_SetImplicitCRCode(uint32 address, uint32 code, uint32 useOptParam, uint32 optParam, 
                               uint32 setCR, uint32 msgCode, uint32 optDataLength, byte* optData,
                               bool intrusionOnlyViaLed);

//DSMSignals_e DSM_ForceUnlocked(uint32 isUnlocked,bool refreshCRs);

Result_e DSM_SignalAccessGranted(DSMAGSourceCard_e cardType);

void DSM_DoBinding();

Result_e DSM_Start(DoorEnviromentType_e enviromentType);

Result_e DSM_Stop();

void DSM_Task();

