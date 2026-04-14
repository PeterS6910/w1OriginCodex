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
    public sealed class Scenes :
        ANcasBaseOrmTable<Scenes, Scene>, 
        IScenes
    {
        private Scenes() : base(null)
        {
        }

        protected override IModifyObject CreateModifyObject(Scene ormbObject)
        {
            return new SceneModifyObj(ormbObject);
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
            var listScenes = List(out error);
            var listScenesModifyObj = new List<IModifyObject>();
            if (listScenes != null)
            {
                foreach (var scene in listScenes)
                {
                    listScenesModifyObj.Add(new SceneModifyObj(scene));
                }
                listScenesModifyObj = listScenesModifyObj.OrderBy(scene => scene.ToString()).ToList();
            }
            return listScenesModifyObj;
        }

        public ICollection<SceneShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<Scene> listScene = SelectByCriteria(filterSettings, out error);
            ICollection<SceneShort> result = new List<SceneShort>();
            if (listScene != null)
            {
                foreach (Scene scene in listScene)
                {
                    var newSceneShort = new SceneShort(scene);
                    result.Add(newSceneShort);
                }
            }
            return result;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                return new LinkedList<AOrmObject>(List().OrderBy(scene => scene.Name).Cast<AOrmObject>());
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<Scene>(scene => scene.Name.IndexOf(name) >= 0)
                    : SelectLinq<Scene>(scene => scene.Name.IndexOf(name) >= 0 || scene.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(linqResult.OrderBy(scene => scene.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Scene> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<Scene>(scene => scene.Name.IndexOf(name) >= 0 || scene.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(linqResult.OrderBy(scene => scene.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<Scene> linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<Scene>(scene => scene.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(linqResult.OrderBy(scene => scene.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.Scene; }
        }
    }
}
