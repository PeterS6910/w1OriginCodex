using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;

using System.IO;
using System.Security.Cryptography.X509Certificates;



using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    [Obsolete]
    public class TunnelSocketPair
    {
        private Socket _client = null;
        public Socket Client
        {
            get { return _client; }
        }

        protected byte[] m_arClientReceiveBuffer = null;
        public byte[] ClientReceiveBuffer
        {
            get { return m_arClientReceiveBuffer; }
        }

        private const int _defaultBufferSize = 8192;
        private Socket _server = null;
        public Socket Server
        {
            get { return _server; }
        }

        protected byte[] m_arServerReceiveBuffer = null;
        public byte[] ServerReceiveBuffer
        {
            get { return m_arServerReceiveBuffer; }
        }

        public TunnelSocketPair(
            [NotNull] Socket client,
            [NotNull] Socket server,
            int receiveBufferSize)
        {
            Validator.CheckForNull(client,"client");
            Validator.CheckForNull(server,"server");

            _client = client;
            _server = server;

            if (receiveBufferSize <= 0 || receiveBufferSize > 33554432)
            {
                m_arClientReceiveBuffer = new byte[_defaultBufferSize];
                m_arServerReceiveBuffer = new byte[_defaultBufferSize];
            }
            else
            {
                m_arClientReceiveBuffer = new byte[receiveBufferSize];
                m_arServerReceiveBuffer = new byte[receiveBufferSize];
            }

        }


        public TunnelSocketPair(Socket client, Socket server)
            :this(client,server,-1)
        { 
        }
    }

    [Obsolete]
    public class SecureTunnelSocketPair : TunnelSocketPair
    {
        private Stream _encryptedStream = null;
        public Stream Encrypted
        {
            get { return _encryptedStream; }
        }

        private Stream _plainStream = null;
        public Stream Plain
        {
            get { return _plainStream; }
        }

        private byte[] m_arPlainReceiveBuffer = null;
        public byte[] PlainReceiveBuffer
        {
            get { return m_arPlainReceiveBuffer; }
        }

        private byte[] m_arEncryptedReceiveBuffer = null;
        public byte[] EncryptedReceiveBuffer
        {
            get { return m_arEncryptedReceiveBuffer; }
        }

        public SecureTunnelSocketPair(Socket client, Socket server,Stream plain,Stream encrypted,bool plainSideIsServer,int bufferSize)
            :base(client,server,bufferSize)
        {
            Validator.CheckForNull(plain,"plain");
            Validator.CheckForNull(encrypted,"encrypted");

            _encryptedStream = encrypted;
            _plainStream = plain;

            if (plainSideIsServer)
            {

                m_arPlainReceiveBuffer = m_arServerReceiveBuffer;
                m_arEncryptedReceiveBuffer = m_arClientReceiveBuffer;
            }
            else {

                m_arPlainReceiveBuffer = m_arClientReceiveBuffer;
                m_arEncryptedReceiveBuffer = m_arServerReceiveBuffer;
            }



        }

        

        public SecureTunnelSocketPair(Socket client, Socket server, Stream plain, Stream encrypted, bool plainSideIsServer)
            : this(client, server,plain,encrypted,plainSideIsServer,-1)
        {
           
        }

        public void ClosePair()
        {
            try
            {
                if (null != _encryptedStream) {
                    _encryptedStream.Close();
                    _encryptedStream = null;
                }
            }
            catch { }

            try
            {
                if (null != _plainStream) 
                {
                    _plainStream.Close();
                    _plainStream = null;
                }
            }
            catch { }
        }

        public bool IsOpened
        {
            get { return null != _plainStream && null != _encryptedStream; }
        }

        protected internal LinkedListNode<SecureTunnelSocketPair> _connectionItem = null;
    }

    [Obsolete]
    public class SecureTunnel
    {
        private Socket _encryptedSocket = null;
        private Socket _plainSocket = null;
        private SecureTunnelMode _mode;

        private IPEndPoint _connectingEndPoint = null;
        private IPEndPoint _hostingEndPoint = null;
        private X509Certificate2 _certificate = null;

        private const int _bufferSize = 8192;
        private byte[] m_arPlainBuffer = new byte[_bufferSize];
        private byte[] m_arEncryptedBuffer = new byte[_bufferSize];

        private bool _allowUntrustedRoot = false;

        /*public bool SslAllowUntrustedRoot
        {
            get { return _allowUntrustedRoot; }
            set { _allowUntrustedRoot = value; }
        }*/

        private LinkedList<SecureTunnelSocketPair> _connections = new LinkedList<SecureTunnelSocketPair>();

        public SecureTunnel(SecureTunnelMode mode)
        {
            _mode = mode;

            _encryptedSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _plainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /*
        public CSecureTunnel()
            :this(TSecureTunnelMode.Plain2EncryptedHosting)
        {

        }*/

        public void SetEncryptedSide(IPEndPoint iPEndPoint,string certificatePath,bool allowUntrustedRoot)
        {
            switch (_mode)
            {
                case SecureTunnelMode.Encrypted2PlainHosting:
                    if (iPEndPoint.Address == IPAddress.Any ||
                        iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \""+iPEndPoint.Address+"\" - is invalid");

                    TcpUdpPort.CheckValidity(iPEndPoint.Port);

                    
                    if (null != certificatePath &&
                        !File.Exists(certificatePath))
                    {
                        throw new ArgumentException("Client certificate specified by path \"" +
                            (null == certificatePath ? "" : certificatePath) + "\" does not exist");
                    }

                    _connectingEndPoint = iPEndPoint;

                    break;

                case SecureTunnelMode.Plain2EncryptedHosting:
                    if (iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \"" + iPEndPoint.Address + "\" - is invalid");

                    TcpUdpPort.CheckValidity(iPEndPoint.Port);

                    
                    
                    if (!File.Exists(certificatePath))
                    {
                        throw new ArgumentException("Server certificate specified by path \""+
                            (null == certificatePath ? "" : certificatePath)+"\" does not exist");
                    }

                    _hostingEndPoint = iPEndPoint;

                    break;


            }

            _allowUntrustedRoot = allowUntrustedRoot;

            if (null != certificatePath)
            {   
                _certificate = new X509Certificate2(certificatePath);
            }
        }

        public void SetEncryptedSide(IPEndPoint iPEndPoint, string certificatePath)
        {
            SetEncryptedSide(iPEndPoint, certificatePath, false);
        }

        public void SetEncryptedSide(IPAddress iP, int iPort, string certificatePath, bool allowUntrustedRoot)
        {
            SetEncryptedSide(new IPEndPoint(iP, iPort), certificatePath, allowUntrustedRoot);
        }

        public void SetPlainSide(IPEndPoint iPEndPoint)
        {
            switch (_mode)
            {
                case SecureTunnelMode.Encrypted2PlainHosting:
                    if (iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \"" + iPEndPoint.Address + "\" - is invalid");

                    TcpUdpPort.CheckValidity(iPEndPoint.Port);

                    _hostingEndPoint = iPEndPoint;


                    break;

                case SecureTunnelMode.Plain2EncryptedHosting:
                    

                    if (iPEndPoint.Address == IPAddress.Any ||
                        iPEndPoint.Address == IPAddress.Broadcast)
                        throw new ArgumentException("IP address to be used for encrypted connection - \"" + iPEndPoint.Address + "\" - is invalid");

                    TcpUdpPort.CheckValidity(iPEndPoint.Port);


                    _connectingEndPoint = iPEndPoint;

                    break;
            }
        }

        public void SetPlainSide(IPAddress iP,int i_iPort)
        {
            SetPlainSide(new IPEndPoint(iP, i_iPort));
        }

        public void Start()
        {
            switch (_mode)
            {
                case SecureTunnelMode.Plain2EncryptedHosting:
                    _encryptedSocket.Bind(_hostingEndPoint);
                    _encryptedSocket.Listen(16);


                    _encryptedSocket.BeginAccept(
                        OnEncryptedSocketAccept,
                        null);
                        
                    
                    break;
                case SecureTunnelMode.Encrypted2PlainHosting:
                    

                    _plainSocket.Bind(_hostingEndPoint);
                    _plainSocket.Listen(16);

                    _plainSocket.BeginAccept(
                        OnPlainSocketAccept,
                        null);


                    break;
            }
        }


        

        private void PlanPlainSocketReceive(SecureTunnelSocketPair socketPair)
        {
            socketPair.Plain.BeginRead(
                socketPair.PlainReceiveBuffer, 0, socketPair.PlainReceiveBuffer.Length,
                OnPlainSocketReceived,
                socketPair);
        }

        private void PlanEncryptedSocketReceive(SecureTunnelSocketPair socketPair)
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

            SecureTunnelSocketPair aSocketPair = new SecureTunnelSocketPair(aEncryptedClient, aPlainServer, 
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
            catch
            {
                return;
            }

            SecureTunnelSocketPair aSocketPair = new SecureTunnelSocketPair(aPlainClient, aEncryptedServer, 
                aPlainStream, aSslStream,false);
            
            // planning asycnhronous socket receive on both sides
            PlanPlainSocketReceive(aSocketPair);
            PlanEncryptedSocketReceive(aSocketPair);


            RegisterSocketPair(aSocketPair);
        }

        private void RegisterSocketPair(SecureTunnelSocketPair socketPair)
        {
            if (null == socketPair)
                return;

            // registering the pair of connections
            LinkedListNode<SecureTunnelSocketPair> aConnectionItem = _connections.AddLast(socketPair);
            socketPair._connectionItem = aConnectionItem;

#if DEBUG
            Console.WriteLine(".. connection count : "+_connections.Count);
#endif
        }

        private void UnregisterSocketPair(SecureTunnelSocketPair socketPair)
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
            SecureTunnelSocketPair aSocketPair = (SecureTunnelSocketPair)result.AsyncState;
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

                Array.Copy(aSocketPair.EncryptedReceiveBuffer, arSendBuffer, iSize);

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
            SecureTunnelSocketPair aSocketPair = (SecureTunnelSocketPair)result.AsyncState;
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

                Array.Copy(aSocketPair.PlainReceiveBuffer, arSendBuffer, iSize);

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
            SecureTunnelSocketPair aSocketPair = (SecureTunnelSocketPair)result.AsyncState;
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
            SecureTunnelSocketPair aSocketPair = (SecureTunnelSocketPair)result.AsyncState;
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
}

