using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Server.CcuDataReplicator
{
    public class ModifiedObjectsCollection
    {
        public int Version { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public IEnumerable<object> Objects
        {
            get { return _objects; }
        }

        public bool IsEmpty
        {
            get { return _objects.Count == 0; }
        }

        private static Dictionary<ObjectType, Func<Guid,object>> CreateObjectLoaders()
        {
            return
                new Dictionary<ObjectType, Func< Guid, object>>
                {
                    {
                        ObjectType.AccessControlList,
                        guid => LoadObject(AccessControlLists.Singleton, guid)
                    },
                    {
                        ObjectType.ACLPerson,
                        guid => LoadObject(ACLPersons.Singleton, guid)
                    },
                    {
                        ObjectType.ACLSetting,
                        guid => LoadObject(ACLSettings.Singleton, guid)
                    },
                    {
                        ObjectType.ACLSettingAA,
                        guid => LoadObject(ACLSettingAAs.Singleton, guid)
                    },
                    {
                        ObjectType.Calendar,
                        guid => LoadObject(Calendars.Singleton, guid)
                    },
                    {
                        ObjectType.CalendarDateSetting,
                        guid => LoadObject(CalendarDateSettings.Singleton, guid)
                    },
                    {
                        ObjectType.Card,
                        guid => LoadObject(Cards.Singleton, guid)
                    },
                    {
                        ObjectType.CardSystem,
                        guid => LoadObject(CardSystems.Singleton, guid)
                    },
                    {
                        ObjectType.DailyPlan,
                        guid => LoadObject(DailyPlans.Singleton, guid)
                    },
                    {
                        ObjectType.DayInterval,
                        guid => LoadObject(DayIntervals.Singleton, guid)
                    },
                    {
                        ObjectType.DayType,
                        guid => LoadObject(DayTypes.Singleton, guid)
                    },
                    {
                        ObjectType.Input,
                        guid => LoadObject(Inputs.Singleton, guid)
                    },
                    {
                        ObjectType.Output,
                        guid => LoadObject(Outputs.Singleton, guid)
                    },
                    {
                        ObjectType.TimeZone,
                        guid => LoadObject(TimeZones.Singleton, guid)
                    },
                    {
                        ObjectType.TimeZoneDateSetting,
                        guid => LoadObject(TimeZoneDateSettings.Singleton, guid)
                    },
                    {
                        ObjectType.AlarmArea,
                        guid => LoadObject(AlarmAreas.Singleton, guid)
                    },
                    {
                        ObjectType.AACardReader,
                        guid => LoadObject(AACardReaders.Singleton, guid)
                    },
                    {
                        ObjectType.AccessZone,
                        guid => LoadObject(AccessZones.Singleton, guid)
                    },
                    {
                        ObjectType.CCU,
                        guid => LoadObject(CCUs.Singleton, guid)
                    },
                    {
                        ObjectType.AAInput,
                        guid => LoadObject(AAInputs.Singleton, guid)
                    },
                    {
                        ObjectType.AntiPassBackZone,
                        guid => LoadObject(AntiPassBackZones.Singleton, guid)
                    },
                    {
                        ObjectType.DoorEnvironment,
                        guid => LoadObject(DoorEnvironments.Singleton, guid)
                    },
                    {
                        ObjectType.CardReader,
                        guid => LoadObject(CardReaders.Singleton, guid)
                    },
                    {
                        ObjectType.SecurityDailyPlan,
                        guid => LoadObject(SecurityDailyPlans.Singleton, guid)
                    },
                    {
                        ObjectType.SecurityDayInterval,
                        guid => LoadObject(SecurityDayIntervals.Singleton, guid)
                    },
                    {
                        ObjectType.DCU,
                        guid => LoadObject(DCUs.Singleton, guid)
                    },
                    {
                        ObjectType.DevicesAlarmSetting,
                        guid => LoadObject(DevicesAlarmSettings.Singleton, guid)
                    },
                    {
                        ObjectType.SecurityTimeZone,
                        guid => LoadObject(SecurityTimeZones.Singleton, guid)
                    },
                    {
                        ObjectType.SecurityTimeZoneDateSetting,
                        guid => LoadObject(SecurityTimeZoneDateSettings.Singleton, guid)
                    },
                    {
                        ObjectType.MultiDoor,
                        guid => LoadObject(MultiDoors.Singleton, guid)
                    },
                    {
                        ObjectType.MultiDoorElement,
                        guid => LoadObject(MultiDoorElements.Singleton, guid)
                    },
                    {
                        ObjectType.AlarmTransmitter,
                        guid => LoadObject(AlarmTransmitters.Singleton, guid)
                    },
                    {
                        ObjectType.AlarmArc,
                        guid => LoadObject(AlarmArcs.Singleton, guid)
                    },
                    {
                        ObjectType.Person,
                        guid => LoadObject(Persons.Singleton, guid)
                    }
                };
        }

        public ModifiedObjectsCollection(int version, ObjectType objectType)
        {
            _objects = new LinkedList<object>();
            Version = version;
            ObjectType = objectType;
        }

        public void AddObjects(ICollection<Guid> objectsGuid)
        {
            if (objectsGuid == null || objectsGuid.Count <= 0)
                return;

            var objectLoader = GetObjectLoader();

            if (objectLoader == null)
                return;

            foreach (Guid objectGuid in objectsGuid)
            {
                var objectToSend = objectLoader(objectGuid);

                if (objectToSend == null)
                    continue;

                _objects.Add(objectToSend);
            }
        }

        private Func<Guid, object> GetObjectLoader()
        {
            lock (ObjectLoadersSync)
            {
                if (_objectLoaders == null)
                    _objectLoaders = CreateObjectLoaders();

                Func<Guid, object> objectLoader;

                _objectLoaders.TryGetValue(
                    ObjectType,
                    out objectLoader);

                return objectLoader;
            }
        }

        private static object LoadObject<TObject>(
            ITableORM<TObject> getTableOrm,
            Guid guid)
            where TObject : class
        {
            var obj = getTableOrm.GetById(guid);

            if (obj == null)
                return null;

            System.Reflection.MethodInfo miPrepareToSend =
                obj.GetType().GetMethod("PrepareToSend");

            if (miPrepareToSend != null)
                miPrepareToSend.Invoke(obj, null);

            if (obj.GetType() == typeof(CCU))
                ModifyCcuForNtpAddresses(obj as CCU);

            return obj;
        }

        private static volatile Dictionary<ObjectType, Func< Guid, object>> _objectLoaders;
        private static readonly object ObjectLoadersSync = new object();

        private readonly ICollection<object> _objects;

        private static void ModifyCcuForNtpAddresses(CCU ccu)
        {
            if (ccu == null)
                return;

            if (!ccu.InheritGeneralNtpSettings)
                return;

            string aditionalIpAddress =
                ServerGeneralOptionsDBs.Singleton.GetGeneralNtpIpAddress();

            if (string.IsNullOrEmpty(aditionalIpAddress))
                return;

            ccu.SNTPIpAddresses += aditionalIpAddress;
        }
    }
}