using System;
using System.IO.Ports;
using System.Threading;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Parsing;
using JetBrains.Annotations;



namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public enum SerialPortReadingMode
    {
        /// <summary>
        /// 
        /// </summary>
        Async,
        /// <summary>
        /// 
        /// </summary>
        AsyncPreemptive,
        /// <summary>
        /// 
        /// </summary>
        AsyncPerByte,
        /// <summary>
        /// 
        /// </summary>
        Timeout,
        /// <summary>
        /// 
        /// </summary>
        PreambleHeaderData,
    }

    /// <summary>
    /// 
    /// </summary>
    public class SimpleSerialPort : ISimpleSerialPort, IDisposable, IPhdEventHandler
    {
        private bool _useNativePort = true;
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public bool UseNativePort
        {
            get
            {
                return _useNativePort;
            }
            set
            {
                if (_nativePort != null || _dotNetPort !=null)
                    throw new InvalidOperationException("Serial port interface cannot be changed throughout operation");

                _useNativePort = value;
            }
        }


        private volatile SerialPort _dotNetPort = null;

        private volatile SuperSerialPort _nativePort = null;


        /// <summary>
        /// 
        /// </summary>
        public bool IsStarted
        {
            get
            {
                if (_useNativePort)
                    return null != _nativePort;
                return null != _dotNetPort;
            }
        }

        private int _bufferSize = 256;
        /// <summary>
        /// 
        /// </summary>
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                if (value > 0)
                    _bufferSize = value;
            }
        }

        private int _readTimeout = 300;
        /// <summary>
        /// read timeout in ms
        /// </summary>
        public int ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                if (value > 0)
                    _readTimeout = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BytesToRead
        {
            get
            {
                if (_useNativePort)
                    return _nativePort.BytesToRead;
                return _dotNetPort.BytesToRead;
            }
        }

        private SerialPortReadingMode _mode = SerialPortReadingMode.Async;

        private volatile SuperSerialPort.CommTimeouts _timeouts = null;
        private readonly object _timeoutsLock = new object();

        /// <summary>
        /// 
        /// </summary>
        public SuperSerialPort.CommTimeouts Timeouts
        {
            get
            {
                if (_timeouts == null)
                    lock(_timeoutsLock)
                        if (_timeouts == null)
                            _timeouts = new SuperSerialPort.CommTimeouts();

                return _timeouts;
            }
        }

        private string _portName = null;
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                Validator.CheckNullString(value);
                
                var found = false;

                var ports = SerialPort.GetPortNames();

                foreach (var portName in ports)
                {
                    var pn = portName.Replace("o",String.Empty);

                    if (Validator.IsNullString(pn))
                        continue;

                    if (pn.ToLower() == value.ToLower())
                    {
                        _portName = pn;
                        found = true;
                    }
                }

                if (!found)
                    throw new ArgumentException("Invalid or non-existent serial port");
            }
        }

        private int _baudRate = 9600;
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public int BaudRate
        {
            get { return _baudRate; }
            set {
                if (value > 0)
                {
                    if (value != _baudRate)
                    {
                        _baudRate = value;

                        if (_useNativePort)
                        {
                            if (null != _nativePort)
                                _nativePort.BaudRate = _baudRate;
                        }
                        else
                        {
                            if (null != _dotNetPort)
                                _dotNetPort.BaudRate = _baudRate;
                        }
                    }
                }
                else
                    throw new ArgumentException("Invalid baud rate");
            }
        }

        private Parity _parity = Parity.None;
        /// <summary>
        /// 
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        private int _dataBits = 8;
        /// <summary>
        /// 
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                Validator.CheckIntegerRange(value, 5, 8);

                _dataBits = value;
            }
        }

        private StopBits _stopBits = StopBits.One;
        /// <summary>
        /// 
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
            }
        }

