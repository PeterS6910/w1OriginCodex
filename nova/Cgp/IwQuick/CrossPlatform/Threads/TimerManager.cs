using System;
using System.Collections;
using System.Threading;
using JetBrains.Annotations;


using Contal.IwQuick.Data;

namespace Contal.IwQuick.Threads
{
    /// <summary>
    /// class for starting and managing system timers and timeouts
    /// </summary>
    public sealed class TimerManager : ADisposable, IEnumerable
    {
        private static volatile TimerManager _static = null;
        private static readonly object _syncStatic = new object();

        /// <summary>
        /// implicit static instance, NOT a singleton
        /// </summary>
        public static TimerManager Static
        {
            get
            {
                if (null == _static)
                    lock (_syncStatic)
                    {
                        if (_static == null)
                            _static = new TimerManager();
                    }

                return _static;
            }
        }

        internal class TimerCarrierReserved
        {
            
        }

        // used instead of HashSet due synchronized nature of SyncDictionary
        private readonly SyncDictionary<TimerCarrier,TimerCarrierReserved> _timers = 
            new SyncDictionary<TimerCarrier,TimerCarrierReserved>(32);
        
        internal void ExecuteIfContains(
            [NotNull] TimerCarrier timerCarrier,
            [NotNull] SyncDictionary<TimerCarrier, TimerCarrierReserved>.DTryGetProcessingLambda lambaIfContains)
        {
            _timers.TryGetValue(timerCarrier, lambaIfContains);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DebugOutput = false;

        /// <summary>
        /// starts system timer with specified period and delegate to handle the tick;
        /// returns null, if the parameters are incorrect (null onTimerEvent or wrong doFirstTick timeout)
        /// </summary>
        /// <param name="isTimeout">if true, the timer tends to tick just once</param>
        /// <param name="firstTickIn">timeout for the first tick</param>
        /// <param name="period">period of the timer in miliseconds</param>
        /// <param name="onTimerEvent">event to raise on tick</param>
        /// <param name="data">optional data</param>
        /// <exception cref="ArgumentNullException">if onTimerEvent is null</exception>
        /// <exception cref="ArgumentException">if period or timeout is 0</exception>
        /// <returns></returns>
        private ITimer StartTimer(
            bool isTimeout, 
            long firstTickIn, 
            long period, 
            [NotNull] DOnTimerEvent onTimerEvent, 
            [CanBeNull] Object data)
        {
            if (onTimerEvent == null)
                throw new ArgumentNullException("onTimerEvent");

            if (Timeout.Infinite == firstTickIn ||
                firstTickIn < 0)
                throw new ArgumentException("Unable start timer without having a first tick");

            if (period < 0 || isTimeout)
                period = Timeout.Infinite;
            
            var timerCarrier = new TimerCarrier(this, isTimeout, onTimerEvent)
            {
                _data = data,
                _firstTickIn = firstTickIn,
                _period = (isTimeout
                    ? -1
                    : period)
            };


            SetTimer(timerCarrier);

            return timerCarrier;
        }

        /// <summary>
        /// starts timer with specified period and timer event;
        /// returns null, if the parameters are incorrect
        /// </summary>
        /// <param name="period">period in miliseconds</param>
        /// <param name="immediateTick">if true, the onTimerEvent is raised immediately</param>
        /// <param name="onTimerEvent">the event to raise</param>
        /// <param name="data">optional data</param>
        /// <returns></returns>
        public ITimer StartTimer(
            long period, 
            bool immediateTick,
            [NotNull] DOnTimerEvent onTimerEvent, 
            [CanBeNull] Object data)
        {
            long firstTickIn = (immediateTick ? 0 : period);

            return StartTimer(false, firstTickIn, period, onTimerEvent, data);
        }

        /// <summary>
        /// starts timer with specified period and timer event;
        /// returns null, if the parameters are incorrect
        /// </summary>
        /// <param name="period">period in miliseconds</param>
        /// <param name="immediateTick">if true, the onTimerEvent is raised immediately</param>
        /// <param name="onTimerEvent">the event to raise</param>
        /// <returns></returns>
        public ITimer StartTimer(
            long period, 
            bool immediateTick,
            [NotNull] DOnTimerEvent onTimerEvent)
        {
            long firstTickIn = (immediateTick ? 0 : period);

            return StartTimer(false, firstTickIn, period, onTimerEvent, null);
        }

        /// <summary>
        /// starts timeout with specified period and timer event;
        /// returns null, if the parameters are incorrect
        /// </summary>
        /// <param name="timeout">timeout in miliseconds</param>
        /// <param name="data">optional data</param>
        /// <param name="onTimerEvent">event to raise on timeout end</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if onTimerEvent is null</exception>
        /// <exception cref="ArgumentException">if timeout is 0</exception>
        public ITimer StartTimeout(
            long timeout, 
            [CanBeNull] Object data, 
            [NotNull] DOnTimerEvent onTimerEvent)
        {
            return StartTimer(true, timeout, timeout, onTimerEvent, data);
        }

        /// <summary>
        /// starts timeout with specified period and timer event;
        /// returns null, if the parameters are incorrect
        /// </summary>
        /// <param name="timeout">timeout in miliseconds</param>
        /// <param name="onTimerEvent">event to raise on timeout end</param>
        /// <returns></returns>
        public ITimer StartTimeout(
            long timeout, 
            [NotNull] DOnTimerEvent onTimerEvent)
        {
            return StartTimer(true, timeout, 0, onTimerEvent, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerCarrier"></param>
        private void SetTimer([NotNull] TimerCarrier timerCarrier)
        {
            try
            {
                // register
                _timers.Add(timerCarrier, null);
                timerCarrier._lastResumeDateTime = TimerCarrier.CrossPlatformGetSystemTime();

                // same precaution as in TimeCarrier.InternalResumeTimer
                timerCarrier.InternalChangeTimer(timerCarrier._firstTickIn, true, true);
            }
            catch (Exception err)
            {
                //RemoveTimerCarrier(timerCarrier);
                // done by dispose
                timerCarrier.Dispose();

                Sys.HandledExceptionAdapter.Examine(err);

                throw;
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timer"></param>
        internal void RemoveTimerCarrier([NotNull] TimerCarrier timer)
        {
            try
            {
                var removed = _timers.Remove(timer);
                DebugHelper.NOP(removed);
                
            }
            catch(Exception err)
            {
                Sys.HandledExceptionAdapter.Examine(err);
            }
        }

        

        internal void ResumeTimer([NotNull] TimerCarrier timerCarrier)
        {
            if (!_timers.ContainsKey(timerCarrier))
            {
                // this means, that the timer has been already stop by UnsetTimer
                return;
            }

            timerCarrier.ExplicitResumeTimer(
                //out isDelayed
                );
        }

        /// <summary>
        /// stops all timers under current TimerManager instance
        /// </summary>
        public void StopAll()
        {
            try
            {
                var timers = _timers.KeysSnapshot;
                foreach (var t in timers)
                {
                    try
                    {
                        if (t != null)
                            t.StopTimer();
                    }
                    catch (Exception e1)
                    {
                        Sys.HandledExceptionAdapter.Examine(e1);
                    }
                }
            }
            catch (Exception e2)
            {
                Sys.HandledExceptionAdapter.Examine(e2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExplicitDispose"></param>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            StopAll();

            if (_timers.Count > 0)
            {
                _timers.Clear();
            }
        }

        

        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _timers.GetEnumerator();
        }

        /// <summary>
        /// returns count of the timers
        /// </summary>
        public int Count
        {
            get
            {
                return _timers.Count;
            }
        }

    }
}
