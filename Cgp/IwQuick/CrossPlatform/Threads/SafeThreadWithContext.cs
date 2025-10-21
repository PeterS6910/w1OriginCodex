using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// thread encapsulation class
    /// </summary>
    public class SafeThreadWithContext: ASafeThreadBase
    {
        private readonly DThreadBodyWithContext _threadBody;

        /// <summary>
        /// creates non-parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThreadWithContext(
            [NotNull] DThreadBodyWithContext threadBody,
            ThreadPriority priority,
            string name) 
            : this(threadBody, priority, null,name, DefaultCreateSuspended)
        {
        }


        /// <summary>
        /// creates non-parametrized thread encapsulation instance
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="priority">priority of the thread</param>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="createSuspended">should be used by static StartThread only</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThreadWithContext(
            [NotNull] DThreadBodyWithContext threadBody,
            ThreadPriority priority,
            [CanBeNull] ISafeThreadContext context,
            string name,
            bool createSuspended)
        {
            Validator.CheckForNull(threadBody,"threadBody");

            _currentThreadContext = context ?? DefaultSafeThreadContextFactory.Singleton.Create();

            
#if COMPACT_FRAMEWORK
            if (string.IsNullOrEmpty(name))
                name = "SafeThread<SafeThreadContext>";

            PrepareThread(priority, DefaultUseThreadPool,name,createSuspended);
#else
            PrepareThread(priority, DefaultUseThreadPool);
#endif

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

        public delegate void DThreadBodyWithContext(ISafeThreadContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadBody"></param>
        /// <param name="priority"></param>
        public SafeThreadWithContext([NotNull] DThreadBodyWithContext threadBody, ThreadPriority priority)
            :this(threadBody,priority,null,null,DefaultCreateSuspended)
        {

        }

        /// <summary>
        /// creates non-parametrized thread encapsulation instance; 
        /// directly, not by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThreadWithContext([NotNull] DThreadBodyWithContext threadBody)
            :this(threadBody,DefaultThreadPriority,null,null,DefaultCreateSuspended)
        {
        }

        /// <summary>
        /// creates non-parametrized thread encapsulation instance; 
        /// directly, not by ThreadPool
        /// </summary>
        /// <param name="threadBody">method to use as processing core</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">if the delegate is null</exception>
        public SafeThreadWithContext([NotNull] DThreadBodyWithContext threadBody, string name)
            : this(threadBody, DefaultThreadPriority, null,name, DefaultCreateSuspended)
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

        private volatile ISafeThreadContext _currentThreadContext;

        /// <summary>
        /// 
        /// </summary>
        public ISafeThreadContext Context
        {
            get { return _currentThreadContext; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void SpecificThreadBodyCall()
        {
            
            _threadBody(_currentThreadContext);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation or null if start failed
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="name"></param>
        public static SafeThreadWithContext StartThread([NotNull] DThreadBodyWithContext threadBody, string name)
        {
            try
            {
                var st = new SafeThreadWithContext(threadBody,name);
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
        public static SafeThreadWithContext StartThread([NotNull] DThreadBodyWithContext threadBody)
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
        /// <param name="name"></param>
        /// <param name="createSuspended"></param>
        public static SafeThreadWithContext StartThread(
            [NotNull] DThreadBodyWithContext threadBody, 
            ThreadPriority priority, 
            string name,
            bool createSuspended)
        {
            try
            {
                var st = new SafeThreadWithContext(threadBody, priority, null,name,createSuspended);
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
        public static SafeThreadWithContext StartThread(
            [NotNull] DThreadBodyWithContext threadBody,
            ThreadPriority priority,
            bool useThreadPool,
            string name)
        {
            return StartThread(threadBody, priority, name, DefaultCreateSuspended);
        }

        /// <summary>
        /// prepares the encapsulation instance and starts the thread; 
        /// returns instance of the thread encapsulation
        /// </summary>
        /// <param name="threadBody">method for thread processing</param>
        /// <param name="priority">priority of the direct thread</param>
        /// <param name="useThreadPool"></param>
        public static SafeThreadWithContext StartThread(
            [NotNull] DThreadBodyWithContext threadBody, 
            ThreadPriority priority, 
            bool useThreadPool)
        {
            return StartThread(threadBody, priority, null,DefaultCreateSuspended);
        }

        
    }
}
