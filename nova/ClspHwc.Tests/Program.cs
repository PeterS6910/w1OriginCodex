using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Contal.Drivers.InterCommDrivers;
using Contal.IwQuickCF;

namespace ClspHwc.Tests
{
    public class ClspHwcTests
    {
        private void Run()
        {
            InterComm interComm = new InterComm();

            try
            {
                interComm.Start("COM6");
            }
            catch(Exception e)
            {
                DebugHelper.Keep(e);
            }

            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne();
        }

        static void Main(string[] args)
        {
            ClspHwcTests p = new ClspHwcTests();
            p.Run();
        }
    }
}
