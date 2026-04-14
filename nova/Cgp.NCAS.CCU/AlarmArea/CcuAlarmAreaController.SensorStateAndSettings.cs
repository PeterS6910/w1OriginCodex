using System;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    partial class CcuAlarmAreaController
    {
        private class SensorStateAndSettings :
            ISensorStateAndSettings
        {

            private class SetSensorBlockingTypeRequest
                : WaitableProcessingRequest<CcuAlarmAreaController>
            {
                private readonly SensorStateAndSettings _sensorStateAndSettings;
                private readonly SensorBlockingType _sensorBlockingType;

                private readonly Guid _idCardReader;
                private readonly Guid _idCard;
                private readonly Guid _idPerson;

                public SetSensorBlockingTypeRequest(
                    SensorStateAndSettings sensorStateAndSettings,
                    SensorBlockingType sensorBlockingType,
                    Guid idCardReader,
                    AccessDataBase accessData)
                {
                    _sensorStateAndSettings = sensorStateAndSettings;
                    _sensorBlockingType = sensorBlockingType;
                    _idCardReader = idCardReader;
                    _idCard = accessData.IdCard;
                    _idPerson = accessData.IdPerson;
                }

                protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
                {
                    _sensorStateAndSettings.SetSensorBlockingTypeSync(
                         _sensorBlockingType,
                         _idCardReader,
                         _idCard,
                         _idPerson);
                }
            }

            private abstract class ASensorListener : BoolExpressionStateChangedListener
            {
                protected readonly SensorStateAndSettings SensorsSettings;
                protected readonly CcuAlarmAreaController Controller;

                protected ASensorListener(SensorStateAndSettings sensorsSettings)
                {
                    SensorsSettings = sensorsSettings;
                    Controller = SensorsSettings._controller;
                }
            }

            private class SensorBlockedWhenActivatedListener : ASensorListener
            {
                public SensorBlockedWhenActivatedListener(SensorStateAndSettings sensorSettings)
                    : base(sensorSettings)
                {
                }

                public override void OnStateChanged(bool? oldState, bool? newState)
                {
                    var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                        AlarmType.AlarmArea_Alarm,
                        new IdAndObjectType(
                            Controller.Id,
                            ObjectType.AlarmArea));

                    if (alarmArcs == null)
                        return;

                    var input = Database.ConfigObjectsEngine.GetFromDatabase(
                        ObjectType.Input,
                        SensorsSettings.IdInput) as DB.Input;

                    if (input == null)
                        return;

                    CatAlarmsManager.Singleton.SendCatAlarms(
                        new[]
                        {
                            CatAlarmsManager.CreateSensorAlarm(
                                null,
                                newState == true,
                                input,
                                Controller.AlarmAreaStateAndSettings.DbObject,
                                SensorsSettings.SensorPurpose,
                                true,
                                alarmArcs)
                        });
                }
            }

            private class SensorNotAcknowledgedListener : ASensorListener
            {
                public SensorNotAcknowledgedListener(SensorStateAndSettings sensorsSettings)
                    : base(sensorsSettings)
                {
                }

                public override void OnStateChanged(bool? oldState, bool? newState)
                {
                    Controller.AlarmAreaStateAndSettings.SensorsEventHandlerGroup.ForEach(
                        eventHandler => eventHandler.OnSensorNotAcknowledgedChanged(
                            SensorsSettings,
                            newState ?? false));
                }
            }

            private class SensorInAlarmListener : ASensorListener
            {
                public SensorInAlarmListener(SensorStateAndSettings sensorsSettings)
                    : base(sensorsSettings)
                {
                }

                public override void OnStateChanged(bool? otherState, bool? newState)
                {
                    Controller.AlarmAreaStateAndSettings.SensorsEventHandlerGroup.ForEach(
                        eventHandler => eventHandler.OnSensorInAlarmStateChanged(
                            SensorsSettings,
                            newState ?? false));
                }
            }

            private class SensorInTamperListener : ASensorListener
            {
                public SensorInTamperListener(SensorStateAndSettings sensorsSettings)
                    : base(sensorsSettings)
                {
                }

                public override void OnStateChanged(bool? otherState, bool? newState)
                {
                    Controller.AlarmAreaStateAndSettings.SensorsEventHandlerGroup.ForEach(
                        eventHandler => eventHandler.OnSensorInTamperStateChanged(
                            SensorsSettings,
                            newState ?? false));
                }
            }

            private class SensorInAlarmOrTamperListener : ASensorListener
            {
                public SensorInAlarmOrTamperListener(SensorStateAndSettings sensorsSettings)
                    : base(sensorsSettings)
                {
                }

                public override void OnStateChanged(bool? oldState, bool? newState)
                {
                    Controller.OnSensorAlarmOrTamperStateChanged(newState == true);
                }
            }

            public Guid IdInput
            {
                get;
                private set;
            }

            private State _currentInputStateCache;
            private readonly CcuAlarmAreaController _controller;

            private bool _criticalInput;
            private DB.BlockTemporarilyUntilType _blockTemporarilyUntilType;

            private readonly BoolVariable _sensorInAlarm;
            private readonly BoolVariable _sensorInTamper;
            private readonly OrOperation _sensorInAlarmOrTamper;
            private readonly BoolVariable _sensorAlarmNotAcknowledged;
            private readonly BoolVariable _sensorTamperAlarmNotAcknowledged;
            private readonly BoolVariable _sensorTemporarilyBlocked;
            private readonly BoolVariable _sensorPermanentlyBlocked;
            private readonly OrOperation _sensorBlocked;
            private readonly AndOperation _sensorBlockedWhenActivated;

            private SensorBlockingType _sensorBlockingType;
            private bool _createDelayedSensorAlarm;

            private readonly AlarmKey _sensorAlarmKey;
            private readonly AlarmKey _sensorTamperAlarmKey;

            private State _sensorStateForCardReader = State.Normal;

            public SensorPurpose SensorPurpose { get; private set; }

            public Guid IdAlarmArea
            {
                get { return _controller.Id; }
            }

            public SensorBlockingType SensorBlockingType
            {
                get { return _sensorBlockingType; }
            }

            private void SetSensorBlockingTypeSync(
                SensorBlockingType value,
                Guid idCardReader,
                Guid idCard,
                Guid idPerson)
            {
                var oldValue = _sensorBlockingType;

                _sensorBlockingType = value;

                OnSensorBlockingTypeChanged();

                if (_sensorBlockingType == oldValue)
                    return;

                Events.ProcessEvent(
                    new EventSensorBlockingTypeChanged(
                        IdInput,
                        IdAlarmArea,
                        idCardReader,
                        idCard,
                        idPerson,
                        _sensorBlockingType));

                _controller.AlarmAreaStateAndSettings.SensorsEventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnSensorBlockingTypeChanged(
                        this,
                        _sensorBlockingType));

                AlarmAreas.Singleton.SaveSensorBlocking(
                    IdAlarmArea,
                    IdInput,
                    _sensorBlockingType);
            }

            private void OnSensorBlockingTypeChanged()
            {
                switch (_sensorBlockingType)
                {
                    case SensorBlockingType.Unblocked:

                        _sensorPermanentlyBlocked.SetState(false);
                        _sensorTemporarilyBlocked.SetState(false);

                        Events.ProcessEvent(
                            new EventSensorStateChanged(
                                IdInput,
                                IdAlarmArea,
                                _currentInputStateCache));

                        if (_currentInputStateCache == State.Break
                            || _currentInputStateCache == State.Short)
                        {
                            _sensorInTamper.SetState(false);

                            AlarmsManager.Singleton.AddAlarm(
                                new SensorTamperAlarm(
                                    IdInput,
                                    _controller.Id));

                            _sensorTamperAlarmNotAcknowledged.SetState(true);

                            CreateEventSensorStateChangedForCardReader(_currentInputStateCache);

                            return;
                        }

                        if (_currentInputStateCache != State.Alarm)
                            return;

                        _sensorInAlarm.SetState(true);

                        if (_controller.AlarmState == State.Alarm)
                        {
                            AlarmsManager.Singleton.AddAlarm(
                                new SensorAlarm(
                                    IdInput,
                                    _controller.Id));

                            _sensorAlarmNotAcknowledged.SetState(true);

                            CreateEventSensorStateChangedForCardReader(State.Alarm);

                        }
                        else
                            if (_controller.ActivationState == State.TemporaryUnsetEntry)
                                _createDelayedSensorAlarm = true;

                        return;

                    case SensorBlockingType.BlockPermanently:

                        _sensorPermanentlyBlocked.SetState(true);
                        _sensorTemporarilyBlocked.SetState(false);

                        break;

                    case SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset:
                    case SensorBlockingType.BlockTemporarilyUntilSensorStateNormal:

                        _sensorTemporarilyBlocked.SetState(true);
                        _sensorPermanentlyBlocked.SetState(false);

                        break;
                }

                AlarmsManager.Singleton
                    .StopAlarm(_sensorAlarmKey);

                AlarmsManager.Singleton
                    .StopAlarm(_sensorTamperAlarmKey);

                _sensorInAlarm.SetState(false);
                _sensorInTamper.SetState(false);

                Events.ProcessEvent(
                    new EventSensorStateChanged(
                        IdInput,
                        IdAlarmArea,
                        State.Normal));

                CreateEventSensorStateChangedForCardReader(State.Normal);
            }

            public bool IsInAlarm
            {
                get { return _sensorInAlarm.State == true; }
            }

            public bool IsNotAcknowledged
            {
                get
                {
                    return _sensorAlarmNotAcknowledged.State == true
                           || _sensorTamperAlarmNotAcknowledged.State == true;
                }
            }

            public bool IsInTamper
            {
                get
                {
                    return _sensorInTamper.State == true;
                }
            }

            public int SensorId
            {
                get;
                private set;
            }

            public string NickName
            {
                get
                {
                    var input =
                        Database.ConfigObjectsEngine.GetFromDatabase(
                            ObjectType.Input,
                            IdInput) as DB.Input;

                    return
                        input != null
                            ? input.NickName
                            : null;
                }
            }

            public State SensorState
            {
                get
                {
                    switch (_currentInputStateCache)
                    {
                        case State.Alarm:
                            return _sensorBlockingType != SensorBlockingType.Unblocked
                                ? State.Normal
                                : State.Alarm;

                        case State.Short:
                        case State.Break:
                            return _sensorBlockingType != SensorBlockingType.Unblocked
                                ? State.Normal
                                : _currentInputStateCache;

                        default:
                            return State.Normal;
                    }
                }
            }

            public DB.BlockTemporarilyUntilType DefaultBlockTemporarilyUntilType
            {
                get
                {
                    return
                        _blockTemporarilyUntilType;
                }
            }

            public SensorStateAndSettings(
                CcuAlarmAreaController controller,
                DB.AAInput aaInput)
            {
                _controller = controller;
                IdInput = aaInput.GuidInput;
                SensorPurpose = GetSensorPurpose(aaInput.Purpose);

                _criticalInput = !aaInput.NoCriticalInput;

                _blockTemporarilyUntilType =
                    (DB.BlockTemporarilyUntilType)
                        aaInput.BlockTemporarilyUntil;

                SensorId = aaInput.Id;

                _sensorBlockingType = AlarmAreas.Singleton.GetSensorBlockingType(
                    IdAlarmArea,
                    IdInput);

                Inputs.Singleton.AddInputChangedListener(
                    IdInput,
                    _controller._inputChangedListener,
                    _controller.Id);

                _currentInputStateCache = Inputs.Singleton.GetInputLogicalState(IdInput);

                var sensorState = SensorState;

                _sensorInAlarm = new BoolVariable(sensorState == State.Alarm);
                _sensorInAlarm.AddListenerAndGetState(new SensorInAlarmListener(this));

                _sensorInTamper = new BoolVariable(sensorState == State.Short
                                                   || sensorState == State.Break);

                _sensorInTamper.AddListenerAndGetState(new SensorInTamperListener(this));

                _sensorInAlarmOrTamper = new OrOperation();
                _sensorInAlarmOrTamper.AddOperands(
                    new[]
                    {
                        _sensorInAlarm,
                        _sensorInTamper
                    });

                var sensorInAlarmOrTamperState =
                    _sensorInAlarmOrTamper.AddListenerAndGetState(new SensorInAlarmOrTamperListener(this));

                if (sensorInAlarmOrTamperState == true)
                    _controller.OnSensorAlarmOrTamperStateChanged(true);

                _sensorAlarmKey =
                    SensorAlarm.CreateAlarmKey(
                        IdInput,
                        _controller.Id);

                _sensorTamperAlarmKey =
                    SensorTamperAlarm.CreateAlarmKey(
                        IdInput,
                        _controller.Id);

                _sensorAlarmNotAcknowledged = new BoolVariable(
                    AlarmsManager.Singleton.ExistsNotAcknowledgedAlarm(_sensorAlarmKey));

                _sensorTamperAlarmNotAcknowledged = new BoolVariable(
                    AlarmsManager.Singleton.ExistsNotAcknowledgedAlarm(_sensorTamperAlarmKey));

                var sensorNotAcknowledged = new OrOperation();
                sensorNotAcknowledged.AddOperands(
                    new[]
                    {
                        _sensorAlarmNotAcknowledged,
                        _sensorTamperAlarmNotAcknowledged
                    });

                sensorNotAcknowledged.AddListenerAndGetState(new SensorNotAcknowledgedListener(this));

                _sensorPermanentlyBlocked = new BoolVariable(
                    _sensorBlockingType == SensorBlockingType.BlockPermanently);

                _sensorTemporarilyBlocked = new BoolVariable(
                    _sensorBlockingType == SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset
                    || _sensorBlockingType == SensorBlockingType.BlockTemporarilyUntilSensorStateNormal);

                _sensorBlocked = new OrOperation();
                _sensorBlocked.AddOperands(
                    new[]
                    {
                        _sensorPermanentlyBlocked,
                        _sensorTemporarilyBlocked
                    });


                _sensorBlockedWhenActivated = new AndOperation();
                _sensorBlockedWhenActivated.AddOperands(
                    new[]
                    {
                        _controller.Activated,
                        _sensorBlocked
                    });

                _sensorBlockedWhenActivated.AddListenerAndGetState(new SensorBlockedWhenActivatedListener(this));

                if (_criticalInput)
                    _controller._anyCriticalSensorInAlarm.AddOperand(_sensorInAlarm);
                else
                    _controller._anyNonCriticalSensorInAlarm.AddOperand(_sensorInAlarm);

                _controller._anySensorInTamper.AddOperand(_sensorInTamper);
                _controller._anySensorNotAcknowledged.AddOperand(sensorNotAcknowledged);
                _controller._anySensorPermanentlyBlocked.AddOperand(_sensorPermanentlyBlocked);
                _controller._anySensorTemporarilyBlocked.AddOperand(_sensorTemporarilyBlocked);

                _controller.AlarmAreaStateAndSettings.SensorsEventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnSensorAdded(this));

                Events.ProcessEvent(new EventSensorStateInfo(
                    IdInput,
                    IdAlarmArea,
                    sensorState));

                Events.ProcessEvent(new EventSensorBlockingTypeInfo(
                    IdInput,
                    IdAlarmArea,
                    _sensorBlockingType));
            }

            private SensorPurpose GetSensorPurpose(SensorPurpose? specificSensorPurpose)
            {
                if (specificSensorPurpose.HasValue)
                    return specificSensorPurpose.Value;

                return _controller.AlarmAreaStateAndSettings.DbObject.Purpose;
            }

            void CreateEventSensorStateChangedForCardReader(State sensorState)
            {
                if (sensorState == _sensorStateForCardReader)
                    return;

                if (_sensorInTamper.State == true
                    && sensorState != State.Short
                    && sensorState != State.Break)
                {
                    return;
                }

                _sensorStateForCardReader = sensorState;

                Events.ProcessEvent(
                    new EventSensorStateChangedOnlyForCardReader(
                        IdInput,
                        IdAlarmArea,
                        _sensorStateForCardReader));
            }

            public void EnqueueSetSensorBlockingTypeRequest(
                SensorBlockingType sensorBlockingType,
                Guid idCardReader,
                AccessDataBase accessData)
            {

                new SetSensorBlockingTypeRequest(this,
                    sensorBlockingType,
                    idCardReader,
                    accessData).EnqueueAsync(_controller._processingQueue);
            }

            public void AcknowledgeAlarms(
                Guid idCardReader,
                AccessDataBase accessData)
            {
                var actionFromCcuSources =
                    new AlarmsManager.ActionFromCcuSources(
                        idCardReader,
                        accessData);

                AlarmsManager.Singleton.AcknowledgeAlarm(
                    _sensorAlarmKey,
                    actionFromCcuSources);

                AlarmsManager.Singleton.AcknowledgeAlarm(
                    _sensorTamperAlarmKey,
                    actionFromCcuSources);
            }

            public void BlockTemporarilyAndAcknowledge(
                Guid idCardReader,
                AccessDataBase accessData)
            {
                AcknowledgeAlarms(
                    idCardReader,
                    accessData);

                EnqueueSetSensorBlockingTypeRequest(
                    GetTemplateBlockingType(DefaultBlockTemporarilyUntilType),
                    idCardReader,
                    accessData);
            }

            public void BlockPermanentlyAndAcknowledge(
                Guid idCardReader,
                AccessDataBase accessData)
            {
                AcknowledgeAlarms(
                    idCardReader,
                    accessData);

                EnqueueSetSensorBlockingTypeRequest(
                    SensorBlockingType.BlockPermanently,
                    idCardReader,
                    accessData);
            }

            public void Dispose()
            {
                Inputs.Singleton.RemoveInputChangedListener(
                    IdInput,
                    _controller._inputChangedListener,
                    _controller.Id);

                if (_criticalInput)
                    _controller._anyCriticalSensorInAlarm.RemoveOperand(_sensorInAlarm);
                else
                    _controller._anyNonCriticalSensorInAlarm.RemoveOperand(_sensorInAlarm);

                _controller._anySensorInTamper.RemoveOperand(_sensorInTamper);

                if (_sensorInAlarm.State == true
                    || _sensorInTamper.State == true)
                {
                    _controller.OnSensorAlarmOrTamperStateChanged(false);
                }

                _controller._anySensorNotAcknowledged.RemoveOperands(
                    new[]
                    {
                        _sensorAlarmNotAcknowledged,
                        _sensorTamperAlarmNotAcknowledged
                    });

                _controller._anySensorPermanentlyBlocked.RemoveOperand(_sensorPermanentlyBlocked);

                _controller._anySensorTemporarilyBlocked.RemoveOperand(_sensorTemporarilyBlocked);

                _sensorBlockedWhenActivated.RemoveOperands(
                    new[]
                    {
                        _controller.Activated,
                        _sensorBlocked
                    });

                _controller.AlarmAreaStateAndSettings.SensorsEventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnSensorRemoved(this));

                AlarmAreas.Singleton.SaveSensorBlocking(
                    IdAlarmArea,
                    IdInput,
                    SensorBlockingType.Unblocked);
            }

            public void Update(DB.AAInput aaInput)
            {
                SensorId = aaInput.Id;

                var oldSensorPurpose = SensorPurpose;

                SensorPurpose = GetSensorPurpose(aaInput.Purpose);

                if (oldSensorPurpose != SensorPurpose)
                    OnSensorPurposeChanged(oldSensorPurpose);

                var wasCritical = _criticalInput;

                _criticalInput = !aaInput.NoCriticalInput;

                _blockTemporarilyUntilType =
                    (DB.BlockTemporarilyUntilType)
                    aaInput.BlockTemporarilyUntil;

                if (wasCritical == _criticalInput)
                    return;

                if (wasCritical)
                    _controller._anyCriticalSensorInAlarm.RemoveOperand(_sensorInAlarm);
                else
                    _controller._anyNonCriticalSensorInAlarm.RemoveOperand(_sensorInAlarm);

                if (_criticalInput)
                    _controller._anyCriticalSensorInAlarm.AddOperand(_sensorInAlarm);
                else
                    _controller._anyNonCriticalSensorInAlarm.AddOperand(_sensorInAlarm);
            }

            private void OnSensorPurposeChanged(SensorPurpose oldSensorPurpose)
            {
                if (_sensorBlockedWhenActivated.State != true)
                    return;

                var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                    AlarmType.AlarmArea_Alarm,
                    new IdAndObjectType(
                        _controller.Id,
                        ObjectType.AlarmArea));

                if (alarmArcs == null)
                    return;

                var input = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Input,
                    IdInput) as DB.Input;

                if (input == null)
                    return;

                CatAlarmsManager.Singleton.SendCatAlarms(
                    new[]
                    {
                        CatAlarmsManager.CreateSensorAlarm(
                            null,
                            false,
                            input,
                            _controller.AlarmAreaStateAndSettings.DbObject,
                            oldSensorPurpose,
                            true,
                            alarmArcs)
                    });

                CatAlarmsManager.Singleton.SendCatAlarms(
                    new[]
                    {
                        CatAlarmsManager.CreateSensorAlarm(
                            null,
                            true,
                            input,
                            _controller.AlarmAreaStateAndSettings.DbObject,
                            SensorPurpose,
                            true,
                            alarmArcs)
                    });
            }

            public void OnInputChanged(State inputState)
            {
                _currentInputStateCache = inputState;

                switch (_currentInputStateCache)
                {
                    case State.Normal:

                        if (SensorBlockingType == SensorBlockingType.BlockTemporarilyUntilSensorStateNormal)
                            SetSensorBlockingTypeSync(
                                SensorBlockingType.Unblocked,
                                Guid.Empty,
                                Guid.Empty,
                                Guid.Empty);

                        AlarmsManager.Singleton.StopAlarm(
                            _sensorAlarmKey);

                        _sensorInAlarm.SetState(false);

                        AlarmsManager.Singleton.StopAlarm(
                            _sensorTamperAlarmKey);

                        _sensorInTamper.SetState(false);

                        Events.ProcessEvent(
                            new EventSensorStateChanged(
                                IdInput,
                                IdAlarmArea,
                                State.Normal));

                        CreateEventSensorStateChangedForCardReader(State.Normal);

                        break;

                    case State.Alarm:

                        if (SensorBlockingType != SensorBlockingType.Unblocked)
                            break;

                        AlarmsManager.Singleton.StopAlarm(
                            _sensorTamperAlarmKey);

                        var previousAlarmAreaAlarmState = _controller.AlarmState;

                        _sensorInAlarm.SetState(true);
                        _sensorInTamper.SetState(false);

                        Events.ProcessEvent(
                            new EventSensorStateChanged(
                                IdInput,
                                IdAlarmArea,
                                State.Alarm));

                        if (previousAlarmAreaAlarmState == State.Alarm)
                        {
                            CreateSensorAlarm();
                            break;
                        }

                        if (_controller.AlarmState != State.Alarm)
                            switch (_controller.ActivationState)
                            {
                                case State.Prewarning:

                                    BlockSensorActiveDuringPrewarning();

                                    break;

                                case State.TemporaryUnsetEntry:

                                    _createDelayedSensorAlarm = true;
                                    break;
                            }

                        break;

                    case State.Short:
                    case State.Break:

                        if (SensorBlockingType != SensorBlockingType.Unblocked)
                            break;

                        AlarmsManager.Singleton.StopAlarm(
                            _sensorAlarmKey);

                        _sensorInTamper.SetState(true);
                        _sensorInAlarm.SetState(false);

                        if (_controller.ActivationState == State.Prewarning)
                        {
                            BlockSensorActiveDuringPrewarning();
                            return;
                        }

                        AlarmsManager.Singleton.AddAlarm(
                            new SensorTamperAlarm(
                                IdInput,
                                _controller.Id));

                        _sensorTamperAlarmNotAcknowledged.SetState(true);

                        Events.ProcessEvent(
                            new EventSensorStateChanged(
                                IdInput,
                                IdAlarmArea,
                                _currentInputStateCache));

                        CreateEventSensorStateChangedForCardReader(_currentInputStateCache);

                        break;
                }
            }

            private void BlockSensorActiveDuringPrewarning()
            {
                var continueInActivatationRequest = _controller._continueInActivatationRequest;

                if (continueInActivatationRequest == null
                    || !continueInActivatationRequest.SetParameters.UnconditionalSet)
                {
                    return;
                }

                if (_sensorInTamper.State != true
                    && !_criticalInput)
                {
                    return;
                }

                SetSensorBlockingTypeSync(
                    GetTemplateBlockingType(_blockTemporarilyUntilType),
                    Guid.Empty,
                    Guid.Empty,
                    Guid.Empty);
            }

            private static SensorBlockingType GetTemplateBlockingType(
                DB.BlockTemporarilyUntilType blockTemporarilyUntilType)
            {
                switch (blockTemporarilyUntilType)
                {
                    case DB.BlockTemporarilyUntilType.AreaUnset:

                        return
                            SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset;

                    default:

                        return
                            SensorBlockingType.BlockTemporarilyUntilSensorStateNormal;
                }
            }

            public void OnAlarmAreaActivationStateChanged(State activationState)
            {
                switch (activationState)
                {
                    case State.Unset:

                        if (_sensorBlockingType == SensorBlockingType.BlockTemporarilyUntilSensorStateNormal
                            || _sensorBlockingType == SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset)
                        {
                            SetSensorBlockingTypeSync(
                                SensorBlockingType.Unblocked,
                                Guid.Empty,
                                Guid.Empty,
                                Guid.Empty);
                        }

                        _createDelayedSensorAlarm = false;
                        _sensorStateForCardReader = State.Normal;

                        AlarmsManager.Singleton.StopAlarm(
                            _sensorAlarmKey);

                        break;

                    case State.Prewarning:
                    case State.TemporaryUnsetExit:
                    case State.Set:

                        if (_sensorInAlarmOrTamper.State == true)
                        {
                            BlockSensorActiveDuringPrewarning();
                        }

                        break;

                    case State.TemporaryUnsetEntry:

                        if (_sensorInAlarm.State == true)
                            _createDelayedSensorAlarm = true;

                        break;
                }
            }

            public void OnAlarmAreaAlarmStateChanged(State alarmState)
            {
                if (alarmState != State.Alarm)
                    return;

                if (_sensorInAlarm.State == true)
                {
                    CreateSensorAlarm();
                    return;
                }

                if (!_createDelayedSensorAlarm)
                    return;

                // - we're commanded to create delayed sensor alarm (such as in case of cancelled TmpUnsetEntry)
                // - or in case of sensor's Normal->Alarm transition

                CreateSensorAlarm();

                AlarmsManager.Singleton.StopAlarm(
                    _sensorAlarmKey);

                CreateEventSensorStateChangedForCardReader(State.Normal);
            }

            private void CreateSensorAlarm()
            {
                _createDelayedSensorAlarm = false;

                AlarmsManager.Singleton.AddAlarm(
                    new SensorAlarm(
                        IdInput,
                        _controller.Id));

                _sensorAlarmNotAcknowledged.SetState(true);

                CreateEventSensorStateChangedForCardReader(State.Alarm);
            }

            public void OnSensorAlarmAcknowledged()
            {
                _sensorAlarmNotAcknowledged.SetState(false);
            }

            public void OnSensorTamperAlarmAcknowledged()
            {
                _sensorTamperAlarmNotAcknowledged.SetState(false);
            }
        }
    }
}