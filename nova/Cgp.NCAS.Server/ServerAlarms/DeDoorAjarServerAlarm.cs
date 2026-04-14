using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Contal.CatCom;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(818)]
    public class DeDoorAjarServerAlarm : ServerAlarm
    {
        private DeDoorAjarServerAlarm()
        {
            
        }

        public DeDoorAjarServerAlarm(
            CCU ccu,
            DoorEnvironment doorEnvironment,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            doorEnvironment)),
                    GetName(doorEnvironment),
                    ccu.Name,
                     doorEnvironment.DCU != null
                            ? doorEnvironment.DCU.Name
                            : doorEnvironment.Name))
        {

        }

        public static string GetName(DoorEnvironment doorEnvironment)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.DoorEnvironment_DoorAjar,
                doorEnvironment.DCU != null
                    ? doorEnvironment.DCU.Name
                    : doorEnvironment.Name);
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu,
            DoorEnvironment doorEnvironment)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            if (doorEnvironment.DCU != null)
            {
                parentObjects = parentObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            doorEnvironment.DCU.IdDCU,
                            ObjectType.DCU),
                        1));
            }

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            DoorEnvironment doorEnvironment)
        {
            var relatedObjects = GetParentObjects(
                idCcu,
                doorEnvironment);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        doorEnvironment.IdDoorEnvironment,
                        ObjectType.DoorEnvironment),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return doorEnvironment != null && doorEnvironment.DoorAjarPresentationGroup != null
                ? doorEnvironment.DoorAjarPresentationGroup
                : DevicesAlarmSettings.Singleton.AlarmDeDoorAjarPresentationGroup();
        }

        public  static string GetDSMAlarmPerson(DateTime dtAlarmTime,  string  dcuName)
        {
            string personDescription = "";

                DateTime dateStart = dtAlarmTime.AddMinutes(-3);
                var filterSettings = new List<FilterSettings>
                {
                    new FilterSettings(
                        Eventlog.COLUMN_TYPE,
                        new List<string>
                        {
                            Eventlog.TYPEDSMNORMALACCESS,
                        },
                        ComparerModes.EQUALL),

                    new FilterSettings(
                        Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                        dateStart,
                        ComparerModes.EQUALLMORE),

                    new FilterSettings(
                        Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                        dtAlarmTime,
                        ComparerModes.EQUALLLESS),
                };

                var trim_dcuName = Regex.Replace(dcuName, @"\s", "");

                var dsmevents = Eventlogs.Singleton.SelectByCriteria(filterSettings);
                if (dsmevents != null)
                {
                    var dsmevents2 = dsmevents.ToList().OrderByDescending(d => d.EventlogDateTime);

                    foreach (var dsm in dsmevents2)
                    {
                        personDescription = GetDCUPersonFromDSMSources(dsm, trim_dcuName, dtAlarmTime);
                        if (!string.IsNullOrEmpty(personDescription))
                            break;
                    }
     
            }
            return personDescription;
        }
         
        private static  string GetDCUPersonFromDSMSources(Eventlog dsm, string trimDcuName, DateTime dtAlarmTime)
        {
            string personDescription = "";
            if (dsm != null && dsm.EventSources == null)
            {
                dsm = Eventlogs.Singleton.GetObjectById(dsm.IdEventlog);
            }

            DCU dcu = null;
            Person person = null;

            if (dsm.EventSources != null)
            {
                foreach (var eventsource in dsm.EventSources)
                {
                    if (dcu == null && CentralNameRegisters.Singleton.GetObjectTypeFromGuid(eventsource.EventSourceObjectGuid) == ObjectType.DCU)
                          dcu = DCUs.Singleton.GetObjectById(eventsource.EventSourceObjectGuid);

                    if (person == null && CentralNameRegisters.Singleton.GetObjectTypeFromGuid(eventsource.EventSourceObjectGuid) == ObjectType.Person)
                         person = Persons.Singleton.GetObjectById(eventsource.EventSourceObjectGuid);

                    if (dcu != null &&  person != null)
                    {
                        var trimName = Regex.Replace(dcu.Name, @"\s", "");
                        if (trimName == trimDcuName)
                        {
                            personDescription = $" ,{dtAlarmTime.ToShortDateString()} {dtAlarmTime.ToLongTimeString()}, {person.ToString()} ( Personal Id: {person.Identification ?? " - "} )";
                        }
                        break;
                    }
                }
            }
            return personDescription;
        }

        public  override string GetPresentationDescription()
        {
                string descDCU = ServerAlarmCore.Description;
                string person = "";
                int count = 0;
                while (count < 2 && string.IsNullOrEmpty(person))
                {
                    Thread.Sleep(8000); //wait to be written in the eventlog
                    person = GetDSMAlarmPerson(ServerAlarmCore.Alarm.CreatedDateTime, descDCU);
                    count++;
                }

                string template = $"Door environment '{descDCU}' is in door ajar {person}";
                return template;

        }
    }
}
