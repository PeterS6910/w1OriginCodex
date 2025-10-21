using System;
using System.Collections.Generic;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICardTemplates : IBaseOrmTable<CardTemplate>
    {
        string[] GetAllCardTemplateNames();

        ICollection<CardTemplateShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        CardTemplateShort ShortGetById(object id, out Exception ex);
    }
}
