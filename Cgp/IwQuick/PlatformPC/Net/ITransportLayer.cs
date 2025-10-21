using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net.Sockets;

namespace Contal.IwQuick.Net
{
    public interface ITransportLayer
    {
        /// <summary>
        /// Prepare client stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new stream</returns>
        Stream PrepareClientStream(Socket socket);

        /// <summary>
        /// Prepare server stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new stream</returns>
        Stream PrepareServerStream(Socket socket);

        void PreConnect(Socket socket);
    }
}
