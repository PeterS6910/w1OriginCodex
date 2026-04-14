#include "System\baseTypes.h"
#include "Inputs.h"
#include "System\ADC\adc.h"
#include "System\Timer\SysTick.h"
#include "System\GPIO\gpio.h"
#include "IO\InputLeds.h"
#include "IO\DIPSwitch.h"
#include "IO\Outputs.h"
#include "IO\IOExpansion.h"
#include "System\systemInfo.h"

#include "System\UART\uart.h"

#include "System\Tasks.h"

//#include <stdio.h>

#ifndef BOOTLOADER
/* valid states for  inputs */
volatile InputState_e _inputStates[MAX_INPUT_COUNT];   
#endif
/* valid statuses for special inputs */
volatile SpecialInputState_t _specialInputState;
/* mark with logic 1 input that has changed (both BSI/DI and special */
volatile uint32 _inputChanged;                    

#ifndef BOOTLOADER
#define DIP6_INDEX      6
#define TAMPER_INDEX    7
#define FUSE_INDEX      8
#define EXT_FUSE_INDEX  9
#define EXT_POWER_INDEX 10
#endif
//#define DIP1_INDEX      0
//#define DIP2_INDEX      1

#ifndef BOOTLOADER
InputDescription_t _inputs[MAX_INPUT_COUNT];
uint32 _reportedInputs = 0;
#endif
SpecialInputDescriptor_t _specialInputs[SPECIAL_INPUT_COUNT];

FilterTableRow_t _filterTable[4];

volatile bool _pauseRead = false;

#ifndef BOOTLOADER
uint32 _defaultBSILevels[3];
uint32 _defaultDILevel;

void (*Inputs_OnInputChanged)(uint32 inputId, InputState_e inputState) = null;

void (*Inputs_OnSpecialInputChanged)(uint32 inputId, InputState_e inputState) = null;

void (*Inputs_OnDIPChanged)(uint32 dipValue) = null;
#endif

#ifndef BOOTLOADER
/**
 * Binds function that handles change in BSI/DI input
 * onInputChanged - pointer to the handler function
 */
void Inputs_BindOnInputChanged(void (*onInputChanged)(uint32 inputId, InputState_e inputState))
{
    Inputs_OnInputChanged = onInputChanged;
}

#endif

/** 
 * Binds function that handles change in special input (tamper)
 * @param onSpecialInputChange - pointer to the handler function
 */
#ifndef BOOTLOADER
void Inputs_BindOnSpecialInputChanged(void (*onSpecialInputChanged)(uint32 inputId, InputState_e))
{
    Inputs_OnSpecialInputChanged = onSpecialInputChanged;
}

/**
 * Binds function that handles change in DIP switch
 * @param onDIPChanged - pointer to the handler function 
 */
void Inputs_BindOnDIPChanged(void (*onDIPChanged)(uint32 dipValue))
{
    Inputs_OnDIPChanged = onDIPChanged;
}
#endif

/* Forward declaration */
void ADC_RefreshValidState(uint32 inputID);
void Inputs_ResetInput(uint32 i);
void Inputs_ActivateOutputs(uint32 inputID, uint32 activate);

#ifndef BOOTLOADER

/*
void Inputs_FireOutputLogicChanged(uint32 outputID, uint32 active)
{
    if (Inputs_OnOutputLogicStateChanged != null)
        Inputs_OnOutputLogicStateChanged(outputID, active);
}
*/

void Inputs_FireInputChange(uint32 inputID)
{
    InputState_e validState = _inputs[inputID].inputStateForLevel[_inputs[inputID].validLevel];
    
    if (Inputs_OnInputChanged != null)
    {
        if (_reportedInputs & (1 << inputID))
            Inputs_OnInputChanged(inputID, validState);   
    }
}

Result_e Inputs_SetReportedInputs(uint32 inputIDs)
{
    for (uint32 i = 0; i < 32; i++)
    {   
        if (inputIDs & (1 << i))
        {
            if (i >= SysInfo_GetInputCount())
                return R_IndexOutOfRange;
            
            _reportedInputs |= (1 << i);            
            
            Inputs_FireInputChange(i);
        }
    }
    return R_Ok;
}

