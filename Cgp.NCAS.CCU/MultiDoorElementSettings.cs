using System;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.DoorStateMachine;

using CardReader = Contal.Cgp.NCAS.CCU.DB.CardReader;

namespace Contal.Cgp.NCAS.CCU
{
    public class MultiDoorElementSettings
    {
        private abstract class InputChangedListener : IInputChangedListener
        {
            protected readonly MultiDoorElementSettings _multiDoorElementSettings;

            public Guid GuidMultiDoorElement
            {
                get;
                private set;
            }

            protected InputChangedListener(MultiDoorElementSettings multiDoorElementSettings)
            {
                _multiDoorElementSettings = multiDoorElementSettings;

                GuidMultiDoorElement = 
                    _multiDoorElementSettings._multiDoorElement.IdMultiDoorElement;
            }

            public abstract bool Equals(IInputChangedListener other);

            public abstract void OnInputChanged(
                Guid guidInput,
                State state);
        }

        private class SensorOpenDoorChangedListener : InputChangedListener
        {
            public SensorOpenDoorChangedListener(MultiDoorElementSettings multiDoorElementSettings)
                : base(multiDoorElementSettings)
            {
            }

            public override bool Equals(IInputChangedListener other)
            {
                var x = other as InputChangedListener;

                return x != null
                       && GuidMultiDoorElement.Equals(x.GuidMultiDoorElement);
            }

            public override void OnInputChanged(
                Guid guidInput,
                State state)
            {
                _multiDoorElementSettings.SensorOpenDoorChanged(
                    guidInput,
                    state);
            }
        }

        private class BlockedByInputChangeListener : InputChangedListener
        {
            public BlockedByInputChangeListener(MultiDoorElementSettings multiDoorElementSettings)
                : base(multiDoorElementSettings)
            {
            }

            public override bool Equals(IInputChangedListener other)
            {
                var otherListener = other as BlockedByInputChangeListener;

                return
                    otherListener != null
                    && GuidMultiDoorElement.Equals(otherListener.GuidMultiDoorElement);
            }

            public override void OnInputChanged(
                Guid guidInput,
                State state)
            {
                _multiDoorElementSettings.BlockOnOffObjectStateChanged(
                    guidInput,
                    state);
            }
        }

        private class AlarmAreaEventHandler : IAlarmAreaActivationEventHandler
        {
            private readonly MultiDoorElementSettings _multiDoorElementSettings;

            public AlarmAreaEventHandler(MultiDoorElementSettings multiDoorElementSettings)
            {
                _multiDoorElementSettings = multiDoorElementSettings;
            }

            public void OnActivationStateChanged(State activationState)
            {
                _multiDoorElementSettings.BlockAlarmAreaActivationStateChanged(activationState);
            }

            public bool Equals(IAlarmAreaActivationEventHandler other)
            {
                return ReferenceEquals(
                    this,
                    other);
            }

            public override int GetHashCode()
            {
                return _multiDoorElementSettings._idMultiDoorElement.GetHashCode();
            }
        }

        private class DoorStateMachineEventHandler : IDoorStateMachineEventHandler
        {
            private readonly MultiDoorElementSettings _multiDoorElementSettings;

            private bool _isActive;

            public DoorStateMachineEventHandler(MultiDoorElementSettings multiDoorElementSettings)
            {
                _multiDoorElementSettings = multiDoorElementSettings;
            }

            public bool IsActive
            {
                set { _isActive = value; }
            }

            void IDoorStateMachineEventHandler.UnlockStateChanged(bool isUnlocked, DoorEnvironmentAccessTrigger dsmAccessTrigger)
            {
                if (_isActive)
                    _multiDoorElementSettings.UnlockStateChanged(isUnlocked);
            }

            void IDoorStateMachineEventHandler.IntrusionStateChanged(bool isInIntrusion)
            {
            }

            void IDoorStateMachineEventHandler.SabotageStateChanged(bool isInSabotage)
            {
            }

