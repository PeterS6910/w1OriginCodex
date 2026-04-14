using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

using System.Threading;

using Contal.IwQuick.Data;

using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.RemotingCommon;
using System.Data;
using System.Windows.Forms;
using Contal.Cgp.Server.ExportData;
using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.Server.DB
{
    public sealed class Cards :
        ABaserOrmTableWithAlarmInstruction<Cards, Card>,
        ICards
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<Card>
        {
            public override void BeforeUpdate(Card obj)
            {
                base.BeforeCreate(obj);
                Singleton.BeforeCardUpdateOrDelete(obj);
            }

            public override void BeforeDelete(Card obj)
            {
                base.BeforeDelete(obj);
                Singleton.BeforeCardUpdateOrDelete(obj);
            }
        }

        public const string CARDS_STREAM_NAME = "Cards.ms";

        private Cards() : base(null, new CudPreparation())
        {
            _temporaryBlockTimeouts.ItemTimedOut += OnTemporaryBlockTimedOut;
        }

        private void OnTemporaryBlockTimedOut(
            Guid key,
            CardState cardState,
            int timeout)
        {
            ActivateTemporarilyBlockedCard(
                key,
                cardState);
        }

        private readonly TimeoutDictionary<Guid, CardState> _temporaryBlockTimeouts =
            new TimeoutDictionary<Guid, CardState>();

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var card = ormObject as Card;

            return card != null
                ? card.Person
                : null;
        }

        protected override IModifyObject CreateModifyObject(Card ormbObject)
        {
            return new CardModifyObj(ormbObject);
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(Card card)
        {
            yield return CardPairs.Singleton.GetRelatedCard(card.IdCard);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.CardsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARDS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.CardsInsertDeletePerform), login);
        }

        private readonly SyncDictionary<Guid, Guid> _updatePerson = new SyncDictionary<Guid, Guid>();

        private void BeforeCardUpdateOrDelete(Card card)
        {
            if (card != null)
            {
                var oldCard = GetById(card.IdCard);
                if (oldCard != null && oldCard.Person != null)
                {
                    _updatePerson[oldCard.IdCard] = oldCard.Person.IdPerson;
                }
            }
        }

        public override void CUDSpecial(Card card, ObjectDatabaseAction objectDatabaseAction)
        {
            if (card == null)
                return;

            Guid oldPersonGuid;

            if (_updatePerson.TryGetValue(card.IdCard, out oldPersonGuid))
            {
                _updatePerson.Remove(card.IdCard);

                var personGuid = Guid.Empty;
                if (card.Person != null)
                {
                    personGuid = card.Person.IdPerson;
                }

                if (oldPersonGuid != personGuid)
                {
                    var oldPerson = Persons.Singleton.GetById(oldPersonGuid);
                    if (oldPerson != null)
                    {
                        DbWatcher.Singleton.DbCardChanged(oldPerson, card);
                    }
                }
            }

            if (card.Person != null)
            {
                var person = Persons.Singleton.GetById(card.Person.IdPerson);
                DbWatcher.Singleton.DbCardChanged(person, card);
            }

            var state = (CardState)card.State;

            if (state == CardState.TemporarilyBlocked
                || state == CardState.HybridTemporarilyBlocked)
            {
                var remainingTimeout =
                    InvalidPinRetriesLimitReachedTimeout
                    - (DateTime.UtcNow - card.UtcDateStateLastChange);

                if (remainingTimeout > TimeSpan.Zero)
                {
                    _temporaryBlockTimeouts.SetValue(
                        card.IdCard,
                        state,
                        remainingTimeout);
                }
                else
                    ActivateTemporarilyBlockedCard(
                        card.IdCard,
                        (CardState)card.State);
            }
            else
            {
                _temporaryBlockTimeouts.Remove(card.IdCard);
            }
        }

        public override bool Delete(Card ormObject)
        {
            if (!CardPairs.Singleton.SetRelatedCard(ormObject.IdCard, null))
                return false;

            return base.Delete(ormObject);
        }

        public override bool Delete(Card ormObject, out Exception error)
        {
            if (!base.Delete(ormObject, out error))
                return false;

            if (!CardPairs.Singleton.SetRelatedCard(ormObject.IdCard, null))
            {
                error = new SqlDeleteReferenceConstraintException();
                return false;
            }

            ConsecutiveEvents.Singleton.CleanConsecutiveEvents(ormObject.IdCard);

            return true;
        }

        public override void AfterUDFailed(Card card)
        {
            if (card != null && card.Person != null)
            {
                _updatePerson.Remove(card.IdCard);
            }
        }

        protected override void LoadObjectsInRelationship(Card obj)
        {
            if (obj.Person != null)
                obj.Person = Persons.Singleton.GetById(obj.Person.IdPerson);

            if (obj.CardSystem != null)
                obj.CardSystem = CardSystems.Singleton.GetById(obj.CardSystem.IdCardSystem);
        }

        protected override void LoadObjectsInRelationshipGetById(Card obj)
        {
            if (obj.CardSystem != null)
                obj.CardSystem = CardSystems.Singleton.GetById(obj.CardSystem.IdCardSystem);
        }

        public override bool CheckData(Card ormObject, out Exception error)
        {
            if (ormObject != null)
            {
                var cards = SelectLinq<Card>(card => card.IdCard != ormObject.IdCard && card.FullCardNumber == ormObject.FullCardNumber);

                if (cards != null && cards.Count > 0)
                {
                    error = new SqlUniqueException(Card.COLUMNFULLCARDNUMBER);
                    return false;
                }
            }

            return base.CheckData(ormObject, out error);
        }

        public Card GetCardByFullNumber(string cardNumber)
        {
            var cards = SelectLinq<Card>(card => card.FullCardNumber == cardNumber);

            if (cards != null && cards.Count > 0)
            {
                var card = cards.ElementAt(0);
                if (card != null)
                    LoadObjectsInRelationship(card);

                return card;
            }

            return null;
        }

        public Card GetCardForClient(Guid idCard)
        {
            var card = GetById(idCard);

            if (card != null)
                LoadObjectsInRelationship(card);

            return card;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var card = GetById(idObj);
                if (card == null) return null;

                var result = new List<AOrmObject>();

                if (card.Person != null)
                {
                    var outPerson = Persons.Singleton.GetById(card.Person.IdPerson);
                    if (outPerson != null)
                    {
                        result.Add(outPerson);
                    }
                }

                var relatedCard = CardPairs.Singleton.GetRelatedCard(card.IdCard);
                if (relatedCard != null)
                    result.Add(relatedCard);

                if (result.Count > 0)
                {
                    return result.OrderBy(orm => orm.ToString()).ToList();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<Card> linqResult;

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
                        ? SelectLinq<Card>(
                            c => c.Number.IndexOf(name) >= 0)
                        : SelectLinq<Card>(
                            c =>
                                c.Number.IndexOf(name) >= 0 ||
                                c.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(card => card.Number).ToList();
                foreach (var card in linqResult)
                {
                    resultList.Add(GetById(card.IdCard));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Card> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<Card>(
                        c =>
                            c.Number.IndexOf(name) >= 0 ||
                            c.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<Card>(
                        c => c.Number.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(ICollection<Card> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                foreach (var card in linqResult)
                {
                    resultList.Add(GetById(card.IdCard));
                }
                linqResult.OrderBy(card => card.ToString()).ToList();
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<CardShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listCard = SelectByCriteria(filterSettings, out error);
            ICollection<CardShort> result = new List<CardShort>();
            var relatedCards = CardPairs.Singleton.GetAllPairedCards();

            if (listCard != null)
            {
                foreach (var card in listCard)
                {
                    if (relatedCards != null && relatedCards.Contains(card.IdCard))
                    {
                        if (card.State != (byte)(card.State | 0x10))
                        {
                            CardPairs.Singleton.TransformCardState(card.IdCard, true);
                            card.State = (byte)(card.State | 0x10);
                        }
                    }
                    else
                    {
                        if (card.State != (byte)(card.State & 0x0f))
                        {
                            CardPairs.Singleton.TransformCardState(card.IdCard, false);
                            card.State = (byte)(card.State & 0x0f);
                        }
                    }

                    result.Add(new CardShort(card));
                }
            }
            return result;
        }

        public IList<Card> GetCardsByListGuids(IList<object> idCards)
        {
            IList<Card> resultCard = new List<Card>();
            foreach (Guid id in idCards)
            {
                Card card = GetObjectById(id);
                if (card != null)
                {
                    resultCard.Add(card);
                }
            }
            return resultCard;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            return ListModifyObjects(null, true, out error);
        }

        public IList<IModifyObject> ListModifyObjects(Guid[] except, bool allowRelatedCards, out Exception error)
        {
            var listCard = List(out error);
            var listCardModifyObj = new List<IModifyObject>();
            if (listCard != null)
            {
                var pairedCards = CardPairs.Singleton.GetAllPairedCards();

                foreach (var card in listCard)
                {
                    if (except != null && except.Contains(card.IdCard))
                        continue;

                    if (!allowRelatedCards && pairedCards != null && pairedCards.Contains(card.IdCard))
                        continue;

                    listCardModifyObj.Add(new CardModifyObj(card));
                }

                listCardModifyObj = listCardModifyObj.OrderBy(card => card.ToString()).ToList();
            }
            return listCardModifyObj;
        }

        public IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listCard = SelectByCriteria(filterSettings, out error);
            var listCardModifyObj = new List<IModifyObject>();
            if (listCard != null)
            {
                foreach (var card in listCard)
                {
                    listCardModifyObj.Add(new CardModifyObj(card));
                }
                listCardModifyObj = listCardModifyObj.OrderBy(card => card.ToString()).ToList();
            }
            return listCardModifyObj;
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idCard)
        {
            var objects = new List<AOrmObject>();

            var card = GetById(idCard);
            if (card != null)
            {
                if (card.CardSystem != null)
                {
                    objects.Add(card.CardSystem);
                }
            }

            return objects;
        }

        private const int CSV_IMPORT_TIMER_DELAY = 600000;
        private bool _csvImportInProgress;
        private TimerCarrier _csvImportTimer = null;

        public bool EnableStartCSVImport(out bool licenceRestriction)
        {
            licenceRestriction = false;

#if !DEBUG
            string localisedName = string.Empty;
            object value = null;
            if (CgpServer.Singleton.GetLicencePropertyInfo(RequiredLicenceProperties.OfflineImport.ToString(), out localisedName, out value))
            {
                if (value is bool && (bool)value == true)
                {
#endif
            if (!_csvImportInProgress)
            {
                _csvImportInProgress = true;
                return true;
            }
            return false;
#if !DEBUG
                }
            }

            licenceRestriction = true;
            return false;
#endif
        }

        private readonly EventHandlerGroup<IImportEventHandler> _importEventHandlerGroup =
            new EventHandlerGroup<IImportEventHandler>();

        public void AddImportEventHandler(IImportEventHandler importEventHandler)
        {
            _importEventHandlerGroup.Add(importEventHandler);
        }

        public bool ImportCards(Guid formIdentification, List<ImportCardData> importCardsData, CSVImportType importType, Guid guidCardSystem, out List<CSVImportCard> csvImportCards, out bool licenceRestriction, out int importedCardsCount)
        {
            csvImportCards = new List<CSVImportCard>();
            importedCardsCount = 0;

            if (!EnableStartCSVImport(out licenceRestriction))
                return false;

            try
            {
                _importEventHandlerGroup.ForEach(
                    importEventHandler =>
                        importEventHandler.ImportStarted());

                if (importCardsData != null && importCardsData.Count > 0)
                {
                    CardSystem cardSystem = null;
                    if (guidCardSystem != Guid.Empty)
                    {
                        cardSystem = CardSystems.Singleton.GetById(guidCardSystem);
                    }

                    var addedCards = new List<Card>();
                    var count = 0;
                    foreach (var importCardData in importCardsData)
                    {
                        count++;
                        var param = new object[2];
                        param[0] = formIdentification;
                        param[1] = count;
                        CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(RunImportedCardCountChanged, DelegateSequenceBlockingMode.Asynchronous, false, param);

                        var card = new Card(importCardData);
                        if (cardSystem != null)
                        {
                            if (card.Number.Length != cardSystem.LengthCardNumber())
                            {
                                csvImportCards.Add(new CSVImportCard(card.IdCard, card.Number, CSVImportResult.InvalidCardNumberLength));
                                continue;
                            }
                            card.CardSystem = cardSystem;
                            card.FullCardNumber = cardSystem.GetFullCompanyCode() + card.Number;
                        }
                        else
                        {
                            card.FullCardNumber = card.Number;
                        }

                        card.DateStateLastChange = DateTime.Now;
                        card.UtcDateStateLastChange = DateTime.UtcNow;

                        card.State = (byte)CardState.Active;

                        var settings = new FilterSettings(Person.COLUMNIDENTIFICATION, importCardData.Identification, ComparerModes.EQUALL);
                        var persons = Persons.Singleton.SelectByCriteria(new List<FilterSettings>(new[] { settings }));
                        if (persons == null || persons.Count == 0)
                        {
                            csvImportCards.Add(new CSVImportCard(card.IdCard, card.Number, CSVImportResult.NonExistingPersonalId));
                            continue;
                        }

                        switch (importType)
                        {
                            case CSVImportType.OverwriteOnConflict:
                                var oldCard = GetCardByFullNumber(card.FullCardNumber);
                                if (oldCard != null)
                                    oldCard = GetObjectForEdit(oldCard.IdCard);

                                if (oldCard == null)
                                {
                                    card.Person = persons.ElementAt(0);
                                    if (Insert(ref card))
                                    {
                                        addedCards.Add(card);
                                        csvImportCards.Add(new CSVImportCard(card.IdCard, card.Number, CSVImportResult.Added));
                                        importedCardsCount++;
                                    }
                                    else
                                        csvImportCards.Add(new CSVImportCard(Guid.Empty, card.Number, CSVImportResult.Failed));
                                }
                                else
                                {
                                    if (oldCard.Person == null)
                                    {
                                        oldCard.Person = persons.ElementAt(0);
                                        oldCard.State = (byte)CardState.Active;
                                    }
                                    if (OverrideCard(card, oldCard))
                                    {
                                        addedCards.Add(oldCard);
                                        csvImportCards.Add(new CSVImportCard(oldCard.IdCard, oldCard.Number, CSVImportResult.Overwritten));
                                        importedCardsCount++;
                                    }
                                    else
                                        csvImportCards.Add(new CSVImportCard(oldCard.IdCard, oldCard.Number, CSVImportResult.Failed));
                                }
                                break;
                        }
                    }
                }
            }
            catch { }

            _importEventHandlerGroup.ForEach(
                importEventHandler =>
                    importEventHandler.ImportDone());

            _csvImportInProgress = false;
            return true;
        }

        private static void RunImportedCardCountChanged(ARemotingCallbackHandler remoteHandler, object[] objInput)
        {
            if (objInput == null || objInput.Length != 2) return;

            if (remoteHandler is ImportedCardCountChangedHandler)
                (remoteHandler as ImportedCardCountChangedHandler).RunEvent((Guid)objInput[0], (int)objInput[1]);
        }

        private bool OverrideCard(Card newCard, Card oldCard)
        {
            if (newCard.CardSystem != oldCard.CardSystem)
                oldCard.CardSystem = newCard.CardSystem;

            if (newCard.Pin != oldCard.Pin)
            {
                oldCard.Pin = newCard.Pin;
                oldCard.PinLength = newCard.PinLength;
            }

            var result = Update(oldCard);
            EditEnd(oldCard);
            return result;
        }

        private static void AfterImport(List<Card> addedCards)
        {
            foreach (var card in addedCards)
            {
                if (card.Person != null)
                {
                    DbWatcher.Singleton.DbCardChanged(card.Person, card);
                }
            }
        }

        public Card GetCardFromFullCardNumber(string fullCardNumber)
        {
            var listCard = SelectLinq<Card>(card => card.FullCardNumber == fullCardNumber);
            if (listCard != null && listCard.Count > 0)
            {
                return listCard.ElementAt(0);
            }

            return null;
        }

        public IList<IModifyObject> ModifyObjectsFormPersonAddCard(Guid idPerson, out Exception error)
        {
            error = null;
            var listCardModifyObj = new List<IModifyObject>();
            try
            {
                ICollection<Card> listCard;

                if (GeneralOptions.Singleton.ListOnlyUnassignedCardsInPersonForm)
                {
                    listCard = SelectLinq<Card>(card => card.Person == null);
                }
                else
                {
                    var person = Persons.Singleton.GetById(idPerson);

                    listCard =
                        person == null
                            ? List()
                            : SelectLinq<Card>(card => card.Person != person || card.Person == null);
                }

                if (listCard != null)
                {
                    foreach (var card in listCard)
                    {
                        if (!(StructuredSubSites.Singleton.HasAccessUpdate(card)
                              || (HasAccessViewForObject(card)
                                  && (card.Person == null
                                      || StructuredSubSites.Singleton.HasAccessUpdate(card.Person)))))
                        {
                            continue;
                        }

                        listCardModifyObj.Add(new CardModifyObj(card));
                    }

                    listCardModifyObj = listCardModifyObj.OrderBy(card => card.ToString()).ToList();
                }
            }
            catch (Exception exError)
            {
                error = exError;
            }

            return listCardModifyObj;
        }

        public bool SetCardsToPerson(IList<Guid> listIdCards, Guid idPerson)
        {
            try
            {
                var person = Persons.Singleton.GetObjectForEdit(idPerson);
                if (person == null)
                    return false;

                foreach (var idCard in listIdCards)
                {
                    var card = GetObjectForEdit(idCard);
                    if (card != null)
                    {
                        if (CardPairs.Singleton.IsCardRelated(card.IdCard))
                            card.State = (byte)CardState.HybridActive;
                        else
                            card.State = (byte)CardState.Active;
                        card.Person = person;

                        if (!Singleton.UpdateOnlyInDatabase(card))
                            return false;

                        person.Cards.Add(card);
                    }
                }

                return Persons.Singleton.Update(person);
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveCardsFromPerson(IList<Guid> listIdCards, Guid idPerson)
        {
            try
            {
                var person = Persons.Singleton.GetById(idPerson);
                foreach (var idCard in listIdCards)
                {
                    var card = GetObjectForEdit(idCard);
                    if (card != null)
                    {
                        card.Person = null;

                        if (CardPairs.Singleton.IsCardRelated(card.IdCard))
                            card.State = (byte)CardState.HybridUnused;
                        else
                            card.State = (byte)CardState.Unused;

                        Update(card);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private readonly AutoResetEvent _waitForChecksum = new AutoResetEvent(false);
        private const int WAIT_FOR_TFTP_CHECKSUM_TIMEOUT = 20000;
        private uint _validChecksum;

        public bool WaitForChecksum(uint checksum)
        {
            _validChecksum = checksum;
            if (!_waitForChecksum.WaitOne(WAIT_FOR_TFTP_CHECKSUM_TIMEOUT, false))
                return false;

            return _validChecksum == _receivedChecksum;
        }

        private uint _receivedChecksum;

        public void ProcessReceivedChecksum(uint checksum)
        {
            _receivedChecksum = checksum;
            _waitForChecksum.Set();
        }

        protected override bool AddObjectNotInStructuredSubSite(Card ormObject, bool getExistObjects)
        {
            if (ormObject == null)
                return false;

            if (ormObject.Person != null)
                return false;

            return true;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.Card; }
        }

        public void OnInvalidPinRetriesLimitReached(
            Guid idCard,
            DateTime utcTime)
        {
            var card = GetObjectForEdit(idCard);

            if (card == null)
                return;

            try
            {
                if (card.UtcDateStateLastChange > utcTime)
                    return;

                switch ((CardState)card.State)
                {
                    case CardState.Active:

                        card.State = (byte)CardState.TemporarilyBlocked;
                        break;

                    case CardState.HybridActive:

                        card.State = (byte)CardState.HybridTemporarilyBlocked;
                        break;

                    default:

                        return;
                }

                card.DateStateLastChange = DateTime.Now;
                card.UtcDateStateLastChange = utcTime;

                Update(card);
            }
            finally
            {
                EditEnd(card);
            }
        }

        public TimeSpan InvalidPinRetriesLimitReachedTimeout
        {
            private get;
            set;
        }

        public void StartTemporaryBlocks()
        {
            var temporarilyBlockedCards =
                SelectLinq<Card>(
                    card =>
                        card.State == (byte)CardState.TemporarilyBlocked
                        || card.State == (byte)CardState.HybridTemporarilyBlocked);

            if (temporarilyBlockedCards == null)
                return;

            var utcNow = DateTime.UtcNow;

            foreach (var temporarilyBlockedCard in temporarilyBlockedCards)
            {
                var remainingTime =
                    (temporarilyBlockedCard.UtcDateStateLastChange
                     + InvalidPinRetriesLimitReachedTimeout) - utcNow;

                if (remainingTime > TimeSpan.Zero)
                    _temporaryBlockTimeouts.Add(
                        temporarilyBlockedCard.IdCard,
                        (CardState)temporarilyBlockedCard.State,
                        remainingTime);
                else
                    ActivateTemporarilyBlockedCard(
                        temporarilyBlockedCard.IdCard,
                        (CardState)temporarilyBlockedCard.State);
            }
        }

        private void ActivateTemporarilyBlockedCard(
            Guid idCard,
            CardState temporarilyBlockedState)
        {
            var editedCard = GetObjectForEdit(idCard);

            if (editedCard == null)
                return;

            editedCard.State =
                temporarilyBlockedState == CardState.TemporarilyBlocked
                    ? (byte)CardState.Active
                    : (byte)CardState.HybridActive;

            Update(editedCard);
            EditEnd(editedCard);
        }

        protected override IEnumerable<Card> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<Card>(
                card =>
                    card.LocalAlarmInstruction != null
                    && card.LocalAlarmInstruction != string.Empty);
        }

        public DataTable ExportCards(IList<FilterSettings> filterSettings,  out bool bFillSection)
        {
            return ExportTableFactory.Generate(ExportTableFactory.Type._Card, filterSettings,  out bFillSection);
        }
    }
}
