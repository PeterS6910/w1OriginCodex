using System.Collections.Generic;
using System.Data.SqlClient;

namespace Contal.Cgp.Server.DB
{
    public abstract class DbCommandFactory
    {
        protected abstract string Text
        {
            get;
        }

        protected abstract IEnumerable<SqlParameter> Parameters
        {
            get;
        }

        public SqlCommand CreateCommand(SqlConnection dbConnection)
        {
            var result = 
                new SqlCommand(
                    Text,
                    dbConnection);

            foreach (SqlParameter sqlParameter in Parameters)
                result.Parameters.Add(sqlParameter);

            return result;
        }
    }
}