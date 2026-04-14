#ifndef BOOTLOADER

#include "IOExpansion.h"
#include "System\I2C\i2c.h"
#include "System\constants.h"
#include "System\systemInfo.h"
#include "System\Timer\sysTick.h"
#include "IO\InputsEnums.h"

typedef enum
{
    I2C_Idle = 0,
    I2C_Started = 1,
    I2C_Restarted = 2,
    I2C_RepeatedStart = 3,
    I2C_DataACK = 4,
    I2C_DataNACK = 5,
    I2C_Busy = 6,
    I2C_NoData = 7,
    I2C_NACKOnAddress = 8,
    I2C_NACKOnData = 9,
    I2C_ArbitrationLost = 10,
    I2C_Timeout = 11,
    I2C_Ok = 12
} MasterState_e;

#define EXT_INPUTS0_ADDRESS     0x60
#define EXT_INPUTS1_ADDRESS     0x61
#define EXT_OUT1_ADDRESS        0x20
#define EXT_OUT2_ADDRESS        0x21

#define ON_STATE    0x00
#define OFF_STATE   0x01
#define PWM0_STATE  0x02
#define PWM1_STATE  0x03

#define BUFSIZE 10

#define COMMAND_TIMEOUT         20

volatile ExtensionBoard_e _extensionBoardType;

volatile uint8 _ledStatus[8];
volatile uint8 _outputStatus[2];

static I2C_InitType_t _I2C_InitStructure;
volatile uint8 _I2C_MasterBuffer[BUFSIZE];
volatile uint8 _I2C_SlaveBuffer[BUFSIZE];
volatile MasterState_e _I2C_MasterState = I2C_Idle;
volatile uint32 _ReadLength, _WriteLength;
volatile uint32 _RdIndex = 0;
volatile uint32 _WrIndex = 0;

void ProcessI2CData()
{
    _I2C_MasterState = I2C_Idle;
    I2C_SetFlag(I2C_I2CONSET_STA);
    
    uint32 setTime = SysTick_GetTickCount();
    while (1)
    {
        if (_I2C_MasterState != I2C_Idle)
            break;
        uint32 elapsed = SysTick_GetElapsedTime(setTime);
        if (elapsed > COMMAND_TIMEOUT)
            break;
    }
}

void InitLowInputLEDs()
{
    for (uint32 i = 0; i < 4; i++)
        _ledStatus[i] = 0x55;
    
    uint8 initLeds[] = { (EXT_INPUTS0_ADDRESS << 1), 0x12, 0x07, 0x80, 0x0F, 0x80, 0x55, 0x55, 0x55, 0x55 };
    memcpy((uint8*)_I2C_MasterBuffer, initLeds, 10);
    _WriteLength = 10;
    _ReadLength = 0;
    
    ProcessI2CData();
}

/*
void InitHighInputLEDs()
{
    for (uint32 i = 4; i < 8; i++)
        _ledStatus[i] = 0x55;
    
    uint8 initLeds[] = { (EXT_INPUTS1_ADDRESS << 1), 0x12, 0x07, 0x80, 0x0F, 0x80, 0x55, 0x55, 0x55, 0x55 };
    memcpy((uint8*)_I2C_MasterBuffer, initLeds, 10);
    _WriteLength = 10;
    _ReadLength = 0;
    
    ProcessI2CData();
}
*/

void InitLowerOutputs()
{
    /* Prepare data for initialization : shifted address, select configuration register, set 4 low bits as output */
    uint8 initOutputs[] = { EXT_OUT1_ADDRESS << 1, 0x03, 0xF0 };
    memcpy((uint8*)_I2C_MasterBuffer, initOutputs, 3);
    _WriteLength = 3;
    _ReadLength = 0;
    
    ProcessI2CData();
    
    _outputStatus[0] = 0x00;
    for (uint32 i = 0; i < 4; i++) 
        IOExp_SetOutput(i, 0);
}

void InitHigherOutputs()
{
    /* Prepare data for initialization : shifted address, select configuration register, set 4 low bits as output */
    uint8 initOutputs[] = { EXT_OUT2_ADDRESS << 1, 0x03, 0xF0 };
    memcpy((uint8*)_I2C_MasterBuffer, initOutputs, 3);
    _WriteLength = 3;
    _ReadLength = 0;
    
    ProcessI2CData();
    
    _outputStatus[1] = 0x00;    
    for (uint32 i = 4; i < 8; i++)
        IOExp_SetOutput(i, 0);
}

