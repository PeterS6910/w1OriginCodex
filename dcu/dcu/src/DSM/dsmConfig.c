#ifndef BOOTLOADER

#include "dsmConfig.h"
#include "IO\Outputs.h"
#include "IO\Inputs.h"
#include "System\baseTypes.h"
#include "System\constants.h"
#include "System\systemInfo.h"
#include "System\dmHandler.h"

volatile DSMConfiguration_t _dsmConfig;
void (*DSM_OnStateChanged)(DSMSignals_e, DSMSignalInfo_e, DSMAccessGrantedSource_e pushButtonId) = null;

void DSM_BindOnStateChanged(void (*onStateChanged)(DSMSignals_e state, DSMSignalInfo_e info, DSMAccessGrantedSource_e pushButtonId))
{
    DSM_OnStateChanged = onStateChanged;
}



void (*DSM_OnSpecialOutputAdded)(DSMSpecialDOType_e outputType) = null;

void (*DSM_OnSpecialOutputRemoved)(DSMSpecialDOType_e outputType) = null;

void DSM_BindOnSpecialOutputAdded(void (*onSpecialOutputAdded)(DSMSpecialDOType_e outputType))
{
    DSM_OnSpecialOutputAdded = onSpecialOutputAdded;
}

void DSM_BindOnSpecialOutputRemoved(void (*onSpecialOutputRemoved)(DSMSpecialDOType_e outputType))
{
    DSM_OnSpecialOutputRemoved = onSpecialOutputRemoved;
}



void (*DSM_OnEnabledAlarmChanged)(uint32 doorAjarAlarm, uint32 intrusionAlarm, uint32 sabotageAlarm) = null;

void DSM_BindOnEnabledAlarmChanged(void (*onEnabledAlarmChanged)(uint32 doorAjarAlarm, uint32 intrusionAlarm, uint32 sabotageAlarm))
{
    DSM_OnEnabledAlarmChanged = onEnabledAlarmChanged;
}




void DSM_Init()
{
    uint32 i;
    /* actuators configuration */
    for (i = 0; i < DSM_ACTUATOR_COUNT; i++)
        _dsmConfig.actuator[i] = UNCONFIGURED_IO;
    /* sensors configuration */
    for (i = 0; i < DSM_SENSOR_COUNT; i++)
        _dsmConfig.sensor[i] = UNCONFIGURED_IO;
    /* special output configuration */
    for (i = 0; i < DSM_SPECIAL_OUTPUT_COUNT; i++)
        _dsmConfig.specialOutput[i] = UNCONFIGURED_IO;
    /* access granted signal sources */
    for (i = 0; i < DSM_AG_SRC_COUNT; i++)
        _dsmConfig.agSources[i] = UNCONFIGURED_IO;
    /* default timming values */
    DSM_SetTimming(DSM_UNLOCK_TIME, DSM_OPEN_TIME, DSM_PREALARM_TIME, DSM_ARRAY_DELAY, DSM_INTRUSION_DELAY);
    
    _dsmConfig.invokeIntrusionAlarm = 1;
    _dsmConfig.locked = 0;
    
    DSM_EnableAlarms(1, 1, 1);
    
//    _dsmConfig.assignedCRs = 0; // no assigned CR at start time
//    _dsmConfig.activeCRs = 0;   // no active CR at start time
    for (i = 0; i < MAX_CARD_READER_ADDR; i++)
    {
        _dsmConfig.cardReaders[i].flags = 0;   // not assigned, not suppressed
        _dsmConfig.cardReaders[i].implicitCode = 0;
        _dsmConfig.cardReaders[i].optionalParameter = -1;
        
        _dsmConfig.cardReaders[i].msgCode = 0;
        _dsmConfig.cardReaders[i].optDataLength = 0;
       
        
        // deallocation should be here, due usage by DSM_Stop
	// which can be called runtime
        //if (_dsmConfig.cardReaders[i].optData != null) 
        {
            Delete(_dsmConfig.cardReaders[i].optData);
            _dsmConfig.cardReaders[i].optData = null;
        }
    }
}

