using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Server.DB
{
    public class AntiPassBackZoneContent
    {
        private bool _initialized;

        public Guid GuidCcu { get; private set; }
        private readonly Guid _guidAntiPassBackZone;

        private readonly IDictionary<Guid, CardInAntiPassBackZone> _cardsInZone =
            new Dictionary<Guid, CardInAntiPassBackZone>();

        public AntiPassBackZoneContent(
            Guid guidCcu,
            Guid guidAntiPassBackZone)
        {
            _initialized = false;

            GuidCcu = guidCcu;
            _guidAntiPassBackZone = guidAntiPassBackZone;
        }

        public ICollection<CardInAntiPassBackZone> CardsInZone
        {
            get
            {
                lock (_cardsInZone)
                {
                    if (!_initialized)
                        Initialize();

                    return 
                        new LinkedList<CardInAntiPassBackZone>(
                            _cardsInZone.Values);
                }
            }
        }

        private void Initialize()
        {
            var cardsInZone =
                CCUConfigurationHandler.Singleton.GetCardsInZone(
                    GuidCcu,
                    _guidAntiPassBackZone);

            if (cardsInZone == null)
                return;

            foreach (var ccuCardInAntiPassBackZone in cardsInZone)
                AddCard(
                    ccuCardInAntiPassBackZone.GuidCard,
                    ccuCardInAntiPassBackZone.GuidEntryCardReader,
                    ccuCardInAntiPassBackZone.EntryDateTime,
                    ccuCardInAntiPassBackZone.EntryBy);

            _initialized = true;
        }

        public void RemoveCard(Guid guidCard)
        {
            lock (_cardsInZone)
            {
                if (!_initialized)
                    Initialize();

                _cardsInZone.Remove(guidCard);

                AMbrSingleton<CCUConfigurationHandler>.Singleton
                    .RemoveCardsFromAntiPassbackZone(
                        GuidCcu,
                        _guidAntiPassBackZone,
                        new []{ guidCard });
            }
        }

        public void RemoveCards(ICollection<Guid> guidCards)
        {
            lock (_cardsInZone)
            {
                if (!_initialized)
                    Initialize();

                foreach (var guidCard in guidCards)
                    _cardsInZone.Remove(guidCard);

                CCUConfigurationHandler.Singleton
                    .RemoveCardsFromAntiPassbackZone(
                        GuidCcu,
                        _guidAntiPassBackZone,
                        guidCards.ToArray());
            }
        }

        public void AddCards(ICollection<Guid> idCards)
        {
            lock (_cardsInZone)
            {
                if (!_initialized)
                    Initialize();

                var addedCards = CCUConfigurationHandler.Singleton
                    .AddCardsToAntiPassbackZone(
                        GuidCcu,
                        _guidAntiPassBackZone,
                        idCards.ToArray());

                if (addedCards == null)
                    return;

                foreach (var ccuCardInAntiPassBackZone in addedCards)
                {
                    AddCard(
                        ccuCardInAntiPassBackZone.GuidCard,
                        ccuCardInAntiPassBackZone.GuidEntryCardReader,
                        ccuCardInAntiPassBackZone.EntryDateTime,
                        ccuCardInAntiPassBackZone.EntryBy);
                }
            }
        }

        public void OnCardEntered(
            Guid guidCard,
            Guid guidEntryCardReader,
            DateTime entryDateTime,
            ApbzCardReaderEntryExitBy entryBy)
        {
            lock (_cardsInZone)
            {
                if (!_initialized)
                    return;

                AddCard(
                    guidCard, 
                    guidEntryCardReader, 
                    entryDateTime, 
                    entryBy);
            }
        }

        private void AddCard(
            Guid guidCard,
            Guid guidEntryCardReader,
            DateTime entryDateTime,
            ApbzCardReaderEntryExitBy entryBy)
        {
            Card card = AMbrSingleton<Cards>.Singleton.GetById(guidCard);

            if (card == null)
                return;

            CardReader entryCardReader =
                AMbrSingleton<CardReaders>.Singleton.GetById(guidEntryCardReader);

            var person = card.Person;

            string name;

            if (person != null)
            {
                name = person.WholeName;

                if (String.IsNullOrEmpty(name))
                    name =
                        String.Format(
                            "{0} {1}",
                            person.FirstName ?? String.Empty,
                            person.Surname ?? String.Empty);
            }
            else
                name = String.Empty;


            _cardsInZone[guidCard] = new CardInAntiPassBackZone(
                guidCard,
                name,
                card.FullCardNumber,
                entryCardReader != null
                    ? entryCardReader.Name
                    : String.Empty,
                entryDateTime,
                entryBy);
        }
    }
}