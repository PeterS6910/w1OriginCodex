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
using System.Threading;
using System.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Client
{

    public partial class ExportForm :
#if DESIGNER
        Form
#else
        ACgpEditForm<AOrmObject>
#endif
    {
        private readonly IList<FilterSettings> _filterSettings;
        private ITimer _timerForProgressBar = null;
        private static ExportForm _openedForm = null;


        /// <summary>
        /// Create form ExportForm
        /// </summary>
        public ExportForm()
            : base(null, ShowOptionsEditForm.Edit)
        {
            InitializeComponent();
        }


        public ExportForm(IList<FilterSettings> filterSettings)
            : this()
        {
            _filterSettings = filterSettings;

            _saveFileDialog.DefaultExt = "xlsx";
            _saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|(*.*)|*.*";
        }

        private void _bBrowse_Click(object sender, EventArgs e)
        {
            if (tbPath.Text != string.Empty)
            {
                string fileName = Path.GetFileName(tbPath.Text);
                _saveFileDialog.InitialDirectory = tbPath.Text.Substring(0, tbPath.Text.Length - fileName.Length);
                _saveFileDialog.FileName = fileName;
            }
            else
            {
                _saveFileDialog.FileName = string.Empty;
            }

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = _saveFileDialog.FileName;
            }
        }

        private void _bExport_Click(object sender, EventArgs e)
        {
                 ExportExcel();
        }


        private void ExportExcel()
        {
            if (tbPath.Text == string.Empty)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eFilePath,
                    GetString("ErrorEntryFilePath"), ControlNotificationSettings.Default);
                return;
            }
            bool bFillSection = false;
            DataTable dataTable = null;
            try
            {
                if (CgpClient.Singleton.MainServerProvider != null
                       && CgpClient.Singleton.MainServerProvider.Cards != null)
                    dataTable = CgpClient.Singleton.MainServerProvider.Cards.ExportCards(_filterSettings, out bFillSection);
                else
                {
                    MessageBox.Show("Main Server Provider not found!", "ERROR");
                    return;
                }

                if (dataTable != null)
                {
                    //Create Excel File;
                    ExcelCreator.CreateExcelFile(tbPath.Text, dataTable, bFillSection);
                }
            }
            catch
            {
                ExportFailed();
                Close();
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
                if (pbExport.Value < (0.9 * pbExport.Maximum))
                {
                    pbExport.Value += 1;
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
                pbExport.Value = pbExport.Maximum;
                Dialog.Info(GetString("InfoEventlogsExportSucceeded"));
                pbExport.Value = 0;
            }
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public  new void Show()
        {
            if (_openedForm == null)
            {
                _openedForm = this;
                base.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
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

}
