using System;
using System.IO;
using System.Security.Cryptography;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Net
#else
namespace Contal.IwQuick.Net
#endif
{
    /// <summary>
    /// Class implement stream with symetric encryption
    /// </summary>
    public class AESStream : Stream
    {
        /// <summary>
        /// Class implement interface IAsyncResult
        /// I use this class for bregin read from external stream
        /// </summary>
        private class AESStreamAsyncResult : IAsyncResult
        {
            private int _offset;
            private int _count;
            object _asyncState;
            byte[] _buffer;
            byte[] _externalStreamBuffer;
            bool _completedSynchronously;
            AsyncCallback _asyncCallback;

            /// <summary>
            /// Implement method for interface IAsyncResult
            /// Allways return true
            /// </summary>
            public bool IsCompleted
            {
                get { return true; }
            }

            /// <summary>
            /// Implement method for interface IAsyncResult
            /// </summary>
            public bool CompletedSynchronously
            {
                get { return _completedSynchronously; }
            }

            /// <summary>
            /// Implement method for interface IAsyncResult
            /// Allways return null
            /// </summary>
            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { return null; }
            }

            /// <summary>
            /// Implement method for interface IAsyncResult
            /// </summary>
            public object AsyncState
            {
                get { return _asyncState; }
            }

            /// <summary>
            /// Return origin buffer
            /// </summary>
            public byte[] Buffer
            {
                get { return _buffer; }
            }

            /// <summary>
            /// Return origin offset
            /// </summary>
            public int Offset
            {
                get { return _offset; }
            }

            /// <summary>
            /// Return origin count
            /// </summary>
            public int Count
            {
                get { return _count; }
            }

            /// <summary>
            /// Return origin callback function
            /// </summary>
            public AsyncCallback AsyncCallback
            {
                get { return _asyncCallback; }
            }

            /// <summary>
            /// Return external stream buffer
            /// </summary>
            public byte[] externalStreamBuffer
            {
                get { return _externalStreamBuffer; }
            }


            public AESStreamAsyncResult(byte[] buffer, int offset, int count, object asyncState, bool completedSynchronously, byte[] externalStreamBuffer, AsyncCallback asyncCallback)
            {
                _offset = offset;
                _count = count;
                _asyncState = asyncState;
                _buffer = buffer;
                _completedSynchronously = completedSynchronously;
                _externalStreamBuffer = externalStreamBuffer;
                _asyncCallback = asyncCallback;
            }
        }

        public const int BUFFER_SIZE = 65536;
        private const byte BLOCK_SIZE = AESSettings.BLOCK_SIZE / 8;
        private const int INITIAL_BLOCK_LENGTH_RETRY_COUNT = 3;

        private Stream _externalStream = null;
        private MemoryStream _internalMemoryStream = null;
        private ICryptoTransform _encryptor = null;
        private ICryptoTransform _decryptor = null;
        private Exception _readException = null;
        private SymmetricAlgorithm _algoritm = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="algorithm">Symmetric crypto algorithm</param>
        public AESStream(Stream stream, SymmetricAlgorithm algorithm)
        {
            _externalStream = stream;
            _internalMemoryStream = new MemoryStream();
            _algoritm = algorithm;
            _encryptor = _algoritm.CreateEncryptor();
            _decryptor = _algoritm.CreateDecryptor();
        }

        /// <summary>
        /// Return CanRead from externa stream
        /// </summary>
        public override bool CanRead
        {
            get
            {
                if (_externalStream == null)
                {
                    throw new ArgumentException("External stream failed");
                }
                else
                {
                    return _externalStream.CanRead;
                }
            }
        }

        /// <summary>
        /// Return CanSeek from external stream
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                if (_externalStream == null)
                {
                    throw new ArgumentException("External stream failed");
                }
                else
                {
                    return _externalStream.CanSeek;
                }
            }
        }

        /// <summary>
        /// Return CanWrite from external stream
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                if (_externalStream == null)
                {
                    throw new ArgumentException("External stream failed");
                }

                return _externalStream.CanWrite;
            }
        }

        /// <summary>
        /// Set length on the external stream
        /// </summary>
        /// <param name="value">New length</param>
        public override void SetLength(long value)
        {
            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            _externalStream.SetLength(value);
        }

        /// <summary>
        /// Set position on the external stream
        /// </summary>
        public override long Position
        {
            get
            {
                if (_externalStream == null)
                {
                    throw new ArgumentException("External stream failed");
                }

                return _externalStream.Position;
            }
            set
            {
                if (_externalStream == null)
                {
                    throw new ArgumentException("External stream failed");
                }

                _externalStream.Position = value;
            }
        }

        /// <summary>
        /// Get length from the external stream
        /// </summary>
        public override long Length
        {
            get
            {
                if (_externalStream == null)
                {
                    throw new ArgumentException("External stream failed");
                }

                return _externalStream.Length;
            }
        }

        /// <summary>
        /// Run seek on the externa stream
        /// </summary>
        /// <param name="offset">Count of bytes to offset</param>
        /// <param name="origin">From where</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            return _externalStream.Seek(offset, origin);
        }

        /// <summary>
        /// Run flush on the externa stream
        /// </summary>
        public override void Flush()
        {
            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            _externalStream.Flush();
        }

        /// <summary>
        /// Read byte from internal memory stream. If internal memory stream is empty then read from exteran stream.
        /// </summary>
        /// <param name="buffer">Buffer for reading bytes</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to read</param>
        /// <returns>Count of reading bytes</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_internalMemoryStream == null)
            {
                throw new ArgumentException("Internal memory stream failed");
            }

            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            byte[] inputBuffer = new byte[BUFFER_SIZE];

            if (_internalMemoryStream.Position == _internalMemoryStream.Length)
            {
                int length;
                int writedDataLength = 0;
                do
                {
                    length = _externalStream.Read(inputBuffer, 0, inputBuffer.Length);

                    if (length > 0)
                    {
                        writedDataLength = WriteToInternalMemoryStream(inputBuffer, length);
                    }
                } while (length > 0 && writedDataLength == 0);
            }

            return _internalMemoryStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Decrypte bytes form array inputData and write decrypted data to internal memory stream
        /// </summary>
        /// <param name="inputData">Data from external stream</param>
        /// <param name="length">Count of bytes to decryption</param>
        private int WriteToInternalMemoryStream(byte[] inputData, int length)
        {
            try
            {
                byte[] decryptedData;

                int decryptedDataLength = DecryptData(inputData, length, out decryptedData);

                if (_internalMemoryStream.Position == _internalMemoryStream.Length)
                {
                    _internalMemoryStream.Position = 0;
                    _internalMemoryStream.SetLength(0);
                }

                long readPosition = _internalMemoryStream.Position;
                _internalMemoryStream.Position = _internalMemoryStream.Length;
                _internalMemoryStream.Write(decryptedData, 0, decryptedDataLength);
                _internalMemoryStream.Position = readPosition;

                return decryptedDataLength;
            }
            catch
            {
            }

            return -1;
        }

        byte[] _oldInputData;

        private int DecryptData(byte[] inputData, int length, out byte[] decryptedData)
        {
            //byte[] actInputData = null;
            if (_oldInputData != null && _oldInputData.Length > 0)
            {
                byte[] newInputData = new byte[_oldInputData.Length + length];
                _oldInputData.CopyTo(newInputData, 0);
                for (int i = 0; i < length; i++)
                {
                    newInputData[_oldInputData.Length + i] = inputData[i];
                }

                //actInputData = inputData;
                inputData = newInputData;
                length = inputData.Length;
            }

            int position = 0;
            int decryptedDataLength = 0;
            decryptedData = new byte[inputData.Length];

            while (length - position > BLOCK_SIZE)
            {
                int dataLenthToDecrypt = ReadLength(inputData, position);
                if (dataLenthToDecrypt > 0)
                {
                    if (length - position - BLOCK_SIZE >= dataLenthToDecrypt)
                    {
                        position += BLOCK_SIZE;

                        byte[] actDecryptedData = DoDecrypteData(inputData, position, dataLenthToDecrypt);

                        if (actDecryptedData != null && actDecryptedData.Length > 0)
                        {
                            actDecryptedData.CopyTo(decryptedData, decryptedDataLength);
                            decryptedDataLength += actDecryptedData.Length;
                            position += dataLenthToDecrypt;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                //else if (actInputData != null)
                //{
                //    if (_oldInputData != null)
                //    {
                //        _oldInputData.CopyTo(decryptedData, decryptedDataLength);
                //        decryptedDataLength += _oldInputData.Length;
                //    }

                //    inputData = actInputData;
                //}
                else
                {
                    decryptedData[decryptedDataLength] = inputData[position];
                    decryptedDataLength += 1;
                    position += 1;
                }

                //_oldInputData = null;
                //actInputData = null;
            }

            if (position < length)
            {
                _oldInputData = new byte[length - position];
                for (int i = 0; i < length - position; i++)
                {
                    _oldInputData[i] = inputData[position + i];
                }
            }
            else
            {
                _oldInputData = null;
            }

            return decryptedDataLength;
        }

        private int ReadLength(byte[] inputData, int position)
        {
            try
            {
                byte[] decryptedData = DoDecrypteData(inputData, position, BLOCK_SIZE);

                if (decryptedData != null && decryptedData.Length == sizeof(Int32) * INITIAL_BLOCK_LENGTH_RETRY_COUNT)
                {
                    bool isInitialBlock = true;
                    for (int actByte = 0; actByte < sizeof(Int32); actByte++)
                    {
                        byte blockCountByte = decryptedData[actByte];
                        for (int i = 1; i < INITIAL_BLOCK_LENGTH_RETRY_COUNT; i++)
                        {
                            if (decryptedData[i * sizeof(Int32) + actByte] != blockCountByte)
                            {
                                isInitialBlock = false;
                                break;
                            }
                        }

                        if (!isInitialBlock)
                            break;
                    }

                    if (isInitialBlock)
                    {
                        int dataLengthToDecrypt = 0;
                        for (int i = sizeof(Int32) - 1; i >= 0; i--)
                        {
                            if (dataLengthToDecrypt > 0)
                            {
                                dataLengthToDecrypt = dataLengthToDecrypt * 256;
                            }

                            if (decryptedData[i] > 0)
                            {
                                dataLengthToDecrypt += decryptedData[i];
                            }
                        }

                        dataLengthToDecrypt = dataLengthToDecrypt * BLOCK_SIZE;

                        if (dataLengthToDecrypt > 0 && dataLengthToDecrypt <= BUFFER_SIZE)
                            return dataLengthToDecrypt;
                    }
                }
            }
            catch { }

            return -1;
        }

        private byte[] DoDecrypteData(byte[] buffer, int offset, int length)
        {
            try
            {
                try
                {
                    return _decryptor.TransformFinalBlock(buffer, offset, length);
                }
                catch
                {
                    _decryptor = _algoritm.CreateDecryptor();
                }
            }
            catch { }

            return null;

            //byte[] notDecryptedData = new byte[length];
            //for (int i = 0; i < length; i++)
            //{
            //    notDecryptedData[i] = buffer[offset + i];
            //}

            //return notDecryptedData;
        }

        /// <summary>
        /// Encrypt bytes and write to external stream
        /// </summary>
        /// <param name="buffer">Buffer with bytes to write</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_encryptor == null)
            {
                throw new ArgumentException("Encryptor failed");
            }

            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            byte[] encryptedData;
            int encryptedDataLength = EncryptData(buffer, offset, count, out encryptedData);

            _externalStream.Write(encryptedData, 0, encryptedDataLength);
        }

        private int EncryptData(byte[] buffer, int offset, int count, out byte[] encryptedData)
        {
            int encryptedDataLength = 0;
            encryptedData = new byte[((buffer.Length / (BUFFER_SIZE - BLOCK_SIZE)) + 1) * (BUFFER_SIZE + BLOCK_SIZE)];

            try
            {
                int position = offset;
                while (position < count)
                {
                    int dataLengthToEncrypt = BUFFER_SIZE - BLOCK_SIZE;

                    if (dataLengthToEncrypt > count - position)
                        dataLengthToEncrypt = count - position;

                    byte[] newEncryptedData = DoEncrypteData(buffer, position, dataLengthToEncrypt);

                    if (newEncryptedData != null && newEncryptedData.Length > 0)
                    {
                        byte[] initialBlock = WriteLength(newEncryptedData.Length);
                        if (initialBlock != null && initialBlock.Length > 0)
                        {
                            initialBlock.CopyTo(encryptedData, encryptedDataLength);
                            encryptedDataLength += initialBlock.Length;
                        }

                        newEncryptedData.CopyTo(encryptedData, encryptedDataLength);
                        encryptedDataLength += newEncryptedData.Length;
                    }

                    position += dataLengthToEncrypt;
                }
            }
            catch { }

            return encryptedDataLength;
        }

        private byte[] WriteLength(int length)
        {
            try
            {
                int blockCount = length / BLOCK_SIZE;
                byte[] blockCountData = new byte[sizeof(Int32)];

                int actByte = 0;
                while (blockCount > 0)
                {
                    if (blockCount > 255)
                    {
                        byte blockCountByte = (byte)(blockCount % 256);
                        blockCountData[actByte] = blockCountByte;
                        blockCount = blockCount / 256;
                        actByte++;
                    }
                    else
                    {
                        blockCountData[actByte] = (byte)blockCount;
                        blockCount = 0;
                    }
                }

                byte[] data = new byte[sizeof(Int32) * INITIAL_BLOCK_LENGTH_RETRY_COUNT];
                for (int i = 0; i < INITIAL_BLOCK_LENGTH_RETRY_COUNT; i++)
                {
                    blockCountData.CopyTo(data, i * sizeof(Int32));
                }

                byte[] encryptedData = DoEncrypteData(data, 0, data.Length);
                return encryptedData;
            }
            catch { }

            return null;
        }

        private byte[] DoEncrypteData(byte[] buffer, int offset, int length)
        {
            try
            {
                return _encryptor.TransformFinalBlock(buffer, offset, length);
            }
            catch { }

            return null;
        }

        /// <summary>
        /// If internal memory stream is not empty than read data from internal memory stream. Else ran begin read from external stream.
        /// </summary>
        /// <param name="buffer">Buffer for reading bytes</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to read</param>
        /// <param name="asyncCallback">This function is called when bytes are reading from stream</param>
        /// <param name="asyncState">Object for asyncState</param>
        /// <returns>Asynchronous result</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            if (_internalMemoryStream == null)
            {
                throw new ArgumentException("Internal memory stream failed");
            }

            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            byte[] inputData = new byte[BUFFER_SIZE];

            if (_internalMemoryStream.Position < _internalMemoryStream.Length)
            {
                AESStreamAsyncResult asyncResult = new AESStreamAsyncResult(buffer, offset, count, asyncState, true, null, null);
                asyncCallback(asyncResult);
                return asyncResult;
            }
            else
            {
                AESStreamAsyncResult aesAsyncResult = new AESStreamAsyncResult(buffer, offset, count, asyncState, false, inputData, asyncCallback);
                return _externalStream.BeginRead(inputData, 0, inputData.Length, OnReadExternalStream, aesAsyncResult);
            }
        }

        /// <summary>
        /// Read bytes from external stream, write decrypted bytes to internal memory stream and run AsyncCallback function.
        /// </summary>
        /// <param name="result">Asynchronous result</param>
        private void OnReadExternalStream(IAsyncResult result)
        {
            AESStreamAsyncResult aesAsyncResult = result.AsyncState as AESStreamAsyncResult;

            try
            {
                int length = _externalStream.EndRead(result);

                if (length > 0)
                {
                    byte[] buffer = aesAsyncResult.externalStreamBuffer;
                    WriteToInternalMemoryStream(buffer, length);
                }

                aesAsyncResult.AsyncCallback(aesAsyncResult);
            }
            catch (Exception exception)
            {
                _readException = exception;
                aesAsyncResult.AsyncCallback(aesAsyncResult);
            }
        }

        /// <summary>
        /// Encrypt data and write to external stream
        /// </summary>
        /// <param name="buffer">Buffer with bytes to write</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to write</param>
        /// <param name="asyncCallback">This function is called when bytes are writing to stream</param>
        /// <param name="asyncState">Object for asyncState</param>
        /// <returns>Asynchronous result</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            if (_encryptor == null)
            {
                throw new ArgumentException("Encryptor failed");
            }

            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            byte[] encryptedData;
            int encryptedDataLength = EncryptData(buffer, offset, count, out encryptedData);
            return _externalStream.BeginWrite(encryptedData, 0, encryptedDataLength, asyncCallback, asyncState);
        }

        /// <summary>
        /// Read bytes from intermal memory stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        /// <returns>Count of readed bytes</returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            if (_readException != null)
            {
                Exception exception = _readException;
                _readException = null;
                throw exception;
            }

            if (_internalMemoryStream == null)
            {
                throw new ArgumentException("Internal memory stream failed");
            }

            AESStreamAsyncResult aesAsyncResult = asyncResult as AESStreamAsyncResult;

            return _internalMemoryStream.Read(aesAsyncResult.Buffer, aesAsyncResult.Offset, aesAsyncResult.Count);
        }

        /// <summary>
        /// End write bytes to external stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (_externalStream == null)
            {
                throw new ArgumentException("External stream failed");
            }

            _externalStream.EndWrite(asyncResult);
        }
    }
}
