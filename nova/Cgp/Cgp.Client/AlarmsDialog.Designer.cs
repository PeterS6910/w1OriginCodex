using System.Windows.Forms;
using Contal.IwQuick.PlatformPC.UI;

namespace Contal.Cgp.Client
{
    partial class AlarmsDialog
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmsDialog));
            this._bCancel = new System.Windows.Forms.Button();
            this._alarmMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._tsAcknowledgeAndBlock = new System.Windows.Forms.ToolStripMenuItem();
            this._tsAcknowledge = new System.Windows.Forms.ToolStripMenuItem();
            this._tsBlock = new System.Windows.Forms.ToolStripMenuItem();
            this._tsUnblock = new System.Windows.Forms.ToolStripMenuItem();
            this._tsLoading = new System.Windows.Forms.ToolStripLabel();
            this._bAcknowledgeState = new System.Windows.Forms.Button();
            this._bViewDetails = new System.Windows.Forms.Button();
            this._gbAlarmsCount = new System.Windows.Forms.GroupBox();
            this._bDefaultSort = new System.Windows.Forms.Button();
            this._bDockUndock = new System.Windows.Forms.Button();
            this._lNormalNotAcknowledgedCount = new System.Windows.Forms.Label();
            this._lNormalNotAcknowledged = new System.Windows.Forms.Label();
            this._lAlarmCount = new System.Windows.Forms.Label();
            this._lAlarm = new System.Windows.Forms.Label();
            this._lAlarmNotAcknowladgedCount = new System.Windows.Forms.Label();
            this._lAlarmNotAcknowledged = new System.Windows.Forms.Label();
            this._gbFilter = new System.Windows.Forms.GroupBox();
            this._chbAndAbove = new System.Windows.Forms.CheckBox();
            this._tcSearch = new System.Windows.Forms.TabControl();
            this._tpParametricSearch = new System.Windows.Forms.TabPage();
            this._rbParametricSearchOr = new System.Windows.Forms.RadioButton();
            this._rbParametricSearchAnd = new System.Windows.Forms.RadioButton();
            this._bClearList = new System.Windows.Forms.Button();
            this._bRemove = new System.Windows.Forms.Button();
            this._bAdd = new System.Windows.Forms.Button();
            this._ilbEventSourceObject = new Contal.IwQuick.UI.ImageListBox();
            this._tpSubSitesFilter = new System.Windows.Forms.TabPage();
            this._ehWpfCheckBoxTreeView = new System.Windows.Forms.Integration.ElementHost();
            this._wpfCheckBoxTreeView = new Contal.IwQuick.PlatformPC.UI.WpfCheckBoxesTreeview();
            this._cbAllSubSites = new System.Windows.Forms.CheckBox();
            this._tbdpDateTo = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tbdpDateFrom = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._cbShowOnlyBlockedAlarm = new System.Windows.Forms.CheckBox();
            this._lAlarmPriority = new System.Windows.Forms.Label();
            this._cbFilterAlarmPriority = new System.Windows.Forms.ComboBox();
            this._lDateTo = new System.Windows.Forms.Label();
            this._lDateFrom = new System.Windows.Forms.Label();
            this._lFilteredCount = new System.Windows.Forms.Label();
            this._bFilter = new System.Windows.Forms.Button();
            this._bClear = new System.Windows.Forms.Button();
            this._eFilterText = new System.Windows.Forms.TextBox();
            this._lFilteredText = new System.Windows.Forms.Label();
            this._bBlock = new System.Windows.Forms.Button();
            this._bUnblock = new System.Windows.Forms.Button();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this._llAlarmName = new System.Windows.Forms.LinkLabel();
            this._bHideOrShow = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._dgAlarmInstructions = new System.Windows.Forms.DataGridView();
            this._lAlarmInstruction = new System.Windows.Forms.Label();
            this._bAcknowledgeAndBlock = new System.Windows.Forms.Button();
            this._alarmMenu.SuspendLayout();
            this._gbAlarmsCount.SuspendLayout();
            this._gbFilter.SuspendLayout();
            this._tcSearch.SuspendLayout();
            this._tpParametricSearch.SuspendLayout();
            this._tpSubSitesFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgAlarmInstructions)).BeginInit();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(792, 332);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 11;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _alarmMenu
            // 
            this._alarmMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsLoading,
            this._tsAcknowledge,
            this._tsAcknowledgeAndBlock,
            this._tsBlock,
            this._tsUnblock});
            this._alarmMenu.Name = "_alarmMenu";
            this._alarmMenu.Size = new System.Drawing.Size(198, 114);
            // 
            // _tsAcknowledgeAndBlock
            // 
            this._tsAcknowledgeAndBlock.Name = "_tsAcknowledgeAndBlock";
            this._tsAcknowledgeAndBlock.Size = new System.Drawing.Size(197, 22);
            this._tsAcknowledgeAndBlock.Text = "AcknowledgeAndBlock";
            this._tsAcknowledgeAndBlock.Click += new System.EventHandler(this._tsAcknowledgeAndBlock_Click);
            // 
            // _tsAcknowledge
            // 
            this._tsAcknowledge.Name = "_tsAcknowledge";
            this._tsAcknowledge.Size = new System.Drawing.Size(197, 22);
            this._tsAcknowledge.Text = "Acknowledge";
            this._tsAcknowledge.Click += new System.EventHandler(this._tsAcknowledge_Click);
            // 
            // _tsBlock
            // 
            this._tsBlock.Name = "_tsBlock";
            this._tsBlock.Size = new System.Drawing.Size(197, 22);
            this._tsBlock.Text = "Block";
            this._tsBlock.Click += new System.EventHandler(this._tsBlock_Click);
            // 
            // _tsUnblock
            // 
            this._tsUnblock.Name = "_tsUnblock";
            this._tsUnblock.Size = new System.Drawing.Size(197, 22);
            this._tsUnblock.Text = "Unblock";
            this._tsUnblock.Click += new System.EventHandler(this._tsUnblock_Click);
            // 
            // _tsLoading
            // 
            this._tsLoading.Name = "_tsLoading";
            this._tsLoading.Size = new System.Drawing.Size(197, 22);
            this._tsLoading.Text = "...";
            // 
            // _bAcknowledgeState
            // 
            this._bAcknowledgeState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bAcknowledgeState.Location = new System.Drawing.Point(110, 332);
            this._bAcknowledgeState.Name = "_bAcknowledgeState";
            this._bAcknowledgeState.Size = new System.Drawing.Size(120, 23);
            this._bAcknowledgeState.TabIndex = 8;
            this._bAcknowledgeState.Text = "Acknowledge";
            this._bAcknowledgeState.UseVisualStyleBackColor = true;
            this._bAcknowledgeState.Click += new System.EventHandler(this._bConfirmState_Click);
            // 
            // _bViewDetails
            // 
            this._bViewDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bViewDetails.Location = new System.Drawing.Point(4, 332);
            this._bViewDetails.Name = "_bViewDetails";
            this._bViewDetails.Size = new System.Drawing.Size(100, 23);
            this._bViewDetails.TabIndex = 7;
            this._bViewDetails.Text = "View details";
            this._bViewDetails.UseVisualStyleBackColor = true;
            this._bViewDetails.Click += new System.EventHandler(this._bViewDetails_Click);
            // 
            // _gbAlarmsCount
            // 
            this._gbAlarmsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbAlarmsCount.Controls.Add(this._bDefaultSort);
            this._gbAlarmsCount.Controls.Add(this._bDockUndock);
            this._gbAlarmsCount.Controls.Add(this._lNormalNotAcknowledgedCount);
            this._gbAlarmsCount.Controls.Add(this._lNormalNotAcknowledged);
            this._gbAlarmsCount.Controls.Add(this._lAlarmCount);
            this._gbAlarmsCount.Controls.Add(this._lAlarm);
            this._gbAlarmsCount.Controls.Add(this._lAlarmNotAcknowladgedCount);
            this._gbAlarmsCount.Controls.Add(this._lAlarmNotAcknowledged);
            this._gbAlarmsCount.Location = new System.Drawing.Point(4, 3);
            this._gbAlarmsCount.Name = "_gbAlarmsCount";
            this._gbAlarmsCount.Size = new System.Drawing.Size(863, 53);
            this._gbAlarmsCount.TabIndex = 0;
            this._gbAlarmsCount.TabStop = false;
            this._gbAlarmsCount.Text = "Alarms count";
            // 
            // _bDefaultSort
            // 
            this._bDefaultSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bDefaultSort.Location = new System.Drawing.Point(701, 19);
            this._bDefaultSort.Name = "_bDefaultSort";
            this._bDefaultSort.Size = new System.Drawing.Size(75, 23);
            this._bDefaultSort.TabIndex = 6;
            this._bDefaultSort.Text = "Default sort";
            this._bDefaultSort.UseVisualStyleBackColor = true;
            this._bDefaultSort.Click += new System.EventHandler(this._bDefaultSort_Click);
            // 
            // _bDockUndock
            // 
            this._bDockUndock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bDockUndock.Location = new System.Drawing.Point(782, 19);
            this._bDockUndock.Name = "_bDockUndock";
            this._bDockUndock.Size = new System.Drawing.Size(75, 23);
            this._bDockUndock.TabIndex = 0;
            this._bDockUndock.Text = "Undock";
            this._bDockUndock.UseVisualStyleBackColor = true;
            this._bDockUndock.Click += new System.EventHandler(this._bDockUndock_Click);
            // 
            // _lNormalNotAcknowledgedCount
            // 
            this._lNormalNotAcknowledgedCount.AutoSize = true;
            this._lNormalNotAcknowledgedCount.Location = new System.Drawing.Point(350, 16);
            this._lNormalNotAcknowledgedCount.Name = "_lNormalNotAcknowledgedCount";
            this._lNormalNotAcknowledgedCount.Size = new System.Drawing.Size(13, 13);
            this._lNormalNotAcknowledgedCount.TabIndex = 5;
            this._lNormalNotAcknowledgedCount.Text = "0";
            // 
            // _lNormalNotAcknowledged
            // 
            this._lNormalNotAcknowledged.AutoSize = true;
            this._lNormalNotAcknowledged.Location = new System.Drawing.Point(196, 16);
            this._lNormalNotAcknowledged.Name = "_lNormalNotAcknowledged";
            this._lNormalNotAcknowledged.Size = new System.Drawing.Size(131, 13);
            this._lNormalNotAcknowledged.TabIndex = 4;
            this._lNormalNotAcknowledged.Text = "Noraml not acknowledged";
            // 
            // _lAlarmCount
            // 
            this._lAlarmCount.AutoSize = true;
            this._lAlarmCount.Location = new System.Drawing.Point(147, 33);
            this._lAlarmCount.Name = "_lAlarmCount";
            this._lAlarmCount.Size = new System.Drawing.Size(13, 13);
            this._lAlarmCount.TabIndex = 3;
            this._lAlarmCount.Text = "0";
            // 
            // _lAlarm
            // 
            this._lAlarm.AutoSize = true;
            this._lAlarm.Location = new System.Drawing.Point(6, 33);
            this._lAlarm.Name = "_lAlarm";
            this._lAlarm.Size = new System.Drawing.Size(33, 13);
            this._lAlarm.TabIndex = 2;
            this._lAlarm.Text = "Alarm";
            // 
            // _lAlarmNotAcknowladgedCount
            // 
            this._lAlarmNotAcknowladgedCount.AutoSize = true;
            this._lAlarmNotAcknowladgedCount.Location = new System.Drawing.Point(147, 16);
            this._lAlarmNotAcknowladgedCount.Name = "_lAlarmNotAcknowladgedCount";
            this._lAlarmNotAcknowladgedCount.Size = new System.Drawing.Size(13, 13);
            this._lAlarmNotAcknowladgedCount.TabIndex = 1;
            this._lAlarmNotAcknowladgedCount.Text = "0";
            // 
            // _lAlarmNotAcknowledged
            // 
            this._lAlarmNotAcknowledged.AutoSize = true;
            this._lAlarmNotAcknowledged.Location = new System.Drawing.Point(6, 16);
            this._lAlarmNotAcknowledged.Name = "_lAlarmNotAcknowledged";
            this._lAlarmNotAcknowledged.Size = new System.Drawing.Size(124, 13);
            this._lAlarmNotAcknowledged.TabIndex = 0;
            this._lAlarmNotAcknowledged.Text = "Alarm not acknowledged";
            // 
            // _gbFilter
            // 
            this._gbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbFilter.Controls.Add(this._chbAndAbove);
            this._gbFilter.Controls.Add(this._tcSearch);
            this._gbFilter.Controls.Add(this._tbdpDateTo);
            this._gbFilter.Controls.Add(this._tbdpDateFrom);
            this._gbFilter.Controls.Add(this._cbShowOnlyBlockedAlarm);
            this._gbFilter.Controls.Add(this._lAlarmPriority);
            this._gbFilter.Controls.Add(this._cbFilterAlarmPriority);
            this._gbFilter.Controls.Add(this._lDateTo);
            this._gbFilter.Controls.Add(this._lDateFrom);
            this._gbFilter.Controls.Add(this._lFilteredCount);
            this._gbFilter.Controls.Add(this._bFilter);
            this._gbFilter.Controls.Add(this._bClear);
            this._gbFilter.Controls.Add(this._eFilterText);
            this._gbFilter.Controls.Add(this._lFilteredText);
            this._gbFilter.Location = new System.Drawing.Point(4, 145);
            this._gbFilter.Name = "_gbFilter";
            this._gbFilter.Size = new System.Drawing.Size(863, 181);
            this._gbFilter.TabIndex = 2;
            this._gbFilter.TabStop = false;
            this._gbFilter.Text = "Filter";
            // 
            // _chbAndAbove
            // 
            this._chbAndAbove.AutoSize = true;
            this._chbAndAbove.Location = new System.Drawing.Point(210, 103);
            this._chbAndAbove.Name = "_chbAndAbove";
            this._chbAndAbove.Size = new System.Drawing.Size(76, 17);
            this._chbAndAbove.TabIndex = 20;
            this._chbAndAbove.Text = "AndAbove";
            this._chbAndAbove.UseVisualStyleBackColor = true;
            this._chbAndAbove.CheckedChanged += new System.EventHandler(this._chbAndAbove_CheckedChanged);
            // 
            // _tcSearch
            // 
            this._tcSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcSearch.Controls.Add(this._tpParametricSearch);
            this._tcSearch.Controls.Add(this._tpSubSitesFilter);
            this._tcSearch.Location = new System.Drawing.Point(318, 9);
            this._tcSearch.Name = "_tcSearch";
            this._tcSearch.SelectedIndex = 0;
            this._tcSearch.Size = new System.Drawing.Size(458, 166);
            this._tcSearch.TabIndex = 19;
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
            this._tpParametricSearch.Size = new System.Drawing.Size(450, 140);
            this._tpParametricSearch.TabIndex = 0;
            this._tpParametricSearch.Text = "Parametric search";
            // 
            // _rbParametricSearchOr
            // 
            this._rbParametricSearchOr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbParametricSearchOr.AutoSize = true;
            this._rbParametricSearchOr.Location = new System.Drawing.Point(3, 120);
            this._rbParametricSearchOr.Name = "_rbParametricSearchOr";
            this._rbParametricSearchOr.Size = new System.Drawing.Size(277, 17);
            this._rbParametricSearchOr.TabIndex = 5;
            this._rbParametricSearchOr.Text = "Show eventlogs that match at least one event source";
            this._rbParametricSearchOr.UseVisualStyleBackColor = true;
            // 
            // _rbParametricSearchAnd
            // 
            this._rbParametricSearchAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbParametricSearchAnd.AutoSize = true;
            this._rbParametricSearchAnd.Checked = true;
            this._rbParametricSearchAnd.Location = new System.Drawing.Point(3, 101);
            this._rbParametricSearchAnd.Name = "_rbParametricSearchAnd";
            this._rbParametricSearchAnd.Size = new System.Drawing.Size(237, 17);
            this._rbParametricSearchAnd.TabIndex = 4;
            this._rbParametricSearchAnd.TabStop = true;
            this._rbParametricSearchAnd.Text = "Show eventlogs that match all event sources";
            this._rbParametricSearchAnd.UseVisualStyleBackColor = true;
            // 
            // _bClearList
            // 
            this._bClearList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClearList.Location = new System.Drawing.Point(369, 63);
            this._bClearList.Name = "_bClearList";
            this._bClearList.Size = new System.Drawing.Size(75, 23);
            this._bClearList.TabIndex = 3;
            this._bClearList.Text = "Clear list";
            this._bClearList.UseVisualStyleBackColor = true;
            this._bClearList.Click += new System.EventHandler(this._bClearList_Click);
            // 
            // _bRemove
            // 
            this._bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRemove.Location = new System.Drawing.Point(369, 35);
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
            this._bAdd.Location = new System.Drawing.Point(369, 6);
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
            this._ilbEventSourceObject.Cursor = System.Windows.Forms.Cursors.Hand;
            this._ilbEventSourceObject.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbEventSourceObject.FormattingEnabled = true;
            this._ilbEventSourceObject.ImageList = null;
            this._ilbEventSourceObject.Location = new System.Drawing.Point(3, 3);
            this._ilbEventSourceObject.Name = "_ilbEventSourceObject";
            this._ilbEventSourceObject.SelectedItemObject = null;
            this._ilbEventSourceObject.Size = new System.Drawing.Size(357, 95);
            this._ilbEventSourceObject.TabIndex = 0;
            this._ilbEventSourceObject.DoubleClick += new System.EventHandler(this._ilbEventSourceObject_DoubleClick);
            this._ilbEventSourceObject.MouseDown += new System.Windows.Forms.MouseEventHandler(this._ilbEventSourceObject_MouseDown);
            this._ilbEventSourceObject.DragOver += new System.Windows.Forms.DragEventHandler(this._ilbEventSourceObject_DragOver);
            this._ilbEventSourceObject.DragDrop += new System.Windows.Forms.DragEventHandler(this._ilbEventSourceObject_DragDrop);
            // 
            // _tpSubSitesFilter
            // 
            this._tpSubSitesFilter.BackColor = System.Drawing.SystemColors.Control;
            this._tpSubSitesFilter.Controls.Add(this._ehWpfCheckBoxTreeView);
            this._tpSubSitesFilter.Controls.Add(this._cbAllSubSites);
            this._tpSubSitesFilter.Location = new System.Drawing.Point(4, 22);
            this._tpSubSitesFilter.Name = "_tpSubSitesFilter";
            this._tpSubSitesFilter.Size = new System.Drawing.Size(450, 140);
            this._tpSubSitesFilter.TabIndex = 2;
            this._tpSubSitesFilter.Text = "Filter for sub-sites";
            // 
            // _ehWpfCheckBoxTreeView
            // 
            this._ehWpfCheckBoxTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ehWpfCheckBoxTreeView.Location = new System.Drawing.Point(3, 3);
            this._ehWpfCheckBoxTreeView.Name = "_ehWpfCheckBoxTreeView";
            this._ehWpfCheckBoxTreeView.Size = new System.Drawing.Size(438, 95);
            this._ehWpfCheckBoxTreeView.TabIndex = 18;
            this._ehWpfCheckBoxTreeView.Text = "elementHost1";
            this._ehWpfCheckBoxTreeView.Child = this._wpfCheckBoxTreeView;
            // 
            // _cbAllSubSites
            // 
            this._cbAllSubSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cbAllSubSites.AutoSize = true;
            this._cbAllSubSites.Location = new System.Drawing.Point(3, 104);
            this._cbAllSubSites.Name = "_cbAllSubSites";
            this._cbAllSubSites.Size = new System.Drawing.Size(113, 17);
            this._cbAllSubSites.TabIndex = 17;
            this._cbAllSubSites.Text = "Select all sub-sites";
            this._cbAllSubSites.UseVisualStyleBackColor = true;
            // 
            // _tbdpDateTo
            // 
            this._tbdpDateTo.addActualTime = false;
            this._tbdpDateTo.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateTo.ButtonClearDateImage = null;
            this._tbdpDateTo.ButtonClearDateText = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbdpDateTo.ButtonClearDateWidth = 23;
            this._tbdpDateTo.ButtonDateImage = null;
            this._tbdpDateTo.ButtonDateText = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbdpDateTo.ButtonDateWidth = 23;
            this._tbdpDateTo.CustomFormat = "d. M. yyyy";
            this._tbdpDateTo.DateFormName = "Calendar";
            this._tbdpDateTo.LocalizationHelper = null;
            this._tbdpDateTo.Location = new System.Drawing.Point(106, 73);
            this._tbdpDateTo.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateTo.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateTo.Name = "_tbdpDateTo";
            this._tbdpDateTo.ReadOnly = false;
            this._tbdpDateTo.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpDateTo.Size = new System.Drawing.Size(192, 22);
            this._tbdpDateTo.TabIndex = 3;
            this._tbdpDateTo.ValidateAfter = 2;
            this._tbdpDateTo.ValidationEnabled = false;
            this._tbdpDateTo.ValidationError = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbdpDateTo.Value = null;
            // 
            // _tbdpDateFrom
            // 
            this._tbdpDateFrom.addActualTime = false;
            this._tbdpDateFrom.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateFrom.ButtonClearDateImage = null;
            this._tbdpDateFrom.ButtonClearDateText = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbdpDateFrom.ButtonClearDateWidth = 23;
            this._tbdpDateFrom.ButtonDateImage = null;
            this._tbdpDateFrom.ButtonDateText = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbdpDateFrom.ButtonDateWidth = 23;
            this._tbdpDateFrom.CustomFormat = "d. M. yyyy";
            this._tbdpDateFrom.DateFormName = "Calendar";
            this._tbdpDateFrom.LocalizationHelper = null;
            this._tbdpDateFrom.Location = new System.Drawing.Point(106, 45);
            this._tbdpDateFrom.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateFrom.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateFrom.Name = "_tbdpDateFrom";
            this._tbdpDateFrom.ReadOnly = false;
            this._tbdpDateFrom.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpDateFrom.Size = new System.Drawing.Size(192, 22);
            this._tbdpDateFrom.TabIndex = 2;
            this._tbdpDateFrom.ValidateAfter = 2;
            this._tbdpDateFrom.ValidationEnabled = false;
            this._tbdpDateFrom.ValidationError = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbdpDateFrom.Value = null;
            // 
            // _cbShowOnlyBlockedAlarm
            // 
            this._cbShowOnlyBlockedAlarm.AutoSize = true;
            this._cbShowOnlyBlockedAlarm.Location = new System.Drawing.Point(9, 158);
            this._cbShowOnlyBlockedAlarm.Name = "_cbShowOnlyBlockedAlarm";
            this._cbShowOnlyBlockedAlarm.Size = new System.Drawing.Size(144, 17);
            this._cbShowOnlyBlockedAlarm.TabIndex = 15;
            this._cbShowOnlyBlockedAlarm.Text = "Show only blocked alarm";
            this._cbShowOnlyBlockedAlarm.UseVisualStyleBackColor = true;
            this._cbShowOnlyBlockedAlarm.CheckedChanged += new System.EventHandler(this._cbShowOnlyBlockedAlarm_CheckedChanged);
            // 
            // _lAlarmPriority
            // 
            this._lAlarmPriority.AutoSize = true;
            this._lAlarmPriority.Location = new System.Drawing.Point(6, 104);
            this._lAlarmPriority.Name = "_lAlarmPriority";
            this._lAlarmPriority.Size = new System.Drawing.Size(66, 13);
            this._lAlarmPriority.TabIndex = 10;
            this._lAlarmPriority.Text = "Alarm priority";
            // 
            // _cbFilterAlarmPriority
            // 
            this._cbFilterAlarmPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbFilterAlarmPriority.FormattingEnabled = true;
            this._cbFilterAlarmPriority.Location = new System.Drawing.Point(106, 101);
            this._cbFilterAlarmPriority.Name = "_cbFilterAlarmPriority";
            this._cbFilterAlarmPriority.Size = new System.Drawing.Size(98, 21);
            this._cbFilterAlarmPriority.TabIndex = 4;
            this._cbFilterAlarmPriority.SelectedIndexChanged += new System.EventHandler(this._cbFilterAlarmPriority_SelectedIndexChanged);
            // 
            // _lDateTo
            // 
            this._lDateTo.AutoSize = true;
            this._lDateTo.Location = new System.Drawing.Point(6, 79);
            this._lDateTo.Name = "_lDateTo";
            this._lDateTo.Size = new System.Drawing.Size(42, 13);
            this._lDateTo.TabIndex = 6;
            this._lDateTo.Text = "Date to";
            // 
            // _lDateFrom
            // 
            this._lDateFrom.AutoSize = true;
            this._lDateFrom.Location = new System.Drawing.Point(6, 51);
            this._lDateFrom.Name = "_lDateFrom";
            this._lDateFrom.Size = new System.Drawing.Size(53, 13);
            this._lDateFrom.TabIndex = 2;
            this._lDateFrom.Text = "Date from";
            // 
            // _lFilteredCount
            // 
            this._lFilteredCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lFilteredCount.Location = new System.Drawing.Point(782, 11);
            this._lFilteredCount.Name = "_lFilteredCount";
            this._lFilteredCount.Size = new System.Drawing.Size(75, 26);
            this._lFilteredCount.TabIndex = 14;
            this._lFilteredCount.Text = "Filtered count: 1234";
            this._lFilteredCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _bFilter
            // 
            this._bFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bFilter.Location = new System.Drawing.Point(782, 119);
            this._bFilter.Name = "_bFilter";
            this._bFilter.Size = new System.Drawing.Size(75, 23);
            this._bFilter.TabIndex = 5;
            this._bFilter.Text = "Filter";
            this._bFilter.UseVisualStyleBackColor = true;
            this._bFilter.Click += new System.EventHandler(this._bFilter_Click);
            // 
            // _bClear
            // 
            this._bClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClear.Location = new System.Drawing.Point(782, 148);
            this._bClear.Name = "_bClear";
            this._bClear.Size = new System.Drawing.Size(75, 23);
            this._bClear.TabIndex = 6;
            this._bClear.Text = "Clear";
            this._bClear.UseVisualStyleBackColor = true;
            this._bClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // _eFilterText
            // 
            this._eFilterText.Location = new System.Drawing.Point(106, 19);
            this._eFilterText.Name = "_eFilterText";
            this._eFilterText.Size = new System.Drawing.Size(192, 20);
            this._eFilterText.TabIndex = 1;
            this._eFilterText.KeyDown += new System.Windows.Forms.KeyEventHandler(this._eFilterText_KeyDown);
            // 
            // _lFilteredText
            // 
            this._lFilteredText.AutoSize = true;
            this._lFilteredText.Location = new System.Drawing.Point(6, 24);
            this._lFilteredText.Name = "_lFilteredText";
            this._lFilteredText.Size = new System.Drawing.Size(28, 13);
            this._lFilteredText.TabIndex = 0;
            this._lFilteredText.Text = "Text";
            // 
            // _bBlock
            // 
            this._bBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bBlock.Location = new System.Drawing.Point(404, 332);
            this._bBlock.Name = "_bBlock";
            this._bBlock.Size = new System.Drawing.Size(75, 23);
            this._bBlock.TabIndex = 9;
            this._bBlock.Text = "Block";
            this._bBlock.UseVisualStyleBackColor = true;
            this._bBlock.Click += new System.EventHandler(this._bBlock_Click);
            // 
            // _bUnblock
            // 
            this._bUnblock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bUnblock.Location = new System.Drawing.Point(485, 332);
            this._bUnblock.Name = "_bUnblock";
            this._bUnblock.Size = new System.Drawing.Size(75, 23);
            this._bUnblock.TabIndex = 10;
            this._bUnblock.Text = "Unblock";
            this._bUnblock.UseVisualStyleBackColor = true;
            this._bUnblock.Click += new System.EventHandler(this._bUnblock_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(32, 19);
            this.toolStripMenuItem4.Text = "toolStripMenuItem4";
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvData.CopyOnRightClick = false;
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
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(648, 80);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(648, 80);
            this._cdgvData.TabIndex = 12;
            this._cdgvData.Click += new System.EventHandler(this._cdgvData_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(4, 62);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this._cdgvData);
            this.splitContainer.Panel1MinSize = 500;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this._llAlarmName);
            this.splitContainer.Panel2.Controls.Add(this._bHideOrShow);
            this.splitContainer.Panel2.Controls.Add(this._bEdit);
            this.splitContainer.Panel2.Controls.Add(this._dgAlarmInstructions);
            this.splitContainer.Panel2.Controls.Add(this._lAlarmInstruction);
            this.splitContainer.Panel2MinSize = 0;
            this.splitContainer.Size = new System.Drawing.Size(863, 80);
            this.splitContainer.SplitterDistance = 648;
            this.splitContainer.TabIndex = 13;
            // 
            // _llAlarmName
            // 
            this._llAlarmName.AutoSize = true;
            this._llAlarmName.Location = new System.Drawing.Point(19, 3);
            this._llAlarmName.Name = "_llAlarmName";
            this._llAlarmName.Size = new System.Drawing.Size(62, 13);
            this._llAlarmName.TabIndex = 82;
            this._llAlarmName.TabStop = true;
            this._llAlarmName.Text = "Alarm name";
            this._llAlarmName.Visible = false;
            this._llAlarmName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._llAlarmName_LinkClicked);
            // 
            // _bHideOrShow
            // 
            this._bHideOrShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._bHideOrShow.FlatAppearance.BorderSize = 0;
            this._bHideOrShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bHideOrShow.Image = global::Contal.Cgp.Client.Properties.Resources.show;
            this._bHideOrShow.Location = new System.Drawing.Point(1, 3);
            this._bHideOrShow.Name = "_bHideOrShow";
            this._bHideOrShow.Size = new System.Drawing.Size(21, 74);
            this._bHideOrShow.TabIndex = 80;
            this._bHideOrShow.UseVisualStyleBackColor = true;
            this._bHideOrShow.Click += new System.EventHandler(this._bHideOrShow_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(136, 57);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 79;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _dgAlarmInstructions
            // 
            this._dgAlarmInstructions.AllowUserToAddRows = false;
            this._dgAlarmInstructions.AllowUserToDeleteRows = false;
            this._dgAlarmInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgAlarmInstructions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this._dgAlarmInstructions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgAlarmInstructions.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgAlarmInstructions.DefaultCellStyle = dataGridViewCellStyle1;
            this._dgAlarmInstructions.Location = new System.Drawing.Point(23, 19);
            this._dgAlarmInstructions.MultiSelect = false;
            this._dgAlarmInstructions.Name = "_dgAlarmInstructions";
            this._dgAlarmInstructions.ReadOnly = true;
            this._dgAlarmInstructions.RowHeadersVisible = false;
            this._dgAlarmInstructions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgAlarmInstructions.Size = new System.Drawing.Size(188, 32);
            this._dgAlarmInstructions.TabIndex = 78;
            // 
            // _lAlarmInstruction
            // 
            this._lAlarmInstruction.AutoSize = true;
            this._lAlarmInstruction.Location = new System.Drawing.Point(19, 3);
            this._lAlarmInstruction.Name = "_lAlarmInstruction";
            this._lAlarmInstruction.Size = new System.Drawing.Size(92, 13);
            this._lAlarmInstruction.TabIndex = 0;
            this._lAlarmInstruction.Text = "Alarm instructions:";
            // 
            // _bAcknowledgeAndBlock
            // 
            this._bAcknowledgeAndBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bAcknowledgeAndBlock.Location = new System.Drawing.Point(236, 332);
            this._bAcknowledgeAndBlock.Name = "_bAcknowledgeAndBlock";
            this._bAcknowledgeAndBlock.Size = new System.Drawing.Size(162, 23);
            this._bAcknowledgeAndBlock.TabIndex = 14;
            this._bAcknowledgeAndBlock.Text = "AcknowledgeAndBlock";
            this._bAcknowledgeAndBlock.UseVisualStyleBackColor = true;
            this._bAcknowledgeAndBlock.Click += new System.EventHandler(this._bAcknowledgeAndBlock_Click);
            // 
            // AlarmsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(871, 357);
            this.Controls.Add(this._gbAlarmsCount);
            this.Controls.Add(this._bAcknowledgeAndBlock);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bUnblock);
            this.Controls.Add(this._gbFilter);
            this.Controls.Add(this._bViewDetails);
            this.Controls.Add(this._bBlock);
            this.Controls.Add(this._bAcknowledgeState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(740, 360);
            this.Name = "AlarmsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AlarmsDialog";
            this.Load += new System.EventHandler(this.AlarmsDialog_Load);
            this.SizeChanged += new System.EventHandler(this.AlarmsDialog_SizeChanged);
            this._alarmMenu.ResumeLayout(false);
            this._gbAlarmsCount.ResumeLayout(false);
            this._gbAlarmsCount.PerformLayout();
            this._gbFilter.ResumeLayout(false);
            this._gbFilter.PerformLayout();
            this._tcSearch.ResumeLayout(false);
            this._tpParametricSearch.ResumeLayout(false);
            this._tpParametricSearch.PerformLayout();
            this._tpSubSitesFilter.ResumeLayout(false);
            this._tpSubSitesFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgAlarmInstructions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bAcknowledgeState;
        private System.Windows.Forms.Button _bViewDetails;
        private System.Windows.Forms.GroupBox _gbAlarmsCount;
        private System.Windows.Forms.Label _lNormalNotAcknowledgedCount;
        private System.Windows.Forms.Label _lNormalNotAcknowledged;
        private System.Windows.Forms.Label _lAlarmCount;
        private System.Windows.Forms.Label _lAlarm;
        private System.Windows.Forms.Label _lAlarmNotAcknowladgedCount;
        private System.Windows.Forms.Label _lAlarmNotAcknowledged;
        private System.Windows.Forms.Button _bDockUndock;
        private System.Windows.Forms.GroupBox _gbFilter;
        private System.Windows.Forms.TextBox _eFilterText;
        private System.Windows.Forms.Label _lFilteredText;
        private System.Windows.Forms.Label _lFilteredCount;
        private System.Windows.Forms.Button _bFilter;
        private System.Windows.Forms.Button _bClear;
        private System.Windows.Forms.Label _lDateFrom;
        private System.Windows.Forms.Label _lDateTo;
        private System.Windows.Forms.Label _lAlarmPriority;
        private System.Windows.Forms.ComboBox _cbFilterAlarmPriority;
        private System.Windows.Forms.CheckBox _cbShowOnlyBlockedAlarm;
        private System.Windows.Forms.Button _bBlock;
        private System.Windows.Forms.Button _bUnblock;
        private System.Windows.Forms.ContextMenuStrip _alarmMenu;
        private System.Windows.Forms.ToolStripMenuItem _tsAcknowledge;
        private System.Windows.Forms.ToolStripMenuItem _tsBlock;
        private System.Windows.Forms.ToolStripLabel _tsLoading;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateFrom;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateTo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem _tsUnblock;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Button _bDefaultSort;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label _lAlarmInstruction;
        private System.Windows.Forms.DataGridView _dgAlarmInstructions;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bHideOrShow;
        private LinkLabel _llAlarmName;
        private CheckBox _cbAllSubSites;
        private System.Windows.Forms.Integration.ElementHost _ehWpfCheckBoxTreeView;
        private WpfCheckBoxesTreeview _wpfCheckBoxTreeView;
        private Button _bAcknowledgeAndBlock;
        private ToolStripMenuItem _tsAcknowledgeAndBlock;
        private TabControl _tcSearch;
        private TabPage _tpParametricSearch;
        private RadioButton _rbParametricSearchOr;
        private RadioButton _rbParametricSearchAnd;
        private Button _bClearList;
        private Button _bRemove;
        private Button _bAdd;
        private Contal.IwQuick.UI.ImageListBox _ilbEventSourceObject;
        private TabPage _tpSubSitesFilter;
        private CheckBox _chbAndAbove;
    }
}