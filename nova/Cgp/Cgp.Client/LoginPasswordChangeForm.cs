using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using System.Collections;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Client
{
    public partial class LoginPasswordChangeForm : CgpTranslateForm
    {
        bool _cancelEnabled = true;

        public bool CancelEnabled
        {
            get { return _cancelEnabled; }
            
            set 
            {
                if (value==false)
                {
                    _bCancel.Enabled = false;
                }
                else
                {
                    _bCancel.Enabled = true;
                }
                _cancelEnabled = value; 
            }
        }

        public LoginPasswordChangeForm(string login)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            _eLogin.Text = login;
            _eLogin.Enabled = false;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormKeyDown);
        }

        void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        //private static volatile LoginPasswordChangeForm _singleton = null;
        // private static object _syncRoot = new object();
        //public static LoginPasswordChangeForm Singleton
        //{
        //    get
        //    {
        //        if (null == _singleton)
        //        lock(_syncRoot)
        //        {
        //            if (null == _singleton)
        //            _singleton = new LoginPasswordChangeForm();
        //        }
        //        return _singleton;
        //    }
        //}
                
        /// <summary>
        /// Check if all necesary values are present
        /// </summary>
        /// <returns></returns>
        private bool ValuesMiss()
        {
            //if (_eLogin.Text == string.Empty)
            //{
            //    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eLogin,
            //           GetString("ErrorDateFailed"), Contal.IwQuick.UI.ControlNotificationSettings.DefaultError);
            //    return true;
            //}
            //if (_eOldPassword.Text == string.Empty)
            //{
            //    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eOldPassword,
            //           GetString("ErrorDateFailed"), Contal.IwQuick.UI.ControlNotificationSettings.DefaultError);
            //    return true;
            //}
            if (_eNewPassword.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNewPassword,
                       GetString("ErrorMissNewPassword"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return true;
            }
            if (_eConfirmPassword.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eConfirmPassword,
                       GetString("ErrorMissConfirmPasswor"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Both new passwords must by same
        /// </summary>
        /// <returns>true if is correctly set new password</returns>
        private bool IsCoorectNewPassword()
        {
            if (_eNewPassword.Text != _eConfirmPassword.Text)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNewPassword,
                       GetString("ErrorPasswordsNotSame"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// save new password to database
        /// </summary>
        /// <returns>true if success</returns>
        private bool SaveChanges()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return false;

            if (_eOldPassword.Text != _eNewPassword.Text)
            {
                Exception error;
                if (!CgpClient.Singleton.MainServerProvider.Logins.ChengeLoginPassword(_eLogin.Text,
                    PasswordHelper.GetPasswordhash(_eLogin.Text, _eOldPassword.Text), 
                    PasswordHelper.GetPasswordhash(_eLogin.Text, _eNewPassword.Text), 
                    out error))
                {
                    if (error != null && error is Contal.IwQuick.InvalidSecureEntityException && (error as Contal.IwQuick.InvalidSecureEntityException).Reason == Contal.Cgp.BaseLib.LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD)
                    {
                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eOldPassword,
                            GetString("ErrorWrongPasswords"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSaveLogin"));
                    }

                    return false;
                }
            }
            else
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorUseNewPassword"));
                return false;
            }

            return true;
        }

        //public void ShowLoginPasswordChangeDialog(string login)
        //{
        //    _eLogin.Text = login;
        //    Show();
        //}

        private void ClearFilds()
        {
            _eLogin.Text = string.Empty;
            _eNewPassword.Text = string.Empty;
            _eOldPassword.Text = string.Empty;
            _eConfirmPassword.Text = string.Empty;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (ValuesMiss()) return;
            if (!IsCoorectNewPassword()) return;
            if (!SaveChanges())
            {
                return;
            }
            ClearFilds();
            //Hide();
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            ClearFilds();
            ClearFilds();
            //Hide();
            Close();
        }
    }
}
