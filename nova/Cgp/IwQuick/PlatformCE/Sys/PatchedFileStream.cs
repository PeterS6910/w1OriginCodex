using System.IO;
using System;
using System.Threading;
using System.Diagnostics;

namespace Contal.IwQuickCF.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class PatchedFileStream : FileStream
    {
        private readonly int _bufferSize;

        private const int DEFAULT_LOW_LEVEL_RETRY_COUNT = 3;

        private volatile int _lowLevelRetryCount = DEFAULT_LOW_LEVEL_RETRY_COUNT;

        private const int DEFAULT_BUFFERSIZE = 32768;

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
        public PatchedFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
            : base(path, mode, access, share, bufferSize)
        {
            _bufferSize = bufferSize;

            if (mode != FileMode.Open)
                // SD hack, see trac nova #2665
                base.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        public PatchedFileStream(string path, FileMode mode, FileAccess access, FileShare share)
            :this(path,mode,access,share,DEFAULT_BUFFERSIZE)
        {
            
        }

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

                        base.Write(array, offset + bytesWritten, sizeToWrite);

                        // SD hack, see trac nova #2665
                        base.Flush();

                        bytesWritten += (int)(base.Position - originalPosition);
                    }

                    break;
                }
                catch (IOException)
                {
                    if (retry == _lowLevelRetryCount - 1)
                        // last retry
                        throw;

                    if (originalPosition >= 0)
                    {
                        base.Seek(originalPosition, SeekOrigin.Begin);

                        if (_failsafeDelay > 0)
                            Thread.Sleep(_failsafeDelay);
                    }
                    else
                        throw;
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
                catch (IOException)
                {
                    if (retry == _lowLevelRetryCount - 1)
                        throw;

                    if (originalPosition >= 0)
                    {
                        base.Seek(originalPosition, SeekOrigin.Begin);

                        if (_failsafeDelay > 0)
                            Thread.Sleep(_failsafeDelay);
                    }
                    else
                        throw;
                }
            }

            return 0;
        }

        private void SignalUnrecoverableError()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryClose()
        {
            try
            {
                base.Flush();

                base.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    
}
