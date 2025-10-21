using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Globals
{
    public static  class ExcelCreator
    {
        private  class ExcelColumnGetter
        {
            private static readonly Dictionary<int, string> _column = new Dictionary<int, string>()
            {
                {1,"A"}, {2,"B"}, {3,"C"},{4,"D"},{5,"E"},{6,"F"},{7,"G"},{8,"H"},{9,"I"}, {10,"J"},
                {11,"K"}, {12,"L"}, {13,"M"}, {14,"N"},{15,"O"},{16,"P"},{17,"Q"},{18,"R"},{19,"S"},{20,"T"}
            };
            public string this[int key]
            {
                get
                {
                    if(_column.ContainsKey(key))
                        return _column[key];

                    return "Z";
                }
            }
        }

        private static readonly ExcelColumnGetter ExcelColumn = new ExcelColumnGetter();

        public static void CreateExcelFile(string fileName, DataTable dataTable, bool bFillSection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo exportedFile = new FileInfo(fileName);
            if (exportedFile.Exists)
            {
                exportedFile.Delete();  // ensures we create a new workbook
                exportedFile = new FileInfo(fileName);
            }

            using (ExcelPackage excelPackage = new ExcelPackage(exportedFile))
            {
                // Add a new worksheet to the empty workbook
                using (ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Export Data"))
                {

                    int colCount = dataTable.Columns.Count;

                    for (int c = 0; c < colCount; c++)
                    {
                        worksheet.Cells[1, c + 1].Value = dataTable.Columns[c].ColumnName;
                    }

                    // Add data...
                    int r = 2;
                    AppendRows(worksheet, r, dataTable, bFillSection);

                    // Format the values of HEAD
                    using (var range = worksheet.Cells[1, 1, 1, colCount])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                        range.Style.Font.Color.SetColor(Color.White);
                    }


                    excelPackage.Save();
                }
            }
        }

        public static void AppendExcelFile(string fileName, DataTable dataTable, bool bFillSection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                var ws = excelPackage.Workbook.Worksheets.First();
                var lastRow = ws.Dimension.End.Row;

                while ((ws.Cells[lastRow, 1].Value == null || ws.Cells[lastRow, 1].Value.ToString().Length < 1) && lastRow > 1)
                {
                    lastRow--;
                }
                AppendRows(ws,lastRow + 1, dataTable, bFillSection);
                excelPackage.Save();
            }
        }

        public static DateTime  ReadDateTimeRowValue(string fileName, int column, string format)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                var ws = excelPackage.Workbook.Worksheets.First();
                var lastRow = ws.Dimension.End.Row;
                while ((ws.Cells[lastRow, column].Value == null || ws.Cells[lastRow, column].Value.ToString().Length < 2) && lastRow > 1)
                {
                    lastRow--;
                }
                var val = ws.Cells[lastRow, column].Value.ToString();
                DateTime result;
                long dateNum;
                if(long.TryParse(ws.Cells[lastRow, column].Value.ToString(), out dateNum))
                {
                    result = DateTime.FromOADate(dateNum);
                }
                else
                {
                    if(!DateTime.TryParseExact(val, format, CultureInfo.InvariantCulture,DateTimeStyles.None, out result))
                        result=DateTime.MinValue;
                }
                return result;
            }
        }
        private static void AppendRows(ExcelWorksheet worksheet, int  r, DataTable dataTable, bool bFillSection)
        {
            int colCount = dataTable.Columns.Count;
            foreach (DataRow row in dataTable.Rows)
            {
                for (int c = 0; c < colCount; c++)
                {
                    worksheet.Cells[r, c + 1].Value = row[c].ToString();
                }
                r++;
            }

            //Format
            if (dataTable.Rows.Count > 0)
            {
                var rangeSection = worksheet.Cells[string.Format("A2:A{0}", r)];
                rangeSection.Style.Font.Bold = true;
                if (bFillSection)
                {
                    foreach (var section in rangeSection.Where(s => s.Value!=null &&  s.Value.ToString().Length == 0))
                    {
                        worksheet.Cells[string.Format("A{0}:{2}{1}", section.Start.Row, section.Start.Row, ExcelColumn[colCount])]
                            .Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[string.Format("A{0}:{2}{1}", section.Start.Row, section.Start.Row, ExcelColumn[colCount])]
                            .Style.Fill.BackgroundColor.SetColor(Color.FromArgb(247, 247, 250));
                    }
                }
                worksheet.Cells[string.Format("A1:{0}{1}", ExcelColumn[colCount], r)].AutoFitColumns();

                var ie = worksheet.IgnoredErrors.Add(worksheet.Cells[string.Format("A2:{0}{1}", ExcelColumn[colCount], r)]);
                ie.NumberStoredAsText = true;
            }
        }


    }
}
