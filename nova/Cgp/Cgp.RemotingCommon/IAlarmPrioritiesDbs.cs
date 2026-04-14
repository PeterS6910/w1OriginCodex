using System;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IAlarmPrioritiesDbs
    {
        System.Collections.Generic.ICollection<AlarmPriorityDatabase> List(out Exception error);
        void SaveListAlarmPriorities(System.Collections.Generic.ICollection<AlarmPriorityDatabase> List);
        IList<AlarmPriorityDatabase> GetAlarmTypesFromDatabase(IList<Contal.Cgp.Globals.AlarmType> wantedAlarmTypes);
        void SaveAlarmPrioritiesToDatabase(IList<AlarmPriorityDatabase> alarmPriorities);
        ObjectType? GetClosestParentObject(AlarmType alarmType);
        ObjectType? GetSecondClosestParentObject(AlarmType alarmType);
    }
}
