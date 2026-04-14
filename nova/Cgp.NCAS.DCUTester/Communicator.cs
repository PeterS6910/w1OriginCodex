using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using Contal.IwQuick.Net;
using Contal.Drivers.ClspDriversPC;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.Cgp.NCAS.NodeDataProtocolPC;
using Contal.Drivers.LPC3250;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.CardReaderPC;

namespace Contal.Cgp.NCAS.DCUTester
{
    public enum MessageCode
    {
        DeviceInfo,
        InputCount,
        OutputCount,
        CRPLevel,
        FWVersion,
        InputChanged,
        SpecialInputChanged,
        OutputChanged,
        LogicOutputChanged,
        DIPChanged,
        DSMChanged,
        ProtocolChanged,
        Error,
        UpgradeProgreessUpdate,
        UpgradeError,
        CROnlineStateChanged,
        MemoryLoad
    }

    

    public delegate void DProcessData(MessageCode code, object value);
    public delegate void DNodeListRefresh(byte[] nodeList);

    public class DeviceInfo
    {
        public DeviceInfo(int inputCount, int outputCount)
        {
            InputCount = inputCount;
            OutputCount = outputCount;
        }

        public int InputCount = 0;
        public int OutputCount = 0;
    }

    public class InputChangeInfo
    {
        byte _inputID;
        public byte InputID
        {
            get { return _inputID; }
            set { _inputID = value; }
        }

        InputState _inputState;
        public InputState InputState
        {
            get { return _inputState; }
            set { _inputState = value; }
        }

        public InputChangeInfo(byte inputID, InputState inputState)
        {
            _inputID = inputID;
            _inputState = inputState;
        }
    }

    public class SpecialInputChangedInfo
    {
        SpecialInput _inputType;
        public SpecialInput InputType
        {
            get { return _inputType; }
            set { _inputType = value; }
        }

        private InputState _inputState;
        public InputState InputState
        {
            get { return _inputState; }
            set { _inputState = value; }
        }

        public SpecialInputChangedInfo(SpecialInput inputType, InputState state)
        {
            _inputType = inputType;
            _inputState = state;
        }
    }

    public class OutputChangeInfo
    {
        byte _outputID;
        public byte OutputID
        {
            get { return _outputID; }
            set { _outputID = value; }
        }

        bool _isOn;
        public bool IsOn
        {
            get { return _isOn; }
            set { _isOn = value; }
        }

        public OutputChangeInfo(byte outputID, bool isOn)
        {
            _outputID = outputID;
            _isOn = isOn;
        }
    }

    public class DSMChangedInfo
    {
        private DoorEnvironmentState _state;
        public DoorEnvironmentState State
        {
            get { return _state; }
            set { _state = value; }
        }

        private DoorEnvironmentStateDetail _detail;
        public DoorEnvironmentStateDetail Detail
        {
            get { return _detail; }
            set { _detail = value; }
        }

        public DSMChangedInfo(DoorEnvironmentState state, DoorEnvironmentStateDetail detail)
        {
            _state = state;
            _detail = detail;
        }
    }

    public class CRInfo
    {
        public byte _address;
        public bool _isOnline;

        public CRInfo(byte address, bool isOnline)
        {
            _address = address;
            _isOnline = isOnline;
        }
    }

