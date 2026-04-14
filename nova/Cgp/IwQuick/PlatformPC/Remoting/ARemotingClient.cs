using System;

namespace Contal.IwQuick.Remoting
{
    public class ARemotingClient:MarshalByRefObject
    {
        /// <summary>
        /// implicitly disallowes lifetime GC mechanism over remoted objects
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
