using System;

namespace Contal.IwQuick.Data
{
	internal class BitHelper
	{
	    private readonly int m_length;
		private readonly int[] m_array;

	    internal BitHelper(int[] bitArray, int length)
		{
			m_array = bitArray;
			m_length = length;
		}
		internal void MarkBit(int bitPosition)
		{
		    int num2 = bitPosition/32;
		    if (num2 < m_length &&
		        num2 >= 0)
		    {
		        m_array[num2] |= 1 << bitPosition%32;
		    }
		}

	    internal bool IsMarked(int bitPosition)
		{
			int num2 = bitPosition / 32;
			return num2 < m_length && num2 >= 0 && (m_array[num2] & 1 << bitPosition % 32) != 0;
		}
		internal static int ToIntArrayLength(int n)
		{
			if (n <= 0)
			{
				return 0;
			}
			return (n - 1) / 32 + 1;
		}
	}
}
