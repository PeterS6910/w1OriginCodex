using System;
using System.Collections.Generic;
using System.IO;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.NodeDataProtocol;
using Contal.Drivers.CardReader;
using Contal.Drivers.ClspDrivers;
using Contal.Drivers.InterCommDrivers;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal sealed class DCUs : AStateAndSettingsObjectCollection<DCUs, DCUStateAndSettings, DB.DCU>
    {
        private const string UPGRADE_INFO_FILE = "UpgradeInfo.txt";

        private const int SET_TIME_DELAY = 3600000;

        private const byte DEFAULT_MAX_NODE_LOOKUP_SEQUENCE = 30;

        public const string TAMPER_PARAM = "TAMPER";

        private volatile NodeCommunicator _nodeCommunicator;

        private readonly SyncDictionary<byte, DCUStateAndSettings> _objectsByAddress =
            new SyncDictionary<byte, DCUStateAndSettings>();

        private readonly SyncDictionary<byte, string> _registeredUpgradeVersions =
            new SyncDictionary<byte, string>();

        private readonly SyncDictionary<Guid, DVoid2Void> _eventsForRunAfterConnected =
            new SyncDictionary<Guid, DVoid2Void>();

        private volatile uint _actualCoprocessorBuildNumber;

        private DCUs()
            : base(null)
        {
        }

        public uint ActualCoprocessorBuildNumber
        {
            get { return _actualCoprocessorBuildNumber; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ActualCoprocessorStats
        {
            get
            {
                if (_interComm == null)
                    return String.Empty;

                return
                    new TimeSpan(
                        0,
                        0,
                        0,
                        (int)_interComm.Statistics.Uptime) + "/" +
                    _interComm.Statistics.MemoryLoad + "%/" +
                    _interComm.Statistics.IntercommFramingErrorCount + "/" +
                    _interComm.Statistics.Uart0FramingErrorCount;


            }
        }

        private int _commandTimeouts;

        public int CommandTimeouts
        {
            get { return _commandTimeouts; }
            set { _commandTimeouts = value; }
        }

        private volatile ICUpgradeResult _coprocessorUpgradeResult = ICUpgradeResult.Unknown;

        public string CoprocessorUpgradeResult
        {
            get { return _coprocessorUpgradeResult.ToString(); }
        }

        private volatile InterComm _interComm;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void DCUs.Start()");

            var mbv =
                (MainBoardVariant)
                    MainBoard.Variant;

            if (mbv != MainBoardVariant.CCU0_RS485
                && mbv != MainBoardVariant.CCU0_ECHELON)
            {
                return;
            }
            var pollingViaCoprocessor = true;

            if (mbv == MainBoardVariant.CCU0_ECHELON)
            {
                _nodeCommunicator =
                    new NodeCommunicator(
                        ClspTransportMode.IndirectViaLonShortStack,
                        null,
                        null,
                        CcuCore.DebugLog);

                pollingViaCoprocessor = false;
            }

            if (pollingViaCoprocessor)
            {
                try
                {
                    if (File.Exists(CcuCore.CLSP_HWC_FILE_PATH))
                    {
                        _interComm = new InterComm();

                        _coprocessorUpgradeResult = ICUpgradeResult.Unknown;

                        ICUpgradeContext uc = null;

                        try
                        {
                            uc = new ICUpgradeContext(
                                _interComm,
                                CcuCore.CLSP_HWC_VERSION,
                                CcuCore.CLSP_HWC_FILE_PATH);

                            //uc.UpgradeFile = @"\NandFlash\clsphwc.bin";
                            _coprocessorUpgradeResult = uc.Upgrade(CcuCore.CLSP_COM_PORT);
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                        if (uc != null)
                        {
                            _actualCoprocessorBuildNumber = uc.ActualBuildNumber;

                            uc.Close();
                        }
                    }
                    else
                        _coprocessorUpgradeResult = ICUpgradeResult.UpgradeFileNotPresent;
                }
                catch (Exception error)
                {
                    // just safety, so there will be place for NodeCommunicator instantiation below
                    HandledExceptionAdapter.Examine(error);
                }

                if (null == _interComm)
                    _interComm = new InterComm();

                _nodeCommunicator =
                    new NodeCommunicator(
                        ClspTransportMode.IndirectViaClspHwc,
                        _interComm,
                        null,
                        CcuCore.DebugLog
                        );
            }

            _nodeCommunicator.ClspHwcEnablePolling = false;
            _nodeCommunicator.NodeResponseTimeout = 15;
            _nodeCommunicator.NodeResponseLatency = 1;
            _nodeCommunicator.NodeUnpolledTimeout = 10;
            _nodeCommunicator.DelayBetweenSuccessfulPolling = 5;

            switch (mbv)
            {
                case MainBoardVariant.CCU0_RS485:
                    ClspNode.GlobalMaxRetryCount = 10;
                    _nodeCommunicator.AsyncTimeout = 16000;
                    // with new CommandTimeout retrying mechanism, it should not be as big as 30000

                    var ccu = Ccus.Singleton.GetCCU();
                    if (ccu != null)
                    {
                        byte maxNodeLookupSequence = 0;
                        if (ccu.MaxNodeLookupSequence > 0)
                            maxNodeLookupSequence = (byte)(ccu.MaxNodeLookupSequence - 1);

                        _nodeCommunicator.MaxNodeLookupSequence = maxNodeLookupSequence;
                    }
                    else
                        _nodeCommunicator.MaxNodeLookupSequence =
                            DEFAULT_MAX_NODE_LOOKUP_SEQUENCE;

                    break;
                case MainBoardVariant.CCU0_ECHELON:
                    try
                    {
                        Echelon.SetLonTransportTimeout(2000);
                    }
                    catch (Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
#if DEBUG
                        Console.WriteLine(
                            "\nERROR SETTING LON TRANSPORT TIMEOUT : " + e.Message);
#endif
                    }
                    _nodeCommunicator.AsyncTimeout = 24000;
                    // with new CommandTimeout retrying mechanism, it should not be as big as 10000
                    break;
            }

            _nodeCommunicator.NodeAssigned += OnNodeAssigned;
            _nodeCommunicator.NodeRenewed += OnNodeRenewed;
            _nodeCommunicator.NodeReleased += OnNodeReleased;
            _nodeCommunicator.ReportInputCount += ReportInputCount;
            _nodeCommunicator.ReportOutputCount += ReportOutputCount;
            _nodeCommunicator.ReportFWVersion += ReportFWVersion;
            _nodeCommunicator.InputChanged += InputChanged;
            _nodeCommunicator.OutputChanged += OutputChanged;
            _nodeCommunicator.OutputLogicChanged += OutputLogicChanged;
            _nodeCommunicator.DSMChanged += DSMChanged;
            _nodeCommunicator.CommandNACK += CommandNACK;
            _nodeCommunicator.CommandTimeout += CommandTimeout;
            _nodeCommunicator.UpgradeDone += UpgradeDone;
            _nodeCommunicator.SpecialInputChanged += OnSpecialInputChanged;
            _nodeCommunicator.UpgradeError += UpgradeError;
            _nodeCommunicator.UpgradeProgress += UpgradeProgress;
            _nodeCommunicator.UpgradeTimeout += UpgradeTimeout;
            _nodeCommunicator.UpgradeNodeLost += UpgradeNodeLost;
            _nodeCommunicator.MemoryWarning += ReportDcuMemoryWarning;
            _nodeCommunicator.DSMActivationChanged += _nodeCommunicator_DSMActivationChanged;
            _nodeCommunicator.CoprocessorFailureChanged +=
                _nodeCommunicator_CoprocessorFailureChanged;
        }

        private readonly object _lockIsStarte = new object();
        private bool _isStarted;

        public void Start()
        {
            lock (_lockIsStarte)
            {
                if (_isStarted)
                    return;

                _isStarted = true;
            }

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void DCUs.Start()");

            var mbv =
                (MainBoardVariant)
                    MainBoard.Variant;

            if (mbv != MainBoardVariant.CCU0_RS485
                && mbv != MainBoardVariant.CCU0_ECHELON)
            {
                return;
            }

            _nodeCommunicator.Start(
                mbv == MainBoardVariant.CCU0_RS485
                    ? CcuCore.CLSP_COM_PORT
                    : "COM1");

            StartSetTimeTimer();
            _nodeCommunicator_CoprocessorFailureChanged(_nodeCommunicator.CoprocessorFailure);
        }

        public void SetMaxNodeLookupSequence(DB.CCU ccu)
        {
            if (_nodeCommunicator == null)
                return;

            if (ccu == null)
                return;

            byte maxNodeLookupSequence = 0;

            if (ccu.MaxNodeLookupSequence > 0)
                maxNodeLookupSequence = (byte)(ccu.MaxNodeLookupSequence - 1);

            _nodeCommunicator.MaxNodeLookupSequence = maxNodeLookupSequence;
        }

        private static void _nodeCommunicator_CoprocessorFailureChanged(bool coprocessorFailure)
        {
            var idCcu = Ccus.Singleton.GetCcuId();

            if (idCcu != null)
            {
                if (!coprocessorFailure)
                    AlarmsManager.Singleton.StopAlarm(
                        CcuCoprocessorFailureAlarm.CreateAlarmKey(
                            idCcu.Value));
                else
                    AlarmsManager.Singleton.AddAlarm(
                        new CcuCoprocessorFailureAlarm(
                            idCcu.Value));
            }

            Events.ProcessEvent(
                new EventCoprocessorFailureChanged(
                    coprocessorFailure));
        }

        /// <summary>
        /// Returns information about automatic refresh after set launguage support
        /// </summary>
        /// <param name="idDcu">DCU id</param>
        /// <returns></returns>
        public bool IsAutomaticRefreshAfterSetLaunguageSupported(Guid idDcu)
        {
            DCUStateAndSettings dcuStateAndSettings;

            return
                _objects.TryGetValue(
                    idDcu,
                    out dcuStateAndSettings)
                && dcuStateAndSettings.IsAutomaticRefreshAfterSetLaunguageSupported();
        }

        /// <summary>
        /// Returns true if automatic loose card reader after restart is supported otherwise returns false
        /// </summary>
        /// <param name="idDcu"></param>
        /// <returns></returns>
        public bool AutomaticLooseCardReaderAfterRestartSupported(Guid idDcu)
        {
            DCUStateAndSettings dcuStateAndSettings;

            return
                _objects.TryGetValue(
                    idDcu,
                    out dcuStateAndSettings)
                && dcuStateAndSettings.AutomaticLooseCardReaderAfterRestartSupported();
        }

        private void _nodeCommunicator_DSMActivationChanged(
            IClspNode node,
            bool isRunning)
        {
            if (node == null)
                return;

            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                DoorEnvironments.Singleton.OnDcuDsmActivationStateChanged(
                    dcuStateAndSettings.IdDoorEnvironment,
                    isRunning);
            }
        }

        public bool RegisterCRsForUpgradeVersion(
            byte dcuLogicalAddress,
            byte[] cardReaderAddresses,
            string upgradeVersion)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool DCUs.RegisterCRsForUpgradeVersion(byte dcuLogicalAddress, byte[] cardReaderAddresses, string upgradeVersion): [{0}]",
                    Log.GetStringFromParameters(
                        dcuLogicalAddress,
                        cardReaderAddresses,
                        upgradeVersion)));

            DCUStateAndSettings dcuStateAndSettings;

            if (!_objectsByAddress.TryGetValue(
                dcuLogicalAddress,
                out dcuStateAndSettings))
            {
                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool DCUs.RegisterCRsForUpgradeVersion return false[1]");

                return false;
            }

            foreach (var address in cardReaderAddresses)
                CardReaders.Singleton.RegisterCRsForUpgradeVersion(
                    dcuStateAndSettings.Id,
                    address,
                    upgradeVersion);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool DCUs.RegisterCRsForUpgradeVersion return true[1]");

            return true;
        }

        public void SetNodeCommunicatorPolling(bool enable)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetNodeCommunicatorPolling(bool enable): [{0}]",
                    Log.GetStringFromParameters(enable)));
            if (_nodeCommunicator != null)
            {
                _nodeCommunicator.ClspHwcEnablePolling = enable;
            }
        }

        private void UpgradeTimeout(IClspNode node)
        {
            if (node == null)
                return;

            Events.ProcessEvent(
                new EventDcuUpgradePercentageSet(
                    node.LogicalAddress,
                    -1));

            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SetOnlineState(
                    node.IsOnline
                        ? State.Online
                        : State.Offline);
            }

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "DCU with logical address: " + node.LogicalAddress + " Upgrade Timeout");
        }

        private static void UpgradeProgress(
            IClspNode node,
            int percent)
        {
            if (node != null)
                Events.ProcessEvent(
                    new EventDcuUpgradePercentageSet(
                        node.LogicalAddress,
                        percent));
        }

        private void UpgradeError(
            IClspNode node,
            UpgradeErrors errors,
            string param)
        {
            if (node == null)
                return;

            Events.ProcessEvent(
                new EventDcuUpgradePercentageSet(
                    node.LogicalAddress,
                    -1));

            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SetOnlineState(
                    node.IsOnline
                        ? State.Online
                        : State.Offline);
            }

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "DCU with logical address: " + node.LogicalAddress + " Upgrade Failed");
        }

        private void UpgradeNodeLost(byte logicalAddress)
        {
            Events.ProcessEvent(
                new EventDcuUpgradePercentageSet(
                    logicalAddress,
                    -1));

            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                logicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SetOnlineState(State.Offline);
            }

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "DCU with logical address: " + logicalAddress + " Upgrade Failed - node lost");
        }

        private static void UpgradeDone(IClspNode node)
        {
            if (node == null)
                return;

            Events.ProcessEvent(
                new EventDcuUpgradePercentageSet(
                    node.LogicalAddress,
                    100));

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "DCU with logical address: " + node.LogicalAddress + " Upgrade Done");
        }

        private void OnSpecialInputChanged(
            IClspNode node,
            SpecialInput specInput,
            InputState state)
        {
            if (SpecialInput.Tamper != specInput)
                return;

            var isInTamper = state == InputState.Alarm;

            node.SetParameter(TAMPER_PARAM, isInTamper);

            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.OnDcuTamperChanged(isInTamper);
            }
        }

        /*
        void _nodeCommunicator_CommandACK(SlaveNode node, byte param2)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "_nodeCommunicator_CommandACK command: " + param2);
        }*/

        private void CommandTimeout(
            IClspNode node,
            IClspFrame frame)
        {
            CcuCore.DebugLog.Error("CommandTimeout DCU logical address: " + node.LogicalAddress);

            _commandTimeouts++;

            //outputs need specific command timeout handling
            if ((NodeCommand)frame.Command == NodeCommand.ActivateOutput)
                ProcessOutputActivationTimeout(
                    node,
                    frame);

            DCUStateAndSettings dcuStateAndSetting;

            if (!_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSetting))
            {
                return;
            }

            Events.ProcessEvent(
                new EventDcuCommandTimeOut(
                    dcuStateAndSetting.Id,
                    NodeCommandHelper.GetCommandName(frame.Command)));
        }

        private void ProcessOutputActivationTimeout(
            IClspNode node,
            IClspFrame frame)
        {
            if (node == null
                || frame == null)
                return;

            if ((NodeCommand)frame.Command != NodeCommand.ActivateOutput)
                return;

            if (frame.OptionalData == null
                || frame.OptionalData.Length < 3)
                return;

            try
            {
                var dcuLogicalAddress = node.LogicalAddress;
                var outputNumber = (byte)frame.OptionalData.GetValue(2);

                DCUStateAndSettings dcuStateAndSettings;
                if (!_objectsByAddress.TryGetValue(
                    dcuLogicalAddress,
                    out dcuStateAndSettings)
                    || dcuStateAndSettings == null)
                {
                    return;
                }

                var idOutput =
                    Outputs.Singleton.GetOutputIdByDcuAndNumber(
                        dcuStateAndSettings.Id,
                        outputNumber);

                if (idOutput == Guid.Empty)
                    return;

                dcuStateAndSettings.ForcedSetOutputToOff(outputNumber);
                Outputs.Singleton.ReRunOutputActivators(idOutput);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void CommandNACK(
            IClspNode node,
            NodeCommand command,
            NodeErrorCode errorCode,
            IClspFrame frame)
        {
            CcuCore.DebugLog.Error(
                "CommandNACK - DCU logical address: " + node.LogicalAddress + ", error code: "
                + errorCode + ", command: " + command + " frame:"+frame);

            DCUStateAndSettings dcuStateAndSetting;

            if (!_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSetting))
            {
                return;
            }

            Events.ProcessEvent(
                new EventDcuCommandNotAck(
                    dcuStateAndSetting.Id,
                    errorCode.ToString(),
                    command.ToString()));
        }

        private void OnNodeReleased(IClspNode node)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.OnNodeReleased(node);
            }
            else
                Events.ProcessEvent(
                    new EventDcuOnlineStateChanged(
                        State.Offline,
                        node.LogicalAddress,
                        (byte)ProtocolId.InvalidProtocol));
        }

        private void OnNodeAssigned(IClspNode node)
        {
            DCUStateAndSettings dcuStateAndSetting;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.OnNodeAssigned(node);
            }
            else
                Events.ProcessEvent(
                    new EventDcuOnlineStateChanged(
                        State.Online,
                        node.LogicalAddress,
                        (byte)node.MasterProtocol));
        }

        private void OnNodeRenewed(IClspNode node)
        {
            DCUStateAndSettings dcuStateAndSetting;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.OnNodeRenewed(node);
            }
            else
                Events.ProcessEvent(
                    new EventDcuOnlineStateChanged(
                        State.Online,
                        node.LogicalAddress,
                        (byte)node.MasterProtocol));
        }

        public static bool IsRegisteredDefaultUpgradeVersion(out string upgradeFileName)
        {
            upgradeFileName = string.Empty;
            if (!File.Exists(
                Path.Combine(
                    QuickPath.ApplicationStartupDirectory,
                    UPGRADE_INFO_FILE)))
                return false;

            StreamReader sr = null;

            try
            {
                sr = File.OpenText(
                    Path.Combine(
                        QuickPath.ApplicationStartupDirectory,
                        UPGRADE_INFO_FILE));

                var content = sr.ReadLine();

                if (string.IsNullOrEmpty(content))
                    return false;

                if (!Directory.Exists(CcuCore.DCU_UPGRADES_DIRECTORY_NAME))
                    return false;

                foreach (var fullFileName in Directory.GetFiles(CcuCore.DCU_UPGRADES_DIRECTORY_NAME)
                    )
                {
                    Exception ex;
                    var headerParameters = FilePacker.TryGetHeaderParameters(
                        fullFileName,
                        out ex);
                    if (headerParameters == null
                        || headerParameters.Length < 4
                        || headerParameters[0].ToLower() != DeviceType.DCU.ToString()
                            .ToLower()
                        || headerParameters[1] != content)
                        continue;

                    var version = headerParameters[1];

                    var realFileName = version + ".bin";

                    upgradeFileName = Path.Combine(
                        CcuCore.DCU_UPGRADES_DIRECTORY_NAME,
                        realFileName);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                return false;
            }
            finally
            {
                if (sr != null)
                    try
                    {
                        sr.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }
        }

        private void ReportInputCount(
            IClspNode node,
            uint inputCount)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SendDCUInputCountToServer((byte)inputCount);
            }
        }

        private void ReportOutputCount(
            IClspNode node,
            uint outputCount)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SendDCUOutputCountToServer((byte)outputCount);
            }
        }

        private void ReportFWVersion(
            IClspNode node,
            ExtendedVersion fwVersion)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (!_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                return;
            }

            if (fwVersion == null)
                return;

            dcuStateAndSettings.FwVersion = fwVersion;
            dcuStateAndSettings.SendDCUFWVersionToServer();
        }

        private void InputChanged(
            IClspNode node,
            byte inputNumber,
            InputState inputState)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "DCUInput changed - input number: " + inputNumber + " input state: "
                    + inputState);

            DCUStateAndSettings dcuStateAndSetting;

            if (!_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSetting))
            {
                return;
            }

            State state;

            switch (inputState)
            {
                case InputState.Normal:

                    state = State.Normal;
                    break;

                case InputState.Alarm:

                    state = State.Alarm;
                    break;

                case InputState.Short:

                    state = State.Short;
                    break;

                case InputState.Break:

                    state = State.Break;
                    break;

                default:

                    state = State.Unknown;
                    break;
            }

            Inputs.Singleton.OnDcuInputChanged(
                dcuStateAndSetting.Id,
                inputNumber,
                state);
        }

        private void OutputChanged(
            IClspNode node,
            byte outputNumber,
            bool state)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "OutputChanged - output number: " + outputNumber + ", state: " + state);

            DCUStateAndSettings dcuStateAndSettings;

            if (!_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                return;
            }

            Outputs.Singleton.OnDcuOutputRealStateChanged(
                dcuStateAndSettings.Id,
                outputNumber,
                state
                    ? State.On
                    : State.Off);
        }

        private void OutputLogicChanged(
            IClspNode node,
            byte outpuNumber,
            bool state)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "OutputLogicChanged - output number: " + outpuNumber + ", state: " + state);

            DCUStateAndSettings dcuStateAndSettings;

            if (!_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                return;
            }

            Outputs.Singleton.OnDcuOutputLogicalStateChanged(
                dcuStateAndSettings.Id,
                outpuNumber,
                state
                    ? State.On
                    : State.Off);
        }

        private void DSMChanged(
            IClspNode node,
            DoorEnvironmentState state,
            DoorEnvironmentStateDetail stateDetail,
            DoorEnvironmentAccessTrigger agSource)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "DSMChanged state: " + state);

            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                DoorEnvironments.Singleton.OnDcuDsmStateChanged(
                    dcuStateAndSettings.IdDoorEnvironment,
                    state,
                    stateDetail,
                    agSource);
            }
        }

        protected override void OnRemoved(DCUStateAndSettings removedValue)
        {
            _objectsByAddress.Remove(removedValue.LogicalAddress);
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.DCU; }
        }

        protected override DCUStateAndSettings CreateNewStateAndSettingsObject(DB.DCU dbObject)
        {
            var result = new DCUStateAndSettings(dbObject);

            _objectsByAddress.Add(
                dbObject.LogicalAddress,
                result);

            return result;
        }

        public void SendAllStates(bool isNewlyConfigured)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DCUs.SendAllStates()");

            foreach (var dcuStateAndSetting in _objects.ValuesSnapshot)
                dcuStateAndSetting.SendAllStates();

            if (!isNewlyConfigured
                || _nodeCommunicator == null)
            {
                return;
            }

            foreach (var node in _nodeCommunicator.GetNodes())
            {
                var logicalAddress = node.LogicalAddress;

                if (!node.IsOnline
                    || _objectsByAddress.ContainsKey(logicalAddress))
                {
                    continue;
                }

                Events.ProcessEvent(
                    new EventDcuOnlineStateChanged(
                        State.Online,
                        logicalAddress,
                        (byte)node.MasterProtocol));
            }
        }

        public Dictionary<Guid, bool> GetDCUTamperState()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "Dictionary<Guid, bool> DCUs.GetDCUTamperState()");

            var result = new Dictionary<Guid, bool>();

            foreach (var dcuStateAndSettings in _objects.ValuesSnapshot)
                result.Add(
                    dcuStateAndSettings.Id,
                    dcuStateAndSettings.Tamper);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "Dictionary<Guid, bool> DCUs.GetDCUTamperState return {0}",
                    Log.GetStringFromParameters(result)));

            return result;
        }

        public void SendFrame(
            IClspFrame frame,
            byte logicalAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SendFrame(Frame frame, byte logicalAddress): [{0}]",
                    Log.GetStringFromParameters(
                        frame,
                        logicalAddress)));

            try
            {
                _nodeCommunicator.EnqueueFrame(
                    logicalAddress,
                    frame);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
                CcuCore.DebugLog.Error("Send frame: " + e.Message);
            }
        }

        public void SetDcuInputBsiLevels(uint[] bsiLevels)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DCUs.SetDCUBSILevels()");

            foreach (var dcuStateAndSetting in _objects.ValuesSnapshot)
                dcuStateAndSetting.SetInputBsiLevels(bsiLevels);
        }

        public void ApplyInputHwSetup(
            [NotNull]
            DB.Input input)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetDCUInput(Input input): [{0}]",
                    Log.GetStringFromParameters(input)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                input.GuidDCU,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.ApplyInputHwSetup(input);
            }
        }

        public void ApplyOutputHwSetup(DB.Output output)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ApplyOutputHwSetup(Output output): [{0}]",
                    Log.GetStringFromParameters(output)));

            if (output == null)
                return;

            var idDcu = output.GuidDCU;

            if (idDcu == Guid.Empty)
                return;

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.ApplyOutputHwSetup(output);
            }
        }

        public void SetOutputToOn(
            Guid idDcu,
            int otuputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetOutputToOn(Guid idDcu, int otuputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        otuputNumber)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.SetOutputToOn(otuputNumber);
            }
        }

        public void SetOutputToOff(
            Guid idDcu,
            int otuputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetOutputToOff(Guid idDcu, int otuputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        otuputNumber)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.SetOutputToOff(otuputNumber);
            }
        }

        /// <summary>
        /// Forcefully sets output to off state
        /// </summary>
        /// <param name="idDcu"></param>
        /// <param name="otuputNumber"></param>
        public void ForcedSetOutputToOff(
            Guid idDcu,
            int otuputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ForcedSetOutputToOff(Guid idDcu, int otuputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        otuputNumber)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.ForcedSetOutputToOff(otuputNumber);
            }
        }

        public void DirectlyBindOutputToInput(
            Guid idDcu,
            int outputNumber,
            int inputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.DirectlyBindOutputToInput(Guid idDcu, int outputNumber, int inputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        outputNumber,
                        inputNumber)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.DirectlyBindOutputToInput(
                    outputNumber,
                    inputNumber);
            }
        }

        public void UnbindOutputFromInput(
            Guid idDcu,
            int outputNumber,
            int inputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.UnbindOutputToInput(Guid idDcu, int outputNumber, int inputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        outputNumber,
                        inputNumber)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.UnbindOutputFromInput(
                    outputNumber,
                    inputNumber);
            }
        }

        public void ApplyHwSetupForDoorEnvironmentAndStart(
            DB.DoorEnvironment newDoorEnvironment)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ApplyHwSetupForDoorEnvironment(DoorEnvironment doorEnvironment): [{0}]",
                    Log.GetStringFromParameters(newDoorEnvironment)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                newDoorEnvironment.GuidDCU,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.ApplyHwSetupForDoorEnvironmentAndStart(newDoorEnvironment);
            }
        }

        public void ApplyHwSetupForDoorEnvironmentCommon(DB.DoorEnvironment doorEnvironment)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ApplyHwSetupForDoorEnvironmentCommon(DoorEnvironment doorEnvironment): [{0}]",
                    Log.GetStringFromParameters(doorEnvironment)));

            var idDcu = doorEnvironment.GuidDCU;

            if (idDcu == Guid.Empty)
                return;

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.ApplyHwSetupForDoorEnvironmentCommon(doorEnvironment);
            }
        }

        public void ApplyHwSetupAssignDcuCardReaders(
            Guid idDcu,
            int internalCrAddress,
            ImplicitCrCodeParams internalImplicitCrCodeParams,
            int externalCrAddress,
            ImplicitCrCodeParams externalImplicitCrCodeParams)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ApplyHwSetupAssignDcuCardReaders(Guid idDcu, int internalCrAddress, ACardReaderSettings.ImplicitCrCodeParams internalImplicitCrCodeParams, int externalCrAddress, ACardReaderSettings.ImplicitCrCodeParams externalImplicitCrCodeParams): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        internalCrAddress,
                        internalImplicitCrCodeParams,
                        externalCrAddress,
                        externalImplicitCrCodeParams)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.ApplyHwSetupAssignDcuCardReaders(
                    internalCrAddress,
                    internalImplicitCrCodeParams,
                    externalCrAddress,
                    externalImplicitCrCodeParams);
            }
        }
        public void StopDoorEnvironment(Guid idDcu)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.StopDoorEnvironment(Guid idDcu): [{0}]",
                    Log.GetStringFromParameters(idDcu)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.StopDoorEnvironment();
            }
        }

        public void DoorEnvironmentAccessGranted(
            Guid idDcu,
            DsmAccessGrantedSeverity ags,
            DoorEnvironmentAccessTrigger accessTrigger)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.DoorEnvironmentAccessGranted(Guid idDcu, AGSourceCard ags, DoorEnvironmentAccessTrigger accessTrigger): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        ags,
                        accessTrigger)));

            if (idDcu == Guid.Empty)
                return;

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.DoorEnvironmentAccessGranted(
                    ags,
                    accessTrigger);
            }
        }

        public bool IsDcuOnline(Guid idDcu)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.IsDcuOnline(Guid idDcu): [{0}]",
                    Log.GetStringFromParameters(idDcu)));

            DCUStateAndSettings dcuStateAndSetting;

            return _objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting) && dcuStateAndSetting.IsOnline;
        }

        // ReSharper disable once NotAccessedField.Local
        private NativeTimer _setTimeTimer;

        private void StartSetTimeTimer()
        {
            _setTimeTimer = NativeTimerManager.StartTimer(
                SET_TIME_DELAY,
                false,
                OnSetTimeTimer);
        }

        private bool OnSetTimeTimer(NativeTimer timer)
        {
            SetTime();
            return true;
        }

        private void SetTime()
        {
            try
            {
                if (_objects == null)
                    return;

                foreach (var dcuStateAndSetting in _objects.ValuesSnapshot)
                    if (dcuStateAndSetting.IsOnline)
                        dcuStateAndSetting.SetTime();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Registers DCUs for upgrade
        /// </summary>
        /// <param name="logicalAddresses"></param>
        /// <param name="upgradeVersion"></param>
        /// <returns>Successfully registered DCUs</returns>
        public byte[] RegisterDCUsForUpgradeVersion(
            byte[] logicalAddresses,
            string upgradeVersion)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "byte[] DCUs.RegisterDCUsForUpgradeVersion(byte[] logicalAddresses, string upgradeVersion): [{0}]",
                    Log.GetStringFromParameters(
                        logicalAddresses,
                        upgradeVersion)));

            var offlineDcus = new List<byte>();

            //if there is a unavailable DCU, it will not be registered for upgrade and this dcu will be added to offline DCUs list
            foreach (var logicalAddress in logicalAddresses)
                if (_nodeCommunicator.GetNode(logicalAddress) == null)
                    offlineDcus.Add(logicalAddress);

            var onlineDCUs = new List<byte>(logicalAddresses);

            foreach (var logicalAddress in offlineDcus)
                onlineDCUs.Remove(logicalAddress);

            //continue with online DCUs registration

            foreach (var logicalAddress in onlineDCUs)
                _registeredUpgradeVersions[logicalAddress] = upgradeVersion;

            var result = onlineDCUs.ToArray();

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "byte[] DCUs.RegisterDCUsForUpgradeVersion return {0}",
                    Log.GetStringFromParameters(result)));

            return result;
        }

        public void UpgradeDCUs(
            string upgradeVersion,
            string filename)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.UpgradeDCUs(string upgradeVersion, string filename): [{0}]",
                    Log.GetStringFromParameters(
                        upgradeVersion,
                        filename)));

            foreach (var logicalAddress in GetNodesWaitingForUpgradeVersion(
                upgradeVersion,
                true))
            {
                DCUStateAndSettings dcuStateAndSettings;

                if (_objectsByAddress.TryGetValue(
                    logicalAddress,
                    out dcuStateAndSettings))
                {
                    dcuStateAndSettings.SetOnlineState(State.Upgrading);
                }

                _nodeCommunicator.UpgradeNode(
                    logicalAddress,
                    filename);
            }
        }

        public bool ProcessUpgradePackage(
            string filename,
            Stream stream,
            out UnpackPackageFailedCode errorCode,
            out byte[] nodesWaiting)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "bool DCUs.ProcessUpgradePackage(string filename, out UnpackPackageFailedCode errorCode, out byte[] nodesWaiting): [{0}]",
                        Log.GetStringFromParameters(filename)));

            nodesWaiting = new byte[]
            {
            };
            errorCode = UnpackPackageFailedCode.Other;
            if (stream == null
                && string.IsNullOrEmpty(filename))
            {
                Log.Singleton.Error("Upgrade package file name or stream is not set");
                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool DCUs.ProcessUpgradePackage return false[1]");
                return false;
            }

            Exception ex;

            string[] headerParameters =
                stream != null
                    ? FilePacker.TryGetHeaderParameters(
                        stream,
                        out ex)
                    : FilePacker.TryGetHeaderParameters(
                        filename,
                        out ex);

            if (headerParameters.Length < 4)
            {
                Log.Singleton.Error("Upgrade package has unsupported header format");
                errorCode = UnpackPackageFailedCode.UnsupportedHeaderFormat;
                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool DCUs.ProcessUpgradePackage return false[2]");
                return false;
            }
            var result = ProcessUpgradePackage(
                filename,
                stream,
                headerParameters,
                out errorCode,
                out nodesWaiting);
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool DCUs.ProcessUpgradePackage return {0}",
                    Log.GetStringFromParameters(result)));

            return true;
        }


        public bool ProcessUpgradePackage(
            string filename,
            Stream stream,
            string[] headerInfos,
            out UnpackPackageFailedCode errorCode,
            out byte[] nodesWaiting)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "bool DCUs.ProcessUpgradePackage(string filename, string[] headerInfos, out UnpackPackageFailedCode errorCode, out byte[] nodesWaiting): [{0}]",
                        Log.GetStringFromParameters(
                            filename,
                            headerInfos)));
            errorCode = UnpackPackageFailedCode.Other;
            if (headerInfos == null
                || headerInfos.Length < 5)
            {
                Log.Singleton.Error("Upgrade package has unsupported header format");
                errorCode = UnpackPackageFailedCode.UnsupportedHeaderFormat;
                nodesWaiting = new byte[]
                {
                };
                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool DCUs.ProcessUpgradePackage return false[1]");
                return false;
            }

            var hwVersion = ((DCUHWVersion)(Int32.Parse(headerInfos[1]))).ToString();
            var version = headerInfos[2];
            var unpackFileName = headerInfos[3];
            var crc32 = headerInfos[4];

            nodesWaiting = GetNodesWaitingForUpgradeVersion(version);
            var path = Path.GetDirectoryName(filename) + string.Format(
                "\\{0}({1})\\",
                version,
                hwVersion);

            if (!File.Exists(path + version + ".bin"))
            {
                Directory.CreateDirectory(
                    string.Format(
                        "{0}\\{1}({2})",
                        Path.GetDirectoryName(filename),
                        version,
                        hwVersion));
                var fp = new FilePacker();
                Exception fpError;
                if (!fp.TryUnpack(
                    stream, // use stream instead of fileName, so the stream doesn't have to be closed
                    path,
                    out fpError))
                {
                    Log.Singleton.Error("Failed to unpack upgrade package : "+fpError.Message);
                    errorCode = UnpackPackageFailedCode.UnpackFailed;
                    CcuCore.DebugLog.Warning(
                        Log.LOW_LEVEL,
                        "bool DCUs.ProcessUpgradePackage return false[2]");
                    return false;
                }

                File.Move(
                    Path.Combine(
                        path,
                        unpackFileName),
                    Path.Combine(
                        path,
                        version + ".bin"));
            }

            unpackFileName = version + ".bin";
            uint rightChecksum;
            try
            {
                rightChecksum = uint.Parse(crc32);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Log.Singleton.Error("Failed to get checksum from header");
                errorCode = UnpackPackageFailedCode.GetChecksumFailed;
                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool DCUs.ProcessUpgradePackage return false[3]");
                return false;
            }
            if (!CcuCore.ChecksumMatches(
                rightChecksum,
                Path.Combine(
                    path,
                    unpackFileName)))
            {
                Log.Singleton.Error("Checksum does not match");
                errorCode = UnpackPackageFailedCode.ChecksumDoesNotMatch;
                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool DCUs.ProcessUpgradePackage return false[4]");
                return false;
            }

            UpgradeDCUs(
                version,
                Path.Combine(
                    path,
                    unpackFileName));
            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "bool DCUs.ProcessUpgradePackage return true[1]");
            return true;
        }



        public bool UpdateUpgradeInfoFile(string upgradeVersion)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool DCUs.UpdateUpgradeInfoFile(string upgradeVersion): [{0}]",
                    Log.GetStringFromParameters(upgradeVersion)));
            StreamWriter sw = null;
            try
            {
                sw = File.CreateText(
                    Path.Combine(
                        QuickPath.ApplicationStartupDirectory,
                        UPGRADE_INFO_FILE));
                sw.Write(upgradeVersion);
                sw.Close();
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => "bool DCUs.UpdateUpgradeInfoFile return true");
                return true;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            finally
            {
                if (sw != null)
                    try
                    {
                        sw.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }
            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "bool DCUs.UpdateUpgradeInfoFile return false");
            return false;
        }

        private byte[] GetNodesWaitingForUpgradeVersion(string upgradeVersion)
        {
            return GetNodesWaitingForUpgradeVersion(
                upgradeVersion,
                false);
        }

        private byte[] GetNodesWaitingForUpgradeVersion(
            string upgradeVersion,
            bool clearAfterCall)
        {
            var result = new List<byte>();

            foreach (var registeredFile in _registeredUpgradeVersions)
                if (upgradeVersion.EndsWith(registeredFile.Value))
                    result.Add(registeredFile.Key);

            if (clearAfterCall)
                foreach (var nodeAddress in result)
                    _registeredUpgradeVersions.Remove(nodeAddress);

            return result.ToArray();
        }

        public void SetImplicitCrCode(
            Guid idDcu,
            int cardReaderAddress,
            CRMessageCode crMessageCode,
            byte[] optionalData,
            CRMessage message,
            bool isGinOrVariations)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetImplicitCrCode(Guid idDcu, int cardReaderAddress, CRMessageCode crMessageCode, int optionalData, CRMessage message, bool isGinOrVariations): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        cardReaderAddress,
                        crMessageCode,
                        optionalData,
                        message,
                        isGinOrVariations)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.SetImplicitCrCode(
                    cardReaderAddress,
                    (byte)crMessageCode,
                    optionalData,
                    message,
                    isGinOrVariations);
            }
        }

        public void SuppressCardReader(
            Guid idDcu,
            byte cardReaderAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void DCUs.SuppressCardReader(Guid idDcu, byte crAddress): [{0}]",
                        Log.GetStringFromParameters(
                            idDcu,
                            cardReaderAddress)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.SuppressCardReader(cardReaderAddress);
            }
        }

        public void LooseCardReader(
            Guid idDcu,
            byte cardReaderAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void DCUs.LooseCardReader(Guid idDcu, byte crAddress): [{0}]",
                        Log.GetStringFromParameters(
                            idDcu,
                            cardReaderAddress)));

            DCUStateAndSettings dcuStateAndSetting;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSetting))
            {
                dcuStateAndSetting.LooseCardReader(cardReaderAddress);
            }
        }

        public void SetTimeOnCardReaders()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DCUs.SetTimeOnCardReaders()");

            foreach (var settings in _objects.ValuesSnapshot)
                settings.SetTime();
        }

        public bool ResetDcu(byte dcuLogicalAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool DCUs.ResetDcu(byte dcuLogicalAddress): [{0}]",
                    Log.GetStringFromParameters(dcuLogicalAddress)));

            SendFrame(
                NodeFrame.RestartNode(),
                dcuLogicalAddress);

            return true;
        }

        public void RequestDcuMemoryLoad(byte dcuLogicalAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.RequestDcuMemoryLoad(byte dcuLogicalAddress): [{0}]",
                    Log.GetStringFromParameters(dcuLogicalAddress)));

            Singleton.SendFrame(
                NodeFrame.ReadMemoryLoad(),
                dcuLogicalAddress);
        }

        private void ReportDcuMemoryWarning(
            IClspNode node,
            int param)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objectsByAddress.TryGetValue(
                node.LogicalAddress,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SendDcuMemoryWarning((byte)param);
            }
        }

        public void ToggleADCGenerator(
            byte dcuLogicalAddress,
            bool setToOn,
            int minTime,
            int maxTime)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void DCUs.ToggleADCGenerator(byte dcuLogicalAddress, bool setToOn, int minTime, int maxTime): [{0}]",
                        Log.GetStringFromParameters(
                            dcuLogicalAddress,
                            setToOn,
                            minTime,
                            maxTime)));
            SendFrame(
                NodeFrame.ToggleADCGenerator(
                    setToOn,
                    minTime,
                    maxTime),
                dcuLogicalAddress);
        }

        public void ToggleCardGenerator(
            byte dcuLogicalAddress,
            bool setToOn,
            GeneratedCardType cardType,
            int minTime,
            int maxTime)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void DCUs.ToggleCardGenerator(byte dcuLogicalAddress, bool setToOn, GeneratedCardType cardType, int minTime, int maxTime): [{0}]",
                        Log.GetStringFromParameters(
                            dcuLogicalAddress,
                            setToOn,
                            cardType,
                            minTime,
                            maxTime)));
            SendFrame(
                NodeFrame.ToggleCardGenerator(
                    setToOn,
                    cardType,
                    minTime,
                    maxTime),
                dcuLogicalAddress);
        }

        public object[] GetActualDcuRunningTest()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "object[] DCUs.GetActualDcuRunningTest()");

            var result = new List<object>();

            foreach (var dcuStateAndSettings in _objects.ValuesSnapshot)
                if (dcuStateAndSettings != null)
                    result.Add(new DcuRunningTest(dcuStateAndSettings));

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "object[] DCUs.GetActualDcuRunningTest return {0}",
                    Log.GetStringFromParameters(result)));

            return result.ToArray();
        }

        public void SetDcuRunningTest(object values)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetDcuRunningTest(object values): [{0}]",
                    Log.GetStringFromParameters(values)));

            var test = values as DcuRunningTest;
            if (test == null)
                return;

            DCUStateAndSettings dcuStateAndSettings;

            if (!_objects.TryGetValue(
                test._idDcu,
                out dcuStateAndSettings))
                return;

            if (dcuStateAndSettings.IsOnline)
            {
                dcuStateAndSettings._toggleADC = test._toggleADC;
                dcuStateAndSettings._toggleAdcMinTime = test._toggleAdcMinTime;
                dcuStateAndSettings._toggleAdcMaxTime = test._toggleAdcMaxTime;

                ToggleADCGenerator(
                    test._address,
                    test._toggleADC,
                    test._toggleAdcMinTime,
                    test._toggleAdcMaxTime);

                dcuStateAndSettings._toggleCard = test._toggleCard;
                dcuStateAndSettings._toggleCardMinTime = test._toggleCardMinTime;
                dcuStateAndSettings._toggleCardMaxTime = test._toggleCardMaxTime;

                dcuStateAndSettings._toggleCardGeneratedCardType =
                    (GeneratedCardType)
                        test._toggleCardGeneratedCardType;

                ToggleCardGenerator(
                    test._address,
                    test._toggleCard,
                    (GeneratedCardType)
                        test._toggleCardGeneratedCardType,
                    test._toggleCardMinTime,
                    test._toggleCardMaxTime);
            }
        }

        public void AddOnAfterConnected(
            Guid idDcu,
            DVoid2Void doAfterConnected)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.AddDoAfterConnected(Guid idDcu, DVoid2Void doAfterConnected): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        doAfterConnected)));

            lock (_eventsForRunAfterConnected)
            {

                if (_eventsForRunAfterConnected.ContainsKey(idDcu))
                    _eventsForRunAfterConnected[idDcu] += doAfterConnected;
                else
                    _eventsForRunAfterConnected.Add(
                        idDcu,
                        doAfterConnected);
            }
        }

        public void RemoveDoAfterConnected(
            Guid idDcu,
            DVoid2Void doAfterConnected)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.RemoveDoAfterConnected(Guid idDcu, DVoid2Void doAfterConnected): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        doAfterConnected)));

            lock (_eventsForRunAfterConnected)
            {
                if (!_eventsForRunAfterConnected.ContainsKey(idDcu))
                    return;

                // ReSharper disable once DelegateSubtraction
                _eventsForRunAfterConnected[idDcu] -= doAfterConnected;

                if (_eventsForRunAfterConnected[idDcu] == null)
                    _eventsForRunAfterConnected.Remove(idDcu);
            }
        }

        public void RunDoAfterConnected(Guid idDcu)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.RunDoAfterConnected(Guid idDcu): [{0}]",
                    Log.GetStringFromParameters(idDcu)));

            //TODO move _eventsForRunAfterConnected into DCUStateAndSettings
            //after the "relative order" is sorted out

            lock (_eventsForRunAfterConnected)
            {
                DVoid2Void doAfterConnected;

                if (!_eventsForRunAfterConnected.TryGetValue(
                    idDcu,
                    out doAfterConnected)
                    || doAfterConnected == null)
                {
                    return;
                }

                try
                {
                    doAfterConnected();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public void SendDcuTamperStates()
        {
            foreach (var dcuStateAndSettingse in _objects.ValuesSnapshot)
                dcuStateAndSettingse.SendCurrentTamperState();
        }

        public bool StartAutoUpgrade(IClspNode node)
        {
            string defaultUpgradeFile;

            if (!IsRegisteredDefaultUpgradeVersion(out defaultUpgradeFile))
                return false;

            if (string.IsNullOrEmpty(defaultUpgradeFile))
                return false;

            if (!File.Exists(defaultUpgradeFile))
                return false;

            if (Path.GetExtension(defaultUpgradeFile)
                .ToLower() != ".bin")
                return false;

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "DCU with logical address: " + node.LogicalAddress + " AutoUpgrade");

            _nodeCommunicator.UpgradeNode(
                node.LogicalAddress,
                defaultUpgradeFile);

            return true;
        }

        public bool IsNodeUpgrading(IClspNode node)
        {
            return _nodeCommunicator.IsNodeUpgrading(node.LogicalAddress);
        }

        public void UnsetDsmSpecialOutput(
            Guid idDcu,
            SpecialOutputType specialOutputType)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.UnsetDsmSpecialOutput(specialOutputType);
            }
        }

        public void SetDsmSpecialOutput(
            Guid idDcu,
            SpecialOutputType specialOutputType,
            byte outputNumber)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.SetDsmSpecialOutput(
                    specialOutputType,
                    outputNumber);
            }
        }

        public CRCommunicator GetCrCommunicator(Guid idDcu)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (!_objects.TryGetValue(
                idDcu,
                out dcuStateAndSettings))
            {
                return null;
            }

            var clspNode = (_nodeCommunicator == null) ? null : _nodeCommunicator.GetNode(dcuStateAndSettings.LogicalAddress);

            return
                clspNode != null
                    ? NodeCommunicator.GetCrCommunicator(clspNode)
                    : null;
        }

        public IClspNode GetNode(byte logicalAddress)
        {
            if (_nodeCommunicator == null)
                return null;
            
            return _nodeCommunicator.GetNode(logicalAddress);
        }

        public int GetDcuLogicalAddress(Guid idDcu)
        {
            DCUStateAndSettings dcuStateAndSettings;

            return 
                _objects.TryGetValue(
                        idDcu,
                        out dcuStateAndSettings)
                    ? dcuStateAndSettings.LogicalAddress
                    : -1;
        }

        public void UnsetPushButton(
            Guid idDcu,
            PushButtonType pushButtonType)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.UnsetPushButton(pushButtonType);
            }
        }

        public void ApplyHwSetupForPushButton(
            Guid idDcu,
            PushButtonType pushButtonType,
            byte inputNumber)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSettings))
            {
                dcuStateAndSettings.ApplyHwSetupForPushButton(
                    pushButtonType,
                    inputNumber);
            }
        }

        public IDcuStateAndSettings GetDcuStateAndSettings(Guid idDcu)
        {
            DCUStateAndSettings dcuStateAndSettings;

            if (_objects.TryGetValue(
                idDcu,
                out dcuStateAndSettings))
            {
                return dcuStateAndSettings;
            }

            return null;
        }
    }

    public enum DCUHWVersion
    {
        Unknown = 0x00,
        RS485 = 0x01,
        Echelon = 0x02
    }
}
