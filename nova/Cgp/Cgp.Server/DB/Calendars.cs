using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using NHibernate.Criterion;
using NHibernate;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.Server.DB
{
    public sealed class Calendars : 
        ABaseOrmTable<Calendars, Calendar>, 
        ICalendars
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<Calendar>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(Calendar obj)
            {
                if (obj.DateSettings != null)
                    return obj.DateSettings;

                return Enumerable.Empty<AOrmObjectWithVersion>();
            }
        }

        private Calendars()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        protected override void LoadObjectsInRelationship(Calendar obj)
        {
            if (obj.DateSettings != null)
            {
                IList<CalendarDateSetting> list = new List<CalendarDateSetting>();

                foreach (var calendarDateSetting in obj.DateSettings)
                {
                    list.Add(CalendarDateSettings.Singleton.GetById(calendarDateSetting.IdCalendarDateSetting));
                }

                obj.DateSettings.Clear();
                foreach (var calendarDateSetting in list)
                    obj.DateSettings.Add(calendarDateSetting);
            }

            if (obj.TimeZone != null)
            {
                IList<TimeZone> list = new List<TimeZone>();

                foreach (var timeZone in obj.TimeZone)
                {
                    list.Add(TimeZones.Singleton.GetById(timeZone.IdTimeZone));
                }

                obj.TimeZone.Clear();
                foreach (var timeZone in list)
                    obj.TimeZone.Add(timeZone);
            }
        }

        public void CreateDefaultCalendar()
        {
            try
            {
                var result = 
                    SelectLinq<Calendar>(
                        c => c.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT);

                if (result == null || result.Count == 0)
                {
                    var calendar = new Calendar
                    {
                        CalendarName = Calendar.IMPLICIT_CALENDAR_DEFAULT
                    };
                    Insert(ref calendar);
                }
            }
            catch
            { }
        }

        public Calendar GetDefaultCalendar()
        {
            try
            {
                var result = 
                    SelectLinq<Calendar>(
                        c => c.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT);

                if (result != null && result.Count > 0)
                {
                    var calendar = result.ElementAt(0);
                    if (calendar != null
                        && HasAccessViewForObject(calendar))
                    {
                        LoadObjectsInRelationship(calendar);
                        return calendar;
                    }
                }
            }
            catch
            { }
            return null;
        }

        public override void CUDSpecial(Calendar calendar, ObjectDatabaseAction objectDatabaseAction)
        {
            TimeAxis.Singleton.StartOrReset();

            if (calendar != null)
            {
                DbWatcher.Singleton.DbObjectChanged(calendar, objectDatabaseAction);
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(Calendar.COLUMNCALENDARNAME, true));
        }

        public override void EditEndForCollection(Calendar ormObject)
        {
            if (ormObject.DateSettings == null)
                return;

            foreach (var dateSetting in ormObject.DateSettings)
                CentralTransactionControl.Singleton.EditEnd(
                    new IdAndObjectType(
                        dateSetting.GetId(),
                        dateSetting.GetObjectType()));
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CALENDARS_DAY_TYPES), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CALENDARS_DAY_TYPES), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesInsertDeletePerform), login);
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var calendar = GetById(idObj);
                if (calendar == null) return null;

                var result = new List<AOrmObject>();

                if (calendar.TimeZone != null)
                    foreach (var tz in calendar.TimeZone)
                    {
                        var timeZoneResult = TimeZones.Singleton.GetById(tz.IdTimeZone);
                        result.Add(timeZoneResult);
                    }

                return 
                    result.Count > 0
                        ? result.OrderBy(orm => orm.ToString()).ToList() 
                        : null;
            }
            catch
            {
                return null;
            }
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<Calendar> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                linqResult =
                    single
                        ? SelectLinq<Calendar>(
                            c => c.CalendarName.IndexOf(name) >= 0)
                        : SelectLinq<Calendar>(
                            c =>
                                c.CalendarName.IndexOf(name) >= 0 ||
                                c.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(calendar => calendar.CalendarName).ToList();
                foreach (var calendar in linqResult)
                {
                    resultList.Add(calendar);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Calendar> linqResult = null;
            if (!string.IsNullOrEmpty(name))
            {
                linqResult = SelectLinq<Calendar>(c => c.CalendarName.IndexOf(name) >= 0 || c.Description.IndexOf(name) >= 0);
            }
            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<Calendar> linqResult = 
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<Calendar>(c => c.CalendarName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<Calendar> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(calendar => calendar.CalendarName).ToList();
                foreach (var calendar in linqResult)
                {
                    resultList.Add(calendar);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<CalendarShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listCalendar = SelectByCriteria(filterSettings, out error);
            ICollection<CalendarShort> result = new List<CalendarShort>();

            if (listCalendar != null)
            {
                foreach (var calendar in listCalendar)
                {
                    result.Add(new CalendarShort(calendar));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listCalendar = List(out error);
            IList<IModifyObject> listCalendarModifyObj = null;
            if (listCalendar != null)
            {
                listCalendarModifyObj = new List<IModifyObject>();
                foreach (var calendar in listCalendar)
                {
                    listCalendarModifyObj.Add(new CalendarModifyObj(calendar));
                }
                listCalendarModifyObj = listCalendarModifyObj.OrderBy(calendar => calendar.ToString()).ToList();
            }
            return listCalendarModifyObj;
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idCalendar)
        {
            var objects = new List<AOrmObject>();

            var calendar = GetById(idCalendar);
            if (calendar != null)
            {
                if (calendar.DateSettings!=null)
                {
                    foreach (var item in calendar.DateSettings)
                    {
                        if (item!=null)
                        {
                            objects.Add(item);
                        }
                    }
                }
            }

            return objects;
        }

        protected override IModifyObject CreateModifyObject(Calendar ormbObject)
        {
            return new CalendarModifyObj(ormbObject);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.Calendar; }
        }
    }
}
