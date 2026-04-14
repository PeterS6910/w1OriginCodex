using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Cgp.Components;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Drivers.CardReader;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.IwQuick.PlatformPC.UI.Accordion;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using System.Net;
using Contal.Cgp.NCAS.Definitions;
using Contal.IwQuick.Threads;

using CardReader = Contal.Cgp.NCAS.Server.Beans.CardReader;
using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASDCUEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditFormWithAlarmInstructions<DCU>
#endif
    {
        ICollection<CardReader> _actCardReaders;
        CCU _actCCU;
        private BindingSource _bindingSourceCRUpgrades = new BindingSource();

        private BindingSource _bindingSourceInput = new BindingSource();
        private BindingSource _bindingSourceOutput = new BindingSource();
        private Action<Guid, byte, Guid> _eventInputStateChanged;
        private Action<Guid, byte, Guid> _eventOutputStateChanged;
        private Action<Guid, byte, Guid> _eventOutputRealStateChanged;
        private Action<Guid, byte> _eventDoorEnvironmentStateChanged;
        private Action<Guid, OnlineState, Guid> _eventDCUOnlineStateChanged;
        private Action<Guid, string> _eventDCUPhysicalAddressChanged;
        private Action<Guid, State> _eventDcuInputsSabotageStateChanged;

        private Action<Guid, byte, Guid> _eventCardReaderOnlineStateChanged;
        private Action<Guid, byte> _eventMemoryWarningChanged;
        private Action<Guid, Guid> _outputEditChanged;
        private Action<Guid, Guid> _inputEditChanged;
        private Action<ObjectType, object, bool> _cudObjectEvent;
        private Action<IPAddress, int, byte> _eventFileTransferProgressChanged;
        private Action<CRUpgradeState> _eventCRsUpgradeProgressChanged;
        private AOrmObject _crInternal;
        private AOrmObject _crExternal;

        private DoorEnvironment _doorEnvironment;

        private OnlineState _dcuOnlineState = OnlineState.Unknown;

        private readonly ControlAlarmTypeSettings _catsDcuOffline;
        private readonly ControlModifyAlarmArcs _cmaaDcuOffline;
        private readonly ControlAlarmTypeSettings _catsDcuTamperSabotage;
        private readonly ControlModifyAlarmArcs _cmaaDcuTamperSabotage;

        private readonly Dictionary<AlarmType, Action> _openAndScrollToControlByAlarmType;

        public NCASDCUEditForm(
                DCU dcu,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                dcu,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            _openAndScrollToControlByAlarmType = new Dictionary<AlarmType, Action>
            {
                {
                    AlarmType.DCU_Offline,
                    () =>
                    {
                        _tcDCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsDcuOffline);
                    }
                },
                {
                    AlarmType.DCU_TamperSabotage,
                    () =>
                    {
                        _tcDCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsDcuTamperSabotage);
                    }
                }
            };

            InitializeComponent();

            _catsDcuOffline = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDcuOffline",
                TabIndex = 0,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsDcuOffline.EditTextChanger += () => EditTextChanger(null, null);
            _catsDcuOffline.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaDcuOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDcuOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDcuOffline.EditTextChanger += () => EditTextChanger(null, null);

            _catsDcuOffline.AddControl(_cmaaDcuOffline);

            _accordionAlarmSettings.Add(_catsDcuOffline, "DCU offline").Name = "_chbDcuOffline";

            _catsDcuTamperSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDcuTamperSabotage",
                TabIndex = 2,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null
            };

            _catsDcuTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);
            _catsDcuTamperSabotage.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaDcuTamperSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDcuTamperSabotage",
                TabIndex = 3,
                Plugin = Plugin
            };

            _cmaaDcuTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);

            _catsDcuTamperSabotage.AddControl(_cmaaDcuTamperSabotage);

            _accordionAlarmSettings.Add(_catsDcuTamperSabotage, "DCU tamper/sabotage").Name = "_chbDcuTamperSabotage";

            _accordionAlarmSettings.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllAlarmSettings,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllAlarmSettings, _accordionAlarmSettings.IsOpenAll);

            WheelTabContorol = _tcDCU;
            MinimumSize = new Size(Width, Height);

            if (Insert)
            {
                _lInputCount.Visible = true;
                _lOutputCount.Visible = true;
                _eOutputCount.Visible = true;
                _eInputCount.Visible = true;
            }
            _ilbCardReaders.ImageList = Plugin.ObjectImages;
            SetReferenceEditColors();
            InitCGPDataGridView();
            EnabledDisabledTpControl();

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void SetOpenAllCheckState(CheckBox checkBox, bool? isOpenAll)
        {
            checkBox.CheckState = isOpenAll.HasValue
                ? (isOpenAll.Value
                    ? CheckState.Checked
                    : CheckState.Unchecked)
                : CheckState.Indeterminate;
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusSettingsAdmin)));

                HideDisableTabPageInputsOutputs(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusInputsOutputsView)),
                    NCASInputsForm.Singleton.HasAccessView() &&
                    NCASOutputsForm.Singleton.HasAccessView());

                HideDisableTabPageControl(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusControlView)));

                HideDisableTabPageCrUpgrade(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusCrUpgradePerform)));

                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusAlarmSettingsAdmin)));

                HideDisableTabPageSpecialOutputs(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusSpecialOutputsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusSpecialOutputsAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusDescriptionAdmin)));
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
                    _tcDCU.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageInputsOutputs(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageInputsOutputs),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDCU.TabPages.Remove(_tpInpupOutputSettings);
                    return;
                }

                _tpInpupOutputSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageControl(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageControl),
                    view);
            }
            else
            {
                if (!view)
                {
                    _tcDCU.TabPages.Remove(_tpControl);
                }
            }
        }

        private void HideDisableTabPageCrUpgrade(bool perform)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageCrUpgrade),
                    perform);
            }
            else
            {
                if (!perform)
                {
                    _tcDCU.TabPages.Remove(_tpCRUpgrades);
                }
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
                if (!view && !admin)
                {
                    _tcDCU.TabPages.Remove(_tpAlarmSettings);
                    return;
                }

                _tpAlarmSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageSpecialOutputs(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSpecialOutputs),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDCU.TabPages.Remove(_tpSpecialOutputs);
                    return;
                }

                _tpSpecialOutputs.Enabled = admin;
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
                    _tcDCU.TabPages.Remove(_tpUserFolders);
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
                    _tcDCU.TabPages.Remove(_tpReferencedBy);
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
                    _tcDCU.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvInput.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvInput.DataGrid.DoubleClick += DgvInputDoubleClick;

            _cdgvOutput.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvOutput.DataGrid.DoubleClick += DgvOutputDoubleClick;
        }

        protected override void RegisterEvents()
        {
            RegisterServerEvents();
        }

        protected override void UnregisterEvents()
        {
            if (_eventInputStateChanged != null)
                StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventInputStateChanged);
            if (_eventOutputStateChanged != null)
                StateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventOutputStateChanged);
            if (_eventOutputRealStateChanged != null)
                RealStateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventOutputRealStateChanged);
            if (_eventDCUOnlineStateChanged != null)
                DCUOnlineStateChangedHandler.Singleton.UnregisterStateChanged(_eventDCUOnlineStateChanged);
            if (_eventDCUPhysicalAddressChanged != null)
                DCUPhysicalAddressChangedHandler.Singleton.UnregisterPhysicalAddressChanged(_eventDCUPhysicalAddressChanged);
            if (_eventDcuInputsSabotageStateChanged != null)
                DcuInputsSabotageStateChangedHandler.Singleton.UnregisterStateChanged(_eventDcuInputsSabotageStateChanged);
            if (_eventDoorEnvironmentStateChanged != null)
                DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(_eventDoorEnvironmentStateChanged);
            if (_eventCardReaderOnlineStateChanged != null)
                StateChangedCardReaderHandler.Singleton.UnregisterStateChanged(_eventCardReaderOnlineStateChanged);
            if (_eventMemoryWarningChanged != null)
                DCUMemoryWarningChangedHandler.Singleton.UnregisterMemoryWarningChanged(_eventMemoryWarningChanged);
            if (_outputEditChanged != null)
                OutputEditChangedHandler.Singleton.UnregisterOutputEditChanged(_outputEditChanged);
            if (_inputEditChanged != null)
                InputEditChangedHandler.Singleton.UnregisterInputEditChanged(_inputEditChanged);
            if (_cudObjectEvent != null)
                CUDObjectHandler.Singleton.Unregister(_cudObjectEvent, ObjectType.CardReader, ObjectType.Input, ObjectType.Output);
            if (_eventFileTransferProgressChanged != null)
                TFTPFileTransferProgressChangedHandler.Singleton.UnregisterTFTPFileTransferProgressChanged(_eventFileTransferProgressChanged);
            if (_eventCRsUpgradeProgressChanged != null)
                CRUpgradeProgressChangedHandler.Singleton.UnregisterUpgradeProgressChanged(_eventCRsUpgradeProgressChanged);
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("DcuEditFormInsertText");
            } 

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
            var dcuOnlineState = Plugin.MainServerProvider.DCUs.GetOnlineStates(_editingObject);

            if (dcuOnlineState == OnlineState.Online)
                LocalizationHelper.TranslateControl(_bCRMarkAsDead);
            else
                _bCRMarkAsDead.Text = GetString("General_bRemove");
        }

        protected override void BeforeInsert()
        {
            NCASDCUsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASDCUsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASDCUsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASDCUsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void AfterDockUndock()
        {
            ShowCRsUpgrade();
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.DCUs.GetObjectForEdit(_editingObject.IdDCU, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.DCUs.GetObjectById(_editingObject.IdDCU);
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
            }
            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.DCUs.RenewObjectForEdit(_editingObject.IdDCU, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            _bPrecreateCR.Enabled = false;
            _eLogicalAddress.Visible = false;
            _nudLogicalAddress.Left = _eLogicalAddress.Left;
            _nudLogicalAddress.Visible = true;
            EditTextChanger(_nudLogicalAddress, null);
            _editingObject.CardReaders = new List<CardReader>();

            _actCardReaders = new List<CardReader>();

            _actCCU = _editingObject.CCU;
            if (_actCCU != null)
            {
                _itbSuperiorCCU.Text = _actCCU.ToString();
                _itbSuperiorCCU.Image = Plugin.GetImageForAOrmObject(_actCCU);
            }

            ShowCardReaders();
            LoadDoorEnviroment();
            _bApply.Enabled = false;
        }

        //temporary sulution for disabling bypass alarm
        //protected override void AfterFormEnter()
        //{
        //    if (_doorEnvironment != null && _doorEnvironment.ActuatorsBypassAlarm != null)
        //    {
        //        _doorEnvironment.ActuatorsBypassAlarm = null;
        //        EditTextChanger(null, null);
        //    }
        //}

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.Name;
            if (_editingObject.Description != null)
                _eDescription.Text = _editingObject.Description;

            _actCCU = _editingObject.CCU;
            if (_actCCU != null)
            {
                _itbSuperiorCCU.Text = _actCCU.ToString();
                _itbSuperiorCCU.Image = Plugin.GetImageForAOrmObject(_actCCU);
            }

            if (_editingObject.LogicalAddress != 0)
                _eLogicalAddress.Text = _editingObject.LogicalAddress.ToString();
            else
                _eLogicalAddress.Text = string.Empty;


            _actCardReaders = new List<CardReader>();
            if (_editingObject.CardReaders != null)
            {
                foreach (var cardReader in _editingObject.CardReaders)
                {
                    var crFull = Plugin.MainServerProvider.CardReaders.GetObjectById(cardReader.IdCardReader);
                    if (crFull != null)
                        _actCardReaders.Add(crFull);
                }
            }

            var alarmArcsByAlarmType = new SyncDictionary<AlarmType, ICollection<AlarmArc>>();

            if (_editingObject.DcuAlarmArcs != null)
            {
                foreach (var dcuAlarmArc in _editingObject.DcuAlarmArcs)
                {
                    var alarmArc = dcuAlarmArc.AlarmArc;

                    alarmArcsByAlarmType.GetOrAddValue(
                        (AlarmType) dcuAlarmArc.AlarmType,
                        key =>
                            new LinkedList<AlarmArc>(),
                        (key, value, newlyAdded) =>
                            value.Add(alarmArc));
                }
            }

            _catsDcuOffline.AlarmEnabledCheckState = _editingObject.AlarmOffline;
            _catsDcuOffline.BlockAlarmCheckState = _editingObject.BlockAlarmOffline;

            _catsDcuOffline.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmOfflineObjectType,
                _editingObject.ObjBlockAlarmOfflineId);

            _catsDcuOffline.PresentationGroup = _editingObject.OfflinePresentationGroup;

            _cmaaDcuOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DCU_Offline);

            _catsDcuTamperSabotage.AlarmEnabledCheckState = _editingObject.AlarmTamper;
            _catsDcuTamperSabotage.BlockAlarmCheckState = _editingObject.BlockAlarmTamper;
            _catsDcuTamperSabotage.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmTamper;

            _catsDcuTamperSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmTamperObjectType,
                _editingObject.ObjBlockAlarmTamperId);

            _catsDcuTamperSabotage.PresentationGroup = _editingObject.TamperSabotagePresentationGroup;

            _cmaaDcuTamperSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DCU_TamperSabotage);

            if (_editingObject.DcuSabotageOutput != null)
            {
                _outputSabotageDcu = _editingObject.DcuSabotageOutput;
                _tbmOutputSabotageDcu.Text = _editingObject.DcuSabotageOutput.ToString();
                _tbmOutputSabotageDcu.TextImage = Plugin.GetImageForAOrmObject(_editingObject.DcuSabotageOutput);
            }
            else
            {
                _tbmOutputSabotageDcu.Text = string.Empty;
            }

            if (_editingObject.DcuOfflineOutput != null)
            {
                _outputOfflineDcu = _editingObject.DcuOfflineOutput;
                _tbmOutputOfflineDcu.Text = _editingObject.DcuOfflineOutput.ToString();
                _tbmOutputOfflineDcu.TextImage = Plugin.GetImageForAOrmObject(_editingObject.DcuOfflineOutput);
            }
            else
            {
                _tbmOutputOfflineDcu.Text = string.Empty;
            }

            if (_editingObject.DcuInputsSabotageOutput != null)
            {
                _outputSabotageDcuInputs = _editingObject.DcuInputsSabotageOutput;
                _tbmOutputSabotageDCUInputs.Text = _editingObject.DcuInputsSabotageOutput.ToString();
                _tbmOutputSabotageDCUInputs.TextImage = Plugin.GetImageForAOrmObject(_editingObject.DcuInputsSabotageOutput);
            }
            else
            {
                _tbmOutputSabotageDCUInputs.Text = string.Empty;
            }

            LoadDoorEnviroment();
            SetReferencedBy();
            RefreshDcuOnlineStateChanged(Plugin.MainServerProvider.DCUs.GetOnlineStates(_editingObject));
            DCUPhysicalAddressChanged(_editingObject.IdDCU, Plugin.MainServerProvider.DCUs.GetPhysicalAddress(_editingObject));
            RefreshDcuInputsSabotageStateChanged(Plugin.MainServerProvider.DCUs.GetInputsSabotageState(_editingObject.IdDCU));
            _bApply.Enabled = false;

            SafeThread.StartThread(ShowCardReaders);
        }

        private ICollection<AlarmArc> GetAlarmArcs(
            IDictionary<AlarmType, ICollection<AlarmArc>> alarmArcsByAlarmType,
            AlarmType alarmType)
        {
            ICollection<AlarmArc> alarmArcsForAlarmType;

            if (!alarmArcsByAlarmType.TryGetValue(
                alarmType,
                out alarmArcsForAlarmType))
            {
                return null;
            }

            return alarmArcsForAlarmType;
        }

        private AOnOffObject GetBlockAlarmObject(byte? objectType, Guid? objectGuid)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(true) && objectType != null && objectGuid != null)
            {
                switch (objectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(objectGuid.Value);
                    case (byte)ObjectType.TimeZone:
                        return CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(objectGuid.Value);
                    case (byte)ObjectType.Input:
                        return Plugin.MainServerProvider.Inputs.GetObjectById(objectGuid.Value);
                    case (byte)ObjectType.Output:
                        return Plugin.MainServerProvider.Outputs.GetObjectById(objectGuid.Value);
                }
            }

            return null;
        }

        private void ShowCardReaders()
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new DVoid2Void(ShowCardReaders));
                }
                else
                {
                    lock (_cardReaderReloadLock)
                    {
                        if (_cardReaderReload)
                        {
                            var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(_editingObject.IdDCU);

                            if (dcu != null)
                            {
                                if (_actCardReaders == null)
                                    _actCardReaders = new List<CardReader>();

                                _actCardReaders.Clear();

                                foreach (var cardReader in dcu.CardReaders)
                                {
                                    var crFull = Plugin.MainServerProvider.CardReaders.GetObjectById(cardReader.IdCardReader);
                                    if (crFull != null)
                                        _actCardReaders.Add(crFull);
                                }
                            }
                        }
                    }

                    _bCRMarkAsDead.Enabled = false;
                    lock (_ilbCardReaders)
                    {
                        _ilbCardReaders.Items.Clear();//
                        if (CgpClient.Singleton.IsConnectionLost(false))
                            return;
                        if (_actCardReaders == null) return;
                        foreach (var cardReader in _actCardReaders)
                        {
                            cardReader.EnableParentInFullName = false;
                            var onlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader);
                            _ilbCardReaders.Items.Add(
                                onlineState == OnlineState.Online
                                    ? new ImageListBoxItem(
                                        cardReader,
                                        ObjectType.CardReader.ToString())
                                    : new ImageListBoxItem(
                                        cardReader,
                                        ObjTypeHelper.CardReaderBlocked));
                        }
                    }
                    ShowCRsUpgrade();
                }
            }
            catch { }
        }

        private void UpdateShowCardReaders(Guid idCardReader, bool online)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, bool>(UpdateShowCardReaders), idCardReader, online);
            }
            else
            {
                if (_ilbCardReaders != null && _ilbCardReaders.Items.Count > 0)
                {
                    lock (_ilbCardReaders)
                    {
                        foreach (ImageListBoxItem item in _ilbCardReaders.Items)
                        {
                            if (item.MyObject is CardReader)
                            {
                                var cardReader = item.MyObject as CardReader;
                                if (cardReader.IdCardReader == idCardReader)
                                {
                                    _ilbCardReaders.Items.Remove(item);
                                    _ilbCardReaders.Items.Add(
                                        online
                                            ? new ImageListBoxItem(
                                                cardReader,
                                                ObjectType.CardReader.ToString())
                                            : new ImageListBoxItem(
                                                cardReader,
                                                ObjTypeHelper.CardReaderBlocked));
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RegisterServerEvents()
        {
            _eventDCUOnlineStateChanged = DCUOnlineStateChanged;
            DCUOnlineStateChangedHandler.Singleton.RegisterStateChanged(_eventDCUOnlineStateChanged);

            _eventDCUPhysicalAddressChanged = DCUPhysicalAddressChanged;
            DCUPhysicalAddressChangedHandler.Singleton.RegisterPhysicalAddressChanged(_eventDCUPhysicalAddressChanged);

            _eventDcuInputsSabotageStateChanged = DcuInputsSabotageStateChanged;
            DcuInputsSabotageStateChangedHandler.Singleton.RegisterStateChanged(_eventDcuInputsSabotageStateChanged);

            _eventCardReaderOnlineStateChanged = ChangeCardReaderOnlineState;
            StateChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCardReaderOnlineStateChanged);

            _eventMemoryWarningChanged = DcuMemoryWarnigChanged;
            DCUMemoryWarningChangedHandler.Singleton.RegisterMemoryWarningChanged(_eventMemoryWarningChanged);

            _cudObjectEvent = CudObjectEvent;
            CUDObjectHandler.Singleton.Register(
                _cudObjectEvent,
                ObjectType.PresentationGroup,
                ObjectType.DoorEnvironment);

            _eventFileTransferProgressChanged = ChangeTransferPercents;
            TFTPFileTransferProgressChangedHandler.Singleton.RegisterTFTPFileTransferProgressChanged(_eventFileTransferProgressChanged);

            _eventCRsUpgradeProgressChanged = ChangeCRsUpgradeProgress;
            CRUpgradeProgressChangedHandler.Singleton.RegisterUpgradeProgressChanged(_eventCRsUpgradeProgressChanged);
        }

        private readonly object _cardReaderReloadLock = new object();
        private bool _cardReaderReload;

        private void ChangeTransferPercents(IPAddress destinationIP, int percents, byte transferPurpose)
        {
            if (destinationIP == null)
                return;

            if (!destinationIP.ToString().Equals(_editingObject.CCU.IPAddress))
                return;

            if (InvokeRequired)
                Invoke(new Action<IPAddress, int, byte>(ChangeTransferPercents), destinationIP, percents, transferPurpose);
            else
            {
                var fileTransferPurpose = (CCUFileTransferPurpose)transferPurpose;
                switch (fileTransferPurpose)
                {
                    case CCUFileTransferPurpose.CCUUpgrade:
                    case CCUFileTransferPurpose.DCUUpgrade:
                    case CCUFileTransferPurpose.CEUpgrade:
                        break;
                    case CCUFileTransferPurpose.CRUpgrade:
                        if (percents < 0)
                            ShowCRUpgradeTransferFailedMEssage();
                        break;
                }
            }
        }

        private void ShowCRUpgradeTransferFailedMEssage()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(ShowCRUpgradeTransferFailedMEssage));
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _dgvCRUpgrading, _editingObject.CCU.Name + ": " + GetString("FailedToTransferUpgradeFile"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
            }
        }

        private void CudObjectEvent(ObjectType objectType, object id, bool isInsert)
        {
            try
            {
                if (!(id is Guid))
                    return;

                var idGuid = (Guid)id;

                switch (objectType)
                {
                    case ObjectType.DoorEnvironment:
                        RefreshDoorEnvironment(idGuid);
                        break;
                }
            }
            catch { }
        }

        private void ReadDcuStatus()
        {
            if (Plugin.MainServerProvider != null)
            {
                var firmware = Plugin.MainServerProvider.DCUs.GetFirmwareVersion(_editingObject.IdDCU);
                ShowFirmware(firmware);
            }
        }

        private void ShowFirmware(string firmware)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DString2Void(ShowFirmware), firmware);
            }
            else
            {
                _eFirmware.Text = firmware;
            }
        }

        private void LoadDoorEnviroment()
        {
            if (_editingObject.DoorEnvironments == null || _editingObject.DoorEnvironments.Count == 0)
            {
                return;
            }
            
            _doorEnvironment = _editingObject.DoorEnvironments.ToList()[0];

            RefreshDoorEnvironment();

            _eventDoorEnvironmentStateChanged = ChangeState;

            DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(
                _eventDoorEnvironmentStateChanged);

            ChangeState(_doorEnvironment.IdDoorEnvironment,
                (byte)
                    Plugin.MainServerProvider.DoorEnvironments.GetDoorEnvironmentState(
                        _doorEnvironment.IdDoorEnvironment));
        }

        private void RefreshDoorEnvironment(Guid idDoorEnvironment)
        {
            if (_doorEnvironment == null)
                return;

            if (_doorEnvironment.IdDoorEnvironment != idDoorEnvironment)
                return;

            _doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetObjectById(
                idDoorEnvironment);

            _editingObject.DoorEnvironments.Clear();
            _editingObject.DoorEnvironments.Add(_doorEnvironment);

            RefreshDoorEnvironment();
        }

        private void RefreshDoorEnvironment()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshDoorEnvironment));
                return;
            }

            _itbDoorEnvironment.Text = _doorEnvironment.Name;
            _itbDoorEnvironment.Image = Plugin.GetImageForAOrmObject(_doorEnvironment);
        }

        private void ChangeState(Guid doorEnvironmentGuid, byte state)
        {
            if (_doorEnvironment.IdDoorEnvironment == doorEnvironmentGuid)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, byte>(ChangeState), doorEnvironmentGuid, state);
                }
                else
                {
                    var doorEnvironmentState = (DoorEnvironmentState)state;
                    switch (doorEnvironmentState)
                    {
                        case DoorEnvironmentState.Locked:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateLocked");
                                break;
                            }
                        case DoorEnvironmentState.Locking:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateLocking");
                                break;
                            }
                        case DoorEnvironmentState.Opened:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateOpened");
                                break;
                            }
                        case DoorEnvironmentState.Unlocked:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateUnlocked");
                                break;
                            }
                        case DoorEnvironmentState.Unlocking:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateUnlocking");
                                break;
                            }
                        case DoorEnvironmentState.AjarPrewarning:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateDoorAjarPrewarning");
                                break;
                            }
                        case DoorEnvironmentState.Ajar:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateDoorAjar");
                                break;
                            }
                        case DoorEnvironmentState.Intrusion:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateIntrusion");
                                break;
                            }
                        case DoorEnvironmentState.Sabotage:
                            {
                                _eDoorEnvironmentState.Text = GetString("DoorEnvironmentStateSatotage");
                                break;
                            }
                        default:
                            {
                                _eDoorEnvironmentState.Text = string.Empty;
                                break;
                            }
                    }
                }
            }
        }

        private void DCUOnlineStateChanged(Guid dcuGuid, OnlineState onlineState, Guid parentGuid)
        {
            if (_editingObject.IdDCU == dcuGuid)
            {
                RefreshDcuOnlineStateChanged(onlineState);
            }
        }

        private void RefreshDcuOnlineStateChanged(OnlineState onlineState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<OnlineState>(RefreshDcuOnlineStateChanged), onlineState);
            }
            else
            {
                _dcuOnlineState = onlineState;
                _eOnlineState.Text = GetString(onlineState.ToString());

                if (onlineState == OnlineState.Online)
                    LocalizationHelper.TranslateControl(_bCRMarkAsDead);
                else
                    _bCRMarkAsDead.Text = GetString("General_bRemove");

                EnabledDisabledTpControl();
            }
        }

        private void EnabledDisabledTpControl()
        {
            switch (_dcuOnlineState)
            {
                case OnlineState.Online:
                    _bRefresh.Enabled = true;
                    EnabledDisabledReset();
                    _bDcuMemoryLoadRefresh.Enabled = true;
                    break;
                case OnlineState.Upgrading:
                case OnlineState.WaitingForUpgrade:
                case OnlineState.AutoUpgrading:
                    _bRefresh.Enabled = true;
                    _bReset.Enabled = true;
                    _bDcuMemoryLoadRefresh.Enabled = false;
                    break;
                default:
                    _bRefresh.Enabled = false;
                    _bReset.Enabled = false;
                    _bDcuMemoryLoadRefresh.Enabled = false;
                    break;
            }
        }

        private void DCUPhysicalAddressChanged(Guid dcuGuid, string physicalAddress)
        {
            if (_editingObject.IdDCU == dcuGuid)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, string>(DCUPhysicalAddressChanged), dcuGuid, physicalAddress);
                }
                else
                {
                    _ePhysicalAddress.Text = physicalAddress;
                }
            }
        }

        private void DcuInputsSabotageStateChanged(Guid dcuId, State dcuInputsSabotageState)
        {
            if (_editingObject.IdDCU == dcuId)
            {
                RefreshDcuInputsSabotageStateChanged(dcuInputsSabotageState);
            }
        }

        private void RefreshDcuInputsSabotageStateChanged(State dcuInputsSabotageState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<State>(RefreshDcuInputsSabotageStateChanged), dcuInputsSabotageState);
            }
            else
            {
                _eDcuInputsSabotageState.Text = GetString(string.Format("State_{0}", dcuInputsSabotageState));
            }
        }

        private void EnabledDisabledReset()
        {
            if (_dcuOnlineState != OnlineState.Online)
            {
                _bReset.Enabled = false;
                return;
            }

            try
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                _bReset.Enabled =
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.DcusResetPerform));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void ChangeCardReaderOnlineState(Guid crGuid, byte state, Guid parentGuid)
        {
            if (parentGuid == _editingObject.IdDCU)
            {
                SafeThread<Guid, byte>.StartThread(DoChangeCardReaderOnlineState, crGuid, state);
            }
        }

        private void DcuMemoryWarnigChanged(Guid dcuGuid, byte memory)
        {
            if (_editingObject != null && dcuGuid == _editingObject.IdDCU)
            {
                SafeThread<byte>.StartThread(ShowDcuMemoryLoad, memory);
            }
        }

        private void DoChangeCardReaderOnlineState(Guid crGuid, byte state)
        {
            RefreshCardReaderOnlineState(crGuid, (OnlineState)state);
        }

        private void RefreshCardReaderOnlineState(Guid crGuid, OnlineState onlineState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, OnlineState>(RefreshCardReaderOnlineState), crGuid, onlineState);
            }
            else
            {
                try
                {
                    var cr = Plugin.MainServerProvider.CardReaders.GetObjectById(crGuid);
                    if (cr != null && _editingObject != null && _editingObject.Compare(cr.DCU))
                    {
                        var wasFound = false;
                        if (_actCardReaders != null)
                        {
                            foreach (var cardReader in _actCardReaders)
                            {
                                if (cardReader != null)
                                {
                                    if (cardReader.IdCardReader == crGuid)
                                    {
                                        wasFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!wasFound)
                        {
                            ShowCardReaders();
                        }
                        else
                        {
                            UpdateShowCardReaders(crGuid, (onlineState == OnlineState.Online));
                        }

                        wasFound = false;
                        if (_dgvCRUpgrading != null && _dgvCRUpgrading.Rows != null && _dgvCRUpgrading.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                            {
                                var cardReader = _bindingSourceCRUpgrades.List[row.Index] as CardReader;
                                if (cardReader != null)
                                {
                                    if (cardReader.IdCardReader == crGuid)
                                    {
                                        wasFound = true;
                                        row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());
                                    }
                                }
                            }
                        }

                        if (!wasFound)
                        {
                            ShowCRsUpgrade();
                        }
                    }
                }
                catch { }
            }
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
                if (!Insert 
                    && _editingObject.Name != _eName.Text)
                {
                    var objectsToRename = Plugin.MainServerProvider.DCUs.GetObjectsToRename(_editingObject.IdDCU);

                    if (objectsToRename != null 
                        && objectsToRename.Count > 0)
                    {
                        var multiRenameDialog = new MultiRenameDialog(
                            Plugin.GetPluginObjectsImages(),
                            (List<IModifyObject>)objectsToRename,
                            _editingObject.Name,
                            _eName.Text,
                            NCASClient.LocalizationHelper);

                        if (multiRenameDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (var objectToRename in multiRenameDialog.RenameAll
                                ? objectsToRename
                                : multiRenameDialog.SelectedItems)
                            {
                                switch (objectToRename.GetOrmObjectType)
                                {
                                    case ObjectType.CardReader:

                                        var crToRename =
                                            _actCardReaders.FirstOrDefault(
                                                cr => cr.IdCardReader.Equals(objectToRename.GetId));

                                        if (crToRename != null)
                                        {
                                            crToRename.Name = crToRename.Name.Replace(_editingObject.Name, _eName.Text);
                                        }

                                        break;

                                    case ObjectType.Output:

                                        var outputToRename =
                                            _editingObject.Outputs.FirstOrDefault(
                                                output => output.IdOutput.Equals(objectToRename.GetId));

                                        if (outputToRename != null)
                                        {
                                            outputToRename.Name = outputToRename.Name.Replace(_editingObject.Name, _eName.Text);
                                        }

                                        break;

                                    case ObjectType.Input:

                                        var inputToRename =
                                            _editingObject.Inputs.FirstOrDefault(
                                                input => input.IdInput.Equals(objectToRename.GetId));

                                        if (inputToRename != null)
                                        {
                                            inputToRename.Name = inputToRename.Name.Replace(_editingObject.Name, _eName.Text);
                                        }

                                        break;
                                }
                            }
                        }
                    }
                }
                
                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;

                if (_editingObject.Outputs != null)
                {
                    foreach (var output in _editingObject.Outputs)
                    {
                        if (output.ControlType == (byte)OutputControl.controledByDoorEnvironment)
                        {
                            output.ControlType = 0;
                        }
                    }
                }

                _editingObject.AlarmOffline = _catsDcuOffline.AlarmEnabledCheckState;
                _editingObject.BlockAlarmOffline = _catsDcuOffline.BlockAlarmCheckState;

                if (_catsDcuOffline.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmOfflineObjectType =
                        (byte) _catsDcuOffline.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmOfflineId =
                        (Guid) _catsDcuOffline.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmOfflineObjectType = null;
                    _editingObject.ObjBlockAlarmOfflineId = null;
                }

                _editingObject.OfflinePresentationGroup = _catsDcuOffline.PresentationGroup;

                if (_editingObject.DcuAlarmArcs != null)
                    _editingObject.DcuAlarmArcs.Clear();
                else
                    _editingObject.DcuAlarmArcs = new List<DcuAlarmArc>();

                SaveAlarmArcs(
                    _cmaaDcuOffline.AlarmArcs,
                    AlarmType.DCU_Offline);

                _editingObject.AlarmTamper = _catsDcuTamperSabotage.AlarmEnabledCheckState;
                _editingObject.BlockAlarmTamper = _catsDcuTamperSabotage.BlockAlarmCheckState;
                
                _editingObject.EventlogDuringBlockAlarmTamper =
                    _catsDcuTamperSabotage.EventlogDuringBlockedAlarmCheckState;

                if (_catsDcuTamperSabotage.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmTamperObjectType =
                        (byte) _catsDcuTamperSabotage.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmTamperId =
                        (Guid) _catsDcuTamperSabotage.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmTamperObjectType = null;
                    _editingObject.ObjBlockAlarmTamperId = null;
                }

                _editingObject.TamperSabotagePresentationGroup = _catsDcuTamperSabotage.PresentationGroup;

                SaveAlarmArcs(
                    _cmaaDcuTamperSabotage.AlarmArcs,
                    AlarmType.DCU_TamperSabotage);

                _editingObject.CCU = _actCCU;

                if (Insert)
                {
                    _editingObject.LogicalAddress = (byte)_nudLogicalAddress.Value;
                }

                if (_actCardReaders != null && _actCardReaders.Count > 0)
                {
                    if (_editingObject.CardReaders == null)
                        _editingObject.CardReaders = new List<CardReader>();
                    else
                        _editingObject.CardReaders.Clear();

                    foreach (var cr in _actCardReaders)
                    {
                        _editingObject.CardReaders.Add(cr);
                    }
                }

                _editingObject.DcuSabotageOutput = _outputSabotageDcu;
                _editingObject.DcuOfflineOutput = _outputOfflineDcu;
                _editingObject.DcuInputsSabotageOutput = _outputSabotageDcuInputs;

                if (Insert)
                {
                    _editingObject.InputsCount = (int)_eInputCount.Value;
                    _editingObject.OutputsCount = (int)_eOutputCount.Value;
                }

                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private void SaveAlarmArcs(
            IEnumerable<AlarmArc> alarmArcs,
            AlarmType alarmType)
        {
            if (alarmArcs != null)
            {
                foreach (var alarmArc in alarmArcs)
                {
                    _editingObject.DcuAlarmArcs.Add(
                        new DcuAlarmArc(
                            _editingObject,
                            alarmArc,
                            alarmType));
                }
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertDCUName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            if (!_catsDcuOffline.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DCU_Offline]()))
            {
                return false;
            }

            if (!_catsDcuTamperSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DCU_TamperSabotage]()))
            {
                return false;
            }

            return true;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        protected override bool SaveToDatabaseInsert()
        {
            if (!Plugin.MainServerProvider.DCUs.CheckLogicalAddress(_editingObject))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _nudLogicalAddress,
                GetString("ErrorUsedLogicalAddress"), ControlNotificationSettings.Default);
                _nudLogicalAddress.Focus();
                return false;
            }

            if (_doorEnvironment == null)
            {
                _editingObject.DoorEnvironments = new LinkedList<DoorEnvironment>(
                    Enumerable.Repeat(
                        Plugin.MainServerProvider.DoorEnvironments.CreateNewForDCU(),
                        1));
            }

            Exception error;
            var retValue = Plugin.MainServerProvider.DCUs.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains(DCU.COLUMNNAME))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedDCUName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else if (error is SqlUniqueException && error.Message.Contains(DCU.COLUMNLOGICALADDRESS))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _nudLogicalAddress,
                    GetString("ErrorUsedLogicalAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _nudLogicalAddress.Focus();
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

            bool retValue = 
                onlyInDatabase
                    ? Plugin.MainServerProvider.DCUs.UpdateOnlyInDatabase(_editingObject, out error) 
                    : Plugin.MainServerProvider.DCUs.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains(DCU.COLUMNNAME))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedDCUName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }
            else
                UnsetPreviouslyUsedActuatorsAndPushButtons();

            return retValue;
        }

        private void RewriteCardReaders(out Exception error)
        {
            error = null;

            if (_editingObject.CardReaders != null)
            {
                foreach (var cardReader in _editingObject.CardReaders)
                {
                    var editedCardReader = Plugin.MainServerProvider.CardReaders.GetObjectForEdit(cardReader, out error);

                    if (editedCardReader != null)
                    {
                        editedCardReader.DCU = _editingObject;
                        Plugin.MainServerProvider.CardReaders.UpdateOnlyInDatabase(editedCardReader, out error);
                        Plugin.MainServerProvider.CardReaders.EditEnd(editedCardReader);
                    }
                }
            }
        }

        /// <summary>
        /// Unsets outputs previously used as actuators or push buttons
        /// </summary>
        private void UnsetPreviouslyUsedActuatorsAndPushButtons()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            try
            {
                foreach (var output in _editingObject.Outputs)
                {
                    if ((OutputControl)output.ControlType == OutputControl.controledByDoorEnvironment &&
                        !Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(output))
                    {
                        Exception ex;
                        var outputForEdit = Plugin.MainServerProvider.Outputs.GetObjectForEdit(output.IdOutput, out ex);
                        if (ex == null && outputForEdit != null)
                        {
                            outputForEdit.ControlType = (byte)OutputControl.unblocked;
                            Plugin.MainServerProvider.Outputs.Update(outputForEdit, out ex);
                            Plugin.MainServerProvider.Outputs.EditEnd(outputForEdit);
                        }
                    }
                }
            }
            catch { }
        }

        private bool _wasChangedName;
        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (sender == _nudLogicalAddress && !_wasChangedName)
            {
                if (_editingObject.EnableParentInFullName)
                    _eName.Text = "DCU " + ((int)_nudLogicalAddress.Value);
                else
                    _eName.Text = _editingObject.CCU.Name + StringConstants.SLASHWITHSPACES + "DCU " + ((int)_nudLogicalAddress.Value);
            }

            if (!Insert && ValueChanged)
                _bApply.Enabled = true;
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert && ValueChangedOnlyDatabase)
                _bApply.Enabled = true;
        }

        protected override void EditEnd()
        {
            if (_editingObject == null) return;
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.DCUs != null)
                Plugin.MainServerProvider.DCUs.EditEnd(_editingObject);
        }

        private void _itbSuperiorCCU_DoubleClick(object sender, EventArgs e)
        {
            if (_actCCU != null)
            {
                NCASCCUsForm.Singleton.OpenEditForm(_actCCU);
            }
        }

        private void _ilbCardReaders_DoubleClick(object sender, EventArgs e)
        {
            var obj = _ilbCardReaders.SelectedItemObject;
            if (obj == null) return;

            if (obj.GetType() == typeof(CardReader))
            {
                var cardReader = obj as CardReader;
                if (cardReader != null)
                {
                    NCASCardReadersForm.Singleton.OpenEditForm(cardReader);
                }
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.DCUs.
                GetReferencedObjects(_editingObject.IdDCU, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh1_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                _eFirmware.Text = string.Empty;
                if (_editingObject != null)
                {
                    if (Plugin.MainServerProvider != null)
                    {
                        _eFirmware.Text = Plugin.MainServerProvider.DCUs.GetFirmwareVersion(_editingObject.IdDCU);
                    }
                }
            }
            catch
            {
            }
        }

        private void _bPrecreateCR_Click(object sender, EventArgs e)
        {
            var cardReader = 
                new CardReader
                {
                    DCU = _editingObject,
                    EnableParentInFullName = Plugin.MainServerProvider.GetEnableParentInFullName()
                };
            NCASCardReadersForm.Singleton.OpenInsertFromEdit(ref cardReader, ShowCardReaders);
        }

        private void ShowCardReaders(object obj)
        {
            var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(_editingObject.IdDCU);
            if (dcu != null)
            {
                _actCardReaders = dcu.CardReaders;
                ShowCardReaders();
            }
        }

        private string _oldName;
        private void _eName_KeyUp(object sender, KeyEventArgs e)
        {
            if (_oldName != _eName.Text)
                _wasChangedName = true;
        }

        private void _eName_KeyDown(object sender, KeyEventArgs e)
        {
            _oldName = _eName.Text;
        }

        private void _bCRMarkAsDead_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            //CardReader cardReader = _lbCardReaders.SelectedItem as CardReader;
            var cardReader = _ilbCardReaders.SelectedItemObject as CardReader;

            if (cardReader != null)
            {
                var crOnlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader);
                if (crOnlineState != OnlineState.Online)
                {
                    if (!NCASCardReadersForm.Singleton.DeleteAllowedNotEdited(cardReader))
                    {
                        return;
                    }

                    if (Dialog.Question(GetString("QuestionDeleteMarkAsDeadCR")))
                    {
                        Exception error;
                        if (Plugin.MainServerProvider.CardReaders.Delete(cardReader, out error))
                        {
                            var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(_editingObject.IdDCU);
                            if (dcu != null)
                            {
                                _actCardReaders = dcu.CardReaders;
                                ShowCardReaders();
                            }
                        }
                        else
                        {
                            if (error is SqlDeleteReferenceConstraintException)
                            {
                                Dialog.Info(GetString("InfoCannotRemoveUsedObject"));
                            }
                            else
                            {
                                Dialog.Info(GetString("InfoRemoveObjectFailed"));
                            }
                        }
                    }
                }
                else
                {
                    _bCRMarkAsDead.Enabled = false;
                }
            }
        }

        private void _ilbCardReaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            var cardReader = _ilbCardReaders.SelectedItemObject as CardReader;

            if (cardReader != null)
            {
                var crOnlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader);
                if (crOnlineState != OnlineState.Online)
                {
                    _bCRMarkAsDead.Enabled = true;
                    return;
                }
            }

            _bCRMarkAsDead.Enabled = false;
        }

        enum UpgradeProcessSteps
        {
            Idle = 0,
            Transfering = 1,
            Upgrading = 2
        }

        private void ShowFailedToResetMessage()
        {
            if (_bReset.InvokeRequired)
                _bReset.BeginInvoke(new DVoid2Void(ShowFailedToResetMessage));
            else
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _bReset, _editingObject.Name + ": " + GetString("ResetDCUFailed"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
        }

        private bool _tpControlFistTime = true;
        private void _tpControl_Enter(object sender, EventArgs e)
        {
            if (_tpControlFistTime)
            {
                _tpControlFistTime = false;
                ReadDcuStatus();
            }
        }

        private void _bReset_Click(object sender, EventArgs e)
        {
            if (Plugin.MainServerProvider == null)
                return;
            if (Dialog.Question(GetString("QuestionResetDCU")))
            {
                if (
                    !Plugin.MainServerProvider.DCUs.Reset(_editingObject.CCU.IdCCU,
                        _editingObject.LogicalAddress))
                {
                    ShowFailedToResetMessage();
                }
            }
        }

        private Output _outputDoorAjar;
        private Output _outputIntrusion;
        private Output _outputSabotage;
        private Output _outputSabotageDcu;
        private Output _outputOfflineDcu;
        private Output _outputSabotageDcuInputs;

        private void ModifySpecialOutput(byte type)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            var formModify = new ListboxFormAdd(GetAvailableOutputs(type == 2), GetString("ModifyOutput"));
            object outObject;
            formModify.ShowDialog(out outObject);
            if (outObject != null)
            {
                var output = Plugin.MainServerProvider.Outputs.GetObjectById(((OutputModifyObj)outObject).Id);
                if (output != null)
                {
                    SetSpecialOutput(type, output);
                    Plugin.AddToRecentList(output);
                }
            }
        }

        private void SetSpecialOutput(byte type, Output value)
        {
            switch (type)
            {
                case 1:
                    _outputSabotageDcu = value;
                    _tbmOutputSabotageDcu.Text = (value == null ? string.Empty : value.ToString());
                    _tbmOutputSabotageDcu.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
                case 2:
                    _outputOfflineDcu = value;
                    _tbmOutputOfflineDcu.Text = (value == null ? string.Empty : value.ToString());
                    _tbmOutputOfflineDcu.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
                case 3:
                    _outputSabotageDcuInputs = value;
                    _tbmOutputSabotageDCUInputs.Text = (value == null ? string.Empty : value.ToString());
                    _tbmOutputSabotageDCUInputs.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
            }
        }

        private void RemoveSpecialOutput(byte type)
        {
            SetSpecialOutput(type, null);
        }

        private IList<IModifyObject> GetAvailableOutputs(bool outputForDCUOffline)
        {
            var outputs = Plugin.MainServerProvider.DoorEnvironments.
                GetAllCCUOutputsNotInDoorEnvironmentsActuators(_doorEnvironment.IdDoorEnvironment);

            IList<IModifyObject> outputsModifyObjects = new List<IModifyObject>();

            if (outputs != null)
            {
                foreach (var output in outputs)
                {
                    if (!outputForDCUOffline || output.DCU == null || output.DCU.IdDCU != _editingObject.IdDCU)
                        outputsModifyObjects.Add(new OutputModifyObj(output));
                }
            }

            return outputsModifyObjects;
        }

        private static bool CheckOutputAvailability(Output output, IEnumerable<IModifyObject> outputsModifyObjects)
        {
            foreach (var item in outputsModifyObjects)
            {
                if ((Guid)item.GetId == output.IdOutput)
                {
                    return true;
                }
            }
            return false;
        }

        private void _bDcuMemoryLoadRefresh_Click(object sender, EventArgs e)
        {
            _eDcuMemoryLoad.Text = string.Empty;
            if (_editingObject.CCU != null && _editingObject.LogicalAddress > 0)
            {
                SafeThread.StartThread(ObtainDcuMemoryLoad);
            }
        }

        private void ObtainDcuMemoryLoad()
        {
            Plugin.MainServerProvider.CCUs.RequestDcuMemoryLoad(_editingObject.CCU.IdCCU, _editingObject.LogicalAddress);
        }

        private void ShowDcuMemoryLoad(byte dcuMemoryLoad)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<byte>(ShowDcuMemoryLoad), dcuMemoryLoad);
            }
            else
            {
                _eDcuMemoryLoad.Text = dcuMemoryLoad + " %";
            }
        }

        private bool _tpInpupOutputFistTime = true;
        private void _tpInpupOutputSettings_Enter(object sender, EventArgs e)
        {
            if (_tpInpupOutputFistTime)
            {
                _tpInpupOutputFistTime = false;
                SafeThread.StartThread(ObtainInputShortFromServer);
                SafeThread.StartThread(ObtainOutputsShortFromServer);
                _eventInputStateChanged = ChangeInputState;
                StateChangedInputHandler.Singleton.RegisterStateChanged(_eventInputStateChanged);
                _eventOutputStateChanged = ChangeOutputState;
                StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputStateChanged);
                _eventOutputRealStateChanged = ChangeOutputRealState;
                RealStateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputRealStateChanged);
                _outputEditChanged = OutputEditChanged;
                OutputEditChangedHandler.Singleton.RegisterOutputEditChanged(_outputEditChanged);
                _inputEditChanged = InputEditChanged;
                InputEditChangedHandler.Singleton.RegisterInputEditChanged(_inputEditChanged);
            }
        }

        #region InputsShorts
        private void ObtainInputShortFromServer()
        {
            var lastPosition = 0;
            if (_bindingSourceInput != null && _bindingSourceInput.Count != 0)
                lastPosition = _bindingSourceInput.Position;
            _bindingSourceInput = new BindingSource();
            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            var filterSetting = new FilterSettings(Input.COLUMNDCUSDCU, _editingObject, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);
            Exception error;
            var inputShortCollection = Plugin.MainServerProvider.Inputs.ShortSelectByCriteria(filterSettings, out error);
            _bindingSourceInput.DataSource = inputShortCollection;
            _bindingSourceInput.AllowNew = false;
            if (lastPosition != 0) _bindingSourceInput.Position = lastPosition;
            ShowGridInputShorts();
        }

        private void ShowGridInputShorts()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowGridInputShorts));
            }
            else
            {
                _cdgvInput.ModifyGridView(
                    _bindingSourceInput, 
                    InputShort.COLUMNFULLNAME, 
                    InputShort.COLUMNSTATUSONOFF, 
                    InputShort.COLUMNDESCRIPTION);

                foreach (DataGridViewRow row in _cdgvInput.DataGrid.Rows)
                {
                    var input = (InputShort)_bindingSourceInput.List[row.Index];
                    switch (input.State)
                    {
                        case InputState.Alarm:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputAlarm");
                            break;
                        case InputState.Short:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputShort");
                            break;
                        case InputState.Break:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputBreak");
                            break;
                        case InputState.Normal:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputNormal");
                            break;
                        case InputState.UsedByAnotherAplication:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                            break;
                        case InputState.OutOfRange:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                            break;
                        default:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                            break;
                    }
                }

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvInput.DataGrid);
                _cdgvInput.DataGrid.AutoResizeColumns();
            }
        }

        private void DgvInputDoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceInput == null || _bindingSourceInput.Count == 0) return;
            var inputShort = (InputShort)_bindingSourceInput.List[_bindingSourceInput.Position];
            bool editAllowed;
            var input = Plugin.MainServerProvider.Inputs.GetObjectForEditById(inputShort.IdInput, out editAllowed);
            NCASInputsForm.Singleton.OpenEditForm(input, editAllowed);
        }

        private void ChangeInputState(Guid inputGuid, byte state, Guid parent)
        {
            if (_editingObject.IdDCU == parent)
                SafeThread<Guid, byte>.StartThread(DoChangeInputState, inputGuid, state);
        }

        readonly Mutex _doChangeInputStateMutex = new Mutex();
        private void DoChangeInputState(Guid inputGuid, byte state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeInputState), inputGuid, state);
            }
            else
            {
                try
                {
                    _doChangeInputStateMutex.WaitOne();
                    RefreshInputState(inputGuid, (InputState)state);
                    _doChangeInputStateMutex.ReleaseMutex();
                }
                catch { }
            }
        }

        private void RefreshInputState(Guid inputGuid, InputState inputState)
        {
            foreach (DataGridViewRow row in _cdgvInput.DataGrid.Rows)
            {
                var input = (InputShort)_bindingSourceInput.List[row.Index];

                if (input != null && input.IdInput == inputGuid)
                {
                    switch (inputState)
                    {
                        case InputState.Alarm:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputAlarm");
                            break;
                        case InputState.Short:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputShort");
                            break;
                        case InputState.Break:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputBreak");
                            break;
                        case InputState.Normal:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("InputNormal");
                            break;
                        case InputState.UsedByAnotherAplication:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                            break;
                        case InputState.OutOfRange:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                            break;
                        default:
                            row.Cells[InputShort.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                            break;
                    }
                }
            }
        }

        private void InputEditChanged(Guid parentId, Guid inputId)
        {
            if (_editingObject.IdDCU == parentId)
            {
                SafeThread.StartThread(ObtainInputShortFromServer);
            }
        }
        #endregion

        #region OutputsShort

        private void DgvOutputDoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceOutput == null || _bindingSourceOutput.Count == 0) return;
            var outputShort = (OutputShort)_bindingSourceOutput.List[_bindingSourceOutput.Position];
            bool editAllowed;
            var output = Plugin.MainServerProvider.Outputs.GetObjectForEditById(outputShort.IdOutput, out editAllowed);
            NCASOutputsForm.Singleton.OpenEditForm(output, editAllowed);
        }

        private void ObtainOutputsShortFromServer()
        {
            if (_editingObject.Outputs == null) return;
            if (_editingObject.Outputs.Count == 0) return;

            var lastPosition = 0;
            if (_bindingSourceOutput != null && _bindingSourceOutput.Count != 0)
                lastPosition = _bindingSourceOutput.Position;

            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            var filterSetting = new FilterSettings(Output.COLUMNDCU, _editingObject, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);
            Exception error;
            var outputShortCollection = Plugin.MainServerProvider.Outputs.ShortSelectByCriteria(filterSettings, out error);

            _bindingSourceOutput = 
                new BindingSource
                {
                    DataSource = outputShortCollection,
                    AllowNew = false
                };
            if (lastPosition != 0) _bindingSourceOutput.Position = lastPosition;
            ShowGridOutputShort();
        }

        private void ShowGridOutputShort()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowGridOutputShort));
            }
            else
            {
                _cdgvOutput.ModifyGridView(_bindingSourceOutput, OutputShort.COLUMNNAME, OutputShort.COLUMNSTATUSONOFF, OutputShort.COLUMNREALSTATUSONOFF, OutputShort.COLUMNDESCRIPTION);

                foreach (DataGridViewRow row in _cdgvOutput.DataGrid.Rows)
                {
                    var output = (OutputShort)_bindingSourceOutput.List[row.Index];

                    if (output != null)
                    {
                        switch (output.State)
                        {
                            case OutputState.On:
                                row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("On");
                                break;
                            case OutputState.Off:
                                row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("Off");
                                break;
                            case OutputState.UsedByAnotherAplication:
                                row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                                break;
                            case OutputState.OutOfRange:
                                row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                                break;
                            default:
                                row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                                break;
                        }

                        if (!output.RealStateChanges)
                        {
                            row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                        }
                        else
                        {
                            switch (output.RealState)
                            {
                                case OutputState.On:
                                    row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("On");
                                    break;
                                case OutputState.Off:
                                    row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("Off");
                                    break;
                                default:
                                    row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                                    break;
                            }
                        }
                    }
                }
                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvOutput.DataGrid);
                _cdgvOutput.DataGrid.AutoResizeColumns();
            }
        }

        private void ChangeOutputState(Guid outputGuid, byte state, Guid parent)
        {
            if (_editingObject.IdDCU == parent)
                SafeThread<Guid, byte>.StartThread(DoChangeOutputState, outputGuid, state);
        }

        readonly Mutex _doChangeOutputStateMutex = new Mutex();
        private void DoChangeOutputState(Guid outputGuid, byte state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeOutputState), outputGuid, state);
            }
            else
            {
                try
                {
                    _doChangeOutputStateMutex.WaitOne();
                    RefreshOutputState(outputGuid, (OutputState)state);
                    _doChangeOutputStateMutex.ReleaseMutex();
                }
                catch { }
            }
        }
        private void RefreshOutputState(Guid outputGuid, OutputState outputState)
        {
            foreach (DataGridViewRow row in _cdgvOutput.DataGrid.Rows)
            {
                var output = (OutputShort)_bindingSourceOutput.List[row.Index];

                if (output != null && output.IdOutput == outputGuid)
                {
                    if (outputState == OutputState.Off)
                    {
                        row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("Off");
                    }
                    else if (outputState == OutputState.On)
                    {
                        row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("On");
                    }
                    else if (outputState == OutputState.UsedByAnotherAplication)
                    {
                        row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                    }
                    else if (outputState == OutputState.OutOfRange)
                    {
                        row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                    }
                    else
                        row.Cells[OutputShort.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                }
            }
        }

        private void ChangeOutputRealState(Guid outputGuid, byte state, Guid parent)
        {
            if (_editingObject.IdDCU == parent)
                SafeThread<Guid, byte>.StartThread(DoChangeOutputRealState, outputGuid, state);
        }

        private void OutputEditChanged(Guid parentId, Guid outputId)
        {
            if (_editingObject.IdDCU == parentId)
            {
                SafeThread.StartThread(ObtainOutputsShortFromServer);
            }
        }

        private void DoChangeOutputRealState(Guid outputGuid, byte state)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeOutputRealState), outputGuid, state);
            }
            else
            {
                try
                {
                    _doChangeOutputStateMutex.WaitOne();
                    RefreshOutputRealState(outputGuid, (OutputState)state);
                    _doChangeOutputStateMutex.ReleaseMutex();
                }
                catch { }
            }
        }

        private void RefreshOutputRealState(Guid outputGuid, OutputState outputState)
        {
            foreach (DataGridViewRow row in _cdgvOutput.DataGrid.Rows)
            {
                var output = (OutputShort)_bindingSourceOutput.List[row.Index];

                if (output != null && output.IdOutput == outputGuid)
                {
                    if (!output.RealStateChanges)
                    {
                        row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                    }
                    else
                    {
                        if (outputState == OutputState.Off)
                        {
                            row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("Off");
                        }
                        else if (outputState == OutputState.On)
                        {
                            row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("On");
                        }
                        else
                            row.Cells[OutputShort.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                    }
                }
            }
        }
        #endregion

        private void _tpSettings_Enter(object sender, EventArgs e)
        {
            ShowCardReaders();
        }

        private void _tpCRUpgrades_Enter(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;
            string delimeter;
            ICollection<string> crVersions = Plugin.MainServerProvider.GetAvailableCRUpgrades(out delimeter);
            var crVersionsLocalised = new List<CRUpgradeFileInfo>();

            if (crVersions != null && crVersions.Count > 0)
            {
                foreach (var version in crVersions)
                {
                    if (!version.Contains(delimeter))
                        continue;
                    try
                    {
                        crVersionsLocalised.Add(new CRUpgradeFileInfo(version, delimeter[0]));
                    }
                    catch { }
                }

                var source = 
                    new BindingSource
                    {
                        DataSource = crVersionsLocalised
                    };
                _lbAvailableCRUpgradeVersions.DataSource = source;
            }
        }

        private void _bUpgradeCRRefresh_Click(object sender, EventArgs e)
        {
            ShowGridCRUpgrade();
        }

        private void ShowGridCRUpgrade()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(ShowGridCRUpgrade));
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                if (_bindingSourceCRUpgrades == null) return;
                if (_dgvCRUpgrading.DataSource == null) return;

                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    row.Cells[CardReader.COLUMNONLINESTATE].Value = "";
                    ((CardReader)_bindingSourceCRUpgrades[row.Index]).EnableParentInFullName = false;
                    row.Cells[CardReader.COLUMNNAME].Value = _bindingSourceCRUpgrades[row.Index].ToString();
                }

                SafeThread.StartThread(SetCRsUpgradeDgValues);
            }
        }

        private void SetCRsUpgradeDgValues()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetCRsUpgradeDgValues));
            }
            else
            {
                if (_dgvCRUpgrading.DataSource == null) return;

                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    var cr = (CardReader)_bindingSourceCRUpgrades.List[row.Index];

                    OnlineState onlineState = cr != null ? Plugin.MainServerProvider.CardReaders.GetOnlineStates(cr.IdCardReader) : OnlineState.Unknown;

                    row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());
                }
            }
        }

        private void _chbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
            {
                if (row.Cells[CardReader.COLUMNONLINESTATE].Value != null)
                {
                    row.Cells[CardReader.COLUMNSELECTUPGRADE].Value = 
                        row.Cells[CardReader.COLUMNONLINESTATE].Value.ToString() == GetString(OnlineState.Online.ToString()) &&
                        _chbSelectAll.Checked;
                }
            }
        }

        private void _dgvCRUpgrading_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == _dgvCRUpgrading.Columns[CardReader.COLUMNSELECTUPGRADE].Index)
                {
                    if (_dgvCRUpgrading.Rows[e.RowIndex].Cells[CardReader.COLUMNONLINESTATE].Value.ToString() == GetString(OnlineState.Online.ToString()))
                    {
                        _dgvCRUpgrading[e.ColumnIndex, e.RowIndex].ReadOnly = false;
                    }
                    else
                    {
                        _dgvCRUpgrading[e.ColumnIndex, e.RowIndex].ReadOnly = true;
                        _dgvCRUpgrading[e.ColumnIndex, e.RowIndex].Value = false;
                        _dgvCRUpgrading.EndEdit();
                    }
                }
            }
            catch { }
        }

        private void _dgvCRUpgrading_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _dgvCRUpgrading.Columns[CardReader.COLUMNSELECTUPGRADE].Index)
                return;
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (_bindingSourceCRUpgrades == null || _bindingSourceCRUpgrades.Count == 0) return;

            var cr = _bindingSourceCRUpgrades.List[_bindingSourceCRUpgrades.Position] as CardReader;
            NCASCardReadersForm.Singleton.OpenEditForm(cr);
        }

        private void ShowCRsUpgrade()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowCRsUpgrade));
            }
            else
            {
                if (_actCardReaders == null || _actCardReaders.Count == 0)
                {
                    _bindingSourceCRUpgrades = new BindingSource();
                    _dgvCRUpgrading.DataSource = _bindingSourceCRUpgrades;
                    return;
                }

                var lastPosition = 0;
                if (_bindingSourceCRUpgrades != null && _bindingSourceCRUpgrades.Count != 0)
                    lastPosition = _bindingSourceCRUpgrades.Position;
                _bindingSourceCRUpgrades = 
                    new BindingSource
                    {
                        DataSource = _actCardReaders.OrderBy(cr => cr.Address)
                            .ToList(),
                        AllowNew = false
                    };

                if (lastPosition != 0) _bindingSourceCRUpgrades.Position = lastPosition;

                _dgvCRUpgrading.DataSource = _bindingSourceCRUpgrades;

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNONLINESTATE))
                {
                    _dgvCRUpgrading.Columns.Add(CardReader.COLUMNONLINESTATE, CardReader.COLUMNONLINESTATE);
                }

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNUPGRADEPROGRESS))
                {
                    var column = 
                        new DataGridViewProgressColumn
                        {
                            Name = CardReader.COLUMNUPGRADEPROGRESS,
                            HeaderText = CardReader.COLUMNUPGRADEPROGRESS,
                            Selected = false
                        };
                    _dgvCRUpgrading.Columns.Add(column);
                }

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNSELECTUPGRADE))
                {
                    var column = 
                        new DataGridViewCheckBoxColumn
                        {
                            Name = CardReader.COLUMNSELECTUPGRADE,
                            HeaderText = CardReader.COLUMNSELECTUPGRADE
                        };
                    _dgvCRUpgrading.Columns.Add(column);
                }

                foreach (DataGridViewColumn column in _dgvCRUpgrading.Columns)
                {
                    if (column.Name == CardReader.COLUMNONLINESTATE ||
                        column.Name == CardReader.COLUMNUPGRADEPROGRESS)
                    {
                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (column.Name == CardReader.COLUMNNAME)
                    {
                        column.DisplayIndex = 0;
                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else if (column.Name == CardReader.COLUMNSELECTUPGRADE)
                    {
                        column.Visible = true;
                        column.ReadOnly = false;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else
                        column.Visible = false;
                }

                _dgvCRUpgrading.AutoGenerateColumns = false;

                _dgvCRUpgrading.AllowUserToAddRows = false;

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvCRUpgrading);

                ShowGridCRUpgrade();
            }
        }

        private void _bCRUpgrade_Click(object sender, EventArgs e)
        {
            var isSelected = false;

            if (_lbAvailableCRUpgradeVersions.SelectedItem == null)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _bCRUpgrade, GetString("WarningCRUpgradeVersionNotSet"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                return;
            }

            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            var cardReadersToUpgrade = new List<byte>();
            foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
            {
                if (row.Cells[CardReader.COLUMNSELECTUPGRADE].Value is bool &&
                    Convert.ToBoolean(row.Cells[CardReader.COLUMNSELECTUPGRADE].Value))
                {
                    isSelected = true;

                    var crGuid = Guid.Empty;
                    if (row.Cells[CardReader.COLUMNIDCARDREADER].Value != null)
                    {
                        crGuid = ((Guid)row.Cells[CardReader.COLUMNIDCARDREADER].Value);
                    }

                    var crOnlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(crGuid);
                    if (crOnlineState == OnlineState.Upgrading)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading, row.Cells[CardReader.COLUMNNAME].Value
                                    + ": " + GetString("DeviceAlreadyUpgrading"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        continue;
                    }

                    cardReadersToUpgrade.Add((byte)row.Cells[CardReader.COLUMNADDRESS].Value);
                }
            }
            if (cardReadersToUpgrade.Count > 0)
            {
                var dcusAndCardReaders = new Dictionary<byte, List<byte>>();
                dcusAndCardReaders[_editingObject.LogicalAddress] = cardReadersToUpgrade;

                var selectedUpgradeInfo = _lbAvailableCRUpgradeVersions.SelectedItem as CRUpgradeFileInfo;

                Exception ex;
                if (!Plugin.MainServerProvider.UpgradeCRs(selectedUpgradeInfo.Version, (byte)selectedUpgradeInfo.CrHWVersion, Guid.Empty, _editingObject.CCU.IPAddress,
                    null, dcusAndCardReaders, out ex))
                {
                    DoChangeCRsUpgradeProgress(new CRUpgradeState(_editingObject.CCU.IdCCU, _editingObject.IdDCU, cardReadersToUpgrade.ToArray(), null, null, null), ex);
                    ShowGridCRUpgrade();
                }
            }
            else if (!isSelected)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.JustOne, 
                    _bCRUpgrade, 
                    GetString("WarningNoCRSelected"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
            }
        }

        private void ChangeCRsUpgradeProgress(CRUpgradeState upgradeState)
        {
            if (upgradeState != null && upgradeState.DCUGuid.Equals(_editingObject.IdDCU))
                DoChangeCRsUpgradeProgress(upgradeState, null);
        }

        private void DoChangeCRsUpgradeProgress(CRUpgradeState upgradeState, Exception ex)
        {
            if (upgradeState == null)
                return;

            if (InvokeRequired)
                Invoke(new Action<CRUpgradeState, Exception>(DoChangeCRsUpgradeProgress), upgradeState, ex);
            else
            {
                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    if (row.Cells[CardReader.COLUMNADDRESS].Value != null && upgradeState.CardReaderAddresses.Contains((byte)row.Cells[CardReader.COLUMNADDRESS].Value))
                    {
                        var onlineState = OnlineState.Online;

                        if (ex != null)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("DeviceUpgradeFailed") + ":" + ex.Message,
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        }

                        if (upgradeState.UpgradeResult != null)
                        {
                            var upgradeResult = (CRUpgradeResult)upgradeState.UpgradeResult;
                            if (upgradeResult == CRUpgradeResult.Success)
                                ControlNotification.Singleton.Info(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("DeviceUpgradeSucceeded"),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                            else
                                ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                        row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("CRUpgradeResult" + upgradeResult),
                                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        }

                        if (upgradeState.UnpackErrorCode != null)
                        {
                            var unpackFailedCode = (UnpackPackageFailedCode)upgradeState.UnpackErrorCode;
                            ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("UnpackUpgradePackageFailed" + unpackFailedCode),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        }

                        if (upgradeState.Percents != null)
                        {
                            if (upgradeState.Percents < 0)
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("DeviceUpgradeFailed"),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                                upgradeState.Percents = 0;
                                continue;
                            }
                            if (upgradeState.Percents == 100)
                            {
                                upgradeState.Percents = 0;
                            }
                            else
                                onlineState = OnlineState.Upgrading;
                        }

                        row.Cells[CardReader.COLUMNUPGRADEPROGRESS].Value = 
                            upgradeState.Percents ?? 0;

                        row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());
                    }
                }
            }
        }

        bool _firsTimeDgCRUpgradeShow = true;
        private void _dgvCRUpgrading_VisibleChanged(object sender, EventArgs e)
        {
            if (_firsTimeDgCRUpgradeShow)
            {
                ShowGridCRUpgrade();
                _firsTimeDgCRUpgradeShow = false;
            }
        }

        private void _tbmOutputSabotageDcu_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputSabotageDCU(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddOutputSabotageDCU(object newOutput)
        {
            try
            {
                var output = newOutput as Output;
                if (output != null)
                {

                    if (CheckOutputAvailability(output, GetAvailableOutputs(false)))
                    {
                        _tbmOutputSabotageDcu.Text = newOutput.ToString();
                        _tbmOutputSabotageDcu.TextImage = Plugin.GetImageForAOrmObject(output);

                        _outputSabotageDcu = output;
                        Plugin.AddToRecentList(output);
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmOutputSabotageDcu.ImageTextBox,
                             GetString("ErrorOutputAlreadyUsed"), ControlNotificationSettings.Default);

                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmOutputSabotageDcu.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmOutputSabotageDcu_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmOutputSabotageDcu_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifySpecialOutput(1);
            }
            else if (item.Name == "_tsiRemove1")
            {
                RemoveSpecialOutput(1);
            }
        }

        private void _tbmOutputSabotageDcu_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputSabotageDcu != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputSabotageDcu);
            }
        }

        private void _tbmOutputOfflineDcu_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmOutputOfflineDcu_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputOfflineDCU(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddOutputOfflineDCU(object newOutput)
        {
            try
            {
                var output = newOutput as Output;
                if (output != null)
                {

                    if (CheckOutputAvailability(output, GetAvailableOutputs(true)))
                    {
                        _tbmOutputOfflineDcu.Text = newOutput.ToString();
                        _tbmOutputOfflineDcu.TextImage = Plugin.GetImageForAOrmObject(output);

                        _outputOfflineDcu = output;
                        Plugin.AddToRecentList(output);
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmOutputOfflineDcu.ImageTextBox,
                             GetString("ErrorOutputAlreadyUsed"), ControlNotificationSettings.Default);

                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmOutputOfflineDcu.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmOutputOfflineDcu_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifySpecialOutput(2);
            }
            else if (item.Name == "_tsiRemove2")
            {
                RemoveSpecialOutput(2);
            }
        }

        private void _tbmOutputOfflineDcu_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputOfflineDcu != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputOfflineDcu);
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.DcusLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.DcusLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void _tbmOutputSabotageDCUInputs_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify3")
            {
                ModifySpecialOutput(3);
            }
            else if (item.Name == "_tsiRemove3")
            {
                RemoveSpecialOutput(3);
            }
        }

        private void _tbmOutputSabotageDCUInputs_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmOutputSabotageDCUInputs_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputSabotageDCUInputs(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddOutputSabotageDCUInputs(object newOutput)
        {
            try
            {
                var o = newOutput as Output;
                if (o != null)
                {

                    if (CheckOutputAvailability(o, GetAvailableOutputs(false)))
                    {
                        _tbmOutputSabotageDCUInputs.Text = newOutput.ToString();
                        _tbmOutputSabotageDCUInputs.TextImage = Plugin.GetImageForAOrmObject(o);

                        _outputSabotageDcuInputs = o;
                        Plugin.AddToRecentList(o);
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmOutputSabotageDCUInputs.ImageTextBox,
                             GetString("ErrorOutputAlreadyUsed"), ControlNotificationSettings.Default);

                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmOutputSabotageDCUInputs.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmOutputSabotageDCUInputs_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputSabotageDcuInputs != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputSabotageDcuInputs);
            }
        }


        private bool ConfirmAddingUnlockedCardReader(CardReader cardReader)
        {
            if (cardReader == null || cardReader.SecurityLevel != (byte)SecurityLevel.Unlocked)
            {
                return true;
            }

            var internalCardReader = _crInternal as CardReader;
            var externalCardReader = _crExternal as CardReader;
            if ((internalCardReader != null && internalCardReader.SecurityLevel == (byte)SecurityLevel.Unlocked) ||
                (externalCardReader != null && externalCardReader.SecurityLevel == (byte)SecurityLevel.Unlocked))
            {
                return true;
            }

            if ((internalCardReader != null && internalCardReader.SecurityLevel == (byte)SecurityLevel.Locked) ||
                (externalCardReader != null && externalCardReader.SecurityLevel == (byte)SecurityLevel.Locked))
            {
                return Dialog.WarningQuestion(GetString("WarningQuestionAddUnlockedCardReaderToLockedDoorEnvironment"));
            }

            return Dialog.WarningQuestion(GetString("WarningQuestionAddUnlockedCardReaderToDoorEnvironment"));
        }

        private void _itbDoorEnvironment_DoubleClick(object sender, EventArgs e)
        {
            if (_doorEnvironment == null)
                return;

            NCASDoorEnvironmentsForm.Singleton.OpenEditForm(_doorEnvironment);
        }

        private void _cbOpenAllAlarmSettings_CheckedChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionAlarmSettings,
                _cbOpenAllAlarmSettings.CheckState);

            _accordionAlarmSettings.Focus();
        }

        private void OpenAllCheckStateChanged(Accordion accordion, CheckState checkState)
        {
            if (checkState == CheckState.Indeterminate)
                return;

            if (checkState == CheckState.Checked)
            {
                accordion.OpenAll();

                return;
            }

            accordion.CloseAll();
        }
    }

    public class ItemInputOutput
    {
        readonly byte _number;
        readonly string _name;

        public byte Number
        {
            get { return _number; }
        }

        public ItemInputOutput(string name, int number)
        {
            Validator.CheckNullString(name);

            _number = (byte)number;
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}