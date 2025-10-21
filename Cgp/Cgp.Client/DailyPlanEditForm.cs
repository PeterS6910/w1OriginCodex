using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class DailyPlanEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<DailyPlan>
#endif
    {
        public DailyPlanEditForm(DailyPlan dailyPlan, ShowOptionsEditForm showOption)
            : base(dailyPlan, showOption)
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            _dmDailyPlan.SelectedHour += _dmDailyPlan_SelectedHour;
            _dmDailyPlan.WasChanged += EditTextChanger;

            SetReferenceEditColors();
            WheelTabContorol = _tcDailyPlan;
            this.MinimumSize = new Size(this.Width, this.Height);

            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);

            SafeThread.StartThread(HideDisableTabPages);
            
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            var localization = new Dictionary<byte, string>
            {
                {0, GetString("TimeZonesEditForm_lStatusOff")}, 
                {1, GetString("TimeZonesEditForm_lStatusOn")}
            };

            _dmDailyPlan.SetLocalization(localization);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSchedule(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesScheduleView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesScheduleAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesDescriptionAdmin)));
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
                    _tcDailyPlan.TabPages.Remove(_tpSchedule);
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
                    _tcDailyPlan.TabPages.Remove(_tpUserFolders);
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
                    _tcDailyPlan.TabPages.Remove(_tpReferencedBy);
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
                    _tcDailyPlan.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        void _dmDailyPlan_SelectedHour(int parameter1, MouseButtons parameter2)
        {
            EditTextChanger(null, null);
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
                Text = LocalizationHelper.GetString("DailyPlanEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            DailyPlansForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            DailyPlansForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            DailyPlansForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            DailyPlansForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            DailyPlan obj = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectForEdit(_editingObject.IdDailyPlan, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(_editingObject.IdDailyPlan);
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
                _dmDailyPlan.SetIntervals(GetIntervals(obj.DayIntervals));
            }
            _editingObject = obj;
        }

        private List<Interval> GetIntervals(ICollection<DayInterval> dayIntervals)
        {
            List<Interval> intervals = new List<Interval>();

            if (dayIntervals != null)
            {
                foreach (DayInterval interval in dayIntervals)
                {
                    intervals.Add(new Interval(interval.MinutesFrom, interval.MinutesTo, 1));
                }
            }

            return intervals;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
            Exception error;

            CgpClient.Singleton.MainServerProvider.DailyPlans.RenewObjectForEdit(_editingObject.IdDailyPlan, out error);
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
            switch (_editingObject.DailyPlanName)
            {
                case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                    _eName.Text = GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                    EnableEditControls(false);
                    break;
                case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                    _eName.Text = GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                    EnableEditControls(false);
                    break;
                case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                    _eName.Text = GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                    EnableEditControls(false);
                    break;
                case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                    _eName.Text = GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                    EnableEditControls(false);
                    break;
                default:
                    _eName.Text = _editingObject.DailyPlanName;
                    EnableEditControls(true);
                    break;
            }

            _eDescription.Text = _editingObject.Description;
            if (_editingObject.DayIntervals != null)
            {
                _dmDailyPlan.SetIntervals(GetIntervals(_editingObject.DayIntervals));
            }

            SetReferencedBy();
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.DailyPlans.
                GetReferencedObjects(_editingObject.IdDailyPlan as object, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void EnableEditControls(bool enable)
        {
            if (enable)
            {
                _dmDailyPlan.Enabled = true;
                _bOk.Enabled = true;
            }
            else
            {
                _eName.ReadOnly = true;
                _dmDailyPlan.Enabled = false;
                _bOk.Enabled = false;
            }
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
                if (_editingObject.DailyPlanName != DailyPlan.IMPLICIT_DP_ALWAYS_OFF &&
                    _editingObject.DailyPlanName != DailyPlan.IMPLICIT_DP_ALWAYS_ON &&
                    _editingObject.DailyPlanName != DailyPlan.IMPLICIT_DP_NIGHT_ON &&
                    _editingObject.DailyPlanName != DailyPlan.IMPLICIT_DP_DAYLIGHT_ON)
                {
                    _editingObject.DailyPlanName = _eName.Text;
                }
                _editingObject.Description = _eDescription.Text;
                if (_editingObject.DayIntervals == null)
                {
                    _editingObject.DayIntervals = new List<DayInterval>();
                }
                else
                {
                    _editingObject.DayIntervals.Clear();
                }

                foreach (DayInterval di in GetDayIntervals())
                {
                    _editingObject.DayIntervals.Add(di);
                }
                return true;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private List<DayInterval> GetDayIntervals()
        {
            List<DayInterval> dayIntervals = new List<DayInterval>();
            foreach (Interval item in _dmDailyPlan.GetIntervals())
            {
                if (item.Type == 1)
                {
                    DayInterval newDayInterval = new DayInterval();
                    newDayInterval.MinutesFrom = (short)item.MinutesFrom;
                    newDayInterval.MinutesTo = (short)item.MinutesTo;
                    dayIntervals.Add(newDayInterval);
                }
            }
            return dayIntervals;
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertDailyPlanName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }
            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.DailyPlans.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedDailyPlanName"), CgpClient.Singleton.ClientControlNotificationSettings);
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
                retValue = CgpClient.Singleton.MainServerProvider.DailyPlans.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = CgpClient.Singleton.MainServerProvider.DailyPlans.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedDailyPlanName"), CgpClient.Singleton.ClientControlNotificationSettings);
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

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.DailyPlans != null)
                CgpClient.Singleton.MainServerProvider.DailyPlans.EditEnd(_editingObject);
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
    }
}
