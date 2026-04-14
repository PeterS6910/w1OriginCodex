#pragma once
#include "System\baseTypes.h"

void Test_InitLEDTest();
void Test_LEDTask();

void Test_StartIOTest(uint32 isInverted);

#ifdef DEBUG_GENERATOR

typedef enum 
{
    Test_MifareCSN = 0,
    Test_MifareSector = 1
} Test_CardType_e;

uint32 Test_GetRandomValue(uint32 from, uint32 to);

void Test_CardGeneratorTask();

void Test_ToggleCardGenerator(bool enable, Test_CardType_e cardType, int minTime, int maxTime);

void Test_ToggleADCGenerator(bool enable, int minTime, int maxTime);

#endif