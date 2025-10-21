using System;
using System.Windows.Forms;

namespace AnalyticTool.Advitech
{
    public partial class InsertLoginAndPasswordForm : Form
    {
        public string CustomUserName { get; private set; }
        public string CustomUserPassword { get; private set; }

        public InsertLoginAndPasswordForm()
        {
            InitializeComponent();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(
                    Contal.IwQuick.UI.NotificationPriority.JustOne,
                    _eName,
                    "Login is not selected",
                    Contal.IwQuick.UI.ControlNotificationSettings.Default);

                return;
            }

            if (string.IsNullOrEmpty(_ePassword.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(
                    Contal.IwQuick.UI.NotificationPriority.JustOne,
                    _ePassword,
                    "Password is not selected",
                    Contal.IwQuick.UI.ControlNotificationSettings.Default);

                return;
            }

            CustomUserName = _eName.Text;
            CustomUserPassword = _ePassword.Text;

            DialogResult = DialogResult.OK;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
