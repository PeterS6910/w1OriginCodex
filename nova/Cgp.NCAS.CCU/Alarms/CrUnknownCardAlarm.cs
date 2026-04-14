using System;
using System.Collections.Generic;
using System.Linq;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(743)]
    public class CrUnknownCardAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public CrUnknownCardAlarm()
        {

        }

        public CrUnknownCardAlarm(
            Guid idCardReader,
            string fullCardNumber)
            : base(
                new AlarmKey(
                    AlarmType.CardReader_UnknownCard,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader),
                    Enumerable.Repeat(
                        new AlarmParameter(
                            ParameterType.CardNumber,
                            fullCardNumber), 
                        1)),
                AlarmState.Normal)
        {

        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCrUnknownCard(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCrUnknownCard(
                false,
                this);
        }

        ICollection<CatAlarm> ICreateCatAlarms.CreateCatAlarms(bool alarmAcknowledged)
        {
            if (alarmAcknowledged
                || AlarmKey == null
                || AlarmKey.AlarmObject == null)
            {
                return null;
            }

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmKey.AlarmType,
                AlarmKey.AlarmObject);

            if (alarmArcs == null)
                return null;

            var cardReader = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.CardReader,
                (Guid) AlarmKey.AlarmObject.Id) as CardReader;

            if (cardReader == null)
                return null;

            var parameters = AlarmKey.Parameters;

            var cardNumberAlarmParameter = parameters != null
                ? parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.CardNumber)
                : null;

            var fullCardNumber = cardNumberAlarmParameter != null
                ? cardNumberAlarmParameter.Value
                : null;

            return new[]
            {
                CatAlarmsManager.CreateCrAlarm(
                    AlarmEventCode.AccessDenied,
                    this,
                    cardReader,
                    fullCardNumber,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
