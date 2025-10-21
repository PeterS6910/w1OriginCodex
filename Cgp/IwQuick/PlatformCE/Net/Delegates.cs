using System;
using System.Net;

namespace Contal.IwQuick.Net
{
    public delegate void DUdpEventDataRaw(IPEndPoint client, Contal.IwQuick.Data.ByteDataCarrier data);
    public delegate void DUdpEventGeneral(IPEndPoint client);
    public delegate void DUdpEventFailure(IPEndPoint client, Exception exception);

    
    public delegate void DIpEventResolve(string hostname,IPAddress[] i_arIPs);


    public delegate void DUdpDataEvent(ISimpleUdpPeer udpPeer, IPEndPoint iPEndpoint, Contal.IwQuick.Data.ByteDataCarrier data);

    public delegate void DCNMPLookupFinished(CNMPLookupType lookupType, string value, CNMPLookupResultList result);

    public delegate void DSerialDataReceived(
        ISimpleSerialPort peer, 
        Data.ByteDataCarrier data, 
        int optionalDataLength,
        int timeStamp);

    /// SSL delegates
    /// 
    public delegate int DRemoteCertificateValidation(uint type, string host, uint chainLength, IntPtr certificateChain, uint flags);

    public delegate void DSocketBlockingOperation(ref System.Net.Sockets.Socket socket,System.Threading.ManualResetEvent mutex);


    /*
    public delegate void DUdpEventGeneral(IPEndPoint client);
    public delegate void DUdpEventFailure(IPEndPoint client, Exception exception);*/
}
