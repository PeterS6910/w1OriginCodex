using System;
using System.Diagnostics;
using System.Windows.Forms;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.DBSCreator
{
    public partial class FormDbsConversion : TranslateForm
    {
        private readonly Stopwatch _conversionWatch;
        private volatile bool _showProgress;

        private readonly VersionBasedDatabaseConversion _versionBasedDatabaseConversion;

        public FormDbsConversion(DatabaseCommandExecutor databaseCommandExecutor)
            : base(databaseCommandExecutor.LocalizationHelper)
        {
            InitializeComponent();

            _versionBasedDatabaseConversion = new VersionBasedDatabaseConversion(databaseCommandExecutor);

            _versionBasedDatabaseConversion.ShowMessage += ShowMessage;

            //_eInfo.Text = GetString("PerformConversion");
            //_eInfo.Text += Environment.NewLine;

            StartProgressBar();
            IwQuick.Threads.SafeThread.StartThread(ThreadPerformConversion);

            _conversionWatch = Stopwatch.StartNew();
        }

        private void StartProgressBar()
        {
            _showProgress = true;
            _pb.Visible = true;

            IwQuick.Threads.SafeThread.StartThread(DoStepsOnProgressBar);
        }

        private void StopProgressBar()
        {
            _showProgress = false;
        }

        private void DoStepsOnProgressBar()
        {
            while (_showProgress)
            {
                if (IsHandleCreated)
                    BeginInvoke(new MethodInvoker(UpdateProgressBar));

                System.Threading.Thread.Sleep(1000);
            }

            if (IsHandleCreated)
                BeginInvoke(new MethodInvoker(HideProgressBar));
        }

        private void HideProgressBar()
        {
            _pb.Value = 0;
            _pb.Visible = false;
        }

        private void UpdateProgressBar()
        {
            TimeSpan t = _conversionWatch.Elapsed;

            _lConversionTime.Text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);

            if (_pb.Value == 100)
                _pb.Value = 0;

            _pb.Value++;
        }

        public bool Success { get; private set; }

        private void _bContinue_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ThreadPerformConversion()
        {
            try
            {
                if (_versionBasedDatabaseConversion.Execute())
                    Success = true;
            }
            catch
            {
                Success = false;
            }
            finally
            {
                _conversionWatch.Stop();

                StopProgressBar();
                EnableContinueButton();
            }
        }

        private void EnableContinueButton()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new IwQuick.DVoid2Void(EnableContinueButton));
                return;
            }

            _bContinue.Enabled = true;
        }

        private void ShowMessage(string text, bool translate)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, bool>(ShowMessage), text, translate);
                return;
            }

            _eInfo.Text += DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss ") + (translate ? GetString(text) : text);
            _eInfo.Text += Environment.NewLine;
        }

        private void FormDbsConversion_FormClosed(object sender, FormClosedEventArgs e)
        {
            _versionBasedDatabaseConversion.ShowMessage -= ShowMessage;
        }
    }
}
