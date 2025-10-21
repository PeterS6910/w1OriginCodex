using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASSecurityDailyPlanEditForm :
#if DESIGNER
        Form
#else
 ACgpPluginEditForm<NCASClient, SecurityDailyPlan>
#endif
    {
        SecurityLevelColor _securityLevelColor = new SecurityLevelColor();

        public NCASSecurityDailyPlanEditForm(
                SecurityDailyPlan securityDailyPlan,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                securityDailyPlan,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            DoubleBuffered = true;
            InitializeComponent();

            _dmSecDailyPlan.WasChanged += EditTextChanger;
            _dmSecDailyPlan.SelectedHour += _dmSecDailyPlan_SelectedHour;

            _lColorUnlocked.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.unlocked);
            _lColorLocked.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.locked);
            _lColorCard.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.card);
            _lColorCardPin.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.cardpin);
            _lColorToggleCard.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.togglecard);
            _lColorToggleCardPin.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.togglecardpin);
            _lColorGin.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.code);
            _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.unlocked);
            _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.locked);
            SetReferenceEditColors();
            WheelTabContorol = _tcSdp;
            MinimumSize = new Size(Width, Height);

            _eDescription.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            var localization = new Dictionary<byte, string>
            {
                {0, GetString("NCASSecurityDailyPlanEditForm_lUnlocked")}, 
                {1, GetString("NCASSecurityDailyPlanEditForm_lLocked")},
                {2, GetString("NCASSecurityDailyPlanEditForm_lCard")},
                {3, GetString("NCASSecurityDailyPlanEditForm_lCardPin")},
                {4, GetString("NCASSecurityDailyPlanEditForm_lGin")},
                {5, GetString("NCASSecurityDailyPlanEditForm_lGinOrCard")},
                {6, GetString("NCASSecurityDailyPlanEditForm_lGinOrCardPin")}
            };

            _dmSecDailyPlan.SetLocalization(localization);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSchedule(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsScheduleView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsScheduleAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageSchedule(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSchedule),
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
                    _tcSdp.TabPages.Remove(_tpSchedule);
                    return;
                }

                _tpSchedule.Enabled = admin;
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
                    _tcSdp.TabPages.Remove(_tpUserFolders);
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
                    _tcSdp.TabPages.Remove(_tpReferencedBy);
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
                    _tcSdp.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        void _dmSecDailyPlan_SelectedHour(int parameter1, MouseButtons parameter2)
        {
            EditTextChanger(null, null);
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }


        void HoursBtn_SelectedHour(int hour, MouseButtons mouseButton)
        {
            EditTextChanger(null, null);
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("NCASSecurityDailyPlanEditFormInsertText");
            }
            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            NCASSecurityDailyPlansForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASSecurityDailyPlansForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASSecurityDailyPlansForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASSecurityDailyPlansForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            SecurityDailyPlan obj = Plugin.MainServerProvider.SecurityDailyPlans.GetObjectForEdit(_editingObject.IdSecurityDailyPlan, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(_editingObject.IdSecurityDailyPlan);
                }
                else
                {
                    throw error;
                }
                DisableForm();
            }
            else
            {
                allowEdit = true;
                _dmSecDailyPlan.SetIntervals(GetIntervals(obj.SecurityDayIntervals));
            }
            _editingObject = obj;
        }

        private List<Interval> GetIntervals(ICollection<SecurityDayInterval> dayIntervals)
        {
            List<Interval> intervals = new List<Interval>();

            if (dayIntervals != null)
            {
                foreach (SecurityDayInterval interval in dayIntervals)
                {
                    intervals.Add(new Interval(interval.MinutesFrom, interval.MinutesTo, interval.IntervalType));
                }
            }
            return intervals;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.SecurityDailyPlans.RenewObjectForEdit(_editingObject.IdSecurityDailyPlan, out error);

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
            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;

            if (_editingObject.SecurityDayIntervals != null)
            {
                _dmSecDailyPlan.SetIntervals(GetIntervals(_editingObject.SecurityDayIntervals));
            }
            SetReferencedBy();
        }

        protected override void DisableForm()
        {
            base.DisableForm();
            _bCancel.Enabled = true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;

                if (_editingObject.SecurityDayIntervals == null)
                {
                    _editingObject.SecurityDayIntervals = new List<SecurityDayInterval>();
                }
                else
                {
                    _editingObject.SecurityDayIntervals.Clear();
                }
                foreach (SecurityDayInterval di in GetDayIntervals())
                {
                    _editingObject.SecurityDayIntervals.Add(di);
                }

                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private List<SecurityDayInterval> GetDayIntervals()
        {
            List<SecurityDayInterval> dayIntervals = new List<SecurityDayInterval>();
            foreach (Interval item in _dmSecDailyPlan.GetIntervals())
            {
                SecurityDayInterval newDayInterval = new SecurityDayInterval();
                newDayInterval.MinutesFrom = (short)item.MinutesFrom;
                newDayInterval.MinutesTo = (short)item.MinutesTo;
                newDayInterval.IntervalType = item.Type;
                dayIntervals.Add(newDayInterval);
            }
            return dayIntervals;
        }


        private bool ContainsCardReadersWithoutKeyboard()
        {
            IList<AOrmObject> objects = GetListReferencedObjects();

            if (objects != null)
            {
                foreach (AOrmObject item in objects)
                {
                    if (item is CardReader)
                    {
                        CardReader cardReader = item as CardReader;
                        bool? hasKeyboard = Plugin.MainServerProvider.CardReaders.GetHasKeyboard(cardReader.IdCardReader);
                        if (((item as CardReader).SecurityLevel == (byte)SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
                            && (!hasKeyboard.HasValue || !hasKeyboard.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ContainsKeyboardNeededIntervals()
        {
            foreach (Interval di in _dmSecDailyPlan.GetIntervals())
            {
                if (di.Type == (byte)SecurityLevel4SLDP.cardpin
                    || di.Type == (byte)SecurityLevel4SLDP.code
                    || di.Type == (byte)SecurityLevel4SLDP.codeorcard
                    || di.Type == (byte)SecurityLevel4SLDP.codeorcardpin
                    || di.Type == (byte)SecurityLevel4SLDP.togglecardpin)
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertSecurityDailyPlanName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }
            if (ContainsKeyboardNeededIntervals())
            {
                if (ContainsCardReadersWithoutKeyboard())
                {
                    Dialog.Info(GetString("ErrorDayIntervalsUnsupportedByCardReader"));
                    return false;
                }
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
            bool retValue = Plugin.MainServerProvider.SecurityDailyPlans.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains("Name"))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedSecurityDailyPlanName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
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
            bool retValue;

            if (onlyInDatabase)
                retValue = Plugin.MainServerProvider.SecurityDailyPlans.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = Plugin.MainServerProvider.SecurityDailyPlans.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains("Name"))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedSecurityDailyPlanName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.SecurityDailyPlans != null)
                Plugin.MainServerProvider.SecurityDailyPlans.EditEnd(_editingObject);
        }

        private void ColorUnlocked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.unlocked);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.unlocked);

            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.unlocked);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.unlocked);
            }
        }

        private void ColorLocked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.locked);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.locked);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.locked);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.locked);
            }
        }

        private void ColorCard(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.card);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.card);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.card);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.card);
            }
        }

        private void ColorCardPin(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.cardpin);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.cardpin);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.cardpin);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.cardpin);
            }
        }

        private void ColorToggleCard(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.togglecard);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.togglecard);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.togglecard);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.togglecard);
            }
        }

        private void ColorToggleCardPin(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.togglecardpin);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.togglecardpin);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.togglecardpin);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.togglecardpin);
            }
        }

        private void ColorGin(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.code);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.code);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.code);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.code);
            }
        }

        private void ColorGinOrCard(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.codeorcard);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.codeorcard);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.codeorcard);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.codeorcard);
            }
        }

        private void ColorGinOrCardPin(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dmSecDailyPlan.SetLeftClickColor((int)SecurityLevel4SLDP.codeorcardpin);
                _lMouseColorLeft.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.codeorcardpin);
            }
            else
            {
                _dmSecDailyPlan.SetRightClickColor((int)SecurityLevel4SLDP.codeorcardpin);
                _lMouseColorRight.BackColor = _securityLevelColor.GetCollorForSecurityLevel(SecurityLevel4SLDP.codeorcardpin);
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.SecurityDailyPlans.
                GetReferencedObjects(_editingObject.IdSecurityDailyPlan, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }
    }
}
