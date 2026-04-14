using System;

using Contal.Cgp.NCAS.Definitions;

namespace Contal.Cgp.NCAS.Server
{
    public class CCUDoorEnvironemntState
    {
        private DateTime? _dateTimeLastChangeDoorEnvironmentState;
        private DoorEnvironmentState _doorEnvironmentState;

        public DoorEnvironmentState DoorEnvironmentState { get { return _doorEnvironmentState; } }

        public CCUDoorEnvironemntState(DateTime? dateTime, DoorEnvironmentState doorEnvironmentState)
        {
            _dateTimeLastChangeDoorEnvironmentState = dateTime;
            _doorEnvironmentState = doorEnvironmentState;
        }

        public bool SetDoorEnvironmentState(DateTime? dateTime, DoorEnvironmentState doorEnvironmentState)
        {
            if (dateTime == null || _dateTimeLastChangeDoorEnvironmentState == null || dateTime >= _dateTimeLastChangeDoorEnvironmentState)
            {
                _dateTimeLastChangeDoorEnvironmentState = dateTime;
                _doorEnvironmentState = doorEnvironmentState;

                return true;
            }

            return false;
        }
    }
}