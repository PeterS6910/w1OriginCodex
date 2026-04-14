using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.Versioning;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.CCU.RPN;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Net;
using Contal.IwQuick.Net.Microsoft;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.LwMessaging;
using Contal.LwRemoting2;
using Contal.LwSerialization;
using JetBrains.Annotations;
using Microsoft.Win32;
using Crc32 = Contal.IwQuick.Crypto.Crc32;
using TimeZone = Contal.Cgp.NCAS.CCU.DB.TimeZone;

namespace Contal.Cgp.NCAS.CCU
{
    public enum PrirotyForOnTimerEvent : byte
    {
        Outputs = 1,
        DoorEnvironments = 2,
        DailyPlanSDP = 3,
        AlarmAreas = 4,
        CardReaders = 5,
        SendEvents = 6,
        CheckAutonomousMode = 7,
        FileOperations = 8
    }

    public enum StreamMode : byte
    {
        All = 0,
        Add = 1,
        Delete = 2
    }

    public sealed class CcuCore : ASingleton<CcuCore>
    {
        private const string CNMP_CCU_LOOKUP_KEY = "577ccf9d9d6cd14502a4f09ea1c182e89f22b097";
        private const string CNMP_SERVER_LOOKUP_KEY = "a20bdcaba7ae09c7ac4bba2e4cd02179af94587a";
        private const int CNMP_LOOKUP_DURATION = 180000;
        private const int CNMP_LOOKUP_INTERVAL = 30000;

        //private const int TIME_INTERVAL_CHECK_DELAY = 180000;
        public const int CARD_READER_ACCEPTED_DELAY = 3000;
        public const int CARD_READER_REJECT_DELAY = 1200;
        public const int CARD_READER_START_DELAY = 4000;

        public const string DCU_UPGRADES_DIRECTORY_NAME = @"\NandFlash\ccu\DcuUpgrades\";
        public const string CLSP_HWC_FILE_PATH = @"\NandFlash\ccu\clsp_hwc.bin";
        public const int CLSP_HWC_VERSION = 1319;
        public const string CLSP_COM_PORT = "COM6";
        public const int CONTAL_NOVA_DEFAULT_FILTERTIME = 50;
        private const int UNPACK_PROGRESS_STEP = 5;

        public const string STORAGE_CARD_PATH = @"\Storage card\";
        public const string NANDFLASH_PATH = @"\NandFlash\";
        private const string TEMP_PATH = @"\Temp\";
        public const string CCU_SUBPATH = @"CCU\";
        public const string TEMP = @"Temp\";
        private const string UPTIME_MONITORING_FILE_NAME = "UptimeMonitoring.dat";

        private bool? _highCCUMemoryLoad;
        private readonly object _lockHighCCUMemoryLoad = new object();
        private bool _enabledCCUMemoryLoadCheck;

        private ITimer _unconfigureTimer;
        private readonly Object _unconfigureTimerLock = new object();

        //If CCU has Storage card than root path for block files is "Storage Card\CCU" else is "NandFlash\CCU"
        private readonly string _ccuRootPath = Program.SDCardPresent ? STORAGE_CARD_PATH + CCU_SUBPATH : NANDFLASH_PATH + CCU_SUBPATH;
        private bool _safeThreadSleepCondition = true;

        public LocalizationHelper LocalizationHelper { get; private set; }

        public string RootPath
        {
            get { return _ccuRootPath; }
        }

        public string GetCCUFilePath(string filename)
        {
            return _ccuRootPath + filename;
        }

        //send time interval in miliseconds
        private Int64 _sendTimeInterval = 60000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public void SetSynchronizationTimeIterval(int interval)
        {
            if (interval >= 30000)
            {
                if (_sendTimeInterval != (interval / 2))
                {
                    _sendTimeInterval = (interval / 2);
                    RestartSNTP();
                }
            }
        }

        private readonly ExtendedVersion _version = new ExtendedVersion(
            typeof(CcuCore),
            true, false, null,
#if !DEBUG
 DevelopmentStage.Release
#else
 DevelopmentStage.Alpha
#endif
, "EN");

        public ExtendedVersion Version
        {
            get { return _version; }
        }

        private readonly string _ceImageVersion;

        public VersionsInfo VersionsInfo { get; private set; }

        private const string CNMP_CCU_TYPE = "/Cgp/NCAS/CCU";
        //private const string CNMP_SERVER_TYPE = "/Cgp/NCAS/NCASServer";
        private const string THREAD_NAME_INITIALIZE = "CcuCore: Initialize";
        private const string THREAD_NAME_SIGNAL_WATCHDOG = "CcuCore: SignalWatchdog";

        private readonly ManualResetEvent _exitMutex = new ManualResetEvent(false);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly SafeThread _initializationThread;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly SafeThread _watchdogThread;

        //private CNMPAgent _cnmpAgent;

        private DataChannelPeer _dataChannelPeer;

        public DataChannelPeer DataChannelPeer
        {
            get { return _dataChannelPeer; }
        }

        /*
        /// <summary>
        /// Init CNMP - temporary disabled
        /// </summary>
// ReSharper disable UnusedMember.Local
        private void InitCNMP()
// ReSharper restore UnusedMember.Local
        {
            try
            {
                CreateCNMPAgent();
                DoCNMPLookup();

                //Lookup will be performed only for limited time 
                StartCNMPLookupTimeout();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }*/

        /*
        private void CreateCNMPAgent()
        {
            try
            {
                if (_cnmpAgent != null)
                {
                    _cnmpAgent.Stop();
                    _cnmpAgent = null;
                }

                _cnmpAgent = new CNMPAgent(
                    CeNetworkManagement,
                    CNMP_CCU_TYPE + StringConstants.SLASH + Environment.Version,
                    CNMP_CCU_TYPE,
                    CNMP_SERVER_LOOKUP_KEY, _version.ToString()) 
                    { 
                        Timeout = 6000,
                        RetryCount = 2
                    };

                _cnmpAgent.LookupFinished += LookupFinished;
                _cnmpAgent.ValidRequestReceived += ValidRequestReceived;
                _cnmpAgent.SetExtra("MainboardType", MainBoard.Variant.ToString());
                _cnmpAgent.SetExtra("VersionCCU", Singleton.Version.ToString());
                _cnmpAgent.SetExtra("VersionCE", Singleton.GetWinCEImageVersion());
                _cnmpAgent.Start();
                DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "CNMP started successfuly");
            }
            catch (Exception err)
            {
                HandledExceptionAdapter.Examine(err);
                DebugLog.Error(Log.CALM_LEVEL, () => "CNMP startup failed\r\n" + err.Message);
            }
        }

        void LookupFinished(CNMPLookupType lookupType, string value, CNMPLookupResultList results)
        {
            _timerCNMPLookupInterval = NativeTimerManager.StartTimeout(CNMP_LOOKUP_INTERVAL, OnCNMPLookupInterval);
        }

        private static void ValidRequestReceived(IPEndPoint ipEndpoint, CNMPLookupType lookupType, int keyId, string callerInstanceName, string callerType, IDictionary<string, string> extras)
        {
            //do nothing
        }

        private void DoCNMPLookup()
        {
            //if CNMP_LOOKUP_INTERVAL time runs out do not do lookup
            if (_continueLookup)
            {
                //lookup is done to inform the server (ValidRequestReceived method on server will be invoked)
                _cnmpAgent.BeginLookup(CNMPLookupType.Key, CNMP_CCU_LOOKUP_KEY);
            }
        }

        // ReSharper disable once NotAccessedField.Local
        private NativeTimer _timerCNMPLookupDuration;
        bool _continueLookup = true;

        private void StartCNMPLookupTimeout()
        {
            _timerCNMPLookupDuration = NativeTimerManager.StartTimeout(CNMP_LOOKUP_DURATION, OnCNMPLookupDurationFinished);
        }

        private bool OnCNMPLookupDurationFinished(NativeTimer nativeTimer)
        {
            _continueLookup = false;
            return true;
        }

        // ReSharper disable once NotAccessedField.Local
        private NativeTimer _timerCNMPLookupInterval;
        private bool OnCNMPLookupInterval(NativeTimer nativeTimer)
        {
            DoCNMPLookup();
            return true;
        }*/

        private const string TEMPORARY_CCU_UPGRADE_PATH = @"\NandFlash\TempUpgrade";

        private const string TEMPORARY_CCU_UPGRADER_PATH = TEMPORARY_CCU_UPGRADE_PATH + @"\CCUUpgrader";
        private const string CCU_UPGRADER_PATH = @"\NandFlash\CCUUpgrader";

        private const string CcuUpgraderExeName = "CCUUpgrader.exe";

        private const string TEMPORARY_CCU_UPGRADER_EXE_PATH =
            TEMPORARY_CCU_UPGRADER_PATH + StringConstants.BACKSLASH + CcuUpgraderExeName;
        private const string CCU_UPGRADER_EXE_PATH = CCU_UPGRADER_PATH + StringConstants.BACKSLASH + CcuUpgraderExeName;

        // ReSharper disable once UnusedParameter.Local
        private bool UnpackCCUGzToTemp(string filename, Stream stream, string folder)
        {
            var packer = new FilePacker();
            _lastProgressValue = 0;
            packer.UnpackingProgressChanged += packer_UpgradeProgressChanged;
            Exception ex;
            if (!packer.TryUnpack(stream, folder, out ex))
            {
                Log.Singleton.Error("An exception occured: " + ex.Message);
                return false;
            }
            return true;
        }

        private int _lastProgressValue;

        private void packer_UpgradeProgressChanged(int percentage)
        {
            if (percentage == 0 || percentage == 1 || percentage == UNPACK_PROGRESS_STEP || (percentage - _lastProgressValue) >= UNPACK_PROGRESS_STEP ||
            percentage == 99 || percentage == 100)
            {
                _lastProgressValue = percentage;
                Events.ProcessEvent(
                    new EventCcuUpgradeFileUnpackProgress(
                        percentage));
            }
        }

        private static Version GetTransferredCcuUpgraderVersion()
        {
            if (!File.Exists(TEMPORARY_CCU_UPGRADER_EXE_PATH)) return null;

            return FileVersionInfo.GetVersionInfo(TEMPORARY_CCU_UPGRADER_EXE_PATH);
        }

        private const string PathContalApplicationOrder = @"\NandFlash\contal.apploader.xml";
        private const string PathContalCat12ce = @"NandFlash\cat12ce\cat12ce.exe";

        #region Cat12 COMBO

