using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;
using System.IO;

namespace Contal.IwQuick.Net
{
    public class PlainLayer : ITransportLayer
    {
        /// <summary>
        /// Prepare server stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Retrun new stream</returns>
        public Stream PrepareClientStream(Socket socket)
        {
            return PreparePlainStream(socket);
        }

        /// <summary>
        /// Prepare client stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new stream</returns>
        public Stream PrepareServerStream(Socket socket)
        {
            return PreparePlainStream(socket);
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

        public void PreConnect(Socket socket)
        {
        }
    }
}
