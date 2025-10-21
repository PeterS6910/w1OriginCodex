using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.IwQuick.Net;
using Contal.Cgp.Server.Beans;
using Contal.LwDhcp;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using Contal.Cgp.Globals;
using Contal.IwQuick.Threads;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Cgp.Components;
using System.ComponentModel;

namespace Contal.Cgp.Client
{
    enum MACAddressPrefixes
    {
        CCU_000B3D = 0,
        DCU_00003D = 1,
        OTHER_00187D = 2
    }
    public partial class GeneralOptionsForm :
#if DESIGNER
        Form
#else
 ACgpFullscreenForm
#endif
    {
        private class CodeLengthNumericUpDownEditor : UINumericUpDownEditor
        {
            public CodeLengthNumericUpDownEditor()
                : base(4, 12)
            {
            }
        }

        public class CodeLengthIntConverter : DecimalConverter
        {
            private int[] values;
            public CodeLengthIntConverter()
            {
                values = new int[9];
                for (int i = 0; i < 9; i++)
                {
                    values[i] = i + 4;
                }
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;// show drop-down  
            }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;// true - not allow value set from editor
            }
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(values);
            }
        }

        private const string REMOTE_SERVICES_SETTINGS = "RemoteServicesSettings";
        private const string DHCP_SERVER_SETTINGS = "DHCPServerSettings";
        private const string SERIAL_PORT_SETTINGS = "SerialPortSettings";
        private const string DATABASE_BACKUP_SETTINGS = "DatabaseBackupSettings";
        private const string EVENTLOG_EXPIRATION_SETTINGS = "EventlogExpirationSettings";
        private const string COLOR_SETTINGS = "ColorSettings";
        private const string AUTO_CLOSE_SETTINGS = "AutoCloseSettings";
        private const string EVENTLOG_SETTINGS = "EventlogSettings";
        private const string ADVANCED_ACCESS_SETTINGS = "AdvancedAccessSettings";
        private const string ADVANCED_SETTINGS = "AdvancedSettings";
        private const string LANGUAGE_SETTINGS = "LanguageSettings";
        private const string SECURITY_SETTINGS = "SecuritySettings";
        private const string ALARM_SETTINGS = "AlarmSettings";
        private const string CUSTOMER_AND_SUPPLIER_INFO = "CustomerAndSupplierInfo";
        private const string DEMO_LICENSE = "Demo license";
        private const int MAX_PICTURE_WIDTH = 240;
        private const int MAX_PICTURE_HEIGHT = 120;

        private bool? _isSqlServerRunOnServerMachine = null;
        private bool? _loggedAsSuperAdmin = null;
        private Cgp.Server.Beans.ServerGeneralOptions _serverGeneralOptions = null;

        private bool _SqlOnlineState = true;
        private Action<bool> _eventSqlServerOnlineStateChanged = null;

        public bool _readNewGeneralOptionsFromServer = true;

        private static volatile GeneralOptionsForm _singleton = null;
        private static object _syncRoot = new object();

        private List<string> _changedTabPages = new List<string>();
        private BinaryPhoto _binaryPhoto;
        private bool _isPhotoModified = false;

        public static GeneralOptionsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new GeneralOptionsForm();
                    }

                return _singleton;
            }
        }

        public bool LoggedAsSuperAdmin
        {
            get
            {
                if (!CgpClient.Singleton.IsConnectionLost(false))
                    _loggedAsSuperAdmin = CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs);
                else
                    _loggedAsSuperAdmin = false;

                return _loggedAsSuperAdmin.Value;
            }
        }

        public override void CallEscape()
        {
            Close();
        }

        /// <summary>
        /// Change for settings grop
        /// </summary>
        /// <param name="settingsGroup"></param>
        private void EditTextChanger(string settingsGroup)
        {
            lock (_changedTabPages)
            {
                if (!_changedTabPages.Contains(settingsGroup))
                    _changedTabPages.Add(settingsGroup);
            }
        }

        /// <summary>
        /// Change for remote server settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerRemoteServicesSettings(object sender, EventArgs e)
        {
            EditTextChanger(REMOTE_SERVICES_SETTINGS);
        }

        /// <summary>
        /// Change for DHCP server settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerDHCPServerSettings(object sender, EventArgs e)
        {
            EditTextChanger(DHCP_SERVER_SETTINGS);
        }

        /// <summary>
        /// Change for serial port settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerSerialPortSettings(object sender, EventArgs e)
        {
            EditTextChanger(SERIAL_PORT_SETTINGS);
        }

        /// <summary>
        /// Change for database backup settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerDatabaseBackupSettings(object sender, EventArgs e)
        {
            EditTextChanger(DATABASE_BACKUP_SETTINGS);
        }

        /// <summary>
        /// Change for eventlog expiration settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerEvenlogExpiationSettings(object sender, EventArgs e)
        {
            EditTextChanger(EVENTLOG_EXPIRATION_SETTINGS);
        }

        /// <summary>
        /// Change for color settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerColorSettings(object sender, EventArgs e)
        {
            EditTextChanger(COLOR_SETTINGS);
        }

        /// <summary>
        /// Change for auto close settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerAutoCloseSettings(object sender, EventArgs e)
        {
            EditTextChanger(AUTO_CLOSE_SETTINGS);
        }

        /// <summary>
        /// Change for eventlogs settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerEventlogSettings(object sender, EventArgs e)
        {
            EditTextChanger(EVENTLOG_SETTINGS);
        }

        /// <summary>
        /// Change for advanced access settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerAdvancedAccessSettings(object sender, EventArgs e)
        {
            EditTextChanger(ADVANCED_ACCESS_SETTINGS);
        }

        /// <summary>
        /// Change for advanced settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerAdvancedSettings(object sender, EventArgs e)
        {
            EditTextChanger(ADVANCED_SETTINGS);
        }

        /// <summary>
        /// Change for select language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerLanguageSettings(object sender, EventArgs e)
        {
            EditTextChanger(LANGUAGE_SETTINGS);
        }

        /// <summary>
        /// Change for security settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerSecuritySettings(object sender, EventArgs e)
        {
            EditTextChanger(SECURITY_SETTINGS);
        }

        /// <summary>
        /// Change for alarm settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTextChangerAlarmSettings(object sender, EventArgs e)
        {
            EditTextChanger(ALARM_SETTINGS);
        }

        /// <summary>
        /// Reset changes for all settings groups
        /// </summary>
        private void ResetAllChanges()
        {
            lock (_changedTabPages)
            {
                _changedTabPages.Clear();
            }
        }

        /// <summary>
        /// Reset changes for one settings group
        /// </summary>
        /// <param name="tabPageName"></param>
        private void ResetChanges(string settingsGroup)
        {
            lock (_changedTabPages)
            {
                if (_changedTabPages.Contains(settingsGroup))
                    _changedTabPages.Remove(settingsGroup);
            }
        }

        private GeneralOptionsForm()
        {
            InitializeComponent();
            FillSMTPServerSettings();
            FillSecuritySettings();
            FillCustomerAndSupplierInfo();
            //LoadSetting();
            RegisterToMain();
            FormOnEnter += new Action<Form>(RunOnEnter);
            //temporary disable option for SMS
            _tcGeneralOptions.TabPages.Remove(_tpSerialPort);
            LocalizationHelper.LanguageChanged += new DVoid2Void(LocalizationHelper_LanguageChanged);
            _cdgvAlarmSettings.DataGrid.DataError += new DataGridViewDataErrorEventHandler(dgvCombo_DataError);
            InitCGPDataGridView();
            _eventSqlServerOnlineStateChanged = new Action<bool>(SqlServerOnlineStateChanged);
            SqlServerOnlineStateChangedHandler.Singleton.RegisterSqlServerOnlineStateChanged(_eventSqlServerOnlineStateChanged);
        }

        private void HideDisableTabPages()
        {
            RemoveAllTabPages();

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            HideDisableTabPageLicenseInfo(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsLicenseInfoView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsLicenseInfoView)));

            HideDisableTabPageServerControl(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsServerControlView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsServerControlAdmin)));

            HideDisableTabPageRemoteServiceSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsRemoteServiceSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsRemoteServiceSettingsAdmin)));

            HideDisableTabPageDhcpServer(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsDhcpServerView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsDhcpServerAdmin)));

            HideDisableTabPageDatabaseSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsDatabaseSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsDatabaseSettingsAdmin)));

            HideDisableTabPageSecuritySettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsAdmin)));

            HideDisableTabPageAlarmSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsAlarmSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsAlarmSettingsAdmin)));

            HideDisableTabPageUiSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsUiSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsUiSettingsAdmin)));

            HideDisableTabPageEventlogs(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsEventlogsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsEventlogsAdmin)));

            HideDisableTabPageLanguageSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsLanguageSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsLanguageSettingsAdmin)));

            HideDisableTabPageAdvancedAccessSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsAdvancedAccessSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsAdvancedAccessSettingsAdmin)));

            HideDisableTabPageAdvancedSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsAdvancedSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsAdvancedSettingsAdmin)));

            HideDisableTabPageCustomerAndSupplieInfo();

            if (!CgpClient.Singleton.MainServerProvider.CheckTimetecLicense())
                HideReportEventlogControls();
        }

        private void RemoveAllTabPages()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(RemoveAllTabPages));
            }
            else
            {
                _tcGeneralOptions.TabPages.Clear();
            }
        }

        private void HideDisableTabPageLicenseInfo(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageLicenseInfo),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpLicence))
                {
                    _tcGeneralOptions.TabPages.Add(_tpLicence);
                }

                _tpLicence.Enabled = admin;
            }
        }

        private void HideDisableTabPageServerControl(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageServerControl),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpServerControl))
                {
                    _tcGeneralOptions.TabPages.Add(_tpServerControl);
                }

                _tpServerControl.Enabled = admin;
            }
        }

        private void HideDisableTabPageRemoteServiceSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageRemoteServiceSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpRemoteServicesSettings))
                {
                    _tcGeneralOptions.TabPages.Add(_tpRemoteServicesSettings);
                }

                _tpRemoteServicesSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageDhcpServer(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDhcpServer),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpDHCPServer))
                {
                    _tcGeneralOptions.TabPages.Add(_tpDHCPServer);
                }

                _tpDHCPServer.Enabled = admin;
            }
        }

        private void HideDisableTabPageDatabaseSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDatabaseSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpDatabaseOptions))
                {
                    _tcGeneralOptions.TabPages.Add(_tpDatabaseOptions);
                }

                _tpDatabaseOptions.Enabled = admin;
            }
        }

        private void HideDisableTabPageSecuritySettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSecuritySettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpSecuritySettings))
                {
                    _tcGeneralOptions.TabPages.Add(_tpSecuritySettings);
                }

                _tpSecuritySettings.Enabled = admin;
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
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpAlarmSettings))
                {
                    _tcGeneralOptions.TabPages.Add(_tpAlarmSettings);
                }

                _tpAlarmSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageUiSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageUiSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpUiSettings))
                {
                    _tcGeneralOptions.TabPages.Add(_tpUiSettings);
                }

                _tpUiSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageEventlogs(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageEventlogs),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpEventlogs))
                {
                    _tcGeneralOptions.TabPages.Add(_tpEventlogs);
                }

                _tpEventlogs.Enabled = admin;
            }
        }

        private void HideDisableTabPageLanguageSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageLanguageSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpLanguage))
                {
                    _tcGeneralOptions.TabPages.Add(_tpLanguage);
                }

                _tpLanguage.Enabled = admin;
            }
        }

        private void HideDisableTabPageAdvancedAccessSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAdvancedAccessSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpAdvancedAccessSettings))
                {
                    _tcGeneralOptions.TabPages.Add(_tpAdvancedAccessSettings);
                }

                _tpAdvancedAccessSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageAdvancedSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAdvancedSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    return;
                }

                if (!_tcGeneralOptions.Contains(_tpAdvancedSettings))
                {
                    _tcGeneralOptions.TabPages.Add(_tpAdvancedSettings);
                }

                _tpAdvancedSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageCustomerAndSupplieInfo()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(HideDisableTabPageCustomerAndSupplieInfo));
            }
            else
            {
                _tcGeneralOptions.TabPages.Add(_tpCustomerSupplierInfo);
                bool enableControls = true;

                if (_loggedAsSuperAdmin == null ||
                    !_loggedAsSuperAdmin.Value)
                {
                    enableControls = false;
                }

                foreach (var control in _tpCustomerSupplierInfo.Controls.Cast<Control>())
                {
                    control.Enabled = enableControls;
                }
            }
        }

        private void HideReportEventlogControls()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(HideReportEventlogControls));
            }
            else
            {
                _gbEventlogReports.Visible = false;
            }
        }

        private void SqlServerOnlineStateChanged(bool online)
        {
            _SqlOnlineState = online;
        }

        private void InitCGPDataGridView()
        {
            _cdgvAlarmSettings.DataGrid.CellClick += new DataGridViewCellEventHandler(_dgAlarmSettings_CellClick);
            _cdgvAlarmSettings.DataGrid.CellEndEdit += new DataGridViewCellEventHandler(_dgAlarmSettings_CellValueChanged);
        }

        private void ShowLanguagesForSettings()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            string[] allLng = CgpClient.Singleton.MainServerProvider.GetAllLanguages();
            string currentServerLanguage = CgpClient.Singleton.MainServerProvider.GetCurrentLanguage();
            if (allLng == null)
                return;

            List<Control> radioButtonsToRemove = new List<Control>();
            System.Windows.Forms.Control.ControlCollection contColl = _tpLanguage.Controls;
            foreach (System.Windows.Forms.Control cnt in contColl)
            {
                if (cnt.GetType() == typeof(RadioButton))
                {
                    radioButtonsToRemove.Add(cnt);
                }
            }

            if (radioButtonsToRemove != null && radioButtonsToRemove.Count > 0)
            {
                foreach (Control cnt in radioButtonsToRemove)
                {
                    _tpLanguage.Controls.Remove(cnt);
                }
            }

            int i = 0;
            foreach (string lng in allLng)
            {
                RadioButton rb = new RadioButton();
                rb.AutoSize = true;
                rb.Location = new System.Drawing.Point(20, 40 * (i + 1));
                rb.Name = "_rbLng" + i.ToString();
                rb.Size = new System.Drawing.Size(85, 17);
                rb.TabIndex = i;
                rb.TabStop = true;
                rb.Text = lng;
                rb.UseVisualStyleBackColor = true;
                rb.CheckedChanged += new EventHandler(EditTextChangerLanguageSettings);

                if (lng == currentServerLanguage)
                    rb.Checked = true;

                this._tpLanguage.Controls.Add(rb);

                i++;

            }

            this._bSave2.TabIndex = i;
            ResetChanges(LANGUAGE_SETTINGS);
        }

        void dgvCombo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // (No need to write anything in here)
        }

        private void FillLicenseSettings()
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            _epgLicenceInfo.ShowCustomProperties = true;
            _epgLicenceInfo.AutoSizeProperties = false;
            if (CgpClient.Singleton.MainServerProvider.DemoLicence)
            {
                if (_epgLicenceInfo.Items.Count == 0)
                    _epgLicenceInfo.Items.Add(DEMO_LICENSE, 0, true, DEMO_LICENSE, string.Empty, true, "_lDemoInfo");
            }
            else
            {
                _epgLicenceInfo.Items.Clear();
                _epgLicenceInfo.Items.Add("License file", CgpClient.Singleton.MainServerProvider.GetLicencePath(), true, "Valid license", string.Empty, true, "_lLicenceFile");
                _epgLicenceInfo.Items.Add("License expiration", GetExpirationText(CgpClient.Singleton.MainServerProvider.GetLicenceExpirationDate()), true, "Valid license", string.Empty, true, "_lLicenceExpiration");
                foreach (KeyValuePair<string, string> property in CgpClient.Singleton.MainServerProvider.GetLicenceProperies())
                {
                    string localisedName = string.Empty;
                    if (!CgpClient.Singleton.MainServerProvider.GetLicencePropertyLocalisedName(property.Key.Replace(".", ""), LocalizationHelper.ActualLanguage, out localisedName))
                        localisedName = property.Key;
                    if (property.Key.ToLower().EndsWith(CgpClient.PLUGIN_KEYWORD.ToLower()))
                    {
                        if (property.Value.ToLower().Equals("true"))
                        {
                            _epgLicenceInfo.Items.Add(localisedName, GetString("Enabled"), true, "Valid license", string.Empty, true, "_" + property.Key.Replace(".", ""));
                            continue;
                        }
                        else if (property.Value.ToLower().Equals("false"))
                        {
                            _epgLicenceInfo.Items.Add(localisedName, GetString("Disabled"), true, "Valid license", string.Empty, true, "_" + property.Key.Replace(".", ""));
                            continue;
                        }
                    }
                    _epgLicenceInfo.Items.Add(localisedName, property.Value, true, "Valid license", string.Empty, true, "_" + property.Key.Replace(".", ""));
                }

                CgpClient.Singleton.DeactiveDemoLicenceInfo();
            }
            _epgLicenceInfo.Refresh();
            LocalizationHelper.TranslateControl(_tcGeneralOptions);
        }

        private const string LABEL_SMTP_SERVER_SETTINGS = "SMTP Server Settings";

        private void FillSMTPServerSettings()
        {
            _epgSMTPServerSettings.ShowCustomProperties = true;
            _epgSMTPServerSettings.Items.Add("SMTP Server", string.Empty, false, LABEL_SMTP_SERVER_SETTINGS, "SMTPServerDescription", true, "_lSmtpServer");
            _epgSMTPServerSettings.Items.Add("Port", 0, false, LABEL_SMTP_SERVER_SETTINGS, "SMTPServerDescription", true, "_lSmtpPort");
            _epgSMTPServerSettings.Items.Add("Source email address", string.Empty, false, LABEL_SMTP_SERVER_SETTINGS, "SMTPServerDescription", true, "_lSourceEmail");
            _epgSMTPServerSettings.Items.Add("Subject", string.Empty, false, LABEL_SMTP_SERVER_SETTINGS, "SMTPServerDescription", true, "_lSubject");
            _epgSMTPServerSettings.Items.Add("Credentials", string.Empty, false, LABEL_SMTP_SERVER_SETTINGS, "SMTPServerDescription", true, "_lCredentials");
            _epgSMTPServerSettings.Items.Add("SSL", string.Empty, false, LABEL_SMTP_SERVER_SETTINGS, "SMTPServerDescription", true, "_lSSL");
            _epgSMTPServerSettings.AutoSizeProperties = false;
            _epgSMTPServerSettings.Refresh();
        }

        private void FillCustomerAndSupplierInfo()
        {
            _epgCustomerInfo.ShowCustomProperties = true;
            _epgCustomerInfo.Items.Add("Company name", string.Empty, false, null, null, true, "_lCompanyName");
            _epgCustomerInfo.Items.Add("Delivery address", string.Empty, false, null, null, true, "_lDeliveryAddress");
            _epgCustomerInfo.Items.Add("Zip Code", string.Empty, false, null, null, true, "_lZipCode");
            _epgCustomerInfo.Items.Add("City/State", string.Empty, false, null, null, true, "_lCityState");
            _epgCustomerInfo.Items.Add("Country", string.Empty, false, null, null, true, "_lCountry");
            _epgCustomerInfo.Items.Add("Phone", string.Empty, false, null, null, true, "_lPhone");
            _epgCustomerInfo.Items.Add("Website", string.Empty, false, null, null, true, "_lWebsite");
            _epgCustomerInfo.Items.Add("Contact Person", string.Empty, false, null, null, true, "_lContactPerson");
            _epgCustomerInfo.AutoSizeProperties = false;
            _epgCustomerInfo.Refresh();

            _epgSupplierInfo.ShowCustomProperties = true;
            _epgSupplierInfo.Items.Add("Company name", string.Empty, false, null, null, true, "_lCompanyName");
            _epgSupplierInfo.Items.Add("Delivery address", string.Empty, false, null, null, true, "_lDeliveryAddress");
            _epgSupplierInfo.Items.Add("Zip Code", string.Empty, false, null, null, true, "_lZipCode");
            _epgSupplierInfo.Items.Add("City/State", string.Empty, false, null, null, true, "_lCityState");
            _epgSupplierInfo.Items.Add("Country", string.Empty, false, null, null, true, "_lCountry");
            _epgSupplierInfo.Items.Add("Phone", string.Empty, false, null, null, true, "_lPhone");
            _epgSupplierInfo.Items.Add("Website", string.Empty, false, null, null, true, "_lWebsite");
            _epgSupplierInfo.Items.Add("Contact Person", string.Empty, false, null, null, true, "_lContactPerson");
            _epgSupplierInfo.AutoSizeProperties = false;
            _epgSupplierInfo.Refresh();
        }

        private const string LABEL_SECURITY_SETTINGS = "Security settings";
        private const string SECURITY_SETTINGS_UNIQUE_NOT_NULL_PERSONAL_KEY = "UniqueNotNullPersonalKey";
        private const string SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE = "CardPinLoginRequire";
        private const string SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY = "ChangePasswordEvery";
        private const string SECURITY_SETTINGS_LOCK_CLIENT_APPLICATION = "LockClientApplication";
        private const string SECURITY_SETTINGS_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD = "CcuConfigurationToServerByPassword";
        private const string SECURITY_SETTINGS_REQUIRED_SECURE_PIN = "RequiredSecurePin";
        private const string SECURITY_SETTINGS_CCU_PNP_ENABLED = "CcuPnPEnabled";
        private const string SECURITY_SETTINGS_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM = "ListOnlyUnassignedCardsInPersonForm";
        private const string SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS = "DelayToSaveAlarmsFromCardReaders";
        private const string SECURITY_SETTINGS_UNIQUE_A_KEY_CR_RESTICTION = "UniqueAKeyCSRestriction";
        private const string SECURITY_SETTINGS_CARD_READER_ALLOW_PIN_CACHING_IN_MENU = "CardReaderAllowPinCachingInMenu";
        private const string SECURITY_SETTINGS_MINIMAL_CODE_LENGTH = "MinimalCodeLength";
        private const string SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH = "MaximalCodeLength";
        private const string SECURITY_SETTINGS_IS_PIN_CONFIRMATION_OBLIGATORY = "IsPinConfirmationObligatory";
        private SyncDictionary<string, int> _securitySettingsIndexes = new SyncDictionary<string, int>();

        private const string LABEL_MINIMAL_CODE_LENGTH = "MinimalCodeLength";
        private const string LABEL_MAXIMAL_CODE_LENGTH = "MaximalCodeLength";

        private void FillSecuritySettings()
        {
            _epgSecuritySettings.Items.Clear();
            _epgSecuritySettings.ShowCustomProperties = true;

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_UNIQUE_NOT_NULL_PERSONAL_KEY,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_UNIQUE_NOT_NULL_PERSONAL_KEY, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbUniqueNotNullPersonalKey"));

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbCardPinLoginRequire"));

            _securitySettingsIndexes.Add(
                SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY,
                _epgSecuritySettings.Items.Add(
                    SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY,
                    "0",
                    false,
                    LABEL_SECURITY_SETTINGS,
                    LABEL_SECURITY_SETTINGS,
                    true,
                    "_lChangePassEvery"));

            _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY]]
                .CustomEditor = new UINumericUpDownEditor();

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_LOCK_CLIENT_APPLICATION,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_LOCK_CLIENT_APPLICATION, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbLockClientApplication"));

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbCcuConfigurationToServerByPassword"));

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_REQUIRED_SECURE_PIN,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_REQUIRED_SECURE_PIN, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbRequiredSecurePin"));

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_CCU_PNP_ENABLED,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_CCU_PNP_ENABLED, true, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbCcuPnPEnabled"));

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbListOnlyUnassignedCardsInPersonForm"));

            _securitySettingsIndexes.Add(
                SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS,
                _epgSecuritySettings.Items.Add(
                    SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS,
                    CgpServerGlobals.DEFAULT_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS.ToString(),
                    false,
                    LABEL_SECURITY_SETTINGS,
                    LABEL_SECURITY_SETTINGS,
                    true,
                    "_lDelayToSaveAlarmsFromCardReaders"));

            _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]]
                .CustomEditor = new UINumericUpDownEditor();

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_UNIQUE_A_KEY_CR_RESTICTION,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_UNIQUE_A_KEY_CR_RESTICTION, true, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbUniqueAKeyCSRestriction"));

            _securitySettingsIndexes.Add(SECURITY_SETTINGS_CARD_READER_ALLOW_PIN_CACHING_IN_MENU,
                _epgSecuritySettings.Items.Add(SECURITY_SETTINGS_CARD_READER_ALLOW_PIN_CACHING_IN_MENU, false, false,
                    LABEL_SECURITY_SETTINGS, LABEL_SECURITY_SETTINGS, true, "_cbCardReaderAllowPinCachingInMenu"));

            _securitySettingsIndexes.Add(
                SECURITY_SETTINGS_MINIMAL_CODE_LENGTH,
                _epgSecuritySettings.Items.Add(
                    SECURITY_SETTINGS_MINIMAL_CODE_LENGTH,
                    "4",
                    false,
                    LABEL_SECURITY_SETTINGS,
                    LABEL_SECURITY_SETTINGS,
                    true,
                    "_lMinimalCodeLength"));

            _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]]
                .CustomEditor = new CodeLengthNumericUpDownEditor();

            _securitySettingsIndexes.Add(
                SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH,
                _epgSecuritySettings.Items.Add(
                    SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH,
                    "12",
                    false,
                    LABEL_SECURITY_SETTINGS,
                    LABEL_SECURITY_SETTINGS,
                    true,
                    "_lMaximalCodeLength"));

            _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]]
                .CustomEditor = new CodeLengthNumericUpDownEditor();

            _securitySettingsIndexes.Add(
                SECURITY_SETTINGS_IS_PIN_CONFIRMATION_OBLIGATORY,
                _epgSecuritySettings.Items.Add(
                    SECURITY_SETTINGS_IS_PIN_CONFIRMATION_OBLIGATORY,
                    true,
                    false,
                    LABEL_SECURITY_SETTINGS,
                    LABEL_SECURITY_SETTINGS,
                    true,
                    "_lIsPinConfirmationObligatory"));

            _epgSecuritySettings.AutoSizeProperties = false;

            _epgSecuritySettings.Refresh();
        }

        private void RunOnEnter(Form form)
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(true))
                return;
            _tcGeneralOptions.TabPages.Remove(_tpServerControl);
            if (CgpClient.Singleton.IsMainServerProviderAvailable)
            {
                if (CgpClient.Singleton.IsLoggedIn && CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.OldGeneralOptionsAdmin)))
                {
                    _tcGeneralOptions.TabPages.Insert(1, _tpServerControl);
                    LocalizationHelper.TranslateForm(this);
                }
            }
            SetColorFormEnter(this);

            ServerGeneralOptionsEnter();

            _loggedAsSuperAdmin = LoggedAsSuperAdmin;

            if (_loggedAsSuperAdmin == null || !_loggedAsSuperAdmin.Value)
            {
                _lMaxEventsCountForInsert.Visible = false;
                _eMaxEventsCountForInsert.Visible = false;
                _lDelayForSaveEvents.Visible = false;
                _eDelayForSaveEvents.Visible = false;
                _cbCorrectDeserializationFailures.Visible = false;

                _lClientSessionTimeout.Top = 14;
                _eClientSessionTimeout.Top = 12;
            }
            else
            {
                _lMaxEventsCountForInsert.Visible = true;
                _eMaxEventsCountForInsert.Visible = true;
                _lDelayForSaveEvents.Visible = true;
                _eDelayForSaveEvents.Visible = true;
                _cbCorrectDeserializationFailures.Visible = true;

                _lClientSessionTimeout.Top = 66;
                _eClientSessionTimeout.Top = 64;
            }

            if (_epgSecuritySettings.Items.Count > 0)
            {
                _epgSecuritySettings.Items[0].IsReadOnly =
                    CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.TimetecCommunicationIsEnabled();
            }

            SafeThread.StartThread(HideDisableTabPages);
        }

        private void SetColorFormEnter(Form form)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Form>(SetColorFormEnter), form);
            }
            else
            {
                if (_tbmTimeZone.ImageTextBox.ForeColor != CgpClientMainForm.Singleton.GetDragDropTextColor)
                    _tbmTimeZone.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                if (_tbmTimeZone.ImageTextBox.BackColor != CgpClientMainForm.Singleton.GetDragDropBackgroundColor)
                    _tbmTimeZone.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;

                if (_tbmEventlogTimeZone.ImageTextBox.ForeColor != CgpClientMainForm.Singleton.GetDragDropTextColor)
                    _tbmEventlogTimeZone.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                if (_tbmEventlogTimeZone.ImageTextBox.BackColor != CgpClientMainForm.Singleton.GetDragDropBackgroundColor)
                    _tbmEventlogTimeZone.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;

                if (_tbmLogin.ImageTextBox.ForeColor != CgpClientMainForm.Singleton.GetDragDropTextColor)
                    _tbmLogin.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                if (_tbmLogin.ImageTextBox.BackColor != CgpClientMainForm.Singleton.GetDragDropBackgroundColor)
                    _tbmLogin.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;

                if (_tbmEventlogsReportsTimezone.ImageTextBox.ForeColor != CgpClientMainForm.Singleton.GetDragDropTextColor)
                    _tbmEventlogsReportsTimezone.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                if (_tbmEventlogsReportsTimezone.ImageTextBox.BackColor != CgpClientMainForm.Singleton.GetDragDropBackgroundColor)
                    _tbmEventlogsReportsTimezone.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
            }
        }

        protected override void AfterTranslateForm()
        {
            FillLicenseSettings();
        }

        private string GetFunctionalPropertiesText(Dictionary<string, string> properties)
        {
            StringBuilder result = new StringBuilder();
            foreach (string key in properties.Keys)
            {
                result.AppendLine(key + ": " + properties[key]);
            }
            return result.ToString();
        }

        protected override bool VerifySources()
        {
            return true;
        }

        #region ServerGeneralOptions

        private readonly object _lockServerGeneralOptions = new object();

        /// <summary>
        /// Loads general options from server side
        /// </summary>

        private void SetValues()
        {
            if (CgpClient.Singleton.MainServerProvider != null &&
                        CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                SetServerGeneralOptionsToEditorsSMTP();
                SetServerGeneralOptionsToEditorsSerialPort();
                EnableServerGeneralOptionsEditors();
                LoadDatabaseBackupSettingsFromRegistry();
                LoadDatabaseExpirationEventlogSettingsFromRegistry();
                CustomerAndSupplierInfoToEditors();

                //show solor settings
                LoadColorFromRegistry();
                //show autoclose settings
                LoadFromRegAutocloseSettings();
                //show change password days
                LoadFromRegSecuritySettings();
                //show eventlogs settings
                LoadFromRegEventlogs();
                //show advanced access settings
                LoadFromRegAdvancedAccessSettings();
                //show advanced settings
                LoadFromRegAdvancedSettings();

                if (!CgpClient.Singleton.IsLoadedPlugin("NCAS plugin"))
                {
                    if (_tcGeneralOptions.TabPages.Contains(_tpEventlogs))
                        _tcGeneralOptions.TabPages.Remove(_tpEventlogs);

                    if (_tcGeneralOptions.TabPages.Contains(_tpAdvancedAccessSettings))
                        _tcGeneralOptions.TabPages.Remove(_tpAdvancedAccessSettings);

                    if (_tcGeneralOptions.TabPages.Contains(_tpAdvancedSettings))
                        _tcGeneralOptions.TabPages.Remove(_tpAdvancedSettings);
                }
                else
                {
                    if (!_tcGeneralOptions.TabPages.Contains(_tpEventlogs))
                        _tcGeneralOptions.TabPages.Add(_tpEventlogs);

                    if (!_tcGeneralOptions.TabPages.Contains(_tpAdvancedAccessSettings))
                        _tcGeneralOptions.TabPages.Add(_tpAdvancedAccessSettings);

                    if (!_tcGeneralOptions.TabPages.Contains(_tpAdvancedSettings))
                        _tcGeneralOptions.TabPages.Add(_tpAdvancedSettings);
                }

                Contal.IwQuick.Threads.SafeThread.StartThread(SetSqlServerRunOnAnotherMachine);
            }
            else
            {
                DisableServerGeneralOptionsEditors();
            }
        }

        private Cgp.Server.Beans.ServerGeneralOptions LoadServerGenaralOptions()
        {
            try
            {
                lock (_lockServerGeneralOptions)
                {
                    if (_serverGeneralOptions == null && CgpClient.Singleton.MainServerProvider != null &&
                            CgpClient.Singleton.MainServerProvider.IsSessionValid)
                    {
                        _serverGeneralOptions =
                            CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider
                                .ReturnServerGeneralOptions();
                    }
                    else if (CgpClient.Singleton.MainServerProvider == null ||
                            !CgpClient.Singleton.MainServerProvider.IsSessionValid)
                    {
                        _serverGeneralOptions = null;

                    }

                    return _serverGeneralOptions;
                }
            }
            catch
            {
                _serverGeneralOptions = null;
                return null;
            }
        }

        private bool IsEqualsToServer()
        {
            Server.Beans.ServerGeneralOptions tmpSGO = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions.SmtpServer != tmpSGO.SmtpServer) return false;
            if (_serverGeneralOptions.SmtpPort != tmpSGO.SmtpPort) return false;
            if (_serverGeneralOptions.SmtpSourceEmailAddress != tmpSGO.SmtpSourceEmailAddress) return false;
            if (_serverGeneralOptions.SmtpSubject != tmpSGO.SmtpSubject) return false;

            if (_serverGeneralOptions.SerialPort != tmpSGO.SerialPort) return false;
            if (_serverGeneralOptions.SerialPortBaudRate != tmpSGO.SerialPortBaudRate) return false;
            if (_serverGeneralOptions.SerialPortCarrierDetect != tmpSGO.SerialPortCarrierDetect) return false;
            if (_serverGeneralOptions.SerialPortDataBits != tmpSGO.SerialPortDataBits) return false;
            if (_serverGeneralOptions.SerialPortFlowControl != tmpSGO.SerialPortFlowControl) return false;
            if (_serverGeneralOptions.SerialPortParity != tmpSGO.SerialPortParity) return false;
            if (_serverGeneralOptions.SerialPortParityCheck != tmpSGO.SerialPortParityCheck) return false;
            if (_serverGeneralOptions.SerialPortPin != tmpSGO.SerialPortPin) return false;
            if (_serverGeneralOptions.SerialPortStopBits != tmpSGO.SerialPortStopBits) return false;
            return true;
        }

        private void SetServerGeneralOptionsToEditorsSMTP()
        {
            if (!_changedTabPages.Contains(REMOTE_SERVICES_SETTINGS) || Dialog.Question(GetString("QuestionReloadRemotingServicesSettings")))
            {
                _epgSMTPServerSettings.Items[0].Value = (_serverGeneralOptions.SmtpServer == null ?
                    string.Empty : _serverGeneralOptions.SmtpServer);
                _epgSMTPServerSettings.Items[1].Value = (_serverGeneralOptions.SmtpPort == null ? 0 : _serverGeneralOptions.SmtpPort);
                _epgSMTPServerSettings.Items[2].Value = (_serverGeneralOptions.SmtpSourceEmailAddress == null ?
                    string.Empty : _serverGeneralOptions.SmtpSourceEmailAddress);
                _epgSMTPServerSettings.Items[3].Value = (_serverGeneralOptions.SmtpSubject == null ? string.Empty : _serverGeneralOptions.SmtpSubject);
                _epgSMTPServerSettings.Items[4].Value = (_serverGeneralOptions.SmtpCredentials == null ? string.Empty : _serverGeneralOptions.SmtpCredentials);
                _epgSMTPServerSettings.Items[5].Value = (_serverGeneralOptions.SmtpSsl == null ? false : _serverGeneralOptions.SmtpSsl);

                LoadNtpSettingsFromDatabase();
                ResetChanges(REMOTE_SERVICES_SETTINGS);
            }
        }

        private void CustomerAndSupplierInfoToEditors()
        {
            if (!_changedTabPages.Contains(CUSTOMER_AND_SUPPLIER_INFO) ||
                Dialog.Question(GetString("QuestionReloadCustomerAndSupplierInfo")))
            {
                _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();

                _epgCustomerInfo.Items[0].Value = _serverGeneralOptions.CustomerCompanyName == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerCompanyName;
                _epgCustomerInfo.Items[1].Value = _serverGeneralOptions.CustomerDeliveryAddress == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerDeliveryAddress;
                _epgCustomerInfo.Items[2].Value = _serverGeneralOptions.CustomerZipCode == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerZipCode;
                _epgCustomerInfo.Items[3].Value = _serverGeneralOptions.CustomerCityState == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerCityState;
                _epgCustomerInfo.Items[4].Value = _serverGeneralOptions.CustomerCountry == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerCountry;
                _epgCustomerInfo.Items[5].Value = _serverGeneralOptions.CustomerPhone == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerPhone;
                _epgCustomerInfo.Items[6].Value = _serverGeneralOptions.CustomerWebsite == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerWebsite;
                _epgCustomerInfo.Items[7].Value = _serverGeneralOptions.CustomerContactPerson == null
                    ? string.Empty
                    : _serverGeneralOptions.CustomerContactPerson;

                _epgSupplierInfo.Items[0].Value = _serverGeneralOptions.SupplierCompanyName == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierCompanyName;
                _epgSupplierInfo.Items[1].Value = _serverGeneralOptions.SupplierDeliveryAddress == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierDeliveryAddress;
                _epgSupplierInfo.Items[2].Value = _serverGeneralOptions.SupplierZipCode == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierZipCode;
                _epgSupplierInfo.Items[3].Value = _serverGeneralOptions.SupplierCityState == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierCityState;
                _epgSupplierInfo.Items[4].Value = _serverGeneralOptions.SupplierCountry == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierCountry;
                _epgSupplierInfo.Items[5].Value = _serverGeneralOptions.SupplierPhone == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierPhone;
                _epgSupplierInfo.Items[6].Value = _serverGeneralOptions.SupplierWebsite == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierWebsite;
                _epgSupplierInfo.Items[7].Value = _serverGeneralOptions.SupplierContactPerson == null
                    ? string.Empty
                    : _serverGeneralOptions.SupplierContactPerson;

                LoadSupplierLogo();
                ResetChanges(CUSTOMER_AND_SUPPLIER_INFO);
            }
        }

        private void LoadSupplierLogo()
        {
            try
            {
                _binaryPhoto = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.GetSupplierLogo();
                if (_binaryPhoto != null)
                {
                    using (MemoryStream photoStream = new MemoryStream())
                    {
                        photoStream.Write(_binaryPhoto.BinaryData, 0, _binaryPhoto.BinaryData.Length);
                        _pbSupplierLogo.Image = new Bitmap(photoStream);
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void SetServerGeneralOptionsToEditorsSerialPort()
        {
            if (!_changedTabPages.Contains(SERIAL_PORT_SETTINGS) || Dialog.Question(GetString("QuestionReloadSerialPortSettings")))
            {
                _cbPort.Text = _serverGeneralOptions.SerialPort;
                _cbPortBaudRate.Text = _serverGeneralOptions.SerialPortBaudRate.ToString();
                _cbPortDataBits.Text = _serverGeneralOptions.SerialPortDataBits.ToString();
                _cbPortParity.Text = _serverGeneralOptions.SerialPortParity;
                _cbPortStopBits.Text = _serverGeneralOptions.SerialPortStopBits;
                _cbPortFlowControl.Text = _serverGeneralOptions.SerialPortFlowControl;
                if (_serverGeneralOptions.SerialPortParityCheck != null)
                    _chbPortParityCheck.Checked = (bool)_serverGeneralOptions.SerialPortParityCheck;
                if (_serverGeneralOptions.SerialPortCarrierDetect != null)
                    _chbPortCarrierDetect.Checked = (bool)_serverGeneralOptions.SerialPortCarrierDetect;
                _ePortPin.Text = _serverGeneralOptions.SerialPortPin.ToString();
                ResetChanges(SERIAL_PORT_SETTINGS);
            }
        }

        private void SetServerGeneralOptionsFromEditorsSMTP()
        {
            _serverGeneralOptions.SmtpServer = (string)_epgSMTPServerSettings.Items[0].Value;
            _serverGeneralOptions.SmtpPort = (int)_epgSMTPServerSettings.Items[1].Value;
            _serverGeneralOptions.SmtpSourceEmailAddress = (string)_epgSMTPServerSettings.Items[2].Value;
            _serverGeneralOptions.SmtpSubject = (string)_epgSMTPServerSettings.Items[3].Value;
            _serverGeneralOptions.SmtpCredentials = (string)_epgSMTPServerSettings.Items[4].Value;
            _serverGeneralOptions.SmtpSsl = (bool)_epgSMTPServerSettings.Items[5].Value;
        }

        private void SetServerGeneralOptionsFromEditorsSerialPort()
        {
            _serverGeneralOptions.SerialPort = _cbPort.Text;
            _serverGeneralOptions.SerialPortBaudRate = Int32.Parse(_cbPortBaudRate.Text);
            _serverGeneralOptions.SerialPortDataBits = Int32.Parse(_cbPortDataBits.Text);
            _serverGeneralOptions.SerialPortParity = _cbPortParity.Text;
            _serverGeneralOptions.SerialPortStopBits = _cbPortStopBits.Text;
            _serverGeneralOptions.SerialPortFlowControl = _cbPortFlowControl.Text;
            _serverGeneralOptions.SerialPortParityCheck = _chbPortParityCheck.Checked;
            _serverGeneralOptions.SerialPortCarrierDetect = _chbPortCarrierDetect.Checked;
            _serverGeneralOptions.SerialPortPin = Int32.Parse(_ePortPin.Text);
        }

        //if server
        private bool ServerGeneralOptionsSmtpValueOk()
        {
            //if server general options not loaded then ignore
            if (_serverGeneralOptions == null) return true;

            //SMTP settings do not need by set
            //but if is must be correct confirm only mail is OK
            if (!string.IsNullOrEmpty(_epgSMTPServerSettings.Items[2].Value.ToString()) &&
                !IwQuick.Net.EmailAddress.IsValid(_epgSMTPServerSettings.Items[2].Value.ToString()))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _epgSMTPServerSettings,
                    GetString("ErrorWrongEmailAddress"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tcGeneralOptions.SelectedIndex = _tcGeneralOptions.TabPages.IndexOf(_tpRemoteServicesSettings);
                _epgSMTPServerSettings.SelectItemValue(_epgSMTPServerSettings.Items[2].Name);
                return false;
            }
            return true;
        }

        private bool ServerGeneralOptionsSerialPortValueOk()
        {
            //if server general options not loaded then ignore
            if (_serverGeneralOptions == null) return true;

            int outInt;
            if (!Int32.TryParse(_ePortPin.Text, out outInt))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ePortPin,
                    GetString("ErrorInsertSerialPortPin"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tcGeneralOptions.SelectedIndex = _tcGeneralOptions.TabPages.IndexOf(_tpSerialPort);
                _ePortPin.Focus();
                return false;
            }
            return true;
        }

        private void DisableServerGeneralOptionsEditors()
        {
            //SMTP Settings
            _epgSMTPServerSettings.Enabled = false;
            //Serial Port Settings
            _cbPort.Enabled = false;
            _cbPortBaudRate.Enabled = false;
            _cbPortDataBits.Enabled = false;
            _cbPortParity.Enabled = false;
            _cbPortStopBits.Enabled = false;
            _cbPortFlowControl.Enabled = false;
            _chbPortParityCheck.Enabled = false;
            _chbPortCarrierDetect.Enabled = false;
            _ePortPin.Enabled = false;
        }

        private void EnableServerGeneralOptionsEditors()
        {
            //SMTP Settings
            _epgSMTPServerSettings.Enabled = true;
            //Serial Port Settings
            _cbPort.Enabled = true;
            _cbPortBaudRate.Enabled = true;
            _cbPortDataBits.Enabled = true;
            _cbPortParity.Enabled = true;
            _cbPortStopBits.Enabled = true;
            _cbPortFlowControl.Enabled = true;
            _chbPortParityCheck.Enabled = true;
            _chbPortCarrierDetect.Enabled = true;
            _ePortPin.Enabled = true;
        }

        private void ServerGeneralOptionsEnter()
        {
            if (!_readNewGeneralOptionsFromServer)
                return;

            _readNewGeneralOptionsFromServer = false;
            LoadServerGenaralOptions();
            SetValues();
        }

        private void SaveSmtpClick(object sender, EventArgs e)
        {
            SafeThread.StartThread(DoSaveSmtp);
        }

        private void DoSaveSmtp()
        {
            if (_serverGeneralOptions == null) return;
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                if (!ServerGeneralOptionsSmtpValueOk()) return;
                SetServerGeneralOptionsFromEditorsSMTP();
                ResetChanges(REMOTE_SERVICES_SETTINGS);
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistrySmtp(_serverGeneralOptions);
                SaveNtpSettingsToDatabase();
            }
        }

        private void SaveSerialPortClick(object sender, EventArgs e)
        {
            if (_serverGeneralOptions == null) return;
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                if (!ServerGeneralOptionsSerialPortValueOk()) return;
                SetServerGeneralOptionsFromEditorsSerialPort();
                ResetChanges(SERIAL_PORT_SETTINGS);
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistrySerialPort(_serverGeneralOptions);
            }
        }
        #endregion

        private void _bStop_Click(object sender, EventArgs e)
        {
            _bStop.Enabled = false;

            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                if (CgpClient.Singleton.IsMainServerProviderAvailable)
                {
                    Exception error;

                    if (!CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.SuperAdmin)))
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErorrUserIsNotSuperadmin"));
                        _bStop.Enabled = true;
                        return;
                    }

                    if (!Contal.IwQuick.UI.Dialog.Question(GetString("QuestionStopServer")))
                    {
                        _bStop.Enabled = true;
                        return;
                    }

                    CgpClient.Singleton.MainServerProvider.StopServer(out error);

                    if (error != null)
                    {
                        if (error is Contal.IwQuick.AccessDeniedException)
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorStopServerAccessDenied"));
                        else
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorStopServerFailed"));
                    }
                    else
                    {
                        CgpClient.Singleton.ServerStoped();
                    }
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorStopServerFailed"));
                }
            }

            _bStop.Enabled = true;
        }

        private void _tpServerControl_Enter(object sender, EventArgs e)
        {
            bool showResetUserSession = false;
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
            {
                if (CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.SuperAdmin)))
                {
                    showResetUserSession = true;
                }

                SafeThread.StartThread(ShowServerInformations);
            }

            _gbResetUserSession.Visible = showResetUserSession;
        }

        private const string COLUMN_SERVER_INFORMATION_TYPE = "ServerInformationType";
        private const string COLUMN_SERVER_INFORMATION_VALUE = "ServerInformationValue";

        /// <summary>
        /// Load server informations and show them in the datagrid view
        /// </summary>
        private void ShowServerInformations()
        {
            Dictionary<string, object> serverInformations = CgpClient.Singleton.MainServerProvider.GetServerInformations();

            if (InvokeRequired)
                Invoke((MethodInvoker)delegate
                {
                    _dgServerInformations.Rows.Clear();
                    if (!_dgServerInformations.Columns.Contains(COLUMN_SERVER_INFORMATION_TYPE))
                    {
                        _dgServerInformations.Columns.Add(COLUMN_SERVER_INFORMATION_TYPE, COLUMN_SERVER_INFORMATION_TYPE);
                        _dgServerInformations.Columns[COLUMN_SERVER_INFORMATION_TYPE].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    if (!_dgServerInformations.Columns.Contains(COLUMN_SERVER_INFORMATION_VALUE))
                    {
                        _dgServerInformations.Columns.Add(COLUMN_SERVER_INFORMATION_VALUE, COLUMN_SERVER_INFORMATION_VALUE);
                        _dgServerInformations.Columns[COLUMN_SERVER_INFORMATION_VALUE].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    if (serverInformations != null && serverInformations.Count > 0)
                    {
                        int actRow = _dgServerInformations.Rows.Count;
                        _dgServerInformations.Rows.Add(serverInformations.Count);
                        foreach (KeyValuePair<string, object> kvp in serverInformations)
                        {
                            _dgServerInformations.Rows[actRow].Cells[COLUMN_SERVER_INFORMATION_TYPE].Value = GetString("ServerInformationType_" + kvp.Key);
                            _dgServerInformations.Rows[actRow].Cells[COLUMN_SERVER_INFORMATION_VALUE].Value = kvp.Value.ToString();
                            actRow++;
                        }
                    }
                });
        }

        double _remainingMiliseconds = -1;
        long _remainingTimeRefreshDelay = 60000;
        TimerManager _remainingTimerManager = new TimerManager();

        public void SynchronizeRemainingTime()
        {
            _remainingMiliseconds = CgpClient.Singleton.MilisecondsRemaining();
            _remainingTimerManager.StopAll();
            _remainingTimerManager.StartTimer(_remainingTimeRefreshDelay, false, new DOnTimerEvent(UpdateRemainingMiliseconds));
            FillLicenseSettings();
            RefreshRemainingTime();
        }

        private void RefreshRemainingTime()
        {
            if (_epgLicenceInfo.InvokeRequired)
            {
                _epgLicenceInfo.BeginInvoke(new DVoid2Void(RefreshRemainingTime));
            }
            else
            {
                bool demolicence;

                try
                {
                    demolicence = CgpClient.Singleton.MainServerProvider.DemoLicence;
                }
                catch (Exception)
                {
                    demolicence = false;
                }

                if (!demolicence)
                {
                    StopRefreshingDemoTime();
                    return;
                }

                int hoursLeft = (int)(_remainingMiliseconds / (1000 * 60 * 60));
                int minutesLeft = (int)(_remainingMiliseconds - (hoursLeft * 60 * 60 * 1000)) / (1000 * 60);
                if (_epgLicenceInfo.Items.Count > 0)
                    _epgLicenceInfo.Items[0].Value = hoursLeft + " h. " + minutesLeft + " min.";
            }
        }

        private bool UpdateRemainingMiliseconds(TimerCarrier timerCarrier)
        {
            _remainingMiliseconds = _remainingMiliseconds - _remainingTimeRefreshDelay <= 0 ? 0 : _remainingMiliseconds - _remainingTimeRefreshDelay;
            RefreshRemainingTime();
            return true;
        }

        private void _tpLicence_Enter(object sender, EventArgs e)
        {
            FillLicenseSettings();
        }

        private string GetExpirationText(DateTime dateTime)
        {
            if (dateTime.ToShortDateString() == DateTime.MaxValue.ToShortDateString())
            {
                return LocalizationHelper.GetString("NoExpiration");
            }
            else if (dateTime.ToShortDateString() == DateTime.MinValue.ToShortDateString())
            {
                return LocalizationHelper.GetString("LicenceExpirationNotSet");
            }
            else
            {
                return dateTime.ToShortDateString();
            }
        }

        private bool IsSpecificIpAddressInSubnet(string subnetAddress, string subnetMask, string specificIp, TextBox textBoxForNotification)
        {
            try
            {
                return (IPHelper.IsSameNetwork4(subnetAddress, subnetMask, specificIp));
            }
            catch (Exception)
            {
                if (textBoxForNotification != null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, textBoxForNotification,
                        LocalizationHelper.GetString("ErrorIPNotValidOrNotInNetwork"), ControlNotificationSettings.Default);
                }
                return false;
            }
        }

        private bool IsConvertableToIP(string IPAddressString)
        {
            IPAddress currentIP = null;
            return IPAddress.TryParse(IPAddressString, out currentIP);
        }

        private void _bDHCPSave_Click(object sender, EventArgs e)
        {
            if (DHCPGroup.StaticNetworksChecks(_tbSubnet, _tbNetworkMask, _tbGateway, true, new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom),
                LocalizationHelper.GetString("ErrorIPNotValid")))
            {
                if (AreIPRangesValid())
                {
                    if (IsEmptyOrValidIpTextBox(_tbDNS) && IsEmptyOrValidIpTextBox(_tbAlternateDNS))
                    {
                        if (IsDnsSuffixEmptyOrValid())
                        {
                            if (_tbGateway.Text == "" || IPHelper.IsSameNetwork4(_tbSubnet.Text, _tbNetworkMask.Text, _tbGateway.Text))
                            {
                                if (DHCPGroup.LeaseTimeCheck(_tbLeaseTime, true, new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom), LocalizationHelper.GetString("ErrorInvalidData"))
                                    &&
                                    DHCPGroup.MaxLeaseTimeCheck(_tbLeaseTime, _tbMaxLeaseTime, true, new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom), LocalizationHelper.GetString("ErrorInvalidData")))
                                {
                                    if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
                                    {
                                        //string serverIP = DHCPGroup.GetServerIP(IPAddress.Parse(_tbSubnet.Text), IPAddress.Parse(_tbNetworkMask.Text));
                                        string serverIP = CgpClient.Singleton.MainServerProvider.GetServerIpAddress(IPAddress.Parse(_tbSubnet.Text), IPAddress.Parse(_tbNetworkMask.Text));
                                        if (serverIP != null && serverIP != string.Empty)
                                        {
                                            ResetChanges(DHCP_SERVER_SETTINGS);

                                            Dictionary<string, string[]> macPrefixAndIpRange = (Dictionary<string, string[]>)_cbMACMask.Tag;
                                            DHCPGroup newGroup = new DHCPGroup(_tbSubnet.Text, _tbNetworkMask.Text, _tbGateway.Text,
                                                new string[] { _tbDNS.Text, _tbAlternateDNS.Text }, _tbDnsSuffix.Text, uint.Parse(_tbLeaseTime.Text),
                                                uint.Parse(_tbMaxLeaseTime.Text), serverIP, macPrefixAndIpRange);
                                            if (_cbIPGroup.Tag == null)
                                            {
                                                _cbIPGroup.Tag = new DHCPGroups();
                                            }
                                            if (((DHCPGroups)_cbIPGroup.Tag).Add(newGroup))
                                            {
                                                CgpClient.Singleton.MainServerProvider.SetDHCPGroups((DHCPGroups)_cbIPGroup.Tag);
                                                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bDHCPSave,
                                                    LocalizationHelper.GetString("IPGroupAdded"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                                                RefreshIpGroups();
                                                SelectIPGroup(newGroup.Net.ToString());
                                            }
                                            else
                                            {
                                                if (_cbIPGroup.Text != LocalizationHelper.GetString("NewIpGroup"))
                                                {
                                                    if (GetCurrentDHCPGroup() != null)
                                                    {
                                                        GetCurrentDHCPGroup().Replace(newGroup);
                                                        CgpClient.Singleton.MainServerProvider.SetDHCPGroups((DHCPGroups)_cbIPGroup.Tag);
                                                        ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bDHCPSave,
                                                            LocalizationHelper.GetString("IPGroupUpdated"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                                                        RefreshIpGroups();
                                                    }
                                                }
                                                else
                                                {
                                                    ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                                                    _cbIPGroup, LocalizationHelper.GetString("ErrorIpGroupCollision"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                                                _bDHCPSave, LocalizationHelper.GetString("ErrorServerNetwork"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(LocalizationHelper.GetString("ErrorUnableToConnectToServer"), "DHCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                                    _tbGateway, LocalizationHelper.GetString("ErrorIPNotValidOrNotInNetwork"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            }
                        }
                    }
                }
            }
        }

        private void SelectIPGroup(string networkName)
        {
            foreach (object item in _cbIPGroup.Items)
            {
                if (item.ToString() == networkName)
                {
                    _cbIPGroup.SelectedItem = item;
                    break;
                }
            }
        }

        private void RefreshIpGroups()
        {
            object selectedItem = _cbIPGroup.SelectedItem;
            _cbIPGroup.Items.Clear();
            foreach (DHCPGroup group in (DHCPGroups)_cbIPGroup.Tag)
            {
                _cbIPGroup.Items.Add(group.Net);
            }
            _cbIPGroup.Items.Add(LocalizationHelper.GetString("NewIpGroup"));
            if (selectedItem != null && _cbIPGroup.Items.Contains(selectedItem))
            {
                _cbIPGroup.SelectedItem = selectedItem;
            }
            else if (_cbIPGroup.Items.Count > 0)
            {
                _cbIPGroup.SelectedItem = _cbIPGroup.Items[0];
            }
        }

        private bool IsDnsSuffixEmptyOrValid()
        {
            if (_tbDnsSuffix.Text == "")
            {
                return true;
            }
            Regex aRegex = new Regex(@"(?=^.{1,254}$)(^(?:(?!\d+\.|-)[a-zA-Z0-9_\-]{1,63}(?<!-)\.?)+(?:[a-zA-Z]{2,})$)");
            if (!aRegex.Match(_tbDnsSuffix.Text).Success)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                          _tbDnsSuffix, LocalizationHelper.GetString("ErrorWrongFormat"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                _tbDnsSuffix.Focus();
                return false;
            }
            return true;
        }

        private void ClearFields(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control.GetType().Name == typeof(TextBox).Name)
                {
                    control.Text = "";
                }
                if (control.GetType().Name == typeof(ComboBox).Name)
                {
                    ((ComboBox)control).Items.Clear();
                }
                if (control.GetType().Name == typeof(GroupBox).Name)
                {
                    ClearFields(control);
                }
            }
        }

        private bool IsEmptyOrValidIpTextBox(TextBox ipAddressTextBox)
        {
            if (ipAddressTextBox.Text == "")
            {
                return true;
            }
            if (!IPHelper.IsValid4(ipAddressTextBox.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, ipAddressTextBox, LocalizationHelper.GetString("ErrorIPNotValid"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                ipAddressTextBox.Focus();
                return false;
            }
            return true;
        }

        private bool AreIPRangesValid()
        {
            if (_cbMACMask.Tag != null)
            {
                Dictionary<string, string[]> macRanges = (Dictionary<string, string[]>)_cbMACMask.Tag;
                int keyIndex = 0;
                foreach (string macAddress in macRanges.Keys)
                {
                    //check only enabled ranges
                    if (macRanges.ElementAt(keyIndex).Value[2].ToLower() == "true")
                    {
                        if (!IPHelper.IsValid4(macRanges[macAddress][0]) || !IPHelper.IsSameNetwork4(_tbSubnet.Text, _tbNetworkMask.Text, macRanges[macAddress][0]))
                        {
                            _cbMACMask.SelectedIndex = keyIndex;
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbIPRangeFrom, LocalizationHelper.GetString("ErrorIPNotValidOrNotInNetwork"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            _tbIPRangeFrom.Focus();
                            return false;
                        }
                        if (!IPHelper.IsValid4(macRanges[macAddress][1]) || !IPHelper.IsSameNetwork4(_tbSubnet.Text, _tbNetworkMask.Text, macRanges[macAddress][1]))
                        {
                            _cbMACMask.SelectedIndex = keyIndex;
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbIPRangeTo, LocalizationHelper.GetString("ErrorIPNotValidOrNotInNetwork"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            _tbIPRangeTo.Focus();
                            return false;
                        }
                        if (!DHCPGroup.CheckRangeOK(new IPAddress[] { IPAddress.Parse(macRanges[macAddress][0]), IPAddress.Parse(macRanges[macAddress][1]) }))
                        {
                            _cbMACMask.SelectedIndex = keyIndex;
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbIPRangeTo, LocalizationHelper.GetString("ErrorIpRangeMinMaxSwitch"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            _tbIPRangeTo.Focus();
                            return false;
                        }
                    }
                    keyIndex++;
                }
                if (IsAnyCollisionInIpRanges(macRanges))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsAnyCollisionInIpRanges(Dictionary<string, string[]> macRanges)
        {
            return IsAnyCollisionInIpRanges(macRanges, null);
        }

        private bool IsAnyCollisionInIpRanges(Dictionary<string, string[]> macRanges, Control showErrorOn)
        {
            for (int i = 0; i < macRanges.Values.Count; i++)
            {
                //check only enabled ranges
                if (macRanges.ElementAt(i).Value[2].ToLower() == "true")
                {
                    for (int j = i + 1; j < macRanges.Values.Count; j++)
                    {
                        //compare with enabled ranges only
                        if (macRanges.ElementAt(j).Value[2].ToLower() == "true")
                        {
                            if (DHCPGroup.AreIpRangesInCollision(macRanges.ElementAt(i).Value[0], macRanges.ElementAt(i).Value[1],
                                macRanges.ElementAt(j).Value[0], macRanges.ElementAt(j).Value[1]))
                            {
                                Control errorControl = showErrorOn == null ? _bDHCPSave : showErrorOn;
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, errorControl,
                                    LocalizationHelper.GetString("ErrorIpRangeCollision") + " (<" + macRanges.ElementAt(i).Value[0] + "," + macRanges.ElementAt(i).Value[1] + ">  <" + macRanges.ElementAt(j).Value[0] + "," + macRanges.ElementAt(j).Value[1] + ">)",
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void AutoFillRange(IPAddress ipAdd, IPAddress ipMask)
        {
            //_tbIPRangeTo.Text = IPHelper.FromReverseInt4(IPHelper.ToReverseInt4(IPHelper.BroadcastFromIP4(ipAdd, ipMask)) - 1).ToString();
            //int intIpAdd = IPHelper.ToReverseInt4(ipAdd) + 1;
            //_tbIPRangeFrom.Text = IPHelper.FromReverseInt4(intIpAdd).ToString();
            string ipFrom = "";
            string ipTo = "";
            if (DHCPGroup.ComputeIPRange(ipAdd.ToString(), ipMask.ToString(), out ipFrom, out ipTo))
            {
                _tbIPRangeFrom.Text = ipFrom;
                _tbIPRangeTo.Text = ipTo;
            }

        }

        private void _tpDHCPServer_Enter(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
            {
                DHCPStateChange(DHCPState.Unknown);
            }
            else
            {
                DHCPState actualState = CgpClient.Singleton.MainServerProvider.GetDHCPRunningState() ? DHCPState.Running : DHCPState.Stopped;
                DHCPStateChange(actualState);
                _chbAutoStartDHCP.Checked = CgpClient.Singleton.MainServerProvider.GetAutoDHCPStatus();
            }
        }

        private void ProcessDHCPGroups(DHCPGroups dhcpGroups)
        {
            foreach (DHCPGroup group in dhcpGroups)
            {
                _cbIPGroup.Items.Add(group.Net);
            }
        }

        private void FillComboBoxMACPrefixes(ComboBox _cbMACMask)
        {
            Dictionary<string, string[]> macDictionary = (Dictionary<string, string[]>)_cbMACMask.Tag;
            macDictionary.Clear();
            _cbMACMask.Items.Clear();
            foreach (MACAddressPrefixes prefix in Enum.GetValues(typeof(MACAddressPrefixes)))
            {
                _cbMACMask.Items.Add(prefix.ToString().Replace('_', ':'));
                macDictionary.Add(prefix.ToString().Split('_')[1], new string[3] { "0.0.0.0", "255.255.255.255", "false" });

            }
            if (_cbMACMask.Items.Count > 0)
            {
                _cbMACMask.Text = _cbMACMask.Items[0].ToString();
            }
        }

        private void _tbIPRangeFrom_Leave(object sender, EventArgs e)
        {
            //SaveIpRange();
        }

        private bool SaveIpRange()
        {
            return SaveIpRange(false);
        }

        private bool SaveIpRange(bool focusOnError)
        {
            return SaveIpRange(focusOnError, null, true);
        }

        private bool SaveIpRange(bool focusOnError, Control ipCollisionControl, bool checkIPRangeCollision)
        {
            if (_cbMACMask.Text == "" || _cbMACMask.Text == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbMACMask, LocalizationHelper.GetString("ErrorEmptyValue"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                if (focusOnError)
                {
                    _cbMACMask.Focus();
                }
            }
            else
            {
                if (_tbSubnet.Text != "" && _tbNetworkMask.Text != "" && _tbIPRangeFrom.Text != "" && _tbIPRangeTo.Text != "")
                {
                    if (IPHelper.IsValid4(_tbSubnet.Text) && IPHelper.IsValid4(_tbNetworkMask.Text) && IPHelper.IsValid4(_tbIPRangeFrom.Text) && IPHelper.IsValid4(_tbIPRangeTo.Text))
                    {
                        if (IPHelper.IsSameNetwork4(_tbSubnet.Text, _tbNetworkMask.Text, _tbIPRangeFrom.Text) && IPHelper.IsSameNetwork4(_tbSubnet.Text, _tbNetworkMask.Text, _tbIPRangeTo.Text))
                        {
                            if (DHCPGroup.CompareIpAddress(_tbIPRangeFrom.Text, _tbIPRangeTo.Text) != 2)
                            {
                                string currentMacPrefix = _cbMACMask.Text.Split(':')[1];
                                Dictionary<string, string[]> macPrefixes = (Dictionary<string, string[]>)_cbMACMask.Tag;
                                ipCollisionControl = ipCollisionControl == null ? _bDHCPSave : ipCollisionControl;
                                if (checkIPRangeCollision && IsAnyCollisionInIpRanges(macPrefixes, ipCollisionControl))
                                {
                                    return false;
                                }
                                macPrefixes[currentMacPrefix][0] = _tbIPRangeFrom.Text;
                                macPrefixes[currentMacPrefix][1] = _tbIPRangeTo.Text;
                                macPrefixes[currentMacPrefix][2] = _chbEnable.Checked.ToString();
                                return true;
                            }
                            else
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbIPRangeTo, LocalizationHelper.GetString("ErrorIpRangeMinMaxSwitch"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                                if (focusOnError)
                                {
                                    _tbIPRangeTo.Focus();
                                }
                            }
                        }
                        else
                        {
                            TextBox otherNetworkTextBox = !IPHelper.IsSameNetwork4(_tbSubnet.Text, _tbNetworkMask.Text, _tbIPRangeFrom.Text) ? _tbIPRangeFrom : _tbIPRangeTo;
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, otherNetworkTextBox, LocalizationHelper.GetString("ErrorIPNotValidOrNotInNetwork"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            if (focusOnError)
                            {
                                otherNetworkTextBox.Focus();
                            }
                        }
                    }
                    else
                    {
                        TextBox invalidTextBox = !IPHelper.IsValid4(_tbSubnet.Text) ? _tbSubnet : !IPHelper.IsValid4(_tbNetworkMask.Text) ? _tbNetworkMask : !IPHelper.IsValid4(_tbIPRangeFrom.Text) ? _tbIPRangeFrom : _tbIPRangeTo;
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, invalidTextBox, LocalizationHelper.GetString("ErrorIPNotValid"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                        if (focusOnError)
                        {
                            invalidTextBox.Focus();
                        }
                    }
                }
                else
                {
                    TextBox emptyTextBox = _tbSubnet.Text == "" ? _tbSubnet : _tbNetworkMask.Text == "" ? _tbNetworkMask : _tbIPRangeFrom.Text == "" ? _tbIPRangeFrom : _tbIPRangeTo;
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, emptyTextBox, LocalizationHelper.GetString("ErrorEmptyValue"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                    if (focusOnError)
                    {
                        emptyTextBox.Focus();
                    }
                }
            }
            return false;
        }

        private void _tbIPRangeTo_Leave(object sender, EventArgs e)
        {
            //SaveIpRange();
        }

        private void _bMaxRAnge_Click(object sender, EventArgs e)
        {
            TextBox emptyTextBox = null;
            try
            {
                emptyTextBox = _tbSubnet;
                IPAddress ipAdd = IPAddress.Parse(_tbSubnet.Text);

                emptyTextBox = _tbNetworkMask;
                IPAddress ipMask = IPAddress.Parse(_tbNetworkMask.Text);

                AutoFillRange(ipAdd, ipMask);
                SaveIpRange(true);
            }
            catch (Exception)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, emptyTextBox, LocalizationHelper.GetString("ErrorEmptyValue"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                emptyTextBox.Focus();
                _chbEnable.Checked = false;
            }
        }

        private void _cbMACMask_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbMACMask.Text.Contains(":"))
            {
                string currentPrefix = _cbMACMask.Text.Split(':')[1];
                if (_cbMACMask.Tag != null)
                {
                    Dictionary<string, string[]> macPrefixes = (Dictionary<string, string[]>)_cbMACMask.Tag;
                    if (macPrefixes[currentPrefix].Length >= 3)
                    {
                        _tbIPRangeFrom.Text = macPrefixes[currentPrefix][0];
                        _tbIPRangeTo.Text = macPrefixes[currentPrefix][1];
                        _chbEnable.Checked = macPrefixes[currentPrefix][2].ToLower() == "true";
                    }
                }
            }

            EditTextChanger(DHCP_SERVER_SETTINGS);
        }

        private void _tbIPRangeFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                _bDHCPSave.Focus();
            }
        }

        private void _chbEnable_Click(object sender, EventArgs e)
        {
            if (_chbEnable.Checked)
            {
                if (!SaveIpRange(true, _chbEnable, true))
                {
                    _chbEnable.Checked = !_chbEnable.Checked;
                }
            }
            else
            {
                if (!SaveIpRange(true, _chbEnable, false))
                {
                    _chbEnable.Checked = !_chbEnable.Checked;
                }
            }
        }

        private DHCPGroup GetCurrentDHCPGroup()
        {
            if (_cbIPGroup.Tag != null && _cbIPGroup.SelectedItem != null && _cbIPGroup.Text != LocalizationHelper.GetString("NewIpGroup"))
            {
                DHCPGroups dhcpGroups = (DHCPGroups)_cbIPGroup.Tag;
                foreach (DHCPGroup dhcpGroup in dhcpGroups)
                {
                    if (dhcpGroup.Net.Equals(IPAddress.Parse(_cbIPGroup.Text)))
                    {
                        return dhcpGroup;
                    }
                }
            }
            return null;
        }

        private void FillDhcpGroupFields(DHCPGroup currentDhcpGroup)
        {
            _tbSubnet.Text = currentDhcpGroup.Net.ToString();
            _tbNetworkMask.Text = currentDhcpGroup.Mask.ToString();
            _cbMACMask.Tag = currentDhcpGroup.MACPrefixAndIPRange;
            if (_cbMACMask.Items.Count > 0)
            {
                _cbMACMask.SelectedIndex = 0;
                _cbMACMask_SelectedIndexChanged(_cbMACMask, null);
            }
            for (int i = 0; i < currentDhcpGroup.DomainNameServers.Count; i++)
            {
                if (i == 0 && currentDhcpGroup.DomainNameServers[i] != null)
                {
                    _tbDNS.Text = currentDhcpGroup.DomainNameServers[i].ToString();
                }
                if (i == 1 && currentDhcpGroup.DomainNameServers[i] != null)
                {
                    _tbAlternateDNS.Text = currentDhcpGroup.DomainNameServers[i].ToString();
                }
            }
            if (currentDhcpGroup.DnsSuffix != null)
            {
                _tbDnsSuffix.Text = currentDhcpGroup.DnsSuffix;
            }
            if (currentDhcpGroup.DefaultGateway != null)
            {
                _tbGateway.Text = currentDhcpGroup.DefaultGateway.ToString();
            }
            _tbLeaseTime.Text = currentDhcpGroup.LeaseTime.ToString();
            _tbMaxLeaseTime.Text = currentDhcpGroup.MaxLeaseTime.ToString();
        }

        private void _cbIPGroup_MouseDown(object sender, MouseEventArgs e)
        {
            _cbIPGroup.Items.Clear();
            DHCPGroups dhcpGroups = null;
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
            {
                dhcpGroups = CgpClient.Singleton.MainServerProvider.GetServerDHCPGroups();
            }
            if (dhcpGroups != null && dhcpGroups.Count() > 0)
            {
                _cbIPGroup.Tag = dhcpGroups;
                ProcessDHCPGroups(dhcpGroups);
            }
            _cbIPGroup.Items.Add(LocalizationHelper.GetString("NewIpGroup"));
        }

        private void _bDeleteIpGroup_Click(object sender, EventArgs e)
        {
            if (_gbIPGroup.Enabled && _cbIPGroup.Text != "")
            {
                DHCPGroup dhcpGroup = GetCurrentDHCPGroup();
                DHCPGroups dhcpGroups = (DHCPGroups)_cbIPGroup.Tag;
                if (dhcpGroup != null)
                {
                    if (Dialog.Question(LocalizationHelper.GetString("QuestionRemoveIPGroup")))
                    {
                        if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
                        {
                            if (dhcpGroups.Delete(dhcpGroup))
                            {
                                CgpClient.Singleton.MainServerProvider.SetDHCPGroups(dhcpGroups);
                                ClearFields(_gbIPGroup);
                                RefreshIpGroups();
                                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _cbIPGroup, LocalizationHelper.GetString("IPGroupSuccessfullyRemoved"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            }
                            else
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbIPGroup, LocalizationHelper.GetString("ErrorRemoveIpGroupFailed"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                            }
                        }
                    }
                }
            }
        }

        private void _chbAutoStartDHCP_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
            {
                bool autoStart = _chbAutoStartDHCP.Checked;
                CgpClient.Singleton.MainServerProvider.AutoStartDHCP(autoStart);
            }
            else
            {
                _chbAutoStartDHCP.Checked = !_chbAutoStartDHCP.Checked;
            }
        }

        private void _bStopDHCP_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider != null)
            {
                if (CgpClient.Singleton.MainServerProvider.StartDHCP(false))
                {
                    DHCPStateChange(DHCPState.Stopped);
                }
            }
        }

        private enum DHCPState
        {
            Unknown = 0,
            Stopped = 1,
            Running = 2
        }

        private void _bStartDHCP_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider != null)
            {
                if (CgpClient.Singleton.MainServerProvider.StartDHCP(true))
                {
                    DHCPStateChange(DHCPState.Running);
                }
            }
        }

        private void DHCPStateChange(DHCPState state)
        {
            switch (state)
            {
                case DHCPState.Unknown:
                    _bStartDHCP.Visible = false;
                    _bStopDHCP.Visible = false;
                    break;
                case DHCPState.Stopped:
                    _bStartDHCP.Visible = true;
                    _bStopDHCP.Visible = false;
                    break;
                case DHCPState.Running:
                    _bStartDHCP.Visible = false;
                    _bStopDHCP.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void _buttonLicenceRequest_Click(object sender, EventArgs e)
        {
            (new FormLicenceRequest()).ShowDialogWithCheck();
        }

        Contal.Cgp.Server.Beans.TimeZone _actTimeZone;
        Contal.Cgp.Server.Beans.TimeZone _actEventlogTimeZone;
        Contal.Cgp.Server.Beans.TimeZone _actEventlogReportsTimeZone;

        private void LoadDatabaseBackupSettingsFromRegistry()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                if (!_changedTabPages.Contains(DATABASE_BACKUP_SETTINGS) || Dialog.Question(GetString("QuestionReloadDatabaseBackupSettings")))
                {
                    if (_serverGeneralOptions.DatabaseBackupPath != null)
                    {
                        _eDbsBackupPath.Text = _serverGeneralOptions.DatabaseBackupPath;
                    }
                    else
                    {
                        _eDbsBackupPath.Text = string.Empty;
                    }
                    if (_serverGeneralOptions.TimeZoneGuidString != null
                        && _serverGeneralOptions.TimeZoneGuidString != string.Empty)
                    {
                        try
                        {
                            Guid timeZoneGuid = new Guid(_serverGeneralOptions.TimeZoneGuidString);
                            _actTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(timeZoneGuid);
                            if (_actTimeZone != null)
                            {
                                _tbmTimeZone.Text = _actTimeZone.ToString();
                                _tbmTimeZone.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actTimeZone);
                            }
                        }
                        catch
                        {
                            _tbmTimeZone.Text = string.Empty;
                        }
                    }
                    else
                    {
                        _tbmTimeZone.Text = string.Empty;
                    }

                    ResetChanges(DATABASE_BACKUP_SETTINGS);
                }
            }
        }

        private void LoadDatabaseExpirationEventlogSettingsFromRegistry()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                if (!_changedTabPages.Contains(EVENTLOG_EXPIRATION_SETTINGS) || Dialog.Question(GetString("QuestionReloadExpirationEventlogSettings")))
                {
                    if (_serverGeneralOptions.EventlogTimeZoneGuidString != null
                        && _serverGeneralOptions.EventlogTimeZoneGuidString != string.Empty)
                    {
                        try
                        {
                            Guid eventlogTimeZoneGuid = new Guid(_serverGeneralOptions.EventlogTimeZoneGuidString);
                            _actEventlogTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(eventlogTimeZoneGuid);
                            if (_actEventlogTimeZone != null)
                            {
                                _tbmEventlogTimeZone.Text = _actEventlogTimeZone.ToString();
                                _tbmEventlogTimeZone.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actEventlogTimeZone);
                            }
                        }
                        catch
                        {
                            _tbmEventlogTimeZone.Text = string.Empty;
                        }
                    }
                    else
                    {
                        _tbmEventlogTimeZone.Text = string.Empty;
                    }
                    _eCountDaysExpiration.Value = _serverGeneralOptions.EventlogsExpirationDays;
                    _eMaxEventlogRecords.Value = _serverGeneralOptions.EventlogsMaxCountValue;
                    _tbMaxEventlogSlider.Value = _serverGeneralOptions.EventlogsMaxCountExponent;

                    ResetChanges(EVENTLOG_EXPIRATION_SETTINGS);
                }
            }
        }

        private void SaveDatabaseBackupToRegistry()
        {
            if (_isSqlServerRunOnServerMachine != null && (bool)_isSqlServerRunOnServerMachine)
            {
                if (!ControlDatabasePath(_eDbsBackupPath.Text)) return;
                _serverGeneralOptions.DatabaseBackupPath = _eDbsBackupPath.Text;
            }
            else
            {
                _serverGeneralOptions.DatabaseBackupPath = string.Empty;
            }

            if (!ControlTimeZone())
                return;
            _serverGeneralOptions.TimeZoneGuidString = _actTimeZone.IdTimeZone.ToString();

            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                ResetChanges(DATABASE_BACKUP_SETTINGS);
                SafeThread.StartThread(DoSaveDatabaseBackupToRegistry);
            }
        }

        private void DoSaveDatabaseBackupToRegistry()
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryDatabaseBackup(_serverGeneralOptions);
            }
            catch { }
        }

        private void ModifyClick()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listObjects = new List<IModifyObject>();

                IList<IModifyObject> listTz = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listObjects.AddRange(listTz);

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, CgpClient.Singleton.LocalizationHelper.GetString("TimeZonesFormTimeZonesForm"));
                IModifyObject outTimeZone;
                formAdd.ShowDialog(out outTimeZone);
                if (outTimeZone != null)
                {
                    _actTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outTimeZone.GetId);
                    RefreshTimeZone();
                    CgpClientMainForm.Singleton.AddToRecentList(_actTimeZone);
                    EditTextChanger(DATABASE_BACKUP_SETTINGS);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void RefreshTimeZone()
        {
            if (_actTimeZone != null)
            {
                _tbmTimeZone.Text = _actTimeZone.ToString();
                _tbmTimeZone.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actTimeZone);
            }
            else
                _tbmTimeZone.Text = string.Empty;
        }

        private void AddTimeZone(object newTimeZone)
        {
            try
            {
                if (newTimeZone.GetType() == typeof(Contal.Cgp.Server.Beans.TimeZone))
                {
                    Contal.Cgp.Server.Beans.TimeZone timeZone = newTimeZone as Contal.Cgp.Server.Beans.TimeZone;
                    _actTimeZone = timeZone;
                    RefreshTimeZone();
                    CgpClientMainForm.Singleton.AddToRecentList(newTimeZone);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmTimeZone.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmTimeZone_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddTimeZone((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmTimeZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _bSaveDatabaseBackup_Click(object sender, EventArgs e)
        {
            SaveDatabaseBackupToRegistry();
        }

        private void _bForceDBSBackup_Click(object sender, EventArgs e)
        {
            ServerDirectory sd = new ServerDirectory();
            string path = null;

            if (_isSqlServerRunOnServerMachine != null && (bool)_isSqlServerRunOnServerMachine)
            {
                sd.ShowDialog(out path);
                if (!string.IsNullOrEmpty(path))
                {
                    if (path == _eDbsBackupPath.Text ||
                        path == _serverGeneralOptions.DatabaseBackupPath)
                    {
                        Contal.IwQuick.UI.Dialog.Error(LocalizationHelper.GetString("ErrorPathForceBackupInUse"));
                        return;
                    }

                    if (CgpClient.Singleton.MainServerProvider.ExistFileOnServer(path + @"\" + CgpClient.Singleton.MainServerProvider.DatabaseBackupFileName()))
                    {
                        if (!Contal.IwQuick.UI.Dialog.Question(LocalizationHelper.GetString("QuestionOverwriteDatabaseBackup")))
                            return;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!Contal.IwQuick.UI.Dialog.Question(GetString("QuestionUseDefaultDbsBackupPath")))
                    return;
            }

            _bForceDBSBackup.Enabled = false;
            _doProgress = true;
            Contal.IwQuick.Threads.SafeThread<string>.StartThread(ForceDatabaseBackup, path);
            Contal.IwQuick.Threads.SafeThread.StartThread(EndlessProgresForceDatabaseBackup);
        }

        private volatile bool _doProgress;
        private void EndlessProgresForceDatabaseBackup()
        {
            try
            {
                VisibleProggress(true);
                while (_doProgress)
                {
                    ShowDatabaseBackupProgress();
                    System.Threading.Thread.Sleep(1100);
                }
                VisibleProggress(false);
                EnableForceBackupButton(true);
            }
            catch { }
        }

        private void ShowDatabaseBackupProgress()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ShowDatabaseBackupProgress));
            }
            else
            {
                if (_pgForceDbsBackup.Value == 60)
                {
                    _pgForceDbsBackup.Value = 0;
                }
                _pgForceDbsBackup.Value++;
            }
        }

        private void VisibleProggress(bool isVisible)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DBool2Void(VisibleProggress), isVisible);
            }
            else
            {
                _pgForceDbsBackup.Visible = isVisible;
            }
        }

        private void EnableForceBackupButton(bool isEnabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DBool2Void(EnableForceBackupButton), isEnabled);
            }
            else
            {
                _bForceDBSBackup.Enabled = isEnabled;
            }
        }

        private void ForceDatabaseBackup(string backupPath)
        {
            try
            {
                bool success = CgpClient.Singleton.MainServerProvider.ForceDatabaseBackup(backupPath);
                if (success)
                {
                    if (string.IsNullOrEmpty(backupPath))
                    {
                        Contal.IwQuick.UI.Dialog.Info(LocalizationHelper.GetString("InfoForceBackupSuccess"));
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Info(LocalizationHelper.GetString("InfoForceBackupSuccess") +
                            Environment.NewLine + LocalizationHelper.GetString("InfoDBSBackupFile") + backupPath);
                    }
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Error(LocalizationHelper.GetString("ErrorForceBackupFail"));
                }
            }
            catch { }
            finally
            {
                _doProgress = false;
            }
        }

        private bool ControlDatabasePath(string path)
        {
            try
            {
                if (!CgpClient.Singleton.MainServerProvider.DirectoryExitsOnServer(path))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eDbsBackupPath,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorDirectoryNotExits"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    _eDbsBackupPath.Focus();
                    return false;
                }
                //if (!WriteToPathEnabled(path))
                //{
                //    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eDatabaseBackupPath,
                //       CgpClient.Singleton.LocalizationHelper.GetString("ErrorDirectoryWriteNotEnabled"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                //    _eDatabaseBackupPath.Focus();
                //    return false;
                //}
                //if (!AccessPath(path))
                //{
                //    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eDatabaseBackupPath,
                //       CgpClient.Singleton.LocalizationHelper.GetString("ErrorDirectoryWriteNotEnabled"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                //    _eDatabaseBackupPath.Focus();
                //    return false;
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ControlTimeZone()
        {
            if (_actTimeZone == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmTimeZone,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorNotTimeZoneDatabaseBackup"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tbmTimeZone.Focus();
                return false;
            }
            return true;
        }

        private bool WriteToPathEnabled(string path)
        {
            try
            {
                string dummyFile = path + @"\dummy.file";
                if (System.IO.File.Exists(dummyFile))
                {
                    return true;
                }
                System.IO.File.Create(dummyFile);
                if (System.IO.File.Exists(dummyFile))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool AccessPath(string path)
        {
            try
            {
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path);
                System.Security.AccessControl.DirectorySecurity dirSec = dirInfo.GetAccessControl();
                System.Security.AccessControl.AuthorizationRuleCollection ruleCollection = dirSec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                string strRule = string.Empty;
                foreach (System.Security.AccessControl.FileSystemAccessRule rule in ruleCollection)
                {
                    strRule += rule.IdentityReference.Value;
                    strRule += Environment.NewLine;
                }
                Contal.IwQuick.UI.Dialog.Info(strRule);

                foreach (System.Security.AccessControl.FileSystemAccessRule rule in ruleCollection)
                {
                    if (rule.IdentityReference.Value.ToUpper().Contains("SQLSERVER"))
                    {
                        if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.WriteData) > 0)
                            return true;
                    }

                    if (rule.IdentityReference.Value.ToUpper().Contains("AUTHEN"))
                    {
                        if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.WriteData) > 0)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void _buttonSaveSecuritySettings_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                try
                {
                    ResetChanges(SECURITY_SETTINGS);
                    // TODO changing int indexing into named based , as int indexing is not prone to changes in order or amount of items

                    _serverGeneralOptions.UniqueAndNotNull =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_UNIQUE_NOT_NULL_PERSONAL_KEY]].Value;

                    _serverGeneralOptions.RequirePINCardLogin =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE]].Value;

                    _serverGeneralOptions.ChangePassDays =
                        Int32.Parse(_epgSecuritySettings.Items[
                                        _securitySettingsIndexes[SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY]].Value.ToString());

                    _serverGeneralOptions.LockClientApplication =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_LOCK_CLIENT_APPLICATION]].Value;

                    _serverGeneralOptions.CcuConfigurationToServerByPassword =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD]]
                                .Value;

                    _serverGeneralOptions.RequiredSecurePin =
                        (bool)
                            _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_REQUIRED_SECURE_PIN]]
                                .Value;

                    _serverGeneralOptions.DisableCcuPnPAutomaticAssignmnet =
                        !(bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_CCU_PNP_ENABLED]].Value;

                    _serverGeneralOptions.ListOnlyUnassignedCardsInPersonForm =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM]]
                                .Value;

                    _serverGeneralOptions.DelayToSaveAlarmsFromCardReaders =
                        Int32.Parse(_epgSecuritySettings.Items[
                                        _securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]].Value.ToString());

                    _serverGeneralOptions.UniqueAKeyCSRestriction =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_UNIQUE_A_KEY_CR_RESTICTION]].Value;

                    _serverGeneralOptions.CardReadersAllowPINCachingInMenu =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_CARD_READER_ALLOW_PIN_CACHING_IN_MENU]].Value;

                    _serverGeneralOptions.MinimalCodeLength =
                        Int32.Parse(_epgSecuritySettings.Items[
                                        _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Value.ToString());

                    _serverGeneralOptions.MaximalCodeLength =
                        Int32.Parse(_epgSecuritySettings.Items[
                                        _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Value.ToString());

                    _serverGeneralOptions.IsPinConfirmationObligatory =
                        (bool)
                            _epgSecuritySettings.Items[
                                _securitySettingsIndexes[SECURITY_SETTINGS_IS_PIN_CONFIRMATION_OBLIGATORY]].Value;

                    SafeThread.StartThread(SaveSecuritySettings);
                }
                catch (Exception error)
                {
                    Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                }
            }
        }

        private void SaveSecuritySettings()
        {
            try
            {
                if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
                {
                    CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistrySecuritySettings(_serverGeneralOptions);
                }
            }
            catch (Exception error)
            {
                if (error is AccessDeniedException)
                {
                    Dialog.Error(GetString("ErrorEditAccessDenied"));
                }
                else
                {
                    Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                }
            }
        }

        private AlarmPriorityDatabase ApDatabaseFromString(AlarmType alarmType, AlarmPriority priority, ObjectType? closetParentObject, ObjectType? secondClosetParentObject)
        {
            try
            {
                //AlarmType alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), type);
                //AlarmPriority alarmPriority = (AlarmPriority)Enum.Parse(typeof(AlarmPriority), priority);

                AlarmPriorityDatabase newApDbs = new AlarmPriorityDatabase(alarmType, priority, closetParentObject, secondClosetParentObject);
                return newApDbs;
            }
            catch
            {
                return null;
            }
        }

        private void _bCeateGroup_Click(object sender, EventArgs e)
        {
            ClearFields(_gbIPGroup);
            _cbMACMask.Tag = new Dictionary<string, string[]>();
            FillComboBoxMACPrefixes(_cbMACMask);
            _cbIPGroup.Text = "";

        }

        void _cbIPGroup_SelectedValueChanged(object sender, System.EventArgs e)
        {
            ClearFields(_gbIPGroup);
            _cbMACMask.Tag = new Dictionary<string, string[]>();
            FillComboBoxMACPrefixes(_cbMACMask);
            if (_cbIPGroup.Text != "" && _cbIPGroup.Tag != null)
            {
                DHCPGroups dhcpGroups = (DHCPGroups)_cbIPGroup.Tag;
                DHCPGroup currentDhcpGroup = GetCurrentDHCPGroup();
                if (currentDhcpGroup != null)
                {
                    FillDhcpGroupFields(currentDhcpGroup);
                }
            }
            _gbIPGroup.Enabled = _cbIPGroup.SelectedItem != null;

            EditTextChanger(DHCP_SERVER_SETTINGS);
        }

        private void _buttonSetMacGroup_Click(object sender, EventArgs e)
        {
            if (SaveIpRange(true, _buttonSetMacGroup, true))
            {
                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _buttonSetMacGroup,
                    CgpClient.Singleton.LocalizationHelper.GetString("IPRangeSet"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                //Dialog.Info(CgpClient.Singleton.LocalizationHelper.GetString("IPRangeSet"));
            }
        }

        private void SetDragDropColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bDragDropColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bDragDropColorText.BackColor = _colourDialog.Color;
            }
            PreviewDragDrop();
        }

        private void DragDropBackColour(object sender, EventArgs e)
        {
            _colourDialog.Color = _bDragDropColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bDragDropColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewDragDrop();
        }

        private void PreviewDragDrop()
        {
            _eDropDownPreview.ForeColor = _bDragDropColorText.BackColor;
            _eDropDownPreview.BackColor = _bDragDropColorBackground.BackColor;
            if (_eDropDownPreview.Text == string.Empty)
            {
                _eDropDownPreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        private void PreviewReferenceObject()
        {
            _eReferenceObjectsPreview.ForeColor = _bReferenceObjectColorText.BackColor;
            _eReferenceObjectsPreview.BackColor = _bReferenceObjectColorBackground.BackColor;
            if (_eReferenceObjectsPreview.Text == string.Empty)
            {
                _eReferenceObjectsPreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        private void SetReferenceObjectColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bReferenceObjectColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bReferenceObjectColorText.BackColor = _colourDialog.Color;
            }
            PreviewReferenceObject();
        }

        private void SetReferenceObjectColorBackground(object sender, EventArgs e)
        {
            _colourDialog.Color = _bReferenceObjectColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bReferenceObjectColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewReferenceObject();
        }

        private void _bSave_Click(object sender, EventArgs e)
        {
            _serverGeneralOptions.DragDropColorText = _bDragDropColorText.BackColor;
            _serverGeneralOptions.DragDropColorBackground = _bDragDropColorBackground.BackColor;
            _serverGeneralOptions.ReferenceObjectColorText = _bReferenceObjectColorText.BackColor;
            _serverGeneralOptions.ReferenceObjectColorBackground = _bReferenceObjectColorBackground.BackColor;

            _serverGeneralOptions.AlarmNotAcknowledgedColorText = _bAlarmNotAcknowledgedColorText.BackColor;
            _serverGeneralOptions.AlarmNotAcknowledgedColorBackground = _bAlarmNotAcknowledgedColorBackground.BackColor;
            _serverGeneralOptions.AlarmColorText = _bAlarmColorText.BackColor;
            _serverGeneralOptions.AlarmColorBackground = _bAlarmColorBackground.BackColor;
            _serverGeneralOptions.NormalNotAcknowledgedColorText = _bNormalNotAcknowledgedColorText.BackColor;
            _serverGeneralOptions.NormalNotAcknowledgedColorBackground = _bNormalNotAcknowledgedColorBackground.BackColor;
            _serverGeneralOptions.NormalColorText = _bNormalColorText.BackColor;
            _serverGeneralOptions.NormalColorBackground = _bNormalColorBackground.BackColor;
            _serverGeneralOptions.NoAlarmsInQueueColorText = _bNoAlarmsInQueueColorText.BackColor;
            _serverGeneralOptions.NoAlarmsInQueueColorBackground = _bNoAlarmsInQueueColorBackground.BackColor;

            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                ResetChanges(COLOR_SETTINGS);
                SafeThread.StartThread(SaveColourSettings);
            }
        }

        private void SaveColourSettings()
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryColourSettings(_serverGeneralOptions);
            }
            catch { }
        }

        private void _buttonSave_Click(object sender, EventArgs e)
        {
            _serverGeneralOptions.IsTurnedOn = _cbAutocCoseTurnedOn.Checked;
            _serverGeneralOptions.AutoCloseTimeout = (int)_nUpDoAutoCloseTimeout.Value;
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                ResetChanges(AUTO_CLOSE_SETTINGS);
                SafeThread.StartThread(SaveAutoCloseSettings);
            }
        }

        private void SaveAutoCloseSettings()
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryAutoCloseSettings(_serverGeneralOptions);
            }
            catch { }
        }

        public void ReloadColorFromRegistry()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshColor();
        }

        public void ReloadDatabaseBackupSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshDatabaseBackupSettings();
        }

        public void ReloadDatabaseExpirationEventlogSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshDatabaseExpirationEventlogSettings();
        }

        public void ReloadSecuritySettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshSecuritySettings();
        }

        public void ReloadAlarmSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshAlarmSettings();
        }

        public void ReloadEventlogs()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshEventlogs();
        }

        public void ReloadAutoCloseSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshAutoClose();
        }

        public void ReloadRemoteServicesSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshRemoteServicesSettings();
        }

        public void ReloadCustomerAndSupplierInfo()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshCustomerAndSupplierInfo();
        }

        public void ReloadSerialPortSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshSerialPortSettings();
        }

        public void ReloadAdvancedAccessSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshAdvancedAccessSettings();
        }

        public void ReloadAdvancedSettings()
        {
            _serverGeneralOptions = CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
            if (_serverGeneralOptions == null) return;
            RefreshAdvancedSettings();
        }

        private void RefreshDatabaseBackupSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshDatabaseBackupSettings));
            }
            else
            {
                LoadDatabaseBackupSettingsFromRegistry();
            }
        }

        private void RefreshDatabaseExpirationEventlogSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshDatabaseExpirationEventlogSettings));
            }
            else
            {
                LoadDatabaseExpirationEventlogSettingsFromRegistry();
            }
        }

        private void RefreshSecuritySettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshSecuritySettings));
            }
            else
            {
                LoadFromRegSecuritySettings();
                _epgSecuritySettings.Refresh();
            }
        }

        private void RefreshAlarmSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshAlarmSettings));
            }
            else
            {
                ObtainAlarmPrirotities();
            }
        }

        private void RefreshEventlogs()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshEventlogs));
            }
            else
            {
                LoadFromRegEventlogs();
            }
        }

        private void RefreshAutoClose()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshAutoClose));
            }
            else
            {
                LoadFromRegAutocloseSettings();
            }
        }

        private void RefreshAdvancedAccessSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshAdvancedAccessSettings));
            }
            else
            {
                LoadFromRegAdvancedAccessSettings();
            }
        }

        private void RefreshAdvancedSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshAdvancedSettings));
            }
            else
            {
                LoadFromRegAdvancedSettings();
            }
        }

        private void RefreshRemoteServicesSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshRemoteServicesSettings));
            }
            else
            {
                SetServerGeneralOptionsToEditorsSMTP();
            }
        }

        private void RefreshCustomerAndSupplierInfo()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(CustomerAndSupplierInfoToEditors));
            }
            else
            {
                CustomerAndSupplierInfoToEditors();
            }
        }

        private void RefreshSerialPortSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshRemoteServicesSettings));
            }
            else
            {
                SetServerGeneralOptionsToEditorsSerialPort();
            }
        }

        private void RefreshColor()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(RefreshColor));
            }
            else
            {
                LoadColorFromRegistry();
            }
        }

        private void LoadColorFromRegistry()
        {
            if (_serverGeneralOptions == null) return;

            if (!_changedTabPages.Contains(COLOR_SETTINGS) || Dialog.Question(GetString("QuestionReloadColorSettings")))
            {
                _bDragDropColorText.BackColor = _serverGeneralOptions.DragDropColorText;
                _bDragDropColorBackground.BackColor = _serverGeneralOptions.DragDropColorBackground;
                _bReferenceObjectColorText.BackColor = _serverGeneralOptions.ReferenceObjectColorText;
                _bReferenceObjectColorBackground.BackColor = _serverGeneralOptions.ReferenceObjectColorBackground;

                _bAlarmNotAcknowledgedColorText.BackColor = _serverGeneralOptions.AlarmNotAcknowledgedColorText;
                _bAlarmNotAcknowledgedColorBackground.BackColor = _serverGeneralOptions.AlarmNotAcknowledgedColorBackground;
                _bAlarmColorText.BackColor = _serverGeneralOptions.AlarmColorText;
                _bAlarmColorBackground.BackColor = _serverGeneralOptions.AlarmColorBackground;
                _bNormalNotAcknowledgedColorText.BackColor = _serverGeneralOptions.NormalNotAcknowledgedColorText;
                _bNormalNotAcknowledgedColorBackground.BackColor = _serverGeneralOptions.NormalNotAcknowledgedColorBackground;
                _bNormalColorText.BackColor = _serverGeneralOptions.NormalColorText;
                _bNormalColorBackground.BackColor = _serverGeneralOptions.NormalColorBackground;
                _bNoAlarmsInQueueColorText.BackColor = _serverGeneralOptions.NoAlarmsInQueueColorText;
                _bNoAlarmsInQueueColorBackground.BackColor = _serverGeneralOptions.NoAlarmsInQueueColorBackground;

                PreviewDragDrop();
                PreviewReferenceObject();
                PreviewAlarmNotAcknowledgedColor();
                PreviewAlarmColor();
                PreviewNoAlarmsInQueueColor();
                PreviewNormalColor();
                PreviewNormalNotAcknowledgedColor();
                ResetChanges(COLOR_SETTINGS);
            }
        }

        private void LoadFromRegAutocloseSettings()
        {
            if (_serverGeneralOptions == null) return;

            if (!_changedTabPages.Contains(AUTO_CLOSE_SETTINGS) || Dialog.Question(GetString("QuestionReloadAutoCloseSettings")))
            {
                _cbAutocCoseTurnedOn.Checked = _serverGeneralOptions.IsTurnedOn;
                _nUpDoAutoCloseTimeout.Value = _serverGeneralOptions.AutoCloseTimeout;

                ResetChanges(AUTO_CLOSE_SETTINGS);
            }
        }

        private void LoadFromRegSecuritySettings()
        {
            if (_serverGeneralOptions == null) return;
            {
                if (!_changedTabPages.Contains(SECURITY_SETTINGS) || Dialog.Question(GetString("QuestionReloadSecuritySettings")))
                {
                    _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_UNIQUE_NOT_NULL_PERSONAL_KEY]]
                        .Value = _serverGeneralOptions.UniqueAndNotNull;

                    if (_serverGeneralOptions.RequirePINCardLogin != null)
                    {
                        _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE]]
                            .Value = _serverGeneralOptions.RequirePINCardLogin.Value;
                    }
                    else
                    {
                        _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE]]
                            .Value = false;
                    }

                    _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_CARD_PIN_LOGIN_REQUIRE]]
                        .Visible = CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY]].Value
                        = _serverGeneralOptions.ChangePassDays.ToString();

                    _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_LOCK_CLIENT_APPLICATION]]
                        .Value = _serverGeneralOptions.LockClientApplication;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD]].Value =
                        _serverGeneralOptions.CcuConfigurationToServerByPassword;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CCU_CONFIGURATION_TO_SERVER_BY_PASSWORD]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_REQUIRED_SECURE_PIN]].Value =
                        _serverGeneralOptions.RequiredSecurePin;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CCU_PNP_ENABLED]].Value =
                        !_serverGeneralOptions.DisableCcuPnPAutomaticAssignmnet;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CCU_PNP_ENABLED]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_LIST_ONLY_UNASSIGNED_CARDS_IN_PERSON_FORM]].Value =
                        _serverGeneralOptions.ListOnlyUnassignedCardsInPersonForm;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]].Value =
                        _serverGeneralOptions.DelayToSaveAlarmsFromCardReaders.ToString();

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[_securitySettingsIndexes[SECURITY_SETTINGS_UNIQUE_A_KEY_CR_RESTICTION]]
                        .Value = _serverGeneralOptions.UniqueAKeyCSRestriction;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CARD_READER_ALLOW_PIN_CACHING_IN_MENU]].Value =
                        _serverGeneralOptions.CardReadersAllowPINCachingInMenu;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CARD_READER_ALLOW_PIN_CACHING_IN_MENU]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Value =
                        _serverGeneralOptions.MinimalCodeLength.ToString();

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Value =
                        _serverGeneralOptions.MaximalCodeLength.ToString();

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_IS_PIN_CONFIRMATION_OBLIGATORY]].Value =
                        _serverGeneralOptions.IsPinConfirmationObligatory;

                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_IS_PIN_CONFIRMATION_OBLIGATORY]].Visible =
                        CgpClient.Singleton.IsLoadedPlugin("NCAS plugin");

                    ResetChanges(SECURITY_SETTINGS);
                }
            }
        }

        private void LoadFromRegEventlogs()
        {
            if (_serverGeneralOptions == null) return;

            if (!_changedTabPages.Contains(EVENTLOG_SETTINGS) || Dialog.Question(GetString("QuestionReloadEventlogSettings")))
            {
                _cbEventlogInputStateChanged.Checked = _serverGeneralOptions.EventlogInputStateChanged;
                _cbEventlogOutputStateChanged.Checked = _serverGeneralOptions.EventlogOutputStateChanged;
                _cbEventlogAlarmAreaAlarmStateChanged.Checked = _serverGeneralOptions.EventlogAlarmAreaAlarmStateChanged;
                _cbEventlogAlarmAreaActivationStateChanged.Checked = _serverGeneralOptions.EventlogAlarmAreaActivationStateChanged;
                _cbEventlogCardReaderOnlineStateChanged.Checked = _serverGeneralOptions.EventlogCardReaderOnlineStateChanged;
                _cbEventSourcesReverseOrder.Checked = _serverGeneralOptions.EventSourcesReverseOrder;

                // Reports
                if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
                {
                    if (!string.IsNullOrEmpty(_serverGeneralOptions.EventlogReportsTimeZoneGuidString))
                    {
                        try
                        {
                            Guid timeZoneGuid = new Guid(_serverGeneralOptions.EventlogReportsTimeZoneGuidString);
                            _actEventlogReportsTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(timeZoneGuid);
                            if (_actEventlogReportsTimeZone != null)
                            {
                                _tbmEventlogsReportsTimezone.Text = _actEventlogReportsTimeZone.ToString();
                                _tbmEventlogsReportsTimezone.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actEventlogReportsTimeZone);
                            }
                        }
                        catch
                        {
                            _tbmEventlogsReportsTimezone.Text = string.Empty;
                        }
                    }
                    else
                    {
                        _tbmEventlogsReportsTimezone.Text = string.Empty;
                    }
                }
                _eEventlogsReportsEmails.Text = _serverGeneralOptions.EventlogReportsEmails;

                ResetChanges(EVENTLOG_SETTINGS);
            }
        }

        private void SetAlarmNotAcknowledgedColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bAlarmNotAcknowledgedColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bAlarmNotAcknowledgedColorText.BackColor = _colourDialog.Color;
            }
            PreviewAlarmNotAcknowledgedColor();
        }

        private void SetAlarmNotAcknowledgedColorBackground(object sender, EventArgs e)
        {
            _colourDialog.Color = _bAlarmNotAcknowledgedColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bAlarmNotAcknowledgedColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewAlarmNotAcknowledgedColor();
        }

        private void PreviewAlarmNotAcknowledgedColor()
        {
            _eAlarmNotAcknowledgedPreview.ForeColor = _bAlarmNotAcknowledgedColorText.BackColor;
            _eAlarmNotAcknowledgedPreview.BackColor = _bAlarmNotAcknowledgedColorBackground.BackColor;
            if (_eAlarmNotAcknowledgedPreview.Text == string.Empty)
            {
                _eAlarmNotAcknowledgedPreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        private void SetAlarmColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bAlarmColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bAlarmColorText.BackColor = _colourDialog.Color;
            }
            PreviewAlarmColor();
        }

        private void SetAlarmColorBackground(object sender, EventArgs e)
        {
            _colourDialog.Color = _bAlarmColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bAlarmColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewAlarmColor();
        }

        private void PreviewAlarmColor()
        {
            _eAlarmPreview.ForeColor = _bAlarmColorText.BackColor;
            _eAlarmPreview.BackColor = _bAlarmColorBackground.BackColor;
            if (_eAlarmPreview.Text == string.Empty)
            {
                _eAlarmPreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        private void SetNormalNotAcknowledgedColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bNormalNotAcknowledgedColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bNormalNotAcknowledgedColorText.BackColor = _colourDialog.Color;
            }
            PreviewNormalNotAcknowledgedColor();
        }

        private void SetNormalNotAcknowledgedColorBackground(object sender, EventArgs e)
        {
            _colourDialog.Color = _bNormalNotAcknowledgedColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bNormalNotAcknowledgedColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewNormalNotAcknowledgedColor();
        }

        private void PreviewNormalNotAcknowledgedColor()
        {
            _eNormalNotAcknowledgedPreview.ForeColor = _bNormalNotAcknowledgedColorText.BackColor;
            _eNormalNotAcknowledgedPreview.BackColor = _bNormalNotAcknowledgedColorBackground.BackColor;
            if (_eNormalNotAcknowledgedPreview.Text == string.Empty)
            {
                _eNormalNotAcknowledgedPreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        private void SetNormalColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bNormalColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bNormalColorText.BackColor = _colourDialog.Color;
            }
            PreviewNormalColor();
        }

        private void SetNormalColorBackground(object sender, EventArgs e)
        {
            _colourDialog.Color = _bNormalColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bNormalColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewNormalColor();
        }

        private void PreviewNormalColor()
        {
            _eNormalPreview.ForeColor = _bNormalColorText.BackColor;
            _eNormalPreview.BackColor = _bNormalColorBackground.BackColor;
            if (_eNormalPreview.Text == string.Empty)
            {
                _eNormalPreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        private void SetNoAlarmsInQueueColorText(object sender, EventArgs e)
        {
            _colourDialog.Color = _bNoAlarmsInQueueColorText.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bNoAlarmsInQueueColorText.BackColor = _colourDialog.Color;
            }
            PreviewNoAlarmsInQueueColor();
        }

        private void SetNoAlarmsInQueueColorBackground(object sender, EventArgs e)
        {
            _colourDialog.Color = _bNoAlarmsInQueueColorBackground.BackColor;
            DialogResult dr = _colourDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _bNoAlarmsInQueueColorBackground.BackColor = _colourDialog.Color;
            }
            PreviewNoAlarmsInQueueColor();
        }

        private void PreviewNoAlarmsInQueueColor()
        {
            _eNoAlarmQueuePreview.ForeColor = _bNoAlarmsInQueueColorText.BackColor;
            _eNoAlarmQueuePreview.BackColor = _bNoAlarmsInQueueColorBackground.BackColor;
            if (_eNoAlarmQueuePreview.Text == string.Empty)
            {
                _eNoAlarmQueuePreview.Text = LocalizationHelper.GetString("InfoColorPreview");
            }
        }

        public Color GetDragDropTextColor
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                {
                    return _serverGeneralOptions.DragDropColorText;
                }
                else
                    return Color.Black;
            }
        }
        public Color GetDragDropBackgroundColor
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return serverGeneralOptions.DragDropColorBackground;
                else
                    return CgpServerGlobals.DRAG_DROP_COLOR_BACKGROUND;
            }
        }
        public Color GetReferenceTextColor
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.ReferenceObjectColorText;
                else
                    return Color.Black;
            }
        }
        public Color GetReferenceBackgroundColor
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.ReferenceObjectColorBackground;
                else
                    return CgpServerGlobals.REFERENCE_OBJECT_COLOR_BACKGROUND;
            }
        }

        public Color GetAlarmColorText
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AlarmColorText;
                else
                    return Color.Black;
            }
        }
        public Color GetAlarmColorBackground
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AlarmColorBackground;
                else
                    return CgpServerGlobals.ALARM_COLOR_BACKGROUND;
            }
        }
        public Color GetAlarmNotAcknowledgedColorText
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AlarmNotAcknowledgedColorText;
                else
                    return Color.Black;
            }
        }
        public Color GetAlarmNotAcknowledgedColorBackground
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AlarmNotAcknowledgedColorBackground;
                else
                    return CgpServerGlobals.ALARM_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
            }
        }
        public Color GetNormalNotAcknowledgedColorText
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.NormalNotAcknowledgedColorText;
                else
                    return Color.Black;
            }
        }
        public Color GetNormalNotAcknowledgedColorBackground
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.NormalNotAcknowledgedColorBackground;
                else
                    return CgpServerGlobals.NORMAL_NOT_ACKNOWLEDGED_COLOR_BACKGROUND;
            }
        }
        public Color GetNormalColorText
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.NormalColorText;
                else
                    return Color.Black;
            }
        }
        public Color GetNormalColorBackground
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.NormalColorBackground;
                else
                    return CgpServerGlobals.NORMAL_COLOR_BACKGROUND;
            }
        }
        public Color GetNoAlarmsInQueueColorText
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.NoAlarmsInQueueColorText;
                else
                    return Color.Black;
            }
        }
        public Color GetNoAlarmsInQueueColorBackground
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.NoAlarmsInQueueColorBackground;
                else
                    return CgpServerGlobals.NO_ALARMS_IN_QUEUE_COLOR_BACKGROUND;
            }
        }

        public Color GetAlarmStateColorBackground(bool isBlocekd, AlarmState alarmState, bool isAcknowledge)
        {
            if (isBlocekd)
                alarmState = AlarmState.Normal;

            if (alarmState == AlarmState.Alarm)
            {
                if (!isAcknowledge)
                    return GetAlarmNotAcknowledgedColorBackground;
                else
                    return GetAlarmColorBackground;
            }
            else
            {
                if (!isAcknowledge)
                    return GetNormalNotAcknowledgedColorBackground;
                else
                    return GetNormalColorBackground;
            }
        }

        public Color GetAlarmStateColorText(bool isOffline, bool isBlocekd, AlarmState alarmState, bool isAcknowledge)
        {
            if (isBlocekd)
                alarmState = AlarmState.Normal;

            if (alarmState == AlarmState.Alarm)
            {
                if (!isAcknowledge)
                    return GetAlarmNotAcknowledgedColorText;
                else
                    return GetAlarmColorText;
            }
            else
            {
                if (!isAcknowledge)
                    return GetNormalNotAcknowledgedColorText;
                else
                    return GetNormalColorText;
            }
        }

        public bool GetAutoCloseTurnedOn
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.IsTurnedOn;
                else
                    return false;
            }
        }

        public bool GetRequirePINCardLogin
        {
            get
            {
                if (_serverGeneralOptions != null)
                {
                    if (_serverGeneralOptions.RequirePINCardLogin != null)
                    {
                        return (bool)_serverGeneralOptions.RequirePINCardLogin;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (CgpClient.Singleton.MainServerProvider != null)
                    {
                        return CgpClient.Singleton.MainServerProvider.ReturnIsPinCardLoginRequiered();
                    }
                }
                return false;
            }
        }


        public int GetAutoCloseTimeout
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AutoCloseTimeout;
                else
                    return 60;
            }
        }

        public int GetAlarmListSuspendedRefreshTimeout
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AlarmListSuspendedRefreshTimeout;
                else
                    return 60;
            }
        }

        public int GetChangePassDays
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.ChangePassDays;
                else
                    return 0;
            }
        }

        public bool GetUniqueNotNullPersonalKey
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();
                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.UniqueAndNotNull;
                else
                    return false;
            }
        }

        public bool AlarmAreaRestrictivePolicyForTimeBuying
        {
            get
            {
                ServerGeneralOptions serverGeneralOptions = LoadServerGenaralOptions();

                if (serverGeneralOptions != null)
                    return _serverGeneralOptions.AlarmAreaRestrictivePolicyForTimeBuying;

                return true;
            }
        }

        private void _cbIPGroup_Click(object sender, EventArgs e)
        {
            if (_cbIPGroup.Text == string.Empty)
            {
                _cbIPGroup.SelectedIndex = _cbIPGroup.Items.Count - 1;
            }
        }

        private void _cbAutocCoseTurnedOn_CheckStateChanged(object sender, EventArgs e)
        {
            if (_cbAutocCoseTurnedOn.Checked)
            {
                _nUpDoAutoCloseTimeout.Value = (int)60;
            }

            EditTextChanger(AUTO_CLOSE_SETTINGS);
        }

        private void _tbmTimeZone_PopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyClick();
            }
            else if (item.Name == "_tsiRemove")
            {
                _actTimeZone = null;
                RefreshTimeZone();
                EditTextChanger(DATABASE_BACKUP_SETTINGS);
            }
            else if (item.Name == "_tsiCreate")
            {
                Contal.Cgp.Server.Beans.TimeZone timeZone = new Contal.Cgp.Server.Beans.TimeZone();
                TimeZonesForm.Singleton.OpenInsertFromEdit(ref timeZone, TZDoAfterCreatedOrEdited);
            }
        }

        private void TZDoAfterCreatedOrEdited(object timeZone)
        {
            Contal.Cgp.Server.Beans.TimeZone tz = timeZone as Contal.Cgp.Server.Beans.TimeZone;
            if (tz == null)
                return;

            _actTimeZone = tz;
            RefreshTimeZone();
            EditTextChanger(DATABASE_BACKUP_SETTINGS);
        }

        internal void StopRefreshingDemoTime()
        {
            if (_remainingTimerManager != null)
                _remainingTimerManager.StopAll();
        }

        #region NTP settings
        /// <summary>
        /// Add NTP IP address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddIpAddress(object sender, EventArgs e)
        {
            if (CheckIPAddress())
            {
                if (!_lNtpIpAddresses.Items.Contains(_eNtpIpAddress.Text))
                {
                    _lNtpIpAddresses.Items.Add(_eNtpIpAddress.Text);
                    EditTextChanger(REMOTE_SERVICES_SETTINGS);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNtpIpAddress,
                        GetString("ErrorIpAddressAlreadyAdded"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
        }

        /// <summary>
        /// Resolve NTP IP Address from Dns hostname
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResolveDnsClick(object sender, EventArgs e)
        {
            if (CheckHostName())
            {
                Contal.IwQuick.Threads.SafeThread<string>.StartThread(ResolveDnsToIPaddress, _eDnsHostName.Text);
            }
        }

        /// <summary>
        /// Delete MTP IP Address from list NTP Address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _bDelete2_Click(object sender, EventArgs e)
        {
            if (_lNtpIpAddresses.Items.Count > 0 && _lNtpIpAddresses.SelectedItem != null)
            {
                if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteIpAddressConfirm")))
                {
                    _lNtpIpAddresses.Items.Remove(_lNtpIpAddresses.SelectedItem);
                    EditTextChanger(REMOTE_SERVICES_SETTINGS);
                }
            }
        }

        /// <summary>
        /// Valideta IP address for NTP Ip Address edit (_eNtpIpAddress)
        /// </summary>
        /// <returns></returns>
        private bool CheckIPAddress()
        {
            if (_eNtpIpAddress.Text != string.Empty)
            {
                if (IwQuick.Net.IPHelper.IsValid4(_eNtpIpAddress.Text))
                {
                    return true;
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNtpIpAddress,
                        GetString("ErrorNotValidIpAddress"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            else
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNtpIpAddress,
                    GetString("ErrorEntryIpAddress"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
            }

            return false;
        }

        /// <summary>
        /// Validate DNS name in edit _eDnsHostName
        /// </summary>
        /// <returns></returns>
        private bool CheckHostName()
        {

            if (_eDnsHostName.Text != string.Empty)
            {
                if (IwQuick.Net.Hostname.IsValid(_eDnsHostName.Text))
                {
                    return true;
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eDnsHostName,
                        GetString("ErrorNotValidDNSHostname"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            else
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eDnsHostName,
                    GetString("ErrorEntryDNSHostname"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
            }
            return false;
        }


        /// <summary>
        /// Obtain one IP address from DNS name and send it to edit NTP IP Address
        /// </summary>
        /// <param name="dnsName"></param>
        private void ResolveDnsToIPaddress(string dnsName)
        {
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(dnsName);
                if (host.AddressList != null && host.AddressList.Length > 0)
                {
                    IPAddress ipAddress = host.AddressList.ElementAt(0);
                    SetNtpIpAddress(ipAddress.ToString());
                }
            }
            catch (System.Net.Sockets.SocketException error)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDNS"), error.Message);
            }
            catch (ArgumentException aEerror)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDNS"), aEerror.Message);
            }
            catch
            { }
        }

        private void SetNtpIpAddress(string ipAddressString)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DString2Void(SetNtpIpAddress), ipAddressString);
            }
            else
            {
                _eNtpIpAddress.Text = ipAddressString;
            }
        }

        private void _eNtpIpAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '.'))
                e.Handled = true;
        }

        private void LoadNtpSettingsFromDatabase()
        {
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false))
            {
                LoadNtpIpAddress();
                LoadNtpPollingInterval();
            }
        }

        private void LoadNtpPollingInterval()
        {
            int interval = CgpClient.Singleton.MainServerProvider.ServerGeneralOptionsDBs.GetGeneralNtpTimeDiffTolerance();
            try
            {
                _nudTimeDiffTolerance.Value = interval;
            }
            catch { }
        }

        private void LoadNtpIpAddress()
        {
            string ipAddresses = CgpClient.Singleton.MainServerProvider.ServerGeneralOptionsDBs.GetGeneralNtpIpAddress();
            string[] ipAdd = ipAddresses.Split(';');
            _lNtpIpAddresses.Items.Clear();
            if (ipAdd != null && ipAdd.Length > 0)
            {
                foreach (string str in ipAdd)
                {
                    if (str != null && str != String.Empty)
                    {
                        _lNtpIpAddresses.Items.Add(str);
                    }
                }
            }
        }

        private void SaveNtpSettingsToDatabase()
        {
            string ipAddresses = String.Empty;

            if (_lNtpIpAddresses != null && _lNtpIpAddresses.Items.Count != 0)
            {
                foreach (object str in _lNtpIpAddresses.Items)
                {
                    ipAddresses += str.ToString() + ";";
                }
            }
            if (!CgpClient.Singleton.MainServerProvider.ServerGeneralOptionsDBs.SaveNtpSettings(ipAddresses, (int)_nudTimeDiffTolerance.Value))
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSaveNtpIpAddresses"));
            }
        }

        #endregion

        #region EventlogSettings
        private void MaxEventlogRecordsValueChanged(object sender, EventArgs e)
        {
            MaxEventlogInfo();
            EditTextChanger(EVENTLOG_EXPIRATION_SETTINGS);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            MaxEventlogInfo();
            EditTextChanger(EVENTLOG_EXPIRATION_SETTINGS);
        }

        private void MaxEventlogInfo()
        {
            int value = (int)_eMaxEventlogRecords.Value;
            int valueExponent = _tbMaxEventlogSlider.Value + 2;
            long result = value * (long)Math.Pow(10, valueExponent);
            _eResultMaxEventlogRecords.Text = result.ToString("### ### ### ### ###");
        }

        private void _tbmEventlogTimeZone_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifyEventlogTimeZone();
            }
            else if (item.Name == "_tsiRemove1")
            {
                _actEventlogTimeZone = null;
                RefreshEventlogTimeZone();
                EditTextChanger(EVENTLOG_EXPIRATION_SETTINGS);
            }
            else if (item.Name == "_tsiCreate1")
            {
                Contal.Cgp.Server.Beans.TimeZone timeZone = new Contal.Cgp.Server.Beans.TimeZone();
                if (TimeZonesForm.Singleton.OpenInsertDialg(ref timeZone))
                {
                    _actEventlogTimeZone = timeZone;
                    RefreshEventlogTimeZone();
                    EditTextChanger(EVENTLOG_EXPIRATION_SETTINGS);
                }
            }
        }

        private void _bSaveEventlogExpiration_Click(object sender, EventArgs e)
        {
            SaveEventlogExpirationSettingsToRegistry();
        }

        private void _tbmEventlogTimeZone_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddEventLogTimeZone((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmEventlogTimeZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void ModifyEventlogTimeZone()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listObjects = new List<IModifyObject>();

                IList<IModifyObject> listTz = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listObjects.AddRange(listTz);

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, CgpClient.Singleton.LocalizationHelper.GetString("TimeZonesFormTimeZonesForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    _actEventlogTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outModObj.GetId);
                    if (error != null) throw error;
                    RefreshEventlogTimeZone();
                    CgpClientMainForm.Singleton.AddToRecentList(_actEventlogTimeZone);
                    EditTextChanger(EVENTLOG_EXPIRATION_SETTINGS);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void AddEventLogTimeZone(object newTimeZone)
        {
            try
            {
                if (newTimeZone.GetType() == typeof(Contal.Cgp.Server.Beans.TimeZone))
                {
                    Contal.Cgp.Server.Beans.TimeZone timeZone = newTimeZone as Contal.Cgp.Server.Beans.TimeZone;
                    _actEventlogTimeZone = timeZone;
                    RefreshEventlogTimeZone();
                    CgpClientMainForm.Singleton.AddToRecentList(newTimeZone);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmEventlogTimeZone.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void RefreshEventlogTimeZone()
        {
            if (_actEventlogTimeZone != null)
            {
                _tbmEventlogTimeZone.Text = _actEventlogTimeZone.ToString();
                _tbmEventlogTimeZone.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actEventlogTimeZone);
            }

            else
                _tbmEventlogTimeZone.Text = string.Empty;
        }

        private void SaveEventlogExpirationSettingsToRegistry()
        {
            if (!ControlEventlogTimeZone())
                return;

            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                if (_actEventlogTimeZone == null)
                {
                    _serverGeneralOptions.EventlogTimeZoneGuidString = string.Empty;
                }
                else
                {
                    _serverGeneralOptions.EventlogTimeZoneGuidString = _actEventlogTimeZone.IdTimeZone.ToString();
                }
                _serverGeneralOptions.EventlogsExpirationDays = (int)_eCountDaysExpiration.Value;
                _serverGeneralOptions.EventlogsMaxCountValue = (int)_eMaxEventlogRecords.Value;
                _serverGeneralOptions.EventlogsMaxCountExponent = (int)_tbMaxEventlogSlider.Value;
                ResetChanges(EVENTLOG_EXPIRATION_SETTINGS);
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryDatabaseEventlogExpiration(_serverGeneralOptions);
            }
        }

        private void SaveToDatabaseCustomerAndSupplierInfo()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                _serverGeneralOptions.CustomerCompanyName = _epgCustomerInfo.Items[0].Value.ToString();
                _serverGeneralOptions.CustomerDeliveryAddress = _epgCustomerInfo.Items[1].Value.ToString();
                _serverGeneralOptions.CustomerZipCode = _epgCustomerInfo.Items[2].Value.ToString();
                _serverGeneralOptions.CustomerCityState = _epgCustomerInfo.Items[3].Value.ToString();
                _serverGeneralOptions.CustomerCountry = _epgCustomerInfo.Items[4].Value.ToString();
                _serverGeneralOptions.CustomerPhone = _epgCustomerInfo.Items[5].Value.ToString();
                _serverGeneralOptions.CustomerWebsite = _epgCustomerInfo.Items[6].Value.ToString();
                _serverGeneralOptions.CustomerContactPerson = _epgCustomerInfo.Items[7].Value.ToString();

                _serverGeneralOptions.SupplierCompanyName = _epgSupplierInfo.Items[0].Value.ToString();
                _serverGeneralOptions.SupplierDeliveryAddress = _epgSupplierInfo.Items[1].Value.ToString();
                _serverGeneralOptions.SupplierZipCode = _epgSupplierInfo.Items[2].Value.ToString();
                _serverGeneralOptions.SupplierCityState = _epgSupplierInfo.Items[3].Value.ToString();
                _serverGeneralOptions.SupplierCountry = _epgSupplierInfo.Items[4].Value.ToString();
                _serverGeneralOptions.SupplierPhone = _epgSupplierInfo.Items[5].Value.ToString();
                _serverGeneralOptions.SupplierWebsite = _epgSupplierInfo.Items[6].Value.ToString();
                _serverGeneralOptions.SupplierContactPerson = _epgSupplierInfo.Items[7].Value.ToString();

                ResetChanges(CUSTOMER_AND_SUPPLIER_INFO);
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToDatabaseCustomerAndSupplierInfo(_serverGeneralOptions);

                if (_isPhotoModified)
                {
                    SaveSupplierLogo();
                    _isPhotoModified = false;
                }
            }
        }

        private bool ControlEventlogTimeZone()
        {
            if (_actEventlogTimeZone == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmEventlogTimeZone,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorNotTimeZoneEventlogExpiration"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tbmEventlogTimeZone.Focus();
                return false;
            }
            return true;
        }

        private void _tbmEventlogTimeZone_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actEventlogTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actEventlogTimeZone);
        }

        private void _tbmTimeZone_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actTimeZone, TZDoAfterCreatedOrEdited);
        }

        private void _bForceEvenlogExpirationClean_Click(object sender, EventArgs e)
        {
            if (!Contal.IwQuick.UI.Dialog.Question(GetString("RunForceEventlogExpiration"))) return;

            _expirationDays = (int)_eCountDaysExpiration.Value;
            int value = (int)_eMaxEventlogRecords.Value;
            int valueExponent = _tbMaxEventlogSlider.Value + 2;
            _expirationCount = value * (int)Math.Pow(10, valueExponent);
            _bForceEvenlogExpirationClean.Enabled = false;
            Contal.IwQuick.Threads.SafeThread.StartThread(ForceEventlogExpirationCleaning);
        }

        int _expirationDays = 0;
        int _expirationCount = 0;

        private void ForceEventlogExpirationCleaning()
        {
            try
            {
                if (_expirationDays == 0 || _expirationCount == 0) return;
                bool success = CgpClient.Singleton.MainServerProvider.ForceEventlogCleaning(_expirationDays, _expirationCount);
                if (success)
                {
                    Contal.IwQuick.UI.Dialog.Info(LocalizationHelper.GetString("InfoForceEventlogExpirationSuccess"));
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Error(LocalizationHelper.GetString("ErrorForceEventlogExpirationFail"));
                }
            }
            catch { }
            finally
            {
                InfoEndedForceEventlogCleaning();
            }
        }

        private void InfoEndedForceEventlogCleaning()
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Contal.IwQuick.DVoid2Void(InfoEndedForceEventlogCleaning));
                }
                else
                {
                    _bForceEvenlogExpirationClean.Enabled = true;
                }
            }
            catch { }
        }


        #endregion

        private void _bDBTest_Click(object sender, EventArgs e)
        {
            if (_bDBTest.Text == "Run Test")
            {
                CgpClient.Singleton.MainServerProvider.RunDBTest();
                _bDBTest.Text = "Stop Test";
            }
            else
            {
                CgpClient.Singleton.MainServerProvider.StopDBTest();
                _bDBTest.Text = "Run Test";
            }
        }

        private void GeneralOptionsForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            _bDBTest.Visible = true;
