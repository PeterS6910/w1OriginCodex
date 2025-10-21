using System;

using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal partial class AAlarmAreaController<TAlarmAreaController> 
    {
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static State InvertInputState(ref State state)
        {
            switch (state)
            {
                case State.Normal:

                    state = State.Alarm;
                    break;

                case State.Alarm:

                    state = State.Normal;
                    break;

                default:

                    state = State.Normal;
                    break;
            }

            return state;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static State InvertState(ref State state)
        {
            switch (state)
            {
                case State.Off:

                    state = State.On;
                    break;

                case State.On:

                    state = State.Off;
                    break;

                default:

                    state = State.Off;
                    break;
            }

            return state;
        }

        private abstract class AOnOffObjectHandler
        {
            private class ChangeOutputStateRequest
                : WaitableProcessingRequest<TAlarmAreaController>
            {
                private readonly AOnOffObjectHandler _onOffObjectHandler;

                private readonly Guid _idOutput;
                private readonly State _outputState;

                public ChangeOutputStateRequest(
                    AOnOffObjectHandler onOffObjectHandler,
                    Guid idOutput,
                    State outputState)
                {
                    _onOffObjectHandler = onOffObjectHandler;

                    _idOutput = idOutput;
                    _outputState = outputState;
                }

                protected override void ExecuteInternal(TAlarmAreaController param)
                {
                    _onOffObjectHandler.ProcessOutput(
                        _idOutput,
                        _outputState);
                }
            }

            private class ChangeObjectStateRequest : WaitableProcessingRequest<TAlarmAreaController>
            {
                private readonly AOnOffObjectHandler _onOffObjectHandler;

                private readonly Guid _initiatedBy;
                private readonly State _state;

                public ChangeObjectStateRequest(
                    AOnOffObjectHandler onOffObjectHandler,
                    Guid initiatedBy,
                    State state)
                {
                    _onOffObjectHandler = onOffObjectHandler;
                    _initiatedBy = initiatedBy;
                    _state = state;
                }

                protected override void ExecuteInternal(TAlarmAreaController alarmAreaController)
                {
                    _onOffObjectHandler.ProcessStateObject(
                        _initiatedBy,
                        _state);
                }
            }

            private class ChangeInputStateRequest
                : WaitableProcessingRequest<TAlarmAreaController>
            {
                private readonly AOnOffObjectHandler _onOffObjectHandler;

                private readonly Guid _idInput;
                private readonly State _inputState;

                public ChangeInputStateRequest(
                    AOnOffObjectHandler onOffObjectHandler,
                    Guid idInput,
                    State inputState)
                {
                    _onOffObjectHandler = onOffObjectHandler;

                    _idInput = idInput;
                    _inputState = inputState;
                }

                protected override void ExecuteInternal(TAlarmAreaController param)
                {
                    _onOffObjectHandler.ProcessInput(
                        _idInput,
                        _inputState);
                }
            }

            protected readonly AAlarmAreaController<TAlarmAreaController> AlarmAreaController;

            private volatile State _lastStateObj = State.Unknown;
            private State _lastStateInput = State.Unknown;
            private State _lastStateOutput = State.Unknown;

            public Guid? ObjId { get; protected set; }
            protected ObjectType _objType = ObjectType.NotSupport;
            public ObjectType ObjType { get { return _objType; } }
            public bool IsInverted { get; protected set; }

            protected AOnOffObjectHandler(AAlarmAreaController<TAlarmAreaController> alarmAreaController)
            {
                AlarmAreaController = alarmAreaController;
            }

            public void Init(Guid? objId, DB.AlarmArea alarmArea)
            {
                PreInit(objId, alarmArea);

                if (ObjId == null
                    || ObjId.Value == Guid.Empty)
                {
                    return;
                }

                switch (_objType)
                {
                    case ObjectType.DailyPlan:

                        _objectStateChanged = ChangeObjectState;

                        ChangeObjectState(
                            ObjId.Value,
                            DailyPlans.Singleton.GetActualState(ObjId.Value));

                        DailyPlans.Singleton.AddEvent(
                            ObjId.Value,
                            _objectStateChanged);

                        break;

                    case ObjectType.TimeZone:

                        _objectStateChanged = ChangeObjectState;

                        ChangeObjectState(
                            ObjId.Value,
                            TimeZones.Singleton.GetActualState(ObjId.Value));

                        TimeZones.Singleton.AddEvent(
                            ObjId.Value,
                            _objectStateChanged);

                        break;

                    case ObjectType.Input:

                        _inputStateChanged = new InputForAutomaticActChangedListener(this);

                        ChangeInputState(
                            ObjId.Value,
                            Inputs.Singleton.GetInputLogicalState(ObjId.Value));

                        Inputs.Singleton.AddInputChangedListener(
                            ObjId.Value,
                            _inputStateChanged);

                        break;

                    case ObjectType.Output:

                        _outputStateChanged = ChangeOutputState;

                        ChangeOutputState(
                            ObjId.Value,
                            Outputs.Singleton.GetOutputState(ObjId.Value));

                        Outputs.Singleton.AddOutputChanged(
                            ObjId.Value,
                            _outputStateChanged);

                        break;
                }
            }

            protected abstract void PreInit(Guid? objId, DB.AlarmArea alarmArea);

            public void Update(DB.AlarmArea alarmArea)
            {
                Close();
                Init(GetObjId(alarmArea), alarmArea);
            }

            public void Close()
            {
                if (_objectStateChanged != null)
                {
                    if (ObjId != null)
                    {
                        switch (_objType)
                        {
                            case ObjectType.DailyPlan:

                                DailyPlans.Singleton.RemoveEvent(
                                    ObjId.Value,
                                    _objectStateChanged);

                                break;

                            case ObjectType.TimeZone:

                                TimeZones.Singleton.RemoveEvent(
                                    ObjId.Value,
                                    _objectStateChanged);

                                break;
                        }
                    }

                    _objectStateChanged = null;
                }

                if (_inputStateChanged != null)
                {
                    if (ObjId != null
                        && ObjId.Value != Guid.Empty)
                    {
                        Inputs.Singleton.RemoveInputChangedListener(
                            ObjId.Value,
                            _inputStateChanged);
                    }

                    _inputStateChanged = null;
                }

                if (_outputStateChanged != null)
                {
                    if (ObjId != null
                        && ObjId.Value != Guid.Empty
                        && _objType == ObjectType.Output)
                    {
                        Outputs.Singleton.RemoveOutputChanged(
                            ObjId.Value,
                            _outputStateChanged);
                    }

                    _outputStateChanged = null;
                }
            }

            private Action<Guid, State> _objectStateChanged;

            private void ProcessStateObject(Guid initiatedBy, State state)
            {
                if (state != State.On && state != State.Off)
                    return;

                if (IsInverted)
                    InvertState(ref state);

                if (_lastStateObj == state)
                    return;

                _lastStateObj = state;

                ProcessOutputCore(
                    initiatedBy,
                    state);
            }

            private void ChangeObjectState(Guid initiatedBy, State state)
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => string.Format(
                        "void AlarmAreaSettings.ChangeStateObjForAutomaticAct(Guid objectId, State state): [{0}]",
                        Log.GetStringFromParameters(initiatedBy, state)));

                new ChangeObjectStateRequest(
                    this, 
                    initiatedBy, 
                    state).EnqueueAsync(AlarmAreaController._processingQueue);
            }

            private IInputChangedListener _inputStateChanged;

            private void ProcessInput(Guid idInput, State inputState)
            {
                if (idInput != ObjId)
                    return;

                if (inputState != State.Alarm && inputState != State.Normal)
                    return;

                if (IsInverted)
                    InvertInputState(ref inputState);

                if (_lastStateInput == inputState)
                    return;

                _lastStateInput = inputState;

                ProcessInputCore(
                    idInput,
                    inputState);
            }

            private void ChangeInputState(
                Guid idInput,
                State inputState)
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => string.Format(
                        "void AlarmAreaSettings.ChangeStateInputForAutomaticAct(Guid guidInput, State inputState): [{0}]",
                        Log.GetStringFromParameters(idInput, inputState)));

                new ChangeInputStateRequest(
                    this,
                    idInput,
                    inputState).EnqueueAsync(AlarmAreaController._processingQueue);
            }

            private Action<Guid, State> _outputStateChanged;

            private void ProcessOutput(Guid idOutput, State outputState)
            {
                if (idOutput != ObjId)
                    return;

                if (outputState != State.On && outputState != State.Off)
                    return;

                if (IsInverted)
                    InvertState(ref outputState);

                if (_lastStateOutput == outputState)
                    return;

                _lastStateOutput = outputState;

                ProcessOutputCore(
                    idOutput,
                    outputState);
            }

            private void ChangeOutputState(
                Guid idOutput, 
                State outputState)
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL, 
                    () => string.Format(
                        "void AlarmAreaSettings.ChangeStateOutputForAutomaticAct(Guid idOutput, State outputState): [{0}]", 
                        Log.GetStringFromParameters(idOutput, outputState)));

                new ChangeOutputStateRequest(
                    this,
                    idOutput,
                    outputState).EnqueueAsync(AlarmAreaController._processingQueue);
            }

            protected virtual void ProcessStateObjectCore(Guid objectId, State state)
            {
            }

            protected virtual void ProcessInputCore(Guid objectId, State state)
            {
            }

            protected virtual void ProcessOutputCore(Guid objectId, State state)
            {
            }

            public bool IsActivated()
            {
                if (!ObjId.HasValue)
                    return false;

                switch (_objType)
                {
                    case ObjectType.Input:

                        return _lastStateInput == State.Alarm;

                    case ObjectType.Output:

                        return _lastStateOutput == State.On;

                    default:

                        return _lastStateObj == State.On;
                }
            }

            protected abstract Guid? GetObjId(DB.AlarmArea alarmArea);

            private class InputForAutomaticActChangedListener : IInputChangedListener
            {
                private readonly AOnOffObjectHandler _onOffObjHandler;
                private readonly Guid _guidAlarmArea;

                public InputForAutomaticActChangedListener(AOnOffObjectHandler onOffObjHandler)
                {
                    _onOffObjHandler = onOffObjHandler;
                    _guidAlarmArea = onOffObjHandler.AlarmAreaController.Id;
                }

                public override int GetHashCode()
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    return _guidAlarmArea.GetHashCode();
                }

                public bool Equals(IInputChangedListener other)
                {
                    var otherInputChangedListener =
                        other as InputForAutomaticActChangedListener;

                    return otherInputChangedListener != null
                        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                           && _guidAlarmArea.Equals(otherInputChangedListener._guidAlarmArea);
                }

                public void OnInputChanged(
                    Guid guidInput,
                    State state)
                {
                    _onOffObjHandler.ChangeInputState(
                        guidInput,
                        state);
                }
            }
        }

        private class OnOffObjectForForcedTimeBuyingHandler : AOnOffObjectHandler
        {
            public OnOffObjectForForcedTimeBuyingHandler(AAlarmAreaController<TAlarmAreaController> alarmAreaController)
                : base(alarmAreaController)
            {
            }

            protected override void PreInit(Guid? objId, DB.AlarmArea alarmArea)
            {
                ObjId = objId;
                _objType = alarmArea.ObjForForcedTimeBuyingObjectType;
                IsInverted = alarmArea.IsInvertedObjForForcedTimeBuying;
            }

            protected override Guid? GetObjId(DB.AlarmArea alarmArea)
            {
                return alarmArea.ObjForForcedTimeBuyingId;
            }

            protected override void ProcessStateObjectCore(Guid objectId, State state)
            {
                switch (state)
                {
                    case State.Off:
                        AlarmAreaController.OnDeactivatedObjectForTimeBuing();
                        break;

                    case State.On:
                        AlarmAreaController.OnActivatedObjectForTimeBuing();
                        break;
                }
            }

            protected override void ProcessInputCore(Guid objectId, State state)
            {
                switch (state)
                {
                    case State.Normal:
                        AlarmAreaController.OnDeactivatedObjectForTimeBuing();
                        break;

                    case State.Alarm:
                        AlarmAreaController.OnActivatedObjectForTimeBuing();
                        break;

                }
            }

            protected override void ProcessOutputCore(Guid objectId, State state)
            {
                switch (state)
                {
                    case State.Off:
                        AlarmAreaController.OnDeactivatedObjectForTimeBuing();
                        break;

                    case State.On:
                        AlarmAreaController.OnActivatedObjectForTimeBuing();
                        break;
                }
            }
        }

        private class OnOffObjectForAutomaticActivationHandler : AOnOffObjectHandler
        {
            public OnOffObjectForAutomaticActivationHandler(AAlarmAreaController<TAlarmAreaController> alarmAreaController)
                : base(alarmAreaController)
            {
            }

            protected override void PreInit(Guid? objId, DB.AlarmArea alarmArea)
            {
                ObjId = objId;
                _objType = alarmArea.ObjForAutomaticActObjectType;
                IsInverted = alarmArea.IsInvertedObjForAutomaticAct;
            }

            protected override void ProcessStateObjectCore(Guid objectId, State state)
            {
                switch (state)
                {
                    case State.Off:
                        AlarmAreaController.OnDeactivatedObjectForAa();
                        break;

                    case State.On:
                        AlarmAreaController.OnActivatedObjectForAa();
                        break;
                }
            }

            protected override void ProcessInputCore(Guid objectId, State state)
            {
                switch (state)
                {
                    case State.Normal:
                        AlarmAreaController.OnDeactivatedObjectForAa();
                        break;

                    case State.Alarm:
                        AlarmAreaController.OnActivatedObjectForAa();
                        break;
                }
            }

            protected override void ProcessOutputCore(Guid objectId, State state)
            {
                switch (state)
                {
                    case State.Off:
                        AlarmAreaController.OnDeactivatedObjectForAa();
                        break;

                    case State.On:
                        AlarmAreaController.OnActivatedObjectForAa();
                        break;
                }
            }

            protected override Guid? GetObjId(DB.AlarmArea alarmArea)
            {
                return alarmArea.ObjForAutomaticActId;
            }
        }
    }
}
