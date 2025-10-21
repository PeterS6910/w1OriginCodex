using System;
using System.IO;
using JetBrains.Annotations;


namespace Contal.IwQuick.Data
{
    public class QuantizedStreamReader : Stream
    {
        private readonly Stream _stream;

        public QuantizedStreamReader([NotNull] Stream stream)
        {
            _stream = stream;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public bool IsSameInnerStream([NotNull] Stream stream)
        {
            return ReferenceEquals(stream, _stream);
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
            int read = 0;
            while (read < count)
            {
                int temp = _stream.Read(buffer, offset + read, count - read);

                if (temp <= 0)
                    return temp;

                read += temp;
            }

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }

            set { throw new NotSupportedException(); }
        }
    }
}