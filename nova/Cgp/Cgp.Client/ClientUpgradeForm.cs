using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using System.Threading;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using System.IO;
using System.Diagnostics;

namespace Contal.Cgp.Client
{
    public partial class ClientUpgradeForm : CgpTranslateForm
    {
        private string _upgradePackageFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Temp\ClientUpgrade.gz");
        private string _upgraderProgramFullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Temp\ClientUpgrader.exe");
        private int _fileSize = 0;
        private volatile bool _finish = false;
        private Version _upgradeVersion = null;
        private string _upgraderUpdateFilename = Path.Combine(QuickPath.AssemblyStartupPath, @"Client Upgrader\ClientUpgrader.exe");

        public ClientUpgradeForm(int fileSize, Version upgradeVersion)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            _fileSize = fileSize;
            _upgradeVersion = upgradeVersion;
            SetMaxProgressValue(fileSize);
        }

        public void SetMaxProgressValue(int maxValue)
        {
            if (_pbUpgradeProgress.InvokeRequired)
                _pbUpgradeProgress.BeginInvoke(new DInt2Void(SetMaxProgressValue), maxValue);
            else
            {
                _pbUpgradeProgress.Maximum = maxValue;
            }
        }

        private void StartReceiveUpgradeFile()
        {
            if (CgpClient.Singleton.MainServerProvider == null)
            {
                Dialog.Error(GetString("ServerUnavailable"));
                return;
            }
            if (!QuickPath.EnsureDirectory(Path.GetDirectoryName(_upgradePackageFullFileName)))
            {
                Dialog.Error(GetString("DirectoryDoesNotExist") + " " + Path.GetDirectoryName(_upgradePackageFullFileName));
                return;
            }
            byte[] fileData = null;
            int bufferLength = 0;
            FileStream fs = null;
            while (!_finish)
            {
                fileData = CgpClient.Singleton.MainServerProvider.GetClientUpgradeData(_upgradeVersion, fs == null ? 0 : (int)fs.Position);
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
                        fs = new FileStream(_upgradePackageFullFileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferLength);
                    }
                    catch (Exception exc)
                    {
                        if (fs != null)
                            try { fs.Close(); }
                            catch (Exception ex) { }
                        fs = null;
                        Dialog.Error(GetString("Exception") + " (" + exc.Message + ")");
                        DialogResult = DialogResult.Abort;
                        break; ;
                    }
                }
                fs.Write(fileData, 0, fileData.Length);
                SetProgressValue((int)fs.Position);
                if (fileData.Length < bufferLength)
                {
                    try { fs.Close(); }
                    catch (Exception ex) { }
                    DialogResult = DialogResult.OK;
                    SetUpgradeProgressText(GetString("ClosingClientApplication"));
                    RunUpgradeApplication();
                    break;
                }
            }
            if (DialogResult == DialogResult.OK)
                Close();
        }

        private void RunUpgradeApplication()
        {
            if (File.Exists(_upgraderUpdateFilename))
            {
                try
                {
                    if (File.Exists(_upgraderProgramFullFileName))
                        File.Delete(_upgraderProgramFullFileName);
                    File.Copy(_upgraderUpdateFilename, _upgraderProgramFullFileName);
                }
                catch { }
            }

            if (!File.Exists(_upgraderProgramFullFileName))
            {
                DialogResult = DialogResult.Abort;
                Dialog.Error(GetString("FileDoesNotExist") + " (" + _upgraderProgramFullFileName + ")");
                return;
            }
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = _upgraderProgramFullFileName;
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(_upgraderProgramFullFileName);
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                string[] arguments = new string[] 
                    {
                        "\"" + _upgradePackageFullFileName + "\"",
                        "\"" + QuickPath.AssemblyStartupPath + "\"",
                        "\"" + Process.GetCurrentProcess().ProcessName + "\"",
                        "\"" + Process.GetCurrentProcess().Id.ToString() + "\""
                    };
                p.StartInfo.Arguments = string.Join(" ", arguments);
                p.Start();
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.Abort;
                Dialog.Error(GetString("Exception") + " (" + ex.Message + ")");
            }
        }

        private void SetUpgradeProgressText(string text)
        {
            if (_lUpgradeStage.InvokeRequired)
                _lUpgradeStage.BeginInvoke(new DString2Void(SetUpgradeProgressText), text);
            else
            {
                _lUpgradeStage.Text = text;
            }
        }

        private void SetProgressValue(int value)
        {
            if (_pbUpgradeProgress.InvokeRequired)
                _pbUpgradeProgress.BeginInvoke(new DInt2Void(SetProgressValue), value);
            else
            {
                _pbUpgradeProgress.Value = value;
            }
        }

        private void ClientUpgradeForm_Shown(object sender, EventArgs e)
        {
            SafeThread.StartThread(StartReceiveUpgradeFile);
        }

        private void ClientUpgradeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _finish = true;
        }

        private void _bStop_Click(object sender, EventArgs e)
        {
            _finish = true;
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