    public class Communicator
    {
        public Communicator()
        {
            byte[] key = { 0xa0, 0xd3, 0x91, 0x8d, 0x08, 0x18, 0xcc, 0xcd, 0x12, 0x2f, 0x45, 0x1e, 0x2c, 0xa8, 0x27, 0x8d };
            byte[] iv = { 0x1e, 0x6f, 0x95, 0x41, 0x7c, 0x06, 0x4d, 0x31, 0x52, 0xbb, 0x8b, 0xd0, 0xa1, 0x5e, 0x52, 0x35 };
            for (int i = 0; i < 16; i++) iv[i] = 0;
            AESSettings aesSettings = new AESSettings(key, iv, AESKeySize.Size128);
            _nodeComm = new NodeCommunicator(aesSettings);
            _nodeComm.IsEncrypted = false;
            //_nodeComm.IsEncrypted = true;
            _nodeComm.NodeResponseTimeout = 50;
            _nodeComm.DelayBetweenSuccessfulPolling = 20;
            _nodeComm.NodeUnpolledTimeout = 10;
            _nodeComm.AsyncTimeout = 10000;
            _nodeComm.MaxNodeLookupSequence = 3;

            _nodeComm.NodeAssigned += new DType2Void<SlaveNode>(OnNodeAssigned);
            _nodeComm.NodeRenewed += new DType2Void<SlaveNode>(OnNodeRenewed);
            _nodeComm.NodeReleased += new DType2Void<SlaveNode>(OnNodeReleased);
            _nodeComm.NodeAddressConflicted += new D2Type2Void<string, SlaveNode>(OnNodeAddressConflict);
            //_nodeComm.FrameReceived += new D2Type2Void<SlaveNode, Frame>(OnFrameReceived);

            _nodeComm.DSMChanged +=new D4Type2Void<SlaveNode,DoorEnvironmentState,DoorEnvironmentStateDetail,DoorEnvironmentAGSource>(_nodeComm_DSMChanged);
            _nodeComm.ReportInputCount += new D2Type2Void<SlaveNode, uint>(_nodeComm_ReportInputCount);
            _nodeComm.ReportOutputCount += new D2Type2Void<SlaveNode, uint>(_nodeComm_ReportOutputCount);
            _nodeComm.ReportCRPLevel += new D2Type2Void<SlaveNode, CRPLevel>(_nodeComm_ReportCRPLevel);
            _nodeComm.ReportDeviceInfo += new D3Type2Void<SlaveNode, uint, uint>(_nodeComm_ReportDeviceInfo);
            _nodeComm.ReportFWVersion += new D2Type2Void<SlaveNode, ExtendedVersion>(_nodeComm_ReportFWVersion);
            _nodeComm.UnknownFrame += new D2Type2Void<SlaveNode, Frame>(_nodeComm_UnknownFrame);
            _nodeComm.InputChanged += new D3Type2Void<SlaveNode, byte, InputState>(_nodeComm_InputChanged);
            _nodeComm.SpecialInputChanged += new D3Type2Void<SlaveNode, SpecialInput, InputState>(_nodeComm_SpecialInputChanged);
            _nodeComm.OutputChanged += new D3Type2Void<SlaveNode, byte, bool>(_nodeComm_OutputChanged);
            _nodeComm.OutputLogicChanged += new D3Type2Void<SlaveNode, byte, bool>(_nodeComm_OutputLogicChanged);
            _nodeComm.ModeChanged += new D2Type2Void<SlaveNode, ProtocolId>(_nodeComm_ModeChanged);
            _nodeComm.UpgradeProgress += new D2Type2Void<SlaveNode, int>(_nodeComm_UpgradeProgress);
            _nodeComm.UpgradeError += new D3Type2Void<SlaveNode, UpgradeErrors, string>(_nodeComm_UpgradeError);
            _nodeComm.CommandNACK += new D3Type2Void<SlaveNode, NodeErrorCodes, Frame>(_nodeComm_CommandNACK);
            _nodeComm.CommandACK += new D2Type2Void<SlaveNode, byte>(_nodeComm_CommandACK);
            _nodeComm.CROnlineStateChanged += new D3Type2Void<SlaveNode, CardReader, bool>(_nodeComm_CROnlineStateChanged);
            _nodeComm.CRCardSwiped += new D4Type2Void<SlaveNode, CardReader, string,int>(_nodeComm_CRCardSwiped);
            _nodeComm.CRCodeSpecified += new D3Type2Void<SlaveNode, CardReader, string>(_nodeComm_CRCodeSpecified);
            _nodeComm.IOTestProgress += new D2Type2Void<SlaveNode, int>(_nodeComm_IOTestProgress);
            _nodeComm.IOTestTimeout += new D2Type2Void<SlaveNode, int>(_nodeComm_IOTestTimeout);
            _nodeComm.IOTestOk += new DType2Void<SlaveNode>(_nodeComm_IOTestOk);
            _nodeComm.MemoryWarning += new D2Type2Void<SlaveNode,int>(_nodeComm_MemoryWarning);


            _nodeComm.DebugSequenceResponse += new DType2Void<byte>(_nodeComm_DebugSequenceResponse);

            _nodeComm.CommandTimeout += new D2Type2Void<SlaveNode, Frame>(_nodeComm_CommandTimeout);
        }

        void _nodeComm_ReportCRPLevel(SlaveNode parameter1, CRPLevel level)
        {
            FireProcessData(MessageCode.CRPLevel, level);
        }

        void _nodeComm_ReportOutputCount(SlaveNode parameter1, uint outputCount)
        {
            _outputCount = (int)outputCount;
            FireProcessData(MessageCode.OutputCount, _outputCount);
        }

        void _nodeComm_ReportInputCount(SlaveNode parameter1, uint inputCount)
        {
            _inputCount = (int)inputCount;
            FireProcessData(MessageCode.InputCount, _inputCount);
        }

