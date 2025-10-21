using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.DBSCreator
{
    internal class Schema
    {
        public enum ColumnTypeEnum : byte
        {
            Column,
            PrimaryKey,
            ForeignKey,
            Unknown
        }

        public class WrongColumnm
        {
            public string ColumnName { get; private set; }
            public string ColumnError { get; private set; }

            public WrongColumnm(string name, string error)
            {
                ColumnName = name;
                ColumnError = error;
            }
        }

        public class TableInfo
        {
            public string Name { get; set; }

            public List<TableRecord> Columns { get; private set; }

            public TableInfo(string name)
            {
                Name = name;
            }

            public TableInfo()
            {
            }

            public void AddRow(TableRecord talbeRecord)
            {
                if (Columns == null)
                    Columns = new List<TableRecord>();

                Columns.Add(talbeRecord);
            }

            public void UpdateRowColumnType(
                string recordName,
                ColumnTypeEnum newColumnType)
            {
                TableRecord tableRecord =
                    Columns.FirstOrDefault(tr => tr.Name == recordName);

                if (tableRecord != null)
                    tableRecord.ColumnType = newColumnType;
            }

            public void UpdateRowUnique(string recordName, bool newUnigue)
            {
                TableRecord tableRecord =
                    Columns.FirstOrDefault(tr => tr.Name == recordName);

                if (tableRecord != null)
                    tableRecord.Unique = newUnigue;
            }

            public SqlTableError CompareTables(TableInfo otherTable)
            {
                var tableError = new SqlTableError(otherTable.Name);

                bool wasError = false;

                foreach (TableRecord myTableRecord in Columns)
                {
                    // check if columns exists
                    TableRecord otherTableRecord =
                        otherTable.GetTableRecordByName(myTableRecord.Name);

                    if (otherTableRecord == null)
                    {
                        wasError = true;
                        tableError.AddMissingColumn(myTableRecord.Name);

                        continue;
                    }

                    //check if column has same proterties
                    string error = myTableRecord.CompareTableRecord(otherTableRecord);

                    if (error == String.Empty)
                        continue;

                    wasError = true;

                    tableError.AddWrongColumn(
                        new WrongColumnm(
                            myTableRecord.Name,
                            error));
                }

                foreach (TableRecord otherTableRecord in otherTable.Columns)
                {
                    if (otherTableRecord.ColumnType == ColumnTypeEnum.ForeignKey)
                        continue;

                    TableRecord myTableRecord = GetTableRecordByName(otherTableRecord.Name);

                    if (myTableRecord != null)
                        continue;

                    wasError = true;
                    tableError.AddUnusedColumn(otherTableRecord.Name);
                }

                return
                    wasError
                        ? tableError
                        : null;
            }

            public TableRecord GetTableRecordByName(string name)
            {
                return Columns.FirstOrDefault(tr => tr.Name == name);
            }
        }

        public class TableRecord
        {
            public string Name { get; set; }
            public ColumnTypeEnum ColumnType { get; set; }
            public bool Unique { get; set; }
            public bool NotNull { get; set; }

            public TableRecord()
            {
                ColumnType = ColumnTypeEnum.Column;
            }

            public string CompareTableRecord(TableRecord otherTableRecord)
            {
                string error = String.Empty;

                if (Name != otherTableRecord.Name)
                    return "wtf";

                if (Unique != otherTableRecord.Unique)
                    error += "Unique " + Unique + " ";

                if (NotNull != otherTableRecord.NotNull)
                    error += "Not null " + NotNull + " ";

                if (ColumnType != otherTableRecord.ColumnType)
                    error += "Column type" + ColumnType + " ";

                return error;
            }
        }

        public class SqlTableError
        {
            private ICollection<string> _unusedColumns;
            private ICollection<WrongColumnm> _wrongColumns;
            private ICollection<string> _missingColumns;

            public SqlTableError(string name)
            {
                TableName = name;
            }

            public string TableName { get; private set; }

            public IEnumerable<WrongColumnm> WrongColumns
            {
                get { return _wrongColumns; }
            }

            public IEnumerable<string> MissingColumns
            {
                get { return _missingColumns; }
            }

            public IEnumerable<string> UnusedColumns
            {
                get { return _unusedColumns; }
            }

            public bool HasUnusedColumns
            {
                get
                {
                    return _unusedColumns != null && _unusedColumns.Count > 0;
                }
            }

            public bool HasMissingColumns
            {
                get
                {
                    return _missingColumns != null && _missingColumns.Count > 0;
                }
            }

            public bool HasWrongColumns
            {
                get
                {
                    return _wrongColumns != null && _wrongColumns.Count > 0;
                }
            }

            public void AddWrongColumn(WrongColumnm wrongColumnName)
            {
                if (_wrongColumns == null)
                    _wrongColumns = new List<WrongColumnm>();

                _wrongColumns.Add(wrongColumnName);
            }

            public void AddMissingColumn(string missingColumnName)
            {
                if (_missingColumns == null)
                    _missingColumns = new List<string>();

                _missingColumns.Add(missingColumnName);
            }

            public void AddUnusedColumn(string unusedColumnName)
            {
                if (_unusedColumns == null)
                    _unusedColumns = new List<string>();

                _unusedColumns.Add(unusedColumnName);
            }
        }

        public class DatabaseErrors
        {
            private readonly ICollection<string> _missingTables =
                new LinkedList<string>();

            private readonly ICollection<SqlTableError> _tableErrors =
                new LinkedList<SqlTableError>();

            public bool HasMissingTables
            {
                get
                {
                    return _missingTables.Count > 0;
                }
            }

            public IEnumerable<string> MissingTables
            {
                get
                {
                    return _missingTables;
                }
            }

            public bool HasTableErrors
            {
                get
                {
                    return _tableErrors.Count > 0;
                }
            }

            public IEnumerable<SqlTableError> TableErrors
            {
                get
                {
                    return _tableErrors;
                }
            }

            public void AddMissingTable(string tableName)
            {
                _missingTables.Add(tableName);
            }

            public void AddTableError(SqlTableError sqlTableError)
            {
                _tableErrors.Add(sqlTableError);
            }
        }
    }
}