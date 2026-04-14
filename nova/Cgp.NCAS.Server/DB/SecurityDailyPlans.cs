using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class SecurityDailyPlans :
        ANcasBaseOrmTable<SecurityDailyPlans, SecurityDailyPlan>, 
        ISecurityDailyPlans
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<SecurityDailyPlan>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(SecurityDailyPlan obj)
            {
                if (obj.SecurityDayIntervals != null)
                    return obj.SecurityDayIntervals;

                return Enumerable.Empty<AOrmObjectWithVersion>();
            }
        }

        private SecurityDailyPlans()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        public override void CUDSpecial(SecurityDailyPlan securityDailyPlan, ObjectDatabaseAction objectDatabaseAction)
        {
            SecurityTimeAxis.Singleton.StartOrReset();

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        securityDailyPlan.GetId(),
                        securityDailyPlan.GetObjectType()));
            }
            else if (securityDailyPlan != null)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(securityDailyPlan);
            }
        }

        protected override IModifyObject CreateModifyObject(SecurityDailyPlan ormbObject)
        {
            return new SecurityDailyPlanModifyObj(ormbObject);
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(SecurityDailyPlan.COLUMNNAME, true));
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.SDPS_STZS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.SdpsStzsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.SDPS_STZS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.SdpsStzsInsertDeletePerform), login);
        }

        protected override void LoadObjectsInRelationship(SecurityDailyPlan obj)
        {
            if (obj.SecurityDayIntervals != null)
            {
                IList<SecurityDayInterval> list = new List<SecurityDayInterval>();

                foreach (SecurityDayInterval securityInterval in obj.SecurityDayIntervals)
                {
                    list.Add(SecurityDayIntervals.Singleton.GetById(securityInterval.IdInterval));
                }

                obj.SecurityDayIntervals.Clear();
                foreach (SecurityDayInterval securityDayInterval in list)
                    obj.SecurityDayIntervals.Add(securityDayInterval);
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                SecurityDailyPlan secDailyPlan = GetById(idObj);
                if (secDailyPlan == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();

                ICollection<SecurityTimeZone> securityTimeZones = SecurityTimeZoneDateSettings.Singleton.GetSecurityTimeZonesFromDateSettingForSecurityDailyPlan(secDailyPlan.IdSecurityDailyPlan);
                if (securityTimeZones != null && securityTimeZones.Count > 0)
                {
                    foreach (SecurityTimeZone stz in securityTimeZones)
                    {
                        SecurityTimeZone secTimeZoneResult = SecurityTimeZones.Singleton.GetById(stz.IdSecurityTimeZone);
                        result.Add(secTimeZoneResult);
                    }
                }

                IList<CardReader> cardReaders = CardReaders.Singleton.IsReferencedSecurityDailyPlan(secDailyPlan.IdSecurityDailyPlan);
                if (cardReaders != null && cardReaders.Count > 0)
                {
                    foreach (CardReader cr in cardReaders)
                    {
                        CardReader outCardReader = CardReaders.Singleton.GetById(cr.IdCardReader);
                        result.Add(outCardReader);
                    }
                }

                var deviceAlarmSettingsSdpForEnterToMenu =
                    DevicesAlarmSettings.Singleton.SecurityDailyPlanForEnterToMenu();

                if (deviceAlarmSettingsSdpForEnterToMenu != null)
                {
                    result.Add(GetById(deviceAlarmSettingsSdpForEnterToMenu.IdSecurityDailyPlan));
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
            ICollection<SecurityDailyPlan> linqResult;

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
                        ? SelectLinq<SecurityDailyPlan>(sdp => sdp.Name.IndexOf(name) >= 0)
                        : SelectLinq<SecurityDailyPlan>(
                            sdp => sdp.Name.IndexOf(name) >= 0 ||
                                   sdp.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(sdp => sdp.Name).ToList();
                foreach (SecurityDailyPlan dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<SecurityDailyPlan> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = SelectLinq<SecurityDailyPlan>(sdp => sdp.Name.IndexOf(name) >= 0 ||
                    sdp.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<SecurityDailyPlan> linqResult = 
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<SecurityDailyPlan>(sdp => sdp.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<SecurityDailyPlan> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(sdp => sdp.Name).ToList();
                foreach (SecurityDailyPlan secDP in linqResult)
                {
                    resultList.Add(secDP);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<SecurityDailyPlanShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<SecurityDailyPlan> listSecurityDailyPlan = SelectByCriteria(filterSettings, out error);
            ICollection<SecurityDailyPlanShort> result = new List<SecurityDailyPlanShort>();
            if (listSecurityDailyPlan != null)
            {
                foreach (SecurityDailyPlan sdp in listSecurityDailyPlan)
                {
                    result.Add(new SecurityDailyPlanShort(sdp));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<SecurityDailyPlan> listSecurityDailyPlan = List(out error);
            IList<IModifyObject> listSdpModifyObj = null;
            if (listSecurityDailyPlan != null)
            {
                listSdpModifyObj = new List<IModifyObject>();
                foreach (SecurityDailyPlan sdp in listSecurityDailyPlan)
                {
                    listSdpModifyObj.Add(new SecurityDailyPlanModifyObj(sdp));
                }
                listSdpModifyObj = listSdpModifyObj.OrderBy(sdp => sdp.ToString()).ToList();
            }
            return listSdpModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.SecurityDailyPlan; }
        }
    }
}
