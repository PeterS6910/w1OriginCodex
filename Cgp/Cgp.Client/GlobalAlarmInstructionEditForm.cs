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
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Threads;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class GlobalAlarmInstructionEditForm :
#if DESIGNER
    Form
#else
    ACgpEditForm<GlobalAlarmInstruction>
#endif
    {
        public GlobalAlarmInstructionEditForm(GlobalAlarmInstruction globalAlarmInstruction, ShowOptionsEditForm showOption)
            : base(globalAlarmInstruction, showOption)
        {
            InitializeComponent();
            SetReferenceEditColors();
            WheelTabContorol = _tcGlobalAlarmInstruction;
            this.MinimumSize = new Size(this.Width, this.Height);
            _eInstructions.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageAlarmInstructions(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.GlobalAlarmInstructionsAlarmInstructionsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.GlobalAlarmInstructionsAlarmInstructionsAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.GlobalAlarmInstructionsReferencedByView)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageAlarmInstructions(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAlarmInstructions),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = false;
                }

                if (!view && !admin)
                {
                    _tcGlobalAlarmInstruction.TabPages.Remove(_tpInstructions);
                    return;
                }

                _tpInstructions.Enabled = admin;
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
                    _tcGlobalAlarmInstruction.TabPages.Remove(_tpUserFolders);
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
                    _tcGlobalAlarmInstruction.TabPages.Remove(_tpReferencedBy);
                    return;
                }
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
                Text = LocalizationHelper.GetString("GlobalAlarmInstructionEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            GlobalAlarmInstructionsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            GlobalAlarmInstructionsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            GlobalAlarmInstructionsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            GlobalAlarmInstructionsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            GlobalAlarmInstruction obj = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectForEdit(_editingObject.IdGlobalAlarmInstruction, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(_editingObject.IdGlobalAlarmInstruction);
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
            CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.RenewObjectForEdit(_editingObject.IdGlobalAlarmInstruction, out error);
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
            _eInstructions.Text = _editingObject.Instructions;

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
                _editingObject.Name = _eName.Text;
                _editingObject.Instructions = _eInstructions.Text;
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
                        GetString("ErrorInsertGlobalAlarmInstructionName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
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
            bool retValue = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message.Contains("Name"))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedGlobalAlarmInstructionName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                    return false;
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message.Contains("Name"))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedGlobalAlarmInstructionName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                    return false;
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
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions != null)
                CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.
                GetReferencedObjects(_editingObject.IdGlobalAlarmInstruction as object, CgpClient.Singleton.GetListLoadedPlugins());
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
