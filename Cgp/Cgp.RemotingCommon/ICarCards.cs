using System;
using System.Collections.Generic;

using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICarCards : IBaseOrmTable<CarCard>
    {
        IList<Card> GetCardsForCar(Guid idCar, out Exception error);
        IList<Card> GetAvailableCards(Guid idCar, out Exception error);
        bool AssignCardToCar(Guid idCar, Guid idCard);
        bool UnassignCardFromCar(Guid idCar, Guid idCard);
    }
}
