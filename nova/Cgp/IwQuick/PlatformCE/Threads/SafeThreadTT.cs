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
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread(
            [NotNull] Action<T1,T2> threadBody, 
            ThreadPriority priority, 
            bool useThreadPool,
            string name) 
            : this(threadBody, priority, useThreadPool, name, DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// create parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <param name="useThreadPool">if true, ThreadPool is used to manage the thread</param>
        /// <param name="priority">thread priority</param>
        /// <param name="name"></param>
        /// <param name="createSuspended"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        private SafeThread(
            [NotNull] Action<T1,T2> threadBody, 
            ThreadPriority priority, 
            bool useThreadPool,
            string name,
            bool createSuspended)
        {
            if (null == threadBody)
                throw new ArgumentNullException("threadBody");

            if (string.IsNullOrEmpty(name))
                try
                {
                    name = "SafeThread<" + typeof(T1) + "," + typeof(T2) + "> " + threadBody.GetTargetDescription();
                }
                catch
                {
                }

            PrepareThread(priority, useThreadPool, name, createSuspended);

            _threadBody = threadBody;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadBody"></param>
        /// <param name="priority"></param>
        /// <param name="useThreadPool"></param>
        public SafeThread([NotNull] Action<T1, T2> threadBody, ThreadPriority priority, bool useThreadPool)
            :this(threadBody,priority,useThreadPool,null,DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// create parametrized thread encapsulation instance; 
        /// direct thread, not created by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action<T1,T2> threadBody)
            : this(threadBody, DefaultThreadPriority, DefaultUseThreadPool, null, DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadBody"></param>
        /// <param name="name"></param>
        public SafeThread([NotNull] Action<T1, T2> threadBody, string name)
            : this(threadBody, DefaultThreadPriority, DefaultUseThreadPool, name, DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// starts the thread with a parameter; 
        /// returns true, if thread start succeeded
        /// </summary>
        public bool Start(T1 parameter1,T2 parameter2)
        {
            _parameter1 = parameter1;
            _parameter2 = parameter2;

// ReSharper disable once RedundantBaseQualifier
            return base.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void SpecificThreadBodyCall()
        {
            _threadBody(_parameter1, _parameter2);
        }

        ///// <summary>
        ///// processing body encapsulating the thread processing
        ///// </summary>
        ///// <param name="parameter">parameter to be passed to the thread processing</param>
        //protected override void  EncapsulatedThreadBody(object parameter)
        //{
        //    try
        //    {
        //        _threadBody(_parameter1,_parameter2);
        //        InvokeOnFinished();
        //    }
        //    catch (ThreadAbortException)
        //    {
        //        InvokeOnFinished();
        //    }
        //    catch(Exception error)
        //    {
        //        InvokeOnException(error);
        //    }

        //    base.EncapsulatedThreadBody(parameter);

        //}

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="name"></param>
        public static SafeThread<TParam1, TParam2> StartThread<TParam1, TParam2>(
            [NotNull] Action<TParam1, TParam2> threadBody, 
            TParam1 parameter1,
            TParam2 parameter2, 
            string name)
        {
            try
            {
                var st = new SafeThread<TParam1,TParam2>(threadBody,name);
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
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public static SafeThread<TParam1, TParam2> StartThread<TParam1, TParam2>(
            [NotNull] Action<TParam1, TParam2> threadBody,
            TParam1 parameter1, 
            TParam2 parameter2)
        {
            return StartThread(threadBody, parameter1, parameter2, null);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter2"></param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="parameter1"></param>
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        public static SafeThread<TParam1,TParam2> StartThread<TParam1,TParam2>(
            [NotNull] Action<TParam1, TParam2> threadBody,
            TParam1 parameter1,
            TParam2 parameter2, 
            ThreadPriority priority,
            bool useThreadPool,
            string name)
        {
            return StartThread(threadBody, parameter1, parameter2, priority, useThreadPool, name, DefaultCreateSuspended);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter2"></param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="parameter1"></param>
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        /// <param name="createSuspended"></param>
        public static SafeThread<TParam1,TParam2> StartThread<TParam1,TParam2>(
            [NotNull] Action<TParam1, TParam2> threadBody,
            TParam1 parameter1,
            TParam2 parameter2, 
            ThreadPriority priority,
            bool useThreadPool,
            string name,
            bool createSuspended)
        {
            try
            {
                var st = new SafeThread<TParam1,TParam2>(threadBody, priority, useThreadPool, name, createSuspended);
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
        /// 
        /// </summary>
        /// <typeparam name="TParam1"></typeparam>
        /// <typeparam name="TParam2"></typeparam>
        /// <param name="threadBody"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="priority"></param>
        /// <param name="useThreadPool"></param>
        /// <returns></returns>
        public static SafeThread<TParam1, TParam2> StartThread<TParam1, TParam2>(
            [NotNull] Action<TParam1, TParam2> threadBody, 
            TParam1 parameter1,
            TParam2 parameter2, 
            ThreadPriority priority,
            bool useThreadPool)
        {
            return StartThread(threadBody, parameter1, parameter2, priority,useThreadPool, null, DefaultCreateSuspended);
        }
    }
}
