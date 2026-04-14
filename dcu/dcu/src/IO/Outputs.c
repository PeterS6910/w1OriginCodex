#include "Outputs.h"
#include "System\GPIO\gpio.h"
#include "System\Timer\sysTick.h"
#include "System\Tasks.h"
#include "System\baseTypes.h"
#include "System\systemInfo.h"
#include "IO\IOExpansion.h"

#define LOGIC_OFF          0
#define LOGIC_DELAY_ON     1
#define LOGIC_ON           2
#define LOGIC_DELAY_OFF    3

#define FUNC_OFF            0
#define FUNC_PULSE_LENGTH   1
//#define FUNC_ON             2
#define FUNC_PULSE_DELAY    3

#define OUTPUTS_OFFSET      14

OutputDescription_t _outputsDescriptor[MAX_OUTPUT_COUNT];

volatile uint32 _reportedOutputs = 0;
volatile uint32 _reportedOutputsLogic = 0;
volatile uint32 _blockedOutputs = 0;

void (*Outputs_OnOutputChanged)(uint32 outputId, uint32 isOn);

void (*Outputs_OnOutputLogicStateChanged)(uint32 outputID, uint32 active) = null;

void Outputs_BindOnOutputChanged(void (*onOutputChanged)(uint32 outputId, uint32 isOn))
{
    Outputs_OnOutputChanged = onOutputChanged;
}

/**
 * Bind funtion that handles change in output logic status
 * @param onOutputLogicStateChanged - pointer to the handler function 
 */
void Outputs_BindOnOutputLogicStateChanged(void (*onOutputLogicStateChanged)(uint32 outputID, uint32 active))
{
    Outputs_OnOutputLogicStateChanged = onOutputLogicStateChanged;
}

void Outputs_FireOutputChanged(uint32 outputID, bool isOn)
{
    if (Outputs_OnOutputChanged != null)
    {
        if (_reportedOutputs & (1 << outputID))
            Outputs_OnOutputChanged(outputID, isOn);
    }
}

void Outputs_FireOutputLogicChanged(uint32 outputID, bool isOn)
{
    if (Outputs_OnOutputLogicStateChanged != null)
    {
        if (_reportedOutputsLogic & (1 << outputID))
            Outputs_OnOutputLogicStateChanged(outputID, isOn);
    }
}

void Outputs_ReportCurrentLogicState(uint32 outputID)
{
    Outputs_FireOutputLogicChanged(outputID, _outputsDescriptor[outputID].activated);
}

Result_e Outputs_SetReportedOutputs(uint32 outputIDs)
{
    for (uint32 i = 0; i < 32; i++)
    {   
        if (outputIDs & (1 << i))
        {
            if (i >= SysInfo_GetOutputCount())
                return R_IndexOutOfRange;
            
            _reportedOutputs |= (1 << i);            
            
            uint32 isOn = _outputsDescriptor[i].isOn ^ _outputsDescriptor[i].inverted;
            Outputs_FireOutputChanged(i, isOn);
        }
    }
    
    return R_Ok;
}

Result_e Outputs_SetReportedOutputsEx(uint32 outputIDs)
{
    _reportedOutputs = 0;
    return Outputs_SetReportedOutputs(outputIDs);
}

Result_e Outputs_UnsetReportedOutputs(uint32 outputIDs)
{
    for (uint32 i = 0; i < 32; i++)
    {
        if (outputIDs & (1 << i))
        {
            if (i >= SysInfo_GetOutputCount())
                return R_IndexOutOfRange;
            
            _reportedOutputs &= ~(1 << i);
        }
    }
    
    return R_Ok;
}

Result_e Outputs_SetReportedOutputsLogic(uint32 outputIDs)
{
    for (uint32 i = 0; i < 32; i++)
    {   
        if (outputIDs & (1 << i))
        {
            if (i >= SysInfo_GetOutputCount())
                return R_IndexOutOfRange;
            
            _reportedOutputsLogic |= (1 << i);            
            
            Outputs_FireOutputLogicChanged(i, _outputsDescriptor[i].activated);
        }
    }
        
    return R_Ok;
}

Result_e Outputs_SetReportedOutputsLogicEx(uint32 outputIDs)
{
    _reportedOutputsLogic = 0;
    return Outputs_SetReportedOutputsLogic(outputIDs);
}