        private volatile int _isCat12Combo = -1;
        private static volatile int _cat12ComboLicence = -1;
        private static readonly object _syncCat12Combo = new Object();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool GetCat12PresenceInAppOrder()
        {
            if (MainBoardVariantG7.CAT12CE != MainBoard.Variant)
                return false;

            if (File.Exists(PathContalApplicationOrder))
            {
                XmlReader xmlReader = null;
                try
                {
                    xmlReader = XmlReader.Create(PathContalApplicationOrder);

                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.Name == "filepath")
                            {
                                string filepathValue = xmlReader.ReadElementString();

                                filepathValue = filepathValue.Replace(
                                    StringConstants.SLASH,
                                    StringConstants.BACKSLASH);

                                if (filepathValue.IndexOf(
                                    PathContalCat12ce,
                                    StringComparison.InvariantCultureIgnoreCase)
                                    >= 0)
                                    return true;

                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
                finally
                {
                    if (xmlReader != null)
                        try
                        {
                            xmlReader.Close();
                        }
                        catch
                        {

                        }
                }

            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasLicence"></param>
        internal static void SetCat12ComboLicence(bool hasLicence)
        {
            lock (_syncCat12Combo)
            {
                var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CONTAL_PATH);
                if (registryKey != null)
                {
                    _cat12ComboLicence = hasLicence ? 1 : 0;
                    registryKey.SetValue(CAT12_COMBO_REG, _cat12ComboLicence);
                    registryKey.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static bool HasCat12ComboLicence()
        {
            if (_cat12ComboLicence < 0)
                lock (_syncCat12Combo)
                {
                    if (_cat12ComboLicence < 0)
                    {
                        var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CONTAL_PATH);
                        if (registryKey != null)
                        {
                            _cat12ComboLicence = (int)registryKey.GetValue(CAT12_COMBO_REG, 0);
                            registryKey.Close();
                        }
                    }
                }

            return _cat12ComboLicence > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsCat12Combo
        {
            get
            {
                if (_isCat12Combo < 0)
                    lock (_syncCat12Combo)
                        if (_isCat12Combo < 0)
                            _isCat12Combo = GetCat12PresenceInAppOrder() ? 1 : 0;

                return (_isCat12Combo == 1);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferredVersion"></param>
        /// <returns>true, if the CCUupgrader already present in nandflash is newest</returns>
        private static bool DoesCcuUpgradeNeedUpdate(Version transferredVersion)
        {
            try
            {
                if (File.Exists(CCU_UPGRADER_EXE_PATH) && (transferredVersion != null))
                {
                    var presentVersion = FileVersionInfo.GetVersionInfo(CCU_UPGRADER_EXE_PATH);

                    if (presentVersion <= transferredVersion)
                        return true;

                    if (Directory.Exists(TEMPORARY_CCU_UPGRADER_PATH))
                    {
                        Directory.Delete(TEMPORARY_CCU_UPGRADER_PATH, true);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);

            }

            return true;
        }

        public static bool ChecksumMatches(uint rightChecksum, string unpackFileName)
        {
            if (string.IsNullOrEmpty(unpackFileName))
                return false;

            var computedChecksum = ~rightChecksum; // if the operation of computing fails to catch, the result will be false for sure
            FileStream fs = null;
            try
            {
                fs = PatchedFileStream.Open(unpackFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                computedChecksum = Crc32.ComputeChecksum(BlockFile.FILE_STREAM_BUFFER_SIZE, fs);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (fs != null)
                    try { fs.Close(); }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }

            if (computedChecksum != rightChecksum)
                try
                {
                    File.Delete(unpackFileName);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            return computedChecksum == rightChecksum;

        }

        private static bool UpdateCCUUpgrader()
        {
            try
            {
                if (Directory.Exists(CCU_UPGRADER_PATH))
                {
                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            Log.Singleton.Info("Attempt " + (i + 1) + "/5 to remove " + CCU_UPGRADER_PATH + " directory");
                            Directory.Delete(CCU_UPGRADER_PATH, true);
                            break;
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                            Thread.Sleep(500);
                        }
                        if (i == 4)
                            Directory.Delete(CCU_UPGRADER_PATH, true);
                    }
                }
                if (Directory.Exists(TEMPORARY_CCU_UPGRADER_PATH))
                {
                    Directory.Move(TEMPORARY_CCU_UPGRADER_PATH, CCU_UPGRADER_PATH);
                }
                return true;

            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                return false;
            }
        }

        public void SoftRebootCCU()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "void CcuCore.SoftRebootCCU()");
            RunCcuUpgrader(new[]
            {
                "\"" + QuickPath.ApplicationStartupDirectory + "\\ccu.exe\"",
                "\"" + Process.GetCurrentProcess().Id + "\""
            });
        }

        private void RunCcuUpgrader(string[] arguments)
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = @"\NandFlash\CCUUpgrader\CCUUpgrader.exe",
                    UseShellExecute = true,
                    Arguments = string.Join(StringConstants.SPACE, arguments)
                }
            };
            p.Start();

            ExitCore();
        }

        private string[] GetArgumentsForUpgrade()
        {
            //Arguments are: file name, path to save extracted data, file to execute (supports absolute and relative path)

            return new[]
            {
                "\"" + RootPath.Substring(0, RootPath.Length - 1) + "\"",
                "\"" + QuickPath.ApplicationStartupDirectory + "\"",
                "\"ccu.exe\"",
                "\"" + Process.GetCurrentProcess().Id + "\""
            };
        }

        private static void CEUpgradeResult(CeUpgradeAction action, ActionResultUpgrade result, string version)
        {
            Events.ProcessEvent(
                new EventCeUpgradeFinished(
                    (int)action,
                    (int)result,
                    version ?? string.Empty));

            if (action == CeUpgradeAction.BeginUpgrade && result == ActionResultUpgrade.Success)
                CeKickstartControl.FinalizeUpgrade();
        }

        private static IPAddress[] GeSNTPIpAddresses(DB.CCU ccu)
        {
            var listStringIpAddresses = new List<string>();
            var listIpAddresses = new List<IPAddress>();

            if (ccu != null)
            {
                if (!string.IsNullOrEmpty(ccu.SNTPIpAddresses))
                {
                    var ipAddresses = ccu.SNTPIpAddresses;
                    var ipAddress = GetNextIpAddressHostName(ref ipAddresses);

                    while (ipAddress != string.Empty)
                    {
                        listStringIpAddresses.Add(ipAddress);
                        ipAddress = GetNextIpAddressHostName(ref ipAddresses);
                    }
                }
            }
            if (listStringIpAddresses.Count > 0)
            {
                foreach (var stringIpAddress in listStringIpAddresses)
                {
                    try
                    {
                        var ipAddress = IPAddress.Parse(stringIpAddress);
                        listIpAddresses.Add(ipAddress);
                    }
                    catch { }
                }
            }
            return listIpAddresses.ToArray();
        }

        private int GetPollingIntervalMinutes()
        {
            return Convert.ToInt32(_sendTimeInterval / 60000);
        }

        private int GetPollingIntervalSeconds()
        {
            return (int)((_sendTimeInterval % 60000 / 1000));
        }

        /// <summary>
        /// Returns whether time is synchronized from the server
        /// </summary>
        /// <returns></returns>
        private bool GetSyncingTimeFromServer(DB.CCU ccu)
        {
            if (ccu != null)
            {
                if (ccu.SyncingTimeFromServer != null)
                {
                    return ccu.SyncingTimeFromServer.Value;
                }

                return GetRegistryTimeSyncingFromServer();
            }

            return false;
        }

        private void StartSNTP(DB.CCU ccu, IPAddress[] ipAddresses)
        {
            // Not start sntp, if time is synchronized from Nova server
            if (GetSyncingTimeFromServer(ccu))
                return;

            if (ipAddresses == null || !(ipAddresses.Length > 0))
            {
#  if DEBUG
                SimpleSNTP.Singleton.IPAddresses = new[] { IPAddress.Parse("10.0.0.1") };
                SimpleSNTP.Singleton.AllowedTimeTolerance = 2000;
                SimpleSNTP.Singleton.PollingInterval = new TimeSpan(0, GetPollingIntervalMinutes(), GetPollingIntervalSeconds());
                SimpleSNTP.Singleton.SynchronizationErrorOccured += Singleton_SynchronizationErrorOccured;
                SimpleSNTP.Singleton.BindTimeSynchronizedEventWithPreviousTime(SNTPTimeSynchronizedEventWithPreviousTime);
                SimpleSNTP.Singleton.Start();
#endif
                return;
            }

            SimpleSNTP.Singleton.IPAddresses = ipAddresses;
            SimpleSNTP.Singleton.AllowedTimeTolerance = 2000;
            SimpleSNTP.Singleton.PollingInterval = new TimeSpan(0, GetPollingIntervalMinutes(), GetPollingIntervalSeconds());
            SimpleSNTP.Singleton.SynchronizationErrorOccured += Singleton_SynchronizationErrorOccured;
            SimpleSNTP.Singleton.BindTimeSynchronizedEventWithPreviousTime(SNTPTimeSynchronizedEventWithPreviousTime);
            SimpleSNTP.Singleton.Start();
        }

        private void RestartSNTP()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "SNTP restart");

            SimpleSNTP.Singleton.Stop();
            SimpleSNTP.Singleton.HostNames = null;
            SimpleSNTP.Singleton.SynchronizationErrorOccured -= Singleton_SynchronizationErrorOccured;
            SimpleSNTP.Singleton.RemoveTimeSynchronizedEventWithPreviousTime(SNTPTimeSynchronizedEventWithPreviousTime);

            DB.CCU ccu = Ccus.Singleton.GetCCU();
            StartSNTP(ccu, GeSNTPIpAddresses(ccu));
        }

        private static string GetNextIpAddressHostName(ref string ipAddressesHostNames)
        {
            if (!string.IsNullOrEmpty(ipAddressesHostNames))
            {
                var pos = ipAddressesHostNames.IndexOf(StringConstants.SEMICOLON, StringComparison.Ordinal);
                if (pos > 0)
                {
                    var ipAddresseHostName = ipAddressesHostNames.Substring(0, pos);
                    ipAddressesHostNames = ipAddressesHostNames.Substring(pos + 1);
                    return ipAddresseHostName;
                }
            }

            return string.Empty;
        }

        static void Singleton_SynchronizationErrorOccured(Exception inputError)
        {
            DebugLog.Warning(Log.LOW_LEVEL, "SNTP Error : " + inputError.Message);
        }

        private static void SNTPTimeSynchronizedEventWithPreviousTime(DateTime oldDateTiem, DateTime newDateTime)
        {
            Events.ProcessEvent(
                new EventCcuTimeAdjusted(
                    oldDateTiem,
                    newDateTime));
        }

        private void InitDateTimeSynchronization()
        {
            SafeThread.StartThread(SendTime);
        }

        private void SendTime()
        {
            var syncCycleCounter = 0;

            while (true)
            {
                ASafeThreadBase.Sleep((int)_sendTimeInterval, ref _safeThreadSleepCondition);

                if (!WasExited)
                {
                    if (SimpleSNTP.Singleton.TimeWasSynchronized || syncCycleCounter > 2)
                    {
                        syncCycleCounter = 0;
                        Events.ProcessEvent(
                            new EventCcuActualTimeSent(
                                UtcTime));

                        //if (SimpleSNTP.Singleton.TimeWasSynchronized)

                        var mbv = (MainBoardVariant)MainBoard.Variant;
                        if (mbv == MainBoardVariant.CCU0_ECHELON ||
                            mbv == MainBoardVariant.CCU0_RS485)
                        {
                            DCUs.Singleton.SetTimeOnCardReaders(); //code for CCU0
                        }
                        else
                        {
                            CcuCardReaders.ApplyTimeSettings();
                        }
                    }
                    else
                    {
                        syncCycleCounter++;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private ulong _ceUpTimeMilisecondsOnStartCCU;

        public readonly CeNetworkManagement CeNetworkManagement;

        private CcuCore()
            : base(null)
        {
            _ceImageVersion = ReadWinCeImageVersionFromFile();

            VersionsInfo = new VersionsInfo(
                _version.ToString(),
                _ceImageVersion);

            if (WindowsCE.Build >= LPC32xxConstants.CeVersionDatetimeNowFix)
                SystemTime.UseDateTimeNow = true;

            CeNetworkManagement = CeNetworkManagement.Obtain(
                LPC32xxConstants.EthernetInterfaceName,
                LPC32xxConstants.EthernetInterfaceNdisPort);


            if (!IsCat12Combo)
            {
                SetCat12ComboLicence(false);
            }
            else if (!HasCat12ComboLicence())
            {
                lock (_unconfigureTimerLock)
                {
                    if (_unconfigureTimer != null)
                    {
                        _unconfigureTimer.StopTimer();
                    }

                    _unconfigureTimer = TimerManager.Static.StartTimeout(
#if DEBUG
180000,
#else
                        180000,
#endif
 null,
                        timer =>
                        {
                            lock (_unconfigureTimerLock)
                            {
                                if (_unconfigureTimer != null)
                                    _unconfigureTimer.StopTimer();

                                _unconfigureTimer = null;
                            }

                            Unconfigure();

                            Log.Singleton.Warning(
                                "CCU unconfigured because of licence");

                            return true;
                        });
                }
            }

            _initializationThread = new SafeThread(Initialize, THREAD_NAME_INITIALIZE);
            _initializationThread.OnException += _thread_OnException;
            _initializationThread.Start();

            _watchdogThread = new SafeThread(SignalWatchdog, THREAD_NAME_SIGNAL_WATCHDOG);
            _watchdogThread.OnException += _thread_OnException;
            _watchdogThread.Start();


            //_queueApplyChanges.ItemProcessing += new Action<bool>(DoApplyChanges);
            CeKickstartControl.UpgradeResult += CEUpgradeResult;

            InitUptime();
        }

        static void _thread_OnException(Exception inputError)
        {
            HandledExceptionAdapter.Examine(inputError);
        }

        //private static Log _mainLog = null;
        //private static object _mainLogSync = new object();

        /// <summary>
        /// returns and/or instantiates main log of the CCU
        /// </summary>
        //public static Log MainLog
        //{
        //    get
        //    {
        //        if (null == _mainLog)
        //        {
        //            lock (_mainLogSync)
        //            {
        //                if (null == _mainLog)
        //                {
        //                    _mainLog = new Log("CCU", true, false, null, 0)
        //                    {
        //                        ShowThreadId = true
        //                    };
        //                }
        //            }
        //        }

        //        return _mainLog;
        //    }

        //}

        private static volatile Log _debugLog;
        private static readonly object _debugLogSync = new object();

        private static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
            return false;
#endif
            }
        }


        /// <summary>
        /// returns and/or instantiates debug log of the CCU
        /// </summary>
        public static Log DebugLog
        {
            get
            {
                if (null != _debugLog)
                    return _debugLog;

                lock (_debugLogSync)
                {
                    if (null != _debugLog) return _debugLog;

                    var debugLog = new Log("CCU", true, false, null, 0)
                    {
                        ShowThreadId = true,
                        VerbosityLevel = IsDebug ? (Log.NORMAL_LEVEL) : Log.PERFORMANCE_LEVEL
                    };

                    HandledExceptionAdapter.UseLogClass = debugLog;
                    HandledExceptionAdapter.ExceptionFilterTime = 1000;

                    try
                    {
                        var sdRootPathForLogging = STORAGE_CARD_PATH;
                        var sdPathWouldBeUsed = false;

                        try
                        {
                            // ensure existence of the CCU subdirectory
                            if (Directory.Exists(sdRootPathForLogging))
                            {
                                sdPathWouldBeUsed = true;

                                sdRootPathForLogging += CCU_SUBPATH;

                                if (!Directory.Exists(sdRootPathForLogging))
                                    Directory.CreateDirectory(sdRootPathForLogging);
                            }
                        }
                        catch (Exception dirEx)
                        {
                            debugLog.Error(string.Format("\tUnable to ensure SD root path for logging \"{0}\" :\r\n\t{1}", sdRootPathForLogging, dirEx));
                            sdPathWouldBeUsed = false;
                        }

                        if (sdPathWouldBeUsed)
                        {
                            var rfl = new RotaryFileLog(
                                sdRootPathForLogging + @"\ccu.debug.txt",
                                50,
                                128 * 1024, 10000);

                            debugLog.SetIntoFileLog(rfl);
                        }
                    }
                    catch (Exception error)
                    {
                        debugLog.Error("\tUnable to startup DEBUGLOG logging into SD card because of error :\r\n\t" + error, true, false);
                    }

                    _debugLog = debugLog;
                }

                return _debugLog;
            }
        }
        /*
        private void InitEchelon()
        { 
            try
            {
                EchelonControl.Reset();
                _log.Info("InitEchelon success");
            }
            catch
            {
                _log.Error("InitEchelon failed");
            }
        }*/

        private void InitUptime()
        {
            _ceUpTimeMilisecondsOnStartCCU = GetCEUpTime();

            uint lastUptimeReadTimestamp;
            uint tickCountOverflowCount;
            ReadValuesUptime(out lastUptimeReadTimestamp, out tickCountOverflowCount);

            SystemTime.InitUptimeMonitoring(lastUptimeReadTimestamp, tickCountOverflowCount, SaveValuesUptime);
        }

        private static void ReadValuesUptime(out uint lastUptimeReadTimestamp, out uint tickCountOverflowCount)
        {
            lastUptimeReadTimestamp = 0;
            tickCountOverflowCount = 0;
            Stream inputStream = null;
            try
            {
                if (File.Exists(TEMP_PATH + CCU_SUBPATH + TEMP + UPTIME_MONITORING_FILE_NAME))
                {
                    inputStream = PatchedFileStream.Open(
                        TEMP_PATH + CCU_SUBPATH + TEMP + UPTIME_MONITORING_FILE_NAME,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read);

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (inputStream != null && inputStream.Length > 0)
                    {
                        var buffer = new byte[sizeof(UInt32)];
                        var readedBytes = inputStream.Read(buffer, 0, buffer.Length);
                        if (readedBytes == buffer.Length)
                        {
                            lastUptimeReadTimestamp = BitConverter.ToUInt32(buffer, 0);
                        }

                        readedBytes = inputStream.Read(buffer, 0, buffer.Length);
                        if (readedBytes == buffer.Length)
                        {
                            tickCountOverflowCount = BitConverter.ToUInt32(buffer, 0);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (inputStream != null)
                {
                    try
                    {
                        inputStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }
        }

        private static void SaveValuesUptime(uint lastUptimeReadTimestamp, uint tickCountOverflowCount)
        {
            Stream outputStream = null;
            try
            {
                if (!Directory.Exists(TEMP_PATH + CCU_SUBPATH + TEMP))
                    Directory.CreateDirectory(TEMP_PATH + CCU_SUBPATH + TEMP);

                outputStream = PatchedFileStream.Open(
                    TEMP_PATH + CCU_SUBPATH + TEMP + UPTIME_MONITORING_FILE_NAME,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (outputStream != null)
                {
                    var buffer = BitConverter.GetBytes(lastUptimeReadTimestamp);
                    if (buffer != null)
                    {
                        outputStream.Write(buffer, 0, buffer.Length);
                    }

                    buffer = BitConverter.GetBytes(tickCountOverflowCount);
                    if (buffer != null)
                    {
                        outputStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (outputStream != null)
                {
                    try
                    {
                        outputStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }
        }

        private LwRemotingServer _remotingServer;

        /// <summary>
        /// 
        /// </summary>
        private void InitRemoting()
        {
            try
            {
                var aesSettings = new AESSettings(
                    WindowsCE.Build >= NCASConstants.NEW_AES_CE_VERSION_THRESHOLD,
                    LwRemotingGlobals.GetSHA1(LwRemotingGlobals.LWREMOTING_KEY),
                    LwRemotingGlobals.LWREMOTING_SALT,
                    AESKeySize.Size128);

                _remotingServer = new LwRemotingServer(aesSettings, NCASConstants.TCP_CCU_REMOTING_PORT);

                //_remotingServer = new LwRemotingServer(NCASConstants.TCP_CCU_REMOTING_PORT, typeof(DB.TimeZone).Assembly);

                _remotingServer.RegisterService(CcuCoreRemotingProvider.Singleton);
                _remotingServer.AutheticateFunction += CcuCoreRemotingProvider.AuthenticateClient;
                _remotingServer.GetPriorityFromMethodName += GetPriorityFromMethodName;
                _remotingServer.SendACK += SendACK;
                _remotingServer.ClientDisconnected += ClientDisconnected;

                DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "Remoting initialization succeeded");
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                DebugLog.Error(Log.CALM_LEVEL, () => "Remoting initialization failed");
            }
        }

        private class PriorityForMethodNames : AGetter<Dictionary<string, MethodNames>>
        {
            private PriorityForMethodNames([NotNull] Dictionary<string, MethodNames> defaultValue)
                : base(defaultValue)
            {
                EnumHelper.IterateAllValues<MethodNames>(
                    methodNameEnumValue =>
                        defaultValue[methodNameEnumValue.ToString()] = methodNameEnumValue);
            }

            protected override void GetOrModifyValue(ref Dictionary<string, MethodNames> valueToBePassedAndOrModified)
            {

            }

            protected override void DisposeValue(ref Dictionary<string, MethodNames> valueToBeDisposed, bool isExplicitDispose)
            {

            }

            private static readonly PriorityForMethodNames _singleton = new PriorityForMethodNames(new Dictionary<string, MethodNames>(150));

            internal static bool Parse([NotNull] string methodName, out MethodNames methodNameEnumValue)
            {
                return _singleton._value.TryGetValue(methodName, out methodNameEnumValue);
            }
        }


        private static uint GetPriorityFromMethodName([NotNull] string methodName)
        {
            MethodNames enumValueForMethodName;
            try
            {
                // mechanism to avoid ArgumentException of previous Enum.Parse solution
                if (!PriorityForMethodNames.Parse(methodName, out enumValueForMethodName))
                    return LwRemotingServer.DEFAULT_SENDING_PRIORITY;

                //name = (MethodNames)Enum.Parse(typeof(MethodNames), methodName, true);
            }
            catch
            {
                return LwRemotingServer.DEFAULT_SENDING_PRIORITY;
            }
            switch (enumValueForMethodName)
            {
                case MethodNames.Configure:
                case MethodNames.Upgrade:
                case MethodNames.UnsetUpgradeMode:
                case MethodNames.Unconfigure:
                case MethodNames.ConfigureForAnotherClient:
                case MethodNames.IsCcuConfiguredServerOnline:
                case MethodNames.Reset:
                case MethodNames.SoftReset:
                case MethodNames.ExistsPathOrFile:
                case MethodNames.GetIsConfigured:
                case MethodNames.ForceReconfiguration:
                case MethodNames.SendAllStatesToServer:
                case MethodNames.SaveObject:
                case MethodNames.ReadMaximumVersionAndIds:
                case MethodNames.SaveMaxObjectTypeVersion:
                case MethodNames.DeleteObject:
                case MethodNames.GetInputCount:
                case MethodNames.ApplyChanges:
                case MethodNames.GetMacAddress:
                case MethodNames.GetMainBoardType:
                case MethodNames.GetOutputCount:
                case MethodNames.GetDoorEnvironmentsCount:
                case MethodNames.DoorEnvironmentAccessGranted:
                case MethodNames.GetAlarmAreaActualState:
                case MethodNames.GetAlarmAreaActivationState:
                case MethodNames.SetAlarmArea:
                case MethodNames.UnconditionalSetAlarmArea:
                case MethodNames.UnsetAlarmArea:
                case MethodNames.ContainsDCUUpgradePackageVersion:
                case MethodNames.EnsureDirectory:
                case MethodNames.GetAvailableDCUUpgrades:
                case MethodNames.SetDefaultDCUUpgradeVersion:
                case MethodNames.FileExists:
                case MethodNames.UpgradeDCUs:
                case MethodNames.RegisterDCUsForUpgradeVersion:
                case MethodNames.RegisterCRsForUpgradeVersion:
                case MethodNames.GetIPSettings:
                case MethodNames.ResetDcu:
                case MethodNames.SetIPSettings:
                case MethodNames.IsCCU0:
                case MethodNames.IsCCUUpgrader:
                case MethodNames.GetCurrentTime:
                case MethodNames.AcknowledgeAllowed:
                case MethodNames.SaveConfigurePassword:
                case MethodNames.CompareConfigurePassword:
                case MethodNames.IsSetConfigurePassword:
                case MethodNames.AuthenticateClient:
                case MethodNames.WinCEImageVersion:
                case MethodNames.GetCcuFirmwareVersion:
                case MethodNames.GetInputState:
                case MethodNames.DeleteEvent:
                case MethodNames.DeleteEvents:
                case MethodNames.CommunicationStatistic:
                case MethodNames.ResetSended:
                case MethodNames.ResetDeserializationError:
                case MethodNames.ResetReceived:
                case MethodNames.ResetReceivedError:
                case MethodNames.ResetCommunicationStatistic:
                case MethodNames.ResetMsgRetry:
                case MethodNames.GetCcuStartsCount:
                case MethodNames.ResetCcuStartCounter:
                case MethodNames.CoprocessorBuildNumberStatistics:
                case MethodNames.ResultSimulationCardSwiped:
                case MethodNames.RequestDcuMemoryLoad:
                case MethodNames.DummyMethod:
                case MethodNames.CCUConfiguredStateChanged:
                case MethodNames.SetOldEventsSettings:
                case MethodNames.GetOtherStatistics:
                case MethodNames.ResetCardReader:
                case MethodNames.SetTimeManually:
                case MethodNames.DeleteAllEvents:
                case MethodNames.GetAllBlockFilesInfo:
                case MethodNames.ResetBlockFilesInfo:
                case MethodNames.SetMaxEventsCount:
                case MethodNames.SetTimeSyncingFromServer:
                case MethodNames.SetTimeFromServer:
                case MethodNames.GetThreadsInfo:
                case MethodNames.GetVerbosityLevelsInfo:
                case MethodNames.SetVerbosityLevel:
                case MethodNames.ResetCCUCommandTimeouts:
                case MethodNames.IsCat12Combo:
                case MethodNames.HasCat12ComboLicence:
                    return 10;
                case MethodNames.SaveEvent:
                case MethodNames.CcuCoreRemotingProviderSaveEvent:
                case MethodNames.SetNTPTimeDifferenceTolerance:
                    return 20;
                default:
                    return LwRemotingServer.DEFAULT_SENDING_PRIORITY;
            }
        }

        private static bool SendACK([NotNull] string methodName)
        {
            MethodNames methodNameEnumValue;
            try
            {
                if (!PriorityForMethodNames.Parse(methodName, out methodNameEnumValue))
                    return true;

                //name = (MethodNames)Enum.Parse(typeof(MethodNames), methodName, true);
            }
            catch
            {
                return true;
            }
            switch (methodNameEnumValue)
            {
                case MethodNames.DeleteEvents:
                    return false;
                default:
                    return true;
            }
        }

        private IPEndPoint _authenticatedConnection;

        public IPAddress ServerIpAddress
        {
            get
            {
                return _authenticatedConnection == null ? null : _authenticatedConnection.Address;
            }
        }

        private void ClientDisconnected(ISimpleTcpConnection connection)
        {
            if (_authenticatedConnection != null && _authenticatedConnection.Equals(connection.RemoteEndPoint))
            {
                _authenticatedConnection = null;
                SendEventsToServerDispatcher.Singleton.SetAutonomousRun(true);
                AlarmsManager.Singleton.ServerDisconnected();
                Ccus.Singleton.WatchdogOutputSetToOn();
            }
        }

        public void ClientAuthenticated(ISimpleTcpConnection connection)
        {
            if (_authenticatedConnection == null)
            {
                _authenticatedConnection = connection.RemoteEndPoint;
                _dataChannelPeer.AllowIP(_authenticatedConnection.Address);
                SendEventsToServerDispatcher.Singleton.SetAutonomousRun(false);
                AlarmsManager.Singleton.ServerConnected();
                Ccus.Singleton.WatchdogOutputSetToOff();
            }
        }

        public bool IsAuthenticatedClientConnected()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.IsAuthenticatedClientConnected()");
            if (CCUConfigurationState.Singleton.IsConfigured && _authenticatedConnection != null)
                return true;

            return false;
        }

        private void SignalWatchdog()
        {
#if ! DEBUG
            // SIMPLE WATCHDOG CONTROL
            int pid = Process.GetCurrentProcess().Id;
            // use process id instead of thread id
            WatchdogControl.SetRebootTimeout(pid, 240000);   // make initial reboot timeout longer

            int iterations = 0;
            bool rebootTimeoutReSetAfterPeriod = false;
#endif

            Thread.CurrentThread.Priority = ThreadPriority.Normal;

            while (!WasExited)
            {
                try
                {
#if ! DEBUG
                    WatchdogControl.SignalWatchdog(pid);

                    iterations++;

                    if (iterations == 12 && !rebootTimeoutReSetAfterPeriod) // after about 2 minute run, switch to 180s
                    {
                        WatchdogControl.SetRebootTimeout(pid, 180000);
                        rebootTimeoutReSetAfterPeriod = true;
                    }
#endif
                    CCUMemoryLoadCheck();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                ASafeThreadBase.Sleep(10000, ref _safeThreadSleepCondition);
            }
        }

        private void EnableCCUMemoryLoadCheck()
        {
            _enabledCCUMemoryLoadCheck = true;
        }

        private int _countCR;
        private void Initialize()
        {
            LocalizationHelper = new LocalizationHelper(GetType().Assembly);

            DebugLog.VerbosityLevel = GetRegistryLogVerbosityLevel();
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "CcuCore.MainThread()");

            TypeCache.LoadAssemblies(
                typeof(ObjectType).Assembly,
                typeof(TimeZone).Assembly,
                typeof(LwRemotingMessage).Assembly,
                typeof(BlockFileInfo).Assembly);

            //Events.Singleton.RunTest(); //test
            SendEventsToServerDispatcher.Singleton.Init();
            Events.AddEventDispatcher(SendEventsToServerDispatcher.Singleton);

            EnableCCUMemoryLoadCheck();

            HandledExceptionAdapter.ExceptionOccured +=
                (exception, dateTime, threadId) =>
                    Events.ProcessEvent(
                        new EventExceptionOccured(
                            dateTime,
                            exception,
                            threadId));

            PatchedFileStream.ErrorOccured +=
                (fileName, fileOperation, error) =>
                {
                    var idCcu = Ccus.Singleton.GetCcuId();

                    if (idCcu != null)
                    {
                        if (error == null)
                            AlarmsManager.Singleton.StopAlarm(
                                CcuFileSystemProblemAlarm.CreateAlarmKey(
                                    idCcu.Value,
                                    fileName));
                        else
                            AlarmsManager.Singleton.AddAlarm(
                                new CcuFileSystemProblemAlarm(
                                    idCcu.Value,
                                    fileName,
                                    fileOperation.ToString()));
                    }

                    Events.ProcessEvent(
                        new EventCcuFilesystemProblem(
                            error,
                            fileName,
                            fileOperation.ToString()));
                };

            try
            {
                DCUs.Singleton.Initialize();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            try
            {
                Database.ConfigObjectsEngine.ReadObjectsFromFiles();
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);

                return;
            }

            RegisterTimeChangeEvent();

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            // I found out the SNTP server is started again from ApplyChanges() so it is redundant to call it here
            //StartSNTP(ccu, GeSNTPIpAddresses(ccu));            

            var mbv = (MainBoardVariant)MainBoard.Variant;
            if (mbv != MainBoardVariant.CCU0_ECHELON &&
                mbv != MainBoardVariant.CCU0_RS485)
            {
                if (ccu != null)
                {
                    _countCR = ccu.MaxNodeLookupSequence;
                    ValidateMaxCrCount(ref _countCR);
                }
                CcuCardReaders.InitCardReaders(_countCR);
            }

            //InitCNMP();

            CcuSpecialInputs.Singleton.RegisterSpecialInputChanged();

            Events.AddEventDispatcher(EventsForCardReadersDispatcher.Singleton);

            CatAlarmsManager.Singleton.Init();
            AlarmsManager.Singleton.Init();

            CardSystemData.Singleton.Init();

            ApplyChanges();

            InterCcuCommunication.Singleton.LoadFromFile();

            try
            {
                if (Database.ConfigObjectsEngine.ContainsAnyObjects(ObjectType.CCU))
                    DCUs.Singleton.Start();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            EventsForCardReadersDispatcher.Singleton.ReadOldEvents();

            bool sdCardPresent = Program.SDCardPresent;

            Events.ProcessEvent(
                new EventCcuSdCardNotFound(
                    sdCardPresent));

            if (ccu != null)
            {
                if (sdCardPresent)
                    AlarmsManager.Singleton.StopAlarm(
                        CcuSdCardNotFoundAlarm.CreateAlarmKey(
                            ccu.IdCCU));
                else
                    AlarmsManager.Singleton.AddAlarm(
                        new CcuSdCardNotFoundAlarm(
                            ccu.IdCCU));
            }

            Ccus.Singleton.WatchdogOutputSetToOn();

            RemoveCEFiles();

            InitTCPDataChannel();

            InitRemoting();

            InitDateTimeSynchronization();

            DCUs.Singleton.SetNodeCommunicatorPolling(true);

            IncrementCcuStartCounter();

            UPSMonitor.Singleton.InitUpsMonitor();

            AlarmAreas.Singleton.ReadAlarmAreaTimeBuyingStates();

            AntiPassBackZones.Singleton.LoadCards();
        }

        private void CCUMemoryLoadCheck()
        {
            if (!_enabledCCUMemoryLoadCheck)
                return;

            lock (_lockHighCCUMemoryLoad)
            {
                var idCcu = Ccus.Singleton.GetCcuId();

                if (idCcu != null)
                {
                    var isHighCcuMemoryLoad = SystemInfo.Singleton.MemoryLoad > NCASConstants.CCU_MEMORY_LOAD_TRESHOLD;

                    if (_highCCUMemoryLoad == null || _highCCUMemoryLoad.Value != isHighCcuMemoryLoad)
                    {
                        _highCCUMemoryLoad = isHighCcuMemoryLoad;

                        if (!isHighCcuMemoryLoad)
                            AlarmsManager.Singleton.StopAlarm(
                                CcuHighMemoryLoadAlarm.CreateAlarmKey(
                                    idCcu.Value));
                        else
                            AlarmsManager.Singleton.AddAlarm(
                                new CcuHighMemoryLoadAlarm(
                                    idCcu.Value));

                        Events.ProcessEvent(
                            new EventCcuMemoryLoadStateChanged(
                                isHighCcuMemoryLoad));
                    }
                }
            }
        }

        public string[] GetDebugFiles()
        {
            var files = Directory.GetFiles(@"\Storage Card\CCU");

            return files.Where(
                file => file.Contains("ccu.debug.")).ToArray();
        }

        private void InitTCPDataChannel()
        {
            _dataChannelPeer = new DataChannelPeer();
            _dataChannelPeer.StartListening(null, NCASConstants.TCP_DATA_CHANNEL_PORT, true);
            _dataChannelPeer.IncomingTransferResult += _dataChannelPeer_IncomingTransferResult;
        }

        private void _dataChannelPeer_IncomingTransferResult(DataChannelPeer.TransferInfo transferInfo, bool success, out bool closeStreamAfterProcessing)
        {
            DebugLog.Info(
                Log.ABOVE_NORMAL_LEVEL,
                () =>
                    string.Format(
                        "Data channel peer received: {0}, name: {1}",
                        success,
                        transferInfo.DestinationName));

            closeStreamAfterProcessing = false;
            if (transferInfo == null ||
                transferInfo.Success == false)
                return;

            if (transferInfo.DestinationStream == null)
                return;

            try
            {
                transferInfo.DestinationStream.Position = 0;
            }
            catch (Exception)
            {
                return;
            }

            Events.ProcessEvent(
                new EventCcuIncomingTransferInfo(
                    transferInfo.DestinationName,
                    transferInfo.TransferDurationPure));

            Exception e;

            string[] headerParameters =
                FilePacker.TryGetHeaderParameters(transferInfo.DestinationStream, out e);

            if (headerParameters != null &&
                headerParameters.Length > 0
                &&
                headerParameters[0].ToLower()
                    .Equals(
                        DeviceType.DCU.ToString()
                            .ToLower()))
            {
                UnpackPackageFailedCode errorCode;
                byte[] nodesWaiting;
                if (
                    !DCUs.Singleton.ProcessUpgradePackage(
                        transferInfo.DestinationName,
                        transferInfo.DestinationStream,
                        headerParameters,
                        out errorCode,
                        out nodesWaiting))
                {
                    Events.ProcessEvent(
                        new EventProcessDcuUpgradePackageFailed(
                            nodesWaiting,
                            (byte)errorCode));
                }
                closeStreamAfterProcessing = true;
            }
            else
                if (headerParameters != null &&
                    headerParameters.Length > 0
                    &&
                    headerParameters[0].ToLower()
                        .Equals(
                            DeviceType.CR.ToString()
                                .ToLower()))
                {
                    var failureInfo =
                        CardReaderUpgradeProcess.UpgradeCardReaders(
                            transferInfo.DestinationName,
                            transferInfo.DestinationStream,
                            headerParameters);

                    if (failureInfo != null
                        && failureInfo.CrAddresses != null)
                    {
                        Events.ProcessEvent(
                            new EventProcessCrUpgradePackageFailed(
                                failureInfo.CrAddresses,
                                (byte)failureInfo.ErrorCode));
                    }

                    closeStreamAfterProcessing = true;
                }
                else
                    if (PassCeUpgradeFileCondition(Path.GetFileName(transferInfo.DestinationName)))
                    {
                        try
                        {
                            if (transferInfo.DestinationStream != null)
                                transferInfo.DestinationStream.Close();

                            CeKickstartControl.BeginUpgrade(transferInfo.DestinationName);
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                            Events.ProcessEvent(
                                new EventCeUpgradeFinished(
                                    (int)CeUpgradeAction.BeginUpgrade,
                                    (int)ActionResultUpgrade.Unkonwn,
                                    string.Empty));
                        }
                    }
                    else
                        if (headerParameters != null &&
                            headerParameters.Length > 0
                            &&
                            headerParameters[0].ToLower()
                                .Equals(
                                    DeviceType.CCU.ToString()
                                        .ToLower()))
                        {
                            try
                            {
                                if (UnpackCCUGzToTemp(
                                    transferInfo.DestinationName,
                                    transferInfo.DestinationStream,
                                    TEMPORARY_CCU_UPGRADE_PATH))
                                {
                                    transferInfo.DestinationStream.Close();
                                    File.Delete(transferInfo.DestinationName);

                                    var transferredVersion = GetTransferredCcuUpgraderVersion();
                                    if (!DoesCcuUpgradeNeedUpdate(transferredVersion))
                                    {
                                        DebugLog.Info(
                                            Log.BELOW_NORMAL_LEVEL,
                                            () => "CCUUpgrader exists and does not need an update");
                                        RunCcuUpgrader(GetArgumentsForUpgrade());
                                    }
                                    else
                                    {
                                        if (UpdateCCUUpgrader())
                                        {
                                            DebugLog.Info(
                                                Log.LOW_LEVEL,
                                                () => "CCUUpgrader was updated");
                                            RunCcuUpgrader(GetArgumentsForUpgrade());
                                        }
                                    }
                                }
                                else
                                {
                                    transferInfo.DestinationStream.Close();
                                    File.Delete(transferInfo.DestinationName);

                                    DebugLog.Error(
                                        Log.LOW_LEVEL,
                                        () => "An exception occured while trying to unpack gz file");
                                    Events.ProcessEvent(
                                        new EventCcuUpgraderStartFailed());
                                }
                            }
                            catch (Exception ex)
                            {
                                HandledExceptionAdapter.Examine(ex);
                                DebugLog.Error(
                                    Log.LOW_LEVEL,
                                    () =>
                                        "An exception occured while trying to start CCUUpgrader: " +
                                        ex.Message);
                                Events.ProcessEvent(
                                    new EventCcuUpgraderStartFailed());
                            }
                            finally
                            {
                                closeStreamAfterProcessing = true;
                            }
                        }
        }


        private static void RegisterTimeChangeEvent()
        {
            if (SystemTime.RegisterTimeChangedEvent(SystemTime_TimeChanged))
            {
                Log.Singleton.Info("TimeChangeEvent registration succeeded");
            }
            else
            {
                Log.Singleton.Error("TimeChangeEvent registration failed");
            }
        }

        static void SystemTime_TimeChanged(DateTime currentDateTime)
        {
#if DEBUG
            Log.Singleton.Error("Current time: " + LocalTime.ToString("dd.MM.yyy HH:mm:ss") +
                " Time changed: [" + currentDateTime.ToString("dd.MM.yyy HH:mm:ss") + "]");
#endif
        }

        //private const string STORAGE_CARD_PATH = @"\Storage Card";
        private static void RemoveCEFiles()
        {
            try
            {
                if (Directory.Exists(STORAGE_CARD_PATH))
                {
                    foreach (var fileName in Directory.GetFiles(STORAGE_CARD_PATH))
                    {
                        if (Path.GetExtension(fileName) == ".bin")
                            try
                            {
                                File.Delete(fileName);
                            }
                            catch (Exception ex)
                            {
                                HandledExceptionAdapter.Examine(ex);
                            }
                    }
                    if (File.Exists(CcuCoreRemotingProvider.SIGNATURE_FILE))
                    {
                        try
                        {
                            File.Delete(CcuCoreRemotingProvider.SIGNATURE_FILE);
                        }
                        catch (Exception ex)
                        {
                            HandledExceptionAdapter.Examine(ex);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public event DVoid2Void BeforeExit;

        private void ExitCore()
        {
            _exitMutex.Set();
            //_tftpServer.Stop();
            //_remotingServer.Stop();
        }

        public void WaitForExit()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "void CcuCore.WaitForExit()");
            //Thread.Sleep(40000);
            //SendEventLog("Start CCU", "CCU", "CCU", "none", "description");
            //SendAlarm("CCU Core", "identificate", AlarmStates.Alarm, "Alarm 1");
            //List<Guid> cr =  ACLSetting.GetCR("01125");
            _exitMutex.WaitOne();

            _safeThreadSleepCondition = false;

            try
            {
                _remotingServer.Stop();
                GlobalEventIds.SaveActualEventIdBeforeExitCcu();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            if (BeforeExit != null)
            {
                try
                {
                    BeforeExit();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public bool WasExited
        {
            get { return _exitMutex.WaitOne(0, false); }
        }

        public void Sleep(int milisecondsTimeout)
        {
            _exitMutex.WaitOne(milisecondsTimeout, false);
        }

        private readonly TableOfVariables _tableVariables = new TableOfVariables();
        public TableOfVariables TableVariables
        {
            get { return _tableVariables; }
            //set { _tableVariables = value }
        }

        public void ChangeStateDailyPlan(DB.DailyPlan dailyPlan, bool state)
        {
        }

        public void ChangeStateTimeZone(TimeZone timeZone, bool state)
        {
        }

        /// <summary>
        /// Send tamper to CCU
        /// </summary>
        /// <param name="tamperChanged"></param>
        /// <param name="isTamper"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="saveEventlog">if true, event sent to server's eventlog</param>
        public void SendTamper(
            bool tamperChanged,
            bool isTamper,
            Guid objectId,
            ObjectType objectType,
            bool saveEventlog)
        {
            if (tamperChanged)
            {
                UpdateTamperAlarm(
                    objectId,
                    objectType,
                    isTamper);

                if (saveEventlog)
                {
                    Events.ProcessEvent(
                        new EventSendTamper(
                            objectId,
                            objectType,
                            isTamper));

                    return;
                }
            }

            if (objectType != ObjectType.CardReader)
                return;

            Events.ProcessEvent(
                new EventTamperInfo(
                    objectId,
                    objectType,
                    isTamper));
        }

        private static void UpdateTamperAlarm(
            Guid objectId,
            ObjectType objectType,
            bool isAlarm)
        {
            switch (objectType)
            {
                case ObjectType.CCU:
                    if (!isAlarm)
                        AlarmsManager.Singleton.StopAlarm(
                            CcuTamperAlarm.CreateAlarmKey(
                                objectId));
                    else
                        AlarmsManager.Singleton.AddAlarm(
                            new CcuTamperAlarm(
                                objectId));

                    break;

                case ObjectType.DCU:
                    if (!isAlarm)
                        AlarmsManager.Singleton.StopAlarm(
                            DcuTamperAlarm.CreateAlarmKey(
                                objectId));
                    else
                        AlarmsManager.Singleton.AddAlarm(
                            new DcuTamperAlarm(
                                objectId));

                    break;

                case ObjectType.CardReader:
                    if (!isAlarm)
                        AlarmsManager.Singleton.StopAlarm(
                            CrTamperAlarm.CreateAlarmKey(
                                objectId));
                    else
                        AlarmsManager.Singleton.AddAlarm(
                            new CrTamperAlarm(
                                objectId));

                    break;
            }
        }

        public void ApplyChanges()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "void CcuCore.ApplyChanges()");

            var restartSNTP = false;
            DB.CCU ccu = null;

            try
            {
                var idChangedCardReaders =
                    Database.ConfigObjectsEngine
                        .GetIdsOfRecentlySavedObjects(ObjectType.CardReader);

                if (idChangedCardReaders != null)
                    CardReaders.Singleton.Create(idChangedCardReaders);

                var doorEnvironmentGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.DoorEnvironment);

                if (doorEnvironmentGuids != null)
                    DoorEnvironments.Singleton.Create(doorEnvironmentGuids);

                var multiDoorGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.MultiDoor);

                if (multiDoorGuids != null)
                    MultiDoors.Singleton.Create(multiDoorGuids);

                var dcuGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.DCU);

                if (dcuGuids != null)
                    DCUs.Singleton.Create(dcuGuids);

                var inputGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.Input);

                if (inputGuids != null)
                    Inputs.Singleton.Create(inputGuids);

                var outputGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.Output);

                if (outputGuids != null)
                    Outputs.Singleton.Create(outputGuids);

                var alarmAreaGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.AlarmArea);

                if (alarmAreaGuids != null)
                    AlarmAreas.Singleton.Create(alarmAreaGuids);

                //Cards.Singleton.SaveToFile();
                var devicesAlarmSettingGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.DevicesAlarmSetting);

                if (devicesAlarmSettingGuids != null && devicesAlarmSettingGuids.Count > 0)
                {
                    DevicesAlarmSettings.Singleton.LoadDevicesAlarmSettings();
                }

                var ccuGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.CCU);

                if (ccuGuids != null && ccuGuids.Count > 0)
                {
                    restartSNTP = true;
                    Ccus.Singleton.Configure();

                    CcuSpecialInputs.Singleton.SetSpecialOutputs();
                }

                if (alarmAreaGuids != null)
                    AlarmAreas.Singleton.Configure(alarmAreaGuids);

                EventsForCardReadersDispatcher.Singleton
                    .UpdateAlarmAreaConfiguration(alarmAreaGuids);

                EventsForCardReadersDispatcher.Singleton.MaxCountOfEventsForAlarmArea =
                    Ccus.Singleton.GetCrEventLogSize();

                EventsForCardReadersDispatcher.Singleton.AlarmAreaMarkingDuration =
                    Ccus.Singleton.GetTimeForMarkAlarArea();

                if (dcuGuids != null)
                    DCUs.Singleton.Configure(dcuGuids);

                if (inputGuids != null)
                    Inputs.Singleton.Configure(inputGuids);

                if (outputGuids != null)
                    Outputs.Singleton.Configure(outputGuids);

                ApplyChangesForCardReaders(idChangedCardReaders);

                var cardSystemGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.CardSystem);

                if (cardSystemGuids != null)
                {
                    CardSystemData.Singleton.CardSystemChanged(cardSystemGuids);

                    foreach (var idCs in cardSystemGuids)
                        Events.ProcessEvent(
                            new EventSectorCardSystemAdded(
                                idCs));
                }

                if (doorEnvironmentGuids != null)
                    DoorEnvironments.Singleton.Configure(doorEnvironmentGuids);

                SecurityDailyPlans.Singleton.AddSecurityDailyPlans(
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.SecurityDailyPlan));

                var securityTimeZoneGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.SecurityTimeZone);

                SecurityTimeZones.Singleton.AddSecurityTimeZones(securityTimeZoneGuids);
                if (securityTimeZoneGuids == null || securityTimeZoneGuids.Count == 0)
                {
                    var securityTimeZoneDateSettingGuids =
                        Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.SecurityTimeZoneDateSetting);
                    var calendarGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.Calendar);

                    if ((securityTimeZoneDateSettingGuids != null && securityTimeZoneDateSettingGuids.Count > 0) ||
                        (calendarGuids != null && calendarGuids.Count > 0))
                    {
                        SecurityTimeZones.Singleton.OnTimeout(LocalTime);
                    }
                }

                DailyPlans.Singleton.AddDailyPlans(
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.DailyPlan));

                var timeZoneGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.TimeZone);
                TimeZones.Singleton.AddTimeZones(timeZoneGuids);
                if (timeZoneGuids == null || timeZoneGuids.Count == 0)
                {
                    var timeZoneDateSettingGuids =
                        Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.TimeZoneDateSetting);
                    var calendarGuids = Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.Calendar);

                    if ((timeZoneDateSettingGuids != null && timeZoneDateSettingGuids.Count > 0) ||
                        (calendarGuids != null && calendarGuids.Count > 0))
                    {
                        TimeZones.Singleton.OnTimeout();
                    }
                }

                var antiPassBackZoneGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.AntiPassBackZone);

                if (antiPassBackZoneGuids != null)
                    foreach (var guidAntiPassBackZone in antiPassBackZoneGuids)
                        AntiPassBackZones.Singleton.AddAntiPassBackZone(guidAntiPassBackZone);

                if (multiDoorGuids != null)
                    MultiDoors.Singleton.Configure(multiDoorGuids);

                var multiDoorElementsGuids =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.MultiDoorElement);

