using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;

using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// class for medium-latency message transfer between TCP peers ; 
    /// if peer1 has connection to peer2 , peer2 would not need to establish
    /// a new connection back if message is to be transferred back to peer1
    /// </summary>
    public class MessagingPeer
    {
        private const int DEFAULT_SOCKET_BUFFER_SIZE = 32768;
        private const int DEFAULT_LISTEN_BACKLOG = 16;

        #region Protocol definitions

        private const byte PROTO_VERSION_1 = 0x01;
        private const byte PROTO_VERSION = 0x02;

        private static readonly byte[] PREAMBLE = { 0xFA, 0xAF };

#region Binary protocol definitions
        private const int PROTO_PART_LENGTH = 1;
        private const int CMD_PART_LENGTH = 1;
        private const int MID_PART_LENGTH = sizeof(uint);
        private const int ODL_PART_LENGTH = 2;
        
        // proto(1B), cmd(1B), messageId(4B), cmd-param-length (2B)
        private const int HEADER_LENGTH = PROTO_PART_LENGTH + CMD_PART_LENGTH + MID_PART_LENGTH + ODL_PART_LENGTH;
        private const int HEADER_LENGTH_1 = PROTO_PART_LENGTH + CMD_PART_LENGTH + ODL_PART_LENGTH;

#endregion

        /// <summary>
        /// 
        /// </summary>
        internal enum LowLevelCommand : byte
        {
            Unknown = 0xF0,
            // should be in condition of LowLevelMessage.ParseRawData
            BinaryMessage = 0x01,

            // should be in condition of LowLevelMessage.ParseRawData
            TextMessage = 0x02,

            // should be in condition of LowLevelMessage.ParseRawData
            // polyformed custom application message implemented as binary message on this layer
            ApplicationMessage = 0x03,

            // should be in condition of LowLevelMessage.ParseRawData
            KeepAlive = 0x10,

            // should be in condition of LowLevelMessage.ParseRawData
            //TransportAck = 0x11,
        }

        /// <summary>
        /// 
        /// </summary>
        internal class LowLevelMessage
        {
            protected readonly LowLevelCommand _command;
            public LowLevelCommand Command { get { return _command; } }
            internal int _optionalDataLength = -1;
            internal bool _partial = false;

            // sequence number are global, not per connection
            // based on statistical span
            private volatile uint _messageId;

            /// <summary>
            /// 
            /// </summary>
            public uint MessageId
            {
                get
                {
                    return _messageId;
                }
            }

            private static volatile uint _messageIdIterator = 0;
            private static readonly object _miiSync = new object();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="command"></param>
            /// <param name="generateNewId"></param>
            protected LowLevelMessage(LowLevelCommand command,bool generateNewId)
            {
                _command = command;

                if (generateNewId)
                    lock (_miiSync)
                    {
                        _messageIdIterator++;
                        if (_messageIdIterator == 0)
                            _messageIdIterator = 1;

                        _messageId = _messageIdIterator;
                    }
                
            }

            protected internal ByteDataCarrier _rawParameters;

            internal ByteDataCarrier PrepareHeader(int optionalDataLength)
            {
                var rawHeader = new ByteDataCarrier(PREAMBLE.Length + HEADER_LENGTH, false);
                rawHeader.Append(PREAMBLE);

                //int pos = PREAMBLE.Length;

                var asPos = rawHeader.ActualSize;
                rawHeader.ActualSize = asPos + PROTO_PART_LENGTH + CMD_PART_LENGTH;

                rawHeader[asPos+0] = PROTO_VERSION;
                rawHeader[asPos+1] = (byte)_command;

                rawHeader.Append(BitConverter.GetBytes(_messageId));

                rawHeader.Append(BitConverter.GetBytes((UInt16)optionalDataLength), true);

                return rawHeader;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal virtual ByteDataCarrier GetRawData()
            {
                return PrepareHeader(0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="offset"></param>
            /// <param name="length"></param>
            /// <param name="finishedPosition"></param>
            /// <returns>LowLevelMessage child, if able to parse</returns>
            internal static LowLevelMessage ParseRawData(
                byte[] data, 
                int offset, 
                int length,
                out int finishedPosition)
            {
                finishedPosition = ByteDataCarrier.IndexOf(data, offset, length, PREAMBLE);
                if (finishedPosition < 0)
                    return null;

                // it still can happen , that raw data would contain PREAMBLE, however
                // further conditions should eliminate that situation 

                finishedPosition += PREAMBLE.Length;
                

                if (finishedPosition + PROTO_PART_LENGTH > length)
                    // incomplete header
                    return null;

                var protoVersion = data[finishedPosition];

                int headerLength;

                switch (protoVersion)
                {
                    case PROTO_VERSION:
                        headerLength = HEADER_LENGTH;
                        break;
                    case PROTO_VERSION_1:
                        headerLength = HEADER_LENGTH_1;
                        break;
                    default:
                        // unsupported protocol
                        // further conditioning will be added when protocol evolves
                        return null;

                }

                if (finishedPosition + headerLength > length)
                    // incomplete header - should not happen on separate connections
                    // as every previous message should be confirmed/ack or not ack
                    // thus not even fragmentation should be relevant here
                    return null;

                var command = data[finishedPosition + PROTO_PART_LENGTH];
                var llc = (LowLevelCommand)command;
                // verification of the command validity
                // in case in raw data PREAMBLE will be found (non-zero probability)
                switch (llc)
                {
                    case LowLevelCommand.BinaryMessage:
                    case LowLevelCommand.KeepAlive:
                    case LowLevelCommand.TextMessage:
                    case LowLevelCommand.ApplicationMessage:
                    //case LowLevelCommand.TransportAck:
                        break;
                    default:
                        return null;
                }

                // rely on IPv4 and/or TCP checksumming capabilities
                UInt16 odl;
                uint messageId = 0;

                switch (protoVersion)
                {
                    case PROTO_VERSION:
                        messageId = BitConverter.ToUInt32(data, finishedPosition + + PROTO_PART_LENGTH + CMD_PART_LENGTH);
                        odl = BitConverter.ToUInt16(data, finishedPosition + PROTO_PART_LENGTH + CMD_PART_LENGTH + MID_PART_LENGTH);
                        break;
                    case PROTO_VERSION_1:
                        odl = BitConverter.ToUInt16(data, finishedPosition + PROTO_PART_LENGTH + CMD_PART_LENGTH);
                        break;
                    default:
                        return null;
                }
                

                ByteDataCarrier rawParams = null;
                var partial = false;

                if (odl > 0)
                {
                    rawParams = new ByteDataCarrier(data, finishedPosition + headerLength, odl);
                    partial = rawParams.Length < odl;
                    finishedPosition += headerLength + rawParams.Length;
                }
                else
                {
                    odl = 0;
                    finishedPosition += headerLength;
                }



                LowLevelMessage llm;

                switch (llc)
                {
                    case LowLevelCommand.BinaryMessage:
                        llm = new LLBinaryMessage(true,rawParams);
                        break;
                    case LowLevelCommand.KeepAlive:
                        llm = new LLKeepAliveMessage(rawParams);
                        break;
                    case LowLevelCommand.TextMessage:
                        llm = new LLTextMessage(rawParams);
                        break;
                    //case LowLevelCommand.TransportAck:
                        //llm = new LLTransportAckMessage();
                        //break;
                    case LowLevelCommand.ApplicationMessage:
                        llm = new LLApplicationMessage(rawParams);
						break;
                    default:
                        return null;
                }

                llm._messageId = messageId;
                llm._optionalDataLength = odl;
                llm._partial = partial;

                return llm;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            internal static LowLevelMessage ParseRawData(ByteDataCarrier data)
            {
                int finishedPosition;
                var llm = ParseRawData(data.Buffer, data.Offset, data.ActualSize, out finishedPosition);

                if (llm != null)
                    data.Offset = finishedPosition;

                return llm;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            internal void AppendRemainingData(ByteDataCarrier data)
            {
                _rawParameters.Append(data);
                _partial = _rawParameters.Length < _optionalDataLength;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        internal class LLKeepAliveMessage : LowLevelMessage
        {
            private readonly uint[] _messageIdsToAck;

            /// <summary>
            /// 
            /// </summary>
            public uint[] MessageIdIdsToAck
            {
                get
                {
                    return _messageIdsToAck;
                }
            }

            internal LLKeepAliveMessage(params uint[] messageIdsToAck)
                : base(LowLevelCommand.KeepAlive,true)
            {
                _messageIdsToAck = messageIdsToAck;
            }

            internal LLKeepAliveMessage(ByteDataCarrier rawParams)
                : base(LowLevelCommand.KeepAlive,false)
            {
                _rawParameters = rawParams;

                if (null != rawParams && rawParams.Length > 0)
                {
                    var m2aCount = (rawParams.ActualSize/ConnectionInfo.MID2ACK_SIZEOF);
                    if (m2aCount > 0 && m2aCount <= ConnectionInfo.MID2ACK_MAX_COUNT)
                    {
                        _messageIdsToAck = new uint[m2aCount];

                        for (var i = 0; i < m2aCount; i++)
                        {
                            _messageIdsToAck[i] = BitConverter.ToUInt32(rawParams.Buffer,
                                i*ConnectionInfo.MID2ACK_SIZEOF);
                        }
                    }
                }
            }

            internal override ByteDataCarrier GetRawData()
            {
                var odl = 0;

                if (_messageIdsToAck != null)
                {
                    // will do UTF8 conversion
                    _rawParameters = new ByteDataCarrier(_messageIdsToAck.Length * ConnectionInfo.MID2ACK_SIZEOF,true);

                    ByteConverter.ToBytes(_rawParameters.Buffer, 0, _messageIdsToAck);

                    odl = _rawParameters.ActualSize;
                }
                else
                    _rawParameters = null;

                var headerBdc = PrepareHeader(odl);
                if (_rawParameters != null)
                    headerBdc.Append(_rawParameters);

                return headerBdc;
            }


        }

        /*
        /// <summary>
        /// 
        /// </summary>
        internal class LLTransportAckMessage : LowLevelMessage
        {
            internal LLTransportAckMessage()
                : base(LowLevelCommand.TransportAck)
            {
            }
        }*/

        /// <summary>
        /// 
        /// </summary>
        private class LLBinaryMessage : LowLevelMessage
        {
            /*
            private LLBinaryMessage()
                : base(LowLevelCommand.BinaryMessage,true)
            {
            }*/

            /// <summary>
            /// parsing constructor
            /// </summary>
            /// <param name="isParsing"></param>
            /// <param name="rawParams"></param>
            internal LLBinaryMessage(bool isParsing, ByteDataCarrier rawParams)
                : base(LowLevelCommand.BinaryMessage,!isParsing)
            {
                _rawParameters = rawParams;
            }

            /*
            /// <summary>
            /// message with binary data instantiation
            /// </summary>
            /// <param name="data"></param>
            public LLBinaryMessage(byte[] data)
                : this()
            {
                _rawParameters = new ByteDataCarrier(data, 0, data.Length);
            }*/

            /// <summary>
            /// binary data carried by the message
            /// </summary>
// ReSharper disable once UnusedMember.Local
            public byte[] Data
            {
                get { return _rawParameters.Buffer; }
            }

            /// <summary>
            /// 
            /// </summary>
            public ByteDataCarrier BinaryDataCarrier
            {
                get { return _rawParameters; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override ByteDataCarrier GetRawData()
            {
                var odl = _rawParameters.ActualSize;

                var headerBdc = PrepareHeader(odl);
                headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class LLTextMessage : LowLevelMessage
        {
            private LLTextMessage()
                : base(LowLevelCommand.TextMessage,true)
            {
            }

            private readonly string _textData;

            /// <summary>
            /// parsing constructor
            /// </summary>
            /// <param name="rawParams"></param>
            internal LLTextMessage(ByteDataCarrier rawParams)
                : base(LowLevelCommand.TextMessage,false)
            {
                _rawParameters = rawParams;

                if (rawParams != null && rawParams.ActualSize > 0)
                {
                    _textData = rawParams.GetUTF8String();
                }
                else
                    _textData = String.Empty;
            }

            /// <summary>
            /// message with binary data instantiation
            /// </summary>
            /// <param name="textData"></param>
            public LLTextMessage(string textData)
                : this()
            {
                _textData = textData ?? String.Empty;
            }

            /// <summary>
            /// binary data carried by the message
            /// </summary>
            public string TextData
            {
                get { return _textData; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override ByteDataCarrier GetRawData()
            {
                var odl = 0;

                if (_textData != null)
                {
                    // will do UTF8 conversion
                    _rawParameters = new ByteDataCarrier(_textData);

                    odl = _rawParameters.ActualSize;
                }
                else
                    _rawParameters = null;

                var headerBdc = PrepareHeader(odl);
                if (_rawParameters != null)
                    headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        private class LLApplicationMessage : LowLevelMessage
        {
            /*
            private LLApplicationMessage()
                : base(LowLevelCommand.ApplicationMessage)
            {
            }*/

            private readonly UInt32 _applicationMessageType;

            /// <summary>
            /// 
            /// </summary>
// ReSharper disable once MemberCanBePrivate.Local
            internal UInt32 ApplicationMessageType
            {
                  get { return _applicationMessageType; }
            }

            private const int AMT_BYTE_SIZE = 4;

            /// <summary>
            /// parsing constructor
            /// </summary>
            /// <param name="rawParams"></param>
            internal LLApplicationMessage(ByteDataCarrier rawParams)
                : base(LowLevelCommand.ApplicationMessage,false)
            {
                _applicationMessageType = BitConverter.ToUInt32(rawParams.Buffer,0);

                _rawParameters = new ByteDataCarrier(rawParams.Buffer,AMT_BYTE_SIZE, rawParams.ActualSize - AMT_BYTE_SIZE);

            }

            private IApplicationMessage _applicationMessage;
            internal IApplicationMessage ApplicationMessage
            {
                get { return _applicationMessage; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="messagingPeer"></param>
            /// <param name="messageId"></param>
            /// <returns></returns>
            internal bool ParseApplicationMessage(MessagingPeer messagingPeer,uint messageId)
            {
                if (null == messagingPeer)
                    return false;

                _applicationMessage =  messagingPeer.ParseApplicationMessage(ApplicationMessageType, messageId,_rawParameters);
                return (_applicationMessage != null);
            }


            internal LLApplicationMessage(IApplicationMessage message)
                :base (LowLevelCommand.ApplicationMessage,true)
            {
                _applicationMessageType = message.ApplicationMessageFactory.ApplicationMessageType;

                _applicationMessage = message;
            }

            /*
            internal ByteDataCarrier RawApplicationData
            {
                get { return _rawParameters; }
            }*/
            
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override ByteDataCarrier GetRawData()
            {
                var odl = AMT_BYTE_SIZE;
                if (ReferenceEquals(_rawParameters , null))
                {
                    // if this is null, usually it means the LLApplicationMessage was created with IApplicationMessage parameter
                    if (_applicationMessage != null)
                        _rawParameters = _applicationMessage.GetRawApplicationData();
                    else
                        // inconsistent situation; _applicationMessage should be filled
                        DebugHelper.NOP();
                }

                if (_rawParameters != null)
                    odl += _rawParameters.ActualSize;

                var headerBdc = PrepareHeader(odl);

                var amtBytes = BitConverter.GetBytes(_applicationMessageType);

                headerBdc.Append(amtBytes,true);

                if (!ReferenceEquals(_rawParameters, null))
                    headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        #endregion

        #region Application message 
        /// <summary>
        /// 
        /// </summary>
        public interface IApplicationMessage
        {
            /// <summary>
            /// in the implementation , static storage of this value is suggested
            /// </summary>
            IApplicationMessageFactory ApplicationMessageFactory { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            ByteDataCarrier GetRawApplicationData();

            /// <summary>
            /// 
            /// </summary>
            uint MessageId { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public interface IApplicationMessageFactory
        {
            /// <summary>
            /// in the implementation , static storage of this value is suggested
            /// </summary>
            UInt32 ApplicationMessageType { get; }

            /// <summary>
            /// for creating the message from the prototype instance
            /// </summary>
            /// <returns></returns>
            IApplicationMessage CreateNew(uint messageId,ByteDataCarrier rawApplicationData);
        }

        private volatile SyncDictionary<UInt32, IApplicationMessageFactory> _applicationMessageTypes;
        private readonly object _amtSync = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationMessageFactory"></param>
        public void RegisterApplicationMessageType(
            [NotNull] IApplicationMessageFactory applicationMessageFactory)
        {
            Validator.CheckForNull(applicationMessageFactory,"applicationMessageFactory");

            // to make the instantiation on demand
            if (_applicationMessageTypes == null)
                lock (_amtSync)
                {
                    if (_applicationMessageTypes == null)
                        _applicationMessageTypes = new SyncDictionary<uint, IApplicationMessageFactory>();
                }

            _applicationMessageTypes.Add(applicationMessageFactory.ApplicationMessageType, applicationMessageFactory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationMessageType"></param>
        /// <param name="messageId"></param>
        /// <param name="rawApplicationData"></param>
        /// <returns></returns>
        private IApplicationMessage ParseApplicationMessage(UInt32 applicationMessageType, uint messageId, ByteDataCarrier rawApplicationData)
        {
            if (null == _applicationMessageTypes)
                return null;

            IApplicationMessageFactory instanceFactory;
            if (_applicationMessageTypes.TryGetValue(applicationMessageType, out instanceFactory) &&
                instanceFactory != null)
            {
                return instanceFactory.CreateNew(messageId, rawApplicationData);
            }
            return null;
        }
        #endregion

        private volatile SimpleTcpServer _tcpListener = null;
        private readonly object _tcpListenerLock = new object();

        private int _peerPort;

        /*
        /// <summary>
        /// implicit constructor
        /// </summary>
        public MessagingPeer()
        {

        }*/

        /// <summary>
        /// starts listening on the specific IP and TCP port
        /// </summary>
        /// <param name="bindToIp">if null, IPAddress.Any would be used</param>
        /// <param name="peerPort">TCP port specification ; all communicating messaging peer should have this very same port</param>
        /// <exception cref="InvalidTransportPortException">if peerPort is out of range 1-65535</exception>
        public void Start(IPAddress bindToIp, int peerPort)
        {
            TcpUdpPort.CheckValidity(peerPort, false);

            var newlyCreated = false;
            if (null == _tcpListener)
            {
                lock (_tcpListenerLock)
                {
                    if (_tcpListener == null)
                    {
                        _tcpListener = new SimpleTcpServer();
                        newlyCreated = true;
                    }
                }
            }

            if (newlyCreated)
            {

                _tcpListener.DataReceived +=OnConnectionDataReceived;
                _tcpListener.Connected += OnClientConnected;
                _tcpListener.Disconnected += OnClientDisconnected;

                _tcpListener.BufferSize = DEFAULT_SOCKET_BUFFER_SIZE;

                if (null == bindToIp) 
                    bindToIp = IPAddress.Any;

                //_onlyAllowedIPs = onlyAllowedIPs;

                _tcpListener.Start(bindToIp, peerPort, null, DEFAULT_LISTEN_BACKLOG);

                _peerPort = peerPort;

                // sending queue started later than a tcpListener start can raise exception (e.g. port already used)
                _sendingPq = new ProcessingQueue<MPDataCarrier>(new object[]{},
                    PQPickQueueMode.RoundRobin,
                    false,
                    ThreadPriority.Normal,
                    "MessagingPeer" + peerPort);
                _sendingPq.ItemProcessing += OnSendProcessing;

                //_keepaliveLowPrioThread = 
                    SafeThread.StartThread(MaintenanceAndKeepaliveThread);
            }
        }

        //private SafeThread _keepaliveLowPrioThread = null;

        // this interval has to be rather short, as it affects how long
        // the other messages would need to wait in case this message is pending
        private const int WAITING_FOR_CONNECT_TIMEOUT = 1000;

        /// <summary>
        /// verifies, if the the maxRetries condition does not apply , 
        /// and if not, re-enqueues the data carrier
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="dataCarrier"></param>
        /// <param name="calledViaPollingMaintenanceThread"></param>
        /// <returns></returns>
        private void ValidateDataCarrierRetry(ConnectionInfo connectionInfo, MPDataCarrier dataCarrier,bool calledViaPollingMaintenanceThread)
        {
            if (dataCarrier._lowLevelMessage.Command == LowLevelCommand.KeepAlive)
                return;

            // can be DateTime even in CF environment, as it's used only for differential counting of timeout
            var now = DateTime.Now;

            // counted even for infinite retrying , especially for calledViaPollingMaintenanceThread variant
            var timeFrom1stProcessing = now - dataCarrier._1stProcessingTimeStamp;

            var timeFromLastProcessing = now - dataCarrier._lastProcessingTimeStamp;

            var evaluateAsPossibleTimeout = 
                !calledViaPollingMaintenanceThread || 
                // to avoid re-enqueue, when processed recently
                timeFromLastProcessing.TotalMilliseconds > BASE_KEEPALIVE_PERIOD*3; // as the maintenance thread is sending keepalives and also checking the timeouts

            Debug.WriteLine(
                "MP/ "+
                dataCarrier._peerIPe+" "+
                dataCarrier._lowLevelMessage.MessageId + " "
                + dataCarrier._lowLevelMessage.Command + " diff=" +
                timeFrom1stProcessing + " evalAsTimeout="+evaluateAsPossibleTimeout);


            if (evaluateAsPossibleTimeout && dataCarrier._timeout != default(TimeSpan) &&
                timeFrom1stProcessing > dataCarrier._timeout)
            {
                // raise the amount of failures
                if (dataCarrier._connectionInfo != null)
                    dataCarrier._connectionInfo.IncrementMessageTimeoutCount();
                else
                    DebugHelper.NOP(dataCarrier);

                //Debug.WriteLine(" TIMED-OUT");
                connectionInfo.MessagesAcknowledgedOrRemoved(false,dataCarrier._lowLevelMessage.MessageId);

                InvokeMessageRetryingFailed(dataCarrier);
            }
            else
            {

                // re-enqueue at the end of the queue
                if (evaluateAsPossibleTimeout)
                {
                    var itemCarrier = _sendingPq.EnqueueByKey(
                        dataCarrier._peerIPe.Address,
                        dataCarrier,
                        PQEnqueueFlags.OnTop,
                        0);

                    DebugHelper.Keep(itemCarrier);
                    //Debug.WriteLine(" RETRIED");
                }
                //else
                //{
                //    Debug.WriteLine(" NOTHING");
                //}
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataCarrier"></param>
        private void InvokeMessageRetryingFailed(MPDataCarrier dataCarrier)
        {
            switch (dataCarrier._lowLevelMessage.Command)
            {
                case LowLevelCommand.BinaryMessage:


                    if (BinaryMessageRetryingFailed != null)
                        try
                        {
                            var llbm = (LLBinaryMessage) dataCarrier._lowLevelMessage;

                            BinaryMessageRetryingFailed(this, dataCarrier._peerIPe, llbm.BinaryDataCarrier);
                        }
                        catch
                        {
                        }
                    break;
                case LowLevelCommand.TextMessage:
                    if (TextMessageRetryingFailed != null)
                        try
                        {
                            var lltm = (LLTextMessage) dataCarrier._lowLevelMessage;

                            TextMessageRetryingFailed(this, dataCarrier._peerIPe, lltm.TextData);
                        }
                        catch
                        {
                        }
                    break;
                case LowLevelCommand.ApplicationMessage:
                    if (ApplicationMessageRetryingFailed != null)
                    {
                        try
                        {
                            var llam = (LLApplicationMessage) dataCarrier._lowLevelMessage;

                            ApplicationMessageRetryingFailed(this, dataCarrier._peerIPe, llam.ApplicationMessage);
                        }
                        catch
                        {
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataCarrier"></param>
        void OnSendProcessing(MPDataCarrier dataCarrier)
        {
            if (dataCarrier._aimedToBeRemoved)
                return;

            Debug.WriteLine(DateTime.Now+" MP/ >>> "+
                dataCarrier._peerIPe+" "+
                dataCarrier._lowLevelMessage.MessageId+" "+dataCarrier._lowLevelMessage.Command);

            var ci = GetConnection(dataCarrier._peerIPe);

            if (ci != null && dataCarrier._lowLevelMessage.Command != LowLevelCommand.KeepAlive)
                DebugHelper.NOP();

            if (ci == null || ci.AimedToBeDisposed || 
                (ci._client !=null && ci._connection == null))
            {
                if (dataCarrier._lowLevelMessage.Command == LowLevelCommand.KeepAlive
                    //|| dataCarrier._lowLevelMessage.Command == LowLevelCommand.TransportAck
                    )
                    // Keepalive and TransportAck request should not revive a connection
                    return;

                Exception connectError = null;

                if (null == ci)
                {

                    ci = new ConnectionInfo(true)
                    {
                        _peerIPe = dataCarrier._peerIPe,
                        _client = new SimpleTcpClient(),
                    };


                    // add the connection info even if in connection stage to avoid multi-creation of the very same thread
                    AddRemovePeerConnection(ci, true);


                    ci._client.DataReceived += OnConnectionDataReceived;

                    ci._client.ConnectionFailed += delegate(IPEndPoint ipEndpoint, Exception connection)
                    {
                        Debug.WriteLine("Connect failed "+ipEndpoint);
                        connectError = connection;
                    };

                    ci._client.Disconnected += OnClientDisconnected;
                }

                bool connectedYet;
                try
                {
                    ci._client.Connect(ci._peerIPe, null);
                    connectedYet = ci._client.WaitForConnect(WAITING_FOR_CONNECT_TIMEOUT);
                }
                catch (InvalidOperationException)
                {
                    connectedYet = true;
                }

                

                if (connectedYet)
                {
                    if (connectError != null)
                        connectedYet = false;    
                    else
                        ci._connection = ci._client.SimpleTcpConnection;
                }

                if (!connectedYet)
                {
                    //this validates the timeout of message even for connect
                    PrepareForValidation(dataCarrier, ci);

                    ValidateDataCarrierRetry(ci, dataCarrier,false);

                    return;
                }
            }

            PrepareForValidation(dataCarrier, ci);

            if (ci._connection != null)
            {

                try
                {
                    var bdc = dataCarrier._lowLevelMessage.GetRawData();

                    // throws exception in case of a problem
                    ci._connection.Send(bdc);

                    if (dataCarrier._lowLevelMessage.Command != LowLevelCommand.KeepAlive)
                        ci.AddOutgoingMessageToAck(dataCarrier);
                }
                catch (Exception)
                {
                    if (!ci.AimedToBeDisposed)
                        ValidateDataCarrierRetry(ci, dataCarrier, false);
                }

            }
#if DEBUG
            else
            {
                DebugHelper.TryBreak("MessagingPeer ci._connection shouldn't be null",ci);
            }
#endif

        }

        private static void PrepareForValidation(MPDataCarrier dataCarrier, ConnectionInfo ci)
        {
// marking the time for measuring the timeout for retries
            if (dataCarrier._1stProcessingTimeStamp == default(DateTime))
                // can be DateTime even in CF environment, as it's used only for differential counting of timeout
                dataCarrier._1stProcessingTimeStamp = DateTime.Now;

            dataCarrier._lastProcessingTimeStamp = DateTime.Now;

            // connection info necessary for ValidateCarrierRetry
            if (dataCarrier._connectionInfo == null)
                dataCarrier._connectionInfo = ci;
        }

        //private const int RECONNECT_GRACEFUL_TIMEOUT = 5000;

        /// <summary>
        /// 
        /// </summary>
        private class ConnectionInfo : ADisposable
        {
// ReSharper disable once MemberCanBePrivate.Local
// ReSharper disable once NotAccessedField.Local
            internal readonly bool _initiator;

            internal ConnectionInfo(bool initiator)
            {
                _initiator = initiator;
            }

            internal volatile ISimpleTcpConnection _connection = null;
            internal volatile SimpleTcpClient _client = null;

            internal IPEndPoint _peerIPe = null;

            private readonly SyncDictionary<uint, MPDataCarrier> _outgoingMessageIdsToAck =
                new SyncDictionary<uint, MPDataCarrier>();

            internal DateTime _lastSentAck = default(DateTime);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="returnRemoved"></param>
            /// <param name="messageIds"></param>
            internal IEnumerable<MPDataCarrier> MessagesAcknowledgedOrRemoved(
                bool returnRemoved,
                params uint[] messageIds)
            {
                if (messageIds == null ||
                    messageIds.Length == 0)
                    return null;

                LinkedList<MPDataCarrier> removed = null;
                if (returnRemoved)
                    removed = new LinkedList<MPDataCarrier>();

                foreach (var mid in messageIds)
                {
                    MPDataCarrier dataCarrier;
                    if (_outgoingMessageIdsToAck.Remove(mid, out dataCarrier))
                    {
                        dataCarrier._aimedToBeRemoved = true;
                        if (returnRemoved)
                            removed.AddLast(dataCarrier);
                    }
                }

                return removed;

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataCarrier"></param>
            internal void AddOutgoingMessageToAck(MPDataCarrier dataCarrier)
            {
                if (ReferenceEquals(dataCarrier, null) ||
                    ReferenceEquals(dataCarrier._lowLevelMessage, null) ||
                    dataCarrier._lowLevelMessage.MessageId == 0)
                    return;

                
                _outgoingMessageIdsToAck[dataCarrier._lowLevelMessage.MessageId] = dataCarrier;
                
            }

            internal IEnumerable<MPDataCarrier> MPDataCarrierSnapshot
            {
                get
                {
                    return _outgoingMessageIdsToAck.GetValuesSnapshot(true);
                }
            }

            private readonly object _mtcSync = new object();
            private int _messageTimeoutCount = 0;
            internal void IncrementMessageTimeoutCount()
            {
                lock (_mtcSync)
                {
                    _messageTimeoutCount++;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>true, if MessageTimeoutCount was non zero</returns>
            internal bool NullMessageTimeoutCount()
            {

                bool wasNonZero;

                lock (_mtcSync)
                {
                    wasNonZero = _messageTimeoutCount > 0;

                    _messageTimeoutCount = 0;
                }

                return wasNonZero;
            }

            internal volatile LowLevelMessage _partialMessage = null;

            /// <summary>
            /// 
            /// </summary>
            internal void Disconnect()
            {
                _partialMessage = null;

                if (_connection != null)
                    try
                    {
                        _connection.Disconnect();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _connection = null;
                    }
            }

            protected override void InternalDispose(bool isExplicitDispose)
            {
                Disconnect();
            }

            private readonly LinkedList<uint> _incomingMessageIdsToAck = new LinkedList<uint>();

            internal void AddIncomingMessageIdToAck(uint messageId)
            {
                if (messageId != 0)
                {
                    lock (_incomingMessageIdsToAck)
                        _incomingMessageIdsToAck.AddLast(messageId);
                }
            }

            internal const int MID2ACK_MAX_COUNT = 370;
            internal const int MID2ACK_SIZEOF = sizeof (uint);

            /// <summary>
            /// 
            /// </summary>
            internal uint[] IncomingMessageIdsToAckSnapshot
            {
                get
                {
                    lock (_incomingMessageIdsToAck)
                    {
                        if (_incomingMessageIdsToAck.Count == 0)
                            return null;

                        var countToCopy = _incomingMessageIdsToAck.Count;
                        uint[] output;

                        if (countToCopy > MID2ACK_MAX_COUNT)
                        {
                            countToCopy = MID2ACK_MAX_COUNT;
                            output = new uint[countToCopy];

                            var lln = _incomingMessageIdsToAck.First;
                            var i = 0;

                            while (lln != null && i < countToCopy)
                            {
                                output[i] = lln.Value;

                                _incomingMessageIdsToAck.RemoveFirst();

                                i++;
                                lln = lln.Next;
                            }
                        }
                        else
                        {
                            output = new uint[countToCopy];
                            _incomingMessageIdsToAck.CopyTo(output, 0);

                            _incomingMessageIdsToAck.Clear();
                        }

                        return output;
                    }
                }
            }
        }

        private readonly SyncDictionary<IPAddress, ConnectionInfo> _peerConnections = new SyncDictionary<IPAddress, ConnectionInfo>();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="add"></param>
        private void AddRemovePeerConnection(ISimpleTcpConnection connection, bool add)
        {
            if (connection == null)
                return;

            if (add)
            {
                var ipe = connection.RemoteEndPoint;

                ConnectionInfo ci;

                _peerConnections.GetOrAddValue(ipe.Address, out ci,
// ReSharper disable once ImplicitlyCapturedClosure
                    ipePassed =>
                    {
                        ci = new ConnectionInfo(false)
                        {
                            _connection = connection,
                            _peerIPe = connection.RemoteEndPoint
                        };

                        var queueAddedAsNew = _sendingPq.EnsureQueue(connection.RemoteEndPoint.Address);
                        DebugHelper.Keep(queueAddedAsNew);

                        return ci;
                    },
                    (ipePassed, ciPassed, newlyAdded) =>
                    {
                        if (!newlyAdded)
                        {
                            ciPassed._connection = connection;
                            ciPassed._peerIPe = connection.RemoteEndPoint;
                        }

                    });

            }
            else
            {
                _peerConnections.Remove(
                    connection.RemoteEndPoint.Address,
                    (keyPassed, removed, value) =>
                    {
                        if (removed)
                        {
                            var removedQueue = _sendingPq.RemoveQueue(keyPassed);
                            if (!removedQueue)
                                DebugHelper.NOP(keyPassed);
                        }
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="add"></param>
        private void AddRemovePeerConnection(ConnectionInfo connectionInfo, bool add)
        {
            if (connectionInfo == null)
                return;

            if (add)
            {
                ConnectionInfo tmp;
                _peerConnections.GetOrAddValue(
                    connectionInfo._peerIPe.Address,
                    out tmp,
                    keyPassed =>
                    {
                        var queueAddedAsNew = _sendingPq.EnsureQueue(connectionInfo._peerIPe.Address);
                        DebugHelper.Keep(queueAddedAsNew);

                        return connectionInfo;
                    },
                    null
                    );

            }
            else
            {
                _peerConnections.Remove(
                    connectionInfo._peerIPe.Address,
                    (keyPassed, removed, value) =>
                    {
                        if (removed)
                        {
                            var removedQueue = _sendingPq.RemoveQueue(keyPassed);
                            if (!removedQueue)
                                DebugHelper.NOP(keyPassed);
                        }
                    }
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <returns></returns>
        private ConnectionInfo GetConnection(IPEndPoint ipEndpoint)
        {
            ConnectionInfo ci;
            _peerConnections.TryGetValue(ipEndpoint.Address, out ci);

            return ci;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        void OnClientDisconnected(ISimpleTcpConnection connection)
        {
            AddRemovePeerConnection(connection, false);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        void OnClientConnected(ISimpleTcpConnection connection)
        {
            AddRemovePeerConnection(connection, true);

#if DEBUG
            if (connection == null)
            {
                DebugHelper.TryBreak("MessagingPeer connection shouldn't be null");
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        void OnConnectionDataReceived(ISimpleTcpConnection connection, ByteDataCarrier data)
        {
            var ci = GetConnection(connection.RemoteEndPoint);
            if (ci != null)
                OnDataReceived(ci, data);
#if DEBUG
            //else
            //{
            //    // should not happen
            //    DebugHelper.TryBreak("ci cannot be null",connection, data);
            //}
#endif
        }

        /// <summary>
        /// second level data parsing either from SimpleTcpServer.DataReceived or ConnectAndReadingThread
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="data"></param>
        private void OnDataReceived(ConnectionInfo connectionInfo, ByteDataCarrier data)
        {
            LowLevelMessage llm = null;

            //Debug.WriteLine(" RCV ["+data.ActualSize+"] "+data.HexDump());

            var raceCondition = 0;
            while (data.Offset != data.Length &&
                raceCondition++ < 64)
            {

                try
                {
                    llm = LowLevelMessage.ParseRawData(data);

                    // IMPORTANT !!!!
                    // if llm is returning null for a newly added message,
                    // don't forget if it's not missing within the ParseRawData switch condition
                }
                catch
                {
                }

                if (llm == null)
                {
                    if (connectionInfo._partialMessage == null)
                        // possibly some distortion 
                    {
                        connectionInfo.Disconnect();
                        return;
                    }

                    connectionInfo._partialMessage.AppendRemainingData(data);

                    if (connectionInfo._partialMessage._partial)
                        // the fragment data should come soon
                        return;

                    llm = connectionInfo._partialMessage;
                    connectionInfo._partialMessage = null;
                }
                else if (llm._partial)
                {
                    connectionInfo._partialMessage = llm;
                    return;
                }

                // verifies, if there weren't any message failures, and if so, 
                // raises event, that peer is back online
                var timeoutsInPast = connectionInfo.NullMessageTimeoutCount();
                if (timeoutsInPast && RemotePeerAliveAgain != null)
                    try
                    {
                        RemotePeerAliveAgain(this, connectionInfo._peerIPe);
                    }
                    catch
                    {
                    }


                if (llm.Command != LowLevelCommand.KeepAlive)
                {
                    connectionInfo.AddIncomingMessageIdToAck(llm.MessageId);
                }
                else
                {
                    // do this asap
                    var llkam = (LLKeepAliveMessage) llm;

                    var messagesToAck = llkam.MessageIdIdsToAck;
                    var confirmedMessages = connectionInfo.MessagesAcknowledgedOrRemoved(
                        (null!= BinaryMessageSucceeded ||
                        null != TextMessageSucceeded ||
                        null != ApplicationMessageSucceeded)
                        ,messagesToAck);

                    if (null != confirmedMessages)
                    {
                        InvokeMessageSucceeded(confirmedMessages);
                    }
#if DEBUG
                    try
                    {
                        if (messagesToAck != null)
                        {
                            string tmp = String.Empty;
                            foreach (var id in messagesToAck)
                                tmp += id + ",";

                            Debug.WriteLine(DateTime.Now + " MP/ RCV ACK : " + tmp);
                        }
                    }catch{}
#endif

                }

                InvokeMessageReceived(connectionInfo, llm);
            }
        }

        private void InvokeMessageSucceeded(IEnumerable<MPDataCarrier> confirmedMessages)
        {
            foreach (var dataCarrier in confirmedMessages)
            {
                switch (dataCarrier._lowLevelMessage.Command)
                {
                    case LowLevelCommand.BinaryMessage:
                        if (null != BinaryMessageSucceeded)
                            try
                            {
                                var bm = dataCarrier._lowLevelMessage as LLBinaryMessage;
                                BinaryMessageSucceeded(this, dataCarrier._peerIPe,
                                    bm != null ? bm.BinaryDataCarrier : null);
                            }
                            catch
                            {
                            }
                        break;
                    case LowLevelCommand.TextMessage:
                        if (null != TextMessageSucceeded)
                            try
                            {
                                var tm = dataCarrier._lowLevelMessage as LLTextMessage;
                                TextMessageSucceeded(this, dataCarrier._peerIPe,
                                    tm != null ? tm.TextData : null);
                            }
                            catch
                            {
                            }
                        break;
                    case LowLevelCommand.ApplicationMessage:
                        if (null != ApplicationMessageSucceeded)
                            try
                            {
                                var am = dataCarrier._lowLevelMessage as LLApplicationMessage;
                                ApplicationMessageSucceeded(this, dataCarrier._peerIPe,
                                    am != null ? am.ApplicationMessage : null);
                            }
                            catch
                            {
                            }
                        break;
                }
            }
        }

        private void InvokeMessageReceived(ConnectionInfo connectionInfo, LowLevelMessage llm)
        {
            switch (llm.Command)
            {
                case LowLevelCommand.BinaryMessage:
                    if (BinaryMessageReceived != null)
                        try
                        {
                            var llbm = (LLBinaryMessage) llm;

                            BinaryMessageReceived(this, connectionInfo._peerIPe, llbm.BinaryDataCarrier);
                        }
                        catch
                        {
                        }
                    break;

                case LowLevelCommand.TextMessage:
                    if (TextMessageReceived != null)
                        try
                        {
                            var lltm = (LLTextMessage) llm;
                            TextMessageReceived(this, connectionInfo._peerIPe, lltm.TextData);
                        }
                        catch
                        {
                        }
                    break;
                case LowLevelCommand.ApplicationMessage:
                    if (ApplicationMessageReceived != null)
                        try
                        {
                            var llam = (LLApplicationMessage) llm;

                            if (llam.ParseApplicationMessage(this, llam.MessageId))
                                ApplicationMessageReceived(this, connectionInfo._peerIPe, llam.ApplicationMessage);
                        }
                        catch
                        {
                        }
                    break;
                case LowLevelCommand.KeepAlive:
                    DebugHelper.NOP();
                    break;
            }
        }

        #region events and delegates

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagingPeer"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="binaryMessage"></param>
        public delegate void DBinaryMessageLambda(MessagingPeer messagingPeer, IPEndPoint remoteAddress, ByteDataCarrier binaryMessage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagingPeer"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="textMessage"></param>
        public delegate void DTextMessageLambda(MessagingPeer messagingPeer, IPEndPoint remoteAddress, string textMessage);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagingPeer"></param>
        /// <param name="remoteAddress"></param>
        public delegate void DRemotePeerAliveAgain(MessagingPeer messagingPeer, IPEndPoint remoteAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagingPeer"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="applicationMessage"></param>
        public delegate void DApplicationMessageLambda(MessagingPeer messagingPeer, IPEndPoint remoteAddress, IApplicationMessage applicationMessage);


        /// <summary>
        ///
        /// </summary>
        public event DBinaryMessageLambda BinaryMessageReceived;

        /// <summary>
        /// if UTF8 encoded text message received
        /// </summary>
        public event DTextMessageLambda TextMessageReceived;

        /// <summary>
        /// 
        /// </summary>
        public event DBinaryMessageLambda BinaryMessageRetryingFailed;

        /// <summary>
        /// 
        /// </summary>
        public event DBinaryMessageLambda BinaryMessageSucceeded;

        /// <summary>
        /// 
        /// </summary>
        public event DTextMessageLambda TextMessageRetryingFailed;

        /// <summary>
        /// 
        /// </summary>
        public event DTextMessageLambda TextMessageSucceeded;

        /// <summary>
        /// 
        /// </summary>
        public event DApplicationMessageLambda ApplicationMessageReceived;

        /// <summary>
        /// 
        /// </summary>
        public event DApplicationMessageLambda ApplicationMessageRetryingFailed;

        /// <summary>
        /// 
        /// </summary>
        public event DApplicationMessageLambda ApplicationMessageSucceeded;

        /// <summary>
        /// 
        /// </summary>
        public event DRemotePeerAliveAgain RemotePeerAliveAgain;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private class MPDataCarrier
        {
            internal MPDataCarrier(
                IPEndPoint peerIPe, 
                ConnectionInfo connectionInfo, 
                LowLevelMessage message,
                TimeSpan timeout,
                bool retryDuringTimeout
                )
            {
                _peerIPe = peerIPe;
                _connectionInfo = connectionInfo;
                _lowLevelMessage = message;
                _timeout = timeout;
                _retryDuringTimeout = retryDuringTimeout;
            }

            internal readonly IPEndPoint _peerIPe;
            internal volatile ConnectionInfo _connectionInfo;
            internal readonly LowLevelMessage _lowLevelMessage;
            
            internal readonly TimeSpan _timeout;
            internal readonly bool _retryDuringTimeout;

            internal DateTime _1stProcessingTimeStamp = default(DateTime);
            internal DateTime _lastProcessingTimeStamp = default(DateTime);

            internal volatile bool _aimedToBeRemoved = false;
        }

        private volatile ProcessingQueue<MPDataCarrier> _sendingPq = null;

        private const string ERROR_NOT_STARTED = "MessagingPeer must be started first";

        private const bool DEFAULT_RETRY_DURING_TIMEOUT = true;

        /// <summary>
        /// sends a binary message to a specified peer's IP address (to a TCP port than is same as this peer's listening port)
        /// </summary>
        /// <param name="peerIp">IP address</param>
        /// <param name="message">non-empty binary message</param>
        /// <param name="secondsTimeout">maximum timeout in seconds for retries of sending the message ; 
        /// if less or equal to  zero the message would be retried infinitely</param>
        /// <param name="retryDuringTimeout"></param>
        /// <exception cref="InvalidOperationException">if the MessagingPeer is not started</exception>
        /// <exception cref="ArgumentNullException">if peerIp or message is null</exception>
        public void SendMessage(IPAddress peerIp, ByteDataCarrier message, int secondsTimeout,bool retryDuringTimeout)
        {
            if (_sendingPq == null)
                throw new InvalidOperationException(ERROR_NOT_STARTED);

            if (peerIp == null)
                throw new ArgumentNullException("peerIp");

            if (ReferenceEquals(message, null))
                throw new ArgumentNullException("message");

            var llbm = new LLBinaryMessage(false,message);

            EnqueueMessage(peerIp, secondsTimeout, retryDuringTimeout, llbm);
        }

        /// <summary>
        /// sends a binary message to a specified peer's IP address (to a TCP port than is same as this peer's listening port)
        /// </summary>
        /// <param name="peerIp">IP address</param>
        /// <param name="message">non-empty binary message</param>
        /// <param name="secondsTimeout">maximum timeout in seconds for retries of sending the message ; 
        /// if less or equal to  zero the message would be retried infinitely</param>
        /// <exception cref="InvalidOperationException">if the MessagingPeer is not started</exception>
        /// <exception cref="ArgumentNullException">if peerIp or message is null</exception>
        public void SendMessage(IPAddress peerIp, ByteDataCarrier message, int secondsTimeout)
        {
            SendMessage(peerIp, message, secondsTimeout, DEFAULT_RETRY_DURING_TIMEOUT);
        }

        /// <summary>
        /// sends a binary message to a specified peer's IP address (to a TCP port than is same as this peer's listening port)
        /// with infinite retrying
        /// </summary>
        /// <param name="peerIp">IP address</param>
        /// <param name="message">non-empty binary message</param>
        /// <exception cref="InvalidOperationException">if the MessagingPeer is not started</exception>
        /// <exception cref="ArgumentNullException">if peerIp or message is null</exception>
        public void SendMessage(IPAddress peerIp, ByteDataCarrier message)
        {
            SendMessage(peerIp, message, -1,DEFAULT_RETRY_DURING_TIMEOUT);
        }

        /// <summary>
        /// sends a text (UTF8 encoded) message to a specified peer's IP address 
        /// (to a TCP port than is same as this peer's listening port)
        /// </summary>
        /// <param name="peerIp">IP address</param>
        /// <param name="message">non-empty text message</param>
        /// <param name="secondsTimeout">maximum timeout in seconds for retries of sending the message ; 
        /// if less or equal to  zero the message would be retried infinitely</param>
        /// <param name="retryDuringTimeout"></param>
        /// <exception cref="InvalidOperationException">if the MessagingPeer is not started</exception>
        /// <exception cref="ArgumentNullException">if peerIp or message is null</exception>
        public void SendMessage(IPAddress peerIp, string message, int secondsTimeout,bool retryDuringTimeout)
        {
            if (_sendingPq == null)
                throw new InvalidOperationException(ERROR_NOT_STARTED);

            if (peerIp == null)
                throw new ArgumentNullException("peerIp");

            if (ReferenceEquals(message, null))
                throw new ArgumentNullException("message");

            var lltm = new LLTextMessage(message);

            EnqueueMessage(peerIp, secondsTimeout, retryDuringTimeout, lltm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerIp"></param>
        /// <param name="secondsTimeout"></param>
        /// <param name="retryDuringTimeout"></param>
        /// <param name="message"></param>
        private void EnqueueMessage(IPAddress peerIp, int secondsTimeout, bool retryDuringTimeout, LowLevelMessage message)
        {
            var ts = default(TimeSpan);
            if (secondsTimeout > 0)
                ts = new TimeSpan(0, 0, 0, secondsTimeout);

            
            var queueAdded = _sendingPq.EnsureQueue(peerIp);
            if (!queueAdded)
                DebugHelper.NOP(peerIp);

            var itemCarrier =_sendingPq.EnqueueByKey(
                peerIp,
                new MPDataCarrier(
                    new IPEndPoint(peerIp, _peerPort),
                    null,
                    message,
                    ts,
                    retryDuringTimeout
                    ));

            if (itemCarrier == null)
                DebugHelper.NOP();
        }

        /// <summary>
        /// sends a text (UTF8 encoded) message to a specified peer's IP address 
        /// (to a TCP port than is same as this peer's listening port) with infinite retrying
        /// </summary>
        /// <param name="peerIp">IP address</param>
        /// <param name="message">non-empty text message</param>
        /// <exception cref="InvalidOperationException">if the MessagingPeer is not started</exception>
        /// <exception cref="ArgumentNullException">if peerIp or message is null</exception>
        public void SendMessage(IPAddress peerIp, string message)
        {
            SendMessage(peerIp, message, -1, DEFAULT_RETRY_DURING_TIMEOUT);
        }

        /// <summary>
        /// sends a text (UTF8 encoded) message to a specified peer's IP address 
        /// (to a TCP port than is same as this peer's listening port) with infinite retrying
        /// </summary>
        /// <param name="peerIp">IP address</param>
        /// <param name="message">non-empty text message</param>
        /// <param name="secondsTimeout"></param>
        /// <exception cref="InvalidOperationException">if the MessagingPeer is not started</exception>
        /// <exception cref="ArgumentNullException">if peerIp or message is null</exception>
        public void SendMessage(IPAddress peerIp, string message, int secondsTimeout)
        {
            SendMessage(peerIp, message, secondsTimeout, DEFAULT_RETRY_DURING_TIMEOUT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerIp"></param>
        /// <param name="message"></param>
        /// <param name="secondsTimeout"></param>
        /// <param name="retryDuringTimeout"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SendMessage(IPAddress peerIp, IApplicationMessage message, int secondsTimeout, bool retryDuringTimeout)
        {
            if (_sendingPq == null)
                throw new InvalidOperationException(ERROR_NOT_STARTED);

            if (peerIp == null)
                throw new ArgumentNullException("peerIp");

            if (ReferenceEquals(message, null))
                throw new ArgumentNullException("message");

            var llam = new LLApplicationMessage(message);

            // transfer the internal indexing into IApplicationMessage instance
            // which is yet unaware of the index
            message.MessageId = llam.MessageId;

            EnqueueMessage(peerIp, secondsTimeout, retryDuringTimeout, llam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerIp"></param>
        /// <param name="message"></param>
        /// <param name="secondsTimeout"></param>
        public void SendMessage(IPAddress peerIp, IApplicationMessage message, int secondsTimeout)
        {
            SendMessage(peerIp, message, secondsTimeout, DEFAULT_RETRY_DURING_TIMEOUT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerIp"></param>
        /// <param name="message"></param>
        public void SendMessage(IPAddress peerIp, IApplicationMessage message)
        {
            SendMessage(peerIp, message, -1, DEFAULT_RETRY_DURING_TIMEOUT);
        }

        private const int BASE_KEEPALIVE_PERIOD = 1000;
        private const int IDLE_KEEPALIVE_PERIOD = 5000;
       
        /// <summary>
        /// flushes into sending queue a keepalive for every running connection 
        /// with KEEPALIVE_PERIOD
        /// </summary>
        private void MaintenanceAndKeepaliveThread()
        {
            while (true)
            {
                Thread.Sleep(BASE_KEEPALIVE_PERIOD);

                // no need for cycling, if the peer list is empty
                _peerConnections.WaitWhileEmpty();


                try
                {
                    
                    // first iteration, send KeepAlives ASAP
                    _peerConnections.ForEach((ipePassed, ciPassed) =>
                        {
                            var messageIdsToAck = ciPassed.IncomingMessageIdsToAckSnapshot;
                            var toSendAck =
                                messageIdsToAck != null ||
                                ciPassed._lastSentAck == default(DateTime) ||
                                (DateTime.Now - ciPassed._lastSentAck).TotalMilliseconds >= IDLE_KEEPALIVE_PERIOD ;

                            if (toSendAck)
                            {
                                _sendingPq.EnsureQueue(ciPassed._peerIPe.Address);

                                var itemCarrier =
                                _sendingPq.EnqueueByKey(
                                    ciPassed._peerIPe.Address,
                                    new MPDataCarrier(
                                        ciPassed._peerIPe,
                                        ciPassed,
                                        // doesn't matter if messageIdsToAck is null here
                                        new LLKeepAliveMessage(messageIdsToAck),
                                        default(TimeSpan), false));

                                if (itemCarrier == null)
                                    DebugHelper.NOP(ciPassed._peerIPe.Address);

                                ciPassed._lastSentAck = DateTime.Now;
                            }

                        }
                        );

                    // intentionally separate foreach
                    Thread.Sleep(50);

                    _peerConnections.ForEach(
                        (ipePassed, ciPassed) =>
                        {
                            var mpCarriers = ciPassed.MPDataCarrierSnapshot;

                            if (mpCarriers !=null)
                                foreach (var dataCarrier in mpCarriers)
                                {
                                    if (dataCarrier._retryDuringTimeout)
                                        ValidateDataCarrierRetry(ciPassed, dataCarrier, true);
                                }
                        }
                        );
                    


                }
                catch (Exception)
                {
                    
                }


            }
// ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// removes the peer's connection and all of it's messages
        /// if connected
        /// </summary>
        /// <param name="peerIp"></param>
        /// <return>true if the peer connection was present</return>
        public bool Unbind(IPAddress peerIp)
        {
            var ipe = new IPEndPoint(peerIp, _peerPort);

            var ci = GetConnection(ipe);
            if (ci == null)
                return false;

            // it's better to call it here, to mark it AimedToBeDisposed ASAP
            // as during ConnectionInfo.Dispose() the peerIp is not destroyed , it
            // can be used e.g. for removal from the list
            try { ci.Dispose(); }
            catch { }

            try
            {
                _sendingPq.RemoveWhere(
                    UnbindComparator,
                    ipe);
            }
            catch
            {
            }

            try { AddRemovePeerConnection(ci, false); }
            catch { }

            return true;
        }

        /// <summary>
        /// comparator used withdrawing the messages during unbind
        /// </summary>
        /// <param name="itemInQueue"></param>
        /// <param name="itemToCompareWith"></param>
        private bool UnbindComparator(MPDataCarrier itemInQueue, object itemToCompareWith)
        {
            var ipe = (IPEndPoint)itemToCompareWith;

            return (Equals(itemInQueue._peerIPe, ipe));               

        }
    }
}
