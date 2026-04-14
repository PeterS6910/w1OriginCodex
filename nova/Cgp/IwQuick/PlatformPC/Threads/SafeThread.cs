using System;
using System.Threading;
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
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action threadBody,ThreadPriority priority,bool useThreadPool)
        {
            Validator.CheckForNull(threadBody,"threadBody");
            
            _threadBody = threadBody;

            PrepareThread(priority, useThreadPool);
        }

        /// <summary>
        /// creates non-parametrized thread encapsulation instance; 
        /// directly, not by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThread([NotNull] Action threadBody)
            :this(threadBody,ThreadPriority.Normal, false)
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
        public static SafeThread StartThread([NotNull] Action threadBody)
        {
            try
            {
                SafeThread st = new SafeThread(threadBody);
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
        public static SafeThread StartThread([NotNull] Action threadBody, ThreadPriority priority)
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
    }
}
