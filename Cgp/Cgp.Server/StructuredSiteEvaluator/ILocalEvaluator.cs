using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public interface ILocalEvaluator
    {
        void OnObjectAdded(GlobalEvaluator.GlobalObjectInfo globalObjectInfo);
        void OnObjectUpdated(GlobalEvaluator.GlobalObjectInfo globalObjectInfo);
        void OnObjectDeleted(IdAndObjectType parameter);

        void OnSiteCreated(GlobalEvaluator.GlobalSiteInfo globalSiteInfo);
        void OnSiteUpdated(GlobalEvaluator.GlobalSiteInfo globalSiteInfo);
        void OnSiteRemoved(GlobalEvaluator.GlobalSiteInfo globalSiteInfo);

        void OnBeginSave();
        void OnEndSave();

        void OnObjectMoved(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo oldSiteInfo);

        void OnObjectParentChanged(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            IdAndObjectType oldParent);

        void OnObjectRenamed(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo);

        void OnSubTreeReferenceAdded(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo, 
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo);

        void OnSubTreeReferenceRemoved(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo, 
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo);

        void OnDirectBrokenReferencesChanged(GlobalEvaluator.GlobalObjectInfo globalObjectInfo);

        void OnBeginUpdate();

        void OnEndUpdate();

        void OnUserFoldersStructureChanged();

        void SetClient(IStructuredSiteBuilderClient structuredSiteBuilderClient);

        IStructuredSiteBuilder Builder { get; }
    }
}