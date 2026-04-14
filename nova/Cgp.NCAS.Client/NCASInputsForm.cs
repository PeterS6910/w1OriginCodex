using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASInputsForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, Input, InputShort>
#endif
    {
        public NCASInputsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.Inputs48;
            InitializeComponent();
            _cdgvData.DataGrid.RowPrePaint += DgValuesRowPrePaint;
            _filterSettingsWithJoin = new[] { FilterSettings, new List<FilterSettings>(), new List<FilterSettings>(), new List<FilterSettings>() };
            InitCGPDataGridView();
            SetReferenceEditColors();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.AfterSort += _cdgvData_AfterSort;
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.EnabledDeleteButton = false;
        }

        void _cdgvData_AfterSort(IList dataSourceList)
        {
            RuntimeFilterValueChanged(null, null);
            RunFilter();
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (InputShort input in bindingSource)
            {
                input.Symbol = _cdgvData.GetDefaultImage(input);
            }
        }

        protected override Input GetObjectForEdit(InputShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.Inputs.GetObjectForEditById(listObj.IdInput, out editAllowed);
        }

        protected override Input GetFromShort(InputShort listObj)
        {
            return Plugin.MainServerProvider.Inputs.GetObjectById(listObj.IdInput);
        }

        BindingSource _bindingSourceInputs;
        private Action<Guid, byte, Guid> _eventInputStateChanged;
        private Action<Guid, byte, Guid> _eventOutputStateChanged;
        private Action<Guid, byte> _eventTimeZoneStateChanged;
        private Action<Guid, byte> _eventDailyPlanStateChanged;

        private static volatile NCASInputsForm _singleton;
        private static readonly object _syncRoot = new object();

        private NCASInputEditForm _inputInsertForm;

        private readonly IList<FilterSettings>[] _filterSettingsWithJoin;
        private LogicalOperators _filterSettingsJoinOperator = LogicalOperators.AND;

        private readonly SyncDictionary<Guid, State> _inputBlockingObjectsToState = new SyncDictionary<Guid, State>();
        private readonly SyncDictionary<Guid, ShortObjectExtension<InputShort, Guid?>> _shortInputToFilter = new SyncDictionary<Guid, ShortObjectExtension<InputShort, Guid?>>();

        private volatile int _currentlyBlockedFilterIndex;
        private volatile int _currentStateFilterIndex;

        public static NCASInputsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = 
                                new NCASInputsForm
                                {
                                    MdiParent = CgpClientMainForm.Singleton
                                };
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<InputShort> GetData()
        {
            Exception error;
            ICollection<InputShort> resultList;

            if (_currentlyBlockedFilterIndex > 0 || _currentStateFilterIndex > 0)
            {
                resultList = new List<InputShort>();
                var remoting = Plugin.MainServerProvider;
                var tempList = remoting.Inputs.SelectByCriteriaFullInputs(out error, _filterSettingsJoinOperator, _filterSettingsWithJoin);

                _inputBlockingObjectsToState.Clear(
                    () =>
                    {
                        foreach (var input in tempList)
                        {   
                            var inputShort = new InputShort(input);
                            ShortObjectExtension<InputShort, Guid?> filterValues;
                            if (input.BlockingType == 3 /*BlockingType.BlockedByObject */
                                && input.OnOffObjectId != null)
                            {
                                var state = Plugin.MainServerProvider.GetRealOnOffObjectState(input.OnOffObject.GetObjectType(), input.OnOffObjectId.Value);//input.OnOffObject.State ? State.On : State.Off;
                                /*if (state == State.Unknown && input.OnOffObjectObjectType == ObjectType.DailyPlan)
                                {
                                    state = Plugin.MainServerProvider.GetRealOnOffObjectState(ObjectType.TimeZone, input.OnOffObjectId.Value);
                                }*/
                                _inputBlockingObjectsToState[input.OnOffObjectId.Value] = state;
                                filterValues = new ShortObjectExtension<InputShort, Guid?>(inputShort, input.OnOffObjectId);
                            }
                            else
                                filterValues = new ShortObjectExtension<InputShort, Guid?>(inputShort, null);

                            inputShort.State = remoting.Inputs.GetActualStatesByGuid(input.IdInput);
                            resultList.Add(inputShort);
                            _shortInputToFilter[inputShort.IdInput] = filterValues;
                        }
                    });
            }
            else
            {
                _inputBlockingObjectsToState.Clear();
                _shortInputToFilter.Clear();
                resultList = Plugin.MainServerProvider.Inputs.ShortSelectByCriteria(out error, _filterSettingsJoinOperator, _filterSettingsWithJoin);
            }

            if (error != null)
                throw (error);

            var nameFilter = _eNameFilter.Text;

            if (!string.IsNullOrEmpty(nameFilter))
            {
                resultList =
                    resultList.Where(input => 
                        (input.SectionIds != null && input.SectionIds.Contains(nameFilter)) 
                        || input.FullName.IndexOf(nameFilter,StringComparison.InvariantCultureIgnoreCase) >=0
                        || input.NickName.IndexOf(nameFilter, StringComparison.InvariantCultureIgnoreCase) >=0 )
                    .ToList();
            }

            return resultList;
        }

        protected override bool Compare(Input obj1, Input obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(Input obj, Guid idObj)
        {
            return obj.IdInput == idObj;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            var lastPosition = 0;
            if (_bindingSourceInputs != null && _bindingSourceInputs.Count != 0)
                lastPosition = _bindingSourceInputs.Position;
            _bindingSourceInputs = bindingSource;
            _bindingSourceInputs.AllowNew = false;
            if (lastPosition != 0) _bindingSourceInputs.Position = lastPosition;

            _cdgvData.ModifyGridView(
                bindingSource, 
                InputShort.COLUMN_SYMBOL, 
                InputShort.COLUMNSECTIONIDS, 
                InputShort.COLUMNFULLNAME, 
                InputShort.COLUMNNICKNAME,
                InputShort.COLUMNSTATUSONOFF, 
                InputShort.COLUMNDESCRIPTION);
        }

        void DgValuesRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var bs = (BindingSource)_cdgvData.DataGrid.DataSource;
            var inputShort = (InputShort)bs.List[e.RowIndex];

            var row = _cdgvData.DataGrid.Rows[e.RowIndex];
            if (row.Cells[InputShort.COLUMNSTATUSONOFF].Value == null)
            {
                if (inputShort != null)
                {
                    switch (inputShort.State)
                    {
                        case InputState.Alarm:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputAlarm");
                            break;
                        case InputState.Short:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputShort");
                            break;
                        case InputState.Break:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputBreak");
                            break;
                        case InputState.Normal:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputNormal");
                            break;
                        case InputState.UsedByAnotherAplication:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                            break;
                        case InputState.OutOfRange:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                            break;
                        default:
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                            break;
                    }
                }

                _cdgvData.DataGrid.AutoResizeColumn(row.Cells[Input.COLUMNSTATUSONOFF].ColumnIndex);
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.DataGrid.DataSource = null;
        }

        protected override ACgpPluginEditForm<NCASClient, Input> CreateEditForm(Input obj, ShowOptionsEditForm showOption)
        {
            return new NCASInputEditForm(obj, showOption, this);
        }

        protected override void DeleteObj(Input obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.Inputs.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            _filterSettingsJoinOperator = (_rbFilterAnd.Checked ? LogicalOperators.AND : LogicalOperators.OR);
            foreach (var list in _filterSettingsWithJoin)
                list.Clear();

            //if (_eNameFilter.Text != "")
            //{
            //    var filterSetting = new FilterSettings(Input.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH, _filterSettingsJoinOperator);
            //    FilterSettings.Add(filterSetting);
            //}

            if (_cbInvertedFilter.SelectedIndex > 0)
            {
                var inverted = (_cbInvertedFilter.SelectedIndex == 1);
                var filterSetting = new FilterSettings(Input.COLUMNINVERTED, inverted, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            if (_cbInputTypeFilter.SelectedIndex > 0)
            {
                var type = (byte)(_cbInputTypeFilter.SelectedIndex == 1 ? 0 : 1);
                var filterSetting = new FilterSettings(Input.COLUMNINPUTTYPE, type, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            if (_cbEnableAlarmsFilter.SelectedIndex > 0)
            {
                var value = (_cbEnableAlarmsFilter.SelectedIndex == 1);
                var filterSetting = new FilterSettings(Input.COLUMNFILTERALARMON, value, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            foreach(var item in _clbInputControlFilter.Items)
            {
                if (item != null)
                {
                    var value = (byte)_clbInputControlFilter.Items.IndexOf(item);
                    if (_clbInputControlFilter.GetItemCheckState(value) == CheckState.Checked)
                    {
                        var filterSetting = new FilterSettings(Input.COLUMNBLOCKINGTYPE, value, ComparerModes.EQUALL, LogicalOperators.OR);
                        _filterSettingsWithJoin[1].Add(filterSetting);
                    }
                }
            }

            if (_tbmPresentationGroupFilter.Tag != null)
            {
                var list = _tbmPresentationGroupFilter.Tag as ListOfObjects;
                if (list != null)
                    foreach (PresentationGroup item in list)
                        if (item != null)
                        {
                            //Guid value = (Guid)item.GetId();
                            var filterSetting = new FilterSettings(Input.COLUMNALARMPRESENTATIONGROUP, item, ComparerModes.EQUALL, LogicalOperators.OR);
                            _filterSettingsWithJoin[2].Add(filterSetting);
                            filterSetting = new FilterSettings(Input.COLUMNTAMPERALARMPRESENTATIONGROUP, item, ComparerModes.EQUALL, LogicalOperators.OR);
                            _filterSettingsWithJoin[2].Add(filterSetting);
                        }
            }
            if (_tbmBlockedByFilter.Tag != null)
            {
                var list = _tbmBlockedByFilter.Tag as ListOfObjects;
                if (list != null)
                    foreach (AOrmObject item in list)
                        if (item != null)
                        {
                            var value = (Guid)item.GetId();
                            var filterSetting = new FilterSettings(Input.COLUMNONOFFOBJECTID, value, ComparerModes.EQUALL, LogicalOperators.OR);
                            _filterSettingsWithJoin[3].Add(filterSetting);
                        }
            }

            _currentlyBlockedFilterIndex = _cbCurrentlyBlockedFilter.SelectedIndex;
            _currentStateFilterIndex = _cbCurrentStateFilter.SelectedIndex;
        }

        protected override void ClearFilterEdits()
        {
            _filterSettingsJoinOperator = LogicalOperators.AND;
            _rbFilterAnd.Checked = true;

            foreach (var filter in _filterSettingsWithJoin)
                filter.Clear();

            _eNameFilter.Text = "";
            _cbCurrentlyBlockedFilter.SelectedIndex = 0;
            _cbCurrentStateFilter.SelectedIndex = 0;
            _currentlyBlockedFilterIndex = 0;
            _currentStateFilterIndex = 0;
            _cbEnableAlarmsFilter.SelectedIndex = 0;
            _cbInputTypeFilter.SelectedIndex = 0;
            _cbInvertedFilter.SelectedIndex = 0;
            for (var i = 0; i < _clbInputControlFilter.Items.Count; i++)
            {
                _clbInputControlFilter.SetItemChecked(i, false);
            }
            
            SetSelectedObjects(_tbmBlockedByFilter, null);
            SetSelectedObjects(_tbmPresentationGroupFilter, null);
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Inputs.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(Input input)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Inputs.HasAccessViewForObject(input);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Inputs.HasAccessInsert();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Inputs.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
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
            var dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            // update string in DataGridView
            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                var inputShort = (InputShort)_bindingSourceInputs.List[row.Index];

                if (inputShort != null)
                {
                    if (inputShort.IdInput == inputGuid)
                    {
                        switch (inputState)
                        {
                            case InputState.Alarm:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("InputAlarm"));
                                break;

                            case InputState.Short:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("InputShort"));
                                break;

                            case InputState.Break:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("InputBreak"));
                                break;

                            case InputState.Normal:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("InputNormal"));
                                break;
                        
                            case InputState.UsedByAnotherAplication:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("UsedByAnotherAplication"));
                                break;
                        
                            case InputState.OutOfRange:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("OutOfRange"));
                                break;

                            default:
                                _cdgvData.UpdateValue(row.Index, Input.COLUMNSTATUSONOFF, GetString("Unknown"));
                                break;
                        }
                    }
                }
            }

            if (dgcell != null && dgcell.Visible)
            {
                _cdgvData.DataGrid.CurrentCell = dgcell;
            }
        }

        private void _bPreCreateInput_Click(object sender, EventArgs e)
        {
            Insert();
        }

        #region Events from server

        protected override void RegisterEvents()
        {
            if (_eventInputStateChanged == null)
            {
                _eventInputStateChanged = ChangeInputState;
                StateChangedInputHandler.Singleton.RegisterStateChanged(_eventInputStateChanged);
            }

            if(_eventOutputStateChanged == null)
            {
                _eventOutputStateChanged = ChangeOutputState;
                StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputStateChanged);
            }
            
            if(_eventTimeZoneStateChanged == null)
            {
                _eventTimeZoneStateChanged = ChangeTimeZoneState;
                StatusChangedTimeZoneHandler.Singleton.RegisterStatusChanged(_eventTimeZoneStateChanged);
            }

            if(_eventDailyPlanStateChanged == null)
            {
                _eventDailyPlanStateChanged = ChangeDailyPlanState;
                StatusChangedDailyPlainHandler.Singleton.RegisterStatusChanged(_eventDailyPlanStateChanged);
            }
        }

        protected override void UnregisterEvents()
        {
            if (_eventInputStateChanged != null)
            {
                StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventInputStateChanged);
                _eventInputStateChanged = null;
            }

            if (_eventOutputStateChanged != null)
            {
                StateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventOutputStateChanged);
                _eventOutputStateChanged = null;
            }

            if (_eventTimeZoneStateChanged != null)
            {
                StatusChangedTimeZoneHandler.Singleton.UnregisterStatusChanged(_eventTimeZoneStateChanged);
                _eventTimeZoneStateChanged = null;
            }

            if (_eventDailyPlanStateChanged != null)
            {
                StatusChangedDailyPlainHandler.Singleton.UnregisterStatusChanged(_eventDailyPlanStateChanged);
                _eventDailyPlanStateChanged = null;
            }
        }

        private void ChangeInputState(Guid inputGuid, byte state, Guid parent)
        {
            SafeThread<Guid, byte>.StartThread(DoChangeInputState, inputGuid, state);

            UpdateRuntimeFilter(ObjectType.Input, inputGuid, state);
        }

        private void ChangeOutputState(Guid inputGuid, byte state, Guid parent)
        {
            UpdateRuntimeFilter(ObjectType.Output, inputGuid, state);
        }

        private void ChangeTimeZoneState(Guid inputGuid, byte state)
        {
            UpdateRuntimeFilter(ObjectType.TimeZone, inputGuid, state);
        }

        private void ChangeDailyPlanState(Guid inputGuid, byte state)
        {
            UpdateRuntimeFilter(ObjectType.DailyPlan, inputGuid, state);
        }

        #endregion

        private void _bPrecreateCCUInput_Click(object sender, EventArgs e)
        {
            if (_inputInsertForm == null || _inputInsertForm.Visible == false)
                _inputInsertForm = new NCASInputEditForm(new Input(), ShowOptionsEditForm.Insert, this);
            else
            {
                _inputInsertForm.Focus();
                return;
            }

            _inputInsertForm.CcuInput = true;
            _inputInsertForm.Show();
        }

        private void _tbmPresentationGroup_DragOver(object sender, DragEventArgs e)
        {
            var pg = GetObjectFromDragDrop<PresentationGroup>(e);
            if (pg != null)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void _tbmPresentationGroup_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var pg = GetObjectFromDragDrop<PresentationGroup>(e);
                if (pg != null)
                {
                    var tempList = 
                        new List<object>
                        {
                            pg
                        };

                    SetSelectedObjects(_tbmPresentationGroupFilter, new ListOfObjects(tempList));
                }
            }
            catch
            {
            }
        }

        private void _tbmPresentationGroup_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify3)
            {
                ModifyPresentationGroupObjects();
            }
            else if (item == _tsiRemove3)
            {
                SetSelectedObjects(_tbmPresentationGroupFilter, null);
            }
        }

        private void _tbmBlockedBy_DragOver(object sender, DragEventArgs e)
        {
            var data = GetObjectFromDragDrop<AOnOffObject>(e);

            if (data != null)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void _tbmBlockedBy_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var data = GetObjectFromDragDrop<AOnOffObject>(e);

                if (data != null)
                {
                    var tempList = 
                        new List<object>
                        {
                            data
                        };
                    SetSelectedObjects(_tbmBlockedByFilter, new ListOfObjects(tempList));
                }
            }
            catch
            {
            }
        }

        private void _tbmBlockedBy_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify2)
            {
                ModifyBlockedByObjects();
            }
            else if (item == _tsiRemove2)
            {
                SetSelectedObjects(_tbmBlockedByFilter, null);
            }
        }

        private void _rbFilterOr_CheckedChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            if (_rbFilterOr.Checked)
                _filterSettingsJoinOperator = LogicalOperators.OR;
            else
                _filterSettingsJoinOperator = LogicalOperators.AND;
        }
    
        private void SetSelectedObjects(TextBoxMenu control, ListOfObjects objects)
        {
            control.Tag = objects;
            RefreshSelectedObject(control);
        }

        private void RefreshSelectedObject(TextBoxMenu control)
        {
            var actOnOffObjects = control.Tag as ListOfObjects;
            if (control.Tag == null)
            {
                control.Text = string.Empty;
                control.TextImage = null;
            }
            else
            {
                if (actOnOffObjects.Count > 1)
                {
                    control.Text = actOnOffObjects.ToString();
                    try
                    {
                        control.TextImage = Plugin.GetImageForListOfObject(actOnOffObjects);
                    }
                    catch
                    {
                        control.TextImage = null;
                    }
                }
                else
                {
                    var tempAOrm = actOnOffObjects[0] as AOrmObject;
                    if (tempAOrm != null)
                    {
                        var listFS = new ListObjFS(tempAOrm);
                        control.Text = listFS.ToString();
                        control.TextImage = Plugin.GetImageForAOrmObject(tempAOrm);
                    }
                    else
                    {
                        control.Text = actOnOffObjects.ToString();
                        control.TextImage = Plugin.GetImageForListOfObject(actOnOffObjects);
                    }
                }
            }

            base.FilterValueChanged(control, new EventArgs());
        }
        
        private void ModifyBlockedByObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error;
            try
            {
                ConnectionLost();

                var listObjects = new List<IModifyObject>();

                var listTimeZonesFromDatabase = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;

                if (listTimeZonesFromDatabase != null && listTimeZonesFromDatabase.Count > 0)
                    listObjects.AddRange(listTimeZonesFromDatabase);

                var listDailyPlansFromDatabase = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlansFromDatabase);
                if (listDailyPlansFromDatabase != null && listDailyPlansFromDatabase.Count > 0)
                {
                    listObjects.AddRange(listDailyPlansFromDatabase);
                }

                IList<FilterSettings> filterSettings = new List<FilterSettings>();

                IList<IModifyObject> listInputsFromDatabase = Plugin.MainServerProvider.Inputs.ModifyObjectsSelectByCriteria(filterSettings, out error);

                if (error != null)
                    throw error;

                if (listInputsFromDatabase != null && listInputsFromDatabase.Count > 0)
                    listObjects.AddRange(listInputsFromDatabase);

                IList<IModifyObject> listOutputsFromDatabase = Plugin.MainServerProvider.Outputs.ListModifyObjects(out error);
                
                if (error != null) throw error;

                if (listOutputsFromDatabase != null && listOutputsFromDatabase.Count > 0)
                {
                    listObjects.AddRange(listOutputsFromDatabase);
                }

                var formAdd = new ListboxFormAdd(listObjects, GetString("NCASInputEditFormListObjectForBlockedInputText"));

                ListOfObjects outObjects;
                formAdd.ShowDialogMultiSelect(out outObjects);
                for (var i = 0; i < outObjects.Count; i++)
                {
                    var modifyObject = outObjects[i] as IModifyObject;
                    if (modifyObject != null)
                    {
                        switch (modifyObject.GetOrmObjectType)
                        {
                            case ObjectType.TimeZone:
                                outObjects[i] = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(modifyObject.GetId);
                                break;
                            case ObjectType.DailyPlan:
                                outObjects[i] = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(modifyObject.GetId);
                                break;
                            case ObjectType.Input:
                                outObjects[i] = Plugin.MainServerProvider.Inputs.GetObjectById(modifyObject.GetId);
                                break;
                            case ObjectType.Output:
                                outObjects[i] = Plugin.MainServerProvider.Outputs.GetObjectById(modifyObject.GetId);
                                break;
                        }
                    }
                }

                if (outObjects != null)
                {
                    SetSelectedObjects(_tbmBlockedByFilter, outObjects);
                }
            }
            catch
            {
            }
        }

        private void ModifyPresentationGroupObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                ConnectionLost();

                var listObjects = new List<AOrmObject>();

                Exception error;
                var listPresentationGroup = CgpClient.Singleton.MainServerProvider.PresentationGroups.List(out error);
                if (error != null) throw error;

                if (listPresentationGroup != null && listPresentationGroup.Count > 0)
                    foreach (var pg in listPresentationGroup)
                        listObjects.Add(pg);

                var formAdd = new ListboxFormAdd(listObjects, GetString("NCASInputsForm_lPresentationGroupFilter"));

                ListOfObjects outObjects;
                formAdd.ShowDialogMultiSelect(out outObjects);

                if (outObjects != null)
                {
                    SetSelectedObjects(_tbmPresentationGroupFilter, outObjects);
                }
            }
            catch
            {
            }
        }

        private void UpdateRuntimeFilter(ObjectType objectType, Guid objectId, byte state)
        {
            SafeThread<ObjectType, Guid, byte>.StartThread(
                DoUpdateRuntimeFilter, 
                objectType, 
                objectId, 
                state);
        }

        private void DoUpdateRuntimeFilter(ObjectType objectType, Guid objectId, byte state)
        {
            State objectState;
            switch (state)
            {
                case 0:
                    objectState = State.Off;
                    break;

                case 1:
                    objectState = State.On;
                    break;

                default:
                    objectState = State.Unknown;
                    break;
            }

            var updated = false;
            _inputBlockingObjectsToState.TryGetValue(objectId,
                (key, found, value) =>
                {
                    if ((found && value != objectState)
                        || !found)
                    {
                        updated = true;
                        RuntimeFilterValueChanged(key, null);
                    }

                    _inputBlockingObjectsToState[key] = objectState;
                });

            if (objectType == ObjectType.Input)            
            {
                _shortInputToFilter.TryGetValue(objectId,
                    (key, found, value) =>
                    {
                        if (found && value.ShortObject.State != (InputState)state)
                        {
                            value.ShortObject.State = (InputState)state;
                            updated = true;
                            RuntimeFilterValueChanged(key, null);
                        }
                    });
            }

            if (updated)
                RunFilter();
        }

        protected override void ApplyRuntimeFilter()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ApplyRuntimeFilter));
                return;
            }

            var currencyManager = (CurrencyManager)BindingContext[_cdgvData.DataGrid.DataSource];
            currencyManager.SuspendBinding();

            // Runtime filters is not used
            if (_currentlyBlockedFilterIndex < 1 && _currentStateFilterIndex < 1)
            {
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    row.Visible = true;
                }
                currencyManager.ResumeBinding();
                _lRecordCount.Text = GetString("NCASInputsForm_lRecordCount") + ": " + _cdgvData.DataGrid.Rows.Count;
                return;
            }

            var dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            var visibleCount = 0;

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                var inputShort = (InputShort)_bindingSourceInputs.List[row.Index];

                if (inputShort != null)
                {
                    ShortObjectExtension<InputShort, Guid?> filterValues;
                    if (_shortInputToFilter.TryGetValue(inputShort.IdInput, out filterValues))
                    {
                        // Apply current input state filter
                        if (_currentStateFilterIndex < 1
                            || (_currentStateFilterIndex - 1 <= (int)InputState.OutOfRange
                                && inputShort.State == (InputState)(_currentStateFilterIndex - 1)) // Compare current input state with selected state
                            || ((_currentStateFilterIndex == 7)
                                && inputShort.State == InputState.Unknown))
                        {
                            // Apply currently blocked filter
                            if (filterValues.Value != null)
                            {
                                if (filterValues.ShortObject == null)
                                {
                                    row.Visible = false;
                                    continue;
                                }

                                // Get blocking object state
                                _inputBlockingObjectsToState.TryGetValue(
                                    filterValues.Value.Value,
                                    (key, found, value) =>
                                    {
                                        if (found)
                                        {
                                            // Add currently not blocked
                                            if (_currentlyBlockedFilterIndex == 1
                                                && value == State.Off)
                                            {
                                                visibleCount++;
                                                row.Visible = true;
                                            }
                                            // Add currently blocked
                                            else if (_currentlyBlockedFilterIndex == 2
                                                && value == State.On)
                                            {
                                                visibleCount++;
                                                row.Visible = true;
                                            }
                                            else if (_currentlyBlockedFilterIndex < 1)
                                            {
                                                visibleCount++;
                                                row.Visible = true;
                                            }
                                            else
                                            {
                                                row.Visible = false;
                                            }
                                        }
                                        else
                                        {
                                            row.Visible = false;
                                        }
                                    });
                            }
                            else
                            {
                                // This input does not have onOff object
                                        
                                if (_currentlyBlockedFilterIndex < 1)
                                {
                                    visibleCount++;
                                    row.Visible = true;
                                }
                                else
                                {
                                    row.Visible = false;
                                }
                            }
                        }
                        else
                        {
                            row.Visible = false;
                        }
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
            }

            currencyManager.ResumeBinding();

            _lRecordCount.Text = GetString("NCASInputsForm_lRecordCount") + ": " + visibleCount + "/" + _cdgvData.DataGrid.Rows.Count;

            if (dgcell != null && dgcell.Visible)
            {
                _cdgvData.DataGrid.CurrentCell = dgcell;
            }
        }

        private void DisableKeyPress_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void _clbInputControlFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            FilterValueChanged(sender, e);
        }
    }
}
