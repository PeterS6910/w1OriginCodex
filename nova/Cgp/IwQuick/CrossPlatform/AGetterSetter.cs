namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AGetterSetter<T> : AGetter<T>
    {



        protected AGetterSetter(T defaultValue)
            :base(defaultValue)
        {
            
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public new T Value
        {
            get { return Get(); }
            set { Set(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Set(T value)
        {
            SetValue(ref _value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToBeModified"></param>
        protected abstract void SetValue(ref T valueToBeModified);

        
    }
}