// ReSharper disable once ConvertToConstant.Local
        private readonly Handshake _handshake = Handshake.None;

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Validator.CheckInvalidArgument(_portName, null, "Serial port was not specified");

            Validator.CheckInvalidArgument(_baudRate, 0, "Baud rate was not specified");

            if (null != _nativePort || null != _dotNetPort)
                Stop();

            if (_useNativePort)
            {
                _nativePort = new SuperSerialPort(
                _portName,
                _baudRate,
                _dataBits,
                _parity,
                _stopBits);

                _nativePort.Timeouts.CopyFrom(_timeouts);
                _nativePort.Start();
            }
            else
            {
                _dotNetPort = new SerialPort(
                    _portName,
                    _baudRate,
                    _parity,
                    _dataBits,
                    _stopBits) {Handshake = _handshake};
                //_port.ReadBufferSize = _bufferSize;
                //_port.WriteBufferSize = _bufferSize;
                _dotNetPort.Open();
            }
            
            if (_preambleHeaderDataParser != null)
                _preambleHeaderDataParser.Start();

            PrepareRead();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void Start(SerialPortReadingMode mode)
        {
            _mode = mode;

            Start();
        
        }

        private void PrepareRead()
        {
            switch (_mode)
            {
                case SerialPortReadingMode.Async:
                    if (_useNativePort)
                    {
                        _nativePort.DataReceived += OnDataReceived_Async_Native;
                    }
                    else
                    {
                        _dotNetPort.ReceivedBytesThreshold = 1;
                        _dotNetPort.ReadTimeout = _readTimeout;
                        _dotNetPort.DataReceived += OnDataReceived_Async_DotNet;
                    }

                    break;
                case SerialPortReadingMode.AsyncPreemptive:
                    if (_useNativePort)
                    {
                        _nativePort.DataReceived += OnDataReceived_Preemptive_Native;
                    }
                    else
                    {
                        _dotNetPort.ReceivedBytesThreshold = 1;
                        _dotNetPort.ReadTimeout = _readTimeout;
                    }
                    break;
                case SerialPortReadingMode.AsyncPerByte:
                    if (_useNativePort)
                    {
                    }
                    else
                    {
                        _dotNetPort.ReceivedBytesThreshold = 1;
                        _dotNetPort.ReadTimeout = _readTimeout;
                        _dotNetPort.DataReceived += OnDataReceived_PerByte_DotNet;
                    }
                    break;
                case SerialPortReadingMode.PreambleHeaderData:
                    if (_useNativePort)
                    {
                        _nativePort.DataReceived += OnDataReceived_PHD_Native;
                    }
                    else
                    {
                        _dotNetPort.ReceivedBytesThreshold = 1;
                        _dotNetPort.ReadTimeout = _readTimeout;
                        _dotNetPort.DataReceived += OnDataReceived_PHD_DotNet;
                    }
                    break;
            }

            if (!_useNativePort)
                _dotNetPort.ErrorReceived += OnDotNetSerialPortErrorReceived;
            else
                _nativePort.ErrorOccured += OnNativePortErrorOccured;
        }

        void OnNativePortErrorOccured(int param)
        {
            if (null !=ErrorOccured)
                try
                {
                    ErrorOccured((SerialError)param);
                }
                catch
                {
                }

            //if (null != ErrorOccured)
            //    SafeThread<SerialError>.StartThread(ErrorOccuredThreadCall, e.EventType);
        }

#region Preamble Header data
        private PreambleHeaderDataParser _preambleHeaderDataParser = null;
// ReSharper disable once NotAccessedField.Local
        private bool _dontUseCharacterTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="expectedDataLengthFromHeader"></param>
        /// <param name="dontUseCharacterTimeout"></param>
        public void SetPreambleHeaderData(
            byte[] preamble,
            int headerLength,
            [NotNull] PreambleHeaderDataParser.DExpectedDataLengthFromHeader expectedDataLengthFromHeader,
            bool dontUseCharacterTimeout)
        {
            if (_preambleHeaderDataParser != null)
            {
                try
                {
                    _preambleHeaderDataParser.Dispose();
                }
                catch
                {
                    
                }
            }

            _preambleHeaderDataParser = new PreambleHeaderDataParser(
                preamble, 
                headerLength, 
                expectedDataLengthFromHeader,
                dontUseCharacterTimeout ? 0 : _baudRate);

            _mode = SerialPortReadingMode.PreambleHeaderData;
            _dontUseCharacterTimeout = dontUseCharacterTimeout;
            _preambleHeaderDataParser.EventHandlerGroup.Add(this); 
        }

        void OnDataReceived_PHD_DotNet(
            object sender, 
            SerialDataReceivedEventArgs e) 
        {
            if (e.EventType != SerialData.Chars)
            {
                //Console.WriteLine("Received "+e.EventType);
                return;
            }

            var bytes2read = _dotNetPort.BytesToRead;

            if (bytes2read <= 0)
                return;

            var buffer = new byte[bytes2read];

#if DEBUG
            var read = 
#endif
                _dotNetPort.Read(buffer, 0, bytes2read);

            if (_preambleHeaderDataParser != null)
                _preambleHeaderDataParser.ProcessData(
                    buffer,
                    Environment.TickCount);

#if DEBUG
            if (read < bytes2read)
                DebugHelper.TryBreak( "SimpleSerialPort Incoherrent bytes to read and bytes read",read,bytes2read);
#endif
        }

        void OnDataReceived_PHD_Native(
            ByteDataCarrier data,
            int timeStamp)
        {
            if (_preambleHeaderDataParser != null)
                _preambleHeaderDataParser.ProcessData(
                    data,
                    timeStamp);

        }

