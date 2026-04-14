using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Mail;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Contal.Cgp.Server
{
    public class ExcelHelper
    {
        public string _FileName;
        public string _DateFormat;
        public string _EmailToSend;

        Dictionary<long, ICollection<string>> _EventSourceNames;

        public event DVoid2Void OnGenerateFinished;

        private void BuildEventSources(ICollection<Eventlog> eventLogs)
        {
            try
            {
                _EventSourceNames = new Dictionary<long, ICollection<string>>();

                if (eventLogs.Count > 0)
                    _EventSourceNames =
                        Eventlogs.Singleton
                            .GetEventSourceNames(
                                eventLogs
                                    .Where(eventlog => eventlog != null)
                                    .Select(eventlog => eventlog.IdEventlog)
                                    .ToList());

            }
            catch (Exception)
            {
            }
        }

        private string GetEventSources(Eventlog eventlog)
        {
            if (_EventSourceNames == null || eventlog == null)
                return string.Empty;

            var result = new StringBuilder();

            ICollection<string> eventLogNames;

            if (!_EventSourceNames.TryGetValue(eventlog.IdEventlog, out eventLogNames))
                return string.Empty;

            if (eventLogNames == null)
                return string.Empty;

            foreach (string name in eventLogNames)
                result.Append(name + ",");

            if (result[result.Length - 1] == ',')
                result.Length = result.Length - 1;

            return result.ToString();
        }

        public void GenerateExcelFile(ICollection<Eventlog> eventLogs)
        {
            Eventlogs.Singleton.LastReportFile = null;

            FileInfo exportedFile = new FileInfo(_FileName);
            if (exportedFile.Exists)
            {
                exportedFile.Delete();  // ensures we create a new workbook
                exportedFile = new FileInfo(_FileName);
            }

            BuildEventSources(eventLogs);

            using (ExcelPackage excelPackage = new ExcelPackage(exportedFile))
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Eventlog");

                //Add the headers
                worksheet.Cells[1, 1].Value = "Type";
                worksheet.Cells[1, 2].Value = "Date";
                worksheet.Cells[1, 3].Value = "Event sources";
                worksheet.Cells[1, 4].Value = "Description";

                // Add data...
                int index = 2;
                string cellPos = string.Empty;
                foreach (Eventlog eventlog in eventLogs)
                {
                    cellPos = string.Format("A{0}", index);
                    worksheet.Cells[cellPos].Value = eventlog.Type;

                    cellPos = string.Format("B{0}", index);
                    worksheet.Cells[cellPos].Value = eventlog.EventlogDateTime.ToString(_DateFormat);

                    cellPos = string.Format("C{0}", index);
                    worksheet.Cells[cellPos].Value = GetEventSources(eventlog);

                    cellPos = string.Format("D{0}", index);
                    worksheet.Cells[cellPos].Value = eventlog.Description;

                    index++;
                }

                // Format the values
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                // Create an autofilter for the range
                worksheet.Cells[string.Format("A1:D{0}", index)].AutoFilter = true;

                worksheet.Cells[string.Format("A2:D{0}", index)].Style.Numberformat.Format = "@";   //Format as text

                //worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&14&\"Arial,Regular Bold\" Eventlog";
                // add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.OddFooter.CenteredText = string.Format("{0} / {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

                worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                // Change the sheet view to show it in page layout mode
                worksheet.View.PageLayoutView = true;

                // set some document properties
                excelPackage.Workbook.Properties.Title = "Eventlog";
                excelPackage.Workbook.Properties.Author = "Contal Nova Server";
                excelPackage.Workbook.Properties.Comments = "";

                // set some extended property values
                excelPackage.Workbook.Properties.Company = "Contal OK";

                // set some custom property values
                excelPackage.Workbook.Properties.SetCustomPropertyValue("Checked by", "admin");
                excelPackage.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "Contal Nova Server");

                // save our new workbook and we are done!
                excelPackage.Save();
            }

            if (OnGenerateFinished != null)
            {
                // Send email with report
                OnGenerateFinished();
            }
            else
            {
                // Remember the file (e.g. for transferring to Client)
                Eventlogs.Singleton.LastReportFile = _FileName;
            }
        }

        public void SendEmail()
        {
            try
            {
                if (!GeneralOptions.Singleton.IsSetSMTP())
                    return;

                using (MailMessage mail = new MailMessage())
                {
                    using (SmtpClient smtpClient = new SmtpClient(GeneralOptions.Singleton.SmtpServer))
                    {
                        mail.From = new MailAddress(GeneralOptions.Singleton.SmtpSourceEmailAddress);
                        mail.To.Add(_EmailToSend);
                        mail.Subject = "Contal Nova Eventlog Report";
                        mail.Body = "Contal Nova Eventlog Report is attached";

                        Attachment attachment = new Attachment(_FileName);
                        mail.Attachments.Add(attachment);

                        smtpClient.Port = GeneralOptions.Singleton.SmtpPort;
                        smtpClient.EnableSsl = GeneralOptions.Singleton.SmtpSsl;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        if (String.IsNullOrEmpty(GeneralOptions.Singleton.SmtpCredentials))
                        {
                            smtpClient.UseDefaultCredentials = true;
                        }
                        else
                        {
                            smtpClient.UseDefaultCredentials = false;
                            string[] credentials = GeneralOptions.Singleton.SmtpCredentials.Split('|');
                            smtpClient.Credentials = new System.Net.NetworkCredential(credentials[0], credentials[1]);
                        }

                        smtpClient.Send(mail);
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                // Delete report file after sending
                File.Delete(_FileName);
            }
        }
    }
}
