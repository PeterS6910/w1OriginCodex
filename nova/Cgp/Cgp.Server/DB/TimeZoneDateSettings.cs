using System;
using System.Collections.Generic;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.Server.DB
{
    public sealed class TimeZoneDateSettings : 
        ABaseOrmTable<TimeZoneDateSettings, TimeZoneDateSetting>, 
        ITimeZoneDateSettings
    {
        private TimeZoneDateSettings()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<TimeZoneDateSetting>())
        {
        }

        protected override void LoadObjectsInRelationship(TimeZoneDateSetting obj)
        {
            if (obj.TimeZone != null)
            {
                obj.TimeZone = TimeZones.Singleton.GetById(obj.TimeZone.IdTimeZone);
            }
            if (obj.DailyPlan != null)
            {
                obj.DailyPlan = DailyPlans.Singleton.GetById(obj.DailyPlan.IdDailyPlan);
            }
            if (obj.DayType != null)
            {
                obj.DayType = DayTypes.Singleton.GetById(obj.DayType.IdDayType);
            }
        }

        public override void CUDSpecial(TimeZoneDateSetting dateSetting, ObjectDatabaseAction objectDatabaseAction)
        {
            TimeAxis.Singleton.StartOrReset();

            if (dateSetting != null)
            {
                DbWatcher.Singleton.DbObjectChanged(dateSetting, objectDatabaseAction);
            }
        }

        public override bool HasAccessView(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.DAILY_PLANS_TIME_ZONES), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.DAILY_PLANS_TIME_ZONES), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesInsertDeletePerform), login);
        }

        public IList<TimeZone> GetTimeZonesFromDateSettingForDailyPlan(Guid idDailyPlan)
        {
            try
            {
                DailyPlan dailyPlan = DailyPlans.Singleton.GetById(idDailyPlan);

                if (dailyPlan == null)
                    return null;

                ICollection<TimeZoneDateSetting> timeZoneDateSettings = SelectLinq<TimeZoneDateSetting>(timeZoneDateSetting => timeZoneDateSetting.DailyPlan == dailyPlan);
                if (timeZoneDateSettings != null && timeZoneDateSettings.Count > 0)
                {
                    IList<TimeZone> result = new List<TimeZone>();
                    foreach (TimeZoneDateSetting tzs in timeZoneDateSettings)
                    {
                        if (!result.Contains(tzs.TimeZone))
                        {
                            result.Add(tzs.TimeZone);
                        }
                    }
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public IList<TimeZone> GetTimeZonesFromDateSettingForDayType(Guid idDayType)
        {
            try
            {
                DayType dayType = DayTypes.Singleton.GetById(idDayType);

                if (dayType == null)
                    return null;

                ICollection<TimeZoneDateSetting> timeZoneDateSettings = SelectLinq<TimeZoneDateSetting>(timeZoneDateSetting => timeZoneDateSetting.DayType == dayType);
                if (timeZoneDateSettings != null && timeZoneDateSettings.Count > 0)
                {
                    IList<TimeZone> result = new List<TimeZone>();
                    foreach (TimeZoneDateSetting tzs in timeZoneDateSettings)
                    {
                        if (!result.Contains(tzs.TimeZone))
                        {
                            result.Add(tzs.TimeZone);
                        }
                    }
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idTimeZoneDateSettings)
        {
            List<AOrmObject> objects = new List<AOrmObject>();

            TimeZoneDateSetting timeZoneDateSettings = GetById(idTimeZoneDateSettings);
            if (timeZoneDateSettings != null)
            {
                if (timeZoneDateSettings.DailyPlan != null)
                {
                    objects.Add(timeZoneDateSettings.DailyPlan);
                }
                if (timeZoneDateSettings.DayType != null)
                {
                    objects.Add(timeZoneDateSettings.DayType);
                }
            }

            return objects;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.TimeZoneDateSetting; }
        }
    }
}
