using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;

using NHibernate.Util;

namespace Contal.Cgp.Server.DB
{
    partial class Eventlogs
    {
        private class FilterSettingsCommandGenerator
        {
            private readonly Eventlogs _eventlogs;
            private readonly string _sessionId;

            private DateTime? _dateFrom;
            private DateTime? _dateTo;

            private FilterSettings _filterSettingEventSourceCollectionOr;
            private FilterSettings _filterSettingEventSourceSimpleOr;
            private FilterSettings _filterSettingDescriptionOr;
            private FilterSettings _filterSettingCgpSourceOr;
            private FilterSettings _filterSettingEventSourceCollectionAnd;
            private FilterSettings _filterSettingEventSourceSimpleAnd;
            private FilterSettings _filterSettingDescriptionAnd;
            private FilterSettings _filterSettingCgpSourceAnd;
            private FilterSettings _filterSettingType;
            private FilterSettings _filterSettingEventlogId;

            public FilterSettingsCommandGenerator(
                Eventlogs eventlogs,
                string sessionId)
            {
                _eventlogs = eventlogs;
                _sessionId = sessionId;
            }

            private void AppendEventlogId(StringBuilder stringBuilder)
            {
                string op = "=";

                switch (_filterSettingEventlogId.ComparerMode)
                {
                    case ComparerModes.EQUALLMORE:
                        op = ">=";
                        break;

                    case ComparerModes.EQUALLLESS:
                        op = "<=";
                        break;

                    case ComparerModes.LESS:
                        op = "<";
                        break;

                    case ComparerModes.MORE:
                        op = ">";
                        break;
                }

                stringBuilder.AppendFormat(
                    "{0} {1} {2}",
                    _filterSettingEventlogId.Column,
                    op,
                    _filterSettingEventlogId.Value);
            }

            private void AppendType(StringBuilder stringBuilder)
            {
                //stringBuilder.AppendFormat(
                //    "{0} = '{1}'",
                //    _filterSettingType.Column,
                //    _filterSettingType.Value);

                stringBuilder.AppendFormat(
                    "{0} IN (",
                    _filterSettingType.Column);

                var collection = _filterSettingType.Value as IEnumerable;

                foreach (string parameter in collection)
                {
                    stringBuilder.AppendFormat("'{0}',", parameter);
                }

                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.AppendFormat(")");
            }

            private void AppendDescriptionAnd(StringBuilder stringBuilder)
            {
                AppendDescription(
                    stringBuilder,
                    _filterSettingDescriptionAnd);
            }

            private void AppendDescriptionOr(StringBuilder stringBuilder)
            {
                AppendDescription(
                    stringBuilder,
                    _filterSettingDescriptionOr);
            }

            private static void AppendDescription(
                StringBuilder stringBuilder,
                FilterSettings filterSettings)
            {
                stringBuilder.AppendFormat(
                    "{0} like '%{1}%'",
                    filterSettings.Column,
                    filterSettings.Value);
            }

            private void AppendCgpSourceAnd(StringBuilder stringBuilder)
            {
                AppendCgpSource(
                    stringBuilder,
                    _filterSettingCgpSourceAnd);
            }

            private void AppendCgpSourceOr(StringBuilder stringBuilder)
            {
                AppendCgpSource(
                    stringBuilder,
                    _filterSettingCgpSourceOr);
            }

            private static void AppendCgpSource(
                StringBuilder stringBuilder,
                FilterSettings filterSetting)
            {
                stringBuilder.AppendFormat(
                    "{0} like '%{1}%'",
                    filterSetting.Column,
                    filterSetting.Value);
            }

            private void AppendEventSourcesSimpleAnd(StringBuilder stringBuilder)
            {
                AppendEventSourcesSimple(
                    stringBuilder,
                    _filterSettingEventSourceSimpleAnd);
            }

            private void AppendEventSourcesSimpleOr(StringBuilder stringBuilder)
            {
                AppendEventSourcesSimple(
                    stringBuilder,
                    _filterSettingEventSourceSimpleOr);
            }

            private void AppendEventSourcesSimple(
                StringBuilder stringBuilder,
                FilterSettings filterSetting)
            {
                string tmpTableCentralNameRegisterId =
                    _eventlogs.CreateTmpTableCentralNameRegisterId(
                        (string) filterSetting.Value,
                        _sessionId);

                stringBuilder.AppendFormat(
                    "(select count({0}) from {1} as {2} where {3}.{4} = {5}.{6} and {7}.{8} in (select {9} from {10})) > 0",
                    EventSource.COLUMN_EVENTLOG_ID,
                    EventSources.EVENTSOURCE_TABLE_NAME,
                    EVENT_SOURCE_ALIAS,
                    EVENTLOG_ALIAS,
                    Eventlog.COLUMN_ID_EVENTLOG,
                    EVENT_SOURCE_ALIAS,
                    EventSource.COLUMN_EVENTLOG_ID,
                    EVENT_SOURCE_ALIAS,
                    EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                    CentralNameRegister.COLUMN_ID,
                    tmpTableCentralNameRegisterId);
            }

