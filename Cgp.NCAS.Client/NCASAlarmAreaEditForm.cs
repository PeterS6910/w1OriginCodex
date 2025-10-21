//#define DESIGNER
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Components;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using CheckBox = System.Windows.Forms.CheckBox;
using Label = System.Windows.Forms.Label;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAlarmAreaEditForm :
#if DESIGNER
    Form
#else
 ACgpPluginEditFormWithAlarmInstructions<AlarmArea>
#endif
    {
        private const int DEFAULT_EIS_FILTER_TIME = 30;
        private const int DEFAULT_EIS_SET_UNSET_PULSE_LENGTH = 2;
        private const string COLUMN_STR_BLOCK_TEMPORARILY_UNTIL = "strBlockTemporarilyUntil";
        private const string COLUMN_SENSOR_STATE = "SensorState";
        private const string COLUMN_BLOCKING_TYPE = "BlockingType";
        private const string COLUMN_REQUESTED_BLOCKING_TYPE = "RequestedBlockingType";
        private const string COLUMN_SENSOR_PURPOSE = "SensorPurpose";
        private const string COLUMN_MOVE_UP = "Move up";
        private const string COLUMN_MOVE_DOWN = "Move down";

        private AOnOffObject _actObjAutomaticAct;
        private AOnOffObject _actObjForcedTimeBuying;
        private BindingSource _bsVisualAACardReaders;
        private BindingSource _bsAAInputs;
        private ListOfObjects _actInputObjects;
        private ListOfObjects _actCardReaderObjects;
        private readonly AACardReader _editingAACardReader = null;
        private Action<Guid, byte> _eventAlarmStateChanged;
        private Action<Guid, TimeBuyingMatrixState> _eventTimeBuyingMatrixStateChanged;
        private Action<Guid, byte> _eventActivationStateChanged;
        private Action<Guid, byte, bool> _eventRequestActivationStateChanged;
        private Action<Guid, byte> _eventSabotageStateChanged;
        private List<Guid> _deletedReferencedCRs;
        private ActivationState _activationState;
        private TimeBuyingMatrixState? _timeBuyingMatrixState;
        private RequestActivationState _requestActivationState = RequestActivationState.Unknown;
        private bool _firstEnter = true;
        private bool _outputSirenChanged;

        private Input _eisInputActivationState;
        private Output _eisSetUnsetOutput;
        private readonly Color _baseColorForResultOfAction;
        private readonly List<BlockTemporarilyUntilTypeView> _blockTemporarilyUntilTypeView = new List<BlockTemporarilyUntilTypeView>();
        private readonly List<SensorPurposeView> _sensorPurposeView = new List<SensorPurposeView>();

        private Action<Guid, string, int, int> _eventBoughtTimeChanged;
        private Action<Guid, byte, int, int> _eventTimeBuyingFailed;

        private ControlModifyAlarmArcs _cmaaAlarmReceptionCenterSettings;
        private ControlAlarmTypeSettings _catsAlarmAreaAlarm;
        private ControlAlarmTypeSettings _catsAlarmAreaSetByOnOffObjectFailed;
        private ControlAlarmTypeSettings _catsSensorAlarm;
        private ControlAlarmTypeSettings _catsSensorTamperAlarm;

        private ToolTip _toolTipForPbWarningUnsetAction = new ToolTip();

        private int _oldSectionId = -1;

        public NCASAlarmAreaEditForm(
                AlarmArea alarmArea,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                alarmArea,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

#if DEBUG
            _eBuyTime.CustomFormat = "HH:mm:ss";
            _eMaxTimeBuyingDuration.CustomFormat = "HH:mm:ss";
            _eMaxTotalTimeBuying.CustomFormat = "HH:mm:ss";
#endif

            WheelTabContorol = _tcAlarmArea;
            _eDescription.MouseWheel += ControlMouseWheel;
            _baseColorForResultOfAction = _eResultOfAction.BackColor;
            _eResultOfAction.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _bAlarmAreaSetUnset.Text = GetString("NCASAlarmAreaEditForm_bAlarmAreaUnset");
            _chbUnconditional.Visible = false;
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            SetReferenceEditColors();
            LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
            InitCGPDataGridView();

            foreach (BlockTemporarilyUntilType blockTemporarilyUntilType in Enum.GetValues(typeof(BlockTemporarilyUntilType)))
            {
                _blockTemporarilyUntilTypeView.Add(new BlockTemporarilyUntilTypeView(blockTemporarilyUntilType));
            }

            _sensorPurposeView.Add(new SensorPurposeView(null));

            foreach (SensorPurpose sensorPurpose in Enum.GetValues(typeof(SensorPurpose)))
            {
                var item = new SensorPurposeView(sensorPurpose);
                _cbPurpose.Items.Add(item);
                _sensorPurposeView.Add(item);
            }

            _cdgvDataInputs.DataGrid.ReadOnly = false;
            _cdgvDataInputs.DataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            _cdgvDataInputs.DataGrid.CurrentCellDirtyStateChanged += DataGrid_CurrentCellDirtyStateChanged;

            HideDisableTabPages();
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            InitAlarmAreasSettingsControls();

            foreach (var enumValue in Enum.GetValues(typeof (AlarmAreaAutomaticActivationMode)))
            {
                _cbAutomaticActivationMode.Items.Add(GetString(string.Format("AlarmAreaAutomaticActivationMode_{0}", enumValue)));
            }

            _cbAutomaticActivationMode.SelectedIndex = 0;

            RefreshOutputSetByObjectForAaFailed(AlarmAreaAutomaticActivationMode.TryToSetAreaUntilNotCalm);

            var toolTip = new ToolTip();
            toolTip.SetToolTip(_pbWarningNoEntrySensor, GetString("WarningNoEntryExitSensor"));
            toolTip.SetToolTip(_pbWarningNoExitSensor, GetString("WarningNoEntryExitSensor"));
            //toolTip.SetToolTip(_pbWarningUnsetAction, GetString("WarningTimeBuyingUnsetAction"));
            SetTimeBuyingInfoToolTip();
            _timeBuyingMatrixState = Plugin.MainServerProvider.AlarmAreas.GetTimeBuyingMatrixState(_editingObject.IdAlarmArea);
        }

        private readonly ToolTip _toolTip = new ToolTip();

        private void SetTimeBuyingInfoToolTip()
        {
            _toolTip.RemoveAll();

            _toolTip.SetToolTip(
                _pbWarningTimeBuyingInfo,
                GetString(
                    "TimeBuyingInfo",
                    _cbOfFTBInverted.Checked
                        ? GetString("General_Off")
                        : GetString("General_On")));
        }

        private readonly ToolTip _timeBuyingMatrixInfoToolTip = new ToolTip();

        private void SetTimeBuyingMatrixInfoToolTip()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetTimeBuyingMatrixInfoToolTip));
            }
            else
            {
                if (_editingObject.TimeBuyingEnabled
                    && _activationState == ActivationState.Set
                    || _activationState == ActivationState.TemporaryUnsetEntry
                    || _activationState == ActivationState.TemporaryUnsetExit)
                {
                    _pbTimeBuyingMatrixStateInfo.Visible = true;
                }
                else
                {
                    _pbTimeBuyingMatrixStateInfo.Visible = false;
                    return;
                }

                _timeBuyingMatrixInfoToolTip.RemoveAll();

                if (!_timeBuyingMatrixState.HasValue)
                    return;

                _timeBuyingMatrixInfoToolTip.SetToolTip(
                    _pbTimeBuyingMatrixStateInfo,
                    GetString(
                        string.Format(
                            _timeBuyingMatrixState != TimeBuyingMatrixState.O4TBA_OFF
                                ? "TimeBuyingMatrixState_{0}"
                                : GeneralOptionsForm.Singleton.AlarmAreaRestrictivePolicyForTimeBuying
                                    ? "TimeBuyingMatrixState_{0}_Restrictive"
                                    : "TimeBuyingMatrixState_{0}_Lenient",
                            _timeBuyingMatrixState.Value)));
            }
        }

        private void InitAlarmAreasSettingsControls()
        {
            _cmaaAlarmReceptionCenterSettings = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaAlarmReceptionCenterSettings",
                TabIndex = 0,
                Plugin = Plugin,
                MaximumSize = new Size(710, 300)
            };

            _cmaaAlarmReceptionCenterSettings.EditTextChanger += () => EditTextChanger(null, null);
            _accordionAlarmAreaAlarms.Add(_cmaaAlarmReceptionCenterSettings, "Alarm reception center settings").Name = "_chbAlarmReceptionCenterSettings";

            _catsAlarmAreaAlarm = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsAlarmAreaAlarm",
                TabIndex = 1,
                Plugin = Plugin,
                BlockAlarmVisible = false,
                AlarmEnableVisible = false,
                MaximumSize = new Size(710, 300)
            };

            _catsAlarmAreaAlarm.EditTextChanger += () => EditTextChanger(null, null);
            _catsAlarmAreaAlarm.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);
            _accordionAlarmAreaAlarms.Add(_catsAlarmAreaAlarm, "Alarm area alarm").Name = "_chbAlarmAreaAlarm";

            _catsAlarmAreaSetByOnOffObjectFailed = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsAlarmAreaSetByOnOffObjectFailed",
                TabIndex = 2,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                EventlogDuringBlockedAlarmVisible = false,
                MaximumSize = new Size(710, 300)
            };

            _catsAlarmAreaSetByOnOffObjectFailed.EditTextChanger += () => EditTextChanger(null, null);
            _catsAlarmAreaSetByOnOffObjectFailed.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);
            _accordionAlarmAreaAlarms.Add(_catsAlarmAreaSetByOnOffObjectFailed, "Set by on/off object failed").Name = "_chbSetByOnOffObjectFailed";

            _catsSensorAlarm = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsSensorAlarm",
                TabIndex = 3,
                Plugin = Plugin,
                BlockAlarmVisible = false,
                AlarmEnableVisible = false,
                MaximumSize = new Size(710, 300)
            };

            _catsSensorAlarm.EditTextChanger += () => EditTextChanger(null, null);
            _catsSensorAlarm.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);
            _accordionAlarmAreaAlarms.Add(_catsSensorAlarm, "Sensor alarm").Name = "_chbSensorAlarm";

            _catsSensorTamperAlarm = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsSensorTamperAlarm",
                TabIndex = 4,
                Plugin = Plugin,
                BlockAlarmVisible = false,
                AlarmEnableVisible = false,
                MaximumSize = new Size(710, 300)
            };

            _catsSensorTamperAlarm.EditTextChanger += () => EditTextChanger(null, null);
            _catsSensorTamperAlarm.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);
            _accordionAlarmAreaAlarms.Add(_catsSensorTamperAlarm, "Sensor tamper alarm").Name = "_chbSensorTamperAlarm";
        }

        private void InitCGPDataGridView()
        {
            _cdgvDataInputs.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvDataInputs.DataGrid.CellClick += _dgValues2_CellClick;
            _cdgvDataInputs.DataGrid.DragDrop += _dgValues2_DragDrop;
            _cdgvDataInputs.DataGrid.DragOver += _dgValues2_DragOver;
            _cdgvDataInputs.CgpDataGridEvents = new CgpDataGridViewActions(_cdgvData2_OpenEdit, () => _bDelete2_Click(null, null), InsertAAInputAction);
            _cdgvDataInputs.EnabledInsertButton = true;
            _cdgvDataInputs.SmallSizeInsertButton = true;
            _cdgvDataInputs.DataGrid.CellPainting += DataGrid_CellPainting;

            _cdgvDataCRs.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvDataCRs.DataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            _cdgvDataCRs.DataGrid.CellClick += _dgValues_CellClick;
            _cdgvDataCRs.DataGrid.DragDrop += _dgValues_DragDrop;
            _cdgvDataCRs.DataGrid.DragOver += _dgValues_DragOver;
            _cdgvDataCRs.CgpDataGridEvents = new CgpDataGridViewActions(_cdgvData1_OpenEdit, () => _bDelete_Click(null, null), InsertAACRAction);
            _cdgvDataCRs.EnabledInsertButton = true;
            _cdgvDataCRs.SmallSizeInsertButton = true;
        }

        void DataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex != 0)
                return;

            var aaInput = ((BindingSource) ((DataGridView) sender).DataSource).List[e.RowIndex] as AAInput;

            if (aaInput == null)
                return;

            var input = Plugin.MainServerProvider.Inputs.GetObjectById(aaInput.Input.IdInput);

            if (input == null)
                return;

            if (input.BlockingType != (byte)BlockingType.ForcefullyBlocked)
                return;

            var img = Cgp.Client.ResourceGlobal.IconBlocked16.ToBitmap();

            if (img == null)
                return;

            bool isSelected =
                (e.State & DataGridViewElementStates.Selected) ==
                    DataGridViewElementStates.Selected;

            e.PaintBackground(
                e.ClipBounds,
                isSelected);

            var p = e.CellBounds.Location;

            p.X += img.Width + 2;
            p.Y += 5;

            e.Graphics.DrawImage(
                img,
                e.CellBounds.X + 2,
                e.CellBounds.Y + 3,
                16,
                16);

            e.Graphics.DrawString(
                e.Value == null ? string.Empty : e.Value.ToString(),
                e.CellStyle.Font,
                isSelected
                    ? new SolidBrush(e.CellStyle.SelectionForeColor)
                    : new SolidBrush(e.CellStyle.ForeColor),
                p);

            e.Handled = true;
        }

        private void InsertAAInputAction()
        {
            var insertAAInputDialog = new AddAAInputsDialog(Plugin, _editingObject);

            if (insertAAInputDialog.ShowDialog() == DialogResult.OK)
            {
                _actInputObjects = insertAAInputDialog.ActInputObjects;
                _bInsert2_Click(insertAAInputDialog.LowSecurityInput, 
                    insertAAInputDialog.BlockTemporarilyUntil,
                    insertAAInputDialog.SensorPurpose);
            }
        }

        private void InsertAACRAction()
        {
            var insertAACrDialog = new AddAACardReaderDialog(Plugin, _editingObject);

            if (insertAACrDialog.ShowDialog() == DialogResult.OK)
            {
                _actCardReaderObjects = insertAACrDialog.ActCardReaderObjects;
                InsertAACardReader(insertAACrDialog.AASet,
                    insertAACrDialog.AAUnset,
                    insertAACrDialog.AAUnconditionalSet,
                    insertAACrDialog.PermanentlyUnlock,
                    insertAACrDialog.EnableEventlog);
            }
        }

        void LocalizationHelper_LanguageChanged()
        {
            RefreshSetUnsetButton();
        }

        protected override void RegisterEvents()
        {
            _eventAlarmStateChanged = ChangeAlarmState;
            AlarmStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventAlarmStateChanged);

            _eventTimeBuyingMatrixStateChanged = ChangeTimeBuyingMatrixState;
            TimeBuyingMatrixStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventTimeBuyingMatrixStateChanged);

            _eventActivationStateChanged = ChangeActivationState;
            ActivationStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventActivationStateChanged);

            _eventRequestActivationStateChanged = ChangeRequestActivationState;
            RequestActivationStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventRequestActivationStateChanged);

            _eventSabotageStateChanged = ChangeSabotageState;
            SabotageStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventSabotageStateChanged);

            if (_eventTimeBuyingFailed == null)
            {
                _eventTimeBuyingFailed = EventTimeBuyingFailed;
                AlarmAreaTimeBuyingHandler.Singleton.RegisterTimeBuyingFailed(_eventTimeBuyingFailed);
            }

            if (_eventBoughtTimeChanged == null)
            {
                _eventBoughtTimeChanged = EventBoughtTimeChanged;
                AlarmAreaTimeBuyingHandler.Singleton.RegisterBoughtTimeChanged(_eventBoughtTimeChanged);
            }

            AlarmAreaSensorBlockingTypeChangedHandler.Singleton.RegisterStateChanged(SensorBlockingTypeChanged);
            AlarmAreaSensorStateChangedHandler.Singleton.RegisterStateChanged(SensorStateChanged);
            AdvancedAccessSettingsChangedHandler.Singleton.RegisterAdvancedAccessSettingsChanged(AdvancedAccessSettingsChanged);
        }

        protected override void UnregisterEvents()
        {
            AlarmStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventAlarmStateChanged);
            ActivationStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventActivationStateChanged);
            RequestActivationStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventRequestActivationStateChanged);
            SabotageStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventSabotageStateChanged);

            if (_eventTimeBuyingFailed != null)
            {
                AlarmAreaTimeBuyingHandler.Singleton.UnregisterTimeBuyingFailed(_eventTimeBuyingFailed);
                _eventTimeBuyingFailed = null;
            }

            if (_eventBoughtTimeChanged != null)
            {
                AlarmAreaTimeBuyingHandler.Singleton.UnregisterBoughtTimeChanged(_eventBoughtTimeChanged);
                _eventBoughtTimeChanged = null;
            }

            AlarmAreaSensorBlockingTypeChangedHandler.Singleton.UnregisterStateChanged(SensorBlockingTypeChanged);
            AlarmAreaSensorStateChangedHandler.Singleton.UnregisterStateChanged(SensorStateChanged);
            AdvancedAccessSettingsChangedHandler.Singleton.UnregisterAdvancedAccessSettingsChanged(AdvancedAccessSettingsChanged);
        }

        private void AdvancedAccessSettingsChanged()
        {
            if (_editingObject == null
                || _editingObject.IdAlarmArea == Guid.Empty
                || !_timeBuyingMatrixState.HasValue)
            {
                return;
            }

            ChangeTimeBuyingMatrixState(
                _editingObject.IdAlarmArea,
                _timeBuyingMatrixState.Value);
        }

        private void SensorBlockingTypeChanged(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType? blockingType)
        {
            if (!idAlarmArea.Equals(_editingObject.IdAlarmArea))
                return;

            RefreshSensorBlockingType(
                idInput,
                blockingType);
        }

        private void RefreshSensorBlockingType(
            Guid idInput,
            SensorBlockingType? blockingType)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<Guid, SensorBlockingType?>(RefreshSensorBlockingType),
                    idInput,
                    blockingType);

                return;
            }

            for (int rowIndex = 0; rowIndex < _bsAAInputs.Count; rowIndex++)
            {
                var aaInput = _bsAAInputs[rowIndex] as AAInput;

                if (aaInput == null
                    || !idInput.Equals(aaInput.Input.IdInput))
                {
                    continue;
                }

                _cdgvDataInputs.DataGrid.Rows[rowIndex].Cells[COLUMN_BLOCKING_TYPE].Value =
                    new BlockingTypeView(blockingType);

                var cellRequestedBlockingType =
                    (DataGridViewComboBoxCell) _cdgvDataInputs.DataGrid.Rows[rowIndex].Cells[COLUMN_REQUESTED_BLOCKING_TYPE];

                if (blockingType == null
                    || blockingType == cellRequestedBlockingType.Value as SensorBlockingType?)
                {
                    ClearSensorRequestedBlockingType(cellRequestedBlockingType);
                }

                return;
            }
        }

        private void ClearSensorRequestedBlockingType(DataGridViewComboBoxCell cellRequestedBlockingType)
        {
            if (cellRequestedBlockingType.IsInEditMode)
            {
                _cdgvDataInputs.DataGrid.EndEdit();
            }

            cellRequestedBlockingType.DataSource = new List<BlockingTypeView> {new BlockingTypeView(null)};
            cellRequestedBlockingType.Value = null;
        }

        private void SensorStateChanged(
            Guid idAlarmArea,
            Guid idInput,
            State? sensorState)
        {
            if (!idAlarmArea.Equals(_editingObject.IdAlarmArea))
                return;

            RefreshSensorState(
                idInput,
                sensorState);
        }

        private void RefreshSensorState(
            Guid idInput,
            State? sensorState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<Guid, State?>(RefreshSensorState),
                    idInput,
                    sensorState);

                return;
            }

            for (int rowIndex = 0; rowIndex < _bsAAInputs.Count; rowIndex++)
            {
                var aaInput = _bsAAInputs[rowIndex] as AAInput;

                if (aaInput == null
                    || !idInput.Equals(aaInput.Input.IdInput))
                {
                    continue;
                }

                _cdgvDataInputs.DataGrid.Rows[rowIndex].Cells[COLUMN_SENSOR_STATE].Value =
                    new StateView(sensorState);

                return;
            }
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageBasicSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasBasicSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasBasicSettingsAdmin)));

                HideDisableTabPageBasicTiming(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasBasicTimingView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasBasicTimingAdmin)));

                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasAlarmSettingsAdmin)),
                    PresentationGroupsForm.Singleton.HasAccessView());

                HideDisableTabPageSpecialOutputs(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasSpecialOutputsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasSpecialOutputsAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmAreasDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageBasicSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageBasicSettings),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                    _eShortName.ReadOnly = true;

                    DisabledControls(_tpBasicSettings);
                }

                if (!view && !admin)
                {
                    _tcAlarmArea.TabPages.Remove(_tpBasicSettings);
                    return;
                }

                _cdgvDataCRs.DataGrid.ReadOnly = !admin;
                _cdgvDataInputs.DataGrid.ReadOnly = !admin;
            }
        }

        private void HideDisableTabPageBasicTiming(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageBasicTiming),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcAlarmArea.TabPages.Remove(_tpBasicTiming);
                    return;
                }

                _tpBasicTiming.Enabled = admin;
            }
        }

        private void HideDisableTabPageAlarmSettings(bool view, bool admin, bool viewPresentationGroup)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool, bool>(HideDisableTabPageAlarmSettings),
                    view,
                    admin,
                    viewPresentationGroup);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcAlarmArea.TabPages.Remove(_tpAlarmSettings);
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
                    _tcAlarmArea.TabPages.Remove(_tpSpecialOutputs);
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
                    _tcAlarmArea.TabPages.Remove(_tpUserFolders);
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
                    _tcAlarmArea.TabPages.Remove(_tpReferencedBy);
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
                    _tcAlarmArea.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("NCASAlarmAreaEditFormInsertText");
            }
            CgpClientMainForm.Singleton.SetTextOpenWindow(this);

            TranslateCbObjectForAutomaticDeactivation();
            NCASClient.LocalizationHelper.TranslateControl(_gbEISSettings);
            NCASClient.LocalizationHelper.TranslateControl(_gbAreaInputs);
            NCASClient.LocalizationHelper.TranslateControl(_gbCardReaders);

            RefreshOutputSetByObjectForAaFailed(
                (AlarmAreaAutomaticActivationMode) _cbAutomaticActivationMode.SelectedIndex);

            ShowInfoForNoCriticalSensors();
        }

        private void TranslateCbObjectForAutomaticDeactivation()
        {
            _cbAutomaticDeactivate.Text =
                NCASClient.LocalizationHelper.GetString(
                    _cbOfAAInverted.Checked
                        ? "NCASAlarmAreaEditForm_cbAutomaticDeactivateInverted"
                        : "NCASAlarmAreaEditForm_cbAutomaticDeactivate");
        }

        protected override void BeforeInsert()
        {
            NCASAlarmAreasForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASAlarmAreasForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASAlarmAreasForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASAlarmAreasForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.AlarmAreas.GetObjectForEdit(_editingObject.IdAlarmArea, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.AlarmAreas.GetObjectById(_editingObject.IdAlarmArea);
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
            _outputSirenChanged = false;

            if (_editingObject.AACardReaders != null)
            {
                foreach (var aaCardReader in _editingObject.AACardReaders)
                {
                    if (aaCardReader.CardReader != null)
                    {
                        aaCardReader.CardReader = Plugin.MainServerProvider.CardReaders.GetObjectById(aaCardReader.CardReader.IdCardReader);
                    }
                }
            }

            MethodInvoker mi = () =>
            {
                ShowAAInputs(-1);
                ShowAACardReaders();
            };

            if (InvokeRequired)
                BeginInvoke(mi);
            else
                mi();
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.AlarmAreas.RenewObjectForEdit(_editingObject.IdAlarmArea, out error);
            
            if (error != null)
            {
                throw error;
            }

            if (_editingObject.AACardReaders != null)
            {
                foreach (var aaCardReader in _editingObject.AACardReaders)
                {
                    if (aaCardReader.CardReader != null)
                    {
                        aaCardReader.CardReader = Plugin.MainServerProvider.CardReaders.GetObjectById(aaCardReader.CardReader.IdCardReader);
                    }
                }
            }

            MethodInvoker mi = () =>
                {
                    ShowAAInputs(-1);
                    ShowAACardReaders();
                };

            if (InvokeRequired)
                BeginInvoke(mi);
            else
                mi();
        }

        protected override void SetValuesInsert()
        {
            RefreshOnOffObject(_actObjAutomaticAct, _tbmObjAutomaticAct, _lICCU);
            RefreshOnOffObject(_actObjForcedTimeBuying, _tbmObjForcedTimeBuying, _lICCU1);

            RefreshPrealarm();
            RefreshPrewarning();
            RefreshTimeBuyingEnabled();
            RefreshABAlarmHandling();

            _ePrealarmWarningDuration.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 5, 0);
            _eTemporaryUnsetDuration.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 5);
            _ePrewarningDuration.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 5, 0);
            _eMaxTimeBuyingDuration.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, 0, 0);

            _chbAllowAAtoCRsReporting.CheckState = CheckState.Indeterminate;

            _bAlarmAreaSetUnset.Enabled = false;
            _chbUnconditional.Visible = false;
            _chbUseEIS.Checked = false;
            _cbNoPrewarning.Checked = false;
            _cbNoPrewarning.Visible = false;
            HideOrShowEISSettings();
            _nudEISFilterTime.Value = DEFAULT_EIS_FILTER_TIME;
            _nudEISSetUnsetPulseLength.Value = DEFAULT_EIS_SET_UNSET_PULSE_LENGTH;

            SetSelectedItemForPurpose();

            RefreshCardReaderEventlogsCheckBoxes(false);
            _bApply.Enabled = false;

            _eId.Value = Plugin.MainServerProvider.AlarmAreas.GetNewId();
            CheckTmpUnsetEntryExitFunctionality();

            _editingObject.AlwaysProvideUnsetForTimeBuying = true;
            _chbTimeBuyingAlwaysOn.Checked = _editingObject.AlwaysProvideUnsetForTimeBuying;
        }

        private void SetSelectedItemForPurpose()
        {
            foreach (SensorPurposeView sensorPurposeView in _cbPurpose.Items)
            {
                if (sensorPurposeView.Purpose != _editingObject.Purpose)
                    continue;

                _cbPurpose.SelectedItem = sensorPurposeView;
                break;
            }
        }

        protected override void SetValuesEdit()
        {
            LoadSettingsForEventlog();
            _eName.Text = _editingObject.Name;
            _eShortName.Text = _editingObject.ShortName;
            _actObjAutomaticAct = _editingObject.ObjForAutomaticAct;
            _oldSectionId = _editingObject.Id;
            
            RefreshOnOffObject(_actObjAutomaticAct, _tbmObjAutomaticAct, _lICCU);
            RefreshOnOffObject(_actObjForcedTimeBuying, _tbmObjForcedTimeBuying, _lICCU1);

            if (_editingObject.AllowSendingStateToCRs == null)
                _chbAllowAAtoCRsReporting.CheckState = CheckState.Indeterminate;
            else
                _chbAllowAAtoCRsReporting.Checked = _editingObject.AllowSendingStateToCRs.Value;

            _cbAutomaticDeactivate.Checked = _editingObject.AutomaticDeactive;
            _cbOfAAInverted.Checked = _editingObject.IsInvertedObjForAutomaticAct;
            _cbPrealamOn.Checked = _editingObject.PreAlarm;

            _ePrealarmWarningDuration.Value =
                _editingObject.PreAlarmDuration != null
                    ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0,
                        _editingObject.PreAlarmDuration.Value/60, _editingObject.PreAlarmDuration.Value%60)
                    : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 5, 0);

            RefreshPrealarm();

            _eTemporaryUnsetDuration.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0,
                _editingObject.TemporaryUnsetDuration/60, _editingObject.TemporaryUnsetDuration%60);

            _cbPrewarningOn.Checked = _editingObject.PreWarning;
            _cbTimeBuyingOnlyInPrewarning.Checked = _editingObject.TimeBuyingOnlyInPrewarning;

            _ePrewarningDuration.Value =
                _editingObject.PreWarningDuration != null
                    ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0,
                        _editingObject.PreWarningDuration.Value/60, _editingObject.PreWarningDuration.Value%60)
                    : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 5, 0);

            RefreshPrewarning();

            _cbActivateTimeBuyingOn.Checked = _editingObject.TimeBuyingEnabled;
            _cbMaxTimeBuyingDurationOn.Checked = _editingObject.TimeBuyingMaxDuration != null;
            _cbMaxTotalTimeBuyingOn.Checked = _editingObject.TimeBuyingTotalMax != null;

            if (_cbMaxTimeBuyingDurationOn.Checked)
            {
                _eMaxTimeBuyingDuration.Value = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    _editingObject.TimeBuyingMaxDuration.Value/3600,
                    (_editingObject.TimeBuyingMaxDuration.Value/60)%60,
                    _editingObject.TimeBuyingMaxDuration.Value%60);
            }
            else
            {
                _eMaxTimeBuyingDuration.Value = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    1,
                    0,
                    0);
            }

            if (_cbMaxTotalTimeBuyingOn.Checked)
            {
                _eMaxTotalTimeBuying.Value = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    _editingObject.TimeBuyingTotalMax.Value/3600,
                    (_editingObject.TimeBuyingTotalMax.Value/60)%60,
                    _editingObject.TimeBuyingTotalMax.Value%60);
            }
            else
            {
                _eMaxTotalTimeBuying.Value = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    1,
                    0,
                    0);
            }

            _actObjForcedTimeBuying = _editingObject.ObjForForcedTimeBuying;
            _cbOfFTBInverted.Checked = _editingObject.IsInvertedObjForForcedTimeBuying;
            _cbProvideOnlyUnset.Checked = _editingObject.NotForcedTimeBuyingProvideOnlyUnset;

            RefreshTimeBuyingEnabled();
            
            //_cbAcknowledgeOFF.Checked = _editingObject.AcknowledgeOFF;

            _eDescription.Text = _editingObject.Description;

            ChangeAlarmState(_editingObject.IdAlarmArea, (byte)Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaAlarmState(_editingObject.IdAlarmArea));
            ChangeActivationState(_editingObject.IdAlarmArea, (byte)Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(_editingObject.IdAlarmArea));
            ChangeRequestActivationState(_editingObject.IdAlarmArea, (byte)Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaRequestActivationState(_editingObject.IdAlarmArea), false);
            ChangeSabotageState(_editingObject.IdAlarmArea, (byte)Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaSabotageState(_editingObject.IdAlarmArea));
            RefreshImplictManager(Plugin.MainServerProvider.AlarmAreas.GetImplicitCCUForAlarmArea(_editingObject.IdAlarmArea));

            _chbABAlarmHandling.Checked = _editingObject.ABAlarmHandling;
            RefreshABAlarmHandling();
            _tbPercentageSensorsToAAlarm.Value = _editingObject.PercentageSensorsToAAlarm;

            if (_editingObject.OutputActivation != null)
            {
                _outputActivation = _editingObject.OutputActivation;
                _tbmOutputActivation.Text = _editingObject.OutputActivation.ToString();
                _tbmOutputActivation.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputActivation);
            }
            else
            {
                _tbmOutputActivation.Text = string.Empty;
            }

            if (_editingObject.OutputAlarmState != null)
            {
                _outputAlarmState = _editingObject.OutputAlarmState;
                _tbmOutputAlarmState.Text = _editingObject.OutputAlarmState.ToString();
                _tbmOutputAlarmState.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputAlarmState);
            }
            else
            {
                _tbmOutputAlarmState.Text = string.Empty;
            }

            if (_editingObject.OutputSabotage != null)
            {
                _outputSabotageState = _editingObject.OutputSabotage;
                _tbmOutputSabotage.Text = _editingObject.OutputSabotage.ToString();
                _tbmOutputSabotage.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputSabotage);
            }
            else
            {
                _tbmOutputSabotage.Text = string.Empty;
            }

            if (_editingObject.OutputPrewarning != null)
            {

                _outputPrewarning = _editingObject.OutputPrewarning;
                _tbmOutputPrewarning.Text = _editingObject.OutputPrewarning.ToString();
                _tbmOutputPrewarning.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputPrewarning);
            }
            else
            {
                _tbmOutputPrewarning.Text = string.Empty;
            }

            if (_editingObject.OutputTmpUnsetEntry != null)
            {
                _outputTmpUnsetEntry = _editingObject.OutputTmpUnsetEntry;
                _tbmOutputTmpUnsetEntry.Text = _editingObject.OutputTmpUnsetEntry.ToString();
                _tbmOutputTmpUnsetEntry.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputTmpUnsetEntry);
            }
            else
            {
                _tbmOutputTmpUnsetEntry.Text = string.Empty;
            }

            if (_editingObject.OutputTmpUnsetExit != null)
            {

                _outputTmpUnsetExit = _editingObject.OutputTmpUnsetExit;
                _tbmOutputTmpUnsetExit.Text = _editingObject.OutputTmpUnsetExit.ToString();
                _tbmOutputTmpUnsetExit.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputTmpUnsetExit);
            }
            else
            {
                _tbmOutputTmpUnsetExit.Text = string.Empty;
            }

            if (_editingObject.OutputAAlarm != null)
            {
                _outputAAlarm = _editingObject.OutputAAlarm;
                _tbmOutputAAlarm.Text = _editingObject.OutputAAlarm.ToString();
                _tbmOutputAAlarm.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputAAlarm);
            }
            else
            {
                _tbmOutputAAlarm.Text = string.Empty;
            }

            if (_editingObject.OutputSiren != null)
            {
                _outputSiren = _editingObject.OutputSiren;
                _tbmOutputSiren.Text = _editingObject.OutputSiren.ToString();
                _tbmOutputSiren.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputSiren);
            }
            else
            {
                _tbmOutputSiren.Text = string.Empty;
            }

            var sirenMaxOnPeriod = _editingObject.SirenMaxOnPeriod;
            var sirenMaxOnPeriodSeconds = 0;
            var sirenMaxOnPeriodMinutes = 15;
            if (sirenMaxOnPeriod != null)
            {
                sirenMaxOnPeriodSeconds = sirenMaxOnPeriod.Value % 60;
                sirenMaxOnPeriodMinutes = (sirenMaxOnPeriod.Value - sirenMaxOnPeriodSeconds) / 60;
            }

            _dtpSirenMaximumOnPeriod.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, sirenMaxOnPeriodMinutes, sirenMaxOnPeriodSeconds);

            if (_editingObject.OutputNotAcknowledged != null)
            {
                _outputNotAcknowledged = _editingObject.OutputNotAcknowledged;
                _tbmOutputNotAcknowledged.Text = _editingObject.OutputNotAcknowledged.ToString();
                _tbmOutputNotAcknowledged.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputNotAcknowledged);
            }
            else
            {
                _tbmOutputNotAcknowledged.Text = string.Empty;
            }

            if (_editingObject.OutputMotion != null)
            {
                _outputMotion = _editingObject.OutputMotion;
                _tbmOutputMotion.Text = _editingObject.OutputMotion.ToString();
                _tbmOutputMotion.TextImage = Plugin.GetImageForAOrmObject(_editingObject.OutputMotion);
            }
            else
            {
                _tbmOutputMotion.Text = string.Empty;
            }

            SetReferencedBy();

            RefreshOnOffObject(_actObjAutomaticAct, _tbmObjAutomaticAct, _lICCU);
            RefreshOnOffObject(_actObjForcedTimeBuying, _tbmObjForcedTimeBuying, _lICCU1);

            _chbUseEIS.Checked = _editingObject.UseEIS;

            if (_editingObject.UseEIS)
                DisabledControls(_gbAreaInputs);
            
            HideOrShowEISSettings();

            if (_editingObject.FilterTimeEIS != null)
            {
                _nudEISFilterTime.Value = _editingObject.FilterTimeEIS.Value;
            }
            else
            {
                _nudEISFilterTime.Value = DEFAULT_EIS_FILTER_TIME;
            }

            if (_editingObject.ActivationStateInputEIS != null)
            {
                var input = Plugin.MainServerProvider.Inputs.GetObjectById(_editingObject.ActivationStateInputEIS.Value);
                if (input != null)
                {
                    _eisInputActivationState = input;
                    RefreshEISInputActivationState();

                    _chbEISInputActivationStateInverted.Checked = input.Inverted;
                }
            }

            var setDefaultEISPulseLength = true;
            if (_editingObject.SetUnsetOutputEIS != null)
            {
                var output = Plugin.MainServerProvider.Outputs.GetObjectById(_editingObject.SetUnsetOutputEIS);
                if (output != null)
                {
                    _eisSetUnsetOutput = output;
                    RefreshEISSetUnsetOutput();

                    if (output.OutputType == (byte)OutputCharacteristic.pulsed)
                    {
                        _nudEISSetUnsetPulseLength.Value = (output.SettingsPulseLength / 1000);
                        setDefaultEISPulseLength = false;
                    }
                }
            }

            if (setDefaultEISPulseLength)
            {
                _nudEISSetUnsetPulseLength.Value = DEFAULT_EIS_SET_UNSET_PULSE_LENGTH;
            }

            RefreshCardReaderEventlogsCheckBoxes(false);

            _cmaaAlarmReceptionCenterSettings.AlarmArcs = LoadAlarmArcs(AlarmType.AlarmArea_Alarm);

            _catsAlarmAreaAlarm.PresentationGroup = _editingObject.PresentationGroup;

            _catsAlarmAreaSetByOnOffObjectFailed.AlarmEnabledCheckState =
                _editingObject.AlarmAreaSetByOnOffObjectFailed;

            _catsAlarmAreaSetByOnOffObjectFailed.BlockAlarmCheckState =
                _editingObject.BlockAlarmAreaSetByOnOffObjectFailed;

            _catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType,
                _editingObject.ObjBlockAlarmAreaSetByOnOffObjectFailedId);

            _catsAlarmAreaSetByOnOffObjectFailed.PresentationGroup =
                _editingObject.AlarmAreaSetByOnOffObjectFailedPresentationGroup;

            _catsSensorAlarm.PresentationGroup = _editingObject.SensorAlarmPresentationGroup;
            _catsSensorTamperAlarm.PresentationGroup = _editingObject.SensorTamperAlarmPresentationGroup;

            _eId.Value = _editingObject.Id;
            _cbAutomaticActivationMode.SelectedIndex = (int)_editingObject.AutomaticActivationMode;
            SetSelectedItemForPurpose();

            RefreshOutputSetByObjectForAaFailed(_editingObject.OutputSetByObjectForAaFailed);
            _eOutputSetAaNotCalmOnPeriod.Value = _editingObject.OutputSetNotCalmAaByObjectForAaOnPeriod;

            CheckTmpUnsetEntryExitFunctionality();
            _chbTimeBuyingAlwaysOn.Checked = _editingObject.AlwaysProvideUnsetForTimeBuying;

            _bApply.Enabled = false;
        }

        private  AOnOffObject GetBlockAlarmObject(byte? objectType, Guid? objectGuid)
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

        private ICollection<AlarmArc> LoadAlarmArcs(AlarmType alarmType)
        {
            return _editingObject.AlarmAreaAlarmArcs != null
                ? new LinkedList<AlarmArc>(
                    _editingObject.AlarmAreaAlarmArcs
                        .Where(
                            alarmAreaAlarmArc =>
                                alarmAreaAlarmArc.AlarmType == (byte)alarmType)
                        .Select(
                            alarmAreaAlarmArc =>
                                alarmAreaAlarmArc.AlarmArc))
                : null;
        }

        private Guid _guidImplicitCCU = Guid.Empty;

        private void RefreshImplictManager(CCU implicitCCU)
        {
            if (implicitCCU != null)
            {
                _itbImplicitManager.Text = implicitCCU.Name;
                _itbImplicitManager.Image = Plugin.GetImageForAOrmObject(implicitCCU);
                _guidImplicitCCU = implicitCCU.IdCCU;
            }
            else
            {
                _itbImplicitManager.Text = string.Empty;
                _guidImplicitCCU = Guid.Empty;  
            }

            _catsAlarmAreaSetByOnOffObjectFailed.SetParentCcu(_guidImplicitCCU);
        }

        private void RefreshImplictManager()
        {
            var guidImplicitCcu = Guid.Empty;

            if (!NCASClient.INTER_CCU_COMMUNICATION)
            {

                if (_editingObject.AAInputs != null && _editingObject.AAInputs.Count > 0)
                {
                    var aaInput = _editingObject.AAInputs.First();
                    if (aaInput != null && aaInput.Input != null)
                    {
                        guidImplicitCcu = Plugin.MainServerProvider.Inputs.GetParentCCU(aaInput.Input.IdInput);
                    }
                }
                else if (_editingObject.AACardReaders != null && _editingObject.AACardReaders.Count > 0)
                {
                    var aaCardReader = _editingObject.AACardReaders.First();
                    if (aaCardReader != null && aaCardReader.CardReader != null)
                    {
                        guidImplicitCcu =
                            Plugin.MainServerProvider.CardReaders.GetParentCCU(aaCardReader.CardReader.IdCardReader);
                    }
                }
                else if (_eisInputActivationState != null)
                {
                    guidImplicitCcu = Plugin.MainServerProvider.Inputs.GetParentCCU(_eisInputActivationState.IdInput);
                }
                else if (_eisSetUnsetOutput != null)
                {
                    guidImplicitCcu = Plugin.MainServerProvider.Outputs.GetParentCCU(_eisSetUnsetOutput.IdOutput);
                }
            }

            RefreshImplictManager(guidImplicitCcu != Guid.Empty
                ? Plugin.MainServerProvider.CCUs.GetObjectById(guidImplicitCcu)
                : null);
        }

        private void ChangeAlarmState(Guid alarmAreaGuid, byte state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(ChangeAlarmState), alarmAreaGuid, state);
            }
            else
            {
                if (alarmAreaGuid == _editingObject.IdAlarmArea)
                {
                    var actualStates = AlarmAreaAlarmStates.GetActualStates(LocalizationHelper, state);

                    _eAlarmState.Text =
                        actualStates != null
                            ? actualStates.ToString()
                            : "";
                }
            }
        }

        private void ChangeTimeBuyingMatrixState(Guid alarmAreaGuid, TimeBuyingMatrixState state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, TimeBuyingMatrixState>(ChangeTimeBuyingMatrixState), alarmAreaGuid, state);
            }
            else
            {
                if (alarmAreaGuid == _editingObject.IdAlarmArea)
                {
                    _timeBuyingMatrixState = state;
                    RefreshSetUnsetButton();
                    SetTimeBuyingMatrixInfoToolTip();
                }
            }
        }

        private void ChangeActivationState(Guid alarmAreaGuid, byte state)
        {
            if (alarmAreaGuid == _editingObject.IdAlarmArea)
            {
                _activationState = (ActivationState)state;
                RefreshActivationState(state);
                RefreshSetUnsetButton();
                RefreshSensorRequestedBlockingType();
                SetTimeBuyingMatrixInfoToolTip();
            }
        }

        private void RefreshSetUnsetButton()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(RefreshSetUnsetButton));
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                try
                {
                    var serverAlarmAreas = Plugin
                        .MainServerProvider
                        .AlarmAreas;

                    _bAlarmAreaSetUnset.Text =
                        GetString(
                            _activationState == ActivationState.Unset
                                ? "NCASAlarmAreaEditForm_bAlarmAreaSet"
                                : "NCASAlarmAreaEditForm_bAlarmAreaUnset");

                    if (_activationState == ActivationState.Unknown ||
                        _requestActivationState != RequestActivationState.Unknown)
                    {
                        _bAlarmAreaSetUnset.Enabled = false;
                        _chbUnconditional.Visible = false;
                        _chbUnconditional.Checked = false;

                        _cbNoPrewarning.Checked = false;
                        _cbNoPrewarning.Visible = false;

                        UpdateTimeBuyingControls(true);
                    }
                    else
                    {
                        var aa = GetEditingObject();

                        if (_activationState == ActivationState.Unset)
                        {
                            _bAlarmAreaSetUnset.Enabled =
                                serverAlarmAreas
                                    .HasAccessForAlarmAreaActionFromCurrentLogin(
                                        AccessNCAS.AlarmAreasSetPerform,
                                        aa.IdAlarmArea)
                                || serverAlarmAreas
                                    .HasAccessForAlarmAreaActionFromCurrentLogin(
                                        AccessNCAS.AlarmAreasUnconditionalSetPerform,
                                        aa.IdAlarmArea);
                        }
                        else
                        {
                            _bAlarmAreaSetUnset.Enabled =
                                serverAlarmAreas
                                    .HasAccessForAlarmAreaActionFromCurrentLogin(
                                        AccessNCAS.AlarmAreasUnsetPerform,
                                        aa.IdAlarmArea);
                        }

                        if (_activationState == ActivationState.Unset)
                        {
                            _chbUnconditional.Enabled =
                                serverAlarmAreas
                                    .HasAccessForAlarmAreaActionFromCurrentLogin(
                                        AccessNCAS.AlarmAreasUnconditionalSetPerform,
                                        GetEditingObject().IdAlarmArea);

                            _cbNoPrewarning.Enabled = _chbUnconditional.Enabled;
                        }

                        if (_activationState == ActivationState.Unset && !aa.UseEIS)
                        {
                            _chbUnconditional.Checked = false;
                            _chbUnconditional.Visible = true;
                            _cbNoPrewarning.Checked = false;
                            _cbNoPrewarning.Visible = GetEditingObject().PreWarning;
                        }
                        else
                        {
                            _chbUnconditional.Visible = false;
                            _cbNoPrewarning.Visible = false;
                        }

                        UpdateTimeBuyingControls(false);
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        private void RefreshActivationState(byte stateByte)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<byte>(RefreshActivationState), stateByte);
            }
            else
            {
                var activationStates = AlarmAreaActivationStates.GetActivationStates(LocalizationHelper, stateByte);

                _eActivationState.Text =
                    activationStates != null
                        ? activationStates.ToString()
                        : "";

                UpdateTimeBuyingControls(false);
            }
        }

        private void RefreshSensorRequestedBlockingType()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(RefreshSensorRequestedBlockingType));
                return;
            }

            if (!ActivationStateIsUnsetOrUnknown())
                return;

            for (int rowIndex = 0; rowIndex < _cdgvDataInputs.DataGrid.RowCount; rowIndex++)
            {
                var cellRequestedBlockingType =
                    (DataGridViewComboBoxCell) _cdgvDataInputs.DataGrid.Rows[rowIndex].Cells[COLUMN_REQUESTED_BLOCKING_TYPE];

                var requestedBlockingType = cellRequestedBlockingType.Value as SensorBlockingType?;

                if (requestedBlockingType == null
                    || requestedBlockingType.Value == SensorBlockingType.Unblocked
                    || requestedBlockingType.Value == SensorBlockingType.BlockPermanently)
                {
                    continue;
                }
                
                ClearSensorRequestedBlockingType(
                    (DataGridViewComboBoxCell) _cdgvDataInputs.DataGrid.Rows[rowIndex].Cells[COLUMN_REQUESTED_BLOCKING_TYPE]);
            }
        }

        private bool ActivationStateIsUnsetOrUnknown()
        {
            return _activationState == ActivationState.Unknown
                   || _activationState == ActivationState.Unset
                   || _activationState == ActivationState.UnsetBoughtTime;
        }

        private void ChangeRequestActivationState(Guid alarmAreaGuid, byte state, bool setUnsetNotConfirm)
        {
            if (alarmAreaGuid != _editingObject.IdAlarmArea)
                return;

            var newRequestActivationState = (RequestActivationState)state;
            if (newRequestActivationState == _requestActivationState)
                return;

            if (_requestActivationState != RequestActivationState.Unknown && newRequestActivationState == RequestActivationState.Unknown)
            {
                if (setUnsetNotConfirm)
                {
                    switch (_requestActivationState)
                    {
                        case RequestActivationState.Set:
                            SetSetAlarmAreaResult(AlarmAreaActionResult.SetUnsetNotConfirm);
                            break;
                        case RequestActivationState.Unset:
                            SetUnsetAlarmAreaResult(AlarmAreaActionResult.SetUnsetNotConfirm, false);
                            break;
                    }
                }
                else
                {
                    switch (_requestActivationState)
                    {
                        case RequestActivationState.Set:
                            SetSetAlarmAreaResult(AlarmAreaActionResult.Success);
                            break;
                        case RequestActivationState.UnconditionalSet:
                            SetUnconditionalSetAlarmAreaResult(AlarmAreaActionResult.Success);
                            break;
                        case RequestActivationState.Unset:
                            SetUnsetAlarmAreaResult(AlarmAreaActionResult.Success, false);
                            break;
                    }
                }
            }

            _requestActivationState = newRequestActivationState;
            RefreshRequestActivationState(newRequestActivationState);
            RefreshSetUnsetButton();
        }

        private void RefreshRequestActivationState(RequestActivationState requestActivationState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<RequestActivationState>(RefreshRequestActivationState), requestActivationState);
            }
            else
            {
                switch (requestActivationState)
                {
                    case RequestActivationState.Set:
                        _eRequestActivationState.Text = GetString("AlarmAreaRequestActivationStates_Set");
                        break;
                    case RequestActivationState.UnconditionalSet:
                        _eRequestActivationState.Text = GetString("AlarmAreaRequestActivationStates_UnconditionalSet");
                        break;
                    case RequestActivationState.Unset:
                        _eRequestActivationState.Text = GetString("AlarmAreaRequestActivationStates_Unset");
                        break;
                    default:
                        _eRequestActivationState.Text = string.Empty;
                        break;
                }
            }
        }

        private void ChangeSabotageState(Guid alarmAreaGuid, byte state)
        {
            if (alarmAreaGuid != _editingObject.IdAlarmArea)
            {
                return;
            }

            RefreshSabotageState((State)state);
        }

        private void RefreshSabotageState(State sabotageState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<State>(RefreshSabotageState), sabotageState);
            }
            else
            {
                switch (sabotageState)
                {
                    case State.Normal:
                        _eSabotage.Text = GetString("State_Normal");
                        break;
                    case State.Alarm:
                        _eSabotage.Text = GetString("State_Alarm");
                        break;
                    default:
                        _eSabotage.Text = GetString("State_Unknown");
                        break;
                }
            }
        }

        private void RefreshOnOffObject(AOnOffObject obj, TextBoxMenu objEditor, Label lICCU)
        {
            if (obj != null)
            {
                var listFS = new ListObjFS(obj);
                objEditor.Text = listFS.ToString();
                objEditor.TextImage = Plugin.GetImageForAOrmObject(obj);

                var idCCUObjAutomaticAct = Guid.Empty;
                if (obj is Input)
                {
                    var input = (Input)obj;

                    if (input.CCU != null)
                    {
                        idCCUObjAutomaticAct = input.CCU.IdCCU;
                    }
                    else if (input.DCU != null && input.DCU.CCU != null)
                    {
                        idCCUObjAutomaticAct = input.DCU.CCU.IdCCU;
                    }
                }
                else if (obj is Output)
                {
                    var output = (Output)obj;

                    if (output.CCU != null)
                    {
                        idCCUObjAutomaticAct = output.CCU.IdCCU;
                    }
                    else if (output.DCU != null && output.DCU.CCU != null)
                    {
                        idCCUObjAutomaticAct = output.DCU.CCU.IdCCU;
                    }
                }

                if (idCCUObjAutomaticAct != Guid.Empty 
                    && _guidImplicitCCU != Guid.Empty 
                    && idCCUObjAutomaticAct != _guidImplicitCCU)
                {
                    lICCU.Visible = true;
                }
                else
                {
                    lICCU.Visible = false;
                }

                _cbTimeBuyingOnlyInPrewarning.Checked = false;
            }
            else
            {
                objEditor.Text = string.Empty;
                lICCU.Visible = false;
            }

            
            _toolTipForPbWarningUnsetAction.SetToolTip(_pbWarningUnsetAction,
                _actObjAutomaticAct != null
                    ? GetString("WarningTimeBuyingUnsetAction")
                    : GetString("WarningTimeBuyingUnsetActionNoO4AA"));
            
            _pbWarningTimeBuyingInfo.Visible = _actObjForcedTimeBuying != null;
        }

        private void RefreshPrealarm()
        {
            _ePrealarmWarningDuration.Enabled = _cbPrealamOn.Checked;
        }

        private void RefreshPrewarning()
        {
            _ePrewarningDuration.Enabled = _cbPrewarningOn.Checked;
            _cbTimeBuyingOnlyInPrewarning.Enabled = _cbPrewarningOn.Checked && _cbActivateTimeBuyingOn.Checked;

            if (!_cbPrewarningOn.Checked)
            {
                _cbTimeBuyingOnlyInPrewarning.Checked = false;
                RefreshTimeBuyingEnabled();
            }
        }

        private void RefreshTimeBuyingEnabled()
        {
            _cbTimeBuyingOnlyInPrewarning.Enabled = _cbPrewarningOn.Checked && _cbActivateTimeBuyingOn.Checked;
            _cbMaxTimeBuyingDurationOn.Enabled = _cbActivateTimeBuyingOn.Checked;
            _cbMaxTotalTimeBuyingOn.Enabled = _cbActivateTimeBuyingOn.Checked;
            _eMaxTimeBuyingDuration.Enabled = _cbActivateTimeBuyingOn.Checked && _cbMaxTimeBuyingDurationOn.Checked;
            _eMaxTotalTimeBuying.Enabled = _cbActivateTimeBuyingOn.Checked && _cbMaxTotalTimeBuyingOn.Checked;

            _gbObjForcedTimeBuying.Enabled = _cbActivateTimeBuyingOn.Checked;
            if (!_gbObjForcedTimeBuying.Enabled)
            {
                _actObjForcedTimeBuying = null;
                RefreshOnOffObject(_actObjForcedTimeBuying, _tbmObjForcedTimeBuying, _lICCU1);
            }
        }

        private void RefreshABAlarmHandling()
        {
            _tbPercentageSensorsToAAlarm.Enabled = _chbABAlarmHandling.Checked;
            _tbmOutputAAlarm.Enabled = _chbABAlarmHandling.Checked;
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
                RemoveUnusedAACardReaders();

                _editingObject.Name = _eName.Text;
                _editingObject.ShortName = _eShortName.Text;

                _editingObject.ObjForAutomaticAct = _actObjAutomaticAct;
                _editingObject.AutomaticDeactive = _cbAutomaticDeactivate.Checked;
                _editingObject.IsInvertedObjForAutomaticAct = _cbOfAAInverted.Checked;

                _editingObject.ObjForForcedTimeBuying = _actObjForcedTimeBuying;
                _editingObject.IsInvertedObjForForcedTimeBuying = _cbOfFTBInverted.Checked;
                _editingObject.NotForcedTimeBuyingProvideOnlyUnset = _cbProvideOnlyUnset.Checked;

                _editingObject.PreAlarm = _cbPrealamOn.Checked;
                _editingObject.PreAlarmDuration = _ePrealarmWarningDuration.Value.Minute * 60 + _ePrealarmWarningDuration.Value.Second;
                _editingObject.TemporaryUnsetDuration = _eTemporaryUnsetDuration.Value.Minute * 60 + _eTemporaryUnsetDuration.Value.Second;
                _editingObject.PreWarning = _cbPrewarningOn.Checked;
                _editingObject.PreWarningDuration = _ePrewarningDuration.Value.Minute * 60 + _ePrewarningDuration.Value.Second;
                _editingObject.TimeBuyingOnlyInPrewarning = _cbTimeBuyingOnlyInPrewarning.Checked && _cbPrewarningOn.Checked;

                _editingObject.TimeBuyingEnabled = _cbActivateTimeBuyingOn.Checked && _cbActivateTimeBuyingOn.Enabled;
                _editingObject.TimeBuyingMaxDuration = _cbMaxTimeBuyingDurationOn.Checked
                    ? (int?)(_eMaxTimeBuyingDuration.Value.Hour * 3600
                        + _eMaxTimeBuyingDuration.Value.Minute * 60
                        + _eMaxTimeBuyingDuration.Value.Second)
                    : null;
                _editingObject.TimeBuyingTotalMax = _cbMaxTotalTimeBuyingOn.Checked
                    ? (int?)(_eMaxTotalTimeBuying.Value.Hour * 3600
                        + _eMaxTotalTimeBuying.Value.Minute * 60
                        + _eMaxTotalTimeBuying.Value.Second)
                    : null;

                //_editingObject.AcknowledgeOFF = _cbAcknowledgeOFF.Checked;
                _editingObject.Description = _eDescription.Text;
                _editingObject.ABAlarmHandling = _chbABAlarmHandling.Checked;
                _editingObject.PercentageSensorsToAAlarm = _tbPercentageSensorsToAAlarm.Value;

                //_editingObject.OutputActivation = _cbOutputActivation.SelectedItem as Output;
                //_editingObject.OutputAlarmState = _cbOutputAlarmState.SelectedItem as Output;
                //_editingObject.OutputPrewarning = _cbOutputPrewarning.SelectedItem as Output;
                //_editingObject.OutputTmpUnsetEntry = _cbOutputTmpUnsetEntry.SelectedItem as Output;
                //_editingObject.OutputTmpUnsetExit = _cbOutputTmpUnsetExit.SelectedItem as Output;
                //_editingObject.OutputAAlarm = _cbOutputAAlarm.SelectedItem as Output;


                _editingObject.OutputActivation = _outputActivation;
                _editingObject.OutputAlarmState = _outputAlarmState;
                _editingObject.OutputSabotage = _outputSabotageState;
                _editingObject.OutputPrewarning = _outputPrewarning;
                _editingObject.OutputTmpUnsetEntry = _outputTmpUnsetEntry;
                _editingObject.OutputTmpUnsetExit = _outputTmpUnsetExit;
                _editingObject.OutputAAlarm = _outputAAlarm;
                _editingObject.OutputSiren = _outputSiren;
                _editingObject.OutputNotAcknowledged = _outputNotAcknowledged;
                _editingObject.OutputMotion = _outputMotion;

                _editingObject.SirenMaxOnPeriod = (_dtpSirenMaximumOnPeriod.Value.Minute * 60) + _dtpSirenMaximumOnPeriod.Value.Second;

                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                _editingObject.UseEIS = _chbUseEIS.Checked;
                _editingObject.FilterTimeEIS = (int)_nudEISFilterTime.Value;

                if (_eisInputActivationState != null)
                {
                    _editingObject.ActivationStateInputEIS = _eisInputActivationState.IdInput;
                }
                else
                {
                    _editingObject.ActivationStateInputEIS = null;
                }

                if (_eisSetUnsetOutput != null)
                {
                    _editingObject.SetUnsetOutputEIS = _eisSetUnsetOutput.IdOutput;
                }
                else
                {
                    _editingObject.SetUnsetOutputEIS = null;
                }

                if (_cmaaAlarmReceptionCenterSettings.AlarmArcsWasChanged)
                {
                    SaveAlarmAreaArcs(_cmaaAlarmReceptionCenterSettings.AlarmArcs,
                        AlarmType.AlarmArea_Alarm);
                }

                _editingObject.PresentationGroup = _catsAlarmAreaAlarm.PresentationGroup;

                _editingObject.AlarmAreaSetByOnOffObjectFailed =
                        _catsAlarmAreaSetByOnOffObjectFailed.AlarmEnabledCheckState;

                _editingObject.BlockAlarmAreaSetByOnOffObjectFailed =
                        _catsAlarmAreaSetByOnOffObjectFailed.BlockAlarmCheckState;

                if (_catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmAreaSetByOnOffObjectFailedId =
                        (Guid)_catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm.GetId();

                    _editingObject.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType =
                        (byte)_catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm.GetObjectType();
                }

                _editingObject.AlarmAreaSetByOnOffObjectFailedPresentationGroup =
                    _catsAlarmAreaSetByOnOffObjectFailed.PresentationGroup;

                _editingObject.SensorAlarmPresentationGroup = _catsSensorAlarm.PresentationGroup;
                _editingObject.SensorTamperAlarmPresentationGroup = _catsSensorTamperAlarm.PresentationGroup;

                _editingObject.Id = (int) _eId.Value;
                _editingObject.AutomaticActivationMode = (AlarmAreaAutomaticActivationMode)_cbAutomaticActivationMode.SelectedIndex;
                _editingObject.Purpose = ((SensorPurposeView) _cbPurpose.SelectedItem).Purpose.Value;

                _editingObject.OutputSetNotCalmAaByObjectForAaOnPeriod = (int) _eOutputSetAaNotCalmOnPeriod.Value;
                _editingObject.AlwaysProvideUnsetForTimeBuying = _chbTimeBuyingAlwaysOn.Checked;

                return true;
            }
            catch
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private void SaveAlarmAreaArcs(IEnumerable<AlarmArc> alarmArcs, AlarmType alarmType)
        {
            if (_editingObject.AlarmAreaAlarmArcs == null)
                _editingObject.AlarmAreaAlarmArcs = new List<AlarmAreaAlarmArc>();
            else
                _editingObject.AlarmAreaAlarmArcs.Clear();

            if (alarmArcs != null)
            {
                foreach (var alarmArc in alarmArcs)
                {
                    _editingObject.AlarmAreaAlarmArcs.Add(
                        new AlarmAreaAlarmArc(
                            _editingObject,
                            alarmArc,
                            alarmType));
                }
            }
        }

        private void RemoveUnusedAACardReaders()
        {
            if (_editingObject.AACardReaders != null && _editingObject.AACardReaders.Count > 0)
            {
                var crsToRemove = new List<AACardReader>();
                foreach (var cr in _editingObject.AACardReaders)
                {
                    if (!cr.AASet
                        && !cr.AAUnconditionalSet
                        && !cr.AAUnset
                        && cr.PermanentlyUnlock
                        && !cr.EnableEventlog)
                    {
                        crsToRemove.Add(cr);
                    }
                }

                if (crsToRemove.Count > 0)
                {
                    foreach (var cr in crsToRemove)
                    {
                        _editingObject.AACardReaders.Remove(cr);
                    }
                }
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                CgpClient.Singleton.LocalizationHelper.GetString("ErrorEntryName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            ControlAACardReader();

            if (_chbUseEIS.Checked)
            {
                if (_eisInputActivationState == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmEISInputActivatianState,
                        GetString("ErrorEntryEISInputForActivationState"), CgpClient.Singleton.ClientControlNotificationSettings);

                    return false;
                }

                if (_eisSetUnsetOutput == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmEISSetUnsetOutput,
                        GetString("ErrorEntryEISSetUnsetOutput"), CgpClient.Singleton.ClientControlNotificationSettings);

                    return false;
                }
            }

            if (!_catsAlarmAreaSetByOnOffObjectFailed.ControlValues(
                control =>
                {
                    _tcAlarmArea.SelectedTab = _tpAlarmSettings;
                }))
            {
                return false;
            }

            // SB
            if (_cbMaxTotalTimeBuyingOn.Checked && _eMaxTimeBuyingDuration.Value > _eMaxTotalTimeBuying.Value)
            {
                if (_tcAlarmArea.SelectedTab != _tpBasicTiming)
                    _tcAlarmArea.SelectedTab = _tpBasicTiming;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eMaxTimeBuyingDuration,
                    GetString("ErrorMaxTimeBuyingDuration"), CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            if (_cbActivateTimeBuyingOn.Checked
                && _actObjForcedTimeBuying == null
                && !_chbTimeBuyingAlwaysOn.Checked)
            {
                if (_tcAlarmArea.SelectedTab != _tpBasicTiming)
                    _tcAlarmArea.SelectedTab = _tpBasicTiming;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmObjForcedTimeBuying,
                    GetString("ErrorObjForcedTimeBuyingNotSelected"), CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            return true;
        }

        private void ControlAACardReader()
        {
            try
            {
                var filterSettings = new List<FilterSettings>();
                var fSetting = new FilterSettings(AACardReader.COLUMNALARMAREA, _editingObject, ComparerModes.EQUALL);
                filterSettings.Add(fSetting);

                Exception error;
                var aaCardReaderDatabase = Plugin.MainServerProvider.AACardReaders.SelectByCriteria(filterSettings, out error);

                if (aaCardReaderDatabase == null || aaCardReaderDatabase.Count < 1) return;

                IList<AACardReader> badAACardReader = new List<AACardReader>();
                foreach (var aaCr in _editingObject.AACardReaders)
                {
                    if (aaCr.IdAACardReader == Guid.Empty)
                    {
                        foreach (var aaCrDBS in aaCardReaderDatabase)
                        {
                            if (aaCrDBS.CardReader != null && aaCr.CardReader != null &&
                                aaCrDBS.CardReader.IdCardReader == aaCr.CardReader.IdCardReader && !_deletedReferencedCRs.Contains(aaCr.CardReader.IdCardReader))
                            {
                                badAACardReader.Add(aaCr);
                            }
                        }
                    }
                }

                if (badAACardReader.Count > 0)
                {
                    foreach (var aaCr in badAACardReader)
                    {
                        _editingObject.AACardReaders.Remove(aaCr);
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Control and set forced off on the spesial output to siren
        /// </summary>
        private void CheckAndUpdateSirenOutput()
        {
            if (_outputSirenChanged)
            {
                if (_editingObject.OutputSiren != null)
                {
                    Exception error;
                    var outputSiren = Plugin.MainServerProvider.Outputs.GetObjectForEdit(_editingObject.OutputSiren.IdOutput, out error);

                    if (outputSiren == null)
                        return;

                    if ((outputSiren.OutputType == (byte)OutputCharacteristic.pulsed || outputSiren.OutputType == (byte)OutputCharacteristic.frequency) &&
                        !outputSiren.SettingsForcedToOff)
                    {
                        if (Dialog.Question(GetString("QuestionEnableForcedOffOnSirenOutput")))
                        {
                            outputSiren.SettingsForcedToOff = true;
                            Plugin.MainServerProvider.Outputs.Update(outputSiren, out error);
                        }
                    }

                    Plugin.MainServerProvider.Outputs.EditEnd(outputSiren);
                }

                _outputSirenChanged = false;
            }
        }

        private void SaveEISInputsOutputs()
        {
            if (_eisInputActivationState != null)
            {
                Exception error;
                var input = Plugin.MainServerProvider.Inputs.GetObjectForEdit(_eisInputActivationState.IdInput, out error);

                if (input != null)
                {
                    var saveInput = false;

                    if (input.Inverted != _chbEISInputActivationStateInverted.Checked)
                    {
                        input.Inverted = _chbEISInputActivationStateInverted.Checked;
                        saveInput = true;
                    }

                    if (saveInput)
                    {
                        Plugin.MainServerProvider.Inputs.Update(input, out error);
                    }

                    Plugin.MainServerProvider.Inputs.EditEnd(input);
                }
            }

            if (_eisSetUnsetOutput != null)
            {
                Exception error;
                var output = Plugin.MainServerProvider.Outputs.GetObjectForEdit(_eisSetUnsetOutput.IdOutput, out error);

                if (output != null)
                {
                    var saveOutput = false;

                    if (output.OutputType != (byte)OutputCharacteristic.pulsed)
                    {
                        output.OutputType = (byte)OutputCharacteristic.pulsed;
                        saveOutput = true;
                    }

                    if (output.SettingsPulseLength != (int)_nudEISSetUnsetPulseLength.Value * 1000)
                    {
                        output.SettingsPulseLength = (int)_nudEISSetUnsetPulseLength.Value * 1000;
                        saveOutput = true;
                    }

                    if (output.SettingsDelayToOn != 0)
                    {
                        output.SettingsDelayToOn = 0;
                        saveOutput = true;
                    }

                    if (output.SettingsDelayToOff != 0)
                    {
                        output.SettingsDelayToOff = 0;
                        saveOutput = true;
                    }

                    if (!output.SettingsForcedToOff)
                    {
                        output.SettingsForcedToOff = true;
                        saveOutput = true;
                    }

                    if (saveOutput)
                    {
                        Plugin.MainServerProvider.Outputs.Update(output, out error);
                    }

                    Plugin.MainServerProvider.Outputs.EditEnd(output);
                }
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.AlarmAreas.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message == AlarmArea.COLUMNID)
                    {
                        if (TryReplaceSectionId())
                        {
                            retValue = true;
                        }
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                            GetString("ErrorUsedAlarmAreaName"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _eName.Focus();
                    }
                }
                else
                    throw error;
            }

            if (retValue)
            {
                CheckAndUpdateSirenOutput();
                SaveEISInputsOutputs();
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

        private bool SaveToDatabaseEditCore(bool OnlyInDatabase)
        {
            Exception error;

            if (!CanSaveData())
                return false;

            bool retValue =
                OnlyInDatabase
                    ? Plugin.MainServerProvider.AlarmAreas.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.AlarmAreas.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message == AlarmArea.COLUMNID)
                    {
                        if (TryReplaceSectionId())
                        {
                            retValue = true;
                        }
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                            GetString("ErrorUsedAlarmAreaName"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _eName.Focus();
                    }
                }
                else
                    throw error;
            }

            if (retValue)
            {
                CheckAndUpdateSirenOutput();
                SaveEISInputsOutputs();
            }

            return retValue;
        }

        private bool TryReplaceSectionId()
        {
            var alarmAreaBySectionId = Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaBySectionId(_editingObject.Id);

            if (alarmAreaBySectionId == null)
                return false;

            int newSectionId = ShowOption == ShowOptionsEditForm.Edit
                ? _oldSectionId
                : Plugin.MainServerProvider.AlarmAreas.GetNewId();

            if (Dialog.WarningQuestion(
                GetString(
                "ErrorUsedAlarmAreaId",
                alarmAreaBySectionId.Id,
                alarmAreaBySectionId.Name,
                _editingObject.Name,
                newSectionId)))
            {
                return Plugin.MainServerProvider.AlarmAreas.ReplaceAlarmAreasSectionId(
                    alarmAreaBySectionId,
                    _editingObject);
            }

            return false;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.AlarmAreas != null)
                Plugin.MainServerProvider.AlarmAreas.EditEnd(_editingObject);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            SetSettingsForEventlog();
            Ok_Click();
        }

        private bool CanSaveData()
        {
            var currentState = Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(_editingObject.IdAlarmArea);
            if (currentState == ActivationState.Set)
            {
                if (_inputInserted || _inputModified)
                {
                    if (!_dialogConfirmed && !Dialog.Question(LocalizationHelper.GetString("NCASAlarmAreaEditForm_QuestionConfirmUnsetAA")))
                    {
                        return false;
                    }
                    AlarmAreaActionResult ar;
                    UnsetAlarmArea(out ar);
                    if (ar != AlarmAreaActionResult.Success)
                    {
                        Dialog.Error(LocalizationHelper.GetString("ErrorUnsetAABlocksSaveData"));
                        return false;
                    }
                }
            }
            return true;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (sender == _cbOfAAInverted)
                TranslateCbObjectForAutomaticDeactivation();

            if (sender == _cbPrealamOn)
                RefreshPrealarm();

            if (sender == _cbPrewarningOn)
                RefreshPrewarning();

            if (sender == _cbMaxTimeBuyingDurationOn
                || sender == _cbMaxTotalTimeBuyingOn
                || sender == _cbActivateTimeBuyingOn)
                RefreshTimeBuyingEnabled();

            if (sender == _tbPercentageSensorsToAAlarm)
                _lPercentageSensorsToAAlarmActual.Text = _tbPercentageSensorsToAAlarm.Value.ToString(CultureInfo.InvariantCulture);

            if (sender == _chbABAlarmHandling)
                RefreshABAlarmHandling();

            if (sender == _tbmOutputSiren.ImageTextBox.TextBox)
            {
                _outputSirenChanged = true;
            }

            if (sender == _chbUseEIS)
            {
                HideOrShowEISSettings();   
            }

            if (sender == _nudEISFilterTime)
            {
                if (_nudEISFilterTime.Value < _nudEISSetUnsetPulseLength.Value)
                {
                    _nudEISFilterTime.Value = _nudEISSetUnsetPulseLength.Value;
                }
            }

            if (sender == _nudEISSetUnsetPulseLength)
            {
                if (_nudEISSetUnsetPulseLength.Value > _nudEISFilterTime.Value)
                {
                    _nudEISSetUnsetPulseLength.Value = _nudEISFilterTime.Value;
                }
            }

            if (sender == _cbAutomaticActivationMode)
                RefreshOutputSetByObjectForAaFailed(
                    (AlarmAreaAutomaticActivationMode) _cbAutomaticActivationMode.SelectedIndex);

            if (!Insert)
                _bApply.Enabled = true;

            if (sender == _cbOfFTBInverted)
                SetTimeBuyingInfoToolTip();
        }

        private void RefreshOutputSetByObjectForAaFailed(AlarmAreaAutomaticActivationMode automaticActivationMode)
        {
            var unconditionalSet = automaticActivationMode == AlarmAreaAutomaticActivationMode.UnconditionalSet;

            _gbOutputSetByObjectForAaFailed.Text = GetString(
                unconditionalSet
                    ? "NCASAlarmAreaEditForm_OutputSetNotCalmAaByObjectForAa"
                    : "NCASAlarmAreaEditForm_OutputSetByObjectForAaFailed");

            _lOutputSetAaNotCalmOnPeriod.Enabled = unconditionalSet;
            _eOutputSetAaNotCalmOnPeriod.Enabled = unconditionalSet;
        }

        private void HideOrShowEISSettings()
        {
            if (_chbUseEIS.Checked)
            {
                if (_editingObject.AAInputs != null && _editingObject.AAInputs.Count > 0)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _chbUseEIS,
                        GetString("ErrorEISCanNotHaveInternalSnsors"), ControlNotificationSettings.Default);

                    _chbUseEIS.Checked = false;
                    return;
                }

                _tblAreaInputsAndCardReaders.RowStyles[0].Height = 0;
                _gbEISSettings.Visible = true;
                _tblAreaInputsAndCardReaders.Location = new Point(_tblAreaInputsAndCardReaders.Location.X, 228);
                _tblAreaInputsAndCardReaders.Height -= _gbEISSettings.Height;

                RefreshSetUnsetButton();
                _gbPrewarning.Visible = false;
                _gbPrealarm.Visible = false;
                _gbTimeBuying.Visible = false;

                _lTemporaryUnsetExitDuration.Visible = false;
                _eTemporaryUnsetDuration.Visible = false;
            }
            else if (_gbEISSettings.Visible)
            {
                _tblAreaInputsAndCardReaders.RowStyles[0].Height = 50;
                _gbEISSettings.Visible = false;
                _tblAreaInputsAndCardReaders.Location = new Point(_tblAreaInputsAndCardReaders.Location.X, 122);
                _tblAreaInputsAndCardReaders.Height += _gbEISSettings.Height;

                RefreshSetUnsetButton();
                _gbPrewarning.Visible = true;
                _gbPrealarm.Visible = true;
                //_gbTimeBuying.Visible = true;

                _lTemporaryUnsetExitDuration.Visible = true;
                _eTemporaryUnsetDuration.Visible = true;

                SetEISInputActivatianState(null, false);
                SetEISSetUnsetOutput(null, false);
                _chbEISInputActivationStateInverted.Checked = false;
                _nudEISFilterTime.Value = DEFAULT_EIS_FILTER_TIME;
                _nudEISSetUnsetPulseLength.Value = DEFAULT_EIS_SET_UNSET_PULSE_LENGTH;
            }
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void ModifyObject(ref AOnOffObject obj, TextBoxMenu objEditor, Label lICCU)
        {
            try
            {
                ConnectionLost();
                var listObjects = new List<IModifyObject>();
                Exception error;

                var listTz = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null)
                    throw error;

                listObjects.AddRange(listTz);

                var listDailyPlans = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlans);
                listObjects.AddRange(listDailyPlans);

                var listInputs = Plugin.MainServerProvider.Inputs.ListModifyObjectsForAlarmAreaActivationObject(_editingObject.IdAlarmArea, out error);
                if (error != null)
                    throw error;

                listObjects.AddRange(listInputs);

                var listOutputs = Plugin.MainServerProvider.Outputs.ListModifyObjectsForAlarmAreaActivationObject(_editingObject.IdAlarmArea, out error);
                if (error != null)
                    throw error;

                listObjects.AddRange(listOutputs);

                var formAdd = new ListboxFormAdd(listObjects, GetString("NCASAlaramAreaEditFormListObjectForAutomaticActivationText"));
                IModifyObject outObject;
                formAdd.ShowDialog(out outObject);

                if (outObject != null)
                {
                    obj = null;

                    switch (outObject.GetOrmObjectType)
                    {
                        case ObjectType.TimeZone:
                        {
                            var timeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outObject.GetId);
                            obj = timeZone;
                            Plugin.AddToRecentList(obj);
                            break;
                        }
                        case ObjectType.DailyPlan:
                        {
                            var dailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(outObject.GetId);
                            obj = dailyPlan;
                            Plugin.AddToRecentList(obj);
                            break;
                        }
                        case ObjectType.Input:
                        {
                            var input = Plugin.MainServerProvider.Inputs.GetObjectById(outObject.GetId);
                            if (input != null)
                                if (_editingObject.AAInputs != null)
                                    foreach (var aaInput in _editingObject.AAInputs)
                                    {
                                        if (aaInput.Input != null && aaInput.Input.IdInput == input.IdInput)
                                        {
                                            Dialog.Error(input.ToString(), GetString("ErrorObjectAlreadyUsedAsAlarmAreaInput"));
                                            return;
                                        }
                                    }
                            obj = input;
                            Plugin.AddToRecentList(obj);
                            break;
                        }
                        case ObjectType.Output:
                        {
                            var output = Plugin.MainServerProvider.Outputs.GetObjectById(outObject.GetId);
                            obj = output;
                            Plugin.AddToRecentList(obj);
                            break;
                        }
                    }

                    RefreshOnOffObject(obj, objEditor, lICCU);
                }
            }
            catch
            {
            }
        }

        private void _tbmObjAutomaticAct_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) 
                    return;

                AddObjAutomaticAct(
                    e.Data.GetData(output[0]), 
                    ref _actObjAutomaticAct, 
                    _tbmObjAutomaticAct, 
                    _lICCU);
            }
            catch
            {
            }
        }

        private void _tbmObjForcedTimeBuying_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) 
                    return;

                AddObjAutomaticAct(e.Data.GetData(
                    output[0]), 
                    ref _actObjForcedTimeBuying, 
                    _tbmObjForcedTimeBuying, 
                    _lICCU1);
            }
            catch
            {
            }
        }

        private void AddObjAutomaticAct(object obj, ref AOnOffObject onOffObject, TextBoxMenu objEditor, Label lICCU)
        {
            try
            {
                if (obj.GetType() == typeof(TimeZone) || obj.GetType() == typeof(DailyPlan) ||
                    obj.GetType() == typeof(Input) || obj.GetType() == typeof(Output))
                {
                    if (obj is Output)
                    {
                        var output = obj as Output;
                        ConnectionLost();
                        if (!Plugin.MainServerProvider.Outputs.OutputFromTheSameCCUOrInterCCUComunicationEnabledAlarmAreaActivationObject(_editingObject.IdAlarmArea, output.IdOutput))
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmObjAutomaticAct.ImageTextBox,
                                GetString("InterCCUCommunicationNotEnabled"), ControlNotificationSettings.Default);

                            return;
                        }
                    }

                    if (obj is Input)
                    {
                        var input = obj as Input;
                        ConnectionLost();
                        if (!Plugin.MainServerProvider.Inputs.InputFromTheSameCCUOrInterCCUComunicationEnabledAlarmAreaActivationObject(_editingObject.IdAlarmArea, input.IdInput))
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmObjAutomaticAct.ImageTextBox,
                                GetString("InterCCUCommunicationNotEnabled"), ControlNotificationSettings.Default);

                            return;
                        }
                    }

                    if (obj is AOnOffObject)
                        onOffObject = obj as AOnOffObject;
                    else
                        onOffObject = null;

                    RefreshOnOffObject(onOffObject, objEditor, lICCU);

                    Plugin.AddToRecentList(obj);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmObjAutomaticAct.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmOnOffObject_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null 
                    || output.Length < 1)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                object o = e.Data.GetData(output[0]);
                if(o is AOnOffObject)
                    e.Effect = DragDropEffects.All;
                else
                    e.Effect = DragDropEffects.None;
            }
            catch (Exception)
            {

                e.Effect = DragDropEffects.None;
            }
        }

        private void _tbmObjAutomaticAct_DoubleClick(object sender, EventArgs e)
        {
            OpenEditForm(_actObjAutomaticAct);
        }

        private void _tbmObjForcedTimeBuying_DoubleClick(object sender, EventArgs e)
        {
            OpenEditForm(_actObjForcedTimeBuying);
        }

        private void OpenEditForm(AOnOffObject obj)
        {
            if(obj == null)
                return;

            if (obj is TimeZone)
            {
                TimeZonesForm.Singleton.OpenEditForm((TimeZone)obj);
            }
            else if (obj is DailyPlan)
            {
                DailyPlansForm.Singleton.OpenEditForm((DailyPlan)obj);
            }
            else if (obj is Input)
            {
                NCASInputsForm.Singleton.OpenEditForm((Input)obj);
            }
            else if (obj is Output)
            {
                NCASOutputsForm.Singleton.OpenEditForm((Output)obj);
            }
        }

        private void _tpBaseSettings_Enter(object sender, EventArgs e)
        {
            if (_firstEnter)
            {
                _firstEnter = false;
                ShowAAInputs(-1);
                ShowAACardReaders();
            }
        }

        private void ShowAACardReaders()
        {
            try
            {
                if (_editingObject.AACardReaders != null)
                {
                    _bsVisualAACardReaders =
                        new BindingSource
                        {
                            DataSource = GetVisualAACardReaders()
                        };

                    _cdgvDataCRs.DataGrid.DataSource = _bsVisualAACardReaders;
                    HideColumnDgw(_cdgvDataCRs.DataGrid, AACardReader.COLUMNIDAACARDREADER);
                    HideColumnDgw(_cdgvDataCRs.DataGrid, AACardReader.COLUMNALARMAREA);
                    HideColumnDgw(_cdgvDataCRs.DataGrid, AACardReader.COLUMNGUIDALARMAREA);
                    HideColumnDgw(_cdgvDataCRs.DataGrid, AACardReader.COLUMNGUIDCARDREADER);
                    HideColumnDgw(_cdgvDataCRs.DataGrid, AACardReader.ColumnVersion);

                    if (_cdgvDataCRs.DataGrid.Columns.Contains(AACardReader.COLUMNAASET))
                        _cdgvDataCRs.DataGrid.Columns[AACardReader.COLUMNAASET].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    if (_cdgvDataCRs.DataGrid.Columns.Contains(AACardReader.COLUMNAAUNSET))
                        _cdgvDataCRs.DataGrid.Columns[AACardReader.COLUMNAAUNSET].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    if (_cdgvDataCRs.DataGrid.Columns.Contains(AACardReader.COLUMNAAUNCONDITIONALSET))
                        _cdgvDataCRs.DataGrid.Columns[AACardReader.COLUMNAAUNCONDITIONALSET].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    if (_cdgvDataCRs.DataGrid.Columns.Contains(AACardReader.COLUMNPERMANENTLYUNLOCK))
                        _cdgvDataCRs.DataGrid.Columns[AACardReader.COLUMNPERMANENTLYUNLOCK].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    if (_cdgvDataCRs.DataGrid.Columns.Contains(AACardReader.COLUMNCARDREADER))
                        _cdgvDataCRs.DataGrid.Columns[AACardReader.COLUMNCARDREADER].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (!_cdgvDataCRs.DataGrid.Columns.Contains("WarningImage"))
                    {
                        var imageColumn = new DataGridViewImageColumn();
                        imageColumn.DefaultCellStyle.NullValue = null;
                        imageColumn.Name = "WarningImage";
                        imageColumn.HeaderText = string.Empty;
                        imageColumn.Width = 30;
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        _cdgvDataCRs.DataGrid.Columns.Insert(7, imageColumn);
                    }

                    int i = 0;

                    foreach (var obj in _bsVisualAACardReaders)
                    {
                        var aaCr = (AACardReader)obj;

                        if (aaCr.PermanentlyUnlock
                            &&
                            !Plugin.MainServerProvider.DoorEnvironments.HasCardReaderDoorEnvironment(
                                aaCr.CardReader.IdCardReader))
                        {
                            _cdgvDataCRs.DataGrid.Rows[i].Cells["WarningImage"].Value = ResourceGlobal.Warning;
                            _cdgvDataCRs.DataGrid.Rows[i].Cells["WarningImage"].ToolTipText = GetString("WarningCardReaderNotUsedToAnyDoorEnvironment");
                        }

                        i++;
                    }

                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvDataCRs.DataGrid);

                    if (!_cdgvDataCRs.DataGrid.Columns.Contains("Empty"))
                    {
                        _cdgvDataCRs.DataGrid.Columns.Add("Empty", string.Empty);
                        _cdgvDataCRs.DataGrid.Columns["Empty"].Width = 35;
                    }
                }
                else
                {
                    _bsVisualAACardReaders = null;
                    _cdgvDataCRs.DataGrid.DataSource = null;
                }
            }
            catch
            {
                _bsVisualAACardReaders = null;
                _cdgvDataCRs.DataGrid.DataSource = null;
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTable"));
            }

        }

        private ICollection<AACardReader> GetVisualAACardReaders()
        {
            var result = new LinkedList<AACardReader>();

            foreach (var aaCardReader in _editingObject.AACardReaders)
            {
                var resultAaCardReader =
                    new AACardReader
                    {
                        AASet = aaCardReader.AASet,
                        AAUnset = aaCardReader.AAUnset,
                        AAUnconditionalSet = aaCardReader.AAUnconditionalSet,
                        AlarmArea = aaCardReader.AlarmArea,
                        IdAACardReader = aaCardReader.IdAACardReader,
                        EnableEventlog = aaCardReader.EnableEventlog,
                        CardReader = aaCardReader.CardReader,
                        PermanentlyUnlock = !aaCardReader.PermanentlyUnlock
                    };
                result.AddLast(resultAaCardReader);
            }

            return result.OrderBy(
                aaCardReader =>
                    aaCardReader.CardReader.ToString())
                .ToArray();
        }

        private bool CheckCardReaderBeforeAdding(
            CardReader cardReader,
            out string errorMessage)
        {
            if (!NCASClient.INTER_CCU_COMMUNICATION)
            {
                if (
                    !Plugin.MainServerProvider.AlarmAreas.SetCardReaderToAlarmArea(cardReader.IdCardReader,
                        _guidImplicitCCU))
                {
                    errorMessage = GetString("InterCCUCommunicationNotEnabled");
                    return false;
                }
            }

            if (_editingObject.AACardReaders == null)
            {
                errorMessage = null;
                return true;
            }

            if (_editingObject.AACardReaders.Any(
                actAaCardReader =>
                    actAaCardReader.CardReader != null
                    && actAaCardReader.CardReader.Compare(cardReader)))
            {
                errorMessage = GetString("ErrorCardReaderAlreadyInAlarmArea");
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void GetValuesAACardReader(AACardReader aaCardReader, 
            CardReader cardReader, 
            bool AASet, 
            bool AAUnset, 
            bool AAUnconditionalSet, 
            bool PermanentlyUnlock, 
            bool EnableEventlog)
        {
            aaCardReader.AlarmArea = _editingObject;
            aaCardReader.CardReader = cardReader;
            aaCardReader.AASet = AASet;
            aaCardReader.AAUnset = AAUnset;
            aaCardReader.AAUnconditionalSet = AAUnconditionalSet;
            aaCardReader.PermanentlyUnlock = PermanentlyUnlock;
            aaCardReader.EnableEventlog = EnableEventlog;
        }

        private bool GetValuesAACardReader(out IList<AACardReader> aaCardReaders, 
            bool AASet, bool AAUnset, bool AAUnconditionalSet, bool PermanentlyUnlock, bool EnableEventlog)
        {
            aaCardReaders = null;

            if (ControlValuesAACardReader())
            {
                try
                {
                    aaCardReaders = new List<AACardReader>();

                    foreach (var obj in _actCardReaderObjects.Objects)
                    {
                        var aaCardReader = new AACardReader();
                        GetValuesAACardReader(aaCardReader, obj as CardReader, AASet, AAUnset, AAUnconditionalSet, PermanentlyUnlock, EnableEventlog);
                        aaCardReaders.Add(aaCardReader);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private bool ControlValuesAACardReader()
        {
            if (_actCardReaderObjects == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvDataCRs,
                    GetString("ErrorEntryCardReader"), ControlNotificationSettings.Default);
                return false;
            }

            if (_editingObject.AACardReaders != null)
            {
                foreach (var actAACardReader in _editingObject.AACardReaders)
                {
                    foreach (var obj in _actCardReaderObjects)
                    {
                        if (_editingAACardReader != actAACardReader && actAACardReader.CardReader != null &&
                            actAACardReader.CardReader.Compare(obj))
                        {
                            Dialog.Error(GetString("ErrorCardReaderAlreadyInAlarmArea"));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void InsertAACardReader(bool AASet, bool AAUnset, bool AAUnconditionalSet, bool PermanentlyUnlock, bool EnableEventlog)
        {
            IList<AACardReader> aaCardReaders;
            if (GetValuesAACardReader(out aaCardReaders, AASet, AAUnset, AAUnconditionalSet, PermanentlyUnlock, EnableEventlog))
            {
                if (aaCardReaders != null)
                {
                    if (_editingObject.AACardReaders == null)
                        _editingObject.AACardReaders = new List<AACardReader>();

                    foreach (var aaCardReader in aaCardReaders)
                    {
                        var addCardReader = true;
                        if (!NCASClient.INTER_CCU_COMMUNICATION && _guidImplicitCCU != Guid.Empty)
                        {
                            var cardReader = aaCardReader.CardReader;
                            if (cardReader != null)
                            {
                                CCU cardReaderCCU = null;
                                if (cardReader.DCU != null)
                                {
                                    cardReaderCCU = cardReader.DCU.CCU;
                                }
                                else if (cardReader.CCU != null)
                                {
                                    cardReaderCCU = cardReader.CCU;
                                }

                                if (cardReaderCCU == null)
                                {
                                    addCardReader = false;
                                }
                                else
                                {
                                    if (cardReaderCCU.IdCCU != _guidImplicitCCU)
                                        addCardReader = false;
                                }
                            }
                        }

                        if (addCardReader)
                        {
                            _editingObject.AACardReaders.Add(aaCardReader);
                            RefreshImplictManager();
                        }
                    }
                }
            }

            ShowAACardReaders();
            EditTextChanger(null, null);
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_bsVisualAACardReaders == null || _bsVisualAACardReaders.Count == 0) return;

            if (Dialog.Question(CgpClient.Singleton.LocalizationHelper.GetString("QuestionDeleteConfirm")))
            {
                var aaCardReader = (AACardReader)_bsVisualAACardReaders.List[_bsVisualAACardReaders.Position];
                DeleteAACardReader(aaCardReader);
            }
        }

        private void DeleteAACardReader(AACardReader aaCardReader)
        {
            aaCardReader = GetFromVisualCR(aaCardReader);

            if (aaCardReader == null) 
                return;

            if (_editingAACardReader == aaCardReader)
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteEditing"));
                return;
            }

            _editingObject.AACardReaders.Remove(aaCardReader);

            if (_deletedReferencedCRs == null)
                _deletedReferencedCRs = new List<Guid>();

            if (!_deletedReferencedCRs.Contains(aaCardReader.CardReader.IdCardReader))
                _deletedReferencedCRs.Add(aaCardReader.CardReader.IdCardReader);

            RefreshImplictManager();

            ShowAACardReaders();
            EditTextChanger(null, null);
        }

        private AACardReader GetFromVisualCR(AACardReader aaCardReader)
        {
            return
                _editingObject.AACardReaders.FirstOrDefault(
                    aaCr => 
                        aaCardReader.IdAACardReader == aaCr.IdAACardReader && 
                        aaCardReader.CardReader.IdCardReader == aaCr.CardReader.IdCardReader);
        }

        private void _dgValues_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddAACardReader(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddAACardReader(object newCardReader)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) 
                return;

            try
            {
                if (newCardReader.GetType() != typeof(CardReader))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cdgvDataCRs.DataGrid,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                var cardReader = newCardReader as CardReader;

                if (cardReader == null)
                    return;

                string errorMessage;

                if (!CheckCardReaderBeforeAdding(
                    cardReader,
                    out errorMessage))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cdgvDataCRs.DataGrid,
                        errorMessage,
                        ControlNotificationSettings.Default);

                    return;
                }

                if (_editingObject.AACardReaders == null)
                    _editingObject.AACardReaders = new List<AACardReader>();

                _editingObject.AACardReaders.Add(
                    new AACardReader
                    {
                        AlarmArea = _editingObject,
                        CardReader = cardReader,
                        AASet = false,
                        AAUnset = false,
                        AAUnconditionalSet = false,
                        PermanentlyUnlock = false
                    });

                RefreshImplictManager();
                ShowAACardReaders();
                EditTextChanger(null, null);
                Plugin.AddToRecentList(newCardReader);
            }
            catch
            {
                Dialog.Error(GetString("ErrorAddAACardReader"));
            }
        }

        private static void _dgValues_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void SetAlarmArea(bool noPrewarning)
        {
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
            {
                var result = 
                    Plugin.MainServerProvider.AlarmAreas.SetAlarmArea(
                        _editingObject.IdAlarmArea,
                        noPrewarning);

                if (result != AlarmAreaActionResult.Success)
                {
                    SetSetAlarmAreaResult(result);
                }
            }
        }

        private void SetSetAlarmAreaResult(AlarmAreaActionResult result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<AlarmAreaActionResult>(SetSetAlarmAreaResult), result);
            }
            else
            {
                if (result == AlarmAreaActionResult.Success)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_SetSucceeded");
                    _eResultOfAction.BackColor = Color.LightGreen;
                }
                else if (result == AlarmAreaActionResult.FailedNoImplicitManager)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_SetFailedNoImplicitManager");
                    _eResultOfAction.BackColor = Color.Yellow;
                }
                else if (result == AlarmAreaActionResult.FailedInputInAlarm)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_SetFailedInputInAlarm");
                    _eResultOfAction.BackColor = Color.Yellow;
                }
                else if (result == AlarmAreaActionResult.FailedCCUOffline)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_SetFailedCCUOffline");
                    _eResultOfAction.BackColor = Color.Red;
                }
                else if (result == AlarmAreaActionResult.SetUnsetNotConfirm)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_SetFailedNotConfirm");
                    _eResultOfAction.BackColor = Color.Red;
                }
                else
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_SetFailed");
                    _eResultOfAction.BackColor = Color.Red;
                }
            }
        }

        private void UnsetAlarmArea(int timeToBuy)
        {
            var result = 
                Plugin.MainServerProvider.AlarmAreas.UnsetAlarmArea(
                    _editingObject.IdAlarmArea, 
                    timeToBuy);

            if (result != AlarmAreaActionResult.Success)
            {
                SetUnsetAlarmAreaResult(result, timeToBuy > 0);
            }
        }

        private void UnsetAlarmArea(out AlarmAreaActionResult result)
        {
            result = 
                Plugin.MainServerProvider.AlarmAreas.UnsetAlarmArea(
                    _editingObject.IdAlarmArea, 
                    0);

            if (result != AlarmAreaActionResult.Success)
            {
                SetUnsetAlarmAreaResult(result, false);
            }
        }

        private void SetUnsetAlarmAreaResult(AlarmAreaActionResult result, bool timeBuyingRequested)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<AlarmAreaActionResult, bool>(SetUnsetAlarmAreaResult), result, timeBuyingRequested);
            }
            else
            {
                switch (result)
                {
                    case AlarmAreaActionResult.Success:
                        if (GetEditingObject().TimeBuyingEnabled && timeBuyingRequested) 
                        {
                            _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetRequested");
                            _eResultOfAction.BackColor = Color.LightSkyBlue;
                        }
                        else
                        {
                            _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetSucceeded");
                            _eResultOfAction.BackColor = Color.LightGreen;
                        }
                        break;

                    case AlarmAreaActionResult.FailedNoImplicitManager:
                        _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetFailedNoImplicitManager");
                        _eResultOfAction.BackColor = Color.Yellow;
                        break;

                    case AlarmAreaActionResult.FailedCCUOffline:
                         _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetFailedCCUOffline");
                        _eResultOfAction.BackColor = Color.Red;
                        break;

                    case AlarmAreaActionResult.SetUnsetNotConfirm:
                        _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetFailedNotConfirm");
                        _eResultOfAction.BackColor = Color.Red;
                        break;

                    case AlarmAreaActionResult.FailedInsufficientRights:
                        _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetFailedInsufficientRights", "\"" + CgpClient.Singleton.LoggedAs + "\"");
                        _eResultOfAction.BackColor = Color.Red;
                        break;

                    case AlarmAreaActionResult.FailedTimeBuyingNotAvaible:
                        _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_FailedTimeBuyingNotAvaible");
                        _eResultOfAction.BackColor = Color.Red;
                        break;

                    default:
                        _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnsetFailed");
                        _eResultOfAction.BackColor = Color.Red;
                        break;
                }
            }
        }

        private void UnconditionalSetAlarmArea(bool noPrewarning)
        {
            var result = 
                Plugin.MainServerProvider.AlarmAreas.UnconditionalSetAlarmArea(
                    _editingObject.IdAlarmArea, 
                    noPrewarning);

            if (result != AlarmAreaActionResult.Success)
            {
                SetUnconditionalSetAlarmAreaResult(result);
            }
        }

        private void SetUnconditionalSetAlarmAreaResult(AlarmAreaActionResult result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<AlarmAreaActionResult>(SetUnconditionalSetAlarmAreaResult), result);
            }
            else
            {
                if (result == AlarmAreaActionResult.Success)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnconditionalSetSucceeded");
                    _eResultOfAction.BackColor = Color.LightGreen;
                }
                else if (result == AlarmAreaActionResult.FailedNoImplicitManager)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnconditionalSetFailedNoImplicitManager");
                    _eResultOfAction.BackColor = Color.Yellow;
                }
                else if (result == AlarmAreaActionResult.FailedCCUOffline)
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnconditionalSetFailedCCUOffline");
                    _eResultOfAction.BackColor = Color.Red;
                }
                else
                {
                    _eResultOfAction.Text = GetString("NCASAlarmAreaEditForm_UnconditionalSetFailed");
                    _eResultOfAction.BackColor = Color.Red;
                }
            }
        }

        protected override void AfterFormEnter()
        {
            if (_tcAlarmArea.SelectedTab.Equals(_tpBasicSettings))
                _tpBaseSettings_Enter(null, null);
        }

        private HashSet<int> _noCriticalSensorsRowsIndexes = new HashSet<int>();

        private void ShowAAInputs(int selectedIndex)
        {
            try
            {
                if (_editingObject.AAInputs != null)
                {
                    _bsAAInputs =
                        new BindingSource
                        {
                            DataSource = GetAAInputsWithActualInputs()
                        };

                    _cdgvDataInputs.DataGrid.DataSource = _bsAAInputs;

                    SetABAlarmRange(_bsAAInputs);

                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMNIDAAINPUT);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMN_ID);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMNINPUT);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMNALARMAREA);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMNGUIDINPUT);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMN_BLOCK_TEMPORARILY_UNTIL);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.COLUMN_SENSOR_PURPOSE);
                    HideColumnDgw(_cdgvDataInputs.DataGrid, AAInput.ColumnVersion);

                    if (_cdgvDataInputs.DataGrid.Columns.Contains(AAInput.COLUMN_SECTION_ID))
                    {
                        _cdgvDataInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.DisplayedCells;

                        _cdgvDataInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].DefaultCellStyle.Alignment =
                            DataGridViewContentAlignment.MiddleRight;

                        _cdgvDataInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].SortMode =
                            DataGridViewColumnSortMode.NotSortable;
                    }

                    if (_cdgvDataInputs.DataGrid.Columns.Contains(AAInput.COLUMNNOCRITICALINPUT))
                    {
                        _cdgvDataInputs.DataGrid.Columns[AAInput.COLUMNNOCRITICALINPUT].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.DisplayedCells;

                        _cdgvDataInputs.DisabledDoubleClickEventForColumns.Add(AAInput.COLUMNNOCRITICALINPUT);
                    }

                    if (_cdgvDataInputs.DataGrid.Columns.Contains(AAInput.COLUMNINPUTNAME))
                        _cdgvDataInputs.DataGrid.Columns[AAInput.COLUMNINPUTNAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (_cdgvDataInputs.DataGrid.Columns.Contains(AAInput.COLUMNINPUTNICKNAME))
                        _cdgvDataInputs.DataGrid.Columns[AAInput.COLUMNINPUTNICKNAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_STR_BLOCK_TEMPORARILY_UNTIL))
                    {
                        _cdgvDataInputs.DataGrid.Columns.Add(
                            new DataGridViewComboBoxColumn
                            {
                                Name = COLUMN_STR_BLOCK_TEMPORARILY_UNTIL,
                                AutoComplete = true,
                                FlatStyle = FlatStyle.System,
                                DataPropertyName = AAInput.COLUMN_BLOCK_TEMPORARILY_UNTIL,
                                DisplayMember = "Name",
                                ValueMember = "BlockTemporarilyUntilType",
                                ValueType = typeof (BlockTemporarilyUntilType?),
                                DataSource = _blockTemporarilyUntilTypeView,
                                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                            });

                        _cdgvDataInputs.DisabledDoubleClickEventForColumns.Add(COLUMN_STR_BLOCK_TEMPORARILY_UNTIL);
                    }

                    if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_SENSOR_PURPOSE))
                    {
                        _cdgvDataInputs.DataGrid.Columns.Add(
                            new DataGridViewComboBoxColumn
                            {
                                Name = COLUMN_SENSOR_PURPOSE,
                                AutoComplete = true,
                                FlatStyle = FlatStyle.System,
                                DataPropertyName = AAInput.COLUMN_SENSOR_PURPOSE,
                                DisplayMember = "Name",
                                ValueMember = "Purpose",
                                ValueType = typeof(SensorPurposeView),
                                DataSource = _sensorPurposeView,
                                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                            });

                        _cdgvDataInputs.DataGrid.Columns[COLUMN_SENSOR_PURPOSE].DefaultCellStyle.NullValue =
                            _sensorPurposeView.First().ToString();
                        _cdgvDataInputs.DisabledDoubleClickEventForColumns.Add(COLUMN_SENSOR_PURPOSE);
                    }

                    if (!Insert)
                    {
                        if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_SENSOR_STATE))
                            _cdgvDataInputs.DataGrid.Columns.Add(
                                new DataGridViewTextBoxColumn
                                {
                                    Name = COLUMN_SENSOR_STATE,
                                    ValueType = typeof(StateView),
                                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                                });

                        if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_BLOCKING_TYPE))
                            _cdgvDataInputs.DataGrid.Columns.Add(
                                new DataGridViewTextBoxColumn
                                {
                                    Name = COLUMN_BLOCKING_TYPE,
                                    ValueType = typeof(BlockingTypeView),
                                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                                });

                        if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_REQUESTED_BLOCKING_TYPE))
                        {
                            _cdgvDataInputs.DataGrid.Columns.Add(
                                new DataGridViewComboBoxColumn
                                {
                                    Name = COLUMN_REQUESTED_BLOCKING_TYPE,
                                    AutoComplete = true,
                                    FlatStyle = FlatStyle.System,
                                    DisplayMember = "Name",
                                    ValueMember = "BlockingType",
                                    ValueType = typeof (SensorBlockingType?),
                                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                                });

                            _cdgvDataInputs.DisabledDoubleClickEventForColumns.Add(COLUMN_REQUESTED_BLOCKING_TYPE);
                        }
                    }

                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvDataInputs.DataGrid);

                    if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_MOVE_UP))
                    {
                        _cdgvDataInputs.DataGrid.Columns.Add(
                            COLUMN_MOVE_UP,
                            string.Empty);

                        _cdgvDataInputs.DataGrid.Columns[COLUMN_MOVE_UP].DisplayIndex = 2;
                        
                        _cdgvDataInputs.DataGrid.Columns[COLUMN_MOVE_UP].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.DisplayedCells;

                        _cdgvDataInputs.AddControlColumn(
                            COLUMN_MOVE_UP,
                            rowIndex =>
                                rowIndex > 0);

                        _cdgvDataInputs.DisabledDoubleClickEventForColumns.Add(COLUMN_MOVE_UP);
                    }

                    if (!_cdgvDataInputs.DataGrid.Columns.Contains(COLUMN_MOVE_DOWN))
                    {
                        _cdgvDataInputs.DataGrid.Columns.Add(
                            COLUMN_MOVE_DOWN,
                            string.Empty);

                        _cdgvDataInputs.DataGrid.Columns[COLUMN_MOVE_DOWN].DisplayIndex = 3;

                        _cdgvDataInputs.DataGrid.Columns[COLUMN_MOVE_DOWN].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.DisplayedCells;

                        _cdgvDataInputs.AddControlColumn(
                            COLUMN_MOVE_DOWN,
                            rowIndex =>
                                rowIndex < _cdgvDataInputs.DataGrid.RowCount - 1);

                        _cdgvDataInputs.DisabledDoubleClickEventForColumns.Add(COLUMN_MOVE_DOWN);
                    }

                    _noCriticalSensorsRowsIndexes.Clear();

                    foreach (DataGridViewRow row in _cdgvDataInputs.DataGrid.Rows)
                    {
                        var aaInput = _bsAAInputs[row.Index] as AAInput;

                        if (aaInput != null)
                        {
                            if (aaInput.NoCriticalInput)
                                _noCriticalSensorsRowsIndexes.Add(row.Index);

                            if (!Insert)
                            {
                                var sensorState = Plugin.MainServerProvider.AlarmAreas.GetSensorState(
                                    _editingObject.IdAlarmArea,
                                    aaInput.Input.IdInput);

                                row.Cells[COLUMN_SENSOR_STATE].Value =
                                    new StateView(sensorState);

                                var blockingType = Plugin.MainServerProvider.AlarmAreas.GetSensorBlockingType(
                                    _editingObject.IdAlarmArea,
                                    aaInput.Input.IdInput);

                                row.Cells[COLUMN_BLOCKING_TYPE].Value =
                                    new BlockingTypeView(blockingType);
                            }
                        }

                        if (row.Index == selectedIndex)
                            row.Selected = true;

                        row.Cells[COLUMN_MOVE_UP] = row.Index > 0
                            ? (DataGridViewCell) new DataGridViewImageCell
                            {
                                Value = ResourceGlobal.show12
                            }
                            : new DataGridViewTextBoxCell();

                        row.Cells[COLUMN_MOVE_DOWN] = row.Index < _cdgvDataInputs.DataGrid.RowCount - 1
                            ? (DataGridViewCell) new DataGridViewImageCell
                            {
                                Value = ResourceGlobal.hide12
                            }
                            : new DataGridViewTextBoxCell();
                    }

                    ShowInfoForNoCriticalSensors();

                    if (!_cdgvDataInputs.DataGrid.Columns.Contains("Empty"))
                    {
                        _cdgvDataInputs.DataGrid.Columns.Add("Empty", string.Empty);
                        _cdgvDataInputs.DataGrid.Columns["Empty"].Width = 35;
                    }
                }
                else
                {
                    _bsAAInputs = null;
                    _cdgvDataInputs.DataGrid.DataSource = null;
                }
            }
            catch
            {
                _bsAAInputs = null;
                _cdgvDataInputs.DataGrid.DataSource = null;
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTable"));
            }

        }

        private void ShowInfoForNoCriticalSensors()
        {
            if (_noCriticalSensorsRowsIndexes.Count == 0)
            {
                if (_cdgvDataInputs.DataGrid.Columns.Contains("Icon"))
                    _cdgvDataInputs.DataGrid.Columns.Remove("Icon");

                return;
            }

            if (!_cdgvDataInputs.DataGrid.Columns.Contains("Icon"))
            {
                _cdgvDataInputs.DataGrid.Columns.Add(
                    "Icon",
                    string.Empty);

                _cdgvDataInputs.DataGrid.Columns["Icon"].DisplayIndex =
                    _cdgvDataInputs.DataGrid.Columns[COLUMN_STR_BLOCK_TEMPORARILY_UNTIL].DisplayIndex - 1;

                _cdgvDataInputs.DataGrid.Columns["Icon"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.DisplayedCells;
            }

            foreach (DataGridViewRow row in _cdgvDataInputs.DataGrid.Rows)
            {
                if (_noCriticalSensorsRowsIndexes.Contains(row.Index))
                {
                    row.Cells["Icon"] =
                        new DataGridViewImageCell
                        {
                            Value = ResourceGlobal.information,
                            ToolTipText = LocalizationHelper.GetString(
                                "InfoSensorTemporarilyBlockingOnlyInSabotage")
                        };
                }
                else
                {
                    row.Cells["Icon"] =
                        new DataGridViewTextBoxCell
                        {
                            ToolTipText = string.Empty
                        };
                }
            }
        }

        private void SetABAlarmRange(BindingSource _bsAAInputs)
        {
            if (_bsAAInputs == null || _bsAAInputs.Count == 0)
            {
                _chbABAlarmHandling.Checked = false;
                _chbABAlarmHandling.Enabled = false;
            }
            else
            {
                _chbABAlarmHandling.Enabled = true;
                if (_tbPercentageSensorsToAAlarm.Value > _bsAAInputs.Count)
                    _tbPercentageSensorsToAAlarm.Value = _bsAAInputs.Count;
                _tbPercentageSensorsToAAlarm.Maximum = _bsAAInputs.Count;
            }

        }

        private ICollection<AAInput> GetAAInputsWithActualInputs()
        {
            var result = _editingObject.AAInputs.OrderBy(aaInput => aaInput.Id).ToList();

            foreach (var aaInput in result)
            {
                aaInput.Input = Plugin.MainServerProvider.Inputs.GetObjectById(aaInput.Input.IdInput);
            }

            return result.ToArray();
        }

        private void _bInsert2_Click(bool lowSecurityInput, byte blockTemporarilyUntil, SensorPurpose? sensorPurpose)
        {
            IList<AAInput> aaInputs;

            if (!GetValuesAAInput(out aaInputs, lowSecurityInput, blockTemporarilyUntil, sensorPurpose))
                return;

            if (_editingObject.ObjForAutomaticAct != null)
                foreach (var input in aaInputs)
                {
                    if (input.Input != null 
                        && input.Input.IdInput == _editingObject.ObjForAutomaticActId)
                    {
                        Dialog.Error(input.Input.ToString(), GetString("ErrorInputAlreadyUsedAsObjForAutAct"));
                        return;
                    }
                }

            if (CgpClient.Singleton.MainServerProvider == null ||
                CgpClient.Singleton.IsConnectionLost(false))
            {
                return;
            }

            var currentState =
                Plugin.MainServerProvider
                    .AlarmAreas
                    .GetAlarmAreaActivationState(_editingObject.IdAlarmArea);

            if (currentState == ActivationState.Set &&
                !_dialogConfirmed &&
                !Dialog.Question(
                     LocalizationHelper.GetString("NCASAlarmAreaEditForm_QuestionConfirmUnsetAA")))
            {
                return;
            }

            _inputInserted = true;

            if (currentState == ActivationState.Set)
                _dialogConfirmed = true;

            if (aaInputs != null)
            {
                if (_editingObject.AAInputs == null)
                    _editingObject.AAInputs = new List<AAInput>();

                foreach (var aaInput in aaInputs)
                {
                    var addInput = true;
                    if (!NCASClient.INTER_CCU_COMMUNICATION && _guidImplicitCCU != Guid.Empty)
                    {
                        var input = aaInput.Input;
                        if (input != null)
                        {
                            CCU inputCCU = null;
                            if (input.DCU != null)
                            {
                                inputCCU = input.DCU.CCU;
                            }
                            else if (input.CCU != null)
                            {
                                inputCCU = input.CCU;
                            }

                            if (inputCCU == null)
                            {
                                addInput = false;
                            }
                            else
                            {
                                if (inputCCU.IdCCU != _guidImplicitCCU)
                                    addInput = false;
                            }
                        }
                    }

                    if (addInput)
                    {
                        _editingObject.AAInputs.Add(aaInput);
                        RefreshImplictManager();
                    }
                }
            }

            ShowAAInputs(_cdgvDataInputs.DataGrid.RowCount);
            EditTextChanger(null, null);
        }

        private void GetValuesAAInput(AAInput aaInput, Input input)
        {
            aaInput.AlarmArea = _editingObject;
            aaInput.Input = input;
            aaInput.NoCriticalInput = false;
        }

        private bool GetValuesAAInput(
            out IList<AAInput> aaInputs, 
            bool lowSecurityInput, 
            byte blockTemporarilyUntil,
            SensorPurpose? sensorPurpose)
        {
            aaInputs = null;

            if (CheckValuesAAInput())
            {
                try
                {
                    aaInputs = new List<AAInput>();
                    foreach (var obj in _actInputObjects)
                    {
                        var aaInput = new AAInput();

                        aaInput.Id = aaInputs.Count
                                     + (_editingObject.AAInputs != null
                                         ? _editingObject.AAInputs.Count + 1
                                         : 1);
                        
                        GetValuesAAInput(aaInput, obj as Input);
                        aaInput.NoCriticalInput = lowSecurityInput;
                        aaInput.BlockTemporarilyUntil = blockTemporarilyUntil;
                        aaInput.Purpose = sensorPurpose;
                        aaInputs.Add(aaInput);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private bool CheckValuesAAInput()
        {
            if (_actInputObjects == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _cdgvDataInputs,
                    GetString("ErrorEntryAreaInput"),
                    ControlNotificationSettings.Default);

                return false;
            }

            if (_editingObject.AAInputs == null)
                return true;

            foreach (var actAAInput in _editingObject.AAInputs)
                foreach (var obj in _actInputObjects.Objects)
                    if (actAAInput.Input != null &&
                        actAAInput.Input.Compare(obj))
                    {
                        Dialog.Error(GetString("ErrorAreaInputAlreadyInAlarmArea"));
                        return false;
                    }

            return true;
        }

        bool _inputInserted;
        bool _inputModified;
        bool _dialogConfirmed;

        private void _bDelete2_Click(object sender, EventArgs e)
        {
            if (_bsAAInputs == null || _bsAAInputs.Count == 0) return;

            if (Dialog.Question(CgpClient.Singleton.LocalizationHelper.GetString("QuestionDeleteConfirm")))
            {
                var aaInput = (AAInput)_bsAAInputs.List[_bsAAInputs.Position];

                if (aaInput == null)
                    return;

                _editingObject.AAInputs.Remove(aaInput);

                foreach (var actAaInput in _editingObject.AAInputs)
                {
                    if (actAaInput.Id > aaInput.Id)
                        actAaInput.Id--;
                }

                RefreshImplictManager();

                ShowAAInputs(-1);
                EditTextChanger(null, null);
            }
        }

        private bool CheckInputBeforeAdding(
            Input input,
            out string errorMessage)
        {
            if (!NCASClient.INTER_CCU_COMMUNICATION)
            {
                if (!Plugin.MainServerProvider.AlarmAreas.SetSensorToAlarmArea(input.IdInput, _guidImplicitCCU))
                {
                    errorMessage = GetString("InterCCUCommunicationNotEnabled");
                    return false;
                }
            }

            if (_editingObject.AAInputs == null)
            {
                errorMessage = null;
                return true;
            }

            if (_editingObject.AAInputs.Any(
                actAaInput =>
                    actAaInput.Input != null
                    && actAaInput.Input.Compare(input)))
            {
                errorMessage = GetString("ErrorAreaInputAlreadyInList");
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void _dgValues2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgValues2_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddAAInput(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddAAInput(object newInput)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            try
            {
                if (newInput.GetType() == typeof(Input))
                {
                    var input = newInput as Input;

                    if (input != null)
                    {
                        string errorMessage;

                        if (!CheckInputBeforeAdding(
                            input,
                            out errorMessage))
                        {
                            ControlNotification.Singleton.Error(
                                NotificationPriority.JustOne,
                                _cdgvDataInputs.DataGrid,
                               errorMessage,
                               ControlNotificationSettings.Default);

                            return;
                        }

                        if (_editingObject.AAInputs == null)
                            _editingObject.AAInputs = new List<AAInput>();

                        var aaInput =
                            new AAInput
                            {
                                Id = _editingObject.AAInputs != null
                                    ? _editingObject.AAInputs.Count + 1
                                    : 1,

                                AlarmArea = _editingObject,
                                Input = input,
                                NoCriticalInput = false
                            };

                        RefreshImplictManager();
                        _editingObject.AAInputs.Add(aaInput);
                        ShowAAInputs(_cdgvDataInputs.DataGrid.RowCount);
                        EditTextChanger(null, null);
                        Plugin.AddToRecentList(newInput);
                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvDataInputs.DataGrid,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            {
                Dialog.Error(GetString("ErrorAddAACardReader"));
            }
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            SetSettingsForEventlog();
            if (Apply_Click())
            {
                ResetWasChangedValues();
                _bApply.Enabled = false;
                _dialogConfirmed = false;
                _inputInserted = false;
                _inputModified = false;

                var aa = GetEditingObject();

                _cbNoPrewarning.Checked = false;
                _cbNoPrewarning.Visible = aa.PreWarning && _activationState == ActivationState.Unset;

                _bAlarmAreaSetUnset.Enabled = false;
                SafeThread.StartThread(EnableSetButtonTimer);
                ShowAAInputs(-1);
                ShowAACardReaders();
 
                UpdateTimeBuyingControls(true);
                CheckTmpUnsetEntryExitFunctionality();
            }
        }

        private void EnableSetButtonTimer()
        {
            Thread.Sleep(3000);
            EnableSetButton();
        }

        private void EnableSetButton()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(EnableSetButton));
            }
            else
            {
                var aa = GetEditingObject();
                _bAlarmAreaSetUnset.Enabled =
                            Plugin
                                .MainServerProvider
                                .AlarmAreas
                                .HasAccessForAlarmAreaActionFromCurrentLogin(
                                    AccessNCAS.AlarmAreasSetPerform,
                                    aa.IdAlarmArea)
                            || Plugin
                                .MainServerProvider
                                .AlarmAreas
                                .HasAccessForAlarmAreaActionFromCurrentLogin(
                                    AccessNCAS.AlarmAreasUnconditionalSetPerform,
                                    aa.IdAlarmArea);
                
                UpdateTimeBuyingControls(false);
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(
                new ReferencedByForm(
                    GetListReferencedObjects,
                    NCASClient.LocalizationHelper,
                    ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return
                Plugin.MainServerProvider.AlarmAreas
                    .GetReferencedObjects(
                        _editingObject.IdAlarmArea,
                        CgpClient.Singleton.GetListLoadedPlugins());
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

        private void _tbmObjAutomaticAct_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify3")
            {
                ModifyObject(ref _actObjAutomaticAct, _tbmObjAutomaticAct, _lICCU);
            }
            else if (item.Name == "_tsiRemove3")
            {
                _actObjAutomaticAct = null;
                RefreshOnOffObject(_actObjAutomaticAct, _tbmObjAutomaticAct, _lICCU);
            }
        }

        private void _tbmObjForcedTimeBuying_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify16")
            {
                ModifyObject(ref _actObjForcedTimeBuying, _tbmObjForcedTimeBuying, _lICCU1);
            }
            else if (item.Name == "_tsiRemove16")
            {
                _actObjForcedTimeBuying = null;
                RefreshOnOffObject(_actObjForcedTimeBuying, _tbmObjForcedTimeBuying, _lICCU1);
            }
        }

        private IList<IModifyObject> GetAvailableOutputs()
        {
            var outputs = Plugin.MainServerProvider.AlarmAreas.GetSpecialOutputs(_editingObject.IdAlarmArea);

            IList<IModifyObject> outputsModifyObjects = new List<IModifyObject>();

            if (outputs == null)
                return outputsModifyObjects;

            foreach (var output in outputs)
            {
                if (output == null)
                    continue;

                if (_eisSetUnsetOutput != null &&
                    _eisSetUnsetOutput.IdOutput == output.IdOutput)
                {
                    continue;
                }

                outputsModifyObjects.Add(new OutputModifyObj(output));
            }

            return outputsModifyObjects;
        }

        private void ModifyOutputAlarm(byte type)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            var formModify = new ListboxFormAdd(GetAvailableOutputs(), GetString("ModifyOutput"));
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


        private Output _outputActivation;
        private Output _outputAlarmState;
        private Output _outputSabotageState;
        private Output _outputPrewarning;
        private Output _outputTmpUnsetEntry;
        private Output _outputTmpUnsetExit;
        private Output _outputAAlarm;
        private Output _outputSiren;
        private Output _outputNotAcknowledged;
        private Output _outputMotion;


        private void SetSpecialOutput(byte type, Output value)
        {
            switch (type)
            {
                case 4:
                    {
                        _outputActivation = value;
                        _tbmOutputActivation.Text = value == null ? string.Empty : value.ToString();
                        _tbmOutputActivation.TextImage = Plugin.GetImageForAOrmObject(value);
                        //return _tbmOutputActivation;
                    }
                    break;
                case 5:
                    {
                        _outputAlarmState = value;
                        _tbmOutputAlarmState.Text = value == null ? string.Empty : value.ToString();
                        _tbmOutputAlarmState.TextImage = Plugin.GetImageForAOrmObject(value);
                        //return _tbmOutputAlarmState;
                    }
                    break;
                case 6:
                    {
                        _outputPrewarning = value;
                        _tbmOutputPrewarning.Text = value == null ? string.Empty : value.ToString();
                        _tbmOutputPrewarning.TextImage = Plugin.GetImageForAOrmObject(value);
                        //return _tbmOutputPrewarning;
                    }
                    break;
                case 7:
                    {
                        _outputTmpUnsetEntry = value;
                        _tbmOutputTmpUnsetEntry.Text = value == null ? string.Empty : value.ToString();
                        _tbmOutputTmpUnsetEntry.TextImage = Plugin.GetImageForAOrmObject(value);
                        //return _tbmOutputTmpUnsetEntry;
                    }
                    break;
                case 8:
                    {
                        _outputTmpUnsetExit = value;
                        _tbmOutputTmpUnsetExit.Text = value == null ? string.Empty : value.ToString();
                        _tbmOutputTmpUnsetExit.TextImage = Plugin.GetImageForAOrmObject(value);
                        //return _tbmOutputTmpUnsetExit;
                    }
                    break;
                case 9:
                    {
                        _outputAAlarm = value;
                        _tbmOutputAAlarm.Text = value == null ? string.Empty : value.ToString();
                        _tbmOutputAAlarm.TextImage = Plugin.GetImageForAOrmObject(value);
                        //return _tbmOutputAAlarm;
                    }
                    break;
                case 10:
                    _outputSiren = value;
                    _tbmOutputSiren.Text = value == null ? string.Empty : value.ToString();
                    _tbmOutputSiren.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
                case 13:
                    _outputSabotageState = value;
                    _tbmOutputSabotage.Text = value == null ? string.Empty : value.ToString();
                    _tbmOutputSabotage.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
                case 14:
                    _outputNotAcknowledged = value;
                    _tbmOutputNotAcknowledged.Text = value == null ? string.Empty : value.ToString();
                    _tbmOutputNotAcknowledged.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
                case 15:
                    _outputMotion = value;
                    _tbmOutputMotion.Text = value == null ? string.Empty : value.ToString();
                    _tbmOutputMotion.TextImage = Plugin.GetImageForAOrmObject(value);
                    break;
            }
        }

        private void RemoveOutputAlarm(byte type)
        {
            SetSpecialOutput(type, null);
        }


        private void ItemClicked(ToolStripItem item, int index)
        {
            if (item.Name.Substring(4, 6).Equals("Modify"))
            {
                ModifyOutputAlarm(Convert.ToByte(item.Name.Substring(10, item.Name.Length - 10)));
            }
            else
            {
                RemoveOutputAlarm(Convert.ToByte(item.Name.Substring(10, item.Name.Length - 10)));
            }
        }


        private void _tbmOutputActivation_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 4);
            }
            catch
            {
            }
        }

        private void _tbmOutputAlarmState_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 5);
            }
            catch
            {
            }
        }

        private void _tbmOutputPrewarning_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 6);
            }
            catch
            {
            }
        }

        private void _tbmOutputTmpUnsetEntry_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 7);
            }
            catch
            {
            }
        }

        private void _tbmOutputTmpUnsetExit_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 8);
            }
            catch
            {
            }
        }

        private void _tbmOutputAAlarm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 9);
            }
            catch
            {
            }
        }

        private void AddOutputAlarm(object sender, object newOutput, byte type)
        {
            try
            {
                TextBoxMenu textBoxMenu =
                    sender is TextBoxMenu
                        ? sender as TextBoxMenu
                        : _tbmOutputActivation;

                var output = newOutput as Output;

                if (output == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        textBoxMenu.ImageTextBox,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (!CheckOutputAvailability(output, GetAvailableOutputs()))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        textBoxMenu.ImageTextBox,
                        GetString("ErrorOutputAlreadyUsed"),
                        ControlNotificationSettings.Default);

                    return;
                }

                SetSpecialOutput(type, output);
                Plugin.AddToRecentList(newOutput);
            }
            catch
            { }
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

        private void AlarmOutputsDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmOutputActivation_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputActivation != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputActivation);
            }
        }

        private void _tbmOutputAlarmState_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputAlarmState != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputAlarmState);
            }
        }

        private void _tbmOutputPrewarning_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputPrewarning != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputPrewarning);
            }
        }

        private void _tbmOutputTmpUnsetEntry_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputTmpUnsetEntry != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputTmpUnsetEntry);
            }
        }

        private void _tbmOutputTmpUnsetExit_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputTmpUnsetExit != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputTmpUnsetExit);
            }
        }

        private void _tbmOutputAAlarm_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputAAlarm != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputAAlarm);
            }
        }

        private void CheckTmpUnsetEntryExitFunctionality()
        {
            if ((!_chbUseEIS.Checked)
                && (_cbPrealamOn.Checked
                    || _eTemporaryUnsetDuration.Value.Minute > 0
                    || _eTemporaryUnsetDuration.Value.Second > 0)
                && AlarmAreaHasNoCriticalInput())
            {
                _pbWarningNoEntrySensor.Visible = true;
                _pbWarningNoExitSensor.Visible = true;
            }
            else
            {
                _pbWarningNoEntrySensor.Visible = false;
                _pbWarningNoExitSensor.Visible = false;
            }
        }

        private bool AlarmAreaHasNoCriticalInput()
        {
            if (_editingObject == null) return false;

            if (_editingObject.AAInputs == null) return true;

            foreach (var input in _editingObject.AAInputs)
            {
                if (input.NoCriticalInput)
                {
                    return false;
                }
            }
            return true;
        }

        private void _chbInheritGeneralAlarmSetting_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (_chbAllowAAtoCRsReporting.CheckState == CheckState.Indeterminate)
                _editingObject.AllowSendingStateToCRs = null;
            else
                _editingObject.AllowSendingStateToCRs = _chbAllowAAtoCRsReporting.Checked;
        }

        private void _dgValues2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_cdgvDataInputs.DataGrid.ReadOnly || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (!(_bsAAInputs.Current is AAInput))
                return;

            if (_cdgvDataInputs.DataGrid.Columns[e.ColumnIndex].Name == AAInput.COLUMNNOCRITICALINPUT)
            {
                var input = (_bsAAInputs.Current as AAInput);

                var currentState = Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(_editingObject.IdAlarmArea);
                if (currentState != ActivationState.Set || _dialogConfirmed
                    || Dialog.Question(LocalizationHelper.GetString("NCASAlarmAreaEditForm_QuestionConfirmUnsetAA")))
                {
                    if (currentState == ActivationState.Set)
                        _dialogConfirmed = true;
                    _inputModified = true;
                    input.NoCriticalInput = !input.NoCriticalInput;

                    if (input.NoCriticalInput)
                    {
                        if (_noCriticalSensorsRowsIndexes.Add(e.RowIndex))
                            ShowInfoForNoCriticalSensors();
                    }
                    else
                    {
                        if (_noCriticalSensorsRowsIndexes.Remove(e.RowIndex))
                            ShowInfoForNoCriticalSensors();
                    }

                    _cdgvDataInputs.DataGrid.Refresh();
                    EditTextChanger(null, null);
                }

                return;
            }

            if (_cdgvDataInputs.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_STR_BLOCK_TEMPORARILY_UNTIL)
            {
                _cdgvDataInputs.DataGrid.BeginEdit(true);
                ((ComboBox)_cdgvDataInputs.DataGrid.EditingControl).DroppedDown = true;

                EditTextChanger(null, null);
                return;
            }

            if (_cdgvDataInputs.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_SENSOR_PURPOSE)
            {
                _cdgvDataInputs.DataGrid.BeginEdit(true);
                ((ComboBox)_cdgvDataInputs.DataGrid.EditingControl).DroppedDown = true;

                EditTextChanger(null, null);
                return;
            }

            if (_cdgvDataInputs.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_REQUESTED_BLOCKING_TYPE)
            {
                var cellRequestedBlockingType =
                    (DataGridViewComboBoxCell) _cdgvDataInputs.DataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cellRequestedBlockingType.IsInEditMode)
                    return;

                var lastRequestedBlockingType =
                    cellRequestedBlockingType.Value as SensorBlockingType?;

                var blockingTypeView =
                    _cdgvDataInputs.DataGrid.Rows[e.RowIndex].Cells[COLUMN_BLOCKING_TYPE].Value as BlockingTypeView;

                if (blockingTypeView == null
                    || blockingTypeView.BlockingType == null)
                {
                    return;
                }

                cellRequestedBlockingType.DataSource =
                    GetDataSourceForSensorRequestedBlockingType(
                        lastRequestedBlockingType,
                        blockingTypeView.BlockingType.Value);

                _cdgvDataInputs.DataGrid.BeginEdit(true);
                ((ComboBox)_cdgvDataInputs.DataGrid.EditingControl).DroppedDown = true;

                return;
            }

            if (_cdgvDataInputs.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_MOVE_UP)
            {
                if (e.RowIndex < 1
                    || !_cdgvDataInputs.AllowedActionButtonClick)
                {
                    return;
                }

                if (_editingObject.AAInputs == null)
                    return;

                foreach (var aaInput in _editingObject.AAInputs)
                {
                    if (aaInput.Id == e.RowIndex + 1)
                        aaInput.Id--;
                    else if (aaInput.Id == e.RowIndex)
                        aaInput.Id++;
                }

                ShowAAInputs(e.RowIndex - 1);
                EditTextChanger(null, null);

                return;
            }

            if (_cdgvDataInputs.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_MOVE_DOWN)
            {
                if (e.RowIndex < 0
                    || e.RowIndex == _cdgvDataInputs.DataGrid.RowCount - 1
                    || !_cdgvDataInputs.AllowedActionButtonClick)
                {
                    return;
                }

                if (_editingObject.AAInputs == null)
                    return;

                foreach (var aaInput in _editingObject.AAInputs)
                {
                    if (aaInput.Id == e.RowIndex + 1)
                        aaInput.Id++;
                    else if (aaInput.Id == e.RowIndex + 2)
                        aaInput.Id--;
                }

                ShowAAInputs(e.RowIndex + 1);
                EditTextChanger(null, null);
            }
        }

        void DataGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_cdgvDataInputs.DataGrid.IsCurrentCellDirty)
            {
                _cdgvDataInputs.DataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                _cdgvDataInputs.DataGrid.EndEdit();

                if (_cdgvDataInputs.DataGrid.CurrentCell.ColumnIndex ==
                    _cdgvDataInputs.DataGrid.Columns[COLUMN_REQUESTED_BLOCKING_TYPE].Index)
                {
                    SafeThread<AAInput, SensorBlockingType?>.StartThread(
                        SetAlarmAreaSensorBlockingType,
                        _bsAAInputs.Current as AAInput,
                        _cdgvDataInputs.DataGrid.CurrentCell.Value as SensorBlockingType?);
                }
            }
        }

        private void SetAlarmAreaSensorBlockingType(
            AAInput aaInput,
            SensorBlockingType? blockingType)
        {
            if (blockingType == null)
                return;

            if (aaInput == null)
                return;

            Plugin.MainServerProvider.AlarmAreas.SetAlarmAreaSensorBlockingType(
                _editingObject.IdAlarmArea,
                aaInput.Input.IdInput,
                blockingType.Value);
        }

        private List<BlockingTypeView> GetDataSourceForSensorRequestedBlockingType(
            SensorBlockingType? lastRequestedBlockingType,
            SensorBlockingType blockingType)
        {
            var alarmAreaIsUnset = ActivationStateIsUnsetOrUnknown();

            var dataSource = new List<BlockingTypeView>();

            if (lastRequestedBlockingType == null)
                dataSource.Add(new BlockingTypeView(null));

            if (blockingType != SensorBlockingType.Unblocked)
                dataSource.Add(new BlockingTypeView(SensorBlockingType.Unblocked));

            if (!alarmAreaIsUnset
                && blockingType != SensorBlockingType.BlockTemporarilyUntilSensorStateNormal)
            {
                dataSource.Add(new BlockingTypeView(SensorBlockingType.BlockTemporarilyUntilSensorStateNormal));
            }

            if (!alarmAreaIsUnset
                && blockingType != SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset)
            {
                dataSource.Add(new BlockingTypeView(SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset));
            }

            if (blockingType != SensorBlockingType.BlockPermanently)
                dataSource.Add(new BlockingTypeView(SensorBlockingType.BlockPermanently));

            return dataSource;
        }

        private void _cdgvData2_OpenEdit()
        {
            if (_cdgvDataInputs.CurrentRowIndex < 0)
                return;

            var aaInput = _bsAAInputs.Current as AAInput;

            if (aaInput == null)
                return;

            NCASInputsForm.Singleton.OpenEditForm(aaInput.Input);
        }

        private void _dgValues_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_cdgvDataCRs.DataGrid.ReadOnly || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var aaVisualCardReader = _bsVisualAACardReaders.Current as AACardReader;

            if (aaVisualCardReader == null)
                return;

            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            var aaCardReader = GetFromVisualCR(aaVisualCardReader);

            switch (_cdgvDataCRs.DataGrid.Columns[e.ColumnIndex].Name)
            {
                case AACardReader.COLUMNAASET:

                    aaCardReader.AASet = !aaCardReader.AASet;
                    aaVisualCardReader.AASet = aaCardReader.AASet;

                    EditTextChanger(null, null);

                    break;

                case AACardReader.COLUMNAAUNSET:

                    aaCardReader.AAUnset = !aaCardReader.AAUnset;
                    aaVisualCardReader.AAUnset = aaCardReader.AAUnset;

                    EditTextChanger(null, null);

                    break;

                case AACardReader.COLUMNAAUNCONDITIONALSET:

                    aaCardReader.AAUnconditionalSet = !aaCardReader.AAUnconditionalSet;
                    aaVisualCardReader.AAUnconditionalSet = aaCardReader.AAUnconditionalSet;

                    EditTextChanger(null, null);

                    break;

                case AACardReader.COLUMNENABLEEVENTLOG:

                    aaCardReader.EnableEventlog = !aaCardReader.EnableEventlog;
                    aaVisualCardReader.EnableEventlog = aaCardReader.EnableEventlog;

                    EditTextChanger(null, null);

                    break;

                case AACardReader.COLUMNPERMANENTLYUNLOCK:

                    if (aaCardReader.PermanentlyUnlock)
                    {
                        var alarmArea = Plugin.MainServerProvider.AACardReaders.GetImplicitAlarmArea(aaCardReader.CardReader);
                        if (alarmArea != null && !_editingObject.Compare(alarmArea))
                        {
                            if (!Dialog.WarningQuestion(GetString("QuestionChangeImplicitAlarmAreaForCardReader", alarmArea.Name)))
                            {
                                return;
                            }
                        }
                    }

                    aaCardReader.PermanentlyUnlock = !aaCardReader.PermanentlyUnlock;
                    aaVisualCardReader.PermanentlyUnlock = !aaCardReader.PermanentlyUnlock;

                    EditTextChanger(null, null);

                    break;
            }
        }

        private void _cdgvData1_OpenEdit()
        {
            if (_cdgvDataCRs.CurrentRowIndex < 0)
                return;

            var aaCr = _bsVisualAACardReaders.Current as AACardReader;

            if (aaCr == null)
                return;

            NCASCardReadersForm.Singleton.OpenEditForm(aaCr.CardReader);
        }

        private void _bAlarmAreaSetUnset_Click(object sender, EventArgs e)
        {
            _eResultOfAction.Text = string.Empty;
            _eResultOfAction.BackColor = _baseColorForResultOfAction;

            if (_activationState == ActivationState.Unset)
            {
                if (_chbUnconditional.Checked)
                {
                    SafeThread<bool>.StartThread(UnconditionalSetAlarmArea, _cbNoPrewarning.Checked);
                }
                else
                {
                    SafeThread<bool>.StartThread(SetAlarmArea, _cbNoPrewarning.Checked);
                }
            }
            else
            {
                SafeThread<int>.StartThread(UnsetAlarmArea, 0);
            }
        }

        private void _bAlarmAreaBuyTime_Click(object sender, EventArgs e)
        {
            _eResultOfAction.Text = string.Empty;
            _eResultOfAction.BackColor = _baseColorForResultOfAction;

            if (GetEditingObject().TimeBuyingEnabled)
            {
                int timeToBuy = _eBuyTime.Value.Hour * 3600 + _eBuyTime.Value.Minute * 60 + _eBuyTime.Value.Second;

                if (timeToBuy <= 0)
                {
                    _eResultOfAction.Text = LocalizationHelper.GetString("NCASAlarmAreaEditFormEnterAmountOfTimeToBuy");
                    _eResultOfAction.BackColor = Color.Red;
                }
                else
                {
                    SafeThread<int>.StartThread(UnsetAlarmArea, timeToBuy);
                }
            }

        }

        private void _eName_TextChanged(object sender, EventArgs e)
        {
            if (_eShortName.Text == string.Empty)
                EditTextChanger(sender, e);
            else
                EditTextChangerOnlyInDatabase(sender, e);
        }

        private void _itbImplicitManager_DoubleClick(object sender, EventArgs e)
        {
            if (_guidImplicitCCU != Guid.Empty)
            {
                var implicitCCU = Plugin.MainServerProvider.CCUs.GetObjectById(_guidImplicitCCU);
                if (implicitCCU != null)
                    NCASCCUsForm.Singleton.OpenEditForm(implicitCCU);
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.AlarmAreasLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.AlarmAreasLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void _tbmOutputSiren_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 10);
            }
            catch { }
        }

        private void _tbmOutputSiren_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputSiren != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputSiren);
            }
        }

        private void RefreshEISInputActivationState()
        {
            if (_eisInputActivationState != null)
            {
                _tbmEISInputActivatianState.Text = _eisInputActivationState.ToString();
                _tbmEISInputActivatianState.TextImage = Plugin.GetImageForAOrmObject(_eisInputActivationState);
            }
            else
            {
                _tbmEISInputActivatianState.Text = string.Empty;
            }
        }

        private void RefreshEISSetUnsetOutput()
        {
            if (_eisSetUnsetOutput != null)
            {
                _tbmEISSetUnsetOutput.Text = _eisSetUnsetOutput.ToString();
                _tbmEISSetUnsetOutput.TextImage = Plugin.GetImageForAOrmObject(_eisSetUnsetOutput);
            }
            else
            {
                _tbmEISSetUnsetOutput.Text = string.Empty;
            }
        }

        private void _tbmEISInputActivatianState_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify11")
            {
                ModifyEISInputActivatianState();
            }
            else
            {
                RmoveEISInputActivatianState();
            }
        }

        private void ModifyEISInputActivatianState()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            try
            {
                Exception error;
                var listModObj = Plugin.MainServerProvider.Inputs.ListModifyObjectsFromCCUNotUsedInDoorEnvironments(_guidImplicitCCU, _editingObject.IdAlarmArea, out error);
                if (error != null) throw error;

                var formAdd = new ListboxFormAdd(listModObj, GetString("NCASInputsFormNCASInputsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var eisInputActivationState = Plugin.MainServerProvider.Inputs.GetObjectById(outModObj.GetId);

                    if (eisInputActivationState != null)
                    {
                        SetEISInputActivatianState(eisInputActivationState, false);
                        Plugin.AddToRecentList(eisInputActivationState);
                    }
                }
            }
            catch
            {
            }
        }

        private void RmoveEISInputActivatianState()
        {
            SetEISInputActivatianState(null, false);
        }

        private void SetEISInputActivatianState(Input eisInputActivationState, bool checkInput)
        {
            if (checkInput && eisInputActivationState != null)
            {
                if (!NCASClient.INTER_CCU_COMMUNICATION && _guidImplicitCCU != Guid.Empty)
                {
                    var inputCCUId = Guid.Empty;

                    if (eisInputActivationState.CCU != null)
                    {
                        inputCCUId = eisInputActivationState.CCU.IdCCU;
                    }
                    else if (eisInputActivationState.DCU != null && eisInputActivationState.DCU.CCU != null)
                    {
                        inputCCUId = eisInputActivationState.DCU.CCU.IdCCU;
                    }

                    if (inputCCUId != _guidImplicitCCU)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmEISInputActivatianState.ImageTextBox,
                            GetString("ErrorInputMustBeFromImplicitManager"), ControlNotificationSettings.Default);

                        return;
                    }
                }

                var isCorrectInput = false;
                Exception error;
                var listModObj = Plugin.MainServerProvider.Inputs.ListModifyObjectsFromCCUNotUsedInDoorEnvironments(_guidImplicitCCU, _editingObject.IdAlarmArea, out error);
                if (listModObj != null && listModObj.Count > 0)
                {
                    foreach (var iModifyObject in listModObj)
                    {
                        if (iModifyObject != null && iModifyObject.GetId is Guid && (Guid)iModifyObject.GetId == eisInputActivationState.IdInput)
                        {
                            isCorrectInput = true;
                            break;
                        }
                    }
                }

                if (!isCorrectInput)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmEISInputActivatianState.ImageTextBox,
                        GetString("ErrorInputAlreadyUsed"), ControlNotificationSettings.Default);

                    return;
                }
            }

            _eisInputActivationState = eisInputActivationState;
            RefreshEISInputActivationState();
            RefreshImplictManager();
            EditTextChanger(null, null);
        }

        private void _tbmEISInputActivatianState_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmEISInputActivatianState_DragDrop(object sender, DragEventArgs e)
        {
            var output = e.Data.GetFormats();
            if (output == null) return;

            var obj = e.Data.GetData(output[0]);

            var input = obj as Input;

            if (input != null)
                SetEISInputActivatianState(input, true);
            else
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmEISInputActivatianState.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);
        }

        private void _tbmEISInputActivatianState_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_eisInputActivationState != null)
            {
                NCASInputsForm.Singleton.OpenEditForm(_eisInputActivationState);
            }
        }

        private void _tbmEISSetUnsetOutput_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify12")
            {
                ModifyEISSetUnsetOutput();
            }
            else
            {
                RmoveEISSetUnsetOutput();
            }
        }

        private void ModifyEISSetUnsetOutput()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            try
            {
                var outputs = GetOutputsForEisSetUnsetOutput();

                if (outputs != null)
                {
                    var formAdd = new ListboxFormAdd(outputs, GetString("NCASOutputsFormNCASOutputsForm"));
                    IModifyObject outModObj;
                    formAdd.ShowDialog(out outModObj);
                    if (outModObj != null)
                    {
                        var eisSetUnsetOutput = Plugin.MainServerProvider.Outputs.GetObjectById(outModObj.GetId);

                        if (eisSetUnsetOutput != null)
                        {
                            SetEISSetUnsetOutput(eisSetUnsetOutput, false);
                            Plugin.AddToRecentList(eisSetUnsetOutput);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private List<IModifyObject> GetOutputsForEisSetUnsetOutput()
        {
            Exception error;
            var listModObj =
                Plugin.MainServerProvider.Outputs
                    .ListModifyObjectsFromCCUNotUsedInDoorEnvironmentsAnaAlarmAreas(_guidImplicitCCU,
                        _editingObject.IdAlarmArea, out error);

            if (listModObj != null)
            {
                var outputs = new List<IModifyObject>();
                foreach (var iModifyObject in listModObj)
                {
                    if (iModifyObject != null && iModifyObject.GetId is Guid)
                    {
                        if (_outputAAlarm != null && _outputAAlarm.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputActivation != null && _outputActivation.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputAlarmState != null && _outputAlarmState.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputSabotageState != null && _outputSabotageState.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputPrewarning != null && _outputPrewarning.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputSiren != null && _outputSiren.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputTmpUnsetEntry != null && _outputTmpUnsetEntry.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputTmpUnsetExit != null && _outputTmpUnsetExit.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputNotAcknowledged != null &&
                            _outputNotAcknowledged.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_outputMotion != null &&
                            _outputMotion.IdOutput == (Guid)iModifyObject.GetId)
                        {
                            continue;
                        }

                        if (_editingObject.OutputSetByObjectForAaFailed != null
                            && _editingObject.OutputSetByObjectForAaFailed.IdOutput == iModifyObject.GetId)
                        {
                            continue;
                        }

                        outputs.Add(iModifyObject);
                    }
                }

                return outputs;
            }

            return null;
        }

        private void RmoveEISSetUnsetOutput()
        {
            SetEISSetUnsetOutput(null, false);
        }

        private void SetEISSetUnsetOutput(Output eisSetUnsetOutput, bool checkOutput)
        {
            if (checkOutput && eisSetUnsetOutput != null)
            {
                if (!NCASClient.INTER_CCU_COMMUNICATION && _guidImplicitCCU != Guid.Empty)
                {
                    var inputCCUId = Guid.Empty;

                    if (eisSetUnsetOutput.CCU != null)
                    {
                        inputCCUId = eisSetUnsetOutput.CCU.IdCCU;
                    }
                    else if (eisSetUnsetOutput.DCU != null && eisSetUnsetOutput.DCU.CCU != null)
                    {
                        inputCCUId = eisSetUnsetOutput.DCU.CCU.IdCCU;
                    }

                    if (inputCCUId != _guidImplicitCCU)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmEISInputActivatianState.ImageTextBox,
                            GetString("ErrorOutputMustBeFromImplicitManager"), ControlNotificationSettings.Default);

                        return;
                    }
                }

                var isCorrectOutput = false;
                var outputs = GetOutputsForEisSetUnsetOutput();
                if (outputs != null && outputs.Count > 0)
                {
                    foreach (var iModifyObject in outputs)
                    {
                        if (iModifyObject != null && iModifyObject.GetId is Guid && (Guid)iModifyObject.GetId == eisSetUnsetOutput.IdOutput)
                        {
                            isCorrectOutput = true;
                            break;
                        }
                    }
                }

                if (!isCorrectOutput)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmEISInputActivatianState.ImageTextBox,
                        GetString("ErrorOutputAlreadyUsed"), ControlNotificationSettings.Default);

                    return;
                }
            }

            _eisSetUnsetOutput = eisSetUnsetOutput;
            RefreshEISSetUnsetOutput();
            RefreshImplictManager();
            EditTextChanger(null, null);
        }

        private void _tbmEISSetUnsetOutput_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmEISSetUnsetOutput_DragDrop(object sender, DragEventArgs e)
        {
            var output = e.Data.GetFormats();
            if (output == null)
                return;

            var unsetOutput = e.Data.GetData(output[0]) as Output;

            if (unsetOutput != null)
            {
                SetEISSetUnsetOutput(unsetOutput, true);
                return;
            }

            ControlNotification.Singleton.Error(
                NotificationPriority.JustOne,
                _tbmEISSetUnsetOutput.ImageTextBox,
                CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                ControlNotificationSettings.Default);
        }

        private void _tbmEISSetUnsetOutput_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_eisSetUnsetOutput != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_eisSetUnsetOutput);
            }
        }

        private void _tbmOutputSabotageState_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 13);
            }
            catch
            {
            }
        }

        private void _tbmOutputSabotageState_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputSabotageState != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputSabotageState);
            }
        }

        private void _tbmOutputNotAcknowledged_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 14);
            }
            catch
            {
            }
        }

        private void _tbmOutputNotAcknowledged_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputNotAcknowledged != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputNotAcknowledged);
            }
        }

        private void _tbmOutputMotion_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOutputAlarm(sender, e.Data.GetData(output[0]), 15);
            }
            catch
            {
            }
        }

        private void _tbmOutputMotion_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputMotion != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputMotion);
            }
        }

        private void _chbSetEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void SetSettingsForEventlog()
        {
            _editingObject.EnableEventlogsInCR = _chbEnableEventLogs.Checked;

            _editingObject.AaSet = _chbSetEvent.Checked;
            _editingObject.AaUnset = _chbUnsetEvent.Checked;
            _editingObject.AaAlarm = _chbAlarmEvent.Checked;
            _editingObject.AaNormal = _chbNormalEvent.Checked;
            _editingObject.AaAcknowledged = _chbAcknowledgeEvent.Checked;
            //_editingObject.AaUnconditionalSet = _chbUnconditionalSet.Checked;
            _editingObject.AaUnconditionalSet = false;

            _editingObject.SensorAlarm = _chbAlarmEventSensor.Checked;
            _editingObject.SensorNormal = _chbNormalEventSensor.Checked;
            _editingObject.SensorAcknowledged = _chbAcknowledgeEventSensor.Checked;
            _editingObject.SensorUnblocked = _chbUnblockedEvent.Checked;
            _editingObject.SensorTemporarilyBlocked = _chbTemporarilyBlockedEvent.Checked;
            _editingObject.SensorPermanentlyBlocked = _chbPermanentlyBlockedEvent.Checked;
        }

        private void LoadSettingsForEventlog()
        {
            _chbEnableEventLogs.Checked = _editingObject.EnableEventlogsInCR;

            _chbSetEvent.Checked = _editingObject.AaSet;
            _chbUnsetEvent.Checked = _editingObject.AaUnset;
            _chbAlarmEvent.Checked = _editingObject.AaAlarm;
            _chbNormalEvent.Checked = _editingObject.AaNormal;
            _chbAcknowledgeEvent.Checked = _editingObject.AaAcknowledged;
            _chbUnconditionalSet.Checked = _editingObject.AaUnconditionalSet;

            _chbAlarmEventSensor.Checked = _editingObject.SensorAlarm;
            _chbNormalEventSensor.Checked = _editingObject.SensorNormal;
            _chbAcknowledgeEventSensor.Checked = _editingObject.SensorAcknowledged;
            _chbUnblockedEvent.Checked = _editingObject.SensorUnblocked;
            _chbTemporarilyBlockedEvent.Checked = _editingObject.SensorTemporarilyBlocked;
            _chbPermanentlyBlockedEvent.Checked = _editingObject.SensorPermanentlyBlocked;
        }

        private void _chbUnsetEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbAlarmEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbNormalEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbAcknowledgeEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbUnconditionalSet_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbAlarmEventSensor_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbNormalEventSensor_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbAcknowledgeEventSensor_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbUnblockedEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbEnableEventLogs_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsSetValues)
                return;

            EditTextChanger(sender, e);
            RefreshCardReaderEventlogsCheckBoxes(true);
        }

        private void RefreshCardReaderEventlogsCheckBoxes(bool autoCheckAll)
        {
            var checkBoxes = new LinkedList<CheckBox>(
                _gbCardReaderEventlogEvents.Controls
                    .OfType<CheckBox>()
                    .Where(
                        checkBox =>
                            checkBox != _chbEnableEventLogs
                            && checkBox != _chbUnconditionalSet));

            if (autoCheckAll 
                && !checkBoxes.Any(
                    checkBox =>
                        checkBox.Checked))
            {
                foreach (var checkBox in checkBoxes)
                {
                    if (_chbEnableEventLogs.Checked)
                        checkBox.Checked = true;
                }
            }

            foreach (var checkBox in checkBoxes)
            {
                checkBox.Enabled = _chbEnableEventLogs.Checked;
            }
        }

        private void _chbPermanentlyBlockedEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _chbTemporarilyBlockedEvent_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void UpdateTimeBuyingControls(bool forcedOff)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            try
            {
                _eBuyTime.Visible = GetEditingObject().TimeBuyingEnabled;
                _bAlarmAreaBuyTime.Visible = GetEditingObject().TimeBuyingEnabled;

                if (!GetEditingObject().TimeBuyingEnabled || forcedOff)
                {
                    _eBuyTime.Enabled = false;
                    _bAlarmAreaBuyTime.Enabled = false;
                }
                else
                {
                    bool enable = (GetEditingObject().TimeBuyingOnlyInPrewarning
                                   && _activationState == ActivationState.Prewarning)
                                  || (!GetEditingObject().TimeBuyingOnlyInPrewarning
                                      && _activationState != ActivationState.Unset);

                    if (enable)
                    {
                        enable = Plugin.MainServerProvider.AlarmAreas
                            .HasAccessForAlarmAreaActionFromCurrentLogin(
                                AccessNCAS.AlarmAreasTimeBuyingPerform,
                                GetEditingObject().IdAlarmArea);
                    }

                    _eBuyTime.Enabled = enable;
                    _bAlarmAreaBuyTime.Enabled = enable;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private string GetTimeIntervalForTimeBuyingResult(int time, bool negative)
        {
            string result;

            Func<int, TimeSpan> timeCalc = (timeInSeconds) =>
            {
                int hours = timeInSeconds / 3600;
                int seconds = timeInSeconds % 3600;
                int minutes = timeInSeconds / 600;
                seconds -= minutes * 60;

                return new TimeSpan(hours, minutes, seconds);
            };

            if (time == int.MinValue)
            {
                result = LocalizationHelper.GetString("Unknown");
            }
            else if (time == int.MaxValue)
            {
                result = LocalizationHelper.GetString("Unlimited");
            }
            else if (time < 0)
            {
                if (negative)
                    result = string.Format("{0:hh\\:mm\\:ss}", timeCalc(time * -1));
                else
                    result = LocalizationHelper.GetString("Unknown");
            }
            else
            {
                result = string.Format("{0:hh\\:mm\\:ss}", timeCalc(time));
            }

            return result;
        }

        private void EventTimeBuyingFailed(Guid idAlarmArea, byte reason, int timeToBuy, int remainingTime)
        {
            if (idAlarmArea != GetEditingObject().IdAlarmArea)
                return;

            if (_requestActivationState != RequestActivationState.Unset
                && _requestActivationState != RequestActivationState.Unknown)
                return;

            MethodInvoker mi = () =>
            {
                string actionResult = LocalizationHelper.GetString(
                    "AlarmAreaActionResult_" 
                    + ((AlarmAreaActionResult)reason).ToString());

                _eResultOfAction.Text = string.Format(
                    LocalizationHelper.GetString("NCASAlarmAreaEditForm_EventTimeBuyingFailed"),
                    actionResult,
                    GetTimeIntervalForTimeBuyingResult(timeToBuy, false),
                    (remainingTime >= 0 
                        ? LocalizationHelper.GetString("remaining") 
                        : LocalizationHelper.GetString("missing")),
                    GetTimeIntervalForTimeBuyingResult(remainingTime, true));
                _eResultOfAction.BackColor = Color.Red;
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                mi();
            }

            /*_requestActivationState = RequestActivationState.Unknown;
            RefreshActivationState((byte)_activationState);
            RefreshRequestActivationState(_requestActivationState);
            RefreshSetUnsetButton();*/
        }

        private void EventBoughtTimeChanged(Guid idAlarmArea, string userName, int boughtTime, int remainingTime)
        {
            // Check if this event belong to editing object
            if (idAlarmArea != GetEditingObject().IdAlarmArea)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<Guid, string, int, int>(EventBoughtTimeChanged),
                    idAlarmArea,
                    userName,
                    boughtTime,
                    remainingTime);
                return;
            }

            _eResultOfAction.Text =
                string.Format(
                    LocalizationHelper.GetString("NCASAlarmAreaEditForm_EventBoughtTimeChanged"),
                    GetTimeIntervalForTimeBuyingResult(boughtTime, false),
                    GetTimeIntervalForTimeBuyingResult(remainingTime, false));
            _eResultOfAction.BackColor = Color.LightGreen;
        }

        private void _tbmOutputSetByObjectForAaFailed_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify17)
            {
                ModifyOutputSetByObjectForAaFailed();
                return;
            }

            SetOutputSetByObjectForAaFailed(null);
        }

        private void ModifyOutputSetByObjectForAaFailed()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            var formModify = new ListboxFormAdd(GetAvailableOutputs(), GetString("ModifyOutput"));
            object outObject;
            formModify.ShowDialog(out outObject);

            var outputModifyObject = outObject as OutputModifyObj;

            if (outputModifyObject != null)
            {
                var output = Plugin.MainServerProvider.Outputs.GetObjectById(outputModifyObject.Id);

                if (output != null)
                {
                    SetOutputSetByObjectForAaFailed(output);
                    Plugin.AddToRecentList(output);
                }
            }
        }

        private void RefreshOutputSetByObjectForAaFailed(Output output)
        {
            if (output == null)
            {
                _tbmOutputSetByObjectForAaFailed.Text = string.Empty;
                return;
            }

            _tbmOutputSetByObjectForAaFailed.Text = output.ToString();
            _tbmOutputSetByObjectForAaFailed.TextImage = Plugin.GetImageForAOrmObject(output);
        }

        private void SetOutputSetByObjectForAaFailed(Output output)
        {
            _editingObject.OutputSetByObjectForAaFailed = output;
            RefreshOutputSetByObjectForAaFailed(output);
        }

        private void _tbmOutputSetByObjectForAaFailed_DragDrop(object sender, DragEventArgs e)
        {
            var output = e.Data.GetFormats();

            if (output == null)
                return;

            AddOutputSetByObjectForAaFailed(e.Data.GetData(output[0]));
        }

        private void AddOutputSetByObjectForAaFailed(object newOutput)
        {
            try
            {
                var output = newOutput as Output;

                if (output == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmOutputSetByObjectForAaFailed.ImageTextBox,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (!CheckOutputAvailability(output, GetAvailableOutputs()))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmOutputSetByObjectForAaFailed.ImageTextBox,
                        GetString("ErrorOutputAlreadyUsed"),
                        ControlNotificationSettings.Default);

                    return;
                }

                SetOutputSetByObjectForAaFailed(output);
                Plugin.AddToRecentList(newOutput);
            }
            catch
            { }
        }

        private void _tbmOutputSetByObjectForAaFailed_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_editingObject.OutputSetByObjectForAaFailed == null)
                return;

            NCASOutputsForm.Singleton.OpenEditForm(_editingObject.OutputSetByObjectForAaFailed);
        }
    }

    public class BlockTemporarilyUntilTypeView
    {
        private readonly BlockTemporarilyUntilType? _blockTemporarilyUntilType;

        public byte? BlockTemporarilyUntilType { get { return (byte?)_blockTemporarilyUntilType; } }

        public BlockTemporarilyUntilTypeView(BlockTemporarilyUntilType? blockTemporarilyUntilType)
        {
            _blockTemporarilyUntilType = blockTemporarilyUntilType;
        }

        public string Name { get { return ToString(); } }

        public override string ToString()
        {
            if (_blockTemporarilyUntilType != null && NCASClient.LocalizationHelper != null)
                return NCASClient.LocalizationHelper.GetString("BlockTemporarilyUntilType_" + _blockTemporarilyUntilType);
            return string.Empty;
        }
    }

    public class BlockingTypeView
    {
        public SensorBlockingType? BlockingType { get; private set; }

        public BlockingTypeView(SensorBlockingType? blockingType)
        {
            BlockingType = blockingType;
        }

        public string Name { get { return ToString(); } }

        public override string ToString()
        {
            if (BlockingType != null && NCASClient.LocalizationHelper != null)
                return NCASClient.LocalizationHelper.GetString(
                    string.Format("SensorBlockingType_{0}",
                        BlockingType));

            return string.Empty;
        }
    }

    public class StateView
    {
        private readonly State? _state;

        public StateView(State? state)
        {
            _state = state;
        }

        public string Name { get { return ToString(); } }

        public override string ToString()
        {
            if (_state != null && NCASClient.LocalizationHelper != null)
                return NCASClient.LocalizationHelper.GetString(
                    string.Format("Input{0}",
                        _state));

            return string.Empty;
        }
    }

    public class CgpDataGridViewActions : ICgpDataGridView
    {
        private readonly Action _editAction;
        private readonly Action _deleteAction;
        private readonly Action _insertAction;

        public CgpDataGridViewActions(Action editAction, Action deleteAction, Action insertAction)
        {
            _editAction = editAction;
            _deleteAction = deleteAction;
            _insertAction = insertAction;
        }

        #region ICgpDataGridView Members

        public void EditClick()
        {
            if (_editAction != null)
                _editAction();
        }

        public virtual void EditClick(ICollection<int> indexes)
        {
        }

        public void DeleteClick()
        {
            if (_deleteAction != null)
                _deleteAction();
        }

        public virtual void DeleteClick(ICollection<int> indexes)
        {
        }

        public void InsertClick()
        {
            if (_insertAction != null)
                _insertAction();
        }

        #endregion
    }

    public class SensorPurposeView
    {
        public SensorPurpose? Purpose { get; private set; }

        public SensorPurposeView(SensorPurpose? purpose)
        {
            Purpose = purpose;
        }

        public string Name
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            if (NCASClient.LocalizationHelper != null)
                return NCASClient.LocalizationHelper.GetString(
                    string.Format("SensorPurpose_{0}",
                        Purpose == null
                            ? "Inherit"
                            : Purpose.Value.ToString()));

            return string.Empty;
        }
    }
}
