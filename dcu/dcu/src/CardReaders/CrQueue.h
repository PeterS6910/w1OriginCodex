#pragma once
#include "System\baseTypes.h"
#include "CrProtocol.h"

uint32 CrQueue_Count(uint32 queueId);
void CrQueue_Clear(uint32 queueId);

bool CrQueue_Enqueue(uint32 queueId, CRMessage_t* message,bool onTop,bool shiftIfFull);
CRMessage_t* CrQueue_Pop(uint32 crAddress);

CRMessage_t* CrQueue_AllocRequest(uint32 odLength);