ExtensionBoard_e GetExtBoardType()
{
    return _extensionBoardType;
}

ExtensionBoard_e ResolveExtensionBoardType(bool inputs0Present, bool inputs1Present, bool lowOutputsPresent, bool highOutputsPresent)
{
    SysInfo_Init(4, 4);
    
    if (!inputs0Present && !inputs1Present && !lowOutputsPresent && !highOutputsPresent)
        return ExtNone;
    
    if (inputs0Present && !inputs1Present && !lowOutputsPresent && !highOutputsPresent)
    {
        InitLowInputLEDs();
        //InitHighInputLEDs();
        
        SysInfo_Init(16, 4);
        return Ext12I;
    }
    
    if (inputs0Present && !inputs1Present && lowOutputsPresent && !highOutputsPresent)
    {
        InitLowInputLEDs();
        InitLowerOutputs();
        
        SysInfo_Init(12, 8);
        return Ext8I_4O;
    }
    
    if (inputs0Present && !inputs1Present && lowOutputsPresent && highOutputsPresent)
    {
        InitLowInputLEDs();
        InitLowerOutputs();
        InitHigherOutputs();
        
        SysInfo_Init(8, 12);
        return Ext4I_8O;
    }
    
    if (!inputs0Present && !inputs1Present && lowOutputsPresent && highOutputsPresent)
    {
        InitLowerOutputs();
        InitHigherOutputs();
        
        SysInfo_Init(4, 12);
        return Ext8O;
    }
    
    return ExtUnknown;
}

void IOExp_SetLed(uint32 ledID, uint32 isOn)
{
    if (isOn)
        IOExp_SetLedState(ledID, Alarm);
    else
        IOExp_SetLedState(ledID, Normal);
}

void IOExp_SetLedState(uint32 ledId, InputState_e state)
{
    uint32 address;
    
    uint32 statusIndex = ledId / 4; //((ledId * 2) + 1) / 4;
    uint8* ledStatus = (uint8*)&_ledStatus[statusIndex];
    
//    if (ledId < 8)
//    {
        address = (EXT_INPUTS0_ADDRESS << 1);        
//    }
//    else
//    {
//        address = (EXT_INPUTS1_ADDRESS << 1);
//        ledId -= 8;
//    }
    
    //ledId = ledId * 2 + 1;
    
    uint32 shiftIndex = (ledId % 4) * 2;
    uint32 controlReg = 6 + (ledId / 4);
    
    // clear the data for this led
    *ledStatus &= ~(0x03 << shiftIndex);
    
    switch (state)
    {
        case Normal:
            *ledStatus |= (OFF_STATE << shiftIndex);
            break;
        case Alarm:
            *ledStatus |= (ON_STATE << shiftIndex);
            break;
        case Short:
            *ledStatus |= (PWM0_STATE << shiftIndex);
            break;
        case Break:
            *ledStatus |= (PWM1_STATE << shiftIndex);
            break;
        case Unknown:
        default:
            return;
    }
    
    uint8 data[] = { address, controlReg, *ledStatus };
    memcpy((uint8*)_I2C_MasterBuffer, data, 3);
    _WriteLength = 3;
    _ReadLength = 0;
    
    ProcessI2CData();
}

void IOExp_SetOutput(uint32 outputID, uint32 isOn)
{
    uint32 address;
    uint8* outputStatus;
    
    if (outputID < 4) 
    {
        address = EXT_OUT1_ADDRESS << 1;
        outputStatus = (uint8*)&_outputStatus[0];
    }
    else 
    {
        address = EXT_OUT2_ADDRESS << 1;
        outputID -= 4;
        outputStatus = (uint8*)&_outputStatus[1];
    }
        
    if (!isOn)
        *outputStatus |= (1 << outputID);
    else
        *outputStatus &= ~(1 << outputID);
    
    uint8 data[] = { address, 0x01, *outputStatus };
    memcpy((uint8*)_I2C_MasterBuffer, data, 3);
    _WriteLength = 3;
    _ReadLength = 0;
    
    ProcessI2CData();
}

