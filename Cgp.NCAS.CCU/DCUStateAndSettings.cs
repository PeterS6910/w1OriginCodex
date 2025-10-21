using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.NodeDataProtocol;
using Contal.Drivers.CardReader;
using Contal.Drivers.ClspDrivers;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    // TODO TryApplyHwSettup redesign
    // All actions from OnNodeAssigned/Released/Renewed will be transfered to TryApplyHwSettup thread
    // The thread will take care of offline state as well
    internal class DCUStateAndSettings
        : AStateAndSettingsObject<DB.DCU>
        // TODO delegate implementation to lower layer if necessary
        , ICardReaderEventHandlerSurpassingOnline
        , IDcuStateAndSettings
    {
        internal class SabotageDcuListener : BoolExpressionStateChangedListener
        {
            private readonly Guid _idDcu;

            public SabotageDcuListener(Guid idDcu)
            {
                _idDcu = idDcu;
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                bool tstate = newState ?? false;
                Events.ProcessEvent(
                    new EventDcuInputsSabotage(
                        newState == null ? State.Unknown : (tstate ? State.Alarm : State.Normal), _idDcu));
            }
        }

        private const int RetryApplyHwSetupTimeout = 1000;

        private readonly byte _logicalAddress;

        private ProtocolId _protocolId;
        private bool _isInTamper;

        public Guid IdDoorEnvironment
        {
            get;
            private set;
        }

        private Guid _idOutputForDcuTamper = Guid.Empty;
        private Guid _idOutputForDcuOffline = Guid.Empty;

        public ExtendedVersion FwVersion;
        public string ActuatorKeyForTamperOutput { get { return "Tamper" + Id; } }

        public const string PROCESSING_QUEUE_NAME_CR_ONLINE_STATE = "DCUStateAndSettings: card reader online state";
        public const string PROCESSING_QUEUE_NAME_CONNECTED_DCU = "DCUStateAndSettings: connected DCU";

        public bool _toggleCard = false;
        public bool _toggleADC = false;
        public int _toggleAdcMinTime = 3;
        public int _toggleAdcMaxTime = 5;
        public int _toggleCardMinTime = 3;
        public int _toggleCardMaxTime = 5;
        public GeneratedCardType _toggleCardGeneratedCardType = GeneratedCardType.MifareCSN;

        private readonly OrOperation _sabotageDcuInputs;
        private IEnumerable<IBoolExpression> _sabotageDcuInputsOperands;

        private readonly object _lockAddRemoveHandleOnCardReaderOnlineStateChanged = new object();
        private bool _isAddedHandleOnCardReaderOnlineStateChanged;
        private SabotageDcuListener _listener;

        public byte LogicalAddress { get { return _logicalAddress; } }

        public IBoolExpression SabotageDcuInputs{ get { return _sabotageDcuInputs; } }
        public Guid SabotageDcuInputsOutputId { get; private set; }

        public void SetOnlineState(State state)
        {
            var dcu =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    Id) as DB.DCU;

            if (dcu != null)
                SetOnlineStateInternal(
                    state,
                    dcu);
        }

        private void SetOnlineStateInternal(
            State state,
            DB.DCU dcu)
        {
            ActuateOutputForDcuOffline(state);

            lock (_stateTransitionLock)
            {
                if ((_onlineState == State.Online) != (state == State.Online))
                    ++_stateTransitionsCounter;

                _onlineState = state;
            }

            if (IsOnline)
            {
                DoorEnvironments.Singleton.MarkForceDsmStart(dcu);

                lock (_stateTransitionLock)
                {
                    if (ConfigurationState == ConfigurationState.ConfigurationDone
                        || ConfigurationState == ConfigurationState.ApplyingHwSetupExisting
                        || ConfigurationState == ConfigurationState.ApplyingHwSetupNew)
                    {
                        if (_applyHwSetupThread == null)
                            _applyHwSetupThread = SafeThread.StartThread(TryApplyHwSetup);
                    }
                }
            }

            SendPhysicalConfigurationToServer();

            DcuOfflineAlarm.Update(
                dcu,
                IsOnline);

            if (IsOnline)
                return;

            if (_isInTamper)
                OnDcuTamperChanged(false);

            DoorEnvironments.Singleton.OnDcuDisconnected(dcu);
            CardReaders.Singleton.OnDcuDisconnected(dcu);

            Inputs.Singleton.OnDcuDisconnected(dcu);
            Outputs.Singleton.OnDcuDisconnected(dcu);
        }

        private void ActuateOutputForDcuOffline()
        {
            ActuateOutputForDcuOffline(_onlineState);
        }

        private readonly object _lockOuputForDcuOffline = new object();
        private readonly object _lockOuputForDcuTamper = new object();

        private void ActuateOutputForDcuOffline(State onlineState)
        {
            lock (_lockOuputForDcuOffline)
                if (_idOutputForDcuOffline != Guid.Empty)
                {
                    if (onlineState != State.Online)
                        Outputs.Singleton.On(
                            Id.ToString(),
                            _idOutputForDcuOffline);
                    else
                        Outputs.Singleton.Off(
                            Id.ToString(),
                            _idOutputForDcuOffline);
                }
        }

        private void UnconfigureOutputForDcuOffline(
            [CanBeNull]
            DB.DCU newDcu)
        {
            lock (_lockOuputForDcuOffline)
            {
                if (newDcu != null && _idOutputForDcuOffline == newDcu.GuidDcuOfflineOutput
                    || _idOutputForDcuOffline == Guid.Empty)
                {
                    return;
                }

                if (_onlineState != State.Online)
                    Outputs.Singleton.Off(
                        Id.ToString(),
                        _idOutputForDcuOffline);

                DCUs.Singleton.RemoveDoAfterConnected(
                    Outputs.Singleton.GetIdDcuForOutput(_idOutputForDcuOffline),
                    ActuateOutputForDcuOffline);

                _idOutputForDcuOffline = Guid.Empty;
            }
        }

        private void UnconfigureOutputForDcuTamper(
            [CanBeNull]
            DB.DCU newDcu)
        {
            lock (_lockOuputForDcuTamper)
                if (newDcu == null
                    || _idOutputForDcuTamper != newDcu.GuidDcuSabotageOutput)
                    if (_idOutputForDcuTamper != Guid.Empty)
                    {
                        Outputs.Singleton.Off(
                            ActuatorKeyForTamperOutput,
                            _idOutputForDcuTamper);

                        _idOutputForDcuTamper = Guid.Empty;
                    }
        }

        public bool Tamper { get { return _isInTamper; } }

        public bool IsOnline
        {
            get { return _onlineState == State.Online; }
        }

        public DCUStateAndSettings(DB.DCU dcu)
            : base(dcu.IdDCU)
        {
            _physicalAddress = string.Empty;
            _protocolId = ProtocolId.InvalidProtocol;
            _onlineState = State.Offline;

            _logicalAddress = dcu.LogicalAddress;

            IdDoorEnvironment = dcu.GuidDoorEnvironments.First();

            _sabotageDcuInputs = new OrOperation();
            
            _listener = new SabotageDcuListener(Id);

            _sabotageDcuInputs.AddListenerAndGetState(_listener);
        }

        private void ConfigureOfflineOutput(DB.DCU dcu)
        {
            lock (_lockOuputForDcuOffline)
            {
                if (_idOutputForDcuOffline != Guid.Empty)
                    return;

                _idOutputForDcuOffline = dcu.GuidDcuOfflineOutput;

                if (_idOutputForDcuOffline == Guid.Empty)
                    return;

                ActuateOutputForDcuOffline(_onlineState);

                var idTargetDcu =
                    Outputs.Singleton.GetIdDcuForOutput(_idOutputForDcuOffline);

                DCUs.Singleton.AddOnAfterConnected(
                    idTargetDcu,
                    ActuateOutputForDcuOffline);
            }
        }

        private void ApplyHwSetupSubordinateObjects()
        {
            var dcu =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    Id) as DB.DCU;

            if (dcu == null)
                return;

            foreach (var idInput in dcu.GuidInputs)
                Inputs.Singleton.ApplyHwSetupOnDcuOnline(idInput);

            foreach (var idOutput in dcu.GuidOutputs)
                Outputs.Singleton.ApplyHwSetupOnDcuOnline(idOutput);

            foreach (var idDoorEnvironment in dcu.GuidDoorEnvironments)
                DoorEnvironments.Singleton.ApplyHwSetupOnDcuOnline(idDoorEnvironment);
        }

        private SafeThread _applyHwSetupThread;

        private readonly object _stateTransitionLock = new object();
        private uint _stateTransitionsCounter;

        // TODO TryApplyHwSettup redesign
        private void TryApplyHwSetup()
        {
            lock (_tryApplyHwSetup)
                while (true)
                {
                    bool fullConfiguration;

                    lock (_stateTransitionLock)
                    {
                        if (!IsOnline)
                        {
                            _applyHwSetupThread = null;
                            return;
                        }

                        fullConfiguration = _stateTransitionsCounter > 0;
                        _stateTransitionsCounter = 0;
                    }

                    ApplyHwSetupCore();

                    if (fullConfiguration)
                    {
                        ApplyHwSetupSubordinateObjects();
                        DCUs.Singleton.RunDoAfterConnected(Id);
                    }

                    lock (_stateTransitionLock)
                        if (_stateTransitionsCounter == 0)
                        {
                            _applyHwSetupThread = null;
                            return;
                        }

                    while (true)
                    {
                        var oldStateTransitionCounter = _stateTransitionsCounter;

                        if (_retryApplyHwSetupTimeout.WaitOne(
                            RetryApplyHwSetupTimeout,
                            false))
                        {
                            _applyHwSetupThread = null;
                            return;
                        }

                        lock (_stateTransitionLock)
                        {
                            if (_stateTransitionsCounter == oldStateTransitionCounter)
                            {
                                if (IsOnline)
                                    break;

                                _applyHwSetupThread = null;
                                return;
                            }
                        }
                    }
                }
        }

        private void ApplyHwSetupCore()
        {
            SetTime();

            if (!IsOnline)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.ReadInputCount(),
                _logicalAddress);

            DCUs.Singleton.SendFrame(
                NodeFrame.ReadOutputCount(),
                _logicalAddress);

            DCUs.Singleton.SendFrame(
                NodeFrame.ReadFWVersion(),
                _logicalAddress);

            SetInputBsiLevels(Ccus.Singleton.GetBsiLevels());
        }

        private void SendPhysicalConfigurationToServer()
        {
            Events.ProcessEvent(
                new EventDcuOnlineStateChanged(
                    _onlineState,
                    _logicalAddress,
                    (byte)_protocolId));

            Events.ProcessEvent(
                new EventDcuPhysicalAddressChanged(
                    _logicalAddress,
                    _physicalAddress));
        }

        public void SendDCUInputCountToServer(byte inputCount)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SendDCUInputCountToServer(byte inputCount): [{0}]",
                    Log.GetStringFromParameters(inputCount)));

            Events.ProcessEvent(
                new EventDcuInputCount(
                    Id,
                    inputCount));
        }

        public void SendDCUOutputCountToServer(byte outputCount)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SendDCUOutputCountToServer(byte outputCount): [{0}]",
                    Log.GetStringFromParameters(outputCount)));

            Events.ProcessEvent(
                new EventDcuOutputCount(
                    Id,
                    outputCount));
        }

        public void SendDCUFWVersionToServer()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void DCUs.SendDCUFWVersionToServer()");

            if (FwVersion == null)
                return;

            Events.ProcessEvent(
                new EventDcuFirmwareVersion(
                    Id,
                    FwVersion.ToString()));
        }

        public void SendDcuMemoryWarning(byte memory)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SendDcuMemoryWarning(byte memory): [{0}]",
                    Log.GetStringFromParameters(memory)));

            Events.ProcessEvent(
                new EventDcuMemoryWarning(
                    Id,
                    memory));
        }

        public void SetInputBsiLevels(uint[] bsiLevels)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetBSILevels(uint[] bsiLevels): [{0}]",
                    Log.GetStringFromParameters(bsiLevels)));

            if (_onlineState == State.Online)
                DCUs.Singleton.SendFrame(
                    NodeFrame.SetBSILevels(
                        (ushort) bsiLevels[0],
                        (ushort) bsiLevels[1],
                        (ushort) bsiLevels[2]),
                    _logicalAddress);
        }

        /// TODO This must go away !
        public void ApplyInputHwSetup(
            [NotNull]
            DB.Input input)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetInput(Input input): [{0}]",
                    Log.GetStringFromParameters(input)));

            if (_onlineState != State.Online)
                return;

            var inputShouldBeSet = true;

            var dcu = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.DCU,
                Id) as DB.DCU;

            if (dcu != null)
                if (dcu.GuidDoorEnvironments != null)
                    foreach (var doorEnvironmentGuid in dcu.GuidDoorEnvironments)
                    {
                        var doorEnvironment =
                            Database.ConfigObjectsEngine.GetFromDatabase(
                                ObjectType.DoorEnvironment,
                                doorEnvironmentGuid) as DB.DoorEnvironment;

                        if (doorEnvironment == null)
                            continue;

                        if (doorEnvironment.GuidSensorsOpenDoors != input.IdInput
                            && doorEnvironment.GuidSensorsLockDoors != input.IdInput
                            && doorEnvironment.GuidSensorsOpenMaxDoors != input.IdInput)
                        {
                            continue;
                        }

                        inputShouldBeSet = false;
                        break;
                    }

            if (inputShouldBeSet)
            {
                DCUs.Singleton.SendFrame(
                    input.InputType == (byte)InputType.DI
                        ? NodeFrame.SetDIParams(
                            input.InputNumber,
                            CcuCore.CONTAL_NOVA_DEFAULT_FILTERTIME,
                            (uint)input.DelayToOn,
                            (uint)input.DelayToOff,
                            input.Inverted)
                        : NodeFrame.SetBSIParams(
                            input.InputNumber,
                            CcuCore.CONTAL_NOVA_DEFAULT_FILTERTIME,
                            (uint)input.DelayToOn,
                            (uint)input.DelayToOff,
                            (uint)input.TamperDelayToOn,
                            input.Inverted),
                    _logicalAddress);
            }
        }

        public void ApplyOutputHwSetup(DB.Output output)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ApplyOutputHwSetup(Output output): [{0}]",
                    Log.GetStringFromParameters(output)));

            if (output == null || _onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetReportedOutputsLogic(output.OutputNumber),
                _logicalAddress);

            if (output.ControlType == (byte)DB.OutputControl.ControlledByDSM)
                return;

            switch (output.OutputType)
            {
                case (byte)DB.OutputCharacteristic.level:
                    {
                        DCUs.Singleton.SendFrame(
                            NodeFrame.OutputConfigLevel(
                                output.OutputNumber,
                                (uint)output.SettingsDelayToOn,
                                (uint)output.SettingsDelayToOff,
                                output.Inverted),
                            _logicalAddress);

                        break;
                    }

                case (byte)DB.OutputCharacteristic.pulsed:
                    {
                        DCUs.Singleton.SendFrame(
                            NodeFrame.OutputConfigPulse(
                                output.OutputNumber,
                                (uint)output.SettingsPulseLength,
                                (uint)output.SettingsDelayToOn,
                                (uint)output.SettingsDelayToOff,
                                output.SettingsForcedToOff,
                                output.Inverted),
                            _logicalAddress);

                        break;
                    }

                case (byte)DB.OutputCharacteristic.frequency:
                    {
                        DCUs.Singleton.SendFrame(
                            NodeFrame.OutputConfigFrequency(
                                output.OutputNumber,
                                (uint)output.SettingsPulseLength,
                                (uint)output.SettingsPulseDelay,
                                (uint)output.SettingsDelayToOn,
                                (uint)output.SettingsDelayToOff,
                                output.SettingsForcedToOff,
                                output.Inverted),
                            _logicalAddress);

                        break;
                    }
            }

            if (output.RealStateChanges)
            {
                DCUs.Singleton.SendFrame(
                    NodeFrame.SetReportedOutputs(output.OutputNumber),
                    _logicalAddress);
            }
            else
            {
                DCUs.Singleton.SendFrame(
                    NodeFrame.UnsetReportedOutputs(output.OutputNumber),
                    _logicalAddress);

                Events.ProcessEvent(
                    new EventOutputRealStateChanged(
                        State.Unknown,
                        output.IdOutput));
            }
        }

        public void SetOutputToOn(int outputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetOutputToOn(int outputNumber): [{0}]",
                    Log.GetStringFromParameters(outputNumber)));

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetOutput(
                    (byte)outputNumber,
                    true),
                _logicalAddress);
        }

        public void SetOutputToOff(int outputNumber)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void DCUs.SetOutputToOff(int outputNumber): [{0}]", Log.GetStringFromParameters(outputNumber)));
            if (_onlineState == State.Online)
            {
                var frame = NodeFrame.SetOutput((byte)outputNumber, false);
                DCUs.Singleton.SendFrame(frame, _logicalAddress);
            }
        }

        public void ForcedSetOutputToOff(int outputNumber)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void DCUs.ForcedSetOutputToOff(int outputNumber): [{0}]", Log.GetStringFromParameters(outputNumber)));
            if (_onlineState == State.Online)
            {
                var frame = NodeFrame.SetOutputTotalOff((byte)outputNumber);
                DCUs.Singleton.SendFrame(frame, _logicalAddress);
            }
        }

        public void DirectlyBindOutputToInput(
            int outputNumber,
            int inputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.BindingOutputToInput(int outputNumber, int inputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        outputNumber,
                        inputNumber)));

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.BindOutputToInput(
                    (byte)outputNumber,
                    (byte)inputNumber),
                _logicalAddress);
        }

        public void UnbindOutputFromInput(int outputNumber,
            int inputNumber)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.UnbindOutputFromInput(int outputNumber, int inputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        outputNumber,
                        inputNumber)));

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.UnbindOutputFromInput(
                    (byte)outputNumber,
                    (byte)inputNumber),
                _logicalAddress);
        }

        public void UnsetDsmSpecialOutput(SpecialOutputType specialOutputType)
        {
            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.UnsetSpecialOutput(specialOutputType),
                _logicalAddress);
        }

        public void SetDsmSpecialOutput(
            SpecialOutputType specialOutputType,
            byte outputNumber)
        {
            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetSpecialOutput(
                    specialOutputType,
                    outputNumber),
                _logicalAddress);
        }

        public void ApplyHwSetupForDoorEnvironmentAndStart(DB.DoorEnvironment doorEnvironment)
        {
            try
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => string.Format(
                        "void DCUs.SetDoorEnvironment(DoorEnvironment doorEnvironment): [{0}]",
                        Log.GetStringFromParameters(doorEnvironment)));

                if (_onlineState != State.Online)
                    return;

                ApplyHwSetupForSensorsAndActuators(doorEnvironment);

                DCUs.Singleton.SendFrame(
                    NodeFrame.StartDSM(
                        (DoorEnviromentType)
                        doorEnvironment.ActuatorsDoorEnvironment),
                    _logicalAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ApplyHwSetupForSensorsAndActuators(DB.DoorEnvironment doorEnvironment)
        {
            switch (doorEnvironment.ActuatorsDoorEnvironment)
            {
                case (byte)DoorEnviromentType.Standard:

                    ApplyHwSetupForSensorOpenDoor(doorEnvironment);

                    break;

                case (byte)DoorEnviromentType.StandardWithLocking:

                    ApplyHwSetupForSensorOpenDoor(doorEnvironment);
                    ApplyHwSetupForSensorLockDoor(doorEnvironment);

                    break;

                case (byte)DoorEnviromentType.StandardWithMaxOpened:

                    ApplyHwSetupForSensorOpenDoor(doorEnvironment);
                    ApplyHwSetupForSensorOpenMaxDoor(doorEnvironment);

                    break;
                    
                case (byte)DoorEnviromentType.Turnstile:

                    ApplyHwSetupForSensorOpenDoor(doorEnvironment);
                    ApplyHwSetupForActuatorElectricStrikeOpposite(doorEnvironment);
                    ApplyHwSetupForActuatorExtraElectricStrikeOpposite(doorEnvironment);

                    break;

                default:

                    ApplyHwSetupForSensorOpenDoor(doorEnvironment);
                    ApplyHwSetupForSensorOpenMaxDoor(doorEnvironment);
                    ApplyHwSetupForSensorLockDoor(doorEnvironment);
                    ApplyHwSetupForActuatorElectricStrikeOpposite(doorEnvironment);
                    ApplyHwSetupForActuatorExtraElectricStrikeOpposite(doorEnvironment);

                    break;
            }

            ApplyHwSetupForActuatorElectricStrike(doorEnvironment);
            ApplyHwSetupForActuatorExtraElectricStrike(doorEnvironment);
            ApplyHwSetupForActuatorBypassAlarm(doorEnvironment);
        }

        //SetImplicitCRCode, SuppressCardReader, LooseCardReader, SetSpecialOutput, UnsetSpecialOutput, EnableAlarms, SetTimmings
        #region DSM settings allowed to set in run-time

        public void ApplyHwSetupForDoorEnvironmentCommon(DB.DoorEnvironment doorEnvironment)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ApplyHwSetupForDoorEnvironmentCommon(DoorEnvironment doorEnvironment): [{0}]",
                    Log.GetStringFromParameters(doorEnvironment)));

            if (!IsOnline)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetTimmings(
                    (uint)doorEnvironment.DoorTimeUnlock * 1000,
                    (uint)doorEnvironment.DoorTimeOpen * 1000,
                    (uint)doorEnvironment.DoorTimePreAlarm * 1000,
                    (uint)doorEnvironment.DoorTimeSirenAjar * 1000,
                    (uint)doorEnvironment.DoorDelayBeforeBreakIn),
                _logicalAddress);
        }

        public void SetImplicitCrCode(
            int cardReaderAddress,
            byte crMessageCode,
            byte[] optionalData,
            CRMessage message,
            bool isGinOrVariations)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SetCardReadersDefaultState(int cardReaderAddress, byte crMessageCode, int optionalData, CRMessage message, bool isGinOrVariations): [{0}]",
                    Log.GetStringFromParameters(
                        cardReaderAddress,
                        crMessageCode,
                        optionalData,
                        message,
                        isGinOrVariations)));

            if (_onlineState != State.Online)
                return;

            //If cardreader has default state GIN, GIN/Card or GIN/Card + PIN than isGinOrVariations == true
            DCUs.Singleton.SendFrame(
                NodeFrame.SetImplicitCRCode(
                    cardReaderAddress,
                    crMessageCode,
                    optionalData,
                    message,
                    isGinOrVariations),
                _logicalAddress);
        }

        public void SuppressCardReader(int cardReaderAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.SuppressCardReader(int cardReaderAddress): [{0}]",
                    Log.GetStringFromParameters(cardReaderAddress)));

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SuppressCardReader(cardReaderAddress),
                _logicalAddress);
        }

        public void LooseCardReader(int cardReaderAddress)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.LooseCardReader(int cardReaderAddress): [{0}]",
                    Log.GetStringFromParameters(cardReaderAddress)));

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.LooseCardReader(cardReaderAddress),
                _logicalAddress);
        }


        #endregion

        //AssignCardReaders, SetPushButton, UnsetPushButton, SetSensor,	UnsetSensor, SetActuator, SetBypassAlarm, UnsetActuator, UnsetBypassAlarm
        #region DSM settings allowed to set only before DSM starts

        private void ApplyHwSetupForSensorOpenDoor(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidSensorsOpenDoors == Guid.Empty)
                return;

            var sensorDoorOpened =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Input,
                    doorEnvironment.GuidSensorsOpenDoors) as DB.Input;

            if (sensorDoorOpened == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetSensor(
                    SensorType.DoorOpened,
                    sensorDoorOpened.InputNumber,
                    doorEnvironment.SensorsOpenDoorsBalanced,
                    doorEnvironment.SensorsOpenDoorsInverted,
                    (uint)sensorDoorOpened.DelayToOn,
                    (uint)sensorDoorOpened.DelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForSensorOpenMaxDoor(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidSensorsOpenMaxDoors == Guid.Empty)
                return;

            var sensorDoorFullyOpened =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Input,
                    doorEnvironment.GuidSensorsOpenMaxDoors) as DB.Input;

            if (sensorDoorFullyOpened == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetSensor(
                    SensorType.DoorFullyOpened,
                    sensorDoorFullyOpened.InputNumber,
                    doorEnvironment.SensorsOpenMaxDoorsBalanced,
                    doorEnvironment.SensorsOpenMaxDoorsInverted,
                    (uint)sensorDoorFullyOpened.DelayToOn,
                    (uint)sensorDoorFullyOpened.DelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForSensorLockDoor(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidSensorsLockDoors == Guid.Empty)
                return;

            var sensorDoorLocked =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Input,
                    doorEnvironment.GuidSensorsLockDoors) as DB.Input;

            if (sensorDoorLocked == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetSensor(
                    SensorType.DoorLocked,
                    sensorDoorLocked.InputNumber,
                    doorEnvironment.SensorsLockDoorsBalanced,
                    doorEnvironment.SensorsLockDoorsInverted,
                    (uint)sensorDoorLocked.DelayToOn,
                    (uint)sensorDoorLocked.DelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForActuatorElectricStrike(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidActuatorsElectricStrike == Guid.Empty)
                return;

            var actuatorElectricStrike =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Output,
                    doorEnvironment.GuidActuatorsElectricStrike) as DB.Output;

            if (actuatorElectricStrike == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetActuator(
                    ActuatorType.ElectricStrike,
                    actuatorElectricStrike.OutputNumber,
                    doorEnvironment.ActuatorsElectricStrikeImpulse
                        ? StrikeType.Impulse
                        : StrikeType.Level,
                    (uint)doorEnvironment.ActuatorsElectricStrikeImpulseDelay,
                    false,
                    (uint)actuatorElectricStrike.SettingsDelayToOn,
                    (uint)actuatorElectricStrike.SettingsDelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForActuatorExtraElectricStrike(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidActuatorsExtraElectricStrike == Guid.Empty)
                return;

            var actuatorExtraElectricStrike =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Output,
                    doorEnvironment.GuidActuatorsExtraElectricStrike) as DB.Output;

            if (actuatorExtraElectricStrike == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetActuator(
                    ActuatorType.ExtraElectricStrike,
                    actuatorExtraElectricStrike.OutputNumber,
                    doorEnvironment.ActuatorsExtraElectricStrikeImpulse
                        ? StrikeType.Impulse
                        : StrikeType.Level,
                    (uint)doorEnvironment.ActuatorsExtraElectricStrikeImpulseDelay,
                    false,
                    (uint)actuatorExtraElectricStrike.SettingsDelayToOn,
                    (uint)actuatorExtraElectricStrike.SettingsDelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForActuatorElectricStrikeOpposite(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidActuatorsElectricStrikeOpposite == Guid.Empty)
                return;

            var actuatorElectricStrikeOpposite =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Output,
                    doorEnvironment.GuidActuatorsElectricStrikeOpposite) as DB.Output;

            if (actuatorElectricStrikeOpposite == null)
                return;

            var strikeType = StrikeType.Level;

            if (doorEnvironment.ActuatorsElectricStrikeOppositeImpulse)
                strikeType = StrikeType.Impulse;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetActuator(
                    ActuatorType.ElectricStrikeOpposite,
                    actuatorElectricStrikeOpposite.OutputNumber,
                    strikeType,
                    (uint)doorEnvironment.ActuatorsElectricStrikeOppositeImpulseDelay,
                    false,
                    (uint)actuatorElectricStrikeOpposite.SettingsDelayToOn,
                    (uint)actuatorElectricStrikeOpposite.SettingsDelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForActuatorExtraElectricStrikeOpposite(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidActuatorsExtraElectricStrikeOpposite == Guid.Empty)
                return;

            var actuatorExtraElectricStrikeOpposite =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Output,
                    doorEnvironment.GuidActuatorsExtraElectricStrikeOpposite) as DB.Output;

            if (actuatorExtraElectricStrikeOpposite == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetActuator(
                    ActuatorType.ExtraElectricStrikeOpposite,
                    actuatorExtraElectricStrikeOpposite.OutputNumber,
                    doorEnvironment.ActuatorsExtraElectricStrikeOppositeImpulse
                        ? StrikeType.Impulse
                        : StrikeType.Level,
                    (uint)doorEnvironment.ActuatorsExtraElectricStrikeOppositeImpulseDelay,
                    false,
                    (uint)actuatorExtraElectricStrikeOpposite.SettingsDelayToOn,
                    (uint)actuatorExtraElectricStrikeOpposite.SettingsDelayToOff),
                _logicalAddress);
        }

        private void ApplyHwSetupForActuatorBypassAlarm(DB.DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.GuidActuatorsBypassAlarm == Guid.Empty)
                return;

            var actuatorBypassAlarm =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Output,
                    doorEnvironment.GuidActuatorsBypassAlarm) as DB.Output;

            if (actuatorBypassAlarm == null)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.OutputConfigLevel(
                    actuatorBypassAlarm.OutputNumber,
                    (uint)actuatorBypassAlarm.SettingsDelayToOn,
                    (uint)actuatorBypassAlarm.SettingsDelayToOff,
                    false),
                _logicalAddress);

            DCUs.Singleton.SendFrame(
                NodeFrame.SetBypassAlarm(actuatorBypassAlarm.OutputNumber),
                _logicalAddress);
        }

        public void ApplyHwSetupAssignDcuCardReaders(
            int internalCrAddress,
            ImplicitCrCodeParams internalImplicitCrCodeParams,
            int externalCrAddress,
            ImplicitCrCodeParams externalImplicitCrCodeParams)
        {
            try
            {
                if (_onlineState != State.Online)
                    return;

                var frame =
                    NodeFrame.AssignCardReaders(
                        internalCrAddress > 0
                            ? internalCrAddress
                            : 0,
                        (byte) internalImplicitCrCodeParams.ImplicitCrMessage.MessageCode,
                        internalImplicitCrCodeParams.ImplicitCrMessage.OptionalData,
                        CRControlCommands.StackedMessage(
                            internalImplicitCrCodeParams.FollowingMessages,
                            (byte) internalCrAddress),
                        internalImplicitCrCodeParams.IsGinOrVariations,
                        externalCrAddress > 0
                            ? externalCrAddress
                            : 0,
                        (byte) externalImplicitCrCodeParams.ImplicitCrMessage.MessageCode,
                        externalImplicitCrCodeParams.ImplicitCrMessage.OptionalData,
                        CRControlCommands.StackedMessage(
                            externalImplicitCrCodeParams.FollowingMessages,
                            (byte) externalCrAddress),
                        externalImplicitCrCodeParams.IsGinOrVariations);

                DCUs.Singleton.SendFrame(
                    frame,
                    _logicalAddress);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        #endregion

        public void StopDoorEnvironment()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void DCUs.StopDoorEnvironment()");

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.StopDSM(),
                _logicalAddress);
        }

        public void DoorEnvironmentAccessGranted(
            DsmAccessGrantedSeverity ags,
            DoorEnvironmentAccessTrigger accessTrigger)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.DoorEnvironmentAccessGranted(AGSourceCard ags, DoorEnvironmentAccessTrigger accessTrigger): [{0}]",
                    Log.GetStringFromParameters(ags, accessTrigger)));

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SignalAccessGranted(
                    ags,
                    accessTrigger),
                _logicalAddress);
        }

        public void SetTime()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DCUStateAndSettings.SetTime()");

            if (_onlineState != State.Online)
                return;

            DCUs.Singleton.SendFrame(
                NodeFrame.SetTime(CcuCore.LocalTime),
                _logicalAddress);
        }

        public void OnDcuTamperChanged(bool isInTamper)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.DCUTamper(bool isInTamper): [{0}]",
                    Log.GetStringFromParameters(isInTamper)));

            _isInTamper = isInTamper;

            Tampers.SendDcuTamper(
                true,
                _isInTamper,
                Id);

            ActuateOutputForDcuTamper();
        }

        private void ActuateOutputForDcuTamper()
        {
            lock (_lockOuputForDcuTamper)
            {
                if (_idOutputForDcuTamper == Guid.Empty)
                    return;

                if (_isInTamper)
                    Outputs.Singleton.On(
                        ActuatorKeyForTamperOutput,
                        _idOutputForDcuTamper);
                else
                    Outputs.Singleton.Off(
                        ActuatorKeyForTamperOutput,
                        _idOutputForDcuTamper);

            }
        }

        private readonly Version VER_485_STACKED_MESSAGE_SUPPORT = new Version(1, 7, 1580);
        private readonly Version VER_LON_STACKED_MESSAGE_SUPPORT = new Version(1, 9, 1620);

        /// <summary>
        /// Returns information about StackedMessage DCU support
        /// </summary>
        /// <returns></returns>
        public bool IsStackedMessageSupported()
        {
            if (FwVersion == null)
                return false;

            return
                (MainBoardVariant)MainBoard.Variant == MainBoardVariant.CCU0_RS485
                    && FwVersion >= VER_485_STACKED_MESSAGE_SUPPORT
                || (MainBoardVariant)MainBoard.Variant == MainBoardVariant.CCU0_ECHELON
                    && FwVersion >= VER_LON_STACKED_MESSAGE_SUPPORT;
        }

        private readonly Version VER_485_AUTO_REFRESH_AFTER_SET_COUNTRY_CODE = new Version(1, 7, 1570);
        private readonly Version VER_LON_AUTO_REFRESH_AFTER_SET_COUNTRY_CODE = new Version(1, 9, 1610);


        /// <summary>
        /// Returns information about automatic refresh after set launguage support
        /// </summary>
        /// <returns></returns>
        public bool IsAutomaticRefreshAfterSetLaunguageSupported()
        {
            return ((MainBoardVariant)MainBoard.Variant == MainBoardVariant.CCU0_RS485 &&
                    FwVersion >= VER_485_AUTO_REFRESH_AFTER_SET_COUNTRY_CODE)
                   || ((MainBoardVariant)MainBoard.Variant == MainBoardVariant.CCU0_ECHELON &&
                       FwVersion >= VER_LON_AUTO_REFRESH_AFTER_SET_COUNTRY_CODE);
        }

        private readonly Version VER_LON_AUTO_LOOSE_CR_AFTER_RESTART = ExtendedVersion.CreateSimple(1, 9, 1633);

        private string _physicalAddress;

        private State _onlineState;

        private readonly object _tryApplyHwSetup = new object();

        private readonly AutoResetEvent _retryApplyHwSetupTimeout = new AutoResetEvent(false);

        /// <summary>
        /// Returns true if automatic loose card reader after restart is supported otherwise returns false
        /// </summary>
        /// <returns></returns>
        public bool AutomaticLooseCardReaderAfterRestartSupported()
        {
            return
                (MainBoardVariant)MainBoard.Variant == MainBoardVariant.CCU0_ECHELON
                && FwVersion >= VER_LON_AUTO_LOOSE_CR_AFTER_RESTART;
        }

        public void SendAllStates()
        {
            SendPhysicalConfigurationToServer();

            _listener.OnStateChanged(
                _sabotageDcuInputs.State,
                _sabotageDcuInputs.State);
          
            SendDCUFWVersionToServer();
        }

        private static void ConfigureAlarms(DB.DCU dcu)
        {
            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.DCU_Offline,
                new IdAndObjectType(
                    dcu.IdDCU,
                    ObjectType.DCU),
                dcu.AlarmOffline,
                dcu.BlockAlarmOffline,
                dcu.ObjBlockAlarmOfflineId,
                dcu.ObjBlockAlarmOfflineObjectType,
                true);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.DCU_TamperSabotage,
                new IdAndObjectType(
                    dcu.IdDCU,
                    ObjectType.DCU),
                dcu.AlarmTamper,
                dcu.BlockAlarmTamper,
                dcu.ObjBlockAlarmTamperId,
                dcu.ObjBlockAlarmTamperObjectType,
                dcu.EventlogDuringBlockAlarmTamper);

            if (dcu.AlarmTypeAndIdAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        dcu.IdDCU,
                        ObjectType.DCU),
                    dcu.AlarmTypeAndIdAlarmArcs);
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    new IdAndObjectType(
                        dcu.IdDCU,
                        ObjectType.DCU));
        }

        public void SendCurrentTamperState()
        {
            Tampers.SendDcuTamper(
                false,
                Tamper,
                Id);
        }

        private void OnNodeAssignedCommon(IClspNode node)
        {
            var dcu =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    Id) as DB.DCU;

            if (dcu == null)
                return;

            lock (_lockAddRemoveHandleOnCardReaderOnlineStateChanged)
            {
                if (!_isAddedHandleOnCardReaderOnlineStateChanged)
                {
                    _isAddedHandleOnCardReaderOnlineStateChanged = true;

                    var crNestedCommunicator = node.GetCrCommunicator();

                    if (crNestedCommunicator != null)
                    {
                        crNestedCommunicator.EventHandlerSurpassingOnline.Add(this);
                        
                    }

                }
            }

            switch (node.MasterProtocol)
            {
                case ProtocolId.ProtoAccess:

                    _protocolId = ProtocolId.ProtoAccess;
                    _physicalAddress = node.PhysicalAddress;

                    CardReaders.Singleton.OnDcuDisconnected(dcu);

                    SetOnlineStateInternal(
                        State.Online,
                        dcu);

                    return;

                case ProtocolId.ProtoUploader:

                    _protocolId = ProtocolId.ProtoUploader;

                    ConsiderUpgradeAction(
                        node,
                        dcu);

                    return;
            }
        }

        void ICardReaderEventHandlerSurpassingOnline.CardReaderUpgradeResult(CardReader cr, CRUpgradeResult result, Exception error)
        {
            Events.ProcessEvent(
                new EventCrUpgradeResultSet(
                    LogicalAddress,
                    cr.Address,
                    (byte)result));
        }

        void ICardReaderEventHandlerSurpassingOnline.CardReaderUpgradeProgress(CardReader cr, int progress)
        {
            Events.ProcessEvent(
                new EventCrUpgradePercentageSet(
                    LogicalAddress,
                    cr.Address,
                    progress));
        }

        void ICardReaderEventHandlerSurpassingOnline.CardReaderSabotageStateChanged(CardReader cr, bool tamperOn)
        {
            
        }

        void ICardReaderEventHandlerSurpassingOnline.CardReaderOnlineStateChanged(
            [NotNull]
            CardReader cardReader,
            bool isonline)
        {
            if (CardReaders.Singleton.OnOnlineStateChanged(
                Id,
                cardReader))
            {
                return;
            }

            Events.ProcessEvent(
                new EventCardReaderOnlineStateChanged(
                    cardReader.IsOnline,
                    LogicalAddress,
                    string.Empty,
                    cardReader.Address,
                    cardReader.ProtocolVersion,
                    cardReader.FirmwareVersion,
                    ((byte)cardReader.HardwareVersion).ToString(
                        CultureInfo.InvariantCulture),
                    cardReader.ProtocolVersionHigh));
        }

        private void ConsiderUpgradeAction(
            IClspNode node,
            DB.DCU dcu)
        {
            if (!DCUs.Singleton.IsNodeUpgrading(node))
            {
                SetOnlineStateInternal(
                    DCUs.Singleton.StartAutoUpgrade(node)
                        ? State.AutoUpgrading
                        : State.WaitingForUpgrade,
                    dcu);

                return;
            }

            if (_onlineState == State.Upgrading)
                return;

            SetOnlineStateInternal(
                State.Upgrading,
                dcu);

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                "DCU with logical address: " + node.LogicalAddress + " Upgrade");
        }

        // TODO TryApplyHwSettup redesign
        public void OnNodeAssigned(IClspNode node)
        {
            Events.ProcessEvent(new EventDcuNodeAssigned(Id));

            OnNodeAssignedCommon(node);
        }

        // TODO TryApplyHwSettup redesign
        public void OnNodeRenewed(IClspNode node)
        {
            Events.ProcessEvent(new EventDcuNodeRenewed(Id));

            OnNodeReleasedCommon(node);
            OnNodeAssignedCommon(node);
        }

        public void OnNodeReleasedCommon(IClspNode node)
        {
            var dcu =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    Id) as DB.DCU;

            if (dcu == null)
                return;

            lock (_lockAddRemoveHandleOnCardReaderOnlineStateChanged)
            {
                if (_isAddedHandleOnCardReaderOnlineStateChanged)
                {
                    _isAddedHandleOnCardReaderOnlineStateChanged = false;

                    var crNestedCommunicator = node.GetCrCommunicator();

                    if (crNestedCommunicator != null)
                    {
                        crNestedCommunicator.EventHandlerSurpassingOnline.Remove(this);
                    }
                }
            }

            switch (node.MasterProtocol)
            {
                case ProtocolId.ProtoAccess:

                    _physicalAddress = string.Empty;
                    _protocolId = ProtocolId.InvalidProtocol;

                    SetOnlineStateInternal(
                        State.Offline,
                        dcu);

                    return;

                case ProtocolId.ProtoUploader:

                    _protocolId = ProtocolId.InvalidProtocol;

                    SetOnlineStateInternal(
                        State.Offline,
                        dcu);

                    return;
            }
        }

        // TODO TryApplyHwSettup redesign
        public void OnNodeReleased(IClspNode node)
        {
            Events.ProcessEvent(new EventDcuNodeReleased(Id));

            OnNodeReleasedCommon(node);
        }

        protected override void ConfigureInternal(DB.DCU dcu)
        {
            IClspNode node = DCUs.Singleton.GetNode(_logicalAddress);

            if (node != null)
            {
                _onlineState = node.IsOnline
                    ? State.Online
                    : State.Offline;

                if (ConfigurationState == ConfigurationState.ConfiguringNew)
                {
                    _isInTamper = node.GetParameter<bool>(DCUs.TAMPER_PARAM);

                    Tampers.SendDcuTamper(
                        true,
                        _isInTamper,
                        Id);

                    NotifyServerAboutNewCardReaders(node);

                    // TODO TryApplyHwSettup redesign
                    lock (_lockAddRemoveHandleOnCardReaderOnlineStateChanged)
                    {
                        if (!_isAddedHandleOnCardReaderOnlineStateChanged)
                        {
                            _isAddedHandleOnCardReaderOnlineStateChanged = true;

                            var crNestedCommunicator = node.GetCrCommunicator();

                            if (crNestedCommunicator != null)
                            {
                                crNestedCommunicator.EventHandlerSurpassingOnline.Add(this);
                            }
                        }
                    }
                }

                _physicalAddress = node.PhysicalAddress;
                _protocolId = node.MasterProtocol;
            }

            if (ConfigurationState == ConfigurationState.ConfiguringNew)
                DcuOfflineAlarm.Update(
                    dcu,
                    _onlineState == State.Online);
            
            var idInputs = dcu.GuidInputs;

            _sabotageDcuInputsOperands =
                new LinkedList<IBoolExpression>(
                    idInputs != null
                        ? idInputs
                            .Select(idInput => Inputs.Singleton.GetBoolExpresionInputTamper(idInput))
                            .Where(boolExpression => boolExpression != null)
                        : Enumerable.Empty<IBoolExpression>());

            _sabotageDcuInputs.AddOperands(_sabotageDcuInputsOperands);
            
            _sabotageDcuInputs.Resume();

            lock (_lockOuputForDcuTamper)
                if (_idOutputForDcuTamper == Guid.Empty)
                {
                    _idOutputForDcuTamper = dcu.GuidDcuSabotageOutput;

                    if (_idOutputForDcuTamper != Guid.Empty && _isInTamper)
                        Outputs.Singleton.On(
                            ActuatorKeyForTamperOutput,
                            _idOutputForDcuTamper);
                }

            ConfigureOfflineOutput(dcu);
            ConfigureAlarms(dcu);

            SabotageDcuInputsOutputId = dcu.GuidDcuInputsSabotageOutput;
        }

        private void NotifyServerAboutNewCardReaders(IClspNode node)
        {
            var crNestedCommunicator = node.GetCrCommunicator();

            if (crNestedCommunicator == null)
                return;

            foreach (var cardReader in  crNestedCommunicator.AllCardReaders)
            {
                if (!cardReader.IsOnline
                    || CardReaders.Singleton.CardReaderExists(
                        Id,
                        cardReader.Address))
                {
                    continue;
                }

                Events.ProcessEvent(
                    new EventCardReaderOnlineStateChanged(
                        cardReader.IsOnline,
                        LogicalAddress,
                        string.Empty,
                        cardReader.Address,
                        cardReader.ProtocolVersion,
                        cardReader.FirmwareVersion,
                        ((byte)cardReader.HardwareVersion).ToString(
                            CultureInfo.InvariantCulture),
                        cardReader.ProtocolVersionHigh));
            }
        }

        protected override void ApplyHwSetup(DB.DCU dbObject)
        {
            if (ConfigurationState == ConfigurationState.ApplyingHwSetupNew
                && _onlineState == State.Online)
            {
                SendPhysicalConfigurationToServer();
            }

            _retryApplyHwSetupTimeout.Set();

            lock (_tryApplyHwSetup)
            {
                _retryApplyHwSetupTimeout.Reset();

                _applyHwSetupThread = SafeThread.StartThread(TryApplyHwSetup);
            }
        }

        protected override void UnconfigureInternal(DB.DCU newDbObject)
        {
            if (newDbObject == null)
            {
                var clspNode = DCUs.Singleton.GetNode(LogicalAddress);

                if (clspNode != null)
                {
                    // TODO TryApplyHwSettup redesign
                    lock (_lockAddRemoveHandleOnCardReaderOnlineStateChanged)
                    {
                        if (_isAddedHandleOnCardReaderOnlineStateChanged)
                        {
                            _isAddedHandleOnCardReaderOnlineStateChanged = false;

                            var crNestedCommunicator = clspNode.GetCrCommunicator();

                            if (crNestedCommunicator != null)
                                crNestedCommunicator.EventHandlerSurpassingOnline.Remove(this);
                        }
                    }
                }
            }

            _retryApplyHwSetupTimeout.Set();

            lock (_tryApplyHwSetup)
                _retryApplyHwSetupTimeout.Reset();

            _sabotageDcuInputs.Suspend();

            _sabotageDcuInputs.RemoveOperands(_sabotageDcuInputsOperands);

            UnconfigureOutputForDcuTamper(newDbObject);
            UnconfigureOutputForDcuOffline(newDbObject);

            LogicalConfigurator.LogicalConfigurator.Singleton.Unconfigure(
                new IdAndObjectType(
                    Id,
                    ObjectType.DCU),
                newDbObject);
        }

        public void ApplyHwSetupForPushButton(
            PushButtonType pushButtonType,
            byte inputNumber)
        {
            if (_onlineState == State.Online)
                DCUs.Singleton.SendFrame(
                    NodeFrame.SetPushButton(
                        pushButtonType,
                        inputNumber,
                        true),
                    _logicalAddress);
        }

        public void UnsetPushButton(PushButtonType pushButtonType)
        {
            if (_onlineState == State.Online)
                DCUs.Singleton.SendFrame(
                    NodeFrame.UnsetPushButton(pushButtonType),
                    _logicalAddress);
        }
    }
}