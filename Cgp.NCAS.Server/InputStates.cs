using System;

using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Server
{
    public class InputStates
    {
        private Guid _idInput;
        private DateTime? _dateTimeLastInputStateChanged;
        private InputState _inputState;

        public Guid IdInput { get { return _idInput; } set { _idInput = value; } }
        public InputState InputState { get { return _inputState; } }

        public InputStates(Guid idInput, DateTime? dateTime, InputState inputState)
        {
            _idInput = idInput;
            _dateTimeLastInputStateChanged = dateTime;
            _inputState = inputState;
        }

        public bool SetInputState(DateTime? dateTime, InputState inputState)
        {
            if (dateTime == null || _dateTimeLastInputStateChanged == null || dateTime >= _dateTimeLastInputStateChanged)
            {
                _dateTimeLastInputStateChanged = dateTime;
                _inputState = inputState;

                return true;
            }

            return false;
        }
    }
}