Result_e Inputs_SetReportedInputsEx(uint32 inputIDs)
{
    _reportedInputs = 0;
    return Inputs_SetReportedInputs(inputIDs);
}

Result_e Inputs_UnsetReportedInputs(uint32 inputIDs)
{    
    for (uint32 i = 0; i < 32; i++)
    {
        if (inputIDs & (1 << i))
        {
            if (i >= SysInfo_GetInputCount())
                return R_IndexOutOfRange;
            
            _reportedInputs &= ~(1 << i);
        }
    }
    
    return R_Ok;
}

void Inputs_FireAllStatuses()
{
    uint32 i;
    
    for (i = 0; i < SysInfo_GetInputCount(); i++)
    {
        if (_inputs[i].inputType == IT_UNCONFIGURED)
            continue;
        
        Inputs_FireInputChange(i);
    }
    
    if (Inputs_OnSpecialInputChanged != null) 
    {
        Inputs_OnSpecialInputChanged(TAMPER_INDEX, (InputState_e)(_specialInputState.tamper));
        Inputs_OnSpecialInputChanged(FUSE_INDEX, (InputState_e)(_specialInputState.fuse));
        
        ExtensionBoard_e boardType = GetExtBoardType();
        if (boardType == Ext8I_4O || boardType == Ext4I_8O || Ext8O)
        {
            Inputs_OnSpecialInputChanged(EXT_FUSE_INDEX, (InputState_e)(_specialInputState.extFuse));
            Inputs_OnSpecialInputChanged(EXT_POWER_INDEX, (InputState_e)(_specialInputState.extPower));
        }
    }
}
#endif

#ifndef BOOTLOADER
int Inputs_SetDefaultBSI(uint32 inputID)
{
    uint32 i;
    
    if (_inputs[inputID].inputType == IT_BALANCED)       
    {
        for (i = 0; i < 3; i++) 
            _inputs[inputID].toLevel[i] = _defaultBSILevels[i];
        
    }
      
    return 0;
}

int Inputs_SetDefaultDI(uint32 inputID)
{
    if (_inputs[inputID].inputType == IT_DIGITAL)
        _inputs[inputID].toLevel[0] = _defaultDILevel;    

    return 0;
}

int Inputs_SetInputType(uint32 index, InputType_e inputType)
{
    if (index >= SysInfo_GetInputCount())
        return -1;
    
    _pauseRead = true;
    
    _inputs[index].inputType = inputType;
    
    if (inputType == IT_DIGITAL)
        Inputs_SetDefaultDI(index);
    else if (inputType == IT_BALANCED)
        Inputs_SetDefaultBSI(index);
    
    _pauseRead = false;
    
    return 0;
}

/** 
 * Unsets input back to the undefined state (same after reset)
 * @param index - input index
 */
int Inputs_UnsetInput(uint32 index)
{
    if (index >= SysInfo_GetInputCount())
        return -1;
    
    _pauseRead = true;
    
    _inputs[index].inputType = IT_UNCONFIGURED;
    InputLeds_SetState(index, Unknown);
    
    _pauseRead = false;
    
    return 0;
}

/** 
 * Sets voltage levels for all BSI inputs
 * @param toLevel1 - voltage switch value between level 0 and level 1
 * @param toLevel2 - voltage switch value between level 1 and level 2
 * @param toLevel3 - voltage switch value between level 2 and level 3
 * @return 0 if all ok, -1 on error 
 */
int Inputs_SetBsiLevels(uint32 toLevel1, uint32 toLevel2, uint32 toLevel3)
{
    if (toLevel3 >= MAX_ADC_VALUE)
        return -1;
    if (toLevel2 >= toLevel3)
        return -1;
    if (toLevel1 >= toLevel2)
        return -1;
    
    _pauseRead = true;
    
    _defaultBSILevels[0] = toLevel1;
    _defaultBSILevels[1] = toLevel2;
    _defaultBSILevels[2] = toLevel3;
    
    /* Update inputs level values */
    uint32 i;
    for (i = 0; i < SysInfo_GetInputCount(); i++)
    {
        Inputs_SetDefaultBSI(i);
    }
    
    _pauseRead = false;
    
    return 0;
}

