namespace Contal.IwQuick.Net
{
    /*
    public class CPlainTunnel
    {
        private Socket _encryptedSocket = null;
        private Socket _plainSocket = null;
        private TSecureTunnelMode _mode;

        private IPEndPoint _connectingEndPoint = null;
        private IPEndPoint _hostingEndPoint = null;

        private const int _bufferSize = 8192;
        private byte[] m_arHostingBuffer = new byte[_bufferSize];
        private byte[] m_arEncryptedBuffer = new byte[_bufferSize];

        private bool _allowUntrustedRoot = false;

        //public bool SslAllowUntrustedRoot
        //{
        //    get { return _allowUntrustedRoot; }
        //    set { _allowUntrustedRoot = value; }
        //}

        private LinkedList<CSecureTunnelSocketPair> _connections = new LinkedList<CSecureTunnelSocketPair>();

        public CPlainTunnel(TSecureTunnelMode mode)
        {
            _mode = mode;

            _encryptedSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _plainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void SetEncryptedSide(IPEndPoint iPEndPoint,string certificatePath,bool i_bAllowUntrustedRoot)
        {
            switch (_mode)
            {
                case TSecureTunnelMode.Encrypted2PlainHosting:
                    if (iPEndPoint.Address == IPAddress.Any ||
                        iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \""+iPEndPoint.Address+"\" - is invalid");

                    CTcpPort.CheckValidity(iPEndPoint.Port);

                    
                    if (null != certificatePath &&
                        !File.Exists(certificatePath))
                    {
                        throw new ArgumentException("Client certificate specified by path \"" +
                            (null == certificatePath ? "" : certificatePath) + "\" does not exist");
                    }

                    _connectingEndPoint = iPEndPoint;

                    break;

                case TSecureTunnelMode.Plain2EncryptedHosting:
                    if (iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \"" + iPEndPoint.Address + "\" - is invalid");

                    CTcpPort.CheckValidity(iPEndPoint.Port);

                    
                    
                    if (!File.Exists(certificatePath))
                    {
                        throw new ArgumentException("Server certificate specified by path \""+
                            (null == certificatePath ? "" : certificatePath)+"\" does not exist");
                    }

                    _hostingEndPoint = iPEndPoint;

                    break;


            }

            if (null != certificatePath)
            {
                _allowUntrustedRoot = i_bAllowUntrustedRoot;
                _certificate = new X509Certificate2(certificatePath);
            }
        }

        public void SetEncryptedSide(IPEndPoint iPEndPoint, string certificatePath)
        {
            SetEncryptedSide(iPEndPoint, certificatePath, false);
        }

        public void SetPlainSide(IPEndPoint iPEndPoint)
        {
            switch (_mode)
            {
                case TSecureTunnelMode.Encrypted2PlainHosting:
                    if (iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \"" + iPEndPoint.Address + "\" - is invalid");

                    CTcpPort.CheckValidity(iPEndPoint.Port);

                    _hostingEndPoint = iPEndPoint;


                    break;

                case TSecureTunnelMode.Plain2EncryptedHosting:
                    

                    if (iPEndPoint.Address == IPAddress.Any ||
                        iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \"" + iPEndPoint.Address + "\" - is invalid");

                    CTcpPort.CheckValidity(iPEndPoint.Port);


                    _connectingEndPoint = iPEndPoint;

                    break;
            }

            

        }

        public void Start()
        {
            switch (_mode)
            {
                case TSecureTunnelMode.Plain2EncryptedHosting:
                    _encryptedSocket.Bind(_hostingEndPoint);
                    _encryptedSocket.Listen(16);


                    _encryptedSocket.BeginAccept(
                        OnEncryptedSocketAccept,
                        null);
                        
                    
                    break;
                case TSecureTunnelMode.Encrypted2PlainHosting:
                    

                    _plainSocket.Bind(_hostingEndPoint);
                    _plainSocket.Listen(16);

                    _plainSocket.BeginAccept(
                        OnPlainSocketAccept,
                        null);


                    break;
            }
        }


        

        private void PlanPlainSocketReceive(CSecureTunnelSocketPair socketPair)
        {
            socketPair.Plain.BeginRead(
                socketPair.PlainReceiveBuffer, 0, socketPair.PlainReceiveBuffer.Length,
                OnPlainSocketReceived,
                socketPair);
        }

        private void PlanEncryptedSocketReceive(CSecureTunnelSocketPair socketPair)
        {
            socketPair.Encrypted.BeginRead(
                socketPair.EncryptedReceiveBuffer, 0, socketPair.EncryptedReceiveBuffer.Length,
                OnEncryptedSocketReceived,
                socketPair);
        }

        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                return true;

            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                string strSubReasons = String.Empty;

                if (null != chain)
                    foreach (X509ChainElement aElement in chain.ChainElements)
                    {
                        foreach (X509ChainStatus aElementStatus in aElement.ChainElementStatus)
                        {
                            strSubReasons += aElementStatus.Status.ToString() + ",";

                            if (aElementStatus.Status == X509ChainStatusFlags.UntrustedRoot &&
                                _allowUntrustedRoot)
                                return true;

                        }

                    }
                else
                    return _allowUntrustedRoot;
                //_innerSslException = new Exception(strSubReasons);

                return false;
            }
            else
                return true;
        }

        private void OnPlainSocketAccept(IAsyncResult result)
        {
            bool bValid = true;
            Socket aPlainServer = null;
            try
            {
                aPlainServer = _plainSocket.EndAccept(result);
                if (null == aPlainServer)
                    bValid = false;
            }
            catch //(Exception aError)
            {
                bValid = false;
            }
            finally
            {
                _plainSocket.BeginAccept(
                        OnPlainSocketAccept,
                        null);
            }

            if (!bValid)
                return;

            Socket aEncryptedClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                aEncryptedClient.Connect(_connectingEndPoint);
            }
            catch //(Exception aError)
            {
                aPlainServer.Close();
                return;
            }

            NetworkStream aPlainStream = new NetworkStream(aPlainServer, true);

            NetworkStream aEncryptedStream = new NetworkStream(aEncryptedClient, true);
            SslStream aSslStream = new SslStream(aEncryptedStream, true, RemoteCertificateValidation);

            if (null == _certificate)
                aSslStream.AuthenticateAsClient("", null, System.Security.Authentication.SslProtocols.Default, false);

            CSecureTunnelSocketPair aSocketPair = new CSecureTunnelSocketPair(aEncryptedClient, aPlainServer, 
                aPlainStream, aSslStream, true);

            // planning asycnhronous socket receive on both sides
            PlanPlainSocketReceive(aSocketPair);
            PlanEncryptedSocketReceive(aSocketPair);

            RegisterSocketPair(aSocketPair);
        }

        private void OnEncryptedSocketAccept(IAsyncResult result)
        {
            bool bValid = true;
            Socket aEncryptedServer = null;
            try
            {
                aEncryptedServer = _encryptedSocket.EndAccept(result);
                if (null == aEncryptedServer)
                    bValid = false;
            }
            catch //(Exception aError)
            {
                bValid = false;
            }
            finally
            {
                _encryptedSocket.BeginAccept(
                        OnEncryptedSocketAccept,
                        null);
            }

            if (!bValid)
                return;

            Socket aPlainClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                aPlainClient.Connect(_connectingEndPoint);
            }
            catch //(Exception aError)
            {
                aEncryptedServer.Close();
                return;
            }

            NetworkStream aPlainStream = new NetworkStream(aPlainClient, true);

            NetworkStream aEncryptedStream = new NetworkStream(aEncryptedServer, true);
            SslStream aSslStream = new SslStream(aEncryptedStream);
                //aEncryptedStream, true, RemoteCertificateValidation);
            try
            {
                aSslStream.AuthenticateAsServer(_certificate,false,System.Security.Authentication.SslProtocols.Default,false);
            }
            catch (Exception aError)
            {
                return;
            }

            CSecureTunnelSocketPair aSocketPair = new CSecureTunnelSocketPair(aPlainClient, aEncryptedServer, 
                aPlainStream, aSslStream,false);
            
            // planning asycnhronous socket receive on both sides
            PlanPlainSocketReceive(aSocketPair);
            PlanEncryptedSocketReceive(aSocketPair);


            RegisterSocketPair(aSocketPair);
        }

        private void RegisterSocketPair(CSecureTunnelSocketPair socketPair)
        {
            if (null == socketPair)
                return;

            // registering the pair of connections
            LinkedListNode<CSecureTunnelSocketPair> aConnectionItem = _connections.AddLast(socketPair);
            socketPair._connectionItem = aConnectionItem;

#if DEBUG
            Console.WriteLine(".. connection count : "+_connections.Count);
#endif
        }

        private void UnregisterSocketPair(CSecureTunnelSocketPair socketPair)
        {
            if (null == socketPair)
                return;


            if (!socketPair.IsOpened)
                return;

            socketPair.ClosePair();

            if (null != socketPair._connectionItem)
            {
                try
                {
                    _connections.Remove(socketPair._connectionItem);
                    socketPair._connectionItem = null;
                }
                catch { }
            }

            socketPair = null;

#if DEBUG
            Console.WriteLine(".. connection count : " + _connections.Count);
#endif

        }

        private void OnEncryptedSocketReceived(IAsyncResult result)
        {
            CSecureTunnelSocketPair aSocketPair = (CSecureTunnelSocketPair)result.AsyncState;
            Debug.Assert(null != aSocketPair);

            int iSize = 0;
            bool bClosePair = false;

            try
            {
                iSize = aSocketPair.Encrypted.EndRead(result);
            }
            catch //(Exception aError)
            {
                bClosePair = true;
            }

            if (iSize > 0 && !bClosePair)
            {
                byte[] arSendBuffer = new byte[iSize];

                Data.CByteArray.Copy(aSocketPair.EncryptedReceiveBuffer, arSendBuffer, iSize);

#if DEBUG
                Console.WriteLine("... " + iSize + " from encrypted to plain");
#endif


                try
                {
                    aSocketPair.Plain.BeginWrite(
                        arSendBuffer, 0, iSize,
                        OnPlainSocketSent,
                        aSocketPair);

                    PlanEncryptedSocketReceive(aSocketPair);
                }
                catch
                {
                    bClosePair = true;
                }

                
            }
            else
                bClosePair = true;


            if (bClosePair)
                UnregisterSocketPair(aSocketPair);
        }

        private void OnPlainSocketReceived(IAsyncResult result)
        {
            CSecureTunnelSocketPair aSocketPair = (CSecureTunnelSocketPair)result.AsyncState;
            Debug.Assert(null != aSocketPair);

            int iSize = 0;
            bool bClosePair = false;

            try
            {
                iSize = aSocketPair.Plain.EndRead(result);
            }
            catch //(Exception aError)
            {
                bClosePair = true;
            }

            if (iSize > 0 && !bClosePair)
            {
                byte[] arSendBuffer = new byte[iSize];

                Data.CByteArray.Copy(aSocketPair.PlainReceiveBuffer, arSendBuffer, iSize);

#if DEBUG
                Console.WriteLine("... "+iSize+" from plain to encrypted");
#endif

                try
                {
                    aSocketPair.Encrypted.BeginWrite(
                        arSendBuffer, 0, iSize,
                        OnEncryptedSocketSent,
                        aSocketPair);

                    PlanPlainSocketReceive(aSocketPair);
                }
                catch
                {
                    bClosePair = true;
                }


                
            }
            else
                bClosePair = true;


            if (bClosePair)           
                UnregisterSocketPair(aSocketPair);
        }

        private void OnPlainSocketSent(IAsyncResult result)
        {
            CSecureTunnelSocketPair aSocketPair = (CSecureTunnelSocketPair)result.AsyncState;
            Debug.Assert(null != aSocketPair);

            bool bClosePair = false;

            try
            {
                aSocketPair.Plain.EndWrite(result);
            }
            catch //(Exception aError)
            {
                bClosePair = true;
            }

            if (bClosePair)
                UnregisterSocketPair(aSocketPair);
        }

        private void OnEncryptedSocketSent(IAsyncResult result)
        {
            CSecureTunnelSocketPair aSocketPair = (CSecureTunnelSocketPair)result.AsyncState;
            Debug.Assert(null != aSocketPair);

            
            bool bClosePair = false;

            try
            {
                aSocketPair.Encrypted.EndWrite(result);
            }
            catch //(Exception aError)
            {
                bClosePair = true;
            }

            if (bClosePair)
                UnregisterSocketPair(aSocketPair);
        }
        
    }

    */
}
