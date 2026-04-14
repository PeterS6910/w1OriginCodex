using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class GraphicSymbolRawDatas :
        ANcasBaseOrmTable<GraphicSymbolRawDatas, GraphicSymbolRawData>, 
        IGraphicSymbolRawDatas
    {
        private GraphicSymbolRawDatas() : base(null)
        {
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

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.GraphicSymbolRawData; }
        }
    }
}
