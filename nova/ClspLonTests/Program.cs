using System;
using Contal.IwQuickCF;
using Contal.IwQuickCF.Threads;
using System.Threading;
using Contal.Drivers.ClspDrivers;
using Contal.Cgp.NCAS.NodeDataProtocol;
using Contal.Drivers.CardReader;
using Contal.IwQuickCF.Net.Microsoft;
using System.Net;
using Contal.IwQuickCF.Data;
using System.Diagnostics;

namespace ClspLonTests
{
    class Program
    {
        NodeCommunicator nc = null;
        void CpuKiller()
        {
            while (true)
            {
                double x = 2;
                for (int i = 0; i < 200000; i++)
                {
                    x += Math.Sqrt(i + x);

                    if ((i % 100) == 0)
                        Thread.Sleep(1);
                }
            }
        }


        private void Run()
        {
            //SafeThread.StartThread(new DVoid2Void(CpuKiller));

           /* ProcessingQueue<int> pq = new ProcessingQueue<int>();
            pq.ItemProcessing += new DType2Void<int>(pq_ItemProcessing);

            long max = 0;

            for (int i = 0; i < 20000; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                pq.EnqueueP(Environment.TickCount, 1);
                long took = sw.ElapsedMilliseconds;
                if (took > 0)
                {
                    if (took > max)
                        max = took;
                    Console.WriteLine("iteration " + i + " took " + took + "ms, max="+max+"ms");
                }
            }

            DebugHelper.NOP();*/

            /*SNTP sntp = new SNTP();
            try
            {
                sntp.Synchronize(IPAddress.Parse("10.0.0.1"));
            }
            catch
            {
            }*/

            /*ClspCommunicator clspComm = new ClspCommunicator(ClspTransportMode.IndirectViaLonShortStack, null, null);
            clspComm.FrameReceived += new D2Type2Void<SlaveNode, Frame>(clspComm_FrameReceived);

            clspComm.Start(null);*/

            nc = new NodeCommunicator(ClspTransportMode.IndirectViaLonShortStack, null, null);
            nc.FrameReceived += new D2Type2Void<ClspNode, ClspFrame>(nc_FrameReceived);
            nc.NodeAssigned += new DType2Void<ClspNode>(nc_NodeAssigned);
            nc.NodeRenewed += new DType2Void<ClspNode>(nc_NodeRenewed);
            nc.NodeReleased += new DType2Void<ClspNode>(nc_NodeReleased);
            nc.ReportFWVersion += new D2Type2Void<ClspNode, ExtendedVersion>(nc_ReportFWVersion);
            nc.ReportInputCount += new D2Type2Void<ClspNode, uint>(nc_ReportInputCount);
            nc.ReportOutputCount += new D2Type2Void<ClspNode, uint>(nc_ReportOutputCount);
            nc.ReportDeviceInfo += new D3Type2Void<ClspNode, uint, uint>(nc_ReportDeviceInfo);
            nc.InputChanged += new D3Type2Void<ClspNode, byte, Contal.Drivers.LPC3250.InputState>(nc_InputChanged);
            nc.MemoryWarning += new D2Type2Void<ClspNode, int>(nc_MemoryWarning);
            nc.IOTestOk += new DType2Void<ClspNode>(nc_IOTestOk);
            nc.SpecialInputChanged += new D3Type2Void<ClspNode, SpecialInput, Contal.Drivers.LPC3250.InputState>(nc_SpecialInputChanged);
            nc.CROnlineStateChanged += new D3Type2Void<ClspNode, Contal.Drivers.CardReader.CardReader, bool>(nc_CROnlineStateChanged);

            nc.AsyncTimeout = 5000;
            nc.Start(null);

            /*while (true)
            {
                Thread.Sleep(1000);

                foreach (SlaveNode sn in clspComm)
                {
                    if (sn.IsOnline)
                    {
                        clspComm.EnqueueFrame(sn, NodeFrame.ReadMemoryLoad());
                    }
                }
            }*/
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne();
        }

        void nc_IOTestOk(ClspNode param)
        {
            Console.WriteLine("ERROR IO Test OK " + param);
        }

        void clspComm_FrameReceived(ClspNode param1, ClspFrame param2)
        {
            Console.WriteLine(">>> "+param1 + " " + param2);
        }

        void pq_ItemProcessing(int param)
        {
            Thread.Sleep(5000);
        }

        void nc_CROnlineStateChanged(ClspNode param1, Contal.Drivers.CardReader.CardReader param2, bool param3)
        {
            if (param3) 
            {
                CRDisplayCommands crdc = nc.GetCrDisplayCommands(param1.LogicalAddress);
                crdc.ClearAllDisplay(param2);
                crdc.DisplayText(param2, 0, 1, "äôå ÄÅÔ ☺");

                nc.GetCrControlCommands(param1.LogicalAddress).BuzzerMode(param2, Contal.Drivers.CardReader.IndicatorMode.LongPulse);
            }
        }

        bool _outsPerpared = false;