Result_e DSM_UnsetAll()
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    uint32 i;
    for (i = 0; i < DSM_ACTUATOR_COUNT; i++)
    {
        if (_dsmConfig.actuator[i] != UNCONFIGURED_IO)
            DSM_UnsetActuator((DSMActuators_e)i);
    }
    
    for (i = 0; i < DSM_SENSOR_COUNT; i++)
    {
        if (_dsmConfig.sensor[i] != UNCONFIGURED_IO)
            DSM_UnsetSensor((DSMSensors_e)i);
    }
    
    for (i = 0; i < DSM_SPECIAL_OUTPUT_COUNT; i++)
    {
        if (_dsmConfig.specialOutput[i] != UNCONFIGURED_IO)
            DSM_UnsetSpecialOutput((DSMSpecialDOType_e)i);
    }
    
    for (i = 0; i < DSM_AG_SRC_COUNT; i++)
    {
        if (_dsmConfig.agSources[i] != UNCONFIGURED_IO)
            DSM_UnsetPushButton(i);
    }
    
    return R_Ok;
}

Result_e DSM_SetPushButton(uint32 sourceId, uint32 inputId, InputType_e inputType, uint32 inverted,
                           uint32 delayToOn, uint32 delayToOff)
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    if (inputId >= SysInfo_GetInputCount())
        return R_IndexOutOfRange;
    
    if (_dsmConfig.usedInputs & (1 << inputId))
    {
        if (_dsmConfig.agSources[sourceId] != inputId)
            return R_IndexAlreadyUsed;
    }
    
    _dsmConfig.usedInputs |= (1 << inputId);
    
    _dsmConfig.agSources[sourceId] = inputId;
    
    //InputState_e activated = Alarm, idle = Normal;
    /*if (inverted) 
    {
        activated = Normal;
        idle = Alarm;
    }*/
    
    //Inputs_SetInputType(inputId, inputType);
    
    if (inputType == IT_DIGITAL)
    {
        Inputs_SetDiParameters(inputId, 0, delayToOn, delayToOff, inverted);
        //Inputs_RemapDiStates(inputId, activated, idle);
    }
    else if (inputType == IT_BALANCED)
    {
        Inputs_SetBsiParameters(inputId, 0, delayToOn, delayToOff, 0, inverted); 
        //Inputs_RemapBsiStates(inputId, Short, idle, activated, Break);
    }
    
    return R_Ok;
}

#pragma diag_suppress=Pa082
Result_e DSM_UnsetPushButton(uint32 sourceId)
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    if (_dsmConfig.agSources[sourceId] == UNCONFIGURED_IO)
        return R_Ok;
    
    _dsmConfig.usedInputs &= ~(1 << _dsmConfig.agSources[sourceId]);
    _dsmConfig.agSources[sourceId] = UNCONFIGURED_IO;
    
    return R_Ok;
}
#pragma diag_default=Pa082

Result_e DSM_InvokeIntrusionAlarm(uint32 invoke)
{
    _dsmConfig.invokeIntrusionAlarm = invoke;
    
    return R_Ok;
}

Result_e DSM_SetSensor(DSMSensors_e sensorType, uint32 inputId, InputType_e inputType, uint32 inverted,
                       uint32 delayToOn, uint32 delayToOff)
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    if (inputId >= SysInfo_GetInputCount())
        return R_IndexOutOfRange;
    
    if (_dsmConfig.usedInputs & (1 << inputId))
    {
        if (_dsmConfig.sensor[sensorType] != inputId)
            return R_IndexAlreadyUsed;
    }
    _dsmConfig.usedInputs |= (1 << inputId);
    _dsmConfig.sensor[sensorType] = inputId;
    
    //InputState_e activated = Alarm, idle = Normal;
    //if (inverted) 
    /*{
        activated = Normal;
        idle = Alarm;
    }*/
    
    //Inputs_SetInputType(inputId, inputType);
    
    if (inputType == IT_DIGITAL)
    {
        Inputs_SetDiParameters(inputId, 0, delayToOn, delayToOff, inverted);
        //Inputs_RemapDiStates(inputId, activated, idle);
    }
    else if (inputType == IT_BALANCED)
    {
        Inputs_SetBsiParameters(inputId, 0, delayToOn, delayToOff, 0, inverted); 
        //Inputs_RemapBsiStates(inputId, Short, idle, activated, Break);
    }
    
    return R_Ok;
}

#pragma diag_suppress=Pa082
Result_e DSM_UnsetSensor(DSMSensors_e sensorType)
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    if (_dsmConfig.sensor[sensorType] == UNCONFIGURED_IO)
        return R_Ok;    
    
    _dsmConfig.usedInputs &= ~(1 << _dsmConfig.sensor[sensorType]);
    _dsmConfig.sensor[sensorType] = UNCONFIGURED_IO;
    
    return R_Ok;
}
#pragma diag_default=Pa082

