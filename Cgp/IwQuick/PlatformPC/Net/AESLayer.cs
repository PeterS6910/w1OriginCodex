using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// Class for create encrypton stream
    /// </summary>
    public class AESLayer : ITransportLayer
    {
        private readonly SymmetricAlgorithm _algorithm;
        private readonly bool _useAesStream2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="useAesStream2"></param>
        public AESLayer(SymmetricAlgorithm algorithm, bool useAesStream2)
        {
            _algorithm = algorithm;
            _useAesStream2 = useAesStream2;
        }

        /// <summary>
        /// Implement metod PrepareClientStream from interface ITransportSettings
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns></returns>
        public Stream PrepareClientStream(Socket socket)
        {
            return PrepareAESStream(socket);
        }

        /// <summary>
        /// Implement metod PrepareServerStream from interface ITransportSettings
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns></returns>
        public Stream PrepareServerStream(Socket socket)
        {
            return PrepareAESStream(socket);
        }

        /// <summary>
        /// Prepare server and client stream with symetric encryption. Create network stream and than AESStream.
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns></returns>
        private Stream PrepareAESStream(Socket socket)
        {
            Stream innerStream = new NetworkStream(socket, false);

            return
                _useAesStream2
                    ? (Stream) new AESStream2(innerStream, _algorithm)
                    : new AESStream(innerStream, _algorithm);
        }

        public void PreConnect(Socket socket)
        {
        }
    }
}
