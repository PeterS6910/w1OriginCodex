using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class SecurityTimeZoneDateSettings :
        ANcasBaseOrmTable<SecurityTimeZoneDateSettings, SecurityTimeZoneDateSetting>, 
        ISecurityTimeZoneDateSettings
    {
        private SecurityTimeZoneDateSettings()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<SecurityTimeZoneDateSetting>())
        {
        }

        public override void CUDSpecial(SecurityTimeZoneDateSetting dateSetting, ObjectDatabaseAction objectDatabaseAction)
        {
            SecurityTimeAxis.Singleton.StartOrReset();

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        dateSetting.GetId(),
                        dateSetting.GetObjectType()));
            }
            else if (dateSetting != null)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(dateSetting);
            }
        }

        protected override void LoadObjectsInRelationship(SecurityTimeZoneDateSetting obj)
        {
            if (obj.SecurityTimeZone != null)
            {
                obj.SecurityTimeZone = SecurityTimeZones.Singleton.GetById(obj.SecurityTimeZone.IdSecurityTimeZone);

                if (obj.SecurityTimeZone != null && obj.SecurityTimeZone.Calendar != null)
                {
                    obj.SecurityTimeZone.Calendar = Calendars.Singleton.GetById(obj.SecurityTimeZone.Calendar.IdCalendar);

                    if (obj.SecurityTimeZone.Calendar != null && obj.SecurityTimeZone.Calendar.DateSettings != null)
                    {
                        IList<CalendarDateSetting> list = new List<CalendarDateSetting>();

                        foreach (var calendarDateSetting in obj.SecurityTimeZone.Calendar.DateSettings)
                        {
                            var dateSetting = CalendarDateSettings.Singleton.GetById(calendarDateSetting.IdCalendarDateSetting);

                            if (dateSetting.DayType != null)
                            {
                                dateSetting.DayType = DayTypes.Singleton.GetById(dateSetting.DayType.IdDayType);
                            }

                            list.Add(dateSetting);
                        }

                        obj.SecurityTimeZone.Calendar.DateSettings.Clear();
                        foreach (var calendarDateSetting in list)
                            obj.SecurityTimeZone.Calendar.DateSettings.Add(calendarDateSetting);
                    }
                }
            }

            if (obj.SecurityDailyPlan != null)
            {
                obj.SecurityDailyPlan = SecurityDailyPlans.Singleton.GetById(obj.SecurityDailyPlan.IdSecurityDailyPlan);
            }
            
            if (obj.DayType != null)
            {
                obj.DayType = DayTypes.Singleton.GetById(obj.DayType.IdDayType);
            }
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.SDPS_STZS),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.SdpsStzsInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.SDPS_STZS),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.SdpsStzsInsertDeletePerform),
                login);
        }

        public ICollection<SecurityTimeZone> GetSecurityTimeZonesFromDateSettingForDayType(DayType dayType)
        {
            try
            {
                var timeZoneDateSettings = 
                    SelectLinq<SecurityTimeZoneDateSetting>(
                        secTimeZoneDateSetting => 
                                secTimeZoneDateSetting.DayType == dayType);

                if (timeZoneDateSettings == null)
                    return null;

                ICollection<SecurityTimeZone> result = new LinkedList<SecurityTimeZone>();

                foreach (var stzs in timeZoneDateSettings)
                    result.Add(stzs.SecurityTimeZone);

                return result;
            }
            catch
            {
                return null;
            }
        }

        public ICollection<SecurityTimeZone> 
            GetSecurityTimeZonesFromDateSettingForSecurityDailyPlan(Guid idSecurityDailyPlan)
        {
            try
            {
                var secDailyPlan = 
                    SecurityDailyPlans.Singleton.GetById(idSecurityDailyPlan);

                if (secDailyPlan == null)
                    return null;

                var timeZoneDateSettings = SelectLinq<SecurityTimeZoneDateSetting>(secTimeZoneDateSetting => secTimeZoneDateSetting.SecurityDailyPlan == secDailyPlan);
                if (timeZoneDateSettings != null && timeZoneDateSettings.Count > 0)
                {
                    ICollection<SecurityTimeZone> result = new LinkedList<SecurityTimeZone>();
                    foreach (var stzs in timeZoneDateSettings)
                    {
                        if (!result.Contains(stzs.SecurityTimeZone))
                        {
                            result.Add(stzs.SecurityTimeZone);
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

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idSecurityTimeZoneDateSettings)
        {
            var objects = new List<AOrmObject>();

            var securityTimeZoneDateSettings = GetById(idSecurityTimeZoneDateSettings);
            if (securityTimeZoneDateSettings != null)
            {
                if (securityTimeZoneDateSettings.SecurityDailyPlan != null)
                {
                    objects.Add(securityTimeZoneDateSettings.SecurityDailyPlan);
                }
                if (securityTimeZoneDateSettings.DayType != null)
                {
                    objects.Add(securityTimeZoneDateSettings.DayType);
                }
            }

            return objects;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.SecurityTimeZoneDateSetting; }
        }
    }
}
