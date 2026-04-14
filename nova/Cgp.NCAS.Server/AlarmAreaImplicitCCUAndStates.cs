using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server
{
    public class AlarmAreaImplicitCCUAndStates
    {
        private class SensorBlocking
        {
            public SensorBlockingType BlockingType { get; private set; }
            private DateTime _dateTime;

            public SensorBlocking(
                SensorBlockingType blockingType,
                DateTime dateTime)
            {
                BlockingType = blockingType;
                _dateTime = dateTime;
            }

            public bool SetBlockingType(
                SensorBlockingType blockingType,
                DateTime dateTime)
            {
                if (dateTime < _dateTime)
                    return false;

                _dateTime = dateTime;
                BlockingType = blockingType;
                return true;
            }
        }

        private class SensorState
        {
            public State State { get; private set; }
            private DateTime _dateTime;

            public SensorState(
                State state,
                DateTime dateTime)
            {
                State = state;
                _dateTime = dateTime;
            }

            public bool SetState(
                State state,
                DateTime dateTime)
            {
                if (dateTime < _dateTime)
                    return false;

                _dateTime = dateTime;
                State = state;
                return true;
            }
        }

        private readonly Guid _idAlarmArea;
        private readonly Guid _guidCCU;
        private DateTime? _dateTimeLastAlarmAreaActivationStateChanged;
        private ActivationState _activationState;
        private DateTime? _dateTimeLastAlarmAreaActualStateChanged;
        private AlarmAreaAlarmState _alarmState;
        private RequestActivationState _reguestActivationState = RequestActivationState.Unknown;
        private DateTime? _dateTimeLastAlarmAreaSabotageStateChanged;
        private State _sabotageState;
        private readonly object _lockReguestActivationState = new object();

        public Guid GuidCCU { get { return _guidCCU; } }
        public ActivationState ActivationState { get { return _activationState; } }
        public RequestActivationState RequestActivationState { get { return _reguestActivationState; } }
        public AlarmAreaAlarmState AlarmState { get { return _alarmState; } }

        private SyncDictionary<Guid, SensorBlocking> _sensorsBlockingType = new SyncDictionary<Guid, SensorBlocking>();
        private SyncDictionary<Guid, SensorState> _sensorsState = new SyncDictionary<Guid, SensorState>();

        public State SabotageState
        {
            get { return _sabotageState; }
        }

        public AlarmAreaImplicitCCUAndStates(
            Guid idAlarmArea, 
            Guid guidCCU, 
            DateTime? dateTime, 
            ActivationState activationState, 
            AlarmAreaAlarmState actualState)
        {
            _idAlarmArea = idAlarmArea;
            _guidCCU = guidCCU;
            _dateTimeLastAlarmAreaActivationStateChanged = dateTime;
            _activationState = activationState;
            _dateTimeLastAlarmAreaActualStateChanged = dateTime;
            _alarmState = actualState;
        }

        public void SetAlarmAreaActivationState(
            DateTime dateTime,
            ActivationState activationState)
        {
            lock (_lockReguestActivationState)
            {
                if (_dateTimeLastAlarmAreaActivationStateChanged == null || dateTime >= _dateTimeLastAlarmAreaActivationStateChanged)
                {
                    SetAlarmAreaActivationStateCore(
                        dateTime,
                        activationState);
                }
            }
        }

        private void SetAlarmAreaActivationStateCore(
            DateTime? dateTime,
            ActivationState activationState)
        {
            _dateTimeLastAlarmAreaActivationStateChanged = dateTime;
            _activationState = activationState;

            if ((_reguestActivationState != RequestActivationState.Unknown && activationState != ActivationState.Unknown))
            {
                if ((_reguestActivationState == RequestActivationState.Unset && activationState == ActivationState.Unset) ||
                    ((_reguestActivationState == RequestActivationState.Set || _reguestActivationState == RequestActivationState.UnconditionalSet) &&
                     activationState != ActivationState.Unset))
                {
                    _reguestActivationState = RequestActivationState.Unknown;
                    CCUConfigurationHandler.Singleton.AlarmAreaRequestActivationStateChanged(_idAlarmArea, _reguestActivationState, false);
                }
            }

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaActivationStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                        {
                            _idAlarmArea,
                            (byte) activationState
                        });
        }

        public void SetUnsetNotConfirm(DateTime dateTime)
        {
            lock (_lockReguestActivationState)
            {
                if (_dateTimeLastAlarmAreaActivationStateChanged == null || dateTime >= _dateTimeLastAlarmAreaActivationStateChanged)
                {
                    if (_reguestActivationState != RequestActivationState.Unknown)
                    {
                        _reguestActivationState = RequestActivationState.Unknown;
                        CCUConfigurationHandler.Singleton.AlarmAreaRequestActivationStateChanged(_idAlarmArea, _reguestActivationState, true);
                    }
                }
            }
        }

        public void SetAlarmAreaAlarmState(
            DateTime dateTime,
            AlarmAreaAlarmState alarmState)
        {
            if (_dateTimeLastAlarmAreaActualStateChanged == null || dateTime >= _dateTimeLastAlarmAreaActualStateChanged)
            {
                SetAlarmAreaAlarmStateCore(
                    dateTime,
                    alarmState);
            }
        }

        private void SetAlarmAreaAlarmStateCore(
            DateTime? dateTime,
            AlarmAreaAlarmState alarmState)
        {
            _dateTimeLastAlarmAreaActualStateChanged = dateTime;
            _alarmState = alarmState;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaAlarmStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                    {
                        _idAlarmArea,
                        (byte) alarmState
                    });
        }

        public void SetAlarmAreaSabotageState(
            DateTime dateTime,
            State sabotageState)
        {
            if (_dateTimeLastAlarmAreaSabotageStateChanged == null || dateTime >= _dateTimeLastAlarmAreaSabotageStateChanged)
            {
                SetAlarmAreaSabotageStateCore(
                    dateTime,
                    sabotageState);
            }
        }

        private void SetAlarmAreaSabotageStateCore(
            DateTime? dateTime,
            State sabotageState)
        {
            _dateTimeLastAlarmAreaSabotageStateChanged = dateTime;
            _sabotageState = sabotageState;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaSabotageStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                    {
                        _idAlarmArea,
                        (byte) sabotageState
                    });
        }

        public void SetAlarmAreaRequestActivationState(DateTime dateTime, RequestActivationState requestActivationState)
        {
            if (_dateTimeLastAlarmAreaActualStateChanged == null || dateTime >= _dateTimeLastAlarmAreaActualStateChanged)
            {
                SetAlarmAreaRequestActivationState(requestActivationState);
            }
        }

        public void SetAlarmAreaRequestActivationState(RequestActivationState requestActivationState)
        {
            lock (_lockReguestActivationState)
            {
                if (requestActivationState != RequestActivationState.Unknown && _activationState != ActivationState.Unknown)
                {
                    if ((requestActivationState == RequestActivationState.Unset && _activationState != ActivationState.Unset) ||
                        ((requestActivationState == RequestActivationState.Set || requestActivationState == RequestActivationState.UnconditionalSet) &&
                         _activationState == ActivationState.Unset))
                    {
                        _reguestActivationState = requestActivationState;
                        CCUConfigurationHandler.Singleton.AlarmAreaRequestActivationStateChanged(_idAlarmArea, _reguestActivationState, false);
                    }
                }
                else
                {
                    _reguestActivationState = requestActivationState;
                    CCUConfigurationHandler.Singleton.AlarmAreaRequestActivationStateChanged(_idAlarmArea, _reguestActivationState, false);
                }
            }
        }

        public SensorBlockingType? GetSensorBlockingType(Guid idInput)
        {
            SensorBlocking sensorBlocking;

            if (!_sensorsBlockingType.TryGetValue(
                idInput,
                out sensorBlocking))
            {
                return null;
            }

            return sensorBlocking.BlockingType;
        }

        public void SetSensorBlockingType(
            Guid idInput,
            SensorBlockingType sensorBlockingType,
            DateTime dateTime)
        {
            SensorBlocking sensorBlocking;

            if (!_sensorsBlockingType.GetOrAddValue(
                idInput,
                out sensorBlocking,
                new SensorBlocking(
                    sensorBlockingType,
                    dateTime)))
            {
                if (!sensorBlocking.SetBlockingType(
                    sensorBlockingType,
                    dateTime))
                {
                    return;
                }
            }

            SensorBlockingTypeChangedForeachCallbackHandler(
                idInput,
                sensorBlockingType);
        }

        private void SensorBlockingTypeChangedForeachCallbackHandler(
            Guid idInput,
            SensorBlockingType? sensorBlockingType)
        {
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaSensorBlockingTypeChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    _idAlarmArea,
                    idInput,
                    sensorBlockingType
                });
        }

        public State? GetSensorState(Guid idInput)
        {
            SensorState sensorState;

            if (!_sensorsState.TryGetValue(
                idInput,
                out sensorState))
            {
                return null;
            }

            return sensorState.State;
        }

        public void SetSensorState(
            Guid idInput,
            State state,
            DateTime dateTime)
        {
            SensorState sensorState;

            if (!_sensorsState.GetOrAddValue(
                idInput,
                out sensorState,
                new SensorState(
                    state,
                    dateTime)))
            {
                if (!sensorState.SetState(
                    state,
                    dateTime))
                {
                    return;
                }
            }

            SensorStateChangedForeachCallbackHandler(
                idInput,
                state);
        }

        private void SensorStateChangedForeachCallbackHandler(
            Guid idInput,
            State? state)
        {
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaSensorStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    _idAlarmArea,
                    idInput,
                    state
                });
        }

        public void ClearStates()
        {
            SetAlarmAreaAlarmStateCore(
                null,
                AlarmAreaAlarmState.Unknown);

            SetAlarmAreaActivationStateCore(
                null,
                ActivationState.Unknown);

            SetAlarmAreaSabotageStateCore(
                null,
                State.Unknown);

            SetAlarmAreaRequestActivationState(RequestActivationState.Unknown);

            _sensorsBlockingType.Clear(
                (key, value) => SensorBlockingTypeChangedForeachCallbackHandler(
                    key,
                    null),
                null);

            _sensorsState.Clear(
                (key, value) => SensorStateChangedForeachCallbackHandler(
                    key,
                    null),
                null);
        }
    }
}