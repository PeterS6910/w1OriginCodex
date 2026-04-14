using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;

using System.Drawing;
using Contal.IwQuick.UI;
using Contal.IwQuick;
using Contal.Cgp.Globals;
using Cgp.Components;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Sys;
using ComponentFactory.Krypton.Toolkit;

namespace Contal.Cgp.Client
{
    public enum ShowOptionsEditForm : byte
    {
        Insert = 0,
        Edit = 1,
        InsertDialog = 2,
        InsertWithData = 3,
        View = 4
    }
    /// <summary>
    /// 	Generic abstract class for Edit Form classes.
    /// </summary>
    public abstract class ACgpEditForm<T> :
        MdiChildForm, 
        ICgpEditForm,
        IExtendedCgpEditForm,
        IEditFormBase
        where T : AOrmObject
    {
        protected T _editingObject;

        private ShowOptionsEditForm _showOption;
        private bool _wasChangedValues;
        private bool _wasChangedValuesOnlyInDatabase;
        private bool _isSetValues;
        private bool _editingInInsertForm;
        private Action<object> _doAfterInserted;
        private DVoid2Void _doAfterInsertClosed;
        private readonly DVoid2Void _eventColorChanged;
        private Action<object> _doAfterEdited;
        private readonly MdiClient _mdiClient;
        private readonly int _borderWidth;
        private readonly int _borderHeight;
        private bool _isEditAllowed;
        private SizeF _scaleF = new SizeF(1.0F, 1.0F);//DPI 100%

        public event Action<object> EditingObjectChanged;

        public void ShowAndRunSetValues()
        {
            Show();
            SetValues();
        }

        public override bool IsEditForm()
        {
            return true;
        }
        public void SetAllowEdit(bool allowEdit)
        {
            _isEditAllowed = allowEdit;
        }

        public bool? AllowEdit
        {
            get { return _isEditAllowed; }
            set { _isEditAllowed = true; }
        }

        public Action<object> DoAfterInserted
        {
            set { _doAfterInserted = value; }
        }

        public DVoid2Void DoAfterInsertClosed
        {
            set { _doAfterInsertClosed = value; }
        }

        public Action<object> DoAfterEdited
        {
            set { _doAfterEdited = value; }
        }

        protected bool Insert
        {
            get { return _showOption == ShowOptionsEditForm.Insert || _showOption == ShowOptionsEditForm.InsertDialog || _showOption == ShowOptionsEditForm.InsertWithData; }
        }

        protected bool IsSetValues
        {
            get { return _isSetValues; }
        }

        public bool ValueChanged
        {
            get { return _wasChangedValues; }
        }

        protected ACgpEditForm(T editingObject, ShowOptionsEditForm showOption)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            SuspendLayout();
            //default ControlBox settings
            ControlBox = true;
            MinimizeBox = false;
         
       
            _editingObject = editingObject;
            _showOption = showOption;

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                MdiParent = CgpClientMainForm.Singleton;

            FormOnEnter += EditForm_Enter;
           
            _eventColorChanged = ColorSettingsChanged;
            ColorSettingsChangedHandler.Singleton.RegisterColorChanged(_eventColorChanged);
            MouseWheel += EditFormMouseWheel;
            _mdiClient = GetMdiClientWindow();
            // At design-time, each ContainerControl records the scaling mode and
            // its current resolution in the AutoScaleMode and AutoScaleDimensions.
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;

            // Create Dock/Undock button (all Windows versions)
            ButtonSpecAny btnSpecDockUndock = new ButtonSpecAny()
            {
                UniqueName = "btnSpecDockUndock",
                Type = PaletteButtonSpecStyle.FormRestore,
                Style = PaletteButtonStyle.Form,
                ToolTipTitle = "Dock/Undock"
            };
            btnSpecDockUndock.Click += _tbbDockUndock_ButtonClicked;
            ButtonSpecs.Add(btnSpecDockUndock);

            _borderWidth = Width - ClientSize.Width;
            _borderHeight = Height - ClientSize.Height;
            Move += ACgpEditForm_Move;
            KeyPreview = true;
            KeyDown += ACgpEditForm_KeyDown;

            if (!Insert)
                RegisterEvents();
            ResumeLayout();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == DllUser32.WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == DllUser32.SC_MAXIMIZE)
                {
                    MaximizeMinimize();
                    return;
                }
            }

            base.WndProc(ref m);
        }

        // Hook to the title bar button click event      
        void _tbbDockUndock_ButtonClicked(object sender, EventArgs e)
        {
            DockUndock();
        }

        void DockUndock()
        {
            if (MdiParent == null)
            {
                DockWindow();
            }
            else
            {
                UndockWindow();
            }

            AfterDockUndock();
        }

        public void DockWindow()
        {
            TopMost = false;
            WindowState = FormWindowState.Normal;
            MdiParent = CgpClientMainForm.Singleton;
            FormBorderStyle = FormBorderStyle.Sizable;
            Dock = DockStyle.None;
            
            
            var objectName = string.Empty;
            if (_editingObject != null)
                objectName = _editingObject.ToString();

            CgpClientMainForm.Singleton.AddToOpenWindows(this, objectName);
            
            CgpClientMainForm.Singleton.SetActOpenWindow(this);
            CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);

            ButtonSpecs["btnSpecDockUndock"].Type = PaletteButtonSpecStyle.FormRestore;
            MinimizeBox = false;
        }

        public void UndockWindow()
        {
            SuspendLayout();
            var parent = MdiParent;
            MdiParent = null;
            if (parent != null)
            {
                Location = Screen.FromControl(parent).WorkingArea.Location;
            }
            FormBorderStyle = FormBorderStyle.Sizable;
            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            CgpClientMainForm.Singleton.AddToUndockedForms(this);

            ButtonSpecs["btnSpecDockUndock"].Type = PaletteButtonSpecStyle.RibbonExpand;
            MinimizeBox = true;

            ResumeLayout(true);
        }

        void MaximizeMinimize()
        {
            if (MdiParent == null)
            {
                WindowState = 
                    WindowState == FormWindowState.Maximized
                        ? FormWindowState.Normal
                        : FormWindowState.Maximized;
            }
            else
            {
                if (Dock == DockStyle.Fill)
                {
                    //minimize
                    Dock = DockStyle.None;
                    var parentForm = ParentForm;
                    if (parentForm != null)
                        parentForm.LayoutMdi(MdiLayout.Cascade);
                }
                else
                {
                    //maximize
                    Dock = DockStyle.Fill;
                }
            }
        }


        protected abstract void RegisterEvents();

        void ACgpEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (MdiParent == null && e.KeyCode == Keys.Escape)
            {
                Cancel_Click();
            }
        }

        public override void CallEscape()
        {
            if (MdiParent != null)
            {
                Cancel_Click();
            }
        }

        void ACgpEditForm_Move(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                if ((Left - _borderWidth <= 0) //left
                    || (Right + _borderWidth >= GetMdiAreaSize(_mdiClient, -4, -4).Width) //right
                    || (Top - _borderHeight <= 0)//top
                    || (Bottom + _borderHeight >= ((GetMdiAreaSize(_mdiClient, -4, -4).Height)))) //bottom
                {
                    Parent.Height = Parent.Height + 1;
                }
            }
        }

        public Size GetMdiAreaSize(MdiClient mdiClient, int chagerX, int changerY)
        {
            var size = mdiClient.Size;
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

        TabControl _whTabConrol;
        void EditFormMouseWheel(object sender, MouseEventArgs e)
        {
            if (_whTabConrol == null) return;

            if (e.X > _whTabConrol.Location.X &&
                e.X < (_whTabConrol.Location.X + _whTabConrol.Width) &&
                e.Y > _whTabConrol.Location.Y &&
                e.Y < (_whTabConrol.Location.Y + _whTabConrol.Height))
            {
                TabControlNextPage(_whTabConrol, e);
            }
        }

        protected TabControl WheelTabContorol
        {
            set { _whTabConrol = value; }
        }

        protected abstract void BeforeInsert();

        protected abstract void BeforeEdit();

        private void EditForm_Enter(Form form)
        {
            if (!_isSetValues)
            {
                if (_showOption == ShowOptionsEditForm.Insert || _showOption == ShowOptionsEditForm.InsertWithData)
                {
                    BeforeInsert();
                    CgpClientMainForm.Singleton.AddToOpenWindows(this);
                }
                else if (_showOption == ShowOptionsEditForm.Edit || _showOption == ShowOptionsEditForm.View)
                {
                    BeforeEdit();
                    CgpClientMainForm.Singleton.AddToOpenWindows(this, GetEditingObjectName());
                }
                SetValues();
                _wasChangedValues = false;
                _wasChangedValuesOnlyInDatabase = false;
                _isSetValues = true;
            }

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        private string GetEditingObjectName()
        {
            if (_editingObject is Calendar || _editingObject is DailyPlan || _editingObject is DayType)
            {
                var ro = new RecentObjectList.RecentObject(_editingObject, null, null, true);
                return ro.ToString();
            }
            if (_editingObject != null)
            {
                return _editingObject.ToString();
            }
            return string.Empty;
        }

        protected void SetValues()
        {
            ConnectionLost();
            try
            {
                if (Insert)
                {
                    SetValuesInsert();
                }
                else
                {
                    CreateAlarmInstructionsTabPage();
                    SetValuesEdit();
                    if (_showOption == ShowOptionsEditForm.View)
                    {
                        DisabledForm();
                    }
                }
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(GetString("ErrorSetValuesFailed"));
                Close();
            }
        }

        public void ReloadEditingObject(out bool allowEdit)
        {
            InternalReloadEditingObject(out allowEdit);

            if (EditingObjectChanged != null)
                try
                {
                    EditingObjectChanged(_editingObject);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        protected abstract void InternalReloadEditingObject(out bool allowEdit);

        public void ReloadEditingObjectWithEditedData()
        {
            InternalReloadEditingObjectWithEditedData();

            if (EditingObjectChanged != null)
                try
                {
                    EditingObjectChanged(_editingObject);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        public abstract void InternalReloadEditingObjectWithEditedData();

        protected abstract void SetValuesInsert();

        protected abstract void SetValuesEdit();

        #region AlarmInstructions

        private AlarmInstructionsForm _alarmInstructionsForm;
        private BindingSource _alarmInstructionsBindingSource;
        private void CreateAlarmInstructionsTabPage()
        {
            var localAlarmInstructionsView = LocalAlarmInstructionsView();
            var localAlarmInstructionsAdmin = LocalAlarmInstructionsAdmin();

            if (_whTabConrol != null &&
                (localAlarmInstructionsView ||
                 localAlarmInstructionsAdmin ||
                 GlobalAlarmInstructionsForm.Singleton.HasAccessView()))
            {
                var alarmInstructions = new TabPage();
                alarmInstructions.Name = "_tpAlarmInstructions";
                alarmInstructions.Text = "Alarm instructions";
                alarmInstructions.BackColor = SystemColors.Control;

                _alarmInstructionsForm = new AlarmInstructionsForm(GetLocalAlarmInstruction(),
                    localAlarmInstructionsView,
                    localAlarmInstructionsAdmin,
                    LocalAlarmInstructionTextChanged,
                    GlobalAlarmInstructionsCreateClick,
                    GlobalAlarmInstructionsInsertClick,
                    GlobalAlarmInstructionsEditClick, GlobalAlarmInstructionsDeleteClick,
                    GlobalAlarmInstructionsDragDrop);

                AlarmInstructionsReloadData();

                alarmInstructions.Controls.Add(_alarmInstructionsForm);

                //int index = 0;
                //if (_whTabConrol.TabPages.Count > 3)
                //    index = _whTabConrol.TabPages.Count - 3;
                //_whTabConrol.TabPages.Insert(index, alarmInstructions);
                _whTabConrol.TabPages.Add(alarmInstructions);

                LocalizationHelper.TranslateControl(_alarmInstructionsForm);
                LocalizationHelper.TranslateControl(_whTabConrol);
            }
        }

        protected virtual bool LocalAlarmInstructionsView()
        {
            return false;
        }

        protected virtual bool LocalAlarmInstructionsAdmin()
        {
            return false;
        }

        protected virtual string GetLocalAlarmInstruction()
        {
            return string.Empty;
        }

        private void AlarmInstructionsReloadData()
        {
            try
            {
                var position = 0;
                if (_alarmInstructionsBindingSource != null && _alarmInstructionsBindingSource.Count > 0)
                    position = _alarmInstructionsBindingSource.Position;

                var dgGlobalInstructions = _alarmInstructionsForm.GetDataGridViewGlobalinstructions();
                var globalAlarmInstructions = GetGlobalAlarmInstructions();

                if (globalAlarmInstructions == null || globalAlarmInstructions.Count == 0)
                {
                    _alarmInstructionsBindingSource = null;
                    dgGlobalInstructions.DataSource = null;
                }
                else
                {
                    _alarmInstructionsBindingSource = new BindingSource
                    {
                        DataSource = globalAlarmInstructions
                    };

                    if (position < _alarmInstructionsBindingSource.Count)
                        _alarmInstructionsBindingSource.Position = position;
                    else
                        _alarmInstructionsBindingSource.Position = _alarmInstructionsBindingSource.Count - 1;

                    dgGlobalInstructions.DataSource = _alarmInstructionsBindingSource;

                    dgGlobalInstructions.AutoGenerateColumns = false;
                    dgGlobalInstructions.AllowUserToAddRows = false;

                    if (dgGlobalInstructions.Columns.Contains(GlobalAlarmInstruction.COLUMN_NAME))
                        dgGlobalInstructions.Columns[GlobalAlarmInstruction.COLUMN_NAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    if (dgGlobalInstructions.Columns.Contains(GlobalAlarmInstruction.COLUMN_INSTRUCTIONS))
                        dgGlobalInstructions.Columns[GlobalAlarmInstruction.COLUMN_INSTRUCTIONS].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    HideColumnDgw(dgGlobalInstructions, GlobalAlarmInstruction.COLUMN_ID_GLOBAL_ALARM_INSTRUCTION);
                    HideColumnDgw(dgGlobalInstructions, GlobalAlarmInstruction.COLUMN_OBJECT_TYPE);
                    HideColumnDgw(dgGlobalInstructions, GlobalAlarmInstruction.COLUMN_DESCRIPTION);

                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(dgGlobalInstructions);
                }
            }
            catch(Exception error) 
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private static void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;
            if (gridView.Columns.Contains(columnName))
                gridView.Columns[columnName].Visible = false;
        }

        protected string GetNewLocalAlarmInstruction()
        {
            if (_alarmInstructionsForm != null)
                return _alarmInstructionsForm.GetLocalInstruction();

            return string.Empty;
        }

        private ICollection<GlobalAlarmInstruction> GetGlobalAlarmInstructions()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return null;

            try
            {
                return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetGlobalAlarmInstructionsForObject(_editingObject.GetObjectType(), _editingObject.GetIdString());
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return null;
        }

        private void LocalAlarmInstructionTextChanged()
        {
            EditTextChanger(null, null);
        }

        private void GlobalAlarmInstructionsCreateClick()
        {
            try
            {
                var globalAlarmInstruction = new GlobalAlarmInstruction();
                if (GlobalAlarmInstructionsForm.Singleton.OpenInsertDialg(ref globalAlarmInstruction))
                {
                    if (CgpClient.Singleton.IsConnectionLost(true))
                        return;

                    GlobalAlarmInstructionsInsert(globalAlarmInstruction);
                }
            }
            catch(Exception error) 
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void GlobalAlarmInstructionsInsertClick()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                var listModObj = new List<IModifyObject>();

                Exception error;
                var listGlobalAlarmInstructionsFromDatabase = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.ListModifyObjects(out error);
                listModObj.AddRange(listGlobalAlarmInstructionsFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, GetString("GlobalAlarmInstructionsFormGlobalAlarmInstructionsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var globalAlarmInstruction = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(outModObj.GetId);

                    GlobalAlarmInstructionsInsert(globalAlarmInstruction);
                }
            }
            catch(Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(LocalizationHelper.GetString("ErrorInsertGlobalAlarmInstructionToObjectFailed"));
            }
        }

        private void GlobalAlarmInstructionsInsert(GlobalAlarmInstruction globalAlarmInstruction)
        {
            try
            {
                Exception error;
                if (!CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.AddReference(globalAlarmInstruction.IdGlobalAlarmInstruction, _editingObject.GetObjectType(), _editingObject.GetIdString(), out error))
                {
                    throw error;
                }
                AlarmInstructionsReloadData();
                CgpClientMainForm.Singleton.AddToRecentList(globalAlarmInstruction);
            }
            catch (Exception exception)
            {
                if (exception is SqlUniqueException) 
                    Dialog.Error(LocalizationHelper.GetString("ErrorGlobalAlarmInstructionIsAlreadAddedToThisObject"));
                else
                {
                    HandledExceptionAdapter.Examine(exception);
                    Dialog.Error(LocalizationHelper.GetString("ErrorInsertGlobalAlarmInstructionToObjectFailed"));
                }
            }
        }

        private void GlobalAlarmInstructionsEditClick()
        {
            try
            {
                if (_alarmInstructionsBindingSource != null && _alarmInstructionsBindingSource.Count > 0)
                {
                    var globalAlarmInstruction = (GlobalAlarmInstruction)_alarmInstructionsBindingSource.List[_alarmInstructionsBindingSource.Position];
                    if (globalAlarmInstruction != null)
                        GlobalAlarmInstructionsForm.Singleton.OpenEditForm(globalAlarmInstruction);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void GlobalAlarmInstructionsDeleteClick()
        {
            try
            {
                if (!Dialog.Question(GetString("QuestionDeleteConfirm")))
                    return;

                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                if (_alarmInstructionsBindingSource != null && _alarmInstructionsBindingSource.Count > 0)
                {
                    var globalAlarmInstruction = (GlobalAlarmInstruction)_alarmInstructionsBindingSource.List[_alarmInstructionsBindingSource.Position];
                    if (globalAlarmInstruction != null)
                    {
                        CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.RemoveReference(globalAlarmInstruction.IdGlobalAlarmInstruction, _editingObject.GetObjectType(), _editingObject.GetIdString());
                        AlarmInstructionsReloadData();
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void GlobalAlarmInstructionsDragDrop(object dragDropObject)
        {
            try
            {
                var globalAlarmInstruction = dragDropObject as GlobalAlarmInstruction;
                if (globalAlarmInstruction != null)
                {
                    if (CgpClient.Singleton.IsConnectionLost(true))
                        return;

                    GlobalAlarmInstructionsInsert(globalAlarmInstruction);
                }
                else
                {
                    Dialog.Error(LocalizationHelper.GetString("ErrorWrongObjectType"));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        #endregion

        public virtual void SetValuesInsertFromObj(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual void SetValuesInsertFromObjWithFilter(object obj, Dictionary<string, bool> filter)
        {
            throw new NotImplementedException();
        }

        protected virtual void DisabledForm()
        {
            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    DisabledControls(control);
                }
            }
        }

        protected void DisabledControls(Control control)
        {
            if (control.Controls == null || control.Controls.Count == 0)
                DisabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    DisabledControls(actControl);
        }

        private static void DisabledControl(Control control)
        {
            if (!(control is Label))
                control.Enabled = false;
        }

        protected void EnabledControls(Control control)
        {
            if (control.Controls == null || control.Controls.Count == 0)
                EnabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    EnabledControls(actControl);
        }

        private static void EnabledControl(Control control)
        {
            control.Enabled = true;
        }

        protected bool Cancel_Click()
        {
            if (_wasChangedValues || _wasChangedValuesOnlyInDatabase)
            {
                BringToFront();
                var result =
                    MessageBox.Show(GetString("ValueChanged") + "\n" + GetString("SaveAfterCancel"), GetString("Question"),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                switch (result)
                {
                    case DialogResult.Cancel:
                        {
                            return false;
                        }
                    case DialogResult.No:
                        {
                            Close();
                            return true;
                        }
                    case DialogResult.Yes:
                        {
                            return SaveData(true);
                        }
                    default: return false;
                }
            }
            Close();
            return true;
        }

        protected bool Apply_Click()
        {
            var applied = false;

            if (_wasChangedValues || _wasChangedValuesOnlyInDatabase || Insert)
            {
                if (CheckValues())
                {
                    if (SaveToDatabase())
                    {
                        applied = true;
                  
                        ReloadEditingObject(out _isEditAllowed);
                        if (_showOption == ShowOptionsEditForm.Insert)
                            _showOption = ShowOptionsEditForm.Edit;
                        _wasChangedValues = false;
                        _wasChangedValuesOnlyInDatabase = false;
                    }
                }
            }

            return applied;
        }

        protected bool ApplyClickWithDialog()
        {
            if (_wasChangedValues || _wasChangedValuesOnlyInDatabase)
            {
                BringToFront();
                var result =
                    MessageBox.Show(GetString("ValueChanged") + "\n" + GetString("SaveAfterCancel"), GetString("Question"),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Cancel:
                        {
                            return false;
                        }
                    case DialogResult.No:
                        {
                            return false;
                        }
                    case DialogResult.Yes:
                        {
                            return SaveData(false);
                        }
                    default: return false;
                }
            }
            return true;
        }

        protected bool SaveData(bool closeDialog)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return false;

            if (!_wasChangedValues && !_wasChangedValuesOnlyInDatabase && !(Insert))
            {
                if (CheckValues())
                {
                    if (closeDialog)
                    {
                        _isOkClose = true;
                        if (_editingInInsertForm)
                        {
                            _showOption = ShowOptionsEditForm.Insert;
                        }
                        Close();
                    }

                    return true;
                }
            }
            else if (CheckValues())
            {
                if (SaveToDatabase())
                {
                    _isOkClose = true;
                    if (closeDialog)
                    {
                        if (_editingInInsertForm)
                        {
                            _showOption = ShowOptionsEditForm.Insert;
                        }
                        Close();
                    }
                    else
                    {
                        SetFormForEditing();
                    }

                    return true;
                }
            }
            return false;
        }

        private bool _isOkClose;
        protected void Ok_Click()
        {
            Ok_Click(true);
        }

        public bool SaveAfterCancel()
        {
            return Cancel_Click();
        }

        protected void Ok_Click(bool closeDialog)
        {
            SaveData(closeDialog);
        }

        private void SetFormForEditing()
        {
            if (Insert)
            {
                _showOption = ShowOptionsEditForm.Edit;
                _editingInInsertForm = true;
            }

            ReloadEditingObject(out _isEditAllowed);
            _wasChangedValues = false;
            _wasChangedValuesOnlyInDatabase = false;
        }

        protected void ResetWasChangedValues()
        {
            _wasChangedValues = false;
            _wasChangedValuesOnlyInDatabase = false;
        }

        protected abstract bool CheckValues();

        private bool SaveToDatabase()
        {
            ConnectionLost();

            if (GetValues())
            {
                if (Insert)
                {
                    try
                    {
                        ICollection<int> structuredSubSiteIds;
                        var selectSubSitesEnabled = SelectSubSitesEnabled(_editingObject);

                        if (selectSubSitesEnabled)
                        {
                            if (!SelectSubSiteImplicit(_editingObject, out structuredSubSiteIds))
                            {
                                if (!SelectStructuredSubSite(
                                    false,
                                    out structuredSubSiteIds))
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            structuredSubSiteIds = null;
                        }

                        var retValue = SaveToDatabaseInsert();
                        
                        if (retValue)
                        {
                            InsertStructuredSubSiteObject(
                                _editingObject.GetObjectType(),
                                _editingObject.GetIdString(),
                                _editingObject.GetObjectType() == ObjectType.Login ||
                                _editingObject.GetObjectType() == ObjectType.LoginGroup,
                                structuredSubSiteIds);
                        }

                        return retValue;
                    }
                    catch (Exception error)
                    {
                        if (error is AccessDeniedException)
                        {
                            Dialog.Error(GetString("ErrorInsertAccessDenied"));
                        }
                        else
                        {
                            HandledExceptionAdapter.Examine(error);
                            Dialog.Error(GetString("ErrorInsertFailed"));
                        }

                        return false;
                    }
                }
                try
                {
                    if (_wasChangedValues)
                        return SaveToDatabaseEdit();
                    return SaveToDatabaseEditOnlyInDatabase();
                }
                catch (Exception error)
                {
                    if (error is IncoherentDataException)
                    {
                        if (Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            ReloadEditingObject(out _isEditAllowed);
                            if (!_isEditAllowed)
                            {
                                _showOption = ShowOptionsEditForm.View;
                            }
                            SetValues();
                        }
                        else
                        {
                            try
                            {
                                //bool allowEdit;
                                //ReloadEditingObject(out allowEdit);

                                //if (!allowEdit)
                                //{
                                //    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorEditAccessDenied"));
                                //}
                                InternalReloadEditingObjectWithEditedData();
                            }
                            catch(Exception err)
                            {
                                HandledExceptionAdapter.Examine(err);
                                Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                            }
                        }
                    }
                    else if (error is AccessDeniedException)
                    {
                        Dialog.Error(GetString("ErrorEditAccessDenied"));
                    }
                    else
                    {
                        HandledExceptionAdapter.Examine(error);
                        Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                    }

                    return false;
                }
            }
            return false;
        }

        protected virtual bool SelectSubSiteImplicit(T insertedObject, out ICollection<int> selectedSubSites)
        {
            selectedSubSites = null;
            return false;
        }

        protected abstract bool GetValues();

        protected abstract bool SaveToDatabaseInsert();

        protected abstract bool SaveToDatabaseEdit();

        protected virtual bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEdit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);

            if (_showOption == ShowOptionsEditForm.Insert || _showOption == ShowOptionsEditForm.InsertWithData)
            {
                AfterInsert();
                if (_isOkClose && _doAfterInserted != null)
                {
                    try
                    {
                        _doAfterInserted(_editingObject);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }

                if (_doAfterInsertClosed != null)
                {
                    _doAfterInsertClosed();
                }
            }
            else if (_showOption == ShowOptionsEditForm.Edit)
            {
                AfterEdit();
                try
                {
                    if (_doAfterEdited != null)
                        _doAfterEdited(_editingObject);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
            else if (_showOption == ShowOptionsEditForm.View)
            {
                AfterEdit();
            }

            if (_isOkClose)
            {
                CgpClientMainForm.Singleton.AddToRecentList(_editingObject);
            }

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);

            if (!Insert)
                EditEnd();
            if (FormBorderStyle == FormBorderStyle.Sizable)
            {
                CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
            }

            ColorSettingsChangedHandler.Singleton.UnregisterColorChanged(_eventColorChanged);

            if (!Insert)
                UnregisterEvents();
        }

        public void EditUnregisterEvents()
        {
            UnregisterEvents();
        }

        protected abstract void UnregisterEvents();

        protected abstract void EditEnd();

        protected abstract void AfterInsert();

        protected abstract void AfterEdit();

        void IExtendedCgpEditForm.ExtendedEditTextChanger(
            object sender,
            EventArgs e)
        {
            EditTextChanger(
                sender,
                e);
        }

        protected virtual void EditTextChanger(object sender, EventArgs e)
        {
            if (_isSetValues && !_wasChangedValues)
            {
                CgpClientMainForm.Singleton.SetTextOpenWindow(this, true);
                this.Text = "● " + this.Text;
            }
            _wasChangedValues = true;
        }

        void IExtendedCgpEditForm.ExtendedEditTextChangerOnlyInDatabase(
            object sender,
            EventArgs e)
        {
            EditTextChangerOnlyInDatabase(
                sender,
                e);
        }

        protected virtual void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            _wasChangedValuesOnlyInDatabase = true;
        }

        protected void ConnectionLost()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                Close();
        }

        public bool ShowInsertDialog(ref T outObj)
        {
            if (_showOption != ShowOptionsEditForm.InsertDialog)
                return false;
            if (FormOnEnter != null)
            {
                FormOnEnter(this);
            }
            ShowDialog();

            if (_isOkClose)
            {
                outObj = _editingObject;
                return true;
            }

            return false;
        }

        protected void TabControlNextPage(TabControl tabCon, MouseEventArgs e)
        {
            try
            {
                var i = tabCon.SelectedIndex;

                if (e.Delta < 0)
                {
                    i++;
                    if (tabCon.TabPages.Count <= i) //- 1 
                    {
                        return;
                    }
                }
                else
                {
                    i--;
                    if (i < 0)
                    {
                        return;
                    }
                }
                tabCon.SelectedIndex = i;
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        #region Panel Dock/Maximaze
        protected override void OnSizeChanged(EventArgs e)
        {
            if (MdiParent != null && WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                Dock = DockStyle.Fill;
            }
            if (MdiParent != null && WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            base.OnSizeChanged(e);
        }

        protected virtual void AfterDockUndock()
        {
        }

        private void ColorSettingsChanged()
        {
            SetReferenceEditColors();
        }

        #endregion

        #region ICgpEditForm Members

        object ICgpEditForm.GetEditingObject()
        {
            return _editingObject;
        }

        public T GetEditingObject()
        {
            return _editingObject;
        }

        public ShowOptionsEditForm ShowOption
        {
            get
            {
                return (ShowOptionsEditForm)((int)_showOption);
            }
        }

   
        #endregion

        #region UserFolders

        public void UserFolders_Enter(T obj, ImageListBox ilbUsersFolders)
        {
            ObjectsPlacementHandler.UserFolders_Enter(ilbUsersFolders, obj);
        }

        public void UserFolders_Enter(ImageListBox ilbUsersFolders)
        {
            UserFolders_Enter(_editingObject, ilbUsersFolders);
        }

        public void UserFolders_MouseDoubleClick(ImageListBox ilbUsersFolders)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(ilbUsersFolders, _editingObject);
        }

        #endregion

        protected void ControlMouseWheel(object sender, MouseEventArgs e)
        {
            var imagineContrl = (Control)sender;

            if (imagineContrl is ComboBox)
            {
                if ((imagineContrl as ComboBox).DroppedDown)
                    return;
            }

            if (!(e.X > 0 && e.Y > 0 &&
                e.X < imagineContrl.Width &&
                e.Y < imagineContrl.Height))
            {
                var hme = e as HandledMouseEventArgs;
                if (hme != null)
                    hme.Handled = true;

                var screen = PointToClient(new Point(e.X, e.Y));
                var newE = new MouseEventArgs(e.Button, e.Clicks, screen.X, screen.Y, e.Delta);
                _whTabConrol.Focus();
                EditFormMouseWheel(this, newE);
            }
        }
    }
}
