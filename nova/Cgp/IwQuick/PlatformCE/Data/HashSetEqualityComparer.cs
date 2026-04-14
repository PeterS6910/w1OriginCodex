using System.Collections.Generic;

namespace Contal.IwQuick.Data
{
	internal class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
	{
		private readonly IEqualityComparer<T> m_comparer;
		public HashSetEqualityComparer()
		{
			m_comparer = EqualityComparer<T>.Default;
		}
		public HashSetEqualityComparer(IEqualityComparer<T> comparer)
		{
			if (m_comparer == null)
			{
				m_comparer = EqualityComparer<T>.Default;
				return;
			}
			m_comparer = comparer;
		}
		public bool Equals(HashSet<T> x, HashSet<T> y)
		{
			return HashSet<T>.HashSetEquals(x, y, m_comparer);
		}
		public int GetHashCode(HashSet<T> obj)
		{
			int num = 0;
			if (obj != null)
			{
				foreach (T current in obj)
				{
					num ^= (m_comparer.GetHashCode(current) & 2147483647);
				}
			}
			return num;
		}
		public override bool Equals(object obj)
		{
			var hashSetEqualityComparer = obj as HashSetEqualityComparer<T>;
			return hashSetEqualityComparer != null && m_comparer == hashSetEqualityComparer.m_comparer;
		}
		public override int GetHashCode()
		{
			return m_comparer.GetHashCode();
		}
	}
}
