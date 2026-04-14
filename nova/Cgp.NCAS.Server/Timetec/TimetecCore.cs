using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;
using WcfServiceNovaConnection;
using Contal.IwQuick.CrossPlatform;

namespace Contal.Cgp.NCAS.Server.Timetec
{
    public sealed class TimetecCore : ASingleton<TimetecCore>
    {
        private class EventProcessor
        {
            private const int MAX_RETRIES_COUNT = 10;

            private readonly TransitionObject _transitionObject;
            private int _retriesCount;

            public Int64 EventId { get; private set; }

            public EventProcessor(
                Int64 eventId,
                TransitionObject transitionObject)
            {
                EventId = eventId;
                _transitionObject = transitionObject;
            }

            public bool Process([NotNull] WcfHelper wcfHelper)
            {
                var result = wcfHelper.TimetecInsertTransition(_transitionObject);

                if (result == null)
                    return false;

                switch (result.Value)
                {
                    case TransitionAddResult.TRANSITION_ADD_ERROR:
                    case TransitionAddResult.TRANSITION_ADD_UNKNOWN_ERROR:
                        if (IncrementRetriesCount())
                            return false;

                        InsertEventlog(
                            result.Value,
                            EventId);
                            
                        return true;

                    case TransitionAddResult.TRANSITION_ADD_INSUFFICIENT_RIGHTS:
                    case TransitionAddResult.TRANSITION_ADD_NO_USER_LOGGED_IN:
                    case TransitionAddResult.TRANSITION_ADD_READER_BAD_PARAMETERS:
                    case TransitionAddResult.TRANSITION_ADD_READER_DOES_NOT_EXIST:
                    case TransitionAddResult.TRANSITION_ADD_READER_NO_PERMANENT_TRANSITION:
                    case TransitionAddResult.TRANSITION_ADD_LOGIN_USER_DOES_NOT_EXIST:
                    case TransitionAddResult.TRANSITION_ADD_CARD_DOES_NOT_EXIST:
                    case TransitionAddResult.TRANSITION_ADD_CARD_SYSTEM_DOES_NOT_EXIST:
                    case TransitionAddResult.TRANSITION_ADD_CARD_USER_DOES_NOT_EXIST:
                        InsertEventlog(
                            result.Value,
                            EventId);
                        return true;

                    default:
                        return true;
                }
            }

            private bool IncrementRetriesCount()
            {
                if (_retriesCount < MAX_RETRIES_COUNT)
                {
                    _retriesCount++;
                    return true;
                }

                return false;
            }

            private void InsertEventlog(
                TransitionAddResult transitionAddResult,
                Int64 idEventlog)
            {
                var eventlog = Eventlogs.Singleton.GetObjectById(idEventlog);

                if (eventlog == null)
                    return;

                Eventlogs.Singleton.InsertEvent(
                    Eventlog.TYPE_TIMETEC_EVENT_TRANSFER_FAILED,
                    GetType()
                        .Assembly.GetName()
                        .Name,
                    eventlog.EventSources.Select(
                        eventSource =>
                            eventSource.EventSourceObjectGuid).ToArray(),
                    string.Format(
                        "Transfer failed for event: {0}   {1}, with reason: {2}",
                        eventlog.Type,
                        eventlog.EventlogDateTime,
                        transitionAddResult),
                    new[]
                    {
                        new EventlogParameter(
                            "Eventlog id",
                            idEventlog.ToString()),
                        new EventlogParameter(
                            "Reason",
                            transitionAddResult.ToString())
                    });
            }
        }

        private class ChangeObjectEventProcessor : IProcessChangeEventManager
        {
            private readonly WcfHelper _wcfHelper;
            private readonly LinkedList<ObjectChangeResult> _objectChangeResults = new LinkedList<ObjectChangeResult>();

            public ChangeObjectEventProcessor(WcfHelper wcfHelper)
            {
                _wcfHelper = wcfHelper;
            }

