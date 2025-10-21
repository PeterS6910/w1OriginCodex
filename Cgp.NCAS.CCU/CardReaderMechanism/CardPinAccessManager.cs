using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    public sealed class CardPinAccessManager :
        ASingleton<CardPinAccessManager>,
        DB.IDbObjectChangeListener<DB.Card>
    {
        private readonly SyncDictionary<Guid, int> _consecutiveInvalidPinCountsByCard =
            new SyncDictionary<Guid, int>();

        private readonly TimeoutDictionary<Guid, bool> _cardTimeouts;

        private CardPinAccessManager()
            : base(null)
        {
            _cardTimeouts = 
                new TimeoutDictionary<Guid, bool>(
                    DevicesAlarmSettings.Singleton.InvalidPinRetriesLimitReachedTimeout * 60000);

            _cardTimeouts.ItemTimedOut += _cardTimeouts_ItemTimedOut;
        }

        void _cardTimeouts_ItemTimedOut(
            Guid idCard,
            bool value,
            int timeout)
        {
            AlarmsManager.Singleton.StopAlarm(
                CrInvalidPinRetriesLimitReached.CreateAlarmKey(
                    idCard,
                    Guid.Empty));
        }

        public void Reset()
        {
            _cardTimeouts.Clear();
        }

        public void SetInvalidPinRetriesLimitReachedTimeout(int timeout)
        {
            _cardTimeouts.DefaultTimeout = timeout * 60000;
        }

        public bool AuthorizeByPin(
            Guid idCard,
            string pin,
            string codeData,
            Guid idCardReader,
            bool invalidPinRetriesLimitEnabled)
        {
            if (_cardTimeouts.ContainsKey(idCard))
                return false;

            if (pin == QuickHashes.GetCRC32String(codeData))
            {
                _consecutiveInvalidPinCountsByCard.Remove(idCard);
                return true;
            }

            if (invalidPinRetriesLimitEnabled)
                ReportWrongAccessPin(
                    idCard,
                    idCardReader);

            return false;
        }

        private void ReportWrongAccessPin(
            Guid idCard,
            Guid idCardReader)
        {
            int consecutiveInvalidPinCount;

            _consecutiveInvalidPinCountsByCard.TryGetValue(
                idCard,
                out consecutiveInvalidPinCount);

            if (++consecutiveInvalidPinCount < DevicesAlarmSettings.Singleton.InvalidPinRetriesCount)
            {
                _consecutiveInvalidPinCountsByCard[idCard] = consecutiveInvalidPinCount;

                return;
            }

            _consecutiveInvalidPinCountsByCard.Remove(idCard);

            _cardTimeouts.Add(
                idCard,
                true);

            AlarmsManager.Singleton.AddAlarm(
                new CrInvalidPinRetriesLimitReached(
                    idCard,
                    idCardReader));

            Events.ProcessEvent(
                new EventInvalidPinRetriesLimitReached(idCard));
        }

        public void Configure(ICollection<Guid> configuredCardIds)
        {
            var temporarilyBlockedCardIds = Database.ConfigObjectsEngine.CardsStorage.GetCardIdsByState(
                (byte) DB.CardState.temporarilyBlocked);

            foreach (var idConfiguredCard in configuredCardIds)
            {
                if (!temporarilyBlockedCardIds.Contains(idConfiguredCard))
                    continue;

                var card = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Card,
                    idConfiguredCard) as DB.Card;

                if (card == null)
                    continue;

                var remainingTimeout =
                    _cardTimeouts.DefaultTimeout
                    - (int) (DateTime.UtcNow - card.UtcDateStateLastChange).TotalMilliseconds;

                if (remainingTimeout > 0)
                    _cardTimeouts.GetOrAddValue(
                        idConfiguredCard,
                        (Guid key,
                            out bool value,
                            out int timeout) =>
                        {
                            value = false;

                            timeout = remainingTimeout;
                        },
                        // TODO update timeout in obscure corner case of offline temporary block
                        null);
                else
                    AlarmsManager.Singleton.StopAlarm(
                        CrInvalidPinRetriesLimitReached.CreateAlarmKey(
                            idConfiguredCard,
                            Guid.Empty));
            }
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            DB.Card newCard)
        {
            if (newCard.State == (byte) DB.CardState.active)
            {
                _cardTimeouts.Remove(newCard.IdCard);

                AlarmsManager.Singleton.StopAlarm(
                    CrInvalidPinRetriesLimitReached.CreateAlarmKey(
                        idObject,
                        Guid.Empty));

                return;
            }

            _consecutiveInvalidPinCountsByCard.Remove(newCard.IdCard);

            if (newCard.State != (byte) DB.CardState.temporarilyBlocked)
            {
                _cardTimeouts.Remove(newCard.IdCard);

                AlarmsManager.Singleton.StopAlarm(
                    CrInvalidPinRetriesLimitReached.CreateAlarmKey(
                        idObject,
                        Guid.Empty));
            }
        }

        public void OnObjectSaved(
            Guid idObject,
            DB.Card newCard)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            _consecutiveInvalidPinCountsByCard.Remove(idObject);
            _cardTimeouts.Remove(idObject);

            AlarmsManager.Singleton.StopAlarm(
                CrInvalidPinRetriesLimitReached.CreateAlarmKey(
                    idObject,
                    Guid.Empty));
        }
    }
}
