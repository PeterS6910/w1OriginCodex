using System;
using System.Threading;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys.Microsoft;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{
    public abstract partial class ASafeThreadBase
    {

        protected const bool DefaultCreateSuspended = false;
        protected const bool DefaultUseThreadPool = false;
        protected const ThreadPriority DefaultThreadPriority = ThreadPriority.Normal;

        protected volatile Thread _thread;

        /// <summary>
        /// encapsulated thread instance; 
        /// in case of ThreadPool mechanism, this is null until not started
        /// </summary>
        public Thread Thread
        {
            get { return _thread; }
        }

        private volatile bool _abortIndicator = false;
        /// <summary>
        /// returns, whether the thread was already aborted
        /// </summary>
        public bool IsAborted
        {
            get { return _abortIndicator; }
        }

        private volatile int _failsafeAbortTimeout = -1;

        /// <summary>
        /// if equal or less than zero, timeout is not used
        /// </summary>
        public int FailsafeAbortTimeout
        {
            get
            {
                return _failsafeAbortTimeout;
            }
            set
            {
                _failsafeAbortTimeout = value;
            }

        }

        private volatile object _startStopSync = new object();

        /// <summary>
        /// 
        /// </summary>
        public bool IsStarted
        {
            get
            {
                lock (_startStopSync)
                {
                    return _state == SafeThreadState.Running;
                }
            }
        }

        public delegate void DAbortingLambda(Thread currentThread, int milisecondsForAbort);

        /// <summary>
        /// stops/terminates the thread by invoking ThreadAbortException
        /// </summary>
        /// <param name="milisecondsForAbort">timeout in miliseconds for the thread to stop, then abort;
        /// if 0, abort is not used</param>
        public SafeThreadStopResolution Stop(
            int milisecondsForAbort)
        {
            return Stop(milisecondsForAbort, null);
        }

        /// <summary>
        /// stops/terminates the thread by invoking ThreadAbortException
        /// </summary>
        /// <param name="milisecondsForAbort">timeout in miliseconds for the thread to stop, then abort;
        /// if 0, abort is not used</param>
        /// <param name="abortingLambda"></param>
        public SafeThreadStopResolution Stop(
            int milisecondsForAbort,
            DAbortingLambda abortingLambda)
        {
            lock (_startStopSync)
            {
                switch (_state)
                {
                    case SafeThreadState.NotStarted:
                        return SafeThreadStopResolution.NotStarted;

                    case SafeThreadState.Suspended:
                    case SafeThreadState.Running:
                        {
                            _abortIndicator = true;

                            if (null != abortingLambda)
                                // TODO : catched for now 
                                try
                                {
                                    abortingLambda(Thread.CurrentThread, milisecondsForAbort);
                                }
                                catch
                                {

                                }

                            if (_state == SafeThreadState.Running &&
                                milisecondsForAbort > 0)
                            {
                                if (_thread != null && !_thread.Join(milisecondsForAbort))
                                    return Abort();

                                return SafeThreadStopResolution.StoppedGracefuly;
                            }

                            return Abort();
                        }
                    //case SafeThreadState.Stopped:
                    default:
                        return SafeThreadStopResolution.StoppedGracefuly;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool Start()
        {
            if (_state != SafeThreadState.Running)
                lock (_startStopSync)
                {
                    if (_state != SafeThreadState.Running)
                        try
                        {
#if COMPACT_FRAMEWORK
                            if (_state != SafeThreadState.Suspended)
                                _state = SafeThreadState.Running;
#else
                            if (_state == SafeThreadState.Stopped)
                                throw new InvalidOperationException("Thread cannot be REstarted");

                            // this bool should precede the actual thread creation to avoid unsynchronized behavior
                            _state = SafeThreadState.Running;
#endif

                            _abortIndicator = false;
                            _threadFinishedMutex.Reset();

                            if (_useThreadPool)
                            {
                                bool success = 
#if COMPACT_FRAMEWORK
                                    ThreadPool.QueueUserWorkItem(state => SafeThreadBody());
#else
                                    ThreadPool.QueueUserWorkItem(SafeThreadBody);
#endif
                                if (!success)
                                {
                                    _state = SafeThreadState.NotStarted;
                                    _threadFinishedMutex.Set();
                                }

                                return success;
                            }

#if ! COMPACT_FRAMEWORK
                            if (null == _thread)
                            {
                                _thread = new Thread(SafeThreadBody)
                                {
                                    Priority = _threadPriority,
                                    IsBackground = true
                                };

                                if (_apartmentState != null)
                                    _thread.SetApartmentState(_apartmentState.Value);
                            }
#endif

                            _thread.Start();
                            return true;
                        }
                        catch
                        {
                            _state = SafeThreadState.NotStarted;
                            _threadFinishedMutex.Set();
                            return false;
                        }

                    // TODO : THIS IS IN DISPUTE; Start has been called by other thread already, and started the thread
                    return true;
                }

            return false;
        }

        private readonly ManualResetEvent _threadFinishedMutex = new ManualResetEvent(false);

        protected bool _useThreadPool = false;

        /// <summary>
        /// informs, whether thread was created by ThreadPool
        /// </summary>
        public bool UseThreadPool
        {
            get { return _useThreadPool; }
        }

        /// <summary>
        /// locks the suspend mutex, therefore locks all WaitForResume calls
        /// </summary>
        public bool Suspend()
        {
            lock (_startStopSync)
            {
                if (_state == SafeThreadState.Running)
                {
                    _state = SafeThreadState.Suspended;
                    return _suspendResumeMutex.Reset();
                }
                return false;
            }
        }

        /// <summary>
        /// waits/blocks for resume mutex to be unlocked; 
        /// can be used for algorithmic suspend process except the direct suspend over thread
        /// </summary>
        public void WaitForResume()
        {
            _suspendResumeMutex.WaitOne();
        }


        /// <summary>
        /// stops/terminates the thread by invoking ThreadAbortException
        /// </summary>
        public SafeThreadStopResolution Abort()
        {
            if (!_useThreadPool)
            {
                if (_thread != null)
                {
                    var ret = SafeThreadStopResolution.StoppedForcefuly;
                    try
                    {
                        //uint tId = SystemThreadId;

                        //                        Console.WriteLine("Aborting ...");
                        _thread.Abort(InternalThreadAbortKey.Singleton);


                        if (_failsafeAbortTimeout > 0)
                        {
                            // waits for finishing by the the ThreadFinalization call
                            bool abortTimedOut = !_threadFinishedMutex.WaitOne(_failsafeAbortTimeout, false);

#if !COMPACT_FRAMEWORK
                            if (abortTimedOut)
                            {
                                if (FailsafeNativeThreadTermination)
                                    try
                                    {
                                        // try native method
                                        var threadHandle =
                                            DllKernel32.OpenThread(DllKernel32.ThreadSecurityRights.THREAD_ALL_ACCESS,
                                                false,
                                                _systemThreadId);

                                        if (threadHandle != IntPtr.Zero)
                                        {
                                            // dwExitCode == ERROR_OPERATION_ABORTED
                                            DllKernel32.TerminateThread(threadHandle, 0x3E3);
                                        }

                                        abortTimedOut = !_threadFinishedMutex.WaitOne(_failsafeAbortTimeout, false);
                                    }
                                    catch
                                    {

                                    }
                            }
#endif

                            // revalidate due possibility of second waiting after native thread termination
                            if (abortTimedOut)
                                ret = SafeThreadStopResolution.ForcefulStoppedTimedOut;
                        }
                        else
                        {
                            // waits for finishing by the the ThreadFinalization call
                            _threadFinishedMutex.WaitOne();
                        }

                        //DebugHelper.NOP(tId);
                    }
                    catch (NullReferenceException)
                    {
                        return SafeThreadStopResolution.StoppedGracefuly;
                    }
                    catch
                    {
                        ret = SafeThreadStopResolution.StopFailed;
                    }
                    finally
                    {
                        _thread = null;
                    }

                    return ret;
                }

                return SafeThreadStopResolution.StoppedGracefuly;
            }

            // "There is NO way to abort the thread in threadpool from the outside.\n" +
            //                    "Choose different way to stop this kind of thread");
            DebugHelper.TryBreak("ASafeThreadBase.Abort aborting ThreadPool thread is not allowed");

            return SafeThreadStopResolution.AbortNotAllowed;
        }

        /// <summary>
        /// unlocks the suspend mutex, therefore unlocks all WaitForResume calls
        /// </summary>
        public bool Resume()
        {
            lock (_startStopSync)
            {
                if (_state == SafeThreadState.Suspended)
                {
                    _state = SafeThreadState.Running;
                    return _suspendResumeMutex.Set();
                }

                return false;
            }

        }

        private const int PreallocatedThreadStructsCount = 32;

        private static readonly SyncDictionary<int, WeakReference> _threads =
            new SyncDictionary<int, WeakReference>(PreallocatedThreadStructsCount);

        // ReSharper disable once ConvertToConstant.Local - in CF used
        private volatile int _threadIdSnapshot = -1;


        // ReSharper disable once UnusedMember.Local
        private static void RegisterUnregisterThread([NotNull] ASafeThreadBase safeThread, bool register)
        {
            try
            {
                if (register)
                {
                    // sync dictionary ensures the locking
                    _threads[safeThread._threadIdSnapshot] = new WeakReference(safeThread);
                }
                else
                {
                    _threads.Remove(safeThread._threadIdSnapshot);
                }
            }
            catch
            {
            }
        }

    }
}
