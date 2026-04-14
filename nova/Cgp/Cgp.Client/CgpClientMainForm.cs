using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.UI;

using Contal.Cgp.BaseLib;
using System.Threading;
using Contal.IwQuick.Threads;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;
using Color = System.Drawing.Color;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;
using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick.UI.Controls;
using ComponentFactory.Krypton.Toolkit;
using Cgp.Components;

namespace Contal.Cgp.Client
{
    /// <summary>
    ///     Main form of the CGP client. 
    /// </summary>    
    public partial class CgpClientMainForm :
#if DESIGNER
        Form
#else
 CgpTranslateForm, ICgpClientMainForm
#endif

    {
        private const int MAX_QUICK_BUTTONS = 10;
        private const int MAX_FORM_PROGRESS = 20; // Default maximum value for the progress bar
        /// <summary>
        /// The maximum length of string  in characters displayed as a Text property of the ToolStrip Icon.
        /// </summary>
        private const int MAX_STRING_LENGTH = 40;

        private RecentObjectList _recObjList;
        private readonly Dictionary<ToolStripButton, bool> _leftMenuItems = new Dictionary<ToolStripButton, bool>();
        //private Dictionary<string, int> _recentMenuItems = new Dictionary<string, int>();
        private ICollection<ServerAlarmCore> _listAlarms = null;
        private SizeF _scaleF = new SizeF(1.0F, 1.0F); // DPI 100%
        public AlarmsDialog _alarmsDialog = null;
        public AlarmDetailsForm _alarmDetails = null;
        PleaseWaitMonitor _pleaseWaitMonitorEditForms = null;
        PleaseWaitMonitor _pleaseWaitMonitorSplash = null;
        private readonly DVoid2Void _eventColorChanged = null;
        private readonly DVoid2Void _eventCustomerAndSupplierInfoChanged = null;
        private readonly DVoid2Void _eventDatabaseBackupSettingsChanged = null;
        private readonly DVoid2Void _eventDatabaseExpirationEventlogSettingsChanged = null;
        //private Contal.IwQuick.DVoid2Void _eventDhcpServerSettingsChanged = null;

        //autoClose
        ToolStripItem _tsiSelectedWindow = null;
        private readonly Dictionary<Form, ITimer> _formsActiveTimes = new Dictionary<Form, ITimer>();
        private readonly TimerManager _autocloseTimerManager = new TimerManager();
        private readonly Action<bool> _eventSqlServerOnlineStateChanged = null;
        private bool _IsRemotingOn = false;

        private readonly MouseActivityHook _mouseWheelHooks = new MouseActivityHook();

        KryptonManager _kryptonManager = new KryptonManager();

        private CgpClientMainForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            // Init Krypton
            SetKryptonManagerStyles();

#if DEBUG
            TextExtra = "DEBUG!";
#endif

            SuspendLayout();
            InitializeComponent();
            // At run-time, the actual resolution is stored in the CurrentAutoScaleDimensions property.
            // The AutoScaleFactor property dynamically calculates the ratio
            // between the run-time and design-time scaling resolution.
            // Declare your application to be DPI-aware, add <dpiAware>true</dpiAware> to the application manifest.
            if (AutoScaleFactor.IsEmpty == false)
            {
                _scaleF = AutoScaleFactor;
            }
            ResumeLayout(true);
            // align along the left menu
            //_anyProgress.Width = Convert.ToInt32(_anyProgress.Width * _scaleF.Width);

            ChangeToolTipTextHome();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            SafeThread.StartThread(InitLeftMenu);
            _bLeftMenuUp.Enabled = false;
            _bLeftMenuDown.Enabled = _leftMenu.PreferredSize.Height > _pLeftMenuToolStrip.Height;
            //MouseWheel += MouseScroll;
            LeftMenuSetFocusAfterClick();
            ChangeAlarmsHandler.Singleton.RegisterChangeAlarms(ChangeAlarms);
            ChangeAlarmsHandler.Singleton.RegisterDeletedAlarm(DeletedAlarm);
            _tsAlarms.Visible = false;
            _tsHome.Visible = false;
            DeletedObjectHandler.Singleton.Register(DeletedObjectEvent);

            _eventColorChanged = ColorSettingsChanged;
            ColorSettingsChangedHandler.Singleton.RegisterColorChanged(_eventColorChanged);
            _eventCustomerAndSupplierInfoChanged = CustomerAndSupplierInfoChanged;
            CustomerAndSupplierInfoChangedHandler.Singleton.RegisterInfoChanged(_eventCustomerAndSupplierInfoChanged);
            AutoCloseSettingsChangedHandler.Singleton.RegisterAutoCloseChanged(AutoCloseSettingsChanged);
            LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
            _eventSqlServerOnlineStateChanged = SqlServerOnlineStateChanged;
            SqlServerOnlineStateChangedHandler.Singleton.RegisterSqlServerOnlineStateChanged(_eventSqlServerOnlineStateChanged);

            _eventDatabaseBackupSettingsChanged = DatabaseBackupSettingsChanged;
            DatabaseBackupSettingsChangedHandler.Singleton.RegisterDatabaseBackupSettingsChanged(_eventDatabaseBackupSettingsChanged);
            _eventDatabaseExpirationEventlogSettingsChanged = DatabaseExpirationEventlogSettingsChanged;
            DatabaseExpirationEventlogSettingsChangedHandler.Singleton.RegisterDatabaseExpirationEventlogSettingsChanged(_eventDatabaseExpirationEventlogSettingsChanged);
            SecuritySettingsChangedHandler.Singleton.RegisterSecuritySettingsChanged(SecuritySettingsChanged);
            AlarmSettingsChangedHandler.Singleton.RegisterAlarmSettingsChanged(AlarmSettingsChanged);
            EventlogsChangedHandler.Singleton.RegisterEventlogsChanged(EventlogsChanged);
            RemoteServicesSettingsChangedHandler.Singleton.RegisterRemoteServicesSettingsChanged(RemoteServicesSettingsChanged);
            SerialPortSettingsChangedHandler.Singleton.RegisterSerialPortSettingsChanged(SerialPortSettingsChanged);
            RemoteServicesSettingsChangedHandler.Singleton.RegisterRemoteServicesSettingsChanged(NtpSettingsChanged);
            AdvancedAccessSettingsChangedHandler.Singleton.RegisterAdvancedAccessSettingsChanged(AdvancedAccessSettingsChanged);
            AdvancedSettingsChangedHandler.Singleton.RegisterAdvancedSettingsChanged(AdvancedSettingsChanged);
            WarningSetMaxEventsCountHandler.Singleton.RegisterWarningSetMaxEventsCount(WarningSetMaxEventsCount);
            DemoPerionHasExpiredHandler.Singleton.RegisterDemoPerionHasExpired(DemoPeriodHasExpired);

            KeyPreview = true;
            KeyDown += ACgpTableForm_KeyDown;

            CgpClient.Singleton.LocalizationHelper.LanguageChanged += TranslateCgpMainForm;
            CgpClient.Singleton.PluginProxyGained += PluginProxyGained;
            CgpClient.Singleton.PluginProxyLost += PluginProxyLost;
            CgpClient.Singleton.LoggedAsChanged += LoggedAsChanged;

            _mouseWheelHooks.MouseWheel += MouseWheelHooksMouseWheel;
            _mouseWheelHooks.MouseMove += _mouseWheelHooks_MouseMove;
            _mouseWheelHooks.MouseDown += _mouseWheelHooks_MouseDown;
            _mouseWheelHooks.MouseUp += _mouseWheelHooks_MouseUp;
            _mouseWheelHooks.Hook();
        }

        /// <summary>
        /// General style for all windows and controls
        /// </summary>
        private void SetKryptonManagerStyles()
        {
            // Style
            _kryptonManager.GlobalPaletteMode = PaletteModeManager.Office2010Silver;
            _kryptonManager.GlobalApplyToolstrips = false;

            // Update windows frame colors (Windows 10 only!)
            if (System.Environment.OSVersion.Version.Major >= 10)
            {
                // Get Colorization color and its usage
                Microsoft.Win32.RegistryKey registryKey = null;

                try
                {
                    registryKey = RegistryHelper.GetOrAddKey(CgpServerGlobals.WINDOWS_DWM_SETTINGS);
                    if (registryKey != null)
                    {
                        if (Convert.ToBoolean(registryKey.GetValue(CgpServerGlobals.DWM_PREVALENCE_VALUE)) == true)
                        {
                            Int32 value = Convert.ToInt32(registryKey.GetValue(CgpServerGlobals.DWM_COLORIZATION_COLOR_VALUE));
                            Color tmpColor = Color.FromArgb(value);
                            // Remove Alpha channel
                            LocalizationHelper.KryptonColorizationColor = Color.FromArgb(255, tmpColor.R, tmpColor.G, tmpColor.B);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private bool IsInDragingArea(int x, int y)
        {
            try
            {
                var screenRectangle = RectangleToScreen(ClientRectangle);

                int posX = x - screenRectangle.Left;
                int posY = y - screenRectangle.Top;

                return (posX > _splitContainerLeftMenu.Left + _splitContainerLeftMenu.Width - 6
                        && posX < _splitContainerLeftMenu.Left + _splitContainerLeftMenu.Width
                        && posY > _splitContainerLeftMenu.Top
                        && posY < _splitContainerLeftMenu.Top + _splitContainerLeftMenu.Height);
            }
            catch
            {
                return false;
            }
        }

        private bool _dragingLeftMenu;
        private int _lastPosX;
        
        void _mouseWheelHooks_MouseUp(int x, int y)
        {
            if (_dragingLeftMenu)
                _leftMenu.MinimumSize = new Size(_splitContainerLeftUpperMenu.Panel2.Width + _splitContainerLeftUpperMenu.SplitterWidth, 50);

            _dragingLeftMenu = false;
        }

        void _mouseWheelHooks_MouseDown(int x, int y)
        {
            _dragingLeftMenu = IsInDragingArea(x, y);
            _lastPosX = x;

            if (_dragingLeftMenu)
            {
                _leftMenu.MinimumSize = new Size(158, 50);
            }
        }

        void _mouseWheelHooks_MouseMove(int x, int y)
        {
            if (_dragingLeftMenu)
            {
                int delta = x - _lastPosX;

                if (_splitContainerLeftMenu.Width + delta >= _leftMenu.Width + 4)
                    _splitContainerLeftMenu.Width += delta;

                _lastPosX = x;
            }

            if (IsInDragingArea(x, y))
                Cursor = Cursors.SizeWE;
            else
            {
                if (Cursor == Cursors.SizeWE)
                    Cursor = Cursors.Default;

                _dragingLeftMenu = false;
            }
        }

        void MouseWheelHooksMouseWheel(int delta)
        {
            ScrollLeftMenu(delta);
        }

        private void LoggedAsChanged(string loggedAs)
        {
            if (!string.IsNullOrEmpty(loggedAs))
            {
                StatusReport.Info($"Starting the login procedure for the user {loggedAs}.");
            }

            ClearRecentList();
            UpdateLoggedAs(loggedAs);
            RefreshLeftMenuItems();
            
            if (!CgpClient.Singleton.IsLoggedIn)
            {
                StopAutoCloseManager();

                if (!HidingWindows)
                    HideAllWindows();
            }

            _isLoginPasswordDialogCancelled = false;

            if (CgpClient.Singleton.MainServerProvider != null)
                StatusReport.SetVisible(false /*CgpClient.Singleton.MainServerProvider.CheckTimetecLicense()*/);
            StatusReport.Info(string.IsNullOrEmpty(loggedAs) ? "The user has been logged out." : $"The user {loggedAs} has been logged in.");
        }


        public void ShowLoginPasswordDialog()
        {
            SafeThread.StartThread(ShowLoginPasswordDialogThread);
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Space | Keys.Alt:
                {
                    ShowListboxFormAdd();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private void ShowListboxFormAdd()
        {
            List<IModifyObject> listModObj = new List<IModifyObject>();
            var listFromDatabase = CgpClient.Singleton.MainServerProvider.GetIModifyObjects();

            if (listFromDatabase != null)
                listModObj.AddRange(listFromDatabase);

            ListboxFormAdd formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("FastSearchForm"));
            ListOfObjects outModObjects;
            formAdd.ShowDialogMultiSelect(out outModObjects);
            if (outModObjects != null && outModObjects.Count > 0)
            {
                foreach (IModifyObject obj in outModObjects)
                {
                    if (obj != null)
                    {
                        AOrmObject aOrmObject = DbsSupport.GetTableObject(obj.GetOrmObjectType, obj.GetId.ToString());
                        DbsSupport.OpenEditForm(aOrmObject);
                    }
                }
            }
        }

        private readonly object _lockShowLoginPasswordDialog = new object();

        private bool _isLoginPasswordDialogShown;
        private bool _isLoginPasswordDialogCancelled;

        private void ShowLoginPasswordDialogThread()
        {
            BeginInvoke(new Action(() =>
            {
                string newLoggedAs = CgpClient.Singleton.LoggedAs;

                lock (_lockShowLoginPasswordDialog)
                {
                    if (_isLoginPasswordDialogShown)
                        return;

                    if (_isLoginPasswordDialogCancelled)
                        return;

                    _isLoginPasswordDialogShown = true;
                }

                if (LoginPasswordForm.Singleton.ShowDialog(
                    ref newLoggedAs,
                    true) == DialogResult.Cancel)
                {
                    _isLoginPasswordDialogCancelled = true;
                }

                CgpClient.Singleton.LoggedAs = newLoggedAs;

                _isLoginPasswordDialogShown = false;
            }));
        }

        protected override void OnLoad(EventArgs e)
        {
            // Show Splash at the center of the screen
            Rectangle screenRect = Screen.GetBounds(Bounds);
            Size clientSize = new Size((int)(screenRect.Width / 2), (int)(screenRect.Height / 2));
            Point location = new Point(screenRect.Width / 2 - 460 /*ClientSize.Width*/ / 2, screenRect.Height / 2 - 150 /*ClientSize.Height*/ / 2); // AboutForm dimensions!

            _pleaseWaitMonitorSplash = new PleaseWaitMonitor(location, PleaseWaitMode.SplashAbout);

            base.OnLoad(e);
            Font = CgpUIDesign.Default;
            CgpClient.Singleton.Init();

            CgpClientMainFormLoad();
        }

        private void TranslateCgpMainForm()
        {
            TranslateCgpVisualPluginForms(
                CgpClient.Singleton.PluginManager.GetVisualPlugins());

            ChangeAlarms();

            _tsHome.Text = LocalizationHelper.GetString("General_tsHome");
            UpdateRemotingStatus(!CgpClient.Singleton.IsConnectionLost(false));
        }

        public ICollection<ServerAlarmCore> ListAlarms { get { return _listAlarms; } }

        void ACgpTableForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (AboutForm.Singleton.IsVisible)
                {
                    AboutForm.Singleton.HideAbout();
                }
                else
                {
                    CloseActWindow();
                }
            }
        }

        private void CloseActWindow()
        {
            try
            {
                if (_focusedWindow != null)
                {
                    _focusedWindow.CallEscape();
                }
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        void LocalizationHelper_LanguageChanged()
        {
            ChangeToolTipTextHome();
        }

        private void ChangeToolTipTextHome()
        {
            if (_leftMenu.Items.Count > 0)
            {
                _leftMenu.Items[0].ToolTipText = GetString("CgpClientMainForm_goHome").Replace(" ", "");
            }
        }

        private static volatile CgpClientMainForm _singleton = null;
        private static readonly object _syncRoot = new object();

        public static CgpClientMainForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CgpClientMainForm();
                    }

                return _singleton;
            }
        }

        public LocalizationHelper GetLocalizationHelper()
        {
            return LocalizationHelper;
        }

// ReSharper disable once UnusedMember.Local
// ReSharper disable UnusedParameter.Local
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
// ReSharper restore UnusedParameter.Local
        {
            CloseAllWindowsWithCheck();
        }

        private void ColorSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadColorFromRegistry();
            ChangeAlarms();
        }

        private void CustomerAndSupplierInfoChanged()
        {
            GeneralOptionsForm.Singleton.ReloadCustomerAndSupplierInfo();
        }

        private void DatabaseBackupSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadDatabaseBackupSettings();
        }


        private void DatabaseExpirationEventlogSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadDatabaseExpirationEventlogSettings();
        }

