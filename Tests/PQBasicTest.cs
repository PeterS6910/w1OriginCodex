using System;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.IwQuick.Data;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
    public class PQBasicTest: ASingleton<PQBasicTest>
    {
        private ProcessingQueue<string> _pq;

        private readonly Random _random = new Random(Environment.TickCount);

        public void Run()
        {
            _pq = new ProcessingQueue<string>(PQMode.Synchronous, false,
                ThreadPriority.Normal);

            _pq.ItemProcessing += _pq_ItemProcessing;

            for(int i=0;i<50;i++)
            {
                Thread.Sleep(1);
                uint prio = (uint)_random.Next(10);

                string item = DateTime.Now.ToString("hh:mm:ss.fff") + " id=" + i.ToString("D3") + " prio=" + prio;

                Console.WriteLine("Enquenued item="+item+ " pid="+ _pq.Enqueue(item ,PQEnqueueFlags.None, prio).ProcessingId);

                
            }

            Thread.Sleep(1000);
            DebugHelper.NOP();
        }

        void _pq_ItemProcessing(string parameter)
        {
            Console.WriteLine(parameter);
            Debug.WriteLine(parameter);
            Thread.Sleep(_random.Next(20));
        }
    }
}
