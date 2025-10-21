using System;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// abstract class for parameters of any authentication process
    /// </summary>
    [Serializable]
    public abstract class ARemotingAuthenticationParameters
    {
        public abstract void Validate();
    }
}
