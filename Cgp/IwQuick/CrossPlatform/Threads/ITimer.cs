using System;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsTimeout { get; }

        /// <summary>
        /// 
        /// </summary>
        int TickId { get; }

        /// <summary>
        /// true if this is the first tick of the timer
        /// </summary>
        bool IsFirstTick { get; }

        /// <summary>
        /// true if this the previous timeout was being retried by return false
        /// </summary>
        bool IsRetriedTimeout { get; }

        /// <summary>
        /// custom data
        /// </summary>
        Object Data { get; set; }

        /// <summary>
        /// period of the timer/timeout
        /// </summary>
        long Period { get; }

        /// <summary>
        /// returns the amount of time remaining to the next tick
        /// </summary>
        TimeSpan Remaining { get; }

        /// <summary>
        /// returns amount of time remaining to the timer/timeout tick
        /// </summary>
        long RemainingMiliseconds { get; }

        /// <summary>
        /// returns time that elapsed from the last tick
        /// 
        /// if TimeStamp.MaxValue, timeout/timer did not tick yet
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// returns time that elapsed from the last tick, in miliseconds
        /// </summary>
        long ElapsedMiliseconds { get; }

        /// <summary>
        /// returns, whether timer is suspended
        /// </summary>
        bool IsSuspended { get; }

        /// <summary>
        /// stops the timer
        /// </summary>
        void StopTimer();

        /// <summary>
        /// suspends the timer
        /// </summary>
        void SuspendTimer();

        /// <summary>
        /// resumes the timer;
        /// not applicable, when the timer is already resumed, or it's currently executing the OnTimer method
        /// </summary>
        void ResumeTimer();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodOrNextTick"></param>
        /// <exception cref="ArgumentException">if the periodOrNextTick is less than zero</exception>
        void ChangeTimer(long periodOrNextTick);
    }
}