Result_e Outputs_UnsetReportedOutputsLogic(uint32 outputIDs)
{
    for (uint32 i = 0; i < 32; i++)
    {
        if (outputIDs & (1 << i))
        {
            if (i >= SysInfo_GetOutputCount())
                return R_IndexOutOfRange;
            
            _reportedOutputsLogic &= ~(1 << i);
        }
    }
    
    return R_Ok;
}

/*****************************************************************************
** Funtion name:    Outputs_SetOutput
** 
** Description:     Switch on/off selected output
** Parameters:      uint32 iOutputId - id of the requested output (0 - 3)
**                  uint32 isOn - new status of the output
*****************************************************************************/
int Outputs_SetOutput(uint32 iOutputId) 
{
    if (iOutputId >= SysInfo_GetOutputCount())
        return -1;

    uint32 isOn = _outputsDescriptor[iOutputId].isOn ^ _outputsDescriptor[iOutputId].inverted;
    
    if (iOutputId < BASE_OUTPUT_COUNT)
        GPIO_SetValue(PORT0, iOutputId + OUTPUTS_OFFSET, !(isOn & 0x01));
    else
        IOExp_SetOutput(iOutputId - BASE_OUTPUT_COUNT, isOn);
    
    Outputs_FireOutputChanged(iOutputId, isOn);
    
    return 0;
}

Result_e Outputs_SetBlockedOutputsEx(uint32 outputIDs)
{
    _blockedOutputs = 0;
    
    for (uint32 i = 0; i < 32; i++)
    {
        if (outputIDs & (1 << i))
        {
            if (i >= SysInfo_GetOutputCount())
                return R_IndexOutOfRange;
            
            Outputs_TurnOff(i);
            
            _blockedOutputs |= (1 << i);
            
        }
    }
    
    return R_Ok;
}

/*****************************************************************************
** Function name:   Outputs_Init()
** 
** Description:     Initialize processor pins as outputs
*****************************************************************************/
void Outputs_Init() 
{
    SysInfo_SetOutputCount(BASE_OUTPUT_COUNT);
    
    // set as outputs
    for (int i = 0; i < BASE_OUTPUT_COUNT; i++)
        GPIO_SetDir(PORT0, OUTPUTS_OFFSET + i, 1);
    
    // initialize desriptors    
    for (int i = 0; i < SysInfo_GetOutputCount(); i++)
    {
        _outputsDescriptor[i].functionality = OCH_LEVEL;
        _outputsDescriptor[i].pulseLength = 1000;
        _outputsDescriptor[i].pulseDelay = 1000;
        _outputsDescriptor[i].delayToOn = 0;
        _outputsDescriptor[i].delayToOff = 0;
        _outputsDescriptor[i].onOffTimer = 0;
        _outputsDescriptor[i].logicStatus = LOGIC_OFF;    // all off or not active
        _outputsDescriptor[i].funcStatus = FUNC_OFF;
        _outputsDescriptor[i].activated = 0;
        _outputsDescriptor[i].inverted = 0;
        _outputsDescriptor[i].forcedOff = 0;
        _outputsDescriptor[i].isOn = 0;
        _outputsDescriptor[i].timesActivated = 0;
    }
    
    // set all off
    for (int i = 0; i < SysInfo_GetOutputCount(); i++)
        Outputs_SetOutput(i);
    
    Outputs_OnOutputChanged = null;
    
    Tasks_Register(Outputs_Task, 5);
}

Result_e Outputs_TurnOff(uint32 outputID)
{
    if (outputID >= SysInfo_GetOutputCount())
        return R_IndexOutOfRange;
    
    _outputsDescriptor[outputID].activated = 0;
    _outputsDescriptor[outputID].timesActivated = 0;
        
    Outputs_SetOutput(outputID);
    
    return R_Ok;
}

Result_e Outputs_TurnAllOff()
{
    uint32 i;
    Result_e result;
    
    for (i = 0; i < SysInfo_GetOutputCount(); i++)
    {
        result = Outputs_TurnOff(i);
        if (result != R_Ok)
            return result;
    }
        
    return R_Ok;
}

/*****************************************************************************
** Function name:   Outputs_ConfigLevel
**
** Description:     Configure level functionality for selected output
** Parameters:      uint32 outputId - id of the output to be configured
*****************************************************************************/
int Outputs_ConfigLevel(uint32 outputId, uint32 delayToOn, uint32 delayToOff,
                        uint32 inverted)
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    
    /* reset to default */
