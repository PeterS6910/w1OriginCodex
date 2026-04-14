using System;
using System.Runtime.InteropServices;
using Contal.IwQuick;
using JetBrains.Annotations;


namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public static class NativeSuperSerial
    {
        #region NativeSuperSocket interop bindings

        private const string DLL_PATH = "NativeSuperSerial.dll";

        [DllImport(DLL_PATH)]
        private static extern IntPtr NativeSuperSerial_Open(string portName, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_Configure(IntPtr comPort, int baudRate, int dataBits, int parity, int stopBits, int handshaking, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_UseTimeouts(IntPtr comPort, UInt32 readIntervalTimeout, UInt32 readTimeoutConstant, UInt32 readTimeoutMultiplier, UInt32 writeTimeoutConstant, UInt32 writeTimeoutMultiplier, ref  int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_DoNotUseTimeouts(IntPtr comPort, ref  int resultCode);

        [DllImport(DLL_PATH)]
// ReSharper disable once UnusedMember.Local
        private static extern int NativeSuperSerial_Write(IntPtr comPort, byte[] buffer, int bufferSize, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_WriteExtended(IntPtr comPort, byte[] buffer, int offset, int length, int bufferSize, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_WaitForTxEmpty(IntPtr comPort,  ref int resultCode);

        [DllImport(DLL_PATH)]
// ReSharper disable once UnusedMember.Local
        private static extern int NativeSuperSerial_Read(IntPtr comPort, byte[] buffer, int length, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_ReadExtended(IntPtr comPort, byte[] buffer, int offset, int length, int bufferSize, int useRxCharWaiting, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_GetBytesToRead(IntPtr comPort, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSerial_Close(IntPtr comPort,ref int resultCode);

        [DllImport(DLL_PATH)]
        private extern static IntPtr NativeSuperSerial_StartReadingThread(IntPtr callback, IntPtr comPort, IntPtr buffer, int bufferLength);

        [DllImport(DLL_PATH)]
        private extern static int NativeSuperSerial_StopReadingThread(IntPtr readingThread);

        [DllImport(DLL_PATH)]
        private extern static int NativeSuperSerial_ConfigurePhd(
            [In] int comPortNumber,
            [In] byte[] preamble,
            [In] int preambleLength,
            [In] int headerLength,
            [In] int maxOptionalDataLength,
            [In] int optionalDataLengthPosition,
            [In] int dataChecksumPosition,
            [In] int headerChecksumPosition,
            [In] int parsingAbandonTime
            );

        [DllImport(DLL_PATH)]
        private extern static int NativeSuperSerial_ReadPhd(
            [In] IntPtr comPort,   
            [In] int comPortNumber,
            [Out] byte[] headerOutput,
            [Out] byte[] optionalDataOutput,
            ref int checksumValid,
            [In] bool useRxCharWaiting,
            ref int resultCode);

        [DllImport(DLL_PATH)]
        private extern static int NativeSuperSerial_GetVersion();
        #endregion

        #region Win32 Comm bindings

        private const UInt32 PURGE_TXABORT = 0x00000001;
        private const UInt32 PURGE_RXABORT = 0x00000002;
        private const UInt32 PURGE_TXCLEAR = 0x00000004;
        private const UInt32 PURGE_RXCLEAR = 0x00000008;

        [StructLayout(LayoutKind.Sequential)]
        private struct COMSTAT
        {
            internal UInt32 Flags;
            internal UInt32 cbInQue;
            internal UInt32 cbOutQue;
        }

        [DllImport("coredll.dll")]
        private static extern Boolean ClearCommError(
            IntPtr hFile,
            out UInt32 lpErrors,
            out COMSTAT cs);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool PurgeComm(
            IntPtr hFile,
            UInt32 dwFlags);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortNumber"></param>
        /// <returns></returns>
        public static IntPtr Open(int serialPortNumber)
        {
            int resultCode = -1;
            IntPtr comPort;

            string portName = "COM"+serialPortNumber+":";
          
            comPort  = NativeSuperSerial_Open(portName,ref resultCode);

            if (resultCode != 0)
                throw new NativeActionException(resultCode);
            
            return comPort;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        public static void Close(IntPtr comPort)
        {
            if (comPort == IntPtr.Zero)
                return;

            try {

                int rc = 0;
                int closeRet = NativeSuperSerial_Close(comPort,ref rc); 
#if DEBUG
                if (closeRet != 0)
                    System.Diagnostics.Debug.WriteLine("Unable to close native super serial port");
#endif 
                    
            }
            catch{
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="readIntervalTimeout"></param>
        /// <param name="readTimeoutConstant"></param>
        /// <param name="readTimeoutMultiplier"></param>
        /// <param name="writeTimeoutConstant"></param>
        /// <param name="writeTimeoutMultiplier"></param>
        /// <exception cref="NativeActionException"></exception>
        public static void UseTimeouts(
            IntPtr comPort, 
            UInt32 readIntervalTimeout,
            UInt32 readTimeoutConstant, 
            UInt32 readTimeoutMultiplier, 
            UInt32 writeTimeoutConstant, 
            UInt32 writeTimeoutMultiplier)
        {
            Validator.CheckNull(comPort);

            int resultCode = -1;

            NativeSuperSerial_UseTimeouts(comPort, readIntervalTimeout, readTimeoutConstant, readTimeoutMultiplier,
                writeTimeoutConstant, writeTimeoutMultiplier, ref resultCode);

            if (resultCode != 0)
                throw new NativeActionException(resultCode);
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        public static void DoNotUseTimeouts(IntPtr comPort)
        {
            Validator.CheckNull(comPort);

            int resultCode = -1;

            NativeSuperSerial_DoNotUseTimeouts(comPort, ref resultCode);

            if (resultCode != 0)
                throw new NativeActionException(resultCode);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public static void Configure(
            IntPtr comPort, 
            int baudRate, 
            int dataBits, 
            System.IO.Ports.Parity parity, 
            System.IO.Ports.StopBits stopBits)
        {
            Validator.CheckNull(comPort);

            if (baudRate <= 0)
                baudRate = 9600;

            if (dataBits < 4 || dataBits > 8)
                dataBits = 8;

            int resultCode = -1;

            int nativeStopBits;
            // IMPORTANT CAST - .NET raw values are not the same as Native !
            switch (stopBits)
            {
                case System.IO.Ports.StopBits.One:
                    nativeStopBits = 0;
                    break;
                case System.IO.Ports.StopBits.OnePointFive:
                    nativeStopBits = 1;
                    break;
                case System.IO.Ports.StopBits.Two:
                    nativeStopBits = 2;
                    break;
                default:
                    nativeStopBits = 0;
                    break;
            }

            NativeSuperSerial_Configure(comPort,baudRate,dataBits,(int)parity,nativeStopBits,0,ref resultCode);

            if (resultCode != 0)
                throw new NativeActionException(resultCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static void Write(
            IntPtr comPort, 
            [NotNull] byte[] data,
            int offset, 
            int length)
        {
            Validator.CheckForNull(data,"data");
            Validator.CheckIntegerRange(offset, 0, data.Length - 1);
            Validator.CheckIntegerRange(offset + length, offset + 1, data.Length); 

            int resultCode = -1;

            NativeSuperSerial_WriteExtended(comPort, data, offset, length, data.Length, ref resultCode);
            
            if (resultCode != 0)
                throw new NativeActionException(resultCode);
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        public static void WaitForTxEmpty(IntPtr comPort)
        {
            int resultCode = -1;

            NativeSuperSerial_WaitForTxEmpty(comPort,  ref resultCode);

            if (resultCode != 0)
                throw new NativeActionException(resultCode);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="data"></param>
        /// <param name="useRxCharWaiting"></param>
        /// <param name="resultCode"></param>
        /// <exception cref="NativeActionException"></exception>
        /// <returns></returns>
        public static int Read(
            IntPtr comPort, 
            [NotNull] byte[] data, 
            bool useRxCharWaiting, 
            ref int resultCode)
        {
            Validator.CheckForNull(data,"data");

            int bytesRead=0;

            try
            {
                bytesRead = NativeSuperSerial_ReadExtended(comPort, data, -1, -1, data.Length, useRxCharWaiting ? 1 : 0, ref resultCode);
            }
            catch//(Exception e)
            {
                // THIS SHOULD BE ONLY NATIVE ERROR
                //bytesRead = NativeSuperSerial_Read(comPort, data.Buffer, data.Size, ref resultCode);

            }
            
            if (bytesRead == -1 && resultCode != 0)
            {
                throw new NativeActionException(resultCode);
            }
            
            //data.ActualSize = bytesRead;
            return bytesRead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="data"></param>
        /// <param name="useRxCharWaiting"></param>
        /// <exception cref="NativeActionException"></exception>
        /// <returns></returns>
        public static int Read(
            IntPtr comPort, 
            [NotNull] byte[] data, 
            bool useRxCharWaiting)
        {
            int resultCode = -1;
            return Read(comPort, data, useRxCharWaiting, ref resultCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        /// <exception cref="NativeActionException"></exception>
        public static int GetBytesToRead(IntPtr comPort)
        {
            int resultCode = -1;
            int bytes2Read;

            bytes2Read = NativeSuperSerial_GetBytesToRead(comPort, ref resultCode);

            if (resultCode != 0)
            {
                throw new NativeActionException(resultCode);
            }
            
            return bytes2Read;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="receivedBytesCount"></param>
        /// <param name="resultCode"></param>
        public delegate void DReceivedDataHandler(IntPtr bytes,int receivedBytesCount, int resultCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="bufferSize"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IntPtr StartReadingThread(IntPtr comPort, int bufferSize, IntPtr callback)
        {
            Validator.CheckNull(comPort);
            //Validator.CheckNull(receiveBuffer);
            Validator.CheckNull(callback);

            return NativeSuperSerial_StartReadingThread(callback , comPort, IntPtr.Zero, bufferSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadHandle"></param>
        /// <exception cref="NativeActionException"></exception>
        public static void StopReadingThread(IntPtr threadHandle)
        {
            Validator.CheckNull(threadHandle);

            int resultCode = NativeSuperSerial_StopReadingThread(threadHandle);
            if (resultCode != 0)
                throw new NativeActionException(resultCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPortNumber"></param>
        /// <param name="preamble"></param>
        /// <param name="headerLength"></param>
        /// <param name="maxOptionalDataLength"></param>
        /// <param name="optionalDataLengthPosition"></param>
        /// <param name="dataCrc8Position"></param>
        /// <param name="headerCrc8Position"></param>
        /// <param name="parsingAbandonTime"></param>
        /// <exception cref="NativeActionException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ConfigurePhd(
            int comPortNumber,
            [NotNull] byte[] preamble, 
            int headerLength, 
            int maxOptionalDataLength,
            int optionalDataLengthPosition, 
            int dataCrc8Position, 
            int headerCrc8Position, 
            int parsingAbandonTime)
        {
            Validator.CheckForNull(preamble,"preamble");

            int version =  NativeSuperSerial_GetVersion();
            DebugHelper.Keep(version);

            int result = NativeSuperSerial_ConfigurePhd(
                comPortNumber,
                preamble, preamble.Length, 
                headerLength, 
                maxOptionalDataLength, 
                optionalDataLengthPosition, dataCrc8Position, headerCrc8Position,
                parsingAbandonTime);

            if (result != 0)
                throw new NativeActionException(result);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="comPortNumber"></param>
        /// <param name="headerOutput"></param>
        /// <param name="optionalDataOutput"></param>
        /// <param name="checksumValid"></param>
        /// <param name="useRxCharWaiting"></param>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        /// <exception cref="NativeActionException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static int ReadPhd(
            IntPtr comPort,
            int comPortNumber,
            [NotNull] byte[] headerOutput,
            [NotNull] byte[] optionalDataOutput, 
            ref bool checksumValid, 
            bool useRxCharWaiting, 
            ref int resultCode)
        {
            Validator.CheckForNull(headerOutput,"headerOutput");
            Validator.CheckForNull(optionalDataOutput,"optionalDataOutput");

            int chksValid = 0;
            int optionalDataLength = NativeSuperSerial_ReadPhd(comPort, comPortNumber, headerOutput, optionalDataOutput, ref chksValid, useRxCharWaiting, ref resultCode);

            if (optionalDataLength < 0)
                throw new NativeActionException(resultCode);
            
            if (chksValid == 0)
                checksumValid = false;
            else
                if (chksValid == 1)
                    checksumValid = true;

            return optionalDataLength;
        }

        public static void PurgeRx(IntPtr comPort)
        {
            uint comError;
            COMSTAT comStatus;

            if (!ClearCommError(
                comPort,
                out comError,
                out comStatus))
            {
                return;
            }

            PurgeComm(
                comPort,
                PURGE_RXCLEAR);
        }
    }
}
