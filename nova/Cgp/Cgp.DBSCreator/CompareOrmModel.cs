using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Data.SqlClient;

namespace Contal.Cgp.DBSCreator
{
    internal class CompareOrmModel
    {
        #region Variables

        private readonly DatabaseCommandExecutor _databaseCommandExecutor;
        private readonly CreatorProperties _creatorProperties;

        private IEnumerable<Schema.TableInfo> _ormSchema;

        public event IwQuick.DException2Void OnError;

        #endregion

        public CompareOrmModel(DatabaseCommandExecutor databaseCommandExecutor)
        {
            _databaseCommandExecutor = databaseCommandExecutor;
            _creatorProperties = databaseCommandExecutor.CreatorProperties;

            DatabaseErrors = new Schema.DatabaseErrors();

            ModelOk = true;
        }

        #region Properties

        public Schema.DatabaseErrors DatabaseErrors { get; private set; }

        public bool ModelOk { get; private set; }

        #endregion

        public void RunCompareAll()
        {
            try
            {
                if (!_creatorProperties.IsDbsVariableOk())
                    throw new Exception(
                        "CompareOrmModel: connection string or connect data not found");

                if (_creatorProperties.Assemblies == null)
                    throw new Exception(
                        "CompareOrmModel: Assemblies not obtained");

                ModelOk = CompareMainDatabase() && CompareEventlogTables();
            }
            catch (Exception ex) //catch (SqlException ex)
            {
                ModelOk = false;

                if (OnError != null)
                    OnError(ex);
            }
        }

        private bool CompareEventlogTables()
        {
            try
            {
                return
                    _databaseCommandExecutor.TableExists(
                        "EventLog", true) &&
                    _databaseCommandExecutor.TableExists(
                        "EventSource", true) &&
                    _databaseCommandExecutor.TableExists(
                        "EventlogParameter", true);
            }
            catch
            {
                return false;
            }
        }

        private bool CompareMainDatabase()
        {
            ObtainOrmSchema();

            var sqlConnection =
                new SqlConnection
                {
                    ConnectionString =
                        _creatorProperties.GetConnectionStringLoginToDatabase()
                };

            try
            {
                sqlConnection.Open();

                return CompareSchema(_ormSchema, sqlConnection);
            }
            catch
            {
                return false;
            }
            finally
            {
                sqlConnection.Dispose();
            }
        }

        private void ObtainOrmSchema()
        {
            _ormSchema =
                new LinkedList<Schema.TableInfo>(
                    _creatorProperties.Assemblies
                        .Select(assembly =>
                            assembly
                                .GetManifestResourceNames()
                                .Where(fileName => fileName.Contains("hbm.xml"))
                                .Select(fileName =>
                                    ObtainOrmTable(
                                        assembly.GetManifestResourceStream(fileName))))
                        .SelectMany(tables => tables));
        }

        /// <summary>
        /// from the xml file fill the _tableOrm (the object that contain columns and his constraints)
        /// </summary>
        /// <param name="streamXml">xml resource stream</param>
        private static Schema.TableInfo ObtainOrmTable(Stream streamXml)
        {
            var reader = new XmlTextReader(streamXml);

            try
            {
                reader.MoveToContent();

                if (!reader.IsStartElement() || reader.IsEmptyElement)
                    return null;

                reader.ReadStartElement("hibernate-mapping");

                if (!reader.IsStartElement("class"))
                    return null;

                Schema.TableInfo result = ReadClassElement(reader);

                reader.ReadEndElement();

                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }

        private static Schema.TableInfo ReadClassElement(XmlTextReader reader)
        {
            var emptyClassElement = reader.IsEmptyElement;

            var result = new Schema.TableInfo();

            string className = string.Empty;
            string tableName = null;

            while (reader.MoveToNextAttribute()) // Read attributes
                switch (reader.Name)
                {
                    case "table":
                        tableName = reader.Value;
                        break;

                    case "name":
                    {
                        var parts = reader.Value.Split(',')[0].Split('.');
                        className = parts[parts.Length - 1];

                        break;
                    }
                }

            result.Name =
                string.IsNullOrEmpty(tableName)
                    ? className
                    : tableName;

            if (emptyClassElement)
                return result;

            reader.Read();
            reader.MoveToContent();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (!reader.IsStartElement())
                {
                    reader.Skip();
                    reader.MoveToContent();

                    continue;
                }

                ReadInnerClassElement(reader, result);

                reader.MoveToContent();
            }

            reader.ReadEndElement();
            return result;
        }

