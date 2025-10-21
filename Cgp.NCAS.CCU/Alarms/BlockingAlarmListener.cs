using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    public abstract class BlockingAlarmListener : IEquatable<BlockingAlarmListener>
    {
        internal class NotBlockedAlarm : BlockingAlarmListener
        {
            public NotBlockedAlarm(
                AlarmType alarmType,
                IdAndObjectType alarmParentObject,
                Action<bool, IdAndObjectType> blockingStateChanged)
                : base(
                    alarmType,
                    alarmParentObject,
                    blockingStateChanged)
            {

            }

            protected override void Initialize()
            {
                BlockingOnOffObjectStateChanged(false);
            }

            public override void Uncofigure()
            {

            }
        }

        internal class InputBlockingAlarmListener : BlockingAlarmListener, IInputChangedListener
        {
            private readonly Guid _idInput;

            public InputBlockingAlarmListener(
                AlarmType alarmType,
                IdAndObjectType alarmParentObject,
                Guid idInput,
                Action<bool, IdAndObjectType> blockingStateChanged)
                : base(
                    alarmType,
                    alarmParentObject,
                    blockingStateChanged)
            {
                _idInput = idInput;
            }

            protected override void Initialize()
            {
                Inputs.Singleton.AddInputChangedListener(
                    _idInput,
                    this);

                BlockingOnOffObjectStateChanged(
                    Inputs.Singleton.GetInputLogicalState(_idInput) == State.Alarm);
            }

            public override void Uncofigure()
            {
                Inputs.Singleton.RemoveInputChangedListener(
                    _idInput,
                    this);
            }

            public bool Equals(IInputChangedListener other)
            {
                var inputBlockedAlarmListener = other as InputBlockingAlarmListener;

                if (inputBlockedAlarmListener == null)
                    return false;

                return base.Equals(inputBlockedAlarmListener);
            }
            
            public void OnInputChanged(Guid guidInput, State state)
            {
                BlockingOnOffObjectStateChanged(
                    state == State.Alarm);
            }
        }

        internal class OutputBlockingAlarmListner : BlockingAlarmListener
        {
            private readonly Guid _idOutput;

            public OutputBlockingAlarmListner(
                AlarmType alarmType,
                IdAndObjectType alarmParentObject,
                Guid idOutput,
                Action<bool, IdAndObjectType> blockingStateChanged)
                : base(
                    alarmType,
                    alarmParentObject,
                    blockingStateChanged)
            {
                _idOutput = idOutput;
            }

            protected override void Initialize()
            {
                Outputs.Singleton.AddOutputChanged(
                    _idOutput,
                    OutputStateChanged);

                BlockingOnOffObjectStateChanged(
                    Outputs.Singleton.GetOutputState(_idOutput) == State.Alarm);
            }

            public override void Uncofigure()
            {
                Outputs.Singleton.RemoveOutputChanged(
                    _idOutput,
                    OutputStateChanged);
            }

            private void OutputStateChanged(
                Guid idOutput,
                State state)
            {
                BlockingOnOffObjectStateChanged(
                    state == State.On);
            }
        }

        internal class TimeZoneBlockingAlarmListener : BlockingAlarmListener
        {
            private readonly Guid _idTimeZone;

            public TimeZoneBlockingAlarmListener(
                AlarmType alarmType,
                IdAndObjectType alarmParentObject,
                Guid idTimeZone,
                Action<bool, IdAndObjectType> blockingStateChanged)
                : base(
                    alarmType,
                    alarmParentObject,
                    blockingStateChanged)
            {
                _idTimeZone = idTimeZone;
            }

            protected override void Initialize()
            {
                TimeZones.Singleton.AddEvent(
                    _idTimeZone,
                    TimeZoneStateChanged);
            }

            public override void Uncofigure()
            {
                TimeZones.Singleton.RemoveEvent(
                    _idTimeZone,
                    TimeZoneStateChanged);
            }

            private void TimeZoneStateChanged(
                Guid idTimeZone,
                State state)
            {
                BlockingOnOffObjectStateChanged(
                    state == State.On);
            }
        }

        internal class DailyPlanBlockingAlarmListener : BlockingAlarmListener
        {
            private readonly Guid _idDailyPlan;

            public DailyPlanBlockingAlarmListener(
                AlarmType alarmType,
                IdAndObjectType alarmParentObject,
                Guid idDailyPlan,
                Action<bool, IdAndObjectType> blockingStateChanged)
                : base(
                    alarmType,
                    alarmParentObject,
                    blockingStateChanged)
            {
                _idDailyPlan = idDailyPlan;
            }

            protected override void Initialize()
            {
                DailyPlans.Singleton.AddEvent(
                    _idDailyPlan,
                    DailyPlanStateChanged);
            }

            public override void Uncofigure()
            {
                DailyPlans.Singleton.RemoveEvent(
                    _idDailyPlan,
                    DailyPlanStateChanged);
            }

            private void DailyPlanStateChanged(
                Guid idDailyPlan,
                State state)
            {
                BlockingOnOffObjectStateChanged(
                    state == State.On);
            }
        }

        private readonly AlarmType _alarmType;
        private readonly IdAndObjectType _alarmParentObject;

        private bool? _isBlocked;
        public bool IsBlocked { get { return _isBlocked != null && _isBlocked.Value; } }

        private readonly Action<bool, IdAndObjectType> _blockingStateChanged;

        protected BlockingAlarmListener(
            AlarmType alarmType,
            IdAndObjectType alarmParentObject,
            Action<bool, IdAndObjectType> blockingStateChanged)
        {
            _alarmType = alarmType;
            _alarmParentObject = alarmParentObject;
            _blockingStateChanged = blockingStateChanged;
        }

        protected void BlockingOnOffObjectStateChanged(bool onOffObjectIsOn)
        {
            if (onOffObjectIsOn == _isBlocked)
                return;

            _isBlocked = onOffObjectIsOn;

            if (_blockingStateChanged != null)
                _blockingStateChanged(
                    onOffObjectIsOn,
                    _alarmParentObject);
        }

        protected abstract void Initialize();

        public abstract void Uncofigure();

        public override int GetHashCode()
        {
            return _alarmParentObject != null
                ? (int)_alarmType ^ _alarmParentObject.GetHashCode()
                : (int) _alarmType;
        }

        public bool Equals(BlockingAlarmListener blockingAlarmListener)
        {
            if (blockingAlarmListener == null)
                return false;

            return _alarmParentObject != null
                ? _alarmType == blockingAlarmListener._alarmType
                  && _alarmParentObject.Equals(blockingAlarmListener._alarmParentObject)
                : _alarmType == blockingAlarmListener._alarmType
                  && blockingAlarmListener._alarmParentObject == null;
        }

        public static BlockingAlarmListener CreateBlockedAlarmListener(
            AlarmType alarmType,
            IdAndObjectType alarmParentObject,
            bool blockedAlarm,
            IdAndObjectType blockingOnOffObject,
            Action<bool, IdAndObjectType> blockingStateChanged)
        {
            BlockingAlarmListener blockingAlarmListener;

            if (!blockedAlarm)
            {
                blockingAlarmListener = new NotBlockedAlarm(
                    alarmType,
                    alarmParentObject,
                    blockingStateChanged);
            }
            else
            {
                switch (blockingOnOffObject.ObjectType)
                {
                    case ObjectType.Input:
                        blockingAlarmListener = new InputBlockingAlarmListener(
                            alarmType,
                            alarmParentObject,
                            (Guid) blockingOnOffObject.Id,
                            blockingStateChanged);
                        break;

                    case ObjectType.Output:
                        blockingAlarmListener = new OutputBlockingAlarmListner(
                            alarmType,
                            alarmParentObject,
                            (Guid) blockingOnOffObject.Id,
                            blockingStateChanged);
                        break;

                    case ObjectType.TimeZone:
                        blockingAlarmListener = new TimeZoneBlockingAlarmListener(
                            alarmType,
                            alarmParentObject,
                            (Guid) blockingOnOffObject.Id,
                            blockingStateChanged);
                        break;

                    case ObjectType.DailyPlan:
                        blockingAlarmListener = new DailyPlanBlockingAlarmListener(
                            alarmType,
                            alarmParentObject,
                            (Guid) blockingOnOffObject.Id,
                            blockingStateChanged);
                        break;

                    default:
                        blockingAlarmListener = new NotBlockedAlarm(
                            alarmType,
                            alarmParentObject,
                            blockingStateChanged);
                        break;
                }
            }

            blockingAlarmListener.Initialize();

            return blockingAlarmListener;
        }

        public static BlockingAlarmListener CreateBlockedAlarmListener(
            AlarmType alarmType,
            bool blockedAlarm,
            IdAndObjectType blockingOnOffObject,
            Action<bool, IdAndObjectType> blockingStateChanged)
        {
            return CreateBlockedAlarmListener(
                alarmType,
                null,
                blockedAlarm,
                blockingOnOffObject,
                blockingStateChanged);
        }
    }
}
