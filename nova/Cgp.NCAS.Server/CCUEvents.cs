using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Net;
using Contal.IwQuick.Sys;
using Contal.LwSerialization;
using JetBrains.Annotations;
using Contal.IwQuick.CrossPlatform;

namespace Contal.Cgp.NCAS.Server
{
    // SB
    public static class EventSourceHelpers
    {
        public static void AddUniqueEventSource(this ICollection<Guid> eventSources, Guid id)
        {
            if (!eventSources.Contains(id))
            {
                eventSources.Add(id);
            }
        }
    }

    public class CcuEvents
    {
        private class DeleteEventsBatchExecutor : IBatchExecutor<UInt64>
        {
            private readonly CcuEvents _ccuEvents;

            public DeleteEventsBatchExecutor(CcuEvents ccuEvents)
            {
                _ccuEvents = ccuEvents;
            }

            public int Execute(ICollection<ulong> requests)
            {
                CCUConfigurationHandler.Singleton.SendToRemotingCCUs(
                    _ccuEvents.CcuSettings.GuidCCU,
                    "DeleteEvents",
                    requests.ToArray());

                return requests.Count;
            }
        }

        private readonly ProcessingQueue<EventOptions> _queueProcessEvents;

        private readonly LimitedQueue<UInt64> _completedEvents =
            new LimitedQueue<UInt64>(MAX_COUNT_SAVING_INSERTED_EVENTS);

        private readonly IDictionary<UInt64, EventOptions> _processedEvents =
            new Dictionary<UInt64, EventOptions>();

        private readonly BatchWorker<UInt64> _eventsToDelete;

        private const int MAX_COUNT_SAVING_INSERTED_EVENTS = 10000;

        public CcuEvents(CCUSettings ccuSettings)
        {
            CcuSettings = ccuSettings;

            _eventsToDelete =
                new BatchWorker<UInt64>(
                    new DeleteEventsBatchExecutor(this),
                    DELAY_TO_CALL_DELETE_EVENTS);

            ThisAssemblyName = GetType().Assembly.GetName().Name;

            _queueProcessEvents = new ProcessingQueue<EventOptions>();
            _queueProcessEvents.ItemProcessing += ProcessEvent;
        }

        public CCUSettings CcuSettings { get; private set; }

        public string ThisAssemblyName { get; private set; }

