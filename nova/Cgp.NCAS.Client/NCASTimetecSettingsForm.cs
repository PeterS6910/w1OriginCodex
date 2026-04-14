using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using WcfServiceNovaConnection;
using RequiredLicenceProperties = Contal.Cgp.NCAS.Globals.RequiredLicenceProperties;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASTimetecSettingsForm :
#if DESIGNER
        Form
#else
 ACgpPluginFullscreenForm<NCASClient>
#endif
    {
        private const string DEFAULT_PASSWORD = "*****";

        private static volatile NCASTimetecSettingsForm _singleton;
        private static readonly object SyncRoot = new object();

        private TimetecSetting _timetecSetting;
        private bool _needLoaded = true;
        private bool _allowEdit;
        private ICollection<TimetecErrorEvent> _errorTimetecEvents;

        public static NCASTimetecSettingsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (SyncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASTimetecSettingsForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public NCASTimetecSettingsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            FormImage = ResourceGlobal.Timetec;
            FormOnEnter += Form_Enter;

            _wpfUsedCardReadersList.Icons = Plugin.GetPluginObjectsImages();
            _wpfUsedCardReadersList.EnableDeleteButtons = true;
            _wpfUsedCardReadersList.EnableInsertButton = true;
            _wpfUsedCardReadersList.InsertAction += _wpfUsedCardReadersList_InsertAction;
            _wpfUsedCardReadersList.DeleteAction += _wpfUsedCardReadersList_DeleteAction;

            TimetecCommunicationOnlineStateChangedHandler.Singleton.RegisterStateChanged(TimetecCommunicationOnlineState);

            var imageList = new ImageList();
            imageList.Images.Add(ObjectType.Eventlog.ToString(), EventlogsForm.Singleton.Icon);

            _wpfErrorEvents.Icons = imageList;
            _wpfErrorEvents.EnableCheckboxes = true;
            _wpfErrorEvents.EnableDeleteButtons = false;
            _wpfErrorEvents.EnableInsertButton = false;
            _wpfErrorEvents.DefaultCheckBoxesState = false;
        }

        private void TimetecCommunicationOnlineState(bool? isConnected)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<bool?>(TimetecCommunicationOnlineState),
                    isConnected);

                return;
            }

            if (!isConnected.HasValue)
            {
                _tbTimetecConnectivityState.Text = GetString("ObjectState_Unknown");
                _tbTimetecConnectivityState.BackColor = Color.LightGray;
            }
            else if (isConnected.Value)
            {
                _tbTimetecConnectivityState.Text = GetString("ObjectState_Online");
                _tbTimetecConnectivityState.BackColor = Color.SpringGreen;
            }
            else
            {
                _tbTimetecConnectivityState.Text = GetString("ObjectState_Offline");
                _tbTimetecConnectivityState.BackColor = Color.OrangeRed;
            }

            SetDefaultDateTimeMode(isConnected);
        }

        private void Form_Enter(Form form)
        {
            if (!_needLoaded)
                return;

            LoadData();
            _needLoaded = false;

            if (CgpClient.Singleton.IsLoggedIn)
                TimetecCommunicationOnlineState(Plugin.MainServerProvider.TimetecSettings.IsConnected());
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _needLoaded = true;

            if (CgpClient.Singleton.IsLoggedIn)
                Plugin.MainServerProvider.TimetecSettings.EditEnd();

            base.OnFormClosing(e);
        }

        private void LoadData()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                string errorMsg;

                if (!CheckLicense(out errorMsg))
                {
                    Dialog.Error(errorMsg);
                    return;
                }

                _timetecSetting = Plugin.MainServerProvider.TimetecSettings.LoadTimetecSetting(out _allowEdit);

                _chbEnableCommunication.Checked = _timetecSetting.IsEnabled;
                _tbIpAddress.Text = _timetecSetting.IpAddress;
                _nPort.Value = _timetecSetting.Port;
                _tbUserName.Text = _timetecSetting.LoginName;
                _chbDoNotImportDepartments.Checked = _timetecSetting.DoNotImportDepartments;

                _mtbPassword.Text = string.IsNullOrEmpty(_timetecSetting.LoginPassword)
                    ? string.Empty
                    : DEFAULT_PASSWORD;

                _wpfUsedCardReadersList.Items =
                    _timetecSetting.CardReaders.Select(cr => new ShortObject(
                        cr.IdCardReader,
                        cr.Name,
                        ObjectType.CardReader))
                        .Cast<IShortObject>()
                        .ToList();

                _bSave.Enabled = false;

                RefreshTimtetecErrorEventsList();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void SetDefaultDateTimeMode(bool? isConnected)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                var defaultStartDateTime = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day);

                _dtpDefaultStartDateTime.Value = _timetecSetting != null &&
                                                 _timetecSetting.DefaultStartDateTime.HasValue
                    ? _timetecSetting.DefaultStartDateTime.Value
                    : defaultStartDateTime;

                var enabled = !isConnected.HasValue || !isConnected.Value;

                if (Plugin.MainServerProvider.TimetecSettings.LastEventId == -1)
                {
                    _bResetEventlogLastId.Enabled = false;
                    _dtpDefaultStartDateTime.Enabled = enabled;
                }
                else
                {
                    _bResetEventlogLastId.Enabled = enabled;
                    _dtpDefaultStartDateTime.Enabled = false;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private bool CheckData()
        {
            try
            {
                IwQuick.Net.IPHelper.CheckValidity(_tbIpAddress.Text);
            }
            catch
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbIpAddress,
                    GetString("ErrorWrongIPAddress"), ControlNotificationSettings.Default);

                return false;
            }

            if (string.IsNullOrEmpty(_tbUserName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbUserName,
                    GetString("ErrorUsernameCanNotBeEmpty"), ControlNotificationSettings.Default);

                return false;
            }

            if (string.IsNullOrEmpty(_mtbPassword.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _mtbPassword,
                    GetString("ErrorPasswordCanNotBeEmpty"), ControlNotificationSettings.Default);

                return false;
            }

            if (_wpfUsedCardReadersList.Items.Any(
                item => _wpfUsedCardReadersList.Items.Any(
                    item2 => item2.Name == item.Name && item2.Id != item.Id)))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, elementHost,
                    GetString("ErrorSomeCardReadersHaveTheSameName"), ControlNotificationSettings.Default);

                return false;
            }

            return true;
        }

        private void SetData()
        {
            _timetecSetting.IsEnabled = _chbEnableCommunication.Checked;
            _timetecSetting.IpAddress = _tbIpAddress.Text;
            _timetecSetting.Port = (int) _nPort.Value;
            _timetecSetting.LoginName = _tbUserName.Text;
            _timetecSetting.DoNotImportDepartments = _chbDoNotImportDepartments.Checked;

            if (_mtbPassword.Text != DEFAULT_PASSWORD)
                _timetecSetting.LoginPassword = IwQuick.Crypto.QuickCrypto.Encrypt(
                    _mtbPassword.Text,
                    CgpServerGlobals.DATABASE_KEY.ToString());

            _timetecSetting.DefaultStartDateTime = _dtpDefaultStartDateTime.Value;

            _timetecSetting.CardReaders= new List<CardReader>();

            foreach (var item in _wpfUsedCardReadersList.Items)
            {
                var cardReader = Plugin.MainServerProvider.CardReaders.GetObjectById(item.Id);
                _timetecSetting.CardReaders.Add(cardReader);
            }
        }

        void _wpfUsedCardReadersList_DeleteAction(ICollection<IShortObject> items)
        {
            if (!Dialog.WarningQuestion(GetString("QuestionRemoveSelectedCardReaders")))
                return;

            foreach (var item in items)
                _wpfUsedCardReadersList.Items.Remove(item);

            _wpfUsedCardReadersList.UpdateItems();
            _bSave.Enabled = _allowEdit;
        }

        void _wpfUsedCardReadersList_InsertAction()
        {
            Exception error;

            var listCrModObj = Plugin.MainServerProvider.CardReaders.ListModifyObjects(
                    true,
                    out error);

            if (error != null)
                throw error;

            var actualCrIds = _wpfUsedCardReadersList.Items.Select(item => item.Id);
            listCrModObj = listCrModObj.Where(obj => !actualCrIds.Contains(obj.GetId)).ToList();

            var formAdd = new ListboxFormAdd(listCrModObj, GetString("NCASCardReadersFormNCASCardReadersForm"));
            ListOfObjects outCardReaders;
            formAdd.ShowDialogMultiSelect(out outCardReaders);

            if (outCardReaders == null)
                return;

            _wpfUsedCardReadersList.Items.AddRange(outCardReaders.Objects.Cast<AModifyObject>().Select
                (obj => new ShortObject(obj.GetId, obj.FullName, ObjectType.CardReader))
                .Cast<IShortObject>().ToList());

            _wpfUsedCardReadersList.UpdateItems();
            _bSave.Enabled = _allowEdit;
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                {
                    string errMsg;

                    return CheckLicense(out errMsg)
                           && Plugin.MainServerProvider.TimetecSettings.HasAccessView();
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private class ShortObject : IShortObject
        {
            public object Id { get; private set; }
            public string Name { get; private set; }
            public ObjectType ObjectType { get; private set; }

            public string GetSubTypeImageString(object value)
            {
                return null;
            }

            public ShortObject(
                object id,
                string name,
                ObjectType objectType)
            {
                Name = name;
                Id = id;
                ObjectType = objectType;
            }
        }

        private void _bSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                if (!CheckData())
                    return;

                SetData();

                Exception error;
                Plugin.MainServerProvider.TimetecSettings.SaveTimetecSetting(_timetecSetting, out error);

                if (error != null)
                {
                    if (error is IncoherentDataException)
                    {
                        if (Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            LoadData();
                        }
                        else
                        {
                            try
                            {
                                InternalReloadEditingObjectWithEditedData();
                            }
                            catch (Exception err)
                            {
                                HandledExceptionAdapter.Examine(err);
                                Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                            }
                        }
                    }
                    else if (error is AccessDeniedException)
                    {
                        Dialog.Error(GetString("ErrorEditAccessDenied"));
                    }
                    else
                    {
                        HandledExceptionAdapter.Examine(error);
                        Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                    }

                    return;
                }

                _bSave.Enabled = false;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void InternalReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.TimetecSettings.ReloadObjectForEdit(out error);
            
            if (error != null)
                throw error;
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EditChanged(object sender, EventArgs e)
        {
            _bSave.Enabled = _allowEdit;
        }

        private void _mtbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (_mtbPassword.Text == DEFAULT_PASSWORD)
                _mtbPassword.Text = string.Empty;
        }

        private void _bResetEventlogLastId_Click(object sender, EventArgs e)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                if (Dialog.WarningQuestion(GetString("WarningQuestionResetLastEventId")))
                {
                    Plugin.MainServerProvider.TimetecSettings.LastEventId = -1;
                    SetDefaultDateTimeMode(false);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            RefreshTimtetecErrorEventsList();
        }

        private void RefreshTimtetecErrorEventsList()
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new DVoid2Void(RefreshTimtetecErrorEventsList));
                    return;
                }

                _bResend.Enabled = true;
                Cursor.Current = Cursors.Arrow;

                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                _errorTimetecEvents = Plugin.MainServerProvider.TimetecSettings.GeTimetecErrorEvents();

                if (_errorTimetecEvents == null
                    || _errorTimetecEvents.Count == 0)
                {
                    _wpfErrorEvents.Clear();
                    return;
                }

                _wpfErrorEvents.Items =
                    _errorTimetecEvents.Select(
                        obj => CreateShortObject(obj, true))
                        .ToList();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private IShortObject CreateShortObject(
            TimetecErrorEvent timetecErrorEvent,
            bool addReason)
        {
            var description = new StringBuilder();

            description.AppendFormat("Transition failed: {0}, Time: {1}", 
                timetecErrorEvent.SourceEventlog.Type,
                timetecErrorEvent.SourceEventlog.EventlogDateTime);

            var idCardReader = timetecErrorEvent.SourceEventlog.EventlogParameters.
                Where(param => param.TypeObjectType == (byte) ObjectType.CardReader)
                .Select(param => param.TypeGuid)
                .FirstOrDefault();

            if (idCardReader != Guid.Empty)
            {
                var cardReader = Plugin.MainServerProvider.CardReaders.GetObjectById(idCardReader);

                if (cardReader != null)
                    description.AppendFormat(", Card Reader: {0}", cardReader.Name);
            }

            var cardNumber = timetecErrorEvent.SourceEventlog.EventlogParameters.
                Where(param => param.Type == "Card number")
                .Select(param => param.Value)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(cardNumber))
            {
                description.AppendFormat(", Card number: {0}", cardNumber);

                var card = CgpClient.Singleton.MainServerProvider.Cards.GetCardByFullNumber(cardNumber);

                if (card != null
                    && card.Person != null)
                {
                    description.AppendFormat(", Person name: {0}", card.Person.ToString());
                }
            }

            if (addReason)
            {
                var reason = timetecErrorEvent.ErrorEventlog.EventlogParameters.
                    Where(param => param.Type == "Reason")
                    .Select(param => param.Value)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(reason))
                    description.AppendFormat(", Reason: {0}", reason);
            }

            return new ShortObject(
                timetecErrorEvent.Id,
                description.ToString(),
                ObjectType.Eventlog);
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                var selectedTimetecErrorEvents = GetSelectedTimtecErrorEvents();

                if (selectedTimetecErrorEvents == null)
                    return;

                if (!Dialog.WarningQuestion(GetString("WarningRemoveErrorEvents")))
                    return;

                Plugin.MainServerProvider.TimetecSettings.RemoveTimetecErrorEvent(
                    selectedTimetecErrorEvents.Select(obj => obj.Id).ToArray());

                RefreshTimtetecErrorEventsList();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _bResend_Click(object sender, EventArgs e)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                var selectedTimetecErrorEvents = GetSelectedTimtecErrorEvents();

                if (selectedTimetecErrorEvents == null)
                    return;

                _bResend.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                SafeThread.StartThread(() => TryResendTimetecEvents(selectedTimetecErrorEvents));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void TryResendTimetecEvents(ICollection<TimetecErrorEvent> selectedTimetecErrorEvents)
        {
            var result =
                Plugin.MainServerProvider.TimetecSettings.TryResendTimetecErrorEvents(selectedTimetecErrorEvents);

            RefreshTimtetecErrorEventsList();

            ShowTimetecResendingResult(
                selectedTimetecErrorEvents.Select(obj => CreateShortObject(obj, false)),
                result);
        }

        private void ShowTimetecResendingResult(
            IEnumerable<IShortObject> timetecErrorEventsDescriptions,
            Dictionary<int, TransitionAddResult> result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IEnumerable<IShortObject>, Dictionary<int, TransitionAddResult>>
                    (ShowTimetecResendingResult), timetecErrorEventsDescriptions, result);

                return;
            }

            var timetecResendingResultDialog = new TimetecResendEventResultDialog(
                timetecErrorEventsDescriptions,
                result);

            timetecResendingResultDialog.ShowDialog();
        }

        private ICollection<TimetecErrorEvent> GetSelectedTimtecErrorEvents()
        {
            if (_errorTimetecEvents == null)
                return null;

            var selectedItems = _wpfErrorEvents.SelectedItems;

            if (selectedItems == null || selectedItems.Count == 0)
                return null;

            return _errorTimetecEvents.Where(
                obj => selectedItems.Any(
                    item => (int) item.Id == obj.Id))
                .ToList();
        }

        private void _chbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (_chbSelectAll.Checked)
                _wpfErrorEvents.SelectAll();
            else
                _wpfErrorEvents.UnselectAll();
        }

        protected bool CheckLicense(out string errorMessage)
        {
            errorMessage = null;
#if !DEBUG
            string localisedName;
            object licenseValue;
            if (!CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                RequiredLicenceProperties.TimetecMaster.ToString(), out localisedName, out licenseValue) ||
                !(bool)licenseValue)
            {
                errorMessage = GetString("ErrorTimetecNotSupportedByLicense");
                return false;
            }
#endif

            return true;
        }

        private void _chbDoNotImportDepartments_CheckedChanged(object sender, EventArgs e)
        {
            _bSave.Enabled = _allowEdit;
        }
    }
}
