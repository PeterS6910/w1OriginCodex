using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.UI;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class AlarmDetailsForm :
#if DESIGNER
        Form,
#else
 CgpTranslateForm,
#endif
 IAlarmDetailsForm
    {
        private class AlarmImageListBoxItem : ImageListBoxItem
        {
            private readonly AlarmDetailsForm _alarmsDetailForm;
            private readonly IdAndObjectType _idAndObjectType;

            public AlarmImageListBoxItem(
                AlarmDetailsForm alarmDetailsForm,
                AOrmObject ormObject)
                : base(
                    ormObject,
                    ormObject.GetObjectType().ToString())
            {
                _alarmsDetailForm = alarmDetailsForm;

                _idAndObjectType = new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());
            }

            public override string ToString()
            {
                var serverAlarm = _alarmsDetailForm._serverAlarm;

                var specificName = serverAlarm != null
                    ? serverAlarm.GetSpecificObjectName(_idAndObjectType)
                    : null;

                return !string.IsNullOrEmpty(specificName)
                    ? specificName
                    : base.ToString();
            }
        }

        private const string SERVER_ALARM_IMAGE_KEY = "ServerAlarm";

        private ServerAlarmCore _serverAlarm;
        private BindingSource _bindingSourceAlarmInstructions;
        private AlarmDetailsForm _referencedServerAlarmForm;

        public AlarmDetailsForm(ServerAlarmCore serverAlarm)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            _serverAlarm = serverAlarm;
            ShowAlarmParentObjectList();
            ShowAlarm();
            CgpClientMainForm.Singleton.AlarmChanged += RefreshWindow;
            CgpClientMainForm.Singleton.AlarmDeleted += ProcessDeletedAlarm;
            ColorSettingsChangedHandler.Singleton.RegisterColorChanged(ColorSettingsChanged);
            KeyPreview = true;
            KeyDown += FormKeyDown;

            var objectImageList = ObjectImageList.Singleton.GetAllObjectImages();

            objectImageList.Images.Add(
                SERVER_ALARM_IMAGE_KEY,
                ResourceGlobal.IconAlarm16);

            _ilbParentObject.ImageList = objectImageList;
        }

        void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        protected override void AfterTranslateForm()
        {
            base.AfterTranslateForm();
            if (_serverAlarm != null)
            {
                _lAlarmState.Text = GetString("InfoAlarmState");
            }
        }


        public void SetAnotherAlarm(ServerAlarmCore serverAlarm)
        {
            _serverAlarm = serverAlarm;
            ShowAlarmParentObjectList();
            ShowAlarm();

            if (_referencedServerAlarmForm != null
                && !_referencedServerAlarmForm.IsDisposed)
            {
                _referencedServerAlarmForm.Close();
            }

            _referencedServerAlarmForm = null;
        }

        private void RefreshWindow(ICollection<ServerAlarmCore> serverAlarms)
        {
            foreach (var newServerAlarm in serverAlarms)
            {
                var idServerAlarm = _serverAlarm.IdServerAlarm;

                if (idServerAlarm.Equals(newServerAlarm.IdServerAlarm))
                {
                    _serverAlarm = newServerAlarm;

                    SuspendLayout();
                    ShowAlarm();
                    ResumeLayout();
                }

                if (_serverAlarm.ReferencedServerAlarm != null)
                {
                    if (_serverAlarm.ReferencedServerAlarm.IdServerAlarm.Equals(newServerAlarm.IdServerAlarm))
                    {
                        _serverAlarm.ReferencedServerAlarm = newServerAlarm;

                        SuspendLayout();
                        ShowAlarmParentObjectList();
                        ResumeLayout();
                    }
                }
            }
        }

        private void ProcessDeletedAlarm(IdServerAlarm deletedIdServerAlarm)
        {
            if (_serverAlarm.IdServerAlarm.Equals(deletedIdServerAlarm))
            {
                CloseAlarmDetails();
            }
        }

        private void CloseAlarmDetails()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(CloseAlarmDetails));
            }
            else
            {
                Close();
            }
        }

        private void ShowRelatedObjects(
            IEnumerable<IdAndObjectType> alarmRelatedObjects,
            ServerAlarmCore referencedServerAlarm)
        {
            foreach (var relatedObject in alarmRelatedObjects)
            {
                AOrmObject ormObject = DbsSupport.GetTableObject(
                    relatedObject.ObjectType,
                    (Guid) relatedObject.Id);

                if (ormObject != null)
                {
                    AddToParentObjectListBox(
                        new AlarmImageListBoxItem(
                            this,
                            ormObject));
                }
            }

            if (referencedServerAlarm == null)
                return;

            AddToParentObjectListBox(
                new ImageListBoxItem(
                    referencedServerAlarm,
                    SERVER_ALARM_IMAGE_KEY));
        }

        private void AddToParentObjectListBox(
            ImageListBoxItem imageListBoxItem)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ImageListBoxItem>(AddToParentObjectListBox),
                    imageListBoxItem);
            }
            else
            {
                _ilbParentObject.Items.Add(imageListBoxItem);
            }
        }

        private void ColorSettingsChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ColorSettingsChanged));
            }
            else
            {
                var alarmRelatedObjects = _serverAlarm.RelatedObjects;

                if (alarmRelatedObjects != null)
                {
                    _ilbParentObject.ForeColor = CgpClientMainForm.Singleton.GetReferenceTextColor;
                    _ilbParentObject.BackColor = CgpClientMainForm.Singleton.GetReferenceBackgroundColor;
                }
                else
                {
                    _ilbParentObject.ForeColor = Color.Black;
                    _ilbParentObject.BackColor = Color.White;
                }

                var alarm = _serverAlarm.Alarm;

                _eAlarmState.BackColor = GeneralOptionsForm.Singleton.GetAlarmStateColorBackground(
                    _serverAlarm.IsBlocked,
                    alarm.AlarmState,
                    _serverAlarm.IsAcknowledged);

                _eAlarmState.ForeColor = GeneralOptionsForm.Singleton.GetAlarmStateColorText(
                    _serverAlarm.OwnerIsOffline,
                    _serverAlarm.IsBlocked,
                    alarm.AlarmState,
                    _serverAlarm.IsAcknowledged);
            }
        }

        private void ShowAlarmParentObjectList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ShowAlarmParentObjectList));
            }
            else
            {
                _ilbParentObject.Items.Clear();
                _ilbParentObject.Items.Add(new ImageListBoxItem(_serverAlarm.ParentObject));

                var alarmRelatedObjects = _serverAlarm.RelatedObjects;

                if (alarmRelatedObjects != null)
                {
                    _ilbParentObject.ForeColor = CgpClientMainForm.Singleton.GetReferenceTextColor;
                    _ilbParentObject.BackColor = CgpClientMainForm.Singleton.GetReferenceBackgroundColor;
                    _ilbParentObject.Items.Clear();

                    IwQuick.Threads.SafeThread<IEnumerable<IdAndObjectType>, Guid>.StartThread(
                        ShowRelatedObjects,
                        alarmRelatedObjects,
                        _serverAlarm.ReferencedServerAlarm);
                }
                else
                {
                    _ilbParentObject.BackColor = Color.White;
                    _ilbParentObject.ForeColor = Color.Black;
                }
            }
        }

        private void ShowAlarm()
        {
            if (InvokeRequired)
            {
                Invoke(new Contal.IwQuick.DVoid2Void(ShowAlarm));
            }
            else
            {
                _eCreatedDateTime.Text = _serverAlarm.Alarm.CreatedDateTime.ToString();
                _eRelativeUniqueIdentificate.Text = _serverAlarm.Name;
                _eDescription.Text = _serverAlarm.Description;

                var alarmParameters = _serverAlarm.Alarm.AlarmKey.Parameters;

                if (alarmParameters != null)
                {
                    if (!_tcAlarmDetails.TabPages.Contains(_tpParameters))
                        _tcAlarmDetails.TabPages.Insert(1, _tpParameters);

                    if (!_dgvParameters.Columns.Contains("ParameterType"))
                    {
                        _dgvParameters.Columns.Add("ParameterType", "ParameterType");
                    }
                    if (!_dgvParameters.Columns.Contains("ParameterValue"))
                    {
                        _dgvParameters.Columns.Add("ParameterValue", "ParameterValue");
                    }

                    _dgvParameters.Rows.Clear();

                    foreach (AlarmParameter alarmParameter in alarmParameters)
                    {
                        if (alarmParameter.Value == null)
                            continue;

                        _dgvParameters.Rows.Add(
                            LocalizationHelper.GetString("ParameterType_" + alarmParameter.TypeParameter),
                            alarmParameter.Value);
                    }

                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvParameters);

                    if (_dgvParameters.Columns.Contains("ParameterType"))
                    {
                        _dgvParameters.Columns["ParameterType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    if (_dgvParameters.Columns.Contains("ParameterValue"))
                    {
                        _dgvParameters.Columns["ParameterValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
                else
                {
                    if (_tcAlarmDetails.TabPages.Contains(_tpParameters))
                        _tcAlarmDetails.TabPages.Remove(_tpParameters);
                }

                Contal.IwQuick.Threads.SafeThread.StartThread(ShowAlarmInstructions);
                RefreshAlarmState();
                RefreshAcknowledgeState();
                RefreshBlockUnblock();
            }
        }

        private void ShowAlarmInstructions()
        {
            var alarmRelatedObjects = _serverAlarm.RelatedObjects;

            if (alarmRelatedObjects == null)
                return;

            var closestAlarmInstrunctions = new List<AlarmInstruction>();
            var secondClosestAlarmInstrunctions = new List<AlarmInstruction>();

            var addedGlobalAlarmInstructionsGuid = new HashSet<Guid>();

            var alarmType = _serverAlarm.Alarm.AlarmKey.AlarmType;

            ObjectType? closestParentObject =
                CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.GetClosestParentObject(
                    alarmType);

            ObjectType? secondClosestParentObject =
                CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.GetSecondClosestParentObject(
                    alarmType);

            foreach (var relatedObject in alarmRelatedObjects)
            {
                if (relatedObject != null && closestParentObject != ObjectType.NotSupport &&
                    (closestParentObject == ObjectType.AllObjectTypes ||
                     CompareObjectTypes(relatedObject.ObjectType, closestParentObject) ||
                     CompareObjectTypes(relatedObject.ObjectType, secondClosestParentObject)))
                {
                    AOrmObject aOrmObject = DbsSupport.GetTableObject(
                        relatedObject.ObjectType,
                        (Guid) relatedObject.Id);

                    var iOrmObjectWithAlarmInstructions =
                        aOrmObject as IOrmObjectWithAlarmInstructions;

                    if (iOrmObjectWithAlarmInstructions != null)
                    {
                        string localAlarmInstruciton = iOrmObjectWithAlarmInstructions.GetLocalAlarmInstruction();

                        if (!string.IsNullOrEmpty(localAlarmInstruciton))
                        {
                            if (secondClosestParentObject == relatedObject.ObjectType)
                                secondClosestAlarmInstrunctions.Add(new AlarmInstruction(localAlarmInstruciton,
                                    aOrmObject.GetObjectType(), null));
                            else
                                closestAlarmInstrunctions.Add(new AlarmInstruction(localAlarmInstruciton,
                                    aOrmObject.GetObjectType(), null));
                        }

                        List<GlobalAlarmInstruction> globalAlarmInstructions =
                            CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions
                                .GetGlobalAlarmInstructionsForObject(aOrmObject.GetObjectType(),
                                    aOrmObject.GetIdString());
                        if (globalAlarmInstructions != null && globalAlarmInstructions.Count > 0)
                        {
                            foreach (GlobalAlarmInstruction globalAlarmInstruction in globalAlarmInstructions)
                            {
                                if (globalAlarmInstruction != null &&
                                    !addedGlobalAlarmInstructionsGuid.Contains(
                                        globalAlarmInstruction.IdGlobalAlarmInstruction))
                                {
                                    if (secondClosestParentObject == relatedObject.ObjectType)
                                        secondClosestAlarmInstrunctions.Add(
                                            new AlarmInstruction(globalAlarmInstruction.Instructions,
                                                aOrmObject.GetObjectType(), globalAlarmInstruction));
                                    else
                                        closestAlarmInstrunctions.Add(
                                            new AlarmInstruction(globalAlarmInstruction.Instructions,
                                                aOrmObject.GetObjectType(), globalAlarmInstruction));

                                    addedGlobalAlarmInstructionsGuid.Add(
                                        globalAlarmInstruction.IdGlobalAlarmInstruction);
                                }
                            }
                        }
                    }
                }
            }

            List<AlarmInstruction> alarmInstrunctions = new List<AlarmInstruction>();
            if (closestAlarmInstrunctions != null && closestAlarmInstrunctions.Count > 0)
                alarmInstrunctions.AddRange(closestAlarmInstrunctions);

            if (secondClosestAlarmInstrunctions != null && secondClosestAlarmInstrunctions.Count > 0)
                alarmInstrunctions.AddRange(secondClosestAlarmInstrunctions);

            if (alarmInstrunctions != null && alarmInstrunctions.Count > 0)
            {
                Invoke(new Contal.IwQuick.DVoid2Void(
                    delegate()
                    {
                        if (!_tcAlarmDetails.TabPages.Contains(_tpAlarmInstructions))
                            _tcAlarmDetails.TabPages.Insert(1, _tpAlarmInstructions);

                        _bindingSourceAlarmInstructions = new BindingSource();
                        _bindingSourceAlarmInstructions.DataSource = alarmInstrunctions;
                        _dgAlarmInstructions.DataSource = _bindingSourceAlarmInstructions;

                        if (_dgAlarmInstructions.Columns.Contains("ObjectIcon"))
                        {
                            _dgAlarmInstructions.Columns["ObjectIcon"].AutoSizeMode =
                                DataGridViewAutoSizeColumnMode.DisplayedCells;
                        }

                        if (_dgAlarmInstructions.Columns.Contains("AlarmInstructions"))
                            _dgAlarmInstructions.Columns["AlarmInstructions"].AutoSizeMode =
                                DataGridViewAutoSizeColumnMode.Fill;


                        if (_dgAlarmInstructions.Columns.Contains("GlobalAlarmInstructionGuid"))
                            _dgAlarmInstructions.Columns["GlobalAlarmInstructionGuid"].Visible = false;
                    }));
            }
            else
            {
                Invoke(new Contal.IwQuick.DVoid2Void(
                    delegate()
                    {
                        if (_tcAlarmDetails.TabPages.Contains(_tpAlarmInstructions))
                            _tcAlarmDetails.TabPages.Remove(_tpAlarmInstructions);

                        _bindingSourceAlarmInstructions = null;
                        _dgAlarmInstructions.DataSource = null;
                    }));
            }
        }

        public static bool CompareObjectTypes(ObjectType parentObjectType, ObjectType? closestObjectType)
        {
            if (closestObjectType == null)
                return false;

            if (closestObjectType == ObjectType.DoorEnvironment)
                return parentObjectType == ObjectType.DoorEnvironment || parentObjectType == ObjectType.DCU;

            return parentObjectType == closestObjectType;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);

            if (_referencedServerAlarmForm != null
                && !_referencedServerAlarmForm.IsDisposed)
            {
                _referencedServerAlarmForm.Close();
            }

            _referencedServerAlarmForm = null;

            CgpClientMainForm.Singleton.AlarmChanged -= RefreshWindow;
            CgpClientMainForm.Singleton.AlarmDeleted -= ProcessDeletedAlarm;
            CgpClientMainForm.Singleton.AlarmsDetailDialogClose(this);
            ColorSettingsChangedHandler.Singleton.UnregisterColorChanged(ColorSettingsChanged);
        }

        private void _bConfirmState_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            var newServerAlarm = CgpClient.Singleton.MainServerProvider.AcknowledgeAlarmState(_serverAlarm.IdServerAlarm);

            if (newServerAlarm != null
                && (!Equals(CgpClientMainForm.Singleton._alarmDetails)
                    || newServerAlarm.IsBlocked
                    || newServerAlarm.Alarm.AlarmState == AlarmState.Alarm
                    || !newServerAlarm.Alarm.IsAcknowledged))
            {
                _serverAlarm = newServerAlarm;
            }
            else
            {
                Close();
            }

            RefreshAlarmState();
            RefreshAcknowledgeState();
        }

        private void RefreshAcknowledgeState()
        {
            _pbAcknowledgement.Image = _serverAlarm.AcknowledgeInPending
                ? ResourceGlobal.IconAcknowledgeInPending20.ToBitmap()
                : _serverAlarm.Alarm.IsAcknowledged
                    ? ResourceGlobal.IconNewAcknowledged20.ToBitmap()
                    : ResourceGlobal.IconWarning20.ToBitmap();

            if (!_serverAlarm.IsAcknowledged)
            {
                _bAcknowledgeState.Enabled = CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.AlarmsAlarmsAdmin));
                _bAcknowledgeAndBlock.Enabled = _bBlock.Visible;
            }
            else
            {
                _bAcknowledgeState.Enabled = false;
                _bAcknowledgeAndBlock.Enabled = false;
            }
        }

        private void RefreshAlarmState()
        {
            if (_serverAlarm == null)
                return;

            var alarm = _serverAlarm.Alarm;

            _eAlarmState.Text = GetString("AlarmStates_" + alarm.AlarmState);

            _eAlarmState.BackColor = GeneralOptionsForm.Singleton.GetAlarmStateColorBackground(
                _serverAlarm.IsBlocked,
                alarm.AlarmState,
                _serverAlarm.IsAcknowledged);

            _eAlarmState.ForeColor = GeneralOptionsForm.Singleton.GetAlarmStateColorText(
                _serverAlarm.OwnerIsOffline,
                _serverAlarm.IsBlocked,
                alarm.AlarmState,
                _serverAlarm.IsAcknowledged);
        }

        private void RefreshBlockUnblock()
        {
            if (_serverAlarm == null)
                return;

            _pbIndividualBlockingState.Image = _serverAlarm.IndividualBlockinInPending != null
                ? ResourceGlobal.IconBlockingInPending20.ToBitmap()
                : _serverAlarm.IndividualUnblockinInPending != null
                    ? ResourceGlobal.IconUnblockingInPending20.ToBitmap()
                    : _serverAlarm.Alarm.IsBlockedIndividual
                        ? ResourceGlobal.IconNewBlocked20.ToBitmap()
                        : ResourceGlobal.IconUnblocked20.ToBitmap();

            _pbGeneralBlockingState.Image = _serverAlarm.Alarm.IsBlockedGeneral
                ? ResourceGlobal.IconNewBlocked20.ToBitmap()
                : ResourceGlobal.IconUnblocked20.ToBitmap();

            if (_serverAlarm.IsBlockedIndividual)
            {
                _bBlock.Visible = false;
                _bUnblock.Visible = CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsAdmin));
                _bAcknowledgeAndBlock.Enabled = false;
            }
            else
            {
                _bBlock.Visible = CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsAdmin));
                _bUnblock.Visible = false;
                _bAcknowledgeAndBlock.Enabled = _bAcknowledgeState.Enabled;
            }
        }

        private void _ilbParentObject_DoubleClick(object sender, EventArgs e)
        {
            object selectedObject = _ilbParentObject.SelectedItemObject;

            var ormObject = selectedObject as AOrmObject;

            if (ormObject != null)
            {
                DbsSupport.OpenEditForm(ormObject);
                return;
            }

            var serverAlarm = selectedObject as ServerAlarmCore;

            if (serverAlarm != null)
            {
                if (_referencedServerAlarmForm == null
                    || _referencedServerAlarmForm.IsDisposed)
                {
                    _referencedServerAlarmForm = new AlarmDetailsForm(serverAlarm);
                    _referencedServerAlarmForm.Show();
                }
            }
        }

        private void _bBlock_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionConfirmBlockAlarm")))
            {
                _serverAlarm = CgpClient.Singleton.MainServerProvider.BlockAlarm(_serverAlarm.IdServerAlarm);

                RefreshAlarmState();
                RefreshBlockUnblock();
            }
        }

        private void _bUnblock_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            _serverAlarm = CgpClient.Singleton.MainServerProvider.UnblockAlarm(_serverAlarm.IdServerAlarm);

            RefreshAlarmState();
            RefreshBlockUnblock();
        }

        #region IAlarmDetailsForm Members

        public IdServerAlarm GetIdServerAlarm()
        {
            return _serverAlarm.IdServerAlarm;
        }

        #endregion

        private void _dgAlarmInstructions_SelectionChanged(object sender, EventArgs e)
        {
            if (_bindingSourceAlarmInstructions != null && _bindingSourceAlarmInstructions.Count > 0)
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                if (
                    !CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccessesForGroup(LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS)))
                {
                    _bEdit.Enabled = false;
                    return;
                }

                AlarmInstruction alarmInstruction = _bindingSourceAlarmInstructions[_bindingSourceAlarmInstructions.Position] as AlarmInstruction;
                if (alarmInstruction == null || alarmInstruction.GlobalAlarmInstructionGuid == Guid.Empty)
                {
                    _bEdit.Enabled = false;
                }
                else
                {
                    _bEdit.Enabled = true;
                }
            }
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (_bindingSourceAlarmInstructions != null && _bindingSourceAlarmInstructions.Count > 0)
            {
                AlarmInstruction alarmInstruction = _bindingSourceAlarmInstructions[_bindingSourceAlarmInstructions.Position] as AlarmInstruction;
                if (alarmInstruction != null && alarmInstruction.GlobalAlarmInstructionGuid != Guid.Empty)
                {
                    GlobalAlarmInstruction globalAlarmInstruction = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(alarmInstruction.GlobalAlarmInstructionGuid);

                    if (globalAlarmInstruction != null)
                        GlobalAlarmInstructionsForm.Singleton.OpenEditForm(globalAlarmInstruction);
                }
            }
        }

        private void _dgAlarmInstructions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_dgAlarmInstructions.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            _bEdit_Click(null, null);
        }

        private void _ilbParentObject_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _ilbParentObject.SelectedIndex = _ilbParentObject.IndexFromPoint(e.X, e.Y);

                if (_ilbParentObject.SelectedItem != null)
                    Clipboard.SetText(_ilbParentObject.SelectedItem.ToString());
            }
        }

        private void _bAcknowledgeAndBlock_Click(object sender, EventArgs e)
        {
            _bConfirmState_Click(sender, e);
            _bBlock_Click(sender, e);
        }
    }

    public class AlarmInstruction
    {
        private Icon _objectIcon;
        private string _alarmInstructions;
        private Guid _globalAlarmInstructionGuid;

        public Icon ObjectIcon { get { return _objectIcon; } }
        public string AlarmInstructions { get { return _alarmInstructions; } }
        public Guid GlobalAlarmInstructionGuid { get { return _globalAlarmInstructionGuid; } }

        public AlarmInstruction(string alarmInstructions, ObjectType objectType, GlobalAlarmInstruction globalAlarmInstruction)
        {
            _alarmInstructions = alarmInstructions;
            _objectIcon = DbsSupport.GetIconForObjectType(objectType);
            if (globalAlarmInstruction != null)
            {
                _globalAlarmInstructionGuid = globalAlarmInstruction.IdGlobalAlarmInstruction;
                _alarmInstructions = globalAlarmInstruction.Name + ":" + Environment.NewLine + Environment.NewLine + _alarmInstructions;
            }
            else
            {
                _globalAlarmInstructionGuid = Guid.Empty;
            }
        }
    }
}
