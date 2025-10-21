using System;
using System.Collections.Generic;

namespace Contal.IwQuick.CrossPlatform.Common
{
    /// <summary>
    /// Generic interface for filter time guarded object
    /// </summary>
    /// <typeparam name="T">Type representing internal value of this object</typeparam>
    public interface IFilterTimedObject<T>
    {
        /// <summary>
        /// Event raised when internal status changed after filter time elapsed
        /// </summary>
        event EventHandler<SingleParamEventArgs<T>> StatusChanged;

        /// <summary>
        /// Filter time for this object, changes within this time are ignore
        /// </summary>
        TimeSpan FilterTime { get; set; }

        /// <summary>
        /// Gets current value (stable, after time elapsed)
        /// </summary>
        T ValidValue { get; }

        /// <summary>
        /// Sets initial object status (ignores filtertime)
        /// </summary>
        /// <param name="status">Initial value</param>
        void SetInitialStatus(T status);

        /// <summary>
        /// Triggers status change, this value goes to temporary one and will update valid value
        /// if not changed to something else withing filtertime
        /// </summary>
        /// <param name="newStatus">New object status</param>
        void ChangeStatus(T newStatus);

        /// <summary>
        /// Adds exceptions to status values. Calling ChangeStatus with any of this values will 
        /// change object status immediately
        /// </summary>
        /// <param name="args">Values that skip filtertime and change status immediately</param>
        void AddFiltertimeExceptions(params T[] args);
    }

    /// <summary>
    /// Generic implementation for filter time guarded object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterTimedObject<T> : IFilterTimedObject<T>
    {
        private const int MinimalFiltertime = 10;

        private readonly ITimeoutProvider _timeoutProvider;
        private readonly object _rootSync = new object();

        private T _currentValue;
        private readonly List<T> _filtertimeExceptionValues;
        private T _validValue;
        private ITimerWrapper _timeout;

        public FilterTimedObject(
            ITimeoutProvider timeoutProvider,
            TimeSpan filterTime)
            : this(timeoutProvider)
        {
            FilterTime = filterTime;
        }

        public FilterTimedObject(
            ITimeoutProvider timeoutProvider)
        {
            _timeoutProvider = timeoutProvider;
            _filtertimeExceptionValues = new List<T>();
            _currentValue = default(T);
            _validValue = default(T);
        }

        /// <summary>
        /// Event raised when internal status changed after filter time elapsed
        /// </summary>
        public event EventHandler<SingleParamEventArgs<T>> StatusChanged;

        /// <summary>
        /// Filter time for this object, changes within this time are ignore
        /// </summary>
        public TimeSpan FilterTime { get; set; }

        /// <summary>
        /// Gets current value (stable, after time elapsed)
        /// </summary>
        public T ValidValue { get { return _validValue; } }

        /// <summary>
        /// Sets initial object status (ignores filtertime)
        /// </summary>
        /// <param name="status">Initial value</param>
        public void SetInitialStatus(T status)
        {
            lock (_rootSync)
            {
                EnsureTimerStopped();
                _currentValue = status;
                _validValue = status;
            }
        }

        /// <summary>
        /// Triggers status change, this value goes to temporary one and will update valid value
        /// if not changed to something else withing filtertime
        /// </summary>
        /// <param name="newStatus">New object status</param>
        public void ChangeStatus(T newStatus)
        {
            lock (_rootSync)
            {
                if (_validValue.Equals(newStatus))
                {
                    EnsureTimerStopped();
                    _currentValue = _validValue;
                    return;
                }

                if (_currentValue.Equals(newStatus))
                    return;

                EnsureTimerStopped();

                _currentValue = newStatus;

                var shouldUpdateImmediately =
                    IsLessThanMinimalFiltertime() ||
                    IsFiltertimeException(_currentValue);

                if (shouldUpdateImmediately)
                {
                    UpdateValidStatus(_currentValue);
                    return;
                }

                _timeout = _timeoutProvider.StartTimeout(
                    (long)FilterTime.TotalMilliseconds,
                    _currentValue,
                    OnTimerEvent);
            }
        }

        /// <summary>
        /// Filter time is not used for these values and any future change is immediately
        /// changing valid status of input
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void AddFiltertimeExceptions(params T[] args)
        {
            lock (_rootSync)
            {
                foreach (var arg in args)
                {
                    if (_filtertimeExceptionValues.Contains(arg))
                        continue;

                    _filtertimeExceptionValues.Add(arg);
                }
            }
        }

        protected virtual void RaiseStatusChanged()
        {
            var handler = StatusChanged;
            if (handler != null)
                handler(this, new SingleParamEventArgs<T>(_validValue));
        }

        private bool OnTimerEvent(ITimerArgs arg)
        {
            UpdateValidStatus((T)arg.Data);
            return true;
        }

        private void UpdateValidStatus(T status)
        {
            _validValue = status;
            RaiseStatusChanged();
        }

        private void EnsureTimerStopped()
        {
            if (_timeout != null)
            {
                _timeout.StopTimer();
                _timeout = null;
            }
        }

        private bool IsLessThanMinimalFiltertime()
        {
            return FilterTime.TotalMilliseconds <= MinimalFiltertime;
        }

        private bool IsFiltertimeException(T value)
        {
            return _filtertimeExceptionValues.Contains(value);
        }
    }
}
