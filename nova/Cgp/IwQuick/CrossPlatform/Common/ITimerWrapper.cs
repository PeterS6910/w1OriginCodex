using Contal.IwQuick.Threads;

namespace Contal.IwQuick.CrossPlatform.Common
{
    /// <summary>
    /// Interface for system timer, mainly to allow moqs in testing
    /// </summary>
    public interface ITimerWrapper
    {
        /// <summary>
        /// Stops this timer
        /// </summary>
        void StopTimer();
    }

    /// <summary>
    /// Default class implements interface with IwQuick timer
    /// </summary>
    public class TimerWrapper : ITimerWrapper
    {
        private readonly ITimer _timer;

        public TimerWrapper(ITimer timer)
        {
            _timer = timer;
        }

        /// <summary>
        /// Stops this timer
        /// </summary>
        public void StopTimer()
        {
            _timer.StopTimer();
        }
    }
}
