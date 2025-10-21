using System;
using System.IO;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Data
#else
namespace Contal.IwQuick.Data
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class ChunkedMemoryStream : Stream
    {
        private class MemoryChunk
        {
            public byte[] Buffer;
            public MemoryChunk Next;
        }

        private MemoryChunk _chunks;
        private readonly IByteBufferPool _bufferPool;
        private bool _isClosed;
        private MemoryChunk _writeChunk;
        private int _writeOffset;
        private MemoryChunk _readChunk;
        private int _readOffset;

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        private const string ERROR_IS_CLOSED = "Chunked stream is closed";

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get
            {
                if (_isClosed)
                    throw new Exception(ERROR_IS_CLOSED);

                int num = 0;
                MemoryChunk next;

                for (
                    MemoryChunk memoryChunk = _chunks; 
                    memoryChunk != null; 
                    memoryChunk = next)
                {
                    next = memoryChunk.Next;
                    num += 
                        next != null 
                            ? memoryChunk.Buffer.Length 
                            : _writeOffset;
                }

                return num;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get
            {
                if (_isClosed)
                    throw new Exception(ERROR_IS_CLOSED);

                if (_readChunk == null)
                    return 0L;

                int num = 0;

                for (
                        MemoryChunk memoryChunk = _chunks; 
                        memoryChunk != _readChunk; 
                        memoryChunk = memoryChunk.Next)
                    num += memoryChunk.Buffer.Length;

                num += _readOffset;

                return num;
            }

            set
            {
                if (_isClosed)
                    throw new Exception(ERROR_IS_CLOSED);

                if (value < 0L)
                    throw new ArgumentOutOfRangeException("value");

                MemoryChunk readChunk = _readChunk;

                int readOffset = _readOffset;
                _readChunk = null;
                _readOffset = 0;

                int num = (int)value;

                for (
                    MemoryChunk memoryChunk = _chunks; 
                    memoryChunk != null; 
                    memoryChunk = memoryChunk.Next)
                {
                    if (num < memoryChunk.Buffer.Length || (num == memoryChunk.Buffer.Length && memoryChunk.Next == null))
                    {
                        _readChunk = memoryChunk;
                        _readOffset = num;

                        break;
                    }

                    num -= memoryChunk.Buffer.Length;
                }

                if (_readChunk == null)
                {
                    _readChunk = readChunk;
                    _readOffset = readOffset;

                    throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferPool"></param>
        public ChunkedMemoryStream(IByteBufferPool bufferPool)
        {
            _bufferPool = bufferPool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_isClosed)
                throw new Exception(ERROR_IS_CLOSED);

            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;

                case SeekOrigin.Current:
                    Position += offset;
                    break;

                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }

            return Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                _isClosed = true;

                if (disposing)
                    ReleaseMemoryChunks(_chunks);

                _chunks = null;
                _writeChunk = null;
                _readChunk = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_isClosed)
                throw new Exception(ERROR_IS_CLOSED);

            if (_readChunk == null)
            {
                if (_chunks == null)
                    return 0;

                _readChunk = _chunks;
                _readOffset = 0;
            }

            byte[] buffer2 = _readChunk.Buffer;
            int num = buffer2.Length;

            if (_readChunk.Next == null)
                num = _writeOffset;

            int num2 = 0;

            while (count > 0)
            {
                if (_readOffset == num)
                {
                    if (_readChunk.Next == null)
                        break;

                    _readChunk = _readChunk.Next;
                    _readOffset = 0;

                    buffer2 = _readChunk.Buffer;
                    num = buffer2.Length;

                    if (_readChunk.Next == null)
                        num = _writeOffset;
                }

                int num3 = Math.Min(count, num - _readOffset);

                Buffer.BlockCopy(
                    buffer2, 
                    _readOffset, 
                    buffer, 
                    offset, 
                    num3);
                
                offset += num3;
                count -= num3;

                _readOffset += num3;
                num2 += num3;
            }

            return num2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            if (_isClosed)
                throw new Exception(ERROR_IS_CLOSED);

            if (_readChunk == null)
            {
                if (_chunks == null)
                {
                    return 0;
                }
                _readChunk = _chunks;
                _readOffset = 0;
            }

            byte[] buffer = _readChunk.Buffer;
            int num = buffer.Length;

            if (_readChunk.Next == null)
                num = _writeOffset;

            if (_readOffset == num)
            {
                if (_readChunk.Next == null)
                    return -1;

                _readChunk = _readChunk.Next;
                _readOffset = 0;

                buffer = _readChunk.Buffer;
            }

            return buffer[_readOffset++];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_isClosed)
                throw new Exception(ERROR_IS_CLOSED);

            if (_chunks == null)
            {
                _chunks = AllocateMemoryChunk();

                _writeChunk = _chunks;
                _writeOffset = 0;
            }

            byte[] buffer2 = _writeChunk.Buffer;
            int num = buffer2.Length;

            while (count > 0)
            {
                if (_writeOffset == num)
                {
                    _writeChunk.Next = AllocateMemoryChunk();
                    _writeChunk = _writeChunk.Next;
                    _writeOffset = 0;

                    buffer2 = _writeChunk.Buffer;
                    num = buffer2.Length;
                }

                int num2 = Math.Min(count, num - _writeOffset);

                Buffer.BlockCopy(
                    buffer, 
                    offset, 
                    buffer2, 
                    _writeOffset, 
                    num2);

                offset += num2;
                count -= num2;

                _writeOffset += num2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void WriteByte(byte value)
        {
            if (_isClosed)
                throw new Exception(ERROR_IS_CLOSED);

            if (_chunks == null)
            {
                _chunks = AllocateMemoryChunk();
                _writeChunk = _chunks;
                _writeOffset = 0;
            }

            byte[] buffer = _writeChunk.Buffer;
            int num = buffer.Length;

            if (_writeOffset == num)
            {
                _writeChunk.Next = AllocateMemoryChunk();
                _writeChunk = _writeChunk.Next;

                _writeOffset = 0;

                buffer = _writeChunk.Buffer;
            }

            buffer[_writeOffset++] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToArray()
        {
            int count = (int)Length;
            byte[] array = new byte[Length];

            MemoryChunk readChunk = _readChunk;

            int readOffset = _readOffset;
            _readChunk = _chunks;
            _readOffset = 0;

            Read(array, 0, count);

            _readChunk = readChunk;
            _readOffset = readOffset;

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public virtual void WriteTo(Stream stream)
        {
            if (_isClosed)
                throw new Exception(ERROR_IS_CLOSED);

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (_readChunk == null)
            {
                if (_chunks == null)
                    return;

                _readChunk = _chunks;
                _readOffset = 0;
            }

            byte[] buffer = _readChunk.Buffer;
            int num = buffer.Length;

            if (_readChunk.Next == null)
                num = _writeOffset;

            while (true)
            {
                if (_readOffset == num)
                {
                    if (_readChunk.Next == null)
                        break;

                    _readChunk = _readChunk.Next;
                    _readOffset = 0;

                    buffer = _readChunk.Buffer;
                    num = buffer.Length;

                    if (_readChunk.Next == null)
                        num = _writeOffset;
                }

                int count = num - _readOffset;

                stream.Write(
                    buffer, 
                    _readOffset, 
                    count);

                _readOffset = num;
            }
        }

        private MemoryChunk AllocateMemoryChunk()
        {
            return new MemoryChunk
            {
                Buffer = _bufferPool.GetBuffer(),
                Next = null
            };
        }

        private void ReleaseMemoryChunks(MemoryChunk chunk)
        {
            while (chunk != null)
            {
                _bufferPool.ReturnBuffer(chunk.Buffer);
                chunk = chunk.Next;
            }
        }
    }
}
