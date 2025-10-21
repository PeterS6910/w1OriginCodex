using System;
using System.Collections.Generic;
using System.Linq;

using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class PresentationGroups : 
        ABaseOrmTable<PresentationGroups, PresentationGroup>, 
        IPresentationGroups
    {
        private const string GROUP_NAME = "GroupName";

        private PresentationGroups() : base(null)
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            PresentationGroup presentationGroup)
        {
            var cisNGGroups = presentationGroup.CisNGGroup;

            if (cisNGGroups != null)
                foreach (var cisNgGroup in cisNGGroups)
                    yield return cisNgGroup;

            var cisNGs = presentationGroup.CisNG;

            if (cisNGs != null)
                foreach (var cisNg in cisNGs)
                    yield return cisNg;

            var plugins = CgpServer.Singleton.PluginManager.GetLoadedPlugins();

            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    var directReferences = plugin.GetDirectReferences(presentationGroup);

                    if (directReferences == null)
                        continue;

                    foreach (var directReference in directReferences)
                    {
                        yield return directReference;
                    }
                }
            }

            yield return presentationGroup.PresentationFormatter;
        }

        protected override void LoadObjectsInRelationship(PresentationGroup obj)
        {
            if (obj.PresentationFormatter != null)
                obj.PresentationFormatter =
                    PresentationFormatters.Singleton
                        .GetById(obj.PresentationFormatter.IdFormatter);

            if (obj.SystemEvents != null)
            {
                IList<SystemEvent> list = new List<SystemEvent>();

                foreach (SystemEvent systemEvent in obj.SystemEvents)
                {
                    list.Add(SystemEvents.Singleton.GetById(systemEvent.IdSystemEvent));
                }

                obj.SystemEvents.Clear();
                foreach (SystemEvent systemEvent in list)
                    obj.SystemEvents.Add(systemEvent);
            }

            if (obj.CisNG != null)
            {
                IList<CisNG> list = new List<CisNG>();

                foreach (CisNG cisNG in obj.CisNG)
                {
                    list.Add(CisNGs.Singleton.GetById(cisNG.IdCisNG));
                }

                obj.CisNG.Clear();
                foreach (CisNG cisNG in list)
                    obj.CisNG.Add(cisNG);
            }

            if (obj.CisNGGroup != null)
            {
                IList<CisNGGroup> list = new List<CisNGGroup>();

                foreach (CisNGGroup cisNGGroup in obj.CisNGGroup)
                {
                    list.Add(CisNGGroups.Singleton.GetById(cisNGGroup.IdCisNGGroup));
                }

                obj.CisNGGroup.Clear();
                foreach (CisNGGroup cisNGGroup in list)
                    obj.CisNGGroup.Add(cisNGGroup);
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(PresentationGroup.COLUMNGROUPNAME, true));
        }

        #region ITableORM<PresentationGroup> Members

        public bool ExistPresentationGroupName(string groupName)
        {
            ICollection<PresentationGroup> ret = SelectLinq<PresentationGroup>(pg => pg.GroupName == groupName);

            if (null == ret || ret.Count == 0)
                return false;
            return true;
        }

        public ICollection<CisNG> GetCisNGs(PresentationGroup pg)
        {
            try
            {
                PresentationGroup actPG = GetById(pg.IdGroup);
                if (actPG.CisNG.Count > 0)
                {
                    ICollection<CisNG> colCisNGs = new List<CisNG>();

                    foreach (CisNG cisNG in actPG.CisNG)
                    {
                        CisNG newCisNG = CisNGs.Singleton.GetById(cisNG.IdCisNG);
                        colCisNGs.Add(newCisNG);
                    }
                    return colCisNGs;
                }
            }
            catch
            { }
            return null;
        }

        public ICollection<CisNGGroup> GetCisNGGroups(PresentationGroup pg)
        {
            try
            {
                PresentationGroup actPG = GetById(pg.IdGroup);
                if (actPG.CisNGGroup.Count > 0)
                {
                    ICollection<CisNGGroup> colCisNGGroups = new List<CisNGGroup>();

                    foreach (CisNGGroup cisNGGroup in actPG.CisNGGroup)
                    {
                        CisNGGroup newCisNGGroup = CisNGGroups.Singleton.GetById(cisNGGroup.IdCisNGGroup);
                        colCisNGGroups.Add(newCisNGGroup);
                    }
                    return colCisNGGroups;
                }
            }
            catch
            { }
            return null;
        }

        public bool AddCisNG(PresentationGroup pg, CisNG cisNG)
        {
            try
            {
                CisNG editCisNG = CisNGs.Singleton.GetById(cisNG.IdCisNG);
                PresentationGroup editPG = GetById(pg.IdGroup);
                editPG.CisNG.Add(editCisNG);
                return base.Merge(editPG);
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveCisNG(PresentationGroup pg, CisNG cisNG)
        {
            try
            {
                CisNG editcisNG = CisNGs.Singleton.GetById(cisNG.IdCisNG);
                PresentationGroup editPG = GetById(pg.IdGroup);

                for (int i = 0; i < editPG.CisNG.Count; i++)
                {
                    if (editcisNG.IdCisNG == editPG.CisNG.ElementAt(i).IdCisNG)
                    {
                        editPG.CisNG.Remove(editPG.CisNG.ElementAt(i));
                        return base.Merge(editPG);
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool AddCisNGGroup(PresentationGroup pg, CisNGGroup cisNGGroup)
        {
            try
            {
                CisNGGroup editCisNGGroup = CisNGGroups.Singleton.GetById(cisNGGroup.IdCisNGGroup);
                PresentationGroup editPG = GetById(pg.IdGroup);
                editPG.CisNGGroup.Add(editCisNGGroup);
                return base.Merge(editPG);
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveCisNGGroup(PresentationGroup pg, CisNGGroup cisNG)
        {
            try
            {
                CisNGGroup editCisNGGroup = CisNGGroups.Singleton.GetById(cisNG.IdCisNGGroup);
                PresentationGroup editPG = GetById(pg.IdGroup);

                for (int i = 0; i < editPG.CisNG.Count; i++)
                {
                    if (editCisNGGroup.IdCisNGGroup == editPG.CisNGGroup.ElementAt(i).IdCisNGGroup)
                    {
                        editPG.CisNGGroup.Remove(editPG.CisNGGroup.ElementAt(i));
                        return base.Merge(editPG);
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public ICollection<CisNG> ReturnAllCisNG(PresentationGroup pg)
        {
            ICriteria c = PrepareCriteria<CisNG>();
            c.Add(Expression.Sql("this_.IdCisNG in (select IdCisNG from CisNGCisNGGroupRelation where IdCisNGGroup in (select IdCisNGGroup from PGCisNGGroupRelation where IdGroup = '" + pg.IdGroup + "'))or (IdCisNG in (select IdCisNG from PGCisNGRelation where IdGroup = '" + pg.IdGroup + "'))"));
            ICollection<CisNG> ret = base.Select<CisNG>(c);
            return ret;
        }

        public Guid[] ReturnAllCisNGId(PresentationGroup pg)
        {
            try
            {

                ICriteria c = PrepareCriteria<CisNG>();
                c.Add(Expression.Sql("this_.IdCisNG in (select IdCisNG from CisNGCisNGGroupRelation where IdCisNGGroup in (select IdCisNGGroup from PGCisNGGroupRelation where IdGroup = '" + pg.IdGroup + "'))or (IdCisNG in (select IdCisNG from PGCisNGRelation where IdGroup = '" + pg.IdGroup + "'))"));
                ICollection<CisNG> ret = base.Select<CisNG>(c);
                var retId = new Guid[ret.Count];
                for (int i = 0; i < ret.Count; i++)
                {
                    retId[i] = ret.ElementAt(i).IdCisNG;
                }
                return retId;
            }
            catch
            {
                return null;
            }
        }

        public void RefreshRelationWithEvent(PresentationGroup pg, string eventName)
        {
            try
            {
                ICriteria c = PrepareCriteria<PresentationGroup>();
                c.Add(Expression.Sql("this_.idgroup in (select idgroup from SystemEvent where name = '" + eventName + "')and (idgroup = '" + pg.IdGroup + "')"));
                ICollection<PresentationGroup> ret = base.Select<PresentationGroup>(c);
                if (ret == null || ret.Count == 0) return;
                PerformPresentationProcessor.Singleton.CreateStore();
            }
            catch
            {
            }
        }
        #endregion

        public override bool HasAccessView(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccessesForGroup(LoginAccessGroups.PRESENT_GROUPS_FORMATTERS),
                    login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersInsertDeletePerform),
                    login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccessesForGroup(LoginAccessGroups.PRESENT_GROUPS_FORMATTERS),
                    login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.PresentGroupsFormattersInsertDeletePerform),
                    login);
        }

        public ICollection<PresentationGroup> GetPresentationGroupForPresentationFormatter(PresentationFormatter presentationFormatter)
        {
            ICollection<PresentationGroup> presentationGroups = SelectLinq<PresentationGroup>(presentationGroup => presentationGroup.PresentationFormatter == presentationFormatter);

            if (presentationGroups != null && presentationGroups.Count > 0)
            {
                return presentationGroups;
            }
            return null;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                PresentationGroup presentationGroup = GetById(idObj);
                if (presentationGroup == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();
                if (presentationGroup.SystemEvents != null && presentationGroup.SystemEvents.Count > 0)
                {
                    foreach (SystemEvent sysEvent in presentationGroup.SystemEvents)
                    {
                        SystemEvent systemEventResult = SystemEvents.Singleton.GetById(sysEvent.IdSystemEvent);
                        result.Add(systemEventResult);
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

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<PresentationGroup> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (single)
                {
                    linqResult = SelectLinq<PresentationGroup>(pq => pq.GroupName.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult = SelectLinq<PresentationGroup>(pq => pq.GroupName.IndexOf(name) >= 0
                        || pq.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(pg => pg.GroupName).ToList();
                foreach (PresentationGroup pg in linqResult)
                {
                    resultList.Add(pg);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<PresentationGroup> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = SelectLinq<PresentationGroup>(pq => pq.GroupName.IndexOf(name) >= 0
                    || pq.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<PresentationGroup> linqResult = 
                string.IsNullOrEmpty(name) 
                    ? List() : 
                    SelectLinq<PresentationGroup>(pq => pq.GroupName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<PresentationGroup> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(pg => pg.GroupName).ToList();
                foreach (PresentationGroup pg in linqResult)
                {
                    resultList.Add(pg);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public void SendCisInfoMessage(PresentationGroup noDbsPg, string msg)
        {
#if !DEBUG
            string localisedName = string.Empty;
            object value = null;
            if (CgpServer.Singleton.GetLicencePropertyInfo(RequiredLicenceProperties.CISIntegration.ToString(), out localisedName, out value))
            {
                if (value is bool && (bool)value == true)
#endif
                    PerformPresentationProcessor.Singleton.ProcessSendMessageNoDbs(new NoDbsPG(noDbsPg), msg);
#if !DEBUG
            }
#endif
        }

        public ICollection<PresentationGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<PresentationGroup> listPresentationGroup = SelectByCriteria(filterSettings, out error);
            ICollection<PresentationGroupShort> result = new List<PresentationGroupShort>();
            if (listPresentationGroup != null)
            {
                foreach (PresentationGroup presentationGroup in listPresentationGroup)
                {
                    result.Add(new PresentationGroupShort(presentationGroup));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<PresentationGroup> listPg = List(out error);
            IList<IModifyObject> listPgModifyObj = null;
            if (listPg != null)
            {
                listPgModifyObj = new List<IModifyObject>();
                foreach (PresentationGroup pg in listPg)
                {
                    listPgModifyObj.Add(new PresentationGroupModifyObj(pg));
                }
                listPgModifyObj = listPgModifyObj.OrderBy(pg => pg.ToString()).ToList();
            }
            return listPgModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.PresentationGroup; }
        }
    }
}