        private void SecuritySettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadSecuritySettings();
        }

        private void AlarmSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadAlarmSettings();
        }

        private void EventlogsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadEventlogs();
        }

        private void RemoteServicesSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadRemoteServicesSettings();
        }

        private void SerialPortSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadSerialPortSettings();
        }

        private void NtpSettingsChanged()
        {
        }

        private void AdvancedAccessSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadAdvancedAccessSettings();
        }

        private void AdvancedSettingsChanged()
        {
            GeneralOptionsForm.Singleton.ReloadAdvancedSettings();
        }

        private void WarningSetMaxEventsCount(List<string> ccuNames)
        {
            if (ccuNames != null && ccuNames.Count > 0)
                ShowDialogWarningSetMaxEventsCount(ccuNames);
        }

        private void ShowDialogWarningSetMaxEventsCount(List<string> ccuNames)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<string>>(ShowDialogWarningSetMaxEventsCount), ccuNames);
            }
            else
            {
                if (ccuNames != null && ccuNames.Count > 0)
                {
                    string strCCUNames = string.Empty;
                    foreach (string ccuName in ccuNames)
                    {
                        if (strCCUNames != string.Empty)
                            strCCUNames += ", ";

                        strCCUNames += ccuName;
                    }

                    if (ccuNames.Count > 1)
                    {
                        Dialog.Warning(GetString("WarningSetMaxEventsCountForCCUs") + ": " + strCCUNames +
                            Environment.NewLine + Environment.NewLine +
                            GetString("WarningSetMaxEventsCount"));
                    }
                    else
                    {
                        Dialog.Warning(GetString("WarningSetMaxEventsCountForCCU") + ": " + strCCUNames +
                            Environment.NewLine + Environment.NewLine +
                            GetString("WarningSetMaxEventsCount"));
                    }
                }
            }
        }

        private void SqlServerOnlineStateChanged(bool online)
        {
            if (online)
            {
                if (!CgpClient.Singleton.IsLoggedIn)
                {
                    UpdateRemotingStatus(true);
                    ShowLoginPasswordDialog();
                }
            }
            else
            {
                UpdateRemotingStatusSqlServerDisconnected();

                CgpClient.Singleton.LoggedAs = null;
            }

            EnableBtnSqlServerOnlineState(online);
        }

        private void EnableBtnSqlServerOnlineState(bool enable)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new DBool2Void(EnableBtnSqlServerOnlineState), 
                    enable);

                return;
            }

            _loginMmi.Enabled = enable;
            _goHome.Enabled = enable;
        }

        private void AutoCloseSettingsChanged()
        {
            ChangeAutoClose();
            GeneralOptionsForm.Singleton.ReloadAutoCloseSettings();
        }

        private void ChangeAutoClose()
        {
            if (!GetAutoCloseTurnedOn)
            {
                foreach (var item in _formsActiveTimes)
                {
                    if (item.Value != null)
                    {
                        item.Value.StopTimer();
                    }
                }
                _formsActiveTimes.Clear();
            }
            else
            {
                foreach (ToolStripItem item in _tsOpenWindows.Items)
                {
                    RemoveFromActiveTimes(item.Tag as Form);
                    Font font = new Font(item.Font, FontStyle.Regular);
                    item.Font = font;

                    if (item.BackColor != Color.LightBlue)
                    {
                        StartAutoCloseTimer(item.Tag as Form);
                    }
                    else
                    {
                        font = new Font(item.Font, FontStyle.Bold);
                        item.Font = font;
                        _tsiSelectedWindow = item;
                    }
                }
            }
        }

        public Color GetDragDropTextColor
        {
            get { return GeneralOptionsForm.Singleton.GetDragDropTextColor; }
        }
        public Color GetDragDropBackgroundColor
        {
            get { return GeneralOptionsForm.Singleton.GetDragDropBackgroundColor; }
        }
        public Color GetReferenceTextColor
        {
            get { return GeneralOptionsForm.Singleton.GetReferenceTextColor; }
        }
        public Color GetReferenceBackgroundColor
        {
            get { return GeneralOptionsForm.Singleton.GetReferenceBackgroundColor; }
        }

        public bool GetAutoCloseTurnedOn
        {
            get { return GeneralOptionsForm.Singleton.GetAutoCloseTurnedOn; }
        }
        public int GetAutoCloseTimeout
        {
            get { return GeneralOptionsForm.Singleton.GetAutoCloseTimeout; }
        }

        readonly List<Form> _undockedForms = new List<Form>();
        public void AddToUndockedForms(Form undockedForm)
        {
            if (!_undockedForms.Contains(undockedForm))
            {
                _undockedForms.Add(undockedForm);
            }
        }

        public void RemoveFormUndockedForms(Form undockedForm)
        {
            if (_undockedForms.Contains(undockedForm))
            {
                _undockedForms.Remove(undockedForm);
            }
        }

        private void SafeHideFormImage()
        {
            this.BeginInvokeInUI(HideFormImage);
        }

        private void HideFormImage()
        {
            if (_pbFormImage != null)
            {
                _pbFormImage.Image = null;
                _pbFormImage.Visible = false;
            }
        }

        private readonly object _lockHideAllWindows = new object();
        public void HideAllWindows()
        {
            lock (_lockHideAllWindows)
            {
                //SafeHideTsAlarms();
                SafeHideFormImage();

                _hidingWindows = true;
                try
                {
                    for (int i = _tsOpenWindows.Items.Count - 1; i >= 0; i--)
                    {
                        var form = _tsOpenWindows.Items[i].Tag as Form;

                        if (form !=null && !form.Visible)
                        {
                            RemoveFromOpenWindows(form);
                        }
                        //else
                        //{
                        //    HideWindow(_tsOpenWindows.Items[i].Tag as Form);
                        //}
                    }
                    //ClearTsi();

                    for (int i = MdiChildren.Length - 1; i >= 0; i--)
                    {
                        if (null != MdiChildren[i] && MdiChildren[i].Visible)
                        {
                            HideWindow(MdiChildren[i]);
                        }
                    }

                    for (int i = _undockedForms.Count - 1; i >= 0; i--)
                    {
                        HideUndockWindow(_undockedForms[i]);
                    }
                    HideExternWindows();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    _hidingWindows = false;
                }
            }
        }

        public bool HideAllWindowsWithCheck()
        {
            //SafeHideTsAlarms();
            SafeHideFormImage();

            _hidingWindows = true;
            try
            {
                HideExternWindows();
                if (!CloseAllWithChecks())
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                _hidingWindows = false;
            }
            return true;
        }

        private bool CloseAllWithChecks()
        {
            while (_undockedForms.Count > 0)
            {
                if (_undockedForms[0].Visible)
                {
                    if (!CheckAfterCancel(_undockedForms[0]))
                    {
                        return false;
                    }
                }
                else
                    _undockedForms.RemoveAt(0);
            }
            int openWindCount = _tsOpenWindows.Items.Count;
            int counter = 0;
            while (_tsOpenWindows.Items.Count > 0 && (counter < openWindCount))
            {
                var form = _tsOpenWindows.Items[0].Tag as Form;

                if (form != null)
                {
                    if (form.Visible)
                    {
                        if (!CheckAfterCancel(form))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        RemoveFromOpenWindows(form);
                    }
                    counter++;
                }
            }

            for (int i = MdiChildren.Length - 1; i >= 0; i--)
            {
                if (null != MdiChildren[i] && MdiChildren[i].Visible)
                {
                    HideWindow(MdiChildren[i]);
                }
            }
            return true;
        }

        private void HideWindow(Form f)
        {
            if (f.InvokeRequired)
            {
                f.Invoke(new Action<Form>(HideWindow), f);
            }
            else
            {
                RemoveFromActiveTimes(f);
                f.Close();
            }
        }

        public void HideUndockWindow(Form f)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Form>(HideUndockWindow), f);
            }
            else
            {
                try
                {
                    RemoveFromActiveTimes(f);
                    f.Close();
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
            }
        }

        public void HideExternWindows()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(HideExternWindows));
            }
            else
            {
                try
                {
                    if (_alarmsDialog != null)
                    {
                        _alarmsDialog.Dispose();
                        _alarmsDialog = null;
                    }
                    if (_alarmDetails != null)
                    {
                        _alarmDetails.Dispose();
                        _alarmDetails = null;
                    }
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
            }
        }

        private bool _hidingWindows = false;
        public bool HidingWindows
        {
            get { return _hidingWindows; }
        }

        private void CloseAllWindows()
        {
            //if (!Dialog.Question(GetString("SureToCloseAll")))
            //    return false;
            //else
            var wasLoggedIn = CgpClient.Singleton.IsLoggedIn;

            if (CgpClient.Singleton.Unauthenticate())
                _forcedClose = true;
            else
                return;

            try
            {
                if (wasLoggedIn)
                {
                    Singleton.SaveOpenedWindows();
                }

                Form[] formsToClose = MdiChildren;
                foreach (Form f in formsToClose)
                {
                    if (null == f)
                        continue;

                    try
                    {
                        RemoveFromActiveTimes(f);
                        f.Close();
                    }
                    catch (Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
                    }
                }
                Singleton.Close();
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        private bool _isClosing;

        public bool IsClosing
        {
            get { return _isClosing; }
            set { _isClosing = value; }
        }

        private bool CloseAllWindowsWithCheck()
        {
            CloseAppDialog cDialog = new CloseAppDialog();
            byte r = cDialog.MyShowDialog(CgpClient.Singleton.IsLoggedIn);

            if (r == 0) return false;

            //Force Logout
            if (r == 1)
            {
                CgpClient.Singleton.ForceLogout();
            }
            else //close
            {
                var wasLoggedIn = CgpClient.Singleton.IsLoggedIn;

                if (CgpClient.Singleton.Unauthenticate())
                    _forcedClose = true;
                else
                    return false;

                try
                {
                    _isClosing = true;
                    if (wasLoggedIn)
                    {
                        Singleton.SaveOpenedWindows();

                        if (!CloseAllWithChecks())
                        {
                            _isClosing = false;
                            return false;
                        }
                    }

                    Singleton.Close();
                    return true;
                }
                catch
                {
                    _isClosing = false;
                }
            }
            return false;
        }

        public void CgpClientMainFormLoad()
        {
            // let the forms create now
// ReSharper disable RedundantAssignment
// ReSharper disable NotAccessedVariable
            Form f = PluginDashboardForm.Singleton;


            f = GeneralOptionsForm.Singleton;

            if (!GeneralOptions.Singleton.IsConfigured())
                LocalOptionsForm.Singleton.Show();

            f = PersonsForm.Singleton;
            f = LoginsForm.Singleton;
            f = EventlogsForm.Singleton;
            // ReSharper restore NotAccessedVariable
// ReSharper restore RedundantAssignment


            Text += " [ " + CgpClient.Singleton.Version + " ]";



            UpdateRemotingStatus(false);

            RefreshLeftMenuItems();

#if !DEBUG
            if (CgpClient.Singleton.Version.DevelopmentStage == DevelopmentStage.Testing ||
                CgpClient.Singleton.Version.DevelopmentStage == DevelopmentStage.Alpha ||
                CgpClient.Singleton.Version.DevelopmentStage == DevelopmentStage.Beta)
                Dialog.Error("Contal Nova Client - TESTING VERSION",
                    "THIS VERSION IS FOR TESTING PURPOSE ONLY !!!\n\nANY REDISTRIBUTION IS RESTRICTED !!!");
#endif

            _pleaseWaitMonitorSplash.Dispose();
            _pleaseWaitMonitorSplash = null;
        }


        void OnWindowNameSelectedVisualPlugin(object sender, EventArgs e)
        {
            ToolStripItem tsi = sender as ToolStripItem;

            if (!CgpClient.Singleton.IsConnectionLost(true))
            {
                if (tsi != null && tsi.Tag != null)
                {
                    var form = tsi.Tag as Form;

                    if (form != null)
                    {
                        form.Show();
                        form.BringToFront();
                    }
                }
            }
        }

        private void CgpClientMainForm_Shown(object sender, EventArgs e)
        {
            PluginDashboardForm.Singleton.BringToFront();
            _lvMenu.ForeColor = GetReferenceTextColor;
            _lvMenu.BackColor = GetReferenceBackgroundColor;
            _pbMinMaxLeftMenu.BackColor = GetReferenceBackgroundColor;
            _lvUnderMenu.ForeColor = GetReferenceTextColor;
            _lvUnderMenu.BackColor = GetReferenceBackgroundColor;
        }

        private readonly LinkedList<Form> _directChildForms = new LinkedList<Form>();
        protected internal void RegisterChildForm(Form form)
        {
            if (null == form)
                return;
            try
            {
                lock (_directChildForms)
                {
                    if (!_directChildForms.Contains(form))
                    {
                        _directChildForms.AddLast(form);
                        //StartChildFormListRefreshing();
                    }
                }
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        private bool _massRefreshSync = false;
        protected internal void StartChildFormListRefreshing()
        {
            this.BeginInvokeInUI(() =>
            {
                if (_massRefreshSync)
                    return;

                _massRefreshSync = true;

                // to withdraw the cumulated calls
                Thread.Sleep(200);

                RefreshChildFormList();
                _massRefreshSync = false;
            });
        }

        private void RefreshChildFormList()
        {

        }

        protected internal void RefreshPlugingsMenu(IEnumerable<ICgpVisualPlugin> plugins)
        {
            lock (_leftMenu)
            {
                //LinkedList<ToolStripItem> items2remove = new LinkedList<ToolStripItem>();

                //foreach (ToolStripItem tsi in _windowsMMI.DropDownItems)
                //{
                //    if (tsi.Tag != null &&
                //        tsi.Tag is PluginMainForm)
                //    {
                //        items2remove.AddLast(tsi);
                //    }
                //}

                //foreach (ToolStripItem tsi in items2remove)
                //{
                //    _windowsMMI.DropDownItems.Remove(tsi);
                //}

                /*
                foreach (Form f in this.MdiChildren)
                {
                    ToolStripItem tsi = new ToolStripMenuItem(f.Text);
                    tsi.Tag = f;
                    tsi.Click += new EventHandler(OnWindowNameSelected);
                    tsi.Image  = f.Icon.ToBitmap();
                    _windowsMMI.DropDownItems.Add(tsi);

                }*/

                int index = 0;
                //while (!(_leftMenu.Items[index] is ToolStripSeparator) && index < _leftMenu.Items.Count)  // Add plugin before Cgp
                while (_leftMenu.Items[index].Name != _logoutMmi.Name && index < _leftMenu.Items.Count)
                    index++;
                //Add NCAS before item logout
                if (index > 2)
                    index -= 2;

                foreach (ICgpVisualPlugin p in plugins)
                {
                    if (p.MainForm != null)
                    {
                        p.AddToOnEnter(ACgpVisualPlugin_Enter);
                        p.AddToOnShow(ACgpVisualPlugin_Show);
                        p.AddToOnClose(ACgpVisualPlugin_Close);
                        p.SetLanguage(LocalizationHelper.ActualLanguage);

                        //string formName = p.GetTranslateNameForm(p.MainForm);
                        //ToolStripItem tsi = new ToolStripMenuItem(formName);
                        //tsi.Name = p.MainForm.Name;
                        //tsi.Tag = p.MainForm;
                        //tsi.Click += new EventHandler(OnWindowNameSelectedVisualPlugin);
                        //tsi.Image = p.MainForm.Icon.ToBitmap();
                        //tsi.ImageAlign = ContentAlignment.MiddleLeft;
                        //tsi.TextAlign = ContentAlignment.MiddleLeft;
                        //tsi.Enabled = false;
                        //tsi.Name = formName;
                        ////_windowsMMI.DropDownItems.Add(tsi);

                        //_leftMenu.Items.Insert(index, tsi);
        
                        ToolStripSeparator tss = new ToolStripSeparator();
                        //index++;
                        _leftMenu.Items.Insert(index, tss);

                        //CheckAccess checkAccess = p.MainForm.GetCheckAccesControl();
                        //if (checkAccess != null && checkAccess.View)
                        //    tsi.Visible = true;
                        //else
                        //    tsi.Visible = false;
                    }

                    if (p.SubForms != null)
                    {
                        foreach (IPluginMainForm subForm in p.SubForms)
                        {
                            string formName = p.GetTranslateNameForm((Form)subForm);
                            ToolStripItem tsi = new ToolStripMenuItem(formName);
                            tsi.Name = subForm.Name;
                            tsi.Tag = subForm;
                            tsi.Click += OnWindowNameSelectedVisualPlugin;
                            tsi.Image = subForm.Icon.ToBitmap();
                            tsi.ImageAlign = ContentAlignment.MiddleLeft;
                            tsi.TextAlign = ContentAlignment.MiddleLeft;
                            tsi.Enabled = false;
                            //_windowsMMI.DropDownItems.Add(tsi);

                            index++;
                            _leftMenu.Items.Insert(index, tsi);

                            tsi.Visible = subForm.HasAccessView();
                        }
                        index++;
                    }
                }
            }
            RefreshLeftMenuItems();
        }

        private void ACgpVisualPlugin_Enter(ICgpClientPlugin sender, Form form)
        {
            if (sender == null)
                return;

            if (!(sender is ICgpVisualPlugin))
                return;

            ICgpVisualPlugin plugin = sender as ICgpVisualPlugin;

            if (form != null)
            {
                if (!SetActOpenWindow(form))
                {
                    AddToOpenWindows(form, plugin.GetTranslateNameForm(form), "", false);
                    SetActOpenWindow(form);
                }
            }
        }

        private void ACgpVisualPlugin_Show(ICgpClientPlugin sender, Form form)
        {
            if (sender == null)
                return;

            if (!(sender is ICgpVisualPlugin))
                return;

            ICgpVisualPlugin plugin = sender as ICgpVisualPlugin;

            if (form != null)
                AddToOpenWindows(form, plugin.GetTranslateNameForm(form), "", false);
        }

        private void ACgpVisualPlugin_Close(ICgpClientPlugin sender, Form form)
        {
            if (sender == null)
                return;

            if (!(sender is ICgpVisualPlugin))
                return;

            var plugin = sender as ICgpVisualPlugin;

            DebugHelper.Keep(plugin);

            if (form != null)
                RemoveFromOpenWindows(form);
        }

        private void TranslateCgpVisualPluginForms(IEnumerable<ICgpVisualPlugin> plugins)
        {
            foreach (ICgpVisualPlugin plugin in plugins)
            {
                plugin.SetLanguage(LocalizationHelper.ActualLanguage);
                if (plugin.MainForm != null)
                {
                    string formName = plugin.GetTranslateNameForm((Form)plugin.MainForm);
                    string subFormName;

                    foreach (ToolStripItem tsi in _leftMenu.Items)
                    {
                        if (tsi.Tag == plugin.MainForm)
                        {
                            tsi.Text = formName;
                        }
                        else
                        {
                            foreach (IPluginMainForm subForm in plugin.SubForms)
                            {
                                if (tsi.Tag == subForm)
                                {
                                    subFormName = plugin.GetTranslateNameForm((Form)subForm);
                                    tsi.Text = subFormName;
                                }
                            }
                        }

                    }

                    foreach (ToolStripItem tsi in _tsOpenWindows.Items)
                    {
                        if (tsi.Tag == plugin.MainForm)
                        {
                            tsi.Text = formName;
                        }
                        else
                        {
                            foreach (IPluginMainForm subForm in plugin.SubForms)
                            {
                                if (tsi.Tag == subForm)
                                {
                                    subFormName = plugin.GetTranslateNameForm((Form)subForm);
                                    tsi.Text = subFormName;
                                }
                            }
                        }
                    }

                    //foreach (ToolStripItem tsi in _tsRecentMenu.Items)
                    //{
                    //    if (tsi.Tag != null && (tsi.Tag as ToolStripItem).Tag != null)
                    //    {
                    //        ToolStripItem referencedItem = tsi.Tag as ToolStripItem;
                    //        if (referencedItem.Tag == plugin.MainForm)
                    //        {
                    //            tsi.Text = formName;
                    //        }
                    //        else
                    //        {
                    //            foreach (IPluginMainForm subForm in plugin.SubForms)
                    //            {
                    //                if (referencedItem.Tag == subForm)
                    //                {
                    //                    subFormName = plugin.GetTranslateNameForm((Form)subForm);
                    //                    tsi.Text = subFormName;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }

        public void UpdateRemotingStatus(bool connected)
        {
            UpdateRemotingStatus(
                connected,
                () => CgpClient.Singleton.LocalizationHelper.GetString("RemotingStatusDisconnected"));
        }

        public void UpdateRemotingStatusSqlServerDisconnected()
        {
            UpdateRemotingStatus(
                false,
                () => CgpClient.Singleton.LocalizationHelper.GetString("SqlServerDisconnected"));
        }

        private void UpdateRemotingStatus(
            bool connected, 
            Func<string> getDisconnectedMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<bool, Func<string>>(UpdateRemotingStatus), 
                    connected,
                    getDisconnectedMessage);

                return;
            }

            if (connected)
            {
                _remotingStatus.BackColor = Color.LightGreen;
                _remotingStatus.ForeColor = Color.DarkGreen;

                _remotingStatus.Text = string.Format("{0} ({1})", CgpClient.Singleton.LocalizationHelper.GetString("RemotingStatusConnected"), GeneralOptions.Singleton.ServerAddress);

                _IsRemotingOn = true;
            }
            else
            {
                _remotingStatus.BackColor = Color.Red;
                _remotingStatus.ForeColor = Color.White;

                _remotingStatus.Text = getDisconnectedMessage();

                _IsRemotingOn = false;

                HideAlarms();
            }
        }

        public void UpdateLoggedAs(string username)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new DString2Void(UpdateLoggedAs), 
                    username);

                return;
            }

            if (Validator.IsNotNullString(username))
            {
                _loggedAs.BackColor = Color.LightCyan;
                _loggedAs.ForeColor = Color.DarkBlue;
                _loggedAs.Text = username;

                ShowHome();
                ShowAlarms();

                _loginMmi.Visible = false;
                _logoutMmi.Visible = true;
                _changePwd.Visible = true;

                ChangePasswordIfNeeded();
                LoadAndShowOpenedWindows();

                //StartVisualSavingThread();
                ChangeAlarms();
                UpdateClientLanguage(username);

                _leftMenu.Focus();

                if (CgpClient.Singleton.MainServerProvider != null && 
                    CgpClient.Singleton.MainServerProvider.DemoLicence)
                {
                    GeneralOptionsForm.Singleton.SynchronizeRemainingTime();
                }
            }
            else
            {
                _loggedAs.BackColor = Color.Gray;
                _loggedAs.ForeColor = Color.Silver;

                _loggedAs.Text = CgpClient.Singleton.LocalizationHelper.GetString("UnauthenticatedStatus");

                LocalOptionsForm.Singleton.ShowCrLastCard(string.Empty);

                HideHome();
                HideAlarms();

                _loginMmi.Visible = true;
                _logoutMmi.Visible = false;
                _changePwd.Visible = false;
            }

            AlarmSound.Singleton.ChangedLogedUser(username);
        }

        private void UpdateClientLanguage(string username)
        {
            if (string.IsNullOrEmpty(username))
                return;

            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;
            
            string loginLanguage = CgpClient.Singleton.MainServerProvider.GetLoginLanguage();

            if (loginLanguage != string.Empty)
            {
                CgpClient.Singleton.LocalizationHelper.SetLanguage(loginLanguage);
                LocalOptionsForm.Singleton.UpdateSelectedLanguage(loginLanguage);
            }
            else
            {
                CgpClient.Singleton.MainServerProvider.SetLoginLanguage(CgpClient.Singleton.LocalizationHelper.ActualLanguage);
            }
        }

        private void ChangePasswordIfNeeded()
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (CgpClient.Singleton.MainServerProvider.Logins.IsPassOutOfDate() ||
                CgpClient.Singleton.MainServerProvider.Logins.MustChangePassword())
            {
                var passwordChange = new LoginPasswordChangeForm(CgpClient.Singleton.LoggedAs)
                {
                    CancelEnabled = false
                };
                passwordChange.ShowDialog();
            }
        }

        public static void SetVisualFormParameters(Form form, UserOpenedWindow openedWindow)
        {
            form.SuspendLayout();
            var cgpEditForm = form as ICgpEditForm;

            if (cgpEditForm != null)
                cgpEditForm.AllowEdit = openedWindow.AllowEdit;

            form.Dock = (DockStyle)(openedWindow.Docked);

            if (!openedWindow.HasParent)
            {
                if (cgpEditForm != null)
                {
                    cgpEditForm.UndockWindow();
                    cgpEditForm.ShowAndRunSetValues();
                }

                form.Left =
                    IsInHorizontalRange(openedWindow.PosLeft)
                        ? openedWindow.PosLeft
                        : 0;

                form.Top =
                    openedWindow.PosTop < Screen.FromControl(form).Bounds.Bottom
                        ? openedWindow.PosTop
                        : 0;
            }
            else
            {
                form.Left = openedWindow.PosLeft;
                form.Top = openedWindow.PosTop;
            }

            form.Width = openedWindow.Width;
            form.Height = openedWindow.Height;
            form.ResumeLayout();
        }

        private static bool IsInHorizontalRange(int x)
        {
            int maxX = Screen.AllScreens
                .Select(t => t.Bounds.Right)
                .Concat(new[] { 0 })
                .Max();

            return (x <= maxX);
        }

        public static void LoadAndShowOpenedForms(out Form selectedForm)
        {
            selectedForm = null;
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;
            ICollection<UserOpenedWindow> userOpenedWindows = CgpClient.Singleton.MainServerProvider.GetUserOpenedWindows();

            if (userOpenedWindows == null)
                return;

            foreach (UserOpenedWindow openedWindow in userOpenedWindows.OrderBy(x => x.WindowIndex))
            {
                try
                {
                    Form formToOpen = null;
                    bool isLocalisedObjectName = false;
                    //classic Forms
                    if (openedWindow.FormName == typeof(CalendarsForm).Name)
                    {
                        formToOpen = CalendarsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(AlarmsDialog).Name)
                    {
                        Singleton.ShowAlarmsDialog();
                        selectedForm = openedWindow.Selected ? Singleton._alarmsDialog : selectedForm;
                    }
                    else if (openedWindow.FormName == typeof(CardsForm).Name)
                    {
                        formToOpen = CardsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(CardTemplatesForm).Name)
                    {
                        formToOpen = CardTemplatesForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(CardSystemsForm).Name)
                    {
                        formToOpen = CardSystemsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(CisNGGroupsForm).Name)
                    {
                        formToOpen = CisNGGroupsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(CisNGsForm).Name)
                    {
                        formToOpen = CisNGsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(DailyPlansForm).Name)
                    {
                        formToOpen = DailyPlansForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(DayTypesForm).Name)
                    {
                        formToOpen = DayTypesForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(EventlogsForm).Name)
                    {
                        formToOpen = EventlogsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(FormLicenceRequest).Name)
                    {
                        formToOpen = new FormLicenceRequest();
                    }
                    else if (openedWindow.FormName == typeof(GeneralOptionsForm).Name)
                    {
                        formToOpen = GeneralOptionsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(LocalOptionsForm).Name)
                    {
                        formToOpen = LocalOptionsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(LoginPasswordForm).Name)
                    {
                        formToOpen = LoginPasswordForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(LoginGroupsForm).Name)
                    {
                        formToOpen = LoginGroupsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(LoginsForm).Name)
                    {
                        formToOpen = LoginsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(ObjectsSearchForm).Name)
                    {
                        formToOpen = ObjectsSearchForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(PersonsForm).Name)
                    {
                        formToOpen = PersonsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(PluginDashboardForm).Name)
                    {
                        formToOpen = PluginDashboardForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(PresentationFormattersForm).Name)
                    {
                        formToOpen = PresentationFormattersForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(PresentationGroupsForm).Name)
                    {
                        formToOpen = PresentationGroupsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(TimeZonesForm).Name)
                    {
                        formToOpen = TimeZonesForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(SystemEventsEditForm).Name)
                    {
                        formToOpen = SystemEventsEditForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(GlobalAlarmInstructionsForm).Name)
                    {
                        formToOpen = GlobalAlarmInstructionsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof (StructuredSiteForm).Name)
                    {
                        formToOpen = StructuredSiteForm.Singleton;
                    }


                        //edit Forms
                    else if (openedWindow.FormName == typeof(AlarmDetailsForm).Name)
                    {
                        Singleton.ShowAlarmDetail(openedWindow.ObjectId);
                    }
                    else
                    {
                        bool editAllowed;
                        AOrmObject objForEdit;
                        if (openedWindow.FormName == typeof(CalendarEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    isLocalisedObjectName = objForEdit.ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT;

                                    formToOpen = CalendarsForm.Singleton.OpenEditForm(objForEdit as Calendar, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new CalendarEditForm(new Calendar(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(CardsEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = CardsForm.Singleton.OpenEditForm(objForEdit as Card, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new CardsEditForm(new Card(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(CardSystemsEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = CardSystemsForm.Singleton.OpenEditForm(objForEdit as CardSystem, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new CardSystemsEditForm(new CardSystem(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(CisNGEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.CisNGs.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = CisNGsForm.Singleton.OpenEditForm(objForEdit as CisNG, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new CisNGEditForm(new CisNG(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(CisNGGroupEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.CisNGGroups.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = CisNGGroupsForm.Singleton.OpenEditForm(objForEdit as CisNGGroup, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new CisNGGroupEditForm(new CisNGGroup(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(DailyPlanEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = DailyPlansForm.Singleton.OpenEditForm(objForEdit as DailyPlan, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new DailyPlanEditForm(new DailyPlan(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(DayTypeEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = DayTypesForm.Singleton.OpenEditForm(objForEdit as DayType, editAllowed, false);
                                    isLocalisedObjectName = (objForEdit.ToString() == DayType.IMPLICIT_DAY_TYPE_HOLIDAY || objForEdit.ToString() == DayType.IMPLICIT_DAY_TYPE_VACATION);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new DayTypeEditForm(new DayType(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(EventlogParametersForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    long eventlogId;

                                    if (!Int64.TryParse(openedWindow.ObjectId, out eventlogId))
                                        continue;

                                    objForEdit =
                                        CgpClient.Singleton.MainServerProvider.Eventlogs
                                            .GetObjectById(eventlogId);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen = EventlogsForm.Singleton.OpenEditForm(objForEdit as Eventlog, false, false);
                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new EventlogParametersForm(new Eventlog());
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(LoginGroupEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectForEditById(openedWindow.ObjectId, out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = LoginGroupsForm.Singleton.OpenEditForm(objForEdit as LoginGroup, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new LoginGroupEditForm(new LoginGroup(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(LoginEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.Logins.GetObjectForEditById(openedWindow.ObjectId, out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = LoginsForm.Singleton.OpenEditForm(objForEdit as Login, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new LoginEditForm(new Login(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(LoginPasswordChangeForm).Name)
                        {
                            formToOpen =
                                new LoginPasswordChangeForm(CgpClient.Singleton.LoggedAs);
                        }
                        else if (openedWindow.FormName == typeof(PersonEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.Persons.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = PersonsForm.Singleton.OpenEditForm(objForEdit as Person, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new PersonEditForm(new Person(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(PresentationFormatterEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = PresentationGroupsForm.Singleton.OpenEditForm(objForEdit as PresentationGroup, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new PresentationFormatterEditForm(new PresentationFormatter(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(PresentationGroupEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = PresentationGroupsForm.Singleton.OpenEditForm(objForEdit as PresentationGroup, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new PresentationGroupEditForm(new PresentationGroup(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(TimeZonesEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = TimeZonesForm.Singleton.OpenEditForm(objForEdit as TimeZone, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new TimeZonesEditForm(new TimeZone(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(GlobalAlarmInstructionEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectForEditById(new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = GlobalAlarmInstructionsForm.Singleton.OpenEditForm(objForEdit as GlobalAlarmInstruction, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new GlobalAlarmInstructionEditForm(new GlobalAlarmInstruction(), (ShowOptionsEditForm)(openedWindow.ShowOption));
                                    break;
                            }
                        }
                    }

                    if (formToOpen == null)
                        continue;

                    var icgpTableForm = formToOpen as ICgpTableForm;
                    if (icgpTableForm != null && !icgpTableForm.HasAccessView())
                        continue;

                    if (openedWindow.HasParent == false)
                    {
                        DllUser32.SendMessage(
                            Singleton.Handle,
                            (uint)Singleton.WM_SETREDRAW,
                            0,
                            0);

                        SetVisualFormParameters(formToOpen, openedWindow);
                        selectedForm = openedWindow.Selected ? formToOpen : selectedForm;

                        DllUser32.SendMessage(
                            Singleton.Handle,
                            (uint)Singleton.WM_SETREDRAW,
                            1,
                            0);

                        Singleton.Refresh();
                    }
                    else
                    {
                        formToOpen.Text = CgpClient.Singleton.LocalizationHelper.GetString(formToOpen.Name + formToOpen.Name);

                        var cgpEditform = formToOpen as ICgpEditForm;

                        if (cgpEditform == null)
                            Singleton.AddToOpenWindows(
                                formToOpen);
                        else
                        {
                            object editingObject =
                                cgpEditform.GetEditingObject();

                            Singleton.AddToOpenWindows(
                                formToOpen,
                                editingObject != null
                                    ? editingObject.ToString()
                                    : "",
                                isLocalisedObjectName);
                        }

                        SetVisualFormParameters(formToOpen, openedWindow);
                        formToOpen.Visible = false;

                        selectedForm =
                            openedWindow.Selected
                                ? formToOpen
                                : selectedForm;
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Loads and shows saved opened windows for current login and select active window
        /// </summary>
        private void LoadAndShowOpenedWindows()
        {
            Form form = null;
            Form selectedForm = null;

            Form pluginForm = AllPluginsForm.Singleton;
            pluginForm.Text = LocalizationHelper.GetString(pluginForm.Name + pluginForm.Name);
            pluginForm.Visible = true;

            foreach (ICgpVisualPlugin visualPlugin in CgpClient.Singleton.PluginManager.GetVisualPlugins())
            {
                try
                {
                    visualPlugin.LoadAndShowOpenedForms(out form);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }

            }
            if (form != null)
            {
                selectedForm = form;
                StopAutocloseTimer(selectedForm);
            }
            try
            {
                LoadAndShowOpenedForms(out form);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }

            try
            {
                ShowSelectedForm(selectedForm, form, pluginForm);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }

        }

        private void ShowSelectedForm(Form selectedForm, Form form, Form pluginForm)
        {
            if (form != null)
            {
                selectedForm = form;
            }

            if (selectedForm != null)
            {
                if (selectedForm.Visible)
                {
                    selectedForm.Focus();
                }
                else
                {
                    selectedForm.Show();
                }
                StopAutocloseTimer(selectedForm);
            }
            else
            {
                selectedForm = pluginForm;
                selectedForm.Show();
            }
        }

        public void UpdateTsAlarms(bool isConnected)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DBool2Void(UpdateTsAlarms), isConnected);
                return;
            }

            _tsAlarms.Width = 190;

            if (isConnected)
            {
                ChangeAlarms();
            }
            else
            {
                _tsAlarms.BackColor = Color.LightGray;
                _tsAlarms.ForeColor = Color.Black;
            }
        }

        private void _generalOptionsMi_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                GeneralOptionsForm.Singleton.Show();
            }
        }

        private void _ormObjectSearch_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                ObjectsSearchForm.Singleton.Show();
            }
        }

        private void _personsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                PersonsForm.Singleton.Show();
        }

        private void _loginGroupsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                LoginGroupsForm.Singleton.Show();
        }

        private void _loginsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                LoginsForm.Singleton.Show();
        }

        private void _presentationGroupsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                PresentationGroupsForm.Singleton.Show();
        }

        private void _presentationFormatterForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                PresentationFormattersForm.Singleton.Show();
        }

        private void _eventlogsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                EventlogsForm.Singleton.Show();
        }

        private void SystemEventEditFormClick(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                SystemEventsEditForm.Singleton.Show();
        }

        private void CisNgFormClick(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CisNGsForm.Singleton.Show();
        }

        private void _cisNgGroupForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CisNGGroupsForm.Singleton.Show();
        }

        private void _cardSystemsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CardSystemsForm.Singleton.Show();
        }

        private void _cardsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CardsForm.Singleton.Show();
        }

        private void _cardTemplatesForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CardTemplatesForm.Singleton.Show();
        }

        private void _carsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CarsForm.Singleton.Show();
        }

        private void _dailyPlanForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                DailyPlansForm.Singleton.Show();
        }

        private void _TimeZonesForm_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CgpClient.Singleton.IsConnectionLost(false))
                    TimeZonesForm.Singleton.Show();
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _dayTypeForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                DayTypesForm.Singleton.Show();
        }

        private void _calendarForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CalendarsForm.Singleton.Show();
        }

        private void _globalAlarmInstructionsForm_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                GlobalAlarmInstructionsForm.Singleton.Show();
        }

        private void _about_Click(object sender, EventArgs e)
        {
            AboutForm.Singleton.Show();
        }

        private void _closeMi_Click(object sender, EventArgs e)
        {
            CloseAllWindowsWithCheck();
        }

        private bool _forcedClose = false;
        private void CgpClientMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forcedClose)
            {
                e.Cancel = false;
            }
            else if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                _isClosing = true;
                CloseAllWindows();
            }
            else if (e.CloseReason == CloseReason.UserClosing)
            {
                {
                    e.Cancel = !CloseAllWindowsWithCheck();
                }
            }

            if (!e.Cancel)
            {
                ColorSettingsChangedHandler.Singleton.UnregisterColorChanged(_eventColorChanged);
                DatabaseBackupSettingsChangedHandler.Singleton.UnregisterDatabaseBackupSettingsChanged(_eventDatabaseBackupSettingsChanged);
                DatabaseExpirationEventlogSettingsChangedHandler.Singleton.UnregisterDatabaseExpirationEventlogSettingsChanged(_eventDatabaseExpirationEventlogSettingsChanged);
                SqlServerOnlineStateChangedHandler.Singleton.UnregisterSqlServerOnlineStateChanged(_eventSqlServerOnlineStateChanged);
                CustomerAndSupplierInfoChangedHandler.Singleton.UnregisterInfoChanged(_eventCustomerAndSupplierInfoChanged);
            }
        }

        private readonly List<Form> _runningProgressFroms = new List<Form>();

        public void StartProgress(Form form)
        {
            ShowFormLoadProgress(form);
        }

        public void StopProgress(Form form)
        {
            HideFormLoadProgress(form);
        }

        private bool _canResize = true;

        private void OnNewLeftMenuResize(object sender, EventArgs e)
        {
            if (!_canResize || _textBoxFilter.Text.Length > 1)
                return;

            if (_leftMenu.Width + 4 > _splitContainerLeftMenu.Width)
            {
                _splitContainerLeftMenu.Width = _leftMenu.Width + 4;
                //_anyProgress.Width = _leftMenu.Width + 1;               
            }

            if (_leftMenu.Items.Count == 0)
            {
                _pLeftMenuToolStrip.Top = 0;
                _bLeftMenuDown.Enabled = false;
                _bLeftMenuUp.Enabled = false;
            }
            else
            {
                //+ 25 prevent upbutton click after down button click/
                while (_leftMenu.PreferredSize.Height + _pLeftMenuToolStrip.Top + 25 < _pLeftMenuUnderToolStrip.Height && _pLeftMenuToolStrip.Top < 0)
                {
                    _bLeftMenuUp_Click(sender, e);
                }
                //_bLeftMenuDown enabling conditions
                if (_leftMenu.PreferredSize.Height + _pLeftMenuToolStrip.Top <= _pLeftMenuUnderToolStrip.Height && _leftMenu.Items.Count > 0)
                {
                    _bLeftMenuDown.Enabled = false;
                }
                else
                {
                    _bLeftMenuDown.Enabled = true;
                }
            }
        }

        private void _goHome_Click(object sender, EventArgs e)
        {
            LoadPluginsForm();
        }
        public void LoadPluginsForm()
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                AllPluginsForm.Singleton.Show();
            }
        }

        private void _loginPasswordForm_Click(object sender, EventArgs e)
        {
            ShowLoginPasswordDialog();
        }

        private void _logoutMmi_Click(object sender, EventArgs e)
        {
            LogoutWithDialog();
        }

        private void InitLeftMenu()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(InitLeftMenu));
            }
            else
            {
                _recObjList = new RecentObjectList(_lvMenu);
                _recObjList.RefreshListView();
            }
        }

        public void ClearRecentList()
        {
            if (_recObjList != null)
                _recObjList.ClearRecentList();
        }

        public void AddToRecentList(object inputData)
        {
            _recObjList.Add(inputData, true);
        }

        public void DeleteFromRecentList(object inObject)
        {
            _recObjList.Delete(inObject);
        }

        public void AddToRecentList(object inObj, Form form, bool editEnable)
        {
            _recObjList.Add(inObj, form, editEnable);
        }

        public int WM_SETREDRAW = 11;

        private Bitmap GetOpenWindowImage(Form form)
        {
            if (form is LocalOptionsForm || CgpClient.Singleton.IsAuthenticated())
            {
                try
                {
                    Bitmap formBitmap = new Bitmap(form.Width, form.Height);

                    if (form.Visible == false)
                    {
                        DllUser32.SendMessage(Handle, (uint)WM_SETREDRAW, 0, 0);
                        form.DrawToBitmap(formBitmap, new Rectangle(0, 0, form.Width, form.Height));
                        form.Visible = false;
                        DllUser32.SendMessage(Handle, (uint)WM_SETREDRAW, 1, 0);
                        Refresh();
                    }
                    else
                    {
                        form.DrawToBitmap(formBitmap, new Rectangle(0, 0, form.Width, form.Height));
                    }

                    return formBitmap;
                }
                catch
                {
                    DllUser32.SendMessage(Handle, (uint)WM_SETREDRAW, 1, 0);
                }
            }

            return null;
        }

        public void AddToOpenWindows(Form form)
        {
            AddToOpenWindows(form, "");
        }

        public void AddToOpenWindows(Form form, string objectName)
        {
            AddToOpenWindows(form, form.Text, objectName, false);
        }

        public void AddToOpenWindows(Form form, string objectName, bool objectNameLocalised)
        {
            AddToOpenWindows(form, form.Text, objectName, objectNameLocalised);
        }

        private void AddToOpenWindows(Form form, string formName, string objectName, bool objectNameLocalised)
        {
            if (!(form is AllPluginsForm))
            {
                ToolStripItem tsi = null;
                foreach (ToolStripItem actTsi in _tsOpenWindows.Items)
                {
                    if (actTsi.Tag == form)
                        tsi = actTsi;
                }

                if (tsi == null)
                {
                    if (!string.IsNullOrEmpty(objectName))
                        if (objectNameLocalised)
                            formName += ": " + LocalizationHelper.GetString(objectName);
                        else
                            formName += ": " + objectName;

                    formName = TruncateString(formName, MAX_STRING_LENGTH);

                    Image image = form.Icon.ToBitmap();

                    if (form is PersonEditForm || form is PersonsForm)
                        image = ResourceGlobal.Persons16;
                    else if (form is LoginGroupEditForm || form is LoginGroupsForm)
                        image = ResourceGlobal.LoginGroup16;
                    else if (form is LoginEditForm || form is LoginsForm)
                        image = ResourceGlobal.Logins16;
                    else if (form is PresentationFormattersForm || form is PresentationFormatterEditForm)
                        image = ResourceGlobal.Formater16;
                    else if (form is PresentationGroupEditForm || form is PresentationGroupsForm)
                        image = ResourceGlobal.PresentationGroup16;
                    else if (form is GeneralOptionsForm)
                        image = ResourceGlobal.GeneralOptions16;
                    else if (form is LocalOptionsForm)
                        image = ResourceGlobal.LocalSettings16;
                    else if (form is EventlogsForm || form is EventlogParametersForm)
                        image = ResourceGlobal.EventLogsNew16;
                    else if (form is SystemEventsEditForm)
                        image = ResourceGlobal.SystemEvents16;
                    else if (form is CardsForm || form is CardsEditForm)
                        image = ResourceGlobal.IconCardsNew16.ToBitmap();
                    else if (form is CardTemplatesForm)
                        image = ResourceGlobal.IconCardTemplate16.ToBitmap();
                    else if (form is CardSystemsForm || form is CardSystemsEditForm)
                        image = ResourceGlobal.IconCardSystemNew16.ToBitmap();
                    else if (form is CisNGsForm || form is CisNGEditForm)
                        image = ResourceGlobal.CisNG16;
                    else if (form is CisNGGroupsForm || form is CisNGGroupEditForm)
                        image = ResourceGlobal.CisNGGrpup16;
                    else if (form is DailyPlansForm || form is DailyPlanEditForm)
                        image = ResourceGlobal.DailyPLan16;
                    else if (form is TimeZonesForm || form is TimeZonesEditForm)
                        image = ResourceGlobal.TimeZone16;
                    else if (form is CalendarEditForm || form is CalendarsForm)
                        image = ResourceGlobal.Calendar16;
                    else if (form is DayTypeEditForm || form is DayTypesForm)
                        image = ResourceGlobal.DayType16;
                    else if (form is GlobalAlarmInstructionEditForm || form is GlobalAlarmInstructionsForm)
                        image = ResourceGlobal.IconAlarmInstructions16.ToBitmap();

                    tsi = new CgpTabToolStripItemButton(formName, image, _recObjList.CloseImage, _recObjList.CloseImageOn)
                    {
                        Name = form.Name,
                        Tag = form,
                        Padding = new Padding(2, 2, 40, 2),
                    };
                    tsi.Click += _tsOpenWindowsItem_Click;
                    tsi.MouseEnter += _tsOpenWindowsItem_MouseEnter;
                    tsi.MouseLeave += _tsOpenWindowsItem_MouseLeave;
                    tsi.MouseDown += _tsOpenWindowsItem_MouseDown;
                    // Click the Close image on ToolStripItem
                    (tsi as CgpTabToolStripItemButton).CloseImageClick += _miClose_Click;

                    _tsOpenWindows.Items.Add(tsi);
                    if (GetAutoCloseTurnedOn)
                    {
                        if (!form.Visible)
                        {
                            //initial loaded form without show and enter events.
                            StartAutoCloseTimer(form);
                        }
                    }
                }

                _bOpenWindowsRight.Enabled = _tsOpenWindows.Width + _tsOpenWindows.Left > _pDockOpenWindows.Width;
            }
        }

        public void RemoveFromOpenWindows(Form form)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Form>(RemoveFromOpenWindows), form);
            }
            else
            {
                ToolStripItem tsi = null;
                foreach (ToolStripItem actTsi in _tsOpenWindows.Items)
                {
                    if (actTsi.Tag == form)
                    {
                        tsi = actTsi;
                        break;
                    }
                }

                if (tsi != null)
                {
                    _tsOpenWindows.Items.Remove(tsi);
                    ChangePosTSOpenWindows();
                }
            }
        }

        private MdiChildForm _focusedWindow;

        public MdiChildForm FocusedWindow{ get { return _focusedWindow; } }

        public bool SetActOpenWindow(Form form)
        {
            ToolStripItem tsiWindow = null;
            foreach (ToolStripItem tsiItem in _tsOpenWindows.Items)
            {
                if (tsiItem.Tag == form)
                {
                    tsiWindow = tsiItem;
                    break;
                }
            }

            if (tsiWindow != null)
            {
                try
                {
                    _focusedWindow = tsiWindow.Tag as MdiChildForm;
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
                
                if (GetAutoCloseTurnedOn)
                {
                    if (_tsiSelectedWindow != null)
                    {
                        StartAutoCloseTimer(_tsiSelectedWindow.Tag as Form);
                        _tsiSelectedWindow = tsiWindow;
                        StopAutocloseTimer(_tsiSelectedWindow.Tag as Form);//stop active window timer 
                    }
                    else
                    {
                        // Select window
                        _tsiSelectedWindow = tsiWindow;
                    }
                }

                Font font = new Font(tsiWindow.Font, FontStyle.Regular);
                foreach (ToolStripItem tsiItem in _tsOpenWindows.Items)
                {              
                    tsiItem.BackColor = _tsOpenWindows.BackColor;
                    tsiItem.Font = font;
                }

                // Set this Item as active
                tsiWindow.BackColor = Color.LightBlue;
                font = new Font(tsiWindow.Font, FontStyle.Bold);
                tsiWindow.Font = font;

                // Scroll the _tsOpenWindows to the Left or Right so the Item becomes visible              
                int rightPos = (tsiWindow.Bounds.Right - (_tsOpenWindows.Bounds.Left * -1) + _bOpenWindowsRight.Width) - _pDockOpenWindows.Bounds.Right;
                if (rightPos > 0)
                {
                    int count = rightPos / SCROLLSTEPOW;
                    count += (rightPos % SCROLLSTEPOW) > 0 ? 1 : 0;

                    for (int i = 0; i < count; i++)
                        _bOpenWindowsRight_Click(null, null);
                }
                else
                {
                    if ((_tsOpenWindows.Bounds.Left + _bOpenWindowsLeft.Width) < 0)
                    {
                        int leftPos = ((_tsOpenWindows.Bounds.Left * -1) + _bOpenWindowsLeft.Width) - tsiWindow.Bounds.Left;
                        int count = leftPos / SCROLLSTEPOW;
                        count += (leftPos % SCROLLSTEPOW) > 0 ? 1 : 0;

                        for (int i = 0; i < count; i++)
                            _bOpenWindowsLeft_Click(null, null);
                    }
                }

                return true;
            }
            
            _focusedWindow = null;
            return false;
            
        }

        public void SetTextOpenWindow(Form form, bool changed = false)
        {
            SetTextOpenWindow(form, false, changed);
        }

        public void SetTextOpenWindow(Form form, bool localised, bool changed)
        {
            ToolStripItem tsiWindow = null;
            foreach (ToolStripItem tsiItem in _tsOpenWindows.Items)
            {
                if (tsiItem.Tag == form)
                {
                    tsiWindow = tsiItem;
                    break;
                }
            }

            if (tsiWindow != null)
            {
                if (tsiWindow.Tag is ICgpEditForm)
                {
                    var cgpEditForm = form as ICgpEditForm;

                    var editingObject = cgpEditForm != null ? cgpEditForm.GetEditingObject() : null;
                    if (editingObject == null)
                        tsiWindow.Text = form.Text;
                    else
                    {
                        if (localised)
                            tsiWindow.Text = form.Text + ": " + LocalizationHelper.GetString(editingObject.ToString());
                        else
                            tsiWindow.Text = form.Text + ": " + editingObject;

                        if (changed)
                            tsiWindow.Text = "● " + tsiWindow.Text;

                        tsiWindow.Text = TruncateString(tsiWindow.Text, MAX_STRING_LENGTH);
                    }
                }
            }
        }

        private void _tsOpenWindowsItem_Click(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripItem;
            if (tsi == null)
                return;

            if (tsi.Name == LocalOptionsForm.Singleton.Name 
                || !CgpClient.Singleton.IsConnectionLost(true))
            {
                var tsiTag = tsi.Tag as Form;

                if (tsiTag != null)
                {
                    // Open appropriate table Form for this form (e.g. Persons for Person etc.)
                    OpenTableFormForEditedObject(tsiTag);

                    try
                    {
                        if (tsiTag.Visible == false)
                        {
                            tsiTag.Show();
                        }
                        tsiTag.BringToFront();
                    }
                    catch
                    {
                        RemoveFromOpenWindows(tsi.Tag as Form);
                    }
                }

                HideFormImage();
            }
        }

        private ToolStripItem _actTsiTsOpenWindows = null;
        private void _tsOpenWindowsItem_MouseDown(object sender, MouseEventArgs e)
        {
            _actTsiTsOpenWindows = sender as ToolStripItem;
        }

        public void StartAutoCloseTimer(Form form)
        {
            if (!(form is AlarmsDialog || form is EventlogsForm || form is StructuredSiteForm))
            {
                var tc = _autocloseTimerManager.StartTimeout(GetAutoCloseTimeout * 60000, form, AutoCloseTick);
                if (_formsActiveTimes.Keys.Contains(form))
                {
                    _formsActiveTimes[form] = tc;
                }
                else
                {
                    _formsActiveTimes.Add(form, tc);
                }
            }
        }

        public void StopAutocloseTimer(Form form)
        {
            if (_formsActiveTimes == null || _formsActiveTimes.Keys == null)
            {
                return;
            }
            if (_formsActiveTimes.Keys.Contains(form))
            {
                if (_formsActiveTimes[form] != null)
                {
                    _formsActiveTimes[form].StopTimer();
                    //_formsActiveTimes[form] = null;
                }
            }
        }

        private void RemoveFromActiveTimes(Form form)
        {
            try
            {
                if (_formsActiveTimes.Keys.Contains(form))
                {
                    _formsActiveTimes[form].StopTimer();
                    _formsActiveTimes.Remove(form);
                }
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        private bool AutoCloseTick(TimerCarrier timerCarrier)
        {
            Form form = timerCarrier.Data as Form;

            if (form != null)
                form.BeginInvokeInUI(() =>
                {
                    _formsActiveTimes.Remove(form);
                    if (form.Visible)
                    {
                        if ((!CheckEditValuesChanged(form)) && form.FormBorderStyle != FormBorderStyle.Sizable)
                        {
                            form.Close();
                        }
                    }
                    else
                    {
                        RemoveFromOpenWindows(form);
                    }
                });
            return true;
        }

        public void StopAutoCloseManager()
        {
            _autocloseTimerManager.StopAll();
            _formsActiveTimes.Clear();
            _tsiSelectedWindow = null;
        }


        private const int SCROLLSTEPOW = 20;
        private void _tsOpenWindows_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Right && _tsOpenWindows.Width + _tsOpenWindows.Left > _pDockOpenWindows.Width)
                _tsOpenWindows.Left -= SCROLLSTEPOW;

            if (e.KeyCode == Keys.Left && _tsOpenWindows.Left < 0)
                _tsOpenWindows.Left += SCROLLSTEPOW;

            UpdateOpenWindowsButtons();
            e.IsInputKey = true;
        }

        bool _mouseOverOpenWindows = false;
        private void _tsOpenWindows_MouseEnter(object sender, EventArgs e)
        {
            _mouseOverOpenWindows = true;
        }

        private void ChangePosTSOpenWindows()
        {
            if (_tsOpenWindows.Left < 0 && _tsOpenWindows.Width + _tsOpenWindows.Left < _pOpenWindows.Width)
            {
                int left = _pOpenWindows.Width - _tsOpenWindows.Width;

                _tsOpenWindows.Left = left < 0 ? left : 0;
            }
            UpdateOpenWindowsButtons();
        }

        private ITimer _showOWITimer = null;
        private const int SHOWOWIDELEAY = 1000;
        private void _tsOpenWindowsItem_MouseEnter(object sender, EventArgs e)
        {
            ToolStripItem tsi = sender as ToolStripItem;

            if (_showOWITimer != null)
            {
                _showOWITimer.StopTimer();
                _showOWITimer = null;
            }

            _showOWITimer = TimerManager.Static.StartTimer(SHOWOWIDELEAY, false, OnShowOWITimer, tsi);

            //_tsOpenWindows.Focus();
        }

        private bool OnShowOWITimer(TimerCarrier timerCarrier)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Func<TimerCarrier, bool>(OnShowOWITimer), timerCarrier);
            }
            else
            {
                var tsi = timerCarrier.Data as ToolStripItem;

                if (tsi !=null && tsi.Tag != null && tsi.Visible)
                {
                    try
                    {
                        Image formImage = GetOpenWindowImage(tsi.Tag as Form);
                        if (formImage != null)
                        {
                            _pbFormImage.Image = formImage;
                            float scale = _pbFormImage.Height / (float)formImage.Height;
                            _pbFormImage.Width = (int)(scale * formImage.Width);
                            _pbFormImage.Left = _pOpenWindows.Left + _tsOpenWindows.Left + tsi.Bounds.Left;
                            _pbFormImage.Top = _pOpenWindows.Top + _tsOpenWindows.Top + tsi.Bounds.Bottom;
                            _pbFormImage.Visible = true;
                        }
                    }
                    catch (Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
                    }
                }
            }
            return false;
        }

        private void _tsOpenWindowsItem_MouseLeave(object sender, EventArgs e)
        {
            if (_showOWITimer != null)
            {
                _showOWITimer.StopTimer();
                _showOWITimer = null;
            }

            HideFormImage();
        }

        private void _miClose_Click(object sender, EventArgs e)
        {
            if (e != null && e is MouseEventArgs && sender != null)
            {
                // Window is closed directly by clicking the Close image
                _actTsiTsOpenWindows = sender as CgpTabToolStripItemButton;
            }

            if (_actTsiTsOpenWindows != null)
            {
                if (_actTsiTsOpenWindows.Tag != null)
                {
                    try
                    {
                        var form = _actTsiTsOpenWindows.Tag as Form;
                        if (form != null)
                        {
                            RemoveFromActiveTimes(form);
                            if (form.Visible)
                            {
                                CheckAfterCancel(form);
                            }
                            else
                            {
                                RemoveFromOpenWindows(form);
                            }
                        }
                        _actTsiTsOpenWindows = null;
                    }
                    catch (Exception ex)
                    {
                        HandledExceptionAdapter.Examine(ex);
                    }
                }
            }
        }

        private void _closeAllButThis_Click(object sender, EventArgs e)
        {
            bool closeAllFailed = false;
            if (_actTsiTsOpenWindows != null)
            {
                if (_actTsiTsOpenWindows.Tag != null)
                {
                    int i = 0;
                    while (_tsOpenWindows.Items.Count > 1 && !closeAllFailed)
                    {
                        if (_tsOpenWindows.Items[i].Tag != _actTsiTsOpenWindows.Tag)
                        {
                            RemoveFromActiveTimes(_tsOpenWindows.Items[i].Tag as Form);

                            var form = (_tsOpenWindows.Items[i].Tag as Form);
                            if (form != null && form.Visible)
                            {
                                if (!CheckAfterCancel(form))
                                {
                                    closeAllFailed = true;
                                }
                            }
                            else
                            {
                                RemoveFromOpenWindows(_tsOpenWindows.Items[i].Tag as Form);
                            }
                        }
                        else
                        {
                            i = 1; //_tsOpenWindows.Items[0] is now window we dont want to close!
                        }
                    }
                    ChangePosTSOpenWindows();
                    if (!closeAllFailed)
                    {
                        var form = (_actTsiTsOpenWindows.Tag as Form);

                        if (form != null)
                        {
                            if (form.Visible == false)
                            {
                                form.Show();
                            }
                            else
                            {
                                form.BringToFront();
                            }
                        }
                    }

                }
            }
        }

        private bool CheckAfterCancel(Form form)
        {
            var editFormBase = form as IEditFormBase;

            if (editFormBase != null)
                return editFormBase.SaveAfterCancel();

            RemoveFromActiveTimes(form);
            form.Close();
            return true;
        }

        private static bool CheckEditValuesChanged(Form form)
        {
            var cgpEditForm = form as ICgpEditForm;

            return 
                cgpEditForm != null && 
                cgpEditForm.ValueChanged;
        }

        private void CgpClientMainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                ChangePosTSOpenWindows();
                UpdateLeftMenuSplitContainer();
            }
        }

        private void _changePwd_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsLoggedIn) return;
            if (string.IsNullOrEmpty(CgpClient.Singleton.LoggedAs)) return;
            //LoginPasswordChangeForm.Singleton.ShowLoginPasswordChangeDialog(CgpClient.Singleton.LoggedAs);
            LoginPasswordChangeForm changeLogin = new LoginPasswordChangeForm(CgpClient.Singleton.LoggedAs);
            changeLogin.ShowDialog();
        }

        private void UpdateLeftMenuSplitContainer()
        {
            if (_splitContainerLeftMenu.SplitterDistance - (_splitContainerLeftUpperMenu.Panel2MinSize + _splitContainerLeftMenu.SplitterWidth)
                <= _splitContainerLeftUpperMenu.SplitterDistance)
            {
                _splitContainerLeftMenu.SplitterDistance = _splitContainerLeftUpperMenu.SplitterDistance
                    + _splitContainerLeftUpperMenu.Panel2MinSize + _splitContainerLeftMenu.SplitterWidth;
            }
        }


        private void _bLeftMenuUp_Click(object sender, EventArgs e)
        {
            if (_pLeftMenuToolStrip.Top < 0 && _leftMenu.Items.Count > 0)
            {
                int index = 0;
                int itemsHeight = 0;
                while (_pLeftMenuToolStrip.Top < itemsHeight && index < _leftMenu.Items.Count)
                {
                    itemsHeight -= _leftMenu.Items[index].Height + _leftMenu.Items[index].Margin.Bottom + _leftMenu.Margin.Top;
                    index++;
                }

                if (index > 0)
                    index--;

                if (_pLeftMenuToolStrip.Top > itemsHeight)
                {
                    int deltaTop = itemsHeight - _pLeftMenuToolStrip.Top;
                    _pLeftMenuToolStrip.Top += deltaTop;
                    _pLeftMenuToolStrip.Height -= deltaTop;
                }

                int delta = _leftMenu.Items[index].Height + _leftMenu.Items[index].Margin.Bottom + _leftMenu.Margin.Top;

                _pLeftMenuToolStrip.Top += delta;
                _pLeftMenuToolStrip.Height -= delta;

                if (_pLeftMenuToolStrip.Top >= 0)
                {
                    //_pLeftMenuToolStrip.Top = 0;
                    //_pLeftMenuToolStrip.Height = _baseHeigthLeftMenu;
                    _bLeftMenuUp.Enabled = false;
                }

                _bLeftMenuDown.Enabled = true;
            }
        }

        private void _bLeftMenuDown_Click(object sender, EventArgs e)
        {
            if (_leftMenu.PreferredSize.Height + _pLeftMenuToolStrip.Top > _pLeftMenuUnderToolStrip.Height && _leftMenu.Items.Count > 0)
            {
                int index = 0;
                int itemsHeight = 0;
                while (_pLeftMenuToolStrip.Top < itemsHeight && index < _leftMenu.Items.Count)
                {
                    itemsHeight -= _leftMenu.Items[index].Height + _leftMenu.Items[index].Margin.Bottom + _leftMenu.Margin.Top;
                    index++;
                }

                if (_pLeftMenuToolStrip.Top > itemsHeight)
                {
                    int deltaTop = itemsHeight - _pLeftMenuToolStrip.Top;
                    _pLeftMenuToolStrip.Top += deltaTop;
                    _pLeftMenuToolStrip.Height -= deltaTop;
                }

                int delta = _leftMenu.Items[index].Height + _leftMenu.Items[index].Margin.Bottom + _leftMenu.Margin.Top;
                _pLeftMenuToolStrip.Top -= delta;
                _pLeftMenuToolStrip.Height += delta;


                if (_leftMenu.PreferredSize.Height + _pLeftMenuToolStrip.Top <= _pLeftMenuUnderToolStrip.Height && _leftMenu.Items.Count > 0)
                {
                    _bLeftMenuDown.Enabled = false;
                }
                _bLeftMenuUp.Enabled = true;
            }
        }

        /*private void MouseScroll(object sender, MouseEventArgs e)
        {
            ScrollLeftMenu(e.Delta);
        }*/

        private void ScrollLeftMenu(int delta)
        {
            if (_mouseOverLeftMenu)
            {
                if (delta > 0)
                    _bLeftMenuUp_Click(null, null);

                if (delta < 0)
                    _bLeftMenuDown_Click(null, null);
            }
            if (_mouseOverOpenWindows)
            {
                if (delta > 0)
                    _bOpenWindowsLeft_Click(null, null);

                if (delta < 0)
                    _bOpenWindowsRight_Click(null, null);
            }
        }

        bool _mouseOverLeftMenu;

        private void _leftMenu_MouseEnter(object sender, EventArgs e)
        {
            _mouseOverLeftMenu = true;
            //_leftMenu.Focus();
        }

        private void _leftMenu_MouseLeave(object sender, EventArgs e)
        {
            _mouseOverLeftMenu = false;
        }
     
        /*const int WM_MOUSEWHEEL = 0x020A;

        protected override void WndProc(ref Message m)//AAA
        {
            base.WndProc(ref m);

            if (m.Msg == WM_MOUSEWHEEL
                && _mouseOverLeftMenu)
            {
                int Rotation = m.WParam.ToInt32() / 65536;
                int delta = Rotation / 120;
                ScrollLeftMenu(delta);
                Debug.WriteLine(delta);
            }
        }*/

        private void LeftMenuSetFocusAfterClick()
        {
            foreach (ToolStripItem tsi in _leftMenu.Items)
            {
                tsi.Click += _LeftMenuItem_AfterClick;
            }
        }

        private void _LeftMenuItem_AfterClick(object sender, EventArgs e)
        {
            _leftMenu.Focus();
            //(sender as ToolStripItem).Select();
        }

        private void _leftMenu_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            try
            {
                e.Item.Click += _LeftMenuItem_AfterClick;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        public void PluginProxyGained(ICgpClientPlugin plugin)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ICgpClientPlugin>(PluginProxyGained), 
                    plugin);

                return;
            }

            if (!(plugin is ICgpVisualPlugin))
                return;

            var visualPlugin = plugin as ICgpVisualPlugin;

            foreach (ToolStripItem tsi in _leftMenu.Items)
                if (tsi.Tag == visualPlugin.MainForm)
                    tsi.Enabled = true;
                else
                    foreach (IPluginMainForm subForm in visualPlugin.SubForms)
                        if (tsi.Tag == subForm)
                            tsi.Enabled = true;
        }

        public void PluginProxyLost(ICgpClientPlugin plugin)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ICgpClientPlugin>(PluginProxyLost), plugin);
                return;
            }

            if (!(plugin is ICgpVisualPlugin))
                return;

            var visualPlugin = plugin as ICgpVisualPlugin;

            RemoveFromActiveTimes((Form)visualPlugin.MainForm);
            visualPlugin.MainForm.Close();

            foreach (ToolStripItem tsi in _leftMenu.Items)
                if (tsi.Tag == visualPlugin.MainForm)
                    tsi.Enabled = false;
                else
                    foreach (IPluginMainForm subForm in visualPlugin.SubForms)
                        if (tsi.Tag == subForm)
                            tsi.Enabled = false;
        }

        public void RefreshLeftMenuItems()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(RefreshLeftMenuItems));
                return;
            }

            //ClearRecentMenu();
            //SetQuickMenuMinSize();
            if (CgpClient.Singleton.IsLoggedIn)
            {
                _ormObjectSearch.Visible =
                    CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.SearchView));
                
                _generalOptionsMi.Visible =
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccessesForGroup(LoginAccessGroups.GENERAL_OPTIONS));

                //UpdateVisualRecentMenuItems();
                _alarmsMI.Visible = true;
            }
            else
            {
                _ormObjectSearch.Visible = false;
                _generalOptionsMi.Visible = false;
                _alarmsMI.Visible = false;
            }

            _textBoxFilter.Clear();
            _leftMenuItems.Clear();
            //_leftMenu.Items[0].ToolTipText = 
            //    CgpClient.Singleton.LocalizationHelper.GetString("CgpClientMainForm_goHome").Replace(" ", "");

            _leftMenuItems.Add(_personsForm, PersonsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_loginGroupsForm, LoginGroupsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_loginsForm, LoginsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_presentationGroupsForm, PresentationGroupsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_presentationFormatterForm, PresentationFormattersForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_eventlogsForm, EventlogsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_systemEventForm, SystemEventsEditForm.HasAccessView());
            _leftMenuItems.Add(_cisNgForm, CisNGsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_cisNgGroupForm, CisNGGroupsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_cardSystemsForm, CardSystemsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_cardsForm, CardsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_cardTemplatesForm, CardTemplatesForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_dailyPlanForm, DailyPlansForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_TimeZonesForm, TimeZonesForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_dayTypeForm, DayTypesForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_calendarForm, CalendarsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_alarmsMI, AlarmsDialog.HasAccessView());
            _leftMenuItems.Add(_globalAlarmInstructionsForm, GlobalAlarmInstructionsForm.Singleton.HasAccessView());
            _leftMenuItems.Add(_structuredSiteForm, StructuredSiteForm.Singleton.HasAccessView());

            foreach (KeyValuePair<ToolStripButton, bool> kvp in _leftMenuItems)
            {
                kvp.Key.Visible = kvp.Value;
            }

            foreach (ToolStripItem toolStripItem in _leftMenu.Items)
            {
                var pluginMainForm = (toolStripItem.Tag as IPluginMainForm);
                if (pluginMainForm != null)
                {
                    toolStripItem.Visible = pluginMainForm.HasAccessView();
                }

                if (toolStripItem is ToolStripSeparator)
                {
                    toolStripItem.Visible = true;
                }
            }

            bool hiddenItem = false;
            foreach (ToolStripItem toolStripItem in _leftMenu.Items)
            {
                if (toolStripItem is ToolStripSeparator)
                {
                    if (hiddenItem)
                    {
                        toolStripItem.Visible = false;
                    }

                    hiddenItem = true;
                }
                else
                {
                    if (toolStripItem.Visible)
                    {
                        hiddenItem = false;
                    }
                }
            }
            OnNewLeftMenuResize(null, null);
            //OnLeftMenuResize(null, null);
        }

// ReSharper disable UnusedMember.Local
        private void SetQuickMenuMinSize()
// ReSharper restore UnusedMember.Local
        {
            _splitContainerLeftUpperMenu.Panel1MinSize = MAX_QUICK_BUTTONS * Convert.ToInt32(21 * _scaleF.Width);
            _splitContainerLeftMenu.Panel1MinSize = _splitContainerLeftUpperMenu.Panel1MinSize +
            _splitContainerLeftUpperMenu.Panel2MinSize + _splitContainerLeftMenu.SplitterWidth;
        }

        public bool MainIsConnectionLost(bool showDialog)
        {
            if (_forcedClose)
            {
                return true;
            }
            return CgpClient.Singleton.IsConnectionLost(showDialog);
        }

        private void _leftMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void _tsAlarms_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsLoggedIn)
            {

                //checkAccess = new CheckAccess(true, true);
                if (AlarmsDialog.HasAccessView())
                {
                    if (_alarmsDialog == null)
                    {
                        _alarmsDialog = new AlarmsDialog(_listAlarms);
                    }
                    _alarmsDialog.Show();
                }
            }
        }

        public void ShowAlarmsDialog()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowAlarmsDialog));
            }
            else
            {
                if (!CgpClient.Singleton.IsConnectionLost(false))
                {
                    Login login = CgpClient.Singleton.MainServerProvider.Logins.GetActualLogin();
                    if (login != null)
                    {
                        _alarmsDialog = new AlarmsDialog(_listAlarms);
                        _alarmsDialog.Text = CgpClient.Singleton.LocalizationHelper.GetString(_alarmsDialog.Name + _alarmsDialog.Name);
                        UserOpenedWindow openedWindowSettings = CgpClient.Singleton.GetOpenedWindowSettings(typeof(AlarmsDialog).Name, login);
                        if (openedWindowSettings != null)
                        {
                            if (openedWindowSettings.HasParent)
                                Singleton.AddToOpenWindows(_alarmsDialog);
                            else
                            {
                                _alarmsDialog.Show();
                                _alarmsDialog.DoDockUndock();
                            }
                            
                            SetVisualFormParameters(_alarmsDialog, openedWindowSettings);
                        }
                    }

                }
            }
        }

        public void ShowAlarmDetail(string alarmID)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DString2Void(ShowAlarmDetail), alarmID);
            }
            else
            {
                ServerAlarmCore currentAlarm = null;
                foreach (var alarm in _listAlarms)
                {
                    if (alarm.IdServerAlarm.ToString() == alarmID)
                    {
                        currentAlarm = alarm;
                    }
                }
                if (currentAlarm != null)
                {
                    Login login = CgpClient.Singleton.MainServerProvider.Logins.GetActualLogin();
                    if (login != null)
                    {
                        _alarmDetails = new AlarmDetailsForm(currentAlarm);
                        UserOpenedWindow windowSettings = CgpClient.Singleton.GetOpenedWindowSettings(typeof(AlarmDetailsForm).Name, login);
                        if (windowSettings != null)
                        {
                            _alarmDetails.Show();
                            SetVisualFormParameters(_alarmDetails, windowSettings);
                        }
                    }
                }
            }
        }

        public void AlarmsDialogClose()
        {
            _alarmsDialog = null;
        }

        public void AlarmsDetailDialogClose(AlarmDetailsForm alarmDetailsForm)
        {
            if (_alarmDetails == null)
                return;

            if (!_alarmDetails.Equals(alarmDetailsForm))
                return;

            _alarmDetails = null;
        }

        private readonly object _lockDoChangeAlarms = new object();
        private SafeThread _threadDoChangeAlarms = null;
        public void ChangeAlarms()
        {
            lock (_lockDoChangeAlarms)
            {
                if (_threadDoChangeAlarms == null)
                {
                    _threadDoChangeAlarms = new SafeThread(DoChangeAlarms);
                    _threadDoChangeAlarms.Start();
                }
                else
                {
                    _threadDoChangeAlarms.Resume();
                }
            }
        }

        public void DoChangeAlarms()
        {
            try
            {
                while (true)
                {
                    _threadDoChangeAlarms.WaitForResume();

                    lock (_lockDoChangeAlarms)
                    {
                        _threadDoChangeAlarms.Suspend();
                    }

                    if (!CgpClient.Singleton.IsMainServerProviderAvailable)
                    {
                        _listAlarms = null;
                    }
                    else
                    {
                        try
                        {
                            _listAlarms = CgpClient.Singleton.MainServerProvider.GetAlarms();
                        }
                        catch
                        {
                            _listAlarms = null;
                        }
                    }

                    ShowChangedAlarms(_listAlarms);

                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
            finally
            {
                _threadDoChangeAlarms = null;
            }
        }

        public void ShowChangedAlarms(ICollection<ServerAlarmCore> listAlarms)
        {
            if (!CgpClient.Singleton.IsLoggedIn)
                return;

            if (listAlarms == null || listAlarms.Count == 0)
            {
                this.InvokeInUI(() =>
                {
                    _tsAlarms.Width = 190;
                    _tsAlarms.Text = GetString("CgpClientMainForm_NoAlarms");
                    _tsAlarms.BackColor = GeneralOptionsForm.Singleton.GetNoAlarmsInQueueColorBackground;
                    _tsAlarms.ForeColor = GeneralOptionsForm.Singleton.GetNoAlarmsInQueueColorText;
                });
                AlarmSound.Singleton.PlayAlarmSound(new ActualAlarms());
            }
            else
            {
                bool bAlarmNotAck = false;
                bool bAlarm = false;
                bool bNormalNotAck = false;

                int allAlarms = 0;
                int notAcknowladge = 0;
                ActualAlarms playAlarms = new ActualAlarms();
                foreach (var serverAlarm in listAlarms)
                {
                    var alarm = serverAlarm.Alarm;

                    if (serverAlarm.IsBlocked && serverAlarm.IsAcknowledged)
                        continue;

                    if ((alarm.AlarmState != AlarmState.Normal || !serverAlarm.IsAcknowledged) && !serverAlarm.IsBlocked)
                    {
                        allAlarms++;
                    }

                    if (!serverAlarm.IsAcknowledged && !serverAlarm.IsBlocked)
                    {
                        playAlarms._notAck = true;
                        notAcknowladge++;
                    }

                    if (alarm.AlarmState == AlarmState.Alarm && !serverAlarm.IsAcknowledged)
                    {
                        if (serverAlarm.IsBlocked)
                        {
                            bNormalNotAck = true;
                        }
                        else
                        {
                            bAlarmNotAck = true;
                            playAlarms._alarmNotAck = true;
                        }
                    }

                    if (alarm.AlarmState == AlarmState.Alarm)
                    {
                        if (serverAlarm.IsBlocked)
                        {
                            bNormalNotAck = true;
                        }
                        else
                        {
                            bAlarm = true;
                            playAlarms._alarm = true;
                        }
                    }

                    if (alarm.AlarmState == AlarmState.Normal && !serverAlarm.IsAcknowledged)
                    {
                        bNormalNotAck = true;
                    }
                }

                AlarmSound.Singleton.PlayAlarmSound(playAlarms);

                this.InvokeInUI(()=>
                {
                    _tsAlarms.Width = 190;
                    _tsAlarms.Text = GetString("CgpClientMainForm_Alarms") + ": " + allAlarms;
                    if (notAcknowladge > 0)
                    {
                        _tsAlarms.Text += " (" + GetString("CgpClientMainForm_NotAcknowledged") + ": " + notAcknowladge + ")";
                    }

                    if (bAlarmNotAck)
                    {
                        _tsAlarms.BackColor = GeneralOptionsForm.Singleton.GetAlarmNotAcknowledgedColorBackground;
                        _tsAlarms.ForeColor = GeneralOptionsForm.Singleton.GetAlarmNotAcknowledgedColorText;
                    }
                    //Alarm.GetAlarmColor(AlarmState.AlarmNotAcknowledged);
                    else if (bAlarm)
                    {
                        _tsAlarms.BackColor = GeneralOptionsForm.Singleton.GetAlarmColorBackground;
                        _tsAlarms.ForeColor = GeneralOptionsForm.Singleton.GetAlarmColorText;
                    }
                    //Alarm.GetAlarmColor(AlarmState.Alarm);
                    else if (bNormalNotAck)
                    {
                        _tsAlarms.BackColor = GeneralOptionsForm.Singleton.GetNormalNotAcknowledgedColorBackground;
                        _tsAlarms.ForeColor = GeneralOptionsForm.Singleton.GetNormalNotAcknowledgedColorText;
                    }
                    //Alarm.GetAlarmColor(AlarmState.NormalNotAcknowledged);
                    else
                    {
                        _tsAlarms.BackColor = GeneralOptionsForm.Singleton.GetNoAlarmsInQueueColorBackground;
                        _tsAlarms.ForeColor = GeneralOptionsForm.Singleton.GetNoAlarmsInQueueColorText;
                    }
                    //Color.LightGray;
                });
            }

            if (_alarmsDialog != null)
            {
                _alarmsDialog.ShowAlarms(listAlarms, false);
            }

            if (AlarmChanged != null)
            {
                try
                {
                    AlarmChanged(listAlarms);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
            }
            AlarmTypeSound.Singleton.ProcessAllAlarmsFromServer(listAlarms);
        }

        public void DeletedAlarm(IdServerAlarm deleted)
        {
            if (AlarmDeleted != null)
            {
                try
                {
                    AlarmDeleted(deleted);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
            }
        }

        public event Action<ICollection<ServerAlarmCore>> AlarmChanged;
        public event Action<IdServerAlarm> AlarmDeleted;

        private readonly LinkedList<ToolStripItem> _hiddenItems = new LinkedList<ToolStripItem>();
        private void _textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            _canResize = false;
            _leftMenu.MinimumSize = new Size(_splitContainerLeftUpperMenu.Panel2.Width + _splitContainerLeftUpperMenu.SplitterWidth, 50);

            if (_textBoxFilter.Text != string.Empty)
            {
                string searchTerm = PersonsForm.RemoveDiacritism(_textBoxFilter.Text.Trim().ToLower());

                // Check visible buttons (if they don't match filter condition, they will become invisible)
                _pLeftMenuToolStrip.Top = 0;
                _bLeftMenuUp.Enabled = false;
                List<ToolStripItem> itemsToBeHidden = new List<ToolStripItem>();

                foreach (ToolStripItem item in _leftMenu.Items)
                {
                    if ((item.Visible) && (!PersonsForm.RemoveDiacritism(item.Text.Trim().ToLower()).Contains(searchTerm)))
                    {
                        item.Visible = false;
                        itemsToBeHidden.Add(item);
                    }
                }

                // Check invisible buttons from special collection (if they match filter condition, they will become visible)
                List<ToolStripItem> itemsToBeVisible = new List<ToolStripItem>();
                foreach (ToolStripItem item in _hiddenItems)
                {
                    if (PersonsForm.RemoveDiacritism(item.Text.Trim().ToLower()).Contains(searchTerm))
                    {
                        item.Visible = true;
                        itemsToBeVisible.Add(item);
                    }
                }

                // Add new invisible buttons into collection
                foreach (ToolStripItem item in itemsToBeHidden)
                {
                    _hiddenItems.AddLast(item);
                }
                // Remove old invisible buttons from collection
                foreach (ToolStripItem item in itemsToBeVisible)
                {
                    _hiddenItems.Remove(item);
                }
                ManageLeftMenuButtons();
            }
            else
            {
                foreach (ToolStripItem item in _hiddenItems)
                {
                    item.Visible = true;
                }
                _hiddenItems.Clear();
                ManageLeftMenuButtons();
            }
            _canResize = true;
        }

        private void _buttonClearFilter_Click(object sender, EventArgs e)
        {
            _textBoxFilter.Clear();
        }

        public void ClearLeftMenuFilter()
        {
            if (_textBoxFilter.InvokeRequired)
            {
                _textBoxFilter.BeginInvoke(new DVoid2Void(ClearLeftMenuFilter));
            }
            else
            {
                _textBoxFilter.Clear();
            }
        }

        private void _textBoxFilter_MouseEnter(object sender, EventArgs e)
        {
            _textBoxFilter.Focus();
        }

        private void ManageLeftMenuButtons()
        {
            //Enabling and disabling _bLeftMenuDown
            if (_leftMenu.Items.Count == 0)
            {
                _pLeftMenuToolStrip.Top = 0;
                _bLeftMenuDown.Enabled = false;
                _bLeftMenuUp.Enabled = false;
            }
            else
            {
                if (_leftMenu.PreferredSize.Height + _pLeftMenuToolStrip.Top <= _pLeftMenuUnderToolStrip.Height && _leftMenu.Items.Count > 0)
                {
                    _bLeftMenuDown.Enabled = false;
                }
                else
                {
                    _bLeftMenuDown.Enabled = true;
                }
            }
        }

        private void _bOpenWindowsLeft_Click(object sender, EventArgs e)
        {
            if (_tsOpenWindows.Left < 0)
                _tsOpenWindows.Left += SCROLLSTEPOW;
            UpdateOpenWindowsButtons();
        }

        private void _bOpenWindowsRight_Click(object sender, EventArgs e)
        {
            if (_tsOpenWindows.Width + _tsOpenWindows.Left > _pDockOpenWindows.Width)
                _tsOpenWindows.Left -= SCROLLSTEPOW;
            UpdateOpenWindowsButtons();
        }

        private void UpdateOpenWindowsButtons()
        {
            _bOpenWindowsLeft.Enabled = _tsOpenWindows.Left < 0;

            _bOpenWindowsRight.Enabled = _tsOpenWindows.Width + _tsOpenWindows.Left > _pDockOpenWindows.Width;
        }

        private void _loggedAs_DoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsLoggedIn)
            {
                Login login = CgpClient.Singleton.MainServerProvider.Logins.GetActualLogin();
                if (CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.LoginsLoginGroupsAdmin)))
                {
                    if (!CgpClient.Singleton.IsConnectionLost(false))
                        LoginsForm.Singleton.OpenEditForm(login);
                }
                else
                {
                    LoginPasswordChangeForm changePass = new LoginPasswordChangeForm(CgpClient.Singleton.LoggedAs);
                    changePass.ShowDialog();
                }
            }
            //else
            //{
            //    CgpClient.Singleton.SetExpiredInfo(false);
            //    CgpClient.Singleton.ShowLoginPasswordDialog();
            //}
        }

        private void _tsLocalOptions_Click(object sender, EventArgs e)
        {
            LocalOptionsForm.Singleton.Show();
        }

        private void LogoutWithDialog()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (_IsRemotingOn)
                CgpClient.Singleton.Logout();
            else
                LocalOptionsForm.Singleton.ShowTabPage("_tpRemotingSettings");
        }

        private void _remotingStatus_DoubleClick(object sender, EventArgs e)
        {
            LogoutWithDialog();
        }

        private void _alarmsMI_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                if (_alarmsDialog == null)
                {
                    _alarmsDialog = new AlarmsDialog(_listAlarms);
                }
                _alarmsDialog.Show();
            }
        }

        public void HideAlarms()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(HideAlarms));
            }
            else
            {
                _tsAlarms.Visible = false;

                if (_alarmsDialog != null)
                {
                    _alarmsDialog.Close();
                    _alarmsDialog = null;
                }

                if (_alarmDetails != null)
                {
                    _alarmDetails.Close();
                    _alarmDetails = null;
                }
            }
        }

        public void ShowAlarms()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowAlarms));
            }
            else
            {
                _tsAlarms.Visible = true;
                _tsAlarms.Width = 190;
            }
        }

        public void ShowHome()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowHome));
            }
            else
            {
                _tsHome.Visible = true;
                _tsHome.Text = LocalizationHelper.GetString("General_tsHome");
            }
        }

        public void HideHome()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(HideHome));
            }
            else
            {
                _tsHome.Visible = false;
                _tsHome.Text = LocalizationHelper.GetString("General_tsHome");
            }
        }


        //public void UpdateVisualRecentMenuItems()
        //{
        //    if (!CgpClient.Singleton.IsConnectionLost(false))
        //    {
        //        _recentMenuItems.Clear();
        //        Exception ex = new Exception();
        //        IDictionary<string, int> quickMenuItems = CgpClient.Singleton.MainServerProvider.GetUserQuickMenu(CgpClient.Singleton.LoggedAs);
        //        if (quickMenuItems != null)
        //        {
        //            IOrderedEnumerable<string> sortedKeys = from k in quickMenuItems.Keys
        //                                                    orderby quickMenuItems[k] descending
        //                                                    select k;
        //            foreach (string key in sortedKeys)
        //            {
        //                ToolStripItem referencedItem = GetLeftMenuButton(key);
        //                if (referencedItem != null)
        //                {
        //                    //create new QuickButton
        //                    ToolStripItem newQuickItem = new ToolStripMenuItem();
        //                    newQuickItem.Name = referencedItem.Name;
        //                    newQuickItem.Tag = referencedItem;
        //                    newQuickItem.Text = referencedItem.Text;
        //                    newQuickItem.Image = referencedItem.Image;
        //                    newQuickItem.TextImageRelation = referencedItem.TextImageRelation;
        //                    newQuickItem.TextAlign = referencedItem.TextAlign;
        //                    newQuickItem.ImageAlign = referencedItem.ImageAlign;
        //                    newQuickItem.Click += quickItem_Click;
        //                    //newQuickItem.Click += referencedItem.PerformClick();
        //                    _recentMenuItems.Add(newQuickItem.Name, quickMenuItems[key]);
        //                    TryInsertButtonIntoQuickButtons(newQuickItem, true);
        //                }
        //            }
        //        }
        //    }
        //}

