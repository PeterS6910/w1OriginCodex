namespace Contal.IwQuick.CrossPlatform.Threads
{
    public sealed class ThreadPoolGetter
        : ASingleton<ThreadPoolGetter>
    {
        private const int DefaultPoolingThreadsCount = 20;

        private readonly ThreadPool _threadPool;

        private ThreadPoolGetter()
            : base(null)
        {
            _threadPool = new ThreadPool(DefaultPoolingThreadsCount);
        }

        public int PoolingThreadsCount
        {
            set { _threadPool.PooledThreadsCount = value; }
        }

        public static IThreadPool Get()
        {
            return Singleton._threadPool;
        }
    }
}
