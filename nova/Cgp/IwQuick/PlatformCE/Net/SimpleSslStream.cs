using System;

using System.IO;
using System.Net.Sockets;

using System.Threading;

namespace Contal.IwQuick.Net
{
    class SslStreamAsyncResult : IAsyncResult
    {
        #region IAsyncResult Members

        public static SslStreamAsyncResult Create(object asyncState)
        {
            SslStreamAsyncResult i = new SslStreamAsyncResult();

            i._asyncState = asyncState;
            i._asyncWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);



            return i;
        }

        private SslStreamAsyncResult()
        {
        }

        private object _asyncState = null;

        public object AsyncState
        {
            get { return _asyncState; }
        }

        private EventWaitHandle _asyncWaitHandle = null;
        public WaitHandle AsyncWaitHandle
        {
            get { return _asyncWaitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        private bool _isCompleted = false;
        protected internal void Complete()
        {
            if (_isCompleted)
                return;

            _isCompleted = _asyncWaitHandle.Set();

        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        #endregion
    }

    public class SimpleSslStream : Stream
    {
        private Socket _socket;
        private SslHelper2 _sh;

        public SimpleSslStream(Socket socket,SslSettings sslSettings)
        {
            if (null == socket)
                throw new ArgumentNullException();

            _socket = socket;

            _sh = new SslHelper2(_socket, sslSettings);
        }




        public override bool CanRead
        {
            get
            {
                return _socket.Connected;
            }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return _socket.Connected; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { return _socket.Available; }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _socket.Receive(buffer, offset, count, SocketFlags.None);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {

        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _socket.Send(buffer, offset, count, SocketFlags.None);
        }

        private bool _prepared = false;

        private void ValidatePrepare()
        {
            if (!_prepared)
            {
                byte[] nb = new byte[1];
                int NonBlock = 0;
                unchecked
                {
                    NonBlock = (int)2147772030;
                }
                int ret = _socket.IOControl(NonBlock, new byte[] { 1 }, nb);
                _prepared = true;
            }
        }

        private Thread _readingThread = null;
        private byte[] _readBuffer = null;
        private int _readOffset = 0;
        private int _readSize = 0;
        private SslStreamAsyncResult _readAsyncResult = null;
        private AsyncCallback _onReadCallback = null;


        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (null != _readingThread)
                throw new SocketException();


            _readBuffer = buffer;
            _readOffset = offset;
            _readSize = count;
            _onReadCallback = callback;

            _readAsyncResult = SslStreamAsyncResult.Create(state);

            _readingThread = new Thread(new ThreadStart(ReadingThread));
            _readingThread.IsBackground = true;
            _readingThread.Start();

            return _readAsyncResult;
        }

        private void InvokeReadEnd(bool complete)
        {
            if (_readAsyncResult == null ||
                _onReadCallback == null)
                return;

            if (complete)
                _readAsyncResult.Complete();
            _readingThread = null;
            try
            {
                _onReadCallback.Invoke(_readAsyncResult);
            }
            catch
            {
            }
        }

        private void ReadingThread()
        {
            try
            {
                while (true)
                {
                    if (_socket.Poll(10, SelectMode.SelectRead))
                    {
                        InvokeReadEnd(true);
                        break;
                    }

                    if (_socket.Poll(10, SelectMode.SelectError))
                    {
                        InvokeReadEnd(false);
                        break;
                    }

                }
            }
            catch
            {
            }
            finally
            {
                _readingThread = null;
            }
        }



        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _socket.BeginSend(buffer, offset, count, SocketFlags.None, callback, state);
        }



        public override int EndRead(IAsyncResult asyncResult)
        {
            try
            {
                lock (_socket)
                {
                    int avail = _socket.Available;
                    if (avail > 0)
                        return _socket.Receive(_readBuffer, _readOffset, avail, SocketFlags.None);
                    else
                        return 0;
                }
            }
            catch
            {
                throw;
            }
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _socket.EndSend(asyncResult);
        }
    }
}
