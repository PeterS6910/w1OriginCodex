using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;

using NHibernate;
using NHibernate.Criterion;
using Contal.IwQuick.Sys;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class CardPairs : 
        ABaseOrmTable<CardPairs, CardPair>, 
        ICardPairs
    {
        private CardPairs() : base(null)
        {
        }

        public override object ParseId(string strObjectId)
        {
            throw new NotImplementedException("TODO");
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public CardPair GetCardPairForCard(Guid cardId)
        {
            Exception ex;
            return GetCardPairForCard(cardId, out ex);
        }

        public CardPair GetCardPairForCard(Guid cardId, out Exception error)
        {
            var filterSettings = new List<FilterSettings>
            {
                new FilterSettings(CardPair.COLUMNFIRSTCARD, cardId, ComparerModes.EQUALL)
            };
            var result = SelectByCriteria(filterSettings, out error);

            if (result == null || result.Count == 0)
                return null;

            return result.ElementAt(0);
        }

        protected override bool AddCriteriaSpecial(ref ICriteria c, FilterSettings filterSetting)
        {
            if (filterSetting == null || c == null)
                return false;

            if (filterSetting.Column == CardPair.COLUMNFIRSTCARD || filterSetting.Column == CardPair.COLUMNSECONDCARD)
                c = c.Add(Restrictions.Or(Restrictions.Eq("FirstCard", filterSetting.Value), Restrictions.Eq("SecondCard", filterSetting.Value)));

            return true;
        }

        public Card GetRelatedCard(Guid toCard)
        {
            Exception ex;
            return GetRelatedCard(toCard, out ex);
        }

        public Card GetRelatedCard(Guid toCard, out Exception error)
        {
            error = null;
            if (toCard == Guid.Empty)
                return null;

            var pair = GetCardPairForCard(toCard, out error);
            if (pair == null)
                return null;

            var cardId = Guid.Empty;
            if (pair.FirstCard.Equals(toCard))
                cardId = pair.SecondCard;

            if (pair.SecondCard.Equals(toCard))
                cardId = pair.FirstCard;

            return Cards.Singleton.GetObjectById(cardId);
        }

        public CardPair[] GetAllCardPairs()
        {
            var list = List();
            if (list == null)
                return null;

            return list.ToArray();
        }

        public HashSet<Guid> GetAllPairedCards()
        {
            var cardPairs = GetAllCardPairs();
            if (cardPairs == null || cardPairs.Length == 0)
                return null;

            var pairedCards = new HashSet<Guid>();
            foreach (var pair in cardPairs)
            {
                pairedCards.Add(pair.FirstCard);
                pairedCards.Add(pair.SecondCard);
            }

            return pairedCards;
        }

        public bool SetRelatedCard(Guid toCard, Guid? relatedCard)
        {
            try
            {
                CardPair pair;
                if (relatedCard == null)
                {
                    pair = GetCardPairForCard(toCard);
                    if (pair == null)
                        return true;

                    if (Delete(pair))
                        return TransformCardState(pair.FirstCard.Equals(toCard) ? pair.SecondCard : pair.FirstCard, false);
                }
                else
                {
                    pair = GetCardPairForCard(toCard);
                    if (pair == null)
                    {
                        pair = new CardPair
                        {
                            FirstCard = toCard,
                            SecondCard = relatedCard.Value
                        };

                        if (Insert(ref pair))
                            return TransformCardState(pair.FirstCard.Equals(toCard) ? pair.SecondCard : pair.FirstCard, true);
                    }
                    else
                    {
                        if (Delete(pair))
                        {
                            if (!TransformCardState(pair.FirstCard.Equals(toCard) ? pair.SecondCard : pair.FirstCard, false))
                                return false;
                        }
                        else
                            return false;

                        pair.FirstCard = toCard;
                        pair.SecondCard = relatedCard.Value;

                        if (Insert(ref pair))
                            return TransformCardState(pair.FirstCard.Equals(toCard) ? pair.SecondCard : pair.FirstCard, true);
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }

            return false;
        }

        /// <summary>
        /// Sets the card state to state according to its relation
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="toRelated">Means if a card state should be hybrid state</param>
        /// <returns></returns>
        public bool TransformCardState(Guid idCard, bool toRelated)
        {
            var editCard = Cards.Singleton.GetObjectForEdit(idCard);
            if (editCard == null)
                return false;

            if (toRelated)
                editCard.State = (byte)(editCard.State | 0x10);
            else
                editCard.State = (byte)(editCard.State & 0x0f);

            return Cards.Singleton.Update(editCard);
        }

        public bool IsCardRelated(Guid cardId)
        {
            return GetRelatedCard(cardId) != null;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CardPair; }
        }
    }
}