            void IDoorStateMachineEventHandler.AjarStateChanged(bool isInAjar)
            {
            }

            void IDoorStateMachineEventHandler.BypassAlarmStateChanged(bool isBypassActivated)
            {
            }

            void IDoorStateMachineEventHandler.DSMStateChanged(DsmStateInfo dsmStateInfo)
            {
                if (_isActive)
                    _multiDoorElementSettings.DsmStateChanged(dsmStateInfo.DsmState);
            }
        }

        private readonly Guid _idMultiDoorElement;

        private MultiDoorElement _multiDoorElement;

        private DoorEnvironmentState _dsmState = 
            DoorEnvironmentState.Unknown;

        private bool _blockOnOffObjectIsOn;
        private bool _blockAlarmAreaIsSet;

        private readonly IDoorStateMachine _doorStateMachine;
        private IInputChangedListener _sensorOpenDoorsChanged;

        private readonly DoorStateMachineEventHandler _doorStateMachineEventHandler;

        private IInputChangedListener _blockedByInputChangedListener;

        private AlarmAreaEventHandler _alarmAreaEventHandler;

        public MultiDoorElementSettings(MultiDoorElement multiDoorElement)
        {
            _multiDoorElement = multiDoorElement;
            _idMultiDoorElement = multiDoorElement.IdMultiDoorElement;

            _doorStateMachineEventHandler = new DoorStateMachineEventHandler(this);

            _doorStateMachine =
                DefaultDsmFactory.Singleton.Create(_doorStateMachineEventHandler);

            MultiDoors.Singleton.OnMultiDoorElementAdded(
                _multiDoorElement.MultiDoorId,
                _multiDoorElement.IdMultiDoorElement);
        }

        public bool IsUnlocked
        {
            get
            {
                return !_doorStateMachine.DoorLockedState;
            }
        }

        private void UnlockStateChanged(bool isUnlocked)
        {
            if (isUnlocked)
            {
                Outputs.Singleton.On(
                    _multiDoorElement.IdMultiDoorElement.ToString(),
                    _multiDoorElement.ElectricStrikeId);

                if (_multiDoorElement.ExtraElectricStrikeId.HasValue)
                    Outputs.Singleton.On(
                        _multiDoorElement.IdMultiDoorElement.ToString(),
                        _multiDoorElement.ExtraElectricStrikeId.Value);
            }
            else
            {
                Outputs.Singleton.Off(
                    _multiDoorElement.IdMultiDoorElement.ToString(),
                    _multiDoorElement.ElectricStrikeId);

                if (_multiDoorElement.ExtraElectricStrikeId.HasValue)
                    Outputs.Singleton.Off(
                        _multiDoorElement.IdMultiDoorElement.ToString(),
                        _multiDoorElement.ExtraElectricStrikeId.Value);
            }
        }

        public void ApplyChanges(MultiDoorElement multiDoorElement)
        {
            _multiDoorElement = multiDoorElement;
        }

        public void Configure()
        {
            if (_multiDoorElement.ElectricStrikeId == Guid.Empty)
            {
                DsmStateChanged(DoorEnvironmentState.Unknown);
                return;
            }

            var multiDoor =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.MultiDoor,
                    _multiDoorElement.MultiDoorId) as MultiDoor;

            if (multiDoor == null)
                return;

            _doorStateMachine.SetTimmings(
                (uint)multiDoor.DoorTimeUnlock * 1000,
                (uint)multiDoor.DoorTimeOpen * 1000,
                (uint)multiDoor.DoorTimePreAlarm * 1000,
                0,
                0);

            AttachEvents();

            SetForceUnlocked(MultiDoors.Singleton.IsForceUnlocked(_multiDoorElement.MultiDoorId));
        }

