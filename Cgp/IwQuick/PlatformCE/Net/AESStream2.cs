using System;
using System.IO;
using System.Security.Cryptography;

namespace Contal.IwQuickCF.Net
{
    public class AESStream2 : Stream
    {
        private readonly Stream _innerStream;

        private readonly int _blockSize;

        private readonly ICryptoTransform _encryptor;
        private readonly ICryptoTransform _decryptor;

        private readonly byte[] _inputReadBuffer;
        private int _inputReadOffset;
        private int _inputReadCount;

        private readonly byte[] _outputReadBuffer;
        private int _outputReadOffset;
        private int _outputReadCount;

        private byte[] _readFinalBlock;
        private int _readFinalBlockOffset;
        private int _readFinalBlockCount;

        private readonly byte[] _initialBlockInputBuffer;
        private readonly BinaryWriter _initialBlockBinaryWriter;

        private readonly byte[] _outputWriteBuffer;

        private const int MAX_JUMBO_BLOCK_SIZE = 65536;

        private bool _readStarted;

        public AESStream2(
            Stream innerStream,
            SymmetricAlgorithm algorithm)
        {
            _innerStream = innerStream;

            _blockSize = algorithm.BlockSize / 8;

            _encryptor = algorithm.CreateEncryptor();
            _decryptor = algorithm.CreateDecryptor();

            _inputReadBuffer =
                new byte[MAX_JUMBO_BLOCK_SIZE + _blockSize];

            _outputReadBuffer = new byte[_blockSize];

            _outputWriteBuffer =
                new byte[MAX_JUMBO_BLOCK_SIZE + _blockSize];

            _initialBlockInputBuffer = new byte[sizeof(int) * 3];

            _initialBlockBinaryWriter = new BinaryWriter(
                new MemoryStream(_initialBlockInputBuffer));
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ///TODO uncomment this when upper tiers (LwRemoting ...) become
            /// less dependent upon semantics of the Stream.Read
            /// (ie. after LwSerialization consumes underlying stream directly)
            // return FillBuffer(buffer, offset, count, ReadPart);

            return ReadPart(buffer, offset, count);
        }

        private int FillBuffer(
            byte[] buffer,
            int offset,
            int count,
            Func<byte[], int, int, int> readPart)
        {
            int result = 0;

            while (count > 0)
            {
                int bytesRead = readPart(buffer, offset, count);

                if (bytesRead == 0)
                    break;

                result += bytesRead;

                offset += bytesRead;
                count -= bytesRead;
            }

            return result;
        }

        private int ReadPart(byte[] buffer, int offset, int count)
        {
            int originalCount = count;

            if (_readFinalBlockCount > 0)
            {
                if (count > _readFinalBlockCount)
                    count = _readFinalBlockCount;

                Buffer.BlockCopy(
                    _readFinalBlock,
                    _readFinalBlockOffset,
                    buffer,
                    offset,
                    count);

                _readFinalBlockOffset += count;
                _readFinalBlockCount -= count;

                return count;
            }

            if (!ReadJumboBlock())
                return 0;

            if (_outputReadCount > 0)
            {
                int bytesToCopyFromOutputBuffer =
                    Math.Min(
                        _outputReadCount,
                        count);

                Buffer.BlockCopy(
                    _outputReadBuffer,
                    _outputReadOffset,
                    buffer,
                    offset,
                    bytesToCopyFromOutputBuffer);

                _outputReadOffset += bytesToCopyFromOutputBuffer;
                _outputReadCount -= bytesToCopyFromOutputBuffer;

                count -= bytesToCopyFromOutputBuffer;

                if (count == 0)
                    return originalCount;

                offset += bytesToCopyFromOutputBuffer;
            }

            while (_inputReadCount > _blockSize &&
                   count >= _blockSize)
            {
                int chunkSize =
                    Math.Min(
                        (count / _blockSize) * _blockSize,
                        _inputReadCount);

                int bytesTransformedInChunk =
                    _decryptor.TransformBlock(
                        _inputReadBuffer,
                        _inputReadOffset,
                        chunkSize,
                        buffer,
                        offset);

                _inputReadOffset += chunkSize;
                _inputReadCount -= chunkSize;

                count -= bytesTransformedInChunk;
                offset += bytesTransformedInChunk;
            }

            if (count < _blockSize)
            {
                while (_inputReadCount > _blockSize)
                {
                    bool outputBufferPopulated =
                        _decryptor.TransformBlock(
                            _inputReadBuffer,
                            _inputReadOffset,
                            _blockSize,
                            _outputReadBuffer,
                            0) > 0;

                    _inputReadOffset += _blockSize;
                    _inputReadCount -= _blockSize;

                    if (outputBufferPopulated)
                    {
                        Buffer.BlockCopy(
                            _outputReadBuffer,
                            0,
                            buffer,
                            offset,
                            count);

                        _outputReadOffset = count;

                        return originalCount;
                    }
                }
            }

            _readFinalBlock =
                _decryptor.TransformFinalBlock(
                    _inputReadBuffer,
                    _inputReadOffset,
                    _inputReadCount);

            _inputReadOffset += _inputReadCount;
            _inputReadCount = 0;

            int bytesToCopyFromFinalBlock =
                Math.Min(
                    count,
                    _readFinalBlock.Length);

            Buffer.BlockCopy(
                _readFinalBlock,
                0,
                buffer,
                offset,
                bytesToCopyFromFinalBlock);

            count -= bytesToCopyFromFinalBlock;

            _readFinalBlockOffset = bytesToCopyFromFinalBlock;

            _readFinalBlockCount = 
                _readFinalBlock.Length -
                bytesToCopyFromFinalBlock;

            return originalCount - count;
        }

