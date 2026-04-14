using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.Server.DB
{
    public sealed class CarCards : ABaseOrmTable<CarCards, CarCard>, ICarCards
    {
        private CarCards() : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return HasAccessView(login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return HasAccessView(login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return HasAccessView(login);
        }

        protected override void LoadObjectsInRelationship(CarCard obj)
        {
            if (obj.Car != null)
                obj.Car = Cars.Singleton.GetById(obj.Car.IdCar);
            if (obj.Card != null)
                obj.Card = Cards.Singleton.GetById(obj.Card.IdCard);
        }

        public IList<Card> GetCardsForCar(Guid idCar, out Exception error)
        {
            error = null;
            var relations = SelectLinq<CarCard>(cc => cc.Car.IdCar == idCar);
            var result = new List<Card>();
            if (relations != null)
            {
                foreach (var cc in relations)
                {
                    if (cc.Card != null)
                    {
                        var card = Cards.Singleton.GetCardForClient(cc.Card.IdCard);
                        if (card != null)
                        {
                            card.PrepareToSend();
                            result.Add(card);
                        }
                    }
                }
            }
            return result;
        }

        public IList<Card> GetAvailableCards(Guid idCar, out Exception error)
        {
            error = null;
            var allCards = Cards.Singleton.List();
            var allRelations = List();
            var used = new HashSet<Guid>();
            if (allRelations != null)
                foreach (var rel in allRelations)
                    if (rel.Card != null)
                        used.Add(rel.Card.IdCard);

            var result = new List<Card>();
            if (allCards != null)
            {
                foreach (var card in allCards)
                {
                    if (!used.Contains(card.IdCard))
                    {
                        var cardClient = Cards.Singleton.GetCardForClient(card.IdCard);
                        if (cardClient != null)
                        {
                            cardClient.PrepareToSend();
                            result.Add(cardClient);
                        }
                    }
                }
            }
            return result;
        }

        public bool AssignCardToCar(Guid idCar, Guid idCard)
        {
            try
            {
                var car = Cars.Singleton.GetById(idCar);
                var card = Cards.Singleton.GetById(idCard);
                if (car == null || card == null)
                    return false;
                var rel = new CarCard { Car = car, Card = card };
                Exception error;
                return Insert(rel, out error);
            }
            catch
            {
                return false;
            }
        }

        public bool UnassignCardFromCar(Guid idCar, Guid idCard)
        {
            try
            {
                var relations = SelectLinq<CarCard>(cc => cc.Car.IdCar == idCar && cc.Card.IdCard == idCard);
                if (relations == null || relations.Count == 0)
                    return true;
                var relationIds = relations
                    .Where(rel => rel != null)
                    .Select(rel => rel.IdCarCard)
                    .ToList();
                foreach (var relationId in relationIds)
                {
                    Exception err;
                    if (!DeleteById(relationId, out err))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.NotSupport; }
        }
    }
}
