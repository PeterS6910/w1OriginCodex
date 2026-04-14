using System;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using System.Threading;
using Contal.IwQuick.Threads;

namespace Tests
{
    public class PQWaitingTest : ASingleton<PQWaitingTest>
    {
        private ProcessingQueue<string> _pq;
        private readonly Random _random = new Random(Environment.TickCount);

        public void Run()
        {
            _pq = new ProcessingQueue<string>();

            _pq.ItemProcessing += PqItemProcessing;



            while (true)
            {

                ItemCarrier<string> a = _pq.Enqueue(DateTime.Now.ToString("mm:ss.fff") + " " + Environment.TickCount,
                    PQEnqueueFlags.ExpectedWaitingToFinish);

                SafeThread t = SafeThread.StartThread(() =>
                {
                    bool success = a.Wait(1);
                    Console.WriteLine("Wait : " + success);
                    //a.Dispose();
                }
            );


                Thread.Sleep(_random.Next(20));
            }
        }

        void PqItemProcessing(string parameter)
        {
            Console.WriteLine(DateTime.Now.ToString("mm:ss.fff") + " >>> " + parameter + " " + _pq.Count);
            Thread.Sleep(_random.Next(5));
        }
    }
}
