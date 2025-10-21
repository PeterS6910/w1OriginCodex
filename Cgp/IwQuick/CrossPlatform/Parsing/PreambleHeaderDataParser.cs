using System;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;



namespace Contal.IwQuick.Parsing
{
    public interface IPhdEventHandler
    {
        void FrameAvailable(
            ByteDataCarrier message, 
            int optionalDataLength, 
            int timeStamp,
            APhdParsingContext context);

        void FrameScrambled(
            ByteDataCarrier message, 
            int optionalDataLength, 
            PreambleHeaderDataParser.FrameScrambledSource scrambledSource, 
            APhdParsingContext context);

        void PhaseChanged(
            PreambleHeaderDataParser.ReadingPhase readingPhase, 
            APhdParsingContext context);
    }

    /// <summary>
    /// 
    /// </summary>
    public class PreambleHeaderDataParser : ADisposable
    {
        private readonly byte[] _preamble;
        private readonly int _headerLength;
        private readonly DExpectedDataLengthFromHeader _expectedDataLengthFromHeader;
        private readonly int _odlSpecificationOffset = -1;
        private readonly int _odlSpecificationLength = -1;

        private int _preambleStartTimeStamp;

        private float _oneByteTime; //in ms
        // ReSharper disable once NotAccessedField.Local
        private long _headerCharTime;

        private const float _charTimeAccuracy = 1.1f;

        /// <summary>
        /// 
        /// </summary>
        public float CharTimeAccuracy
        {
            get
            {
                return _charTimeAccuracy;
            }
        }

        private readonly ThreadPoolQueue<AInputRequest, PreambleHeaderDataParser> _dataToBeProcessedQueue;

        private abstract class AInputRequest : IProcessingQueueRequest<PreambleHeaderDataParser>
        {
            public void Execute(PreambleHeaderDataParser preambleHeaderDataParser)
            {
#if DEBUG
                var sw = Stopwatch.StartNew();
#endif

                ExecuteInternal(preambleHeaderDataParser);

#if DEBUG
                var elapsedTime = sw.ElapsedMilliseconds;
                preambleHeaderDataParser._sumElapsedTime += elapsedTime;

                if (elapsedTime > preambleHeaderDataParser._maxElapsedTime)
                    preambleHeaderDataParser._maxElapsedTime = elapsedTime;

                preambleHeaderDataParser._countOfProcessing++;
#endif
            }

            public void OnError(
                PreambleHeaderDataParser param,
                Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            protected abstract void ExecuteInternal(PreambleHeaderDataParser preambleHeaderDataParser);
        }

        private abstract class AInputRequest<TInputData> : AInputRequest
        {
            private readonly TInputData _inputData;
            protected readonly int TimeStamp;

            protected AInputRequest(
                TInputData inputData,
                int timeStamp)
            {
                _inputData = inputData;
                TimeStamp = timeStamp;
            }

            protected override void ExecuteInternal(PreambleHeaderDataParser preambleHeaderDataParser)
            {
                ExecuteInternal(
                    preambleHeaderDataParser,
                    _inputData);
            }

            protected abstract void ExecuteInternal(
                PreambleHeaderDataParser preambleHeaderDataParser,
                TInputData inputData);
        }

        private class ByteArrayInputRequest : AInputRequest<byte[]>
        {
            public ByteArrayInputRequest(
                byte[] inputData,
                int timeStamp)
                : base(
                    inputData, 
                    timeStamp)
            {
            }

            protected override void ExecuteInternal(
                PreambleHeaderDataParser preambleHeaderDataParser,
                byte[] inputData)
            {
                foreach (var c in inputData)
                {
                    if (preambleHeaderDataParser._dataToBeProcessedQueue.IsBlocked)
                        break;

                    preambleHeaderDataParser.ParseOneByte(
                        c,
                        TimeStamp);
                }
            }
        }

        private class ByteDataCarrierInputRequest : AInputRequest<ByteDataCarrier>
        {
            public ByteDataCarrierInputRequest(
                ByteDataCarrier inputData,
                int timeStamp)
                : base(
                    inputData,
                    timeStamp)
            {
            }