InputState_e IOExp_ReadFuse()
{
    _I2C_MasterBuffer[0] = (EXT_OUT1_ADDRESS << 1);
    _I2C_MasterBuffer[1] = 0x00;
    _I2C_MasterBuffer[2] = (EXT_OUT1_ADDRESS << 1) | 0x01;
    
    _WriteLength = 2;
    _ReadLength = 2;
    
    ProcessI2CData();
    
    if (_I2C_SlaveBuffer[0] & 0x80)
        return Alarm;
    return Normal;
}

InputState_e IOExp_ReadPower()
{
    _I2C_MasterBuffer[0] = (EXT_OUT1_ADDRESS << 1);
    _I2C_MasterBuffer[1] = 0x00;
    _I2C_MasterBuffer[2] = (EXT_OUT1_ADDRESS << 1) | 0x01;
    
    _WriteLength = 2;
    _ReadLength = 2;
    
    ProcessI2CData();
    
    if (_I2C_SlaveBuffer[0] & 0x40)
        return Alarm;
    return Normal;
}

bool CheckIOPresence(uint32 address)
{
    uint32 firstByte = (address << 1);      /* shift the address up, LSB is the direction (R = 1 / W = 0) */
    
    _I2C_MasterBuffer[0] = firstByte; 
    _I2C_MasterBuffer[1] = 0x00;
    _I2C_MasterBuffer[2] = firstByte | 0x01;    /* 0x01 for the Read operation */
    
    _WriteLength = 2;
    _ReadLength = 2;
    
    _I2C_MasterState = I2C_Idle;
    I2C_SetFlag(I2C_I2CONSET_STA);
    
    uint32 checkStart = SysTick_GetTickCount();
    
    while (1)
    {
        if (_I2C_MasterState == I2C_NACKOnAddress)
            return false;
        
        if (_I2C_MasterState == I2C_Ok)           
            return true;
        
        /* Just a race-condition precaution */
        if (SysTick_GetElapsedTime(checkStart) > COMMAND_TIMEOUT)
            return false;
    }
}



void InitIOExpansion()
{
    _extensionBoardType = ExtNone;
    
    _I2C_InitStructure.Mode = I2C_MASTER;
    _I2C_InitStructure.ClockRate = 400000;
    _I2C_InitStructure.InterruptMode = I2C_INTERRUPT_MODE;

    I2C_Init(&_I2C_InitStructure);
    
//#ifdef DEBUG
    uint8 inputs0Present = CheckIOPresence(EXT_INPUTS0_ADDRESS);
    uint8 inputs1Present = CheckIOPresence(EXT_INPUTS1_ADDRESS);
    uint8 lowOutputsPresent = CheckIOPresence(EXT_OUT1_ADDRESS);
    uint8 highOutputsPresent = CheckIOPresence(EXT_OUT2_ADDRESS);
    
    _extensionBoardType = ResolveExtensionBoardType(inputs0Present, inputs1Present, lowOutputsPresent, highOutputsPresent);
//#else
//     _extensionBoardType = ResolveExtensionBoardType(
//                                                     CheckIOPresence(EXT_INPUTS0_ADDRESS),
//                                                     CheckIOPresence(EXT_INPUTS1_ADDRESS),
//                                                     CheckIOPresence(EXT_OUT1_ADDRESS),
//                                                     CheckIOPresence(EXT_OUT2_ADDRESS)
//                                                         );
//#endif
}


/**
  * @brief  General Master Interrupt handler for I2C peripheral
  *
  * @param  None
  * @retval None
  */
