using System;
using System.Windows.Forms;

using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    internal partial class FormBeforeExecution : TranslateForm, IFormBeforeExecution, ICreateOrUpdateForm
    {
        private bool _doneOk;

        private volatile bool _doShowProgress;

        public event DException2Void OnError;

        private IwQuick.Threads.SafeThread _threadProgress;
        private IwQuick.Threads.SafeThread _threadCreateDatabase;

        private readonly DatabaseCreator _databaseCreator;

        #region Properties

        public bool WasCanceled { get; private set; }
        public bool GoBack { get; private set; }

        #endregion

        public FormBeforeExecution(
            DatabaseCreator databaseCreator)
            : base(databaseCreator.LocalizationHelper)
        {
            InitializeComponent();

            _databaseCreator = databaseCreator;

            _databaseCreator.OnError += InvokeOnError;

            CreatorProperties creatorProperties =
                _databaseCreator.CreatorProperties;

            _eDatabase.Text = creatorProperties.DatabaseName;
            _eSqlServer.Text = creatorProperties.DatabaseServer;
            _tbLogin.Text = creatorProperties.DatabaseLogin;

            _eExternDatabase.Text =
                /*creatorProperties.EnableExternDatabase 
                    ?*/ creatorProperties.ExternDatabaseName;
            //: string.Empty;
        }

        private void _bPerform_Click(object sender, EventArgs e)
        {
            WasCanceled = false;
            GoBack = false;

            if (_doneOk)
            {
                Close();
                return;
            }

            if (!CheckDatabaseAndLogin())
            {
                _rbProgress.Text += "\n " + GetString("FormBeforeExecutionDatabaseCannotByCreated") + " \n";
                return;
            }

            _doShowProgress = true;
            _rbProgress.Height = 85;
            _pbWait.Visible = true;
            _bPerform.Enabled = false;
            _bBack.Enabled = false;

            _threadProgress = IwQuick.Threads.SafeThread.StartThread(ThreadShowProgress);

            _threadCreateDatabase = new IwQuick.Threads.SafeThread(ThreadCreateDatabase);

            _threadCreateDatabase.OnFinished += StopProgress;
            _threadCreateDatabase.OnException += StopProgress;

            _threadCreateDatabase.Start();
        }

        private void StopProgress(Exception ex)
        {
            StopProgress();
        }

        private void StopProgress()
        {
            _doShowProgress = false;
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            WasCanceled = false;
            GoBack = true;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            if (_threadCreateDatabase != null)
            {
                if (_threadCreateDatabase.IsStarted)
                {
                    if (!Dialog.Question(GetString("QuestionCancelDbConfiguration")))
                        return;

                    _threadCreateDatabase.Abort();
                    
                }

                _threadCreateDatabase = null;
            }

            if (_threadProgress != null)
            {
                _threadProgress.Stop(1000);
                _threadProgress = null;
            }

            WasCanceled = true;
            GoBack = false;

            Close();
        }

        private void InvokeOnError(Exception inputError)
        {
            if (OnError != null) 
                OnError(inputError);
        }

        public bool PrepareCustomLogin()
        {
            var formDbsLogin = new FormDbsLogin(_databaseCreator);

            if (formDbsLogin.ShowDialog() != DialogResult.OK)
                return false;

            _tbLogin.Text = _databaseCreator.CreatorProperties.DatabaseLogin;

            return true;
        }

        private bool CheckDatabaseAndLogin()
        {
            if (_databaseCreator.DatabasesExist)
            {
                _rbProgress.Text +=
                    GetString("FormBeforeExecutionDatabaseExists") + "\n";

                return false;
            }

            return _databaseCreator.PrepareLogin(this);
        }

        private void ThreadCreateDatabase()
        {
            _databaseCreator.Create(this);
        }

        public void ShowCreateDatabaseSuccess()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowCreateDatabaseSuccess));
                return;
            }

            _rbProgress.Text += GetString("FormBeforeExecutionDatabaseCreated") + "\n";
            _rbProgress.Text += GetString("FormBeforeExecutionUserCreated") + "\n";
        }

        public void ShowCreateTablesFailure()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowCreateTablesFailure));
                return;
            }

            _rbProgress.Text += GetString("FormBeforeExecutionCreationTablesFail") + "\n";

            ShowCreateFailureCommon();
        }


        public void ShowBeforeExecutionSuccess()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowBeforeExecutionSuccess));
                return;
            }

            _rbProgress.Text += GetString("FormBeforeExecutionTablesCreated") + "\n";

            _pbWait.Value = 0;
            _pbWait.Visible = false;
            _rbProgress.Height = 97;

            _doneOk = true;

            _bBack.Enabled = false;
            _bCancel.Enabled = false;

            _bPerform.Text = GetString("FormBeforeExecutionFinished");

            _bPerform.Enabled = true;
            _bBack.Enabled = true;
        }

        public void ShowCreateDatabaseFailure()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowCreateDatabaseFailure));
                return;
            }

            if (!_databaseCreator.DatabaseCreated)
                _rbProgress.Text +=
                    GetString("FormBeforeExecutionDatabaseCreationFailed") + "\n";

            if (!_databaseCreator.UserCreated)
                _rbProgress.Text +=
                    GetString("FormBeforeExecutionCreationUserFail") + "\n";

            if (!string.IsNullOrEmpty(_databaseCreator.ErrorMessage))
                _rbProgress.Text += _databaseCreator.ErrorMessage + '\n';

            ShowCreateFailureCommon();
        }

        private void ShowCreateFailureCommon()
        {
            _pbWait.Value = 0;
            _pbWait.Visible = false;

            _rbProgress.Height = 97;

            _bPerform.Enabled = true;
            _bBack.Enabled = true;

            Dialog.Error(GetString("FormBeforeExecutionDatabaseCreationFailed"));
        }

        private void ThreadShowProgress()
        {
            while (_doShowProgress)
            {
                UpdateProgresBar();
                System.Threading.Thread.Sleep(666);
            }
        }

        private void UpdateProgresBar()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(UpdateProgresBar));
                return;
            }

            if (_pbWait.Value >= 100)
                _pbWait.Value = 0;
            else
                _pbWait.Value++;
        }

        private void FormBeforeExecution_FormClosed(object sender, FormClosedEventArgs e)
        {
            _databaseCreator.OnError -= InvokeOnError;
        }
    }
}