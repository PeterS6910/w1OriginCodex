using System;
using System.Net;
using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISuperSocket
    {
        /// <summary>
        /// Connect to specified endpoint
        /// </summary>
        /// <param name="endPoint">IPEndPoint specifing remote host</param>
        void Connect(IPEndPoint endPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndPoint"></param>
        void Bind(IPEndPoint ipEndPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        void Bind(IPAddress ip, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        void Bind(string ipAddress, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backLog"></param>
        void Listen(int backLog);

        /// <summary>
        /// 
        /// </summary>
        void Listen();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        SuperSocket Accept();

        /// <summary>
        /// 
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Indicates whether socket is already connected
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Timeout for receive operation in ms
        /// </summary>
        int ReceiveTimeout { get; set; }

        /// <summary>
        /// Timeout for send operation in ms
        /// </summary>
        int SendTimeout { get; set; }

        /// <summary>
        /// Timeout for connect operation in ms
        /// </summary>
        int ConnectTimeout { get; set; }

        /// <summary>
        /// Set whether SSL should be used
        /// </summary>
        bool UseSSL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        SSLVersion SSLVersion { get; set; }

        /// <summary>
        /// Connect to specified address and port
        /// </summary>
        /// <param name="ipAddress">IP address of the remote host</param>
        /// <param name="port">port of the remote host</param>
        void Connect(IPAddress ipAddress, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        void Connect(string ipAddress, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void Send(byte[] buffer, int offset, int count);

        /// <summary>
        /// Send data viac the socket
        /// </summary>
        /// <param name="data">byte array containing data</param>
        /// <param name="length">number of bytes to send</param>
        void Send(byte[] data, int length);

        /// <summary>
        /// Send data via the socket
        /// </summary>
        /// <param name="data">data object that contains the data to be sent</param>
        void Send(ByteDataCarrier data);

        /// <summary>
        /// Receive data from the socket
        /// </summary>
        /// <param name="data">reference to object that will contain the data</param>
        /// <param name="receiveTimeout"></param>
        /// <returns>number of bytes read</returns>
        int Receive(ByteDataCarrier data, int receiveTimeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Receive(ByteDataCarrier data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int Receive(byte[] buffer, int offset, int count);

        /// <summary>
        /// Receive data from the socket
        /// </summary>
        /// <param name="data">reference to byte array</param>
        /// <param name="length">maxlength of the data</param>
        /// <returns>number of bytes read</returns>
        int Receive(ref byte[] data, int length);

        /// <summary>
        /// Disconnect socket
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 
        /// </summary>
        void Close();


        /// <summary>
        /// 
        /// </summary>
        void Dispose();
    }
}