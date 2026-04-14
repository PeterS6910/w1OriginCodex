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
    /// <typeparam name="T3">type of parameter 3</typeparam>
    public class SafeThread<T1,T2,T3> : ASafeThreadBase
    {
        private readonly Action<T1,T2,T3> _threadBody;

        /// <summary>
        /// generic parameter 1 to be passed to the thread
        /// </summary>
        private T1 _param1;

        /// <summary>
        /// generic parameter 2 to be passed to the thread
        /// </summary>
        private T2 _param2;

        /// <summary>
        /// generic parameter 3 to be passed to the thread
        /// </summary>
        private T3 _param3;

        /// <summary>
        /// create parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <param name="useThreadPool">if true, ThreadPool is used to manage the thread</param>
        /// <param name="priority">thread priority</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread(
            [NotNull] Action<T1,T2,T3> threadBody, 
            ThreadPriority priority, 
            bool useThreadPool)
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
        public SafeThread([NotNull] Action<T1,T2,T3> threadBody)
            :this (threadBody, ThreadPriority.Normal, false)
        {
        }

        /// <summary>
        /// starts the thread with a parameter; 
        /// returns true, if thread start succeeded
        /// </summary>
        /// <param name="parameter1">first parameter to be passed to the thread</param>
        /// <param name="parameter2">second parameter to be passed to the thread</param>
        /// <param name="parameter3">third parameter to be passed to the thread</param>
        public bool Start(T1 parameter1,T2 parameter2,T3 parameter3)
        {
            _param1 = parameter1;
            _param2 = parameter2;
            _param3 = parameter3;

// ReSharper disable once RedundantBaseQualifier
            return base.Start();
        }

        /// <summary>
        /// processing body encapsulating the thread processing
        /// </summary>
        protected override void SpecificThreadBodyCall()
        {
            _threadBody(_param1, _param2, _param3);
        }
        



        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter1">first parameter to be passed to the thread</param>
        /// <param name="parameter2">second parameter to be passed to the thread</param>
        /// <param name="parameter3">third parameter to be passed to the thread</param>
        public static SafeThread<TParam1, TParam2, TParam3> StartThread<TParam1, TParam2, TParam3>(
            [NotNull] Action<TParam1, TParam2, TParam3> threadBody, 
            TParam1 parameter1, 
            TParam2 parameter2,
            TParam3 parameter3)
        {
            try
            {
                SafeThread<TParam1,TParam2,TParam3> st = new SafeThread<TParam1,TParam2,TParam3>(threadBody);
                if (st.Start(parameter1,parameter2,parameter3))
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
        /// <param name="parameter3">third parameter to be passed to the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="useThreadPool"></param>
        public static SafeThread<TParam1,TParam2,TParam3> StartThread<TParam1,TParam2,TParam3>(
            [NotNull] Action<TParam1,TParam2,TParam3> threadBody, 
            TParam1 parameter1,
            TParam2 parameter2, 
            TParam3 parameter3,
            ThreadPriority priority, 
            bool useThreadPool)
        {
            try
            {
                SafeThread<TParam1,TParam2,TParam3> st = new SafeThread<TParam1,TParam2,TParam3>(threadBody, priority, useThreadPool);
                if (st.Start(parameter1,parameter2,parameter3))
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
