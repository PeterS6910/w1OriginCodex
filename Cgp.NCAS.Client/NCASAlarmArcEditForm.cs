using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAlarmArcEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditForm<NCASClient, AlarmArc>
#endif
    {
        public NCASAlarmArcEditForm(
            AlarmArc alarmArc,
            ShowOptionsEditForm showOption,
            PluginMainForm<NCASClient> myTableForm)
            : base(
                alarmArc,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcAlarmArc;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            _bApply.Enabled = false;

            SafeThread.StartThread(HideDisableTabPages);
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
                Text = GetString("NCASAlarmArcEditFormInsertText");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        private void HideDisableTabPages()
        {
            try
            {
                DisableSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmArcsSettingsAdmin)));

                HideDisableTabPageObjectPlacement(
                    !Insert
                    && ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    !Insert
                    && CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmArcsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmArcsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmArcsDescriptionAdmin)));
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void DisableSettings(bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageObjectPlacement), admin);
            }
            else
            {
                _eName.Enabled = admin;
            }
        }

        private void HideDisableTabPageObjectPlacement(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageObjectPlacement), view);
            }
            else
            {
                if (!view)
                {
                    _tcAlarmArc.TabPages.Remove(_tpObjectPlacement);
                }
            }
        }

        private void HideDisableTabReferencedBy(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabReferencedBy), view);
            }
            else
            {
                if (!view)
                {
                    _tcAlarmArc.TabPages.Remove(_tpReferencedBy);
                }
            }
        }

        private void HideDisableTabPageDescription(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDescription),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcAlarmArc.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        protected override void RegisterEvents()
        {

        }

        protected override void UnregisterEvents()
        {

        }

        protected override void BeforeInsert()
        {
            NCASAlarmArcsForm.Singleton.BeforeInsert(this);
        }

        protected override void AfterInsert()
        {
            NCASAlarmArcsForm.Singleton.AfterInsert();
        }

        protected override void BeforeEdit()
        {
            NCASAlarmArcsForm.Singleton.BeforeEdit(
                this,
                _editingObject);
        }

        protected override void AfterEdit()
        {
            NCASAlarmArcsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj =
                Plugin.MainServerProvider.AlarmArcs.GetObjectForEdit(
                    _editingObject.IdAlarmArc,
                    out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj =
                        Plugin.MainServerProvider.AlarmArcs.GetObjectById(
                            _editingObject.IdAlarmArc);

                    DisableForm();
                }
                else
                {
                    throw error;
                }
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.AlarmArcs.RenewObjectForEdit(
                _editingObject.IdAlarmArc,
                out error);

            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            
        }

        protected override void SetValuesEdit()
        {
            SetReferencedBy();

            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;

            _bApply.Enabled = false;
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(
                new ReferencedByForm(
                    GetListReferencedObjects,
                    CgpClient.Singleton.LocalizationHelper,
                    ObjectImageList.Singleton.GetAllObjectImages()));
        }

        private IList<AOrmObject> GetListReferencedObjects()
        {
            return
                Plugin.MainServerProvider.AlarmArcs
                    .GetBackReferences(
                        _editingObject.IdAlarmArc);
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eName,
                    GetString("ErrorEntryAlarmArcName"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                _eName.Focus();
                return false;
            }

            return true;
        }

        protected override bool GetValues()
        {
            _editingObject.Name = _eName.Text;
            _editingObject.Description = _eDescription.Text;

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;

            var retValue = Plugin.MainServerProvider.AlarmArcs.Insert(
                ref _editingObject,
                out error);
            
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(AlarmArc.COLUMN_NAME))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedAlarmArcName"),
                            ControlNotificationSettings.Default);

                        _eName.Focus();
                        return false;
                    }
                }

                throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            return SaveToDatabaseEditCore(false);
        }

        protected override bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEditCore(true);
        }

        private bool SaveToDatabaseEditCore(bool onlyInDatabase)
        {
            Exception error;

            bool retValue =
                onlyInDatabase
                    ? Plugin.MainServerProvider.AlarmArcs.UpdateOnlyInDatabase(
                        _editingObject,
                        out error)
                    : Plugin.MainServerProvider.AlarmArcs.Update(
                        _editingObject,
                        out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(AlarmArc.COLUMN_NAME))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedAlarmArcName"),
                            ControlNotificationSettings.Default);

                        _eName.Focus();
                        return false;
                    }
                }

                throw error;
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null
                && Plugin.MainServerProvider.AlarmTransmitters != null)
            {
                Plugin.MainServerProvider.AlarmArcs.EditEnd(_editingObject);
            }
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _tpObjectPlacement_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }
    }
}
