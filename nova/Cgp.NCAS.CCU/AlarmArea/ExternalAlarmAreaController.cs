using System;
using System.Collections.Generic;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal class ExternalAlarmAreaController : AAlarmAreaController<ExternalAlarmAreaController>
    {
        private class InputActivationStateChangedListener : IInputChangedListener
        {
            private readonly ExternalAlarmAreaController _externalAlarmAreaController;
            private readonly Guid _guidAlarmArea;

            public InputActivationStateChangedListener(ExternalAlarmAreaController externalAlarmAreaController)
            {
                _externalAlarmAreaController = externalAlarmAreaController;
                _guidAlarmArea = externalAlarmAreaController.AlarmAreaStateAndSettings.GuidAlarmArea;
            }

            public override int GetHashCode()
            {
                return _guidAlarmArea.GetHashCode();
            }

            public bool Equals(IInputChangedListener other)
            {
                var x = other as InputActivationStateChangedListener;

                return x != null
                       && _guidAlarmArea.Equals(x._guidAlarmArea);
            }

            public void OnInputChanged(
                Guid guidInput,
                State state)
            {
                new ActivationInputChangedRequest(state).EnqueueAsync(_externalAlarmAreaController._processingQueue);
            }
        }

        private abstract class ABeginHandshakeRequest
            : WaitableProcessingRequest<ExternalAlarmAreaController>
        {
            protected ABeginHandshakeRequest(AlarmAreas.SetUnsetParams setUnsetParams)
            {
                SetUnsetParams = setUnsetParams;
            }

            [NotNull]
            public AlarmAreas.SetUnsetParams SetUnsetParams
            {
                get;
                private set;
            }

            public abstract State RequestedActivationState
            {
                get;
            }
        }

        private class BeginUnsetHandshakeRequest :
            ABeginHandshakeRequest
        {
            private readonly UnsetAlarmAreaExecutor _unsetAlarmAreaExecutor;

            public BeginUnsetHandshakeRequest(
                [NotNull]
                UnsetAlarmAreaExecutor unsetAlarmAreaExecutor)
                : base(unsetAlarmAreaExecutor.SetUnsetParams)
            {
                _unsetAlarmAreaExecutor = unsetAlarmAreaExecutor;
            }

            protected override void ExecuteInternal(ExternalAlarmAreaController alarmAreaController)
            {
                if (!_unsetAlarmAreaExecutor.CheckUnsetPreconditions(alarmAreaController))
                    return;

                if (alarmAreaController.RequestSetOrUnsetExternAlarmArea(this))
                    _unsetAlarmAreaExecutor.AlarmAreaUnsetResult.OnSucceded(
                        0,
                        0,
                        false);
            }

            public override State RequestedActivationState
            {
                get { return State.Unset; }
            }
        }

        private class BeginSetHandshakeRequest
            : ABeginHandshakeRequest
        {
            public BeginSetHandshakeRequest(AlarmAreas.SetUnsetParams setUnsetParams)
                : base(setUnsetParams)
            {
            }

            protected override void ExecuteInternal(ExternalAlarmAreaController alarmAreaController)
            {
                alarmAreaController.RequestSetOrUnsetExternAlarmArea(this);
            }

            public override State RequestedActivationState
            {
                get { return State.Set; }
            }
        }

        private class FilterTimeSetUnsetExpiredRequest :
            WaitableProcessingRequest<ExternalAlarmAreaController>
        {
            protected override void ExecuteInternal(ExternalAlarmAreaController alarmAreaController)
            {
                alarmAreaController.OnFilterTimeSetUnsetExpired();
            }
        }

        private Guid _idActivationInput = Guid.Empty;
        private Guid _idOutputForSetUnset = Guid.Empty;

        private int _filterTimeForSetUnset = 30;

        private readonly IInputChangedListener _activationInputListener;

        private NativeTimer _filterTimeSetUnsetTimer;
        private readonly object _lockFilterTimeSetUnsetTimer = new object();

        private ABeginHandshakeRequest _currentHandshakeRequest;
        private ABeginHandshakeRequest _lastHandshakeRequest;

        private readonly IBoolExpression _anySensorInAlarm;
        private readonly IBoolExpression _inAlarm;
        private readonly IBoolExpression _sirenOutput;
        private readonly IBoolExpression _inSabotage;
        private readonly IBoolExpression _alarmNotAcknowledged;
        private readonly IBoolExpression _anySensorNotAcknowledged;
        private readonly IBoolExpression _anySensorTemporarilyBlocked;
        private readonly IBoolExpression _anySensorPermanentlyBlocked;
        private readonly IBoolExpression _notAcknowledged;

        public ExternalAlarmAreaController(AlarmAreaStateAndSettings alarmAreaStateAndSettings)
            : base(alarmAreaStateAndSettings)
        {
            _activationInputListener =
                new InputActivationStateChangedListener(this);

            _inAlarm = new BoolVariable(false);
            _inSabotage = new BoolVariable(false);
            _alarmNotAcknowledged = new BoolVariable(false);
            _anySensorInAlarm = new BoolVariable(false);
            _sirenOutput = new BoolVariable(false);
            _anySensorNotAcknowledged = new BoolVariable(false);
            _anySensorPermanentlyBlocked = new BoolVariable(false);
            _anySensorTemporarilyBlocked = new BoolVariable(false);
            _notAcknowledged = new BoolVariable(false);
        }

        private class ActivationInputChangedRequest
            : WaitableProcessingRequest<ExternalAlarmAreaController>
        {
            private readonly State _inputState;

            public ActivationInputChangedRequest(State inputState)
            {
                _inputState = inputState;
            }

            protected override void ExecuteInternal(ExternalAlarmAreaController alarmAreaController)
            {
                switch (_inputState)
                {
                    case State.Alarm:

                        alarmAreaController.SetAlarmAreaCore();
                        break;

                    case State.Normal:

                        alarmAreaController.UnsetAlarmAreaCore();
                        break;

                }
            }
        }

        protected override ExternalAlarmAreaController This
        {
            get { return this; }
        }

        public override IBoolExpression AnySensorInAlarm
        {
            get { return _anySensorInAlarm; }
        }

        public override IBoolExpression AnySensorNotAcknowledged
        {
            get
            {
                return _anySensorNotAcknowledged;
            }
        }

        public override IBoolExpression AnySensorTemporarilyBlocked
        {
            get
            {
                return _anySensorTemporarilyBlocked;
            }
        }

        public override IBoolExpression AnySensorPermanentlyBlocked
        {
            get
            {
                return _anySensorPermanentlyBlocked;
            }
        }

        public override IBoolExpression NotAcknowledged
        {
            get
            {
                return _notAcknowledged;
            }
        }

        public override IBoolExpression InAlarm
        {
            get { return _inAlarm; }
        }

        public override IBoolExpression SirenOutput
        {
            get { return _sirenOutput; }
        }

        public override IBoolExpression InSabotage
        {
            get { return _inSabotage; }
        }

        public override IBoolExpression AlarmNotAcknowledged
        {
            get { return _alarmNotAcknowledged; }
        }

        public override void OnAlarmAcknowledged(AlarmsManager.IActionSources acknowledgeSources)
        {
        }

        protected override void BeginInit(DB.AlarmArea alarmArea)
        {
            if (alarmArea.ActivationStateInputEIS != null)
                _idActivationInput = alarmArea.ActivationStateInputEIS.Value;

            if (alarmArea.SetUnsetOutputEIS != null)
                _idOutputForSetUnset = alarmArea.SetUnsetOutputEIS.Value;

            if (alarmArea.FilterTimeEIS != null)
                _filterTimeForSetUnset = alarmArea.FilterTimeEIS.Value;

            Inputs.Singleton.AddInputChangedListener(
                _idActivationInput,
                _activationInputListener);

            SetActivationStateByActivationInput();
        }

        public override ICollection<ISensorStateAndSettings> GetSensors()
        {
            return new ISensorStateAndSettings[] { };
        }

        public override void AcknowledgeAllSensorAlarms(
            Guid idCardReader,
            AccessDataBase accessData)
        {
        }

        public override void SendBlockingTypeForAllSensors()
        {
        }

        public override void SendStateForAllSensors()
        {
        }

        public override void SetSensorBlockingType(
            Guid idInput,
            SensorBlockingType sensorBlockingType)
        {
        }

        private void SetActivationStateByActivationInput()
        {
            switch (Inputs.Singleton.GetInputLogicalState(_idActivationInput))
            {
                case State.Normal:

                    UnsetAlarmAreaCore();

                    break;

                case State.Alarm:

                    SetAlarmAreaCore();

                    break;
            }
        }

        protected override void BeginUpdate(DB.AlarmArea alarmArea)
        {
            if (alarmArea.ActivationStateInputEIS != null)
                _idActivationInput = alarmArea.ActivationStateInputEIS.Value;

            Inputs.Singleton.AddInputChangedListener(
                _idActivationInput,
                _activationInputListener);

            if (alarmArea.SetUnsetOutputEIS != null)
                _idOutputForSetUnset = alarmArea.SetUnsetOutputEIS.Value;

            if (alarmArea.FilterTimeEIS != null)
                _filterTimeForSetUnset = alarmArea.FilterTimeEIS.Value;

            SetActivationStateByActivationInput();
        }

        public override void OnUnset()
        {
        }

        public override bool HasAnySensorActive()
        {
            return false;
        }

        public override void Dispose()
        {
            base.Dispose();

            StopFilterSetUnsetTimeTimer();

            if (_idOutputForSetUnset != Guid.Empty)
            {
                Outputs.Singleton.Off(
                    AlarmAreaStateAndSettings.GuidAlarmArea.ToString(),
                    _idOutputForSetUnset);

                _idOutputForSetUnset = Guid.Empty;
            }

            Inputs.Singleton.RemoveInputChangedListener(
                _idActivationInput,
                _activationInputListener);
        }

        public override int GetSensorId(Guid idInput)
        {
            return 0;
        }

        public override SensorPurpose GetSensorPurpose(Guid idInput)
        {
            return SensorPurpose.BurglaryAlarm;
        }

        public override void OnSensorAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources)
        {
        }

        public override void OnSensorTamperAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources)
        {
        }

        public override void Unconfigure(DB.AlarmArea alarmArea)
        {
            Inputs.Singleton.RemoveInputChangedListener(
                _idActivationInput,
                _activationInputListener);

            _idActivationInput = Guid.Empty;

            if (_idOutputForSetUnset != alarmArea.SetUnsetOutputEIS)
            {
                Outputs.Singleton.Off(
                    Id.ToString(),
                    _idOutputForSetUnset);

                _idOutputForSetUnset = Guid.Empty;
            }
        }

        private void SetAlarmAreaCore()
        {
            if (Activated.State == true)
                return;

            if (_currentHandshakeRequest != null)
                if (_currentHandshakeRequest.RequestedActivationState != State.Set)
                    return;

            WasSetOrUnsetConfirmed = true;

            StopFilterSetUnsetTimeTimer();

            Outputs.Singleton.Off(
                AlarmAreaStateAndSettings.GuidAlarmArea.ToString(),
                _idOutputForSetUnset);

            // should precede ChangeActivationState, as the OnActivationStateChanged evaluation is about to verify this field
            // after successful Set
            _currentHandshakeRequest = null;

            ChangeActivationState(
                State.Set,
                GetSetUnsetParams());



            var lastHandshakeRequest = _lastHandshakeRequest;

            if (lastHandshakeRequest != null)
            {
                _lastHandshakeRequest = null;
                lastHandshakeRequest.EnqueueAsync(_processingQueue);
            }
        }

        private void UnsetAlarmAreaCore()
        {
            if (Activated.State != true)
                return;

            if (_currentHandshakeRequest != null)
                if (_currentHandshakeRequest.RequestedActivationState != State.Unset)
                    return;

            WasSetOrUnsetConfirmed = true;

            StopFilterSetUnsetTimeTimer();

            Outputs.Singleton.Off(
                AlarmAreaStateAndSettings.GuidAlarmArea.ToString(),
                _idOutputForSetUnset);

            _currentHandshakeRequest = null;

            new UnsetAlarmAreaExecutor(
                new DummyAlarmAreaUnsetResult(),
                Guid.Empty,
                Guid.Empty,
                0,
                GetSetUnsetParams()).Execute(this);


            var lastHandshakeRequest = _lastHandshakeRequest;

            if (lastHandshakeRequest != null)
            {
                _lastHandshakeRequest = null;
                lastHandshakeRequest.EnqueueAsync(_processingQueue);
            }
        }

        private AlarmAreas.SetUnsetParams GetSetUnsetParams()
        {
            return _currentHandshakeRequest != null
                ? _currentHandshakeRequest.SetUnsetParams
                : new AlarmAreas.SetUnsetParams(
                    Guid.Empty,
                    new AccessDataBase(),
                    _idActivationInput,
                    false,
                    false);
        }


        protected override WaitableProcessingRequest<ExternalAlarmAreaController> CreateSetAlarmAreaRequestItem(AlarmAreas.SetUnsetParams setUnsetParams)
        {
            return new BeginSetHandshakeRequest(setUnsetParams);
        }

        protected override WaitableProcessingRequest<ExternalAlarmAreaController> CreateUnsetAlarmAreaRequestItem(UnsetAlarmAreaExecutor unsetAlarmAreaExecutor)
        {
            return new BeginUnsetHandshakeRequest(unsetAlarmAreaExecutor);
        }


        public override void SendSabotageStateInfoToServer()
        {
        }


        private bool RequestSetOrUnsetExternAlarmArea(
            ABeginHandshakeRequest beginHandshakeRequest)
        {
            if (_currentHandshakeRequest != null)
            {
                if (_currentHandshakeRequest.RequestedActivationState
                    != beginHandshakeRequest.RequestedActivationState)
                {
                    _lastHandshakeRequest = beginHandshakeRequest;
                    return true;
                }

                _lastHandshakeRequest = null;
                return false;
            }

            if (beginHandshakeRequest.RequestedActivationState == ActivationState)
                return false;

            _currentHandshakeRequest = beginHandshakeRequest;

            WasSetOrUnsetConfirmed = false;

            var idActivationObject = _currentHandshakeRequest.SetUnsetParams.IdActivationObject;

            if (idActivationObject != Guid.Empty)
                SendEventAlarmAreaSetUnsetByObjectForAa(
                    idActivationObject,
                    _currentHandshakeRequest.RequestedActivationState == State.Set);

            Outputs.Singleton.On(
                AlarmAreaStateAndSettings.GuidAlarmArea.ToString(),
                _idOutputForSetUnset);

            _filterTimeSetUnsetTimer =
                NativeTimerManager.StartTimeout(
                    _filterTimeForSetUnset * 1000,
                    OnFilterTimeSetUnsetTimer);

            AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                eventHandler => eventHandler
                    .OnSetUnsetStarted(
                        AlarmAreaStateAndSettings,
                        _currentHandshakeRequest.RequestedActivationState == State.Set));

            return true;
        }

        private void StopFilterSetUnsetTimeTimer()
        {
            lock (_lockFilterTimeSetUnsetTimer)
            {
                if (_filterTimeSetUnsetTimer == null)
                    return;

                _filterTimeSetUnsetTimer.StopTimer();
                _filterTimeSetUnsetTimer = null;
            }
        }

        public bool WasSetOrUnsetConfirmed
        {
            get;
            private set;
        }

        private void OnFilterTimeSetUnsetExpired()
        {
            Outputs.Singleton.Off(
                AlarmAreaStateAndSettings.GuidAlarmArea.ToString(),
                _idOutputForSetUnset);

            Events.ProcessEvent(
                new EventNotConfirmSetUnsetAaFromEis(
                    AlarmAreaStateAndSettings.GuidAlarmArea));

            AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                eventHandler => eventHandler
                    .OnSetUnsetTimeout(
                        AlarmAreaStateAndSettings,
                        _currentHandshakeRequest.RequestedActivationState == State.Set));

            _currentHandshakeRequest = null;
        }

        private bool OnFilterTimeSetUnsetTimer(NativeTimer nativeTimer)
        {
            lock (_lockFilterTimeSetUnsetTimer)
                _filterTimeSetUnsetTimer = null;

            new FilterTimeSetUnsetExpiredRequest().EnqueueAsync(_processingQueue);
            return true;
        }

        public bool IsWaitingForExternalSystem()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool ExternAlarmAreaSettings.IsWaitingForExternalSystem()");

            return _currentHandshakeRequest != null;
        }
    }
}