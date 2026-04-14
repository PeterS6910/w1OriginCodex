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
    public sealed class CisNGGroups : 
        ABaserOrmTableWithAlarmInstruction<CisNGGroups, CisNGGroup>, 
        ICisNGGroups
    {
        private CisNGGroups() : base(null)
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            CisNGGroup cisNGGroup)
        {
            var cisNGs = cisNGGroup.CisNG;

            if (cisNGs != null)
                foreach (var cisNg in cisNGs)
                    yield return cisNg;
        }

        protected override void LoadObjectsInRelationship(CisNGGroup obj)
        {
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

            if (obj.PresentationGroup != null)
            {
                IList<PresentationGroup> list = new List<PresentationGroup>();

                foreach (PresentationGroup presentationGroup in obj.PresentationGroup)
                {
                    list.Add(PresentationGroups.Singleton.GetById(presentationGroup.IdGroup));
                }

                obj.PresentationGroup.Clear();
                foreach (PresentationGroup presentationGroup in list)
                    obj.PresentationGroup.Add(presentationGroup);
            }
        }

        public bool Merge(CisNGGroup obj)
        {
            return base.Merge(obj);
        }

        public ICollection<CisNG> GetCisNGs(CisNGGroup cisNGGroup)
        {
            try
            {
                CisNGGroup cisGroup = GetById(cisNGGroup.IdCisNGGroup);
                if (cisGroup.CisNG.Count > 0)
                {
                    ICollection<CisNG> colCisNGs = new List<CisNG>();

                    foreach (CisNG cisNG in cisGroup.CisNG)
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

        public bool AddCisNG(CisNGGroup cisNGGroup, CisNG cisNG)
        {
            try
            {
                CisNG editCisNG = CisNGs.Singleton.GetById(cisNG.IdCisNG);
                CisNGGroup editcisNGGroup = GetById(cisNGGroup.IdCisNGGroup);
                editcisNGGroup.CisNG.Add(editCisNG);
                return base.Merge(editcisNGGroup);
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveCisNG(CisNGGroup cisNGGroup, CisNG cisNG)
        {
            try
            {
                CisNG editcisNG = CisNGs.Singleton.GetById(cisNG.IdCisNG);
                CisNGGroup editcisNGGroup = GetById(cisNGGroup.IdCisNGGroup);

                for (int i = 0; i < editcisNGGroup.CisNG.Count; i++)
                {
                    if (editcisNG.IdCisNG == editcisNGGroup.CisNG.ElementAt(i).IdCisNG)
                    {
                        editcisNGGroup.CisNG.Remove(editcisNGGroup.CisNG.ElementAt(i));
                        return base.Merge(editcisNGGroup);
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(CisNGGroup.COLUMNGROUPNAME, true));
        }

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

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                CisNGGroup cisNgGroup = GetById(idObj);
                if (cisNgGroup == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();
                if (cisNgGroup.PresentationGroup != null && cisNgGroup.PresentationGroup.Count > 0)
                {
                    foreach (PresentationGroup pg in cisNgGroup.PresentationGroup)
                    {
                        PresentationGroup presentationGroupResult = PresentationGroups.Singleton.GetById(pg.IdGroup);
                        result.Add(presentationGroupResult);
                    }
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
            ICollection<CisNGGroup> linqResult;

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
                    linqResult =
                        SelectLinq<CisNGGroup>(
                            cngg => cngg.GroupName.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult =
                        SelectLinq<CisNGGroup>(
                            cngg =>
                                cngg.GroupName.IndexOf(name) >= 0 ||
                                cngg.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cis => cis.GroupName).ToList();
                foreach (CisNGGroup dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<CisNGGroup> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<CisNGGroup>(
                        cngg =>
                            cngg.GroupName.IndexOf(name) >= 0 ||
                            cngg.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<CisNGGroup> linqResult = 
                string.IsNullOrEmpty(name) 
                    ? List() 
                    : SelectLinq<CisNGGroup>(cngg => cngg.GroupName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<CisNGGroup> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cis => cis.GroupName).ToList();
                foreach (CisNGGroup cisNGGroup in linqResult)
                {
                    resultList.Add(cisNGGroup);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<CisNGGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<CisNGGroup> listCisNGGroup = SelectByCriteria(filterSettings, out error);
            ICollection<CisNGGroupShort> result = new List<CisNGGroupShort>();
            if (listCisNGGroup != null)
            {
                foreach (CisNGGroup cisNGGroup in listCisNGGroup)
                {
                    result.Add(new CisNGGroupShort(cisNGGroup));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<CisNGGroup> listCisNGGroup = List(out error);
            IList<IModifyObject> listCisNGGroupModifyObj = null;
            if (listCisNGGroup != null)
            {
                listCisNGGroupModifyObj = new List<IModifyObject>();
                foreach (CisNGGroup cisNGGroup in listCisNGGroup)
                {
                    listCisNGGroupModifyObj.Add(new CisNGGroupModifyObj(cisNGGroup));
                }
                listCisNGGroupModifyObj = listCisNGGroupModifyObj.OrderBy(cisNGGroup => cisNGGroup.ToString()).ToList();
            }
            return listCisNGGroupModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CisNGGroup; }
        }

        protected override IEnumerable<CisNGGroup> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<CisNGGroup>(
                cisNgGroup =>
                    cisNgGroup.LocalAlarmInstruction != null
                    && cisNgGroup.LocalAlarmInstruction != string.Empty);
        }
    }
}
