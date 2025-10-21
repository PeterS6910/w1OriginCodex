#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public static class BCD
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ToByte(int value)
        {
            if (value > 99)
                return 100;

            byte b = (byte)(value % 10);
            b |= (byte)((value / 10) << 4);

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FromByte(byte value)
        {
            int lowDigit = value & 0x0F;
            if (lowDigit > 9)
                lowDigit = 9;

            return (value >> 4) * 10 + lowDigit;
        }
    }
}
