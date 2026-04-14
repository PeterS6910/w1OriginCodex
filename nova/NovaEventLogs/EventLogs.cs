using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using System.Data.SqlClient;
using System.Diagnostics;

namespace NovaEventLogs
{
    public partial class EventLogs : TranslateForm
    {
        private const int DAY_WITHOUT_SECOND = 86399;

        public EventLogs()
        {
            InitializeComponent();
            InitCbType();
        }

        private void _bFilterDateOneDay_Click(object sender, EventArgs e)
        {
            if (!FilterCheckDates(false))
                return;

            DateTime now = DateTime.Now;

            if (_tbdpDateFromFilter.Value == null && _tbdpDateToFilter.Value == null)
            {
                _tbdpDateToFilter.Value = now;
                _tbdpDateFromFilter.Value = now.AddSeconds(-1 * DAY_WITHOUT_SECOND);
            }
            else
            {
                if (_tbdpDateFromFilter.Value == null)
                {
                    _tbdpDateFromFilter.Value = _tbdpDateToFilter.Value.Value.AddSeconds(-1 * DAY_WITHOUT_SECOND); ;
                }
                else
                {
                    _tbdpDateToFilter.Value = _tbdpDateFromFilter.Value.Value.AddSeconds(DAY_WITHOUT_SECOND);
                }
            }
        }

        private bool FilterCheckDates(bool checkBiggerDateTo)
        {
            if (_tbdpDateFromFilter.Value == null || _tbdpDateToFilter.Value == null)
                return true;

            if (_tbdpDateFromFilter.Value > _tbdpDateToFilter.Value && checkBiggerDateTo)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbdpDateToFilter.TextBox,
                    GetString("ErrorDateTo"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tbdpDateToFilter.TextBox.Focus();
                return false;
            }

            return true;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            if (!_dbsCnt.ValidCS())
            {
                DbsConnection formValues = new DbsConnection(_dbsCnt);
                formValues.ShowDialog();
                
                if (!_dbsCnt.ValidCS())
                    return;
            }
            ShowDataEventlogs();
        }

        //int _firstToShow = 0;
        //private void ShowDataEventlogsPaggin()
        //{
        //    _dbsCnt.CreateSqlConnection();
        //    SqlDataAdapter dataadapter = new SqlDataAdapter(GetSqlCommand(), _dbsCnt.SqlConnection);
        //    try
        //    {
        //        DataSet dataSet = new DataSet();
        //        _dbsCnt.SqlConnection.Open();
        //        //dataadapter.Fill(dataSet, "EventLogs");
        //        dataadapter.Fill(dataSet, _firstToShow, 30, "EventLogs");
        //        _dgValues.DataSource = dataSet;
        //        _dgValues.DataMember = "EventLogs";
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        _dbsCnt.SqlConnection.Close();
        //    }
        //}

        private void ShowDataEventlogs()
        {
            try
            {
                _dbsCnt.CreateSqlConnection();
                SqlDataAdapter dataadapter = new SqlDataAdapter(GetSqlCommand(), _dbsCnt.SqlConnection);
                dataadapter.SelectCommand.CommandTimeout = 90000;

                try
                {
                    DataSet dataSet = new DataSet();
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    _dbsCnt.SqlConnection.Open();
                    dataadapter.Fill(dataSet, "EventLogs");
                    _dgValues.DataSource = dataSet;
                    _dgValues.DataMember = "EventLogs";
                    sw.Stop();
                    _lTime.Text = "Seconds: " + sw.Elapsed.Seconds.ToString();
                }
                catch (Exception ex)
                {
                    Contal.IwQuick.UI.Dialog.Error(ex.ToString());
                }
                finally
                {
                    _dbsCnt.SqlConnection.Close();
                }
            }
            catch { }
        }

