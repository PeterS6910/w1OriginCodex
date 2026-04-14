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
        /// <param name="useThreadPool">if true, ThreadPool is used to manage the thread</param>
        /// <param name="priority">thread priority</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread(
            [NotNull] Action<T> threadBody,
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
            [NotNull] Action<T> threadBody,
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
                    name = "SafeThread<" + typeof(T) + "> " + threadBody.GetTargetDescription();
                }
                catch
                {
                }

            PrepareThread(priority, useThreadPool, name, createSuspended);

            _threadBody = threadBody;
        }

        
        //public SafeThread([NotNull] Action<T> threadBody, ThreadPriority priority, bool useThreadPool, string name)
        //{
            
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadBody"></param>
        /// <param name="priority"></param>
        /// <param name="useThreadPool"></param>
        public SafeThread([NotNull] Action<T> threadBody, ThreadPriority priority, bool useThreadPool)
            :this(threadBody,priority,useThreadPool,null,DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// create parametrized thread encapsulation instance; 
        /// direct thread, not created by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action<T> threadBody)
            :this (threadBody, DefaultThreadPriority, DefaultUseThreadPool,null,DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// create parametrized thread encapsulation instance; 
        /// direct thread, not created by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to process</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action<T> threadBody,string name)
            : this(threadBody, DefaultThreadPriority, DefaultUseThreadPool, name, DefaultCreateSuspended)
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
        /// 
        /// </summary>
        protected override void SpecificThreadBodyCall()
        {
            _threadBody(_parameter);
        }

        ///// <summary>
        ///// processing body encapsulating the thread processing
        ///// </summary>
        ///// <param name="parameter">parameter to be passed to the thread processing</param>
        //protected override void  EncapsulatedThreadBody(object parameter)
        //{
        //    try
        //    {
                
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
        /// <param name="parameter">optional parameter passed to the thread</param>
        /// <param name="name"></param>
        public static SafeThread<TParam> StartThread<TParam>([NotNull] Action<TParam> threadBody, TParam parameter, string name)
        {
            try
            {
                var st = new SafeThread<TParam>(threadBody,name);
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
        public static SafeThread<TParam> StartThread<TParam>([NotNull] Action<TParam> threadBody, TParam parameter)
        {
            return StartThread(threadBody, parameter, null);
        }

        

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter">optional parameter passed to the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        /// <param name="createSuspended"></param>
        public static SafeThread<TParam> StartThread<TParam>(
            [NotNull] Action<TParam> threadBody, 
            TParam parameter, 
            ThreadPriority priority,
            bool useThreadPool,
            string name,bool 
            createSuspended)
        {
            try
            {
                SafeThread<TParam> st = new SafeThread<TParam>(threadBody, priority, useThreadPool, name,createSuspended);
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
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        public static SafeThread<TParam> StartThread<TParam>(
            [NotNull] Action<TParam> threadBody,
            TParam parameter,
            ThreadPriority priority,
            bool useThreadPool,
            string name)
        {
            return StartThread(threadBody, parameter, priority, useThreadPool, name, DefaultCreateSuspended);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start was not successful
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="parameter">optional parameter passed to the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="useThreadPool"></param>
        public static SafeThread<TParam> StartThread<TParam>(
            [NotNull] Action<TParam> threadBody, 
            TParam parameter, 
            ThreadPriority priority, 
            bool useThreadPool)
        {
            return StartThread(threadBody, parameter, priority,useThreadPool, null, DefaultCreateSuspended);
        }
    }
}
