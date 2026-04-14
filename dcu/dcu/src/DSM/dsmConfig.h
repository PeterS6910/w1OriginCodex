#pragma once

#include "dsmStructures.h"
#include "System\constants.h"
#include "IO\InputsEnums.h"
#include "IO\Inputs.h"
#include "IO\Outputs.h"

void DSM_Init();

Result_e DSM_UnsetAll();

void DSM_BindOnStateChanged(void (*onStateChanged)(DSMSignals_e state, DSMSignalInfo_e info, DSMAccessGrantedSource_e pushButtonId));

Result_e DSM_InvokeIntrusionAlarm(uint32 invoke);

Result_e DSM_SetPushButton(uint32 sourceId, uint32 inputId, InputType_e inputType, uint32 inverted, 
                           uint32 delayToOn, uint32 delayToOff);
Result_e DSM_UnsetPushButton(uint32 sourceId);

Result_e DSM_SetSensor(DSMSensors_e sensorType, uint32 inputId, InputType_e inputType, uint32 inverted,
                       uint32 delayToOn, uint32 delayToOff);
Result_e DSM_UnsetSensor(DSMSensors_e sensorType);

Result_e DSM_SetElectricStrike(DSMActuators_e actuatorId, 
                                    uint32 outputId,
                                    DSMStrikeType_e strikeType,
                                    uint32 pulseTime,
                                    uint32 inverted,
                                    uint32 delayToOn,
                                    uint32 delayToOff);

Result_e DSM_SetBypassAlarm(uint32 outputId);

Result_e DSM_UnsetActuator(DSMActuators_e actuatorId);

void DSM_BindOnEnabledAlarmChanged(void (*onEnabledAlarmChanged)(uint32 doorAjarAlarm, uint32 intrusionAlarm, uint32 sabotageAlarm));

Result_e DSM_EnableAlarms(uint32 doorAjarAlarm, uint32 intrusionAlarm, uint32 sabotageAlarm);

Result_e DSM_SetTimming(uint32 unlockTime, uint32 openTime, uint32 preAlarmTime, uint32 sireneAjarDelay, uint32 beforeIntrusion);

void DSM_BindOnSpecialOutputAdded(void (*onSpecialOutputAdded)(DSMSpecialDOType_e outputType));

void DSM_BindOnSpecialOutputRemoved(void (*onSpecialOutputRemoved)(DSMSpecialDOType_e outputType));

Result_e DSM_SetSpecialOutput(DSMSpecialDOType_e outputType, uint32 outputId);

Result_e DSM_UnsetSpecialOutput(DSMSpecialDOType_e outputType);

/*(uint32 cr1, uint32 implicitCode1, uint32 isOptParam1, uint32 optParam1,
                       uint32 setCR1, uint32 msgCode1, uint32 optDataLength1, uint8* optData1,
                       uint32 cr2, uint32 implicitCode2, uint32 isOptParam2, uint32 optParam2,
                       uint32 setCR2, uint32 msgCode2, uint32 optDataLength2, uint8* optData2);*/



/* Inputs wrapper functions */
Result_e DSM_UnsetInput(uint32 index);

Result_e DSM_SetBsiLevels(uint32 toLevel1, uint32 toLevel2, uint32 toLevel3);

Result_e DSM_SetBsiParameters(uint32 index, 
                      uint32 filtertime,
                      uint32 delayOn, 
                      uint32 delayOff,
                      uint32 tamperDelayOn,
                      bool inverted
                      );

Result_e DSM_RemapBsiStates(uint32 index, 
                           InputState_e state0, 
                           InputState_e state1, 
                           InputState_e state2,
                           InputState_e state3);

Result_e DSM_SetDiParameters(uint32 index,
                       uint32 filtertime,
                       uint32 delayOn, 
                       uint32 delayOff,
                       bool inverted
                       );

Result_e DSM_RemapDiStates(uint32 index,
                          InputState_e state0,
                          InputState_e state1);

Result_e DSM_BindOutputToInput(uint32 outputId, uint32 inputId);

Result_e DSM_UnbindOutputToInput(uint32 outputId, uint32 inputId);

/* Outputs wrapper functions */
Result_e DSM_ConfigLevel(uint32 outputId, uint32 delayToOn, uint32 delayToOff,
                        uint32 inverted);

Result_e DSM_ConfigFrequency(uint32 outputId, uint32 onTime, uint32 offTime,
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted);

Result_e DSM_ConfigPulse(uint32 outputId, uint32 pulseTime, 
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted);

Result_e DSM_InvertOutput(uint32 outputId, uint32 isInverted);

Result_e DSM_ActivateOutput(uint32 outputId, uint32 activate);

Result_e DSM_TurnOutputOff(uint32 outputId);

Result_e DSM_SetBlockedOutputsEx(uint32 outputIDs);