        private void ProcessEvent(EventOptions eventOptions)
        {
            if (CcuSettings.NcasServer == null
                || !eventOptions.Active)
            {
                return;
            }

            eventOptions.EventParameters.HandleEvent(CcuSettings);

            try
            {

                Eventlog eventlog;
                List<EventSource> eventSources;
                List<EventlogParameter> eventlogParameters;

                if (eventOptions.EventParameters.EnqueueEventlog(
                    CcuSettings,
                    out eventlog,
                    out eventSources,
                    out eventlogParameters))
                {

                    EventlogInsertQueue.Singleton.Enqueue(
                        new CcuEventlogInsertItem(
                            eventlog,
                            eventSources,
                            eventlogParameters,
                            this,
                            eventOptions.EventParameters.EventId));

                    return;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            EndEventProcessing(eventOptions.EventParameters.EventId);
        }

        public int GetNumberOfNotProcessedEvents()
        {
            return _queueProcessEvents.Count;
        }

        public void ClearEventsQueues()
        {
            lock (_processedEvents)
            {
                foreach (var eventOptions in _processedEvents.Values)
                    eventOptions.MarkNotActive();

                _completedEvents.Clear();

                _processedEvents.Clear();

                _eventsToDelete.Clear();
            }

            lock (_notStoredEvents)
            {
                foreach (var eventlogInsertItem in _notStoredEvents)
                    eventlogInsertItem.MarkNotActive();

                _notStoredEvents.Clear();
            }
        }

        private const int DELAY_TO_CALL_DELETE_EVENTS = 10000;

        public void EnqueueEvent(
            ICollection<EventParameters.EventParameters> eventsParameters)
        {
            if (eventsParameters == null)
                return;

            foreach (var evenParameters in eventsParameters)
            {
                var eventOptions = new EventOptions(evenParameters);

                if (!BeginEventProcessing(eventOptions))
                    continue;

                evenParameters.Initialize();

                _queueProcessEvents.Enqueue(
                    eventOptions,
                    PQEnqueueFlags.None,
                    evenParameters.Priority);
            }
        }

        private bool BeginEventProcessing(EventOptions eventOptions)
        {
            var eventId = eventOptions.EventParameters.EventId;

            lock (_processedEvents)
            {
                if (_processedEvents.ContainsKey(eventId))
                {
                    return false;
                }

                if (!_completedEvents.Contains(eventId))
                {
                    _processedEvents.Add(
                        eventId,
                        eventOptions);

                    return true;
                }

                _eventsToDelete.Add(eventId);

                return false;
            }
        }

        public void EndEventProcessing(UInt64 eventId)
        {
            lock (_processedEvents)
            {
                _completedEvents.Add(Enumerable.Repeat(eventId, 1));
                _processedEvents.Remove(eventId);

                _eventsToDelete.Add(eventId);
            }
        }

        private class CcuEventlogInsertItem : EventlogInsertItem
        {
            private readonly CcuEvents _ccuEvents;
            private readonly UInt64 _eventId;

            public CcuEventlogInsertItem(
                Eventlog eventlog,
                IEnumerable<EventSource> eventSources,
                IEnumerable<EventlogParameter> eventlogParameters,
                CcuEvents ccuEvents,
                UInt64 eventId)
                : base(
                    eventlog,
                    eventSources,
                    eventlogParameters)
            {
                _ccuEvents = ccuEvents;
                _eventId = eventId;
                _active = true;

                _ccuEvents.RegisterNotStoredEvent(this);
            }

            private bool _active;

            public override bool Active
            {
                get { return _active; }
            }

            public override void OnComplete()
            {
                _ccuEvents.EndEventProcessing(_eventId);
                _ccuEvents.RemoveNotStoredEvent(this);
            }

            public void MarkNotActive()
            {
                _active = false;
            }
        }

        public void ClearAllEventsAndTransfers()
        {
            ClearEventsQueues();
        }

        public int GetNumberOfNotStoredEvents()
        {
            lock (_notStoredEvents)
                return _notStoredEvents.Count;
        }

        private readonly HashSet<CcuEventlogInsertItem> _notStoredEvents =
            new HashSet<CcuEventlogInsertItem>();

        private void RegisterNotStoredEvent(CcuEventlogInsertItem ccuEventlogInsertItem)
        {
            lock (_notStoredEvents)
                _notStoredEvents.Add(ccuEventlogInsertItem);
        }

        private void RemoveNotStoredEvent(CcuEventlogInsertItem eventlogInsertItem)
        {
            lock (_notStoredEvents)
                _notStoredEvents.Remove(eventlogInsertItem);
        }


        public static ICollection<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            bool addDoorEnvironment)
        {
            var eventSources = CCUEventsManager.GetEventSourcesFromCardReader(cardReader);

            if (cardReader != null && addDoorEnvironment)
            {
                var doorEnvironmnet = DoorEnvironments.Singleton.GetDoorEnvironmentForCardReader(cardReader.IdCardReader);

                if (doorEnvironmnet != null)
                    eventSources.Add(doorEnvironmnet.IdDoorEnvironment);
            }

            return eventSources;
        }

        public static ICollection<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            bool addDoorEnvironment,
            Card card,
            out string personName)
        {
            var eventSources = GetEventSourcesFromCardReader(cardReader, addDoorEnvironment);

            personName = string.Empty;

            if (card != null)
            {
                if (card.Person != null)
                {
                    eventSources.Add(card.Person.IdPerson);
                    personName = card.Person.ToString();
                }

                eventSources.Add(card.IdCard);
            }

            return eventSources;
        }

