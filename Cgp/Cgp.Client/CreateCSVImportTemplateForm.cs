using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class CreateCSVImportTemplateForm :
#if DESIGNER
        Form
#else
        CgpTranslateForm
#endif
    {
        private string _csvImportSchemaName = string.Empty;

        public CreateCSVImportTemplateForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
        }

        public string GetCSVImportSchemaName { get { return _csvImportSchemaName; } }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_eName.Text != string.Empty)
            {
                _csvImportSchemaName = _eName.Text;
                Close();
            }
            else
            {
                ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorEntryName"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _eName.Focus();
            }
        }

        private void CreateCSVImportTemplateForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                _bOk_Click(sender, null);

            if (e.KeyCode == Keys.Escape)
                _bCancel_Click(sender, null);
        }
    }
}
