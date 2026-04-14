#include "communication.h"

#include "System\constants.h"
#include "System\baseTypes.h"

#ifndef BOOTLOADER
#include "DSM\dsmConfig.h"
#include "DSM\dsm.h"
#include "CardReaders\CrCommunicator.h"
#include "CardReaders\CrProtocol.h"
#include "Testing\testing.h"
#else
#include "IO\Inputs.h"
#endif

#include "System\resultCodes.h"
#include "Communication\Clsp485\clsp485.h"
#include "Communication\clspGeneric.h"
#include "System\IAP\iap.h"
#include "System\systemInfo.h"
#include "System\Timer\sysTick.h"
#include "System\UART\uart.h"

#include "System\lpc122x.h"
#include "System\dmHandler.h"

#ifndef BOOTLOADER
#include "app_version.h"
#else
#include "boot_version.h"
#include "Flashloader\flashloader.h"
#endif

extern void StartApplication();
extern void StartBootloader();

void Comm_SendACK(uint32 command, uint32 seq)
{
    ClspFrame_t* frame = Clsp_CreateFrame(
#ifndef BOOTLOADER
            PROTO_ACCESS
#else
            PROTO_UPLOADER
#endif
                                                    , COMM_ACK, 2, true);
    if (null != frame) {
        frame->optionalData[0] = seq;
        frame->optionalData[1] = command;

        //if (!
            Clsp_EnqueueFrame(frame,false);
            //)
            //asm("NOP");
    }
#ifdef DEBUG
    else
    	__ASM("NOP");
#endif
}

void Comm_SendNACK(uint32 command, Result_e errorCode, uint32 seq)
{
    ClspFrame_t* frame = Clsp_CreateFrame(
#ifndef BOOTLOADER
            PROTO_ACCESS
#else
            PROTO_UPLOADER
#endif
            , COMM_NACK, 3, true);
    if (null != frame) {
        frame->optionalData[0] = seq;
        frame->optionalData[1] = command;
        frame->optionalData[2] = errorCode;

        Clsp_EnqueueFrame(frame, false);
    }
#ifdef DEBUG
    else
    	__ASM("NOP");
#endif
}

void Comm_FireResultNotifier(uint32 command, Result_e result, uint32 seq)
{
    if (result == R_Ok)
        Comm_SendACK(command, seq);
    else
        Comm_SendNACK(command, result, seq);
}

//volatile InputState_e _state[4];
//volatile int _inputsCount[4];

#ifndef BOOTLOADER
void Comm_OnInputChanged(uint32 inputId, InputState_e inputState)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_INPUT_CHANGED, 2, true);
    if (null != frame) {
        frame->optionalData[0] = inputId;
        frame->optionalData[1] = inputState;

        Clsp_EnqueueFrame(frame,false);
    }

    //_state[inputId] = inputState;
    //_inputsCount[inputId]++;
}
#endif

void Comm_OnSpecialInputChanged(uint32 inputId, InputState_e inputState)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_SPECIAL_INPUT_CHANGED, 2, true);
    if (null != frame) {
        frame->optionalData[0] = inputId;
        frame->optionalData[1] = inputState;

        Clsp_EnqueueFrame(frame,false);
    }
}

void Comm_OnOutputChanged(uint32 outputId, uint32 isOn)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_OUTPUT_CHANGED, 2, true);
    if (null != frame) {
        frame->optionalData[0] = outputId;
        frame->optionalData[1] = isOn;

        Clsp_EnqueueFrame(frame,false);
    }
}

void Comm_OnDIPChanged(uint32 dipValue)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_DIP_CHANGED, 1, true);
    if (null != frame) {
        frame->optionalData[0] = dipValue;

        Clsp_EnqueueFrame(frame,false);
    }

#ifdef BOOTLOADER
//    if (System_CheckForceSetAddress())
//    {
//        Clsp485_SetForceLogicalAddress();
//        System_ClearForceSetAddress();
//    }
#endif
}

#ifndef BOOTLOADER
void Comm_OnDSMStateChanged(DSMSignals_e state, DSMSignalInfo_e info, DSMAccessGrantedSource_e agSource)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_DSM_CHANGED, 3, true);
    if (null != frame) {
        frame->optionalData[0] = state;
        frame->optionalData[1] = info;
        frame->optionalData[2] = agSource;

        Clsp_EnqueueFrame(frame, false);
    }
}

void Comm_OnOutputLogicStateChanged(uint32 outputID, uint32 active)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_OUTPUT_LOGIC_STATE, 2, true);
    if (null != frame) {
        frame->optionalData[0] = outputID;
        frame->optionalData[1] = active;

        Clsp_EnqueueFrame(frame,false);
    }
}