        void _nodeComm_ReportDeviceInfo(SlaveNode parameter1, uint inputCount, uint outputCount)
        {
            _inputCount = (int)inputCount;
            _outputCount = (int)outputCount;

            FireProcessData(MessageCode.DeviceInfo, new DeviceInfo((int)inputCount, (int)outputCount));
        }

        NodeCommunicator _nodeComm;
        public NodeCommunicator NodeCommunicator
        {
            get { return _nodeComm; }
        }

        private SafeThread _mainThread = null;
        private string _portName = string.Empty;
        private int _selectedNode = -1;
        private ProtocolId _commMode;

        private int _outputCount = 4;
        private int _inputCount = 4;
        private bool _raiseRTS = false;

        public void Start(string portName, bool raiseRTS)
        {
            if (_mainThread != null)
                Stop();

            _raiseRTS = raiseRTS;

            _portName = portName;

            _mainThread = SafeThread.StartThread(MainThread);
        }

        public void Stop()
        {
            if (_nodeComm != null)
            {
                try { _nodeComm.Stop(); }
                catch { }

                //_nodeComm = null;
            }

            if (_mainThread != null)
            {
                try { _mainThread.Stop(400); }
                catch { }

                _mainThread = null;
            }
        }

        public string GetNodePhysicalAddress(byte logicalAddress)
        {
            SlaveNode node = _nodeComm.GetNode(logicalAddress);
            
            if (node == null)
                return null;

            return node.PhysicalAddress;
        }

        void MainThread()
        {
            _nodeComm.NodeResponseTimeout = 30;
            _nodeComm.PHDParsingExecutionTolerance = 150;
            _nodeComm.NodeResponseLatency = 4;
            _nodeComm.NodeUnpolledTimeout = 5;
            _nodeComm.MaxNodeLookupSequence = 1;
            _nodeComm.DelayBetweenSuccessfulPolling = 10;
            _nodeComm.StackingFramesEnabled = false;
            //_nodeComm.
            //SlaveNode.GlobalMaxRetryCount = 
            _nodeComm.AsyncTimeout = 3000;

            _nodeComm.Start(_portName);
            _nodeComm.RaiseRtsOnTransmission = _raiseRTS;

            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne();
        }

        

        void _nodeComm_MemoryWarning(SlaveNode slaveNode, int memoryLoad)
        {
            System.Diagnostics.Debug.WriteLine("Memory warning: " + memoryLoad + "% allocated");
            FireProcessData(MessageCode.MemoryLoad, memoryLoad);
        }

        void _nodeComm_UpgradeError(SlaveNode node, UpgradeErrors errorSource, string info)
        {
            //FireProcessData(MessageCode.UpgradeError, parameter2);
            System.Diagnostics.Debug.WriteLine("Upgrade error: " + errorSource + "; Info: " + info);
        }

        void _nodeComm_IOTestOk(SlaveNode node)
        {
            FireIOTestPassed();
        }

        void _nodeComm_IOTestTimeout(SlaveNode node, int outputID)
        {
            FireIOTestFailed(outputID);
        }

        void _nodeComm_IOTestProgress(SlaveNode node, int parameter)
        {
            FireIOTestProgress(parameter);
        }

        void _nodeComm_ReportFWVersion(SlaveNode node, ExtendedVersion version)
        {
            FireProcessData(MessageCode.FWVersion, version);
        }

        void _nodeComm_CommandTimeout(SlaveNode parameter1, Frame parameter2)
        {
            System.Diagnostics.Debug.WriteLine("Command timeout : " + Command.GetCommandName(parameter2.Command));
            if (parameter2.Command == Command.DebugSequenceRequest)
            {
                _debugSeqTimeout = true;
                _debugSeqEvent.Set();
            }
        }

        void _nodeComm_CRCodeSpecified(SlaveNode parameter1, CardReader parameter2, string parameter3)
        {
            if (parameter3 == "1234")
                _nodeComm.GetCrAccessCommands(parameter1).Accepted(parameter2);
            else
                _nodeComm.GetCrAccessCommands(parameter1).Rejected(parameter2);
        }

        //bool _cardSwipedToggle = true;
        void _nodeComm_CRCardSwiped(SlaveNode parameter1, CardReader parameter2, string parameter3,int cardSystem)
        {
            _nodeComm.GetCrAccessCommands(parameter1).WaitingForPIN(parameter2);
            /*if (_cardSwipedToggle)
                _nodeComm.GetCrAccessCommands(parameter1).Accepted(parameter2);
            else
                _nodeComm.GetCrAccessCommands(parameter1).Rejected(parameter2);

            _cardSwipedToggle = !_cardSwipedToggle;*/
        }