            public void SendResults()
            {
                _wcfHelper.SendObjectChangeResults(_objectChangeResults);
                _objectChangeResults.Clear();
            }

            public void ResetResultBuffer()
            {
                _objectChangeResults.Clear();
            }

            public void PersonSave(PersonObject personObject, int version)
            {
                var person = GetPersonByPersonalNumber(personObject.PersonalNumber);

                var savePersonResult = new SavePersonResult
                {
                    IdObject = personObject.PersonalNumber,
                    Version = version,
                    Result = ObjectChangeProccessResult.FAILED
                };

                _objectChangeResults.AddLast(savePersonResult);

                var timetecSetting = TimetecSettings.Singleton.GetTimetecSetting();

                if (person == null)
                {
                    person = new Person();
                    SetPersonParameters(person, personObject);

                    if (Persons.Singleton.Insert(ref person))
                    {
                        if (!timetecSetting.DoNotImportDepartments)
                            Persons.Singleton.SetPersonDepartment(CreatePersonDepartmentRecursive(personObject.Department), person.IdPerson);
                        savePersonResult.Result = ObjectChangeProccessResult.SUCCESS;
                    }
                }
                else
                {
                    person = Persons.Singleton.GetObjectForEdit(person.IdPerson);

                    if (person == null)
                        return;

                    SetPersonParameters(person, personObject);

                    savePersonResult.Result = Persons.Singleton.Update(person)
                        ? ObjectChangeProccessResult.SUCCESS
                        : ObjectChangeProccessResult.FAILED;

                    Persons.Singleton.EditEnd(person);

                    // Update also Department (if changed)
                    if (!timetecSetting.DoNotImportDepartments)
                    {
                        person.Department = UserFoldersStructures.Singleton.GetPersonDepartment(person.GetIdString());
                        Persons.Singleton.SetPersonDepartment(CreatePersonDepartmentRecursive(personObject.Department), person.Department, person.IdPerson);
                    }
                }

                if (savePersonResult.Result == ObjectChangeProccessResult.SUCCESS)
                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_TIMETEC_EVENT_SAVE_OBJECT,
                        DateTime.Now,
                        GetType()
                            .Assembly.GetName()
                            .Name,
                        new[] {person.IdPerson},
                        "Timetec person was synchronized");
            }

            private Person GetPersonByPersonalNumber(string personalNumber)
            {
                var filterSetting = new FilterSettings(Person.COLUMNIDENTIFICATION,
                    personalNumber,
                    ComparerModes.EQUALL);

                var persons = Persons.Singleton.SelectByCriteria(new List<FilterSettings> { filterSetting });

                return persons.FirstOrDefault();
            }

            private void SetPersonParameters(Person person, PersonObject personObject)
            {
                var stringBuilder = new StringBuilder();

                person.SynchronizedWithTimetec = true;
                person.FirstName = personObject.FirstName;
                person.Surname = personObject.LastName;
                person.WholeName = string.Format("{0} {1}", person.FirstName, person.Surname);
                person.Identification = personObject.PersonalNumber;
                person.Birthday = personObject.DateOfBirth;

                if (!string.IsNullOrEmpty(personObject.StreetHouseNumber))
                    stringBuilder.Append(personObject.StreetHouseNumber);

                if (!string.IsNullOrEmpty(personObject.CityCode))
                    stringBuilder.AppendFormat(
                        stringBuilder.Length == 0
                            ? "{0}"
                            : ", {0}",
                        personObject.CityCode);

                if (!string.IsNullOrEmpty(personObject.City))
                    stringBuilder.AppendFormat(
                        stringBuilder.Length == 0
                            ? "{0}"
                            : ", {0}",
                        personObject.City);

                person.Address = stringBuilder.ToString();

                person.Tiltle = string.Format("{0} {1}",
                    personObject.FirstTitulus,
                    personObject.LastTitulus);
                
                person.PhoneNumber = personObject.HomePhoneNumber;
                person.Email = personObject.Email;
                person.EmploymentBeginningDate = personObject.DateArrival;
                person.EmploymentEndDate = personObject.DateDeparture;
            }

