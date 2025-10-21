using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using Contal.IwQuick.Net.Microsoft;

namespace Contal.IwQuick.Net
{
    public class SystemSerialPortAdapter 
        : SystemSerialPortAdapterBase
        , ISerialPortAdapter
    {
        private readonly string _portName;
        private IntPtr _handle;

        private const Int32 INVALID_HANDLE_VALUE = -1;

        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 GENERIC_WRITE = 0x40000000;

        private const UInt32 OPEN_EXISTING = 3;

        private const UInt32 PURGE_TXABORT = 0x00000001;
        private const UInt32 PURGE_RXABORT = 0x00000002;
        private const UInt32 PURGE_TXCLEAR = 0x00000004;
        private const UInt32 PURGE_RXCLEAR = 0x00000008;

        [DllImport(DllName, SetLastError = true)]
        private static extern IntPtr CreateFile(
            String lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport(DllName)]
        private static extern Boolean CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct COMMTIMEOUTS
        {
            internal UInt32 ReadIntervalTimeout;
            internal UInt32 ReadTotalTimeoutMultiplier;
            internal UInt32 ReadTotalTimeoutConstant;
            internal UInt32 WriteTotalTimeoutMultiplier;
            internal UInt32 WriteTotalTimeoutConstant;
        }

        private const int FlagParity = (1 << 1);

        [StructLayout(LayoutKind.Sequential)]
        private struct DCB
        {
            internal Int32 DCBlength;
            internal Int32 BaudRate;
            internal Int32 PackedValues;
            internal Int16 wReserved;
            internal Int16 XonLim;
            internal Int16 XoffLim;
            internal Byte ByteSize;
            internal Byte Parity;
            internal Byte StopBits;
            internal Byte XonChar;
            internal Byte XoffChar;
            internal Byte ErrorChar;
            internal Byte EofChar;
            internal Byte EvtChar;
            internal Int16 wReserved1;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COMSTAT
        {
            internal UInt32 Flags;
            internal UInt32 cbInQue;
            internal UInt32 cbOutQue;
        }

        [DllImport(DllName)]
        private static extern Boolean SetCommTimeouts(
            IntPtr hFile,
            [In] ref COMMTIMEOUTS lpCommTimeouts);

        [DllImport(DllName)]
        private static extern Boolean SetupComm(
            IntPtr hFile,
            UInt32 dwInQueue,
            UInt32 dwOutQueue);

        [DllImport(DllName)]
        private static extern Boolean GetCommState(
            IntPtr hFile,
            ref DCB lpDCB);

        [DllImport(DllName)]
        private static extern Boolean SetCommState(
            IntPtr hFile,
            [In] ref DCB lpDCB);

        [DllImport(DllName)]
        public static extern UInt32 GetLastError();

        [DllImport(DllName)]
        private static extern Boolean ClearCommError(
            IntPtr hFile,
            out UInt32 lpErrors,
            out COMSTAT cs);

        [DllImport(DllName, SetLastError = true)]
        private static extern Boolean WriteFile(
            IntPtr fFile,
            IntPtr lpBuffer,
            UInt32 nNumberOfBytesToWrite,
            out UInt32 lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport(DllName, SetLastError = true)]
        private static extern Boolean ReadFile(
            IntPtr hFile,
            IntPtr lpBuffer,
            UInt32 nNumberOfBytesToRead,
            out UInt32 nNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport(DllName, SetLastError = true)]
        private static extern bool PurgeComm(
            IntPtr hFile,
            UInt32 dwFlags);

        public SystemSerialPortAdapter(string portName)
        {
            _portName = portName;
        }

        public bool Open()
        {
            _handle = CreateFile(
                _portName,
                 GENERIC_READ | GENERIC_WRITE,
                 0,
                 IntPtr.Zero,
                 OPEN_EXISTING,
                 0,
                 IntPtr.Zero);

            if (_handle == (IntPtr) INVALID_HANDLE_VALUE)
                return false;

            //if (!SetupComm(_handle, 2048, 2048))
            //    return false;

            return true;
        }

        public bool Configure(
            int baudRate, 
            int dataBits, 
            Parity parity, 
            StopBits stopBits)
        {
            var dcb = new DCB();

            if (!GetCommState(_handle, ref dcb))
                return false;

            dcb.BaudRate = baudRate;
            dcb.ByteSize = (byte) dataBits;

            switch (stopBits)
            {
                case StopBits.None:
                case StopBits.One:
                    dcb.StopBits = 0;
                    break;

                case StopBits.OnePointFive:
                    dcb.StopBits = 1;
                    break;

                case StopBits.Two:
                    dcb.StopBits = 2;
                    break;
            }

            if (parity != Parity.None)
            {
                dcb.PackedValues |= FlagParity;
                dcb.Parity = (byte) parity;
            }

            return SetCommState(_handle, ref dcb);
        }

        public bool SetTimeouts(SerialPortTimeouts timeouts)
        {
            var commTimeouts = timeouts.UseTimeouts
                ? new COMMTIMEOUTS
                {
                    ReadIntervalTimeout = timeouts.ReadIntervalTimeout,
                    ReadTotalTimeoutConstant = timeouts.ReadTimeoutConstant,
                    ReadTotalTimeoutMultiplier = timeouts.ReadTimeoutMultiplier,
                    WriteTotalTimeoutConstant = timeouts.WriteTimeoutConstant,
                    WriteTotalTimeoutMultiplier = timeouts.WriteTimeoutMultiplier
                }
                : new COMMTIMEOUTS
                {
                    ReadIntervalTimeout = UInt32.MaxValue,
                    ReadTotalTimeoutConstant = 0,
                    ReadTotalTimeoutMultiplier = 0,
                    WriteTotalTimeoutConstant = 0,
                    WriteTotalTimeoutMultiplier = 0
                };

            return SetCommTimeouts(_handle, ref commTimeouts);
        }

        public bool Read(byte[] buffer, int offset, int length, out int bytesRead)
        {
            uint comError;
            COMSTAT comStatus;

            if (!ClearCommError(
                _handle,
                out comError,
                out comStatus))
            {
                bytesRead = 0;
                return false;
            }

            uint bytesReadArg;

            var bufferHandle = GCHandle.Alloc(
                buffer,
                GCHandleType.Pinned);

            IntPtr bufferPtr =
                IntPtr.Size == 4
                    ? new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt32() + offset)
                    : new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt64() + offset);

            bool result;

            try
            {
                result = ReadFile(
                    _handle,
                    bufferPtr,
                    (uint)length,
                    out bytesReadArg,
                    IntPtr.Zero);
            }
            finally
            {
                bufferHandle.Free();
            }

            bytesRead = (int) bytesReadArg;
            return result;
        }

