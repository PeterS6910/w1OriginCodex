#ifndef BOOTLOADER

#include "testing.h"
#include "System\Timer\sysTick.h"
#include "System\GPIO\gpio.h"
#include "System\Tasks.h"
#include "System\systemInfo.h"

#include "Communication\communication.h"
#include "IO\Outputs.h"
#include "IO\Inputs.h"
#include "IO\IOExpansion.h"
#include "DSM\dsm.h"
#include "System\IAP\iap.h"
#include "System\ADC\adc.h"

#include "Communication\clspGeneric.h"

uint32 _startTime = 0;
uint32 _ledID = 2;

#define IO_TEST_CASES   2
#define IO_TEST_TIMEOUT 500

void Test_InitLEDTest()
{
    _startTime = SysTick_GetTickCount();
    
    GPIO_SetDir(PORT1, 2, 1);
    GPIO_SetDir(PORT1, 3, 1);
    GPIO_SetDir(PORT1, 4, 1);
    GPIO_SetDir(PORT1, 5, 1);
    
    GPIO_SetValue(PORT1, 2, 1);
    GPIO_SetValue(PORT1, 3, 1);
    GPIO_SetValue(PORT1, 4, 1);
    GPIO_SetValue(PORT1, 5, 1);
    
    Tasks_Register(Test_LEDTask, 10);
}

void Test_LEDTask()
{
    uint32 elapsed = SysTick_GetElapsedTime(_startTime);
    
    if (elapsed > 500)
    {
        GPIO_SetValue(PORT1, _ledID, 1);
        
        _ledID++;
        
        if (_ledID > 5)
            _ledID = 2;
        
        GPIO_SetValue(PORT1, _ledID, 0);
        
        _startTime = SysTick_GetTickCount();
    }
}

volatile uint32 _ioTestState = 0;
volatile uint32 _progress = 0;
volatile uint32 _testCase = 0;

volatile uint32 _timeout = 0;

volatile uint32 _outputId = 0;
volatile uint32 _outputIdFrom = 0;
volatile uint32 _outputIdTo = 0;

void Test_InputsTestTask();

void Test_FireIOTestProgressNotifier(uint32 progress)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_IO_TEST_PROGRESS, 1, false);
    if (null != frame)
    {
        frame->optionalData[0] = progress;
        
        Clsp_EnqueueFrame(frame, false);
    }
}

void Test_FireIOTestTimeout(uint32 outputId)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_IO_TEST_TIMEOUT, 1, false);
    if (frame != null)
    {
        frame->optionalData[0] = outputId;
        Clsp_EnqueueFrame(frame, false);
    }
}

void Test_FireIOTestOk()
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_IO_TEST_OK, 0, false);
    if (frame != null)
        Clsp_EnqueueFrame(frame, false);
}

void Test_StartIOTest(uint32 isInverted)
{
    _progress = 0;
    _ioTestState = 0;
    _testCase = 0;
    
    Test_FireIOTestProgressNotifier(_progress);
    
    DSM_Stop();
    // prepare config
    for (uint32 i = 0; i < SysInfo_GetOutputCount(); i++) 
    {
        Outputs_TurnOff(i);
        Outputs_ConfigLevel(i, 0, 0, isInverted);
    }

    switch (GetExtBoardType())
    {
        case ExtNone:
            Inputs_SetBsiLevels(243, 271, 307);
            _outputIdFrom = 0;
            _outputIdTo = 3;
            break;
        case Ext4I_8O:
            Inputs_SetBsiLevels(150, 164, 180);
            _outputIdFrom = 4;
            _outputIdTo = 11;
            break;
        case Ext8I_4O:
            Inputs_SetBsiLevels(393, 433, 473);
            _outputIdFrom = 4;
            _outputIdTo = 7;
            break;
        case Ext12I:
            Inputs_SetBsiLevels(567, 607, 647);
            _outputIdFrom = 0;
            _outputIdTo = 3;
            break;
        default:
        	break;
    }
    
    _outputId = _outputIdFrom;    
    
    for (uint32 i = 0; i < SysInfo_GetInputCount(); i++) 
    {
        Inputs_SetBsiParameters(i, 0, 0, 0, 0, false);
        //Inputs_RemapBsiStates(i, Short, Normal, Alarm, Break);
    }
    
    Tasks_Register(Test_InputsTestTask, 10);
}

