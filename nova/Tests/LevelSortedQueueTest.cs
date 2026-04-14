using Contal.IwQuick.Data;
using System;
using Contal.IwQuick;
using System.Diagnostics;
using System.Collections.Generic; 
namespace Tests
{
    class LevelSortedQueueTest:ASingleton<LevelSortedQueueTest> 
    { 
        private readonly Random _random = new Random(Environment.TickCount);

        private bool Evaluate(IEnumerable<long> array)
        {
            if (null == array)
                return false;

            long tmp = -1;
            bool wasTopMost = false;

            foreach (var l in array)
            {
                bool isTopMost = (l % 2 == 0);

                if ((l < tmp && !wasTopMost)
                    ||
                    (wasTopMost && isTopMost && l > tmp &&
                    (l & 0x0000FF0000000000) != (tmp & 0x0000FF0000000000)))
                {
                    Debugger.Break();
                    return false;
                }

                tmp = l;
                wasTopMost = isTopMost;
            }

            return true;
        }

        public void Run()
        {
            LevelSortedQueue<long> _lsq = new LevelSortedQueue<long>(100);

            //Random r = new Random(Environment.TickCount);

            int k = 0;
            while (true)
            {
                if (_random.Next(2) == 1)
                {
                    uint prio = (uint)_random.Next(200);

                    bool topMost = (_random.Next(5) == 3);

                    long tmp = (topMost ? 0 : 1) + (k << 8) + ((long)prio << 32);
                    _lsq.Enqueue(tmp, prio, topMost);

                    Console.WriteLine("Enqueued : 0x" + tmp.ToString("X16") + " / " + _lsq.Count);
                }
                else
                {
                    long tmp;
                    if (_lsq.Dequeue(out tmp))
                        Console.WriteLine("Dequeued : 0x" + tmp.ToString("X16") + " / " + _lsq.Count);
                    else
                        Console.WriteLine("Nothing to dequeue / " + _lsq.Count);


                }

                if (!Evaluate(_lsq.QueueSnapshot))
                    Console.WriteLine("Consistency failue");

                if (++k % 100000 == 0)
                {
                    Console.Write("Press enter to continue ...");
                    Console.ReadLine();
                }
            }
        }
    }
}
