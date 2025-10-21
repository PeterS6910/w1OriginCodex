using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys.Microsoft;
using JetBrains.Annotations;

using Microsoft.Win32;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.IwQuick.Threads;
using Contal.LwSerialization;
using Contal.Drivers.LPC3250;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal sealed class AlarmAreas : 
        AStateAndSettingsObjectCollection<AlarmAreas, AlarmAreaStateAndSettings, DB.AlarmArea>
    {
        public class SetUnsetParams 
        {
            public Guid IdCardReader
            {
                get;
                private set;
            }

            public Guid IdCard
            {
                get;
                private set;
            }

            public Guid IdPerson
            {
                get;
                private set;
            }

            public Guid IdActivationObject
            {
                get;
                private set;
            }

            public bool UnconditionalSet
            {
                get;
                private set;
            }

            public bool NoPrewarning
            {
                get;
                private set;
            }

            public SetUnsetParams(
                Guid idCardReader,
                AccessDataBase accessData,
                Guid idActivationObject,
                bool unconditionalSet,
                bool noPrewarning)
            {
                IdCardReader = idCardReader;
                IdCard = accessData.IdCard;
                IdPerson = accessData.IdPerson;
                IdActivationObject = idActivationObject;

                UnconditionalSet = unconditionalSet;
                NoPrewarning = noPrewarning;
            }

        }

        private class SensorsBlocking
        {
           public Dictionary<Guid, SensorBlockingType> SensorsBlockingTypes
           { 
               get;
               private set; 
           }

           public SensorsBlocking()
           {
               SensorsBlockingTypes = new Dictionary<Guid, SensorBlockingType>();
           }
        }

        private class AlarmAreaTimeBuyingSettings
        {
            // Run-time values

            private ITimer _timer;
            private bool _systemTimeChanged;

            // Stored values
            public DateTime TimeOfLastBuying
            {
                get;
                private set;
            }

            public int TotalBoughtTime
            {
                get;
                private set;
            }

            public int LastBoughtTime
            {
                get;
                private set;
            }

            public AlarmAreaTimeBuyingSettings(byte[] data)
            {
                TimeOfLastBuying = new DateTime(
                    BitConverter.ToInt64(data, 1),
                    (DateTimeKind)data[0]);

                TotalBoughtTime = BitConverter.ToInt32(data, 9);
                LastBoughtTime = BitConverter.ToInt32(data, 13);

                _systemTimeChanged = false;
            }

            public AlarmAreaTimeBuyingSettings(
                DateTime timeOfLastBuy,
                int totalBoughtTime,
                int lastBoughtTime)
            {
                TimeOfLastBuying = timeOfLastBuy;
                TotalBoughtTime = totalBoughtTime;
                LastBoughtTime = lastBoughtTime;

                _systemTimeChanged = false;
            }

            public byte[] GetBytes()
            {
                return Enumerable.Repeat((byte)TimeOfLastBuying.Kind, 1)
                    .Concat(BitConverter.GetBytes(TimeOfLastBuying.Ticks))
                    .Concat(BitConverter.GetBytes(TotalBoughtTime))
                    .Concat(BitConverter.GetBytes(LastBoughtTime))
                    .ToArray();
            }

            public AlarmAreaActionResult BuyTimeForAlarmArea(
                DB.AlarmArea alarmArea,
                bool isNew,
                bool useAllTime,
                DateTime currentTime,
                ref int tempTimeToBuy,
                out int remaining)
            {
                // Check if total bought time is limited
                remaining =
                    alarmArea.TimeBuyingTotalMax != null
                        ? alarmArea.TimeBuyingTotalMax.Value - TotalBoughtTime
                        : int.MaxValue;

                if (!isNew)
                {
                    // Check if total bought time is limited
                    if (alarmArea.TimeBuyingTotalMax != null)
                        // Subtract time to buy (new value do not need to subtract, because TotalBoughtTime is filled during creation)
                        remaining -= tempTimeToBuy;

                    // Check if previous bought time expired
                    if (_timer != null)
                        return AlarmAreaActionResult.FailedTimeAlreadyBought;

                    // Check if maximal time buying for alarm area is reached
                    if (TotalBoughtTime + tempTimeToBuy > alarmArea.TimeBuyingTotalMax)
                    {
                        // If not using all available time return error
                        if (!useAllTime)
                            return AlarmAreaActionResult.FailedTotalBoughtTimeReached;

                        // If this request use all available time calculate real bought time
                        tempTimeToBuy += remaining;
                        remaining = 0;

                        // There is no more time to buy
                        if (tempTimeToBuy <= 0)
                            return AlarmAreaActionResult.FailedTotalBoughtTimeReached;
                    }

                    // Calculate and set new time buying data
                    TimeOfLastBuying = currentTime;
                    LastBoughtTime = tempTimeToBuy;
                    TotalBoughtTime += tempTimeToBuy;

                    _systemTimeChanged = false;
                }

                _timer =
                    TimerManager.Static.StartTimeout(
                        tempTimeToBuy * 1000,
                        alarmArea.IdAlarmArea,
                        Singleton.OnBoughtTimeExpired);

                Singleton.EnqueueTimeBuyingRegistrySetting(
                    alarmArea.IdAlarmArea,
                    this);

                return AlarmAreaActionResult.Success;
            }

            public bool TryCancelBoughtTime(
                Guid idAlarmArea,
                DateTime currentTime)
            {
                if (_timer == null)
                    return false;

                // Stop timer
                _timer.StopTimer();
                _timer = null;

                // Do not recalculate bought time if system time was changed
                if (_systemTimeChanged)
                    return true;

                // Edit total bought time to contain only real spent time
                var usedTime = currentTime - TimeOfLastBuying;

                TotalBoughtTime -= LastBoughtTime;
                TotalBoughtTime += (int)(usedTime.TotalMilliseconds / 1000);

                Singleton.EnqueueTimeBuyingRegistrySetting(
                    idAlarmArea,
                    this);

                return true;
            }

            public void StopTimer()
            {
                if (_timer != null)
                    _timer.StopTimer();

                _timer = null;
            }

            public void OnBoughtTimeExpired()
            {
                _timer = null;
            }

            public void InitialStartTimer(
                int timeUntilSet,
                Guid idAlarmArea)
            {
                _timer =
                    TimerManager.Static.StartTimeout(
                        timeUntilSet,
                        idAlarmArea,
                        Singleton.OnBoughtTimeExpired);
            }

            public void OnSystemTimeChanged()
            {
                _systemTimeChanged = true;
            }
        }

        private class RegistryUpdaterForAlarmAreasStates : IProcessingQueueRequest<AlarmAreas>
        {
            public void Execute(AlarmAreas parentAlarmAreas)
            {
                var addedSetAlarmAreas = new LinkedList<Guid>();
                var removedSetAlarmAreas = new LinkedList<Guid>();
                ICollection<Guid> newSetAlarmAreas = new HashSet<Guid>();

                foreach (var kvpAlarmAreaState in parentAlarmAreas._alarmAreaStates.PairsSnapshot)
                {
                    if (!AlarmAreaIsSet(kvpAlarmAreaState.Value.ActivationState))
                        continue;

                    newSetAlarmAreas.Add(kvpAlarmAreaState.Key);

                    if (!parentAlarmAreas._savedSetAlarmAreas.Contains(kvpAlarmAreaState.Key))
                        addedSetAlarmAreas.AddLast(kvpAlarmAreaState.Key);
                }

                if (addedSetAlarmAreas.Count > 0
                    || parentAlarmAreas._savedSetAlarmAreas.Count != newSetAlarmAreas.Count)
                {
                    foreach (var guidSetAlarmArea in parentAlarmAreas._savedSetAlarmAreas)
                        if (!newSetAlarmAreas.Contains(guidSetAlarmArea))
                            removedSetAlarmAreas.AddLast(guidSetAlarmArea);

                    parentAlarmAreas._savedSetAlarmAreas = newSetAlarmAreas;
                }

                if (addedSetAlarmAreas.Count > 0
                    || removedSetAlarmAreas.Count > 0)
                {
                    parentAlarmAreas.LoadRegistryKeyForAlarmAreasAndExecute(
                        AlarmAreaRegistryKeyType.SetAlarmAreas,
                        registryKey =>
                        {
                            foreach (var removedSetAlarmArea in removedSetAlarmAreas)
                                registryKey.DeleteValue(removedSetAlarmArea.ToString());

                            foreach (var addedSetAlarmArea in addedSetAlarmAreas)
                                registryKey.SetValue(
                                    addedSetAlarmArea.ToString(),
                                    1,
                                    RegistryValueKind.DWord);
                        });
                }
            }

            public void OnError(
                AlarmAreas param, 
                Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private abstract class RegistryUpdaterForAlarmArea : IProcessingQueueRequest<AlarmAreas>
        {
            private readonly AlarmAreaRegistryKeyType _alarmAreaRegistryKeyType;
            protected Guid IdAlarmArea { get; private set; }

            protected RegistryUpdaterForAlarmArea(
                AlarmAreaRegistryKeyType alarmAreaRegistryKeyType,
                Guid idAlarmAlrea)
            {
                _alarmAreaRegistryKeyType = alarmAreaRegistryKeyType;
                IdAlarmArea = idAlarmAlrea;
            }

            protected void LoadRegistryKeyForAlarmAreasAndExecute(
                AlarmAreas parentAlarmAreas,
                Action<RegistryKey> lambda)
            {
                parentAlarmAreas.LoadRegistryKeyForAlarmAreasAndExecute(
                    _alarmAreaRegistryKeyType,
                    lambda);
            }

            public abstract void Execute(AlarmAreas param);

            public void OnError(
                AlarmAreas param, 
                Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private class RegistryRemoverForTimeBuying : RegistryUpdaterForAlarmArea
        {
            public RegistryRemoverForTimeBuying(Guid idAlarmAlrea)
                : base(
                    AlarmAreaRegistryKeyType.TimeBuying,
                    idAlarmAlrea)
            {
            }

            public override void Execute(AlarmAreas parentAlarmAreas)
            {
                LoadRegistryKeyForAlarmAreasAndExecute(
                    parentAlarmAreas,
                    registryKey =>
                        registryKey.DeleteValue(IdAlarmArea.ToString()));
            }
        }

        private class RegistryUpdaterForTimeBuying : RegistryUpdaterForAlarmArea
        {
            private readonly AlarmAreaTimeBuyingSettings _alarmAreaTimeBuyingSettings;

            public RegistryUpdaterForTimeBuying(
                Guid idAlarmAlrea,
                AlarmAreaTimeBuyingSettings alarmAreaTimeBuyingSettings)
                : base(
                    AlarmAreaRegistryKeyType.TimeBuying,
                    idAlarmAlrea)
            {
                _alarmAreaTimeBuyingSettings = alarmAreaTimeBuyingSettings;
            }

            public override void Execute(AlarmAreas parentAlarmAreas)
            {
                LoadRegistryKeyForAlarmAreasAndExecute(
                    parentAlarmAreas,
                    registryKey => registryKey.SetValue(
                        IdAlarmArea.ToString(),
                        _alarmAreaTimeBuyingSettings.GetBytes()));

            }
        }

        private class RegistryUpdaterForSensorBlocking : RegistryUpdaterForAlarmArea
        {
            private readonly Guid _idInput;
            private readonly SensorBlockingType _sensorBlockingType;

            public RegistryUpdaterForSensorBlocking(
                Guid idAlarmAlrea,
                Guid idInput,
                SensorBlockingType sensorBlockingType)
                : base(
                    AlarmAreaRegistryKeyType.SensorsBlocking,
                    idAlarmAlrea)
            {
                _idInput = idInput;
                _sensorBlockingType = sensorBlockingType;
            }

            public override void Execute(AlarmAreas parentAlarmAreas)
            {
                LoadRegistryKeyForAlarmAreasAndExecute(
                    parentAlarmAreas,
                    registryKey =>
                    {
                        SensorsBlocking sensorsBlocking;

                        var alarmAreaSubKey = parentAlarmAreas._sensorsBlockingTypes.GetOrAddValue(
                            IdAlarmArea,
                            out sensorsBlocking,
                            new SensorsBlocking())
                            ? registryKey.CreateSubKey(IdAlarmArea.ToString())
                            : registryKey.OpenSubKey(IdAlarmArea.ToString(), true);

                        try
                        {
                            if (_sensorBlockingType == SensorBlockingType.Unblocked)
                            {
                                if (!sensorsBlocking.SensorsBlockingTypes.Remove(_idInput))
                                    return;

                                alarmAreaSubKey.DeleteValue(_idInput.ToString(), false);
                                return;
                            }

                            sensorsBlocking.SensorsBlockingTypes[_idInput] = _sensorBlockingType;
                            alarmAreaSubKey.SetValue(_idInput.ToString(), (byte)_sensorBlockingType,
                                RegistryValueKind.DWord);
                        }
                        finally
                        {
                            alarmAreaSubKey.Close();
                        }
                    });
            }
        }

        private const string ALARM_AREAS_CRS_REPORTING_FILENAME = "AlarmAreaCRsReporting.dat";

        public const string ACTIVATIONONOFF = "Activation";
        public const string PREWARNINGONOFF = "Prewarning";
        public const string TMPUNSETENTRYONOFF = "TemporaryUnsetEntryOnOff";
        public const string TMPUNSETEXITONOFF = "TemporaryUnsetExitOnOff";
        public const string AALARMONOFF = "AAlarmOnOff";
        public const string OUTPUT_SET_BY_ON_OFF_OBJECT_FAILED_ON_OFF = "AA set by on off object failed";
        private const string CCU_REG_ALARM_AREA_RESTRICTIVE_POLICY_FOR_TIME_BUYING = "AlarmAreaRestrictivePolicyForTimeBuying";

        public event DVoid2Void GeneralAlarmAreasReportingToCRChanged;

        public EventHandlerGroup<IAlarmAreaEventHandler> EventHandlerGroup = 
            new EventHandlerGroup<IAlarmAreaEventHandler>();

        public bool AlarmAreaRestrictivePolicyForTimeBuying { get; private set; }

        private AlarmAreas()
            : base(null)
        {
            _alarmAreaRegistryUpdater =  new ThreadPoolQueue<IProcessingQueueRequest<AlarmAreas>, AlarmAreas>(
                ThreadPoolGetter.Get(),
                this);

            CcuCore.Singleton.BeforeExit += BeforeExit;

            SystemTime._timeChanged += SystemTime__timeChanged;

            AlarmsManager.Singleton.AlarmAcknowledged += OnAlarmAcknowledged;

            ReadAlarmAreaStates();
            _alarmAreasToCRsReporting = LoadAAToCRsReporting();

            ReadSensorBlockingTypes();
            GetAlarmAreaRestrictivePolicyForTimeBuying();
        }

        private void OnAlarmAcknowledged(
            Alarm alarm,
            AlarmsManager.IActionSources acknowledgeSources)
        {
            var alarmKey = alarm.AlarmKey;

            var alarmType = alarmKey.AlarmType;

            Guid idAlarmArea;

            switch (alarmType)
            {
                case AlarmType.AlarmArea_Alarm:
                case AlarmType.AlarmArea_SetByOnOffObjectFailed:

                    idAlarmArea = (Guid)alarmKey.AlarmObject.Id;
                    break;

                case AlarmType.Sensor_Alarm:
                case AlarmType.Sensor_Tamper_Alarm:

                    var alarmAreaIdAndObjectType = 
                        alarmKey.ExtendedObjects.First(
                            idAndObjectType =>
                                idAndObjectType.ObjectType == ObjectType.AlarmArea);

                    idAlarmArea = (Guid)alarmAreaIdAndObjectType.Id;
                    break;

                default:

                    return;
            }

            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            if (alarmAreaController == null)
                return;

            switch (alarmType)
            {
                case AlarmType.AlarmArea_Alarm:

                    alarmAreaController.OnAlarmAcknowledged(acknowledgeSources);
                    break;

                case AlarmType.Sensor_Alarm:

                    alarmAreaController.OnSensorAlarmAcknowledged(
                        (Guid)alarmKey.AlarmObject.Id,
                        acknowledgeSources);

                    break;

                case AlarmType.Sensor_Tamper_Alarm:

                    alarmAreaController.OnSensorTamperAlarmAcknowledged(
                        (Guid)alarmKey.AlarmObject.Id,
                        acknowledgeSources);

                    break;

                case AlarmType.AlarmArea_SetByOnOffObjectFailed:

                    if (alarm.AlarmState == AlarmState.Alarm)
                        alarmAreaStateAndSettings.SetSpecialOutputSetByObjecForAaFailedOff();

                    break;
            }
        }

        private readonly SyncDictionary<Guid, AlarmAreaStates> _alarmAreaStates =
            new SyncDictionary<Guid, AlarmAreaStates>();

        public void SaveAlarmAreaActivationState(Guid guidAlarmArea, State activationState)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void AlarmAreas.SaveAlarmAreaActivationState(Guid guidAlarmArea, State activationState): [{0}]",
                        Log.GetStringFromParameters(guidAlarmArea, activationState)));

            lock (_alarmAreaStates)
            {
                AlarmAreaStates alarmAreaStates;

                if (!_alarmAreaStates.TryGetValue(
                    guidAlarmArea, out alarmAreaStates))
                {
                    alarmAreaStates = new AlarmAreaStates();

                    _alarmAreaStates.Add(
                        guidAlarmArea,
                        alarmAreaStates);
                }

                alarmAreaStates.ActivationState = activationState;
                SaveAlarmAreasStates();
            }
        }

        private void SaveAlarmAreasStates()
        {
            _alarmAreaRegistryUpdater.Enqueue(new RegistryUpdaterForAlarmAreasStates());
        }

        private void BeforeExit()
        {
            _alarmAreaRegistryUpdater.Enqueue(new RegistryUpdaterForAlarmAreasStates());
 }

        private ICollection<Guid> _savedSetAlarmAreas = 
            new HashSet<Guid>();

        #region Alarm area registry

        readonly ThreadPoolQueue<IProcessingQueueRequest<AlarmAreas>, AlarmAreas> _alarmAreaRegistryUpdater;

        private void ReadAlarmAreaStates()
        {
            try
            {
                string[] setAlarmAreasNames = null;

                LoadRegistryKeyForAlarmAreasAndExecute(
                    AlarmAreaRegistryKeyType.SetAlarmAreas,
                    registryKey => setAlarmAreasNames = registryKey.GetValueNames());

                _alarmAreaStates.Clear(
                    () => ReadAlarmAreaStatesCore(setAlarmAreasNames));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void ReadAlarmAreaStatesCore(string[] setAlarmAreasNames)
        {
            _savedSetAlarmAreas.Clear();

            if (setAlarmAreasNames == null
                || setAlarmAreasNames.Length == 0)
                return;

            foreach (var setAlarmAreaName in setAlarmAreasNames)
            {
                var alarmAreaGuid = new Guid(setAlarmAreaName);

                if (_alarmAreaStates.ContainsKey(alarmAreaGuid))
                    continue;

                _savedSetAlarmAreas.Add(alarmAreaGuid);

                _alarmAreaStates.Add(
                    alarmAreaGuid,
                    new AlarmAreaStates
                    {
                        ActivationState = State.Set
                    });
            }
        }

        internal void ReadAlarmAreaTimeBuyingStates()
        {
            try
            {
                var timeBuyingFromRegistry = LoadTimeByingFromRegistry();

                _alarmAreaTimeBuyingStates.Clear(
                    (key, value) => value.StopTimer(),
                    timeBuyingFromRegistry == null || timeBuyingFromRegistry.Count == 0
                        ? (Action)null
                        : (() => AddTimeBuyingFromRegistry(timeBuyingFromRegistry)));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddTimeBuyingFromRegistry(IEnumerable<KeyValuePair<Guid, AlarmAreaTimeBuyingSettings>> timeBuyingFromRegistry)
        {
            var i = 1;

            foreach (var entry in timeBuyingFromRegistry)
            {
                if (!AlarmAreaIsUnset(entry.Key))
                {
                    _alarmAreaTimeBuyingStates[entry.Key] = entry.Value;
                    continue;
                }

                var timeDiff =
                    (int)(DateTime.UtcNow - entry.Value.TimeOfLastBuying).TotalMilliseconds;

                var timeUntilSet = (entry.Value.LastBoughtTime * 1000) - timeDiff;

                if (timeUntilSet <= 0)
                    timeUntilSet = i++;

                var idAlarmArea = entry.Key;

                _alarmAreaTimeBuyingStates.Add(
                    entry.Key,
                    entry.Value,
                    null,
                    (key, value) =>
                        value.InitialStartTimer(
                            timeUntilSet,
                            idAlarmArea));
            }
        }

        private ICollection<KeyValuePair<Guid, AlarmAreaTimeBuyingSettings>>
            LoadTimeByingFromRegistry()
        {
            ICollection<KeyValuePair<Guid, AlarmAreaTimeBuyingSettings>> result = null;

            LoadRegistryKeyForAlarmAreasAndExecute(
                AlarmAreaRegistryKeyType.TimeBuying,
                registryKey =>
                {
                    var setAlarmAreasNames = registryKey.GetValueNames();
                    if (setAlarmAreasNames.Length <= 0)
                        return;

                    result =
                        new LinkedList<KeyValuePair<Guid, AlarmAreaTimeBuyingSettings>>();

                    foreach (var alarmAreaName in setAlarmAreasNames)
                        try
                        {
                            // Get serialized value from registry
                            var value = registryKey.GetValue(
                                alarmAreaName,
                                null) as byte[];

                            if (value == null)
                                continue;

                            result.Add(
                                new KeyValuePair<Guid, AlarmAreaTimeBuyingSettings>(
                                    new Guid(alarmAreaName),
                                    new AlarmAreaTimeBuyingSettings(value)));
                        }
                        catch (Exception ex)
                        {
                            HandledExceptionAdapter.Examine(ex);
                        }
                });

            return result;
        }

        public void DeleteAllAlarmAreasAndTimeBuyingStates()
        {
            try
            {
                LoadRegistryKeyForAlarmAreasAndExecute(
                    AlarmAreaRegistryKeyType.SetAlarmAreas,
                    DeleteAllValuesInRegistryKey);

                LoadRegistryKeyForAlarmAreasAndExecute(
                    AlarmAreaRegistryKeyType.TimeBuying,
                    DeleteAllValuesInRegistryKey);

                LoadRegistryKeyForAlarmAreasAndExecute(
                    AlarmAreaRegistryKeyType.SensorsBlocking,
                    DeleteAllSubKeysInRegistryKey);

                lock (_alarmAreaStates)
                {
                    _alarmAreaStates.Clear();
                    _savedSetAlarmAreas.Clear();
                }

                _alarmAreaTimeBuyingStates.Clear();

                _sensorsBlockingTypes.Clear();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private static void DeleteAllValuesInRegistryKey(RegistryKey registryKey)
        {
            var setAlarmAreasNames = registryKey.GetValueNames();

            if (setAlarmAreasNames != null && setAlarmAreasNames.Length > 0)
            {
                foreach (var alarmAreaName in setAlarmAreasNames)
                {
                    registryKey.DeleteValue(alarmAreaName);
                }
            }
        }

        private static void DeleteAllSubKeysInRegistryKey(RegistryKey registryKey)
        {
            var subKeyNames = registryKey.GetSubKeyNames();

            if (subKeyNames != null && subKeyNames.Length > 0)
            {
                foreach (var subKeyName in subKeyNames)
                {
                    registryKey.DeleteSubKey(subKeyName);
                }
            }
        }

        private readonly SyncDictionary<Guid, SensorsBlocking> _sensorsBlockingTypes = new SyncDictionary<Guid, SensorsBlocking>();

        private void ReadSensorBlockingTypes()
        {
            LoadRegistryKeyForAlarmAreasAndExecute(
                AlarmAreaRegistryKeyType.SensorsBlocking,
                registryKey =>
                {
                    var alarmAreaSubKeyNames = registryKey.GetSubKeyNames();

                    foreach (var alarmAreaSubKeyName in alarmAreaSubKeyNames)
                    {
                        var alarmAreaSubKey = registryKey.OpenSubKey(alarmAreaSubKeyName, false);

                        var alarmAreaSubKeyValueNames = alarmAreaSubKey.GetValueNames();

                        foreach (var alarmAreaSubKeyValueName in alarmAreaSubKeyValueNames)
                        {
                            var blockingType = Convert.ToByte(alarmAreaSubKey.GetValue(alarmAreaSubKeyValueName));

                            var idAlarmArea = new Guid(alarmAreaSubKeyName);
                            var idInput = new Guid(alarmAreaSubKeyValueName);

                            _sensorsBlockingTypes.GetOrAddValue(
                                idAlarmArea,
                                key =>
                                    new SensorsBlocking(),
                                (key, vlaue, newlyAdded) =>
                                    vlaue.SensorsBlockingTypes.Add(
                                        idInput,
                                        (SensorBlockingType) blockingType));
                        }

                        alarmAreaSubKey.Close();
                    }
                });
        }

        public SensorBlockingType GetSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput)
        {
            var result = SensorBlockingType.Unblocked;

            _sensorsBlockingTypes.TryGetValue(
                idAlarmArea,
                (key, found, value) =>
                {
                    if (!found)
                        return;

                    SensorBlockingType sensorBlockingType;

                    if (value.SensorsBlockingTypes.TryGetValue(
                        idInput,
                        out sensorBlockingType))
                    {
                        result = sensorBlockingType;
                    }
                });

            return result;
        }

        public void SaveSensorBlocking(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType sensorBlockingType)
        {
            _alarmAreaRegistryUpdater.Enqueue(
                new RegistryUpdaterForSensorBlocking(
                    idAlarmArea,
                    idInput,
                    sensorBlockingType));

        }

        private RegistryKey _setAlarmAreasRegistryKey;
        private RegistryKey _alarmAreasTimeBuyingRegistryKey;
        private RegistryKey _sensorsBlockingRegistryKey;
        private readonly object _lockSetAlarmAreasRegistryKey = new object();
        private const int DELAY_FOR_CLOSE_SET_ALARM_AREAS_REGSTRY_KEY = 30000;
        private const string REGISTRY_SET_ALARM_AREAS_PATH = @"SetAlarmAreas";
        private const string REGISTRY_TIME_BUYING_PATH = @"TimeBuying";
        private const string REGISTRY_SENSORS_BLOCKING_PATH = @"SensorsBlocking";

        private enum AlarmAreaRegistryKeyType
        {
            SetAlarmAreas,
            TimeBuying,
            SensorsBlocking
        }

        private void LoadRegistryKeyForAlarmAreasAndExecute(
            AlarmAreaRegistryKeyType keyType,
            Action<RegistryKey> lambda)
        {
            if (lambda == null)
                return;

            lock (_lockSetAlarmAreasRegistryKey)
            {
                var allKeysNull = _setAlarmAreasRegistryKey == null
                                  && _alarmAreasTimeBuyingRegistryKey == null
                                  && _sensorsBlockingRegistryKey == null;

                RegistryKey key = null;

                if (keyType == AlarmAreaRegistryKeyType.SetAlarmAreas)
                {
                    if (_setAlarmAreasRegistryKey == null)
                        _setAlarmAreasRegistryKey =
                            RegistryHelper.GetOrAddKey(
                                string.Format(@"{0}\{1}",
                                    CcuCore.REGISTRY_CCU_PATH,
                                    REGISTRY_SET_ALARM_AREAS_PATH));


                    key = _setAlarmAreasRegistryKey;
                }

                if (keyType == AlarmAreaRegistryKeyType.TimeBuying)
                {
                    if (_alarmAreasTimeBuyingRegistryKey == null)
                        _alarmAreasTimeBuyingRegistryKey =
                            RegistryHelper.GetOrAddKey(
                                string.Format(@"{0}\{1}",
                                    CcuCore.REGISTRY_CCU_PATH,
                                    REGISTRY_TIME_BUYING_PATH));

                    key = _alarmAreasTimeBuyingRegistryKey;
                }

                if (keyType == AlarmAreaRegistryKeyType.SensorsBlocking)
                {
                    if (_sensorsBlockingRegistryKey == null)
                        _sensorsBlockingRegistryKey =
                            RegistryHelper.GetOrAddKey(
                                string.Format(@"{0}\{1}",
                                    CcuCore.REGISTRY_CCU_PATH,
                                    REGISTRY_SENSORS_BLOCKING_PATH));

                    key = _sensorsBlockingRegistryKey;
                }

                if (key == null)
                    return; //TODO Send error to server

                if (allKeysNull)
                    NativeTimerManager.StartTimeout(
                        DELAY_FOR_CLOSE_SET_ALARM_AREAS_REGSTRY_KEY,
                        OnNativeTimerCloseSetAlarmAreasRegistryKey);

                lambda(key);
            }
        }

        private bool OnNativeTimerCloseSetAlarmAreasRegistryKey(NativeTimer timer)
        {
            lock (_lockSetAlarmAreasRegistryKey)
            {
                try
                {
                    if (_setAlarmAreasRegistryKey != null)
                        _setAlarmAreasRegistryKey.Close();

                    if (_alarmAreasTimeBuyingRegistryKey != null)
                        _alarmAreasTimeBuyingRegistryKey.Close();

                    if (_sensorsBlockingRegistryKey != null)
                        _sensorsBlockingRegistryKey.Close();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                _setAlarmAreasRegistryKey = null;
                _alarmAreasTimeBuyingRegistryKey = null;
                _sensorsBlockingRegistryKey = null;
            }

            return true;
        }

        #endregion

        public bool SetAlarmArea(
            Guid guidAlarmArea,
            bool setSynchronously,
            SetUnsetParams setUnsetParams)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSetting;

            if (!_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSetting))
            {
                return false;
            }
            // noPrewarning is available only if Time Buying is enabled and prewarning is enabled
            if (setUnsetParams.NoPrewarning)
            {
                var alarmArea = 
                    Database.ConfigObjectsEngine.GetFromDatabase(
                        ObjectType.AlarmArea, 
                        guidAlarmArea) as DB.AlarmArea;

                if (alarmArea == null)
                    return false;

                if (!alarmArea.PreWarning)
                    return false;
            }

            var alarmAreaController = alarmAreaStateAndSetting.AlarmAreaController;

            return
                alarmAreaController != null
                && alarmAreaController.SetAlarmArea(
                    setSynchronously,
                    setUnsetParams);
        }

        public void SetAlarmAreaOnOffObject(
            Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSetting;

            if (_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSetting))
            {
                var alarmAreaController = alarmAreaStateAndSetting.AlarmAreaController;

                if (alarmAreaController != null)
                    alarmAreaController.OnActivatedObjectForAa();
            }
        }

        public void UnsetAlarmAreaOnOffObject(
            Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSetting;

            if (!_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSetting))
            {
                return;
            }

            var alarmAreaController = alarmAreaStateAndSetting.AlarmAreaController;

            if (alarmAreaController != null)
                alarmAreaController.OnDeactivatedObjectForAa();
        }

        public bool IsCrReportingEnabled(Guid alarmAreaId)
        {
            if (_alarmAreasToCRsReporting != null)
            {
                bool? isReportingEnabled;

                if (_alarmAreasToCRsReporting.TryGetValue(
                    alarmAreaId,
                    out isReportingEnabled)
                        && isReportingEnabled != null)
                {
                    return isReportingEnabled.Value;
                }
            }

            return DevicesAlarmSettings.Singleton.AllowAAToCRsReporting;
        }

        public bool IsCrReportingRegistered(Guid idAlarmArea)
        {
            bool? isReportingEnabled;

            return
                _alarmAreasToCRsReporting != null
                && _alarmAreasToCRsReporting.TryGetValue(
                    idAlarmArea,
                    out isReportingEnabled)
                && isReportingEnabled != null;
        }

        public void UnsetAlarmArea(
            AlarmAreaStateAndSettings.IAlarmAreaUnsetResult alarmAreaUnsetResult,
            bool unsetSynchronously,
            Guid guidAlarmArea,
            Guid guidLogin,
            Guid guidPerson,
            int timeToBuy,
            [NotNull]
            SetUnsetParams setUnsetParams)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSettings))
            {
                var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

                if (alarmAreaController != null)
                    alarmAreaController.UnsetAlarmArea(
                        alarmAreaUnsetResult, 
                        unsetSynchronously, 
                        guidLogin, 
                        guidPerson, 
                        timeToBuy,
                        setUnsetParams);
            }
        }

        public byte GetAlarmAreaAlarmState(Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return (byte)alarmAreaStateAndSettings.AlarmState;
            }

            return (byte)State.Unknown;
        }

        public byte GetAlarmAreaActivationState(Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return (byte)alarmAreaStateAndSettings.ActivationState;
            }

            return (byte)State.Unknown;
        }

        public bool AlarmAreaIsSet(Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (_objects.TryGetValue(
                guidAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return AlarmAreaIsSet(alarmAreaStateAndSettings.ActivationState);
            }

            return false;
        }

        public static bool AlarmAreaIsSet(State activationState)
        {
            return activationState == State.Set
                || activationState == State.TemporaryUnsetEntry
                || activationState == State.TemporaryUnsetExit
                || activationState == State.Prewarning;
        }

        public bool AlarmAreaAlarmNotAcknowledged(Guid guidAlarmArea)
        {
            return
                AlarmsManager.Singleton.ExistsNotAcknowledgedAlarm(
                    AlarmAreaAlarm.CreateAlarmKey(guidAlarmArea));
        }

        public bool AlarmAreaIsUnset(Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            return
                _objects.TryGetValue(
                        guidAlarmArea,
                        out alarmAreaStateAndSettings)
                    && !AlarmAreaIsSet(alarmAreaStateAndSettings.ActivationState);
        }

        /// <summary>
        /// Runs an event after general alarm area reporting to CR's property change       
        /// </summary>
        public void RunGeneralAlarmAreasReportingToCRChanged()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "void AlarmAreas.RunGeneralAlarmAreasReportingToCRChanged()");

            if (GeneralAlarmAreasReportingToCRChanged != null)
                GeneralAlarmAreasReportingToCRChanged();
        }

        public void SendAllStates()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                "void AlarmAreas.SendAllStates()");

            ICollection<AlarmAreaStateAndSettings> alarmAreaSettingsCollection =
                _objects.ValuesSnapshot;

            foreach (var alarmAreasSettings in alarmAreaSettingsCollection)
                alarmAreasSettings.SendAllStates();
        }

        public bool HasSomeSensorsActive()
        {
            return _objects.ValuesSnapshot
                .Select(alarmAreasSettings => alarmAreasSettings.AlarmAreaController)
                .Any(alarmAreaController => 
                    alarmAreaController != null
                    && alarmAreaController.HasAnySensorActive());
        }

        private Dictionary<Guid, bool?> _alarmAreasToCRsReporting = new Dictionary<Guid, bool?>();

        public bool SaveAAToCRsReporting(Dictionary<Guid, bool?> settings)
        {
            QuickPath.EnsureDirectory(CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME);

            lock (_syncAAToCRsReporting)
            {
                MemoryStream memoryStream = null;
                Stream outputStream = null;

                try
                {
                    memoryStream = new MemoryStream();

                    var binarySerializer = new LwBinarySerializer<Dictionary<Guid, bool?>>(memoryStream);
                    binarySerializer.Serialize(settings);

                    var buffer = new byte[512];
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    outputStream =
                        PatchedFileStream.Open(
                            CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME + ALARM_AREAS_CRS_REPORTING_FILENAME,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.Read);
                    do
                    {
                        int length;

                        try
                        {
                            length = memoryStream.Read(buffer, 0, buffer.Length);
                        }
                        catch (Exception error)
                        {
                            CcuCore.DebugLog.Error(
                                Log.CALM_LEVEL,
                                () => string.Format("AlarmAreas: LoadAAToCRsReporting - Critical file problem on : {0}, exception: {1}",
                                CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME + ALARM_AREAS_CRS_REPORTING_FILENAME,
                                error));

                            throw;
                        }

                        if (length == 0)
                            break;

                        outputStream.Write(buffer, 0, length);
                    }
                    while (true);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    // if an error occurs, the outputStream would not be closed, if used inside of try section
                    return false;
                }
                finally
                {
                    if (memoryStream != null)
                        try
                        {
                            memoryStream.Close();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                    if (outputStream != null)
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

            return true;
        }

        private Dictionary<Guid, bool?> LoadAAToCRsReporting()
        {
            var result = new Dictionary<Guid, bool?>();

            lock (_syncAAToCRsReporting)
            {
                MemoryStream memoryStream = null;
                Stream inputStream = null;

                try
                {
                    if (!File.Exists(
                        CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME +
                        ALARM_AREAS_CRS_REPORTING_FILENAME))
                    {
                        return result;
                    }

                    inputStream =
                        PatchedFileStream.Open(
                            CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME +
                            ALARM_AREAS_CRS_REPORTING_FILENAME,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read);

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
                            CcuCore.DebugLog.Error(
                                Log.CALM_LEVEL,
                                () => string.Format(
                                    "AlarmAreas: LoadAAToCRsReporting - Critical file problem on : {0}, exception: {1}",
                                    CcuCore.Singleton.RootPath +
                                    Database.DATABASE_DIRECTORY_NAME +
                                    ALARM_AREAS_CRS_REPORTING_FILENAME,
                                    error));

                            throw;
                        }

                        if (length == 0)
                            break;

                        memoryStream.Write(buffer, 0, length);
                    }
                    while (true);

                    if (memoryStream.Length == 0)
                        return result;

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var binaryDeserializer = new LwBinaryDeserializer<Dictionary<Guid, bool?>>(memoryStream);

                    try
                    {
                        result = binaryDeserializer.Deserialize();
                    }
                    catch (Exception error)
                    {
                        CcuCore.Singleton.SaveEventObjectDeserializeFailed(
                            Guid.Empty,
                            ObjectType.NotSupport,
                            "AlarmAreas - LoadAAToCRsReporting",
                            error.Message);

                        throw;
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
                finally
                {
                    if (memoryStream != null)
                        try
                        {
                            memoryStream.Close();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                    if (inputStream != null)
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

            return result;
        }

        /// <summary>
        /// Updates alarm area reporting to CR's property specific to alarm area
        /// </summary>
        /// <param name="guidAlarmArea"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool UpdateAAToCRsReporting(
            Guid guidAlarmArea,
            bool? report)
        {
            if (_alarmAreasToCRsReporting != null)
            {
                bool? previousValue;

                if (_alarmAreasToCRsReporting.TryGetValue(
                    guidAlarmArea,
                    out previousValue))
                {
                    if (previousValue == report)
                        return SaveAAToCRsReporting(_alarmAreasToCRsReporting);

                    _alarmAreasToCRsReporting[guidAlarmArea] = report;

                    AlarmAreaStateAndSettings alarmAreaStateAndSettings;

                    if (_objects.TryGetValue(
                        guidAlarmArea,
                        out alarmAreaStateAndSettings))
                    {
                        alarmAreaStateAndSettings.OnAlarmAreaReportingToCRChanged(report);
                    }

                    return SaveAAToCRsReporting(_alarmAreasToCRsReporting);
                }
            }
            else
                _alarmAreasToCRsReporting = new Dictionary<Guid, bool?>();

            _alarmAreasToCRsReporting.Add(
                guidAlarmArea,
                report);

            return SaveAAToCRsReporting(_alarmAreasToCRsReporting);
        }

        /// <summary>
        /// Removes alarm area from reporting alarm areas
        /// </summary>
        /// <param name="aaGuid"></param>
        /// <returns></returns>
        public bool DeleteFromAAToCRsReporting(Guid aaGuid)
        {
            if (_alarmAreasToCRsReporting == null)
                _alarmAreasToCRsReporting = new Dictionary<Guid, bool?>();

            if (_alarmAreasToCRsReporting.ContainsKey(aaGuid))
                _alarmAreasToCRsReporting.Remove(aaGuid);

            return SaveAAToCRsReporting(_alarmAreasToCRsReporting);
        }

        /// <summary>
        /// Clears alarm area reporting records
        /// </summary>
        public void ClearAAToCRsReporting()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void AlarmAreas.ClearAAToCRsReporting()");
            if (_alarmAreasToCRsReporting == null)
                _alarmAreasToCRsReporting = new Dictionary<Guid, bool?>();

            _alarmAreasToCRsReporting.Clear();
            SaveAAToCRsReporting(_alarmAreasToCRsReporting);
        }

        /// <summary>
        /// Return state of external alarm area
        /// </summary>
        /// <param name="idAlarmArea"></param>
        /// <param name="isExteranlAA"></param>
        /// <param name="isWaiting"></param>
        /// <param name="wasSetOrUnsetConfirmed"></param>
        public void GetStateOfExternalAA(
            Guid idAlarmArea,
            out bool isExteranlAA,
            out bool isWaiting,
            out bool wasSetOrUnsetConfirmed)
        {
            isExteranlAA = false;
            isWaiting = false;
            wasSetOrUnsetConfirmed = false;

            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return;
            }

            var externAlarmAreaSettings =
                alarmAreaStateAndSettings.AlarmAreaController
                    as ExternalAlarmAreaController;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (externAlarmAreaSettings == null)
                return;

            isExteranlAA = true;

            isWaiting = externAlarmAreaSettings.IsWaitingForExternalSystem();
            wasSetOrUnsetConfirmed = externAlarmAreaSettings.WasSetOrUnsetConfirmed;
        }

        #region TimeBuying

        readonly SyncDictionary<Guid, AlarmAreaTimeBuyingSettings> _alarmAreaTimeBuyingStates =
            new SyncDictionary<Guid, AlarmAreaTimeBuyingSettings>();

        private readonly object _syncAAToCRsReporting = new object();

        /// <summary>
        /// Update bought time for alarm area and start timeout
        /// </summary>
        /// <param name="alarmArea"></param>
        /// <param name="timeToBuy">Time to buy or -1 for buying all available time. If -1 is used, it will be changed to amount of time which was bought.</param>
        /// <param name="remainingTime">returns remaining time to buy or int.MaxValue if maximal value is not specified</param>
        /// <returns></returns>
        internal AlarmAreaActionResult BuyTimeForAlarmArea(
            DB.AlarmArea alarmArea,
            ref int timeToBuy,
            out int remainingTime)
        {
            if (timeToBuy == -1
                && (!alarmArea.TimeBuyingMaxDuration.HasValue
                    && !alarmArea.TimeBuyingTotalMax.HasValue))
            {
                remainingTime = 0;
                return AlarmAreaActionResult.FailedDueError;
            }

            var currentTime = DateTime.UtcNow;
            var result = AlarmAreaActionResult.FailedDueError;
            var remaining = 0;

            var useAllTime = false;

            // if time to buy is -1 it means that this unset want to use all available time
            if (timeToBuy == -1)
            {
                useAllTime = true;

                timeToBuy = alarmArea.TimeBuyingMaxDuration.HasValue
                    ? alarmArea.TimeBuyingMaxDuration.Value
                    // ReSharper disable once PossibleInvalidOperationException, it is checked in first condition of this method
                    : alarmArea.TimeBuyingTotalMax.Value;
            }

            var tempTimeToBuy = timeToBuy;

            AlarmAreaTimeBuyingSettings timeBuyingInfo;

            _alarmAreaTimeBuyingStates.GetOrAddValue(
                alarmArea.IdAlarmArea,
                out timeBuyingInfo,
                key => new AlarmAreaTimeBuyingSettings(currentTime,
                    tempTimeToBuy,
                    tempTimeToBuy),
                (key, value, isNew) =>
                {
                    result =
                        value.BuyTimeForAlarmArea(
                            alarmArea,
                            isNew,
                            useAllTime,
                            currentTime,
                            ref tempTimeToBuy,
                            out remaining);
                });

            timeToBuy = tempTimeToBuy;

            // Return calculated remaining time
            remainingTime = remaining;

            return result;
        }

        /// <summary>
        /// This method reset total bought time for alarm area. Should be called from unset without time buying and from final set.
        /// </summary>
        /// <param name="idAlarmArea">Id of alarm area</param>
        internal bool ResetTimeBuyingForAlarmArea(Guid idAlarmArea)
        {
            var result = false;

            _alarmAreaTimeBuyingStates.Remove(
                idAlarmArea,
                (key, removed, value) =>
                {
                    result = removed;

                    if (!removed)
                        return;
                    
                    value.StopTimer();

                    _alarmAreaRegistryUpdater.Enqueue(
                        new RegistryRemoverForTimeBuying(idAlarmArea));

                });

            return result;
        }

        /// <summary>
        /// Cancel currently bought time and recalculate total bought time for alarm area. This method should be called during set request after time buying.
        /// </summary>
        /// <param name="idAlarmArea">Id of alarm area</param>
        /// <returns>true if alarm area was found</returns>
        internal bool TryCancelBoughtTime(Guid idAlarmArea)
        {
            var currentTime = DateTime.UtcNow;
            var result = false;

            _alarmAreaTimeBuyingStates.TryGetValue(
                idAlarmArea,
                (key, found, value) =>
                {
                    if (found)
                        result = value.TryCancelBoughtTime(
                            idAlarmArea,
                            currentTime);
                });

            return result;
        }

        private bool OnBoughtTimeExpired(TimerCarrier timer)
        {
            if (!(timer.Data is Guid))
                return true;

            AlarmAreaTimeBuyingSettings alarmAreaTimeBuyingSettings = null;
            var idAlarmArea = (Guid)timer.Data;

            _alarmAreaTimeBuyingStates.TryGetValue(
                idAlarmArea,
                (key, found, value) =>
                {
                    if (!found)
                        return;

                    value.OnBoughtTimeExpired();
                    alarmAreaTimeBuyingSettings = value;
                });

            if (alarmAreaTimeBuyingSettings == null)
                return true;

            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return true;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            if (alarmAreaController == null)
                return true;

            alarmAreaController.OnBoughtTimeExpired(
                alarmAreaTimeBuyingSettings.LastBoughtTime,
                alarmAreaTimeBuyingSettings.TotalBoughtTime);

            return true;
        }

        public void DeleteFromTimeBuyingRegistry(Guid idAlarmArea)
        {
            _alarmAreaRegistryUpdater.Enqueue(
                new RegistryRemoverForTimeBuying(idAlarmArea));
        }

        void SystemTime__timeChanged(DateTime param)
        {
            _alarmAreaTimeBuyingStates.ForEach((key, value) => value.OnSystemTimeChanged());
        }

        private void EnqueueTimeBuyingRegistrySetting(
            Guid idAlarmArea,
            AlarmAreaTimeBuyingSettings alarmAreaTimeBuyingSettings)
        {
            // Save into registry
            _alarmAreaRegistryUpdater.Enqueue(
                new RegistryUpdaterForTimeBuying(
                    idAlarmArea, 
                    alarmAreaTimeBuyingSettings));
        }

        #endregion

        /// <summary>
        /// Function to get currently used alarm area
        /// </summary>
        /// <param name="guidAlarmArea"></param>
        /// <returns></returns>
        public DB.AlarmArea GetAlarmArea(Guid guidAlarmArea)
        {
            AlarmAreaStateAndSettings aaStateAndSettings;
            if (!_objects.TryGetValue(guidAlarmArea, out aaStateAndSettings) || aaStateAndSettings == null)
                return null;

            return aaStateAndSettings.DbObject;
        }

        public IAlarmAreaStateAndSettings GetAlarmAreaSettings(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings aaStateAndSettings;
            if (!_objects.TryGetValue(idAlarmArea, out aaStateAndSettings))
                return null;

            return aaStateAndSettings;
        }

        public bool EnabledCrEventlogs(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogs();
            }

            return false;
        }

        public bool EnabledCrEventlogsAlarmAreaSet(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsAlarmAreaSet();
            }

            return false;
        }

        public bool EnabledCrEventlogsAlarmAreaUnconditionalSet(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsAlarmAreaUnconditionalSet();
            }

            return false;
        }

        public bool EnabledCrEventlogsAlarmAreaUnset(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsAlarmAreaUnset();
            }

            return false;
        }

        public bool EnabledCrEventlogsAlarmAreaAlarm(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsAlarmAreaAlarm();
            }

            return false;
        }

        public bool EnabledCrEventlogsAlarmAreaNormal(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsAlarmAreaNormal();
            }

            return false;
        }

        public bool EnabledCrEventlogsAlarmAreaAcknowledged(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsAlarmAreaAcknowledged();
            }

            return false;
        }

        public bool EnabledCrEventlogsSensorsAlarm(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsSensorsAlarm();
            }

            return false;
        }

        public bool EnabledCrEventlogsSensorsNormal(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsSensorsNormal();
            }

            return false;
        }

        public bool EnabledCrEventlogsSensorsAcknowledged(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.EnabledCrEventlogsSensorsAcknowledged();
            }

            return false;
        }

        public bool AddActivatedSensorDuringAlarmState(
            Guid idAlarmArea,
            Guid idInput)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.AddActivatedSensorDuringAlarmState(idInput);
            }

            return false;
        }

        public bool RemoveActivatedSensorDuringAlarmState(
            Guid idAlarmArea,
            Guid idInput)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.RemoveActivatedSensorDuringAlarmState(idInput);
            }

            return false;
        }

        public IEnumerable<Guid> GetActivatedSensorsDuringAlarmStateAndClear(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;
            if (_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings)
                    && alarmAreaStateAndSettings != null)
            {
                return alarmAreaStateAndSettings.GetActivatedSensorsDuringAlarmStateAndClear();
            }

            return null;
        }

        public int GetSensorId(
            Guid idAlarmArea,
            Guid idInput)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return 0;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            return
                alarmAreaController != null
                    ? alarmAreaController.GetSensorId(idInput)
                    : 0;
        }

        public SensorPurpose GetSensorPurpose(
            Guid idAlarmArea,
            Guid idInput)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return 0;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            return
                alarmAreaController != null
                    ? alarmAreaController.GetSensorPurpose(idInput)
                    : SensorPurpose.BurglaryAlarm;
        }

        public void AddAlarmAreaActivationEventHandler(
            Guid idAlarmArea,
            IAlarmAreaActivationEventHandler alarmAreaActivationEventHandler)
        {
            _objects.TryGetValue(
                idAlarmArea,
                (key,
                    found,
                    value) =>
                {
                    if (found)
                        value.AddAlarmAreaActivationEventHandler(alarmAreaActivationEventHandler);
                });
        }

        public void RemoveAlarmAreaActivationEventHandler(
            Guid idAlarmArea,
            IAlarmAreaActivationEventHandler alarmAreaActivationEventHandler)
        {
            _objects.TryGetValue(
                idAlarmArea,
                (key,
                    found,
                    value) =>
                {
                    if (found)
                        value.RemoveAlarmAreaActivationEventHandler(alarmAreaActivationEventHandler);
                });
        }

        public bool HasInactiveObjectForForcedTimeBuying(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return false;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            return
                alarmAreaController != null
                && alarmAreaController.HasInactiveObjectForForcedTimeBuying();
        }

        public bool IsTimeBuyingEnabled(Guid idAlarmArea)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return false;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            return
                alarmAreaController != null
                && alarmAreaController.IsTimeBuyingEnabled();
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.AlarmArea; }
        }

        protected override AlarmAreaStateAndSettings CreateNewStateAndSettingsObject(DB.AlarmArea dbObject)
        {
            return new AlarmAreaStateAndSettings(dbObject);
        }

        public AlarmAreaStates GetAlarmAreaStates(Guid idAlarmArea)
        {
            AlarmAreaStates result;

            _alarmAreaStates.TryGetValue(
                idAlarmArea,
                out result);

            return result;
        }

        protected override void OnRemoved(AlarmAreaStateAndSettings removedValue)
        {
            var idAlarmArea = removedValue.Id;

            DeleteFromAAToCRsReporting(idAlarmArea);

            EventsForCardReadersDispatcher.Singleton
                .RemoveAlarmAreaConfiguration(idAlarmArea);
        }

        public void SetSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType sensorBlockingType)
        {
            AlarmAreaStateAndSettings alarmAreaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out alarmAreaStateAndSettings))
            {
                return;
            }

            var alarmAreaController = alarmAreaStateAndSettings.AlarmAreaController;

            if (alarmAreaController != null)
                alarmAreaController.SetSensorBlockingType(
                    idInput,
                    sensorBlockingType);
        }

        public bool CheckUnsetRights(
            Guid idAlarmArea,
            Guid idPerson)
        {
            AlarmAreaStateAndSettings aaStateAndSettings;

            if (!_objects.TryGetValue(
                idAlarmArea,
                out aaStateAndSettings))
            {
                return false;
            }

            return aaStateAndSettings.CheckUnsetRights(idPerson);
        }

        public void SetAlarmAreaRestrictivePolicyForTimeBuying(bool alarmAreaRestrictivePolicyForTimeBuying)
        {
            AlarmAreaRestrictivePolicyForTimeBuying = alarmAreaRestrictivePolicyForTimeBuying;

            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CCU_REG_ALARM_AREA_RESTRICTIVE_POLICY_FOR_TIME_BUYING, alarmAreaRestrictivePolicyForTimeBuying, RegistryValueKind.DWord);
                    registryKey.Close();
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void GetAlarmAreaRestrictivePolicyForTimeBuying()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    var alarmAreaRestrictivePolicyForTimeBuying =
                        Convert.ToInt32(
                            registryKey.GetValue(
                                CCU_REG_ALARM_AREA_RESTRICTIVE_POLICY_FOR_TIME_BUYING));

                    AlarmAreaRestrictivePolicyForTimeBuying = alarmAreaRestrictivePolicyForTimeBuying != 0;

                    registryKey.Close();

                    return;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            AlarmAreaRestrictivePolicyForTimeBuying = true;
        }
    }
}