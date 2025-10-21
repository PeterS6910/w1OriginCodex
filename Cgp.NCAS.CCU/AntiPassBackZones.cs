using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class AntiPassBackZones : 
        ASingleton<AntiPassBackZones>,
        DB.IDbObjectChangeListener<DB.AntiPassBackZone>
    {
        private readonly SyncDictionary<Guid, AntiPassBackZoneSettings> _antiPassBackZoneSettings =
            new SyncDictionary<Guid, AntiPassBackZoneSettings>();

        private readonly SyncDictionary<IdCardReaderApbzCrEntryExitBy, AntiPassBackZoneSettings> _antiPassBackZoneByEntryCardReader =
            new SyncDictionary<IdCardReaderApbzCrEntryExitBy, AntiPassBackZoneSettings>();

        private readonly SyncDictionary<IdCardReaderApbzCrEntryExitBy, ICollection<AntiPassBackZoneSettings>> _antiPassBackZonesByExitCardReader =
            new SyncDictionary<IdCardReaderApbzCrEntryExitBy, ICollection<AntiPassBackZoneSettings>>();

        private AntiPassBackZones()
            : base(null)
        {
            
        }
        public void AddAntiPassBackZone(Guid guidAntiPassBackZone)
        {
            var antiPassBackZone =
                    (DB.AntiPassBackZone)
                    Database.ConfigObjectsEngine.GetFromDatabase(
                        ObjectType.AntiPassBackZone, 
                        guidAntiPassBackZone);

            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (!_antiPassBackZoneSettings.TryGetValue(
                guidAntiPassBackZone,
                out antiPassBackZoneSettings))
            {
                antiPassBackZoneSettings =
                    new AntiPassBackZoneSettings(antiPassBackZone);

                _antiPassBackZoneSettings.Add(
                    guidAntiPassBackZone,
                    antiPassBackZoneSettings);
            }
            else
                antiPassBackZoneSettings.ApplyChanges(antiPassBackZone);

            antiPassBackZoneSettings.AttachEvents();
            AddCardReaders(antiPassBackZoneSettings);
        }

        private void AddCardReaders(AntiPassBackZoneSettings antiPassBackZoneSettings)
        {
            foreach (var kvPair in antiPassBackZoneSettings.GuidEntryCardReaders)
                _antiPassBackZoneByEntryCardReader.Add(
                    new IdCardReaderApbzCrEntryExitBy(
                        kvPair.Key,
                        kvPair.Value),
                    antiPassBackZoneSettings);

            foreach (var kvPair in antiPassBackZoneSettings.GuidExitCardReaders)
                AddCardReader(
                    _antiPassBackZonesByExitCardReader,
                    new IdCardReaderApbzCrEntryExitBy(
                        kvPair.Key,
                        kvPair.Value),
                    antiPassBackZoneSettings);
        }

        private void RemoveCardReaders(AntiPassBackZoneSettings antiPassBackZoneSettings)
        {
            foreach (var kvPair in antiPassBackZoneSettings.GuidEntryCardReaders)
                _antiPassBackZoneByEntryCardReader.Remove(
                    new IdCardReaderApbzCrEntryExitBy(
                        kvPair.Key,
                        kvPair.Value));

            foreach (var kvPair in antiPassBackZoneSettings.GuidExitCardReaders)
                RemoveCardReader(
                    _antiPassBackZonesByExitCardReader,
                    new IdCardReaderApbzCrEntryExitBy(
                        kvPair.Key,
                        kvPair.Value),
                    antiPassBackZoneSettings);
        }

        private static void RemoveCardReader(
            Dictionary<IdCardReaderApbzCrEntryExitBy, ICollection<AntiPassBackZoneSettings>> antiPassBackZonesByCardReader,
            IdCardReaderApbzCrEntryExitBy key,
            AntiPassBackZoneSettings antiPassBackZoneSettings)
        {
            ICollection<AntiPassBackZoneSettings> antiPassBackZoneSettingses;

            if (!antiPassBackZonesByCardReader.TryGetValue(key, out antiPassBackZoneSettingses))
                return;

            antiPassBackZoneSettingses.Remove(antiPassBackZoneSettings);

            if (antiPassBackZoneSettingses.Count == 0)
                antiPassBackZonesByCardReader.Remove(key);
        }

        private static void AddCardReader(
            Dictionary<IdCardReaderApbzCrEntryExitBy, ICollection<AntiPassBackZoneSettings>> antiPassBackZonesByCardReader,
            IdCardReaderApbzCrEntryExitBy key,
            AntiPassBackZoneSettings antiPassBackZoneSettings)
        {
            ICollection<AntiPassBackZoneSettings> antiPassBackZoneSettingses;

            if (!antiPassBackZonesByCardReader.TryGetValue(key, out antiPassBackZoneSettingses))
            {
                antiPassBackZoneSettingses = 
                    new HashSet<AntiPassBackZoneSettings>(
                        new AntiPassBackZoneSettings.EqualityComparer());

                antiPassBackZonesByCardReader.Add(
                    key, 
                    antiPassBackZoneSettingses);
            }

            antiPassBackZoneSettingses.Add(antiPassBackZoneSettings);
        }

        public bool HasAccess(
            Guid guidCard,
            Guid guidCardReader)
        {
            return EnumHelper.ListAllValues<ApbzCardReaderEntryExitBy>().All(
                entryExitBy =>
                    HasAccess(
                        guidCard,
                        new IdCardReaderApbzCrEntryExitBy(
                            guidCardReader,
                            entryExitBy)));
        }

        private bool HasAccess(
            Guid guidCard,
            IdCardReaderApbzCrEntryExitBy idCardReaderApbzCrEntryExitBy)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettingsForEntryCardReader;

            if (_antiPassBackZoneByEntryCardReader.TryGetValue(
                idCardReaderApbzCrEntryExitBy,
                out antiPassBackZoneSettingsForEntryCardReader))
            {
                if (!antiPassBackZoneSettingsForEntryCardReader.HasAccessEntry(guidCard))
                {
                    return false;
                }
            }

            ICollection<AntiPassBackZoneSettings> antiPassBackZoneSettingsForExitCardReader;

            if (_antiPassBackZonesByExitCardReader.TryGetValue(
                idCardReaderApbzCrEntryExitBy,
                out antiPassBackZoneSettingsForExitCardReader))
            {
                if (antiPassBackZoneSettingsForExitCardReader.Any(
                    antiPassBackZoneSettings =>
                        !antiPassBackZoneSettings.HasAccessExit(guidCard)))
                {
                    return false;
                }
            }

            return true;
        }

        public void AfterTimeoutEntry(
            Guid guidDestinationApbzAfterTimeout,
            Guid guidCard)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (!_antiPassBackZoneSettings.TryGetValue(
                guidDestinationApbzAfterTimeout,
                out antiPassBackZoneSettings))
            {
                return;
            }

            antiPassBackZoneSettings.AfterTimeoutEntry(guidCard);
        }

        public void LoadCards()
        {
            var savedCards = 
                Database.ConfigObjectsEngine.ApbzStorage.LoadCards();

            if (savedCards == null)
                return;

            foreach (var cardInAntiPassBackZone in savedCards)
            {
                AntiPassBackZoneSettings antiPassBackZoneSettings;

                if (!_antiPassBackZoneSettings.TryGetValue(
                    cardInAntiPassBackZone.GuidAntiPassBackZone, 
                    out antiPassBackZoneSettings))
                {
                    continue;
                }

                antiPassBackZoneSettings.LoadCard(
                    cardInAntiPassBackZone.GuidCard,
                    cardInAntiPassBackZone.GuidEntryCardReader,
                    cardInAntiPassBackZone.EntryDateTime,
                    cardInAntiPassBackZone.EntryBy);
            }
        }

        public void RemoveCardsFromAntiPassBackZone(
            Guid guidAntiPassBackZone,
            Guid[] guidCards)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (_antiPassBackZoneSettings.TryGetValue(
                guidAntiPassBackZone,
                out antiPassBackZoneSettings))
            {
                antiPassBackZoneSettings.RemoveCards(guidCards);
            }
        }

        public ICollection<CCUCardInAntiPassBackZone> AddCardsToAntiPassBackZone(
            Guid guidAntiPassBackZone,
            Guid[] guidCards)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (_antiPassBackZoneSettings.TryGetValue(
                guidAntiPassBackZone,
                out antiPassBackZoneSettings))
            {
                return antiPassBackZoneSettings.AddCards(guidCards);
            }

            return new LinkedList<CCUCardInAntiPassBackZone>();
        }

        public ICollection<CCUCardInAntiPassBackZone> GetCardsInZone(Guid guidAntiPassBackZone)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (_antiPassBackZoneSettings.TryGetValue(
                guidAntiPassBackZone,
                out antiPassBackZoneSettings))
            {
                return antiPassBackZoneSettings.CardsInZone;
            }

            return new LinkedList<CCUCardInAntiPassBackZone>(); ;
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            DB.AntiPassBackZone newObject)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (!_antiPassBackZoneSettings.TryGetValue(
                idObject,
                out antiPassBackZoneSettings))
            {
                return;
            }

            antiPassBackZoneSettings.DetachEvents();
            RemoveCardReaders(antiPassBackZoneSettings);
        }

        public void OnObjectSaved(
            Guid idObject,
            DB.AntiPassBackZone newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            AntiPassBackZoneSettings antiPassBackZoneSettings;

            if (!_antiPassBackZoneSettings.TryGetValue(
                idObject,
                out antiPassBackZoneSettings))
            {
                return;
            }
            
            antiPassBackZoneSettings.DetachEvents();

            RemoveCardReaders(antiPassBackZoneSettings);
            _antiPassBackZoneSettings.Remove(idObject);
        }
    }
}
