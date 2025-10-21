using System;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// provides general MarshalByRefObject surrogate for event calls
    /// </summary>
    [Serializable]
    public class RemotingEventSurrogate: MarshalByRefObject
    {
        protected Delegate _targetDelegate;

        private RemotingEventSurrogate(DVoid2Void targetClientDelegate)
        {
            DVoid2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DBool2Void targetClientDelegate)
        {
            DBool2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DObject2Void targetClientDelegate)
        {
            DObject2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DInt2Void targetClientDelegate)
        {
            DInt2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DDouble2Void targetClientDelegate)
        {
            DDouble2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DObjects2Void targetClientDelegate)
        {
            DObjects2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DString2Void targetClientDelegate)
        {
            DString2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(DException2Void targetClientDelegate)
        {
            DException2Void d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        private RemotingEventSurrogate(EventHandler targetClientDelegate)
        {
            EventHandler d = null;
            d += targetClientDelegate;
            _targetDelegate = d;
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        public void Event()
        {
            ((DVoid2Void)_targetDelegate)();
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="param"></param>
        public void Event(bool param)
        {
            ((DBool2Void)_targetDelegate)(param);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="param"></param>
        public void Event(int param)
        {
            ((DInt2Void)_targetDelegate)(param);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="param"></param>
        public void Event(double param)
        {
            ((DDouble2Void)_targetDelegate)(param);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="param"></param>
        public void Event(object param)
        {
            ((DObject2Void)_targetDelegate)(param);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="parameters"></param>
        public void Event(params object[] parameters)
        {
            ((DObjects2Void)_targetDelegate)(parameters);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="param"></param>
        public void Event(String param)
        {
            ((DString2Void)_targetDelegate)(param);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="param"></param>
        public void Event(Exception param)
        {
            ((DException2Void)_targetDelegate)(param);
        }

        /// <summary>
        /// method surrogate; NOT TO BE CALLED DIRECTLY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Event(Object sender, EventArgs args)
        {
            ((EventHandler)_targetDelegate)(sender, args);
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DVoid2Void Create(DVoid2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DVoid2Void Create(DBool2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DBool2Void Create(DObject2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DObjects2Void Create(DObjects2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DInt2Void Create(DInt2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DDouble2Void Create(DDouble2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DString2Void Create(DString2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static DException2Void Create(DException2Void targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
            return res.Event;
        }

        /// <summary>
        /// creates surrogate for remoted event on client-side
        /// </summary>
        /// <param name="targetClientDelegate">delegate to execute on client-side, when server side event raised</param>
        /// <returns>event surrogate</returns>
        public static EventHandler Create(EventHandler targetClientDelegate)
        {
            RemotingEventSurrogate res = new RemotingEventSurrogate(targetClientDelegate);
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