//    _outputsDescriptor[outputId].onOffTimer = 0;    
//    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;
//    _outputsDescriptor[outputId].funcStatus = FUNC_OFF;
//    _outputsDescriptor[outputId].activated = 0;
//    _outputsDescriptor[outputId].forcedOff = 0;
//    _outputsDescriptor[outputId].isOn = 0;
//    _outputsDescriptor[outputId].timesActivated = 0;
    
    _outputsDescriptor[outputId].functionality = OCH_LEVEL;
    _outputsDescriptor[outputId].delayToOn = delayToOn;
    _outputsDescriptor[outputId].delayToOff = delayToOff;
    _outputsDescriptor[outputId].inverted = inverted & 0x01;
    Outputs_SetOutput(outputId);
    
    Outputs_ReportCurrentLogicState(outputId);
    
    return 0;
}

/*****************************************************************************
** Function name:   Outputs_ConfigFrequency
**
** Description:     Configure frequency functionality for selected output (time
**                  in ms)
** Parameters:      uint32 outputId - id of the output to be configured
**                  uint32 onTime - time during the output will be switched on 
**                  uint32 offTime - time during the output will be switched off                
*****************************************************************************/
int Outputs_ConfigFrequency(uint32 outputId, uint32 onTime, uint32 offTime,
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted) 
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    
    /* reset to default */
//    _outputsDescriptor[outputId].onOffTimer = 0;
//    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;
//    _outputsDescriptor[outputId].funcStatus = FUNC_OFF;
//    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;    // all off or not active
//    _outputsDescriptor[outputId].activated = 0;
//    _outputsDescriptor[outputId].isOn = 0;
//    _outputsDescriptor[outputId].timesActivated = 0;
    
    //if (_outputsDescriptor[outputId].funcStatus == FUNC_ON)
  //      _outputsDescriptor[outputId].funcStatus = 
    
    _outputsDescriptor[outputId].functionality = OCH_FREQUENCY;
    _outputsDescriptor[outputId].pulseLength = onTime;
    _outputsDescriptor[outputId].pulseDelay = offTime;
    _outputsDescriptor[outputId].delayToOn = delayToOn;
    _outputsDescriptor[outputId].delayToOff = delayToOff;
    _outputsDescriptor[outputId].forcedOff = forcedOff & 0x01;
    _outputsDescriptor[outputId].inverted = inverted & 0x01;
    Outputs_SetOutput(outputId);
    
    Outputs_ReportCurrentLogicState(outputId);
    
    return 0;
}

/*****************************************************************************
** Function name:   Outputs_ConfigPulse
**
** Description:     Configure pulse functionality for selected output (time
**                  in ms)
** Parameters:      uint32 outputId - id of the output to be configured
**                  uint32 pulseTime - time of the pulse
*****************************************************************************/
int Outputs_ConfigPulse(uint32 outputId, uint32 pulseTime, 
    uint32 delayToOn, uint32 delayToOff, uint32 forcedOff, uint32 inverted) 
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    
    /* reset to default */
//    _outputsDescriptor[outputId].onOffTimer = 0;
//    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;
//    _outputsDescriptor[outputId].funcStatus = FUNC_OFF;
        
//    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;    // all off or not active
//    _outputsDescriptor[outputId].activated = 0;
//    _outputsDescriptor[outputId].isOn = 0;
//    _outputsDescriptor[outputId].timesActivated = 0;
    
    _outputsDescriptor[outputId].functionality = OCH_PULSE;
    _outputsDescriptor[outputId].pulseLength = pulseTime;
    _outputsDescriptor[outputId].delayToOn = delayToOn;
    _outputsDescriptor[outputId].delayToOff = delayToOff;
    _outputsDescriptor[outputId].forcedOff = forcedOff & 0x01;
    _outputsDescriptor[outputId].inverted = inverted & 0x01;
    Outputs_SetOutput(outputId);
    
    Outputs_ReportCurrentLogicState(outputId);

    return 0;
}

int Outputs_EnsureMinDelayToOn(uint32 outputId, uint32 minDelayToOn)
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    
    if (_outputsDescriptor[outputId].delayToOn < minDelayToOn)
        _outputsDescriptor[outputId].delayToOn = minDelayToOn;
    
    return 0;
}

/*****************************************************************************
** Function name:   Outputs_InvertOutput
**
** Description:     Invert the functionality for selected output
** Parameters:      uint32 outputId - id of the output that should be inverted
******************************************************************************/
int Outputs_InvertOutput(uint32 outputId, uint32 isInverted) 
{
    if (outputId >= SysInfo_GetOutputCount())
        return -1;
    
    _outputsDescriptor[outputId].inverted = isInverted & 0x01;
    Outputs_SetOutput(outputId);
    
    return 0;
}

