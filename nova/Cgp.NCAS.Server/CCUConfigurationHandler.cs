using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Alarms;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using System.Net;

using Contal.IwQuick.Net;
using System.IO;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using System.Collections;

using Contal.Cgp.NCAS.Definitions;
using Contal.LwMessaging;

using CardReader = Contal.Cgp.NCAS.Server.Beans.CardReader;
using CRHWVersion = Contal.Drivers.CardReader.CRHWVersion;
using Ionic.Zip;

namespace Contal.Cgp.NCAS.Server
{
    public sealed class CCUConfigurationHandler : AMbrSingleton<CCUConfigurationHandler>
    {
        private const string CNMP_TYPE = "/Cgp/NCAS/NCASServer";
        private const string CnmpLookupFromCcuKey = "577ccf9d9d6cd14502a4f09ea1c182e89f22b097";
        private const string CnmpCeLookupKey = "6b5d7558e05c81805a0219bee521f75148c5027666509d7dd9c6c765ade37411";
        private const string CNMPSERVERLOOKUPKEY = "a20bdcaba7ae09c7ac4bba2e4cd02179af94587a";

        public static string CCU_DUMPS_DIRECTORY
        {
            get
            {
                return string.Format(
                    "{0}CCUdumps\\",
                    AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        private int _ccuDebugFileReceivedCounter;
        private int _ccuDebugFilesCount;
        private bool _loadingCcuDebugFiles;

        private readonly ExtendedVersion _version = new ExtendedVersion(
            typeof(CCUConfigurationHandler),
            true, true, String.Empty, DevelopmentStage.Alpha, "EN");

        private readonly CNMPAgent _cnmpAgent;
        private readonly DataChannelPeer _dataChannel;
        private readonly object _getDebugFilesLock = new object();
        private readonly AutoResetEvent _pullDataChannelPeerAutoResetEvent = new AutoResetEvent(false);
        private readonly string _thisAssemblyName;
        private readonly Dictionary<Guid, TimeBuyingMatrixValue> _timeBuyingMatrixValues = new Dictionary<Guid, TimeBuyingMatrixValue>();

        public DataChannelPeer DataChannel
        {
            get { return _dataChannel; }
        }

        private readonly Dictionary<Guid, CCUSettings> _remotingCCUs =
            new Dictionary<Guid, CCUSettings>();

        private readonly ProcessingQueue<ActivationStateChangedCarrier> _queueAlarmAreaRequestActivationStateChanged;

        internal class ActivationStateChangedCarrier
        {
            public Action MethodToInvoke { get; private set; }
            public object[] Data { get; private set; }

            public ActivationStateChangedCarrier(params object[] data)
            {
                MethodToInvoke = null;
                Data = data;
            }

            public ActivationStateChangedCarrier(Action methodToInvoke)
            {
                MethodToInvoke = methodToInvoke;
                Data = null;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public NCASServer NcasServer { get; set; }

        private readonly string[] _getExtras = { "apps" };
        private readonly ICollection<Guid> _ccusLookupingClients = new LinkedList<Guid>();
        private readonly ICollection<Guid> _alarmTransmittersLookupingClients = new LinkedList<Guid>();

        private CCUConfigurationHandler()
            : base(null)
        {
            _thisAssemblyName =
                GetType()
                    .Assembly.GetName()
                    .Name;

            _cnmpAgent = new CNMPAgent(
                CNMP_TYPE + StringConstants.SLASH + Environment.Version,
                CNMP_TYPE,
                CnmpLookupFromCcuKey,
                _version.ToString());

            _cnmpAgent.Timeout = 3500;
            _cnmpAgent.RetryCount = 1;

            _cnmpAgent.ValidRequestReceived += OnCnmpValidRequestReceived;
            _cnmpAgent.LookupFinished += OnCnmpAgentLookupFinished;
            _cnmpAgent.Start();

            _queueAlarmAreaRequestActivationStateChanged = new ProcessingQueue<ActivationStateChangedCarrier>();

            _queueAlarmAreaRequestActivationStateChanged.ItemProcessing +=
                CCUCallbackRunner.RunAlarmAreaRequestActivationStateChanged;

            _dataChannel = new DataChannelPeer();
            _dataChannel.TransferResult += DataChannelTransferResult;
            _dataChannel.TransferProgress += DataChannelTransferProgress;
            _dataChannel.IncomingTransferResult += DataChannelIncomingTransferResult;
        }

        public void Init()
        {
            LoadImplictCCUForAlarmAreas();
            BindingInterCCUCommunication.Singleton.Init();
        }

        private static void DataChannelTransferProgress(
            DataChannelPeer.TransferInfo transferInfo,
            int progress)
        {
            NCASServer.Singleton.DataChannelTransferProgress(
                transferInfo,
                progress);
        }

        private static void DataChannelTransferResult(
            DataChannelPeer.TransferInfo transferDescriptor,
            bool success)
        {
            NCASServer.Singleton.DataChannelTransferResult(
                transferDescriptor,
                success);
        }

        private void DataChannelIncomingTransferResult(
            DataChannelPeer.TransferInfo transferInfo,
            bool success,
            out bool closeStreamAfterProcessing)
        {
            closeStreamAfterProcessing = true;

            try
            {
                if (transferInfo == null ||
                    transferInfo.RemoteEndPoint == null ||
                    transferInfo.RemoteEndPoint.Address == null)
                    return;

                var ccu =
                    CCUs.Singleton.GetCCUFormIpAddress(transferInfo.RemoteEndPoint.Address.ToString());

                if (ccu == null)
                    return;

                if (!success || transferInfo.DestinationStream == null)
                {
                    var destinationName = transferInfo.DestinationName.Trim(StringConstants.BACKSLASH[0]);

                    TCPTransferFailed(
                        ccu,
                        ccu.IdCCU,
                        destinationName,
                        "Data channel error",
                        new EventlogParameter(
                            EventlogParameter.TYPECCU,
                            transferInfo.RemoteEndPoint.Address.ToString(),
                            ccu.IdCCU,
                            ObjectType.CCU),
                        new EventlogParameter(
                            EventlogParameter.TYPE_STREAM_NAME,
                            destinationName),
                        new EventlogParameter(
                            "Error code",
                            transferInfo.LastErrorCode.ToString()),
                        new EventlogParameter(
                            "Error message",
                            transferInfo.LastErrorMessage));

                    return;
                }

                if (transferInfo.DestinationName.Contains("ccu.debug."))
                {
                    try
                    {
                        transferInfo.DestinationStream.Position = 0;
                        var file = File.Create(transferInfo.DestinationName);
                        var buffer = new byte[1024];
                        int readBytes;

                        do
                        {
                            readBytes = transferInfo.DestinationStream.Read(buffer, 0, buffer.Length);
                            file.Write(buffer, 0, readBytes);
                        } while (readBytes > 0);

                        file.Close();

                        _ccuDebugFileReceivedCounter++;

                        NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                            CCUCallbackRunner.RunCCUMakeLogDumpProgressChanged,
                            DelegateSequenceBlockingMode.Asynchronous,
                            false,
                            new object[] { ccu.IdCCU, (_ccuDebugFileReceivedCounter * 100 / _ccuDebugFilesCount) });

                        //Console.WriteLine("Save debug file: " + Path.GetFileName(transferInfo.DestinationName));
                    }
                    catch (Exception)
                    {
                       //Console.WriteLine("Save debug file error: " + transferInfo.LastErrorMessage + ex.ToString());
                    }
                    finally
                    {
                        _pullDataChannelPeerAutoResetEvent.Set();
                    }
                }

            }
            catch
            {
            }
        }


        void OnCnmpAgentLookupFinished(
            CNMPLookupType lookupType,
            string value,
            CNMPLookupResultList cnmpLookupResult)
        {
            ICollection<Guid> ccusLookupingClients = null;

            lock (_ccusLookupingClients)
            {
                if (_ccusLookupingClients.Count > 0)
                    ccusLookupingClients = new HashSet<Guid>(_ccusLookupingClients);
                _ccusLookupingClients.Clear();
            }

            ICollection<Guid> alarmTransmittersLokupingClients = null;

            lock (_alarmTransmittersLookupingClients)
            {
                if (_alarmTransmittersLookupingClients.Count > 0)
                    alarmTransmittersLokupingClients = new HashSet<Guid>(_alarmTransmittersLookupingClients);
                _alarmTransmittersLookupingClients.Clear();
            }

            if (cnmpLookupResult == null)
                return;

            if (ccusLookupingClients != null &&
                ccusLookupingClients.Count > 0)
            {
                var ccusFromDatabase = CCUs.Singleton.List();

                var ccusIpAddressesInDatabase = ccusFromDatabase != null
                    ? new HashSet<string>(ccusFromDatabase.Select(
                        ccu =>
                            ccu.IPAddress))
                    : new HashSet<string>();

                var lookupedCcus = new LinkedList<LookupedCcu>();

                foreach (var item in cnmpLookupResult)
                {
                    var ipAddress = item.EndPoint.Address.ToString();

                    if (ccusIpAddressesInDatabase.Contains(ipAddress))
                        continue;

                    string mainBoardType = null;
                    string appVersion = null;
                    string ceVersion = null;

                    if (!ParseCmpLookupResultItem(
                        item,
                        MainBoardVariant.Unknown,
                        "ccu",
                        ref mainBoardType,
                        ref appVersion,
                        ref ceVersion))
                        continue;

                    lookupedCcus.AddLast(new LookupedCcu(ipAddress)
                    {
                        CcuVersion = appVersion,
                        CeVersion = ceVersion,
                        MainboardType = mainBoardType
                    });
                }

                CCULookupFinished(lookupedCcus, ccusLookupingClients);
            }

            if (alarmTransmittersLokupingClients != null && alarmTransmittersLokupingClients.Count > 0)
            {
                var alarmTransmittersFromDatabase = AlarmTransmitters.Singleton.List();

                var alarmTransmittersIpAddressesInDatabase = alarmTransmittersFromDatabase != null
                    ? new HashSet<string>(alarmTransmittersFromDatabase.Select(
                        alarmTransmitter =>
                            alarmTransmitter.IpAddress))
                    : new HashSet<string>();

                var lookupedAlarmTransmitters = new LinkedList<LookupedAlarmTransmitter>();

                foreach (var item in cnmpLookupResult)
                {
                    var ipAddress = item.EndPoint.Address.ToString();

                    if (alarmTransmittersIpAddressesInDatabase.Contains(ipAddress))
                        continue;

                    string mainBoardType = null;
                    string appVersion = null;
                    string ceVersion = null;

                    if (!ParseCmpLookupResultItem(
                        item,
                        MainBoardVariant.CAT12CE,
                        "cat12ce",
                        ref mainBoardType,
                        ref appVersion,
                        ref ceVersion))
                        continue;

                    lookupedAlarmTransmitters.AddLast(
                        new LookupedAlarmTransmitter(
                            ipAddress,
                            mainBoardType,
                            appVersion,
                            ceVersion));



                }

                AlarmTransmittersLookupFinished(
                    lookupedAlarmTransmitters,
                    alarmTransmittersLokupingClients);
            }
        }

        private static bool ParseCmpLookupResultItem(
            CNMPLookupResultItem item,
            MainBoardVariant mainBoardVariantRequirement,
            string appMarker,
            ref string mainBoardVariantString,
            ref string appVersion,
            ref string ceVersion)
        {
            var mainBoardVariant = MainBoardVariant.Unknown;



            var mbv0 = item.GetExtra("ce/build_mbt_freq");
            if (!string.IsNullOrEmpty(mbv0))
            {
                var parts = mbv0.Split(StringConstants.SLASH[0]);

                try
                {
                    mainBoardVariant = ((MainBoardVariant)int.Parse(parts[0]));
                    if (mainBoardVariantRequirement != MainBoardVariant.Unknown &&
                        mainBoardVariant != mainBoardVariantRequirement)
                        return false;

                    mainBoardVariantString = mainBoardVariant.ToString();
                }
                catch
                {
                }

                if (parts.Length > 1)
                {
                    ceVersion = parts[1];

                    int ceVersionAsInt = -1;
                    if (int.TryParse(ceVersion, out ceVersionAsInt)
                        && ceVersionAsInt >= 3099)
                    {
                        var appsExtra = item.GetExtra("apps");
                        int startCcuMarkerPos;

                        if (appMarker.Last() != '=')
                            appMarker += '=';

                        if (!string.IsNullOrEmpty(appsExtra) &&
                            (startCcuMarkerPos = appsExtra.IndexOf(appMarker, System.StringComparison.InvariantCultureIgnoreCase)) >= 0)
                        {
                            var endCcuMarkerPos = startCcuMarkerPos + appMarker.Length;
                            var posOfSemicolon = appsExtra.IndexOf(';', startCcuMarkerPos);
                            appVersion = posOfSemicolon >= 0
                                ? appsExtra.Substring(endCcuMarkerPos, posOfSemicolon - endCcuMarkerPos)
                                : appsExtra.Substring(endCcuMarkerPos);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            switch (mainBoardVariant)
            {
                case MainBoardVariant.Unknown:
                case MainBoardVariant.CCR_PLUS:
                    return false;
                default:
                    int tmp;

                    if (int.TryParse(mainBoardVariant.ToString(), out tmp))
                        // means the mainBoardVariant value doesn't have friendly name
                        return false;
                    break;
            }


            return true;
        }

        private static CCU GetCCUFromInput(Guid guidInput, out Input input)
        {
            input = Inputs.Singleton.GetById(guidInput);
            return GetCCUFromInput(input);
        }

        private static CCU GetCCUFromInput(Input input)
        {
            return
                input != null
                    ? (input.CCU ??
                       (input.DCU != null
                           ? input.DCU.CCU
                           : null))
                    : null;
        }

        private void OnCnmpValidRequestReceived(
            IPEndPoint ipEndpoint,
            CNMPLookupType lookupType,
            int keyId,
            string callerInstanceName,
            string callerType,
            IDictionary<string, string> extras)
        {
            if (GeneralOptions.Singleton.DisableCcuPnPAutomaticAssignmnet)
                return;

            SafeThread<string, int?>.StartThread(
                CreateCCU,
                ipEndpoint.Address.ToString(),
                (int?)null);
        }

        public void DoCCUsLookUp(Guid clientID)
        {
            lock (_ccusLookupingClients)
            {
                _ccusLookupingClients.Add(clientID);
            }

            _cnmpAgent.Lookup(CNMPLookupType.Key, CnmpCeLookupKey, _getExtras);
        }

        public void AlarmTransmittersLookup(Guid clientId)
        {
            lock (_alarmTransmittersLookupingClients)
            {
                _alarmTransmittersLookupingClients.Add(clientId);
            }

            _cnmpAgent.Lookup(CNMPLookupType.Key, CnmpCeLookupKey, _getExtras);
        }

        private readonly object _lockCreateCcu = new object();
        private void CreateCCU(
            string ipAddress,
            int? idStructuredSubSite)
        {
            lock (_lockCreateCcu)
            {
                var ccus =
                    CCUs.Singleton.SelectByCriteria(
                        new List<FilterSettings>
                        {
                            new FilterSettings(
                                CCU.COLUMNIPADDRESS,
                                ipAddress,
                                ComparerModes.EQUALL)
                        });

                if (ccus != null && ccus.Count > 0)
                    return;

                var newCCU = new CCU
                {
                    IPAddress = ipAddress,
                    Name = "CCU " + ipAddress,
                    IndexCCU = CCUs.Singleton.GetNewIndex()
                };

                if (CCUs.Singleton.Insert(
                        ref newCCU,
                        idStructuredSubSite))
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCreatedCCUEvent,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false);
                }
            }
        }

        public void CreateCCUs(
            IEnumerable<string> ipAddresses,
            int? idStructuredSubSite)
        {
            foreach (var item in ipAddresses)
                CreateCCU(
                    item,
                    idStructuredSubSite);
        }

        public void ConnectCCUs()
        {
            var listCCU = CCUs.Singleton.List();

            if (listCCU == null)
                return;

            foreach (var ccu in listCCU)
                ConnectCCU(ccu.IdCCU, true);
        }

        public void ConnectCCU(Guid guidCCU, bool forcedChangeAlarm)
        {
            lock (_remotingCCUs)
            {
                var ccu = CCUs.Singleton.GetById(guidCCU);
                if (ccu == null)
                {
                    var ccuSetting = GetCCUSettings(guidCCU);
                    if (ccuSetting == null)
                        return;

                    Unconfigure(guidCCU);

                    ccuSetting.Disconnect();
                    _remotingCCUs.Remove(guidCCU);

                    //remove alarm
                    CcuOfflineServerAlarm.RemoveAlarm(guidCCU);

                    return;
                }

                if (ccu.IPAddress == null)
                {
                    CCUSettings ccuSetting = GetCCUSettings(guidCCU);

                    if (ccuSetting != null)
                    {
                        ccuSetting.Disconnect();
                        _remotingCCUs.Remove(guidCCU);
                    }

                    return;
                }

                CCUSettings actCCSetting;

                if (!_remotingCCUs.TryGetValue(ccu.IdCCU, out actCCSetting))
                    _remotingCCUs.Add(ccu.IdCCU, new CCUSettings(ccu));
                else
                    if (actCCSetting != null)
                {
                    actCCSetting.ChangeIpAddress(ccu.IPAddress);
                    actCCSetting.ChangeSettingsForAlarmCCUOffline(ccu, forcedChangeAlarm);
                    actCCSetting.SetCCUName(ccu.Name);
                }
            }
        }

        private CCUSettings GetCCUSettings(Guid guidCCU)
        {
            CCUSettings ccuSetting;

            _remotingCCUs.TryGetValue(guidCCU, out ccuSetting);

            return ccuSetting;
        }

        public CCUOnlineState GetCCUState(Guid guidCCU)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);

            return
                ccuSetting != null
                    ? ccuSetting.State
                    : CCUOnlineState.Unknown;
        }

        public bool IsCCU0FromCCU(Guid guidCCU)
        {
            if (!_remotingCCUs.ContainsKey(guidCCU))
                return false;

            object retValue = false;

            try
            {
                retValue = SendToRemotingCCUs(guidCCU, true, "IsCCU0");
            }
            catch
            {
            }

            return retValue is bool && (bool)retValue;
        }

        public bool AcknowledgeAllowed(Guid guidCCU)
        {
            if (!_remotingCCUs.ContainsKey(guidCCU))
                return false;

            object retValue = false;

            try
            {
                retValue = SendToRemotingCCUs(guidCCU, true, "AcknowledgeAllowed");
            }
            catch
            {
            }

            return retValue is bool && (bool)retValue;
        }

        public bool? IsCCUUpgraderFromCCU(Guid guidCCU)
        {
            object retValue = SendToRemotingCCUs(guidCCU, true, "IsCCUUpgrader");

            if (retValue is bool)
            {
                return (bool)retValue;
            }

            return null;
        }

        public void CreateAlarmsCCUOffline()
        {
            var ccus =
                CCUs.Singleton.List()
                    .Where(ccu => ccu != null)
                    .ToList();

            foreach (var ccu in ccus)
            {
                CCUSettings ccuSettings = GetCCUSettings(ccu.IdCCU);

                if (ccuSettings != null)
                    ccuSettings.UpdateAlarmCcuOffline(
                        ccu,
                        ccuSettings.State == CCUOnlineState.Online);
            }
        }

        public void CreateAlarmsCCUUnconfigured()
        {
            var ccus =
                CCUs.Singleton.List()
                    .Where(ccu => ccu != null)
                    .ToList();

            foreach (var ccu in ccus)
            {
                CCUSettings ccuSettings = GetCCUSettings(ccu.IdCCU);

                if (ccuSettings != null)
                    ccuSettings.UpdateAlarmCcuUnconfigured();
            }
        }

        private static bool IsInputOfCCU(Input input, Guid guidCCU)
        {
            CCU ccu = GetCCUFromInput(input);

            return
                ccu != null &&
                ccu.IdCCU == guidCCU;
        }

        public void SendAllStatesToServer(Guid guidCCU)
        {
            try
            {
                SendToRemotingCCUs(guidCCU, "SendAllStatesToServer");
            }
            catch
            { }
        }

        private void BindEventsInternal(
            CCUSettings ccuSetting,
            string eventName,
            DObjects2Void method)
        {
            if (!ccuSetting.Client.BindEvent(
                    eventName,
                    method,
                    this))
                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPEBINDINGEVENTFAILED,
                    _thisAssemblyName,
                    null,
                    string.Format(
                        "Binding event {0} on the ccu failed",
                        eventName),
                    new EventlogParameter(
                        EventlogParameter.TYPECCU,
                        ccuSetting.IPAddressString));
        }

        private void BindEventsInternal(
            Guid guidCCU,
            string eventName,
            DObjects2Void method)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);
            if (ccuSetting == null)
                return;

            try
            {
                BindEventsInternal(
                    ccuSetting,
                    eventName,
                    method);
            }
            catch
            { }
        }

        public void BindEvents(Guid guidCCU)
        {
            BindEventsInternal(
                guidCCU,
                "SaveEvent",
                SaveEvent);

            BindEventsInternal(
                guidCCU,
                "SaveAlarmEvent",
                SaveAlarmEvent);
        }

        public void BindEventsAllCcu(Guid guidCCU)
        {
            BindEventsInternal(
                guidCCU,
                "CcuConfiguredStateChanged",
                CcuConfiguredStateChanged);
        }

        public void BindEventsCCUUpgrader(Guid guidCCU)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);
            if (ccuSetting == null)
                return;

