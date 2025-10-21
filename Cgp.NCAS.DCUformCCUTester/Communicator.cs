using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Contal.Drivers.ClspDrivers;
using Contal.IwQuickCF.Net;
using Contal.IwQuickCF.Sys.Microsoft;
using Contal.IwQuickCF.Threads;
using Contal.IwQuickCF;
using Contal.Cgp.NCAS.NodeDataProtocol;
using Contal.Drivers.LPC3250;
using Contal.Cgp.NCAS.GlobalsCF;
using System.Threading;
using Contal.Drivers.CardReader;
using Contal.IwQuickCF.Data;
using Contal.Drivers.InterCommDrivers;
using Contal.Cgp.NCAS.DefinitionsCF;
using Timer = System.Threading.Timer;

namespace Contal.Cgp.NCAS.DCUfromCCUTester
{
    class Communicator
    {
        private NodeCommunicator _nodeComm;
        private ManualResetEvent _wait;

        void CpuKiller()
        {
            double i = 1;
            while (true)
            {
                for (int j = 0; j < 5000; j++)
                {
                    i += Math.Sqrt(i + j);
                }

                Thread.Sleep(5);
            }
        }

        public Communicator()
        {
            SystemTime.SynchronizeMiliseconds();

            var mbt = MainBoard.Variant;

            ErstControl.LooseReset();
            ErstControl.KeepReset();

            InterComm interComm = new InterComm();
            ICUpgradeResult res = ICUpgradeResult.Unknown;

            //ICUpgradeContext uc = new ICUpgradeContext(interComm, 1313, @"\NandFlash\clsp_hwc.bin");

            ////SafeThread.StartThread(CpuKiller);

            try
            {
                //uc.UpgradeFile = @"\NandFlash\clsp_hwc.bin";

                //switch (mbt)
                //{
                //    case MainBoardType.CCU0_RS485:
                //        res = uc.Upgrade("COM6");
                //        break;
                //    case MainBoardType.CCU40:
                //        res = uc.Upgrade("COM4");
                //        break;
                //}
            }
            catch
            {
            }


            switch (mbt)
            {
                case MainBoardVariantG7.CCU0_RS485:
                    _nodeComm = new NodeCommunicator(ClspTransportMode.IndirectViaClspHwc, interComm, null);
                    break;
                case MainBoardVariantG7.CCU12:
                case MainBoardVariantG7.CCU40:
                    _nodeComm = new NodeCommunicator(ClspTransportMode.DirectViaNativeSerialPort, null, null);
                    break;
                case MainBoardVariantG7.CCU0_ECHELON:
                    _nodeComm = new NodeCommunicator(ClspTransportMode.IndirectViaLonShortStack, interComm, null);
                    break;
            }
            
            
            //_nodeComm = new NodeCommunicator(Clsp485CommunicationMode.DirectViaNativeSerialPort, null, null);
            _nodeComm.CommandACK += _nodeComm_CommandACK;
            _nodeComm.CommandNACK += _nodeComm_CommandNACK;
            _nodeComm.CommandTimeout += _nodeComm_CommandTimeout;
            _nodeComm.NodeAssigned +=_nodeComm_NodeAssigned;
            _nodeComm.NodeReleased += (_nodeComm_NodeReleased);
            _nodeComm.NodeRenewed += (_nodeComm_NodeRenewed);
            _nodeComm.UnknownFrame += (_nodeComm_UnknownFrame);

            _nodeComm.DIPChanged += _nodeComm_DIPChanged;
            _nodeComm.DSMChanged += _nodeComm_DSMChanged;
            _nodeComm.InputChanged += _nodeComm_InputChanged;
            _nodeComm.OutputChanged += _nodeComm_OutputChanged;
            _nodeComm.OutputLogicChanged += _nodeComm_OutputLogicChanged;
            _nodeComm.ReportInputCount += _nodeComm_ReportInputCount;
            _nodeComm.ReportOutputCount += _nodeComm_ReportOutputCount;
            _nodeComm.SpecialInputChanged += _nodeComm_SpecialInputChanged;

            _nodeComm.CRCardSwiped += _nodeComm_CRCardSwiped;
            _nodeComm.CRSpecialKeyPressed += _nodeComm_CRSpecialKeyPressed;
            _nodeComm.CROnlineStateChanged += _nodeComm_CROnlineStateChanged;
            _nodeComm.CRCodeSpecified += _nodeComm_CRCodeSpecified;
            _nodeComm.CRCodeTimedOut += _nodeComm_CRCodeTimedOut;
            _nodeComm.CRMenuCancelled += _nodeComm_CRMenuCancelledOrTimedOut;
            //_nodeComm.CRMenuTimedOut += new D2Type2Void<ISlaveNode, CardReader>(_nodeComm_CRMenuCancelledOrTimedOut);

            _nodeComm.CRUpgradeProgress += _nodeComm_CRUpgradeProgress;
            _nodeComm.CRUpgradeResult += _nodeComm_CRUpgradeResult;

            _nodeComm.MemoryWarning += _nodeComm_MemoryWarning;

            _nodeComm.DebugSequenceResponse += _nodeComm_DebugSequenceResponse;

            _nodeComm.DSMActivationChanged += _nodeComm_DSMActivationChanged;

            /* Upgrade testing */
            _nodeList = new List<int>();
            _connectionList = new Dictionary<byte, SimpleGeneralTcpConnection>();
            _tcpServer = new SimpleTcpServer();
            _tcpServer.DataReceived += _tcpServer_DataReceived;

            _nodeComm.UpgradeDone += _nodeComm_UpgradeDone;
            _nodeComm.UpgradeError += _nodeComm_UpgradeError;
            _nodeComm.UpgradeProgress += _nodeComm_UpgradeProgress;
            _nodeComm.UpgradeTimeout += _nodeComm_UpgradeTimeout;

            /* IO HW test */
            _nodeComm.IOTestOk += _nodeComm_IOTestOk;
            _nodeComm.IOTestProgress += _nodeComm_IOTestProgress;
            _nodeComm.IOTestTimeout += _nodeComm_IOTestTimeout;

            ClspNode.GlobalMaxRetryCount = 150;

            _nodeComm.GlobalMaxRetryCount = 50;
            _nodeComm.NodeResponseTimeout = 10;
            _nodeComm.DelayBetweenSuccessfulPolling = 10;
            _nodeComm.NodeResponseLatency = 2;
            _nodeComm.NodeUnpolledTimeout = 5;
            _nodeComm.MaxNodeLookupSequence = 3;
            _nodeComm.AsyncTimeout = 10000;
        }