        private CardReader[] _currentCardReaders = null;
        public CardReader[] CurrentCardReaders
        {
            get
            {
                return _currentCardReaders;
            }
        }

        void _nodeComm_CROnlineStateChanged(SlaveNode node, CardReader parameter2, bool parameter3)
        {
            if (_selectedNode != node.LogicalAddress)
            {
                _currentCardReaders = null;
                return;
            }
            else
                _currentCardReaders = _nodeComm.GetCardReaders(node);
            
            FireProcessData(MessageCode.CROnlineStateChanged, parameter2);
        }

        void _nodeComm_CommandACK(SlaveNode parameter1, byte parameter2)
        {
            //System.Diagnostics.Debug.WriteLine("Command ACK: " + string.Format("0x{0:X}", parameter2));
        }

        void _nodeComm_CommandNACK(SlaveNode parameter1, NodeErrorCodes parameter2, Frame frame)
        {
            System.Diagnostics.Debug.WriteLine("!!! COMMAND NACK: " + parameter2.ToString() + 
                "; Command: " + string.Format("0x{0:X}", frame.Command));
        }

        void _nodeComm_DSMChanged(SlaveNode parameter1, DoorEnvironmentState parameter2, DoorEnvironmentStateDetail parameter3,DoorEnvironmentAGSource agSource)
        {
            //System.Diagnostics.Debug.WriteLine("DSM changed, state: " + parameter2 + "; detail: " + parameter3);
            FireProcessData(MessageCode.DSMChanged, new DSMChangedInfo(parameter2, parameter3));
        }

        void _nodeComm_OutputLogicChanged(SlaveNode parameter1, byte parameter2, bool parameter3)
        {
            FireProcessData(MessageCode.LogicOutputChanged, new OutputChangeInfo(parameter2, parameter3));
            //System.Diagnostics.Debug.WriteLine("Logic state changed, output: " + parameter2 + " to " + parameter3);
        }

        void _nodeComm_OutputChanged(SlaveNode parameter1, byte parameter2, bool parameter3)
        {
            FireProcessData(MessageCode.OutputChanged, new OutputChangeInfo(parameter2, parameter3));
        }

        void _nodeComm_UpgradeProgress(SlaveNode node, int parameter)
        {
            FireProcessData(MessageCode.UpgradeProgreessUpdate, parameter);
        }

        void _nodeComm_ModeChanged(SlaveNode node, ProtocolId param)
        {
            _commMode = param;
            FireProcessData(MessageCode.ProtocolChanged, param);
        }
        
        public byte SelectedNode
        {
            get { return (byte)_selectedNode; }
            set 
            {
                if (_selectedNode != value)
                {
                    _selectedNode = value;
                    SlaveNode node = _nodeComm.GetNode((byte)_selectedNode);
                    if (node != null)
                    {
                        FireProcessData(MessageCode.ProtocolChanged, node.MasterProtocol);
                    }
                }
            }
        }
        
        /*
        void _nodeComm_ReportOutputCount(SlaveNode node, uint param)
        {
            FireProcessData(MessageCode.OutputCount, param);
            //System.Diagnostics.Debug.WriteLine("Input count: " + param);
            _outputCount = (int)param;
        }

        void _nodeComm_ReportInputCount(SlaveNode node, uint param)
        {
            FireProcessData(MessageCode.InputCount, param);
            //System.Diagnostics.Debug.WriteLine("Output count: " + param);
            _inputCount = (int)param;
        }
        */

        void _nodeComm_InputChanged(SlaveNode node, byte inputID, InputState state)
        {
            FireProcessData(MessageCode.InputChanged, new InputChangeInfo(inputID, state));
            //System.Diagnostics.Debug.WriteLine("Input " + inputID + " to " + state);
        }

        void _nodeComm_SpecialInputChanged(SlaveNode node, SpecialInput inputID, InputState state)
        {
            FireProcessData(MessageCode.SpecialInputChanged, new SpecialInputChangedInfo(inputID, state));
        }

        void _nodeComm_UnknownFrame(SlaveNode node, Frame param)
        {

        }

        #region Events

        public event DNodeListRefresh NodeListRefresh;

        private void FireRefreshNodeList(byte[] nodeList)
        {
            try
            {
                if (NodeListRefresh != null)
                    NodeListRefresh(nodeList);
            }
            catch
            {
            }
        }

        public event D2Type2Void<MessageCode, object> ProcessData;

