using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.BoolExpressions.CrossPlatform;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal class InputStateAndSettings : AStateAndSettingsObject<DB.Input>
    {
        private class InputChangeListenerForBoundOutputs : IInputChangedListener
        {
            private readonly InputStateAndSettings _inputStateAndSettings;

            private readonly ICollection<Guid> _idOutputs = new HashSet<Guid>();

            public InputChangeListenerForBoundOutputs(InputStateAndSettings inputStateAndSettings)
            {
                _inputStateAndSettings = inputStateAndSettings;
            }

            public override int GetHashCode()
            {
                return _inputStateAndSettings.Id.GetHashCode();
            }

            public bool Equals(IInputChangedListener other)
            {
                return other is InputChangeListenerForBoundOutputs;
            }

            public void OnInputChanged(
                Guid guidInput,
                State state)
            {
                foreach (var idOutput in _idOutputs)
                    Outputs.Singleton.OnBoundInputStateChanged(
                        idOutput,
                        _inputStateAndSettings.Id,
                        _inputStateAndSettings.IdDcu,
                        state);
            }

            public void AddIdOutput(Guid idOutput)
            {
                _idOutputs.Add(idOutput);
            }

            public void RemoveIdOutput(Guid idOutput, out bool isEmpty)
            {
                _idOutputs.Remove(idOutput);

                isEmpty = _idOutputs.Count == 0;
            }

            public void OnInputBlocked(bool requestedOutputState)
            {
                foreach (var idOutput in _idOutputs)
                    Outputs.Singleton.OnBoundInputBlocked(
                        idOutput,
                        _inputStateAndSettings,
                        requestedOutputState);
            }

            public void OnInputUnblocked()
            {
                foreach (var idOutput in _idOutputs)
                    Outputs.Singleton.OnBoundInputUnblocked(
                        idOutput,
                        _inputStateAndSettings);
            }
        }

        private class InputBlockedByInputChangedListener : IInputChangedListener
        {
            private readonly InputStateAndSettings _inputStateAndSettings;

            public InputBlockedByInputChangedListener(InputStateAndSettings inputStateAndSettings)
            {
                _inputStateAndSettings = inputStateAndSettings;
            }

            public override int GetHashCode()
            {
                return _inputStateAndSettings.Id.GetHashCode();
            }

            public bool Equals(IInputChangedListener other)
            {
                var otherInputChangedListener = other as InputBlockedByInputChangedListener;

                return
                    otherInputChangedListener != null
                    && _inputStateAndSettings.Id == otherInputChangedListener._inputStateAndSettings.Id;
            }

            public void OnInputChanged(
                Guid guidInput,
                State state)
            {
                _inputStateAndSettings.InputStateChanged(
                    guidInput,
                    state);
            }
        }

        private State _baseInputState;

        private InputConfiguration _inputConfiguration;

        public Guid IdDcu { get; private set; }

        public byte InputNumber { get; private set; }

        public bool AlarmOn { get; private set; }
        public bool AlarmTamperOn { get; private set; }

        private State _blockedInputState = State.Unknown;

        private BlockingType _blockingType;

        private IdAndObjectType _onOffObject;

        private InputBlockedByInputChangedListener _inputBlockedByInputStateChanged;
        private InputChangeListenerForBoundOutputs _inputChangeListenerForBoundOutputs;

        private readonly object _lockBlockUnblock = new object();

        public BoolVariable InputTamper { get; private set; }

        private InputType _inputType;

        public State InputLogicalState
        {
            get
            {
                return
                    _blockedInputState != State.Unknown
                        ? _blockedInputState
                        : _baseInputState;
            }
        }

        public void SetBaseInputState(State value)
        {
            if (_baseInputState == value)
                return;

            _baseInputState = value;

            OnInputLogicalStateChanged();

            // the rest of the method deals with situation 
            // when input had been blocked with BlockTemporarilyUntilType set to SensorStateNormal
            // and is about to be unblocked
        }

        private State _previousInputLogicalState = State.Unknown;

        private void OnInputLogicalStateChanged()
        {
            if (InputLogicalState == _previousInputLogicalState)
                return;

            _previousInputLogicalState = InputLogicalState;

            UpdateTamperState();

            Inputs.Singleton.EnqueueInputLogicalStateChanged(
                new Inputs.InputChangedPqRequest(
                    InputChangedEventHandler,
                    InputLogicalState));

            EnqueueSendInputLogicalStateToServer(UpdateAlarms());
        }

        public InputChangedEventHandlers InputChangedEventHandler { get; private set; }

        private void UpdateTamperState()
        {
            InputTamper.SetState(InputLogicalState == State.Short
                                 || InputLogicalState == State.Break);
        }

        public bool IsBlocked
        {
            get { return _blockedInputState != State.Unknown; }
        }

        public bool IsBlockedAndCurrentStateIsNotNormal
        {
            get { return IsBlocked && _baseInputState != State.Normal; }
        }

        public InputStateAndSettings(
            DB.Input input,
            State initialBaseInputState)
            : base(input.IdInput)
        {
            IdDcu = input.GuidDCU;
            InputNumber = input.InputNumber;

            _baseInputState = initialBaseInputState;

            InputTamper = new BoolVariable(false);

            InputChangedEventHandler = new InputChangedEventHandlers(this);
        }

        protected override void ConfigureInternal(
            [NotNull] DB.Input input)
        {
            if (_baseInputState == State.UsedByAnotherAplication
                || _baseInputState == State.OutOfRange)
            {
                return;
            }

            _inputType = (InputType)input.InputType;

            AlarmOn = input.AlarmOn;
            AlarmTamperOn = input.AlarmTamper;

            _blockingType = (BlockingType)input.BlockingType;

            Guid? onOffObjectGuid = input.OnOffObjectId;

            ObjectType onOffObjectType = input.OnOffObjectObjectType;

            _onOffObject =
                onOffObjectGuid.HasValue
                    ? new IdAndObjectType(
                        onOffObjectGuid,
                        onOffObjectType)
                    : null;

            switch (_blockingType)
            {
                case BlockingType.ForcefullyBlocked:

                    Block(false);

                    break;

                case BlockingType.ForcefullySet:

                    Block(true);

                    break;

                case BlockingType.BlockedByObject:

                    if (onOffObjectGuid == null)
                        return;

                    switch (onOffObjectType)
                    {
                        case ObjectType.DailyPlan:

                            DailyPlans.Singleton.AddEvent(
                                onOffObjectGuid.Value,
                                OnOffObjectStateChangedDailyPlan);

                            break;

                        case ObjectType.TimeZone:

                            TimeZones.Singleton.AddEvent(
                                onOffObjectGuid.Value,
                                OnOffObjectStateChangedTimeZone);

                            break;

                        case ObjectType.Input:

                            if (Database.ConfigObjectsEngine.Exists(ObjectType.Input, onOffObjectGuid.Value))
                            {
                                _inputBlockedByInputStateChanged =
                                    new InputBlockedByInputChangedListener(this);

                                Inputs.Singleton.AddInputChangedListener(
                                    onOffObjectGuid.Value,
                                    _inputBlockedByInputStateChanged);

                                InputStateChanged(
                                    onOffObjectGuid.Value,
                                    Inputs.Singleton.GetInputLogicalState(onOffObjectGuid.Value));
                            }
                            else
                            {
                                InterCcuCommunication.Singleton.SetObjectActualState(
                                    ObjectType.Input,
                                    onOffObjectGuid.Value);
                            }

                            break;

                        case ObjectType.Output:

                            Outputs.Singleton.AddOutputChanged(
                                onOffObjectGuid.Value,
                                OutputStateChanged);

                            OutputStateChanged(
                                onOffObjectGuid.Value,
                                Outputs.Singleton.GetOutputState(onOffObjectGuid.Value));

                            break;
                    }

                    break;

                case BlockingType.NotBlocked:

                    Unblock();

                    break;
            }
        }

        protected override void ApplyHwSetup(DB.Input input)
        {
            if (_baseInputState == State.UsedByAnotherAplication
                || _baseInputState == State.OutOfRange)
            {
                return;
            }

            if (IdDcu != Guid.Empty)
                DCUs.Singleton.ApplyInputHwSetup(input);
            else
                ApplyCcuInputHwSetup(input);
        }

        private void ApplyCcuInputHwSetup(DB.Input input)
        {
            var requiredInputType = (InputTypeG7)input.InputType;

            InputStateVariantG7[] inputStates;

            uint[] inputLevels;

            if ((InputType)input.InputType == InputType.BSI)
            {
                inputLevels = Ccus.Singleton.GetBsiLevels();

                inputStates =
                    new[]
                    {
                        InputStateVariantG7.Default(InputState.Short),
                        InputStateVariantG7.Default(InputState.Normal),
                        InputStateVariantG7.Default(InputState.Alarm),
                        InputStateVariantG7.Default(InputState.Break)
                    };

                if (input.Inverted)
                    Array.Reverse(
                        inputStates,
                        1,
                        2);
            }
            else
            {
                inputLevels = null;

                inputStates =
                    new[]
                    {
                        InputStateVariantG7.Default(InputState.Alarm),
                        InputStateVariantG7.Default(InputState.Normal)
                    };

                if (input.Inverted)
                    Array.Reverse(inputStates);
            }

            _inputConfiguration = InputConfiguration.Get(
                inputCon =>
                {
                    inputCon.InputType = requiredInputType;
                    inputCon.FilterTime = IOControl.MINIMAL_FILTERTIME;
                    inputCon.DelayToOn = input.DelayToOn;
                    inputCon.DelayToOff = input.DelayToOff;
                    inputCon.TamperDelayToOn = input.TamperDelayToOn;
                    inputCon.SetLevelValues(inputLevels);
                    inputCon.SetInputStatesForLevels(inputStates);
                });

            if (_inputConfiguration == null)
                return;

            IOControl.ConfigureInput(
                InputNumber,
                _inputConfiguration);
        }

        private void OnOffObjectStateChangedDailyPlan(Guid initiatedBy, State state)
        {
            switch (state)
            {
                case State.On:

                    Block(false);

                    break;

                case State.Off:

                    Unblock();

                    break;
            }
        }

        private void OnOffObjectStateChangedTimeZone(Guid initiatedBy, State state)
        {
            switch (state)
            {
                case State.On:

                    Block(false);

                    break;

                case State.Off:

                    Unblock();

                    break;
            }
        }

        private void InputStateChanged(Guid inputGuid, State inputState)
        {
            switch (inputState)
            {
                case State.Alarm:

                    Block(false);

                    break;

                case State.Normal:

                    Unblock();

                    break;
            }
        }

        private void OutputStateChanged(Guid outputGuid, State outputState)
        {
            switch (outputState)
            {
                case State.On:

                    Block(false);

                    break;

                case State.Off:

                    Unblock();

                    break;
            }
        }

        public void IccuInputOrOutputStateChanged(bool state)
        {
            switch (state)
            {
                case true:

                    Block(false);

                    break;

                case false:

                    Unblock();

                    break;
            }
        }

        private void Block(bool forcefullySet)
        {
            lock (_lockBlockUnblock)
            {
                if (forcefullySet && _blockedInputState == State.Alarm
                    || !forcefullySet && _blockedInputState == State.Normal)
                {
                    return;
                }

                if (forcefullySet)
                {
                    _blockedInputState = State.Alarm;

                    if (_inputChangeListenerForBoundOutputs != null)
                        _inputChangeListenerForBoundOutputs.OnInputBlocked(true);
                }
                else
                {
                    _blockedInputState = State.Normal;

                    if (_inputChangeListenerForBoundOutputs != null)
                        _inputChangeListenerForBoundOutputs.OnInputBlocked(false);
                }

                OnInputLogicalStateChanged();
            }
        }

        private bool UpdateAlarms()
        {
            var alarmWasGenerated = UpdateAlarm(
                Id,
                InputLogicalState,
                AlarmOn);

            UpdateTamperAlarm();

            return alarmWasGenerated;
        }

        private bool UpdateAlarm(
            Guid idInput,
            State inputState,
            bool alarmOn)
        {
            var generateAlarm = alarmOn;

            if (inputState == State.Alarm)
            {
                if (!generateAlarm)
                {
                    AlarmsManager.Singleton.StopAlarm(
                        InputAlarm.CreateAlarmKey(idInput));

                    return false;
                }

                AlarmsManager.Singleton.AddAlarm(
                    new InputAlarm(idInput));

                return true;
            }

            AlarmsManager.Singleton.StopAlarm(
                InputAlarm.CreateAlarmKey(idInput));

            return generateAlarm;
        }

        private void UpdateTamperAlarm()
        {
            InputTamperAlarm.Update(
                Id,
                InputLogicalState,
                AlarmTamperOn);
        }

        private void Unblock()
        {
            lock (_lockBlockUnblock)
            {
                if (!IsBlocked)
                    return;

                _blockedInputState = State.Unknown;

                OnInputLogicalStateChanged();

                if (_inputChangeListenerForBoundOutputs != null)
                    _inputChangeListenerForBoundOutputs.OnInputUnblocked();
            }
        }

        private void EnqueueSendInputLogicalStateToServer(bool alarmWasGenerated)
        {
            Inputs.Singleton.EnqueueSendInputStateToServer(
                new Inputs.SendInputStatePqRequest(
                    this,
                    InputLogicalState,
                    true,
                    alarmWasGenerated));
        }

        public void RemoveStateChangedEventsOnBlockingObject()
        {
            if (_blockingType != BlockingType.BlockedByObject)
                return;

            if (_onOffObject == null)
                return;

            switch (_onOffObject.ObjectType)
            {
                case ObjectType.DailyPlan:

                    DailyPlans.Singleton.RemoveEvent(
                        (Guid)_onOffObject.Id,
                        OnOffObjectStateChangedDailyPlan);

                    break;

                case ObjectType.TimeZone:

                    TimeZones.Singleton.RemoveEvent(
                        (Guid)_onOffObject.Id,
                        OnOffObjectStateChangedTimeZone);

                    break;

                case ObjectType.Input:

                    Inputs.Singleton.RemoveInputChangedListener(
                        (Guid)_onOffObject.Id,
                        _inputBlockedByInputStateChanged);

                    break;

                case ObjectType.Output:

                    Outputs.Singleton.RemoveOutputChanged(
                        (Guid)_onOffObject.Id,
                        OutputStateChanged);

                    break;
            }
        }

        /// <summary>
        /// Resets input state to unknown without sending information to server
        /// </summary>
        public void ResetInputState()
        {
            _baseInputState = State.Unknown;
        }

        public void SendEventInputStateChanged(
            State inputState,
            bool inputChanged,
            bool alarmWasGenerated)
        {
            if (inputChanged)
            {
                List<Guid> activatedAAsForSensorList;

                lock (_idsOfAlarmAreasLock)
                    activatedAAsForSensorList =
                        _idsOfAlarmAreas != null
                            ? new List<Guid>(
                                _idsOfAlarmAreas.Where(
                                    idAlarmArea => AlarmAreas.Singleton.AlarmAreaIsSet(idAlarmArea)))
                            : null;

                Events.ProcessEvent(
                    new EventInputStateChanged(
                        Id,
                        inputState,
                        true,
                        alarmWasGenerated,
                        activatedAAsForSensorList));
            }
            else
                Events.ProcessEvent(
                    new EventInputStateInfo(
                        Id,
                        inputState));
        }

        private readonly object _idsOfAlarmAreasLock = new object();

        private ICollection<Guid> _idsOfAlarmAreas;

        public static InputStateAndSettings CreateNewInputStateAndSettings(DB.Input input)
        {
            if (input.GuidDCU.Equals(Guid.Empty))
            {
                if (SharedResources.Singleton.GetInputAccess(input.InputNumber) != SRInputAccess.FullAccess)
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "Input used by another aplication: " + input.InputNumber);

                    return
                        new InputStateAndSettings(
                            input,
                            State.UsedByAnotherAplication);
                }

                if (input.InputNumber >= Inputs.GetInputCount())
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "Input out of range: " + input.InputNumber);

                    return
                        new InputStateAndSettings(
                            input,
                            State.OutOfRange);
                }
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Set input: " + input.InputNumber);

            return
                new InputStateAndSettings(
                    input,
                    State.Unknown);
        }

        public void AddInputChangedListener(
            IInputChangedListener inputChangedListener,
            Guid idAlarmArea)
        {
            InputChangedEventHandler.AddInputChangedListener(inputChangedListener);

            if (idAlarmArea.Equals(Guid.Empty))
                return;

            lock (_idsOfAlarmAreasLock)
            {
                if (_idsOfAlarmAreas == null)
                    _idsOfAlarmAreas = new HashSet<Guid>();

                _idsOfAlarmAreas.Add(idAlarmArea);
            }
        }

        public void RemoveInputChangedListener(
            IInputChangedListener inputChangedListener,
            Guid idAlarmArea)
        {
            InputChangedEventHandler
                .RemoveInputChangedListener(inputChangedListener);

            if (idAlarmArea == Guid.Empty)
                return;

            lock (_idsOfAlarmAreasLock)
            {
                if (_idsOfAlarmAreas == null)
                    return;

                _idsOfAlarmAreas.Remove(idAlarmArea);

                if (_idsOfAlarmAreas.Count == 0)
                    _idsOfAlarmAreas = null;
            }
        }

        public void BindOutputToInput(
            OutputStateAndSettings outputStateAndSettings)
        {
            if (outputStateAndSettings.IdDcu == Guid.Empty
                || outputStateAndSettings.IdDcu != IdDcu)
            {
                outputStateAndSettings.OnBoundInputStateChanged(
                    Id,
                    IdDcu,
                    InputLogicalState);
            }

            if (_inputChangeListenerForBoundOutputs == null)
            {
                _inputChangeListenerForBoundOutputs = new InputChangeListenerForBoundOutputs(this);

                AddInputChangedListener(
                    _inputChangeListenerForBoundOutputs,
                    Guid.Empty);
            }

            _inputChangeListenerForBoundOutputs.AddIdOutput(outputStateAndSettings.Id);
        }

        public void UnbindOutputFromInput(
            OutputStateAndSettings outputStateAndSettings)
        {
            if (outputStateAndSettings.IdDcu != Guid.Empty
                && outputStateAndSettings.IdDcu == IdDcu)
            {
                if (!Inputs.Singleton.IsInputBlocked(Id))
                {
                    DCUs.Singleton.UnbindOutputFromInput(
                        outputStateAndSettings.IdDcu,
                        outputStateAndSettings.OutputNumber,
                        InputNumber);
                }
            }

            if (_inputChangeListenerForBoundOutputs == null)
                return;

            bool isEmpty;

            _inputChangeListenerForBoundOutputs.RemoveIdOutput(
                outputStateAndSettings.Id,
                out isEmpty);

            if (!isEmpty)
                return;

            RemoveInputChangedListener(
                _inputChangeListenerForBoundOutputs,
                Guid.Empty);

            _inputChangeListenerForBoundOutputs = null;
        }

        protected override void UnconfigureInternal(DB.Input newDbObject)
        {
            RemoveStateChangedEventsOnBlockingObject();

            if (_inputConfiguration != null)
            {
                _inputConfiguration.Return();
                _inputConfiguration = null;
            }
        }

        public void SetBsiLevels(uint[] bsiLevels)
        {
            // Only inputs that are BSI and are not scheduled for (re)configuration

            if (ConfigurationState != ConfigurationState.ConfigurationDone
                || _inputType != InputType.BSI
                || _inputConfiguration == null)
            {
                return;
            }

            _inputConfiguration.SetLevelValues(bsiLevels);

            IOControl.ConfigureInput(
                InputNumber,
                _inputConfiguration);
        }
    }
}
