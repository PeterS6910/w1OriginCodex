using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.Common;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Sys;
using System.Web.UI.WebControls;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.NCAS.Server.DB;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Client
{

    public partial class ExcelExportForm :
#if DESIGNER
        Form
#else
      PluginMainForm<NCASClient>
#endif
    {
        private readonly IList<FilterSettings> _filterSettings;
        private ITimer _timerForProgressBar = null;
        private static ExcelExportForm _openedForm = null;
        private readonly bool bCR;
        // Excel export

        /// <summary>
        /// Create form ExcelExportForm
        /// </summary>
        public ExcelExportForm()
            :  base(NCASClient.LocalizationHelper, CgpClientMainForm.Singleton)
        {
           InitializeComponent();

            DateFormat dateFormat = new DateFormat("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss");
            _cbDateFormat.Items.Add(dateFormat);
            _cbDateFormat.SelectedItem = dateFormat;
            dateFormat = new DateFormat("yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm");
            _cbDateFormat.Items.Add(dateFormat);
            dateFormat = new DateFormat("dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm:ss");
            _cbDateFormat.Items.Add(dateFormat);
            dateFormat = new DateFormat("dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm");
            _cbDateFormat.Items.Add(dateFormat);
            //_cbColumnCGPSource.Checked = GeneralOptions.Singleton.EventlogListReporter;
        }

        public ExcelExportForm(bool bCardReader, IList<FilterSettings> filterSettings=null)
            : this()
        {
            _filterSettings = filterSettings;
            bCR = bCardReader;
            _saveFileDialog.DefaultExt = "xlsx";
            _saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|(*.*)|*.*";

            _gbColumns.Enabled = false;
        }

        private void _bBrowse_Click(object sender, EventArgs e)
        {
            if (_eFilePath.Text != string.Empty)
            {
                string fileName = Path.GetFileName(_eFilePath.Text);
                _saveFileDialog.InitialDirectory = _eFilePath.Text.Substring(0, _eFilePath.Text.Length - fileName.Length);
                _saveFileDialog.FileName = fileName;
            }
            else
            {
                _saveFileDialog.FileName = string.Empty;
            }

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _eFilePath.Text = _saveFileDialog.FileName;
            }
        }

        private void _bExport_Click(object sender, EventArgs e)
        {
             ExportExcel();
        }

        private void ExportExcel()
        {
            if (_eFilePath.Text == string.Empty)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eFilePath,
                    GetString("ErrorEntryFilePath"), ControlNotificationSettings.Default);
                return;
            }
            DataTable dataTable = null;
            bool bFillSection = false;
            try
            {
                if (Plugin.MainServerProvider!=null)
                    dataTable=Plugin.MainServerProvider.ExportDataLogs(_filterSettings,  bCR, out  bFillSection);
                else
                {
                    MessageBox.Show("Main Server Provider not found!", "ERROR");
                    return;
                }

                if(dataTable!=null)
                {
                    //Create Excel File;
                    ExcelCreator.CreateExcelFile(_eFilePath.Text, dataTable, bFillSection);
                }
            }
            catch
            {
                ExportFailed();
                Close();
                return;
            }
            finally
            {

            }

            ExportSucceeded();
            Close();
        }

        /// <summary>
        /// Call method for increment value in the progress
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        private bool OnTimerForProgressBar(TimerCarrier timer)
        {
            IncrementProgressBar();
            return true;
        }

        /// <summary>
        /// Increment value in the progress bar
        /// </summary>
        private void IncrementProgressBar()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(IncrementProgressBar));
            }
            else
            {
                if (_pbExport.Value < (0.9 * _pbExport.Maximum))
                {
                    _pbExport.Value += 1;
                }
            }
        }

        /// <summary>
        /// Stop timer form progress bar
        /// </summary>
        private void StopTimerForProgressBar()
        {
            if (_timerForProgressBar != null)
            {
                _timerForProgressBar.StopTimer();
                _timerForProgressBar = null;
            }
        }

        /// <summary>
        /// Close file stream for export, stop timer for progress and enable buttons from export and close form
        /// </summary>
        /// <param name="fileStream"></param>
        private void CloseFileAndStopTimerForProgressBar(StreamWriter fileStream)
        {
            if (fileStream != null)
            {
                try
                {
                    fileStream.Close();
                }
                catch { }
            }

            StopTimerForProgressBar();
            EnableButtons();
        }

        /// <summary>
        /// Enable buttons for export and close form
        /// </summary>
        private void EnableButtons()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(EnableButtons));
            }
            else
            {
                _bClose.Enabled = true;
                _bExport.Enabled = true;
            }
        }

        /// <summary>
        /// Show dialog for export failed
        /// </summary>
        private void ExportFailed()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ExportFailed));
            }
            else
            {
                _pbExport.Value = 0;
                Dialog.Error(GetString("ExportFailed"));
            }
        }

        /// <summary>
        /// Show dialog for export succeeded
        /// </summary>
        private void ExportSucceeded()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ExportSucceeded));
            }
            else
            {
                _pbExport.Value = _pbExport.Maximum;
                Dialog.Info(GetString("ExportSucceeded"));
                _pbExport.Value = 0;
            }
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public new void Show()
        {
           if (_openedForm == null)
           {
               _openedForm = this;
               base.Show();
           }
           else
           {
               _openedForm.BringToFront();
           }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _openedForm = null;
            base.OnFormClosing(e);
        }

    }

    public class ExportSettings
    {
        private readonly StreamWriter _fileStream;
        private readonly List<string> _columns;
        private readonly string _separator;
        private readonly string _dateFormat;

        public StreamWriter FileStream { get { return _fileStream; } }
        public List<string> Columns { get { return _columns; } }
        public string Separator { get { return _separator; } }
        public string DateFormat { get { return _dateFormat; } }

        public ExportSettings(StreamWriter fileStream, List<string> columns, string separator, string dateFormat)
        {
            _fileStream = fileStream;
            _columns = columns;
            _separator = separator;
            _dateFormat = dateFormat;
        }
    }
}
