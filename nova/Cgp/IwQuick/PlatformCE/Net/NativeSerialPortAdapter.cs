using System;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Net
{
    public class NativeSerialPortAdapter : ISerialPortAdapter
    {

        private const string DLL_PATH = "NativeSuperSerial.dll";

        public string PortName
        {
            get; 
            private set;
        }

        [DllImport(DLL_PATH)]
        private static extern IntPtr NativeSuperSerial_Open(string portName, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_Configure(IntPtr comPort, int baudRate, int dataBits, int parity, int stopBits, int handshaking, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_UseTimeouts(IntPtr comPort, UInt32 readIntervalTimeout, UInt32 readTimeoutConstant, UInt32 readTimeoutMultiplier, UInt32 writeTimeoutConstant, UInt32 writeTimeoutMultiplier, ref  int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_DoNotUseTimeouts(IntPtr comPort, ref  int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_WriteExtended(IntPtr comPort, byte[] buffer, int offset, int length, int bufferSize, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_WaitForTxEmpty(IntPtr comPort, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_ReadExtended(IntPtr comPort, byte[] buffer, int offset, int length, int bufferSize, int useRxCharWaiting, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_GetBytesToRead(IntPtr comPort, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_Close(IntPtr comPort, ref int resultCode);

        private IntPtr _commHandle;

        public NativeSerialPortAdapter(string portName)
        {
            PortName = portName;
        }

        public bool Open()
        {
            int resultCode = -1;

            _commHandle = NativeSuperSerial_Open(
                PortName,
                ref resultCode);

            return resultCode == 0;
        }

        public bool Configure(
            int baudRate,
            int dataBits,
            Parity parity,
            StopBits stopBits)
        {
            int resultCode = -1;

            int nativeStopBits;

            switch (stopBits)
            {
                case StopBits.One:
                    nativeStopBits = 0;
                    break;
                case StopBits.OnePointFive:
                    nativeStopBits = 1;
                    break;
                case StopBits.Two:
                    nativeStopBits = 2;
                    break;
                default:
                    nativeStopBits = 0;
                    break;
            }

            NativeSuperSerial_Configure(
                _commHandle,
                baudRate,
                dataBits,
                (int)parity,
                nativeStopBits,
                0,
                ref resultCode);

            return resultCode == 0;
        }

        public bool SetTimeouts(SerialPortTimeouts timeouts)
        {
            int resultCode = -1;

            if (timeouts.UseTimeouts)
            {
                NativeSuperSerial_UseTimeouts(
                    _commHandle,
                    timeouts.ReadIntervalTimeout,
                    timeouts.ReadTimeoutConstant,
                    timeouts.ReadTimeoutMultiplier,
                    timeouts.WriteTimeoutConstant,
                    timeouts.WriteTimeoutMultiplier,
                    ref resultCode);
            }
            else
                NativeSuperSerial_DoNotUseTimeouts(
                    _commHandle,
                    ref resultCode);

            return resultCode == 0;
        }

        public bool Read(
            byte[] buffer,
            int offset,
            int length,
            out int bytesRead)
        {
            int resultCode = -1;

            bytesRead = NativeSuperSerial_ReadExtended(
                _commHandle,
                buffer,
                offset,
                length,
                buffer.Length,
                0,
                ref resultCode);

            return resultCode == 0;
        }

        public void PurgeRx()
        {
        }

        public bool BlockingWrite(
            byte[] buffer,
            int offset,
            int length)
        {
            int resultCode = -1;

            NativeSuperSerial_WriteExtended(
                _commHandle,
                buffer,
                offset,
                length,
                buffer.Length,
                ref resultCode);

            if (resultCode != 0)
                return false;

            NativeSuperSerial_WaitForTxEmpty(
                _commHandle,
                ref resultCode);

            return resultCode == 0;
        }

        public bool Write(
            byte[] buffer,
            int offset,
            int length)
        {
            int resultCode = -1;

            NativeSuperSerial_WriteExtended(
                _commHandle,
                buffer,
                offset,
                length,
                buffer.Length,
                ref resultCode);

            return resultCode == 0;
        }

        public bool GetBytesToRead(out int bytesToRead)
        {
            int resultCode = -1;

            bytesToRead = NativeSuperSerial_GetBytesToRead(
                _commHandle,
                ref resultCode);

            return resultCode == 0;
        }

        public bool IsOpen
        {
            get { return !_commHandle.Equals(IntPtr.Zero); }
        }

        public bool Close()
        {
            try
            {
                int resultCode = -1;

                return NativeSuperSerial_Close(
                    _commHandle,
                    ref resultCode) == 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                _commHandle = IntPtr.Zero;
            }
        }
    }
}