        private void AttachEvents()
        {
            var sensorOpenDoorId = _multiDoorElement.SensorOpenDoorId;

            if (sensorOpenDoorId != null)
            {
                _sensorOpenDoorsChanged = new SensorOpenDoorChangedListener(this);

                Inputs.Singleton.AddInputChangedListener(
                    sensorOpenDoorId.Value,
                    _sensorOpenDoorsChanged);
            }

            _doorStateMachineEventHandler.IsActive = true;

            _doorStateMachine.StartDSM(DoorEnviromentType.Standard);

            if (_multiDoorElement.BlockOnOffObjectType != null &&
                _multiDoorElement.BlockOnOffObjectId != null)
            {
                switch (_multiDoorElement.BlockOnOffObjectType)
                {
                    case ObjectType.DailyPlan:

                        DailyPlans.Singleton.AddEvent(
                            _multiDoorElement.BlockOnOffObjectId.Value,
                            BlockOnOffObjectStateChanged);

                        break;

                    case ObjectType.TimeZone:

                        TimeZones.Singleton.AddEvent(
                            _multiDoorElement.BlockOnOffObjectId.Value,
                            BlockOnOffObjectStateChanged);

                        break;

                    case ObjectType.Output:

                        Outputs.Singleton.AddOutputChanged(
                            _multiDoorElement.BlockOnOffObjectId.Value,
                            BlockOnOffObjectStateChanged);

                        _blockOnOffObjectIsOn =
                            Outputs.Singleton.GetOutputState(_multiDoorElement.BlockOnOffObjectId.Value)
                            == State.On;

                        break;

                    case ObjectType.Input:

                        _blockedByInputChangedListener = new BlockedByInputChangeListener(this);

                        Inputs.Singleton.AddInputChangedListener(
                            _multiDoorElement.BlockOnOffObjectId.Value,
                            _blockedByInputChangedListener);

                        _blockOnOffObjectIsOn =
                            Inputs.Singleton.GetInputLogicalState(_multiDoorElement.BlockOnOffObjectId.Value) == State.Alarm;

                        break;
                }
            }

            if (_multiDoorElement.BlockAlarmAreaId != null)
            {
                _alarmAreaEventHandler = new AlarmAreaEventHandler(this);

                AlarmAreas.Singleton.AddAlarmAreaActivationEventHandler(
                    _multiDoorElement.BlockAlarmAreaId.Value,
                    _alarmAreaEventHandler);

                _blockAlarmAreaIsSet =
                    AlarmAreas.Singleton.AlarmAreaIsSet(_multiDoorElement.BlockAlarmAreaId.Value);
            }
        }

        private void BlockOnOffObjectStateChanged(Guid onOffObjectId, State state)
        {
            _blockOnOffObjectIsOn = 
                state == State.Alarm || 
                state == State.On;

            BlockObjectStateChanged();
        }

        private void BlockAlarmAreaActivationStateChanged(State state)
        {
            _blockAlarmAreaIsSet = AlarmAreas.AlarmAreaIsSet(state);

            BlockObjectStateChanged();
        }

        private void SensorOpenDoorChanged(
            Guid guidInput,
            State state)
        {
            _doorStateMachine.SetDoorSensorState(DoorEnvironmentSettings.GetDSMSensorState(state));
        }

        private void DsmStateChanged(DoorEnvironmentState dsmState)
        {
            _dsmState = dsmState;

            DoorEnvironments.SendDoorEnvironmentState(
                dsmState,
                _multiDoorElement.IdMultiDoorElement);
        }

        public void SendDsmState()
        {
            DoorEnvironments.SendDoorEnvironmentState(
                _dsmState, 
                _multiDoorElement.IdMultiDoorElement);
        }

        public void Unconfigure()
        {
            if (!_doorStateMachine.IsRunning)
                return;

            _doorStateMachine.SetForceUnlocked(false);
            _doorStateMachine.StopDSM();

            DetachEvents();

            _blockOnOffObjectIsOn = false;

            _blockAlarmAreaIsSet = false;
        }