/*
        private ToolStripItem GetLeftMenuButton(string buttonName)
        {
            foreach (ToolStripItem item in _leftMenu.Items)
            {
                if (item.Name == buttonName)
                {
                    return item;
                }
            }
            return null;
        }
*/

        public void SaveOpenedWindows()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(SaveOpenedWindows));
                return;
            }

            try
            {
                List<UserOpenedWindow> userOpenedWindows = new List<UserOpenedWindow>();
                SetDockedWindows(userOpenedWindows);
                SetUndockedWindows(userOpenedWindows);
                SetExternWindows(userOpenedWindows);

                CgpClient.Singleton.MainServerProvider.SaveUserOpenedWindows(userOpenedWindows, CgpClient.Singleton.LoggedAs);
                //CgpClient.Singleton.MainServerProvider.SaveRecentMnuItems(CgpClient.Singleton.LoggedAs, _recentMenuItems);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        private void SetExternWindows(List<UserOpenedWindow> userOpenedWindows)
        {
            if (_alarmDetails != null && _alarmDetails.Visible)
            {
                UserOpenedWindow openedWindow = new UserOpenedWindow();
                SetCommonParameters(_alarmDetails, openedWindow, false, 0);
                openedWindow.ObjectId = _alarmDetails.GetIdServerAlarm().ToString();
                userOpenedWindows.Add(openedWindow);
            }
        }

        private void SetDockedWindows(List<UserOpenedWindow> userOpenedWindows)
        {
            for (int i = 0; i < _tsOpenWindows.Items.Count; i++)
            {
                ToolStripItem item = _tsOpenWindows.Items[i];
                if (item.Tag != null)
                {
                    var mdiChildForm = item.Tag as MdiChildForm;
                    bool editForm = mdiChildForm != null && mdiChildForm.IsEditForm();

                    if (!((item.Tag as Form) is AllPluginsForm))
                    {
                        SetOpenWindow(item.Tag as Form, userOpenedWindows, editForm, item.BackColor != _tsOpenWindows.BackColor, i);
                    }
                }
            }
        }

        private void SetUndockedWindows(List<UserOpenedWindow> userOpenedWindows)
        {
            for (int i = 0; i < _undockedForms.Count; i++)
            {
                Form form = _undockedForms[i];
                var mdiChildForm = form as MdiChildForm;
                bool editForm = mdiChildForm != null && mdiChildForm.IsEditForm();
                SetOpenWindow(form, userOpenedWindows, editForm, false, i);
            }
        }

        private void SetOpenWindow(Form openedForm, List<UserOpenedWindow> userOpenedWindows, bool editForm, bool selected, int windowIndex)
        {
            UserOpenedWindow openedWindow = new UserOpenedWindow();
            if (editForm)
            {
                return;

                var cgpEditForm = openedForm as ICgpEditForm;
                if (cgpEditForm != null)
                {
                    var ormObject = cgpEditForm.GetEditingObject() as AOrmObject;
                    if (ormObject != null)
                        openedWindow.ObjectId = ormObject.GetIdString();
                    openedWindow.ShowOption = (int) (cgpEditForm.ShowOption);
                    openedWindow.AllowEdit = (cgpEditForm).AllowEdit;
                }
            }
            SetCommonParameters(openedForm, openedWindow, selected, windowIndex);
            userOpenedWindows.Add(openedWindow);
        }

        private void SetCommonParameters(Form openedForm, UserOpenedWindow openedWindow, bool selected, int windowIndex)
        {
            openedWindow.Docked = (int)(openedForm.Dock);
            openedWindow.FormName = openedForm.Name;
            openedWindow.Height = openedForm.Height;
            openedWindow.Modul = "CgpClient";
            openedWindow.Monitor = 1;
            if (openedForm.WindowState == FormWindowState.Minimized)
            {
                openedWindow.PosLeft = openedForm.RestoreBounds.X;
                openedWindow.PosTop = openedForm.RestoreBounds.Y;
            }
            else
            {
                openedWindow.PosLeft = openedForm.Left;
                openedWindow.PosTop = openedForm.Top;
            }
            openedWindow.Selected = selected;
            openedWindow.Width = openedForm.Width;
            openedWindow.BorderStyle = (int)openedForm.FormBorderStyle;
            openedWindow.WindowIndex = windowIndex;
            openedWindow.HasParent = openedForm.MdiParent != null;
        }

// ReSharper disable UnusedMember.Local
        private void UpdateOpenedWindows(IEnumerable<KeyValuePair<string, int>> openedWindows)
// ReSharper restore UnusedMember.Local
        {
            Form activeForm = null;
            foreach (KeyValuePair<string, int> item in openedWindows)
            {
                Form formToInsert = GetFormByName(item.Key);
                formToInsert.Show();
                if (item.Value == 1)
                {
                    activeForm = formToInsert;
                }
            }
            if (activeForm != null)
            {
                SetActOpenWindow(activeForm);
            }
        }

        private Form GetFormByName(string formName)
        {
            foreach (ToolStripItem item in _leftMenu.Items)
            {
                if (item.Visible && item.Tag != null)
                {
                    if (((Form)(item.Tag)).Name == formName)
                    {
                        return (Form)(item.Tag);
                    }
                }
            }
            return null;
        }

/*
        private Dictionary<string, int> GetOpenedWindows()
        {
            Dictionary<string, int> returnValue = new Dictionary<string, int>();
            for (int i = 0; i < _tsOpenWindows.Items.Count; i++)
            {
                if (_tsOpenWindows.Items[i].BackColor == _tsOpenWindows.BackColor)
                {
                    returnValue.Add(_tsOpenWindows.Items[i].Name, 0);
                }
                else
                {
                    returnValue.Add(_tsOpenWindows.Items[i].Name, 1);
                }
            }
            return returnValue;
        }
*/

        private void DeletedObjectEvent(ObjectType objType, object objId)
        {
            _recObjList.RemoveObjectFromRecentListById(objId);
            DbsSupport.CloseEditedForm(objType, objId);
        }

        public void RegisterColorChanged(DVoid2Void eventColorChanged)
        {
            ColorSettingsChangedHandler.Singleton.RegisterColorChanged(eventColorChanged);
        }

        public void UnregisterColorChanged(DVoid2Void eventColorChanged)
        {
            ColorSettingsChangedHandler.Singleton.UnregisterColorChanged(eventColorChanged);
        }

        public void RegisterAutoCloseChanged(DVoid2Void eventAutoCloseChanged)
        {
            AutoCloseSettingsChangedHandler.Singleton.RegisterAutoCloseChanged(eventAutoCloseChanged);
        }

        public void UnregisterAutoCloseChanged(DVoid2Void eventAutoCloseChanged)
        {
            AutoCloseSettingsChangedHandler.Singleton.UnregisterAutoCloseChanged(eventAutoCloseChanged);
        }



        private void _leftMenu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                _bLeftMenuUp_Click(null, null);

            if (e.KeyCode == Keys.Down)
                _bLeftMenuDown_Click(null, null);
            e.IsInputKey = true;
        }

        private void _tsOpenWindows_MouseLeave(object sender, EventArgs e)
        {
            _mouseOverOpenWindows = false;
        }

        private void _tsHome_Click(object sender, EventArgs e)
        {
            LoadPluginsForm();
        }

        private void _tsLogo_Click(object sender, EventArgs e)
        {
            AboutForm.Singleton.Show();
        }

        #region HandCursor

        private void AddCursorType(Cursor cursor)
        {
            Cursor = cursor;
        }

        private void _tsHome_MouseEnter(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Hand);
        }

        private void _tsAlarms_MouseEnter(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Hand);
        }

        private void _tsLogo_MouseEnter(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Hand);
        }

        private void _remotingStatus_MouseEnter(object sender, EventArgs e)
        {
            AddCursorType(Cursors.UpArrow);
        }

        private void _loggedAs_MouseEnter(object sender, EventArgs e)
        {
            AddCursorType(Cursors.UpArrow);
        }

        private void _tsHome_MouseLeave(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Default);
        }

        private void _tsAlarms_MouseLeave(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Default);
        }

        private void _tsLogo_MouseLeave(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Default);
        }

        private void _remotingStatus_MouseLeave(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Default);
        }

        private void _loggedAs_MouseLeave(object sender, EventArgs e)
        {
            AddCursorType(Cursors.Default);
        }
        #endregion

        private const int SPLIT_CONTAINER_PANEL_MIN_HEIGHT = 26;

        //method preventing menu overlay  
        private void splitContainerLeftMenu_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (e.SplitY - _splitContainerLeftMenu.SplitterWidth <= _splitContainerLeftUpperMenu.SplitterDistance + _splitContainerLeftUpperMenu.Panel2MinSize)
            {
                _splitContainerLeftUpperMenu.SplitterDistance = e.SplitY + _splitContainerLeftMenu.SplitterWidth;
            }

            int spliterPosition = _splitContainerLeftMenu.Height - (_splitContainerLeftMenu.SplitterDistance + _splitContainerLeftMenu.SplitterWidth);

            if (spliterPosition == SPLIT_CONTAINER_PANEL_MIN_HEIGHT || spliterPosition == SPLIT_CONTAINER_PANEL_MIN_HEIGHT + 1)
            {
                SetLeftMenuSize(false);
            }
            else
            {
                SetLeftMenuSize(true);
            }
        }

        private void splitContainerLeftUpperMenu_SplitterMoved(object sender, SplitterEventArgs e)
        {
// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (_splitContainerLeftUpperMenu.SplitterDistance == SPLIT_CONTAINER_PANEL_MIN_HEIGHT)
            {
                SetLeftUpperMenuSize(false);
            }
            else
            {
                SetLeftUpperMenuSize(true);
            }
        }


