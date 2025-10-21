using System;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(490)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmStateChanged : EventParametersWithObjectIdAndState
    {
        public EventDsmStateChanged(
            State doorEnvironmentState,
            Guid idDoorEnvironment)
            : base(
                EventType.DSMStateChanged,
                idDoorEnvironment,
                doorEnvironmentState)
        {
            
        }

        public EventDsmStateChanged()
        {
            
        }
    }

    [LwSerialize(491)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForDsmAccess : EventParametersWithObjectId
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }
        public Guid IdPushButton { get; private set; }

        protected EventForDsmAccess(
            EventType eventType,
            Guid idDoorEnvironment,
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                eventType,
                idDoorEnvironment)
        {
            IdCardReader = idCardReader;

            if (accessData == null) return;
            IdCard = accessData.IdCard;
            IdPerson = accessData.IdPerson;
            IdPushButton = accessData.IdPushButton;
        }

        protected EventForDsmAccess()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card reader: {0}, Card: {1}, Person: {2}, Pushbutton: {3}",
                    IdCardReader,
                    IdCard,
                    IdPerson,
                    IdPushButton));
        }
    }

    [LwSerialize(492)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessPermitted : EventForDsmAccess
    {
        public EventDsmAccessPermitted(
            Guid idDoorEnvironment,
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                EventType.DSMAccessPermitted,
                idDoorEnvironment,
                idCardReader,
                accessData)
        {
        }

        public EventDsmAccessPermitted()
        {
        }
    }

    [LwSerialize(493)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmNormalAccess : EventForDsmAccess
    {
        public EventDsmNormalAccess(
            Guid idDoorEnvironment,
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                EventType.DSMNormalAccess,
                idDoorEnvironment,
                idCardReader,
                accessData)
        {
        }

        public EventDsmNormalAccess()
        {
        }
    }

    [LwSerialize(494)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmApasRestored : EventForDsmAccess
    {
        public EventDsmApasRestored(
            Guid idDoorEnvironment,
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                EventType.DSMApasRestored,
                idDoorEnvironment,
                idCardReader,
                accessData)
        {
        }

        public EventDsmApasRestored()
        {
        }
    }

    [LwSerialize(495)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessInterupted : EventForDsmAccess
    {
        public EventDsmAccessInterupted(
            Guid idDoorEnvironment,
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                EventType.DSMAccessInterupted,
                idDoorEnvironment,
                idCardReader,
                accessData)
        {
        }

        public EventDsmAccessInterupted()
        {
        }
    }

    [LwSerialize(496)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessViolated : EventParametersWithObjectId
    {
        public string Reason { get; private set; }

        public EventDsmAccessViolated(
            Guid idDoorEnvironment,
            string reason)
            : base(
                EventType.DSMAccessViolated,
                idDoorEnvironment)
        {
            Reason = reason;
        }

        public EventDsmAccessViolated()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Reason: {0}",
                    Reason));
        }
    }

    [LwSerialize(497)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessRestricted : EventParametersWithObjectId
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public string CardNumber { get; private set; }
        public string Message { get; private set; }

        public EventDsmAccessRestricted(
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            string cardNumber,
            string message)
            : base(
                EventType.DSMAccessRestricted,
                idDoorEnvironment)
        {
            IdCardReader = idCardReader;
            IdCard = idCard;
            CardNumber = cardNumber;
            Message = message;
        }

        public EventDsmAccessRestricted()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card reader: {0}, Card: {1}, Card number: {2}, Message: {3}",
                    IdCardReader,
                    IdCard,
                    CardNumber,
                    Message));
        }
    }

    public static class TestDoorEnvironmentEvents
    {
        public class CardAccessData : AccessDataBase
        {
            private readonly ICard _card;

            public override Guid IdCard
            {
                get { return _card.IdCard; }
            }

            public override Guid IdPerson { get { return _card.GuidPerson; } }

            public override bool EntryViaCard { get { return true; } }

            public override AccessGrantedSource AccessGrantedSource
            {
                get { return AccessGrantedSource.Card; }
            }

            public CardAccessData(ICard card)
            {
                _card = card;
            }
        }

        public static void EnqueueTestEvents(
            Guid idDoorEnvironment,
            Guid idCardReader,
            ICard card)
        {
            Events.ProcessEvent(new EventDsmStateChanged(
                State.opened,
                idDoorEnvironment));

            Events.ProcessEvent(new EventDsmAccessPermitted(
                idDoorEnvironment,
                idCardReader,
                new CardAccessData(card)));

            Events.ProcessEvent(new EventDsmNormalAccess(
                idDoorEnvironment,
                idCardReader,
                new CardAccessData(card)));

            Events.ProcessEvent(new EventDsmApasRestored(
                idDoorEnvironment,
                idCardReader,
                new CardAccessData(card)));

            Events.ProcessEvent(new EventDsmAccessInterupted(
                idDoorEnvironment,
                idCardReader,
                new CardAccessData(card)));

            Events.ProcessEvent(new EventDsmAccessViolated(
                idDoorEnvironment,
                "Test"));

            Events.ProcessEvent(new EventDsmAccessRestricted(
                idDoorEnvironment,
                idCardReader,
                card.IdCard,
                "123456789012",
                "Test"));
        }
    }
}
