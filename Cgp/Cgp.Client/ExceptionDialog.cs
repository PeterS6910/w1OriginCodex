using Contal.IwQuick.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public partial class ExceptionDialog : Form
    {
        const int DIALOG_HEIGHT = 330;
        const int DIALOG_OPEN   = 150;

        public ExceptionDialog()
        {
            InitializeComponent();
            _lException.Text = "Unhandled exception has occurred in your application. If you click" + Environment.NewLine;
            _lException.Text += "Continue, the application will ignore this error and attempt to continue. If" + Environment.NewLine;
            _lException.Text += "you click Quit, the application will close immediately";
            _errorPricure.Image = SystemIcons.Error.ToBitmap();

            // Scale dialog for high DPIs
            if(WinFormsHelper.GetCurrentDPI() != ScreenDpi.Dpi96)
            {
                this.Width = WinFormsHelper.DpiScaleY(this.Width);
                this.Height = WinFormsHelper.DpiScaleX(this.Height);
            }
        }

        private void DetailClick(object sender, EventArgs e)
        {
            // Scale dialog for high DPIs
            if (WinFormsHelper.GetCurrentDPI() != ScreenDpi.Dpi96)
                Height = Size.Height >= DIALOG_HEIGHT ? WinFormsHelper.DpiScaleX(DIALOG_OPEN) : WinFormsHelper.DpiScaleX(DIALOG_HEIGHT);
            else
                Height = Size.Height >= DIALOG_HEIGHT ? DIALOG_OPEN : DIALOG_HEIGHT;
        }

        DialogResult result = DialogResult.Ignore;
        public DialogResult ShowDialogExc(Exception error)
        {
            if (error != null)
            {
                _lExceptionType.Text = "Exception of type '" + error.GetType() + "' was thrown";
                _eExceptionText.Text = Environment.NewLine;
                _eExceptionText.Text += "************** Exception Text **************" + Environment.NewLine;
                _eExceptionText.Text += Environment.NewLine;
                _eExceptionText.Text += error.ToString();
                _eExceptionText.Text += error.StackTrace;
            }
            ShowDialog();
            return result;
        }

        private void ContinueClick(object sender, EventArgs e)
        {
            result = DialogResult.Ignore;
            Close();
        }

        private void QuitClick(object sender, EventArgs e)
        {
            result = DialogResult.Abort;
            Close();
        }
    }
}
