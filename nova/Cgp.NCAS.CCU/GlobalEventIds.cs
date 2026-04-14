using System;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public static class GlobalEventIds
    {
        private const int FirstBlockInRtcMemoryForSavingEventIdTreshold = 24;
        private const UInt64 IncrementOfEventIdTreshold = 100;

        private static volatile bool _savingEventsEnabled = true;

        private static readonly object _getNextEventIdLock = new object();
        private static UInt64 _lastEventId;
        private static UInt64 _eventIdTreshold;

        static GlobalEventIds()
        {
            ReadLastEventId();
            SetNextEventIdTreshold();
        }

        /// <summary>
        /// Returns next Id
        /// </summary>
        public static bool GetNextEventId(out UInt64 eventId)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void GlobalEventIds.NextEventId");
            lock (_getNextEventIdLock)
            {
                eventId = 0;

                if (!_savingEventsEnabled)
                    return false;

                if (_lastEventId >= Int64.MaxValue)
                    ResetCore();

                eventId = ++_lastEventId;

                if (_lastEventId > _eventIdTreshold)
                {
                    SetNextEventIdTreshold();
                }

                return true;
            }
        }

        private static void ReadLastEventId()
        {
            try
            {
                var lastEventIdData = new UInt32[2];

                RTCMemory.Read(
                    FirstBlockInRtcMemoryForSavingEventIdTreshold,
                    ref lastEventIdData);

                _lastEventId = lastEventIdData[0];
                _lastEventId = (_lastEventId << 32) + lastEventIdData[1];

                if (_lastEventId >= Int64.MaxValue)
                    ResetCore();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private static void SetNextEventIdTreshold()
        {
            _eventIdTreshold = _lastEventId + IncrementOfEventIdTreshold;
            SaveEventIdTreshold(_eventIdTreshold);
        }

        private static void SaveEventIdTreshold(UInt64 eventIdTreshold)
        {
            try
            {
                var lastEventIdBytes = BitConverter.GetBytes(eventIdTreshold);
                var lastEventIdData = new[]
                {
                    BitConverter.ToUInt32(lastEventIdBytes, 4),
                    BitConverter.ToUInt32(lastEventIdBytes, 0)
                };

                RTCMemory.Write(
                    FirstBlockInRtcMemoryForSavingEventIdTreshold,
                    lastEventIdData);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public static void SaveActualEventIdBeforeExitCcu()
        {
            lock (_getNextEventIdLock)
            {
                if (!_savingEventsEnabled)
                    return;

                _savingEventsEnabled = false;

                SaveEventIdTreshold(_lastEventId);
            }
        }

        public static void Reset()
        {
            lock (_getNextEventIdLock)
            {
                ResetCore();
            }
        }

        private static void ResetCore()
        {
            _lastEventId = 0;
            SetNextEventIdTreshold();
        }
    }
}
