using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Data;
using Contal.BoolExpressions.CrossPlatform;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal partial class CcuAlarmAreaController :
        AAlarmAreaController<CcuAlarmAreaController>
    {
        private class InputChangedListener :
            IInputChangedListener
        {
            private readonly CcuAlarmAreaController _controller;

            public InputChangedListener(CcuAlarmAreaController controller)
            {
                _controller = controller;
            }

            public override int GetHashCode()
            {
                return _controller.Id.GetHashCode();
            }

            public bool Equals(IInputChangedListener other)
            {
                var otherListener = other as InputChangedListener;

                return otherListener != null
                       && _controller.Id.Equals(otherListener._controller.Id);
            }

            public void OnInputChanged(
                Guid idInput,
                State state)
            {
                new OnInputChangedRequest(
                    idInput,
                    state).EnqueueAsync(_controller._processingQueue);
            }
        }

        private class OnInputChangedRequest : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            private readonly Guid _idInput;
            private readonly State _state;

            public OnInputChangedRequest(
                Guid idInput,
                State state)
            {
                _idInput = idInput;
                _state = state;
            }

            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                SensorStateAndSettings sensorStateAndSettings;

                if (!alarmAreaController._sensors.TryGetValue(
                    _idInput,
                    out sensorStateAndSettings))
                {
                    return;
                }

                sensorStateAndSettings.OnInputChanged(_state);
            }
        }

        private class SetAlarmAreaRequest : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            private readonly AlarmAreas.SetUnsetParams _setParameters;

            public SetAlarmAreaRequest(AlarmAreas.SetUnsetParams setUnsetParams)
            {
                _setParameters = setUnsetParams;
            }

            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                if (alarmAreaController.ActivationState != State.Unset)
                    return;

                alarmAreaController.StartActivation(
                    new ContinueInActivatationRequest(_setParameters));
            }
        }

        private class ContinueInActivatationRequest
            : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            public AlarmAreas.SetUnsetParams SetParameters { get; private set; }

            public ContinueInActivatationRequest(AlarmAreas.SetUnsetParams setUnsetParams)
            {
                SetParameters = setUnsetParams;
            }

            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                switch (alarmAreaController.ActivationState)
                {
                    case State.Prewarning:

                        alarmAreaController.ContinueInActivatationAfterPrewarning();
                        break;

                    case State.TemporaryUnsetExit:

                        alarmAreaController.CompleteActivation();

                        if (alarmAreaController._anyNonCriticalSensorInAlarm.State == true
                            && !alarmAreaController.TryStartPreAlarm())
                        {
                            alarmAreaController._inAlarm.SetState(true);
                        }

                        break;
                }
            }
        }

        private class CompletePreAlarmRequest : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                alarmAreaController.CompletePreAlarm();

                alarmAreaController._inAlarm.SetState(true);

                if (alarmAreaController._anySensorInAlarmOrTamper.State != true)
                    alarmAreaController._inAlarm.SetState(false);
            }
        }

        private abstract class OnSensorAlarmOrTamperAlarmAcknowledgedRequest
            : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            protected Guid IdSensor { get; private set; }

            protected OnSensorAlarmOrTamperAlarmAcknowledgedRequest(Guid idSensor)
            {
                IdSensor = idSensor;
            }
        }

        private class OnSensorAlarmAcknowledgedRequest : OnSensorAlarmOrTamperAlarmAcknowledgedRequest
        {
            public OnSensorAlarmAcknowledgedRequest(Guid idSensor)
                : base(idSensor)
            {
            }

            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                SensorStateAndSettings sensorStateAndSettings;

                if (!alarmAreaController._sensors.TryGetValue(
                    IdSensor,
                    out sensorStateAndSettings))
                {
                    return;
                }

                sensorStateAndSettings.OnSensorAlarmAcknowledged();
            }
        }

        private class OnSensorTamperAlarmAcknowledgedRequest
            : OnSensorAlarmOrTamperAlarmAcknowledgedRequest
        {
            public OnSensorTamperAlarmAcknowledgedRequest(Guid idSensor)
                : base(idSensor)
            {
            }

            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                SensorStateAndSettings sensorStateAndSettings;

                if (!alarmAreaController._sensors.TryGetValue(
                    IdSensor,
                    out sensorStateAndSettings))
                {
                    return;
                }

                sensorStateAndSettings.OnSensorTamperAlarmAcknowledged();
            }
        }

        //---------------------------------------
        private class UnsetAlarmAreaRequest : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            private readonly UnsetAlarmAreaExecutor _unsetAlarmAreaExecutor;

            public UnsetAlarmAreaRequest(UnsetAlarmAreaExecutor unsetAlarmAreaExecutor)
            {
                _unsetAlarmAreaExecutor = unsetAlarmAreaExecutor;
            }

            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                if (!_unsetAlarmAreaExecutor.CheckUnsetPreconditions(alarmAreaController))
                    return;

                var idActivationObject = _unsetAlarmAreaExecutor.SetUnsetParams.IdActivationObject;

                if (idActivationObject != Guid.Empty)
                    alarmAreaController.SendEventAlarmAreaSetUnsetByObjectForAa(
                        idActivationObject,
                        false);

                _unsetAlarmAreaExecutor.Execute(alarmAreaController);
            }
        }

        private class OnAlarmAcknowledgedRequest : WaitableProcessingRequest<CcuAlarmAreaController>
        {
            protected override void ExecuteInternal(CcuAlarmAreaController alarmAreaController)
            {
                alarmAreaController._alarmNotAcknowledged.SetState(false);
            }
        }

        private abstract class AListener : BoolExpressionStateChangedListener
        {
            protected readonly CcuAlarmAreaController Controller;

            protected AListener(CcuAlarmAreaController controller)
            {
                Controller = controller;
            }
        }

        private class AnyCriticalSensorInAlarmListener : AListener
        {
            public AnyCriticalSensorInAlarmListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                if (newState == true)
                    Controller.AnounceAlarmState();
            }
        }

        private class OnAlarmNotAcknowledgedListener : AListener
        {
            public OnAlarmNotAcknowledgedListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                Controller.AlarmAreaStateAndSettings
                    .OnNotAcknowledgedStateChanged(newState == true);
            }
        }

        private class AnySensorTemporarilyBlockedListener : AListener
        {
            public AnySensorTemporarilyBlockedListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnAnySensorTemporarilyBlockedChanged(
                        Controller.AlarmAreaStateAndSettings,
                        newState ?? false));
            }
        }

        private class AnySensorPermanetlyBlockedListener : AListener
        {
            public AnySensorPermanetlyBlockedListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnAnySensorPermanentlyBlockedChanged(
                        Controller.AlarmAreaStateAndSettings,
                        newState ?? false));
            }
        }

        private class AnySensorNotAcknowledgedListener : AListener
        {
            public AnySensorNotAcknowledgedListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnAnySensorNotAcknowledgedChanged(
                        Controller.AlarmAreaStateAndSettings,
                        newState ?? false));
            }
        }

        private class OnAnySensorInAlarmListener : AListener
        {
            public OnAnySensorInAlarmListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnAnySensorInAlarmChanged(
                        Controller.AlarmAreaStateAndSettings,
                        newState ?? false));
            }
        }

        private class OnSensorInAlarmOrTamperListener : AListener
        {
            public OnSensorInAlarmOrTamperListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                if (newState == true)
                    return;

                Controller._inAlarm.SetState(false);

                if (Controller.RetrySetAlarmAreaByObjectForAa)
                    Controller.OnActivatedObjectForAa();
            }
        }

        private class InAlarmListener : AListener
        {
            public InAlarmListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                var alarmState = Controller.AlarmState;
                var alarmAreaStateAndSettingsa = Controller.AlarmAreaStateAndSettings;

                AlarmAreaAlarm.Update(alarmAreaStateAndSettingsa.Id, alarmState);
                Events.ProcessEvent(
                    new EventaAlarmAreaAlarmStateChanged(alarmAreaStateAndSettingsa.Id, alarmState));

                foreach (var sensor in Controller._sensors.ValuesSnapshot)
                    sensor.OnAlarmAreaAlarmStateChanged(alarmState);

                alarmAreaStateAndSettingsa.OnAlarmStateChanged(alarmState);

                if (newState.HasValue && newState.Value)
                    Controller._alarmNotAcknowledged.SetState(true);

                Controller.UpdateAAlarm();

                if (newState == true)
                    Controller._sirenOutputController.TurnOn();
            }
        }

        private class AnySensorInTamperListener : AListener
        {
            public AnySensorInTamperListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                if (newState == true)
                    Controller.AnounceAlarmState();

                var alarmAreaStateAndSettings = Controller.AlarmAreaStateAndSettings;

                alarmAreaStateAndSettings.OnSabotageStateChanged(newState ?? false);

                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnAnySensorInTamperChanged(
                        alarmAreaStateAndSettings,
                        newState ?? false));
            }
        }

        private class AnyNonCriticalSensorInAlarmListener : AListener
        {
            public AnyNonCriticalSensorInAlarmListener(CcuAlarmAreaController controller)
                : base(controller)
            {
            }

            public override void OnStateChanged(bool? oldState, bool? newState)
            {
                if (!newState.HasValue
                    || !newState.Value
                    || Controller.ActivationState != State.Set)
                {
                    return;
                }

                var state = Controller._inAlarm.State;

                if (state == null || !state.Value)
                {
                    if (!Controller.TryStartPreAlarm())
                        Controller._inAlarm.SetState(true);
                }
            }
        }

        private class SirenOutputController
        {
            private class TurnOffSirenOutputListener :
                BoolExpressionStateChangedListener
            {
                private readonly SirenOutputController _sirenOutputController;

                public TurnOffSirenOutputListener(SirenOutputController sirenOutputController)
                {
                    _sirenOutputController = sirenOutputController;
                }

                public override void OnStateChanged(bool? oldState, bool? newState)
                {
                    if (newState != true)
                    {
                        _sirenOutputController.TurnOff();
                    }
                }
            }

            private ITimer _timer;
            public BoolVariable SirenOutput { get; private set; }
            public int? SirenMaxOnPeriod { private get; set; }

            public SirenOutputController(CcuAlarmAreaController controller)
            {
                SirenOutput = new BoolVariable(false);

                var activatedAndAlarmNotAcknowledged = new AndOperation();
                activatedAndAlarmNotAcknowledged.AddOperands(
                    new[]
                    {
                        controller.Activated,
                        controller._alarmNotAcknowledged
                    });

                var turnOffSirenOutputListener = new TurnOffSirenOutputListener(this);
                activatedAndAlarmNotAcknowledged.AddListenerAndGetState(turnOffSirenOutputListener);
            }

            public void TurnOn()
            {
                if (SirenMaxOnPeriod.HasValue)
                {
                    _timer = TimerManager.Static.StartTimeout(
                        SirenMaxOnPeriod.Value * 1000,
                        OnSirenTimeout);
                }

                SirenOutput.SetState(true);
            }

            private void TurnOff()
            {
                if (_timer != null)
                {
                    _timer.StopTimer();
                    _timer = null;
                }

                SirenOutput.SetState(false);
            }

            private bool OnSirenTimeout(ITimer timer)
            {
                _timer = null;
                SirenOutput.SetState(false);

                return true;
            }
        }

        private readonly SyncDictionary<Guid, SensorStateAndSettings> _sensors =
            new SyncDictionary<Guid, SensorStateAndSettings>();

        private ContinueInActivatationRequest _continueInActivatationRequest;

        private readonly InputChangedListener _inputChangedListener;

        private readonly BoolVariable _inAlarm;
        private readonly BoolVariable _alarmNotAcknowledged;
        private readonly OrOperation _anyCriticalSensorInAlarm;
        private readonly OrOperation _anyNonCriticalSensorInAlarm;
        private readonly OrOperation _anySensorInAlarm;
        private readonly OrOperation _anySensorInTamper;
        private readonly SirenOutputController _sirenOutputController;

        private readonly OrOperation _anySensorInAlarmOrTamper;

        private readonly OrOperation _anySensorNotAcknowledged;

        private readonly OrOperation _anySensorTemporarilyBlocked;

        private readonly OrOperation _anySensorPermanentlyBlocked;

        private int _sensorsInAlarmOrTamperCount;

        public override IBoolExpression InAlarm
        {
            get { return _inAlarm; }
        }

        public override IBoolExpression SirenOutput
        {
            get { return _sirenOutputController.SirenOutput; }
        }

        public override IBoolExpression InSabotage
        {
            get { return _anySensorInTamper; }
        }

        public override IBoolExpression AnySensorInAlarm
        {
            get { return _anySensorInAlarm; }
        }

        public override IBoolExpression AlarmNotAcknowledged
        {
            get { return _alarmNotAcknowledged; }
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable

        public CcuAlarmAreaController(AlarmAreaStateAndSettings alarmAreaStateAndSettings)
            : base(alarmAreaStateAndSettings)
        {
            _inputChangedListener = new InputChangedListener(this);

            _inAlarm = new BoolVariable(false);
            var listenerInAlarm = new InAlarmListener(this);
            _inAlarm.AddListenerAndGetState(listenerInAlarm);

            _alarmNotAcknowledged = new BoolVariable(
                AlarmsManager.Singleton.ExistsNotAcknowledgedAlarm(
                    AlarmAreaAlarm.CreateAlarmKey(alarmAreaStateAndSettings.Id)));

            var listenerAlarmNotAcknowledge = new OnAlarmNotAcknowledgedListener(this);
            _alarmNotAcknowledged.AddListenerAndGetState(listenerAlarmNotAcknowledge);

            _sirenOutputController = new SirenOutputController(this);

            _anyCriticalSensorInAlarm = new OrOperation();
            var listenerAnyCriticalSensorInAlarm = new AnyCriticalSensorInAlarmListener(this);
            _anyCriticalSensorInAlarm.AddListenerAndGetState(listenerAnyCriticalSensorInAlarm);

            _anyNonCriticalSensorInAlarm = new OrOperation();
            var listenerAnyNonCriticalSensorInAlarm = new AnyNonCriticalSensorInAlarmListener(this);
            _anyNonCriticalSensorInAlarm.AddListenerAndGetState(listenerAnyNonCriticalSensorInAlarm);

            _anySensorInAlarm = new OrOperation();
            _anySensorInAlarm.AddOperands(
                new[]
                {
                    _anyCriticalSensorInAlarm,
                    _anyNonCriticalSensorInAlarm
                });

            var listenerAnySensorInAlarmListener = new OnAnySensorInAlarmListener(this);
            _anySensorInAlarm.AddListenerAndGetState(listenerAnySensorInAlarmListener);

            _anySensorInTamper = new OrOperation();
            var listenerAnySensorInTamper = new AnySensorInTamperListener(this);
            _anySensorInTamper.AddListenerAndGetState(listenerAnySensorInTamper);

            _anySensorInAlarmOrTamper = new OrOperation();
            _anySensorInAlarmOrTamper.AddOperands(
                new[]
                {
                    _anySensorInAlarm,
                    _anySensorInTamper
                });

            var listnerSensorInAlarmOrTamper = new OnSensorInAlarmOrTamperListener(this);
            _anySensorInAlarmOrTamper.AddListenerAndGetState(listnerSensorInAlarmOrTamper);

            _anySensorNotAcknowledged = new OrOperation();
            var listenerAnySensorNotAcknowledged = new AnySensorNotAcknowledgedListener(this);
            _anySensorNotAcknowledged.AddListenerAndGetState(listenerAnySensorNotAcknowledged);

            _anySensorTemporarilyBlocked = new OrOperation();
            var listenerAnySensorTemporarilyBlocked = new AnySensorTemporarilyBlockedListener(this);
            _anySensorTemporarilyBlocked.AddListenerAndGetState(listenerAnySensorTemporarilyBlocked);

            _anySensorPermanentlyBlocked = new OrOperation();
            var listenerAnySensorPermanetlyBlocked = new AnySensorPermanetlyBlockedListener(this);
            _anySensorPermanentlyBlocked.AddListenerAndGetState(listenerAnySensorPermanetlyBlocked);

            alarmAreaStateAndSettings.SendEventSabotageStateChanged(
                _anySensorInTamper.State ?? false,
                false);
        }

        private void OnSensorAlarmOrTamperStateChanged(bool isInAlarmOrTamper)
        {
            if (isInAlarmOrTamper)
                ++_sensorsInAlarmOrTamperCount;
            else
                --_sensorsInAlarmOrTamperCount;

            UpdateAAlarm();
        }

        private void UpdateAAlarm()
        {
            if (_inAlarm.State != true ||
               _sensorsInAlarmOrTamperCount < AlarmAreaStateAndSettings.SensorsCountToAAlarm)
            {
                AlarmAreaStateAndSettings.SetSpecialOutputAAlarmOff();

                AlarmsManager.Singleton.StopAlarm(
                    AlarmAreaAAlarm.CreateAlarmKey(Id));

                return;
            }

            if (!AlarmAreaStateAndSettings.ABAlarmHandling)
                return;

            AlarmAreaStateAndSettings.SetSpecialOutputAAlarmOn();

            AlarmsManager.Singleton.AddAlarm(
                new AlarmAreaAAlarm(
                    Id,
                    _sensorsInAlarmOrTamperCount));
        }

        private void AnounceAlarmState()
        {
            if (Activated.State != true)
                return;

            switch (ActivationState)
            {
                case State.TemporaryUnsetExit:

                    CompleteActivation();
                    break;

                case State.TemporaryUnsetEntry:

                    CompletePreAlarm();
                    break;
            }

            _inAlarm.SetState(true);
        }

        protected override CcuAlarmAreaController This
        {
            get { return this; }
        }

        public override IBoolExpression AnySensorNotAcknowledged
        {
            get { return _anySensorNotAcknowledged; }
        }

        public override IBoolExpression AnySensorTemporarilyBlocked
        {
            get { return _anySensorTemporarilyBlocked; }
        }

        public override IBoolExpression AnySensorPermanentlyBlocked
        {
            get { return _anySensorPermanentlyBlocked; }
        }

        public override IBoolExpression NotAcknowledged
        {
            get { return _alarmNotAcknowledged; }
        }

        public override void OnAlarmAcknowledged(AlarmsManager.IActionSources acknowledgeSources)
        {
            Events.ProcessEvent(
                new EventAlarmAreaAlarmStateChangedAlarm(
                    Id,
                    State.Acknowledge,
                    acknowledgeSources.IdCardReader,
                    acknowledgeSources.IdCard,
                    acknowledgeSources.IdPerson));

            new OnAlarmAcknowledgedRequest().EnqueueAsync(_processingQueue);
        }

        protected override void BeginInit(DB.AlarmArea alarmArea)
        {
            SetAlarmAreaInitialStates(AlarmAreas.Singleton.GetAlarmAreaStates(Id));
            _sirenOutputController.SirenMaxOnPeriod = alarmArea.SirenMaxOnPeriod;
            ConfigureSensors(alarmArea);
        }

        public override ICollection<ISensorStateAndSettings> GetSensors()
        {
            var result = new LinkedList<ISensorStateAndSettings>();

            _sensors.ForEach((key, value) => result.AddLast(value));

            return result;
        }

        public override void AcknowledgeAllSensorAlarms(
            Guid idCardReader,
            AccessDataBase accessData)
        {
            _sensors.ForEach(
                (key, value) => value.AcknowledgeAlarms(
                    idCardReader,
                    accessData));
        }

        public override void SendBlockingTypeForAllSensors()
        {
            _sensors.ForEach(
                (key, value) => Events.ProcessEvent(new EventSensorBlockingTypeInfo(
                    key,
                    Id,
                    value.SensorBlockingType)));
        }

        public override void SendStateForAllSensors()
        {
            _sensors.ForEach(
                (key, value) => Events.ProcessEvent(new EventSensorStateInfo(
                    key,
                    Id,
                    value.SensorState)));
        }

        public override void SetSensorBlockingType(
            Guid idInput,
            SensorBlockingType sensorBlockingType)
        {
            _sensors.TryGetValue(
                idInput,
                (key, found, value) =>
                {
                    if (found)
                        value.EnqueueSetSensorBlockingTypeRequest(
                            sensorBlockingType,
                            Guid.Empty,
                            new AccessDataBase());
                });
        }

        protected override void BeginUpdate(DB.AlarmArea alarmArea)
        {
            _sirenOutputController.SirenMaxOnPeriod = alarmArea.SirenMaxOnPeriod;
            ConfigureSensors(alarmArea);
        }

        private void ConfigureSensors(DB.AlarmArea alarmArea)
        {
            var idAaInputs = alarmArea.GuidAAInputs;

            var idSensorsToRemove =
                new HashSet<Guid>(_sensors.ValuesSnapshot.Select(sensor => sensor.IdInput));

            if (idAaInputs != null)
                foreach (var aaInputGuid in idAaInputs)
                {
                    var aaInput =
                        Database.ConfigObjectsEngine.GetFromDatabase(
                            ObjectType.AAInput,
                            aaInputGuid) as DB.AAInput;

                    if (aaInput == null)
                        continue;

                    var idInput = aaInput.GuidInput;

                    _sensors.GetOrAddValue(
                        idInput,
                        key => new SensorStateAndSettings(
                            this,
                            aaInput),
                        (key,
                            value,
                            newlyAdded) =>
                        {
                            if (newlyAdded)
                                return;

                            value.Update(aaInput);
                        });

                    idSensorsToRemove.Remove(idInput);
                }

            foreach (var idSensor in idSensorsToRemove)
                _sensors.Remove(
                    idSensor,
                    (key,
                        removed,
                        value) =>
                    {
                        if (removed)
                        {
                            value.Dispose();
                        }
                    });
        }

        private void SetAlarmAreaInitialStates(AlarmAreaStates alarmAreaStates)
        {
            if (alarmAreaStates == null)
                return;

            switch (alarmAreaStates.ActivationState)
            {
                case State.Prewarning:

                    StartActivation(
                        new ContinueInActivatationRequest(
                            new AlarmAreas.SetUnsetParams(
                                Guid.Empty,
                                new AccessDataBase(),
                                Guid.Empty,
                                false,
                                false)));

                    break;

                case State.TemporaryUnsetExit:

                    _continueInActivatationRequest =
                        new ContinueInActivatationRequest(
                            new AlarmAreas.SetUnsetParams(
                                Guid.Empty,
                                new AccessDataBase(),
                                Guid.Empty,
                                false,
                                false));

                    ContinueInActivatationAfterPrewarning();

                    break;

                case State.TemporaryUnsetEntry:

                    ChangeActivationState(
                        State.Set,
                        null);

                    TryStartPreAlarm();

                    break;

                default:

                    ChangeActivationState(
                        alarmAreaStates.ActivationState,
                        null);

                    break;
            }
        }

        protected override void OnActivationStateChanged(
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            foreach (var sensor in _sensors.ValuesSnapshot)
                sensor.OnAlarmAreaActivationStateChanged(ActivationState);

            base.OnActivationStateChanged(setUnsetParams);
        }

        public override void Dispose()
        {
            base.Dispose();

            _sensors.Clear(
                (key, value) => value.Dispose(),
                null);
        }

        public override void SendSabotageStateInfoToServer()
        {
            AlarmAreaStateAndSettings.SendEventSabotageStateChanged(
                _anySensorInTamper.State == true,
                false);
        }

        private void CompleteActivation()
        {
            ChangeActivationState(
                State.Set,
                _continueInActivatationRequest.SetParameters);

            StopSetRequestTimer();
            _continueInActivatationRequest = null;

            if (AlarmAreaStateAndSettings.TimeBuyingOnlyInPrewarning)
            {
                // If "time buying only in prewarning" is enabled and alarm area is set so time buying values can be removed
                AlarmAreas.Singleton.ResetTimeBuyingForAlarmArea(Id);
            }
        }

        private NativeTimer _preAlarmTimer;

        private void StopPreAlarmTimer()
        {
            if (_preAlarmTimer != null)
            {
                _preAlarmTimer.StopTimer();
                _preAlarmTimer = null;
            }
        }

        private bool OnPreAlarmTimer(NativeTimer timerCarrier)
        {
            _preAlarmTimer = null;

            new CompletePreAlarmRequest().EnqueueAsync(_processingQueue);

            return true;
        }

        public override void OnUnset()
        {
            StopSetRequestTimer();
            StopPreAlarmTimer();
            _continueInActivatationRequest = null;

            _inAlarm.SetState(false);
        }

        private NativeTimer _setRequestTimer;

        private void StartPrewarning(int prewarningDuration)
        {
            ChangeActivationState(
                State.Prewarning,
                _continueInActivatationRequest.SetParameters);

            _setRequestTimer =
                NativeTimerManager.StartTimeout(
                    prewarningDuration * 1000,
                    OnSetRequestTimer,
                    (byte)PrirotyForOnTimerEvent.AlarmAreas);
        }

        private bool OnSetRequestTimer(NativeTimer timer)
        {
            _setRequestTimer = null;

            _continueInActivatationRequest.EnqueueAsync(_processingQueue);

            return true;
        }

        private void StartTemporaryUnsetExit(int temporaryUnsetExitDuration)
        {
            ChangeActivationState(
                State.TemporaryUnsetExit,
                _continueInActivatationRequest.SetParameters);

            _setRequestTimer =
                NativeTimerManager.StartTimeout(
                    temporaryUnsetExitDuration * 1000,
                    OnSetRequestTimer,
                    (byte)PrirotyForOnTimerEvent.AlarmAreas);
        }

        private void StopSetRequestTimer()
        {
            if (_setRequestTimer == null)
                return;

            _setRequestTimer.StopTimer();
            _setRequestTimer = null;
        }

        public override bool HasAnySensorActive()
        {
            return _anySensorInAlarmOrTamper.State == true;
        }

        public override int GetSensorId(Guid idInput)
        {
            SensorStateAndSettings sensorStateAndSettings;

            return
                _sensors.TryGetValue(
                        idInput,
                        out sensorStateAndSettings)
                    ? sensorStateAndSettings.SensorId
                    : 0;
        }

        public override SensorPurpose GetSensorPurpose(Guid idInput)
        {
            SensorStateAndSettings sensorStateAndSettings;

            return
                _sensors.TryGetValue(
                        idInput,
                        out sensorStateAndSettings)
                    ? sensorStateAndSettings.SensorPurpose
                    : SensorPurpose.BurglaryAlarm;
        }

        public override void OnSensorAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources)
        {
            Events.ProcessEvent(
                new EventSensorAlarmAknowledged(
                    idSensor,
                    Id,
                    acknowledgeSources.IdCardReader,
                    acknowledgeSources.IdCard,
                    acknowledgeSources.IdPerson));

            new OnSensorAlarmAcknowledgedRequest(idSensor)
                .EnqueueAsync(_processingQueue);
        }

        public override void OnSensorTamperAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources)
        {
            new OnSensorTamperAlarmAcknowledgedRequest(idSensor)
                .EnqueueAsync(_processingQueue);
        }

        public override void Unconfigure(DB.AlarmArea newDbObject)
        {
        }


        protected override WaitableProcessingRequest<CcuAlarmAreaController> CreateSetAlarmAreaRequestItem(AlarmAreas.SetUnsetParams setUnsetParams)
        {
            if (_anyCriticalSensorInAlarm.State == true
            || _anySensorInTamper.State == true)
            {
                if (!setUnsetParams.UnconditionalSet)
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "bool CcuAlarmAreaController.SetAlarmArea return false");

                    return null;
                }

                if (setUnsetParams.IdActivationObject != Guid.Empty)
                    AlarmAreaStateAndSettings.SetSpecialOutputSetNotCalmAaByObjectForAaOn();
            }

            return new SetAlarmAreaRequest(setUnsetParams);
        }


        protected override WaitableProcessingRequest<CcuAlarmAreaController> CreateUnsetAlarmAreaRequestItem(UnsetAlarmAreaExecutor unsetAlarmAreaExecutor)
        {
            return new UnsetAlarmAreaRequest(unsetAlarmAreaExecutor);
        }

        private void ContinueInActivatationAfterPrewarning()
        {
            var temporaryUnsetExitDuration =
                AlarmAreaStateAndSettings.TemporaryUnsetExitDuration;

            if (temporaryUnsetExitDuration != null
                && _anyCriticalSensorInAlarm.State != true
                && _anySensorInTamper.State != true
                && _anyNonCriticalSensorInAlarm.HasAnyOperand)
            {
                StartTemporaryUnsetExit(temporaryUnsetExitDuration.Value);
                return;
            }

            CompleteActivation();

            if (_anyCriticalSensorInAlarm.State == true
                || _anySensorInTamper.State == true)
            {
                _inAlarm.SetState(true);
                return;
            }

            if (_anyNonCriticalSensorInAlarm.State == true
                && !TryStartPreAlarm())
                _inAlarm.SetState(true);
        }

        private bool TryStartPreAlarm()
        {
            var temporaryUnsetEntryDuration =
                AlarmAreaStateAndSettings.TemporaryUnsetEntryDuration;

            if (!temporaryUnsetEntryDuration.HasValue)
                return false;

            ChangeActivationState(
                State.TemporaryUnsetEntry,
                null);

            _preAlarmTimer =
                NativeTimerManager.StartTimeout(
                temporaryUnsetEntryDuration.Value * 1000,
                OnPreAlarmTimer,
                (byte)PrirotyForOnTimerEvent.AlarmAreas);

            return true;
        }

        private void CompletePreAlarm()
        {
            StopPreAlarmTimer();

            ChangeActivationState(
                State.Set,
                null);
        }

        private void StartActivation(ContinueInActivatationRequest continueInActivatationRequest)
        {
            _continueInActivatationRequest = continueInActivatationRequest;

            var setParams = _continueInActivatationRequest.SetParameters;

            var idActivationObject = setParams.IdActivationObject;

            if (idActivationObject != Guid.Empty)
                SendEventAlarmAreaSetUnsetByObjectForAa(
                    idActivationObject,
                    true);

            if (!setParams.NoPrewarning)
            {
                var prewarningDuration = AlarmAreaStateAndSettings.PrewarningDuration;

                if (prewarningDuration != null)
                {
                    StartPrewarning(
                        prewarningDuration.Value);

                    return;
                }
            }

            ContinueInActivatationAfterPrewarning();
        }
    }
}