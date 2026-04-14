using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

#if !COMPACT_FRAMEWORK
// ReSharper disable once CheckNamespace
namespace Contal.IwQuick.Sys
#else
// ReSharper disable once CheckNamespace
namespace Contal.IwQuickCF.Sys
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class RotaryFileLog
    {
        private const int DEFAULT_STREAM_BUFFER_SIZE = 32768;
        private const int MINIMAL_STREAM_BUFFER_SIZE = 128;
        private const int MAXIMAL_STREAM_BUFFER_SIZE = 65536;

        private int _streamBufferSize = DEFAULT_STREAM_BUFFER_SIZE;

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

        private const int DEFAULT_MAX_SUBFILE_SIZE = 2097152; // 2MB
        private volatile int _maximalSubfileSize = DEFAULT_MAX_SUBFILE_SIZE;
        /// <summary>
        /// maximal size of subfile in bytes ; 
        /// this size is aproximate and can differ from +0b to +2*MaxBytesWithoutFlush
        /// </summary>
        public int MaxSubfileSize
        {
            get { return _maximalSubfileSize; }
            set
            {
                if (value <= 0)
                    _maximalSubfileSize = DEFAULT_MAX_SUBFILE_SIZE;
                else
                {
                    /*if (value < 2 * _maxBytesWithoutFlush)
                        _maximalSubfileSize = 2 * _maxBytesWithoutFlush;
                    else*/
                        _maximalSubfileSize = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="closeAfterSuccessfulTesting"></param>
        /// <param name="streamBufferSize"></param>
        /// <returns></returns>
        internal static FileStream CheckFileCreate(string path,bool closeAfterSuccessfulTesting, int streamBufferSize)
        {
            FileStream fs = null;
            Exception error = null;

            if (streamBufferSize < MINIMAL_STREAM_BUFFER_SIZE || streamBufferSize > MAXIMAL_STREAM_BUFFER_SIZE)
                streamBufferSize = DEFAULT_STREAM_BUFFER_SIZE;

            try
            {
                fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, streamBufferSize);
            }
            catch(Exception e)
            {
                error = e;

                if (fs != null)
                {
                    fs.TryClose();
                    fs = null;
                }
            }

            if (error != null)
                throw error;
            
            if (closeAfterSuccessfulTesting)
            {
                // always true
                //if (fs != null)
                {
                    fs.TryClose();
                    fs = null;
                }
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
        public RotaryFileLog(string mainFilePath,int maximumSubFiles,int maxSubFileSize, int flushTimeout)
        {
            Validator.CheckNullString(mainFilePath);
            Validator.CheckNegativeOrZeroInt(maximumSubFiles);

            CheckFileCreate(mainFilePath,true,_streamBufferSize);

            _mainFilePath = mainFilePath;

            //_maximumSubFiles = maximumSubFiles;
            MaxSubfileSize = maxSubFileSize;

            string cardinality = "D2";

            try
            {
                int digitCount = (int) (Math.Floor(Math.Log10(maximumSubFiles)) + 1);
                if (digitCount > 2)
                    cardinality = "D" + digitCount;
            }
            catch
            {
                
            }


            FlushTimeout = flushTimeout;

            _subFiles = new SubFile[maximumSubFiles];

            string ext = Path.GetExtension(mainFilePath);

            if (string.IsNullOrEmpty(ext))
            {
                for (int i = 0; i < _subFiles.Length; i++)
                {
                    _subFiles[i] = new SubFile
                    {
                        _filePath = (mainFilePath + StringConstants.DOT + i.ToString(cardinality))
                    };

                }
            }
            else
            {
                string prefixPath = Path.ChangeExtension(mainFilePath, null);

                for (int i = 0; i < _subFiles.Length; i++)
                {
                    _subFiles[i] = new SubFile
                    {
                        _filePath = (prefixPath + StringConstants.DOT + i.ToString(cardinality) + ext)
                    };

                }
            }

            _batchWorkerQueue = new BatchWorker<object>(OnWriteMessageBatch,flushTimeout);
            //_mainPQ = new ProcessingQueue<object>(
                //ProcessingQueue<object>.Mode.Synchronous, false, ThreadPriority.Normal);
            //_mainPQ.ItemProcessing += OnWriteMessageProcessing;
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        private void OnWriteMessageBatch(IEnumerable<object> batch)
        {
            if (batch == null)
                return;

            Data.ByteDataCarrier finalBdc = new Data.ByteDataCarrier(_streamBufferSize); // just pre-allocation

            StringBuilder sb = null;

            int count = 0;
            foreach (object itemInBatch in batch)
            {
                if (itemInBatch == null)
                    return;

                count++;

                if (itemInBatch is string)
                {
                    var s = (string) itemInBatch;
                    if (sb == null)
                        sb = new StringBuilder(s);
                    else
                        sb.Append(s);

                }
                else
                {
                    if (sb != null)
                    {
                        // new Data.ByteDataCarrier is making the UTF8 to bytes conversion
                        finalBdc.Append(new Data.ByteDataCarrier(sb), true);
                        sb = null;
                    }

                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                    if (itemInBatch is Data.ByteDataCarrier)
                        finalBdc.Append((Data.ByteDataCarrier) itemInBatch);
                    else
                        // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                        if (itemInBatch is byte[])
                        {
                            //byte[] ba = (byte[])param;
                            //bdc = new ByteDataCarrier(ba, 0, ba.Length, true);
                            finalBdc.Append((byte[]) itemInBatch, true);
                        }
                }
            }

            if (sb != null)
                // new Data.ByteDataCarrier is making the UTF8 to bytes conversion
                finalBdc.Append(new Data.ByteDataCarrier(sb), true);

            DebugReport(" Batch received : " + count,false);

            if (finalBdc.ActualSize > 0)
                WriteMessageBuffer(finalBdc);
        }

        //private volatile Timer _flushCloseTimer = null;
        //private readonly object _flushCloseTimerLock = new object();

        private const int DEFAULT_FLUSH_TIMEOUT = 10000;
        private const int MINIMAL_FLUSH_TIMEOUT = 1000;

        private volatile int _flushTimeout = DEFAULT_FLUSH_TIMEOUT;
        /// <summary>
        /// in miliseconds
        /// </summary>
        public int FlushTimeout
        {
            get { return _flushTimeout; }
            set {
                int flushTimeout = value < MINIMAL_FLUSH_TIMEOUT ? DEFAULT_FLUSH_TIMEOUT : value;

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
            set {
                _streamBufferSize = (value < MINIMAL_STREAM_BUFFER_SIZE || value > MAXIMAL_STREAM_BUFFER_SIZE)
                    ? DEFAULT_STREAM_BUFFER_SIZE : value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /*private void RefreshFlushCloseTimeout()
        {
            if (null == _flushCloseTimer)
                lock (_flushCloseTimerLock)
                {
                    if (null == _flushCloseTimer)
                    {
                        _flushCloseTimer = new Timer(OnFlushTimer, null, _flushTimeout, -1);
                    }
                }
            else
                lock (_flushCloseTimerLock)
                {
                    if (null != _flushCloseTimer)
                    {
                        _flushCloseTimer.Change(_flushTimeout,-1);
                    }
                }

        }*/

        private readonly object _shiftFilesLock = new object();

        private const string LOG_PREFIX = "RotaryFileLog/ ";

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
                    LOG_PREFIX,
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
                DebugReport(string.Format("Shifting files for {0}", _mainFilePath),false);

                for (int i = _subFiles.Length - 2; i >= 0; i--)
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
                    catch(Exception error)
                    {
                        DebugReport(string.Format("Problem deleting subfile {0} :\r\n\t{1}", 
                            _subFiles[i + 1]._filePath, error.Message),true);
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
                            _subFiles[i]._filePath, _subFiles[i + 1]._filePath, error.Message),true);

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
                        _subFiles[0]._filePath, error.Message),true);
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
                        _mainFilePath, _subFiles[0]._filePath, error.Message),true);
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

        private const int LOW_LEVEL_WRITE_RETRY_COUNT = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bdc"></param>
        void WriteMessageBuffer(Data.ByteDataCarrier bdc)
        {
            if (bdc == null)
                return;

            DebugReport(" Writting "+bdc.ActualSize+ " "+Thread.CurrentThread.ManagedThreadId,false);

            //bool written = false;
            //long currentBytesWithoutFlushing = -1;

            // lock, because this section can be actually called parallely by
            // the batchworker based on System.Threading.Timer
            lock (_outputFileMainLock) 
            {
#if DEBUG
                DebugReport(" INLOCK >>> writting " + bdc.ActualSize + " " + Thread.CurrentThread.ManagedThreadId,false);
#endif

                try
                {
                    // hack for SD cards
                    //
                    // USING THIS APPROACH INSTEAD OF
                    // PatchedFileStream, because the shift can intercept
                    // even within sub-writes

                    long originalPosition = -1;
                    int retryCount = 0;

                    // should be outside of the retrying mechanism
                    int writtenCount = 0;

                    while (retryCount <= LOW_LEVEL_WRITE_RETRY_COUNT)
                    {
                        try
                        {
                            
                            while (writtenCount != bdc.ActualSize)
                            {
                                int sizeToWrite = _streamBufferSize;
                                int remainingSizeInBdc = bdc.ActualSize - writtenCount;
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

                            if (retryCount > LOW_LEVEL_WRITE_RETRY_COUNT)
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
                catch(Exception err)
                {
                    DebugReport(" Writting error : " + err.Message, true);
                }
            }

#if DEBUG
            DebugReport(" OUTLOCK >> writting " + " " + Thread.CurrentThread.ManagedThreadId,false);
#endif

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
            Write(string.Format("{0}{1}", message, StringConstants.CR_LF));
        }
    }
}
