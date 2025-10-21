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
    public sealed class DayTypes :
        ABaseOrmTable<DayTypes, DayType>, 
        IDayTypes
    {
        private DayTypes()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<DayType>())
        {
        }

        public override void CUDSpecial(DayType dayType, ObjectDatabaseAction objectDatabaseAction)
        {
            if (dayType != null)
            {
                DbWatcher.Singleton.DbObjectChanged(dayType, objectDatabaseAction);
            }
        }

        protected override IModifyObject CreateModifyObject(DayType ormbObject)
        {
            return new DayTypeModifyObj(ormbObject);
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(DayType.COLUMNDAYTYPENAME, true));
        }

        protected override void LoadObjectsInRelationship(DayType obj)
        {
            if (obj.TimeZoneDateSetting != null)
            {
                IList<TimeZoneDateSetting> list = new List<TimeZoneDateSetting>();

                foreach (TimeZoneDateSetting timeZoneDateSetting in obj.TimeZoneDateSetting)
                {
                    list.Add(TimeZoneDateSettings.Singleton.GetById(timeZoneDateSetting.IdTimeZoneDateSetting));
                }

                obj.TimeZoneDateSetting.Clear();
                foreach (TimeZoneDateSetting timeZoneDateSetting in list)
                    obj.TimeZoneDateSetting.Add(timeZoneDateSetting);
            }
        }

        public void CreateDefaultDayTypes()
        {
            try
            {
                DayType dateType;

                ICollection<DayType> result = 
                    SelectLinq<DayType>(
                        dt => dt.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY);

                if (result == null || result.Count == 0)
                {
                    dateType = new DayType
                    {
                        DayTypeName = DayType.IMPLICIT_DAY_TYPE_HOLIDAY
                    };
                    Insert(ref dateType);
                }

                result = SelectLinq<DayType>(dt => dt.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION);
                if (result == null || result.Count == 0)
                {
                    dateType = new DayType
                    {
                        DayTypeName = DayType.IMPLICIT_DAY_TYPE_VACATION
                    };
                    Insert(ref dateType);
                }
            }
            catch
            {}
        }

        private DayType GetDayTypeHoliday()
        {
            try
            {
                ICollection<DayType> result = 
                    SelectLinq<DayType>(
                        dt => dt.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY);

                if (result != null && result.Count > 0)
                {
                    DayType dayType = result.ElementAt(0);
                    return dayType;
                }
            }
            catch
            { }
            return null;
        }

        private DayType GetDayTypeVacation()
        {
            try
            {
                ICollection<DayType> result = 
                    SelectLinq<DayType>(
                        dt => dt.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION);

                if (result != null && result.Count > 0)
                {
                    DayType dayType = result.ElementAt(0);
                    return dayType;
                }
            }
            catch
            { }
            return null;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.CALENDARS_DAY_TYPES),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.CALENDARS_DAY_TYPES),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesInsertDeletePerform),
                login);
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            //calendar
            try
            {
                DayType dateType = GetById(idObj);
                if (dateType == null)
                    return null;

                IList<AOrmObject> result = new List<AOrmObject>();

                var calendars = CalendarDateSettings.Singleton.GetCalendarsDateSettingForDayType(dateType.IdDayType);

                if (calendars != null)
                    foreach (Calendar cal in calendars)
                        result.Add(Calendars.Singleton.GetById(cal.IdCalendar));

                var timeZones = 
                    TimeZoneDateSettings.Singleton.GetTimeZonesFromDateSettingForDailyPlan(dateType.IdDayType);

                if (timeZones != null)
                    foreach (TimeZone tz in timeZones)
                        result.Add(TimeZones.Singleton.GetById(tz.IdTimeZone));

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
            ICollection<DayType> linqResult;

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
                        ? SelectLinq<DayType>(
                            dt => dt.DayTypeName.IndexOf(name) >= 0)
                        : SelectLinq<DayType>(
                            dt =>
                                dt.DayTypeName.IndexOf(name) >= 0 ||
                                dt.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(dt => dt.DayTypeName).ToList();
                foreach (DayType dayType in linqResult)
                {
                    resultList.Add(dayType);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<DayType> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<DayType>(
                        dt =>
                            dt.DayTypeName.IndexOf(name) >= 0 ||
                            dt.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<DayType> linqResult = 
                string.IsNullOrEmpty(name) ? 
                    List() : 
                    SelectLinq<DayType>(
                        dt => dt.DayTypeName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<DayType> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(dt => dt.DayTypeName).ToList();
                foreach (DayType dayType in linqResult)
                {
                    resultList.Add(dayType);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<DayTypeShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<DayType> listDayType = SelectByCriteria(filterSettings, out error);
            ICollection<DayTypeShort> result = new List<DayTypeShort>();

            DayType dt = GetDayTypeHoliday();
            if (dt != null)
                result.Add(new DayTypeShort(dt));
            dt = GetDayTypeVacation();
            if (dt != null)
                result.Add(new DayTypeShort(dt));

            if (listDayType != null)
            {
                foreach (DayType dayType in listDayType)
                {
                    if (dayType.DayTypeName != DayType.IMPLICIT_DAY_TYPE_HOLIDAY &&
                        dayType.DayTypeName != DayType.IMPLICIT_DAY_TYPE_VACATION)
                    {
                        result.Add(new DayTypeShort(dayType));
                    }
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<DayType> listDayType = List(out error);
            IList<IModifyObject> listDayTypeModifyObj = null;
            if (listDayType != null)
            {
                listDayTypeModifyObj = new List<IModifyObject>();
                foreach (DayType dayType in listDayType)
                {
                    listDayTypeModifyObj.Add(new DayTypeModifyObj(dayType));
                }
                listDayTypeModifyObj = listDayTypeModifyObj.OrderBy(dayType => dayType.ToString()).ToList();
            }
            return listDayTypeModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.DayType; }
        }
    }
}
