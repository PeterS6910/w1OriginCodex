using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Contal.Cgp.Client;

namespace Contal.Cgp.NCAS.Client
{
    /// <summary>
    /// abstract parent class for all fullschreen MDI child forms to be shown under the CGP client
    /// </summary>
    //public class ACgpFullscreenForm : Form
    public abstract class ANcasCgpFullscreenForm : Contal.Cgp.BaseLib.PluginMainForm
    {
        public ANcasCgpFullscreenForm()
            : base(true)
        {
            this.MdiParent = CgpClientMainForm.Singleton;
            this.FormBorderStyle = FormBorderStyle.None;
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ControlBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormOnEnter += new Contal.IwQuick.DFromTToVoid<Form>(Form_Enter);
            this.FormOnLeave += new Contal.IwQuick.DFromVoidToVoid(Form_Leave);
        }

        //protected void RegisterToMain()
        //{
        //    CgpClientMainForm.Singleton.RegisterChildForm(this);
        //}

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            _isRunedForm_Enter = false;
            e.Cancel = true;
            Hide();
        }

        protected abstract bool VerifySources();

        private bool _isRunedForm_Enter = false;
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
            CgpClientMainForm.Singleton.StopProgress();
            CgpClientMainForm.Singleton.ModifyOpenWindowImage(this);
        }

        public new void Show()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new IwQuick.DFromVoidToVoid(Show));
            }
            else
            {
                if (!VerifySources())
                    return;

                /// hack to let the child window to resize to proper size of the MDI parent container
                base.Show();
                this.Left = 0;
                this.Top = 0;
                this.WindowState = FormWindowState.Maximized;
                int width = this.Width;
                int height = this.Height;

                this.WindowState = FormWindowState.Normal;
                this.Width = width;
                this.Height = height;

                this.BringToFront();
            }
        }

        public void ChangeSize(int deltaWidth, int deltaHeight)
        {

            this.Width += deltaWidth;
            this.Height += deltaHeight;
        }
    }
}
