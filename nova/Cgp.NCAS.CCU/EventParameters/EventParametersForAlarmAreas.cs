using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(410)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSendRequestedActivationState : EventParametersWithObjectIdAndState
    {
        public Guid IdLogin { get; private set; }
        public Guid IdPerson { get; private set; }

        public EventAlarmAreaSendRequestedActivationState(
            Guid guidAlarmArea,
            State requestedState,
            Guid guidLogin,
            Guid guidPerson)
            : base(
                EventType.AlarmAreaRequestActivationState,
                guidAlarmArea,
                requestedState)
        {
            IdLogin = guidLogin;
            IdPerson = guidPerson;
        }

        public EventAlarmAreaSendRequestedActivationState(
            Guid guidAlarmArea,
            State requestedState)
            : this(
                guidAlarmArea,
                requestedState,
                Guid.Empty,
                Guid.Empty)
        {

        }

        public EventAlarmAreaSendRequestedActivationState()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Login: {0}, Person: {1}",
                    IdLogin,
                    IdPerson));
        }
    }

    /// <summary>
    /// Event with this parameters is send to server after alarm state change (example: alarm -> normal)
    /// </summary>
    [LwSerialize(411)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventaAlarmAreaAlarmStateChanged : EventParametersWithObjectIdAndState
    {
        private class EventaAlarmAreaAlarmStateChangedForCardReader :
            AEventForCardReader<EventaAlarmAreaAlarmStateChanged>
        {
            public EventaAlarmAreaAlarmStateChangedForCardReader(
                EventaAlarmAreaAlarmStateChanged eventaAlarmAreaAlarmStateChanged)
                : base(eventaAlarmAreaAlarmStateChanged)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return EventParameters.State; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return null;
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.AlarmAreaIsNotAcknowledged;

                    yield return EventParameters.State == State.Alarm
                        ? CrIconSymbol.AlarmAreaIsInAlarm
                        : CrIconSymbol.AlarmAreaIsNormal;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.AlarmAreaIsSet(EventParameters.IdObject)
                    || !AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject))
                {
                    return null;
                }

                switch (EventParameters.State)
                {
                    case State.Alarm:
                        if (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaAlarm(EventParameters.IdObject))
                            return null;

                        break;
                    case State.Normal:
                        if (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaNormal(EventParameters.IdObject))
                            return null;

                        break;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public EventaAlarmAreaAlarmStateChanged(
            Guid idAlarmArea,
            State newState)
            : base(
                EventType.AlarmAreaAlarmStateChanged,
                idAlarmArea,
                newState)
        {

        }

        public EventaAlarmAreaAlarmStateChanged()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventaAlarmAreaAlarmStateChangedForCardReader(this);
        }
    }

    [LwSerialize(412)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaAlarmStateInfo : EventParametersWithObjectIdAndState
    {
        public EventAlarmAreaAlarmStateInfo(
            Guid idAlarmArea,
            State state)
            : base(
                EventType.AlarmAreaAlarmStateInfo,
                idAlarmArea,
                state)
        {

        }

        public EventAlarmAreaAlarmStateInfo()
        {
            
        }
    }

    [LwSerialize(413)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaTimeBuyingFailed : EventParametersWithObjectId
    {
        private class EventAlarmAreaTimeBuyingFailedForCardReader
            : AEventForCardReader<EventAlarmAreaTimeBuyingFailed>
        {
            public EventAlarmAreaTimeBuyingFailedForCardReader(
                EventAlarmAreaTimeBuyingFailed eventAlarmAreaTimeBuyingFailed)
                : base(eventAlarmAreaTimeBuyingFailed)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return crEventlogProcessor.GetAlarmAreaTimeBuyingFailedInformations(
                    EventParameters.TimeToBuy,
                    EventParameters.RemainingTime);
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.TimeBuying;
                    yield return CrIconSymbol.ActionFailed;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnset(EventParameters.IdObject))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public byte Reason { get; private set; }
        public Guid IdLogin { get; private set; }
        public Guid IdPerson { get; private set; }
        public int TimeToBuy { get; private set; }
        public int RemainingTime { get; private set; }
        public Guid[] Sources { get; private set; }

        public EventAlarmAreaTimeBuyingFailed(
            Guid guidAlarmArea,
            byte reason,
            Guid guidLogin,
            Guid guidPerson,
            int timeToBuy,
            int remainingTime,
            Guid[] sources)
            : base(
                EventType.AlarmAreaTimeBuyingFailed,
                guidAlarmArea)
        {
            Reason = reason;
            IdLogin = guidLogin;
            IdPerson = guidPerson;
            TimeToBuy = timeToBuy;
            RemainingTime = remainingTime;
            Sources = sources;
        }

        public EventAlarmAreaTimeBuyingFailed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Reason: {0}, Login: {1}, Person: {2}, TimeToBy: {3}, RemainingTime: {4}",
                    Reason,
                    IdLogin,
                    IdPerson,
                    TimeToBuy,
                    RemainingTime));

            var sourcesString = GetSourcesString();

            if (!string.IsNullOrEmpty(sourcesString))
            {
                parameters.Append(
                    string.Format(
                        ", Sources: {0}",
                        sourcesString));
            }
        }

        private string GetSourcesString()
        {
            if (Sources == null)
                return null;

            var sourcesEnumerator = Sources.GetEnumerator();

            if (!sourcesEnumerator.MoveNext())
                return null;

            var sourcesString = new StringBuilder(
                sourcesEnumerator.Current.ToString());

            while (sourcesEnumerator.MoveNext())
            {
                sourcesString.Append(
                    string.Format(
                        ", {0}",
                        sourcesEnumerator.Current));
            }

            return sourcesString.ToString();
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventAlarmAreaTimeBuyingFailedForCardReader(this);
        }
    }

    [LwSerialize(414)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaBoughtTimeExpired : EventParametersWithObjectId
    {
        private class EventAlarmAreaBoughtTimeExpiredForCardReader
            : AEventForCardReader<EventAlarmAreaBoughtTimeExpired>
        {
            public EventAlarmAreaBoughtTimeExpiredForCardReader(
                EventAlarmAreaBoughtTimeExpired eventAlarmAreaBoughtTimeExpired)
                : base(eventAlarmAreaBoughtTimeExpired)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return crEventlogProcessor.GetAlarmAreaBoughtTimeExpiredInformations(
                    EventParameters.LastBoughtTime,
                    EventParameters.TotalBoughtTime);
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.TimeBuying;
                    yield return CrIconSymbol.TimeBuyingExpired;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaSet(EventParameters.IdObject)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnconditionalSet(EventParameters.IdObject)))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public int LastBoughtTime { get; private set; }
        public int TotalBoughtTime { get; private set; }

        public EventAlarmAreaBoughtTimeExpired(
            Guid idAlarmArea,
            int lastBoughtTime,
            int totalBoughtTime)
            : base(
                EventType.AlarmAreaBoughtTimeExpired,
                idAlarmArea)
        {
            LastBoughtTime = lastBoughtTime;
            TotalBoughtTime = totalBoughtTime;
        }

        public EventAlarmAreaBoughtTimeExpired()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", LastBoughtTime: {0}, TotalBoughtTime: {1}",
                    LastBoughtTime,
                    TotalBoughtTime));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventAlarmAreaBoughtTimeExpiredForCardReader(this);
        }
    }

    [LwSerialize(415)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaBoughtTimeChanged : EventParametersWithObjectId
    {
        private class EventAlarmAreaBoughtTimeChangedForCardReader
            : AEventForCardReader<EventAlarmAreaBoughtTimeChanged>
        {
            public EventAlarmAreaBoughtTimeChangedForCardReader(
                EventAlarmAreaBoughtTimeChanged eventAlarmAreaBoughtTimeChanged)
                : base(eventAlarmAreaBoughtTimeChanged)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return crEventlogProcessor.GetAlarmAreaBoughtTimeChangedInformations(
                    EventParameters.IdCard,
                    EventParameters.UsedTime,
                    EventParameters.RemainingTime);
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.TimeBuying;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnset(EventParameters.IdObject))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdLogin { get; private set; }
        public Guid IdPerson { get; private set; }
        public int UsedTime { get; private set; }
        public int RemainingTime { get; private set; }

        private EventAlarmAreaBoughtTimeChanged(
            Guid guidAlarmArea,
            Guid guidCardReader,
            Guid guidCard,
            Guid guidLogin,
            Guid guidPerson,
            int usedTime,
            int remainingTime)
            : base(
                EventType.AlarmAreaBoughtTimeChanged,
                guidAlarmArea)
        {
            IdCardReader = guidCardReader;
            IdCard = guidCard;
            IdLogin = guidLogin;
            IdPerson = guidPerson;
            UsedTime = usedTime;
            RemainingTime = remainingTime;
        }

        public EventAlarmAreaBoughtTimeChanged()
        {
            
        }

        /// <summary>
        /// Buy time from card reader
        /// </summary>
        /// <param name="guidAlarmArea"></param>
        /// <param name="guidCardReader"></param>
        /// <param name="guidCard"></param>
        /// <param name="idPerson"></param>
        /// <param name="usedTime"></param>
        /// <param name="remainingTime"></param>
        public static EventAlarmAreaBoughtTimeChanged EventAlarmAreaBoughtTimeChangedFromCardReader(
            Guid guidAlarmArea,
            Guid guidCardReader,
            Guid guidCard,
            Guid idPerson,
            int usedTime,
            int remainingTime)
        {
            return new EventAlarmAreaBoughtTimeChanged(
                guidAlarmArea,
                guidCardReader,
                guidCard,
                Guid.Empty,
                idPerson,
                usedTime,
                remainingTime);
        }

        /// <summary>
        /// Buy time from client
        /// </summary>
        /// <param name="guidAlarmArea"></param>
        /// <param name="usedTime"></param>
        /// <param name="remainingTime"></param>
        /// <param name="guidLogin"></param>
        /// <param name="guidPerson"></param>
        public static EventAlarmAreaBoughtTimeChanged EventAlarmAreaBoughtTimeChangedFromClient(
            Guid guidAlarmArea,
            Guid guidLogin,
            Guid guidPerson,
            int usedTime,
            int remainingTime)
        {
            return new EventAlarmAreaBoughtTimeChanged(
                guidAlarmArea,
                Guid.Empty,
                Guid.Empty,
                guidLogin,
                guidPerson,
                usedTime,
                remainingTime);
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", CardReader: {0}, Card: {1}, Login: {2}, Person: {3}, UsedTime: {4}, RemainingTime: {5}",
                    IdCardReader,
                    IdCard,
                    IdLogin,
                    IdPerson,
                    UsedTime,
                    RemainingTime));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventAlarmAreaBoughtTimeChangedForCardReader(this);
        }
    }

    [LwSerialize(416)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetAlarmAreaFromClient : EventParametersWithObjectId
    {
        private class EventSetAlarmAreaFromClientForCardReader
            : AEventForCardReader<EventSetAlarmAreaFromClient>
        {
            public EventSetAlarmAreaFromClientForCardReader(
                EventSetAlarmAreaFromClient eventSetAlarmAreaFromClient)
                : base(eventSetAlarmAreaFromClient)
            {

            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return State.Set; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return null;
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get { yield return CrIconSymbol.SetAlarmArea; }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaSet(EventParameters.IdObject)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnconditionalSet(EventParameters.IdObject)))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public Guid IdLogin { get; private set; }
        public bool NoPrewarning { get; private set; }

        public EventSetAlarmAreaFromClient(
            Guid idAlarmArea,
            Guid idLogin,
            bool noPrewarning)
            : base(
                EventType.SetAlarmAreaFromClient,
                idAlarmArea)
        {
            IdLogin = idLogin;
            NoPrewarning = noPrewarning;
        }

        public EventSetAlarmAreaFromClient()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Login: {0}, No prewarning: {1}",
                    IdLogin,
                    NoPrewarning));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventSetAlarmAreaFromClientForCardReader(this);
        }
    }

    [LwSerialize(417)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnsetAlarmAreaFromClient : EventParametersWithObjectId
    {
        private class EventUnsetAlarmAreaFromClientForCardReader
            : AEventForCardReader<EventUnsetAlarmAreaFromClient>
        {
            public EventUnsetAlarmAreaFromClientForCardReader(
                EventUnsetAlarmAreaFromClient eventUnsetAlarmAreaFromClient)
                : base(eventUnsetAlarmAreaFromClient)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return State.Unset; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return null;
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get { yield return CrIconSymbol.UnsetAlarmArea; }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (EventParameters.TimeToBuy > 0
                    || EventParameters.TimeToBuy == -1)
                {
                    return null;
                }

                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnset(EventParameters.IdObject))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public Guid IdLogin { get; private set; }
        public int TimeToBuy { get; private set; }

        public EventUnsetAlarmAreaFromClient(
            Guid guidAlarmArea,
            Guid idLogin,
            int timeToBuy)
            : base(
                EventType.UnsetAlarmAreaFromClient,
                guidAlarmArea)
        {
            IdLogin = idLogin;
            TimeToBuy = timeToBuy;
        }

        public EventUnsetAlarmAreaFromClient()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Login: {0}, Time to buy: {1}",
                    IdLogin,
                    TimeToBuy));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventUnsetAlarmAreaFromClientForCardReader(this);
        }
    }

    [LwSerialize(418)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaActivationStateChanged : EventParametersWithObjectIdAndState
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        public EventAlarmAreaActivationStateChanged(
            State activationState,
            Guid idAlarmArea,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson)
            : base(
                EventType.AlarmAreaActivationStateChanged,
                idAlarmArea,
                activationState)
        {
            IdCardReader = idCardReader;
            IdCard = idCard;
            IdPerson = idPerson;
        }

        public EventAlarmAreaActivationStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", IdCardReader: {0}, IdCard: {1}, IdPerson: {2}",
                    IdCardReader,
                    IdCard,
                    IdPerson));
        }
    }

    [LwSerialize(419)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetUnsetAlarmAreaByObjectForAutomaticActivation : EventParametersWithObjectId
    {
        public Guid IdOfObjectForAutomaticActivation { get; private set; }
        public ObjectType TypeOfObjectForAutomaticAtivation { get; private set; }

        protected EventSetUnsetAlarmAreaByObjectForAutomaticActivation(
            EventType eventType,
            Guid idAlarmArea,
            Guid idOfObjectForAutomaticActivation,
            ObjectType typeOfObjectForAutomaticAtivation)
            : base(
                eventType,
                idAlarmArea)
        {
            IdOfObjectForAutomaticActivation = idOfObjectForAutomaticActivation;
            TypeOfObjectForAutomaticAtivation = typeOfObjectForAutomaticAtivation;
        }

        protected EventSetUnsetAlarmAreaByObjectForAutomaticActivation()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Id of object for automatic activation: {0}, Type of object for automatic activation: {1}",
                    IdOfObjectForAutomaticActivation,
                    TypeOfObjectForAutomaticAtivation));
        }
    }

    [LwSerialize(420)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetAlarmAreaByObjectForAutomaticActivation : EventSetUnsetAlarmAreaByObjectForAutomaticActivation
    {
        private class EventSetAlarmAreaByObjectForAutomaticActivationForCardReader :
            AEventForCardReader<EventSetAlarmAreaByObjectForAutomaticActivation>
        {
            public EventSetAlarmAreaByObjectForAutomaticActivationForCardReader(
                EventSetAlarmAreaByObjectForAutomaticActivation eventSetAlarmAreaByObjectForAutomaticActivation)
                : base(eventSetAlarmAreaByObjectForAutomaticActivation)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return State.Set; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new []
                {
                    crEventlogProcessor.GetObjectTypeInformation(
                        EventParameters.TypeOfObjectForAutomaticAtivation)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get { yield return CrIconSymbol.SetAlarmArea; }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaSet(EventParameters.IdObject)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnconditionalSet(EventParameters.IdObject)))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public EventSetAlarmAreaByObjectForAutomaticActivation(
            Guid idAlarmArea,
            Guid idOfObjectForAutomaticActivation,
            ObjectType typeOfObjectForAutomaticAtivation)
            : base(
                EventType.SetAlarmAreaByObjectForAutomaticActivation,
                idAlarmArea,
                idOfObjectForAutomaticActivation,
                typeOfObjectForAutomaticAtivation)
        {

        }

        public EventSetAlarmAreaByObjectForAutomaticActivation()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventSetAlarmAreaByObjectForAutomaticActivationForCardReader(this);
        }
    }

    [LwSerialize(421)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnsetAlarmAreaByObjectForAutomaticActivation : EventSetUnsetAlarmAreaByObjectForAutomaticActivation
    {
        private class EventUnsetAlarmAreaByObjectForAutomaticActivationForCardReader :
            AEventForCardReader<EventUnsetAlarmAreaByObjectForAutomaticActivation>
        {
            public EventUnsetAlarmAreaByObjectForAutomaticActivationForCardReader(
                EventUnsetAlarmAreaByObjectForAutomaticActivation eventUnsetAlarmAreaByObjectForAutomaticActivation)
                : base(eventUnsetAlarmAreaByObjectForAutomaticActivation)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return State.Unset; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new[]
                {
                    crEventlogProcessor.GetObjectTypeInformation(
                        EventParameters.TypeOfObjectForAutomaticAtivation)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get { yield return CrIconSymbol.UnsetAlarmArea; }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnset(EventParameters.IdObject))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public EventUnsetAlarmAreaByObjectForAutomaticActivation(
            Guid idAlarmArea,
            Guid idOfObjectForAutomaticActivation,
            ObjectType typeOfObjectForAutomaticAtivation)
            : base(
                EventType.UnsetAlarmAreaByObjectForAutomaticActivation,
                idAlarmArea,
                idOfObjectForAutomaticActivation,
                typeOfObjectForAutomaticAtivation)
        {

        }

        public EventUnsetAlarmAreaByObjectForAutomaticActivation()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventUnsetAlarmAreaByObjectForAutomaticActivationForCardReader(this);
        }
    }

    [LwSerialize(422)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSabotageStateChanged : EventParametersWithObjectIdAndState
    {
        private class EventAlarmAreaSabotageStateChangedForCardReader :
            AEventForCardReader<EventAlarmAreaSabotageStateChanged>
        {
            public EventAlarmAreaSabotageStateChangedForCardReader(
                EventAlarmAreaSabotageStateChanged eventAlarmAreaSabotageStateChanged)
                : base(eventAlarmAreaSabotageStateChanged)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return EventParameters.State; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return null;
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    switch (EventParameters.State)
                    {
                        case State.Normal:

                            yield return CrIconSymbol.AlarmAreaInSabotage;
                            yield return CrIconSymbol.AlarmAreaIsNormal;

                            break;

                        case State.Alarm:

                            yield return CrIconSymbol.AlarmAreaInSabotage;
                            yield return CrIconSymbol.AlarmAreaIsInAlarm;

                            break;
                    }
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject))
                    return null;

                switch (EventParameters.State)
                {
                    case State.Alarm:
                        if (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaAlarm(EventParameters.IdObject))
                            return null;

                        break;
                    case State.Normal:
                        if (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaNormal(EventParameters.IdObject))
                            return null;

                        break;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public EventAlarmAreaSabotageStateChanged(
            Guid idAlarmArea,
            State sabotageState)
            : base(
                EventType.AlarmAreaSabotageStateChanged,
                idAlarmArea,
                sabotageState)
        {

        }

        public EventAlarmAreaSabotageStateChanged()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventAlarmAreaSabotageStateChangedForCardReader(this);
        }
    }

    [LwSerialize(423)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSabotageStateInfo : EventParametersWithObjectIdAndState
    {
        public EventAlarmAreaSabotageStateInfo(
            Guid idAlarmArea,
            State sabotageState)
            : base(
                EventType.AlarmAreaSabotageStateInfo,
                idAlarmArea,
                sabotageState)
        {

        }

        public EventAlarmAreaSabotageStateInfo()
        {
            
        }
    }

    [LwSerialize(424)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventNotConfirmSetUnsetAaFromEis : EventParametersWithObjectId
    {
        private class EventNotConfirmSetUnsetAaFromEisForCardReader :
            AEventForCardReader<EventNotConfirmSetUnsetAaFromEis>
        {
            public EventNotConfirmSetUnsetAaFromEisForCardReader(
                EventNotConfirmSetUnsetAaFromEis eventNotConfirmSetUnsetAaFromEis)
                : base(eventNotConfirmSetUnsetAaFromEis)
            {

            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return null;
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.UnsetAlarmArea;
                    yield return CrIconSymbol.Waiting;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaSet(EventParameters.IdObject)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnconditionalSet(EventParameters.IdObject)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnset(EventParameters.IdObject)))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public EventNotConfirmSetUnsetAaFromEis(
            Guid idAlarmArea)
            : base(
                EventType.NotConfirmSetUnsetAAFromEIS,
                idAlarmArea)
        {

        }

        public EventNotConfirmSetUnsetAaFromEis()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventNotConfirmSetUnsetAaFromEisForCardReader(this);
        }
    }

    [LwSerialize(671)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaAlarmStateChangedAlarm : EventParametersWithObjectIdAndState
    {
        private class EventAlarmAreaAlarmStateChangedAlarmForCardReader :
            AEventForCardReader<EventAlarmAreaAlarmStateChangedAlarm>
        {
            public EventAlarmAreaAlarmStateChangedAlarmForCardReader(
                EventAlarmAreaAlarmStateChangedAlarm eventAlarmAreaAlarmStateChangedAlarm)
                : base(eventAlarmAreaAlarmStateChangedAlarm)
            {

            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdObject);
            }

            public override State? EventState
            {
                get { return EventParameters.State; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new []
                {
                    crEventlogProcessor.GetCardInformation(
                        EventParameters.IdCard)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    switch (EventParameters.State)
                    {
                        case State.Acknowledge:

                            yield return CrIconSymbol.AlarmAreaIsNotAcknowledged;
                            yield return CrIconSymbol.AcknowledgeAlarmArea;

                            break;

                        case State.AcknowledgeAndBlock:

                            yield return CrIconSymbol.AlarmAreaIsNotAcknowledged;
                            yield return CrIconSymbol.AcknowledgeAlarmArea;
                            yield return CrIconSymbol.PermanentlyBlockSensorAlarm;

                            break;

                        case State.Block:

                            yield return CrIconSymbol.AlarmAreaIsNotAcknowledged;
                            yield return CrIconSymbol.PermanentlyBlockSensorAlarm;

                            break;
                    }
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdObject)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaAcknowledged(EventParameters.IdObject))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdObject,
                    1);
            }
        }

        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        public EventAlarmAreaAlarmStateChangedAlarm(
            Guid idAlarmArea,
            State newState,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson)
            : base(
                EventType.AlarmAreaAlarmStateChangeAlarm,
                idAlarmArea,
                newState)
        {
            IdCardReader = idCardReader;
            IdCard = idCard;
            IdPerson = idPerson;
        }

        public EventAlarmAreaAlarmStateChangedAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card reader: {0}, Card: {1}, Person: {2}",
                    IdCardReader,
                    IdCard,
                    IdPerson));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventAlarmAreaAlarmStateChangedAlarmForCardReader(this);
        }
    }

    [LwSerialize(425)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorBlockingTypeChanged : EventParametersWithObjectId
    {
        private class EventSensorBlockingTypeChangedForCardReader : ASensorEventForCardReader<EventSensorBlockingTypeChanged>
        {
            public EventSensorBlockingTypeChangedForCardReader(EventSensorBlockingTypeChanged eventSensorBlockingTypeChanged)
                : base(eventSensorBlockingTypeChanged)
            {

            }

            protected override DB.AlarmArea GetAlarmArea()
            {
                return Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.AlarmArea,
                    EventParameters.IdAlarmArea) as DB.AlarmArea;
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new[]
                {
                    crEventlogProcessor.GetCardInformation(EventParameters.IdCard)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    switch (EventParameters.SensorBlockingType)
                    {
                        case SensorBlockingType.Unblocked:

                            yield return CrIconSymbol.SensorsPermanentlyBlocked;
                            yield return CrIconSymbol.UnblockSensor;

                            break;

                        case SensorBlockingType.BlockPermanently:

                            yield return CrIconSymbol.SensorsPermanentlyBlocked;

                            break;

                        case SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset:
                        case SensorBlockingType.BlockTemporarilyUntilSensorStateNormal:

                            yield return CrIconSymbol.SensorsTemporarilyBlocked;

                            break;
                    }
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdAlarmArea))
                    return null;

                return Enumerable.Repeat(
                    EventParameters.IdAlarmArea,
                    1);
            }
        }
        public Guid IdAlarmArea { get; private set; }
        public SensorBlockingType SensorBlockingType { get; private set; }
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        public EventSensorBlockingTypeChanged()
        {

        }

        public EventSensorBlockingTypeChanged(
            Guid idInput,
            Guid idAlarmArea,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            SensorBlockingType sensorBlockingType)
            : base(
                EventType.AlarmAreaSensorBlockingTypeChanged,
                idInput)
        {
            IdAlarmArea = idAlarmArea;
            SensorBlockingType = sensorBlockingType;
            IdCardReader = idCardReader;
            IdCard = idCard;
            IdPerson = idPerson;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}, SensorBlockingType: {1}, CardReader: {2}, Card: {3}, Person: {4}",
                    IdAlarmArea,
                    SensorBlockingType.ToString(),
                    IdCardReader,
                    IdCard,
                    IdPerson));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventSensorBlockingTypeChangedForCardReader(this);
        }
    }

    [LwSerialize(426)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorBlockingTypeInfo : EventParametersWithObjectId
    {
        public Guid IdAlarmArea { get; private set; }
        public SensorBlockingType SensorBlockingType { get; private set; }

        public EventSensorBlockingTypeInfo()
        {

        }

        public EventSensorBlockingTypeInfo(
            Guid idInput,
            Guid idAlarmArea,
            SensorBlockingType sensorBlockingType)
            : base(
                EventType.AlarmAreaSensorBlockingTypeInfo,
                idInput)
        {
            IdAlarmArea = idAlarmArea;
            SensorBlockingType = sensorBlockingType;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}, SensorBlockingType: {1}",
                    IdAlarmArea,
                    SensorBlockingType.ToString()));
        }
    }

    [LwSerialize(427)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorStateChanged : EventParametersWithObjectIdAndState
    {
        public Guid IdAlarmArea { get; private set; }

        public EventSensorStateChanged()
        {

        }

        public EventSensorStateChanged(
            Guid idInput,
            Guid idAlarmArea,
            State sensorState)
            : base(
                EventType.AlarmAreaSensorStateChanged,
                idInput,
                sensorState)
        {
            IdAlarmArea = idAlarmArea;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}",
                    IdAlarmArea));
        }
    }

    [LwSerialize(428)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorStateInfo : EventParametersWithObjectIdAndState
    {
        public Guid IdAlarmArea { get; private set; }

        public EventSensorStateInfo()
        {

        }

        public EventSensorStateInfo(
            Guid idInput,
            Guid idAlarmArea,
            State sensorState)
            : base(
                EventType.AlarmAreaSensorStateInfo,
                idInput,
                sensorState)
        {
            IdAlarmArea = idAlarmArea;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}",
                    IdAlarmArea));
        }
    }

    [LwSerialize(429)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorAlarmAknowledged : EventParametersWithObjectId
    {
        private class EventSensorAlarmAknowledgedForCardReader : ASensorEventForCardReader<EventSensorAlarmAknowledged>
        {
            public EventSensorAlarmAknowledgedForCardReader(EventSensorAlarmAknowledged eventSensorAlarmAknowledged)
                : base(eventSensorAlarmAknowledged)
            {

            }

            protected override DB.AlarmArea GetAlarmArea()
            {
                return Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.AlarmArea,
                    EventParameters.IdAlarmArea) as DB.AlarmArea;
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new[]
                {
                    crEventlogProcessor.GetCardInformation(EventParameters.IdCard)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.SensorsInAlarm;
                    yield return CrIconSymbol.AcknowledgeSensorAlarm;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdAlarmArea)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsSensorsAcknowledged(EventParameters.IdAlarmArea))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdAlarmArea,
                    1);
            }
        }
        public Guid IdAlarmArea { get; private set; }
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        public EventSensorAlarmAknowledged()
        {

        }

        public EventSensorAlarmAknowledged(
            Guid idInput,
            Guid idAlarmArea,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson)
            : base(
                EventType.AlarmAreaSensorAlarmAcknowledged,
                idInput)
        {
            IdAlarmArea = idAlarmArea;
            IdCardReader = idCardReader;
            IdCard = idCard;
            IdPerson = idPerson;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}, CardReader: {1}, Card: {2}, Person: {3}",
                    IdAlarmArea,
                    IdCardReader,
                    IdCard,
                    IdPerson));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventSensorAlarmAknowledgedForCardReader(this);
        }
    }

    [LwSerialize(670)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorStateChangedOnlyForCardReader : EventParametersWithObjectIdAndState
    {
        private class EventSensorStateChangedForCardReader : ASensorEventForCardReader<EventSensorStateChangedOnlyForCardReader>
        {
            public EventSensorStateChangedForCardReader(EventSensorStateChangedOnlyForCardReader eventSensorStateChanged)
                : base(eventSensorStateChanged)
            {

            }

            protected override DB.AlarmArea GetAlarmArea()
            {
                return Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.AlarmArea,
                    EventParameters.IdAlarmArea) as DB.AlarmArea;
            }

            public override State? EventState
            {
                get { return EventParameters.State; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return null;
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    switch (EventParameters.State)
                    {
                        case State.Alarm:

                            yield return CrIconSymbol.SensorsInAlarm;

                            break;

                        case State.Normal:

                            yield return CrIconSymbol.SensorsNormal;

                            break;

                        case State.Short:
                        case State.Break:

                            yield return CrIconSymbol.ShowSensors;
                            yield return CrIconSymbol.SensorsInSabotage;

                            break;
                    }
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdAlarmArea))
                    return null;

                switch (EventParameters.State)
                {
                    case State.Alarm:
                    case State.Short:
                    case State.Break:

                        if (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaAlarm(EventParameters.IdAlarmArea))
                            return null;

                        break;

                    case State.Normal:

                        if (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaNormal(EventParameters.IdAlarmArea))
                            return null;

                        break;
                }

                return Enumerable.Repeat(
                    EventParameters.IdAlarmArea,
                    1);
            }
        }
        public Guid IdAlarmArea { get; private set; }

        public EventSensorStateChangedOnlyForCardReader()
        {

        }

        public EventSensorStateChangedOnlyForCardReader(
            Guid idInput,
            Guid idAlarmArea,
            State sensorState)
            : base(
                EventType.AlarmAreaSensorStateChangedOnlyForCardReader,
                idInput,
                sensorState)
        {
            IdAlarmArea = idAlarmArea;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}",
                    IdAlarmArea));
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventSensorStateChangedForCardReader(this);
        }
    }

    [LwSerialize(672)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventTimeBuyingMatrixStateInfo : EventParametersWithObjectId
    {
        public TimeBuyingMatrixState TimeBuyingMatrixState { get; private set; }

        public EventTimeBuyingMatrixStateInfo()
        {

        }

        public EventTimeBuyingMatrixStateInfo(
            Guid idAlarmArea,
            TimeBuyingMatrixState timeBuyingMatrixState)
            : base(
                EventType.TimeBuingMatrixStateChangedInfo,
                idAlarmArea)
        {
            TimeBuyingMatrixState = timeBuyingMatrixState;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", TimeBuyingMatrixState: {0}",
                    TimeBuyingMatrixState));
        }
    }

    public static class TestAlarmAreaEvents
    {
        public static void EnqueueTestEvents(
            Guid idAlarmArea,
            Guid idCardReader,
            ICard card,
            Guid idInput)
        {
            Events.ProcessEvent(
                new EventAlarmAreaSendRequestedActivationState(
                    idAlarmArea,
                    State.Set));

            Events.ProcessEvent(
                new EventaAlarmAreaAlarmStateChanged(
                    idAlarmArea,
                    State.Alarm));

            Events.ProcessEvent(
                new EventAlarmAreaAlarmStateInfo(
                    idAlarmArea,
                    State.Normal));

            Events.ProcessEvent(
                new EventAlarmAreaTimeBuyingFailed(
                    idAlarmArea,
                    (byte) AlarmAreaActionResult.FailedTimeBuyingNotEnabled,
                    Guid.Empty,
                    Guid.Empty,
                    10,
                    5,
                    new[]
                    {
                        idCardReader,
                        card.IdCard
                    }));

            Events.ProcessEvent(
                new EventAlarmAreaBoughtTimeExpired(
                    idAlarmArea,
                    10,
                    20));

            Events.ProcessEvent(
                EventAlarmAreaBoughtTimeChanged.EventAlarmAreaBoughtTimeChangedFromCardReader(
                    idAlarmArea,
                    idCardReader,
                    card.IdCard,
                    card.GuidPerson,
                    10,
                    20));

            Events.ProcessEvent(
                new EventAlarmAreaActivationStateChanged(
                    State.Set,
                    idAlarmArea,
                    idCardReader,
                    card.IdCard,
                    card.GuidPerson));

            Events.ProcessEvent(
                new EventSetAlarmAreaByObjectForAutomaticActivation(
                    idAlarmArea,
                    idInput,
                    ObjectType.Input));

            Events.ProcessEvent(
                new EventUnsetAlarmAreaByObjectForAutomaticActivation(
                    idAlarmArea,
                    idInput,
                    ObjectType.Input));

            Events.ProcessEvent(
                new EventAlarmAreaSabotageStateChanged(
                    idAlarmArea,
                    State.Alarm));

            Events.ProcessEvent(
                new EventAlarmAreaSabotageStateInfo(
                    idAlarmArea,
                    State.Normal));

            Events.ProcessEvent(
                new EventNotConfirmSetUnsetAaFromEis(idAlarmArea));

            Events.ProcessEvent(
                new EventAlarmAreaAlarmStateChangedAlarm(
                    idAlarmArea,
                    State.Acknowledge,
                    idCardReader,
                    card.IdCard,
                    card.GuidPerson));

            Events.ProcessEvent(
                new EventSensorBlockingTypeChanged(
                    idInput,
                    idAlarmArea,
                    idCardReader,
                    card.IdCard,
                    card.GuidPerson,
                    SensorBlockingType.BlockPermanently));

            Events.ProcessEvent(
                new EventSensorBlockingTypeInfo(
                    idInput,
                    idAlarmArea,
                    SensorBlockingType.Unblocked));

            Events.ProcessEvent(
                new EventSensorStateChanged(
                    idInput,
                    idAlarmArea,
                    State.Alarm));

            Events.ProcessEvent(
                new EventSensorStateInfo(
                    idInput,
                    idAlarmArea,
                    State.Normal));

            Events.ProcessEvent(
                new EventSensorAlarmAknowledged(
                    idInput,
                    idAlarmArea,
                    idCardReader,
                    card.IdCard,
                    card.GuidPerson));

            Events.ProcessEvent(
                new EventTimeBuyingMatrixStateInfo(
                    idInput,
                    TimeBuyingMatrixState.O4AA_ON_AND_O4TBA_OFF));
        }
    }
}
