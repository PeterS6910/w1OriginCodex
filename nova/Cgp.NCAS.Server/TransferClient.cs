using Contal.IwQuick.Net;

namespace Contal.Cgp.NCAS.Server
{
    public class TransferClient
    {
        private TFTPClient _tftpClient;
        public TFTPClient TftpClient
        {
            get { return _tftpClient; }
            set { _tftpClient = value; }
        }

        private DataChannelPeer.TransferInfo _tcpClient;
        public DataChannelPeer.TransferInfo TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        public TransferClient(TFTPClient client)
        {
            _tftpClient = client;
        }

        public TransferClient(DataChannelPeer.TransferInfo client)
        {
            _tcpClient = client;
        }

        public void Stop()
        {
            try
            {
                if (_tftpClient != null)
                    _tftpClient.Stop();
                if (_tcpClient != null)
                    _tcpClient.AsyncStopTransfer();
            }
            catch
            {
            }
        }
    }
}