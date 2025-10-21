using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Client
{
    public partial class UserFoldersStructuresForm :
#if DESIGNER
        Form
#else
        ACgpTableForm<UserFoldersStructure, UserFoldersStructure>
#endif
    {
        private const string MAIN_NODE_ICON = "MAIN_NODE_ICON";
        private const string COLLAPS_NODE_ICON = "COLLAPS_NODE_ICON";
        private const string EXPAND_NODE_ICON = "EXPAND_NODE_ICON";
        private TreeNode _mainNode = null;

        private static volatile UserFoldersStructuresForm _singleton;
        private static object _syncRoot = new object();

        private UserFoldersStructureItem _userFoldersStructureItem = null;
        private List<string> _loadPlug = new List<string>();
        private Dictionary<UserFoldersStructureDictionaryItem, TreeNode> _foldersTreeNodes = new Dictionary<UserFoldersStructureDictionaryItem, TreeNode>();
        private UserFoldersStructureDictionaryItemObject _selectedItem = null;
        private ImageList _imageList = new ImageList();
        private List<UserFoldersStructureDictionaryItem> _userFoldersStructureToEditEnd = new List<UserFoldersStructureDictionaryItem>();

        public UserFoldersStructuresForm()
        {
            InitializeComponent();
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.Images.Add(MAIN_NODE_ICON, ResourceGlobal.UserFoldersStructure);
            _imageList.Images.Add(COLLAPS_NODE_ICON, ResourceGlobal.Folder_collaps);
            _imageList.Images.Add(EXPAND_NODE_ICON, ResourceGlobal.Folder_expand);
            _userTreeView.ImageList = _imageList;

            this._bDockUndock.Text = this.LocalizationHelper.GetString("ObjectsSearchForm_bUndock");
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ControlBox = true;
            this.MinimizeBox = true;
            this.ShowInTaskbar = true;
            this.MinimumSize = new Size(this.Width, this.Height);

            var values = Enum.GetValues(typeof(ObjectType));
            foreach (ObjectType enumValue in values)
            {
                Icon icon = DbsSupport.GetIconForObjectType(enumValue);
                if (icon != null)
                    _imageList.Images.Add(enumValue.ToString(), icon);
            }         
        }

        public static UserFoldersStructuresForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new UserFoldersStructuresForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override UserFoldersStructure GetObjectForEdit(UserFoldersStructure listObj, out bool editAllowed)
        {
            editAllowed = HasAccessUpdate();
            return listObj;
        }

        protected override UserFoldersStructure GetFromShort(UserFoldersStructure listObj)
        {
            return listObj;
        }

        protected override UserFoldersStructure GetById(object idObj)
        {
            return null;
        }

        protected override ICollection<UserFoldersStructure> GetData()
        {
            _foldersTreeNodes.Clear();
            _userFoldersStructureItem = new UserFoldersStructureItem(null, false);
            ICollection<UserFoldersStructure> data = ReadData(null, _userFoldersStructureItem);

            CheckAccess();

            return data;
        }

        private ICollection<UserFoldersStructure> ReadData(UserFoldersStructure userFolderStructure, UserFoldersStructureItem userFoldersStructureItem)
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

            return list;
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

        private void CheckAccess()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(CheckAccess));
            else
            {
                _bCreate.Enabled = HasAccessInsert();
                _bDelete.Enabled = HasAccessDelete();

                if (HasAccessUpdate())
                {
                    _bRename.Enabled = true;
                    _bAssignObject.Enabled = true;
                    _bCreateObject.Enabled = true;
                    _bUnassignObject.Enabled = true;
                    _bOpenEditForm.Enabled = true;

                    TreeViewEventArgs tvEventsArgs = new TreeViewEventArgs(_userTreeView.SelectedNode);
                    _userTreeView_AfterSelect(_userTreeView, tvEventsArgs);
                }
                else
                {
                    _bRename.Enabled = false;
                    _bAssignObject.Enabled = false;
                    _bCreateObject.Enabled = false;
                    _bUnassignObject.Enabled = false;
                    _bOpenEditForm.Enabled = false;
                }
            }
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessInsert();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessUpdate()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessUpdate();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override bool Compare(UserFoldersStructure obj1, UserFoldersStructure obj2)
        {
            return obj1.Compare(obj2);
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _miAssignObject.DropDownItems.Clear();
            _cmAssignObject.Items.Clear();
            _miCreateObject.DropDownItems.Clear();
            _cmCreateObject.Items.Clear();

            _loadPlug.Clear();
            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                _loadPlug.Add(plugin._plugin.FriendlyName);
            }

            ICollection<TableTypeSettings> objectTypes = CgpClient.Singleton.MainServerProvider.GetSearchableObjectTypes(_loadPlug);
            if (objectTypes != null)
            {
                List<ToolStripItem> listMiAssignObject = new List<ToolStripItem>();
                List<ToolStripItem> listCmAssignObject = new List<ToolStripItem>();
                List<ToolStripItem> listMiCreateObject = new List<ToolStripItem>();
                List<ToolStripItem> listCmCreateObject = new List<ToolStripItem>();

                foreach (TableTypeSettings tableTypeSettings in objectTypes)
                {
                    if (tableTypeSettings.TypeName == (typeof(UserFoldersStructure).Name + "s"))
                    {
                        continue;
                    }
                    
                    string name = string.Empty;
                    if (tableTypeSettings.PluginName == string.Empty)
                        name = GetString("ObjectType_" + tableTypeSettings.TypeName.Substring(0, tableTypeSettings.TypeName.Length - 1));
                    else
                        name = GetObjectTypeNameFromPlugin(tableTypeSettings.TypeName, tableTypeSettings.PluginName);

                    ToolStripItem toolStripItem = new ToolStripMenuItem(name);
                    toolStripItem.Tag = tableTypeSettings;
                    toolStripItem.Click += new EventHandler(_miAssignObjectType_Click);
                    listMiAssignObject.Add(toolStripItem);
                    toolStripItem = new ToolStripMenuItem(name);
                    toolStripItem.Tag = tableTypeSettings;
                    toolStripItem.Click += new EventHandler(_miAssignObjectType_Click);
                    listCmAssignObject.Add(toolStripItem);

                    if (tableTypeSettings.CanCreate)
                    {
                        toolStripItem = new ToolStripMenuItem(name);
                        toolStripItem.Tag = tableTypeSettings;
                        toolStripItem.Click += new EventHandler(_miCreateObjectType_Click);
                        listMiCreateObject.Add(toolStripItem);
                        toolStripItem = new ToolStripMenuItem(name);
                        toolStripItem.Tag = tableTypeSettings;
                        toolStripItem.Click += new EventHandler(_miCreateObjectType_Click);
                        listCmCreateObject.Add(toolStripItem);
                    }
                }

                listMiAssignObject = listMiAssignObject.OrderBy(tsi => tsi.Text).ToList();
                listCmAssignObject = listCmAssignObject.OrderBy(tsi => tsi.Text).ToList();
                listMiCreateObject = listMiCreateObject.OrderBy(tsi => tsi.Text).ToList();
                listCmCreateObject = listCmCreateObject.OrderBy(tsi => tsi.Text).ToList();

                foreach (ToolStripItem tsi in listMiAssignObject)
                {
                    _miAssignObject.DropDownItems.Add(tsi);
                }

                foreach (ToolStripItem tsi in listMiCreateObject)
                {
                    _miCreateObject.DropDownItems.Add(tsi);
                }

                foreach (ToolStripItem tsi in listCmAssignObject)
                {
                    _cmAssignObject.Items.Add(tsi);
                }

                foreach (ToolStripItem tsi in listCmCreateObject)
                {
                    _cmCreateObject.Items.Add(tsi);
                }
            }

            if (_mainNode != null)
            {
                _mainNode.Remove();
                _mainNode = null;
            }

            _mainNode = new TreeNode(GetString("UserFoldersStructuresForm_RootFolderName"));
            SetNodeIcon(_mainNode, MAIN_NODE_ICON);
            _userTreeView.Nodes.Add(_mainNode);
            _userTreeView.SelectedNode = _userTreeView.Nodes[0];

            AddToTreeView(_userFoldersStructureItem, _mainNode);
            _userFoldersStructureItem = null;
            _mainNode.Expand();

            SelectFolder();
        }

        private string GetObjectTypeNameFromPlugin(string typeName, string pluginName)
        {
            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                if (plugin._plugin.FriendlyName == pluginName)
                {
                    ICgpVisualPlugin visualPlugin = plugin._plugin as ICgpVisualPlugin;

                    if (visualPlugin != null)
                    {
                        string translateText = visualPlugin.GetTranslateTableObjectTypeName(typeName);
                        if (translateText != string.Empty)
                            return translateText;
                    }
                }
            }

            return string.Empty;
        }

        private string GetAddFormNameFromPlugin(string typeName, string pluginName)
        {
            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                if (plugin._plugin.FriendlyName == pluginName)
                {
                    ICgpVisualPlugin visualPlugin = plugin._plugin as ICgpVisualPlugin;

                    if (visualPlugin != null)
                    {
                        string translateText = visualPlugin.GetTranslateString(plugin._plugin.Description + typeName + "Form" +
                            plugin._plugin.Description + typeName + "Form");

                        if (translateText != string.Empty)
                            return translateText;
                    }
                }
            }

            return string.Empty;
        }

        private void SetNodeIcon(TreeNode node, string key)
        {
            node.ImageKey = key;
            node.SelectedImageKey = key;
            node.StateImageKey = key;
        }

        private void _miAssignObjectType_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true) || _userTreeView.SelectedNode == null ||
                _userTreeView.SelectedNode == _mainNode)
                return;
            
            ToolStripItem toolStripItem = sender as ToolStripItem;

            if (toolStripItem != null)
            {
                TableTypeSettings tableTypeSetting = toolStripItem.Tag as TableTypeSettings;
                if (tableTypeSetting != null)
                {
                    IEnumerable<AOrmObject> list = CgpClient.Singleton.MainServerProvider.GetAllOrmObjectsOfObjectType(tableTypeSetting.ObjectType, _loadPlug);

                    if (list != null)
                    {
                        List<IListObjects> listOrmObjects = new List<IListObjects>();
                        foreach (AOrmObject ormObject in list)
                        {
                            listOrmObjects.Add(new OrmObjectItem(ormObject));
                        }

                        string formName = string.Empty;
                        if (tableTypeSetting.PluginName == string.Empty)
                            formName = GetString(tableTypeSetting.TypeName + "Form" + tableTypeSetting.TypeName + "Form");
                        else
                            formName = GetAddFormNameFromPlugin(tableTypeSetting.TypeName, tableTypeSetting.PluginName);

                        ListboxFormAdd formAdd = new ListboxFormAdd(listOrmObjects, formName);
                        object outObject;
                        formAdd.ShowDialog(out outObject);
                        IListObjects ormObjectItem = outObject as IListObjects;
                        if (ormObjectItem != null)
                        {
                            AOrmObject ormObject = ormObjectItem.GetOrmObj();
                            AddOrmObject(ormObject);
                        }
                    }
                }
            }
        }

        private void AddOrmObject(AOrmObject ormObject)
        {
            if (ormObject != null)
            {
                TreeViewNodesSettings treeViewNodeSetting = _userTreeView.SelectedNode.Tag as TreeViewNodesSettings;
                if (treeViewNodeSetting != null && treeViewNodeSetting.UserFoldersStructure != null)
                {
                    if (CheckUniqueOrmObject(treeViewNodeSetting.UserFoldersStructure, ormObject.GetIdString(), ormObject.GetObjectType()))
                    {
                        TreeViewCancelEventArgs tvcEventArgs = new TreeViewCancelEventArgs(_userTreeView.SelectedNode, false, TreeViewAction.Expand);
                        _userTreeView_BeforeExpand(_userTreeView, tvcEventArgs);
                        if (treeViewNodeSetting.UserFoldersStructure.UserFoldersStructureObjects == null)
                        {
                            treeViewNodeSetting.UserFoldersStructure.UserFoldersStructureObjects = new List<UserFoldersStructureObject>();
                        }

                        UserFoldersStructureObject userFoldersStructureObject = new UserFoldersStructureObject();
                        userFoldersStructureObject.ObjectType = ormObject.GetObjectType();
                        userFoldersStructureObject.ObjectId = ormObject.GetIdString();
                        userFoldersStructureObject.Folder = treeViewNodeSetting.UserFoldersStructure;

                        treeViewNodeSetting.UserFoldersStructure.UserFoldersStructureObjects.Add(userFoldersStructureObject);

                        TreeNode node = new TreeNode(OrmObjectItem.ToString(ormObject));
                        node.Tag = userFoldersStructureObject;
                        SetNodeIcon(node, userFoldersStructureObject.ObjectType.ToString());
                        _userTreeView.SelectedNode.Nodes.Add(node);
                        TreeNode parentNode = _userTreeView.SelectedNode;
                        SaveUserFolderStructure(parentNode);
                        _selectedItem = new UserFoldersStructureDictionaryItemObject(node);
                        _userTreeView.SelectedNode = node;
                    }
                }
            }
        }

        private void _miCreateObjectType_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true) || _userTreeView.SelectedNode == null ||
                _userTreeView.SelectedNode == _mainNode)
                return;

            ToolStripItem toolStripItem = sender as ToolStripItem;

            if (toolStripItem != null)
            {
                TableTypeSettings tableTypeSetting = toolStripItem.Tag as TableTypeSettings;
                
                if (tableTypeSetting != null)
                    DbsSupport.OpenInsertForm(tableTypeSetting.TypeName, new Action<object>(OrmObjectAfterInsert));
            }
        }

        private void OrmObjectAfterInsert(object obj)
        {
            AOrmObject ormObject = obj as AOrmObject;
            if (ormObject != null)
            {
                AddOrmObject(ormObject);
            }
        }

        private bool CheckUniqueOrmObject(UserFoldersStructure userFoldersStructure, string newObjectId, ObjectType newObjectType)
        {
            if (userFoldersStructure != null && userFoldersStructure.UserFoldersStructureObjects != null &&
                userFoldersStructure.UserFoldersStructureObjects.Count > 0)
            {
                foreach (UserFoldersStructureObject userFoldersStructureObject in userFoldersStructure.UserFoldersStructureObjects)
                {
                    if (newObjectId == userFoldersStructureObject.ObjectId && newObjectType == userFoldersStructureObject.ObjectType)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorOrmObjectAlreadyExists"));
                        return false;
                    }
                }
            }

            return true;
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

            if (userFoldersStructureItem.UserFoldersStructure != null &&
                userFoldersStructureItem.UserFoldersStructure.UserFoldersStructureObjects != null &&
                userFoldersStructureItem.UserFoldersStructure.UserFoldersStructureObjects.Count > 0)
            {
                foreach (UserFoldersStructureObject userFoldersStructureObject in userFoldersStructureItem.UserFoldersStructure.UserFoldersStructureObjects)
                {
                    AOrmObject ormObject = DbsSupport.GetTableObject(userFoldersStructureObject.ObjectType, userFoldersStructureObject.ObjectId);
                    if (ormObject != null &&
                        CgpClient.Singleton.MainServerProvider.HasAccessView(
                            ormObject.GetObjectType(),
                            ormObject.GetId()))
                    {
                        TreeNode ormObjectNode = new TreeNode(OrmObjectItem.ToString(ormObject));
                        ormObjectNode.Tag = userFoldersStructureObject;
                        SetNodeIcon(ormObjectNode, userFoldersStructureObject.ObjectType.ToString());
                        node.Nodes.Add(ormObjectNode);
                    }
                }
            }
        }

        protected override void RemoveGridView()
        {
        }

        #endregion

        protected override ACgpEditForm<UserFoldersStructure> CreateEditForm(UserFoldersStructure obj, ShowOptionsEditForm showOption)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteObj(UserFoldersStructure obj)
        {
            Exception error;
            //if (!CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.RemoveDepartment(obj, out error))
            //    throw error;
            if (!CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.Delete(obj, out error))
                throw error;

            UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(obj);
            if (_userFoldersStructureToEditEnd.Contains(userFoldersStructureDictionaryItem))
                _userFoldersStructureToEditEnd.Remove(userFoldersStructureDictionaryItem);
        }

        protected override void DeleteObjById(object obj)
        {
            UserFoldersStructure ufs = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectById(obj);

            DeleteObj(ufs);
        }

        protected override void SetFilterSettings()
        {
            throw new NotImplementedException();
        }

        protected override void ClearFilterEdits()
        {
            throw new NotImplementedException();
        }

        private bool HasFolderChildren(TreeNode node)
        {
            if (node.Tag is Contal.Cgp.Client.TreeViewNodesSettings)
            {
                Contal.Cgp.Client.TreeViewNodesSettings twBN = node.Tag as TreeViewNodesSettings;
                UserFoldersStructure ufs = twBN.UserFoldersStructure;
                return CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasSubFolders(ufs);
            }
            return false;
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_userTreeView.SelectedNode == null || _userTreeView.SelectedNode == _mainNode)
                return;
                
            if (_userTreeView.SelectedNode.Tag is TreeViewNodesSettings)
            {
                if (HasFolderChildren(_userTreeView.SelectedNode))
                {
                    if (Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionDeleteFolder")))
                    {
                        DeleteNode(_userTreeView.SelectedNode);
                        _userTreeView.SelectedNode.Remove();
                    }
                }
                else
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteSingleFolder")))
                    {
                        DeleteNode(_userTreeView.SelectedNode);
                        _userTreeView.SelectedNode.Remove();
                    }
                }
            }
            //else if (_userTreeView.SelectedNode.Tag is UserFoldersStructureObject)
            //{
            //    if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteOrmObject")))
            //    {
            //        DeleteOrmObject(_userTreeView.SelectedNode);
            //        _userTreeView.SelectedNode.Remove();
            //    }
            //}

            _selectedItem = new UserFoldersStructureDictionaryItemObject(_userTreeView.SelectedNode);
        }

        private void DeleteNode(TreeNode node)
        {
            if (node.Tag is TreeViewNodesSettings)
            {
                DeleteFolder(node);
            }
            else if (node.Tag is UserFoldersStructureObject)
            {
                DeleteOrmObject(node);
            }

        }

        private void DeleteOrmObject(TreeNode node)
        {
            if (node.Parent == null)
                return;

            TreeViewNodesSettings treeViewNodesSettings = node.Parent.Tag as TreeViewNodesSettings;
            if (treeViewNodesSettings != null && treeViewNodesSettings.UserFoldersStructure != null)
            {
                UserFoldersStructureObject userFoldersStructureObject = node.Tag as UserFoldersStructureObject;
                if (userFoldersStructureObject != null)
                {
                    UserFoldersStructureObject deletedUserFoldersStructureObject = FindUserfFolderStructureObject(treeViewNodesSettings.UserFoldersStructure, userFoldersStructureObject.ObjectId, userFoldersStructureObject.ObjectType);
                    
                    if (deletedUserFoldersStructureObject != null)
                    {
                        //CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.OnDeleteUfsObj(deletedUserFoldersStructureObject);
                        treeViewNodesSettings.UserFoldersStructure.UserFoldersStructureObjects.Remove(deletedUserFoldersStructureObject);
                        SaveUserFolderStructure(node.Parent);
                    }
                }
            }
        }

        private UserFoldersStructureObject FindUserfFolderStructureObject(UserFoldersStructure userFolderStructure, string objectId, ObjectType objectType)
        {
            if (userFolderStructure == null || userFolderStructure.UserFoldersStructureObjects == null ||
                userFolderStructure.UserFoldersStructureObjects.Count == 0)
                return null;

            foreach (UserFoldersStructureObject userFoldersStructureObject in userFolderStructure.UserFoldersStructureObjects)
            {
                if (userFoldersStructureObject.ObjectId == objectId && userFoldersStructureObject.ObjectType == objectType)
                    return userFoldersStructureObject;
            }

            return null;
        }

        private void DeleteFolder(TreeNode node)
        {
            if (node != null)
            {
                TreeViewCancelEventArgs tvcEventArgs = new TreeViewCancelEventArgs(node, false, TreeViewAction.Expand);
                _userTreeView_BeforeExpand(_userTreeView, tvcEventArgs);
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    foreach (TreeNode actNode in node.Nodes)
                    {
                        DeleteNode(actNode);
                    }
                }

                TreeViewNodesSettings treeViewNodesSettings = node.Tag as TreeViewNodesSettings;
                if (treeViewNodesSettings != null && treeViewNodesSettings.UserFoldersStructure != null)
                {
                    DeleteObj(treeViewNodesSettings.UserFoldersStructure);
                }
            }
        }

        private void _bRename_Click(object sender, EventArgs e)
        {
            if (_userTreeView.SelectedNode == null || _userTreeView.SelectedNode == _mainNode ||
                !(_userTreeView.SelectedNode.Tag is TreeViewNodesSettings) || _userTreeView.SelectedNode.Parent == null)
                return;

            string name = _userTreeView.SelectedNode.Text;

            RenameFolderForm form = new RenameFolderForm();
            DialogResult result = form.ShowDialog(ref name);

            if (result == DialogResult.OK && CheckUniqueName(_userTreeView.SelectedNode.Parent, name))
            {
                _userTreeView.SelectedNode.Text = name;
                SaveUserFolderStructure(_userTreeView.SelectedNode);
            }
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            if (_userTreeView.SelectedNode == null || (_userTreeView.SelectedNode != _mainNode &&
                !(_userTreeView.SelectedNode.Tag is TreeViewNodesSettings)))
                return;

            string name;

            CreateFolderForm form = new CreateFolderForm();
            DialogResult result = form.ShowDialog(out name);

            if (result == DialogResult.OK && CheckUniqueName(_userTreeView.SelectedNode, name))
            {
                TreeNode node = new TreeNode(name);
                node.Tag = new TreeViewNodesSettings(null);
                _userTreeView.SelectedNode.Nodes.Add(node);
                
                _userTreeView.SelectedNode = node;
                SetNodeIcon(node, EXPAND_NODE_ICON);
                SaveUserFolderStructure(node);
                _selectedItem = new UserFoldersStructureDictionaryItemObject(node);
            }
        }

        private bool CheckUniqueName(TreeNode parentNode, string name)
        {
            if (parentNode != null && parentNode.Nodes != null && parentNode.Nodes.Count > 0)
            {
                foreach (TreeNode node in parentNode.Nodes)
                {
                    if (node.Text == name)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorFolderNameAlreadyExists"));
                        return false;
                    }
                }
            }

            return true;
        }

        private void SaveUserFolderStructure(TreeNode node)
        {
            try
            {
                if (node != null)
                {
                    TreeViewNodesSettings treeViewNodesSetting = node.Tag as TreeViewNodesSettings;

                    if (treeViewNodesSetting != null)
                    {
                        if (treeViewNodesSetting.UserFoldersStructure == null)
                        {
                            Insert(node, treeViewNodesSetting);
                        }
                        else
                        {
                            Update(node, treeViewNodesSetting);
                        }
                    }
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSaveUserFoldersStructureFailed"));
            }
        }

        private void Insert(TreeNode node, TreeViewNodesSettings treeViewNodesSetting)
        {
            if (node == null || treeViewNodesSetting == null)
                return;

            UserFoldersStructure parentFolder = null;

            if (node.Parent != null)
            {
                TreeViewNodesSettings parentTreeViewNodesSetting = node.Parent.Tag as TreeViewNodesSettings;
                if (parentTreeViewNodesSetting != null)
                    parentFolder = parentTreeViewNodesSetting.UserFoldersStructure;
            }

            UserFoldersStructure userFolderStructure = new UserFoldersStructure();
            userFolderStructure.ParentFolder = parentFolder;
            userFolderStructure.FolderName = node.Text;

            SaveToDatabaseInsert(ref userFolderStructure);
            treeViewNodesSetting.UserFoldersStructure = userFolderStructure;
        }

        private void Update(TreeNode node, TreeViewNodesSettings treeViewNodesSetting)
        {
            if (node == null || treeViewNodesSetting == null)
                return;

            UserFoldersStructure userFolderStructure = treeViewNodesSetting.UserFoldersStructure;
            userFolderStructure.FolderName = node.Text;

            SaveToDatabaseEdit(ref userFolderStructure);
            treeViewNodesSetting.UserFoldersStructure = userFolderStructure;
        }

        private void SaveToDatabaseInsert(ref UserFoldersStructure userFolderStructure)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.Insert(ref userFolderStructure, out error))
            {
                if (error != null)
                    throw error;
                else
                    throw new Exception();
            }

            userFolderStructure = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectForEdit(userFolderStructure.IdUserFoldersStructure, out error);
            if (error != null)
                throw error;

            UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(userFolderStructure);
            if (!_userFoldersStructureToEditEnd.Contains(userFoldersStructureDictionaryItem))
                _userFoldersStructureToEditEnd.Add(userFoldersStructureDictionaryItem);
        }

        private void SaveToDatabaseEdit(ref UserFoldersStructure userFolderStructure)
        {
            if (userFolderStructure == null)
                return;

            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.Update(userFolderStructure, out error);

            if (!retValue)
            {
                if (error != null)
                    throw error;
                else
                    throw new Exception();
            }

            UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(userFolderStructure);
            if (_userFoldersStructureToEditEnd.Contains(userFoldersStructureDictionaryItem))
                _userFoldersStructureToEditEnd.Remove(userFoldersStructureDictionaryItem);

            userFolderStructure = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectForEdit(userFolderStructure.IdUserFoldersStructure, out error);
            if (error != null)
                throw error;

            userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(userFolderStructure);
            if (!_userFoldersStructureToEditEnd.Contains(userFoldersStructureDictionaryItem))
                _userFoldersStructureToEditEnd.Add(userFoldersStructureDictionaryItem);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _userTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_userTreeView.SelectedNode != null)
            {
                _userTreeView.SelectedNode.ForeColor = _userTreeView.ForeColor;
                _userTreeView.SelectedNode.BackColor = _userTreeView.BackColor;
            }
        }


        private void _userTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_userTreeView.SelectedNode != null)
            {
                if (e.Action != TreeViewAction.Unknown && _userTreeView.SelectedNode != _mainNode)
                    _selectedItem = new UserFoldersStructureDictionaryItemObject(_userTreeView.SelectedNode);

                _userTreeView.SelectedNode.ForeColor = SystemColors.HighlightText;
                _userTreeView.SelectedNode.BackColor = SystemColors.Highlight;

                if (_userTreeView.SelectedNode == _mainNode)
                {
                    _bCreate.Enabled = true;
                    _bRename.Enabled = false;
                    _bDelete.Enabled = false;
                    _bAssignObject.Enabled = false;
                    _bOpenEditForm.Enabled = false;
                    _bCreateObject.Enabled = false;
                    _bUnassignObject.Enabled = false;
                }
                else if (_userTreeView.SelectedNode.Tag is UserFoldersStructureObject)
                {
                    _bCreate.Enabled = false;
                    _bRename.Enabled = false;
                    _bDelete.Enabled = false;
                    _bAssignObject.Enabled = false;
                    _bOpenEditForm.Enabled = true;
                    _bCreateObject.Enabled = false;
                    _bUnassignObject.Enabled = true;
                }
                else
                {
                    _bCreate.Enabled = true;
                    _bRename.Enabled = true;
                    _bDelete.Enabled = true;
                    _bAssignObject.Enabled = true;
                    _bOpenEditForm.Enabled = false;
                    _bUnassignObject.Enabled = false;
                    _bCreateObject.Enabled = true;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false) && _userFoldersStructureToEditEnd != null && _userFoldersStructureToEditEnd.Count > 0)
            {
                foreach (UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem in _userFoldersStructureToEditEnd)
                {
                    if (userFoldersStructureDictionaryItem.UserFoldersStructure != null)
                        CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.EditEnd(userFoldersStructureDictionaryItem.UserFoldersStructure);
                }
            }

            base.OnClosing(e);
        }

        private void _userTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                if (e.Node == _mainNode)
                {
                    _miCreate.Visible = true;
                    _miDelete.Visible = false;
                    _miRename.Visible = false;
                    _miAssignObject.Visible = false;
                    _miOpenEditForm.Visible = false;
                    _miCreateObject.Visible = false;
                    _miUnassignObject.Visible = false;
                }
                else if (e.Node.Tag is UserFoldersStructureObject)
                {
                    _miCreate.Visible = false;
                    _miDelete.Visible = false;
                    _miRename.Visible = false;
                    _miAssignObject.Visible = false;
                    _miOpenEditForm.Visible = true;
                    _miCreateObject.Visible = false;
                    _miUnassignObject.Visible = true;
                }
                else
                {
                    _miCreate.Visible = true;
                    _miDelete.Visible = true;
                    _miRename.Visible = true;
                    _miAssignObject.Visible = true;
                    _miOpenEditForm.Visible = false;
                    _miCreateObject.Visible = true;
                    _miUnassignObject.Visible = false;
                }

                _userTreeView.SelectedNode = e.Node;
                _cmTreeView.Show(MousePosition);
            }
        }

        protected override void AfterTranslateForm()
        {
            if (_bDockUndock != null)
            {
                this._bDockUndock.Text = 
                    this.LocalizationHelper.GetString(
                        this.MdiParent == null ? "ObjectsSearchForm_bDockUndock" : "ObjectsSearchForm_bUndock");
            }

            foreach (ToolStripItem item in _cmTreeView.Items)
            {
                item.Text = GetString(Name + item.Name);
            }
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

        public void OpenFormAndSelectFolder(UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem, AOrmObject ormObject)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<UserFoldersStructureDictionaryItem, AOrmObject>(OpenFormAndSelectFolder), userFoldersStructureDictionaryItem, ormObject);
            }
            else
            {
                Show();
                UserFoldersStructureDictionaryItemObject userFoldersStructureDictionaryItemObject = new UserFoldersStructureDictionaryItemObject(userFoldersStructureDictionaryItem, ormObject);
                _selectedItem = userFoldersStructureDictionaryItemObject;
            }

        }

        private void SelectFolder()
        {
            if (_selectedItem != null && _selectedItem.UserFoldersStructureDictionaryItem != null)
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

        private void _userTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node != _mainNode)
            {
                SetNodeIcon(e.Node, EXPAND_NODE_ICON);
            }
        }

        private void _userTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node != _mainNode)
            {
                SetNodeIcon(e.Node, COLLAPS_NODE_ICON);
            }
        }

        private void _userTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenEditForm(e.Node);
        }

        private void OpenEditForm(TreeNode node)
        {
            if (node != null && node != _mainNode)
            {
                UserFoldersStructureObject userFoldersStructureObject = node.Tag as UserFoldersStructureObject;
                if (userFoldersStructureObject != null)
                {
                    AOrmObject ormObject = DbsSupport.GetTableObject(userFoldersStructureObject.ObjectType, userFoldersStructureObject.ObjectId);
                    DbsSupport.OpenEditForm(ormObject);
                }
            }
        }

        private void _miOpenEditForm_Click(object sender, EventArgs e)
        {
            OpenEditForm(_userTreeView.SelectedNode);
        }

        private void _bAssignObject_Click(object sender, EventArgs e)
        {
            _cmAssignObject.Show(new Point(MousePosition.X, MousePosition.Y - _cmAssignObject.Height));
        }

        private void _bOpenEditForm_Click(object sender, EventArgs e)
        {
            OpenEditForm(_userTreeView.SelectedNode);
        }

        private void _bCreateObject_Click(object sender, EventArgs e)
        {
            _cmCreateObject.Show(new Point(MousePosition.X, MousePosition.Y - _cmAssignObject.Height));
        }

        private void _userTreeView_DragOver(object sender, DragEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            pt = _userTreeView.PointToClient(pt);
            TreeNode node = _userTreeView.GetNodeAt(pt);

            if (node != null)
                _userTreeView.SelectedNode = node;

            if (node != null && node != _mainNode && node.Tag is TreeViewNodesSettings)
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void _userTreeView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AOrmObject ormObject = e.Data.GetData(output[0]) as AOrmObject;
                AddOrmObject(ormObject);
            }
            catch
            {
            }
        }

        private void _bDockUndock_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
            {
                Close();
            }
            else
            {
                if (this.MdiParent == null)
                {
                    this.TopMost = false;
                    this.WindowState = FormWindowState.Normal;
                    this.MdiParent = CgpClientMainForm.Singleton;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.Dock = DockStyle.Fill;
                    CgpClientMainForm.Singleton.AddToOpenWindows(this);
                    CgpClientMainForm.Singleton.SetActOpenWindow(this);
                    this._bDockUndock.Text = this.LocalizationHelper.GetString("ObjectsSearchForm_bUndock");
                }
                else
                {
                    this.MdiParent = null;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    //this.TopMost = true;
                    CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
                    this._bDockUndock.Text = this.LocalizationHelper.GetString("ObjectsSearchForm_bDockUndock");
                }
            }
        }

        private void _bUnassignObject_Click(object sender, EventArgs e)
        {
            if (_userTreeView.SelectedNode == null || _userTreeView.SelectedNode == _mainNode)
                return;
                
            if (_userTreeView.SelectedNode.Tag is UserFoldersStructureObject)
            {
                if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteOrmObject")))
                {
                    DeleteOrmObject(_userTreeView.SelectedNode);
                    _userTreeView.SelectedNode.Remove();
                }
            }

            _selectedItem = new UserFoldersStructureDictionaryItemObject(_userTreeView.SelectedNode);
        }
    }

    public class UserFoldersStructureItem
    {
        private UserFoldersStructure _userFoldersStructure = null;
        private List<UserFoldersStructureItem> _nodes = new List<UserFoldersStructureItem>();
        private bool _hasSubFolders = false;

        public UserFoldersStructure UserFoldersStructure { get { return _userFoldersStructure; } }
        public List<UserFoldersStructureItem> Nodes { get { return _nodes; } set { _nodes = value; } }
        public bool HasSubFolders { get { return _hasSubFolders; } }

        public UserFoldersStructureItem(UserFoldersStructure userFoldersStructure, bool hasSubFolders)
        {
            _userFoldersStructure = userFoldersStructure;
            _hasSubFolders = hasSubFolders;
        }
    }

    public class TreeViewNodesSettings
    {
        private UserFoldersStructure _userFoldersStructure = null;

        public UserFoldersStructure UserFoldersStructure { get { return _userFoldersStructure; } set { _userFoldersStructure = value; } }

        public TreeViewNodesSettings(UserFoldersStructure userFoldersStructure)
        {
            if (userFoldersStructure != null)
            {
                Exception error;
                _userFoldersStructure = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectForEdit(userFoldersStructure.IdUserFoldersStructure, out error);
                
                if (error != null)
                    throw (error);
            }
        }
    }

    public class UserFoldersStructureListBoxItem
    {
        UserFoldersStructure _userFoldersStructure = null;
        string _name = string.Empty;

        public UserFoldersStructure UserFoldersStructure { get { return _userFoldersStructure; } }

        public UserFoldersStructureListBoxItem(UserFoldersStructure userFoldersSturcture, string name)
        {
            _userFoldersStructure = userFoldersSturcture;
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class UserFoldersStructureDictionaryItem
    {
        UserFoldersStructure _userFoldersStructure = null;

        public UserFoldersStructure UserFoldersStructure { get { return _userFoldersStructure; } }

        public UserFoldersStructureDictionaryItem(UserFoldersStructure userFoldersStructure)
        {
            _userFoldersStructure = userFoldersStructure;
        }

        public static bool operator ==(UserFoldersStructureDictionaryItem obj1, UserFoldersStructureDictionaryItem obj2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)obj1 == null) || ((object)obj2 == null))
            {
                return false;
            }

            if (obj1._userFoldersStructure == obj2._userFoldersStructure)
            {
                return true;
            }

            if (obj1._userFoldersStructure == null)
            {
                return false;
            }

            // Return true if the fields match:

            return obj1._userFoldersStructure.Compare(obj2._userFoldersStructure);
        }

        public static bool operator !=(UserFoldersStructureDictionaryItem obj1, UserFoldersStructureDictionaryItem obj2)
        {
            return !(obj1 == obj2);
        }

        public override int GetHashCode()
        {
            if (_userFoldersStructure == null)
                return IwQuick.Crypto.QuickHashes.GetSHA1String(ToString()).GetHashCode();
            else
                return IwQuick.Crypto.QuickHashes.GetSHA1String(_userFoldersStructure.IdUserFoldersStructure.ToString()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem = obj as UserFoldersStructureDictionaryItem;
            if (_userFoldersStructure == userFoldersStructureDictionaryItem._userFoldersStructure)
            {
                return true;
            }

            if (_userFoldersStructure == null)
            {
                return true;
            }

            return _userFoldersStructure.Compare(userFoldersStructureDictionaryItem._userFoldersStructure);
        }
    }

    public class UserFoldersStructureDictionaryItemObject
    {
        UserFoldersStructureDictionaryItem _userFoldersStructureDictionaryItem = null;
        string _ormObjectId = string.Empty;
        ObjectType _ormObjectType;

        public UserFoldersStructureDictionaryItem UserFoldersStructureDictionaryItem { get { return _userFoldersStructureDictionaryItem; } }
        public string OrmObjectId { get { return _ormObjectId; } }
        public ObjectType OrmObjectType { get { return _ormObjectType; } }

        public UserFoldersStructureDictionaryItemObject(UserFoldersStructureDictionaryItem userFoldersStructureDictionaryItem, AOrmObject ormObject)
        {
            _userFoldersStructureDictionaryItem = userFoldersStructureDictionaryItem;
            if (ormObject != null)
            {
                _ormObjectId = ormObject.GetIdString();
                _ormObjectType = ormObject.GetObjectType();
            }
        }

        public UserFoldersStructureDictionaryItemObject(TreeNode node)
        {
            if (node != null)
            {
                if (node.Tag is TreeViewNodesSettings)
                {
                    TreeViewNodesSettings treeViewNodesSettings = node.Tag as TreeViewNodesSettings;
                    if (treeViewNodesSettings != null)
                    {
                        _userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(treeViewNodesSettings.UserFoldersStructure);
                    }
                }
                else if (node.Tag is UserFoldersStructureObject)
                {
                    UserFoldersStructureObject userFoldersStructureObject = node.Tag as UserFoldersStructureObject;
                    if (userFoldersStructureObject != null)
                    {
                        _userFoldersStructureDictionaryItem = new UserFoldersStructureDictionaryItem(userFoldersStructureObject.Folder);
                        _ormObjectId = userFoldersStructureObject.ObjectId;
                        _ormObjectType = userFoldersStructureObject.ObjectType;
                    }
                }
            }
        }
    }

    public class OrmObjectItem : IListObjects
    {
        private AOrmObject _ormObject = null;

        public bool Contains(string expression)
        {
            if (_ormObject.Contains(expression))
                return true;
            if (this.ToString().ToLower().Contains(expression.ToLower()))
                return true;
            return false;
        }

        public AOrmObject GetOrmObj()
        {
            return _ormObject;
        }

        public OrmObjectItem(AOrmObject ormObject)
        {
            _ormObject = ormObject;
        }

        public static string ToString(AOrmObject ormObject)
        {
            if (ormObject == null)
                return string.Empty;

            switch (ormObject.GetObjectType())
            {
                case ObjectType.Calendar:
                    if (ormObject.ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                    {
                        return CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                    }
                    break;
                case ObjectType.DayType:
                    switch (ormObject.ToString())
                    {
                        case DayType.IMPLICIT_DAY_TYPE_HOLIDAY:
                            return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                        case DayType.IMPLICIT_DAY_TYPE_VACATION:
                            return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                    }
                    break;
                case ObjectType.DailyPlan:
                    switch (ormObject.ToString())
                    {
                        case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                            return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                        case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                            return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                        case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                            return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                        case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                            return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                    }
                    break;
            }
            return ormObject.ToString();
        }

        public override string ToString()
        {
            return ToString(_ormObject);
        }
    }
}
