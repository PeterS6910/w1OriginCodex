namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// 
    /// </summary>
    public enum SafeThreadStopResolution
    {
        /// <summary>
        /// 
        /// </summary>
        NotStarted = -3,
        /// <summary>
        /// 
        /// </summary>
        AbortNotAllowed = -2,
        /// <summary>
        /// 
        /// </summary>
        DidNotStopYet = -1,
        /// <summary>
        /// 
        /// </summary>
        StopFailed = 0,
        /// <summary>
        /// 
        /// </summary>
        StoppedGracefuly = 1,
        /// <summary>
        /// 
        /// </summary>
        StoppedForcefuly = 2,
        /// <summary>
        /// 
        /// </summary>
        ForcefulStoppedTimedOut = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SafeThreadState
    {
        /// <summary>
        /// 
        /// </summary>
        NotStarted = -2,
        /// <summary>
        /// 
        /// </summary>
        Suspended = -1,
        /// <summary>
        /// 
        /// </summary>
        Running = 1,
        /// <summary>
        /// 
        /// </summary>
        Stopped = 0
    }
}