/*****************************************************************************
** Function name:   Outputs_ActivateOutput
**
** Description:     Activate the functionality for selected output
** Parameters:      uint32 outputId - id of the output that should be activated
**                  uint32 activate - activate or deactivate
******************************************************************************/
Result_e Outputs_ActivateOutput(uint32 outputId, uint32 activate)
{
    if (outputId >= SysInfo_GetOutputCount())
        return R_IndexOutOfRange;
    
    /* Increment for every activation and decrement for deactivation */
    if (activate & 0x01)
        _outputsDescriptor[outputId].timesActivated++;
    else
        _outputsDescriptor[outputId].timesActivated--;
    
    /* Just to be sure ;) */
    if (_outputsDescriptor[outputId].timesActivated < 0)
        _outputsDescriptor[outputId].timesActivated = 0;
    
    bool previousValue = _outputsDescriptor[outputId].activated;
    
    /* Activate output if at least one call for activate and deactivate only            */
    /* if deactivate called enough times (once for every activation) - OR functionality */
    _outputsDescriptor[outputId].activated = (_outputsDescriptor[outputId].timesActivated > 0);
    
    
    if (_outputsDescriptor[outputId].activated != previousValue)
    {
        Outputs_FireOutputLogicChanged(outputId, _outputsDescriptor[outputId].activated);
    }
    
    // applies initial value of the relay according to output characteristic 
    // useful especially when inversion flag changes
    Outputs_SetOutput(outputId);
    
    /* Force Process State in case of rapid activate/deactivate calls */
    Outputs_Task();
    
    return R_Ok;
}

uint32 Outputs_IsActivated(uint32 outputID)
{
    return _outputsDescriptor[outputID].activated;
}

/*****************************************************************************
** Function name:   Outputs_ProcessState
**
** Description:     Process the logic output state (off, on, delay on, delay off)
**                  In on state the functionality itself is performed (level, 
**                  frequency, pulse)
** Parameters:      uint32 outputId - id of the output that should be activated
******************************************************************************/
void Outputs_ProcessState(uint32 outputId) 
{
    switch (_outputsDescriptor[outputId].logicStatus) 
    {
        case LOGIC_OFF:                                                              
            if (_outputsDescriptor[outputId].activated)
            {
                if (_outputsDescriptor[outputId].delayToOn > 0)                 
                {
                    _outputsDescriptor[outputId].logicStatus = LOGIC_DELAY_ON;
                    _outputsDescriptor[outputId].onOffTimer = SysTick_GetTickCount();
                }
                else                                                            
                    _outputsDescriptor[outputId].logicStatus = LOGIC_ON;
            }
            break; 
            
        case LOGIC_DELAY_ON:  
            if (_outputsDescriptor[outputId].activated)
            {
                uint32 elapsedTime = SysTick_GetElapsedTime(_outputsDescriptor[outputId].onOffTimer);
                
                if (elapsedTime >= _outputsDescriptor[outputId].delayToOn) 
                    _outputsDescriptor[outputId].logicStatus = LOGIC_ON;
            }
            else                                                                
                _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;
            break;
            
        case LOGIC_ON: 
            if (!_outputsDescriptor[outputId].activated)
            {
                if (_outputsDescriptor[outputId].delayToOff > 0)                
                {
                    _outputsDescriptor[outputId].logicStatus = LOGIC_DELAY_OFF;
                    _outputsDescriptor[outputId].onOffTimer = SysTick_GetTickCount();
                }
                else                                                           
                    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;
            }
            break;
            
        case LOGIC_DELAY_OFF: 
            if (!_outputsDescriptor[outputId].activated)
            {
                uint32 elapsedTime = SysTick_GetElapsedTime(_outputsDescriptor[outputId].onOffTimer);
                
                if (elapsedTime >= _outputsDescriptor[outputId].delayToOff) 
                    _outputsDescriptor[outputId].logicStatus = LOGIC_OFF;
            }
            else                                                
                _outputsDescriptor[outputId].logicStatus = LOGIC_ON;
            
            break;
    }
}