#endregion


        void OnDataReceived_Async_DotNet(object sender, SerialDataReceivedEventArgs e) 
        {
            if (e.EventType != SerialData.Chars)
                return;

            var bytes2read = _dotNetPort.BytesToRead;

            while (bytes2read > 0)
            {
                var buffer = new byte[bytes2read];
                try
                {
                    var read = _dotNetPort.Read(buffer, 0, bytes2read);
                    if (read > 0)
                    {
                        var aData = new ByteDataCarrier(buffer, 0, read);
                        bytes2read -= read;

                        FireDataReceived(
                            aData,
                            Environment.TickCount);
                    }

                }
                catch (TimeoutException)
                {
                    break;
                }
            }
            
        }

        private void OnDataReceived_Async_Native(
            ByteDataCarrier data,
            int timeStamp)
        {
            lock (_nativePort)
            {
                FireDataReceived(
                    data,
                    timeStamp);
            }
        }


        private int _preemptiveSubTimeout = 10;
        /// <summary>
        /// 
        /// </summary>
        public int PreemptiveSubTimeout
        {
            get
            {
                return _preemptiveSubTimeout;
            }
            set {
                _preemptiveSubTimeout = value <= 0 ? 10 : value;
            }
        }

        private int _preemptiveMinimalLength = 2;
        /// <summary>
        /// 
        /// </summary>
        public int PreemptiveMinimalLength
        {
            get { return _preemptiveMinimalLength; }
            set {
                _preemptiveMinimalLength = value > 0 ? value : 2;
            }
        }

        //private TimerCarrier _preemptiveTimeout = null;
// ReSharper disable once UnusedMember.Local
        void OnDataReceived_Preemptive_DotNet(
            object sender, 
            SerialDataReceivedEventArgs e) 
        {
            if (e.EventType != SerialData.Chars)
                return;

            try
            {
                _dotNetPort.DataReceived -= OnDataReceived_Preemptive_DotNet;

                var retryCount = _readTimeout / _preemptiveSubTimeout + 1; 

                while (_dotNetPort.BytesToRead > 0)
                {

                    var lastBytes2Read = _dotNetPort.BytesToRead;
                    if (lastBytes2Read < _preemptiveMinimalLength)
                    {
                        // wait additional interval
                        var iEqualInIteration = 0;
                        for (var i = 0; i < retryCount; i++)
                        {
                            Thread.Sleep(_preemptiveSubTimeout);

                            var bytes2read = _dotNetPort.BytesToRead;

                            if (bytes2read >= _preemptiveMinimalLength &&                                        
                                bytes2read == lastBytes2Read)
                                iEqualInIteration++;

                            lastBytes2Read = bytes2read;

                            if (iEqualInIteration > 1)
                                break;
                        }
                    }

                    var arBuffer = new byte[lastBytes2Read];

                    try
                    {
                        var iRead = _dotNetPort.Read(arBuffer, 0, lastBytes2Read);

                        if (iRead > 0)
                        {
                            var aData = new ByteDataCarrier(arBuffer, 0, iRead);

                            FireDataReceived(
                                aData,
                                Environment.TickCount);
                        }
                    }
                    catch (TimeoutException)
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                _dotNetPort.DataReceived += OnDataReceived_Preemptive_DotNet;
            }
        
        }
        

        void OnDataReceived_Preemptive_Native(
            ByteDataCarrier data,
            int timeStamp)
        {
            lock (_nativePort)
            {
                FireDataReceived(
                    data,
                    timeStamp);
            }
        }

        void OnDataReceived_PerByte_DotNet(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_dotNetPort)
            {
                if (e.EventType != SerialData.Chars)
                    return;

                while (_dotNetPort.BytesToRead > 0)
                {
                    var arBuffer = new byte[1];
                    try
                    {
                        var iRead = _dotNetPort.Read(arBuffer, 0, 1);
                        if (iRead > 0)
                        {
                            var aData = new ByteDataCarrier(arBuffer, 0, iRead);

                            FireDataReceived(
                                aData,
                                Environment.TickCount);
                        }
                    }
                    catch (TimeoutException)
                    {
                        break;
                    }
                }
            }
        }

