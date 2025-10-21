using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Threads;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class PresentationGroupEditForm :
#if DESIGNER
        Form
#else 
        ACgpEditForm<PresentationGroup>        
#endif
    {
        private bool _wasInicialized;

        public PresentationGroupEditForm(PresentationGroup presentationGroup, ShowOptionsEditForm showOption)
            : base(presentationGroup, showOption)
        {
            InitializeComponent();
            _editingObject = presentationGroup;

            if (Insert)
            {
                _editingObject.CisNG = new List<CisNG>();
                _editingObject.CisNGGroup = new List<CisNGGroup>();
            }
               
            WheelTabContorol = _tcPG;
            MinimumSize = new Size(Width, Height + 15);
            _eDescription.MouseWheel += ControlMouseWheel;
            _cbClass.MouseWheel += ControlMouseWheel;
            SafeThread.StartThread(HideDisableTabPages);
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
            LoadNCASPlugin();

            SetReferenceEditColors();
        }

        private void LoadNCASPlugin()
        {
            if (!CgpClient.Singleton.LoadPluginControlToForm("NCAS plugin",
                _editingObject,
                _tpSms,
                this,
                true))
            {
                _tcPG.TabPages.Remove(_tpSms);
            }
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersAlarmSettingsAdmin)));

                HideDisableTabPageEmail(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersEmailView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersEmailAdmin)));

                HideDisableTabPageCisNg(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersCisNgView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersCisNgAdmin)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsInsertDeletePerfrom)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageAlarmSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAlarmSettings),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eGroupName.ReadOnly = true;
                    _tbmFormatterName.Enabled = false;
                }

                if (!view && !admin)
                {
                    _tcPG.TabPages.Remove(_tpAlarmSettings);
                    return;
                }

                _tpAlarmSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageEmail(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageEmail),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcPG.TabPages.Remove(_tpEmail);
                    return;
                }

                _tpEmail.Enabled = admin;
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
                if (!view && !admin)
                {
                    _tcPG.TabPages.Remove(_tpCisNG);
                    return;
                }

                _tpCisNG.Enabled = admin;

                if (!insertCisNgCisNgGroup)
                {
                    _bCreateCisNG.Enabled = false;
                    _bCreateCisNGGroup.Enabled = false;
                }
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
                    _tcPG.TabPages.Remove(_tpUserFolders);
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
                    _tcPG.TabPages.Remove(_tpReferencedBy);
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
                    _tcPG.TabPages.Remove(_tpDescription);
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
                Text = LocalizationHelper.GetString("PresentationGroupEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            PresentationGroupsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            PresentationGroupsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            PresentationGroupsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            PresentationGroupsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            PresentationGroup obj = CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectForEdit(_editingObject.IdGroup, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(_editingObject.IdGroup);
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

            List<Cgp.Server.Beans.CisNG> listCisNG = _editingObject.CisNG.ToList();
            List<Cgp.Server.Beans.CisNGGroup> listCisNGGroup = _editingObject.CisNGGroup.ToList();
            PresentationFormatter pressFormatter = _editingObject.PresentationFormatter;

            CgpClient.Singleton.MainServerProvider.PresentationGroups.RenewObjectForEdit(_editingObject.IdGroup, out error);
            if (error != null)
            {
                throw error;
            }
            else
            {
                _editingObject.CisNG.Clear();
                foreach (CisNG cisNG in listCisNG)
                {
                    _editingObject.CisNG.Add(cisNG);
                }

                _editingObject.CisNGGroup.Clear();
                foreach (CisNGGroup cisNGGroup in listCisNGGroup)
                {
                    _editingObject.CisNGGroup.Add(cisNGGroup);
                }

                _editingObject.PresentationFormatter = pressFormatter;
            }
        }

        protected override void SetValuesInsert()
        {
            _chbAlarm.Checked = false;
            _chbAlarmNotAck.Checked = true;
            _chbNormalNotAck.Checked = true;
            _chbNormal.Checked = false;
            SetCisNGEnabled();
        }

        protected override void SetValuesEdit()
        {
            _eGroupName.Text = _editingObject.GroupName;
            _eEmail.Text = _editingObject.Email;
            //_tbNewPhoneNumber.Text = _editingObject.Sms;
            _eDescription.Text = _editingObject.Description;

            _chbAlarm.Checked = _editingObject.ResponseAlarm;
            _chbAlarmNotAck.Checked = _editingObject.ResponseAlarmNotAck;
            _chbNormalNotAck.Checked = _editingObject.ResponseNormalNotAck;
            _chbNormal.Checked = _editingObject.ResponseNormal;
            _chbDateTimeUpdate.Checked = _editingObject.ResponseDateTimeUpdate;

            _chbInherited.Checked = _editingObject.InheritedEmailSubject;
            _eSubject.Text = _editingObject.EmailSubject;

            SetValuesCisNG();
            ShowFormatter();
            ShowCisNGs();
            SetCisNGEnabled();
            SetReferencedBy();
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
        }

        private void SetValuesCisNG()
        {
            _rbLifetime.Checked = false;
            _rbDatetime.Checked = false;
            _rbCyclecount.Checked = false;
            _rbUnicitygroup.Checked = false;


            _cbClass.Text = _editingObject.cisClass;
            if ((decimal)_editingObject.cisShowtime >= 1
                && (decimal)_editingObject.cisShowtime <= 3600)
                _eShowTime.Value = (decimal)_editingObject.cisShowtime;
            if (_editingObject.cisPriority == null)
            {
                _chbPriorityOverride.Checked = false;
            }
            else
            {
                _chbPriorityOverride.Checked = true;
                _ePriority.Value = (decimal)_editingObject.cisPriority;
            }
            _chbIdle.Checked = _editingObject.cisIdle;
            _chbImmediate.Checked = _editingObject.cisImmediate;
            _chbNoscroling.Checked = _editingObject.cisNoscrolling;
            _chbFullscreen.Checked = _editingObject.cisFullscreen;

            if (_editingObject.cisDatetime != null)
            {
                DateTime dt = (DateTime)_editingObject.cisDatetime;
                _eExDay.Value = dt.Day;
                _eExHour.Value = dt.Hour;
                _eExMinute.Value = dt.Minute;
                _eExMonth.Value = dt.Month;
                _eExSecond.Value = dt.Second;
                _eExYear.Value = dt.Year;
            }
            if (_editingObject.cisLifetime != null)
            {
                _eDays.Value = _editingObject.cisLifetime.Value.Days;
                _eHours.Value = _editingObject.cisLifetime.Value.Hours;
                _eMinutes.Value = _editingObject.cisLifetime.Value.Minutes;
                _eSeconds.Value = _editingObject.cisLifetime.Value.Seconds;
            }
            if (_editingObject.cisCyclecount >= 1 && _editingObject.cisCyclecount <= 65535)
                _eCycleCount.Value = _editingObject.cisCyclecount;
            _eUnicityGroup.Text = _editingObject.cisUnicitygroup;
            if (_editingObject.cisUnicitygroupTimeout != null && _editingObject.cisUnicitygroupTimeout >= 1
                && _editingObject.cisUnicitygroupTimeout <= 2592000)
                _eUnicityGroupTimeOut.Value = (decimal)_editingObject.cisUnicitygroupTimeout;
            switch (_editingObject.cisExpirationType)
            {
                case (int)PresentationGroup.ExpirationType.lifetime:
                    _rbLifetime.Checked = true;
                    _pLifetime.Enabled = true;
                    break;
                case (int)PresentationGroup.ExpirationType.datetime:
                    _rbDatetime.Checked = true;
                    _pDatetime.Enabled = true;
                    break;
                case (int)PresentationGroup.ExpirationType.cyclecount:
                    _rbCyclecount.Checked = true;
                    _pCyclecount.Enabled = true;
                    break;
                case (int)PresentationGroup.ExpirationType.unicitygroup:
                    _rbUnicitygroup.Checked = true;
                    _pUnicitygroup.Enabled = true;
                    break;
                default: _rbLifetime.Checked = true; break;
            }

            if (_chbIdle.Checked)
            {
                _gbExpirationType.Enabled = false;
                _chbPriorityOverride.Checked = false;
                _chbPriorityOverride.Enabled = false;
                _ePriority.Enabled = false;
            }
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.GroupName = _eGroupName.Text;
                _editingObject.Email = _eEmail.Text;
                //_editingObject.Sms = _tbNewPhoneNumber.Text;
                _editingObject.Description = _eDescription.Text;

                if (_lbCisNG.Items.Count != 0)
                {
                    _editingObject.cisClass = _cbClass.Text;
                    if (_chbPriorityOverride.Checked)
                        _editingObject.cisPriority = (int)_ePriority.Value;
                    else
                        _editingObject.cisPriority = null;
                    _editingObject.cisShowtime = (int)_eShowTime.Value;

                    if (!_chbIdle.Checked)
                    {
                        if (_rbLifetime.Checked)
                        {
                            _editingObject.cisLifetime = new TimeSpan((int)_eDays.Value, (int)_eHours.Value, (int)_eMinutes.Value, (int)_eSeconds.Value);
                        }
                        else if (_rbDatetime.Checked)
                        {
                            try
                            {
                                DateTime dt = new DateTime((int)_eExYear.Value, (int)_eExMonth.Value, (int)_eExDay.Value, (int)_eExHour.Value, (int)_eExMinute.Value, (int)_eExSecond.Value);
                                _editingObject.cisDatetime = dt;
                            }
                            catch
                            {
                                return false;
                            }
                        }
                        else if (_rbCyclecount.Checked)
                        {
                            _editingObject.cisCyclecount = (int)_eCycleCount.Value;
                        }
                        else if (_rbUnicitygroup.Checked)
                        {
                            _editingObject.cisUnicitygroup = _eUnicityGroup.Text;
                            _editingObject.cisUnicitygroupTimeout = (int)_eUnicityGroupTimeOut.Value;
                        }

                        if (_rbLifetime.Checked)
                            _editingObject.cisExpirationType = (int)PresentationGroup.ExpirationType.lifetime;
                        else if (_rbDatetime.Checked)
                            _editingObject.cisExpirationType = (int)PresentationGroup.ExpirationType.datetime;
                        else if (_rbCyclecount.Checked)
                            _editingObject.cisExpirationType = (int)PresentationGroup.ExpirationType.cyclecount;
                        else if (_rbUnicitygroup.Checked)
                            _editingObject.cisExpirationType = (int)PresentationGroup.ExpirationType.unicitygroup;
                    }
                    _editingObject.cisIdle = _chbIdle.Checked;
                    _editingObject.cisNoscrolling = _chbNoscroling.Checked;
                    _editingObject.cisImmediate = _chbImmediate.Checked;
                    _editingObject.cisFullscreen = _chbFullscreen.Checked;
                }

                _editingObject.ResponseAlarm = _chbAlarm.Checked;
                _editingObject.ResponseAlarmNotAck = _chbAlarmNotAck.Checked;
                _editingObject.ResponseNormalNotAck = _chbNormalNotAck.Checked;
                _editingObject.ResponseNormal = _chbNormal.Checked;
                _editingObject.ResponseDateTimeUpdate = _chbDateTimeUpdate.Checked;

                _editingObject.InheritedEmailSubject = _chbInherited.Checked;
                _editingObject.EmailSubject = _eSubject.Text;
                return true;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private void ShowFormatter()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_editingObject.PresentationFormatter == null) return;
            PresentationFormatter formatter = CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectById(_editingObject.PresentationFormatter.IdFormatter);
            if (formatter == null) return;
            _tbmFormatterName.Text = formatter.FormatterName;
            _tbmFormatterName.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(formatter);
            _eMsgFormat.Text = formatter.MessageFormat;
        }

        protected override bool CheckValues()
        {
            if (!string.IsNullOrEmpty(_eEmail.Text))
            {
                if (!CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.IsSetSMTP())
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSMTPNotSet"));
                    return false;
                }
            }

            //if (!string.IsNullOrEmpty(_tbNewPhoneNumber.Text))
            //{
            //    if (!CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.IsSetSerialPort())
            //    {
            //        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSerialPortNotSet"));
            //        return false;
            //    }
            //}

            if (string.IsNullOrEmpty(_eGroupName.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eGroupName,
                        GetString("ErrorInsertGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eGroupName.Focus();
                return false;
            }
            if (!string.IsNullOrEmpty(_eEmail.Text))
            {
                if (!IsAddressOk())
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eEmail,
                        GetString("ErrorWrongEmail"), CgpClient.Singleton.ClientControlNotificationSettings);
                    if (_tcPG.SelectedTab != _tpEmail)
                    {
                        _tcPG.SelectedTab = _tpEmail;
                    }
                    _eEmail.Focus();
                    return false;
                }
            }
            if (!_chbInherited.Checked)
            {
                if (string.IsNullOrEmpty(_eSubject.Text))
                {
                    IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eSubject,
                        GetString("ErrorEntryEmailSubject"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eSubject.Focus();
                    return false;
                }
            }
            //if (!string.IsNullOrEmpty(_tbNewPhoneNumber.Text))
            //{
            //    if (!IsSmsOk())
            //    {
            //        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbNewPhoneNumber,
            //            GetString("ErrorWrongSms"), CgpClient.Singleton.ClientControlNotificationSettings);
            //        if (_tcPG.SelectedTab != _tpSms)
            //        {
            //            _tcPG.SelectedTab = _tpSms;
            //        }
            //        _tbNewPhoneNumber.Focus();
            //        return false;
            //    }
            //}

            //check values for CisNG
            if (_lbCisNG.Items.Count != 0)
            {
                if (String.IsNullOrEmpty(_cbClass.Text))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbClass,
                            GetString("ErrorInsertClassName"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    if (_tcPG.SelectedTab != _tpCisNG)
                    {
                        _tcPG.SelectedTab = _tpCisNG;
                    }
                    _cbClass.Focus();
                    return false;
                }
                if (String.IsNullOrEmpty(_eShowTime.Text))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eShowTime,
                            GetString("ErrorInsertShowtime"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    if (_tcPG.SelectedTab != _tpCisNG)
                    {
                        _tcPG.SelectedTab = _tpCisNG;
                    }
                    _eShowTime.Focus();
                    return false;
                }

                if (!_chbIdle.Checked) //if Idle than Expiration type is useless
                {
                    if (_rbLifetime.Checked)
                    {
                        if (String.IsNullOrEmpty(_eDays.Text))
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eDays,
                                    GetString("ErrorInsertLifetimeDays"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }
                            _eDays.Focus();
                            return false;
                        }
                        if (String.IsNullOrEmpty(_eHours.Text))
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eHours,
                                    GetString("ErrorInsertLifetimeHours"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }
                            _eHours.Focus();
                            return false;
                        }
                        if (String.IsNullOrEmpty(_eMinutes.Text))
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eMinutes,
                                    GetString("ErrorInsertLifetimeMinutes"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }                            
                            _eMinutes.Focus();
                            return false;
                        }
                        if (String.IsNullOrEmpty(_eSeconds.Text))
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eSeconds,
                                    GetString("ErrorInsertLifetimeSeconds"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }                            
                            _eSeconds.Focus();
                            return false;
                        }
                    }
                    else if (_rbDatetime.Checked)
                    {
                        try
                        {
                            DateTime dt = new DateTime((int)_eExYear.Value, (int)_eExMonth.Value, (int)_eExDay.Value, (int)_eExHour.Value, (int)_eExMinute.Value, (int)_eExSecond.Value);
                        }
                        catch
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eExYear,
                                    GetString("ErrorDateRange"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }                            
                            _eExYear.Focus();
                            return false;
                        }
                    }
                    else if (_rbCyclecount.Checked)
                    {
                        if (String.IsNullOrEmpty(_eCycleCount.Text))
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eCycleCount,
                                    GetString("ErrorInsertCycleCount"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }                            
                            _eCycleCount.Focus();
                            return false;
                        }
                    }
                    else if (_rbUnicitygroup.Checked)
                    {
                        if (String.IsNullOrEmpty(_eUnicityGroup.Text))
                        {
                            Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eUnicityGroup,
                                    GetString("ErrorInsertUnicityGroup"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                            if (_tcPG.SelectedTab != _tpCisNG)
                            {
                                _tcPG.SelectedTab = _tpCisNG;
                            }                            
                            _eUnicityGroup.Focus();
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool IsAddressOk()
        {
            string[] parts = _eEmail.Text.Split(';', ' ', ',');
            if (parts == null || parts.Length == 0) return false;
            foreach (string email in parts)
            {
                string testEmail = email.Trim();
                if (testEmail == string.Empty) continue;
                if (!Contal.IwQuick.Net.EmailAddress.IsValid(testEmail))
                {
                    return false;
                }
            }
            return true;
        }

        //private bool IsSmsOk()
        //{
        //    string[] parts = _tbNewPhoneNumber.Text.Split(';', ' ', ',');
        //    if (parts == null || parts.Length == 0) return false;
        //    foreach (string phone in parts)
        //    {
        //        string testSms = phone.Trim();
        //        if (testSms == string.Empty) continue;
        //        if (!IsValidPhoneNumber(testSms))
        //            return false;
        //    }
        //    return true;
        //}

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (Contal.IwQuick.Validator.IsNullString(phoneNumber))
                return false;

            try
            {
                bool result = false;
                Regex aRegex = new Regex(@"^([0-9]{6,10})$");
                result = aRegex.IsMatch(phoneNumber);
                if (!result)
                {
                    aRegex = new Regex(@"^([+]?|[0]{2}?)([0-9]{9,12})$");
                    result = aRegex.IsMatch(phoneNumber);
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            CgpClient.Singleton.MainServerProvider.PresentationGroups.RefreshRelationWithEvent(_editingObject, SystemEvent.DATABASE_DISCONNECTED);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.PresentationGroups.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eGroupName,
                    GetString("ErrorUsedGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eGroupName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.PresentationGroups.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eGroupName,
                    GetString("ErrorUsedGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eGroupName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }


        private void _bSet_Click(object sender, EventArgs e)
        {
            SetFormatter();
        }

        private string GetForamtter()
        {
            return string.Empty;
        }

        private bool SetFormatter()
        {
            ConnectionLost();

            try
            {
                List<IModifyObject> listModObj = new List<IModifyObject>();
                Exception error;
                ICollection<IModifyObject> listFormaters = CgpClient.Singleton.MainServerProvider.PresentationFormatters.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listFormaters);

                ListboxFormAdd formAdd = new ListboxFormAdd(listModObj, GetString("PresentationGroupEditFormSetFormatterFormText"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    PresentationFormatter addFomatter = CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectById(outModObj.GetId);
                    _editingObject.PresentationFormatter = addFomatter;
                    _tbmFormatterName.Text = addFomatter.FormatterName;
                    _tbmFormatterName.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(addFomatter);

                    _eMsgFormat.Text = addFomatter.MessageFormat;
                    EditTextChanger(null, null);
                    CgpClientMainForm.Singleton.AddToRecentList(addFomatter);
                    return true;
                }

                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddFormatter"));
                return false;
            }
        }

        private void _eFormatterName_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddDroppedPresentationForamtter((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void EditDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void AddDroppedPresentationForamtter(object inFormatter)
        {
            try
            {
                if (inFormatter.GetType() == typeof(Contal.Cgp.Server.Beans.PresentationFormatter))
                {
                    PresentationFormatter formater = inFormatter as PresentationFormatter;
                    _editingObject.PresentationFormatter = formater;
                    _tbmFormatterName.Text = formater.FormatterName;
                    _tbmFormatterName.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(formater);

                    _eMsgFormat.Text = formater.MessageFormat;
                    CgpClientMainForm.Singleton.AddToRecentList(inFormatter);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmFormatterName.ImageTextBox,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void ClearFormatterClick(object sender, EventArgs e)
        {
            _tbmFormatterName.Text = string.Empty;
            _eMsgFormat.Text = string.Empty;
            _editingObject.PresentationFormatter = null;
        }

        private void ExpirationTypeCheckedChanged(object sender, EventArgs e)
        {
            _pLifetime.Enabled = false;
            _pDatetime.Enabled = false;
            _pCyclecount.Enabled = false;
            _pUnicitygroup.Enabled = false;

            if (_rbLifetime.Checked)
            {
                _pLifetime.Enabled = true;
                if (_editingObject.cisLifetime == null ||
                    (_eHours.Value == 0 && _eMinutes.Value == 0 && _eSeconds.Value == 0 && _eDays.Value == 0))
                {
                    InitExpirationLifeTime();
                }
            }
            else if (_rbDatetime.Checked)
            {
                _pDatetime.Enabled = true;
                if (_editingObject.cisDatetime == null)
                {
                    InitExpirationDatetime();
                }
            }
            else if (_rbCyclecount.Checked)
            {
                _pCyclecount.Enabled = true;
                if (_editingObject.cisCyclecount == 0)
                {
                    InitExpirationCycleCount();
                }
            }
            else if (_rbUnicitygroup.Checked)
            {
                _pUnicitygroup.Enabled = true;
                if (_editingObject.cisUnicitygroupTimeout == null || _eUnicityGroup.Text == string.Empty)
                {
                    InitExpirationUnicityGroup();
                }
            }
        }

        private void PriorityCheckedChanged(object sender, EventArgs e)
        {
            if (_chbPriorityOverride.Checked)
                _ePriority.Enabled = true;
            else
                _ePriority.Enabled = false;
            EditTextChanger(null, null);
        }


        private void CisNGChanged(object sender, EventArgs e)
        {
            SetCisNGEnabled();
        }

        private void SetCisNGEnabled()
        {
            if (_lbCisNG.Items.Count == 0)
            {
                _gbExpirationType.Enabled = false;
                _gbCisMessage.Enabled = false;
            }
            else
            {
                _gbExpirationType.Enabled = true;
                _gbCisMessage.Enabled = true;
            }
            EditTextChanger(null, null);
        }

        private void ClassTextChanged(object sender, EventArgs e)
        {
            if (_cbClass.Text == "alarm")
            {
                _ePriority.Value = 32;
            }
            else if (_cbClass.Text == "info")
            {
                _ePriority.Value = 96;
            }
            else if (_cbClass.Text == "notification")
            {
                _ePriority.Value = 160;
            }
            else if (_cbClass.Text == "weather")
            {
                _ePriority.Value = 224;
            }
            else if (_cbClass.Text == "")
            {
                _ePriority.Value = 254;
            }
            EditTextChanger(null, null);
        }

        private void IdleCheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
            if (_chbIdle.Checked)
            {
                _chbPriorityOverride.Checked = false;
                _chbPriorityOverride.Enabled = false;
                _ePriority.Enabled = false;
                _gbExpirationType.Enabled = false;
            }
            else
            {
                _chbPriorityOverride.Enabled = true;
                _gbExpirationType.Enabled = true;
            }
        }

        private void InitCisNGValues()
        {
            if (_wasInicialized) return;
            _wasInicialized = true;
            _cbClass.Text = "info";
            _chbPriorityOverride.Checked = false;
            _ePriority.Value = 96;
            _eShowTime.Value = 15;
            _chbFullscreen.Checked = false;
            _chbIdle.Checked = false;
            _chbImmediate.Checked = false;
            _chbNoscroling.Checked = false;

            InitExpirationLifeTime();
            InitExpirationDatetime();
            InitExpirationCycleCount();
            InitExpirationUnicityGroup();
            _rbLifetime.Checked = true;
        }

        private void InitExpirationLifeTime()
        {
            _eDays.Value = 0;
            _eHours.Value = 0;
            _eMinutes.Value = 10;
            _eSeconds.Value = 0;

        }

        private void InitExpirationDatetime()
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
            _eExSecond.Value = dt.Second;
        }

        private void InitExpirationCycleCount()
        {
            _eCycleCount.Value = 10;
        }

        private void InitExpirationUnicityGroup()
        {
            _eUnicityGroupTimeOut.Value = 3600;
        }


        #region Presentation Group ICollections

        private bool AddCisNG()
        {
            ConnectionLost();
            try
            {
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
                    listCisNGs.Add(cis);
                }

                ListboxFormAdd formAdd = new ListboxFormAdd(listCisNGs, GetString("PresentationGroupEditAddCisNG"));

                ListOfObjects outCisNGs;
                formAdd.ShowDialogMultiSelect(out outCisNGs);

                if (outCisNGs != null)
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionAddCisNGs")))
                    {
                        for (int i = 0; i < outCisNGs.Count; i++)
                        {
                            SetCisNGToPG(outCisNGs.Objects[i] as CisNG, true);
                        }
                    }
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddCisNGtoPG"));
                return false;
            }
        }

        private bool AddCisNGGroup()
        {
            ConnectionLost();
            try
            {
                List<AOrmObject> listCisNGGroups = new List<AOrmObject>();

                IList<FilterSettings> filterSettings = new List<FilterSettings>();
                foreach (CisNGGroup cisGrup in _editingObject.CisNGGroup)
                {
                    FilterSettings filter = new FilterSettings(CisNGGroup.COLUMNIDCISNGGROUP, cisGrup.IdCisNGGroup, ComparerModes.NOTEQUALL);
                    filterSettings.Add(filter);
                }

                Exception error;
                ICollection<CisNGGroup> listCisNGGroupFromDatabase = CgpClient.Singleton.MainServerProvider.CisNGGroups.SelectByCriteria(filterSettings, out error);
                foreach (CisNGGroup cis in listCisNGGroupFromDatabase)
                {
                    listCisNGGroups.Add(cis);
                }

                ListboxFormAdd formAdd = new ListboxFormAdd(listCisNGGroups, GetString("PresentationGroupEditAddCisNGGroup"));

                ListOfObjects outCisNGs;
                formAdd.ShowDialogMultiSelect(out outCisNGs);

                if (outCisNGs != null)
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionAddCisNGsGroup")))
                    {
                        for (int i = 0; i < outCisNGs.Count; i++)
                        {
                            SetCisNGGroupToPG(outCisNGs.Objects[i] as CisNGGroup, true);
                        }
                    }
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddCisNGGrouptoPG"));
                return false;
            }
        }

        private bool SetCisNGToPG(CisNG cisNG)
        {
            return SetCisNGToPG(cisNG, false);
        }
        private bool SetCisNGToPG(CisNG cisNG, bool autoConfirm)
        {
            try
            {
                if (autoConfirm || Contal.IwQuick.UI.Dialog.Question(GetString("QuestionAddCisNG")))
                {
                    _editingObject.CisNG.Add(cisNG);
                    CgpClientMainForm.Singleton.AddToRecentList(cisNG);
                    return true;
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddCisNGtoPG"));
                return false;
            }
        }

        private bool SetCisNGGroupToPG(CisNGGroup cisNGGroup)
        {
            return SetCisNGGroupToPG(cisNGGroup, false);
        }

        private bool SetCisNGGroupToPG(CisNGGroup cisNGGroup, bool autoConfirm)
        {
            try
            {
                if (autoConfirm || Contal.IwQuick.UI.Dialog.Question(GetString("QuestionAddCisNGGroup")))
                {
                    _editingObject.CisNGGroup.Add(cisNGGroup);
                    CgpClientMainForm.Singleton.AddToRecentList(cisNGGroup);
                    return true;
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddCisNGGrouptoPG"));
                return false;
            }
        }

        private bool CisNGAlreadySet(CisNG cisNG)
        {
            foreach (CisNG cis in _editingObject.CisNG)
            {
                if (cis.IdCisNG == cisNG.IdCisNG)
                    return true;
            }
            return false;
        }

        private bool CisNGGroupAlreadySet(CisNGGroup cisNGGroup)
        {
            foreach (CisNGGroup cis in _editingObject.CisNGGroup)
            {
                if (cis.IdCisNGGroup == cisNGGroup.IdCisNGGroup)
                    return true;
            }
            return false;
        }

        private void AddDroppedCisNG(object inCisNG)
        {
            try
            {
                if (inCisNG.GetType() == typeof(Contal.Cgp.Server.Beans.CisNG))
                {
                    if (CisNGAlreadySet(inCisNG as CisNG))
                    {
                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _lbCisNG,
                        GetString("ErrorCisAllreadySet"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                        _lbCisNG.Focus();
                        return;
                    }

                    SetCisNGToPG(inCisNG as CisNG);
                }
                else if (inCisNG.GetType() == typeof(Contal.Cgp.Server.Beans.CisNGGroup))
                {
                    if (CisNGGroupAlreadySet(inCisNG as CisNGGroup))
                    {
                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _lbCisNG,
                        GetString("ErrorCisGroupAllreadySet"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                        _lbCisNG.Focus();
                        return;
                    }

                    SetCisNGGroupToPG(inCisNG as CisNGGroup);
                    CgpClientMainForm.Singleton.AddToRecentList(inCisNG);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _lbCisNG,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
                InitCisNGValues();
                ShowCisNGs();
            }
            catch
            { }
        }

        private void ShowCisNGs()
        {
            _lbCisNG.Items.Clear();
            if (_editingObject.CisNG != null)
            {
                foreach (CisNG cisNG in _editingObject.CisNG)
                {
                    _lbCisNG.Items.Add(cisNG);
                }
            }
            if (_editingObject.CisNGGroup != null)
            {
                foreach (CisNGGroup cisNGGroup in _editingObject.CisNGGroup)
                {
                    _lbCisNG.Items.Add(cisNGGroup);
                }
            }
            SetCisNGEnabled();
        }

        private void RemoveCisNgClick(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
            object obj = _lbCisNG.SelectedItem;
            if (obj == null) return;

            if (obj.GetType() == typeof(Cgp.Server.Beans.CisNGGroup))
            {
                CisNGGroup cisNGGroup = _lbCisNG.SelectedItem as CisNGGroup;
                if (cisNGGroup != null)
                {
                    if (RemoveCisNGGroup(cisNGGroup))
                    {
                        ShowCisNGs();
                    }
                }
            }
            if (obj.GetType() == typeof(Cgp.Server.Beans.CisNG))
            {
                CisNG cisNG = _lbCisNG.SelectedItem as CisNG;
                if (cisNG != null)
                {
                    if (RemoveCisNG(cisNG))
                    {
                        ShowCisNGs();
                    }
                }
            }
        }

        private bool RemoveCisNG(CisNG cisNG)
        {
            ConnectionLost();
            try
            {
                if (cisNG != null)
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("ConfirmRemoveCisNG")))
                    {
                        _editingObject.CisNG.Remove(cisNG);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorRemoveCisNG"));
                return false;
            }
        }

        private bool RemoveCisNGGroup(CisNGGroup cisNGGroup)
        {
            ConnectionLost();
            try
            {
                if (cisNGGroup != null)
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("ConfirmRemoveCisNGGroup")))
                    {
                        _editingObject.CisNGGroup.Remove(cisNGGroup);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorRemoveCisNGGroup"));
                return false;
            }
        }

        private void _eCisNG_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddDroppedCisNG((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddCisNgClick(object sender, EventArgs e)
        {
            AddCisNG();
            InitCisNGValues();
            ShowCisNGs();
        }


        private void AddCisNgGroupClick(object sender, EventArgs e)
        {
            AddCisNGGroup();
            InitCisNGValues();
            ShowCisNGs();
        }
        #endregion

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        private void _eFormatterName_TextChanged(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        private void _eFormatterName_DoubleClick(object sender, EventArgs e)
        {
            if (_editingObject.PresentationFormatter != null)
            {
                PresentationFormattersForm.Singleton.OpenEditForm(_editingObject.PresentationFormatter);
            }
        }

        private void _lbCisNG_DoubleClick(object sender, EventArgs e)
        {
            object obj = _lbCisNG.SelectedItem;
            if (obj == null) return;

            if (obj.GetType() == typeof(Cgp.Server.Beans.CisNGGroup))
            {
                CisNGGroupsForm.Singleton.OpenEditForm(_lbCisNG.SelectedItem as CisNGGroup);
            }
            else if (obj.GetType() == typeof(Cgp.Server.Beans.CisNG))
            {
                CisNGsForm.Singleton.OpenEditForm(_lbCisNG.SelectedItem as CisNG);
            }
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.PresentationGroups != null)
                CgpClient.Singleton.MainServerProvider.PresentationGroups.EditEnd(_editingObject);
        }

        private bool _sendingTestMessage = false;

        private void _bTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (_eEmail.Text == string.Empty)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eEmail,
                           GetString("ErrorEnterMailAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _tcPG.SelectedTab = _tpEmail;
                    _eEmail.Focus();
                    return;
                }

                if (!IsAddressOk())
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eEmail,
                        GetString("ErrorWrongEmail"), CgpClient.Singleton.ClientControlNotificationSettings);
                    if (_tcPG.SelectedTab != _tpEmail)
                    {
                        _tcPG.SelectedTab = _tpEmail;
                    }
                    _eEmail.Focus();
                    return;
                }

                if (!_chbInherited.Checked)
                {
                    if (string.IsNullOrEmpty(_eSubject.Text))
                    {
                        IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eSubject,
                            GetString("ErrorEntryEmailSubject"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _eSubject.Focus();
                        return;
                    }
                }

                if (!CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.IsSetSMTP())
                {
                    if (CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.GeneralOptionsRemoteServiceSettingsAdmin)))
                    {
                        if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionSetSMTPNow")))
                        {
                            GeneralOptionsForm.Singleton.ShowSMTPSettings();
                            return;
                        }
                    }
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSMTPNotSet"));
                    return;
                }

                if (!_sendingTestMessage)
                {
                    Contal.IwQuick.Threads.SafeThread.StartThread(ThreadSendTestMessage);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Info(GetString("ErrorSendTestMailFailed"));
            }
        }

        private void ThreadSendTestMessage()
        {
            try
            {
                _sendingTestMessage = true;
                Exception error = null;
                _editingObject.Email = _eEmail.Text;
                _editingObject.InheritedEmailSubject = _chbInherited.Checked;
                _editingObject.EmailSubject = _eSubject.Text;
                if (CgpClient.Singleton.MainServerProvider.TestSendMailPresentationGroup(_editingObject, "Test mail", out error))
                {
                    Contal.IwQuick.UI.Dialog.Info(GetString("InfoTestMailSend"));
                }
                else
                {
                    if (error == null || error.Message == null)
                    {
                        Contal.IwQuick.UI.Dialog.Info(GetString("ErrorSendTestMailFailed"));
                    }
                    else
                    {
                        if (error.InnerException != null && error.InnerException.Message != null && error.InnerException.Message != string.Empty)
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSendTestMailFailed") + Environment.NewLine + error.InnerException.Message);
                        }
                        else
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSendTestMailFailed") + Environment.NewLine + error.Message);
                        }
                    }
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Info(GetString("ErrorSendTestMailFailed"));
            }
            finally
            {
                _sendingTestMessage = false;
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.PresentationGroups.
                GetReferencedObjects(_editingObject.IdGroup as object, CgpClient.Singleton.GetListLoadedPlugins());
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

        private void _eFormatterName_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                SetFormatter();
            }
            else if (item.Name == "_tsiRemove")
            {
                _tbmFormatterName.Text = string.Empty;
                _eMsgFormat.Text = string.Empty;
                _editingObject.PresentationFormatter = null;
            }
        }

        private void _bCreateCisNG_Click(object sender, EventArgs e)
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

        private void _bCreateCisNGGroup_Click(object sender, EventArgs e)
        {
            CisNGGroup cisNgGrup = new CisNGGroup();
            CisNGGroupsForm.Singleton.OpenInsertFromEdit(ref cisNgGrup, DoAfterCisNGGroupCreated);
        }

        private void DoAfterCisNGGroupCreated(object newCisNGGroup)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCisNGGroupCreated), newCisNGGroup);
            }
            else
            {
                _editingObject.CisNGGroup.Add((CisNGGroup)newCisNGGroup);
                ShowCisNGs();
            }
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            if (_editingObject.CisNG.Count == 0 && _editingObject.CisNGGroup.Count == 0)
                return;

            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionClearAllCisNGs")))
            {
                _editingObject.CisNG.Clear();
                _editingObject.CisNGGroup.Clear();
                ShowCisNGs();
            }
        }

        private void _chbInherited_CheckedChanged(object sender, EventArgs e)
        {
            _eSubject.Enabled = !_chbInherited.Checked;
            EditTextChanger(sender, e);
        }
    }
}