void Comm_OnCrOnlineStateChanged(CardReader_t* cr,bool isOnline)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_CR_ONLINE_STATE_CHANGED, 10, true);
    if (null != frame) {
        frame->optionalData[0] = cr->address;
        frame->optionalData[1] = isOnline;
        frame->optionalData[2] = cr->protocolVersionHigh;
        frame->optionalData[3] = cr->protocolVersionLow;
        frame->optionalData[4] = cr->firmwareVersionHigh;
        frame->optionalData[5] = cr->firmwareVersionLow;
        frame->optionalData[6] = cr->hardwareVersion;
        frame->optionalData[7] = CrProtocol_GetBaudRateEnum(UART_GetBaudRate(CR_PORT));
        frame->optionalData[8] = CrProtocol_GetBaudRateEnum(UART_GetBaudRate(CR_PORT));
        frame->optionalData[9] = cr->inUpgrade;

        Clsp_EnqueueFrame(frame, false);
    }
#ifdef DEBUG
    else
    	__ASM("NOP");
#endif
}

volatile uint32 _cardCount = 0;
void Comm_OnCrDataReceived(CardReader_t* cr,uint32 messageCode,byte* optionalData, uint32 optionalDataLength)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_CR_DATA_RECEIVED, optionalDataLength + 2, true);

    if (null != frame) {

        frame->optionalData[0] = cr->address;
        frame->optionalData[1] = messageCode;
        if (messageCode == 0xA0)
            _cardCount++;

        if (optionalDataLength > 0)
            CopyAll(optionalData,optionalDataLength,(byte*)(frame->optionalData+2),frame->optionalDataLength - 2);

        Clsp_EnqueueFrame(frame, false);
    }
#ifdef DEBUG
    else
    	__ASM("NOP");
#endif
}

void Comm_FireMemoryWarning(uint32_t allocated, uint32 seq)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_MEMORY_LOAD, 2, true);
    if (null != frame)
    {
        frame->optionalData[0] = seq;
        frame->optionalData[1] = allocated;
        Clsp_EnqueueFrame(frame, false);
    }
}

void Comm_FireMemoryRestore(uint32 allocated)
{
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_MEMORY_RESTORE, 1, true);
    if (null != frame)
    {
        frame->optionalData[0] = allocated;
        Clsp_EnqueueFrame(frame, false);
    }
}
#endif

#ifndef BOOTLOADER
void Comm_RegisterCallbacks(uint32 callbacks)
{

    if (callbacks & COMM_ON_INPUT_CHANGE)
        Inputs_BindOnInputChanged(Comm_OnInputChanged);

    if (callbacks & COMM_ON_SPECIAL_INPUT_CHANGE)
        Inputs_BindOnSpecialInputChanged(Comm_OnSpecialInputChanged);

    if (callbacks & COMM_ON_OUTPUT_CHANGE)
        Outputs_BindOnOutputChanged(Comm_OnOutputChanged);

    if (callbacks & COMM_ON_DIP_CHANGE)
        Inputs_BindOnDIPChanged(Comm_OnDIPChanged);

    if (callbacks & COMM_ON_DSM_CHANGE)
        DSM_BindOnStateChanged(Comm_OnDSMStateChanged);

    if (callbacks & COMM_ON_OUTPUT_LOGIC_CHANGE)
        Outputs_BindOnOutputLogicStateChanged(Comm_OnOutputLogicStateChanged);

    if (callbacks & COMM_ON_CR_ONLINE_STATE_CHANGE)
        CrCommunicator_BindOnlineStateChanged(Comm_OnCrOnlineStateChanged);

    if (callbacks & COMM_ON_CR_DATA_RECEIVED)
        CrCommunicator_BindRawDataReceived(Comm_OnCrDataReceived);
}
#endif

uint32 Comm_Get32BitValue(byte* data)
{
    return (data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
}

void Comm_Set32BitValue(byte* data, uint32 value)
{
    data[0] = value & 0x000000ff;
    data[1] = (value >> 8) & 0x000000ff;
    data[2] = (value >> 16) & 0x000000ff;
    data[3] = (value >> 24) & 0x000000ff;
}

uint16_t Comm_Get16BitValue(byte* data)
{
    return (data[0] | (data[1] << 8));
}

void Comm_Set16BitValue(byte* data, uint16_t value)
{
    data[0] = value & 0x00ff;
    data[1] = (value >> 8) & 0x00ff;
}

void Comm_ReportDeviceInfo(uint32 seq)
{
    ClspFrame_t* frame;

    frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_DEVICE_INFO, 4, true);
    if (frame != null)
    {
        frame->optionalData[0] = seq;
        frame->optionalData[1] = SysInfo_GetInputCount();
        frame->optionalData[2] = SysInfo_GetOutputCount();
        frame->optionalData[3] = SysInfo_GetCRPLevel();

        Clsp_EnqueueFrame(frame, false);
    }
}