// ReSharper disable once UnusedMember.Local
        private int ReadFromSerialPort(
            [NotNull] SerialPort comPort, 
            [NotNull] byte[] returnBuffer, 
            int fromPosition, 
            int waitSize, 
            int timeout)
        {
            Validator.CheckInvalidOperation(
                timeout <= 0 ||
                fromPosition < 0);

            Validator.CheckInvalidOperation(fromPosition >= returnBuffer.Length);

            Validator.CheckInvalidOperation(fromPosition + waitSize - 1 >returnBuffer.Length);

            if (waitSize > 0)
            {
                // synchronous implementation of the asynchronous wait for data
                var iStart = Environment.TickCount;
                //comPort.
                while (comPort.BytesToRead < waitSize)
                {
                    Thread.Sleep(1);
                    var iNow = Environment.TickCount;
                    var iDiff = (iNow > iStart
                        ? iNow - iStart
                        : (Int32.MaxValue - iStart) + (iNow - Int32.MinValue));

                    if (iDiff > timeout)
                        throw new TimeoutException();
                }
            }
            else
            {
                Thread.Sleep(timeout);

                if (comPort.BytesToRead > 0)
                    waitSize = comPort.BytesToRead;
                else
                    throw new TimeoutException();
            }

            return comPort.Read(returnBuffer, fromPosition, waitSize);
        }

        /// <summary>
        /// 
        /// </summary>
        public event DSerialDataReceived DataReceived;


        private void FireDataReceived(
            ByteDataCarrier data,
            int timeStamp)
        {
            if (Validator.IsNull(data))
                return;

            if (null != DataReceived)
            {
                try
                {
                    DataReceived(
                        this,
                        data,
                        data.ActualSize,
                        timeStamp);
                }
                catch(Exception e)
                {
                    Sys.HandledExceptionAdapter.Examine(e);
                }
            }

        }

        

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (null == _nativePort && null == _dotNetPort)
                return;

            

            if (_useNativePort)
            {
                if (_nativePort != null)
                try
                {
                    _nativePort.Stop();
                }
                catch { }
                finally
                {
                    _nativePort = null;
                }
            }
            else
            {
                try
                {
                    _dotNetPort.Close();
                }
                catch { }
                finally
                {
                    _nativePort = null;
                }
            }

            if (_preambleHeaderDataParser != null)
                _preambleHeaderDataParser.Stop();
        }

        private void ValidatePortOpened()
        {
            if (_useNativePort)
                Validator.CheckInvalidOperation(null == _nativePort); 
            else
                Validator.CheckInvalidOperation(null == _dotNetPort);
        }

