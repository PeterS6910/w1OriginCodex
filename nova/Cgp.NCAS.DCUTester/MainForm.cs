using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.NodeDataProtocolPC;
using Contal.Drivers.LPC3250;
using Contal.Drivers.ClspDriversPC;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.Drivers.CardReaderPC;

namespace Contal.Cgp.NCAS.DCUTester
{
    public partial class MainForm : Form
    {
        /* GUI elements */
        private TextBoxBrowser _upgradeBrowser;
        private List<Label> _inputStatuses;
        private List<Label> _outputStatuses;
        private List<Label> _outputLogicalStatuses;
        private List<CheckBox> _outputActivations;

        private int _outputCount;
        private int _inputCount;

        private Communicator _communicator;
        private bool _isRunning = false;
        private bool _dsmIsRunning = false;
        
        private ControlNotificationSettings _notificationSettings;


        public MainForm()
        {
            InitializeComponent();

            _communicator = new Communicator();

            _communicator.NodeCommunicator.NodeReleased += new DType2Void<SlaveNode>(NodeCommunicator_NodeReleased);
            _communicator.NodeListRefresh += new DNodeListRefresh(OnRefreshNodeList);
            _communicator.ProcessData += new D2Type2Void<MessageCode, object>(OnProcessData);
            _communicator.NodeCommunicator.ReportFWVersion += new D2Type2Void<SlaveNode, ExtendedVersion>(NodeCommunicator_ReportFWVersion);
            _communicator.IOTestFailed += new DType2Void<int>(_communicator_IOTestFailed);
            _communicator.IOTestPassed += new DVoid2Void(_communicator_IOTestPassed);
            _communicator.IOTestProgress += new DType2Void<int>(_communicator_IOTestProgress);
            /* Read port list */
            string[] portList = SerialPort.GetPortNames();
            foreach (string portName in portList)
                _portList.Items.Add(portName);
            if (_portList.Items.Count > 0)
                _portList.SelectedIndex = 0;

            _inputStatuses = new List<Label>();
            _outputStatuses = new List<Label>();
            _outputLogicalStatuses = new List<Label>();
            _outputActivations = new List<CheckBox>();

            _inputCount = -1;
            _outputCount = -1;

            /* Init GUI */
            _crpLevelSelect.SelectedIndex = 0;
            _notificationSettings = new ControlNotificationSettings();
            _notificationSettings.Duration = 3000;
            _upgradeBrowser = new TextBoxBrowser();
            _upgradeBrowser.Location = new Point(90, 25);
            _upgradeGroup.Controls.Add(_upgradeBrowser);
            //_notificationSettings.ErrorControlColors.BackColor = Color.Transparent;
            /* Output configuration */
            _outputFunctionality.SelectedIndex = 0;
            _communicator.NodeCommunicator.CRCardSwiped += new D4Type2Void<SlaveNode, CardReader, string,int>(NodeCommunicator_CRCardSwiped);
            _communicator.NodeCommunicator.UpgradeError += new D3Type2Void<SlaveNode, UpgradeErrors, string>(NodeCommunicator_UpgradeError);
        }

        void NodeCommunicator_ReportFWVersion(SlaveNode parameter1, ExtendedVersion parameter2)
        {
            string bVer = parameter1.GetParameter<string>(NodeCommunicator.BOOTLOADER_VERSION_NODE_PARAM);
            if (Validator.IsNotNullString(bVer))
                Thread2UI.SetText(this, bVer, _bootLoaderVersion);
        }

        void NodeCommunicator_UpgradeError(SlaveNode parameter1, UpgradeErrors parameter2, string parameter3)
        {
            Dialog.Error("Upgrading of node "+parameter1.LogicalAddress+" failed with error:\n"+parameter2+"\n"+ (parameter3 ?? String.Empty));
        }

        void NodeCommunicator_NodeReleased(SlaveNode parameter)
        {
            if (parameter.LogicalAddress == _selectedNodeAddress)
            {
                ClearUI();
                _selectedNodeAddress = 0;
            }
        }