        void _nodeComm_DSMActivationChanged(IClspNode param1, bool param2)
        {
            Console.WriteLine("WARNING\tDSM " + (param2 ? "started" : "stopped") + "\r\n");
        }


        void _nodeComm_CRUpgradeResult(IClspNode sn, CardReader cr, CRUpgradeResult param3, Exception param4)
        {
            string tmp = cr + " on " + sn + " finished upgrading with result " + param3;
            if (param4 != null)
                tmp += "\r\n" + param4.Message;
            Console.WriteLine(tmp);

            Send2All(tmp);
        }

        void _nodeComm_CRUpgradeProgress(IClspNode sn, CardReader cr, int param3)
        {
            string tmp = cr + " on " + sn + " upgrade progress : " + param3;
            Console.WriteLine(tmp);

            Send2All(tmp);
        }

        #region IO hw test 
        void _nodeComm_IOTestTimeout(IClspNode node, int param)
        {
            Log.Singleton.Error("IO HW Test timeout: " + param);
        }

        void _nodeComm_IOTestProgress(IClspNode node, int param)
        {
            Log.Singleton.Info("IO HW Test progress: " + param);
        }

        void _nodeComm_IOTestOk(IClspNode node)
        {
            Log.Singleton.Info("IO HW Test done");
        }
        #endregion

        void _nodeComm_CRMenuCancelledOrTimedOut(IClspNode param1, CardReader param2,bool x)
        {
            Thread.Sleep(200);
            RunCrMenu(param1, param2);

        }

        void _nodeComm_MemoryWarning(IClspNode node, int param)
        {
            //_selectedNode = param.LogicalAddress;

            Log.Singleton.Warning("Warning : Memory load ="+param);
        }

        void _nodeComm_CRCodeTimedOut(IClspNode param1, CardReader param2)
        {
            _selectedNode = param1.LogicalAddress;

            Console.WriteLine(param2 + " Code timed out ");

            param2.ParentCommunicator.AccessCommands.WaitingForCard(param2);
        }

        void _nodeComm_CRCodeSpecified(IClspNode sn, CardReader cr, string code)
        {
            _selectedNode = sn.LogicalAddress;

            Console.WriteLine(cr + " Code specified : " + code);

            if (code == "1234" || code == "1397")
            {
                Log.Singleton.Info("Signalling Access Granted");
                OpenDoorEtc(sn);
            }
            else
            {
                if (code == "9999")
                {
                    Send(sn.LogicalAddress, NodeFrame.StopDSM());
                }
                else
                {
                    cr.ParentCommunicator.AccessCommands.Rejected(cr);
                    Thread.Sleep(1000);
                    Send(sn.LogicalAddress, NodeFrame.LooseCardReader(cr.Address));
                }
            }
        }

        private void OpenDoorEtc(IClspNode clspNode)
        {
            _nodeComm.EnqueueFrame(clspNode, NodeFrame.SignalAccessGranted());
            //cr.ParentCommunicator.AccessCommands.Accepted(cr);
            /*try
            {
                CRAccessCommands crac = _nodeComm.GetCrAccessCommands(slaveNode);
                crac.DoorUnlocked(_nodeComm.GetCardReader(slaveNode,1));
                crac.DoorUnlocked(_nodeComm.GetCardReader(slaveNode, 2));
            }
            catch
            {
            }*/
        }

        void _nodeComm_CROnlineStateChanged(IClspNode param1, CardReader param2, bool param3)
        {
            _selectedNode = param1.LogicalAddress;

            string tmp = param2 + " on " + param1;

            Console.WriteLine(tmp);
            Send2All(tmp);

            //System.Diagnostics.Debug.WriteLine(tmp);

            /*

            param2.ParentCommunicator.DisplayCommands.DisplayText(param2, 3, 4, "HELLO");
            param2.ParentCommunicator.DisplayCommands.DisplayText(param2, 3, 5, "OLA");
            param2.ParentCommunicator.DisplayCommands.DisplayText(param2, 3, 6, "Špeciálne znaky");
            param2.ParentCommunicator.DisplayCommands.DisplayText(param2, 3, 7, "Åbro");

            Thread.Sleep(2000);

            if (!param2.ParentCommunicator.AccessCommands.WaitingForCard(param2))
                Console.WriteLine("ERROR : unable to setup waitforcard");*/
        }

