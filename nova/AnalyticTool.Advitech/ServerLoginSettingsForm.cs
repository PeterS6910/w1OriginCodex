using System;
using System.Windows.Forms;
using Contal.IwQuick.Threads;

namespace AnalyticTool.Advitech
{
    public partial class ServerLoginSettingsForm : Form, IDialog
    {
        private Form _resultDialog;

        public ServerLoginSettingsForm()
        {
            InitializeComponent();

            LoadSettings();
        }

        private void LoadSettings()
        {
            _tbServerIp.Text = string.IsNullOrEmpty(ApplicationProperties.Singleton.ServerIp)
                ? "127.0.0.1"
                : ApplicationProperties.Singleton.ServerIp;

            _ePort.Value = ApplicationProperties.Singleton.ServerPort == 0
                ? 54001
                : ApplicationProperties.Singleton.ServerPort;

            _tbUserName.Text = ApplicationProperties.Singleton.ServerUserName;
            _tbPassword.Text = ApplicationProperties.Singleton.ServerPassword;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            SetValues();
            Cursor = Cursors.WaitCursor;

            foreach (Control control in Controls)
                control.Enabled = false;

            SafeThread.StartThread(ConnectToServer);
        }

        private void ConnectToServer()
        {
            bool result = NovaServerHelper.Singleton.Init();

            BeginInvoke(
                new Action(() =>
                {
                    foreach (Control control in Controls)
                        control.Enabled = true;

                    Cursor = Cursors.Default;

                    if (!result)
                    {
                        Contal.IwQuick.UI.Dialog.Error("Server not found");
                        return;
                    }

                    _resultDialog = new DatabaseServerSettingsForm();
                    DialogResult = DialogResult.OK;
                }));
        }

        private void SetValues()
        {
            ApplicationProperties.Singleton.ServerIp = _tbServerIp.Text;
            ApplicationProperties.Singleton.ServerPort = (int)_ePort.Value;
            ApplicationProperties.Singleton.ServerUserName = _tbUserName.Text;
            ApplicationProperties.Singleton.ServerPassword = _tbPassword.Text;
        }

        private void _tbServerIp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }

            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '.' || e.KeyChar == ':')
            {
                e.Handled = false;
                return;
            }

            if (e.KeyChar < 'A' || e.KeyChar > 'F')
            {
                if (e.KeyChar < 'a' || e.KeyChar > 'f')
                {
                    e.Handled = true;
                }
            }
        }

        #region IDialog Members

        public Form ResultDialog
        {
            get { return _resultDialog; }
        }

        #endregion
    }
}
