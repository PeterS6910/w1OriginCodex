using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.IwQuick.Localization;
using Contal.IwQuick;
using Contal.IwQuick.UI;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.IwQuick.Parsing;

namespace Contal.Cgp.Client
{
    public partial class LoginPasswordForm : CgpTranslateForm
    {
        private bool _isAuthenticated;
        private bool _isSessionExpired;
        private bool _isApplicationLock = false;

        private static volatile LoginPasswordForm _singleton = null;
        private static object _syncRoot = new object();

        public static LoginPasswordForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new LoginPasswordForm();
                    }

                return _singleton;
            }
        }

        public LoginPasswordForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();

            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = false;

            CgpClient.Singleton.CgpClientProxyGained += new DVoid2Void(OnCgpClientProxyGained);
            _gbCard.Parent = null;
            _bOk.Location = new Point(_bOk.Location.X, _authGroup.Location.Y + _authGroup.Height + 10);
            _bCancel.Location = new Point(_bCancel.Location.X, _authGroup.Location.Y + _authGroup.Height + 10);
            this.ClientSize = new Size(this.ClientRectangle.Width + WinFormsHelper.DpiScaleX(5), _bCancel.Location.Y + _bCancel.Height + 10);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormKeyDown);
            CgpClient.Singleton.LoginCrOnlineStateChanged += CardEnabled;
        }

        void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        void OnCgpClientProxyGained()
        {
            if (this.IsHandleCreated)
            {
                ShowCardReaderLoginOption(GeneralOptionsForm.Singleton.GetRequirePINCardLogin);
            }

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ShowCardReaderLoginOption(GeneralOptionsForm.Singleton.GetRequirePINCardLogin);
            CardEnabled(CgpClient.Singleton.IsCrOnline);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            SendCardReaderLoginSuccess(CardReaderSceneType.WaitingForCard);
            Close();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {            
            if (_eUserName.Text == string.Empty && _ePIN.Text != string.Empty)
            {
                TryLoginWithCard();
                return;
            }
            CheckLoginPassword();
        }

        private bool _authenticate = true;

        public void ShowDialogApplicationLocked(ref string userName)
        {
            _isApplicationLock = true;

            ShowDialog(
                ref userName, 
                false);
        }

        public DialogResult ShowDialog(
            ref string userName, 
            bool authenticate)
        {
            _authenticate = authenticate;

            if (Visible)
            {
                this.InvokeInUI(BringToFront);
                
                return DialogResult.None;
            }

            DialogResult = DialogResult.None;

            if (!CgpClient.Singleton.IsMainServerProviderAvailable)
                return DialogResult;

            _eUserName.Text = userName ?? String.Empty;
            _ePassword.Text = String.Empty;
            _lastCardSwiped = String.Empty;
            _ePIN.Text = String.Empty;

            _isAuthenticated = false;

            ShowDialog();

            if (DialogResult == DialogResult.Cancel)
                return DialogResult;

            userName = 
                Validator.IsNotNullString(_eUserName.Text) && _isAuthenticated
                    ? _eUserName.Text
                    : null;

            return DialogResult;
        }

        private ControlNotificationSettings _cns = new ControlNotificationSettings(
            NotificationParts.Hint, HintPosition.RightTop);

        private ControlNotification _notification = new ControlNotification();

        private object _lockCardLogin = new object();
        public void LoginWithCard(string card)
        {
            if (InvokeRequired)
            {
                Invoke(new Contal.IwQuick.DString2Void(LoginWithCard), card);
            }
            else
            {
                lock (_lockCardLogin)
                {
                    if (!this.Modal) return;
                    if (_isAuthenticated) return;
                    string loginName;
                    try
                    {
                        if (_authenticate)
                        {
                            Exception error = CgpClient.Singleton.MainServerProvider.ClientAutenticateWithCard(card, out loginName);
                            _eUserName.Text = loginName;
                            if (error != null)
                                throw (error);

                            Cgp.Server.Beans.Extern.EventlogParameter parameter = new Contal.Cgp.Server.Beans.Extern.EventlogParameter(Cgp.Server.Beans.Extern.EventlogParameter.TYPEUSERNAME, loginName);
                            IList<Cgp.Server.Beans.Extern.EventlogParameter> parameterList = new List<Cgp.Server.Beans.Extern.EventlogParameter>();
                            parameterList.Add(parameter);
                            //CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEvent(Cgp.Server.Beans.Extern.Eventlog.TYPECLIENTLOGIN,
                            //    this.GetType().Assembly.GetName().Name, null, "Client with USERNAME " + loginName + " login", parameterList);
                            CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEventClientLogin(loginName);
                        }
                        else
                        {
                            Exception error;
                            if (!CgpClient.Singleton.MainServerProvider.Logins.CheckLoginPassword(card, out loginName, out error))
                            {
                                if (error != null)
                                    throw error;
                                else
                                    throw new Exception();
                            }
                            _eUserName.Text = loginName;
                        }

                        _isAuthenticated = true;
                        DialogResult = DialogResult.OK;
                        SendCardReaderLoginSuccess(CardReaderSceneType.Accepted);
                        Close();
                    }
                    catch (Exception exception)
                    {
                        if (exception is InvalidSecureEntityException)
                        {
                            InvalidSecureEntityException error = (InvalidSecureEntityException)exception;

                            switch (error.Reason)
                            {
                                case LoginAuthenticationParameters.WRONG_CARD:
                                    _notification.Error(NotificationPriority.Last,
                                        _gbCard,
                                        GetString("ErrorUnknownWrongCard"),
                                        _cns);

                                    Cgp.Server.Beans.Extern.EventlogParameter parameter = new Contal.Cgp.Server.Beans.Extern.EventlogParameter(Cgp.Server.Beans.Extern.EventlogParameter.TYPEUSERNAME, card);
                                    IList<Cgp.Server.Beans.Extern.EventlogParameter> parameterList = new List<Cgp.Server.Beans.Extern.EventlogParameter>();
                                    parameterList.Add(parameter);
                                    CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEvent(Cgp.Server.Beans.Extern.Eventlog.TYPECLIENTLOGINWRONGPASSWORD, this.GetType().Assembly.GetName().Name, null, "Wrong card " + card, parameterList);
                                    break;
                                case LoginAuthenticationParameters.LOGIN_DISABLED:

                                    _notification.Invoke(
                                        _gbCard,
                                        GetString("ErrorLoginDisabled"),
                                        NotificationPriority.Last,
                                        NotificationSeverity.Error,
                                        _cns);
                                    break;
                                case LoginAuthenticationParameters.LOGIN_EXPIRED:
                                    _notification.Invoke(
                                        _gbCard,
                                        GetString("ErrorLoginExpired"),
                                        NotificationPriority.Last,
                                        NotificationSeverity.Error,
                                        _cns);
                                    break;
                                case LoginAuthenticationParameters.USER_ALREADY_LOGGED:
                                    _notification.Invoke(
                                        _gbCard,
                                        GetString("ErrorUserAlreadyLogged"),
                                        NotificationPriority.Last,
                                        NotificationSeverity.Error,
                                        _cns);
                                    break;
                                default:
                                    _notification.Invoke(
                                       _gbCard,
                                       GetString("ErrorAuthenticationFailed"),
                                       NotificationPriority.Last,
                                       NotificationSeverity.Error,
                                       _cns);
                                    break;
                            }

                        }
                        else
                        {
                            _notification.Invoke(
                                       _gbCard,
                                       GetString("ErrorAuthenticationFailed"),
                                       NotificationPriority.Last,
                                       NotificationSeverity.Error,
                                       _cns);
                        }
                        SendCardReaderLoginSuccess(CardReaderSceneType.Rejected);
                        _ePassword.Text = String.Empty;
                        _ePIN.Text = String.Empty;
                        _lastCardSwiped = String.Empty;
                    }
                }
            }
        }

        private void CheckLoginPassword()
        {
            if (Validator.IsNullString(_eUserName.Text))
            {
                _notification.Error(NotificationPriority.Last,
                    _eUserName,
                    GetString("ErrorEntryUserName"),
                    _cns);

                return;
            }

            try
            {
                if (_authenticate)
                {
                    Contal.Cgp.BaseLib.LoginAuthenticationParameters loginAuthParam = new Contal.Cgp.BaseLib.LoginAuthenticationParameters(_eUserName.Text, _ePassword.Text, false);

                    Exception error = CgpClient.Singleton.MainServerProvider.ClientAuthenticate(loginAuthParam);
                    if (error != null)
                        throw (error);

                    Cgp.Server.Beans.Extern.EventlogParameter parameter = new Contal.Cgp.Server.Beans.Extern.EventlogParameter(Cgp.Server.Beans.Extern.EventlogParameter.TYPEUSERNAME, loginAuthParam.LoginUsername);
                    IList<Cgp.Server.Beans.Extern.EventlogParameter> parameterList = new List<Cgp.Server.Beans.Extern.EventlogParameter>();
                    parameterList.Add(parameter);
                    //CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEvent(Cgp.Server.Beans.Extern.Eventlog.TYPECLIENTLOGIN, this.GetType().Assembly.GetName().Name, null, "Client with USERNAME " + loginAuthParam.LoginUsername + " login", parameterList);
                    CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEventClientLogin(loginAuthParam.LoginUsername);
                }
                else
                {
                    Exception error;
                    if (!CgpClient.Singleton.MainServerProvider.Logins.CheckLoginPassword(_eUserName.Text,
                        PasswordHelper.GetPasswordhash(_eUserName.Text, _ePassword.Text), out error))
                    {
                        if (error != null)
                            throw error;
                        else
                            throw new Exception();
                    }
                }

                _isAuthenticated = true;
                DialogResult = DialogResult.OK;
                CgpClient.Singleton.PluginManager.PreRegisterAttachCallbackHandlers();
                Close();
            }
            catch (Exception exception)
            {
                if (exception is InvalidSecureEntityException)
                {
                    InvalidSecureEntityException error = (InvalidSecureEntityException)exception;

                    switch (error.Reason)
                    {
                        case LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD:
                            _notification.Error(NotificationPriority.Last,
                                _eUserName,
                                GetString("ErrorWrongUsernamePassword"),
                                _cns);

                            Cgp.Server.Beans.Extern.EventlogParameter parameter = new Contal.Cgp.Server.Beans.Extern.EventlogParameter(Cgp.Server.Beans.Extern.EventlogParameter.TYPEUSERNAME, _eUserName.Text);
                            IList<Cgp.Server.Beans.Extern.EventlogParameter> parameterList = new List<Cgp.Server.Beans.Extern.EventlogParameter>();
                            parameterList.Add(parameter);
                            CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEvent(Cgp.Server.Beans.Extern.Eventlog.TYPECLIENTLOGINWRONGPASSWORD, this.GetType().Assembly.GetName().Name, null, "Wrong password by login client with USERNAME " + _eUserName.Text, parameterList);

                            break;
                        case LoginAuthenticationParameters.LOGIN_DISABLED:

                            _notification.Invoke(
                                _eUserName,
                                GetString("ErrorLoginDisabled"),
                                NotificationPriority.Last,
                                NotificationSeverity.Error,
                                _cns);
                            break;
                        case LoginAuthenticationParameters.LOGIN_EXPIRED:
                            _notification.Invoke(
                                _eUserName,
                                GetString("ErrorLoginExpired"),
                                NotificationPriority.Last,
                                NotificationSeverity.Error,
                                _cns);
                            break;
                        case LoginAuthenticationParameters.USER_ALREADY_LOGGED:
                            _notification.Invoke(
                                _eUserName,
                                GetString("ErrorUserAlreadyLogged"),
                                NotificationPriority.Last,
                                NotificationSeverity.Error,
                                _cns);
                            break;
                        default:
                            _notification.Invoke(
                               _eUserName,
                               GetString("ErrorAuthenticationFailed"),
                               NotificationPriority.Last,
                               NotificationSeverity.Error,
                               _cns);
                            break;
                    }

                }
                else
                {
                    _notification.Invoke(
                               _authGroup,
                               GetString("ErrorAuthenticationFailed"),
                               NotificationPriority.Last,
                               NotificationSeverity.Error,
                               _cns);
                }
                //DialogResult = DialogResult.Abort;
                _ePassword.Text = String.Empty;
                _ePIN.Text = String.Empty;
                _lastCardSwiped = String.Empty;
            }
        }

        private void LoginPasswordForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_ePIN.Text == string.Empty)
                    CheckLoginPassword();
            }
        }

        private void LoginPasswordForm_Shown(object sender, EventArgs e)
        {
            this.Text = " ";
            _lLogin.Visible = true;
            _lLogin.Text = GetString("LoginPasswordFormLoginPasswordForm");
            _lInfo.Visible = false;
            if (_isSessionExpired)
            {
                SetTextSessionExpired();
            }
            if (_isApplicationLock)
            {
                SetTextLocked();
                _isApplicationLock = false;
            }
            /*try
            {
                Contal.IwQuick.Sys.Microsoft.DllUser32.SetForegroundWindow(this.Handle);
            }
            catch
            {
            }*/

            BeginInvoke(new Action(() =>
            {
                if (IsDisposed)
                    return;

                Activate();

                if (_eUserName.Text.Length > 0)
                {
                    ActiveControl = _ePassword;
                    _ePassword.Select();
                }
                else
                {
                    ActiveControl = _eUserName;
                    _eUserName.Select();
                }
            }));

            // after successful story from DBSCreator, to evaluate WinForms behaviour here
            // DllUser32.SetForegroundWindows wouldn't be necessary maybe
            try
            {
                this.TopMost = false;
                System.Threading.Thread.Sleep(100);
                this.TopMost = true;
            }
            catch
            {
            }
        }

        private void SetTextSessionExpired()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DVoid2Void(SetTextSessionExpired));
            }
            else
            {
                _lLogin.Visible = false;
                _lInfo.Visible = true;
                this._lInfo.Text = GetString("InfoExpiredSession");
            }
        }

        private void SetTextLocked()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DVoid2Void(SetTextLocked));
            }
            else
            {
                _lLogin.Visible = false;
                _lInfo.Visible = true;
                this._lInfo.Text = GetString("ApplicationLock");
            }
        }


        public void ResetTopMost()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(ResetTopMost));
            else
            {
                this.TopMost = false;
                this.TopMost = true;
            }
        }

        public void CardEnabled(bool onlineCardReader)
        {
            try
            {
                if (IsHandleCreated)
                    CardReaderPresent(onlineCardReader);
            }
            catch
            {
            }
        }

        private object _lockFormShowP = new object();
        private void CardReaderPresent(bool isPresent)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DBool2Void(CardReaderPresent), isPresent);
            }
            else
            {
                lock (_lockFormShowP)
                {
                    if (isPresent)
                    {
                        _lCRloginEnabled.Text = GetString("InfoLoginCREnabled");
                        _gbCard.Parent = this;
                        _bCancel.Location = new Point(_bCancel.Location.X, _gbCard.Location.Y + _gbCard.Height + 10);
                        _bOk.Location = new Point(_bOk.Location.X, _gbCard.Location.Y + _gbCard.Height + 10);
                    }
                    else
                    {
                        _lCRloginEnabled.Text = string.Empty;
                        _gbCard.Parent = null;
                        _bOk.Location = new Point(_bOk.Location.X, _authGroup.Location.Y + _authGroup.Height + 10);
                        _bCancel.Location = new Point(_bCancel.Location.X, _authGroup.Location.Y + _authGroup.Height + 10);
                    }

                    this.ClientSize = new Size(this.ClientRectangle.Width + WinFormsHelper.DpiScaleX(5), _bCancel.Location.Y + _bCancel.Height + 10);
                }
            }
        }

        private void ShowCardReaderLoginOption(bool pinRequired)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DBool2Void(ShowCardReaderLoginOption), pinRequired);
            }
            else
            {
                if (pinRequired)
                {
                    _lPIN.Visible = true;
                    _ePIN.Visible = true;
                    _gbCard.Height = _ePIN.Location.Y + _ePIN.Height + 10;
                }
                else
                {
                    _lPIN.Visible = false;
                    _ePIN.Visible = false;
                    _gbCard.Height = _lCRloginEnabled.Location.Y + _lCRloginEnabled.Height + 10;
                }

                if (_gbCard.Parent == null)
                {
                    _bOk.Location = new Point(_bOk.Location.X, _authGroup.Location.Y + _authGroup.Height + 10);
                    _bCancel.Location = new Point(_bCancel.Location.X, _authGroup.Location.Y + _authGroup.Height + 10);
                }
                else
                {
                    _bCancel.Location = new Point(_bCancel.Location.X, _gbCard.Location.Y + _gbCard.Height + 10);
                    _bOk.Location = new Point(_bOk.Location.X, _gbCard.Location.Y + _gbCard.Height + 10);
                }

                this.ClientSize = new Size(this.ClientRectangle.Width + WinFormsHelper.DpiScaleX(5), _bCancel.Location.Y + _bCancel.Height + 10);
            }
        }

        private void _ePIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
                e.Handled = true;

            _notification.Revoke(_ePIN);
        }

        private void _ePIN_TextChanged(object sender, EventArgs e)
        {
            string validText = QuickParser.GetValidDigitString(_ePIN.Text);
            if (_ePIN.Text != validText)
            {
                _ePIN.Text = validText;
                _ePIN.SelectionStart = _ePIN.TextLength;
                _ePIN.SelectionLength = 0;
            }
            if (_ePIN.Text.Length > 8)
            {
                _ePIN.Text = _ePIN.Text.Substring(0, 8);
                _ePIN.SelectionStart = _ePIN.Text.Length;
                _ePIN.SelectionLength = 0;
            }
        }

        private void _ePIN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TryLoginWithCard();
        }

        private string _lastCardSwiped = string.Empty;
        public void CardReaderCardSwiped(string card)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DString2Void(CardReaderCardSwiped), card);
            }
            else
            {
                _lastCardSwiped = card;
                byte pinlength = CgpClient.Singleton.MainServerProvider.GetCardPinLengthByFullCardNumber(_lastCardSwiped);
                if (pinlength == 0)
                {
                    _notification.Error(NotificationPriority.Last, _gbCard, GetString("ErrorUnknownWrongCard"), _cns);
                }
                else
                {
                    _notification.Info(NotificationPriority.Last, _ePIN, GetString("ErrorEntryPin"), _cns);
                    _ePIN.Focus();
                }
            }
        }

        public void ClientInfoWrongPIN()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ClientInfoWrongPIN));
            }
            else
            {
                _notification.Error(NotificationPriority.Last, _gbCard, GetString("ErrorInvalidPin"), _cns);
                _ePIN.Text = string.Empty;
                _lastCardSwiped = string.Empty;
                _eUserName.Focus();
            }
        }

        private void TryLoginWithCard()
        {
            if (_lastCardSwiped == string.Empty)
            {
                _notification.Error(NotificationPriority.Last, _gbCard, GetString("ErrorSwipedCardNull"), _cns);
                return;
            }

            if (CgpClient.Singleton.MainServerProvider != null)
            {
                if (CgpClient.Singleton.MainServerProvider.IsValidPinForCard(_lastCardSwiped, IwQuick.Crypto.QuickHashes.GetCRC32String(_ePIN.Text)))
                {
                    LoginWithCard(_lastCardSwiped);
                }
                else
                {
                    _notification.Error(NotificationPriority.Last, _gbCard, GetString("ErrorInvalidPin"), _cns);
                    _ePIN.Text = string.Empty;
                    _lastCardSwiped = string.Empty;
                    _eUserName.Focus();
                }
            }
        }

        private void SendCardReaderLoginSuccess(CardReaderSceneType success)
        {
            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                if (plugin._plugin.Description == "NCAS")
                {
                    plugin._plugin.SendCardReaderCommand(success);
                    return;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _notification.Revoke(_ePIN);
            base.OnFormClosing(e);
        }
    }
}