        void ClearUI()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new DVoid2Void(ClearUI));
            else
            {
                _lbCardReaders.Items.Clear();
            }
        }

        void _communicator_IOTestProgress(int parameter)
        {
            if (InvokeRequired)
            {
                this.Invoke(new DType2Void<int>(_communicator_IOTestProgress), parameter);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Progress: " + parameter);
                _ioTestProgress.Value = parameter;
            }
        }

        void _communicator_IOTestPassed()
        {
            if (InvokeRequired)
            {
                this.Invoke(new DVoid2Void(_communicator_IOTestPassed));
            }
            else
            {
                MessageBox.Show("IO Test passed.", "IO Test done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _ioTestStart.Text = "Start";
            }
        }

        void _communicator_IOTestFailed(int parameter)
        {
            if (InvokeRequired)
            {
                this.Invoke(new DType2Void<int>(_communicator_IOTestFailed), parameter);
            }
            else
            {
                MessageBox.Show("IO Test failed on input: " + parameter, "IO Test Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _ioTestStart.Text = "Start";
            }
        }

        private void _setCRPLevelButton_Click(object sender, EventArgs e)
        {
            _communicator.SetCRPLevel((CRPLevel)(_crpLevelSelect.SelectedIndex));
        }

        void NodeCommunicator_CRCodeTimedOut(SlaveNode parameter1, CardReader parameter2)
        {
            
        }

        void NodeCommunicator_CRCodeSpecified(SlaveNode parameter1, CardReader parameter2, string parameter3)
        {

        }

        #region Message system (info, error, warning)
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
        #endregion

        #region Routines for GUI values setting

        void EnableOutputConfig(OutputType outputType)
        {
            switch (outputType)
            {
                case OutputType.Level:
                    _outputOnTime.Enabled = false;
                    _outputOffTime.Enabled = false;
                    _outputPulseTime.Enabled = false;
                    _outputOnTimeLabel.Enabled = false;
                    _outputOffTimeLabel.Enabled = false;
                    _outputPulseTimeLabel.Enabled = false;
                    break;

                case OutputType.Frequency:
                    _outputOnTime.Enabled = true;
                    _outputOffTime.Enabled = true;
                    _outputPulseTime.Enabled = false;
                    _outputOnTimeLabel.Enabled = true;
                    _outputOffTimeLabel.Enabled = true;
                    _outputPulseTimeLabel.Enabled = false;
                    break;

                case OutputType.Impulse:
                    _outputOnTime.Enabled = false;
                    _outputOffTime.Enabled = false;
                    _outputPulseTime.Enabled = true;
                    _outputOnTimeLabel.Enabled = false;
                    _outputOffTimeLabel.Enabled = false;
                    _outputPulseTimeLabel.Enabled = true;
                    break;
            }
        }

        void SetInputData(int inputCount)
        {
            _inputCountLabel.Text = inputCount.ToString();

            /* List of avail. inputs generation */
            _doorLockedInput.Items.Clear();
            _doorOpenedInput.Items.Clear();
            _doorMaxOpenedInput.Items.Clear();
            _intPushButtonInput.Items.Clear();
            _extPushButtonInput.Items.Clear();
            _inputConfig.Items.Clear();

            _reportedInputsGroup.Controls.Clear();

            /* Inputs Statuses */
            _inputStatusGroup.Controls.Clear();
            _inputStatuses.Clear();

            int x = 0;
            int y = 0;

            for (int i = 0; i < inputCount; i++)
            {
                string item = "Input " + (i + 1);
                _doorLockedInput.Items.Add(item);
                _doorOpenedInput.Items.Add(item);
                _doorMaxOpenedInput.Items.Add(item);
                _intPushButtonInput.Items.Add(item);
                _extPushButtonInput.Items.Add(item);
                _inputConfig.Items.Add(item);

                /* Single status */
                Label inputstatus = new Label();
                inputstatus.Text = "-";
                inputstatus.TextAlign = ContentAlignment.MiddleCenter;
                inputstatus.Size = new Size(44, 17);
                inputstatus.Location = new Point(12 + (50 * x), 33 + (y * 60));
                inputstatus.Padding = new Padding(2, 2, 2, 2);
                inputstatus.ForeColor = Color.White;
                inputstatus.BackColor = Color.Gray;
                _inputStatuses.Add(inputstatus);
                _inputStatusGroup.Controls.Add(inputstatus);

                /* Status id */
                Label inputId = new Label();
                inputId.Text = (i + 1).ToString();
                inputId.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                inputId.Location = new Point(30 + (x * 50), 10 + (y * 60));
                _inputStatusGroup.Controls.Add(inputId);

                #region checkboxes for inputs reporting
                /* Report input checkbox */
                CheckBox checkBox = new CheckBox();
                checkBox.Name = "_reportInput_" + i;
                checkBox.Text = "";
                checkBox.Size = new Size(13, 13);
                checkBox.Location = new Point(25 + (21 * i), 20);
                _reportedInputsGroup.Controls.Add(checkBox);
                /* Label for the report input */
                Label number = new Label();
                number.Text = (i + 1).ToString();
                number.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                number.Location = new Point(27 + (i * 21) - ((i + 1) < 10 ? 0 : 6), 1);
                _reportedInputsGroup.Controls.Add(number);
                #endregion

                x++;
                if (x > 7)
                {
                    x = 0;
                    y++;
                }
            }
        }

        void SetOutputData(int outputCount)
        {
            _outputCountLabel.Text = outputCount.ToString();

            _elStrikeOutput.Items.Clear();
            _elStrikeOppOutput.Items.Clear();
            _extraElStrikeOutput.Items.Clear();
            _extraElStrikeOppOutput.Items.Clear();
            _bypassAlarmOutput.Items.Clear();
            _outputConfig.Items.Clear();
            _outputsActGroupbox.Controls.Clear();
            _outputActivations.Clear();

            _ajarSpecOutput.Items.Clear();
            _intrusionSpecOutput.Items.Clear();
            _sabotageSpecOutput.Items.Clear();

            /* outputs info */
            _outputStatusGroup.Controls.Clear();
            _outputStatuses.Clear();
            _reportedOutputsGroup.Controls.Clear();
            _reportedOutputsLogicGroup.Controls.Clear();
            /* outputs assignement */
            _assignedOutputsGroup.Controls.Clear();

            for (int i = 0; i < outputCount; i++)
            {
                string item = "Output " + (i + 1);
                _elStrikeOutput.Items.Add(item);
                _elStrikeOppOutput.Items.Add(item);
                _extraElStrikeOutput.Items.Add(item);
                _extraElStrikeOppOutput.Items.Add(item);
                _bypassAlarmOutput.Items.Add(item);
                
                _outputConfig.Items.Add(item);
                _ajarSpecOutput.Items.Add(item);
                _intrusionSpecOutput.Items.Add(item);
                _sabotageSpecOutput.Items.Add(item);

                #region output activation 
                // add checkboxes based on the number of output
                CheckBox checkBox = new CheckBox();
                checkBox.Name = "_actOutput_" + i;
                checkBox.Text = "";
                checkBox.Size = new Size(13, 13);
                checkBox.Location = new Point(25 + (i * 21), 20);
                checkBox.CheckedChanged += new EventHandler(OnActivateOutput);
                _outputsActGroupbox.Controls.Add(checkBox);
                _outputActivations.Add(checkBox);
                // add checkbox descriptions
                Label number = new Label();
                number.Text = (i + 1).ToString();
                number.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                number.Location = new Point(27 + (i * 21) - ((i + 1) < 10 ? 0 : 6), 1);
                _outputsActGroupbox.Controls.Add(number);
                #endregion

                #region output statuses
                /* Single status */
                Label outputStatus = new Label();
                outputStatus.Text = "Off";
                outputStatus.TextAlign = ContentAlignment.MiddleCenter;
                outputStatus.Size = new Size(44, 17);
                outputStatus.Location = new Point(12 + (50 * i), 33);
                outputStatus.Padding = new Padding(2, 2, 2, 2);
                outputStatus.ForeColor = Color.White;
                outputStatus.BackColor = Color.LightGray;
                _outputStatuses.Add(outputStatus);
                _outputStatusGroup.Controls.Add(outputStatus);

                /* Status id */
                Label outputID = new Label();
                outputID.Text = (i + 1).ToString();
                outputID.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                outputID.Location = new Point(30 + (i * 50), 10);
                _outputStatusGroup.Controls.Add(outputID);
                #endregion

                #region output logical statuses
                /* Single status */
                outputStatus = new Label();
                outputStatus.Text = "Off";
                outputStatus.TextAlign = ContentAlignment.MiddleCenter;
                outputStatus.Size = new Size(44, 17);
                outputStatus.Location = new Point(12 + (50 * i), 33);
                outputStatus.Padding = new Padding(2, 2, 2, 2);
                outputStatus.ForeColor = Color.White;
                outputStatus.BackColor = Color.LightGray;
                _outputLogicalStatuses.Add(outputStatus);
                _outputLogicalStatusGroup.Controls.Add(outputStatus);

                /* Status id */
                outputID = new Label();
                outputID.Text = (i + 1).ToString();
                outputID.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                outputID.Location = new Point(30 + (i * 50), 10);
                _outputLogicalStatusGroup.Controls.Add(outputID);
                #endregion

                #region checkboxes for output to input assigment
                /* Assignable output checkbox */
                checkBox = new CheckBox();
                checkBox.Name = "_assOutput_" + i;
                checkBox.Text = "";
                checkBox.Size = new Size(13, 13);
                checkBox.Location = new Point(25 + (21 * i), 20);
                checkBox.CheckedChanged += new EventHandler(OnOutputAssigned);
                _assignedOutputsGroup.Controls.Add(checkBox);
                /* Label for the assignable output */
                number = new Label();
                number.Text = (i + 1).ToString();
                number.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                number.Location = new Point(27 + (i * 21) - ((i + 1) < 10 ? 0 : 6), 1);
                _assignedOutputsGroup.Controls.Add(number);
                #endregion

                #region checkboxes for output reporting
                /* Report output checkbox */
                checkBox = new CheckBox();
                checkBox.Name = "_reportOutput_" + i;
                checkBox.Text = "";
                checkBox.Size = new Size(13, 13);
                checkBox.Location = new Point(25 + (21 * i), 20);
                _reportedOutputsGroup.Controls.Add(checkBox);
                /* Label for the report output */
                number = new Label();
                number.Text = (i + 1).ToString();
                number.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                number.Location = new Point(27 + (i * 21) - ((i + 1) < 10 ? 0 : 6), 1);
                _reportedOutputsGroup.Controls.Add(number);
                #endregion

                #region checkboxes for output logic reporting
                /* Report output checkbox */
                checkBox = new CheckBox();
                checkBox.Name = "_reportOutputLogic_" + i;
                checkBox.Text = "";
                checkBox.Size = new Size(13, 13);
                checkBox.Location = new Point(25 + (21 * i), 20);
                _reportedOutputsLogicGroup.Controls.Add(checkBox);
                /* Label for the report output */
                number = new Label();
                number.Text = (i + 1).ToString();
                number.Size = new Size((i + 1 < 10) ? 13 : 19, 13);
                number.Location = new Point(27 + (i * 21) - ((i + 1) < 10 ? 0 : 6), 1);
                _reportedOutputsLogicGroup.Controls.Add(number);
                #endregion
            }

        }

        void ClearAllReadInfo()
        {
            _nodeList.SelectedIndex = -1;
            _nodeList.Items.Clear();
            
            _outputCountLabel.Text = "0";
            _inputCountLabel.Text = "0";
            _nodePhysicalAddress.Text = "0";

            _outputStatusGroup.Controls.Clear();
            _outputLogicalStatusGroup.Controls.Clear();
            _inputStatusGroup.Controls.Clear();

            _tamperLabel.Text = "-";
            _tamperLabel.BackColor = Color.Gray;

            /* HW testing */
            _host485Status.Text = "Error";
            _host485Status.BackColor = Color.Red;

            _ccr485Status.Text = "Error";
            _ccr485Status.BackColor = Color.Red;

            _fuseMirrorLabel.Text = "-";
            _fuseMirrorLabel.BackColor = Color.Gray;

            _tamperMirrorLabel.Text = "-";
            _tamperMirrorLabel.BackColor = Color.Gray;

            _expConnLabel.Text = "-";
            _expConnLabel.BackColor = Color.Gray;
            /* HW testing END */

            _nodeList.Enabled = false;

            /* APAS tab */
            _doorLockedInput.SelectedIndex = -1;
            _doorOpenedInput.SelectedIndex = -1;
            _doorMaxOpenedInput.SelectedIndex = -1;

            _elStrikeOutput.SelectedIndex = -1;
            _elStrikeOppOutput.SelectedIndex = -1;
            _extraElStrikeOutput.SelectedIndex = -1;
            _extraElStrikeOppOutput.SelectedIndex = -1;
            _bypassAlarmOutput.SelectedIndex = -1;

            _intPushButtonInput.SelectedIndex = -1;
            _extPushButtonInput.SelectedIndex = -1;

            _dsmEnviromentType.SelectedIndex = -1;
            _startStopDSM.Text = "Start";
            _dsmIsRunning = false;
            /* APAS tab end */

            /* Miscellanous tab */
            _ajarSpecOutput.SelectedIndex = -1;
            _intrusionSpecOutput.SelectedIndex = -1;
            _sabotageSpecOutput.SelectedIndex = -1;
            /* Miscellanous tab end */

            /* Input/output tab */
            _inputConfig.SelectedIndex = -1;
            _outputConfig.SelectedIndex = -1;
            _outputFunctionality.SelectedIndex = -1;
            /* Input/output tab end */

            /* Node overview tab */
            _dsmStateLabel.Text = "-";
            _dsmDetailLabel.Text = "-";
            _fwVersionLabel.Text = "-";
            _memoryLoadLabel.Text = "-";

            _tamperLabel.Text = "-";
            _tamperLabel.BackColor = Color.Gray;
            _fuseLabel.Text = "-";
            _fuseLabel.BackColor = Color.Gray;
            _powerExtLabel.Text = "-";
            _powerExtLabel.BackColor = Color.Gray;
            _fuseExtLabel.Text = "-";
            _fuseExtLabel.BackColor = Color.Gray;
            /* Node overview tab end */
        }

        void ShowInputChange(byte inputID, InputState state)
        {
            try
            {
                int maxInput = int.Parse(_inputCountLabel.Text);
                if (maxInput <= inputID)
                    return;

                Label inputStatus = _inputStatuses[inputID];

                switch (state)
                {
                    case InputState.Short:
                        inputStatus.BackColor = Color.Black;
                        inputStatus.Text = "Short";
                        break;

                    case InputState.Normal:
                        inputStatus.BackColor = Color.Green;
                        inputStatus.Text = "Normal";
                        break;

                    case InputState.Alarm:
                        inputStatus.BackColor = Color.Red;
                        inputStatus.Text = "Alarm";
                        break;

                    case InputState.Break:
                        inputStatus.BackColor = Color.Black;
                        inputStatus.Text = "Break";
                        break;

                    default:
                        inputStatus.BackColor = Color.DarkGray;
                        inputStatus.Text = "-";
                        break;
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Show input change: " + error.Message);
            }
            
        }

        void ShowOutputChange(byte outputID, bool isOn)
        {
            try
            {
                Label outputStatus = _outputStatuses[outputID];
                if (isOn)
                {
                    outputStatus.BackColor = Color.Red;
                    outputStatus.Text = "On";
                }
                else
                {
                    outputStatus.BackColor = Color.Black;
                    outputStatus.Text = "Off";
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Show output change: " + error.Message);
            }
        }

        void ShowLogicalOutputChange(byte outputID, bool isOn)
        {
            try
            {
                Label outputLogicalStatus = _outputLogicalStatuses[outputID];
                if (isOn)
                {
                    outputLogicalStatus.BackColor = Color.Red;
                    outputLogicalStatus.Text = "On";
                }
                else
                {
                    outputLogicalStatus.BackColor = Color.Black;
                    outputLogicalStatus.Text = "Off";
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Show logical output change: " + error.Message);
            }
        }

        void ShowSpecialInputChange(SpecialInput input, InputState state)
        {
            Label label = null;
            Label mirrorLabel = null;

            switch (input)
            {
                case SpecialInput.Tamper:
                    label = _tamperLabel;
                    mirrorLabel = _tamperMirrorLabel;
                    break;
                case SpecialInput.Fuse:
                    label = _fuseLabel;
                    mirrorLabel = _fuseMirrorLabel;
                    break;
                case SpecialInput.ExtensionBoardFuse:
                    label = _fuseExtLabel;
                    break;
                case SpecialInput.ExtensionBoardPower:
                    label = _powerExtLabel;
                    break;
            }
            
            if (label != null)
            {
                switch (state)
                {
                    case InputState.Alarm:
                        label.Text = "Alarm";
                        label.BackColor = Color.Red;
                        if (mirrorLabel != null)
                        {
                            mirrorLabel.Text = "Alarm";
                            mirrorLabel.BackColor = Color.Red;
                        }
                        break;

                    case InputState.Normal:
                        label.Text = "Normal";
                        label.BackColor = Color.Green;
                        if (mirrorLabel != null)
                        {
                            mirrorLabel.Text = "Normal";
                            mirrorLabel.BackColor = Color.Green;
                        }
                        break;

                    default:
                        label.Text = "-";
                        label.BackColor = Color.Gray;
                        if (mirrorLabel != null)
                        {
                            mirrorLabel.Text = "-";
                            mirrorLabel.BackColor = Color.Gray;
                        }
                        break;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ShowSpecialInputChange: unknown special input");
            }
        }

        #endregion



        #region Communicator event handlers

        void OnProcessData(MessageCode code, object value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new DProcessData(OnProcessData), code, value);
            }
            else
            {
                switch (code)
                {
                    case MessageCode.MemoryLoad:
                        int memoryLoad = (int)value;
                        _memoryLoadLabel.Text = memoryLoad.ToString();
                        break;

                    case MessageCode.DeviceInfo:
                        DeviceInfo devInfo = (DeviceInfo)value;

                        _inputCount = devInfo.InputCount;
                        SetInputData(_inputCount);
                        if (_inputCount > 4)
                        {
                            _expConnLabel.Text = "OK";
                            _expConnLabel.BackColor = Color.Green;
                        }

                        _outputCount = devInfo.OutputCount;
                        SetOutputData(_outputCount);
                        if (_outputCount > 4)
                        {
                            _expConnLabel.Text = "OK";
                            _expConnLabel.BackColor = Color.Green;
                        }
                        break;

                    case MessageCode.InputCount:
                        _inputCount = (int)value;
                        SetInputData(_inputCount);
                        if (_inputCount > 4)
                        {
                            _expConnLabel.Text = "OK";
                            _expConnLabel.BackColor = Color.Green;
                        }
                        break;

                    case MessageCode.OutputCount:
                        _outputCount = (int)value;
                        SetOutputData(_outputCount);
                        if (_outputCount > 4)
                        {
                            _expConnLabel.Text = "OK";
                            _expConnLabel.BackColor = Color.Green;
                        }
                        break;
                    
                    case MessageCode.CRPLevel:
                        CRPLevel level = (CRPLevel)value;
                        _crpLevelSelect.SelectedIndex = (int)level;
                        break;

                    case MessageCode.FWVersion:
                        ExtendedVersion version = (ExtendedVersion)value;
                        _fwVersionLabel.Text = version.ToString();
                        break;

                    case MessageCode.InputChanged:
                        InputChangeInfo inputChangedInfo = (InputChangeInfo)value;
                        ShowInputChange(inputChangedInfo.InputID, inputChangedInfo.InputState);
                        System.Diagnostics.Debug.WriteLine("Input " + inputChangedInfo.InputID + ": " +
                            inputChangedInfo.InputState);
                        break;

                    case MessageCode.SpecialInputChanged:
                        SpecialInputChangedInfo specialInputInfo = (SpecialInputChangedInfo)value;
                        ShowSpecialInputChange(specialInputInfo.InputType, specialInputInfo.InputState);
                        System.Diagnostics.Debug.WriteLine("Input " + (specialInputInfo.InputType) + ": " +
                            specialInputInfo.InputState);
                        break;

                    case MessageCode.OutputChanged:
                        OutputChangeInfo outputChangedInfo = (OutputChangeInfo)value;
                        ShowOutputChange(outputChangedInfo.OutputID, outputChangedInfo.IsOn);
                        Console.Write(DateTime.Now.ToString("ss.fff")+ " DO" + outputChangedInfo.OutputID + ": " +
                            outputChangedInfo.IsOn+" ");
                        break;

                    case MessageCode.LogicOutputChanged:
                        outputChangedInfo = (OutputChangeInfo)value;
                        ShowLogicalOutputChange(outputChangedInfo.OutputID, outputChangedInfo.IsOn);
                        break;

                    case MessageCode.DSMChanged:
                        DSMChangedInfo dsmChangedInfo = (DSMChangedInfo)value;
                        _dsmStateLabel.Text = dsmChangedInfo.State.ToString();
                        _dsmDetailLabel.Text = dsmChangedInfo.Detail.ToString();
                        break;

                    case MessageCode.ProtocolChanged:
                        ProtocolId protocol = (ProtocolId)value;
                        _protocolLabel.Text = protocol.ToString();
                        break;

                    case MessageCode.UpgradeProgreessUpdate:
                        int progress = (int)value;
                        _upgradeProgressBar.Value = progress;
                        _upgradeProgressLabel.Text = progress.ToString() + " / 100";

                        TimeSpan elapsedTime = DateTime.Now - _upgradeSw;
                        _elapsedTimeLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", elapsedTime.Hours,
                            elapsedTime.Minutes, elapsedTime.Seconds);

                        if (progress > 0)
                        {
                            TimeSpan diff = DateTime.Now - _upgradeSw;
                            float secondsPerPercent = (float)(diff.TotalSeconds / progress);
                            int remainingSeconds = (int)((100 - progress) * secondsPerPercent);
                            TimeSpan remainingTime = new TimeSpan(0, 0, remainingSeconds);
                            _remainingTimeLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", remainingTime.Hours,
                                remainingTime.Minutes, remainingTime.Seconds);
                        }

                        break;
                        
                    case MessageCode.CROnlineStateChanged:
                        _lbCardReaders.Items.Clear();
                        int onlineReaders = 0;

                        foreach (CardReader cr in _communicator.CurrentCardReaders)
                        {
                            _lbCardReaders.Items.Add(cr);
                            if (cr.IsOnline)
                                onlineReaders++;
                        }

                        if (value is CardReader)
                        {
                            CardReader cr = (CardReader)value;
                            AddCrEvent("CR" + cr.Address + " online=" + cr.IsOnline);
                        }

                        if (onlineReaders > 0)
                        {
                            _ccr485Status.Text = "OK";
                            _ccr485Status.BackColor = Color.Green;
                        }
                        else
                        {
                            _ccr485Status.Text = "Error";
                            _ccr485Status.BackColor = Color.Red;
                        }



                        break;

                }
            }
        }

        private void AddCrEvent(String message)
        {
            if (message == null)
                message = string.Empty;

            if (this.InvokeRequired)
                this.Invoke(new DString2Void(AddCrEvent), message);
            else
            {
                int index = _lbCrEvents.Items.Add(DateTime.Now+" "+message);
                if (index >= 0)
                    _lbCrEvents.SelectedIndex = index;
            }
        }

        void OnRefreshNodeList(byte[] nodeList)
        {
            if (InvokeRequired)
            {
                this.Invoke(new DNodeListRefresh(OnRefreshNodeList), nodeList);
            }
            else
            {
                _nodeList.Items.Clear();

                foreach (byte address in nodeList)
                {
                    _nodeList.Items.Add(address);
                }

                if (_nodeList.Items.Count > 0)
                {
                    _nodeList.Enabled = true;

                    _nodeList.SelectedIndex = 0;
                    string address = _communicator.GetNodePhysicalAddress((byte)_nodeList.SelectedItem);
                    if (address != null)
                        _nodePhysicalAddress.Text = address;
                    else
                        _nodePhysicalAddress.Text = "Unable to read";
                }
                else
                {
                    _nodeList.Enabled = false;
                    ClearAllReadInfo();
                }

                if (nodeList.Length > 0)
                {
                    _host485Status.Text = "OK";
                    _host485Status.BackColor = Color.Green;

                    if (_inputCount > 4 || _outputCount > 4)
                    {
                        _expConnLabel.Text = "OK";
                        _expConnLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        _expConnLabel.Text = "Error";
                        _expConnLabel.BackColor = Color.Red;
                    }

                    _startHWIOTest.Enabled = true;
                }
                else
                {
                    _host485Status.Text = "Error";
                    _host485Status.BackColor = Color.Red;

                    _startHWIOTest.Enabled = false;
                }
            }
        }

        #endregion

        #region Main controls

        private void _startButton_Click(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                _startButton.Text = "Stop";

                string portName = (string)_portList.Items[_portList.SelectedIndex];
                _communicator.Start(portName, _raiseRTS.Checked);

                //_communicator.NodeCommunicator.CRCodeSpecified += new D3Type2Void<SlaveNode, CardReader, string>(NodeCommunicator_CRCodeSpecified);
                //_communicator.NodeCommunicator.CRCodeTimedOut += new D2Type2Void<SlaveNode, CardReader>(NodeCommunicator_CRCodeTimedOut);

                _isRunning = true;

                _portList.Enabled = false;
            }
            else
            {
                _startButton.Text = "Start";
                _communicator.Stop();

                _isRunning = false;

                _portList.Enabled = true;

                ClearAllReadInfo();
            }
        }

        private byte _selectedNodeAddress = 0;
        private void _nodeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_nodeList.SelectedIndex >= 0)
            {
                _communicator.SelectedNode = (byte)_nodeList.SelectedItem;
                _selectedNodeAddress = (byte)_nodeList.SelectedItem;
            }
        }

        private void _mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_communicator != null)
                _communicator.Stop();
        }

        #endregion
        
        #region DSM

        private void _accessGrantedSignal_Click(object sender, EventArgs e)
        {
            _communicator.SignalAccessGranted();
        }

        private void _intPushButtonSet_Click(object sender, EventArgs e)
        {
            _communicator.SetPushButton(PushButtonType.Internal,
                                        (byte)_intPushButtonInput.SelectedIndex,
                                        _intPushButtonBalanced.Checked,
                                        _intPushButtonInverted.Checked, 0, 0);
        }

        private void _intPushButtonDelete_Click(object sender, EventArgs e)
        {
            _communicator.UnsetPushButton(PushButtonType.Internal);
        }

        private void _extPushButtonSet_Click(object sender, EventArgs e)
        {
            _communicator.SetPushButton(PushButtonType.External,
                                        (byte)_extPushButtonInput.SelectedIndex,
                                        _extPushButtonBalanced.Checked,
                                        _extPushButtonInverted.Checked, 0, 0);
        }

        private void _extPushButtonDelete_Click(object sender, EventArgs e)
        {
            _communicator.UnsetPushButton(PushButtonType.External);
        }

        private void _setTimmings_Click(object sender, EventArgs e)
        {
            _communicator.SetTimmings((UInt32)(_unlockTime.Value * 1000), (UInt32)(_openTime.Value * 1000),
                (UInt32)(_preAlarmTime.Value * 1000), (UInt32)(_doorArrayDelay.Value * 1000),
                (UInt32)_beforeIntrusionTime.Value);
        }

        private void _alarmSet_Click(object sender, EventArgs e)
        {
            _communicator.EnableAlarms(_enableDoorAJAR.Checked, _enableIntrusion.Checked,
                _enableSabotage.Checked);
        }

        private void _ajarSpecSet_Click(object sender, EventArgs e)
        {
            if (_ajarSpecOutput.SelectedIndex == -1)
            {
                ShowError(_ajarSpecOutput, "No output selected");
                return;
            }
            _communicator.SetSpecialOutput(SpecialOutputType.Ajar, (byte)_ajarSpecOutput.SelectedIndex);
        }

        private void _intrusionSpecSet_Click(object sender, EventArgs e)
        {
            if (_intrusionSpecOutput.SelectedIndex == -1)
            {
                ShowError(_intrusionSpecOutput, "No output selected");
                return;
            }
            _communicator.SetSpecialOutput(SpecialOutputType.Intrusion, (byte)_intrusionSpecOutput.SelectedIndex);
        }

        private void _sabotageSpecSet_Click(object sender, EventArgs e)
        {
            if (_sabotageSpecOutput.SelectedIndex == -1)
            {
                ShowError(_sabotageSpecOutput, "No output selected");
                return;
            }
            _communicator.SetSpecialOutput(SpecialOutputType.Sabotage, (byte)_sabotageSpecOutput.SelectedIndex);
        }

        private void _ajarSpecUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetSpecialOutput(SpecialOutputType.Ajar);
        }

        private void _intrusionSpecUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetSpecialOutput(SpecialOutputType.Intrusion);
        }

        private void _sabotageSpecUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetSpecialOutput(SpecialOutputType.Sabotage);
        }

        private void _setDoorLocked_Click(object sender, EventArgs e)
        {
            if (_doorLockedInput.SelectedIndex == -1)
            {
                ShowError(_doorLockedInput, "No input selected");
                return;
            }

            _communicator.SetSensor(SensorType.DoorLocked, (byte)_doorLockedInput.SelectedIndex,
                _doorLockedBalanced.Checked, _doorLockedInverted.Checked, 0, 0);
        }

        private void _setDoorOpened_Click(object sender, EventArgs e)
        {
            if (_doorOpenedInput.SelectedIndex == -1)
            {
                ShowError(_doorOpenedInput, "No input selected");
                return;
            }
            _communicator.SetSensor(SensorType.DoorOpened, (byte)_doorOpenedInput.SelectedIndex,
                _doorOpenedBalanced.Checked, _doorOpenedInverted.Checked, 0, 0);
        }

        private void _setDoorMaxOpened_Click(object sender, EventArgs e)
        {
            if (_doorMaxOpenedInput.SelectedIndex == -1)
            {
                ShowError(_doorMaxOpenedInput, "No input selected");
                return;
            }
            _communicator.SetSensor(SensorType.DoorFullyOpened, (byte)_doorMaxOpenedInput.SelectedIndex,
                _doorMaxOpenedBalanced.Checked, _doorMaxOpenedInverted.Checked, 0, 0);
        }

        private void _unsetDoorLocked_Click(object sender, EventArgs e)
        {
            _communicator.UnsetSensor(SensorType.DoorLocked);
        }

        private void _unsetDoorOpened_Click(object sender, EventArgs e)
        {
            _communicator.UnsetSensor(SensorType.DoorOpened);
        }

        private void _unsetDoorMaxOpened_Click(object sender, EventArgs e)
        {
            _communicator.UnsetSensor(SensorType.DoorFullyOpened);
        }

        private void _elStrikeSet_Click(object sender, EventArgs e)
        {
            _communicator.SetElectricStrike(ActuatorType.ElectricStrike, (byte)_elStrikeOutput.SelectedIndex,
                (_elStrikeImpulse.Checked ? StrikeType.Impulse : StrikeType.Level), (UInt32)_elStrikePulseTime.Value,
                _elStrikeInverted.Checked, 2000, 3000);
        }

        private void _elStrikeOppSet_Click(object sender, EventArgs e)
        {
            _communicator.SetElectricStrike(ActuatorType.ElectricStrikeOpposite, (byte)_elStrikeOppOutput.SelectedIndex,
                (_elStrikeOppImpulse.Checked ? StrikeType.Impulse : StrikeType.Level), (UInt32)_elStrikeOppPulseTime.Value,
                _elStrikeInverted.Checked, 0, 0);
        }

        private void _extraElStrikeSet_Click(object sender, EventArgs e)
        {
            _communicator.SetElectricStrike(ActuatorType.ExtraElectricStrike, (byte)_extraElStrikeOutput.SelectedIndex,
                (_extraElStrikeImpulse.Checked ? StrikeType.Impulse : StrikeType.Level), (UInt32)_extraElStrikePulseTime.Value,
                _extraElStrikeInverted.Checked, 0, 0);
        }

        private void _extraElStrikeOppSet_Click(object sender, EventArgs e)
        {
            _communicator.SetElectricStrike(ActuatorType.ExtraElectricStrikeOpposite, (byte)_extraElStrikeOppOutput.SelectedIndex,
                (_extraElStrikeOppImpulse.Checked ? StrikeType.Impulse : StrikeType.Level), (UInt32)_extraElStrikeOppPulseTime.Value,
                _extraElStrikeOppInverted.Checked, 0, 0);
        }

        private void _bypassAlarmSet_Click(object sender, EventArgs e)
        {
            _communicator.SetBypassAlarm((byte)_bypassAlarmOutput.SelectedIndex);
        }

        private void _elStrikeUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetActuator(ActuatorType.ElectricStrike);
        }

        private void _elStrikeOppUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetActuator(ActuatorType.ElectricStrikeOpposite);
        }

        private void _extraElStrikeUnset_Click(object sender, EventArgs e)
        {

            _communicator.UnsetActuator(ActuatorType.ExtraElectricStrike);
        }

        private void _extraElStrikeOppUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetActuator(ActuatorType.ExtraElectricStrikeOpposite);
        }

        private void _bypassAlarmUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetBypassAlarm();
        }

        private void _elStrikeImpulse_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                _elStrikePulseTime.Enabled = true;
            else
                _elStrikePulseTime.Enabled = false;
        }

        private void _elStrikeOppImpulse_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                _elStrikeOppPulseTime.Enabled = true;
            else
                _elStrikeOppPulseTime.Enabled = false;
        }

        private void _extraElStrikeImpulse_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                _extraElStrikePulseTime.Enabled = true;
            else
                _extraElStrikePulseTime.Enabled = false;
        }

        private void _extraElStrikeOppImpulse_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                _extraElStrikeOppPulseTime.Enabled = true;
            else
                _extraElStrikeOppPulseTime.Enabled = false;
        }

        private void _startStopDSM_Click(object sender, EventArgs e)
        {
            if (_dsmIsRunning)
            {
                _startStopDSM.Text = "Start";
                _dsmIsRunning = false;
                _communicator.StopDSM();
            }
            else /* DSM is not running -> start */
            {
                _startStopDSM.Text = "Stop";
                _dsmIsRunning = true;

                DoorEnviromentType doorEnviroment = DoorEnviromentType.Standard;
                switch (_dsmEnviromentType.SelectedIndex)
                {
                    case 0:
                        doorEnviroment = DoorEnviromentType.Standard;
                        break;
                    case 1:
                        doorEnviroment = DoorEnviromentType.Rotating;
                        break;
                    case 2:
                        doorEnviroment = DoorEnviromentType.StandardWithLocking;
                        break;
                    case 3:
                        doorEnviroment = DoorEnviromentType.StandardWithMaxOpened;
                        break;
                    case 4:
                        doorEnviroment = DoorEnviromentType.Minimal;
                        break;
                    default:
                        return;
                }

                _communicator.StartDSM(doorEnviroment);   
            }
        }

        private void _forceUnlockedState_CheckedChanged(object sender, EventArgs e)
        {
            _communicator.ForceUnlockedState(_forceUnlockedState.Checked);
        }

        #endregion  // DSM
                
        #region Outputs
        private bool _outputActivationDisabled = false;

        private void _activateAllOutputs_Click(object sender, EventArgs e)
        {
            _outputActivationDisabled = true;

            for (int i = 0; i < _outputCount; i++)
            {
                _communicator.ActivateOutput((byte)i, true);
                _outputActivations[i].Checked = true;
            }

            _outputActivationDisabled = false;
        }

        private void _deactivateAllOutputs_Click(object sender, EventArgs e)
        {
            _outputActivationDisabled = true;

            for (int i = 0; i < _outputCount; i++)
            {
                _communicator.ActivateOutput((byte)i, false);
                _outputActivations[i].Checked = false;
            }

            _outputActivationDisabled = false;
        }

        private void _forceAllOutputsOff_Click(object sender, EventArgs e)
        {
            try
            {
                _outputActivationDisabled = true;

                for (int i = 0; i < _outputCount; i++)
                {
                    _communicator.ForceOutputOff((byte)i);
                    _outputActivations[i].Checked = false;
                }

                _outputActivationDisabled = false;
            }
            catch (Exception error)
            {
                Dialog.Warning("Error during forcing the outputs to off\n"+error.Message);
            }
        }

        private void OnActivateOutput(object sender, EventArgs e)
        {
            if (_outputActivationDisabled)
                return;

            try
            {
                CheckBox checkbox = (CheckBox)sender;
                string name = checkbox.Name;

                name = name.Replace("_actOutput_", "");
                _communicator.ActivateOutput((byte)int.Parse(name), checkbox.Checked);
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Activate output error: " + error.Message);
            }
        }

        private void _outputFunctionality_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableOutputConfig((OutputType)_outputFunctionality.SelectedIndex);
        }

        private void _outputConfigSet_Click(object sender, EventArgs e)
        {
            if (_outputConfig.SelectedIndex == -1)
            {
                MessageBox.Show("No output selected.", "Output configuration error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            OutputType outType = (OutputType)_outputFunctionality.SelectedIndex;

            switch (outType)
            {
                case OutputType.Level:
                    _communicator.ConfigLevel((byte)_outputConfig.SelectedIndex, (UInt32)_outputDelayToOn.Value,
                        (UInt32)_outputDelayToOff.Value, _outputInverted.Checked);
                    break;

                case OutputType.Frequency:
                    _communicator.ConfigFrequency((byte)_outputConfig.SelectedIndex, (UInt32)_outputOnTime.Value,
                        (UInt32)_outputOffTime.Value, (UInt32)_outputDelayToOn.Value, (UInt32)_outputDelayToOff.Value,
                        _outputForcedOff.Checked, _outputInverted.Checked);
                    break;
                    
                case OutputType.Impulse:
                    _communicator.ConfigPulse((byte)_outputConfig.SelectedIndex, (UInt32)_outputPulseTime.Value,
                        (UInt32)_outputDelayToOn.Value, (UInt32)_outputDelayToOff.Value, _outputForcedOff.Checked,
                        _outputInverted.Checked);
                    break;
            }
        }

        private int[] GetCheckedOutputs(string basename, Panel group)
        {
            if (_outputCount == -1)
                return null;

            List<int> repOutputs = new List<int>();
            for (int i = 0; i < _outputCount; i++)
            {
                CheckBox checkbox = null;
                try
                {
                    checkbox = (CheckBox)group.Controls[basename + i];
                }
                catch { }

                if (checkbox != null && checkbox.Checked)
                    repOutputs.Add(i);
            }

            return repOutputs.ToArray();
        }

        private void _reportedOutputSet_Click(object sender, EventArgs e)
        {
            _communicator.SetReportedOutputs(GetCheckedOutputs("_reportOutput_", _reportedOutputsGroup));
        }

        private void _reportedOutputsSetEx_Click(object sender, EventArgs e)
        {
            _communicator.SetReportedOutputsEx(GetCheckedOutputs("_reportOutput_", _reportedOutputsGroup));
        }

        private void _unsetReportedOutputs_Click(object sender, EventArgs e)
        {
            _communicator.UnsetReportedOutputs(GetCheckedOutputs("_reportOutput_", _reportedOutputsGroup));
        }

        private void _reportedOutputsLogicSetEx_Click(object sender, EventArgs e)
        {
            _communicator.SetReportedOutputsLogicEx(GetCheckedOutputs("_reportOutputLogic_", _reportedOutputsLogicGroup));
        }

        private void _reportedOutputsLogicSet_Click(object sender, EventArgs e)
        {
            _communicator.SetReportedOutputsLogic(GetCheckedOutputs("_reportOutputLogic_", _reportedOutputsLogicGroup));
        }

        private void _reportedOutputLogicUnset_Click(object sender, EventArgs e)
        {
            _communicator.UnsetReportedOutputsLogic(GetCheckedOutputs("_reportOutputLogic_", _reportedOutputsLogicGroup));
        }

        #endregion  // Outputs
        
        #region Inputs

        private void _inputBalanced_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                _inputTamperDelay.Enabled = true;
                _inputTamperDelayLabel.Enabled = true;
            }
            else
            {
                _inputTamperDelay.Enabled = false;
                _inputTamperDelayLabel.Enabled = false;
            }
        }

        private void SetBalancedInput(int index)
        {
            if (_inputBalanced.Checked)
            {
                _communicator.SetBSIParams((byte)index, (UInt32)_inputFiltertime.Value,
                    (UInt32)_inputDelayToOn.Value, (UInt32)_inputDelayToOff.Value, (UInt32)_inputTamperDelay.Value);
                if (_inputInverted.Checked)
                {
                    _communicator.RemapBSI((byte)index,
                        InputState.Short, InputState.Alarm, InputState.Normal, InputState.Break);
                }
                else
                {
                    _communicator.RemapBSI((byte)index,
                        InputState.Short, InputState.Normal, InputState.Alarm, InputState.Break);
                }
            }
            else
            {
                _communicator.SetDIParams((byte)index, (UInt32)_inputFiltertime.Value,
                    (UInt32)_inputDelayToOn.Value, (UInt32)_inputDelayToOff.Value);
                if (_inputInverted.Checked)
                {
                    _communicator.RemapDI((byte)index, InputState.Normal, InputState.Alarm);
                }
                else
                {
                    _communicator.RemapDI((byte)index, InputState.Alarm, InputState.Normal);
                }
            }
        }

        private void _inputConfigSetAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _inputCount; i++)
                SetBalancedInput(i);
        }

        private void _inputConfigSet_Click(object sender, EventArgs e)
        {
            if (_inputConfig.SelectedIndex == -1)
            {
                MessageBox.Show("No input selected.", "Input configuration error", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            SetBalancedInput(_inputConfig.SelectedIndex);
        }

        private void _inputConfigUnset_Click(object sender, EventArgs e)
        {
            if (_inputConfig.SelectedIndex == -1)
            {
                MessageBox.Show("No input selected.", "Input configuration error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _communicator.UnsetInput((byte)_inputConfig.SelectedIndex);
        }

        private void _bsiLevelSet_Click(object sender, EventArgs e)
        {
            try
            {
                _communicator.SetBSILevels((UInt16)_bsiToLevel1.Value, (UInt16)_bsiToLevel2.Value, (UInt16)_bsiToLevel3.Value);
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Levels have to be in range: 0 - 1023", "BSI levels configuration error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("To Level 1 must be lower than To Level 2\nand To Level 2 must be lower than To Level 3",
                    "BSI levels configuration error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnOutputAssigned(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            string id = checkbox.Name.Replace("_assOutput_", "");
            try
            {

                int outputID = int.Parse(id);

                if (checkbox.Checked)
                {
                    _communicator.BindOutputToInput((byte)outputID, (byte)_inputConfig.SelectedIndex);
                }
                else
                {
                    _communicator.UnbindOutputFromInput((byte)outputID, (byte)_inputConfig.SelectedIndex);
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Assignign output to input: " + error.Message);
            }
        }

        private void _setReportedInputs_Click(object sender, EventArgs e)
        {
            _communicator.SetReportedInputs(GetCheckedOutputs("_reportInput_", _reportedInputsGroup));
        }

        private void _unsetReportedInputs_Click(object sender, EventArgs e)
        {
            _communicator.UnsetReportedInputs(GetCheckedOutputs("_reportInput_", _reportedInputsGroup));
        }

        private void _setReportedInputsEx_Click(object sender, EventArgs e)
        {
            _communicator.SetReportedInputsEx(GetCheckedOutputs("_reportInput_", _reportedInputsGroup));
        }
       
        #endregion

        #region Upgrade

        private DateTime _upgradeSw;

        private void _jumpToBootloader_Click(object sender, EventArgs e)
        {
            _communicator.ResetToBootloader();
        }

        private void _jumpToApplication_Click(object sender, EventArgs e)
        {
            _communicator.ResetToApplication();
        }

        private void _upgradeStart_Click(object sender, EventArgs e)
        {
            _upgradeSw = DateTime.Now;            

            string filename = _upgradeBrowser.Path;
            /*
            if (!File.Exists(filename))
            {
                ShowError(_upgradeBrowser, "No valid file selected");
                return;
            }
            */
            _communicator.UpgradeNode(filename);
        }

        #endregion

        private void _restartButton_Click(object sender, EventArgs e)
        {
            _communicator.RestartNode();
        }

        private void _doIt_Click(object sender, EventArgs e)
        {
            _communicator.DoIt();
        }

        #region CR
        private CRAccessCommands GetAccessCommands()
        {
            CRAccessCommands crac = _communicator.NodeCommunicator.GetCrAccessCommands(_communicator.SelectedNode);
            if (crac == null)
            {
                Dialog.Error("No slave node has been selected");
                return null;
            }
            else
                return crac;
        }

        private CRUpgradeCommands GetUpgradeCommands()
        {
            CRUpgradeCommands cruc = _communicator.NodeCommunicator.GetCrUpgradeCommands(_communicator.SelectedNode);
            if (cruc == null)
            {
                Dialog.Error("No slave node has been selected");
                return null;
            }
            else
                return cruc;
        }

        private CRMenuCommands GetMenuCommands()
        {
            CRMenuCommands crac = _communicator.NodeCommunicator.GetCrMenuCommands(_communicator.SelectedNode);
            if (crac == null)
            {
                Dialog.Error("No slave node has been selected");
                return null;
            }
            else
                return crac;
        }

        private CRMifareSpecificCommands GetCrMifareSpecificCommands()
        {
            CRMifareSpecificCommands crmsc = _communicator.NodeCommunicator.GetCrMifareSpecificCommands(_communicator.SelectedNode);
            if (crmsc == null)
            {
                Dialog.Error("No slave node has been selected");
                return null;
            }
            else
                return crmsc;
        }

        private CRControlCommands GetControlCommands()
        {
            CRControlCommands crcc = _communicator.NodeCommunicator.GetCrControlCommands(_communicator.SelectedNode);
            if (crcc == null)
            {
                Dialog.Error("No slave node has been selected");
                return null;
            }
            else
                return crcc;
        }

        private CRSetCommands GetSetCommands()
        {
            CRSetCommands crcc = _communicator.NodeCommunicator.GetCrSetCommands(_communicator.SelectedNode);
            if (crcc == null)
            {
                Dialog.Error("No slave node has been selected");
                return null;
            }
            else
                return crcc;
        }

        private CardReader GetSelectedCR()
        {
            CardReader cr;
            try
            {
                cr = (CardReader)_lbCardReaders.SelectedItem;
                if (cr == null)
                {
                    Dialog.Error("No card reader has been selected");
                    return null;
                }
                else
                    return cr;
            }
            catch
            {
                Dialog.Error("Invalid card reader selection");
                return null;
            }
        }

        private void _bWaitForCard_Click(object sender, EventArgs e)
        {
            CRAccessCommands crac = GetAccessCommands();
            if (null == crac)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            crac.WaitingForCard(cr);
        }

        private void _bWaitForPIN_Click(object sender, EventArgs e)
        {
            CRAccessCommands crac = GetAccessCommands();
            if (null == crac)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            crac.WaitingForPIN(cr);
        }

        private void _setTimeButton_Click(object sender, EventArgs e)
        {
            _communicator.SetTime((int)_timeHours.Value, (int)_timeMinutes.Value, (int)_timeSeconds.Value);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CRAccessCommands crac = GetAccessCommands();
            if (null == crac)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            crac.WaitingForGIN(cr);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            CRControlCommands crcc = GetControlCommands();
            if (null == crcc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            crcc.Reset(cr);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            CRSetCommands crsc = GetSetCommands();
            if (null == crsc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            crsc.SetTime(cr,0,cr.MaxDisplayLinesCount-1,_chb24hTimeFormat.Checked);
        }

        #endregion

        private void _portList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label51_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void _startHWIOTest_Click(object sender, EventArgs e)
        {
            _communicator.StartIOTest();
        }

        private void ShowPreview(Bitmap image, string caption)
        {
            Form preview = new Form();
            preview.Text = caption;
            PictureBox dipBox = new PictureBox();
            dipBox.Location = new Point(0, 0);
            dipBox.Image = image;
            dipBox.Size = new Size(image.Width, image.Height);

            preview.Controls.Add(dipBox);
            preview.Size = new Size(dipBox.Width + 10, dipBox.Height + 40);

            preview.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ShowPreview(Resource1.DIP, "DIP switch");
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            ShowPreview(Resource1.tamper, "Tamper");
        }

        private void _refreshMemoryLoadButton_Click(object sender, EventArgs e)
        {
            /*if (_communicator.SelectedNode != 0)
            {
                SlaveNode sn = _communicator.NodeCommunicator[_communicator.SelectedNode];
            }*/

            _communicator.ReadMemoryLoad();
        }

        private void _debugSeqNumbersButton_Click(object sender, EventArgs e)
        {
            _communicator.StartSeqDebug();
        }

        private void _assignCRs_Click(object sender, EventArgs e)
        {
            _communicator.AssignCRs(_assignedCR1.Checked, 
                _chbCr1LedSpecial.Checked,
                _assignedCR2.Checked,
                _chbCr2LedSpecial.Checked
                );
        }

        private void _suppressCR1_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressCR1.Checked)
                _communicator.SuppressCardReader(1);
            else
                _communicator.LooseCardReader(1);
        }

        private void _suppressCR2_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressCR2.Checked)
                _communicator.SuppressCardReader(2);
            else
                _communicator.LooseCardReader(2);
        }

        private void _fastDSMSetup_Click(object sender, EventArgs e)
        {
            _communicator.FastDSMSetup();
        }

        private void _setImplicitCode1_Click(object sender, EventArgs e)
        {
            _communicator.SetImplicitCRCode(1,_chbCr1LedSpecial.Checked);
        }

        private void _setImplicitCode2_Click(object sender, EventArgs e)
        {
            _communicator.SetImplicitCRCode(2,_chbCr2LedSpecial.Checked);
        }

        private void _bSend_Click(object sender, EventArgs e)
        {
            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            CRMifareSpecificCommands msc = GetCrMifareSpecificCommands();
            if (msc == null)
                return;

            byte cardSystemNumber = (byte)_eCardNumber.Value;
            byte[] AID = GetByteFromString(_eAid.Text);
            if (AID == null || AID.Length != 2)
            {
                AID = new byte[2];
            }
            byte[] akey = GetByteFromString(_eAkey.Text);
            byte[] bkey = GetByteFromString(_eBkey.Text);
            byte bank1Sector = (byte)_eBank1Sector.Value;
            byte bank1Offset = (byte)_eBank1Offset.Value;
            byte bank1Length = (byte)_eBank1Length.Value;
            byte bank2Sector = (byte)_eBank2Sector.Value;
            byte bank2Offset = (byte)_eBank2Offset.Value;
            byte bank2Length = (byte)_eBank2Length.Value;
            byte bank3Sector = (byte)_eBank3Sector.Value;
            byte bank3Offset = (byte)_eBank3Offset.Value;
            byte bank3Length = (byte)_eBank3Length.Value;

            msc.CardSystemData(cr, cardSystemNumber, AID, akey, bkey,
                bank1Sector, bank1Offset, bank1Length, bank2Sector, bank2Offset,
                bank2Length, bank3Sector, bank3Offset, bank3Length);
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            CRMifareSpecificCommands msc = GetCrMifareSpecificCommands();
            if (msc == null)
                return;

            msc.RemoveAllCardSystem(cr);
        }

        private byte[] GetByteFromString(string str)
        {
            byte[] result = new byte[str.Length / 2];

            string ps = string.Empty;
            int position = 0;
            for (int i = 0; i < str.Length; i++)
            {
                ps += str[i];
                if ( (i+1) % 2 == 0)
                {
                    result[position] = Convert.ToByte(ps, 16);
                    position++;
                    ps = string.Empty;
                }
            }

            return result;
        }

        void NodeCommunicator_CRCardSwiped(SlaveNode parameter1, CardReader parameter2, string parameter3,int cardSystem)
        {
            AddToCardData(parameter3);
        }

        private void AddToCardData(string str)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new IwQuick.DType2Void<string>(AddToCardData), str);
            }
            else
            {
                _eCardData.Text += str + Environment.NewLine;
            }
        }

        private void _bLowMenu_Click(object sender, EventArgs e)
        {
            CRMenuCommands crmc = GetMenuCommands();
            if (null == crmc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            GetAccessCommands().WaitingForGIN(cr,6);
            crmc.SetLowMenuButtons(cr,
                CRLowMenuButton.Down, CRSpecialKey.Down, CRLowMenuButton.Up, CRSpecialKey.Up,
                CRLowMenuButton.Home, CRSpecialKey.Unlock, CRLowMenuButton.Unknown, CRSpecialKey.No);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            CRMenuCommands crmc = GetMenuCommands();
            if (null == crmc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            GetAccessCommands().WaitingForCard(cr);
            crmc.SetLowMenuButtons(cr,
                CRLowMenuButton.Up, CRSpecialKey.Up, CRLowMenuButton.Unknown, CRSpecialKey.Up,
                CRLowMenuButton.Yes, CRSpecialKey.Yes, CRLowMenuButton.No, CRSpecialKey.No);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            CRAccessCommands crac = GetAccessCommands();
            if (null == crac)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            crac.WaitingForCard(cr);
            crac.WaitingForGIN(cr);
            crac.WaitingForPIN(cr);
            crac.WaitingForCard(cr);
            crac.WaitingForGIN(cr);
            crac.WaitingForPIN(cr);
        }

        private void _bCRStartUpgrade_Click(object sender, EventArgs e)
        {
            CRUpgradeCommands cruc = GetUpgradeCommands();
            if (null == cruc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            cruc.EnterFlashService(cr, CRUpgradeTargetArea.MainApplicationCode);
        }

        private void _bFlashTestSingle_Click(object sender, EventArgs e)
        {

            /*CRUpgradeCommands cruc = GetUpgradeCommands();
            if (null == cruc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            int dataPacketNumber = 0;
            int targetAddress = 0x100;

            byte[] helperBuffer = new byte[64];


            cruc.FlashUploadSingle(cr, dataPacketNumber++, targetAddress, helperBuffer);*/
        }

        private void button19_Click(object sender, EventArgs e)
        {
            CRUpgradeCommands cruc = GetUpgradeCommands();
            if (null == cruc)
                return;

            CardReader cr = GetSelectedCR();
            if (null == cr)
                return;

            cruc.StartUpgrade(cr,@"C:\CCrSdp5536-upgrade.bin");
        }

        
    }
}
