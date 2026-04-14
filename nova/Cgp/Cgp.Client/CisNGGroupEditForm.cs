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
    public partial class CisNGGroupEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<CisNGGroup>
#endif
    {

        public CisNGGroupEditForm(CisNGGroup cisNG, ShowOptionsEditForm showOption)
            : base(cisNG, showOption)
        {
            InitializeComponent();
            if (Insert)
            {
                _editingObject.CisNG = new List<Cgp.Server.Beans.CisNG>();
            }
            SetReferenceEditColors();
            WheelTabContorol = _tcCisNGGroup;
            InitCisMsgValues();
            this.MinimumSize = new Size(this.Width, this.Height);

            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbClass.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _eMessageText.MouseWheel += new MouseEventHandler(ControlMouseWheel);

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageCisNg(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsCisNgView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsCisNgAdmin)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsInsertDeletePerfrom)));

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

        private void HideDisableTabPageCisNg(bool view, bool admin, bool insertCisNgCisNgGroup)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool, bool>(HideDisableTabPageCisNg),
                    view,
                    admin,
                    insertCisNgCisNgGroup);
            }
            else
            {
                if (!admin)
                {
                    _eCisNGGroupName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcCisNGGroup.TabPages.Remove(_tpCisNG);
                    return;
                }

                _tpCisNG.Enabled = admin;

                if (!insertCisNgCisNgGroup)
                    _bCreate.Enabled = false;
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
                    _tcCisNGGroup.TabPages.Remove(_tpSendInfoMessage);
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
                    _tcCisNGGroup.TabPages.Remove(_tpUserFolders);
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
                    _tcCisNGGroup.TabPages.Remove(_tpReferencedBy);
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
                    _tcCisNGGroup.TabPages.Remove(_tpDescription);
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
                Text = LocalizationHelper.GetString("CisNGGroupEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            CisNGGroupsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            CisNGGroupsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            CisNGGroupsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            CisNGGroupsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            CisNGGroup obj = CgpClient.Singleton.MainServerProvider.CisNGGroups.GetObjectForEdit(_editingObject.IdCisNGGroup, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.CisNGGroups.GetObjectById(_editingObject.IdCisNGGroup);
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

            List<CisNG> listCisNG = new List<CisNG>();
            if (_editingObject.CisNG != null)
            {
                foreach (CisNG cisNG in _editingObject.CisNG)
                {
                    listCisNG.Add(cisNG);
                }
            }

            CgpClient.Singleton.MainServerProvider.CisNGGroups.RenewObjectForEdit(_editingObject.IdCisNGGroup, out error);
            if ( error == null)
            {
                _editingObject.CisNG = listCisNG;
            }
            else
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            ShowCisNGs();
        }

        protected override void SetValuesEdit()
        {
            _eCisNGGroupName.Text = _editingObject.GroupName;
            _eDescription.Text = _editingObject.Description;
            ShowCisNGs();
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
                _editingObject.GroupName = _eCisNGGroupName.Text;
                _editingObject.Description = _eDescription.Text;
                _editingObject.LocalAlarmInstruction = GetLocalAlarmInstruction();
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
            if (string.IsNullOrEmpty(_eCisNGGroupName.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eCisNGGroupName,
                        GetString("ErrorInsertGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eCisNGGroupName.Focus();
                return false;
            }
            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.CisNGGroups.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eCisNGGroupName,
                    GetString("ErrorUsedGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eCisNGGroupName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.CisNGGroups.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eCisNGGroupName,
                    GetString("ErrorUsedGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eCisNGGroupName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        private void OkClick(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void ShowCisNGs()
        {
            ConnectionLost();
            _lbCisNG.Items.Clear();
            if (_editingObject.CisNG == null) return;
            foreach (CisNG cisNG in _editingObject.CisNG.OrderBy(cis => cis.ToString()))
            {
                _lbCisNG.Items.Add(cisNG);
            }
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            CisNG cisNg = new CisNG();
            CisNGsForm.Singleton.OpenInsertFromEdit(ref cisNg, DoAfterCisNGCreated);
        }

        private void DoAfterCisNGCreated(object newCisNG)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCisNGCreated), newCisNG);
            }
            else
            {
                _editingObject.CisNG.Add((CisNG)newCisNG);
                ShowCisNGs();
            }
        }

        private void AddClick(object sender, EventArgs e)
        {
            ConnectionLost();
            AddCisNG();
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            ConnectionLost();
            CisNG cisNG = _lbCisNG.SelectedItem as CisNG;
            if (cisNG != null)
            {
                if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteCisNG")))
                {
                    if (DeleteCisNG(cisNG))
                    {
                        ShowCisNGs();
                    }
                }
            }
        }

        private bool CisAlreadySet(CisNG cisNG)
        {
            foreach (CisNG cis in _editingObject.CisNG)
            {
                if (cis.IdCisNG == cisNG.IdCisNG)
                    return true;
            }
            return false;
        }

        private bool AddCisNG()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return false;
                List<AOrmObject> listCisNGs = new List<AOrmObject>();

                IList<FilterSettings> filterSettings = new List<FilterSettings>();
                foreach (CisNG cis in _editingObject.CisNG)
                {
                    FilterSettings filter = new FilterSettings(CisNG.COLUMNIDCISNG, cis.IdCisNG, ComparerModes.NOTEQUALL);
                    filterSettings.Add(filter);
                }

                Exception error;
                ICollection<CisNG> listCisNGFromDatabase = CgpClient.Singleton.MainServerProvider.CisNGs.SelectByCriteria(filterSettings, out error);

                foreach (CisNG cis in listCisNGFromDatabase)
                {

                    if (!_editingObject.CisNG.Contains<CisNG>(cis))
                    {
                        listCisNGs.Add(cis);
                        ShowCisNGs();
                    }
                }

                ListboxFormAdd formAdd = new ListboxFormAdd(listCisNGs, GetString("CisNGGroupEditAddCisNG"));

                object outCisNG;
                formAdd.ShowDialog(out outCisNG);

                if (outCisNG != null)
                {
                    return SetCisNG(outCisNG as CisNG);
                }

                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddPresentationGroup"));
                return false;
            }
        }

        private bool SetCisNG(CisNG cisNG)
        {
            ConnectionLost();

            if (CisAlreadySet(cisNG))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _lbCisNG,
                    GetString("ErrorCisAllreadySet"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _lbCisNG.Focus();
                return false;
            }

            _editingObject.CisNG.Add(cisNG);
            CgpClientMainForm.Singleton.AddToRecentList(cisNG);
            ShowCisNGs();
            EditTextChanger(null, null);
            CgpClientMainForm.Singleton.AddToRecentList(cisNG);
            return true;
        }

        private bool DeleteCisNG(CisNG cisNG)
        {
            try
            {
                if (cisNG != null)
                {
                    _editingObject.CisNG.Remove(cisNG);
                    EditTextChanger(null, null);
                    return true;
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorRemoveCisNG"));
                return false;
            }
        }

        private void CisNGDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                ConnectionLost();

                string[] output = e.Data.GetFormats();
                if (output == null) return;

                object obj = e.Data.GetData(output[0]);
                if (obj.GetType() == typeof(Contal.Cgp.Server.Beans.CisNG))
                {
                    SetCisNG(obj as CisNG);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _lbCisNG,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            {
            }
        }

        private void CisNGDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        private void _lbCisNG_DoubleClick(object sender, EventArgs e)
        {
            ConnectionLost();
            CisNG cisNG = _lbCisNG.SelectedItem as CisNG;
            if (cisNG != null)
            {
                CisNGsForm.Singleton.OpenEditForm(cisNG);
            }
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.CisNGGroups != null)
                CgpClient.Singleton.MainServerProvider.CisNGGroups.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.CisNGGroups.
                GetReferencedObjects(_editingObject.IdCisNGGroup as object, CgpClient.Singleton.GetListLoadedPlugins());
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

            if (_editingObject.CisNG == null || _editingObject.CisNG.Count == 0)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _lbCisNG,
                       GetString("ErrorNotsetCisNG"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tcCisNGGroup.SelectedTab = _tpCisNG;
                return;
            }

            if (_eMessageText.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eMessageText,
                       GetString("ErrorMsgTextEmpty"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
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

            noDbsPg.CisNGGroup = new List<CisNGGroup>();
            noDbsPg.CisNGGroup.Add(_editingObject);

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