        public static ICollection<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            bool addDoorEnvironment,
            [NotNull]
            Person person)
        {
            var eventSources = GetEventSourcesFromCardReader(cardReader, addDoorEnvironment);

            eventSources.Add(person.IdPerson);

            return eventSources;
        }

        public static ICollection<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            Card card,
            AlarmArea alarmArea,
            out string personName)
        {
            var result = GetEventSourcesFromCardReader(
                cardReader,
                false,
                card,
                out personName);

            result.Add(alarmArea.IdAlarmArea);

            return result;
        }

        public static ICollection<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            Person person,
            AlarmArea alarmArea)
        {
            var result = GetEventSourcesFromCardReader(
                cardReader,
                false,
                person);

            result.Add(alarmArea.IdAlarmArea);

            return result;
        }

        public static ICollection<Guid> GetEventSourcesFromCardReader(
            CardReader cardReader,
            AlarmArea alarmArea)
        {
            var eventSources = CCUEventsManager.GetEventSourcesFromCardReader(cardReader);

            eventSources.Add(alarmArea.IdAlarmArea);

            return eventSources;
        }

        public static ICollection<string> CreateDsmEventsParametersAndFillEventSources(
            Guid idCardReader,
            Guid idCard,
            string cardNumber,
            Guid idPerson,
            Guid idPushButton,
            string reason,
            ICollection<Guid> eventSources)
        {
            // SB - AddUniqueEventSource
            ICollection<string> result = new LinkedList<string>();

            if (idCardReader != Guid.Empty)
            {
                var cardReader = CardReaders.Singleton.GetById(idCardReader);

                if (cardReader != null)
                {
                    result.Add("CardReaderGuid");
                    result.Add(idCardReader.ToString());

                    eventSources.AddUniqueEventSource(idCardReader);
                }
            }

            if (idCard != Guid.Empty)
            {
                var card = Cards.Singleton.GetById(idCard);

                if (card != null)
                {
                    result.Add("Card number");
                    result.Add(card.GetFullCardNumber());

                    eventSources.AddUniqueEventSource(idCard);

                    if (card.Person != null)
                    {
                        result.Add("User name");
                        result.Add(card.Person.ToString());

                        eventSources.AddUniqueEventSource(card.Person.IdPerson);
                    }
                }
            }

            if (!string.IsNullOrEmpty(cardNumber))
            {
                result.Add("Card number");
                result.Add(cardNumber);

                var card = Cards.Singleton.GetCardFromFullCardNumber(cardNumber);
                if (card != null)
                {
                    eventSources.AddUniqueEventSource(card.IdCard);

                    if (card.Person != null)
                    {
                        result.Add("User name");
                        result.Add(card.Person.ToString());

                        eventSources.AddUniqueEventSource(card.Person.IdPerson);
                    }
                }
            }

            if (idPerson != Guid.Empty)
            {
                var person = Persons.Singleton.GetById(idPerson);

                if (person != null)
                {
                    result.Add("User name");
                    result.Add(person.ToString());

                    eventSources.AddUniqueEventSource(person.IdPerson);
                }
            }

            if (idCard != Guid.Empty)
            {
                var card = Cards.Singleton.GetById(idCard);

                if (card != null)
                {
                    result.Add("Card number");
                    result.Add(card.GetFullCardNumber());

                    eventSources.AddUniqueEventSource(idCard);

                    if (card.Person != null)
                    {
                        result.Add("User name");
                        result.Add(card.Person.ToString());

                        eventSources.AddUniqueEventSource(card.Person.IdPerson);
                    }
                }
            }

            if (idPushButton != Guid.Empty)
            {
                var pushButton = Inputs.Singleton.GetById(idPushButton);
                if (pushButton != null)
                {
                    result.Add("PushButtonGuid");
                    result.Add(idPushButton.ToString());

                    eventSources.AddUniqueEventSource(idPushButton);
                }
            }

            if (!string.IsNullOrEmpty(reason))
            {
                result.Add("Reason");
                result.Add(reason);
            }

            return result;
        }
    }
}