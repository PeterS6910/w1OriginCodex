using System;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.Drivers.CardReader;
using Contal.Drivers.ClspDrivers;

using Contal.Cgp.NCAS.Definitions;

namespace Contal.Cgp.NCAS.NodeDataProtocol
{
    

    public class NodeCommandHelper
    {
        public static string GetCommandName(byte command)
        {
            return ((NodeCommand) command).ToString();
        }

        public static string GetFrameInfo(ClspFrame clspFrame)
        {
            switch ((NodeCommand)clspFrame.Command)
            {
                case NodeCommand.SetBSIParams:
                    return "Set BSI; Input " + clspFrame.OptionalData[0];
            }

            return null;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class NodeFrame
    {
        public const int MaxPushSourceID = 1;
        public const int MaxInputID = 16;
        public const int MaxOutputID = 12;
        public const int MaxBSILevel = 1023;

        internal class SequenceNumber
        {
            private const byte FirstSeqNumber = 0x10;
            private const byte LastSeqNumber = 0xE0;

            public static SequenceNumber Singleton = new SequenceNumber();
           
            private SequenceNumber()
            {
                _nextNumber = FirstSeqNumber;
            }

            private byte _nextNumber;
            public byte GetNextNumber()
            {
                _nextNumber++;
                if (_nextNumber >= LastSeqNumber)
                    _nextNumber = FirstSeqNumber;

                return _nextNumber;
            }
        }

        private static byte _outputSeq = 0;

        private static byte[] GetReportedIOData(int maxValue, params int[] IOs)
        {
            UInt32 reportedIO = 0;

            foreach (int io in IOs)
            {
                Validator.CheckIntegerRange(io, 0, maxValue);

                reportedIO |= (UInt32)(1 << io);
            }

            byte[] data = new byte[5];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            ByteConverter.ToBytes(data, 1, reportedIO);

            return data;
        }

        #region DSM Settings

        public static IClspFrame SignalAccessGranted(
            DsmAccessGrantedSeverity accessGrantedSeverity,
            DoorEnvironmentAccessTrigger accessTrigger)
        {
            byte[] data =
            {
                SequenceNumber.Singleton.GetNextNumber(),
                (byte)accessGrantedSeverity,
                (byte)accessTrigger
            };

            return ClspFrame.Create(
                ProtocolId.ProtoAccess,
                (byte)NodeCommand.SignalAccessGranted,
                data);
        }

        public static IClspFrame SignalAccessGranted(DsmAccessGrantedSeverity accessGrantedSeverity)
        {
            return SignalAccessGranted(
                accessGrantedSeverity,
                DoorEnvironmentAccessTrigger.None);
        }

        public static IClspFrame SignalAccessGranted()
        {
            return SignalAccessGranted(DsmAccessGrantedSeverity.NormalCard);
        }

        /*
        public static IClspFrame ForceUnlockedState(bool isUnlocked)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)((isUnlocked) ? 1 : 0) };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ForceUnlockedState, data);
        }*/

        public static IClspFrame StartDSM(DoorEnviromentType enviromentType)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)enviromentType };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.StartDSM, data);
        }

        public static IClspFrame StopDSM()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.StopDSM, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushbuttonType"></param>
        /// <param name="inputID"></param>
        /// <param name="balanced"></param>
        /// <param name="inverted"></param>
        /// <param name="delayToOn"></param>
        /// <param name="delayToOff"></param>
        /// <returns></returns>
        [Obsolete("Use SetPushButton without input characteristics overload")]
        public static IClspFrame SetPushButton(
            PushButtonType pushbuttonType, 
            byte inputID, 
            bool balanced, 
            bool inverted,
            UInt32 delayToOn, 
            UInt32 delayToOff)
        {
            Validator.CheckIntegerRange(inputID, 0, MaxInputID);

            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, balanced);
            BitArray.SetBit(misc, 1, inverted);

            byte[] data = new byte[12];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = (byte)pushbuttonType;
            data[2] = inputID;
            data[3] = BitArray.ToByte(misc);
            ByteConverter.ToBytes(data, 4, delayToOn, delayToOff);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetPushButton, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushbuttonType"></param>
        /// <param name="inputId"></param>
        /// <param name="bind">if true, tries to SetPushButton, if false, tries to UnsetPushButton</param>
        /// <returns></returns>
        public static IClspFrame SetPushButton(PushButtonType pushbuttonType, byte inputId, bool bind)
        {
            Validator.CheckIntegerRange(inputId, 0, MaxInputID);

            byte[] data =
            {
                SequenceNumber.Singleton.GetNextNumber(), 
                (byte)pushbuttonType,
                inputId,
                bind.ToByte()
            };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetPushButtonPure, data);
        }

        /// <summary>
        /// Create frame for unsetting push button
        /// </summary>
        /// <param name="pushbuttonType">Push button position</param>
        /// <returns></returns>
        public static IClspFrame UnsetPushButton(PushButtonType pushbuttonType)
        {
            return SetPushButton(pushbuttonType, 0, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="inputID"></param>
        /// <param name="balanced"></param>
        /// <param name="inverted"></param>
        /// <param name="delayToOn"></param>
        /// <param name="delayToOff"></param>
        /// <returns></returns>
        public static IClspFrame SetSensor(
            SensorType sensorType, 
            byte inputID, 
            bool balanced, 
            bool inverted, 
            UInt32 delayToOn, 
            UInt32 delayToOff)
        {
            Validator.CheckIntegerRange(inputID, 0, MaxInputID);

            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, balanced);
            BitArray.SetBit(misc, 1, inverted);

            byte[] data = new byte[12];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = (byte)sensorType;
            data[2] = inputID;
            data[3] = BitArray.ToByte(misc);
            ByteConverter.ToBytes(data, 4, delayToOn, delayToOff);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetSensor, data);
        }

        /// <summary>
        /// Create frame for unsetting sensor from input
        /// </summary>
        /// <param name="sensorType">Type of the sensor to be unset</param>
        /// <returns></returns>
        public static IClspFrame UnsetSensor(SensorType sensorType)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)sensorType };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetSensor, data);
        }

        public static IClspFrame SetActuator(ActuatorType actuator, byte outputID, StrikeType strikeType,
            UInt32 pulseTime, bool inverted, UInt32 delayToOn, UInt32 delayToOff)
        {
            byte[] data = new byte[16];
            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, strikeType == StrikeType.Impulse);
            BitArray.SetBit(misc, 1, inverted);

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = (byte)actuator;
            data[2] = outputID;
            ByteConverter.ToBytes(data, 3, pulseTime);
            data[7] = BitArray.ToByte(misc);
            ByteConverter.ToBytes(data, 8, delayToOn, delayToOff);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetElectricStrike, data);
        }

        public static IClspFrame SetBypassAlarm(byte outputID)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), outputID };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetBypassAlarm, data);
        }

        public static IClspFrame UnsetBypassAlarm()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), 4 };
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetActuator, data);
        }

        public static IClspFrame UnsetActuator(ActuatorType actuator)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)actuator };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetActuator, data);
        }

        public static IClspFrame SetTimmings(UInt32 unlockTime, UInt32 openTime, UInt32 preAlarmTime, UInt32 sireneAjarDelay,
            UInt32 beforeIntrusionDelay)
        {
            byte[] data = new byte[21];

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            ByteConverter.ToBytes(data, 1, unlockTime, openTime, preAlarmTime, sireneAjarDelay, beforeIntrusionDelay);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetTimmings, data);
        }

        public static IClspFrame EnableAlarms(bool doorAjarAlarm, bool intrusionAlarm, bool sabotageAlarm)
        {
            byte alarmFlags =
                (byte)
                (
                (doorAjarAlarm ? 1 : 0) |
                (intrusionAlarm ? 2 : 0) |
                (sabotageAlarm ? 4: 0));
            
            byte[] data =
            {
                SequenceNumber.Singleton.GetNextNumber(),
                alarmFlags
            };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.EnableAlarms, data);
        }

        public static IClspFrame SetSpecialOutput(SpecialOutputType outputType, byte outputID)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)outputType, outputID };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetSpecialOutput, data);
        }

        public static IClspFrame UnsetSpecialOutput(SpecialOutputType outputType)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)outputType };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetSpecialOutput, data);
        }

        #endregion // DSM

        #region Config Inputs

        public static IClspFrame UnsetInput(byte inputID)
        {
            Validator.CheckIntegerRange(inputID, 0, MaxInputID);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), inputID };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetInput, data);
        }

        public static IClspFrame SetBSILevels(UInt16 toLevel1, UInt16 toLevel2, UInt16 toLevel3)
        {
            Validator.CheckIntegerRange(toLevel1, 0, MaxBSILevel);
            Validator.CheckIntegerRange(toLevel2, 0, MaxBSILevel);
            Validator.CheckIntegerRange(toLevel3, 0, MaxBSILevel);
            if (toLevel1 >= toLevel2)
                throw new ArgumentException("toLevel1 is greater or equal to toLevel2");
            if (toLevel2 >= toLevel3)
                throw new ArgumentException("toLevel2 is greater or equal to toLevel3");

            byte[] data = new byte[7];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            ByteConverter.ToBytes(data, 1, toLevel1, toLevel2, toLevel3);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetBSILevels, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="filtertime"></param>
        /// <param name="delayToOn"></param>
        /// <param name="delayToOff"></param>
        /// <param name="delayToTamper"></param>
        /// <param name="inverted"></param>
        /// <returns></returns>
        public static IClspFrame SetBSIParams(byte inputID, UInt32 filtertime, UInt32 delayToOn, UInt32 delayToOff,
            UInt32 delayToTamper, bool inverted)
        {
            byte[] data = new byte[19];

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = inputID;
            ByteConverter.ToBytes(data, 2, filtertime, delayToOn, delayToOff, delayToTamper);
            data[18] = (byte)(inverted ? 1 : 0);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetBSIParams, data);
        }

        /// <summary>
        /// just a older overload of SetBSIParams without inverted specification
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="filtertime"></param>
        /// <param name="delayToOn"></param>
        /// <param name="delayToOff"></param>
        /// <param name="delayToTamper"></param>
        /// <returns></returns>
        public static IClspFrame SetBSIParams(byte inputID, UInt32 filtertime, UInt32 delayToOn, UInt32 delayToOff,
            UInt32 delayToTamper)
        {
            return SetBSIParams(inputID, filtertime, delayToOn, delayToOff, delayToTamper, false);
        }

        private static byte GetInputStateID(InputState state)
        {
            switch (state)
            {
                case InputState.Short:
                    return 2;
                case InputState.Normal:
                    return 0;
                case InputState.Alarm:
                    return 1;
                case InputState.Break:
                    return 3;
                default:
                    return 0xff;
            }
        }

        public static IClspFrame RemapBSI(
            byte inputID, 
            InputState state0, 
            InputState state1, 
            InputState state2,
            InputState state3)
        {
            byte[] data = new byte[6];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = inputID;
            data[2] = GetInputStateID(state0);
            data[3] = GetInputStateID(state1);
            data[4] = GetInputStateID(state2);
            data[5] = GetInputStateID(state3);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.RemapBSI, data);
        }

        /// <summary>
        /// for inverted flag application, DCU with firmware 1371 or newer is required
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="filtertime"></param>
        /// <param name="delayToOn"></param>
        /// <param name="delayToOff"></param>
        /// <param name="inverted"></param>
        /// <returns></returns>
        public static IClspFrame SetDIParams(byte inputID, UInt32 filtertime, UInt32 delayToOn, UInt32 delayToOff,bool inverted)
        {
            byte[] data = new byte[15];

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = inputID;
            ByteConverter.ToBytes(data, 2, filtertime, delayToOn, delayToOff);
            data[14] = (byte)(inverted ? 1 : 0);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetDIParams, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="filtertime"></param>
        /// <param name="delayToOn"></param>
        /// <param name="delayToOff"></param>
        /// <returns></returns>
        public static IClspFrame SetDIParams(byte inputID, UInt32 filtertime, UInt32 delayToOn, UInt32 delayToOff)
        {
            return SetDIParams(inputID, filtertime, delayToOn, delayToOff,false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="state0"></param>
        /// <param name="state1"></param>
        /// <returns></returns>
        public static IClspFrame RemapDI(byte inputID, InputState state0, InputState state1)
        {
            byte[] data = new byte[4];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = inputID;
            data[2] = GetInputStateID(state0);
            data[3] = GetInputStateID(state1);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.RemapDI, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="inputID"></param>
        /// <returns></returns>
        public static IClspFrame BindOutputToInput(byte outputID, byte inputID)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);
            Validator.CheckIntegerRange(inputID, 0, MaxInputID);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), outputID, inputID };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.BindOutputToInput, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputId"></param>
        /// <param name="suspend"></param>
        /// <returns></returns>
        public static IClspFrame OutputBindingSuspendResume(
            byte outputId, 
            bool suspend)
        {
            Validator.CheckIntegerRange(outputId, 0, MaxOutputID);
            
            byte[] data =
            {
                SequenceNumber.Singleton.GetNextNumber(), 
                outputId, 
                suspend.ToByte()
            };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.OutputBindingSuspendResume, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="inputID"></param>
        /// <returns></returns>
        public static IClspFrame UnbindOutputFromInput(byte outputID, byte inputID)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);
            Validator.CheckIntegerRange(inputID, 0, MaxInputID);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), outputID, inputID };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnbindOutputFromInput, data);
        }

        [Obsolete("All inputs reported automatically since DCU FW ...")]
        public static IClspFrame SetReportedInputs(params int[] inputs)
        {
            byte[] data = GetReportedIOData(MaxInputID, inputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetReportedInputs, data);
        }

        [Obsolete("All inputs reported automatically since DCU FW ...")]
        public static IClspFrame SetReportedInputsEx(params int[] inputs)
        {
            byte[] data = GetReportedIOData(MaxInputID, inputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetReportedInputsEx, data);
        }

        [Obsolete("All inputs reported automatically since DCU FW ...")]
        public static IClspFrame UnsetReportedInputs(params int[] inputs)
        {
            byte[] data = GetReportedIOData(MaxInputID, inputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetReportedInputs, data);
        }

        #endregion

        #region Config and control Outputs

        public static IClspFrame OutputConfigLevel(byte outputID, UInt32 delayToOn, UInt32 delayToOff, bool inverted)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);

            byte[] data = new byte[11];

            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, inverted);

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = outputID;
            ByteConverter.ToBytes(data, 2, delayToOn, delayToOff);
            data[10] = BitArray.ToByte(misc);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ConfigOutputLevel, data);
        }

        public static IClspFrame OutputConfigFrequency(byte outputID, UInt32 onTime, UInt32 offTime,
            UInt32 delayToOn, UInt32 delayToOff, bool forcedOff, bool inverted)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);

            byte[] data = new byte[19];
            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, forcedOff);
            BitArray.SetBit(misc, 1, inverted);

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = outputID;
            ByteConverter.ToBytes(data, 2, onTime, offTime, delayToOn, delayToOff);
            data[18] = BitArray.ToByte(misc);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ConfigOutputFrequency, data);
        }

        public static IClspFrame OutputConfigPulse(byte outputID, UInt32 pulseTime, UInt32 delayToOn,
            UInt32 delayToOff, bool forcedOff, bool inverted)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);

            byte[] data = new byte[15];
            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, forcedOff);
            BitArray.SetBit(misc, 1, inverted);

            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = outputID;
            ByteConverter.ToBytes(data, 2, pulseTime, delayToOn, delayToOff);
            data[14] = BitArray.ToByte(misc);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ConfigOutputPulse, data);
        }

        public static IClspFrame SetReportedOutputs(params int[] outputs)
        {
            byte[] data = GetReportedIOData(MaxOutputID, outputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetReportedOutput, data);
        }

        public static IClspFrame SetReportedOutputsEx(params int[] outputs)
        {
            byte[] data = GetReportedIOData(MaxOutputID, outputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetReportedOutputsEx, data);
        }

        public static IClspFrame UnsetReportedOutputs(params int[] outputs)
        {
            byte[] data = GetReportedIOData(MaxOutputID, outputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetReportedOutput, data);
        }

        /// <summary>
        /// Create frame for output activation / deactivation 
        /// </summary>
        /// <param name="outputID">ID of the output to be activated / deactivated</param>
        /// <param name="activate">Activate / deactivate</param>
        /// <returns></returns>
        public static IClspFrame SetOutput(byte outputID, bool activate)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);

            bool[] misc = new bool[8];
            BitArray.SetBit(misc, 0, activate);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), _outputSeq++, outputID, BitArray.ToByte(misc) };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ActivateOutput, data);
        }

        public static IClspFrame SetOutputTotalOff(byte outputID)
        {
            Validator.CheckIntegerRange(outputID, 0, MaxOutputID);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), outputID };
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ForceSwitchOff, data);
        }

        public static IClspFrame SetReportedOutputsLogic(params int[] outputs)
        {
            byte[] data = GetReportedIOData(MaxOutputID, outputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetReportedOutputsLogic, data);
        }

        public static IClspFrame SetReportedOutputsLogicEx(params int[] outputs)
        {
            byte[] data = GetReportedIOData(MaxOutputID, outputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetReportedOutputsLogicEx, data);
        }

        public static IClspFrame UnsetReportedOutputsLogic(params int[] outputs)
        {
            byte[] data = GetReportedIOData(MaxOutputID, outputs);
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.UnsetReportedOutputsLogic, data);
        }

        public static IClspFrame SetBlockedOutputsEx(params int[] outputs)
        {
            /* if outputs are null, then remove blocking from all outputs */
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), 0, 0, 0, 0 };
            
            if (outputs != null)
                data = GetReportedIOData(MaxOutputID, outputs);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetBlockedOutputsEx, data);
        }

        #endregion

        #region Read commands

        /// <summary>
        /// Create frame for reading input count 
        /// </summary>
        /// <returns></returns>
        public static IClspFrame ReadInputCount()
        {
            return ReadDeviceInfo();
        }

        /// <summary>
        /// Create frame for reading output count
        /// </summary>
        /// <returns></returns>
        public static IClspFrame ReadOutputCount()
        {
            return ReadDeviceInfo();
        }

        public static IClspFrame ReadDeviceInfo()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ReadDeviceInfo, data);
        }
        
        public static IClspFrame ReadFWVersion()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ReadFWVersion, data);
        }

        public static IClspFrame ReadMemoryLoad()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ReadMemoryLoad, data);
        }

        public static IClspFrame DebugSequenceRequest(byte seq)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), seq };
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.DebugSequenceRequest, data);
        }

        #endregion

        #region Upgrading

        public static IClspFrame ResetToBootloader()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ResetToBootloader, data);
        }

        public static IClspFrame ResetToApplication()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.ResetToApplication, data);
        }

        public static IClspFrame InitUpgrade()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.StartUpgrade, data);
        }

        public static IClspFrame WriteData(UInt16 block, byte[] upgData, int length)
        {
            byte[] data = new byte[length + 4];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            ByteConverter.ToBytes(data, 1, block);
            data[3] = (byte)length;

            for (int i = 0; i < length; i++)
                data[i + 4] = upgData[i];

            ClspFrame clspFrame = ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.WriteData, data);
            clspFrame.SpecificTimeout = 70;

            return clspFrame;
        }

        public static IClspFrame ErasePage(int page)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)page };
            ClspFrame clspFrame = ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.ErasePage, data);
            clspFrame.SpecificTimeout = 100;

            return clspFrame;
        }

        public static IClspFrame WriteChecksum(UInt32 checksum)
        {
            byte[] data = new byte[5];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            ByteConverter.ToBytes(data, 1, checksum);

            return ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.WriteChecksum, data);
        }

        public static IClspFrame WriteApplicationLength(UInt32 length)
        {
            byte[] data = new byte[5];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            ByteConverter.ToBytes(data, 1, length);

            return ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.WriteAppLength, data);
        }

        public static IClspFrame RequestUpgrade()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.RequestUpgrade, data);
        }

        public static IClspFrame ReadBaseAddress()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };
            return ClspFrame.Create(ProtocolId.ProtoUploader, (byte)NodeCommand.ReadBaseAddress, data);
        }

        #endregion

        #region Miscellanous
        public static IClspFrame SetTime(int hour, int minute, int second)
        {
            Validator.CheckIntegerRange(hour, 0, 23);
            Validator.CheckIntegerRange(minute, 0, 59);
            Validator.CheckIntegerRange(second, 0, 59);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)hour, (byte)minute, (byte)second };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetTime, data);
        }

        public static IClspFrame SetTime(DateTime dateTime)
        {
            return SetTime(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        public static IClspFrame RestartNode()
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber() };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.RestartNode, data);
        }

        public static IClspFrame StartIOTest(bool outputsInverted)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)(outputsInverted ? 1 : 0) };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.StartIOTest, data);
        }

        public static IClspFrame SetCRPLevel(CRPLevel level)
        {
            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)(level) };

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetCRPLevel, data);
        }

        public static IClspFrame ToggleCardGenerator(bool enabled)
        {
            return ToggleCardGenerator(enabled, GeneratedCardType.MifareCSN, -1, -1);
        }

        public static IClspFrame ToggleCardGenerator(bool enabled, GeneratedCardType cardType, int minTime, int maxTime)
        {
            if (maxTime > 0 && minTime > 0 && maxTime <= minTime)
                throw new ArgumentException("maxTime has to be greater than minTime");
            byte[] data = new byte[10];
            
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = (byte)(enabled ? 1 : 0);
            data[2] = (byte)(cardType == GeneratedCardType.MifareCSN ? 0 : 1);
            // mark that min | max time is present
            if (minTime >= 0)
                data[2] |= (1 << 6);
            if (maxTime >= 0)
                data[2] |= (1 << 7);

            data[3] = (byte)minTime;
            data[4] = (byte)maxTime;

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ToggleCardGenerator, data);
        }

        public static IClspFrame ToggleADCGenerator(bool enabled)
        {
            return ToggleADCGenerator(enabled, -1, -1);
        }

        public static IClspFrame ToggleADCGenerator(bool enabled, int minTime, int maxTime)
        {
            if (maxTime > 0 && minTime > 0 && maxTime <= minTime)
                throw new ArgumentException("maxTime has to be greater than minTime");

            byte[] data = new byte[10];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            data[1] = (byte)(enabled ? 1 : 0);
            data[2] = (byte)minTime;
            data[3] = (byte)maxTime;

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.ToggleADCGenerator, data);
        }
        #endregion

        #region DSM continued 

        /// <summary>
        /// Assign card readers to the DSM (so DSM can directly send commands to the CRs based 
        /// on the current DSM status
        /// </summary>
        /// <param name="cr1">Address of the card reader #1, 0 if CR is not used</param>
        /// <param name="implicitCode1">
        /// Implicit code that should be sent to CR in case DSM is in "Locked" state;
        /// ignored if cr1 is 0
        /// </param>
        /// <param name="optionalParam1">
        /// Optional data for the 'implicit code' or -1 if no extra data are used ; 
        /// ignored if cr1 is 0</param>
        /// <param name="intrusionOnlyViaLeds1"></param>
        /// <param name="cr2">Address of the card reader #2, 0 if CR is not used</param>
        /// <param name="implicitCode2">
        /// Implicit code that should be sent to CR in case DSM is in "Locked" state ; 
        /// ignored if cr2 is 0</param>
        /// <param name="optionalParam2">
        /// Optional data for the 'implicit code' or -1 if no extra data are used ; 
        /// ignored if cr2 is 0</param>
        /// <param name="followingCrMessage1"></param>
        /// <param name="followingCrMessage2"></param>
        /// <param name="intrusionOnlyViaLeds2"></param>
        /// <returns></returns>
        public static IClspFrame AssignCardReaders(
            int cr1, byte implicitCode1, byte[] optionalParam1, CRMessage followingCrMessage1, bool intrusionOnlyViaLeds1,
            int cr2, byte implicitCode2, byte[] optionalParam2, CRMessage followingCrMessage2, bool intrusionOnlyViaLeds2)
        {
            Validator.CheckIntegerRange(cr1, 0, 2);

            if (cr1 == 0)
            {
                implicitCode1 = 0x40; // just masking the problem, that code is validated even if the CR is not used
                optionalParam1 = null;
                followingCrMessage1 = null;
            }

            Validator.CheckIntegerRange(cr2, 0, 2);

            if (cr2 == 0)
            {
                implicitCode2 = 0x40; // just masking the problem, that code is validated even if the CR is not used
                                      // found in 1547-RS485 and 1594-LON
                optionalParam2 = null;
                followingCrMessage2 = null;
            }

            var data1 = CreateDateForImplicitCRCode(
                cr1,
                implicitCode1,
                optionalParam1,
                followingCrMessage1,
                intrusionOnlyViaLeds1);

            var data2 = CreateDateForImplicitCRCode(
                cr2,
                implicitCode2,
                optionalParam2,
                followingCrMessage2,
                intrusionOnlyViaLeds2);

            var data = new byte[1 + data1.Length + data2.Length];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            Array.Copy(data1, 0, data, 1, data1.Length);
            Array.Copy(data2, 0, data, 1 + data1.Length, data2.Length);

            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SetCRs, data);
        }

        public static IClspFrame SuppressCardReader(int cardReaderAddress)
        {
            Validator.CheckIntegerRange(cardReaderAddress, 1, 2);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)cardReaderAddress };
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.SuppresCR, data);
        }

        public static IClspFrame LooseCardReader(int cardReaderAddress)
        {
            Validator.CheckIntegerRange(cardReaderAddress, 1, 2);

            byte[] data = { SequenceNumber.Singleton.GetNextNumber(), (byte)cardReaderAddress };
            return ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.LooseCR, data);
        }

        /// <summary>
        /// sets the group of CCR protocol commands to be set to a specific reader, when it's parent DSM turns internal/non-special lock state
        /// </summary>
        /// <param name="cardReaderAddress">should be between 1 and 2 for CR usage on DCU</param>
        /// <param name="accessCommand">CRMessageCode from the range of access commands, therefore between 0x40 and 0x5f</param>
        /// <param name="accessCommandOptionalData">
        /// optional parameters for the previous access commands
        /// </param>
        /// <param name="followingCrMessage">optional CRMessage to be sent to CR after the previous access command; can be null</param>
        /// <param name="intrusionOnlyViaLed">if true, in case of intrusion over parent DSM, not whole visualisation is sent to CR, but only led/buzzer interpretation</param>
        /// <returns></returns>
        public static IClspFrame SetImplicitCRCode(int cardReaderAddress, byte accessCommand, byte[] accessCommandOptionalData, CRMessage followingCrMessage, bool intrusionOnlyViaLed)
        {
            Validator.CheckIntegerRange(cardReaderAddress, 1, 2);

            var dataForImplicitCRCode = CreateDateForImplicitCRCode(
                cardReaderAddress,
                accessCommand,
                accessCommandOptionalData,
                followingCrMessage,
                intrusionOnlyViaLed);

            var data = new byte[1 + dataForImplicitCRCode.Length];
            data[0] = SequenceNumber.Singleton.GetNextNumber();
            Array.Copy(dataForImplicitCRCode, 0, data, 1, dataForImplicitCRCode.Length);

            return ClspFrame.Create(
                ProtocolId.ProtoAccess,
                (byte) NodeCommand.SetImplicitCode,
                data);
        }

        private static byte[] CreateDateForImplicitCRCode(
            int cardReaderAddress,
            byte accessCommand,
            byte[] accessCommandOptionalData,
            CRMessage followingCrMessage,
            bool intrusionOnlyViaLed)
        {
            byte[] data;

            var accessCommandOptionalDataLength = accessCommandOptionalData != null
                ? accessCommandOptionalData.Length
                : 0;

            if (followingCrMessage != null)
            {
                int followingCrMessageOdl;
                var followingCrMessageOptionalData =
                    NodeCommunicator.GetCrMessageOptionalData(followingCrMessage, out followingCrMessageOdl);

                data = new byte[7 + accessCommandOptionalDataLength + followingCrMessageOdl];
                data[0] = (byte) cardReaderAddress;
                data[1] = accessCommand;
                data[2] = (byte) accessCommandOptionalDataLength;
                data[3] = (byte) (intrusionOnlyViaLed ? 1 : 0);
                data[4] = 1; // mark that there is cr message
                data[5] = (byte) followingCrMessage.MessageCode;
                data[6] = (byte) followingCrMessageOdl;

                if (accessCommandOptionalDataLength > 0)
                    Array.Copy(
                        accessCommandOptionalData,
                        0,
                        data,
                        7,
                        accessCommandOptionalDataLength);

                if (followingCrMessageOdl > 0)
                    Array.Copy(
                        followingCrMessageOptionalData,
                        0,
                        data,
                        7 + accessCommandOptionalDataLength,
                        followingCrMessageOdl);
            }
            else
            {
                data = new byte[5 + accessCommandOptionalDataLength];
                data[0] = (byte) cardReaderAddress;
                data[1] = accessCommand;
                data[2] = (byte) accessCommandOptionalDataLength;
                data[3] = (byte) (intrusionOnlyViaLed ? 1 : 0);
                data[4] = 0; // mark that there is no cr message

                if (accessCommandOptionalDataLength > 0)
                    Array.Copy(
                        accessCommandOptionalData,
                        0,
                        data,
                        5,
                        accessCommandOptionalDataLength);
            }

            return data;
        }

        #endregion
    }
}