void Comm_ReportFWVersion(uint32 seq)
{
    ClspFrame_t* frame;

    ProtocolId_t proto = PROTO_ACCESS;
    uint32 ml = 13;


#ifndef BOOTLOADER
    uint32 bootVersion;
    uint32 bootRevision;

    SysInfo_GetBootloaderVersion(&bootVersion,&bootRevision);

    if (bootVersion != 0 && bootRevision != 0)
        ml+=2;
#else
    proto = PROTO_UPLOADER;
#endif

    frame = Clsp_CreateFrame(proto, COMM_FW_VERSION, ml, true);

    if (frame != null)
    {
        frame->optionalData[0] = seq;

        Comm_Set32BitValue(frame->optionalData + 1, MAJOR_VERSION);
        Comm_Set32BitValue(frame->optionalData + 5, MINOR_VERSION);
        Comm_Set32BitValue(frame->optionalData + 9, BUILD_NUMBER);

#ifndef BOOTLOADER
        if (bootVersion != 0 && bootRevision != 0)
        {
            frame->optionalData[13] = bootVersion;
            frame->optionalData[14] = bootRevision;
        }
#endif

        Clsp_EnqueueFrame(frame,false);
    }
}


#ifdef BOOTLOADER
void Comm_ReportBaseAddress(uint32 seq) {
    ClspFrame_t* frame = Clsp_CreateFrame(PROTO_UPLOADER, COMM_BASE_ADDRESS, 3, true);
    if (frame != null)
    {
        frame->optionalData[0] = seq;
        Comm_Set16BitValue(frame->optionalData + 1, APPLICATION_BASE_ADDRESS);
        Clsp_EnqueueFrame(frame, false);
    }
}
#endif


volatile int32 _lastOutputSeq = -1;



#ifdef DEBUG
volatile uint32 debugOutCnt = 0;

void Comm_DebugCommand(uint32 cmd, byte* data, uint32 dataLength)
{
    char number[10];

    if (cmd > 0x20)
    {
        //UART_Debug("Cmd: ");
        //itoa(cmd, number);
        //UART_Debug(number);
        //UART_Debug("; ");
        //for(int i = 0; i < dataLength; i++)
        {
            itoaCustom(data[1], number, 10, true);
            //UART_Debug(CR_PORT,number);
            //UART_Debug(CR_PORT," ");
        }
        //UART_Debug(CR_PORT,"\r\n");
    }

    if (cmd == 60)
    {
        debugOutCnt++;
    }
}
#endif


