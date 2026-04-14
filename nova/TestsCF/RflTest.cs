using Contal.IwQuickCF;
using Contal.IwQuickCF.Threads;
using System.Threading;
using Contal.IwQuickCF.Data;
using Contal.IwQuickCF.Sys;
using System.IO;
using System.Diagnostics;
using Contal.IwQuickCF.Crypto;
using System;

namespace TestsCF
{
    class RflTest : ASingleton<RflTest>
    {
        class AInfo
        {
            internal uint _ch;
            internal long _took;
            internal ManualResetEvent _mre = new ManualResetEvent(false);
        }


        public void Run()
        {
            
            RotaryFileLog rfl = new RotaryFileLog(@"\Storage card\test.txt", 50,32*1024,3000);

            Random r = new Random(Environment.TickCount);
            /*
            byte[] xxx = new byte[65500];
            for (int i = 0; i < xxx.Length; i++)
                xxx[i] = (byte) i;*/

            while (true)
            {
                for (int i = 0; i < r.Next(50); i++)
                {
                    /*byte[] subPart = new byte[r.Next(xxx.Length)];

                    Array.Copy(xxx, subPart, subPart.Length);

                    rfl.Write(subPart);*/

                    try
                    {
                        rfl.WriteLine(i + " " + DateTime.Now + " " +
                                      Thread.CurrentThread.ManagedThreadId);
                    }
                    catch
                    {
                        
                    }

                    //Console.Write(".");
                    //Thread.Sleep(r.Next(20));
                }

                Thread.Sleep(r.Next(100));
            }

            /*
            PatchedFileStream pfs = new PatchedFileStream(@"\Storage card\test.dat", System.IO.FileMode.Create,
                FileAccess.ReadWrite, FileShare.Read);

            for (int i = 0; i < 6500; i++)
            {
                byte[] tmp = Encoding.UTF8.GetBytes(i + "\r\n");

                Array.Resize(ref tmp, 33000);

                pfs.Write(tmp, 0, tmp.Length);
            }

            pfs.Close();*/



            SyncDictionary<int, AInfo> d = new SyncDictionary<int, AInfo>();


            Action lambda =
                () =>
                {
                    int ctid = Thread.CurrentThread.ManagedThreadId;
                    AInfo ai = new AInfo();

                    d[ctid] = ai;

                    PatchedFileStream pfs = PatchedFileStream.Open(@"\Storage card\test.dat", FileMode.Open,
                        FileAccess.Read, FileShare.Read);

                    Stopwatch sw = Stopwatch.StartNew();

                    uint ch = Crc32.ComputeChecksum(512, pfs);

                    long took = sw.ElapsedMilliseconds;

                    ai._ch = ch;
                    ai._took = took;

                    PatchedFileStream pfsOut = PatchedFileStream.Open(@"\Storage card\testReplica.dat", FileMode.Create,
                        FileAccess.ReadWrite, FileShare.Read);

                    pfs.Seek(0, SeekOrigin.Begin);
                    byte[] tmpBuf = new byte[512];

                    int read = -1;
                    while (read != 0)
                    {
                        read = pfs.Read(tmpBuf, 0, tmpBuf.Length);

                        pfsOut.Write(tmpBuf, 0, read);
                    }

                    pfsOut.Seek(0, SeekOrigin.Begin);

                    uint ch2 = Crc32.ComputeChecksum(32768, pfsOut);

                    if (ch2 != ch)
                        DebugHelper.NOP();

                    ai._mre.Set();
                };

            SafeThread.StartThread(lambda);
            //SafeThread.StartThread(lambda);

            while (d.Count < 2)
                Thread.Sleep(100);

            foreach (AInfo a in d.Values)
            {
                a._mre.WaitOne();
            }

            var x = 
                d.ValuesSnapshot;

            Console.WriteLine("thr1>> "+    x[0]._took);
            Console.WriteLine("thr2>> " + x[1]._took);
        }
    }
}
