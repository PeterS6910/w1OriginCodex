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
using Contal.Cgp.Client.StructuredSubSites;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public enum ExportMode
    {
        Csv = 0,
        Excel = 1
    }

    public partial class EventlogsExportForm :
#if DESIGNER
        Form
#else
        ACgpEditForm<AOrmObject>
#endif
    {
        private readonly IList<FilterSettings> _filterSettings;
        private readonly int _lastQueryDelay;
        private ITimer _timerForProgressBar = null;
        private static EventlogsExportForm _openedForm = null;
        private ExportMode _exportMode = ExportMode.Csv;

        // Excel export
        private readonly SelectingOfSubSitesSupport _selectingOfSubSitesSupport;

        /// <summary>
        /// Create form EventlogsExportForm
        /// </summary>
        public EventlogsExportForm()
            : base(null, ShowOptionsEditForm.Edit)
        {
            InitializeComponent();

            List<Separators> separators = new List<Separators>();
            separators.Add(new Separators("TAB", '\t'));
            separators.Add(new Separators(",", ','));
            separators.Add(new Separators(";", ';'));
            Separators separator = new Separators("TAB", '\t');
            _cbSeparator.Items.Add(separator);
            separator = new Separators(",", ',');
            _cbSeparator.Items.Add(separator);
            separator = new Separators(";", ';');
            _cbSeparator.Items.Add(separator);
            _cbSeparator.SelectedItem = separator;
            DateFormat dateFormat = new DateFormat("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss");
            _cbDateFormat.Items.Add(dateFormat);
            _cbDateFormat.SelectedItem = dateFormat;
            dateFormat = new DateFormat("yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm");
            _cbDateFormat.Items.Add(dateFormat);
            dateFormat = new DateFormat("dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm:ss");
            _cbDateFormat.Items.Add(dateFormat);
            dateFormat = new DateFormat("dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm");
            _cbDateFormat.Items.Add(dateFormat);
            _cbColumnCGPSource.Checked = GeneralOptions.Singleton.EventlogListReporter;
        }

        /// <summary>
        /// Create form EventlogsExportForm
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="lastQueryDelay"></param>
        public EventlogsExportForm(IList<FilterSettings> filterSettings, int lastQueryDelay)
            : this()
        {
            _filterSettings = filterSettings;
            _lastQueryDelay = lastQueryDelay;

            if (_lastQueryDelay == 0)
                _lastQueryDelay = 1;
        }

        public EventlogsExportForm(IList<FilterSettings> filterSettings, SelectingOfSubSitesSupport selectingOfSubSitesSupport)
            : this()
        {
            _filterSettings = filterSettings;
            _selectingOfSubSitesSupport = selectingOfSubSitesSupport;
            _exportMode = ExportMode.Excel;

            _saveFileDialog.DefaultExt = "xlsx";
            _saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|(*.*)|*.*";

            _gbColumns.Enabled = false;
            _cbAddColumnHeaders.Checked = true;
            _cbAddColumnHeaders.Enabled = false;
            _cbSeparator.Enabled = false;
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
            switch (_exportMode)
            {
                case ExportMode.Csv:
                    ExportCsv();
                    break;

                case ExportMode.Excel:
                    ExportExcel();
                    break;
            }
        }

        private void ExportCsv()
        {
            StreamWriter fileStream = null;
            bool exportStarted = false;
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                if (_lastQueryDelay > 30)
                {
                    if (!Dialog.WarningQuestion(GetString("WarningQuestionEventlogsExportDelay", _lastQueryDelay)))
                    {
                        return;
                    }
                }

                List<string> columns = new List<string>();
                if (_cbColumnType.Checked)
                {
                    columns.Add(Eventlog.COLUMN_TYPE);
                }

                if (_cbColumnDate.Checked)
                {
                    columns.Add(Eventlog.COLUMN_EVENTLOG_DATE_TIME);
                }

                if (_cbColumnCGPSource.Checked)
                {
                    columns.Add(Eventlog.COLUMN_CGPSOURCE);
                }

                if (_cbColumnEventSources.Checked)
                {
                    columns.Add(Eventlog.COLUMN_EVENTSOURCES);
                }

                if (_cbColumnDescription.Checked)
                {
                    columns.Add(Eventlog.COLUMN_DESCRIPTION);
                }

                if (columns.Count == 0)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _gbColumns,
                        GetString("ErrorSelectColumns"), ControlNotificationSettings.Default);
                    return;
                }

                string strSeparator;
                Separators separator = _cbSeparator.SelectedItem as Separators;

                if (separator == null)
                {
                    if (_cbSeparator.Text != string.Empty)
                    {
                        strSeparator = _cbSeparator.Text;
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbSeparator,
                            GetString("ErrorEntrySeparator"), ControlNotificationSettings.Default);
                        return;
                    }
                }
                else
                {
                    strSeparator = separator.Sepearator.ToString(CultureInfo.InvariantCulture);
                }

                DateFormat dateFormat = _cbDateFormat.SelectedItem as DateFormat;
                if (dateFormat == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbDateFormat,
                        GetString("ErrorEntryDateFormat"), ControlNotificationSettings.Default);
                    return;
                }

                if (_eFilePath.Text == string.Empty)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eFilePath,
                        GetString("ErrorEntryFilePath"), ControlNotificationSettings.Default);
                    return;
                }

                try
                {
                    fileStream = File.CreateText(_eFilePath.Text);
                }
                catch
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eFilePath,
                        GetString("ErrorCanNotCreateFile"), ControlNotificationSettings.Default);
                    return;
                }

                if (_cbAddColumnHeaders.Checked)
                {
                    string header = string.Empty;

                    foreach (string column in columns)
                    {
                        if (header != string.Empty)
                        {
                            header += strSeparator;
                        }

                        header += column;
                    }

                    fileStream.WriteLine(header);
                }

                _pbExport.Value = 0;
                _pbExport.Maximum = _lastQueryDelay;
                _bClose.Enabled = false;
                _bExport.Enabled = false;

                ExportSettings exportSettings = new ExportSettings(fileStream, columns, strSeparator, dateFormat.DateFormatString);
                SafeThread<ExportSettings>.StartThread(RunExportCsv, exportSettings);

                _timerForProgressBar = TimerManager.Static.StartTimer(1000, false, OnTimerForProgressBar);

                exportStarted = true;
            }
            catch { }
            finally
            {
                if (!exportStarted)
                {
                    CloseFileAndStopTimerForProgressBar(fileStream);
                }
            }
        }

        private void ExportExcel()
        {
            DateFormat dateFormat = _cbDateFormat.SelectedItem as DateFormat;
            if (dateFormat == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbDateFormat,
                    GetString("ErrorEntryDateFormat"), ControlNotificationSettings.Default);
                return;
            }

            if (_eFilePath.Text == string.Empty)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eFilePath,
                    GetString("ErrorEntryFilePath"), ControlNotificationSettings.Default);
                return;
            }

            try
            {
                // Generate the report
                CgpClient.Singleton.MainServerProvider.Eventlogs.GenerateDataForExcelByCriteria(_filterSettings, _selectingOfSubSitesSupport.GetSelectedSiteIds(),
                    dateFormat.DateFormatString, null, null);

                // Transfer the file from Server
                if (CgpClient.Singleton.MainServerProvider == null)
                {
                    Dialog.Error(GetString("ServerUnavailable"));
                    return;
                }
                if (!QuickPath.EnsureDirectory(Path.GetDirectoryName(_eFilePath.Text)))
                {
                    Dialog.Error(GetString("DirectoryDoesNotExist") + " " + Path.GetDirectoryName(_eFilePath.Text));
                    return;
                }

                bool _finish = false;
                byte[] fileData = null;
                int bufferLength = 0;
                FileStream fs = null;
                while (!_finish)
                {
                    if (!CgpClient.Singleton.MainServerProvider.IsLastEventlogReportAvailable())
                    {
                        // Wait till report is generated
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    fileData = CgpClient.Singleton.MainServerProvider.GetLastEventlogReport(fs == null ? 0 : (int)fs.Position);
                    if (fileData == null)
                    {
                        Dialog.Error(GetString("GetDataFromServerFailed"));
                        DialogResult = DialogResult.Abort;
                        break;
                    }
                    if (fs == null)
                    {
                        try
                        {
                            bufferLength = fileData.Length;
                            fs = new FileStream(_eFilePath.Text, FileMode.Create, FileAccess.Write, FileShare.None, bufferLength);
                        }
                        catch (Exception exc)
                        {
                            if (fs != null)
                                try { fs.Close(); }
                                catch (Exception ex) { }
                            fs = null;
                            Dialog.Error(GetString("Exception") + " (" + exc.Message + ")");
                            break; ;
                        }
                    }
                    fs.Write(fileData, 0, fileData.Length);
                    if (fileData.Length < bufferLength)
                    {
                        try { fs.Close(); }
                        catch (Exception ex) { }
                        break;
                    }
                }
            }
            catch
            {
                ExportFailed();
                return;
            }
            finally
            {
                CgpClient.Singleton.MainServerProvider.CleanLastEventlogReport();
            }

            ExportSucceeded();
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
        /// Get export lines from server and write lines to the file
        /// </summary>
        /// <param name="exportSettings"></param>
        private void RunExportCsv(ExportSettings exportSettings)
        {
            try
            {
                if (exportSettings == null)
                {
                    ExportFailed();
                    return;
                }

                IList<string> lines = CgpClient.Singleton.MainServerProvider.Eventlogs.CreateCSVExportLines(_filterSettings, exportSettings.Columns, exportSettings.Separator, exportSettings.DateFormat);
                foreach (string line in lines)
                {
                    exportSettings.FileStream.WriteLine(line);
                }
            }
            catch
            {
                ExportFailed();
                return;
            }
            finally
            {
                if (exportSettings != null)
                    CloseFileAndStopTimerForProgressBar(exportSettings.FileStream);
            }

            ExportSucceeded();
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
                Dialog.Error(GetString("ErrorEventlogsExportFailed"));
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
                Dialog.Info(GetString("InfoEventlogsExportSucceeded"));
                _pbExport.Value = 0;
            }
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public new void Show()
        {
            EventlogsForm.Singleton.OpeningEventlogsExportForm();
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
            EventlogsForm.Singleton.ClosingEventlogsExportForm();
            _openedForm = null;
            base.OnFormClosing(e);
        }

        #region ACGEditFormMethods
        protected override void AfterEdit()
        {
        }

        protected override void AfterInsert()
        {
        }

        protected override void EditEnd()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        protected override bool SaveToDatabaseEdit()
        {
            return false;
        }

        protected override bool SaveToDatabaseInsert()
        {
            return false;
        }

        protected override bool GetValues()
        {
            return false;
        }

        protected override bool CheckValues()
        {
            return false;
        }

        protected override void SetValuesEdit()
        {
        }

        protected override void SetValuesInsert()
        {
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            allowEdit = false;
        }

        protected override void BeforeEdit()
        {
        }

        protected override void BeforeInsert()
        {
        }

        protected override void RegisterEvents()
        {
        }
        #endregion
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
