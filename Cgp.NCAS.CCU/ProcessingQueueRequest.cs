using System;
using System.Threading;

using Contal.IwQuickCF.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public abstract class ProcessingQueueRequest
    {
        private AutoResetEvent _requestComplete;
        private AutoResetEvent _waiterAwoke;

        private bool _wasEnqueued;

        private bool _isComplete;
        private bool _completeAcknowledged;

        public Exception Exception { get; private set; }

        protected abstract void ExecuteInternal();

        public void Enqueue(
            ProcessingQueue<ProcessingQueueRequest> processingQueue,
            AutoResetEvent requestComplete,
            AutoResetEvent waiterAwoke)
        {
            if (requestComplete == null)
            {
                processingQueue.Enqueue(this);

                _wasEnqueued = true;

                return;
            }

            try
            {
                processingQueue.Enqueue(
                    this,
                    PQEnqueueFlags.ProhibitCallFromItemProcessing
                    );
            }
            catch
            {
                return;
            }

            _wasEnqueued = true;

            _requestComplete = requestComplete;
            _waiterAwoke = waiterAwoke;
        }

        public void Execute()
        {
            try
            {
                ExecuteInternal();
            }
            catch (Exception exception)
            {
                Exception = exception;
            }
            finally
            {
                _isComplete = true;

                if (_requestComplete != null)
                    do
                    {
                        _requestComplete.Set();
                        _waiterAwoke.WaitOne();
                    }
                    while (!_completeAcknowledged);
            }
        }

        /// <summary>
        /// TODO comment
        /// </summary>
        public virtual void WaitForCompletion()
        {
            if (_requestComplete != null)
            {
                do
                {
                    _requestComplete.WaitOne();

                    if (_isComplete)
                        _completeAcknowledged = true;

                    _waiterAwoke.Set();
                }
                while (!_completeAcknowledged);

                return;
            }

            if (_wasEnqueued || _isComplete)
                return;

            try
            {
                ExecuteInternal();
            }
            finally
            {
                _isComplete = true;
            }
        }
    }
}