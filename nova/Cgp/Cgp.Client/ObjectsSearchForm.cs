using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;
using System.Reflection;
using Contal.Cgp.Server.Beans;
using System.Threading;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Client
{
    public partial class ObjectsSearchForm :
#if DESIGNER
        Form
#else
 MdiChildForm
#endif
    {
        public ObjectsSearchForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            this.FormOnEnter += new Action<Form>(Form_Enter);
            this.VisibleChanged += new EventHandler(ObjectsSearchForm_VisibleChanged);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MdiParent = null;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.TopMost = true;
            this.MinimumSize = new Size(this.Width, this.Height);
            CgpClientMainForm.Singleton.AddToUndockedForms(this);
            this.LocalizationHelper.LanguageChanged += new Contal.IwQuick.DVoid2Void(LocalizationHelper_LanguageChanged);
            ObtainAllTypes();
            FillComboBoxSelectSearch();
            _cdgvData.DataGrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(_dgSearchResult_RowPrePaint);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormKeyDown);
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.DataGrid.DoubleClick += new EventHandler(_dgSearchResult_DoubleClick);
        }

        void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        void _dgSearchResult_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            AOrmObject ormObj = (AOrmObject)_bindingSourceOrmObjects.List[e.RowIndex];
            if (ormObj == null) return;

            DataGridViewRow row = _cdgvData.DataGrid.Rows[e.RowIndex];

            switch (ormObj.GetObjectType())
            {
                case ObjectType.Calendar:
                    if (ormObj.ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                    {
                        row.Cells["Object"].Value = LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                    }
                    else
                    {
                        row.Cells["Object"].Value = ormObj.ToString();
                    }
                    break;
                case ObjectType.DayType:
                    switch (ormObj.ToString())
                    {
                        case DayType.IMPLICIT_DAY_TYPE_HOLIDAY:
                            row.Cells["Object"].Value = LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                            break;
                        case DayType.IMPLICIT_DAY_TYPE_VACATION:
                            row.Cells["Object"].Value = LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                            break;
                        default:
                            row.Cells["Object"].Value = ormObj.ToString();
                            break;
                    }
                    break;
                case ObjectType.DailyPlan:
                    switch (ormObj.ToString())
                    {
                        case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                            row.Cells["Object"].Value = LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                            break;
                        case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                            row.Cells["Object"].Value = LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                            break;
                        case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                            row.Cells["Object"].Value = LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                            break;
                        case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                            row.Cells["Object"].Value = LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                            break;
                        default:
                            row.Cells["Object"].Value = ormObj.ToString();
                            break;
                    }
                    break;
                default:
                    row.Cells["Object"].Value = ormObj.ToString();
                    break;
            }

            if (row.Cells["ObjectType"].Value == null)
            {
                row.Cells["ObjectType"].Value = DbsSupport.GetIconForObjectType(ormObj.GetObjectType());
            }

            Type type = ormObj.GetType();
            PropertyInfo pi = type.GetProperty("Description");
            if (pi != null)
            {
                row.Cells["Description"].Value = pi.GetValue(ormObj, null) as string;
            }
        }

        void LocalizationHelper_LanguageChanged()
        {
            ObtainAllTypes();
            FillComboBoxSelectSearch();
        }

        private List<AOrmObject> _listOrmObjects = new List<AOrmObject>();
        BindingSource _bindingSourceOrmObjects = new BindingSource();

        private void CancelTopMost()
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Contal.IwQuick.DVoid2Void(CancelTopMost));
                }
                else
                {
                    //ObtainAllTypes();
                    Thread.Sleep(1);
                    this.TopMost = false;
                }
            }
            catch
            { }
        }

        private static volatile ObjectsSearchForm _singleton = null;
        private static object _syncRoot = new object();

        public static ObjectsSearchForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new ObjectsSearchForm();
                    }

                return _singleton;
            }
        }

        private bool _isRunedForm_Enter = false;
        private void Form_Enter(Form form)
        {
            if (!_isRunedForm_Enter)
            {
                CgpClientMainForm.Singleton.AddToOpenWindows(this);
                _isRunedForm_Enter = true;
            }

            if (MdiParent != null)
            {
                CgpClientMainForm.Singleton.AddToOpenWindows(this);
                CgpClientMainForm.Singleton.SetActOpenWindow(this);
                ShowSearchDataGrid();
            }

            this.TopMost = false;
        }


        void ObjectsSearchForm_VisibleChanged(object sender, EventArgs e)
        {
            if (MdiParent == null)
            {
                CgpClientMainForm.Singleton.AddToUndockedForms(this);
                ShowSearchDataGrid();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ClearSearchResults();
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
            if (this.FormBorderStyle == FormBorderStyle.Sizable)
            {
                CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
            }
            else
            {
                CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            }
        }

        private void ClearSearchResults()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ClearSearchResults));
            }
            else
            {
                _listOrmObjects.Clear();
                ShowSearchDataGrid();
            }
        }

        private void ObtainAllTypes()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            List<string> loadPlug = new List<string>();

            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                loadPlug.Add(plugin._plugin.FriendlyName);
            }

            ICollection<TableTypeSettings> listTypes = CgpClient.Singleton.MainServerProvider.GetSearchableObjectTypes(loadPlug);
            IList<SearchTypeObj> listSearchTypes = new List<SearchTypeObj>();

            if (listTypes != null)
            {
                foreach (TableTypeSettings typeNameAndPluginName in listTypes)
                {
                    string name;
                    if (typeNameAndPluginName.PluginName == string.Empty)
                        name = GetString(typeNameAndPluginName.TypeName + "Form" + typeNameAndPluginName.TypeName + "Form");
                    else
                        name = GetStringFromPlugin(typeNameAndPluginName.TypeName, typeNameAndPluginName.PluginName);

                    listSearchTypes.Add(new SearchTypeObj(typeNameAndPluginName.ObjectType, name));
                }
            }
            listSearchTypes = listSearchTypes.OrderBy(sto => sto.LocalizeName).ToList();

            _cbObjectType.Items.Clear();
            _cbObjectType.Items.Add(string.Empty);
            foreach (SearchTypeObj sto in listSearchTypes)
            {
                _cbObjectType.Items.Add(sto);
            }
        }

        private void bDockUndockClick(object sender, EventArgs e)
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
                CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
                this._bDockUndock.Text = this.LocalizationHelper.GetString("ObjectsSearchForm_bUndock");
            }
            else
            {
                this.MdiParent = null;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = true;
                CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
                CgpClientMainForm.Singleton.AddToUndockedForms(this);
                this._bDockUndock.Text = this.LocalizationHelper.GetString("ObjectsSearchForm_bDockUndock");
            }
            ShowSearchDataGrid();
        }

        private void _bSearch_Click(object sender, EventArgs e)
        {
            SearchType searchType;
            if (_cbSearchType.SelectedItem is DatabaseSearchType)
                searchType = ((DatabaseSearchType)_cbSearchType.SelectedItem).SearchType;
            else
                return;

            switch (searchType)
            {
                case SearchType.Fulltext:
                    if (ControlBeforeFullTextSearch())
                    {
                        //Search(_eNameParams.Text, null);
                        SearchFultext();
                    }
                    break;
                case SearchType.FulltextSearchByFolderStructure:
                    if (ControlBeforeFullTextSearch())
                    {
                        SearchFultextUserFolderStructure();
                    }
                    break;
                case SearchType.ParametricalSearchByType:
                    SearchParametric();
                    break;
            }
        }

        private void SearchFultext()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            string name = _eNameParams.Text;
            List<string> loadPlug = new List<string>();
            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                loadPlug.Add(plugin._plugin.FriendlyName);
            }

            IEnumerable<AOrmObject> listOrmObjects = CgpClient.Singleton.MainServerProvider.GetFultextSearchResult(name, loadPlug);
            _listOrmObjects.Clear();
            if (listOrmObjects != null)
            {
                _listOrmObjects.AddRange(listOrmObjects);
            }

            ShowSearchDataGrid();
        }

        private void SearchParametric()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            string name = _eNameParams.Text;
            ObjectType? objectType;

            if (_cbObjectType.SelectedItem is SearchTypeObj)
            {
                objectType = (_cbObjectType.SelectedItem as SearchTypeObj).ObjectType;
            }
            else
            {
                objectType = null;
            }

            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            List<string> loadPlug = new List<string>();

            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                loadPlug.Add(plugin._plugin.FriendlyName);
            }

            IEnumerable<AOrmObject> listOrmObjects = CgpClient.Singleton.MainServerProvider.GetParameterSearchResult(name, objectType, loadPlug);

            _listOrmObjects.Clear();
            if (listOrmObjects != null)
            {
                _listOrmObjects.AddRange(listOrmObjects);
            }

            ShowSearchDataGrid();
        }

        private void SearchFultextUserFolderStructure()
        {
            IList<UserFoldersStructure> listUFS = CgpClient.Singleton.MainServerProvider.GetFolderStructureSearchResult(_eNameParams.Text);
            IList<AOrmObject> listOrmObjects = new List<AOrmObject>();

            if (listUFS != null)
            {
                foreach (UserFoldersStructure ufs in listUFS)
                {
                    UserFoldersStructure outUfs = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectById(ufs.IdUserFoldersStructure);
                    listOrmObjects.Add(outUfs as AOrmObject);
                    foreach (UserFoldersStructureObject ufsObj in outUfs.UserFoldersStructureObjects)
                    {
                        AOrmObject ormObject = DbsSupport.GetTableObject(ufsObj.ObjectType, ufsObj.ObjectId);
                        if (ormObject != null)
                        {
                            listOrmObjects.Add(ormObject);
                        }
                    }
                }
            }

            _listOrmObjects.Clear();
            if (listOrmObjects != null)
            {
                foreach (AOrmObject ormObj in listOrmObjects)
                {
                    _listOrmObjects.Add(ormObj);
                }
            }
            ShowSearchDataGrid();
        }

        private bool ControlBeforeFullTextSearch()
        {
            if (_eNameParams.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNameParams,
                    GetString("ErrorEnterSearchSring"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _eNameParams.Focus();
                return false;
            }
            return true;
        }

        private void ShowSearchDataGrid()
        {
            _bindingSourceOrmObjects = new BindingSource();
            _bindingSourceOrmObjects.DataSource = _listOrmObjects;
            _bindingSourceOrmObjects.AllowNew = false;

            _cdgvData.DataGrid.DataSource = _bindingSourceOrmObjects;
            if (!_cdgvData.DataGrid.Columns.Contains("ObjectType"))
            {
                DataGridViewImageColumn dgvic = new DataGridViewImageColumn(false);
                dgvic.Name = "ObjectType";
                dgvic.HeaderText = "Type";
                dgvic.ImageLayout = DataGridViewImageCellLayout.Normal;
                dgvic.Resizable = DataGridViewTriState.True;
                //dgvic.in
                dgvic.Width = 32;
                _cdgvData.DataGrid.Columns.Add(dgvic);
            }
            if (!_cdgvData.DataGrid.Columns.Contains("Object"))
            {
                _cdgvData.DataGrid.Columns.Add("Object", "Object");
            }
            if (!_cdgvData.DataGrid.Columns.Contains("Description"))
            {
                _cdgvData.DataGrid.Columns.Add("Description", "Description");
            }

            _cdgvData.DataGrid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //_dgSearchResult.RowTemplate.Height = 32;
            //_dgSearchResult.Columns["ObjectType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //_dgSearchResult.Sort(_dgSearchResult.Columns["Object"], ListSortDirection.Ascending);
            //_dgSearchResult.Columns["Object"].SortMode = DataGridViewColumnSortMode.Automatic;
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);
            _cdgvData.DataGrid.AutoResizeColumns();
        }

        private void _dgSearchResult_DoubleClick(object sender, EventArgs e)
        {
            AOrmObject ormObj = (AOrmObject)_bindingSourceOrmObjects.List[_bindingSourceOrmObjects.Position];
            DbsSupport.OpenEditForm(ormObj);
        }

        private void ObjectsSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall ||
                e.CloseReason != CloseReason.FormOwnerClosing ||
                e.CloseReason != CloseReason.WindowsShutDown ||
                e.CloseReason != CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void ObjectsSearchForm_Activated(object sender, EventArgs e)
        {
            Contal.IwQuick.Threads.SafeThread.StartThread(CancelTopMost);
        }

        private string GetStringFromPlugin(string typeName, string pluginName)
        {
            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                if (plugin._plugin.FriendlyName == pluginName)
                {
                    ICgpVisualPlugin visualPlugin = plugin._plugin as ICgpVisualPlugin;

                    if (visualPlugin != null)
                    {
                        string translateText = visualPlugin.GetTranslateString(plugin._plugin.Description + typeName + "Form" + plugin._plugin.Description + typeName + "Form");
                        if (translateText != string.Empty)
                            return translateText;
                    }
                }
            }

            return string.Empty;
        }

        class SearchTypeObj
        {
            private ObjectType _objectType;
            private string _localizeName;

            public string LocalizeName
            {
                get { return _localizeName; }
            }

            public ObjectType ObjectType
            {
                get { return _objectType; }
            }

            public SearchTypeObj(ObjectType objectType, string localizeName)
            {
                _objectType = objectType;
                _localizeName = localizeName;
            }

            public override string ToString()
            {
                return _localizeName;
            }
        }

        private void FillComboBoxSelectSearch()
        {
            _cbSearchType.Items.Clear();
            foreach (SearchType val in Enum.GetValues(typeof(SearchType)))
            {
                DatabaseSearchType dst = new DatabaseSearchType(val, GetString("Search_" + val.ToString()));
                _cbSearchType.Items.Add(dst);
            }

            if (_cbSearchType.Items.Count > 1)
            {
                _cbSearchType.SelectedIndex = 0;
            }
        }

        private void _cbSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbSearchType.SelectedItem is DatabaseSearchType)
            {
                if (((DatabaseSearchType)_cbSearchType.SelectedItem).SearchType == SearchType.ParametricalSearchByType)
                {
                    _cbObjectType.Enabled = true;
                }
                else
                {
                    _cbObjectType.Enabled = false;
                }
            }
        }
    }

    enum SearchType
    {
        Fulltext = 1,
        ParametricalSearchByType = 2,
        FulltextSearchByFolderStructure = 3,
    }

    class DatabaseSearchType
    {
        SearchType _searchType;
        string _name;

        public SearchType SearchType
        {
            get { return _searchType; }
        }

        public DatabaseSearchType(SearchType searchType, string name)
        {
            _searchType = searchType;
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
