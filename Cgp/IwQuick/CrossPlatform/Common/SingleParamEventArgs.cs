using System;

namespace Contal.IwQuick.CrossPlatform.Common
{
    /// <summary>
    /// Generic class for customizable event args parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleParamEventArgs<T> : EventArgs
    {
        public SingleParamEventArgs(T value)
        {
            Data = value;
        }

        /// <summary>
        /// Data that are part of fired event
        /// </summary>
        public T Data { get; private set; }
    }
}
