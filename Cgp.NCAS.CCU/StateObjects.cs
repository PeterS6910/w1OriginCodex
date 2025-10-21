using System;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public static class StateObjects
    {
        private static readonly SyncDictionary<ObjectType, Func<Guid, State>> _getStateForObjectType;

        static StateObjects()
        {
            _getStateForObjectType = new SyncDictionary<ObjectType, Func<Guid, State>>
            {
                { ObjectType.Input, idInput => Inputs.Singleton.GetInputLogicalState(idInput)},
                { ObjectType.Output, idOutput => Outputs.Singleton.GetOutputState(idOutput)},
                { ObjectType.TimeZone, idTimeZone => TimeZones.Singleton.GetActualState(idTimeZone)},
                { ObjectType.DailyPlan, idDailyPlan => DailyPlans.Singleton.GetActualState(idDailyPlan)},
                { ObjectType.SecurityTimeZone, idSecurityTimeZone => SecurityTimeZones.Singleton.GetActualState(idSecurityTimeZone)},
                { ObjectType.SecurityDailyPlan, idSecurityDailyPlan => SecurityDailyPlans.Singleton.GetActualState(idSecurityDailyPlan)},
            };
        }

        public static State GetObjectState(
            ObjectType objectType,
            Guid idObject)
        {
            Func<Guid, State> stateGetter;

            return _getStateForObjectType.TryGetValue(
                objectType,
                out stateGetter)
                ? stateGetter(idObject)
                : State.Unknown;
        }
    }
}
