using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using Contal.IwQuick.Sys;


namespace Contal.IwQuick.Net
{
    public enum OpCodes
    {
        RRQ = 1,
        WRG = 2,
        DATA = 3,
        ACK = 4,
        ERROR = 5
    }

    public enum CurrentTransferState
    {
        Started = 0,
        Waiting = 1
    }

    public enum ServerActivityState
    {
        Working = 0,
        Idle = 3
    }

    public class TFTPServer
    {
        public static int DEFAULT_BUFFER_LENGTH = 16384;

        public delegate void DOnFileReceived(string filename, bool memoryStream);
        public event DOnFileReceived OnFileReceived;

        public delegate void DOnFileDataAppend(long currentFileBytes);
        public event DOnFileDataAppend OnFileDataAppend;

        private CurrentTransferState _currentTransferState = CurrentTransferState.Waiting;

        private int _datBufferLength = DEFAULT_BUFFER_LENGTH;

        public int DatBufferLength
        {
            get { return _datBufferLength; }
            set { _datBufferLength = value; }
        }

        UdpClient _client;

        private string _defaultDataDirectory = QuickPath.ApplicationStartupDirectory;
        public string DefaultDataDirectory
        {
            get { return _defaultDataDirectory; }
            set { _defaultDataDirectory = value; }
        }

        int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private volatile bool _stop;

        private bool _lastBlock;
        byte[] _dataBuffer = null;

        FileStream _fileToWrite = null;
        string _filenameToWrite = string.Empty;
        FileStream _fileToRead = null;
        string _filenameToRead = string.Empty;

        MemoryStream _streamToWrite = null;

        int _blockNumber = 0;

        private Dictionary<string, MemoryStream> _receivedMemoryStreams = null;

        public TFTPServer(int port, string defaultDataDirectory)
            : this(port)
        {
            _defaultDataDirectory = defaultDataDirectory;
        }

        public TFTPServer(int port)
        {
            _port = port;
        }

        Thread _listenThread;

        public void Start()
        {
            Start(0, false);
        }

        public void Start(bool intoMemory)
        {
            Start(0, intoMemory);
        }

        public void Start(int dataBlockSize)
        {
            Start(dataBlockSize, false);
        }

        public void Start(int dataBlockSize, bool intoMemory)
        {
            if (_client == null)
            {
                if (dataBlockSize > 0)
                    _datBufferLength = dataBlockSize;
                else
                    _datBufferLength = DEFAULT_BUFFER_LENGTH;
                _client = new UdpClient(_port);
                _listenThread = new Thread(delegate { ReceiveData(intoMemory); });
                _listenThread.Start();
            }
        }

        public void Stop()
        {
            _stop = true;
            SetStateToDefault();
            if (_client != null)
            {
                try
                {
                    _listenThread.Abort();
                }
                catch { }
                finally
                {
                    try
                    {
                        _client.Close();
                    }
                    catch { }
                    _client = null;
                }
            }
        }

        private void SetStateToDefault()
        {
            if (_fileToRead != null)
                try
                {
                    _fileToRead.Close();
                }
                catch (Exception)
                { }
            if (_fileToWrite != null)
                try
                {
                    _fileToWrite.Close();
                }
                catch (Exception)
                { }
            _fileToRead = null;
            _fileToWrite = null;
            _filenameToRead = string.Empty;
            _filenameToWrite = string.Empty;
            _blockNumber = 0;
            _dataBuffer = null;
            _lastBlock = false;
        }

        public long FileReceivedBytes
        {
            get
            {
                if (_fileToWrite != null)
                    try { return _fileToWrite.Position; }
                    catch { }
                return 0;
            }
        }