/**
 * Sets input as BSI and configure its parameters
 * @param index - index of the input that will be set
 * @param filtertime - basic filtertime for the input (if set lower than 50 ms then 50 ms will used anyway)
 * @param delayOn - delay between voltage change on input and switching input logic to ON state
 * @param delayOff - delay between voltage change on input and switching input logic to OFF state 
 * @param tamperDelayOn - delay between voltage change on input and switching input logic to TAMPER(sabotage) state
 * @return 0 if all ok, -1 on error
 */
int Inputs_SetBsiParameters(uint32 index, 
                      uint32 filtertime,
                      uint32 delayOn, 
                      uint32 delayOff,
                      uint32 tamperDelayOn,
                      bool inverted
                     )
{
    if (index >= SysInfo_GetInputCount())
        return -1;
    
    _pauseRead = true;
    
    /* Deactivate possible binded outputs */
    Inputs_ActivateOutputs(index, 0);
    Inputs_ResetInput(index);
    
    if (filtertime < INPUTS_DEFAULT_FILTERTIME)
        filtertime = INPUTS_DEFAULT_FILTERTIME;
    
    _inputs[index].inputType = IT_BALANCED;
    _inputs[index].filtertime = filtertime;
    _inputs[index].delayOn = delayOn;
    _inputs[index].delayOff = delayOff;
    _inputs[index].tamperDelayOn = tamperDelayOn;
    _inputs[index].forceRefresh = 1;
    
    Inputs_SetDefaultBSI(index);
    
    if (!inverted)
        Inputs_RemapBsiStates(index, Short, Normal, Alarm, Break);
    else
        Inputs_RemapBsiStates(index, Short, Alarm, Normal, Break);
    
    _pauseRead = false;
    
    return 0;
}

/**
 * Remaps states for respective voltage levels
 * @param index - index of the input
 * @param inputStateLevel0 - input state for level 0
 * @param inputStateLevel1 - input state for level 1
 * @param inputStateLevel2 - input state for level 2
 * @param inputStateLevel3 - input state for level 3
 * @return 0 if all ok, -1 on error
 */
int Inputs_RemapBsiStates(uint32 index,
                      InputState_e inputStateLevel0,
                      InputState_e inputStateLevel1,
                      InputState_e inputStateLevel2,
                      InputState_e inputStateLevel3)
{
    if (index >= SysInfo_GetInputCount())
        return -1;
    
    _pauseRead = true;
    
    /* Deactivate possible binded outputs */
    Inputs_ActivateOutputs(index, 0);
    
    _inputs[index].inputStateForLevel[0] = inputStateLevel0;
    _inputs[index].inputStateForLevel[1] = inputStateLevel1;
    _inputs[index].inputStateForLevel[2] = inputStateLevel2;
    _inputs[index].inputStateForLevel[3] = inputStateLevel3;
    _inputs[index].forceRefresh = 1;
    
    _pauseRead = false;
    
    if (_inputs[index].currentLevel != UNREAD_VALUE)
    {
        InputState_e lastState = _inputStates[index];
        InputState_e newState = _inputs[index].inputStateForLevel[_inputs[index].validLevel];
        
        if (lastState != newState)
            ADC_RefreshValidState(index);
    }
    
    return 0;
}

/**
 * Sets input as DI and configure its parameters
 * @param index - index of the input that will be set
 * @param filtertime - basic filtertime for the input (if set lower than 50 ms then 50 ms will used anyway)
 * @param delayOn - delay between voltage change on input and switching input logic to ON state
 * @param delayOff - delay between voltage change on input and switching input logic to OFF state 
 * @return 0 if all ok, -1 on error
 */
int Inputs_SetDiParameters(uint32 index,
                           uint32 filtertime,
                           uint32 delayOn, 
                           uint32 delayOff,
                           bool inverted
                           )
{
    if (index >= SysInfo_GetInputCount())
        return -1;
    
    _pauseRead = true;
    
    /* Deactivate possible binded outputs */
    Inputs_ActivateOutputs(index, 0);
    Inputs_ResetInput(index);   
    
    if (filtertime < INPUTS_DEFAULT_FILTERTIME)
        filtertime = INPUTS_DEFAULT_FILTERTIME;
    
    _inputs[index].inputType = IT_DIGITAL;
    _inputs[index].filtertime = filtertime;
    _inputs[index].delayOn = delayOn;
    _inputs[index].delayOff = delayOff;
    _inputs[index].forceRefresh = 1;
    
    Inputs_SetDefaultDI(index);
    
    if (!inverted)
        Inputs_RemapDiStates(index, Alarm, Normal);
    else
        Inputs_RemapDiStates(index, Normal, Alarm);
    
    _pauseRead = false;
    
    return 0;
}