        private static void ReadInnerClassElement(
            XmlTextReader reader,
            Schema.TableInfo result)
        {
            // loking for table name
            var elementName = reader.Name.ToLower();

            Schema.TableRecord tmpTableRecord;

            switch (elementName)
            {
                case "composite-id":

                    ReadCompositeKey(reader, result);

                    break;

                case "property":

                    result.AddRow(ReadOrmColumnAttributes(reader));
                    break;

                case "id":

                    tmpTableRecord = ReadOrmColumnAttributes(reader);

                    tmpTableRecord.NotNull = true;
                    tmpTableRecord.Unique = true;
                    tmpTableRecord.ColumnType = Schema.ColumnTypeEnum.PrimaryKey;

                    result.AddRow(tmpTableRecord);
                    break;

                case "many-to-one":

                    tmpTableRecord = ReadOrmColumnAttributes(reader);

                    tmpTableRecord.ColumnType = Schema.ColumnTypeEnum.ForeignKey;

                    result.AddRow(tmpTableRecord);
                    break;
            }

            reader.Skip();
        }

        private static void ReadCompositeKey(
            XmlTextReader reader,
            Schema.TableInfo result)
        {
            if (reader.IsEmptyElement)
                return;

            reader.Read();
            reader.MoveToContent();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (!reader.IsStartElement("key-property"))
                {
                    reader.Skip();
                    reader.MoveToContent();

                    continue;
                }

                // loking for composide id columns
                var tmpTableRecord = ReadOrmColumnAttributes(reader);

                tmpTableRecord.NotNull = true;
                tmpTableRecord.Unique = true;

                tmpTableRecord.ColumnType = Schema.ColumnTypeEnum.PrimaryKey;

                result.AddRow(tmpTableRecord);

                reader.Skip();

                reader.MoveToContent();
            }
        }

        private static Schema.TableRecord ReadOrmColumnAttributes(XmlTextReader reader)
        {
            var result = new Schema.TableRecord();

            string column = string.Empty;

            while (reader.MoveToNextAttribute())
                switch (reader.Name.ToLower())
                {
                    case "name":
                        result.Name = reader.Value;
                        break;

                    case "column":
                        column = reader.Value;
                        break;

                    case "not-null":
                        result.NotNull = reader.Value.ToLower() == "true";
                        break;

                    case "unique":
                        result.Unique =
                            reader.Value.ToLower() == "true" ||
                            reader.Value.ToLower() == "1";
                        break;

                    case "unique-key":
                        result.Unique = true;
                        break;
                }

            if (column != string.Empty)
                result.Name = column;

            return result;
        }

        private bool CompareSchema(
            IEnumerable<Schema.TableInfo> ormSchema,
            SqlConnection sqlConnection)
        {
            bool result = true;

            foreach (Schema.TableInfo st in ormSchema)
            {
                Schema.TableInfo sqlTable =
                    GetSqlTable(
                        st.Name,
                        sqlConnection);

                if (sqlTable == null)
                {
                    result = false;
                    DatabaseErrors.AddMissingTable(st.Name);

                    continue;
                }

                Schema.SqlTableError tableError =
                    st.CompareTables(sqlTable);

                if (tableError == null)
                    continue;

                // Dont set wassError to true if error is only unused columns
                if (tableError.HasMissingColumns || tableError.HasWrongColumns)
                    result = false;

                DatabaseErrors.AddTableError(tableError);
            }

            return result;
        }

