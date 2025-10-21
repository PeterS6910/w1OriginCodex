using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace NovaEventLogs
{
    public partial class DbsConnection : Form
    {
        DbsConnect _dbsCnt;
        public DbsConnection(DbsConnect dbsCnt)
        {
            InitializeComponent();
            _dbsCnt = dbsCnt;
        }

        private void _Ok_Click(object sender, EventArgs e)
        {
            _dbsCnt.Server = _eServer.Text;
            _dbsCnt.User = _eUser.Text;
            _dbsCnt.Password = _ePassword.Text;
            _dbsCnt.DataSource = _eDataSoucerDb.Text;
            _dbsCnt.EventLogDbs = _eDataSoucerDbLog.Text;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DoConnection()
        {
            //try
            //{
            //    SqlConnection nwindConn = new SqlConnection(@"Data Source=WSJANEK\SQLEXPRESS; Integrated Security=SSPI;" +
            //                                    "Initial Catalog=Nova mala DB log");
            //    //SqlConnection connection = new SqlConnection(
            //    nwindConn.Open();
            //}
            //catch (Exception error)
            //{
            //    Console.Write(error.ToString());
            //}
        }
    }
}
