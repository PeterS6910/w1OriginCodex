using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Threads;
using Microsoft.Win32;

namespace AnalyticTool.Advitech
{
    public class Core
    {
        private DateTime _startDate;

        private bool _wasAnalysedInThisDay 
        {
            get
            {
                string analysingDailyValue = string.Empty;

                RegistryKey registryKey;

                if (RegistryHelper.TryParseKey(ApplicationProperties.RegPathClientRoot, true, out registryKey))
                {
                    analysingDailyValue = (string) registryKey.GetValue("AnalysingDailyValue");
                }

                return !string.IsNullOrEmpty(analysingDailyValue)
                       && analysingDailyValue.ToLower() == "true";
            }
            set
            {
                RegistryHelper.GetOrAddKey(
                    ApplicationProperties.RegPathClientRoot).
                    SetValue("AnalysingDailyValue", value, RegistryValueKind.String);
            }
        }

        private static Core _singleton;
        private DatabaseEngine _databaseEngine;
        private const int _defaultTimePeriod = 60000;
        private ITimer _timer;
        private HashSet<Guid> _inputsGuids;

        public static Core Singleton 
        {
            get { return _singleton ?? (_singleton = new Core()); }
        }

        public void Start()
        {
            try
            {
                NovaServerHelper.Singleton.Init();

                _inputsGuids = new HashSet<Guid>();

                if (ApplicationProperties.Singleton.Pump1Input != Guid.Empty)
                    _inputsGuids.Add(ApplicationProperties.Singleton.Pump1Input);

                if (ApplicationProperties.Singleton.Pump2Input != Guid.Empty)
                    _inputsGuids.Add(ApplicationProperties.Singleton.Pump2Input);

                if (ApplicationProperties.Singleton.Pump3Input != Guid.Empty)
                    _inputsGuids.Add(ApplicationProperties.Singleton.Pump3Input);

                if (ApplicationProperties.Singleton.Pump4Input != Guid.Empty)
                    _inputsGuids.Add(ApplicationProperties.Singleton.Pump4Input);

                _databaseEngine = new DatabaseEngine(
                    ApplicationProperties.Singleton.GetConnectionStringBuilderForDatabase(),
                    ApplicationProperties.Singleton.TableName);

                _startDate = ApplicationProperties.Singleton.DefaultStartDate;

                int timerPeriod = ApplicationProperties.Singleton.ScheduleType == 0
                    ? _defaultTimePeriod
                    : ApplicationProperties.Singleton.ScheduleValue * 60000;

                _timer = TimerManager.Static.StartTimer(
                    timerPeriod,
                    true,
                    OnTimerTick);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Console.WriteLine(ex.ToString());
            }
        }

        public void Stop()
        {
            if (_timer != null)
                _timer.StopTimer();

            NovaServerHelper.Singleton.Disconnect();
        }

        private Guid GetInputByEventSources(Eventlog eventlog)
        {
            return
                _inputsGuids.FirstOrDefault(
                    inputGuid => eventlog.EventSources.
                        Any(eventSource => eventSource.EventSourceObjectGuid.Equals(inputGuid)));
        }

        private bool OnTimerTick(TimerCarrier timerCarrier)
        {
            try
            {
                if (ApplicationProperties.Singleton.ScheduleType == 0) //daily mode
                {
                    if (DateTime.Now.Hour*60 + DateTime.Now.Minute < ApplicationProperties.Singleton.ScheduleValue)
                    {
                        if (_wasAnalysedInThisDay)
                            _wasAnalysedInThisDay = false;

                        return true;
                    }

                    if (_wasAnalysedInThisDay)
                        return true;

                    _wasAnalysedInThisDay = true;
                }

                long lastId = _databaseEngine.GetLastId();

                var events = lastId == -1
                    ? NovaServerHelper.Singleton.GetEvents(_startDate)
                    : NovaServerHelper.Singleton.GetEvents(lastId);

                Dictionary<Guid, int> inputsCounter = null;
                Card card = null;
                var stopTime = DateTime.MinValue;
                var databaseRecords = new LinkedList<DatabaseRecord>();

                foreach (var e in events)
                {
                    if (e.Type == Eventlog.TYPEDSMNORMALACCESS
                        && e.EventSources.Any
                            (eventSource =>
                                eventSource.EventSourceObjectGuid.Equals(
                                    ApplicationProperties.Singleton.OutputCardReader)))
                    {
                        card = NovaServerHelper.Singleton.FindCardFromEventSources(e.EventSources);

                        if (card == null)
                            continue;

                        stopTime = e.EventlogDateTime;
                        inputsCounter = new Dictionary<Guid, int>();

                        continue;
                    }

                    if (card != null
                        && e.Type == Eventlog.TYPE_INPUT_STATE_CHANGED
                        && e.Description.EndsWith("Alarm"))
                    {
                        var inputGuid = GetInputByEventSources(e);

                        if (inputGuid != Guid.Empty)
                        {
                            if (!inputsCounter.ContainsKey(inputGuid))
                                inputsCounter.Add(inputGuid, 0);

                            inputsCounter[inputGuid]++;
                        }

                        continue;
                    }

                    if (e.Type == Eventlog.TYPEDSMNORMALACCESS
                        && card != null
                        && e.EventSources.Any
                            (s => s.EventSourceObjectGuid.Equals(ApplicationProperties.Singleton.InputCardReader)))
                    {
                        databaseRecords.AddLast(CreateDatabaseRecord(card, inputsCounter, e.EventlogDateTime, stopTime));

                        lastId = lastId < e.IdEventlog
                            ? e.IdEventlog
                            : lastId;

                        card = null;
                        inputsCounter = null;
                    }
                }

                _databaseEngine.InsertRecords(databaseRecords, lastId);
            }
            catch (Exception ex)
            {
                if (ApplicationProperties.Singleton.ScheduleType == 0)
                    _wasAnalysedInThisDay = false;

                if (!NovaServerHelper.Singleton.IsClientLogged())
                    NovaServerHelper.Singleton.Init();

                HandledExceptionAdapter.Examine(ex);
//#if DEBUG
                Console.WriteLine(ex.ToString());
//#endif
            }

            return true;
        }

        private DatabaseRecord CreateDatabaseRecord(Card card, IDictionary<Guid, int> counts, DateTime start, DateTime stop)
        {
            int pump1;
            counts.TryGetValue(ApplicationProperties.Singleton.Pump1Input, out pump1);

            int pump2;
            counts.TryGetValue(ApplicationProperties.Singleton.Pump2Input, out pump2);

            int pump3;
            counts.TryGetValue(ApplicationProperties.Singleton.Pump3Input, out pump3);

            int pump4;
            counts.TryGetValue(ApplicationProperties.Singleton.Pump4Input, out pump4);

            var databaseRecord = new DatabaseRecord
            {
                FirstName = card.Person.FirstName,
                Surname = card.Person.Surname,
                Company = card.Person.Company,
                Address = card.Person.Address,
                Email = card.Person.Email,
                PhoneNumber = card.Person.PhoneNumber,
                Identification = card.Person.Identification,
                CostCenter = card.Person.CostCenter,
                Department = NovaServerHelper.Singleton.GetDepartmentFolderName(card.Person.IdPerson),
                EmploymentBeginningDate = card.Person.EmploymentBeginningDate,
                EmploymentEndDate = card.Person.EmploymentEndDate,
                CardNumber = card.FullCardNumber,
                Pump1 = pump1,
                Pump2 = pump2,
                Pump3 = pump3,
                Pump4 = pump4,
                Start = start,
                Stop = stop,
                Invoiced = 0
            };

            return databaseRecord;
        }
    }
}
