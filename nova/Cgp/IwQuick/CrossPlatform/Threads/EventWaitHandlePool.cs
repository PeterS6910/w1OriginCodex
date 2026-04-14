using System.Threading;
using System;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMutex"></typeparam>
    public class EventWaitHandlePool<TMutex> : AObjectPool<TMutex>
        where TMutex:EventWaitHandle
    {
// ReSharper disable once StaticFieldInGenericType
        private static readonly object _syncRoot = new object();
        private static volatile EventWaitHandlePool<TMutex> _singleton;

        public static EventWaitHandlePool<TMutex> Singleton
        {
            get
            {
                if (null == _singleton)
                    lock(_syncRoot)
                        if (null == _singleton)
                            _singleton = new EventWaitHandlePool<TMutex>();

                return _singleton;
            }
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public EventWaitHandlePool()
        {
            if (typeof(TMutex) != typeof(AutoResetEvent) &&
                typeof(TMutex) != typeof(ManualResetEvent))
                throw new NotImplementedException("Instantiating variant " + typeof(TMutex) + " is not implemented at the moment");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override TMutex CreateObject()
        {
            EventWaitHandle ewh = null;

            if (typeof (TMutex) == typeof (ManualResetEvent))
            {
                ewh = new ManualResetEvent(false);
            }
            else if (typeof (TMutex) == typeof (AutoResetEvent))
                ewh = new AutoResetEvent(false);

            if (ewh != null)
#if COMPACT_FRAMEWORK
                GC.SuppressFinalize(ewh);
#else
                // expecting return even in disposing mechanism of other classes,
                // so do not allow finalizing of these created Mutexes !!!
                GC.SuppressFinalize(ewh.SafeWaitHandle);
#endif
            else
                throw new NotImplementedException("Instantiating variant "+typeof(TMutex)+" is not implemented at the moment");

            return (TMutex)ewh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TMutex Get()
        {
            bool newlyAdded;
            EventWaitHandle ewh = base.Get(out newlyAdded);

            if (!newlyAdded)
                ewh.Reset();

            return (TMutex)ewh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnedObject"></param>
        public override void Return(TMutex returnedObject)
        {
            /*object o = null;
            try
            {
                o = returnedObject.ToString();
                o = returnedObject.WaitOne(0, false);
                o = returnedObject.Reset();
            }
            catch(Exception e)
            {
                DebugHelper.NOP(o);
            }*/

            if (returnedObject != null)
            {
                base.Return(returnedObject);
            }

        }

#if DEBUG
// ReSharper disable once StaticFieldInGenericType
        private static int _iterations = 0;
#endif

#if DEBUG
        /// <summary>
        /// 
        /// </summary>
        /// <param name="freeObjectsCountSnapshot"></param>
        /// <param name="averageFreeObjectsCount"></param>
        /// <param name="maxFreeObjectsCount"></param>
        /// <returns></returns>
        protected override int ValidateReducingCondition(
            int freeObjectsCountSnapshot, 
            double averageFreeObjectsCount,
            int maxFreeObjectsCount)
        {

            _iterations++;
            if (_iterations % 1000 == 0)
                System.Diagnostics.Debug.WriteLine(
                    string.Concat(">>> EventWaitHandlePool<",typeof(TMutex),
                        "> : free=" , freeObjectsCountSnapshot , 
                        " avg=" , averageFreeObjectsCount,
                        " max=" , maxFreeObjectsCount));


            return -1;
        }

#endif
    }

    
}
