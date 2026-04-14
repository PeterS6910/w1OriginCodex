using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Data;

using Calendar = Contal.Cgp.Server.Beans.Calendar;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class NcasDbs : ASingleton<NcasDbs>
    {
        private static readonly IDictionary<ObjectType, ITableORM>
            _objectTypeTablesORM;

        static NcasDbs()
        {
            _objectTypeTablesORM =
                new SyncDictionary<ObjectType, ITableORM>
                {
                    {
                        ObjectType.AACardReader,
                        AACardReaders.Singleton
                    },
                    {
                        ObjectType.AAInput,
                        AAInputs.Singleton
                    },
                    {
                        ObjectType.AccessControlList,
                        AccessControlLists.Singleton
                    },
                    {
                        ObjectType.AccessZone,
                        AccessZones.Singleton
                    },
                    {
                        ObjectType.ACLGroup,
                        ACLGroups.Singleton
                    },
                    {
                        ObjectType.ACLPerson,
                        ACLPersons.Singleton
                    },
                    {
                        ObjectType.ACLSettingAA,
                        ACLSettingAAs.Singleton
                    },
                    {
                        ObjectType.ACLSetting,
                        ACLSettings.Singleton
                    },
                    {
                        ObjectType.AlarmArea,
                        AlarmAreas.Singleton
                    },
                    {
                        ObjectType.CardReader,
                        CardReaders.Singleton
                    },
                    {
                        ObjectType.LprCamera,
                        LprCameras.Singleton
                    },
                    {
                        ObjectType.CCU,
                        CCUs.Singleton
                    },
                    {
                        ObjectType.DCU,
                        DCUs.Singleton
                    },
                    {
                        ObjectType.DevicesAlarmSetting,
                        DevicesAlarmSettings.Singleton
                    },
                    {
                        ObjectType.DoorEnvironment,
                        DoorEnvironments.Singleton
                    },
                    {
                        ObjectType.GraphicSymbol,
                        GraphicSymbols.Singleton
                    },
                    {
                        ObjectType.GraphicSymbolRawData,
                        GraphicSymbolRawDatas.Singleton
                    },
                    {
                        ObjectType.GraphicSymbolTemplate,
                        GraphicSymbolTemplates.Singleton
                    },
                    {
                        ObjectType.Input,
                        Inputs.Singleton
                    },
                    {
                        ObjectType.Output,
                        Outputs.Singleton
                    },
                    {
                        ObjectType.Scene,
                        Scenes.Singleton
                    },
                    {
                        ObjectType.SecurityDailyPlan,
                        SecurityDailyPlans.Singleton
                    },
                    {
                        ObjectType.SecurityDayInterval,
                        SecurityDayIntervals.Singleton
                    },
                    {
                        ObjectType.SecurityTimeZoneDateSetting,
                        SecurityTimeZoneDateSettings.Singleton
                    },
                    {
                        ObjectType.SecurityTimeZone,
                        SecurityTimeZones.Singleton
                    },
                    {
                        ObjectType.AntiPassBackZone,
                        AntiPassBackZones.Singleton
                    },
                    {
                        ObjectType.MultiDoor,
                        MultiDoors.Singleton
                    },
                    {
                        ObjectType.MultiDoorElement,
                        MultiDoorElements.Singleton
                    },
                    {
                        ObjectType.Floor,
                        Floors.Singleton
                    },
                    {
                        ObjectType.AlarmTransmitter,
                        AlarmTransmitters.Singleton
                    },
                    {
                        ObjectType.AlarmArc,
                        AlarmArcs.Singleton
                    }
                };
        }

        private NcasDbs() : base(null)
        {
        }

        public static ITableORM GetTableOrm(ObjectType objectType)
        {
            ITableORM result;

            _objectTypeTablesORM.TryGetValue(
                objectType,
                out result);

            return result;
        }

        public static IEnumerable<AOrmObject> GetDirectReferences(AOrmObject objRef)
        {
            var person = objRef as Person;

            if (person != null)
                return PersonDirectReferences(person);

            var presentationGroup = objRef as PresentationGroup;

            if (presentationGroup != null)
            {
                var alarmTransmitter = AlarmTransmitters.Singleton.GetAlarmTransmitterForPresentationGroup(
                    presentationGroup);

                return alarmTransmitter != null
                    ? Enumerable.Repeat(
                        (AOrmObject) alarmTransmitter,
                        1)
                    : null;
            }

            return null;
        }

        private static IEnumerable<AOrmObject> PersonDirectReferences(Person person)
        {
            var acls = ACLPersons.Singleton.GetAclForPerson(person);

            if (acls != null)
                foreach (var accessControlList in acls)
                    yield return accessControlList;

            Exception err;
            var accessZones = AccessZones.Singleton.GetAccessZonesByPerson(person.IdPerson, out err);

            foreach (var accessZone in accessZones)
            {
                AccessZones.ReadCardReaderObject(accessZone);
                yield return accessZone.CardReaderObject;

                yield return accessZone.TimeZone;
            }
        }

        public ICollection<AOrmObject> GetReferencedByObjects(AOrmObject objRef)
        {
            try
            {
                var calendar = objRef as Calendar;

                if (calendar != null)
                    return CalendarReferencedBy(calendar);

                var presentationGroup = objRef as PresentationGroup;

                if (presentationGroup != null)
                    return PresentationGroupReferencedBy(presentationGroup);

                var timeZone = objRef as TimeZone;

                if (timeZone != null)
                    return TimeZoneReferencedBy(timeZone);

                var dailyPlan = objRef as DailyPlan;

                if (dailyPlan != null)
                    return DailyPlanReferencedBy(dailyPlan);

                var globalAlarmInstruction = objRef as GlobalAlarmInstruction;

                if (globalAlarmInstruction != null)
                    return GlobalAlarmInstructionReferencedBy(globalAlarmInstruction);

                var dayType = objRef as DayType;

                if (dayType != null)
                    return DayTypeReferencedBy(dayType);

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static ICollection<AOrmObject> DayTypeReferencedBy(DayType dayType)
        {
            ICollection<SecurityTimeZone> stzList = 
                SecurityTimeZoneDateSettings.Singleton.GetSecurityTimeZonesFromDateSettingForDayType(dayType);

            ICollection<AOrmObject> result = new LinkedList<AOrmObject>();

            if (stzList != null)
                foreach (SecurityTimeZone stz in stzList)
                    result.Add(SecurityTimeZones.Singleton.GetById(stz.IdSecurityTimeZone));

            return result;
        }

        private static ICollection<AOrmObject> CalendarReferencedBy(Calendar calendar)
        {
            //SecurityTimeZone
            ICollection<SecurityTimeZone> stzList = 
                SecurityTimeZones.Singleton.GetStzForCalendar(calendar);

            if (stzList == null)
                return null;

            ICollection<AOrmObject> result = new LinkedList<AOrmObject>();

            foreach (SecurityTimeZone stz in stzList)
                result.Add(SecurityTimeZones.Singleton.GetById(stz.IdSecurityTimeZone));

            return result;
        }

        private static ICollection<AOrmObject> 
            PresentationGroupReferencedBy(PresentationGroup presentationGroup)
        {
            ICollection<AOrmObject> result = new LinkedList<AOrmObject>();

            ICollection<Output> outputsList = 
                Outputs.Singleton.GetOutputsForPg(presentationGroup);

            if (outputsList != null)
                foreach (Output output in outputsList)
                    result.Add(Outputs.Singleton.GetById(output.IdOutput));

            ICollection<Input> inputsList = 
                Inputs.Singleton.GetInputsForPg(presentationGroup);

            if (inputsList != null)
                foreach (Input input in inputsList)
                    result.Add(Inputs.Singleton.GetById(input.IdInput));

            ICollection<DCU> dcus =
                DCUs.Singleton.GetDcusForPg(presentationGroup);

            if (dcus != null)
                foreach (var dcu in dcus)
                    result.Add(
                        DCUs.Singleton.GetById(dcu.IdDCU));

            ICollection<CCU> ccus =
                CCUs.Singleton.GetCcusForPg(presentationGroup);

            if (ccus != null)
                foreach (var ccu in ccus)
                    result.Add(
                        CCUs.Singleton.GetById(ccu.IdCCU));

            ICollection<DoorEnvironment> doorEnvironmentsList = 
                DoorEnvironments.Singleton.GetDoorEnvironmentsForPg(presentationGroup);

            if (doorEnvironmentsList != null)
                foreach (DoorEnvironment doorEnvironment in doorEnvironmentsList)
                    result.Add(
                        DoorEnvironments.Singleton.GetById(doorEnvironment.IdDoorEnvironment));

            ICollection<AlarmArea> alarmAreasList = 
                AlarmAreas.Singleton.GetAlarmAreasForPg(presentationGroup);

            if (alarmAreasList != null)
                foreach (AlarmArea alarmArea in alarmAreasList)
                    result.Add(AlarmAreas.Singleton.GetById(alarmArea.IdAlarmArea));

            ICollection<DevicesAlarmSetting> devicesAlarmSettingsList =
                DevicesAlarmSettings.Singleton.GetDevicesAlarmSettingsForPg(presentationGroup);

            if (devicesAlarmSettingsList != null)
                foreach (DevicesAlarmSetting devicesAlarmSetting in devicesAlarmSettingsList)
                    result.Add(
                        DevicesAlarmSettings.Singleton.GetById(devicesAlarmSetting.IdDevicesAlarmSetting));

            return result.Count > 0 ? result : null;
        }

        private static ICollection<AOrmObject> TimeZoneReferencedBy(TimeZone timeZone)
        {
            ICollection<AOrmObject> result = new LinkedList<AOrmObject>();

            ICollection<AccessControlList> aclList = 
                ACLSettings.Singleton.GetAclForTimeZone(timeZone);

            if (aclList != null)
                foreach (AccessControlList acl in aclList)
                    result.Add(AccessControlLists.Singleton.GetById(acl.IdAccessControlList));

            ICollection<Person> personsList = 
                AccessZones.Singleton.GetPersonForTimeZone(timeZone);

            if (personsList != null)
                foreach (Person person in personsList)
                    result.Add(Persons.Singleton.GetById(person.IdPerson));

            ICollection<AlarmArea> alarmAreasList = 
                AlarmAreas.Singleton.UsedLikeOnOffObject(timeZone);

            if (alarmAreasList != null)
                foreach (AlarmArea alarmArea in alarmAreasList)
                    result.Add(AlarmAreas.Singleton.GetById(alarmArea.IdAlarmArea));

            ICollection<Input> inputsList = 
                Inputs.Singleton.UsedLikeOnOffObject(timeZone);

            if (inputsList != null)
                foreach (Input input in inputsList)
                    result.Add(Inputs.Singleton.GetById(input.IdInput));

            ICollection<Output> outputsList = 
                Outputs.Singleton.UsedLikeOnOffObject(timeZone);

            if (outputsList != null)
                foreach (Output output in outputsList)
                    result.Add(Outputs.Singleton.GetById(output.IdOutput));

            var multiDoorElements = MultiDoorElements.Singleton.GetMultiDoorElementsForOnOffObject(timeZone);
            if (multiDoorElements != null)
            {
                foreach (var multiDoorElementProxy in multiDoorElements)
                {
                    result.Add(MultiDoorElements.Singleton.GetById(multiDoorElementProxy.IdMultiDoorElement));
                }
            }

            return result.Count > 0 ? result : null;
        }


        private static ICollection<AOrmObject> DailyPlanReferencedBy(DailyPlan dailyPlan)
        {
            ICollection<AOrmObject> result = new LinkedList<AOrmObject>();

            ICollection<AlarmArea> alarmAreasList =
                AlarmAreas.Singleton.UsedLikeOnOffObject(dailyPlan);

            if (alarmAreasList != null)
                foreach (AlarmArea alarmArea in alarmAreasList)
                    result.Add(AlarmAreas.Singleton.GetById(alarmArea.IdAlarmArea));

            ICollection<Input> inputsList = 
                Inputs.Singleton.UsedLikeOnOffObject(dailyPlan);

            if (inputsList != null)
                foreach (Input input in inputsList)
                    result.Add(Inputs.Singleton.GetById(input.IdInput));

            ICollection<Output> outputsList = 
                Outputs.Singleton.UsedLikeOnOffObject(dailyPlan);

            if (outputsList != null)
                foreach (Output output in outputsList)
                    result.Add(Outputs.Singleton.GetById(output.IdOutput));

            ICollection<CardReader> cardReaderList = 
                CardReaders.Singleton.UsedLikeOnOffObject(dailyPlan);

            if (cardReaderList != null)
                foreach (CardReader cardReader in cardReaderList)
                    result.Add(CardReaders.Singleton.GetById(cardReader.IdCardReader));

            var multiDoorElements = MultiDoorElements.Singleton.GetMultiDoorElementsForOnOffObject(dailyPlan);
            if (multiDoorElements != null)
            {
                foreach (var multiDoorElementProxy in multiDoorElements)
                {
                    result.Add(MultiDoorElements.Singleton.GetById(multiDoorElementProxy.IdMultiDoorElement));
                }
            }

            return result.Count > 0 ? result : null;
        }

        private static ICollection<AOrmObject> 
            GlobalAlarmInstructionReferencedBy(GlobalAlarmInstruction globalAlarmInstruction)
        {
            if (globalAlarmInstruction == null)
                return null;

            ICollection<AOrmObject> result = new LinkedList<AOrmObject>();

            ICollection<Guid> cardReaderGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.CardReader);

            if (cardReaderGuids != null)
                foreach (Guid crGuid in cardReaderGuids)
                {
                    CardReader cardReader = CardReaders.Singleton.GetById(crGuid);

                    if (cardReader != null)
                        result.Add(cardReader);
                }

            ICollection<Guid> ccuGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.CCU);

            if (ccuGuids != null)
                foreach (Guid ccuGuid in ccuGuids)
                {
                    CCU ccu = CCUs.Singleton.GetById(ccuGuid);

                    if (ccu != null)
                        result.Add(ccu);
                }

            ICollection<Guid> dcuGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.DCU);

            if (dcuGuids != null)
                foreach (Guid dcuGuid in dcuGuids)
                {
                    DCU dcu = DCUs.Singleton.GetById(dcuGuid);

                    if (dcu != null)
                        result.Add(dcu);
                }

            ICollection<Guid> inputsGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.Input);

            if (inputsGuids != null)
                foreach (Guid inputGuid in inputsGuids)
                {
                    Input input = Inputs.Singleton.GetById(inputGuid);

                    if (input != null)
                        result.Add(input);
                }

            ICollection<Guid> outputsGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.Output);

            if (outputsGuids != null)
                foreach (Guid outputGuid in outputsGuids)
                {
                    Output output = Outputs.Singleton.GetById(outputGuid);

                    if (output != null)
                        result.Add(output);
                }

            ICollection<Guid> doorEnvironmentGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.DoorEnvironment);

            if (doorEnvironmentGuids != null)
                foreach (Guid doorEnvironmentGuid in doorEnvironmentGuids)
                {
                    DoorEnvironment doorEnvironment =
                        DoorEnvironments.Singleton.GetById(doorEnvironmentGuid);

                    if (doorEnvironment != null)
                        result.Add(doorEnvironment);
                }

            ICollection<Guid> alarmAreaGuids = 
                RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(
                    globalAlarmInstruction.IdGlobalAlarmInstruction, 
                    ObjectType.AlarmArea);

            if (alarmAreaGuids != null)
                foreach (Guid alarmAreaGuid in alarmAreaGuids)
                {
                    AlarmArea alarmArea = AlarmAreas.Singleton.GetById(alarmAreaGuid);

                    if (alarmArea != null)
                        result.Add(alarmArea);
                }

            return result.Count > 0 ? result : null;
        }
    }
}
