using System.Collections.Generic;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// session information for any remoting client
    /// </summary>
    public class RemotingSession
    {
        public const string SESSIONPARAMETERS = "Parameters";

        public RemotingSession(object identification)
        {

            _identification = identification;
        }

        private bool _authenticated;
        /// <summary>
        /// returns, whether the session is considered autheticated
        /// </summary>
        public bool Authenticated
        {
            get { return _authenticated; }
            protected internal set
            {
                _authenticated = value;
            }
        }

        private volatile Dictionary<string, object> _values = new Dictionary<string, object>();
        /// <summary>
        /// returns variable's value or null if invalid variable name;
        /// </summary>
        /// <param name="variableName">name of the variable</param>
        public object this[string variableName]
        {
            get
            {
                if (Validator.IsNullString(variableName))
                    return null;

                object ret;
                lock (_values)
                {
                    _values.TryGetValue(variableName, out ret);
                }
                return ret;                
            }
            set
            {
                Validator.CheckNullString(variableName);

                lock (_values)
                {
                    _values[variableName] = value;
                }
            }
        }

        private readonly object _identification;
        public object Identification
        {
            get { return _identification; }
        }

        protected internal void Clear()
        {
            lock (_values)
            {
                _values.Clear();
            }
        }
    }
}
