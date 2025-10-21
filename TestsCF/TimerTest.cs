using System;
using Contal.IwQuickCF;
using Contal.IwQuickCF.Threads;
using System.Diagnostics;
using System.Threading;
using Contal.IwQuickCF.Sys.Microsoft;

namespace TestsCF
{
    internal class TimerTest : ASingleton<TimerTest>
    {

        public void Run()
        {
            //SimpleSNTP.Singleton.IPAddresses = new IPAddress[]
            //{
            //    IPAddress.Parse("10.0.0.1")
            //};
            //SimpleSNTP.Singleton.Start();
            //SimpleSNTP.Singleton.SynchronizeNow();
            //DebugHelper.NOP(SimpleSNTP.Singleton.TimeWasSynchronized);
            //SystemTime.SynchronizeMiliseconds();

            //SafeThread.StartThread(OverhaulThread);
            //SafeThread.StartThread(OverhaulThread2,ThreadPriority.AboveNormal);

            
            Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + " start");
            //var st = TimerManager.Static.StartTimer(0, false, OnTimerEvent);
            //var st = TimerManager.Static.StartTimeout(50, OnTimerEvent);
            Thread.Sleep(3000);
            //st.StopTimer();
        }

// ReSharper disable once UnusedMember.Local
        private void OverhaulThread2()
        {

            Thread.Sleep(5000);
            double x = Environment.TickCount;
            for (int i = 0; i > -1; i++)
            {
                x += 1 + Math.Sqrt(x);
                if (i%1000 == 0)
                    Thread.Sleep(5);
            }
        }


        private
// ReSharper disable once UnusedMember.Local
            void OverhaulThread()
        {
            
            //double x = Environment.TickCount;
            DateTime st = SystemTime.GetSystemTime();
            int count999 = 0;
            for (int i = 0; i < 50000000; i++)
            {
                //x += 1 + Math.Sqrt(x);
               
                {
                    var now = SystemTime.GetSystemTime();
                    if (now < st)
                    {
                        Debug.WriteLine("Time travel " + st.ToString("hh:mm:ss.fff") + "->" +
                                        now.ToString("hh:mm:ss.fff"));
                    }
                    /*else
                    {
                        

                        //if ((now - st).TotalMilliseconds > 1)
                        //{
                        //    Debug.WriteLine("Context diff " + st.ToString("hh:mm:ss.fff") + "->" + now.ToString("hh:mm:ss.fff"));
                        //}
                    }*/

                    if (now.Millisecond == 999)
                    {
                        count999++;

                        if (count999 > 1)
                            Debug.WriteLine("Too many 999 "+ count999 + " "+ st.ToString("hh:mm:ss.fff") + "->" + now.ToString("hh:mm:ss.fff"));
                    }
                    else
                        count999 = 0;

                    st = now;
                }
            }

        }

        private int _i = 0;

// ReSharper disable once UnusedMember.Local
        private bool OnTimerEvent(TimerCarrier timerCarrier)
        {

            Interlocked.Increment(ref _i);

            Debug.WriteLine(
                "What="+timerCarrier.Description+         
                " isRetriedTimeout="+timerCarrier.IsRetriedTimeout+         
                " Id="+timerCarrier.Id + " tick="+_i+" tickId="+timerCarrier.TickId+
                " isFirstTick="+timerCarrier.IsFirstTick+ " "                
                + DateTime.Now.ToString("mm:ss.fff") + 
                " " + timerCarrier.RemainingMiliseconds+" "+Thread.CurrentThread.ManagedThreadId.ToString("X8"));

            //Thread.Sleep(20);


            bool ret = true;
            //if (_i > 5)
            //{
            //    //Thread.Sleep(200);
            //    ret = false;
            //}


            Debug.WriteLine("Tick end");


            if (_i < 10)
            {
                ret = false;
                   // Thread.Sleep(50);
                if (timerCarrier.IsTimeout)
                    Debug.WriteLine("Resuming at " + _i);
                else
                    Debug.WriteLine("Stopping at " + _i);
                //timerCarrier.StopTimer();
            }



            return ret;
        }
    }

}
