using System;
using System.Globalization;
using Contal.IwQuickCF;
using Contal.IwQuickCF.Sys;
using Contal.IwQuickCF.Threads;
using Contal.IwQuickCF.Data;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace TestsCF
{
    internal class PfsTest : ASingleton<PfsTest>
    {
        private PatchedFileStream _pfs1;
        private PatchedFileStream _pfs2;

        private readonly Random _r = new Random(Environment.TickCount);

        private SyncDictionary<string, int> _counts = new SyncDictionary<string, int>();

        public void Run()
        {
            _pfs1 = PatchedFileStream.Open(
                @"\Storage card\pfs1.txt", 
                System.IO.FileMode.Create,
                System.IO.FileAccess.ReadWrite, 
                System.IO.FileShare.ReadWrite);

            _pfs2 = PatchedFileStream.Open(
                @"\Storage card\pfs2.txt", 
                System.IO.FileMode.Create,
                System.IO.FileAccess.ReadWrite, 
                System.IO.FileShare.ReadWrite);

            

            PatchedFileStream.ErrorOccured += PatchedFileStream_ErrorOccured;

            SafeThread.StartThread(OverhaulTest);

            var st = SafeThread.StartThread(Test);


            /*SafeThread.StartThread(ThreadBody1);
            SafeThread.StartThread(ThreadBody2);*/

            Thread.Sleep(500);
            //st.WaitForExit();

            //SafeThreadStopResolution x = st.Abort();

            //DebugHelper.NOP(x);

            double tmp = Environment.TickCount;
            while (true)
            {
                //tmp += Math.Sqrt(tmp) + 1;
                Thread.Sleep(1000);
            }
        }

        private void OverhaulTest()
        {
            double tmp = Environment.TickCount;
            while (true)
            {
                tmp += Math.Sqrt(tmp) + 1;
                
            }
        }

        private void Test()
        {

            PatchedFileStream pfs = PatchedFileStream.Open(@"\Storage card\pfs.txt", FileMode.Create, FileAccess.ReadWrite,
                FileShare.Read);

            byte[] buf = new byte[64];
            

            try
            {
                pfs.Read(buf, 0, buf.Length);
            }
            catch
            {

            }

            //Thread.Sleep(2000);

            try
            {
                pfs.Close();
            }
            catch
            {

            }

            pfs.Dispose();

            GC.Collect();
        }

        void PatchedFileStream_ErrorOccured(string fileName, PatchedFileStream.FileOperation fileOperation, Exception error)
        {
            int c = 0;

            _counts.SetValue(
                fileName,
                key =>
                {
                    if (error == null)
                        // should not happen
                        DebugHelper.NOP();
                    c = 1;
                    return 1;
                },
                (key, value) =>
                {
                    if (error == null)
                        c = value - 1;
                    else
                        c= value + 1;

                    if (c > 1)
                        DebugHelper.NOP();

                    return c;
                },
                null

                );
                
                

            Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff")+" FILE " + fileName + " op=" + fileOperation + " err=" + 
                (error != null ? "ON_" : "off")+ " c="+c);
        }



        private void ThreadBody1()
        {
            int i = 0;
            ByteDataCarrier bdcRead = new ByteDataCarrier(32);
            while (true)
            {
                i++;
                if (i%2 == 0)
                {

                    var bdc = new ByteDataCarrier(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    try
                    {
                        _pfs1.Write(bdc.Buffer, 0, bdc.ActualSize);
                    }
                    catch
                    {

                    }
                }
                else
                {


                    try
                    {
                        _pfs1.Seek(_r.Next((int) _pfs1.Length), System.IO.SeekOrigin.Begin);
                        _pfs1.Read(bdcRead.Buffer, 0, bdcRead.Length);
                    }
                    catch
                    {
                        
                    }
                }

                Thread.Sleep(_r.Next(10));
            }
        }

        private void ThreadBody2()
        {
            int i = 0;
            ByteDataCarrier bdcRead = new ByteDataCarrier(32);
            while (true)
            {
                i++;
                if (i % 2 == 0)
                {

                    var bdc = new ByteDataCarrier("2 "+DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    try
                    {
                        _pfs2.Write(bdc.Buffer, 0, bdc.ActualSize);
                    }
                    catch
                    {

                    }
                }
                else
                {


                    try
                    {
                        _pfs2.Seek(_r.Next((int)_pfs1.Length), System.IO.SeekOrigin.Begin);
                        _pfs2.Read(bdcRead.Buffer, 0, bdcRead.Length);
                    }
                    catch
                    {

                    }
                }

                Thread.Sleep(_r.Next(10));
            }
        }
    }
}