#pragma diag_suppress=Pa082
void I2C_IRQHandler(void) 
{
    uint32 statValue = I2C_GetI2CStatus();
    switch (statValue)
    {
        /* 0x08: A Start condition is issued. */
        case I2C_I2STAT_M_TX_START:     
            _WrIndex = 0;
            I2C_SendByte(_I2C_MasterBuffer[_WrIndex++]);
            
            /* Clear SI interrupt flag + clear start flag */
            I2C_ClearFlag(I2C_I2CONCLR_SIC | I2C_I2CONCLR_STAC);    
            break;
        
        /* 0x10: A repeated started is issued (in case of read) */
        case I2C_I2STAT_M_TX_RESTART:			        
            _RdIndex = 0;
            /* Send SLA with R bit set */
            I2C_SendByte(_I2C_MasterBuffer[_WrIndex++]); 			
            I2C_ClearFlag(I2C_I2CONCLR_SIC | I2C_I2CONCLR_STAC);
            break;
        
        /* 0x18: SLA+W + ACK. Regardless, it's a ACK */
        case I2C_I2STAT_M_TX_SLAW_ACK:	        
            if (_WriteLength == 1)
            {
                I2C_SetFlag(I2C_I2CONSET_STO);      	/* Set Stop flag */
                _I2C_MasterState = I2C_NoData;
            }
            else
            {
                I2C_SendByte(_I2C_MasterBuffer[_WrIndex++]);
            }
            I2C_ClearFlag(I2C_I2CONCLR_SIC);
            break;
        
        /* 0x28: Data byte has been transmitted and ACK */
        case I2C_I2STAT_M_TX_DAT_ACK:			
            if (_WrIndex < _WriteLength)
            {   
                I2C_SendByte(_I2C_MasterBuffer[_WrIndex++]);
            }
            else
            {
                if (_ReadLength != 0)	  
                {
                    I2C_SetFlag(I2C_I2CONSET_STA);	/* Set Repeated-start flag so reading can start */
                }
                else
                {
                    I2C_SetFlag(I2C_I2CONSET_STO);	/* No reading so Set Stop flag */
                    _I2C_MasterState = I2C_Ok;
                }
            }
            I2C_ClearFlag(I2C_I2CONCLR_SIC);
            break;
        
        /* 0x30: Data has been transmitted, NACK has been received */ 
        case I2C_I2STAT_M_TX_DAT_NACK:			
            I2C_SetFlag(I2C_I2CONSET_STO);		/* Set Stop flag */
            _I2C_MasterState = I2C_NACKOnData;
            I2C_ClearFlag(I2C_I2CONCLR_SIC);
            break;
            
        /* 0x40: Master Receive, SLA_R has been sent */
        case I2C_I2STAT_M_RX_SLAR_ACK:		
            if ((_RdIndex + 1) < _ReadLength)
            {
                /* Will go to State 0x50 */
                I2C_SetFlag(I2C_I2CONSET_AA);		/* assert ACK after data is received */
            }
            else
            {
                /* Will go to State 0x58 */
                I2C_ClearFlag(I2C_I2CONCLR_AAC);	/* assert NACK after data is received */
            }
            I2C_ClearFlag(I2C_I2CONCLR_SIC);
            break;
        
        /* 0x50: Data byte has been received, regardless following ACK or NACK */
        case I2C_I2STAT_M_RX_DAT_ACK:			
            _I2C_SlaveBuffer[_RdIndex++] = I2C_GetByte();
            if ((_RdIndex + 1) < _ReadLength)
            {   
                I2C_SetFlag(I2C_I2CONSET_AA);		/* assert ACK after data is received */
            }
            else
            {
                I2C_ClearFlag(I2C_I2CONCLR_AAC);	/* assert NACK on last byte */
            }
            I2C_ClearFlag(I2C_I2CONCLR_SIC);
            break;
        
        /*0x58: Data has been received, NACK has been returned */
        case I2C_I2STAT_M_RX_DAT_NACK:			
            _I2C_SlaveBuffer[_RdIndex++] = I2C_GetByte();
            _I2C_MasterState = I2C_Ok;
            I2C_SetFlag(I2C_I2CONSET_STO);		/* Set Stop flag */ 
            I2C_ClearFlag(I2C_I2CONCLR_SIC);		/* Clear SI flag */
            break;
        
        /*0x20: SLA+W has been transmitted, NACK has been received */            
        case I2C_I2STAT_M_TX_SLAW_NACK:			
        /*0x48: SLA+R has been transmitted, NACK has been received */
        case I2C_I2STAT_M_RX_SLAR_NACK:			
            I2C_SetFlag(I2C_I2CONSET_STO);		/* Set Stop flag */
            _I2C_MasterState = I2C_NACKOnAddress;
            I2C_ClearFlag(I2C_I2CONCLR_SIC);
            break;
        
        /*0x38: Arbitration lost, in this example, we don't deal with multiple master situation */            
        case I2C_I2STAT_M_TX_ARB_LOST:			
        default:
            _I2C_MasterState = I2C_ArbitrationLost;
            I2C_ClearFlag(I2C_I2CONCLR_SIC);	
            break;
    }
    return;
}
#pragma diag_default=Pa082

#endif // BOOTLOADER