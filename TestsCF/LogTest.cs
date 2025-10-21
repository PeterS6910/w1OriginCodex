using System;
using Contal.IwQuickCF;
using Contal.IwQuickCF.Sys;
using System.Globalization;
using Contal.IwQuickCF.Sys.Microsoft;

namespace TestsCF
{
    internal class LogTest : ASingleton<LogTest>
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

            Log l = new Log("TestCF", true, false, null, 0);
            l.ShowThreadId = true;

            RotaryFileLog rfl = new RotaryFileLog(
                @"\Storage card\test.txt",
                50,
                64*1024, 3000);

            l.SetIntoFileLog(rfl);


            Random r = new Random(Environment.TickCount);
            int count = 0;
            while (true)
            {

                l.Info(0, () =>
                {
                    return string.Format(
                        "id=" + (count++).ToString("D4") +
                        " load=" + SystemInfo.Singleton.MemoryLoad +
                        "% logC=" + l.Count + " " + "{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));


                });

                //if (count % 50 == 0)
                //    Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

                //if (count % 60 == 1)
                //    Thread.CurrentThread.Priority = ThreadPriority.Normal;

                //Thread.Sleep(r.Next(20));
            }
        }
    }
}
