using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Contal.IwQuick.Net;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    enum Types
    {
        tBool,
        tByte,
        tShort,
        tInt,
        tLong,
        tFloat,
        tString,
        tOther
    }
    public partial class FormLicenceRequest : CgpTranslateForm
    {
        private const string PLUGIN_FILE_KEYWORD = ".plugin";
        private string _publisherName;
        private string _publisherKey;
        private string _commonName;
        private Dictionary<int, string> _propertyNames;
        private Dictionary<int, string> _propertyTypes;
        private Dictionary<int, string> _propertyValues;
        private Dictionary<int, string> _propertyDescriptions;
        private bool _filesValid = false;


        public FormLicenceRequest()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            _propertyNames = new Dictionary<int, string>();
            _propertyTypes = new Dictionary<int, string>();
            _propertyValues = new Dictionary<int, string>();
            _propertyDescriptions = new Dictionary<int, string>();
            CheckValidFiles();
            this.MinimumSize = new Size(this.Width, this.Height);
        }

        private void CheckValidFiles()
        {
            _filesValid = false;
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (CgpClient.Singleton.MainServerProvider.LoadLicenceRequestFields(out _publisherName, out _publisherKey, out _commonName,
                out _propertyNames, out _propertyTypes, out _propertyValues, out _propertyDescriptions))
            {
                _filesValid = true;
                CreatePropertyControls();
                _textBoxCustomerName.Focus();
            }
            else
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorReadLicencePublisherAndProperties"));
            }
        }

        private void CreatePropertyControls()
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;
            Dictionary<string, Type> allRequiredLicenceProperties = CgpClient.Singleton.MainServerProvider.GetAllRequiredLicenceProperties();
            Type propertyType = null;
            int controlIndex = 0;
            string localisedPropertyName = string.Empty;

            foreach (KeyValuePair<int, string> name in _propertyNames)
            {
                controlIndex++;
                Control inputControl = null;
                propertyType = null;
                Label label = null;

                if (name.Value.Contains(PLUGIN_FILE_KEYWORD))
                {
                    label = new Label();
                    label.AutoSize = true;
                    label.Location = new Point(_labelContactEmail.Location.X, (_groupBoxCustomerInfo.Location.Y + _groupBoxCustomerInfo.Size.Height) +
                        2 * controlIndex * (_labelContactEmail.Size.Height));
                    label.Name = "_propertyLabel" + name.Value;

                    label.Text = GetString("General_Enable") + StringConstants.SPACE;
                    if (CgpClient.Singleton.MainServerProvider.GetLicencePropertyLocalisedName(name.Value.Replace(".",""), LocalizationHelper.ActualLanguage, out localisedPropertyName))
                        label.Text += localisedPropertyName;
                    else
                        label.Text += name.Value;

                    Controls.Add(label);

                    inputControl = new CheckBox();
                    ((CheckBox)inputControl).Checked = true;
                    ((CheckBox)inputControl).Enabled = false;

                    propertyType = typeof(bool);
                }
                else
                {
                    label = new Label();
                    label.Name = "_propertyLabel" + name.Value;

                    if (CgpClient.Singleton.MainServerProvider.GetLicencePropertyLocalisedName(name.Value, LocalizationHelper.ActualLanguage, out localisedPropertyName))
                        label.Text = localisedPropertyName;
                    else
                        label.Text = name.Value;

                    label.AutoSize = true;
                    label.Location = new Point(_labelContactEmail.Location.X, (_groupBoxCustomerInfo.Location.Y + _groupBoxCustomerInfo.Size.Height) +
                        2 * controlIndex * (_labelContactEmail.Size.Height));
                    Controls.Add(label);

                    if (allRequiredLicenceProperties.ContainsKey(name.Value))
                        propertyType = allRequiredLicenceProperties[name.Value];
                    if (propertyType == typeof(int))
                    {
                        inputControl = new NumericUpDown();
                        ((NumericUpDown)inputControl).Minimum = 0;
                        ((NumericUpDown)inputControl).Maximum = 1000;
                    }
                    else if (propertyType == typeof(bool))
                        inputControl = new CheckBox();
                    else
                        inputControl = new TextBox();
                }

                inputControl.Name = "_propertyInput" + name.Value;
                inputControl.Tag = propertyType ?? typeof(string);
                inputControl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                inputControl.Location = new Point(_buttonSave.Location.X, label.Location.Y);

                Controls.Add(inputControl);
                this.Height = inputControl.Location.Y + 4 * _buttonSave.Size.Height;
            }
        }

        public DialogResult ShowDialogWithCheck()
        {
            if (_filesValid)
            {
                return base.ShowDialog();
            }
            return DialogResult.Ignore;
        }

        void textBox_Leave(object sender, EventArgs e)
        {
            ValidateTextBox(sender);
        }

        void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                if (ValidateTextBox(sender))
                {
                    SendKeys.Send("{TAB}");
                }
            }
        }

        private bool ValidateTextBox(object sender)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Tag != null)
            {
                try
                {
                    switch (GetType(_propertyTypes[(int)textBox.Tag]))
                    {
                        case Types.tBool:
                            bool.Parse(textBox.Text);
                            break;
                        case Types.tByte:
                            byte.Parse(textBox.Text);
                            break;
                        case Types.tShort:
                            short.Parse(textBox.Text);
                            break;
                        case Types.tInt:
                            int.Parse(textBox.Text);
                            break;
                        case Types.tLong:
                            long.Parse(textBox.Text);
                            break;
                        case Types.tFloat:
                            float.Parse(textBox.Text);
                            break;
                        case Types.tString:
                        case Types.tOther:
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    textBox.Text = "";
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, textBox, LocalizationHelper.GetString("GeneralRequestedType") + ": " + _propertyTypes[(int)textBox.Tag].ToString(),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.RightTop));
                    return false;
                }
            }
            _propertyValues[(int)textBox.Tag] = textBox.Text;
            return true;
        }

        private Types GetType(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "bool":
                    return Types.tBool;
                case "byte":
                    return Types.tByte;
                case "short":
                    return Types.tShort;
                case "int":
                    return Types.tInt;
                case "long":
                    return Types.tLong;
                case "float":
                    return Types.tFloat;
                case "string":
                    return Types.tString;
                default:
                    return Types.tOther;
            }

        }

        private void _buttonSave_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;
            if (!AreAllTextBoxesFilled())
                return;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "(Licence request) *.lreq | *.lreq";

            if (sf.ShowDialog() == DialogResult.OK)
            {
                FillPropertiesFromControls();
                int result = CgpClient.Singleton.MainServerProvider.GenerateLicenceRequestFile(_propertyNames, _propertyValues, _commonName, _textBoxCustomerName.Text,
                    _textBoxAddress.Text, _textBoxFirstName.Text, _textBoxLastName.Text, _textBoxPhone.Text, _textBoxEmail.Text, _publisherName, _publisherKey, sf.FileName);
                if (result == 0)
                {
                    Dialog.Error(LocalizationHelper.GetString("ErrorOnCreateRequest"));
                }
                else if (result == -1)
                {
                    Dialog.Error(LocalizationHelper.GetString("ErrorOnCreateRequestCIID"));
                }
                else
                {
                    Dialog.Info(LocalizationHelper.GetString("RequestSuccessfullyCreated"));
                }
            }
        }

        private bool AreAllTextBoxesFilled()
        {
            return AreAllTextBoxesFilled(this);
        }

        private void FillPropertiesFromControls()
        {
            int i = 0;
            Control currentControl = null;
            foreach (KeyValuePair<int, string> propertyName in _propertyNames)
            {
                i++;
                try
                {
                    currentControl = Controls.Find("_propertyInput" + propertyName.Value, false)[0];
                    if (currentControl == null)
                        continue;

                    if (currentControl.Tag == null)
                        _propertyValues[propertyName.Key] = ((TextBox)currentControl).Text;
                    else
                    {
                        if (currentControl.Tag == typeof(bool))
                            _propertyValues[propertyName.Key] = ((CheckBox)currentControl).Checked.ToString();
                        else if (currentControl.Tag == typeof(string))
                            _propertyValues[propertyName.Key] = ((TextBox)currentControl).Text;
                        else if (currentControl.Tag == typeof(int) || currentControl.Tag == typeof(byte))
                            _propertyValues[propertyName.Key] = ((NumericUpDown)currentControl).Value.ToString();
                    }
                }
                catch { }
            }
        }

        private bool AreAllTextBoxesFilled(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control.GetType().Name == typeof(TextBox).Name)
                {
                    if (((TextBox)control).Text == string.Empty)
                    {
                        ((TextBox)control).Focus();
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, control, LocalizationHelper.GetString("ErrorEmptyValue"),
                            ControlNotificationSettings.Default);
                        return false;
                    }
                }
                if (control.GetType().Name == typeof(GroupBox).Name)
                {
                    if (!AreAllTextBoxesFilled(control))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void _textBoxEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                if (_textBoxEmail.Text != string.Empty && !EmailAddress.IsValid(_textBoxEmail.Text))
                {
                    _textBoxEmail.Text = "";
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
        }

        private void _textBoxEmail_Leave(object sender, EventArgs e)
        {
            if (_textBoxEmail.Text != string.Empty && !EmailAddress.IsValid(_textBoxEmail.Text))
            {
                _textBoxEmail.Text = "";
            }
        }

        private void _textBoxCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                TextBox textBox = (TextBox)sender;
                if (textBox.Text == string.Empty)
                {
                    textBox.Focus();
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, textBox, LocalizationHelper.GetString("ErrorEmptyValue"),
                            ControlNotificationSettings.Default);
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
        }

        private void _buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