void Comm_ProcessCommand(uint32 command, ProtocolId_t protocol, uint8* data, uint32 dataLength)
{
#ifndef BOOTLOADER
    uint32 tmp1, tmp2, tmp3, tmp4, tmp5;
#endif
    Result_e result;
    ClspFrame_t* frame;
#ifdef BOOTLOADER
    uint32 blockNumber;
    uint32 page;
#endif


    // debug frame
    //Comm_DebugCommand(command, data, dataLength);

    if (protocol == PROTO_ACCESS)
    {

        switch (command)
        {
#ifndef BOOTLOADER
            case COMM_SIGNAL_AG:
                result = DSM_SignalAccessGranted((DSMAGSourceCard_e)data[1]);
                Comm_FireResultNotifier(COMM_SIGNAL_AG, result, data[0]);
                break;

            case COMM_FORCE_UNLOCKED:
                // don't allow force unlock anymore
                //result = DSM_ForceUnlocked(data[1]);
                Comm_FireResultNotifier(COMM_FORCE_UNLOCKED, R_UnknownCommand, data[0]);
                break;

            case COMM_START_DSM:
                result = DSM_Start((DoorEnviromentType_e)data[1]);
                Comm_FireResultNotifier(COMM_START_DSM, result, data[0]);
                break;

            case COMM_STOP_DSM:
                result = DSM_Stop();
                Comm_FireResultNotifier(COMM_STOP_DSM, result, data[0]);
                break;

            case COMM_INVOKE_INTRUSION_ALARM:
                result = DSM_InvokeIntrusionAlarm(data[1]);
                Comm_FireResultNotifier(COMM_INVOKE_INTRUSION_ALARM, result, data[0]);
                break;

            case COMM_SET_PUSHBUTTON:
                tmp1 = Comm_Get32BitValue(&data[4]);
                tmp2 = Comm_Get32BitValue(&data[8]);

                result = DSM_SetPushButton(data[1] - 1,                             /* Source ID (1, ...), in DSM it is indexed from 0 thats why '-1' */
                                  data[2],                                          /* Input ID */
                                  (InputType_e)(data[3] & 0x01),                    /* Input Type (0 = digital, 1 = balanced */
                                  data[3] & 0x02,                                   /* Inverted */
                                  tmp1,                                             /* Delay to ON */
                                  tmp2);                                            /* Delay to OFF */

                Comm_FireResultNotifier(COMM_SET_PUSHBUTTON, result, data[0]);
                break;

            case COMM_UNSET_PUSHBUTTON:
                result = DSM_UnsetPushButton(data[1] - 1);                          /* Source ID (1, ...), in DSM it is indexed from 0 thats why '-1' */
                Comm_FireResultNotifier(COMM_UNSET_PUSHBUTTON, result, data[0]);
                break;

            case COMM_SET_SENSOR:
                if (dataLength >= 12)
                {
                    tmp1 = Comm_Get32BitValue(&data[4]);
                    tmp2 = Comm_Get32BitValue(&data[8]);

                    result = DSM_SetSensor((DSMSensors_e)data[1],                       /* Source Type */
                                  data[2],                                              /* Input ID */
                                  (InputType_e)(data[3] & 0x01),                        /* Input Type (0 = digital, 1 = balanced */
                                  data[3] & 0x02,                                       /* Inverted */
                                  tmp1,                                                 /* Delay to ON */
                                  tmp2);                                                /* Delay to OFF */
                }
                else
                    result = R_InvalidDataLength;

                Comm_FireResultNotifier(COMM_SET_SENSOR, result, data[0]);
                break;

            case COMM_UNSET_SENSOR:
                result = DSM_UnsetSensor((DSMSensors_e)data[1]);                    /* Source Type */
                Comm_FireResultNotifier(COMM_UNSET_SENSOR, result, data[0]);
                break;

            case COMM_SET_EL_STRIKE:
                tmp1 = Comm_Get32BitValue(&data[3]);
                tmp2 = Comm_Get32BitValue(&data[8]);
                tmp3 = Comm_Get32BitValue(&data[12]);
                result = DSM_SetElectricStrike((DSMActuators_e)data[1],             /* Actuator type */
                                      data[2],                                      /* Output ID */
                                      (DSMStrikeType_e)(data[7] & 0x01),            /* Strike type (level / impulse) */
                                      tmp1,                                         /* Pulse Time (only important if impulse type set */
                                      data[7] & 0x02,                               /* Inverted */
                                      tmp2,                                         /* Delay to ON */
                                      tmp3);                                        /* Delay to OFF */

                Comm_FireResultNotifier(COMM_SET_EL_STRIKE, result, data[0]);
                break;

            case COMM_SET_BYPAS_ALARM:
                result = DSM_SetBypassAlarm(data[1]);                               /* Output ID */
                Comm_FireResultNotifier(COMM_SET_BYPAS_ALARM, result, data[0]);
                break;

            case COMM_UNSET_ACTUATOR:
                result = DSM_UnsetActuator((DSMActuators_e)data[1]);                /* Actuator type */
                Comm_FireResultNotifier(COMM_UNSET_ACTUATOR, result, data[0]);
                break;

            case COMM_ENABLE_ALARMS:
                result = DSM_EnableAlarms((data[1] & 0x01) != 0,                    /* Door AJAR alarm */
                                         (data[1] & 0x02) != 0,                     /* Intrusion alarm */
                                         (data[1] & 0x04)!= 0);                     /* Sabotage alarm */

                Comm_FireResultNotifier(COMM_ENABLE_ALARMS, result, data[0]);
                break;

            case COMM_SET_TIMMINGS:
                tmp1 = Comm_Get32BitValue(&data[1]);
                tmp2 = Comm_Get32BitValue(&data[5]);
                tmp3 = Comm_Get32BitValue(&data[9]);
                tmp4 = Comm_Get32BitValue(&data[13]);
                tmp5 = Comm_Get32BitValue(&data[17]);

                result = DSM_SetTimming(tmp1,                                       /* Unlock time */
                                       tmp2,                                        /* Open time */
                                       tmp3,                                        /* Pre alarm time */
                                       tmp4,                                        /* Sirene Ajar Delay */
                                       tmp5);                                       /* Before intrusion delay */

                Comm_FireResultNotifier(COMM_SET_TIMMINGS, result, data[0]);
                break;

            case COMM_SET_SPECIAL_OUTPUT:
                result = DSM_SetSpecialOutput((DSMSpecialDOType_e)data[1],                   /* Special output type */
                                     data[2]);                                      /* Output ID */

                Comm_FireResultNotifier(COMM_SET_SPECIAL_OUTPUT, result, data[0]);
                break;

            case COMM_UNSET_SPECIAL_OUTPUT:
                result = DSM_UnsetSpecialOutput((DSMSpecialDOType_e)data[1]);    /* Special output type */

                Comm_FireResultNotifier(COMM_UNSET_SPECIAL_OUTPUT, result, data[0]);
                break;

            case COMM_UNSET_INPUT:
                result = DSM_UnsetInput(data[1]);                                   /* Input ID */
                Comm_FireResultNotifier(COMM_UNSET_INPUT, result, data[0]);
                break;

            case COMM_SET_CRS:                                                  /* Assign card readers to the DSM */
                //result = DSM_AssignCRs(data[1], data[2], data[3], data[4],
                //                       data[5], data[6], data[7], data[8]);
                result = DSM_AssignCRs(data);
                Comm_FireResultNotifier(COMM_SET_CRS, result, data[0]);
                break;

            case COMM_SUPPRESS_CR:
                result = DSM_SuppressCR(data[1]);
                Comm_FireResultNotifier(COMM_SUPPRESS_CR, result, data[0]);
                break;

            case COMM_LOOSE_CR:
                result = DSM_LooseCR(data[1]);
                Comm_FireResultNotifier(COMM_LOOSE_CR, result, data[0]);
                break;

            case COMM_SET_IMPLICIT_CODE:
                if (data[6] == 0)
                    result = DSM_SetImplicitCRCode(data[1], data[2], data[3], data[4],
                                                   0, 0, 0, null, data[5]);
                else
                    result = DSM_SetImplicitCRCode(data[1], data[2], data[3], data[4],
                                                   1, data[7], data[8], &data[9], data[5]);

                Comm_FireResultNotifier(COMM_SET_IMPLICIT_CODE, result, data[0]);
                break;

            case COMM_SET_BSI_LEVELS:
                if (dataLength >= 7)
                {
                    tmp1 = Comm_Get16BitValue(&data[1]);
                    tmp2 = Comm_Get16BitValue(&data[3]);
                    tmp3 = Comm_Get16BitValue(&data[5]);

                    result = DSM_SetBsiLevels(tmp1,                                     /* To Level 1 */
                                            tmp2,                                       /* To Level 2 */
                                            tmp3);                                      /* To Level 3 */
                }
                else
                    result = R_InvalidDataLength;

                Comm_FireResultNotifier(COMM_SET_BSI_LEVELS, result, data[0]);
                break;

            case COMM_SET_BSI_PARAMS:
                if (dataLength >= 18) // 18 is older , 19 is newe including invert
                {
                    tmp1 = Comm_Get32BitValue(&data[2]);
                    tmp2 = Comm_Get32BitValue(&data[6]);
                    tmp3 = Comm_Get32BitValue(&data[10]);
                    tmp4 = Comm_Get32BitValue(&data[14]);

                    tmp5 = 0;
                    if (dataLength >= 19) {
                        tmp5 = data[18];
                    }

                    result = DSM_SetBsiParameters(data[1],                              /* Input ID */
                                                 tmp1,                                  /* Filtertime */
                                                 tmp2,                                  /* Delay to ON */
                                                 tmp3,                                  /* Delay to OFF */
                                                 tmp4,                                  /* Tamper delay to ON */
                                                 tmp5                                   /* Inverted flag */
                                                 );
                }
                else
                    result = R_InvalidDataLength;

                Comm_FireResultNotifier(COMM_SET_BSI_PARAMS, result, data[0]);
                break;

            case COMM_REMAP_BSI:
                result = DSM_RemapBsiStates(data[1],                                /* Input ID */
                                   (InputState_e)data[2],                           /* State for level 0 */
                                   (InputState_e)data[3],                           /* State for level 1 */
                                   (InputState_e)data[4],                           /* State for level 2 */
                                   (InputState_e)data[5]);                          /* State for level 3 */

                Comm_FireResultNotifier(COMM_REMAP_BSI, result, data[0]);
                break;

            case COMM_SET_DI_PARAMS:
                if (dataLength >= 14) {

                    tmp1 = Comm_Get32BitValue(&data[2]);
                    tmp2 = Comm_Get32BitValue(&data[6]);
                    tmp3 = Comm_Get32BitValue(&data[10]);
                    tmp4 = 0;

                    if (dataLength >=15)
                        tmp4 = data[14];

                    result = DSM_SetDiParameters(data[1],                               /* Input ID */
                                                 tmp1,                                  /* Filtertime */
                                                 tmp2,                                  /* Delay to ON */
                                                 tmp3,                                  /* Delay to OFF */
                                                 tmp4
                                                 );
                }
                else
                    result = R_InvalidDataLength;

                Comm_FireResultNotifier(COMM_SET_DI_PARAMS, result, data[0]);

                break;

            case COMM_REMAP_DI:
                result = DSM_RemapDiStates(data[1],                                 /* Input ID */
                                              (InputState_e)data[2],                /* State for level 0 */
                                              (InputState_e)data[3]);               /* State for level 1 */

                Comm_FireResultNotifier(COMM_REMAP_DI, result, data[0]);
                break;

            case COMM_BIND_OUT_TO_IN:
                result = DSM_BindOutputToInput(data[1],                             /* Output ID */
                                                data[2]);                           /* Input ID */
                Comm_FireResultNotifier(COMM_BIND_OUT_TO_IN, result, data[0]);
                break;

            case COMM_UNBIND_OUT_TO_IN:
                result = DSM_UnbindOutputToInput(data[1],                           /* Output ID */
                                                data[2]);                           /* Input ID */

                Comm_FireResultNotifier(COMM_UNBIND_OUT_TO_IN, result, data[0]);
                break;


            case COMM_SET_REPORTED_OUTPUT:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_SetReportedOutputs(tmp1);

                Comm_FireResultNotifier(COMM_SET_REPORTED_OUTPUT, result, data[0]);
                break;

            case COMM_UNSET_REPORTED_OUTPUT:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_UnsetReportedOutputs(tmp1);

                Comm_FireResultNotifier(COMM_UNSET_REPORTED_OUTPUT, result, data[0]);
                break;

            case COMM_OUTPUT_LEVEL:
                tmp1 = Comm_Get32BitValue(&data[2]);
                tmp2 = Comm_Get32BitValue(&data[6]);

                result = DSM_ConfigLevel(data[1],                                   /* Output ID */
                                        tmp1,                                       /* Delay to ON */
                                        tmp2,                                       /* Delay to OFF */
                                        data[10] & 0x01);                            /* Inverted */

                Comm_FireResultNotifier(COMM_OUTPUT_LEVEL, result, data[0]);
                break;

            case COMM_OUTPUT_FREQUENCY:
                tmp1 = Comm_Get32BitValue(&data[2]);
                tmp2 = Comm_Get32BitValue(&data[6]);
                tmp3 = Comm_Get32BitValue(&data[10]);
                tmp4 = Comm_Get32BitValue(&data[14]);

                result = DSM_ConfigFrequency(data[1],                               /* Output ID */
                                        tmp1,                                       /* ON Time */
                                        tmp2,                                       /* OFF Time */
                                        tmp3,                                       /* Delay to ON */
                                        tmp4,                                       /* Delay to OFF */
                                        data[18] & 0x01,                            /* Forced OFF */
                                        (data[18] & 0x02) != 0);                    /* Inverted */

                Comm_FireResultNotifier(COMM_OUTPUT_FREQUENCY, result, data[0]);
                break;

            case COMM_OUTPUT_PULSE:
                tmp1 = Comm_Get32BitValue(&data[2]);
                tmp2 = Comm_Get32BitValue(&data[6]);
                tmp3 = Comm_Get32BitValue(&data[10]);

                result = DSM_ConfigPulse(data[1],                                   /* Output ID */
                                    tmp1,                                           /* Pulse Time */
                                    tmp2,                                           /* Delay to ON */
                                    tmp3,                                           /* Delay to OFF */
                                    data[14] & 0x01,                                /* Forced OFF */
                                    (data[14] & 0x02) != 0);                               /* Inverted */

                Comm_FireResultNotifier(COMM_OUTPUT_PULSE, result, data[0]);
                break;

            case COMM_ACTIVATE_OUTPUT:
                if (_lastOutputSeq == -1 || _lastOutputSeq != data[1])
                {
                    result = DSM_ActivateOutput(data[2],                        /* Output ID */
                                                data[3] & 0x01);                /* Is Activated */
                    _lastOutputSeq = data[1];

                    Comm_FireResultNotifier(COMM_ACTIVATE_OUTPUT, result, data[0]);
                }
                break;

            case COMM_FORCE_OUTPUT_OFF:
                result = Outputs_TurnOff(data[1]);
                Comm_FireResultNotifier(COMM_FORCE_OUTPUT_OFF, result, data[0]);
                break;

            case COMM_SET_BLOCKED_OUTPUT_EX:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_SetBlockedOutputsEx(tmp1);
                Comm_FireResultNotifier(COMM_SET_BLOCKED_OUTPUT_EX, result, data[0]);
                break;

            case COMM_SET_REPORTED_OUT_LOGIC:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_SetReportedOutputsLogic(tmp1);
                Comm_FireResultNotifier(COMM_SET_REPORTED_OUT_LOGIC, result, data[0]);
                break;

            case COMM_UNSET_REPORTED_OUT_LOGIC:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_UnsetReportedOutputsLogic(tmp1);
                Comm_FireResultNotifier(COMM_UNSET_REPORTED_OUT_LOGIC, result, data[0]);
                break;

            case COMM_SET_REPORTED_OUT_LOGIC_EX:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_SetReportedOutputsLogicEx(tmp1);
                Comm_FireResultNotifier(COMM_SET_REPORTED_OUT_LOGIC_EX, result, data[0]);
                break;

            case COMM_SET_REPORTED_OUTPUT_EX:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Outputs_SetReportedOutputsEx(tmp1);
                Comm_FireResultNotifier(COMM_SET_REPORTED_OUTPUT_EX, result, data[0]);
                break;

            case COMM_SET_REPORTED_INPUTS:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Inputs_SetReportedInputs(tmp1);
                Comm_FireResultNotifier(COMM_SET_REPORTED_INPUTS, result, data[0]);
                break;

            case COMM_UNSET_REPORTED_INPUTS:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Inputs_UnsetReportedInputs(tmp1);
                Comm_FireResultNotifier(COMM_UNSET_REPORTED_INPUTS, result, data[0]);
                break;

            case COMM_SET_REPORTED_INPUTS_EX:
                tmp1 = Comm_Get32BitValue(&data[1]);
                result = Inputs_SetReportedInputsEx(tmp1);
                Comm_FireResultNotifier(COMM_SET_REPORTED_INPUTS_EX, result, data[0]);
                break;

            case COMM_CR_REQUEST:
                {
                    CardReader_t* cr = CrCommunicator_GetCR(data[1]);
                    if (cr == null) {
                        Comm_FireResultNotifier(command, R_InvalidParameter, data[0]);
                    }
                    else {
                        if (!cr->isOnline) {
                            Comm_FireResultNotifier(command, R_CRIsOffline, data[0]);
                        }
                        else {
#define CLSP485_CR_HEADER_LENGTH 4

                            uint32 crODL =   dataLength - CLSP485_CR_HEADER_LENGTH;
                            byte crOptionalData[CR_MAX_OPTIONAL_DATA_LENGTH];
                            CopyAll((data + CLSP485_CR_HEADER_LENGTH), crODL , crOptionalData, CR_MAX_OPTIONAL_DATA_LENGTH);

                            if (CrCommunicator_ConceptAndEnqueue(cr->address, (CRMessageCode_t)data[2], crODL, crOptionalData, false, true))
                                Comm_FireResultNotifier(COMM_CR_REQUEST, R_Ok, data[0]);
                            else
                                Comm_FireResultNotifier(COMM_CR_REQUEST, R_Error, data[0]);
                        }
                    }
                }

                break;

            case COMM_SET_TIME:
                // on 0, there are sequence numbers
                result = SysTick_SetTime(data[1], data[2], data[3]);
                if (result == R_Ok) {
                    for(int i = 1; i <= CR_MAX_COUNT; i++)
                    {
                        CardReader_t* cr = CrCommunicator_GetCR(i);
                        if (cr != null && cr->isOnline)
                            if (!CR_SetTime(cr))
                            	__ASM("NOP");
                    }
                }

                Comm_FireResultNotifier(COMM_SET_TIME, result, data[0]);
                break;



            case COMM_START_IO_TEST:
                Test_StartIOTest(data[1]);
                Comm_FireResultNotifier(COMM_START_IO_TEST, R_Ok, data[0]);
                break;

            case COMM_SET_CRP_LEVEL:
                result = SysInfo_SetCRPLevel((SysInfo_CRPLevel_e)data[1]);
                Comm_FireResultNotifier(COMM_SET_CRP_LEVEL, result, data[0]);
                break;

#ifdef DEBUG_GENERATOR
            case COMM_TOGGLE_CR_GENERATOR:
                tmp1 = 3000;
                tmp2 = 5000;
                if (data[2] & (1 << 6))
                    tmp1 = data[3] * 1000;
                if (data[2] & (1 << 7))
                    tmp2 = data[4] * 1000;
                Test_ToggleCardGenerator(data[1] != 0, (Test_CardType_e)(data[2] & 0x3f), tmp1, tmp2);
                Comm_FireResultNotifier(COMM_TOGGLE_CR_GENERATOR, R_Ok, data[0]);
                break;

            case COMM_TOGGLE_ADC_GENERATOR:
                Test_ToggleADCGenerator(data[1] != 0, data[2] * 1000, data[3] * 1000);
                Comm_FireResultNotifier(COMM_TOGGLE_ADC_GENERATOR, R_Ok, data[0]);
                break;
#endif  // DEBUG_GENERATOR
#endif  // BOOTLOADER

            /* Read commands */
            case COMM_READ_DEVICE_INFO:
                Comm_ReportDeviceInfo(data[0]);
                break;

            case COMM_RESTART_DCU:
                SysInfo_DelayTask(SysInfo_RestartDCUDelayed, 400);
                Comm_FireResultNotifier(COMM_RESTART_DCU, R_Ok, data[0]);
                break;

            /*
            case COMM_READ_INPUT_CNT:
                Comm_ReportInputCount(data[0]);
                break;

            case COMM_READ_OUTPUT_CNT:
                Comm_ReportOutputCount(data[0]);
                break;
            */
            case COMM_READ_FW_VERSION:
                Comm_ReportFWVersion(data[0]);
                break;

#ifndef BOOTLOADER
            case COMM_REQUEST_UPGRADE:
                System_RequestUpgrade();
                frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_READY_FOR_UPGRADE, 1, false);
                if (frame != null)
                {
                    frame->optionalData[0] = data[0];
                    Clsp_EnqueueFrame(frame, false);
                }
                break;



            case COMM_READ_MEMORY_LOAD:
                Comm_FireMemoryWarning(MemoryLoad(), data[0]);
                break;

            case COMM_DEBUG_SEQ_REQUEST:
                frame = Clsp_CreateFrame(PROTO_ACCESS, COMM_DEBUG_SEQ_RESPONSE, 2, false);
                if (frame != null)
                {
                    frame->optionalData[0] = data[0];
                    frame->optionalData[1] = data[1];
                    Clsp_EnqueueFrame(frame,false);
                }
                break;

            case COMM_RESET_TO_BOOTLOADER:
                //System_RequestForceAddressSet();
                SysInfo_DelayTask(SysInfo_ResetToBootloaderDelayed, 200);
                Comm_FireResultNotifier(COMM_RESET_TO_BOOTLOADER, R_Ok, data[0]);
                //StartBootloader();
                break;
#endif
            default:
                Comm_FireResultNotifier(command,R_UnknownCommand,data[0]);
                break;


        }
    }