        public void PurgeRx()
        {
            uint comError;
            COMSTAT comStatus;

            if (!ClearCommError(
                _handle,
                out comError,
                out comStatus))
            {
                return;
            }

            PurgeComm(
                _handle,
                PURGE_RXCLEAR);
        }

        public bool BlockingWrite(byte[] buffer, int offset, int length)
        {
            if (!Write(
                buffer,
                offset,
                length))
            {
                return false;
            }

            do
            {
                uint commErrors;
                COMSTAT comStat;

                if (!ClearCommError(
                    _handle, 
                    out commErrors, 
                    out comStat))
                {
                    return false;
                }

                if (comStat.cbOutQue == 0)
                    return true;

                Thread.Sleep(1);
            } while (true);
        }

        public bool Write(
            byte[] buffer, 
            int offset, 
            int length)
        {
            uint comError;
            COMSTAT comStatus;

            if (!ClearCommError(
                _handle,
                out comError,
                out comStatus))
            {
                return false;
            }

            var bufferHandle = GCHandle.Alloc(
                buffer,
                GCHandleType.Pinned);

            IntPtr bufferPtr =
                IntPtr.Size == 4
                    ? new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt32() + offset)
                    : new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt64() + offset);

            bool result;

            try
            {
                uint bytesWrittenArg;

                result = WriteFile(
                    _handle,
                    bufferPtr,
                    (uint)length,
                    out bytesWrittenArg,
                    IntPtr.Zero);
            }
            finally
            {
                bufferHandle.Free();
            }

            return result;
        }

        public bool GetBytesToRead(out int bytesToRead)
        {
            uint commErrors;
            COMSTAT comStat;

            if (!ClearCommError(_handle, out commErrors, out comStat))
            {
                bytesToRead = 0;
                return false;
            }

            bytesToRead = (int) comStat.cbInQue;
            return true;
        }

        public bool IsOpen
        {
            get { return !_handle.Equals(IntPtr.Zero); }
        }

        public bool Close()
        {
            try
            {
                return CloseHandle(_handle);
            }
            catch
            {
                return false;
            }
            finally
            {
                _handle = IntPtr.Zero;
            }
        }
    }
}
