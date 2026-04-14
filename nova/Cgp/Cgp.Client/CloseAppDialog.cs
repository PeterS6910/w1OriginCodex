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
    public partial class CloseAppDialog : CgpTranslateForm
    {
        public CloseAppDialog()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();

            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = false;
        }

        public byte MyShowDialog(bool isLoged)
        {
            if (!isLoged)
            {
                _bLogout.Visible = false;
                _lQuestionCloseApp.Text = GetString("QuestionCloseAppShort");
                ChangeSize();
            }
            else
            {
                _bLogout.Visible = true;            
                _lQuestionCloseApp.Text = GetString("CloseDialog_lQuestionCloseApp");
                ChangeSize();
            }
            this.ShowDialog();
            return _result;
        }

        private void ChangeSize()
        {
            this.ClientSize = new Size(_lQuestionCloseApp.Left + (int)this.CreateGraphics().MeasureString(_lQuestionCloseApp.Text, _lQuestionCloseApp.Font).Width + WinFormsHelper.DpiScaleY(10), this.ClientSize.Height);            
        }

        byte _result = 0;
        public byte ResultDialog { get { return _result; } }

        private void _bLogout_Click(object sender, EventArgs e)
        {
            _result = 1;
            Close();
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            _result = 2;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _result = 0;
            Close();
        }   
    }
}
