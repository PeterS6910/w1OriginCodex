using System;
using System.IO;
using System.Text;

namespace Contal.IwQuick.Crypto
{
    public static class Crc32
    {
        private readonly static uint[] _table;

        public const int CRC32_BYTE_SIZE = 4;

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
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ data[i]);
                crc = ((crc >> 8) ^ _table[index]);
            }
            return ~crc;
        }

        public static uint ComputeChecksum(byte[] data)
        {
            return ComputeChecksum(data, -1, -1);
        }

        public static uint ComputeChecksum(Data.ByteDataCarrier data)
        {
            Validator.CheckNull(data);

            return ComputeChecksum(data.Buffer, data.Offset, data.ActualSize);
        }

        public static uint ComputeChecksum(string inputString)
        {
            Validator.CheckNull(inputString);

            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            return ComputeChecksum(bytes, 0, -1);

        }

        /// <summary>
        /// computes checksum from stream's actual position with count of bytes specified
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="count">if zero or negative, stream will be counted until the end</param>
        /// <returns></returns>
        public static uint ComputeChecksum(Stream inputStream, int count)
        {
            uint crc = 0xffffffff;
            int readCount = 0;
            int inputByte = inputStream.ReadByte();
            readCount++;

            while (inputByte != -1)
            {
                byte index = (byte)(((crc) & 0xff) ^ (byte)inputByte);
                crc = ((crc >> 8) ^ _table[index]);

                if (count > 0 && readCount >= count)
                    break;

                inputByte = inputStream.ReadByte();
                readCount++;
            }

            return ~crc;
        }

        /// <summary>
        /// computes checksum from stream's actual position until end of the stream
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
        /// <param name="chunkSize"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static uint ComputeChecksum(int chunkSize, FileStream inputStream)
        {
            uint crc = 0xffffffff;

            byte[] buffer = new byte[chunkSize];

            while (true)
            {
                int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                for (int i = 0; i < bytesRead; i++)
                {
                    byte index = (byte)(((crc) & 0xff) ^ buffer[i]);
                    crc = ((crc >> 8) ^ _table[index]);
                }
            }

            return ~crc;
        }

        public static byte[] ComputeChecksumBytes(string inputString)
        {
            return BitConverter.GetBytes(ComputeChecksum(inputString));
        }

        static Crc32()
        {
            uint poly = 0xedb88320;
            _table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < _table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = ((temp >> 1) ^ poly);
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
