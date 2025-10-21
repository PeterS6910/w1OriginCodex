using System;
using System.Collections.Generic;
using Contal.Cgp.NCAS.CCU.DB;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IApbzStorage
    {
        void AddCard(
            CCUCardInAntiPassBackZone ccuCardInAntiPassBackZone,
            Guid guidAntiPassBackZone);

        void RemoveCard(
            Guid idCard,
            Guid idAntiPassBackZone);

        ICollection<StoredCardInAntiPassBackZone> LoadCards();
    }
}
