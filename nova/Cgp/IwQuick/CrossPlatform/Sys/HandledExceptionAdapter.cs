using System;
using System.IO;
using System.Threading;
using Contal.IwQuick.Data;
// ReSharper disable once RedundantUsingDirective
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// static class with features to log the handled exception in several reporting formats
    /// </summary>
    public static class HandledExceptionAdapter
    {
        private static string _outputFilePath = null;


        static HandledExceptionAdapter()
        {
#if DEBUG
            _useOutputFile = true;
#endif
        }

        /// <summary>
        /// path to the output text file, if used ; 
        /// if null or empty specified, the path will be reverted to "exception.log" ; 
        /// the system will immediately try the writing capabilities into this file
        /// </summary>
        /// <exception cref="Exception">Can produce any exception regarding I/O to the new file specified</exception>
        /// <exception cref="ArgumentNullException">raised if value is null or empty string</exception>
        public static string OutputFilePath
        {
            get { return _outputFilePath; }
            set
            {
                Validator.CheckNullString(value);

                if (value != _outputFilePath)
                {
                    FileStream f = null;

                    // just checking section

                    #region checking section

                    try
                    {
                        f = File.Open(value, FileMode.Append, FileAccess.Write, FileShare.Read);
                    }
                    finally
                    {
                        if (f != null)
                            try
                            {
                                f.Close();
                            }
                            catch
                            {
                            }
                    }


                    #endregion

                    CloseOutputFile();

                    _outputFilePath = value;

                }

            }
        }

        private static bool _automaticFileClosing = false;

        /// <summary>
        /// if true, the log file is closed every time is flushed
        /// </summary>
        public static bool AutomaticFileClosing
        {
            get { return _automaticFileClosing; }
            set
            {
                if (value != _automaticFileClosing)
                {
                    _automaticFileClosing = value;

                    if (value && !_ofFlushTimeoutInProgress)
                    {
                        try
                        {
                            CloseOutputFile();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="dateTime"></param>
        /// <param name="threadId"></param>
        public delegate void DExceptionOccured(Exception error, DateTime dateTime, int threadId);

        /// <summary>
        /// 
        /// </summary>
        public static event DExceptionOccured ExceptionOccured;

        private static System.Threading.Timer _ofFlushTimeoutTimer = null;
        private static bool _ofFlushTimeoutInProgress = false;

        private const int DEFAULT_OUTFILE_FLUSH_TIMEOUT = 5000;

        private static int _ofFlushTimeout = DEFAULT_OUTFILE_FLUSH_TIMEOUT;

        /// <summary>
        /// minimal timeout between last message written to the output file and
        /// actual flush of the output file's write buffer
        /// </summary>
        public static int OutputFileFlushTimeout
        {
            get { return _ofFlushTimeout; }
            set { _ofFlushTimeout = value; }
        }


        private const int MAX_OUTPUT_FILE_FAILURES_COUNT = 5;

        // this counts number of retries to use to output file
        // and when maximum is reached, output file is not used any more
        private static volatile int _outputFileRetryCount = 0;


        private static volatile bool _useOutputFile = false;

        /// <summary>
        /// defines, whether Examine method call will produce output into text file
        /// </summary>
        public static bool UseOutputFile
        {
            get { return _useOutputFile; }
            set { _useOutputFile = value; }
        }

        private static volatile PatchedFileStream _outputFile = null;
        private static readonly object _ofInstanceLock = new object();

        private static volatile Log _useLogClass = null;

        /// <summary>
        /// 
        /// </summary>
        public static Log UseLogClass
        {
            get { return _useLogClass; }
            set { _useLogClass = value; }
        }



        /// <summary>
        /// ensures that output file gets created within defined number of retries
        /// </summary>
        private static void EnsureOutputFile()
        {
            if (!_useOutputFile)
                return;

            if (_outputFile == null &&
                _outputFileRetryCount < MAX_OUTPUT_FILE_FAILURES_COUNT)
                lock (_ofInstanceLock)
                {
                    if (_outputFile == null)
                    {
                        try
                        {
                            _outputFile = PatchedFileStream.Open(_outputFilePath, FileMode.Append, FileAccess.Write,
                                FileShare.ReadWrite);
                        }
                        catch
                        {
                            _outputFile = null;
                            _outputFileRetryCount++;
                        }
                    }
                }
        }

        private class ExceptionQueueGetter : AGetter<ProcessingQueue<ExceptionInfo>>
        {
            public ExceptionQueueGetter()
                : base(null)
            {

            }

            private readonly object _syncPq = new object();

            protected override void GetOrModifyValue(ref ProcessingQueue<ExceptionInfo> valueToBePassedAndOrModified)
            {
                if (valueToBePassedAndOrModified == null)
                    lock (_syncPq)
                    {
                        if (valueToBePassedAndOrModified == null)
                        {
                            var pq = new ProcessingQueue<ExceptionInfo>(null,
                                PQPickQueueMode.RoundRobin,
                                false,
                                System.Threading.ThreadPriority.Normal,
                                null);


                            pq.ItemProcessing +=
                                // HandledExceptionAdapter.
                                    OnItemProcessing;

                            valueToBePassedAndOrModified = pq;

                        }
                    }
            }

            protected override void DisposeValue(ref ProcessingQueue<ExceptionInfo> valueToBeDisposed, bool isExplicitDispose)
            {
                if (null != valueToBeDisposed)
                    valueToBeDisposed.Dispose(isExplicitDispose);
            }
        }


        private static readonly ExceptionQueueGetter ExceptionQueue
            = new ExceptionQueueGetter();

        /// <summary>
        /// 
        /// </summary>
        public const int MinimalExceptionFilterTime = 100;
        /// <summary>
        /// 
        /// </summary>
        public const int DefaultExceptionFilterTime = 5000;

        /// <summary>
        /// 
        /// </summary>
        private static volatile int _exceptionFilterTime = DefaultExceptionFilterTime;

        /// <summary>
        /// 
        /// </summary>
        public static int ExceptionFilterTime
        {
            get { return _exceptionFilterTime; }
            set
            {
                if (value < MinimalExceptionFilterTime)
                    _exceptionFilterTime = -1;
                else
                {
                    _exceptionFilterTime = value;
                }
            }
        }

        private class ExceptionInfo : APoolable<ExceptionInfo>
        {
            internal Exception Error;
            internal int ThreadId;
            internal DateTime Timestamp;
            internal int InProcessing;

            internal ExceptionInfo(IObjectPool objectPool)
                : base(objectPool)
            {
            }

            public bool InDictionary;

            protected override void BeforeGet()
            {
#if !COMPACT_FRAMEWORK
                Timestamp = DateTime.Now;
#else
                Timestamp = SystemTime.GetLocalTime();
#endif

                InDictionary = true;
            }

            protected override bool FinalizeBeforeReturn()
            {
                Error = null;
                return true;
            }
        }

        private static volatile RecentExceptionsGetter RecentExceptions = new RecentExceptionsGetter(null);


        private class RecentExceptionsGetter : AGetter<TimeoutDictionary<string, ExceptionInfo>>
        {
            public RecentExceptionsGetter(TimeoutDictionary<string, ExceptionInfo> defaultValue)
                : base(defaultValue)
            {
            }

            private readonly object _recentExceptionsLock = new object();

            protected override void GetOrModifyValue(ref TimeoutDictionary<string, ExceptionInfo> valueToBePassedAndOrModified)
            {
                if (null == valueToBePassedAndOrModified)
                    lock (_recentExceptionsLock)
                        if (null == valueToBePassedAndOrModified)
                        {

                            valueToBePassedAndOrModified = new TimeoutDictionary<string, ExceptionInfo>(_exceptionFilterTime);
                            valueToBePassedAndOrModified.ItemTimedOut += OnExceptionInfoTimedOut;
                        }
            }

            private void OnExceptionInfoTimedOut(string key, ExceptionInfo value, int timeout)
            {
                if (value.InProcessing == 1)
                    value.InDictionary = false;
                else
                {
                    value.TryReturn();
                }
            }


            protected override void DisposeValue(ref TimeoutDictionary<string, ExceptionInfo> valueToBeDisposed, bool isExplicitDispose)
            {
                RecentExceptions.Dispose(isExplicitDispose);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public static void Examine(Exception exception)
        {
            try
            {
                if (ReferenceEquals(exception, null) ||
                    (!_useOutputFile && _useLogClass == null && ExceptionOccured == null))
                    // no need for any more examination
                    return; // do not raise any other exception

                int threadId = 0;
                try
                {
                    threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                }
                catch
                {

                }

                ExceptionInfo exceptionInfo = null;

                if (_exceptionFilterTime > 0)
                {
                    string exceptionFakeId =
                        string.Concat(
                        //threadId, - maybe not needed, as stacktrace can uniquely identify source of the problem
                            exception.GetType(),
                            exception.Message,
                            exception.StackTrace
                            );

                    // intentionally this approach, instead of evaluation bool from SetValue method
                    // because SetValue would prolong the timeout of the item all the time
                    // 
                    // with this approach 
                    if (RecentExceptions.Value.ContainsKey(exceptionFakeId))
                        // still within filter time
                        return;

                    RecentExceptions.Value.SetValue(
                        exceptionFakeId,
                        (string key, out ExceptionInfo value, out int timeout) =>
                        {
                            value = ExceptionInfo.Get();
                            value.Error = exception;
                            value.ThreadId = threadId;

                            timeout = _exceptionFilterTime;
                            exceptionInfo = value;
                        },
                        (string key, ref ExceptionInfo value, ref int timeout) =>
                        {
                            value.ThreadId = threadId;

                            exceptionInfo = value;
                        });

                }
                else
                {
                    exceptionInfo = ExceptionInfo.Get(info =>
                    {
                        info.InDictionary = false;
                        info.Error = exception;
                    });

                }

                ExceptionQueue.Value.Enqueue(exceptionInfo);
            }
            catch (OutOfMemoryException)
            {
                try
                {
                    ExceptionQueue.Value.Clear();
                }
                catch
                {
                }
            }
            catch
            {
                // mute any possible exception
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionInfo"></param>
        private static void OnItemProcessing(ExceptionInfo exceptionInfo)
        {
            try
            {
                if (null == exceptionInfo ||
                    exceptionInfo.Error == null)
                    return;

                Interlocked.Exchange(ref exceptionInfo.InProcessing, 1);

                if (_useLogClass != null)
                {
                    try
                    {
                        _useLogClass.Error("Exception thrown\r\n   " + exceptionInfo.Error);
                    }
                    catch
                    {
                    }
                }

                if (null != ExceptionOccured)
                    try
                    {
                        ExceptionOccured(
                            exceptionInfo.Error,
                            exceptionInfo.Timestamp,
                            exceptionInfo.ThreadId
                            );
                    }
                    catch
                    {

                    }

                if (_useOutputFile)
                {
                    try
                    {
                        EnsureOutputFile();

                        if (_outputFile != null)
                        {

                            string message = null;

                            try
                            {
                                message =
                                    string.Concat(
                                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                                        exceptionInfo.Timestamp.ToString("yyMMdd HH:mm:ss"),
                                        Environment.NewLine,
                                        exceptionInfo.Error,
                                        Environment.NewLine,
                                        Environment.NewLine);
                            }
                            catch
                            {

                            }

                            var bdc = new ByteDataCarrier(message ?? String.Empty);

                            lock (_ofInstanceLock)
                                try
                                {
                                    _outputFile.Write(bdc.Buffer, 0, bdc.Length);
                                }
                                catch
                                {
                                }

                            RefreshOutputFileFlushTimeout();
                        }
                    }
                    catch
                    {
                    }
                }


            }
            catch
            {
                // mute any possible exception
            }
            finally
            {
                if (exceptionInfo != null)
                {
                    Interlocked.Exchange(ref exceptionInfo.InProcessing, 0);
                    if (!exceptionInfo.InDictionary)
                        exceptionInfo.TryReturn();
                }

            }


        }

        /// <summary>
        /// 
        /// </summary>
        private static void RefreshOutputFileFlushTimeout()
        {
            try
            {
                if (_ofFlushTimeoutTimer != null)
                {
                    _ofFlushTimeoutInProgress = true;
                    _ofFlushTimeoutTimer.Change(_ofFlushTimeout, -1);
                }
                else
                {
                    _ofFlushTimeoutInProgress = true;
                    var t = new System.Threading.Timer(OnFlushFileTimeout, null, _ofFlushTimeout, -1);
                    _ofFlushTimeoutTimer = t;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private static void OnFlushFileTimeout(object state)
        {
            _ofFlushTimeoutInProgress = false;

            // no need for locking against RefreshOutputFileFlushTimeout calls
            if (null != _outputFile)
                lock (_ofInstanceLock)
                {
                    if (null != _outputFile)
                    {
                        try
                        {
                            _outputFile.Flush();
                        }
                        catch
                        {
                        }

                        if (_automaticFileClosing)
                            try
                            {
                                CloseOutputFile();
                            }
                            catch
                            {
                            }
                    }
                }
        }

        /// <summary>
        /// closes the actually opened text file
        /// </summary>
        private static void CloseOutputFile()
        {
            if (null != _outputFile)
                lock (_ofInstanceLock)
                {
                    if (null != _outputFile)
                    {
                        _outputFile.Close(); // let the exception throw out to higher level

                        _outputFileRetryCount = 0;
                        _outputFile = null;
                    }
                }
        }

        /// <summary>
        /// enables raising or retrieving the processing queue's priority
        /// </summary>
        public static System.Threading.ThreadPriority ProcessingThreadPriority
        {
            get
            {
                return ExceptionQueue.Value.ProcessingThreadPriority;
            }
            set
            {
                ExceptionQueue.Value.ProcessingThreadPriority = value;
            }
        }


    }
}
