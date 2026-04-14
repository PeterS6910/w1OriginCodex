using System;
using System.Linq;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    public interface ICrEventlogDisplayContext
    {
        DB.AlarmArea AlarmArea
        {
            get;
        }
    }

    public interface IEventForCardReader
    {
        UInt64 EventId { get; }
        DateTime DateTime { get; }

        string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext);

        EventType EventType
        {
            get;
        }

        State? EventState
        {
            get;
        }

        string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor);

        IEnumerable<CrIconSymbol> InlinedIcons
        {
            get;
        }

        void SaveToDabase(LinkedListNode<IEventForCardReader> nodeInAllEventsList);

        IEnumerable<Guid> GetAlarmAreasForSavingEventToCrEventlog();

        void RemoveRelatedAlarmAreas(Guid idAlarmArea, out LinkedListNode<IEventForCardReader> nodeInAllEventsList);

        bool IsInRelaltedAlarmAreas(Guid idAlarmaArea);
    }

    public abstract class AEventForCardReader<T> : IEventForCardReader
        where T : EventParameters.EventParameters
    {
        protected T EventParameters { get; private set; }

        private LinkedListNode<IEventForCardReader> _nodeInAllEventsList;
        private readonly ICollection<Guid> _relatedAlarmAreas;

        public UInt64 EventId
        {
            get { return EventParameters.EventId; }
        }

        public DateTime DateTime
        {
            get { return EventParameters.DateTime; }
        }

        public abstract string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext);

        public EventType EventType
        {
            get { return EventParameters.EventType; }
        }

        public abstract State? EventState
        {
            get;
        }

        public abstract string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor);

        public abstract IEnumerable<CrIconSymbol> InlinedIcons
        {
            get;
        }

        protected AEventForCardReader(T eventParameters)
        {
            _relatedAlarmAreas = new HashSet<Guid>();
            EventParameters = eventParameters;
        }

        public void SaveToDabase(LinkedListNode<IEventForCardReader> nodeInAllEventsList)
        {
            _nodeInAllEventsList = nodeInAllEventsList;

            Database.ConfigObjectsEngine.CrEventlogEventsStorage.SaveToDatabase(
                EventParameters);
        }

        protected abstract IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog();

        public IEnumerable<Guid> GetAlarmAreasForSavingEventToCrEventlog()
        {
            var alarmAreasForSavingEventToCrEventlog = InternalGetAlarmAreasForSavingEventToCrEventlog();
            if (alarmAreasForSavingEventToCrEventlog == null)
                return null;

            foreach (var alarmAreaForSavingEventToCrEventlog in alarmAreasForSavingEventToCrEventlog)
            {
                _relatedAlarmAreas.Add(alarmAreaForSavingEventToCrEventlog);
            }

            if (_relatedAlarmAreas.Count == 0)
                return null;

            return _relatedAlarmAreas;
        }

        public void RemoveRelatedAlarmAreas(
            Guid idAlarmArea,
            out LinkedListNode<IEventForCardReader> nodeInAllEventsList)
        {
            _relatedAlarmAreas.Remove(idAlarmArea);

            nodeInAllEventsList = _relatedAlarmAreas.Count == 0
                ? _nodeInAllEventsList
                : null;
        }

        public bool IsInRelaltedAlarmAreas(Guid idAlarmaArea)
        {
            return _relatedAlarmAreas.Contains(idAlarmaArea);
        }
    }

    public abstract class ASensorEventForCardReader<T> : AEventForCardReader<T>
        where T : EventParametersWithObjectId
    {
        protected ASensorEventForCardReader(T eventParameters)
            : base(eventParameters)
        {
        }

        protected abstract DB.AlarmArea GetAlarmArea();

        public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
        {
            return CrEventlogProcessor.GetNickNameForSensor(
                EventParameters.IdObject,
                GetAlarmArea());
        }
    }

    public class EventsForCardReadersDispatcher : IEventDispatcher
    {
        private static volatile EventsForCardReadersDispatcher _singleton;
        private static readonly object _syncRoot = new object();

        private readonly LinkedList<IEventForCardReader> _globalEventList =
            new LinkedList<IEventForCardReader>();

        private readonly SyncDictionary<Guid, LinkedList<IEventForCardReader>> _eventsByAlarmArea =
            new SyncDictionary<Guid, LinkedList<IEventForCardReader>>();

        private readonly ICollection<Guid> _markedAlarmAreas =
            new HashSet<Guid>();

        private readonly SafeThread _unmarkAlarmAreasThread;
      
        private int _maxCountOfEventsForAlarmArea = 100;

        private volatile bool _readedOldEvents;

        private readonly ICollection<Action> _processEventsBeforeReadingOldEvents =
            new LinkedList<Action>();

        public int MaxCountOfEventsForAlarmArea
        {
            get
            {
                return _maxCountOfEventsForAlarmArea;
            }

            set
            {
                _maxCountOfEventsForAlarmArea = value;

                _eventsByAlarmArea.ForEach(
                    (key, alarmAreaEvents) =>
                    {
                        while (alarmAreaEvents.Count > _maxCountOfEventsForAlarmArea)
                        {
                            var eventForCardReader = alarmAreaEvents.Last.Value;
                            alarmAreaEvents.RemoveLast();

                            RemoveEventFromGlobalEventList(
                                eventForCardReader,
                                key);
                        }
                    });
            }
        }

        public int AlarmAreaMarkingDuration { get; set; }

        public event Action<Guid, bool> AlarmAreaMarkingChanged;

        public static EventsForCardReadersDispatcher Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new EventsForCardReadersDispatcher();
                        }
                    }

                return _singleton;
            }
        }

        public EventsForCardReadersDispatcher()
        {
            _unmarkAlarmAreasThread = new SafeThread(DoUnmarkAlarmAreas);
            CcuCore.Singleton.BeforeExit += BeforeExit;
        }

        public void ReadOldEvents()
        {
            try
            {
                var crEventlgoAutonomousRunEvents = Database.ConfigObjectsEngine.CrEventlogEventsStorage.GetAllEventParameters();

                if (crEventlgoAutonomousRunEvents != null)
                {
                    var notSavedEventsParamatersIds = new LinkedList<UInt64>();

                    foreach (var crEventlgoAutonomousRunEvent in crEventlgoAutonomousRunEvents)
                    {
                        if (!InternalProcessEvent(
                            crEventlgoAutonomousRunEvent,
                            false))
                        {
                            notSavedEventsParamatersIds.AddLast(
                                crEventlgoAutonomousRunEvent.EventId);
                        }
                    }

                    foreach (var notSavedEventParamatersId in notSavedEventsParamatersIds)
                    {
                        Database.ConfigObjectsEngine.CrEventlogEventsStorage.DeleteFromDatabase(
                            notSavedEventParamatersId);
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            lock (_processEventsBeforeReadingOldEvents)
            {
                _readedOldEvents = true;

                foreach (var processEventsBeforeReadingOldEvent in _processEventsBeforeReadingOldEvents)
                {
                    processEventsBeforeReadingOldEvent();
                }

                _processEventsBeforeReadingOldEvents.Clear();
            }
        }

        private void ProcessEvent(
            IEventForCardReader eventForCardReader,
            Guid idAlarmArea)
        {
            bool wasMarkedAlarmArea = false;

            _eventsByAlarmArea.GetOrAddValue(
                idAlarmArea,
                key =>
                    new LinkedList<IEventForCardReader>(),
                (key, value, newlyAdded) =>
                {
                    value.AddFirst(eventForCardReader);

                    lock (_markedAlarmAreas)
                    {
                        if (!_markedAlarmAreas.Contains(key))
                        {
                            _markedAlarmAreas.Add(key);
                            wasMarkedAlarmArea = true;
                        }
                    }

                    if (value.Count <= MaxCountOfEventsForAlarmArea)
                        return;

                    var deletedEventForCardReader = value.Last.Value;
                    value.RemoveLast();

                    RemoveEventFromGlobalEventList(
                        deletedEventForCardReader,
                        idAlarmArea);
                });

            if (AlarmAreaMarkingChanged == null
                || !wasMarkedAlarmArea)
            {
                return;
            }

            try
            {
                AlarmAreaMarkingChanged(idAlarmArea, true);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            
        }

        private void SaveEventToGlobalEventList(
            IEventForCardReader eventForCardReader,
            bool saveEventToDatabase)
        {
            LinkedListNode<IEventForCardReader> nodeInAllEventsList;

            lock (_globalEventList)
            {
                nodeInAllEventsList = _globalEventList.AddFirst(eventForCardReader);
            }

            if (saveEventToDatabase)
                eventForCardReader.SaveToDabase(nodeInAllEventsList);
        }

        private void RemoveEventFromGlobalEventList(
            IEventForCardReader eventForCardReader,
            Guid idAlarmArea)
        {
            lock (_globalEventList)
            {
                LinkedListNode<IEventForCardReader> nodeInAllEventsList;

                eventForCardReader.RemoveRelatedAlarmAreas(
                    idAlarmArea,
                    out nodeInAllEventsList);

                if (nodeInAllEventsList == null)
                    return;

                _globalEventList.Remove(nodeInAllEventsList);
            }

            Database.ConfigObjectsEngine.CrEventlogEventsStorage.DeleteFromDatabase(
                eventForCardReader.EventId);
        }

        private void BeforeExit()
        {
            if (_unmarkAlarmAreasThread.State == SafeThreadState.Suspended)
                _unmarkAlarmAreasThread.Resume();
        }

        public bool GetMarkingOfAlarmArea(Guid idAlarmArea)
        {
            if (!_eventsByAlarmArea.ContainsKey(idAlarmArea))
                return false;

            lock (_markedAlarmAreas)
                return _markedAlarmAreas.Contains(idAlarmArea);
        }

        private void DoUnmarkAlarmAreas()
        {
            while (true)
            {
                _unmarkAlarmAreasThread.WaitForResume();

                CcuCore.Singleton.Sleep(3000);

                if (CcuCore.Singleton.WasExited)
                    return;

                UnmarkAlarmAreas(true);
            }
        }

        private void UnmarkAlarmAreas(bool runAlarmAreaMarkingChanged)
        {
            var dateTimeNow = CcuCore.LocalTime;

            var unmarkedAlarmAreaIds = new LinkedList<Guid>();

            _eventsByAlarmArea.ForEach(
                (key, value) =>
                {
                    lock (_markedAlarmAreas)
                    {
                        if (!_markedAlarmAreas.Contains(key))
                            return;

                        var diferncial = dateTimeNow - value.First().DateTime;
                        if (diferncial.TotalSeconds > AlarmAreaMarkingDuration)
                        {
                            _markedAlarmAreas.Remove(key);
                            unmarkedAlarmAreaIds.AddLast(key);
                        }
                    }
                });

            if (!runAlarmAreaMarkingChanged)
                return;

            if (AlarmAreaMarkingChanged != null)
            {
                try
                {
                    foreach (var unmarkedAlarmAreaId in unmarkedAlarmAreaIds)
                    {
                        AlarmAreaMarkingChanged(unmarkedAlarmAreaId, false);
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public void StartMarkingObservation()
        {
            UnmarkAlarmAreas(false);

            if (_unmarkAlarmAreasThread.State == SafeThreadState.NotStarted)
                _unmarkAlarmAreasThread.Start();
            else
                _unmarkAlarmAreasThread.Resume();
        }

        public void StopMarkingObservation()
        {
            _unmarkAlarmAreasThread.Suspend();
        }

        public void UpdateAlarmAreaConfiguration(IEnumerable<Guid> modifiedAlarmAreaIds)
        {
            if (modifiedAlarmAreaIds == null)
                return;

            foreach (Guid idAlarmArea in modifiedAlarmAreaIds)
            {
                var alarmArea = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.AlarmArea, idAlarmArea) as DB.AlarmArea;

                if (alarmArea == null || !alarmArea.EnableEventlogsInCR)
                    RemoveAlarmAreaConfiguration(idAlarmArea);
            }
        }

        public void RemoveAlarmAreaConfiguration(Guid idAlarmArea)
        {
            _eventsByAlarmArea.Remove(
                idAlarmArea,
                (key, removedResult, removedValue) =>
                {
                    if (!removedResult)
                        return;

                    foreach (var eventForCardReader in removedValue)
                    {
                        RemoveEventFromGlobalEventList(
                            eventForCardReader,
                            idAlarmArea);
                    }
                });
        }

        #region IEventDispatcher Members

        public void ProcessEvent(
            EventParameters.EventParameters eventParameters)
        {
            lock (_processEventsBeforeReadingOldEvents)
            {
                if (!_readedOldEvents)
                {
                    _processEventsBeforeReadingOldEvents.Add(
                        () =>
                            InternalProcessEvent(
                                eventParameters,
                                true));

                    return;
                }
            }

            InternalProcessEvent(
                eventParameters,
                true);
        }

        #endregion

        private bool InternalProcessEvent(
            EventParameters.EventParameters eventParameters,
            bool saveEventToDatabase)
        {
            try
            {
                if (eventParameters == null)
                    return false;

                var eventForCardReader = eventParameters.CreateEventForCardReader();

                if (eventForCardReader == null)
                    return false;

                var idsAlarmAreas = eventForCardReader.GetAlarmAreasForSavingEventToCrEventlog();

                if (idsAlarmAreas == null)
                    return false;

                SaveEventToGlobalEventList(
                    eventForCardReader,
                    saveEventToDatabase);

                foreach (var idAlarmArea in idsAlarmAreas)
                {
                    ProcessEvent(
                        eventForCardReader,
                        idAlarmArea);
                }

                return true;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                return false;
            }
        }

        public ICollection<IEventForCardReader> GetEvents(ICollection<Guid> filterByAcl, ICollection<Guid> filterByEnableEventlog)
        {
            ICollection<IEventForCardReader> events = new LinkedList<IEventForCardReader>();

            lock (_globalEventList)
            {
                foreach (var eventForCardReader in _globalEventList)
                {
                    var actEventForCardReader = eventForCardReader;

                    if (!filterByEnableEventlog
                        .Any(actEventForCardReader.IsInRelaltedAlarmAreas))
                    {
                        continue;
                    }

                    if (filterByAcl != null
                        && !filterByAcl
                            .Any(actEventForCardReader.IsInRelaltedAlarmAreas))
                    {
                        continue;
                    }

                    events.Add(eventForCardReader);
                }
            }

            return events;
        }

        [CanBeNull] 
        public ICollection<IEventForCardReader> GetEventsForAlarmArea(Guid idAlarmArea, ICollection<Guid> filterByAcl)
        {
            if (filterByAcl != null
                && !filterByAcl.Contains(idAlarmArea))
                return null;

            LinkedList<IEventForCardReader> result;
            return _eventsByAlarmArea.TryGetValue(idAlarmArea, out result)
                ? result
                : null;
        }

        public void DeleteCrEventlogs()
        {
            lock (_globalEventList)
                _globalEventList.Clear();

            _eventsByAlarmArea.Clear();

            lock (_markedAlarmAreas)
                _markedAlarmAreas.Clear();
        }
    }
}
