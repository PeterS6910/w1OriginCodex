using System;
using System.Threading;
using System.Diagnostics;
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// abstract base for thread encapsulation classes
    /// </summary>
    public abstract partial class ASafeThreadBase
    {
        protected void PrepareThread(ThreadPriority priority, bool useThreadPool)
        {
            if (null != _thread)
                throw new InvalidOperationException();

            _useThreadPool = useThreadPool;
            _threadPriority = priority;


        }

        

        private volatile SafeThreadState _state = SafeThreadState.NotStarted;

        private readonly ManualResetEvent _suspendResumeMutex = new ManualResetEvent(true);
        
        

        /// <summary>
        /// USE WITH CAUTION, CAN CAUSE WHOLE PROCESS TO CRASH ; 
        /// works only if FailsafeAbortTimeout is higher than zero
        /// </summary>
        public bool FailsafeNativeThreadTermination { get; set; }

        

        protected volatile uint _systemThreadId;
        /// <summary>
        /// 
        /// </summary>
        public uint SystemThreadId
        {
            get
            {
                return _systemThreadId;
            }
        }

        /// <summary>
        /// main thread processing encapsulation 
        /// </summary>
        /// <param name="parameter">unused parameter</param>
        private void SafeThreadBody(object parameter)
        {
            try
            {
                _systemThreadId = DllKernel32.GetCurrentThreadId();
            }
            catch
            {
                // TODO : verify if this is a good idea
                _systemThreadId = (uint)Thread.CurrentThread.ManagedThreadId;
            }

            try
            {
                SpecificThreadBodyCall();
                ThreadFinalization();
                InvokeOnFinished();
            }
            catch (ThreadAbortException)
            {
                ThreadFinalization();
                InvokeOnFinished();
            }
            catch (Exception error)
            {
                ThreadFinalization();
                InvokeOnException(error);
            }
            finally
            {
                if (!_threadFinishedMutex.WaitOne(0, false))
                    DebugHelper.NOP(_systemThreadId);
            }
        }

        // intended for overriding the calling of the thread with specific parameters
        protected abstract void SpecificThreadBodyCall();

        private void ThreadFinalization()
        {
            _state = SafeThreadState.Stopped;
            _threadFinishedMutex.Set();

            if (!_useThreadPool)
            {
                _thread = null;
                //                Console.WriteLine("Thread finalization");
            }
        }

        /// <summary>
        /// raised, when an exception is thrown within the running thread
        /// </summary>
        public event DException2Void OnException;
        protected void InvokeOnException(Exception error)
        {
            //ThreadFinalization();

            if (null != OnException)
                try { OnException(error); }
                catch { }
        }

        /// <summary>
        /// raised, when the thread processing has finished
        /// </summary>
        public event DVoid2Void OnFinished;
        protected void InvokeOnFinished()
        {
            //ThreadFinalization();

            if (null != OnFinished)
                try { OnFinished(); }
                catch { }
        }

        /// <summary>
        /// wait for thread exit for infinite time
        /// </summary>
        public bool Join()
        {
            if (_state != SafeThreadState.Running)
                return (_state == SafeThreadState.Stopped);

            if (!_useThreadPool)
            {
                if (null != _thread)
                    try
                    {
                        _thread.Join();
                        return true;
                    }
                    catch (NullReferenceException)
                    {
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                
                return true;
            }
            
            return _threadFinishedMutex.WaitOne();
        }

        /// <summary>
        /// wait for thread exit for infinite time
        /// </summary>
        public bool WaitForExit()
        {
            return Join();
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="milisecondsTimeout">time to wait for the thread to exit in miliseconds</param>
        public bool Join(int milisecondsTimeout)
        {
            if (_state != SafeThreadState.Running)
                return (_state == SafeThreadState.Stopped);

            try
            {
                if (_thread.GetApartmentState() == ApartmentState.Unknown)
                    return true; // this state ensures behaviour during application finalization

                bool result = _threadFinishedMutex.WaitOne(milisecondsTimeout, false);
                
                //if (!result) // JUST FOR DEBUGING
                //    Console.WriteLine();

                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="milisecondsTimeout">time to wait for the thread to exit in miliseconds</param>
        public bool WaitForExit(int milisecondsTimeout)
        {
            return Join(milisecondsTimeout);
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="timeout">time to wait for the thread to exit</param>
        /// <returns></returns>
        public bool Join(TimeSpan timeout)
        {
            if (_state != SafeThreadState.Running)
                return (_state == SafeThreadState.Stopped);


            try
            {
                return _threadFinishedMutex.WaitOne(timeout, false);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// waits for thread to exit
        /// </summary>
        /// <param name="timeout">time to wait for the thread to exit</param>
        /// <returns></returns>
        public bool WaitForExit(TimeSpan timeout)
        {
            return Join(timeout);
        }

        protected ThreadPriority _threadPriority;

        private ApartmentState? _apartmentState = null;

        public void SetApartmentState(ApartmentState apartmentState)
        {
            if (_state != SafeThreadState.NotStarted)
                throw new InvalidOperationException("This option must be called before the thread is started");

            _apartmentState = apartmentState;
        }


    }
}
