using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Net;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.LwMessaging;
using Contal.LwRemoting2;
using Contal.LwSerialization;
using Contal.SLA.Client;

namespace Contal.Cgp.NCAS.Server
{
    public class CCUSettings
    {
        private class PreInitCcuAndResult
        {
            public Func<bool> PreInitCcu { get; private set; }
            public bool Result { get; set; }

            public PreInitCcuAndResult(
                Func<bool> preInitCcu)
            {
                PreInitCcu = preInitCcu;
                Result = true;
            }
        }

        private const string CCU_REMOTING_SERVICE = "CcuCoreRemotingProvider";
        private const int CCU_METHOD_TIMEOUT = 30000;
        private const int CCU_METHOD_LONG_TIMEOUT = 900000;
        private const int CCU_METHOD_SHORT_TIMEOUT = 15000;
        private const int MAX_DELEAY_FOR_BLOCK_SENDING_OBJECTS = 300000;
        private const int DELAY_FOR_REPEATING_OF_CHECKING_CONFIGURATION = 30000;

        public CcuEvents CCUEvents { get; private set; }

        public IPAddress IPAddress { get; private set; }
        private readonly Dictionary<byte, int> _dcusUpgradePercents;

        private MainBoardVariant _mainBoardType;
        private SafeThread<CCU, PreInitCcuAndResult> _threadDoInitCCU;
        private readonly object _lockThreadDoInitCCU = new object();

        private readonly ProcessingQueue<CcuDataBatch> _queueSendingObjectsToCCU =
            new ProcessingQueue<CcuDataBatch>();

        public string CcuName { get; private set; }

        public Guid GuidCCU { get; private set; }
        public CCUOnlineState State { get; private set; }
        public CCUConfigurationState ConfigurationState { get; private set; }

        public Dictionary<Guid, InputStates> InputStates { get; set; }
        public Dictionary<Guid, OutputStates> OutputStates { get; set; }
        public Dictionary<Guid, CardReaderState> CardReaderStates { get; set; }
        public SyncDictionary<byte, DCUState> DcuStates { get; set; }

        public LwRemotingClient Client { get; private set; }
        public string IPAddressString { get; private set; }

        public Dictionary<Guid, CCUDoorEnvironemntState> DoorEnvironmentStates { get; set; }

        public MainBoardVariant MainBoardType
        {
            get { return _mainBoardType; }
            set
            {
                _mainBoardType = value;
                FixMaximumCardReadersForCcu();
            }
        }



