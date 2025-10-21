using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AntiPassBackZones :
        ANcasBaseOrmTable<AntiPassBackZones, AntiPassBackZone>,
        IAntiPassBackZones
    {
        private AntiPassBackZones()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<AntiPassBackZone>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.ACCESS_CONTROL_LISTS),
                login);
        }

        protected override IModifyObject CreateModifyObject(AntiPassBackZone ormbObject)
        {
            return new AntiPassBackZoneModifyObj(ormbObject);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AclsInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.ACCESS_CONTROL_LISTS),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AclsInsertDeletePerform),
                login);
        }

        public override bool CheckData(
            AntiPassBackZone antiPassBackZone,
            out Exception error)
        {
            error = null;

            var entryCardReaders = antiPassBackZone.EntryCardReaders;

            Guid idAntiPassBackZone = antiPassBackZone.IdAntiPassBackZone;

            var allAntiPassBackZones = List();

            if (entryCardReaders != null)
            {
                var existingEntryCardReadersAccessPermitted =
                    GetExistingEntryCardReaderGuids(
                        allAntiPassBackZones,
                        idAntiPassBackZone,
                        ApbzCardReaderEntryExitBy.AccessPermitted);

                var existingEntryCardReadersNormalAccess =
                    GetExistingEntryCardReaderGuids(
                        allAntiPassBackZones,
                        idAntiPassBackZone,
                        ApbzCardReaderEntryExitBy.NormalAccess);

                var existingEntryCardReadersAccessInterrupted =
                    GetExistingEntryCardReaderGuids(
                        allAntiPassBackZones,
                        idAntiPassBackZone,
                        ApbzCardReaderEntryExitBy.AccessInterupted);

                if (entryCardReaders
                    .Any(kvPair =>
                        (kvPair.Value.EntryExitBy == ApbzCardReaderEntryExitBy.AccessPermitted
                         && (existingEntryCardReadersAccessPermitted.Contains(kvPair.Key.IdCardReader)
                             || existingEntryCardReadersNormalAccess.Contains(kvPair.Key.IdCardReader)
                             || existingEntryCardReadersAccessInterrupted.Contains(kvPair.Key.IdCardReader)))
                        || (kvPair.Value.EntryExitBy == ApbzCardReaderEntryExitBy.NormalAccess
                            && (existingEntryCardReadersAccessPermitted.Contains(kvPair.Key.IdCardReader)
                                || existingEntryCardReadersNormalAccess.Contains(kvPair.Key.IdCardReader)))
                        || (kvPair.Value.EntryExitBy == ApbzCardReaderEntryExitBy.AccessInterupted
                            && (existingEntryCardReadersAccessPermitted.Contains(kvPair.Key.IdCardReader)
                                || existingEntryCardReadersAccessInterrupted.Contains(kvPair.Key.IdCardReader)))))
                {
                    error = new ConflictingEntryCardReadersException();
                    return false;
                }
            }

            if (antiPassBackZone.Timeout == 0 &&
                (antiPassBackZone.ExitCardReaders == null ||
                 antiPassBackZone.ExitCardReaders.Count == 0))
            {
                error = new TimeoutNotSetAndNoExitCardReaderException();
                return false;
            }

            AntiPassBackZone destinationApbzAfterTimeout =
                antiPassBackZone.DestinationAPBZAfterTimeout;

            if (idAntiPassBackZone != Guid.Empty &&
                destinationApbzAfterTimeout != null)
            {
                var referencingZonesSearch =
                    new ReferencingAnitPassBackZonesSearch(idAntiPassBackZone);

                var guidsZonesReferencingRoot =
                    referencingZonesSearch.Execute(allAntiPassBackZones);

                if (guidsZonesReferencingRoot.Contains(
                    destinationApbzAfterTimeout.IdAntiPassBackZone))
                {
                    error = new CycleInExirationTargetsException();
                    return false;
                }
            }

            return true;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            var antiPassBackZone = GetById(idObj);

            if (antiPassBackZone == null)
                return null;

            var result = new List<AOrmObject>();

            var sourceAntiPassBackZone =
                SelectLinq<AntiPassBackZone>(
                    sourceAntiPassBackZone_ =>
                        sourceAntiPassBackZone_.DestinationAPBZAfterTimeout == antiPassBackZone)
                    .FirstOrDefault();

            if (sourceAntiPassBackZone != null)
                result.Add(sourceAntiPassBackZone);

            return result;
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            AntiPassBackZone antiPassBackZone)
        {
            yield return antiPassBackZone.DestinationAPBZAfterTimeout;

            var entryCardReaders = antiPassBackZone.EntryCardReaders;

            if (entryCardReaders != null)
                foreach (var cardReader in entryCardReaders.Keys)
                    yield return cardReader;

            var exitCardReaders = antiPassBackZone.ExitCardReaders;

            if (exitCardReaders != null)
                foreach (var cardReader in exitCardReaders.Keys)
                    yield return cardReader;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<AntiPassBackZone> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                linqResult =
                    single
                        ? SelectLinq<AntiPassBackZone>(
                            antiPassBackZone => antiPassBackZone.Name.IndexOf(name) >= 0)
                        : SelectLinq<AntiPassBackZone>(
                            antiPassBackZone =>
                                antiPassBackZone.Name.IndexOf(name) >= 0 ||
                                antiPassBackZone.Description.IndexOf(name) >= 0);
            }

            if (linqResult == null)
                return resultList;

            linqResult = linqResult.OrderBy(antiPassBackZone => antiPassBackZone.Name).ToList();

            foreach (var antiPassBackZone in linqResult)
                resultList.Add(antiPassBackZone);

            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<AntiPassBackZone> linqResult =
                !string.IsNullOrEmpty(name)
                    ? SelectLinq<AntiPassBackZone>(
                        antiPassBackZone =>
                            antiPassBackZone.Name.IndexOf(name) >= 0 ||
                            antiPassBackZone.Description.IndexOf(name) >= 0)
                    : null;

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<AntiPassBackZone> linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<AntiPassBackZone>(
                        antiPassBackZone => antiPassBackZone.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(
            ICollection<AntiPassBackZone> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();

            if (linqResult == null)
                return resultList;

            linqResult =
                linqResult
                    .OrderBy(antiPassBackZone => antiPassBackZone.Name)
                    .ToList();

            foreach (var aa in linqResult)
                resultList.Add(aa);

            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<AntiPassBackZoneShort> ShortSelectByCriteria(
            out Exception error,
            LogicalOperators filterJoinOperator,
            params ICollection<FilterSettings>[] filterSettings)
        {
            return
                new LinkedList<AntiPassBackZoneShort>(
                    SelectByCriteria(out error, filterJoinOperator, filterSettings)
                        .Select(antiPassBackZone => new AntiPassBackZoneShort(antiPassBackZone)));
        }

        public ICollection<IModifyObject> ModifyObjectsSelectByCriteria(
            ICollection<FilterSettings> filterSettings,
            out Exception err)
        {
            var antiPassBackZones =
                SelectByCriteria(
                    filterSettings,
                    out err);

            return
                new LinkedList<IModifyObject>(
                    antiPassBackZones
                        .Select(antiPassBackZone => new AntiPassBackZoneModifyObj(antiPassBackZone))
                        .OrderBy(antiPassBackZoneModifyObj => antiPassBackZoneModifyObj.ToString())
                        .Cast<IModifyObject>());
        }

        public IEnumerable<IModifyObject> GetAvailableExpirationTargets(
            Guid guidAntiPassBackZone,
            Guid guidCCU,
            out Exception err)
        {
            var antiPassBackZonesOrmList = List(out err);

            if (antiPassBackZonesOrmList == null || err != null)
                return null;

            IEnumerable<AntiPassBackZone> antiPassBackZones;
            if (guidAntiPassBackZone != Guid.Empty)
            {
                ICollection<Guid> zonesReferencingRoot =
                    new ReferencingAnitPassBackZonesSearch(guidAntiPassBackZone)
                        .Execute(antiPassBackZonesOrmList);

                antiPassBackZones =
                    antiPassBackZonesOrmList.Where(
                        antiPBZone =>
                            !zonesReferencingRoot.Contains(antiPBZone.IdAntiPassBackZone));
            }
            else
                antiPassBackZones = antiPassBackZonesOrmList;

            return
                new LinkedList<IModifyObject>(
                    antiPassBackZones
                        .Where(
                            antiPassBackZone =>
                            {
                                var ccu = antiPassBackZone.GetParentCCU();

                                return ccu == null || ccu.IdCCU == guidCCU;
                            })
                        .Select(
                            antiPassBackZone =>
                                (IModifyObject)
                                new AntiPassBackZoneModifyObj(antiPassBackZone)));
        }

        public ICollection<Guid> GetExistingEntryCardReaderGuids(
            Guid skippedAntiPassBackZone,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            return
                GetExistingEntryCardReaderGuids(
                    List(),
                    skippedAntiPassBackZone,
                    entryExitBy);
        }

        private static ICollection<Guid> GetExistingEntryCardReaderGuids(
            IEnumerable<AntiPassBackZone> antiPassBackZones,
            Guid skippedAntiPassBackZone,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            return
                new HashSet<Guid>(
                    antiPassBackZones.SelectMany(
                        antiPassBackZone =>
                            antiPassBackZone.EntryCardReaders != null &&
                            antiPassBackZone.IdAntiPassBackZone != skippedAntiPassBackZone
                                ? antiPassBackZone.EntryCardReaders
                                    .Where(kvPair => kvPair.Value.EntryExitBy == entryExitBy)
                                    .Select(kvPair => kvPair.Key.IdCardReader)
                                : Enumerable.Empty<Guid>()));
        }

        protected override void LoadObjectsInRelationship(AntiPassBackZone antiPassBackZone)
        {
            antiPassBackZone.EntryCardReaders =
                antiPassBackZone.EntryCardReaders
                    .ToDictionary(
                        kvPair => CardReaders.Singleton.GetById(kvPair.Key.IdCardReader),
                        kvPair =>
                            new APBZCardReader
                            {
                                Direction = true,
                                EntryExitBy = kvPair.Value.EntryExitBy
                            });

            antiPassBackZone.ExitCardReaders =
                antiPassBackZone.ExitCardReaders
                    .ToDictionary(
                        kvPair => CardReaders.Singleton.GetById(kvPair.Key.IdCardReader),
                        kvPair =>
                            new APBZCardReader
                            {
                                Direction = false,
                                EntryExitBy = kvPair.Value.EntryExitBy
                            });

            var destinationApbzAfterTimeout = antiPassBackZone.DestinationAPBZAfterTimeout;

            if (destinationApbzAfterTimeout != null)
                antiPassBackZone.DestinationAPBZAfterTimeout =
                    GetById(destinationApbzAfterTimeout.IdAntiPassBackZone);
        }

        public override void CUDSpecial(
            AntiPassBackZone antiPassBackZone,
            ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        antiPassBackZone.GetId(),
                        antiPassBackZone.GetObjectType()));

                return;
            }

            if (antiPassBackZone == null)
                return;

            var objectsToSend =
                Enumerable.Repeat<AOrmObject>(antiPassBackZone, 1)
                    .Concat(
                        antiPassBackZone.DestinationAPBZAfterTimeout != null
                            ? Enumerable.Repeat<AOrmObject>(
                                antiPassBackZone.DestinationAPBZAfterTimeout,
                                1)
                            : Enumerable.Empty<AOrmObject>())
                    .ToList();

            DataReplicationManager.Singleton.SendModifiedObjectToCcus(antiPassBackZone);
        }

        public void GetParentCCU(
            ICollection<Guid> ccus,
            Guid guid)
        {
            var apbz = GetById(guid);

            if (apbz == null)
                return;

            var ccu = apbz.GetParentCCU();
            if (ccu == null)
                return;

            var idCCU = ccu.IdCCU;

            if (!ccus.Contains(idCCU))
                ccus.Add(idCCU);
        }

        private IEnumerable<AntiPassBackZone> GetAntiPassBackZonesForEntryCardReaders(
            ICollection<Guid> guidCardReaders,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            return
                List()
                    .Where(
                        antiPassBackZone =>
                            antiPassBackZone.EntryCardReaders != null &&
                            antiPassBackZone.EntryCardReaders.Any(
                                kvPair =>
                                    guidCardReaders.Contains(kvPair.Key.IdCardReader) &&
                                    kvPair.Value.EntryExitBy == entryExitBy));
        }

        public ICollection<IModifyObject> GetAPBZModifyObjectsForEntryCardReaders(
            ICollection<Guid> guidCardReaders,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            return
                new LinkedList<IModifyObject>(
                    GetAntiPassBackZonesForEntryCardReaders(
                        guidCardReaders,
                        entryExitBy)
                        .Select(
                            antiPassBackZone =>
                                (IModifyObject)
                                    new AntiPassBackZoneModifyObj(antiPassBackZone)));
        }

        public void RemoveEntryCardReadersFromAntiPassBackZones(
            Guid guidSkippedAntiPassBackZone,
            ICollection<Guid> guidCardReaders,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            var guidAntiPassBackZones =
                new HashSet<Guid>(
                    GetAntiPassBackZonesForEntryCardReaders(
                        guidCardReaders,
                        entryExitBy)
                    .Select(antiPassBackZone => antiPassBackZone.IdAntiPassBackZone));

            foreach (var guidAntiPassBackZone in guidAntiPassBackZones)
            {
                var editAntiPassBackZone =
                    GetObjectForEdit(guidAntiPassBackZone);

                var entryCardReaders = editAntiPassBackZone.EntryCardReaders;

                if (entryCardReaders == null)
                    continue;

                var readersToRemove =
                    new LinkedList<CardReader>(
                        entryCardReaders.Keys
                            .Where(
                                cardReader =>
                                    guidCardReaders.Contains(cardReader.IdCardReader)));

                if (readersToRemove.Count == 0)
                    continue;

                foreach (var cardReader in readersToRemove)
                    editAntiPassBackZone.EntryCardReaders.Remove(cardReader);

                Update(editAntiPassBackZone);

                EditEnd(editAntiPassBackZone);
            }
        }

        private readonly SyncDictionary<Guid, AntiPassBackZoneContent> _antiPassBackZoneContents =
            new SyncDictionary<Guid, AntiPassBackZoneContent>();

        public ICollection<CardInAntiPassBackZone> GetCardsInZone(Guid idAntiPassbackZone)
        {
            AntiPassBackZoneContent antiPassBackZoneContent;

            _antiPassBackZoneContents.GetOrAddValue(
                idAntiPassbackZone,
                out antiPassBackZoneContent,
                key =>
                {
                    var antiPassBackZone = GetById(key);

                    if (antiPassBackZone == null)
                        return null;

                    var ccu = antiPassBackZone.GetParentCCU();

                    return
                        new AntiPassBackZoneContent(
                            ccu != null
                                ? ccu.IdCCU
                                : Guid.Empty,
                            key);
                },
                null);

            return
                antiPassBackZoneContent != null
                    ? antiPassBackZoneContent.CardsInZone
                    : null;
        }

        public void RemoveCardsFromZone(
            Guid idAntiPassbackZone,
            ICollection<Guid> guidCardsToRemove)
        {
            _antiPassBackZoneContents.TryGetValue(
                idAntiPassbackZone,
                (key, found, value) =>
                {
                    if (found)
                        value.RemoveCards(guidCardsToRemove);
                });
        }

        public void AddCardsToZone(
            Guid idAntiPassbackZone,
            ICollection<Guid> idCardsToAdd)
        {
            _antiPassBackZoneContents.TryGetValue(
                idAntiPassbackZone,
                (key, found, value) =>
                {
                    if (found)
                        value.AddCards(idCardsToAdd);
                });
        }

        public IEnumerable<AntiPassBackZone> GetAntiPassBackZonesForCardReaders(
            ICollection<Guid> guidCardReaders)
        {
            foreach (var antiPassBackZone in List())
            {
                var entryCardReaders = antiPassBackZone.EntryCardReaders;

                if (entryCardReaders != null &&
                    entryCardReaders.Keys.Any(
                        cardReader =>
                            guidCardReaders.Contains(cardReader.IdCardReader)))
                {
                    yield return antiPassBackZone;
                    continue;
                }

                var exitCardReaders = antiPassBackZone.ExitCardReaders;

                if (exitCardReaders != null &&
                    exitCardReaders.Keys.Any(
                        cardReader =>
                            guidCardReaders.Contains(cardReader.IdCardReader)))
                {
                    yield return antiPassBackZone;
                }
            }
        }

        public IEnumerable<AntiPassBackZone> GetAntiPassBackZonesForCardReader(
            CardReader cardReader)
        {
            return
                SelectLinq<AntiPassBackZone>(
                    antiPassBackZone =>
                        antiPassBackZone.EntryCardReaders.ContainsKey(cardReader) ||
                        antiPassBackZone.ExitCardReaders.ContainsKey(cardReader));
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidObject)
        {
            var result = new List<AOrmObject>();

            var antiPassBackZone = GetById(guidObject);

            if (antiPassBackZone != null &&
                antiPassBackZone.DestinationAPBZAfterTimeout != null)
            {
                result.Add(antiPassBackZone.DestinationAPBZAfterTimeout);
            }

            return result;
        }

        public void OnCardEntered(
            Guid guidAntiPassBackZone,
            DateTime entryDateTime,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy)
        {
            _antiPassBackZoneContents.TryGetValue(
                guidAntiPassBackZone,
                (key, found, value) =>
                {
                    if (found)
                        value.OnCardEntered(
                            guidCard,
                            guidEntryCardReader,
                            entryDateTime,
                            entryBy);
                });
        }

        public void OnCardExitedOrTimedOut(
            Guid guidAntiPassBackZone,
            Guid guidCard)
        {
            _antiPassBackZoneContents.TryGetValue(
                guidAntiPassBackZone,
                (key, found, value) =>
                {
                    if (found)
                        value.RemoveCard(guidCard);
                });
        }

        public void ClearAntiPassBackZonesContent(Guid guidCCU)
        {
            _antiPassBackZoneContents.RemoveWhere(
                (key, value) => value.GuidCcu == guidCCU);
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.AntiPassBackZone; }
        }

        public ICollection<Card> GetCardsWhichCanBeAdded(Guid idApbz)
        {
            var apbz = GetById(idApbz);

            if (apbz == null)
                return null;

            var ccu = apbz.GetParentCCU();

            if (ccu == null)
                return null;

            return GetCardsNotInApbz(
                idApbz,
                GetCardsOnCcu(ccu.IdCCU));
        }

        private static IEnumerable<Guid> GetCardsOnCcu(Guid idCcu)
        {
            var persons = new HashSet<Person>(new AOrmObjectComparer<Person>());

            GetPersonsFromAccessControlLists(
                idCcu,
                persons);

            GetPersonsFromAccessZones(
                idCcu,
                persons);

            var cards = new LinkedList<Guid>();

            foreach (var person in persons)
            {
                if (person.Cards == null)
                    continue;

                foreach (var cardForPerson in person.Cards)
                    cards.AddLast(cardForPerson.IdCard);
            }

            return cards;
        }

        private static void GetPersonsFromAccessControlLists(
            Guid idCcu,
            ICollection<Person> result)
        {
            var accessCntrolLists = AccessControlLists.Singleton.List();

            if (accessCntrolLists != null)
                foreach (var accessControlList in accessCntrolLists)
                {
                    var idCcus = new HashSet<Guid>();
                    AccessControlLists.Singleton.GetParentCCU(
                        idCcus,
                        accessControlList);

                    if (!idCcus.Contains(idCcu))
                        continue;

                    var personsForAcl = ACLPersons.Singleton.GetPersonsForACL(accessControlList);

                    if (personsForAcl == null)
                        continue;

                    foreach (var personForAcl in personsForAcl)
                        result.Add(personForAcl);
                }
        }

        private static void GetPersonsFromAccessZones(
            Guid idCcu,
            ICollection<Person> persons)
        {
            var accessZones = AccessZones.Singleton.List();
            if (accessZones != null)
                foreach (var accessZone in accessZones)
                {
                    var idCcus = new HashSet<Guid>();
                    AccessZones.Singleton.GetParentCCU(
                        idCcus,
                        accessZone);

                    if (!idCcus.Contains(idCcu))
                        continue;

                    persons.Add(accessZone.Person);
                }
        }

        private ICollection<Card> GetCardsNotInApbz(
            Guid idApbz,
            IEnumerable<Guid> idCards)
        {
            if (idCards == null)
                return null;

            var cardsInZone =
                new HashSet<Guid>(
                    GetCardsInZone(idApbz)
                        .Select(
                            cardInApbz =>
                                cardInApbz.IdCard));

            var cardsNotInApbz = new LinkedList<Card>(
                idCards
                    .Where(
                        idCard =>
                            !cardsInZone.Contains(idCard))
                    .Select(
                        idCard =>
                            Cards.Singleton.GetCardForClient(idCard))
                    .Where(
                        card =>
                            card != null));

            return cardsNotInApbz.Count > 0
                ? cardsNotInApbz
                : null;
        }

        private ICollection<Card> GetCardsWhichCanBeAdded(
            Guid idApbz,
            IEnumerable<Guid> idCards)
        {
            var apbz = GetById(idApbz);

            if (apbz == null)
                return null;

            var ccu = apbz.GetParentCCU();

            if (ccu == null)
                return null;

            var cardsInCcuDatabase = new HashSet<Guid>(
                GetCardsOnCcu(ccu.IdCCU));

            return GetCardsNotInApbz(
                idApbz,
                idCards.Where(cardsInCcuDatabase.Contains));
        }

        public bool CardCanBeAdded(
            Guid idApbz,
            Guid idCard)
        {
            return GetCardsWhichCanBeAdded(
                idApbz,
                Enumerable.Repeat(
                    idCard,
                    1)) != null;
        }

        public ICollection<Card> GetCardsWhichCanBeAdded(
            Guid idApbz,
            Guid idPerson)
        {
            var person = Persons.Singleton.GetById(idPerson);

            if (person == null
                || person.Cards == null)
            {
                return null;
            }

            return GetCardsWhichCanBeAdded(
                idApbz,
                person.Cards.Select(
                    card =>
                        card.IdCard));
        }
    }
}
