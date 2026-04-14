using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    partial class LocalEvaluator
    {
        public class LocalSiteInfo :
            ASiteInfo<
                LocalSiteInfo,
                SaveableSubset<IdAndObjectType>>
        {
            private GlobalEvaluator.GlobalSiteInfo _globalSiteInfo;

            public LocalSiteInfo(GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
                : base(
                    new SaveableSubset<IdAndObjectType>(globalSiteInfo.SubTreeReferencesSet),
                    new SaveableSubset<IdAndObjectType>(globalSiteInfo.ObjectsSet))
            {
                _globalSiteInfo = globalSiteInfo;
            }

            public LocalSiteInfo(int id, string name)
                : base(
                    new SaveableSubset<IdAndObjectType>(),
                    new SaveableSubset<IdAndObjectType>())
            {
                _id = id;
                _name = name;
            }

            public GlobalEvaluator.GlobalSiteInfo GlobalSiteInfo
            {
                get { return _globalSiteInfo; }
            }

            public int _id;

            public override int Id
            {
                get
                {
                    return
                        _globalSiteInfo != null
                            ? _globalSiteInfo.Id
                            : _id;
                }
            }

            private string _name;

            public override string Name
            {
                get
                {
                    return _name ?? _globalSiteInfo.Name;
                }

                set { _name = value; }
            }

            protected override LocalSiteInfo This
            {
                get
                {
                    return this;
                }
            }

            public void OnCreated(
                LocalEvaluator localEvaluator,
                GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
            {
                if (_globalSiteInfo != null)
                    return;

                int oldId = _id;

                _globalSiteInfo = globalSiteInfo;

                UpdateId(localEvaluator, oldId);
            }

            private void UpdateId(
                LocalEvaluator localEvaluator,
                int oldId)
            {
                localEvaluator.RenumberSiteId(this, oldId);
                ParentSite.RenumberChildId(this, oldId);

                foreach (var idAndObjectType in SubTreeReferences)
                    localEvaluator
                        .GetObjectInfo(idAndObjectType)
                        .RenumberSubTreeReferenceSiteId(this, oldId);
            }

            public void SaveSiteInfo(LocalEvaluator localEvaluator)
            {
                if (_globalSiteInfo == null)
                {
                    localEvaluator.SaveCreateSite(this);
                }
                else
                {
                    if (_name != null)
                        localEvaluator.SaveUpdateSite(this);
                }

                if (_globalSiteInfo == null)
                    return;

                var childrenSites = new LinkedList<LocalSiteInfo>(ChildrenSites);

                foreach (var localSiteInfo in childrenSites)
                    localSiteInfo.SaveSiteInfo(localEvaluator);
            }

            public void SaveSiteContent(GlobalEvaluator globalEvaluator)
            {
                var addedObjects =
                    new LinkedList<IdAndObjectType>(_objects.AddedElements);

                foreach (var idAndObjectType in addedObjects)
                    globalEvaluator.SaveMoveObject(
                        idAndObjectType,
                        _globalSiteInfo);

                var addedSubTreeReferences =
                    new LinkedList<IdAndObjectType>(_subTreeReferences.AddedElements);

                foreach (var idAndObjectType in addedSubTreeReferences)
                {
                    var globalObjectInfo = globalEvaluator.GetObjectInfo(idAndObjectType);

                    if (globalObjectInfo == null)
                        continue;

                    globalEvaluator.AddSubTreeReference(
                        globalObjectInfo,
                        _globalSiteInfo);
                }

                var removedSubTreeReferences =
                    new LinkedList<IdAndObjectType>(_subTreeReferences.RemovedElements);

                foreach (var idAndObjectType in removedSubTreeReferences)
                {
                    var globalObjectInfo =
                        globalEvaluator.GetObjectInfo(idAndObjectType);

                    if (globalObjectInfo == null)
                        continue;

                    globalEvaluator.RemoveSubTreeReference(
                        globalObjectInfo,
                        _globalSiteInfo);
                }

                _subTreeReferences.OnSaved(_globalSiteInfo.SubTreeReferencesSet);

                foreach (var localSiteInfo in ChildrenSites)
                    localSiteInfo.SaveSiteContent(globalEvaluator);
            }

            public void ObjectsOnSaved()
            {
                _objects.OnSaved(_globalSiteInfo.ObjectsSet);

                foreach (var localSiteInfo in ChildrenSites)
                    localSiteInfo.ObjectsOnSaved();
            }

            public void SetIdIfNew(
                LocalEvaluator localEvaluator,
                int siteId)
            {
                if (_globalSiteInfo != null)
                    return;

                int oldId = _id;

                _id = siteId;

                UpdateId(
                    localEvaluator,
                    oldId);
            }

            public void UpdateSets()
            {
                _objects.Update();
                _subTreeReferences.Update();
            }

            public void SaveRemovedSites(LocalEvaluator localEvaluator)
            {
                var globalSiteInfos =
                    new LinkedList<GlobalEvaluator.GlobalSiteInfo>(
                        _globalSiteInfo.ChildrenSites);

                foreach (var globalSiteInfo in globalSiteInfos)
                    if (!HasChild(globalSiteInfo.Id))
                        localEvaluator.SaveRemoveSite(globalSiteInfo);

                foreach (var childSiteInfo in ChildrenSites)
                    childSiteInfo.SaveRemovedSites(localEvaluator);
            }
        }
    }
}
