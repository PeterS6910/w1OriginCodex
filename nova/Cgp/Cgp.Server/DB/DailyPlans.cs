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
    public sealed class DailyPlans : 
        ABaseOrmTable<DailyPlans, DailyPlan>, 
        IDailyPlans
    {
        private DailyPlans()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<DailyPlan>())
        {
        }

        protected override void LoadObjectsInRelationship(DailyPlan obj)
        {
            if (obj.DayIntervals != null)
            {
                IList<DayInterval> list = new List<DayInterval>();
                foreach (DayInterval dayInterval in obj.DayIntervals)
                {
                    list.Add(DayIntervals.Singleton.GetById(dayInterval.IdInterval));
                }
                obj.DayIntervals.Clear();
                foreach (DayInterval dayInterval in list)
                    obj.DayIntervals.Add(dayInterval);
            }
        }

        protected override IModifyObject CreateModifyObject(DailyPlan ormbObject)
        {
            return new DailyPlanModifyObj(ormbObject);
        }

        public override void CUDSpecial(DailyPlan dailyPlan, ObjectDatabaseAction objectDatabaseAction)
        {
            TimeAxis.Singleton.StartOrReset();

            if (dailyPlan != null)
            {
                DbWatcher.Singleton.DbObjectChanged(dailyPlan, objectDatabaseAction);
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(DailyPlan.COLUMNDAILYPLANNAME, true));
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

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                DailyPlan dailyPlan = GetById(idObj);
                if (dailyPlan == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();
                
                IList<TimeZone> timeZones = TimeZoneDateSettings.Singleton.GetTimeZonesFromDateSettingForDailyPlan(dailyPlan.IdDailyPlan);
                if (timeZones != null && timeZones.Count > 0)
                {
                    foreach (TimeZone tz in timeZones)
                    {
                        TimeZone timeZoneResult = TimeZones.Singleton.GetById(tz.IdTimeZone);
                        result.Add(timeZoneResult);
                    }
                }

                if (result.Count == 0)
                {
                    return null;
                }
                return result.OrderBy(orm => orm.ToString()).ToList();
            }
            catch
            {
                return null;
            }
        }

        public void CreateDefaultDailyPlans()
        {
            try
            {
                DailyPlan dailyPlan;
                DayInterval dayInterval;

                ICollection<DailyPlan> result = 
                    SelectLinq<DailyPlan>(
                        dp => dp.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_ON);

                if (result == null || result.Count == 0)
                {
                    dailyPlan = new DailyPlan
                    {
                        DailyPlanName = DailyPlan.IMPLICIT_DP_ALWAYS_ON,
                        DayIntervals = new List<DayInterval>()
                    };

                    dayInterval = new DayInterval
                    {
                        MinutesFrom = 0,
                        MinutesTo = 1439
                    };
                    dailyPlan.DayIntervals.Add(dayInterval);
                    Insert(ref dailyPlan);
                }

                result = SelectLinq<DailyPlan>(dp => dp.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                if (result == null || result.Count == 0)
                {
                    dailyPlan = new DailyPlan
                    {
                        DailyPlanName = DailyPlan.IMPLICIT_DP_ALWAYS_OFF
                    };
                    Insert(ref dailyPlan);
                }

                result = SelectLinq<DailyPlan>(dp => dp.DailyPlanName == DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                if (result == null || result.Count == 0)
                {
                    dailyPlan = new DailyPlan
                    {
                        DailyPlanName = DailyPlan.IMPLICIT_DP_DAYLIGHT_ON,
                        DayIntervals = new List<DayInterval>()
                    };

                    dayInterval = new DayInterval
                    {
                        MinutesFrom = 360,
                        MinutesTo = 1079
                    };
                    dailyPlan.DayIntervals.Add(dayInterval);
                    Insert(ref dailyPlan);
                }

                result = SelectLinq<DailyPlan>(dp => dp.DailyPlanName == DailyPlan.IMPLICIT_DP_NIGHT_ON);
                if (result == null || result.Count == 0)
                {
                    dailyPlan = new DailyPlan
                    {
                        DailyPlanName = DailyPlan.IMPLICIT_DP_NIGHT_ON,
                        DayIntervals = new List<DayInterval>()
                    };

                    dayInterval = new DayInterval
                    {
                        MinutesFrom = 0,
                        MinutesTo = 359
                    };
                    dailyPlan.DayIntervals.Add(dayInterval);
                    dayInterval = new DayInterval
                    {
                        MinutesFrom = 1080,
                        MinutesTo = 1439
                    };
                    dailyPlan.DayIntervals.Add(dayInterval);
                    Insert(ref dailyPlan);
                }
            }
            catch
            { }
        }

        public DailyPlan GetDailyPlanAlwaysON()
        {
            DailyPlan alwaysOn = null;
            try
            {
                ICollection<DailyPlan> result = 
                    SelectLinq<DailyPlan>(
                        dp => dp.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_ON);

                if (result != null && result.Count > 0)
                {
                    alwaysOn = result.ElementAt(0);
                }
            }
            catch {}
            return alwaysOn;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<DailyPlan> linqResult;

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
                        ? SelectLinq<DailyPlan>(
                            dp => dp.DailyPlanName.IndexOf(name) >= 0)
                        : SelectLinq<DailyPlan>(
                            dp =>
                                dp.DailyPlanName.IndexOf(name) >= 0 ||
                                dp.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(dp => dp.DailyPlanName).ToList();
                foreach (DailyPlan dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<DailyPlan> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<DailyPlan>(
                        dp =>
                            dp.DailyPlanName.IndexOf(name) >= 0 ||
                            dp.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<DailyPlan> linqResult = 
                string.IsNullOrEmpty(name) 
                    ? List() 
                    : SelectLinq<DailyPlan>(
                        dp => dp.DailyPlanName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<DailyPlan> listDailyPlans)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (listDailyPlans != null)
            {
                listDailyPlans = listDailyPlans.OrderBy(dp => dp.DailyPlanName).ToList();
                foreach (DailyPlan dp in listDailyPlans)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<DailyPlanShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<DailyPlan> listDailyPlan = SelectByCriteria(filterSettings, out error);
            ICollection<DailyPlanShort> result = new List<DailyPlanShort>();

            if (listDailyPlan != null)
            {
                foreach (DailyPlan dailyPlan in listDailyPlan)
                {
                    result.Add(new DailyPlanShort(dailyPlan));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<DailyPlan> listDailyPlan = List(out error);
            IList<IModifyObject> listDailyPlanModifyObj = null;
            if (listDailyPlan != null)
            {
                listDailyPlanModifyObj = new List<IModifyObject>();
                foreach (DailyPlan dailyPlan in listDailyPlan)
                {
                    listDailyPlanModifyObj.Add(new DailyPlanModifyObj(dailyPlan));
                }
                listDailyPlanModifyObj = listDailyPlanModifyObj.OrderBy(dailyPlan => dailyPlan.ToString()).ToList();
            }
            return listDailyPlanModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.DailyPlan; }
        }
    }
}
