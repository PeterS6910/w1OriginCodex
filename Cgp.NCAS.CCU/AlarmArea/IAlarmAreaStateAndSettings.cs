using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.BoolExpressions.CrossPlatform;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal interface IAlarmAreaStateAndSettings
    {
        State AlarmState
        {
            get;
        }

        State ActivationState
        {
            get;
        }

        Guid Id
        {
            get;
        }

        DB.AlarmArea DbObject
        {
            get;
        }

        string AlarmAreaName
        {
            get;
        }

        void AddSensorsEventHandler(IAlarmAreaSensorsEventHandler sensorsEventHandler);
        void RemoveSensorsEventHandler(IAlarmAreaSensorsEventHandler sensorsEventHandler);

        void AcknowledgeAllSensorAlarms(
            Guid idCardReader,
            AccessDataBase accessData);

        IBoolExpression InAlarm { get; }

        IBoolExpression SirenOutput { get; }

        IBoolExpression InSabotage { get; }

        IBoolExpression AlarmNotAcknowledged { get; }

        IBoolExpression AnySensorInAlarm { get; }
        
        IBoolExpression AnySensorNotAcknowledged { get; }

        IBoolExpression AnySensorPermanentlyBlocked { get; }

        IBoolExpression AnySensorTemporarilyBlocked { get; }

        IBoolExpression NotAcknowledged { get; }
    }
}