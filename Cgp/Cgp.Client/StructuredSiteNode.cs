using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public enum StructuredSiteNodeType : byte
    {
        SubSiteNode = 0,
        SubSitesFolderNode = 1,
        SubTreeReferencingFolderNode = 2,
        ObjectTypeFolderNode = 3,
        OrmObjectNode = 4,
        HardwareFolderNode = 5,
        FolderStructuresFolderNode = 6,
        FolderStructureNode = 7
    }

    public class StructuredSiteNode : TreeNode
    {
        public StructuredSiteNodeType StructuredSiteNodeType { get; private set; }
        public StructuredSiteNode OtherTreeViewNode { get; set; }
        public byte PriorityForOrderBy { get; private set; }

        public StructuredSiteNode(string text, StructuredSiteNode otherTreeViewNode,
            StructuredSiteNodeType structuredSiteNodeType, byte priorityForOrderBy)
            : base(text)
        {
            OtherTreeViewNode = otherTreeViewNode;
            StructuredSiteNodeType = structuredSiteNodeType;
            PriorityForOrderBy = priorityForOrderBy;
        }
    }
}