        public string ServerHashCode { get; private set; }
        public IPSetting IPSetting { get; private set; }
        public bool IsCCU0 { get; private set; }
        public bool IsCCUUpgrader { get; private set; }
        public string FirmwareVersion { get; set; }
        public string WinCeVersion { get; set; }
        public ActionResultUpgrade CeUpgradeResult { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public long FileReceivedBytes { get; set; }

        /// <summary>
        /// true if the CCU device is sharing resources with CAT12CE
        /// </summary>
        public bool IsCat12Combo { get; set; }

        public bool IsCCUCorrectlyConfiguredForThisServer
        {
            get
            {
                return ConfigurationState != CCUConfigurationState.ConfiguredForAnotherServer &&
                       ConfigurationState != CCUConfigurationState.ConfiguredForThisServerUpgradeOnly &&
                       ConfigurationState != CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe &&
                       ConfigurationState != CCUConfigurationState.Unknown &&
                       ConfigurationState != CCUConfigurationState.Unconfigured;
            }
        }

        public void SetCCUName(string ccuName)
        {
            CcuName = ccuName;
        }

        private void ChangeActualIpAddress(string newIpAddress)
        {
            CCUConfigurationHandler.Singleton.ChangeCCUIpAddress(IPAddressString, newIpAddress, GuidCCU);
            IPAddressString = newIpAddress;

            IPAddress ipAddress;

            IPAddress.TryParse(newIpAddress, out ipAddress);

            IPAddress = ipAddress;

            _lwMessagingRemoteEndpoint = new IPEndPoint(
                ipAddress,
                NCASConstants.TCP_LW_MESSAGING_PORT);
        }
        public object Call(string methodName, params object[] parameters)
        {
            if (Client == null)
                return null;

            switch (methodName)
            {
                case "DeleteObject":
                case "ForceReconfiguration":
                case "SaveObject":
                case "ApplyChanges":
                case "ConfigureForAnotherClient":
                case "Unconfigure":
                case "WaitForFinishProcessingStream":
                case "ReadMaximumVersionAndIds":
                    return Client.CallWithTimeout(methodName, CCU_METHOD_LONG_TIMEOUT, parameters);

                case "AcknowledgeAllowed":
                    return Client.CallWithTimeout(methodName, CCU_METHOD_SHORT_TIMEOUT, parameters);

                default:
                    return Client.Call(methodName, parameters);
            }
        }

        public CCUSettings(CCU ccu)
        {
            CCUEvents = new CcuEvents(this);

            CeUpgradeResult = ActionResultUpgrade.Unkonwn;
            FirmwareVersion = string.Empty;
            WinCeVersion = string.Empty;
            GuidCCU = ccu.IdCCU;
            ChangeActualIpAddress(ccu.IPAddress);
            SetCCUName(ccu.Name);
            State = CCUOnlineState.Unknown;
            ConfigurationState = CCUConfigurationState.Unknown;
            InputStates = new Dictionary<Guid, InputStates>();
            OutputStates = new Dictionary<Guid, OutputStates>();
            DoorEnvironmentStates = new Dictionary<Guid, CCUDoorEnvironemntState>();
            CardReaderStates = new Dictionary<Guid, CardReaderState>();
            DcuStates = new SyncDictionary<byte, DCUState>();
            _dcusUpgradePercents = new Dictionary<byte, int>();

            if (ccu.DCUs != null)
                foreach (DCU dcu in ccu.DCUs)
                    if (dcu.LogicalAddress != 0 && !DcuStates.ContainsKey(dcu.LogicalAddress))
                        DcuStates.Add(
                            dcu.LogicalAddress,
                            new DCUState(
                                dcu,
                                null,
                                OnlineState.Offline));

            Exception err;
            ICollection<CardReader> cardReaders = CardReaders.Singleton.GetCcuCardReaders(ccu.IdCCU, out err);
            foreach (CardReader cardReader in cardReaders)
                if (cardReader != null)
                    CardReaderStates.Add(
                        cardReader.IdCardReader,
                        new CardReaderState(
                            cardReader,
                            null,
                            false));

            _mainBoardType = MainBoardVariant.Unknown;

            SetServerHashCode();
            CreateLwRemotingClient();

            _queueSendingObjectsToCCU.ItemProcessing += TrySendCcuDataBatch;
            _queueSendingObjectsToCCU.DefaultPriority = 100;
        }

        public void GeneralOptionsEventlogsChanged()
        {
            if (ConfigurationState == CCUConfigurationState.ConfiguredForThisServer ||
                    ConfigurationState == CCUConfigurationState.ForceReconfiguration)
                CCUConfigurationHandler.Singleton.SetOldEventsSettings(GuidCCU);
        }

        public object SetEnableLoggingSDPSTZChanges()
        {
            return
                ConfigurationState == CCUConfigurationState.ConfiguredForThisServer ||
                ConfigurationState == CCUConfigurationState.ForceReconfiguration
                    ? CCUConfigurationHandler.Singleton.SetEnableLoggingSDPSTZChanges(
                        GuidCCU,
                        GeneralOptions.Singleton.EnableLoggingSDPSTZChanges)
                    : null;
        }

        public void SetAlarmAreaRestrictivePolicyForTimeBuying()
        {
            if (ConfigurationState != CCUConfigurationState.ConfiguredForThisServer &&
                    ConfigurationState != CCUConfigurationState.ForceReconfiguration)
                return;

            CCUConfigurationHandler.Singleton.SetAlarmAreaRestrictivePolicyForTimeBuying(
                GuidCCU,
                GeneralOptions.Singleton.AlarmAreaRestrictivePolicyForTimeBuying);
        }

        /// <summary>
        /// Send whether CCU time is synchronized from the server
        /// </summary>
        public void SetTimeSyncingFromServer()
        {
            if (ConfigurationState != CCUConfigurationState.ConfiguredForThisServer &&
                    ConfigurationState != CCUConfigurationState.ForceReconfiguration)
                return;

            CCUConfigurationHandler.Singleton.SetTimeSyncingFromServer(
                GuidCCU,
                GeneralOptions.Singleton.SyncingTimeFromServer);

            StartStopSendingTimeFromServer();
        }

        private ITimer _sendingTimeFromServer;
        private readonly object _lockStartStopSendingTimeFromServer = new object();
        private int _oldPeriodOfTimeSyncWithoutStratum;
        /// <summary>
        /// Start or stop sending time to the CCU according to the time synchronization settings
        /// </summary>
        public void StartStopSendingTimeFromServer()
        {
            if (ConfigurationState != CCUConfigurationState.ConfiguredForThisServer &&
                    ConfigurationState != CCUConfigurationState.ForceReconfiguration)
                return;

            lock (_lockStartStopSendingTimeFromServer)
            {
                CCU ccu = CCUs.Singleton.GetById(GuidCCU);
                if (ccu == null)
                    return;

                bool syncingTimeFromServer =
                    ccu.SyncingTimeFromServer != null
                        ? ccu.SyncingTimeFromServer.Value
                        : GeneralOptions.Singleton.SyncingTimeFromServer;

                if (!syncingTimeFromServer)
                {
                    if (_sendingTimeFromServer == null)
                        return;

                    _sendingTimeFromServer.StopTimer();
                    _sendingTimeFromServer = null;
                    _oldPeriodOfTimeSyncWithoutStratum = 0;

                    return;
                }

                int actPeriodOfTimeSyncWithoutStratum =
                    GeneralOptions.Singleton.PeriodOfTimeSyncWithoutStratum;

                if (_sendingTimeFromServer != null &&
                    _oldPeriodOfTimeSyncWithoutStratum != actPeriodOfTimeSyncWithoutStratum)
                {
                    _sendingTimeFromServer.StopTimer();
                    _sendingTimeFromServer = null;
                }

                if (_sendingTimeFromServer != null)
                    return;

                _sendingTimeFromServer =
                    TimerManager.Static.StartTimer(
                        actPeriodOfTimeSyncWithoutStratum * 1000 * 60,
                        true,
                        OnTimeoutSendingTimeFromServer);

                _oldPeriodOfTimeSyncWithoutStratum = actPeriodOfTimeSyncWithoutStratum;
            }
        }

        /// <summary>
        /// Send actual time to the CCU for syncing
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public bool OnTimeoutSendingTimeFromServer(TimerCarrier timer)
        {
            CCUConfigurationHandler.Singleton.SetTimeFromServer(
                GuidCCU,
                DateTime.UtcNow,
                GeneralOptions.Singleton.PeriodicTimeSyncTolerance);

            return true;
        }

        private readonly EventWaitHandle _sendingObjectsToCCUAllowed =
            new ManualResetEvent(false);

        private readonly EventWaitHandle _initCCUMarkerReached =
            new ManualResetEvent(false);

        private readonly AutoResetEvent _initDataBatchSent = new AutoResetEvent(false);

        public void EnqueueCcuDataBatch(CcuDataBatch ccuDataBatch)
        {
            Console.WriteLine("EnqueueCcuDataBatch for CCU:" + GuidCCU);

            switch (ccuDataBatch.ItemType)
            {
                case CcuDataBatch.ItemTypeEnum.InitCCUEnqueue:

                    _queueSendingObjectsToCCU.Enqueue(
                        ccuDataBatch,
                        PQEnqueueFlags.None,
                        0);

                    _sendingObjectsToCCUAllowed.Set();

                    break;

                case CcuDataBatch.ItemTypeEnum.NormalEnqueue:

                    _queueSendingObjectsToCCU.Enqueue(ccuDataBatch);

                    break;
            }
        }

        /// <summary>
        /// Custom exception for object sending
        /// </summary>
        private class SendingObjectException : Exception
        {
            // ReSharper disable once UnusedMember.Local
            public SendingObjectException(string message)
                : base(message)
            {
            }

            public SendingObjectException(string message, Exception innerInnerException)
                : base(message, innerInnerException)
            {
            }
        }

        private bool _initialObjectSending = true;
        private const int INITIAL_OBJECTS_SENDING_RETRY_COUNT = 3;
        private const int INITIAL_OBJECTS_SENDING_DELAY = 5000;

        /// <summary>
        /// Method will resend objects to CCU if exception occurred.
        /// </summary>
        /// <param name="ccuDataBatch"></param>
        private void SendCcuDataBatchWithRetry(CcuDataBatch ccuDataBatch)
        {
            for (int i = 0; i < INITIAL_OBJECTS_SENDING_RETRY_COUNT; i++)
                try
                {
                    SendCcuDataBatch(ccuDataBatch);
                    break;
                }
                catch (Exception ex)
                {
                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_INITIAL_OBJECT_SENDING_FAILED,
                        GetType().Assembly.GetName().Name,
                        null,
                        string.Format(
                            "{0}{1}. {2}\r\n{3}",
                                i == 0
                                    ? string.Empty
                                    : "Retry " + i + ": ",
                                ex.Message,
                                ex.InnerException != null
                                    ? ex.InnerException.Message
                                    : string.Empty,
                                ex.StackTrace),
                        new EventlogParameter(
                            EventlogParameter.TYPECCU,
                            IPAddressString));

                    if (i < (INITIAL_OBJECTS_SENDING_RETRY_COUNT - 1))
                        Thread.Sleep(INITIAL_OBJECTS_SENDING_DELAY);
                }

            if (ccuDataBatch.ItemType == CcuDataBatch.ItemTypeEnum.InitCCUEnqueue)
                _initDataBatchSent.Set();
        }

        private void TrySendCcuDataBatch(CcuDataBatch ccuDataBatch)
        {
            if (ccuDataBatch.ItemType == CcuDataBatch.ItemTypeEnum.InitCCUMarker)
            {
                _initCCUMarkerReached.Set();
                _sendingObjectsToCCUAllowed.WaitOne();

                return;
            }

            if (!IsCCUCorrectlyConfiguredForThisServer)
                return;

            if (_initialObjectSending)
            {
                SendCcuDataBatchWithRetry(ccuDataBatch);
                _initialObjectSending = false;

                return;
            }

            try
            {
                SendCcuDataBatch(ccuDataBatch);
            }
            catch (Exception ex)
            {
                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPE_INITIAL_OBJECT_SENDING_FAILED,
                    GetType().Assembly.GetName().Name,
                    null,
                    string.Format(
                        "{0}. {1}\r\n{2}",
                        ex.Message,
                        ex.InnerException != null
                            ? ex.InnerException.Message
                            : string.Empty,
                        ex.StackTrace),
                    new EventlogParameter(
                        EventlogParameter.TYPECCU,
                        IPAddressString));

                if (ccuDataBatch.ItemType == CcuDataBatch.ItemTypeEnum.InitCCUEnqueue)
                    _initDataBatchSent.Set();
            }
        }