            public void PersonDelete(string personalNumber, int version)
            {
                var person = GetPersonByPersonalNumber(personalNumber);

                var deletePersonResult = new DeletePersonResult
                {
                    IdObject = personalNumber,
                    Version = version,
                    Result = ObjectChangeProccessResult.FAILED
                };

                _objectChangeResults.AddLast(deletePersonResult);

                if (person == null)
                {
                    deletePersonResult.Result = ObjectChangeProccessResult.SUCCESS;
                    return;
                }

                deletePersonResult.Result = Persons.Singleton.DeleteById(person.IdPerson)
                    ? ObjectChangeProccessResult.SUCCESS
                    : ObjectChangeProccessResult.FAILED;

                if (deletePersonResult.Result == ObjectChangeProccessResult.SUCCESS)
                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_TIMETEC_EVENT_DELETE_OBJECT,
                        DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        null,
                        string.Format("Timetec person was deleted ({0}, {1})", person.WholeName, person.Identification));
            }

            public UserFoldersStructure CreatePersonDepartmentRecursive(string departmentFullPath, UserFoldersStructure parent = null)
            {
                UserFoldersStructure result = null;

                // Department looks like this: "Skupina\Pod\Uroven" or just "Skupina"
                int index = departmentFullPath.IndexOf('\\');
                if (index > -1)
                {
                    string subDepartment = departmentFullPath.Substring(0, index);
                    parent = CreateFolder(subDepartment, parent);

                    // Create path (recursive)
                    result = CreatePersonDepartmentRecursive(departmentFullPath.Substring(index+1), parent);
                }
                else
                    result = CreateFolder(departmentFullPath, parent);

                return result;
            }

            private UserFoldersStructure CreateFolder(string name, UserFoldersStructure parent)
            {
                UserFoldersStructure result = null;

                // Check department for existence 
                var folder = UserFoldersStructures.Singleton.FolderStructureSearchExactName(name, parent);
                if (folder != null)
                {
                    // Correct folder
                    result = folder;
                }

                // Create it if not exists
                if (result == null && name != null && name.Length > 0)
                {
                    result = new UserFoldersStructure();
                    result.ParentFolder = parent;
                    result.FolderName = name;
                    UserFoldersStructures.Singleton.Insert(ref result);
                }

                return result;
            }