#region Sending

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual void Send(
            [NotNull] ByteDataCarrier data)
        {
            Send(data, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isBlocking"></param>
        public virtual void Send(
            [NotNull] ByteDataCarrier data,
            bool isBlocking)
        {
            Validator.CheckForNull(data,"data");

            if (data.ActualSize == 0)
                throw new ArgumentNullException("data");

            ValidatePortOpened();

            Send(data.Buffer, data.Offset, data.ActualSize,isBlocking);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="isBlocking"></param>
        public virtual void Send(
            [NotNull] byte[] data, 
            int offset, 
            int length, 
            bool isBlocking)
        {
            Validator.CheckForNull(data,"data");

            Validator.CheckIntegerRange(offset, 0, data.Length - 1);
            ValidatePortOpened();
            

            if (_useNativePort)
            {
                lock (_nativePort)
                {
                    if (isBlocking)
                        _nativePort.BlockingWrite(data, offset, length);
                    else
                        _nativePort.Write(data, offset, length);
                }
            }
            else
            {
                lock (_dotNetPort)
                {
                    _dotNetPort.Write(data, offset, length);

                    if (isBlocking)
                        while (_dotNetPort.BytesToWrite > 0)
                        {
                            Thread.Sleep(1);
                        }
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public virtual void Send(
            [NotNull] byte[] data, 
            int offset, 
            int length)
        {
            Send(data, offset, length, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual void Send(
            [NotNull] byte[] data)
        {
            Send(data,0,data.Length,false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isBlocking"></param>
        public virtual void Send(byte[] data, bool isBlocking)
        {
            Send(data, 0, data.Length, isBlocking);
        }
#endregion

#region Signals
        /// <summary>
        /// 
        /// </summary>
        public bool RTS
        {
            get
            {
                ValidatePortOpened();

                if (_useNativePort)
                    return false; // this is implicitly disabled
                return _dotNetPort.RtsEnable;
            }
            set
            {
                ValidatePortOpened();

                if (!_useNativePort)
                    _dotNetPort.RtsEnable = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DTR
        {
            get
            {
                ValidatePortOpened();

                if (_useNativePort)
                    return false;
                return _dotNetPort.DtrEnable;
            }
            set
            {
                ValidatePortOpened();

                if (!_useNativePort)
                    _dotNetPort.DtrEnable = value;

            }
        }
#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useNativePort">if true, NativeSuperSerial.dll is used</param>
        public SimpleSerialPort(bool useNativePort)
        {
            _useNativePort = useNativePort;
        }

        /// <summary>
        /// implicit constructor with use of the native port
        /// </summary>
        public SimpleSerialPort()
            :this(true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<SerialError> ErrorOccured;

        private void OnDotNetSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (null != ErrorOccured)
                SafeThread<SerialError>.StartThread(ErrorOccuredThreadCall, e.EventType);
            //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff") + " SerialPort error " + e.EventType); 
        }

        private void ErrorOccuredThreadCall(SerialError error)
        {
            if (null != ErrorOccured)
                ErrorOccured(error);
        }

        private bool _aimedToBeDisposed = false;
        private readonly object _disposalSync = new object();
        private void EnsureDisposed()
        {
            lock (_disposalSync)
            {
                if (!_aimedToBeDisposed)
                {
                    _aimedToBeDisposed = true;

                    Stop();
                }
            }
        }

        ~SimpleSerialPort()
        {
            EnsureDisposed();
        }

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            EnsureDisposed();
        }

        #endregion

        void IPhdEventHandler.FrameAvailable(
            ByteDataCarrier message, 
            int optionalDataLength, 
            int timeStamp,
            APhdParsingContext context)
        {
            // this is already separated by a thread
            try
            {
                if (DataReceived != null)
                {
                    DataReceived(
                        this, 
                        message, 
                        optionalDataLength,
                        timeStamp);
                }
            }
            catch { }
        }

        void IPhdEventHandler.FrameScrambled(ByteDataCarrier message, int optionalDataLength, PreambleHeaderDataParser.FrameScrambledSource scrambledSource,
            APhdParsingContext context)
        {
            
        }

        void IPhdEventHandler.PhaseChanged(PreambleHeaderDataParser.ReadingPhase readingPhase, APhdParsingContext context)
        {
            
        }

        public void PurgeRx()
        {
            var startStopPhdParser = 
                _mode == SerialPortReadingMode.PreambleHeaderData
                && _preambleHeaderDataParser != null;

            if (startStopPhdParser)
                _preambleHeaderDataParser.Stop();

            if (_useNativePort)
                lock (_nativePort)
                    _nativePort.PurgeRx();
            else
                lock (_dotNetPort)
                    _dotNetPort.DiscardInBuffer();

            if (startStopPhdParser)
                _preambleHeaderDataParser.Start();
        }
    }
}
