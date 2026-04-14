using Contal.IwQuick.Threads;

namespace Contal.IwQuick.CrossPlatform.Common
{
    /// <summary>
    /// Wrapper interface for timer argumets
    /// </summary>
    public interface ITimerArgs
    {
        /// <summary>
        /// Data passed as argument to timer callback
        /// </summary>
        object Data { get; }
    }

    /// <summary>
    /// Default class implements interface with IwQuick timer
    /// </summary>
    public class TimerArgs: ITimerArgs
    {
        private readonly TimerCarrier _carrier;

        public TimerArgs(TimerCarrier carrier)
        {
            _carrier = carrier;
        }

        /// <summary>
        /// Data passed as argument to timer callback
        /// </summary>
        public object Data
        {
            get { return _carrier.Data; }
        }
    }
}