        private void FireProcessData(MessageCode code, object value)
        {
            try
            {
                if (ProcessData != null)
                    ProcessData(code, value);
            }
            catch
            {
            }
        }

        #endregion // Events

        byte[] BuildNodeList()
        {
            byte[] nodeList = new byte[_nodeComm.NodeCount];
            int i = 0;
            foreach (SlaveNode node in _nodeComm)
                nodeList[i++] = node.LogicalAddress;

            return nodeList;
        }

        #region CLSP485 Event handlers 

        void ReportNodes()
        {
            int i = 1;
            foreach (SlaveNode sn in _nodeComm)
            {
                System.Diagnostics.Debug.WriteLine("DEBUG "+i + ". ADDR=" + sn.LogicalAddress + " > " + sn.PhysicalAddress);
                Console.WriteLine((i++) + ". ADDR=" + sn.LogicalAddress + " > " + sn.PhysicalAddress);
            }
        }

        void OnNodeAddressConflict(string param1, SlaveNode conflictingNode)
        {
            System.Diagnostics.Debug.WriteLine("WARNING " + DateTime.Now + " Node uid=" + param1 + " IN CONFLICT WITH addr=" +
                conflictingNode.LogicalAddress + " uid=" + conflictingNode.PhysicalAddress);
        }

        void OnNodeReleased(SlaveNode node)
        {
            System.Diagnostics.Debug.WriteLine("WARNING " + DateTime.Now.ToString("hh:mm:ss.fff") + " Node RELEASED addr=" + node.LogicalAddress + 
                " uid=" + node.PhysicalAddress);

            ReportNodes();

            FireRefreshNodeList(BuildNodeList());

            if (_selectedNode == node.LogicalAddress)
                _selectedNode = -1;
        }

        void OnNodeRenewed(SlaveNode node)
        {
            System.Diagnostics.Debug.WriteLine("WARNING " + DateTime.Now + " Node RENEWED addr=" + node.LogicalAddress + 
                " uid=" + node.PhysicalAddress);

            FireRefreshNodeList(BuildNodeList());
        }

        void OnNodeAssigned(SlaveNode node)
        {
            System.Diagnostics.Debug.WriteLine("WARNING " + DateTime.Now + " Node ASSIGNED addr=" + node.LogicalAddress + 
                " uid=" + node.PhysicalAddress); 
            
            ReportNodes();

            FireRefreshNodeList(BuildNodeList());
        }

        #endregion  // CLSP485 event handlers

        #region Command sending

        private void SendFrameAllNodes(Frame frame)
        {
            foreach (SlaveNode node in _nodeComm)
            {
                _nodeComm.EnqueueFrame(node.LogicalAddress, frame);
            }
        }

        private void Send(Frame frame)
        {
            _nodeComm.EnqueueFrame((byte)_selectedNode, frame);
        }

        #region DSM
        public void StartDSM(DoorEnviromentType doorEnviromentType)
        {
            Send(NodeFrame.StartDSM(doorEnviromentType));
        }

        public void StopDSM()
        {
            Send(NodeFrame.StopDSM());
        }

        public void SignalAccessGranted()
        {
            Send(NodeFrame.SignalAccessGranted());
        }

        public void SetPushButton(PushButtonType pushbuttonType, byte inputID, bool balanced, bool inverted, 
            UInt32 delayToOn, UInt32 delayToOff)
        {
            Frame frame = NodeFrame.SetPushButton(pushbuttonType, inputID, balanced, inverted, delayToOn, delayToOff);
            Send(frame);
        }

        public void UnsetPushButton(PushButtonType pushbuttonType)
        {
            Frame frame = NodeFrame.UnsetPushButton(pushbuttonType);
            Send(frame);
        }

        public void SetSensor(SensorType sensorType, byte inputID, bool balanced, bool inverted,
            UInt32 delayToOn, UInt32 delayToOff)
        {
            Frame frame = NodeFrame.SetSensor(sensorType, inputID, balanced, inverted, delayToOn, delayToOff);
            Send(frame);
        }

        public void UnsetSensor(SensorType sensorType)
        {
            Frame frame = NodeFrame.UnsetSensor(sensorType);
            Send(frame);
        }

        public void SetTimmings(UInt32 unlockTime, UInt32 openTime, UInt32 preAlarmTime, UInt32 doorArrayDelay,
            UInt32 beforeIntrusionDelay)
        {
            Frame frame = NodeFrame.SetTimmings(unlockTime, openTime, preAlarmTime, doorArrayDelay, beforeIntrusionDelay);
            Send(frame);
        }

