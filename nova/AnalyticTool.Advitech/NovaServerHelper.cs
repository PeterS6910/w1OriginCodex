using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.NonVisual;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Client.NonVisual;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;

namespace AnalyticTool.Advitech
{
    public class NovaServerHelper
    {
        private static NovaServerHelper _singleton;
        private readonly CgpClientNonVisual _client = CgpClientNonVisual.Singleton;
        private ICgpNCASRemotingProvider _ncasRemotingProvider;

        public static NovaServerHelper Singleton
        {
            get { return _singleton ?? (_singleton = new NovaServerHelper()); }
        }

        public bool Init()
        {
            Disconnect();

            _client.SetServerConnectionParams(
                ApplicationProperties.Singleton.ServerIp,
                ApplicationProperties.Singleton.ServerPort);

            _client.SetLoginAuthentication(
                ApplicationProperties.Singleton.ServerUserName,
                ApplicationProperties.Singleton.ServerPassword);

            if (!_client.Launch())
                return false;

            var ncasPlugin = (NCASClientNonVisual)_client.PluginManager.GetLoadedPlugin("NCAS plugin");
            _ncasRemotingProvider = ncasPlugin.MainServerProvider;

            return true;
        }

        public void Disconnect()
        {
            if (!IsClientLogged())
                return;

            _client.RequestShutdown();
            _ncasRemotingProvider = null;
        }

        public bool IsClientLogged()
        {
            return _client.IsLoggedIn;
        }

        public ICollection<CardReader> GetCardReaders()
        {
            if (_ncasRemotingProvider == null)
                return null;

            Exception error;

            var cardReadrs = _ncasRemotingProvider.CardReaders.List(out error);

            if (error != null)
                throw error;

            return cardReadrs;
        }

        public ICollection<Input> GetInputs()
        {
            if (_ncasRemotingProvider == null)
                return null;

            Exception error;

            var inputs = _ncasRemotingProvider.Inputs.List(out error);

            if (error != null)
                throw error;

            return inputs;
        }

        public Card FindCardFromEventSources(ICollection<EventSource> eventSources)
        {
            if (eventSources == null)
                return null;

            foreach (var eventSource in eventSources)
            {
                if (_client.MainServerProvider.CentralNameRegisters.GetObjectTypeFromGuid(
                        eventSource.EventSourceObjectGuid) == ObjectType.Card)
                {
                    return _client.MainServerProvider.Cards.GetObjectById(eventSource.EventSourceObjectGuid);
                }
            }

            return null;
        }

        public string GetDepartmentFolderName(Guid idPerson)
        {
            var userFolderStructures = _client.MainServerProvider.UserFoldersSutructures.GetUserFoldersForObject(
                idPerson.ToString(),
                ObjectType.Person);

            var userFolderStructure = userFolderStructures != null
                ? userFolderStructures.FirstOrDefault()
                : null;

            return userFolderStructure != null
                ? userFolderStructure.FolderName
                : string.Empty;
        }

        public ICollection<Eventlog> GetEvents(long lastId)
        {
            var filterSettings = new List<FilterSettings>
            {
                new FilterSettings(Eventlog.COLUMN_ID_EVENTLOG, lastId, ComparerModes.MORE)
            };

            return GetEvents(filterSettings);
        }

        public ICollection<Eventlog> GetEvents(DateTime dateFrom)
        {
            var filterSettings = new List<FilterSettings>
            {
                new FilterSettings(Eventlog.COLUMN_EVENTLOG_DATE_TIME, dateFrom, ComparerModes.EQUALLMORE)
            };

            return GetEvents(filterSettings);
        }

        private ICollection<Eventlog> GetEvents(List<FilterSettings> filterSettings)
        {
            if (filterSettings == null)
                return null;

            filterSettings.Add(
                new FilterSettings(
                    Eventlog.COLUMN_TYPE,
                    new List<string>()
                    {
                        Eventlog.TYPEDSMNORMALACCESS,
                        Eventlog.TYPE_INPUT_STATE_CHANGED
                    },
                    ComparerModes.IN));

            filterSettings.Add(
                new FilterSettings(
                    Eventlog.COLUMN_EVENTSOURCES,
                    new List<Guid>()
                    {
                        ApplicationProperties.Singleton.InputCardReader,
                        ApplicationProperties.Singleton.OutputCardReader,
                        ApplicationProperties.Singleton.Pump1Input,
                        ApplicationProperties.Singleton.Pump2Input,
                        ApplicationProperties.Singleton.Pump3Input,
                        ApplicationProperties.Singleton.Pump4Input
                    },
                    ComparerModes.IN));

            Exception error;

            int count = _client.MainServerProvider.Eventlogs.SelectCount(filterSettings, null, out error);

            if (error != null)
                return null;

            var events = new List<Eventlog>(count);
            events.AddRange(_client.MainServerProvider.Eventlogs.SelectRangeByCriteria(
                filterSettings, null, 0, count)
                .Select(e => _client.MainServerProvider.Eventlogs.GetObjectById(e.IdEventlog)));

            return events;
        }
    }
}
