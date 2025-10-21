using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class SecurityTimeZones :
        ANcasBaseOrmTable<SecurityTimeZones, SecurityTimeZone>,
        ISecurityTimeZones
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<SecurityTimeZone>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(SecurityTimeZone obj)
            {
                if (obj.DateSettings != null)
                    return obj.DateSettings;

                return Enumerable.Empty<AOrmObjectWithVersion>();
            }
        }

        private SecurityTimeZones()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            SecurityTimeZone securityTimeZone)
        {
            yield return securityTimeZone.Calendar;

            var dateSettings = securityTimeZone.DateSettings;

            if (dateSettings != null)
                foreach (var securityTimeZoneDateSetting in dateSettings)
                {
                    yield return securityTimeZoneDateSetting.SecurityDailyPlan;
                    yield return securityTimeZoneDateSetting.DayType;
                }
        }

        protected override IModifyObject CreateModifyObject(SecurityTimeZone ormbObject)
        {
            return new SecurityTimeZoneModifyObj(ormbObject);
        }

        public override void CUDSpecial(SecurityTimeZone securityTimeZone, ObjectDatabaseAction objectDatabaseAction)
        {
            SecurityTimeAxis.Singleton.StartOrReset();

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        securityTimeZone.GetId(),
                        securityTimeZone.GetObjectType()));
            }
            else if (securityTimeZone != null)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(securityTimeZone);
            }
        }

        protected override void LoadObjectsInRelationship(SecurityTimeZone obj)
        {
            if (obj.Calendar != null)
            {
                obj.Calendar = Calendars.Singleton.GetById(obj.Calendar.IdCalendar);
            }

            if (obj.DateSettings != null)
            {
                IList<SecurityTimeZoneDateSetting> list = new List<SecurityTimeZoneDateSetting>();

                foreach (var dateSettings in obj.DateSettings)
                {
                    list.Add(SecurityTimeZoneDateSettings.Singleton.GetById(dateSettings.IdSecurityTimeZoneDateSetting));
                }

                obj.DateSettings.Clear();
                foreach (var dateSettings in list)
                    obj.DateSettings.Add(dateSettings);
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

        public ICollection<SecurityTimeZone> GetStzForCalendar(Calendar calendar)
        {
            try
            {
                if (calendar == null) return null;
                return SelectLinq<SecurityTimeZone>(securityTimeZone => securityTimeZone.Calendar == calendar);
            }
            catch
            {
                return null;
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var secTimeZone = GetById(idObj);
                if (secTimeZone == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();

                var cardReaders = CardReaders.Singleton.IsReferencedSecurityTimeZone(secTimeZone.IdSecurityTimeZone);
                if (cardReaders != null && cardReaders.Count > 0)
                {
                    foreach (var cr in cardReaders)
                    {
                        var outCardReader = CardReaders.Singleton.GetById(cr.IdCardReader);
                        result.Add(outCardReader);
                    }
                }

                var deviceAlarmSettingsStzForEnterToMenu =
                    DevicesAlarmSettings.Singleton.SecurityTimeZoneForEnterToMenu();

                if (deviceAlarmSettingsStzForEnterToMenu != null)
                {
                    result.Add(GetById(deviceAlarmSettingsStzForEnterToMenu.IdSecurityTimeZone));
                }

                return
                    result.Count == 0
                        ? null
                        : result.OrderBy(orm => orm.ToString()).ToList();
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
            ICollection<SecurityTimeZone> linqResult;

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
                        ? SelectLinq<SecurityTimeZone>(stz => stz.Name.IndexOf(name) >= 0)
                        : SelectLinq<SecurityTimeZone>(
                            stz =>
                                stz.Name.IndexOf(name) >= 0 ||
                                stz.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(stz => stz.Name).ToList();
                foreach (var dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<SecurityTimeZone> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<SecurityTimeZone>(
                        stz =>
                            stz.Name.IndexOf(name) >= 0 ||
                            stz.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<SecurityTimeZone>(stz => stz.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<SecurityTimeZone> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(stz => stz.Name).ToList();
                foreach (var secTZ in linqResult)
                {
                    resultList.Add(secTZ);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<SecurityTimeZoneShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listSecurityTimeZone = SelectByCriteria(filterSettings, out error);
            ICollection<SecurityTimeZoneShort> result = new List<SecurityTimeZoneShort>();
            if (listSecurityTimeZone != null)
            {
                foreach (var stz in listSecurityTimeZone)
                {
                    result.Add(new SecurityTimeZoneShort(stz));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listSecurityTimeZone = List(out error);
            IList<IModifyObject> listStzModifyObj = null;
            if (listSecurityTimeZone != null)
            {
                listStzModifyObj = new List<IModifyObject>();
                foreach (var stz in listSecurityTimeZone)
                {
                    listStzModifyObj.Add(new SecurityTimeZoneModifyObj(stz));
                }
                listStzModifyObj = listStzModifyObj.OrderBy(stz => stz.ToString()).ToList();
            }
            return listStzModifyObj;
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idSecurityTimeZone)
        {
            var objects = new List<AOrmObject>();

            var securityTimeZone = GetById(idSecurityTimeZone);
            if (securityTimeZone != null)
            {
                if (securityTimeZone.DateSettings != null)
                {
                    foreach (var item in securityTimeZone.DateSettings)
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

        public IList<SecurityDayInterval> GetActualSecurityDayInterval(Guid idSecurityTimeZone, DateTime dateTime)
        {
            var securityTimeZone = GetById(idSecurityTimeZone);
            if (securityTimeZone == null || securityTimeZone.DateSettings == null)
                return null;

            IList<SecurityDailyPlan> securityDailyPlans = new List<SecurityDailyPlan>();
            IList<SecurityDailyPlan> explicitSecurityDailyPlans = new List<SecurityDailyPlan>();

            foreach (var dateSetting in securityTimeZone.DateSettings)
            {
                if (dateSetting.SecurityDailyPlan != null && dateSetting.IsActual(dateTime))
                {
                    if (dateSetting.ExplicitSecurityDailyPlan)
                    {
                        explicitSecurityDailyPlans.Add(dateSetting.SecurityDailyPlan);
                    }
                    else
                    {
                        securityDailyPlans.Add(dateSetting.SecurityDailyPlan);
                    }
                }
            }

            if (explicitSecurityDailyPlans.Count > 0)
            {
                return GetIntervalsFromSecurityDailyPlans(explicitSecurityDailyPlans);
            }
            return GetIntervalsFromSecurityDailyPlans(securityDailyPlans);
        }

        private static IList<SecurityDayInterval> GetIntervalsFromSecurityDailyPlans(
            IEnumerable<SecurityDailyPlan> securityDailyPlansList)
        {
            IList<SecurityDayInterval> resultIntervals = new List<SecurityDayInterval>();
            foreach (var securityDailyPlan in securityDailyPlansList)
            {
                if (securityDailyPlan.SecurityDayIntervals != null)
                {
                    foreach (var securityDayInterval in securityDailyPlan.SecurityDayIntervals)
                    {
                        resultIntervals.Add(securityDayInterval);
                    }
                }
            }
            return resultIntervals;
        }

        public List<SecurityDailyPlan> GetActualSecurityDailyPlans(SecurityTimeZone securityTimeZone, DateTime dateTime)
        {
            var securityDailyPlans = new List<SecurityDailyPlan>();
            var explicitSecurityDailyPlans = new List<SecurityDailyPlan>();

            foreach (var dateSetting in securityTimeZone.DateSettings)
            {
                if (dateSetting.SecurityDailyPlan != null && dateSetting.IsActual(dateTime))
                {
                    if (dateSetting.ExplicitSecurityDailyPlan)
                    {
                        explicitSecurityDailyPlans.Add(dateSetting.SecurityDailyPlan);
                    }
                    else
                    {
                        securityDailyPlans.Add(dateSetting.SecurityDailyPlan);
                    }
                }
            }

            if (explicitSecurityDailyPlans.Count > 0)
            {
                return explicitSecurityDailyPlans;
            }
            return securityDailyPlans;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.SecurityTimeZone; }
        }
    }
}
