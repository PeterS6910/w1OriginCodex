using System;

using System.Runtime.InteropServices;

namespace Contal.IwQuickCF.Sys.Microsoft
{
    public class P2PMessageQueueHelper
    {
        public static IntPtr CreateMessageQueue(string queueName, bool readAccess)
        {
            try
            {
                return CreateMsgQueue(queueName,
                    new MsgQueueOptions(readAccess));
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [DllImport("coredll.dll")]
        private static extern IntPtr CreateMsgQueue(string lpszName,
            MsgQueueOptions lpOptions);

        private class MsgQueueOptions
        {
            private const int MSGQUEUE_ALLOW_BROKEN = 0x02;
            public MsgQueueOptions(bool readAccess)
            {
                dwSize = Marshal.SizeOf(typeof(MsgQueueOptions));
                dwFlags = MSGQUEUE_ALLOW_BROKEN;
                dwMaxMessages = 20;
                cbMaxMessage = 100;
                bReadAccess = Convert.ToInt32(readAccess);
            }
            public int dwSize;
            public int dwFlags;
            public int dwMaxMessages;
            public int cbMaxMessage;
            public int bReadAccess;
        }

        public static bool ReadMessageQueue(IntPtr queueHandle, byte[] buffer, int timeout, out int bytesRead)
        {
            if (timeout < 0)
                timeout = 0;

            int flags;
            return ReadMsgQueue(queueHandle, buffer, buffer.Length, out bytesRead, timeout, out flags);
        }

        [DllImport("coredll.dll")]
        private static extern bool ReadMsgQueue(IntPtr hMsgQ, byte[] lpBuffer,
                int cbBufferSize, out int lpNumberOfBytesRead, int dwTimeout,
                out int pdwFlags);


        public static IntPtr OpenMessageQueue(IntPtr hProcess, IntPtr originalQueueHandle, bool readAccess)
        {
            return OpenMsgQueue(hProcess, originalQueueHandle,
                new MsgQueueOptions(readAccess));
        }

        [DllImport("coredll.dll")]
        private static extern IntPtr OpenMsgQueue(IntPtr hSrcProc, IntPtr hMsgQ,
            MsgQueueOptions lpOptions);

        public static bool WriteMessageQueue(IntPtr queueHandle, byte[] buffer,
            int length, int timeout)
        {
            bool ret= WriteMsgQueue(queueHandle, buffer, length, timeout, 0);
            return ret;
        }

        [DllImport("coredll.dll")]
        private static extern bool WriteMsgQueue(IntPtr hMsgQ, byte[] lpBuffer,
            int cbDataSize, int dwTimeout, int dwFlags);

        [DllImport("coredll.dll",EntryPoint="CloseMsgQueue")]
        private static extern bool NativeCloseMsgQueue(IntPtr hMsgQ);

        public static bool CloseMsgQueue(IntPtr hMessageQueue)
        {
            try
            {
                return NativeCloseMsgQueue(hMessageQueue);
            }
            catch
            {
                return false;
            }
        }

    }
}