uint32 Test_CheckInputsStatus(uint32 activeOutput)
{
    uint32 repeater = 1;
    uint32 shift = 0;
    
    switch (GetExtBoardType())
    {
        case ExtNone:
            repeater = 1;
            break;
        case Ext4I_8O:
            repeater = 1;
            if (activeOutput > 7)
                activeOutput -= 4;
            shift = BASE_INPUT_COUNT;
            break;
        case Ext8I_4O:
            repeater = 2;
            shift = BASE_INPUT_COUNT;
            break;
        case Ext12I:
            repeater = 4;
            break;
        default:
            break;
    }
    
    for (uint32 j = 0; j < repeater; j++)
    {
        if (_inputStates[activeOutput + (BASE_INPUT_COUNT * j)] != Normal)
            return 0;
        
        for (uint32 i = 0; i < BASE_INPUT_COUNT; i++)
        {
            if ((i + shift) == activeOutput)
                continue;
            
            if (_inputStates[(i + shift) + (BASE_INPUT_COUNT * j)] != Alarm)
                return 0;
        }
    }
    
    return 1;
}

void Test_InputsTestTask()
{
    uint32 elapsed;
    
    switch (_ioTestState)
    {
        /* Activate next output */
        case 0:
            Outputs_ActivateOutput(_outputId, 1);
            _timeout = SysTick_GetTickCount();
            _ioTestState = 1;
            break;
            
        /* Check input status */
        case 1:
            if (Test_CheckInputsStatus(_outputId))
            {
                _ioTestState = 2;
                _progress += (float)(100 / (IO_TEST_CASES * BASE_INPUT_COUNT));
                Test_FireIOTestProgressNotifier(_progress);
                                
                break;
            }
            
            elapsed = SysTick_GetElapsedTime(_timeout);
            if (elapsed > IO_TEST_TIMEOUT)
            {
                // test failed -> end it.
                Tasks_Unregister(Test_InputsTestTask);
                Test_FireIOTestTimeout(_outputId);
                Outputs_TurnAllOff();
            }
            break;        
            
        /* Wait for specified time (so the tester is able to see LEDs */
        case 2:
            elapsed = SysTick_GetElapsedTime(_timeout);
            if (elapsed > IO_TEST_TIMEOUT)
                _ioTestState = 3;
            break;
            
        /* Disable output and continue */
        case 3:
            Outputs_ActivateOutput(_outputId, 0);
            _outputId++;
            if (_outputId > _outputIdTo)
            {
                _outputId = _outputIdFrom;
                _testCase++;
            }
            if (_testCase >= IO_TEST_CASES)
            {
                Test_FireIOTestProgressNotifier(100);
                Test_FireIOTestOk();
                Tasks_Unregister(Test_InputsTestTask);        
                Outputs_TurnAllOff();
            }
            
            _ioTestState = 0;
            
            break;
    }
}

#ifdef DEBUG_GENERATOR

volatile uint32 _randomX = 43908123;
volatile uint32 _isRandomInitialized = false;

void Test_InitRandomGenerator()
{
    byte* uid = IAP_ReadSerialNumber();
    for (uint32 i = 0; i < 16; i++)
        _randomX += (uid[i] * 100);
    
    _isRandomInitialized = true;
}

uint32 Test_GetRandomValue(uint32 from, uint32 to)
{
    if (to <= from)
        return 0;
    
    if (!_isRandomInitialized)
        Test_InitRandomGenerator();
    
    uint32 mod = to - from;
    _randomX += SysTick_GetTickCount();
    _randomX = (265 * _randomX + 71);
    
    uint32 result = (_randomX % mod) + from;
    
    return result;
}



typedef struct 
{
    uint32 minInterval;
    uint32 maxInterval;
    uint32 nextTime;
    uint32 timer;

    Test_CardType_e cardType;
} Test_CardGeneratorContext_t;

