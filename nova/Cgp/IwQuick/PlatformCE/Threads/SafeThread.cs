using System;
using System.Threading;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// thread encapsulation class
    /// </summary>
    public class SafeThread: ASafeThreadBase
    {
        private readonly Action _threadBody;

        /// <summary>
        /// creates non-parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="useThreadPool">if true, the ThreadPool is used to manage the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread(
            [NotNull] Action threadBody,
            ThreadPriority priority,
            bool useThreadPool,
            string name) 
            : this(threadBody, priority, useThreadPool, name, DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// creates non-parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="useThreadPool">if true, the ThreadPool is used to manage the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="name"></param>
        /// <param name="createSuspended">should be used by static StartThread only</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        private SafeThread(
            [NotNull] Action threadBody,
            ThreadPriority priority,
            bool useThreadPool,
            string name,
            bool createSuspended)
        {
            Validator.CheckForNull(threadBody,"threadBody");

            if (string.IsNullOrEmpty(name))
                try
                {
                    name = "SafeThread<void> " + threadBody.GetTargetDescription();
                }
                catch
                {
                    
                }
        
            PrepareThread(priority, useThreadPool, name,createSuspended);

            _threadBody = threadBody; 
        }

        /*
        /// <summary>
        /// creates non-parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="useThreadPool">if true, the ThreadPool is used to manage the thread</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        private SafeThread(
            DVoid2Void threadBody,
            ThreadPriority priority,
            bool useThreadPool,
            string name)
            : this(threadBody, priority, useThreadPool, name, false)
        {
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadBody"></param>
        /// <param name="priority"></param>
        /// <param name="useThreadPool"></param>
        public SafeThread([NotNull] Action threadBody, ThreadPriority priority, bool useThreadPool)
            :this(threadBody,priority,useThreadPool,null,DefaultCreateSuspended)
        {

        }

        /// <summary>
        /// creates non-parametrized thread encapsulation instance; 
        /// directly, not by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action threadBody)
            :this(threadBody,DefaultThreadPriority,DefaultUseThreadPool,null,DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// creates non-parametrized thread encapsulation instance; 
        /// directly, not by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action threadBody,string name)
            : this(threadBody, DefaultThreadPriority, DefaultUseThreadPool, name, DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// starts the thread; 
        /// returns true if successful
        /// </summary>
        public new bool Start()
        {
            return base.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void SpecificThreadBodyCall()
        {
            _threadBody();
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start failed
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="name"></param>
        public static SafeThread StartThread([NotNull] Action threadBody, string name)
        {
            try
            {
                var st = new SafeThread(threadBody,name);
                if (st.Start())
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
        /// returns instance of the thread encapsulation or null if start failed
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        public static SafeThread StartThread([NotNull] Action threadBody)
        {
            return StartThread(threadBody, null);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="priority">priority of the direct thread</param>
        public static SafeThread StartThread([NotNull] Action threadBody,ThreadPriority priority)
        {
            try
            {
                var st = new SafeThread(threadBody, priority, false);
                if (st.Start())
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
        /// returns instance of the thread encapsulation
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="priority">priority of the direct thread</param>
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        /// <param name="createSuspended"></param>
        public static SafeThread StartThread(
            [NotNull] Action threadBody, 
            ThreadPriority priority, 
            bool useThreadPool,
            string name,
            bool createSuspended)
        {
            try
            {
                var st = new SafeThread(threadBody, priority, useThreadPool,name,createSuspended);
                if (st.Start())
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
        /// returns instance of the thread encapsulation
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="priority">priority of the direct thread</param>
        /// <param name="useThreadPool"></param>
        /// <param name="name"></param>
        public static SafeThread StartThread(
            [NotNull] Action threadBody,
            ThreadPriority priority,
            bool useThreadPool,
            string name)
        {
            return StartThread(threadBody, priority, useThreadPool, name, DefaultCreateSuspended);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="priority">priority of the direct thread</param>
        /// <param name="useThreadPool"></param>
        public static SafeThread StartThread([NotNull] Action threadBody, ThreadPriority priority, bool useThreadPool)
        {
            return StartThread(threadBody, priority, useThreadPool, null,DefaultCreateSuspended);
        }
    }
}
