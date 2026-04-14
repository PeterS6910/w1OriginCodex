namespace Contal.IwQuick
{
    public abstract class AGetter<T> : ADisposable
    {
        protected AGetter(T defaultValue)
        {
            _value = defaultValue;
        } 

        protected T _value;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            GetOrModifyValue(ref _value);

            return _value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToBePassedAndOrModified"></param>
        protected abstract void GetOrModifyValue(ref T valueToBePassedAndOrModified);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToBeDisposed"></param>
        /// <param name="isExplicitDispose"></param>
        protected abstract void DisposeValue(ref T valueToBeDisposed, bool isExplicitDispose);

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get { return Get(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExplicitDispose"></param>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            try
            {
                DisposeValue(ref _value, isExplicitDispose);
            }
            catch
            {
                if (isExplicitDispose)
                    throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static implicit operator T(AGetter<T> getter)
        {
            return getter.Get();
        }

    }
}