// ReSharper disable once NotAccessedField.Local
        bool _isLeftUpperMenuMaximized = true;
/*
        private void _pbMinMaxLeftUpperMenu_Click(object sender, EventArgs e)
        {
            if (_isLeftUpperMenuMaximized)
            {
                //set to 0 to ensure _splitContainerLeftUpperMenu.Panel1 minimum size
                _splitContainerLeftUpperMenu.SplitterDistance = 0;
                SetLeftUpperMenuSize(false);
            }
            else
            {
                _splitContainerLeftUpperMenu.SplitterDistance = _splitContainerLeftUpperMenu.SplitterDistance + 180;
                SetLeftUpperMenuSize(true);
            }
        }
*/

        bool _isLeftMenuMaximized = true;
        private void _pbMinMaxLeftMenu_Click(object sender, EventArgs e)
        {
            if (_isLeftMenuMaximized)
            {
                //set to 1000 to ensure _splitContainerLeftMenu.Panel2 minimum size
                _splitContainerLeftMenu.SplitterDistance = 1000;
                SetLeftMenuSize(false);
            }
            else
            {
                _splitContainerLeftMenu.SplitterDistance = _splitContainerLeftMenu.SplitterDistance - 180;
                SetLeftMenuSize(true);
            }
        }

        private void SetLeftMenuSize(bool isMaximize)
        {
            if (isMaximize)
            {
                _pbMinMaxLeftMenu.Image = ResourceGlobal.Hide16;
                _isLeftMenuMaximized = true;
                _lvMenu.Visible = true;
            }
            else
            {
                _pbMinMaxLeftMenu.Image = ResourceGlobal.Show16;
                _isLeftMenuMaximized = false;
                _lvMenu.Visible = false;
            }
        }

        private void SetLeftUpperMenuSize(bool isMaximize)
        {
            _isLeftUpperMenuMaximized = isMaximize;
        }

        /// <summary>
        /// Truncates a string from specified position and adds
        /// an ellipsis to the end of the resulting string.
        /// </summary>
        /// <param name="orgString">Original string.</param>
        /// <param name="length">Character position to start truncating.</param>
        /// <returns>Returns a truncated string.</returns>
        private string TruncateString(string orgString, int length)
        {
            if (string.IsNullOrEmpty(orgString))
            {
                return orgString;
            }
            if (orgString.Length > length)
            {
                return orgString.Substring(0, length) + "...";
            }
            return orgString;
        }

        

        private void DemoPeriodHasExpired()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(DemoPeriodHasExpired));
            }
            else
            {
                Dialog.Warning(GetString("WarningDemoPeriodHasExpired"));
                _forcedClose = true;
                _isClosing = true;
                Close();
            }
        }

        private void _tsLogo_Paint(object sender, PaintEventArgs e)
        {
            _lDemoLicense.Top = _status.Top;
            _lDemoLicense.Left = _tsLogo.Bounds.Left;
            _lDemoLicense.Width = _tsLogo.Width;
        }

        public void ShowInfoDemoLicense()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowInfoDemoLicense));
            }
            else
            {
                _lDemoLicense.Visible = true;
            }
        }

        public void HideInfoDemoLicense()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(HideInfoDemoLicense));
            }
            else
            {
                _lDemoLicense.Visible = false;
            }
        }

        public void UpdateInfoDemoLicense(int hours, int minutes)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, int>(UpdateInfoDemoLicense), hours, minutes);
            }
            else
            {
                _lDemoLicense.Text = string.Format("({0}) {1}: {2} h. {3} min.",
                    GetString("DemoLicence"),
                    GetString("RemainingTime"),
                    hours,
                    minutes);
            }
        }

        private void _structuredSiteForm_Click(object sender, EventArgs e)
        {
            StructuredSiteForm.Singleton.Show();
        }