/**
 * Remaps states for respective voltage levels
 * @param index - index of the input
 * @param inputStateLevel0 - input state for level 0
 * @param inputStateLevel1 - input state for level 1
 * @return 0 if all ok, -1 on error
 */
int Inputs_RemapDiStates(uint32 index,
                       InputState_e inputStateForLevel0,
                       InputState_e inputStateForLevel1)
{
    if (index >= SysInfo_GetInputCount())
        return -1;
    
    _pauseRead = true;
    
    /* Deactivate possible binded outputs */
    Inputs_ActivateOutputs(index, 0);
    
    _inputs[index].inputStateForLevel[0] = inputStateForLevel0;
    _inputs[index].inputStateForLevel[1] = inputStateForLevel1;
    _inputs[index].forceRefresh = 1;
    
    _pauseRead = false;
    
    if (_inputs[index].currentLevel != UNREAD_VALUE)
    {
        InputState_e lastState = _inputStates[index];
        InputState_e newState = _inputs[index].inputStateForLevel[_inputs[index].validLevel];
        
        if (lastState != newState)
            ADC_RefreshValidState(index);
    }
        
    return 0;
}
#endif

/** 
 * Set parameters for tamper
 * @param filtertime - custom tamper filtertime value 
 * @param delayOn - time between change on tamper input and switching to ON state
 * @return 0 if all ok, -1 on error 
 */
#ifndef BOOTLOADER
int Inputs_SetTamperParameters(uint32 filtertime, uint32 delayOn)
{
    if (filtertime < INPUTS_DEFAULT_FILTERTIME)
        filtertime = INPUTS_DEFAULT_FILTERTIME;
    
    _pauseRead = true;
    
    _specialInputs[TAMPER_INDEX].filtertime = filtertime;
    _specialInputs[TAMPER_INDEX].delayOn = delayOn;
    
    _pauseRead = false;
    
    return 0;
}
#endif

#ifndef BOOTLOADER
/**
 * Binds output to input. If input changes its status to ON, this output will 
 * be activated
 * @param outputId - id of output that should be assigned
 * @param inputId - id of input that the output will be assigned to 
 * @return 0 if all ok, -1 on error
 */
int Inputs_BindOutputToInput(uint32 outputId, uint32 inputId)
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    if (inputId >= SysInfo_GetInputCount())
        return -1;
    
    _inputs[inputId].bindedOutputs |= (1 << outputId);
    
    InputState_e validState = _inputs[inputId].inputStateForLevel[_inputs[inputId].validLevel];
    
    //BOOL isOn = (validState == Alarm);
    
    if (validState == Alarm &&
        !(_inputs[inputId].activatedOutputs & (1 << outputId)))
    {
        _inputs[inputId].activatedOutputs |= (1 << outputId);
        Outputs_ActivateOutput(outputId, 1);
    }
    /* deactivate if the output was activated by this input (this should not be happening) */
    else if (validState == Normal &&
             (_inputs[inputId].activatedOutputs & (1 << outputId)))
    {
         _inputs[inputId].activatedOutputs &= ~(1 << outputId);
         Outputs_ActivateOutput(outputId, 0);
    }
    /*
    else
    {
        Outputs_ReportCurrentLogicState(outputId);
    }
    */
    
    return 0;
}

/**
 * Unbinds the output from specified input 
 * @param outputId - id of output that should be deassigned 
 * @param inputId - id of input
 */
int Inputs_UnbindOutputToInput(uint32 outputId, uint32 inputId)
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    if (inputId >= SysInfo_GetInputCount())
        return -1;
    
    _inputs[inputId].bindedOutputs &= ~(1 << outputId);
    
    InputState_e validState = _inputs[inputId].inputStateForLevel[_inputs[inputId].validLevel];
    
    if (validState == Alarm &&
        (_inputs[inputId].activatedOutputs & (1 << outputId)))
    {
        Outputs_ActivateOutput(outputId, 0);
        //Inputs_FireOutputLogicChanged(outputId, 0);
        _inputs[inputId].activatedOutputs &= ~(1 << outputId);
    }
