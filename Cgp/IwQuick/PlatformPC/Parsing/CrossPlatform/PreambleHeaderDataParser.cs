using System;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

#if !COMPACT_FRAMEWORK
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
#else
using Contal.IwQuickCF.Data;

#endif


#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Parsing
#else
namespace Contal.IwQuickCF.Parsing
#endif
{
    public class PreambleHeaderDataParser
    {
        private byte[] _preamble = null;
        private int _headerLength = 0;
        private DType2Type<int, Data.ByteDataCarrier> _expectedDataLengthFromHeader = null;
        private int _odlSpecificationOffset = -1;
        private int _odlSpecificationLength = -1;
        
        private float _oneByteTime = 0; //in ms
        private long _headerCharTime = 0;

        private float _charTimeAccuracy = 1.1f;
        public float CharTimeAccuracy
        {
            get
            {
                return _charTimeAccuracy;
            }
        }

        private Data.ProcessingQueue<object> _receivedBuffers = null;

        private APhdParsingContext[] _contextDisposalLevelingArray = null;

        long _countOfProcessing = 0;
        long _sumElapsedTime = 0;
        long _maxElapsedTime = 0;

        private object _readingPhaseSync = new object();
        private ReadingPhase _readingPhase = ReadingPhase.WaitForPreamble;

        public enum ReadingPhase
        {
            WaitForPreamble,
            //WaitForUnclearHeader,
            WaitForHeader,
            WaitForData,

        }

        //private object _oneByteParsingSync = new object();
        private AutoResetEvent _oneByteParsingMutex = new AutoResetEvent(true);

        private enum PhdEventType
        {
            FrameAvailable,
            FrameScrambled,
            PhaseChanged,
        }

        private class PhdEvent
        {
            public PhdEventType type;
            public ByteDataCarrier message = null;
            public int optionalDataLength = 0;
            public ReadingPhase readingPhase;
            public FrameScrambledSource frameScramblingSource;
            public APhdParsingContext actualContext = null;
        }

        private ProcessingQueue<PhdEvent> _eventProcessingQueue = new ProcessingQueue<PhdEvent>(PQMode.Synchronous);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="expectedDataLengthFromHeader"></param>
        /// <param name="bitsPerSecond"></param>
        public PreambleHeaderDataParser(byte[] preamble, int headerLength, DType2Type<int, ByteDataCarrier> expectedDataLengthFromHeader, int bitsPerSecond, int contextDisposalLevelingDepth)
        {
            Validator.CheckNull(preamble);
            Validator.CheckNegativeOrZeroInt(preamble.Length);
            Validator.CheckNegativeOrZeroInt(headerLength);
            Validator.CheckNull(expectedDataLengthFromHeader);

            _preamble = preamble;
            _headerLength = headerLength;
            _expectedDataLengthFromHeader = expectedDataLengthFromHeader;

            if (bitsPerSecond > 0)
                // this should be always called after the header length had been specified
                UpdateBitsPerSecond(bitsPerSecond);

            if (contextDisposalLevelingDepth > 0)
                _contextDisposalLevelingArray = new APhdParsingContext[contextDisposalLevelingDepth];

            _receivedBuffers = new ProcessingQueue<object>();
            _receivedBuffers.ItemProcessing += new DType2Void<object>(OnPreambleHeaderDataProcessing);

            _eventProcessingQueue.ItemProcessing += new DType2Void<PhdEvent>(OnEventProcessing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="odlSpecificationOffset"></param>
        /// <param name="odlSpecificationLength"></param>
        /// <param name="odlSpecificationIsSigned"></param>
        /// <param name="bitsPerSecond"></param>
        public PreambleHeaderDataParser(byte[] preamble, int headerLength, int odlSpecificationOffset,int odlSpecificationLength, int bitsPerSecond, int contextDisposalLevelingDepth)
        {
            Validator.CheckNull(preamble);
            Validator.CheckNegativeOrZeroInt(preamble.Length);
            Validator.CheckNegativeOrZeroInt(headerLength);
            Validator.CheckIntegerRange(odlSpecificationOffset, 0, headerLength - 1);
            Validator.CheckIntegerRange(odlSpecificationLength, 1, headerLength - odlSpecificationOffset);

            switch (odlSpecificationLength)
            {
                case 1:
                case 2:
                case 4:
                    break;
                default:
                    throw new ArgumentException("Only ODL specification lengths of 1,2,4 are supported");
            }
            
            _preamble = preamble;
            _headerLength = headerLength;
            _odlSpecificationLength = odlSpecificationLength;
            _odlSpecificationOffset = odlSpecificationOffset;           

            if (bitsPerSecond > 0)
                // this should be always called after the header length had been specified
                UpdateBitsPerSecond(bitsPerSecond);

            if (contextDisposalLevelingDepth > 0)
                _contextDisposalLevelingArray = new APhdParsingContext[contextDisposalLevelingDepth];

            _receivedBuffers = new ProcessingQueue<object>();
            _receivedBuffers.ItemProcessing += new DType2Void<object>(OnPreambleHeaderDataProcessing);

            _eventProcessingQueue.ItemProcessing += new DType2Void<PhdEvent>(OnEventProcessing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="odlSpecificationOffset"></param>
        /// <param name="odlSpecificationLength"></param>
        /// <param name="odlSpecificationIsSigned"></param>
        public PreambleHeaderDataParser(byte[] preamble, int headerLength, int odlSpecificationOffset, int odlSpecificationLength)
            :this(preamble, headerLength, odlSpecificationOffset, odlSpecificationLength, 0, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="odlSpecificationOffset"></param>
        /// <param name="odlSpecificationLength"></param>
        /// <param name="odlSpecificationIsSigned"></param>
        public PreambleHeaderDataParser(byte[] preamble, int headerLength, int odlSpecificationOffset, int odlSpecificationLength, int contextDisposalLevelingDepth)
            : this(preamble, headerLength, odlSpecificationOffset, odlSpecificationLength, 0, contextDisposalLevelingDepth)
        {
        }

        void OnEventProcessing(PreambleHeaderDataParser.PhdEvent parameter)
        {
            
            switch (parameter.type)
            {
                case PhdEventType.FrameAvailable:
                    if (null != FrameAvailable)
                        // DO TRY CATCH EXPLICITLY, SO THE PROCESSING QUEUE WONT STOP BECAUSE OF THAT
                        try
                        {
                            FrameAvailable(parameter.message, parameter.optionalDataLength,parameter.actualContext);
                        }
                        catch
                        {
                        }
                    break;
                case PhdEventType.PhaseChanged:
                    if (null != PhaseChanged)
                        // DO TRY CATCH EXPLICITLY, SO THE PROCESSING QUEUE WONT STOP BECAUSE OF THAT
                        try
                        {
                            PhaseChanged(parameter.readingPhase,parameter.actualContext);
                        }
                        catch
                        {
                        }
                    break;
                case PhdEventType.FrameScrambled:
                    if (null != FrameScrambled)
                        // DO TRY-CATCH EXPLICITLY, SO THE PROCESSING QUEUE WONT STOP BECAUSE OF THAT
                        try
                        {
                            FrameScrambled(parameter.message, parameter.optionalDataLength, parameter.frameScramblingSource,parameter.actualContext);
                        }
                        catch
                        {
                        }
                    break;
            }

        }

        public PreambleHeaderDataParser(byte[] preamble, int headerLength, DType2Type<int, ByteDataCarrier> expectedDataLengthFromHeader)
            :this(preamble,headerLength,expectedDataLengthFromHeader,0,0)
        {
        }

        public PreambleHeaderDataParser(byte[] preamble, int headerLength, DType2Type<int, ByteDataCarrier> expectedDataLengthFromHeader, int contextDisposalLevelingDepth)
            : this(preamble, headerLength, expectedDataLengthFromHeader, 0, contextDisposalLevelingDepth)
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitsPerSecond">the zero will disable the character timeout feature</param>
        public void UpdateBitsPerSecond(int bitsPerSecond)
        {
            if (bitsPerSecond > 0)
                _oneByteTime = (1 / ((float)bitsPerSecond / 8)) * 1000; // time of one byte in ms
            else
                _oneByteTime = 0;

            _headerCharTime = (long)Math.Ceiling(_headerLength * _oneByteTime * _charTimeAccuracy);
        }

        public void ProcessData(byte[] data)
        {
            if (null != data)
                _receivedBuffers.Enqueue(data);
        }

        public void ProcessData(ByteDataCarrier data)
        {
            if (null != data)
                _receivedBuffers.Enqueue(data);
        }

        public void ProcessData(byte oneByte)
        {
            _receivedBuffers.Enqueue(oneByte);
        }

        public void ProcessData(Exception error)
        {
            if (error != null)
                _receivedBuffers.Enqueue(error);
        }

        public void ProcessData(SerialError serialError)
        {
            _receivedBuffers.Enqueue(serialError);
        }

        public void ProcessData(APhdParsingContext parsingContext)
        {
            _receivedBuffers.Enqueue(parsingContext);
        }

        private const int MAX_OPTIONAL_BUFFER_LENGTH = ushort.MaxValue;

        private void SetPhase(ReadingPhase newPhase)
        {
            switch (newPhase)
            {
                case ReadingPhase.WaitForPreamble:
                    _partialMatchingPreambleCount = 0;
                    break;
                case ReadingPhase.WaitForHeader:
                    //Debug.Assert(newPhase != _readingPhase,"Why the same phase \""+newPhase+"\" ?");

                    _optionalDataLength = 0;

                    if (_headerBuffer == null)
                        _headerBuffer = new ByteDataCarrier(_headerLength);
                    else
                    {
                        _headerBuffer.Clear(true);
                    }

                    break;
                case ReadingPhase.WaitForData:
                    //Debug.Assert(newPhase != _readingPhase, "Why the same phase \"" + newPhase + "\" ?");

                    if (_dataBuffer == null)
                        _dataBuffer = new ByteDataCarrier(MAX_OPTIONAL_BUFFER_LENGTH);
                    else
                        _dataBuffer.Clear(true);
                    break;
            }

            _readingPhase = newPhase;

            InvokePhaseChanged(_readingPhase);
        }

        private void ParseOneByte(byte oneByte)
        {
            bool unlocked = _oneByteParsingMutex.WaitOne(200,false);
            //lock (_oneByteParsingSync)
            if (unlocked)
            {
                try
                {
                    ByteDataCarrier finalMessage;
                    switch (_readingPhase)
                    {
                        case ReadingPhase.WaitForPreamble:
                            if (_preamble[_partialMatchingPreambleCount] == oneByte)
                            {
                                _partialMatchingPreambleCount++;
                            }
                            else
                            {
                                if (_preamble.Length > 1 &&
                                    _preamble[0] == oneByte)
                                {
                                    _partialMatchingPreambleCount = 1;
                                }
                                else
                                    _partialMatchingPreambleCount = 0;
                            }

                            if (_partialMatchingPreambleCount == _preamble.Length)
                            {
                                SetPhase(ReadingPhase.WaitForHeader);
                            }

                            break;
                        case ReadingPhase.WaitForHeader:
                            // this must precede actual assignment, because ByteDataCarrier
                            // does not allow storing bytes to non-actual size positions
                            _headerBuffer.ActualSize++;
                            _headerBuffer[_headerBuffer.ActualSize - 1] = oneByte;

                            if (_headerBuffer.ActualSize == _headerBuffer.Size)
                            {
                                #region Examination of the optional data length from header
                                try
                                {
                                    if (null != _expectedDataLengthFromHeader)
                                    {

                                        _optionalDataLength = _expectedDataLengthFromHeader(_headerBuffer);


                                    }

                                    else
                                    {
                                        switch (_odlSpecificationLength)
                                        {
                                            case 1:
                                                _optionalDataLength = _headerBuffer[_odlSpecificationOffset];
                                                break;
                                            case 2:
                                                _optionalDataLength = BitConverter.ToInt16(_headerBuffer, _odlSpecificationOffset);
                                                break;
                                            case 4:
                                                _optionalDataLength = BitConverter.ToInt32(_headerBuffer, _odlSpecificationOffset);
                                                break;
                                            default:
                                                _optionalDataLength = -1;
                                                break;
                                        }
                                    }
                                }
                                catch
                                {
                                    _optionalDataLength = -1;
                                }
                                #endregion

                                if (_optionalDataLength == 0)
                                {
                                    finalMessage = new ByteDataCarrier(_headerBuffer, true);
                                    InvokeFrameAvailable(finalMessage, _optionalDataLength);

                                    SetPhase(ReadingPhase.WaitForPreamble);
                                }
                                else
                                {
                                    if (_optionalDataLength > 0)
                                    {
                                        SetPhase(ReadingPhase.WaitForData);
                                    }
                                    else
                                    {
                                        // frame scrambled branch
                                        finalMessage = new ByteDataCarrier(_headerBuffer, true);
                                        InvokeFrameScrambled(finalMessage, _optionalDataLength,FrameScrambledSource.ExpectedDataLengthFromHeader);

                                        SetPhase(ReadingPhase.WaitForPreamble);
                                    }
                                }
                            }

                            break;
                        case ReadingPhase.WaitForData:
                            // this must precede actual assignment, because ByteDataCarrier
                            // does not allow storing bytes to non-actual size positions
                            _dataBuffer.ActualSize++;
                            _dataBuffer[_dataBuffer.ActualSize - 1] = oneByte;


                            if (_dataBuffer.ActualSize == _optionalDataLength)
                            {
                                finalMessage = new ByteDataCarrier(_headerBuffer.Length + _dataBuffer.ActualSize);
                                finalMessage.Append(_headerBuffer);
                                finalMessage.Append(_dataBuffer);

                                InvokeFrameAvailable(finalMessage, _optionalDataLength);

                                SetPhase(ReadingPhase.WaitForPreamble);
                            }
                            break;
                    }
                }
                catch
                {
                }
                finally
                {
                    _oneByteParsingMutex.Set();
                }
            }

            if (!unlocked)
                throw new PossibleDeadlockException();
        }

        public long MaxProcessingTime
        {
            get
            {
                return _maxElapsedTime;
            }
        }

        public float AverageProcessingTime
        {
            get
            {
                return _sumElapsedTime / (float)_countOfProcessing;
            }
        }

        public void ResetStatistics() 
        {
            _countOfProcessing = 0;
            _sumElapsedTime = 0;
            _maxElapsedTime = 0;
        }

        private APhdParsingContext _actualContext = null;

        private void ParseContext(APhdParsingContext context)
        {
            bool unlocked = _oneByteParsingMutex.WaitOne(200, false);

            if (unlocked)
                try
                {
                    // DISPOSAL TACTICS
                    if (_contextDisposalLevelingArray != null &&
                        _actualContext != null)
                    {
                        // do not care about the initial nulls
                        if (_contextDisposalLevelingArray[0] != null)
                        {
                            try
                            {
                                _contextDisposalLevelingArray[0].Dispose();
                            }
                            catch
                            {
                            }
                            finally
                            {
                                _contextDisposalLevelingArray[0] = null;
                            }
                        }

                        for (int i = 0; i < _contextDisposalLevelingArray.Length - 1; i++)
                        {
                            _contextDisposalLevelingArray[i] = _contextDisposalLevelingArray[i + 1];
                        }

                        _contextDisposalLevelingArray[_contextDisposalLevelingArray.Length - 1] = _actualContext;

                    }
                    _actualContext = context;
                }
                catch
                {
                }
                finally
                {
                    _oneByteParsingMutex.Set();
                }

            if (!unlocked)
                throw new PossibleDeadlockException();
        }

        private void ParseError()
        {
            bool unlocked = _oneByteParsingMutex.WaitOne(200, false);

            if (unlocked)
            //lock (_oneByteParsingSync)
            {
                try
                {
                    if (null != FrameScrambled)
                    {
                        ByteDataCarrier finalMessage;
                        switch (_readingPhase)
                        {
                            case ReadingPhase.WaitForPreamble:
                                InvokeFrameScrambled(null, -1,FrameScrambledSource.ExternalError);
                                break;
                            case ReadingPhase.WaitForHeader:
                                finalMessage = new ByteDataCarrier(_headerBuffer, true);
                                InvokeFrameScrambled(finalMessage, -1, FrameScrambledSource.ExternalError);
                                break;
                            case ReadingPhase.WaitForData:
                                finalMessage = new ByteDataCarrier(_headerBuffer.ActualSize + _dataBuffer.ActualSize);
                                finalMessage.Append(_headerBuffer);
                                finalMessage.Append(_dataBuffer);

                                InvokeFrameScrambled(finalMessage, _optionalDataLength, FrameScrambledSource.ExternalError);
                                break;
                        }
                    }

                    SetPhase(ReadingPhase.WaitForPreamble);
                }
                catch
                {
                }
                finally
                {
                    _oneByteParsingMutex.Set();
                }
            }
            if (!unlocked)
                throw new PossibleDeadlockException();
        }

        private void OnPreambleHeaderDataProcessing(object input)
        {
            if (null == input)
                return;

            Stopwatch sw = Stopwatch.StartNew();

            if (input is byte[])
            {
                byte[] buffer = (byte[])input;
                if (buffer.Length == 0)
                    return;

                for (int i = 0; i < buffer.Length; i++)
                {
                    ParseOneByte(buffer[i]);
                }
            }
            else
            {
                if (input is ByteDataCarrier)
                {
                    ByteDataCarrier bdc = (ByteDataCarrier)input;
                    int upperBound = bdc.ActualSize;
                    
                    for (int i = bdc.Offset; i < upperBound; i++)
                    {
                        ParseOneByte(bdc.Buffer[i]);
                    }
                }
                else
                {
                    if (input is byte)
                        ParseOneByte((byte)input);
                    else {
                        if (input is APhdParsingContext)
                        {
                            ParseContext((APhdParsingContext)input);
                        }
                        else
                        {
                            if (input is SerialError || input is Exception)
                            {
                                ParseError();
                            }
                        }
                    }
                }
            }

            
            long elapsedTime = sw.ElapsedMilliseconds;
            _sumElapsedTime += elapsedTime;
            
            if (elapsedTime > _maxElapsedTime)
                _maxElapsedTime = elapsedTime;
    
            _countOfProcessing++;

            // other types would not be parsed at all
        }

        public ReadingPhase Phase
        {
            get
            {
                lock (_readingPhaseSync)
                {
                    return _readingPhase;
                }
            }

        }

        public event D2Type2Void<ReadingPhase,APhdParsingContext> PhaseChanged;
        private void InvokePhaseChanged(ReadingPhase readingPhase)
        {
            if (null != PhaseChanged)
            {
                if (_raiseEventsAsynchronously)
                {
                    PhdEvent phde = new PhdEvent();
                    phde.type = PhdEventType.PhaseChanged;
                    phde.readingPhase = readingPhase;
                    phde.actualContext = _actualContext;

                    _eventProcessingQueue.Enqueue(phde);
                }
                else
                {
                    try
                    {
                        PhaseChanged(readingPhase,_actualContext);
                    }
                    catch
                    {
                    }
                }
            }
        }

        
        private ByteDataCarrier _headerBuffer = null;

        private ByteDataCarrier _dataBuffer = null;

        private int _partialMatchingPreambleCount = 0;

        private int _optionalDataLength = 0;
        public int OptionalDataLength
        {
            get { return _optionalDataLength; }
        }

        /*
        void OnDataReceived_PreambleHeaderData(object sender, SerialDataReceivedEventArgs e, SerialPort port)
        {
            if (e.EventType != SerialData.Chars)
            {   
                //Console.WriteLine("Received "+e.EventType);
                return;
            }

            byte[] buffer = null;

            int bytes2read = port.BytesToRead;

            if (bytes2read <= 0)
                return;

            buffer = new byte[bytes2read];

            port.Read(buffer, 0, bytes2read);

//            lock (GeneralLock._sync)
//            {
//                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff") + " !!! " + ByteDataCarrier.HexDump(buffer, 0, bytes2read));
//            }

            _receivedBuffers.Enqueue(buffer);
        }
        */


        public event D3Type2Void<ByteDataCarrier, int,APhdParsingContext> FrameAvailable;


        public enum FrameScrambledSource
        {
            ExpectedDataLengthFromHeader,
            ExternalError
        }
        public event D4Type2Void<ByteDataCarrier, int, FrameScrambledSource, APhdParsingContext> FrameScrambled;

        private bool _raiseEventsAsynchronously = false;
        public bool RaiseEventAsynchronously
        {
            get { return _raiseEventsAsynchronously; }
            set { _raiseEventsAsynchronously = value; }
        }


        private void InvokeFrameAvailable(ByteDataCarrier message, int optionalDataLength)
        {
            if (null != FrameAvailable)
            {
                if (_raiseEventsAsynchronously)
                {
                    PhdEvent phde = new PhdEvent();
                    phde.type = PhdEventType.FrameAvailable;
                    phde.message = message;
                    phde.optionalDataLength = optionalDataLength;
                    phde.actualContext = _actualContext;

                    Stopwatch sw = Stopwatch.StartNew();
                    _eventProcessingQueue.Enqueue(phde);
                    long took = sw.ElapsedMilliseconds;
                    took *= 10;
                }
                else
                    try
                    {
                        FrameAvailable(message, optionalDataLength,_actualContext);
                    }
                    catch
                    {
                    }
            }
        }

        private void InvokeFrameScrambled(ByteDataCarrier message, int optionalDataLength,FrameScrambledSource frameScramblingSource)
        {
            if (null != FrameAvailable)
            {
                if (_raiseEventsAsynchronously)
                {
                    PhdEvent phde = new PhdEvent();
                    phde.type = PhdEventType.FrameScrambled;
                    phde.message = message;
                    phde.optionalDataLength = optionalDataLength;
                    phde.frameScramblingSource = frameScramblingSource;
                    phde.actualContext = _actualContext;

                    _eventProcessingQueue.Enqueue(phde);
                }
                else
                    try
                    {
                        FrameScrambled(message, optionalDataLength,frameScramblingSource,_actualContext);
                    }
                    catch
                    {
                    }
            }
           
        }

        public void Clear()
        {
            bool unlocked = _oneByteParsingMutex.WaitOne(200, false);

            if (unlocked)
            //lock (_oneByteParsingSync)
            {
                try
                {
                    if (null != _receivedBuffers)
                        _receivedBuffers.Clear();

                    SetPhase(ReadingPhase.WaitForPreamble);

                    /*if (_readingPhase != ReadingPhase.WaitForPreamble)
                    {
                        SetPhaseOLD(ReadingPhase.WaitForPreamble, false, true);

                        SetInitialValues();
                    }*/

                }
                catch
                {
                }
                finally
                {
                    _oneByteParsingMutex.Set();
                }
            }

            if (!unlocked)
                throw new PossibleDeadlockException();
        }


        public int BuffersCount
        {
            get
            {
                return _receivedBuffers.Count;
            }
        }

        /*
        #region OLD implementation


        private bool _parsingInProgress = false;
        private object _singleParsingSync = new object();

        private void AnalyzeHeaderAfterPreamble(int headerStart, byte[] buffer)
        {
            if (headerStart < 0)
                return;

            int headerEnd = headerStart + _headerLength;
            if (headerEnd <= buffer.Length)
            {
                // optimize character timeout creation

                SetPhaseOLD(ReadingPhase.WaitForHeader, false, true);

                Array.Copy(
                    buffer, headerStart, _headerBuffer.Buffer, 0, _headerLength);
                _headerBuffer.ActualSize = _headerBuffer.Size;

                ExamineHeader(headerStart, _headerLength, buffer);
            }
            else
            // if not whole header had been received
            {
                SetPhaseOLD(ReadingPhase.WaitForHeader, false, false);

                if (buffer.Length - headerStart > 0)
                {

                    Array.Copy(
                        buffer, headerStart,
                        _headerBuffer.Buffer, 0, buffer.Length - headerStart);

                    _headerBuffer.ActualSize = buffer.Length - headerStart;
                }
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

        private void AnalyzeIfPreamblePresent(byte[] buffer)
        {
            int partialMatchingCount = _partialMatchingPreambleCount;
            int preambleStart = FindPreamble(buffer, 0, ref partialMatchingCount);

            if (preambleStart >= 0)
            {
                if (partialMatchingCount == 0)
                {
                    AnalyzeHeaderAfterPreamble(preambleStart + _preamble.Length - _partialMatchingPreambleCount, buffer);
                }
                else
                {
                    _partialMatchingPreambleCount = partialMatchingCount;

                    if (_partialMatchingPreambleCount >= _preamble.Length)
                    {
                        _partialMatchingPreambleCount = 0;
                    }
                }
            }
            else
                _partialMatchingPreambleCount = 0;
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

            InvokeDataAvailable(message, _optionalDataLength);

            RevertToWaitForPreamble(remainingBuffer);
        }
        private void AnalyzeData(int dataStart, byte[] buffer)
        {
            int usableLength = buffer.Length - dataStart;
            if (usableLength == 0)
            {
                SetPhaseOLD(ReadingPhase.WaitForData, false, false);
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
                    SetPhaseOLD(ReadingPhase.WaitForData, false, false);


                _dataBuffer = new ByteDataCarrier(_optionalDataLength, 0);

                int dataStart = headerStart + headerByteCount;

                AnalyzeData(dataStart, buffer);
            }
            catch
            {
                RevertToWaitForPreamble(null);
            }

        }

        private void SetInitialValues()
        {
            if (null != _headerBuffer)
                _headerBuffer.ActualSize = 0;

            if (null != _dataBuffer)
            {
                _dataBuffer.Dispose();
                _dataBuffer = null;
            }

            _optionalDataLength = 0;

            _partialMatchingPreambleCount = 0;
        }

        private void RevertToWaitForPreamble(byte[] remainingBuffer)
        {
            SetPhaseOLD(ReadingPhase.WaitForPreamble, false, false);

            SetInitialValues();

            if (null != remainingBuffer &&
                remainingBuffer.Length != 0)
                OnPreambleHeaderDataProcessing(remainingBuffer);
        }

        private struct PartialPreambleInfo
        {
            public int _index;
            public int _matchingCount;

            public PartialPreambleInfo(int index, int matchingCount)
            {
                _index = index;
                _matchingCount = matchingCount;
            }
        }

        private int FindPreamble(byte[] buffer, int index, ref int partialMatchingCount)
        {
            if (index >= buffer.Length)
                return -1;


            int found = -1;
            int maxMatching = 0;
            bool foundFinal = false;

            // this means, that if any previous premable bytes had been found, the next buffer can be searched only from the begining,
            // because there can not be any in-proper bytes in the middle of the preamble sequence
            int seekingEnd = partialMatchingCount > 0 ? 1 : buffer.Length;


            for (int i = index; i < seekingEnd; i++)
            {

                int j = 0;
                for (j = 0; j < _preamble.Length - partialMatchingCount; j++)
                {
                    if (i + j >= buffer.Length)
                        break;

                    if (buffer[i + j] != _preamble[j + partialMatchingCount])
                        break;
                }

                if (j == _preamble.Length - partialMatchingCount)
                {
                    found = i;
                    foundFinal = true;
                    break;
                }
                else
                {
                    if (j > 0 && j < _preamble.Length)
                        if (j > maxMatching)
                        {
                            found = i;
                            maxMatching = j;
                        }
                }
            }

            if (!foundFinal)
            {

                if (found >= 0)
                    // partial preamble sequence found
                    partialMatchingCount += maxMatching;
                else
                {
                    // no partial preamble sequence found

                    if (partialMatchingCount > 0 && buffer[0] == _preamble[0])
                    {
                        // this verifies, if the non-coherent data is not by any accident begining of the new preamble
                        partialMatchingCount = 1;
                        found = 0;
                    }
                    else
                        // nothing suitable for preamble found
                        partialMatchingCount = 0;
                }
            }
            else
                // all preamble found
                partialMatchingCount = 0;


            return found;
        }

        private void SetPhaseOLD(ReadingPhase newPhase, bool isFromCharTimeout, bool dontStartCharTimeout)
        {
            lock (_readingPhaseSync)
            {
                if (newPhase != _readingPhase)
                {
                    ReadingPhase oldPhase = _readingPhase;

                    _readingPhase = newPhase;

                    //lock (_charTimeoutMutex)
                    if (!isFromCharTimeout && _oneByteTime > 0)
                    {
                        if (null != _charTimeout)
                        {
                            _charTimeout.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                    }

                    long charTime = 0;

                    switch (_readingPhase)
                    {
                        case ReadingPhase.WaitForPreamble:
                            break;
                        case ReadingPhase.WaitForHeader:
                            // header buffer must be created here !!!
                            if (_headerBuffer == null)
                                _headerBuffer = new ByteDataCarrier(_headerLength, 0);

                            if (_headerCharTime > 0 && !dontStartCharTimeout)
                            {
                                if (_charTimeout == null)
                                    _charTimeout =
                                        new Timer(OnCharTimeout, null, charTime, Timeout.Infinite);
                                else
                                    _charTimeout.Change(charTime, Timeout.Infinite);

                            }
                            break;
                        case ReadingPhase.WaitForData:
                            if (_oneByteTime > 0 && !dontStartCharTimeout)
                            {
                                charTime = (long)Math.Ceiling(_optionalDataLength * _oneByteTime * _charTimeAccuracy);

                                if (charTime > 0)
                                {
                                    if (null == _charTimeout)
                                        _charTimeout =
                                            new Timer(OnCharTimeout, null, charTime, Timeout.Infinite);
                                    else
                                        _charTimeout.Change(charTime, Timeout.Infinite);
                                }
                            }

                            break;
                    }

                    InvokePhaseChanged(newPhase);
                }
            }
        }

        public event DVoid2Void CharTimeoutOccured;

        private void OnCharTimeout(object state)
        {

            bool charTimeouted = false;
            if (_receivedBuffers.Count == 0 && !_parsingInProgress)
            {
                // this should be to avoid deadlock on SetPhase call
                lock (_readingPhaseSync)
                {
                    if (_readingPhase != ReadingPhase.WaitForPreamble)
                    {
                        _readingPhase = ReadingPhase.WaitForPreamble;

                        SetInitialValues();

                        InvokePhaseChanged(_readingPhase);
                    }


                    charTimeouted = true;

                }
            }

            if (charTimeouted)
            {
                // no need to separate in thread, as timer is in separate thread
                if (null != CharTimeoutOccured)
                    try
                    {
                        CharTimeoutOccured();
                    }
                    catch
                    {
                    }
            }

        }

        private Timer _charTimeout = null;

        private void OnPreambleHeaderDataProcessingOLD(byte[] buffer)
        {
            _parsingInProgress = true;

            lock (_singleParsingSync)
            {
                try
                {

                    //Console.WriteLine("\t"+ByteDataCarrier.HexDump(buffer));
                    switch (Phase)
                    {
                        case ReadingPhase.WaitForPreamble:
                            AnalyzeIfPreamblePresent(buffer);
                            break;

                        case ReadingPhase.WaitForHeader:
                            AnalyzeRemainingHeader(buffer);
                            break;
                        case ReadingPhase.WaitForData:
                            AnalyzeData(0, buffer);
                            break;
                    }
                }
                catch
                {
                }
                finally
                {
                    _parsingInProgress = false;
                }
            }
        }

        public void ReportLinkProblem(object problem)
        {
            lock (_singleParsingSync)
            {
                switch (_readingPhase)
                {
                    case ReadingPhase.WaitForPreamble:
                        if (_partialMatchingPreambleCount > 0)
                            _partialMatchingPreambleCount = 0;
                        break;
                    case ReadingPhase.WaitForHeader:
                    case ReadingPhase.WaitForData:
                        SetPhaseOLD(ReadingPhase.WaitForPreamble, false, true);

                        SetInitialValues();

                        break;
                }
            }

            //Console.WriteLine("\r\nProblem reported : "+problem !=null ? problem.ToString() : "UNKNOWN");
        }

        #endregion*/
    }
}
