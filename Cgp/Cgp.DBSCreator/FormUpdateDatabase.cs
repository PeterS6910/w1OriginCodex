using System;
using System.Windows.Forms;

using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    internal partial class FormUpdateDatabase : TranslateForm, IFormUpdateDatabase, ICreateOrUpdateForm
    {
        private bool _rubProgressRun;

        public event DException2Void OnError;

        #region Properties

        #endregion

        public FormUpdateDatabase(
            DatabaseUpdater databaseUpdater)
            : base(databaseUpdater.LocalizationHelper)
        {
            InitializeComponent();

            _databaseUpdater = databaseUpdater;

            _databaseUpdater.OnError += InvokeOnError;

            CreatorProperties creatorProperties =
                databaseUpdater.CreatorProperties;

            _eDatabase.Text = creatorProperties.DatabaseName;
            _eSqlServer.Text = creatorProperties.DatabaseServer;         
            _tbLogin.Text = creatorProperties.DatabaseLogin;

            _eExternDatabase.Text =
                /*creatorProperties.EnableExternDatabase 
                    ?*/ creatorProperties.ExternDatabaseName;
            //: string.Empty;
        }

        void InvokeOnError(Exception inputError)
        {
            if (OnError != null)
                OnError(inputError);
        }

        private void _bPerform_Click(object sender, EventArgs e)
        {
            if (_databaseUpdater.Result == DatabaseUpdater.ResultEnum.Done)
            {
                Close();
                return;
            }

            try
            {
                if (!_databaseUpdater.DatabasesExist)
                {
                    _rbProgress.Text +=
                        "Database exists error" +
                        Environment.NewLine;

                    return;
                }

                if (!_databaseUpdater.PrepareLogin(this))
                    return;
            }
            catch
            {
                _rbProgress.Text += "Prepare database failed." + Environment.NewLine;
                return;
            }

            _rubProgressRun = true;
            _rbProgress.Height = 85;
            _pbWait.Visible = true;

            _bBack.Enabled = false;
            _bPerform.Enabled = false;

            IwQuick.Threads.SafeThread.StartThread(ThreadShowProgress);

            _databaseUpdater.Update(this);
        }

        private readonly DatabaseUpdater _databaseUpdater;

        /// <summary>
        /// Disable button cancel and insert text backup database to the _rbProgress before backup the database
        /// </summary>
        public void ShowStartDatabaseBackup()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowStartDatabaseBackup));
                return;
            }

            _bCancel.Enabled = false;
            _rbProgress.Text += GetString("TextStartBackupDatabase") + "\n";
        }

        /// <summary>
        /// Enable button cancel after backup the database
        /// </summary>
        public void ShowStopDatabaseBackup(bool createdDatabaseBackup)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<bool>(ShowStopDatabaseBackup), 
                    createdDatabaseBackup);

                return;
            }

            _bCancel.Enabled = true;

            _rbProgress.Text += 
                createdDatabaseBackup
                    ? GetString("TextBackupDatabaseSucceeded") + "\n"
                    : GetString("TextBackupDatabaseFailed") + "\n";
        }

        public void CloseSync()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(CloseSync));
                return;
            }

            Close();
        }

        public bool PerformVersionBasedDatabaseConversion(DatabaseCommandExecutor databaseCommandExecutor)
        {
            bool result = false;

            this.InvokeInUI(() =>
            {

                var formDbsConversion = new FormDbsConversion(databaseCommandExecutor);

                formDbsConversion.ShowDialog();

                result = formDbsConversion.Success;
            });

            return result;
        }

        public void OnUpdateFinished()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(OnUpdateFinished));
                return;
            }

            _pbWait.Value = 0;
            _pbWait.Visible = false;
            _rbProgress.Height = 97;

            if (_databaseUpdater.Result == DatabaseUpdater.ResultEnum.Done)
                ShowUpdateDatabaseSuccess();
            else
                ShowUpdateDatabaseFailure();

            _bBack.Enabled = true;
            _bPerform.Enabled = true;
        }

        public void StopProgress()
        {
            _rubProgressRun = false;
        }

        private void ShowUpdateDatabaseSuccess()
        {
            _rbProgress.Text += GetString("FormUdateDatabaseDatabaseAltered") + "\n";

            _bPerform.Text = GetString("FormUpdateDatabaseClose");

            _bCancel.Enabled = false;

            CompareOrmModel compareOrmModel = _databaseUpdater.CompareOrmModel;

            if (compareOrmModel != null && !compareOrmModel.DatabaseErrors.HasTableErrors)
                return;

            _rbProgress.Text += GetString("TextInDatabaseAreUselessColumns") + "\n";
            _bFixDbs.Visible = true;
        }

        private void ShowUpdateDatabaseFailure()
        {
            _bFixDbs.Visible = true;

            _rbProgress.Text += GetString("FormUpdateDatabaseErrorsDatabaseAlter") + "\n";
            _rbProgress.Text += "Click on " + GetString("FormUpdateDatabase_bFixDbs") + " to see error list" + "\n";

            if (!_databaseUpdater.CreatedDatabaseBackup)
                return;

            if (!Dialog.ErrorQuestion(
                    LocalizationHelper.GetString("QuestionUpdateFailedRestoreDatabase")))
                return;

            _bCancel.Enabled = false;
            _bFixDbs.Enabled = false;

            _rbProgress.Text += GetString("TextStartRestoreDatabase") + "\n";

            bool databaseRestoreSucceded =
                _databaseUpdater.RestoreDatabaseBackup();

            _rbProgress.Text +=
                databaseRestoreSucceded
                    ? GetString("TextRestoreDatabaseSucceeded") + "\n"
                    : GetString("TextRestoreDatabaseFailed") + "\n";

            _bCancel.Enabled = true;
            _bFixDbs.Enabled = true;
        }

        private void ThreadShowProgress()
        {
            while (_rubProgressRun)
            {
                RunProgresBar();
                System.Threading.Thread.Sleep(666);
            }
        }

        private void RunProgresBar()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(RunProgresBar));
                return;
            }

            if (_pbWait.Value >= 100)
                _pbWait.Value = 0;
            else
                _pbWait.Value++;
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _databaseUpdater.OnGoBack(this);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _databaseUpdater.OnCancel(this);
        }

        private void _bFixDbs_Click(object sender, EventArgs e)
        {
            var formAlterTables = new FormAlterTables(_databaseUpdater);

            formAlterTables.ShowDialog();
        }

        public bool PrepareCustomLogin()
        {
            var formDbsLogin = new FormDbsLogin(_databaseUpdater);

            if (formDbsLogin.ShowDialog() != DialogResult.OK)
                return false;

            _tbLogin.Text = _databaseUpdater.CreatorProperties.DatabaseLogin;

            return true;
        }

        private void FormUpdateDatabase_FormClosed(object sender, FormClosedEventArgs e)
        {
            _databaseUpdater.OnError -= InvokeOnError;
        }
    }
}
