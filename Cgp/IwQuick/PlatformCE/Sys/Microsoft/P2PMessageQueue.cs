using System;
using Contal.IwQuickCF.Data;

namespace Contal.IwQuickCF.Sys.Microsoft
{
    public class P2PMessageQueue:IDisposable
    {
        private volatile IntPtr _queueHandle = IntPtr.Zero;

        private int _receiveBufferSize = 1024;
        public int ReceiveBufferSize
        {
            get { return _receiveBufferSize; }
            set
            {
                if (value > 0)
                    _receiveBufferSize = value;
            }
        }

        public bool GetHandles(out IntPtr processHandle, out IntPtr queueHandle)
        {
            processHandle = IntPtr.Zero;
            queueHandle = IntPtr.Zero;
            if (null == _queueHandle)
                return false;

            try
            {
                processHandle = (IntPtr)System.Diagnostics.Process.GetCurrentProcess().Id;
                queueHandle = _queueHandle;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public event DType2Void<Contal.IwQuickCF.Data.ByteDataCarrier> MessageReceived = null;

        private void InvokeMessageReceived(ByteDataCarrier message)
        {
            if (null != message &&
                null != MessageReceived)
            {
                try
                {
                    MessageReceived(message);
                }
                catch
                {
                }
            }
        }

        public bool Open(string queueName)
        {
            if (_queueHandle != IntPtr.Zero)
                return true;

            _receiveBuffer = new byte[_receiveBufferSize];

            _queueHandle = P2PMessageQueueHelper.CreateMessageQueue(queueName, true);
            if (_queueHandle == IntPtr.Zero)
                return false;
            else
            {
                _waitingThread = new Contal.IwQuickCF.Threads.SafeThread(WaitForMessageThread);
                _waitingThread.Start();
                return true;
            }
        }

        private object _disposalLock = new object();
        public void Close()
        {
            lock (_disposalLock)
            {
                if (_queueHandle == IntPtr.Zero)
                    return;

                bool r = P2PMessageQueueHelper.CloseMsgQueue(_queueHandle);
                _queueHandle = IntPtr.Zero;

                if (_waitingThread != null)
                    _waitingThread.Stop(500);

                _waitingThread = null;
                _receiveBuffer = null;

            }
        }

        private Threads.SafeThread _waitingThread = null;
        private byte[] _receiveBuffer = null;
        private void WaitForMessageThread()
        {
            int actSize;
            while (_queueHandle != IntPtr.Zero)
            {
                if (P2PMessageQueueHelper.ReadMessageQueue(_queueHandle, _receiveBuffer, int.MaxValue, out actSize))
                {
                    if (actSize > 0)
                    {
                        ByteDataCarrier bdc = new ByteDataCarrier(_receiveBuffer, actSize);
                        InvokeMessageReceived(bdc);
                    }

                }
            }            
        }

        ~P2PMessageQueue()
        {
            Close();
        }


        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
