using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class AlarmAreaSensorListener :
        IAlarmAreaSensorsEventHandler
    {
        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

        public interface IEventHandler
        {
            void OnAttached(ICollection<ISensorStateAndSettings> sensors);

            void OnDetached();

            void OnSensorAdded(
                ISensorStateAndSettings sensorStateAndSettings,
                ISensorStateAndSettings predecessor);

            void OnSensorRemoved(ISensorStateAndSettings sensorStateAndSettings);

            void OnSensorBlockingTypeChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                SensorBlockingType sensorBlockingType);

            void OnSensorInAlarmStateChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                bool inAlarm);

            void OnSensorInTamperStateChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                bool inTamper);

            void OnSensorNotAcknowledgedChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                bool newValue);
        }

        private class Comparer : IComparer<ISensorStateAndSettings>
        {
            public int Compare(
                ISensorStateAndSettings x,
                ISensorStateAndSettings y)
            {
                if (ReferenceEquals(
                    x,
                    y))
                {
                    return 0;
                }

                var result = x.SensorId.CompareTo(y.SensorId);

                return result != 0
                    ? result
                    : x.IdInput.CompareTo(y.IdInput);
            }
        }

        private static readonly IComparer<ISensorStateAndSettings> _comparer =
            new Comparer();

        private SimpleSortedList<ISensorStateAndSettings> _sortedSensors;

        private readonly EventHandlerGroup<IEventHandler> _eventHandlerGroup =
            new EventHandlerGroup<IEventHandler>();

        public AlarmAreaSensorListener(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            _crAlarmAreaInfo = crAlarmAreaInfo;
        }

        public CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get { return _crAlarmAreaInfo; }
        }

        public void AttachSensorEventHandler(IEventHandler sensorsEventHandler)
        {
            if (_eventHandlerGroup.Count == 0)
                _crAlarmAreaInfo.AddSensorsEventHandler(this);

            _eventHandlerGroup.Add(sensorsEventHandler);
            sensorsEventHandler.OnAttached(_sortedSensors);
        }

        public void DetachSensorEventHandler(IEventHandler sensorsEventHandler)
        {
            if (!_eventHandlerGroup.Remove(sensorsEventHandler))
                return;

            if (_eventHandlerGroup.Count == 0)
                _crAlarmAreaInfo.RemoveSensorsEventHandler(this);

            sensorsEventHandler.OnDetached();
        }

        public void OnAttached(ICollection<ISensorStateAndSettings> sensors)
        {
            _sortedSensors = new SimpleSortedList<ISensorStateAndSettings>(
                _comparer,
                sensors);
        }

        public void OnDetached()
        {
            _sortedSensors.Clear();
            _sortedSensors = null;
        }

        void IAlarmAreaSensorsEventHandler.OnSensorAdded(ISensorStateAndSettings sensorStateAndSettings)
        {
            var predecessor =
                _sortedSensors.AddAndGetPredecessor(sensorStateAndSettings);

            _eventHandlerGroup.ForEach(eventHandler =>
                eventHandler.OnSensorAdded(
                    sensorStateAndSettings,
                    predecessor));
        }

        void IAlarmAreaSensorsEventHandler.OnSensorRemoved(ISensorStateAndSettings sensorStateAndSettings)
        {
            if (_sortedSensors.Remove(sensorStateAndSettings))
                _eventHandlerGroup.ForEach(eventHandler =>
                    eventHandler.OnSensorRemoved(sensorStateAndSettings));
        }

        void IAlarmAreaSensorsEventHandler.OnSensorBlockingTypeChanged(
            ISensorStateAndSettings sensorStateAndSettings,
            SensorBlockingType sensorBlockingType)
        {
            _eventHandlerGroup.ForEach(
                eventHandler =>
                    eventHandler.OnSensorBlockingTypeChanged(
                        sensorStateAndSettings,
                        sensorBlockingType));
        }

        void IAlarmAreaSensorsEventHandler.OnSensorInAlarmStateChanged(
            ISensorStateAndSettings sensorStateAndSettings,
            bool inAlarm)
        {
            _eventHandlerGroup.ForEach(
                eventHandler =>
                    eventHandler.OnSensorInAlarmStateChanged(
                        sensorStateAndSettings,
                        inAlarm));
        }

        void IAlarmAreaSensorsEventHandler.OnSensorInTamperStateChanged(
            ISensorStateAndSettings sensorStateAndSettings,
            bool inTamper)
        {
            _eventHandlerGroup.ForEach(
                eventHandler =>
                    eventHandler.OnSensorInTamperStateChanged(
                        sensorStateAndSettings,
                        inTamper));
        }

        void IAlarmAreaSensorsEventHandler.OnSensorNotAcknowledgedChanged(
            ISensorStateAndSettings sensorStateAndSettings,
            bool newValue)
        {
            _eventHandlerGroup.ForEach(
                eventHandler =>
                    eventHandler.OnSensorNotAcknowledgedChanged(
                        sensorStateAndSettings,
                        newValue));
        }
    }
}