#pragma diag_suppress=Pa082
Result_e DSM_SetElectricStrike(DSMActuators_e actuatorId, 
                                    uint32 outputId,
                                    DSMStrikeType_e strikeType,
                                    uint32 pulseTime,
                                    uint32 inverted,
                                    uint32 delayToOn,
                                    uint32 delayToOff)
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    if (outputId >= SysInfo_GetOutputCount())
        return R_IndexOutOfRange;
    
    if (actuatorId != ElectricStrike && actuatorId != ElectricStrikeOpposite &&
        actuatorId != ExtraElectricStrike && actuatorId != ExtraElectricStrikeOpposite)
        return R_InvalidES;
    
    if (strikeType == StrikeTypePulse && pulseTime == 0)
        return R_InvalidParameter;
    
    if (_dsmConfig.usedOutputs & (1 << outputId))
    {
        if (_dsmConfig.actuator[actuatorId] != outputId)
            return R_IndexAlreadyUsed;
    }
    
    if (_dsmConfig.usedSpecialOutputs[outputId] != 0)
        return R_IndexAlreadyUsed;
    
    _dsmConfig.usedOutputs |= (1 << outputId);
    
    _dsmConfig.actuator[actuatorId] = outputId;
    
    
    /* If bypass is configured make sure that delayToOn (beforeUnlock) is at least 50 ms */
    if (_dsmConfig.actuator[AlarmBypass] == UNCONFIGURED_IO && 
        delayToOn < DSM_MIN_BEFORE_UNLOCK_TIME)
    {
        delayToOn = DSM_MIN_BEFORE_UNLOCK_TIME;
    }
        
    _dsmConfig.delay[DelayBeforeUnlock] = delayToOn;
    _dsmConfig.delay[DelayBeforeLock] = delayToOff;
    
    switch (strikeType)
    {
        case StrikeTypeLevel:
            Outputs_ConfigLevel(outputId, 
                                _dsmConfig.delay[DelayBeforeUnlock], 
                                _dsmConfig.delay[DelayBeforeLock], 
                                inverted);
            break;
        case StrikeTypePulse:
            Outputs_ConfigPulse(outputId, 
                                (pulseTime != 0) ? pulseTime : DSM_STRIKE_PULSE_TIME, 
                                _dsmConfig.delay[DelayBeforeUnlock], 
                                _dsmConfig.delay[DelayBeforeLock],
                                1, inverted);
            break;
    }
    
    return R_Ok;
}


Result_e DSM_SetBypassAlarm(uint32 outputId)
{
    if (outputId >= SysInfo_GetOutputCount())
        return R_IndexOutOfRange;
    
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    if (_dsmConfig.usedOutputs & (1 << outputId))
    {
        if (_dsmConfig.actuator[AlarmBypass] != outputId)
            return R_IndexAlreadyUsed;
    }
    if (_dsmConfig.usedSpecialOutputs[outputId] != 0)
        return R_IndexAlreadyUsed;
 
    _dsmConfig.usedOutputs |= (1 << outputId);
    _dsmConfig.actuator[AlarmBypass] = outputId;
    
    /* check if all electric strikes has at least 50 ms before unlock time 
      Electric strikes have indexes 0-3, 4 is bypass itself */
    for (int i = 0; i < DSM_ACTUATOR_COUNT - 1; i++)
    {
        if (_dsmConfig.actuator[i] != UNCONFIGURED_IO)
            Outputs_EnsureMinDelayToOn(i, DSM_MIN_BEFORE_UNLOCK_TIME);        
    }
        
    return R_Ok;
}

Result_e DSM_UnsetActuator(DSMActuators_e actuatorId)
{
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
    _dsmConfig.usedOutputs &= ~(1 << _dsmConfig.actuator[actuatorId]);
    _dsmConfig.actuator[actuatorId] = UNCONFIGURED_IO;
    
    return R_Ok;
}



Result_e DSM_EnableAlarms(uint32 doorAjarAlarm, uint32 intrusionAlarm, uint32 sabotageAlarm)
{
    if (DSM_OnEnabledAlarmChanged != null)
        DSM_OnEnabledAlarmChanged(doorAjarAlarm, intrusionAlarm, sabotageAlarm);
    
    _dsmConfig.doorAjarAlarmEnabled = doorAjarAlarm;
    _dsmConfig.intrusionAlarmEnabled = intrusionAlarm;
    _dsmConfig.sabotageAlarmEnabled = sabotageAlarm;
    
    return R_Ok;
}

