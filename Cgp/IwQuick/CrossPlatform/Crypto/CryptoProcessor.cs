using System;
using System.IO;
using System.Security.Cryptography;

using Contal.IwQuick.Data;


namespace Contal.IwQuick.Crypto
{
    public class CryptoProcessor : IDisposable
    {
        private static readonly ByteBufferPool ByteBufferPool =
            new ByteBufferPool(50, 1024);

        private readonly EncryptorPool _encryptorPool;
        private readonly DecryptorPool _decryptorPool;

        public CryptoProcessor(SymmetricAlgorithm algorithm)
        {
            _encryptorPool = new EncryptorPool(algorithm);
            _decryptorPool = new DecryptorPool(algorithm);
        }

        private static Stream CopyStream(
            Stream inStream,
            CryptoTranformPool cryptoTransformPool)
        {
            var outStream =
                new ChunkedMemoryStream(ByteBufferPool);

            var cryptoTransform = cryptoTransformPool.Get();

            try
            {
                // set up the encryption properties
                using (
                    Stream cryptoStream =
                        new CryptoStream(
                            inStream,
                            cryptoTransform,
                            CryptoStreamMode.Read))
                {
                    // write the whole contents through the new streams
                    var buf = ByteBufferPool.GetBuffer();

                    try
                    {
                        int cnt = cryptoStream.Read(buf, 0, buf.Length);

                        while (cnt > 0)
                        {
                            outStream.Write(buf, 0, cnt);
                            cnt = cryptoStream.Read(buf, 0, buf.Length);
                        }
                    }
                    finally
                    {
                        ByteBufferPool.ReturnBuffer(buf);
                    }
                }

                outStream.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                cryptoTransformPool.Return(cryptoTransform);
            }

            return outStream;
        }

        public Stream EncryptStream(Stream inStream)
        {
            return 
                CopyStream(
                    inStream, 
                    _encryptorPool);
        }

        public Stream DecryptStream(Stream inStream)
        {
            return 
                CopyStream(
                    inStream,
                    _decryptorPool);
        }

        public void Dispose()
        {
            _encryptorPool.Dispose();
            _decryptorPool.Dispose();
        }
    }
}
