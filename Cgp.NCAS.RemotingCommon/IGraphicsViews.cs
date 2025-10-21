using System;
using System.Collections.Generic;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IGraphicsViews : IBaseOrmTable<GraphicsView>
    {
        ICollection<GraphicsViewShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
