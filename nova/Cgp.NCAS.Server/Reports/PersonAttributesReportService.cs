
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ExportData;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.ExportData;

using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform;
using Contal.IwQuick.Threads;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.NCAS.Server.Reports
{
    sealed public class PersonAttributesReportService :ASingleton<PersonAttributesReportService>
    {
        private Cgp.Server.Beans.TimeZone _timeZone;
        private PersonAttributeOutput _personattrOutput;
        string logFile = @"C:\Contal Nova\AccessDenied.log";
        private const string dateFormat = "ddMMyyyy";
        private PersonAttributesReportService() : base(null)
        {
        }

        //private void AddLogMessage(string message)
        //{
        //    if (!File.Exists(logFile))
        //    {
        //        // Create a file to write to.
        //        using (StreamWriter sw = File.CreateText(logFile))
        //        {
        //            sw.WriteLine(message);
        //        }
        //    }
        //    else
        //    {
        //        using (StreamWriter sw = File.AppendText(logFile))
        //        {
        //            sw.WriteLine(message);
        //        }
        //    }

        //}

        public  void  Initialize()
        {
            _personattrOutput = PersonAttributeOutputs.Singleton.GetPersonAttributeOutput();
            if (_personattrOutput!=null && _personattrOutput.IdTimeZone!=null && _personattrOutput.IdTimeZone != Guid.Empty)
                _timeZone = TimeZones.Singleton.GetObjectById(_personattrOutput.IdTimeZone);
            else
                _timeZone = null;
        }

        public void Start()
        {
            Initialize();
            PersonAttributeOutputs.Singleton.OnChanged += SetChanges;
            TimeAxis.Singleton.TimeZoneStateChanged += TimeZoneReportsStateChanged;
            //AddLogMessage("Start Service");
        }

        private void SetChanges()
        {
            Initialize();
           // AddLogMessage("Service apply change");
        }

        private void TimeZoneReportsStateChanged(Guid arg1, byte status)
        {
            if (_timeZone != null && _timeZone.IdTimeZone == arg1)
            {
                if (status == 1)
                {
                   // AddLogMessage("Time zone is ON, Make access denied report");
                    MakeReport();
                }
            }
        }

        private bool MakeReport()
        {
            if (_personattrOutput != null && _personattrOutput.IsEnabled)
            {
                DateTime dtToday = DateTime.Now.Date;
                DateTime dtStart = dtToday.Date;
                var parts = _personattrOutput.Output.Split('#');
                if (parts != null && parts.Length > 1)
                {
                    string filename = parts[0] + @"\" + parts[1] + "_";
                    DateTime dtEnd;
                    switch (_personattrOutput.Interval)
                    {
                        case 0://each day
                            filename = filename + dtStart.ToString(dateFormat) + ".xlsx";
                            break;
                        case 1: //each month
                            dtStart = new DateTime(dtToday.Year, dtToday.Month, 1);
                            dtEnd = new DateTime(dtToday.Year, dtToday.Month, DateTime.DaysInMonth(dtToday.Year, dtToday.Month));
                            filename = filename + dtStart.ToString(dateFormat) + "-" + dtEnd.ToString(dateFormat) + ".xlsx";
                            break;
                        case 2: //each year
                            dtStart = new DateTime(dtToday.Year, 1, 1);
                            dtEnd = new DateTime(dtToday.Year, 12, 31);
                            filename = filename + dtStart.ToString(dateFormat) + "-" + dtEnd.ToString(dateFormat) + ".xlsx";
                            break;
                    }

                    bool bFillSection;
                    var personAttributeExportData = new PersonAttributeExportData(dtStart);

                    var exportTable = new ExportTable<Persons, Person>(personAttributeExportData)
                       .ExportData(null, out bFillSection);

                    try
                    {

                        if (exportTable != null && exportTable.Rows.Count > 0)
                        {
                            if (!File.Exists(filename))
                            {
                                //Create new Excel File;
                                ExcelCreator.CreateExcelFile(filename, exportTable, bFillSection);
                            }
                            else
                            {
                                ExcelCreator.AppendExcelFile(filename, exportTable, bFillSection);
                            }

                            IExcelExportEvent excelExportEvent = personAttributeExportData;
                            if (excelExportEvent != null)
                            {
                                var excelEvent = excelExportEvent.GetExportedEvent();
                                foreach (var e in excelEvent)
                                {
                                    ConsecutiveEvents.Singleton.InsertOrUpdateEventExport(e);
                                }
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        return false;
                    }
                    finally
                    {
                    }
                    return true;
                }
            }
            return true;
        }
    }
 }
