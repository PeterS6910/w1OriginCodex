using Contal.IwQuick.PlatformPC.UI;

namespace Contal.Cgp.Client
{
    partial class EventlogsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventlogsForm));
            this._pControl = new System.Windows.Forms.Panel();
            this._bExportCsv = new System.Windows.Forms.Button();
            this._bExportExcel = new System.Windows.Forms.Button();
            this._bStop = new System.Windows.Forms.Button();
            this._bParameters = new System.Windows.Forms.Button();
            this._bRefresh = new System.Windows.Forms.Button();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._pFilter = new System.Windows.Forms.Panel();
            this._bUnselectAll = new System.Windows.Forms.Button();
            this._bSelectAll = new System.Windows.Forms.Button();
            this._chbCheckUncheckOnlyVisibleItems = new System.Windows.Forms.CheckBox();
            this._tbSearchEventType = new System.Windows.Forms.TextBox();
            this._bToDay = new System.Windows.Forms.Button();
            this._chblType = new System.Windows.Forms.CheckedListBox();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._tcSearch = new System.Windows.Forms.TabControl();
            this._tpParametricSearch = new System.Windows.Forms.TabPage();
            this._rbParametricSearchOr = new System.Windows.Forms.RadioButton();
            this._rbParametricSearchAnd = new System.Windows.Forms.RadioButton();
            this._bClearList = new System.Windows.Forms.Button();
            this._bRemove = new System.Windows.Forms.Button();
            this._bAdd = new System.Windows.Forms.Button();
            this._ilbEventSourceObject = new Contal.IwQuick.UI.ImageListBox();
            this._tpFulltextSearch = new System.Windows.Forms.TabPage();
            this._rbFullTextSearchOr = new System.Windows.Forms.RadioButton();
            this._rbFullTextSearchAnd = new System.Windows.Forms.RadioButton();
            this._lCGPSourceFilter = new System.Windows.Forms.Label();
            this._eCGPSourceFilter = new System.Windows.Forms.TextBox();
            this._eEventSourceFilter = new System.Windows.Forms.TextBox();
            this._lEventSourceFilter = new System.Windows.Forms.Label();
            this._eDescriptionFilter = new System.Windows.Forms.TextBox();
            this._lDescription = new System.Windows.Forms.Label();
            this._tpSubSitesFilter = new System.Windows.Forms.TabPage();
            this._cbAllSubSites = new System.Windows.Forms.CheckBox();
            this._tbdpDateToFilter = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tbdpDateFromFilter = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._bFilterDateOneDay = new System.Windows.Forms.Button();
            this._lDateToFilter = new System.Windows.Forms.Label();
            this._lDateFromFilter = new System.Windows.Forms.Label();
            this._lTypeFilter = new System.Windows.Forms.Label();
            this._pbLoading = new System.Windows.Forms.ProgressBar();
            this._pDGVBack = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._ehWpfCheckBoxTreeView = new System.Windows.Forms.Integration.ElementHost();
            this._wpfCheckBoxTreeView = new Contal.IwQuick.PlatformPC.UI.WpfCheckBoxesTreeview();
            this._pControl.SuspendLayout();
            this._pFilter.SuspendLayout();
            this._tcSearch.SuspendLayout();
            this._tpParametricSearch.SuspendLayout();
            this._tpFulltextSearch.SuspendLayout();
            this._tpSubSitesFilter.SuspendLayout();
            this._pDGVBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._bExportCsv);
            this._pControl.Controls.Add(this._bExportExcel);
            this._pControl.Controls.Add(this._bStop);
            this._pControl.Controls.Add(this._bParameters);
            this._pControl.Controls.Add(this._bRefresh);
            this._pControl.Controls.Add(this._bFilterClear);
            this._pControl.Controls.Add(this._bRunFilter);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 509);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(924, 37);
            this._pControl.TabIndex = 1;
            // 
            // _bExport
            // 
            this._bExportCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bExportCsv.Location = new System.Drawing.Point(407, 6);
            this._bExportCsv.Name = "_bExportCsv";
            this._bExportCsv.Size = new System.Drawing.Size(96, 23);
            this._bExportCsv.TabIndex = 13;
            this._bExportCsv.Text = "Export to CSV/TSV";
            this._bExportCsv.UseVisualStyleBackColor = true;
            this._bExportCsv.Click += new System.EventHandler(this._bExportCsv_Click);
            // 
            // _bExportExcel
            // 
            this._bExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bExportExcel.Location = new System.Drawing.Point(300, 6);
            this._bExportExcel.Name = "_bExportExcel";
            this._bExportExcel.Size = new System.Drawing.Size(96, 23);
            this._bExportExcel.TabIndex = 12;
            this._bExportExcel.Text = "Export to Excel";
            this._bExportExcel.UseVisualStyleBackColor = true;
            this._bExportExcel.Click += new System.EventHandler(this._bExportExcel_Click);
            // 
            // _bStop
            // 
            this._bStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bStop.Location = new System.Drawing.Point(509, 6);
            this._bStop.Name = "_bStop";
            this._bStop.Size = new System.Drawing.Size(96, 23);
            this._bStop.TabIndex = 11;
            this._bStop.Text = "Stop";
            this._bStop.UseVisualStyleBackColor = true;
            this._bStop.Click += new System.EventHandler(this._bStop_Click);
            // 
            // _bParameters
            // 
            this._bParameters.Location = new System.Drawing.Point(10, 6);
            this._bParameters.Name = "_bParameters";
            this._bParameters.Size = new System.Drawing.Size(96, 23);
            this._bParameters.TabIndex = 10;
            this._bParameters.Text = "Parameters";
            this._bParameters.UseVisualStyleBackColor = true;
            this._bParameters.Click += new System.EventHandler(this._bParameters_Click);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(816, 6);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(96, 23);
            this._bRefresh.TabIndex = 9;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bFilterClear.Location = new System.Drawing.Point(714, 6);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(96, 23);
            this._bFilterClear.TabIndex = 8;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRunFilter.Location = new System.Drawing.Point(611, 6);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(96, 23);
            this._bRunFilter.TabIndex = 7;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._bUnselectAll);
            this._pFilter.Controls.Add(this._bSelectAll);
            this._pFilter.Controls.Add(this._chbCheckUncheckOnlyVisibleItems);
            this._pFilter.Controls.Add(this._tbSearchEventType);
            this._pFilter.Controls.Add(this._bToDay);
            this._pFilter.Controls.Add(this._chblType);
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._tcSearch);
            this._pFilter.Controls.Add(this._tbdpDateToFilter);
            this._pFilter.Controls.Add(this._tbdpDateFromFilter);
            this._pFilter.Controls.Add(this._bFilterDateOneDay);
            this._pFilter.Controls.Add(this._lDateToFilter);
            this._pFilter.Controls.Add(this._lDateFromFilter);
            this._pFilter.Controls.Add(this._lTypeFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 310);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(924, 199);
            this._pFilter.TabIndex = 8;
            // 
            // _bUnselectAll
            // 
            this._bUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bUnselectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bUnselectAll.Location = new System.Drawing.Point(103, 173);
            this._bUnselectAll.Name = "_bUnselectAll";
            this._bUnselectAll.Size = new System.Drawing.Size(93, 23);
            this._bUnselectAll.TabIndex = 24;
            this._bUnselectAll.Text = "UnselectAll";
            this._bUnselectAll.UseVisualStyleBackColor = true;
            this._bUnselectAll.Click += new System.EventHandler(this._bUnselectAll_Click);
            // 
            // _bSelectAll
            // 
            this._bSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSelectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bSelectAll.Location = new System.Drawing.Point(4, 173);
            this._bSelectAll.Name = "_bSelectAll";
            this._bSelectAll.Size = new System.Drawing.Size(93, 23);
            this._bSelectAll.TabIndex = 23;
            this._bSelectAll.Text = "SelectAll";
            this._bSelectAll.UseVisualStyleBackColor = true;
            this._bSelectAll.Click += new System.EventHandler(this._bSelectAll_Click);
            // 
            // _chbCheckUncheckOnlyVisibleItems
            // 
            this._chbCheckUncheckOnlyVisibleItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._chbCheckUncheckOnlyVisibleItems.AutoSize = true;
            this._chbCheckUncheckOnlyVisibleItems.Location = new System.Drawing.Point(202, 177);
            this._chbCheckUncheckOnlyVisibleItems.Name = "_chbCheckUncheckOnlyVisibleItems";
            this._chbCheckUncheckOnlyVisibleItems.Size = new System.Drawing.Size(177, 17);
            this._chbCheckUncheckOnlyVisibleItems.TabIndex = 22;
            this._chbCheckUncheckOnlyVisibleItems.Text = "CheckUncheckOnlyVisibleItems";
            this._chbCheckUncheckOnlyVisibleItems.UseVisualStyleBackColor = true;
            // 
            // _tbSearchEventType
            // 
            this._tbSearchEventType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._tbSearchEventType.Location = new System.Drawing.Point(3, 22);
            this._tbSearchEventType.Name = "_tbSearchEventType";
            this._tbSearchEventType.Size = new System.Drawing.Size(301, 20);
            this._tbSearchEventType.TabIndex = 21;
            this._tbSearchEventType.TextChanged += new System.EventHandler(this._tbSearchEventType_TextChanged);
            // 
            // _bToDay
            // 
            this._bToDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bToDay.Location = new System.Drawing.Point(375, 99);
            this._bToDay.Name = "_bToDay";
            this._bToDay.Size = new System.Drawing.Size(60, 23);
            this._bToDay.TabIndex = 20;
            this._bToDay.Text = "To day";
            this._bToDay.UseVisualStyleBackColor = true;
            this._bToDay.Click += new System.EventHandler(this._bToDay_Click);
            // 
            // _chblType
            // 
            this._chblType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._chblType.FormattingEnabled = true;
            this._chblType.Location = new System.Drawing.Point(4, 46);
            this._chblType.Name = "_chblType";
            this._chblType.Size = new System.Drawing.Size(301, 124);
            this._chblType.TabIndex = 18;
            this._chblType.MouseClick += new System.Windows.Forms.MouseEventHandler(this._chblType_MouseClick);
            this._chblType.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._chblType_ItemCheck);
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(834, 8);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(72, 13);
            this._lRecordCount.TabIndex = 11;
            this._lRecordCount.Text = "Record count";
            this._lRecordCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._lRecordCount.Visible = false;
            // 
            // _tcSearch
            // 
            this._tcSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcSearch.Controls.Add(this._tpParametricSearch);
            this._tcSearch.Controls.Add(this._tpFulltextSearch);
            this._tcSearch.Controls.Add(this._tpSubSitesFilter);
            this._tcSearch.Location = new System.Drawing.Point(526, 3);
            this._tcSearch.Name = "_tcSearch";
            this._tcSearch.SelectedIndex = 0;
            this._tcSearch.Size = new System.Drawing.Size(395, 190);
            this._tcSearch.TabIndex = 17;
            this._tcSearch.SelectedIndexChanged += new System.EventHandler(this._tcSearch_SelectedIndexChanged);
            // 
            // _tpParametricSearch
            // 
            this._tpParametricSearch.BackColor = System.Drawing.SystemColors.Control;
            this._tpParametricSearch.Controls.Add(this._rbParametricSearchOr);
            this._tpParametricSearch.Controls.Add(this._rbParametricSearchAnd);
            this._tpParametricSearch.Controls.Add(this._bClearList);
            this._tpParametricSearch.Controls.Add(this._bRemove);
            this._tpParametricSearch.Controls.Add(this._bAdd);
            this._tpParametricSearch.Controls.Add(this._ilbEventSourceObject);
            this._tpParametricSearch.Location = new System.Drawing.Point(4, 22);
            this._tpParametricSearch.Name = "_tpParametricSearch";
            this._tpParametricSearch.Padding = new System.Windows.Forms.Padding(3);
            this._tpParametricSearch.Size = new System.Drawing.Size(387, 164);
            this._tpParametricSearch.TabIndex = 0;
            this._tpParametricSearch.Text = "Parametric search";
            // 
            // _rbParametricSearchOr
            // 
            this._rbParametricSearchOr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbParametricSearchOr.AutoSize = true;
            this._rbParametricSearchOr.Location = new System.Drawing.Point(6, 125);
            this._rbParametricSearchOr.Name = "_rbParametricSearchOr";
            this._rbParametricSearchOr.Size = new System.Drawing.Size(277, 17);
            this._rbParametricSearchOr.TabIndex = 5;
            this._rbParametricSearchOr.TabStop = true;
            this._rbParametricSearchOr.Text = "Show eventlogs that match at least one event source";
            this._rbParametricSearchOr.UseVisualStyleBackColor = true;
            this._rbParametricSearchOr.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _rbParametricSearchAnd
            // 
            this._rbParametricSearchAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbParametricSearchAnd.AutoSize = true;
            this._rbParametricSearchAnd.Location = new System.Drawing.Point(6, 144);
            this._rbParametricSearchAnd.Name = "_rbParametricSearchAnd";
            this._rbParametricSearchAnd.Size = new System.Drawing.Size(237, 17);
            this._rbParametricSearchAnd.TabIndex = 4;
            this._rbParametricSearchAnd.TabStop = true;
            this._rbParametricSearchAnd.Text = "Show eventlogs that match all event sources";
            this._rbParametricSearchAnd.UseVisualStyleBackColor = true;
            this._rbParametricSearchAnd.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _bClearList
            // 
            this._bClearList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClearList.Location = new System.Drawing.Point(306, 83);
            this._bClearList.Name = "_bClearList";
            this._bClearList.Size = new System.Drawing.Size(75, 36);
            this._bClearList.TabIndex = 3;
            this._bClearList.Text = "Clear list";
            this._bClearList.UseVisualStyleBackColor = true;
            this._bClearList.Click += new System.EventHandler(this._bClearList_Click);
            // 
            // _bRemove
            // 
            this._bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRemove.Location = new System.Drawing.Point(306, 35);
            this._bRemove.Name = "_bRemove";
            this._bRemove.Size = new System.Drawing.Size(75, 23);
            this._bRemove.TabIndex = 2;
            this._bRemove.Text = "Remove";
            this._bRemove.UseVisualStyleBackColor = true;
            this._bRemove.Click += new System.EventHandler(this._bRemove_Click);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(306, 6);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(75, 23);
            this._bAdd.TabIndex = 1;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _ilbEventSourceObject
            // 
            this._ilbEventSourceObject.AllowDrop = true;
            this._ilbEventSourceObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ilbEventSourceObject.BackColor = System.Drawing.SystemColors.Info;
            this._ilbEventSourceObject.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbEventSourceObject.FormattingEnabled = true;
            this._ilbEventSourceObject.ImageList = null;
            this._ilbEventSourceObject.Location = new System.Drawing.Point(6, 6);
            this._ilbEventSourceObject.Name = "_ilbEventSourceObject";
            this._ilbEventSourceObject.SelectedItemObject = null;
            this._ilbEventSourceObject.Size = new System.Drawing.Size(294, 108);
            this._ilbEventSourceObject.TabIndex = 0;
            this._ilbEventSourceObject.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._ilbEventSourceObject_MouseDoubleClick);
            this._ilbEventSourceObject.DragOver += new System.Windows.Forms.DragEventHandler(this._ilbEventSourceObject_DragOver);
            this._ilbEventSourceObject.DragDrop += new System.Windows.Forms.DragEventHandler(this._ilbEventSourceObject_DragDrop);
            this._ilbEventSourceObject.MouseDown += new System.Windows.Forms.MouseEventHandler(this._ilbEventSourceObject_MouseDown);
            // 
            // _tpFulltextSearch
            // 
            this._tpFulltextSearch.BackColor = System.Drawing.SystemColors.Control;
            this._tpFulltextSearch.Controls.Add(this._rbFullTextSearchOr);
            this._tpFulltextSearch.Controls.Add(this._rbFullTextSearchAnd);
            this._tpFulltextSearch.Controls.Add(this._lCGPSourceFilter);
            this._tpFulltextSearch.Controls.Add(this._eCGPSourceFilter);
            this._tpFulltextSearch.Controls.Add(this._eEventSourceFilter);
            this._tpFulltextSearch.Controls.Add(this._lEventSourceFilter);
            this._tpFulltextSearch.Controls.Add(this._eDescriptionFilter);
            this._tpFulltextSearch.Controls.Add(this._lDescription);
            this._tpFulltextSearch.Location = new System.Drawing.Point(4, 22);
            this._tpFulltextSearch.Name = "_tpFulltextSearch";
            this._tpFulltextSearch.Padding = new System.Windows.Forms.Padding(3);
            this._tpFulltextSearch.Size = new System.Drawing.Size(387, 164);
            this._tpFulltextSearch.TabIndex = 1;
            this._tpFulltextSearch.Text = "Fulltext search";
            // 
            // _rbFullTextSearchOr
            // 
            this._rbFullTextSearchOr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFullTextSearchOr.AutoSize = true;
            this._rbFullTextSearchOr.Location = new System.Drawing.Point(6, 48);
            this._rbFullTextSearchOr.Name = "_rbFullTextSearchOr";
            this._rbFullTextSearchOr.Size = new System.Drawing.Size(273, 17);
            this._rbFullTextSearchOr.TabIndex = 14;
            this._rbFullTextSearchOr.TabStop = true;
            this._rbFullTextSearchOr.Text = "Show eventlogs that meets at least one filter criterion";
            this._rbFullTextSearchOr.UseVisualStyleBackColor = true;
            this._rbFullTextSearchOr.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _rbFullTextSearchAnd
            // 
            this._rbFullTextSearchAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFullTextSearchAnd.AutoSize = true;
            this._rbFullTextSearchAnd.Location = new System.Drawing.Point(312, 48);
            this._rbFullTextSearchAnd.Name = "_rbFullTextSearchAnd";
            this._rbFullTextSearchAnd.Size = new System.Drawing.Size(244, 17);
            this._rbFullTextSearchAnd.TabIndex = 13;
            this._rbFullTextSearchAnd.TabStop = true;
            this._rbFullTextSearchAnd.Text = "Show eventlogs that meets both filter criterions";
            this._rbFullTextSearchAnd.UseVisualStyleBackColor = true;
            this._rbFullTextSearchAnd.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _lCGPSourceFilter
            // 
            this._lCGPSourceFilter.AutoSize = true;
            this._lCGPSourceFilter.Location = new System.Drawing.Point(3, 73);
            this._lCGPSourceFilter.Name = "_lCGPSourceFilter";
            this._lCGPSourceFilter.Size = new System.Drawing.Size(64, 13);
            this._lCGPSourceFilter.TabIndex = 2;
            this._lCGPSourceFilter.Text = "CGP source";
            // 
            // _eCGPSourceFilter
            // 
            this._eCGPSourceFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eCGPSourceFilter.Location = new System.Drawing.Point(6, 89);
            this._eCGPSourceFilter.Name = "_eCGPSourceFilter";
            this._eCGPSourceFilter.Size = new System.Drawing.Size(591, 20);
            this._eCGPSourceFilter.TabIndex = 4;
            this._eCGPSourceFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _eEventSourceFilter
            // 
            this._eEventSourceFilter.Location = new System.Drawing.Point(6, 22);
            this._eEventSourceFilter.Name = "_eEventSourceFilter";
            this._eEventSourceFilter.Size = new System.Drawing.Size(273, 20);
            this._eEventSourceFilter.TabIndex = 5;
            this._eEventSourceFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _lEventSourceFilter
            // 
            this._lEventSourceFilter.AutoSize = true;
            this._lEventSourceFilter.Location = new System.Drawing.Point(3, 6);
            this._lEventSourceFilter.Name = "_lEventSourceFilter";
            this._lEventSourceFilter.Size = new System.Drawing.Size(70, 13);
            this._lEventSourceFilter.TabIndex = 6;
            this._lEventSourceFilter.Text = "Event source";
            // 
            // _eDescriptionFilter
            // 
            this._eDescriptionFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescriptionFilter.Location = new System.Drawing.Point(285, 22);
            this._eDescriptionFilter.Name = "_eDescriptionFilter";
            this._eDescriptionFilter.Size = new System.Drawing.Size(312, 20);
            this._eDescriptionFilter.TabIndex = 6;
            this._eDescriptionFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _lDescription
            // 
            this._lDescription.AutoSize = true;
            this._lDescription.Location = new System.Drawing.Point(282, 6);
            this._lDescription.Name = "_lDescription";
            this._lDescription.Size = new System.Drawing.Size(60, 13);
            this._lDescription.TabIndex = 12;
            this._lDescription.Text = "Description";
            // 
            // _tpSubSitesFilter
            // 
            this._tpSubSitesFilter.BackColor = System.Drawing.SystemColors.Control;
            this._tpSubSitesFilter.Controls.Add(this._ehWpfCheckBoxTreeView);
            this._tpSubSitesFilter.Controls.Add(this._cbAllSubSites);
            this._tpSubSitesFilter.Location = new System.Drawing.Point(4, 22);
            this._tpSubSitesFilter.Name = "_tpSubSitesFilter";
            this._tpSubSitesFilter.Size = new System.Drawing.Size(387, 164);
            this._tpSubSitesFilter.TabIndex = 2;
            this._tpSubSitesFilter.Text = "Filter for sub-sites";
            // 
            // _cbAllSubSites
            // 
            this._cbAllSubSites.AutoSize = true;
            this._cbAllSubSites.Location = new System.Drawing.Point(3, 126);
            this._cbAllSubSites.Name = "_cbAllSubSites";
            this._cbAllSubSites.Size = new System.Drawing.Size(113, 17);
            this._cbAllSubSites.TabIndex = 0;
            this._cbAllSubSites.Text = "Select all sub-sites";
            this._cbAllSubSites.UseVisualStyleBackColor = true;
            // 
            // _tbdpDateToFilter
            // 
            this._tbdpDateToFilter.addActualTime = false;
            this._tbdpDateToFilter.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateToFilter.ButtonClearDateImage = null;
            this._tbdpDateToFilter.ButtonClearDateText = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbdpDateToFilter.ButtonClearDateWidth = 23;
            this._tbdpDateToFilter.ButtonDateImage = null;
            this._tbdpDateToFilter.ButtonDateText = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbdpDateToFilter.ButtonDateWidth = 23;
            this._tbdpDateToFilter.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpDateToFilter.DateFormName = "Calendar";
            this._tbdpDateToFilter.LocalizationHelper = null;
            this._tbdpDateToFilter.Location = new System.Drawing.Point(309, 65);
            this._tbdpDateToFilter.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateToFilter.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateToFilter.Name = "_tbdpDateToFilter";
            this._tbdpDateToFilter.ReadOnly = false;
            this._tbdpDateToFilter.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.EndOfDay;
            this._tbdpDateToFilter.Size = new System.Drawing.Size(211, 22);
            this._tbdpDateToFilter.TabIndex = 2;
            this._tbdpDateToFilter.ValidateAfter = 2;
            this._tbdpDateToFilter.ValidationEnabled = false;
            this._tbdpDateToFilter.ValidationError = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbdpDateToFilter.Value = null;
            this._tbdpDateToFilter.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.FilterValueChanged);
            // 
            // _tbdpDateFromFilter
            // 
            this._tbdpDateFromFilter.addActualTime = false;
            this._tbdpDateFromFilter.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateFromFilter.ButtonClearDateImage = null;
            this._tbdpDateFromFilter.ButtonClearDateText = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbdpDateFromFilter.ButtonClearDateWidth = 23;
            this._tbdpDateFromFilter.ButtonDateImage = null;
            this._tbdpDateFromFilter.ButtonDateText = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbdpDateFromFilter.ButtonDateWidth = 23;
            this._tbdpDateFromFilter.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpDateFromFilter.DateFormName = "Calendar";
            this._tbdpDateFromFilter.LocalizationHelper = null;
            this._tbdpDateFromFilter.Location = new System.Drawing.Point(310, 21);
            this._tbdpDateFromFilter.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateFromFilter.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateFromFilter.Name = "_tbdpDateFromFilter";
            this._tbdpDateFromFilter.ReadOnly = false;
            this._tbdpDateFromFilter.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Delta;
            this._tbdpDateFromFilter.Size = new System.Drawing.Size(211, 22);
            this._tbdpDateFromFilter.TabIndex = 1;
            this._tbdpDateFromFilter.ValidateAfter = 2;
            this._tbdpDateFromFilter.ValidationEnabled = false;
            this._tbdpDateFromFilter.ValidationError = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbdpDateFromFilter.Value = null;
            this._tbdpDateFromFilter.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.FilterValueChanged);
            // 
            // _bFilterDateOneDay
            // 
            this._bFilterDateOneDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bFilterDateOneDay.Location = new System.Drawing.Point(309, 99);
            this._bFilterDateOneDay.Name = "_bFilterDateOneDay";
            this._bFilterDateOneDay.Size = new System.Drawing.Size(60, 23);
            this._bFilterDateOneDay.TabIndex = 3;
            this._bFilterDateOneDay.Text = "One day";
            this._bFilterDateOneDay.UseVisualStyleBackColor = true;
            this._bFilterDateOneDay.Click += new System.EventHandler(this._bFilterDateOneDay_Click);
            // 
            // _lDateToFilter
            // 
            this._lDateToFilter.AutoSize = true;
            this._lDateToFilter.Location = new System.Drawing.Point(306, 49);
            this._lDateToFilter.Name = "_lDateToFilter";
            this._lDateToFilter.Size = new System.Drawing.Size(42, 13);
            this._lDateToFilter.TabIndex = 15;
            this._lDateToFilter.Text = "Date to";
            // 
            // _lDateFromFilter
            // 
            this._lDateFromFilter.AutoSize = true;
            this._lDateFromFilter.Location = new System.Drawing.Point(306, 3);
            this._lDateFromFilter.Name = "_lDateFromFilter";
            this._lDateFromFilter.Size = new System.Drawing.Size(53, 13);
            this._lDateFromFilter.TabIndex = 4;
            this._lDateFromFilter.Text = "Date from";
            // 
            // _lTypeFilter
            // 
            this._lTypeFilter.AutoSize = true;
            this._lTypeFilter.Location = new System.Drawing.Point(0, 3);
            this._lTypeFilter.Name = "_lTypeFilter";
            this._lTypeFilter.Size = new System.Drawing.Size(31, 13);
            this._lTypeFilter.TabIndex = 0;
            this._lTypeFilter.Text = "Type";
            // 
            // _pbLoading
            // 
            this._pbLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._pbLoading.Location = new System.Drawing.Point(309, 155);
            this._pbLoading.MarqueeAnimationSpeed = 10;
            this._pbLoading.MaximumSize = new System.Drawing.Size(300, 20);
            this._pbLoading.Name = "_pbLoading";
            this._pbLoading.Size = new System.Drawing.Size(300, 20);
            this._pbLoading.Step = 1;
            this._pbLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this._pbLoading.TabIndex = 10;
            this._pbLoading.Visible = false;
            // 
            // _pDGVBack
            // 
            this._pDGVBack.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this._pDGVBack.Controls.Add(this._pbLoading);
            this._pDGVBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pDGVBack.Location = new System.Drawing.Point(0, 0);
            this._pDGVBack.Name = "_pDGVBack";
            this._pDGVBack.Size = new System.Drawing.Size(924, 310);
            this._pDGVBack.TabIndex = 11;
            this._pDGVBack.Resize += new System.EventHandler(this._pDGVBack_Resize);
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(924, 310);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DataGrid.TabStop = false;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(924, 310);
            this._cdgvData.TabIndex = 11;
            // 
            // _ehWpfCheckBoxTreeView
            // 
            this._ehWpfCheckBoxTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ehWpfCheckBoxTreeView.Location = new System.Drawing.Point(3, 3);
            this._ehWpfCheckBoxTreeView.Name = "_ehWpfCheckBoxTreeView";
            this._ehWpfCheckBoxTreeView.Size = new System.Drawing.Size(379, 117);
            this._ehWpfCheckBoxTreeView.TabIndex = 1;
            this._ehWpfCheckBoxTreeView.Text = "_ehWpfCheckBoxTreeView";
            this._ehWpfCheckBoxTreeView.Child = this._wpfCheckBoxTreeView;
            // 
            // EventlogsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 546);
            this.Controls.Add(this._pDGVBack);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Controls.Add(this._pControl);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EventlogsForm";
            this.Text = "EventlogForm";
            this.Load += new System.EventHandler(this.EventlogsForm_Load);
            this._pControl.ResumeLayout(false);
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._tcSearch.ResumeLayout(false);
            this._tpParametricSearch.ResumeLayout(false);
            this._tpParametricSearch.PerformLayout();
            this._tpFulltextSearch.ResumeLayout(false);
            this._tpFulltextSearch.PerformLayout();
            this._tpSubSitesFilter.ResumeLayout(false);
            this._tpSubSitesFilter.PerformLayout();
            this._pDGVBack.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Button _bParameters;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lEventSourceFilter;
        private System.Windows.Forms.Label _lDateFromFilter;
        private System.Windows.Forms.Label _lCGPSourceFilter;
        private System.Windows.Forms.Label _lTypeFilter;
        private System.Windows.Forms.TextBox _eEventSourceFilter;
        private System.Windows.Forms.TextBox _eCGPSourceFilter;
        private System.Windows.Forms.Label _lDescription;
        private System.Windows.Forms.TextBox _eDescriptionFilter;
        private System.Windows.Forms.Label _lDateToFilter;
        private System.Windows.Forms.Button _bFilterDateOneDay;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateToFilter;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateFromFilter;
        private System.Windows.Forms.TabControl _tcSearch;
        private System.Windows.Forms.TabPage _tpParametricSearch;
        private System.Windows.Forms.TabPage _tpFulltextSearch;
        private Contal.IwQuick.UI.ImageListBox _ilbEventSourceObject;
        private System.Windows.Forms.Button _bRemove;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.Button _bClearList;
        private System.Windows.Forms.Label _lRecordCount;
        private System.Windows.Forms.RadioButton _rbParametricSearchOr;
        private System.Windows.Forms.RadioButton _rbParametricSearchAnd;
        private System.Windows.Forms.Button _bStop;
        private System.Windows.Forms.RadioButton _rbFullTextSearchOr;
        private System.Windows.Forms.RadioButton _rbFullTextSearchAnd;
        private System.Windows.Forms.Button _bExportCsv;
        private System.Windows.Forms.Button _bExportExcel;
        private System.Windows.Forms.ProgressBar _pbLoading;
        private System.Windows.Forms.Panel _pDGVBack;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.CheckedListBox _chblType;
        private System.Windows.Forms.Button _bToDay;
        private System.Windows.Forms.TabPage _tpSubSitesFilter;
        private System.Windows.Forms.CheckBox _cbAllSubSites;
        private System.Windows.Forms.TextBox _tbSearchEventType;
        private System.Windows.Forms.CheckBox _chbCheckUncheckOnlyVisibleItems;
        private System.Windows.Forms.Button _bUnselectAll;
        private System.Windows.Forms.Button _bSelectAll;
        private System.Windows.Forms.Integration.ElementHost _ehWpfCheckBoxTreeView;
        private WpfCheckBoxesTreeview _wpfCheckBoxTreeView;
    }
}