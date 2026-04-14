using System;
using System.Threading;
using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public abstract class AAsyncThreadPoolQueue<TRequest, TRequestCollection>
        : ADisposable
        where TRequest : class
        where TRequestCollection : class, IRequestCollection<TRequest>
    {
        private enum State
        {
            Idle,
            FirstRequestEnqueued,
            Executing,
            Waiting,
            Ended,
            EndedWithScheduledRetry,
            TimedOut,
            LastRequestFinished
        }

        protected TRequestCollection RequestCollection;

        private readonly AutoResetEvent _requestExecuteComplete =
            new AutoResetEvent(false);

        private readonly ManualResetEvent _idle =
            new ManualResetEvent(true);

        private readonly IThreadPool _threadPool;

        private readonly Timer _timeout;

        private State _state = State.Idle;

        public TRequest CurrentRequest { get; private set; }
        private int _minimalTimeoutTickCount;
        private int _requestTimeout;

        public bool IsBlocked
        {
            get;
            protected set;
        }

        public int RequestTimeout
        {
            get { return _requestTimeout; }

            set
            {
                if (value <= 0 && value != Timeout.Infinite)
                    throw new ArgumentException("AsyncThreadPoolQueue : Invalid timeout value");

                _requestTimeout = value;
            }
        }

        protected AAsyncThreadPoolQueue(
            IThreadPool threadPool,
            TRequestCollection requestCollection)
        {
            _threadPool = threadPool;
            RequestCollection = requestCollection;

            _timeout =
                new Timer(
                    OnProcessingTimeOut,
                    null,
                    Timeout.Infinite,
                    Timeout.Infinite);

        }

        private void OnProcessingTimeOut(object state)
        {
            lock (RequestCollection)
            {
                if (_state != State.Waiting
                    || Environment.TickCount - _minimalTimeoutTickCount < 0)
                {
                    return;
                }

                _requestExecuteComplete.Reset();

                _state = State.TimedOut;
                TryScheduleProcessing();
            }
        }

        protected abstract bool OnExecutionFinished(TRequest processedRequest, bool timedOut);

        public bool EndItemProcessing(
            out TRequest request,
            bool retry)
        {
            lock (RequestCollection)
            {
                if (_state != State.Waiting && _state != State.Executing)
                {
                    request = null;
                    return false;
                }

                _requestExecuteComplete.WaitOne();

                if (AimedToBeDisposed)
                {
                    request = null;
                    return false;
                }

                _timeout.Change(
                    Timeout.Infinite,
                    Timeout.Infinite);

                request = CurrentRequest;

                _state =
                    retry
                        ? State.EndedWithScheduledRetry
                        : State.Ended;

                TryScheduleProcessing();
            }

            return true;
        }

        protected void FinishEnqueue()
        {
            if (_state != State.Idle
                && _state != State.LastRequestFinished)
            {
                return;
            }

            if (_state == State.Idle)
            {
                _idle.Reset();
                _threadPool.Execute(ProcessNextItem);
            }

            _state = State.FirstRequestEnqueued;
        }

        private void TryScheduleProcessing()
        {
            _threadPool.Execute(ProcessNextItem);
        }

        private void ProcessNextItem()
        {
            do
            {
                bool aimedToBeDisposed;

                bool requestProcessingFinished = FinishProcessingRequest(out aimedToBeDisposed);

                if (aimedToBeDisposed)
                    return;

                if (!PrepareExecution(requestProcessingFinished))
                    return;

                Execute(CurrentRequest);

                if (EnterAsynchronousWaitingPhase())
                    return;

            } while (true);
        }

        private bool PrepareExecution(bool requestProcessingFinished)
        {
            lock (RequestCollection)
            {
                if (AimedToBeDisposed)
                {
                    EnterIdleState();
                    return false;
                }

                if (!requestProcessingFinished)
                    // retry
                    _state = State.Executing;
                else
                {
                    if (RequestCollection.Count == 0
                        || _state == State.LastRequestFinished)
                    {
                        EnterIdleState();
                        return false;
                    }

                    CurrentRequest = RequestCollection.GetAndRemoveFirstRequest();
                    _state = State.Executing;
                }
            }

            return true;
        }

        private bool EnterAsynchronousWaitingPhase()
        {
            _requestExecuteComplete.Set();

            lock (RequestCollection)
            {
                if (AimedToBeDisposed)
                {
                    EnterIdleState();
                    return true;
                }

                if (_state != State.Executing)
                    // EndItemProcessing arrived in the meantime
                    return false;

                // Otherwise start the "waiting" phase
                _state = State.Waiting;

                var currentRequestTimeout = RequestTimeout;

                if (currentRequestTimeout != Timeout.Infinite)
                {
                    _minimalTimeoutTickCount = Environment.TickCount + (currentRequestTimeout >> 1);

                    _timeout.Change(
                        currentRequestTimeout,
                        Timeout.Infinite);
                }

                return true;
            }
        }

        private bool FinishProcessingRequest(out bool aimedToBeDisposed)
        {
            bool timedOut;

            lock (RequestCollection)
            {
                if (AimedToBeDisposed)
                {
                    EnterIdleState();

                    aimedToBeDisposed = true;
                    return true;
                }

                aimedToBeDisposed = false;

                if (_state == State.EndedWithScheduledRetry)
                    // item processing not finished, retry
                    return false;

                timedOut = _state == State.TimedOut;

                if (RequestCollection.Count == 0)
                    // we expect the queue to enter the idle state
                    // but another enqueueing can take place below, outside the lock
                    _state = State.LastRequestFinished;
            }

            if (CurrentRequest == null)
                return true;

            return OnExecutionFinished(
                CurrentRequest,
                timedOut);
        }

        private void EnterIdleState()
        {
            CurrentRequest = null;

            _state = State.Idle;
            _idle.Set();
        }

        protected abstract void Execute(TRequest item);

        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (isExplicitDispose)
            {
                ClearAndBlock();

                TRequest request;

                EndItemProcessing(
                    out request,
                    false);

                WaitForIdle();

                _idle.Close();
                _requestExecuteComplete.Close();
            }
        }

        public void WaitForIdle()
        {
            _idle.WaitOne();
        }

        public void ClearAndBlock()
        {
            lock (RequestCollection)
            {
                if (IsBlocked)
                    return;

                IsBlocked = true;
                RequestCollection.Clear();
            }
        }

        public void Unblock()
        {
            lock (RequestCollection)
                IsBlocked = false;
        }
    }
}