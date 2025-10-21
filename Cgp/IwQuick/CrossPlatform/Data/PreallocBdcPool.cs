using System;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class PreallocBdcPool : AObjectPool<ByteDataCarrier>
    {
        private readonly int _preallocSize;

        public PreallocBdcPool(int preallocSize)
        {
            _preallocSize = preallocSize;
        }

        protected override ByteDataCarrier CreateObject()
        {
            return new ByteDataCarrier(_preallocSize, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Return([NotNull] ByteDataCarrier buffer)
        {
            Validator.CheckForNull(buffer, "buffer");
            if (buffer.Size < _preallocSize)
                throw new InvalidOperationException("This buffer has not enough preallocated space " + buffer.Size + " according to required " + _preallocSize);

            buffer.Clear(true);

            base.Return(buffer);
        }

        private static volatile PreallocBdcPool _implicit64kPool = null;
        private static readonly object _implicit64PoolSync = new object();

        public const int Implicit64kPreallocSize = 65536;

        /// <summary>
        /// 
        /// </summary>
        public static PreallocBdcPool Implicit64k
        {
            get
            {
                if (_implicit64kPool == null)
                    lock(_implicit64PoolSync)
                        if (_implicit64kPool == null)
                            _implicit64kPool = new PreallocBdcPool(Implicit64kPreallocSize);

                return _implicit64kPool;
            }
        }
    }
}
