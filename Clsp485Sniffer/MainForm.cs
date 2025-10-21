using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Net;
using Contal.IwQuick.UI;

using Contal.Cgp.NCAS.NodeDataProtocolPC;
using Contal.Drivers.ClspDriversPC;

namespace Clsp485Sniffer
{
    public partial class MainForm : Form
    {
        private Sniffer _sniffer = null;
        private SerialPortDescriptor[] _portDescs;

        private ControlNotificationSettings _notificationSettings;
        private bool _isEncrypted;
        private byte[] _key;
        private byte[] _iv;
        private AESSettings _aesSettings;

        private int _selectedProtocol = -1;

        public MainForm()
        {
            InitializeComponent();

            _portDescs = SimpleSerialPort.GetExtendedPortNames();
            
            _portList.Items.Clear();
            foreach (SerialPortDescriptor portDesc in _portDescs)
            {
                _portList.Items.Add(portDesc);
            }

            _protocolList.SelectedIndex = 0;

            _key = new byte[16];
            _iv = new byte[16];
            _aesSettings = null;

            /* Init GUI */
            _notificationSettings = new ControlNotificationSettings();
            _notificationSettings.Duration = 3000;

            _dataTextbox.BindScroll(_infoTextbox);
            _infoTextbox.BindScroll(_dataTextbox);

            _selectedProtocol = 2;
            _protocolList.SelectedIndex = _selectedProtocol;

            _masterColorLabel.BackColor = Color.White;
            _nodeColorLabel.BackColor = Color.FromArgb(216, 227, 245);

            _autoscroll.Checked = true;
        }

        #region GUI Messages
        private void ShowError(Control control, string message)
        {
            ControlNotification.Singleton.Error(NotificationPriority.JustOne, control, message, _notificationSettings);
        }

        private void ShowInfo(Control control, string message)
        {
            ControlNotification.Singleton.Info(NotificationPriority.JustOne, control, message, _notificationSettings);
        }

        private void ShowWarning(Control control, string message)
        {
            ControlNotification.Singleton.Warning(NotificationPriority.JustOne, control, message, _notificationSettings);
        }

        private void ErrorBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region Main start GUI handling
        private void _startButton_Click(object sender, EventArgs e)
        {
            if (_sniffer == null)
            {
                if (_portList.SelectedIndex == -1)
                {
                    ShowError(_portList, "No port selected");
                    return;
                }

                SerialPortDescriptor portDesc = (SerialPortDescriptor)_portList.SelectedItem;
                _sniffer = new Sniffer(portDesc.PortName);
                _sniffer.MasterFrameReceived += new D2Type2Void<Frame, DateTime>(_sniffer_MasterFrameReceived);
                _sniffer.NodeFrameReceived += new D2Type2Void<Frame, DateTime>(_sniffer_NodeFrameReceived);
                _sniffer.Start();

                _startButton.Text = "Stop";
            }
            else
            {
                _sniffer.Stop();
                _sniffer = null;

                _startButton.Text = "Start";
            }
        }

