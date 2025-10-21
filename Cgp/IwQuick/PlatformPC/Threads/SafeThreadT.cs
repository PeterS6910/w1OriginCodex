using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// thread encapsulation class
    /// </summary>
    /// <typeparam name="T">type of </typeparam>
    public class SafeThread<T> : ASafeThreadBase
    {
        private readonly Action<T> _threadBody;
        /// <summary>
        /// generic parameter to be passed to the thread
        /// </summary>
        private T _parameter;

        /// <summary>
        /// create parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <param name="useThreadPool">if true, ThreadPool.QueueUserWorkItem is used to create the thread</param>
        /// <param name="priority">thread priority</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action<T> threadBody, ThreadPriority priority, bool useThreadPool)
        {
            Validator.CheckForNull(threadBody,"threadBody");

            _threadBody = threadBody;

            PrepareThread(priority, useThreadPool);
        }

        /// <summary>
        /// create parametrized thread encapsulation instance; 
        /// direct thread, not created by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action<T> threadBody)
            :this (threadBody, ThreadPriority.Normal, false)
        {
        }

        /// <summary>
        /// starts the thread with a parameter; 
        /// returns true, if thread start succeeded
        /// </summary>
        /// <param name="parameter">parameter to be passed to the thread</param>
        public bool Start(T parameter)
        {
            _parameter = parameter;

// ReSharper disable once RedundantBaseQualifier
            return base.Start();
        }

        /// <summary>
        /// processing body encapsulating the thread processing
        /// </summary>
        protected override void  SpecificThreadBodyCall()
        {
 	        _threadBody(_parameter);
        }
        
        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter">optional parameter passed to the thread</param>
        public static SafeThread<TParam> StartThread<TParam>([NotNull] Action<TParam> threadBody, TParam parameter)
        {
            try
            {
                SafeThread<TParam> st = new SafeThread<TParam>(threadBody);
                if (st.Start(parameter))
                    return st;
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter">optional parameter passed to the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="useThreadPool">if true, ThreadPool.QueueUserWorkItem would be used</param>
        public static SafeThread<TParam> StartThread<TParam>([NotNull] Action<TParam> threadBody, TParam parameter, ThreadPriority priority,bool useThreadPool)
        {
            try
            {
                SafeThread<TParam> st = new SafeThread<TParam>(threadBody, priority, useThreadPool);
                if (st.Start(parameter))
                    return st;
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
