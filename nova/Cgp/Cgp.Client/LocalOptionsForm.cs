using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick.Net;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Client
{
    public partial class LocalOptionsForm :
#if DESIGNER
        Form
#else
 ACgpFullscreenForm
#endif
    {
        public LocalOptionsForm()
        {
            InitializeComponent();
            ShowLanguagesForSettings();
            LoadSetting();
            ShowTabComSettings();
            ShowCrLastCard(CgpClient.Singleton.LoginCrLastCard);
            CgpClient.Singleton.LoginCrOnlineStateChanged += ShowCrOnlineState;
            CgpClient.Singleton.LoginCrCardSwiped += ShowCrLastCard;
            RefreshCrOnlineState();
            InitGridAlarmSounds();
            _cdgvSoundSettings.DataGrid.DataError += DgvDataError;
            LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
            InitCGPDataGridView();
            FormOnEnter += FormEnter;
        }

        private void FormEnter(Form form)
        {
            SafeThread.StartThread(ShowTabsLocalSoundsAdvanced);
        }

        private void InitCGPDataGridView()
        {
            _cdgvServers.DataGrid.MouseDoubleClick += _dgPossibleServers_MouseDoubleClick;
            _cdgvSoundSettings.DataGrid.CellDoubleClick += _dgAlarmSoundSettings_CellDoubleClick;
        }

        void LocalizationHelper_LanguageChanged()
        {
            InitGridAlarmSounds();
        }

        void DgvDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // (No need to write anything in here)
        }

        public override void CallEscape()
        {
            Close();
        }

        private static volatile LocalOptionsForm _singleton = null;
        private static readonly object _syncRoot = new object();

        public static LocalOptionsForm Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new LocalOptionsForm();
                    }

                return _singleton;
            }
        }

        protected override void AfterTranslateForm()
        {
            FillCbAlarmInvocation();
            SelectCbAlarmInvocation(GeneralOptions.Singleton.AlarmSoundInvocationType);

            if (_lastCard != null)
            {
                _bCreateCard.Text = GetString("General_bEdit");
            }
            else
            {
                _bCreateCard.Text = GetString("General_bCreateCard");
            }
        }

        private void LoadSetting()
        {
            _eIP.Text = GeneralOptions.Singleton.ServerAddress;
            if (GeneralOptions.Singleton.Port == 0)
                _ePort.Text = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT.ToString(CultureInfo.InvariantCulture);
            else
                _ePort.Text = GeneralOptions.Singleton.Port.ToString(CultureInfo.InvariantCulture);
            _eFriendlyName.Text = GeneralOptions.Singleton.FriendlyName;
            _chbAutoUpgradeClient.Checked = GeneralOptions.Singleton.AutoUpgradeClient;
            _chbEventlogListReporter.Checked = GeneralOptions.Singleton.EventlogListReporter;
            _chbAlarmSoundEnabled.Checked = GeneralOptions.Singleton.AlarmSoundNotification;
            FillCbAlarmInvocation();
            SelectCbAlarmInvocation(GeneralOptions.Singleton.AlarmSoundInvocationType);
            FillCbAlarmInvocation();
            SelectCbAlarmInvocation(GeneralOptions.Singleton.AlarmSoundInvocationType);
            _eSoundFile.Text = GeneralOptions.Singleton.AlarmSoundFile;
            _eFrequencyRepeatAlarmSound.Value = GeneralOptions.Singleton.AlarmSoundRepeatFrequency;
            CheckAccessLocalSounds();
        }

        private void CheckAccessLocalSounds()
        {
            bool hasAccessLocalSoundAdmim = false;
            if (CgpClient.Singleton.IsLoggedIn)
            {
                if (CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.LocalSoundsAdmin)))
                    hasAccessLocalSoundAdmim = true;
            }

            if (!hasAccessLocalSoundAdmim)
                _gbAlarmSettings.Enabled = false;
            else
                _gbAlarmSettings.Enabled = true;
        }

        public void ShowTabPage(string tabPageName)
        {
            Show();
            for (int i = 0; i < _tcLocalOptions.TabPages.Count; i++)
            {
                if (_tcLocalOptions.TabPages[i].Name == tabPageName)
                {
                    _tcLocalOptions.SelectedIndex = i;
                }
            }
        }

        private void _eIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Checks for backspace
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }

            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '.' || e.KeyChar == ':')
            {
                e.Handled = false;
                return;
            }

            if (e.KeyChar < 'A' || e.KeyChar > 'F')
            {
                if (e.KeyChar < 'a' || e.KeyChar > 'f')
                {
                    e.Handled = true;
                }
            }
        }


        private void _cnmpLookup_Click(object sender, EventArgs e)
        {
            _cnmpLookup.Enabled = false;
            CgpClient.Singleton.CNMP.BeginLookupWithLocals(CNMP_LookupFinished, CNMPLookupType.Type,
                CgpServerGlobals.CNMP_TYPE,
                CgpServerGlobals.CGP_SERVER_FRIENDLY_NAME,
                CgpServerGlobals.CNMP_SERVER_PORT,
                CgpServerGlobals.CNMP_SERVER_MACHINE_NAME,
                CgpServerGlobals.CNMP_SERVER_MULTIHOMING,
                CgpServerGlobals.CNMP_SERVER_VERSION,
                CgpServerGlobals.CNMP_SERVER_EDITION_NAME);
            _cdgvServers.DataGrid.Focus();
        }

        BindingSource _bindSource;
        private void CNMP_LookupFinished(CNMPLookupType lookupType, string value, CNMPLookupResultList result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DCNMPLookupFinished(CNMP_LookupFinished), lookupType, value, result);
            }
            else
            {
                _cnmpLookup.Enabled = true;
                ICollection<DgServer> listDgServer = new List<DgServer>();
                if (null != result)
                {
                    foreach (CNMPLookupResultItem item in result)
                    {
                        DgServer dgServer = new DgServer {IpAddress = item.IP.ToString()};
                        string tmp = item.GetExtra(CgpServerGlobals.CGP_SERVER_FRIENDLY_NAME);
                        if (Validator.IsNotNullString(tmp))
                        {
                            dgServer.FriendlyName = tmp;
                        }

                        tmp = item.GetExtra(CgpServerGlobals.CNMP_SERVER_MACHINE_NAME);
                        if (Validator.IsNotNullString(tmp))
                        {
                            dgServer.MachineName = tmp;
                        }

                        tmp = item.GetExtra(CgpServerGlobals.CNMP_SERVER_MULTIHOMING);
                        if (Validator.IsNotNullString(tmp))
                        {
                            tmp = tmp.Trim().ToLower();
                            switch (tmp)
                            {
                                case "true":
                                case "1":
                                case "enabled":
                                    dgServer.Multihoming = true;
                                    break;
                                case "false":
                                case "0":
                                case "disabled":
                                    dgServer.Multihoming = false;
                                    break;
                            }

                        }

                        tmp = item.GetExtra(CgpServerGlobals.CNMP_SERVER_VERSION);
                        if (Validator.IsNotNullString(tmp))
                        {
                            dgServer.ServerVersion = tmp;
                        }

                        tmp = item.GetExtra(CgpServerGlobals.CNMP_SERVER_EDITION_NAME);
                        if (Validator.IsNotNullString(tmp))
                        {
                            dgServer.EditionName = tmp;
                        }

                        tmp = item.GetExtra(CgpServerGlobals.CNMP_SERVER_PORT);
                        if (Validator.IsNotNullString(tmp))
                        {
                            int itmp;
                            if (int.TryParse(tmp, out itmp))
                                dgServer.TcpPort = itmp;
                        }

                        listDgServer.Add(dgServer);
                    }
                }
                _bindSource = new BindingSource {DataSource = listDgServer};
                _cdgvServers.DataGrid.DataSource = _bindSource;
                _cdgvServers.DataGrid.Columns[_cdgvServers.DataGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                DataGridViewColumn ipAddressColumn = _cdgvServers.DataGrid.Columns["IpAddress"];
                if (ipAddressColumn != null)
                    ipAddressColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                DataGridViewColumn machineNameColumn = _cdgvServers.DataGrid.Columns["MachineName"];
                if (machineNameColumn != null)
                    machineNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                DataGridViewColumn serverVersionColumn = _cdgvServers.DataGrid.Columns["ServerVersion"];
                if (serverVersionColumn != null)
                    serverVersionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                DataGridViewColumn editionNameColumn = _cdgvServers.DataGrid.Columns["EditionName"];
                if (editionNameColumn != null)
                    editionNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                
                _cdgvServers.DataGrid.Refresh();
                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvServers.DataGrid);
            }
        }


        private void _bSave_Click(object sender, EventArgs e)
        {
            if (!IPHelper.IsValid(_eIP.Text))
            {
                MessageBox.Show(GetString("ErrorInsertValidIPAddress"));
                _eIP.Focus();
                return;
            }
            //wtf
            try
            {
                GeneralOptions.Singleton.ServerAddress = _eIP.Text;
                GeneralOptions.Singleton.Port = (int)_ePort.Value;
                GeneralOptions.Singleton.FriendlyName = _eFriendlyName.Text;
                GeneralOptions.Singleton.SaveClientSettingsToRegisty();

                if (GeneralOptions.Singleton.RemotingSettingsChanged)
                {
                    CgpClient.Singleton.ReconfigureRemoting();
                    //CgpClient.Singleton.AllowProxyRegaining();
                    GeneralOptions.Singleton.RemotingSettingsChangedReset();
                }

            }
            catch (Exception error)
            {
#if DEBUG
                Dialog.Error(error);
#endif
            }

        }

        private void _bSaveLng_Click(object sender, EventArgs e)
        {
            SaveLanguage();
        }

        public void UpdateSelectedLanguage(string language)
        {
            Control.ControlCollection contColl = _tpLanguageSettings.Controls;
            foreach (Control cnt in contColl)
            {
                if (cnt.GetType() == typeof(RadioButton)) // && cnt.Name != string.Empty)
                {
                    RadioButton rb = (RadioButton)cnt;
                    rb.Checked = rb.Text == language;
                }
            }
        }

        private void SaveLanguage()
        {
            string lng = string.Empty;

            Control.ControlCollection contColl = _tpLanguageSettings.Controls;

            foreach (Control cnt in contColl)
            {
                if (cnt.GetType() == typeof(RadioButton)) // && cnt.Name != string.Empty)
                {
                    var rb = (RadioButton) cnt;

                    if (rb.Checked)
                        lng = rb.Text;
                }
            }

            if (lng == string.Empty)
            {
                Dialog.Error(GetString("GeneralOptionsFormErrorLngSelect"));
                return;
            }

            GeneralOptions.Singleton.Language = lng;

            if (CgpClient.Singleton.IsLoggedIn)
                CgpClient.Singleton.MainServerProvider.SetLoginLanguage(lng);
            else
                GeneralOptions.Singleton.SaveLanguageSettingsToRegistry();

            CgpClient.Singleton.LocalizationHelper.SetLanguage(lng);
        }

        private void ShowLanguagesForSettings()
        {
            string[] allLng;
            allLng = CgpClient.Singleton.LocalizationHelper.AllLanguages;

            Login currentLogin = null;
            if (CgpClient.Singleton.IsLoggedIn && CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
            {
                currentLogin = CgpClient.Singleton.MainServerProvider.Logins.GetActualLogin();
            }
            int i = 0;

            foreach (string lng in allLng)
            {
                var rb = new RadioButton
                {
                    AutoSize = true,
                    Location = new Point(20, 40*(i + 1)),
                    Name = "_rbLng" + i,
                    Size = new Size(85, 17),
                    TabIndex = i,
                    TabStop = true,
                    Text = lng,
                    UseVisualStyleBackColor = true
                };
                if (CgpClient.Singleton.IsLoggedIn)
                {
                    if (currentLogin != null && currentLogin.ClientLanguage == lng)
                    {
                        rb.Checked = true;
                    }
                }
                else if (GeneralOptions.Singleton.Language == lng)
                    rb.Checked = true;
                _tpLanguageSettings.Controls.Add(rb);

                i++;

            }

            _bSaveLng.TabIndex = i;
        }

        protected override bool VerifySources()
        {
            return true;
        }

        private void FillComPortStrings()
        {
            SerialPortDescriptor actualSpd = null;
            if (_cbComPortName.Items != null &&
                _cbComPortName.SelectedItem is SerialPortDescriptor)
            {
                actualSpd = (SerialPortDescriptor)_cbComPortName.SelectedItem;
            }

            SerialPortDescriptor[] spDesriptors = SimpleSerialPort.GetExtendedPortNames();
            if (_cbComPortName.Items != null)
            {
                _cbComPortName.Items.Clear();
                _cbComPortName.Items.Add(string.Empty);
                foreach (SerialPortDescriptor spd in spDesriptors)
                {
                    _cbComPortName.Items.Add(spd);
                }
            }

            if (actualSpd != null)
                SetComPort(actualSpd.PortName);
        }

        private void SetComPort(string name)
        {
            foreach (object obj in _cbComPortName.Items)
            {
                if ((obj is SerialPortDescriptor) &&
                    ((SerialPortDescriptor)obj).PortName == name)
                {
                    _cbComPortName.SelectedItem = obj;
                    return;
                }
            }
        }

        private void ShowTabComSettings()
        {
            IEnumerable<ICgpVisualPlugin> plugins = 
                CgpClient.Singleton.PluginManager.GetVisualPlugins();

            bool isLoaded = false;

            foreach (ICgpVisualPlugin plugin in plugins)
            {
                if (plugin.Description == "NCAS")
                    isLoaded = true;
            }

            if (isLoaded)
            {
                FillComPortStrings();
                SetComPort(GeneralOptions.Singleton.ComPortName);
            }
            else
            {
                _tcLocalOptions.TabPages.Remove(_tpCardReaderSettings);
            }
        }

        private void ShowTabsLocalSoundsAdvanced()
        {
            Invoke((MethodInvoker) delegate
            {
                _tcLocalOptions.TabPages.Remove(_tpLocalSounds);
                _tcLocalOptions.TabPages.Remove(_tpAdvanced);
            });

            bool localSoundsView = false;
            bool localSoundsAdmin = false;
            bool showTabAdvanced = false;
            if (CgpClient.Singleton.MainServerProvider != null)
            {
                try
                {
                    localSoundsView =
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.LocalSoundsView));

                    localSoundsAdmin =
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.LocalSoundsAdmin));

                    showTabAdvanced =
                        CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.SuperAdmin));
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }

            Invoke((MethodInvoker)delegate
            {
                if (localSoundsView || localSoundsAdmin)
                {
                    if (!_tcLocalOptions.TabPages.Contains(_tpLocalSounds))
                    {
                        _tcLocalOptions.TabPages.Add(_tpLocalSounds);
                    }

                    _tpLocalSounds.Enabled = localSoundsAdmin;
                }

                if (showTabAdvanced)
                {
                    if (!_tcLocalOptions.TabPages.Contains(_tpAdvanced))
                    {
                        _tcLocalOptions.TabPages.Add(_tpAdvanced);
                    }
                }
            });
        }

        private void _bSave2_Click(object sender, EventArgs e)
        {
            SaveComPortSettings();
        }

        private void SaveComPortSettings()
        {
            string comPort;

            if (_cbComPortName.SelectedItem is SerialPortDescriptor)
            {
                comPort = ((SerialPortDescriptor)_cbComPortName.SelectedItem).PortName;
            }
            else
            {
                comPort = string.Empty;
            }

            GeneralOptions.Singleton.ComPortName = comPort;
            GeneralOptions.Singleton.SaveComPortSettingsToRegistry();

            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                plugin._plugin.RestartCardReaderCommunication();
            }
            ResetCrOnlineState();
        }

        public void ShowCrOnlineState(bool onlineState)
        {
            try
            {
                if (IsHandleCreated)
                {
                    RefreshCrOnlineState();
                }
            }
            catch
            {
            }
        }

        public void RefreshCrOnlineState()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(RefreshCrOnlineState));
            }
            else
            {
                try
                {
                    if (CgpClient.Singleton.LoginCrState == null)
                    {
                        _eCrState.Text = GetString("Unknown");
                        _eCrState.BackColor = Color.Yellow;
                    }
                    else
                    {
                        if (CgpClient.Singleton.IsCrOnline)
                        {
                            _eCrState.Text = GetString("Online");
                            _eCrState.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            _eCrState.Text = GetString("Offline");
                            _eCrState.BackColor = Color.Red;
                        }
                    }

                    _lTypeCRInformation.Text = CgpClient.Singleton.CardReaderTypeInformation;
                    _lFirmwareInformation.Text = CgpClient.Singleton.CardReaderFirmwareInformation;
                }
                catch
                { }
            }
        }

        public void ResetCrOnlineState()
        {
            CgpClient.Singleton.LoginCrState = null;
            RefreshCrOnlineState();
        }

        public void ShowCrLastCard(string lastCard)
        {
            SafeThread<string>.StartThread(RunShowCrLastCard, lastCard);
        }

        public void RunShowCrLastCard(string strLastCard)
        {
            try
            {
                Card lastCard = null;
                if (!string.IsNullOrEmpty(strLastCard) && !CgpClient.Singleton.IsConnectionLost(false))
                    lastCard = CgpClient.Singleton.MainServerProvider.Cards.GetCardByFullNumber(strLastCard);

                ShowCrLastCard(lastCard, strLastCard);
            }
            catch { }
        }

        Card _lastCard = null;
        Person _lastCardPerson = null;
        private void ShowCrLastCard(Card lastCard, string strLastCard)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Card, string>(ShowCrLastCard), lastCard, strLastCard);
            }
            else
            {
                try
                {
                    _lastCard = lastCard;
                    _eLastCard.Text = strLastCard;

                    if (_lastCard != null)
                    {
                        _bCreateCard.Text = GetString("General_bEdit");
                        _lastCardPerson = _lastCard.Person;
                    }
                    else
                    {
                        _bCreateCard.Text = GetString("General_bCreateCard");
                        _lastCardPerson = null;

                    }

                    if (_lastCardPerson != null)
                    {
                        _lPerson.Visible = true;
                        _itbPerson.Visible = true;
                        _itbPerson.Text = _lastCardPerson.ToString();
                        _itbPerson.Image = ObjectImageList.Singleton.GetImageForAOrmObject(_lastCardPerson);
                    }
                    else
                    {
                        _lPerson.Visible = false;
                        _itbPerson.Visible = false;
                    }

                    ChangeCreateCardButtonEnabled();
                }
                catch
                { }
            }
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            FillComPortStrings();
        }

        private void ChangeCreateCardButtonEnabled()
        {
            if (_eLastCard.Text == string.Empty)
            {
                _bCreateCard.Enabled = false;
            }
            else
            {
                _bCreateCard.Enabled = true;
            }
        }

        private void _bCreateCard_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsAuthenticated() &&
                CgpClient.Singleton.IsLoggedIn)
            {
                if (_lastCard != null)
                {
                    if (CardsForm.Singleton.HasAccessView())
                    {
                        CardsForm.Singleton.OpenEditForm(_lastCard, AfterEditCard);
                        return;
                    }
                    Dialog.Error(GetString("ErrorNoAccessEditCard"));
                }
                else
                {
                    if (CardsForm.Singleton.HasAccessInsert())
                    {
                        _bCreateCard.Enabled = false;
                        SafeThread.StartThread(DoCreateCard);
                        return;
                    }

                    Dialog.Error(GetString("ErrorNoAccessCreateCard"));
                }
            }
        }

        private void AfterEditCard(object obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(AfterEditCard), obj);
            }
            else
            {
                ShowCrLastCard(_eLastCard.Text);
            }
        }

        private void DoCreateCard()
        {
            try
            {
                CardsForm.Singleton.InsertWithData(_eLastCard.Text, AfterEditCard);
            }
            catch { }

            DoAfterShowCardForm();
        }

        private void DoAfterShowCardForm()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(DoAfterShowCardForm));
            }
            else
            {
                _bCreateCard.Enabled = true;
            }
        }

        private void _bSave3_Click(object sender, EventArgs e)
        {
            GeneralOptions.Singleton.AutoUpgradeClient = _chbAutoUpgradeClient.Checked;
            GeneralOptions.Singleton.EventlogListReporter = _chbEventlogListReporter.Checked;
            GeneralOptions.Singleton.AlarmSoundNotification = _chbAlarmSoundEnabled.Checked;
            if (_cbAlarmTypeInvocation.SelectedItem is AlarmTypeBuzzerClass)
            {
                AlarmTypeBuzzerClass atbc = (AlarmTypeBuzzerClass)_cbAlarmTypeInvocation.SelectedItem;
                GeneralOptions.Singleton.AlarmSoundInvocationType = atbc.AlarmTypeBuzzer;
            }
            GeneralOptions.Singleton.AlarmSoundFile = _eSoundFile.Text;
            GeneralOptions.Singleton.AlarmSoundRepeatFrequency = (int)_eFrequencyRepeatAlarmSound.Value;
            GeneralOptions.Singleton.SaveOtherSettingsToRegistry();
            AlarmSound.Singleton.SettingChanged();
            SaveAlarmSoundDgr();
        }

        private void FillCbAlarmInvocation()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(FillCbAlarmInvocation));
            }
            else
            {
                _cbAlarmTypeInvocation.Items.Clear();
                foreach (AlarmTypeBuzzer ab in Enum.GetValues(typeof(AlarmTypeBuzzer)))
                {
                    _cbAlarmTypeInvocation.Items.Add(new AlarmTypeBuzzerClass(ab));
                }
            }
        }

        private void SelectCbAlarmInvocation(AlarmTypeBuzzer nATB)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<AlarmTypeBuzzer>(SelectCbAlarmInvocation), nATB);
            }
            else
            {
                foreach (object obj in _cbAlarmTypeInvocation.Items)
                {
                    if ((obj is AlarmTypeBuzzerClass) &&
                        ((AlarmTypeBuzzerClass)obj).AlarmTypeBuzzer == nATB)
                    {
                        _cbAlarmTypeInvocation.SelectedItem = obj;
                        return;
                    }
                }
            }
        }


        private void _bSelect_Click(object sender, EventArgs e)
        {
            SoundExplorer se = new SoundExplorer();
            string file;
            se.ShowDialog(out file);
            if (string.IsNullOrEmpty(file)) return;
            _eSoundFile.Text = file;
        }

        private void _dgPossibleServers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvServers.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            if (_bindSource != null && _bindSource.Count > 0)
            {
                DgServer server = (DgServer)_bindSource.List[_bindSource.Position];
                _eIP.Text = server.IpAddress;

                if (TcpUdpPort.IsValid(server.TcpPort, false))
                {
                    _ePort.Value = server.TcpPort;
                }
            }
        }

        BindingSource _bindSourceAlarmSound;
        readonly IList<CbAlarmTypeBuzzer> _listAlarmTypeBuzzer = new List<CbAlarmTypeBuzzer>();
