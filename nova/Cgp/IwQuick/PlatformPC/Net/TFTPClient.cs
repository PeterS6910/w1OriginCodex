using System;
using System.Text;
using System.Net;
using System.IO;
using Contal.IwQuick.Data;
using System.Threading;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public enum Opcodes
    {
        RRQ = 1,
        WRG = 2,
        DATA = 3,
        ACK = 4,
        ERROR = 5
    }

    public enum Modes
    {
        Unknown = 0,
        NetAscii = 1,
        Octet = 2,
        Mail = 3
    }

    public enum RequestTypes
    {
        Read = 0,
        Write = 1
    }

    public class TFTPClient : ADisposable
    {
        private int _progressStep = 1;

        public int ProgressStep
        {
            get { return _progressStep; }
            set { _progressStep = value; }
        }

        private int _lastSentProgress = 0;

        private int _dataBufferLength = 16384;

        public int DataBufferLength
        {
            get { return _dataBufferLength; }
            set { _dataBufferLength = value; }
        }

        private int _resendDelay = 100;
        public int ResendDelay
        {
            get { return _resendDelay; }
            set { _resendDelay = value; }
        }

        private int _maxResendCount = 15;
        public int MaxResendCount
        {
            get { return _maxResendCount; }
            set { _maxResendCount = value; }
        }

        int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        SimpleUdpPeer _client = null;
        volatile bool _lastBlock;
        IPAddress _serverIPAddress = null;
        FileStream _fileToWrite = null;
        Stream _fileToRead = null;
        string _fileNameToRead = string.Empty;
        string _saveToPath = string.Empty;

        readonly AutoResetEvent _are = new AutoResetEvent(false);
        int _blockNumber;
        ByteDataCarrier _dataToSend = null;

        public TFTPClient(int port)
        {
            _port = port;
        }

        public void Start()
        {
            Start(0);
        }

        public void Start(int dataBufferlength)
        {
            if (_client == null)
            {
                _resendNewData.Reset();
                _confirmed.Reset();
                SafeThread.StartThread(ResendingThread);
                if (dataBufferlength > 0)
                    _dataBufferLength = dataBufferlength;
                if (_receivedQueue == null)
                {
                    _receivedQueue = new ProcessingQueue<DataToIpe>();
                    _receivedQueue.ItemProcessing += ProcessDataReceived;
                }
                _client = new SimpleUdpPeer(false);
                _client.DataReceived += DataReceived;
                _client.Start();
            }
        }

        readonly AutoResetEvent _resendNewData = new AutoResetEvent(false);
        readonly AutoResetEvent _confirmed = new AutoResetEvent(false);
        private void ResendingThread()
        {
            try
            {
                while (true)
                {
                    _resendNewData.WaitOne();
                    bool confirmed = false;
                    for (int i = 0; i < _maxResendCount; i++)
                    {
                        if (_client == null)
                            return;
                        if (_serverIPAddress != null && _dataToSend != null)
                        {
                            lock (_client)
                            {
                                _client.Send(new IPEndPoint(_serverIPAddress, _port), _dataToSend);
                            }
                            lock (_client)
                            {
                                bool confirm = _confirmed.WaitOne(_resendDelay, false);
                                if (confirm)
                                {
                                    if (_lastBlock)
                                    {
                                        _are.Set();
                                        return;
                                    }
                                    confirmed = true;
                                    break;
                                }
                            }
                        }

                    }
                    if (!confirmed)
                    {
                        _are.Set();
                        return;
                    }
                }
            }
            catch
            {
                _are.Set();
            }
        }

        public void Stop()
        {
            if (_client != null)
            {
                try
                {
                    _client.Stop();
                    _client.Dispose();
                }
                catch { }
                _are.Set();
                _client = null;
            }

            try
            {
                _receivedQueue.Clear();
                _receivedQueue.Dispose();
            }
            catch
            { }

            try
            {
                _resendNewData.Set();
            }
            catch
            { }

            CloseReadFile();
            CloseWriteFile();
        }

        public event Action<IPAddress, int> OnPercentSent;

        public bool SendFile(string filename, string savePath, string address, out Exception ex)
        {
            try
            {
                _fileToRead = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, _dataBufferLength);
                return SendFile(Path.GetFileName(filename), _fileToRead, savePath, address, out ex);
            }
            catch (Exception error)
            {
                if (_onSendFinish != null)
                    _onSendFinish(filename, savePath, address, false, error);
                ex = error;
                return false;
            }
            finally
            {
                try
                {
                    if (_fileToRead != null)
                        CloseReadFile();
                }
                catch { }
            }
        }

        public bool SendFile(string filename, Stream readStream, string savePath, string address, out Exception ex)
        {
            try
            {
                ex = null;
                try
                {
                    _serverIPAddress = IPAddress.Parse(address);
                    _fileToRead = readStream;
                }
                catch (Exception e)
                {
                    if (_onSendFinish != null)
                        _onSendFinish(filename, savePath, address, false, e);
                    ex = e;
                    return false;
                }
                _lastBlock = false;
                lock (_client)
                {
                    _blockNumber = 0;
                    _dataToSend = new ByteDataCarrier(CreateRequest(_serverIPAddress, savePath + @"\" + filename, RequestTypes.Write));
                    _resendNewData.Set();
                }

                _are.WaitOne();

                if (!_lastBlock)
                    ex = new NoResponseException("Host unreacheable (" + _serverIPAddress + ")");
                if (_onSendFinish != null)
                    _onSendFinish(filename, savePath, address, _lastBlock, ex);
                return _lastBlock;
            }
            catch (Exception error)
            {
                if (_onSendFinish != null)
                    _onSendFinish(filename, savePath, address, false, error);
                ex = error;
                return false;
            }
        }

        public delegate void DOnSendFinish(string filename, string savePath, string address, bool success, Exception ex);
        private event DOnSendFinish _onSendFinish;
        public void BeginSendFile(string filename, string savePath, string address, DOnSendFinish finishMethod)
        {
            if (!BeginSendFileCore(filename, savePath, address, finishMethod))
                return;

            SafeThread.StartThread(StartSendFile);
        }

        public void BeginSendFile(string filename, Stream readStream, string savePath, string address, DOnSendFinish finishMethod)
        {
            if (!BeginSendFileCore(filename, savePath, address, finishMethod))
                return;

            _fileToRead = readStream;
            SafeThread.StartThread(StartSendFileWithStream);
        }

        private bool BeginSendFileCore(string filename, string savePath, string address, DOnSendFinish finishMethod)
        {
            _onSendFinish = null;
            _onSendFinish += finishMethod;
            if (!IPAddress.TryParse(address, out _serverIPAddress))
            {
                if (finishMethod != null)
                    finishMethod(filename, savePath, address, false, new FormatException("IP address " + address + " has invalid format"));
                return false;
            }
            _fileNameToRead = filename;
            _saveToPath = savePath;

            return true;
        }

        private void StartSendFile()
        {
            Exception ex;
            SendFile(_fileNameToRead, _saveToPath, _serverIPAddress.ToString(), out ex);
        }

        private void StartSendFileWithStream()
        {
            Exception ex;
            SendFile(_fileNameToRead, _fileToRead, _saveToPath, _serverIPAddress.ToString(), out ex);
        }

        public bool GetFile(string filename, string savePath, string address, out Exception ex)
        {
            ex = null;
            try
            {
                _serverIPAddress = IPAddress.Parse(address);
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }

            if (!Directory.Exists(savePath))
            {
                ex = new DirectoryNotFoundException("Directory " + savePath + " does not exist");
                return false;
            }

            try
            {
                _fileToWrite = new FileStream(savePath + @"\" + Path.GetFileName(filename), FileMode.Create);
            }
            catch (Exception e)
            {
                ex = e;
                _fileToWrite = null;
                return false;
            }

            _lastBlock = false;

            lock (_client)
            {
                _blockNumber = 0;
                _dataToSend = new ByteDataCarrier(CreateRequest(_serverIPAddress, filename, RequestTypes.Read));
                _resendNewData.Set();
            }

            _are.WaitOne();

            CloseWriteFile();
            if (!_lastBlock)
                ex = new NoResponseException("Host unreacheable (" + _serverIPAddress + ")");
            return _lastBlock;
        }

        ProcessingQueue<DataToIpe> _receivedQueue = null;
        private void DataReceived(ISimpleUdpPeer udpPeer, IPEndPoint ipEndpoint, ByteDataCarrier data)
        {
            try
            {
                _receivedQueue.EnqueueTop(new DataToIpe(ipEndpoint, data));
            }
            catch
            { }
        }

        void ProcessDataReceived(DataToIpe dataToIPE)
        {
            IPEndPoint currentEndPoint = new IPEndPoint(dataToIPE.IPEndPoint.Address, dataToIPE.IPEndPoint.Port);
            if (Equals(dataToIPE.IPEndPoint.Address, _serverIPAddress))
            {
                if (dataToIPE.Data.Length >= 4)
                {
                    Opcodes opcode = (Opcodes)((dataToIPE.Data[0] << 8) + dataToIPE.Data[1]);
                    int blockNumber = (dataToIPE.Data[2] << 8) + dataToIPE.Data[3];
                    switch (opcode)
                    {
                        case Opcodes.RRQ:
                            break;
                        case Opcodes.WRG:
                            break;
                        case Opcodes.DATA:
                            if (blockNumber == _blockNumber + 1)
                            {
                                _confirmed.Set();
                                if (dataToIPE.Data.Length < _dataBufferLength + 4)
                                {
                                    lock (_client)
                                        _lastBlock = true;
                                }
                                byte[] fileData = new byte[dataToIPE.Data.Length - 4];
                                Buffer.BlockCopy(dataToIPE.Data.Buffer, 4, fileData, 0, fileData.Length);
                                if (fileData.Length > 0)
                                    SaveData(new ByteDataCarrier(fileData));

                                DoAcknowledge(currentEndPoint, blockNumber);
                            }
                            break;
                        case Opcodes.ACK:
                            if (blockNumber == _blockNumber)
                            {
                                _confirmed.Set();

                                lock (_client)
                                {
                                    if (!_lastBlock)
                                        SendNextData(dataToIPE.IPEndPoint, blockNumber + 1);
                                }
                            }
                            break;
                        case Opcodes.ERROR:
                            break;
                    }
                }
            }
        }

        private void SendNextData(
            [PublicAPI] IPEndPoint endPoint, 
            int blockNumber)
        {
            byte[] bufferData = new byte[_dataBufferLength];
            int dataRed = _fileToRead.Read(bufferData, 0, _dataBufferLength);
            byte[] dataToSend = new byte[dataRed + 4];
            Buffer.BlockCopy(bufferData, 0, dataToSend, 4, dataRed);

            dataToSend[0] = 0;
            dataToSend[1] = (byte)(Opcodes.DATA);
            dataToSend[2] = (byte)(blockNumber >> 8);
            dataToSend[3] = (byte)blockNumber;

            if (OnPercentSent != null)
            {
                int percent = GetPercentSent();
                if (_progressStep == 1 ||
                    (percent == 0 || percent == 1 || percent == _progressStep || percent == 99 || percent == 100 || (percent - _lastSentProgress) >= _progressStep))
                {
                    _lastSentProgress = percent;
                    OnPercentSent(_serverIPAddress, percent);
                }
            }

            lock (_client)
            {
                if (dataRed < _dataBufferLength)
                    _lastBlock = true;
                _blockNumber = blockNumber;
                _dataToSend = new ByteDataCarrier(dataToSend);
                _resendNewData.Set();
            }
        }

        private void DoAcknowledge(
            [PublicAPI] IPEndPoint endPoint,
            int blockNumber)
        {
            byte[] ackData = new byte[4];
            ackData[0] = 0;
            ackData[1] = (byte)(Opcodes.ACK);
            ackData[2] = (byte)(blockNumber >> 8);
            ackData[3] = (byte)blockNumber;

            lock (_client)
            {
                _blockNumber = blockNumber;
                _dataToSend = new ByteDataCarrier(ackData);
                _resendNewData.Set();
            }
        }

        private void CloseWriteFile()
        {
            try
            {
                _fileToWrite.Close();
                _fileToWrite = null;
            }
            catch { }
        }

        private void CloseReadFile()
        {
            try
            {
                _fileToRead.Close();
                _fileToRead = null;
            }
            catch { }
        }


        private void SaveData(ByteDataCarrier data)
        {
            if (_fileToWrite.CanWrite)
            {
                _fileToWrite.Write(data.Buffer, 0, data.ActualSize);
            }
        }

        private byte[] CreateRequest(
            [PublicAPI] IPAddress ip, 
            string filename, 
            RequestTypes requestType)
        {
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(filename);
            byte[] modeBytes = Encoding.ASCII.GetBytes(Modes.Octet.ToString());

            byte[] result = new byte[2 + fileNameBytes.Length + 1 + modeBytes.Length + 1];
            result[0] = 0;
            if (requestType == RequestTypes.Read)
                result[1] = (byte)Opcodes.RRQ;
            else if (requestType == RequestTypes.Write)
                result[1] = (byte)Opcodes.WRG;
            Buffer.BlockCopy(fileNameBytes, 0, result, 2, fileNameBytes.Length);
            Buffer.BlockCopy(modeBytes, 0, result, 2 + fileNameBytes.Length + 1, modeBytes.Length);
            return result;
        }

        public int GetPercentSent()
        {
            if (_lastBlock)
                return 100;
            if (_fileToRead == null)
                return 0;
            try
            {
                return (int)((double)_fileToRead.Position / _fileToRead.Length * 100);
            }
            catch { }
            return 0;
        }

        private class DataToIpe
        {
            public readonly IPEndPoint IPEndPoint;
            public readonly ByteDataCarrier Data;

            public DataToIpe(
                [NotNull] IPEndPoint ipEndpoint, 
                [NotNull] ByteDataCarrier data)
            {
                Validator.CheckForNull(ipEndpoint,"ipEndpoint");
                Validator.CheckForNull(data,"data");

                IPEndPoint = ipEndpoint;
                Data = data;
            }
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            Stop();
        }
    }

    class NoResponseException : Exception
    {
        public NoResponseException(string message)
            : base(message)
        { }
    }
}
