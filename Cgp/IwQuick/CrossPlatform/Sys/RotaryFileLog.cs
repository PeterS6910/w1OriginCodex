using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Contal.IwQuick.CrossPlatform;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class RotaryFileLog
    {
        private class BatchExecutor : ABatchExecutor<object, RotaryFileLog>
        {
            public BatchExecutor(RotaryFileLog rotaryFileLog)
                : base(rotaryFileLog)
            {
            }

            protected override bool OnError(object request, Exception error)
            {
                return true;
            }

            protected override void AfterBatch(RotaryFileLog rotaryFileLog)
            {
                rotaryFileLog.CheckToAppendUtf8StringToFinal(rotaryFileLog._stringBuilder);

                if (rotaryFileLog._bdcTransformBatch.ActualSize > 0)
                    rotaryFileLog.WriteMessageBuffer(rotaryFileLog._bdcTransformBatch);
            }

            protected override bool BeforeBatch(RotaryFileLog param)
            {
                param._bdcTransformBatch.ActualSize = 0;
                param._bdcUtf8Transform.ActualSize = 0;
                param._stringBuilder.Length = 0;

                return true;
            }

            protected override void ExecuteInternal(
                object itemInBatch, 
                RotaryFileLog rotaryFileLog)
            {
                if (ReferenceEquals(itemInBatch, null))
                    return;

                var stringItemInBatch = itemInBatch as string;
                if (stringItemInBatch != null)
                {
                    rotaryFileLog._stringBuilder.Append(stringItemInBatch);
                    return;
                }

                rotaryFileLog.CheckToAppendUtf8StringToFinal(rotaryFileLog._stringBuilder);

                var bdcItemInBatch = itemInBatch as Data.ByteDataCarrier;
                if (bdcItemInBatch != null)
                {
                    rotaryFileLog._bdcTransformBatch.Append(bdcItemInBatch);
                    return;
                }

                var bdc = itemInBatch as byte[];
                if (bdc != null)
                    rotaryFileLog._bdcTransformBatch.Append(
                        bdc, 
                        true);
            }
        }

        private const int DefaultStreamBufferSize = 32768;
        private const int DefaultPreallocatedBufferSize = 32768;
        private const int MaximalPreallocatedBufferSize = 327680;
        private const int MinimalStreamBufferSize = 128;
        private const int MaximalStreamBufferSize = 65536;

        private int _streamBufferSize = DefaultStreamBufferSize;

        private readonly string _mainFilePath = null;

        //private int _maximumSubFiles = 0;

        private volatile FileStream _outputFileMain = null;
        private readonly object _outputFileMainLock = new object();

        /*
        private const int DEFAULT_MAX_BYTES_WITHOUT_FLUSH = 8192;
        private int _maxBytesWithoutFlush = DEFAULT_MAX_BYTES_WITHOUT_FLUSH;

        /// <summary>
        /// secondary conditions, when writes to RotaryFileLog occur too often to timeout be applied ; 
        /// if there are in file more than MaxBytesWithoutFlush bytes they will be flushed even if timeout did not tick ; 
        /// default is 8192
        /// </summary>
        public int MaxBytesWithoutFlush
        {
            get { return _maxBytesWithoutFlush; }
            set {
                _maxBytesWithoutFlush = _maxBytesWithoutFlush <= 0 ? DEFAULT_MAX_BYTES_WITHOUT_FLUSH : value;
            }
        }*/


        private class SubFile
        {
            internal string _filePath = null;
            //internal FileStream _fs = null;
            internal bool _used = false;
        }

        private readonly SubFile[] _subFiles = null;

        //private readonly ProcessingQueue<object> _mainPQ = null;

        private readonly BatchWorker<object> _batchWorkerQueue = null;

        //private volatile bool _useContinuosClosing = false;

        //private long _byteCountWhenOpened = 0;

        private const int DefaultMaxSubfileSize = 2097152; // 2MB
        private volatile int _maximalSubfileSize = DefaultMaxSubfileSize;
        /// <summary>
        /// maximal size of subfile in bytes ; 
        /// this size is aproximate and can differ from +0b to +2*MaxBytesWithoutFlush
        /// </summary>
        public int MaxSubfileSize
        {
            get { return _maximalSubfileSize; }
            set
            {
                _maximalSubfileSize = value <= 0 ? DefaultMaxSubfileSize : value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="closeAfterSuccessfulTesting"></param>
        /// <param name="streamBufferSize"></param>
        /// <returns></returns>
        internal static FileStream CheckFileCreate(string path, bool closeAfterSuccessfulTesting, int streamBufferSize)
        {
            FileStream fs = null;

            if (streamBufferSize < MinimalStreamBufferSize || streamBufferSize > MaximalStreamBufferSize)
                streamBufferSize = DefaultStreamBufferSize;

            try
            {
                fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, streamBufferSize);
            }
            catch (Exception)
            {
                fs.TryClose();
                throw;
            }

            if (closeAfterSuccessfulTesting)
            {
                // always true
                fs.TryClose();
            }

            return fs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void EnsureFileOpen()
        {
            if (_outputFileMain == null)
                lock (_outputFileMainLock)
                {
                    if (_outputFileMain == null)
                    {
                        _outputFileMain = CheckFileCreate(_mainFilePath, false, _streamBufferSize);
                        /*if (null != _outputFileMain)
                            _byteCountWhenOpened = _outputFileMain.Position;*/
                    }
                }

        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseFile()
        {
            if (_outputFileMain != null)
                lock (_outputFileMainLock)
                {
                    if (_outputFileMain != null)
                    {
                        _outputFileMain.TryClose();
                        _outputFileMain = null;
                    }
                }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainFilePath"></param>
        /// <param name="maximumSubFiles"></param>
        /// <param name="maxSubFileSize"></param>
        /// <param name="flushTimeout"></param>
        /// <exception cref="ArgumentNullException">if mainFilePath is null or empty</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException">in case the file cannot be created at the path specified</exception>
        public RotaryFileLog(string mainFilePath, int maximumSubFiles, int maxSubFileSize, int flushTimeout)
        {
            Validator.CheckNullString(mainFilePath);
            Validator.CheckNegativeOrZeroInt(maximumSubFiles);

            CheckFileCreate(mainFilePath, true, _streamBufferSize);

            _mainFilePath = mainFilePath;

            //_maximumSubFiles = maximumSubFiles;
            MaxSubfileSize = maxSubFileSize;

            // subfile suffix cardinality D2 stands for 00-99 subfiles
            var cardinality = "D2";

            try
            {
                var digitCount = (int)(Math.Floor(Math.Log10(maximumSubFiles)) + 1);
                if (digitCount > 2)
                    cardinality = "D" + digitCount;
            }
            catch
            {

            }


            FlushTimeout = flushTimeout;

            _subFiles = new SubFile[maximumSubFiles];

            var ext = Path.GetExtension(mainFilePath);

            if (string.IsNullOrEmpty(ext))
            {
                for (var i = 0; i < _subFiles.Length; i++)
                {
                    _subFiles[i] = new SubFile
                    {
                        _filePath = (mainFilePath + StringConstants.DOT + i.ToString(cardinality))
                    };

                }
            }
            else
            {
                var prefixPath = Path.ChangeExtension(mainFilePath, null);

                for (var i = 0; i < _subFiles.Length; i++)
                {
                    _subFiles[i] = new SubFile
                    {
                        _filePath = (prefixPath + StringConstants.DOT + i.ToString(cardinality) + ext)
                    };

                }
            }

            _batchWorkerQueue = new BatchWorker<object>(
                new BatchExecutor(this),
                FlushTimeout);
            //_mainPQ = new ProcessingQueue<object>(
            //ProcessingQueue<object>.Mode.Synchronous, false, ThreadPriority.Normal);
            //_mainPQ.ItemProcessing += OnWriteMessageProcessing;

        }

        // all buffers in following region are counting on fact, that OnWriteMessageBatch should be executed atomically
        // by the BatchWorker, WITHOUT any paralelism done by System.Threading.Timer internal ThreadPool behaviour
        #region transforming buffers
        private readonly Data.ByteDataCarrier _bdcTransformBatch =
            new Data.ByteDataCarrier(DefaultPreallocatedBufferSize); // just pre-allocation

        private readonly Data.ByteDataCarrier _bdcUtf8Transform =
            new Data.ByteDataCarrier(DefaultPreallocatedBufferSize);

        private readonly StringBuilder _stringBuilder = new StringBuilder(DefaultStreamBufferSize);
        #endregion

        private void CheckToAppendUtf8StringToFinal(StringBuilder stringBuilder)
        {
            if (stringBuilder == null ||
                stringBuilder.Length == 0)
                return;

            var longString = stringBuilder.ToString();

            var sizeAfterEncoding = Encoding.UTF8.GetByteCount(longString);

            // try to grow the buffer while in limits
            if (sizeAfterEncoding > _bdcUtf8Transform.Size &&
                _bdcUtf8Transform.Size < MaximalPreallocatedBufferSize)
            {
                // preallocate at least 1,5-times the demand
                var intendedSize = sizeAfterEncoding * 15 / 10;

                _bdcUtf8Transform.Grow(
                    intendedSize < MaximalPreallocatedBufferSize ? intendedSize : MaximalPreallocatedBufferSize, false);
            }


            var reachedPosition = 0;

            while (reachedPosition != sizeAfterEncoding)
            {
                var remainingSize = sizeAfterEncoding - reachedPosition;
                var subSize = remainingSize > _bdcUtf8Transform.Size
                    ? _bdcUtf8Transform.Size
                    : remainingSize;

                try
                {
                    var byteSize =
                        Encoding.UTF8.GetBytes(longString, reachedPosition, subSize, _bdcUtf8Transform.Buffer, 0);
                    _bdcUtf8Transform.ActualSize = byteSize;

                    reachedPosition += byteSize;

                    _bdcTransformBatch.Append(_bdcUtf8Transform);
                }
                catch (Exception e)
                {
                    // try to avoid HandledExceptionAdapter here,
                    // as it might recurse via RotaryFileLog usage
                    e.TryBreak();

                    break;
                }
            }



            stringBuilder.Length = 0;
        }

        //private volatile Timer _flushCloseTimer = null;
        //private readonly object _flushCloseTimerLock = new object();

        private const int DefaultFlushTimeout = 10000;
        private const int MinimalFlushTimeout = 1000;

        private volatile int _flushTimeout = DefaultFlushTimeout;
        /// <summary>
        /// in miliseconds
        /// </summary>
        public int FlushTimeout
        {
            get { return _flushTimeout; }
            set
            {
                var flushTimeout = value < MinimalFlushTimeout ? DefaultFlushTimeout : value;

                if (_batchWorkerQueue != null)
                    _batchWorkerQueue.Timeout = flushTimeout;
                _flushTimeout = flushTimeout;
            }
        }

        /*
        /// <summary>
        /// if true, the main file is trying to be closed every time after FlushTimeout 
        /// or MaxBytesWithoutFlush condition
        /// </summary>
        public bool UseContinuosClosing
        {
            get { return _useContinuosClosing; }
            set { _useContinuosClosing = value; }
        }*/

        /// <summary>
        /// 
        /// </summary>
        public int StreamBufferSize
        {
            get { return _streamBufferSize; }
            set
            {
                _streamBufferSize = (value < MinimalStreamBufferSize || value > MaximalStreamBufferSize)
                    ? DefaultStreamBufferSize : value;
            }
        }

        private readonly object _shiftFilesLock = new object();

        private const string LogPrefix = "RotaryFileLog/ ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError">if true, report line is prefixed with error </param>
        private void DebugReport(string message, bool isError)
        {
            try
            {
                var tmp = string.Format("{0} {1} {2}{3}",
                    (isError ? "ERROR" : string.Empty),
                    Data.UnifiedFormat.DateTime,
                    LogPrefix,
                    message);

                // DO NOT USE RECURSION TO Log INSTANCE HERE !!!

                Console.WriteLine(tmp);
#if DEBUG
                Debug.WriteLine(tmp);
#endif
            }
            catch
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void ShiftFiles()
        {
            lock (_shiftFilesLock)
            {
                DebugReport(string.Format("Shifting files for {0}", _mainFilePath), false);

                for (var i = _subFiles.Length - 2; i >= 0; i--)
                {
                    if (!_subFiles[i]._used)
                        continue;


                    try
                    {
                        var fp = _subFiles[i + 1]._filePath;

                        if (File.Exists(fp))
                        {
                            File.Delete(fp);
#if DEBUG
                            DebugReport("Subfile " + fp + " deleted", false);
#endif
                        }
                    }
                    catch (Exception error)
                    {
                        DebugReport(string.Format("Problem deleting subfile {0} :\r\n\t{1}",
                            _subFiles[i + 1]._filePath, error.Message), true);
                    }

                    try
                    {
                        File.Move(_subFiles[i]._filePath, _subFiles[i + 1]._filePath);
                        _subFiles[i + 1]._used = true;
#if DEBUG
                        DebugReport("Subfile " + _subFiles[i]._filePath + " moved to " + _subFiles[i + 1]._filePath, false);
#endif
                    }
                    catch (Exception error)
                    {
                        DebugReport(string.Format("Problem moving subfile {0} into {1} :\r\n\t{2}",
                            _subFiles[i]._filePath, _subFiles[i + 1]._filePath, error.Message), true);

                    }
                }
            }

            lock (_outputFileMainLock)
            {
                // in this case 
                // must be called within lock explicitly
                CloseFile();

                try
                {
                    var fp = _subFiles[0]._filePath;

                    if (File.Exists(fp))
                    {
                        File.Delete(fp);
#if DEBUG
                        DebugReport("Subfile " + fp + " deleted", false);
#endif
                    }
                }
                catch (Exception error)
                {
                    DebugReport(string.Format("Problem deleting subfile {0} :\r\n\t{1}",
                        _subFiles[0]._filePath, error.Message), true);
                }

                try
                {
                    File.Move(_mainFilePath, _subFiles[0]._filePath);
                    _subFiles[0]._used = true;
#if DEBUG
                    DebugReport("Subfile " + _mainFilePath + " moved to " + _subFiles[0]._filePath, false);
#endif
                }
                catch (Exception error)
                {
                    DebugReport(string.Format("Problem moving mainfile {0} into {1} :\r\n\t{2}",
                        _mainFilePath, _subFiles[0]._filePath, error.Message), true);
                }
            }
        }


        /*private void OnFlushTimer(object state)
        {
            
            try
            {
                bool resultClose =
                    CloseFile();

                if (resultClose)
                    DebugReport("Main file "+_mainFilePath+"closed", false);
                else
                    DebugReport("UNABLE TO CLOSE FILE " + _mainFilePath, true);

            }
            catch
            {
            }
        }*/

        private const int LowLevelWriteRetryCount = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bdc"></param>
        void WriteMessageBuffer(Data.ByteDataCarrier bdc)
        {
            if (bdc == null)
                return;

            //DebugReport(" Writting "+bdc.ActualSize+ " "+Thread.CurrentThread.ManagedThreadId,false);

            //bool written = false;
            //long currentBytesWithoutFlushing = -1;

            // lock, because this section can be actually called parallely by
            // the batchworker based on System.Threading.Timer
            lock (_outputFileMainLock)
            {
                //#if DEBUG
                //                DebugReport(" INLOCK >>> writting " + bdc.ActualSize + " " + Thread.CurrentThread.ManagedThreadId,false);
                //#endif

                try
                {
                    // hack for SD cards
                    //
                    // USING THIS APPROACH INSTEAD OF
                    // PatchedFileStream, because the shift can intercept
                    // even within sub-writes

                    long originalPosition = -1;
                    var retryCount = 0;

                    // should be outside of the retrying mechanism
                    var writtenCount = 0;

                    while (retryCount <= LowLevelWriteRetryCount)
                    {
                        try
                        {

                            while (writtenCount != bdc.ActualSize)
                            {
                                var sizeToWrite = _streamBufferSize;
                                var remainingSizeInBdc = bdc.ActualSize - writtenCount;
                                if (remainingSizeInBdc < sizeToWrite)
                                    sizeToWrite = remainingSizeInBdc;

                                // as the shift files can happen in this cycle
                                EnsureFileOpen();

                                // storing position for seek due failue
                                originalPosition = _outputFileMain.Position;

                                _outputFileMain.Write(bdc.Buffer, writtenCount, sizeToWrite);
                                // SD ensuring , see trac nova #2665
                                _outputFileMain.Flush();

                                writtenCount += sizeToWrite;

                                if (_outputFileMain.Length >= _maximalSubfileSize)
                                {
                                    ShiftFiles();
                                }
                            }

                            // ending retrying cycle, because of no exception
                            break;
                        }
                        catch (IOException)
                        {
                            retryCount++;

                            if (retryCount > LowLevelWriteRetryCount)
                                throw;

                            if (originalPosition >= 0)
                            {
                                _outputFileMain.Seek(originalPosition, SeekOrigin.Begin);
                                Thread.Sleep(50);
                            }
                            else
                                throw;
                        }
                    }

                    /*if (_outputFileMain.Length >= _maximalSubfileSize)
                        {
                            ShiftFiles();
                            return;
                        }*/

                    /*if (_useContinuosClosing)
                    {
                        written = true;
                        currentBytesWithoutFlushing = _outputFileMain.Position - _byteCountWhenOpened;
                    }*/

                }
                catch (Exception err)
                {
                    DebugReport(" Writting error : " + err.Message, true);
                }
            }

            //#if DEBUG
            //            DebugReport(" OUTLOCK >> writting " + " " + Thread.CurrentThread.ManagedThreadId,false);
            //#endif

            /* not necessary due nature of the BatchWorker
            if (_useContinuosClosing)
            {
                if (written)
                {
                    if (currentBytesWithoutFlushing < _maxBytesWithoutFlush)
                        RefreshFlushCloseTimeout();
                    //else
                    //skip refreshing, thus stopping timer to allow flushing/closing to proceed
                }
                else
                    // refresh in case of failure
                    RefreshFlushCloseTimeout();
            }
            */
        }

        /// <summary>
        /// writes message into the rotary file log if not null neither empty , without CR LF
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            if (Validator.IsNotNullString(message))
                try
                {
                    //_mainPQ.Enqueue(message);
                    _batchWorkerQueue.Add(message);
                }
                catch
                {
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">data that are used for writting ; 
        /// THEY ARE NOT CLONED INTERNALLY ; 
        /// THUS ENSURE ATOMICITY OF ACCESS TO DATA OUTSIDE</param>
        public void Write(byte[] data)
        {
            if (data != null)
                try
                {
                    //_mainPQ.Enqueue(data);
                    _batchWorkerQueue.Add(data);
                }
                catch
                {
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">data that are used for writting ; 
        /// THEY ARE NOT CLONED INTERNALLY ; 
        /// THUS ENSURE ATOMICITY OF ACCESS TO DATA OUTSIDE</param>
        public void Write(Data.ByteDataCarrier data)
        {
            if (data != null)
                try
                {
                    //_mainPQ.Enqueue(data);
                    _batchWorkerQueue.Add(data);
                }
                catch
                {
                }
        }

        /// <summary>
        /// writes message into the rotary file log , suffixing with CR LF
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            Write(message + StringConstants.CR_LF);
        }
    }
}