        private void DetachEvents()
        {
            var sensorOpenDoorId = _multiDoorElement.SensorOpenDoorId;

            if (sensorOpenDoorId != null && _sensorOpenDoorsChanged != null)
            {
                Inputs.Singleton.RemoveInputChangedListener(
                    sensorOpenDoorId.Value,
                    _sensorOpenDoorsChanged);

                _sensorOpenDoorsChanged = null;
            }

            _doorStateMachineEventHandler.IsActive = false;

            var blockOnOffObjectType = _multiDoorElement.BlockOnOffObjectType;
            var blockOnOffObjectId = _multiDoorElement.BlockOnOffObjectId;

            if (blockOnOffObjectType != null && blockOnOffObjectId != null)
            {
                switch (blockOnOffObjectType)
                {
                    case ObjectType.DailyPlan:

                        DailyPlans.Singleton.RemoveEvent(
                            blockOnOffObjectId.Value,
                            BlockOnOffObjectStateChanged);

                        break;

                    case ObjectType.TimeZone:

                        TimeZones.Singleton.RemoveEvent(
                            blockOnOffObjectId.Value,
                            BlockOnOffObjectStateChanged);

                        break;

                    case ObjectType.Output:

                        Outputs.Singleton.RemoveOutputChanged(
                            blockOnOffObjectId.Value,
                            BlockOnOffObjectStateChanged);

                        break;

                    case ObjectType.Input:

                        if (_blockedByInputChangedListener != null)
                        {
                            Inputs.Singleton.RemoveInputChangedListener(
                                blockOnOffObjectId.Value,
                                _blockedByInputChangedListener);

                            _blockedByInputChangedListener = null;
                        }

                        break;
                }
            }

            if (_multiDoorElement.BlockAlarmAreaId != null
                && _alarmAreaEventHandler != null)
            {
                AlarmAreas.Singleton.RemoveAlarmAreaActivationEventHandler(
                    _multiDoorElement.BlockAlarmAreaId.Value,
                    _alarmAreaEventHandler);

                _alarmAreaEventHandler = null;
            }
        }

        public bool OnAccessGranted(
            Guid guidCardReader,
            AccessDataBase accessData,
            int crAddress)
        {
            if (!_doorStateMachine.IsRunning)
                return false;

            if (_blockOnOffObjectIsOn ||
                _blockAlarmAreaIsSet)
            {
                return false;
            }

            Events.ProcessEvent(
                new EventDsmAccessPermitted(
                    _multiDoorElement.IdMultiDoorElement,
                    guidCardReader,
                    accessData));

            AlarmsManager.Singleton.AddAlarm(
                new CrAccessPermittedAlarm(
                    guidCardReader,
                    accessData));

            _doorStateMachine.SignalAccessGranted(
                DoorEnvironmentSettings.GetAgSeverity(accessData.AccessGrantedSource),
                DoorEnvironmentAccessTrigger.CardReader);

            return true;
        }

        public void OnRemoved()
        {
            Unconfigure();

            _doorStateMachine.Dispose();

            MultiDoors.Singleton.OnMultiDoorElementRemoved(
                _multiDoorElement.MultiDoorId,
                _multiDoorElement.IdMultiDoorElement);
        }

        public void SetForceUnlocked(bool isForceUnlocked)
        {
            if (!_doorStateMachine.IsRunning ||
                _blockOnOffObjectIsOn ||
                _blockAlarmAreaIsSet)
            {
                return;
            }

            _doorStateMachine.SetForceUnlocked(isForceUnlocked);
        }

        private void BlockObjectStateChanged()
        {
            if (!MultiDoors.Singleton.IsForceUnlocked(_multiDoorElement.IdMultiDoorElement))
                return;

            _doorStateMachine.SetForceUnlocked(
                !(_blockOnOffObjectIsOn || _blockAlarmAreaIsSet));
        }

        public void OnTimingsChanged(MultiDoor multiDoor)
        {
            if (!_doorStateMachine.IsRunning)
                return;

            _doorStateMachine.SetTimmings(
                (uint)multiDoor.DoorTimeUnlock * 1000,
                (uint)multiDoor.DoorTimeOpen * 1000,
                (uint)multiDoor.DoorTimePreAlarm * 1000,
                0,
                0);
        }
    }
}
