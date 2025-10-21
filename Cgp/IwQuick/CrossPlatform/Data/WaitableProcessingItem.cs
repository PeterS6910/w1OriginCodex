using System;
using System.Threading;
using Contal.IwQuick.Sys;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public abstract class WaitableProcessingRequest<TParam> 
        : IProcessingQueueRequest<TParam>
    {
        private ManualResetEvent _synch;

        public void Execute(TParam param)
        {
            try
            {
                ExecuteInternal(param);
            }
            finally
            {
                if (_synch != null)
                    _synch.Set();
            }
        }

        public void OnError(
            TParam param, 
            Exception error)
        {
            HandledExceptionAdapter.Examine(error);
        }

        protected abstract void ExecuteInternal(TParam param);

        public void EnqueueAsync(ThreadPoolQueue<WaitableProcessingRequest<TParam>, TParam> processingQueue)
        {
            processingQueue.Enqueue(this);
        }

        public void EnqueueSync(ThreadPoolQueue<WaitableProcessingRequest<TParam>, TParam> processingQueue)
        {
            _synch = new ManualResetEvent(false);
            processingQueue.Enqueue(this);
        }

        public void WaitForCompletion()
        {
            if (_synch == null)
                return;

            _synch.WaitOne();
        }
    }
}


