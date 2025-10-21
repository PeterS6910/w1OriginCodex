using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public class AntiPassBackZoneSettings
    {
        private readonly TimeoutDictionary<Guid, CCUCardInAntiPassBackZone> _cardsInZone =
            new TimeoutDictionary<Guid, CCUCardInAntiPassBackZone>(1000);

        private IDictionary<Guid, ApbzCardReaderEntryExitBy> _guidEntryCardReaders;
        private IDictionary<Guid, ApbzCardReaderEntryExitBy> _guidExitCardReaders;

        private bool _prohibitAccessForCardNotPresent;

        private Guid _guidDestinationAPBZAfterTimeout;

        public AntiPassBackZoneSettings(AntiPassBackZone antiPassBackZone)
        {
            IdAntiPassBackZone = antiPassBackZone.IdAntiPassBackZone;

            _cardsInZone.ItemTimedOut += OnCardTimedOut;

            _guidEntryCardReaders = 
                new Dictionary<Guid, ApbzCardReaderEntryExitBy>(antiPassBackZone.GuidEntryCardReaders);

            _guidExitCardReaders =
                new Dictionary<Guid, ApbzCardReaderEntryExitBy>(antiPassBackZone.GuidExitCardReaders);

            _prohibitAccessForCardNotPresent = antiPassBackZone.ProhibitAccessForCardNotPresent;

            _guidDestinationAPBZAfterTimeout = antiPassBackZone.GuidDestinationAPBZAfterTimeout;

            _cardsInZone.DefaultTimeout = 60000 * antiPassBackZone.Timeout;
        }

        public IEnumerable<KeyValuePair<Guid, ApbzCardReaderEntryExitBy>> GuidEntryCardReaders
        {
            get { return _guidEntryCardReaders; }
        }

        public IEnumerable<KeyValuePair<Guid, ApbzCardReaderEntryExitBy>> GuidExitCardReaders
        {
            get { return _guidExitCardReaders; }
        }

        public void AttachEvents()
        {
            foreach (var kvPair in GuidEntryCardReaders)
                switch (kvPair.Value)
                {
                    case ApbzCardReaderEntryExitBy.NormalAccess:

                        CardReaders.Singleton.AddCardReaderAccessed(
                            kvPair.Key,
                            EntryCardReadersAccessedNormal);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessPermitted:

                        CardReaders.Singleton.AddCardReaderAccessed(
                            kvPair.Key,
                            EntryCardReadersAccessedPermitted);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessInterupted:

                        CardReaders.Singleton.AddCardReaderAccessed(
                            kvPair.Key,
                            EntryCardReadersAccessedInterrupted);

                        break;
                }

            foreach (var kvPair in GuidExitCardReaders)
                switch (kvPair.Value)
                {
                    case ApbzCardReaderEntryExitBy.NormalAccess:

                        CardReaders.Singleton.AddCardReaderAccessed(
                            kvPair.Key,
                            ExitCardReadersAccessedNormal);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessPermitted:

                        CardReaders.Singleton.AddCardReaderAccessed(
                            kvPair.Key,
                            ExitCardReadersAccessedPermitted);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessInterupted:

                        CardReaders.Singleton.AddCardReaderAccessed(
                            kvPair.Key,
                            ExitCardReadersAccessedInterrupted);

                        break;
                }
        }

        public void DetachEvents()
        {
            foreach (var kvPair in GuidEntryCardReaders)
                switch (kvPair.Value)
                {
                    case ApbzCardReaderEntryExitBy.NormalAccess:

                        CardReaders.Singleton.RemoveCardReaderAccessed(
                            kvPair.Key,
                            EntryCardReadersAccessedNormal);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessPermitted:

                        CardReaders.Singleton.RemoveCardReaderAccessed(
                            kvPair.Key,
                            EntryCardReadersAccessedPermitted);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessInterupted:

                        CardReaders.Singleton.RemoveCardReaderAccessed(
                            kvPair.Key,
                            EntryCardReadersAccessedInterrupted);

                        break;
                }

            foreach (var kvPair in GuidExitCardReaders)
                switch (kvPair.Value)
                {
                    case ApbzCardReaderEntryExitBy.NormalAccess:

                        CardReaders.Singleton.RemoveCardReaderAccessed(
                            kvPair.Key,
                            ExitCardReadersAccessedNormal);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessPermitted:

                        CardReaders.Singleton.RemoveCardReaderAccessed(
                            kvPair.Key,
                            ExitCardReadersAccessedPermitted);

                        break;

                    case ApbzCardReaderEntryExitBy.AccessInterupted:

                        CardReaders.Singleton.RemoveCardReaderAccessed(
                            kvPair.Key,
                            ExitCardReadersAccessedInterrupted);

                        break;
                }
        }

        public void ApplyChanges(AntiPassBackZone antiPassBackZone)
        {
            _guidEntryCardReaders =
                new Dictionary<Guid, ApbzCardReaderEntryExitBy>(antiPassBackZone.GuidEntryCardReaders);

            _guidExitCardReaders =
                new Dictionary<Guid, ApbzCardReaderEntryExitBy>(antiPassBackZone.GuidExitCardReaders);

            _prohibitAccessForCardNotPresent = 
                antiPassBackZone.ProhibitAccessForCardNotPresent;

            _guidDestinationAPBZAfterTimeout = 
                antiPassBackZone.GuidDestinationAPBZAfterTimeout;

            int newTimeout = 60000 * antiPassBackZone.Timeout;

            if (_cardsInZone.DefaultTimeout != newTimeout)
                ReplanCards(newTimeout);
        }

        private void ReplanCards(int newTimeout)
        {
            int previousTimeout = 
                _cardsInZone.DefaultTimeout > 0
                    ? _cardsInZone.DefaultTimeout
                    : 0;

            _cardsInZone.DefaultTimeout = newTimeout;

            if (newTimeout == 0)
            {
                ReplanCardsToInfinity();
                return;
            }

            int timeoutIncrement = newTimeout - previousTimeout;

            foreach (var guidCard in _cardsInZone.Keys)
            {
                CCUCardInAntiPassBackZone ccuCardInAntiPassBackZone;

                if (!_cardsInZone.TryGetValue(guidCard, out ccuCardInAntiPassBackZone))
                    continue;

                ccuCardInAntiPassBackZone.EndTickCount = 
                    timeoutIncrement + 
                    (ccuCardInAntiPassBackZone.EndTickCount != -1
                        ? ccuCardInAntiPassBackZone.EndTickCount
                        : Environment.TickCount);

                int currentTickCount = Environment.TickCount;
                int remaining = ccuCardInAntiPassBackZone.EndTickCount - currentTickCount;

                if (remaining <= 0)
                {
                    _cardsInZone.Remove(
                        guidCard,
                        (key, removed, removedValue) => 
                            OnCardTimedOutInternal(
                                key,
                                ccuCardInAntiPassBackZone.GuidEntryCardReader,
                                ccuCardInAntiPassBackZone.EntryBy,
                                ccuCardInAntiPassBackZone.EntryDateTime));

                    continue;
                }

                _cardsInZone.SetTimeout(
                    guidCard,
                    remaining);
            }
        }

        private void ReplanCardsToInfinity()
        {
            foreach (var guidCard in _cardsInZone.Keys)
            {
                CCUCardInAntiPassBackZone ccuCardInAntiPassBackZone;

                if (!_cardsInZone.TryGetValue(guidCard, out ccuCardInAntiPassBackZone))
                    continue;

                ccuCardInAntiPassBackZone.EndTickCount = -1;

                _cardsInZone.SetTimeout(
                    guidCard, 
                    0);
            }
        }

        private void OnCardEntryAccess(
            Guid guidCardAccessed,
            Guid guidCardReaderAccessed,
            ApbzCardReaderEntryExitBy entryBy)
        {
            _cardsInZone.GetOrAddValue(
                guidCardAccessed,
                (Guid key,
                    out CCUCardInAntiPassBackZone newValue,
                    out int timeout) =>
                {
                    int defaultTimeout = _cardsInZone.DefaultTimeout;

                    newValue = 
                        new CCUCardInAntiPassBackZone(
                            guidCardAccessed,
                            guidCardReaderAccessed,
                            defaultTimeout > 0
                                ? Environment.TickCount + defaultTimeout
                                : -1,
                            CcuCore.LocalTime,
                            entryBy);

                    timeout = defaultTimeout;
                },
                (key, value, timeout, newlyAdded) =>
                {
                    Database.ConfigObjectsEngine.ApbzStorage.AddCard(
                        value,
                        IdAntiPassBackZone);

                    Events.ProcessEvent(
                        new EventParameters.EventApbzCardEntered(
                            IdAntiPassBackZone,
                            guidCardAccessed,
                            guidCardReaderAccessed,
                            entryBy));
                });
        }

        private void EntryCardReadersAccessedNormal(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            if (doorEnvironmentStateChangedArgs.State ==
                DoorEnvironmentState.Opened
                && doorEnvironmentStateChangedArgs.StateDetail ==
                DoorEnvironmentStateDetail.NormalAccess)
            {
                OnCardEntryAccess(
                    doorEnvironmentStateChangedArgs.GuidCardAccessed,
                    doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                    ApbzCardReaderEntryExitBy.NormalAccess);
            }
        }

        private void EntryCardReadersAccessedPermitted(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            if (doorEnvironmentStateChangedArgs.State ==
                DoorEnvironmentState.Unlocked)
            {
                OnCardEntryAccess(
                    doorEnvironmentStateChangedArgs.GuidCardAccessed,
                    doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                    ApbzCardReaderEntryExitBy.AccessPermitted);
            }
        }

        private void EntryCardReadersAccessedInterrupted(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            if (doorEnvironmentStateChangedArgs.State ==
                DoorEnvironmentState.Locked
                && doorEnvironmentStateChangedArgs.StateDetail ==
                DoorEnvironmentStateDetail.InterruptedAccess)
            {
                OnCardEntryAccess(
                    doorEnvironmentStateChangedArgs.GuidCardAccessed,
                    doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                    ApbzCardReaderEntryExitBy.AccessInterupted);
            }
        }

        private void ExitCardReadersAccessedNormal(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            if (doorEnvironmentStateChangedArgs.State ==
                DoorEnvironmentState.Opened
                && doorEnvironmentStateChangedArgs.StateDetail ==
                DoorEnvironmentStateDetail.NormalAccess)
            {
                OnCardExitAccess(
                    doorEnvironmentStateChangedArgs.GuidCardAccessed,
                    doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                    ApbzCardReaderEntryExitBy.NormalAccess);
            }
        }

        private void ExitCardReadersAccessedPermitted(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            if (doorEnvironmentStateChangedArgs.State ==
                DoorEnvironmentState.Unlocked)
            {
                OnCardExitAccess(
                    doorEnvironmentStateChangedArgs.GuidCardAccessed,
                    doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                    ApbzCardReaderEntryExitBy.AccessPermitted);
            }
        }

        private void ExitCardReadersAccessedInterrupted(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            if (doorEnvironmentStateChangedArgs.State ==
                DoorEnvironmentState.Locked
                && doorEnvironmentStateChangedArgs.StateDetail ==
                DoorEnvironmentStateDetail.InterruptedAccess)
            {
                OnCardExitAccess(
                    doorEnvironmentStateChangedArgs.GuidCardAccessed,
                    doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                    ApbzCardReaderEntryExitBy.AccessInterupted);
            }
        }

        private void OnCardExitAccess(
            Guid guidCardAccessed,
            Guid guidCardReaderAccessed,
            ApbzCardReaderEntryExitBy exitBy)
        {
            _cardsInZone.Remove(
                guidCardAccessed,
                (key, removed, removedValue) =>
                {
                    Database.ConfigObjectsEngine.ApbzStorage.RemoveCard(
                        guidCardAccessed,
                        IdAntiPassBackZone);

                    if (removed)
                        Events.ProcessEvent(
                            new EventParameters.EventApbzCardExited(
                                IdAntiPassBackZone,
                                guidCardAccessed,
                                removedValue.GuidEntryCardReader,
                                removedValue.EntryBy,
                                removedValue.EntryDateTime,
                                guidCardReaderAccessed,
                                exitBy));
                });
        }

        private void OnCardTimedOut(
            Guid guidCard,
            CCUCardInAntiPassBackZone value,
            int timeout)
        {
            OnCardTimedOutInternal(
                guidCard, 
                value.GuidEntryCardReader, 
                value.EntryBy, 
                value.EntryDateTime);
        }

        private void OnCardTimedOutInternal(
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy EntryBy,
            DateTime entryDateTime)
        {
            Database.ConfigObjectsEngine.ApbzStorage
                .RemoveCard(
                    guidCard,
                    IdAntiPassBackZone);

            Events.ProcessEvent(
                new EventParameters.EventApbzCardTimedOut(
                    IdAntiPassBackZone,
                    guidCard,
                    guidEntryCardReader,
                    EntryBy,
                    entryDateTime));

            if (_guidDestinationAPBZAfterTimeout != Guid.Empty)
                AntiPassBackZones.Singleton.AfterTimeoutEntry(
                    _guidDestinationAPBZAfterTimeout,
                    guidCard);
        }

        public class EqualityComparer : IEqualityComparer<AntiPassBackZoneSettings>
        {
            public bool Equals(
                AntiPassBackZoneSettings antiPassBackZoneSettings1,
                AntiPassBackZoneSettings antiPassBackZoneSettings2)
            {
                return 
                    antiPassBackZoneSettings1.IdAntiPassBackZone
                        .Equals(antiPassBackZoneSettings2.IdAntiPassBackZone);
            }

            public int GetHashCode(AntiPassBackZoneSettings antiPassBackZoneSettings)
            {
                return antiPassBackZoneSettings.IdAntiPassBackZone.GetHashCode();
            }
        }

        public Guid IdAntiPassBackZone { get; private set; }

        public bool HasAccessEntry(Guid guidCard)
        {
            return !_cardsInZone.ContainsKey(guidCard);
        }

        public bool HasAccessExit(Guid guidCard)
        {
            return 
                !_prohibitAccessForCardNotPresent ||
                _cardsInZone.ContainsKey(guidCard);
        }

        public void AfterTimeoutEntry(Guid guidCard)
        {
            OnCardEntryAccess(
                guidCard,
                Guid.Empty,
                ApbzCardReaderEntryExitBy.NormalAccess);
        }

        /// <summary>
        /// Restores the card's membership in the zone after the occurrence of shutdown/startup cycle.
        /// This method is supposed to be launched by the CCU's startup routines. The card' expiration
        /// is replanned in such a manner as if the CCU shutdown/startup had not taken place, i.e. taking into 
        /// account only the card's original entry timepoint and zone's current value of default timeout.
        /// </summary>
        /// <param name="guidCard"></param>
        /// <param name="guidEntryCardReader"></param>
        /// <param name="entryDateTime">the real DateTime of the card's entry into the zone. This value is not altered by the intermittent CCU shutdowns / startups.</param>
        /// <param name="entryBy"></param>
        public void LoadCard(
            Guid guidCard,
            Guid guidEntryCardReader,
            DateTime entryDateTime,
            ApbzCardReaderEntryExitBy entryBy)
        {
            int defaultTimeout = _cardsInZone.DefaultTimeout;

            if (defaultTimeout == 0)
            {
                _cardsInZone.Add(
                    guidCard,
                    new CCUCardInAntiPassBackZone(
                        guidCard,
                        guidEntryCardReader,
                        -1,
                        entryDateTime,
                        entryBy));

                return;
            }

            int remainingTicks = 
                defaultTimeout - (int)(DateTime.Now - entryDateTime).TotalMilliseconds;

            if (remainingTicks <= 0)
            {
                OnCardTimedOutInternal(
                    guidCard,
                    guidEntryCardReader,
                    entryBy,
                    entryDateTime);
                
                return;
            }

            _cardsInZone.Add(
                guidCard,
                new CCUCardInAntiPassBackZone(
                    guidCard,
                    guidEntryCardReader, 
                    Environment.TickCount + remainingTicks,
                    entryDateTime,
                    entryBy),
                remainingTicks);
        }

        public ICollection<CCUCardInAntiPassBackZone> CardsInZone
        {
            get
            {
                return _cardsInZone.Values;
            }
        }

        public void RemoveCards(Guid[] guidCards)
        {
            foreach (var guidCard in guidCards)
                _cardsInZone.Remove(
                    guidCard,
                    (key, removed, value) =>
                    {
                        if (removed)
                            OnCardTimedOutInternal(
                                value.GuidCard,
                                value.GuidEntryCardReader,
                                value.EntryBy,
                                value.EntryDateTime);
                    });
        }

        public ICollection<CCUCardInAntiPassBackZone> AddCards(Guid[] idCards)
        {
            var ccuCardsInAntiPassBackZone = new LinkedList<CCUCardInAntiPassBackZone>();

            foreach (var idCard in idCards)
            {
                AfterTimeoutEntry(idCard);

                CCUCardInAntiPassBackZone ccuCardInAntiPassBackZone;

                if (_cardsInZone.TryGetValue(
                    idCard,
                    out ccuCardInAntiPassBackZone))
                {
                    ccuCardsInAntiPassBackZone.AddLast(ccuCardInAntiPassBackZone);
                }
            }

            return ccuCardsInAntiPassBackZone;
        }
    }
}