                if (multiDoorElementsGuids != null)
                    foreach (var guidMultiDoorElement in multiDoorElementsGuids)
                        MultiDoorElements.Singleton.AddMultiDoorElement(guidMultiDoorElement);

                var alarmTransmittersIds =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.AlarmTransmitter);

                if (alarmTransmittersIds != null)
                    foreach (var idAlarmTransmitter in alarmTransmittersIds)
                        AlarmTransmitters.Singleton
                            .ValidateCat(idAlarmTransmitter);

                var cardIds =
                    Database.ConfigObjectsEngine.GetIdsOfRecentlySavedObjects(ObjectType.Card);

                if (cardIds != null)
                    CardPinAccessManager.Singleton.Configure(cardIds);

                LogicalConfigurator.LogicalConfigurator.Singleton.Configure(
                    (dcuGuids != null
                        ? dcuGuids.Select(
                            idDcu =>
                                new IdAndObjectType(
                                    idDcu,
                                    ObjectType.DCU))
                        : Enumerable.Empty<IdAndObjectType>())
                    .Concat(
                        alarmAreaGuids != null
                            ? alarmAreaGuids.Select(
                                idAlarmArea =>
                                    new IdAndObjectType(
                                        idAlarmArea,
                                        ObjectType.AlarmArea))
                            : Enumerable.Empty<IdAndObjectType>()));

