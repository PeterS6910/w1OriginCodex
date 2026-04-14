using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(212)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AlarmAreaStates
    {
        private State _activationState;

        private State _alarmState;

        [LwSerialize]
        public State ActivationState
        {
            get { return _activationState; }
            set { _activationState = value; }
        }

        [LwSerialize]
        public State AlarmState
        {
            get { return _alarmState; }
            set { _alarmState = value; }
        }

        public AlarmAreaStates()
        {
            _activationState = State.Unset;
            _alarmState = State.Normal;
        }
    }
}