        void nc_ReportOutputCount(ClspNode param1, uint outCount)
        {
            Console.WriteLine("ERROR DEVICE INFO >>> " + param1 + " OUTcount=" + outCount);

            if (outCount > 0)
            {
                try { nc.EnqueueFrame(param1, NodeFrame.OutputConfigLevel(0, 200, 200, false)); }
                catch
                {
                }
                //nc.EnqueueFrame(param1, NodeFrame.ActivateOutput(0, true));
                _outsPerpared = true;
            }

            //for (int i = 0; i < param2; i++)
            //{
            //    if (i % 2 == 0)
            //        nc.EnqueueFrame(param1, NodeFrame.OutputConfigLevel((byte)i, 0, 0, false));
            //    else
            //        nc.EnqueueFrame(param1, NodeFrame.OutputConfigFrequency((byte)i, 60000,60000, 0, 0, true, false));
            //}

            
        }

        void nc_SpecialInputChanged(ClspNode param1, SpecialInput param2, Contal.Drivers.LPC3250.InputState param3)
        {
            Console.WriteLine(param1 + "special input=" + param2 + " changed to " + param3);
        }

        void nc_MemoryWarning(ClspNode node, int load)
        {
            Console.WriteLine(node+" memory load = "+load);
        }

        private const string _ficMark = "firstInputChanged";
        private const int _inputChangedTimeout = 6000;

        void OnInputChangedTimeout(object state)
        {
            if (null != state)
            {
                ClspNode sn = (state as ClspNode);

                try
                {
                    nc.EnqueueFrame(sn, NodeFrame.OutputConfigFrequency(0, 150, 150, 0, 2500, false, false));
                    nc.EnqueueFrame(sn, NodeFrame.SetOutput(0, true));
                    nc.EnqueueFrame(sn, NodeFrame.SetOutput(0, false));

                    nc.EnqueueFrame(sn, NodeFrame.SetDIParams(0, 50, 0, 0, false));
                    nc.EnqueueFrame(sn, NodeFrame.SetDIParams(0, 50, 0, 0, true));
                }
                catch
                {
                }

                Console.WriteLine("ERROR : State InputChanged not received from "+sn+" within "+_inputChangedTimeout+"ms");
            }

            
        }

        void nc_InputChanged(ClspNode node, byte inputIndex, Contal.Drivers.LPC3250.InputState inputState)
        {
            
            if (inputIndex==0 )
            //if (_outsPerpared)
            {

                /*bool state = inputState == Contal.Drivers.LPC3250.InputState.Normal;
                if (state)
                    nc.EnqueueFrame(param1, NodeFrame.ActivateOutput(inputIndex, true));
                else
                    nc.EnqueueFrame(param1, NodeFrame.ForceSwitchOff(inputIndex));*/

                try
                {

                    if (inputState == Contal.Drivers.LPC3250.InputState.Alarm)
                        nc.EnqueueFrame(node, NodeFrame.SetDIParams(inputIndex, 50, 0, 0, false));
                    else
                        nc.EnqueueFrame(node, NodeFrame.SetDIParams(inputIndex, 50, 0, 0, true));

                    //if (node.LogicalAddress == 6)
                    {

                        object o = node.GetParameter(_ficMark);

                        if (o == null)
                        {
                            node.SetParameter(_ficMark, new Timer(OnInputChangedTimeout, node, _inputChangedTimeout, -1));

                            Console.WriteLine("FIRST " + node + " IN=" + inputIndex + " " + inputState);
                        }
                        else
                        {
                            (o as Timer).Change(_inputChangedTimeout, -1);
                            Console.Write(".");
                        }
                    }
                }
                catch
                {
                }
            }

            
        }

        void nc_ReportDeviceInfo(ClspNode param1, uint param2, uint param3)
        {
            Console.WriteLine("ERROR DEVICE INFO >>> "+param1 + " IN=" + param2 + " OUT=" + param3);
        }

        void nc_ReportInputCount(ClspNode param1, uint param2)
        {
            Console.WriteLine("ERROR DEVICE INFO >>> " + param1 + " INcount=" + param2);

            for (int i = 0; i < param2; i++)
            {
                try
                {
                    nc.EnqueueFrame(param1, NodeFrame.SetDIParams((byte)i, 50, 0, 0));
                }
                catch
                {
                }
            }
        }

        void nc_ReportFWVersion(ClspNode param1, ExtendedVersion param2)
        {
            Console.WriteLine("FW " + param2);
        }

        void nc_NodeReleased(ClspNode param)
        {
            Timer t = param.GetParameter<Timer>(_ficMark);
            if (t != null)
            {
                t.Change(-1, -1);
                param.UnsetParameter(_ficMark);
            }
            

            Console.WriteLine("WARNING : NodeReleased " + param);
        }

        void nc_NodeRenewed(ClspNode param)
        {
            
        }

        void nc_NodeAssigned(ClspNode node)
        {
            Console.WriteLine("WARNING : NodeAssigned "+node);

            //nc.EnqueueFrame(param, NodeFrame.ReadFWVersion());
            try
            {
                nc.EnqueueFrame(node, NodeFrame.ReadMemoryLoad());

                nc.EnqueueFrame(node, NodeFrame.SetReportedInputs(0, 1, 2, 3));
                nc.EnqueueFrame(node, NodeFrame.SetOutput(0, false));
            }
            catch
            {
            }
        }

        void nc_FrameReceived(ClspNode param1, ClspFrame param2)
        {
            
        }

        static void Main(string[] args)
        {
            Program ip = new Program();
            ip.Run();
        }
    }
}
