using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICardPairs
    {
        bool Delete(CardPair ormObject, out Exception error);
        bool DeleteById(Object objId, out Exception deleteException);
        bool Insert(ref CardPair ormObject, out Exception error);
        bool Update(CardPair ormObject, out Exception error);
        CardPair GetCardPairForCard(Guid cardId, out Exception error);
        Card GetRelatedCard(Guid toCard, out Exception error);
        CardPair[] GetAllCardPairs();
        HashSet<Guid> GetAllPairedCards();
        bool SetRelatedCard(Guid toCard, Guid? relatedCard);
        bool IsCardRelated(Guid cardId);
    }
}