        /// <summary>
        /// from the database fill the _tableSql (the object that contain columns and his constraints)
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="sqlConnection"></param>
        private static Schema.TableInfo GetSqlTable(
            string tableName,
            SqlConnection sqlConnection)
        {
            var sqlQuery = new StringBuilder();

            sqlQuery.Append("select COLUMN_NAME, IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS");
            sqlQuery.AppendFormat(" where TABLE_NAME = '{0}'", tableName);

            var myCommand =
                new SqlCommand(
                    sqlQuery.ToString(),
                    sqlConnection);

            var sqlReader = myCommand.ExecuteReader();
            var tmpTable = new Schema.TableInfo(tableName);

            if (!sqlReader.HasRows)
            {
                sqlReader.Close();
                return null;
            }

            while (sqlReader.Read())
            {
                var tmpTableRecord =
                    new Schema.TableRecord
                    {
                        Name = sqlReader.GetString(0)
                    };

                if (sqlReader.GetString(1) == "NO")
                    tmpTableRecord.NotNull = true;

                tmpTable.AddRow(tmpTableRecord);
            }

            sqlReader.Close();

            sqlQuery = new StringBuilder("SELECT ");

            sqlQuery.Append("CONSTRAINT_COLUMN_USAGE.COLUMN_NAME, ");
            sqlQuery.Append("TABLE_CONSTRAINTS.CONSTRAINT_TYPE, ");
            sqlQuery.Append("TABLE_CONSTRAINTS.TABLE_NAME ");
            sqlQuery.Append("FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS, ");
            sqlQuery.Append("INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ");

            sqlQuery.Append("WHERE TABLE_CONSTRAINTS.CONSTRAINT_NAME = CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME ");
            sqlQuery.AppendFormat("AND TABLE_CONSTRAINTS.TABLE_NAME = '{0}'", tableName);

            myCommand = new SqlCommand(sqlQuery.ToString(), sqlConnection);
            sqlReader = myCommand.ExecuteReader();

            if (sqlReader.HasRows)
                while (sqlReader.Read())
                {
                    string tmpName = sqlReader.GetString(0);
                    string tmpStr = sqlReader.GetString(1);

                    if (tmpStr == "FOREIGN KEY")
                        tmpTable.UpdateRowColumnType(
                            tmpName,
                            Schema.ColumnTypeEnum.ForeignKey);

                    if (tmpStr == "PRIMARY KEY")
                    {
                        tmpTable.UpdateRowColumnType(
                            tmpName,
                            Schema.ColumnTypeEnum.PrimaryKey);

                        tmpTable.UpdateRowUnique(tmpName, true);
                    }

                    if (tmpStr == "UNIQUE")
                        tmpTable.UpdateRowUnique(tmpName, true);
                }

            sqlReader.Close();
            return tmpTable;
        }
    }
}

#region SQL
/*
 * 
SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
GO
SELECT * FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
GO
SELECT INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME, INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_TYPE,
INFORMATION_SCHEMA.TABLE_CONSTRAINTS.TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS, INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
WHERE INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME
AND INFORMATION_SCHEMA.TABLE_CONSTRAINTS.TABLE_NAME = 'Book'
AND INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_TYPE = 'FOREIGN KEY'
 * 
*/


/*
select  INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, INFORMATION_SCHEMA.COLUMNS.IS_NULLABLE,
INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME
from INFORMATION_SCHEMA.COLUMNS
LEFT JOIN 
INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
on INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME
where INFORMATION_SCHEMA.COLUMNS.TABLE_NAME = 'Orion' 


SELECT INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME, INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_TYPE,
INFORMATION_SCHEMA.TABLE_CONSTRAINTS.TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS, INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
WHERE INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME
 */
#endregion