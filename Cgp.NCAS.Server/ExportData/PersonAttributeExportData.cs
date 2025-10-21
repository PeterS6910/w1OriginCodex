using Contal.Cgp.BaseLib;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.ExportData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OfficeOpenXml.ExcelErrorValue;

namespace Contal.Cgp.NCAS.Server.ExportData
{
    public  class PersonAttributeExportData : IRefExportData<Person>, IExcelExportEvent
    {
        private class Counter
        {
            public int count = 0;
            public DateTime dtFirst = DateTime.MinValue;
        }

        public ICollection<string> Columns => new List<string> {"ColumnPerson",
                                                                "ColumnPersonID",
                                                                "ColumnCardNumber",
                                                                "ColumnCardReader",
                                                                "ColumnAccessDenied",
                                                                "ColumnEventlogDateTimeFirst",
                                                                "ColumnEventlogDateTimeLast" };

        public bool UseSection => false;

        private  DateTime dtStart;
        private readonly PersonAttributeOutput perAttr;
        public PersonAttributeExportData(DateTime dtEvent)
        {
            dtStart = dtEvent;
            perAttr = PersonAttributeOutputs.Singleton.GetPersonAttributeOutput();
        }

        private IList<IRefExportObject> GetReferencedPersons(Person ormObj, out Exception error)
        {
            DateTime dtEventlogFrom = dtStart;
            List<IRefExportObject> refPersons = new List<IRefExportObject>();
            error = null;
            try
            {
                var filterSettings = new List<FilterSettings>
               {
                    new FilterSettings(
                        Card.COLUMNPERSON,
                        ormObj,
                        ComparerModes.EQUALL),
               };
                var cards = Cards.Singleton.SelectByCriteria(filterSettings);
                foreach(var card in cards)
                {
                    var ExportedEventsForCard = ConsecutiveEvents.Singleton.GetLastExportedEventForCardSourceId(card.IdCard);
                    if (ExportedEventsForCard.Count > 0)
                    {
                        dtEventlogFrom = ExportedEventsForCard.Values.OrderByDescending(d => d).First().AddSeconds(2);
                      //  if (dtEventlogFrom < dtStart)
                           // dtEventlogFrom = dtStart; //if for each day, then event from prevous days will not written in the current file
                    }

                    var items= ConsecutiveEvents.Singleton.FilterEventsByCard(dtEventlogFrom, card.IdCard);

                    Dictionary<Guid, Counter> _dicCount = new Dictionary<Guid, Counter>();

                    foreach(var item in items)
                    {
                        if (item.AccessDeniedType)
                        {
                            bool bPassDateTime = true;
                            if (ExportedEventsForCard.ContainsKey(item.ReasonId))
                            {
                                bPassDateTime = item.EventDateTime.Value > ExportedEventsForCard[item.ReasonId];
                            }

                            if (bPassDateTime)
                            {
                                if (_dicCount.ContainsKey(item.ReasonId))
                                {
                                    if (ConsecutiveEvents.Singleton.InMinutes(_dicCount[item.ReasonId].dtFirst, item.EventDateTime.Value, 60 * 12))
                                        _dicCount[item.ReasonId].count += 1;
                                    else
                                    {
                                        _dicCount[item.ReasonId].count = 1;
                                        _dicCount[item.ReasonId].dtFirst = item.EventDateTime.Value;
                                    }

                                }
                                else
                                {
                                    _dicCount[item.ReasonId] = new Counter { count = 1, dtFirst = item.EventDateTime.Value };
                                }
                            }
                        }

                        if(_dicCount.ContainsKey(item.ReasonId) && _dicCount[item.ReasonId].count == perAttr.FailsCount)
                        {
                            var cr=CardReaders.Singleton.GetById(item.ReasonId);
                            refPersons.Add(new RefPersonAttribute(ormObj.ToString(),ormObj.Identification ?? "", cr.ToString(), card.Number,
                                                                  _dicCount[item.ReasonId].dtFirst, item.EventDateTime.Value, perAttr.FailsCount));
                            _dicCount.Remove(item.ReasonId);

                            AddExportedEvent(0, card.IdCard, item.ReasonId, item.EventDateTime.Value);

                            ExportedEventsForCard[item.ReasonId] = item.EventDateTime.Value; //datetime for the  last reported to excel event for pair reader + card 
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                error = ex;
            }

            return refPersons;
        }

        public void AddReferencesToDataTable(Person ormObj, ref DataTable table, out Exception error)
        {
            foreach (var refPerson in GetReferencedPersons(ormObj, out error))
            {
                DataRow dr = table.NewRow();
                refPerson.FillDataRow(0, ref dr);
                table.Rows.Add(dr);
            }
        }

        public void AddExportedEvent(int id, Guid sourceId, Guid reasonId, DateTime eventDateTime)
        {

           var itemIndex = _listExportEvent.FindIndex(e => e.SourceId == sourceId && e.ReasonId == reasonId);
            if(itemIndex > -1)
            {
                _listExportEvent.ElementAt(itemIndex).UpdateDateTime(eventDateTime);
            }
            else
            {
                _listExportEvent.Add(new ConsecutiveEvent(id, sourceId, reasonId, eventDateTime));
            }

        }

        public List<ConsecutiveEvent> GetExportedEvent()
        {
            return _listExportEvent;
        }

        private List<ConsecutiveEvent> _listExportEvent=new List<ConsecutiveEvent>();
    }
}