#ifdef BOOTLOADER
    if (protocol == PROTO_UPLOADER)
    {
        switch (command)
        {

            case COMM_READ_BASE_ADDRESS:
                Comm_ReportBaseAddress(data[0]);
                break;

            case COMM_RESET_TO_APPLICATION:
                //System_PrepareForApplication();
                SysInfo_DelayTask(SysInfo_ResetToApplicationDelayed, 500);
                Comm_FireResultNotifier(COMM_RESET_TO_APPLICATION, R_Ok, data[0]);
                break;

            case COMM_WRITE_DATA:
                blockNumber = Comm_Get16BitValue(data + 1);
                result = flash_WriteData(blockNumber, data + 4, data[3]);

                frame = Clsp_CreateFrame(PROTO_UPLOADER,
                    (result == 0 ? COMM_SEND_NEXT_DATA : COMM_RESEND_LAST_DATA),
                    (result == 0 ? 5 : 3), true);

                if (frame != null)
                {
                    frame->optionalData[0] = data[0];
                    Comm_Set16BitValue(frame->optionalData + 1, blockNumber);

                    if (result == 0)
                        Comm_Set16BitValue(frame->optionalData + 3, flash_getLastChecksum());

                    Clsp_EnqueueFrame(frame, false);
                }
                break;

            case COMM_ERASE_NEXT_PAGE:
                page = data[1];
                result = flash_ErasePage(data[1]);

                frame = Clsp_CreateFrame(PROTO_UPLOADER,
                    (result == 0 ? COMM_ERASE_OK : COMM_ERASE_ERROR), 2, true);

                if (frame != null)
                {
                    frame->optionalData[0] = data[0];
                    frame->optionalData[1] = page;

                    Clsp_EnqueueFrame(frame, false);
                }
                break;
            default:
                Comm_FireResultNotifier(command, R_UnknownCommand, data[0]);
                break;
        }
    }
