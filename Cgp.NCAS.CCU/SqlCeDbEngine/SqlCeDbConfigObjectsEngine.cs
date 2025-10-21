using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbConfigObjectsEngine
        : ASqlCeDbEngine
        , IConfigObjectsEngine
    {
        public ICardsStorage CardsStorage { get; private set; }
        public IApbzStorage ApbzStorage { get; private set; }
        public ICrEventlogEventsStorage CrEventlogEventsStorage { get; private set; }
        public IAlarmsStorage AlarmsStorage { get; private set; }
        public IPersonsStorage PersonsStorage { get; private set; }
        public IAclPersonsStorage AclPersonsStorage { get; private set; }
        public IAclSettingsStorage AclSettingsStorage { get; private set; }
        public IDoorEnvironmentsStorage DoorEnvironmentsStorage { get; private set; }
        public IAclSettingAAsStorage AclSettingAasStorage { get; private set; }
        public IAccessZonesStorage AccessZonesStorage { get; private set; }
        public IAaCardReadersStorage AaCardReadersStorage { get; private set; }

        private readonly ICollection<ISqlCeDbAccessorBase> _sqlCeDbAccessorsBase;
        private readonly SyncDictionary<ObjectType, ISqlCeDbAccessor> _sqlCeDbAccessors;

        private readonly EventHandlerGroup<DB.IDbObjectRemovalListener> _dbObjectRemovalHandlerGroup =
            new EventHandlerGroup<DB.IDbObjectRemovalListener>();

        private string _connectionString;

        public SqlCeDbConfigObjectsEngine()
        {
            InitDatabaseFile();

            _dbObjectRemovalHandlerGroup.Add(BlockedAlarmsManager.Singleton);
            _dbObjectRemovalHandlerGroup.Add(CatAlarmsManager.Singleton);
            _dbObjectRemovalHandlerGroup.Add(AlarmsManager.Singleton);

            SqlCeDbVersionHelper.Singleton.Init(this);

            _sqlCeDbAccessorsBase = new LinkedList<ISqlCeDbAccessorBase>();
            _sqlCeDbAccessors = new SyncDictionary<ObjectType, ISqlCeDbAccessor>();

            var dailyPlanAccessor = new SqlCeDbConfigObjectsAccessor<DB.DailyPlan>(
                this,
                ObjectType.DailyPlan,
                "IdDailyPlan",
                new ObjectCreatorFromSerializedData<DB.DailyPlan>(),
                DailyPlans.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.DailyPlan] = dailyPlanAccessor;
            _sqlCeDbAccessorsBase.Add(dailyPlanAccessor);

            var timeZoneAccessor = new SqlCeDbConfigObjectsAccessor<DB.TimeZone>(
                this,
                ObjectType.TimeZone,
                "IdTimeZone",
                new ObjectCreatorFromSerializedData<DB.TimeZone>(),
                TimeZones.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.TimeZone] = timeZoneAccessor;
            _sqlCeDbAccessorsBase.Add(timeZoneAccessor);

            var calendarAccessor = new SqlCeDbConfigObjectsAccessor<DB.Calendar>(
                this,
                ObjectType.Calendar,
                "IdCalendar",
                new ObjectCreatorFromSerializedData<DB.Calendar>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.Calendar] = calendarAccessor;
            _sqlCeDbAccessorsBase.Add(calendarAccessor);

            var calendarDateSetttingAccessor = new SqlCeDbConfigObjectsAccessor<DB.CalendarDateSetting>(
                this,
                ObjectType.CalendarDateSetting,
                "IdCalendarDateSetting",
                new ObjectCreatorFromSerializedData<DB.CalendarDateSetting>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.CalendarDateSetting] = calendarDateSetttingAccessor;
            _sqlCeDbAccessorsBase.Add(calendarDateSetttingAccessor);

            var timeZoneDateSettingAccessor = new SqlCeDbConfigObjectsAccessor<DB.TimeZoneDateSetting>(
                this,
                ObjectType.TimeZoneDateSetting,
                "IdTimeZoneDateSetting",
                new ObjectCreatorFromSerializedData<DB.TimeZoneDateSetting>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.TimeZoneDateSetting] = timeZoneDateSettingAccessor;
            _sqlCeDbAccessorsBase.Add(timeZoneDateSettingAccessor);

            var dayTypeAccessor = new SqlCeDbConfigObjectsAccessor<DB.DayType>(
                this,
                ObjectType.DayType,
                "IdDayType",
                new ObjectCreatorFromSerializedData<DB.DayType>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.DayType] = dayTypeAccessor;
            _sqlCeDbAccessorsBase.Add(dayTypeAccessor);

            var aclPersonAccessor = new SqlCeDbAclPersonsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.ACLPerson] = aclPersonAccessor;
            _sqlCeDbAccessorsBase.Add(aclPersonAccessor);

            AclPersonsStorage = aclPersonAccessor;

            var aclSettingAccessor = new SqlCeDbAclSettingsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.ACLSetting] = aclSettingAccessor;
            _sqlCeDbAccessorsBase.Add(aclSettingAccessor);

            AclSettingsStorage = aclSettingAccessor;

            var aclSettingAaAccessor = new SqlCeDbAclSettingAAsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.ACLSettingAA] = aclSettingAaAccessor;
            _sqlCeDbAccessorsBase.Add(aclSettingAaAccessor);

            AclSettingAasStorage = aclSettingAaAccessor;

            var cardsAccessor = new SqlCeDbCardsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.Card] = cardsAccessor;
            _sqlCeDbAccessorsBase.Add(cardsAccessor);

            CardsStorage = cardsAccessor;

            var cardSystemAccessor = new SqlCeDbConfigObjectsAccessor<DB.CardSystem>(
                this,
                ObjectType.CardSystem,
                "IdCardSystem",
                new ObjectCreatorFromSerializedData<DB.CardSystem>(),
                CardSystemData.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.CardSystem] = cardSystemAccessor;
            _sqlCeDbAccessorsBase.Add(cardSystemAccessor);

            var alarmAreaAccessor = new SqlCeDbConfigObjectsAccessor<DB.AlarmArea>(
                this,
                ObjectType.AlarmArea,
                "IdAlarmArea",
                new ObjectCreatorFromSerializedData<DB.AlarmArea>(),
                AlarmAreas.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AlarmArea] = alarmAreaAccessor;
            _sqlCeDbAccessorsBase.Add(alarmAreaAccessor);

            var aaCardReaderAccessor = new SqlCeDbAaCardReadersAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AACardReader] = aaCardReaderAccessor;
            _sqlCeDbAccessorsBase.Add(aaCardReaderAccessor);

            AaCardReadersStorage = aaCardReaderAccessor;

            var accessZoneAccessor = new SqlCeDbAccessZonesAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AccessZone] = accessZoneAccessor;
            _sqlCeDbAccessorsBase.Add(accessZoneAccessor);

            AccessZonesStorage = accessZoneAccessor;

            var inputAccessor = new SqlCeDbConfigObjectsAccessor<DB.Input>(
                this,
                ObjectType.Input,
                "IdInput",
                new ObjectCreatorFromSerializedData<DB.Input>(),
                Inputs.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.Input] = inputAccessor;
            _sqlCeDbAccessorsBase.Add(inputAccessor);

            var outputAccessor = new SqlCeDbConfigObjectsAccessor<DB.Output>(
                this,
                ObjectType.Output,
                "IdOutput",
                new ObjectCreatorFromSerializedData<DB.Output>(),
                Outputs.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.Output] = outputAccessor;
            _sqlCeDbAccessorsBase.Add(outputAccessor);

            var ccuAccessor = new SqlCeDbConfigObjectsAccessor<DB.CCU>(
                this,
                ObjectType.CCU,
                "IdCCU",
                new ObjectCreatorFromSerializedData<DB.CCU>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.CCU] = ccuAccessor;
            _sqlCeDbAccessorsBase.Add(ccuAccessor);

            var aaInputAccessor = new SqlCeDbConfigObjectsAccessor<DB.AAInput>(
                this,
                ObjectType.AAInput,
                "IdAAInput",
                new ObjectCreatorFromSerializedData<DB.AAInput>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AAInput] = aaInputAccessor;
            _sqlCeDbAccessorsBase.Add(aaInputAccessor);

            var doorEnvironmentAccessor = new SqlCeDbDoorEnvironmentsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.DoorEnvironment] = doorEnvironmentAccessor;
            _sqlCeDbAccessorsBase.Add(doorEnvironmentAccessor);

            DoorEnvironmentsStorage = doorEnvironmentAccessor;

            var cardReaderAccessor = new SqlCeDbConfigObjectsAccessor<DB.CardReader>(
                this,
                ObjectType.CardReader,
                "IdCardReader",
                new ObjectCreatorFromSerializedData<DB.CardReader>(),
                CardReaders.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.CardReader] = cardReaderAccessor;
            _sqlCeDbAccessorsBase.Add(cardReaderAccessor);

            var securityDailyPlanAccessor = new SqlCeDbConfigObjectsAccessor<DB.SecurityDailyPlan>(
                this,
                ObjectType.SecurityDailyPlan,
                "IdSecurityDailyPlan",
                new ObjectCreatorFromSerializedData<DB.SecurityDailyPlan>(),
                SecurityDailyPlans.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.SecurityDailyPlan] = securityDailyPlanAccessor;
            _sqlCeDbAccessorsBase.Add(securityDailyPlanAccessor);

            var securityDayIntervalAccessor = new SqlCeDbConfigObjectsAccessor<DB.SecurityDayInterval>(
                this,
                ObjectType.SecurityDayInterval,
                "IdInterval",
                new ObjectCreatorFromSerializedData<DB.SecurityDayInterval>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.SecurityDayInterval] = securityDayIntervalAccessor;
            _sqlCeDbAccessorsBase.Add(securityDayIntervalAccessor);

            var dcuAccessor = new SqlCeDbConfigObjectsAccessor<DB.DCU>(
                this,
                ObjectType.DCU,
                "IdDCU",
                new ObjectCreatorFromSerializedData<DB.DCU>(),
                DCUs.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.DCU] = dcuAccessor;
            _sqlCeDbAccessorsBase.Add(dcuAccessor);

            var devicesAlarmSettingAccessor = new SqlCeDbConfigObjectsAccessor<DB.DevicesAlarmSetting>(
                this,
                ObjectType.DevicesAlarmSetting,
                "IdDevicesAlarmSetting",
                new ObjectCreatorFromSerializedData<DB.DevicesAlarmSetting>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.DevicesAlarmSetting] = devicesAlarmSettingAccessor;
            _sqlCeDbAccessorsBase.Add(devicesAlarmSettingAccessor);

            var securityTimeZoneAccessor = new SqlCeDbConfigObjectsAccessor<DB.SecurityTimeZone>(
                this,
                ObjectType.SecurityTimeZone,
                "IdSecurityTimeZone",
                new ObjectCreatorFromSerializedData<DB.SecurityTimeZone>(),
                SecurityTimeZones.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.SecurityTimeZone] = securityTimeZoneAccessor;
            _sqlCeDbAccessorsBase.Add(securityTimeZoneAccessor);

            var securityTimeZoneDateSettingAccessor = new SqlCeDbConfigObjectsAccessor<DB.SecurityTimeZoneDateSetting>(
                this,
                ObjectType.SecurityTimeZoneDateSetting,
                "IdSecurityTimeZoneDateSetting",
                new ObjectCreatorFromSerializedData<DB.SecurityTimeZoneDateSetting>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.SecurityTimeZoneDateSetting] = securityTimeZoneDateSettingAccessor;
            _sqlCeDbAccessorsBase.Add(securityTimeZoneDateSettingAccessor);

            var antiPassBackZoneAccessor = new SqlCeDbConfigObjectsAccessor<DB.AntiPassBackZone>(
                this,
                ObjectType.AntiPassBackZone,
                "IdAntiPassBackZone",
                new ObjectCreatorFromSerializedData<DB.AntiPassBackZone>(),
                AntiPassBackZones.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AntiPassBackZone] = antiPassBackZoneAccessor;
            _sqlCeDbAccessorsBase.Add(antiPassBackZoneAccessor);

            var multiDoorAccessor = new SqlCeDbConfigObjectsAccessor<DB.MultiDoor>(
                this,
                ObjectType.MultiDoor,
                "IdMultiDoor",
                new ObjectCreatorFromSerializedData<DB.MultiDoor>(),
                MultiDoors.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.MultiDoor] = multiDoorAccessor;
            _sqlCeDbAccessorsBase.Add(multiDoorAccessor);

            var multiDoorElementAccessor = new SqlCeDbMultiDoorElementsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.MultiDoorElement] = multiDoorElementAccessor;
            _sqlCeDbAccessorsBase.Add(multiDoorElementAccessor);

            var alarmTransmitterAccessor = new SqlCeDbConfigObjectsAccessor<DB.AlarmTransmitter>(
                this,
                ObjectType.AlarmTransmitter,
                "IdAlarmTransmitter",
                new ObjectCreatorFromSerializedData<DB.AlarmTransmitter>(),
                AlarmTransmitters.Singleton,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AlarmTransmitter] = alarmTransmitterAccessor;
            _sqlCeDbAccessorsBase.Add(alarmTransmitterAccessor);

            var alarmArcAccessor = new SqlCeDbConfigObjectsAccessor<DB.AlarmArc>(
                this,
                ObjectType.AlarmArc,
                "IdAlarmArc",
                new ObjectCreatorFromSerializedData<DB.AlarmArc>(),
                null,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.AlarmArc] = alarmArcAccessor;
            _sqlCeDbAccessorsBase.Add(alarmArcAccessor);

            var personsAccessor = new SqlCeDbPersonsAccessor(
                this,
                _dbObjectRemovalHandlerGroup);

            _sqlCeDbAccessors[ObjectType.Person] = personsAccessor;
            _sqlCeDbAccessorsBase.Add(personsAccessor);

            PersonsStorage = personsAccessor;

            var apbzStorage = new SqlCeDbApbzStorage(this);

            _sqlCeDbAccessorsBase.Add(apbzStorage);

            ApbzStorage = apbzStorage;

            var crEventlogEventsStorage = new SqlCeDbCrEventlogEventsStorage(this);

            _sqlCeDbAccessorsBase.Add(crEventlogEventsStorage);

            CrEventlogEventsStorage = crEventlogEventsStorage;

            var alarmsStorage = new SqlCeDbAlarmsStorage(this);

            _sqlCeDbAccessorsBase.Add(alarmsStorage);

            AlarmsStorage = alarmsStorage;
        }

        private void InitDatabaseFile()
        {
            var databaseDirectoryPath = Path.Combine(
                CcuCore.Singleton.RootPath,
                Database.DATABASE_DIRECTORY_NAME);

            if (!Directory.Exists(databaseDirectoryPath))
                Directory.CreateDirectory(databaseDirectoryPath);

            var databaseFilePath = Path.Combine(
                databaseDirectoryPath,
                "SqlCeDb.sdf");

            _connectionString = string.Format(
                "Data source = {0}; Max Database Size = 512;",
                databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                var sqlCeEnginge = new SqlCeEngine(_connectionString);

                sqlCeEnginge.CreateDatabase();
                sqlCeEnginge.Dispose();
            }
        }

        protected override SqlCeConnection CreateConnection()
        {
            return new SqlCeConnection(_connectionString);
        }

        public void ReadObjectsFromFiles()
        {
            lock (_sqlCeDbAccessorsBase)
                foreach (var sqlCeDbAccessorBase in _sqlCeDbAccessorsBase)
                {
                    sqlCeDbAccessorBase.Load();
                }
        }

        public void SaveObjectsToDatabase(ObjectType objectType, IEnumerable<DB.IDbObject> objectsToSave)
        {
            if (objectsToSave == null)
                return;

            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                sqlCeDbAccessor.SaveToDatabase(
                    objectsToSave,
                    true);
            }
        }

        public void SaveMaxObjectTypeVersion(ObjectType objectType, int version)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                sqlCeDbAccessor.SaveVersion(version);
            }
        }

        public object GetFromDatabase(ObjectType objectType, Guid guid)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                return sqlCeDbAccessor.GetFromDatabase(guid);
            }

            return null;
        }

        public bool Exists(ObjectType objectType, Guid guid)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                return sqlCeDbAccessor.Exists(guid);
            }

            return false;
        }

        public MaximumVersionAndIds GetMaximumVersionAndIds(ObjectType objectType)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                return sqlCeDbAccessor.GetMaximumVersionAndIds();
            }

            return new MaximumVersionAndIds(
                -1,
                null);
        }

        public void DeleteFromDatabase(ObjectType objectType, IEnumerable<Guid> objectGuids)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                foreach (var objectGuid in objectGuids)
                {
                    sqlCeDbAccessor.DeleteFromDatabase(objectGuid);
                }
            }
        }

        public void DeleteAllFromDatabase()
        {
            lock (_sqlCeDbAccessorsBase)
                foreach (var sqlCeDbAccessorBase in _sqlCeDbAccessorsBase)
                {
                    sqlCeDbAccessorBase.DeleteAllFromDatabase();
                }
        }

        public void DeleteAllFromDatabase(ObjectType objectType)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                sqlCeDbAccessor.DeleteAllFromDatabase();
            }
        }

        public ICollection<Guid> GetPrimaryKeysForObjectType(ObjectType objectType)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                return sqlCeDbAccessor.GetPrimaryKeys();
            }

            return null;
        }

        public bool ContainsAnyObjects(ObjectType objectType)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                return sqlCeDbAccessor.ContainsAnyObjects();
            }

            return false;
        }

        public ICollection<Guid> GetIdsOfRecentlySavedObjects(ObjectType objectType)
        {
            ISqlCeDbAccessor sqlCeDbAccessor;

            if (!_sqlCeDbAccessors.TryGetValue(
                objectType,
                out sqlCeDbAccessor))
            {
                return null;
            }

            return sqlCeDbAccessor.GetIdsOfRecentlySavedObjects();
        }

        public void OnApplyChangesDone()
        {
            _sqlCeDbAccessors.ForEach(
                (key, value) =>
                    value.OnApplyChangesDone());
        }
    }
}
