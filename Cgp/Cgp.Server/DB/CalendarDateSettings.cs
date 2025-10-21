using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using NHibernate.Bytecode;

namespace Contal.Cgp.Server.DB
{
    public sealed class CalendarDateSettings : 
        ABaseOrmTable<CalendarDateSettings, CalendarDateSetting>, 
        ICalendarDateSettings
    {
        private CalendarDateSettings()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<CalendarDateSetting>())
        {
        }

        protected override void LoadObjectsInRelationship(CalendarDateSetting obj)
        {
            if (obj.Calendar != null)
            {
                obj.Calendar = Calendars.Singleton.GetById(obj.Calendar.IdCalendar);
            }

            if (obj.DayType != null)
            {
                obj.DayType = DayTypes.Singleton.GetById(obj.DayType.IdDayType);
            }
        }

        public ICollection<DayType> GetDayTypeByCalendar(Calendar calendar)
        {
            ICollection<DayType> dayTypes = new List<DayType>();
            try
            {
                var result = 
                    SelectLinq<CalendarDateSetting>(cds => cds.Calendar == calendar);

                if (result != null)
                {
                    foreach (var cds in result)
                    {
                        dayTypes.Add(cds.DayType);
                    }
                }
            }
            catch
            {
            }
            return dayTypes;
        }

        public ICollection<DayType> GetDayTypeUniqueByCalendar(Calendar calendar)
        {
            ICollection<DayType> dayTypes = new List<DayType>();
            try
            {
                var result = 
                    SelectLinq<CalendarDateSetting>(
                        cds => cds.Calendar == calendar);

                if (result != null)
                {
                    foreach (var cds in result)
                    {
                        if (!(dayTypes.Contains(cds.DayType)))
                        {
                            dayTypes.Add(cds.DayType);
                        }
                    }
                }
            }
            catch
            {
            }
            return dayTypes;
        }

        public override void CUDSpecial(CalendarDateSetting dateSetting, ObjectDatabaseAction objectDatabaseAction)
        {
            TimeAxis.Singleton.StartOrReset();

            if (dateSetting != null)
            {
                DbWatcher.Singleton.DbObjectChanged(dateSetting, objectDatabaseAction);
            }
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

        public IList<Calendar> GetCalendarsDateSettingForDayType(Guid idDayType)
        {
            try
            {
                var dayType = DayTypes.Singleton.GetById(idDayType);

                if (dayType == null)
                    return null;



                var calendarDateSettings = SelectLinq<CalendarDateSetting>(calendarDateSetting => calendarDateSetting.DayType == dayType);

                if (calendarDateSettings != null && calendarDateSettings.Count > 0)
                {
                    var result = new List<Calendar>();

                    foreach (var cds in calendarDateSettings)
                    {
                        if (!result.Contains(cds.Calendar))
                        {
                            result.Add(cds.Calendar);
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

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idCalendarDateSettings)
        {
            var objects = new List<AOrmObject>();

            var calendarDateSetting = GetById(idCalendarDateSettings);
            if (calendarDateSetting != null)
            {
                if (calendarDateSetting.DayType != null)
                {
                    objects.Add(calendarDateSetting.DayType);
                }
            }

            return objects;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CalendarDateSetting; }
        }
    }
}
