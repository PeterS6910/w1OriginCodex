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
    [LwSerialize(839)]
    public class OutputServerAlarm : ServerAlarm
    {
        private OutputServerAlarm()
        {

        }

        public OutputServerAlarm(
            CCU ccu,
            Output output,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            output)),
                    GetName(output),
                    ccu.Name,
                    string.Format(
                        "Output '{0}' is on",
                        output.Name)))
        {

        }

        public static string GetName(Output output)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.Output_Alarm,
                output.Name);
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu,
            Output output)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            if (output.DCU != null)
            {
                parentObjects = parentObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            output.DCU.IdDCU,
                            ObjectType.DCU),
                        1));
            }

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            Output output)
        {
            var relatedObjects = GetParentObjects(
                idCcu,
                output);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        output.IdOutput,
                        ObjectType.Output),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var output = Outputs.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return output != null
                ? output.AlarmPresentationGroup
                : null;
        }
    }
}