// ReSharper disable once UnusedMethodReturnValue.Local
        private IList<CbAlarmTypeBuzzer> CreateListAlarmTypeBuzzer()
        {
            _listAlarmTypeBuzzer.Clear();
            foreach (AlarmTypeBuzzer alarmTypeBuzzer in Enum.GetValues(typeof(AlarmTypeBuzzer)))
            {
                _listAlarmTypeBuzzer.Add(new CbAlarmTypeBuzzer(alarmTypeBuzzer));
            }
            return _listAlarmTypeBuzzer;
        }

        private CbAlarmTypeBuzzer GetFromListAlarmTypeBuzzer(AlarmTypeBuzzer alarmTypeBuzzer)
        {
            foreach (CbAlarmTypeBuzzer atb in _listAlarmTypeBuzzer)
            {
                if (atb.AlarmTypeBuzzer == alarmTypeBuzzer)
                    return atb;
            }

            CbAlarmTypeBuzzer cbAlarmTypeBuzzer = null;
            foreach (CbAlarmTypeBuzzer atb in _listAlarmTypeBuzzer)
            {
                if (atb.AlarmTypeBuzzer == AlarmTypeBuzzer.Alarm)
                    cbAlarmTypeBuzzer = atb;
            }
            return cbAlarmTypeBuzzer;
        }

        private void InitGridAlarmSounds()
        {
            CreateListAlarmTypeBuzzer();
            _cdgvSoundSettings.DataGrid.AutoGenerateColumns = false;
            _cdgvSoundSettings.DataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            _bindSourceAlarmSound = new BindingSource();
            List<AlarmTypeSoundDgvObj> listAlarmSettings = LoadTypeAlarm();
            listAlarmSettings = listAlarmSettings.OrderBy(alarm => alarm.AlarmTypeName.ToString(CultureInfo.InvariantCulture)).ToList();
            _bindSourceAlarmSound.DataSource = listAlarmSettings;
            _cdgvSoundSettings.DataGrid.DataSource = _bindSourceAlarmSound;

            if (!_cdgvSoundSettings.DataGrid.Columns.Contains("AlarmTypeName"))
            {
                DataGridViewTextBoxColumn columnTypeName = new DataGridViewTextBoxColumn
                {
                    Name = "AlarmTypeName",
                    DataPropertyName = "AlarmTypeName",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                };
                _cdgvSoundSettings.DataGrid.Columns.Add(columnTypeName);
            }

            if (!_cdgvSoundSettings.DataGrid.Columns.Contains("SoundFileName"))
            {
                DataGridViewTextBoxColumn columnTypeName = new DataGridViewTextBoxColumn
                {
                    Name = "SoundFileName",
                    DataPropertyName = "SoundFileName",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                };
                _cdgvSoundSettings.DataGrid.Columns.Add(columnTypeName);
            }

            if (!_cdgvSoundSettings.DataGrid.Columns.Contains("SoundRepeatCount"))
            {
                DataGridViewTextBoxColumn columnTypeName = new DataGridViewTextBoxColumn
                {
                    Name = "SoundRepeatCount",
                    DataPropertyName = "SoundRepeatCount",
                    ReadOnly = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                };
                _cdgvSoundSettings.DataGrid.Columns.Add(columnTypeName);
            }

            if (!_cdgvSoundSettings.DataGrid.Columns.Contains("SoundRepeatTillDone"))
            {
                DataGridViewCheckBoxColumn columnTypeName = new DataGridViewCheckBoxColumn
                {
                    Name = "SoundRepeatTillDone",
                    DataPropertyName = "SoundRepeatTillDone",
                    ReadOnly = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                };
                _cdgvSoundSettings.DataGrid.Columns.Add(columnTypeName);
            }

            if (!_cdgvSoundSettings.DataGrid.Columns.Contains("AlarmTypeBuzzerName"))
            {
                DataGridViewComboBoxColumn columnAlarmTypeBuzzer = new DataGridViewComboBoxColumn
                {
                    DataSource = _listAlarmTypeBuzzer,
                    Name = "AlarmTypeBuzzerName",
                    DataPropertyName = "AlarmTypeBuzzerName",
                    DisplayMember = "LocalizeAlarmTypeBuzzer",
                    ValueMember = "Self",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                _cdgvSoundSettings.DataGrid.Columns.Add(columnAlarmTypeBuzzer);
            }
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvSoundSettings.DataGrid);
        }

        private List<AlarmTypeSoundDgvObj> LoadTypeAlarm()
        {
            List<AlarmTypeSoundDgvObj> result = LoadAlarmSoundFromRegisty();
            if (result == null)
            {
                result = GenerateAlarmSoundByServer();
            }
            return result;
        }

        private List<AlarmTypeSoundDgvObj> LoadAlarmSoundFromRegisty()
        {
            ReadRegAlarmSound regLoad = new ReadRegAlarmSound();
            List<AlarmSoundRegObj> listAlarmSoundregObj = regLoad.LoadRegAlarmSoundFromRegisrty();
            if (listAlarmSoundregObj == null) return null;

            List<AlarmTypeSoundDgvObj> result = new List<AlarmTypeSoundDgvObj>();

            foreach (AlarmSoundRegObj regAlarm in listAlarmSoundregObj)
            {
                result.Add(new AlarmTypeSoundDgvObj(regAlarm, GetFromListAlarmTypeBuzzer((AlarmTypeBuzzer)regAlarm.AlarmTriggerType)));
            }
            return result;
        }

        private List<AlarmTypeSoundDgvObj> GenerateAlarmSoundByServer()
        {
            List<AlarmTypeSoundDgvObj> result = new List<AlarmTypeSoundDgvObj>();
            IList<AlarmType> alarmTypes = CgpClient.Singleton.GetAlarmTypesWithPlugin();
            if (alarmTypes != null)
            {
                foreach (AlarmType at in alarmTypes)
                {
                    result.Add(new AlarmTypeSoundDgvObj(at));
                }
            }
            return result;
        }

        private void SaveAlarmSoundDgr()
        {
            ReadRegAlarmSound regSaveAlarmSound = new ReadRegAlarmSound();
            List<AlarmTypeSoundDgvObj> alarmSoundDgv = (List<AlarmTypeSoundDgvObj>)_bindSourceAlarmSound.DataSource;
            regSaveAlarmSound.SavegAlarmSoundDgvToRegistry(alarmSoundDgv);
        }

        private void _dgAlarmSoundSettings_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _cdgvSoundSettings.DataGrid.Columns["SoundFileName"].Index)
            {
                SoundExplorer se = new SoundExplorer();
                string file;
                se.ShowDialog(out file);
                DataGridViewCell actualCell = _cdgvSoundSettings.DataGrid.CurrentCell;
                if (string.IsNullOrEmpty(file))
                {
                    actualCell.Value = string.Empty;
                }
                else
                {
                    actualCell.Value = file;
                }
            }
        }

        private void _lPort_Click(object sender, EventArgs e)
        {

        }

        private void _ePort_ValueChanged(object sender, EventArgs e)
        {

        }

        private void _tpOtherSettings_Enter(object sender, EventArgs e)
        {
            CheckAccessLocalSounds();
        }

        private void _itbPerson_DoubleClick(object sender, EventArgs e)
        {
            if (_lastCardPerson != null)
            {
                PersonsForm.Singleton.OpenEditForm(_lastCardPerson);
            }
        }

        private void _bGarbageCollection_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }
    }

    public class AlarmTypeBuzzerClass
    {
        readonly AlarmTypeBuzzer _atb;
        readonly string _name;

        public AlarmTypeBuzzer AlarmTypeBuzzer
        {
            get { return _atb; }
        }

        public override string ToString()
        {
            return _name;
        }

        public AlarmTypeBuzzerClass(AlarmTypeBuzzer atb)
        {
            _atb = atb;
            _name = CgpClient.Singleton.LocalizationHelper.GetString("AlarmStates_" + atb.ToString());
        }
    }

    public class DgServer
    {
        private string _ipAddress;
        private int _tcpPort = CgpServerGlobals.DEFAULT_REMOTING_SERVER_PORT;
        private string _friendlyName;
        private string _machineName;
        private bool _multihoming;
        private string _serverVersion;
        private string _editionName;


        public string IpAddress { get { return _ipAddress; } set { _ipAddress = value; } }
        public int TcpPort { get { return _tcpPort; } set { _tcpPort = value; } }
        public string FriendlyName { get { return _friendlyName; } set { _friendlyName = value; } }
        public string MachineName { get { return _machineName; } set { _machineName = value; } }
        public bool Multihoming { get { return _multihoming; } set { _multihoming = value; } }
        public string ServerVersion { get { return _serverVersion; } set { _serverVersion = value; } }
        public string EditionName { get { return _editionName; } set { _editionName = value; } }
    }

    public class AlarmTypeSoundDgvObj
    {
        public AlarmType _alarmType;
        private string _alarmTypeName;
        public string _soundFileName;
        public int _soundRepeatCount = 1;
        public bool _untilOn;
        //public AlarmTypeBuzzer _alarmTypeBuzzer = AlarmTypeBuzzer.Alarm;
        CbAlarmTypeBuzzer _AlarmTypeBuzzer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmType"></param>
        public AlarmTypeSoundDgvObj(AlarmType alarmType)
        {
            _alarmType = alarmType;
            //_alarmTypeName = CgpClient.Singleton.LocalizationHelper.GetString(AlarmsConst.GetTranslateSymbol(alarmType));

            _alarmTypeName = CgpClient.Singleton.GetLocalizedString(CgpClient.ALARM_TYPE_LOCALIZATION_PREFIX + alarmType.ToString());

            if (string.IsNullOrEmpty(_alarmTypeName) || _alarmTypeName.Contains("NO_TRANSLATION"))
                _alarmTypeName = alarmType.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmTypeRegObj"></param>
        /// <param name="cbAlarmTypeBuzzer"></param>
        public AlarmTypeSoundDgvObj(AlarmSoundRegObj alarmTypeRegObj, CbAlarmTypeBuzzer cbAlarmTypeBuzzer)
        {
            _alarmType = (AlarmType)alarmTypeRegObj.AlarmType;
            _alarmTypeName = CgpClient.Singleton.GetLocalizedAlarmRuiPrefix(_alarmType);
            if (string.IsNullOrEmpty(_alarmTypeName) || _alarmTypeName.Contains("NO_TRANSLATION"))
                _alarmTypeName = _alarmType.ToString();


            _soundFileName = alarmTypeRegObj.SoundFileName;
            _soundRepeatCount = alarmTypeRegObj.RepeatCount;
            _untilOn = alarmTypeRegObj.TillExists;
            _AlarmTypeBuzzer = cbAlarmTypeBuzzer;
        }

        public AlarmType AlarmType { get { return _alarmType; } }
        public string AlarmTypeName { get { return _alarmTypeName; } set { _alarmTypeName = value; } }
        public string SoundFileName { get { return _soundFileName; } set { _soundFileName = value; } }
        public int SoundRepeatCount { get { return _soundRepeatCount; } set { _soundRepeatCount = value; } }
        public bool SoundRepeatTillDone { get { return _untilOn; } set { _untilOn = value; } }
        public CbAlarmTypeBuzzer AlarmTypeBuzzerName { get { return _AlarmTypeBuzzer; } set { _AlarmTypeBuzzer = value; } }
        public AlarmTypeBuzzer AlarmTypeBuzzer { get { return _AlarmTypeBuzzer.AlarmTypeBuzzer; } }

        public int RegAlarmBuzzerType
        {
            get
            {
                if (_AlarmTypeBuzzer != null)
                {
                    return (int)_AlarmTypeBuzzer.AlarmTypeBuzzer;
                }
                return 0;
            }
        }

        public string RegistrySoundFile
        {
            get
            {
                if (_soundFileName != null)
                {
                    return _soundFileName;
                }
                return string.Empty;
            }
        }
    }


    public class CbAlarmTypeBuzzer
    {
        private AlarmTypeBuzzer _alarmTypeBuzzer;
        private string _alarmTypeBuzzerName;

        public AlarmTypeBuzzer AlarmTypeBuzzer { get { return _alarmTypeBuzzer; } set { _alarmTypeBuzzer = value; } }
        public string LocalizeAlarmTypeBuzzer { get { return _alarmTypeBuzzerName; } set { _alarmTypeBuzzerName = value; } }

        public CbAlarmTypeBuzzer(AlarmTypeBuzzer alarmTypeBuzzer)
        {
            _alarmTypeBuzzer = alarmTypeBuzzer;
            _alarmTypeBuzzerName = CgpClient.Singleton.LocalizationHelper.GetString("AlarmStates_" + alarmTypeBuzzer.ToString());
        }

        public override string ToString()
        {
            return _alarmTypeBuzzerName;
        }

        public CbAlarmTypeBuzzer Self
        {
            get { return this; }
        }
    }
}
