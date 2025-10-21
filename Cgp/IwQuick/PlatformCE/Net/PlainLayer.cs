using System.Net.Sockets;
using System.IO;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class PlainLayer : ITransportLayer
    {
        /// <summary>
        /// Prepare server stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Retrun new stream</returns>
        public Stream PrepareClientStream(Socket socket)
        {
            return null; //PreparePlainStream(socket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public Stream PrepareClientStream(ISuperSocket socket)
        {
            // DO NOT CREATE A STREAM, THE NETWORK TRAFFIC WILL GO DIRECTLY
            return null;
        }

        /// <summary>
        /// Prepare client stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new stream</returns>
        public Stream PrepareServerStream(Socket socket)
        {
            return null; // PreparePlainStream(socket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public Stream PrepareServerStream(ISuperSocket socket)
        {
            // DO NOT CREATE A STREAM, THE NETWORK TRAFFIC WILL GO DIRECTLY
            return null;
        }

        /// <summary>
        /// Create new network stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new network stream</returns>
        private Stream PreparePlainStream(Socket socket)
        {
            return new NetworkStream(socket, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        public void PreConnect(Socket socket)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        public void PreConnect(ISuperSocket socket)
        {
        }
    }
}
