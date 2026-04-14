using System;

using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IPresentationFormatters : IBaseOrmTable<PresentationFormatter>
    {
        bool ExistPresentationFormatterName(string formatterName);

        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