            private void AppendOrClauses(StringBuilder stringBuilder)
            {
                GenerateStringBuilderSequence(
                    stringBuilder,
                    "(",
                    " OR ",
                    OrClauses,
                    ")");
            }

            private IEnumerable<Action<StringBuilder>> OrClauses
            {
                get
                {
                    if (_filterSettingCgpSourceOr != null)
                        yield return AppendCgpSourceOr;

                    if (_filterSettingDescriptionOr != null)
                        yield return AppendDescriptionOr;

                    if (_filterSettingEventSourceSimpleOr != null)
                        yield return AppendEventSourcesSimpleOr;
                    else if (_filterSettingEventSourceCollectionOr != null)
                        yield return AppendEventSourceCollectionOr;
                }
            }

            private void AppendEventSourceCollectionAnd(
                StringBuilder stringBuilder)
            {
                AppendEventSourceCollection(
                    stringBuilder,
                    _filterSettingEventSourceCollectionAnd);
            }

            private void AppendEventSourceCollectionOr(
                StringBuilder stringBuilder)
            {
                AppendEventSourceCollection(
                    stringBuilder,
                    _filterSettingEventSourceCollectionOr);
            }

            private static void AppendEventSourceCollection(
                StringBuilder stringBuilder,
                FilterSettings filterSetting)
            {
                string values = string.Empty;
                int valuesCount = 0;

                foreach (object collectionValue in (IEnumerable) filterSetting.Value)
                {
                    if (valuesCount >= 1)
                        values += ", ";

                    values += "'" + collectionValue + "'";
                    valuesCount++;
                }

                switch (filterSetting.ComparerMode)
                {
                    case ComparerModes.IN:

                        stringBuilder.AppendFormat(
                            "(Select count({0}) from {1} as {2} where {3}.{4} = {5}.{6} and {7}.{8} in ({9})) > 0",
                            EventSource.COLUMN_EVENTLOG_ID,
                            EventSources.EVENTSOURCE_TABLE_NAME,
                            EVENT_SOURCE_ALIAS,
                            EVENTLOG_ALIAS,
                            Eventlog.COLUMN_ID_EVENTLOG,
                            EVENT_SOURCE_ALIAS,
                            EventSource.COLUMN_EVENTLOG_ID,
                            EVENT_SOURCE_ALIAS,
                            EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                            values);
                        break;

                    case ComparerModes.EQUALL:

                        stringBuilder.AppendFormat(
                            "(Select count({0}) from {1} as {2} where {3}.{4} = {5}.{6} and {7}.{8} in ({9})) = {10}",
                            EventSource.COLUMN_EVENTLOG_ID,
                            EventSources.EVENTSOURCE_TABLE_NAME,
                            EVENT_SOURCE_ALIAS,
                            EVENTLOG_ALIAS,
                            Eventlog.COLUMN_ID_EVENTLOG,
                            EVENT_SOURCE_ALIAS,
                            EventSource.COLUMN_EVENTLOG_ID,
                            EVENT_SOURCE_ALIAS,
                            EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                            values,
                            valuesCount);

                        break;
                }
            }

            private void AppendDateTime(StringBuilder stringBuilder)
            {
                if (_dateFrom != null && _dateTo != null)
                    stringBuilder.AppendFormat(
                        "{0} BETWEEN '{1}' AND '{2}'",
                        Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                        _eventlogs.GetDateFromDateTime(_dateFrom.Value),
                        _eventlogs.GetDateFromDateTime(_dateTo.Value));
                else if (_dateFrom != null)
                    stringBuilder.AppendFormat(
                        "{0} >= '{1}'",
                        Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                        _eventlogs.GetDateFromDateTime(_dateFrom.Value));
                else if (_dateTo != null)
                    stringBuilder.AppendFormat(
                        "{0} <= '{1}'",
                        Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                        _eventlogs.GetDateFromDateTime(_dateTo.Value));
            }

            private IEnumerable<Action<StringBuilder>> AndClauses
            {
                get
                {
                    if (_filterSettingEventSourceSimpleAnd != null)
                        yield return AppendEventSourcesSimpleAnd;
                    else
                        if (_filterSettingEventSourceCollectionAnd != null)
                            yield return AppendEventSourceCollectionAnd;

                    if (_dateFrom != null || _dateTo != null)
                        yield return AppendDateTime;

                    if (OrClauses.Any())
                        yield return AppendOrClauses;

                    if (_filterSettingEventlogId != null)
                        yield return AppendEventlogId;

                    if (_filterSettingType != null)
                        yield return AppendType;

                    if (_filterSettingCgpSourceAnd != null)
                        yield return AppendCgpSourceAnd;

                    if (_filterSettingDescriptionAnd != null)
                        yield return AppendDescriptionAnd;
                }
            }

