using System;
using System.Collections.Generic;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.Globals;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal interface IAlarmAreaController
    {
        State ActivationState
        {
            get;
        }

        State AlarmState
        {
            get;
        }

        Guid Id
        {
            get;
        }

        IBoolExpression InAlarm { get; }
        IBoolExpression SirenOutput { get; }
        IBoolExpression InSabotage { get; }
        IBoolExpression AnySensorInAlarm { get; }
        IBoolExpression AlarmNotAcknowledged { get; }
        IBoolExpression AnySensorNotAcknowledged { get; }
        IBoolExpression AnySensorPermanentlyBlocked { get; }
        IBoolExpression AnySensorTemporarilyBlocked { get; }
        IBoolExpression NotAcknowledged { get; }

        bool SetAlarmArea(
            bool setSynchronously,
            AlarmAreas.SetUnsetParams setUnsetParams);

        void Update(DB.AlarmArea alarmArea);

        void OnDeactivatedObjectForAa();

        void Dispose();

        void UnsetAlarmArea(AlarmAreaStateAndSettings.IAlarmAreaUnsetResult alarmAreaUnsetResult,
            bool unsetSynchronously,
            Guid guidLogin,
            Guid guidPerson,
            int timeToBuy,
            [NotNull]
            AlarmAreas.SetUnsetParams setUnsetParams);

        void OnAlarmAcknowledged(AlarmsManager.IActionSources acknowledgeSources);

        void OnActivatedObjectForAa();

        void SendSabotageStateInfoToServer();

        bool HasAnySensorActive();

        void OnBoughtTimeExpired(
            int lastBoughtTime,
            int totalBoughtTime);

        int GetSensorId(Guid idInput);
        
        SensorPurpose GetSensorPurpose(Guid idInput);

        bool HasInactiveObjectForForcedTimeBuying();

        bool IsTimeBuyingEnabled();

        bool ProvideUnsetForOnlyTimeBuyingAccessRightsSync();

        void OnSensorAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources);

        void OnSensorTamperAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources);

        void Unconfigure(DB.AlarmArea newDbObject);

        void Init(DB.AlarmArea alarmArea);

        ICollection<ISensorStateAndSettings> GetSensors();

        void AcknowledgeAllSensorAlarms(
            Guid idCardReader,
            AccessDataBase accessData);

        void SendBlockingTypeForAllSensors();

        void SendStateForAllSensors();

        void SetSensorBlockingType(
            Guid idInput,
            SensorBlockingType sensorBlockingType);

        void SendTimeBuyingMatrixStateToServer();
    }
}