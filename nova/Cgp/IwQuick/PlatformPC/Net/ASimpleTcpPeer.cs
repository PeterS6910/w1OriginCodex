using System.Net.Sockets;

using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// base class for tcp client/server
    /// </summary>
    public abstract class ASimpleTcpPeer:ADisposable
    {
        /// <summary>
        /// this socket is created on demand during Start or Connect actions, as it can be destroyed by 
        /// Close calls
        /// </summary>
        protected volatile Socket _mainSocket = null;

        /*
        protected int _sendTimeout = 0;
        public int SendTimeout
        {
            get { return _sendTimeout; }
            set
            {
                if (value <= 0)
                    _sendTimeout = 0;
                else
                {
                    if (value <= 499)
                        _sendTimeout = 500;
                    else
                        _sendTimeout = value;
                }
            }
        }*/

        protected int _bufferSize = 8192;
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
        protected internal abstract void Send(ISimpleTcpConnection connection, ByteDataCarrier data);

        protected ITransportSettings _transportSettings = null;
        /// <summary>
        /// reference to SSL settings
        /// </summary>
        public ITransportSettings TransportSettings
        {
            get { return _transportSettings; }
        }

        public abstract bool UseSendingProcessingQueue { get; set; }
        protected internal abstract void FireDisconnected(ISimpleTcpConnection connection);
        protected internal abstract void FireDataReceived(ISimpleTcpConnection connection, ByteDataCarrier data);
        protected internal abstract void FireDataSent(ISimpleTcpConnection connection, ByteDataCarrier data);
        protected internal abstract void FireSocketException(ISimpleTcpConnection connection, SocketException socketException);

        protected override void InternalDispose(bool isExplicitDispose)
        {
            
        }
    }
}
