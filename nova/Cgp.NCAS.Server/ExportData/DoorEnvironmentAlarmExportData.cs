using Contal.Cgp.BaseLib;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.ExportData;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OfficeOpenXml.ExcelErrorValue;

namespace Contal.Cgp.NCAS.Server.ExportData
{
    public  class DoorEnvironmentAlarmExportData : IRefExportData
    {
        private class Counter
        {
            public int count = 0;
            public DateTime dtFirst = DateTime.MinValue;
        }

        private class DoorAjar
        {
            public DateTime dtCreated;
            public AlarmState AlarmState;
            public string Parent;
            public string Name;
            public DateTime dtEnded=DateTime.MinValue;
            public string Person=string.Empty;
            public string PersonId="";
            private string _trimName;
            public DoorAjar(Eventlog eventlog)
            {
                dtCreated = eventlog.EventlogDateTime;
                if(eventlog.EventlogParameters==null)
                {
                    eventlog = Eventlogs.Singleton.GetObjectById(eventlog.IdEventlog);
                }
                if (eventlog.EventlogParameters != null)
                {
                    var parent = eventlog.EventlogParameters.Where(p => p.Type == "Parent object").First();
                    if (parent != null)
                    {
                        Parent = parent.Value;
                    }
                    var name = eventlog.EventlogParameters.Where(p => p.Type == "Unique name").First();
                    if (name != null && name.Value.Length > 1)
                    {
                        var name2 = name.Value.Split(':');
                        if (name2.Count() > 1)
                        {
                             Name = name2[1];
                            _trimName = Regex.Replace(Name, @"\s", "");
                        }
                    }

                    var states = eventlog.EventlogParameters.Where(p => p.Type == "Alarm state").First();
                    if (states != null && states.Value.Length > 1)
                    {
                        AlarmState = states.Value.Split('/')[0] == "Alarm" ? AlarmState.Alarm : AlarmState.Normal;
                        if(AlarmState == AlarmState.Alarm)
                          GetDSMEventParamters(eventlog.EventlogDateTime);
                    }
                }
            }
            private void GetDSMEventParamters(DateTime dtEvenLogTime)
            {
                if (Person == string.Empty && Name.Length > 0)
                {
                    DateTime dateStart = dtEvenLogTime.AddMinutes(-3);
                    var filterSettings = new List<FilterSettings>
                    {
                       new FilterSettings(
                          Eventlog.COLUMN_TYPE,
                           new List<string>
                           {
                               Eventlog.TYPEDSMNORMALACCESS,
                           },
                           ComparerModes.EQUALL),

                        new FilterSettings(
                            Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                            dateStart,
                            ComparerModes.EQUALLMORE),

                        new FilterSettings(
                            Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                            dtEvenLogTime,
                            ComparerModes.EQUALLLESS),
                    };

                    var dsmevents = Eventlogs.Singleton.SelectByCriteria(filterSettings);
                    if (dsmevents != null)
                    {
                        var dsmevents2 = dsmevents.ToList().OrderByDescending(d => d.EventlogDateTime);
                        foreach (var dsm in dsmevents2)
                        {
                            if (GetDCUPersonFromDSMSources(dsm))
                                break;
                        }
                    }
                }
            }

            public bool  GetDCUPersonFromDSMSources(Eventlog dsm)
            {
                if (dsm != null && dsm.EventSources == null)
                {
                    dsm = Eventlogs.Singleton.GetObjectById(dsm.IdEventlog);
                }

                string dcuName = string.Empty;
                Person person = null;

                if (dsm.EventSources != null)
                {
                    foreach (var eventsource in dsm.EventSources)
                    {
                        if (dcuName == string.Empty)
                        {
                            var temp = CentralNameRegisters.Singleton.GetNameFromId(eventsource.EventSourceObjectGuid);
                            temp = Regex.Replace(temp, @"\s", "");
                            if (temp != string.Empty && temp == _trimName)
                                dcuName = _trimName;
                        }

                        if (person == null && CentralNameRegisters.Singleton.GetObjectTypeFromGuid(eventsource.EventSourceObjectGuid) == ObjectType.Person)
                            person = Persons.Singleton.GetObjectById(eventsource.EventSourceObjectGuid);

                        if (dcuName != string.Empty && person != null)
                        {
                            Person=person.ToString();
                            PersonId = person.Identification ?? " ";
                            return true;
                        }
                    }

                    return false;
                }
                return false;
            }
        }