        private string GetSqlCommand()
        {
            string sqlCommand = "Select Type, Date, CGPSource, Description from Eventlog";

            string tmpString;
            List<String> addConditions = new List<string>();

            if (!String.IsNullOrEmpty(_tbdpDateFromFilter.Text))
            {
                if (!String.IsNullOrEmpty(_tbdpDateToFilter.Text))
                {
                    tmpString = "Date between '";
                    tmpString += GetDateFromDateTime(_tbdpDateFromFilter.Value);
                    tmpString += "' AND '";
                    tmpString += GetDateFromDateTime(_tbdpDateToFilter.Value, 1);
                    tmpString += "'";
                    addConditions.Add(tmpString);
                }
                else
                {
                    tmpString = "Date >= '";
                    tmpString += GetDateFromDateTime(_tbdpDateFromFilter.Value);
                    tmpString += "'";
                    addConditions.Add(tmpString);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(_tbdpDateToFilter.Text))
                {
                    tmpString = "Date <= '";
                    tmpString += GetDateFromDateTime(_tbdpDateToFilter.Value);
                    tmpString += "'";
                    addConditions.Add(tmpString);
                }
            }

            if (!String.IsNullOrEmpty(_eCGPSourceFilter.Text))
            {
                tmpString = "CGPSource like '%";
                tmpString += _eCGPSourceFilter.Text;
                tmpString += "%'";
                addConditions.Add(tmpString);
            }

            if (!String.IsNullOrEmpty(_eDescriptionFilter.Text))
            {
                tmpString = "Description like '%";
                tmpString += _eDescriptionFilter.Text;
                tmpString += "%'";
                addConditions.Add(tmpString);
            }

            if (_cbType.SelectedItem is string)
            {
                string type = _cbType.SelectedItem as string;
                if (!string.IsNullOrEmpty(type))
                {
                    tmpString = "Type = '";
                    tmpString += type;
                    tmpString += "'";
                    addConditions.Add(tmpString);
                }
            }

            if (!string.IsNullOrEmpty(_eEventSourceFilter.Text))
            {
                tmpString = "IdEventlog in (select IdEventlog from EventSource where  EventSource.EventSourceObjectGuid in ";
                tmpString += "(Select Id from [" + _dbsCnt.DataSource + "].dbo.CentralNameRegister where Name like '%" + _eEventSourceFilter.Text + "%'))";
                addConditions.Add(tmpString);
            }

            bool isFirst = true;
            foreach (String str in addConditions)
            {
                if (isFirst)
                {
                    isFirst = false;
                    sqlCommand += " where " + str;
                }
                else
                {
                    sqlCommand += " and " + str;
                }
            }

            return sqlCommand;
        }

        private string GetDateFromDateTime(DateTime? dt)
        {
            string result = string.Empty;
            if (dt != null)
            {
                result = dt.Value.Year.ToString() + "-" + dt.Value.Month.ToString() + "-" + dt.Value.Day.ToString();
            }
            return result;
        }

        private string GetDateFromDateTime(DateTime? dt, int addDay)
        {
            string result = string.Empty;
            if (dt != null)
            {
                result = dt.Value.Year.ToString() + "-" + dt.Value.Month.ToString() + "-" + (dt.Value.Day + addDay).ToString();
            }
            return result;
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _tbdpDateFromFilter.Value = null;
            _tbdpDateToFilter.Value = null;
            _eCGPSourceFilter.Text = string.Empty;
            _eEventSourceFilter.Text = string.Empty;
            _eDescriptionFilter.Text = string.Empty;
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {

        }

        DbsConnect _dbsCnt = new DbsConnect(); 

        private void button1_Click(object sender, EventArgs e)
        {
            DbsConnection dbsConn = new DbsConnection(_dbsCnt);
            dbsConn.Show();
        }

        private void InitCbType()
        {
            _cbType.Items.Clear();
            _cbType.Items.Add(string.Empty);

            ElsC type = new ElsC();
            List<string> lTypes = type.GetEventLogTypes();

            foreach (string str in lTypes)
            {
                _cbType.Items.Add(str);
            }
        }
    }
}
