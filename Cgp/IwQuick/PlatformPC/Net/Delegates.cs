using System;

using System.Net;

namespace Contal.IwQuick.Net
{
    public delegate void DIpResolved(string hostname,IPAddress[] ipList);

    public delegate void DTcpConnectEvent(ISimpleTcpConnection connection);
    public delegate void DTcpConnectFailureEvent(IPEndPoint ipEndpoint, Exception connection);

    public delegate void DTcpDataEvent(ISimpleTcpConnection connection, Data.ByteDataCarrier data);


    public delegate void DUdpDataEvent(ISimpleUdpPeer udpPeer, IPEndPoint ipEndpoint, Data.ByteDataCarrier data);

    public delegate void DCNMPLookupFinished(CNMPLookupType lookupType, string value, CNMPLookupResultList result);

    public delegate void DSerialDataReceived(
        ISimpleSerialPort peer, 
        Data.ByteDataCarrier data, 
        int optionalDataLength,
        int timeStamp);
   
}
