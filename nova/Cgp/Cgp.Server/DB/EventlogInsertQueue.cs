using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server.DB
{
    public sealed class EventlogInsertQueue : ASingleton<EventlogInsertQueue>
    {
        private class BatchWorker : ADbBatchWorker<EventlogInsertItem>
        {
            private class ItemWorker : ADbItemWorker
            {
                private readonly SqlCommand _eventlogInsertCommand;
                private readonly SqlCommand _eventSourcesInsertCommand;
                private readonly SqlCommand _eventlogParametersInsertCommand;

                public ItemWorker(
                    SqlConnection sqlConnection,
                    SqlCommand eventlogInsertCommand,
                    SqlCommand eventSourcesInsertCommand,
                    SqlCommand eventlogParametersInsertCommand)
                    : base(
                        sqlConnection,
                        new[]
                        {
                            eventlogInsertCommand,
                            eventSourcesInsertCommand,
                            eventlogParametersInsertCommand
                        })
                {
                    _eventlogInsertCommand = eventlogInsertCommand;
                    _eventSourcesInsertCommand = eventSourcesInsertCommand;
                    _eventlogParametersInsertCommand = eventlogParametersInsertCommand;
                }

                protected override void ProcessInternal(EventlogInsertItem item)
                {
                    if (item.Eventlog == null || !item.Active)
                        return;

                    Eventlogs.Singleton.InsertInternal(
                        item.Eventlog,
                        _eventlogInsertCommand);

                    if (item.EventSources != null)
                        foreach (var eventSource in item.EventSources)
                            EventSources.Singleton.InsertInternal(
                                eventSource,
                                _eventSourcesInsertCommand);

                    if (item.EventlogParameters == null)
                        return;

                    foreach (var eventlogParameter in item.EventlogParameters)
                    {
                        eventlogParameter.Eventlog = item.Eventlog;

                        EventlogParameters.Singleton.InsertInternal(
                            eventlogParameter,
                            _eventlogParametersInsertCommand);
                    }
                }
            }

            public BatchWorker(DbConnectionManager dbConnectionManager)
                : base(dbConnectionManager)
            {
            }

            protected override ADbItemWorker CreateDbItemWorker(
                DbConnectionHolder dbConnectionHolder)
            {
                var eventlogsInsertCommand =
                    dbConnectionHolder.GetCommand(
                        Eventlogs.Singleton.InsertCommandFactory);

                var eventSourcesInsertCommand =
                    dbConnectionHolder.GetCommand(
                        EventSources.Singleton.InsertCommandFactory);

                var eventlogParametersInsertCommand =
                    dbConnectionHolder.GetCommand(
                        EventlogParameters.Singleton.InsertCommandFactory);

                return
                    new ItemWorker(
                        dbConnectionHolder.SqlConnection,
                        eventlogsInsertCommand,
                        eventSourcesInsertCommand,
                        eventlogParametersInsertCommand);
            }

            public override int InitialConnectionRetryInterval
            {
                get { return 60000; }
            }

            public override int MaxConnectionRetryInterval
            {
                get { return 300000; }
            }

            public override int ConnectionRetryIntervalIncrement
            {
                get { return 30000; }
            }
        }

        private readonly LinkedList<EventlogInsertItem> _items =
            new LinkedList<EventlogInsertItem>();

        private LinkedList<EventlogInsertItem> DequeueBatch(int count)
        {
            var result =
                new LinkedList<EventlogInsertItem>();

            lock (_items)
            {
                if (_items.Count > count)
                {
                    while (count > 0)
                    {
                        result.AddLast(_items.First.Value);
                        _items.RemoveFirst();

                        --count;
                    }

                    return result;
                }

                foreach (var eventlogRecord in _items)
                    result.AddLast(eventlogRecord);

                _items.Clear();

                return result;
            }
        }

        public void Enqueue(
            EventlogInsertItem eventlogInsertItem)
        {
            lock (_items)
            {
                _items.AddLast(eventlogInsertItem);
            }
        }

        private readonly BatchWorker _batchWorker;
        private ITimer _dequeueTimerCarrier;

        public event Action<EventlogInsertItem> OnInsertEventlogSucceded
        {
            add { _batchWorker.OnProcessItemSucceded += value; }
            remove { _batchWorker.OnProcessItemSucceded -= value; }
        }

        private EventlogInsertQueue() : base(null)
        {
            _batchWorker = 
                new BatchWorker(
                    Eventlogs.Singleton.DbConnectionManager);

            GeneralOptions.Singleton.SetLoadedFromDatabaseCallback(StartDequeueLoop);
        }

        private void StartDequeueLoop()
        {
            _dequeueTimerCarrier = 
                TimerManager.Static.StartTimer(
                    Math.Max(
                        GeneralOptions.Singleton.DelayForSaveEvents,
                        100),
                    false,
                    InsertBatch);
        }

        private bool InsertBatch(TimerCarrier timerCarrier)
        {
            _batchWorker.Process(
                DequeueBatch(
                    Math.Max(
                        GeneralOptions.Singleton.MaxEventsCountForInsert,
                        1)));

            return true;
        }

        public void SetDelayForSaveEvents(int newDelayForSaveEvents)
        {
            _dequeueTimerCarrier.ChangeTimer(newDelayForSaveEvents);
        }
    }
}
