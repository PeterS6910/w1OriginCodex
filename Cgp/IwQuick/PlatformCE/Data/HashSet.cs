using System;
using System.Collections;
using System.Collections.Generic;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Represents a set of values.
    /// </summary>
    /// <typeparam name="T">he type of elements in the list.</typeparam>
	public class HashSet<T> : ICollection<T>
    {
		internal struct ElementCount
		{
			internal int UniqueCount;
			internal int UnfoundCount;
		}

		internal struct Slot
		{
			internal int HashCode;
			internal T Value;
			internal int Next;
		}

		public struct Enumerator : IEnumerator<T>
		{
			private readonly HashSet<T> _set;
			private int _index;
			private readonly int _version;
			private T _current;
			public T Current
			{
				get
				{
					return _current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (_index == 0 || _index == _set._lastIndex + 1)
					{
						throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
					}
					return Current;
				}
			}

			internal Enumerator(HashSet<T> set)
			{
				_set = set;
				_index = 0;
				_version = set._version;
				_current = default(T);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (_version != _set._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				while (_index < _set._lastIndex)
				{
					if (_set._slots[_index].HashCode >= 0)
					{
						_current = _set._slots[_index].Value;
						_index++;
						return true;
					}
					_index++;
				}
				_index = _set._lastIndex + 1;
				_current = default(T);
				return false;
			}

			void IEnumerator.Reset()
			{
				if (_version != _set._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				_index = 0;
				_current = default(T);
			}
		}

	    private int[] _buckets;
		private Slot[] _slots;

		private int _count;
		private int _lastIndex;
		private int _freeList;

		private readonly IEqualityComparer<T> _comparer;
		private int _version;

        //
        // Summary:
        //     Gets the number of elements that are contained in a set.
        //
        // Returns:
        //     The number of elements that are contained in the set.
		public int Count
		{
			get
			{
				return _count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

        // Summary:
        //     Gets the System.Collections.Generic.IEqualityComparer<T> object that is used
        //     to determine equality for the values in the set.
        //
        // Returns:
        //     The System.Collections.Generic.IEqualityComparer<T> object that is used to
        //     determine equality for the values in the set.
		public IEqualityComparer<T> Comparer
		{
			get
			{
				return _comparer;
			}
		}

        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.HashSet<T> class
        //     that is empty and uses the default equality comparer for the set type.
		public HashSet() : this(EqualityComparer<T>.Default)
		{
		}

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.HashSet&lt;T&gt; class
        ///     that is empty and uses the specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">The System.Collections.Generic.IEqualityComparer&lt;T&gt; implementation to use
        ///     when comparing values in the set, or null to use the default System.Collections.Generic.EqualityComparer&lt;T&gt;
        ///     implementation for the set type.</param>
		public HashSet(IEqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			_comparer = comparer;
			_lastIndex = 0;
			_count = 0;
			_freeList = -1;
			_version = 0;
		}

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.HashSet<T> class
        //     that uses the default equality comparer for the set type, contains elements
        //     copied from the specified collection, and has sufficient capacity to accommodate
        //     the number of elements copied.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements are copied to the new set.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
		public HashSet(IEnumerable<T> collection) : this(collection, EqualityComparer<T>.Default)
		{
		}

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.HashSet<T> class
        //     that uses the specified equality comparer for the set type, contains elements
        //     copied from the specified collection, and has sufficient capacity to accommodate
        //     the number of elements copied.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements are copied to the new set.
        //
        //   comparer:
        //     The System.Collections.Generic.IEqualityComparer<T> implementation to use
        //     when comparing values in the set, or null to use the default System.Collections.Generic.EqualityComparer<T>
        //     implementation for the set type.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
		public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			var capacity = 0;
			var collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				capacity = collection2.Count;
			}
			Initialize(capacity);
			UnionWith(collection);
			if ((_count == 0 && _slots.Length > HashHelpers.GetMinPrime()) || (_count > 0 && _slots.Length / _count > 3))
			{
				TrimExcess();
			}
		}

		void ICollection<T>.Add(T item)
		{
			AddIfNotPresent(item);
		}

        //
        // Summary:
        //     Removes all elements from a System.Collections.Generic.HashSet<T> object.
		public void Clear()
		{
			if (_lastIndex > 0)
			{
				Array.Clear(_slots, 0, _lastIndex);
				Array.Clear(_buckets, 0, _buckets.Length);
				_lastIndex = 0;
				_count = 0;
				_freeList = -1;
			}
			_version++;
		}

        //
        // Summary:
        //     Determines whether a System.Collections.Generic.HashSet<T> object contains
        //     the specified element.
        //
        // Parameters:
        //   item:
        //     The element to locate in the System.Collections.Generic.HashSet<T> object.
        //
        // Returns:
        //     true if the System.Collections.Generic.HashSet<T> object contains the specified
        //     element; otherwise, false.
		public bool Contains(T item)
		{
			if (_buckets != null)
			{
				var num = InternalGetHashCode(item);
				for (var i = _buckets[num % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
				{
					if (_slots[i].HashCode == num && _comparer.Equals(_slots[i].Value, item))
					{
						return true;
					}
				}
			}
			return false;
		}

        /// <summary>
        /// Copies the elements of a System.Collections.Generic.HashSet&lt;T&gt; object to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied
        ///     from the System.Collections.Generic.HashSet&lt;T&gt; object. The array must have
        ///     zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">
        ///     arrayIndex is greater than the length of the destination array.  -or- count
        ///     is larger than the size of the destination array</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex, _count);
		}

		public bool Remove(T item)
		{
			if (_buckets != null)
			{
				var num = InternalGetHashCode(item);
				var num2 = num % _buckets.Length;
				var num3 = -1;
				for (var i = _buckets[num2] - 1; i >= 0; i = _slots[i].Next)
				{
					if (_slots[i].HashCode == num && _comparer.Equals(_slots[i].Value, item))
					{
						if (num3 < 0)
						{
							_buckets[num2] = _slots[i].Next + 1;
						}
						else
						{
							_slots[num3].Next = _slots[i].Next;
						}
						_slots[i].HashCode = -1;
						_slots[i].Value = default(T);
						_slots[i].Next = _freeList;
						_freeList = i;
						_count--;
						_version++;
						return true;
					}
					num3 = i;
				}
			}
			return false;
		}

        //
        // Summary:
        //     Returns an enumerator that iterates through a System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Returns:
        //     A System.Collections.Generic.HashSet<T>.Enumerator object for the System.Collections.Generic.HashSet<T>
        //     object.
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

        
        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>true if the element is added to the System.Collections.Generic.HashSet&lt;T&gt; object; 
        /// false if the element is already present.</returns>
		public bool Add(T item)
		{
			return AddIfNotPresent(item);
		}

		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (var current in other)
			{
				AddIfNotPresent(current);
			}
		}

        //
        // Summary:
        //     Modifies the current System.Collections.Generic.HashSet<T> object to contain
        //     only elements that are present in that object and in the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_count == 0)
			{
				return;
			}
			var collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					Clear();
					return;
				}
				var hashSet = other as HashSet<T>;
				if (hashSet != null && AreEqualityComparersEqual(this, hashSet))
				{
					IntersectWithHashSetWithSameEC(hashSet);
					return;
				}
			}
			IntersectWithEnumerable(other);
		}

        //
        // Summary:
        //     Removes all elements in the specified collection from the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Parameters:
        //   other:
        //     The collection of items to remove from the System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_count == 0)
			{
				return;
			}
			if (other == this)
			{
				Clear();
				return;
			}
			foreach (var current in other)
			{
				Remove(current);
			}
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_count == 0)
			{
				UnionWith(other);
				return;
			}
			if (other == this)
			{
				Clear();
				return;
			}
			var hashSet = other as HashSet<T>;
			if (hashSet != null && AreEqualityComparersEqual(this, hashSet))
			{
				SymmetricExceptWithUniqueHashSet(hashSet);
				return;
			}
			SymmetricExceptWithEnumerable(other);
		}

        //
        // Summary:
        //     Determines whether a System.Collections.Generic.HashSet<T> object is a subset
        //     of the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Returns:
        //     true if the System.Collections.Generic.HashSet<T> object is a subset of other;
        //     otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_count == 0)
			{
				return true;
			}
			var hashSet = other as HashSet<T>;
			if (hashSet != null && AreEqualityComparersEqual(this, hashSet))
			{
				return _count <= hashSet.Count && IsSubsetOfHashSetWithSameEC(hashSet);
			}
			var elementCount = CheckUniqueAndUnfoundElements(other, false);
			return elementCount.UniqueCount == _count && elementCount.UnfoundCount >= 0;
		}

        //
        // Summary:
        //     Determines whether a System.Collections.Generic.HashSet<T> object is a proper
        //     subset of the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Returns:
        //     true if the System.Collections.Generic.HashSet<T> object is a proper subset
        //     of other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			var collection = other as ICollection<T>;
			if (collection != null)
			{
				if (_count == 0)
				{
					return collection.Count > 0;
				}
				var hashSet = other as HashSet<T>;
				if (hashSet != null && AreEqualityComparersEqual(this, hashSet))
				{
					return _count < hashSet.Count && IsSubsetOfHashSetWithSameEC(hashSet);
				}
			}
			var elementCount = CheckUniqueAndUnfoundElements(other, false);
			return elementCount.UniqueCount == _count && elementCount.UnfoundCount > 0;
		}

        //
        // Summary:
        //     Determines whether a System.Collections.Generic.HashSet<T> object is a superset
        //     of the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Returns:
        //     true if the System.Collections.Generic.HashSet<T> object is a superset of
        //     other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			var collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return true;
				}
				var hashSet = other as HashSet<T>;
				if (hashSet != null && AreEqualityComparersEqual(this, hashSet) && hashSet.Count > _count)
				{
					return false;
				}
			}
			return ContainsAllElements(other);
		}

        //
        // Summary:
        //     Determines whether a System.Collections.Generic.HashSet<T> object is a proper
        //     superset of the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Returns:
        //     true if the System.Collections.Generic.HashSet<T> object is a proper superset
        //     of other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_count == 0)
			{
				return false;
			}
			var collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return true;
				}
				var hashSet = other as HashSet<T>;
				if (hashSet != null && AreEqualityComparersEqual(this, hashSet))
				{
					return hashSet.Count < _count && ContainsAllElements(hashSet);
				}
			}
			var elementCount = CheckUniqueAndUnfoundElements(other, true);
			return elementCount.UniqueCount < _count && elementCount.UnfoundCount == 0;
		}

        //
        // Summary:
        //     Determines whether the current System.Collections.Generic.HashSet<T> object
        //     overlaps the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current System.Collections.Generic.HashSet<T>
        //     object.
        //
        // Returns:
        //     true if the System.Collections.Generic.HashSet<T> object and other share
        //     at least one common element; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_count == 0)
			{
				return false;
			}
			foreach (var current in other)
			{
				if (Contains(current))
				{
					return true;
				}
			}
			return false;
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			var hashSet = other as HashSet<T>;
			if (hashSet != null && AreEqualityComparersEqual(this, hashSet))
			{
				return _count == hashSet.Count && ContainsAllElements(hashSet);
			}
			var collection = other as ICollection<T>;
			if (collection != null && _count == 0 && collection.Count > 0)
			{
				return false;
			}
			var elementCount = CheckUniqueAndUnfoundElements(other, true);
			return elementCount.UniqueCount == _count && elementCount.UnfoundCount == 0;
		}

        /// <summary>
        /// Copies the elements of a System.Collections.Generic.HashSet&lt;T&gt; object to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied
        ///     from the System.Collections.Generic.HashSet&lt;T&gt; object. The array must have
        ///     zero-based indexing.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
		public void CopyTo(T[] array)
		{
			CopyTo(array, 0, _count);
		}

	    /// <summary>
	    /// Copies the elements of a System.Collections.Generic.HashSet&lt;T&gt; object to an array.
	    /// </summary>
	    /// <param name="array">The one-dimensional array that is the destination of the elements copied
	    ///     from the System.Collections.Generic.HashSet&lt;T&gt; object. The array must have
	    ///     zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy to array.</param>
	    /// <exception cref="System.ArgumentNullException">array is null.</exception>
	    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.  -or- count is less than 0.</exception>
	    /// <exception cref="System.ArgumentException">
	    ///     arrayIndex is greater than the length of the destination array.  -or- count
	    ///     is greater than the available space from the index to the end of the destination
	    ///     array.</exception>
	    public void CopyTo(T[] array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (arrayIndex > array.Length || count > array.Length - arrayIndex)
			{
				throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
			}
			var num = 0;
			var num2 = 0;
			while (num2 < _lastIndex && num < count)
			{
				if (_slots[num2].HashCode >= 0)
				{
					array[arrayIndex + num] = _slots[num2].Value;
					num++;
				}
				num2++;
			}
		}

		public int RemoveWhere(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			var num = 0;
			for (var i = 0; i < _lastIndex; i++)
			{
				if (_slots[i].HashCode >= 0)
				{
					var value = _slots[i].Value;
					if (match(value) && Remove(value))
					{
						num++;
					}
				}
			}
			return num;
		}

		public void TrimExcess()
		{
			if (_count == 0)
			{
				_buckets = null;
				_slots = null;
				_version++;
				return;
			}
			var prime = HashHelpers.GetPrime(_count);
			var array = new Slot[prime];
			var array2 = new int[prime];
			var num = 0;
			for (var i = 0; i < _lastIndex; i++)
			{
				if (_slots[i].HashCode >= 0)
				{
					array[num] = _slots[i];
					var num2 = array[num].HashCode % prime;
					array[num].Next = array2[num2] - 1;
					array2[num2] = num + 1;
					num++;
				}
			}
			_lastIndex = num;
			_slots = array;
			_buckets = array2;
			_freeList = -1;
		}

		public static IEqualityComparer<HashSet<T>> CreateSetComparer()
		{
			return new HashSetEqualityComparer<T>();
		}

		private void Initialize(int capacity)
		{
			var prime = HashHelpers.GetPrime(capacity);
			_buckets = new int[prime];
			_slots = new Slot[prime];
		}
		private void IncreaseCapacity()
		{
			var num = _count * 2;
			if (num < 0)
			{
				num = _count;
			}
			var prime = HashHelpers.GetPrime(num);
			if (prime <= _count)
			{
				throw new ArgumentException("Arg_HSCapacityOverflow");
			}
			var array = new Slot[prime];
			if (_slots != null)
			{
				Array.Copy(_slots, 0, array, 0, _lastIndex);
			}
			var array2 = new int[prime];
			for (var i = 0; i < _lastIndex; i++)
			{
				var num2 = array[i].HashCode % prime;
				array[i].Next = array2[num2] - 1;
				array2[num2] = i + 1;
			}
			_slots = array;
			_buckets = array2;
		}

		private bool AddIfNotPresent(T value)
		{
			if (_buckets == null)
			{
				Initialize(0);
			}
			var num = InternalGetHashCode(value);
// ReSharper disable PossibleNullReferenceException
			var num2 = num % _buckets.Length;
// ReSharper restore PossibleNullReferenceException
			for (var i = _buckets[num % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
			{
				if (_slots[i].HashCode == num && _comparer.Equals(_slots[i].Value, value))
				{
					return false;
				}
			}
			int num3;
			if (_freeList >= 0)
			{
				num3 = _freeList;
				_freeList = _slots[num3].Next;
			}
			else
			{
				if (_lastIndex == _slots.Length)
				{
					IncreaseCapacity();
					num2 = num % _buckets.Length;
				}
				num3 = _lastIndex;
				_lastIndex++;
			}
			_slots[num3].HashCode = num;
			_slots[num3].Value = value;
			_slots[num3].Next = _buckets[num2] - 1;
			_buckets[num2] = num3 + 1;
			_count++;
			_version++;
			return true;
		}

		private bool ContainsAllElements(IEnumerable<T> other)
		{
			foreach (var current in other)
			{
				if (!Contains(current))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsSubsetOfHashSetWithSameEC(HashSet<T> other)
		{
			foreach (var current in this)
			{
				if (!other.Contains(current))
				{
					return false;
				}
			}
			return true;
		}

		private void IntersectWithHashSetWithSameEC(HashSet<T> other)
		{
			for (var i = 0; i < _lastIndex; i++)
			{
				if (_slots[i].HashCode >= 0)
				{
					var value = _slots[i].Value;
					if (!other.Contains(value))
					{
						Remove(value);
					}
				}
			}
		}

		private void IntersectWithEnumerable(IEnumerable<T> other)
		{
		    var lastIndex = _lastIndex;
		    var num = BitHelper.ToIntArrayLength(lastIndex);
		    var bitArray = new int[num];
		    var bitHelper = new BitHelper(bitArray, num);
		    foreach (var current in other)
		    {
		        var num2 = InternalIndexOf(current);
		        if (num2 >= 0)
		        {
		            bitHelper.MarkBit(num2);
		        }
		    }
		    for (var i = 0; i < lastIndex; i++)
		    {
		        if (_slots[i].HashCode >= 0 &&
		            !bitHelper.IsMarked(i))
		        {
		            Remove(_slots[i].Value);
		        }
		    }
		}

	    private int InternalIndexOf(T item)
		{
			var num = InternalGetHashCode(item);
			for (var i = _buckets[num % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
			{
				if (_slots[i].HashCode == num && _comparer.Equals(_slots[i].Value, item))
				{
					return i;
				}
			}
			return -1;
		}

		private void SymmetricExceptWithUniqueHashSet(IEnumerable<T> other)
		{
			foreach (var current in other)
			{
				if (!Remove(current))
				{
					AddIfNotPresent(current);
				}
			}
		}

		private void SymmetricExceptWithEnumerable(IEnumerable<T> other)
		{
		    var lastIndex = _lastIndex;
		    var num = BitHelper.ToIntArrayLength(lastIndex);

		    var bitArray = new int[num];
		    var bitHelper = new BitHelper(bitArray, num);
		    var bitArray2 = new int[num];
		    var bitHelper2 = new BitHelper(bitArray2, num);
		    foreach (var current in other)
		    {
		        int num2;
		        var flag = AddOrGetLocation(current, out num2);
		        if (flag)
		        {
		            bitHelper2.MarkBit(num2);
		        }
		        else
		        {
		            if (num2 < lastIndex &&
		                !bitHelper2.IsMarked(num2))
		            {
		                bitHelper.MarkBit(num2);
		            }
		        }
		    }
		    for (var i = 0; i < lastIndex; i++)
		    {
		        if (bitHelper.IsMarked(i))
		        {
		            Remove(_slots[i].Value);
		        }
		    }
		}

	    private bool AddOrGetLocation(T value, out int location)
		{
			var num = InternalGetHashCode(value);
			var num2 = num % _buckets.Length;
			for (var i = _buckets[num % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
			{
				if (_slots[i].HashCode == num && _comparer.Equals(_slots[i].Value, value))
				{
					location = i;
					return false;
				}
			}
			int num3;
			if (_freeList >= 0)
			{
				num3 = _freeList;
				_freeList = _slots[num3].Next;
			}
			else
			{
				if (_lastIndex == _slots.Length)
				{
					IncreaseCapacity();
					num2 = num % _buckets.Length;
				}
				num3 = _lastIndex;
				_lastIndex++;
			}
			_slots[num3].HashCode = num;
			_slots[num3].Value = value;
			_slots[num3].Next = _buckets[num2] - 1;
			_buckets[num2] = num3 + 1;
			_count++;
			_version++;
			location = num3;
			return true;
		}

		private ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
		{
		    ElementCount result;
		    if (_count == 0)
		    {
		        var num = 0;
		        using (var enumerator = other.GetEnumerator())
		        {
		            if (enumerator.MoveNext())
		                num++;
		        }
		        result.UniqueCount = 0;
		        result.UnfoundCount = num;
		        return result;
		    }
		    var lastIndex = _lastIndex;
		    var num2 = BitHelper.ToIntArrayLength(lastIndex);
		    var bitArray = new int[num2];
		    var bitHelper = new BitHelper(bitArray, num2);
		    var num3 = 0;
		    var num4 = 0;
		    foreach (var current in other)
		    {
		        var num5 = InternalIndexOf(current);
		        if (num5 >= 0)
		        {
		            if (!bitHelper.IsMarked(num5))
		            {
		                bitHelper.MarkBit(num5);
		                num4++;
		            }
		        }
		        else
		        {
		            num3++;
		            if (returnIfUnfound)
		            {
		                break;
		            }
		        }
		    }
		    result.UniqueCount = num4;
		    result.UnfoundCount = num3;
		    return result;
		}

	    internal T[] ToArray()
		{
			var array = new T[Count];
			CopyTo(array);
			return array;
		}

		internal static bool HashSetEquals(HashSet<T> set1, HashSet<T> set2, IEqualityComparer<T> comparer)
		{
			if (set1 == null)
			{
				return set2 == null;
			}
			if (set2 == null)
			{
				return false;
			}
			if (!AreEqualityComparersEqual(set1, set2))
			{
				foreach (var current in set2)
				{
					var flag = false;
					foreach (var current2 in set1)
					{
						if (comparer.Equals(current, current2))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
					    return false;
					}
				}
				return true;
			}
			if (set1.Count != set2.Count)
			{
				return false;
			}
			foreach (var current3 in set2)
			{
				if (!set1.Contains(current3))
				{
				    return false;
				}
			}
			return true;
		}

		private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2)
		{
			return set1.Comparer.Equals(set2.Comparer);
		}

		private int InternalGetHashCode(T item)
		{
			if (ReferenceEquals(item,null))
			{
				return 0;
			}
			return _comparer.GetHashCode(item) & 2147483647;
		}
	}
}
