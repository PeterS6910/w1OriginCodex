using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Cgp.Components;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.UI;
using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using ComponentFactory.Krypton.Toolkit;

namespace Contal.Cgp.Client.PluginSupport
{
    /// <summary>
    /// 	Generic abstract class for Plugin Edit Form classes.
    /// </summary>
    public abstract class ACgpPluginEditForm<TCgpVisualPlugin, TObject> :
        PluginMainForm<TCgpVisualPlugin>, 
        ICgpEditForm,
        IEditFormBase
        where TCgpVisualPlugin : ICgpVisualPlugin
        where TObject : AOrmObject
    {
        protected TObject _editingObject;
        private ShowOptionsEditForm _showOption;
        private bool _wasChangedValues;
        private bool _wasChangedValuesOnlyInDatabase;
        private bool _isLockChanges;
        private bool _isSetValues;
        private readonly ICgpClientMainForm _cgpClientMainForm;
        private readonly PluginMainForm<TCgpVisualPlugin> _myTableForm;
        private Action<object> _doAfterInserted;
        private readonly DVoid2Void _eventColorChanged;
        private readonly MdiClient _mdiClient;
        private readonly int _borderWidth;
        private readonly int _borderHeight;
        private SizeF _scaleF = new SizeF(1.0F, 1.0F); // DPI 100%

        public void ShowAndRunSetValues()
        {
            Show();
            SetValues();
        }

        public Action<object> DoAfterInserted
        {
            set { _doAfterInserted = value; }
        }

        public override bool IsEditForm()
        {
            return true;
        }

        protected bool Insert
        {
            get
            {
                return _showOption == ShowOptionsEditForm.Insert || _showOption == ShowOptionsEditForm.InsertDialog ||
                       _showOption == ShowOptionsEditForm.InsertWithData;
            }
        }

        protected bool IsSetValues
        {
            get { return _isSetValues; }
        }

        public bool ValueChanged
        {
            get { return _wasChangedValues; }
        }

        public bool DisableFormForViewMode { get; set; }

        public bool ValueChangedOnlyDatabase
        {
            get { return _wasChangedValuesOnlyInDatabase; }
        }

        public override TCgpVisualPlugin Plugin
        {
            get { return _myTableForm.Plugin; }
        }

        protected ACgpPluginEditForm(
            TObject editingObject,
            ShowOptionsEditForm showOption,
            ICgpClientMainForm cgpClientMainForm,
            PluginMainForm<TCgpVisualPlugin> myTableForm,
            LocalizationHelper localizationHelper)
            : base(localizationHelper, cgpClientMainForm)
        {
            SuspendLayout();
            //default ControlBox settings
            ControlBox = true;
            MinimizeBox = false;
            DisableFormForViewMode = true;

            _editingObject = editingObject;
            _showOption = showOption;
            _cgpClientMainForm = cgpClientMainForm;
            _myTableForm = myTableForm;

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                MdiParent = (Form) _cgpClientMainForm;

            FormOnEnter += EditForm_Enter;

            // At design-time, each ContainerControl records the scaling mode and
            // its current resolution in the AutoScaleMode and AutoScaleDimensions.
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;

            _eventColorChanged = ColorSettingsChanged;
            _cgpClientMainForm.RegisterColorChanged(_eventColorChanged);
            MouseWheel += EditFormMouseWheel;

            _mdiClient = GetMdiClientWindow();

            _borderWidth = Width - ClientSize.Width;
            _borderHeight = Height - ClientSize.Height;
            Move += ACgpPluginEditForm_Move;
            KeyPreview = true;
            KeyDown += ACgpPluginEditForm_KeyDown;

            // Create Dock/Undock button (all Windows versions)
            ButtonSpecAny btnSpecDockUndock = new ButtonSpecAny()
            {
                UniqueName = "btnSpecDockUndock",
                Type = PaletteButtonSpecStyle.FormRestore,
                Style = PaletteButtonStyle.Form,
                ToolTipTitle = "Dock/Undock"
            };
            btnSpecDockUndock.Click += _tbbMenu_ButtonClicked;
            ButtonSpecs.Add(btnSpecDockUndock);

            if (!Insert)
                RegisterEvents();

            ResumeLayout();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == DllUser32.WM_SYSCOMMAND)
            {
                // Check your window state here
                if (m.WParam.ToInt32() == DllUser32.SC_MAXIMIZE)
                {
                    MaximizeMinimize();
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void _tbbMenu_ButtonClicked(object sender, EventArgs e)
        {
            DockUndock();
        }

        private void DockUndock()
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
            MdiParent = (Form) _cgpClientMainForm;
            FormBorderStyle = FormBorderStyle.Sizable;
            Dock = DockStyle.None;
            _cgpClientMainForm.AddToOpenWindows(this, _editingObject.ToString());
            _cgpClientMainForm.SetActOpenWindow(this);
            _cgpClientMainForm.RemoveFormUndockedForms(this);

            ButtonSpecs["btnSpecDockUndock"].Type = PaletteButtonSpecStyle.FormRestore;
            MinimizeBox = false;
        }

        public void UndockWindow()
        {
            SuspendLayout();
            Form parent = MdiParent;
            MdiParent = null;
            if (parent != null)
            {
                Location = Screen.FromControl(parent).WorkingArea.Location;
            }
            FormBorderStyle = FormBorderStyle.Sizable;
            _cgpClientMainForm.RemoveFromOpenWindows(this);
            _cgpClientMainForm.AddToUndockedForms(this);

            ButtonSpecs["btnSpecDockUndock"].Type = PaletteButtonSpecStyle.RibbonExpand;
            MinimizeBox = true;

            ResumeLayout(true);
        }

        private void MaximizeMinimize()
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
                    // minimize
                    Dock = DockStyle.None;
                    Form parentForm = ParentForm;
                    if (parentForm != null)
                        parentForm.LayoutMdi(MdiLayout.Cascade);
                }
                else
                {
                    // maximize
                    Dock = DockStyle.Fill;
                }
            }
        }

        protected abstract void RegisterEvents();

        private void ACgpPluginEditForm_KeyDown(object sender, KeyEventArgs e)
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

        private void ACgpPluginEditForm_Move(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                if ((Left - _borderWidth <= 0) //left
                    || (Right + _borderWidth >= GetMdiAreaSize(_mdiClient, -4, -4).Width) //right
                    || (Top - _borderHeight <= 0) //top
                    || (Bottom + _borderHeight >= ((GetMdiAreaSize(_mdiClient, -4, -4).Height)))) //bottom
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

        protected TabControl _whTabConrol = null;

        private void EditFormMouseWheel(object sender, MouseEventArgs e)
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

        protected void ControlMouseWheel(object sender, MouseEventArgs e)
        {
            var imagineContrl = (Control) sender;

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

                Point screen = PointToClient(new Point(e.X, e.Y));
                var newE = new MouseEventArgs(e.Button, e.Clicks, screen.X, screen.Y, e.Delta);
                _whTabConrol.Focus();
                EditFormMouseWheel(this, newE);
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
                if (_showOption == ShowOptionsEditForm.Insert)
                {
                    BeforeInsert();
                    _cgpClientMainForm.AddToOpenWindows(this);
                }
                else if (_showOption == ShowOptionsEditForm.Edit 
                    || _showOption == ShowOptionsEditForm.View)
                {
                    BeforeEdit();
                    _cgpClientMainForm.AddToOpenWindows(this, _editingObject.ToString());
                }

                SetValues();
                _wasChangedValues = false;
                _wasChangedValuesOnlyInDatabase = false;
                _isLockChanges = false;
                _isSetValues = true;
            }

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                _cgpClientMainForm.SetActOpenWindow(this);

            AfterFormEnter();
        }

        protected virtual void AfterFormEnter()
        {
        }

        private bool _isEditAllowed;

        protected bool IsEditAllowed
        {
            get { return _isEditAllowed; }
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
                    if (_showOption == ShowOptionsEditForm.View
                        && DisableFormForViewMode)
                    {
                        DisableForm();
                    }
                }
            }
            catch
            {
                Dialog.Error(
                    _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorSetValuesFailed"));
            }
        }

        protected virtual void CreateAlarmInstructionsTabPage()
        {
        }

        public abstract void ReloadEditingObject(out bool allowEdit);
        public abstract void ReloadEditingObjectWithEditedData();

        protected abstract void SetValuesInsert();

        protected abstract void SetValuesEdit();

        protected virtual void DisableForm()
        {
            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    DisabledControls(control);
                }
            }
        }

        protected void EnabledControls(Control control)
        {
            if (control is ElementHost)
                return;

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

        protected void DisabledControls(Control control)
        {
            if (control is ElementHost)
                return;

            if (control.Controls == null || control.Controls.Count == 0)
                DisabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    DisabledControls(actControl);
        }

        private static void DisabledControl(Control control)
        {
            if (!(control is Label))
            {
                control.Enabled = false;
            }
        }

        protected bool Cancel_Click()
        {
            if (_wasChangedValues || _wasChangedValuesOnlyInDatabase)
            {
                BringToFront();
                DialogResult result =
                    MessageBox.Show(GetString("ValueChanged") + "\n" + GetString("SaveAfterCancel"),
                        GetString("Question"),
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                switch (result)
                {
                    case DialogResult.Cancel:
                    {
                        return false;
                    }
                    case DialogResult.No:
                    {
                        ConnectionLost();
                        Close();
                        return true;
                    }
                    case DialogResult.Yes:
                    {
                        return SaveData();
                    }
                    default:
                        return false;
                }
            }
            ConnectionLost();
            Close();
            return true;
        }

        public void SetAllowEdit(bool allowEdit)
        {
            _isEditAllowed = allowEdit;
        }

        public bool SaveAfterCancel()
        {
            return Cancel_Click();
        }

        private bool _isOkClose;

        protected void Ok_Click()
        {
            if (!_wasChangedValues && !_wasChangedValuesOnlyInDatabase && !(Insert))
            {
                if (CheckValues())
                {
                    _isOkClose = true;
                    Close();
                }
            }
            else if (CheckValues())
            {
                if (SaveToDatabase())
                {
                    _isOkClose = true;
                    Close();
                }
            }
        }

        protected bool SaveData()
        {
            if (!_wasChangedValues && !_wasChangedValuesOnlyInDatabase && !(Insert))
            {
                if (CheckValues())
                {
                    _isOkClose = true;
                    Close();
                    return true;
                }
            }
            else if (CheckValues())
            {
                if (SaveToDatabase())
                {
                    _isOkClose = true;
                    Close();
                    return true;
                }
            }
            return false;
        }

        protected bool Apply_Click()
        {
            bool applied = false;

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
                            if (!SelectStructuredSubSite(
                                false,
                                out structuredSubSiteIds))
                            {
                                return false;
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
                                false,
                                structuredSubSiteIds);
                        }

                        return retValue;
                    }
                    catch (Exception error)
                    {
                        if (error is AccessDeniedException)
                        {
                            Dialog.Error(
                                _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorInsertAccessDenied"));
                        }
                        else
                        {
                            Dialog.Error(
                                _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorInsertFailed"));
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
                        if (
                            Dialog.WarningQuestion(
                                _cgpClientMainForm.GetLocalizationHelper().GetString("QuestionLoadActualData")))
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
                                ReloadEditingObjectWithEditedData();
                            }
                            catch
                            {
                                Dialog.Error(
                                    _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorEditFailed") + ": " +
                                    error.Message);
                            }
                        }
                    }
                    else if (error is AccessDeniedException)
                    {
                        Dialog.Error(
                            _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorEditAccessDenied"));
                    }
                    else
                    {
                        Dialog.Error(
                            _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorEditFailed") + ": " +
                            error.Message);
                    }

                    return false;
                }
            }
            return false;
        }

        protected abstract bool GetValues();

        protected abstract bool SaveToDatabaseInsert();

        protected abstract bool SaveToDatabaseEdit();

        public virtual void SetValuesInsertFromObj(object obj)
        {
            throw new NotImplementedException();
        }

        protected virtual bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEdit();
        }

        public void LockChanges()
        {
            _isLockChanges = true;
        }

        public void UnlockChanges()
        {
            _isLockChanges = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);

            if (_showOption == ShowOptionsEditForm.Insert)
            {
                AfterInsert();
                if (_isOkClose && _doAfterInserted != null)
                {
                    try
                    {
                        _doAfterInserted(_editingObject);
                    }
                    catch
                    {
                    }
                }
            }
            else if (_showOption == ShowOptionsEditForm.Edit || _showOption == ShowOptionsEditForm.View)
                AfterEdit();

            if (_isOkClose)
            {
                _cgpClientMainForm.AddToRecentList(_editingObject, _myTableForm, true);
            }

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                _cgpClientMainForm.RemoveFromOpenWindows(this);

            if (!Insert)
            {
                //try catch is here to catch exception when server is down
                try
                {
                    EditEnd();
                }
                catch
                {
                }
            }
            if (FormBorderStyle == FormBorderStyle.Sizable)
            {
                _cgpClientMainForm.RemoveFormUndockedForms(this);
            }

            _cgpClientMainForm.UnregisterColorChanged(_eventColorChanged);

            if (!Insert)
                UnregisterEvents();

            e.Cancel = false;
        }

        public void EditUnregisterEvents()
        {
            UnregisterEvents();
        }

        protected abstract void UnregisterEvents();

        protected abstract void EditEnd();

        protected abstract void AfterInsert();

        protected abstract void AfterEdit();

        protected virtual void EditTextChanger(object sender, EventArgs e)
        {
            if (!_isLockChanges && _isSetValues && !_wasChangedValues)
            {
                CgpClientMainForm.Singleton.SetTextOpenWindow(this, true);
                this.Text = "● " + this.Text;
            }

            if (!_isLockChanges)
                _wasChangedValues = true;
        }

        protected virtual void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            if (!_isLockChanges)
                _wasChangedValuesOnlyInDatabase = true;
        }

        public bool ShowInsertDialog(ref TObject outObj)
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

        protected void ConnectionLost()
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true))
                Close();
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;
            if (gridView.Columns.Contains(columnName))
                gridView.Columns[columnName].Visible = false;
        }

        protected void TabControlNextPage(TabControl tabCon, MouseEventArgs e)
        {
            int i = tabCon.SelectedIndex;

            if (e.Delta < 0)
            {
                i++;
                if (tabCon.TabPages.Count <= i)
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

        #region Panel Dock/Maximize

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
            //SetReferenceEditColors(this);
            SetReferenceEditColors();
        }

        //public void SetReferenceEditColors()
        //{
        //    SetReferenceEditColors(this);
        //}

        //private void SetReferenceEditColors(Control mainControl)
        //{
        //    foreach (Control cnt in mainControl.Controls)
        //    {
        //        if (cnt is TextBox || cnt is ListBox || cnt is ImageTextBox)
        //        {
        //            DoAdd(cnt);
        //        }
        //        else if (cnt is TextBoxMenu)
        //        {
        //            DoAdd((cnt as TextBoxMenu));
        //        }
        //        if ((cnt.Controls != null) && (!(cnt is TextBoxMenu)))
        //            SetReferenceEditColors(cnt);
        //    }
        //}

        //private void DoAdd(Control control)
        //{
        //    if (InvokeRequired)
        //    {
        //        BeginInvoke(new Action<Control>(DoAdd), control);
        //    }
        //    else
        //    {
        //        if (control.AllowDrop)
        //        {
        //            var menu = control as TextBoxMenu;
        //            if (menu != null)
        //            {
        //                menu.ImageTextBox.ForeColor = _cgpClientMainForm.GetDragDropTextColor;
        //                menu.ImageTextBox.BackColor = _cgpClientMainForm.GetDragDropBackgroundColor;
        //            }
        //            else
        //            {
        //                control.ForeColor = _cgpClientMainForm.GetDragDropTextColor;
        //                control.BackColor = _cgpClientMainForm.GetDragDropBackgroundColor;
        //            }
        //        }
        //        else if (control.Tag != null && control.Tag.ToString() == "Reference")
        //        {
        //            control.ForeColor = _cgpClientMainForm.GetReferenceTextColor;
        //            control.BackColor = _cgpClientMainForm.GetReferenceBackgroundColor;
        //        }
        //    }
        //}

        #endregion

        #region ICgpEditForm Members

        object ICgpEditForm.GetEditingObject()
        {
            return _editingObject;
        }

        public TObject GetEditingObject()
        {
            return _editingObject;
        }

        public ShowOptionsEditForm ShowOption
        {
            get
            {
                return _showOption;
            }
            protected set { _showOption = value; }
        }

        public bool? AllowEdit
        {
            get { return _isEditAllowed; }
            set { _isEditAllowed = true; }
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
// 
// ACgpPluginEditForm
// 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ACgpPluginEditForm";
            this.ResumeLayout(false);

        }
    }
}
