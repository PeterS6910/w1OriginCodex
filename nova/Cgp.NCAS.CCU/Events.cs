using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public static class Events
    {
        private class ProcessEventQueueRequest : IProcessingQueueRequest
        {
            private readonly EventParameters.EventParameters _eventParameters;

            public ProcessEventQueueRequest(EventParameters.EventParameters eventParameters)
            {
                _eventParameters = eventParameters;
            }

            public void Execute()
            {
                UInt64 eventId;
                if (!GlobalEventIds.GetNextEventId(out eventId))
                    return;

                _eventParameters.EventId = eventId;

                lock (_eventDispatchers)
                {
                    foreach (var eventDispatcher in _eventDispatchers)
                    {
                        eventDispatcher.ProcessEvent(_eventParameters);
                    }
                }
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private static readonly ICollection<IEventDispatcher> _eventDispatchers =
            new HashSet<IEventDispatcher>();

        private static readonly ThreadPoolQueue<ProcessEventQueueRequest> _processEventQueue;

        static Events()
        {
            _processEventQueue =
                new ThreadPoolQueue<ProcessEventQueueRequest>(ThreadPoolGetter.Get());
        }


        public static void AddEventDispatcher(IEventDispatcher eventDispatcher)
        {
            lock (_eventDispatchers)
            {
                if (eventDispatcher == null || _eventDispatchers.Contains(eventDispatcher))
                    return;

                _eventDispatchers.Add(eventDispatcher);
            }
        }

        public static void RemoveEventDispatcher(IEventDispatcher eventDispatcher)
        {
            lock (_eventDispatchers)
            {
                if (eventDispatcher == null)
                    return;

                _eventDispatchers.Remove(eventDispatcher);
            }
        }

        public static void ProcessEvent(EventParameters.EventParameters eventParameters)
        {
            _processEventQueue.Enqueue(new ProcessEventQueueRequest(eventParameters));
        }

        /// <summary>
        /// Save and send event run method fialed to the server
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="exception"></param>
        public static void SaveEventRunMethodFailed(
            string methodName,
            Exception exception)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Events.SaveEventRunMethodFailed(string methodName, Exception exception): [{0}]",
                    Log.GetStringFromParameters(
                        methodName,
                        exception)));

            ProcessEvent(
                new EventRunMethodFailed(
                    methodName,
                    exception.ToString()));
        }
    }
}
