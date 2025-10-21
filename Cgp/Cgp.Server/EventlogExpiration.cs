using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.Server
{
    public class EventlogExpiration
    {
        private int _expirationDays;
        private int _eventlogsCount;
        private Beans.TimeZone _timeZone;
        private Beans.TimeZone _timeZoneReports;

        private static volatile EventlogExpiration _singleton = null;
        private static object _syncRoot = new object();

        public static EventlogExpiration Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new EventlogExpiration();
                    }

                return _singleton;
            }
        }

        public void Start()
        {
            try
            {
                TimeAxis.Singleton.TimeZoneStateChanged += TimeZoneStateChanged;
                ServerGeneralOptionsProvider.Singleton.DatabaseEventlogExpirationChanged += DatabaseChanged;
                EventlogExpiration.Singleton.ReadFromGeneralSettings();

                // Eventlog Reports
                if (_timeZoneReports != null)
                {
                    TimeAxis.Singleton.TimeZoneStateChanged += TimeZoneReportsStateChanged;
                    ServerGeneralOptionsProvider.Singleton.DatabaseEventlogExpirationChanged += DatabaseChanged;
                }
            }
            catch
            { }
        }

        static void DatabaseChanged()
        {
            EventlogExpiration.Singleton.ReadFromGeneralSettings();
        }

        static void TimeZoneStateChanged(Guid idTimeZone, byte status)
        {
            if (EventlogExpiration.Singleton._timeZone != null && EventlogExpiration.Singleton._timeZone.IdTimeZone == idTimeZone)
            {
                if (status == 1)
                {
                    EventlogExpiration.Singleton.DoEventlogExpitationCleaning();
                }
            }
        }

        static void TimeZoneReportsStateChanged(Guid idTimeZone, byte status)
        {
            if (EventlogExpiration.Singleton._timeZoneReports != null && EventlogExpiration.Singleton._timeZoneReports.IdTimeZone == idTimeZone)
            {
                if (status == 1)
                {
                    EventlogExpiration.Singleton.DoEventlogReports();
                }
            }
        }

        private void ReadFromGeneralSettings()
        {
            string timeZoneStringGuid = GeneralOptions.Singleton.EventlogTimeZoneGuidString;
            if (timeZoneStringGuid != null && timeZoneStringGuid != string.Empty)
            {
                Guid tzGuid = new Guid(timeZoneStringGuid);
                _timeZone = Contal.Cgp.Server.DB.TimeZones.Singleton.GetById(tzGuid);
            }
            else
                _timeZone = null;

            _expirationDays = GeneralOptions.Singleton.EventlogsExpirationDays;
            _eventlogsCount = (int)(GeneralOptions.Singleton.EventlogsMaxCountValue * Math.Pow(10, GeneralOptions.Singleton.EventlogsMaxCountExponent + 2));

            // Eventlog Reports
            timeZoneStringGuid = GeneralOptions.Singleton.EventlogReportsTimeZoneGuidString;
            if (!string.IsNullOrEmpty(timeZoneStringGuid) && !string.IsNullOrEmpty(GeneralOptions.Singleton.EventlogReportsEmails))
            {
                Guid tzGuid = new Guid(timeZoneStringGuid);
                _timeZoneReports = Contal.Cgp.Server.DB.TimeZones.Singleton.GetById(tzGuid);
            }
            else
                _timeZoneReports = null;
        }

        private void DoEventlogExpitationCleaning()
        {
            DeleteCountExpiration(_eventlogsCount);
            DeleteDayExpiration(_expirationDays);
        }

        private bool DeleteDayExpiration(int days)
        {
            try
            {
                if (days == 0) return true;
                DB.Eventlogs.Singleton.DeleteEventlogByExpiratedDays(days);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteCountExpiration(int maxRecords)
        {
            try
            {
                if (maxRecords == 0) return true;
                Contal.Cgp.Server.DB.Eventlogs.Singleton.DeleteEventlogByExpiratedCount(maxRecords);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ForceEventlogExpitationCleaning(int expiratedDays, int maxRecords)
        {
            if (!DeleteCountExpiration(maxRecords)) return false;
            if (!DeleteDayExpiration(expiratedDays)) return false;
            return true;
        }

        private void DoEventlogReports()
        {
            string emails = GeneralOptions.Singleton.EventlogReportsEmails;

            if (!string.IsNullOrEmpty(emails) /*&& EmailAddress.IsValid(emails)*/)
            {
                // Generate and send reports to predefined emails
                IList<FilterSettings> filterSettingsList = new List<FilterSettings>();
                DateTime dateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dateTo = dateTo.AddDays(1);
                dateTo = dateTo.Subtract(new TimeSpan(0, 0, 0, 1, 0));

                DateTime dateFrom = dateTo.AddDays(-7);
                dateFrom = dateFrom.AddSeconds(1);

                FilterSettings filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTLOG_DATE_TIME, dateFrom, ComparerModes.EQUALLMORE);
                filterSettingsList.Add(filterSetting);
                filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTLOG_DATE_TIME, dateTo, ComparerModes.EQUALLLESS);
                filterSettingsList.Add(filterSetting);

                var eventlogTypes = new LinkedList<string>();
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIED);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE);
                eventlogTypes.AddLast(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_CODE);
                eventlogTypes.AddLast(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_PIN);
                eventlogTypes.AddLast(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_CODE);
                eventlogTypes.AddLast(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_PIN);
                eventlogTypes.AddLast(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_CODE);
                eventlogTypes.AddLast(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_PIN);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDINVALIDCODE);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDINVALIDEMERGENCYCODE);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDINVALIDPIN);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDCODE);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS);
                eventlogTypes.AddLast(Eventlog.TYPEUNKNOWNCARD);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDCODE);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN);
                eventlogTypes.AddLast(Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS);
                eventlogTypes.AddLast(Eventlog.TYPEDSMACCESSINTERRUPTED);
                eventlogTypes.AddLast(Eventlog.TYPEDSMACCESSPERMITTED);
                eventlogTypes.AddLast(Eventlog.TYPEDSMACCESSRESTRICTED);
                eventlogTypes.AddLast(Eventlog.TYPEDSMNORMALACCESS);

                FilterSettings filterSettingRules = new FilterSettings(Eventlog.COLUMN_TYPE, eventlogTypes, ComparerModes.IN);
                filterSettingsList.Add(filterSettingRules);

                bool isInRootSite = false;
                Login login = Logins.Singleton.GetLoginByUserName("admin");
                var subSitesForLogin = StructuredSubSites.Singleton.GetAllSubSitesForLogin(login, out isInRootSite);
                List<int> subSitesForLoginIds = subSitesForLogin.Select(x => x.IdStructuredSubSite).ToList<int>();

                Eventlogs.Singleton.GenerateDataForExcelByCriteria(filterSettingsList, subSitesForLoginIds, "yyyy-MM-dd HH:mm:ss", Guid.NewGuid().ToString(), emails);
            }
        }
    }
}
