using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick.Localization;
using Contal.Cgp.Globals;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class EventlogParametersForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<Eventlog>
#endif
    {
        private BindingSource _bindingSource = null;
        private bool _runShowData = false;

        public EventlogParametersForm(Eventlog eventlog)
            : base(eventlog, ShowOptionsEditForm.Edit)
        {
            InitializeComponent();
            _ilbEventSource.ImageList = ObjectImageList.Singleton.GetAllObjectImages();           
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            //Exception error;
            //Eventlog obj = CgpClient.Singleton.MainServerProvider.Eventlogs.GetObjectById(_editingObject.IdEventlog, out error);

            allowEdit = false;
            DisabledForm();
            //_editingObject = obj;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
            Eventlog obj = 
                CgpClient.Singleton.MainServerProvider.Eventlogs.GetObjectById(
                    _editingObject.IdEventlog);

            _editingObject = obj;
        }

        protected override void SetValuesInsert()
        {
            throw new NotImplementedException();
        }

        protected override void SetValuesEdit()
        {
            _eELType.Text = _editingObject.Type;
            _eELDate.Text = _editingObject.EventlogDateTime.ToString();
            _eELCGPSource.Text = _editingObject.CGPSource;

            //if (!string.IsNullOrEmpty(_editingObject.EventSource))
            //    _eELEventSource.Text = _editingObject.EventSource;
            //else
            //    _eELEventSource.Text = "";

            //if (!string.IsNullOrEmpty(_editingObject.Localization))
            //    _eELLocalization.Text = _editingObject.Localization;
            //else
            //    _eELLocalization.Text = "";

            _eELDescription.Text = _editingObject.Description;

            ShowData();
        }

        protected override void AfterTranslateForm()
        {
            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }


        protected override void DisabledForm()
        {
        }

        protected override void BeforeInsert()
        {
            throw new NotImplementedException();
        }

        protected override void BeforeEdit()
        {
            EventlogsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            throw new NotImplementedException();
        }

        protected override void AfterEdit()
        {
            EventlogsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override bool CheckValues()
        {
            throw new NotImplementedException();
        }

        protected override bool GetValues()
        {
            throw new NotImplementedException();
        }

        protected override bool SaveToDatabaseInsert()
        {
            throw new NotImplementedException();
        }

        protected override bool SaveToDatabaseEdit()
        {
            throw new NotImplementedException();
        }

        private void EventlogParametersForm_Enter(object sender, EventArgs e)
        {
            CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        private void EventlogParametersForm_Leave(object sender, EventArgs e)
        {
            CgpClientMainForm.Singleton.StopProgress(this);
        }

        #region ShowDataGrid
        private void ShowData()
        {
            ShowDataThread();
        }

        private void ShowDataThread()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (_runShowData) return;
            _runShowData = true;
            CgpClientMainForm.Singleton.StartProgress(this);
            IwQuick.Threads.SafeThread.StartThread(LoadEventSources);
            IwQuick.Threads.SafeThread.StartThread(LoadTable);
        }

        private void LoadEventSources()
        {
            if (CgpClient.Singleton.MainServerProvider == null || 
                    CgpClient.Singleton.IsConnectionLost(false))
                return;

            Eventlog eventlog =
                CgpClient.Singleton.MainServerProvider.Eventlogs
                    .GetObjectById(_editingObject.IdEventlog);

            if (eventlog == null)
                return;

            ICollection<EventSource> eventSources = 
                eventlog.EventSources;

            ShowLoadEventSources(eventSources);
        }

        private void ShowLoadEventSources(ICollection<EventSource> eventSources)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<ICollection<EventSource>>(ShowLoadEventSources), 
                    eventSources);

                return;
            }

            if (eventSources == null || eventSources.Count == 0)
            {                 
                _ilbEventSource.BackColor = Color.White;
                _ilbEventSource.ForeColor = Color.Black;

                return;
            }

            _ilbEventSource.Items.Clear();

            _ilbEventSource.BackColor = CgpClientMainForm.Singleton.GetReferenceBackgroundColor;
            _ilbEventSource.ForeColor = CgpClientMainForm.Singleton.GetReferenceTextColor;

            var eventSourceObjectGuids = 
                eventSources.Select(eventSource => eventSource.EventSourceObjectGuid);

            var eventSourceObjectGuidsAndTypes =
                eventSourceObjectGuids.Select(
                    eventSourceObjectGuid =>
                        new
                        {
                            eventSourceObjectGuid,
                            objType =
                                CgpClient.Singleton.MainServerProvider.CentralNameRegisters
                                    .GetObjectTypeFromGuid(eventSourceObjectGuid)
                        });

            IList<ClAOrmObjectPriority> listEventSources =
                eventSourceObjectGuidsAndTypes
                    .Select(
                        eventSourceGuidAndType =>
                            new ClAOrmObjectPriority(
                                DbsSupport.GetTableObject(
                                    eventSourceGuidAndType.objType,
                                    eventSourceGuidAndType.eventSourceObjectGuid),
                                eventSourceGuidAndType.objType))
                    .ToList();

            listEventSources = 
                listEventSources
                    .OrderByDescending(x => x._priority)
                    .ToList();

            foreach (ClAOrmObjectPriority cObj in listEventSources)
            {
                AOrmObject ormObject = cObj._ormObj;

                _ilbEventSource.Items.Add(
                    new ImageListBoxItem(
                        ormObject,
                        ormObject == null ? ObjectType.NotSupport.ToString() : ormObject.GetObjectType().ToString()));
            }
        }

        private void LoadTable()
        {
            _runShowData = false;

            if (CgpClient.Singleton.MainServerProvider == null || 
                    CgpClient.Singleton.IsConnectionLost(false))
                return;

            Eventlog eventlog = 
                CgpClient.Singleton.MainServerProvider.Eventlogs
                    .GetObjectById(_editingObject.IdEventlog);

            if (eventlog == null)
                return;

            ICollection<EventlogParameter> eventParameters =
                eventlog.EventlogParameters;

            UpdateGridView(eventParameters);
        }

        private void UpdateGridView(ICollection<EventlogParameter> listEventlogParameters)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ICollection<EventlogParameter>>(UpdateGridView), 
                    listEventlogParameters);

                return;
            }

            int lastPosition = 0;

            if (_bindingSource != null && _bindingSource.Count != 0)
                lastPosition = _bindingSource.Position;

            _bindingSource = 
                new BindingSource
                {
                    DataSource = listEventlogParameters
                };

            if (_bindingSource.DataSource == null ||
                _bindingSource.Count <= 0)
            {
                CgpClientMainForm.Singleton.StopProgress(this);
                return;
            }

            if (lastPosition != 0)
                _bindingSource.Position = lastPosition;

            _bindingSource.AllowNew = false;
            _cdgvData.DataGrid.DataSource = _bindingSource;

            if (!_cdgvData.DataGrid.Columns.Contains("strValue"))
            {
                int columnIdx = _cdgvData.DataGrid.Columns.Add("strValue", "strValue");

                DataGridViewColumn newColumn = _cdgvData.DataGrid.Columns[columnIdx];

                var dataGridViewColumnValue = _cdgvData.DataGrid.Columns["Value"];

                if (dataGridViewColumnValue != null)
                    newColumn.DisplayIndex =
                        dataGridViewColumnValue.DisplayIndex;

                newColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            HideColumnDgw("Value");
            HideColumnDgw("IdEventlogParameter");
            HideColumnDgw("Eventlog");
            HideColumnDgw("TypeGuid");
            HideColumnDgw("TypeObjectType");

            _cdgvData.DataGrid.AutoGenerateColumns = false;
            _cdgvData.DataGrid.AllowUserToAddRows = false;

            CgpClient.Singleton.LocalizationHelper
                .TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                var eventlogParameter =
                    (EventlogParameter)
                        ((BindingSource)_cdgvData.DataGrid.DataSource).List[
                            row.Index];

                switch (eventlogParameter.TypeObjectType)
                {
                    case (byte)ObjectType.CardReader:
                        try
                        {
                            AOrmObject aOrm =
                                DbsSupport.GetTableObject(
                                    ObjectType.CardReader,
                                    eventlogParameter.TypeGuid.ToString());

                            row.Cells["strValue"].Value = 
                                aOrm != null 
                                    ? aOrm.ToString() 
                                    : row.Cells["Value"].Value;
                        }
                        catch
                        {
                            row.Cells["strValue"].Value =
                                row.Cells["Value"].Value;
                        }
                        break;

                    case (byte)ObjectType.Input:
                        try
                        {
                            AOrmObject aOrm =
                                DbsSupport.GetTableObject(
                                    ObjectType.Input,
                                    eventlogParameter.TypeGuid.ToString());

                            row.Cells["strValue"].Value = 
                                aOrm != null 
                                    ? aOrm.ToString() 
                                    : row.Cells["Value"].Value;
                        }
                        catch
                        {
                            row.Cells["strValue"].Value =
                                row.Cells["Value"].Value;
                        }
                        break;

                    default:
                        row.Cells["strValue"].Value = row.Cells["Value"].Value;
                        break;
                }
            }

            CgpClientMainForm.Singleton.StopProgress(this);
        }

        #endregion

        #region Connection

        void ConnectionLost(Type type)
        {
            CgpClientMainForm.Singleton.StopProgress(this);
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RemoveDataSource));
            }
            else
            {
                RemoveDataSource();
            }
        }

        private void RemoveDataSource()
        {
            _cdgvData.DataGrid.DataSource = null;
            _bindingSource = null;
        }

        #endregion
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            EventlogsForm.Singleton.BlockShowData();
            Close();
            EventlogsForm.Singleton.UnblockShowData();
        }

        public void HideColumnDgw(string columnName)
        {
            if (_cdgvData.DataGrid == null) return;
            if (_cdgvData.DataGrid.Columns.Contains(columnName))
                _cdgvData.DataGrid.Columns[columnName].Visible = false;
        }

        protected override void EditEnd()
        {
        }

        private void _ilbEventSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            object selectedObject = _ilbEventSource.SelectedItemObject;
            if (selectedObject is AOrmObject)
            {
                DbsSupport.OpenEditForm(selectedObject as AOrmObject);
            }
        }
    }

    public class ClAOrmObjectPriority
    {
        public AOrmObject _ormObj;
        public byte _priority;

        public ClAOrmObjectPriority(AOrmObject ormObj, ObjectType objType)
        {
            _ormObj = ormObj;
            _priority = Cgp.Globals.ObjTypeHelper.GetObjectTypePriority((byte)objType);
        }
    }
}