        public void EnableAlarms(bool doorAjarAlarm, bool intrusionAlarm, bool sabotageAlarm)
        {
            Frame frame = NodeFrame.EnableAlarms(doorAjarAlarm, intrusionAlarm, sabotageAlarm);
            Send(frame);
        }

        public void SetSpecialOutput(SpecialOutputType outputType, byte outputID)
        {
            Frame frame = NodeFrame.SetSpecialOutput(outputType, outputID);
            Send(frame);
        }

        public void UnsetSpecialOutput(SpecialOutputType outputType)
        {
            Frame frame = NodeFrame.UnsetSpecialOutput(outputType);
            Send(frame);
        }

        public void SetElectricStrike(ActuatorType actuator, byte outputID, StrikeType strikeType,
            UInt32 pulseTime, bool inverted, UInt32 delayToOn, UInt32 delayToOff)
        {
            Frame frame = NodeFrame.SetActuator(actuator, outputID, strikeType, pulseTime, inverted, 
                delayToOn, delayToOff);
            Send(frame);
        }

        public void SetBypassAlarm(byte outputID)
        {
            Frame frame = NodeFrame.SetBypassAlarm(outputID);
            Send(frame);
        }

        public void UnsetBypassAlarm()
        {
            Send(NodeFrame.UnsetBypassAlarm());
        }

        public void UnsetActuator(ActuatorType actuator)
        {
            Frame frame = NodeFrame.UnsetActuator(actuator);
            Send(frame);
        }

        public void ForceUnlockedState(bool isUnlocked)
        {
            Send(NodeFrame.ForceUnlockedState(isUnlocked));
        }

        public void AssignCRs(bool cr1, bool ledSpecial1, bool cr2, bool ledSpecial2)
        {
            Send(NodeFrame.AssignCardReaders(
                cr1 ? 1 : 0, 0x45, 4, null, ledSpecial1, 
                cr2 ? 2 : 0, 0x44, 4, null, ledSpecial2));
        }

        public void SuppressCardReader(int address)
        {
            Send(NodeFrame.SuppressCardReader(address));
        }

        public void LooseCardReader(int address)
        {
            Send(NodeFrame.LooseCardReader(address));
        }

        public void SetImplicitCRCode(int address,bool crLedSpecial)
        {
            if (address == 1)
            {
                Send(NodeFrame.SetImplicitCRCode(1, 0x43, -1, null, crLedSpecial));
            }
            else
            {
                Send(NodeFrame.SetImplicitCRCode(2, 0x43, -1, null, crLedSpecial));
            }
        }

        public void FastDSMSetup()
        {
            Send(NodeFrame.SetActuator(ActuatorType.ElectricStrike, 0, StrikeType.Level, 0, false, 0, 0));
            Send(NodeFrame.SetBypassAlarm(1));
            Send(NodeFrame.SetPushButton(PushButtonType.Internal, 0, false, false, 0, 0));
            Send(NodeFrame.SetSensor(SensorType.DoorOpened, 1, false, false, 0, 0));
            Send(NodeFrame.AssignCardReaders(1, 0x45, 4, null, false, 2, 0x44, 5, new CRMessage(0, CRMessageCode.RED_LED_MODE, (byte)IndicatorMode.LowFrequency),false));
        }
        #endregion

        #region Outputs
        public void ConfigLevel(byte outputID, UInt32 delayToOn, UInt32 delayToOff, bool inverted)
        {
            Frame frame = NodeFrame.OutputConfigLevel(outputID, delayToOn, delayToOff, inverted);
            Send(frame);
        }

        public void ConfigFrequency(byte outputID, UInt32 onTime, UInt32 offTime, UInt32 delayToOn,
            UInt32 delayToOff, bool forcedOff, bool inverted)
        {
            Frame frame = NodeFrame.OutputConfigFrequency(outputID, onTime, offTime, delayToOn, delayToOff,
                forcedOff, inverted);
            Send(frame);
        }

        public void ConfigPulse(byte outputID, UInt32 pulseTime, UInt32 delayToOn, UInt32 delayToOff,
            bool forcedOff, bool inverted)
        {
            Frame frame = NodeFrame.OutputConfigPulse(outputID, pulseTime, delayToOn, delayToOff, forcedOff,
                inverted);
            Send(frame);
        }

        public void SetReportedOutputs(params int[] outputIDs)
        {
            if (outputIDs == null || outputIDs.Length == 0)
                return;

            Send(NodeFrame.SetRerpotedOutputs(outputIDs));
        }

