using System;
using System.Threading;
using System.Windows.Forms;

using Contal.IwQuick.UI;

namespace AnalyticTool.Advitech
{
    public partial class SelectDatabaseAndTableForm : Form, IDialog
    {
        private Thread _obtainServersThread;
        private string _selectedDatabase;

        public Form ResultDialog { get; private set; }
        
        public SelectDatabaseAndTableForm()
        {
            InitializeComponent();

            StartGetDatabasesThread();
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            ResultDialog = new DatabaseServerSettingsForm();
            DialogResult = DialogResult.Cancel;
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_cbDatabaseList.Text))
            {
                Dialog.Error("The database is not selected");
                _cbDatabaseList.Focus();
                return;
            }

            if (string.IsNullOrEmpty(_cbTables.Text))
            {
                Dialog.Error("The table is not selected");
                _cbTables.Focus();
                return;
            }

            ApplicationProperties.Singleton.DatabaseName = _cbDatabaseList.Text;
            ApplicationProperties.Singleton.TableName = _cbTables.Text;

            if (!DatabaseAccessor.Singleton.CheckDatabaseAndTable())
            {
                Dialog.Error("Could not create database or table");
                return;
            }

            ResultDialog = new ConfigureNovaObjectsForm();
            DialogResult = DialogResult.OK;
        }

        private void StartGetDatabasesThread()
        {
            _cbDatabaseList.Enabled = false;

            _obtainServersThread =
                new Thread(ObtainDatabasesThread)
                {
                    IsBackground = true
                };

            _obtainServersThread.Start();
        }

        private void ObtainDatabasesThread()
        {
            var databases = DatabaseAccessor.Singleton.GetDatabases();

            _cbDatabaseList.BeginInvoke(new Action(() =>
            {
                _cbDatabaseList.DataSource = databases;
                _cbDatabaseList.Enabled = true;

                if (string.IsNullOrEmpty(ApplicationProperties.Singleton.DatabaseName))
                    return;

                foreach (var item in _cbDatabaseList.Items)
                {
                    if (item.ToString() == ApplicationProperties.Singleton.DatabaseName)
                    {
                        _cbDatabaseList.SelectedItem = item;
                        break;
                    }
                }

                _cbDatabaseList_SelectedIndexChanged(null, null);
            }));
        }

        private void StartGetTablesThread()
        {
            _cbDatabaseList.Enabled = false;
            _cbTables.Enabled = false;

            ObtainTablesThread();
        }

        private void ObtainTablesThread()
        {
            var tables = DatabaseAccessor.Singleton.GetTables(_selectedDatabase);

            _cbTables.DataSource = tables;
            _cbDatabaseList.Enabled = true;
            _cbTables.Enabled = true;

            if (string.IsNullOrEmpty(ApplicationProperties.Singleton.TableName))
                return;

            foreach (var item in _cbTables.Items)
            {
                if (item.ToString() == ApplicationProperties.Singleton.TableName)
                {
                    _cbTables.SelectedItem = item;
                    break;
                }
            }
        }

        private void _cbDatabaseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedDatabase = _cbDatabaseList.Text;          
            StartGetTablesThread();
        }
    }
}
