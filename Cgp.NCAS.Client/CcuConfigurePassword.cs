using System;
using System.Drawing;
using System.Windows.Forms;
using Contal.Cgp.Client;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class CcuConfigurePassword :
#if DESIGNER
        Form
#else
        CgpTranslateForm
#endif
    {
        private string _password;
        private Guid _guidCcu = Guid.Empty;
        NCASClient _plugin;
        private bool _onlyCancelPassword;

        public CcuConfigurePassword(LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();
            _gbOldPassword.Visible = false;
            //this.Height = 175;
        }

        public CcuConfigurePassword(LocalizationHelper localizationHelper, Guid idCcu, NCASClient clientPlugin, bool onlyCancelPassword)
            : base(localizationHelper)
        {
            InitializeComponent();
            _guidCcu = idCcu;
            _plugin = clientPlugin;
            _onlyCancelPassword = onlyCancelPassword;

            if (_onlyCancelPassword)
            {
                _gbPassword.Visible = false;
                _gbPassword.Height = 0;
            }
        }

        private void ControlAlreadyEnterdCcuPassword()
        {
            try
            {
                bool hasPwd = _plugin.MainServerProvider.CCUs.HasCcuConfigurationPassword(_guidCcu);

                if (!hasPwd)
                {
                    _guidCcu = Guid.Empty;
                    HidePwdVerification();
                }
            }
            catch { }
        }

        private void HidePwdVerification()
        {
            this.BeginInvokeInUI(() =>
            {
                _gbOldPassword.Enabled = false;
                _eOldCcuPassword.PasswordChar = '\0';
                _eOldCcuPassword.Text = GetString("InfoCcuNoConfigurationPassword");
            });
        }

        public void ShowDialog(out string password)
        {
            SafeThread.StartThread(ControlAlreadyEnterdCcuPassword);
            password = string.Empty;
            _gbPassword.Text = LocalizationHelper.GetString("StringSetCcuPassword");           
            ShowDialog();
            password = _password;
        }

        public void ShowDialog(out string password, bool newPassword)
        {
            password = string.Empty;
            if (newPassword)
            {
                _gbPassword.Text = LocalizationHelper.GetString("StringNewCcuPassword");
            }
            else
            {
                _gbPassword.Text = LocalizationHelper.GetString("StringVerifyCcuPassword");
                _eVerifyCcuPassword.Visible = false;
                _lCcuRetypePassword.Visible = false;
                //_gbPassword.Height = 97;
                //_gbPassword.Location = new Point(12,12);
                //this.Height = 135;
            }

            ShowDialog();
            password = _password;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (!_onlyCancelPassword)
            {
                if (_eCcuPassword.Text.Length < 8)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCcuPassword,
                        GetString("ErrorConfigurePassword"),
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.RightTop));
                    _eCcuPassword.Focus();
                    return;
                }

                if (_eVerifyCcuPassword.Visible && _eCcuPassword.Text != _eVerifyCcuPassword.Text)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCcuPassword,
                        GetString("ErrorRetypeVerifyPassword"),
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.RightTop));
                    _eCcuPassword.Focus();
                    return;
                }
            }

            if (_guidCcu != Guid.Empty)
            {
                _bOk.Enabled = false;
                _oldPwd = _eOldCcuPassword.Text;
                SafeThread.StartThread(ConfirmCcuPassword);
                return;
            }

            _password = QuickHashes.GetSHA1String(_eCcuPassword.Text);
            Close();
        }

        string _oldPwd = string.Empty;
        private void ConfirmCcuPassword()
        {
            try
            {
                if (string.IsNullOrEmpty(_eOldCcuPassword.Text))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eOldCcuPassword,
                            GetString("ErrorEnterOldPassword"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.RightTop));
                    EnableOkButton();
                    return;
                }
                string hashPassword = QuickHashes.GetSHA1String(_oldPwd);
                bool validPwd = _plugin.MainServerProvider.CCUs.ValidConfigurePassword(_guidCcu, hashPassword);
                if (!validPwd)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eOldCcuPassword,
                           GetString("ErrorWrongOldPassword"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.RightTop));
                    EnableOkButton();
                    return;
                }
                CloseForm();
            }
            catch { }
        }

        private void EnableOkButton()
        {
            this.BeginInvokeInUI(() =>
            {
                _eOldCcuPassword.Focus();
                _bOk.Enabled = true;
            });
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _password = string.Empty;
            Close();
        }

        private void CloseForm()
        {
            try
            {
                Invoke(new EventHandler(delegate 
                    {
                        if (_onlyCancelPassword)
                        {
                            DialogResult = DialogResult.OK;
                        }

                        _password = QuickHashes.GetSHA1String(_eCcuPassword.Text);
                        Close(); 
                    }));
            }
            catch { }
        }

        private void SetMargin()
        {
            int margin = 5;
            int formClientSizeWidth = 2 * margin + _gbPassword.Width;
            int formClientSizeHeight = 0;

            if (_gbOldPassword.Visible)
                formClientSizeHeight = _gbOldPassword.Height + _gbPassword.Height + _bOk.Height + 4*margin;
            else
                formClientSizeHeight = _gbPassword.Height + _bOk.Height + 3 * margin;

            ClientSize = new Size(formClientSizeWidth, formClientSizeHeight);

            _bOk.Left = margin;
            _bCancel.Left = formClientSizeWidth - margin - _bCancel.Width;

            if (_gbOldPassword.Visible)
            {
                _gbOldPassword.Left = margin;
                _gbOldPassword.Top = margin;

                _gbPassword.Left = margin;
                _gbPassword.Top = _gbOldPassword.Top + _gbOldPassword.Height + margin;               
            }
            else
            {
                _gbPassword.Left = margin;
                _gbPassword.Top = margin;
            }

            _bOk.Top = _gbPassword.Top + _gbPassword.Height + margin;
            _bCancel.Top = _bOk.Top;
        }

        private void CcuConfigurePassword_Shown(object sender, EventArgs e)
        {
            SetMargin();
        }
    }
}
