namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// 
    /// </summary>
    public class InternalThreadAbortKey
    {
        /// <summary>
        /// private constructor, locks ability to instantiate without Singleton call
        /// </summary>
        private InternalThreadAbortKey()
        {
        }

        private static volatile InternalThreadAbortKey _singleton = null;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// allows only IwQuick classes to use this instance
        /// </summary>
        internal static InternalThreadAbortKey Singleton 
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new InternalThreadAbortKey();
                    }

                return _singleton;
            }
        }
    }
}
