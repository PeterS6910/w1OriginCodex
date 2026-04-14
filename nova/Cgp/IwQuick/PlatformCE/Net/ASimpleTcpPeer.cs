using System.Net.Sockets;

using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// base class for tcp client / server
    /// </summary>
    public abstract class ASimpleTcpPeer
    {
        protected volatile ISuperSocket _mainSocket = null;
        protected readonly object _msSync = new object();

        protected int _bufferSize = 65536;
        /// <summary>
        /// receive buffer size for the reception
        /// </summary>
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                if (value >= 1)
                    _bufferSize = value;
            }
        }

        /// <summary>
        /// abstract send to be implement by all peer implementations
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        protected internal abstract void Send(SimpleGeneralTcpConnection connection, ByteDataCarrier data);

        protected ITransportSettings _transportSettings = null;
        /// <summary>
        /// reference to encryption settings
        /// </summary>
        public ITransportSettings TransportSettings
        {
            get { return _transportSettings; }
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract bool UseSendingProcessingQueue { get; set; }
        protected internal abstract void InvokeDisconnected(SimpleGeneralTcpConnection connection);
        protected internal abstract void InvokeDataReceived(SimpleGeneralTcpConnection connection, ByteDataCarrier data);
        protected internal abstract void InvokeDataSent(SimpleGeneralTcpConnection connection, ByteDataCarrier data);
        protected internal abstract void InvokeSocketException(SimpleGeneralTcpConnection connection, SocketException socketException);

        
    }
}