/*
    else
    {
        Inputs_FireOutputLogicChanged(outputId, 1);
    }
*/
    
    return 0;
}

int Inputs_UnbindAll()
{
    int input, output;
    for (input = 0; input < SysInfo_GetInputCount(); input++)
    {
        for (output = 0; output < SysInfo_GetOutputCount(); output++)
        {
            Inputs_UnbindOutputToInput(output, input);
        }
    }
    
    return 0;
}



void Inputs_ResetInput(uint32 i)
{
    //_inputs[i].inputType = IT_UNCONFIGURED;
    //_inputs[i].filtertime = INPUTS_DEFAULT_FILTERTIME;
    //_inputs[i].delayOn = 0;
    //_inputs[i].delayOff = 0;    
    //_inputs[i].tamperDelayOn = 0;
    _inputs[i].currentLevel = UNREAD_VALUE;
    _inputs[i].changeTime = 0;
    _inputs[i].validLevel = UNREAD_VALUE;
    //_inputs[i].bindedOutputs = 0;
    //_inputs[i].activatedOutputs = 0;
    _inputStates[i] = Unknown;
    
    _inputChanged &= ~(1 << i);
}

#endif

void Inputs_Init() 
{
    
#ifndef BOOTLOADER
    SysInfo_SetInputCount(BASE_INPUT_COUNT);
    
//    for(int i=0;i<=2;i++)
    _defaultBSILevels[0] = INPUT_TOLEVEL1;
    _defaultBSILevels[1] = INPUT_TOLEVEL2;
    _defaultBSILevels[2] = INPUT_TOLEVEL3;
    
    _defaultDILevel = INPUT_DIG_TOLEVEL1;
    
    for (int32 i = 0; i < SysInfo_GetInputCount(); i++)
    {
        _inputs[i].inputType = IT_UNCONFIGURED;
        _inputs[i].filtertime = INPUTS_DEFAULT_FILTERTIME;
        _inputs[i].delayOn = 0;
        _inputs[i].delayOff = 0;    
        _inputs[i].tamperDelayOn = 0;
        _inputs[i].currentLevel = UNREAD_VALUE;
        _inputs[i].validLevel = 0;
        _inputs[i].bindedOutputs = 0;
        _inputs[i].activatedOutputs = 0;
        _inputs[i].forceRefresh = 1;
        _inputStates[i] = Unknown;
        
        Inputs_SetDefaultBSI(i);
        Inputs_SetDefaultDI(i);
    }
    
    _inputChanged = 0x00;
    
    /* Initialize the filter time table */
    for (int32 i = 0; i < 4; i++) 
    {
        _filterTable[i].S = FT;
        _filterTable[i].N = FT;
        _filterTable[i].A = FT;
        _filterTable[i].B = FT;
    }
    
    _filterTable[Normal].S = FT_TdON;
    _filterTable[Normal].A = FT_dON;
    _filterTable[Normal].B = FT_TdON;
    _filterTable[Alarm].S = FT_TdON;
    _filterTable[Alarm].N = FT_dOff;
    _filterTable[Alarm].B = FT_TdON;
#endif
    
    
    /* Special input configuration */
    for (int32 i = 0; i < SPECIAL_INPUT_COUNT; i++)
    {
        _specialInputs[i].inputType = IT_SPECIAL;
        
        _specialInputs[i].filtertime = INPUTS_DEFAULT_FILTERTIME;
        _specialInputs[i].delayOn = 0;
        _specialInputs[i].validLevel = 0;
        _specialInputs[i].currentLevel = 0;
        
    }
    /* Longer filtertimes for DIP switch */
    for (uint32 i = 0; i < (DIP_COUNT+1); i++) // count also DIP6
    {
        _specialInputs[i].filtertime = DIP_DEFAULT_FILTERTIME;
    }
    
#ifndef BOOTLOADER
    /* Tamper GPIO configuration */
    GPIO_SetDir(PORT1, 6, GPIO_INPUT);
    /* Fuse configuration */
    GPIO_SetDir(PORT0, 19, GPIO_INPUT);
#endif
    
    /* DIP switch initalization */
    DIP_Init();
    uint32 origAddress = _specialInputState.address;
    // inverted state
    _specialInputState.address = 0x1F; // 5-bit value
    for (uint32 i = 0; i < DIP_COUNT; i++)
    {
        _specialInputs[i].validLevel = DIP_GetFrom(i);
        if (!(_specialInputs[i].validLevel & 0x01))
            _specialInputState.address |= (1 << i);   
        else
            _specialInputState.address &= ~(1 << i);
    }
    
    if (_specialInputState.address != origAddress)
    {
        _inputChanged |= SI_ADDRESS_BIT;

#ifndef BOOTLOADER
        if (Inputs_OnDIPChanged != null)
            Inputs_OnDIPChanged(_specialInputState.address);
#endif
    }
    
#ifndef BOOTLOADER
    InputLeds_Init();    

    ADC_Init(ADC_CLK);
	#ifdef LON
    //Tasks_Register(ADC_Task, 10);
	#else
        #ifndef RS485 // if old RS485 implementation
    	Tasks_Register(ADC_Task, 3);
        #endif
	#endif
    
#endif

#ifdef LON 
    Tasks_Register(Inputs_Task, 15);    
#else
	Tasks_Register(Inputs_Task, 5);    
#endif
}

