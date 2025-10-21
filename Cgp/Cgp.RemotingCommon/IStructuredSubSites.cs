using System;
using System.Collections.Generic;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IStructuredSubSites : IBaseOrmTable<StructuredSubSite>
    {
        bool InsertStructuredSubSiteObject(StructuredSubSiteObject structuredSubSiteObject);

        ICollection<StructuredSubSite> GetAllSubSitesForLogin(out bool isInRootSite);

        IStructuredSiteBuilder GetBuilder(IStructuredSiteBuilderClient client);

        ICollection<ObjectPlacement> GetObjectPlacements(
            ObjectType objectType,
            string objectIdString,
            string separator,
            string rootName,
            bool onlyInSite);

        bool IsAlarmVisibleForSites(
            IEnumerable<IdAndObjectType> parentObjects,
            ICollection<int> subSitesFilter);
    }

    [Serializable]
    public class SiteIdAndName
    {
        public int Id { get; private set; } 
        public string Name { get; private set; }

        public SiteIdAndName(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    [Serializable]
    public class ObjectInSiteInfo : IdAndObjectType
    {
        public string Name { get; private set; }
        public IdAndObjectType Parent { get; private set; }

        public ObjectInSiteInfo(
            object id, 
            ObjectType objectType,
            string name,
            IdAndObjectType parent)
            : base(id, objectType)
        {
            Name = name;
            Parent = parent;
        }
    }

    [Serializable]
    public class ReferenceInSiteInfo : ObjectInSiteInfo
    {
        public bool DefinedInThisSite { get; private set; }

        public ReferenceInSiteInfo(
            object id,
            ObjectType objectType,
            string name,
            IdAndObjectType parent,
            bool definedInThisSite)
            : base(id, objectType, name, parent)
        {
            DefinedInThisSite = definedInThisSite;
        }
    }

    [Serializable]
    public class ObjectPlacement
    {
        public int SiteId { get; private set; }
        public string Name { get; private set; }
        public bool IsReference { get; private set; }
        public IdAndObjectType Parent { get; private set; }

        public ObjectPlacement(int siteId, string name, bool isReference, IdAndObjectType parent)
        {
            SiteId = siteId;
            Name = name;
            IsReference = isReference;
            Parent = parent;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public interface IStructuredSiteBuilderClient
    {
        void StructureChanged();

        void TopMostSitesChanged();

        void SiteCreated(SiteIdAndName siteIdAndName, int parentSiteId);
        void SiteRenamed(SiteIdAndName siteIdAndName);
        void SiteDeleted(int siteId);

        void ObjectAdded(ObjectInSiteInfo objectInSiteInfo, int siteId);
        void ObjectRemoved(IdAndObjectType idAndObjectType, int siteId);
        void ObjectRenamed(IdAndObjectType idAndObjectType, string newName);

        void SubTreeReferenceAdded(
            ReferenceInSiteInfo referenceInSiteInfo, 
            int siteId);

        void SubTreeReferenceRemoved(
            IdAndObjectType idAndObjectType, 
            int siteId, 
            bool ancestralReferencesExist);

        void BrokenDirectReferencesForObjectsChanged(
            IEnumerable<BrokenReferencesInfo> objectsWithBrokenReferences);

        void ObjectParentChanged(
            IdAndObjectType idAndObjectType,
            IdAndObjectType newParent,
            IdAndObjectType oldParent);

        void BeginUpdate();
        void EndUpdate();
    }

    [Serializable]
    public class BrokenReferencesInfo : ObjectInSiteInfo
    {
        public BrokenReferencesInfo(
            IdAndObjectType idAndObjectType, 
            string name, 
            IdAndObjectType parent, 
            IEnumerable<ObjectInSiteInfo> brokenReferences)
            : base(idAndObjectType.Id, idAndObjectType.ObjectType, name, parent)
        {
            BrokenReferences = brokenReferences;
        }

        public IEnumerable<ObjectInSiteInfo> BrokenReferences { get; private set; }
    }

    public interface IStructuredSiteBuilder
    {
        int CreateSubSite(int parentSiteId, string name);
        void RenameSite(int siteId, string newName);
        void DeleteSite(int siteId);

        void MoveObject(IdAndObjectType objectIds, int newSiteId);
        void AddSubTreeReference(IdAndObjectType objectId, int siteId);
        void RemoveSubTreeReference(IdAndObjectType objectId, int siteId);

        IEnumerable<SiteIdAndName> GetTopMostSites();
        IEnumerable<SiteIdAndName> GetSubSites(int siteId);

        IEnumerable<ObjectInSiteInfo> GetObjectsInSite(int siteId);

        int GetSubTreeReferenceCount(IdAndObjectType objectId);

        IEnumerable<ObjectInSiteInfo> GetSubTreeReferences(int siteId);
        IEnumerable<ReferenceInSiteInfo> GetAllSubTreeReferences(int siteId);

        IEnumerable<BrokenReferencesInfo>
            GetObjectsWithBrokenDirectReferences();

        int GetSiteOfObject(IdAndObjectType objectId);

        void CreateFolder(int siteId, IdAndObjectType parentFolderObject, string folderName);
        void RemoveFolder(IdAndObjectType objectId);
        void RenameFolder(IdAndObjectType objectId, string newFolderName);
        void AddObjectToFolder(IdAndObjectType objectId, IdAndObjectType folderId);
        void RemoveObjectFromFolder(IdAndObjectType objectId, IdAndObjectType folderId);

        void Save();
        void Cancel();
    }
}
