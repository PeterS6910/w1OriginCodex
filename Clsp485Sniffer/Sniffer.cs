using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.IwQuick;
using Contal.IwQuick.Net;
using Contal.IwQuick.Parsing;
using Contal.IwQuick.Data;
using Contal.IwQuick.Crypto;
using Contal.Drivers.ClspDriversPC;


namespace Clsp485Sniffer
{
    class Sniffer
    {
        private PreambleHeaderDataParser _masterParser;
        private PreambleHeaderDataParser _nodeParser;

        private SimpleSerialPort _port;

        private AESSettings _aesSettings;

        private const int IMPLICIT_BAUDRATE = 115200;
        private const int MINIMAL_NODE_RESPONSE_TIMEOUT = 7; // in ms

        public Sniffer(string portName)
        {
            byte[] masterPreamble = { 0xFE, 0xEF };
            byte[] nodePreamble = { 0xEF, 0xFE };

            _masterParser = new PreambleHeaderDataParser(masterPreamble, Frame.HEADER_LENGTH_485, DataLengthFromHeader, IMPLICIT_BAUDRATE);
            _masterParser.FrameAvailable += new Contal.IwQuick.D3Type2Void<ByteDataCarrier, int,APhdParsingContext>(OnMasterDataAvailable);
            _nodeParser = new PreambleHeaderDataParser(nodePreamble, Frame.HEADER_LENGTH_485, DataLengthFromHeader, IMPLICIT_BAUDRATE);
            _nodeParser.FrameAvailable += new Contal.IwQuick.D3Type2Void<ByteDataCarrier, int,APhdParsingContext>(OnNodeDataAvailable);

            _port = new SimpleSerialPort();
            _port.BaudRate = IMPLICIT_BAUDRATE;
            _port.ReadTimeout = MINIMAL_NODE_RESPONSE_TIMEOUT;
            _port.PortName = portName;
            _port.DataReceived += new DSerialDataReceived(OnDataReceived);

            _aesSettings = null;
        }

        public void Start()
        {
            _port.Start(SerialPortReadingMode.Async);
        }

        public void Stop()
        {
            _port.Stop();
        }

        public bool IsRunning
        {
            get { return _port.IsStarted; }
        }

        public AESSettings AESSettings
        {
            get { return _aesSettings; }
            set { _aesSettings = value; }
        }

        #region Receive events
        public event DType2Void<ByteDataCarrier> NodeData;
        private void FireNodeData(ByteDataCarrier data)
        {
            try
            {
                if (NodeData != null)
                    NodeData(data);
            }
            catch { }
        }
        public event D2Type2Void<Frame, DateTime> NodeFrameReceived;
        private void FireNodeFrameReceived(Frame frame, DateTime timestamp)
        {
            try
            {
                if (NodeFrameReceived != null)
                    NodeFrameReceived(frame, timestamp);
            }
            catch { }
        }
        
        public event DType2Void<ByteDataCarrier> MasterData;
        private void FireMasterData(ByteDataCarrier data)
        {
            try
            {
                if (MasterData != null)
                    MasterData(data);
            }
            catch { }
        }

        public event D2Type2Void<Frame, DateTime> MasterFrameReceived;
        private void FireMasterFrameReceived(Frame frame, DateTime timestamp)
        {
            try
            {
                if (MasterFrameReceived != null)
                    MasterFrameReceived(frame, timestamp);
            }
            catch { }
        }
        #endregion


        void OnNodeDataAvailable(ByteDataCarrier data, int length,APhdParsingContext parsingContext)
        {
            //Frame frame = Frame.Parse(data, length, _aesSettings, false);

            //FireNodeData(data);
            //FireNodeFrameReceived(frame, DateTime.Now);
        }

        void OnMasterDataAvailable(ByteDataCarrier data, int length, APhdParsingContext parsingContext)
        {
            //Frame frame = Frame.Parse(data, length, _aesSettings, true);
            //FireMasterData(data);
            //FireMasterFrameReceived(frame, DateTime.Now);
        }

        void OnDataReceived(ISimpleSerialPort peer, ByteDataCarrier data, int optionalDataLength)
        {
            _masterParser.ProcessData(data);
            _nodeParser.ProcessData(data);
        }

        private int DataLengthFromHeader(ByteDataCarrier data)
        {
            if (data == null || data.Buffer == null)
                return -1;

            if (data.ActualSize != Frame.HEADER_LENGTH_485)
            {
                // this should NOT happen
                //ReleaseWaitingForReception();
                //FinalizeReception(ReceptionResult.InvalidData);

                System.Diagnostics.Debug.WriteLine("Data size differs from header length");

                return -1;
            }

            if (Crc8.ComputeChecksum(data.Buffer, 0, Frame.HEADER_LENGTH_485 - 1) != data[Frame.HEADER_LENGTH_485 - 1])
            {
                //ReleaseWaitingForReception();
                // true must be here, otherwise the seq numbers would not be incremented and the SlaveNode will try to respond again
                //FinalizeReception(ReceptionResult.ChecksumInvalid);

                return -1;
            }

            int dl = data[4];

            if (dl > Frame.MAX_OPTIONAL_DATA_LENGTH_485)
                return Frame.MAX_OPTIONAL_DATA_LENGTH_485;
            else
                return dl;
        }
    }
}
