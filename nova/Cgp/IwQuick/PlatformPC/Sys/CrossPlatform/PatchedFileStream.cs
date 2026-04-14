using System.IO;
using System;
using System.Threading;
using System.Diagnostics;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Sys
#else

namespace Contal.IwQuickCF.Sys
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class PatchedFileStream : FileStream
    {
        /// <summary>
        /// 
        /// </summary>
        public enum FileOperation
        {
            /// <summary>
            /// 
            /// </summary>
            OpenOrCreate,
            /// <summary>
            /// 
            /// </summary>
            None,
            /// <summary>
            /// 
            /// </summary>
            Read,
            /// <summary>
            /// 
            /// </summary>
            Write,
            /// <summary>
            /// 
            /// </summary>
            Seek,
            /// <summary>
            /// 
            /// </summary>
            Close,
            /// <summary>
            /// 
            /// </summary>
            Flush
        }

        private readonly int _bufferSize;

        private const int DEFAULT_LOW_LEVEL_RETRY_COUNT = 3;

        private volatile int _lowLevelRetryCount = DEFAULT_LOW_LEVEL_RETRY_COUNT;

        /// <summary>
        /// 
        /// </summary>
        private const int DEFAULT_BUFFERSIZE = 32768;

        private const bool DEFAULT_USE_FLUSH_PATCH = true;
        
        /// <summary>
        /// defines newly instantiated PatchedFileStream.UseFlushPatch property
        /// </summary>
        public static volatile bool ImplicitUseFlushPatch = DEFAULT_USE_FLUSH_PATCH;

        private volatile bool _useFlushPatch = ImplicitUseFlushPatch;

        /// <summary>
        /// 
        /// </summary>
        public bool UseFlushPatch
        {
            get { return _useFlushPatch; }
            set { _useFlushPatch = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LowLevelRetryCount
        {
            get
            {
                return _lowLevelRetryCount;
            }
            set
            {
                if (value > 0)
                    _lowLevelRetryCount = value;
                else
                    _lowLevelRetryCount = DEFAULT_LOW_LEVEL_RETRY_COUNT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int FailsafeDelay
        {
            get { return _failsafeDelay; }
            set
            {
                if (value <= 0)
                    _failsafeDelay = -1;
                else
                    _failsafeDelay = value;
            }
        }

        private const int DEFAULT_FAILSAFE_DELAY = 50;
        private volatile int _failsafeDelay = DEFAULT_FAILSAFE_DELAY;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="bufferSize"></param>
        private PatchedFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
            : base(path, mode, access, share, bufferSize)
        {
            _bufferSize = bufferSize;

            try
            {
                if (mode != FileMode.Open && _useFlushPatch)
                    // SD hack, see trac nova #2665
                    base.Flush();
            }
            catch
            {
                try
                {
                    base.Close();
                }
                catch
                {

                }

                throw;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <returns></returns>
        public static PatchedFileStream Open(
            string path, 
            FileMode mode, 
            FileAccess access, 
            FileShare share)
        {
            return Open(path, mode, access, share, DEFAULT_BUFFERSIZE);
        }

        private static volatile Data.TimeoutDictionary<string, FileOperation> _fileOpenErrorFiltering = null;
        private static readonly object _foefLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static PatchedFileStream Open(
            string path, 
            FileMode mode, 
            FileAccess access, 
            FileShare share,
            int bufferSize)
        {
            PatchedFileStream pfs = null;

            try
            {
                pfs = new PatchedFileStream(path, mode, access, share, bufferSize);
            }
            catch(Exception error)
            {
                if (null != pfs)
                    try
                    {
                        pfs.Close();
                    }
                    catch
                    {
                        
                    }

                if (ErrorOccured != null)
                    try
                    {
                        if (null == _fileOpenErrorFiltering)
                            lock (_foefLock)
                            {
                                if (null == _fileOpenErrorFiltering)
                                {
                                     var tmp =
                                        new Data.TimeoutDictionary<string, FileOperation>(DEFAULT_ERROR_SIGNALING_TIMEOUT);
                                     tmp.ItemTimedOut += tmp_ItemTimedOut;

                                     _fileOpenErrorFiltering = tmp;
                                }

                               
                            }

                        if (_fileOpenErrorFiltering.SetValue(path,FileOperation.OpenOrCreate))
                        {
                            // 0->1
                            try
                            {
                                ErrorOccured(path, FileOperation.OpenOrCreate, error);
                            }
                            catch
                            {
                                
                            }


                        }
                    }
                    catch
                    {
                        
                    }

                throw;
            }

            return pfs;
        }

        static void tmp_ItemTimedOut(string key, FileOperation value, int timeout)
        {
            if (ErrorOccured != null)
                try
                {
                    ErrorOccured(key, value, null);
                }
                catch
                {
                    
                }
        }

        



        //private readonly Random _r = new Random(Environment.TickCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] array, int offset, int count)
        {
            if (ReferenceEquals(array, null))
                throw new ArgumentNullException("array");

            array.VerifyIndexOffset(ref offset, ref count, false);

            int bytesWritten = 0;
            long originalPosition = -1;

            for (int retry = 0; retry < _lowLevelRetryCount; retry++)
            {
                try
                {
                    while (bytesWritten != count)
                    {
                        int sizeToWrite = _bufferSize;
                        int remainingSizeInBdc = count - bytesWritten;
                        if (remainingSizeInBdc < sizeToWrite)
                            sizeToWrite = remainingSizeInBdc;

                        originalPosition = base.Position;

                        //throw new IOException("fake");
                        
                        base.Write(array, offset + bytesWritten, sizeToWrite);

                        if (_useFlushPatch)
                            // SD hack, see trac nova #2665
                            base.Flush();

                        bytesWritten += (int)(base.Position - originalPosition);
                    }

                    break;
                }
                catch (IOException error)
                {
                    if (retry == _lowLevelRetryCount - 1)
                    {
                        // last retry
                        ReportException(error,FileOperation.Write);
                        throw;
                    }

                    if (originalPosition >= 0)
                    {
                        try
                        {
                            base.Seek(originalPosition, SeekOrigin.Begin);
                        }
                        catch(Exception seekError)
                        {
                            ReportException(seekError,FileOperation.Read);
                            throw;
                        }

                        if (_failsafeDelay > 0)
                            Thread.Sleep(_failsafeDelay);
                    }
                    else
                    {
                        ReportException(error,FileOperation.Write);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] array, int offset, int count)
        {
            if (ReferenceEquals(array, null))
                throw new ArgumentNullException("array");

            array.VerifyIndexOffset(ref offset, ref count, false);

            long originalPosition = 0;

            for (int retry = 0; retry < _lowLevelRetryCount; retry++)
            {
                try
                {
                    originalPosition = base.Position;

                    int read = 0;
                    int raceCondition = 0;
                    do
                    {
                        //throw new IOException("fake");

                        read += base.Read(array, offset + read, count - read);

                        if (base.Position == base.Length)
                            // EOF
                            break;

                        if (read < count)
                        {
                            if (base.Position != originalPosition + read)
                                // paranoid condition, but who can be sure with CE
                                // to apply retrying mechanism
                                throw new IOException("Read count and position differs");

                            DebugHelper.NOP();
                        }

                        if (raceCondition++ > 255)
                        {
                            Debugger.Break();
                            // should be different than IOException
                            throw new InvalidOperationException("Reading "+Name+" seems to be in race loop");
                        }
                    }                    
                    while (
                        // solving the situation, when the FileStream in CF is returning fewer data-bytes than count
                        // even though those bytes in file are available
                        (read < count && read > 0) || 

                        // the read == 0 can still be a problematic situation instead of actual EOF 
                        // so validate on position condition
                        (read == 0 && base.Position != base.Length));

                    return read;
                }
                catch (IOException error)
                {
                    if (retry == _lowLevelRetryCount - 1)
                    {
                        ReportException(error,FileOperation.Read);
                        throw;
                    }

                    if (originalPosition >= 0)
                    {
                        try
                        {
                            //throw new IOException("fake");
                            base.Seek(originalPosition, SeekOrigin.Begin);
                        }
                        catch (Exception seekError)
                        {
                            ReportException(seekError,FileOperation.Seek);
                            throw;
                        }

                        if (_failsafeDelay > 0)
                            Thread.Sleep(_failsafeDelay);
                    }
                    else
                    {
                        ReportException(error,FileOperation.Read);
                        throw;
                    }
                }
            }

            return 0;
        }

        private const int DEFAULT_ERROR_SIGNALING_TIMEOUT = 5000;

        private volatile Timer _signalingTimer = null;
        private readonly object _signalingTimerLock = new object();

        private volatile FileOperation _lastFileOperationProblem = FileOperation.None;
        private int _problemReported = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="fileOperation"></param>
        private void ReportException(Exception error,FileOperation fileOperation)
        {
            if (error == null)
                return;

            _lastFileOperationProblem = fileOperation;

            // DON'T DO HAE.Examine HERE, BECAUSE THE EXCEPTIONS
            // ARE BEING RETHROWN
            //HandledExceptionAdapter.Examine(error);

            if (ErrorOccured != null)
            {
                if (Interlocked.Exchange(ref _problemReported,1) == 0)
                    try
                    {
                        ErrorOccured(Name, fileOperation, error);
                    }
                    catch (Exception err)
                    {
                        HandledExceptionAdapter.Examine(err);
                    }

                lock (_signalingTimerLock)
                {
                    if (_signalingTimer == null)
                        _signalingTimer = new Timer(ReportProblemOrItsEnd, fileOperation, DEFAULT_ERROR_SIGNALING_TIMEOUT, -1);
                    else
                        _signalingTimer.Change(DEFAULT_ERROR_SIGNALING_TIMEOUT, -1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public delegate void DErrorOccured(string fileName, FileOperation fileOperation, Exception error);

        /// <summary>
        /// 
        /// </summary>
        public static event DErrorOccured ErrorOccured;

        private void ReportProblemOrItsEnd(object state)
        {
            if (!(state is FileOperation))
                return;

            FileOperation fileOperation = (FileOperation)state;
            
            if (Interlocked.Exchange(ref _problemReported, 0) == 0)
                return;
            

            if (ErrorOccured != null)
                try
                {
                    ErrorOccured(Name, fileOperation,null);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryClose()
        {
            if (_useFlushPatch)
                try
                {
                    base.Flush();
                }
                catch (Exception error)
                {
                    ReportException(error, FileOperation.Flush);
                }

            return Close(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool Close(bool throwException)
        {
            Exception error = null;

            try
            {
                base.Close();
            }
            catch (Exception err)
            {
                error = err;

                ReportException(err, FileOperation.Close);
            }
            finally
            {
                try
                {
                    if (Interlocked.Exchange(ref _problemReported, 0) == 1)
                    {
                        lock (_signalingTimerLock)
                        {
                            if (null != _signalingTimer)
                                _signalingTimer.Change(-1, -1);
                        }

                        // just to simulate some delay after last ErrorOccured
                        Thread.Sleep(100);

                        if (ErrorOccured != null)

                            ErrorOccured(Name, _lastFileOperationProblem, null);
                    }
                    
                }
                catch
                {
                }
            }


            if (null != error)
            {
                if (throwException)
                    throw error;

                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            Close(true);
        }
    }

    
}
