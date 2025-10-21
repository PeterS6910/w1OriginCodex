using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal class OutputStateAndSettings : AStateAndSettingsObject<DB.Output>, IOutputConfiguration
    {
        public static readonly Guid WatchdogGuid = new Guid("11111111-2222-3333-4444-555555555555");
        public static readonly Guid ForcedOnGuid = new Guid("22222222-3333-4444-5555-666666666666");

        private State _outputState = State.Unknown;

        private State _outputRealState;

        private DB.OutputCharacteristic _outputType = DB.OutputCharacteristic.Unconfigured;

        private DB.OutputControl _controlType;

        private readonly Guid _idDcu;

        private int _delayToOn;

        private int _delayToOff;

        private int _pulseLength;

        private int _pulseDelay;

        private bool _forcedOff;

        private bool _sendRealStateChanges;

        private bool _inverted;

        private bool _alarmControlByObjOn;

        private bool _hwSetupDone;

        private readonly OutputController _outputController;

        public State OutputState
        {
            get { return _outputState; }
        }

        public State OutputRealState
        {
            get { return _outputRealState; }
        }

        public DB.OutputCharacteristic OutputType
        {
            get { return _outputType; }
        }

        public IOControl.OutputMode OutputMode
        {
            get
            {
                switch (_outputType)
                {
                    case DB.OutputCharacteristic.frequency:
                        return IOControl.OutputMode.Frequency;

                    case DB.OutputCharacteristic.pulsed:
                        return IOControl.OutputMode.Pulse;

                    default:
                        return IOControl.OutputMode.Level;
                }
            }
        }

        public int DelayToOn
        {
            get { return _delayToOn; }
        }

        public int DelayToOff
        {
            get { return _delayToOff; }
        }

        public int PulseLength
        {
            get { return _pulseLength; }
        }

        public int PulseDelay
        {
            get { return _pulseDelay; }
        }

        public bool ForcedOff
        {
            get { return _forcedOff; }
        }

        public bool IsOrLogic { get { return false; } }

        public byte OutputNumber
        {
            get;
            private set;
        }

        public bool SendRealStateChanges
        {
            get { return _sendRealStateChanges; }
        }

        public Guid IdDcu
        {
            get { return _idDcu; }
        }

        public bool Inverted
        {
            get { return _inverted; }
        }

        public OutputStateAndSettings(
            Guid idOutput,
            Guid idDcu,
            byte outputNumber,
            State initialState)
            : base (idOutput)
        {
            _idDcu = idDcu;

            OutputNumber = outputNumber;

            _outputState = State.Unknown;
            _outputRealState = State.Unknown;

            _outputState = initialState;

            _outputController = new OutputController(this);
        }

        public static OutputStateAndSettings CreateNewOutputStateAndSettings(DB.Output output)
        {
            if (output.GuidCCU != Guid.Empty)
            {
                if (SharedResources.Singleton.GetOutputAccess(output.OutputNumber)
                    != SROutputAccess.FullAccess)
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "Output used by another aplication: " + output.OutputNumber);

                    return
                        new OutputStateAndSettings(
                            output.IdOutput,
                            Guid.Empty,
                            output.OutputNumber,
                            State.UsedByAnotherAplication);
                }

                if (output.OutputNumber >= Outputs.GetOutputCount())
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "Output out of range: " + output.OutputNumber);

                    return
                        new OutputStateAndSettings(
                            output.IdOutput,
                            Guid.Empty,
                            output.OutputNumber,
                            State.OutOfRange);
                }
            }

            return new OutputStateAndSettings(
                output.IdOutput,
                output.GuidDCU,
                output.OutputNumber,
                State.Unknown);
        }

        protected override void ConfigureInternal(DB.Output output)
        {
            _hwSetupDone = false;

            if (OutputState == State.UsedByAnotherAplication
                || OutputState == State.OutOfRange)
            {
                return;
            }

            _outputType = (DB.OutputCharacteristic)output.OutputType;
            _controlType = (DB.OutputControl)output.ControlType;

            _onOffObjectId = output.OnOffObjectId;
            _onOffObjectType = output.OnOffObjectObjectType;

            _delayToOn = output.SettingsDelayToOn;
            _delayToOff = output.SettingsDelayToOff;

            _pulseLength = output.SettingsPulseLength;
            _pulseDelay = output.SettingsPulseDelay;

            _forcedOff = output.SettingsForcedToOff;

            _sendRealStateChanges = output.RealStateChanges;
            _inverted = output.Inverted;

            _alarmControlByObjOn = output.AlarmControlByObjOn;

            if (_controlType == DB.OutputControl.ControlledByObject)
                SetOutputControlByObject();
        }

        protected override void ApplyHwSetup(DB.Output output)
        {
            if (OutputState == State.UsedByAnotherAplication
                || OutputState == State.OutOfRange)
            {
                return;
            }

            if (output.GuidDCU != Guid.Empty)
                DCUs.Singleton.ApplyOutputHwSetup(output);
            else
                _outputController.ApplyOutputHwSetup(this);

            _hwSetupDone = true;

            switch (output.ControlType)
            {
                case (byte)DB.OutputControl.ForcedOn:

                    _outputController.On(ForcedOnGuid.ToString());

                    break;

                case (byte)DB.OutputControl.ManuallyBlocked:

                    _outputController.ManualOff();

                    //output should not trigger its activators when it is in manualy blocked mode
                    return;

                case (byte)DB.OutputControl.ControlledByObject:

                    if (_onOffObjectId.HasValue)
                        if (!TryDirectBindToInput())
                            OnControllingObjectStateChanged(
                                _onOffObjectId.Value,
                                StateObjects.GetObjectState(
                                    _onOffObjectType,
                                    _onOffObjectId.Value));

                    break;
            }

            Ccus.Singleton.WatchdogOutputActivator(Id);

            _outputController.ReEvaluateOutputActivators();
        }

        private bool TryDirectBindToInput()
        {
            if (_onOffObjectType != ObjectType.Input
                || !_onOffObjectId.HasValue)
            {
                return false;
            }

            return
                Inputs.Singleton.TryDirectBindToInput(
                    _onOffObjectId.Value,
                    this);
        }

        public bool IsOutputBlocked
        {
            get { return _controlType == DB.OutputControl.ManuallyBlocked; }
        }

        public void SetOutputLogicalState(State state)
        {
            if (_outputState == state)
                return;

            _outputState = state;

            Outputs.Singleton.RunOutputChanged(
                Id,
                _outputState);

            OutputAlarm.Update(
                Id,
                _outputState,
                _alarmControlByObjOn);

            SendOutputStateChanged();
        }

        public void SendOutputStateChanged()
        {
            Events.ProcessEvent(
                new EventOutputStateChanged(
                    _outputState,
                    Id));
        }

        public void SetOutputRealState(State state)
        {
            _outputRealState = state;
            SendOutputRealStateChanged();
        }

        public void SendOutputRealStateChanged()
        {
            if (SendRealStateChanges)
                Events.ProcessEvent(
                    new EventOutputRealStateChanged(
                        _outputRealState,
                        Id));
        }

        private void OnControllingObjectStateChanged(
            Guid idObject,
            State state)
        {
            if (!_hwSetupDone)
                return;

            switch (state)
            {
                case State.Normal:
                case State.Off:

                    _outputController.Off(idObject.ToString());
                    break;

                case State.Alarm:
                case State.On:

                    _outputController.On(idObject.ToString());

                    break;
            }
        }

        private Guid? _onOffObjectId;

        private ObjectType _onOffObjectType;

        public void SetOutputControlByObject()
        {
            if (_onOffObjectId == null)
                return;

            Guid onOffObjectId = _onOffObjectId.Value;

            switch (_onOffObjectType)
            {
                case ObjectType.Input:

                    Inputs.Singleton.BindOutputToInput(
                        onOffObjectId,
                        this);

                    break;

                case ObjectType.Output:

                    if (InterCcuCommunication.Singleton.SetObjectActualState(
                        ObjectType.Output,
                        Id))
                    {
                        return;
                    }

                    Outputs.Singleton.AddOutputChanged(
                        onOffObjectId,
                        OnControllingObjectStateChanged);

                    break;

                case ObjectType.TimeZone:

                    TimeZones.Singleton.AddEvent(
                        onOffObjectId,
                        OnControllingObjectStateChanged);

                    break;

                case ObjectType.DailyPlan:

                    DailyPlans.Singleton.AddEvent(
                        onOffObjectId,
                        OnControllingObjectStateChanged);

                    break;
            }
        }

        public void OnBoundInputStateChanged(
            Guid idInput,
            Guid idDcu,
            State state)
        {
            if (_idDcu != Guid.Empty && idDcu == _idDcu)
                return;

            OnControllingObjectStateChanged(
                idInput,
                state);
        }

        public void OnBoundInputUnblocked(InputStateAndSettings inputStateAndSettings)
        {
            var idDcu = inputStateAndSettings.IdDcu;

            if (_idDcu == Guid.Empty
                || _idDcu != idDcu)
            {
                return;
            }

            _outputController.Off(inputStateAndSettings.Id.ToString());

            DCUs.Singleton.DirectlyBindOutputToInput(
                idDcu,
                OutputNumber,
                inputStateAndSettings.InputNumber);
        }

        public void OnBoundInputBlocked(
            InputStateAndSettings inputStateAndSettings,
            bool requestedOutputState)
        {
            if (_idDcu == Guid.Empty
                || _idDcu != inputStateAndSettings.IdDcu)
            {
                return;
            }

            if (requestedOutputState)
                _outputController.On(inputStateAndSettings.Id.ToString());
            else
                _outputController.Off(inputStateAndSettings.Id.ToString());

            DCUs.Singleton.UnbindOutputFromInput(
                IdDcu,
                OutputNumber,
                inputStateAndSettings.InputNumber);
        }

        protected override void UnconfigureInternal(
            DB.Output newOutput)
        {
            if (newOutput == null)
            {
                if (WindowsCE.Build > NCASConstants.FW_VERSION_ENABLED_UNCONFIGURE_OUTPUT)
                    _outputController.Unconfigure();
                else
                    _outputController.TotalOff();
            }
            else
                ProcessOutputControlledByChange(newOutput);

            if (_controlType != DB.OutputControl.ControlledByObject
                || _onOffObjectId == null)
            {
                return;
            }

            switch (_onOffObjectType)
            {
                case ObjectType.Input:

                    Inputs.Singleton.UnbindOutputFromInput(
                        _onOffObjectId.Value,
                        this);

                    break;

                case ObjectType.Output:

                    Outputs.Singleton.RemoveOutputChanged(
                        _onOffObjectId.Value,
                        OnControllingObjectStateChanged);

                    break;

                case ObjectType.TimeZone:

                    TimeZones.Singleton.RemoveEvent(
                        _onOffObjectId.Value,
                        OnControllingObjectStateChanged);

                    break;

                case ObjectType.DailyPlan:

                    DailyPlans.Singleton.RemoveEvent(
                        _onOffObjectId.Value,
                        OnControllingObjectStateChanged);

                    break;
            }
        }

        public void ProcessOutputControlledByChange(
            [NotNull]
            DB.Output newOutput)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void OutputManager.ProcessOutputControlledByChange(Output newOutput): [{0}]",
                    Log.GetStringFromParameters(
                        newOutput)));

            if (_controlType == (DB.OutputControl)newOutput.ControlType)
            {
                //object for output control remains the same, in these specific cases there is no need to change
                if (_controlType == DB.OutputControl.ControlledByObject)
                    ProcessPossibleControlObjectIdChange(newOutput);
            }
            else
                SwitchControlTypes(newOutput);
        }

        private void SwitchControlTypes(
        [NotNull]
        DB.Output newOutput)
        {
            switch (_controlType)
            {
                case DB.OutputControl.unblocked:

                    SwitchFromUnblockedOutput(newOutput);
                    break;

                case DB.OutputControl.ManuallyBlocked:

                    SwitchFromManualyBlockedOutput(newOutput);
                    break;

                case DB.OutputControl.ForcedOn:

                    SwitchFromForcedOn(newOutput);
                    break;

                case DB.OutputControl.watchdog:

                    SwitchFromWatchdog(newOutput);
                    break;

                case DB.OutputControl.ControlledByObject:

                    SwitchFromOnOffObject(newOutput);
                    break;
            }
        }

        private void SwitchFromOnOffObject(DB.Output newOutput)
        {
            //outputs controlled by input from the same DCU are treated different
            var wasOnOffObjectFromSameDCU = OnOffObjectFromSameDCU();

            switch ((DB.OutputControl)newOutput.ControlType)
            {
                case DB.OutputControl.ControlledByDSM:
                case DB.OutputControl.unblocked:

                    if (!wasOnOffObjectFromSameDCU
                        && IsOnOffObjectStateOn())
                    {
                        _outputController.Off(_onOffObjectId.Value.ToString());
                    }

                    break;

                case DB.OutputControl.ManuallyBlocked:

                    if (!wasOnOffObjectFromSameDCU
                        && IsOnOffObjectStateOn())
                    {
                        _outputController.Off(_onOffObjectId.Value.ToString());
                    }

                    _outputController.ManualOff();

                    break;

                case DB.OutputControl.ForcedOn:

                    if (!wasOnOffObjectFromSameDCU
                        && IsOnOffObjectStateOn())
                    {
                        _outputController.AddOrReplaceOutputActivator(
                            _onOffObjectId.Value.ToString(),
                            ForcedOnGuid.ToString());
                    }
                    else
                        if (wasOnOffObjectFromSameDCU
                            || !IsOnOffObjectStateOn())
                        {
                            _outputController.On(ForcedOnGuid.ToString());
                        }

                    break;

                case DB.OutputControl.watchdog:

                    if (CcuCore.Singleton.IsAuthenticatedClientConnected())
                    {
                        if (!wasOnOffObjectFromSameDCU
                            && IsOnOffObjectStateOn())
                        {
                            _outputController.Off(_onOffObjectId.Value.ToString());
                        }
                    }
                    else
                    {
                        if (!wasOnOffObjectFromSameDCU
                            && IsOnOffObjectStateOn())
                        {
                            _outputController.AddOrReplaceOutputActivator(
                                _onOffObjectId.ToString(),
                                WatchdogGuid.ToString());
                        }
                        else
                            if (wasOnOffObjectFromSameDCU
                                || !IsOnOffObjectStateOn())
                            {
                                _outputController.On(WatchdogGuid.ToString());
                            }
                    }

                    break;
            }
        }

        private static bool IsOnOffObjectStateOn(
            ObjectType onOffObjectType,
            Guid? onOffObjectId)
        {
            if (!onOffObjectId.HasValue)
                return false;

            State state =
                StateObjects.GetObjectState(
                    onOffObjectType,
                    onOffObjectId.Value);

            return
                state == State.On || state == State.Alarm;
        }

        private bool IsOnOffObjectStateOn()
        {
            return IsOnOffObjectStateOn(
                _onOffObjectType,
                _onOffObjectId);
        }

        private static bool IsOnOffObjectStateOn(DB.Output newOutput)
        {
            return
                IsOnOffObjectStateOn(
                    newOutput.OnOffObjectObjectType,
                    newOutput.OnOffObjectId);
        }

        private void SwitchFromWatchdog(DB.Output newOutput)
        {
            switch ((DB.OutputControl)newOutput.ControlType)
            {
                case DB.OutputControl.ControlledByDSM:
                case DB.OutputControl.unblocked:

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                        _outputController.Off(WatchdogGuid.ToString());

                    break;

                case DB.OutputControl.ManuallyBlocked:

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                        _outputController.Off(WatchdogGuid.ToString());

                    _outputController.ManualOff();

                    break;

                case DB.OutputControl.ForcedOn:

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                        _outputController.AddOrReplaceOutputActivator(
                            WatchdogGuid.ToString(),
                            ForcedOnGuid.ToString());
                    else
                        _outputController.On(ForcedOnGuid.ToString());

                    break;

                case DB.OutputControl.ControlledByObject:

                    var onOffObjectFromSameDCU = OnOffObjectFromSameDCU(newOutput);

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                    {
                        if (!onOffObjectFromSameDCU
                            && IsOnOffObjectStateOn(newOutput))
                        {
                            _outputController.AddOrReplaceOutputActivator(
                                WatchdogGuid.ToString(),
                                newOutput.OnOffObjectId.Value.ToString());
                        }
                        else
                            if (onOffObjectFromSameDCU
                                || !IsOnOffObjectStateOn(newOutput))
                            {
                                _outputController.Off(WatchdogGuid.ToString());
                            }
                    }
                    else
                    {
                        if (!onOffObjectFromSameDCU
                            && IsOnOffObjectStateOn(newOutput))
                        {
                            _outputController.On(newOutput.OnOffObjectId.Value.ToString());
                        }
                    }

                    break;
            }
        }

        private void SwitchFromForcedOn(DB.Output newOutput)
        {
            switch ((DB.OutputControl)newOutput.ControlType)
            {
                case DB.OutputControl.ControlledByDSM:
                case DB.OutputControl.unblocked:

                    _outputController.Off(ForcedOnGuid.ToString());
                    break;

                case DB.OutputControl.ManuallyBlocked:

                    _outputController.Off(ForcedOnGuid.ToString());
                    _outputController.ManualOff();

                    break;

                case DB.OutputControl.ControlledByObject:

                    if (OnOffObjectFromSameDCU(newOutput))
                        _outputController.Off(ForcedOnGuid.ToString());
                    else
                        if (IsOnOffObjectStateOn(newOutput))
                        {
                            _outputController.AddOrReplaceOutputActivator(
                                ForcedOnGuid.ToString(),
                                newOutput.OnOffObjectId.Value.ToString());
                        }
                        else
                            if (!IsOnOffObjectStateOn(newOutput))
                                _outputController.Off(ForcedOnGuid.ToString());
                    break;

                case DB.OutputControl.watchdog:

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                        _outputController.AddOrReplaceOutputActivator(
                            ForcedOnGuid.ToString(),
                            WatchdogGuid.ToString());
                    else
                        _outputController.Off(ForcedOnGuid.ToString());

                    break;
            }
        }

        private void SwitchFromManualyBlockedOutput(DB.Output newOutput)
        {
            _outputController.ReRunOutputActivators();

            switch ((DB.OutputControl)newOutput.ControlType)
            {
                case DB.OutputControl.ForcedOn:

                    _outputController.On(ForcedOnGuid.ToString());

                    break;

                case DB.OutputControl.ControlledByObject:

                    if (!OnOffObjectFromSameDCU(newOutput)
                        && IsOnOffObjectStateOn(newOutput))
                    {
                        _outputController.On(newOutput.OnOffObjectId.Value.ToString());
                    }

                    break;

                case DB.OutputControl.watchdog:

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                        _outputController.On(WatchdogGuid.ToString());

                    break;
            }
        }

        private void SwitchFromUnblockedOutput(DB.Output newOutput)
        {
            switch ((DB.OutputControl)newOutput.ControlType)
            {
                case DB.OutputControl.ManuallyBlocked:

                    _outputController.ManualOff();
                    break;

                case DB.OutputControl.ForcedOn:

                    _outputController.On(ForcedOnGuid.ToString());

                    break;

                case DB.OutputControl.watchdog:

                    if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
                        _outputController.On(WatchdogGuid.ToString());

                    break;

                case DB.OutputControl.ControlledByObject:

                    if (!OnOffObjectFromSameDCU(newOutput)
                        && IsOnOffObjectStateOn(newOutput))
                    {
                        _outputController.On(newOutput.OnOffObjectId.Value.ToString());
                    }

                    break;

                case DB.OutputControl.ControlledByDSM:
                case DB.OutputControl.unblocked:

                    break;
            }
        }

        private void ProcessPossibleControlObjectIdChange(
            [NotNull]
            DB.Output newOutput)
        {
            //there is no change in control object
            var oldOnOffObjectIdNullable = _onOffObjectId;
            var newOnOffObjectIdNullable = newOutput.OnOffObjectId;

            //in this case, the states of old and new OnOffObjects have to be compared for next processing
            if (oldOnOffObjectIdNullable == newOnOffObjectIdNullable)
                return;

            if (!oldOnOffObjectIdNullable.HasValue
                || !newOnOffObjectIdNullable.HasValue)
            {
                return;
            }

            Guid oldOnOffObjectId = oldOnOffObjectIdNullable.Value;
            Guid newOnOffObjectId = newOnOffObjectIdNullable.Value;

            var oldOnOffObjectFromSameDCU = OnOffObjectFromSameDCU();
            var newOnOffObjectFromSameDCU = OnOffObjectFromSameDCU(newOutput);

            //output will not change its state and also its activators will not be changed
            if (oldOnOffObjectFromSameDCU || !IsOnOffObjectStateOn())
            {
                if (!newOnOffObjectFromSameDCU && IsOnOffObjectStateOn(newOutput))
                    _outputController.On(newOnOffObjectId.ToString());

                return;
            }

            if (newOnOffObjectFromSameDCU || !IsOnOffObjectStateOn(newOutput))
                _outputController.Off(oldOnOffObjectId.ToString());
            else
                _outputController.AddOrReplaceOutputActivator(
                    oldOnOffObjectId.ToString(),
                    newOnOffObjectId.ToString());
        }

        private static bool OnOffObjectFromSameDCU(
            Guid idDcu,
            DB.OutputControl controlType,
            ObjectType onOffObjectType,
            Guid? onOffObjectId)
        {
            if (idDcu == Guid.Empty
                || controlType != DB.OutputControl.ControlledByObject
                || onOffObjectType != ObjectType.Input)
            {
                return false;
            }

            return
                onOffObjectId.HasValue
                && Inputs.Singleton.GetInputDCU(onOffObjectId.Value) == idDcu;
        }

        private static bool OnOffObjectFromSameDCU(
            [NotNull]
            DB.Output output)
        {
            return OnOffObjectFromSameDCU(
                output.GuidDCU,
                (DB.OutputControl)output.ControlType,
                output.OnOffObjectObjectType,
                output.OnOffObjectId);
        }

        private bool OnOffObjectFromSameDCU()
        {
            return OnOffObjectFromSameDCU(
                _idDcu,
                _controlType,
                _onOffObjectType,
                _onOffObjectId);
        }

        public void OnDcuDisconnected()
        {
            _hwSetupDone = false;
        }

        public void Off(string activatorKey)
        {
            _outputController.Off(activatorKey);
        }

        public void On(string activatorKey)
        {
            _outputController.On(activatorKey);
        }

        public bool HasOutputActivator(string activatorKey)
        {
            return _outputController.HasOutputActivator(activatorKey);
        }

        public void ReRunOutputActivators()
        {
            _outputController.ReRunOutputActivators();
        }
    }
}