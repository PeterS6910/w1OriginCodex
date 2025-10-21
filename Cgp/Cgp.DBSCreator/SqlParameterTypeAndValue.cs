using System.Data;

namespace Contal.Cgp.DBSCreator
{
    public class SqlParameterTypeAndValue
    {
        public SqlDbType? Type;
        public object Value;

        /// <summary>
        /// Type must be defined when Value is null
        /// </summary>
        /// <param name="value"></param>
        public SqlParameterTypeAndValue(object value)
            : this(null, value)
        {
        }

        /// <summary>
        /// Type must be defined when Value is null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public SqlParameterTypeAndValue(SqlDbType? type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}