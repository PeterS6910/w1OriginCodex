using System.IO;
using System.Net.Sockets;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransportLayer
    {
        /// <summary>
        /// intiates actions over transport layer that are predcessing the Connect process over the socket
        /// </summary>
        /// <param name="socket"></param>
        void PreConnect(Socket socket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        void PreConnect(ISuperSocket socket);

        /// <summary>
        /// Prepare client stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new stream</returns>
        Stream PrepareClientStream(Socket socket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        Stream PrepareClientStream(ISuperSocket socket);

        /// <summary>
        /// Prepare server stream
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return new stream</returns>
        Stream PrepareServerStream(Socket socket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        Stream PrepareServerStream(ISuperSocket socket);
    }
}