        /// <summary>
        /// Base method that handles sending of objects to CCU
        /// </summary>
        /// <param name="ccuDataBatch"></param>
        private void SendCcuDataBatch(CcuDataBatch ccuDataBatch)
        {
            foreach (ModifiedObjectsCollection item in ccuDataBatch.ModifiedObjectsForCCU)
                Console.WriteLine(
                    "Sending object type: {0}, count :{1}",
                    item.ObjectType,
                    item.Objects.Count());

            if (ccuDataBatch.ModifiedObjectsForCCU != null)
                foreach (ModifiedObjectsCollection modifyObjectsForCCU in
                    ccuDataBatch.ModifiedObjectsForCCU)
                {
                    SendModifiedObjectsToCCU(modifyObjectsForCCU);
                }

            foreach (var item in ccuDataBatch.ObjectsToDelete)
                Console.WriteLine(
                    "Deleting object type: {0}, count :{1}",
                    item.Key,
                    item.Value.Count());

            DeleteObjectsFromCCU(ccuDataBatch);

            if (ccuDataBatch.RunApplyChanges)
                CCUConfigurationHandler.Singleton.ApplyChanges(GuidCCU);

            //Try catch inside
            BindingInterCCUCommunication.Singleton.SendBindingsToCCU(GuidCCU);

            if (ccuDataBatch.ItemType == CcuDataBatch.ItemTypeEnum.InitCCUEnqueue)
                _initDataBatchSent.Set();
        }

        private const int MAX_SENDING_RETRY_COUNT = 2;

        public bool SendObjects(
            Guid guidCCU,
            ObjectsToSend objectsToSend)
        {
            try
            {
                int repeatCount = 0;

                do
                {
                    var retValue = CCUConfigurationHandler.Singleton.SendToRemotingCCUs(
                            guidCCU,
                            "SaveObject",
                            objectsToSend);

                    if (!(retValue is bool))
                        return false;

                    if ((bool)retValue)
                        return true;
                }
                while (++repeatCount < MAX_SENDING_RETRY_COUNT);

                return false;
            }
            catch (Exception ex)
            {
                throw new SendingObjectException(
                    string.Format(
                        "Sending serialized objects type of {0} throw exception",
                        objectsToSend.ObjectType),
                    ex);
            }
        }

