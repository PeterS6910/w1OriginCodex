using System;
//using System.Runtime.Serialization;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// exception thrown, if the specified IP is invalid
    /// </summary>
    [Serializable]
    public class InvalidIPException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        public InvalidIPException(string ipAddress)
            :
            base(null == ipAddress ? "" : "Specified IP address \"" + ipAddress + "\" is invalid")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InvalidIPException()
        {
        }

        /*
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidIPException(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {

        }*/
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InvalidGatewayIPException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        public InvalidGatewayIPException(string ipAddress)
            :
            base(null == ipAddress ? "" : "Specified gateway IP address \"" + ipAddress + "\" is invalid")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InvalidGatewayIPException()
        {
        }

        /*
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidGatewayIPException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }*/
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InvalidSubnetMaskException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subnetMask"></param>
        public InvalidSubnetMaskException(string subnetMask)
            :
            base(null == subnetMask ? "" : "Specified IP SubnetMask \"" + subnetMask + "\" is invalid")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InvalidSubnetMaskException()
        {
        }

        /*
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidSubnetMaskException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }*/
    }

    
    /// <summary>
    /// 
    /// </summary>
    public class InvalidHostnameException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        public InvalidHostnameException(string hostName)
            :
            base(null == hostName ? "" : "Specified hostname \"" + hostName + "\" is invalid.\n"+
                "It can contain only alphanumeric characters and dots.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InvalidHostnameException()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DnsLookupException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        public DnsLookupException(string hostName)
            :
            base(null == hostName ? "" : "Dns lookup for \"" + hostName + "\" hostname failed")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DnsLookupException()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalidTransportPortException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        public InvalidTransportPortException(int port)
            :
            base("Specified port \""+port+"\" is invalid.\n"+
                "Port must be in the range of 1-65535 or zero in specific cases")
        {
        }
    }
}
