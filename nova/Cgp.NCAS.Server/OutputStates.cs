using System;

using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Server
{
    public class OutputStates
    {
        private Guid _idOutput;
        private DateTime? _dateTimeLastOutputStateChanged;
        private OutputState _outputState;
        private DateTime? _dateTimeLastOutputRealStateChanged;
        private OutputState _outputRealState;

        public Guid IdOutput { get { return _idOutput; } set { _idOutput = value; } }
        public OutputState OutputState { get { return _outputState; } }
        public OutputState OutputRealState { get { return _outputRealState; } }

        public OutputStates(Guid idOutput, DateTime? dateTimeOutputState, OutputState outputState, DateTime? dateTimeOutputRealState, OutputState outputRealState)
        {
            _idOutput = idOutput;


            _dateTimeLastOutputStateChanged = dateTimeOutputState;
            _outputState = outputState;

            _dateTimeLastOutputRealStateChanged = dateTimeOutputRealState;
            _outputRealState = outputRealState;
        }

        public bool SetOutputState(DateTime? dateTime, OutputState outputState)
        {
            if (dateTime == null || _dateTimeLastOutputStateChanged == null || dateTime >= _dateTimeLastOutputStateChanged)
            {
                _dateTimeLastOutputStateChanged = dateTime;
                _outputState = outputState;

                return true;
            }

            return false;
        }

        public bool SetOutputRealState(DateTime? dateTime, OutputState outputRealState)
        {
            if (dateTime != null &&
                    _dateTimeLastOutputRealStateChanged != null &&
                    dateTime < _dateTimeLastOutputRealStateChanged)
                return false;

            _dateTimeLastOutputRealStateChanged = dateTime;
            _outputRealState = outputRealState;

            return true;
        }
    }
}