Result_e DSM_SetTimming(uint32 unlockTime, uint32 openTime, uint32 preAlarmTime, uint32 sireneAjarDelay, 
                        uint32 beforeIntrusion)
{
    _dsmConfig.timming[MaxUnlockTime] = unlockTime;
    _dsmConfig.timming[MaxOpenTime] = openTime;
    _dsmConfig.timming[DoorPreAjarTime] = preAlarmTime;
    _dsmConfig.timming[SireneAjarDelay] = sireneAjarDelay;
    
    _dsmConfig.delay[DelayBeforeIntrusion] = beforeIntrusion;
    
    return R_Ok;
}



Result_e DSM_SetSpecialOutput(DSMSpecialDOType_e outputType, uint32 outputId)
{  
    if (outputId >= SysInfo_GetOutputCount())
        return R_IndexOutOfRange;
    
    if (outputType >= DSM_SPECIAL_OUTPUT_COUNT)
        return R_UnknownActuator;
    
    /* Check that output is not used for some major functionality */
    if (_dsmConfig.usedOutputs & (1 << outputId))
    {
        //if (_dsmConfig.specialOutput[outputType] != outputId)
        return R_IndexAlreadyUsed;
    }
    
    /* check if not assigned already */
    if (_dsmConfig.specialOutput[outputType] == outputId)
        return R_Ok;
    
    /* check if this output type was not used, and if it was remove it */
    if (_dsmConfig.specialOutput[outputType] != UNCONFIGURED_IO)
        DSM_UnsetSpecialOutput(outputType);
    
    _dsmConfig.usedSpecialOutputs[outputId]++;
    _dsmConfig.specialOutput[outputType] = outputId;
    
    // check if needs to be activated
    if (DSM_OnSpecialOutputAdded != null)
        DSM_OnSpecialOutputAdded(outputType);
    
    return R_Ok;
}

Result_e DSM_UnsetSpecialOutput(DSMSpecialDOType_e outputType)
{
    if (outputType >= DSM_SPECIAL_OUTPUT_COUNT)
        return R_UnknownActuator;
        
    if (_dsmConfig.usedSpecialOutputs[_dsmConfig.specialOutput[outputType]] > 0)
        _dsmConfig.usedSpecialOutputs[_dsmConfig.specialOutput[outputType]]--;
    
    /* Check if needs to be deactivated */
    if (DSM_OnSpecialOutputRemoved != null)
        DSM_OnSpecialOutputRemoved(outputType);
    
    _dsmConfig.specialOutput[outputType] = UNCONFIGURED_IO;
    
    return R_Ok;    
}

#pragma diag_default=Pa082      



/* Inputs wrapper functions */
Result_e DSM_UnsetInput(uint32 index)
{
    if (_dsmConfig.usedInputs & (1 << index))
        return R_IndexAlreadyUsed;
    
    if (Inputs_UnsetInput(index) == 0)
        return R_Ok;
    else
        return R_Error;
}

Result_e DSM_SetBsiLevels(uint32 toLevel1, uint32 toLevel2, uint32 toLevel3)
{
    if (Inputs_SetBsiLevels(toLevel1, toLevel2, toLevel3) == 0)
        return R_Ok;
    else
        return R_Error;
}

Result_e DSM_SetBsiParameters(uint32 index, 
                      uint32 filtertime,
                      uint32 delayOn, 
                      uint32 delayOff,
                      uint32 tamperDelayOn,
                      bool inverted
                      )
{
    if (_dsmConfig.usedInputs & (1 << index))
        return R_IndexAlreadyUsed;
    
    if (Inputs_SetBsiParameters(index, filtertime, delayOn, delayOff, tamperDelayOn,inverted) == 0)
        return R_Ok;
    else
        return R_Error;
}

Result_e DSM_RemapBsiStates(uint32 index, 
                           InputState_e state0, 
                           InputState_e state1, 
                           InputState_e state2,
                           InputState_e state3)
{
    if (Inputs_RemapBsiStates(index, state0, state1, state2, state3) == 0)
        return R_Ok;
    else
        return R_Error;
}

