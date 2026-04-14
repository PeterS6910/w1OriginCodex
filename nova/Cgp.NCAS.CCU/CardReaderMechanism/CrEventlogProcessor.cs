using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.IwQuick.Data;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    public class CrEventlogProcessor
    {
        private readonly CrDisplayProcessor _crDisplayProcessor;

        private ICollection<Guid> _alarmAreasEnableEventlog;
        private ICollection<Guid> _alarmAreaGuidsForEventLogInCr;

        public CrEventlogProcessor(CrDisplayProcessor crDisplayProcessor)
        {
            _crDisplayProcessor = crDisplayProcessor;
        }

        public static string GetNickNameForAlarmArea(Guid idAlarmArea)
        {
            var alarmArea = AlarmArea.AlarmAreas.Singleton.GetAlarmArea(idAlarmArea);

            return alarmArea != null
                ? alarmArea.ToString()
                : string.Empty;
        }

        public static string GetNickNameForSensor(
            Guid idInput,
            DB.AlarmArea alarmArea)
        {
            var input = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.Input,
                idInput) as DB.Input;

            if (input == null)
                return string.Empty;

            if (alarmArea == null)
                return input.NickName;

            return string.Format(
                "{0}{1} {2}",
                alarmArea.Id.ToString("D2"),
                AlarmArea.AlarmAreas.Singleton.GetSensorId(
                    alarmArea.IdAlarmArea,
                    input.IdInput).ToString("D2"),
                input.NickName);
        }

        public string GetCardInformation(
            Guid idCard)
        {
            string cardNumber = string.Empty;

            if (idCard != Guid.Empty)
                cardNumber = Database.ConfigObjectsEngine.CardsStorage.GetFullCardNumber(idCard);

            return !string.IsNullOrEmpty(cardNumber)
                ? string.Format("{0}:\\n{1}",
                    _crDisplayProcessor.GetLocalizationString("Card"),
                    cardNumber)
                : null;
        }

        public string GetObjectTypeInformation(
            ObjectType typeOfObjectForAutomaticActivation)
        {
            return string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("ObjectType"),
                _crDisplayProcessor.GetLocalizationString(
                    string.Format(
                        "ObjectType_{0}",
                        typeOfObjectForAutomaticActivation)));
        }

        private static string GetTimeStringFromSeconds(int seconds)
        {
            int hours = seconds / 3600;
            int rest = seconds % 3600;

            int minutes = rest / 60;
            rest = rest % 60;

            return string.Format("{0}:{1}:{2}",
                hours.ToString("D2"),
                minutes.ToString("D2"),
                rest.ToString("D2"));
        }

        public string[] GetAlarmAreaBoughtTimeExpiredInformations(
            int lastBoughtTime,
            int totalBoughtTime)
        {
            var informations = new string[2];

            informations[0] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("lastBoughtTime"),
                GetTimeStringFromSeconds(lastBoughtTime));

            informations[1] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("totalBoughtTime"),
                GetTimeStringFromSeconds(totalBoughtTime));

            return informations;
        }

        public string[] GetAlarmAreaTimeBuyingFailedInformations(
            int timeToBuy,
            int remainingTime)
        {
            var informations = new string[2];

            informations[0] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("TimeToBuy"),
                GetTimeStringFromSeconds(timeToBuy));

            informations[1] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("RemainingTime"),
                GetTimeStringFromSeconds(remainingTime));

            return informations;
        }

        public string[] GetAlarmAreaBoughtTimeChangedInformations(
            Guid idCard,
            int usedTime,
            int remainingTime)
        {
            var informations = new string[3];

            informations[0] = idCard != Guid.Empty
                ? string.Format(
                    "{0}: {1}",
                    _crDisplayProcessor.GetLocalizationString("Card"),
                    Database.ConfigObjectsEngine.CardsStorage.GetFullCardNumber(idCard))
                : null;

            informations[1] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("UsedTime"),
                GetTimeStringFromSeconds(usedTime));

            informations[2] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("RemainingTime"),
                GetTimeStringFromSeconds(remainingTime));
                
            return informations;
        }

        public string[] GetTemporarilyBlockedInputChangedInformations(
            Guid idCard,
            Guid idAlarmArea)
        {
            var informations = new string[2];

            var cardInformation = GetCardInformation(idCard);
            informations[0] = !string.IsNullOrEmpty(cardInformation)
                ? string.Format(
                    "{0}: {1}",
                    _crDisplayProcessor.GetLocalizationString("Card"),
                    cardInformation)
                : null;

            informations[1] = string.Format(
                "{0}: {1}",
                _crDisplayProcessor.GetLocalizationString("AlarmArea"),
                GetNickNameForAlarmArea(idAlarmArea));

            return informations;
        }

        public void DrawEventInformation(
            IEventForCardReader eventForCardReader,
            ICrEventlogDisplayContext alarmArea)
        {
            var header =
                string.Format(
                    "{0} {1}",
                    _crDisplayProcessor.GetLocalizationString("Type"),
                    _crDisplayProcessor.GetLocalizationString(eventForCardReader.EventType.ToString()));

            var objectName = 
                string.Format(
                    "{0} {1}",
                    _crDisplayProcessor.GetLocalizationString("Object"),
                    eventForCardReader.GetEventObjectName(alarmArea));

            byte top = 0;

            top = _crDisplayProcessor.DisplayWriteText(header, 0, top);
            top = _crDisplayProcessor.DisplayWriteText(objectName, 0, top);

            var state = eventForCardReader.EventState;

            if (state != null)
                top = _crDisplayProcessor.DisplayWriteText(
                    string.Format(
                        "{0} {1}",
                        _crDisplayProcessor.GetLocalizationString("State"),
                        _crDisplayProcessor.GetLocalizationString(state.ToString())),
                    0,
                    top);

            var dateTime = eventForCardReader.DateTime;

            string time = string.Format(
                "{6}:{0}-{1}-{2} {3}:{4}:{5}",
                dateTime.Year.ToString("D2"),
                dateTime.Month.ToString("D2"),
                dateTime.Day.ToString("D2"),
                dateTime.Hour.ToString("D2"),
                dateTime.Minute.ToString("D2"),
                dateTime.Second.ToString("D2"),
                _crDisplayProcessor.GetLocalizationString("Time"));

            top = _crDisplayProcessor.DisplayWriteText(time, 0, top);

            var extraParameters = eventForCardReader.GetExtraParameters(this);

            if (extraParameters != null)
                foreach (string eventParameter in extraParameters)
                    top = _crDisplayProcessor.DisplayWriteText(eventParameter, 0, top);
        }

        [CanBeNull]
        public IEnumerable<IEventForCardReader> GetEventsForAlarmArea(Guid idAlarmArea)
        {
            return idAlarmArea == Guid.Empty
                ? EventsForCardReadersDispatcher.Singleton.GetEvents(
                    _alarmAreaGuidsForEventLogInCr,
                    _alarmAreasEnableEventlog)
                : EventsForCardReadersDispatcher.Singleton.GetEventsForAlarmArea(
                    idAlarmArea,
                    _alarmAreaGuidsForEventLogInCr);
        }

        private static ICollection<Guid> GetAlarmAreasWithEnabledEventlog(
            DB.CardReader cardReaderDb)
        {
            var aaGuids = new HashSet<Guid>();

            var aaCardReaders = Database.ConfigObjectsEngine.AaCardReadersStorage.GetAaCardReadersByIdCardReader(
                cardReaderDb.IdCardReader);

            if (aaCardReaders != null)
                foreach (var aaCardReader in aaCardReaders)
                {
                    if (aaCardReader.EnableEventlog)
                        aaGuids.Add(aaCardReader.GuidAlarmArea);
                }

            return aaGuids;
        }

        public bool ControlAccessToEventlog(AccessDataBase accessData)
        {
            _alarmAreaGuidsForEventLogInCr =
                AlarmAreaAccessRightsManager.Singleton.GetAlarmAreasGuid(accessData);

            return
                _alarmAreaGuidsForEventLogInCr != null
                && _alarmAreaGuidsForEventLogInCr.Any(
                    aaGuid =>
                        _alarmAreasEnableEventlog.Contains(aaGuid));
        }

        public bool SetAlarmAreasWithEnabledEventlog(DB.CardReader cardReaderDb)
        {
             _alarmAreasEnableEventlog = GetAlarmAreasWithEnabledEventlog(cardReaderDb);

            return 
                _alarmAreasEnableEventlog == null
                || _alarmAreasEnableEventlog.Count != 0;
        }

        public void SetAllAlarmAreas()
        {
            _alarmAreaGuidsForEventLogInCr = 
                Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.AlarmArea);
        }
    }
}
