using System.Collections.Generic;

using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal interface IAlarmAreaSensorsEventHandler
    {
        void OnAttached(ICollection<ISensorStateAndSettings> sensors);

        void OnDetached();

        void OnSensorAdded(ISensorStateAndSettings sensorStateAndSettings);

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
}
