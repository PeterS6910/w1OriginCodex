using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class SoundExplorer : CgpTranslateForm
    {
        public SoundExplorer()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            LoadFiles();
        }

        private void LoadFiles()
        {
            _lvFiles.Items.Clear();
            _lvFiles.View = View.Details;
            _lvFiles.HeaderStyle = ColumnHeaderStyle.None;
            string path = Application.StartupPath;
            path += @"\Sounds";

            ListViewItem lvt;

            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists)
            {
                FileInfo[] rgFiles = di.GetFiles("*.wav");
                foreach (FileInfo fi in rgFiles)
                {
                    lvt = new ListViewItem(fi.Name);
                    _lvFiles.Items.Add(lvt);
                }
                rgFiles = di.GetFiles("*.mp3");
                foreach (FileInfo fi in rgFiles)
                {
                    lvt = new ListViewItem(fi.Name);
                    _lvFiles.Items.Add(lvt);
                }
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_lvFiles.SelectedItems != null && _lvFiles.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = _lvFiles.SelectedItems[0];
                _file = selectedItem.Text;
            }
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _file = string.Empty;
            Close();
        }

        string _file = string.Empty;
        public void ShowDialog(out string file)
        {
            ShowDialog();
            file = _file;
        }

        private void _lvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (_lvFiles.SelectedItems != null && _lvFiles.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = _lvFiles.SelectedItems[0];
                _file = selectedItem.Text;
            }
            Close();
        }
    }
}