            private static void GenerateStringBuilderSequence(
                StringBuilder stringBuilder,
                string prefix,
                string separator,
                IEnumerable<Action<StringBuilder>> clauses,
                string postfix)
            {
                bool firstEncountered = false;

                foreach (var clause in clauses)
                {
                    if (firstEncountered)
                        stringBuilder.Append(separator);
                    else
                    {
                        firstEncountered = true;
                        stringBuilder.Append(prefix);
                    }

                    clause(stringBuilder);
                }

                if (firstEncountered)
                    stringBuilder.Append(postfix);
            }

            public string Generate()
            {
                var stringBuilder = new StringBuilder();

                GenerateStringBuilderSequence(
                    stringBuilder,
                    " WHERE ",
                    " AND ",
                    AndClauses,
                    "");

                return stringBuilder.ToString();
            }

            public void Parse(IList<FilterSettings> filterSettings)
            {
                //Create date criterion
                GetDatesFromFilterSettings(
                    filterSettings,
                    out _dateFrom,
                    out _dateTo);

                foreach (FilterSettings filterSetting in filterSettings)
                    switch (filterSetting.Column)
                    {
                        case Eventlog.COLUMN_ID_EVENTLOG:

                            long id;

                            if (long.TryParse(filterSetting.Value.ToString(), out id))
                            {
                                _filterSettingEventlogId = filterSetting;
                            }

                            break;

                        case Eventlog.COLUMN_TYPE:
                            //if (filterSetting.Value is string)
                            //    _filterSettingType = filterSetting;
                            var types =
                                filterSetting.Value as IEnumerable;

                            if (types == null)
                                break;

                            _filterSettingType = filterSetting;

                            break;

                        case Eventlog.COLUMN_CGPSOURCE:
                            if (filterSetting.Value is string &&
                                filterSetting.ComparerMode ==
                                ComparerModes.LIKEBOTH)
                                switch (filterSetting.LogicalOperator)
                                {
                                    case LogicalOperators.AND:
                                        _filterSettingCgpSourceAnd =
                                            filterSetting;
                                        break;

                                    case LogicalOperators.OR:
                                        _filterSettingCgpSourceOr =
                                            filterSetting;
                                        break;
                                }

                            break;

                        case Eventlog.COLUMN_DESCRIPTION:

                            if (filterSetting.Value is string &&
                                filterSetting.ComparerMode ==
                                ComparerModes.LIKEBOTH)
                                switch (filterSetting.LogicalOperator)
                                {
                                    case LogicalOperators.AND:
                                        _filterSettingDescriptionAnd =
                                            filterSetting;
                                        break;

                                    case LogicalOperators.OR:
                                        _filterSettingDescriptionOr =
                                            filterSetting;
                                        break;
                                }

                            break;

                        case Eventlog.COLUMN_EVENTSOURCES:

                            if (filterSetting.Value is string &&
                                filterSetting.ComparerMode ==
                                ComparerModes.LIKEBOTH)
                            {
                                switch (filterSetting.LogicalOperator)
                                {
                                    case LogicalOperators.AND:
                                        _filterSettingEventSourceSimpleAnd =
                                            filterSetting;
                                        break;
                                    case LogicalOperators.OR:
                                        _filterSettingEventSourceSimpleOr =
                                            filterSetting;
                                        break;
                                }

                                break;
                            }

                            if (filterSetting.ComparerMode != ComparerModes.IN &&
                                filterSetting.ComparerMode !=
                                ComparerModes.EQUALL)
                                break;

                            var collection =
                                filterSetting.Value as IEnumerable;

                            if (collection == null)
                                break;

                            switch (filterSetting.LogicalOperator)
                            {
                                case LogicalOperators.AND:
                                    _filterSettingEventSourceCollectionAnd =
                                        filterSetting;
                                    break;
                                case LogicalOperators.OR:
                                    _filterSettingEventSourceCollectionOr =
                                        filterSetting;
                                    break;
                            }

                            break;
                    }
            }

            /// <summary>
            /// Return date from and date to from filter settings
            /// </summary>
            /// <param name="filterSettings"></param>
            /// <param name="dateFrom"></param>
            /// <param name="dateTo"></param>
            private void GetDatesFromFilterSettings(
                IList<FilterSettings> filterSettings,
                out DateTime? dateFrom,
                out DateTime? dateTo)
            {
                dateFrom = null;
                dateTo = null;

                if (filterSettings == null || filterSettings.Count <= 0)
                    return;

                foreach (FilterSettings filterSetting in filterSettings)
                {
                    if (filterSetting.Column != Eventlog.COLUMN_EVENTLOG_DATE_TIME ||
                            !(filterSetting.Value is DateTime))
                        continue;

                    switch (filterSetting.ComparerMode)
                    {
                        case ComparerModes.EQUALLMORE:
                            dateFrom = (DateTime)filterSetting.Value;
                            break;

                        case ComparerModes.EQUALLLESS:
                            dateTo = (DateTime)filterSetting.Value;
                            break;
                    }
                }
            }
        }
        
    }
}