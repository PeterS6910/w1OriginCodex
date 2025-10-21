using System;

namespace Contal.IwQuick.Sys
{
    public class EnvironmentVariable
    {
        protected string _buffer;
        protected SystemAccessLevel _level = SystemAccessLevel.Process;
        protected string _varName;

        public EnvironmentVariable(String variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentNullException("variableName");

            _varName = variableName;

            Load();
        }

        public EnvironmentVariable(String variableName, SystemAccessLevel accessLevel)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentNullException("variableName");

            _varName = variableName;
            _level = accessLevel;

            Load();
        }

        protected void Load()
        {
            _buffer = Environment.GetEnvironmentVariable(_varName,
                (EnvironmentVariableTarget)_level);
        }

        public void SetAccessLevel(SystemAccessLevel input)
        {
            _level = input;
            Load();
        }

        protected void Dump()
        {
            Environment.SetEnvironmentVariable(_varName, _buffer,
                (EnvironmentVariableTarget)_level);
        }

        public string GetVariable()
        {
            return _buffer;
        }

        public string Variable
        {
            get
            {
                return GetVariable();
            }
        }

        public override string ToString()
        {
            return GetVariable();
        }

    }
}
