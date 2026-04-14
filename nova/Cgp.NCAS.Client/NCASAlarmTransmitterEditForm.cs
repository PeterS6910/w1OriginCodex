using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Net;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using System.Drawing;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAlarmTransmitterEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditFormWithAlarmInstructions<AlarmTransmitter>
#endif
    {
        private OnlineState _onlineState = OnlineState.Unknown;

        private OnlineState OnlineState
        {
            set
            {
                _onlineState = value;
                ShowOnlineState();
            }
        }

        private void ShowOnlineState()
        {
            _eOnlineState.Text = GetString(
                _onlineState.ToString());

            switch (_onlineState)
            {
                case OnlineState.Online:
                    _eOnlineState.BackColor = Color.LightGreen;
                    break;
                case OnlineState.Offline:
                    _eOnlineState.BackColor = Color.Red;
                    break;
                default:
                    _eOnlineState.BackColor = Color.Yellow;
                    break;
            }
        }

        public NCASAlarmTransmitterEditForm(
            AlarmTransmitter alarmTransmitter,
            ShowOptionsEditForm showOption,
            PluginMainForm<NCASClient> myTableForm)
            : base(
                alarmTransmitter,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            WheelTabContorol = _tcAlarmTransmitter;
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
                Text = GetString("NCASAlarmTransmitterEditFormInsertText");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);

            ShowOnlineState();
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
                _eIpAddress.Enabled = admin;
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
                    _tcAlarmTransmitter.TabPages.Remove(_tpObjectPlacement);
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
                    _tcAlarmTransmitter.TabPages.Remove(_tpReferencedBy);
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
                    _tcAlarmTransmitter.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        protected override void RegisterEvents()
        {
            AlarmTransmitterOnlineStateChangedHandler.Singleton.RegisterOnlineStateChanged(OnlineStateChanged);
        }

        protected override void UnregisterEvents()
        {
            AlarmTransmitterOnlineStateChangedHandler.Singleton.UnregisterOnlineStateChanged(OnlineStateChanged);
        }

        private void OnlineStateChanged(
            string ipAddress,
            OnlineState onlineState)
        {
            if (!ipAddress.Equals(_editingObject.IpAddress))
                return;

            if (InvokeRequired)
            {
                Invoke(
                    new Action(
                        () =>
                            OnlineState = onlineState));
            }
        }

        protected override void BeforeInsert()
        {
            NCASAlarmTransmittersForm.Singleton.BeforeInsert(this);
        }

        protected override void AfterInsert()
        {
            NCASAlarmTransmittersForm.Singleton.AfterInsert();
        }

        protected override void BeforeEdit()
        {
            NCASAlarmTransmittersForm.Singleton.BeforeEdit(
                this,
                _editingObject);
        }

        protected override void AfterEdit()
        {
            NCASAlarmTransmittersForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj =
                Plugin.MainServerProvider.AlarmTransmitters.GetObjectForEdit(
                    _editingObject.IdAlarmTransmitter,
                    out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj =
                        Plugin.MainServerProvider.AlarmTransmitters.GetObjectById(
                            _editingObject.IdAlarmTransmitter);

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
            Plugin.MainServerProvider.AlarmTransmitters.RenewObjectForEdit(
                _editingObject.IdAlarmTransmitter,
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
            _eIpAddress.Text = _editingObject.IpAddress;
            _eDescription.Text = _editingObject.Description;
            OnlineState = Plugin.MainServerProvider.AlarmTransmitters.GetOnlineState(
                _editingObject.IpAddress);

            _bApply.Enabled = false;
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(
                new ReferencedByForm(
                    GetListReferencedObjects,
                    NCASClient.LocalizationHelper,
                    ObjectImageList.Singleton.GetAllObjectImages()));
        }

        private IList<AOrmObject> GetListReferencedObjects()
        {
            return
                Plugin.MainServerProvider.AlarmTransmitters
                    .GetReferencedObjects(
                        _editingObject.IdAlarmTransmitter,
                        CgpClient.Singleton.GetListLoadedPlugins());
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eName,
                    GetString("ErrorEntryAlarmTransmitterName"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                _eName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(_eIpAddress.Text))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eIpAddress,
                    GetString("ErrorEntryAlarmTransmitterIpAddress"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                _eIpAddress.Focus();
                return false;
            }

            if (!IPHelper.IsValid4(_eIpAddress.Text))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eIpAddress,
                    GetString("ErrorNotValidIpAddress"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            return true;
        }

        private void SetNameByIpAddress()
        {
            if (Insert)
                return;

            string oldIpAddress = _editingObject.IpAddress ?? string.Empty;

            if (_eIpAddress.Text != oldIpAddress
                && _eName.Text.Contains(oldIpAddress))
            {
                _eName.Text = oldIpAddress == string.Empty 
                    ? _eName.Text = string.Format("AT {0}", _eIpAddress.Text)
                    : _eName.Text.Replace(oldIpAddress, _eIpAddress.Text);
            }
        }

        protected override bool GetValues()
        {
            SetNameByIpAddress();
            
            _editingObject.Name = _eName.Text;
            _editingObject.IpAddress = _eIpAddress.Text;
            _editingObject.Description = _eDescription.Text;
            _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;

            var retValue = Plugin.MainServerProvider.AlarmTransmitters.Insert(
                ref _editingObject,
                out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(AlarmTransmitter.COLUMN_NAME))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedAlarmTransmitterName"),
                            ControlNotificationSettings.Default);

                        _eName.Focus();
                        return false;
                    }

                    if (error.Message.Contains(AlarmTransmitter.COLUMN_IP_ADDRESS))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eIpAddress,
                            GetString("ErrorUsedAlarmTransmitterIpAddress"),
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
                    ? Plugin.MainServerProvider.AlarmTransmitters.UpdateOnlyInDatabase(
                        _editingObject,
                        out error)
                    : Plugin.MainServerProvider.AlarmTransmitters.Update(
                        _editingObject,
                        out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(AlarmTransmitter.COLUMN_NAME))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedAlarmTransmitterName"),
                            ControlNotificationSettings.Default);

                        _eName.Focus();
                        return false;
                    }

                    if (error.Message.Contains(AlarmTransmitter.COLUMN_IP_ADDRESS))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eIpAddress,
                            GetString("ErrorUsedAlarmTransmitterIpAddress"),
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
                Plugin.MainServerProvider.AlarmTransmitters.EditEnd(_editingObject);
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.AlarmTransmittersLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.AlarmTransmittersLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

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
            else if (sender == _eIpAddress)
            {
                _eName.Text = string.Format("AT {0}", _eIpAddress.Text);
            }
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
            SetNameByIpAddress();

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
