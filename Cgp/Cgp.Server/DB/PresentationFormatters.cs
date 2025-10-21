using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class PresentationFormatters : 
        ABaseOrmTable<PresentationFormatters, PresentationFormatter>, 
        IPresentationFormatters
    {
        private const string FORMATER_NAME = "FormatterName";

        #region ITableORM<PresentationFormatter> Members

        private PresentationFormatters() : base(null)
        {
        }

        public PresentationFormatter GetByName(String formatterName)
        {
            return GetByPk<PresentationFormatter>(formatterName);
        }

        public bool ExistPresentationFormatterName(string formatterName)
        {
            ICollection<PresentationFormatter> ret = SelectLinq<PresentationFormatter>(pf => pf.FormatterName == formatterName);

            if (null == ret || ret.Count == 0)
                return false;
            return true;
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

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(PresentationFormatter.COLUMNFORMATTERNAME, true));
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                PresentationFormatter pf = GetById(idObj);
                if (pf == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();
                ICollection<PresentationGroup> presentationGroups = PresentationGroups.Singleton.GetPresentationGroupForPresentationFormatter(pf);

                if (presentationGroups != null && presentationGroups.Count > 0)
                {
                    foreach (PresentationGroup pg in presentationGroups)
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
            ICollection<PresentationFormatter> linqResult;

            if (single && (name == null || name == string.Empty))
            {
                linqResult = List();
            }
            else
            {
                if (name == null || name == string.Empty)
                    return null;

                if (single)
                {
                    linqResult = SelectLinq<PresentationFormatter>(pf => pf.FormatterName.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult = SelectLinq<PresentationFormatter>(pf => pf.FormatterName.IndexOf(name) >= 0
                        || pf.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(pf => pf.FormatterName).ToList();
                foreach (PresentationFormatter pf in linqResult)
                {
                    resultList.Add(pf);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<PresentationFormatter> linqResult = null;

            if (name != null && name != string.Empty)
            {
                linqResult = SelectLinq<PresentationFormatter>(pf => pf.FormatterName.IndexOf(name) >= 0
                    || pf.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<PresentationFormatter> linqResult = null;

            if (name == null || name == string.Empty)
            {
                linqResult = base.List();
            }
            else
            {
                linqResult = base.SelectLinq<PresentationFormatter>(pf => pf.FormatterName.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(ICollection<PresentationFormatter> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(pf => pf.FormatterName).ToList();
                foreach (PresentationFormatter pf in linqResult)
                {
                    resultList.Add(pf);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<PresentationFormatter> listPf = List(out error);
            IList<IModifyObject> listPfModifyObj = null;
            if (listPf != null)
            {
                listPfModifyObj = new List<IModifyObject>();
                foreach (PresentationFormatter pf in listPf)
                {
                    listPfModifyObj.Add(new PresentationFormatterModifyObj(pf));
                }
                listPfModifyObj = listPfModifyObj.OrderBy(pf => pf.ToString()).ToList();
            }
            return listPfModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.PresentationFormatter; }
        }
    }
}