        private void ReceiveData(bool intoMemory)
        {
            _stop = false;
            while (!_stop)
            {
                IPEndPoint endPoint = null;
                byte[] data = _client.Receive(ref endPoint);
                int blockNumber = GetAcknowledgeBlockNumber(data);
                if (data.Length >= 4)
                {
                    OpCodes opCode = (OpCodes)(short)((((short)data[0]) * 256) + (short)data[1]);
                    switch (opCode)
                    {
                        case OpCodes.RRQ:
                            if (IgnoreRequest())
                                break;
                            if (_currentTransferState == CurrentTransferState.Waiting)
                            {
                                _currentTransferState = CurrentTransferState.Started;
                                _filenameToRead = GetFilenameFromRequestData(data);
                                _fileToRead = null;
                                _blockNumber = 1;
                            }
                            SendFirstBlock(endPoint, _filenameToRead, 1);
                            break;
                        case OpCodes.WRG:
                            _filenameToWrite = GetFilenameFromRequestData(data);
                            BeginReceiveFile(endPoint, _filenameToWrite, intoMemory);
                            break;
                        case OpCodes.DATA:
                            if (data.Length == _datBufferLength + 4)
                                ProcessReceivedData(endPoint, data, intoMemory);
                            else
                                ConsiderWriteDataAndCloseFile(endPoint, data, intoMemory);
                            if (OnFileDataAppend != null)
                                OnFileDataAppend(_fileToRead.Position);
                            break;
                        case OpCodes.ACK:
                            if (blockNumber < _blockNumber - 1)
                                break;
                            if (blockNumber > _blockNumber)
                                break;
                            if (blockNumber == _blockNumber)
                            {
                                if (_lastBlock)
                                {
                                    EndReadingProcess();
                                    break;
                                }
                                _blockNumber++;
                                _dataBuffer = GetNextDataToSend(_blockNumber);
                            }
                            SendData(endPoint, _dataBuffer);
                            break;
                        case OpCodes.ERROR:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private bool IgnoreRequest()
        {
            return (_blockNumber > 1);
        }

        private void SendData(IPEndPoint endPoint, byte[] dataToSend)
        {
            if (dataToSend.Length < _datBufferLength + 4)
                _lastBlock = true;
            _client.Send(dataToSend, dataToSend.Length, endPoint);
        }


        private byte[] GetNextDataToSend(int blockNumber)
        {
            byte[] bufferData = new byte[_datBufferLength];
            int dataRed = _fileToRead.Read(bufferData, 0, bufferData.Length);
            byte[] bytesToSend = new byte[2 + 2 + dataRed];
            Buffer.BlockCopy(bufferData, 0, bytesToSend, 4, dataRed);
            bytesToSend[0] = 0;
            bytesToSend[1] = (byte)OpCodes.DATA;
            bytesToSend[2] = (byte)(blockNumber >> 8);
            bytesToSend[3] = (byte)blockNumber;
            return bytesToSend;
        }

        private void EndReadingProcess()
        {
            try
            {
                if (_fileToRead != null)
                    _fileToRead.Close();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Exception: " + ex.Message);
#endif
            }
            _fileToRead = null;
            _blockNumber = 0;
            _currentTransferState = CurrentTransferState.Waiting;
            _lastBlock = false;
            _dataBuffer = null;
        }

        private int GetAcknowledgeBlockNumber(byte[] data)
        {
            if (data.Length != 4)
                return 0;
            return ((data[2] << 8) + data[3]);
        }

        private void SendFirstBlock(IPEndPoint endPoint, string filenameToRead, int blockNumber)
        {
            try
            {
                if (_fileToRead == null)
                {
                    if (Path.IsPathRooted(filenameToRead))
                        _fileToRead = new FileStream(filenameToRead, FileMode.Open);
                    else
                        _fileToRead = new FileStream(_defaultDataDirectory + @"\" + filenameToRead, FileMode.Open);
                }
            }
            catch (Exception ex)
            {
                EndReadingProcess();
#if DEBUG
                Console.WriteLine("Exception: " + ex.Message);
#endif
                return;
            }
            if (_dataBuffer == null || _dataBuffer.Length == 0)
            {
                byte[] readBuffer = new byte[_datBufferLength];
                int currentBufferLength = 0;
                currentBufferLength = _fileToRead.Read(readBuffer, 0, _datBufferLength);
                if (currentBufferLength < _datBufferLength)
                    _lastBlock = true;
                _dataBuffer = new byte[currentBufferLength + 4];
                Buffer.BlockCopy(readBuffer, 0, _dataBuffer, 4, currentBufferLength);
                _dataBuffer[0] = 0;
                _dataBuffer[1] = (byte)OpCodes.DATA;
                _dataBuffer[2] = (byte)(blockNumber >> 8);
                _dataBuffer[3] = (byte)blockNumber;
            }
            SendData(endPoint, _dataBuffer);
        }

        private void ConsiderWriteDataAndCloseFile(IPEndPoint endPoint, byte[] data, bool intoMemory)
        {
            int blockNumber = (data[2] << 8) + data[3];
            if (blockNumber < _blockNumber)
                return;
            if (blockNumber == _blockNumber + 1)
            {
                _blockNumber = blockNumber;
                SendAcknowledge(endPoint, blockNumber);

                string receivedFileOrStreamName = string.Empty;
                if (intoMemory)
                {
                    if (_streamToWrite != null)
                    {
                        receivedFileOrStreamName = _filenameToWrite;
                        _streamToWrite.Write(data, 4, data.Length - 4);
                        _streamToWrite.Position = 0;
                        InsertIntoMemoryStreams(_streamToWrite, _filenameToWrite);
                    }
                }
                else
                    if (_fileToWrite != null)
                    {
                        receivedFileOrStreamName = _fileToWrite.Name;
                        _fileToWrite.Write(data, 4, data.Length - 4);
                        _fileToWrite.Close();
                        _fileToWrite = null;
                    }

                if (OnFileReceived != null)
                {
                    OnFileReceived(receivedFileOrStreamName, intoMemory);
                }
            }
        }

        private void InsertIntoMemoryStreams(MemoryStream stream, string fileName)
        {
            if (_receivedMemoryStreams == null)
                _receivedMemoryStreams = new Dictionary<string, MemoryStream>();
            if (!_receivedMemoryStreams.ContainsKey(fileName))
            {
                _receivedMemoryStreams.Add(fileName, stream);
                return;
            }
            _receivedMemoryStreams[fileName] = stream;
        }

        public MemoryStream GetMemoryStream(string streamName)
        {
            if (_receivedMemoryStreams == null)
                return null;

            if (_receivedMemoryStreams.ContainsKey(streamName))
                return _receivedMemoryStreams[streamName];

            return null;
        }

        public void RemoveMemoryStream(string streamName)
        {
            if (_receivedMemoryStreams == null)
                return;

            if (_receivedMemoryStreams.ContainsKey(streamName))
            {
                _receivedMemoryStreams.Remove(streamName);
            }
        }

        private void ProcessReceivedData(IPEndPoint endPoint, byte[] data, bool intoMemory)
        {
            if (_filenameToWrite == null)
                return;
            int blockNumber = (data[2] << 8) + data[3];
            if (blockNumber < _blockNumber)
                return;
            if (blockNumber == _blockNumber + 1)
            {
                _blockNumber = blockNumber;
                if (intoMemory)
                    _streamToWrite.Write(data, 4, data.Length - 4);
                else
                    _fileToWrite.Write(data, 4, data.Length - 4);
            }
            if (blockNumber == _blockNumber || blockNumber == _blockNumber + 1)
                SendAcknowledge(endPoint, blockNumber);
        }

        private void Result(IAsyncResult ar)
        {

        }

        private void BeginReceiveFile(IPEndPoint endPoint, string fileToSave, bool intoMemory)
        {
            _blockNumber = 0;
            try
            {
                if (intoMemory)
                {
                    if (_streamToWrite != null)
                    {
                        _streamToWrite = null;
                    }
                    _streamToWrite = new MemoryStream();
                }
                else
                {
                    if (_fileToWrite != null)
                    {
                        _fileToWrite.Close();
                        _fileToWrite = null;
                    }
                    if (Directory.Exists(Path.GetDirectoryName(fileToSave)))
                    {
                        _fileToWrite = new FileStream(fileToSave, FileMode.Create, FileAccess.Write, FileShare.None, _datBufferLength);
                    }
                    else
                    {
                        _fileToWrite = new FileStream(_defaultDataDirectory + @"\" + fileToSave, FileMode.Create, FileAccess.Write, FileShare.None, _datBufferLength);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Exception: " + ex.Message);
#endif
                return;
            }
            SendAcknowledge(endPoint, 0);
        }

        private void SendAcknowledge(IPEndPoint endPoint, int blockNumber)
        {
            byte[] ackData = new byte[4];
            ackData[0] = 0;
            ackData[1] = (byte)(OpCodes.ACK);
            ackData[2] = (byte)(blockNumber >> 8);
            ackData[3] = (byte)(blockNumber);
            _client.Send(ackData, ackData.Length, endPoint);
        }

        private string GetFilenameFromRequestData(byte[] data)
        {
            List<byte> relevantBytes = new List<byte>();
            for (int i = 2; i < data.Length; i++)
            {
                if (data[i] == 0)
                    break;
                relevantBytes.Add(data[i]);
            }
            return ASCIIEncoding.ASCII.GetString(relevantBytes.ToArray(), 0, relevantBytes.Count);
        }
    }
}
