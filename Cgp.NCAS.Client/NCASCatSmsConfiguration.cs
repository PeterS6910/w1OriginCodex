using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASCatSmsConfiguration : 
#if DESIGNER    
        Form
#else
 PluginMainForm<NCASClient>
#endif
    {
        private PresentationGroup _presentationGroup;
        private readonly IExtendedCgpEditForm _cgpEditForm;
        private bool _loadedValues;
        private AlarmTransmitter _actAlarmTransmitter;

        public NCASCatSmsConfiguration(Control control, IExtendedCgpEditForm cgpEditForm, PresentationGroup presentationGroup)
            : base (NCASClient.LocalizationHelper, CgpClientMainForm.Singleton)
        {
            InitializeComponent();
            TopLevel = false;
            control.Controls.Add(this);
            Dock = DockStyle.Fill;
            AutoSize = false;

            _presentationGroup = presentationGroup;
            _cgpEditForm = cgpEditForm;
            _cgpEditForm.EditingObjectChanged += _cgpEditForm_EditingObjectChanged;

            SetValues();
        }

        void _cgpEditForm_EditingObjectChanged(object obj)
        {
            _presentationGroup = (PresentationGroup) obj;
            SetValues();
        }

        public override NCASClient Plugin
        {
            get { return NCASClient.Singleton; }
        }

        private void SetValues()
        {
            if (_presentationGroup == null)
                return;

            _actAlarmTransmitter = Plugin.MainServerProvider.AlarmTransmitters.GetObjectById(_presentationGroup.AlarmTransmitterId);

            if (_actAlarmTransmitter != null)
            {
                _tbmCat.Text = _actAlarmTransmitter.Name;
                _tbmCat.TextImage = Plugin.GetImageForObjectType(ObjectType.AlarmTransmitter);
            }

            if (!string.IsNullOrEmpty(_presentationGroup.MessagesPrefix))
                _tbSmsMessage.Text = _presentationGroup.MessagesPrefix;

            _lbPhoneNumbers.Items.Clear();
            if (!string.IsNullOrEmpty(_presentationGroup.PhoneNumbers))
            {
                var phoneNumbers = _presentationGroup.PhoneNumbers.Split(';');

                if (phoneNumbers != null && phoneNumbers.Length > 0)
                {
                    foreach (string phoneNumber in phoneNumbers)
                    {
                        _lbPhoneNumbers.Items.Add(phoneNumber);
                    }
                }
            }

            _loadedValues = true;
        }

        private void _tbmCat_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                SetAlarmTransmitter();
            }
            else if (item.Name == "_tsiRemove")
            {
                _tbmCat.Text = string.Empty;
                _presentationGroup.AlarmTransmitterId = Guid.Empty;
                _cgpEditForm.ExtendedEditTextChangerOnlyInDatabase(null, null);
            }
        }

        private void SetAlarmTransmitter()
        {
            try
            {
                var listModObj = new List<IModifyObject>();
                Exception error;
                var listAlarmTransmitters = Plugin.MainServerProvider.AlarmTransmitters.ListModifyObjects(out error);
                
                if (error != null) 
                    throw error;
                
                listModObj.AddRange(listAlarmTransmitters);
                var formAdd = new ListboxFormAdd(listModObj, GetString("PresentationGroupEditFormSetFormatterFormText"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);

                if (outModObj != null)
                {
                    _actAlarmTransmitter = Plugin.MainServerProvider.AlarmTransmitters.GetObjectById(outModObj.GetId);
                    _presentationGroup.AlarmTransmitterId = _actAlarmTransmitter.IdAlarmTransmitter;
                    _tbmCat.Text = _actAlarmTransmitter.Name;
                    _tbmCat.TextImage = Plugin.GetImageForObjectType(ObjectType.AlarmTransmitter);

                    _cgpEditForm.ExtendedEditTextChangerOnlyInDatabase(null, null);
                    CgpClientMainForm.Singleton.AddToRecentList(_actAlarmTransmitter);
                }
            }
            catch
            {
                IwQuick.UI.Dialog.Error(GetString("ErrorAddFormatter"));
            }
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_tbNewPhoneNumber.Text))
                return;

            string editedNumber = _tbNewPhoneNumber.Text.Replace("/", "").Replace(" ", "");
            bool incorrectPhoneNumber = false;

            if (editedNumber.Substring(0, 1) == "+")
            {
                if (!editedNumber.Substring(1, editedNumber.Length - 1).All(char.IsDigit)
                    || editedNumber.Length >= 20)
                {
                    incorrectPhoneNumber = true;
                }
            }
            else if (!editedNumber.All(char.IsDigit)
                || editedNumber.Length >= 21)
            {
                incorrectPhoneNumber = true;
            }

            if (incorrectPhoneNumber)
            {
                IwQuick.UI.ControlNotification.Singleton.Error(IwQuick.UI.NotificationPriority.JustOne, _tbNewPhoneNumber,
                        GetString("ErrorInvalidPhoneNumberFormat"), CgpClient.Singleton.ClientControlNotificationSettings);

                return;
            }

            if (_lbPhoneNumbers.Items.OfType<string>().Any(number => number == _tbNewPhoneNumber.Text))
            {
                _tbNewPhoneNumber.Text = string.Empty;
                return;
            }

            _lbPhoneNumbers.Items.Add(_tbNewPhoneNumber.Text);
            _tbNewPhoneNumber.Text = string.Empty;

            SetPhoneNumbers();
        }

        private void SetPhoneNumbers()
        {
            if (_lbPhoneNumbers.Items.Count == 0)
            {
                _presentationGroup.PhoneNumbers = string.Empty;
                _cgpEditForm.ExtendedEditTextChangerOnlyInDatabase(null, null);
                return;
            }

            var phoneNumbers = new StringBuilder();

            foreach (var item in _lbPhoneNumbers.Items)
            {
                phoneNumbers.Append(string.Format("{0};", item));
            }

            phoneNumbers.Remove(phoneNumbers.Length - 1, 1);

            _presentationGroup.PhoneNumbers = phoneNumbers.ToString();
            _cgpEditForm.ExtendedEditTextChangerOnlyInDatabase(null, null);
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            var selectedItem = _lbPhoneNumbers.SelectedItem;

            if (selectedItem != null)
                _lbPhoneNumbers.Items.Remove(selectedItem);

            SetPhoneNumbers();
        }

        private void _bTestSms_Click(object sender, EventArgs e)
        {
        }

        private void _tbSmsMessage_TextChanged(object sender, EventArgs e)
        {
            if (_loadedValues)
            {
                _presentationGroup.MessagesPrefix = _tbSmsMessage.Text;
                _cgpEditForm.ExtendedEditTextChangerOnlyInDatabase(null, null);
            }
        }

        private void _tbmCat_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actAlarmTransmitter != null)
                NCASAlarmTransmittersForm.Singleton.OpenEditForm(_actAlarmTransmitter);
        }
    }
}
