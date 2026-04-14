using System;
using System.Collections.Generic;
using System.Text;


namespace Contal.IwQuick.Data
{
    /// <summary>
    /// allows casting the enumerator from type fromT to it's derived or ancestor type toT
    /// 
    /// CURRENTLY NOT FOR A USE IN SERIALIZATION/REMOTING ENVIRONMENT
    /// </summary>
    /// <typeparam name="toT"></typeparam>
    /// <typeparam name="fromT"></typeparam>
    [Serializable()]
    public class CastEnumerator<Tto, Tfrom> : IEnumerator<Tto>
    {
        private IEnumerator<Tfrom> _originalEnumerator = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalEnumerator">reference to an existing enumerator of IEnumerator/fromT/ type</param>
        /// <exception cref="ArgumentNullException">if originalEnumerator is null</exception>
        /// <exception cref="InvalidCastException">if fromT type cannot be cast to toT</exception>
        public CastEnumerator(IEnumerator<Tfrom> originalEnumerator)
        {
            Validator.CheckNull(originalEnumerator);

            Type aFromType = typeof(Tfrom);
            Type aToType = typeof(Tto);


            bool bValidCast = aToType.IsAssignableFrom(aFromType);

            if (!bValidCast)
                throw new InvalidCastException("Type \"" + aFromType + "\" cannot be cast to \"" + aToType + "\"!");

            _originalEnumerator = originalEnumerator;
        }

        #region IEnumerator<fromT> Members

        public Tto Current
        {
            get
            {
                Object aObject = _originalEnumerator.Current;
                return (Tto)aObject;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return _originalEnumerator.Current;
            }
        }

        public bool MoveNext()
        {
            return _originalEnumerator.MoveNext();
        }

        public void Reset()
        {
            _originalEnumerator.Reset();
        }

        #endregion

    }
}
