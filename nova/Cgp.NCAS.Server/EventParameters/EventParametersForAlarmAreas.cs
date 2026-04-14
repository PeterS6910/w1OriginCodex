using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using Contal.Cgp.NCAS.Server.DB;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(410)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSendRequestedActivationState : EventParametersWithObjectIdAndState
    {
        public Guid IdLogin { get; private set; }
        public Guid IdPerson { get; private set; }

        public EventAlarmAreaSendRequestedActivationState(
            UInt64 eventId,
            Guid guidAlarmArea,
            State requestedState,
            Guid guidLogin,
            Guid guidPerson)
            : base(
                eventId,
                EventType.AlarmAreaRequestActivationState,
                guidAlarmArea,
                requestedState)
        {
            IdLogin = guidLogin;
            IdPerson = guidPerson;
        }

        public EventAlarmAreaSendRequestedActivationState(
            UInt64 eventId,
            Guid guidAlarmArea,
            State requestedState)
            : this(
                eventId,
                guidAlarmArea,
                requestedState,
                Guid.Empty,
                Guid.Empty)
        {

        }

        protected EventAlarmAreaSendRequestedActivationState()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            RequestActivationState requestActivationState;

            switch (State)
            {
                case State.Set:
                    requestActivationState = RequestActivationState.Set;
                    break;

                case State.Unset:
                    requestActivationState = RequestActivationState.Unset;
                    break;

                case State.UnconditionaSet:
                    requestActivationState = RequestActivationState.UnconditionalSet;
                    break;

                default:
                    requestActivationState = RequestActivationState.Unknown;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaRequestActivationStateChanged(
                IdObject,
                DateTime,
                requestActivationState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(411)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventaAlarmAreaAlarmStateChanged : EventParametersWithObjectIdAndState
    {
        public EventaAlarmAreaAlarmStateChanged(
            UInt64 eventId,
            Guid idAlarmArea,
            State newState)
            : base(
                eventId,
                EventType.AlarmAreaAlarmStateChanged,
                idAlarmArea,
                newState)
        {

        }

        protected EventaAlarmAreaAlarmStateChanged()
        {

        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var alarmAreaAlarmState = AlarmAreaAlarmState.Unknown;

            switch (State)
            {
                case State.Alarm:
                    alarmAreaAlarmState = AlarmAreaAlarmState.Alarm;
                    break;

                case State.Normal:
                    alarmAreaAlarmState = AlarmAreaAlarmState.Normal;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaAlarmStateChanged(
                IdObject,
                DateTime,
                alarmAreaAlarmState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            if (!GeneralOptions.Singleton.EventlogAlarmAreaAlarmStateChanged)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var description =
                string.Format(
                    "Alarm area {0} changed its alarm state to {1}",
                    alarmArea.ToString(),
                    State);

            eventlogParameters = null;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_ALARM_STATE_CHANGED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    new[] { alarmArea.IdAlarmArea },
                    description,
                    out eventlog,
                    out eventSources);
        }
    }

    [LwSerialize(412)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaAlarmStateInfo : EventParametersWithObjectIdAndState
    {
        public EventAlarmAreaAlarmStateInfo(
            UInt64 eventId,
            Guid idAlarmArea,
            State state)
            : base(
                eventId,
                EventType.AlarmAreaAlarmStateInfo,
                idAlarmArea,
                state)
        {

        }

        protected EventAlarmAreaAlarmStateInfo()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var alarmAreaAlarmState = AlarmAreaAlarmState.Unknown;

            switch (State)
            {
                case State.Alarm:
                    alarmAreaAlarmState = AlarmAreaAlarmState.Alarm;
                    break;

                case State.Normal:
                    alarmAreaAlarmState = AlarmAreaAlarmState.Normal;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaAlarmStateChanged(
                IdObject,
                DateTime,
                alarmAreaAlarmState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(413)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaTimeBuyingFailed : EventParametersWithObjectId
    {
        public byte Reason { get; private set; }
        public Guid IdLogin { get; private set; }
        public Guid IdPerson { get; private set; }
        public int TimeToBuy { get; private set; }
        public int RemainingTime { get; private set; }
        public Guid[] Sources { get; private set; }

        public EventAlarmAreaTimeBuyingFailed(
            UInt64 eventId,
            Guid guidAlarmArea,
            byte reason,
            Guid guidLogin,
            Guid guidPerson,
            int timeToBuy,
            int remainingTime,
            Guid[] sources)
            : base(
                eventId,
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

        protected EventAlarmAreaTimeBuyingFailed()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaTimeBuyingFailed(
                IdObject,
                IdLogin,
                Reason,
                TimeToBuy,
                RemainingTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var person = "UNKNOW";
            var login = "UNKNOW";

            ICollection<Guid> eventSourcesIds = new HashSet<Guid>();

            eventSourcesIds.Add(ccu.IdCCU);

            eventSourcesIds.Add(IdObject);

            // Get person name from login
            if (IdLogin != Guid.Empty)
            {
                eventSourcesIds.Add(IdLogin);

                var tempLogin = Logins.Singleton.GetById(IdLogin);
                if (tempLogin != null)
                {
                    login = tempLogin.ToString();

                    if (tempLogin.Person != null)
                    {
                        if (!eventSourcesIds.Contains(tempLogin.Person.IdPerson))
                            eventSourcesIds.Add(tempLogin.Person.IdPerson);

                        person = tempLogin.Person.ToString();
                    }
                }
            }

            // Get person name from card
            if (IdPerson != Guid.Empty)
            {
                if (!eventSourcesIds.Contains(IdPerson))
                    eventSourcesIds.Add(IdPerson);

                var tempPerson = Persons.Singleton.GetById(IdPerson);

                if (tempPerson != null)
                    person = tempPerson.ToString();
            }

            Func<int, string> timeConvert = time =>
            {
                if (time == int.MinValue)
                    return "UNLIMITED";

                if (time == int.MaxValue)
                    return "UNKNOW";

                int hour = time / 3600;
                int minute = time % 3600;
                int second = minute % 60;
                minute /= 60;

                return String.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hour,
                    minute,
                    second);
            };

            var aaaResult = (AlarmAreaActionResult)Reason;
            var timeToBuy = TimeSpan.FromSeconds(TimeToBuy);

            string remainingTime;
            string remainingOrMissing;
            if (RemainingTime >= 0)
            {
                remainingTime = timeConvert(RemainingTime);
                remainingOrMissing = "remaining";
            }
            else
            {
                remainingTime = timeConvert(RemainingTime * -1);
                remainingOrMissing = "missing";
            }

            var description = string.Format(
                "Alarm area time buying failed because of \"{0}\". Alarm area: {1}. Invoked by login: {2}, person: {3}, time to buy: {4}, {5} time: {6}",
                aaaResult,
                alarmArea,
                login,
                person,
                timeToBuy,
                remainingOrMissing,
                remainingTime);

            // Add sources from ccu
            foreach (var source in Sources)
                if (!eventSourcesIds.Contains(source))
                    eventSourcesIds.Add(source);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_TIME_BUYING_FAILED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    description,
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }

    [LwSerialize(414)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaBoughtTimeExpired : EventParametersWithObjectId
    {
        public int LastBoughtTime { get; private set; }
        public int TotalBoughtTime { get; private set; }

        public EventAlarmAreaBoughtTimeExpired(
            UInt64 eventId,
            Guid idAlarmArea,
            int lastBoughtTime,
            int totalBoughtTime)
            : base(
                eventId,
                EventType.AlarmAreaBoughtTimeExpired,
                idAlarmArea)
        {
            LastBoughtTime = lastBoughtTime;
            TotalBoughtTime = totalBoughtTime;
        }

        protected EventAlarmAreaBoughtTimeExpired()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaBoughtTimeExpired,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                typeof (AlarmAreaTimeBuyingHandler),
                IdObject, //Guid alarm area
                LastBoughtTime, //int used time
                TotalBoughtTime); //int total bought time
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            ICollection<Guid> eventSourcesIds = new LinkedList<Guid>();

            eventSourcesIds.Add(ccu.IdCCU);
            eventSourcesIds.Add(alarmArea.IdAlarmArea);

            Func<int, string> timeConvert = time =>
            {
                if (time == int.MinValue)
                    return "UNLIMITED";

                if (time == int.MaxValue)
                    return "UNKNOW";

                int hour = time / 3600;
                int minute = time % 3600;
                int second = minute % 60;
                minute /= 60;

                return String.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hour,
                    minute,
                    second);
            };

            var boughtTime = timeConvert(LastBoughtTime);
            var totalBoughtTime = timeConvert(TotalBoughtTime);

            var description = string.Format("Bought time for alarm area \"{0}\" expired. Bought time: {1}. Total bought time: {2}",
                alarmArea,
                boughtTime,
                totalBoughtTime);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_BOUGHT_TIME_EXPIRED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    description,
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }

    [LwSerialize(415)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaBoughtTimeChanged : EventParametersWithObjectId
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdLogin { get; private set; }
        public Guid IdPerson { get; private set; }
        public int UsedTime { get; private set; }
        public int RemainingTime { get; private set; }

        private EventAlarmAreaBoughtTimeChanged(
            UInt64 eventId,
            Guid guidAlarmArea,
            Guid guidCardReader,
            Guid guidCard,
            Guid guidLogin,
            Guid guidPerson,
            int usedTime,
            int remainingTime)
            : base(
                eventId,
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

        protected EventAlarmAreaBoughtTimeChanged()
        {
            
        }

        /// <summary>
        /// Buy time from card reader
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="guidAlarmArea"></param>
        /// <param name="guidCardReader"></param>
        /// <param name="guidCard"></param>
        /// <param name="usedTime"></param>
        /// <param name="remainingTime"></param>
        public static EventAlarmAreaBoughtTimeChanged EventAlarmAreaBoughtTimeChangedFromCardReader(
            UInt64 eventId,
            Guid guidAlarmArea,
            Guid guidCardReader,
            Guid guidCard,
            int usedTime,
            int remainingTime)
        {
            return new EventAlarmAreaBoughtTimeChanged(
                eventId,
                guidAlarmArea,
                guidCardReader,
                guidCard,
                Guid.Empty,
                Guid.Empty,
                usedTime,
                remainingTime);
        }

        /// <summary>
        /// Buy time from client
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="guidAlarmArea"></param>
        /// <param name="usedTime"></param>
        /// <param name="remainingTime"></param>
        /// <param name="guidLogin"></param>
        /// <param name="guidPerson"></param>
        public static EventAlarmAreaBoughtTimeChanged EventAlarmAreaBoughtTimeChangedFromClient(
            UInt64 eventId,
            Guid guidAlarmArea,
            Guid guidLogin,
            Guid guidPerson,
            int usedTime,
            int remainingTime)
        {
            return new EventAlarmAreaBoughtTimeChanged(
                eventId,
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var stringLogin = string.Empty;

            if (IdLogin != Guid.Empty)
            {
                var temp = Logins.Singleton.GetById(IdLogin);
                if (temp != null)
                    stringLogin = temp.Username;
            }

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaBoughtTimeChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                typeof (AlarmAreaTimeBuyingHandler),
                IdObject, //Guid alarm area
                stringLogin, //string idLogin
                UsedTime, //int used   
                RemainingTime); //int remaining
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            Func<int, string> timeConvert = time =>
            {
                if (time == int.MinValue)
                    return "UNLIMITED";

                if (time == int.MaxValue)
                    return "UNKNOW";

                int hour = time / 3600;
                int minute = time % 3600;
                int second = minute % 60;
                minute /= 60;

                return String.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hour,
                    minute,
                    second);
            };

            string description;
            ICollection<Guid> eventSourcesIds;
            if (IdCardReader != Guid.Empty)
            {
                //This event is from card reader
                var card = Cards.Singleton.GetById(IdCard);
                var cardReader = CardReaders.Singleton.GetById(IdCardReader);
                var person = Persons.Singleton.GetById(IdPerson);

                string personName = "UNKNOW";

                if (cardReader == null)
                {
                    // Card reader is unknow so add only alarm area and card to sources
                    eventSourcesIds = new LinkedList<Guid>(
                        Enumerable.Repeat(
                            alarmArea.IdAlarmArea,
                            1));

                    if (card != null)
                    {
                        eventSourcesIds.Add(card.IdCard);
                        if (card.Person != null)
                        {
                            eventSourcesIds.Add(card.Person.IdPerson);
                            personName = card.Person.ToString();
                        }
                    }
                    else if (person != null)
                    {
                        eventSourcesIds.Add(person.IdPerson);
                        personName = person.ToString();
                    }
                }
                else
                {
                    // If card reader is not null create sources
                    if (person != null)
                    {
                        eventSourcesIds = new LinkedList<Guid>(
                            CcuEvents.GetEventSourcesFromCardReader(
                                cardReader,
                                person,
                                alarmArea));

                        personName = person.ToString();
                    }
                    else
                        eventSourcesIds = new LinkedList<Guid>(
                            CcuEvents.GetEventSourcesFromCardReader(
                                cardReader,
                                card,
                                alarmArea,
                                out personName));
                }

                var boughtTime = timeConvert(UsedTime);
                var remainingTime = timeConvert(RemainingTime);

                if (card == null
                    && person == null)
                {
                    description = string.Format(
                        "Alarm area was unset by CODE with time buying from card reader: {0}, alarm area: {1}, time bought: {2}, remaining time: {3}",
                        cardReader != null
                            ? cardReader.Name
                            : "UNKNOW",
                        alarmArea,
                        boughtTime,
                        remainingTime
                    );
                }
                else if (card != null)
                {
                    description = string.Format(
                        "Alarm area was unset by time buying from card reader: {0}, card: {1}, person: {2}, alarm area: {3}, time bought: {4}, remaining time: {5}",
                        cardReader != null
                            ? cardReader.Name
                            : "UNKNOW",
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea,
                        boughtTime,
                        remainingTime
                    );
                }
                else
                {
                    description = string.Format(
                        "Alarm area was unset by time buying from card reader: {0}, person: {1}, alarm area: {2}, time bought: {3}, remaining time: {4}",
                        cardReader != null
                            ? cardReader.Name
                            : "UNKNOW",
                        personName,
                        alarmArea,
                        boughtTime,
                        remainingTime
                    );
                }

            }
            else
            {
                //This event is from client
                var idLogin = IdLogin;

                if (idLogin == Guid.Empty)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                var login = Logins.Singleton.GetById(idLogin);
                if (login == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                eventSourcesIds = new LinkedList<Guid>(
                    Enumerable.Repeat(
                        ccu.IdCCU,
                        1)
                        .Concat(
                            Enumerable.Repeat(
                                alarmArea.IdAlarmArea,
                                1))
                        .Concat(
                            Enumerable.Repeat(
                                idLogin,
                                1))
                    );

                var timeToBuy = timeConvert(UsedTime);
                var remainingTime = timeConvert(RemainingTime);

                if (login.Person != null)
                {
                    //this login has person
                    eventSourcesIds.Add(login.Person.IdPerson);
                    description = string.Format(
                        "Alarm area was unset by time buying from client by person {0}, alarm area: {1}, time bought: {2}, remaining time: {3}",
                        login.Person.WholeName,
                        alarmArea,
                        timeToBuy,
                        remainingTime
                    );
                }
                else
                {
                    //this login does not have person
                    description = string.Format(
                        "Alarm area was unset by time buying from client by login {0}, alarm area: {1}, time bought: {2}, remaining time: {3}",
                        login,
                        alarmArea,
                        timeToBuy,
                        remainingTime
                    );
                }
            }

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_BOUGHT_TIME_CHANGED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    description,
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }

    [LwSerialize(416)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetAlarmAreaFromClient : EventParametersWithObjectId
    {
        public Guid IdLogin { get; private set; }
        public bool NoPrewarning { get; private set; }

        public EventSetAlarmAreaFromClient(
            UInt64 eventId,
            Guid idAlarmArea,
            Guid idLogin,
            bool noPrewarning)
            : base(
                eventId,
                EventType.SetAlarmAreaFromClient,
                idAlarmArea)
        {
            IdLogin = idLogin;
            NoPrewarning = noPrewarning;
        }

        protected EventSetAlarmAreaFromClient()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var login = Logins.Singleton.GetById(IdLogin);
            if (login == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            ICollection<Guid> eventSourcesIds = new LinkedList<Guid>(
                Enumerable.Repeat(
                    alarmArea.IdAlarmArea,
                    1)
                    .Concat(
                        Enumerable.Repeat(
                            login.IdLogin,
                            1)));

            if (login.Person != null)
                eventSourcesIds.Add(login.Person.IdPerson);

            string info = NoPrewarning
                ? " with no prewarning"
                : string.Empty;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_SET_ALARM_AREA_FROM_CLIENT,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    login.Person != null
                        ? string.Format(
                            "Set alarm area from client, login: {0}, person: {1}, alarm area: {2}{3}",
                            login,
                            login.Person,
                            alarmArea,
                            info)
                        : string.Format(
                            "Set alarm area from client, login: {0}, alarm area: {1}{2}",
                            login,
                            alarmArea,
                            info),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }

    [LwSerialize(417)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnsetAlarmAreaFromClient : EventParametersWithObjectId
    {
        public Guid IdLogin { get; private set; }
        public int TimeToBuy { get; private set; }

        public EventUnsetAlarmAreaFromClient(
            UInt64 eventId,
            Guid guidAlarmArea,
            Guid idLogin,
            int timeToBuy)
            : base(
                eventId,
                EventType.UnsetAlarmAreaFromClient,
                guidAlarmArea)
        {
            IdLogin = idLogin;
            TimeToBuy = timeToBuy;
        }

        protected EventUnsetAlarmAreaFromClient()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var login = Logins.Singleton.GetById(IdLogin);
            if (login == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            ICollection<Guid> eventSourcesIds = new LinkedList<Guid>(
                Enumerable.Repeat(
                    alarmArea.IdAlarmArea,
                    1)
                    .Concat(
                        Enumerable.Repeat(
                            login.IdLogin,
                            1)));

            if (login.Person != null)
                eventSourcesIds.Add(login.Person.IdPerson);


            string info = TimeToBuy > 0
                ? String.Format(
                    " with time buying {0:00}:{1:00}:{2:00}",
                    TimeToBuy / 3600,
                    (TimeToBuy % 3600) / 60,
                    TimeToBuy % 60)
                : string.Empty;

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_UNSET_ALARM_AREA_FROM_CLIENT,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    login.Person != null
                        ? string.Format(
                            "Unset alarm area from client, login: {0}, person: {1}, alarm area: {2}{3}",
                            login,
                            login.Person,
                            alarmArea,
                            info)
                        : string.Format(
                            "Unset alarm area from client, login: {0}, alarm area: {1}{2}",
                            login,
                            alarmArea,
                            info),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }

    [LwSerialize(418)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaActivationStateChanged : EventParametersWithObjectIdAndState
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        protected EventAlarmAreaActivationStateChanged()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var activationState = ActivationState.Unknown;

            switch (State)
            {
                case State.Set:
                    activationState = ActivationState.Set;
                    break;

                case State.Unset:
                    activationState = ActivationState.Unset;
                    break;

                case State.Prewarning:
                    activationState = ActivationState.Prewarning;
                    break;

                case State.TemporaryUnsetExit:
                    activationState = ActivationState.TemporaryUnsetExit;
                    break;

                case State.TemporaryUnsetEntry:
                    activationState = ActivationState.TemporaryUnsetEntry;
                    break;

                case State.UnsetBoughtTime:
                    activationState = ActivationState.UnsetBoughtTime;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaActivationStateChanged(
                IdObject,
                DateTime,
                activationState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {

            if (!GeneralOptions.Singleton.EventlogAlarmAreaActivationStateChanged)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);

            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            ICollection<Guid> eventSourcesIds = new LinkedList<Guid>();

            if (IdCardReader != Guid.Empty)
            {
                var cardReader = CardReaders.Singleton.GetById(IdCardReader);

                if (cardReader != null)
                {
                    var cardReaderEventSources = CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        alarmArea);

                    foreach (var cardReaderEventSource in cardReaderEventSources)
                    {
                        eventSourcesIds.Add(cardReaderEventSource);
                    }
                }
            }

            if (IdCard != Guid.Empty)
            {
                var card = Cards.Singleton.GetById(IdCard);

                if (card != null)
                {
                    eventSourcesIds.Add(card.IdCard);

                    if (card.Person != null)
                        eventSourcesIds.Add(card.Person.IdPerson);
                }
            }
            else if (IdPerson != Guid.Empty)
            {
                var person = Persons.Singleton.GetById(IdPerson);

                if (person != null)
                    eventSourcesIds.Add(person.IdPerson);
            }

            if (!eventSourcesIds.Contains(alarmArea.IdAlarmArea))
                eventSourcesIds.Add(alarmArea.IdAlarmArea);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEALARMAREAACTIVATIONSTATECHANGED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                        "Alarm area {0} changed its activation state to {1}",
                        alarmArea.ToString(),
                        State),
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(419)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventSetUnsetAlarmAreaByObjectForAutomaticActivation : EventParametersWithObjectId
    {
        public Guid IdOfObjectForAutomaticActivation { get; private set; }
        public ObjectType TypeOfObjectForAutomaticAtivation { get; private set; }

        protected EventSetUnsetAlarmAreaByObjectForAutomaticActivation(
            UInt64 eventId,
            EventType eventType,
            Guid idAlarmArea,
            Guid idOfObjectForAutomaticActivation,
            ObjectType typeOfObjectForAutomaticAtivation)
            : base(
                eventId,
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

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var alarmArea = AlarmAreas.Singleton.GetById(IdObject);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            IEnumerable<Guid> eventSourcesIds = null;
            var objectName = string.Empty;

            switch (TypeOfObjectForAutomaticAtivation)
            {
                case ObjectType.Input:
                {
                    var input = Inputs.Singleton.GetById(IdOfObjectForAutomaticActivation);
                    if (input != null)
                        objectName = input.Name;
                    eventSourcesIds = CCUEventsManager.GetEventSourcesFromInput(input, IdObject);
                    break;
                }

                case ObjectType.Output:
                {
                    var output = Outputs.Singleton.GetById(IdOfObjectForAutomaticActivation);
                    if (output != null)
                        objectName = output.Name;
                    eventSourcesIds = CCUEventsManager.GetEventSourcesFromOutput(output, IdObject);
                    break;
                }

                case ObjectType.TimeZone:
                {
                    var timeZone = TimeZones.Singleton.GetById(IdOfObjectForAutomaticActivation);
                    if (timeZone != null)
                        objectName = timeZone.Name;
                    eventSourcesIds = new[] {IdObject, IdOfObjectForAutomaticActivation};
                    break;
                }

                case ObjectType.DailyPlan:
                {
                    var dailyPlan = DailyPlans.Singleton.GetById(IdOfObjectForAutomaticActivation);
                    if (dailyPlan != null)
                        objectName = dailyPlan.DailyPlanName;
                    eventSourcesIds = new[] {IdObject, IdOfObjectForAutomaticActivation};
                    break;
                }
            }

            var eventType =
                EventType == EventType.SetAlarmAreaByObjectForAutomaticActivation
                    ? Eventlog.TYPESETALARMAREABYOBJECTFORAA
                    : Eventlog.TYPEUNSETALARMAREABYOBJECTFORAA;

            var description =
                string.Format(
                    "{0} alarm area by object for automatic activation: {1}",
                    EventType == EventType.SetAlarmAreaByObjectForAutomaticActivation
                        ? "Set"
                        : "Unset",
                    objectName);

            return Eventlogs.Singleton.CreateEvent(
                eventType,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(420)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetAlarmAreaByObjectForAutomaticActivation : EventSetUnsetAlarmAreaByObjectForAutomaticActivation
    {
        public EventSetAlarmAreaByObjectForAutomaticActivation(
            UInt64 eventId,
            Guid idAlarmArea,
            Guid idOfObjectForAutomaticActivation,
            ObjectType typeOfObjectForAutomaticAtivation)
            : base(
                eventId,
                EventType.SetAlarmAreaByObjectForAutomaticActivation,
                idAlarmArea,
                idOfObjectForAutomaticActivation,
                typeOfObjectForAutomaticAtivation)
        {

        }

        protected EventSetAlarmAreaByObjectForAutomaticActivation()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }
    }

    [LwSerialize(421)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnsetAlarmAreaByObjectForAutomaticActivation : EventSetUnsetAlarmAreaByObjectForAutomaticActivation
    {
        public EventUnsetAlarmAreaByObjectForAutomaticActivation(
            UInt64 eventId,
            Guid idAlarmArea,
            Guid idOfObjectForAutomaticActivation,
            ObjectType typeOfObjectForAutomaticAtivation)
            : base(
                eventId,
                EventType.UnsetAlarmAreaByObjectForAutomaticActivation,
                idAlarmArea,
                idOfObjectForAutomaticActivation,
                typeOfObjectForAutomaticAtivation)
        {

        }

        protected EventUnsetAlarmAreaByObjectForAutomaticActivation()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }
    }

    [LwSerialize(422)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSabotageStateChanged : EventParametersWithObjectIdAndState
    {
        public EventAlarmAreaSabotageStateChanged(
            UInt64 eventId,
            Guid idAlarmArea,
            State sabotageState)
            : base(
                eventId,
                EventType.AlarmAreaSabotageStateChanged,
                idAlarmArea,
                sabotageState)
        {

        }

        protected EventAlarmAreaSabotageStateChanged()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSabotageStateChanged(
                IdObject,
                DateTime,
                State);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(423)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSabotageStateInfo : EventParametersWithObjectIdAndState
    {
        public EventAlarmAreaSabotageStateInfo(
            UInt64 eventId,
            Guid idAlarmArea,
            State sabotageState)
            : base(
                eventId,
                EventType.AlarmAreaSabotageStateInfo,
                idAlarmArea,
                sabotageState)
        {

        }

        protected EventAlarmAreaSabotageStateInfo()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSabotageStateChanged(
                IdObject,
                DateTime,
                State);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(424)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventNotConfirmSetUnsetAaFromEis : EventParametersWithObjectId
    {
        public EventNotConfirmSetUnsetAaFromEis(
            UInt64 eventId,
            Guid idAlarmArea)
            : base(
                eventId,
                EventType.NotConfirmSetUnsetAAFromEIS,
                idAlarmArea)
        {

        }

        protected EventNotConfirmSetUnsetAaFromEis()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSetUnsetNotConfirm(
                IdObject,
                DateTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                Enumerable.Repeat(
                    ccu.IdCCU,
                    1)
                    .Concat(
                        Enumerable.Repeat(
                            IdObject,
                            1)));

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ALARM_AREA_SETUNSET_NOT_RESPOND,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "External alarm system did not respond in proper time",
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(425)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorBlockingTypeChanged : EventParametersWithObjectId
    {
        public Guid IdAlarmArea { get; private set; }
        public SensorBlockingType SensorBlockingType { get; private set; }
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        protected EventSensorBlockingTypeChanged()
        {

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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSensorBlockingTypeChanged(
                IdAlarmArea,
                IdObject,
                SensorBlockingType,
                DateTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(426)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorBlockingTypeInfo : EventParametersWithObjectId
    {
        public Guid IdAlarmArea { get; private set; }
        public SensorBlockingType SensorBlockingType { get; private set; }

        protected EventSensorBlockingTypeInfo()
        {

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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSensorBlockingTypeChanged(
                IdAlarmArea,
                IdObject,
                SensorBlockingType,
                DateTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(427)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorStateChanged : EventParametersWithObjectIdAndState
    {
        public Guid IdAlarmArea { get; private set; }

        protected EventSensorStateChanged()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}",
                    IdAlarmArea));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSensorStateChanged(
                IdAlarmArea,
                IdObject,
                State,
                DateTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(428)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSensorStateInfo : EventParametersWithObjectIdAndState
    {
        public Guid IdAlarmArea { get; private set; }

        protected EventSensorStateInfo()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}",
                    IdAlarmArea));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSensorStateChanged(
                IdAlarmArea,
                IdObject,
                State,
                DateTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(672)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventTimeBuyingMatrixStateInfo : EventParametersWithObjectId
    {
        public TimeBuyingMatrixState TimeBuyingMatrixState { get; private set; }

        protected EventTimeBuyingMatrixStateInfo()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", TimeBuyingMatrixState: {0}",
                    TimeBuyingMatrixState));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaTimeBuingMatrixStateChanged(
                IdObject,
                TimeBuyingMatrixState,
                DateTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }
}
