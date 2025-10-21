using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASOutputsForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, Output, OutputShort>
#endif
    {
        private Action<Guid, byte, Guid> _eventInputStateChanged;
        private Action<Guid, byte, Guid> _eventOutputStateChanged;
        private Action<Guid, byte> _eventTimeZoneStateChanged;
        private Action<Guid, byte> _eventDailyPlanStateChanged;
        private Action<Guid, byte, Guid> _eventOutputRealStateChanged;

        private readonly IList<FilterSettings>[] _filterSettingsWithJoin;
        private LogicalOperators _filterSettingsJoinOperator = LogicalOperators.AND;

        private readonly SyncDictionary<Guid, State> _outputControllObjectsToState = new SyncDictionary<Guid, State>();
        private readonly SyncDictionary<Guid, ShortObjectExtension<OutputShort, Guid?>> _shortOutputToFilter = new SyncDictionary<Guid, ShortObjectExtension<OutputShort, Guid?>>();
        int _currentlyControlledFilterIndex;
        int _currentStateFilterIndex;

        public NCASOutputsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.Outputs48;
            InitializeComponent();
            _cdgvData.DataGrid.RowPrePaint += DgValuesRowPrePaint;
            _filterSettingsWithJoin = new IList<FilterSettings>[] { base.FilterSettings, new List<FilterSettings>(), new List<FilterSettings>(), new List<FilterSettings>() };
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

            foreach (OutputShort output in bindingSource)
            {
                output.Symbol = _cdgvData.GetDefaultImage(output);
            }
        }

        protected override Output GetObjectForEdit(OutputShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.Outputs.GetObjectForEditById(listObj.IdOutput, out editAllowed);
        }

        protected override Output GetFromShort(OutputShort listObj)
        {
            return Plugin.MainServerProvider.Outputs.GetObjectById(listObj.IdOutput);
        }

        BindingSource _bindingSourceOutputs;

        private static volatile NCASOutputsForm _singleton;
        private static readonly object _syncRoot = new object();

        NCASOutputEditForm _outputInsertForm;

        public static NCASOutputsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new NCASOutputsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        #region Events from server

        protected override void RegisterEvents()
        {
            if (_eventOutputStateChanged == null)
            {
                _eventOutputStateChanged = ChangeOutputState;
                StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputStateChanged);
            }
            if (_eventOutputRealStateChanged == null)
            {
                _eventOutputRealStateChanged = ChangeOutputRealState;
                RealStateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputRealStateChanged);
            }

            if (_eventInputStateChanged == null)
            {
                _eventInputStateChanged = ChangeInputState;
                StateChangedInputHandler.Singleton.RegisterStateChanged(_eventInputStateChanged);
            }

            if (_eventTimeZoneStateChanged == null)
            {
                _eventTimeZoneStateChanged = ChangeTimeZoneState;
                StatusChangedTimeZoneHandler.Singleton.RegisterStatusChanged(_eventTimeZoneStateChanged);
            }

            if (_eventDailyPlanStateChanged == null)
            {
                _eventDailyPlanStateChanged = ChangeDailyPlanState;
                StatusChangedDailyPlainHandler.Singleton.RegisterStatusChanged(_eventDailyPlanStateChanged);
            }
        }

        protected override void UnregisterEvents()
        {
            if (_eventOutputStateChanged != null)
            {
                StateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventOutputStateChanged);
                _eventOutputStateChanged = null;
            }
            if (_eventOutputRealStateChanged != null)
            {
                RealStateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventOutputRealStateChanged);
                _eventOutputRealStateChanged = null;
            }

            if (_eventInputStateChanged != null)
            {
                StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventInputStateChanged);
                _eventInputStateChanged = null;
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
            UpdateRuntimeFilter(ObjectType.Input, inputGuid, state);
        }

        private void ChangeOutputState(Guid outputGuid, byte state, Guid parent)
        {
            SafeThread<Guid, byte>.StartThread(DoChangeOutputState, outputGuid, state);

            UpdateRuntimeFilter(ObjectType.Output, outputGuid, state);
        }

        private void ChangeTimeZoneState(Guid timeZoneGuid, byte state)
        {
            UpdateRuntimeFilter(ObjectType.TimeZone, timeZoneGuid, state);
        }

        private void ChangeDailyPlanState(Guid dailyPlanGuid, byte state)
        {
            UpdateRuntimeFilter(ObjectType.DailyPlan, dailyPlanGuid, state);
        }

        private void ChangeOutputRealState(Guid outputGuid, byte state, Guid parent)
        {
            SafeThread<Guid, byte>.StartThread(DoChangeOutputRealState, outputGuid, state);
        }
        #endregion

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
            DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                OutputShort outputShort = (OutputShort)((BindingSource)_cdgvData.DataGrid.DataSource).List[row.Index];

                if (outputShort != null)
                {
                    if (outputShort.IdOutput == outputGuid)
                    {
                        switch(outputState)
                        {
                            case OutputState.Off:
                                _cdgvData.UpdateValue(row.Index, Output.COLUMNSTATUSONOFF, GetString("Off"));
                                break;

                            case OutputState.On:
                                _cdgvData.UpdateValue(row.Index, Output.COLUMNSTATUSONOFF, GetString("On"));
                                break;

                            case OutputState.UsedByAnotherAplication:
                                _cdgvData.UpdateValue(row.Index, Output.COLUMNSTATUSONOFF, GetString("UsedByAnotherAplication"));
                                break;

                            case OutputState.OutOfRange:
                                _cdgvData.UpdateValue(row.Index, Output.COLUMNSTATUSONOFF, GetString("OutOfRange"));
                                break;

                            default:
                                _cdgvData.UpdateValue(row.Index, Output.COLUMNSTATUSONOFF, GetString("Unknown"));
                                break;
                        }

                        _cdgvData.DataGrid.AutoResizeColumn(row.Cells[Output.COLUMNSTATUSONOFF].ColumnIndex);
                    }
                }
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
            DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            // Update current view
            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                OutputShort outputShort = (OutputShort)((BindingSource)_cdgvData.DataGrid.DataSource).List[row.Index];

                if (outputShort != null)
                {
                    if (outputShort.IdOutput == outputGuid)
                    {
                        outputShort.RealState = outputState;
                        if (!outputShort.RealStateChanges)
                        {
                            _cdgvData.UpdateValue(row.Index, Output.COLUMNREALSTATUSONOFF, GetString("ReportingSupressed"));
                        }
                        else
                        {
                            switch(outputState)
                            {
                                case OutputState.Off:
                                    _cdgvData.UpdateValue(row.Index, Output.COLUMNREALSTATUSONOFF, GetString("Off"));
                                    break;

                                case OutputState.On:
                                    _cdgvData.UpdateValue(row.Index, Output.COLUMNREALSTATUSONOFF, GetString("On"));
                                    break;

                                default:
                                    _cdgvData.UpdateValue(row.Index, Output.COLUMNREALSTATUSONOFF, GetString("ReportingSupressed"));
                                    break;
                            }
                        }

                        _cdgvData.DataGrid.AutoResizeColumn(row.Cells[Output.COLUMNREALSTATUSONOFF].ColumnIndex);
                    }
                }
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

            bool updated = false;
            _outputControllObjectsToState.TryGetValue(objectId,
                (key, found, value) =>
                {
                    if ((found && value != objectState)
                        || !found)
                    {
                        updated = true;
                        RuntimeFilterValueChanged(key, null);
                    }

                    _outputControllObjectsToState[key] = objectState;
                });

            if (objectType == ObjectType.Output)            
            {
                _shortOutputToFilter.TryGetValue(objectId,
                    (key, found, value) =>
                    {
                        if (found && value.ShortObject.State != (OutputState)state)
                        {
                            value.ShortObject.State = (OutputState)state;
                            updated = true;
                            RuntimeFilterValueChanged(key, null);
                        }
                    });
            }

            if (updated)
                RunFilter();
        }

        protected override ICollection<OutputShort> GetData()
        {
            Exception error;
            ICollection<OutputShort> resultList;

            if (_currentlyControlledFilterIndex > 0 || _currentStateFilterIndex > 0)
            {
                resultList = new List<OutputShort>();
                ICgpNCASRemotingProvider remoting = Plugin.MainServerProvider;

                ICollection<Output> tempList = remoting.Outputs.SelectByCriteria(out error, _filterSettingsJoinOperator, _filterSettingsWithJoin);

                _outputControllObjectsToState.Clear(
                    () =>
                    {
                        foreach (Output output in tempList)
                        {
                            OutputShort outputShort = new OutputShort(output);
                            ShortObjectExtension<OutputShort, Guid?> filterValues;
                            if (output.ControlType == 3 /*OutputControl.controledByObject */
                                && output.OnOffObjectId != null)
                            {
                                State state = Plugin.MainServerProvider.GetRealOnOffObjectState(output.OnOffObjectObjectType, output.OnOffObjectId.Value);//output.OnOffObject.State ? State.On : State.Off;
                                if (state == State.Unknown && output.OnOffObjectObjectType == ObjectType.DailyPlan)
                                {
                                    state = Plugin.MainServerProvider.GetRealOnOffObjectState(ObjectType.TimeZone, output.OnOffObjectId.Value);
                                }
                                _outputControllObjectsToState[output.OnOffObjectId.Value] = state;
                                filterValues = new ShortObjectExtension<OutputShort, Guid?>(outputShort, output.OnOffObjectId);
                            }
                            else
                                filterValues = new ShortObjectExtension<OutputShort, Guid?>(outputShort, null);

                            outputShort.State = remoting.Outputs.GetActualStatesByGuid(output.IdOutput);
                            outputShort.RealState = remoting.Outputs.GetRealStatesByGuid(output.IdOutput);
                            resultList.Add(outputShort);
                            _shortOutputToFilter[outputShort.IdOutput] = filterValues;
                        }
                    });

            }
            else
            {
                _outputControllObjectsToState.Clear();
                _shortOutputToFilter.Clear();
                resultList = Plugin.MainServerProvider.Outputs.ShortSelectByCriteria(out error, _filterSettingsJoinOperator, _filterSettingsWithJoin);
            }

            if (error != null)
                throw (error);

            CheckAccess();
            _lRecordCount.BeginInvoke(new Action(
            () =>
            {
                _lRecordCount.Text = string.Format("{0} : {1}",
                                                        GetString("TextRecordCount"),
                                                        resultList == null
                                                            ? 0
                                                            : resultList.Count);
            }));
            return resultList;
        }

        private void CheckAccess()
        {
        }

        protected override bool Compare(Output obj1, Output obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(Output obj, Guid idObj)
        {
            return obj.IdOutput == idObj;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _bindingSourceOutputs = bindingSource;

            _cdgvData.ModifyGridView(bindingSource, OutputShort.COLUMN_SYMBOL, OutputShort.COLUMNFULLNAME, OutputShort.COLUMNSTATUSONOFF, OutputShort.COLUMNREALSTATUSONOFF, OutputShort.COLUMNDESCRIPTION);
        }

        void DgValuesRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            BindingSource bs = (BindingSource)_cdgvData.DataGrid.DataSource;
            OutputShort outputShort = (OutputShort)bs.List[e.RowIndex];

            DataGridViewRow row = _cdgvData.DataGrid.Rows[e.RowIndex];
            if (row.Cells[InputShort.COLUMNSTATUSONOFF].Value == null)
            {
                if (outputShort != null)
                {
                    switch (outputShort.State)
                    {
                        case OutputState.On:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("On");
                            break;
                        case OutputState.Off:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("Off");
                            break;
                        case OutputState.UsedByAnotherAplication:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                            break;
                        case OutputState.OutOfRange:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                            break;
                        default:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                            break;
                    }

                    if (!outputShort.RealStateChanges)
                    {
                        row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                    }
                    else
                    {
                        switch (outputShort.RealState)
                        {
                            case OutputState.On:
                                row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("On");
                                break;
                            case OutputState.Off:
                                row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("Off");
                                break;
                            default:
                                row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                                break;
                        }
                    }
                }

                _cdgvData.DataGrid.AutoResizeColumn(row.Cells[Output.COLUMNREALSTATUSONOFF].ColumnIndex);
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.DataGrid.DataSource = null;
        }

        protected override ACgpPluginEditForm<NCASClient, Output> CreateEditForm(Output obj, ShowOptionsEditForm showOption)
        {
            return new NCASOutputEditForm(obj, showOption, this);
        }

        protected override void DeleteObj(Output obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.Outputs.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            _filterSettingsJoinOperator = (_rbFilterAnd.Checked ? LogicalOperators.AND : LogicalOperators.OR);
            foreach (IList<FilterSettings> list in _filterSettingsWithJoin)
                list.Clear();

            if (_eNameFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(Output.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }

            if (_cbInvertedFilter.SelectedIndex > 0)
            {
                bool inverted = (_cbInvertedFilter.SelectedIndex == 1);
                FilterSettings filterSetting = new FilterSettings(Output.COLUMNINVERTED, inverted, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            if (_cbOutputTypeFilter.SelectedIndex > 0)
            {
                byte type = (byte)(_cbOutputTypeFilter.SelectedIndex - 1);
                FilterSettings filterSetting = new FilterSettings(Output.COLUMNOUTPUTTYPE, type, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            if (_cbEnableAlarmsFilter.SelectedIndex > 0)
            {
                bool value = (_cbEnableAlarmsFilter.SelectedIndex == 1);
                FilterSettings filterSetting = new FilterSettings(Output.COLUMNALARMCONROLBYOBJON, value, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            foreach(var item in _clbOutputControlFilter.Items)
            {
                if (item != null)
                {
                    byte value = (byte)_clbOutputControlFilter.Items.IndexOf(item);
                    if (_clbOutputControlFilter.GetItemCheckState(value) == CheckState.Checked)
                    {
                        FilterSettings filterSetting = new FilterSettings(Output.COLUMNCONTROLTYPE, value, ComparerModes.EQUALL, LogicalOperators.OR);
                        _filterSettingsWithJoin[1].Add(filterSetting);
                    }
                }
            }

            if (_tbmPresentationGroupFilter.Tag != null)
            {
                ListOfObjects list = _tbmPresentationGroupFilter.Tag as ListOfObjects;
                if (list != null)
                    foreach (PresentationGroup item in list)
                        if (item != null)
                        {
                            //Guid value = (Guid)item.GetId();
                            FilterSettings filterSetting = new FilterSettings(Output.COLUMNALARMPRESENTATIONGROUP, item, ComparerModes.EQUALL, LogicalOperators.OR);
                            _filterSettingsWithJoin[2].Add(filterSetting);
                        }
            }
            if (_tbmControlledByFilter.Tag != null)
            {
                ListOfObjects list = _tbmControlledByFilter.Tag as ListOfObjects;
                if (list != null)
                    foreach (AOrmObject item in list)
                        if (item != null)
                        {
                            Guid value = (Guid)item.GetId();
                            FilterSettings filterSetting = new FilterSettings(Output.COLUMNONOFFOBJECTID, value, ComparerModes.EQUALL, LogicalOperators.OR);
                            _filterSettingsWithJoin[3].Add(filterSetting);
                        }
            }
            
            _currentlyControlledFilterIndex = _cbCurrentlyControlledFilter.SelectedIndex;
            _currentStateFilterIndex = _cbCurrentStateFilter.SelectedIndex;
        }

        protected override void ClearFilterEdits()
        {
            _filterSettingsJoinOperator = LogicalOperators.AND;
            _rbFilterAnd.Checked = true;

            foreach (var filter in _filterSettingsWithJoin)
                filter.Clear();

            _eNameFilter.Text = "";
            _cbCurrentlyControlledFilter.SelectedIndex = 0;
            _cbCurrentStateFilter.SelectedIndex = 0;
            _currentlyControlledFilterIndex = 0;
            _currentStateFilterIndex = 0;
            _cbEnableAlarmsFilter.SelectedIndex = 0;
            _cbOutputTypeFilter.SelectedIndex = 0;
            _cbInvertedFilter.SelectedIndex = 0;
            for (int i = 0; i < _clbOutputControlFilter.Items.Count; i++)
            {
                _clbOutputControlFilter.SetItemChecked(i, false);
            }

            SetSelectedObjects(_tbmControlledByFilter, null);
            SetSelectedObjects(_tbmPresentationGroupFilter, null);
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            //_bRunFilter_Click(sender, e);
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Outputs.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(Output output)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Outputs.HasAccessViewForObject(output);
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
                    return Plugin.MainServerProvider.Outputs.HasAccessInsert();
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
                    return Plugin.MainServerProvider.Outputs.HasAccessDelete();
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

        private void _bPreCreateOutput_Click(object sender, EventArgs e)
        {
            Insert();
        }

        private void _bPrecreateCCUOutput_Click(object sender, EventArgs e)
        {
            if (_outputInsertForm == null || _outputInsertForm.Visible == false)
                _outputInsertForm = new NCASOutputEditForm(new Output(), ShowOptionsEditForm.Insert, this);
            else
            {
                _outputInsertForm.Focus();
                return;
            }

            _outputInsertForm.CcuOutput = true;
            _outputInsertForm.Show();

            
        }

        private void _tbmControlledByFilter_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify2)
            {
                ModifyControlledByObjects();
            }
            else if (item == _tsiRemove2)
            {
                SetSelectedObjects(_tbmControlledByFilter, null);
            }
        }

        private void _tbmControlledByFilter_DragOver(object sender, DragEventArgs e)
        {
            AOnOffObject data = GetObjectFromDragDrop<AOnOffObject>(e);

            if (data != null)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void _tbmControlledByFilter_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                AOnOffObject data = GetObjectFromDragDrop<AOnOffObject>(e);

                if (data != null)
                {
                    List<object> tempList = new List<object>();
                    tempList.Add(data);
                    SetSelectedObjects(_tbmControlledByFilter, new ListOfObjects(tempList));
                }
            }
            catch
            {
            }
        }

        private void _tbmPresentationGroupFilter_ButtonPopupMenuItemClick(ToolStripItem item, int index)
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

        private void _tbmPresentationGroupFilter_DragOver(object sender, DragEventArgs e)
        {
            PresentationGroup data = GetObjectFromDragDrop<PresentationGroup>(e);

            if (data != null)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void _tbmPresentationGroupFilter_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                PresentationGroup pg = GetObjectFromDragDrop<PresentationGroup>(e);
                if (pg != null)
                {
                    List<object> tempList = new List<object>();
                    tempList.Add(pg);
                    SetSelectedObjects(_tbmPresentationGroupFilter, new ListOfObjects(tempList));
                }
            }
            catch
            {
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

        private void ModifyControlledByObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error;
            try
            {
                ConnectionLost();

                List<IModifyObject> listObjects = new List<IModifyObject>();

                IList<IModifyObject> listTimeZonesFromDatabase = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;

                if (listTimeZonesFromDatabase != null && listTimeZonesFromDatabase.Count > 0)
                    listObjects.AddRange(listTimeZonesFromDatabase);

                IList<IModifyObject> listDailyPlansFromDatabase = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlansFromDatabase);
                if (listDailyPlansFromDatabase != null && listDailyPlansFromDatabase.Count > 0)
                {
                    listObjects.AddRange(listDailyPlansFromDatabase);
                }

                IList<FilterSettings> filterSettings = new List<FilterSettings>();
                IList<IModifyObject> listInputsFromDatabase = null;

                listInputsFromDatabase = Plugin.MainServerProvider.Inputs.ModifyObjectsSelectByCriteria(filterSettings, out error);

                if (error != null)
                    throw error;

                if (listInputsFromDatabase != null && listInputsFromDatabase.Count > 0)
                    listObjects.AddRange(listInputsFromDatabase);

                IList<IModifyObject> listOutputsFromDatabase = null;

                listOutputsFromDatabase = Plugin.MainServerProvider.Outputs.ListModifyObjects(out error);

                if (error != null) throw error;

                if (listOutputsFromDatabase != null && listOutputsFromDatabase.Count > 0)
                {
                    listObjects.AddRange(listOutputsFromDatabase);
                }

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, GetString("NCASInputEditFormListObjectForBlockedInputText"));

                ListOfObjects outObjects;
                formAdd.ShowDialogMultiSelect(out outObjects);
                for (int i = 0; i < outObjects.Count; i++)
                {
                    IModifyObject modifyObject = outObjects[i] as IModifyObject;
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
                    SetSelectedObjects(_tbmControlledByFilter, outObjects);
                }
            }
            catch
            {
            }
        }

        private void ModifyPresentationGroupObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                ConnectionLost();

                List<AOrmObject> listObjects = new List<AOrmObject>();

                ICollection<PresentationGroup> listPresentationGroup = CgpClient.Singleton.MainServerProvider.PresentationGroups.List(out error);
                if (error != null) throw error;

                if (listPresentationGroup != null && listPresentationGroup.Count > 0)
                    foreach (PresentationGroup pg in listPresentationGroup)
                        listObjects.Add(pg);

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, GetString("NCASInputsForm_lPresentationGroupFilter"));

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

        private void SetSelectedObjects(TextBoxMenu control, ListOfObjects objects)
        {
            control.Tag = objects;
            RefreshSelectedObject(control);
        }

        private void RefreshSelectedObject(TextBoxMenu control)
        {
            ListOfObjects actOnOffObjects = control.Tag as ListOfObjects;
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
                    AOrmObject tempAOrm = actOnOffObjects[0] as AOrmObject;
                    if (tempAOrm != null)
                    {
                        ListObjFS listFS = new ListObjFS(tempAOrm);
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

        protected override void ApplyRuntimeFilter()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ApplyRuntimeFilter));
                return;
            }

            CurrencyManager currencyManager = (CurrencyManager)BindingContext[_cdgvData.DataGrid.DataSource];
            currencyManager.SuspendBinding();

            // Runtime filters is not used
            if (_currentlyControlledFilterIndex < 1 && _currentStateFilterIndex < 1)
            {
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    if (!row.Visible)
                        row.Visible = true;
                }
                currencyManager.ResumeBinding();
                _lRecordCount.Text = GetString("NCASOutputsForm_lRecordCount") + ": " + _cdgvData.DataGrid.Rows.Count;
                return;
            }

            DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            int visibleCount = 0;

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                OutputShort outputShort = (OutputShort)_bindingSourceOutputs.List[row.Index];

                if (outputShort != null)
                {
                    ShortObjectExtension<OutputShort, Guid?> filterValues;
                    if (_shortOutputToFilter.TryGetValue(outputShort.IdOutput, out filterValues))
                    {
                        // Apply current output state filter
                        if (_currentStateFilterIndex < 1
                            || (_currentStateFilterIndex - 1 <= (int)OutputState.OutOfRange
                                && outputShort.State == (OutputState)(_currentStateFilterIndex - 1))
                            || ((_currentStateFilterIndex == 5)
                                && outputShort.State == OutputState.Unknown))
                        {
                            // Apply currently controlled filter
                            if (filterValues.Value != null)
                            {
                                if (filterValues.ShortObject == null)
                                {
                                    if (row.Visible)
                                        row.Visible = false;

                                    continue;
                                }

                                // Get controlled by object state
                                _outputControllObjectsToState.TryGetValue(filterValues.Value.Value,
                                        (key, found, value) =>
                                        {
                                            if (found)
                                            {
                                                // Add currently controlled to off
                                                if (_currentlyControlledFilterIndex == 1
                                                    && value == State.Off)
                                                {
                                                    visibleCount++;

                                                    if(!row.Visible)
                                                        row.Visible = true;
                                                }
                                                // Add currently controlled to on
                                                else if (_currentlyControlledFilterIndex == 2
                                                    && value == State.On)
                                                {
                                                    visibleCount++;

                                                    if (!row.Visible)
                                                        row.Visible = true;
                                                }
                                                else if (_currentlyControlledFilterIndex < 1)
                                                {
                                                    visibleCount++;

                                                    if (!row.Visible)
                                                        row.Visible = true;
                                                }
                                                else
                                                {
                                                    if(row.Visible)
                                                        row.Visible = false;
                                                }
                                            }
                                            else
                                            {
                                                if (row.Visible)
                                                    row.Visible = false;
                                            }
                                        });
                            }
                            else
                            {
                                visibleCount++;

                                if (!row.Visible)
                                    row.Visible = true;
                            }
                        }
                        else
                        {
                            if (row.Visible)
                                row.Visible = false;
                        }
                    }
                    else
                    {
                        if (row.Visible)
                            row.Visible = false;
                    }
                }
            }
            currencyManager.ResumeBinding();

            _lRecordCount.Text = GetString("NCASOutputsForm_lRecordCount") + ": " + visibleCount + "/" + _cdgvData.DataGrid.Rows.Count;

            if (dgcell != null && dgcell.Visible)
            {
                _cdgvData.DataGrid.CurrentCell = dgcell;
            }
        }

        private void _clbOutputControlFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            FilterValueChanged(sender, e);
        }
    }
}
