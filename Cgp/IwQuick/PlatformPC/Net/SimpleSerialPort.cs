// implicitly disabled
// this option replaces reception SerialPort.DataReceived by internal implementation of reading thread
//#define USE_OWN_DATA_RECEIVED

using System;
using System.IO.Ports;
using System.Threading;
using Contal.IwQuick.Threads;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;
using Contal.IwQuick.Data;
using Contal.IwQuick.Parsing;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public enum SerialPortReadingMode
    {
        Async,
        AsyncPreemptive,
        AsyncPerByte,
        Timeout,
        PreambleHeaderData
    }

    public class SimpleSerialPort : ISimpleSerialPort, IPhdEventHandler
    {
        private SerialPort _serialPort = null;

        private int _bufferSize = 256;
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

        // in ms
        private int _readTimeout = Timeout.Infinite;
        public int ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set {
                _readTimeout = value > 0 ? value : Timeout.Infinite;
            }
        }

        private SerialPortReadingMode _mode = SerialPortReadingMode.Async;

        private string _portName = null;
        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                Validator.CheckNullString(value);
                
                var bFound = false;

                foreach (var strPort in SerialPort.GetPortNames())
                {
                    if (Validator.IsNullString(strPort))
                        continue;

                    if (strPort.ToLower() == value.ToLower())
                    {
                        _portName = strPort;
                        bFound = true;
                    }
                }

                Validator.CheckInvalidArgument(bFound,false,"Invalid or non-existent serial port "+value);
            }
        }

        private int _baudRate = 9600;
        public int BaudRate
        {
            get { return _baudRate; }
            set {
                if (value > 0)
                {                    
                    if (IsStarted)
                       _serialPort.BaudRate = value;

                    _baudRate = value;

                    if (!_dontUseCharacterTimeout &&
                        _preambleHeaderDataParser != null)
                        _preambleHeaderDataParser.UpdateBitsPerSecond(value);

                }
                else
                    throw new ArgumentException("Invalid baud rate");
            }
        }

        private Parity _parity = Parity.None;
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        private int _dataBits = 8;
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
        public StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
            }
        }

        private volatile Handshake _handshake = Handshake.None;

        public Handshake Handshake
        {
            get { return _handshake; }
            set { _handshake = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Validator.CheckInvalidArgument(_portName,null,"Serial port was not specified");

            Validator.CheckInvalidArgument(_baudRate,0,"Baud rate was not specified");

            if (null != _serialPort)
                Stop();

            _serialPort = new SerialPort(
                _portName,
                _baudRate,
                _parity,
                _dataBits,
                _stopBits)
            {
                Handshake = Handshake, 
                ReadBufferSize = _bufferSize, 
                WriteBufferSize = _bufferSize
            };
            _serialPort.Open();

            PrepareRead();
        
        }

        public void Start(SerialPortReadingMode mode)
        {
            _mode = mode;
            Start();
        }

#if USE_OWN_DATA_RECEIVED
        private SafeThread _readingThread = null;
#endif

        private PreambleHeaderDataParser _preambleHeaderDataParser = null;

        private void PrepareRead()
        {
#if ! USE_OWN_DATA_RECEIVED

            switch (_mode)
            {
                case SerialPortReadingMode.Async:
                    _serialPort.ReceivedBytesThreshold = 1;
                    _serialPort.ReadTimeout = _readTimeout;
                    _serialPort.DataReceived += OnDataReceived_Async;
                    break;

                case SerialPortReadingMode.AsyncPreemptive:
                    _serialPort.ReceivedBytesThreshold = 1;
                    _serialPort.ReadTimeout = _readTimeout;
                    _serialPort.DataReceived += OnDataReceived_Preemptive;
                    break;
                case SerialPortReadingMode.AsyncPerByte:
                    _serialPort.ReceivedBytesThreshold = 1;
                    _serialPort.ReadTimeout = _readTimeout;
                    _serialPort.DataReceived += OnDataReceived_PerByte;
                    break;
                case SerialPortReadingMode.PreambleHeaderData:
                    _serialPort.ReceivedBytesThreshold = 1;
                    _serialPort.ReadTimeout = _readTimeout;
                    _serialPort.DataReceived += OnDataReceived_PreambleHeaderData;
                    break;

            }

            _serialPort.ErrorReceived += OnSerialPortErrorReceived;

#else
            if (null == _readingThread)
            {
                _readingThread = new SafeThread(ReadingThread);
                _readingThread.Start();
            }

            
#endif

            
            


        }

        public event Action<SerialError> ErrorOccured;

        private void OnSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
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

        void OnDataReceived_Async(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
            {
                //Console.WriteLine("Received "+e.EventType);
                return;
            }

            var bytes2read = _serialPort.BytesToRead;

            if (bytes2read <= 0)
                return;

            var buffer = new byte[bytes2read];

            var iRead = _serialPort.Read(buffer, 0, bytes2read);

            if (iRead > 0)
            {
                var data = new ByteDataCarrier(buffer, 0, iRead);
                FireDataReceived(
                    data,
                    Environment.TickCount);
            }
        }

        void OnDataReceived_Preemptive(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_serialPort)
            {
                if (e.EventType == SerialData.Chars)
                {

                    try
                    {

                        _serialPort.DataReceived -= OnDataReceived_Preemptive;
                        
                        var iSubTimeout = _readTimeout / 10;
                        if (iSubTimeout < 1)
                            iSubTimeout = 1;

                        while (_serialPort.BytesToRead > 0)
                        {

                            var iLastActualSize = _serialPort.BytesToRead;
                            var iEqualInIteration = 0;
                            for (var i = 0; i < 10; i++)
                            {
                                Thread.Sleep(iSubTimeout);
                                var iNewActualSize = _serialPort.BytesToRead;
                                if (iNewActualSize == iLastActualSize)
                                    iEqualInIteration++;

                                iLastActualSize = iNewActualSize;

                                if (iEqualInIteration > 1)
                                    break;
                            }

                            var arBuffer = new byte[iLastActualSize];

                            try
                            {
                                var iRead = _serialPort.Read(arBuffer, 0, iLastActualSize);

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
                        _serialPort.DataReceived += OnDataReceived_Preemptive;
                    }
                }
            }
        }

        void OnDataReceived_PerByte(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_serialPort)
            {
                if (e.EventType == SerialData.Chars)
                {
                    while (_serialPort.BytesToRead > 0)
                    {
                        var arBuffer = new byte[1];
                        try
                        {
                            var iRead = _serialPort.Read(arBuffer, 0, 1);
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
        }

        public void PurgeRx()
        {
            var startStopPhdParser =
                _mode == SerialPortReadingMode.PreambleHeaderData
                && _preambleHeaderDataParser != null;

            if (startStopPhdParser)
                _preambleHeaderDataParser.Stop();

            lock (_serialPort)
                _serialPort.DiscardInBuffer();

            if (startStopPhdParser)
                _preambleHeaderDataParser.Start();
        }

#if USE_OWN_DATA_RECEIVED
        private int ReadFromSerialPort(SerialPort comPort, byte[] returnBuffer, int offset, int waitSize, int timeout)
        {
            Validator.CheckNull(comPort);
            Validator.CheckNull(returnBuffer);

            Validator.CheckInvalidOperation(
                timeout <= 0 ||
                offset < 0);

            Validator.CheckInvalidOperation(offset >= returnBuffer.Length);

            Validator.CheckInvalidOperation(offset + waitSize - 1 >returnBuffer.Length);

            if (waitSize > 0)
            {
                // synchronous implementation of the asynchronous wait for data
                int startTicks = Environment.TickCount;
                //comPort.
                while (comPort.BytesToRead < waitSize)
                {
                    Thread.Sleep(1);
                    int nowTicks = Environment.TickCount;
                    int diffTicks = (nowTicks > startTicks
                        ? nowTicks - startTicks
                        : (Int32.MaxValue - startTicks) + (nowTicks - Int32.MinValue));

                    if (diffTicks > timeout)
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

            return comPort.Read(returnBuffer, offset, comPort.BytesToRead);
        }


        private void ReadingThread()
        {
            byte[] receiveBuffer = new byte[_bufferSize];

            try
            {
                while (_listening)
                    try
                    {
                        //_port.ReadTimeout = _readTimeout;

                        int bytesRead = ReadFromSerialPort(_port, receiveBuffer, 0, 1, _readTimeout);
                            //_port.Read(aBuffer, 0, _bufferSize);
                        if (bytesRead > 0)
                        {
                            ByteDataCarrier aData = new ByteDataCarrier(receiveBuffer, bytesRead);

                            switch(_mode) {
                                case SerialPortReadingMode.PreambleHeaderData:
                                    lock (GeneralLock._sync)
                                    {
                                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff") + " !!! " + aData.HexDump());
                                    }
                                    _receivedBuffers.Enqueue(aData);
                                    break;
                                default:
                                    FireDataReceived(aData);
                                    break;
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                    }

                _readingThread = null;

            }
            catch (ThreadAbortException)
            {
            }
        }
#endif

        public event DSerialDataReceived DataReceived;
        private void FireDataReceived(
            ByteDataCarrier data,
            int timeStamp)
        {
            FireDataReceived(
                data, 
                -1,
                timeStamp);
        }

        private void FireDataReceived(
            [NotNull] ByteDataCarrier data,
            int optionalDataLength,
            int timeStamp)
        {
            if (null != DataReceived)
            {
                try
                {
                    DataReceived(
                        this, 
                        data, 
                        optionalDataLength < 0 ? data.ActualSize:optionalDataLength,
                        timeStamp);
                }
                catch
                {
                }
            }

        }

#region Sending
        private readonly object _sendingSync = new object();
        public virtual void Send([NotNull] ByteDataCarrier data)
        {
            Validator.CheckNullAndEmpty(data,"data");
            
            ValidatePortOpened();

            lock (_sendingSync)
            {
                _serialPort.Write(data.Buffer, 0, data.Length);
            }
        }

        public virtual void Send(
            [NotNull] byte[] data, 
            int offset, 
            int length, 
            bool isBlocking)
        {
            Validator.CheckForNull(data,"data");

            Validator.CheckIntegerRange(offset, 0, data.Length - 1);
            ValidatePortOpened();

            lock (_sendingSync)
            {
                //_port.DiscardOutBuffer();

                try
                {

                    _serialPort.Write(data, offset, length);

                    if (isBlocking)
                        while (_serialPort.BytesToWrite > 0)
                        {
                            Thread.Sleep(1);
                        }
                }
                catch
                {
                }
            }
        }

        public virtual void Send(byte[] data, int offset, int length)
        {
            Send(data, offset, length, false);
        }

        public virtual void Send(byte[] data)
        {
            Send(data, 0, data.Length, false);
        }

        public virtual void Send(byte[] data, bool isBlocking)
        {
            Send(data, 0, data.Length, isBlocking);
        }
#endregion
        /// <summary>
        /// exceptionless Close of the serial port
        /// </summary>
        public void Stop()
        {
            if (null == _serialPort)
                return;

#if USE_OWN_DATA_RECEIVED
            if (null != _readingThread)
            {
                try { _readingThread.Abort(); }
                catch { }
            }
#endif

            try { _serialPort.Close(); }
            catch { }
            finally { _serialPort = null; }
        }

        public bool IsStarted
        {
            get
            {
                return null != _serialPort;
            }
        }

#region Preamble Header data
        #region old parser
        /*
        private byte[] _preamble = null;
        private int _headerLength = 0;
        private DType2Type<int, Data.ByteDataCarrier> _expectedDataLengthFromHeader = null;

        public void SetPreambleHeaderData(byte[] preamble, int headerLength, DType2Type<int, Contal.IwQuick.Data.ByteDataCarrier> expectedDataLengthFromHeader)
        {
            Validator.CheckNull(preamble);
            Validator.CheckNegativeOrZeroInt(preamble.Length);
            Validator.CheckNegativeOrZeroInt(headerLength);
            Validator.CheckNull(expectedDataLengthFromHeader);

            _mode = SerialPortReadingMode.PreambleHeaderData;
            _preamble = preamble;
            _headerLength = headerLength;
            _expectedDataLengthFromHeader = expectedDataLengthFromHeader;

            if (null == _receivedBuffers)
            {
                _receivedBuffers = new Contal.IwQuick.Data.ProcessingQueue<byte[]>();
                _receivedBuffers.ItemProcessing += new Action<byte[]>(OnPreambleHeaderDataProcessing);
            }
            else
                _receivedBuffers.Clear();

        }
        
        private int FindPreamble(byte[] buffer, int index)
        {
            if (index >= buffer.Length)
                return -1;

            int found = -1;
            for (int i = index; i < buffer.Length; i++)
            {

                int j = 0;
                for (j = 0; j < _preamble.Length; j++)
                {
                    if (i + j >= buffer.Length)
                        break;

                    if (buffer[i + j] != _preamble[j])
                        break;
                }

                if (j == _preamble.Length)
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        public void SimulateTraffic(byte[] buffer)
        {
            _receivedBuffers.Enqueue(buffer);
        }

        private void AnalyzeHeaderAfterPreamble(int preambleStart, byte[] buffer)
        {
            if (preambleStart < 0)
                return;

            if (_headerBuffer == null)
                _headerBuffer = new ByteDataCarrier(_headerLength, 0);

            int headerStart = preambleStart + _preamble.Length;
            int headerEnd = headerStart + _headerLength;

            if (headerEnd <= buffer.Length)
            {
                Array.Copy(
                    buffer, headerStart, _headerBuffer.Buffer, 0, _headerLength);
                _headerBuffer.ActualSize = _headerBuffer.Size;

                ExamineHeader(headerStart, _headerLength, buffer);
            }
            else
            // if not whole header had been received
            {
                if (buffer.Length - headerStart == 0)
                {
                    // validity of the received data unclear;
                    Phase = ReadingPhase.WaitForUnclearHeader;
                    return;
                }

                Array.Copy(
                    buffer, headerStart,
                    _headerBuffer.Buffer, 0, buffer.Length - headerStart);

                _headerBuffer.ActualSize = buffer.Length - headerStart;

                Phase = ReadingPhase.WaitForHeader;

            }
        }

        private void AnalyzeRemainingHeader(byte[] buffer)
        {
            int remainingHeaderLength = _headerBuffer.Size - _headerBuffer.ActualSize;
            if (remainingHeaderLength == 0)
            {
                ExamineHeader(0, 0, buffer);
                return;
            }

            if (remainingHeaderLength <= buffer.Length)
            {
                Array.Copy(buffer, 0, _headerBuffer.Buffer, _headerBuffer.ActualSize, remainingHeaderLength);

                _headerBuffer.ActualSize = _headerBuffer.Size;

                ExamineHeader(0, remainingHeaderLength, buffer);
            }
            else
            {
                Array.Copy(buffer, 0, _headerBuffer.Buffer, _headerBuffer.ActualSize, buffer.Length);

                _headerBuffer.ActualSize += buffer.Length;

            }
        }

        private void OnPreambleHeaderDataProcessing(byte[] buffer)
        {
            //Console.WriteLine("\t"+ByteDataCarrier.HexDump(buffer));
            int preambleStart;
            switch (Phase)
            {
                case ReadingPhase.WaitForPreamble:
                    preambleStart = FindPreamble(buffer, 0);

                    AnalyzeHeaderAfterPreamble(preambleStart, buffer);

                    break;
                case ReadingPhase.WaitForUnclearHeader:
                    preambleStart = FindPreamble(buffer, 0);
                    if (preambleStart >= 0)
                        AnalyzeHeaderAfterPreamble(preambleStart, buffer);
                    else
                        AnalyzeRemainingHeader(buffer);

                    break;

                case ReadingPhase.WaitForHeader:
                    AnalyzeRemainingHeader(buffer);

                    break;
                case ReadingPhase.WaitForData:
                    AnalyzeData(0, buffer);

                    break;
            }

        }

        private Data.ProcessingQueue<byte[]> _receivedBuffers = null;

        public enum ReadingPhase
        {
            WaitForPreamble,
            WaitForUnclearHeader,
            WaitForHeader,
            WaitForData,

        }

        private ReadingPhase _readingPhase = ReadingPhase.WaitForPreamble;
        public ReadingPhase Phase
        {
            get
            {
                if (_mode != SerialPortReadingMode.PreambleHeaderData)
                    throw new InvalidOperationException("The mode " + _mode + " does not support");

                return _readingPhase;
            }
            private set
            {
                if (value != _readingPhase)
                {
                    //#if DEBUG
                    //                    Console.WriteLine("SIMPLE_SERIAL_PORT: " + _readingPhase + " -> " + value);
                    //#endif
                    _readingPhase = value;
                }
                //#if DEBUG
                //                else
                //                    Console.WriteLine("SIMPLE_SERIAL_PORT WARNING Switching to same state: " + _readingPhase );
                //#endif
            }
        }


        private ByteDataCarrier _headerBuffer = null;

        private ByteDataCarrier _dataBuffer = null;

        private int _optionalDataLength = 0;

        //Log _debugLog = new Log("SIMPLE_SERIAL_PORT", false,false, "10.0.5.16", 1234);
        */
        #endregion

        void OnDataReceived_PreambleHeaderData(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
            {
                //Console.WriteLine("Received "+e.EventType);
                return;
            }

            var bytes2read = _serialPort.BytesToRead;

            if (bytes2read <= 0)
                return;

            var buffer = new byte[bytes2read];

            var read = _serialPort.Read(buffer, 0, bytes2read);
            
            if (_preambleHeaderDataParser != null)
                _preambleHeaderDataParser.ProcessData(
                    buffer,
                    Environment.TickCount);

            Debug.Assert(read >= bytes2read, "Incoherrent bytes to read and bytes read");

           /* lock (GeneralLock._sync)
            {
                //if (GeneralLock._globalDebug1 > 30)
                    Console.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff") + " <<< " + ByteDataCarrier.HexDump(buffer, 0, read));
            }

            /*_receivedBuffers.Enqueue(buffer);*/

        }

        private bool _dontUseCharacterTimeout;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="expectedDataLengthFromHeader"></param>
        /// <param name="dontUseCharacterTimeout"></param>
        public void SetPreambleHeaderData(
            [NotNull] byte[] preamble, 
            int headerLength,
            [NotNull] PreambleHeaderDataParser.DExpectedDataLengthFromHeader expectedDataLengthFromHeader,
            bool dontUseCharacterTimeout)
        {
            // validation inside new PreambleHeaderDataParser(...)

            _preambleHeaderDataParser = 
                new PreambleHeaderDataParser(
                    preamble, 
                    headerLength, 
                    expectedDataLengthFromHeader,
                    dontUseCharacterTimeout ? 0 : _baudRate);

            _mode = SerialPortReadingMode.PreambleHeaderData;
            _dontUseCharacterTimeout = dontUseCharacterTimeout;
            _preambleHeaderDataParser.EventHandlerGroup.Add(this);
        }

        #region old parser continue
        /*
        private void AnalyzeData(int dataStart, byte[] buffer)
        {
            int usableLength = buffer.Length - dataStart;
            if (usableLength == 0)
            {
                Phase = ReadingPhase.WaitForData;
                return;
            }

            int remainingLength = _dataBuffer.Size - _dataBuffer.ActualSize;

            if (remainingLength > usableLength)
            {
                Array.Copy(buffer, dataStart, _dataBuffer.Buffer, _dataBuffer.ActualSize, usableLength);
                _dataBuffer.ActualSize += usableLength;
            }
            else
            {
                Array.Copy(buffer, dataStart, _dataBuffer.Buffer, _dataBuffer.ActualSize, remainingLength);
                _dataBuffer.ActualSize = _dataBuffer.Size;

                ComposeMessageAndRaiseEvent(SeparateRemainingBuffer(dataStart + remainingLength, buffer));
            }

        }

        private byte[] SeparateRemainingBuffer(int newBufferStart, byte[] originalBuffer)
        {
            if (null == originalBuffer ||
                originalBuffer.Length == 0)
                return null;

            if (newBufferStart >= originalBuffer.Length)
                return null;

            byte[] newBuffer = new byte[originalBuffer.Length - newBufferStart];
            Array.Copy(originalBuffer, newBufferStart, newBuffer, 0, newBuffer.Length);

            return newBuffer;
        }

        private void ExamineHeader(int headerStart, int headerByteCount, byte[] buffer)
        {
            try
            {
                _optionalDataLength = _expectedDataLengthFromHeader(_headerBuffer);
                if (_optionalDataLength < 0)
                {
                    RevertToWaitForPreamble(SeparateRemainingBuffer(headerStart + headerByteCount, buffer));
                    return;
                }
            }
            catch
            {
                RevertToWaitForPreamble(SeparateRemainingBuffer(headerStart + headerByteCount, buffer));
                return;
            }

            try
            {
                if (_optionalDataLength == 0)
                {
                    // there are no optional data
                    ComposeMessageAndRaiseEvent(SeparateRemainingBuffer(headerStart + headerByteCount, buffer));
                    return;
                }
                else
                    Phase = ReadingPhase.WaitForData;


                _dataBuffer = new ByteDataCarrier(_optionalDataLength, 0);

                int dataStart = headerStart + headerByteCount;

                AnalyzeData(dataStart, buffer);
            }
            catch
            {
                RevertToWaitForPreamble(null);
            }

        }

        private void RevertToWaitForPreamble(byte[] remainingBuffer)
        {
            Phase = ReadingPhase.WaitForPreamble;
            if (null != _headerBuffer)
                _headerBuffer.ActualSize = 0;

            if (null != _dataBuffer)
            {
                _dataBuffer.Dispose();
                _dataBuffer = null;
            }

            _optionalDataLength = 0;

            if (null != remainingBuffer &&
                remainingBuffer.Length != 0)
                OnPreambleHeaderDataProcessing(remainingBuffer);
        }

        private void ComposeMessageAndRaiseEvent(byte[] remainingBuffer)
        {
            ByteDataCarrier message = null;
            bool withData = null != _dataBuffer &&
                _dataBuffer.Length > 0;

            if (withData)
                message = new ByteDataCarrier(_headerBuffer.Length + _dataBuffer.Length, true);
            else
                message = new ByteDataCarrier(_headerBuffer.Length, true);

            Array.Copy(_headerBuffer.Buffer, message.Buffer, _headerBuffer.Length);

            if (withData)
                Array.Copy(_dataBuffer.Buffer, 0, message.Buffer, _headerBuffer.Length, _dataBuffer.Length);

            if (null != DataReceived)
                try
                {
                    DataReceived(this, message, _optionalDataLength);
                }
                catch
                {
                }

            RevertToWaitForPreamble(remainingBuffer);
        }
        */
        #endregion  // old parser continue
        #endregion

        private void ValidatePortOpened()
        {
            Validator.CheckInvalidOperation(null == _serialPort);
        }

#region Signals
        public bool RTS
        {
            get
            {
                ValidatePortOpened();

                return _serialPort.RtsEnable;
            }
            set
            {
                ValidatePortOpened();

                _serialPort.RtsEnable = value;
            }
        }

        public bool DTR
        {
            get
            {
                ValidatePortOpened();

                return _serialPort.DtrEnable;

            }
            set
            {
                ValidatePortOpened();

                _serialPort.DtrEnable = value;

            }
        }

        

        #endregion

        public static SerialPortDescriptor[] GetExtendedPortNames()
        {
            var list = new LinkedList<SerialPortDescriptor>();

            try
            {

                var wmiQuery = 
                    new ManagementObjectSearcher("select Caption from Win32_PNPEntity where ClassGuid='{4d36e978-e325-11ce-bfc1-08002be10318}'");
                foreach (var o in wmiQuery.Get())
                {
                    var instance = o as ManagementObject;
                    if (ReferenceEquals(instance,null))
                        continue;

                    var caption = instance.GetPropertyValue("Caption");
                    if (caption != null && caption.ToString().IndexOf("LPT", StringComparison.Ordinal) < 0)
                    {
                        try
                        {
                            list.AddLast(new SerialPortDescriptor( caption.ToString()));
                        }
                        catch
                        {
                        }
                    }
                }

                

                
            }
            catch
            {
                list.Clear();
                var alternativeNames = SerialPort.GetPortNames();

                foreach (var an in alternativeNames)
                {
                    if (!string.IsNullOrEmpty(an))
                    {
                        list.AddLast(new SerialPortDescriptor(an));
                    }
                }
            }

            var ret = new SerialPortDescriptor[list.Count];
            list.CopyTo(ret, 0);
            return ret;
        }

        void IPhdEventHandler.FrameAvailable(
            ByteDataCarrier message, 
            int optionalDataLength,
            int timeStamp,
            APhdParsingContext context)
        {
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
    }
}