                Database.ConfigObjectsEngine.OnApplyChangesDone();

                ccu = Ccus.Singleton.GetCCU();
                DCUs.Singleton.SetMaxNodeLookupSequence(ccu);

                //CcuCore.Singleton.StartRPN();
                // after everything is set up, trigger the inputs and outputs changes
                IOControl.FlushInitalEvents();

                if (restartSNTP)
                {
                    RestartSNTP();
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                throw;
            }

            var mbv = (MainBoardVariant)MainBoard.Variant;
            if (mbv == MainBoardVariant.CCU40 ||
                mbv == MainBoardVariant.CCU12 ||
                mbv == MainBoardVariant.CCU05 ||
                mbv == MainBoardVariant.CAT12CE)
            {
                if (ccu != null)
                {
                    if (_countCR != ccu.MaxNodeLookupSequence)
                    {
                        _countCR = ccu.MaxNodeLookupSequence;

                        ValidateMaxCrCount(ref _countCR);

                        CcuCardReaders.StopCRCommunicator();
                        CcuCardReaders.InitCardReaders(_countCR);
                    }
                }

                CcuCardReaders.StartCRCommunicator();
            }

            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "ApplyChanges END");
        }

        private static void ApplyChangesForCardReaders(ICollection<Guid> idChangedCardReaders)
        {
            var idChangedAaCardReaders =
                Database.ConfigObjectsEngine
                    .GetIdsOfRecentlySavedObjects(ObjectType.AACardReader);

            var idCardReadersWithChangedAAs =
                new Dictionary<Guid, ICollection<DB.AACardReader>>();

            if (idChangedAaCardReaders != null)
                foreach (var aaCardReaderGuid in idChangedAaCardReaders)
                {
                    var aaCardReader =
                        Database.ConfigObjectsEngine.GetFromDatabase(
                            ObjectType.AACardReader,
                            aaCardReaderGuid) as
                            DB.AACardReader;

                    if (aaCardReader == null)
                        continue;

                    var guidCardReader = aaCardReader.GuidCardReader;

                    if (!CardReaders.Singleton.ObjectWithIdExists(guidCardReader))
                        continue;

                    ICollection<DB.AACardReader> aaCardReaders;

                    if (!idCardReadersWithChangedAAs.TryGetValue(
                        guidCardReader,
                        out aaCardReaders))
                    {
                        aaCardReaders = new LinkedList<DB.AACardReader>();
                        idCardReadersWithChangedAAs.Add(guidCardReader, aaCardReaders);
                    }

                    aaCardReaders.Add(aaCardReader);
                }

            if (idChangedCardReaders != null)
                CardReaders.Singleton.Configure(idChangedCardReaders);

            foreach (var idCardReaderWithChangedAAs in idCardReadersWithChangedAAs)
                CardReaders.Singleton.ConfigureAaCardReaders(
                    idCardReaderWithChangedAAs.Key,
                    idCardReaderWithChangedAAs.Value);
        }

        // just temporary solution
        public static void ValidateMaxCrCount(ref int crCount)
        {
            switch ((MainBoardVariant)MainBoard.Variant)
            {
                case MainBoardVariant.CCU05:
                    if (crCount > NCASConstants.CCU05_MAX_CR_COUNT)
                        crCount = NCASConstants.CCU05_MAX_CR_COUNT;
                    break;
                case MainBoardVariant.CCU12:
                    if (crCount > NCASConstants.CCU12_MAX_CR_COUNT)
                        crCount = NCASConstants.CCU12_MAX_CR_COUNT;
                    break;
                case MainBoardVariant.CCU40:
                    if (crCount > NCASConstants.CCU40_MAX_CR_COUNT)
                        crCount = NCASConstants.CCU40_MAX_CR_COUNT;
                    break;
                case MainBoardVariant.CAT12CE:
                    if (crCount > NCASConstants.CAT12CE_MAX_CR_COUNT)
                        crCount = NCASConstants.CAT12CE_MAX_CR_COUNT;
                    break;
                default:
                    if (crCount > 0)
                        crCount = 0;
                    break;
            }
        }

        //public AlarmAreaSetUnsetSecurityLevel SecurityLevelForEventlogsInCR;
        //public string EventlogsInCrGin = string.Empty;

        //private void GetSecurityLevelForCREventlogs()
        //{
        //    SecurityLevelForEventlogsInCR = Ccus.Singleton.GetCCU().SecurityLevel;
        //    EventlogsInCrGin = Ccus.Singleton.GetCCU().GIN;
        //}

        public void Unconfigure()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "void CcuCore.Unconfigure()");

            DeleteAllFromDatabase();
            CCUConfigurationState.Singleton.UnsetServer();

            _authenticatedConnection = null;

            AlarmAreas.Singleton.DeleteAllAlarmAreasAndTimeBuyingStates();

            SetCat12ComboLicence(false);
        }

        public void TryStopUnconfigureTimer()
        {
            lock (_unconfigureTimerLock)
            {
                if (_unconfigureTimer != null)
                    _unconfigureTimer.StopTimer();

                _unconfigureTimer = null;
            }
        }

        public void DeleteAllFromDatabase()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "void CcuCore.DeleteAllFromDatabase()");
            try
            {
                Database.ConfigObjectsEngine.DeleteAllFromDatabase();
                Outputs.Singleton.ClearOutputsChanged();
                SendEventsToServerDispatcher.Singleton.DeleteEvents();
                EventsForCardReadersDispatcher.Singleton.DeleteCrEventlogs();
                BlockedAlarmsManager.Singleton.Unconfigure();
                DevicesAlarmSettings.Singleton.Unconfigure();

                GlobalEventIds.Reset();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public BlockFileInfo[] GetBlockFilesInfo()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "BlockFileInfo[] CcuCore.GetBlockFilesInfo()");

            var result =
                _blockFileInfoProviders
                    .Select(blockFileInfoProvider => blockFileInfoProvider.BlockFileInfo)
                    .ToArray();

            DebugLog.Info(
                Log.PERFORMANCE_LEVEL,
                () => string.Format(
                    "BlockFileInfo[] CcuCore.GetBlockFilesInfo return {0}",
                    // ReSharper disable once CoVariantArrayConversion
                    Log.GetStringFromParameters(result)));

            return result;
        }

        public void SendAllStatesToServer()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.SendAllStatesToServer()");

            //SendEventsToServerDispatcher.Singleton.SaveEventTest();

            var isNewlyConfigured = CCUConfigurationState.Singleton.IsNewlyConfigured;

            DCUs.Singleton.SendAllStates(isNewlyConfigured);

            CardReaders.Singleton.SendAllStates(isNewlyConfigured);

            if (isNewlyConfigured)
                CCUConfigurationState.Singleton.ResetNewlyConfigured();

            Inputs.Singleton.SendAllStates();

            Outputs.Singleton.SendAllLogicalStates();

            Outputs.Singleton.SendAllRealStates();

            AlarmAreas.Singleton.SendAllStates();

            DoorEnvironments.Singleton.SendAllStates();

            MultiDoors.Singleton.SendAllStates();

            Tampers.SendAllTampers();
        }

        public bool StartUpgradeMode()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.StartUpgradeMode()");

            if (CCUConfigurationState.Singleton.IsUpgrading)
            {
                DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.StartUpgradeMode return true[1]");
                return true;
            }

            try
            {
                CCUConfigurationState.Singleton.SetUpgrading();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                DebugLog.Error(Log.CALM_LEVEL, () => "bool CcuCore.StartUpgradeMode return false");
                return false;
            }
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.StartUpgradeMode return true[2]");
            return true;
        }

        internal bool StopUpgradeMode()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.StopUpgradeMode()");

            if (!CCUConfigurationState.Singleton.IsUpgrading)
            {
                DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.StopUpgradeMode return true[1]");
                return true;
            }

            try
            {
                CCUConfigurationState.Singleton.UnsetUpgrading();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                DebugLog.Error(Log.CALM_LEVEL, () => "bool CcuCore.StopUpgradeMode return false");
                return false;
            }
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.StopUpgradeMode return true[2]");
            return true;
        }

        private static bool PassCeUpgradeFileCondition(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (fileName.Substring(0, 2) == "NK")
                /*if (Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}.bin")
                    || Regex.IsMatch(fileName, "NK_[0-9]{4,5}.bin")
                    || Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}_[0-9]{1}.bin")
                    || Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}_[C]{1}[R]{1}[C]{1}[v]{1}[0-9]{1}.bin")
                    || Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}_[0-9]{1}_[C]{1}[R]{1}[C]{1}[v]{1}[0-9]{1}.bin"))*/
                return true;
            return false;
        }

        public int[] CommunicationStatistic()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCore.CommunicationStatistic()");

            var remotigServerStatistic = _remotingServer.Statistics();

            var remotigServerStatisticLength = 0;
            if (remotigServerStatistic != null)
            {
                remotigServerStatisticLength = remotigServerStatistic.Length;
            }

            var result = new int[remotigServerStatisticLength + 4];

            if (remotigServerStatisticLength > 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                remotigServerStatistic.CopyTo(result, 0);
            }

            result[remotigServerStatisticLength] = SendEventsToServerDispatcher.Singleton.GetNotAcknowledgedEventsCount();
            result[remotigServerStatisticLength + 1] = SendEventsToServerDispatcher.Singleton.GetEventsFromAutonomousRunCount();
            result[remotigServerStatisticLength + 2] = SendEventsToServerDispatcher.Singleton.GetUnprocessedEventsCount();
            result[remotigServerStatisticLength + 3] = DCUs.Singleton.CommandTimeouts;

            DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int[] CcuCore.CommunicationStatistic return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        public void ResetSended()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetSended()");
            _remotingServer.ResetSended();
        }

        public void ResetDeserializationError()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetDeserializationError()");
            _remotingServer.ResetDeserializationError();
        }

        public void ResetReceived()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetReceived()");
            _remotingServer.ResetReceived();
        }

        public void ResetReceivedError()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetReceivedError()");
            _remotingServer.ResetReceivedError();
        }

        public void ResetMsgRetry()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetMsgRetry()");
            _remotingServer.ResetMsgRetry();
        }

        /// <summary>
        /// Resets CCU commant timeouts counter
        /// </summary>
        public void ResetCCUCommandTimeouts()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetCCUCommandTimeouts()");
            DCUs.Singleton.CommandTimeouts = 0;
        }

        public void ResetAll()
        {
            DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCore.ResetAll()");
            _remotingServer.ResetReceivedError();
            _remotingServer.ResetDeserializationError();
            _remotingServer.ResetSended(-1);
            _remotingServer.ResetReceived();
            _remotingServer.ResetMsgRetry();
            DCUs.Singleton.CommandTimeouts = 0;
        }

        public const string REGISTRY_CONTAL_PATH = @"HKLM\Software\Contal";
        private const string CAT12_COMBO_REG = @"Cat12ComboLicence";

        public const string REGISTRY_CCU_PATH = @"HKLM\Software\Contal\CCU";
        private const string CCU_START_COUNTER = @"StartCounter";

        public int GetRegistryCcuStartsCount()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "int CcuCore.GetRegistryCcuStartsCount()");
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    return (int)registryKey.GetValue(CCU_START_COUNTER);
                }
            }
            catch { }
            return 0;
        }

        private void IncrementCcuStartCounter()
        {
            var count = GetRegistryCcuStartsCount();
            count++;

            RegistryHelper.TrySetValue(REGISTRY_CCU_PATH, CCU_START_COUNTER, count, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Return CE image uptime
        /// </summary>
        /// <returns></returns>
        public ulong GetCEUpTime()
        {
            try
            {
                return WindowsCE.GetTickCount64();
            }
            catch
            {
                return (ulong)SystemTime.GetUpTime();
            }
        }

        public object[] GetCcuStartsCount()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "object[] CcuCore.GetCcuStartsCount()");
            var statistics = new object[3];
            statistics[0] = GetRegistryCcuStartsCount();

            var ceUpTimeMiliseconds = GetCEUpTime();
            var CCUUpTime = TimeSpan.FromMilliseconds(ceUpTimeMiliseconds - _ceUpTimeMilisecondsOnStartCCU);
            statistics[1] = string.Format("{0:D4}.{1:D2}.{2:D2}.{3:D2}", CCUUpTime.Days, CCUUpTime.Hours, CCUUpTime.Minutes, CCUUpTime.Seconds);

            var ceUpTime = TimeSpan.FromMilliseconds(ceUpTimeMiliseconds);
            statistics[2] = string.Format("{0:D4}.{1:D2}.{2:D2}.{3:D2}", ceUpTime.Days, ceUpTime.Hours, ceUpTime.Minutes, ceUpTime.Seconds);

            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => string.Format("object[] CcuCore.GetCcuStartsCount return {0}", Log.GetStringFromParameters(statistics)));
            return statistics;
        }

        public void ResetCcuStartCounter()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "void CcuCore.ResetCcuStartCounter()");
            RegistryHelper.TrySetValue(REGISTRY_CCU_PATH, CCU_START_COUNTER, 0, RegistryValueKind.DWord);
        }

        public string GetWinCEImageVersion()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "string CcuCore.GetWinCEImageVersion()");
            return _ceImageVersion ?? string.Empty;
        }

        //private const string WINCE_FILE_VERSION = @"Windows\Version.txt";
        private static string ReadWinCeImageVersionFromFile()
        {
            return WindowsCE.Build.ToString();

            //StreamReader versionFile = null;

            //try
            //{
            //    versionFile = new StreamReader(WINCE_FILE_VERSION);
            //    var version = versionFile.ReadLine();
            //    version = version.Trim();
            //    return version;
            //}
            //catch (Exception error)
            //{
            //    HandledExceptionAdapter.Examine(error);
            //    return string.Empty;
            //}
            //finally
            //{
            //    if (versionFile != null)
            //    {
            //        try { versionFile.Close(); }
            //        catch (Exception error)
            //        {
            //            HandledExceptionAdapter.Examine(error);
            //        }
            //    }
            //}
        }

        public string[] CoprocessorBuildNumber()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "string[] CcuCore.CoprocessorBuildNumber()");
            var result = new string[3];
            result[0] = DCUs.Singleton.ActualCoprocessorBuildNumber.ToString(CultureInfo.InvariantCulture);
            result[1] = CLSP_HWC_VERSION.ToString(CultureInfo.InvariantCulture);
            result[2] = DCUs.Singleton.CoprocessorUpgradeResult + " " + DCUs.Singleton.ActualCoprocessorStats;
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => string.Format("string[] CcuCore.CoprocessorBuildNumber return {0}", Log.GetStringFromParameters((object)result)));
            return result;
        }

        public string SimulationCardSwiped(
            ObjectType objectType,
            Guid idObject,
            string cardNumber,
            string pin,
            int pinLength)
        {
            DebugLog.Info(
                Log.PERFORMANCE_LEVEL,
                () =>
                    string.Format(
                        "string CcuCore.SimulationCardSwiped(ObjectType objectType, Guid idObject, string cardNumber, string pin, int pinLength): [{0}]",
                        Log.GetStringFromParameters(
                            objectType,
                            idObject,
                            cardNumber,
                            pin,
                            pinLength)));

            var lastCardDB = Database.ConfigObjectsEngine.CardsStorage.GetCard(cardNumber);

            if (lastCardDB == null)
            {
                DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "string CcuCore.SimulationCardSwiped return Unknown card");
                return "Unknown card";
            }

            switch ((DB.CardState)lastCardDB.State)
            {
                case DB.CardState.blocked:
                    DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "string CcuCore.SimulationCardSwiped return Card BLOCKED");
                    return "Card BLOCKED";
                case DB.CardState.destroyed:
                    DebugLog.Info(Log.PERFORMANCE_LEVEL,
                        () => "string CcuCore.SimulationCardSwiped return Card DESTROYED");
                    return "Card DESTROYED";
                case DB.CardState.lost:
                    DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "string CcuCore.SimulationCardSwiped return Card LOST");
                    return "Card LOST";
                case DB.CardState.unused:
                    DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "string CcuCore.SimulationCardSwiped return Card UNUSED");
                    return "Card UNUSED";
            }

            if (lastCardDB.State != (byte)DB.CardState.active)
            {
                DebugLog.Info(Log.PERFORMANCE_LEVEL,
                    () => "string CcuCore.SimulationCardSwiped return Card in not ACTIVE");
                return "Card in not ACTIVE";
            }

            var cardPinResult = string.Empty;
            if (pin != string.Empty)
            {
                cardPinResult = " INVALID PIN";
                if (lastCardDB.Pin == pin)
                {
                    cardPinResult = " PIN valid";
                }
                else if (lastCardDB.PinLength != pinLength)
                {
                    cardPinResult = " PIN length invalid";
                }
            }

            if (!lastCardDB.IsValid)
            {
                return "Card is not valid";
            }

            string description;

            if (objectType == ObjectType.CardReader)
            {
                var cardReader = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.CardReader, idObject) as DB.CardReader;

                if (cardReader == null)
                    return "Card reader not exists in the database";

                if (CardAccessRightsManager.Singleton.HasAccessTest(
                    lastCardDB,
                    idObject,
                    Database.ConfigObjectsEngine.DoorEnvironmentsStorage.GetDoorEnvironemntIdForCardReader(idObject),
                    cardReader.GuidDCU,
                    out description))
                {
                    DebugLog.Info(Log.PERFORMANCE_LEVEL,
                        () => "string CcuCore.SimulationCardSwiped return Access granted " + cardPinResult);

                    return "Access granted " + cardPinResult;
                }
            }
            else if (objectType == ObjectType.MultiDoorElement)
            {
                if (CardAccessRightsManager.Singleton.HasAccessMultiDoorTest(
                    lastCardDB,
                    idObject,
                    out description))
                {
                    DebugLog.Info(Log.PERFORMANCE_LEVEL,
                        () => "string CcuCore.SimulationCardSwiped return Access granted " + cardPinResult);

                    return "Access granted " + cardPinResult;
                }
            }
            else
            {
                description = "Wrong object type.";
            }

            DebugLog.Info(
                Log.PERFORMANCE_LEVEL,
                () => "string CcuCore.SimulationCardSwiped return Access DENIED " + cardPinResult);

            return "Access DENIED " + cardPinResult + "(" + description + ")";
        }

        public int[] GetOtherStatistics()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "int[] CcuCore.GetOtherStatistics()");
            var threadsCount = GetCurrentProcessThreadsCount();

            var nandFlashInfo = new NandFlashInfo();
            var sdCardInfo = new SdCardInfo();

            var totalMemory = (int)(SystemInfo.Singleton.TotalPhysicalMemory * 1000000);
            var freeMemory = (int)(SystemInfo.Singleton.AvailablePhysicalMemory * 1000000);
            var memoryLoad = (int)(SystemInfo.Singleton.MemoryLoad);

            int[] result =
            {
                threadsCount,
                (int)(nandFlashInfo.FreeBytesAvailable / 1000),
                (int)(nandFlashInfo.TotalNumberOfBytes / 1000),
                sdCardInfo.Present
                    ? 1
                    : 0,
                (int)(sdCardInfo.FreeSpace / 1000),
                (int)(sdCardInfo.Size / 1000),
                freeMemory,
                totalMemory,
                memoryLoad
            };

            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => string.Format("int[] CcuCore.GetOtherStatistics return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        private static int GetCurrentProcessThreadsCount()
        {
            var info = ProcessInfo.GetAllProcessInfos();
            return (from item in info
                    where item.Name.ToLower() == "ccu.exe"
                    select (int)item.ThreadCount)
                        .FirstOrDefault();
        }

        public void ResetCardReader(Guid cardReaderGuid)
        {
            DebugLog.Info(
                Log.PERFORMANCE_LEVEL,
                () => string.Format(
                    "void CcuCore.ResetCardReader(Guid cardReaderGuid): [{0}]",
                    Log.GetStringFromParameters(cardReaderGuid)));

            SafeThread<Guid>.StartThread(
                DoResetCardReader,
                cardReaderGuid);
        }

        private static void DoResetCardReader(Guid idCardReader)
        {
            CardReaders.Singleton.ResetCardReader(idCardReader);
        }

        public interface IBlockFileInfoProvider
        {
            BlockFileInfo BlockFileInfo { get; }
            void Reset();
        }

        private readonly ICollection<IBlockFileInfoProvider> _blockFileInfoProviders =
            new LinkedList<IBlockFileInfoProvider>();

        public void RegisterBlockFileInfoProvider(IBlockFileInfoProvider blockFileInfoProvider)
        {
            _blockFileInfoProviders.Add(blockFileInfoProvider);
        }

        private readonly object _loadFromDataChannelPeerConsumersLock = new object();

        public bool ResetBlockFilesInfo()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.ResetBlockFilesInfo()");

            foreach (var blockFileInfoProvider in _blockFileInfoProviders)
                blockFileInfoProvider.Reset();

            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.ResetBlockFilesInfo return true");
            return true;
        }

        /// <summary>
        /// Save to registry whether time is synchronized from the server and restart SNTP
        /// </summary>
        /// <param name="syncingTimeFromServer"></param>
        public void SetTimeSyncingFromServer(bool syncingTimeFromServer)
        {
            SetRegistryTimeSyncingFromServer(syncingTimeFromServer);
            RestartSNTP();
        }

        private const string CCU_REG_TIME_SYNCING_FROM_SERVER = "TimeSyncingFromServer";

        /// <summary>
        /// Save to registry whether time is synchronized from the server
        /// </summary>
        /// <param name="syncingTimeFromServer"></param>
        private static void SetRegistryTimeSyncingFromServer(bool syncingTimeFromServer)
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CCU_REG_TIME_SYNCING_FROM_SERVER, syncingTimeFromServer, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Get from registry whether time is synchronized from the server
        /// </summary>
        /// <returns></returns>
        public bool GetRegistryTimeSyncingFromServer()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.GetRegistryTimeSyncingFromServer()");
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    var timeSyncingFromServer = Convert.ToInt32(registryKey.GetValue(CCU_REG_TIME_SYNCING_FROM_SERVER));
                    var result = timeSyncingFromServer != 0;
                    DebugLog.Info(Log.PERFORMANCE_LEVEL, () => string.Format("bool CcuCore.GetRegistryTimeSyncingFromServer return {0}[1]", Log.GetStringFromParameters(result)));
                    return result;
                }
            }
            catch { }

            DebugLog.Warning(Log.PERFORMANCE_LEVEL, () => "bool CcuCore.GetRegistryTimeSyncingFromServer return false[2]");
            return false;
        }

        private const string CCU_REG_LOG_VERBOSITY_LEVEL = "LogVerbosityLevel";

        /// <summary>
        /// Sets Log verbosity level
        /// </summary>
        /// <param name="verbosity">verbosity level</param>
        public void SetRegistryLogVerbosityLevel(byte verbosity)
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => string.Format("void CcuCore.SetRegistryLogVerbosityLevel(byte verbosity): [{0}]", Log.GetStringFromParameters(verbosity)));
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CCU_REG_LOG_VERBOSITY_LEVEL, verbosity, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Gets Log verbosity level
        /// </summary>
        /// <returns>verbosity level</returns>
        public static byte GetRegistryLogVerbosityLevel()
        {
            DebugLog.Info(Log.PERFORMANCE_LEVEL, () => "byte CcuCore.GetRegistryLogVerbosityLevel()");
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    var result = Convert.ToInt32(registryKey.GetValue(CCU_REG_LOG_VERBOSITY_LEVEL));
                    DebugLog.Info(Log.PERFORMANCE_LEVEL, () => string.Format("byte CcuCore.GetRegistryLogVerbosityLevel return {0}[1]", Log.GetStringFromParameters(result)));
                    return (byte)result;
                }
            }
            catch { }

            DebugLog.Warning(Log.PERFORMANCE_LEVEL, () => "byte CcuCore.GetRegistryLogVerbosityLevel return 128[2]");
            return Log.LOW_LEVEL;
        }

        /// <summary>
        /// Send event object deserialize failed to the server
        /// </summary>
        /// <param name="objectGuid"></param>
        /// <param name="objectType"></param>
        /// <param name="message"></param>
        /// <param name="exceptionMessage"></param>
        public void SaveEventObjectDeserializeFailed(Guid objectGuid, ObjectType objectType, string message, string exceptionMessage)
        {
            Events.ProcessEvent(
                new EventObjectDeserializeFailed(
                    objectGuid,
                    objectType,
                    message,
                    exceptionMessage));
        }

        public static DateTime LocalTime
        {
            get
            {
                if (WindowsCE.Build >= LPC32xxConstants.CeVersionDatetimeNowFix)
                {
                    return DateTime.Now;
                }

                return SystemTime.GetLocalTime();
            }
        }

        public static DateTime UtcTime
        {
            get
            {
                if (WindowsCE.Build >= LPC32xxConstants.CeVersionDatetimeNowFix)
                {
                    return DateTime.UtcNow;
                }

                return SystemTime.GetLocalTime();
            }
        }
    }
}
