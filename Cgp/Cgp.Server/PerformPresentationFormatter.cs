using System;
using System.Globalization;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Server
{
    class PerformPresentationFormatter
    {
        const string SUBSTITUTE_THIS = "%msg";

        //string _formatSettings;        

        string _partBefore;
        string _partAfter;

        public string FormatterName { get; set; }


        /// <summary>
        /// Create Presentation Formater via Database
        /// </summary>
        /// <param name="idFormatter">formatter Guid</param>
        public PerformPresentationFormatter(Guid idFormatter)
        {
            PresentationFormatter formatter = DB.PresentationFormatters.Singleton.GetById(idFormatter);
            
            FormatterName = formatter.FormatterName;

            SetFormatterVariable(formatter.MessageFormat);

        }

        public PerformPresentationFormatter(string formatterName, string formatType)
        {
            Validator.CheckNullString(formatterName);
            Validator.CheckNullString(formatType);
            FormatterName = formatterName;
            SetFormatterVariable(formatType);
        }

        private void SetFormatterVariable(string formatType)
        {
            Validator.CheckNullString(formatType);

            //_formatSettings = formatType;
            _partBefore = string.Empty;
            _partAfter = string.Empty;

            CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
            int whereStarts = Compare.IndexOf(formatType, SUBSTITUTE_THIS, CompareOptions.IgnoreCase);
            if (whereStarts > 0)
            {
                _partBefore = formatType.Substring(0, whereStarts);
            }
            int reminder = formatType.Length - whereStarts - SUBSTITUTE_THIS.Length;
            if (reminder > 0)
            {
                _partAfter = formatType.Substring(whereStarts + SUBSTITUTE_THIS.Length, reminder);
            }
        }

        public bool CreateFormation(string inString, out string outString)
        {
            string result = string.Empty;
            result += _partBefore;
            result += inString;
            result += _partAfter;
            outString = result;
            return true;
        }

        public string FormateString(string inString)
        {
            string result = string.Empty;
            result += _partBefore;
            result += inString;
            result += _partAfter;
            return result;
        }
    }
}
