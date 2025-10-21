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
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class DayTypeEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<DayType>
#endif
    {
        public DayTypeEditForm(DayType dayType, ShowOptionsEditForm showOption)
            : base(dayType, showOption)
        {
            InitializeComponent();
            _editingObject = dayType;
            SetReferenceEditColors();
            WheelTabContorol = _tcDayType;

            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesDescriptionAdmin)));
            }
            catch
            {
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
                    _tcDayType.TabPages.Remove(_tpUserFolders);
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
                    _tcDayType.TabPages.Remove(_tpReferencedBy);
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
                    _tcDayType.TabPages.Remove(_tpDescription);
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
                Text = LocalizationHelper.GetString("DayTypeEditFormInsertText");
            }
            if (_editingObject.ToString() == DayType.IMPLICIT_DAY_TYPE_HOLIDAY || _editingObject.ToString() == DayType.IMPLICIT_DAY_TYPE_VACATION)
                CgpClientMainForm.Singleton.SetTextOpenWindow(this, true);
            else
                CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            DayTypesForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            DayTypesForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            DayTypesForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            DayTypesForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            DayType obj = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectForEdit(_editingObject.IdDayType, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(_editingObject.IdDayType);
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
            CgpClient.Singleton.MainServerProvider.DayTypes.RenewObjectForEdit(_editingObject.IdDayType, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            _eName.Text = string.Empty;
        }

        protected override void SetValuesEdit()
        {
            if (_editingObject.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
            {
                _eName.Text = GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                _eName.ReadOnly = true;
            }
            else if (_editingObject.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                _eName.Text = GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                _eName.ReadOnly = true;
            }
            else
            {
                _eName.Text = _editingObject.DayTypeName;
                _eName.ReadOnly =
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesCalendarSettingsAdmin));
            }
            _eDescription.Text = _editingObject.Description;
            SetReferencedBy();
            UserFolders_Enter(_lbUserFolders);
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
                if (_editingObject.DayTypeName != DayType.IMPLICIT_DAY_TYPE_HOLIDAY
                    && _editingObject.DayTypeName != DayType.IMPLICIT_DAY_TYPE_VACATION)
                {
                    _editingObject.DayTypeName = _eName.Text;
                }
                _editingObject.Description = _eDescription.Text;
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
                        GetString("ErrorInsertDayTypeName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }
            return true;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.DayTypes.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedDayTypeName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.DayTypes.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedDayTypeName"), CgpClient.Singleton.ClientControlNotificationSettings);
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

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.DayTypes != null)
                CgpClient.Singleton.MainServerProvider.DayTypes.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.DayTypes.
                GetReferencedObjects(_editingObject.IdDayType as object, CgpClient.Singleton.GetListLoadedPlugins());
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

        protected override void AfterDockUndock()
        {
            if (this.MdiParent != null)
            {
                if (_editingObject.ToString() == DayType.IMPLICIT_DAY_TYPE_HOLIDAY || _editingObject.ToString() == DayType.IMPLICIT_DAY_TYPE_VACATION)
                    CgpClientMainForm.Singleton.SetTextOpenWindow(this, true);
            }
        }
    }
}
