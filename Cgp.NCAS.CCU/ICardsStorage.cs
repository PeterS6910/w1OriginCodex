using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU
{
    public interface ICardsStorage
    {
        ICard GetCard(String fullCardNumber);
        ICard GetFirstCard();
        string GetFullCardNumber(Guid idCard);
        ICollection<Guid> GetCardIdsByState(byte state);
    }
}