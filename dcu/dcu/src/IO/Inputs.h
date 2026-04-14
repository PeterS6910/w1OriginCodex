#pragma once

#include "InputsEnums.h"
#include "System\baseTypes.h"
#include "System\resultCodes.h"

#define INPUTS_DEFAULT_FILTERTIME   50
#define DIP_DEFAULT_FILTERTIME      1500
#define MAX_ADC_VALUE               1023

#define FT      0x00
#define FT_TdON 0x01
#define FT_dON  0x02
#define FT_dOff 0x03

/* Defines for easier access of bits that marks change in special input.    */
/* Regular inputs (BSI/DI) are located on the bits 0 to INPUT_COUNT - 1     */
#define SI_TAMPER_BIT      (uint32)(1 << 31)
#define SI_DIP6_BIT        (uint32)(1 << 30)
//#define DIP2_BIT        (uint32)(1 << 29)
#define SI_ADDRESS_BIT     (uint32)(1 << 28)
#define SI_FUSE_BIT        (uint32)(1 << 27)
#define SI_EXT_FUSE_BIT    (uint32)(1 << 26)
#define SI_EXT_POWER_BIT   (uint32)(1 << 25)

#ifndef BOOTLOADER
extern volatile InputState_e _inputStates[MAX_INPUT_COUNT];    /* Valid states for BSI/DI inputs */
#endif
extern volatile SpecialInputState_t _specialInputState;     /* Valid states for special inputs */
extern volatile uint32 _inputChanged;                     /* Bits indicating if the input has changed its value, needs to be cleared */

typedef struct
{
    uint8 S;
    uint8 N;
    uint8 A;
    uint8 B;
} FilterTableRow_t;

#ifndef BOOTLOADER
typedef struct 
{
    InputType_e inputType;                  /* Specifies the input type */
    uint32 filtertime;                    /* Basic filtertime */
    uint32 delayOn;                       /* Delay from Normal to Alarm state */
    uint32 delayOff;                      /* Delay from Alarm to Normal state */
    uint32 tamperDelayOn;                 /* Delay to Short of Break from Normal or Alarm */
    uint32 toLevel[3];                    /* Switching voltage levels */
    InputState_e inputStateForLevel[4];     /* States for respective levels */
    uint32 validLevel;                     /* Valid level for current inputs */
    uint32 currentLevel;                   /* Current (before filtertime) level */
    uint32 changeTime;                    /* time that the last change happened */
    uint32 bindedOutputs;                 /* Outputs assigned to this input */
    uint32 activatedOutputs;              /* Helper variable to mark already activated outputs */
    bool forceRefresh;
} InputDescription_t;
#endif

typedef struct
{
    InputType_e inputType;                  /* Specifies the input type */
    uint32 filtertime;                    /* Basic filtertime */
    uint32 delayOn;                       /* Delay from Normal to Alarm state */
    uint32 changeTime;                    /* time that the last change happened */
    int32 validLevel;                     /* Valid level for current inputs */
    int32 currentLevel;                   /* Current (before filtertime) level */
} SpecialInputDescriptor_t;



#ifndef BOOTLOADER

void Inputs_BindOnSpecialInputChanged(void (*onSpecialInputChanged)(uint32 inputId, InputState_e));

void Inputs_BindOnInputChanged(void (*onInputChanged)(uint32 inputId, InputState_e inputState));

//void Inputs_BindOnOutputLogicStateChanged(void (*onOutputLogicStateChanged)(uint32 outputID, uint32 active));

Result_e Inputs_SetReportedInputs(uint32 inputIDs);

Result_e Inputs_SetReportedInputsEx(uint32 inputIDs);

Result_e Inputs_UnsetReportedInputs(uint32 inputIDs);

void Inputs_BindOnDIPChanged(void (*onDIPChanged)(uint32 dipValue));

#endif

#ifndef BOOTLOADER
//void Inputs_FireOutputLogicChanged(uint32 outputID, uint32 active);

int Inputs_SetInputType(uint32 index, InputType_e inputType);

int Inputs_UnsetInput(uint32 index);

int Inputs_SetBsiLevels(uint32 toLevel1, uint32 toLevel2, uint32 toLevel3);

int Inputs_RemapBsiStates(uint32 index,
                      InputState_e inputStateLevel0,
                      InputState_e inputStateLevel1,
                      InputState_e inputStateLevel2,
                      InputState_e inputStateLevel3);

int Inputs_SetBsiParameters(uint32 index, 
                      uint32 filtertime,
                      uint32 delayOn, 
                      uint32 delayOff,
                      uint32 tamperDelayOn,
                      bool inverted
                      );

int Inputs_SetDiParameters(uint32 index,
                       uint32 filtertime,
                       uint32 delayOn, 
                       uint32 delayOff,
                       bool inverted
                       );

int Inputs_RemapDiStates(uint32 index,
                       InputState_e inputStateForLevel0,
                       InputState_e inputStateForLevel1);
#endif

int Inputs_SetTamperParameters(uint32 filtertime, uint32 delayOn);

#ifndef BOOTLOADER
int Inputs_BindOutputToInput(uint32 outputId, uint32 inputId);

int Inputs_UnbindOutputToInput(uint32 outputId, uint32 inputId);

int Inputs_UnbindAll();

void Inputs_FireAllStatuses();
#endif

void Inputs_Init();

void Inputs_Task();

