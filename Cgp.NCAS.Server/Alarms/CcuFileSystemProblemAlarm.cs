using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(730)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class CcuFileSystemProblemAlarm : Alarm, ICreateServerAlarm
    {
        public string FileOperation { get; private set; }

        private CcuFileSystemProblemAlarm()
        {

        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var fileNameAlarmParameter = AlarmKey.Parameters != null
                ? AlarmKey.Parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.FileName)
                : null;

            var fileName = fileNameAlarmParameter != null
                ? fileNameAlarmParameter.Value
                : string.Empty;

            return new ServerAlarm(
                new ServerAlarmCore(
                    idCcu,
                    new Alarm(
                        Id,
                        AlarmKey,
                        CreatedDateTime,
                        AlarmState,
                        IsAcknowledged,
                        IsBlockedGeneral,
                        IsBlockedIndividual),
                    GetName(
                        ccu,
                        fileName),
                    ccu.Name,
                    string.Format(
                        "CCU filesystem problem, fileName: {0}, file operation {1}",
                        fileName,
                        FileOperation)));
        }

        public static string GetName(
            CCU ccu,
            string fileName)
        {
            return string.Format(
                "{0} : {1} : {2}",
                AlarmType.CCU_FilesystemProblem,
                ccu.ToString(),
                fileName);
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);
        }
    }
}
