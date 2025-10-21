using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{

    /// <summary>
    /// system timer descriptor
    /// </summary>
    public sealed class TimerCarrier : ADisposable, ITimer
    {
        internal readonly bool _isTimeout = false;
        internal readonly DOnTimerEvent _onTimerEvent = null;

        [NotNull]
        internal readonly TimerManager _timersParent = null;

        private volatile TimerPool.TimerParams _timerParams = null;
        /// <summary>
        /// 
        /// </summary>
        private readonly object _syncTimer = new object();

        /// <summary>
        /// 
        /// </summary>
        internal volatile Object _data = null;

        internal long _firstTickIn = -1;

        internal long _period = -1;


        /// <summary>
        /// 
        /// </summary>
        public bool IsTimeout
        {
            get
            {
                return _isTimeout;
            }
        }


        internal DateTime _lastResumeDateTime;
        internal TimeSpan _timeElapsed = TimeSpan.MaxValue;
        internal DateTime _lastSuspendDateTime;

        internal int _tickId;

        /// <summary>
        /// 
        /// </summary>
        public int TickId
        {
            get
            {
                return _tickId;
            }
        }

        /// <summary>
        /// true if this is the first tick of the timer
        /// </summary>
        public bool IsFirstTick
        {
            get { return _tickId == 1; }
        }

        /// <summary>
        /// true if this the previous timeout was being retried by return false
        /// </summary>
        public bool IsRetriedTimeout
        {
            get { return (_isTimeout && _tickId > 1); }
        }

        internal void IncTickId()
        {
            
        }

#if DEBUG
        //private bool _isDisposed = false;
        private string _description = null;
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime CrossPlatformGetSystemTime()
        {
#if COMPACT_FRAMEWORK
            return Sys.Microsoft.SystemTime.GetSystemTime();
#else
            return DateTime.UtcNow;
#endif

        }


        /// <summary>
        /// implicit constructor
        /// </summary>
        internal TimerCarrier(
            [NotNull] TimerManager parent, 
            bool isTimeout, 
            DOnTimerEvent timerEvent)
        {
            if (null == parent)
                throw new ArgumentNullException("parent");

            //_id = Interlocked.Increment(ref _idCounter);

            _timersParent = parent;

            _isTimeout = isTimeout;

#if DEBUG
            if (_isTimeout)
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _description = "Timeout" + GetHashCode().ToString(StringConstants.HEX8_BIG_FORMAT);
            else
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _description = "Timer" + GetHashCode().ToString(StringConstants.HEX8_BIG_FORMAT);
#endif

            _onTimerEvent = timerEvent;

            // prepare timer instance BUT SUSPENDED/IDLE
            _timerParams =
                TimerPool.Implicit.GetTimer(OnTimerBridge, this, Timeout.Infinite, Timeout.Infinite);
                //new Timer(OnTimerBridge, this, Timeout.Infinite, Timeout.Infinite);
        }

#if DEBUG
        /// <summary>
        /// informational description of the timer
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _description = value;
            }
        }
