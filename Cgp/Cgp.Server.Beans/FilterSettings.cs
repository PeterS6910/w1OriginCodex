using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public enum ComparerModes
    {
        EQUALL,
        NOTEQUALL,
        LIKE,
        LIKEBOTH,
        MORE,
        LESS,
        EQUALLLESS,
        EQUALLMORE
    }

    [Serializable()]
    public class FilterSettings
    {
        private string _column = string.Empty;
        private object _value = null;
        private ComparerModes _comparerMode;

        public string Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ComparerModes ComparerMode
        {
            get { return _comparerMode; }
            set { _comparerMode = value; }
        }

        public FilterSettings(string column, object value, ComparerModes comparerMode)
        {
            _column = column;
            _value = value;
            _comparerMode = comparerMode;
        }
    }
}