#endif

}



void Comm_OnNodeAssigned(uint32 logicalAddress,bool isAssigned)
{
    if (isAssigned)
    {
#ifndef BOOTLOADER
        //Comm_ReportInputCount(0);
        //Comm_ReportOutputCount(0);
        Comm_ReportDeviceInfo(0);
#endif
        Comm_ReportFWVersion(0);
#ifndef BOOTLOADER
        Inputs_FireAllStatuses();
        CrCommunicator_FireOnlineEvents();
#else
        Comm_ReportBaseAddress(0);
#endif
    }
}


ClspFrame_t* Comm_OnFrameReceived(ClspFrame_t* masterFrame, ProtocolId_t protocol, bool* isTopPriority)
{
    Comm_ProcessCommand(masterFrame->messageParam, protocol, masterFrame->optionalData, masterFrame->optionalDataLength);

    return (ClspFrame_t*)null;   // no response back just yet
}

void Comm_Init() {

#ifndef BOOTLOADER
    Comm_RegisterCallbacks(
#ifndef DEBUG
                           0xFF
#else
                           COMM_ON_INPUT_CHANGE |
                           COMM_ON_SPECIAL_INPUT_CHANGE |
                           COMM_ON_OUTPUT_CHANGE |
                           COMM_ON_OUTPUT_LOGIC_CHANGE |
                           COMM_ON_DIP_CHANGE |
                           COMM_ON_DSM_CHANGE |
                           COMM_ON_CR_ONLINE_STATE_CHANGE |
                           COMM_ON_CR_DATA_RECEIVED
#endif
    );
#endif

    Clsp_BindOnNodeAssignmentChanged(Comm_OnNodeAssigned);

    Clsp_BindOnFrameReceived(Comm_OnFrameReceived);
}