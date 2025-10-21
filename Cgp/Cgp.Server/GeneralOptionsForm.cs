using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;
using Contal.IwQuick.Net;
using Contal.IwQuick.UI;
using System.IO;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using System.Net.NetworkInformation;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.Cgp.Server
{
    public partial class GeneralOptionsForm : TranslateForm
    {
        private GeneralOptions _generalOptions = null;

        public GeneralOptionsForm(LocalizationHelper localizationHelper, GeneralOptions generalOptions)
            : base(localizationHelper)
        {
            InitializeComponent();

            _generalOptions = generalOptions;

            SetStartValues();
            ShowLanguages();
            this.Height = (this.Height - this.ClientSize.Height) + _bSave.Location.Y + _bSave.Height + 100;
            _bSave.DialogResult = DialogResult.OK;
            _bClose.DialogResult = DialogResult.Cancel;
        }

        protected override void AfterTranslateForm()
        {
            _generalOptions.CurrentLanguage = LocalizationHelper.ActualLanguage;

            try
            {
                _generalOptions.SaveSettingsToRegistry();
            }
            catch
            {
            }
        }

        private void KeyPressForHexa(object sender, KeyPressEventArgs e)
        {
            // Checks for numbers
            if ((e.KeyChar < '0' || e.KeyChar > '9') && !(e.KeyChar >= 'a' && e.KeyChar <= 'f') && !(e.KeyChar >= 'A' && e.KeyChar <= 'F'))
                e.Handled = true;

            // Checks for backspace
            if (e.KeyChar == 8)
                e.Handled = false;
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            if (!_generalOptions.IsConfigured)
            {
                if (!Dialog.WarningQuestion(GetString("GeneralOptionsFormUnconfiguredReallyContinue")))
                    return;
            }

            Close();
        }

        private void _bSave_Click(object sender, EventArgs e)
        {
            if (_generalOptions != null)
            {
                int remotingServerPort = (int)_eServerPort.Value;
                if (TcpUdpPort.IsFreeTcp(remotingServerPort, true))
                {
                    _generalOptions.RemotingServerPort = remotingServerPort;
                }
                else
                {
                    Dialog.Error(GetString("GeneralOptionsFormTcpPortIsNotFree"), GetString("GeneralOptionsFormReenterRemotingServerPort"));
                    _eServerPort.Focus();
                    return;
                }

                _generalOptions.RemotingServerIpAddress = string.Empty;
                if (_eRemotingIpAddress.Text != null && _eRemotingIpAddress.Text != string.Empty)
                {
                    if (IPHelper.IsValid4(_eRemotingIpAddress.Text))
                    {
                        _generalOptions.RemotingServerIpAddress = _eRemotingIpAddress.Text;
                    }
                    else
                    {
                        Dialog.Error(GetString("GeneralOptionsFormServerIpAddressIsNotValid"), GetString("GeneralOptionsFormReenterIpAddress"));
                        _eRemotingIpAddress.Focus();
                        return;
                    }
                }

                _generalOptions.FriendlyName = _eFriendlyName.Text;

                try
                {
                    _generalOptions.SaveSettingsToRegistry();
                }
                catch (Contal.IwQuick.DoesNotExistException)
                {
                    Dialog.Error(GetString("GeneralOptionsFormServerNoRegistryWriteAccess"));
                }
                catch
                {
                }

                Close();
            }
        }

        private void SetStartValues()
        {
            if (_generalOptions.RemotingServerPort != 0)
                _eServerPort.Text = _generalOptions.RemotingServerPort.ToString();
            else
                _eServerPort.Text = "";

            if (_generalOptions.RemotingServerIpAddress != null)
            {
                _eRemotingIpAddress.Text = _generalOptions.RemotingServerIpAddress;
            }
            else
            {
                _eRemotingIpAddress.Text = string.Empty;
            }

            if (_generalOptions.CurrentLanguage != null && _generalOptions.CurrentLanguage != string.Empty)
                LocalizationHelper.SetLanguage(_generalOptions.CurrentLanguage);
            else
                LocalizationHelper.SetLanguage(Contal.Cgp.Globals.CgpServerGlobals.DEFAULT_LANGUAGE);

            _eFriendlyName.Text = _generalOptions.FriendlyName;
        }

        private void GeneralOptionsForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadAvailableLicences()
        {
            List<string> licences = new List<string>();
            foreach (string fullFileName in Directory.GetFiles(QuickPath.AssemblyStartupPath))
            {
                try
                {
                    if (Path.GetExtension(fullFileName).Equals(".lkey"))
                        licences.Add(Path.GetFileNameWithoutExtension(fullFileName));
                }
                catch
                { }
            }

            string text = _lbAvailableLicences.Text;

            _lbAvailableLicences.DataSource = null;
            if (licences.Count > 0)
                _lbAvailableLicences.DataSource = licences;

            _lbAvailableLicences.Text = text;
        }

        private void ProcessLicenseFromRegistry()
        {
            if (_generalOptions.LicencePath != null && _generalOptions.LicencePath != string.Empty)
            {
                if (!File.Exists(_generalOptions.LicencePath) || (Path.GetDirectoryName(_generalOptions.LicencePath) != QuickPath.AssemblyStartupPath))
                {
                    _generalOptions.LicencePath = string.Empty;
                    Dialog.Warning(GetString("LicenseFileNotInServerDir"));
                }
                else if (Path.GetExtension(_generalOptions.LicencePath) != ".lkey")
                {
                    _generalOptions.LicencePath = string.Empty;
                    Dialog.Warning(GetString("LicenseFileWrongExtension"));
                }
                else
                {
                    _lbAvailableLicences.Text = Path.GetFileNameWithoutExtension(_generalOptions.LicencePath);
                }
            }
        }

        private void _lbAvailableLicences_SelectedValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_lbAvailableLicences.Text))
                return;

            string fullLicenceFileName = Path.Combine(QuickPath.AssemblyStartupPath, _lbAvailableLicences.Text + ".lkey");

            if (!File.Exists(fullLicenceFileName))
                return;

            LicenseHelper.Singleton.CheckLicence(fullLicenceFileName);

            if (LicenseHelper.Singleton.IsValid)
                ControlNotification.Singleton.Info(NotificationPriority.JustOne,
                    _lbAvailableLicences,
                    "License is valid\n" + fullLicenceFileName,
                    ControlNotificationSettings.Default);
            else
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _lbAvailableLicences,
                    "Invalid license \n" + fullLicenceFileName, ControlNotificationSettings.Default);
        }

        private void GeneralOptionsForm_Shown(object sender, EventArgs e)
        {
            ListPossibleIPs();

            ProcessLicenseFromRegistry();
            LoadAvailableLicences();

            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                    DllUser32.FlashWindow(this.Handle, true);
            }
            catch
            {
            }
        }

        private void ListPossibleIPs()
        {
            SafeThread<bool>.StartThread(ListPossibleIPsThread, false);

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            ListPossibleIPsThread(true);
        }

        private volatile object _ipListingSync = new object();

        private void ListPossibleIPsThread(bool byIfaceChange)
        {
            int count = 0;
            string[] arrayIPs = new string[16];

            lock (_ipListingSync)
            {
                NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

                try
                {

                    if (nis != null)
                        foreach (NetworkInterface ni in nis)
                        {
                            if (ni.OperationalStatus == OperationalStatus.Up ||
                                ni.OperationalStatus == OperationalStatus.Dormant ||
                                ni.OperationalStatus == OperationalStatus.LowerLayerDown)
                            {
                                switch (ni.NetworkInterfaceType)
                                {
                                    case NetworkInterfaceType.Ethernet:
                                    case NetworkInterfaceType.FastEthernetFx:
                                    case NetworkInterfaceType.GigabitEthernet:
                                    case NetworkInterfaceType.Wireless80211:
                                    case NetworkInterfaceType.Loopback:
                                    case NetworkInterfaceType.Tunnel:
                                        try
                                        {
                                            IPInterfaceProperties ips = ni.GetIPProperties();
                                            foreach (UnicastIPAddressInformation uip in ips.UnicastAddresses)
                                            {
                                                string ipString = uip.Address.ToString();
                                                if (uip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                                    && ipString.IndexOf("169.254") < 0)
                                                {
                                                    if (count < arrayIPs.Length)
                                                    {
                                                        arrayIPs[count++] = ipString;
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }

                                        break;
                                }
                            }
                        }
                }
                catch
                {
                }
            }

            if (count > 0)
                RefreshBindIPList(arrayIPs, count, byIfaceChange);
        }



        private void RefreshBindIPList(string[] ips, int count, bool byIfaceChange)
        {
            if (this.InvokeRequired)
            {
                if (ips != null &&
                    ips.Length > 0)
                    try
                    {
                        this.Invoke(new Action<string[], int, bool>(RefreshBindIPList), (object)ips, count, byIfaceChange);
                    }
                    catch
                    {
                    }
            }
            else
            {
                _eRemotingIpAddress.Items.Clear();
                _eRemotingIpAddress.Items.Add(String.Empty);

                for (int i = 0; i < count; i++)
                {
                    if (ips[i] != null &&
                        ips[i].Length > 0)
                        _eRemotingIpAddress.Items.Add(ips[i]);
                }

                _eRemotingIpAddress.Text = _generalOptions.RemotingServerIpAddress;

                if (byIfaceChange)
                    _eRemotingIpAddress.Focus();
            }
        }

        private void _bRefreshLicenceList_Click(object sender, EventArgs e)
        {
            LoadAvailableLicences();
        }

        public bool SkipDBReconfiguration
        {
            get
            {
                return _chbSaveAndSkipDB.Checked;
            }
        }

        private void _chbSaveAndSkipDB_CheckedChanged(object sender, EventArgs e)
        {
            if (_chbSaveAndSkipDB.Checked)
            {
                if (!Dialog.WarningQuestion(GetString("GeneralOptionsFormReallySkipDB")))
                    _chbSaveAndSkipDB.Checked = false;
            }
        }

        private void _eRemotingIpAddress_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
