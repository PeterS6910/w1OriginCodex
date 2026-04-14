using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    /// <summary>
    /// main class of the CgpClient application
    /// </summary>
    public sealed class CgpClient : ACgpClientBase<CgpClient, CgpVisualPluginManager>
    {
        /// <summary>
        /// plugin manager to load and manage visible plugins
        /// </summary>
        internal const string ALARM_TYPE_LOCALIZATION_PREFIX = "AlarmType_";

        private readonly ExtendedVersion _version =
            new ExtendedVersion(typeof(CgpClient),
                true, null,
#if !DEBUG
#if RELEASE_SPECIAL
                DevelopmentStage.Testing
#else
                DevelopmentStage.Release
#endif
#else
 DevelopmentStage.Testing
#endif
);

        private CgpClient() : base(null)
        {
            _namesLocalizationByObjectType = new Dictionary<ObjectType, Func<AOrmObject, string>>
            {
                {
                    ObjectType.Calendar,
                    CalendarNameLocalization
                },
                {
                    ObjectType.DayType,
                    DayTypeNameLocalization
                }
            };
        }

        public override ExtendedVersion Version
        {
            get { return _version; }
        }

        protected override string Language
        {
            get { return GeneralOptions.Singleton.Language; }
        }

        public override bool GetRequirePINCardLogin
        {
            get
            {
                return GeneralOptionsForm.Singleton.GetRequirePINCardLogin;
            }
        }

        public override string ComPortName
        {
            get { return GeneralOptions.Singleton.ComPortName; }
        }

        /// <summary>
        /// runs the visible modules and forms
        /// </summary>
        protected override void RunInternal()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // hook the events here
            Application.Run(CgpClientMainForm.Singleton);
        }

        protected override string PluginPath
        {
            get { return Application.StartupPath; }
        }

        /// <summary>
        /// loads all visible plugins
        /// </summary>
        protected override void LoadPlugins()
        {
            _pluginManager.ImplicitParent = CgpClientMainForm.Singleton;

            base.LoadPlugins();
        }

        protected override string FriendlyName
        {
            get { return GeneralOptions.Singleton.FriendlyName; }
        }

        protected override void OnPluginSetChanged(ICgpClientPlugin[] parameter)
        {
            PluginDashboardForm.Singleton.PluginsReport(parameter);

            CgpClientMainForm.Singleton.RefreshPlugingsMenu(
                _pluginManager.GetVisualPlugins());
        }

        protected override void OnPluginLoaded(ICgpClientPlugin parameter)
        {

            //PluginDashboardForm.Singleton.ShowPlugin();
        }

        protected override void NotifyProcessAlreadyRunning(Process otherProcess)
        {
            WindowManipulation.BringProcessWindowToFront(otherProcess);
            Dialog.Info("Contal Nova Client is already started for this user");
        }

        protected override void OnInitRemotingFailed()
        {
            LocalOptionsForm.Singleton.Show();
        }

        protected override int Port
        {
            get
            {
                return GeneralOptions.Singleton.Port;
            }
        }

        protected override string ServerAddress
        {
            get { return GeneralOptions.Singleton.ServerAddress; }
        }

        protected override void OnDisconnected()
        {
            CgpClientMainForm.Singleton.UpdateRemotingStatus(false);
            CgpClientMainForm.Singleton.UpdateTsAlarms(false);

            LoginPasswordForm.Singleton.Hide();

            GeneralOptionsForm.Singleton.StopRefreshingDemoTime();
            AlarmSound.Singleton.StopPlayingSoundServerDisconnected();

            DeactiveDemoLicenceInfo();
        }

        public void DeactiveDemoLicenceInfo()
        {
            if (_timerDemoLicenseRemainingTime == null)
                return;

            _timerDemoLicenseRemainingTime.StopTimer();
            _timerDemoLicenseRemainingTime = null;

            CgpClientMainForm.Singleton.HideInfoDemoLicense();
        }

        private readonly AutoResetEvent _clientUpgradeTransferMutex =
            new AutoResetEvent(false);

        //this value must be smaller than delay on server side
        private ITimer _timerDemoLicenseRemainingTime;

        protected override void TryPerformClientUpgrade()
        {
            Version upgradeVersion;

            if (MainServerProvider.NovaClientUpgradeAvailable(
                Version,
                out upgradeVersion))
            {
                bool startManually =
                    !GeneralOptions.Singleton.AutoUpgradeClient &&
                    Dialog.Question(LocalizationHelper.GetString("QuestionUpgradeClient"));

                if (GeneralOptions.Singleton.AutoUpgradeClient || startManually)
                {
                    SafeThread<Version>.StartThread(
                        StartClientUpgrade,
                        upgradeVersion);

                    _clientUpgradeTransferMutex.WaitOne();
                }
            }
        }

        private void StartClientUpgrade(Version upgradeVersion)
        {
            if (MainServerProvider == null)
                return;

            var upgradeForm =
                new ClientUpgradeForm(
                    (int)MainServerProvider.GetClientUpgradeFileLength(upgradeVersion),
                    upgradeVersion);

            upgradeForm.ShowDialog();

            _clientUpgradeTransferMutex.Set();
        }

        protected override void OnConnected()
        {
            if (MainServerProvider.DemoLicence)
            {
                _timerDemoLicenseRemainingTime = TimerManager.Static.StartTimer(
                    60 * 1000,
                    true,
                    OnDemoLicenseTimerTick);

                CgpClientMainForm.Singleton.ShowInfoDemoLicense();
            }

            CgpClientMainForm.Singleton.UpdateRemotingStatus(true);
            CgpClientMainForm.Singleton.UpdateTsAlarms(true);

            CgpClientMainForm.Singleton.ChangeAlarms();
        }

        private bool OnDemoLicenseTimerTick(TimerCarrier timerCarrier)
        {
            if (MainServerProvider != null && !MainServerProvider.DemoLicence)
                return false;

            double milisecondsRemaining =
                MilisecondsRemaining();

            int hours = 
                (int)milisecondsRemaining / (1000 * 60 * 60);

            int minutes =
                (int)(milisecondsRemaining - (hours * 60 * 60 * 1000)) / (1000 * 60);

            CgpClientMainForm.Singleton.UpdateInfoDemoLicense(hours, minutes);
            return true;
        }

        protected override void RequestAuthenticateLoginAsync()
        {
            CgpClientMainForm.Singleton.ShowLoginPasswordDialog();
        }

        protected override bool PrepareLogout()
        {
            CgpClientMainForm.Singleton.ClearLeftMenuFilter();
            CgpClientMainForm.Singleton.SaveOpenedWindows();

            return CgpClientMainForm.Singleton.HideAllWindowsWithCheck();
        }

        protected override void OnLoggedOut()
        {
            AllPluginsForm.Singleton.ClearListView();

            CloneFilterValues.Singleton.ClearCloneFilterValues();
        }

        protected override void RequestUnlockLogin(ref string loggedAs)
        {
            LoginPasswordForm.Singleton.ShowDialogApplicationLocked(ref loggedAs);
        }

        public override void ClientLoginWithCard(string fullCardNumber)
        {
            LoginPasswordForm.Singleton.LoginWithCard(fullCardNumber);
        }

        public override void ClientInfoWrongPIN()
        {
            LoginPasswordForm.Singleton.ClientInfoWrongPIN();
        }

        public override void LoginCardSwiped(string fullCardNumber)
        {
            LoginPasswordForm.Singleton.CardReaderCardSwiped(fullCardNumber);
        }

        public bool LoadPluginControlToForm(
            string pluginFriendlyName,
            Object obj, 
            Control control,
            IExtendedCgpEditForm cgpEditForm,
            bool allowEdit)
        {
            ICgpClientPlugin plugin = _pluginManager.GetLoadedPlugin(pluginFriendlyName);

            if (plugin == null)
                return false;

            var aCgpVisualPlugin = plugin as ICgpVisualPlugin;

            return 
                aCgpVisualPlugin != null && 
                aCgpVisualPlugin.LoadPluginControlToForm(
                    obj, 
                    control,
                    cgpEditForm,
                    allowEdit);
        }

        private readonly Dictionary<ObjectType, Func<AOrmObject, string>> _namesLocalizationByObjectType;

        public string GetLocalizedObjectName(AOrmObject aOrmObject)
        {
            if (aOrmObject == null)
                return string.Empty;

            Func<AOrmObject, string> nameLocalization;

            if (_namesLocalizationByObjectType.TryGetValue(
                aOrmObject.GetObjectType(),
                out nameLocalization))
            {
                return nameLocalization(aOrmObject);
            }

            var plugins = _pluginManager.GetVisualPlugins();

            return plugins.Select(
                plugin =>
                    plugin.GetLocalizedObjectName(aOrmObject))
                .FirstOrDefault(
                    localizedObjectName =>
                        !string.IsNullOrEmpty(localizedObjectName))
                   ?? aOrmObject.ToString();
        }

        private string CalendarNameLocalization(AOrmObject aOrmObject)
        {
            var calendarName = aOrmObject.ToString();

            if (calendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
            {
                return LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
            }

            return calendarName;
        }

        private string DayTypeNameLocalization(AOrmObject aOrmObject)
        {
            var dayTypeName = aOrmObject.ToString();

            if (dayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
            {
                return LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
            }

            if (dayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                return LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
            }

            return dayTypeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetLocalizedString(string input)
        {
            string result = LocalizationHelper.GetString(input, false);

            if (!result.StartsWith(
                    IwQuick.Localization.LocalizationHelper.NO_TRANSLATION))
                return result;

            foreach (ICgpVisualPlugin plugin in _pluginManager.GetVisualPlugins())
            {
                result = plugin.GetTranslateString(input);

                if (!result.StartsWith(IwQuick.Localization.LocalizationHelper.NO_TRANSLATION))
                    return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmType"></param>
        /// <returns></returns>
        public string GetLocalizedAlarmRuiPrefix(AlarmType alarmType)
        {
            return GetLocalizedString(ALARM_TYPE_LOCALIZATION_PREFIX + alarmType);
        }

        public bool LoadPluginControlToForm(
            string pluginFriendlyName, 
            Object obj, 
            Dictionary<string, bool> filter,
            Control control,
            IExtendedCgpEditForm cgpEditForm,
            bool allowEdit)
        {
            ICgpClientPlugin plugin = _pluginManager.GetLoadedPlugin(pluginFriendlyName);

            if (plugin == null)
                return false;

            var aCgpVisualPlugin = plugin as ICgpVisualPlugin;

            return 
                aCgpVisualPlugin != null && 
                aCgpVisualPlugin.LoadPluginControlToForm(
                    obj, 
                    filter,
                    control,
                    cgpEditForm,
                    allowEdit);
        }
    }
}
