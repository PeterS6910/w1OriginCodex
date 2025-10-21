using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class CisNGEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<CisNG>
#endif
    {
        public CisNGEditForm(CisNG cisNG, ShowOptionsEditForm showOption)
            : base(cisNG, showOption)
        {
            InitializeComponent();
            SetReferenceEditColors();
            WheelTabContorol = _tcCis;
            InitCisMsgValues();
            if (showOption != ShowOptionsEditForm.Edit)
            {
                _tcCis.TabPages.Remove(_tpSendInfoMessage);
            }
            this.MinimumSize = new Size(this.Width, this.Height);
            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbClass.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _ePort.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _eMessageText.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsSettingsAdmin)));

                HideDisableTabPageSendInfoMessage(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsSendMessageView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsSendMessageAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSettings),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcCis.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageSendInfoMessage(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSendInfoMessage),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCis.TabPages.Remove(_tpSendInfoMessage);
                    return;
                }

                _tpSendInfoMessage.Enabled = admin;
            }
        }

        private void HideDisableTabFoldersSructure(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabFoldersSructure), view);
            }
            else
            {
                if (!view)
                {
                    _tcCis.TabPages.Remove(_tpUserFolders);
                    return;
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
                    _tcCis.TabPages.Remove(_tpReferencedBy);
                    return;
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
                    _tcCis.TabPages.Remove(_tpDescription);
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

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("CisNGEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            CisNGsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            CisNGsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            CisNGsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            CisNGsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            CisNG obj = CgpClient.Singleton.MainServerProvider.CisNGs.GetObjectForEdit(_editingObject.IdCisNG, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.CisNGs.GetObjectById(_editingObject.IdCisNG);
                }
                else
                {
                    throw error;
                }
                DisabledForm();
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
            Exception error;
            CgpClient.Singleton.MainServerProvider.CisNGs.RenewObjectForEdit(_editingObject.IdCisNG, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            _ePort.Value = 63001;
            _eUser.Text = @"#XML";
            _ePassword.Text = @"#XML";

            _lCisNGStatus.Text = "offline";
            _lCisNGStatus.BackColor = Color.Red;
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.CisNGName;
            _eIpAddress.Text = _editingObject.IpAddress;
            if (_editingObject.Port == 0)
                _ePort.Value = 63001;
            else
                _ePort.Value = _editingObject.Port;
            _eUser.Text = _editingObject.UserName;
            _ePassword.Text = _editingObject.Password;

            _lCisNGStatus.Text = string.Empty;
            switch (CgpClient.Singleton.MainServerProvider.CisNGs.ReturnCisNGState(_editingObject))
            {
                case 0: _lCisNGStatus.Text = "offline"; _lCisNGStatus.BackColor = Color.Red; break;
                case 1: _lCisNGStatus.Text = "not authenticated"; _lCisNGStatus.BackColor = Color.Yellow; break;
                case 2: _lCisNGStatus.Text = "online"; _lCisNGStatus.BackColor = Color.LightGreen; break;
            }
            SetReferencedBy();
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.CisNGName = _eName.Text;
                _editingObject.IpAddress = _eIpAddress.Text;
                _editingObject.Port = (int)_ePort.Value;
                _editingObject.UserName = _eUser.Text;
                _editingObject.Description = _eDescription.Text;
                if (Insert)
                {
                    _editingObject.Password = _ePassword.Text;
                }
                else
                {
                    if (_ePassword.Text != string.Empty && _ePassword.Text != _editingObject.Password)
                        _editingObject.Password = _ePassword.Text;
                }

                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();
                return true;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertCisNGName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eIpAddress.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eIpAddress,
                        GetString("ErrorInsertIPAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                if (_tcCis.SelectedTab != _tpSettings)
                {
                    _tcCis.SelectedTab = _tpSettings;
                }
                _eIpAddress.Focus();
                return false;
            }
            else
            {
                if (!Contal.IwQuick.Net.IPHelper.IsValid(_eIpAddress.Text))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eIpAddress,
                            GetString("ErrorInsertValidIPAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                    if (_tcCis.SelectedTab != _tpSettings)
                    {
                        _tcCis.SelectedTab = _tpSettings;
                    }
                    _eIpAddress.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(_ePort.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ePort,
                        GetString("ErrorInsertPort"), CgpClient.Singleton.ClientControlNotificationSettings);
                if (_tcCis.SelectedTab != _tpSettings)
                {
                    _tcCis.SelectedTab = _tpSettings;
                }
                _ePort.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eUser.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eUser,
                        GetString("ErrorInsertUserName"), CgpClient.Singleton.ClientControlNotificationSettings);
                if (_tcCis.SelectedTab != _tpSettings)
                {
                    _tcCis.SelectedTab = _tpSettings;
                }
                _eUser.Focus();
                return false;
            }
            if (Insert && string.IsNullOrEmpty(_ePassword.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ePassword,
                        GetString("ErrorInsertPassword"), CgpClient.Singleton.ClientControlNotificationSettings);
                if (_tcCis.SelectedTab != _tpSettings)
                {
                    _tcCis.SelectedTab = _tpSettings;
                }
                _ePassword.Focus();
                return false;
            }
            return true;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void OkClick(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.CisNGs.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message.Contains("Name"))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedCisNGName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                    return false;
                }
                else if (error is Contal.IwQuick.SqlUniqueException && error.Message == CisNG.COLUMNIPADDRESS)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eIpAddress,
                    GetString("ErrorUsedIPAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eIpAddress.Focus();
                    return false;
                }
                else
                    throw error;
            }
            else
            {
                CgpClient.Singleton.MainServerProvider.CisNGs.ReturnCisNGState(_editingObject);
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.CisNGs.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message.Contains("Name"))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedCisNGName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                    return false;
                }
                else if (error is Contal.IwQuick.SqlUniqueException && error.Message == CisNG.COLUMNIPADDRESS)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eIpAddress,
                    GetString("ErrorUsedIPAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eIpAddress.Focus();
                    return false;
                }
                else
                    throw error;
            }
            else
            {
                CgpClient.Singleton.MainServerProvider.CisNGs.ReturnCisNGState(_editingObject);
            }

            return retValue;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.CisNGs != null)
                CgpClient.Singleton.MainServerProvider.CisNGs.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.CisNGs.
                GetReferencedObjects(_editingObject.IdCisNG as object, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UserFolders_MouseDoubleClick(_lbUserFolders);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        #region Send info message
        private void _cbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (_cbClass.Text)
            {
                case "alarm":
                    _ePriority.Value = 32;
                    break;
                case "info":
                    _ePriority.Value = 96;
                    break;
                case "notification":
                    _ePriority.Value = 160;
                    break;
                case "weather":
                    _ePriority.Value = 224;
                    break;
                default:
                    _ePriority.Value = 254;
                    break;
            }
        }

        private void _chbPriorityOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (_chbPriorityOverride.Checked)
                _ePriority.Enabled = true;
            else
                _ePriority.Enabled = false;
        }

        private void InitCisMsgValues()
        {
            _cbClass.Text = "info";
            //Expiration life time
            _eDays.Value = 0;
            _eHours.Value = 0;
            _eMinutes.Value = 10;
            _eSeconds.Value = 0;
            //Expiration datetime
            DateTime dt = DateTime.Now;
            _eExYear.Value = dt.Year;
            _eExMonth.Value = dt.Month;
            _eExDay.Value = dt.Day;
            _eExHour.Value = dt.Hour;
            if (dt.Minute + 10 > 59)
            {
                _eExHour.Value++;
                _eExMinute.Value = dt.Minute + 10 - 60;
            }
            else
            {
                _eExMinute.Value = dt.Minute + 10;
            }
            _eExSecond.Value = dt.Second;
            //Expiration cycle count
            _eCycleCount.Value = 10;
            //Expiration unicity group
            _eUnicityGroupTimeOut.Value = 3600;
        }

        private void _rbDatetime_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbDatetime.Checked)
            {
                DateTime dt = DateTime.Now;
                _eExYear.Value = dt.Year;
                _eExMonth.Value = dt.Month;
                _eExDay.Value = dt.Day;
                _eExHour.Value = dt.Hour;
                if (dt.Minute + 10 > 59)
                {
                    _eExHour.Value++;
                    _eExMinute.Value = dt.Minute + 10 - 60;
                }
                else
                {
                    _eExMinute.Value = dt.Minute + 10;
                }
            }
        }

        private void _bSend_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (_eMessageText.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eMessageText,
                       GetString("ErrorMsgTextEmpty"), CgpClient.Singleton.ClientControlNotificationSettings);
                return;
            }