            protected override void ExecuteInternal(
                PreambleHeaderDataParser preambleHeaderDataParser,
                ByteDataCarrier inputData)
            {
                var upperBound = inputData.ActualSize;

                for (var i = inputData.Offset; i < upperBound; i++)
                {
                    if (preambleHeaderDataParser._dataToBeProcessedQueue.IsBlocked)
                        break;

                    preambleHeaderDataParser.ParseOneByte(
                        inputData.Buffer[i],
                        TimeStamp);
                }
            }
        }

        private class PhdParsingContextInputRequest : AInputRequest<APhdParsingContext>
        {
            public PhdParsingContextInputRequest(APhdParsingContext inputData)
                : base(
                    inputData,
                    0)
            {
            }

            protected override void ExecuteInternal(
                PreambleHeaderDataParser preambleHeaderDataParser,
                APhdParsingContext inputData)
            {
                preambleHeaderDataParser.ParseContext(inputData);
            }
        }

        private class ErrorInputRequest : AInputRequest
        {
            protected override void ExecuteInternal(PreambleHeaderDataParser preambleHeaderDataParser)
            {
                preambleHeaderDataParser.ParseError();
            }
        }

        private readonly APhdParsingContext[] _contextDisposalLevelingArray;

        private long _countOfProcessing;
        private long _sumElapsedTime;
        private long _maxElapsedTime;

        private readonly object _readingPhaseSync = new object();
        private volatile ReadingPhase _readingPhase = ReadingPhase.WaitForPreamble;

        /// <summary>
        /// 
        /// </summary>
        public enum ReadingPhase
        {
            /// <summary>
            /// 
            /// </summary>
            WaitForPreamble,
            //WaitForUnclearHeader,
            /// <summary>
            /// 
            /// </summary>
            WaitForHeader,
            /// <summary>
            /// 
            /// </summary>
            WaitForData,

        }

