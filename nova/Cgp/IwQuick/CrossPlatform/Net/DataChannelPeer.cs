using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public enum DCPError
    {
        /// <summary>
        /// 
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 
        /// </summary>
        ExceptionThrown = 0x00010,
        /// <summary>
        /// 
        /// </summary>
        ProtocolProblem = 0x20001,
        /// <summary>
        /// 
        /// </summary>
        ConnectionProblem = 0x20002,
        /// <summary>
        /// 
        /// </summary>
        TransferStopped = 0x20003,
        /// <summary>
        /// 
        /// </summary>
        FileAlreadyExists = 0x30001,
        /// <summary>
        /// 
        /// </summary>
        FileCannotBeCreated = 0x30002,
        /// <summary>
        /// 
        /// </summary>
        IOOperationFailed = 0x30003,
        /// <summary>
        /// 
        /// </summary>
        IOPerminentError = 0x30004,
        /// <summary>
        /// 
        /// </summary>
        FileDoesNotExists = 0x30005,
        /// <summary>
        /// 
        /// </summary>
        TransferProhibited = 0x30010,
        /// <summary>
        /// 
        /// </summary>
        MemoryStreamCannotBeCreated = 0x40001,
        /// <summary>
        /// 
        /// </summary>
        DataInconsistentTransfer = 0x60001,
        /// <summary>
        /// 
        /// </summary>
        DataFragmentationProblem = 0x60002,
        /// <summary>
        /// 
        /// </summary>
        DataChecksumInvalid = 0x60003,
        


    }

    /// <summary>
    /// class for transferring and receiving files over TCP non-persistent sessions
    /// </summary>
    public class DataChannelPeer
    {
        /// <summary>
        /// only instantiated if StartListening is called
        /// </summary>
        private volatile SimpleTcpServer _tcpListener = null;
        
        private readonly object _tcpListenerLock = new object();

        //private IPAddress _bindToIp;
        //private int _bindToPort;
        private bool _onlyAllowedIPs = false;

        // THIS SIZE CANNOT BE BIGGER THAN UInt16.MaxValue !!!! as protocol does not allow more than 2B optional dataLength
        private const int DEFAULT_BUFFER_SIZE = OPTIMIZED_SOCKET_STREAM_BUFFER_SIZE;
        private const int DEFAULT_LISTEN_BACKLOG = 16;

        private const int OPTIMIZED_SOCKET_STREAM_BUFFER_SIZE = 32768;
        private const int OPTIMIZED_FILE_STREAM_BUFFER_SIZE = 512;

        private const int SHRINKED_STREAM_READING_SIZE = 32000; // counting with protocolar overhead

        #region Protocol definitions

        private const ushort PROTO_VERSION = 0x0100;
        private readonly static byte[] PREAMBLE = { 0xF5, 0x5F };

        private const int PROTO_PART_LENGTH = 2;
        private const int CMD_PART_LENGTH = 2;
        //private const int ODL_PART_LENGTH = 2;
        private const int HEADER_LENGTH = 6; // proto(2B), cmd(2B), cmd-param-length (2B)

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public enum StreamType 
        {
            /// <summary>
            /// 
            /// </summary>
            Memory,
            /// <summary>
            /// 
            /// </summary>
            File,
            /// <summary>
            /// 
            /// </summary>
            Unknown,
            /*/// <summary>
            /// 
            /// </summary>
            ByteDataCarrier,*/
        }

        internal enum TransferPhase
        {
            Dormant,
            Connecting,
            InitialPushCommand,
            InitialPullCommand,
            DataPushing,
            DataPulling,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum TransferSide
        {
            /// <summary>
            /// 
            /// </summary>
            PushInitiator,
            /// <summary>
            /// 
            /// </summary>
            PushReceiver,
            /// <summary>
            /// 
            /// </summary>
            PullInitiator,
            /// <summary>
            /// 
            /// </summary>
            PullSender
        }

        /// <summary>
        /// 
        /// </summary>
        public class TransferInfo
        {
            // counter for generating ids
            private static int _idCounter = 0;

            internal readonly int _id;
            /// <summary>
            /// 
            /// </summary>
            public int Id { get { return _id; }}

            /// <summary>
            /// 
            /// </summary>
            /// <param name="transferSide"></param>
            internal TransferInfo(TransferSide transferSide)
            {
                _id = ++_idCounter;

                if (_id == 0)
                    _id = ++_idCounter;

                TransferSide = transferSide;

                /*if (isPull)
                    _mrePullMutex = new ManualResetEvent(false);*/
            }

            /// <summary>
            /// 
            /// </summary>
            public readonly TransferSide TransferSide;
            
            internal IPEndPoint _remoteIPEndPoint;
            /// <summary>
            /// 
            /// </summary>
            public IPEndPoint RemoteEndPoint
            {
                get { return _remoteIPEndPoint; }
            }

            internal IPEndPoint _localIPEndPoint;
            /// <summary>
            /// 
            /// </summary>
            public IPEndPoint LocalEndPoint
            {
                get
                {
                    return _localIPEndPoint;
                }
            }

            internal volatile LowLevelMessage _partialMessage = null;

            internal string _destinationName = null;
            internal string _sourceName = null;
            /// <summary>
            /// 
            /// </summary>
            public string SourceName
            {
                get
                {
                    return _sourceName; 
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string DestinationName { get { return _destinationName; } }

            internal StreamType _streamType;

            /// <summary>
            /// 
            /// </summary>
            public StreamType DestinationStreamType
            {
                get
                {

                    switch (TransferSide)
                    {
                        case TransferSide.PullInitiator:
                        case TransferSide.PushReceiver:
                            return _streamType;
                        default:
                            return StreamType.Unknown;
                    }
                }
            }

            internal volatile Stream _stream = null;
            
            internal int _originalStreamPosition = 0;

            /// <summary>
            /// stream from which are the data transferred ; null on the destination/server side
            /// </summary>
            public Stream SourceStream
            {
                get
                {
                    switch (TransferSide)
                    {
                        case TransferSide.PullInitiator:
                            return null;
                        case TransferSide.PullSender:
                            return _stream;
                        case TransferSide.PushInitiator:
                            return _stream;
                        case TransferSide.PushReceiver:
                            return null;
                    }
                                        
                    return null;
                }
            }

            /// <summary>
            /// stream into which the transferred data are transferred ; null on the source/client side
            /// </summary>
            public Stream DestinationStream
            {
                get
                {
                    switch (TransferSide)
                    {
                        case TransferSide.PullInitiator:
                            return _stream;
                        case TransferSide.PullSender:
                            return null;
                        case TransferSide.PushInitiator:
                            return null;
                        case TransferSide.PushReceiver:
                            return _stream;
                    }

                    return null;
                }
            }

            /// <summary>
            /// if true, the destination will store it as FileStream, otherwise as MemoryStream
            /// </summary>
            internal bool _storeAsFile;
            /// <summary>
            /// 
            /// </summary>
            public bool StoreAsFile { get { return _storeAsFile; } }

            
            internal bool _overwrite = false;
            /// <summary>
            /// 
            /// </summary>
            public bool Overwrite { get { return _overwrite; } }

            private volatile int _result = -1;
            private readonly object _resultSync = new object();

            /// <summary>
            /// 
            /// </summary>
            internal int Result
            {
                get
                {
                    lock (_resultSync) return _result;
                }
            }

            private readonly ManualResetEvent _mreResultMutex = new ManualResetEvent(false);

            /// <summary>
            /// 
            /// </summary>
            internal void WaitForResult()
            {
                _mreResultMutex.WaitOne();
            }

            internal void ResetWaitingForResult()
            {
                _mreResultMutex.Reset();
            }

            /*
            private readonly ManualResetEvent _mrePullMutex;

            internal bool WaitForPullAck(int timeout)
            {
                if (_mrePullMutex != null)
                    return _mrePullMutex.WaitOne(timeout < 0 ? -1 : timeout, false);
                
                    return true;
            }*/
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="result"></param>
            /// <param name="lastErrorCode"></param>
            /// <param name="lastErrorMessage"></param>
            internal void SetResult(int result, DCPError lastErrorCode, string lastErrorMessage)
            {
                lock (_resultSync)
                {
                    _lastErrorCode = lastErrorCode;

                    _lastErrorMessage = lastErrorMessage ?? String.Empty;

                    _result = result;

                    _mreResultMutex.Set();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="exception"></param>
            internal void SetLastError(Exception exception)
            {
                lock (_resultSync)
                {
                    if (string.IsNullOrEmpty(_lastErrorMessage) && exception != null)
                    {
                        _lastErrorMessage = exception.Message;
                    }

                    if (exception is SocketException)
                        _lastErrorCode = DCPError.ConnectionProblem;
                    else
                        _lastErrorCode = DCPError.ExceptionThrown;


                    _lastException = exception;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="result"></param>
            /// <param name="exception"></param>
            internal void SetResult(int result, Exception exception)
            {
                lock (_resultSync)
                {
                    SetLastError(exception);

                    _result = result;

                    _mreResultMutex.Set();
                }
            }

            
            internal void SetLastError(DCPError lastErrorCode, string lastErrorMessage)
            {
                lock (_resultSync)
                {
                    _lastErrorCode = lastErrorCode;

                    _lastErrorMessage = lastErrorMessage ?? String.Empty;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public bool Success
            {
                get { return _result > 0; }
            }

            /// <summary>
            /// overall size to transfer
            /// </summary>
            internal int _overallSize = 0;
            /// <summary>
            /// 
            /// </summary>
            public int OverallSize { get { return _overallSize; } }

            internal bool _closeStreamAfterProcessing = true;

            // thread doing the actual sending receiving handshake
            internal Threads.SafeThread<TransferInfo,ISimpleTcpConnection> _transferThread = null;

            internal Threads.SafeThread<TransferInfo> _progressingThread = null;

            internal Exception _lastException = null;
            /// <summary>
            /// 
            /// </summary>
            public Exception LastException
            {
                get { return _lastException; }
            }

            private DCPError _lastErrorCode = DCPError.Unknown;
            /// <summary>
            /// 
            /// </summary>
            public DCPError LastErrorCode { get { lock(_resultSync) return _lastErrorCode; } }

            private string _lastErrorMessage = String.Empty;
            /// <summary>
            /// 
            /// </summary>
            public string LastErrorMessage
            {
                get { lock (_resultSync) return _lastErrorMessage; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal bool TryCloseStream(bool forceCloseStream)
            {
                if (_stream != null && 
                    (
                    forceCloseStream ||
                    _closeStreamAfterProcessing ||
                    _result <= 0
                    ))
                    try
                    {
                        Stream s = _stream;
                        // atomic operation as _stream is volatile
                        _stream = null;
                        s.Close(); 
                         
                        return true;
                    }
                    catch { return false; }
                
                return false;
            }

            internal DateTime _transferStart;
            internal DateTime _transferEnd;

            /// <summary>
            /// 
            /// </summary>
            public TimeSpan TransferDurationPure
            {
                get { 
                    if (_transferStart == default(DateTime) ||
                        _transferEnd == default(DateTime))
                        return default(TimeSpan);
                    
                    return _transferEnd - _transferStart; 
                }
            }

            internal ManualResetEvent _waitForEndMutex = new ManualResetEvent(false);

            internal ISimpleTcpConnection _connection = null;

            internal int _transferProgress = 0;

            /// <summary>
            /// 
            /// </summary>
            public double AverageKbps
            {
                get
                {
                    double ms = TransferDurationPure.TotalMilliseconds;
                    if (Math.Abs(ms) < 0.000000001)
                        return 0;
                    
                    double rate = _overallSize / ms * 1024 / 1000;
                    return rate;
                }

            }

            internal volatile bool _stopTransfer = false;
            private readonly object _stopTransferLock = new object();
            //private SafeThread _stopTransferThread = null;

            internal volatile TransferPhase _transferPhase = TransferPhase.Dormant;

            private const int THREAD_EXIT_GRACEFUL_PERIOD = 2000;

            /// <summary>
            /// marks the stop indicator, and tries to kill the transferring thread if it does not stop
            /// gracefuly within 2+2s ; 
            /// blocking call, invoking over Form is not advised
            /// </summary>
            public void StopTransfer()
            {
                if (_result >= 0)
                    return; // // the transfer already stopped gracefuly 

                if (!_stopTransfer)
                    lock (_stopTransferLock)
                        if (!_stopTransfer)
                        {
                            _stopTransfer = true;

                            try
                            {
                                bool endWithoutTimeout = _waitForEndMutex.WaitOne(THREAD_EXIT_GRACEFUL_PERIOD, false);

                                if (!endWithoutTimeout &&
                                    null != _transferThread &&
                                    _result < 0) // double checking
                                    _transferThread.Stop(THREAD_EXIT_GRACEFUL_PERIOD);
                            }
                            catch
                            {
                            }
                        }
                
            }

            /// <summary>
            /// 
            /// </summary>
            public void AsyncStopTransfer()
            {
                if (_result < 0 &&
                    !_stopTransfer)
                    Threads.SafeThread.StartThread(StopTransfer);
            }

            //internal bool _breakFromPushDataCycle = false;
            internal byte[] _tmpBuffer = null;
            internal int _problemRaceCondition = 0;
            internal int _previousPercentage = 0;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static string GetStringIdentifier(
                IPEndPoint localIPEndPoint,
                IPEndPoint remoteIPEndPoint)
            {
                string tmp =
                    string.Concat(
                        localIPEndPoint != null ? localIPEndPoint.ToString() : String.Empty,
                        StringConstants.SEMICOLON,
                        remoteIPEndPoint != null ? remoteIPEndPoint.ToString() : String.Empty);
                return tmp;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string  ToString()
            {
                return GetStringIdentifier(_localIPEndPoint, _remoteIPEndPoint);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                string tmp = ToString();
                return tmp.GetHashCode();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                if (!(obj is TransferInfo))
                    return false;

                TransferInfo otherTi = obj as TransferInfo;

                if (_localIPEndPoint != otherTi._localIPEndPoint)
                    return false;

                if (_remoteIPEndPoint != otherTi._remoteIPEndPoint)
                    return false;
                
                return true;
            }
        }

        //private volatile Dictionary<int, TransferInfo> _transmits;

        private readonly Data.SyncDictionary<string, TransferInfo> _peers = new Data.SyncDictionary<string, TransferInfo>(8);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private TransferInfo GetReceiveTransferInfo(ISimpleTcpConnection connection)
        {
            if (null == connection)
                return null;

            TransferInfo ti = null;

            try
            {

                string tmp = TransferInfo.GetStringIdentifier(connection.LocalEndPoint, connection.RemoteEndPoint);

                _peers.TryGetValue(tmp, out ti);
                
            }
            catch
            {
            }

            return ti;
        }

// ReSharper disable once UnusedField.Compiler
// ReSharper disable once ConvertToConstant.Local
        private readonly bool _isCompactFramework = 
#if COMPACT_FRAMEWORK
            true
#else
            false
#endif
            ;            

        /// <summary>
        /// this instantiates the class only in client-mode
        /// </summary>
// ReSharper disable once EmptyConstructor
        public DataChannelPeer()
        {
            
        }

        private volatile Dictionary<IPAddress, bool> _allowedIPs = null;
        private readonly object _allowedIPsLock = new object();
        
        private void EnsureAllowedIPs()
        {
            if (_allowedIPs == null)
                lock (_allowedIPsLock)
                {
                    if (_allowedIPs == null)
                    {
                        _allowedIPs = new Dictionary<IPAddress, bool>(8);
                    }
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool AllowIP(IPAddress ip)
        {
            if (ip != null)
            {
                EnsureAllowedIPs();
                try
                {
                    _allowedIPs[ip] = true;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool RejectIP(IPAddress ip)
        {
            if (ip !=null)
            {
                EnsureAllowedIPs();
                try
                {
                    _allowedIPs.Remove(ip);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            
            return false;
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAllowedIPs()
        {
            try
            {
                if (_allowedIPs != null)
                    lock (_allowedIPsLock)
                    {
                        if (_allowedIPs != null)
                            _allowedIPs.Clear();
                    }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindToIp">if null, IPAddress.Any will be used</param>
        /// <param name="port">TCP port to bind to</param>
        /// <exception cref="InvalidTransportPortException">if port is out of range 1-65535</exception>
        /// <exception cref="SocketException">if binding to the specific IPAddress and port was not successful</exception>
        /// <param name="onlyAllowedIPs"></param>
        public void StartListening(IPAddress bindToIp, int port, bool onlyAllowedIPs)
        {
            TcpUdpPort.CheckValidity(port);
            bool newlyCreated = false;
            if (null == _tcpListener)
            {
                lock (_tcpListenerLock)
                {
                    if (_tcpListener == null) {
                        _tcpListener = new SimpleTcpServer();
                        newlyCreated = true;
                    }
                }
            }

            if (newlyCreated)
            {

                _tcpListener.DataReceived += OnTcpDataReceived;
                _tcpListener.BufferSize = DEFAULT_BUFFER_SIZE;


                _tcpListener.Connected += OnListenerClientConnected;

                _tcpListener.Disconnected += OnRemoteSideDisconnected;

                if (null == bindToIp)
                    bindToIp = IPAddress.Any;

                _onlyAllowedIPs = onlyAllowedIPs;

                _tcpListener.Start(bindToIp, port, null, DEFAULT_LISTEN_BACKLOG);

            }
        }

        #region Protocol

        /// <summary>
        /// 
        /// </summary>
        public enum LowLevelCommand : ushort
        {
            /// <summary>
            /// 
            /// </summary>
            Unknown = 0xF000,
            /// <summary>
            /// 
            /// </summary>
            StoreFile = 0x0100,
            /// <summary>
            /// 
            /// </summary>
            PullFile = 0x0110,
            /// <summary>
            /// 
            /// </summary>
            StoreMemoryStream = 0x0200,
            /// <summary>
            /// 
            /// </summary>
            PushData = 0x1000,
            /// <summary>
            /// 
            /// </summary>
            Acknowledged = 0x0001,
            /// <summary>
            /// 
            /// </summary>
            NotAcknowledged = 0x000F,
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

            protected LowLevelMessage(LowLevelCommand command)
            {
                _command = command;
            }

            protected internal Data.ByteDataCarrier _rawParameters;

            internal Data.ByteDataCarrier PrepareHeader(int optionalDataLength)
            {
                Data.ByteDataCarrier rawHeader = new Data.ByteDataCarrier(PREAMBLE.Length + HEADER_LENGTH, false);
                rawHeader.Append(PREAMBLE);

                //int pos = PREAMBLE.Length;

                rawHeader.Append(BitConverter.GetBytes(PROTO_VERSION),true);

                rawHeader.Append(BitConverter.GetBytes((UInt16)_command), true);
                rawHeader.Append(BitConverter.GetBytes((UInt16)optionalDataLength), true);

                return rawHeader;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal virtual Data.ByteDataCarrier GetRawData()
            {
                return PrepareHeader(0);
            }
            
            //{
            //    ByteDataCarrier rawMessage = new ByteDataCarrier(PREAMBLE.Length + HEADER_LENGTH, false);
            //    rawMessage.Append(PREAMBLE);

            //    int pos = PREAMBLE.Length;
                
            //    rawMessage.Append(BitConverter.GetBytes((ushort)PROTO_VERSION));


            //    int odl = 0;
            //    _rawParameters = null;
            //    switch (_command)
            //    {
            //        case LowLevelCommand.StoreMemoryStream:
            //        case LowLevelCommand.StoreFile:
                        
            //            break;

            //        case LowLevelCommand.ACKCommand:
            //        case LowLevelCommand.ACKStoring:
            //            /*strLen = (_stringParam ?? String.Empty).Length;
            //            odl = INT_PART_LENGTH + strLen;

            //            _rawParameters = new ByteDataCarrier(odl, false);

            //            _rawParameters.Append(BitConverter.GetBytes(_intParam),true);
            //            if (strLen > 0)
            //                _rawParameters.Append(Encoding.UTF8.GetBytes(_stringParam), true);

            //            odl = _rawParameters.ActualSize; // correct the ODL because of the UTF8 encoding*/
            //            break;
            //        case LowLevelCommand.NACKCommand:
            //        case LowLevelCommand.NACKStoring:
            //            /*strLen = (_stringParam ?? String.Empty).Length;
            //            odl = USHORT_PART_LENGTH+INT_PART_LENGTH + strLen;

            //            _rawParameters = new ByteDataCarrier(odl, false);

            //            _rawParameters.Append(BitConverter.GetBytes(_ushortParam),true);
            //            _rawParameters.Append(BitConverter.GetBytes(_intParam),true);

            //            if (strLen > 0)
            //                _rawParameters.Append(Encoding.UTF8.GetBytes(_stringParam), true);

            //            odl = _rawParameters.ActualSize; // correct the ODL because of the UTF8 encoding
            //            break;*/
            //    }

            //    rawMessage.Append(BitConverter.GetBytes((UInt16)_command), true);
            //    rawMessage.Append(BitConverter.GetBytes((UInt16)odl), true);
            //    rawMessage.Append(_rawParameters, true);

            //    return rawMessage;
            //}

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="length"></param>
            /// <returns>LowLevelMessage child, if able to parse</returns>
            internal static LowLevelMessage ParseRawData(byte[] data, int length)
            {
                int pos = Data.ByteDataCarrier.IndexOf(data, 0, length, PREAMBLE);
                if (pos < 0)
                    return null;

                // it still can happen , that raw data would contain PREAMBLE, however
                // further conditions should eliminate that situation 

                pos += PREAMBLE.Length;

                if (pos + HEADER_LENGTH > length)
                    // incomplete header - should not happen on separate connections
                    // as every previous message should be confirmed/ack or not ack
                    // thus not even fragmentation should be relevant here
                    return null;

                UInt16 protoVersion = BitConverter.ToUInt16(data, pos);
                if (protoVersion != PROTO_VERSION)
                    // unsupported protocol
                    // further conditioning will be added when protocol evolves
                    return null;

                UInt16 command = BitConverter.ToUInt16(data, pos + PROTO_PART_LENGTH);
                LowLevelCommand llc = (LowLevelCommand)command;
                // verification of the command validity
                // in case in raw data PREAMBLE will be found (non-zero probability)
                switch (llc)
                {
                    case LowLevelCommand.Acknowledged:
                    case LowLevelCommand.NotAcknowledged:
                    case LowLevelCommand.PushData:
                    case LowLevelCommand.StoreFile:
                    case LowLevelCommand.StoreMemoryStream:
                    case LowLevelCommand.PullFile:
                        break;
                    default:
                        return null;
                }

                // rely on IPv4 and/or TCP checksumming capabilities
                UInt16 odl = BitConverter.ToUInt16(data, pos + PROTO_PART_LENGTH + CMD_PART_LENGTH);

                var rawParams = new Data.ByteDataCarrier(data, pos + HEADER_LENGTH, odl);
                bool partial = rawParams.Length < odl;

                LowLevelMessage llm;

                switch (llc)
                {
                    case LowLevelCommand.StoreFile:
                        llm = new LLStoreFileMessage(rawParams);
                        break;
                    case LowLevelCommand.PullFile:
                        llm = new LLPullFileMessage(rawParams);
                        break;
                    case LowLevelCommand.StoreMemoryStream:
                        llm = new LLStoreMemoryStreamMessage(rawParams);
                        break;
                    case LowLevelCommand.Acknowledged:
                        llm = new LLAcknowledgedMessage(rawParams);
                        break;
                    case LowLevelCommand.NotAcknowledged:
                        llm = new LLNotAcknowledgedMessage(rawParams);
                        break;
                    case LowLevelCommand.PushData:
                        llm = new LLPushDataMessage(rawParams);
                        break;
                    default:
                        return null;
                }

                llm._optionalDataLength = odl;
                llm._partial = partial;

                return llm;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            internal static LowLevelMessage ParseRawData(Data.ByteDataCarrier data)
            {
                return ParseRawData(data.Buffer, data.ActualSize);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            internal void AppendRemainingData(Data.ByteDataCarrier data)
            {
                _rawParameters.Append(data);
                _partial = _rawParameters.Length < _optionalDataLength;
            }

            internal virtual void ReevaluateRawData()
            {
                
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private class LLStoreFileMessage : LowLevelMessage
        {
            private LLStoreFileMessage()
                :base(LowLevelCommand.StoreFile)
            {
                Overwrite = false;
            }

            private const int OVERWRITE_PART = 1;
            private const int SIZE_PART = sizeof(int);

            internal LLStoreFileMessage(Data.ByteDataCarrier rawParams)
                :base(LowLevelCommand.StoreFile)
            {
                _rawParameters = rawParams;

                Overwrite = (rawParams[0] > 0); // not used for MemoryStream
                OverallSize = BitConverter.ToInt32(rawParams.Buffer, OVERWRITE_PART);

                // in case of MemoryStream it's just a name
                FilePath = Encoding.UTF8.GetString(rawParams.Buffer, OVERWRITE_PART + SIZE_PART,
                    rawParams.ActualSize - (OVERWRITE_PART + SIZE_PART));
            }

            internal LLStoreFileMessage(bool overwrite, int overallSize, string filePath)
                :this()
            {
                Overwrite = overwrite;
                OverallSize = overallSize;
                FilePath = filePath;
            }

            public bool Overwrite { get; private set; }

            private string _filePath;
            public string FilePath
            {
                get { return _filePath; }
                private set { _filePath = value; }
            }

            public int OverallSize { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override Data.ByteDataCarrier GetRawData()
            {
                int strLen = (_filePath ?? String.Empty).Length;
                int odl = OVERWRITE_PART + SIZE_PART + strLen;

                _rawParameters = new Data.ByteDataCarrier(odl, false);

                _rawParameters[0] = (byte)(Overwrite ? 1 : 0);
                _rawParameters.ActualSize = OVERWRITE_PART;

                _rawParameters.Append(BitConverter.GetBytes(OverallSize), true);
                if (strLen > 0)
                    _rawParameters.Append(Encoding.UTF8.GetBytes(_filePath ?? String.Empty), true);

                odl = _rawParameters.ActualSize; // correct the ODL because of the UTF8 encoding

                var headerBdc = PrepareHeader(odl);
                headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class LLPullFileMessage : LowLevelMessage
        {
            private LLPullFileMessage()
                : base(LowLevelCommand.PullFile)
            {
                PullAsFile = false;
            }

            private const int PULL_AS_FILE_PART = 1;


            internal LLPullFileMessage(Data.ByteDataCarrier rawParams)
                : base(LowLevelCommand.PullFile)
            {
                _rawParameters = rawParams;

                PullAsFile = (rawParams[0] > 0); 

                // in case of MemoryStream it's just a name
                FilePath = Encoding.UTF8.GetString(rawParams.Buffer, PULL_AS_FILE_PART ,
                    rawParams.ActualSize - PULL_AS_FILE_PART);
            }

            internal LLPullFileMessage(string filePath,bool pullAsFile)
                : this()
            {
                FilePath = filePath;
                PullAsFile = pullAsFile;
            }

            private string _filePath;
            public string FilePath
            {
                get { return _filePath; }
                private set { _filePath = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool PullAsFile { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override Data.ByteDataCarrier GetRawData()
            {
                int strLen = (_filePath ?? String.Empty).Length;
                int odl = PULL_AS_FILE_PART + strLen;

                _rawParameters = new Data.ByteDataCarrier(odl, false);

                _rawParameters[0] = (byte)(PullAsFile ? 1 : 0);
                _rawParameters.ActualSize = PULL_AS_FILE_PART;

                if (strLen > 0)
                    _rawParameters.Append(Encoding.UTF8.GetBytes(_filePath ?? String.Empty), true);

                odl = _rawParameters.ActualSize; // correct the ODL because of the UTF8 encoding

                var headerBdc = PrepareHeader(odl);
                headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class LLStoreMemoryStreamMessage : LowLevelMessage
        {
            private LLStoreMemoryStreamMessage()
                : base(LowLevelCommand.StoreMemoryStream)
            {
            }

            private const int SIZE_PART = sizeof(int);

            internal LLStoreMemoryStreamMessage(Data.ByteDataCarrier rawParams)
                : base(LowLevelCommand.StoreMemoryStream)
            {
                _rawParameters = rawParams;

                OverallSize = BitConverter.ToInt32(rawParams.Buffer, 0);

                // in case of MemoryStream it's just a name
                StreamName = Encoding.UTF8.GetString(rawParams.Buffer, SIZE_PART,
                    rawParams.ActualSize - (SIZE_PART));
            }

            internal LLStoreMemoryStreamMessage(int overallSize, string streamName)
                : this()
            {
                OverallSize = overallSize;
                StreamName = streamName;
            }

            private string _streamName;
            public string StreamName
            {
                get { return _streamName; }
                private set { _streamName = value; }
            }

            public int OverallSize { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override Data.ByteDataCarrier GetRawData()
            {
                int strLen = (_streamName ?? String.Empty).Length;
                int odl = SIZE_PART + strLen;

                _rawParameters = new Data.ByteDataCarrier(odl, false);

                _rawParameters.Append(BitConverter.GetBytes(OverallSize), true);
                if (strLen > 0)
                    _rawParameters.Append(Encoding.UTF8.GetBytes(_streamName ?? String.Empty), true);

                odl = _rawParameters.ActualSize; // correct the ODL because of the UTF8 encoding

                var headerBdc = PrepareHeader(odl);
                headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class LLPushDataMessage : LowLevelMessage
        {
            private LLPushDataMessage()
                : base(LowLevelCommand.PushData)
            {
            }

            private const int OFFSET_PART = sizeof(int);
            private const int CRC_PART = sizeof(int);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="rawParams"></param>
            internal LLPushDataMessage(Data.ByteDataCarrier rawParams)
                : base(LowLevelCommand.PushData)
            {
                _rawParameters = rawParams;

                ReevaluateRawData();
            }

            internal override void ReevaluateRawData()
            {
                _offset = BitConverter.ToInt32(_rawParameters.Buffer, 0);
                _crc = BitConverter.ToInt32(_rawParameters.Buffer, OFFSET_PART);
                int blockLength = _rawParameters.ActualSize - (OFFSET_PART + CRC_PART);

                _data = new byte[blockLength];
                if (blockLength > 0)
                    Array.Copy(_rawParameters.Buffer, OFFSET_PART + CRC_PART, _data, 0, blockLength);
            }



            /// <summary>
            /// 
            /// </summary>
            /// <param name="offset"></param>
            /// <param name="crc"></param>
            /// <param name="data"></param>
            /// <param name="actualSize"></param>
            internal LLPushDataMessage(int offset, int crc, byte[] data, int actualSize)
                : this()
            {
                _offset = offset;
                _crc = crc;

                int blockLength = data != null ? data.Length : 0;
                if (actualSize < blockLength)
                    blockLength = actualSize;

                _data = new byte[blockLength];
                if (blockLength > 0 && data != null)
                    Array.Copy(data, _data, blockLength);
            }

            private int _offset;
            /// <summary>
            /// position in the final stream, where to put data
            /// </summary>
            public int Offset
            {
                get { return _offset; }
            }

            private int _crc;
            public bool CrcUsed
            {
                get { return _crc != -1; }
            }

            /// <summary>
            /// 
            /// </summary>
            public uint Crc32
            {
                get { return (uint)_crc; }
            }

            private byte[] _data;
            public byte[] Data
            {
                get { return _data; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal override Data.ByteDataCarrier GetRawData()
            {

                int odl = OFFSET_PART + CRC_PART + _data.Length;

                _rawParameters = new Data.ByteDataCarrier(odl, false);

                _rawParameters.Append(BitConverter.GetBytes(_offset), true);
                _rawParameters.Append(BitConverter.GetBytes(_crc), true);

                _rawParameters.Append(_data, true);

                var headerBdc = PrepareHeader(odl);
                headerBdc.Append(_rawParameters);

                return headerBdc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class LLAckNackBaseMessage : LowLevelMessage
        {
            protected LowLevelCommand _responseToCommand;
            protected int _intParam;
            protected string _stringParam;

            private const int REQ_PART = sizeof(UInt16);
            private const int INT_PARAM_PART = sizeof(int);

            protected LLAckNackBaseMessage(LowLevelCommand lowLevelCommand)
                :base(lowLevelCommand)
            {
            }

            protected LLAckNackBaseMessage(LowLevelCommand lowLevelCommand, Data.ByteDataCarrier rawParams)
                :base(lowLevelCommand)
            {
                _rawParameters = rawParams;

                _responseToCommand = (LowLevelCommand)BitConverter.ToUInt16(rawParams.Buffer, 0);

                _intParam = BitConverter.ToInt32(rawParams.Buffer, REQ_PART);

                _stringParam = Encoding.UTF8.GetString(rawParams.Buffer, REQ_PART + INT_PARAM_PART,
                    rawParams.ActualSize - (REQ_PART + INT_PARAM_PART));
                
            }

            internal override Data.ByteDataCarrier GetRawData()
            {
                int strLen = (_stringParam ?? String.Empty).Length;
                int odl = REQ_PART + INT_PARAM_PART + strLen;

                _rawParameters = new Data.ByteDataCarrier(odl, false);

                _rawParameters.Append(BitConverter.GetBytes((UInt16)_responseToCommand),true);
                _rawParameters.Append(BitConverter.GetBytes(_intParam),true);

                if (strLen > 0)
                    _rawParameters.Append(Encoding.UTF8.GetBytes(_stringParam ?? String.Empty), true);

                odl = _rawParameters.ActualSize; // correct the ODL because of the UTF8 encoding

                var headerBdc = PrepareHeader(odl);
                headerBdc.Append(_rawParameters); //

                return headerBdc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class LLNotAcknowledgedMessage : LLAckNackBaseMessage
        {
            private LLNotAcknowledgedMessage()
                :base(LowLevelCommand.NotAcknowledged)
            {
            }

            internal LLNotAcknowledgedMessage(Data.ByteDataCarrier rawParams)
                :base(LowLevelCommand.NotAcknowledged, rawParams)
            {
               
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="responseToCommand"></param>
            /// <param name="errorCode"></param>
            /// <param name="errorMessage"></param>
            internal LLNotAcknowledgedMessage(LowLevelCommand responseToCommand, DCPError errorCode, string errorMessage)
                :this()
            {
                ResponseToCommand = responseToCommand;
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }

            public LowLevelCommand ResponseToCommand
            {
                get { return _responseToCommand; }
                private set { _responseToCommand = value; }
            }

            public DCPError ErrorCode
            {
                get { return (DCPError)_intParam; }
                private set { _intParam = (int)value; }
            }

            public string ErrorMessage
            {
                get { return _stringParam; }
                private set { _stringParam = value; }
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        private class LLAcknowledgedMessage : LLAckNackBaseMessage
        {
            private LLAcknowledgedMessage()
                :base(LowLevelCommand.Acknowledged)
            {
            }

            //private const int REQ_PART = sizeof(UInt16);
            //private const int INT_PARAM_PART = sizeof(int);

            internal LLAcknowledgedMessage(Data.ByteDataCarrier rawParams)
                :base(LowLevelCommand.Acknowledged, rawParams)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="responseToCommand"></param>
            /// <param name="intParam"></param>
            /// <param name="errorMessage"></param>
            internal LLAcknowledgedMessage(LowLevelCommand responseToCommand, int intParam, string errorMessage)
                : this()
            {
                ResponseToCommand = responseToCommand;
                IntParam = intParam;
                OptionalMessage = errorMessage;
            }

            public LowLevelCommand ResponseToCommand
            {
                get { return _responseToCommand; }
                private set { _responseToCommand = value; }
            }

            public int IntParam
            {
                get { return _intParam; }
                private set { _intParam = value; }
            }

            

            /// <summary>
            /// 
            /// </summary>
// ReSharper disable once MemberCanBePrivate.Local
            public string OptionalMessage
            {
// ReSharper disable once UnusedMember.Local
                get { return _stringParam; }
                private set { _stringParam = value; }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="lowLevelMessage"></param>
        private bool TransmitMessage(ISimpleTcpConnection connection, LowLevelMessage lowLevelMessage)
        {
            try
            {
                var bdc = lowLevelMessage.GetRawData();

                connection.Send(bdc);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferInfo"></param>
        /// <param name="success"></param>
        /// <returns>true, if the stream should be closed after the processing</returns>
        public delegate void DIncomingTransferResult(
            [NotNull]
            TransferInfo transferInfo, 
            bool success, 
            out bool closeStreamAfterProcessing);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="continueInTransfer"></param>
        /// <param name="descriptionOnDissaproval"></param>
        public delegate void DPreTransferValidation(
            string streamName,
            [NotNull]
            TransferInfo transferInfo,
            out bool continueInTransfer,
            ref string descriptionOnDissaproval);

        /// <summary>
        /// 
        /// </summary>
        public event DIncomingTransferResult IncomingTransferResult;

        /// <summary>
        /// 
        /// </summary>
        public event DTransferProgress IncomingTransferProgress;

        /// <summary>
        /// defines whether to continue in transfer
        /// either by continueInTransfer bool returned
        /// or by throwing an exception
        /// </summary>
        public event DPreTransferValidation PreTransferValidation;


        private void InvokeIncomingTransferResult(TransferInfo ti, bool success)
        {
            
            if (IncomingTransferResult != null)
                try
                {
                    bool closeStreamAfterProcessing;
                    IncomingTransferResult(ti, success, out closeStreamAfterProcessing);
                    ti._closeStreamAfterProcessing = closeStreamAfterProcessing;
                }
                catch
                {
                }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        void OnTcpDataReceived(ISimpleTcpConnection connection, Data.ByteDataCarrier data)
        {
            try
            {
                TransferInfo ti = GetReceiveTransferInfo(connection);

                //LLNotAcknowledgedMessage llNack;
                //LLAcknowledgedMessage llACK;
                LowLevelMessage llm;
                if (!ResolveBinaryFragmentation(connection, data, ti, out llm)) 
                    return;

                #region Testing fragmentation
                //#if DEBUG
                //            if (llm == null)
                //            {
                //                byte[] tmp = new byte[20];
                //                data.CopyTo(ref tmp, 0, tmp.Length);

                //                System.Diagnostics.Debug.WriteLine("\tDCP received : unparsable["+data.Length+"] "+ByteDataCarrier.HexDump(tmp));
                //            }
                //            else
                //                System.Diagnostics.Debug.WriteLine("\tDCP received : parsable[" + data.Length + "] cmd=" + llm.Command+
                //                    " odl="+llm._optionalDataLength+" realOdl="+llm._rawParameters.Length+" partial="+llm._partial);
                //#endif 
                #endregion

                if (llm != null)
                {
                    switch (llm.Command)
                    {
                        case LowLevelCommand.Acknowledged:
                            {
                                if (ti == null)
                                    connection.Disconnect();
                                else
                                {
                                    LLAcknowledgedMessage llACK = (LLAcknowledgedMessage)llm;
                                    ParseAcknowledgedMessage(ti, llACK);
                                }
                            }
                            break;
                        case LowLevelCommand.NotAcknowledged:
                            {
                                if (ti == null)
                                    connection.Disconnect();
                                else
                                {
                                    LLNotAcknowledgedMessage llNACK = (LLNotAcknowledgedMessage) llm;

                                    ParseNotAcknowledgedMessage(ti, llNACK);
                                }
                            }
                            break;
                        case LowLevelCommand.PushData:
                            {
                                LLPushDataMessage llpd = (LLPushDataMessage)llm;

                                ParsePushDataMessage(connection, ti, llpd);
                            }
                            break;
                        case LowLevelCommand.StoreMemoryStream:
                            LLStoreMemoryStreamMessage llsms = (LLStoreMemoryStreamMessage)llm;
                            
                            ParseStoreMemoryStreamMessage(ti,connection, llsms);
                            break;
                        case LowLevelCommand.StoreFile:
                            LLStoreFileMessage llsf = (LLStoreFileMessage)llm;

                            ParseStoreFileMessage(ti, connection, llsf);
                            break;
                        case LowLevelCommand.PullFile:
                            LLPullFileMessage llpf = (LLPullFileMessage) llm;

                            ParsePullFileMessage(connection, llpf);
                            break;
                        default:
                            if (ti == null)
                                connection.Disconnect();
                            else
                            {  
                                string errorMessage = String.Empty;
                                try
                                {
                                    errorMessage = "Unexpected command " + llm.Command;
                                }
                                catch
                                {
                                }
                                ti.SetLastError(DCPError.ProtocolProblem, errorMessage);

                                var llNACK = new LLNotAcknowledgedMessage(llm.Command,
                                    ti.LastErrorCode, errorMessage);
                                TransmitMessage(connection, llNACK);

                            }
                            break;
                    }
                }
                else
                {
                    if (ti != null)
                    {
                        ti.SetLastError(DCPError.ProtocolProblem, "Unable to parse incoming message");
                        
                        var llNACK = new LLNotAcknowledgedMessage(LowLevelCommand.Unknown,
                            ti.LastErrorCode, ti.LastErrorMessage);

                        TransmitMessage(connection, llNACK);

                        
                    }
                    else
                        // invalid parsing
                        // probably unknown protocol
                        connection.Disconnect();
                }

            }
            catch (Exception generalError)
            {
                Sys.HandledExceptionAdapter.Examine(generalError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferInfo"></param>
        /// <param name="connection"></param>
        /// <param name="requestCommand"></param>
        /// <param name="streamName"></param>
        /// <returns></returns>
        private bool ValidationBeforeTransfer(
            [NotNull]
            TransferInfo transferInfo,
            ISimpleTcpConnection connection,
            LowLevelCommand requestCommand,
            string streamName)
        {
            if (null != PreTransferValidation)
            {
                bool continueInTransfer = true;
                string descriptionOnDissaproval = null;
                Exception disapprovalByException = null;

                try
                {
                    PreTransferValidation(
                        streamName, 
                        transferInfo,
                        out continueInTransfer, 
                        ref descriptionOnDissaproval);
                }
                catch (Exception e)
                {
                    disapprovalByException = e;
                }

                if (continueInTransfer && disapprovalByException == null)
                    return true;

                LLNotAcknowledgedMessage llNACK;

                if (disapprovalByException != null)
                {
                    llNACK = new LLNotAcknowledgedMessage(
                        requestCommand,
                        DCPError.TransferProhibited,
                        disapprovalByException.Message
                        );
                }
                else
                {
                    llNACK = new LLNotAcknowledgedMessage(
                        requestCommand,
                        DCPError.TransferProhibited,
                        descriptionOnDissaproval ?? (
                            "Transfer of \"" + streamName + "\" prohibited at " + transferInfo.TransferSide + " side")
                        );

                    
                }

                TransmitMessage(connection, llNACK);
                
                return false;
            }
            return true;
        }

        private void ParsePullFileMessage(ISimpleTcpConnection connection, LLPullFileMessage llpf)
        {
            var ti = new TransferInfo(TransferSide.PullSender)
            {
                _remoteIPEndPoint = connection.RemoteEndPoint,
                _localIPEndPoint = connection.LocalEndPoint,
                //_stream = fsSource, // filled later
                _destinationName = String.Empty,
                _connection = connection,
                _overwrite = true,
                //_overallSize = (int)fsSource.Length, // filled later
                _storeAsFile = llpf.PullAsFile
            };

            if (!ValidationBeforeTransfer(
                ti,
                connection,
                llpf.Command,
                llpf.FilePath
                ))
                return;

            if (!File.Exists(llpf.FilePath))
            {
                var llNACK = new LLNotAcknowledgedMessage(llpf.Command, DCPError.FileDoesNotExists,
                    string.Format("File {0} does not exists", llpf.FilePath));

                TransmitMessage(connection, llNACK);
            }
            else
            {
                try
                {
                    FileStream fsSource = new FileStream(llpf.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    if (_isCompactFramework)
                        fsSource.Flush();

                    // originally the TransferInfo was created here
                    ti._stream = fsSource;
                    ti._overallSize = (int) fsSource.Length;

                    _peers[ti.ToString()] = ti;

                    var sft = new Threads.SafeThread<TransferInfo, ISimpleTcpConnection>(TransferingThread);
                    ti._transferThread = sft;
                    sft.Start(ti, connection);
                }
                catch (Exception error)
                {
                    var llNACK = new LLNotAcknowledgedMessage(llpf.Command,
                        DCPError.ExceptionThrown, error.Message);

                    TransmitMessage(connection, llNACK);

                    try
                    {
                        ti.SetResult(0, error);
                    }
                    catch
                    {

                    }
                }
            }
        }



        private void ParseStoreFileMessage(TransferInfo ti, ISimpleTcpConnection connection, LLStoreFileMessage llsf)
        {
            bool isPushReceiver;

            if (ti == null)
            {
                isPushReceiver = true;
                ti = new TransferInfo(TransferSide.PushReceiver)
                {
                    //_transferStart = DateTime.Now,
                    _remoteIPEndPoint = connection.RemoteEndPoint,
                    //_stream = fs,
                    _streamType = StreamType.File,
                    _destinationName = llsf.FilePath,
                    _overallSize = llsf.OverallSize
                };
            }
            else
                isPushReceiver = false;

            if (!ValidationBeforeTransfer(
                ti,
                connection,
                llsf.Command,
                llsf.FilePath
                ))
                return;

            if (isPushReceiver)
                if (!llsf.Overwrite && File.Exists(llsf.FilePath))
                {
                    var llNACK = new LLNotAcknowledgedMessage(llsf.Command, DCPError.FileAlreadyExists,
                        string.Format("File {0} already exists", llsf.FilePath));

                    TransmitMessage(connection, llNACK);
                    return;
                }

            
            FileStream fs = null;

            try
            {
                if (!isPushReceiver && ti._stream != null)
                    fs = (FileStream) ti._stream;
                else
                {
                    fs = new FileStream(
                        llsf.FilePath,
                        FileMode.Create, 
                        FileAccess.ReadWrite, 
                        FileShare.ReadWrite, 
                        OPTIMIZED_FILE_STREAM_BUFFER_SIZE);

                    if (_isCompactFramework)
                        fs.Flush();
                }

                // inital ACK
                var llACK = new LLAcknowledgedMessage(llsf.Command, (int) fs.Position, null);

                if (TransmitMessage(connection, llACK))
                {
                    if (isPushReceiver)
                    {
                        ti._transferStart = DateTime.Now;
                        ti._stream = fs;

                        _peers[ti.ToString()] = ti;
                    }
                    else
                    {
                        // in this case, the transfer info should already be of TransferSide.PullInitiator
                        if (ti.TransferSide != TransferSide.PullInitiator)
                            DebugHelper.NOP(ti.TransferSide);

                        ti._transferStart = DateTime.Now;
                        ti._transferPhase = TransferPhase.DataPulling;
                        ti._overallSize = llsf.OverallSize;
                    }
                }

            }
            catch (ThreadAbortException tae)
            {
                Sys.HandledExceptionAdapter.Examine(tae);

                if (fs != null)
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                    }
            }
            catch (Exception ioError)
            {
                Sys.HandledExceptionAdapter.Examine(ioError);

                var llNACK = new LLNotAcknowledgedMessage(llsf.Command, DCPError.FileCannotBeCreated, ioError.Message);
                TransmitMessage(connection, llNACK);

                if (fs != null)
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                    }
            }
        }

        private void ParseStoreMemoryStreamMessage(TransferInfo ti,ISimpleTcpConnection connection, LLStoreMemoryStreamMessage llsms)
        {
            bool isPushReceiver;

            if (ti == null)
            {
                isPushReceiver = true;
                ti = new TransferInfo(TransferSide.PushReceiver)
                {
                    //_transferStart = DateTime.Now,
                    _remoteIPEndPoint = connection.RemoteEndPoint,
                    //_stream = ms,
                    _streamType = StreamType.Memory,
                    _destinationName = llsms.StreamName,
                    _overallSize = llsms.OverallSize
                };
            }
            else
                isPushReceiver = false;

            if (!ValidationBeforeTransfer(
                ti,
                connection,
                llsms.Command,
                llsms.StreamName
                ))
                return;

            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();

                // inital ACK
                var llACK = new LLAcknowledgedMessage(llsms.Command, (int) ms.Position, null);

                if (TransmitMessage(connection, llACK))
                {
                    if (isPushReceiver)
                    {
                        ti._transferStart = DateTime.Now;
                        ti._stream = ms;

                        _peers[ti.ToString()] = ti;
                    }
                    else
                    {
                        // in this case, the transfer info should already be of TransferSide.PullInitiator
                        if (ti.TransferSide != TransferSide.PullInitiator)
                            DebugHelper.NOP(ti.TransferSide);

                        ti._stream = ms;
                        //ti._streamType = StreamType.Memory;
                        ti._transferStart = DateTime.Now;
                        ti._transferPhase = TransferPhase.DataPulling;
                        ti._overallSize = llsms.OverallSize;
                    }
                }
            }
            catch (ThreadAbortException tae)
            {
                Sys.HandledExceptionAdapter.Examine(tae);

                if (ms != null)
                    try
                    {
                        ms.Close();
                    }
                    catch
                    {
                    }
            }
            catch (Exception ioError)
            {
                Sys.HandledExceptionAdapter.Examine(ioError);

                var llNACK = new LLNotAcknowledgedMessage(llsms.Command, DCPError.MemoryStreamCannotBeCreated, ioError.Message);
                TransmitMessage(connection, llNACK);

                if (ms != null)
                    try
                    {
                        ms.Close();
                    }
                    catch
                    {
                    }
            }
        }

        private void ParseNotAcknowledgedMessage(TransferInfo ti, LLNotAcknowledgedMessage llNACK)
        {
            switch (ti._transferPhase)
            {
                case TransferPhase.DataPushing:
                    ti._problemRaceCondition++;

                    if (ti._problemRaceCondition > IoOperationRetryCount || 
                        llNACK.ErrorCode == DCPError.IOPerminentError)
                    {
                        // replaced by ti.SetResult mutex signalling
                        //ti._breakFromPushDataCycle = true;

                        string errorMessage = null;
                        try
                        {
                            errorMessage = llNACK.ErrorMessage ??
                                           "Command " + llNACK.ResponseToCommand +
                                           " was not acknowledged by remote side after " +
                                           IoOperationRetryCount + " retries";
                        }
                        catch
                        {

                        }

                        ti.SetResult(0, llNACK.ErrorCode, errorMessage);
                    }
                    else
                    {
                        ti._stream.Seek(ti._originalStreamPosition, SeekOrigin.Begin);
                        if (llNACK.ResponseToCommand == LowLevelCommand.Unknown ||
                            llNACK.ErrorCode == DCPError.DataFragmentationProblem ||
                            llNACK.ErrorCode == DCPError.DataInconsistentTransfer ||
                            llNACK.ErrorCode == DCPError.ProtocolProblem ||
                            llNACK.ErrorCode == DCPError.IOOperationFailed ||
                            llNACK.ErrorCode == DCPError.DataChecksumInvalid)
                            Thread.Sleep(GRACEFUL_DELAY); // graceful period 
                        ReadStreamAndPush(ti,llNACK);

                        
                    }
                    break;
                // includes InitialPullCommand, InitialPushCommand
                default:
                    {
                        string errorMessage = null;
                        try
                        {
                            errorMessage = llNACK.ErrorMessage ??
                                           ("Command " + llNACK.ResponseToCommand +
                                            " was not acknowledged by remote side");
                        }
                        catch
                        {
                            
                        }
                        ti.SetResult(0, llNACK.ErrorCode, errorMessage);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="llACK"></param>
        private void ParseAcknowledgedMessage(TransferInfo ti, LLAcknowledgedMessage llACK)
        {
            switch (ti._transferPhase)
            {
                case TransferPhase.InitialPushCommand:

                    if ((llACK.ResponseToCommand != LowLevelCommand.StoreFile && ti._storeAsFile) ||
                        (llACK.ResponseToCommand != LowLevelCommand.StoreMemoryStream &&
                         !ti._storeAsFile) ||
                        llACK.IntParam != 0)
                    {
                        string errorMessage = null;
                        try
                        {
                            errorMessage = 
                                string.Format("Different acknowledge : {0}/{1}", llACK.ResponseToCommand, llACK.IntParam);
                        }
                        catch
                        {
                            
                        }

                        ti.SetResult(0, DCPError.ProtocolProblem, errorMessage);

                        //InvokeTransferResult(ti,false);
                    }
                    else
                    {
                        ti._transferStart = DateTime.Now;
                        // replaced by ti.SetResult mutex signalling
                        //ti._breakFromPushDataCycle = false;
                        ti._problemRaceCondition = 0;
                        ti._previousPercentage = 0;

                        if (ti.TransferSide == TransferSide.PushInitiator &&
                            null != TransferProgress)
                            ti._progressingThread = Threads.SafeThread<TransferInfo>.StartThread(ProgressingThread, ti);

                        ReadStreamAndPush(ti,llACK);
                    }
                    break;
                case TransferPhase.DataPushing:
                    if (llACK.IntParam == ti._stream.Position)
                    {
                        ti._problemRaceCondition = 0;

                        if (llACK.IntParam == ti._overallSize)
                        {
                            // TODO : verify necessity of the condition
                            //if (!string.IsNullOrEmpty(llACK.OptionalMessage))
                            {
                                
                                ti._transferEnd = DateTime.Now;
                                // this must be set before the actual result is set
                                ti._transferProgress = 100;

                                ti.SetResult(1, DCPError.Unknown, String.Empty);

                                //InvokeTransferResult(ti, true);
                            }

                            // replaced by ti.SetResult mutex signalling
                            //ti._breakFromPushDataCycle = true;
                        }
                        else
                        {
                            RefreshProgressPercentage(ti, llACK.IntParam);

                            ReadStreamAndPush(ti,llACK);
                        }
                    }
                    else
                    {
                        // when acknowledge invalid
                        ti._problemRaceCondition++;
                        if (ti._problemRaceCondition <= IoOperationRetryCount)
                        {
                            ti._stream.Seek(ti._originalStreamPosition, SeekOrigin.Begin);

                            ReadStreamAndPush(ti,llACK);
                        }
                        else
                        {
                            // replaced by ti.SetResult mutex signalling
                            //ti._breakFromPushDataCycle = true;

                            string errorMessage = null;
                            try
                            {
                                errorMessage =
                                    "Expected position " +
                                    ti._stream.Position + " instead " + llACK.IntParam + " on " +
                                    ti._destinationName + " after " + IoOperationRetryCount + " retries";
                            }
                            catch
                            {

                            }

                            ti.SetResult(0, DCPError.DataInconsistentTransfer, errorMessage);
                            
                        }
                    }

                    break;
            }
        }

        private void ReadStreamAndPush(TransferInfo ti,LowLevelMessage incomingMessage)
        {
            // condition ti._overallSize==0 should be present so the InitialPush-ACK sequence is proceed even for empty files
            if ((ti._stream.Position != ti._overallSize || ti._overallSize == 0) &&
                //!ti._breakFromPushDataCycle &&
                !ti._stopTransfer)
            {
                ti._originalStreamPosition = (int) ti._stream.Position;
                int read = 0;

                Exception ioException = null;
                int k;
                for (k=0;k<_ioOperationRetryCount;k++)
                    try
                    {
                        read = ti._stream.Read(ti._tmpBuffer, 0, SHRINKED_STREAM_READING_SIZE);
                        ioException = null;
                        break;
                    }
                    catch (IOException error)
                    {
                        try
                        {
                            ti._stream.Seek(ti._originalStreamPosition, SeekOrigin.Begin);
                        }
                        catch(IOException seekError)
                        {
                            ioException = seekError;
                            // to mark generating the NACK in condition after this loop
                            k = _ioOperationRetryCount;
                            break;
                        }
                        ioException = error;
                        Thread.Sleep(GRACEFUL_DELAY);
                    }

                if (k >= _ioOperationRetryCount && ioException != null)
                {
                    LLNotAcknowledgedMessage llNACK = new LLNotAcknowledgedMessage(incomingMessage.Command,
                        DCPError.IOPerminentError, ioException.Message);
                    TransmitMessage(ti._connection, llNACK);

                    ti.SetResult(0, ioException);

                    return;
                }
                

                int crc = -1;

                if (_useCrcForTransfer)
                {
                    crc = (int) Crypto.Crc32.ComputeChecksum(ti._tmpBuffer, 0, read);
                }

                LLPushDataMessage llpd = new LLPushDataMessage(ti._originalStreamPosition, crc,
                    ti._tmpBuffer, read);

                ti._transferPhase = TransferPhase.DataPushing;

                if (!ti._stopTransfer)
                    TransmitMessage(ti._connection, llpd);
            }
        }

        int _fucker = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ti"></param>
        /// <param name="llpd"></param>
        private void ParsePushDataMessage(ISimpleTcpConnection connection, TransferInfo ti, LLPushDataMessage llpd)
        {
            if (ti == null)
            {
                // no reason to deal with NACK, because there is not previous context info about the connection anyway
                connection.Disconnect();
            }
            else
            {
                try
                {
                    if (llpd.Offset != ti._stream.Position)
                        ti._stream.Seek(llpd.Offset, SeekOrigin.Begin);

                    ti._stream.Write(llpd.Data, 0, llpd.Data.Length);
                    if (_isCompactFramework)
                        ti._stream.Flush();

                    int positionAfterWrite = (int) ti._stream.Position;

                    bool checksumOkOrnNotUsed = true;
                    int byteCountToVerify = -1;

                    if (llpd.CrcUsed)
                    {
                        //#if DEBUG
                        //                                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        //#endif

                        byteCountToVerify = positionAfterWrite - llpd.Offset;
                        ti._stream.Seek(llpd.Offset, SeekOrigin.Begin);


                        uint computedChecksum = Crypto.Crc32.ComputeChecksum(ti._stream, byteCountToVerify);
                        //#if DEBUG
                        //                                    long took = sw.ElapsedMilliseconds;
                        //                                    System.Diagnostics.Debug.WriteLine("DCP crc count took : "+took);
                        //#endif

                        ti._stream.Position = positionAfterWrite;

                        if (computedChecksum != llpd.Crc32)
                        {
                            checksumOkOrnNotUsed = false;
                        }
                    }

                    if (_fucker++%7 == 0)
                        checksumOkOrnNotUsed = false;

                    if (checksumOkOrnNotUsed)
                    {
                        string tmp = null;
                        LLAcknowledgedMessage llACK;

                        //bool finished = false;
                        
                        //System.Diagnostics.Debug.WriteLine(ti._overallSize+" "+ti._stream.Position);

                        bool sendPostAck = true;

                        if (ti._overallSize == ti._stream.Position)
                        {
                            //finished = true;

                            switch (ti._streamType)
                            {
                                case StreamType.File:
                                    tmp = string.Format("File {0} stored", ti._destinationName);
                                    break;
                                case StreamType.Memory:
                                    tmp = string.Format("MemoryStream {0} transferred", ti._destinationName);
                                    break;
                                default:
                                    tmp = "Not supported yet";
                                    break;
                            }

                            if (ti.TransferSide == TransferSide.PullInitiator)
                            {
                                // in pull mode, the result must be marked before the client closes the connection
                                llACK = new LLAcknowledgedMessage(llpd.Command, positionAfterWrite, tmp);
                                TransmitMessage(connection, llACK);
                                sendPostAck = false;
                            }

                            ti._transferEnd = DateTime.Now;

                            // result needs to be marked before the ACK is sent back to client
                            // as the client side can close the connection, thus invoking ThreadAbortException
                            // over this thread
                            ti.SetResult(1, DCPError.Unknown, String.Empty);

                            
                        }

                        if (ti.TransferSide == TransferSide.PullInitiator)
                        {
                            RefreshProgressPercentage(ti, positionAfterWrite);
                        }

                        if (sendPostAck)
                        {
                            llACK = new LLAcknowledgedMessage(llpd.Command, positionAfterWrite, tmp);
                            TransmitMessage(connection, llACK);
                        }
                    }
                    else
                    {
                        string errorMessage = String.Empty;
                        try
                        {
                            errorMessage = "Crc invalid on block starting " + llpd.Offset + " with length " +
                                                   byteCountToVerify;
                        }
                        catch
                        {
                        }

                        ti.SetLastError(DCPError.DataChecksumInvalid, errorMessage);

                        var llNack = new LLNotAcknowledgedMessage(llpd.Command,
                            ti.LastErrorCode, errorMessage);

                        TransmitMessage(connection, llNack);
                    }
                }
                catch (ThreadAbortException tae)
                {
                    ti.SetLastError(DCPError.TransferStopped,"Reception thread has been stopped");

                    Sys.HandledExceptionAdapter.Examine(tae);
                }
                catch (Exception error)
                {
                    ti.SetLastError(error);

                    Sys.HandledExceptionAdapter.Examine(error);

                    var llNack = new LLNotAcknowledgedMessage(llpd.Command, DCPError.IOOperationFailed,
                        error.Message);
                    TransmitMessage(connection, llNack);
                }
                finally
                {
                    // intentionally moved here for cases when this thread is about
                    // to be Aborted (ThreadAbortException) by the client disconnection 
                    // after reception of last ACK confirming successful write
                    if (ti.Result > 0)
                    {
                        ResetTransferStreamPosition(ti);

                        ti.SetLastError(DCPError.Unknown,String.Empty);

                        InvokeIncomingTransferResult(ti, true);
                        ti.TryCloseStream(false);
                    }
                }
            }
        }

        private static void RefreshProgressPercentage(TransferInfo ti, int position)
        {
            int percentage = (int)((long) position*100 / ti._overallSize);
            if (ti._previousPercentage < percentage)
            {
                ti._transferProgress = ti._previousPercentage = percentage;
            }
        }

        private static void ResetTransferStreamPosition(TransferInfo ti)
        {
            try
            {
                if (ti._stream != null && ti._stream.Position != 0)
                    ti._stream.Seek(0, SeekOrigin.Begin);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="ti"></param>
        /// <param name="llm"></param>
        /// <returns>true, if message is finalized</returns>
        private bool ResolveBinaryFragmentation(
            ISimpleTcpConnection connection,
            Data.ByteDataCarrier data, 
            TransferInfo ti,
            out LowLevelMessage llm)
        {
            llm = null;
            try
            {
                llm = LowLevelMessage.ParseRawData(data);
            }
            catch (Exception dataError)
            {
                Sys.HandledExceptionAdapter.Examine(dataError);
            }

            if (llm != null)
            {
                if (llm._partial && ti != null)
                {
                    ti._partialMessage = llm;
                    // this means that the message came fragmented, and the rest will came in next round (hopefully)
                    return false;
                }

                // further parsing of finalized message needed
            }
            else
            {
                if (ti != null && ti._partialMessage != null)
                {
                    ti._partialMessage.AppendRemainingData(data);
                    if (ti._partialMessage._partial)
                        // this means that the message came fragmented AGAIN, and the rest will came in next round (hopefully)
                        return false;
                    
                    // fragmented message should be reassembled at this moment
                    llm = ti._partialMessage;
                    ti._partialMessage = null;
                    llm.ReevaluateRawData();
                }
                else
                {
                    if (ti != null)
                    {
                        ti.SetLastError(DCPError.DataFragmentationProblem,
                            "Unparsable data received and no partial message present");

                        LLNotAcknowledgedMessage llNack = new LLNotAcknowledgedMessage(LowLevelCommand.Unknown,
                            ti.LastErrorCode, ti.LastErrorMessage);

                        TransmitMessage(connection, llNack);
                    }
                    else
                        connection.Disconnect();
                    // error state
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        void OnRemoteSideDisconnected(ISimpleTcpConnection param)
        {
            TransferInfo ti = null;

            try
            {
                string tmp = TransferInfo.GetStringIdentifier(param.LocalEndPoint, param.RemoteEndPoint);

                _peers.Remove(tmp, out ti);
            }
            catch
            {
            }

            if (ti != null)
            {
                if (ti.Result <= 0)
                {
                    if (ti.LastErrorCode == DCPError.Unknown)
                    {
                        ti.SetResult(0,DCPError.ConnectionProblem,"Remote side was disconnected");
                    }

                    if (ti.TransferSide == TransferSide.PushReceiver)
                        InvokeIncomingTransferResult(ti, false);
                }

                if (ti.TransferSide == TransferSide.PushReceiver)
                    ti.TryCloseStream(false);
            }
        }

        /// <summary>
        /// NOT USED
        /// </summary>
        /// <param name="connection"></param>
        void OnListenerClientConnected(ISimpleTcpConnection connection)
        {
            if (_onlyAllowedIPs)
            {
                bool found = false;

                try
                {
                    lock (_allowedIPsLock)
                    {
                        found = _allowedIPs.ContainsKey(connection.RemoteEndPoint.Address);
                    }
                }
                catch
                {
                }

                if (!found)
                    try
                    {
                        connection.Disconnect();
                    }
                    catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">source stream, any one that supports seeking and reading</param>
        /// <param name="destinationEndpoint">IP and port of the destination DataPeerChannel instance</param>
        /// <param name="storeAsFile">if true, the data from source stream will be stored on remote site as FileStream,
        /// otherwise as MemoryStream</param>
        /// <param name="destinationName">name of the destination MemoryStream or the absolute path for the destination FileStream</param>
        /// <param name="overwriteIfExists">relevant for storeAsFile == true, if the file with the destinationName already exists</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">if Stream source does not support seeking or reading</exception>
        /// <exception cref="ArgumentNullException">if source or destinationEndpoint is null</exception>
        /// <exception cref="ArgumentException">if the source stream is larger than 2GB</exception>
        public TransferInfo Transfer(
            [NotNull] Stream source,
            [NotNull] IPEndPoint destinationEndpoint, 
            bool storeAsFile, 
            string destinationName, 
            bool overwriteIfExists)
        {
            Validator.CheckForNull(source, "source");
            Validator.CheckForNull(destinationEndpoint, "destinationEndpoint");

            if (!source.CanRead)
                throw new InvalidOperationException("Stream must support reading");

            if (!source.CanSeek)
                throw new InvalidOperationException("Stream must support seeking");

            if (source.Length > int.MaxValue)
                throw new ArgumentException("Streams larger than 2GB are not supported");

            if (source.Position != 0)
                source.Seek(0, SeekOrigin.Begin);

            TransferInfo ti = new TransferInfo(TransferSide.PushInitiator)
            {
                _remoteIPEndPoint = destinationEndpoint,
                _stream = source,
                _destinationName = destinationName,
                _overwrite = overwriteIfExists,
                _overallSize = (int)source.Length,
                _storeAsFile = storeAsFile
            };

            var sft = new Threads.SafeThread<TransferInfo,ISimpleTcpConnection>(TransferingThread);
            ti._transferThread = sft;
            sft.Start(ti,null);

            return ti;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationEndpoint"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="storeAsFile"></param>
        /// <param name="destinationName">if storeAsFile is true, must refer to absolute/relative reachable file path</param>
        /// <returns></returns>
        public TransferInfo Pull(
            [NotNull] IPEndPoint destinationEndpoint,
            [NotNull] string sourceFileName,
            bool storeAsFile,
            string destinationName)
        {
            Validator.CheckForNull(destinationEndpoint, "destinationEndpoint");
            Validator.CheckForNull(sourceFileName, "sourceFileName");

            Stream dstStreamOnPullInitiatorSide = null;
            if (storeAsFile)
            {
                dstStreamOnPullInitiatorSide = new FileStream(
                    destinationName, 
                    FileMode.Create, 
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);

                if (_isCompactFramework)
                    dstStreamOnPullInitiatorSide.Flush();
            }

            TransferInfo ti = new TransferInfo(TransferSide.PullInitiator)
            {
                _remoteIPEndPoint = destinationEndpoint,
                _stream = dstStreamOnPullInitiatorSide,
                _sourceName = sourceFileName,
                _destinationName = destinationName,
                _storeAsFile = storeAsFile,
                _streamType = storeAsFile ? StreamType.File : StreamType.Memory
            };

            var sft = new Threads.SafeThread<TransferInfo, ISimpleTcpConnection>(TransferingThread);
            ti._transferThread = sft;
            sft.Start(ti, null);

            return ti;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationEndpoint"></param>
        /// <param name="storeAsFile"></param>
        /// <param name="destinationName"></param>
        /// <param name="overwriteIfExists"></param>
        /// <returns></returns>
        public TransferInfo BlockingTransfer(
            Stream source,
            IPEndPoint destinationEndpoint,
            bool storeAsFile, string destinationName, bool overwriteIfExists)
        {
            TransferInfo ti = Transfer(source, destinationEndpoint, storeAsFile, destinationName, overwriteIfExists);
            if (ti != null)
            {
                int maximumTimeCounted = -1;
                try
                {
                    // result in miliseconds
                    maximumTimeCounted = (int)source.Length / 1000;  // average 1kB/1000ms
                    if (maximumTimeCounted == 0)
                        maximumTimeCounted = 15000; 
                    else
                        maximumTimeCounted *= 15; // ensure multiplication of counted time

                    if (maximumTimeCounted <= _socketReadWriteTimeout)
                        maximumTimeCounted = _socketReadWriteTimeout + 500;
                }
                catch
                {
                }

                if (maximumTimeCounted > 0)
                    ti._waitForEndMutex.WaitOne(maximumTimeCounted, false);
                else
                    ti._waitForEndMutex.WaitOne();
                return ti;
            }
            
            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferInfo"></param>
        /// <param name="success"></param>
        public delegate void DTransferResult(
            [NotNull]
            TransferInfo transferInfo,
            bool success);

        /// <summary>
        /// 
        /// </summary>
        public event DTransferResult TransferResult;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferInfo"></param>
        /// <param name="progress"></param>
        public delegate void DTransferProgress(
            [NotNull]
            TransferInfo transferInfo, 
            int progress);

        /// <summary>
        /// 
        /// </summary>
        public event DTransferProgress TransferProgress;


        /// <summary>
        /// ti._result must be SET before this call
        /// </summary>
        /// <param name="ti"></param>
        private void InvokeTransferResult(TransferInfo ti)
        {
            //ti._result = success ? 1 : 0;

            ti._waitForEndMutex.Set();

            if (TransferResult != null)
                try
                {
                    TransferResult(ti, ti.Result > 0);
                }
                catch
                {
                }
        }

        /*
        /// <summary>
        /// let all exception pass up
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="lowLevelMessage"></param>
        private void TransmitMessage(Socket clientSocket, LowLevelMessage lowLevelMessage)
        {
            ByteDataCarrier transmitBdc = lowLevelMessage.GetRawData();

            //LowLevelMessage llmx = LowLevelMessage.ParseRawData(intialBdc.Buffer,intialBdc.ActualSize);

            int written = clientSocket.Send(transmitBdc.Buffer, 0, transmitBdc.ActualSize, SocketFlags.None);
            if (transmitBdc.ActualSize > 0 && written == 0)
                throw new SocketException((int)SocketError.ConnectionReset);
        }*/

        /*
        /// <summary>
        /// let all exception pass up
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private LowLevelMessage ReceiveMessage(Socket clientSocket,byte[] buffer)
        {
            int read = clientSocket.Receive(buffer);

            if (read == 0)
                throw new SocketException((int)SocketError.ConnectionReset);

            LowLevelMessage llmRsp = LowLevelMessage.ParseRawData(buffer, read);

            return llmRsp;
        }*/

        private const int DEFAULT_IO_OPERATION_RETRY_COUNT = 3;

        private int _ioOperationRetryCount = DEFAULT_IO_OPERATION_RETRY_COUNT;
        /// <summary>
        /// 
        /// </summary>
        public int IoOperationRetryCount
        {
            get { return _ioOperationRetryCount; }
            set {
                _ioOperationRetryCount = value < 0 ? DEFAULT_IO_OPERATION_RETRY_COUNT : value;
            }
        }

        //private int _initialConnectionOperationRetryCount = 3;

// ReSharper disable once ConvertToConstant.Local
        private readonly int _socketReadWriteTimeout =
#if DEBUG
                60000
#else
                15000
#endif
            ;

        private bool _useCrcForTransfer = true;
        /// <summary>
        /// 
        /// </summary>
        public bool UseCrc32ForTransferChecking
        {
            get { return _useCrcForTransfer; }
            set { _useCrcForTransfer = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RaiseTransferResultOnPullSender
        {
            get { return _raiseTransferResultOnPullSender; }
            set { _raiseTransferResultOnPullSender = value; }
        }

        private volatile bool _raiseTransferResultOnPullSender = false;

        

        private const int GRACEFUL_DELAY = 50;

        /// <summary>
        /// implementation not yet done via SimpleTcpClient, as the implementations differ on CF and big .NET
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="connection"></param>
        private void TransferingThread(TransferInfo ti,ISimpleTcpConnection connection)
        {
            SimpleTcpClient client = null;
            try
            {
                if (connection == null)
                {
// ReSharper disable once UseObjectOrCollectionInitializer
                    client = new SimpleTcpClient();
#if DEBUG
                    client.ConnectTimeout = 60000;
#else
                    client.ConnectTimeout = 15000;
#endif

                    Exception connectError = null;
                    client.Disconnected += OnRemoteSideDisconnected;
                    client.ConnectionFailed += (ipE, error) =>
                    {
                        connectError = error;
                    };

                    #region SND/RCV timeouts

                    try
                    {

                        /*#if ! COMPACT_FRAMEWORK
                                        // might not work in CF
                                        clientSocket.ReceiveTimeout = _socketReadWriteTimeout;
                                        clientSocket.SendTimeout = _socketReadWriteTimeout;
                    #endif*/
                    }
                    catch
                    {
                    }

                    #endregion

                    ti._transferPhase = TransferPhase.Connecting;
                    ti.ResetWaitingForResult();

                    client.BufferSize = DEFAULT_BUFFER_SIZE;

                    // no explicit retrying needed here, as SYN retrying is done by the TCP layer itself
                    client.Connect(ti._remoteIPEndPoint, null);
                    
                    if (!client.WaitForConnect())
                        if (connectError == null)
                            throw new SocketException((int)SocketError.TimedOut);

                    if (connectError != null)
                        throw connectError;
                        

                    ti._connection = client.SimpleTcpConnection;
                    client.DataReceived += OnTcpDataReceived;
                    ti._localIPEndPoint = client.SimpleTcpConnection.LocalEndPoint;

                    _peers[ti.ToString()] = ti;
                }

                ti._tmpBuffer = new byte[DEFAULT_BUFFER_SIZE];
                LowLevelMessage llmRequest = null;

                switch (ti.TransferSide)
                {
                    case TransferSide.PullInitiator:
                        ti._transferPhase = TransferPhase.InitialPullCommand;

                        llmRequest = new LLPullFileMessage(ti._sourceName, ti._storeAsFile);
                        break;
                    case TransferSide.PushInitiator:
                    case TransferSide.PullSender:
                        ti._transferPhase = TransferPhase.InitialPushCommand;

                        if (ti._storeAsFile)
                            llmRequest = new LLStoreFileMessage(ti._overwrite, (int)ti._stream.Length, ti._destinationName);
                        else
                            llmRequest = new LLStoreMemoryStreamMessage((int)ti._stream.Length, ti._destinationName);
                        break;
                }

                TransmitMessage(ti._connection, llmRequest);

                if (ti.TransferSide == TransferSide.PullInitiator &&
                    null != IncomingTransferProgress)
                {
                    ti._progressingThread = Threads.SafeThread<TransferInfo>.StartThread(ProgressingThread, ti);
                }

                /*
                 * LowLevelMessage llmResponse = null;
                for (int i = 0; i < _initialConnectionOperationRetryCount; i++)
                {

                    

                    llmResponse = ReceiveMessage(clientSocket, tmpBuffer);

                    if (llmResponse != null)
                        break;
                }*/

                /*if (llmResponse == null)
                {
                    ti._result = 0;
                    ti._lastErrorCode = DCPError.ProtocolProblem;
                    try { ti._lastErrorMessage = "Unable to parse initial response for " + llmRequest.Command; }
                    catch { }
                    
                    //InvokeTransferResult(ti);

                }
                else*/
                    #region Synchronous approach
                    /*switch (llmResponse.Command)
                    {
                        case LowLevelCommand.NotAcknowledged:
                            LLNotAcknowledgedMessage llNACK = (LLNotAcknowledgedMessage)llmResponse;

                            ti._result = 0;
                            ti._lastErrorCode = llNACK.ErrorCode;
                            try
                            {
                                ti._lastErrorMessage = llNACK.ErrorMessage ?? ("Command " + llNACK.ResponseToCommand + " was not acknowledged by remote side");
                            }
                            catch { }
                            
                            //InvokeTransferResult(ti,false);
                            break;
                        case LowLevelCommand.Acknowledged:
                            LLAcknowledgedMessage llACK = (LLAcknowledgedMessage)llmResponse;
                            if ((llACK.ResponseToCommand != LowLevelCommand.StoreFile && ti._storeAsFile) ||
                                (llACK.ResponseToCommand != LowLevelCommand.StoreMemoryStream && !ti._storeAsFile) ||
                                llACK.IntParam != 0)
                            {
                                
                                ti._result = 0;
                                ti._lastErrorCode = DCPError.ProtocolProblem;
                                try
                                {
                                    ti._lastErrorMessage = "Different acknowledge " + llACK.ResponseToCommand + "/" +
                                        llACK.IntParam + " received for request " + llmRequest.Command;
                                }
                                catch
                                {
                                }
                                
                                //InvokeTransferResult(ti,false);
                            }
                            else
                            {
                                try
                                {
                                    if (null != TransferProgress)
                                        ti._progressingThread = SafeThread<TransferInfo>.StartThread(ProgressingThread, ti);

                                    ti._transferStart = DateTime.Now;
                                    bool breakFromCycle = false;
                                    int problemRaceCondition = 0;
                                    int oldPercentage = 0;

                                    while (ti._stream.Position != ti._overallSize &&
                                        !breakFromCycle &&
                                        !ti._stopTransfer)
                                    {
                                        int originalPosition = (int)ti._stream.Position;
                                        int read = ti._stream.Read(tmpBuffer, 0, SHRINKED_STREAM_READING_SIZE);

                                        int crc = -1;

                                        if (_useCrcForTransfer)
                                        {
                                            crc = (int)Crc32.ComputeChecksum(tmpBuffer, 0, read);
                                        }

                                        LLPushDataMessage llpd = new LLPushDataMessage(originalPosition, crc, tmpBuffer, read);

                                        if (!ti._stopTransfer)
                                            TransmitMessage(clientSocket, llpd);
                                        else
                                            break;

                                        llmResponse = ReceiveMessage(clientSocket, tmpBuffer);

                                        if (llmResponse == null)
                                        {
                                            // parsing problem
                                            problemRaceCondition++;
                                            if (problemRaceCondition <= _ioOperationRetryCount)
                                            {
                                                Thread.Sleep(GRACEFUL_DELAY); // graceful period // probably packets distorted
                                                ti._stream.Seek(originalPosition, SeekOrigin.Begin);
                                                continue;
                                            }
                                            else
                                            {
                                                breakFromCycle = true;

                                                ti._result = 0;
                                                ti._lastErrorCode = DCPError.ProtocolProblem;
                                                ti._lastErrorMessage = "Unable to parse response for PushData request";
                                                
                                                //InvokeTransferResult(ti, false);
                                            }
                                        }
                                        else
                                            switch (llmResponse.Command)
                                            {
                                                case LowLevelCommand.Acknowledged:
                                                    llACK = (LLAcknowledgedMessage)llmResponse;
                                                    if (llACK.IntParam == ti._stream.Position)
                                                    {
                                                        problemRaceCondition = 0;

                                                        if (llACK.IntParam == ti._overallSize)
                                                        {
                                                            if (!string.IsNullOrEmpty(llACK.OptionalMessage))
                                                            {
                                                                ti._result = 1;
                                                                ti._transferEnd = DateTime.Now;
                                                                // this must be set before the actual result is set
                                                                ti._transferProgress = 100;
                                                                
                                                                //InvokeTransferResult(ti, true);
                                                            }

                                                            breakFromCycle = true;
                                                        }
                                                        else
                                                        {
                                                            int percentage = llACK.IntParam * 100 / ti._overallSize;
                                                            if (oldPercentage != percentage)
                                                            {

                                                                //#if DEBUG
                                                                //                                                        if (percentage - oldPercentage >= 3)
                                                                //                                                        {
                                                                //                                                            System.Diagnostics.Debug.WriteLine(
                                                                //                                                                DateTime.Now.ToString("HH:mm:ss.fff") +
                                                                //                                                                " Progress for " + ti._destinationName + " " + percentage + "%");
                                                                //                                                            ti._transferProgress = oldPercentage = percentage;
                                                                //                                                        }
                                                                //#else
                                                                ti._transferProgress = oldPercentage = percentage;
                                                                //#endif
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        problemRaceCondition++;
                                                        if (problemRaceCondition <= _ioOperationRetryCount)
                                                        {
                                                            ti._stream.Seek(originalPosition, SeekOrigin.Begin);
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            breakFromCycle = true;
                                                            
                                                            ti._result = 0;
                                                            ti._lastErrorCode = DCPError.DataInconsistentTransfer;
                                                            try
                                                            {
                                                                ti._lastErrorMessage = "Expected position " +
                                                                    ti._stream.Position + " instead " + llACK.IntParam + " on " +
                                                                    ti._destinationName+ " after "+_ioOperationRetryCount+" retries";
                                                            }
                                                            catch
                                                            {
                                                            }
                                                            
                                                            //InvokeTransferResult(ti, false);
                                                        }
                                                    }

                                                    break;
                                                case LowLevelCommand.NotAcknowledged:
                                                    llNACK = (LLNotAcknowledgedMessage)llmResponse;
                                                    problemRaceCondition++;
                                                    if (problemRaceCondition <= _ioOperationRetryCount)
                                                    {
                                                        ti._stream.Seek(originalPosition, SeekOrigin.Begin);
                                                        if (llNACK.ResponseToCommand == LowLevelCommand.Unknown ||
                                                            llNACK.ErrorCode == DCPError.DataFragmentationProblem ||
                                                            llNACK.ErrorCode == DCPError.DataInconsistentTransfer ||
                                                            llNACK.ErrorCode == DCPError.ProtocolProblem ||
                                                            llNACK.ErrorCode == DCPError.IOOperationFailed ||
                                                            llNACK.ErrorCode == DCPError.DataChecksumInvalid)
                                                            Thread.Sleep(GRACEFUL_DELAY); // graceful period 
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        breakFromCycle = true;
                                                        ti._result = 0;
                                                        ti._lastErrorCode = llNACK.ErrorCode;
                                                        try
                                                        {
                                                            ti._lastErrorMessage = llNACK.ErrorMessage ?? "Command " + llNACK.ResponseToCommand + 
                                                                " was not acknowledged by remote side after "+_ioOperationRetryCount+" retries";
                                                        }
                                                        catch { }
                                                        
                                                        //InvokeTransferResult(ti, false);
                                                    }
                                                    break;
                                                default:
                                                    ti._result = 0;
                                                    ti._lastErrorCode = DCPError.ProtocolProblem;
                                                    try
                                                    {
                                                        ti._lastErrorMessage = "Unexpected command " + llmResponse.Command;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    break;
                                            }
                                    }

                                }
                                catch (ThreadAbortException)
                                {
                                }
                                catch (Exception error)
                                {
                                    ti._result = 0;
                                    ti.LastException = error;
                                    
                                    //InvokeTransferResult(ti,false);
                                }
                            }


                            break;
                        default:
                            ti._result = 0;
                            ti._lastErrorCode = DCPError.ProtocolProblem;
                            try
                            {
                                ti._lastErrorMessage = "Unexpected command " + llmResponse.Command;
                            }
                            catch
                            {
                            }
                            break;
                    }*/
                    #endregion

                //while (!ti._breakFromPushDataCycle && ti.Result < 0)
                    //Thread.Sleep(500);

                ti.WaitForResult();

            }
            catch (ThreadAbortException)
            {
                ti.SetResult(0, DCPError.TransferStopped, "Transferring thread was aborted");                
            }
            catch (Exception error)
            {
                ti.SetResult(0,error);
                
                //InvokeTransferResult(ti,false);
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.DataReceived -= OnTcpDataReceived;
                        client.Disconnected -= OnRemoteSideDisconnected;
                    }
                    catch
                    {
                        
                    }
                }

                if (ti.Result < 0)
                {
                    ti.SetResult(0,
                        ti._stopTransfer ? DCPError.TransferStopped : DCPError.ProtocolProblem,
                        ti._stopTransfer ? "Transferring thread was stopped" : "Unknown protocol problem"
                        );
                }


                // Invokes have been moved here to cover also situations with ThreadAbortExceptions
                switch (ti.TransferSide)
                {
                    case TransferSide.PullInitiator:
                        if (ti.Result <= 0)
                        {
                            ResetTransferStreamPosition(ti);
                            InvokeIncomingTransferResult(ti, false);
                            ti.TryCloseStream(false);
                        }
                        //else
                        //  success variants are handled asynchronously
                        //  in the ParsePushMessageData method
                        break;
                    case TransferSide.PullSender:
                        if (_raiseTransferResultOnPullSender)
                            InvokeTransferResult(ti);

                        ti.TryCloseStream(true);
                        break;
                    case TransferSide.PushInitiator:
                        InvokeTransferResult(ti);

                        // do not close stream here, as it was opened before the Transfer call started
                        // TODO : validate other approach
                        break;
                    case TransferSide.PushReceiver:
                        // should not happen
                        DebugHelper.NOP(ti.TransferSide);
                        break;
                }                    

                if (client != null)
                    try
                    {
                        // TODO : replace by Socket Linger option
                        Thread.Sleep(1000); // gracefull period

                        if (client.IsConnected)
                            client.Disconnect();

                    }
                    catch { }
            }


        }

        /// <summary>
        /// accuracy is not that needed, progress information only
        /// </summary>
        /// <param name="ti"></param>
        private void ProgressingThread(TransferInfo ti)
        {
            switch (ti.TransferSide)
            {
                case TransferSide.PushInitiator:
                    try
                    {
                        TransferProgress(ti, ti._transferProgress);
                    }
                    catch
                    {
                    }
                    break;
                case TransferSide.PullInitiator:
                    try
                    {
                        IncomingTransferProgress(ti, ti._transferProgress);
                    }
                    catch
                    {
                    }
                    break;
                default:
                    // should not happen
                    DebugHelper.NOP(ti.TransferSide);
                    break;
            }

            // to avoid repeated calls of event over the same percentage
            int oldPercentage = ti._transferProgress; 

            while (ti.Result < 0 && !ti._stopTransfer)
            {
                Thread.Sleep(1000);

                if (!ti._stopTransfer &&
                    ti._transferProgress != oldPercentage)
                {
                    oldPercentage = ti._transferProgress;

                    if (ti.Result < 0 ||
                        (ti.Result > 0 && ti._transferProgress == 100))
                        switch (ti.TransferSide)
                        {
                            case TransferSide.PushInitiator:
                                try
                                {
                                    TransferProgress(ti, ti._transferProgress);
                                }
                                catch
                                {
                                }
                                break;
                            case TransferSide.PullInitiator:
                                try
                                {
                                    IncomingTransferProgress(ti, ti._transferProgress);
                                }
                                catch
                                {
                                }
                                break;
                            default:
                                // should not happen
                                DebugHelper.NOP(ti.TransferSide);
                                break;
                        }
                }
            }
        }

        
    }
}
