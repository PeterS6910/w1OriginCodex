using System.IO.Ports;

namespace Contal.IwQuick.Net
{
    public class SerialPortTimeouts
    {
        public bool UseTimeouts;
        public uint ReadIntervalTimeout;
        public uint ReadTimeoutConstant;
        public uint ReadTimeoutMultiplier;
        public uint WriteTimeoutConstant;
        public uint WriteTimeoutMultiplier;
    }

    public interface ISerialPortAdapter
    {
        bool Open();

        bool Configure(
            int baudRate,
            int dataBits,
            Parity parity,
            StopBits stopBits);

        bool SetTimeouts(SerialPortTimeouts timeouts);

        bool Read(
            byte[] buffer,
            int offset,
            int length,
            out int bytesRead);

        void PurgeRx();

        bool BlockingWrite(
            byte[] buffer,
            int offset,
            int length);

        bool Write(
            byte[] buffer,
            int offset,
            int length);

        bool GetBytesToRead(out int bytesToRead);

        bool IsOpen { get; }
        bool Close();
    }
}