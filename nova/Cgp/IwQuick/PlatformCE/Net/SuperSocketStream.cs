using System;
using System.IO;
using System.Threading;

namespace Contal.IwQuick.Net
{
    internal class SuperSocketStream : Stream
    {
        private readonly bool _ownsSocket;
        private bool _readable;
        private bool _writeable;
        private readonly ISuperSocket _superSocket;
        private bool _cleanedUp;

        public SuperSocketStream(
            ISuperSocket superSocket,
            FileAccess fileAccess,
            bool ownsSocket)
        {
            _superSocket = superSocket;

            switch (fileAccess)
            {
                case FileAccess.Read:
                    _readable = true;
                    break;
                case FileAccess.Write:
                    _writeable = true;
                    break;
                case FileAccess.ReadWrite:
                    _readable = true;
                    _writeable = true;
                    break;
            }

            _ownsSocket = ownsSocket;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_cleanedUp && disposing && _superSocket != null)
            {
                _readable = false;
                _writeable = false;

                if (_ownsSocket)
                {
                    ISuperSocket superSocket = _superSocket;

                    if (superSocket != null)
                        superSocket.Close();
                }
            }

            _cleanedUp = true;

            base.Dispose(disposing);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("net_noseek");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("net_noseek");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_cleanedUp)
                throw new ObjectDisposedException(GetType().FullName);

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("count");

            if (!CanRead)
                throw new InvalidOperationException("net_writeonlystream");

            ISuperSocket superSocket = _superSocket;

            if (superSocket == null)
                throw new IOException("net_io_readfailure");

            int result;

            try
            {
                result = superSocket.Receive(buffer, offset, count);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                    throw;

                throw new IOException(
                    "net_io_readfailure", 
                    ex);
            }

            return result;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_cleanedUp)
                throw new ObjectDisposedException(GetType().FullName);

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("count");

            if (!CanWrite)
                throw new InvalidOperationException("net_readonlystream");

            ISuperSocket superSocket = _superSocket;

            if (superSocket == null)
                throw new IOException("net_io_writefailure");

            try
            {
                superSocket.Send(buffer, offset, count);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
                {
                    throw;
                }
                throw new IOException("net_io_writefailure", ex);
            }
            catch
            {
                throw new IOException(
                    "net_io_writefailure", 
                    new Exception("net_nonClsCompliantException"));
            }

        }

        public override bool CanRead
        {
            get
            {
                return _readable;
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
                return _writeable;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException("net_noseek");
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException("net_noseek");
            }
            set
            {
                throw new NotSupportedException("net_noseek");
            }
        }
    }
}