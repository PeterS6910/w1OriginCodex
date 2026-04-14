using System;
using System.IO;
using System.Text;

#if COMPACT_FRAMEWORK
// ReSharper disable once CheckNamespace
namespace Contal.IwQuickCF.Crypto
#else
// ReSharper disable once CheckNamespace
namespace Contal.IwQuick.Crypto
#endif
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
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static uint ComputeChecksum(byte[] data, int startIndex, int length)
        {
            Validator.CheckNull(data);

            if (startIndex <= 0)
                startIndex = 0;
            else
                Validator.CheckIntegerRange(startIndex, 0, data.Length);

            if (length <= 0)
                length = data.Length;
            else
                Validator.CheckIntegerRange(length, 1, data.Length - startIndex + 1);
            

            uint crc = 0xffffffff;
            for (int i = startIndex; i < startIndex+length ; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ data[i]);
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
        public static uint ComputeChecksum(Data.ByteDataCarrier data)
        {
            Validator.CheckNull(data);

            return ComputeChecksum(data.Buffer, data.Offset, data.ActualSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static uint ComputeChecksum(string inputString)
        {
            Validator.CheckNull(inputString);
            
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            return ComputeChecksum(bytes, 0, -1);
                
        }

        /// <summary>
        /// computes checksum from streams actual position with count of bytes specified
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="count">if zero or negative, stream will be counted until the end</param>
        /// <returns></returns>
        public static uint ComputeChecksum(Stream inputStream,long count)
        {
            long originalPosition = -1;
            uint crc = 0xffffffff;

            // workaround due SD problems on CPU7, maybe fixed in future
            if (Environment.OSVersion.Platform == PlatformID.WinCE && 
                inputStream is FileStream)
            {
                try
                {
                    originalPosition = inputStream.Position;

                    if (count <= 0)
                        count = inputStream.Length - originalPosition;

                    int chunkSize = DEFAULT_CHUNK_SIZE;
                    if (count < chunkSize)
                        chunkSize = (int)count;

                    byte[] buffer = new byte[chunkSize];
                    int readBytes = 0;
                    int read = -1;
                    int sdProblemRetryCount = 0;

                    while (readBytes < count || read == 0)
                    {
                        long safePosition = -1;
                        try
                        {
                            safePosition = inputStream.Position;
                            read = inputStream.Read(buffer, 0, buffer.Length);
                            sdProblemRetryCount = 0;
                        }
                        catch(IOException ioError)
                        {
                            try
                            {
                                if (safePosition >= 0)
                                    // usually when is problem is caused by accidental SD read IOException, other operations
                                    // succeedes
                                    inputStream.Position = safePosition;
                            }
                            catch(Exception otherError)
                            {
                                // this will be differrent error than SD problem, so no retrying
// ReSharper disable once PossibleIntendedRethrow
                                throw otherError;
                            }


                            if (sdProblemRetryCount++ > 3)
                                // untollerable occurence of IOException
// ReSharper disable once PossibleIntendedRethrow
                                throw ioError;
                            // tolerating the ioError due it's accidental nature (observed in CE 3024,3025 and some other)

                            read = -1;
                            System.Threading.Thread.Sleep(50);
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(string.Format("\t\t\tRetrying read on Crc32 checksum with position {0}", safePosition));
#endif
                            continue;
                        }

                        for (int i = 0; i < read ; ++i)
                        {
                            byte index = (byte)(((crc) & 0xff) ^ buffer[i]);
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

            int readCount = 0;

            try
            {
                originalPosition = inputStream.Position;

                int inputByte = inputStream.ReadByte();
                readCount++;

                while (inputByte != -1)
                {
                    byte index = (byte)(((crc) & 0xff) ^ (byte)inputByte);
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
        /// <returns></returns>
        public static uint ComputeChecksum(Stream inputStream)
        {
            return ComputeChecksum(inputStream, -1);
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
        /// <returns></returns>
        public static uint ComputeChecksum(int chunkSize, FileStream inputStream)
        {
            uint crc = 0xffffffff;

            if (chunkSize <= 0)
                chunkSize = DEFAULT_CHUNK_SIZE;

            byte[] buffer = new byte[chunkSize];
            
            while (true)
            {
                int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                for (int i = 0; i < bytesRead; i++)
                {
                    // ReSharper disable once RedundantCast
                    byte index = (byte)(((crc) & 0xff) ^ (byte)buffer[i]);
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
                uint temp = i;
                for (int j = 8; j > 0; --j)
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
