using System;
using System.Text;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class StringBuilderPool : AObjectPool<StringBuilder>
    {
        /// <summary>
        /// 2kB used to cover packet-like (~1500B) buffers
        /// </summary>
        public const int DEFAULT_IMPLICIT_CAPACITY = 2048;

        /// <summary>
        /// 
        /// </summary>
        public const int MAX_IMPLICIT_CAPACITY = 
#if COMPACT_FRAMEWORK
            65536       // usually 2^24 capacity or .NETs default int.MaxValue is way too big here
#else
            16777216
#endif
            ;            

        private volatile int _implicitCapacity = DEFAULT_IMPLICIT_CAPACITY;

        /// <summary>
        /// 
        /// </summary>
        public int ImplicitCapacity
        {
            get { return _implicitCapacity; }
            set
            {
                if (value > MAX_IMPLICIT_CAPACITY)
                    throw new ArgumentException("Implicit capacity not supported to be more than "+MAX_IMPLICIT_CAPACITY);

                if (value <= 0)
                {
#if DEBUG
                    DebugHelper.TryBreak(
                            "StringBuilderPool implicit capacity "+value+
                            " is not advised. Thus used "+DEFAULT_IMPLICIT_CAPACITY);
#endif
                    value = DEFAULT_IMPLICIT_CAPACITY;
                }

                _implicitCapacity = value;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="implicitCapacity"></param>
        public StringBuilderPool(int implicitCapacity)
        {
            ImplicitCapacity = implicitCapacity;
        }


        protected override StringBuilder CreateObject()
        {
            return new StringBuilder(_implicitCapacity,MAX_IMPLICIT_CAPACITY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>StringBuilder with Length set to 0</returns>
        public override StringBuilder Get()
        {
            bool newlyAdded;
            StringBuilder sb = base.Get(out newlyAdded);

            if (!newlyAdded)
            {
                if (sb.Capacity < _implicitCapacity)
                    sb.Capacity = _implicitCapacity;

                sb.Length = 0;
            }

            return sb;
        }

        private static volatile StringBuilderPool _implicitInstance2k = null;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// provides implicit StringBuilderPool instance with 2k capacity
        /// 
        /// 2k used to cover packet-like buffers
        /// </summary>
        public static StringBuilderPool Implicit2k
        {
            get
            {
                if (null == _implicitInstance2k)
                    lock (_syncRoot)
                    {
                        if (null == _implicitInstance2k)
                            _implicitInstance2k = new StringBuilderPool(2048);
                    }

                return _implicitInstance2k;
            }
        }

        private static volatile StringBuilderPool _implicitInstance128 = null;

        /// <summary>
        /// provides implicit StringBuilderPool instance with 128B capacity
        /// 
        /// 128B for smaller fragmentation
        /// </summary>
        public static StringBuilderPool Implicit128
        {
            get
            {
                if (null == _implicitInstance128)
                    lock (_syncRoot)
                    {
                        if (null == _implicitInstance128)
                            _implicitInstance128 = new StringBuilderPool(128);
                    }

                return _implicitInstance128;
            }
        }
    }
}
