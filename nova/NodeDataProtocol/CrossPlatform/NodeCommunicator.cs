#define UPGRADE_DEBUG

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System;
using System.Diagnostics;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Net;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Sys;

using Contal.Drivers.ClspDrivers;
using Contal.Cgp.NCAS.Definitions;


#if !PC

using Contal.Drivers.LPC3250;
using Contal.Drivers.InterCommDrivers;

#endif


namespace Contal.Cgp.NCAS.NodeDataProtocol
{
    public enum UpgradeStage : byte
    {
        WaitForRequestConfirmed,
        WaitForBootloader,
        ReadBaseAddress,
        Erase,
        Read,
        Write,
        EraseInfoPage,
        WriteLength,
        WriteChecksum
    }

    public enum UpgradeErrors : byte
    {
        NodeLost = 0,
        TooManyWriteErrors = 1,
        RequestUpgradeError = 2,
        ResetToBootloaderError = 3,
        ReadBaseAddress = 4,
        OtherError = 0xff
    }

    internal class NodeDataObject
    {
        internal readonly ClspFrame _clspFrame;
        internal readonly ClspNode _node;
        //private int _timeoutCount;

        // not needed when used directly from nodeInfo
        //internal readonly AutoResetEvent _refTransmittedByLinkLayerMutex = null;
        internal int _processingId = -1;

        internal NodeDataObject(ClspFrame clspFrame, ClspNode node)
        //AutoResetEvent transmittedByLinkLayerMutex)
        {
            _clspFrame = clspFrame;
            _node = node;
            //_refTransmittedByLinkLayerMutex = transmittedByLinkLayerMutex;
            //_timeoutCount = 0;
        }

        /*public int TimeoutCount
        {
            get { return _timeoutCount; }
            set { _timeoutCount = value; }
        }*/