volatile Test_CardGeneratorContext_t _cardGenContext;
volatile bool _cardGenContextInit = false;

volatile uint32 _maxCGenTime = 0;
void Test_CardGeneratorTask()
{
    if (!_cardGenContextInit)
    {
        _cardGenContext.minInterval = 3000;
        _cardGenContext.maxInterval = 5000;
        _cardGenContext.nextTime = _cardGenContext.minInterval;
        _cardGenContext.timer = SysTick_GetTickCount();
        
        _cardGenContextInit = true;
    }
    
    uint32 elapsed = SysTick_GetElapsedTime(_cardGenContext.timer);
    if (elapsed > _cardGenContext.nextTime)
    {
        uint32 startTime = SysTick_GetTickCount();
        
        _cardGenContext.timer = SysTick_GetTickCount();
        _cardGenContext.nextTime = Test_GetRandomValue(_cardGenContext.minInterval, _cardGenContext.maxInterval);
        
        ClspFrame_t* frame = null;
               
        switch (_cardGenContext.cardType)
        {
            case Test_MifareCSN:
            default:
                frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_CR_DATA_RECEIVED, 8, true);
                frame->optionalData[0] = 1;
                frame->optionalData[1] = 0xA0;
                frame->optionalData[2] = 0x20;
                frame->optionalData[3] = 0x38;
                frame->optionalData[4] = 0x65;
                frame->optionalData[5] = 0x91;
                frame->optionalData[6] = Test_GetRandomValue(1, 9) | (Test_GetRandomValue(1, 9) << 4);
                frame->optionalData[7] = Test_GetRandomValue(1, 9) | (Test_GetRandomValue(1, 9) << 4);
                break;
                
            case Test_MifareSector:
                frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_CR_DATA_RECEIVED, 12, true);
                frame->optionalData[0] = 1;
                frame->optionalData[1] = 0x41;
                frame->optionalData[2] = 0x33;
                frame->optionalData[3] = 0x52;
                frame->optionalData[4] = 0x31;
                frame->optionalData[5] = 0x11;
                frame->optionalData[6] = 0x10;
                frame->optionalData[7] = 0x30;
                frame->optionalData[8] = 0x00;
                frame->optionalData[9] = 0x10;
                frame->optionalData[10] = Test_GetRandomValue(1, 9) | (Test_GetRandomValue(1, 9) << 4);
                frame->optionalData[11] = Test_GetRandomValue(1, 9) | (Test_GetRandomValue(1, 9) << 4);
                break;
        }

            if (frame != null)        
                Clsp_EnqueueFrame(frame, false);
            
    }
}

volatile bool _testCardGenEnabled = false;
void Test_ToggleCardGenerator(bool enable, Test_CardType_e cardType, int minTime, int maxTime)
{
    if (minTime >= 0)
        _cardGenContext.minInterval = minTime;
    if (maxTime >= 0)
        _cardGenContext.maxInterval = maxTime;
    
    _cardGenContext.cardType = cardType;
    
    if (enable && !_testCardGenEnabled)
    {
        Tasks_Register(Test_CardGeneratorTask, 10);
        _testCardGenEnabled = true;
    }
    else if (!enable && _testCardGenEnabled)
    {
        Tasks_Unregister(Test_CardGeneratorTask);
        _testCardGenEnabled = false;
    }
}

volatile bool _testADCGenEnabled = false;
void Test_ToggleADCGenerator(bool enable, int minTime, int maxTime)
{
    if (minTime >= 0 && maxTime >= 0)
        ADC_InitGenerator(minTime, maxTime);
    
    if (enable && !_testADCGenEnabled)
    {
        Tasks_Register(ADC_RandomTask, 10);
        _testADCGenEnabled = true;
    }
    else if (!enable && _testADCGenEnabled)
    {
        Tasks_Unregister(ADC_RandomTask);
        _testADCGenEnabled = false;
    }
}

#endif // DEBUG_GENERATOR

#endif // BOOTLOADER