        private void _portList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_portList.SelectedIndex != -1)
                _startButton.Enabled = true;
            else
                _startButton.Enabled = false;
        }
        #endregion

        #region Frame printing
        private void _protocolList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedProtocol = _protocolList.SelectedIndex;
        }

        private bool IsFiltered(Frame frame)
        {
            ProtocolId protocol;

            bool byProtocol = false;    // default is not filtered by protocol
            bool byResend = true;       // default is filter resend messages

            #region Filter by protocol
            if (_filterByProtocol.Checked)
            {
                byte messageCode = (byte)frame.MessageCode;

                switch (_selectedProtocol)
                {
                    /* All */
                    case 0:
                        byProtocol = false;
                        break;

                    /* Link layer */
                    case 1:
                        if (messageCode < (byte)MessageCode.ProtocolFirstId)
                        {
                            byProtocol = false;
                            break;
                        }

                        byProtocol = true;
                        break;

                    /* Proto access */
                    case 2:
                        if (messageCode < (byte)MessageCode.ProtocolFirstId)
                        {
                            byProtocol = true;
                            break;
                        }
                         
                        protocol = (ProtocolId)(messageCode - (byte)MessageCode.ProtocolFirstId);
                        if (protocol == ProtocolId.ProtoAccess)
                        {
                            byProtocol = false;
                            break;
                        }

                        byProtocol = true;
                        break;

                    /* Proto uploader */
                    case 3:
                        if (messageCode < (byte)MessageCode.ProtocolFirstId)
                        {
                            byProtocol = true;
                            break;
                        }

                        protocol = (ProtocolId)(messageCode - (byte)MessageCode.ProtocolFirstId);
                        if (protocol == ProtocolId.ProtoUploader)
                        {
                            byProtocol = false;
                            break;
                        }

                        byProtocol = true;
                        break;

                    /* Proto CCR */
                    case 4:
                        if (messageCode < (byte)MessageCode.ProtocolFirstId)
                        {
                            byProtocol = true;
                            break;
                        }

                        protocol = (ProtocolId)(messageCode - (byte)MessageCode.ProtocolFirstId);
                        if (protocol == ProtocolId.ProtoCardReader)
                        {
                            byProtocol = false;
                            break;
                        }

                        byProtocol = true;
                        break;

                    default:
                        byProtocol = false;
                        break;
                }

            }
            #endregion

            #region Filter by Resend
            if (_resendFilter.Checked)
            {
                if (frame.MessageCode == MessageCode.ResendRequired)
                    byResend = false;
                else
                    byResend = true;
            }
            #endregion

            if (!byProtocol || !byResend)
                return false;
            
            return true;
        }

        private void PushDataWithColor(byte[] rawData, bool isNode)
        {
            string rawBytesStr = ByteDataCarrier.HexDump(rawData);
            
            switch (rawData[3])
            {
                /* Resend required */
                case (byte)MessageCode.ResendRequired:
                    _dataTextbox.Push(rawBytesStr + " \r\n", Color.White, Color.DarkRed);
                    break;

                /* Regular messages */
                default:
                    if (isNode)
                        _dataTextbox.Push(rawBytesStr + " \r\n", Color.Black, Color.FromArgb(216, 227, 245));
                    else
                        _dataTextbox.Push(rawBytesStr + " \r\n", Color.Black, Color.White);
                    break;
            }   
        }

        private void PushInfoWithColor(byte[] rawData, bool isNode, DateTime timestamp)
        {
            string textToPrint = "";
            string time = timestamp.ToString("mm.ss.fff") + ": ";

            /* Show frame description if available */
            if ((byte)rawData[3] > (byte)MessageCode.ProtocolFirstId)
            {
                textToPrint = Command.GetCommandName(rawData[4]);
                if (rawData[4] == Command.Ack)
                    textToPrint += " to " + Command.GetCommandName(rawData[9]);

                textToPrint = time + textToPrint;
            }
            else
                textToPrint = ((MessageCode)rawData[3]).ToString() + "\r\n";


            if (isNode)
                _infoTextbox.Push(textToPrint + " \r\n", Color.Black, Color.FromArgb(216, 227, 245));
            else
                _infoTextbox.Push(textToPrint + " \r\n", Color.Black, Color.White);
            
        }

        private byte[] FrameRawBytes(Frame frame, AESSettings aesSettings)
        {
            byte[] rawBytes = null;
            
            if (_aesSettings != null)
            {
                frame.IsEncrypted = true;
                //rawBytes = frame.DecryptRawBytes(_aesSettings);
            }
            else
            {
                //rawBytes = frame.GetRawBytes(null);
            }

            return rawBytes;
        }

        void _sniffer_NodeFrameReceived(Frame frame, DateTime timestamp)
        {
            try
            {
                if (!IsFiltered(frame))
                {
                    byte[] rawBytes = FrameRawBytes(frame, _aesSettings);

                    PushDataWithColor(rawBytes, true);
                    PushInfoWithColor(rawBytes, true, timestamp);
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("NodeFrameReceived: " + error.Message);
            }
        }

        void _sniffer_MasterFrameReceived(Frame frame, DateTime timestamp)
        {
            try
            {
                if (!IsFiltered(frame))
                {
                    byte[] rawBytes = FrameRawBytes(frame, _aesSettings);

                    PushDataWithColor(rawBytes, false);
                    PushInfoWithColor(rawBytes, false, timestamp);
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("MasterFrameReceived: " + error.Message);
            }
        }
        #endregion

        #region Text box GUI routines 
        private void _autoscroll_CheckedChanged(object sender, EventArgs e)
        {
            _dataTextbox.AutoScroll = _autoscroll.Checked;
            _infoTextbox.AutoScroll = _autoscroll.Checked;
        }
        #endregion
     
        int tmpvalue = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            if ((tmpvalue++)%2 == 0)
                _dataTextbox.Push("Test message\r\n", Color.White, Color.DarkRed);
            else
                _dataTextbox.Push("Test message\r\n", Color.White, Color.Black);
        }

        #region Ecryption related GUI
        private void _isEncryptedChckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isEncryptedChckbox.Checked)
            {
                // parse key (space separated decimals)
                try
                {
                    string[] keyParts = _keyText.Text.Split(' ');
                    if (keyParts.Length < _key.Length)
                    {
                        ErrorBox("Key length must be 16");
                        return;
                    }
                    for (int i = 0; i < _key.Length; i++)
                        _key[i] = byte.Parse(keyParts[i]);
                }
                catch (Exception error)
                {
                    System.Diagnostics.Debug.WriteLine("Error parsing key: " + error.Message);
                    ErrorBox("Error parsing key: " + error.Message);
                    _aesSettings = null;
                    _isEncrypted = false;
                }

                // parse iv (space separated decimals)
                try
                {
                    string[] ivParts = _ivText.Text.Split(' ');
                    if (ivParts.Length < _iv.Length)
                    {
                        ErrorBox("IV length must be 16");
                        return;
                    }
                    for (int i = 0; i < _iv.Length; i++)
                        _iv[i] = byte.Parse(ivParts[i]);
                }
                catch (Exception error)
                {
                    System.Diagnostics.Debug.WriteLine("Error parsing iv: " + error.Message);
                    ErrorBox("Error parsing iv: " + error.Message);
                    _aesSettings = null;
                    _isEncrypted = false;
                }

                _aesSettings = new AESSettings(_key, _iv, AESKeySize.Size128);
                _isEncrypted = true;
            }
            else
            {
                _isEncrypted = false;
                _aesSettings = null;
            }
        }
        #endregion


    }
}
