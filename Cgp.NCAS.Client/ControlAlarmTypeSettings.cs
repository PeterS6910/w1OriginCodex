using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class ControlAlarmTypeSettings : UserControl
    {
        public NCASClient Plugin { get; set; }

        public bool ThreeStateCheckBoxes
        {
            set
            {
                _chbAlarmEnabled.ThreeState = value;
                _chbBlockAlarm.ThreeState = value;
                _chbEventlogDuringBlockedAlarm.ThreeState = value;
            }
        }

        public Action EditTextChanger;

        public Action EditTextChangerOnlyInDatabase;

        public bool AlarmEnabledChecked
        {
            get { return _chbAlarmEnabled.Checked; }
            set { _chbAlarmEnabled.Checked = value; }
        }

        public bool? AlarmEnabledCheckState
        {
            get { return ConvertFromCheckState(_chbAlarmEnabled.CheckState); }
            set { _chbAlarmEnabled.CheckState = ConvertToCheckState(value); }
        }

        private bool? ConvertFromCheckState(CheckState checkState)
        {
            switch (checkState)
            {
                case CheckState.Checked:
                    return true;

                case CheckState.Unchecked:
                    return false;

                default:
                    return null;
            }
        }

        private CheckState ConvertToCheckState(bool? checkState)
        {
            if (checkState == null)
                return CheckState.Indeterminate;

            return checkState.Value
                ? CheckState.Checked
                : CheckState.Unchecked;
        }

        public bool AlarmEnableVisible
        {
            set
            {
                _pAlarmEnabled.Visible = value;
                _pPresentationGroup.Top += value 
                    ? _pAlarmEnabled.Height 
                    : -_pAlarmEnabled.Height;
            }
        }

        public bool BlockAlarmVisible
        {
            set
            {
                _pBlockAlarm.Visible = value;
                _pEventlogDuringBlockedAlarm.Visible = value;
                _pObjectForBlockingAlarm.Visible = value;

                _pPresentationGroup.Width = !value
                    ? Width
                    : _pObjectForBlockingAlarm.Left;

                _pPresentationGroup.Anchor = !value
                    ? AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                    : AnchorStyles.Top | AnchorStyles.Left;
            }
        }

        public bool BlockAlarmChecked
        {
            get { return _chbBlockAlarm.Checked; }
            set { _chbBlockAlarm.Checked = value; }
        }

        public bool? BlockAlarmCheckState
        {
            get { return ConvertFromCheckState(_chbBlockAlarm.CheckState); }
            set { _chbBlockAlarm.CheckState = ConvertToCheckState(value); }
        }

        private AOnOffObject _objectForBlockingAlarm;

        public AOnOffObject ObjectForBlockingAlarm
        {
            get { return _objectForBlockingAlarm; }
            set
            {
                _objectForBlockingAlarm = value;
                ShowObjectForBlockingAlarm();

                if (EditTextChanger != null)
                    EditTextChanger();
            }
        }

        public bool EventlogDuringBlockedAlarmVisible
        {
            set
            {
                if (value)
                    ShowControls(_pEventlogDuringBlockedAlarm.Controls);
                else
                    HideControls(_pEventlogDuringBlockedAlarm.Controls);
            }
        }

        private void ShowControls(ControlCollection controls)
        {
            if (controls == null)
                return;

            foreach (Control control in controls)
            {
                control.Visible = true;
            }
        }

        private void HideControls(ControlCollection controls)
        {
            if (controls == null)
                return;

            foreach (Control control in controls)
            {
                control.Visible = false;
            }
        }

        public bool EventlogDuringBlockedAlarmChecked
        {
            get { return _chbEventlogDuringBlockedAlarm.Checked; }
            set { _chbEventlogDuringBlockedAlarm.Checked = value; }
        }

        public bool? EventlogDuringBlockedAlarmCheckState
        {
            get { return ConvertFromCheckState(_chbEventlogDuringBlockedAlarm.CheckState); }
            set { _chbEventlogDuringBlockedAlarm.CheckState = ConvertToCheckState(value); }
        }

        public bool PresentationGroupVisible
        {
            set
            {
                _pPresentationGroup.Visible = value;

                if (!value)
                {
                    _pAlarmEnabled.Width = _chbAlarmEnabled.MaximumSize.Width + 2*_chbAlarmEnabled.Left;
                    
                    _pBlockAlarm.Left = _pAlarmEnabled.Right;
                    
                    _pEventlogDuringBlockedAlarm.Left = _pBlockAlarm.Right;
                    _pEventlogDuringBlockedAlarm.Width = Width - _pEventlogDuringBlockedAlarm.Left;

                    _pObjectForBlockingAlarm.Left = 0;
                    _pObjectForBlockingAlarm.Width = Width;
                }
            }
        }

        private PresentationGroup _presentationGroup;

        public PresentationGroup PresentationGroup
        {
            get { return _presentationGroup; }
            set
            {
                _presentationGroup = value;
                ShowPresentationGroup();

                if (EditTextChangerOnlyInDatabase != null)
                    EditTextChangerOnlyInDatabase();
            }
        }

        private Guid? _idParentCcu;

        public ControlAlarmTypeSettings()
        {
            InitializeComponent();

            _tbmObjectForBlockingAlarm.MaximumSize = new Size(0, 0);

            _tbmObjectForBlockingAlarm.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmObjectForBlockingAlarm.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;

            _tbmPresentationGroup.MaximumSize = new Size(0, 0);

            _tbmPresentationGroup.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmPresentationGroup.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;

            FocusOnClick(this);
        }

        private void FocusOnClick(Control control)
        {
            control.Click +=
                (sender, args) =>
                    Focus();

            if (control.Controls == null)
                return;

            foreach (Control childControl in control.Controls)
            {
                FocusOnClick(childControl);
            }
        }

        private void ShowObjectForBlockingAlarm()
        {
            if (_objectForBlockingAlarm == null)
            {
                _tbmObjectForBlockingAlarm.Text = string.Empty;
                return;
            }

            _tbmObjectForBlockingAlarm.Text = _objectForBlockingAlarm.ToString();
            
            if (Plugin != null)
                _tbmObjectForBlockingAlarm.TextImage = Plugin.GetImageForAOrmObject(_objectForBlockingAlarm);
        }

        private void ShowPresentationGroup()
        {
            if (_presentationGroup == null)
            {
                _tbmPresentationGroup.Text = string.Empty;
                return;
            }

            _tbmPresentationGroup.Text = _presentationGroup.ToString();
            
            if (Plugin != null)
                _tbmPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_presentationGroup);
        }

        private void DoEditTextChanger(object sender, EventArgs e)
        {
            DoEditTextChanger();
        }
        
        private void DoEditTextChanger()
        {
            if (EditTextChanger != null)
                EditTextChanger();
        }

        private void DoDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        public void AddControl(Control control)
        {
            if (control == null)
                return;

            Controls.Add(control);

            control.Location = new Point(0, Height);
            Height += control.Height;
            control.Width = Width;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            FocusOnClick(control);
        }

        public void SetParentCcu(Guid idCcu)
        {
            _idParentCcu = idCcu;
        }

        private void _tbmObjectForBlockingAlarm_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify1)
            {
                ModifyObjectForBlockingAlarm();
                return;
            }

            ObjectForBlockingAlarm = null;
        }

        private void ModifyObjectForBlockingAlarm()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            try
            {
                Exception error;
              
                var dailyPlans =
                    CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);

                if (error != null)
                    throw error;

                var timeZones =
                    CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);

                IList<IModifyObject> inputs = null;
                IList<IModifyObject> outputs = null;
                
                if (_idParentCcu != null)
                {
                    inputs = Plugin.MainServerProvider.Inputs.ListModifyObjectsFromCCU(
                        _idParentCcu.Value,
                        out error);

                    outputs = Plugin.MainServerProvider.Outputs.ListModifyObjectsFromCCU(
                        _idParentCcu.Value,
                        out error);
                }

                if (error != null)
                    throw error;

                var formAdd = new ListboxFormAdd(
                    (dailyPlans ?? Enumerable.Empty<IModifyObject>())
                        .Concat(timeZones ?? Enumerable.Empty<IModifyObject>())
                        .Concat(inputs ?? Enumerable.Empty<IModifyObject>())
                        .Concat(outputs ?? Enumerable.Empty<IModifyObject>()),
                    Plugin != null
                        ? Plugin.GetTranslateString("NCASCCUEditFormBlockObjects")
                        : string.Empty);


                object selectedObject;
                formAdd.ShowDialog(out selectedObject);

                var iModifyObject = selectedObject as IModifyObject;
                
                if (iModifyObject != null)
                {
                    var objectForBlockingAlarm = DbsSupport.GetTableObject(
                        iModifyObject.GetOrmObjectType,
                        iModifyObject.GetId) as AOnOffObject;

                    if (objectForBlockingAlarm != null)
                    {
                        ObjectForBlockingAlarm = objectForBlockingAlarm;

                        if (Plugin != null)
                            Plugin.AddToRecentList(objectForBlockingAlarm);
                    }
                }
            }
            catch (Exception error)
            {
                Dialog.Error(error);
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _tbmObjectForBlockingAlarm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddObjectForBlockingAlarm(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddObjectForBlockingAlarm(object newObjectForBlockingAlarm)
        {
            if (newObjectForBlockingAlarm is DailyPlan
                || newObjectForBlockingAlarm is Cgp.Server.Beans.TimeZone)
            {
                ObjectForBlockingAlarm = newObjectForBlockingAlarm as AOnOffObject;

                if (Plugin != null)
                    Plugin.AddToRecentList(newObjectForBlockingAlarm);

                return;
            }

            ControlNotification.Singleton.Error(
                NotificationPriority.JustOne,
                _tbmObjectForBlockingAlarm.ImageTextBox,
                Plugin != null
                    ? Plugin.GetTranslateString("ErrorWrongObjectType")
                    : string.Empty,
                ControlNotificationSettings.Default);
        }

        private void _tbmObjectForBlockingAlarm_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (ObjectForBlockingAlarm != null)
                DbsSupport.OpenEditForm(ObjectForBlockingAlarm);
        }

        private void _tbmPresentationGroup_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify2)
            {
                ModifyPresentationGroup();
                return;
            }

            if (item == _tsiCreate2)
            {
                var presetationGroup = new PresentationGroup();
                PresentationGroupsForm.Singleton.OpenInsertFromEdit(
                    ref presetationGroup,
                    DoAfterCreatedPresentationGroup);

                return;
            }

            PresentationGroup = null;
        }

        private void ModifyPresentationGroup()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            try
            {
                Exception error;

                var presentationGroups =
                    CgpClient.Singleton.MainServerProvider.PresentationGroups.ListModifyObjects(out error);

                if (error != null)
                    throw error;

                var formAdd = new ListboxFormAdd(
                    presentationGroups ?? Enumerable.Empty<IModifyObject>(),
                    CgpClient.Singleton.GetLocalizedString("PresentationGroupsFormPresentationGroupsForm"));


                object selectedObject;
                formAdd.ShowDialog(out selectedObject);

                var iModifyObject = selectedObject as IModifyObject;

                if (iModifyObject != null)
                {
                    var presentationGroup =
                        CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(iModifyObject.GetId);

                    if (presentationGroup != null)
                    {
                        PresentationGroup = presentationGroup;

                        if (Plugin != null)
                            Plugin.AddToRecentList(presentationGroup);
                    }
                }
            }
            catch (Exception error)
            {
                Dialog.Error(error);
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void DoAfterCreatedPresentationGroup(object newPresentationGroup)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(
                    DoAfterCreatedPresentationGroup),
                    newPresentationGroup);

                return;
            }

            var presetnationGroup = newPresentationGroup as PresentationGroup;

            if (presetnationGroup != null)
                PresentationGroup = presetnationGroup;
        }

        private void _tbmPresentationGroup_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddPresentationGroup(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddPresentationGroup(object newPresentationGroup)
        {
            var presentationGroup = newPresentationGroup as PresentationGroup;

            if (presentationGroup == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmPresentationGroup.ImageTextBox,
                    Plugin != null
                        ? Plugin.GetTranslateString("ErrorWrongObjectType")
                        : string.Empty,
                    ControlNotificationSettings.Default);

                return;
            }

            PresentationGroup = presentationGroup;
            
            if (Plugin != null)
                Plugin.AddToRecentList(presentationGroup);
        }

        private void _tbmPresentationGroup_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (PresentationGroup != null)
                PresentationGroupsForm.Singleton.OpenEditForm(PresentationGroup);
        }

        private void _pEventlogDuringBlockedAlarm_SizeChanged(object sender, EventArgs e)
        {
            _chbEventlogDuringBlockedAlarm.MaximumSize = new Size(
                _pEventlogDuringBlockedAlarm.Width - _chbEventlogDuringBlockedAlarm.Left - 5,
                _chbEventlogDuringBlockedAlarm.MaximumSize.Height);
        }

        public bool ControlValues(Action<Control> beforeNotificationError)
        {
            if (_chbBlockAlarm.CheckState != CheckState.Checked
                || ObjectForBlockingAlarm != null)
            {
                return true;
            }

            if (beforeNotificationError != null)
            {
                beforeNotificationError(this);
            }

            ControlNotification.Singleton.Error(
                NotificationPriority.JustOne,
                _tbmObjectForBlockingAlarm.ImageTextBox,
                Plugin != null
                    ? Plugin.GetTranslateString("ErrorEntryBlockAlarmObject")
                    : string.Empty,
                ControlNotificationSettings.Default);

            _tbmObjectForBlockingAlarm.Focus();

            return false;
        }
    }
}
