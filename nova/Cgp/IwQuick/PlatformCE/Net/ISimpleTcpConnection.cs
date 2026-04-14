using Contal.IwQuickCF.Data;

namespace Contal.IwQuickCF.Net
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
    }
}
