using System;
using System.Threading;
using Contal.IwQuick.Sys;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// 
    /// </summary>
    public class TimerPool : AObjectPool<TimerPool.TimerParams>
    {
        /// <summary>
        /// not calling it singleton, because parallel instances are allowed
        /// </summary>
        private static volatile TimerPool _implicitInstance = null;
        private static readonly object _lockImplicitInstance = new object();

        /// <summary>
        /// not calling it singleton, because parallel instances are allowed
        /// </summary>
        public static TimerPool Implicit
        {
            get
            {
                if (_implicitInstance == null)
                    lock (_lockImplicitInstance)
                    {
                        if (_implicitInstance == null)
                            _implicitInstance = new TimerPool();
                    }

                return _implicitInstance;
            }
        }

        public class TimerParams
        {
            private readonly Timer _timer;
            public Timer Timer
            {
                get { return _timer; }
            }

            internal TimerCallback _userCallback;
            internal object _state;

            internal int _dueTime;
            internal int _period;

            private TimerParams(TimerCallback callbackBridgeDelegate)
            {
                // DO NOT DO THIS, _userCallback will be used for delagate from outside, 
                // however callbackBridgeDelegate should always point to TimerPool.OnTimerBridge
                //_userCallback = callbackBridgeDelegate;
                
                // create timer suspended
                //
                // instance of TimerParams will be tightly bound to instance of Timer from now on,
                // without the ability to break this bond
                _timer = new Timer(callbackBridgeDelegate, this, Timeout.Infinite, Timeout.Infinite);
            }

            /// <summary>
            /// done this way to avoid new() instantiation by missunderstanding
            /// </summary>
            /// <returns></returns>
            internal static TimerParams Create(TimerCallback callback)
            {
                return new TimerParams(callback);
            }

            internal void FinalizeBeforeReturn()
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                // drop references
                _userCallback = null;
                _state = null;
            }

// ReSharper disable RedundantNameQualifier
            public static implicit operator System.Threading.Timer(TimerParams timerParams)
// ReSharper restore RedundantNameQualifier
            {
                if (null == timerParams)
                    return null;

                return timerParams._timer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TimerParams CreateObject()
        {
            // just registerring empty reference for later usage
            var timerParams = TimerParams.Create(OnTimerBridge);


            // start the timer after it's registerred
            // DON'T START HERE, DONE IN StartTimer
            //timer.Change(timerParams._dueTime, timerParams._period);

            return timerParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
#pragma warning disable 809

        [Obsolete("Cannot use parameterless Get method on this pool, use GetTimer instead",true)]
        public override TimerParams Get()
        {
            return null;
        }
#pragma warning restore 809

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerCallback"></param>
        /// <param name="state"></param>
        /// <param name="dueTime"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public TimerParams GetTimer(
            [NotNull] TimerCallback timerCallback,
            object state,
            int dueTime,
            int period)
        {
            if (ReferenceEquals(timerCallback, null))
                throw new ArgumentNullException("timerCallback");

            if (dueTime < 0)
                dueTime = Timeout.Infinite;

            if (period <= 0)
                period = Timeout.Infinite;

            bool newlyAdded;

            var timerParams = base.Get(out newlyAdded);
            
            if (timerParams != null)
            {
                timerParams._userCallback = timerCallback;
                timerParams._state = state;
                timerParams._dueTime = dueTime;
                timerParams._period = period;

                // start / re-start timer according the new parameters
                if (dueTime > 0 || period > 0)
                    timerParams.Timer.Change(dueTime, period);
                //else
                    // otherwise should be stopped Change(Timeout.Infinite,Timeout.Infinite) in last Return call

            }
            else
            {
                throw new SystemException("TimerPool.GetTimer TimerParams not instantiated");
            }


            return timerParams;
        }

        /// <summary>
        /// necessary to be able to call different delegates over
        /// rotating timers
        /// 
        /// SHOULD BE AS FAST AS POSSIBLE
        /// </summary>
        protected void OnTimerBridge(object state)
        {
            var timerParams = state as TimerParams;
            if (timerParams == null)
            {
#if DEBUG
                DebugHelper.TryBreak("TimerPool.OnTimerBridge state is not TimerParams");
#endif
                return;
            }

            try
            {
                timerParams._userCallback(timerParams._state);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }

        }

        /// <summary>
        /// returning timer will be also stopped
        /// </summary>
        /// <param name="returnedTimer"></param>
        /// <exception cref="ArgumentNullException">if returnedTimer is null</exception>
        public override void Return([NotNull] TimerParams returnedTimer)
        {
            if (ReferenceEquals(returnedTimer, null))
                throw new ArgumentNullException("returnedTimer");

            // includes stopping of the timer
            // 
            // on the other hand also validates ObjectDisposedException
            returnedTimer.FinalizeBeforeReturn();


            base.Return(returnedTimer);
        }

#if DEBUG
        private volatile int _lastMax;
        private const int DebuggingThresholdCount = 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="freeObjectsCountSnapshot"></param>
        /// <param name="averageFreeObjectsCount"></param>
        /// <param name="maxFreeObjectsCount"></param>
        /// <returns></returns>
        protected override int ValidateReducingCondition(int freeObjectsCountSnapshot, double averageFreeObjectsCount, int maxFreeObjectsCount)
        {
#pragma warning disable 420
            var previousLastMax = Interlocked.Exchange(ref _lastMax, maxFreeObjectsCount);
#pragma warning restore 420

            if (previousLastMax != maxFreeObjectsCount &&
                maxFreeObjectsCount > DebuggingThresholdCount)
                DebugHelper.TryBreak("TimerPool filled with timer count "+previousLastMax+" over "+DebuggingThresholdCount,
                    freeObjectsCountSnapshot, averageFreeObjectsCount, maxFreeObjectsCount);

            return -1;
        }
#endif
    }
}
