using System;
using Contal.IwQuickCF;
using Contal.IwQuickCF.Data;
using System.Threading;
using Contal.IwQuickCF.Threads;

namespace TestsCF
{
    public class PQWaitingTest : ASingleton<PQWaitingTest>
    {
        private ProcessingQueue<string> _pq;
        private readonly Random _random = new Random(Environment.TickCount);

        public void Run()
        {
            _pq = new ProcessingQueue<string>(PQMode.Asynchronous);
            _pq.AsynchronousTimeout = 2000;
            _pq.ItemProcessing += PqItemProcessing;
            _pq.ItemProcessingTimedOut += _pq_ItemProcessingTimedOut;


            int j = 0;
            while (true)
            {
                uint p = (uint)_random.Next(20);

                ItemCarrier<string> a = _pq.Enqueue(
                    DateTime.Now.ToString("mm:ss.fff") + " enqid=" + (j++) +" prio"+p,
                    PQEnqueueFlags.ExpectedWaitingToFinish | PQEnqueueFlags.WithPriority,
                    p);

                SafeThread t = SafeThread.StartThread(() =>
                {
                    bool success = a.Wait(5);

                    if (!success)
                    {
                        _pq.EndItemProcessing(true);
                        PQProcessingState lastState= a.ProcessingState;

                        while (true)
                        {
                            success = a.Wait(200);
                            if (!success && a.ProcessingState == PQProcessingState.Enqueued)
                            {
                                lastState = PQProcessingState.Enqueued;
                                continue;
                            }

                            if (success)
                                break;

                            if (lastState == PQProcessingState.Enqueued)
                            {
                                lastState = a.ProcessingState;
                                continue;
                            }

                            DebugHelper.NOP();

                        }
                    }

                    Console.WriteLine("Wait : " + success);
                    //a.Dispose();
                },
                    ThreadPriority.Normal,false );


                Thread.Sleep(_random.Next(50));
            }
        }

        void _pq_ItemProcessingTimedOut(string param)
        {
            Console.WriteLine("PROCESSING_TIMEDOUT " + DateTime.Now.ToString("mm:ss.fff") + " >>> " + param + " " + _pq.Count);
        }

        void PqItemProcessing(string parameter)
        {
            Console.WriteLine("PROCESS_BEGIN "+DateTime.Now.ToString("mm:ss.fff") + " >>> " + parameter + " " + _pq.Count);
            int sleep = _random.Next(60);
            Console.WriteLine("Sleeping for "+sleep);
            Thread.Sleep(sleep);
            Console.WriteLine("PROCESS_END " + DateTime.Now.ToString("mm:ss.fff") + " >>> " + parameter + " " +
                              _pq.Count);
        }
    }
}
