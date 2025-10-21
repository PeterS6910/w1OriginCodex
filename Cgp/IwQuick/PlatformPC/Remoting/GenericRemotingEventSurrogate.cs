using System;

namespace Contal.IwQuick.Remoting
{
    [Serializable]
    public class GenericRemotingEventSurrogate<TReturn,TParam>: MarshalByRefObject
    {
        protected Delegate _targetDelegate;

        private GenericRemotingEventSurrogate(Func<TParam, TReturn> targetClientDelegate)
        {
            Func<TParam, TReturn> d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        public TReturn Event(TParam param)
        {
            return ((Func<TParam,TReturn>)_targetDelegate)(param);
        }

        public static Func<TParam,TReturn> Create(Func<TParam,TReturn> targetClientDelegate)
        {
            GenericRemotingEventSurrogate<TReturn,TParam> res = new GenericRemotingEventSurrogate<TReturn,TParam>(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// disable expiration of the object
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

    }
}
