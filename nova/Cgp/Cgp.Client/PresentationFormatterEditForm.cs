using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Threads;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class PresentationFormatterEditForm :
#if DESIGNER
        Form
#else
        ACgpEditForm<PresentationFormatter>
#endif
    {
        private const string FORMATTER_SUBSTRING = "%msg";

        public PresentationFormatterEditForm(PresentationFormatter presentationFormatter, ShowOptionsEditForm showOption)
            : base(presentationFormatter, showOption)
        {
            InitializeComponent();
            _editingObject = presentationFormatter;
            SetReferenceEditColors();
            WheelTabContorol = _tcFormaterSettings;
            this.MinimumSize = new Size(this.Width, this.Height);
            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);

            if (
                !CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersAlarmSettingsAdmin)))
            {
                _eFormatterName.ReadOnly = true;
                _eFormatMsg.ReadOnly = true;
            }

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
                    _tcFormaterSettings.TabPages.Remove(_tpUserFolders);
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
                    _tcFormaterSettings.TabPages.Remove(_tpReferencedBy);
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
                    _tcFormaterSettings.TabPages.Remove(_tpDescription);
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
                Text = LocalizationHelper.GetString("PresentationFormatterEditFormInsertText");
            }
            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            PresentationFormattersForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            PresentationFormattersForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            PresentationFormattersForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            PresentationFormattersForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            PresentationFormatter obj = CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectForEdit(_editingObject.IdFormatter, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectById(_editingObject.IdFormatter);
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
            CgpClient.Singleton.MainServerProvider.PresentationFormatters.RenewObjectForEdit(_editingObject.IdFormatter, out error);
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
            _eFormatterName.Text = _editingObject.FormatterName;
            _eFormatMsg.Text = _editingObject.MessageFormat;
            _eDescription.Text = _editingObject.Description;
            SetReferencedBy();
            UserFolders_Enter(_lbUserFolders);
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eFormatterName.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFormatterName,
                        GetString("ErrorInsertFormatterName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eFormatterName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eFormatMsg.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFormatMsg,
                        GetString("ErrorInsertForamtterMsg"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eFormatMsg.Focus();
                return false;
            }
            else
            {
                //formatter message must contain
                // %msg - it will be substituted by send message content
                if (!_eFormatMsg.Text.Contains(FORMATTER_SUBSTRING))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFormatMsg,
                        GetString("ErrorIncorrectForamtterMsg"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eFormatMsg.Focus();
                    return false;
                }

            }
            return true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.FormatterName = _eFormatterName.Text;
                _editingObject.MessageFormat = _eFormatMsg.Text;
                _editingObject.Description = _eDescription.Text;
                return true;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.PresentationFormatters.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFormatterName,
                    GetString("ErrorUsedFormatterName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eFormatterName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.PresentationFormatters.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFormatterName,
                    GetString("ErrorUsedFormatterName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eFormatterName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.PresentationFormatters != null)
                CgpClient.Singleton.MainServerProvider.PresentationFormatters.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.PresentationFormatters.
                GetReferencedObjects(_editingObject.IdFormatter as object, CgpClient.Singleton.GetListLoadedPlugins());
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
