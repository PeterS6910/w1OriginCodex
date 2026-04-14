using System;
using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISimpleTcpConnection
    {
        System.Net.IPEndPoint RemoteEndPoint { get; }
        void Send(ByteDataCarrier data);
        void StartReceive();
        bool IsConnected { get; }
        void Disconnect();        
    }
}
