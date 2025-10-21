using System;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.CrossPlatform.Common
{
    public interface ITimeoutProvider
    {
        /// <summary>
        /// Starts timeout
        /// </summary>
        /// <param name="timeoutMs">Timeout value</param>
        /// <param name="data">Data to be passed to callback</param>
        /// <param name="onTimeoutCallback">Callback that is raised when timeout occurs</param>
        ITimerWrapper StartTimeout(
            long timeoutMs,
            object data,
            Func<ITimerArgs, bool> onTimeoutCallback);
    }

    /// <summary>
    /// Default class implements interface with IwQuick timer
    /// </summary>
    public class TimeoutProvider : ITimeoutProvider
    {
        public ITimerWrapper StartTimeout(
            long timeoutMs, 
            object data,
            Func<ITimerArgs, bool> onTimeoutCallback)
        {
            return new TimerWrapper(
                TimerManager.Static.StartTimeout(
                    timeoutMs,
                    data,
                    carrier => onTimeoutCallback(new TimerArgs(carrier))));
        }
    }
}
