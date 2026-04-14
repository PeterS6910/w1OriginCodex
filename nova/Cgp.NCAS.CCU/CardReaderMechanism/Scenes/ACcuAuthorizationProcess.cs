using System;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Crypto;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    public class AccessDataBase
    {
        public virtual Guid IdCard
        {
            get { return Guid.Empty; }
        }

        public virtual Guid IdPerson { get { return Guid.Empty; } }

        public virtual Guid IdPushButton { get { return Guid.Empty; } }

        public virtual AccessGrantedSource AccessGrantedSource { get { return AccessGrantedSource.Other; } }

        public virtual bool EntryViaCard { get { return false; } }

        public virtual bool EntryViaPersonalCode { get { return false; } }
    }

    internal abstract class ACcuAuthorizationProcess : AAuthorizationProcessBase
    {
        private class ExtendedAccessData : AccessDataBase
        {
            public virtual string Pin { get { return null; } }

            public virtual int PinLength { get { return -1; } }
        }

        private class PersonalCodeAccessData : ExtendedAccessData
        {
            private readonly Guid _idPerson;

            public override Guid IdPerson
            {
                get { return _idPerson; }
            }

            public override bool EntryViaPersonalCode{ get { return true; } }

            public PersonalCodeAccessData(Guid idPerson)
            {
                _idPerson = idPerson;
            }
        }

        private class CardAccessData : ExtendedAccessData
        {
            private readonly ICard _card;

            public override Guid IdCard
            {
                get { return _card.IdCard; }
            }

            public override Guid IdPerson { get { return _card.GuidPerson; } }

            public override bool EntryViaCard { get { return true; } }

            public override string Pin { get { return _card.Pin; } }

            public override int PinLength { get { return _card.PinLength; } }

            public override AccessGrantedSource AccessGrantedSource
            {
                get { return AccessGrantedSource.Card; }
            }

            public CardAccessData(ICard card)
            {
                _card = card;
            }
        }

        private ExtendedAccessData _accessData;

        public AccessDataBase AccessData { get { return _accessData; } }

        public abstract ACardReaderSettings CardReaderSettings
        {
            get;
        }

        private void OnAccessDeniedCardBlockedOrInactive()
        {
            if (BlockedAlarmsManager.Singleton.ProcessEvent(
                AlarmType.CardReader_CardBlockedOrInactive,
                new IdAndObjectType(
                    CardReaderSettings.Id,
                    ObjectType.CardReader)))
            {
                Events.ProcessEvent(
                    new EventAccessDeniedCardBlockedOrInactive(
                        CardReaderSettings.Id,
                        _accessData));
            }

            AlarmsManager.Singleton.AddAlarm(
                new CrCardBlockedOrInactiveAlarm(
                    CardReaderSettings.Id,
                    _accessData.IdCard));
        }

        private void OnAccessDeniedUnknownCard()
        {
            if (BlockedAlarmsManager.Singleton.ProcessEvent(
                AlarmType.CardReader_UnknownCard,
                new IdAndObjectType(
                    CardReaderSettings.Id,
                    ObjectType.CardReader)))
            {
                Events.ProcessEvent(
                    new EventUnknownCard(
                        CardReaderSettings.Id,
                        CardData));
            }

            AlarmsManager.Singleton.AddAlarm(
                new CrUnknownCardAlarm(
                    CardReaderSettings.Id,
                    CardData));
        }

        protected abstract bool AuthorizeByCardInternal();

        protected abstract void OnAccessDeniedNoRightsForCard();

        protected override bool AuthorizeByCard(out bool isRedundant)
        {
            var card =
                Database.ConfigObjectsEngine.CardsStorage.GetCard(CardData);

            if (card == null)
            {
                OnAccessDeniedUnknownCard();
                isRedundant = false;

                return false;
            }

            _accessData = new CardAccessData(card);

            if ((card.State != (byte) DB.CardState.active
                 && card.State != (byte) DB.CardState.temporarilyBlocked)
                || !card.IsValid)
            {
                OnAccessDeniedCardBlockedOrInactive();
                isRedundant = false;

                return false;
            }

            if (!AuthorizeByCardInternal())
            {
                OnAccessDeniedNoRightsForCard();
                isRedundant = false;

                return false;
            }

            isRedundant = IsRedundant;

            return true;
        }

        protected sealed override bool AuthorizeByPin(string codeData)
        {
            if (CardPinAccessManager.Singleton.AuthorizeByPin(
                _accessData.IdCard,
                _accessData.Pin,
                codeData,
                CardReaderSettings.Id,
                CardReaderSettings.InvalidPinRetriesLimitEnabled))
            {
                return true;
            }

            OnAccessDeniedInvalidPin();
            return false;
        }

        protected abstract void OnAccessDeniedInvalidPin();

        protected abstract bool AuthorizeByPersonInternal();

        protected abstract void OnAccessDeniedNoRightsForPerson();

        protected override sealed bool AuthorizeByCode(
            string codeData,
            out bool isRedundant)
        {
            if (CardReaderSettings.InvalidCodeRetriesLimitReached)
            {
                isRedundant = false;
                return false;
            }

            _accessData = AuthorizeByCode(codeData);

            if (_accessData == null)
            {
                OnAccessDeniedInvalidCode();
                CardReaderSettings.ReportWrongAccessCode();

                isRedundant = false;
                return false;
            }

            CardReaderSettings.ReportCorrectAccessCode();

            if (_accessData.IdPerson != Guid.Empty 
                && !AuthorizeByPersonInternal())
            {
                OnAccessDeniedNoRightsForPerson();
                isRedundant = false;

                return false;
            }

            isRedundant = IsRedundant;
            return true;
        }

        private ExtendedAccessData AuthorizeByCode(string codeData)
        {
            var codeLength = codeData.Length;

            if (CcuCardReaders.MinimalCodeLength > codeLength
                || CcuCardReaders.MaximalCodeLength < codeLength)
            {
                return null;
            }

            var codeHashValue = QuickHashes.GetCRC32String(codeData);

            if (!codeHashValue.Equals(Gin))
            {
                var idPerson = Database.ConfigObjectsEngine.PersonsStorage.GetPersonId(codeHashValue);

                return idPerson != Guid.Empty
                    ? new PersonalCodeAccessData(idPerson)
                    : null;
            }

            return new ExtendedAccessData();
        }

        protected abstract string Gin
        {
            get;
        }

        protected virtual bool IsRedundant 
        {
            get { return false; }
        }

        protected abstract void OnAccessDeniedInvalidCode();

        protected override void OnReset()
        {
            _accessData = null;
        }

        public override int PinLength
        {
            get { return _accessData.PinLength; }
        }
    }
}
