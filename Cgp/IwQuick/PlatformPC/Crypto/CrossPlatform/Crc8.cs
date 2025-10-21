using System.Text;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Crypto
#else
namespace Contal.IwQuickCF.Crypto
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public static class Crc8
    {
        /// <summary>
        /// 
        /// </summary>
        public const byte DEFAULT_PRESET = 0xE3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte ComputeChecksum(byte[] data, int offset, int length)
        {

            Validator.CheckNull(data);

            if (offset <= 0)
                offset = 0;
            else
                Validator.CheckIntegerRange(offset, 0, data.Length);

            if (length <= 0)
                length = data.Length;
            else
                Validator.CheckIntegerRange(length, 1, data.Length - offset + 1);

            byte crc = 0xC7;	// bit-swapped 0xE3     

            for (int i = offset; i < offset + length; i++)
            {
                crc = (byte)(crc ^ data[i]);

                for (byte j = 0; j < 8; j++)
                {
                    if ((crc & 0x80) > 0)
                    {
                        crc = (byte)((crc << 1) ^ 0x1D);
                    }
                    else
                    {
                        crc = (byte)(crc << 1);
                    }
                }
            }
            return crc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte ComputeChecksum(string inputString)
        {
            Validator.CheckNull(inputString);

            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            return ComputeChecksum(bytes, 0, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
#if !COMPACT_FRAMEWORK
        public static byte ComputeChecksum(Contal.IwQuick.Data.ByteDataCarrier data)
#else
        public static byte ComputeChecksum(Data.ByteDataCarrier data)
#endif
        {
            Validator.CheckNull(data);

            return ComputeChecksum(data.Buffer, data.Offset, data.ActualSize);
        }
    }
}
