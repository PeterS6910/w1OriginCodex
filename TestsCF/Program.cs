using System.Threading;
using Contal.IwQuickCF.Sys.Microsoft;

namespace TestsCF
{
    class Program
    {
        
        static void Main()
        {
            //PQWaitingTest.Singleton.Run();

            //Process p = new Process();
            //p.StartInfo.FileName = "CEKickstart.exe";
            //p.Start();

            //RflTest.Singleton.Run();

            //TimerTest.Singleton.Run();

            //PfsTest.Singleton.Run();

            SystemTime.SetLocalTime(2014,10,26,2,59,40,0);

            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne();


            /*long sumTook1 = 0;
            long sumTook2 = 0;

            int i;
            for (i = 0; i < 5000; i++)
            {
                Stopwatch sw1 = Stopwatch.StartNew();

                byte[] buf = new byte[512];

                Random _r = new Random(Environment.TickCount);

                for (int j = 0; j < buf.Length; j++)
                {
                    buf[j] = (byte)_r.Next(256);
                }

                Stopwatch sw2 = Stopwatch.StartNew();
                Array.Reverse(buf);
                Array.Sort(buf);
                long took1 = sw1.ElapsedMilliseconds;
                long took2 = sw2.ElapsedMilliseconds;

                sumTook1 += took1;
                sumTook2 += took2;
            }

            float avg1 = sumTook1/(float)i;
            float avg2 = sumTook2 / (float)i;*/

            //Console.WriteLine("avg1="+avg1+" avg2="+avg2+" i="+i);
        }
    }
}
