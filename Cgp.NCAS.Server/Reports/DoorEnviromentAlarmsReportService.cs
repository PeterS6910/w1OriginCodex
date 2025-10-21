
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ExportData;
using Contal.Cgp.Server;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.ExportData;

using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform;
using Contal.IwQuick.Threads;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Contal.Cgp.NCAS.Server.Reports
{
    sealed public class DoorEnviromentAlarmsReportService : ASingleton<DoorEnviromentAlarmsReportService>
    {
        private Cgp.Server.Beans.TimeZone _timeZone;
        private ExcelReportOutput _doorAlarmReportSettings;
        private const string dateFormat = "ddMMyyyy";
        private DoorEnviromentAlarmsReportService() : base(null)
        {
        }


        public  void  Initialize()
        {
            _doorAlarmReportSettings = ExcelReportOutputs.Singleton.GetSettings();
            if (_doorAlarmReportSettings != null && _doorAlarmReportSettings.TimeZone != null && _doorAlarmReportSettings.TimeZone.IdTimeZone != null && _doorAlarmReportSettings.TimeZone.IdTimeZone != Guid.Empty)
                _timeZone = TimeZones.Singleton.GetObjectById(_doorAlarmReportSettings.TimeZone.IdTimeZone);
            else
                _timeZone = null;
        }

        public void Start()
        {
            Initialize();
            ExcelReportOutputs.Singleton.OnChanged += SetChanges;
            TimeAxis.Singleton.TimeZoneStateChanged += TimeZoneReportsStateChanged;
        }

        private void SetChanges()
        {
            Initialize();
        }

        private void TimeZoneReportsStateChanged(Guid arg1, byte status)
        {
            if (_timeZone != null && _timeZone.IdTimeZone == arg1)
            {
                if (status == 1)
                {
                    MakeReport();
                }
            }
        }

        private bool MakeReport()
        {
            if (_doorAlarmReportSettings != null && _doorAlarmReportSettings.IsEnabled)
            {
                DateTime dtToday= DateTime.Now.Date;
                DateTime dtStart = dtToday.Date;
                string filename = _doorAlarmReportSettings.Output + @"\" + _doorAlarmReportSettings.Filename + @"_";
                DateTime dtEnd;
                switch (_doorAlarmReportSettings.Interval)
                {
                    case 0://each day
                        filename= filename+ dtStart.ToString(dateFormat) + ".xlsx";
                        break;
                    case 1: //each month
                        dtStart = new DateTime(dtToday.Year, dtToday.Month, 1);
                        dtEnd = new DateTime(dtToday.Year, dtToday.Month, DateTime.DaysInMonth(dtToday.Year, dtToday.Month));
                        filename = filename + dtStart.ToString(dateFormat) + "-" + dtEnd.ToString(dateFormat) + ".xlsx";
                        break;
                    case 2: //each year
                        dtStart = new DateTime(dtToday.Year, 1, 1);
                        dtEnd = new DateTime(dtToday.Year, 12,31);
                        filename = filename + dtStart.ToString(dateFormat) + "-" + dtEnd.ToString(dateFormat) + ".xlsx";
                        break;
                }


                bool bFillSection;
                //AddLogMessage("Report name is "+filename);
                if (File.Exists(filename))
                {
                   var  dtStart2 = ExcelCreator.ReadDateTimeRowValue(filename, 5, "yyyy/MM/dd/HH:mm:ss");
                    if( dtStart2 != DateTime.MinValue)
                    {
                        dtStart= dtStart2.AddSeconds(1);
                    }
                }

                    var exportTable = new ExportTable(new DoorEnvironmentAlarmExportData(dtStart))
                       .ExportData(out bFillSection);

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
            return true;
        }
    }
 }
