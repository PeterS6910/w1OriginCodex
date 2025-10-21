using System;
using System.Drawing;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASClientForm :
#if DESIGNER
        Form
#else
        PluginMainForm<NCASClient>
#endif
    {
        private ICgpClientMainForm _cgpClientMainForm;
        internal ImageList _imagesSmall;

        public override NCASClient Plugin
        {
            get { return NCASClient.Singleton; }
        }

        public NCASClientForm()
            :base (
            CgpClientMainForm.Singleton,
            true,
            NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            var baseFont = _lvNcasPlugins.Font;
            _lvNcasPlugins.Font = new Font(baseFont.FontFamily, baseFont.Size + 1.6f, baseFont.Style, baseFont.Unit);

            _cgpClientMainForm = CgpClientMainForm.Singleton;      
            MdiParent = (Form)_cgpClientMainForm;
            LocalizationHelper.KryptonColorizationColor = _cgpClientMainForm.GetLocalizationHelper().KryptonColorizationColor;
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Dock = DockStyle.Fill;
            FormOnEnter += Form_Enter;
            _lvNcasPlugins.DoubleClick += _lvNcasPlugins_DoubleClick;
            //this.FormOnLeave += new Contal.IwQuick.DVoid2Void(Form_Leave);
            NCASClient.Singleton.RestartCardReaderCommunication();
        }

        private void Form_Enter(Form form)
        {
            InitPlugins();
        }

        private void InitPlugins()
        {
            _lvNcasPlugins.Items.Clear();
            NCASClient cc = Plugin;
            if (cc == null) return;

            _imagesSmall = new ImageList();
            _imagesSmall.ImageSize = new Size(32, 32);
            int i = 0;
            foreach (IPluginMainForm avp in Plugin.SubForms)
            {
                _imagesSmall.Images.Add(avp.Icon);
                ListViewItem lv = new ListViewItem(LocalizationHelper.GetString(avp.Name + avp.Name), i);
                lv.Name = avp.Name;
                _lvNcasPlugins.Items.Add(lv);
                i++;
            }
            _lvNcasPlugins.LargeImageList = _imagesSmall;
        }

        void _lvNcasPlugins_DoubleClick(object sender, EventArgs e)
        {
            OpenTableForm(_lvNcasPlugins.FocusedItem.Name);
        }

        private void OpenTableForm(string name)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            foreach (IPluginMainForm avp in Plugin.SubForms)
            {
                if (name == avp.Name)
                {
                    avp.Show();
                    return;
                }
            }
        }
    }
}
