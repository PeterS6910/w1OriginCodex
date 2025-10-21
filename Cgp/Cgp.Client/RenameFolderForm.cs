using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class RenameFolderForm : CgpTranslateForm
    {
        private bool _isOk = false;

        public RenameFolderForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            this.Height = (this.Height - this.ClientSize.Height) + _bOk.Location.Y + _bOk.Height + 50;
        }

        public DialogResult ShowDialog(ref string name)
        {
            _eOldName.Text = name;
            ShowDialog();
            name = _eNewName.Text;

            if (_isOk)
                return DialogResult.OK;
            else
                return DialogResult.Cancel;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_eNewName.Text == String.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNewName,
                    GetString("ErrorInsertFolderName"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
            }
            else if (_eNewName.Text == _eOldName.Text)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNewName,
                    GetString("ErrorTheSameFolderName"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
            }
            else
            {
                _isOk = true;
                Close();
            }
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _isOk = false;
            Close();
        }

        private void RenameFolderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _bCancel_Click(this, null);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                _bOk_Click(this, null);
            }
        }
    }
}
