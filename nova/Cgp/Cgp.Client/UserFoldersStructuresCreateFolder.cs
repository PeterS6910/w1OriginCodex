using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.Client
{
    public partial class UserFoldersStructuresCreateFolder : CgpTranslateForm
    {
        private TreeNode _mainNode = null;
        private const string MAIN_NODE_ICON = "MAIN_NODE_ICON";
        private const string COLLAPS_NODE_ICON = "COLLAPS_NODE_ICON";
        private const string EXPAND_NODE_ICON = "EXPAND_NODE_ICON";
        private ImageList _imageList = new ImageList();
        private UserFoldersStructureItem _userFoldersStructureItem = null;
        private Dictionary<UserFoldersStructureDictionaryItem, TreeNode> _foldersTreeNodes = new Dictionary<UserFoldersStructureDictionaryItem, TreeNode>();
        private UserFoldersStructureDictionaryItemObject _selectedItem = null;
        private List<UserFoldersStructureDictionaryItem> _userFoldersStructureToEditEnd = new List<UserFoldersStructureDictionaryItem>();

        public UserFoldersStructuresCreateFolder()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.Images.Add(MAIN_NODE_ICON, ResourceGlobal.UserFoldersStructure);
            _imageList.Images.Add(COLLAPS_NODE_ICON, ResourceGlobal.Folder_collaps);
            _imageList.Images.Add(EXPAND_NODE_ICON, ResourceGlobal.Folder_expand);
            _userTreeView.ImageList = _imageList;
            FillTreeView();
        }

        private void FillTreeView()
        {
            _foldersTreeNodes.Clear();
            _userFoldersStructureItem = new UserFoldersStructureItem(null, false);
            ReadData(null, _userFoldersStructureItem);

            _mainNode = new TreeNode(GetString("UserFoldersStructuresForm_RootFolderName"));
            SetNodeIcon(_mainNode, MAIN_NODE_ICON);
            _userTreeView.Nodes.Add(_mainNode);
            _userTreeView.SelectedNode = _userTreeView.Nodes[0];

            AddToTreeView(_userFoldersStructureItem, _mainNode);
            _userFoldersStructureItem = null;
            _mainNode.Expand();

            SelectFolder();
        }

        private void SelectFolder()
        {
            if (_selectedItem != null)
            {
                if (_foldersTreeNodes.ContainsKey(_selectedItem.UserFoldersStructureDictionaryItem))
                {
                    _userTreeView.SelectedNode = _foldersTreeNodes[_selectedItem.UserFoldersStructureDictionaryItem];
                }
                else
                {
                    List<UserFoldersStructureDictionaryItem> list = new List<UserFoldersStructureDictionaryItem>();
                    list.Add(_selectedItem.UserFoldersStructureDictionaryItem);

                    UserFoldersStructure userFoldersStructure = _selectedItem.UserFoldersStructureDictionaryItem.UserFoldersStructure;
                    TreeNode node = null;
                    while (userFoldersStructure != null && userFoldersStructure.ParentFolder != null)
                    {
                        userFoldersStructure = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectById(userFoldersStructure.ParentFolder.IdUserFoldersStructure);

                        if (userFoldersStructure != null)
                        {
                            UserFoldersStructureDictionaryItem actUserFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(userFoldersStructure);
                            if (!_foldersTreeNodes.ContainsKey(actUserFoldersStructureDictionaryItem))
                            {
                                list.Insert(0, actUserFoldersStructureDictionaryItem);
                            }
                            else
                            {
                                userFoldersStructure = null;
                                node = _foldersTreeNodes[actUserFoldersStructureDictionaryItem];
                            }
                        }
                    }

                    if (list != null && list.Count > 0)
                    {
                        if (node == null)
                            node = _mainNode;

                        foreach (UserFoldersStructureDictionaryItem actUserFoldersStructureDictionaryItem in list)
                        {
                            if (node != null)
                            {
                                TreeViewCancelEventArgs tvcEventArgs = new TreeViewCancelEventArgs(node, false, TreeViewAction.Expand);
                                _userTreeView_BeforeExpand(_userTreeView, tvcEventArgs);

                                if (_foldersTreeNodes.ContainsKey(actUserFoldersStructureDictionaryItem))
                                {
                                    node = _foldersTreeNodes[actUserFoldersStructureDictionaryItem];
                                }
                                else
                                    node = null;
                            }
                        }
                    }

                    if (node != null)
                    {
                        _userTreeView.SelectedNode = node;
                    }
                }

                if (_selectedItem.OrmObjectId != string.Empty && _userTreeView.SelectedNode != null && _userTreeView.SelectedNode.Nodes != null && _userTreeView.SelectedNode.Nodes.Count > 0)
                {
                    TreeViewCancelEventArgs tvcEventArgs = new TreeViewCancelEventArgs(_userTreeView.SelectedNode, false, TreeViewAction.Expand);
                    _userTreeView_BeforeExpand(_userTreeView, tvcEventArgs);
                    foreach (TreeNode actNode in _userTreeView.SelectedNode.Nodes)
                    {
                        UserFoldersStructureObject userFoldersStructureObject = actNode.Tag as UserFoldersStructureObject;
                        if (userFoldersStructureObject != null && userFoldersStructureObject.ObjectId == _selectedItem.OrmObjectId &&
                            userFoldersStructureObject.ObjectType == _selectedItem.OrmObjectType)
                        {
                            _userTreeView.SelectedNode = actNode;
                        }
                    }
                }
            }
        }


        private void ReadData(UserFoldersStructure userFolderStructure, UserFoldersStructureItem userFoldersStructureItem)
        {
            ICollection<UserFoldersStructure> list = ReadDataFromDatabase(userFolderStructure);
            if (list != null && list.Count > 0)
            {
                foreach (UserFoldersStructure actUserFolderStructure in list)
                {
                    UserFoldersStructureItem newUserFoldersStructureItem = new UserFoldersStructureItem(actUserFolderStructure,
                        CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasSubFolders(actUserFolderStructure));
                    userFoldersStructureItem.Nodes.Add(newUserFoldersStructureItem);
                }
            }
        }

        private ICollection<UserFoldersStructure> ReadDataFromDatabase(UserFoldersStructure userFolderStructure)
        {
            Exception error;

            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            FilterSettings filterSetting = new FilterSettings(UserFoldersStructure.COLUMNPARENTFOLDER, userFolderStructure, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);

            ICollection<UserFoldersStructure> list = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.SelectByCriteria(filterSettings, out error);

            if (error != null)
                throw (error);

            if (list != null)
                list = list.OrderBy(ufs => ufs.ToString()).ToList();

            return list;
        }

        private void AddToTreeView(UserFoldersStructureItem userFoldersStructureItem, TreeNode node)
        {
            if (userFoldersStructureItem != null && userFoldersStructureItem.Nodes != null &&
                userFoldersStructureItem.Nodes.Count > 0)
            {
                foreach (UserFoldersStructureItem actUserFoldersStructureItem in userFoldersStructureItem.Nodes)
                {
                    if (actUserFoldersStructureItem.UserFoldersStructure != null)
                    {
                        TreeNode actNode = new TreeNode(actUserFoldersStructureItem.UserFoldersStructure.ToString());
                        TreeViewNodesSettings treeViewNodesSetting = new TreeViewNodesSettings(actUserFoldersStructureItem.UserFoldersStructure);
                        UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(treeViewNodesSetting.UserFoldersStructure);
                        if (!_userFoldersStructureToEditEnd.Contains(userFoldersStructureDictionaryItem))
                            _userFoldersStructureToEditEnd.Add(userFoldersStructureDictionaryItem);
                        actNode.Tag = treeViewNodesSetting;

                        userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(actUserFoldersStructureItem.UserFoldersStructure);
                        if (!_foldersTreeNodes.ContainsKey(userFoldersStructureDictionaryItem))
                        {
                            _foldersTreeNodes.Add(userFoldersStructureDictionaryItem, actNode);
                        }

                        node.Nodes.Add(actNode);

                        if (actUserFoldersStructureItem.HasSubFolders)
                        {
                            TreeNode tmpNode = new TreeNode();
                            actNode.Nodes.Add(tmpNode);
                            SetNodeIcon(actNode, COLLAPS_NODE_ICON);
                        }
                        else
                        {
                            SetNodeIcon(actNode, EXPAND_NODE_ICON);
                        }
                    }
                }
            }
        }

        private void SetNodeIcon(TreeNode node, string key)
        {
            node.ImageKey = key;
            node.SelectedImageKey = key;
            node.StateImageKey = key;
        }

        private void _userTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null || e.Node == _mainNode)
                return;

            if (e.Node.Nodes != null && e.Node.Nodes.Count == 1 &&
                e.Node.Nodes[0].Tag == null)
            {
                TreeViewNodesSettings treeViewNodesSetting = e.Node.Tag as TreeViewNodesSettings;

                if (treeViewNodesSetting != null)
                {
                    UserFoldersStructureItem userFoldersStructureItem = new UserFoldersStructureItem(treeViewNodesSetting.UserFoldersStructure,
                        false);
                    ReadData(treeViewNodesSetting.UserFoldersStructure, userFoldersStructureItem);
                    e.Node.Nodes.Clear();
                    AddToTreeView(userFoldersStructureItem, e.Node);
                }
            }
        }


        private void CreateUserFolderRoot()
        {
            try
            {
                UserFoldersStructure ufs = new UserFoldersStructure();
                if (_selectedItem != null)
                {
                    ufs.ParentFolder = _selectedItem.UserFoldersStructureDictionaryItem.UserFoldersStructure;
                }
                ufs.FolderName = _eName.Text;
                Exception error;
                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.Insert(ref ufs, out error);
                if (error != null)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSaveUserFoldersStructureFailed"));
                    return;
                }
                _ufs = ufs;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSaveUserFoldersStructureFailed"));
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            CreateUserFolderRoot();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _ufs = null;
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        UserFoldersStructure _ufs = null;

        public void ShowDialog(out UserFoldersStructure ufs)
        {
            ShowDialog();
            ufs = _ufs;
        }

        private void _userTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_userTreeView.SelectedNode != null)
            {
                if (e.Action != TreeViewAction.Unknown && _userTreeView.SelectedNode != _mainNode)
                    _selectedItem = new UserFoldersStructureDictionaryItemObject(_userTreeView.SelectedNode);

                _userTreeView.SelectedNode.ForeColor = SystemColors.HighlightText;
                _userTreeView.SelectedNode.BackColor = SystemColors.Highlight;
            }
        }

        private void _userTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_userTreeView.SelectedNode != null)
            {
                _userTreeView.SelectedNode.ForeColor = _userTreeView.ForeColor;
                _userTreeView.SelectedNode.BackColor = _userTreeView.BackColor;
            }
        }
    }
}
