using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Contal.IwQuickCF.Crypto;
using Contal.IwQuickCF.Net;
using System.Net;
using Contal.Drivers.ClspDrivers;
using Contal.IwQuickCF.Threads;
using System.Threading;
using System.Diagnostics;

namespace Contal.Cgp.NCAS.DCUfromCCUTester
{
    class Program
    {
        /*
        string buffer = "Let's try shit, if it'll be fastr than cumulated call";
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
                Console.WriteLine(buffer);
            long took = sw.ElapsedMilliseconds;

            StringBuilder sb = new StringBuilder(String.Empty);

            for (int i = 0; i < 1000; i++) 
            {
                sb.Append(buffer);
                sb.Append("\r\n");
            }

            buffer = sb.ToString();

            sw = Stopwatch.StartNew();
            Console.WriteLine(buffer);
            took = sw.ElapsedMilliseconds;

            took *= 10;
         */ 
        static void Main(string[] args)
        {
            /*try
            {
                SimpleSNTP.Singleton.SynchronizeNow(new IPAddress[] { IPAddress.Parse("10.0.0.1") });
            }
            catch (Exception e)
            {
                Console.WriteLine("SNTP error : "+e.Message);

                
            }*/

            /*
            Frame f1 = Frame.Create(ProtocolId.ProtoAccess, 0x12, 0x1,0x2,0x3);
            Frame f2 = Frame.Create(ProtocolId.ProtoAccess, 0x34, 0x3, 0x4, 0x5, 0x6, 0x7);
            Frame f3 = Frame.Create(ProtocolId.ProtoAccess, 0x56, 0x5, 0x6, 0x7, 0x8);

            Frame sf = Frame.CreateStacked(f1, f2);
            sf = Frame.CreateStacked(sf, f3);

            f1 = Frame.Create(ProtocolId.ProtoAccess, 0x12);
            f2 = Frame.Create(ProtocolId.ProtoAccess, 0x34, 0x3, 0x4, 0x5, 0x6, 0x7);
            f3 = Frame.Create(ProtocolId.ProtoAccess, 0x56);

            sf = Frame.CreateStacked(f1, f2);
            sf = Frame.CreateStacked(sf, f3);*/

            //SafeThread pkT = SafeThread.StartThread(new Contal.IwQuickCF.DVoid2Void(ProcessorKillerThread));

            Communicator comm = new Communicator();
            comm.Start();
        }

        static void ProcessorKillerThread() {
            double l = 1;
            int j = 0;
            Random r = new Random(Environment.TickCount);
            while (true)
            {
                l *= l++;
                l = Math.Sqrt(l);
                j++;
                /*if (j % 3000 == 0)
                    Thread.Sleep(r.Next(10) + 1);*/
            }
        }
    }
}