        private bool _output2 = true;
        private bool _menuWithOurWithoutYes = true;

        private void RunCrMenu(IClspNode param1, CardReader param2)
        {
            /*_nodeComm.GetCrMenuCommands(param1).StartMenu(param2, 1, _menuWithOurWithoutYes,
                       "HELLO"+Environment.TickCount,
                       "`26 OLA",
                       "`27 Špeciálne znaky",
                       "`28 Cgp-NCAS",
                       "Åbro" + DateTime.Now.Second,
                       "`25 item6",
                       "Item7_789012345678901",
                       "Item8_789012345678901",
                       "položka 9",
                       "premiumus 10",
                       "overlapped 11");

            _menuWithOurWithoutYes = !_menuWithOurWithoutYes;*/
        }

        void _nodeComm_CRSpecialKeyPressed(IClspNode param1, CardReader param2, CRSpecialKey specialKey)
        {
            _selectedNode = param1.LogicalAddress;

            if (specialKey == CRSpecialKey.Up)
            {
                RunCrMenu(param1, param2);
            }

            Send(NodeFrame.SetOutput(2,_output2));
#if DEBUG
            Log.Singleton.Info(param2+" Special key pressed : "+specialKey);
#endif

            _output2 = !_output2;

            /*
            if (specialKey == CRSpecialKey.Lock)
            {
                StartSeqDebug();
            }
            if (specialKey == CRSpecialKey.Unlock)
            {
                StopSeqDebug();
            }*/
        }

        void _nodeComm_CRCardSwiped(IClspNode param1, CardReader param2, string param3, int cardSystemNumber)
        {
            _selectedNode = param1.LogicalAddress;

            Console.WriteLine(param2 + " Card swiped : " + param3);

            //Send(NodeFrame.ActivateOutput(3, false));
            //Send(NodeFrame.ActivateOutput(3, true));
            //if (!param2.ParentCommunicator.AccessCommands.WaitingForPIN(param2))
            //    OpenDoorEtc(param1);
            Send(param1.LogicalAddress, NodeFrame.SignalAccessGranted());
            //Thread.Sleep(10);
            //Send(NodeFrame.ActivateOutput(3, false));
            //Send(NodeFrame.ActivateOutput(3, true));
        }

        public void Start()
        {
            _tcpServer.Start(7777);

            _nodeComm.Start("COM6");

            while (true)
            {
                Console.WriteLine("    "+_seqCountErrors+"   "+_commandTimeoutCount);
                Thread.Sleep(5000);
            }
        }

        void _nodeComm_SpecialInputChanged(IClspNode param1, SpecialInput param2, InputState param3)
        {
            Console.WriteLine("Special input " + param2 + " changed to " + param3);
        }

        void _nodeComm_ReportOutputCount(IClspNode param1, uint param2)
        {
            Console.WriteLine("Reported output count: " + param2);
        }

        void _nodeComm_ReportInputCount(IClspNode param1, uint param2)
        {
            Console.WriteLine("Reported input count: " + param2);
        }

        void _nodeComm_OutputLogicChanged(IClspNode param1, byte param2, bool param3)
        {
            Console.WriteLine("Output " + param2 + " logic changed to " + param3);
        }

        void _nodeComm_OutputChanged(IClspNode param1, byte param2, bool param3)
        {
            Console.WriteLine("Output " + param2 + " state changed to " + param3);
        }

        void _nodeComm_InputChanged(IClspNode param1, byte param2, InputState param3)
        {
            Console.WriteLine("Input " + param2 + " state changed to " + param3);
        }

        void _nodeComm_DSMChanged(IClspNode param1, DoorEnvironmentState param2, DoorEnvironmentStateDetail param3, DoorEnvironmentAGSource source)
        {
            switch (param2)
            {
                case DoorEnvironmentState.Locked:
                    try
                    {
                        /*CRAccessCommands crac = _nodeComm.GetCrAccessCommands(param1);
                        crac.WaitingForCard(_nodeComm.GetCardReader(param1, 1));
                        crac.WaitingForCard(_nodeComm.GetCardReader(param1, 2));*/

                    }
                    catch
                    {
                    }
                    break;
                case DoorEnvironmentState.Opened:
                    try
                    {
                        /*CRAccessCommands crac = _nodeComm.GetCrAccessCommands(param1);
                        crac.DoorOpened(_nodeComm.GetCardReader(param1, 1));
                        crac.DoorOpened(_nodeComm.GetCardReader(param1, 2));*/

                    }
                    catch
                    {
                    }
                    break;
                case DoorEnvironmentState.Intrusion:
                    try
                    {
                        CRAccessCommands crac = _nodeComm.GetCrAccessCommands(param1);
                        //crac.AlarmIntrusion(_nodeComm.GetCardReader(param1, 1));
                        //crac.AlarmIntrusion(_nodeComm.GetCardReader(param1, 2));

                    }
                    catch
                    {
                    }
                    break;
                case DoorEnvironmentState.Ajar:
                    try
                    {
                        /*CRAccessCommands crac = _nodeComm.GetCrAccessCommands(param1);
                        crac.AlarmDoorAjar(_nodeComm.GetCardReader(param1, 1));
                        crac.AlarmDoorAjar(_nodeComm.GetCardReader(param1, 2));*/

                    }
                    catch
                    {
                    }
                    break;
                case DoorEnvironmentState.AjarPrewarning:
                    try
                    {
                        /*CRAccessCommands crac = _nodeComm.GetCrAccessCommands(param1);
                        crac.WarningDoorAjar(_nodeComm.GetCardReader(param1, 1));
                        crac.WarningDoorAjar(_nodeComm.GetCardReader(param1, 2));*/

                    }
                    catch
                    {
                    }
                    break;
                default:
                    try
                    {
                        CRDisplayCommands crdc = _nodeComm.GetCrDisplayCommands(param1);
                        crdc.DisplayText(_nodeComm.GetCardReader(param1, 1), 0, 2, param2.ToString());
                        crdc.DisplayText(_nodeComm.GetCardReader(param1, 1), 0, 3, param3.ToString());
                    }
                    catch
                    {
                    }
                    break;
                
            }
            Console.WriteLine("DSM state changed to " + param2 + ", detail to " + param3 + ", AG source: " + source);
        }

