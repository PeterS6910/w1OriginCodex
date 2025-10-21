using System;
using System.IO;

#if !COMPACT_FRAMEWORK
using Contal.IwQuick.Data;

namespace Contal.IwQuick.Sys
#else
using Contal.IwQuickCF.Data;
using Contal.IwQuickCF.Sys.Microsoft;

namespace Contal.IwQuickCF.Sys
#endif
{
    /// <summary>
    /// static class with features to log the handled exception in several reporting formats
    /// </summary>
    public static class HandledExceptionAdapter
    {
        private static string _outputFilePath = null;

        /// <summary>
        /// path to the output text file, if used ; 
        /// if null or empty specified, the path will be reverted to "exception.log" ; 
        /// the system will immediately try the writing capabilities into this file
        /// </summary>
        /// <exception cref="Exception">Can produce any exception regarding I/O to the new file specified</exception>
        /// <exception cref="ArgumentNullException">raised if value is null or empty string</exception>
        public static string OutputFilePath
        {
            get
            {
                return _outputFilePath;
            }
            set
            {
                Validator.CheckNullString(value);

                if (value != _outputFilePath)
                {
                    Exception e = null;

                    FileStream f = null;

                    // just checking section
                    #region checking section
                    try
                    {
                        f = File.Open(value, FileMode.Append, FileAccess.Write, FileShare.Read);
                    }
                    catch (Exception anyError)
                    {
                        e = anyError;
                    }
                    finally
                    {
                        if (f != null)
                            try { f.Close(); }
                            catch { }
                    }
                    

                    if (e != null)
                        throw e;
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
            set {
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
        /// internal carrier of the exceptional state and timestamp of its occurence
        /// </summary>
        private class ExceptionCarrier 
        {
            public readonly Exception _exception = null;
            public readonly DateTime _timestamp;

            public readonly int _threadId;


            public ExceptionCarrier(Exception exception) 
            {
                try
                {
#if !COMPACT_FRAMEWORK
                    _timestamp = DateTime.Now;
#else
                    _timestamp = SystemTime.GetLocalTime();
#endif

                    _exception = exception;

                    try
                    {
                        _threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    }
                    catch
                    {
                        
                    }
                }
                catch
                {
                }
            }
        }

        private static volatile ProcessingQueue<ExceptionCarrier> _pq = null;
        private static readonly object _instantiatePqSync = new object();

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
                            _outputFile = PatchedFileStream.Open(_outputFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        }
                        catch
                        {
                            _outputFile = null;
                            _outputFileRetryCount++;
                        }
                    }
                }
        }

        /// <summary>
        /// instantiation of the singleton instance
        /// </summary>
        private static void EnsureProcessingQueue()
        {
            if (_pq == null)
                lock (_instantiatePqSync)
                {
                    if (_pq == null)
                    {
                        var pq = new ProcessingQueue<ExceptionCarrier>(
                            PQMode.Synchronous,
                            null,
                            PQPickQueueMode.RoundRobin,
                            false,
                            System.Threading.ThreadPriority.Normal,
                            null); 
                        pq.ItemProcessing += OnItemProcessing;

                        _pq = pq;

                    }
                }
        }

        /// <summary>
        /// 
        /// </summary>
        public const int MINIMAL_EXCEPTION_FILTER_TIME = 100;
        /// <summary>
        /// 
        /// </summary>
        public const int DEFAULT_EXCEPTION_FILTER_TIME = 5000;

        private static volatile int _exceptionFilterTime = DEFAULT_EXCEPTION_FILTER_TIME;

        /// <summary>
        /// 
        /// </summary>
        public static int ExceptionFilterTime
        {
            get { return _exceptionFilterTime; }
            set
            {
                if (value < MINIMAL_EXCEPTION_FILTER_TIME)
                    _exceptionFilterTime = -1;
                else
                {
                    _exceptionFilterTime = value;
                }
            }
        }

        private static volatile TimeoutDictionary<string, Exception> _recentExceptions = null;
        private static readonly object _recentExceptionsLock = new object();

        private static void EnsureRecentExceptions()
        {
            if (null == _recentExceptions)
                lock(_recentExceptionsLock)
                    if (null == _recentExceptions)
                    {
                        _recentExceptions = new TimeoutDictionary<string, Exception>(_exceptionFilterTime);
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
                if (ReferenceEquals(exception,null) ||
                    (!_useOutputFile && _useLogClass == null && ExceptionOccured == null))
                    // no need for any more examination
                    return; // do not raise any other exception

                if (_exceptionFilterTime > 0)
                {
                    EnsureRecentExceptions();

                    int threadId = 0;
                    try
                    {
                        threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    }
                    catch
                    {
                        
                    }

                    string exceptionFakeId =
                        string.Concat(
                            threadId,
                            exception.GetType(),
                            exception.Message,
                            exception.StackTrace
                            );

                    // intentionally this approach, instead of evaluation bool from SetValue method
                    // because SetValue would prolong the timeout of the item all the time
                    // 
                    // with this approach 
                    if (_recentExceptions.ContainsKey(exceptionFakeId))
                        // still within filter time
                        return;

                    _recentExceptions.SetValue(exceptionFakeId,exception,_exceptionFilterTime);

                }

                EnsureProcessingQueue();

                _pq.Enqueue(new ExceptionCarrier(exception));
            }
            catch (OutOfMemoryException)
            {
                try
                {
                    if (null != _pq)
                        _pq.Clear();
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
        /// <param name="exceptionCarrier"></param>
        private static void OnItemProcessing(ExceptionCarrier exceptionCarrier)
        {
            try
            {
                if (null == exceptionCarrier ||
                    exceptionCarrier._exception== null)
                    return;

                if (_useLogClass != null)
                {
                    try
                    {
                        _useLogClass.Error("Exception thrown\r\n   " + exceptionCarrier._exception);
                    }
                    catch
                    {
                    }
                }

                if (null != ExceptionOccured)
                    try
                    {
                        ExceptionOccured(
                            exceptionCarrier._exception,
                            exceptionCarrier._timestamp,
                            exceptionCarrier._threadId
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

                            string message = String.Empty;

                            try
                            {
                                message = 
                                string.Format("{0}{1}{2}{1}{1}",
// ReSharper disable once ImpureMethodCallOnReadonlyValueField
                                    exceptionCarrier._timestamp.ToString("yyMMdd HH:mm:ss"),
                                    Environment.NewLine, exceptionCarrier._exception);
                            }
                            catch
                            {
                                
                            }

                            ByteDataCarrier bdc = new ByteDataCarrier(message);

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
                    System.Threading.Timer t = new System.Threading.Timer(OnFlushFileTimeout, null, _ofFlushTimeout, -1);
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
                EnsureProcessingQueue();

                return _pq.ProcessingThreadPriority;
            }
            set
            {
                EnsureProcessingQueue();

                _pq.ProcessingThreadPriority = value;
            }
        }

        
    }
}
