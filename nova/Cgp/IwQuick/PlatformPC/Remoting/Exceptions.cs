using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Contal.IwQuick.Remoting
{
    [Serializable]
    public class SessionNotAuthenticatedException : Exception
    {
        public SessionNotAuthenticatedException() :
            base("Session not authenticated")
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public SessionNotAuthenticatedException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }
    }

    [Serializable]
    public class ChannelPreparationException : Exception
    {
        private string _channel;

        public ChannelPreparationException(string channel,Exception innerException) :
            base("Channel \""+(channel??String.Empty)+"\" preparation failed ",innerException)
        {
            _channel = channel ?? String.Empty;
        }
    }
}
