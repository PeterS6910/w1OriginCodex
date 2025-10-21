using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class GraphicsViews :
        ANcasBaseOrmTable<GraphicsViews, GraphicsView>,
        IGraphicsViews
    {
        private GraphicsViews()
            : base(null)
        {
        }

        protected override IModifyObject CreateModifyObject(GraphicsView ormbObject)
        {
            return new ViewModifyObj(ormbObject);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.WPF_GRAPHICS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.WpfGraphicsAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.WpfGraphicsAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.WpfGraphicsAdmin), login);
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listViews = List(out error);
            var listViewsModifyObj = new List<IModifyObject>();
            if (listViews != null)
            {
                foreach (var view in listViews)
                {
                    listViewsModifyObj.Add(new ViewModifyObj(view));
                }
                listViewsModifyObj = listViewsModifyObj.OrderBy(view => view.ToString()).ToList();
            }
            return listViewsModifyObj;
        }

        public ICollection<GraphicsViewShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<GraphicsView> listView = SelectByCriteria(filterSettings, out error);
            ICollection<GraphicsViewShort> result = new List<GraphicsViewShort>();
            if (listView != null)
            {
                foreach (GraphicsView view in listView)
                {
                    var newViewShort = new GraphicsViewShort(view);
                    result.Add(newViewShort);
                }
            }
            return result;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                return new LinkedList<AOrmObject>(List().OrderBy(view => view.Name).Cast<AOrmObject>());
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<GraphicsView>(view => view.Name.IndexOf(name) >= 0)
                    : SelectLinq<GraphicsView>(view => view.Name.IndexOf(name) >= 0 || view.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(linqResult.OrderBy(view => view.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<GraphicsView> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<GraphicsView>(view => view.Name.IndexOf(name) >= 0 || view.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(linqResult.OrderBy(view => view.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<GraphicsView> linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<GraphicsView>(view => view.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(linqResult.OrderBy(view => view.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.GraphicsView; }
        }
    }
}
