using System;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    public static class Crc32
    {
        static readonly uint[] Table;

        public static uint ComputeChecksum(string inputString)
        {
            uint crc = 0xffffffff;

            for (int i = 0; i < inputString.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ (byte)inputString[i]);
                crc = (crc >> 8) ^ Table[index];
            }

            return ~crc;
        }

        public static uint ComputeChecksum(byte[] inputBytes)
        {
            uint crc = 0xffffffff;

            for (int i = 0; i < inputBytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ inputBytes[i]);
                crc = (crc >> 8) ^ Table[index];
            }

            return ~crc;
        }

        public static byte[] ComputeChecksumBytes(string inputString)
        {
            return BitConverter.GetBytes(ComputeChecksum(inputString));
        }

        public static byte[] ComputeChecksumBytes(byte[] inputBytes)
        {
            return BitConverter.GetBytes(ComputeChecksum(inputBytes));
        }

        static Crc32()
        {
            Table = new uint[256];

            const uint poly = 0xedb88320;

            for (uint i = 0; i < Table.Length; ++i)
            {
                uint temp = i;

                for (int j = 8; j > 0; --j)
                    if ((temp & 1) == 1)
                        temp = (temp >> 1) ^ poly;
                    else
                        temp >>= 1;

                Table[i] = temp;
            }
        }
    }
}