        void _nodeComm_DIPChanged(IClspNode param1, int param2)
        {
            Console.WriteLine("DIP changed to " + param2);
        }

        void _nodeComm_UnknownFrame(IClspNode param1, IClspFrame param2)
        {
            Console.WriteLine("UNKNOWN FRAME");
        }

        void _nodeComm_NodeRenewed(IClspNode param)
        {
            _selectedNode = param.LogicalAddress;

            if (!_nodeList.Contains(param.LogicalAddress))
                _nodeList.Add(param.LogicalAddress);

            string tmp = "--- "+param+" renewed---";
            Console.WriteLine(tmp);
            Send2All(tmp);
        }

        void _nodeComm_NodeReleased(IClspNode param)
        {
            string tmp = "--- "+param+" released---";
            Console.WriteLine(tmp);
            Send2All(tmp);

            if (_nodeList.Contains(param.LogicalAddress))
                _nodeList.Remove(param.LogicalAddress);

            TestingContext tc;
            if (_testingContexts.TryGetValue(param.LogicalAddress,out tc) && null != tc.timer)
                try
                {
                    tc.timer.Change(Timeout.Infinite, Timeout.Infinite);
                    tc.timer.Dispose();
                }
                catch
                {
                }
                finally
                {
                    try { _testingContexts.Remove(param.LogicalAddress); }
                    catch
                    {
                    }
                }

        }

        private class TestingContext
        {
            public Timer timer;
            public int crActionCounter = 0;
        }

        Dictionary<byte, TestingContext> _testingContexts = new Dictionary<byte, TestingContext>();
        Random _r = new Random((int)DateTime.Now.Ticks);
        void _nodeComm_NodeAssigned(IClspNode param)
        {
            string tmp = "--- " + param + " assigned ---";
            Console.WriteLine(tmp);
            Send2All(tmp);

            if (!_nodeList.Contains(param.LogicalAddress))
                _nodeList.Add(param.LogicalAddress);

            _selectedNode = param.LogicalAddress;


            if (param.MasterProtocol == ProtocolId.ProtoAccess)
            {
                //Send(NodeFrame.SetDIParams(2, 50, 0, 0));
                //Send(NodeFrame.SetDIParams(3, 50, 0, 0));
                //Send(NodeFrame.SetBSILevels(243, 271, 307));

                Send(NodeFrame.SetBSILevels(327, 440, 593));
                /*for (int i = 0; i < 4; i++)
                    Send(NodeFrame.SetBSIParams((byte)i, 100, 0, 0, 0, false));*/
                
                //Send(NodeFrame.SetBSIParams(2, 50, 0, 0, 0,true));
                //Send(NodeFrame.SetBSIParams(3, 50, 0, 0, 0));

                //Send(NodeFrame.OutputConfigLevel(1, 0, 0, false));
                //Send(NodeFrame.ActivateOutput(1, false));
                //Send(NodeFrame.OutputConfigLevel(2, 0, 0, false));
                //Send(NodeFrame.ActivateOutput(2, false));
                //Send(NodeFrame.OutputConfigLevel(3, 0, 0, false));
                //Send(NodeFrame.ActivateOutput(3, true));
               
                //Send(NodeFrame.SetTime(IwQuickCF.Sys.Microsoft.SystemTime.GetLocalTime()));

                Send(NodeFrame.SetSensor(SensorType.DoorOpened, 0, false, false, 0, 200));
                Send(NodeFrame.SetActuator(ActuatorType.ElectricStrike, 0, StrikeType.Level, 0, false, 0, 1000));
                Send(NodeFrame.SetPushButton(PushButtonType.Internal, 1, false, false, 0, 0));
                //Send(NodeFrame.SetPushButton(PushButtonType.External, 2, false, false, 0, 0));
                
                Send(NodeFrame.SetSpecialOutput(SpecialOutputType.Sabotage, 1));

                Console.WriteLine("\t\tWARNING\tSETTING DSM TIMINGS");

                Send(NodeFrame.SetTimmings(4000, 5000, 3000, 100, 0));

                Send(
                    NodeFrame.AssignCardReaders(
                    1, 
                    (byte)CRMessageCode.WAITING_FOR_PIN_CODE,
                    4, null, true,
                    
                    0,0,0,null,false)); 

                //Send(NodeFrame.SetImplicitCRCode(2, 0x43, -1, null, false));

                //Send(NodeFrame.SetReportedInputs(0, 1, 2, 3));

                Send(NodeFrame.StartDSM(DoorEnviromentType.Standard));

                //Send(NodeFrame.StopDSM());

                Console.WriteLine("\t\tWARNING\tSTARTING DSM");


                int randomVal = _r.Next(1000) + 1000;

                try
                {
                    //TestingContext tc = new TestingContext();
                    //if (param != null)
                    //    _testingContexts.Add(param.LogicalAddress, tc);
                }
                catch
                {
                }

               // tc.timer = new System.Threading.Timer(new TimerCallback(OnTimer), param, randomVal, Timeout.Infinite);
            }
        }

