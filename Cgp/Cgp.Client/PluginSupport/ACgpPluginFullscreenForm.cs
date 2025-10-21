using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public abstract class ACgpPluginFullscreenForm<TCgpVisualPlugin> : PluginMainForm<TCgpVisualPlugin> 
        where TCgpVisualPlugin : ICgpVisualPlugin
    {
        private readonly TCgpVisualPlugin _plugin;
        private readonly ICgpClientMainForm _cgpClientMainForm;

        public override TCgpVisualPlugin Plugin
        {
            get { return _plugin; }
        }

        protected ACgpPluginFullscreenForm(
            ICgpClientMainForm cgpClientMainForm,
            TCgpVisualPlugin plugin,
            LocalizationHelper localizationHelper)
            : base(
                cgpClientMainForm,
                true,
                localizationHelper)
        {
            _plugin = plugin;
            _cgpClientMainForm = cgpClientMainForm;

            MdiParent = (Form)_cgpClientMainForm;
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Dock = DockStyle.Fill;
            FormOnEnter += Form_Enter;
            FormOnLeave += Form_Leave;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);

            _cgpClientMainForm.RemoveFromOpenWindows(this);
            _isRunedForm_Enter = false;
            e.Cancel = true;
            Hide();
        }

        private bool _isRunedForm_Enter;

        private void Form_Enter(Form form)
        {
            if (!_isRunedForm_Enter)
            {
                _cgpClientMainForm.AddToOpenWindows(this);
                _isRunedForm_Enter = true;
            }

            _cgpClientMainForm.SetActOpenWindow(this);
        }

        private void Form_Leave()
        {
            //_cgpClientMainForm.StopProgress(this);
        }

        public new void Show()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(Show));
            }
            else
            {
                base.Show();
                Focus();
            }
        }
    }
}