#ifndef BOOTLOADER
/*****************************************************************************
** Function name:		FindLevelFromValue
**
** Descriptions:		Finds to which level of given input new read ADC value 
**                      belongs
**
** parameters:			index - input index
**                      adcValue - read ADC value
** Returned value:		returns the level to which read ADC value belongs
** 
*****************************************************************************/
uint8 FindLevelFromValue(uint32 index, uint32 adcValue)
{
    uint32 i = 0;
    
    if (index >= SysInfo_GetInputCount())
        return 0;
    
    if (_inputs[index].inputType == IT_UNCONFIGURED)
        return 0;
    
    switch (_inputs[index].inputType) 
    {
        case IT_UNCONFIGURED:
            return 0;
            
        case IT_DIGITAL:
            if (_inputs[index].toLevel[0] > adcValue)
                return 0;
            else 
                return 1;
        
        case IT_BALANCED:        
            for (i = 0; i < 3; i++)
            {
                if (_inputs[index].toLevel[i] > adcValue)
                    return i;
            }
            return 3;
    }
    
    return 0;
}

/*****************************************************************************
** Function name:		GetFilterValue
**
** Descriptions:		Calculates filtertime value based on given type and 
**                      input configuration
**
** parameters:			whatType - type of the filtertime
**                      index - index of the input
** Returned value:		calculated filtertime
** 
*****************************************************************************/
uint32 GetFilterValue(uint32 whatType, uint32 index)
{
    switch (whatType) 
    {
        case FT:
            return _inputs[index].filtertime;
        case FT_TdON:
            return _inputs[index].filtertime + _inputs[index].tamperDelayOn;
        case FT_dON:
            return _inputs[index].filtertime + _inputs[index].delayOn;
        case FT_dOff:
            return _inputs[index].filtertime + _inputs[index].delayOff;
        default:
            return _inputs[index].filtertime;
    }
}

/*****************************************************************************
** Function name:		CalculateFilterTime
**
** Descriptions:		Calculates filtertime for given transition and input
**                      configuration
**
** parameters:			validState - original state (source)
**                      currentState - new state (destination)
**                      index - index of the input
** Returned value:		calculated filtertime
** 
*****************************************************************************/
uint32 CalculateFilterTime(InputState_e validState, InputState_e currentState, 
                            uint32 index)
{
    /* Time switch between the same state is zero, or is it? :) */
    if (validState == currentState)
        return 0;
    
    switch (currentState)
    {
        case Short:
            return GetFilterValue(_filterTable[validState].S, index);
        case Normal:
            return GetFilterValue(_filterTable[validState].N, index);
        case Alarm:
            return GetFilterValue(_filterTable[validState].A, index);
        case Break:
            return GetFilterValue(_filterTable[validState].B, index);
    }
    
    return 0;
}
#endif

