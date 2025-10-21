using System;
using System.Collections;
using System.Threading;
using Contal.IwQuick.Sys;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// the processing queue will process the item on top of queue within ItemProcessing
    /// until EndItemProcessing is called, 
    /// if EndItemProcessing is not called within AsynchronousTimeout, ItemProcessingTimedOut is called
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncProcessingQueue<T> : ProcessingQueue<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<T> ItemProcessingTimedOut;

        private volatile int _asynchronousTimeout = 100;

        /// <summary>
        /// asynchronous timeout between the begin of itemprocessing and EndItemProcessing call
        /// 
        /// if 0 or negative, it means, that timeout is infinite
        /// </summary>
        public int AsynchronousTimeout
        {
            get { return _asynchronousTimeout; }
            set { _asynchronousTimeout = value > 0 ? value : 0; }
        }


        private volatile bool _noTimeoutOnProcessingFailure;

        /// <summary>
        /// 
        /// </summary>
        public bool NoTimeoutOnProcessingFailure
        {
            get { return _noTimeoutOnProcessingFailure; }
            set { _noTimeoutOnProcessingFailure = value; }
        }

        private readonly ManualResetEvent _asyncMutexFinal = new ManualResetEvent(true);
        private readonly ManualResetEvent _asyncMutexEndItemProcessingStarted = new ManualResetEvent(true);

        protected override bool ProcessSingleItem(object queueKey, ItemCarrier<T> itemCarrier, int postDelay)
        {
            _asyncMutexEndItemProcessingStarted.Reset();
            _asyncMutexFinal.Reset();

            // main processing
            var itemProcessingResult = ProcessSingleItem(queueKey, itemCarrier, false,0);

            // if _asynchronousTimeout is set to 0 or negative
            // it means it's not used
            if (_asynchronousTimeout > 0 &&
                // this condition will block starting async timeout in cases ItemProcessing
                // stopped by any exception and NoTimeoutOnProcessingFailure is set to true
                ((_noTimeoutOnProcessingFailure && itemProcessingResult) || !_noTimeoutOnProcessingFailure))
            {
                try
                {
                    var asyncMutexFinalResult = _asyncMutexFinal.WaitOne(_asynchronousTimeout, false);

                    if (AimedToBeDisposed)
                        // don't block here too long, the request queue is in process to be disposed
                        return true;


                    if (!asyncMutexFinalResult)
                    {
                        lock (_endItemProcessingLock)
                        {
                            // THIS BRANCH MEANS TIMEOUT
                            _asyncMutexFinal.Set();
                            // set this ASAP, so potential EndItemProcessing would know about it
                            // if the mutex is already set by started EndItemProcessing does not really matter, as the further mutex checking will verify it

                            var endItemProcessingAlreadyStarted =
                                _asyncMutexEndItemProcessingStarted.WaitOne(0, false);

                            if (AimedToBeDisposed)
                                // don't block here too long, the request queue is in process to be disposed
                                return true;

                            if (!endItemProcessingAlreadyStarted)
                            {
                                // this means timeout, as EndItemProcessing did NOT YET released the _asyncMutexFinal
                                // and also that EndItemProcessing DID NOT STARTED YET

                                /*
                                            if (_actualItem != null)
                                                System.Diagnostics.Debug.WriteLine("TIMEOUT: " + _actualItem.ToString() + "; " + this.GetHashCode().ToString());
                                            else
                                                System.Diagnostics.Debug.WriteLine("ITEM TIMEOUT but NULL; " + this.GetHashCode().ToString());
                                            */

                                lock (_actualItemCarrierSync)
                                {
                                    try
                                    {
                                        _actualItemCarrier.SetProcessingState(PQProcessingState.Processed);
                                    }
                                    catch (Exception error)
                                    {
                                        HandledExceptionAdapter.Examine(error);
                                    }
                                    _actualItemCarrier = null;
                                }

                                // if waiting for processing end timed out
                                if (null != ItemProcessingTimedOut)
                                    try
                                    {
                                        ItemProcessingTimedOut(itemCarrier._item);
                                    }
                                    catch (Exception e)
                                    {
                                        HandledExceptionAdapter.Examine(e);
                                    }
                            }
#if DEBUG_PROCESSING
                                        else
                                            Console.WriteLine(
                                                Thread.CurrentThread.ManagedThreadId.ToString("X8") + StringConstants.SPACE +
                                                DateTime.Now.ToString("hh:mm:ss.fff") +
                                                " End item processing did not started yet");

#endif
                        }
                    }
                    else
                    {
                        // if NO timeout occurs and post-success delay is defined
                        if (_postSuccessDelay > 0 && _postSuccessDelay > postDelay)
                            postDelay = _postSuccessDelay;
                    }
                }
                catch (ThreadAbortException)
                {
                    // do not HAE.Examine here, can be legitimate
                }
            }
            else
            {
                // wait infinitely
                _asyncMutexFinal.WaitOne();

                if (AimedToBeDisposed)
                    // don't block here too long, the request queue is in process to be disposed
                    return true;
            }

            if (postDelay > 0)
                Thread.Sleep(postDelay);

            return false;
        }

        
        protected override void InternalDispose(bool isExplicitDispose)
        {
            ItemProcessingTimedOut = null;

            try
            {
                var disposalCheck = _asyncMutexFinal.WaitOne(0, false);
                DebugHelper.Keep(disposalCheck);

                _asyncMutexFinal.Set();
            }
            catch (Exception)
            {
                // avoid exception examination in dispose
                //Sys.HandledExceptionAdapter.Examine(e);
            }

            try
            {
                var disposalCheck = _asyncMutexEndItemProcessingStarted.WaitOne(0, false);
                DebugHelper.Keep(disposalCheck);

                _asyncMutexEndItemProcessingStarted.Set();
            }
            catch (Exception)
            {
                // avoid exception examination in dispose
                //Sys.HandledExceptionAdapter.Examine(e);
            }

            base.InternalDispose(isExplicitDispose);

        }

        private volatile object _endItemProcessingLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool EndItemProcessing(bool success, object state)
        {
            var itemRef = default(T);

            return EndItemProcessing(success, state, true, ref itemRef);
        }

        /// <summary>
        /// 
        /// </summary>
        public new event Action<T, bool, object> ItemProcessingResult;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="state"></param>
        /// <param name="releaseAfterResult"></param>
        /// <param name="itemOutput"></param>
        /// <returns>true, if end of processing started before timeout occured</returns>
        public bool EndItemProcessing(bool success, object state, bool releaseAfterResult, ref T itemOutput)
        {
            if (AimedToBeDisposed)
            {
                return false;
            }

            bool itemProcessingAlreadyTimedOut;

            //Stopwatch sw = Stopwatch.StartNew();

            lock (_endItemProcessingLock)
            {
                _asyncMutexEndItemProcessingStarted.Set();
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();

                // if waitone with zero is true, it was not blocking before, so it was released by timeout
                itemProcessingAlreadyTimedOut = _asyncMutexFinal.WaitOne(0, false);

                if (AimedToBeDisposed)
                    return false;

                if (!itemProcessingAlreadyTimedOut)
                {
                    var assigned = false;
                    var item = default(T);

                    if (_actualItemCarrier != null)
                        lock (_actualItemCarrierSync)
                        {
                            if (_actualItemCarrier != null)
                            {
                                item = _actualItemCarrier._item;
                                assigned = true;

                                //ItemCarrier<T> icTmp = _actualItemCarrier;

                                try
                                {
                                    _actualItemCarrier.SetProcessingState(PQProcessingState.Processed);
                                }
                                catch (Exception e)
                                {
                                    HandledExceptionAdapter.Examine(e);
                                }
                                _actualItemCarrier = null;
#if DEBUG
                                /*if (typeof(T).Name.Contains("NodeDataObject"))
                                {
                                    Console.WriteLine("WARN: AIC null EndItemProc("+this.GetHashCode()+"/"+icTmp._processingId+") ");
                                }*/
#endif
                            }
                        }

                    if (assigned)
                    {
                        /*
                        DateTime time = DateTime.Now;
                        string tmpstr = item == null ? "NULL" : item.ToString();
                        lock (GeneralLock._sync)
                        {
                            System.Diagnostics.Debug.WriteLine("END ITEM (" + time.ToString("mm.ss.fffffff") + ") " + tmpstr + "; " + this.GetHashCode().ToString());
                        }
                        */

                        if (!releaseAfterResult)
                            _asyncMutexFinal.Set();

                        itemOutput = item;

                        if (null != ItemProcessingResult)
                            try
                            {
                                ItemProcessingResult(item, success, state);
                            }
                            catch (Exception e)
                            {
                                HandledExceptionAdapter.Examine(e);
                            }

                        if (releaseAfterResult)
                            _asyncMutexFinal.Set();
                    }

                }
                /*
            else
            {
                DateTime time = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("Already timed out(" + time.ToString("mm.ss.ffffff") + "); " + this.GetHashCode());
            }
            */
            }


            /*long took = sw.ElapsedMilliseconds;

            if (took > 50)
            {
                took *= 10;
            }*/



            return !itemProcessingAlreadyTimedOut;
        }

        /// <summary>
        /// implies object == null
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public bool EndItemProcessing(bool success)
        {
            var itemRef = default(T);
            return EndItemProcessing(success, null, true, ref itemRef);
        }

        /// <summary>
        /// implies success == true
        /// </summary>
        /// <returns></returns>
        public bool EndItemProcessing()
        {
            var itemRef = default(T);
            return EndItemProcessing(true, null, true, ref itemRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _toStringCached ??
                (_toStringCached = "PQasync<" + typeof(T).FullName + ">");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKeys"></param>
        /// <param name="queuePickMode"></param>
        public AsyncProcessingQueue(IEnumerable queueKeys, PQPickQueueMode queuePickMode)
            : base(queueKeys, queuePickMode, false, ThreadPriority.Normal, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        public AsyncProcessingQueue(bool useThreadPool, ThreadPriority threadPriority)
            : base(null, PQPickQueueMode.RoundRobin, useThreadPool, threadPriority, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        /// <param name="threadNamePrefix"></param>
        public AsyncProcessingQueue(bool useThreadPool, ThreadPriority threadPriority, string threadNamePrefix)
            : base(null, PQPickQueueMode.RoundRobin, useThreadPool, threadPriority, threadNamePrefix)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public AsyncProcessingQueue()
            : base(null, PQPickQueueMode.RoundRobin, false, ThreadPriority.Normal, null)
        {
        }
    }
}