        public ICollection<string> Columns => new List<string> {"ColumnParent",
                                                                "ColumnName",
                                                                "ColumnPerson",
                                                                "ColumnPersonID",
                                                                "ColumnDateTimeCreated",
                                                                "ColumnDuration" };
                                                              
        public bool UseSection => false;

         private DateTime dtStart;

        public DoorEnvironmentAlarmExportData(DateTime dtEvent)
        {
            dtStart = dtEvent;
        }

        private IList<IRefExportObject> GetReferencedAlarms(out Exception error)
        {
            List<IRefExportObject> refAlarms = new List<IRefExportObject>();
            error = null;
            try
            {
                var filterSettings = new List<FilterSettings>
                {
                   new FilterSettings(
                            Eventlog.COLUMN_TYPE,
                            new List<string>
                            {
                               Eventlog.TYPEALARMOCCURED,
                               Eventlog.TYPEACTALARMACKNOWLEDGED
                            },
                            ComparerModes.IN),

                    new FilterSettings(
                        Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                        dtStart,
                        ComparerModes.EQUALLMORE),

                    new FilterSettings(
                        Eventlog.COLUMN_DESCRIPTION,
                        "DoorEnvironment_DoorAjar",
                        ComparerModes.LIKEBOTH),
                };

                var eventlogs = Eventlogs.Singleton.SelectByCriteria(filterSettings);
                if (eventlogs != null)
                {
                    Dictionary<string, DoorAjar> _dic = new Dictionary<string, DoorAjar>();
                    foreach (var eventlog in eventlogs)
                    {
                        var doorAjar = new DoorAjar(eventlog);
                        if (doorAjar.Name.Length > 0)
                        {
                            if (_dic.ContainsKey(doorAjar.Name))
                            {
                                if (doorAjar.AlarmState == AlarmState.Normal)
                                {
                                    _dic[doorAjar.Name].dtEnded = doorAjar.dtCreated;
                                    refAlarms.Add(new DoorAjarAlarmAttribute(_dic[doorAjar.Name].Name, _dic[doorAjar.Name].Parent,
                                                      _dic[doorAjar.Name].Person, _dic[doorAjar.Name].PersonId,
                                                      _dic[doorAjar.Name].dtCreated, _dic[doorAjar.Name].dtEnded));

                                    _dic.Remove(doorAjar.Name);
                                }

                                if (doorAjar.AlarmState == AlarmState.Alarm)
                                {
                                    refAlarms.Add(new DoorAjarAlarmAttribute(_dic[doorAjar.Name].Name, _dic[doorAjar.Name].Parent,
                                                   doorAjar.Person, doorAjar.PersonId,
                                                   _dic[doorAjar.Name].dtCreated, _dic[doorAjar.Name].dtEnded));

                                    _dic[doorAjar.Name] = doorAjar;
                                }
                            }
                            else if (doorAjar.AlarmState == AlarmState.Alarm)
                            {
                                _dic[doorAjar.Name] = doorAjar;
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                error = ex;
            }

            return refAlarms;
        }

        public void AddReferencesToDataTable(ref DataTable table, out Exception error)
        {
            foreach (var refAlarm in GetReferencedAlarms(out error))
            {
                DataRow dr = table.NewRow();
                refAlarm.FillDataRow(0, ref dr);
                table.Rows.Add(dr);
            }
        }

    }
}
