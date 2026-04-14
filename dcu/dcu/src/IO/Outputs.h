#ifndef __OUTPUTS_H
#define __OUTPUTS_H

#include "System\constants.h"
#include "System\baseTypes.h"
#include "System\resultCodes.h"

#ifndef BOOTLOADER
#include "DSM\dsmStructures.h"
#endif

#define OUTPUT_PERIOD 5

typedef enum
{
    OCH_LEVEL = 0, 
    OCH_FREQUENCY = 1,
    OCH_PULSE = 2,
        
    OCH_MAX_UINT = 0xFFFFFFFF
} OutputCharacteristic_e;

typedef struct 
{
    OutputCharacteristic_e functionality;   /* Level, frequency, pulse functionality */
    uint32 pulseLength;           /* Length of the pulse in ms */
    uint32 pulseDelay;            /* Length of the delay in ms */
    uint32 onOffTimer;            /* helper timer for delay to ON/OFF calculation */
    uint32 pulseTimer;            /* helper timer for pulse length/delay calculation */
    uint32 delayToOn;             /* delay before output physicaly activated */
    uint32 delayToOff;            /* delay before output physicaly deactivated */
    uint32 logicStatus : 2;        /* logic status: outputs is in on, off, delay to on, or delay to off */
    uint32 funcStatus : 2;         /* functional status: outputs is physically on, off, frequency or pulse */
    uint32 activated : 1;          /* indicates whether output has been activated */
    uint32 inverted : 1;           /* indicates output in inverted funtionality */
    uint32 forcedOff : 1;          /* forceOff flag */
    uint32 isOn : 1;               /* indicates whether is output relay is currently ON/OFF */
    int32 timesActivated;         /* helper variable to count the number output was activated */
} OutputDescription_t;

void Outputs_BindOnOutputChanged(void (*onOutputChanged)(uint32 outputId, uint32 isOn));

void Outputs_BindOnOutputLogicStateChanged(void (*onOutputLogicStateChanged)(uint32_t outputID, uint32_t active));

void Outputs_FireOutputLogicChanged(uint32 outputID, bool isOn);

void Outputs_ReportCurrentLogicState(uint32 outputID);

#ifndef BOOTLOADER
Result_e Outputs_SetReportedOutputs(uint32 outputIDs);

Result_e Outputs_SetReportedOutputsEx(uint32 outputIDs);

Result_e Outputs_UnsetReportedOutputs(uint32 outputIDs);

Result_e Outputs_SetReportedOutputsLogic(uint32 outputIDs);

Result_e Outputs_SetReportedOutputsLogicEx(uint32 outputIDs);

Result_e Outputs_UnsetReportedOutputsLogic(uint32 outputIDs);

Result_e Outputs_SetBlockedOutputsEx(uint32 outputIDs);
#endif

void Outputs_Init();

//int Outputs_SetOutput(byte iOutputId, byte isOn);   // !! use other functionality

Result_e Outputs_TurnOff(uint32_t outputID);

Result_e Outputs_TurnAllOff();

int Outputs_ConfigLevel(uint32 outputId, uint32 delayToOn, uint32 delayToOff,
                        uint32 inverted);

int Outputs_ConfigFrequency(uint32 outputId, uint32 onTime, uint32 offTime,
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted);

int Outputs_ConfigPulse(uint32 outputId, uint32 pulseTime, 
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted);

int Outputs_EnsureMinDelayToOn(uint32 outputId, uint32 minDelayToOn);

int Outputs_InvertOutput(uint32 outputId, uint32 isInverted);
#ifndef BOOTLOADER
Result_e Outputs_ActivateOutput(uint32 outputId, uint32 activate);
#endif

uint32 Outputs_IsActivated(uint32 outputID);

void Outputs_Task();

#endif