        bool _output1 = true;
        int _dummy = 0;
        
        void OnTimer(object state)
        {
            //Send(NodeFrame.ActivateOutput(1, _output1));

            

            IClspNode sn = (IClspNode)state;

            TestingContext tc;

            if (!_testingContexts.TryGetValue(sn.LogicalAddress, out tc))
                return;

            int randomVal;
            /*if (tc.crActionCounter % 1000 == 1)
                randomVal = _r.Next(1000) + 1000;
            else*/
            /*if (_dummy % 10 < 8)
                randomVal = _r.Next(100) + 50;*/
            //else
                randomVal = _r.Next(300) + 100;

            _dummy++;

            tc.timer = new Timer(OnTimer, sn, randomVal, Timeout.Infinite);

            if (_dummy % 5 == 2)
            {
                CardReader cr = _nodeComm.GetCardReader(sn, 1);
                if (cr != null && cr.IsOnline)
                {


                    cr.ParentCommunicator.DisplayCommands.DisplayText(cr, 2, 2, (tc.crActionCounter++ + "   " + randomVal + "   "));
                }
                else
                    cr = null;
            }
            else
            {
                
                _nodeComm.EnqueueFrame(sn.LogicalAddress, NodeFrame.SetOutput(3, _dummy % 2 == 0));
            }
        }