            try
            {
                BindEventsInternal(
                    ccuSetting,
                    "CcuUpgradeFileUnpackProgress",
                    CcuUpgradeFileUnpackProgress);

                BindEventsInternal(
                    ccuSetting,
                    "CcuUpgradeFinished",
                    CcuUpgradeFinished);

                SendToRemotingCCUs(guidCCU, true, "BindCompleted");
            }
            catch
            { }
        }

        public void SetCrStateUpgrading(Guid idCcu, Guid idCardReader, bool isUpgrading)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(
                    idCcu,
                    idCardReader);

            if (cardReaderState == null)
                return;

            var oldOnlineState = cardReaderState.OnlineState;

            cardReaderState.SetStateUpgrading(isUpgrading);

            if (isUpgrading)
            {
                CreateCardReaderOnlineStateChangedEventlog(
                    CardReaders.Singleton.GetById(idCardReader),
                    DateTime.Now,
                    OnlineState.Upgrading);
            }

            var newOnlineState = cardReaderState.OnlineState;

            if (oldOnlineState == newOnlineState)
                return;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunCardReaderChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    idCardReader,
                    (byte) newOnlineState,
                    idCcu
                });
        }

        private void SetCROnlineState(Guid guidCCU, Guid guidCR, OnlineState onlineState)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(
                    guidCCU,
                    guidCR);

            if (cardReaderState == null)
                return;

            var oldOnlineState = cardReaderState.OnlineState;

            cardReaderState.SetOnlineState(
                null,
                onlineState);

            CreateCardReaderOnlineStateChangedEventlog(
                CardReaders.Singleton.GetById(guidCR),
                DateTime.Now,
                onlineState);

            var newOnlineState = cardReaderState.OnlineState;

            if (oldOnlineState == newOnlineState)
                return;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunCardReaderChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    guidCR,
                    (byte) newOnlineState,
                    guidCCU
                });
        }

        public static void CreateCardReaderOnlineStateChangedEventlog(
            CardReader cardReader,
            DateTime? dateTime,
            OnlineState onlineState)
        {
            if (!GeneralOptions.Singleton.EventlogCardReaderOnlineStateChanged || cardReader == null)
                return;

            var eventSources = GetCardReaderEventSources(cardReader);

            string description = string.Empty;
            switch (onlineState)
            {
                case OnlineState.Unknown:
                    description = string.Format("Card reader {0} changed its online state to unknown", cardReader);
                    break;
                case OnlineState.Online:
                    description = string.Format("Card reader {0} changed its online state to online", cardReader);
                    break;
                case OnlineState.Offline:
                    description = string.Format("Card reader {0} changed its online state to offline", cardReader);
                    break;
                case OnlineState.Upgrading:
                    description = string.Format("Card reader {0} changed its online state to upgrading", cardReader);
                    break;
                case OnlineState.WaitingForUpgrade:
                    description = string.Format("Card reader {0} changed its online state to waiting for upgrade",
                        cardReader);
                    break;
                case OnlineState.AutoUpgrading:
                    description = string.Format("Card reader {0} changed its online state to auto upgrading", cardReader);
                    break;
                case OnlineState.Reseting:
                    description = string.Format("Card reader {0} changed its online state to reseting", cardReader);
                    break;
            }

            Eventlogs.Singleton.InsertEvent(Eventlog.TYPE_CARD_READER_ONLINE_STATE_CHANGED,
                dateTime == null ? DateTime.Now : dateTime.Value, typeof(CCUConfigurationHandler).Assembly.GetName().Name,
                eventSources.ToArray(), description,
                new EventlogParameter(EventlogParameter.TYPE_ONLINE_STATE, onlineState.ToString()));
        }

        private static List<Guid> GetCardReaderEventSources(CardReader cardReader)
        {
            if (cardReader == null)
            {
                return null;
            }

            var eventSources = new List<Guid> { cardReader.IdCardReader };

            if (cardReader.DCU != null)
            {
                eventSources.Add(cardReader.DCU.IdDCU);

                if (cardReader.DCU.CCU != null)
                {
                    eventSources.Add(cardReader.DCU.CCU.IdCCU);
                }
            }
            else if (cardReader.CCU != null)
            {
                eventSources.Add(cardReader.CCU.IdCCU);
            }

            return eventSources;
        }

        private CardReader CreateCardReader(
            DateTime? dateTime,
            bool createOnlyInDatabase,
            Guid guidParentObjectCCU,
            int dcuLogicalAddress,
            CardReaderCreationParams cardReaderCreationParams,
            out bool wasChanged)
        {
            wasChanged = true;

            var hwVersion =
                !string.IsNullOrEmpty(cardReaderCreationParams.hardwareVersion)
                    ? (CRHWVersion)
                        Enum.Parse(
                            typeof(CRHWVersion),
                            cardReaderCreationParams.hardwareVersion)
                    : CRHWVersion.Unknown;

            var ccu = CCUs.Singleton.GetById(guidParentObjectCCU);

            if (ccu == null)
                return null;

            DCU dcu = null;

            if (dcuLogicalAddress != -1)
            {
                dcu = DCUs.Singleton.GetByLogicalAddress(
                    guidParentObjectCCU,
                    (byte)dcuLogicalAddress)
                      ?? CreateNewDcu(
                          (byte)dcuLogicalAddress,
                          ccu);
            }

            CardReader cr =
                TryGetAndUpdateExistingCardReader(
                    dateTime,
                    guidParentObjectCCU,
                    cardReaderCreationParams,
                    ref wasChanged,
                    ccu,
                    dcu,
                    hwVersion);

            if (cr != null)
                return cr;

            if (!cardReaderCreationParams.onlineState)
                return null;

            var newCardReader =
                new CardReader
                {
                    Address = cardReaderCreationParams.address,
                    Port = cardReaderCreationParams.port,
                    GIN = string.Empty,
                    CardReaderHardware = (byte)hwVersion,
                    SecurityLevel = Contal.Drivers.CardReader.CRConstants.GetHasKeyboard(hwVersion)
                        ? (byte)SecurityLevel.CARDPIN
                        : (byte)SecurityLevel.CARD,
                    CCU = dcu == null
                        ? ccu
                        : null,
                    DCU = dcu
                };

            newCardReader.Name =
                CardReaders.GetNewCardReaderName(
                    cardReaderCreationParams.address,
                    newCardReader.EnableParentInFullName,
                    newCardReader.CCU,
                    newCardReader.DCU);

            bool success =
                createOnlyInDatabase
                    ? CardReaders.Singleton.InsertOnlyInDatabase(ref newCardReader)
                    : CardReaders.Singleton.Insert(ref newCardReader);

            if (!success)
                return null;

            AddOrUpdateCardReaderState(
                dateTime,
                guidParentObjectCCU,
                cardReaderCreationParams,
                false,
                ref wasChanged,
                newCardReader);

            return newCardReader;
        }

        private CardReader TryGetAndUpdateExistingCardReader(
            DateTime? dateTime,
            Guid guidParentObjectCCU,
            CardReaderCreationParams cardReaderCreationParams,
            ref bool wasChanged,
            CCU ccu,
            DCU dcu,
            CRHWVersion hwVersion)
        {
            var cardReaders =
                CardReaders.Singleton.SelectByCriteria(
                    new List<FilterSettings>
                    {
                        new FilterSettings(
                            CardReader.COLUMNADDRESS,
                            cardReaderCreationParams.address,
                            ComparerModes.EQUALL),
                        dcu == null
                            ? new FilterSettings(
                                CardReader.COLUMNCCU,
                                ccu,
                                ComparerModes.EQUALL)
                            : new FilterSettings(
                                CardReader.COLUMNDCU,
                                dcu,
                                ComparerModes.EQUALL)
                    });

            if (cardReaders == null || cardReaders.Count <= 0)
                return null;

            var newCardReader = cardReaders.ElementAt(0);

            if (!cardReaderCreationParams.onlineState)
                hwVersion = (CRHWVersion)newCardReader.CardReaderHardware;
            else
                if (!Contal.Drivers.CardReader.CRConstants.GetHasKeyboard(hwVersion))
                DownGradeSecurityLevel(newCardReader);

            if (!AddOrUpdateCardReaderState(
                    dateTime,
                    guidParentObjectCCU,
                    cardReaderCreationParams,
                    true,
                    ref wasChanged,
                    newCardReader))
                return newCardReader;

            if (!cardReaderCreationParams.onlineState)
                return newCardReader;

            newCardReader = CardReaders.Singleton.GetObjectForEdit(newCardReader.IdCardReader);

            if (newCardReader != null &&
                newCardReader.CardReaderHardware != (byte)hwVersion)
            {
                newCardReader.CardReaderHardware = (byte)hwVersion;
                CardReaders.Singleton.UpdateOnlyInDatabase(newCardReader);
            }

            return newCardReader;
        }

        private bool AddOrUpdateCardReaderState(
            DateTime? dateTime,
            Guid guidParentObjectCCU,
            CardReaderCreationParams cardReaderCreationParams,
            bool updateOnlineState,
            ref bool wasChanged,
            CardReader newCardReader)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidParentObjectCCU);

            if (ccuSetting == null || ccuSetting.CardReaderStates == null)
                return false;

            lock (ccuSetting.CardReaderStates)
            {
                CardReaderState cardReaderState;

                if (ccuSetting.CardReaderStates.TryGetValue(
                    newCardReader.IdCardReader,
                    out cardReaderState))
                {
                    if (cardReaderState == null)
                        return true;

                    if (updateOnlineState)
                        wasChanged =
                            cardReaderState.SetOnlineState(
                                dateTime,
                                cardReaderCreationParams.onlineState
                                    ? OnlineState.Online
                                    : OnlineState.Offline);
                }
                else
                {
                    cardReaderState =
                        new CardReaderState(
                            newCardReader,
                            dateTime,
                            cardReaderCreationParams.onlineState);

                    ccuSetting.CardReaderStates.Add(
                        newCardReader.IdCardReader,
                        cardReaderState);
                }

                cardReaderState.SetCardReaderInformation(
                    cardReaderCreationParams.protocolVersion,
                    cardReaderCreationParams.firmwareVersion,
                    cardReaderCreationParams.hardwareVersion,
                    cardReaderCreationParams.protocolMajor.ToString(CultureInfo.InvariantCulture));
            }

            return true;
        }

        private static void DownGradeSecurityLevel(CardReader cardReader)
        {
            try
            {
                var crSecurityLevel = (SecurityLevel)cardReader.SecurityLevel;

                switch (crSecurityLevel)
                {
                    case SecurityLevel.CODEORCARDPIN:
                    case SecurityLevel.CODEORCARD:
                    case SecurityLevel.CODE:
                    case SecurityLevel.CARDPIN:
                        {
                            var cr = CardReaders.Singleton.GetObjectForEdit(cardReader.IdCardReader);
                            cr.SecurityLevel = (byte)SecurityLevel.CARD;
                            CardReaders.Singleton.Update(cr);
                            CardReaders.Singleton.EditEnd(cr);
                        }
                        break;
                }
            }
            catch { }
        }

        public void InitInputs(Guid guidCCU)
        {
            try
            {
                CCUs.Singleton.CreateOwnInputs(
                    guidCCU,
                    SendToRemotingCCUs(guidCCU, "GetAccessibleOutputsInputs") as int[],
                    "Input");

                CCUSettings ccuSetting = GetCCUSettings(guidCCU);
                if (ccuSetting == null)
                    return;

                lock (ccuSetting.InputStates)
                    ccuSetting.InputStates.Clear();
            }
            catch
            { }
        }

        public void InitOutputs(Guid guidCCU)
        {
            try
            {
                CCUs.Singleton.CreateOwnOutputs(
                    guidCCU,
                    SendToRemotingCCUs(guidCCU, "GetAccessibleOutputsOutputs") as int[],
                    "Output");

                CCUSettings ccuSetting = GetCCUSettings(guidCCU);
                if (ccuSetting == null)
                    return;

                lock (ccuSetting.OutputStates)
                    ccuSetting.OutputStates.Clear();
            }
            catch
            { }
        }

        public void InitDoorEnvironments(Guid guidCCU, MainBoardVariant mainBoardType)
        {
            try
            {
                var retValue = SendToRemotingCCUs(guidCCU, "GetDoorEnvironmentsCount");

                if (!(retValue is int))
                    return;

                var doorEnvironmentsCount = (int)retValue;
                if (doorEnvironmentsCount <= 0)
                    return;

                CCUs.Singleton.CreateDoorEnvironments(
                    guidCCU,
                    doorEnvironmentsCount,
                    CgpServer.Singleton.LocalizationHelper.GetString("DoorEnvironmentName"),
                    mainBoardType);

                CCUSettings ccuSetting = GetCCUSettings(guidCCU);
                if (ccuSetting == null)
                    return;

                lock (ccuSetting.DoorEnvironmentStates)
                    ccuSetting.DoorEnvironmentStates.Clear();
            }
            catch
            { }
        }

        public void InitMainBoardType(Guid guidCCU)
        {
            try
            {
                var retValue = SendToRemotingCCUs(guidCCU, true, "GetMainBoardType");
                if (retValue == null)
                    return;

                CCUSettings ccuSetting = GetCCUSettings(guidCCU);
                if (ccuSetting != null)
                    ccuSetting.MainBoardType = (MainBoardVariant)(byte)retValue;

                var ccu = CCUs.Singleton.GetObjectForEdit(guidCCU);

                if (ccu != null)
                {
                    if (ccu.CcuMainboardType != (byte)retValue)
                    {
                        ccu.CcuMainboardType = (byte)retValue;
                        CCUs.Singleton.Update(ccu);
                    }

                    CCUs.Singleton.EditEnd(ccu);
                }
            }
            catch
            { }
        }

        internal void InitCcuFirmwareVersion(CCUSettings ccuSetting, ref CCUConfigurationState configurationState)
        {
            try
            {
                var retValue = SendToRemotingCCUs(ccuSetting.GuidCCU, true, "GetCcuFirmwareVersion");
                if (retValue == null)
                    return;

                var ceVersion = SendToRemotingCCUs(ccuSetting.GuidCCU, true, "WinCEImageVersion");
                if (ceVersion == null)
                    return;

                ccuSetting.FirmwareVersion = (string)retValue;
                ccuSetting.WinCeVersion = (string)ceVersion;

                if (configurationState == CCUConfigurationState.ConfiguredForThisServer)
                {
                    if (!(IsFirmwareSupported(ccuSetting.FirmwareVersion)
                          && IsWinCeSupported(ccuSetting.FirmwareVersion, ccuSetting.WinCeVersion)))
                    {
                        configurationState = CCUConfigurationState.ConfiguredForThisServerUpgradeOnly;
                    }
                }

                Eventlogs.Singleton.InsertEvent(Eventlog.TYPE_CCU_CE_VERSIONS_WERE_LOADED, DateTime.Now, GetType().Assembly.GetName().Name,
                        new[] { ccuSetting.GuidCCU },
                        "The CCU/CE versions have been loaded.",
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            GetCcuFirmwareVersion(ccuSetting.GuidCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            GetCcuCeVersion(ccuSetting.GuidCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));
            }
            catch
            { }
        }

        internal bool InitCcuLicence(
            CCUSettings ccuSetting,
            CCU ccu,
            CCUConfigurationState configurationState)
        {
            InitIsCat12Combo(ccuSetting);

            if (!ccuSetting.IsCat12Combo)
            {
                if (ComboLicenceManager.Singleton.HasLicence(ccuSetting.GuidCCU))
                {
                    ComboLicenceManager.Singleton.ReleaseLicence(ccuSetting.GuidCCU);
                }

                return false;
            }

            if (configurationState == CCUConfigurationState.ConfiguredForThisServer ||
                configurationState == CCUConfigurationState.ConfiguredForThisServerUpgradeOnly ||
                configurationState == CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe)
            {
                bool unconfigure = false;

                // Check licence for COMBO
                if (ComboLicenceManager.Singleton.HasLicence(ccuSetting.GuidCCU))
                {
                    if (!HasCat12ComboLicence(ccuSetting.GuidCCU))
                    {
                        SetCat12ComboLicence(ccuSetting.GuidCCU, true);
                    }
                }
                else
                {
                    // This CCU does not have licence so if there is free assigne it to this ccu
                    if (!ComboLicenceManager.Singleton.AllocateLicence(ccuSetting.GuidCCU))
                    {
                        SetCat12ComboLicence(ccuSetting.GuidCCU, false);
                        unconfigure = true;

                        if (ccu != null)
                            CcuUnconfiguredServerAlarm.AddInsufficientCcuComboLicenceCountAlarm(ccu);
                    }
                    else
                    {
                        SetCat12ComboLicence(ccuSetting.GuidCCU, true);
                    }
                }

                return unconfigure;
            }

            // Unconfigured CCU can release licence
            if (configurationState == CCUConfigurationState.Unconfigured)
            {
                if (ComboLicenceManager.Singleton.HasLicence(ccuSetting.GuidCCU))
                {
                    ComboLicenceManager.Singleton.ReleaseLicence(ccuSetting.GuidCCU);
                }
            }

            return false;
        }

        private void InitIsCat12Combo(CCUSettings ccuSetting)
        {
            if (ccuSetting == null)
                return;

            try
            {
                var retValue = SendToRemotingCCUs(ccuSetting.GuidCCU, true, "IsCat12Combo");
                if (retValue == null)
                    return;

                ccuSetting.IsCat12Combo = (bool)retValue;
            }
            catch
            { }
        }

        /// <summary>
        /// Gets information about ccu version support on server
        /// </summary>
        /// <param name="ccuFirmwareVersion"></param>
        /// <returns></returns>
        private static bool IsFirmwareSupported(string ccuFirmwareVersion)
        {
            if (string.IsNullOrEmpty(ccuFirmwareVersion))
                return false;

#if DEBUG
            return true;
#endif

            try
            {
                var thisVersion = new Version(ccuFirmwareVersion.Split(' ')[0]);
                var minVersion = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CCU;
                var maxversion = NCASServer.Singleton.MaximalFirmwareVersionForCCU;

                return thisVersion >= minVersion && thisVersion <= maxversion;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                return false;
            }
        }

        /// <summary>
        /// Gets information about windows ce support on server
        /// </summary>
        /// <param name="ccuFirmwareVersion"></param>
        /// <param name="winCeVersion"></param>
        /// <returns></returns>
        private static bool IsWinCeSupported(string ccuFirmwareVersion, string winCeVersion)
        {
            int ceVersion;
            if (string.IsNullOrEmpty(ccuFirmwareVersion)
                || string.IsNullOrEmpty(winCeVersion)
                || !int.TryParse(winCeVersion, out ceVersion))
                return false;

            try
            {
                var thisVersion = new Version(ccuFirmwareVersion.Split(' ')[0]);

                if (thisVersion >= NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CE_CHECKING)
                {
                    if (ceVersion < NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CE)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                return false;
            }
        }

        public bool IsSupportedTCPDataChannel(Guid ccuId)
        {
            if (ccuId == Guid.Empty)
                return false;

            var thisVersionString = GetCcuFirmwareVersion(ccuId);

            if (string.IsNullOrEmpty(thisVersionString))
                return false;

            var thisVersion = new Version(thisVersionString.Split(' ')[0]);

            var requiredVersion =
                NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_TCP_DATA_CHANNEL;

            return thisVersion >= requiredVersion;
        }

        public bool IsSupportedWaitingWhileIncommingStreamIsInProcessing(Guid ccuId)
        {
            if (ccuId == Guid.Empty)
                return false;

            var thisVersionString = GetCcuFirmwareVersion(ccuId);

            if (string.IsNullOrEmpty(thisVersionString))
                return false;

            var thisVersion = new Version(thisVersionString.Split(' ')[0]);

            var requiredVersion =
                NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_WAITING_WHILE_INCOMING_STREAM_IS_IN_PROCESSING;

            return thisVersion >= requiredVersion;
        }

        public bool? DoorEnvironmentAccessGranted(DoorEnvironment doorEnvironment)
        {
            try
            {
                if (doorEnvironment == null)
                    return false;

                if (!DoorEnvironments.Singleton.HasAccessToAccessGranted(doorEnvironment.IdDoorEnvironment))
                    return false;

                CCU ccu = null;
                if (doorEnvironment.CCU != null)
                    ccu = doorEnvironment.CCU;
                else
                    if (doorEnvironment.DCU != null)
                    ccu = doorEnvironment.DCU.CCU;

                if (ccu == null)
                    return false;

                return SendToRemotingCCUs(
                    ccu.IdCCU,
                    "DoorEnvironmentAccessGranted",
                    doorEnvironment.IdDoorEnvironment)
                    as bool?;
            }
            catch
            {
                return null;
            }
        }

        public void ApplyChanges(Guid guidCCU)
        {
            try
            {
                SendToRemotingCCUs(guidCCU, "ApplyChanges");
            }
            catch
            { }
        }

        public CCUConfigurationState CCUConfigured(Guid guidCCU)
        {
            var retValue = SendToRemotingCCUs(guidCCU, true, "GetIsConfigured");

            return retValue is byte
                ? (CCUConfigurationState)retValue
                : CCUConfigurationState.Unknown;
        }

        public string GetServerHashCode(Guid ccuGuid)
        {
            if (ccuGuid == Guid.Empty)
                return string.Empty;

            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            return
                ccuSettings != null
                    ? ccuSettings.ServerHashCode
                    : string.Empty;
        }

        public object SendToRemotingCCUs(Guid guidCCU, string methodName, params object[] objs)
        {
            return SendToRemotingCCUs(guidCCU, false, methodName, objs);
        }

        public object SendToRemotingCCUs(
            Guid guidCCU,
            bool runOnCCUConfiguredForAnotherServer,
            string methodName,
            params object[] objs)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);

            if (ccuSetting == null || ccuSetting.State != CCUOnlineState.Online)
                return null;

            if (!runOnCCUConfiguredForAnotherServer
                && ccuSetting.ConfigurationState != CCUConfigurationState.ConfiguredForThisServer
                && ccuSetting.ConfigurationState != CCUConfigurationState.ForceReconfiguration
                && ccuSetting.ConfigurationState != CCUConfigurationState.ConfiguredForThisServerUpgradeOnly
                && ccuSetting.ConfigurationState != CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe)
                return null;

            try
            {
                var parameters =
                    Enumerable
                        .Repeat<object>(ccuSetting.ServerHashCode, 1)
                        .Concat(objs ?? Enumerable.Empty<object>())
                        .ToArray();

                return ccuSetting.Call(methodName, parameters);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);

                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPERUNMETHODFAILED,
                    _thisAssemblyName,
                    new[] { guidCCU },
                    string.Format(
                        "Run method {0} on the ccu failed with exception: {1}",
                        methodName,
                        e),
                    new EventlogParameter(
                        EventlogParameter.TYPECCU,
                        ccuSetting.IPAddressString),
                    new EventlogParameter(EventlogParameter.TYPECCU,
                        ccuSetting.IPAddressString),
                    new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                        GetCcuFirmwareVersion(guidCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                        GetCcuCeVersion(guidCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                        CgpServer.Singleton.Version));

                return null;
            }
        }

        public object CheckAlarmAreaActivationRights(Guid guidCCU, Guid guidPerson, Guid guidAlarmArea)
        {
            return SendToRemotingCCUs(guidCCU, "ResultCheckAlarmAreaRights", guidPerson, guidAlarmArea);
        }

        public void SendNTPTimeDifferenceTolerance(Guid guidCCU, int interval)
        {
            SendToRemotingCCUs(
                guidCCU,
                "SetNTPTimeDifferenceTolerance",
                interval);
        }

        private void SaveEvent(object[] inputParts)
        {
            if (inputParts == null || inputParts.Length != 2)
                return;

            var ccuIpAddress = inputParts[1] as string;
            if (ccuIpAddress == null)
                return;

            var actIdCcu = GetIdCCUFromIpAddress(ccuIpAddress);
            if (actIdCcu == Guid.Empty)
                return;

            CCUSettings ccuSetting = GetCCUSettings(actIdCcu);
            if (ccuSetting == null)
                return;

            try
            {
                ccuSetting.CCUEvents.EnqueueEvent(
                    inputParts[0] as ICollection<EventParameters.EventParameters>);
            }
            catch { }
        }

        public void SetOldEventsSettings(Guid guidCCU)
        {
            var settings = new[]
            {
                GeneralOptions.Singleton.EventlogInputStateChanged,
                GeneralOptions.Singleton.EventlogOutputStateChanged,
                GeneralOptions.Singleton.EventlogAlarmAreaAlarmStateChanged,
                GeneralOptions.Singleton.EventlogAlarmAreaActivationStateChanged,
                GeneralOptions.Singleton.EventlogCardReaderOnlineStateChanged
            };

            SendToRemotingCCUs(guidCCU, "SetOldEventsSettings", settings);
        }

        public void GeneralOptionsEventlogsChanged()
        {
            lock (_remotingCCUs)
                foreach (var ccuSettings in _remotingCCUs.Values)
                    if (ccuSettings != null)
                        ccuSettings.GeneralOptionsEventlogsChanged();
        }

        public void ForEachCCUSettings(Action<CCUSettings> action)
        {
            lock (_remotingCCUs)
                foreach (var ccuSetting in CCUSettings)
                    if (ccuSetting != null)
                        action(ccuSetting);
        }

        public static CCU GetCCUFromCR(CardReader cardReader)
        {
            return
                cardReader.DCU != null
                    ? cardReader.DCU.CCU
                    : cardReader.CCU;
        }

        public static Guid GetCCUGuidFromCR(CardReader cardReader)
        {
            CCU ccu = GetCCUFromCR(cardReader);

            return
                ccu != null
                    ? ccu.IdCCU
                    : Guid.Empty;
        }

        public void AlarmsAreaReportingToCRChanged(bool isOn)
        {
            foreach (var ccu in _remotingCCUs)
                if (ccu.Value.ConfigurationState == CCUConfigurationState.ConfiguredForThisServer)
                    SendToRemotingCCUs(
                        ccu.Key,
                        "AAToCRsReportingChanged",
                        isOn);
        }

        public struct CardReaderCreationParams
        {
            public string port;
            public byte address;
            public bool onlineState;
            public string protocolVersion;
            public string firmwareVersion;
            public string hardwareVersion;
            public byte protocolMajor;
        }

        public CardReader CardReaderOnlineStateChanged(
            DateTime? dateTime,
            int dcuLogicalAddress,
            CardReaderCreationParams cardReaderCreationParams,
            string ccuIpAddress)
        {
            try
            {
                var idCcu = GetIdCCUFromIpAddress(ccuIpAddress);

                bool wasChanged;

                var cr = CreateCardReader(
                    dateTime,
                    false,
                    idCcu,
                    dcuLogicalAddress,
                    cardReaderCreationParams,
                    out wasChanged);

                if (!wasChanged || cr == null)
                    return cr;

                var onlineState = GetCardReaderOnlineState(cr.IdCardReader);

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCardReaderChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        cr.IdCardReader,
                        onlineState,
                        cr.DCU == null
                            ? idCcu
                            : cr.DCU.IdDCU
                    });

                return cr;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
        }

        public static OnlineState ConvertToOnlineStateFromState(State state)
        {
            var onlineState = OnlineState.Unknown;

            switch (state)
            {
                case State.Online:
                    onlineState = OnlineState.Online;
                    break;

                case State.Offline:
                    onlineState = OnlineState.Offline;
                    break;

                case State.AutoUpgrading:
                    onlineState = OnlineState.AutoUpgrading;
                    break;

                case State.Reseting:
                    onlineState = OnlineState.Reseting;
                    break;

                case State.Upgrading:
                    onlineState = OnlineState.Upgrading;
                    break;

                case State.WaitingForUpgrade:
                    onlineState = OnlineState.WaitingForUpgrade;
                    break;
            }

            return onlineState;
        }

        public void CardReaderLastCardChanged(
            Guid idCardReader,
            DateTime? dateTime,
            string lastCard,
            string ccuIpAddress)
        {
            try
            {
                var idCCU = GetIdCCUFromIpAddress(ccuIpAddress);
                if (idCCU == Guid.Empty)
                    return;

                CardReaderState cardReaderState =
                    GetCardReaderState(idCCU, idCardReader);

                if (cardReaderState != null)
                    cardReaderState.SetLastCard(dateTime, lastCard);
            }
            catch
            {
            }
        }

        private CardReaderState GetCardReaderState(
            Guid guidCCU,
            Guid guidCardReader,
            out CCUSettings ccuSettings)
        {
            ccuSettings = GetCCUSettings(guidCCU);

            if (ccuSettings == null || ccuSettings.CardReaderStates == null)
                return null;

            CardReaderState cardReaderState;

            ccuSettings.CardReaderStates.TryGetValue(
                guidCardReader,
                out cardReaderState);

            return cardReaderState;
        }

        private CardReaderState GetCardReaderState(
            Guid guidCCU,
            Guid guidCardReader)
        {
            CCUSettings ccuSettings;

            return
                GetCardReaderState(
                    guidCCU,
                    guidCardReader,
                    out ccuSettings);
        }

        private CardReaderState GetCardReaderState(
            CardReader cardReader,
            out CCU ccu,
            out CCUSettings ccuSetting)
        {
            ccuSetting = null;
            ccu = null;

            if (cardReader == null)
                return null;

            ccu = GetCCUFromCR(cardReader);

            return ccu != null
                ? GetCardReaderState(
                    ccu.IdCCU,
                    cardReader.IdCardReader,
                    out ccuSetting)
                : null;
        }

        public CardReaderState GetCardReaderState(CardReader cardReader)
        {
            CCU ccu;
            CCUSettings ccuSettings;

            return
                GetCardReaderState(
                    cardReader,
                    out ccu,
                    out ccuSettings);
        }

        public CardReaderState GetCardReaderState(Guid guidCardReader)
        {
            CardReader cardReader;

            try
            {
                cardReader = CardReaders.Singleton.GetById(guidCardReader);
            }
            catch
            {
                return null;
            }

            CCU ccu;
            CCUSettings ccuSettings;

            return
                GetCardReaderState(
                    cardReader,
                    out ccu,
                    out ccuSettings);
        }

        public OnlineState GetCardReaderOnlineState(Guid guidCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(guidCardReader);

            return
                cardReaderState != null
                    ? cardReaderState.OnlineState
                    : OnlineState.Offline;
        }

        public string CardReaderGetLastCard(Guid guidCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(guidCardReader);

            return
                cardReaderState != null
                    ? cardReaderState.LastCard
                    : string.Empty;
        }

        public string CardReaderProtocolVersion(Guid guidCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(guidCardReader);

            return
                cardReaderState != null
                    ? cardReaderState.ProtocolVersion
                    : string.Empty;
        }

        public string CardReaderProtocolMajor(Guid guidCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(guidCardReader);

            return
                cardReaderState != null
                    ? cardReaderState.ProtocolMajor
                    : string.Empty;
        }

        public string CardReaderFirmwareVersion(Guid guidCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(guidCardReader);

            return
                cardReaderState != null
                    ? cardReaderState.FirmwareVersion
                    : string.Empty;
        }

        public string CardReaderHardwareVersion(Guid guidCardReader)
        {
            try
            {
                var cardReader =
                    CardReaders.Singleton.GetById(guidCardReader);

                return
                    cardReader != null
                        ? ((CRHWVersion)cardReader.CardReaderHardware).ToString()
                        : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void CardReaderCommandChanged(Guid idCardReader, DateTime? dateTime, byte crCommand)
        {
            try
            {
                var cardReaderState = GetCardReaderState(idCardReader);

                if (cardReaderState == null)
                    return;

                if (!cardReaderState.SetCardReaderCommand(
                        dateTime,
                        (CardReaderSceneType)crCommand))
                    return;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCardReaderCommandChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { idCardReader, crCommand });
            }
            catch
            {
            }
        }

        public void CardReaderBlockedStateChanged(
            Guid idCardReader,
            DateTime blockedStateChangedDateTime,
            bool isBlocked)
        {
            try
            {
                var cardReaderState = GetCardReaderState(idCardReader);

                if (cardReaderState == null)
                    return;

                if (!cardReaderState.SetBlockedState(
                    blockedStateChangedDateTime,
                    isBlocked))
                {
                    return;
                }

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCardReaderBlockedStatusChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { idCardReader, isBlocked });
            }
            catch
            {
            }
        }

        public CardReaderSceneType GetCardReaderCommand(Guid guidCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(guidCardReader);

            return
                cardReaderState != null
                    ? cardReaderState.CardReaderCommand
                    : CardReaderSceneType.Unknown;
        }

        //default tolerance time is 2 minutes
        private int _timeDifferenceTolerance = 120000;//(miliseconds)

        public int TimeDifferenceTolerance
        {
            get { return _timeDifferenceTolerance; }

            set
            {
                if (value < 30000)
                    return;

                _timeDifferenceTolerance = value;
                SendNTPTimeDiffTolerance(value);
            }
        }

        private void SendNTPTimeDiffTolerance(int interval)
        {
            foreach (var ccuSettings in _remotingCCUs.Values)
                if (ccuSettings.State == CCUOnlineState.Online &&
                        ccuSettings.ConfigurationState == CCUConfigurationState.ConfiguredForThisServer)
                    SendNTPTimeDifferenceTolerance(
                        ccuSettings.GuidCCU,
                        interval);
        }

        #region CcuMACAddress

        private readonly Mutex _ccuMACAddressChangedMutex = new Mutex();

        public void InitCcuMacAddress(Guid guidCCU)
        {
            try
            {
                var retValue = SendToRemotingCCUs(guidCCU, true, "GetMacAddress");
                var macAddress = retValue as string;

                if (macAddress == null)
                    return;

                SaveNewMACAddress(guidCCU, macAddress);

                _ccuMACAddressChangedMutex.WaitOne();

                try
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCcuMACAddressChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[] { guidCCU, macAddress });
                }
                catch { }

                _ccuMACAddressChangedMutex.ReleaseMutex();
            }
            catch
            {
            }
        }

        private static void SaveNewMACAddress(Guid guidCCU, string macAddress)
        {
            var ccu = CCUs.Singleton.GetObjectForEdit(guidCCU);
            if (ccu == null)
                return;

            if (ccu.MACAddress != macAddress)
            {
                ccu.MACAddress = macAddress;
                CCUs.Singleton.Update(ccu);
                CCUs.Singleton.EditEnd(ccu);
            }

            CCUs.Singleton.EditEnd(ccu);
        }

        public string GetCcuFirmwareVersion(Guid guidCCU)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);

            return
                ccuSetting != null
                    ? ccuSetting.FirmwareVersion
                    : string.Empty;
        }

        public string GetCcuCeVersion(Guid guidCCU)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);

            return
                ccuSetting != null
                    ? ccuSetting.WinCeVersion
                    : string.Empty;
        }

        public MainBoardVariant GetCcuMainBoardType(Guid guidCcu)
        {
            CCUSettings ccuSettings = GetCCUSettings(guidCcu);

            return
                ccuSettings != null
                    ? ccuSettings.MainBoardType
                    : MainBoardVariant.Unknown;
        }
        #endregion

        /// <summary>
        /// Send input changed to the clien and create alarm
        /// </summary>
        /// <param name="idInput"></param>
        /// <param name="dateTime"></param>
        /// <param name="inputState"></param>
        /// <param name="alarmWasGenerated"></param>
        /// <param name="alarmAreas"></param>
        public void InputChanged(
            Guid idInput,
            DateTime? dateTime,
            InputState inputState,
            bool alarmWasGenerated,
            List<Guid> alarmAreas)
        {
            try
            {
                Input input;
                var ccu = GetCCUFromInput(idInput, out input);

                if (ccu != null)
                {
                    CCUSettings ccuSetting;

                    InputStates inputStates = GetInputStates(input, out ccuSetting);

                    if (inputStates != null)
                    {
                        if (!inputStates.SetInputState(dateTime, inputState))
                        {
                            PresentationStateInpendentOfAlarm(
                                inputState,
                                alarmWasGenerated,
                                input);

                            return;
                        }
                    }
                    else
                        if (ccuSetting != null)
                        lock (ccuSetting.InputStates)
                            ccuSetting.InputStates.Add(
                                idInput,
                                new InputStates(
                                    idInput,
                                    dateTime,
                                    inputState));

                    PresentationStateInpendentOfAlarm(
                        inputState,
                        alarmWasGenerated,
                        input);
                }

                Guid parentObjectId =
                        input != null
                            ? (input.CCU != null
                                ? input.CCU.IdCCU
                                : input.DCU != null
                                    ? input.DCU.IdDCU
                                    : Guid.Empty)
                        : Guid.Empty;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunInputChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                        {
                            idInput,
                            (byte)inputState,
                            parentObjectId
                        });
            }
            catch
            {
            }
        }

        private void PresentationStateInpendentOfAlarm(
            InputState inputState,
            bool alarmWasGenerated,
            Input input)
        {
            if (alarmWasGenerated)
                return;

            switch (inputState)
            {
                case InputState.Alarm:

                    if (input.AlarmPresentationGroup != null &&
                        input.AlarmPGPresentationStateInpendentOfAlarm)
                        if (input.AlarmPresentationGroup.ResponseAlarm ||
                            input.AlarmPresentationGroup.ResponseAlarmNotAck)
                            SafeThread<PresentationGroup, string>
                                .StartThread(
                                    PGProcessSendMessage,
                                    input.AlarmPresentationGroup,
                                    string.Format(
                                        "Input '{0}' is in {1}",
                                        input.Name,
                                        inputState));

                    break;

                case InputState.Break:
                case InputState.Short:

                    break;

                default:
                    if (inputState != InputState.Unknown &&
                        inputState != InputState.UsedByAnotherAplication &&
                        inputState != InputState.OutOfRange &&
                        input.AlarmPresentationGroup != null &&
                        input.AlarmPGPresentationStateInpendentOfAlarm)
                        if (input.AlarmPresentationGroup.ResponseNormal ||
                            input.AlarmPresentationGroup.ResponseNormalNotAck ||
                            input.AlarmPresentationGroup.ResponseNormalOffAck)
                            SafeThread<PresentationGroup, string>
                                .StartThread(
                                    PGProcessSendMessage,
                                    input.AlarmPresentationGroup,
                                    string.Format(
                                        "Input '{0}' is in {1}",
                                        input.Name,
                                        inputState));

                    break;
            }
        }

        private void PGProcessSendMessage(PresentationGroup pg, string msg)
        {
            if (NcasServer != null)
                NcasServer.PGProcessSendMessage(
                    pg,
                    msg);
        }

        private InputStates GetInputStates(
            Guid idInput,
            CCU ccu,
            out CCUSettings ccuSettings)
        {
            ccuSettings = GetCCUSettings(ccu.IdCCU);
            if (ccuSettings == null || ccuSettings.InputStates == null)
                return null;

            InputStates inputState;

            ccuSettings.InputStates.TryGetValue(
                idInput,
                out inputState);

            return inputState;
        }

        private InputStates GetInputStates(
            Input input,
            out CCUSettings ccuSettings)
        {
            ccuSettings = null;

            CCU ccu = GetCCUFromInput(input);

            return
                ccu != null
                    ? GetInputStates(input.IdInput, ccu, out ccuSettings)
                    : null;
        }

        public InputState GetCurrentInputState(Input input)
        {
            CCUSettings ccuSettings;

            InputStates inputState = GetInputStates(input, out ccuSettings);

            return
                 inputState != null && ccuSettings.State == CCUOnlineState.Online
                    ? inputState.InputState
                    : InputState.Unknown;
        }

        public bool InputIsActivated(Input input)
        {
            CCUSettings ccuSettings;

            InputStates inputState =
                GetInputStates(
                    input,
                    out ccuSettings);

            return
                inputState != null &&
                inputState.InputState != InputState.Unknown &&
                inputState.InputState != InputState.UsedByAnotherAplication &&
                inputState.InputState != InputState.OutOfRange;
        }

        public void ClearInputsStates(Dictionary<Guid, InputStates> dicInputStates)
        {
            if (dicInputStates == null)
                return;

            foreach (var inputStates in dicInputStates.Values)
                InputChanged(
                    inputStates.IdInput,
                    null,
                    InputState.Unknown,
                    false,
                    null);

            dicInputStates.Clear();
        }

        public void ClearOutputsStates(Dictionary<Guid, OutputStates> dicOutputStates)
        {
            if (dicOutputStates == null)
                return;

            foreach (var outputStates in dicOutputStates.Values)
            {
                OutputStateChanged(
                    outputStates.IdOutput,
                    null,
                    OutputState.Unknown);

                OutputRealStateChanged(
                    outputStates.IdOutput,
                    null,
                    OutputState.Unknown);
            }

            dicOutputStates.Clear();
        }

        public void ClearAlarmAreasStates(Guid guidCCU)
        {
            if (_alarmAreasImplicitCCUAndStates == null)
                return;

            foreach (var kvp in _alarmAreasImplicitCCUAndStates)
            {
                if (kvp.Value.GuidCCU != guidCCU)
                    continue;

                kvp.Value.ClearStates();
            }
        }

        public void ClearDoorEnvironmentStates(Dictionary<Guid, CCUDoorEnvironemntState> doorEnvironmentStates)
        {
            if (doorEnvironmentStates == null)
                return;

            foreach (var doorEnvironmentGuid in doorEnvironmentStates.Keys.ToList())
                DoorEnvironmentStateChanged(
                    doorEnvironmentGuid,
                    null,
                    DoorEnvironmentState.Unknown);

            doorEnvironmentStates.Clear();
        }

        public void ClearCardReaderOnlineStates(Dictionary<Guid, CardReaderState> crOnlineStates)
        {
            if (crOnlineStates == null)
                return;

            lock (crOnlineStates)
            {
                foreach (var crPair in crOnlineStates)
                {
                    var cardReader = CardReaders.Singleton.GetById(crPair.Key);
                    if (cardReader == null)
                        continue;

                    CreateCardReaderOnlineStateChangedEventlog(cardReader, DateTime.Now, OnlineState.Offline);

                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCardReaderChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            cardReader.IdCardReader,
                            (byte) OnlineState.Offline,
                            cardReader.DCU != null
                                ? cardReader.DCU.IdDCU
                                : cardReader.CCU.IdCCU
                        });

                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCardReaderBlockedStatusChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            cardReader.IdCardReader,
                            false
                        });
                }
                crOnlineStates.Clear();
            }
        }

        public void ClearDCUOnlineStates(
            string ccuIpAddress,
            Dictionary<byte, DCUState> dcuStates)
        {
            if (dcuStates == null)
                return;

            foreach (var dcuPair in dcuStates)
            {
                DCUOnlineStateChanged(
                    null,
                    dcuPair.Key,
                    OnlineState.Offline,
                    ccuIpAddress,
                    0xFF);

                DCUPhysicalAddressChanged(
                    null,
                    dcuPair.Key,
                    string.Empty,
                    ccuIpAddress);

                DcuInputsSabotageStateChanged(
                    null,
                    dcuPair.Value.DCUId,
                    State.Unknown);
            }
        }

        private AlarmAreaImplicitCCUAndStates GetAlarmAreaImplicitCCUAndStates(
            Guid idAlarmArea)
        {
            AlarmAreaImplicitCCUAndStates result;

            _alarmAreasImplicitCCUAndStates.TryGetValue(
                idAlarmArea,
                out result);

            return result;
        }

        public void AlarmAreaAlarmStateChanged(
            Guid idAlarmArea,
            DateTime dateTime,
            AlarmAreaAlarmState alarmState)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCcuAndStates != null)
                    alarmAreaImplicitCcuAndStates.SetAlarmAreaAlarmState(
                        dateTime,
                        alarmState);
            }
            catch
            {
            }
        }

        public void AlarmAreaActivationStateChanged(
            Guid idAlarmArea,
            DateTime dateTime,
            ActivationState activationState)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCcuAndStates != null)
                    alarmAreaImplicitCcuAndStates.SetAlarmAreaActivationState(
                        dateTime,
                        activationState);
            }
            catch
            {
            }
        }

        public void AlarmAreaRequestActivationStateChanged(
            Guid idAlarmArea,
            DateTime dateTime,
            RequestActivationState requestActivationState)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndState =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCCUAndState != null)
                    alarmAreaImplicitCCUAndState.SetAlarmAreaRequestActivationState(
                        dateTime,
                        requestActivationState);
            }
            catch
            {
            }
        }

        public void AlarmAreaSetUnsetNotConfirm(Guid idAlarmArea, DateTime dateTime)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndState =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCCUAndState != null)
                    alarmAreaImplicitCCUAndState.SetUnsetNotConfirm(dateTime);
            }
            catch
            {
            }
        }

        public void AlarmAreaRequestActivationStateChanged(
            Guid guidAlarmArea,
            RequestActivationState requestActivationState,
            bool setUnsetNotConfirm)
        {
            _queueAlarmAreaRequestActivationStateChanged.Enqueue(
                new ActivationStateChangedCarrier(
                    guidAlarmArea,
                    (byte)requestActivationState,
                    setUnsetNotConfirm
                ));
        }


        public void AlarmAreaSabotageStateChanged(
            Guid idAlarmArea,
            DateTime dateTime,
            State sabotageState)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndState =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCCUAndState != null)
                    alarmAreaImplicitCCUAndState.SetAlarmAreaSabotageState(
                        dateTime,
                        sabotageState);
            }
            catch
            {
            }
        }

        public void AlarmAreaSensorBlockingTypeChanged(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType sensorBlockingType,
            DateTime dateTime)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCcuAndStates != null)
                    alarmAreaImplicitCcuAndStates.SetSensorBlockingType(
                        idInput,
                        sensorBlockingType,
                        dateTime);
            }
            catch
            {
            }
        }

        public void AlarmAreaSensorStateChanged(
            Guid idAlarmArea,
            Guid idInput,
            State sensorState,
            DateTime dateTime)
        {
            try
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates =
                    GetAlarmAreaImplicitCCUAndStates(idAlarmArea);

                if (alarmAreaImplicitCcuAndStates != null)
                    alarmAreaImplicitCcuAndStates.SetSensorState(
                        idInput,
                        sensorState,
                        dateTime);
            }
            catch
            {
            }
        }

        public OutputState GetOutputState(Output output)
        {
            try
            {
                CCUSettings ccuSettings;

                OutputStates outputState =
                    GetOutputStates(
                        output,
                        out ccuSettings);

                return outputState != null && ccuSettings.State == CCUOnlineState.Online
                    ? outputState.OutputState
                    : OutputState.Unknown;
            }
            catch
            {
                return OutputState.Unknown;
            }
        }

        public OutputState GetOutputRealState(Output output)
        {
            try
            {
                CCUSettings ccuSettings;

                OutputStates outputState =
                    GetOutputStates(output, out ccuSettings);

                if (outputState == null ||
                        ccuSettings.State != CCUOnlineState.Online)
                    return OutputState.Unknown;

                if (!output.RealStateChanges &&
                        outputState.OutputRealState != OutputState.Unknown)
                    outputState.SetOutputRealState(
                        null,
                        OutputState.Unknown);

                return outputState.OutputRealState;
            }
            catch
            {
                return OutputState.Unknown;
            }
        }

        private static CCU GetCCUFromOutput(Guid idOutput, out Output output)
        {
            output = Outputs.Singleton.GetById(idOutput);

            if (output == null)
                return null;

            return
                output.CCU ??
                    (output.DCU != null
                        ? output.DCU.CCU
                        : null);
        }

        private OutputStates GetOutputStates(
            Output output,
            out CCUSettings ccuSettings)
        {
            ccuSettings = null;

            if (output == null)
                return null;

            CCU ccu =
                output.CCU ??
                    (output.DCU != null
                        ? output.DCU.CCU
                        : null);

            if (ccu == null)
                return null;

            ccuSettings = GetCCUSettings(ccu.IdCCU);
            if (ccuSettings == null)
                return null;

            if (ccuSettings.OutputStates == null)
                return null;

            OutputStates outputState;

            ccuSettings.OutputStates.TryGetValue(
                output.IdOutput,
                out outputState);

            return outputState;
        }

        private static Guid GetOutputParentId(Output output)
        {
            return
                output != null
                    ? (output.CCU != null
                        ? output.CCU.IdCCU
                        : (output.DCU != null
                            ? output.DCU.IdDCU
                            : Guid.Empty))
                    : Guid.Empty;
        }

        public void OutputStateChanged(
            Guid idOutput,
            DateTime? dateTime,
            OutputState outputState)
        {
            try
            {
                Output output;
                CCU ccu = GetCCUFromOutput(idOutput, out output);

                if (ccu != null)
                {
                    CCUSettings ccuSettings = GetCCUSettings(ccu.IdCCU);

                    if (ccuSettings != null)
                        lock (ccuSettings.OutputStates)
                        {
                            OutputStates outputStates;
                            if (!ccuSettings.OutputStates.TryGetValue(
                                output.IdOutput,
                                out outputStates))
                                ccuSettings.OutputStates.Add(
                                    idOutput,
                                    new OutputStates(
                                        idOutput,
                                        dateTime,
                                        outputState,
                                        null,
                                        OutputState.Unknown));
                            else
                                if (outputStates != null)
                                if (!outputStates.SetOutputState(
                                    dateTime,
                                    outputState))
                                {
                                    return;
                                }
                        }
                }

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunOutputStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        idOutput,
                        (byte)outputState,
                        GetOutputParentId(output)
                    });
            }
            catch
            {
            }
        }

        public void OutputRealStateChanged(
            Guid idOutput,
            DateTime? dateTime,
            OutputState realOutputState)
        {
            try
            {
                Output output;

                CCU ccu = GetCCUFromOutput(idOutput, out output);

                if (ccu != null)
                {
                    CCUSettings ccuSetting = GetCCUSettings(ccu.IdCCU);

                    if (ccuSetting != null)
                        lock (ccuSetting.OutputStates)
                        {
                            OutputStates outputState;

                            if (!ccuSetting.OutputStates.TryGetValue(
                                    output.IdOutput,
                                    out outputState))
                                ccuSetting.OutputStates.Add(
                                    idOutput,
                                    new OutputStates(
                                        idOutput,
                                        null,
                                        OutputState.Unknown,
                                        dateTime,
                                        realOutputState));
                            else
                                if (outputState != null)
                                if (!outputState.SetOutputRealState(
                                        dateTime,
                                        realOutputState))
                                    return;
                        }
                }

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunOutputRealStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        idOutput,
                        (byte)realOutputState,
                        GetOutputParentId(output)
                    });
            }
            catch
            {
            }
        }

        public bool OutputIsActivated(Output output)
        {
            try
            {
                CCUSettings ccuSetting;

                OutputStates outputState =
                    GetOutputStates(output, out ccuSetting);

                return
                    outputState != null &&
                    outputState.OutputState != OutputState.Unknown &&
                    outputState.OutputState != OutputState.UsedByAnotherAplication &&
                    outputState.OutputState != OutputState.OutOfRange;
            }
            catch
            {
                return false;
            }
        }

        public void DoorEnvironmentStateChanged(
            Guid idDoorEnvironment,
            DateTime? dateTime,
            DoorEnvironmentState doorEnvironmentState)
        {
            try
            {
                var doorEnvironment =
                    DoorEnvironments.Singleton.GetById(idDoorEnvironment);

                Guid ccuId;

                if (doorEnvironment != null)
                {
                    if (!doorEnvironment.Configured)
                        doorEnvironmentState = DoorEnvironmentState.Unknown;

                    ccuId = DoorEnvironments.Singleton.GetParentCcuId(doorEnvironment);
                }
                else
                {
                    ccuId = MultiDoorElements.Singleton.GetParentCcuId(idDoorEnvironment);
                }

                var ccu = CCUs.Singleton.GetById(ccuId);
                if (ccu == null)
                {
                    InvokeRunDoorEnvironmentStateChanged(
                        idDoorEnvironment,
                        doorEnvironmentState);

                    return;
                }

                if (!TrySetDoorEnvironmentState(
                    idDoorEnvironment,
                    dateTime,
                    doorEnvironmentState,
                    ccu,
                    idDoorEnvironment))
                {
                    return;
                }

                InvokeRunDoorEnvironmentStateChanged(
                    idDoorEnvironment,
                    doorEnvironmentState);
            }
            catch
            {
            }
        }

        private static void InvokeRunDoorEnvironmentStateChanged(
            Guid idDoorEnvironment,
            DoorEnvironmentState doorEnvironmentState)
        {
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunDoorEnvironmentStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    idDoorEnvironment,
                    (byte)doorEnvironmentState
                });
        }

        private bool TrySetDoorEnvironmentState(
            Guid idDoorEnvironment,
            DateTime? dateTime,
            DoorEnvironmentState doorEnvironmentState,
            CCU ccu,
            Guid doorEnvironmentId)
        {
            CCUSettings ccuSetting;

            if (!_remotingCCUs.TryGetValue(ccu.IdCCU, out ccuSetting))
                return true;

            if (ccuSetting == null)
                return true;

            CCUDoorEnvironemntState ccuDoorEnvironmentState;

            if (!ccuSetting.DoorEnvironmentStates.TryGetValue(
                doorEnvironmentId,
                out ccuDoorEnvironmentState))
            {
                lock (ccuSetting.DoorEnvironmentStates)
                    ccuSetting.DoorEnvironmentStates.Add(
                        idDoorEnvironment,
                        new CCUDoorEnvironemntState(
                            dateTime,
                            doorEnvironmentState));

                return true;
            }

            if (ccuDoorEnvironmentState == null)
                return true;

            return
                ccuDoorEnvironmentState.SetDoorEnvironmentState(
                    dateTime,
                    doorEnvironmentState);
        }

        public DoorEnvironmentState GetDoorEnvironmentState(Guid ccuId, Guid doorEnvironmentId)
        {
            try
            {
                CCUSettings ccuSetting = GetCCUSettings(ccuId);
                if (ccuSetting == null)
                    return DoorEnvironmentState.Unknown;

                CCUDoorEnvironemntState doorEnvironmentState;
                if (ccuSetting.DoorEnvironmentStates.TryGetValue(
                        doorEnvironmentId,
                        out doorEnvironmentState))
                    return doorEnvironmentState.DoorEnvironmentState;
            }
            catch
            {
            }

            return DoorEnvironmentState.Unknown;
        }

        public void DCUOnlineStateChanged(
            DateTime? dateTime,
            byte logicalAddress,
            OnlineState dcuOnlineState,
            string ccuIpAddress,
            byte protocol)
        {
            try
            {
                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIpAddress);

                if (ccu == null)
                    return;

                CCUSettings ccuSetting = GetCCUSettings(ccu.IdCCU);
                if (ccuSetting == null)
                    return;

                DCUState dcuState;

                ccuSetting.DcuStates.TryGetValue(
                    logicalAddress,
                    out dcuState);

                if (dcuState != null)
                {
                    ExistingDCUOnlineStateChanged(
                        dateTime,
                        logicalAddress,
                        dcuOnlineState,
                        ccuIpAddress,
                        protocol,
                        dcuState,
                        ccu,
                        ccuSetting);

                    return;
                }

                if (dcuOnlineState != OnlineState.Online)
                    return;

                var dcu =
                    DCUs.Singleton.GetDcuByLogicalAddressAndCcu(
                        logicalAddress,
                        ccu)
                    ?? CreateNewDcu(logicalAddress, ccu);

                if (dcu == null)
                    return;

                ccuSetting.DcuStates.Add(
                    logicalAddress,
                    new DCUState(dcu, dateTime, dcuOnlineState));

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCreatedDCUEvent,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false);

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunDCUOnlineStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { dcu.IdDCU, dcuOnlineState, ccu.IdCCU });

                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPECREATEDNEWDCU,
                    DateTime.Now,
                    _thisAssemblyName,
                    new[] { dcu.IdDCU },
                    "Created new DCU",
                    new EventlogParameter(
                        EventlogParameter.TYPEPEDCU,
                        dcu.Name,
                        dcu.IdDCU,
                        ObjectType.DCU));
            }
            catch
            {
            }
        }

        private void ExistingDCUOnlineStateChanged(
            DateTime? dateTime,
            byte logicalAddress,
            OnlineState dcuOnlineState,
            string ccuIpAddress,
            byte protocol,
            DCUState dcuState,
            CCU ccu,
            CCUSettings ccuSetting)
        {
            lock (dcuState)
            {
                OnlineState oldOnlineState = dcuState.OnlineState;

                if (!dcuState.SetOnlineState(dateTime, dcuOnlineState))
                {
                    return;
                }

                bool createdNew;

                var dcu =
                    GetOrCreateDCU(
                        logicalAddress,
                        dcuOnlineState,
                        dcuState,
                        ccu,
                        out createdNew);

                if (dcu != null)
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunDCUOnlineStateChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[] { dcu.IdDCU, dcuOnlineState, ccu.IdCCU });

                    string eventType;
                    string description;

                    GetEventTypeAndDescription(
                        dcuOnlineState,
                        createdNew,
                        out eventType,
                        out description);

                    if (eventType != null)
                        Eventlogs.Singleton.InsertEvent(
                            eventType,
                            DateTime.Now,
                            _thisAssemblyName,
                            new[] { dcu.IdDCU },
                            description,
                            new EventlogParameter(
                                EventlogParameter.TYPEPEDCU,
                                dcu.Name,
                                dcu.IdDCU,
                                ObjectType.DCU));

                    if (!createdNew)
                    {
                        var alarmsManager = NCASServer.Singleton.GetAlarmsQueue();

                        if (dcuOnlineState != OnlineState.WaitingForUpgrade)
                            alarmsManager.TryRunOnAlarmsOwner(
                                serverAlarmsOwner =>
                                    serverAlarmsOwner.StopAlarm(
                                        new AlarmKey(
                                            AlarmType.DCU_WaitingForUpgrade,
                                            new IdAndObjectType(
                                                dcu.IdDCU,
                                                ObjectType.DCU))));
                        else
                            alarmsManager.AddAlarm(
                                new ServerAlarm(
                                    new ServerAlarmCore(
                                        new Alarm(
                                            new AlarmKey(
                                                AlarmType.DCU_WaitingForUpgrade,
                                                new IdAndObjectType(
                                                    dcu.IdDCU,
                                                    ObjectType.DCU)),
                                            AlarmState.Alarm),
                                        new LinkedList<IdAndObjectType>(
                                            Enumerable.Repeat(
                                                new IdAndObjectType(
                                                    dcu.CCU.IdCCU,
                                                    ObjectType.CCU),
                                                1)),
                                        string.Format(
                                            "{0} : {1}",
                                            AlarmType.DCU_WaitingForUpgrade,
                                            dcu.ToString()),
                                        ccu.Name,
                                        string.Format(
                                            "DCU waiting for upgrade: {0}",
                                            dcu.Name))));
                    }
                }

                //send alarm dcu offline
                var dcuAlarm = DCUs.Singleton.GetById(dcuState.DCUId);

                UpdateAlarmDcuOutdatedFirmware(
                    dcuAlarm,
                    dcuState,
                    protocol);
            }
        }

        private static void GetEventTypeAndDescription(
            OnlineState dcuOnlineState,
            bool createdNew,
            out string eventType,
            out string description)
        {
            if (createdNew)
            {
                eventType = Eventlog.TYPECREATEDNEWDCU;
                description = "Created new DCU";

                return;
            }

            switch (dcuOnlineState)
            {
                case OnlineState.Online:
                    eventType = Eventlog.TYPEDCUONLINE;
                    description = "DCU online";
                    break;

                case OnlineState.Offline:
                    eventType = Eventlog.TYPEDCUOFFLINE;
                    description = "DCU offline";
                    break;

                case OnlineState.Upgrading:
                    eventType = Eventlog.TYPEDCUUPGRADING;
                    description = "DCU upgrading";
                    break;

                case OnlineState.AutoUpgrading:
                    eventType = Eventlog.TYPEDCUAUTOUPGRADING;
                    description = "DCU auto upgrading";
                    break;

                case OnlineState.WaitingForUpgrade:
                    eventType = Eventlog.TYPEDCUWAITINGFORUPGRADE;
                    description = "DCU waiting for upgrade";
                    break;

                default:
                    eventType = null;
                    description = null;
                    break;
            }
        }

        private static DCU GetOrCreateDCU(
            byte logicalAddress,
            OnlineState dcuOnlineState,
            DCUState dcuState,
            CCU ccu,
            out bool createdNew)
        {
            var dcu = DCUs.Singleton.GetById(dcuState.DCUId);

            if (dcu != null || dcuOnlineState != OnlineState.Online)
            {
                createdNew = false;
                return dcu;
            }

            dcu = CreateNewDcu(logicalAddress, ccu);
            if (dcu == null)
            {
                createdNew = false;
                return null;
            }

            createdNew = true;

            dcuState.SetNewID(dcu.IdDCU);

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunCreatedDCUEvent,
                DelegateSequenceBlockingMode.Asynchronous,
                false);

            return dcu;
        }

        public void DcuInputsSabotageStateChanged(
            DateTime? dateTime,
            Guid dcuId,
            State dcuInputsSabotageState)
        {
            DCU dcu = DCUs.Singleton.GetById(dcuId);

            CCUSettings ccuSettings;

            DCUState dcuState = GetDcuState(dcu, out ccuSettings);

            if (dcuState == null)
                return;

            if (!dcuState.SetInputsSabotageState(dateTime, dcuInputsSabotageState))
                return;

            object[] objDcu = { dcu.IdDCU, dcuInputsSabotageState };

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunDcuInputsSabotageStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                objDcu);
        }

        private void UpdateAlarmDcuOutdatedFirmware(DCU dcu, DCUState dcuState, byte protocol)
        {
            if (protocol == 0)
                return;

            if (dcu == null || dcu.CCU == null)
                return;

            if (dcuState == null || string.IsNullOrEmpty(dcuState.FirmwareVersion))
                return;

            try
            {
                if (dcuState.OnlineState != OnlineState.Online)
                {
                    NcasServer.GetAlarmsQueue().TryRunOnAlarmsOwner(
                        serverAlarmsOwner =>
                            serverAlarmsOwner.StopAlarm(
                                new AlarmKey(
                                    AlarmType.DCU_OutdatedFirmware,
                                    new IdAndObjectType(
                                        dcu.IdDCU,
                                        ObjectType.DCU))));

                    return;
                }

                var currentFW = new Version(dcuState.FirmwareVersion);

                CCUSettings ccuSettings = GetCCUSettings(dcu.CCU.IdCCU);
                if (ccuSettings == null)
                    return;

                MainBoardVariant mbType = ccuSettings.MainBoardType;

                Version minimalFW;

                switch (mbType)
                {
                    case MainBoardVariant.CCU0_ECHELON:
                        minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_DCU;
                        break;
                    case MainBoardVariant.CCU0_RS485:
                        minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_RS485_DCU;
                        break;
                    default:
                        return;
                }

                if (currentFW >= minimalFW ||
                    // failsafe threshold
                    currentFW <= new Version(1, 6, 4000))
                {
                    NcasServer.GetAlarmsQueue().TryRunOnAlarmsOwner(
                        serverAlarmsOwner =>
                            serverAlarmsOwner.StopAlarm(
                                new AlarmKey(
                                    AlarmType.DCU_OutdatedFirmware,
                                    new IdAndObjectType(
                                        dcu.IdDCU,
                                        ObjectType.DCU))));

                    return;
                }

                NcasServer.GetAlarmsQueue().AddAlarm(
                    new ServerAlarm(
                        new ServerAlarmCore(
                            new Alarm(

                                new AlarmKey(
                                    AlarmType.DCU_OutdatedFirmware,
                                    new IdAndObjectType(
                                        dcu.IdDCU,
                                        ObjectType.DCU)),
                                AlarmState.Alarm),
                            new LinkedList<IdAndObjectType>(
                                Enumerable.Repeat(
                                    new IdAndObjectType(
                                        dcu.CCU.IdCCU,
                                        ObjectType.CCU),
                                    1)),
                            string.Format(
                                "{0} : {1}",
                                AlarmType.DCU_OutdatedFirmware,
                                dcu.ToString()),
                            dcu.CCU.Name,
                            string.Format(
                                "{0} {1}\r\n{2}: {3}",
                                NCASServer.Singleton.LocalizationHelper.GetString("DCUOutdatedFirmware"),
                                dcuState.FirmwareVersion,
                                NCASServer.Singleton.LocalizationHelper.GetString("MinimalSupportedFW"),
                                minimalFW))));
            }
            catch { }
        }

        private static DCU CreateNewDcu(byte logicalAddress, CCU parentCcu)
        {
            var dcu = new DCU
            {
                LogicalAddress = logicalAddress,
                CCU = parentCcu
            };

            dcu.Name =
                string.Format(
                    "{0}{1}{2}{3}",
                    dcu.EnableParentInFullName
                        ? ""
                        : dcu.CCU.Name + StringConstants.SLASHWITHSPACES,
                    CgpServer.Singleton.LocalizationHelper.GetString("DCUText"),
                    StringConstants.SPACE,
                    logicalAddress);

            if (!DCUs.Singleton.InsertOnlyInDatabase(ref dcu))
                return null;

            var editingDCU = DCUs.Singleton.GetObjectForEdit(dcu.IdDCU);
            if (editingDCU == null)
                return dcu;

            if (editingDCU.DoorEnvironments == null)
                editingDCU.DoorEnvironments = new List<DoorEnvironment>();

            editingDCU.DoorEnvironments.Add(DoorEnvironments.Singleton.CreateNewForDCU());

            DCUs.Singleton.Update(editingDCU);
            DCUs.Singleton.EditEnd(editingDCU);

            return dcu;
        }

        private void ClearAllDCUStates(Guid idDCU, CCUSettings ccuSetting)
        {
            if (ccuSetting == null || idDCU == Guid.Empty)
                return;

            if (ccuSetting.InputStates != null)
                lock (ccuSetting.InputStates)
                    foreach (var inputState in ccuSetting.InputStates.Values)
                    {
                        var input = Inputs.Singleton.GetById(inputState.IdInput);

                        if (input == null || input.DCU == null || input.DCU.IdDCU != idDCU)
                            continue;

                        inputState.SetInputState(
                            null,
                            InputState.Unknown);

                        InputChanged(
                            inputState.IdInput,
                            null,
                            InputState.Unknown,
                            false,
                            null);
                    }

            if (ccuSetting.OutputStates != null)
                lock (ccuSetting.OutputStates)
                    foreach (var outputState in ccuSetting.OutputStates.Values)
                    {
                        var output = Outputs.Singleton.GetById(outputState.IdOutput);

                        if (output == null || output.DCU == null || output.DCU.IdDCU != idDCU)
                            continue;

                        outputState.SetOutputState(null, OutputState.Unknown);
                        outputState.SetOutputRealState(null, OutputState.Unknown);

                        OutputStateChanged(
                            outputState.IdOutput,
                            null,
                            OutputState.Unknown);

                        OutputRealStateChanged(outputState.IdOutput, null, OutputState.Unknown);
                    }

            if (ccuSetting.DoorEnvironmentStates != null && ccuSetting.DoorEnvironmentStates.Count > 0)
            {
                var dcu = DCUs.Singleton.GetById(idDCU);

                if (dcu != null && dcu.DoorEnvironments != null)
                    foreach (var doorEnvironment in dcu.DoorEnvironments)
                        DoorEnvironmentStateChanged(
                            doorEnvironment.IdDoorEnvironment,
                            null,
                            DoorEnvironmentState.Unknown);
            }

            var actDcu = DCUs.Singleton.GetById(idDCU);

            if (actDcu != null)
                DCUPhysicalAddressChanged(
                    null,
                    actDcu.LogicalAddress,
                    string.Empty,
                    ccuSetting.IPAddressString);

            DcuInputsSabotageStateChanged(
                null,
                idDCU,
                State.Unknown);
        }

        public void DCUPhysicalAddressChanged(
            DateTime? dateTime,
            byte logicalAddress,
            string dcuPhysicalAddress,
            string ccuIpAddress)
        {
            try
            {
                var idCCU = GetIdCCUFromIpAddress(ccuIpAddress);
                if (idCCU == Guid.Empty)
                    return;

                CCUSettings ccuSetting = GetCCUSettings(idCCU);
                if (ccuSetting == null)
                    return;

                DCUState dcuState;
                if (!ccuSetting.DcuStates.TryGetValue(logicalAddress, out dcuState) || dcuState == null)
                    return;

                lock (dcuState)
                    if (dcuState.SetPhysicalAddress(dateTime, dcuPhysicalAddress))
                        NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                            CCUCallbackRunner.RunDCUPhysicalAddressChanged,
                            DelegateSequenceBlockingMode.Asynchronous,
                            false,
                            new object[] { dcuState.DCUId, dcuPhysicalAddress });
            }
            catch
            {
            }
        }

        public void DCUInputCount(Guid dcuGuid, byte inputCount)
        {
            try
            {
                var dcu = DCUs.Singleton.GetById(dcuGuid);

                if (dcu != null)
                    DCUs.Singleton.CreateOwnInputs(dcu, inputCount, "Input");
            }
            catch
            {
            }
        }

        public void DCUOutputCount(Guid dcuGuid, byte outputCount)
        {
            try
            {
                var dcu = DCUs.Singleton.GetById(dcuGuid);

                if (dcu != null)
                    DCUs.Singleton.CreateOwnOutputs(dcu, outputCount, "Output");
            }
            catch
            {
            }
        }

        public void DCUFirmwareVersion(Guid dcuGuid, string firmwareVersion)
        {
            try
            {
                var dcu = DCUs.Singleton.GetById(dcuGuid);

                CCUSettings ccuSetting;
                DCUState dcuState = GetDcuState(dcu, out ccuSetting);

                if (dcuState == null)
                    return;

                dcuState.FirmwareVersion = firmwareVersion;
                UpdateAlarmDcuOutdatedFirmware(dcu, dcuState, 255);
            }
            catch
            {
            }
        }

        public void DcuUpgradePercentageSet(byte dcuLogicalAddress, int percents, string ccuIpAddress)
        {
            try
            {
                IPAddress ccuIP;

                if (!IPAddress.TryParse(ccuIpAddress, out ccuIP))
                    return;

                var idCCU = GetIdCCUFromIpAddress(ccuIP.ToString());

                if (idCCU == Guid.Empty)
                    return;

                CCUSettings ccuSetting = GetCCUSettings(idCCU);
                if (ccuSetting == null)
                    return;

                if (percents < 0 || percents == 100)
                    NCASServer.Singleton.ClearDCUUpgradeResources(
                        idCCU,
                        dcuLogicalAddress);

                object[] objDcus =
                {
                    new DCUUpgradeState(
                        idCCU,
                        new[] { dcuLogicalAddress },
                        percents,
                        null)
                };

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunDCUsUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    objDcus);
            }
            catch
            {
            }
        }

        public void CRUpgradeResultSet(
            int dcuLogicalAddress,
            byte cardReaderAddress,
            byte byteResult,
            string ccuIpAddress)
        {
            try
            {
                var result = (CRUpgradeResult)byteResult;
                IPAddress ccuIP;

                if (!IPAddress.TryParse(ccuIpAddress, out ccuIP))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIP.ToString());

                if (ccu == null)
                    return;

                DCU dcu = null;
                if (dcuLogicalAddress != -1)
                {
                    dcu = ccu.DCUs.FirstOrDefault(
                        dcuObject => dcuObject.LogicalAddress == dcuLogicalAddress);

                    if (dcu == null)
                        return;
                }

                var cardReaders =
                    dcu != null
                        ? dcu.CardReaders
                        : ccu.CardReaders;

                CardReader cardReader = cardReaders.FirstOrDefault(
                    cr => cr.Address == cardReaderAddress);

                if (cardReader == null)
                    return;

                SetCrStateUpgrading(
                    ccu.IdCCU,
                    cardReader.IdCardReader,
                    false);

                if (GetCardReaderOnlineState(cardReader.IdCardReader) != OnlineState.Online)
                    SetCROnlineState(
                        ccu.IdCCU,
                        cardReader.IdCardReader,
                        OnlineState.Online);

                if (result != CRUpgradeResult.Success || result != CRUpgradeResult.FlashingFinishedSuccessfuly)
                {
                    var description = string.Empty;

                    switch (result)
                    {
                        case CRUpgradeResult.Unknown:
                            description = "Unknown error";
                            break;
                        case CRUpgradeResult.UnableToEnterBootloader:
                            description = "Unable to enter bootloader";
                            break;
                        case CRUpgradeResult.FailedToReadBinaryFile:
                            description = "Failed to read binary file";
                            break;
                        case CRUpgradeResult.CardReaderWentOffline:
                            description = "Card reader went offline";
                            break;
                        case CRUpgradeResult.FlashingAcknowledgeTimedOut:
                            description = "Flashing acknowledge timed out";
                            break;
                        case CRUpgradeResult.FlashingOnCRFailed:
                            description = "Flashing on card reader failed";
                            break;
                        case CRUpgradeResult.BootloaderAfterUpgradeAgain:
                            description = "Booltoader is running after upgrade";
                            break;
                        case CRUpgradeResult.ResetToApplicationTimedOut:
                            description = "Reset to application timed out";
                            break;
                        case CRUpgradeResult.IncorrectCardReaderType:
                            description = "Incorrect card reader type";
                            break;
                    }

                    var eventSources =
                        dcu != null
                            ? new[] { ccu.IdCCU, dcu.IdDCU, cardReader.IdCardReader }
                            : new[] { ccu.IdCCU, cardReader.IdCardReader };

                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPECARDREADERSUPGRADEFAILED,
                        DateTime.Now,
                        _thisAssemblyName,
                        eventSources,
                        description,
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));
                }

                var dcuGuid =
                    dcu != null
                        ? (Guid?)dcu.IdDCU
                        : null;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCRsUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        new CRUpgradeState(
                        ccu.IdCCU,
                        dcuGuid,
                        new[] { cardReader.Address },
                        (byte)result,
                        null,
                        null)
                    });
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        public void CRUpgradePercentageSet(
            int dcuLogicalAddress,
            byte cardReaderAddress,
            int percents,
            string ccuIpAddress)
        {
            try
            {
                IPAddress ccuIP;
                if (!IPAddress.TryParse(ccuIpAddress, out ccuIP))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIP.ToString());
                if (ccu == null)
                    return;

                DCU dcu =
                    dcuLogicalAddress != -1
                        ? ccu.DCUs.FirstOrDefault(dcuObject =>
                            dcuObject.LogicalAddress == dcuLogicalAddress)
                        : null;

                if (dcuLogicalAddress != -1 && dcu == null)
                    return;

                var cardReaders =
                    dcu != null
                        ? dcu.CardReaders
                        : ccu.CardReaders;

                CardReader cardReader =
                    cardReaders.FirstOrDefault(cr =>
                        cr.Address == cardReaderAddress);

                if (cardReader == null)
                    return;

                if (GetCardReaderOnlineState(cardReader.IdCardReader) != OnlineState.Upgrading)
                    return;

                Guid? dcuGuid = dcu != null ? (Guid?)dcu.IdDCU : null;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCRsUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        new CRUpgradeState(
                        ccu.IdCCU,
                        dcuGuid,
                        new[] { cardReader.Address },
                        null,
                        percents,
                        null)
                    });
            }
            catch
            {
            }
        }

        public void CEUpgradeFinished(
            int actionUpgrede,
            int actionResult,
            string version,
            string ccuIpAddress)
        {
            try
            {
                IPAddress ipAddress;
                if (!IPAddress.TryParse(ccuIpAddress, out ipAddress))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress.ToString());
                if (ccu == null)
                    return;

                CCUSettings ccuSetting = GetCCUSettings(ccu.IdCCU);
                if (ccuSetting == null)
                    return;

                var result = (ActionResultUpgrade)actionResult;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCEUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { ipAddress, result.ToString() });

                if (result == ActionResultUpgrade.IsJustRunning ||
                        result == ActionResultUpgrade.Started ||
                        result == ActionResultUpgrade.Success)
                    return;

                NCASServer.Singleton.ClearCCUUpgradeResources(ccu.IdCCU);
                StopUpgradeMode(ccu.IdCCU, false);

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    ccuSetting.RunCCUConfiguredChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false);
            }
            catch
            {
            }
        }

        public void ProcessDCUUpgradePackageFailed(
            byte[] dcusWaiting,
            byte byteErrorCode,
            string ccuIpAddress)
        {
            try
            {
                var errorCode = (UnpackPackageFailedCode)byteErrorCode;

                IPAddress ipAddress;
                if (!IPAddress.TryParse(ccuIpAddress, out ipAddress))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress.ToString());
                if (ccu == null)
                    return;

                var description = string.Empty;
                switch (errorCode)
                {
                    case UnpackPackageFailedCode.UnsupportedHeaderFormat:
                        description = "Unsupported package header format (DCU upgrade package)";
                        break;
                    case UnpackPackageFailedCode.UnpackFailed:
                        description = "Failed to unpack package (DCU upgrade package)";
                        break;
                    case UnpackPackageFailedCode.GetChecksumFailed:
                        description = "Failed to get checksum from header (DCU upgrade package)";
                        break;
                    case UnpackPackageFailedCode.ChecksumDoesNotMatch:
                        description = "Checksum does not match (DCU upgrade package)";
                        break;
                    case UnpackPackageFailedCode.Other:
                        description = "An unknown error occured (DCU upgrade package)";
                        break;
                }

                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPEDCUSUPGRADEFAILED,
                    DateTime.Now,
                    _thisAssemblyName,
                    new[] { ccu.IdCCU },
                    description,
                    new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                        GetCcuFirmwareVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                        GetCcuCeVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                        CgpServer.Singleton.Version));

                NCASServer.Singleton.ClearDCUUpgradeResources(ccu.IdCCU, dcusWaiting);

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunDCUsUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        new DCUUpgradeState(
                            ccu.IdCCU,
                            dcusWaiting ?? new byte[] { 0 },
                            null,
                            (byte)errorCode)
                    });
            }
            catch
            {
            }
        }

        public void ProcessCRUpgradePackageFailed(
            byte byteErrorCode,
            byte[] dcusWaiting,
            string ccuIpAddress)
        {
            try
            {
                var errorCode = (UnpackPackageFailedCode)byteErrorCode;

                IPAddress ipAddress;
                if (!IPAddress.TryParse(ccuIpAddress, out ipAddress))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress.ToString());
                if (ccu == null)
                    return;

                var description = string.Empty;

                switch (errorCode)
                {
                    case UnpackPackageFailedCode.UnsupportedHeaderFormat:
                        description = "Unsupported package header format (CR upgrade package)";
                        break;

                    case UnpackPackageFailedCode.UnpackFailed:
                        description = "Failed to unpack package (CR upgrade package)";
                        break;

                    case UnpackPackageFailedCode.GetChecksumFailed:
                        description = "Failed to get checksum from header (CR upgrade package)";
                        break;

                    case UnpackPackageFailedCode.ChecksumDoesNotMatch:
                        description = "Checksum does not match (CR upgrade package)";
                        break;

                    case UnpackPackageFailedCode.Other:
                        description = "An unknown error occured (CR upgrade package)";
                        break;
                }

                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPECARDREADERSUPGRADEFAILED,
                    DateTime.Now,
                    _thisAssemblyName,
                    new[] { ccu.IdCCU },
                    description,
                    new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                        GetCcuFirmwareVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                        GetCcuCeVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                        CgpServer.Singleton.Version));

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCRsUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        new CRUpgradeState(
                            ccu.IdCCU,
                            null,
                            dcusWaiting ?? new byte[] { 0 },
                            null,
                            null,
                            (byte)errorCode)
                    });
            }
            catch
            {
            }
        }

        public void DcuMemoryWarning(Guid dcuGuid, byte memoryLoad)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunDcuMemoryWarning,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { dcuGuid, memoryLoad });
            }
            catch { }
        }

        public void CCUTFTPFileReceived(string fileName, string ccuIpAddress)
        {
            try
            {
                IPAddress ipAddress;
                if (IPAddress.TryParse(ccuIpAddress, out ipAddress))
                    NCASServer.Singleton.OnCCUFileReceived(ipAddress, fileName);
            }
            catch
            {
            }
        }

        public void CCUUpgraderStartFailed(string ccuIpAddress)
        {
            try
            {
                IPAddress ipAddress;
                if (!IPAddress.TryParse(ccuIpAddress, out ipAddress))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress.ToString());
                if (ccu == null)
                    return;

                NCASServer.Singleton.ClearCCUUpgradeResources(ccu.IdCCU);
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCCUUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { ipAddress, -1 });

                CCUSettings ccuSetting = GetCCUSettings(ccu.IdCCU);
                if (ccuSetting == null)
                    return;

                ccuSetting.StopUpgradeMode(false);
            }
            catch
            {
            }
        }

        private void CcuUpgradeFileUnpackProgress(object[] parts)
        {
            try
            {
                if (parts.Length != 2 || !(parts[0] is int))
                    return;

                var ccuIpAddress = parts[1] as string;
                if (ccuIpAddress == null)
                    return;

                CcuUpgradeFileUnpackProgress(
                    (int)parts[0],
                    ccuIpAddress);
            }
            catch
            {
            }
        }

        public void CcuUpgradeFileUnpackProgress(
            int progress,
            string ccuIpAddress)
        {
            try
            {
                IPAddress ccuIP;
                if (!IPAddress.TryParse(ccuIpAddress, out ccuIP))
                    return;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIP.ToString());
                if (ccu == null)
                    return;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCCUUpgradeProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { ccuIP, progress });
            }
            catch
            {
            }
        }

        private void CcuUpgradeFinished(object[] parts)
        {
            try
            {
                if (parts.Length != 3 || !(parts[0] is string) || !(parts[1] is bool))
                    return;

                var ipAsString = parts[2] as string;

                if (ipAsString == null)
                    return;

                IPAddress ccuIP;
                if (!IPAddress.TryParse(ipAsString, out ccuIP))
                    return;

                var success = (bool)parts[1];
                var serverHashCode = parts[0] as string;

                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIP.ToString());

                if (ccu == null || !serverHashCode.Equals(GetServerHashCode(ccu.IdCCU)))
                    return;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCCUUpgradeFinished,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        ccuIP,
                        success
                    });

                if (!success)
                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPECCUUPGRADEFAILED,
                        DateTime.Now,
                        _thisAssemblyName,
                        new[] { ccu.IdCCU },
                        "CCU upgrader: failed to upgrade CCU",
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));

                SendToRemotingCCUs(
                    ccu.IdCCU,
                    true,
                    "ConfirmUpgradeFinish");
            }
            catch
            {
            }
        }

        private void CcuConfiguredStateChanged(object[] parts)
        {
            try
            {
                if (parts.Length != 3 || !(parts[0] is bool) || !(parts[1] is string))
                    return;

                var ipAsString = parts[2] as string;

                if (ipAsString == null)
                    return;

                IPAddress ccuIP;
                if (!IPAddress.TryParse(ipAsString, out ccuIP))
                    return;

                var isConfigured = (bool)parts[0];
                var serverHashCode = parts[1] as string;

                var idCCU = GetIdCCUFromIpAddress(ccuIP.ToString());
                if (idCCU == Guid.Empty)
                    return;

                CCUSettings ccuSetting = GetCCUSettings(idCCU);
                if (ccuSetting == null)
                    return;

                CCUConfigurationState ccuCfgState =
                    isConfigured
                        ? (serverHashCode == ccuSetting.ServerHashCode
                            ? (IsFirmwareSupported(ccuSetting.FirmwareVersion)
                                ? CCUConfigurationState.ConfiguredForThisServer
                                : (IsWinCeSupported(ccuSetting.FirmwareVersion, ccuSetting.WinCeVersion)
                                    ? CCUConfigurationState.ConfiguredForThisServerUpgradeOnly
                                    : CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe))
                            : CCUConfigurationState.ConfiguredForAnotherServer)
                        : CCUConfigurationState.Unconfigured;

                ccuSetting.ChangeConfigurationState(ccuCfgState);

                var ccu = CCUs.Singleton.GetById(idCCU);
                if (ccu != null)
                {
                    ccuSetting.UpdateCcuAlarms(
                        ccu,
                        true,
                        ccuCfgState);
                }

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    ccuSetting.RunCCUConfiguredChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false);
            }
            catch
            {
            }
        }

        private int? lastReported;
        public void FreeComboLicenceChanged(int freeCount)
        {
            if (lastReported != null)
            {
                if (lastReported.Value == 0)
                {
                    if (freeCount == 0)
                        return;
                }
                else
                {
                    if (freeCount != 0)
                        return;
                }
            }

            lastReported = freeCount;

            IList<CCUSettings> comboCCUs = new List<CCUSettings>();
            lock (_remotingCCUs)
            {
                foreach (CCUSettings ccuSetting in _remotingCCUs.Values)
                {
                    if (ccuSetting.IsCat12Combo)
                        comboCCUs.Add(ccuSetting);
                }
            }

            foreach (CCUSettings ccuSetting in comboCCUs)
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    ccuSetting.RunCCUConfiguredChanged,
                    DelegateSequenceBlockingMode.OverallBlocking,
                    false);
            }
        }

        private static void CCULookupFinished(
            ICollection<LookupedCcu> ipAddresses,
            ICollection<Guid> lookupingClients)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCCULookupFinished,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        ipAddresses,
                        lookupingClients
                    });
            }
            catch
            {
            }
        }

        private static void AlarmTransmittersLookupFinished(
            ICollection<LookupedAlarmTransmitter> lookupedTransmitters,
            ICollection<Guid> lookupingClients)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunAlarmTransmittersLookupFinished,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        lookupedTransmitters,
                        lookupingClients
                    });
            }
            catch
            {
            }
        }

        public OnlineState GetDCUOnlineState(DCU dcu)
        {
            try
            {
                CCUSettings ccuSetting;

                DCUState dcuState = GetDcuState(dcu, out ccuSetting);
                if (ccuSetting != null)
                {
                    if (dcuState != null)
                        return dcuState.OnlineState;
#if !DEBUG
                    if (ccuSetting.State == CCUOnlineState.Online)
                        return OnlineState.Offline;
#endif
                }
            }
            catch
            {
            }

            return OnlineState.Unknown;
        }

        public DCUState GetDcuState(
            DCU dcu,
            out CCUSettings ccuSettings)
        {
            if (dcu == null || dcu.CCU == null)
            {
                ccuSettings = null;
                return null;
            }

            ccuSettings = GetCCUSettings(dcu.CCU.IdCCU);

            if (ccuSettings == null)
                return null;

            DCUState dcuState;

            ccuSettings.DcuStates.TryGetValue(
                dcu.LogicalAddress,
                out dcuState);

            return dcuState;
        }

        public State GetDcuInputsSabotageState(DCU dcu)
        {
            CCUSettings ccuSettings;

            DCUState dcuState =
                GetDcuState(
                    dcu,
                    out ccuSettings);

            return
                dcuState != null
                    ? dcuState.InputsSabotageState
                    : State.Unknown;
        }

        public string GetDCUPhysicalAddress(DCU dcu)
        {
            CCUSettings ccuSettings;

            DCUState dcuState =
                GetDcuState(
                    dcu,
                    out ccuSettings);

            return
                dcuState != null
                    ? dcuState.PhysicalAddress
                    : string.Empty;
        }

        public string GetDCUFirmwareVersion(DCU dcu)
        {
            CCUSettings ccuSettings;

            DCUState dcuState =
                GetDcuState(
                    dcu,
                    out ccuSettings);

            return
                dcuState != null
                    ? dcuState.FirmwareVersion
                    : string.Empty;
        }

        public MaximumVersionAndIds ReadMaximumVersionAndIds(Guid guidCCU, ObjectType objectType)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "ReadMaximumVersionAndIds", (byte)objectType);
            return retValue as MaximumVersionAndIds;
        }

        public AlarmAreaActionResult SetAlarmArea(
            Guid alarmAreaGuid,
            Guid idLogin,
            bool noPrewarning)
        {
            var ccu = GetImplicitCCUForAlarmArea(alarmAreaGuid);
            if (ccu == null)
                return AlarmAreaActionResult.FailedNoImplicitManager;

            var retValue = SendToRemotingCCUs(
                ccu.IdCCU,
                "SetAlarmArea",
                alarmAreaGuid,
                idLogin,
                noPrewarning);

            if (!(retValue is bool))
                return AlarmAreaActionResult.FailedCCUOffline;

            if (!(bool)retValue)
                return AlarmAreaActionResult.FailedInputInAlarm;

            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            if (_alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmAreaGuid,
                        out alarmAreaImplicitCCUAndStates) &&
                    alarmAreaImplicitCCUAndStates != null)
                alarmAreaImplicitCCUAndStates
                    .SetAlarmAreaRequestActivationState(
                        RequestActivationState.Set);

            return AlarmAreaActionResult.Success;
        }

        /// <summary>
        /// Registers DCUs for upgrade with specific version
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="logicalAddresses"></param>
        /// <param name="upgradeVersion"></param>
        /// <returns>DCUs, that are successfully registered</returns>
        public byte[] RegisterDCUsForUpgradeVersion(
            string ipAddress,
            byte[] logicalAddresses,
            string upgradeVersion)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress);
            if (ccu == null)
                return null;

            var retValue =
                SendToRemotingCCUs(
                    ccu.IdCCU,
                    "RegisterDCUsForUpgradeVersion",
                    logicalAddresses,
                    upgradeVersion);

            return
                retValue is byte[]
                    ? (byte[])retValue
                    : null;
        }

        public AlarmAreaActionResult UnsetAlarmArea(Guid alarmAreaGuid, Guid guidLogin, Guid guidPerson, int timeToBuy)
        {
            var ccu = GetImplicitCCUForAlarmArea(alarmAreaGuid);
            if (ccu == null)
                return AlarmAreaActionResult.FailedNoImplicitManager;


            var retValue = SendToRemotingCCUs(ccu.IdCCU, "UnsetAlarmArea", alarmAreaGuid, guidLogin, guidPerson, timeToBuy);

            if (!(retValue is bool) || !(bool)retValue)
                return AlarmAreaActionResult.FailedCCUOffline;

            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            if (_alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmAreaGuid,
                        out alarmAreaImplicitCCUAndStates) &&
                    alarmAreaImplicitCCUAndStates != null)
                alarmAreaImplicitCCUAndStates
                    .SetAlarmAreaRequestActivationState(
                        RequestActivationState.Unset);

            return AlarmAreaActionResult.Success;
        }

        public AlarmAreaActionResult UnconditionalSetAlarmArea(
            Guid alarmAreaGuid,
            Guid idLogin,
            bool noPrewarning)
        {
            var ccu = GetImplicitCCUForAlarmArea(alarmAreaGuid);
            if (ccu == null)
                return AlarmAreaActionResult.FailedNoImplicitManager;

            var retValue =
                SendToRemotingCCUs(
                    ccu.IdCCU,
                    "UnconditionalSetAlarmArea",
                    alarmAreaGuid,
                    idLogin,
                    noPrewarning);

            if (!(retValue is bool) || !(bool)retValue)
                return AlarmAreaActionResult.FailedCCUOffline;

            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            if (!_alarmAreasImplicitCCUAndStates.TryGetValue(
                    alarmAreaGuid,
                    out alarmAreaImplicitCCUAndStates))
                return AlarmAreaActionResult.Success;

            if (alarmAreaImplicitCCUAndStates == null)
                return AlarmAreaActionResult.Success;

            alarmAreaImplicitCCUAndStates
                .SetAlarmAreaRequestActivationState(
                    RequestActivationState.UnconditionalSet);

            return AlarmAreaActionResult.Success;
        }

        public AlarmAreaAlarmState GetAlarmAreaAlarmState(Guid alarmAreaGuid)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            return
                _alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmAreaGuid,
                        out alarmAreaImplicitCCUAndStates)
                    ? alarmAreaImplicitCCUAndStates.AlarmState
                    : AlarmAreaAlarmState.Normal;
        }

        public ActivationState GetAlarmAreaActivationState(Guid alarmAreaGuid)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            return
                _alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmAreaGuid,
                        out alarmAreaImplicitCCUAndStates)
                    ? alarmAreaImplicitCCUAndStates.ActivationState
                    : ActivationState.Unknown;
        }

        public RequestActivationState GetAlarmAreaRequestActivationState(Guid alarmAreaGuid)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            return
                _alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmAreaGuid,
                        out alarmAreaImplicitCCUAndStates)
                    ? alarmAreaImplicitCCUAndStates.RequestActivationState
                    : RequestActivationState.Unknown;
        }

        public State GetAlarmAreaSabotageState(Guid alarmAreaGuid)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            return
                _alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmAreaGuid,
                        out alarmAreaImplicitCCUAndStates)
                    ? alarmAreaImplicitCCUAndStates.SabotageState
                    : State.Unknown;
        }

        public SensorBlockingType? GetAlarmAreaSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates;

            return
                _alarmAreasImplicitCCUAndStates.TryGetValue(
                        idAlarmArea,
                        out alarmAreaImplicitCcuAndStates)
                    ? alarmAreaImplicitCcuAndStates.GetSensorBlockingType(idInput)
                    : null;
        }

        public void SetSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType blockingType)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates;

            if (!_alarmAreasImplicitCCUAndStates.TryGetValue(
                idAlarmArea,
                out alarmAreaImplicitCcuAndStates))
                return;

            SendToRemotingCCUs(
                alarmAreaImplicitCcuAndStates.GuidCCU,
                false,
                "SetSensorBlockingType",
                idAlarmArea,
                idInput,
                blockingType);
        }

        public State? GetAlarmAreaSensorState(
            Guid idAlarmArea,
            Guid idInput)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCcuAndStates;

            return
                _alarmAreasImplicitCCUAndStates.TryGetValue(
                        idAlarmArea,
                        out alarmAreaImplicitCcuAndStates)
                    ? alarmAreaImplicitCcuAndStates.GetSensorState(idInput)
                    : null;
        }

        private readonly Dictionary<Guid, AlarmAreaImplicitCCUAndStates> _alarmAreasImplicitCCUAndStates =
            new Dictionary<Guid, AlarmAreaImplicitCCUAndStates>();

        public CCU GetImplicitCCUForAlarmArea(Guid guidAlarmArea)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            if (!_alarmAreasImplicitCCUAndStates.TryGetValue(
                    guidAlarmArea,
                    out alarmAreaImplicitCCUAndStates))
                return null;

            var guidCCU = alarmAreaImplicitCCUAndStates.GuidCCU;

            return
                guidCCU != Guid.Empty
                    ? CCUs.Singleton.GetById(guidCCU)
                    : null;
        }

        public IList<AOrmObject> GetAlarmAreasOfImplicitCcu(Guid idCcu)
        {
            if (_alarmAreasImplicitCCUAndStates == null)
                return null;

            return
                _alarmAreasImplicitCCUAndStates
                    .Where(kvPair => kvPair.Value.GuidCCU == idCcu)
                    .Select(kvPair => (AOrmObject)AlarmAreas.Singleton.GetById(kvPair.Key))
                    .Where(ormObject => ormObject != null)
                    .OrderBy(ormObject => ormObject.ToString())
                    .ToList();
        }

        private void LoadImplictCCUForAlarmAreas()
        {
            var alarmAreas = AlarmAreas.Singleton.List();

            if (alarmAreas == null)
                return;

            foreach (var alarmArea in alarmAreas)
                LoadImplicitCCUForAlarmArea(alarmArea);
        }

        public void LoadImplicitCCUForAlarmArea(AlarmArea alarmArea)
        {
            if (alarmArea == null)
                return;

            var guidCCU = LoadImplicitCCUForAlarmAreaCore(alarmArea);

            if (guidCCU != Guid.Empty)
            {
                AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

                if (_alarmAreasImplicitCCUAndStates.TryGetValue(
                        alarmArea.IdAlarmArea,
                        out alarmAreaImplicitCCUAndStates) &&
                    alarmAreaImplicitCCUAndStates.GuidCCU == guidCCU)
                    return;

                _alarmAreasImplicitCCUAndStates[alarmArea.IdAlarmArea] =
                    new AlarmAreaImplicitCCUAndStates(
                        alarmArea.IdAlarmArea,
                        guidCCU,
                        null,
                        ActivationState.Unknown,
                        AlarmAreaAlarmState.Unknown);

                return;
            }

            if (!_alarmAreasImplicitCCUAndStates.Remove(alarmArea.IdAlarmArea))
                return;

            var description =
                "Alarm area " + alarmArea + " changed its activation state to " +
                ActivationState.Unknown + ". Alarm area is losing its implitit manager.";

            Eventlogs.Singleton.InsertEvent(
                Eventlog.TYPEALARMAREAACTIVATIONSTATECHANGED,
                _thisAssemblyName,
                new[] { alarmArea.IdAlarmArea },
                description);
        }

        private static Guid LoadImplicitCCUForAlarmAreaCore(AlarmArea alarmArea)
        {
            if (alarmArea == null)
                return Guid.Empty;
            var ccus = new Dictionary<Guid, int>();

            if (alarmArea.AAInputs != null)
                foreach (var aaInput in alarmArea.AAInputs)
                {
                    if (aaInput == null)
                        continue;

                    var ccu = GetCCUFromInput(aaInput.Input);

                    if (ccu == null)
                        continue;

                    var guidCCU = ccu.IdCCU;

                    if (guidCCU == Guid.Empty)
                        continue;

                    if (ccus.ContainsKey(guidCCU))
                        ccus[guidCCU] += 1;
                    else
                        ccus.Add(guidCCU, 1);
                }

            if (alarmArea.AACardReaders != null)
                foreach (var aaCardReader in alarmArea.AACardReaders)
                {
                    if (aaCardReader == null || aaCardReader.CardReader == null)
                        continue;

                    var ccu = GetCCUFromCR(aaCardReader.CardReader);

                    if (ccu == null)
                        continue;

                    var guidCCU = ccu.IdCCU;

                    if (guidCCU == Guid.Empty)
                        continue;

                    if (ccus.ContainsKey(guidCCU))
                        ccus[guidCCU] += 1;
                    else
                        ccus.Add(guidCCU, 1);
                }

            if (alarmArea.ActivationStateInputEIS != null)
            {
                var guidCCU = Inputs.Singleton.GetParentCCU(alarmArea.ActivationStateInputEIS.Value);

                if (guidCCU != Guid.Empty)
                    if (ccus.ContainsKey(guidCCU))
                        ccus[guidCCU] += 1;
                    else
                        ccus.Add(guidCCU, 1);
            }

            if (alarmArea.SetUnsetOutputEIS != null)
            {
                var guidCCU = Outputs.Singleton.GetParentCCU(alarmArea.SetUnsetOutputEIS.Value);

                if (guidCCU != Guid.Empty)
                    if (ccus.ContainsKey(guidCCU))
                        ccus[guidCCU] += 1;
                    else
                        ccus.Add(guidCCU, 1);
            }

            if (ccus.Count <= 0)
                return Guid.Empty;

            var implicitCCUGuid = Guid.Empty;
            var implicitCCUCount = 0;

            foreach (var kvp in ccus)
            {
                if (kvp.Value <= implicitCCUCount)
                    continue;

                implicitCCUGuid = kvp.Key;
                implicitCCUCount = kvp.Value;
            }

            return implicitCCUGuid;
        }

        public bool Unconfigure(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null &&
                ccuSetting.Unconfigure();
        }

        public bool CCUUnconfigure(Guid ccuGuid)
        {
            var retValue = SendToRemotingCCUs(ccuGuid, "Unconfigure");

            return retValue is bool && (bool)retValue;
        }

        private bool StartTFTPServer(Guid ccuGuid)
        {
            return StartTFTPServer(ccuGuid, 0);
        }

        private bool StartTFTPServer(Guid ccuGuid, int blockSize)
        {
            object retValue = blockSize > 0
                ? SendToRemotingCCUs(ccuGuid, false, "StartTFTPServerWithEdit", blockSize)
                : SendToRemotingCCUs(ccuGuid, false, "StartTFTPServer");

            return retValue is bool && (bool)retValue;
        }

        public void StopTFTPServer(Guid ccuGuid)
        {
            SendToRemotingCCUs(ccuGuid, true, "StopTFTPServer");
        }

        public ConfigureResult ConfigureForThisServer(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null
                    ? ccuSetting.ConfigureForThisServer()
                    : ConfigureResult.GeneralFailure;
        }

        public bool CCUConfigureForThisServer(Guid ccuGuid)
        {
            var retValue = SendToRemotingCCUs(ccuGuid, true, "ConfigureForAnotherClient");

            return retValue is bool && (bool)retValue;
        }

        private bool IsCcuConfiguredServerOnline(Guid ccuGuid)
        {
            var retValue = SendToRemotingCCUs(ccuGuid, true, "IsCcuConfiguredServerOnline");

            return retValue is bool && (bool)retValue;
        }

        public ConfigureResult ForceReconfiguration(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null
                    ? ccuSetting.ForceReconfiguration()
                    : ConfigureResult.GeneralFailure;
        }

        public bool SendForceReconfigurationCommand(Guid ccuGuid)
        {
            var retValue = SendToRemotingCCUs(ccuGuid, "ForceReconfiguration");

            return retValue is bool && (bool)retValue;
        }

        public CCUConfigurationState GetCCUConfigureState(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return ccuSetting != null
                ? ccuSetting.ConfigurationState
                : CCUConfigurationState.Unconfigured;
        }

        public CCUConfigurationState GetActualCCUConfiguredState(Guid ccuGuid)
        {
            var result = CCUConfigured(ccuGuid);
            if (result == CCUConfigurationState.ConfiguredForThisServer)
            {
                var ccuVersion = GetCcuFirmwareVersion(ccuGuid);
                if (!IsFirmwareSupported(ccuVersion))
                {
                    result = CCUConfigurationState.ConfiguredForThisServerUpgradeOnly;
                }
                else if (!IsWinCeSupported(
                            ccuVersion,
                            WinCECurrentImageVersion(ccuGuid)))
                {
                    result = CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe;
                }
            }

            return result;
        }

        public bool IsCCUCorrectlyConfiguredForThisServer(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null &&
                ccuSetting.IsCCUCorrectlyConfiguredForThisServer;
        }

        public IPSetting GetIPSettingsFromCCU(Guid guidCCU)
        {
            return SendToRemotingCCUs(guidCCU, true, "GetIPSettings") as IPSetting;
        }

        public IPSetting GetIpSettings(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null
                    ? ccuSetting.IPSetting
                    : null;
        }

        public bool SetIPSettingsToCCU(Guid guidCCU, IPSetting ipSetting)
        {
            if (ipSetting == null)
                return false;

            var retValue = SendToRemotingCCUs(
                guidCCU,
                "SetIPSettings",
                ipSetting.IsDHCP,
                ipSetting.IPAddress,
                ipSetting.SubnetMask,
                ipSetting.Gateway);

            return retValue != null
                ? retValue is bool && (bool)retValue
                : !CheckConnectionToCCU(guidCCU);
        }

        public string SetIpSettings(Guid ccuGuid, IPSetting ipSetting)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return ccuSetting != null
                ? ccuSetting.SetIpSettingsOnTheCCU(ipSetting)
                : string.Empty;
        }

        public bool IsCCU0(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null &&
                ccuSetting.IsCCU0;
        }

        public bool IsCCUUpgrader(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null &&
                ccuSetting.IsCCUUpgrader;
        }

        public bool IsCat12Combo(Guid ccuGuid)
        {
            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);

            return
                ccuSetting != null &&
                ccuSetting.IsCat12Combo;
        }

        private bool HasCat12ComboLicence(Guid ccuGuid)
        {
            try
            {
                var retValue = SendToRemotingCCUs(
                    ccuGuid,
                    true,
                    "HasCat12ComboLicence");

                if (retValue == null)
                    return false;

                return (bool)retValue;
            }
            catch
            {
                return false;
            }
        }

        private void SetCat12ComboLicence(Guid ccuGuid, bool hasComboCredit)
        {
            try
            {
                SendToRemotingCCUs(
                    ccuGuid,
                    false,
                    "SetCat12ComboLicence",
                    hasComboCredit);
            }
            catch
            { }
        }

        private bool CheckConnectionToCCU(Guid guidCCU)
        {
            return SendToRemotingCCUs(guidCCU, true, "GetIsConfigured") is byte;
        }

        internal DateTime? GetCurrentCCUTime(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;

            var retValue = SendToRemotingCCUs(guid, "GetCurrentTime");

            return
                retValue is DateTime
                    ? (DateTime)retValue
                    : (DateTime?)null;
        }

        internal bool StopUnpack(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return false;

            var retValue = SendToRemotingCCUs(guidCCU, true, "StopUpgrade");
            return retValue is bool && (bool)retValue;
        }

        internal int GetDCUUpgradePercentage(Guid guidCCU, byte logicalAddressDCU)
        {
            CCUSettings ccuSetting = GetCCUSettings(guidCCU);

            return ccuSetting != null
                ? ccuSetting.GetDcuUpgradePercentage(logicalAddressDCU)
                : 0;
        }

        internal bool StopUpgradeMode(Guid ccuGuid, bool upgraded)
        {
            if (ccuGuid == Guid.Empty)
                return false;

            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);
            if (ccuSetting == null)
                return false;

            ccuSetting.StopUpgradeMode(upgraded);
            return true;
        }

        public bool StartUpgrade(Guid ccuGuid)
        {
            bool result = StartTFTPServer(ccuGuid);

            if (!result)
                return false;

            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings == null)
                return false;

            ccuSettings.StartUpgradeMode();
            return true;
        }

        internal bool StartUpgradeMode(Guid ccuGuid)
        {
            if (ccuGuid == Guid.Empty)
                return false;

            CCUSettings ccuSetting = GetCCUSettings(ccuGuid);
            if (ccuSetting == null)
                return false;

            ccuSetting.StartUpgradeMode();
            return true;
        }

        public void UpgradeDcus(Guid ccuGuid, string upgradeFile)
        {
            SafeThread<Guid, string>.StartThread(BeginUpgradeDcu, ccuGuid, upgradeFile);
        }

        private void BeginUpgradeDcu(Guid ccuGuid, string fullFileName)
        {
            if (ccuGuid == Guid.Empty || string.IsNullOrEmpty(fullFileName))
                return;

            SendToRemotingCCUs(ccuGuid, false, "UpgradeDCUs", fullFileName);
        }

        public List<string> GetAvailableDCUUpgrades(Guid ccuGuid, string fullDirectoryName)
        {
            if (ccuGuid == Guid.Empty)
                return null;

            var retValue = SendToRemotingCCUs(ccuGuid, false, "GetAvailableDCUUpgrades", fullDirectoryName);

            var value = retValue as string[];

            return value != null
                ? new List<string>(value)
                : null;
        }

        public bool EnsureDirectory(Guid ccuGuid, string fullDirectoryName)
        {
            if (ccuGuid == Guid.Empty || string.IsNullOrEmpty(fullDirectoryName))
                return false;

            var retValue = SendToRemotingCCUs(ccuGuid, false, "EnsureDirectory", fullDirectoryName);
            return retValue is bool && (bool)retValue;
        }

        public bool SetDefaultDCUUpgradeVersion(Guid ccuGuid, string upgradeVersion)
        {
            if (ccuGuid == Guid.Empty || string.IsNullOrEmpty(upgradeVersion))
                return false;

            var retValue = SendToRemotingCCUs(ccuGuid, false, "SetDefaultDCUUpgradeVersion", upgradeVersion);
            return retValue is bool && (bool)retValue;
        }

        public bool ExistsPathOrFile(Guid ccuGuid, string fullName)
        {
            if (ccuGuid == Guid.Empty || string.IsNullOrEmpty(fullName))
                return false;
            var retValue = SendToRemotingCCUs(ccuGuid, false, "ExistsPathOrFile", fullName);
            return retValue is bool && (bool)retValue;
        }

        public bool Reset(Guid ccuGuid)
        {
            if (ccuGuid == Guid.Empty)
                return false;

            var retValue = SendToRemotingCCUs(ccuGuid, false, "Reset");
            return retValue is bool && (bool)retValue;
        }

        public bool SoftReset(Guid ccuGuid)
        {
            if (ccuGuid == Guid.Empty)
                return false;

            var retValue = SendToRemotingCCUs(ccuGuid, false, "SoftReset");
            return retValue is bool && (bool)retValue;
        }

        public int[] CommunicationStatistic(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            return
                ccuSettings != null
                    ? ccuSettings.CommunicationStatistic()
                    : null;
        }

        public void ResetServerSended(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings != null)
                ccuSettings.Client.ResetSended();
        }

        public void ResetServerReceived(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings != null)
                ccuSettings.Client.ResetReceived();
        }

        public void ResetServerDeserializeError(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings != null)
                ccuSettings.Client.ResetSerializationError();
        }

        public void ResetServerMsgRetry(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings != null)
                ccuSettings.Client.ResetSendRetry();
        }

        public void ResetServerReceivedError(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings != null)
                ccuSettings.Client.ResetReceivedError();
        }

        public void ResetCommunicationStatistic(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            if (ccuSettings == null)
                return;

            var client = ccuSettings.Client;

            client.ResetSerializationError();
            client.ResetSended();
            client.ResetReceived();
            client.ResetReceivedError();
            client.ResetSendRetry();
        }

        public int GetCEUpgradeActionResult(Guid ccuGuid)
        {
            CCUSettings ccuSettings = GetCCUSettings(ccuGuid);

            return ccuSettings != null
                ? (int)ccuSettings.CeUpgradeResult
                : (int)ActionResultUpgrade.Unkonwn;
        }

        public bool ResetDcu(Guid ccuGuid, byte dcuLogicalAddress)
        {
            if (ccuGuid == Guid.Empty)
                return false;

            var retValue = SendToRemotingCCUs(ccuGuid, false, "ResetDcu", dcuLogicalAddress);
            return retValue is bool && (bool)retValue;
        }

        public bool DCUUpgradeFileVersionExists(
            Guid ccuGuid,
            string searchInDirectory,
            string selectedVersion,
            out string upgradePackage)
        {
            upgradePackage = string.Empty;

            if (ccuGuid == Guid.Empty)
                return false;

            var retVal =
                SendToRemotingCCUs(
                    ccuGuid,
                    false,
                    "ContainsDCUUpgradePackageVersion",
                    searchInDirectory,
                    selectedVersion);

            upgradePackage = retVal as string ?? string.Empty;

            return upgradePackage != string.Empty;
        }

        #region Communication statistic
        public int[] CcuCommunicationStatistic(Guid idCcu)
        {
            return SendToRemotingCCUs(idCcu, false, "CommunicationStatistic") as int[];
        }

        public void ResetCcuSended(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetSended");
        }

        public void ResetCcuReceived(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetReceived");
        }

        public void ResetCcuDeserializeError(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetDeserializationError");
        }

        public void ResetCcuReceivedError(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetReceivedError");
        }

        public void ResetCcuCommunicationStatistic(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetCommunicationStatistic");
        }

        public void ResetCcuMsgRetry(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetMsgRetry");
        }
        #endregion

        #region CCU start counter

        public object[] GetCcuStartsCount(Guid idCcu)
        {
            var retValue = SendToRemotingCCUs(idCcu, false, "GetCcuStartsCount");
            return retValue as object[];
        }

        public void ResetCcuStartCounter(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "ResetCcuStartCounter");
        }

        #endregion

        public void SendUpsMonitorData(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "SendUpsMonitorData");
        }

        public void StopSendUpsMonitorData(Guid idCcu)
        {
            SendToRemotingCCUs(idCcu, false, "StopSendUpsMonitorData");
        }

        #region Dcu test routine

        public object[] ObtainDcuRunningTestStates(Guid idCcu)
        {
            return SendToRemotingCCUs(idCcu, false, "ObtainDcuRunningTestStates") as object[];
        }

        public void SendDcuRoutinTestConfiguration(Guid idCcu, object data)
        {
            SendToRemotingCCUs(idCcu, false, "SendDcuRoutinTestConfiguration", data);
        }
        #endregion

        public string WinCECurrentImageVersion(Guid idCcu)
        {
            var retValue = SendToRemotingCCUs(idCcu, false, "WinCEImageVersion");
            return
                retValue != null
                    ? (string)retValue
                    : string.Empty;
        }

        public string[] CoprocessorBuildNumberStatistics(Guid idCcu)
        {
            var retValue = SendToRemotingCCUs(idCcu, false, "CoprocessorBuildNumberStatistics");

            return
                retValue != null
                    ? (string[])retValue
                    : new string[1];
        }

        public string ResultSimulationCardSwiped(
            Guid idCcu,
            ObjectType objectType,
            Guid idObject,
            string cardNumber,
            string pin,
            int pinLength)
        {
            var retValue = SendToRemotingCCUs(
                idCcu,
                false,
                "ResultSimulationCardSwiped",
                objectType,
                idObject,
                cardNumber,
                pin,
                pinLength);

            return
                retValue != null
                    ? (string)retValue
                    : string.Empty;
        }

        public void RequestDcuMemoryLoad(Guid idCcu, byte logicalAddress)
        {
            SendToRemotingCCUs(idCcu, false, "RequestDcuMemoryLoad", (object)logicalAddress);
        }

        public void SetCcuNewConfigurePassword(Guid idCcu, string password)
        {
            SendToRemotingCCUs(idCcu, false, "SaveConfigurePassword", (object)password);
        }

        public bool IsSetCcuConfigurationPassword(Guid idCcu)
        {
            var retValue = SendToRemotingCCUs(idCcu, true, "IsSetConfigurePassword");
            return retValue != null && (bool)retValue;
        }

        public bool CompareCcuConfigurationPassword(Guid idCcu, string password)
        {
            var retValue = SendToRemotingCCUs(idCcu, true, "CompareConfigurePassword", (object)password);
            return retValue != null && (bool)retValue;
        }

        public CcuConfigurationOptions DetermineConfigureOption(Guid ccuGuid)
        {
            if (GetCCUSettings(ccuGuid) == null)
                return CcuConfigurationOptions.Disable;

            if (CCUConfigured(ccuGuid) == CCUConfigurationState.Unconfigured)
                return
                    GeneralOptions.Singleton.CcuConfigurationToServerByPassword
                        ? CcuConfigurationOptions.EnableNewPassword
                        : CcuConfigurationOptions.Enable;

            if (IsCcuConfiguredServerOnline(ccuGuid))
                return CcuConfigurationOptions.Disable;

            if (IsSetCcuConfigurationPassword(ccuGuid))
                return
                    GeneralOptions.Singleton.CcuConfigurationToServerByPassword
                        ? CcuConfigurationOptions.EnableVerifyPassword
                        : CcuConfigurationOptions.DiableServerSettingPwd;

            return
                GeneralOptions.Singleton.CcuConfigurationToServerByPassword
                    ? CcuConfigurationOptions.EnableNewPassword
                    : CcuConfigurationOptions.Enable;
        }

        public bool CcuHasSameWinCeImage(Guid ccuGuid, string winCeFile)
        {
            var actCcuWinImageVersion = WinCECurrentImageVersion(ccuGuid);

            var ceUpgradesPath = string.Format("{0}\\{1}",
                QuickPath.AssemblyStartupPath,
                NCASServer.CE_UPGRADES_DIRECTORY_NAME);

            var actFileWinImageVersion = GetImageVersionFromFile(
                Directory
                    .GetFiles(ceUpgradesPath)
                    .FirstOrDefault(fullFileName =>
                        Path.GetFileName(fullFileName) == winCeFile)
                            ?? string.Empty);

            return actCcuWinImageVersion == actFileWinImageVersion;
        }

        private const int WinceImageFooterLength = 64;

        private static string GetImageVersionFromFile(string fileName)
        {
            FileStream fs = null;

            try
            {
                if (!File.Exists(fileName))
                    return string.Empty;

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                var footerInfo = new byte[WinceImageFooterLength];
                /* Seek to the footer at the end of the image and read its value */
                fs.Seek(-WinceImageFooterLength, SeekOrigin.End);
                fs.Read(footerInfo, 0, WinceImageFooterLength);
                /* Parse footer info */
                var footerString = Encoding.UTF8.GetString(footerInfo, 0, WinceImageFooterLength - 4);
                var subStr = footerString.Split(',');
                return subStr.Length > 1 ? subStr[1].Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (fs != null)
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                    }
            }
        }

        public bool RegisterCRsForUpgradeVersion(
            string ccuIpAddress,
            Dictionary<byte, byte[]> dcusAndCardReaders,
            string upgradeVersion)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIpAddress);
            if (ccu == null)
                return true;

            foreach (var dcuAndCardReaders in dcusAndCardReaders)
            {
                object retValue =
                    SendToRemotingCCUs(
                        ccu.IdCCU,
                        "RegisterCRsForUpgradeVersion",
                        dcuAndCardReaders.Key,
                        dcuAndCardReaders.Value,
                        upgradeVersion);

                if (retValue is bool && !(bool)retValue)
                    return false;
            }

            return true;
        }

        public bool RegisterCCUsCRsForUpgradeVersion(
            string ccuIpAddress,
            Guid[] cardReaders,
            string upgradeVersion)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIpAddress);
            if (ccu == null)
                return true;

            var retValue = SendToRemotingCCUs(ccu.IdCCU, "RegisterCCUsCRsForUpgradeVersion", cardReaders, upgradeVersion);
            return !(retValue is bool) || (bool)retValue;
        }

        public bool EnqueueCcuDataBatch(Guid guidCCU, CcuDataBatch ccuDataBatch)
        {
            Console.WriteLine("EnqueueCcuDataBatch method CCU guid: " + guidCCU);
            Console.WriteLine("Remoting CCUs count: " + _remotingCCUs.Count);

            CCUSettings ccuSettings = GetCCUSettings(guidCCU);

            if (ccuSettings == null)
                return false;

            Console.WriteLine("Get value from remotingCCUs");

            try
            {
                Console.WriteLine("Value is not null " + ccuSettings.GuidCCU);
                ccuSettings.EnqueueCcuDataBatch(ccuDataBatch);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EnqueueCcuDataBatch exception :" + ex.Message);
                return false;
            }
            Console.WriteLine("after CCU settings");
            return true;
        }

        private readonly Dictionary<string, Guid> _ccusIpAddress = new Dictionary<string, Guid>();
        public void ChangeCCUIpAddress(string oldIpAddress, string newIpAddress, Guid idCCU)
        {
            lock (_ccusIpAddress)
            {
                if (!string.IsNullOrEmpty(oldIpAddress) && _ccusIpAddress.ContainsKey(oldIpAddress))
                    _ccusIpAddress.Remove(oldIpAddress);

                if (string.IsNullOrEmpty(newIpAddress))
                    return;

                if (_ccusIpAddress.ContainsKey(newIpAddress))
                    _ccusIpAddress.Remove(newIpAddress);

                _ccusIpAddress.Add(newIpAddress, idCCU);
            }
        }

        public Guid GetIdCCUFromIpAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return Guid.Empty;

            Guid guidCCU;
            return
                _ccusIpAddress.TryGetValue(ipAddress, out guidCCU)
                    ? guidCCU
                    : Guid.Empty;
        }

        public State GetTimeZonesDailyPlansStateFromCCU(Guid idCCU, ObjectType objectType, Guid objectGuid)
        {
            var retValue =
                SendToRemotingCCUs(
                    idCCU,
                    "GetTimeZonesDailyPlansState",
                    (byte)objectType,
                    objectGuid);

            return
                retValue is byte
                    ? (State)retValue
                    : State.Unknown;
        }

        public bool SendBindingsToCCU(Guid idCCU, byte[] bindings)
        {
            var retValue = SendToRemotingCCUs(idCCU, "SendBindings", bindings);
            return retValue is bool && (bool)retValue;
        }

        public void CcuUpsValuesChanged(string ccuIpAddress, CUps2750Values upsValues)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCcuUpsValuesChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { ccuIpAddress, upsValues });
            }
            catch
            {
            }
        }

        public void CcuUpsOnlineStateChanged(string ccuIpAddress, byte upsOnline)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCcuUpsOnlineStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { ccuIpAddress, upsOnline });
            }
            catch { }
        }

        public void CcuUpsAlarmStateChanged(string ccuIpAddress, bool upsAlarm)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCcuUpsAlarmStateChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { ccuIpAddress, upsAlarm });
            }
            catch { }
        }


        public bool GetOtherCCUStatistics(Guid ccuId,
            out int threadsCount,
            out int flashFreeSpace, out int flashSize,
            out bool sdCardPresent, out int sdCardFreeSpace, out int sdCardSize,
            out int freeMemory, out int totalMemory, out int memoryLoad)
        {
            threadsCount = -1;
            flashFreeSpace = -1;
            flashSize = -1;
            sdCardPresent = true;
            sdCardFreeSpace = -1;
            sdCardSize = -1;
            freeMemory = -1;
            memoryLoad = -1;
            totalMemory = -1;

            if (ccuId == Guid.Empty)
                return false;

            var statistics = SendToRemotingCCUs(ccuId, "GetOtherStatistics") as int[];

            if (statistics == null)
                return false;

            switch (statistics.Length)
            {
                case 5:
                    threadsCount = statistics[0];
                    flashFreeSpace = statistics[1] / 1000;
                    freeMemory = statistics[2];
                    memoryLoad = statistics[3];
                    totalMemory = statistics[4];
                    return true;
                case 9:
                    threadsCount = statistics[0];
                    flashFreeSpace = statistics[1];
                    flashSize = statistics[2];
                    sdCardPresent = statistics[3] > 0;
                    sdCardFreeSpace = statistics[4];
                    sdCardSize = statistics[5];
                    freeMemory = statistics[6];
                    totalMemory = statistics[7];
                    memoryLoad = statistics[8];
                    return true;
            }

            return false;
        }

        public bool ResetCardReader(
            Guid guidCCU,
            Guid guidCardReader)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "ResetCardReader", guidCardReader);
            return retValue is bool && (bool)retValue;
        }

        public bool SetTimeManually(Guid guidCCU, DateTime utcDateTime)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "SetTimeManually", utcDateTime);
            return retValue is bool && (bool)retValue;
        }

        public bool RemoveCCUEvents(Guid guidCCU)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "DeleteAllEvents");

            if (!(retValue is bool) || !(bool)retValue)
                return false;

            CCUSettings ccuSettings = GetCCUSettings(guidCCU);

            if (ccuSettings != null)
                ccuSettings.CCUEvents.ClearAllEventsAndTransfers();

            return true;
        }

        public BlockFileInfo[] GetBlockFilesInfo(Guid guidCCU)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "GetAllBlockFilesInfo");

            var objectArray = retValue as object[];
            return objectArray != null
                ? objectArray
                    .OfType<BlockFileInfo>()
                    .ToArray()
                : null;
        }

        public bool ResetBlockFilesInfo(Guid guidCCU)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "ResetBlockFilesInfo");
            return retValue is bool && (bool)retValue;
        }

        public void SetEnableLoggingSDPSTZChanges()
        {
            lock (_remotingCCUs)
                foreach (CCUSettings ccuSettings in CCUSettings)
                    ccuSettings.SetEnableLoggingSDPSTZChanges();
        }

        public object SetEnableLoggingSDPSTZChanges(
            Guid guidCCU,
            bool enableLoggingSDPSTZChanges)
        {
            return SendToRemotingCCUs(
                guidCCU,
                "SetEnableLoggingSDPSTZChanges",
                enableLoggingSDPSTZChanges);
        }

        public void SetAlarmAreaRestrictivePolicyForTimeBuying()
        {
            lock (_remotingCCUs)
                foreach (CCUSettings ccuSettings in CCUSettings)
                    ccuSettings.SetAlarmAreaRestrictivePolicyForTimeBuying();
        }

        public object SetAlarmAreaRestrictivePolicyForTimeBuying(
            Guid guidCCU,
            bool alarmAreaRestrictivePolicyForTimeBuying)
        {
            return SendToRemotingCCUs(
                guidCCU,
                "SetAlarmAreaRestrictivePolicyForTimeBuying",
                alarmAreaRestrictivePolicyForTimeBuying);
        }

        /// <summary>
        /// Send for all CCUs whether CCU time is synchronized from the server
        /// </summary>
        public void SetTimeSyncingFromServer()
        {
            lock (_remotingCCUs)
                foreach (CCUSettings ccuSettings in _remotingCCUs.Values)
                    if (ccuSettings != null)
                        ccuSettings.SetTimeSyncingFromServer();
        }

        /// <summary>
        /// Start or stop sending time to the CCU according to the time synchronization settings
        /// </summary>
        /// <param name="guidCCU"></param>
        public void StartStopSendingTimeFromServer(Guid guidCCU)
        {

            lock (_remotingCCUs)
            {
                CCUSettings ccuSettings = GetCCUSettings(guidCCU);

                if (ccuSettings != null)
                    ccuSettings.StartStopSendingTimeFromServer();
            }
        }

        /// <summary>
        /// Send whether CCU time is synchronized from the server
        /// </summary>
        /// <param name="guidCCU"></param>
        /// <param name="syncingTimeFromServer"></param>
        /// <returns></returns>
        public void SetTimeSyncingFromServer(
            Guid guidCCU,
            bool syncingTimeFromServer)
        {
            SendToRemotingCCUs(
                guidCCU,
                "SetTimeSyncingFromServer",
                syncingTimeFromServer);
        }

        /// <summary>
        /// Send actual time to the CCU for syncing
        /// </summary>
        /// <param name="guidCCU"></param>
        /// <param name="actDateTime"></param>
        /// <param name="periodicTimeSyncTolerance"></param>
        /// <returns></returns>
        public void SetTimeFromServer(
            Guid guidCCU,
            DateTime actDateTime,
            int periodicTimeSyncTolerance)
        {
            SendToRemotingCCUs(
                guidCCU,
                "SetTimeFromServer",
                actDateTime,
                periodicTimeSyncTolerance);
        }

        /// <summary>
        /// Gets information about threads running on CCU
        /// </summary>
        /// <param name="guidCCU">CCU id</param>
        /// <returns></returns>
        public IList<ThreadInfo> GetThreadsInfo(Guid guidCCU)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "GetThreadsInfo");

            var collection = retValue as ICollection;
            var result =
                collection != null
                    ? collection.OfType<ThreadInfo>()
                    : Enumerable.Empty<ThreadInfo>();

            return result.ToList();
        }

        private static readonly VerbosityLevelInfo[] DefaultCcuVerbosityLevelsInfo
            =
            {
                new VerbosityLevelInfo("Calm", 0),
                new VerbosityLevelInfo("Performance", 32),
                new VerbosityLevelInfo("Low", 64),
                new VerbosityLevelInfo("BelowNormal", 96),
                new VerbosityLevelInfo("Normal", 128),
                new VerbosityLevelInfo("AboveNormal", 160),
                new VerbosityLevelInfo("High", 192),
                new VerbosityLevelInfo("Debug", 224),
                new VerbosityLevelInfo("Highest", 255)
            };

        /// <summary>
        /// Gets information about predefined Log verbosity levels
        /// </summary>
        /// <param name="guidCCU">CCU id</param>
        /// <returns>Collection od VerbosityLevelInfo objects </returns>
        public IList<VerbosityLevelInfo> GetVerbosityLevelsInfo(Guid guidCCU)
        {
            return DefaultCcuVerbosityLevelsInfo;
        }

        /// <summary>
        /// Sets Log verbosity level
        /// </summary>
        /// <param name="guidCCU">CCU id</param>
        /// <param name="verbosityLevel">level</param>
        /// <returns>result of action</returns>
        public bool SetVerbosityLevel(Guid guidCCU, byte verbosityLevel)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "SetVerbosityLevel", verbosityLevel);
            return retValue is bool && (bool)retValue;
        }

        /// <summary>
        /// Gets CCU Log verbosity level
        /// </summary>
        /// <param name="guidCCU">CCU id</param>
        /// <returns>CCU log verbosity</returns>
        public byte GetVerbosityLevel(Guid guidCCU)
        {
            var retValue = SendToRemotingCCUs(guidCCU, "GetVerbosityLevel");

            return
                retValue is byte
                    ? (byte)retValue
                    : (byte)128;
        }

        /// <summary>
        /// Handles failed data channel transfers
        /// </summary>
        /// <param name="ccu">CCU, where the data were sent to</param>
        /// <param name="guidCCU">use if CCU object can not be red from database (alternative to CCU parameter)</param>
        /// <param name="streamName">name of stream</param>
        /// <param name="description">description</param>
        /// <param name="parameters"></param>
        public void TCPTransferFailed(
            CCU ccu,
            Guid guidCCU,
            string streamName,
            string description,
            params EventlogParameter[] parameters)
        {
            if (GetCCUState(guidCCU) == CCUOnlineState.Online)
            {
                CCUAlarms.CreateDataChannelTransferAlarm(
                    ccu,
                    streamName,
                    description);
            }
            else
            {
                CCUAlarms.StopDataChannelTransferAlarm(
                    guidCCU,
                    streamName);
            }

            if (parameters == null || parameters.Length == 0)
                parameters =
                    new[]
                    {
                        new EventlogParameter(
                            EventlogParameter.TYPECCU,
                            ccu != null
                                ? ccu.IPAddress
                                : string.Empty,
                            guidCCU,
                            ObjectType.CCU),
                        new EventlogParameter(
                            EventlogParameter.TYPE_STREAM_NAME,
                            streamName)
                    };

            Eventlogs.Singleton.InsertEvent(
                Eventlog.TYPE_CCU_DATA_CHANNEL_TRANSFER_FAILED,
                DateTime.Now,
                _thisAssemblyName,
                new[] { guidCCU },
                description,
                parameters);
        }

        /// <summary>
        /// Handles successfull transfers
        /// </summary>
        /// <param name="idCcu">use if CCU object can not be red from database (alternative to CCU parameter)</param>
        /// <param name="streamName">name of stream</param>
        public void TCPTransferSucceeded(Guid idCcu, string streamName)
        {
            CCUAlarms.StopDataChannelTransferAlarm(
                idCcu,
                streamName);
        }

        /// <summary>
        /// Resets command timeout counter on CCU
        /// </summary>
        /// <param name="guidCCU">CCU id</param>
        public void ResetCCUCommandTimeouts(Guid guidCCU)
        {
            SendToRemotingCCUs(guidCCU, false, "ResetCCUCommandTimeouts");
        }

        /// <summary>
        /// Set allow PIN caching in card reader menu
        /// </summary>
        /// <param name="guidCCU"></param>
        public void SetAllowPINCachingInCardReaderMenu(Guid guidCCU)
        {
            SendToRemotingCCUs(
                guidCCU,
                "SetAllowPINCachingInCardReaderMenu",
                GeneralOptions.Singleton.CardReadersAllowPINCachingInMenu);
        }

        public void SetAllowCodeLength(Guid guidCCU)
        {
            SendToRemotingCCUs(
                guidCCU,
                "SetAllowCodeLength",
                GeneralOptions.Singleton.MinimalCodeLength,
                GeneralOptions.Singleton.MaximalCodeLength);
        }

        public void SetPinConfirmationObligatory(Guid guidCCU)
        {
            SendToRemotingCCUs(
                guidCCU,
                "SetPinConfirmationObligatory",
                GeneralOptions.Singleton.IsPinConfirmationObligatory);
        }

        private IEnumerable<CCUSettings> CCUSettings
        {
            get
            {
                return _remotingCCUs.Values.Where(ccuSettings => ccuSettings != null);
            }
        }
        /// <summary>
        /// Set allow PIN caching in card reader menu for the all configured CCUs
        /// </summary>
        public void SetAllowPINCachingInCardReaderMenu()
        {
            lock (_remotingCCUs)
                foreach (var ccuSettings in CCUSettings)
                    ccuSettings.SetAllowPINCachingInCardReaderMenu();
        }

        public void SetAllowCodeLength()
        {
            lock (_remotingCCUs)
                foreach (var ccuSettings in CCUSettings)
                    ccuSettings.SetAllowCodeLength();
        }

        public void SetPinConfirmationObligatory()
        {
            lock (_remotingCCUs)
                foreach (var ccuSettings in CCUSettings)
                    ccuSettings.SetPinConfirmationObligatory();
        }

        public void SendModifyObjectsToCCUsAsync(Guid objectGuid, ObjectType objectType)
        {
            var ormTable = CgpServerRemotingProvider.Singleton.GetTableOrmForObjectType(objectType);

            if (ormTable == null)
                return;

            var ormObject = ormTable.GetObjectById(objectGuid) as AOrmObjectWithVersion;

            if (ormObject == null)
                return;

            SafeThread<AOrmObjectWithVersion>.StartThread(
                DataReplicationManager.Singleton.SendModifiedObjectToCcus,
                ormObject);
        }

        public bool RunGcCollect(Guid idCcu)
        {
            object retValue = SendToRemotingCCUs(
                idCcu,
                "RunGcCollect");

            return retValue is bool && (bool)retValue;
        }

        public void AlarmAreaTimeBuyingFailed(
            Guid guidAlarmArea,
            Guid guidLogin,
            byte reason,
            int timeToBuy,
            int remainingTime)
        {
            AlarmAreaImplicitCCUAndStates alarmAreaImplicitCCUAndStates;

            if (_alarmAreasImplicitCCUAndStates.TryGetValue(
                        guidAlarmArea,
                        out alarmAreaImplicitCCUAndStates) &&
                    alarmAreaImplicitCCUAndStates != null)
                alarmAreaImplicitCCUAndStates
                    .SetAlarmAreaRequestActivationState(
                        RequestActivationState.Unknown);

            /*Card card = null;

            foreach (var source in (Guid[])processor._parameters[5])
            {
                card = Cards.Singleton.GetById(source);
                if (card != null)
                    break;
            }*/

            if (guidLogin != Guid.Empty)
            {
                _queueAlarmAreaRequestActivationStateChanged.Enqueue(
                    new ActivationStateChangedCarrier(
                        () => NCASServerRemotingProvider.Singleton.SingleCallback(
                            guidLogin.ToString(),
                            typeof(AlarmAreaTimeBuyingHandler),
                            CCUCallbackRunner.RunAlarmAreaTimeBuyingFailed,
                            DelegateSequenceBlockingMode.OverallBlocking,
                            new object[]
                            {
                                guidAlarmArea,          //Guid guidAlarmArea 
                                reason,                 //byte reason
                                timeToBuy,              //int timeToBuy
                                remainingTime           //int remainingTime
                            })));

            }
        }

        public void RemoveCardsFromAntiPassbackZone(
            Guid guidCcu,
            Guid guidAntiPassBackZone,
            Guid[] guidCards)
        {
            SendToRemotingCCUs(
                guidCcu,
                "RemoveCardsFromAntiPassBackZone",
                guidAntiPassBackZone,
                guidCards);
        }

        public ICollection<CCUCardInAntiPassBackZone> AddCardsToAntiPassbackZone(
            Guid guidCcu,
            Guid guidAntiPassBackZone,
            Guid[] idCards)
        {
            var result =
                SendToRemotingCCUs(
                    guidCcu,
                    "AddCardsToAntiPassBackZone",
                    guidAntiPassBackZone,
                    idCards) as object[];

            return
                result != null
                    ? new LinkedList<CCUCardInAntiPassBackZone>(
                        result.OfType<CCUCardInAntiPassBackZone>())
                    : null;
        }

        public ICollection<CCUCardInAntiPassBackZone> GetCardsInZone(
            Guid guidCcu,
            Guid guidAntiPassBackZone)
        {
            var result =
                SendToRemotingCCUs(
                    guidCcu,
                    "GetCardsInZone",
                    guidAntiPassBackZone) as object[];

            return
                result != null
                    ? new LinkedList<CCUCardInAntiPassBackZone>(
                        result.OfType<CCUCardInAntiPassBackZone>())
                    : null;
        }

        public ICollection<Alarm> GetAlarms(
            Guid idCcu,
            ICollection<AlarmIndividualBlockingChangeInPending> individualBlockingChangesInPending,
            ICollection<AlarmAcknowledgeInPending> alarmsAcknowledgeInPending)
        {
            var retValue = SendToRemotingCCUs(
                idCcu,
                "GetAlarms",
                individualBlockingChangesInPending,
                alarmsAcknowledgeInPending);

            return retValue as ICollection<Alarm>;
        }

        public bool? ProcessAlarmEvent(
            Guid idCcu,
            AlarmEvent alarmEvent)
        {
            var retValue = SendToRemotingCCUs(
                idCcu,
                "ProcessAlarmEvent",
                alarmEvent);

            return retValue != null
                ? (bool)retValue
                : (bool?)null;
        }

        private void SaveAlarmEvent(object[] inputParts)
        {
            if (inputParts == null || inputParts.Length != 2)
                return;

            var ccuIpAddress = inputParts[1] as string;
            if (ccuIpAddress == null)
                return;

            var actIdCcu = GetIdCCUFromIpAddress(ccuIpAddress);
            if (actIdCcu == Guid.Empty)
                return;

            try
            {
                var alarmEvents = inputParts[0] as ICollection<AlarmEvent>;

                if (alarmEvents == null)
                    return;

                foreach (var alarmEvent in alarmEvents)
                {
                    alarmEvent.ProcessEvent(actIdCcu);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public bool GetBlockedState(Guid idCardReader)
        {
            CardReaderState cardReaderState =
                GetCardReaderState(idCardReader);

            return
                cardReaderState != null
                && cardReaderState.IsBlocked;
        }

        public void UnblockCardReader(
            Guid idCcu,
            Guid idCardReader)
        {
            SendToRemotingCCUs(
                idCcu,
                "UnblockCardReader",
                idCardReader);
        }

        public bool GetDebugFiles(Guid idCcu)
        {
            lock (_getDebugFilesLock)
            {
                if (_loadingCcuDebugFiles)
                    return false;

                _loadingCcuDebugFiles = true;

                SafeThread.StartThread(
                    new Action(
                        () => MakeLogDump(idCcu)));

                return true;
            }
        }

        private void MakeLogDump(Guid idCcu)
        {
            var ccu = GetCCUSettings(idCcu);

            if (ccu == null)
            {
                MakeLogDumpFailed(idCcu);
                return;
            }

            var debugFiles = SendToRemotingCCUs(idCcu, false, "GetDebugFiles", null) as string[];

            if (debugFiles == null)
            {
                MakeLogDumpFailed(idCcu);
                return;
            }

            string fileDirectory = string.Format(
                "{0}{1}\\",
                CCU_DUMPS_DIRECTORY,
                ccu.IPAddress);

            if (Directory.Exists(fileDirectory))
            {
                foreach (var file in Directory.GetFiles(fileDirectory))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception)
                    {
                        MakeLogDumpFailed(idCcu);
                        return;
                    }
                }
            }
            else
                new FileInfo(fileDirectory).Directory.Create();

            _ccuDebugFileReceivedCounter = 0;
            _ccuDebugFilesCount = debugFiles.Length;

            foreach (var file in debugFiles)
            {
                _dataChannel.Pull(
                    new IPEndPoint(
                        ccu.IPAddress,
                        NCASConstants.TCP_DATA_CHANNEL_PORT),
                    file,
                    false,
                    string.Format(
                        "{0}{1}",
                        fileDirectory,
                        Path.GetFileName(file)));

                //Console.WriteLine("Send request for debug file: " + file);

                if (!_pullDataChannelPeerAutoResetEvent.WaitOne(10000))
                    break;
            }

            if (_ccuDebugFileReceivedCounter != _ccuDebugFilesCount)
            {
                MakeLogDumpFailed(idCcu);
                return;
            }

            string zipFilePath = string.Format(
                "{0}{1}.zip",
                CCU_DUMPS_DIRECTORY,
                ccu.IPAddress);

            try
            {
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                using (var zip = new ZipFile())
                {
                    zip.AddDirectory(fileDirectory);
                    zip.Save(string.Format(
                        "{0}{1}.zip",
                        CCU_DUMPS_DIRECTORY,
                        ccu.IPAddress));
                }
            }
            catch
            {
                MakeLogDumpFailed(idCcu);
                return;
            }

            _loadingCcuDebugFiles = false;
        }

        private void MakeLogDumpFailed(Guid idCcu)
        {
            _loadingCcuDebugFiles = false;
            
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunCCUMakeLogDumpProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[] { idCcu, -1 });
        }

        public void AlarmAreaTimeBuingMatrixStateChanged(
            Guid idObject,
            TimeBuyingMatrixState timeBuyingMatrixState,
            DateTime dateTime)
        {
            var timeBuyingMatrixValue = new TimeBuyingMatrixValue(dateTime, timeBuyingMatrixState);

            if (!_timeBuyingMatrixValues.ContainsKey(idObject))
                _timeBuyingMatrixValues.Add(idObject, timeBuyingMatrixValue);
            else if (_timeBuyingMatrixValues[idObject].DateTime < timeBuyingMatrixValue.DateTime)
                _timeBuyingMatrixValues[idObject] = timeBuyingMatrixValue;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunTimeBuyingMatrixStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    idObject,
                    timeBuyingMatrixState
                });
        }

        public TimeBuyingMatrixState? GetTimeBuyingMatrixState(Guid idAlarmArea)
        {
            if (!_timeBuyingMatrixValues.ContainsKey(idAlarmArea))
                return null;

            return _timeBuyingMatrixValues[idAlarmArea].TimeBuyingMatrixState;
        }

        private class TimeBuyingMatrixValue
        {
            public DateTime DateTime { get; private set; }
            public TimeBuyingMatrixState TimeBuyingMatrixState { get; private set; }

            public TimeBuyingMatrixValue(
                DateTime dateTime,
                TimeBuyingMatrixState timeBuyingMatrixState)
            {
                DateTime = dateTime;
                TimeBuyingMatrixState = timeBuyingMatrixState;
            }
        }

        public ICollection<MemoryInfo> GetMemoryReport(Guid idCcu)
        {
            var result = SendToRemotingCCUs(
                idCcu,
                "GetMemoryReport");

            return result as ICollection<MemoryInfo>;
        }

        public bool StopCKM(Guid idCcu)
        {
            var result = SendToRemotingCCUs(
                idCcu,
                "StopCKM");

            return (bool)result;
        }

        public bool StartCKM(Guid idCcu, bool isImplicity)
        {
            var result = SendToRemotingCCUs(
                idCcu,
                "StartCKM",
                isImplicity);

            return (bool)result;
        }

        public bool RunProccess(Guid idCcu, string cmd)
        {
            var result = SendToRemotingCCUs(
                idCcu,
                "RunProccess",
                cmd);

            return (bool)result;
        }
    }
}