        public void SetReportedOutputsEx(params int[] outputIDs)
        {
            if (outputIDs == null || outputIDs.Length == 0)
                return;

            Send(NodeFrame.SetReportedOutputsEx(outputIDs));
        }

        public void UnsetReportedOutputs(params int[] outputIDs)
        {
            if (outputIDs == null || outputIDs.Length == 0)
                return;

            Send(NodeFrame.UnsetReportedOutputs(outputIDs));
        }

        public void SetReportedOutputsLogic(params int[] outputIDs)
        {
            if (outputIDs == null || outputIDs.Length == 0)
                return;

            Send(NodeFrame.SetReportedOutputsLogic(outputIDs));
        }

        public void SetReportedOutputsLogicEx(params int[] outputIDs)
        {
            if (outputIDs == null || outputIDs.Length == 0)
                return;

            Send(NodeFrame.SetReportedOutputsLogicEx(outputIDs));
        }

        public void UnsetReportedOutputsLogic(params int[] outputIDs)
        {
            if (outputIDs == null || outputIDs.Length == 0)
                return;

            Send(NodeFrame.UnsetReportedOutputsLogic(outputIDs));
        }

        public void ActivateOutput(byte outputID, bool activate)
        {
            Frame frame = NodeFrame.SetOutput(outputID, activate);
            Send(frame);
        }

        public void ForceOutputOff(byte outputID)
        {
            Send(NodeFrame.SetOutputTotalOff(outputID));
        }
        #endregion

        #region Inputs
        public void SetBSIParams(byte inputID, UInt32 filtertime, UInt32 delayToOn, UInt32 delayToOff,
            UInt32 tamperDelay)
        {
            Frame frame = NodeFrame.SetBSIParams(inputID, filtertime, delayToOn, delayToOff, tamperDelay);
            Send(frame);
        }

        public void RemapBSI(byte inputID, InputState state0, InputState state1, InputState state2, InputState state3)
        {
            Frame frame = NodeFrame.RemapBSI(inputID, state0, state1, state2, state3);
            Send(frame);
        }

        public void SetDIParams(byte inputID, UInt32 filtertime, UInt32 delayToOn, UInt32 delayToOff)
        {
            Frame frame = NodeFrame.SetDIParams(inputID, filtertime, delayToOn, delayToOff);
            Send(frame);
        }

        public void RemapDI(byte inputID, InputState state0, InputState state1)
        {
            Frame frame = NodeFrame.RemapDI(inputID, state0, state1);
            Send(frame);
        }

        public void SetBSILevels(UInt16 toLevel1, UInt16 toLevel2, UInt16 toLevel3)
        {
            Frame frame = NodeFrame.SetBSILevels(toLevel1, toLevel2, toLevel3);
            Send(frame);
        }

        public void UnsetInput(byte inputID)
        {
            Frame frame = NodeFrame.UnsetInput(inputID);
            Send(frame);
        }

        public void BindOutputToInput(byte outputID, byte inputID)
        {
            Frame frame = NodeFrame.BindOutputToInput(outputID, inputID);
            Send(frame);
        }

        public void UnbindOutputFromInput(byte outputID, byte inputID)
        {
            Frame frame = NodeFrame.UnbindOutputFromInput(outputID, inputID);
            Send(frame);
        }

        public void SetReportedInputs(params int[] inputIDs)
        {
            Send(NodeFrame.SetReportedInputs(inputIDs));
        }

        public void SetReportedInputsEx(params int[] inputIDs)
        {
            Send(NodeFrame.SetReportedInputsEx(inputIDs));
        }

        public void UnsetReportedInputs(params int[] inputIDs)
        {
            Send(NodeFrame.UnsetReportedInputs(inputIDs));
        }
        #endregion

        #region Miscellanous
        public void SetTime(int hours, int minutes, int seconds)
        {
            Send(NodeFrame.SetTime(hours, minutes, seconds));
        }

        public void RestartNode()
        {
            Send(NodeFrame.RestartNode());
        }

        public void ReadInputCount()
        {
            Frame frame = NodeFrame.ReadInputCount();
            Send(frame);
        }

        public void ReadOutputCount()
        {
            Frame frame = NodeFrame.ReadOutputCount();
            Send(frame);
        }

        public void ReadMemoryLoad()
        {
            Frame frame = NodeFrame.ReadMemoryLoad();
            Send(frame);
        }

        public void SetCRPLevel(CRPLevel level)
        {
            Send(NodeFrame.SetCRPLevel(level));
        }
        #endregion

        #region Upgrade
        /* Upgrading */
        public void ResetToBootloader()
        {
            Frame frame = NodeFrame.ResetToBootloader();
            Send(frame);
        }

