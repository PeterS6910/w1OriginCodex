using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Contal.IwQuick.Net
{
    [Serializable()]
    public class InvalidIPException : Exception
    {
        public InvalidIPException(string ipAddress)
            :
            base(null == ipAddress ? "" : "Specified IP address \"" + ipAddress + "\" is invalid")
        {
        }

        public InvalidIPException()
            :
            base()
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidIPException(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {

        }
    }

    [Serializable()]
    public class InvalidGatewayIPException : Exception
    {
        public InvalidGatewayIPException(string ipAddress)
            :
            base(null == ipAddress ? "" : "Specified gateway IP address \"" + ipAddress + "\" is invalid")
        {
        }

        public InvalidGatewayIPException()
            :
            base()
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidGatewayIPException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }


    [Serializable()]
    public class InvalidSubnetMaskException : Exception
    {
        public InvalidSubnetMaskException(string subnetMask)
            :
            base(null == subnetMask ? "" : "Specified IP SubnetMask \"" + subnetMask + "\" is invalid")
        {
        }

        public InvalidSubnetMaskException()
            :
            base()
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidSubnetMaskException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

    

    public class InvalidHostnameException : Exception
    {
        public InvalidHostnameException(string hostName)
            :
            base(null == hostName ? "" : "Specified hostname \"" + hostName + "\" is invalid.\n"+
                "It can contain only alphanumeric characters and dots.")
        {
        }

        public InvalidHostnameException()
            :
            base()
        {
        }
    }

    public class DnsLookupException : Exception
    {
        public DnsLookupException(string hostName)
            :
            base(null == hostName ? "" : "Dns lookup for \"" + hostName + "\" hostname failed")
        {
        }

        public DnsLookupException()
            :
            base()
        {
        }
    }

    public class InvalidTransportPortException : Exception
    {
        public InvalidTransportPortException(int port)
            :
            base("Specified port \"" + port + "\" is invalid.\n" +
                "Port must be in the range of 1-65535 or zero in specific cases")
        {
        }
    }

    public class LwDhcpException : Exception
    {
        public LwDhcpException(string message)
            :
            base(message)
        {
        }
    }
}
