using System;
using System.Windows.Forms;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.DBSCreator
{
    internal partial class FormDbsLogin : TranslateForm
    {
        private readonly DatabaseAccessor _databaseAccessor;
        private Control _unvalidatedControl;

        public FormDbsLogin(DatabaseAccessor databaseAccessor)
            : base(databaseAccessor.LocalizationHelper)
        {
            _databaseAccessor = databaseAccessor;
            InitializeComponent();
        }

        private void _rbExistLogin_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbExistLogin.Checked)
            {
                _eConfirmPassword.Text = string.Empty;
                _eConfirmPassword.Enabled = false;
            }
            else
            {
                _eConfirmPassword.Enabled = true;
            }
        }

        private void FormDbsLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            if (!ValidateChildren())
            {
                e.Cancel = true;

                if (_unvalidatedControl != null)
                    _unvalidatedControl.Focus();

                return;
            }

            if (!_databaseAccessor.PrepareCustomLogin(_rbExistLogin.Checked))
                e.Cancel = true;
        }

        private void _eName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_eName.Text != string.Empty)
                return;

            IwQuick.UI.ControlNotification.Singleton.Error(
                IwQuick.UI.NotificationPriority.JustOne,
                _eName,
                GetString("ErrorEntryLogin"),
                IwQuick.UI.ControlNotificationSettings.Default);

            _unvalidatedControl = _eName;
            e.Cancel = true;
        }

        private void _ePassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_ePassword.Text != string.Empty)
                return;

            IwQuick.UI.ControlNotification.Singleton.Error(
                IwQuick.UI.NotificationPriority.JustOne,
                _ePassword,
                GetString("ErrorEntryPassword"),
                IwQuick.UI.ControlNotificationSettings.Default);

            _unvalidatedControl = _ePassword;
            e.Cancel = true;
        }

        private void _eConfirmPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_rbCreateLogin.Checked || _eConfirmPassword.Text == _ePassword.Text)
                return;

            IwQuick.UI.ControlNotification.Singleton.Error(
                IwQuick.UI.NotificationPriority.JustOne,
                _eConfirmPassword,
                GetString("ErrorWrondConfirmPassword"),
                IwQuick.UI.ControlNotificationSettings.Default);

            _unvalidatedControl = _eConfirmPassword;
            e.Cancel = true;
        }
    }
}