        public void StartIOTest()
        {
            Send(NodeFrame.StartIOTest(false));
        }

        public void ResetToApplication()
        {
            Frame frame = NodeFrame.ResetToApplication();
            Send(frame);
        }

        public void UpgradeNode(string filename)
        {
            //if (!File.Exists(filename))
            //    return;

            _nodeComm.UpgradeNode((byte)_selectedNode, filename);
        }

        public void ErasePage()
        {
            Frame frame = NodeFrame.ErasePage(119);
            Send(frame);
        }
        #endregion

       
        #region HW Test
       
        public event DType2Void<int> IOTestFailed;
        private void FireIOTestFailed(int inputID)
        {
            try
            {
                if (IOTestFailed != null)
                    IOTestFailed(inputID);
            }
            catch { }
        }

        public event DVoid2Void IOTestPassed;
        private void FireIOTestPassed()
        {
            try
            {
                if (IOTestPassed != null)
                    IOTestPassed();
            }
            catch { }
        }

        public event DType2Void<int> IOTestProgress;
        private void FireIOTestProgress(int progress)
        {
            try
            {
                if (IOTestProgress != null)
                    IOTestProgress(progress);
            }
            catch { }
        }


        public void DoIt()
        {
            Send(NodeFrame.SetDIParams(2, 50, 0, 0));
            Send(NodeFrame.SetDIParams(3, 50, 0, 0));
            
            Send(NodeFrame.OutputConfigLevel(1, 0, 0, false));
            Send(NodeFrame.SetOutput(1, false));
            Send(NodeFrame.OutputConfigLevel(2, 0, 0, false));
            Send(NodeFrame.SetOutput(2, false));
            Send(NodeFrame.OutputConfigLevel(3, 0, 0, false));
            Send(NodeFrame.SetOutput(3, false));

            Send(NodeFrame.SetSensor(SensorType.DoorOpened, 1, false, false, 0, 1000));
            Send(NodeFrame.SetActuator(ActuatorType.ElectricStrike, 0, StrikeType.Level, 0, false, 0, 0));
            Send(NodeFrame.SetPushButton(PushButtonType.Internal, 0, false, false, 0, 0));
            
            Send(NodeFrame.StartDSM(DoorEnviromentType.Standard));
        }

        #endregion

        #region Protocol testing
        private bool _debugSeqRunning = false;
        private SafeThread _debugSeqThread = null;
        private byte _currentSeqNumber = 0;
        private byte _recSeqNumber = 0;
        private bool _debugSeqTimeout = false;
        private AutoResetEvent _debugSeqEvent;

        public void StartSeqDebug()
        {
            if (_debugSeqRunning)
                return;

            _debugSeqEvent = new AutoResetEvent(false);
            _debugSeqThread = new SafeThread(DebugSeqThread);
            _debugSeqThread.Start();

            _debugSeqRunning = true;
        }

        public void StopSeqDebug()
        {
            if (!_debugSeqRunning)
                return;

            if (_debugSeqThread != null)
                _debugSeqThread.Stop(500);
        }

        private void DebugSeqThread()
        {
            int tmpCounter = 0;

            try
            {
                while (true)
                {
                    //_debugSeqEvent.Reset();
                    _debugSeqTimeout = false;

                    Send(NodeFrame.DebugSequenceRequest(_currentSeqNumber));

                    _debugSeqEvent.WaitOne();

                    if (_currentSeqNumber != _recSeqNumber)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR - current: " + _currentSeqNumber + ", received: " + _recSeqNumber);
                    }
                    else if (_debugSeqTimeout)
                    {
                        System.Diagnostics.Debug.WriteLine("TIMEOUT - current: " + _currentSeqNumber);
                    }
                    else
                    {
                        if ((tmpCounter % 10) == 0)
                            System.Diagnostics.Debug.WriteLine("INFO - current: " + _currentSeqNumber);

                        tmpCounter++;

                        
                    }

                    _currentSeqNumber++;
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Debug sequence numbers: " + error.Message);
            }
        }

        void _nodeComm_DebugSequenceResponse(byte seqNumber)
        {
            if (seqNumber == _currentSeqNumber)
            {
                //System.Diagnostics.Debug.WriteLine("RESPONSE: " + seqNumber);
                _debugSeqTimeout = false;
                _recSeqNumber = seqNumber;
                _debugSeqEvent.Set();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR - received does not match: " + _currentSeqNumber + " : " + seqNumber);
            }
        }

        #endregion

        #endregion
    }
}