        private int _commandTimeoutCount = 0;
        void _nodeComm_CommandTimeout(IClspNode param1, ClspFrame param2)
        {
            //Log.Singleton.Info("WARNING !!! TIMEOUT: " + string.Format("0x{0:X}", param2),"10.0.5.16", 1234);

            Log.Singleton.Error("COMMAND TIMEOUT " + param1.LogicalAddress + " count : " + (++_commandTimeoutCount));

            CardReader cr = _nodeComm.GetCardReader(param1, 1);
            if (cr != null && cr.IsOnline)
            {  
                cr.ParentCommunicator.DisplayCommands.DisplayText(cr, 2, 4, _commandTimeoutCount+"  ");
            }

            cr = _nodeComm.GetCardReader(param1, 2);
            if (cr != null && cr.IsOnline)
            {
                cr.ParentCommunicator.DisplayCommands.DisplayText(cr, 2, 4, _commandTimeoutCount + "  ");
            }

            if ((NodeCommand)param2.Command == NodeCommand.DebugSequenceRequest)
            {
                //System.Diagnostics.Debug.WriteLine("!!!! TIMEOUT !!!! - current: " + _currentSeqNumber);

                _debugSeqTimeout = true;
                _debugSeqEvent.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        event Action<int,bool> x;

        void _nodeComm_CommandNACK(IClspNode param1, NodeCommand command, NodeErrorCode param2, IClspFrame param3)
        {
            Console.WriteLine("ERROR !!! NACK to Command=" + command
                +
                string.Format(" cmd={0:X} errcode={1}", 
                command,
                param2
                )
                //+"\r\n\t"+ByteDataCarrier.HexDump(param3.GetRaw485Bytes(null))
                );
        }
       

        void _nodeComm_CommandACK(IClspNode param1, NodeCommand param2)
        {
            //Console.WriteLine("ACK: " + string.Format("0x{0:X}", param2));
        }

        private byte _selectedNode = 0;

        private void Send(IClspFrame frame)
        {
            _nodeComm.EnqueueFrame((byte)_selectedNode, frame);
        }

        private void Send(byte address, IClspFrame frame)
        {
            _nodeComm.EnqueueFrame(address, frame);
        }

        #region Upgrade testing
        private SimpleTcpServer _tcpServer;
        private List<int> _nodeList;

        private string _serverReceived = string.Empty;
        private Dictionary<byte, SimpleGeneralTcpConnection> _connectionList;

        void _tcpServer_DataReceived(SimpleGeneralTcpConnection connection, ByteDataCarrier data)
        {
            _serverReceived += data.GetUTF8String();

            try
            {
                Regex upCmdRegex = new Regex(@"u (?<address>\d+)");
                Match isUpCmd = upCmdRegex.Match(_serverReceived);

                Regex upMultiCmdRegex = new Regex(@"um (?<address>\d+)");
                Match isUpMultiCmd = upMultiCmdRegex.Match(_serverReceived);

                Regex resetCmdRegex = new Regex(@"r (?<address>\d+)");
                Match isResetCmd = resetCmdRegex.Match(_serverReceived);

                Regex upCrAllCmdRegex = new Regex(@"ucrAll (?<nodeAddress>\d+)");
                Match isUpCrAllCmd = upCrAllCmdRegex.Match(_serverReceived);

                Regex upCrCmdRegex = new Regex(@"ucr (?<nodeAddress>\d+) (?<crAddress>\d+)");
                Match isUpCrCmd = upCrCmdRegex.Match(_serverReceived);

                Regex testCmdRegex = new Regex(@"t (?<address>\d+) (?<inverted>\d)");
                Match isTestCmd = testCmdRegex.Match(_serverReceived);

                Regex implicitCmdRegex = new Regex(@"imp (?<address>\d+) (?<reader>\d) (?<code>\d{2})");
                Match isImplicitCmd = implicitCmdRegex.Match(_serverReceived);

                Regex blockedOutputRegex = new Regex(@"b (?<address>\d+) (?<output>\d)?");
                Match isBlockedCmd = blockedOutputRegex.Match(_serverReceived);

                Regex activateOutputRegex = new Regex(@"o (?<address>\d+) (?<output>\d) (?<activate>\d) (?<type>\d)");
                Match isActivateOutput = activateOutputRegex.Match(_serverReceived);

                if (isUpCrAllCmd.Success)
                {
                    #region Upgrade all CR 
                    _serverReceived = string.Empty;

                    byte nodeAddress = byte.Parse(isUpCrAllCmd.Groups["nodeAddress"].Value);

                    if (_nodeList.Contains(nodeAddress))
                    {
                        if (!_connectionList.ContainsKey(nodeAddress))
                            _connectionList.Add(nodeAddress, connection);

                        for(byte i=1;i<=2;i++)
                        try
                        {
                            CRUpgradeCommands cruc = _nodeComm.GetCrUpgradeCommands(nodeAddress);

                            CardReader cr = _nodeComm.GetCardReader(nodeAddress, i);
                            if (cr == null)
                                throw new DoesNotExistException(cr, "Card reader with address " + i + " on Node" + nodeAddress + " does not exist");

                            cruc.StartUpgrade(cr, @"\NandFlash\DCUTester\CR.bin");
                        }
                        catch (Exception e)
                        {
                            string tmp = "Error upgrading CR" + i + " on Node" + nodeAddress + "\n" + e.Message;
                            Send2All(tmp);
                        }
                    }
                    else
                        connection.Send(new ByteDataCarrier("Warning : Node " + nodeAddress + " is not assigned\n"));
                    #endregion
                }
                else if (isUpCrCmd.Success)
                {
                    #region Upgrade CR
                    _serverReceived = string.Empty;

                    byte nodeAddress = byte.Parse(isUpCrCmd.Groups["nodeAddress"].Value);
                    byte crAddress = byte.Parse(isUpCrCmd.Groups["crAddress"].Value);

                    if (_nodeList.Contains(nodeAddress))
                    {
                        if (!_connectionList.ContainsKey(nodeAddress))
                            _connectionList.Add(nodeAddress, connection);

                        try
                        {
                            CRUpgradeCommands cruc = _nodeComm.GetCrUpgradeCommands(nodeAddress);

                            CardReader cr = _nodeComm.GetCardReader(nodeAddress, crAddress);
                            if (cr == null)
                                throw new DoesNotExistException(cr, "Card reader with address " + crAddress + " on Node" + nodeAddress + " does not exist");

                            cruc.StartUpgrade(cr, @"\NandFlash\DCUTester\CR.bin");
                        }
                        catch (Exception e)
                        {
                            string tmp = "Error upgrading CR" + crAddress + " on Node" + nodeAddress + "\n" + e.Message;
                            Send2All(tmp);
                        }
                    }
                    else
                        connection.Send(new ByteDataCarrier("Warning : Node " + nodeAddress + " is not assigned\n"));
                    #endregion
                } 
                else if (isTestCmd.Success)
                {
                    #region Start IO test
                    _serverReceived = string.Empty;

                    byte address = byte.Parse(isTestCmd.Groups["address"].Value);
                    bool inverted = isTestCmd.Groups["inverted"].Value == "1";
                    if (_nodeList.Contains(address))
                    {
                        if (!_connectionList.ContainsKey(address))
                            _connectionList.Add(address, connection);

                        _nodeComm.EnqueueFrame(address, NodeFrame.StartIOTest(inverted));
                    }
                    else
                        connection.Send(new ByteDataCarrier("Warning : Node " + address + " is not assigned\n"));
                    #endregion
                } 
                else if (isUpMultiCmd.Success)
                {
                    #region Upgrade DCU
                    _serverReceived = string.Empty;

                    byte maxAddress = byte.Parse(isUpMultiCmd.Groups["address"].Value);

                    for (byte i = 1; i <= maxAddress; i++)
                    if (_nodeList.Contains(i))
                    {
                        if (!_connectionList.ContainsKey(i))
                            _connectionList.Add(i, connection);

                        _nodeComm.UpgradeNode(i, @"\NandFlash\ccu\DcuUpgrades\dcu.bin");
                        //_nodeComm.UpgradeNode(address, @"nandflash\dcutester\c.sim");
                    }
                    else
                        connection.Send(new ByteDataCarrier("Warning : Node " + i + " is not assigned\n"));
                
                    #endregion
                }
                else if (isUpCmd.Success)
                {
                    #region Upgrade DCU
                    _serverReceived = string.Empty;

                    byte address = byte.Parse(isUpCmd.Groups["address"].Value);
                    if (_nodeList.Contains(address))
                    {
                        if (!_connectionList.ContainsKey(address))
                            _connectionList.Add(address, connection);

                        _nodeComm.UpgradeNode(address, @"\NandFlash\ccu\dcu.bin");
                        //_nodeComm.UpgradeNode(address, @"nandflash\dcutester\c.sim");
                    }
                    else
                        connection.Send(new ByteDataCarrier("Warning : Node " + address + " is not assigned\n"));
                    #endregion
                }
                else if (isResetCmd.Success)
                {
                    #region Restart DCU
                    _serverReceived = string.Empty;

                    byte address = byte.Parse(isResetCmd.Groups["address"].Value);
                    if (_nodeList.Contains(address))
                    {
                        if (!_connectionList.ContainsKey(address))
                            _connectionList.Add(address, connection);

                        _nodeComm.EnqueueFrame(address, NodeFrame.RestartNode());
                    }
                    else
                        connection.Send(new ByteDataCarrier("Warning : Node " + address + " is not assigned\n"));
                    #endregion
                }
                else if (isImplicitCmd.Success)
                {
                    #region Set implicit command
                    _serverReceived = string.Empty;

                    byte address = byte.Parse(isImplicitCmd.Groups["address"].Value);
                    int reader = int.Parse(isImplicitCmd.Groups["reader"].Value);
                    byte code = byte.Parse(isImplicitCmd.Groups["code"].Value);

                    _nodeComm.EnqueueFrame(address, NodeFrame.SetImplicitCRCode(reader, (int)code, -1, null, false));
                    #endregion
                }
                else if (isBlockedCmd.Success)
                {
                    #region Blocked output ex
                    _serverReceived = string.Empty;
                    byte address = byte.Parse(isBlockedCmd.Groups["address"].Value);

                    int outputId = -1;
                    if (isBlockedCmd.Groups["output"] != null)
                        outputId = int.Parse(isBlockedCmd.Groups["output"].Value);

                    if (outputId != -1)
                        _nodeComm.EnqueueFrame(address, NodeFrame.SetBlockedOutputsEx(outputId));
                    else
                        _nodeComm.EnqueueFrame(address, NodeFrame.SetBlockedOutputsEx(null));
                    #endregion
                }
                else if (isActivateOutput.Success)
                {
                    #region Activate output
                    _serverReceived = string.Empty;
                    byte address = byte.Parse(isActivateOutput.Groups["address"].Value);
                    int outputId = int.Parse(isActivateOutput.Groups["output"].Value);
                    int activate = int.Parse(isActivateOutput.Groups["activate"].Value);
                    int type = int.Parse(isActivateOutput.Groups["type"].Value);

                    switch (type)
                    {
                        case 0:
                            _nodeComm.EnqueueFrame(address, NodeFrame.OutputConfigLevel((byte)outputId, 0, 5000, false));
                            break;
                        case 1:
                            _nodeComm.EnqueueFrame(address, NodeFrame.OutputConfigFrequency((byte)outputId, 500, 500, 0, 5000, false, false));
                            break;
                        case 2:
                            _nodeComm.EnqueueFrame(address, NodeFrame.OutputConfigPulse((byte)outputId, 1000, 0, 5000, false, false));
                            break;
                    }
                    _nodeComm.EnqueueFrame(address, NodeFrame.SetOutput((byte)outputId, activate != 0));
                    #endregion
                }
                #region Output testing
                /*
                else if (_serverReceived.Contains("l"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(4, NodeFrame.OutputConfigLevel((byte)3, 0, 5000, false));
                }
                else if (_serverReceived.Contains("f"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(4, NodeFrame.OutputConfigFrequency((byte)3, 500, 500, 0, 5000, false, false));
                }
                else if (_serverReceived.Contains("p"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(4, NodeFrame.OutputConfigPulse(3, 500, 0, 5000, false, false));
                }
                else if (_serverReceived.Contains("z"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(4, NodeFrame.SetOutput((byte)3, true));
                }
                else if (_serverReceived.Contains("v"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(4, NodeFrame.SetOutput(3, false));
                }
                */
                #endregion

                #region Special output testing
                /*
                else if (_serverReceived.Contains("1"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.UnsetSpecialOutput(SpecialOutputType.Sabotage));
                    _nodeComm.EnqueueFrame(1, NodeFrame.SetSpecialOutput(SpecialOutputType.Sabotage, 1));
                }
                else if (_serverReceived.Contains("2"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.UnsetSpecialOutput(SpecialOutputType.Sabotage));
                    _nodeComm.EnqueueFrame(1, NodeFrame.SetSpecialOutput(SpecialOutputType.Sabotage, 2));
                }
                else if (_serverReceived.Contains("3"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.UnsetSpecialOutput(SpecialOutputType.Sabotage));
                    _nodeComm.EnqueueFrame(1, NodeFrame.SetSpecialOutput(SpecialOutputType.Sabotage, 3));
                }
                */
                #endregion

                #region Toggle event generators
                else if (_serverReceived.Contains("g1"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleCardGenerator(true));
                }
                else if (_serverReceived.Contains("g2"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleCardGenerator(true, GeneratedCardType.MifareCSN, 0, 1));
                }
                else if (_serverReceived.Contains("g3"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleCardGenerator(true, GeneratedCardType.MifareSector, 3, 6));
                }
                else if (_serverReceived.Contains("g0"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleCardGenerator(false));
                }
                else if (_serverReceived.Contains("a1"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleADCGenerator(true));
                }
                else if (_serverReceived.Contains("a2"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleADCGenerator(true, 0, 1));
                }
                else if (_serverReceived.Contains("a3"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleADCGenerator(true, 3, 4));
                }
                else if (_serverReceived.Contains("a0"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.ToggleADCGenerator(false));
                }
                #endregion
                #region Signal AG with Emergency codes & plain
                else if (_serverReceived.Contains("ag0"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.SignalAccessGranted(AGSourceCard.NormalCard));
                }
                else if (_serverReceived.Contains("ag1"))
                {
                    _serverReceived = string.Empty;
                    _nodeComm.EnqueueFrame(1, NodeFrame.SignalAccessGranted(AGSourceCard.EmergencyCard));
                }
                #endregion
                else
                {
                    //_serverReceived = string.Empty;
                    connection.Send(new ByteDataCarrier("Error : Invalid command format\n"));
                }
            }
            catch (Exception e)
            {
                connection.Send(new ByteDataCarrier("Error : "+e.Message+"\n"));
            }
        }

        void _nodeComm_UpgradeTimeout(IClspNode node)
        {
            int nodeAddress = node != null ? node.LogicalAddress : -1;
            string tmp = "ERROR: Upgrade timeout for node " + nodeAddress;
            Console.WriteLine(tmp);
            Send2All(tmp);
        }

        void _nodeComm_UpgradeProgress(IClspNode node, int progress)
        {
            string tmp = "INFO: Upgrade progres for node " + node.LogicalAddress + ": " + progress + "%";
            Console.WriteLine(tmp);
            Send2All(tmp);
        }

        void Send2All(string message)
        {
            if (Validator.IsNotNullString(message))
            {
                ByteDataCarrier bdc = new ByteDataCarrier(message+"\r\n");

                foreach (ISimpleTcpConnection connection in _tcpServer)
                {
                    connection.Send(bdc);
                }
            }
        }

        void _nodeComm_UpgradeError(IClspNode node, UpgradeErrors param1, string param2)
        {
            int nodeAddress = node != null ? node.LogicalAddress : -1;

            string tmpMessage = "ERROR: Upgrade error: " + param2 + " for node " + nodeAddress;

            Console.WriteLine(tmpMessage);
            Send2All(tmpMessage);
        }

        void _nodeComm_UpgradeDone(IClspNode node)
        {
            int nodeAddress = node != null ? node.LogicalAddress : -1;

            string tmpMessage = "INFO: Upgrade done for node " + nodeAddress;

            Console.WriteLine(tmpMessage);
            Send2All(tmpMessage);
        }

        #endregion

        #region Protocol testing

        private bool _debugSeqRunning = false;
        private SafeThread _debugSeqThread = null;
        private byte _currentSeqNumber = 0;
        private byte _recSeqNumber = 0;
        private bool _debugSeqTimeout = false;
        private AutoResetEvent _debugSeqEvent;



        public void StartSeqDebug()
        {
            if (_debugSeqRunning)
                return;

            _debugSeqEvent = new AutoResetEvent(false);
            _debugSeqThread = new SafeThread(DebugSeqThread);
            _debugSeqThread.Start();

            _debugSeqRunning = true;
        }

        public void StopSeqDebug()
        {
            if (!_debugSeqRunning)
                return;

            if (_debugSeqThread != null)
                _debugSeqThread.Stop(500);

            _debugSeqRunning = false;
        }

        private void DebugSeqThread()
        {
            int tmpCounter = 0;

            try
            {
                while (true)
                {
                    //_debugSeqEvent.Reset();
                    _debugSeqTimeout = false;

                    Send(NodeFrame.DebugSequenceRequest(_currentSeqNumber));

                    _debugSeqEvent.WaitOne();

                    if (_debugSeqTimeout)
                    {
                        Debug.WriteLine("TIMEOUT - current: " + _currentSeqNumber);
                    }
                    else
                    {
                        if ((tmpCounter % 10) == 0)
                            Debug.WriteLine("INFO - current: " + _currentSeqNumber);

                        tmpCounter++;

                        _currentSeqNumber++;
                    }
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine("Debug sequence numbers: " + error.Message);
            }
        }

        private int _seqCountErrors = 0;
        void _nodeComm_DebugSequenceResponse(byte seqNumber)
        {
            if (seqNumber == _currentSeqNumber)
            {
                //System.Diagnostics.Debug.WriteLine("RESPONSE: " + seqNumber);
                _debugSeqTimeout = false;
                _recSeqNumber = seqNumber;
                _debugSeqEvent.Set();
            }
            else
            {
                _seqCountErrors++;
                /*
                Log.Singleton.EnableEnqueue = false;
                while (Log.Singleton.QueuedMessagesCount > 0)
                    Thread.Sleep(50);
                System.Diagnostics.Debug.WriteLine("ERROR - received does not match: " + _currentSeqNumber + " : " + seqNumber);
                Log.Singleton.EnableEnqueue = true;*/
            }

        }

        #endregion
    }
}