#ifndef BOOTLOADER
void Inputs_ActivateOutputs(uint32 inputID, uint32 activate)
{
    uint32 j;
    
    for (j = 0; j < SysInfo_GetOutputCount(); j++)
    {
        if (_inputs[inputID].bindedOutputs & (1 << j)) 
        {
            /* activated -> deactivate output */           
            if (!activate && (_inputs[inputID].activatedOutputs & (1 << j))) 
            {
                _inputs[inputID].activatedOutputs &= ~(1 << j);
                Outputs_ActivateOutput(j, 0);
                //Inputs_FireOutputLogicChanged(j, 0);
            }
            /* not activated yet -> activate output */
            if (activate && !(_inputs[inputID].activatedOutputs & (1 << j)))
            {
                _inputs[inputID].activatedOutputs |= (1 << j);
                Outputs_ActivateOutput(j, 1);
                //Inputs_FireOutputLogicChanged(j, 1);
            }
        }
    }
}

void ADC_RefreshValidState(uint32 inputID)
{
    _inputs[inputID].validLevel = _inputs[inputID].currentLevel;
    
    InputState_e validState = _inputs[inputID].inputStateForLevel[_inputs[inputID].validLevel];
    InputLeds_SetState(inputID, validState);
    
    _inputStates[inputID] = validState;
    _inputChanged |= (1 << inputID);
    
    
    /* activate outputs if neccessary */
    if (validState == Normal)
        Inputs_ActivateOutputs(inputID, 0);
    if (validState == Alarm)
        Inputs_ActivateOutputs(inputID, 1);
    
    Inputs_FireInputChange(inputID);    
}
#endif

