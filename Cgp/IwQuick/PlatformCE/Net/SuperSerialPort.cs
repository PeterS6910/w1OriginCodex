using System;
using System.IO.Ports;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using System.Threading;
using Contal.IwQuick.Data;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class SuperSerialPort:ADisposable
    {
        private volatile IntPtr _comPort;
        private volatile IntPtr _nativeReadingThread;

        
        private void IncrementalReadCallback(
            IntPtr bytes, int countReceivedBytes, int resultCode)
        {
            var timeStamp = Environment.TickCount;
            
            if (countReceivedBytes > 0)
                {

                    if (DataReceived != null)
                        try
                        {
                            var bdc = new ByteDataCarrier(countReceivedBytes,true);

                            Marshal.Copy(bytes, bdc.Buffer, 0, countReceivedBytes);
                            //for (int i = 0; i < countReceivedBytes; i++)
                            //    bdc[i] = Marshal.ReadByte(bytes, i);

                            DataReceived(
                                bdc,
                                timeStamp);

#if DEBUG
                            //Log.Singleton.Info("Received (" + countReceivedBytes + ")<<< " + bdc.HexDump() + " ||| ");
#endif
                        }
                        catch
                        {
                        }
                }
                else
                {
                    if (countReceivedBytes == 0)
                    {
                        if (null != ErrorOccured)
                            try
                            {
                                ErrorOccured(resultCode);
                            }
                            catch
                            {
                            }
                    }
                }
        }

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private NativeSuperSerial.DReceivedDataHandler _callback = null;

        private void StartNativeReadingThread()
        {
            if (null == _callback) {
                _callback = IncrementalReadCallback;
                //GC.SuppressFinalize(_callback);
            }

            _nativeReadingThread = NativeSuperSerial.StartReadingThread(_comPort, _receiveBufferSize , Marshal.GetFunctionPointerForDelegate(_callback) );
        }

        private void StopNativeReadingThread()
        {
            NativeSuperSerial.StopReadingThread(_nativeReadingThread);
        }
        
        private readonly bool _isSequentialMode = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSequentialMode
        {
            get { return _isSequentialMode; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStarted
        {
            get { return _comPort != IntPtr.Zero; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CommTimeouts
        {
            private UInt32 _readIntervalTimeout = UInt32.MaxValue - 1;

            /// <summary>
            /// 
            /// </summary>
            public UInt32 ReadIntervalTimeout
            {
                get { return _readIntervalTimeout; }
                set { _readIntervalTimeout = value; }
            }

            private UInt32 _readTimeoutConstant = UInt32.MaxValue - 1;

            /// <summary>
            /// 
            /// </summary>
            public UInt32 ReadTimeoutConstant
            {
                get { return _readTimeoutConstant; }
                set { _readTimeoutConstant = value; }
            }
            private UInt32 _readTimeoutMultiplier = UInt32.MaxValue - 1;

            /// <summary>
            /// 
            /// </summary>
            public UInt32 ReadTimeoutMultiplier
            {
                get { return _readTimeoutMultiplier; }
                set { _readTimeoutMultiplier = value; }
            }

            private UInt32 _writeTimeoutConstant = UInt32.MaxValue - 1;

            /// <summary>
            /// 
            /// </summary>
            public UInt32 WriteTimeoutConstant
            {
                get { return _writeTimeoutConstant; }
                set { _writeTimeoutConstant = value; }
            }

            private UInt32 _writeTimeoutMultiplier = UInt32.MaxValue - 1;

            /// <summary>
            /// 
            /// </summary>
            public UInt32 WriteTimeoutMultiplier
            {
                get { return _writeTimeoutMultiplier; }
                set { _writeTimeoutMultiplier = value; }
            }

            private bool _disableTimeouts = false;
            /// <summary>
            /// 
            /// </summary>
            public bool DisableTimeouts
            {
                get { return _disableTimeouts; }
                set
                {
                    _disableTimeouts = value;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="timeouts"></param>
            public void CopyFrom(CommTimeouts timeouts)
            {
                if (null == timeouts)
                    return;

                _disableTimeouts = timeouts._disableTimeouts;
                _readIntervalTimeout = timeouts._readIntervalTimeout;
                _readTimeoutConstant = timeouts._readTimeoutConstant;
                _readTimeoutMultiplier = timeouts._readTimeoutMultiplier;
                _writeTimeoutConstant = timeouts._writeTimeoutConstant;
                _writeTimeoutMultiplier = timeouts._writeTimeoutMultiplier;
            }
        }

        private int _serialPortNumber = -1;
        private readonly object _serialPortSync = new object();

        private const string SERIAL_PORT_PREFIX = "COM";

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public int PortNumber
        {
            get
            {
                lock (_serialPortSync)
                {
                    return _serialPortNumber;
                }
            }
            set
            {
                Validator.CheckNegativeOrZeroInt(value);

                lock (_serialPortSync)
                {

                    string[] availablePorts = SerialPort.GetPortNames();

                    string sp = SERIAL_PORT_PREFIX + value;
                    bool found = false;
                    foreach (string p in availablePorts)
                    {
                        if (sp == p)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        _serialPortNumber = value;
                    else
                        throw new ArgumentException("Serial port COM" + value + " does not exist");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string PortName
        {
            get
            {
                lock (_serialPortSync)
                {
                    if (_serialPortNumber > 0)
                        return SERIAL_PORT_PREFIX + _serialPortNumber;
                    
                    return string.Empty;
                }
            }
            set
            {
                Validator.CheckNullString(value);

                lock (_serialPortSync)
                {

                    string[] availablePorts = SerialPort.GetPortNames();

                    bool found = false;
                    foreach (string p in availablePorts)
                    {
                        if (value == p)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        string tmp = value.Trim().ToUpper().Replace(SERIAL_PORT_PREFIX, String.Empty);

                        _serialPortNumber = int.Parse(tmp);
                    }
                    else
                        throw new ArgumentException("Serial port COM" + value + " does not exist");
                }
            }
        }

        private void NativeReconfigure()
        {
            // is not called before Start phase
            if (_comPort != IntPtr.Zero)
                NativeSuperSerial.Configure(_comPort, _baudRate, _dataBits, _parity, StopBits);
        }

        int _baudRate = 9600;
        /// <summary>
        /// 
        /// </summary>
        public int BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                Validator.CheckNegativeOrZeroInt(value);

                _baudRate = value;

                NativeReconfigure();
            }
        }

        private readonly CommTimeouts _timeouts = new CommTimeouts();
        /// <summary>
        /// 
        /// </summary>
        public CommTimeouts Timeouts
        {
            get
            {
                return _timeouts;
            }
        }        

        int _dataBits = 8;
        /// <summary>
        /// 
        /// </summary>
        public int DataBits
        {
            get
            {
                return _dataBits;
            }
            set
            {
                Validator.CheckIntegerRange(value, 4, 8);

                _dataBits = value;

                NativeReconfigure();
            }
        }

        Parity _parity = Parity.None;
        /// <summary>
        /// 
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set
            {
                _parity = value;

                NativeReconfigure();
            }
        }

        StopBits _stopBits = StopBits.One;

        /// <summary>
        /// 
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
                NativeReconfigure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudrate"></param>
        /// <param name="dataBits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public SuperSerialPort(string portName, int baudrate, int dataBits, Parity parity, StopBits stopBits)
        {
            PortName = portName;
            BaudRate = baudrate;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortNumber"></param>
        /// <param name="baudrate"></param>
        /// <param name="dataBits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public SuperSerialPort(int serialPortNumber,int baudrate, int dataBits, Parity parity,StopBits stopBits)
        {
            PortNumber = serialPortNumber;
            BaudRate = baudrate;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortNumber"></param>
        /// <param name="baudrate"></param>
        public SuperSerialPort(int serialPortNumber, int baudrate)
        {
            PortNumber = serialPortNumber;
            BaudRate = baudrate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortNumber"></param>
        public SuperSerialPort(int serialPortNumber)
        {
            PortNumber = serialPortNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        public SuperSerialPort()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSequentialMode"></param>
        public SuperSerialPort(bool isSequentialMode)
        {
            _isSequentialMode = isSequentialMode;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ReadingMode
        {
            /// <summary>
            /// 
            /// </summary>
            ManagedReadingThread,
            /// <summary>
            /// 
            /// </summary>
            NativeReadingThread,
            /// <summary>
            /// 
            /// </summary>
            ManagedThreadWithNativePhd
        }

        private ReadingMode _readingMode;

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Start(ReadingMode.ManagedReadingThread);
        }

        private bool _phdConfigured = false;
        private byte[] _phdPreamble = null;
        private byte[] _phdHeaderBuffer = null;
        private byte[] _phdOptionalDataBuffer = null;
// ReSharper disable once NotAccessedField.Local
        private int _phdMaxOptionalDataLength = 0;
        private int _phdHeaderChecksumPosition = -1;
        private int _phdDataChecksumPosition = -1;
        private int _phdOptionalDataLengthPosition = -1;
        private bool _phdUseRxCharWaiting = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="maxOptionalDataLength"></param>
        /// <param name="headerChecksumPosition"></param>
        /// <param name="dataChecksumPosition"></param>
        /// <param name="optionalDataLengthPosition"></param>
        /// <param name="useRxCharWaiting"></param>
        /// <exception cref="OutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void ConfigurePhd(
            [NotNull] byte[] preamble,
            int headerLength,
            int maxOptionalDataLength,
            int headerChecksumPosition,
            int dataChecksumPosition,
            int optionalDataLengthPosition,
            bool useRxCharWaiting)
        {
            Validator.CheckNegativeOrZeroInt(headerLength);
            Validator.CheckNegativeOrZeroInt(maxOptionalDataLength);

            Validator.CheckForNull(preamble,"preamble");
            if (headerChecksumPosition >= 0 &&
                headerChecksumPosition >= headerLength)
                throw new OutOfRangeException(headerChecksumPosition, 0, headerLength - 1);

            if (dataChecksumPosition >= 0 &&
                dataChecksumPosition >= headerLength)
                throw new OutOfRangeException(dataChecksumPosition, 0, headerLength - 1);

            if (headerChecksumPosition >= 0 && dataChecksumPosition >= 0 &&
                headerChecksumPosition == dataChecksumPosition)
                throw new ArgumentException("Header checksum position cannot be equal to data checksum position");

            _phdPreamble = preamble;
            _phdHeaderBuffer = new byte[headerLength];
            _phdOptionalDataBuffer = new byte[maxOptionalDataLength];
            _phdMaxOptionalDataLength = maxOptionalDataLength;

            _phdHeaderChecksumPosition = headerChecksumPosition;
            _phdDataChecksumPosition = dataChecksumPosition;
            _phdOptionalDataLengthPosition = optionalDataLengthPosition;

            _phdUseRxCharWaiting = useRxCharWaiting;

            _phdConfigured = true;
        }

        private ThreadPriority _managedThreadReadingPriority = ThreadPriority.AboveNormal;
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public ThreadPriority ManagedThreadReadingPriority
        {
            get
            {
                if (_readingMode == ReadingMode.NativeReadingThread)
                    throw new InvalidOperationException();

                return _managedThreadReadingPriority;
            }
            set
            {
                if (_readingMode == ReadingMode.NativeReadingThread)
                    throw new InvalidOperationException();

                if (value != _managedThreadReadingPriority) {
                    _managedThreadReadingPriority = value;
                    if (null != _managedReadingThread)
                    {
                        _managedReadingThread.Thread.Priority = _managedThreadReadingPriority;
                    }
                }
            }
        }

        private const int DEFAULT_PHD_PARSING_ABANDON_TIME = 500;

        private int _phdParsingAbandonTime = DEFAULT_PHD_PARSING_ABANDON_TIME;
        /// <summary>
        /// 
        /// </summary>
        public int PhdParsingAbandonTime
        {
            get
            {
                return _phdParsingAbandonTime;
            }
            set {
                _phdParsingAbandonTime = value < (DEFAULT_PHD_PARSING_ABANDON_TIME / 10) ? DEFAULT_PHD_PARSING_ABANDON_TIME : value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readingMode"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Start(ReadingMode readingMode)
        {
            if (IsStarted)
                return;

            if (_serialPortNumber <= 0)
                throw new ArgumentException("No port name, neither port number has been defined");

            if (readingMode == ReadingMode.ManagedThreadWithNativePhd)
            {
                if (!_phdConfigured)
                    throw new ArgumentException("PHD parsing in not configured for " + readingMode);
                
                NativeSuperSerial.ConfigurePhd(
                    _serialPortNumber,
                    _phdPreamble, 
                    _phdHeaderBuffer.Length, 
                    _phdOptionalDataBuffer.Length, 
                    _phdOptionalDataLengthPosition, _phdDataChecksumPosition, _phdHeaderChecksumPosition,
                    _phdParsingAbandonTime);
            }


            _comPort = NativeSuperSerial.Open(_serialPortNumber);

            NativeReconfigure();

            if (_timeouts.DisableTimeouts)
            {
                /*if (_isSequentialMode)
                    throw new ArgumentException("Timeouts must be defined in sequential mode");*/
                NativeSuperSerial.DoNotUseTimeouts(_comPort);
            }
            else
                NativeSuperSerial.UseTimeouts(_comPort,
                    _timeouts.ReadIntervalTimeout, _timeouts.ReadTimeoutConstant, _timeouts.ReadTimeoutMultiplier,
                    _timeouts.WriteTimeoutConstant, _timeouts.WriteTimeoutMultiplier);

            _receiveBuffer = new byte[_receiveBufferSize];
            _readingMode = readingMode;

            if (!_isSequentialMode)
            {
                switch (_readingMode)
                {
                    case ReadingMode.ManagedReadingThread:
                    case ReadingMode.ManagedThreadWithNativePhd:
                        if (null == _managedReadingThread)
                        {
                            _managedReadingThread = new SafeThread(ManagedReadingThread, _managedThreadReadingPriority, false);
                            _managedReadingThread.Start();
                        }
                        break;
                    case ReadingMode.NativeReadingThread:
                        StartNativeReadingThread();
                        break;
                    
                }
                    
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Write([NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(data,"data");

            Write(data.Buffer, data.Offset, data.ActualSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Write(byte[] data, int offset, int length)
        {
            if (!IsStarted)
                throw new InvalidOperationException("Serial port has not been started");

            NativeSuperSerial.Write(_comPort, data, offset,length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            Write(data, 0, data.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void BlockingWrite([NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(data,"data");
            BlockingWrite(data.Buffer, data.Offset, data.ActualSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void BlockingWrite(byte[] data,int offset,int length)
        {
            if (!IsStarted)
                throw new InvalidOperationException("Serial port has not been started");

            //if (!_isSequentialMode)
            //    throw new InvalidOperationException("BlockingWrite can be called only in sequential mode");

            NativeSuperSerial.Write(_comPort, data, offset, length);
            NativeSuperSerial.WaitForTxEmpty(_comPort);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void BlockingWrite(byte[] data)
        {
            BlockingWrite(data, 0, data.Length);
        }

        /// <summary>
        /// invokes read, but handler is still called the same DataReceived event
        /// </summary>
        /// <returns></returns>
/*public int InvokeRead()
        {
            if (!IsStarted)
                throw new InvalidOperationException("Serial port has not been started");

            if (!_isSequentialMode)
                throw new InvalidOperationException("BlockingRead can be called only in sequential mode");

            int read = NativeSuperSerial.Read(_comPort, _receiveBuffer, false);
            if (read > 0)
            {
                if (DataReceived != null)
                    try
                    {
                        DataReceived(_receiveBuffer);
                    }
                    catch
                    {
                    }
            }
            return read;
        }*/
        private const int _receiveBufferSize = 512;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public int ReceiveBufferSize
        {
            get
            {
                return _receiveBufferSize;
            }
// ReSharper disable once ValueParameterNotUsed
            set
            {
                if (IsStarted)
                        throw new InvalidOperationException("Serial port has been already started. Buffer size can be change only when serial port is stopped");
            }
        }

        private byte[] _receiveBuffer = null;
        private SafeThread _managedReadingThread = null;
        private void ManagedReadingThread()
        {
            try
            {
                int resultCode = -1;
                while (true)
                {
                    //Stopwatch sw = Stopwatch.StartNew();

                    switch(_readingMode)
                    {
                        case ReadingMode.ManagedReadingThread:
                        {
                            int read = NativeSuperSerial.Read(_comPort, _receiveBuffer, true, ref resultCode);

                            var timeStamp = Environment.TickCount;
                            //long elapsed = sw.ElapsedMilliseconds;
                                if (read > 0)
                                {
                                    if (DataReceived != null)
                                        try
                                        {
                                            DataReceived(
                                                new ByteDataCarrier(_receiveBuffer, 0, read),
                                                timeStamp);
                                        }
                                        catch
                                        {
                                        }
                                }
                                else
                                {
                                    if (read == 0)
                                    {
                                        if (null != ErrorOccured)
                                            try
                                            {
                                                ErrorOccured(resultCode);
                                            }
                                            catch
                                            {
                                            }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                        }
                            break;
                        case ReadingMode.ManagedThreadWithNativePhd:
                            {
                                bool checksumValid = true;
                                int optionalDataLength = NativeSuperSerial.ReadPhd(
                                    _comPort,
                                    _serialPortNumber,
                                    _phdHeaderBuffer,
                                    _phdOptionalDataBuffer,
                                    ref checksumValid, 
                                    _phdUseRxCharWaiting,
                                    ref resultCode);

                                if (resultCode != 0)
                                    return;

                                if (null != DataReceivedPhd)
                                {
                                    ByteDataCarrier header = new ByteDataCarrier(_phdHeaderBuffer, 0, _phdHeaderBuffer.Length);
                                    ByteDataCarrier optionalData = null;

                                    //Thread.Sleep(200);

                                    if (optionalDataLength > 0)
                                    {
                                        // this should allow copying only here, and only the needed length
                                        optionalData = new ByteDataCarrier(_phdOptionalDataBuffer, 0, optionalDataLength);
                                        //Console.WriteLine("r <<< "+optionalData.HexDump());
                                    }

                                    //Thread.CurrentThread.Priority = ThreadPriority.Normal;
                                    try
                                    {
                                        DataReceivedPhd(
                                            header,
                                            optionalData,
                                            checksumValid);
                                    }
                                    catch
                                    {
                                    }
                                    //Thread.CurrentThread.Priority = _elevatedReadingPriority;

                                }

                            }
                            break;
                    }
                }
            }
            catch //(Exception e)
            {
            }
            finally
            {
                _managedReadingThread = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<int> ErrorOccured;

        public void PurgeRx()
        {
            if (_comPort != IntPtr.Zero)
                NativeSuperSerial.PurgeRx(_comPort);
        }

        /// <summary>
        /// 
        /// </summary>
        public int BytesToRead
        {
            get
            {
                if (!IsStarted)
                    return 0;

                return NativeSuperSerial.GetBytesToRead(_comPort);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<ByteDataCarrier, int> DataReceived;

        /// <summary>
        /// 
        /// </summary>
        public event Action<ByteDataCarrier, ByteDataCarrier, bool> DataReceivedPhd;

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (_comPort != IntPtr.Zero)
            {
                if (_readingMode == ReadingMode.NativeReadingThread)
                    try { StopNativeReadingThread(); }
                    catch
                    {
                    }

                try
                {
                    NativeSuperSerial.Close(_comPort);
                }
                catch
                {
                }

                if (_readingMode != ReadingMode.NativeReadingThread)
                    if ( _managedReadingThread != null )
                        _managedReadingThread.Stop(200);

                _comPort = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicit)
        {
            Stop();
        }
    }
}
