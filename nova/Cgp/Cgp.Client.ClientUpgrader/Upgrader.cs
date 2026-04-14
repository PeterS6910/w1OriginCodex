using System;
using Contal.IwQuick;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client.ClientUpgrader
{
    public class Upgrader
    {
        private readonly string _backupDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Temp\Backup\");

        string _upgradeFile = null;
        string _targetDirectory = null;
        string _programToStart = null;
        int _killProcessId = 0;
        private readonly Log _log;


        internal Upgrader(Log log)
        {
            _log = log;
        }

        public void StartUpgrade(string upgradeFile, string targetDirectory, string processToStart, int killProcessId)
        {
            if (string.IsNullOrEmpty(upgradeFile) || string.IsNullOrEmpty(targetDirectory)
                || string.IsNullOrEmpty(processToStart) || killProcessId <= 0)
            {
                _log.Error("Null or empty argument passed");
                Thread.Sleep(10000);
                return;
            }
            _upgradeFile = upgradeFile;
            _targetDirectory = targetDirectory;
            _programToStart = processToStart;
            _killProcessId = killProcessId;

            DebugHelper.NOP(_upgradeFile, _killProcessId);

            var process = Process.GetProcessById(killProcessId);
            process.Kill();
            if (!process.WaitForExit(10000))
            {
                _log.Error("Failed to stop client, try to stop manually and then press ENTER. Otherwise, press any other key to exit upgrade.");
                if (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    return;
                }
            }
            TryCreateBackup();
            if (!EnsureRemovedClientDirectory())
            {
                _log.Error("Failed to remove client directory. Make sure there is no application that can use directory: " + _targetDirectory + ".");
                Thread.Sleep(10000);
                return;
            }
            var fp = new FilePacker();
            fp.UnpackingProgress += FilePackerUnpackingProgress;
            if (!fp.TryUnpack(upgradeFile, targetDirectory))
            {
                if (_backupDone)
                {
                    _log.Error("Failed to unpack upgrade package. System will be restored from backup.");
                    if (!DoRestore())
                        _log.Error("Failed to restore an application from backup. Contact administrator.");
                    else
                        _log.Info("Application restored.");
                }
                else
                {
                    _log.Error("Failed to unpack upgrade package. Backup does not exist. Contact administrator.");
                }
                Thread.Sleep(10000);
                return;
            }
            if (!StartUpgradedApplication())
                _log.Error("Failed to run upgraded application. Try to run it manually.");
            else
                _log.Info("Application successfully run.");
            Thread.Sleep(10000);
        }

        private bool EnsureRemovedClientDirectory()
        {
            try
            {
                if (Directory.Exists(_targetDirectory))
                    Directory.Delete(_targetDirectory, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool StartUpgradedApplication()
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = _programToStart,
                        UseShellExecute = true,
                        WorkingDirectory = _targetDirectory
                    }
                };
                process.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool DoRestore()
        {
            try
            {
                Directory.Move(_backupDirFullName, _targetDirectory);
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = _programToStart,
                        UseShellExecute = true,
                        WorkingDirectory = _targetDirectory
                    }
                };
                process.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void FilePackerUnpackingProgress(bool isFile, string name)
        {
            _log.Info("Unpacking " + (isFile ? "file " : " directory ") + name);
        }

        private bool _backupDone = false;
        private void TryCreateBackup()
        {
            try
            {
                if (Directory.Exists(_backupDirFullName))
                    Directory.Delete(_backupDirFullName, true);
                Directory.Move(_targetDirectory, _backupDirFullName);
                _log.Info("Backup was successfully created");
                _backupDone = true;
            }
            catch (Exception ex)
            {
                _log.Error("An exception occured while creating backup. Exception: " + ex.Message + ". Upgrade process will continue without backup");
                _backupDone = false;
            }
        }
    }
}