/*****************************************************************************
** Function name:		Inputs_Task
**
** Descriptions:		Main input task routine responsible for status change
**
** parameters:			none
** Returned value:		void
** 
*****************************************************************************/
void Inputs_Task()
{
    uint32 i;
#ifndef BOOTLOADER
    uint32 newLevel;
    //uint32 j;
#endif
    
    if (_pauseRead)
        return;

#ifndef BOOTLOADER    
    if (!ADC_IsValid())
        return;
    
    /* update status for regular inputs */
    for (i = 0; i < SysInfo_GetInputCount(); i++)
    {
        if (_inputs[i].inputType == IT_UNCONFIGURED)
            continue;
        
        newLevel = FindLevelFromValue(i, _ADCValue[i]);
        if (_inputs[i].currentLevel != newLevel) 
        {
            /* mark the change time */
            _inputs[i].currentLevel = newLevel;
            _inputs[i].changeTime = SysTick_GetTickCount();
        }    
        
        if (_inputs[i].currentLevel != _inputs[i].validLevel)
        {
            InputState_e validState;
            
            if (_inputs[i].validLevel == UNREAD_VALUE)
                validState = Unknown;
            else
                validState = _inputs[i].inputStateForLevel[_inputs[i].validLevel];
                
            InputState_e currentState = _inputs[i].inputStateForLevel[_inputs[i].currentLevel];
            
            uint32 timeToPass = CalculateFilterTime(validState, currentState, i);
            uint32 elapsedTime = SysTick_GetElapsedTime(_inputs[i].changeTime);
            
            if (elapsedTime >= timeToPass || _inputs[i].forceRefresh)
            {
                ADC_RefreshValidState(i);
                _inputs[i].forceRefresh = 0;
            }
        }
    }
#endif
    
    bool addressChanged = false;
    
    /* Update status of special inputs (Tamper + DIP switch) */
    for (i = 0; i < SPECIAL_INPUT_COUNT; i++)
    {
        int32 inputValue;

#ifndef BOOTLOADER
        ExtensionBoard_e extBoard = GetExtBoardType();
#endif

        switch (i)
        {
#ifndef BOOTLOADER
            case DIP6_INDEX:
                inputValue = GPIO_GetValue(PORT0, 27); // DIP6
                break;
            case TAMPER_INDEX:
                inputValue = GPIO_GetValue(PORT1, 6);
                break;
            case FUSE_INDEX:
                inputValue = GPIO_GetValue(PORT0, 19);
                break;
            case EXT_FUSE_INDEX:
                if (extBoard == Ext8I_4O || extBoard == Ext4I_8O || extBoard == Ext8O)
                    inputValue = IOExp_ReadFuse() == Alarm;
                else
                    continue;
                break;
            case EXT_POWER_INDEX:
                if (extBoard == Ext8I_4O || extBoard == Ext4I_8O || extBoard == Ext8O)
                    inputValue = IOExp_ReadPower() == Alarm;
                else
                    continue;
                break;
#endif
            default:
                inputValue = DIP_GetFrom(i);
                break;
        }
        
        if (inputValue < 0)
            continue;
        
        if (_specialInputs[i].currentLevel != inputValue) 
        {
            _specialInputs[i].currentLevel = inputValue;
            /* if this is from DIP use the same change time for all of them */
            if (i < DIP_COUNT)  
            {
                _specialInputs[0].changeTime = SysTick_GetTickCount();
            }
            else
            {
                /* mark the change time */
                _specialInputs[i].changeTime = SysTick_GetTickCount();
            }
        }    
        
        if (_specialInputs[i].currentLevel != _specialInputs[i].validLevel)
        {
            uint32 timeToPass = 0;
            
            if (i < DIP_COUNT)
                timeToPass = _specialInputs[0].filtertime;
            else
                timeToPass = _specialInputs[i].filtertime;
                 
#ifndef BOOTLOADER
            if (i == TAMPER_INDEX && _specialInputs[i].validLevel == 0) /* if normal state then add delayToOn */
                timeToPass += _specialInputs[i].delayOn;
#endif
            
            uint32 elapsedTime = 0;
            if (i < DIP_COUNT)
                elapsedTime = SysTick_GetElapsedTime(_specialInputs[0].changeTime);
            else
                elapsedTime = SysTick_GetElapsedTime(_specialInputs[i].changeTime);
            
            if (elapsedTime >= timeToPass)
            {
                _specialInputs[i].validLevel = _specialInputs[i].currentLevel;
                
#ifndef BOOTLOADER
                switch (i)
                {
                    case TAMPER_INDEX:
                        _specialInputState.tamper = _specialInputs[i].validLevel;
                        _inputChanged |= SI_TAMPER_BIT;
                        if (Inputs_OnSpecialInputChanged != null)
                            Inputs_OnSpecialInputChanged(TAMPER_INDEX, (InputState_e)(_specialInputState.tamper));
                        break;
                    case FUSE_INDEX:
                        _specialInputState.fuse = _specialInputs[i].validLevel;
                        _inputChanged |= SI_FUSE_BIT;
                        if (Inputs_OnSpecialInputChanged != null)
                            Inputs_OnSpecialInputChanged(FUSE_INDEX, (InputState_e)(_specialInputState.fuse));
                        break;
                    case EXT_FUSE_INDEX:
                        _specialInputState.extFuse = _specialInputs[i].validLevel;
                        _inputChanged |= SI_EXT_FUSE_BIT;
                        if (Inputs_OnSpecialInputChanged != null)
                            Inputs_OnSpecialInputChanged(EXT_FUSE_INDEX, (InputState_e)(_specialInputState.extFuse));
                        break;
                    case EXT_POWER_INDEX:
                        _specialInputState.extPower = _specialInputs[i].validLevel;
                        _inputChanged |= SI_EXT_POWER_BIT;
                        if (Inputs_OnSpecialInputChanged != null)
                            Inputs_OnSpecialInputChanged(EXT_POWER_INDEX, (InputState_e)(_specialInputState.extPower));
                        break;
                    case DIP6_INDEX:
                        _specialInputState.dip6 = _specialInputs[i].validLevel;
                        _inputChanged |= SI_DIP6_BIT;
                            
                        break;                
                    default:
#endif
                        if (i<DIP_COUNT) 
                        {
                            // DIP switch address
                            if (!(_specialInputs[i].validLevel & 0x01))
                                _specialInputState.address |= (1 << i);   
                            else
                                _specialInputState.address &= ~(1 << i);
                            
                            addressChanged = true;                           
                        }
#ifndef BOOTLOADER
                        break;
                }                
#endif
            }
        }
    }
    
    /* Only signal the address change after all bits have been read */
    if (addressChanged) {
        _inputChanged |= SI_ADDRESS_BIT;

#ifndef BOOTLOADER
        if (Inputs_OnDIPChanged != null)
            Inputs_OnDIPChanged(_specialInputState.address);
#endif
    }
}

