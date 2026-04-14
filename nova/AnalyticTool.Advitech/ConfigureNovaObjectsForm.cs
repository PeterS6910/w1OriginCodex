using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.UI;

namespace AnalyticTool.Advitech
{
    public partial class ConfigureNovaObjectsForm : Form, IDialog
    {
        private readonly Dictionary<ComboBox, PictureBox> _warningIcons = new Dictionary<ComboBox, PictureBox>();
        private const string _inputIsNotReporting 
            = "Input state must be monitored either by enabling an alarm on it or assigning a presentation group with intependent reporting";

        public ConfigureNovaObjectsForm()
        {
            InitializeComponent();

            _warningIcons.Add(_cbInput1, pictureBox3);
            _warningIcons.Add(_cbInput2, pictureBox4);
            _warningIcons.Add(_cbInput3, pictureBox5);
            _warningIcons.Add(_cbInput4, pictureBox6);

            var toolTip = new ToolTip();
            toolTip.SetToolTip(pictureBox3, _inputIsNotReporting);
            toolTip.SetToolTip(pictureBox4, _inputIsNotReporting);
            toolTip.SetToolTip(pictureBox5, _inputIsNotReporting);
            toolTip.SetToolTip(pictureBox6, _inputIsNotReporting);

            LoadObjects();
        }

        #region IDialog Members

        public Form ResultDialog { get; private set; }

        #endregion

        private void LoadObjects()
        {
            var cardReaders = NovaServerHelper.Singleton.GetCardReaders().OrderBy(cr => cr.Name);

            if (cardReaders == null)
            {
                Dialog.Error("Server is disconnected");
                return;
            }

            _cbInputCardReaders.DataSource = new List<CardReader>(cardReaders);
            _cbOutputCardReaders.DataSource = new List<CardReader>(cardReaders);

            var inputs = NovaServerHelper.Singleton.GetInputs().OrderBy(inp => inp.FullName);

            if (inputs == null)
            {
                Dialog.Error("Server is disconnected");
                return;
            }

            _cbInput1.Items.Add(string.Empty);
            _cbInput1.Items.AddRange(inputs.ToArray());

            _cbInput2.Items.Add(string.Empty);
            _cbInput2.Items.AddRange(inputs.ToArray());

            _cbInput3.Items.Add(string.Empty);
            _cbInput3.Items.AddRange(inputs.ToArray());

            _cbInput4.Items.Add(string.Empty);
            _cbInput4.Items.AddRange(inputs.ToArray());

            SelectUsedCardReader(ApplicationProperties.Singleton.InputCardReader, _cbInputCardReaders);
            SelectUsedCardReader(ApplicationProperties.Singleton.OutputCardReader, _cbOutputCardReaders);

            SelectUsedInput(ApplicationProperties.Singleton.Pump1Input, _cbInput1);
            SelectUsedInput(ApplicationProperties.Singleton.Pump2Input, _cbInput2);
            SelectUsedInput(ApplicationProperties.Singleton.Pump3Input, _cbInput3);
            SelectUsedInput(ApplicationProperties.Singleton.Pump4Input, _cbInput4);

            if (ApplicationProperties.Singleton.DefaultStartDate.Year == 1)
                ApplicationProperties.Singleton.DefaultStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            _dtpStartDate.Value = ApplicationProperties.Singleton.DefaultStartDate;

            if (ApplicationProperties.Singleton.ScheduleType == 0)
            {
                _rbDaily.Checked = true;
                _dtpDailyValue.Value = (new DateTime(1900, 1, 1)).AddMinutes(ApplicationProperties.Singleton.ScheduleValue);
            }
            else
            {
                _rbPeriodic.Checked = true;
                _ePeriodicValue.Value = ApplicationProperties.Singleton.ScheduleValue;
            }
        }