/*
        private void _scenes_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
                CalendarsForm.Singleton.Show();
        }
*/

        public static string TranslateOrmObjectName(ObjectType objectType, string name)
        {
            switch (objectType)
            {
                case ObjectType.Calendar:
                    if (name == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                        return CgpClient.Singleton.LocalizationHelper.GetString(name);

                    break;
                case ObjectType.DayType:
                    if (name == DayType.IMPLICIT_DAY_TYPE_HOLIDAY ||
                        name == DayType.IMPLICIT_DAY_TYPE_VACATION)
                    {
                        return CgpClient.Singleton.LocalizationHelper.GetString(name);
                    }

                    break;
                case ObjectType.DailyPlan:
                    if (name == DailyPlan.IMPLICIT_DP_ALWAYS_ON ||
                        name == DailyPlan.IMPLICIT_DP_ALWAYS_OFF ||
                        name == DailyPlan.IMPLICIT_DP_DAYLIGHT_ON ||
                        name == DailyPlan.IMPLICIT_DP_NIGHT_ON)
                    {
                        return CgpClient.Singleton.LocalizationHelper.GetString(name);
                    }

                    break;
            }

            return name;
        }

        private void OnGoBackClicked(object sender, EventArgs e)
        {
            var isOpened = _actTsiTsOpenWindows != null;

            if (isOpened && _actTsiTsOpenWindows.Tag is ICgpEditForm)
            {
                var editForm = (ICgpEditForm)_actTsiTsOpenWindows.Tag;

                foreach (ToolStripItem item in _tsOpenWindows.Items)
                {
                    if (item.Tag is ICgpTableForm)
                    {
                        var tableForm = (ICgpTableForm)item.Tag;

                        if (tableForm.IsMyEditForm(editForm))
                        {
                            ((MdiChildForm)tableForm).Show();
                            break;
                        }
                    }
                }
            }
            else
            {
                LoadPluginsForm();

                if (isOpened)
                {
                    AllPluginsForm.Singleton.SelectPlugin(_actTsiTsOpenWindows.Text);
                }
            }
        }

        private void OpenTableFormForEditedObject(Form findFormForObject)
        {
            // Only for ICgpEditForm forms!
            if (!(findFormForObject is ICgpEditForm))
                return;

            ICgpTableForm tableForm = null;
            bool wasOpened = false;

            ICgpEditForm editForm = (ICgpEditForm)findFormForObject;

            // Find matching ICgpTableForm
            if (editForm.GetEditingObject() is Person)
                tableForm = PersonsForm.Singleton;
            else if (editForm.GetEditingObject() is LoginGroup)
                tableForm = LoginGroupsForm.Singleton;
            else if (editForm.GetEditingObject() is Login)
                tableForm = LoginsForm.Singleton;
            else if (editForm.GetEditingObject() is GlobalAlarmInstruction)
                tableForm = GlobalAlarmInstructionsForm.Singleton;
            else if (editForm.GetEditingObject() is DailyPlan)
                tableForm = DailyPlansForm.Singleton;
            else if (editForm.GetEditingObject() is TimeZone)
                tableForm = TimeZonesForm.Singleton;
            else if (editForm.GetEditingObject() is DayType)
                tableForm = DayTypesForm.Singleton;
            else if (editForm.GetEditingObject() is Calendar)
                tableForm = CalendarsForm.Singleton;
            else if (editForm.GetEditingObject() is Card)
                tableForm = CardsForm.Singleton;
            else if (editForm.GetEditingObject() is CardTemplate)
                tableForm = CardTemplatesForm.Singleton;
            else if (editForm.GetEditingObject() is CardSystem)
                tableForm = CardSystemsForm.Singleton;
            else if (editForm.GetEditingObject() is PresentationGroup)
                tableForm = PresentationGroupsForm.Singleton;
            else if (editForm.GetEditingObject() is PresentationFormatter)
                tableForm = PresentationFormattersForm.Singleton;
            else if (editForm.GetEditingObject() is CisNG)
                tableForm = CisNGsForm.Singleton;
            else if (editForm.GetEditingObject() is CisNGGroup)
                tableForm = CisNGGroupsForm.Singleton;
            else
                return;

            if (tableForm == null)
                return;

            foreach (ToolStripItem item in _tsOpenWindows.Items)
            {
                if (item.Tag is ICgpTableForm)
                {
                    // Appropriate form is opened only if it was already opened before
                    if (item.Tag.GetType() == tableForm.GetType())
                    {
                        wasOpened = true;
                        break;
                    }
                }
            }

            if (wasOpened)
            {
                // This shows the form in front, but this whole method is called first so new form (shown after this method returns) will be again in the front
                var formVar = tableForm as Form;

                if (formVar.Visible == false)
                {
                    formVar.Show();
                }

                formVar.BringToFront();
            }
        }

        public void ShowFormLoadProgress(/*MdiChildForm*/ object mdiChildForm)
        {
            if (InvokeRequired)
            {
                Invoke(new DObject2Void(ShowFormLoadProgress), mdiChildForm);
                return;
            }

            try
            {
                // This PleaseWaitMonitor is active only when Client is already started (Splash is hidden)
                if (_pleaseWaitMonitorSplash != null)
                    return;

                // Remember MdiChildForm that started loading
                /*if (mdiChildForm.GetType().GetInterfaces().Contains(typeof(ICgpEditForm)) == true || mdiChildForm.GetType().GetInterfaces().Contains(typeof(ICgpTableForm)) == true)
                {
                    MdiChildForm form = (MdiChildForm)mdiChildForm;

                    if (_pleaseWaitMonitorEditForms == null)
                    {
                        _pleaseWaitMonitorEditForms = new PleaseWaitMonitor(_status.PointToScreen(_toolStripLoading.Bounds.Location), PleaseWaitMode.EditMdiForms);
                        _pleaseWaitMonitorEditForms.Tag = (mdiChildForm as MdiChildForm).GetType().Name;
                        Trace.WriteLine("ShowFormLoadProgress: " + _pleaseWaitMonitorEditForms.Tag);
                    }
                }*/
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
                _pleaseWaitMonitorEditForms = null;
            }
        }

        public void HideFormLoadProgress(/*MdiChildForm*/ object mdiChildForm)
        {
            if (InvokeRequired)
            {
                Invoke(new DObject2Void(HideFormLoadProgress), mdiChildForm);
                return;
            }

            try
            {
                if (_pleaseWaitMonitorEditForms != null)
                {
                    _pleaseWaitMonitorEditForms.Dispose();
                    _pleaseWaitMonitorEditForms = null;
                }
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
                _pleaseWaitMonitorEditForms = null;
            }
        }

    }
}