        private bool TrySendModifiedObjectsToCcu(ModifiedObjectsCollection modifiedObjectsCollection)
        {
            if (!modifiedObjectsCollection.IsEmpty)
            {
                var objectsToSend = new ObjectsToSend(modifiedObjectsCollection.ObjectType);

                foreach (object objectToSend in modifiedObjectsCollection.Objects)
                {
                    if (objectsToSend.TryAddObject(objectToSend))
                        continue;

                    if (!SendObjects(
                            GuidCCU,
                            objectsToSend))
                    {
                        return false;
                    }

                    objectsToSend = new ObjectsToSend(modifiedObjectsCollection.ObjectType);
                    objectsToSend.TryAddObject(objectToSend);
                }

                if (!SendObjects(
                    GuidCCU,
                    objectsToSend))
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Method will send modified objects to CCU and update objects version on server and CCU
        /// </summary>
        /// <param name="modifiedObjectsCollection"></param>
        private void SendModifiedObjectsToCCU(ModifiedObjectsCollection modifiedObjectsCollection)
        {
            if (modifiedObjectsCollection == null)
                return;

            bool success = false;

            try
            {
                success = TrySendModifiedObjectsToCcu(modifiedObjectsCollection);
            }
            finally
            {
                if (!success)
                    DataReplicationManager.Singleton.ReloadCcuMaxObjectVersionAndIdsForObjectType(
                        GuidCCU,
                        modifiedObjectsCollection.ObjectType);
            }

            if (success)
                SaveMaxObjectTypeVersionOnCcu(modifiedObjectsCollection);
        }

        /// <summary>
        /// Method will update objects versions on server and also on CCU
        /// </summary>
        /// <param name="modifiedObjectsCollection"></param>
        private void SaveMaxObjectTypeVersionOnCcu(ModifiedObjectsCollection modifiedObjectsCollection)
        {
            try
            {
                CCUConfigurationHandler.Singleton.SendToRemotingCCUs(
                    GuidCCU,
                    "SaveMaxObjectTypeVersion",
                    (byte)modifiedObjectsCollection.ObjectType,
                    modifiedObjectsCollection.Version);
            }
            catch (Exception ex)
            {
                throw new SendingObjectException(
                    "Writing updated versions throw exception",
                    ex);
            }
        }

        /// <summary>
        /// Method will delete unnecessary objects from CCU, or save them to server table if deletion fails
        /// </summary>
        /// <param name="ccuDataBatch"></param>
        private void DeleteObjectsFromCCU(CcuDataBatch ccuDataBatch)
        {
            try
            {
                if (ccuDataBatch.ObjectsToDelete == null)
                    return;

                foreach (var kvp in ccuDataBatch.ObjectsToDelete)
                {
                    ICollection<Guid> objectsGuid = kvp.Value;

                    if (objectsGuid == null || objectsGuid.Count <= 0)
                        continue;

                    var retValue = CCUConfigurationHandler.Singleton.SendToRemotingCCUs(
                        GuidCCU,
                        "DeleteObject",
                        (byte)kvp.Key,
                        objectsGuid);

                    if (!(retValue is bool) || !(bool)retValue)
                        DataReplicationManager.Singleton.ReloadCcuMaxObjectVersionAndIdsForObjectType(
                            GuidCCU,
                            kvp.Key);
                }
            }
            catch (Exception ex)
            {
                throw new SendingObjectException(
                    "Deleting objects on CCU throw exception",
                    ex);
            }
        }

        private void SetServerHashCode()
        {
            string externalCIID = SLAClientModule.Singleton.GetExternalCIID();
            ServerHashCode = QuickHashes.GetSHA1String(externalCIID + "," + GuidCCU);
        }

        private void CreateLwRemotingClient()
        {
            ChangeOnlineState(CCUOnlineState.Unknown);

            var aes = new AESSettings(
                LwRemotingGlobals.GetSHA1(LwRemotingGlobals.LWREMOTING_KEY),
                LwRemotingGlobals.LWREMOTING_SALT,
                AESKeySize.Size128);

            Client =
                new LwRemotingClient(
                    aes,
                    IPAddressString,
                    63002)
                {
                    PreferredService = CCU_REMOTING_SERVICE,
                    CallTimeout = CCU_METHOD_TIMEOUT,
                    CallRetry = 2
                };

            //_client = new Contal.LwRemoting.LwRemotingClient(_ipAddress, 63002, this.GetType().Assembly,
            //typeof(CgpServer).Assembly, typeof(Alarm).Assembly, typeof(Contal.Cgp.NCAS.Server.Beans.AlarmArea).Assembly,
            //typeof(Contal.Cgp.Server.Beans.TimeZone).Assembly);

            Client.Connected += ClientConnected;
            Client.ConnectionFailed += ClientConnectionFailed;
            Client.Disconnected += ClientDisconnected;
            Client.WaitForACK += WaitForACK;

            Client.Start();
        }

        private static bool WaitForACK(string methodName)
        {
            return methodName != "DeleteEvents";
        }

        private void AcknowledgeAllow()
        {
            Client.AcknowledgeDisable();

            if (CCUConfigurationHandler.Singleton.AcknowledgeAllowed(GuidCCU))
                Client.AcknowledgeAllow();
        }

        public void ChangeIpAddress(string ipAddress)
        {
            if (IPAddressString == ipAddress)
                return;

            ChangeActualIpAddress(ipAddress);

            Disconnect();

            CreateLwRemotingClient();
        }

        public void Disconnect()
        {
            if (Client == null)
                return;

            _connectionFailedAlarm = false;

            try
            {
                Client.Disconnect();
                Client.ClearConnectionEvents();
            }
            catch
            {

            }

            Client = null;
        }

        private readonly object _clientConnectionEventLock = new object();
        private void ClientConnected(IPEndPoint parameter)
        {
            _connectionFailedAlarm = false;

            lock (_clientConnectionEventLock)
            {
                ChangeOnlineState(CCUOnlineState.Online);
                InitCCU();
            }
        }

        private void CreateAlarmCcuOutdatedFirmware(
            CCU ccu,
            string firmwareVersion)
        {
            if (ccu == null || firmwareVersion == string.Empty)
                return;

            try
            {
                var currentFW = new Version(firmwareVersion.Split(' ')[0]);
                var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CCU;

                if (currentFW >= minimalFW)
                    return;

                NcasServer.GetAlarmsQueue().AddAlarm(
                    new ServerAlarm(
                        new ServerAlarmCore(
                            new Alarm(
                                new AlarmKey(
                                    AlarmType.CCU_OutdatedFirmware,
                                    new IdAndObjectType(
                                        ccu.IdCCU,
                                        ObjectType.CCU)),
                                AlarmState.Alarm),
                            string.Format(
                                "{0} : {1}",
                                AlarmType.CCU_OutdatedFirmware,
                                ccu.ToString()),
                            ccu.Name,
                            string.Format(
                                "{0} {1}\r\n{2}: {3}",
                                NCASServer.Singleton.LocalizationHelper.GetString("CCUOutdatedFirmware"),
                                firmwareVersion,
                                NCASServer.Singleton.LocalizationHelper.GetString("MinimalSupportedFW"),
                                NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CCU))));
            }
            catch { }
        }

        public void StopAlarmCcuOutdatedFirmware()
        {
            NcasServer.GetAlarmsQueue().TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarm(
                        new AlarmKey(
                            AlarmType.CCU_OutdatedFirmware,
                            new IdAndObjectType(
                                GuidCCU,
                                ObjectType.CCU))));
        }

        public void InitCCU()
        {
            SafeThread.StartThread(
                () => DoInitCcu(null));
        }

        readonly EventWaitHandle _doInitCCUFinished = new ManualResetEvent(false);

        private ConfigureResult DoInitCcu(Func<bool> preInitCcu)
        {
            try
            {
                CCU ccu = CCUs.Singleton.GetById(GuidCCU);

                PreInitCcuAndResult preInitCcuAndResult;

                lock (_lockThreadDoInitCCU)
                {
                    if (_threadDoInitCCU != null)
                        return ConfigureResult.AlreadyRunning;

                    _doInitCCUFinished.Reset();

                    preInitCcuAndResult = new PreInitCcuAndResult(preInitCcu);

                    _threadDoInitCCU = new SafeThread<CCU, PreInitCcuAndResult>(DoInitCcu);
                    _threadDoInitCCU.Start(ccu, preInitCcuAndResult);
                }

                _doInitCCUFinished.WaitOne();

                return
                    preInitCcuAndResult.Result
                        ? ConfigureResult.OK
                        : ConfigureResult.GeneralFailure;
            }
            catch
            {
                return ConfigureResult.GeneralFailure;
            }
        }

        private void StopThreadDoInitCCU()
        {
            lock (_lockThreadDoInitCCU)
                try
                {
                    if (_threadDoInitCCU != null &&
                        _threadDoInitCCU.IsStarted)
                    {
                        _threadDoInitCCU.Stop(0);
                    }
                }
                catch
                {
                }
        }

        private void DoInitCcu(
            CCU ccu,
            PreInitCcuAndResult preInitCcuAndResult)
        {
            try
            {
                _queueSendingObjectsToCCU.Clear();

                _sendingObjectsToCCUAllowed.Reset();
                _initCCUMarkerReached.Reset();

                var initCcuMarker =
                    CcuDataBatch.CreateInitMarker();

                _queueSendingObjectsToCCU.Enqueue(
                    initCcuMarker,
                    PQEnqueueFlags.None,
                    0);

                _initCCUMarkerReached.WaitOne();

                var preInitCcu = preInitCcuAndResult.PreInitCcu;

                if (preInitCcu != null && !preInitCcu())
                {
                    preInitCcuAndResult.Result = false;
                    return;
                }

                if (State != CCUOnlineState.Online)
                {
                    preInitCcuAndResult.Result = false;
                    return;
                }

                AcknowledgeAllow();
                CheckCCUUpgrader();

                if (IsCCUUpgrader)
                {
                    CCUConfigurationHandler.Singleton.BindEventsCCUUpgrader(GuidCCU);
                    ChangeConfigurationState(CCUConfigurationState.Upgrading);

                    CCUEvents.ClearAllEventsAndTransfers();

                    return;
                }

                CCUConfigurationState configurationState = GetCcuConfigurationState();

                NCASServer.Singleton.ClearCCUUpgradeResources(GuidCCU);

                CCUConfigurationHandler.Singleton.InitCcuFirmwareVersion(
                    this,
                    ref configurationState);

                ChangeConfigurationState(configurationState);

                if (ccu != null && NcasServer != null)
                {
                    UpdateCcuAlarms(
                        ccu,
                        true,
                        ConfigurationState);

                    CreateAlarmCcuOutdatedFirmware(
                        ccu,
                        CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU));
                }

                CheckIsCCU0();

                SetIpSettings(CCUConfigurationHandler.Singleton.GetIPSettingsFromCCU(GuidCCU));

                CCUConfigurationHandler.Singleton.InitCcuMacAddress(GuidCCU);
                CCUConfigurationHandler.Singleton.InitMainBoardType(GuidCCU);

                NCASServer.Singleton.ClearCCUUpgradeResources(GuidCCU);

                CCUConfigurationHandler.Singleton.BindEventsAllCcu(GuidCCU);

                bool unconfigureAfterInit = false;

#if !DEBUG
                unconfigureAfterInit = CCUConfigurationHandler.Singleton.InitCcuLicence(
                    this,
                    ccu,
                    configurationState);
#endif

                if (!unconfigureAfterInit)
                {
                    switch (ConfigurationState)
                    {
                        case CCUConfigurationState.ForceReconfiguration:
                        case CCUConfigurationState.ConfiguredForThisServer:

                            if (DataReplicationManager.Singleton.InitialSendModifiedObjectsToCcu(GuidCCU))
                                _initDataBatchSent.WaitOne(
                                    MAX_DELEAY_FOR_BLOCK_SENDING_OBJECTS,
                                    false);

                            CCUConfigurationHandler.Singleton.SendNTPTimeDifferenceTolerance(
                                GuidCCU,
                                CCUConfigurationHandler.Singleton.TimeDifferenceTolerance);

                            BindingInterCCUCommunication.Singleton.MustSendingBindingsToCCU(GuidCCU);

                            CCUConfigurationHandler.Singleton.SetEnableLoggingSDPSTZChanges(
                                GuidCCU,
                                GeneralOptions.Singleton.EnableLoggingSDPSTZChanges);

                            CCUConfigurationHandler.Singleton.SetTimeSyncingFromServer(
                                GuidCCU,
                                GeneralOptions.Singleton.SyncingTimeFromServer);

                            CCUConfigurationHandler.Singleton.SetAlarmAreaRestrictivePolicyForTimeBuying(
                                GuidCCU,
                                GeneralOptions.Singleton.AlarmAreaRestrictivePolicyForTimeBuying);

                            StartStopSendingTimeFromServer();

                            CCUConfigurationHandler.Singleton.SetOldEventsSettings(GuidCCU);
                            CCUConfigurationHandler.Singleton.BindEvents(GuidCCU);
                            CCUConfigurationHandler.Singleton.InitInputs(GuidCCU);
                            CCUConfigurationHandler.Singleton.InitOutputs(GuidCCU);
                            CCUConfigurationHandler.Singleton.InitDoorEnvironments(GuidCCU, MainBoardType);
                            CCUConfigurationHandler.Singleton.SetAllowPINCachingInCardReaderMenu(GuidCCU);
                            CCUConfigurationHandler.Singleton.SetAllowCodeLength(GuidCCU);
                            CCUConfigurationHandler.Singleton.SetPinConfirmationObligatory(GuidCCU);
                            CCUConfigurationHandler.Singleton.SendAllStatesToServer(GuidCCU);

                            NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                                GuidCCU,
                                externalAlarmsOwner =>
                                    externalAlarmsOwner.AlarmOwnerConnected());

                            if (CCUConfigurationHandler.Singleton.IsSupportedWaitingWhileIncommingStreamIsInProcessing(GuidCCU))
                            {
                                //TODO

                                //if (!CCUConfigurationHandler.Singleton.WaitForFinishProcessingStream(GuidCCU, string.Empty))
                                //{
                                //    Eventlogs.Singleton.InsertEvent(
                                //        Eventlog.TYPE_CCU_PROCESSING_STREAM_TAKES_TOO_LONG_TIME,
                                //        DateTime.Now,
                                //        GetType().Assembly.GetName().Name,
                                //        new[] { GuidCCU },
                                //        "Processing of the streams on the CCU takes too long time");
                                //}
                            }

                            break;

                        case CCUConfigurationState.ConfiguredForThisServerUpgradeOnly:

                            CCUConfigurationHandler.Singleton.BindEvents(GuidCCU);
                            break;

                        default:

                            if (ccu != null)
                                NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                                    ccu.IdCCU,
                                    externalAlarmsOwner =>
                                        externalAlarmsOwner.RemoveAllAlarms());

                            break;
                    }
                }
                else
                {
                    SafeThread<Guid>.StartThread(
                        idCcu => CCUConfigurationHandler.Singleton.Unconfigure(idCcu),
                        GuidCCU);
                }
            }
            catch
            {
                preInitCcuAndResult.Result = false;
            }
            finally
            {
                _threadDoInitCCU = null;
                _sendingObjectsToCCUAllowed.Set();
                _doInitCCUFinished.Set();
            }
        }

        private bool? _enabledAlarmCCUOffline;
        private bool? _blockAlarmCCUOffline;
        private byte? _blockAlarmCCUOfflineObjectType;
        private Guid? _blockAlarmCCUOfflineObjectId;

        public void ChangeSettingsForAlarmCCUOffline(CCU ccu, bool forcedChangeAlarm)
        {
            bool? actEnabledAlarmCCUOffline = ccu.AlarmOffline;
            bool? actBlockAlarmCCUOffline = ccu.BlockAlarmOffline;
            byte? actBlockAlarmCCUOfflineObjectType = ccu.ObjBlockAlarmOfflineObjectType;
            Guid? actBlockAlarmCCUOfflineObjectId = ccu.ObjBlockAlarmOfflineId;
            bool wasChanged = false;

            if (_enabledAlarmCCUOffline != actEnabledAlarmCCUOffline ||
                _blockAlarmCCUOffline != actBlockAlarmCCUOffline ||
                _blockAlarmCCUOfflineObjectType != actBlockAlarmCCUOfflineObjectType ||
                _blockAlarmCCUOfflineObjectId != actBlockAlarmCCUOfflineObjectId)
            {
                _enabledAlarmCCUOffline = actEnabledAlarmCCUOffline;
                _blockAlarmCCUOffline = actBlockAlarmCCUOffline;
                _blockAlarmCCUOfflineObjectType = actBlockAlarmCCUOfflineObjectType;
                _blockAlarmCCUOfflineObjectId = actBlockAlarmCCUOfflineObjectId;

                wasChanged = true;
            }

            if (forcedChangeAlarm || wasChanged)
                ChangeAlarmCCUOffline(null, null);
        }

        public void ChangeAlarmCCUOffline(ObjectType? objectType, Guid? objectGuid)
        {
            if (objectType != null && objectGuid != null)
            {
                bool condition =
                    _blockAlarmCCUOffline == null
                        ? CCUAlarms.Singleton
                            .IsRelevantObjectForBlockingAlarmCCUOffline(
                                objectType.Value,
                                objectGuid.Value)
                        : _blockAlarmCCUOffline.Value &&
                              (byte?)objectType == _blockAlarmCCUOfflineObjectType &&
                              objectGuid == _blockAlarmCCUOfflineObjectId;

                if (!condition)
                    return;
            }

            CCU ccu = CCUs.Singleton.GetById(GuidCCU);
            if (ccu == null)
                return;

            UpdateAlarmCcuOffline(
                ccu,
                State == CCUOnlineState.Online);
        }

        private bool IsBlockedAlarmCCUOffline()
        {
            if (_blockAlarmCCUOffline == null)
                return CCUAlarms.Singleton.IsBlockedAlarmCCUOffline();

            if (!_blockAlarmCCUOffline.Value ||
                    _blockAlarmCCUOfflineObjectType == null ||
                    _blockAlarmCCUOfflineObjectId == null)
                return false;

            switch (_blockAlarmCCUOfflineObjectType.Value)
            {
                case (byte)ObjectType.DailyPlan:
                    return TimeAxis.Singleton.GetActualStatusDP(_blockAlarmCCUOfflineObjectId.Value) == TimeAxis.ON;

                case (byte)ObjectType.TimeZone:
                    return TimeAxis.Singleton.GetActualStatusTZ(_blockAlarmCCUOfflineObjectId.Value) == TimeAxis.ON;
            }

            return false;
        }

        public void UpdateCcuAlarms(CCU ccu, bool isOnline, CCUConfigurationState configurationState)
        {
            if (!isOnline)
            {
                UpdateAlarmCcuOffline(ccu, false);
                CCUAlarms.StopAlarmCcuClockUnsynchronized(ccu.IdCCU);
                CCUAlarms.StopDataChannelTransferAlarms(ccu.IdCCU);
                StopAlarmCcuOutdatedFirmware();
                DeleteAlarmCCUUnconfigured();
                UpdateAlarmsDcuCrOfflineDueCcuOffline(ccu);

                return;
            }

            UpdateAlarmsDcuCrOfflineDueCcuOffline(ccu);

            if (configurationState == CCUConfigurationState.ConfiguredForAnotherServer)
            {
                NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                    ccu.IdCCU,
                    externalAlarmsOwner =>
                        externalAlarmsOwner.RemoveAllAlarms());

                return;
            }

            if (configurationState == CCUConfigurationState.Unconfigured)
            {
                NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                    ccu.IdCCU,
                    externalAlarmsOwner =>
                        externalAlarmsOwner.RemoveAllAlarms());

                UpdateAlarmCcuUnconfigured();

                return;
            }

            UpdateAlarmCcuOffline(ccu, true);
            DeleteAlarmCCUUnconfigured();
        }

        private void UpdateAlarmsDcuCrOfflineDueCcuOffline(
            CCU ccu)
        {
            if (!ccu.IsConfigured)
                return;

            if (ccu.DCUs != null)
            {
                foreach (var dcu in ccu.DCUs)
                {
                    CCUAlarms.UpdateAlarmDcuOfflineDueCcuOffline(
                        dcu,
                        State);

                    if (dcu.CardReaders != null)
                    {
                        foreach (var cardReader in dcu.CardReaders)
                        {
                            CCUAlarms.UpdateAlarmCrOfflineDueCcuOffline(
                                cardReader,
                                State);
                        }
                    }
                }
            }

            if (ccu.CardReaders != null)
            {
                foreach (var cardReader in ccu.CardReaders)
                {
                    CCUAlarms.UpdateAlarmCrOfflineDueCcuOffline(
                        cardReader,
                        State);
                }
            }
        }

        public void UpdateAlarmCcuOffline(CCU ccu, bool isOnline)
        {
            if (IsBlockedAlarmCCUOffline())
            {
                CcuOfflineServerAlarm.StopAlarm(ccu.IdCCU);
                return;
            }

            if (!isOnline)
            {
                if (!ccu.IsConfigured)
                    return;

                bool alarmEnabled =
                    ccu.AlarmOffline != null
                        ? (bool)ccu.AlarmOffline
                        : DevicesAlarmSettings.Singleton.AlarmCcuOffline();

                if (!alarmEnabled)
                {
                    CcuOfflineServerAlarm.StopAlarm(ccu.IdCCU);
                    return;
                }
            }

            // ReSharper disable once ObjectCreationAsStatement, enqued in constructor
            if (isOnline)
            {
                CcuOfflineServerAlarm.StopAlarm(ccu.IdCCU);
                return;
            }

            CcuOfflineServerAlarm.AddAlarm(ccu);
        }

        private void ChangeOnlineState(CCUOnlineState state)
        {
            if (State == state)
                return;

            State = state;

            switch (State)
            {
                case CCUOnlineState.Online:

                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_CCU_ONLINE,
                        DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { GuidCCU },
                        string.Format(
                            "CCU {0} changed its online state to online",
                            CcuName),
                        new EventlogParameter(
                            EventlogParameter.TYPECCU,
                            IPAddressString,
                            GuidCCU,
                            ObjectType.CCU));

                    break;

                case CCUOnlineState.Offline:

                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_CCU_OFFLINE,
                        DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { GuidCCU },
                        string.Format(
                            "CCU {0} changed its online state to offline",
                            CcuName),
                        new EventlogParameter(
                            EventlogParameter.TYPECCU,
                            IPAddressString,
                            GuidCCU,
                            ObjectType.CCU),
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(GuidCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(GuidCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));

                    break;
            }

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunCCUStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);

            if (State != CCUOnlineState.Offline)
                return;

            ChangeConfigurationState(CCUConfigurationState.Unknown);
            DataReplicationManager.Singleton.RemoveCcuDataReplicator(GuidCCU);
        }

        public void ChangeConfigurationState(CCUConfigurationState configurationState)
        {
            if (_runningForceReconfiguration && configurationState != CCUConfigurationState.ForceReconfiguration)
                return;

            ConfigurationState = configurationState;

            try
            {
                if (configurationState == CCUConfigurationState.ConfiguredForThisServer ||
                    configurationState == CCUConfigurationState.ConfiguredForThisServerUpgradeOnly ||
                    configurationState == CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe)
                {
                    CCU ccuSave = CCUs.Singleton.GetById(GuidCCU);

                    if (ccuSave != null && !ccuSave.IsConfigured)
                    {
                        ccuSave = CCUs.Singleton.GetObjectForEdit(GuidCCU);
                        ccuSave.IsConfigured = true;
                        CCUs.Singleton.UpdateOnlyInDatabase(ccuSave);
                        CCUs.Singleton.EditEnd(ccuSave);
                    }
                }
                else if (configurationState == CCUConfigurationState.ConfiguredForAnotherServer ||
                         configurationState == CCUConfigurationState.Unconfigured)
                {
                    CCU ccuSave = CCUs.Singleton.GetById(GuidCCU);

                    if (ccuSave != null && ccuSave.IsConfigured)
                    {
                        ccuSave = CCUs.Singleton.GetObjectForEdit(GuidCCU);
                        ccuSave.IsConfigured = false;
                        CCUs.Singleton.UpdateOnlyInDatabase(ccuSave);
                        CCUs.Singleton.EditEnd(ccuSave);
                    }
                }
            }
            catch { }

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunCCUConfiguredChanged, DelegateSequenceBlockingMode.Asynchronous, false);
        }


        private bool _isCreatedAlarmCCUUnconfigured;
        private readonly object _lockCrateDeleteAlarmCCUUnconfigured = new object();

        public void UpdateAlarmCcuUnconfigured()
        {
            if (ConfigurationState != CCUConfigurationState.Unconfigured)
            {
                DeleteAlarmCCUUnconfigured();
                return;
            }

            if (!DevicesAlarmSettings.Singleton.AlarmCcuUnconfigured())
            {
                DeleteAlarmCCUUnconfigured();
                return;
            }

            if (CCUAlarms.Singleton.IsBlockedAlarmCCUUnconfigured())
            {
                DeleteAlarmCCUUnconfigured();
                return;
            }

            if (_isCreatedAlarmCCUUnconfigured)
                return;

            CCU ccu = CCUs.Singleton.GetById(GuidCCU);
            if (ccu == null)
            {
                _isCreatedAlarmCCUUnconfigured = true;
                return;
            }

            CcuUnconfiguredServerAlarm.AddCcuUnconfiguredAlarm(ccu);
            _isCreatedAlarmCCUUnconfigured = true;
        }

        public void DeleteAlarmCCUUnconfigured()
        {
            lock (_lockCrateDeleteAlarmCCUUnconfigured)
            {
                if (!_isCreatedAlarmCCUUnconfigured)
                    return;

                CCU ccu = CCUs.Singleton.GetById(GuidCCU);

                if (ccu == null)
                {
                    _isCreatedAlarmCCUUnconfigured = false;
                    return;
                }

                CcuUnconfiguredServerAlarm.RemoveAlarm(ccu.IdCCU);

                _isCreatedAlarmCCUUnconfigured = false;
            }
        }

        private bool _connectionFailedAlarm;

        void ClientConnectionFailed(Exception parameter)
        {
            lock (_clientConnectionEventLock)
            {
                if (!_connectionFailedAlarm)
                {
                    CCU ccu = CCUs.Singleton.GetById(GuidCCU);

                    if (ccu != null &&
                        NcasServer != null)
                    {
                        _connectionFailedAlarm = true;

                        UpdateCcuAlarms(
                            ccu,
                            false,
                            ConfigurationState);

                        if (ccu.IsConfigured)
                        {
                            CCUConfigurationHandler.Singleton.ClearDCUOnlineStates(
                                IPAddressString,
                                DcuStates);
                        }
                    }
                }

                try
                {
                    if (State != CCUOnlineState.Offline)
                        ChangeOnlineState(CCUOnlineState.Offline);
                }
                catch
                { }
                _runningForceReconfiguration = false;
            }
        }

        public NCASServer NcasServer
        {
            get { return _ncasServer ?? (_ncasServer = CCUConfigurationHandler.Singleton.NcasServer); }
        }

        private void ClientDisconnected(IPEndPoint parameter)
        {
            lock (_clientConnectionEventLock)
            {
                CCUConfigurationState oldConfigured = ConfigurationState;

                try
                {
                    ChangeOnlineState(CCUOnlineState.Offline);
                }
                catch
                { }

                StopThreadDoInitCCU();

                CCU ccu = CCUs.Singleton.GetById(GuidCCU);
                if (ccu != null && NcasServer != null)
                {
                    UpdateCcuAlarms(
                        ccu,
                        false,
                        ConfigurationState);
                }

                ChangeConfigurationState(CCUConfigurationState.Unknown);
                SetIpSettings(null);

                if (oldConfigured == CCUConfigurationState.ConfiguredForThisServer ||
                    oldConfigured == CCUConfigurationState.ForceReconfiguration ||
                    oldConfigured == CCUConfigurationState.Upgrading)
                {
                    CCUConfigurationHandler.Singleton.ClearInputsStates(InputStates);
                    CCUConfigurationHandler.Singleton.ClearOutputsStates(OutputStates);
                    CCUConfigurationHandler.Singleton.ClearAlarmAreasStates(GuidCCU);
                    CCUConfigurationHandler.Singleton.ClearDoorEnvironmentStates(DoorEnvironmentStates);
                    CCUConfigurationHandler.Singleton.ClearCardReaderOnlineStates(CardReaderStates);
                    CCUConfigurationHandler.Singleton.ClearDCUOnlineStates(IPAddressString, DcuStates);
                    AntiPassBackZones.Singleton.ClearAntiPassBackZonesContent(GuidCCU);

                    NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                        GuidCCU,
                        externalAlarmsOwner =>
                            externalAlarmsOwner.AlarmOwnerDisconnected());
                }
                _runningForceReconfiguration = false;
            }
        }

        public bool Unconfigure()
        {
            if (!CCUConfigurationHandler.Singleton.CCUUnconfigure(GuidCCU))
                return false;

            ComboLicenceManager.Singleton.ReleaseLicence(GuidCCU);

            ChangeConfigurationState(CCUConfigurationState.Unconfigured);

            CCUConfigurationHandler.Singleton.ClearInputsStates(InputStates);
            CCUConfigurationHandler.Singleton.ClearOutputsStates(OutputStates);
            CCUConfigurationHandler.Singleton.ClearAlarmAreasStates(GuidCCU);
            CCUConfigurationHandler.Singleton.ClearDoorEnvironmentStates(DoorEnvironmentStates);
            CCUConfigurationHandler.Singleton.ClearCardReaderOnlineStates(CardReaderStates);
            CCUConfigurationHandler.Singleton.ClearDCUOnlineStates(IPAddressString, DcuStates);
            DataReplicationManager.Singleton.RemoveCcuDataReplicator(GuidCCU);

            var alarmsManager = NCASServer.Singleton.GetAlarmsQueue();

            alarmsManager.TryRunOnAlarmsOwner(
                GuidCCU,
                externalAlarmsOwner =>
                    externalAlarmsOwner.RemoveAllAlarms());

            CcuClockUnsynchronizedServerAlarm.RemoveAlarm(GuidCCU);

            UpdateAlarmCcuUnconfigured();

            CCUConfigurationHandler.Singleton.BindEventsAllCcu(GuidCCU);
            AntiPassBackZones.Singleton.ClearAntiPassBackZonesContent(GuidCCU);

            CCUEvents.ClearAllEventsAndTransfers();

            return true;
        }

        private bool ConfigureForThisServerPreInit()
        {
            try
            {
                if (!CCUConfigurationHandler.Singleton.CCUConfigureForThisServer(GuidCCU))
                    return false;

                CCUConfigurationHandler.Singleton.ClearInputsStates(InputStates);
                CCUConfigurationHandler.Singleton.ClearOutputsStates(OutputStates);
                CCUConfigurationHandler.Singleton.ClearAlarmAreasStates(GuidCCU);
                CCUConfigurationHandler.Singleton.ClearDoorEnvironmentStates(
                    DoorEnvironmentStates);
                CCUConfigurationHandler.Singleton.ClearCardReaderOnlineStates(
                    CardReaderStates);
                CCUConfigurationHandler.Singleton.ClearDCUOnlineStates(IPAddressString, DcuStates);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ConfigureResult ConfigureForThisServer()
        {
#if !DEBUG
            if (IsCat12Combo
                && ComboLicenceManager.Singleton.FreeLicenceCount == 0
                && !ComboLicenceManager.Singleton.HasLicence(GuidCCU))
            {
                return ConfigureResult.LicenceFailure;
            }
#endif

            return DoInitCcu(ConfigureForThisServerPreInit);
        }

        private bool ForceReconfigurationPreInit()
        {
            try
            {
                if (!CCUConfigurationHandler.Singleton.SendForceReconfigurationCommand(GuidCCU))
                    return false;

                ChangeConfigurationState(CCUConfigurationState.ForceReconfiguration);
                CCUConfigurationHandler.Singleton.ClearInputsStates(InputStates);
                CCUConfigurationHandler.Singleton.ClearOutputsStates(OutputStates);
                CCUConfigurationHandler.Singleton.ClearAlarmAreasStates(GuidCCU);
                CCUConfigurationHandler.Singleton.ClearDoorEnvironmentStates(
                    DoorEnvironmentStates);
                CCUConfigurationHandler.Singleton.ClearCardReaderOnlineStates(
                    CardReaderStates);
                CCUConfigurationHandler.Singleton.ClearDCUOnlineStates(IPAddressString, DcuStates);
                DataReplicationManager.Singleton.RemoveCcuDataReplicator(GuidCCU);
                AntiPassBackZones.Singleton.ClearAntiPassBackZonesContent(GuidCCU);

                CCUEvents.ClearAllEventsAndTransfers();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool _runningForceReconfiguration;

        public ConfigureResult ForceReconfiguration()
        {
            _runningForceReconfiguration = true;

            try
            {
                return DoInitCcu(ForceReconfigurationPreInit);
            }
            finally
            {
                _runningForceReconfiguration = false;

                ChangeConfigurationState(CCUConfigurationState.ConfiguredForThisServer);
                _initialObjectSending = true;
            }
        }

        private void RunCCUStateChanged(ARemotingCallbackHandler remoteHandler)
        {
            var stateChangedCCUHandler =
                remoteHandler as StateChangedCCUHandler;

            if (stateChangedCCUHandler != null)
                stateChangedCCUHandler.RunEvent(
                    GuidCCU,
                    (byte)State);
        }

        public void RunCCUConfiguredChanged(ARemotingCallbackHandler remoteHandler)
        {
            var configuredChangedCCUHandler =
                remoteHandler as ConfiguredChangedCCUHandler;

            if (configuredChangedCCUHandler != null)
                configuredChangedCCUHandler.RunEvent(
                    GuidCCU,
                    (byte)ConfigurationState,
                    IsCCU0,
                    IsCat12Combo
                        && (ConfigurationState == CCUConfigurationState.Unconfigured)
                        && (ComboLicenceManager.Singleton.FreeLicenceCount == 0)
                        && (!ComboLicenceManager.Singleton.HasLicence(GuidCCU)));
        }

        private void SetIpSettings(IPSetting ipSetting)
        {
            IPSetting = ipSetting;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunIPSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        private void RunIPSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var ipSettingsChangedHandler =
                remoteHandler as IPSettingsChangedHandler;

            if (ipSettingsChangedHandler != null)
                ipSettingsChangedHandler.RunEvent(GuidCCU, IPSetting);
        }

        public string SetIpSettingsOnTheCCU(IPSetting ipSetting)
        {
            string retValue = string.Empty;

            if (!CCUConfigurationHandler.Singleton.SetIPSettingsToCCU(GuidCCU, ipSetting))
                return retValue;

            CCU ccu = CCUs.Singleton.GetById(GuidCCU);
            if (ccu == null)
                return retValue;

            string oldCCUIPAddress = ccu.IPAddress;

            if (ipSetting.IsDHCP)
                retValue = ccu.IPAddress;
            else
                if (ccu.IPAddress == ipSetting.IPAddress)
                retValue = ipSetting.IPAddress;
            else
            {
                ccu = CCUs.Singleton.GetObjectForEdit(ccu.IdCCU);

                if (ccu != null)
                {
                    ccu.IPAddress = ipSetting.IPAddress;

                    if (CCUs.Singleton.Update(ccu))
                        retValue = ipSetting.IPAddress;

                    CCUs.Singleton.EditEnd(ccu);
                }
            }

            if (retValue != string.Empty &&
                    (ipSetting.IsDHCP || oldCCUIPAddress == ipSetting.IPAddress))
                SetIpSettings(
                    CCUConfigurationHandler.Singleton
                        .GetIPSettingsFromCCU(GuidCCU));

            return retValue;
        }

        private void CheckIsCCU0()
        {
            IsCCU0 = CCUConfigurationHandler.Singleton.IsCCU0FromCCU(GuidCCU);
        }

        private void CheckCCUUpgrader()
        {
            while (State == CCUOnlineState.Online)
            {
                bool? isCCUUpgrader = CCUConfigurationHandler.Singleton.IsCCUUpgraderFromCCU(GuidCCU);
                if (isCCUUpgrader != null)
                {
                    IsCCUUpgrader = isCCUUpgrader.Value;
                    return;
                }

                Thread.Sleep(DELAY_FOR_REPEATING_OF_CHECKING_CONFIGURATION);
            }

            IsCCUUpgrader = false;
        }

        private CCUConfigurationState GetCcuConfigurationState()
        {
            while (State == CCUOnlineState.Online)
            {
                var ccuConfigurationState = CCUConfigurationHandler.Singleton.CCUConfigured(GuidCCU);
                if (ccuConfigurationState != CCUConfigurationState.Unknown)
                {
                    return ccuConfigurationState;
                }

                Thread.Sleep(DELAY_FOR_REPEATING_OF_CHECKING_CONFIGURATION);
            }

            return CCUConfigurationState.Unknown;
        }

        public int GetDcuUpgradePercentage(byte logicalAddressDCU)
        {
            return
                _dcusUpgradePercents.Keys.Contains(logicalAddressDCU)
                    ? _dcusUpgradePercents[logicalAddressDCU]
                    : 0;
        }

        public void StopUpgradeMode(bool upgraded)
        {
            ChangeConfigurationState(
                upgraded
                    ? CCUConfigurationState.Unconfigured
                    : CCUConfigurationState.ConfiguredForThisServer);
        }

        internal void StartUpgradeMode()
        {
            ChangeConfigurationState(CCUConfigurationState.Upgrading);
        }

        public int[] CommunicationStatistic()
        {
            var server = Client.Statistics();

            var ccu =
                CCUConfigurationHandler.Singleton
                    .CcuCommunicationStatistic(GuidCCU);

            var result = new int[20];

            if (server != null)
                server.CopyTo(result, 0);

            if (ccu != null)
                ccu.CopyTo(result, 7);

            result[18] = CCUEvents.GetNumberOfNotStoredEvents();
            result[19] = CCUEvents.GetNumberOfNotProcessedEvents();

            return result;
        }

        private NCASServer _ncasServer;


        private void FixMaximumCardReadersForCcu()
        {
            switch (_mainBoardType)
            {
                case MainBoardVariant.CCU40:
                    CardReadersInBounds(GuidCCU, NCASConstants.CCU40_MAX_CR_COUNT);
                    break;

                case MainBoardVariant.CCU05:
                    CardReadersInBounds(GuidCCU, NCASConstants.CCU05_MAX_CR_COUNT);
                    break;

                case MainBoardVariant.CCU12:
                    CardReadersInBounds(GuidCCU, NCASConstants.CCU12_MAX_CR_COUNT);
                    break;

                case MainBoardVariant.CAT12CE:
                    CardReadersInBounds(GuidCCU, NCASConstants.CAT12CE_MAX_CR_COUNT);
                    break;
            }
        }

        private static void CardReadersInBounds(Guid idCcu, byte maxCr)
        {
            CCU ccu = CCUs.Singleton.GetById(idCcu);
            if (ccu == null || ccu.MaxNodeLookupSequence <= maxCr)
                return;

            CCU editCcu = CCUs.Singleton.GetObjectForEdit(idCcu);
            editCcu.MaxNodeLookupSequence = maxCr;

            //Exception error;
            CCUs.Singleton.Update(editCcu);
            CCUs.Singleton.EditEnd(editCcu);
        }

        /// <summary>
        /// Set allow PIN caching in card reader menu
        /// </summary>
        public void SetAllowPINCachingInCardReaderMenu()
        {
            if (ConfigurationState == CCUConfigurationState.ConfiguredForThisServer ||
                    ConfigurationState == CCUConfigurationState.ForceReconfiguration)
                CCUConfigurationHandler.Singleton.SetAllowPINCachingInCardReaderMenu(GuidCCU);
        }

        public void SetAllowCodeLength()
        {
            if (ConfigurationState == CCUConfigurationState.ConfiguredForThisServer ||
                    ConfigurationState == CCUConfigurationState.ForceReconfiguration)
                CCUConfigurationHandler.Singleton.SetAllowCodeLength(GuidCCU);
        }

        public void SetPinConfirmationObligatory()
        {
            if (ConfigurationState == CCUConfigurationState.ConfiguredForThisServer ||
                    ConfigurationState == CCUConfigurationState.ForceReconfiguration)
                CCUConfigurationHandler.Singleton.SetPinConfirmationObligatory(GuidCCU);
        }

        public void ChangeAlarmsCROffline(
            ObjectType? objectType,
            Guid? objectGuid)
        {
            if (CardReaderStates == null)
                return;

            foreach (var cardReaderState in CardReaderStates.Values)
                if (cardReaderState != null)
                    cardReaderState.ChangeAlarmCROffline(
                        objectType,
                        objectGuid);
        }

        public void ChangeAlarmDcuOfflineDueCcuOffline(
            ObjectType? objectType,
            Guid? objectGuid)
        {
            if (DcuStates == null)
                return;

            foreach (var dcuState in DcuStates.Values)
                if (dcuState != null)
                    dcuState.ChangeAlarmDcuOfflineDueCcuOffline(
                        objectType,
                        objectGuid);
        }

        private const int WRONG_TIME_DIFFERENCE_TOLERANCE_COUNT = 2;
        private int _wrongTimeDiffCount;

        private IPEndPoint _lwMessagingRemoteEndpoint;

        public void CcuActualTimeReceived(
            DateTime dateTimeCcu,
            DateTime dateTimeServer)
        {
            try
            {
                var resultCompareTime = dateTimeCcu - dateTimeServer;

                var totalDiffrence = Math.Abs(resultCompareTime.TotalMilliseconds);

                if (totalDiffrence > CCUConfigurationHandler.Singleton.TimeDifferenceTolerance)
                {
                    if (_wrongTimeDiffCount <= WRONG_TIME_DIFFERENCE_TOLERANCE_COUNT)
                    {
                        _wrongTimeDiffCount++;
                        return;
                    }

                    if (!DevicesAlarmSettings.Singleton.AlarmCcuClockUnsynchronized())
                        return;

                    _wrongTimeDiffCount = 0;

                    CCUAlarms.CreateAlarmCcuClockUnsynchronized(
                        GuidCCU,
                        IPAddressString,
                        dateTimeCcu,
                        dateTimeServer);
                }
                else
                {
                    _wrongTimeDiffCount = 0;

                    CCUAlarms.StopAlarmCcuClockUnsynchronized(GuidCCU);
                }
            }
            catch
            {
            }
        }

        private static IEnumerable<T[]> FragmentIntoBatches<T>(
            ICollection<T> source,
            int batchSize)
        {
            int remainingCount = source.Count;

            var currentBatch =
                new T[
                    Math.Min(
                        batchSize,
                        remainingCount)];

            remainingCount -= currentBatch.Length;

            int idxInBatch = 0;

            foreach (var item in source)
            {
                currentBatch[idxInBatch] = item;

                if (++idxInBatch < currentBatch.Length)
                    continue;

                yield return currentBatch;

                if (remainingCount <= 0)
                    continue;

                currentBatch =
                    new T[
                        Math.Min(
                            batchSize,
                            remainingCount)];

                idxInBatch = 0;
                remainingCount -= currentBatch.Length;
            }
        }
    }
}