        private void SelectUsedCardReader(Guid idUsedCardReader, ComboBox comboBox)
        {
            if (idUsedCardReader == Guid.Empty)
            {
                comboBox.SelectedIndex = 0;
                return;
            }

            foreach (var item in comboBox.Items)
            {
                var cardReader = item as CardReader;

                if (cardReader == null)
                    continue;

                if (cardReader.IdCardReader.Equals(idUsedCardReader))
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectUsedInput(Guid idUsedInput, ComboBox comboBox)
        {
            if (idUsedInput == Guid.Empty)
            {
                comboBox.SelectedIndex = 0;
                return;
            }

            foreach (var item in comboBox.Items)
            {
                var input = item as Input;

                if (input == null)
                    continue;

                if (input.IdInput.Equals(idUsedInput))
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            ResultDialog = new SelectDatabaseAndTableForm();
            DialogResult = DialogResult.Cancel;
        }

        private void _bFinish_Click(object sender, EventArgs e)
        {
            var inputCr = _cbInputCardReaders.SelectedItem as CardReader;

            ApplicationProperties.Singleton.InputCardReader = inputCr != null 
                ? inputCr.IdCardReader 
                : Guid.Empty;

            var outputCr = _cbOutputCardReaders.SelectedItem as CardReader;

            ApplicationProperties.Singleton.OutputCardReader = outputCr != null
                ? outputCr.IdCardReader
                : Guid.Empty;

            var input1 = _cbInput1.SelectedItem as Input;

            ApplicationProperties.Singleton.Pump1Input = input1 != null
                ? input1.IdInput
                : Guid.Empty;

            var input2 = _cbInput2.SelectedItem as Input;

            ApplicationProperties.Singleton.Pump2Input = input2 != null
                ? input2.IdInput
                : Guid.Empty;

            var input3 = _cbInput3.SelectedItem as Input;

            ApplicationProperties.Singleton.Pump3Input = input3 != null
                ? input3.IdInput
                : Guid.Empty;

            var input4 = _cbInput4.SelectedItem as Input;

            ApplicationProperties.Singleton.Pump4Input = input4 != null
                ? input4.IdInput
                : Guid.Empty;

            ApplicationProperties.Singleton.DefaultStartDate = _dtpStartDate.Value;

            ApplicationProperties.Singleton.ScheduleType = _rbDaily.Checked
                ? 0
                : 1;

            ApplicationProperties.Singleton.ScheduleValue = ApplicationProperties.Singleton.ScheduleType == 0
                ? _dtpDailyValue.Value.Hour*60 + _dtpDailyValue.Value.Minute
                : (int) _ePeriodicValue.Value;               

            if (!ControlSettings())
                return;

            ApplicationProperties.Singleton.SaveProperties();

            ResultDialog = null;
            DialogResult = DialogResult.OK;
        }

        private bool ControlSettings()
        {
            if (ApplicationProperties.Singleton.InputCardReader == ApplicationProperties.Singleton.OutputCardReader)
            {
                Dialog.Error("The input and output card readers can not be the same");
                return false;
            }

            var testHashSet = new HashSet<Guid>();

            if (ApplicationProperties.Singleton.Pump1Input != Guid.Empty)
                testHashSet.Add(ApplicationProperties.Singleton.Pump1Input);

            if (ApplicationProperties.Singleton.Pump2Input != Guid.Empty
                && !testHashSet.Add(ApplicationProperties.Singleton.Pump2Input))
            {
                Dialog.Error("The input for pump 2 is already used");
                return false;
            }

            if (ApplicationProperties.Singleton.Pump3Input != Guid.Empty
                && !testHashSet.Add(ApplicationProperties.Singleton.Pump3Input))
            {
                Dialog.Error("The input for pump 3 is already used");
                return false;
            }

            if (ApplicationProperties.Singleton.Pump4Input != Guid.Empty
                && !testHashSet.Add(ApplicationProperties.Singleton.Pump4Input))
            {
                Dialog.Error("The input for pump 4 is already used");
                return false;
            }

            if (testHashSet.Count == 0)
            {
                Dialog.Error("No input selected");
                return false;
            }

            return true;
        }

        private void _rbDaily_CheckedChanged(object sender, EventArgs e)
        {
            _dtpDailyValue.Enabled = _rbDaily.Checked;
            _ePeriodicValue.Enabled = !_rbDaily.Checked;
        }

        private void _rbPeriodic_CheckedChanged(object sender, EventArgs e)
        {
            _ePeriodicValue.Enabled = _rbPeriodic.Checked;
            _dtpDailyValue.Enabled = !_rbPeriodic.Checked;
        }

        private void _cbInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox) sender;
            var input = comboBox.SelectedItem as Input;

            if (input != null
                && !input.AlarmOn
                && input.AlarmPresentationGroup == null)
            {
                _warningIcons[comboBox].Visible = true;
            }
            else
            {
                _warningIcons[comboBox].Visible = false;
            }
        }
    }
}
