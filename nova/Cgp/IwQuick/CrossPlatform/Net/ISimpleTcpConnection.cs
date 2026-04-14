using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISimpleTcpConnection
    {
        /// <summary>
        /// 
        /// </summary>
        System.Net.IPEndPoint RemoteEndPoint { get; }
        /// <summary>
        /// 
        /// </summary>
        System.Net.IPEndPoint LocalEndPoint { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void Send(ByteDataCarrier data);
        //ByteDataCarrier ReceiveBuffer { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 
        /// </summary>
        void StartReceive();
    }
}