        //private object _oneByteParsingSync = new object();
        //private AutoResetEvent _oneByteParsingMutex = new AutoResetEvent(true);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private abstract class APhdEvent<T>
            : APoolable<T>
            , IProcessingQueueRequest
            where T : APoolable<T>
        {
            protected PreambleHeaderDataParser Parent;

            public void Execute(bool asynchronously)
            {
                if (asynchronously)
                    Parent._eventProcessingQueue.Enqueue(this);
                else
                    Execute();
            }

            protected abstract void ExecuteInternal();

            protected APhdParsingContext Context;

            protected APhdEvent(IObjectPool objectPool)
                : base(objectPool)
            {
            }

            protected override bool FinalizeBeforeReturn()
            {
                Parent = null;

                Context = null;
                return true;
            }

            public void Execute()
            {
                try
                {
                    ExecuteInternal();
                }
                catch
                {
                }
                finally
                {
                    this.TryReturn();
                }
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private class EventFrameAvailable : APhdEvent<EventFrameAvailable>
        {
            public EventFrameAvailable(IObjectPool objectPool)
                : base(objectPool)
            {
            }

            private ByteDataCarrier _message;
            private int _optionalDataLength;
            private int _timeStamp;

            public static EventFrameAvailable Get(
                PreambleHeaderDataParser parser,
                ByteDataCarrier message,
                int optionalDataLength,
                int timeStamp,
                APhdParsingContext context)
            {
                var i = Get();
                i.Parent = parser;
                i._message = message;
                i._optionalDataLength = optionalDataLength;
                i._timeStamp = timeStamp;
                i.Context = context;

                return i;
            }

            protected override void ExecuteInternal()
            {
                if (Parent._eventHandlerGroup.IsNotEmpty)
                    Parent._eventHandlerGroup.ForEach(
                        handler =>
                            handler.FrameAvailable(
                                _message, 
                                _optionalDataLength, 
                                _timeStamp,
                                Context));
            }
        }

        private class EventFrameScrambled : APhdEvent<EventFrameScrambled>
        {
            public EventFrameScrambled(IObjectPool objectPool)
                : base(objectPool)
            {
            }


            private ByteDataCarrier _message;
            private int _optionalDataLength;
            private FrameScrambledSource _frameScrambledSource;

            public static EventFrameScrambled Get(
                PreambleHeaderDataParser parser,
                ByteDataCarrier message,
                int optionalDataLength,
                FrameScrambledSource frameScrambledSource,
                APhdParsingContext context)
            {
                var i = Get();
                i.Parent = parser;
                i._message = message;
                i._optionalDataLength = optionalDataLength;
                i.Context = context;
                i._frameScrambledSource = frameScrambledSource;

                return i;
            }

            protected override void ExecuteInternal()
            {
                if (Parent._eventHandlerGroup.IsNotEmpty)
                    Parent._eventHandlerGroup.ForEach(
                        handler =>
                            handler.FrameScrambled(_message, _optionalDataLength, _frameScrambledSource, Context));
            }
        }

        private class EventPhaseChanged : APhdEvent<EventPhaseChanged>
        {
            public EventPhaseChanged(IObjectPool objectPool)
                : base(objectPool)
            {
            }


            private ReadingPhase _readingPhase;

            public static EventPhaseChanged Get(
                PreambleHeaderDataParser parser,
                ReadingPhase readingPhase,
                APhdParsingContext context)
            {
                var i = Get();
                i.Parent = parser;
                i.Context = context;
                i._readingPhase = readingPhase;

                return i;
            }

            protected override void ExecuteInternal()
            {
                if (Parent._eventHandlerGroup.IsNotEmpty)
                    Parent._eventHandlerGroup.ForEach(
                        handler =>
                            handler.PhaseChanged(_readingPhase, Context));
            }
        }

        private readonly ThreadPoolQueue<IProcessingQueueRequest> _eventProcessingQueue =
            new ThreadPoolQueue<IProcessingQueueRequest>(ThreadPoolGetter.Get());

        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="expectedDataLengthFromHeader"></param>
        /// <param name="bitsPerSecond"></param>
        /// <param name="contextDisposalLevelingDepth"></param>
        public PreambleHeaderDataParser(
            [NotNull] byte[] preamble,
            int headerLength,
            [NotNull] DExpectedDataLengthFromHeader expectedDataLengthFromHeader,
            int bitsPerSecond,
            int contextDisposalLevelingDepth)
        {
            Validator.CheckNullAndEmpty(preamble, "preamble");
            Validator.CheckNegativeOrZeroInt(headerLength);
            Validator.CheckForNull(expectedDataLengthFromHeader, "expectedDataLengthFromHeader");

            _preamble = preamble;
            _headerLength = headerLength;
            _expectedDataLengthFromHeader = expectedDataLengthFromHeader;

            if (bitsPerSecond > 0)
                // this should be always called after the header length had been specified
                UpdateBitsPerSecond(bitsPerSecond);

            if (contextDisposalLevelingDepth > 0)
                _contextDisposalLevelingArray = new APhdParsingContext[contextDisposalLevelingDepth];

            _dataToBeProcessedQueue = new ThreadPoolQueue<AInputRequest, PreambleHeaderDataParser>(
                ThreadPoolGetter.Get(),
                this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="odlSpecificationOffset"></param>
        /// <param name="odlSpecificationLength"></param>
        /// <param name="bitsPerSecond"></param>
        /// <param name="contextDisposalLevelingDepth"></param>
        public PreambleHeaderDataParser(
            [NotNull] byte[] preamble,
            int headerLength,
            int odlSpecificationOffset,
            int odlSpecificationLength,
            int bitsPerSecond,
            int contextDisposalLevelingDepth)
        {
            Validator.CheckNullAndEmpty(preamble, "preamble");
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

            _dataToBeProcessedQueue =
                new ThreadPoolQueue<AInputRequest, PreambleHeaderDataParser>(
                    ThreadPoolGetter.Get(),
                    this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="odlSpecificationOffset"></param>
        /// <param name="odlSpecificationLength"></param>
        public PreambleHeaderDataParser(
            [NotNull] byte[] preamble,
            int headerLength,
            int odlSpecificationOffset,
            int odlSpecificationLength)
            : this(
                preamble,
                headerLength,
                odlSpecificationOffset,
                odlSpecificationLength,
                0,
                0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="odlSpecificationOffset"></param>
        /// <param name="odlSpecificationLength"></param>
        /// <param name="contextDisposalLevelingDepth"></param>
        public PreambleHeaderDataParser(
            [NotNull] byte[] preamble,
            int headerLength,
            int odlSpecificationOffset,
            int odlSpecificationLength,
            int contextDisposalLevelingDepth)
            : this(
                preamble,
                headerLength,
                odlSpecificationOffset,
                odlSpecificationLength,
                0,
                contextDisposalLevelingDepth)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="expectedDataLengthFromHeader"></param>
        public PreambleHeaderDataParser(
            [NotNull] byte[] preamble,
            int headerLength,
            [NotNull] DExpectedDataLengthFromHeader expectedDataLengthFromHeader)
            : this(
                preamble,
                headerLength,
                expectedDataLengthFromHeader,
                0,
                0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawData"></param>
        public delegate int DExpectedDataLengthFromHeader([NotNull] ByteDataCarrier rawData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="expectedDataLengthFromHeader"></param>
        /// <param name="contextDisposalLevelingDepth"></param>
        public PreambleHeaderDataParser(
            [NotNull] byte[] preamble,
            int headerLength,
            [NotNull] DExpectedDataLengthFromHeader expectedDataLengthFromHeader,
            int contextDisposalLevelingDepth)
            : this(
                preamble,
                headerLength,
                expectedDataLengthFromHeader,
                0,
                contextDisposalLevelingDepth)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void ProcessData(
            byte[] data,
            int timeStamp)
        {
            if (null != data)
                _dataToBeProcessedQueue.Enqueue(new ByteArrayInputRequest(
                    data,
                    timeStamp));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeStamp"></param>
        public void ProcessData(
            ByteDataCarrier data,
            int timeStamp)
        {
            if (null != data)
                _dataToBeProcessedQueue.Enqueue(new ByteDataCarrierInputRequest(
                    data,
                    timeStamp));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialError"></param>
        public void ProcessData(SerialError serialError)
        {
            _dataToBeProcessedQueue.Enqueue(new ErrorInputRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parsingContext"></param>
        public void ProcessData(APhdParsingContext parsingContext)
        {
            _dataToBeProcessedQueue.Enqueue(new PhdParsingContextInputRequest(parsingContext));
        }

        private const int MAX_OPTIONAL_BUFFER_LENGTH = ushort.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPhase"></param>
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

#if DEBUG
        private readonly byte[] _previousBytes = new byte[64];
        private int _previousBytesCount;
        private ReadingPhase _previousPhase;
#endif

        private void ParseOneByte(
            byte oneByte,
            int timeStamp)
        {
#if DEBUG
            var currentByte = oneByte;
            var currentPhase = _readingPhase;
#endif

            try
            {
                ByteDataCarrier finalMessage;
                switch (_readingPhase)
                {
                    case ReadingPhase.WaitForPreamble:
                        if (_preamble[_partialMatchingPreambleCount] == oneByte)
                        {
                            if (++_partialMatchingPreambleCount == 1)
                                _preambleStartTimeStamp = timeStamp;
                        }
                        else
                        {
                            if (_preamble.Length > 1 &&
                                _preamble[0] == oneByte)
                            {
                                _partialMatchingPreambleCount = 1;
                                _preambleStartTimeStamp = timeStamp;
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
                                            _optionalDataLength = BitConverter.ToInt16(_headerBuffer.Buffer,
                                                _odlSpecificationOffset);
                                            break;
                                        case 4:
                                            _optionalDataLength = BitConverter.ToInt32(_headerBuffer.Buffer,
                                                _odlSpecificationOffset);
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
                                InvokeFrameAvailable(
                                    finalMessage, 
                                    _optionalDataLength,
                                    _preambleStartTimeStamp);

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
                                    InvokeFrameScrambled(finalMessage, _optionalDataLength,
                                        FrameScrambledSource.ExpectedDataLengthFromHeader);

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

                            InvokeFrameAvailable(
                                finalMessage, 
                                _optionalDataLength,
                                _preambleStartTimeStamp);

                            SetPhase(ReadingPhase.WaitForPreamble);
                        }
                        break;
                }
            }
            catch
            {
            }

#if DEBUG
            DebugHelper.Keep(_previousPhase);

            if (_previousBytesCount < _previousBytes.Length)
            {
                _previousBytes[_previousBytesCount] = currentByte;
                _previousBytesCount++;
            }
            else
            {
                for (var i = 0; i < _previousBytes.Length - 1; i++)
                    _previousBytes[i] = _previousBytes[i + 1];
                _previousBytes[_previousBytes.Length - 1] = currentByte;
            }
            _previousPhase = currentPhase;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public long MaxProcessingTime
        {
            get
            {
                return _maxElapsedTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float AverageProcessingTime
        {
            get
            {
                return _sumElapsedTime / (float)_countOfProcessing;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetStatistics()
        {
            _countOfProcessing = 0;
            _sumElapsedTime = 0;
            _maxElapsedTime = 0;
        }

        private APhdParsingContext _currentContext;

        private void ParseContext(APhdParsingContext context)
        {
            try
            {
                // DISPOSAL TACTICS
                if (_contextDisposalLevelingArray != null &&
                    _currentContext != null)
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

                    for (var i = 0; i < _contextDisposalLevelingArray.Length - 1; i++)
                    {
                        _contextDisposalLevelingArray[i] = _contextDisposalLevelingArray[i + 1];
                    }

                    _contextDisposalLevelingArray[_contextDisposalLevelingArray.Length - 1] = _currentContext;

                }
                _currentContext = context;
            }
            catch
            {
            }
        }

        private void ParseError()
        {
            if (_eventHandlerGroup.IsNotEmpty)
            {
                ByteDataCarrier finalMessage;
                switch (_readingPhase)
                {
                    case ReadingPhase.WaitForPreamble:

                        InvokeFrameScrambled(null, -1, FrameScrambledSource.ExternalError);
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


        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>

        private void InvokePhaseChanged(ReadingPhase readingPhase)
        {
            if (EventHandlerGroup.IsNotEmpty)
            {
                var eventPhaseChanged = EventPhaseChanged.Get(this, readingPhase, _currentContext);
                eventPhaseChanged.Execute(_raiseEventsAsynchronously);
            }
        }


        private ByteDataCarrier _headerBuffer;

        private ByteDataCarrier _dataBuffer;

        private int _partialMatchingPreambleCount;

        private int _optionalDataLength;
        /// <summary>
        /// 
        /// </summary>
        public int OptionalDataLength
        {
            get { return _optionalDataLength; }
        }

        private readonly TinyEventHandlerGroup<IPhdEventHandler> _eventHandlerGroup =
            new TinyEventHandlerGroup<IPhdEventHandler>();

        public IEventHandlerGroup<IPhdEventHandler> EventHandlerGroup
        {
            get { return _eventHandlerGroup; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FrameScrambledSource
        {
            /// <summary>
            /// 
            /// </summary>
            ExpectedDataLengthFromHeader,
            /// <summary>
            /// 
            /// </summary>
            ExternalError
        }


        private bool _raiseEventsAsynchronously;
        /// <summary>
        /// 
        /// </summary>
        public bool RaiseEventAsynchronously
        {
            get { return _raiseEventsAsynchronously; }
            set { _raiseEventsAsynchronously = value; }
        }

        private void InvokeFrameAvailable(
            ByteDataCarrier message, 
            int optionalDataLength,
            int timeStamp)
        {
            if (_eventHandlerGroup.IsNotEmpty)
            {
                var eventFrameAvailable = EventFrameAvailable.Get(
                    this, 
                    message, 
                    optionalDataLength, 
                    timeStamp,
                    _currentContext);

                eventFrameAvailable.Execute(_raiseEventsAsynchronously);
            }
        }

        private void InvokeFrameScrambled(ByteDataCarrier message, int optionalDataLength, FrameScrambledSource frameScramblingSource)
        {
            if (_eventHandlerGroup.IsNotEmpty)
            {
                var eventFrameScrambled = EventFrameScrambled.Get(
                    this,
                    message,
                    optionalDataLength,
                    frameScramblingSource,
                    _currentContext);

                eventFrameScrambled.Execute(_raiseEventsAsynchronously);

            }

        }

        public void Start()
        {
            _dataToBeProcessedQueue.Unblock();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            try
            {
                if (null != _dataToBeProcessedQueue)
                {
                    _dataToBeProcessedQueue.ClearAndBlock();
                    _dataToBeProcessedQueue.WaitUntilIdle();
                }

                SetPhase(ReadingPhase.WaitForPreamble);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BuffersCount
        {
            get
            {
                return _dataToBeProcessedQueue.Count;
            }
        }


        protected override void InternalDispose(bool isExplicitDispose)
        {
            try
            {
                _dataToBeProcessedQueue.Dispose();//explicity
                SetPhase(ReadingPhase.WaitForPreamble);
            }
            catch
            {
            }
        }
    }
}