#endif

        /// <summary>
        /// custom data
        /// </summary>
        public Object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// period of the timer/timeout
        /// </summary>
        public long Period
        {
            get
            {
                return _period;
            }
        }

        /// <summary>
        /// stops the timer
        /// </summary>
        public void StopTimer()
        {
            // calls InternalDispose indirectly
            Dispose();
        }

        /// <summary>
        /// suspends the timer
        /// </summary>
        public void SuspendTimer()
        {
            if (_suspendState != SuspendState.ExplicitSuspend)
                lock (_syncTimer)
                {
                    if (_suspendState == SuspendState.ExplicitSuspend)
                        return;

                    if (_suspendState == SuspendState.ImplicitSuspend)
                    {
                        _nextSuspendState = SuspendState.ExplicitSuspend;
                        return;
                    }

                    // just to be sure, even if the previous tick shouldn't be planned with repetition
                    // see InternalResumeTimer
                    InternalChangeTimer(Timeout.Infinite, true, false);

                    _suspendState = SuspendState.ExplicitSuspend;

                    _lastSuspendDateTime = CrossPlatformGetSystemTime();

                    if (TimeSpan.MaxValue == _timeElapsed)
                    {
                        _timeElapsed =
                            _lastSuspendDateTime -
                            _lastResumeDateTime;
                    }
                    else
                    {
                        _timeElapsed =
                            _timeElapsed +
                            (_lastSuspendDateTime -
                             _lastResumeDateTime);
                    }
                }
        }


        private void OnTimerBridge(Object dataObject)
        {
            var timerCarrier = dataObject as TimerCarrier;

            // used as security key
            if (!ReferenceEquals(timerCarrier,this) ||
                ReferenceEquals(timerCarrier,null))
            {
                DebugHelper.TryBreak(
                    "TimerCarrier.OnTimerBridge was called on different TimerCarrier instance or is null");
                return;
            }

            // first line of defense to avoid ticking when change is already in progress
            if (!Monitor.TryEnter(_syncTimer))
                return;

            bool toStop = false;

            try
            {
                if (!ImplicitSuspendTimer())
                    // means other suspend (internal or explicit) is already in progress
                    return;

                try
                {
                    Interlocked.Increment(ref _tickId);

                    // calling the user callback
                    bool timerEventReturn = _onTimerEvent(this);

                    toStop = EvaluateStoppingTimer(this, timerEventReturn);
                }
                catch (Exception aError)
                {
                    toStop = true;

                    Sys.HandledExceptionAdapter.Examine(aError);

#if DEBUG
                    DebugHelper.TryBreak("TimerManager.OnTimerBridge exception", aError);
#endif
                    //Debug.Assert(false, aError.Message);
                }
                finally
                {
                    if (toStop)
                    {
                        Dispose();
                        //aTimerCarrier.StopTimer();
                    }
                    else
                    {
                        // on the level of resume timer, if it wasn't already stopped by outer call of UnsetTimer
                        // do not have to check it here

                        _timersParent.ExecuteIfContains(
                            this,
                            (timer, timerFound, reserved)
                                => { if (timerFound) ImplicitResumeTimer(); });


                    }
                }
            }
            catch (Exception anyError)
            {
                Dispose();

                Sys.HandledExceptionAdapter.Examine(anyError);

#if DEBUG
                DebugHelper.TryBreak("TimerManager.OnTimerBridge general error",anyError);
#endif
            }
            finally
            {
                Monitor.Exit(_syncTimer);
            }
        }

        private bool EvaluateStoppingTimer(
            [NotNull] TimerCarrier timerCarrier, 
            bool timerEventReturn)
        {
            bool toStop =false;
#if DEBUG
            if (_timersParent.DebugOutput)
                Console.WriteLine(
                    DateTime.Now + " : Tick of " + timerCarrier.Description + " [" +
                    (timerCarrier._isTimeout ? "timeout" : "timer") + "," + timerEventReturn + "]");
#endif

            // timeout timer, OnTimer returns true
            if (timerCarrier._isTimeout && timerEventReturn)
            {
                toStop = true;
#if DEBUG
                if (_timersParent.DebugOutput)
                    Console.WriteLine(DateTime.Now + " : Stop of " + timerCarrier.Description + " [" +
                                      (timerCarrier._isTimeout ? "timeout" : "timer") + ",true]");
#endif
            }


            // normal timer, but OnTimer method returns false
            if (!timerCarrier._isTimeout && !timerEventReturn)
            {
                toStop = true;
#if DEBUG
                if (_timersParent.DebugOutput)
                    Console.WriteLine(DateTime.Now + " : Stop of " + timerCarrier.Description + " [" +
                                      (timerCarrier._isTimeout ? "timeout" : "timer") + ",false]");
#endif
            }
            return toStop;
        }

        /// <summary>
        /// resumes the timer;
        /// not applicable, when the timer is already resumed, or it's currently executing the OnTimer method
        /// </summary>
        public void ResumeTimer()
        {
            _timersParent.ResumeTimer(this);
        }

        /// <summary>
        /// returns the amount of time remaining to the next tick
        /// </summary>
        public TimeSpan Remaining
        {
            get
            {
                // TODO take care of the cases, when _firstTickIn should 
                // be considered instead of the period

                var period = new TimeSpan(_period * TicksPerMs);

                return period - Elapsed;
            }
        }

        /// <summary>
        /// returns amount of time remaining to the timer/timeout tick
        /// </summary>
        public long RemainingMiliseconds
        {
            get
            {
                var remaining = Remaining;
                return remaining.Ticks / TicksPerMs;
            }
        }

        /// <summary>
        /// returns time that elapsed from the last tick
        /// 
        /// if TimeStamp.MaxValue, timeout/timer did not tick yet
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                var tsSinceLastResume =
                    _suspendState == SuspendState.Resumed ||
                    _suspendState == SuspendState.ImplicitSuspend
                        ? CrossPlatformGetSystemTime() - _lastResumeDateTime
                        : TimeSpan.Zero;

                var elapsedProjection = 
                    _timeElapsed != TimeSpan.MaxValue 
                        ? _timeElapsed 
                        : TimeSpan.Zero;

                return tsSinceLastResume + elapsedProjection;
            }
        }

        private const int TicksPerMs = 10000; // same for COMPACT_FRAMEWORK

        /// <summary>
        /// returns time that elapsed from the last tick, in miliseconds
        /// </summary>
        public long ElapsedMiliseconds
        {
            get
            {
                var elapsed = Elapsed;
                return elapsed.Ticks / TicksPerMs;
            }
        }

        private enum SuspendState
        {
            Resumed,
            ImplicitSuspend,
            ExplicitSuspend
        }

        private volatile SuspendState _suspendState = SuspendState.Resumed;
        private volatile SuspendState _nextSuspendState = SuspendState.Resumed;

        /// <summary>
        /// returns, whether timer is suspended
        /// </summary>
        public bool IsSuspended
        {
            get { return _suspendState == SuspendState.ExplicitSuspend; }
        }

        internal bool ImplicitSuspendTimer()
        {
            if (_suspendState == SuspendState.Resumed)
                lock (_syncTimer)
                {
                    if (_suspendState != SuspendState.Resumed)
                        return false;

                    // just to be sure, even if the previous tick shouldn't be planned with repetition
                    // see InternalResumeTimer
                    InternalChangeTimer(Timeout.Infinite, true,false);

                    _suspendState = SuspendState.ImplicitSuspend;
                    _nextSuspendState = SuspendState.Resumed;

                    return true;
                }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Timer" + GetHashCode().ToString(StringConstants.HEX8_BIG_FORMAT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodOrNextTick"></param>
        /// <param name="throwExceptions"></param>
        /// <param name="explicitLockViaSyncTimer"></param>
        /// <returns></returns>
        internal bool InternalChangeTimer(
            long periodOrNextTick, 
            bool throwExceptions,
            bool explicitLockViaSyncTimer
            )
        {
            if (AimedToBeDisposed)
                if (throwExceptions)
                    throw new ObjectDisposedException(ToString());
                else
                    return false;

            bool ret = false;

            try
            {
                if (explicitLockViaSyncTimer)
                    Monitor.Enter(_syncTimer);

                ret = _timerParams.Timer.Change(periodOrNextTick, Timeout.Infinite);
            }
            catch
            {
                if (throwExceptions)
                    throw;
            }
            finally
            {
                if (explicitLockViaSyncTimer)
                    Monitor.Exit(_syncTimer);
            }

            if (throwExceptions && !ret)
                throw new ObjectDisposedException("_timerParams");

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodOrNextTick"></param>
        /// <exception cref="ArgumentException">if the periodOrNextTick is less than zero</exception>
        public void ChangeTimer(long periodOrNextTick)
        {
            if (!_isTimeout && periodOrNextTick < 0)
                throw new ArgumentException("Timer must have positive or zero period", "periodOrNextTick");

            lock (_syncTimer)
            {
                long remainingMilliseconds;

                if (!_isTimeout)
                {
                    // TODO discriminate between first and subsequent ticks
                    // when in the explicit suspend state.

                    remainingMilliseconds = 
                        RemainingMiliseconds + (periodOrNextTick - _period);

                    _period = periodOrNextTick;
                }
                else
                {
                    remainingMilliseconds =
                        RemainingMiliseconds + (periodOrNextTick - _firstTickIn);

                    _firstTickIn = periodOrNextTick;
                }

                if (_suspendState == SuspendState.Resumed && remainingMilliseconds > 0)
                    InternalChangeTimer(
                        remainingMilliseconds, 
                        true,                      
                        // no need for lock, it's already in lock
                        false); 
            }
        }

        internal void ImplicitResumeTimer()
        {
            if (_suspendState == SuspendState.ImplicitSuspend)
                lock (_syncTimer)
                {
                    if (_suspendState != SuspendState.ImplicitSuspend)
                        return;

                    long nextTickIn;

                    if (_nextSuspendState == SuspendState.ExplicitSuspend)
                    {
                        nextTickIn = Timeout.Infinite;
                        _lastSuspendDateTime = CrossPlatformGetSystemTime();
                    }
                    else
                    {   // resuming
                        nextTickIn =
                            _isTimeout
                                ? _firstTickIn
                                : _period;

                        _lastResumeDateTime = CrossPlatformGetSystemTime();
                    }

                    _timeElapsed = TimeSpan.MaxValue;

                    InternalChangeTimer(nextTickIn, true,false);
                    _suspendState = _nextSuspendState;
                }
        }

        internal void ExplicitResumeTimer(
            //out bool isDelayed
            )
        {
            //isDelayed = false;

            if (_suspendState != SuspendState.Resumed)
                lock (_syncTimer)
                {
                    if (_suspendState == SuspendState.Resumed)
                        return;

                    if (_suspendState == SuspendState.ImplicitSuspend)
                    {
                        _nextSuspendState = SuspendState.Resumed;
                        return;
                    }

                    //DateTime aNow = CrossPlatformGetSystemTime();

                    // TODO take care of the cases, when _firstTickIn should
                    // be considered instead of the period

                    var tsPeriod = new TimeSpan(_period * TicksPerMs);

                    TimeSpan tmpTsPeriod = tsPeriod;

                    if (TimeSpan.MaxValue != _timeElapsed)
                    {
                        //if (_timeElapsed >= tsPeriod)
                        //{
                        //    // stop previous timer immediately - because it's late anyway
                        //    InternalChangeTimer(Timeout.Infinite, true,false);

                        //    //lPeriod = timerCarrier.m_lPeriod;
                        //    //timerCarrier._lastResumeDateTime = aNow;
                        //    _lastResumeDateTime = CrossPlatformGetSystemTime();

                        //    // to fake the resume
                        //    _suspendState = SuspendState.Resumed;

                        //    isDelayed = true;

                        //    return;
                        //}

                        tmpTsPeriod -= _timeElapsed;
                    }

                    long nextTickIn = tmpTsPeriod.Ticks / TicksPerMs;

                    /*timerCarrier._lastResumeDateTime = aNow -
                            (TimeSpan)timerCarrier._timeForegone;*/

                    _lastResumeDateTime = CrossPlatformGetSystemTime();

                    // -1 for period as precaution, so even the timer wouldn't
                    // like to surpass suspending the previous timer tick
                    InternalChangeTimer(nextTickIn, true, false);

                    //if (!ChangeTimer(period, period))
                    //    return;

                    _suspendState = SuspendState.Resumed;
                }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            try
            {
                // asap, stopping timer
                InternalChangeTimer(Timeout.Infinite, false,!isExplicitDispose);
            }
            catch (Exception e)
            {
                Sys.HandledExceptionAdapter.Examine(e);
            }

            try
            {
                _timersParent.RemoveTimerCarrier(this);
            }
            catch (Exception e)
            {
                Sys.HandledExceptionAdapter.Examine(e);
            }

            try
            {
                TimerPool.Implicit.Return(_timerParams);
                //_timerParams.Dispose();
            }
            catch (Exception e)
            {
                Sys.HandledExceptionAdapter.Examine(e);
            }
            finally
            {
                _timerParams = null;
//#if DEBUG
//                try
//                {
//                    Debug.WriteLine("End of " + _description + " dispose");
//                }
//                catch
//                {

//                }
//#endif
            }


        }
    }
}