/*****************************************************************************
** Function name:   Outputs_Task
** 
** Description:     Handle outputs timing & switching (in case frequency or pulse 
**                  functionality selected)
*****************************************************************************/
void Outputs_Task() 
{
    uint32 elapsedTime;
    
    for (int i = 0; i < SysInfo_GetOutputCount(); i++)
    {
        Outputs_ProcessState(i);

        OutputDescription_t* pOutput = &(_outputsDescriptor[i]);
        
        switch (pOutput->functionality) 
        {
            case OCH_LEVEL:
                switch (pOutput->funcStatus)
                {
                    case FUNC_PULSE_LENGTH:
                        if (pOutput->logicStatus == LOGIC_OFF)
                        {
                            pOutput->funcStatus = FUNC_OFF;
                            pOutput->isOn = 0;
                            Outputs_SetOutput(i);
                        }
                        break;
                        
                    case FUNC_PULSE_DELAY:
                        if (pOutput->logicStatus == LOGIC_ON ||
                            pOutput->logicStatus == LOGIC_DELAY_OFF)
                        {
                            pOutput->funcStatus = FUNC_PULSE_LENGTH;
                            pOutput->isOn = 1;
                            pOutput->pulseTimer = SysTick_GetTickCount();
                            Outputs_SetOutput(i);
                        }
                        break;
                        
                    case FUNC_OFF:
                        if (pOutput->logicStatus == LOGIC_ON)
                        {
                            pOutput->funcStatus = FUNC_PULSE_LENGTH;
                            pOutput->isOn = 1;
                            pOutput->pulseTimer = SysTick_GetTickCount();
                            Outputs_SetOutput(i);
                        }
                        break;
                }
                break;
                
            case OCH_FREQUENCY:
                switch (pOutput->funcStatus) 
                {
                    case FUNC_OFF:                                              
                        if (pOutput->logicStatus == LOGIC_ON)
                        {
                            pOutput->funcStatus = FUNC_PULSE_LENGTH;           
                            pOutput->pulseTimer = SysTick_GetTickCount();
                            pOutput->isOn = 1;
                            Outputs_SetOutput(i);
                        }
                        break;
                       
                    case FUNC_PULSE_LENGTH:                                              
                        if (pOutput->logicStatus == LOGIC_OFF && pOutput->forcedOff)         
                        {
                            pOutput->funcStatus = FUNC_OFF;
                            pOutput->isOn = 0;
                            Outputs_SetOutput(i);
                            break;
                        }
                        
                        elapsedTime = SysTick_GetElapsedTime(pOutput->pulseTimer);

                        if (elapsedTime >= pOutput->pulseLength)                            
                        {
                            pOutput->pulseTimer = SysTick_GetTickCount();
                            pOutput->funcStatus = FUNC_PULSE_DELAY;
                            pOutput->isOn = 0;
                            Outputs_SetOutput(i);
                        }
                        break;
                        
                    case FUNC_PULSE_DELAY:
                        if (pOutput->logicStatus == LOGIC_OFF)
                        {
                            pOutput->funcStatus = FUNC_OFF;
                            pOutput->isOn = 0;
                            Outputs_SetOutput(i);
                            break;
                        }
                        
                        elapsedTime = SysTick_GetElapsedTime(pOutput->pulseTimer);
                        
                        if (elapsedTime >= pOutput->pulseDelay)
                        {
                            pOutput->pulseTimer = SysTick_GetTickCount();
                            pOutput->funcStatus = FUNC_PULSE_LENGTH;
                            pOutput->isOn = 1;
                            Outputs_SetOutput(i);
                        }
                        break;
                }
                break;
                
            case OCH_PULSE:                    
                switch (pOutput->funcStatus) 
                {
                    case FUNC_OFF:
                        if (pOutput->logicStatus == LOGIC_ON)
                        {
                            pOutput->funcStatus = FUNC_PULSE_LENGTH;           
                            pOutput->pulseTimer = SysTick_GetTickCount();
                            pOutput->isOn = 1;
                            Outputs_SetOutput(i);
                        }
                        break;
                    
                    case FUNC_PULSE_LENGTH:
                        if (pOutput->logicStatus == LOGIC_OFF && pOutput->forcedOff)         
                        {
                            pOutput->funcStatus = FUNC_OFF;
                            pOutput->isOn = 0;
                            Outputs_SetOutput(i);
                            break;
                        }
                        
                        elapsedTime = SysTick_GetElapsedTime(pOutput->pulseTimer);

                        if (elapsedTime >= pOutput->pulseLength)                            
                        {
                            pOutput->funcStatus = FUNC_PULSE_DELAY;
                            pOutput->isOn = 0;
                            Outputs_SetOutput(i);
                        }
                        break;

                    case FUNC_PULSE_DELAY:
                        if (pOutput->logicStatus == LOGIC_OFF)
                            pOutput->funcStatus = FUNC_OFF;
                        break;
                }
            
                break;
        }
    }
     
}