using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

using Contal.IwQuick.UI;
using Contal.IwQuick.Localization;
using Contal.IwQuick;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server
{
    public enum AccountType : byte
    {
        LocalSystem,
        NetworkService,
        CustomAccount,
        NovaServiceUser
    }

    public partial class SelectServiceUserForm : TranslateForm
    {
        public AccountType AccountType { get; private set; }
        public ServiceStartMode ServiceStartupType { get; private set; }
        public string AccountName { get; private set; }
        public string AccountDomain { get; private set; }
        public string AccountPassword { get; private set; }

        public SelectServiceUserForm()
        {
            ServiceStartupType = ServiceStartMode.Automatic;
            AccountType = AccountType.NovaServiceUser;
            AccountName = string.Empty;
            AccountDomain = string.Empty;
            AccountPassword = string.Empty;

            InitializeComponent();
        }

        private void _rbCustomAcount_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbCustomUser.Checked)
            {
                _eAccountName.Enabled = true;
                _eAccountDomain.Enabled = true;
                _eAccountPassword.Enabled = true;
            }
            else
            {
                _eAccountName.Enabled = false;
                _eAccountName.Text = string.Empty;
                _eAccountDomain.Enabled = false;
                _eAccountDomain.Text = string.Empty;
                _eAccountPassword.Enabled = false;
                _eAccountPassword.Text = string.Empty;
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_rbCustomUser.Checked)
            {
                AccountType = AccountType.CustomAccount;

                AccountName = _eAccountName.Text;
                AccountDomain = _eAccountDomain.Text;
                AccountPassword = _eAccountPassword.Text;

                if (AccountName == string.Empty)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, 
                        _eAccountName, 
                        "Enter user name", 
                        ControlNotificationSettings.Default);

                    return;
                }

                try
                {
                    PrincipalContext pc =
                        AccountDomain != string.Empty 
                            ? new PrincipalContext(ContextType.Domain, AccountDomain) 
                            : new PrincipalContext(ContextType.Machine);

                    if (!pc.ValidateCredentials(AccountName, AccountPassword))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eAccountName,
                            "Wrong user name or password",
                            ControlNotificationSettings.Default);

                        return;
                    }

                    UserPrincipal up = 
                        UserPrincipal.FindByIdentity(
                            pc, 
                            IdentityType.SamAccountName, 
                            AccountName);

                    if (!up.GetAuthorizationGroups().Any(p => p.Sid == new SecurityIdentifier(CgpServerGlobals.SID_ADMINISTRATORS)))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne, 
                            _eAccountName, 
                            "User does not belong to administrator group", 
                            ControlNotificationSettings.Default);

                        return;
                    }
                }
                catch
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, 
                        _eAccountName, 
                        "User verification failed",
                        ControlNotificationSettings.Default);

                    return;
                }
            }
            else if (_rbNovaServiceUser.Checked)
            {
                AccountType = AccountType.NovaServiceUser;
            }
            else
            {
                DialogResult = DialogResult.Ignore;
                Close();
                return;
            }

            switch (_cbServiceStartupType.SelectedIndex)
            {
                case 0:
                    ServiceStartupType = ServiceStartMode.Automatic;
                    break;

                case 1:
                    ServiceStartupType = ServiceStartMode.Manual;
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (DialogResult == DialogResult.OK)
                return;

            if (!Dialog.Question("Are you sure that you want to cancel installation"))
                e.Cancel = true;
        }

        private void _eAccountName_Leave(object sender, EventArgs e)
        {
            if (_eAccountName.Text.Length <= 0)
                return;

            if (_eAccountDomain.Text.Length != 0)
                return;

            string[] parts = _eAccountName.Text.Split(StringConstants.BACKSLASH[0]);

            if (parts.Length != 2)
                return;

            _eAccountName.Text = parts[1];
            _eAccountDomain.Text = parts[0];
        }

        private void SelectServiceUserForm_Load(object sender, EventArgs e)
        {
            _cbServiceStartupType.SelectedIndex = 0; // stands for Automatic
        }

        private void SelectServiceUserForm_Shown(object sender, EventArgs e)
        {
            TopMost = false;
            System.Threading.Thread.Sleep(100);
            TopMost = true;
        }
    }
}
