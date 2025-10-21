using System;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICardSystems : IBaseOrmTable<CardSystem>
    {
        ICollection<CardSystemShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
        byte GetCardSystemNumber();
        CardSystem GetCardSystemFromNumber(byte cardSystemNumber);
        CardSystem GetCardSystemByCard(string fullCardNumber);
        bool IsMifareSectorDataInCollision(MifareSectorData sectorData);
    }
}
