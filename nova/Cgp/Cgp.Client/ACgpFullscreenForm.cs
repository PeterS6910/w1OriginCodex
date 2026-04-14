using System.Windows.Forms;

using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    /// <summary>
    /// abstract parent class for all fullschreen MDI child forms to be shown under the CGP client
    /// </summary>
    //public class ACgpFullscreenForm : Form
    public abstract class ACgpFullscreenForm : MdiChildForm
    {
        protected ACgpFullscreenForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            MdiParent = CgpClientMainForm.Singleton;
            FormBorderStyle = FormBorderStyle.None;
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            ControlBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            FormOnEnter += Form_Enter;
            FormOnLeave += Form_Leave;
            Dock = DockStyle.Fill;
        }

        protected void RegisterToMain()
        {
            CgpClientMainForm.Singleton.RegisterChildForm(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);

            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            _isRunedForm_Enter = false;
            e.Cancel = true;
            Hide();
        }

        protected abstract bool VerifySources();

        private bool _isRunedForm_Enter;
        private void Form_Enter(Form form)
        {
            if (!_isRunedForm_Enter)
            {
                CgpClientMainForm.Singleton.AddToOpenWindows(this);
                _isRunedForm_Enter = true;
            }

            CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        private void Form_Leave()
        {
            //CgpClientMainForm.Singleton.StopProgress(this);
        }

        public new void Show()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(Show));
            }
            else
            {
                if (VerifySources())
                {
                    base.Show();
                    Focus();
                }
            }
        }
    }
}
