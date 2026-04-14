using System;
using System.Linq;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.DBSCreator
{
    internal partial class FormAlterTables : TranslateForm
    {
        private readonly CompareOrmModel _ormComparer;
        
        #region Properties

        #endregion

        public FormAlterTables(
            DatabaseUpdater databaseUpdater)
            : base(databaseUpdater.LocalizationHelper)
        {
            InitializeComponent();
            _bRunAlter.Enabled = true;
            _ormComparer = databaseUpdater.CompareOrmModel;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        ///// <summary>
        ///// Drop invalid columns from database
        ///// </summary>
        ///// <returns>true on success</returns>
        //private bool AlterTable()
        //{
        //    //if (_error == null)
        //    //{
        //    //    _rbInfo.Text += GetString("FormAlterTablesDatabaseOK") + "\n";
        //    //    return true;
        //    //}
        //    //if (_error.Count == 0)
        //    //{
        //    //    _rbInfo.Text += GetString("FormAlterTablesDatabaseOK") + "\n";
        //    //    return true;
        //    //}

        //    //try
        //    //{
        //    //    CreateDBS killer;
        //    //    killer = new CreateDBS(_creatorProperties, LocalizationHelper);
        //    //    killer.OnError += new Contal.IwQuick.DException2Void(killerEOnError);
        //    //    killer.User = _userName;
        //    //    killer.Password = _userPassword;

        //    //    killer.DropColumns(_error);        
        //    //    return true;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    _lastException = ex;
        //    //    return false;
        //    //}            
        //}

        private void _bRunAlter_Click(object sender, EventArgs e)
        {
            IwQuick.UI.Dialog.Error(GetString("InfoFixNotSupported"));
            //if (!Dialog.Question(GetString("FormUpdateDatabase_bFixDbs"), GetString("FormAlterTablesfAlterTables"))) return;
            
            //_bListErrors.Enabled = false;
            //if (AlterTable())
            //{
            //    _rbInfo.Text += GetString("FormAlterTablesAlterDatabaseFinished") + "\n";
            //    _bRunAlter.Enabled = false;
            //    _succes = true;
            //}
            //else
            //{
            //    _rbInfo.Text += GetString("FormAlterTablesAlterDatabaseFailed") + "\n";
            //    _succes = false;
            //}
        }

        private void _cbErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (_cbErrors.SelectedIndex)
            {
                case 0:
                    ShowMissingTables();
                    break;

                case 1:
                    ShowMissingColumns();
                    break;

                case 2:
                    ShowWrongColumns();
                    break;

                case 3:
                    ShowUnusedColumns();
                    break;
            }
        }

        private void ShowMissingTables()
        {
            _rbInfo.Text = string.Empty;

            if (_ormComparer == null)
            {
                _rbInfo.Text += GetString("InfoNoComparer") + Environment.NewLine;
                return;
            }

            if (!_ormComparer.DatabaseErrors.HasMissingTables)
            {
                _rbInfo.Text += GetString("InfoDbsHasAllTable") + Environment.NewLine;

                return;
            }
            
            _rbInfo.Text += GetString("InfoMissedTable") +Environment.NewLine;

            foreach (
                    String str in _ormComparer.DatabaseErrors.MissingTables)
                _rbInfo.Text += str + Environment.NewLine;
        }

        private void ShowMissingColumns()
        {
            _rbInfo.Text = string.Empty;

            if (_ormComparer == null)
            {
                _rbInfo.Text += GetString("InfoNoComparer") + Environment.NewLine;

                return;
            }

            bool wasError = false;

            _rbInfo.Text += GetString("InfoTableMissColumn") + Environment.NewLine;

            var tableErrors =
                _ormComparer.DatabaseErrors.TableErrors
                    .Where(sqlTableError => sqlTableError.HasMissingColumns);

            foreach (Schema.SqlTableError sqlTableError in tableErrors)
            {
                wasError = true;

                _rbInfo.Text += 
                    GetString("InfoTable") +
                    sqlTableError.TableName +
                    Environment.NewLine;

                foreach (String str in sqlTableError.MissingColumns)
                    _rbInfo.Text += " " + str + Environment.NewLine;
            }

            if (!wasError)
                _rbInfo.Text += GetString("InfoNoErrors") + Environment.NewLine;

            _rbInfo.Text += Environment.NewLine;
        }

        private void ShowWrongColumns()
        {
            _rbInfo.Text = string.Empty;

            if (_ormComparer == null)
            {
                _rbInfo.Text += GetString("InfoNoComparer") + Environment.NewLine;
                return;
            }

            _rbInfo.Text += GetString("InfoTableWrongColumns") + Environment.NewLine;

            bool wasError = false;

            var sqlTableErrors = 
                _ormComparer.DatabaseErrors.TableErrors
                    .Where(sqlTableError => sqlTableError.HasWrongColumns);

            foreach (Schema.SqlTableError sqlTableError in sqlTableErrors)
            {
                wasError = true;

                _rbInfo.Text += 
                    GetString("InfoTable") + 
                    sqlTableError.TableName + 
                    Environment.NewLine;

                foreach (Schema.WrongColumnm wCol in sqlTableError.WrongColumns)
                {
                    _rbInfo.Text += " " + wCol.ColumnName;
                    _rbInfo.Text += " " + wCol.ColumnError + Environment.NewLine;
                }
            }

            if (!wasError)
                _rbInfo.Text += GetString("InfoNoErrors") + Environment.NewLine;

            _rbInfo.Text += Environment.NewLine;
        }

        private void ShowUnusedColumns()
        {
            _rbInfo.Text = string.Empty;

            if (_ormComparer == null)
            {
                _rbInfo.Text += GetString("InfoNoComparer") + Environment.NewLine;
                return;
            }

            _rbInfo.Text += GetString("InfoTableUselessColumn") + Environment.NewLine;

            bool wasError = false;

            var sqlTableErrors = 
                _ormComparer.DatabaseErrors.TableErrors
                    .Where(sqlTableError => sqlTableError.HasUnusedColumns);

            foreach (Schema.SqlTableError sqlTableError in sqlTableErrors)
            {
                wasError = true;

                _rbInfo.Text += 
                    GetString("InfoTable") +
                    sqlTableError.TableName +
                    Environment.NewLine;

                foreach (String str in sqlTableError.UnusedColumns)
                    _rbInfo.Text += " " + str + Environment.NewLine;
            }

            if (!wasError)
                _rbInfo.Text += GetString("InfoNoErrors") + Environment.NewLine;

            _rbInfo.Text += Environment.NewLine;
        }

        private void _bListErrors_Click(object sender, EventArgs e)
        {
            _cbErrors_SelectedIndexChanged(sender, e);
        }
    }
}