#if !DEBUG
            string localisedName = string.Empty;
            object value = null;
            if (!CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(Cgp.Globals.RequiredLicenceProperties.CISIntegration.ToString(), out localisedName, out value))
            {
                //Dialog.Error(GetString("ErrorCanNotObtainLicencePropertyInfo") + " (" + Cgp.Globals.RequiredLicenceProperties.CISIntegration.ToString() + ")");
                Dialog.Error(GetString("ErrorFeatureNotSupportedByLicense"));
                return;
            }
            if (value is bool && (bool)value == true)
            {
#endif

            PresentationGroup noDbsPg = new PresentationGroup();

            noDbsPg.cisClass = _cbClass.Text;
            if (_chbPriorityOverride.Checked)
                noDbsPg.cisPriority = (int)_ePriority.Value;
            else
                noDbsPg.cisPriority = null;
            noDbsPg.cisShowtime = (int)_eShowTime.Value;

            if (!_chbIdle.Checked)
            {
                if (_rbLifetime.Checked)
                {
                    noDbsPg.cisLifetime = new TimeSpan((int)_eDays.Value, (int)_eHours.Value, (int)_eMinutes.Value, (int)_eSeconds.Value);
                }
                else if (_rbDatetime.Checked)
                {
                    try
                    {
                        DateTime dt = new DateTime((int)_eExYear.Value, (int)_eExMonth.Value, (int)_eExDay.Value, (int)_eExHour.Value, (int)_eExMinute.Value, (int)_eExSecond.Value);
                        noDbsPg.cisDatetime = dt;
                    }
                    catch
                    {
                        return;
                    }
                }
                else if (_rbCyclecount.Checked)
                {
                    noDbsPg.cisCyclecount = (int)_eCycleCount.Value;
                }
                else if (_rbUnicitygroup.Checked)
                {
                    noDbsPg.cisUnicitygroup = _eUnicityGroup.Text;
                    noDbsPg.cisUnicitygroupTimeout = (int)_eUnicityGroupTimeOut.Value;
                }

                if (_rbLifetime.Checked)
                    noDbsPg.cisExpirationType = (int)PresentationGroup.ExpirationType.lifetime;
                else if (_rbDatetime.Checked)
                    noDbsPg.cisExpirationType = (int)PresentationGroup.ExpirationType.datetime;
                else if (_rbCyclecount.Checked)
                    noDbsPg.cisExpirationType = (int)PresentationGroup.ExpirationType.cyclecount;
                else if (_rbUnicitygroup.Checked)
                    noDbsPg.cisExpirationType = (int)PresentationGroup.ExpirationType.unicitygroup;
            }
            noDbsPg.cisIdle = _chbIdle.Checked;
            noDbsPg.cisNoscrolling = _chbNoscroling.Checked;
            noDbsPg.cisImmediate = _chbImmediate.Checked;
            noDbsPg.cisFullscreen = _chbFullscreen.Checked;

            noDbsPg.CisNG = new List<CisNG>();
            noDbsPg.CisNG.Add(_editingObject);

            CgpClient.Singleton.MainServerProvider.PresentationGroups.SendCisInfoMessage(noDbsPg, _eMessageText.Text);

#if !DEBUG
            }
            else
                Dialog.Error(GetString("LicenceCanNotSendMessagesToCIS"));
#endif
        }
        #endregion

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsLocalAlarmInstructionView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsLocalAlarmInstructionAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion
    }
}