            public void CardSave(CardObject cardObject, int version)
            {
                var saveCardResult = new SaveCardResult
                {
                    IdObject = cardObject.CardNumber+"#"+cardObject.ID,
                    Version = version,
                    Result = ObjectChangeProccessResult.FAILED
                };

                _objectChangeResults.AddLast(saveCardResult);

                if (cardObject.PIN.Length < 4
                    || cardObject.PIN.Length > 8)
                {
                    saveCardResult.Result = ObjectChangeProccessResult.CARD_PIN_ERROR;
                    return;
                }

                Person person = null;

                if (!string.IsNullOrEmpty(cardObject.PersonalNumber))
                {
                    person = GetPersonByPersonalNumber(cardObject.PersonalNumber);

                    if (person == null)
                    {
                        saveCardResult.Result = ObjectChangeProccessResult.CARD_PERSON_NOT_FOUND;
                        return;
                    }
                }

                var card = Cards.Singleton.GetCardByFullNumber(cardObject.CardNumber);

                if (card == null)
                {
                    card = new Card();
                    SetCardParameters(card, cardObject, person);

                    if (Cards.Singleton.Insert(ref card))
                        saveCardResult.Result = ObjectChangeProccessResult.SUCCESS;
                }
                else
                {
                    card = Cards.Singleton.GetObjectForEdit(card.IdCard);

                    if (card == null)
                        return;

                    SetCardParameters(card, cardObject, person);

                    if (Cards.Singleton.Update(card))
                        saveCardResult.Result = ObjectChangeProccessResult.SUCCESS;

                    Cards.Singleton.EditEnd(card);
                }

                if (saveCardResult.Result == ObjectChangeProccessResult.SUCCESS)
                    Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_TIMETEC_EVENT_SAVE_OBJECT,
                        DateTime.Now,
                        GetType()
                            .Assembly.GetName()
                            .Name,
                        new[] {card.IdCard},
                        "Timetec card was synchronized");
            }

            private void SetCardParameters(Card card, CardObject cardObject, Person person)
            {
                card.SynchronizedWithTimetec = true;
                card.FullCardNumber = cardObject.CardNumber;
                card.Number = cardObject.CardNumber;
                card.ValidityDateFrom = cardObject.CardSince;
                card.ValidityDateTo = cardObject.CardUntill;

                if (!string.IsNullOrEmpty(cardObject.PIN))
                {
                    card.Pin = QuickHashes.GetCRC32String(cardObject.PIN);
                    card.PinLength = (byte) cardObject.PIN.Length;
                }

                card.Description = cardObject.Description;
                card.DateStateLastChange = DateTime.Now;
                card.UtcDateStateLastChange = DateTime.Now;

                switch (cardObject.Settings)
                {
                    case CardSettings.LOST_CARD:
                        card.State = (byte) CardState.Lost;
                        break;

                    default:
                        if (string.IsNullOrEmpty(cardObject.PersonalNumber))
                            card.State = (byte) CardState.Unused;
                        else
                            card.State = (byte) CardState.Active;
                        break;
                }

                if (person != null)
                    card.Person = person;
            }

            public void CardDelete(string cardNumber, int version)
            {
                var parts = cardNumber.Split('#');

                var deleteCardResult = new DeleteCardResult
                {
                    IdObject = cardNumber,
                    Version = version,
                    Result = ObjectChangeProccessResult.FAILED
                };

                if (parts.Length > 0)
                {
                    var card = Cards.Singleton.GetCardByFullNumber(parts[0]);

                    _objectChangeResults.AddLast(deleteCardResult);

                    if (card == null)
                    {
                        deleteCardResult.Result = ObjectChangeProccessResult.SUCCESS;
                        return;
                    }

                    if (card.Person != null)
                    {
                        // Detach Card from Person to allow the deleting the card (otherwise the Constraint exception is thrown)
                        var updCard = Cards.Singleton.GetObjectForEdit(card.IdCard);
                        updCard.Person = null;

                        Cards.Singleton.Update(updCard);
                        Cards.Singleton.EditEnd(updCard);

                        // Reload the Card object (without Person reference)
                        card = null;
                        card = Cards.Singleton.GetCardByFullNumber(parts[0]);
                    }

                    deleteCardResult.Result = Cards.Singleton.DeleteById(card.IdCard)
                        ? ObjectChangeProccessResult.SUCCESS
                        : ObjectChangeProccessResult.FAILED;

                  if (deleteCardResult.Result == ObjectChangeProccessResult.SUCCESS)
                      Eventlogs.Singleton.InsertEvent(
                        Eventlog.TYPE_TIMETEC_EVENT_DELETE_OBJECT,
                        DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        null,
                        string.Format("Timetec card was deleted ({0})", card.FullCardNumber + (parts.Length > 1? "#"+parts[1]:"")));
                }
            }
        }

        private abstract class Request
        {
            public abstract void Execute(TimetecCore timetecCore);
        }

        private class CommunicationOnlineRequest : Request
        {
            public override void Execute(TimetecCore timetecCore)
            {
                try
                {
                    if (timetecCore._processEvents)
                        return;

                    var wcfHelperIsConnected = timetecCore._wcfHelper.IsConnected;

                    if (!wcfHelperIsConnected.HasValue
                        || !wcfHelperIsConnected.Value)
                    {
                        return;
                    }

                    var filterSettings = new List<FilterSettings>();

                    var lastEventId = TimetecSettings.Singleton.LastEventId;

                    if (lastEventId >= 0)
                        filterSettings.Add(
                            new FilterSettings(
                                Eventlog.COLUMN_ID_EVENTLOG,
                                lastEventId,
                                ComparerModes.MORE));
                    else
                    {
                        var timetecSettings = TimetecSettings.Singleton.GetTimetecSetting();

                        var dateTimeFrom = timetecSettings != null
                            ? timetecSettings.DefaultStartDateTime
                            : DateTime.Now.Date;

                        filterSettings.Add(
                            new FilterSettings(
                                Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                                dateTimeFrom,
                                ComparerModes.EQUALLMORE));
                    }

                    filterSettings.Add(
                        new FilterSettings(
                            Eventlog.COLUMN_TYPE,
                            new List<string>
                            {
                                Eventlog.TYPEDSMNORMALACCESS
                            },
                            ComparerModes.IN));

                    lock (timetecCore._relevantCardReaderIds)
                        filterSettings.Add(
                            new FilterSettings(
                                Eventlog.COLUMN_EVENTSOURCES,
                                timetecCore._relevantCardReaderIds.ToList(),
                                ComparerModes.IN));

                    var eventlogs = Eventlogs.Singleton.SelectByCriteria(filterSettings);

                    var dbConnectionHolder = Eventlogs.Singleton.DbConnectionManager.Get();

                    timetecCore._eventProcessorBatchWorker.Add(
                        new LinkedList<EventProcessor>(
                            eventlogs.Select(
                                eventlog =>
                                {
                                    EventlogParameters.Singleton.GetParametersForEventlog(
                                        eventlog,
                                        dbConnectionHolder);

                                    var eventProcessor = timetecCore.CreateEventProcessor(
                                        eventlog,
                                        eventlog.EventlogParameters,
                                        idCardReader => true);

                                    return eventProcessor;
                                })
                                .Where(
                                    eventProcessor =>
                                        eventProcessor != null)));

                    Eventlogs.Singleton.DbConnectionManager.Return(dbConnectionHolder);

                    timetecCore._processEvents = true;
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);

                    TimerManager.Static.StartTimeout(
                        60000,
                        timer =>
                        {
                            timetecCore._requestProcessingQueue.Enqueue(
                                new CommunicationOnlineRequest());

                            return true;
                        });
                }
            }
        }

        private class CommunicationOfflineRequest : Request
        {
            public override void Execute(TimetecCore timetecCore)
            {
                if (!timetecCore._processEvents)
                    return;

                timetecCore._processEvents = false;
                timetecCore._eventProcessorBatchWorker.Clear();
                timetecCore._objectChangedEventProcessorBatchWorker.Clear();
                timetecCore._changeObjectEventProcessor.ResetResultBuffer();
            }
        }

        private class ProcessEventRequest : Request
        {
            private readonly EventProcessor _eventProcessor;

            public ProcessEventRequest(EventProcessor eventProcessor)
            {
                _eventProcessor = eventProcessor;
            }

            public override void Execute(TimetecCore timetecCore)
            {
                if (!timetecCore._processEvents)
                {
                    return;
                }

                timetecCore._eventProcessorBatchWorker.Add(_eventProcessor);
            }
        }

        private class EventBatchExecutor : IBatchExecutor<EventProcessor>
        {
            private readonly TimetecCore _timetecCore;

            public EventBatchExecutor(TimetecCore timetecCore)
            {
                _timetecCore = timetecCore;
            }

            public int Execute(ICollection<EventProcessor> requests)
            {
                int processedEvents = 0;
                Int64? lastEventId = null;

                try
                {
                    foreach (var eventProcessor in requests)
                    {
                        if (!eventProcessor.Process(_timetecCore._wcfHelper))
                            break;

                        processedEvents++;
                        lastEventId = eventProcessor.EventId;
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                if (lastEventId.HasValue)
                    try
                    {
                        TimetecSettings.Singleton.LastEventId = lastEventId.Value;
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }

                return processedEvents;
            }
        }

        private class ChangeObjectEventBatchExecutor : ABatchExecutor<ObjectChangeEvent, TimetecCore>
        {
            private bool _continueInexecuting;

            public ChangeObjectEventBatchExecutor(TimetecCore timetecCore) : base(timetecCore)
            {
            }

            protected override bool BeforeBatch(TimetecCore timetecCore)
            {
                _continueInexecuting = true;
                return true;
            }

            protected override void ExecuteInternal(
                ObjectChangeEvent request,
                TimetecCore timetecCore)
            {
                if (!_continueInexecuting)
                    return;

                var isConnected = timetecCore.IsConnected();

                if (!isConnected.HasValue || !isConnected.Value)
                {
                    _continueInexecuting = false;
                    return;
                }

                request.Process(timetecCore._changeObjectEventProcessor);
            }

            protected override bool OnError(
                ObjectChangeEvent request,
                Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return true;
            }

            protected override void AfterBatch(TimetecCore timetecCore)
            {
                if (!_continueInexecuting)
                    return;

                timetecCore._changeObjectEventProcessor.SendResults();
            }
        }

        private const int PROCESS_EVENT_TIME_OUT = 5000;
        private const int PROCESS_EVENT_LIMIT_COUNT = 1000;

        private readonly WcfHelper _wcfHelper;
        private readonly BatchWorker<EventProcessor> _eventProcessorBatchWorker;
        private readonly BatchWorker<ObjectChangeEvent> _objectChangedEventProcessorBatchWorker;
        private readonly ChangeObjectEventProcessor _changeObjectEventProcessor;
        private readonly HashSet<Guid> _relevantCardReaderIds = new HashSet<Guid>();
        private bool _processEvents;
        private readonly ProcessingQueue<Request> _requestProcessingQueue;
        private readonly Dictionary<string, TransitionType> _transitionTypes =
            new Dictionary<string, TransitionType>(); 

        private TimetecCore()
            : base(null)
        {
            _transitionTypes.Add(Eventlog.TYPEDSMNORMALACCESS, TransitionType.NORMAL_ACCESS);
            _transitionTypes.Add(Eventlog.TYPEDSMACCESSPERMITTED, TransitionType.ACCESS_PERMITTED);
            _transitionTypes.Add(Eventlog.TYPEDSMACCESSINTERRUPTED, TransitionType.ACCESS_INTERRUPTED);
            _transitionTypes.Add(Eventlog.TYPEDSMACCESSRESTRICTED, TransitionType.ACCESS_RESTRICTED);

            _wcfHelper = new WcfHelper();
            _wcfHelper.OnlineStateChanged += _wcfHelper_OnlineStateChanged;

            _changeObjectEventProcessor = new ChangeObjectEventProcessor(_wcfHelper);

            _eventProcessorBatchWorker = new BatchWorker<EventProcessor>(
                new EventBatchExecutor(this),
                PROCESS_EVENT_TIME_OUT,
                PROCESS_EVENT_LIMIT_COUNT);

            _objectChangedEventProcessorBatchWorker = new BatchWorker<ObjectChangeEvent>(
                new ChangeObjectEventBatchExecutor(this),
                PROCESS_EVENT_TIME_OUT,
                PROCESS_EVENT_LIMIT_COUNT);

            _requestProcessingQueue = new ProcessingQueue<Request>();
            _requestProcessingQueue.ItemProcessing += _requestProcessingQueue_ItemProcessing;

            EventlogInsertQueue.Singleton.OnInsertEventlogSucceded += OnInsertEventlogSucceded;
        }

        private void OnInsertEventlogSucceded(EventlogInsertItem eventlogInsertItem)
        {
            if (eventlogInsertItem == null
                || eventlogInsertItem.Eventlog == null
                || (eventlogInsertItem.Eventlog.Type != Eventlog.TYPEDSMNORMALACCESS
                    && eventlogInsertItem.Eventlog.Type != Eventlog.TYPEACCESSDENIED
                    && eventlogInsertItem.Eventlog.Type != Eventlog.TYPEDSMACCESSPERMITTED
                    && eventlogInsertItem.Eventlog.Type != Eventlog.TYPEDSMACCESSINTERRUPTED
                    && eventlogInsertItem.Eventlog.Type != Eventlog.TYPE_TIMETEC_EVENT_TRANSFER_FAILED))
            {
                return;
            }

            if (eventlogInsertItem.Eventlog.Type == Eventlog.TYPE_TIMETEC_EVENT_TRANSFER_FAILED)
            {
                TimetecSettings.Singleton.InsertTimetecErrorEvent(eventlogInsertItem.Eventlog.IdEventlog);
                return;
            }

            var eventProcessor = CreateEventProcessor(
                eventlogInsertItem.Eventlog,
                new LinkedList<EventlogParameter>(eventlogInsertItem.EventlogParameters),
                idCardReader =>
                {
                    lock (_relevantCardReaderIds)
                        return _relevantCardReaderIds.Contains(idCardReader);
                });

            if (eventProcessor == null)
                return;

            _requestProcessingQueue.Enqueue(new ProcessEventRequest(eventProcessor));
        }

        private EventProcessor CreateEventProcessor(
            Eventlog eventlog,
            ICollection<EventlogParameter> parameters,
            Func<Guid, bool> addCardReader)
        {
            var transitionObject = CreateTransitionObject(
                eventlog,
                parameters,
                addCardReader);

            if (transitionObject == null)
                return null;

            return
                new EventProcessor(
                    eventlog.IdEventlog,
                    transitionObject);
        }

        private TransitionObject CreateTransitionObject(
            Eventlog eventlog,
            ICollection<EventlogParameter> parameters,
            Func<Guid, bool> addCardReader)
        {
            if (parameters == null)
                return null;

            var idCardReader = parameters
                .Where(
                    eventlogParameter =>
                        eventlogParameter.TypeObjectType == (byte)ObjectType.CardReader)
                .Select(
                    eventlogParameter =>
                        eventlogParameter.TypeGuid)
                .FirstOrDefault(addCardReader);

            if (idCardReader == Guid.Empty)
                return null;

            var cardReader = CardReaders.Singleton.GetById(idCardReader);

            if (cardReader == null)
                return null;

            var cardNumber = parameters
                .Where(
                    eventlogParameter =>
                        eventlogParameter.Type == "Card number")
                .Select(
                    eventlogParameter =>
                        eventlogParameter.Value)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(cardNumber))
                return null;

            return
                new TransitionObject
                {
                    AddDateTime = eventlog.EventlogDateTime,
                    CardNumber = cardNumber,
                    ReaderName = cardReader.Name,
                    TransitionType = _transitionTypes[eventlog.Type]
                };
        }

        void _requestProcessingQueue_ItemProcessing(Request request)
        {
            request.Execute(this);
        }

        public void InsertObjectChangeEvents(IEnumerable<ObjectChangeEvent> objectChangeEvents)
        {
            _objectChangedEventProcessorBatchWorker.Add(objectChangeEvents);
        }

        void _wcfHelper_OnlineStateChanged(bool? isConnected)
        {
            TimetecCommunicationOnlineStateChanged(isConnected);

            if (isConnected.HasValue 
                && isConnected.Value)
            {
                _requestProcessingQueue.Enqueue(new CommunicationOnlineRequest());
                return;
            }

            _requestProcessingQueue.Enqueue(new CommunicationOfflineRequest());
        }

        public void Init()
        {
            var timetecSettings = TimetecSettings.Singleton.GetTimetecSetting();

            if (timetecSettings == null)
                return;

            lock (_relevantCardReaderIds)
            {
                if (timetecSettings.CardReaders != null)
                    foreach (var cardReader in timetecSettings.CardReaders)
                    {
                        _relevantCardReaderIds.Add(cardReader.IdCardReader);
                    }
            }

            StartCommunication(timetecSettings);
        }

        private void StartCommunication(TimetecSetting timetecSetting)
        {
            if (timetecSetting == null
                || !timetecSetting.IsEnabled)
            {
                return;
            }

            _wcfHelper.StartCommunication(
                timetecSetting.IpAddress,
                timetecSetting.Port,
                timetecSetting.LoginName,
                IwQuick.Crypto.QuickCrypto.Decrypt(
                    timetecSetting.LoginPassword,
                    CgpServerGlobals.DATABASE_KEY.ToString()),
                IwQuick.Crypto.QuickCrypto.Decrypt(
                    timetecSetting.CertificateData,
                    CgpServerGlobals.DATABASE_KEY.ToString()));
        }

        public void AfterTimetecCommunicationSettingsUpdate(TimetecSetting timetecSetting)
        {
            _wcfHelper.StopCommunication();
            StartCommunication(timetecSetting);
        }

        public void AfterTimetecCardReaderSettingsUpdate(ICollection<CardReader> cardReaders)
        {
            lock (_relevantCardReaderIds)
            {
                _relevantCardReaderIds.Clear();

                if (cardReaders != null)
                    foreach (var cardReader in cardReaders)
                    {
                        _relevantCardReaderIds.Add(cardReader.IdCardReader);
                    }
            }
        }

        public bool? IsConnected()
        {
            return _wcfHelper.IsConnected;
        }

        public Dictionary<int, TransitionAddResult> TryResendEvents(ICollection<TimetecErrorEvent> errorEvents)
        {
            if (!_wcfHelper.IsConnected.HasValue
                || !_wcfHelper.IsConnected.Value)
            {
                return null;
            }

            var result = new Dictionary<int, TransitionAddResult>();

            foreach (var errorEvent in errorEvents)
            {
                var transitionObject = CreateTransitionObject(
                    errorEvent.SourceEventlog,
                    errorEvent.SourceEventlog.EventlogParameters,
                    idCardReader => true);

                if (transitionObject == null)
                    result.Add(errorEvent.Id, TransitionAddResult.TRANSITION_ADD_UNKNOWN_ERROR);

                var transferResult = _wcfHelper.TimetecInsertTransition(transitionObject)
                                     ?? TransitionAddResult.TRANSITION_ADD_UNKNOWN_ERROR;

                result.Add(
                    errorEvent.Id,
                    transferResult);

                if (transferResult == TransitionAddResult.TRANSITION_ADD_SUCCESS
                    || transferResult == TransitionAddResult.TRANSITION_ADD_SUCCESS_EXISTED)
                {
                    TimetecSettings.Singleton.RemoveTimetecErrorEvent(errorEvent.Id);
                }
            }

            return result;
        }

        public void TimetecCommunicationOnlineStateChanged(bool? isConnected)
        {
            Eventlogs.Singleton.InsertEvent(
                Eventlog.TYPE_TIMETEC_COMMUNICATION_ONLINE_STATE_CHANGED,
                GetType()
                    .Assembly.GetName()
                    .Name,
                null,
                string.Format(
                    "Timetec communication is: {0}",
                    !isConnected.HasValue
                        ? "Unknown"
                        : isConnected.Value
                            ? "Online"
                            : "Offline"));

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunTimetecCommunicationOnlineStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    isConnected
                });
        }

        public static void RunTimetecCommunicationOnlineStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] parameters)
        {
            var timetecCommunicationOnlineStateChangedHandler =
                remoteHandler as TimetecCommunicationOnlineStateChangedHandler;

            if (timetecCommunicationOnlineStateChangedHandler != null)
                timetecCommunicationOnlineStateChangedHandler.RunEvent((bool?)parameters[0]);
        }
    }
}
