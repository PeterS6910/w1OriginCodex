using System;
namespace Contal.IwQuick.Net
{
    public interface ISimpleUdpPeer
    {
        int BufferSize { get; set; }
        event DUdpDataEvent DataReceived;
        event DUdpDataEvent DataSent;
        bool IsListening { get; }
        void Send(System.Net.IPEndPoint ipEndpoint, Contal.IwQuick.Data.ByteDataCarrier data);
        System.Net.IPAddress SourceAddress { get; set; }
        void Start();
    }
}
