using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// thread encapsulation class
    /// </summary>
    /// <typeparam name="T1">type of parameter 1</typeparam>
    /// <typeparam name="T2">type of parameter 2</typeparam>
    public class SafeThread<T1,T2> : ASafeThreadBase
    {
        private readonly Action<T1,T2> _threadBody;

        /// <summary>
        /// generic parameter 1 to be passed to the thread
        /// </summary>
        private T1 _parameter1;

        /// <summary>
        /// generic parameter 1 to be passed to the thread
        /// </summary>
        private T2 _parameter2;

        /// <summary>
        /// create parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <param name="useThreadPool">if true, ThreadPool is used to manage the thread</param>
        /// <param name="priority">thread priority</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action<T1,T2> threadBody, ThreadPriority priority, bool useThreadPool)
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
        public SafeThread([NotNull] Action<T1,T2> threadBody)
            :this (threadBody, ThreadPriority.Normal, false)
        {
        }

        /// <summary>
        /// starts the thread with a parameter; 
        /// returns true, if thread start succeeded
        /// </summary>
        /// <param name="parameter1">first parameter to be passed to the thread</param>
        /// <param name="parameter2">second parameter to be passed to the thread</param>
        public bool Start(T1 parameter1,T2 parameter2)
        {
            _parameter1 = parameter1;
            _parameter2 = parameter2;

// ReSharper disable once RedundantBaseQualifier
            return base.Start();
        }

        /// <summary>
        /// processing body encapsulating the thread processing
        /// </summary>
        protected override void SpecificThreadBodyCall()
        {
            _threadBody(_parameter1, _parameter2);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">delegate for the thread body</param>
        /// <param name="parameter1">first parameter to be passed to the thread</param>
        /// <param name="parameter2">second parameter to be passed to the thread</param>
        public static SafeThread<TParam1, TParam2> StartThread<TParam1, TParam2>([NotNull] Action<TParam1, TParam2> threadBody, TParam1 parameter1,TParam2 parameter2)
        {
            try
            {
                SafeThread<TParam1,TParam2> st = new SafeThread<TParam1,TParam2>(threadBody);
                if (st.Start(parameter1,parameter2))
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
        /// <param name="parameter1">first parameter to be passed to the thread</param>
        /// <param name="parameter2">second parameter to be passed to the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="useThreadPool"></param>
        public static SafeThread<TParam1,TParam2> StartThread<TParam1,TParam2>([NotNull] Action<TParam1,TParam2> threadBody, TParam1 parameter1,TParam2 parameter2, ThreadPriority priority,bool useThreadPool)
        {
            try
            {
                SafeThread<TParam1,TParam2> st = new SafeThread<TParam1,TParam2>(threadBody, priority, useThreadPool);
                if (st.Start(parameter1,parameter2))
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
