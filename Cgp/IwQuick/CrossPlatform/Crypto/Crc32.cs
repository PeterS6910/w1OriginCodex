using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;

namespace Contal.IwQuick.Crypto
{
    /// <summary>
    /// 
    /// </summary>
    public static class Crc32
    {
        private static readonly uint[] _table;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex">index in data, from which to start checksum counting ; if negative, replaced by 0</param>
        /// <param name="length">length of bytes to be included in checksum counting; if negative, replaced by the data.Length</param>
        /// <exception cref="ArgumentNullException">if data is null</exception>
        /// <returns></returns>
        public static uint ComputeChecksum(
            [NotNull] byte[] data, 
            int startIndex, 
            int length)
        {
            if (null == data)
                throw new ArgumentNullException("data");

            if (length == 0)
                // shortcut - no need for computing
                // TODO : correct to return 0 ?
                return 0;

            if (startIndex < 0)
                startIndex = 0;
            else
                Validator.CheckIntegerRange(startIndex, 0, data.Length);

            if (length < 0)
                length = data.Length;
            else
                Validator.CheckIntegerRange(length, 0, data.Length - startIndex + 1);
            

            var crc = 0xffffffff;
            for (var i = startIndex; i < startIndex+length ; ++i)
            {
                var index = (byte)(((crc) & 0xff) ^ data[i]);
                crc = (crc >> 8) ^ _table[index];
            }
            return ~crc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static uint ComputeChecksum(byte[] data)
        {
            return ComputeChecksum(data, -1, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if data is null</exception>
        public static uint ComputeChecksum(ByteDataCarrier data)
        {
            if (null == data)
                throw new ArgumentNullException("data");

            return ComputeChecksum(data.Buffer, data.Offset, data.ActualSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <exception cref="ArgumentNullException">if inputString is null</exception>
        /// <returns></returns>
        public static uint ComputeChecksum([NotNull] string inputString)
        {
            if (null == inputString)
                throw new ArgumentNullException("inputString");

            // TODO : correct to return 0 ?
            // optimisation/shortcut when string is empty
            if (inputString.Length == 0)
                return 0;

            uint checksum = 0;
            ByteDataCarrier.StringToBytes(inputString,Encoding.UTF8, (buffer, size) =>
            {
                checksum = ComputeChecksum(buffer, 0, size);
            });

            return checksum;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="fileSize"></param>
        /// <param name="crcProgress"></param>
        /// <returns></returns>
        public static uint ComputeChecksum(
            [NotNull] FilePackerStream inputStream, 
            out long fileSize, 
            [NotNull] Action<int> crcProgress)
        {
            var crc = 0xffffffff;
            fileSize = 0;

            var buffer = new byte[DEFAULT_CHUNK_SIZE];

            try
            {
                var read = inputStream.Read(buffer, 0, DEFAULT_CHUNK_SIZE);

                while (read > 0)
                {
                    fileSize += read;

                    for (var i = 0; i < read; i++)
                    {
                        var index = (byte)(((crc) & 0xff) ^ buffer[i]);
                        crc = (crc >> 8) ^ _table[index];
                    }

                    /* Signal progress if delegate provided and original size was read already */
// ReSharper disable ConditionIsAlwaysTrueOrFalse
                    if (crcProgress != null && inputStream.OriginalFileSize != -1)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
                    {
                        crcProgress((int)(100 * ((double)fileSize / inputStream.OriginalFileSize)));
                    }

                    read = inputStream.Read(buffer, 0, DEFAULT_CHUNK_SIZE);
                }
            }
            catch
            {
                crc = 0;
            }

            /* Signal crc is done */
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (crcProgress != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
                crcProgress(100);

            return ~crc;
        }

        /// <summary>
        /// computes checksum from streams actual position with count of bytes specified
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="count">if zero or negative, stream will be counted until the end</param>
        /// <exception cref="ArgumentNullException">if inputStream is null</exception>
        /// <returns></returns>
        public static uint ComputeChecksum(Stream inputStream,long count)
        {
            return ComputeChecksum(inputStream, -1, count);
        }

        /// <summary>
        /// computes checksum from streams actual position with count of bytes specified
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="offset"></param>
        /// <param name="count">if zero or negative, stream will be counted until the end</param>
        /// <exception cref="ArgumentNullException">if inputStream is null</exception>
        /// <returns></returns>
        public static uint ComputeChecksum(Stream inputStream,long offset,long count)
        {
            if (ReferenceEquals(inputStream, null))
                throw new ArgumentNullException("inputStream");

            // shortcut - no need for computing
            // TODO : correct to return 0 ?
            if (count == 0)
                return 0;

            if (offset >= 0 && inputStream.CanSeek)
                inputStream.Seek(offset, SeekOrigin.Begin);

            long originalPosition = -1;
            var crc = 0xffffffff;

            // workaround due SD problems on CPU7, maybe fixed in future
            if (Environment.OSVersion.Platform == PlatformID.WinCE && 
                inputStream is FileStream)
            {
                try
                {
                    originalPosition = inputStream.Position;

                    if (count < 0)
                        count = inputStream.Length - originalPosition;

                    var chunkSize = DEFAULT_CHUNK_SIZE;
                    if (count < chunkSize)
                        chunkSize = (int)count;

                    var buffer = new byte[chunkSize];
                    var readBytes = 0;
                    var read = -1;
                    var sdProblemRetryCount = 0;

                    while (readBytes < count || read == 0)
                    {
                        long safePosition = -1;
                        try
                        {
                            safePosition = inputStream.Position;
                            read = inputStream.Read(buffer, 0, buffer.Length);
                            sdProblemRetryCount = 0;
                        }
                        catch(IOException)
                        {
                            //try
                            //{
                                if (safePosition >= 0)
                                    // usually when is problem is caused by accidental SD read IOException, other operations
                                    // succeedes
                                    inputStream.Position = safePosition;
                            /*}
                            catch(Exception otherError)
                            {
                                // this will be differrent error than SD problem, so no retrying
// ReSharper disable once PossibleIntendedRethrow
                                throw;
                            }*/


                            if (sdProblemRetryCount++ > 3)
                                // untollerable occurence of IOException
// ReSharper disable once PossibleIntendedRethrow
                                throw;
                            // tolerating the ioError due it's accidental nature (observed in CE 3024,3025 and some other)

                            read = -1;
                            System.Threading.Thread.Sleep(50);
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(string.Format("\t\t\tRetrying read on Crc32 checksum with position {0}", safePosition));
#endif
                            continue;
                        }

                        for (var i = 0; i < read ; ++i)
                        {
                            var index = (byte)(((crc) & 0xff) ^ buffer[i]);
                            crc = (crc >> 8) ^ _table[index];
                        }

                        readBytes+=read;
                    }

                    crc = ~crc;
                }
                catch
                {
                    crc = 0;  // it'll be zero because of ~0xffffffff
                }
                finally
                {
                    if (originalPosition != -1)
                        try
                        {
                            inputStream.Position = originalPosition;
                        }
                        catch
                        {

                        }
                }

                return crc; 
            }

            var readCount = 0;

            try
            {
                originalPosition = inputStream.Position;

                var inputByte = inputStream.ReadByte();
                readCount++;

                while (inputByte != -1)
                {
                    var index = (byte)(((crc) & 0xff) ^ (byte)inputByte);
                    crc = (crc >> 8) ^ _table[index];

                    if (count > 0 && readCount >= count)
                        break;

                    inputByte = inputStream.ReadByte();
                    readCount++;
                }
            }
            catch
            {
            }
            finally
            {
                if (originalPosition != -1)
                    try
                    {
                        inputStream.Position = originalPosition;
                    }
                    catch
                    {
                        
                    }
            }

            return ~crc;
        }

        /// <summary>
        /// computes checksum from streams actual position until end of the stream
        /// </summary>
        /// <param name="inputStream"></param>
        /// <exception cref="ArgumentNullException">if inputStream is null</exception>
        /// <returns></returns>
        public static uint ComputeChecksum(Stream inputStream)
        {
            return ComputeChecksum(inputStream, -1,-1);
        }

        /// <summary>
        /// 
        /// </summary>
        public const int DEFAULT_CHUNK_SIZE = 8192;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="inputStream"></param>
        /// <exception cref="ArgumentNullException">if inputStream is null</exception>
        /// <returns></returns>
        public static uint ComputeChecksum(int chunkSize, FileStream inputStream)
        {
            if (ReferenceEquals(inputStream, null))
                throw new ArgumentNullException("inputStream");

            var crc = 0xffffffff;

            if (chunkSize <= 0)
                chunkSize = DEFAULT_CHUNK_SIZE;

            var buffer = new byte[chunkSize];
            
            while (true)
            {
                var bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                for (var i = 0; i < bytesRead; i++)
                {
                    // ReSharper disable once RedundantCast
                    var index = (byte)(((crc) & 0xff) ^ (byte)buffer[i]);
                    crc = (crc >> 8) ^ _table[index];
                }
            }

            return ~crc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte[] ComputeChecksumBytes(string inputString)
        {
            return BitConverter.GetBytes(ComputeChecksum(inputString));
        }

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 DEFAULT_POLYNOMIAL = 0xedb88320;

        /// <summary>
        /// instantiating support table for faster count
        /// </summary>
        static Crc32()
        {
            _table = new uint[256];
            for (uint i = 0; i < _table.Length; ++i)
            {
                var temp = i;
                for (var j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (temp >> 1) ^ DEFAULT_POLYNOMIAL;
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                _table[i] = temp;
            }
        }
    }
}