#endif
        }

        public bool RequiredSecurePin
        {
            get { return _serverGeneralOptions.RequiredSecurePin; }
        }

        public bool UniqueAKeyCSRestriction
        {
            get { return _serverGeneralOptions.UniqueAKeyCSRestriction; }
        }

        public bool EventSourcesReverseOrder
        {
            get { return _serverGeneralOptions.EventSourcesReverseOrder; }
        }

        public bool IsSetCcuConfigurationPassword
        {
            get { return _serverGeneralOptions.CcuConfigurationToServerByPassword; }
        }

        private IList<AlarmPriorityDatabase> _listAPD = null;
        private void ObtainAlarmPrirotities()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (!_changedTabPages.Contains(ALARM_SETTINGS) || Dialog.Question(GetString("QuestionReloadAlarmSettings")))
            {
                _listAPD = CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.GetAlarmTypesFromDatabase(CgpClient.Singleton.GetAlarmTypesWithPlugin());
                FillAlarmPriorities(_listAPD);
                ResetChanges(ALARM_SETTINGS);
            }
        }

        BindingSource _bindSource;
        private void FillAlarmPriorities(IList<AlarmPriorityDatabase> listAPD)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IList<AlarmPriorityDatabase>>(FillAlarmPriorities), listAPD);
            }
            else
            {
                if (listAPD == null) return;

                try
                {
                    ICollection<DataGridAlarmType> listDgAlarmType = new List<DataGridAlarmType>();
                    CreateListAlarmPriorities();
                    CreateListsClosestParentObject();

                    foreach (AlarmPriorityDatabase apd in listAPD)
                    {
                        string localizeName = CgpClient.Singleton.GetLocalizedAlarmRuiPrefix(apd.AlarmType);
                        if (string.IsNullOrEmpty(localizeName) || localizeName.Contains("NO_TRANSLATION"))
                            localizeName = apd.AlarmType.ToString();

                        DataGridAlarmType dgAlarmType = new DataGridAlarmType(apd.AlarmType, localizeName,
                            GetFromListAlarmPriorities(apd.AlarmPriority), GetFromListClosestParentObject(apd.ClosestParentObject),
                            GetFromListSecondClosestParentObject(apd.SecondClosestParentObject));

                        if (dgAlarmType != null)
                        {
                            listDgAlarmType.Add(dgAlarmType);
                        }
                    }

                    _cdgvAlarmSettings.DataGrid.AutoGenerateColumns = false;
                    _bindSource = new BindingSource();
                    _bindSource.DataSource = listDgAlarmType;
                    _cdgvAlarmSettings.DataGrid.DataSource = _bindSource;

                    if (!_cdgvAlarmSettings.DataGrid.Columns.Contains("AlarmTypeName"))
                    {
                        DataGridViewTextBoxColumn columnTypeName = new DataGridViewTextBoxColumn();
                        columnTypeName.Name = "AlarmTypeName";
                        //columnTypeName.HeaderText = GetString("ColumnAlarmTypeName");
                        columnTypeName.DataPropertyName = "AlarmTypeName";
                        columnTypeName.ReadOnly = true;
                        columnTypeName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        _cdgvAlarmSettings.DataGrid.Columns.Add(columnTypeName);
                    }

                    if (!_cdgvAlarmSettings.DataGrid.Columns.Contains("AlarmPriorityName"))
                    {
                        DataGridViewComboBoxColumn columnAlarmType = new DataGridViewComboBoxColumn();
                        columnAlarmType.DataSource = _listAlarmPriorities;
                        columnAlarmType.Name = "AlarmPriorityName";
                        //columnAlarmType.HeaderText = GetString("ColumnAlarmPriorityName");
                        columnAlarmType.DataPropertyName = "AlarmPriorityName";
                        columnAlarmType.DisplayMember = "LocalizeAlarmPriority";
                        columnAlarmType.ValueMember = "Self";
                        _cdgvAlarmSettings.DataGrid.Columns.Add(columnAlarmType);
                    }

                    if (!_cdgvAlarmSettings.DataGrid.Columns.Contains("ClosestParentObjectName"))
                    {
                        DataGridViewComboBoxColumn columnAlarmType = new DataGridViewComboBoxColumn();
                        columnAlarmType.DataSource = _listClosestParentObject;
                        columnAlarmType.Name = "ClosestParentObjectName";
                        //columnAlarmType.HeaderText = GetString("ColumnAlarmPriorityName");
                        columnAlarmType.DataPropertyName = "ClosestParentObjectName";
                        columnAlarmType.DisplayMember = "Name";
                        columnAlarmType.ValueMember = "Self";
                        _cdgvAlarmSettings.DataGrid.Columns.Add(columnAlarmType);
                    }

                    if (!_cdgvAlarmSettings.DataGrid.Columns.Contains("SecondClosestParentObjectName"))
                    {
                        DataGridViewComboBoxColumn columnAlarmType = new DataGridViewComboBoxColumn();
                        columnAlarmType.DataSource = _listSecondClosestParentObject;
                        columnAlarmType.Name = "SecondClosestParentObjectName";
                        //columnAlarmType.HeaderText = GetString("ColumnAlarmPriorityName");
                        columnAlarmType.DataPropertyName = "SecondClosestParentObjectName";
                        columnAlarmType.DisplayMember = "Name";
                        columnAlarmType.ValueMember = "Self";
                        _cdgvAlarmSettings.DataGrid.Columns.Add(columnAlarmType);
                    }

                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvAlarmSettings.DataGrid);

                }
                catch { }
            }
        }

        void LocalizationHelper_LanguageChanged()
        {
            LocalizeListAlarmPriorities();
            LocalizeDataGridAlarmTypes();
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvAlarmSettings.DataGrid);
        }

        private void LocalizeListAlarmPriorities()
        {
            try
            {
                foreach (CbAlarmPriority alarmPriority in _listAlarmPriorities)
                {
                    alarmPriority.LocalizeAlarmPriority = GetString("AlarmPriority_" + alarmPriority.AlarmPriority.ToString());
                }
            }
            catch { }
        }

        private void LocalizeDataGridAlarmTypes()
        {
            try
            {
                foreach (DataGridViewRow row in _cdgvAlarmSettings.DataGrid.Rows)
                {
                    DataGridAlarmType dgAlarmType = (DataGridAlarmType)((BindingSource)_cdgvAlarmSettings.DataGrid.DataSource).List[row.Index];

                    if (dgAlarmType != null)
                    {
                        string localizeName = CgpClient.Singleton.GetLocalizedAlarmRuiPrefix(dgAlarmType.AlarmType);
                        if (string.IsNullOrEmpty(localizeName) || localizeName.Contains("NO_TRANSLATION"))
                            localizeName = dgAlarmType.AlarmType.ToString();
                        dgAlarmType.AlarmTypeName = localizeName;
                    }
                }
                _cdgvAlarmSettings.DataGrid.Refresh();
            }
            catch { }
        }

        private void SaveAlarmPriority()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;

                IList<AlarmPriorityDatabase> alarmPriorities = new List<AlarmPriorityDatabase>();

                foreach (DataGridViewRow dgRow in _cdgvAlarmSettings.DataGrid.Rows)
                {
                    DataGridAlarmType dgAt = (DataGridAlarmType)((BindingSource)_cdgvAlarmSettings.DataGrid.DataSource).List[dgRow.Index];

                    if (dgAt != null)
                    {
                        AlarmPriorityDatabase insertAPD = ApDatabaseFromString(dgAt.AlarmType, dgAt.AlarmPriority, dgAt.ClosestParentObject, dgAt.SecondClosestParentObject);
                        if (insertAPD != null)
                            alarmPriorities.Add(insertAPD);
                    }
                }

                ResetChanges(ALARM_SETTINGS);
                CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.SaveAlarmPrioritiesToDatabase(alarmPriorities);
                _listAPD = CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.GetAlarmTypesFromDatabase(CgpClient.Singleton.GetAlarmTypesWithPlugin());
            }
            catch { }
        }

        IList<CbAlarmPriority> _listAlarmPriorities = new List<CbAlarmPriority>();
        private IList<CbAlarmPriority> CreateListAlarmPriorities()
        {
            _listAlarmPriorities.Clear();
            foreach (AlarmPriority alarmPriority in Enum.GetValues(typeof(AlarmPriority)))
            {
                _listAlarmPriorities.Add(new CbAlarmPriority(alarmPriority, GetString("AlarmPriority_" + alarmPriority.ToString())));
            }
            return _listAlarmPriorities;
        }

        private CbAlarmPriority GetFromListAlarmPriorities(AlarmPriority alarmPriority)
        {
            foreach (CbAlarmPriority ap in _listAlarmPriorities)
            {
                if (ap.AlarmPriority == alarmPriority)
                    return ap;
            }
            CbAlarmPriority dap = null;
            foreach (CbAlarmPriority ap in _listAlarmPriorities)
            {
                if (ap.AlarmPriority == AlarmPriority.Medium)
                    dap = ap;
            }
            return dap;
        }

        IList<CbClosestParentObject> _listClosestParentObject = new List<CbClosestParentObject>();
        IList<CbClosestParentObject> _listSecondClosestParentObject = new List<CbClosestParentObject>();
        CbClosestParentObject _nullSecondClosestObject = null;
        private void CreateListsClosestParentObject()
        {
            _listClosestParentObject.Clear();
            _listSecondClosestParentObject.Clear();

            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.AllObjectTypes, GetString("TextAll")));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.NotSupport, GetString("TextNone")));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.Person, GetString("ObjectType_" + ObjectType.Person.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.Login, GetString("ObjectType_" + ObjectType.Login.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.Card, GetString("ObjectType_" + ObjectType.Card.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.CardSystem, GetString("ObjectType_" + ObjectType.CardSystem.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.CisNG, GetString("ObjectType_" + ObjectType.CisNG.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.CisNGGroup, GetString("ObjectType_" + ObjectType.CisNGGroup.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.CardReader, GetString("ObjectType_" + ObjectType.CardReader.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.CCU, GetString("ObjectType_" + ObjectType.CCU.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.DCU, GetString("ObjectType_" + ObjectType.DCU.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.Input, GetString("ObjectType_" + ObjectType.Input.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.Output, GetString("ObjectType_" + ObjectType.Output.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.DoorEnvironment, GetString("ObjectType_" + ObjectType.DoorEnvironment.ToString())));
            _listClosestParentObject.Add(new CbClosestParentObject(ObjectType.AlarmArea, GetString("ObjectType_" + ObjectType.AlarmArea.ToString())));

            _nullSecondClosestObject = new CbClosestParentObject(ObjectType.NotSupport, GetString("TextNone"));
            _listSecondClosestParentObject.Add(_nullSecondClosestObject);
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.Person, GetString("ObjectType_" + ObjectType.Person.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.Login, GetString("ObjectType_" + ObjectType.Login.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.Card, GetString("ObjectType_" + ObjectType.Card.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.CardSystem, GetString("ObjectType_" + ObjectType.CardSystem.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.CisNG, GetString("ObjectType_" + ObjectType.CisNG.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.CisNGGroup, GetString("ObjectType_" + ObjectType.CisNGGroup.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.CardReader, GetString("ObjectType_" + ObjectType.CardReader.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.CCU, GetString("ObjectType_" + ObjectType.CCU.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.DCU, GetString("ObjectType_" + ObjectType.DCU.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.Input, GetString("ObjectType_" + ObjectType.Input.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.Output, GetString("ObjectType_" + ObjectType.Output.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.DoorEnvironment, GetString("ObjectType_" + ObjectType.DoorEnvironment.ToString())));
            _listSecondClosestParentObject.Add(new CbClosestParentObject(ObjectType.AlarmArea, GetString("ObjectType_" + ObjectType.AlarmArea.ToString())));
        }

        private CbClosestParentObject GetFromListClosestParentObject(ObjectType? objectType)
        {
            foreach (CbClosestParentObject closestParentObject in _listClosestParentObject)
            {
                if (closestParentObject.ObjectType == objectType)
                    return closestParentObject;
            }

            return null;
        }

        private CbClosestParentObject GetFromListSecondClosestParentObject(ObjectType? objectType)
        {
            foreach (CbClosestParentObject closestParentObject in _listSecondClosestParentObject)
            {
                if (closestParentObject.ObjectType == objectType)
                    return closestParentObject;
            }

            return null;
        }

        private void _bEventlogsSave_Click(object sender, EventArgs e)
        {
            _serverGeneralOptions.EventlogInputStateChanged = _cbEventlogInputStateChanged.Checked;
            _serverGeneralOptions.EventlogOutputStateChanged = _cbEventlogOutputStateChanged.Checked;
            _serverGeneralOptions.EventlogAlarmAreaAlarmStateChanged = _cbEventlogAlarmAreaAlarmStateChanged.Checked;
            _serverGeneralOptions.EventlogAlarmAreaActivationStateChanged = _cbEventlogAlarmAreaActivationStateChanged.Checked;
            _serverGeneralOptions.EventlogCardReaderOnlineStateChanged = _cbEventlogCardReaderOnlineStateChanged.Checked;
            _serverGeneralOptions.EventSourcesReverseOrder = _cbEventSourcesReverseOrder.Checked;
            if (!CgpClient.Singleton.MainServerProvider.CheckTimetecLicense())
            {
                _serverGeneralOptions.EventlogReportsTimeZoneGuidString = null;
                _serverGeneralOptions.EventlogReportsEmails = null;
            }
            else
            {
                if (!ControlReportsTimeZone())
                    return;
                _serverGeneralOptions.EventlogReportsTimeZoneGuidString = _actEventlogReportsTimeZone.IdTimeZone.ToString();
                _serverGeneralOptions.EventlogReportsEmails = _eEventlogsReportsEmails.Text;
            }

            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                ResetChanges(EVENTLOG_SETTINGS);
                SafeThread.StartThread(SaveEventlogs);
            }
        }

        private void SaveEventlogs()
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryEventlogs(_serverGeneralOptions);
            }
            catch { }
        }

        private void ModifyReportsClick()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listObjects = new List<IModifyObject>();

                IList<IModifyObject> listTz = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listObjects.AddRange(listTz);

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, CgpClient.Singleton.LocalizationHelper.GetString("TimeZonesFormTimeZonesForm"));
                IModifyObject outTimeZone;
                formAdd.ShowDialog(out outTimeZone);
                if (outTimeZone != null)
                {
                    _actEventlogReportsTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outTimeZone.GetId);
                    RefreshReportsTimeZone();
                    CgpClientMainForm.Singleton.AddToRecentList(_actEventlogReportsTimeZone);
                    EditTextChanger(EVENTLOG_SETTINGS);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void RefreshReportsTimeZone()
        {
            if (_actEventlogReportsTimeZone != null)
            {
                _tbmEventlogsReportsTimezone.Text = _actEventlogReportsTimeZone.ToString();
                _tbmEventlogsReportsTimezone.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actEventlogReportsTimeZone);
            }
            else
                _tbmEventlogsReportsTimezone.Text = string.Empty;
        }

        private void AddReportsTimeZone(object newTimeZone)
        {
            try
            {
                if (newTimeZone.GetType() == typeof(Contal.Cgp.Server.Beans.TimeZone))
                {
                    Contal.Cgp.Server.Beans.TimeZone timeZone = newTimeZone as Contal.Cgp.Server.Beans.TimeZone;
                    _actEventlogReportsTimeZone = timeZone;
                    RefreshReportsTimeZone();
                    CgpClientMainForm.Singleton.AddToRecentList(newTimeZone);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmEventlogsReportsTimezone.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmEventlogsReportsTimezone_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddReportsTimeZone((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmEventlogsReportsTimezone_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private bool ControlReportsTimeZone()
        {
            if (_actEventlogReportsTimeZone == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmEventlogsReportsTimezone,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorNotTimeZoneReportsSet"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tbmEventlogsReportsTimezone.Focus();
                return false;
            }

            return true;
        }

        private void _tbmEventlogsReportsTimezone_PopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifyReportsClick();
            }
            else if (item.Name == "_tsiRemove2")
            {
                _actEventlogReportsTimeZone = null;
                RefreshReportsTimeZone();
                EditTextChanger(EVENTLOG_SETTINGS);
            }
            else if (item.Name == "_tsiCreate2")
            {
                Contal.Cgp.Server.Beans.TimeZone timeZone = new Contal.Cgp.Server.Beans.TimeZone();
                TimeZonesForm.Singleton.OpenInsertFromEdit(ref timeZone, TZReportsDoAfterCreatedOrEdited);
            }
        }

        private void _tbmEventlogsReportsTimezone_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actEventlogReportsTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actEventlogReportsTimeZone, TZReportsDoAfterCreatedOrEdited);
        }

        private void TZReportsDoAfterCreatedOrEdited(object timeZone)
        {
            Contal.Cgp.Server.Beans.TimeZone tz = timeZone as Contal.Cgp.Server.Beans.TimeZone;
            if (tz == null)
                return;

            _actEventlogReportsTimeZone = tz;
            RefreshReportsTimeZone();
            EditTextChanger(EVENTLOG_SETTINGS);
        }

        private void _bDbsBackupPath_Click(object sender, EventArgs e)
        {
            ServerDirectory sd = new ServerDirectory();
            string path;
            sd.ShowDialog(out path);
            if (!string.IsNullOrEmpty(path))
            {
                _eDbsBackupPath.Text = path;
            }
        }

        private void _bClearAndSaveDbsBackup_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                _eDbsBackupPath.Text = string.Empty;
                _actTimeZone = null;
                _tbmTimeZone.Text = string.Empty;
                _serverGeneralOptions.TimeZoneGuidString = string.Empty;
                _serverGeneralOptions.DatabaseBackupPath = string.Empty;
                ResetChanges(DATABASE_BACKUP_SETTINGS);
                SafeThread.StartThread(SaveDatabaseBackup);
            }
        }

        private void SaveDatabaseBackup()
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryDatabaseBackup(_serverGeneralOptions);
            }
            catch { }
        }

        private void SetSqlServerRunOnAnotherMachine()
        {
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.MainServerProvider.IsSessionValid)
                return;
            _isSqlServerRunOnServerMachine = CgpClient.Singleton.MainServerProvider.ServerGeneralOptionsDBs.IsSqlServerRunningOnServerMachine();
            UpdateInfoSqlServerAnotherMachine();
        }

        private void _tpDatabaseOptions_Enter(object sender, EventArgs e)
        {
            try
            {
                if (_isSqlServerRunOnServerMachine == null)
                {
                    Contal.IwQuick.Threads.SafeThread.StartThread(SetSqlServerRunOnAnotherMachine);
                }
                else
                {
                    UpdateInfoSqlServerAnotherMachine();
                }
            }
            catch { }
        }

        private void UpdateInfoSqlServerAnotherMachine()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(UpdateInfoSqlServerAnotherMachine));
            }
            else
            {
                if ((bool)_isSqlServerRunOnServerMachine)
                {
                    _gbBackupPath.Enabled = true;
                    if (_serverGeneralOptions != null && _serverGeneralOptions.DatabaseBackupPath != null)
                        _eDbsBackupPath.Text = _serverGeneralOptions.DatabaseBackupPath;
                    else
                        _eDbsBackupPath.Text = string.Empty;
                }
                else
                {
                    if (_gbBackupPath.Enabled)
                    {
                        _gbBackupPath.Enabled = false;
                    }
                    _eDbsBackupPath.Text = GetString("InfoUseDbsDefaultBackupPath");
                }

                ResetChanges(DATABASE_BACKUP_SETTINGS);
            }
        }

        private void _bSave2_Click(object sender, EventArgs e)
        {
            SaveActualLanguage();
        }

        private void SaveActualLanguage()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            string lng = string.Empty;

            System.Windows.Forms.Control.ControlCollection contColl = _tpLanguage.Controls;
            foreach (System.Windows.Forms.Control cnt in contColl)
            {
                if (cnt.GetType() == typeof(RadioButton)) // && cnt.Name != string.Empty)
                {
                    RadioButton rb = (RadioButton)cnt;
                    if (rb.Checked)
                        lng = rb.Text;
                }
            }

            if (lng == string.Empty)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("GeneralOptionsFormErrorLngSelect"));
                return;
            }

            ResetChanges(LANGUAGE_SETTINGS);
            CgpClient.Singleton.MainServerProvider.SetServerLanguage(lng);
        }

        private bool _tpLanguageFirstTimeEnter = true;

        private void _tpLanguage_Enter(object sender, EventArgs e)
        {
            if (_tpLanguageFirstTimeEnter)
            {
                _tpLanguageFirstTimeEnter = false;
                ShowLanguagesForSettings();
            }
        }

        private void LoadFromRegAdvancedAccessSettings()
        {
            if (_serverGeneralOptions != null)
            {
                if (!_changedTabPages.Contains(ADVANCED_ACCESS_SETTINGS) || Dialog.Question(GetString("QuestionReloadAdvancedAccessSettings")))
                {
                    _chbEnableLoggingSDPSTZChanges.Checked = _serverGeneralOptions.EnableLoggingSDPSTZChanges;
                    _cbSyncingTimeFromServer.Checked = _serverGeneralOptions.SyncingTimeFromServer;
                    _ePeriodOfTimeSyncWithoutStratum.Value = _serverGeneralOptions.PeriodOfTimeSyncWithoutStratum;
                    _ePeriodicTimeSyncTolerance.Value = _serverGeneralOptions.PeriodicTimeSyncTolerance;

                    _chbAlarmAreaRestrictivePolicyForTimeBuying.Checked =
                        _serverGeneralOptions.AlarmAreaRestrictivePolicyForTimeBuying;

                    ResetChanges(ADVANCED_ACCESS_SETTINGS);
                }
            }
        }

        private void _bSave3_Click(object sender, EventArgs e)
        {
            _serverGeneralOptions.EnableLoggingSDPSTZChanges = _chbEnableLoggingSDPSTZChanges.Checked;
            _serverGeneralOptions.SyncingTimeFromServer = _cbSyncingTimeFromServer.Checked;
            _serverGeneralOptions.PeriodOfTimeSyncWithoutStratum = (int)_ePeriodOfTimeSyncWithoutStratum.Value;
            _serverGeneralOptions.PeriodicTimeSyncTolerance = (int)_ePeriodicTimeSyncTolerance.Value;

            _serverGeneralOptions.AlarmAreaRestrictivePolicyForTimeBuying =
                _chbAlarmAreaRestrictivePolicyForTimeBuying.Checked;

            SafeThread.StartThread(RunSaveToRegistryAdvancedAccessSettings);
        }

        private void RunSaveToRegistryAdvancedAccessSettings()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                ResetChanges(ADVANCED_ACCESS_SETTINGS);
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryAdvancedAccessSettings(_serverGeneralOptions);
            }
        }

        private void LoadFromRegAdvancedSettings()
        {
            if (_serverGeneralOptions != null)
            {
                if (!_changedTabPages.Contains(ADVANCED_SETTINGS) || Dialog.Question(GetString("QuestionReloadAdvancedSettings")))
                {
                    _eMaxEventsCountForInsert.Value = _serverGeneralOptions.MaxEventsCountForInsert;
                    _eDelayForSaveEvents.Value = _serverGeneralOptions.DelayForSaveEvents;
                    _eClientSessionTimeout.Value = _serverGeneralOptions.ClientSessionTimeOut;
                    _eAlarmListSuspendedRefreshTimeout.Value = _serverGeneralOptions.AlarmListSuspendedRefreshTimeout;
                    _eDelayForSendingChangesToCcu.Value = _serverGeneralOptions.DelayForSendingChangesToCcu;
                    _cbCorrectDeserializationFailures.Checked = _serverGeneralOptions.CorrectDeserializationFailures;

                    ResetChanges(ADVANCED_SETTINGS);
                }
            }
        }

        private void _bSave4_Click(object sender, EventArgs e)
        {
            _serverGeneralOptions.MaxEventsCountForInsert = (int)_eMaxEventsCountForInsert.Value;
            _serverGeneralOptions.DelayForSaveEvents = (int)_eDelayForSaveEvents.Value;
            _serverGeneralOptions.ClientSessionTimeOut = (int)_eClientSessionTimeout.Value;
            _serverGeneralOptions.AlarmListSuspendedRefreshTimeout = (int)_eAlarmListSuspendedRefreshTimeout.Value;
            _serverGeneralOptions.DelayForSendingChangesToCcu = (int)_eDelayForSendingChangesToCcu.Value;
            _serverGeneralOptions.CorrectDeserializationFailures = _cbCorrectDeserializationFailures.Checked;

            SafeThread.StartThread(RunSaveToRegistryAdvancedSettings);
        }

        private void RunSaveToRegistryAdvancedSettings()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                ResetChanges(ADVANCED_SETTINGS);
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveToRegistryAdvancedSettings(_serverGeneralOptions);
            }
        }

        private void _dgAlarmSettings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0 && e.RowIndex > 0)
            {
                DataGridAlarmType dgAlarmType = (DataGridAlarmType)((BindingSource)_cdgvAlarmSettings.DataGrid.DataSource).List[e.RowIndex];
                if (dgAlarmType != null &&
                    (dgAlarmType.ClosestParentObject == ObjectType.NotSupport || dgAlarmType.ClosestParentObject == ObjectType.AllObjectTypes) &&
                    dgAlarmType.SecondClosestParentObject != ObjectType.NotSupport ||
                    (dgAlarmType.ClosestParentObject == dgAlarmType.SecondClosestParentObject))
                {
                    dgAlarmType.SecondClosestParentObjectName = _nullSecondClosestObject;
                }
            }

            EditTextChangerAlarmSettings(null, null);
        }

        private void _dgAlarmSettings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridAlarmType dgAlarmType = (DataGridAlarmType)((BindingSource)_cdgvAlarmSettings.DataGrid.DataSource).List[e.RowIndex];
            if (_cdgvAlarmSettings.DataGrid.Columns.Contains("SecondClosestParentObjectName"))
            {
                DataGridViewComboBoxCell secondClosestParentObjectCell = _cdgvAlarmSettings.DataGrid[_cdgvAlarmSettings.DataGrid.Columns["SecondClosestParentObjectName"].Index, e.RowIndex] as DataGridViewComboBoxCell;
                if (secondClosestParentObjectCell != null)
                {
                    if (dgAlarmType != null && (dgAlarmType.ClosestParentObject == ObjectType.NotSupport || dgAlarmType.ClosestParentObject == ObjectType.AllObjectTypes))
                    {
                        secondClosestParentObjectCell.ReadOnly = true;
                    }
                    else
                    {
                        secondClosestParentObjectCell.ReadOnly = false;
                        List<CbClosestParentObject> actListSecondClosestParentObject = new List<CbClosestParentObject>();
                        foreach (CbClosestParentObject closestParentObject in _listSecondClosestParentObject)
                        {
                            if (closestParentObject != null && closestParentObject.ObjectType != dgAlarmType.ClosestParentObject)
                                actListSecondClosestParentObject.Add(closestParentObject);
                        }

                        secondClosestParentObjectCell.DataSource = actListSecondClosestParentObject;
                    }
                }
            }
        }

        private void _bSave5_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.IsSessionValid)
            {
                SaveAlarmPriority();
            }
        }

        private void _tbmLogin_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify3")
            {
                ModifyLogin();
            }
            else if (item.Name == "_tsiRemove3")
            {
                _actLogin = null;
                RefreshLogin();
            }
        }

        /// <summary>
        /// Show actual login name in the image textbox
        /// </summary>
        private Login _actLogin = null;
        private void RefreshLogin()
        {
            if (_actLogin != null)
            {
                _tbmLogin.Text = _actLogin.ToString();
                _tbmLogin.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLogin);
            }
            else
            {
                _tbmLogin.Text = string.Empty;
            }
        }

        /// <summary>
        /// Select a login
        /// </summary>
        private void ModifyLogin()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listObjects = new List<IModifyObject>();

                IList<IModifyObject> listLogin = CgpClient.Singleton.MainServerProvider.Logins.ListModifyObjects(out error);
                if (error != null) throw error;
                listObjects.AddRange(listLogin);

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, CgpClient.Singleton.LocalizationHelper.GetString("LoginsFormLoginsForm"));
                IModifyObject outLogin;
                formAdd.ShowDialog(out outLogin);
                if (outLogin != null)
                {
                    _actLogin = CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(outLogin.GetId);
                    RefreshLogin();
                    CgpClientMainForm.Singleton.AddToRecentList(_actLogin);
                }
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private void _tbmLogin_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmLogin_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddLogin((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Set actual login
        /// </summary>
        /// <param name="newLogin"></param>
        private void AddLogin(object newLogin)
        {
            try
            {
                if (newLogin != null && newLogin is Login)
                {
                    Login login = newLogin as Login;
                    _actLogin = login;
                    RefreshLogin();
                    CgpClientMainForm.Singleton.AddToRecentList(newLogin);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmLogin.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmLogin_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actLogin != null)
                LoginsForm.Singleton.OpenEditForm(_actLogin);
        }

        private void _bResetUserSession_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (_actLogin != null)
            {
                if (Dialog.Question(GetString("QuestionResetUserSession") + " " + _actLogin.Username))
                    SafeThread<string>.StartThread(ResetUserSession, _actLogin.Username);
            }
            else
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmLogin.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorSelectLogin"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
            }
        }

        /// <summary>
        /// Call reset user session on the Nova server
        /// </summary>
        /// <param name="UserName"></param>
        private void ResetUserSession(string UserName)
        {
            if (CgpClient.Singleton.MainServerProvider.ResetUserSession(new LoginAuthenticationParameters(UserName, string.Empty, true)))
                ShowResetUserSessionResult(true);
            else
                ShowResetUserSessionResult(false);
        }

        /// <summary>
        /// Show result for reset user session
        /// </summary>
        /// <param name="succeeded"></param>
        private void ShowResetUserSessionResult(bool succeeded)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(ShowResetUserSessionResult), succeeded);
            }
            else
            {
                if (succeeded)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Info(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmLogin.ImageTextBox,
                        CgpClient.Singleton.LocalizationHelper.GetString("InfoResetUserSessionSucceeded"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmLogin.ImageTextBox,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorResetUserSessionFailed"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(ShowServerInformations);
        }

        /// <summary>
        /// Save changes for all changed settings group
        /// </summary>
        private void SaveAllChanges()
        {
            if (_changedTabPages != null && _changedTabPages.Count > 0)
            {
                List<string> actChangedTabPages = new List<string>();
                actChangedTabPages.AddRange(_changedTabPages);
                foreach (string tabPageName in actChangedTabPages)
                {
                    switch (tabPageName)
                    {
                        case CUSTOMER_AND_SUPPLIER_INFO:
                            _bSave6_Click(null, null);
                            break;
                        case REMOTE_SERVICES_SETTINGS:
                            SaveSmtpClick(null, null);
                            break;
                        case DHCP_SERVER_SETTINGS:
                            _bDHCPSave_Click(null, null);
                            break;
                        case SERIAL_PORT_SETTINGS:
                            SaveSerialPortClick(null, null);
                            break;
                        case DATABASE_BACKUP_SETTINGS:
                            _bSaveDatabaseBackup_Click(null, null);
                            break;
                        case EVENTLOG_EXPIRATION_SETTINGS:
                            _bSaveEventlogExpiration_Click(null, null);
                            break;
                        case COLOR_SETTINGS:
                            _bSave_Click(null, null);
                            break;
                        case AUTO_CLOSE_SETTINGS:
                            _buttonSave_Click(null, null);
                            break;
                        case EVENTLOG_SETTINGS:
                            _bEventlogsSave_Click(null, null);
                            break;
                        case ADVANCED_ACCESS_SETTINGS:
                            _bSave3_Click(null, null);
                            break;
                        case ADVANCED_SETTINGS:
                            _bSave4_Click(null, null);
                            break;
                        case LANGUAGE_SETTINGS:
                            _bSave2_Click(null, null);
                            break;
                        case SECURITY_SETTINGS:
                            _buttonSaveSecuritySettings_Click(null, null);
                            break;
                        case ALARM_SETTINGS:
                            _bSave5_Click(null, null);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Control changes on form closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.MdiFormClosing && _changedTabPages != null && _changedTabPages.Count > 0
                && CgpClient.Singleton != null && CgpClient.Singleton.IsLoggedIn && _SqlOnlineState)
            {
                DialogResult result =
                    MessageBox.Show(GetString("ValueChanged") + "\n" + GetString("SaveAfterCancel"), GetString("Question"),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        SaveAllChanges();
                        SqlServerOnlineStateChangedHandler.Singleton.UnregisterSqlServerOnlineStateChanged(_eventSqlServerOnlineStateChanged);
                        break;
                    case DialogResult.No:
                        ResetAllChanges();
                        SqlServerOnlineStateChangedHandler.Singleton.UnregisterSqlServerOnlineStateChanged(_eventSqlServerOnlineStateChanged);
                        break;
                }
            }

            // Reset settings for read acutal velues from server on form open
            _readNewGeneralOptionsFromServer = true;
            _listAPD = null;
            _tpLanguageFirstTimeEnter = true;

            base.OnFormClosing(e);
        }

        private void _epgSecuritySettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name.Equals(
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]]
                            .Name))
            {
                int minimalCodeLength;

                if (!Int32.TryParse(e.ChangedItem.Value.ToString(), out minimalCodeLength))
                {
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Value = e.OldValue;

                    return;
                }

                if (minimalCodeLength < 4)
                    minimalCodeLength = 4;
                else if (minimalCodeLength > 12)
                    minimalCodeLength = 12;


                _epgSecuritySettings.Items[
                    _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Value = minimalCodeLength.ToString();

                var maximalCodeLength = Int32.Parse(_epgSecuritySettings.Items[
                    _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Value.ToString());

                if (minimalCodeLength > maximalCodeLength)
                {
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Value = minimalCodeLength.ToString();

                    _epgSecuritySettings.Refresh();
                }
            }
            else if (e.ChangedItem.PropertyDescriptor.Name.Equals(
                        _epgSecuritySettings.Items[
                            _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]]
                                .Name))
            {
                var minimalCodeLength = Int32.Parse(_epgSecuritySettings.Items[
                    _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Value.ToString());

                int maximalCodeLength;

                if (!Int32.TryParse(e.ChangedItem.Value.ToString(), out maximalCodeLength))
                {
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Value = e.OldValue;

                    return;
                }

                if (maximalCodeLength < 4)
                    maximalCodeLength = 4;
                else if (maximalCodeLength > 12)
                    maximalCodeLength = 12;

                _epgSecuritySettings.Items[
                    _securitySettingsIndexes[SECURITY_SETTINGS_MAXIMAL_CODE_LENGTH]].Value = maximalCodeLength.ToString();

                if (maximalCodeLength < minimalCodeLength)
                {
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_MINIMAL_CODE_LENGTH]].Value = maximalCodeLength.ToString();

                    _epgSecuritySettings.Refresh();
                }
            }
            else if (e.ChangedItem.PropertyDescriptor.Name.Equals(
                        _epgSecuritySettings.Items[
                            _securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]]
                                .Name))
            {
                int delayToSaveAlarms;

                if (!Int32.TryParse(e.ChangedItem.Value.ToString(), out delayToSaveAlarms))
                {
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]].Value = e.OldValue;

                    return;
                }

                if (delayToSaveAlarms < 0)
                    delayToSaveAlarms = 0;
                else if (delayToSaveAlarms > 730)
                    delayToSaveAlarms = 730;

                _epgSecuritySettings.Items[
                    _securitySettingsIndexes[SECURITY_SETTINGS_DELAY_TO_SAVE_ALARMS_FROM_CARD_READERS]].Value = delayToSaveAlarms.ToString();
            }
            else if (e.ChangedItem.PropertyDescriptor.Name.Equals(
                        _epgSecuritySettings.Items[
                            _securitySettingsIndexes[SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY]]
                                .Name))
            {
                int changePasswordDays;

                if (!Int32.TryParse(e.ChangedItem.Value.ToString(), out changePasswordDays))
                {
                    _epgSecuritySettings.Items[
                        _securitySettingsIndexes[SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY]].Value = e.OldValue;

                    return;
                }

                if (changePasswordDays < 0)
                    changePasswordDays = 0;
                else if (changePasswordDays > 730)
                    changePasswordDays = 730;

                _epgSecuritySettings.Items[
                    _securitySettingsIndexes[SECURITY_SETTINGS_CHANGE_PASSWORD_EVERY]].Value = changePasswordDays.ToString();
            }

            EditTextChangerSecuritySettings(null, null);
        }

        private void _tpAlarmSettings_Enter(object sender, EventArgs e)
        {
            if (_listAPD == null)
            {
                ObtainAlarmPrirotities();
            }
        }

        public void ShowSMTPSettings()
        {
            Show();
            FocusSmtpServerSettings();
        }

        private void FocusSmtpServerSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(FocusSmtpServerSettings));
            }
            else
            {
                _tcGeneralOptions.SelectedTab = _tpRemoteServicesSettings;
                _epgSMTPServerSettings.Focus();
            }
        }

        private void _bSave6_Click(object sender, EventArgs e)
        {
            SaveToDatabaseCustomerAndSupplierInfo();
        }

        private void _epgCustomerInfo_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            EditTextChanger(CUSTOMER_AND_SUPPLIER_INFO);
        }

        private void _epgSupplierInfo_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            EditTextChanger(CUSTOMER_AND_SUPPLIER_INFO);
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            _pbSupplierLogo.Image = null;
            _binaryPhoto = null;
            _isPhotoModified = true;
        }

        private void _bAddPicture_Click(object sender, EventArgs e)
        {
            if (_ofdBrowseImage.ShowDialog() == DialogResult.OK)
            {
                string photoPath = _ofdBrowseImage.FileName;

                try
                {
                    Image imgPhoto = Image.FromFile(photoPath);
                    ProcessPhoto((Bitmap)imgPhoto);
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                }
            }
        }

        private bool ProcessPhoto(Bitmap photo)
        {
            Image newPhoto = null;
            try
            {
                if (photo.Width > MAX_PICTURE_WIDTH || photo.Height > MAX_PICTURE_HEIGHT)
                {
                    // Resize original Image.
                    newPhoto = ImageResizeUtility.GetResizedImage(photo, MAX_PICTURE_WIDTH, MAX_PICTURE_HEIGHT,
                        ImageResizeUtility.ResizeMode.Normal, true);
                }
                else
                {
                    newPhoto = photo;
                }

                if (newPhoto == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _pbSupplierLogo,
                        GetString("ErrorOpenPhotoFileFailed"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    return false;
                }

                _pbSupplierLogo.Image = newPhoto;
                _isPhotoModified = true;
                // Convert an image to a byte array. 
                Byte[] binaryPhotoData = (Byte[])new ImageConverter().ConvertTo(newPhoto, typeof(Byte[]));
                _binaryPhoto = new BinaryPhoto(binaryPhotoData, ".jpg");
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            return true;
        }

        private void SaveSupplierLogo()
        {
            SafeThread<BinaryPhoto>.StartThread(DoSaveSupplierLogo, _binaryPhoto);
        }

        private void DoSaveSupplierLogo(BinaryPhoto binaryPhoto)
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.SaveSupplierLogo(binaryPhoto);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _bInsertLicenceFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "licence files(*.lkey)|*.lkey";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bool isValid, needRestart;
                var fileBytes = File.ReadAllBytes(openFileDialog.FileName);

                if (!CgpClient.Singleton.MainServerProvider.InsertLicenceFile(
                    Path.GetFileName(openFileDialog.FileName),
                    fileBytes,
                    out isValid,
                    out needRestart))
                {
                    Dialog.Error(GetString("ErrorInsertLicenseFile"));
                    return;
                }

                if (!isValid)
                {
                    Dialog.Error(GetString("ErrorInValidLicenseFile"));
                    return;
                }

                if (needRestart)
                {
                    Dialog.Warning(GetString("WarningRestartClientRequired"));
                }

                FillLicenseSettings();
            }
        }

        public int MinimalCodeLength
        {
            get { return _serverGeneralOptions.MinimalCodeLength; }
        }

        public int MaximalCodeLength
        {
            get { return _serverGeneralOptions.MaximalCodeLength; }
        }
    }

    public class CbAlarmPriority
    {
        private AlarmPriority _alarmPriority;
        private string _alarmPriorityName;

        public AlarmPriority AlarmPriority { get { return _alarmPriority; } set { _alarmPriority = value; } }
        public string LocalizeAlarmPriority { get { return _alarmPriorityName; } set { _alarmPriorityName = value; } }
        public CbAlarmPriority(AlarmPriority alarmPriority, string localizeName)
        {
            _alarmPriority = alarmPriority;
            _alarmPriorityName = localizeName;
        }

        public override string ToString()
        {
            return _alarmPriorityName;
        }

        public CbAlarmPriority Self
        {
            get { return this; }
        }
    }

    public class CbClosestParentObject
    {
        private ObjectType? _objectType;
        private string _name;

        public ObjectType? ObjectType { get { return _objectType; } set { _objectType = value; } }
        public string Name { get { return _name; } set { _name = value; } }

        public CbClosestParentObject(ObjectType? objectType, string name)
        {
            _objectType = objectType;
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public CbClosestParentObject Self
        {
            get { return this; }
        }
    }

    public class DataGridAlarmType
    {
        private AlarmType _alarmType;
        private string _alarmTypeName;
        private CbAlarmPriority _alarmPriority;
        private CbClosestParentObject _closestParentObject;
        private CbClosestParentObject _secondClosestParentObject;

        public DataGridAlarmType(AlarmType alarmType, string alarmTypeName, CbAlarmPriority alarmPriority, CbClosestParentObject closestParentObject, CbClosestParentObject secondClosestParentObject)
        {
            _alarmType = alarmType;
            _alarmTypeName = alarmTypeName;
            _alarmPriority = alarmPriority;
            _closestParentObject = closestParentObject;
            _secondClosestParentObject = secondClosestParentObject;
        }

        public string AlarmTypeName { get { return _alarmTypeName; } set { _alarmTypeName = value; } }
        public CbAlarmPriority AlarmPriorityName
        {
            get { return _alarmPriority; }
            set { _alarmPriority = value; }
        }
        public AlarmType AlarmType { get { return _alarmType; } }
        public AlarmPriority AlarmPriority { get { return _alarmPriority.AlarmPriority; } }

        public CbClosestParentObject ClosestParentObjectName { get { return _closestParentObject; } set { _closestParentObject = value; } }
        public ObjectType? ClosestParentObject { get { return _closestParentObject.ObjectType; } }
        public CbClosestParentObject SecondClosestParentObjectName { get { return _secondClosestParentObject; } set { _secondClosestParentObject = value; } }
        public ObjectType? SecondClosestParentObject { get { return _secondClosestParentObject.ObjectType; } }
    }
}