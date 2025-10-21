using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(837)]
    public class InputServerAlarm : ServerAlarm
    {
        private InputServerAlarm()
        {
            
        }

        public InputServerAlarm(
            CCU ccu,
            Input input,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            input)),
                    GetName(input),
                    ccu.Name,
                    string.Format(
                        "Input '{0}' is in alarm",
                        input.Name)))
        {

        }

        public static string GetName(Input input)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.Input_Alarm,
                input.Name);
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu,
            Input input)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            if (input.DCU != null)
            {
                parentObjects = parentObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            input.DCU.IdDCU,
                            ObjectType.DCU),
                        1));
            }

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            Input input)
        {
            var relatedObjects = GetParentObjects(
                idCcu,
                input);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        input.IdInput,
                        ObjectType.Input),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var input = Inputs.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return input != null
                ? input.AlarmPresentationGroup
                : null;
        }
    }
}
