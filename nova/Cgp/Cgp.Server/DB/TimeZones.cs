using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;

using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.Server.DB
{
    public sealed class TimeZones : 
        ABaseOrmTable<TimeZones, TimeZone>, 
        ITimeZones
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<TimeZone>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(TimeZone obj)
            {
                if (obj.DateSettings != null)
                    return obj.DateSettings;

                return Enumerable.Empty<AOrmObjectWithVersion>();
            }
        }

        private TimeZones()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(TimeZone timeZone)
        {
            yield return timeZone.Calendar;

            var timeZoneDateSettings = timeZone.DateSettings;

            foreach (var timeZoneDateSetting in timeZoneDateSettings)
            {
                yield return timeZoneDateSetting.DailyPlan;
                yield return timeZoneDateSetting.DayType;
            }
        }

        protected override IModifyObject CreateModifyObject(TimeZone ormbObject)
        {
            return new TimeZoneModifyObj(ormbObject);
        }

        protected override void LoadObjectsInRelationship(TimeZone obj)
        {
            if (obj.Calendar != null)
            {
                obj.Calendar = Calendars.Singleton.GetById(obj.Calendar.IdCalendar);
                if (obj.Calendar.DateSettings != null)
                {
                    IList<CalendarDateSetting> list = new List<CalendarDateSetting>();
                    
                    foreach (CalendarDateSetting calendarDateSetting in obj.Calendar.DateSettings)
                    {
                        CalendarDateSetting dateSetting = CalendarDateSettings.Singleton.GetById(calendarDateSetting.IdCalendarDateSetting);

                        if (dateSetting.DayType != null)
                        {
                            dateSetting.DayType = DayTypes.Singleton.GetById(dateSetting.DayType.IdDayType);
                        }
                        
                        list.Add(dateSetting);
                    }

                    obj.Calendar.DateSettings.Clear();
                    foreach (CalendarDateSetting calendarDateSetting in list)
                        obj.Calendar.DateSettings.Add(calendarDateSetting);
                }
            }

            if (obj.DateSettings != null)
            {
                IList<TimeZoneDateSetting> list = new List<TimeZoneDateSetting>();

                foreach (TimeZoneDateSetting timeZoneDateSetting in obj.DateSettings)
                {
                    TimeZoneDateSetting dateSetting = TimeZoneDateSettings.Singleton.GetById(timeZoneDateSetting.IdTimeZoneDateSetting);
                    if (dateSetting.DailyPlan != null)
                        dateSetting.DailyPlan = DailyPlans.Singleton.GetById(dateSetting.DailyPlan.IdDailyPlan);

                    dateSetting.TimeZone = obj;

                    list.Add(dateSetting);
                }

                obj.DateSettings.Clear();
                foreach (TimeZoneDateSetting timeZoneDateSetting in list)
                    obj.DateSettings.Add(timeZoneDateSetting);
            }
        }

        public override void CUDSpecial(TimeZone timeZone, ObjectDatabaseAction objectDatabaseAction)
        {
            TimeAxis.Singleton.StartOrReset();
            
            if (timeZone != null)
            {
                DbWatcher.Singleton.DbTimeZoneChanged(timeZone, objectDatabaseAction);
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(TimeZone.COLUMNNAME, true));
        }

        public override void EditEndForCollection(TimeZone ormObject)
        {
            if (ormObject.DateSettings != null)
            {
                foreach (TimeZoneDateSetting dateSetting in ormObject.DateSettings)
                {
                    CentralTransactionControl.Singleton.EditEnd(
                        new IdAndObjectType(
                            dateSetting.GetId(), 
                            dateSetting.GetObjectType()));
                }
            }
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.DAILY_PLANS_TIME_ZONES),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.DAILY_PLANS_TIME_ZONES),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesInsertDeletePerform),
                login);
        }

        public override bool IsReferencedSubObjects(TimeZone ormObject)
        {
            if (IsUsedInGeneralSettings(ormObject.GetId()))
                return true;
            return false;
        }

        private static bool IsUsedInGeneralSettings(object idTimeZone)
        {
            if (idTimeZone is Guid)
            {
                if (GeneralOptions.Singleton.TimeZoneGuidString == idTimeZone.ToString()) return true;
                if (GeneralOptions.Singleton.EventlogTimeZoneGuidString == idTimeZone.ToString()) return true;
            }
            return false;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<TimeZone> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name)) return null;

                linqResult =
                    single
                        ? SelectLinq<TimeZone>(tz => tz.Name.IndexOf(name) >= 0)
                        : SelectLinq<TimeZone>(
                            tz =>
                                tz.Name.IndexOf(name) >= 0 ||
                                tz.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(tz => tz.Name).ToList();
                foreach (TimeZone tz in linqResult)
                {
                    resultList.Add(tz);
                }
            }

            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<TimeZone> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = SelectLinq<TimeZone>(tz => tz.Name.IndexOf(name) >= 0
                        || tz.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<TimeZone> linqResult = null;

            if (name == null || name == string.Empty)
            {
                linqResult = base.List();
            }
            else
            {
                linqResult = SelectLinq<TimeZone>(tz => tz.Name.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<TimeZone> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(tz => tz.Name).ToList();
                foreach (TimeZone tz in linqResult)
                {
                    resultList.Add(tz);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<TimeZoneShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<TimeZone> listTimeZone = SelectByCriteria(filterSettings, out error);
            ICollection<TimeZoneShort> result = new List<TimeZoneShort>();
            if (listTimeZone != null)
            {
                foreach (TimeZone timeZone in listTimeZone)
                {
                    result.Add(new TimeZoneShort(timeZone));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<TimeZone> listTimeZone = List(out error);
            IList<IModifyObject> listTimeZoneModifyObj = null;
            if (listTimeZone != null)
            {
                listTimeZoneModifyObj = new List<IModifyObject>();
                foreach (TimeZone timeZone in listTimeZone)
                {
                    listTimeZoneModifyObj.Add(new TimeZoneModifyObj(timeZone));
                }
                listTimeZoneModifyObj = listTimeZoneModifyObj.OrderBy(timeZone => timeZone.ToString()).ToList();
            }
            return listTimeZoneModifyObj;
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idTimeZone)
        {
            var objects = new List<AOrmObject>();

            TimeZone timeZone = GetById(idTimeZone);
            if (timeZone != null)
            {
                if (timeZone.DateSettings != null)
                {
                    foreach (TimeZoneDateSetting item in timeZone.DateSettings)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }                
            }

            return objects;
        }

        public IList<DayInterval> GetActualDayInterval(Guid idTimeZone, DateTime dateTime)
        {
            TimeZone timeZone = GetById(idTimeZone);
            if (timeZone == null || timeZone.DateSettings == null)
                return null;

            IList<DailyPlan> dailyPlans = new List<DailyPlan>();
            IList<DailyPlan> explicitDailyPlans = new List<DailyPlan>();

            foreach (TimeZoneDateSetting dateSetting in timeZone.DateSettings)
            {
                if (dateSetting.DailyPlan != null && dateSetting.IsActual(dateTime))
                {
                    if (dateSetting.ExplicitDailyPlan)
                    {
                        explicitDailyPlans.Add(dateSetting.DailyPlan);
                    }
                    else
                    {
                        dailyPlans.Add(dateSetting.DailyPlan);
                    }
                }
            }

            if (explicitDailyPlans.Count > 0)
            {
                return GetIntervalsFromDailyPlans(explicitDailyPlans);
            }
            return GetIntervalsFromDailyPlans(dailyPlans);
        }

        private static IList<DayInterval> GetIntervalsFromDailyPlans(IList<DailyPlan> dailyPlansList)
        {
            IList<DayInterval> resultIntervals = new List<DayInterval>();
            foreach (DailyPlan dailyPlan in dailyPlansList)
            {
                if (dailyPlan.DayIntervals != null)
                {
                    foreach (DayInterval dayInterval in dailyPlan.DayIntervals)
                    {
                        resultIntervals.Add(dayInterval);
                    }
                }
            }
            return resultIntervals;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.TimeZone; }
        }
    }
}
