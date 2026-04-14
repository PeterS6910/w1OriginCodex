using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.LwSerialization;
using Contal.IwQuick.Data;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class SendEventsToServerDispatcher 
        : ASingleton<SendEventsToServerDispatcher>
        , IEventDispatcher
    {
        private class AutonomousRunStateChangeExecutor : IProcessingQueueRequest
        {
            private readonly bool _isAutonomousRun;

            public AutonomousRunStateChangeExecutor(bool isAutonomousRun)
            {
                _isAutonomousRun = isAutonomousRun;
            }

            public void Execute()
            {
                lock (Singleton._lockEvents)
                {
                    if (Singleton._isAutonomousRun == _isAutonomousRun)
                        return;

                    Singleton._isAutonomousRun = _isAutonomousRun;

                    if (Singleton._isAutonomousRun)
                    {
                        Singleton._notAcknowledgedEventsCount = 0;
                        Singleton._sentAutonomousEventsIds.Clear();

                        Singleton.SaveAutonomousEventsToDatabase(
                            Singleton._eventsForSavingToDatabaseByEventId.Values);

                        Singleton._eventsForSavingToDatabaseByEventId.Clear();

                        Singleton.SaveAutonomousEventsToDatabase(
                            Singleton._eventsByEventId.Values);

                        Singleton._eventsByEventId.Clear();

                        return;
                    }
                }

                Singleton.SendAutonomousEventsToServer();
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private class EventParametersRequest
            : IProcessingQueueRequest
        {
            private readonly EventParameters.EventParameters _eventParameters;

            public EventParametersRequest(EventParameters.EventParameters eventParameters)
            {
                _eventParameters = eventParameters;
            }

            public void Execute()
            {
                lock (Singleton._lockEvents)
                {
                    if (Singleton.SaveEventInAutonomouseMode(_eventParameters))
                        return;

                    Singleton._sendToServerBatchWorker.Add(
                        _eventParameters,
                        _eventParameters.Priority);

                    Singleton._eventsByEventId.Add(
                        _eventParameters.EventId,
                        _eventParameters);

                    Singleton._notAcknowledgedEventsCount++;
                }
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private class SendToServerBatchExecutor : IBatchExecutor<EventParameters.EventParameters>
        {
            public int Execute(ICollection<EventParameters.EventParameters> requests)
            {
                var eventsParametersToSend = new LinkedList<EventParameters.EventParameters>(requests);

                return CcuCoreRemotingProvider.Singleton.DoSaveEvent(eventsParametersToSend)
                    ? eventsParametersToSend.Count
                    : 0;
            }
        }

        private const string OLD_EVENT_SETTINGS_FILE_NAME = @"OldEventsSettings.dat";

        
        private const int SaveEventToDatabaseDelay = 5*60*1000;
        private const int MAX_EVENTS_COUNT = 100;
        private const int DELAY_BETWEEN_SENDING = 200;
        private const int AutonomousEventsBatchSize = 500;

        private readonly object _lockEvents = new object();

        private Dictionary<UInt64, EventParameters.EventParameters> _eventsByEventId =
            new Dictionary<UInt64, EventParameters.EventParameters>();

        private Dictionary<UInt64, EventParameters.EventParameters> _eventsForSavingToDatabaseByEventId =
            new Dictionary<UInt64, EventParameters.EventParameters>();

        private readonly BatchWorkerWithPriority<EventParameters.EventParameters> _sendToServerBatchWorker;


        private readonly ThreadPoolQueueWithPriority<EventParametersRequest> _queueSaveEvent;
        private readonly OldEventsSettings _oldEventsSettings = new OldEventsSettings();

        private readonly ThreadPoolQueue<DeleteEventsExecutor> _queueDeleteEvent;

        private int _notAcknowledgedEventsCount;
        private bool _isAutonomousRun = true;

        private readonly SafeThread _safeThreadSaveToFileOldEventsSettings;
        private readonly string _directoryForOldEventsSettings = CcuCore.Singleton.RootPath + CcuCore.TEMP;

        private readonly ThreadPoolQueue<AutonomousRunStateChangeExecutor> _queueSetAutonomousRun;
        
        private readonly Dictionary<EventType, Func<bool>> _sendToOldEventsByEventType =
            new Dictionary<EventType, Func<bool>>();

        private readonly Dictionary<EventType, Func<uint>> _priorityByEventType =
            new Dictionary<EventType, Func<uint>>();

        private readonly ExclusiveFilterForEventTypes _exclusiveFilterForEventTypes;

        private readonly HashSet<UInt64> _sentAutonomousEventsIds;

        private bool _existsAutonomousEvents = true;

        private SendEventsToServerDispatcher() : base(null)
        {
            _exclusiveFilterForEventTypes = new ExclusiveFilterForEventTypes(
                EventType.AlarmAreaAlarmStateChangeAlarm,
                EventType.AlarmAreaSensorAlarmAcknowledged,
                EventType.AlarmAreaSensorStateChangedOnlyForCardReader);

            FillSaveToOldEventsByEventType();
            FillPriorityByEventType();

            CcuCore.Singleton.BeforeExit += BeforeExit;

            _queueSaveEvent = new ThreadPoolQueueWithPriority<EventParametersRequest>(ThreadPoolGetter.Get());

            _queueDeleteEvent =
                new ThreadPoolQueue<DeleteEventsExecutor>(ThreadPoolGetter.Get());

            _sendToServerBatchWorker = new BatchWorkerWithPriority<EventParameters.EventParameters>(
                new SendToServerBatchExecutor(),
                DELAY_BETWEEN_SENDING,
                MAX_EVENTS_COUNT);

            TimerManager.Static.StartTimer(
                SaveEventToDatabaseDelay,
                false,
                OnTimerSaveEventToDatabase);

            _safeThreadSaveToFileOldEventsSettings = new SafeThread(SaveOldEventsSettingsToFile);

            _queueSetAutonomousRun =
                new ThreadPoolQueue<AutonomousRunStateChangeExecutor>(
                    ThreadPoolGetter.Get());

            _sentAutonomousEventsIds = new HashSet<UInt64>();
        }

        private bool OnTimerSaveEventToDatabase(TimerCarrier timer)
        {
            lock (_lockEvents)
            {
                if (_eventsForSavingToDatabaseByEventId.Count > 0)
                {
                    SaveAutonomousEventsToDatabase(_eventsForSavingToDatabaseByEventId.Values);
                    _notAcknowledgedEventsCount -= _eventsForSavingToDatabaseByEventId.Count;

                    _eventsForSavingToDatabaseByEventId.Clear();
                }

                if (_eventsByEventId.Count == 0)
                    return true;

                var eventsByEventId = _eventsByEventId;
                _eventsByEventId = _eventsForSavingToDatabaseByEventId;
                _eventsForSavingToDatabaseByEventId = eventsByEventId;

                return true;
            }
        }

        private int _count;
        private const int LimitForCheckSpace = 1000;
        private const int LimitForFreeSpace = 1000 * 2000 * 20;

        /// <summary>
        /// Return true if old event of specific event type must be stored in the queue
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private bool AutonomousSavingEnabledForObjectType(EventType eventType)
        {
            Func<bool> saveToOldEvents;

            return
                !_sendToOldEventsByEventType.TryGetValue(
                    eventType,
                    out saveToOldEvents)
                || saveToOldEvents();
        }

        private void SaveAutonomousEventsToDatabase(ICollection<EventParameters.EventParameters> eventsParameters)
        {
            foreach (var eventParameters in eventsParameters)
                eventParameters.Priority = PRIORITY_FOR_EVENTS_FROM_AUTONOMOUS_RUN;

            EnsureEnoughFreeSpaceForAutonomousEvents(eventsParameters.Count);

            if (Database.AutonomousEventsEngine.SqlCeDbAutonomousEventsAcessor.InsertIntoDatabase(
                eventsParameters.Where(
                    eventParameters => AutonomousSavingEnabledForObjectType(eventParameters.EventType))) > 0)
            {
                _existsAutonomousEvents = true;
            }
        }

        private void EnsureEnoughFreeSpaceForAutonomousEvents(int count)
        {
            _count += count;

            if (_count >= LimitForCheckSpace)
            {
                _count = 0;

                if (IsEnoughFreeSpaceForAutonomousEvents())
                    return;

                var deletedEventsCount = LimitForCheckSpace*2;

                if (count > deletedEventsCount)
                    deletedEventsCount = count;

                Database.AutonomousEventsEngine.SqlCeDbAutonomousEventsAcessor.DeleteOldestFromDatabase(
                    deletedEventsCount);

                Database.AutonomousEventsEngine.ShrinkDatabase();
            }
        }

        private bool IsEnoughFreeSpaceForAutonomousEvents()
        {
            UInt64 freeSpace;

            var sdCardInfo = new SdCardInfo();
            if (sdCardInfo.Present)
            {
                freeSpace = sdCardInfo.FreeSpace;
            }
            else
            {
                var nandFlashInfo = new NandFlashInfo();
                freeSpace = nandFlashInfo.FreeBytesAvailable;
            }

            return freeSpace > LimitForFreeSpace;
        }

        public void SaveEventTest()
        {
            ICollection<Guid> inputs = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.Input);
            ICollection<Guid> outputs = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.Output);
            ICollection<Guid> dcus = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.DCU);
            ICollection<Guid> doorEnviroments = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.DoorEnvironment);
            ICollection<Guid> cardReaders = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.CardReader);
            var card = Database.ConfigObjectsEngine.CardsStorage.GetFirstCard();
            ICollection<Guid> alarmAreas = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.AlarmArea);
            ICollection<Guid> cardSystems = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.CardSystem);
            ICollection<Guid> dailyPlans = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.DailyPlan);
            ICollection<Guid> securityTimeZones = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.SecurityTimeZone);
            ICollection<Guid> apbzs = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.AntiPassBackZone);

            var idCcu = Ccus.Singleton.GetCcuId() ?? Guid.Empty;

            TestAlarmAreaEvents.EnqueueTestEvents(
                alarmAreas.FirstOrDefault(),
                cardReaders.FirstOrDefault(),
                card,
                inputs.FirstOrDefault());

            TestAlarmEvents.EnqueueTestEvents(
                idCcu,
                inputs.FirstOrDefault(),
                alarmAreas.FirstOrDefault(),
                dcus.FirstOrDefault(),
                doorEnviroments.FirstOrDefault(),
                cardReaders.FirstOrDefault(),
                card);

            TestApbzEvents.EnqueueTestEvents(
                apbzs.FirstOrDefault(),
                cardReaders.FirstOrDefault(),
                card.IdCard);

            TestCardReaderEvents.EnqueueTestEvents(
                cardReaders.FirstOrDefault(),
                cardSystems.FirstOrDefault(),
                card,
                alarmAreas.FirstOrDefault(),
                outputs.FirstOrDefault());

            TestDcuEvents.EnqueueTestEvents(
                dcus.FirstOrDefault());

            TestDoorEnvironmentEvents.EnqueueTestEvents(
                doorEnviroments.FirstOrDefault(),
                cardReaders.FirstOrDefault(),
                card);

            TestInputEvents.EnqueueTestEvents(
                inputs.FirstOrDefault());

            TestOutputEvents.EnqueueTestEvents(
                outputs.FirstOrDefault());

            TestSystemEvents.EnqueueTestEvents(
                inputs.FirstOrDefault(),
                card.IdCard,
                cardReaders.FirstOrDefault(),
                dailyPlans.FirstOrDefault(),
                securityTimeZones.FirstOrDefault());

            TestUpcMonitorEvents.EnqueueTestEvents();

            TestUpgradeEvents.EnqueueTestEvents();
        }

        public void Init()
        {
            ReadOldEventsSettingsFromFile();
            SetAutonomousRun(true);
        }

        public void SetAutonomousRun(bool isAutonomousRun)
        {
            _queueSetAutonomousRun.Enqueue(
                new AutonomousRunStateChangeExecutor(isAutonomousRun));
        }

        private void SendAutonomousEventsToServer()
        {
            try
            {
                var autonomousEvents =
                    Database.AutonomousEventsEngine.SqlCeDbAutonomousEventsAcessor.SelectTop(AutonomousEventsBatchSize)
                        .ToList();

                if (autonomousEvents.Count == 0)
                {
                    _existsAutonomousEvents = false;
                    return;
                }

                foreach (var autonomousEvent in autonomousEvents)
                {
                    _sentAutonomousEventsIds.Add(autonomousEvent.EventId);
                    _sendToServerBatchWorker.Add(autonomousEvent, autonomousEvent.Priority);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void ReadOldEventsSettingsFromFile()
        {
            MemoryStream memoryStream = null;
            Stream inputStream = null;

            try
            {
                if (
                    File.Exists(string.Format("{0}{1}", _directoryForOldEventsSettings,
                        OLD_EVENT_SETTINGS_FILE_NAME)))
                {
                    inputStream =
                        PatchedFileStream.Open(
                            string.Format("{0}{1}", _directoryForOldEventsSettings,
                                OLD_EVENT_SETTINGS_FILE_NAME),
                            FileMode.Open, FileAccess.Read, FileShare.Read);

                    if (inputStream.Length <= 0)
                        return;

                    memoryStream = new MemoryStream();
                    var buffer = new byte[512];

                    do
                    {
                        int length;

                        try
                        {
                            length = inputStream.Read(buffer, 0, buffer.Length);
                        }
                        catch (Exception error)
                        {
                            CcuCore.DebugLog.Warning(Log.PERFORMANCE_LEVEL,
                                () =>
                                    string.Format(
                                        "Events: SaveOldEventsSettingsToFile - Tolerated file problem on : {0}{1}, exception: {2}",
                                        _directoryForOldEventsSettings, OLD_EVENT_SETTINGS_FILE_NAME,
                                        error));
                            throw;
                        }

                        if (length == 0)
                            break;

                        memoryStream.Write(buffer, 0, length);
                    }
                    while (true);

                    if (memoryStream.Length == 0)
                        return;

                    OldEventsSettings oldEventsSettings;
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        oldEventsSettings =
                            (new LwBinaryDeserializer<OldEventsSettings>(memoryStream))
                                .Deserialize();
                    }
                    catch (Exception error)
                    {
                        CcuCore.Singleton.SaveEventObjectDeserializeFailed(Guid.Empty, ObjectType.NotSupport,
                            "Events - ReadOldEventsSettingsFromFile", error.Message);
                        throw;
                    }

                    if (oldEventsSettings != null)
                    {
                        _oldEventsSettings.Set(
                            oldEventsSettings.SaveOldInputStateEvents,
                            oldEventsSettings.SaveOldOutputStateEvents,
                            oldEventsSettings.SaveOldAlarmAreaActualStateEvents,
                            oldEventsSettings.SaveOldAlarmAreaActivationStateEvents,
                            oldEventsSettings.SaveOldCardReaderOnlineStateEvents);
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (memoryStream != null)
                {
                    try
                    {
                        memoryStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }

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

        /// <summary>
        /// Set new settings for old events
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool SetOldEventsSettings(bool[] settings)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool Events.SetOldEventsSettings(bool[] settings): [{0}]", Log.GetStringFromParameters(settings)));
            if (settings == null)
            {
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool Events.SetOldEventsSettings return false");
                return false;
            }

            if (settings.Length > 0)
                _oldEventsSettings.SaveOldInputStateEvents = settings[0];

            if (settings.Length > 1)
                _oldEventsSettings.SaveOldOutputStateEvents = settings[1];

            if (settings.Length > 2)
                _oldEventsSettings.SaveOldAlarmAreaActualStateEvents = settings[2];

            if (settings.Length > 3)
                _oldEventsSettings.SaveOldAlarmAreaActivationStateEvents = settings[3];

            if (settings.Length > 4)
                _oldEventsSettings.SaveOldCardReaderOnlineStateEvents = settings[4];

            _safeThreadSaveToFileOldEventsSettings.Resume();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool Events.SetOldEventsSettings return true");
            return true;
        }

        private void SaveOldEventsSettingsToFile()
        {
            while (true)
            {
                _safeThreadSaveToFileOldEventsSettings.WaitForResume();
                _safeThreadSaveToFileOldEventsSettings.Suspend();

                if (!CcuCore.Singleton.WasExited)
                {
                    Stream outputStream = null;
                    MemoryStream memoryStream = null;
                    try
                    {
                        if (!Directory.Exists(_directoryForOldEventsSettings))
                            Directory.CreateDirectory(_directoryForOldEventsSettings);

                        memoryStream = new MemoryStream();

                        (new LwBinarySerializer<OldEventsSettings>(memoryStream))
                            .Serialize(_oldEventsSettings);

                        outputStream =
                            PatchedFileStream.Open(
                                string.Format("{0}{1}", _directoryForOldEventsSettings,
                                    OLD_EVENT_SETTINGS_FILE_NAME),
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.Read);

                        var buffer = new byte[512];
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        do
                        {
                            int length;

                            try
                            {
                                length = memoryStream.Read(buffer, 0, buffer.Length);
                            }
                            catch (Exception error)
                            {
                                CcuCore.DebugLog.Warning(Log.PERFORMANCE_LEVEL,
                                    () =>
                                        string.Format(
                                            "Events: SaveOldEventsSettingsToFile - Tolerated file problem on : {0}{1}, exception: {2}",
                                            _directoryForOldEventsSettings, OLD_EVENT_SETTINGS_FILE_NAME,
                                            error));
                                throw;
                            }

                            if (length == 0)
                                break;

                            outputStream.Write(buffer, 0, length);
                        } while (true);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                    finally
                    {
                        if (memoryStream != null)
                        {
                            try
                            {
                                memoryStream.Close();
                            }
                            catch (Exception error)
                            {
                                HandledExceptionAdapter.Examine(error);
                            }
                        }

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
                else
                {
                    break;
                }
            }
        }

        private void BeforeExit()
        {
            _safeThreadSaveToFileOldEventsSettings.Resume();
        }

        /// <summary>
        /// Get count of not acknowledged events
        /// </summary>
        /// <returns></returns>
        public int GetNotAcknowledgedEventsCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void Events.GetNotAcknowledgedEventsCount()");
            return _notAcknowledgedEventsCount;
        }

        /// <summary>
        /// Get count of events from autonomous run
        /// </summary>
        /// <returns></returns>
        public int GetEventsFromAutonomousRunCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void Events.GetEventsFromAutonomousRunCount()");
            return Database.AutonomousEventsEngine.SqlCeDbAutonomousEventsAcessor.SelectCount();
        }

        /// <summary>
        /// Get count of unprocessed events
        /// </summary>
        /// <returns></returns>
        public int GetUnprocessedEventsCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void Events.GetUnprocessedEventsCount()");
            return _queueSaveEvent.Count;
        }

        /// <summary>
        /// Save event to the queue
        /// </summary>
        /// <param name="eventParameters"></param>
        public void ProcessEvent(
            EventParameters.EventParameters eventParameters)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format("bool Events.SaveEvent(EventParameters eventParameters): [{0}]",
                        Log.GetStringFromParameters(
                        eventParameters)));

            if (eventParameters == null
                || _exclusiveFilterForEventTypes.IsEventTypeExcluded(eventParameters.EventType))
            {
                return;
            }

            eventParameters.Priority = GetEventPriority(eventParameters.EventType);

            _queueSaveEvent.Enqueue(
                new EventParametersRequest(eventParameters),
                eventParameters.Priority);
        }

        private bool SaveEventInAutonomouseMode(EventParameters.EventParameters eventParameters)
        {
            if (_isAutonomousRun)
            {
                SaveAutonomousEventsToDatabase(
                    new[] {eventParameters});

                return true;
            }

            return false;
        }

        private const uint HIGHEST_PRIORITY = 10;
        private const uint HIGH_PRIORITY = 20;
        private const uint NORMAL_PRIORITY = 30;
        private const uint LOW_PRIORITY = 100;
        private const uint PRIORITY_FOR_EVENTS_FROM_AUTONOMOUS_RUN = 200;

        private void FillPriorityByEventType()
        {
            // Highest priority
            _priorityByEventType.Add(
                EventType.CcuActualTimeSent,
                () => HIGHEST_PRIORITY);

            // High priority
            _priorityByEventType.Add(
                EventType.AlarmAreaAlarmStateChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaAlarmStateInfo,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaActivationStateChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DSMStateChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.CardReaderOnlineStateChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.CardReaderOnlineStateInfo,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuOnlineStateChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuMemoryWarning,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuFirmwareVersion,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.CardReaderCommandChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuPhysicalAddressChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuInputCount,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuOutputCount,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.SendTamper,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.TamperInfo,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.SensorInAlarmCount,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.CardReaderLastCardChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.UnsetAlarmAreaByObjectForAutomaticActivation,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmCCUPrimaryPowerMissing,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmCCUBatteryIsLow,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmCCUExtFuse,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.CoprocessorFailureChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaRequestActivationState,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.ICCUSendingOfObjectStateFailed,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.ICCUPortAlreadyUsed,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaSabotageStateChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaSabotageStateInfo,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.DcuInputsSabotage,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AntiPassBackZoneCardEntered,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AntiPassBackZoneCardExited,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AntiPassBackZoneCardTimedOut,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaSensorBlockingTypeChanged,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaSensorBlockingTypeInfo,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.AlarmAreaSensorStateInfo,
                () => HIGH_PRIORITY);

            _priorityByEventType.Add(
                EventType.InvalidPinRetriesLimitReached,
                () => HIGH_PRIORITY);

            // Normal priority
            _priorityByEventType.Add(
                EventType.DcuUpgradePercentageSet,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.CRUpgradePercentageSet,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.CRUpgradeResultSet,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.CEUpgradeFinished,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.ProcessDCUUpgradePackageFailed,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.ProcessCRUpgradePackageFailed,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.CcuUpgradeFileUnpackProgress,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.CCUUpgraderStartFailed,
                () => NORMAL_PRIORITY);

            _priorityByEventType.Add(
                EventType.CCUMemoryLoadStateChanged,
                () => NORMAL_PRIORITY);
        }

        private uint GetEventPriority(EventType eventType)
        {
            Func<uint> getPriority;
            
            return _priorityByEventType.TryGetValue(eventType, out getPriority)
                ? getPriority()
                : LOW_PRIORITY;
        }

        private void FillSaveToOldEventsByEventType()
        {
            _sendToOldEventsByEventType.Add(
                EventType.CcuActualTimeSent,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CcuUpgradeFileUnpackProgress,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CCUUpgraderStartFailed,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CEUpgradeFinished,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CRUpgradePercentageSet,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CRUpgradeResultSet,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.DcuUpgradePercentageSet,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.ProcessDCUUpgradePackageFailed,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.ProcessCRUpgradePackageFailed,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.OutputRealStateChanged,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaRequestActivationState,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaSabotageStateChanged,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.DcuInputsSabotage,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.InputStateInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaAlarmStateInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaSabotageStateInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CardReaderOnlineStateInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.TamperInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AutonomousRunEventsFilesPrepared,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaSensorBlockingTypeInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaSensorStateInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.CardReaderBlockedStateChanged,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.TimeBuingMatrixStateChangedInfo,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.DcuFirmwareVersion,
                () => false);

            _sendToOldEventsByEventType.Add(
                EventType.InputStateChanged,
                () => _oldEventsSettings.SaveOldInputStateEvents);

            _sendToOldEventsByEventType.Add(
                EventType.OutputStateChanged,
                () => _oldEventsSettings.SaveOldOutputStateEvents);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaAlarmStateChanged,
                () => _oldEventsSettings.SaveOldAlarmAreaActualStateEvents);

            _sendToOldEventsByEventType.Add(
                EventType.AlarmAreaActivationStateChanged,
                () => _oldEventsSettings.SaveOldAlarmAreaActivationStateEvents);

            _sendToOldEventsByEventType.Add(
                EventType.CardReaderOnlineStateChanged,
                () => _oldEventsSettings.SaveOldCardReaderOnlineStateEvents);

            _sendToOldEventsByEventType.Add(
                EventType.CardReaderCommandChanged,
                () => _oldEventsSettings.SaveOldCardReaderOnlineStateEvents);
        }

        /// <summary>
        /// Delete events from queue
        /// </summary>
        /// <param name="eventsId"></param>
        public void DeleteEvents(UInt64[] eventsId)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "void Events.DeleteEvents(int[] eventsId): [{0}]", 
                    Log.GetStringFromParameters(eventsId)));

            if (eventsId != null 
                && eventsId.Length > 0)
            {
                _queueDeleteEvent.Enqueue(new DeleteEventsExecutor(eventsId));
            }
        }

        /// <summary>
        /// Delete all events
        /// </summary>
        public void DeleteEvents()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void Events.DeleteEvents()");

            _queueSaveEvent.ClearAndBlock(); 
            _queueDeleteEvent.WaitUntilIdle();

            _queueDeleteEvent.ClearAndBlock();
            _queueDeleteEvent.WaitUntilIdle();

            lock (_lockEvents)
            {
                _eventsByEventId.Clear();
                _eventsForSavingToDatabaseByEventId.Clear();

                Database.AutonomousEventsEngine.SqlCeDbAutonomousEventsAcessor.DeleteAllFromDatabase();

                _notAcknowledgedEventsCount = 0;
            }

            _queueDeleteEvent.Unblock();
            _queueSaveEvent.Unblock(); 
        }

        private class DeleteEventsExecutor : IProcessingQueueRequest
        {
            private readonly UInt64[] _eventIds;

            public DeleteEventsExecutor(UInt64[] eventIds)
            {
                _eventIds = eventIds;
            }

            public void Execute()
            {
                if (_eventIds == null || _eventIds.Length == 0)
                    return;

                lock (Singleton._lockEvents)
                {
                    foreach (var eventId in _eventIds)
                    {
                        if (Singleton._eventsByEventId.Remove(eventId))
                        {
                            Singleton._notAcknowledgedEventsCount--;
                            continue;
                        }

                        if (Singleton._eventsForSavingToDatabaseByEventId.Remove(eventId))
                        {
                            Singleton._notAcknowledgedEventsCount--;
                            continue;
                        }

                        if (Singleton._sentAutonomousEventsIds.Remove(eventId))
                        {
                            Database.AutonomousEventsEngine.SqlCeDbAutonomousEventsAcessor.DeleteFromDataBase(eventId);

                            if (Singleton._sentAutonomousEventsIds.Count == 0)
                                Singleton.SendAutonomousEventsToServer();
                        }
                    }
                                 
                    if (Singleton._sentAutonomousEventsIds.Count == 0
                        && Singleton._existsAutonomousEvents)
                    {
                        Singleton.SendAutonomousEventsToServer();
                    }
                }
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }
    }
}
