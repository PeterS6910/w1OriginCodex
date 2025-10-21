using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class ServerDirectory : CgpTranslateForm
    {
        private string _selectedDirectory = string.Empty;
        public ServerDirectory()
        {
            InitializeComponent();
            _twDirectory.MouseDown += new MouseEventHandler(TwDirectoryMouseDown);
            ListDir();
        }

        void TwDirectoryMouseDown(object sender, MouseEventArgs e)
        {
            TreeViewHitTestInfo hitInfo = _twDirectory.HitTest(e.Location);
            TreeNode tn = hitInfo.Node;
            if (tn != null)
            {
                _twDirectory.SelectedNode = tn;
                _selectedDirectory = tn.Tag.ToString();
                ListDir();
                if (hitInfo.Location != TreeViewHitTestLocations.PlusMinus)
                {
                    if (tn.IsExpanded)
                        tn.Collapse();
                    else
                        tn.Expand();
                }
            }
        }

        public void ShowDialog(out string outSelectedDirectory)
        {
            ShowDialog();
            outSelectedDirectory = _selectedDirectory;
        }

        private void ListDir()
        {
            IList<string> array1 = CgpClient.Singleton.MainServerProvider.GetServerDirectory(_selectedDirectory);
            foreach (string str in array1)
            {
                CreateTreeNode(str);
            }
        }

        private void CreateTreeNode(string text)
        {
            TreeNode node = new TreeNode();
            string pathl = text.Substring(text.LastIndexOf(@"\") + 1);
            if (string.IsNullOrEmpty(pathl))
            {
                node.Text = text;
                node.Tag = text;
                node.Name = text.Substring(0, text.LastIndexOf(@"\"));
            }
            else
            {
                node.Text = pathl;
                node.Tag = text;
                node.Name = text;
            }

            TreeNode[] arr = _twDirectory.Nodes.Find(node.Name, true);
            if (arr != null && arr.Length > 0) return;

            string parentName = text.Substring(0, text.LastIndexOf(@"\"));
            arr = _twDirectory.Nodes.Find(parentName, true);
            if (arr == null || arr.Length == 0)
            {
                _twDirectory.Nodes.Add(node);
            }
            else
            {
                TreeNode nodef = arr[0];
                nodef.Nodes.Add(node);
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_twDirectory.SelectedNode != null)
            {
                _selectedDirectory = _twDirectory.SelectedNode.Tag.ToString();
            }
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _selectedDirectory = string.Empty;
            Close();
        }
    }
}
