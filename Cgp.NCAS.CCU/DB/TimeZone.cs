using System;
using System.Collections.Generic;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(210)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class TimeZone : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdTimeZone { get; set; }
        private Guid _guidCalendar = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidCalendar { get { return _guidCalendar; } set { _guidCalendar = value; } }
        private List<Guid> _guidDateSettings = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidDateSettings { get { return _guidDateSettings; } set { _guidDateSettings = value; } }

        public static State CurrentState(Guid guidTimeZone)
        {
            var timeZone = 
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.TimeZone, 
                    guidTimeZone) as TimeZone;

            return timeZone != null
                ? (timeZone.IsOn(CcuCore.LocalTime)
                    ? State.On
                    : State.Off)
                : State.Off;
        }

        protected virtual bool IsOn(DateTime dateTime)
        {
            if (GuidDateSettings == null)
                return false;

            List<Guid> guidDailyPlanList = GetGuidsActualDailyPlans(dateTime);

            foreach (Guid idDailyPlan in guidDailyPlanList)
                if (DailyPlans.Singleton.GetActualState(idDailyPlan) == State.On)
                    return true;

            return false;
        }

        public List<Guid> GetGuidsActualDailyPlans(DateTime dateTime)
        {
            List<Guid> dailyPlans = new List<Guid>();
            List<Guid> explicitDailyPlans = new List<Guid>();

            if (GuidDateSettings == null ||
                    GuidDateSettings.Count <= 0)
                return explicitDailyPlans.Count > 0
                    ? explicitDailyPlans
                    : dailyPlans;

            foreach (Guid guidDateSetting in GuidDateSettings)
            {
                TimeZoneDateSetting dateSetting = 
                    Database.ConfigObjectsEngine.GetFromDatabase(
                        ObjectType.TimeZoneDateSetting, 
                        guidDateSetting) 
                    as TimeZoneDateSetting;

                if (dateSetting == null)
                    continue;

                if (!dateSetting.IsActual(dateTime, GuidCalendar))
                    continue;

                if (dateSetting.ExplicitDailyPlan)
                    explicitDailyPlans.Add(dateSetting.GuidDailyPlan);
                else
                    dailyPlans.Add(dateSetting.GuidDailyPlan);
            }

            return explicitDailyPlans.Count > 0 ? explicitDailyPlans : dailyPlans;
        }

        public Guid GetGuid()
        {
            return IdTimeZone;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.TimeZone;
        }
    }
}
