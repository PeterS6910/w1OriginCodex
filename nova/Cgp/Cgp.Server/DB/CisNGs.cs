using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using NHibernate.Criterion;
using NHibernate;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class CisNGs : 
        ABaserOrmTableWithAlarmInstruction<CisNGs, CisNG>, 
        ICisNGs
    {
        private CisNGs() : base(null)
        {
        }

        protected override void LoadObjectsInRelationship(CisNG obj)
        {
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

            var presentationGroups = obj.PresentationGroup;

            if (presentationGroups != null)
            {
                IList<PresentationGroup> list = new List<PresentationGroup>();

                foreach (PresentationGroup presentationGroup in presentationGroups)
                {
                    list.Add(PresentationGroups.Singleton.GetById(presentationGroup.IdGroup));
                }

                presentationGroups.Clear();

                foreach (PresentationGroup presentationGroup in list)
                    presentationGroups.Add(presentationGroup);
            }
        }

        #region ITableORM<CisNG> Members

        public int ReturnCisNGState(CisNG cis)
        {
            return PerformPresentationProcessor.Singleton.ReturnCisNGStatus(cis);
        }

        #endregion

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsInsertDeletePerfrom),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.CisNgsCisNgGroupsInsertDeletePerfrom),
                login);
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(CisNG.COLUMNCISNGNAME, true));
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                CisNG cisNG = GetById(idObj);
                if (cisNG == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();
                if (cisNG.CisNGGroup != null && cisNG.CisNGGroup.Count > 0)
                {
                    foreach (CisNGGroup cisGroup in cisNG.CisNGGroup)
                    {
                        CisNGGroup cisNGGroupResult = CisNGGroups.Singleton.GetById(cisGroup.IdCisNGGroup);
                        result.Add(cisNGGroupResult);
                    }
                }

                if (cisNG.PresentationGroup != null && cisNG.PresentationGroup.Count > 0)
                {
                    foreach (PresentationGroup pg in cisNG.PresentationGroup)
                    {
                        PresentationGroup presentationGroupResult = PresentationGroups.Singleton.GetById(pg.IdGroup);
                        result.Add(presentationGroupResult);
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
            ICollection<CisNG> linqResult;

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
                        ? SelectLinq<CisNG>(
                            cng => cng.CisNGName.IndexOf(name) >= 0)
                        : SelectLinq<CisNG>(
                            cng =>
                                cng.CisNGName.IndexOf(name) >= 0 ||
                                cng.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cis => cis.CisNGName).ToList();
                foreach (CisNG cisNG in linqResult)
                {
                    resultList.Add(cisNG);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<CisNG> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<CisNG>(
                        cng =>
                            cng.CisNGName.IndexOf(name) >= 0 ||
                            cng.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<CisNG> linqResult = 
                string.IsNullOrEmpty(name)
                    ? List() 
                    : SelectLinq<CisNG>(
                        cng => cng.CisNGName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<CisNG> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cis => cis.CisNGName).ToList();
                foreach (CisNG cisNG in linqResult)
                {
                    resultList.Add(cisNG);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<CisNGShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<CisNG> listCisNG = SelectByCriteria(filterSettings, out error);
            ICollection<CisNGShort> result = new List<CisNGShort>();
            if (listCisNG != null)
            {
                foreach (CisNG cisNG in listCisNG)
                {
                    result.Add(new CisNGShort(cisNG));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<CisNG> listCisNG = List(out error);
            IList<IModifyObject> listCisNGModifyObj = null;
            if (listCisNG != null)
            {
                listCisNGModifyObj = new List<IModifyObject>();
                foreach (CisNG cisNG in listCisNG)
                {
                    listCisNGModifyObj.Add(new CisNGModifyObj(cisNG));
                }
                listCisNGModifyObj = listCisNGModifyObj.OrderBy(cisNG => cisNG.ToString()).ToList();
            }
            return listCisNGModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CisNG; }
        }

        protected override IEnumerable<CisNG> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<CisNG>(
                cisNg =>
                    cisNg.LocalAlarmInstruction != null
                    && cisNg.LocalAlarmInstruction != string.Empty);
        }
    }
}