Result_e DSM_SetDiParameters(uint32 index,
                       uint32 filtertime,
                       uint32 delayOn, 
                       uint32 delayOff,
                       bool inverted
                       )
{
    if (_dsmConfig.usedInputs & (1 << index))
        return R_IndexAlreadyUsed;
    
    if (Inputs_SetDiParameters(index, filtertime, delayOn, delayOff, inverted) == 0)
        return R_Ok;
    else
        return R_Error;
}

Result_e DSM_RemapDiStates(uint32 index,
                          InputState_e state0,
                          InputState_e state1)
{
    if (Inputs_RemapDiStates(index, state0, state1) == 0)
        return R_Ok;
    else
        return R_Error;
}

Result_e DSM_BindOutputToInput(uint32 outputId, uint32 inputId)
{
//    if (_dsmConfig.usedOutputs & (1 << outputId))
//        return IndexAlreadyUsed;
//    if (_dsmConfig.usedInputs & (1 << inputId))
//        return IndexAlreadyUsed;
    
    if (Inputs_BindOutputToInput(outputId, inputId) == 0)
        return R_Ok;
    else 
        return R_Error;
}

Result_e DSM_UnbindOutputToInput(uint32 outputId, uint32 inputId)
{
//    if (_dsmConfig.usedOutputs & (1 << outputId))
//        return IndexAlreadyUsed;
//    if (_dsmConfig.usedInputs & (1 << inputId))
//        return IndexAlreadyUsed;
    
    if (Inputs_UnbindOutputToInput(outputId, inputId) == 0)
        return R_Ok;
    else 
        return R_Error;
}

/* Outputs wrapper functions */
Result_e DSM_ConfigLevel(uint32 outputId, uint32 delayToOn, uint32 delayToOff,
                        uint32 inverted)
{
    if (_dsmConfig.usedOutputs & (1 << outputId))
        return R_IndexAlreadyUsed;

    if (Outputs_ConfigLevel(outputId, delayToOn, delayToOff, inverted) == 0)
        return R_Ok;
    else 
        return R_IndexOutOfRange;
}

Result_e DSM_ConfigFrequency(uint32 outputId, uint32 onTime, uint32 offTime,
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted)
{
    if (_dsmConfig.usedOutputs & (1 << outputId))
        return R_IndexAlreadyUsed;
    
    if (Outputs_ConfigFrequency(outputId, onTime, offTime, delayToOn, delayToOff, forcedOff, inverted) == 0)
        return R_Ok;
    else 
        return R_Error;
}

Result_e DSM_ConfigPulse(uint32 outputId, uint32 pulseTime, 
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted)
{
    if (_dsmConfig.usedOutputs & (1 << outputId))
        return R_IndexAlreadyUsed;
    
    if (Outputs_ConfigPulse(outputId, pulseTime, delayToOn, delayToOff, forcedOff, inverted) == 0)
        return R_Ok;
    else 
        return R_Error;
}

Result_e DSM_InvertOutput(uint32 outputId, uint32 isInverted)
{
    if (_dsmConfig.usedOutputs & (1 << outputId))
        return R_IndexAlreadyUsed;
    
    if (Outputs_InvertOutput(outputId, isInverted) == 0)
        return R_Ok;
    else 
        return R_Error;
}

Result_e DSM_ActivateOutput(uint32 outputId, uint32 activate)
{
    if (_dsmConfig.usedOutputs & (1 << outputId))
        return R_IndexAlreadyUsed;
    
    if (Outputs_ActivateOutput(outputId, activate) == 0)
        return R_Ok;
    else 
        return R_Error;
}

Result_e DSM_TurnOutputOff(uint32 outputId)
{
    if (_dsmConfig.usedOutputs & (1 << outputId))
        return R_IndexAlreadyUsed;
    
    return Outputs_TurnOff(outputId);
}

Result_e DSM_SetBlockedOutputsEx(uint32 outputIDs)
{
    Result_e returnValue = R_Ok;
    
    for (uint32 i = 0; i < 32; i++)
    {
        if (outputIDs & (1 << i))
        {
            if (i >= SysInfo_GetOutputCount())
                return R_IndexOutOfRange;
            
            if (_dsmConfig.usedOutputs & (1 << i))
            {
                // mark that error result value & disable that output from blocking
                returnValue = R_IndexAlreadyUsed;
                outputIDs &= ~(1 << i);
            }            
        }
    }
    
    Outputs_SetBlockedOutputsEx(outputIDs);
    
    return returnValue;
}

#endif // BOOTLOADER