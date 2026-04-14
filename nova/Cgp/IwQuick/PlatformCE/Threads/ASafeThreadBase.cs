using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
// ReSharper disable once RedundantUsingDirective
using Contal.IwQuick;
// ReSharper disable once RedundantUsingDirective
using Contal.IwQuick.Data;
using JetBrains.Annotations;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// abstract base for thread encapsulation classes
    /// </summary>
    public abstract partial class ASafeThreadBase : ADisposable
    {
        
        

        

        /// <summary>
        /// should be called only within derivates constructor
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        /// <param name="createSuspended"></param>
        protected void PrepareThread(ThreadPriority priority,bool useThreadPool,string name,bool createSuspended)
        {
            _useThreadPool = useThreadPool;

            if (!useThreadPool)
            {
                _thread = new Thread(SafeThreadBody);
                _threadIdSnapshot = _thread.ManagedThreadId;
                RegisterUnregisterThread(this, true);
                _thread.Priority = priority;
                _thread.IsBackground = true;
                _thread.Name = !string.IsNullOrEmpty(name) ? name : "SafeThreadBase";

                if (createSuspended)
                {
                    _state = SafeThreadState.Suspended;
                    _suspendResumeMutex = new ManualResetEvent(false);
                }
                else
                    _suspendResumeMutex = new ManualResetEvent(true);

            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        [CanBeNull]
        public static ASafeThreadBase GetThreadById(int threadId)
        {
            WeakReference stWeakReference;

            if (_threads.TryGetValue(threadId, out stWeakReference) &&
                stWeakReference != null)
                return stWeakReference.Target as ASafeThreadBase;
            
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>snapshot of all currently running threads or threads before they're started</returns>
        [NotNull]
        public static IEnumerable<ASafeThreadBase> GetThreads()
        {
            try
            {
                var snapshot = _threads.ValuesSnapshot
                    .Where(weakReference => !ReferenceEquals(weakReference.Target,null))
                    .Select(weakReference => (ASafeThreadBase)weakReference.Target);

                return snapshot;
            }
            catch
            {
                return Enumerable.Empty<ASafeThreadBase>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int ThreadCount
        {
            get { return _threads.Count; }
        }

        

        private volatile SafeThreadState _state = SafeThreadState.NotStarted;

        
        private volatile ManualResetEvent _suspendResumeMutex;

        

        

        

        

       

        

        /// <summary>
        /// 
        /// </summary>
        protected abstract void SpecificThreadBodyCall();

        /// <summary>
        /// bridge for either direct thread or parametrized direct thread
        /// </summary>
        /*protected void ThreadBody()
        {
            EncapsulatedThreadBody(null);
        }*/

        private void SafeThreadBody()
        {
            //try
            //{
            //    _systemThreadId = DllKernel32.GetCurrentThreadId();
            //}
            //catch
            //{
            //    _systemThreadId = (uint)Thread.CurrentThread.ManagedThreadId;
            //}

            Exception anyError = null;

            try
            {
                // for cases, when thread is suspended from the beginning
                _suspendResumeMutex.WaitOne();

                SpecificThreadBodyCall();
                ThreadFinalization();
                InvokeOnFinished();
            }
            catch (ThreadAbortException tae)
            {
                if (!ReferenceEquals(tae.ExceptionState, InternalThreadAbortKey.Singleton))
                    DebugHelper.NOP(tae);

                anyError = tae;
                ThreadFinalization();
                InvokeOnFinished();
            }
            catch (Exception error)
            {
                anyError = error;
                ThreadFinalization();
                InvokeOnException(error);
            }
            finally
            {
                if (_threadIdSnapshot != -1)
                    RegisterUnregisterThread(this, false);

                InvokeOnThreadStop(anyError);

#if DEBUG
                if (!_threadFinishedMutex.WaitOne(0, false))
                    DebugHelper.NOP(_threadIdSnapshot);
#endif
               
            }
        }

        /// <summary>
        /// raised, when an exception is thrown within the running thread
        /// </summary>
        public event DException2Void OnException;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        protected void InvokeOnException(Exception error)
        {
            if (null != OnException)
                try { OnException(error); }
                catch { }
        }

        /// <summary>
        /// raised, when the thread processing has finished
        /// </summary>
        public event DVoid2Void OnFinished;

        private int _finishedInvoked = 0;

        /// <summary>
        /// 
        /// </summary>
        protected void InvokeOnFinished()
        {
            // ensure being called just once
            if (Interlocked.Exchange(ref _finishedInvoked, 1) == 1)
                return;

            if (null != OnFinished)
                try { OnFinished(); }
                catch { }
        }

        /// <summary>
        /// called even if thread stops naturally or by exception
        /// </summary>
        public event DException2Void OnThreadStop;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        protected void InvokeOnThreadStop(Exception error)
        {
            if (null != OnThreadStop)
                try { OnThreadStop(error); }
                catch { }
        }

        /// <summary>
        /// wait for thread exit for infinite time
        /// </summary>
        public void Join()
        {
            _thread.Join();
        }

        /// <summary>
        /// wait for thread exit for infinite time
        /// </summary>
        public void WaitForExit()
        {
            _thread.Join();
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="milisecondsTimeout">time to wait for the thread to exit in miliseconds</param>
        public bool Join(int milisecondsTimeout)
        {
            if (_thread != null)
                return _thread.Join(milisecondsTimeout);
            
            return false;
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="milisecondsTimeout">time to wait for the thread to exit in miliseconds</param>
        public void WaitForExit(int milisecondsTimeout)
        {
            if (_thread != null)
                _thread.Join(milisecondsTimeout);
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="timeout">time to wait for the thread to exit</param>
        /// <returns></returns>
        public bool Join(TimeSpan timeout)
        {
            if (_thread != null)
                return _thread.Join((int) timeout.TotalMilliseconds);
            return false;
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="timeout">time to wait for the thread to exit</param>
        /// <returns></returns>
        public bool WaitForExit(TimeSpan timeout)
        {
            return _thread.Join((int)timeout.TotalMilliseconds);
        }

        
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get 
            {
                try
                {
                    return _thread.Name;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ThreadId
        {
            get
            {
                return _threadIdSnapshot;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void  InternalDispose(bool isExplicitDispose)
        {
            if (-1 != _threadIdSnapshot)
                RegisterUnregisterThread(this, false);
        }

        private void ThreadFinalization()
        {
            _state = SafeThreadState.Stopped;
            _threadFinishedMutex.Set();

            //if (!_useThreadPool)
            //{
            //    // this is preparation for re-usable Thread
            //    _thread = null;
            //    //                Console.WriteLine("Thread finalization");
            //}

        }

        /// <summary>
        /// 
        /// </summary>
        public SafeThreadState State
        {
            get
            {
                lock (_startStopSync)
                    return _state;
            }
        }

        private const int DEFAULT_TIMEOUT_FRACTION = 100;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeToSleepInMs"></param>
        /// <param name="untilConditionFalse"></param>
        public static bool Sleep(int timeToSleepInMs, ref bool untilConditionFalse)
        {
            if (timeToSleepInMs <= 0)
                return false;

            int fractionOfTimeout = DEFAULT_TIMEOUT_FRACTION;

            if (timeToSleepInMs <= DEFAULT_TIMEOUT_FRACTION*5)
            {
                fractionOfTimeout = timeToSleepInMs/10;
                if (fractionOfTimeout == 0)
                    fractionOfTimeout = 1;
            }

            while (timeToSleepInMs > 0)
            {
                if (!untilConditionFalse)
                    return false;

                if (timeToSleepInMs >= fractionOfTimeout)
                    timeToSleepInMs -= fractionOfTimeout;
                else
                {
                    fractionOfTimeout = timeToSleepInMs;
                    timeToSleepInMs = 0;
                }

                Thread.Sleep(fractionOfTimeout);
            }

            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ThreadExtensions
    {
        /// <summary>
        /// block's until the SafeThread.Resume call
        /// ;
        /// doesn't do anything when the thread supplied is not 
        /// the SafeThread
        /// </summary>
        /// <param name="thread"></param>
        public static void WaitForResume(this Thread thread)
        {
            if (thread == null)
                return;

            var st = ASafeThreadBase.GetThreadById(thread.ManagedThreadId);
            if (st != null)
            {
                if (st.ThreadId == Thread.CurrentThread.ManagedThreadId)
                    st.WaitForResume();
                else
                    throw new InvalidOperationException("Thread.WaitForResume should be called only within the running SafeThread");
            }
        }
    }
}
