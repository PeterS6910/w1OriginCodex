using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class SystemEventsForm : ACgpFullscreenForm
    {
        private BindingSource _bindingSource = null;
        private bool _runShowData = false;
        //ICollection<Cgp.Server.Beans.SystemEvent> _listSystemEvents;
        //private bool _refreshData = false;
        private bool _freeInsert = true;
        private List<string> _listActivatedSystemEvents = new List<string>();

        public SystemEventsForm()
        {
            InitializeComponent();
            RegisterToMain();
        }

        private static SystemEventsForm _singleton;
        public static SystemEventsForm Singleton
        {
            get
            {
                if (null == _singleton)
                {
                    _singleton = new SystemEventsForm();
                    _singleton.MdiParent = CgpClientMainForm.Singleton;
                }
                return _singleton;
            }
        }

        #region CRUP Presentation Groups

        private void _bInsert_Click(object sender, EventArgs e)
        {
            if (!_freeInsert)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertNotFree"));
                return;
            }
            _freeInsert = false;
            InsertSystemEvent();
            ShowDataThread();
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            UpdateSystemEvent();
            _dgValues.Refresh();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bindingSource == null || _bindingSource.Count == 0) return;
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                SystemEvent systemEvent = (SystemEvent)_bindingSource.List[_bindingSource.Position];
                if (IsInActivatedList(systemEvent))
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteEditing"));
                    return;
                }
                if (Contal.IwQuick.UI.Dialog.Question(GetString("SystemEventsForm.DeleteConfirm")))
                {
                    DeleteSystemEvent();
                    ShowDataThread();
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("SystemEventsForm.DeleteFail"));
            }
        }

        private void InsertSystemEvent()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;
                SystemEvent newSystemEvent = new SystemEvent();
                SystemEventsEditForm editSEForm = new SystemEventsEditForm(newSystemEvent, true);
                editSEForm.Show();
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertSystemEvent"));
            }
        }

        private void UpdateSystemEvent()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            SystemEvent newSystemEvent = (SystemEvent)_bindingSource.List[_bindingSource.Position];
            if (IsInActivatedList(newSystemEvent))
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAlreadyEdited"));
                return;
            }
            AddToEditList(newSystemEvent);

            try
            {
                newSystemEvent.ToString();
            }
            catch
            {
                RefreshData();
                newSystemEvent = (SystemEvent)_bindingSource.List[_bindingSource.Position];
            }

            var ws = this.WindowState;
            SystemEventsEditForm editSEForm = new SystemEventsEditForm(newSystemEvent, false);
            CgpClientMainForm.Singleton.AddToRecentList(newSystemEvent, editSEForm);
            editSEForm.Show();
            editSEForm.WindowState = ws;
        }

        private void DeleteSystemEvent()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            SystemEvent systemEvent = (SystemEvent)_bindingSource.List[_bindingSource.Position];
            if (!CgpClient.Singleton.MainServerProvider.SystemEvents.Delete(systemEvent))
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteFailed"));
        }

        #endregion

        #region RefreshDataGrid
        private void ShowDataThread()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (_runShowData) return;
            _runShowData = true;
            CgpClientMainForm.Singleton.StartProgress();
            Contal.IwQuick.Threads.SafeThread.StartThread(new Contal.IwQuick.DFromVoidToVoid(LoadTable));
        }

        private void LoadTable()
        {
            _runShowData = false;
            UpdateGridView(CgpClient.Singleton.MainServerProvider.SystemEvents.List());
        }

        private void UpdateGridView(ICollection<Cgp.Server.Beans.SystemEvent> listSysEvent)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DFromTToVoid<ICollection<Cgp.Server.Beans.SystemEvent>>(UpdateGridView), listSysEvent);
            }
            else
            {
                int lastPosition = 0;
                if (_bindingSource != null && _bindingSource.Count != 0)
                    lastPosition = _bindingSource.Position;
                _bindingSource = new BindingSource();
                _bindingSource.DataSource = listSysEvent;
                if (lastPosition != 0) _bindingSource.Position = lastPosition;
                _dgValues.DataSource = _bindingSource;
                HideColumnDgw(_dgValues, "IdSystemEvent");
                HideColumnDgw(_dgValues, "PresentationGroups");
                CgpClient.Singleton.LocalizationHelper.TranslateDataGridView(_dgValues);
                CgpClientMainForm.Singleton.StopProgress();
            }
        }
        #endregion



        private void RefreshData()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            int position = _bindingSource.Position;
            try
            {
                UpdateGridView(CgpClient.Singleton.MainServerProvider.SystemEvents.List());
            }
            catch
            {
            }
            _bindingSource.Position = position;
        }

        public void RefreshData(bool insert, SystemEvent systemEvent)
        {
            if (insert)
            {
                _freeInsert = true;
                ShowDataThread();
            }
            else
            {
                RemoveFromEditList(systemEvent);
            }
            _dgValues.Refresh();
        }

        public void AddToEditList(SystemEvent systemEvent)
        {
            _listActivatedSystemEvents.Add(systemEvent.Name.ToString());
        }

        private void RemoveFromEditList(SystemEvent systemEvent)
        {
            _listActivatedSystemEvents.Remove(systemEvent.Name.ToString());
        }

        private bool IsInActivatedList(SystemEvent systemEvent)
        {
            return _listActivatedSystemEvents.Contains(systemEvent.Name.ToString());
        }


        #region Connection

        protected override bool VerifySources()
        {
            return null != CgpClient.Singleton.MainServerProvider;
        }

        void ConnectionLost(Type type)
        {
            CgpClientMainForm.Singleton.StopProgress();
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DFromVoidToVoid(RemoveDataSource));
            }
            else
            {
                RemoveDataSource();
            }
        }

        private void RemoveDataSource()
        {
            _dgValues.Visible = false;
            _dgValues.DataSource = null;
            _bindingSource = null;
            _dgValues.Visible = true;
        }

        void ConnectionObtain(Contal.Cgp.RemotingCommon.ICgpServerRemotingProvider parameter)
        {
            ShowDataThread();
        }
        #endregion

        private void SystemEventsForm_Enter(object sender, EventArgs e)
        {
            _dgValues.Visible = false;
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                _dgValues.Visible = true;
                ShowDataThread();
            }
            else
            {
                RemoveDataSource();
            }

            CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        private void SystemEventsForm_Leave(object sender, EventArgs e)
        {
            CgpClientMainForm.Singleton.StopProgress();
            CgpClientMainForm.Singleton.ModifyOpenWindowImage(this);
        }

        private void _dgValues_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            UpdateSystemEvent();
            _dgValues.Refresh();
        }

    }
}
