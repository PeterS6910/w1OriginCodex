using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.DBSCreator
{
    //[STAThread]
    public partial class FormDatabaseSettings : TranslateForm
    {
        private bool _canceled;
        private bool _goBack;
        private readonly CreatorProperties _creatorProperties;
        
        #region Properties
        public bool WasCanceled
        {
            get { return _canceled; }
        }
        public bool GoBack
        {
            get { return _goBack; }
        }
        #endregion

        public FormDatabaseSettings(
            LocalizationHelper localizationHelper, 
            CreatorProperties creatorProperties, 
            bool databaseExists, 
            bool externDatabaseExists)
            : base(localizationHelper)
        {
            InitializeComponent();

            _creatorProperties = creatorProperties;
            _eSqlServer.Text = _creatorProperties.DatabaseServer;
            _eDatabase.Text = _creatorProperties.DatabaseName;

            if (databaseExists || string.IsNullOrEmpty(_creatorProperties.DatabasePath))
            {
                _eDatabasePath.Text = string.Empty;
                _eDatabasePath.Enabled = false;
                _bBrowse.Enabled = false;
            }
            else
                _eDatabasePath.Text = _creatorProperties.DatabasePath;

            _eExternDatabase.Text = _creatorProperties.ExternDatabaseName;

            if (externDatabaseExists || string.IsNullOrEmpty(_creatorProperties.ExternDatabasePath))
            {
                _eExternDatabasePath.Text = string.Empty;
                _eExternDatabasePath.Enabled = false;
                _bBrowse1.Enabled = false;
            }
            else
                _eExternDatabasePath.Text = _creatorProperties.ExternDatabasePath;
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = true;

            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            _creatorProperties.RulesList = null;

            if (_eDatabasePath.Enabled && 
                _creatorProperties.DatabasePath != _eDatabasePath.Text)
            {
                try
                {
                    _creatorProperties.RulesList = new List<FileSystemAccessRule>();

                    var dirInfo = new DirectoryInfo(_creatorProperties.DatabasePath);
                    DirectorySecurity dirSec = dirInfo.GetAccessControl();

                    AuthorizationRuleCollection ruleCollection =
                        dirSec.GetAccessRules(
                            true,
                            true,
                            typeof(NTAccount));

                    foreach (FileSystemAccessRule rule in ruleCollection)
                        if (rule.IdentityReference.Value.ToUpper().IndexOf("SQLSERVER") >= 0)
                            _creatorProperties.RulesList.Add(rule);
                }
                catch
                {
                    _creatorProperties.RulesList = null;
                }
            }

            _creatorProperties.RulesListExternDatabase = null;

            if (_eExternDatabasePath.Enabled && 
                _creatorProperties.ExternDatabasePath != _eExternDatabasePath.Text)
            {
                try
                {
                    _creatorProperties.RulesListExternDatabase = new List<FileSystemAccessRule>();

                    var dirInfo = new DirectoryInfo(_creatorProperties.ExternDatabasePath);
                    DirectorySecurity dirSec = dirInfo.GetAccessControl();

                    AuthorizationRuleCollection ruleCollection = 
                        dirSec.GetAccessRules(
                            true, 
                            true, 
                            typeof(NTAccount));

                    foreach (FileSystemAccessRule rule in ruleCollection)
                        if (rule.IdentityReference.Value.ToUpper().IndexOf("SQLSERVER") >= 0)
                            _creatorProperties.RulesListExternDatabase.Add(rule);
                }
                catch
                {
                    _creatorProperties.RulesListExternDatabase = null;
                }
            }

            if (_eDatabasePath.Enabled)
            {
                _creatorProperties.DatabasePath = _eDatabasePath.Text;
                QuickPath.EnsureDirectory(_creatorProperties.DatabasePath);
            }

            if (_eExternDatabasePath.Enabled)
            {
                _creatorProperties.ExternDatabasePath = _eExternDatabasePath.Text;
                QuickPath.EnsureDirectory(_creatorProperties.ExternDatabasePath);
            }

            _canceled = false;
            _goBack = false;

            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            _goBack = false;

            Close();
        }

        private void _bBrovse_Click(object sender, EventArgs e)
        {
            _folderBrowser.SelectedPath = _eDatabasePath.Text;

            var thread = new Thread(ShowFolderBrowser);
            thread.SetApartmentState(ApartmentState.STA);

            Enabled = false;

            thread.Start();
        }

        private void ShowFolderBrowser()
        {
            try
            {
                _folderBrowser.ShowDialog();
            }
            catch
            {
            }

            EnabledForm(false);
        }

        private void ShowFolderBrowserExternDatabase()
        {
            try
            {
                _folderBrowser.ShowDialog();
            }
            catch
            {
            }

            EnabledForm(true);
        }

        private void EnabledForm(bool externDatabase)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DBool2Void(EnabledForm), externDatabase);
                return;
            }

            if (externDatabase)
                _eExternDatabasePath.Text = _folderBrowser.SelectedPath;
            else
                _eDatabasePath.Text = _folderBrowser.SelectedPath;

            Enabled = true;

            Activate();
        }

        private void _bBrowse1_Click(object sender, EventArgs e)
        {
            _folderBrowser.SelectedPath = _eExternDatabasePath.Text;

            var thread = new Thread(ShowFolderBrowserExternDatabase);
            thread.SetApartmentState(ApartmentState.STA);

            Enabled = false;

            thread.Start();
        }
    }
}