        public override string ToString()
        {
            //DateTime time = DateTime.Now;

            return
                "Frame(Node" + _node.LogicalAddress + ") " + _clspFrame.MessageCode;// time.ToString("mm.ss.fffffff") + "): " + ByteDataCarrier.HexDump(_frame.GetRaw485Bytes(null));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NodeCommunicator :
        //ClspCommunicator, 
        ICardReaderEventHandler,
        IClspNodeEventHandler
    {
        //private const int RETRY_COUNT = 2;

        private const int ASYNC_TIMEOUT = 3000;
        private const int WAIT_BOOTLOADER_TIMEOUT = 60000;
        private const int BASE_ADDRESS_TIMEOUT = 2000;
        //private const int CHUNK_SIZE = 64;
        private int ChunkSize = CLSP_LON_UPGRADE_CHUNK_SIZE;
        private const int PAGE_SIZE = 512;
        //private const int BASE_APP_ADDRESS = 0x5000;
        //private const int CONTROL_DATA_PAGE = 127;

        //private const int APP_LENGTH_CODE = 0xfffe;
        //private const int APP_CHECKSUM_CODE = 0xffff;

        private const int ERROR_COUNT_TRIGGER = 5;

        internal const string ParamFwVersionNode = "fw_version";
        internal const string ParamBootloaderVersionNode = "bootloader_version";
        internal const string ParamBaseAddressNode = "base_address";
        internal const string ParamCrCommunicatorNode = "cr_communicator";

        private volatile int _asyncTimeout;

        private readonly Log _log;

        /// <summary>
        /// 
        /// </summary>
        private class NodeInfo : ADisposable
        {
            internal AsyncProcessingQueue<NodeDataObject> _queue;

            private AutoResetEvent _transmittedByLinkLayerMutex;

            internal int _actualTransmittingProcessingId = -1;

            internal bool WaitForTransmittedByLinkLayer(int milisecondTimeout)
            {
                var waitingResult = _transmittedByLinkLayerMutex.WaitOne(milisecondTimeout,false);
                return
                    waitingResult && !AimedToBeDisposed;
            }

            internal void SetTransmittedByLinkLayer()
            {
                _transmittedByLinkLayerMutex.Set();
            }

            internal NodeInfo([NotNull] IClspNode node)
            {
                Validator.CheckForNull(node, "node");
                //_node = node;
                _transmittedByLinkLayerMutex = EventWaitHandlePool<AutoResetEvent>.Singleton.Get();
                _transmittedByLinkLayerMutex.Reset();

                _queue = new AsyncProcessingQueue<NodeDataObject>();
            }

            protected override void InternalDispose(bool isExplicitDispose)
            {
                try
                {
                    _transmittedByLinkLayerMutex.Set();
                    EventWaitHandlePool<AutoResetEvent>.Singleton.Return(_transmittedByLinkLayerMutex);
                    _transmittedByLinkLayerMutex = null;
                }
                catch
                {
                    
                }

                try
                {
                    if (_queue.ActualItem != null)
                        _queue.EndItemProcessing();
                }
                catch
                {

                }

                try
                {
                    _queue.Dispose();
                }
                catch
                {
                }
                finally
                {
                    _queue = null;
                }
            }
        }

        private readonly SyncDictionary<byte, NodeInfo> _nodeInfos;

        private ProtocolId _masterProtocol = ProtocolId.InvalidProtocol;

        #region Constructors

        private readonly ClspCommunicator _clsp;

#if PC
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aesSettings"></param>
        public NodeCommunicator(AESSettings aesSettings)
            : base(aesSettings)
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="communicationMode"></param>
        /// <param name="interComm"></param>
        /// <param name="aesSettings"></param>
        /// <param name="log"></param>
        public NodeCommunicator(
            ClspTransportMode communicationMode,
            ICoprocessorG1 interComm,
            AESSettings aesSettings,
            Log log)
        //: base(communicationMode, interComm, aesSettings,log)
#endif
        {
            _clsp = new ClspCommunicator(communicationMode, interComm, aesSettings, log)
            {
                DirectEventHandler = this
            };

            _nodeInfos = new SyncDictionary<byte, NodeInfo>();
            _masterProtocol = ProtocolId.ProtoMaxId;
            _asyncTimeout = ASYNC_TIMEOUT;


#if PC
            _log = Log.Singleton;
            _useLinkLayerTransmitWaiting = true;
#else
            _log = log ?? Log.Singleton;
            if (communicationMode == ClspTransportMode.IndirectViaLonShortStack)
                _useLinkLayerTransmitWaiting = false;
#endif
        }

#if ! PC
        public NodeCommunicator(ClspTransportMode communicationMode, ICoprocessorG1 interComm, AESSettings aesSettings)
            : this(communicationMode, interComm, aesSettings, null)
        {
        }
#endif

        #endregion

        private const string NodeCommunicatorSource = "NodeCommunicator";

        #region Processing queue handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeDataObject"></param>
        void OnSlaveNodesItemProcessingTimedOut(NodeDataObject nodeDataObject)
        {
            NodeInfo nodeInfo;
            var relevantTimeout = _nodeInfos.TryGetValue(nodeDataObject._node.LogicalAddress, out nodeInfo);
            //relevantTimeout = _nodeInfos.ContainsKey(data._node.LogicalAddress);

            if (relevantTimeout)
            {
                if (nodeDataObject._clspFrame.MaxRetryCount > 0 && nodeDataObject._clspFrame.MaxRetryCount > nodeDataObject._clspFrame.ActualRetryCount)
                {
                    nodeDataObject._clspFrame.ActualRetryCount++;

                    if (nodeInfo != null) // TODO : does this condition have meaning, when controlled by relevantTimeout ?
                    {
                        var dataObject = new NodeDataObject(nodeDataObject._clspFrame, nodeDataObject._node);//, data._refTransmittedByLinkLayerMutex);
                        var pq = GetNodeQueue(nodeDataObject._node);

                        if (pq != null)
                        {
                            dataObject._processingId = pq.EnqueueTop(dataObject).ProcessingId;

                            _log.Warning(
                                NodeCommunicatorSource,
                                nodeDataObject._node + " COMMAND TIMEOUT retry=" + nodeDataObject._clspFrame.ActualRetryCount + " : " + ByteDataCarrier.HexDump(nodeDataObject._clspFrame.GetRaw485Bytes(null)));
                        }
                        return;
                    }
                }
                else
                {
                    _log.Error(NodeCommunicatorSource, nodeDataObject._node + " COMMAND TIMEOUT FINAL on " + ByteDataCarrier.HexDump(nodeDataObject._clspFrame.GetRaw485Bytes(null)));
                    FireCommandTimeout(nodeDataObject._node, nodeDataObject._clspFrame);
                }
            }

            var cmd = ((NodeCommand)nodeDataObject._clspFrame.Command);

            if (cmd == NodeCommand.WriteData)
            {
                UpgradeContext upContext;
                lock (_upgradeContext)
                {
                    _upgradeContext.TryGetValue(nodeDataObject._node.LogicalAddress, out upContext);
                }

                if (upContext != null)
                {

                    upContext.DataAgain = true;
                    upContext.WaitForReply.Set();
                    upContext.ErrorCount++;
                    //upContext.Timeout = true;
                    DebugUpgrade("Command timeout: WRITE DATA - node: " + nodeDataObject._node.LogicalAddress);
                }
            }

            if (cmd == NodeCommand.ErasePage)
            {
                UpgradeContext upContext;
                lock (_upgradeContext)
                {
                    _upgradeContext.TryGetValue(nodeDataObject._node.LogicalAddress, out upContext);
                }

                if (upContext != null)
                {
                    upContext.EraseAgain = true;
                    upContext.WaitForErase.Set();
                    upContext.ErrorCount++;
                    //upContext.Timeout = true;
                    DebugUpgrade("Command timeout: WRITE DATA - node: " + nodeDataObject._node.LogicalAddress);
                }
            }

            if (cmd == NodeCommand.RequestUpgrade)
            {
                UpgradeContext upContext;
                lock (_upgradeContext)
                {
                    _upgradeContext.TryGetValue(nodeDataObject._node.LogicalAddress, out upContext);
                }

                if (upContext != null)
                {
                    upContext.WaitForReply.Set();
                    upContext.Timeout = true;
                    DebugUpgrade("Command timeout: WRITE DATA - node: " + nodeDataObject._node.LogicalAddress);
                }
            }

        }

        private volatile bool _useLinkLayerTransmitWaiting = true;
        // ReSharper disable once ConvertToConstant.Local
        private readonly int _llTransmitWaitingFailsafeTimeout = 30000;

        private volatile bool _coprocessorFailure = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CoprocessorFailure
        {
            get
            {
                return _coprocessorFailure;
            }
            private set
            {
                if (value != _coprocessorFailure)
                {
                    _coprocessorFailure = value;

                    if (CoprocessorFailureChanged != null)
                        try
                        {
                            CoprocessorFailureChanged(value);
                        }
                        catch
                        {
                        }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event DBool2Void CoprocessorFailureChanged;

        private const string LOG_PREFIX = "NodeCommunicator/ ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeDataObject"></param>
        void OnSlaveNodesItemProcessing(NodeDataObject nodeDataObject)
        {
            NodeInfo nodeInfo;
            _nodeInfos.TryGetValue(nodeDataObject._node.LogicalAddress, out nodeInfo);

            if (nodeInfo == null)
                return; // if the node does not exist the data layer should not enqueue any more frames for that node

            try
            {
                if (_useLinkLayerTransmitWaiting)
                {
                    nodeInfo._actualTransmittingProcessingId = nodeDataObject._processingId;
                    // marks start of the frame transmitting process
                    nodeDataObject._clspFrame.ForeignData = nodeDataObject._processingId;
                }

                // priority is handled by the processing queue itself
                _clsp.EnqueueFrame(nodeDataObject._node.LogicalAddress, nodeDataObject._clspFrame);
                CoprocessorFailure = false;

#if DEBUG
                //Console.WriteLine("Queuing("+param._processingId+") on Node" + param._node.LogicalAddress + " -> MC:" + param._frame.MessageCode);
#endif

                if (_useLinkLayerTransmitWaiting)
                {
                    var passed = nodeInfo.WaitForTransmittedByLinkLayer(_asyncTimeout * 3);
                    var tpid = nodeInfo._actualTransmittingProcessingId;
                    nodeInfo._actualTransmittingProcessingId = -1;
                    // marks end of the waiting for the frame transmitting process

                    if (!passed)
                    {
                        _log.Warning(Log.LOW_LEVEL, () => (LOG_PREFIX +
                                                           "Link layer did not send notice about frame with id " + tpid +
                                                           " transmitted in " + _llTransmitWaitingFailsafeTimeout + "ms"));
                    }

                }
            }
            catch (DoesNotExistException)
            {
            }
            catch (CoprocessorG1Exception cce)
            {
                HandledExceptionAdapter.Examine(cce);

                CoprocessorFailure = true;
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int AsyncTimeout
        {
            get { return _asyncTimeout; }
            set
            {
                _asyncTimeout = value;

                //foreach (ProcessingQueue<DataObject> queue in _queues)
                {
                    //    queue.AsynchronousTimeout = _asyncTimeout;
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private AsyncProcessingQueue<NodeDataObject> GetNodeQueue([NotNull] IClspNode node)
        {
            Validator.CheckForNull(node, "node");

            NodeInfo ni;
            _nodeInfos.TryGetValue(node.LogicalAddress, out ni);

            if (ni == null) // this condition has been added as a reaction to state when ni was null, but TryGetValue returned true
            {
                //DebugHelper.TryBreak("NodeInfo unknown for this node",node);
                return null;
            }

            return ni._queue;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <returns></returns>
        private ProcessingQueue<NodeDataObject> GetNodeQueue(byte nodeAddress)
        {
            NodeInfo ni;
            lock (_nodeInfos)
            {
                _nodeInfos.TryGetValue(nodeAddress, out ni);
            }

            if (ni == null)
            {
#if PC
                Debug.Assert(false, "NodeInfo unknown for this node");
#endif
                DebugUpgrade("NodeInfo unknown for this node", true);
                return null;
            }
            
            return ni._queue;
        }*/

        #region CR stuff

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IList<CardReader> GetCardReaders(IClspNode node)
        {
            try
            {
                // no need to handle null conditions, almost the same speed as conditioning
                if (ReferenceEquals(node, null))
                    return null;

                var crComm = node.GetCrCommunicator();
                if (crComm == null)
                    return null;

                return crComm.AllCardReaders;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <returns></returns>
        public IList<CardReader> GetCardReaders(byte nodeAddress)
        {
            try
            {
                var sn = _clsp[nodeAddress];

                if (sn != null)
                {
                    var crComm = sn.GetCrCommunicator();
                    if (crComm != null)
                        return crComm.AllCardReaders;

                    return null;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="crAddress"></param>
        /// <returns></returns>
        public CardReader GetCardReader(IClspNode node, byte crAddress)
        {
            try
            {
                if (null != node)
                {
                    var crComm = node.GetCrCommunicator();

                    if (crComm != null)
                        return crComm.GetCardReader(crAddress);

                    return null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="crAddress"></param>
        /// <returns></returns>
        public CardReader GetCardReader(byte nodeAddress, byte crAddress)
        {
            try
            {
                var sn = _clsp[nodeAddress];

                if (sn != null)
                {

                    var crComm = sn.GetCrCommunicator();
                    if (crComm != null)
                        return crComm.GetCardReader(crAddress);
                    return null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static CRNestedCommunicator GetCrCommunicator([NotNull] IClspNode node)
        {
            if (ReferenceEquals(node, null))
                throw new ArgumentNullException("node");

            return node.GetParameter<CRNestedCommunicator>(ParamCrCommunicatorNode);
        }


        #endregion

        /*
        private NodeInfo GetNodeInfo(byte address)
        {
            lock (_nodeInfos)
            {
                NodeInfo ni;
                if (!_nodeInfos.TryGetValue(address, out ni))
                {
#if PC
                    Debug.Assert(false, "NodeInfo unknown for this address : " + address);
#endif
                    return null;
                }
                
                return ni;
            }
        }*/

        /*
        private NodeInfo GetNodeInfo(SlaveNode node)
        {
            Validator.CheckNull(node);

            lock (_nodeInfos)
            {
                NodeInfo ni;
                if (!_nodeInfos.TryGetValue(node.LogicalAddress, out ni))
                {
#if PC
                    Debug.Assert(false, "NodeInfo unknown for this node : " + node);
#endif
                    return null;
                }
                return ni;
            }
        }*/

        #region Enqueue methods
        public void EnqueueFrame(IClspNode node, IClspFrame message)
        {
            if (node == null)
                throw new ArgumentNullException("node", "Node does not exists");

            NodeInfo nodeInfo;
            _nodeInfos.TryGetValue(node.LogicalAddress, out nodeInfo);

            if (nodeInfo == null)
                throw new ArgumentNullException("node", string.Format("Node queue for node {0} does not exists", node));

            var data = new NodeDataObject(message as ClspFrame, node as ClspNode);
            data._processingId = nodeInfo._queue.Enqueue(data).ProcessingId;
            //GetNodeQueue(node).Enqueue(data);
        }

        /// <summary>
        /// enqueues frame for specific SlaveNode's address
        /// </summary>
        /// <param name="address">logical address of the SlaveNode</param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException">
        /// in case the node specified by address does not exist</exception>
        public void EnqueueFrame(byte address, IClspFrame message)
        {
            var node = _clsp.GetNode(address);

            EnqueueFrame(node,message);
        }

        /*public new void EnqueueTopFrame(byte address, Frame message)
        {
            SlaveNode node = GetNode(address);

            if (node == null)
                throw new ArgumentNullException("Node does not exists");
            if (!_nodeInfos.ContainsKey(address))
                throw new ArgumentNullException("Node queue does not exists");
            
            DataObject data = new DataObject(message, node);
            GetNodeQueue(node).EnqueueTop(data);
        }

        public new void EnqueueTopFrame(SlaveNode node, Frame message)
        {
            if (node == null)
                throw new ArgumentNullException("Node does not exists");
            if (!_nodeInfos.ContainsKey(node.LogicalAddress))
                throw new ArgumentNullException("Node queue does not exists");

            DataObject data = new DataObject(message, node);
            GetNodeQueue(node).EnqueueTop(data);
        }*/

        #endregion

        #region Message events

        /// <summary>
        /// event raised when logical state of input changes
        /// </summary>
        public event Action<IClspNode, byte, InputState> InputChanged;
        private void FireInputChanged(IClspNode node, byte inputID, InputState newState)
        {
            try
            {
                if (null == InputChanged) return;
                InputChanged.Call(node, inputID, newState);
            }
            catch { }
        }

        /// <summary>
        /// event raised when logical state of the output changes
        /// </summary>
        public event Action<IClspNode, byte, bool> OutputLogicChanged;
        private void FireOutputLogicChanged(IClspNode node, byte outputID, bool isActive)
        {
            try
            {
                if (OutputLogicChanged != null)
                    OutputLogicChanged.Call(node, outputID, isActive);
            }
            catch { }
        }

        /// <summary>
        /// event raised when logical state of special input changes
        /// </summary>
        public event Action<IClspNode, SpecialInput, InputState> SpecialInputChanged;
        private void FireSpecialInputChanged(IClspNode node, SpecialInput inputType, InputState state)
        {
            try
            {
                if (null == SpecialInputChanged) return;

                SpecialInputChanged.Call(node, inputType, state);
            }
            catch { }
        }

        /// <summary>
        /// event raised when physical state of output changes
        /// </summary>
        public event Action<IClspNode, byte, bool> OutputChanged;
        private void FireOutputChanged(IClspNode node, byte outputID, bool isOn)
        {
            try
            {
                if (null == OutputChanged) return;

                OutputChanged.Call(node, outputID, isOn);
            }
            catch { }
        }

        /// <summary>
        /// event raised when DIP switch changes
        /// </summary>
        public event Action<IClspNode, int> DIPChanged;
        private void FireDIPChanged(IClspNode node, int value)
        {
            try
            {
                if (DIPChanged != null)
                    DIPChanged.Call(node, value);
            }
            catch { }
        }

        /// <summary>
        /// event raised when state of door environment changes
        /// </summary>
        public event Action<IClspNode, DoorEnvironmentState, DoorEnvironmentStateDetail, DoorEnvironmentAccessTrigger> DSMChanged;
        private void FireDSMChanged(IClspNode node, DoorEnvironmentState signal, DoorEnvironmentStateDetail detail, DoorEnvironmentAccessTrigger source)
        {
            try
            {
                if (DSMChanged != null)
                    DSMChanged.Call(node, signal, detail, source);
            }
            catch { }
        }

        /*
        /// <summary>
        /// event raised when SlaveNode sends aggregated input/output count
        /// </summary>
        public event D3Type2Void<SlaveNode, uint, uint> ReportDeviceInfo;
        private void FireReportDeviceInfo(SlaveNode node, uint inputCount, uint outputCount)
        {
            try
            {
                if (ReportDeviceInfo != null)
                    ReportDeviceInfo(node, inputCount, outputCount);
            }
            catch { }
        }*/

        /// <summary>
        /// event raised when information about count of outputs arrive from SlaveNode
        /// </summary>
        public event Action<IClspNode, uint> ReportOutputCount;
        private void FireReportOutputCount(IClspNode node, uint outputCount)
        {
            try
            {
                if (ReportOutputCount != null)
                    ReportOutputCount.Call(node, outputCount);
            }
            catch { }
        }

        /// <summary>
        /// event raised when CRP level reading was requested from SlaveNode
        /// </summary>
        public event Action<IClspNode, CRPLevel> ReportCRPLevel;
        private void FireReportCRPLevel(IClspNode node, CRPLevel level)
        {
            try
            {
                if (ReportCRPLevel != null)
                    ReportCRPLevel.Call(node, level);
            }
            catch { }
        }

        /// <summary>
        /// event raised when information about count of inputs arrive from SlaveNode
        /// </summary>
        public event Action<IClspNode, uint> ReportInputCount;
        private void FireReportInputCount(IClspNode node, uint inputCount)
        {
            try
            {
                if (ReportInputCount != null)
                    ReportInputCount.Call(node, inputCount);
            }
            catch { }
        }

        /// <summary>
        /// event raised when previous request for reading FW version was sent to SlaveNode
        /// </summary>
        public event Action<IClspNode, ExtendedVersion> ReportFWVersion;
        private void FireReportFWVersion(IClspNode node, ExtendedVersion version)
        {
            try
            {
                if (ReportFWVersion != null)
                    ReportFWVersion.Call(node, version);
            }
            catch { }
        }

        /// <summary>
        /// event raised if specific command acknowledgment timed out after number of retries
        /// </summary>
        public event Action<IClspNode, ClspFrame> CommandTimeout;
        private void FireCommandTimeout(IClspNode clspNode, ClspFrame command)
        {
            try
            {
                if (CommandTimeout != null)
                    CommandTimeout.Call(clspNode, command);
            }
            catch { }
        }

        /// <summary>
        /// event raised when any command is acknowledged from SlaveNode 
        /// on NodeDataProtocol level
        /// </summary>
        public event Action<IClspNode, NodeCommand> CommandACK;
        private void FireCommandAck(IClspNode node, NodeCommand command)
        {
            try
            {
                if (CommandACK != null)
                    CommandACK.Call(node, command);
            }
            catch { }
        }

        /// <summary>
        /// thrown if command-request rejected by any error reason ; 
        /// called with reference to slave node, original request command, error code causing nack, original request frame
        /// </summary>
        public event Action<IClspNode, NodeCommand, NodeErrorCode, IClspFrame> CommandNACK;
        private void FireCommandNack(IClspNode node, NodeErrorCode error, IClspFrame requestFrame)
        {
            try
            {
                if (CommandNACK != null)
                    CommandNACK.Call(node, (NodeCommand)requestFrame.Command, error, requestFrame);
            }
            catch { }
        }

        /// <summary>
        /// event raised if the frame is not known by NodeDataProtocol
        /// </summary>
        public event Action<IClspNode, IClspFrame> UnknownFrame;
        private void FireUnknownFrame(IClspNode node, IClspFrame frame)
        {
            try
            {
                if (UnknownFrame != null)
                    UnknownFrame.Call(node, frame);
            }
            catch { }
        }

        /// <summary>
        /// event raised when ProtocolId of SlaveNode changes
        /// </summary>
        public event Action<IClspNode, ProtocolId> ModeChanged;
        private void FireModeChanged(IClspNode node, ProtocolId mode)
        {
            try
            {
                if (ModeChanged != null)
                    ModeChanged.Call(node, mode);

                _masterProtocol = mode;
            }
            catch { }
        }

        /// <summary>
        /// event raised when StartDSM or StopDSM request had been acknowledged ; 
        /// isRunning == true if StartDSM was request ; 
        /// isRunning == false if StopDSM was request
        /// </summary>
        public event Action<IClspNode, bool> DSMActivationChanged;
        private void FireDSMActivationChanged(IClspNode node, bool isRunning)
        {
            try
            {
                if (DSMActivationChanged != null)
                    DSMActivationChanged.Call(node, isRunning);
            }
            catch { }
        }

        /// <summary>
        /// event raised when information about percentage of upgrade progress arrive from SlaveNode
        /// </summary>
        public event Action<IClspNode, int> UpgradeProgress;
        private void FireUpgradeProgress(IClspNode node, int progress)
        {
            try
            {
                if (UpgradeProgress != null)
                    UpgradeProgress.Call(node, progress);
            }
            catch { }
        }

        /// <summary>
        /// event raised when the upgrade successfully finishes for specific SlaveNode ; 
        /// it's more important event than UpgradeProgress with 100%
        /// </summary>
        public event Action<IClspNode> UpgradeDone;
        private void FireUpgradeDone(IClspNode node)
        {
            try
            {
                DebugUpgrade("FireUpgradeDone: " + node.LogicalAddress, true);
                if (UpgradeDone != null)
                    UpgradeDone.Call(node);
            }
            catch { }
        }

        /// <summary>
        /// event raised when request to upgrade fails on timeout ; 
        /// e.g. when the device will be plugged-off during the upgrade progress
        /// </summary>
        public event Action<IClspNode> UpgradeTimeout;
        private void FireUpgradeTimeout(IClspNode node)
        {
            try
            {
                DebugUpgrade("FireUpgradeTimeout: " + node.LogicalAddress, true);
                if (UpgradeTimeout != null)
                    UpgradeTimeout.Call(node);
            }
            catch { }
        }

        /// <summary>
        /// event raised when upgrade fails with handled error
        /// </summary>
        public event Action<IClspNode, UpgradeErrors, string> UpgradeError;
        private void FireUpgradeError(IClspNode node, UpgradeErrors error, string message)
        {
            try
            {
                DebugUpgrade("FireUpgradeError: " + node.LogicalAddress, true);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (UpgradeError != null && node != null)
                    UpgradeError.Call(node, error, message);
            }
            catch { }
        }

        public event Action<byte> UpgradeNodeLost;
        private void FireUpgradeNodeLost(byte address)
        {
            try
            {
                var tmp = "FireUpgradeNodeLost: " + address;

                var sn = _clsp[address];
                if (sn != null)
                    tmp += StringConstants.SPACE + sn.MasterProtocol;

                DebugUpgrade(tmp, true);
                if (UpgradeNodeLost != null)
                    UpgradeNodeLost.Call(address);
            }
            catch { }
        }

        public event Action<IClspNode, int> IOTestProgress;
        private void FireIOTestProgress(IClspNode node, int progress)
        {
            try
            {
                if (IOTestProgress != null)
                    IOTestProgress.Call(node, progress);
            }
            catch { }
        }

        public event Action<IClspNode, int> IOTestTimeout;
        private void FireIOTestTimeout(IClspNode node, int outputID)
        {
            try
            {
                if (IOTestTimeout != null)
                    IOTestTimeout.Call(node, outputID);
            }
            catch { }
        }

        public event Action<IClspNode> IOTestOk;
        private void FireIOTestOk(IClspNode node)
        {
            try
            {
                if (IOTestOk != null)
                    IOTestOk.Call(node);
            }
            catch { }
        }

        public event Action<IClspNode, int> MemoryWarning;
        private void FireMemoryWarning(IClspNode node, int allocated)
        {
            try
            {
                if (MemoryWarning != null)
                    MemoryWarning.Call(node, allocated);
            }
            catch { }
        }

        public event Action<IClspNode, int> MemoryRestored;
        private void FireMemoryRestored(IClspNode node, int allocated)
        {
            try
            {
                if (MemoryRestored != null)
                    MemoryRestored.Call(node, allocated);
            }
            catch { }
        }

        public event Action<byte> DebugSequenceResponse;
        private void FireDebugSequenceResponse(byte sequence)
        {
            try
            {
                if (DebugSequenceResponse != null)
                    DebugSequenceResponse.Call(sequence);
            }
            catch { }
        }
        #endregion

        #region Upgrading

        /// <summary>
        /// list of upgrade structures for SlaveNode/it's logical adress,
        /// that can actually surpass the existence of SlaveNode instance (typically during restarts)
        /// </summary>
        private readonly Dictionary<byte, UpgradeContext> _upgradeContext = new Dictionary<byte, UpgradeContext>();

        private const int CLSP_LON_UPGRADE_CHUNK_SIZE = 32;
        private const int CLSP_485_UPGRADE_CHUNK_SIZE = 64;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="path"></param>
        public void UpgradeNode(byte nodeAddress, string path)
        {
            ChunkSize =
                _clsp.TransportMode == ClspTransportMode.IndirectViaLonShortStack
                    ? CLSP_LON_UPGRADE_CHUNK_SIZE
                    : CLSP_485_UPGRADE_CHUNK_SIZE;

            if (!File.Exists(path))
                throw new IOException("File not exists");

            UpgradeContext upContext;

            var upgradeAlreadyInProgress = false;

            // ensure atomicity of adding new element into it
            lock (_upgradeContext)
            {
                /* Check if context for this node does not already exists */
                if (_upgradeContext.TryGetValue(nodeAddress, out upContext))
                {
                    /* Check if upgrade isnt in progress already */
                    if (upContext.UpgradeInProgress)
                        upgradeAlreadyInProgress = true;
                }
                else
                {
                    /* Add new upgrade context */
                    upContext = new UpgradeContext(nodeAddress);
                    _upgradeContext.Add(nodeAddress, upContext);

                }
            }

            if (upgradeAlreadyInProgress)
                throw new Exception("Upgrade for node " + nodeAddress + " already in progress");

            upContext.FilePath = path;
            upContext.UpgradeThread = new SafeThread<byte>(UpgradeThread);
            upContext.UpgradeThread.Start(nodeAddress);

            DebugUpgrade("------------------------ UPGRADE STARTED for " + nodeAddress + " --------------------------");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool IsNodeUpgrading(byte address)
        {
            try
            {
                UpgradeContext upContext;
                lock (_upgradeContext)
                {
                    _upgradeContext.TryGetValue(address, out upContext);
                }
                if (upContext != null)
                {
                    return upContext.UpgradeInProgress;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void CheckBootloader(IClspNode node)
        {
            // unblock the wait if this is "the node"
            UpgradeContext upContext;

            lock (_upgradeContext)
            {
                _upgradeContext.TryGetValue(node.LogicalAddress, out upContext);
            }

            if (upContext != null &&
                upContext.WaitForUploader != null)
            {
                if (node.MasterProtocol == ProtocolId.ProtoUploader)
                {
                    DebugUpgrade("Bootloader online - unlocking wait fot node: " + node.LogicalAddress);
                    upContext.WaitForUploader.Set();
                }
                else
                {
                    if (upContext.Stage == UpgradeStage.WaitForBootloader)
                    {
                        DebugUpgrade("Bootloader expected and ProtoAccess received");
                        EnqueueFrame(node.LogicalAddress, NodeFrame.ResetToBootloader());

                        //FireUpgradeError(node, UpgradeErrors.ResetToBootloaderError, string.Empty);
                        //upContext.Error = true;
                        //upContext.WaitForUploader.Set();    // just unblock to quick waiting
                    }
                }
            }
        }

        private void CheckUpgradeProcess(byte address)
        {
            try
            {
                UpgradeContext upgContext;

                lock (_upgradeContext)
                {
                    _upgradeContext.TryGetValue(address, out upgContext);
                }

                if (upgContext != null)
                {
                    if (upgContext.WrittingRunning)
                    {
                        upgContext.Error = true;
                        // unlock all mutexes so the upgrade thread can exit 
                        upgContext.WaitForErase.Set();
                        upgContext.WaitForReply.Set();
                        upgContext.WaitForUploader.Set();

                        upgContext.UpgradeInProgress = false;
                        try
                        {
                            lock (_upgradeContext)
                            {
                                _upgradeContext.Remove(address);
                            }

                            DebugUpgrade("CHECK UPGRADE PROCESS - Removing upgrade context for node: " + address, true);
                        }
                        catch { }
                        // let the upper layer know that this upgrade is lost
                        FireUpgradeNodeLost(address);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeAddress"></param>
        private void UpgradeThread(byte nodeAddress)
        {
            UpgradeContext upContext;

            lock (_upgradeContext)
            {
                _upgradeContext.TryGetValue(nodeAddress, out upContext);
            }

            if (upContext == null)
                // should not happen
                return;

            upContext.UpgradeInProgress = true;

            BinaryReader binaryReader = null;

            FileStream file = null;
            try
            {
                upContext.SetToDefault();

                file = File.Open(upContext.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                binaryReader = new BinaryReader(file);

                var fileSize = (int)binaryReader.BaseStream.Length;
                var lastProgress = 0;

                FireUpgradeProgress(_clsp.GetNode(nodeAddress), lastProgress);

                var dataCounter = 0;
                var data = new byte[ChunkSize];
                var bytesRead = 0;
                var bytesWritten = 0;

                var node = _clsp[nodeAddress];
                if (node == null)
                    throw new Exception("Node " + nodeAddress + " does not exists");

                if (node.MasterProtocol != ProtocolId.ProtoUploader)
                {
                    DebugUpgrade("Request upgrade for node " + upContext.NodeAddress);

                    upContext.WaitForReply.Reset();
                    upContext.Stage = UpgradeStage.WaitForRequestConfirmed;

                    EnqueueFrame(nodeAddress, NodeFrame.RequestUpgrade());
                }
                else
                {
                    //upContext.Stage = UpgradeStage.Read;
                    //upContext.WrittingRunning = true;
                    upContext.Stage = UpgradeStage.ReadBaseAddress;
                }


                while (true)
                {
                    upContext.Error = false;

                    if (_clsp[nodeAddress] == null)
                    {
                        //FireUpgradeError(null, UpgradeErrors.NodeLost, string.Empty);
                        FireUpgradeNodeLost(nodeAddress);
                        upContext.Error = true;
                        break;
                    }

                    switch (upContext.Stage)
                    {
                        #region Wait for 'Request for upgrade' confirmation
                        /* Wait until request for upgrade is confirmed (the switch app/bootloader is beign set */
                        case UpgradeStage.WaitForRequestConfirmed:

                            if (upContext.WaitForReply.WaitOne(2000, false))
                            {
                                var tmp = LOG_PREFIX + "Reset to bootloader for node " + upContext.NodeAddress;
                                DebugUpgrade(tmp);
                                _log.Info(Log.NORMAL_LEVEL, tmp);

                                upContext.Stage = UpgradeStage.WaitForBootloader;
                                upContext.WaitForUploader.Reset();

                                EnqueueFrame(nodeAddress, NodeFrame.ResetToBootloader());
                            }
                            else
                            {
                                var tmp = LOG_PREFIX + "Request upgrade TIMEOUT for node " + upContext.NodeAddress;
                                DebugUpgrade(tmp);
                                _log.Warning(Log.LOW_LEVEL, tmp);

                                upContext.Error = true;
                                if (_clsp[nodeAddress] != null)
                                    FireUpgradeError(_clsp[nodeAddress], UpgradeErrors.RequestUpgradeError, "timeout");
                                else
                                    FireUpgradeNodeLost(nodeAddress);
                            }
                            break;
                        #endregion

                        #region Wait for DCU to reboot to bootloader
                        /* Wait until the DCU has rebooted to bootloader */
                        case UpgradeStage.WaitForBootloader:
                            DebugUpgrade("Wait for bootloader start - node: " + nodeAddress);
                            if (upContext.WaitForUploader.WaitOne(WAIT_BOOTLOADER_TIMEOUT, false))
                            {
                                //upContext.Stage = UpgradeStage.Read;
                                //upContext.WrittingRunning = true;
                                // testing restart to bootloader
                                //upContext.Done = true;
                                //break;

                                upContext.Stage = UpgradeStage.ReadBaseAddress;
                            }
                            else
                            {
                                DebugUpgrade("Wait for bootloader TIMEOUT - node: " + nodeAddress);
                                upContext.Error = true;
                                if (_clsp[nodeAddress] != null)
                                    FireUpgradeError(_clsp[nodeAddress], UpgradeErrors.ResetToBootloaderError, "timeout");
                                else
                                    FireUpgradeNodeLost(nodeAddress);
                            }
                            break;
                        #endregion

                        #region Read base address
                        case UpgradeStage.ReadBaseAddress:
                            DebugUpgrade("Read base address", true);

                            var baseAddress = node.GetParameter(ParamBaseAddressNode);

                            if (null == baseAddress)
                            {
                                DebugUpgrade("Base address received - node: " + nodeAddress);
                                upContext.WaitForBaseAddress.Reset();
                                //EnqueueFrame(nodeAddress, NodeFrame.ReadBaseAddress());

                                upContext.WaitForBaseAddress.WaitOne(BASE_ADDRESS_TIMEOUT, false);

                                // if received, already set in the context
                            }
                            else
                                upContext.BaseAddress = (int)baseAddress;

                            upContext.Stage = UpgradeStage.Read;
                            upContext.WrittingRunning = true;
                            break;
                        #endregion

                        #region Read data from file
                        /* Read data from file */
                        case UpgradeStage.Read:
                            bytesRead = binaryReader.Read(data, 0, ChunkSize);
                            /* No more data read -> go to control data writting */
                            if (bytesRead == 0)
                            {
                                upContext.Done = true;
                                //upContext.WrittingRunning = false;
                                break;
                            }

                            upContext.PartialChecksum = 0;
                            upContext.CalculatePartialChecksum(data, bytesRead);

                            dataCounter += bytesRead;

                            //DebugUpgrade("Data counter: " + dataCounter + " - node: " + nodeAddress);


                            if (bytesWritten == 0 || dataCounter >= PAGE_SIZE)
                            {
                                //DebugUpgrade("Data written: " + bytesWritten + "(read " + (bytesWritten + bytesRead) + ")" +
                                //    "; Data counter: " + dataCounter + " - node: " + nodeAddress);

                                if (bytesWritten != 0)
                                    dataCounter -= PAGE_SIZE;

                                upContext.Stage = UpgradeStage.Erase;
                            }
                            else
                                /* no need for erase -> just write */
                                upContext.Stage = UpgradeStage.Write;
                            break;
                        #endregion

                        #region Erase next page
                        /* Erase page */
                        case UpgradeStage.Erase:
                            DebugUpgrade("Request erase from " + (upContext.PageForErase * 0x200) +
                                " to " + (((upContext.PageForErase + 1) * 0x200) - 1) + " (page " + upContext.PageForErase + ")" +
                                " - node: " + nodeAddress);

                            upContext.WaitForErase.Reset();
                            EnqueueFrame(nodeAddress, NodeFrame.ErasePage(upContext.PageForErase));

                            if (upContext.WaitForErase.WaitOne() && upContext.EraseAgain == false)
                            {
                                DebugUpgrade("Erased page " + upContext.PageForErase + " - node: " + nodeAddress);

                                upContext.PageForErase++;
                                upContext.Stage = UpgradeStage.Write;
                            }
                            else
                            {
                                DebugUpgrade("Erase page TIMEOUT - node: " + nodeAddress);
                            }
                            break;
                        #endregion

                        #region Write data
                        /* Write data */
                        case UpgradeStage.Write:
                            upContext.WaitForReply.Reset();

                            var frame = NodeFrame.WriteData(upContext.BlockNumber, data, bytesRead);
                            EnqueueFrame(nodeAddress, frame);

                            //int startAddr = BASE_APP_ADDRESS + (upContext.BlockNumber * CHUNK_SIZE);
                            var startAddr = upContext.BaseAddress + (upContext.BlockNumber * ChunkSize);
                            // ReSharper disable once RedundantAssignment
                            var endAddr = startAddr + ChunkSize - 1;
                            DebugHelper.NOP(startAddr, endAddr);

                            //DebugUpgrade("Write data from " + startAddr + " to " + endAddr +
                            //    " (" + bytesRead + " bytes), block: " + upContext.BlockNumber + " - node: " + nodeAddress);
                            //DebugUpgrade("Write block: " + upContext.BlockNumber + " - node: " + nodeAddress);


                            if (upContext.WaitForReply.WaitOne() && upContext.DataAgain == false)
                            {
                                bytesWritten += bytesRead;
                                /* Fire progress change */
                                var newProgress = (bytesWritten * 100) / fileSize;
                                if (newProgress > lastProgress && (newProgress - lastProgress >= 3 || lastProgress == 0 || newProgress == 100))
                                {
                                    DebugUpgrade(nodeAddress + ": Progress : " + newProgress);
                                    FireUpgradeProgress(_clsp.GetNode(nodeAddress), newProgress);
                                    // ReSharper disable once RedundantAssignment
                                    lastProgress = newProgress;
                                }

                                upContext.BlockNumber++;
                                upContext.Stage = UpgradeStage.Read;
                            }
                            else
                            {
                                DebugUpgrade("Write TIMEOUT - node: " + nodeAddress);
                            }
                            break;
                        #endregion
                    }



                    if (upContext.Done)
                    {
                        FireUpgradeDone(_clsp[nodeAddress]);
                        EnqueueFrame(nodeAddress, NodeFrame.ResetToApplication());
                        break;
                    }

                    if (upContext.Error)
                    {
                        //FireUpgradeError(UpgradeErrors.NodeLost);
                        break;
                    }

                    if (upContext.Timeout)
                    {
                        FireUpgradeTimeout(_clsp[nodeAddress]);
                        break;
                    }

                    if (upContext.ErrorCount >= ERROR_COUNT_TRIGGER)
                    {
                        FireUpgradeError(_clsp[nodeAddress], UpgradeErrors.TooManyWriteErrors, string.Empty);
                        break;
                    }
                }
            }
            catch (Exception error)
            {
                FireUpgradeError(_clsp[nodeAddress], UpgradeErrors.OtherError, error.Message);
            }
            finally
            {
                if (file != null)
                    try { file.Close(); }
                    catch
                    {
                    }

                if (binaryReader != null)
                    try { binaryReader.Close(); }
                    catch { }
            }

            upContext.UpgradeInProgress = false;

            try
            {
                bool itemRemoved;

                lock (_upgradeContext)
                {
                    itemRemoved = _upgradeContext.Remove(nodeAddress);
                }

                if (itemRemoved)
                {
                    DebugUpgrade("UPGRADE THREAD - Removing upgrade context for node: " + nodeAddress);
                    // check if there is not any forgotten command that might cause problems later
                }
            }
            catch { }
        }

        [Conditional("UPGRADE_DEBUG")]
        private void DebugUpgrade(string message)
        {
            DebugUpgrade(message, false);
        }

        [Conditional("UPGRADE_DEBUG")]
        private void DebugUpgrade(string message, bool intoConsole)
        {
            if (intoConsole)
                _log.Info(Log.NORMAL_LEVEL, () => ("NodeComunicator/ " + message));
#if DEBUG
            _log.Info("NodeCommunicator",message,false,true,null,0);
#endif
        }

        #endregion

        #region IClspNodeEventHandler explicit overrides

        /// <summary>
        /// general parsing of data received by Clsp485/ClspLon
        /// </summary>
        /// <param name="node"></param>
        /// <param name="receivedFrame"></param>
        void IClspNodeEventHandler.OnFrameReceived(IClspNode node, IClspFrame receivedFrame)
        {
            // unblock the wait if this is "the node"
            //CheckBootloader(node);

            UpgradeContext upContext;

            lock (_upgradeContext)
            {
                _upgradeContext.TryGetValue(node.LogicalAddress, out upContext);
            }

            if (_masterProtocol != receivedFrame.Protocol)
                FireModeChanged(node, receivedFrame.Protocol);

            var queue = GetNodeQueue(node);

            if (queue == null)
                return;

            var triggerData = queue.ActualItem;

            var cmd = (NodeCommand)receivedFrame.Command;

            if (cmd == NodeCommand.Ack)
            {
                var ackToCommand = receivedFrame.OptionalData[1];

                if (triggerData != null &&
                    ackToCommand == triggerData._clspFrame.Command &&
                    triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                {
                    queue.EndItemProcessing();

                    FireCommandAck(node, (NodeCommand)ackToCommand);

                    if ((NodeCommand)triggerData._clspFrame.Command == NodeCommand.StartDSM)
                        FireDSMActivationChanged(node, true);
                    else
                    {
                        if ((NodeCommand)triggerData._clspFrame.Command == NodeCommand.StopDSM)
                            FireDSMActivationChanged(node, false);
                    }
                }
            }
            else if (cmd == NodeCommand.Nack)
            {
                var ackToCommand = receivedFrame.OptionalData[1];
                if (triggerData != null && ackToCommand == triggerData._clspFrame.Command &&
                    triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                {
                    queue.EndItemProcessing();

                    // raise error event
                    var errorCode = receivedFrame.OptionalData[2];
                    FireCommandNack(node, (NodeErrorCode)errorCode, triggerData._clspFrame);

                }
            }
            else
            {
                //NodeInfo ni = null;
                // other message -> raise appropriate event

                if ((receivedFrame.Protocol == ProtocolId.ProtoAccess ||
                    receivedFrame.Protocol == ProtocolId.ProtoUploader) &&
                    (NodeCommand)receivedFrame.Command == NodeCommand.FWVersion)
                {

                    if (triggerData != null &&
                        (NodeCommand)triggerData._clspFrame.Command == NodeCommand.ReadFWVersion &&
                        triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                    {
                        queue.EndItemProcessing();
                    }
                    var major = BitConverter.ToInt32(receivedFrame.OptionalData, 1);
                    var minor = BitConverter.ToInt32(receivedFrame.OptionalData, 5);
                    var build = BitConverter.ToInt32(receivedFrame.OptionalData, 9);

                    var version = new ExtendedVersion(major, minor, build, 0, true, string.Empty);

                    node.SetParameter(ParamFwVersionNode, version);

                    var bVerFound = false;
                    try
                    {

                        if (receivedFrame.OptionalDataLength > 13)
                        {
                            var bootVersion = receivedFrame.OptionalData[13];
                            var bootRevision = receivedFrame.OptionalData[14];

                            if (bootVersion != 0 && bootRevision != 0)
                            {
                                node.SetParameter(ParamBootloaderVersionNode, bootVersion + "." + bootRevision);
                                bVerFound = true;
                            }
                        }
                    }
                    catch
                    {
                    }

                    if (!bVerFound)
                        node.SetParameter(ParamBootloaderVersionNode, "1.0");

                    FireReportFWVersion(node, version);


                }

                if (receivedFrame.Protocol == ProtocolId.ProtoAccess)
                {
                    CRNestedCommunicator crComm;

                    switch (cmd)
                    {
                        #region Inputs, Outputs and DSM

                        case NodeCommand.DebugSequenceResponse:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.DebugSequenceRequest &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                queue.EndItemProcessing();
                                FireDebugSequenceResponse(receivedFrame.OptionalData[1]);
                            }

                            break;

                        case NodeCommand.ReadyForUpgrade:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.RequestUpgrade &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                queue.EndItemProcessing();
                                if (upContext != null)
                                    upContext.WaitForReply.Set();
                            }
                            break;

                        case NodeCommand.DeviceInfo:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.ReadDeviceInfo &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                queue.EndItemProcessing();
                            }
                            //FireReportDeviceInfo(node, receivedFrame.OptionalData[1], receivedFrame.OptionalData[2]);
                            FireReportInputCount(node, receivedFrame.OptionalData[1]);
                            FireReportOutputCount(node, receivedFrame.OptionalData[2]);
                            FireReportCRPLevel(node, (CRPLevel)receivedFrame.OptionalData[3]);
                            break;

                        case NodeCommand.IOTestProgress:
                            FireIOTestProgress(node, receivedFrame.OptionalData[0]);
                            break;

                        case NodeCommand.IOTestTimeout:
                            FireIOTestTimeout(node, receivedFrame.OptionalData[0]);
                            break;

                        case NodeCommand.IOTestOk:
                            FireIOTestOk(node);
                            break;

                        case NodeCommand.MemoryLoad:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.ReadMemoryLoad &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                queue.EndItemProcessing();
                            }
                            FireMemoryWarning(node, receivedFrame.OptionalData[1]);
                            break;

                        case NodeCommand.MemoryRestored:
                            FireMemoryRestored(node, receivedFrame.OptionalData[0]);
                            break;

                        case NodeCommand.SpecialInputChanged:
                            FireSpecialInputChanged(node, (SpecialInput)receivedFrame.OptionalData[0], (InputState)receivedFrame.OptionalData[1]);
                            break;

                        case NodeCommand.DIPChanged:
                            FireDIPChanged(node, receivedFrame.OptionalData[0]);
                            break;

                        case NodeCommand.InputChanged:
                            FireInputChanged(node, receivedFrame.OptionalData[0], (InputState)receivedFrame.OptionalData[1]);
                            break;

                        case NodeCommand.OutputLogicState:
                            FireOutputLogicChanged(node, receivedFrame.OptionalData[0], receivedFrame.OptionalData[1] != 0);
                            break;

                        case NodeCommand.OutputChanged:
                            FireOutputChanged(node, receivedFrame.OptionalData[0], receivedFrame.OptionalData[1] == 1);
                            break;

                        case NodeCommand.DSMChanged:
                            if (receivedFrame.OptionalData.Length == 2)
                            {
                                FireDSMChanged(node, (DoorEnvironmentState)receivedFrame.OptionalData[0],
                                    (DoorEnvironmentStateDetail)receivedFrame.OptionalData[1],
                                    DoorEnvironmentAccessTrigger.None);
                            }
                            else
                            {
                                FireDSMChanged(node, (DoorEnvironmentState)receivedFrame.OptionalData[0],
                                    (DoorEnvironmentStateDetail)receivedFrame.OptionalData[1],
                                    (DoorEnvironmentAccessTrigger)receivedFrame.OptionalData[2]);
                            }
                            break;

                        case NodeCommand.CrOnlineStateChanged:
#if DEBUG
                            Debug.WriteLine("CR FRAME OnlineState " + receivedFrame);
#endif

                            crComm = GetCrCommunicator(node); // GetParameter<CRNestedCommunicator>(CR_COMMUNICATOR_NODE_PARAM);

                            if (null != crComm)
                            {
                                crComm.IndirectCROnlineState(
                                    receivedFrame.OptionalData[0],
                                    receivedFrame.OptionalData[1] > 0,
                                    receivedFrame.OptionalDataLength > 9 && ((receivedFrame.OptionalData[9]) > 0),
                                    receivedFrame.OptionalData[2],
                                    receivedFrame.OptionalData[3],
                                    receivedFrame.OptionalData[4],
                                    receivedFrame.OptionalData[5],
                                    (CRHWVersion)receivedFrame.OptionalData[6],
                                    (CRBaudRate)receivedFrame.OptionalData[7],
                                    (CRBaudRate)receivedFrame.OptionalData[8],
                                    receivedFrame.OptionalDataLength > 10
                                        ? (CRLanguage)receivedFrame.OptionalData[10]
                                        : CRLanguage.English);
                            }
                            else
                                DebugHelper.NOP();

                            break;

                        case NodeCommand.CrDataReceived:
                            crComm = GetCrCommunicator(node);

                            if (crComm != null)
                            {
                                ByteDataCarrier bdc = null;
                                if (receivedFrame.OptionalData.Length > 2)
                                    bdc = new ByteDataCarrier(receivedFrame.OptionalData, 2, receivedFrame.OptionalData.Length - 2);

                                crComm.IndirectCRDataReceived(receivedFrame.OptionalData[0], receivedFrame.OptionalData[1], bdc);
                            }

                            break;
                        case NodeCommand.CrCountryCodeConfirm:

                            crComm = GetCrCommunicator(node);

                            if (crComm != null)
                                crComm.CountryCodeConfirmed(receivedFrame.OptionalData[0]);

                            break;

                        #endregion

                        default:
                            FireUnknownFrame(node, receivedFrame);
                            break;
                    }
                }
                else if (receivedFrame.Protocol == ProtocolId.ProtoUploader)
                {
                    switch (cmd)
                    {
                        #region Upgrade
                        case NodeCommand.BaseAddress:
                            int baseAddress = BitConverter.ToUInt16(receivedFrame.OptionalData, 1);

                            node.SetParameter(ParamBaseAddressNode, baseAddress);
                            if (upContext != null)
                            {
                                upContext.BaseAddress = baseAddress;
                                upContext.WaitForBaseAddress.Set();
                                DebugUpgrade("Read base address: " + upContext.BaseAddress, true);
                            }

                            break;
                        case NodeCommand.EraseOk:
                            if (triggerData != null &&
                                triggerData._clspFrame.Command == (byte)NodeCommand.ErasePage &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                var page = receivedFrame.OptionalData[1];
                                if (upContext != null && page == upContext.PageForErase)
                                {
                                    upContext.EraseAgain = false;
                                    upContext.WaitForErase.Set();
                                    upContext.ErrorCount = 0;
                                    queue.EndItemProcessing();
                                }
                            }
                            break;

                        case NodeCommand.EraseError:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.ErasePage &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {

                                var page = receivedFrame.OptionalData[1];
                                if (upContext != null && page == upContext.PageForErase)
                                {
                                    upContext.EraseAgain = true;
                                    upContext.WaitForErase.Set();
                                    upContext.ErrorCount++;
                                    queue.EndItemProcessing();
                                }
                            }
                            break;

                        case NodeCommand.ResendData:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.WriteData &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                var recBlock = BitConverter.ToUInt16(receivedFrame.OptionalData, 1);
                                if (upContext != null && recBlock == upContext.BlockNumber)
                                {
                                    upContext.DataAgain = true;
                                    upContext.WaitForReply.Set();
                                    upContext.ErrorCount++;
                                    queue.EndItemProcessing();
                                }
                            }
                            break;

                        case NodeCommand.DataWrittenOk:
                            if (triggerData != null &&
                                (NodeCommand)triggerData._clspFrame.Command == NodeCommand.WriteData &&
                                triggerData._clspFrame.OptionalData[0] == receivedFrame.OptionalData[0])
                            {
                                var recBlock = BitConverter.ToUInt16(receivedFrame.OptionalData, 1);
                                var readChecksum = BitConverter.ToUInt16(receivedFrame.OptionalData, 3);

                                if (upContext != null && recBlock == upContext.BlockNumber)
                                {
                                    if (readChecksum != upContext.PartialChecksum)
                                    {
                                        Debug.WriteLine("!!! PARTIAL CHECKSUM ERROR !!!");

                                        upContext.DataAgain = true;
                                        upContext.ErrorCount++;
                                    }
                                    else
                                    {
                                        upContext.DataAgain = false;
                                        upContext.ErrorCount = 0;
                                    }
                                    upContext.WaitForReply.Set();
                                    queue.EndItemProcessing();
                                    //DebugUpgrade("Written OK: " + upContext.BlockNumber);
                                }

                            }
                            break;
                        #endregion

                        default:
                            FireUnknownFrame(node, receivedFrame);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="transmittedFrame"></param>
        void IClspNodeEventHandler.OnFrameTransmissionFinished(IClspNode node, IClspFrame transmittedFrame)
        {
            // MAKE THIS IMPLEMENTATION AS FAST AS POSSIBLE

            if (!_useLinkLayerTransmitWaiting)
                return;

            if (node == null || transmittedFrame == null) return;

            try
            {
                NodeInfo nodeInfo;
                _nodeInfos.TryGetValue(node.LogicalAddress, out nodeInfo);

                if (nodeInfo != null)
                {
                    var procId = -1;
                    if (null != transmittedFrame.ForeignData)
                        procId = (int)transmittedFrame.ForeignData;

                    if (procId != -1 &&
                        procId == nodeInfo._actualTransmittingProcessingId)
                        nodeInfo.SetTransmittedByLinkLayer();
                    else
                        DebugHelper.NOP();
                }
                else
                    DebugHelper.NOP();


            }
            catch
            {
                DebugHelper.NOP();
            }
        }



        void ICardReaderEventHandler.CardReaderUpgradeResult(CardReader cr, CRUpgradeResult upgradeResult, Exception error)
        {
            try
            {
                if (null == CRUpgradeResult) return;
                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;
                CRUpgradeResult.Call(sn, cr, upgradeResult, error);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderUpgradeProgress(CardReader cr, int percentage)
        {
            try
            {
                if (null == CRUpgradeProgress) return;
                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;
                CRUpgradeProgress.Call(sn, cr, percentage);
            }
            catch
            {
            }
        }






        void IClspNodeEventHandler.OnNodeAssigned(IClspNode clspNode)
        {
            DebugUpgrade("NodeAssigned " + clspNode + " (uprading=" + IsNodeUpgrading(clspNode.LogicalAddress) + ")");
            CheckUpgradeProcess(clspNode.LogicalAddress); // MUST PRECEDE AddNodeInfo

            AddNodeInfo(clspNode);

            if (NodeAssigned != null)
                try
                {
                    NodeAssigned(clspNode);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
        }

        void IClspNodeEventHandler.OnNodeRenewed(IClspNode clspNode)
        {
            DebugUpgrade("Node: " + clspNode.LogicalAddress + " RENEWED (uprading=" + IsNodeUpgrading(clspNode.LogicalAddress) + ")");
            CheckUpgradeProcess(clspNode.LogicalAddress); // MUST PRECEDE AddNodeInfo
            AddNodeInfo(clspNode);

            if (NodeRenewed != null)
                try
                {
                    NodeRenewed(clspNode);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
        }

        void IClspNodeEventHandler.OnNodeReleased(IClspNode clspNode)
        {
            DebugUpgrade("NodeReleased " + clspNode + " (uprading=" + IsNodeUpgrading(clspNode.LogicalAddress) + ")");
            RemoveNodeInfo(clspNode);
            CheckUpgradeProcess(clspNode.LogicalAddress);

            if (NodeReleased != null)
                try
                {
                    NodeReleased(clspNode);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
        }

        // ReSharper disable once RedundantOverridenMember
        public void Start(string serialPortName)
        {
            _clsp.Start(serialPortName);
        }

        // ReSharper disable once RedundantOverridenMember
        public void Stop()
        {
            // calls OnNodeRelease which leads to queue clearing and NodeInfoRemoval
            _clsp.Stop();
        }


        #endregion

        public delegate void DNodeAssignmentChanged(IClspNode clspNode);

        /// <summary>
        /// 
        /// </summary>
        public event DNodeAssignmentChanged NodeAssigned;
        /// <summary>
        /// 
        /// </summary>
        public event DNodeAssignmentChanged NodeRenewed;
        /// <summary>
        /// 
        /// </summary>
        public event DNodeAssignmentChanged NodeReleased;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void RegisterCommunicatorInNode(IClspNode node)
        {
            var crComm = new CRNestedCommunicator(node, this);
            node.SetParameter(ParamCrCommunicatorNode, crComm);

            // NOT NEEDED AS NodeCommunicator implements ICardReaderEventHandler interface
            /*
            crComm.OnlineStateChanged += CardReaderOnlineStateChanged;
            crComm.CardSwiped += CardReaderCardSwiped;
            crComm.SpecialKeyPressed += CardReaderSpecialKeyPressed;
            crComm.SabotageStateChanged += CardReaderSabotageStateChanged;
            crComm.NumericKeyPressed += CardReaderNumericKeyPressed;
            crComm.MenuItemSelected += CardReaderMenuItemSelected;
            crComm.MenuCancelled += CardReaderMenuCancelled;
            crComm.MenuTimedOut += CardReaderMenuTimedOut;
            crComm.CodeSpecified += CardReaderCodeSpecified;
            crComm.CodeTimedOut += CardReaderCodeTimedOut;
            crComm.FnBoxTimedOut += CardReaderFnBoxTimedOut;

            crComm.QueryDbStampResponse += CardReaderQueryDbStampResponse;

            crComm.IndirectMessageToSend += CardReaderIndirectMessageToSend;
             */

            // NOT NEEDED AS NodeCommunicator implements ICardReaderEventHandler interface
            // crComm.UpgradeCommands.UpgradeProgress += _crCommunicator_UpgradeProgress;
            // crComm.UpgradeCommands.UpgradeResult += _crCommunicator_UpgradeResult;
        }

        private void AddNodeInfo([NotNull] IClspNode node)
        {
            NodeInfo nodeInfo;

            _nodeInfos.GetOrAddValue(node.LogicalAddress,
                out nodeInfo,
                key =>
                {
                    nodeInfo = new NodeInfo(node);
                    nodeInfo._queue.AsynchronousTimeout = _asyncTimeout;

                    //ni._queue.ProcessingBeginsAsynchronously = false;
                    nodeInfo._queue.ItemProcessing += OnSlaveNodesItemProcessing;
                    nodeInfo._queue.ItemProcessingTimedOut += OnSlaveNodesItemProcessingTimedOut;

                    if (node.MasterProtocol == ProtocolId.ProtoAccess)
                    {
                        RegisterCommunicatorInNode(node);
                    }
                    return nodeInfo;
                },
                (key, formerNodeInfo, newlyAdded) =>
                {
                    if (!newlyAdded)
                    {
                        if (formerNodeInfo._queue.ActualItem != null)
                            formerNodeInfo._queue.EndItemProcessing();
                        formerNodeInfo._queue.Clear();

                        if (node.GetParameter(ParamCrCommunicatorNode) == null)
                            RegisterCommunicatorInNode(node);
                    }


                });

            //_log.Warning("NodeCommunicator", "AddNodeInfo " + node);

            // unblock if required 
            CheckBootloader(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void RemoveNodeInfo(IClspNode node)
        {
            if (null == node)
                return;

            NodeInfo nodeInfo = null;
            _nodeInfos.Remove(
                node.LogicalAddress,
                null,
                (key, removed, nodeInfoRemoved) =>
                {
                    if (removed)
                        nodeInfo = nodeInfoRemoved;

                });

            if (nodeInfo != null)
            {
                nodeInfo.Dispose();

                try
                {
                    var crComm = node.UnsetParameter<CRNestedCommunicator>(ParamCrCommunicatorNode);
                    if (null != crComm)
                        crComm.Dispose();
                }
                catch
                {

                }
            }

            //_log.Warning("NodeCommunicator","RemoveNodeInfo " + node);

        }

        #region CardReader over SlaveNode Events

        public event Action<IClspNode, CardReader, bool> CROnlineStateChanged;

        public event Action<IClspNode, CardReader, bool> CRSabotageStateChanged;

        public event Action<IClspNode, CardReader, string, int> CRCardSwiped;

        public event Action<IClspNode, CardReader, byte[]> CRQueryDbStamp;

        public event Action<IClspNode, CardReader> CRFnBoxTimedOut;

        public event Action<IClspNode, CardReader, int> CRUpgradeProgress;
        public event Action<IClspNode, CardReader, CRUpgradeResult, Exception> CRUpgradeResult;



        /// <summary>
        /// 
        /// </summary>
        public event Action<IClspNode, CardReader> CRCodeTimedOut;
        public event Action<IClspNode, CardReader, string> CRCodeSpecified;

        public event Action<IClspNode, CardReader, int, string> CRMenuItemSelected;

        public event Action<IClspNode, CardReader, bool> CRMenuCancelled;
        public event Action<IClspNode, CardReader> CRMenuTimedOut;

        public event Action<IClspNode, CardReader, CRSpecialKey> CRSpecialKeyPressed;
        public event Action<IClspNode, CardReader, byte> CRNumericKeyPressed;

        private const int MAX_STACKED_ODL = 32;

        internal static byte[] GetCrMessageOptionalData(CRMessage message, out int optionalDataLength)
        {
            if (message.MessageCode == CRMessageCode.STACKED_MESSAGE)
            {
                if (message.OptionalData == null)
                {
                    // messages array CAN contain nulls, that the stacked header can contain length fields
                    // pointing to zero-sized buffer areas
                    // 
                    // this situation is up to parser (usually DCU) to handle

                    // [sub-message-count(1B)][length-submessage1(1B)]..[length-submessageN(1B)][data1-submessage1]..[data1-submessage1]..[dataM-submessageN]..[dataM-submessageN]


                    var bdc = new ByteDataCarrier(1 + message.SubMessages.Count, true);
                    //bdc[0] = (byte)messages.Length; // not necessary here, will be revised at the end of call

                    var sumLength = 1; // 1 stands for 1 byte of messages.Length above

                    var subMessagesCount = message.SubMessages.Count;

                    for (var i = 0; i < message.SubMessages.Count; i++)
                    {
                        if (message.SubMessages[i] == null)
                            continue;

                        int nextOdl = message.SubMessages[i].OptionalDataLength;

                        if (sumLength + nextOdl + 1 + 1 <= MAX_STACKED_ODL)
                        // +1 stands for every submessages length byte-descriptor
                        {
                            bdc[i + 1] = (byte)(nextOdl + 1); // +1 stands for encapsulated messageCode ; see below
                            sumLength += nextOdl + 2;
                        }
                        else
                        {
                            subMessagesCount = i;
                            break;
                        }
                    }

                    bdc[0] = (byte)subMessagesCount;
                    bdc.ActualSize = 1 + subMessagesCount; // 1 stands for 1 byte of messages.Length above

                    for (var i = 0; i < subMessagesCount; i++)
                    {
                        if (message.SubMessages[i] == null)
                            continue;

                        bdc.Append(new[] { (byte)message.SubMessages[i].MessageCode }, true); // encapsulated messageCode
                        bdc.Append(message.SubMessages[i].OptionalData, true);
                    }

                    //if (bdc.ActualSize >= 6 && bdc.IndexOf(new[] {(byte) 0x61}) >= 0)
                    //    DebugHelper.NOP(bdc);

                    message.CacheOptionalData(bdc.Buffer);

                    optionalDataLength = bdc.ActualSize;

                    return bdc.Buffer;
                }

            }

            optionalDataLength = message.OptionalDataLength;

            return message.OptionalData;
        }

        void ICardReaderEventHandler.CardReaderIndirectMessageToSend(CardReader cr, CRMessage message, bool highPriority)
        {


            try
            {
                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                var bdcFinalBinary = new ByteDataCarrier(4, true);
                bdcFinalBinary[0] = NodeFrame.SequenceNumber.Singleton.GetNextNumber();
                bdcFinalBinary[1] = cr.Address;
                bdcFinalBinary[2] = (byte)message.MessageCode;
                bdcFinalBinary[3] = (byte)(highPriority ? 1 : 0);

                int crMessageOdl;
                bdcFinalBinary.Append(GetCrMessageOptionalData(message, out crMessageOdl), true);


                var f = ClspFrame.Create(ProtocolId.ProtoAccess, (byte)NodeCommand.CrRequest, bdcFinalBinary.ToByteArray());

                EnqueueFrame(sn.LogicalAddress, f);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderFnBoxTimedOut(CardReader cr)
        {
            try
            {
                if (null == CRFnBoxTimedOut) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;
                CRFnBoxTimedOut.Call(sn, cr);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderCodeTimedOut(CardReader cr)
        {
            try
            {
                if (null == CRCodeTimedOut) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;
                CRCodeTimedOut.Call(sn, cr);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderCodeSpecified(CardReader cr, string codeData)
        {
            try
            {


                if (CRCodeSpecified != null)
                {
                    var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;
                    CRCodeSpecified.Call(sn, cr, codeData);
                }
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderMenuCancelled(CardReader cr, bool byOtherCommand)
        {
            try
            {
                if (null == CRMenuCancelled) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRMenuCancelled.Call(sn, cr, byOtherCommand);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderMenuTimedOut(CardReader cr)
        {
            try
            {
                if (null == CRMenuTimedOut) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRMenuTimedOut.Call(sn, cr);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderMenuItemSelected(CardReader cr, int itemIndex, string itemText)
        {
            try
            {
                if (CRMenuItemSelected == null) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRMenuItemSelected.Call(sn, cr, itemIndex, itemText);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderNumericKeyPressed(CardReader cr, byte numeric)
        {
            try
            {
                if (CRNumericKeyPressed == null) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRNumericKeyPressed.Call(sn, cr, numeric);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderSabotageStateChanged(CardReader cr, bool tamperOn)
        {
            try
            {
                if (CRSabotageStateChanged == null) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRSabotageStateChanged.Call(sn, cr, tamperOn);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderSpecialKeyPressed(CardReader cr, CRSpecialKey specialKey)
        {
            try
            {
                if (null == CRSpecialKeyPressed) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRSpecialKeyPressed.Call(sn, cr, specialKey);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderOnlineStateChanged(CardReader cr, bool isOnline)
        {
            try
            {
                if (CROnlineStateChanged == null) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CROnlineStateChanged.Call(sn, cr, isOnline);


            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderCardSwiped(CardReader cr, string cardData, int cardSystemNumber)
        {
            try
            {
                if (CRCardSwiped == null) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRCardSwiped.Call(sn, cr, cardData, cardSystemNumber);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderQueryDbStampResponse(CardReader cr, byte[] queryDbStamp)
        {
            try
            {
                if (CRQueryDbStamp == null) return;

                var sn = (ClspNode)cr.ParentCommunicator.IndirectParentObject;

                CRQueryDbStamp.Call(sn, cr, queryDbStamp);
            }
            catch
            {
            }
        }

        void ICardReaderEventHandler.CardReaderResetOccured(CardReader cr)
        {

        }

        void ICardReaderEventHandler.CardReaderCommandFailed(CardReader cr, CRMessageCode messageCode)
        {

        }

        void ICardReaderEventHandler.CardReaderCommandTimedOut(CardReader cr, CRMessageCode messageCode, int retryToCome)
        {

        }

        void ICardReaderEventHandler.CardReaderServiceResultOccured(CardReader cr, CRServiceResult serviceResult)
        {

        }

        void ICardReaderEventHandler.CardReaderConfigurationResponse(CardReader cr, CRConfigurationResult ackResult, byte dbStamp)
        {

        }

        void ICardReaderEventHandler.CardReaderCountryCodeConfirmed(CardReader cr)
        {

        }

        void ICardReaderEventHandler.CardReaderGraphicalMenuUpdated(CardReader cr)
        {

        }

        void ICardReaderEventHandler.CardReaderModeChanged(CardReader cr, CRMode readerMode)
        {
        }

        void ICardReaderEventHandler.CardReaderCardWriterResponse(CardReader cr, MifareCardWriteResult writeResult, MifareCardWriteError? writeError)
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public bool ClspHwcEnablePolling
        {
            get { return _clsp.ClspHwcEnablePolling; }
            set { _clsp.ClspHwcEnablePolling = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NodeResponseTimeout
        {
            get { return _clsp.NodeResponseTimeout; }
            set { _clsp.NodeResponseTimeout = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NodeResponseLatency
        {
            get { return _clsp.NodeResponseLatency; }
            set { _clsp.NodeResponseLatency = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NodeUnpolledTimeout
        {
            get { return _clsp.NodeUnpolledTimeout; }
            set { _clsp.NodeUnpolledTimeout = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DelayBetweenSuccessfulPolling
        {
            get { return _clsp.DelayBetweenSuccessfulPolling; }
            set { _clsp.DelayBetweenSuccessfulPolling = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte MaxNodeLookupSequence
        {
            get { return _clsp.MaxNodeLookupSequence; }
            set { _clsp.MaxNodeLookupSequence = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicalAddres"></param>
        /// <returns></returns>
        public IClspNode GetNode(byte logicalAddres)
        {
            return _clsp.GetNode(logicalAddres);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<IClspNode> GetNodes()
        {
            return _clsp.GetNodes();
        }

        /// <summary>
        /// 
        /// </summary>
        public int GlobalMaxRetryCount
        {
            get { return _clsp.GlobalMaxRetryCount; }
            set { _clsp.GlobalMaxRetryCount = value; }
        }

        #region ICardReaderEventHandler Members


        void ICardReaderEventHandler.CardReaderMenuItemSelected(CardReader cr, int itemReturnCode)
        {
            // TODO
        }

        void ICardReaderEventHandler.CardReaderConfigurationResponse(CardReader cr, CRConfigurationResult result, uint crUniqueId, ushort configId)
        {
            // TODO
        }

        #endregion
    }
}
