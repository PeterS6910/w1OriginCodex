using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public partial class AboutForm : CgpTranslateForm
    {
        private MdiClient _mdiClient;
        private int _borderWidth = 0;
        private int _borderHeight = 0;
        private bool _isVisible = false;

        public AboutForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            this.MdiParent = CgpClientMainForm.Singleton;
            _borderWidth = this.Width - this.ClientSize.Width;
            _borderHeight = this.Height - this.ClientSize.Height;
            _mdiClient = GetMdiClientWindow();
            this.Move += new EventHandler(AboutForm_Move);
            //this.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(FormKeyDown);
        }

        // ctor specially for PleaseWaitMonitor
        public AboutForm(LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();

            _borderWidth = this.Width - this.ClientSize.Width;
            _borderHeight = this.Height - this.ClientSize.Height;

            this.Move += new EventHandler(AboutForm_Move);

            _bOk.Focus();
            _bOk.Visible = false;
            _pbLoading.Visible = true;
        }

        public bool IsVisible { get { return _isVisible; } }

        //void FormKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Escape)
        //    {
        //        this.Hide();
        //    }
        //}

        void AboutForm_Move(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                if ((this.Left - _borderWidth <= 0) //left
                    || (this.Right + _borderWidth >= GetMdiAreaSize(_mdiClient, -4, -4).Width) //right
                    || (this.Top - _borderHeight <= 0)//top
                    || (this.Bottom + _borderHeight >= ((GetMdiAreaSize(_mdiClient, -4, -4).Height)))) //bottom
                {
                    Parent.Height = Parent.Height + 1;
                }
            }
        }
        public Size GetMdiAreaSize(MdiClient mdiClient, int chagerX, int changerY)
        {
            Size size = mdiClient.Size;
            size = new Size(size.Width + chagerX, size.Height + changerY);
            return size;
        }

        public MdiClient GetMdiClientWindow()
        {
            if (MdiParent != null)
            {
                foreach (Control ctl in MdiParent.Controls)
                {
                    if (ctl is MdiClient) return ctl as MdiClient;
                }
            }
            return null;
        }

        private static volatile AboutForm _singleton = null;
        private static object _syncRoot = new object();

        public static AboutForm Singleton
        {
            get
            {
                if (null == _singleton)
                {
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new AboutForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                }
                return _singleton;
            }
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            try
            {
                Contal.IwQuick.Sys.Microsoft.DllUser32.SetForegroundWindow(this.Handle);
                _isVisible = true;
            }
            catch
            {
            }
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            _isVisible = false;
        }

        private void SetVersions()
        {
            _lvVersions.Clear();
            _lvVersions.Columns.Add(GetString("Type"), 60);
            _lvVersions.Columns.Add(GetString("Version"), _lvVersions.Width - 80);
            ListViewItem lviClient = new ListViewItem(new string[2] { GetString("ClientVersion"), "v. " + CgpClient.Singleton.Version });
            ListViewItem lviServer;
            if (CgpClient.Singleton.MainServerProvider != null)
            {
                try
                {
                    lviServer = new ListViewItem(new string[2] { GetString("ServerVersion"), "v. " 
                        + CgpClient.Singleton.MainServerProvider.Version.ToString() });
                }
                catch(Exception error)
                {
                    lviServer = new ListViewItem(new string[2] { GetString("ServerVersion"), GetString("UnknownVersion") });
                    HandledExceptionAdapter.Examine(error);
                }
            }
            else
            {
                lviServer = new ListViewItem(new string[2] { GetString("ServerVersion"), GetString("UnknownVersion") });
            }
            _lvVersions.Items.Add(lviClient);
            _lvVersions.Items.Add(lviServer);
        }

        private void AboutForm_Activated(object sender, EventArgs e)
        {
            SetVersions();
            _isVisible = true;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            this.Hide();
            _isVisible = false;
        }

        public void HideAbout()
        {
            _isVisible = false;
            this.Hide();
        }

        
    }
}
