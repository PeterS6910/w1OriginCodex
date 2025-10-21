using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;

namespace Contal.Cgp.Client
{
    public partial class PluginDashboardForm :
#if DESIGNER
        Form
#else
        ACgpFullscreenForm
#endif
    {
        private PluginDashboardForm()
        {
            InitializeComponent();
        }

        private static volatile PluginDashboardForm _singleton = null;
        private static object _syncRoot = new object();

        public static PluginDashboardForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new PluginDashboardForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        private void PluginDashboardForm_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private ImageList _lvImageList = new ImageList();

        protected internal void PluginsReport(ICgpClientPlugin[] plugins)
        {
            _pluginsLV.Items.Clear();

            ListViewItem lvi = null;

            if (null != plugins)
                foreach (ICgpClientPlugin p in plugins)
                {
                    try
                    {
                        ICgpVisualPlugin vp = (ICgpVisualPlugin)p;

                        lvi = new ListViewItem(p.FriendlyName);
                        lvi.Tag = vp.MainForm;

                        string cpd = vp.CgpDesignation;

                        // icon
                        if (!_lvImageList.Images.ContainsKey(cpd)) {

                            if (null != vp.MainForm)
                            {
                                _lvImageList.Images.Add(cpd, vp.MainForm.Icon.ToBitmap());
                            }
                        }

                        if (_pluginsLV.LargeImageList == null)
                            _pluginsLV.LargeImageList = _lvImageList;

                        lvi.ImageKey = cpd;
                        
                        _pluginsLV.Items.Add(lvi);

                        
                    }
                    catch
                    {
                    }
                }
        }

        private void _pluginsLV_DoubleClick(object sender, EventArgs e)
        {
            if (_pluginsLV.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in _pluginsLV.SelectedItems)
                {
                    if (lvi.Tag != null && lvi.Tag is Form)
                    {
                        Form f = (Form)lvi.Tag;

                        if (!f.Visible)
                            f.Show();
                        else
                            f.BringToFront();
                    }
                }
            }
        }

        protected override bool VerifySources()
        {
            return true;
        }
    }
}