        private bool ReadJumboBlock()
        {
            int bytesRead = FillBuffer(
                _inputReadBuffer,
                0,
                _blockSize,
                _innerStream.Read);

            if (bytesRead != _blockSize)
                return false;

            byte[] initialBlock =
                _decryptor.TransformFinalBlock(
                    _inputReadBuffer,
                    0,
                    _blockSize);

            _inputReadCount = BitConverter.ToInt32(initialBlock, 0);

            if (_inputReadCount !=
                    BitConverter.ToInt32(initialBlock, sizeof(int)) ||
                        _inputReadCount !=
                    BitConverter.ToInt32(initialBlock, 2 * sizeof(int)))
                throw new Exception();

            _inputReadCount *= _blockSize;

            bytesRead =
                FillBuffer(
                    _inputReadBuffer,
                    0,
                    _inputReadCount,
                    _innerStream.Read);

            if (bytesRead != _inputReadCount)
                return false;

            _inputReadOffset = 0;
            _outputReadCount = 0;

            _readFinalBlock = null;
            _readFinalBlockCount = 0;

            return true;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int bytesWritten = WriteJumboBlock(buffer, offset, count);

                offset += bytesWritten;
                count -= bytesWritten;
            }
        }

        private int WriteJumboBlock(byte[] buffer, int offset, int count)
        {
            int jumboBlockInputCount =
                Math.Min(MAX_JUMBO_BLOCK_SIZE - _blockSize, count);

            int outputOffset = _blockSize;

            int bulkBlockInputCount = (jumboBlockInputCount / _blockSize) *
                                      _blockSize;

            if (bulkBlockInputCount > 0)
                outputOffset +=
                    _encryptor.TransformBlock(
                        buffer,
                        offset,
                        bulkBlockInputCount,
                        _outputWriteBuffer,
                        outputOffset);

            byte[] finalBlock =
                _encryptor.TransformFinalBlock(
                    buffer,
                    offset + bulkBlockInputCount,
                    jumboBlockInputCount - bulkBlockInputCount);

            finalBlock.CopyTo(_outputWriteBuffer, outputOffset);
            outputOffset += finalBlock.Length;

            int payloadBlocks = (outputOffset - _blockSize) / _blockSize;

            _initialBlockBinaryWriter.Write(payloadBlocks);
            _initialBlockBinaryWriter.Write(payloadBlocks);
            _initialBlockBinaryWriter.Write(payloadBlocks);

            _initialBlockBinaryWriter.Seek(0, SeekOrigin.Begin);

            byte[] initialBlock =
                _encryptor.TransformFinalBlock(
                    _initialBlockInputBuffer,
                    0,
                    _initialBlockInputBuffer.Length);

            initialBlock.CopyTo(_outputWriteBuffer, 0);

            _innerStream.Write(_outputWriteBuffer, 0, outputOffset);

            return jumboBlockInputCount;
        }

        public override bool CanRead
        {
            get
            {
                return _innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _innerStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
