using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class AddAAInputsDialog : CgpTranslateForm
    {
        private readonly NCASClient _plugin;
        private readonly AlarmArea _alarmArea;
        private readonly Guid _guidImplicitCCU;
        private readonly ToolTip _toolTipInfoSensorTemporarilyBlockingOnlyInSabotage = new ToolTip();

        public ListOfObjects ActInputObjects { get; private set; }
        public byte BlockTemporarilyUntil 
        {
            get
            {
                return (byte) _cbBlockTemporarilyUntil.SelectedValue;
            }
        }

        public SensorPurpose? SensorPurpose
        {
            get
            {
                return ((SensorPurposeView)_cbSymbolPurpose.SelectedItem).Purpose;
            }
        }

        public bool LowSecurityInput
        {
            get { return _chbLowSecurityInput.Checked; }
        }

        public AddAAInputsDialog(NCASClient plugin, AlarmArea alarmArea)
            : base(NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            _plugin = plugin;
            _alarmArea = alarmArea;

            var listCbBlockTemporarilyUntil = new List<BlockTemporarilyUntilTypeView>();
            foreach (BlockTemporarilyUntilType blockTemporarilyUntilType in Enum.GetValues(typeof(BlockTemporarilyUntilType)))
            {
                listCbBlockTemporarilyUntil.Add(new BlockTemporarilyUntilTypeView(blockTemporarilyUntilType));
            }

            _cbSymbolPurpose.Items.Add(new SensorPurposeView(null));
            _cbSymbolPurpose.SelectedIndex = 0;

            foreach (SensorPurpose sensorPurpose in Enum.GetValues(typeof(SensorPurpose)))
            {
                _cbSymbolPurpose.Items.Add(new SensorPurposeView(sensorPurpose));
            }

            _cbBlockTemporarilyUntil.DisplayMember = "Name";
            _cbBlockTemporarilyUntil.ValueMember = "BlockTemporarilyUntilType";
            _cbBlockTemporarilyUntil.DataSource = listCbBlockTemporarilyUntil;

            var implicitCcu = _plugin.MainServerProvider.AlarmAreas.GetImplicitCCUForAlarmArea(_alarmArea.IdAlarmArea);
            _guidImplicitCCU = implicitCcu != null
                ? implicitCcu.IdCCU
                : Guid.Empty;

            _tbmInput.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmInput.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
        }

        protected override void AfterTranslateForm()
        {
            _toolTipInfoSensorTemporarilyBlockingOnlyInSabotage.RemoveAll();

            _toolTipInfoSensorTemporarilyBlockingOnlyInSabotage.SetToolTip(
                _pbSensorTemporarilyBlockingOnlyInSabotageInfo,
                GetString("InfoSensorTemporarilyBlockingOnlyInSabotage"));
        }

        private void _tbmInput_DoubleClick(object sender, EventArgs e)
        {
            if (ActInputObjects != null && ActInputObjects.Objects.Count == 1)
            {
                NCASInputsForm.Singleton.OpenEditForm(ActInputObjects.Objects[0] as Input);
            }
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            if (ActInputObjects == null
                || ActInputObjects.Count == 0)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmInput,
                    GetString("ErrorEntryAreaInput"),
                    ControlNotificationSettings.Default);

                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _tbmInput_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyImput();
            }
        }

        private void ModifyImput()
        {
            try
            {
                IList<IModifyObject> listAreaInputs;

                var filterSettings = new List<FilterSettings>();
                if (_alarmArea.AAInputs != null)
                {
                    foreach (var aaInput in _alarmArea.AAInputs)
                    {
                        var fSetting = new FilterSettings(Input.COLUMNIDIMPUT, aaInput.Input.IdInput, ComparerModes.NOTEQUALL);
                        filterSettings.Add(fSetting);
                    }
                }

                Exception error;

                if (!NCASClient.INTER_CCU_COMMUNICATION)
                {
                    listAreaInputs = _plugin.MainServerProvider.Inputs.ModifyObjectsSelectByCriteria(filterSettings, out error,
                        _guidImplicitCCU);
                }
                else
                {
                    listAreaInputs = _plugin.MainServerProvider.Inputs.ModifyObjectsSelectByCriteria(filterSettings, out error);
                }

                if (error != null)
                    throw error;

                if (listAreaInputs != null)
                {
                    var formAdd = new ListboxFormAdd(listAreaInputs, GetString("NCASInputsFormNCASInputsForm"));

                    ActInputObjects = new ListOfObjects();
                    ListOfObjects outInputs;
                    formAdd.ShowDialogMultiSelect(out outInputs);
                    if (outInputs != null)
                    {
                        foreach (var selectedObject in outInputs)
                        {
                            var inputMo = (InputModifyObj)selectedObject;
                            Input input = _plugin.MainServerProvider.Inputs.GetObjectById(inputMo.Id);
                            ActInputObjects.Objects.Add(input);
                        }
                        if (ActInputObjects != null)
                        {
                            _tbmInput.Text = ActInputObjects.ToString();
                            _tbmInput.TextImage = _plugin.GetImageForObjectType(ObjectType.Input);
                        }
                        else
                        {
                            _tbmInput.Text = string.Empty;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void _tbmInput_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmInput_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddInput(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddInput(object newInput)
        {
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
                                _tbmInput,
                                errorMessage,
                                ControlNotificationSettings.Default);

                            return;
                        }

                        ActInputObjects = new ListOfObjects();
                        ActInputObjects.Objects.Add(input);
                        _plugin.AddToRecentList(newInput);
                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmInput,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private bool CheckInputBeforeAdding(
            Input input,
            out string errorMessage)
        {
            if (!NCASClient.INTER_CCU_COMMUNICATION)
            {
                if (!_plugin.MainServerProvider.AlarmAreas.SetSensorToAlarmArea(input.IdInput, _guidImplicitCCU))
                {
                    errorMessage = GetString("InterCCUCommunicationNotEnabled");
                    return false;
                }
            }

            if (_alarmArea.AAInputs == null)
            {
                errorMessage = null;
                return true;
            }

            if (_alarmArea.AAInputs.Any(
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

        private void _chbLowSecurityInput_CheckedChanged(object sender, EventArgs e)
        {
            _pbSensorTemporarilyBlockingOnlyInSabotageInfo.Visible = _chbLowSecurityInput.Checked;
        }
    }
}
