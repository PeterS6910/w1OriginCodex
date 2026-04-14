using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Contal.Cgp.BaseLib
{
    [Serializable()]
    public enum ComparerModes
    {
        EQUALL,//values which are equall with input 
        NOTEQUALL,//values which are not equall with input 
        LIKE,//values which match with input characters from start
        LIKEBOTH,//values which match with input characters from any position
        MORE,//values which are higher than input
        LESS,//values which are lower than input
        EQUALLLESS,//values which are equal or lower than input
        EQUALLMORE,//values which are equal or higher than input
        IN
    }

    [Serializable()]
    public enum LogicalOperators
    {
        AND,
        OR
    }

    [Serializable()]
    public class FilterSettings
    {
        private string _column = string.Empty;            
        private object _value = null;
        private ComparerModes _comparerMode;
        private LogicalOperators _logicalOperator;

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

        public LogicalOperators LogicalOperator
        {
            get { return _logicalOperator; }
            set { _logicalOperator = value; }
        }

        public FilterSettings(string column, object value, ComparerModes comparerMode)
        {
            _column = column;
            _value = value;
            _comparerMode = comparerMode;
            _logicalOperator = LogicalOperators.AND;
        }

        public FilterSettings(string column, object value, ComparerModes comparerMode, LogicalOperators logicalOperator)
        {
            _column = column;
            _value = value;
            _comparerMode = comparerMode;
            _logicalOperator = logicalOperator;
        }  
    }
}
