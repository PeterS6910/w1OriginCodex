using System.Diagnostics;
using System;
using System.IO;
using System.Threading;
// ReSharper disable once RedundantUsingDirective
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Sys;

namespace Cgp.NCAS.CCUUpgrader
{
    class Program
    {
        private const string SD_PATH = @"\Storage card";
        private const string NAND_PATH = @"\NandFlash";
        private const string CCU_DIR = "ccu";
        private const string CCU_ERROR_LOG = "ccuupgrader.error.log";

        /// <summary>
        /// tries to create/define log file with path [rootPath]\ccu\ccu.error.log
        /// </summary>
        /// <param name="rootPath"></param>
        private static void DefineLogOnRoot(string rootPath)
        {
            try
            {
                if (!Directory.Exists(rootPath + StringConstants.BACKSLASH + CCU_DIR))
                    Directory.CreateDirectory(rootPath + StringConstants.BACKSLASH + CCU_DIR);
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR\tCCUUPGRADER\tUnable to create directory \"" + rootPath + StringConstants.BACKSLASH + CCU_DIR + "\"\n\t" +
                    err);

            }

            try
            {
                HandledExceptionAdapter.OutputFilePath =
                    rootPath + StringConstants.BACKSLASH +
                    CCU_DIR + StringConstants.BACKSLASH +
                    CCU_ERROR_LOG;
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR\tCCUUPGRADER\tUnable to define file for exception log \"" +
                    rootPath + StringConstants.BACKSLASH +
                    CCU_DIR + StringConstants.BACKSLASH +
                    CCU_ERROR_LOG
                    + "\"\n\t" + err);
            }
        }

        private static void ConfigHandledExceptionAdapter()
        {
            try
            {
                if (Directory.Exists(SD_PATH))
                {
                    DefineLogOnRoot(SD_PATH);
                }
                else
                    if (Directory.Exists(NAND_PATH))
                    {
                        DefineLogOnRoot(NAND_PATH);
                    }
                    else
                        Console.WriteLine("ERROR\tCCUUPGRADER\tUnable to find proper persistent storage");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR\tCCUUPGRADER\t" + e);
            }
        }

        

        private static readonly ExtendedVersion Version = new ExtendedVersion(typeof (Program), true,
#if DEBUG
            DevelopmentStage.Testing
#else
            DevelopmentStage.Release
#endif
             );

        static void Main(string[] args)
        {
            if (!Debugger.IsAttached)
                Thread.Sleep(10000);

            //DebugHelper.WaitForDebuggerToBreak();

            ConfigHandledExceptionAdapter();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            CcuUpgradeHandler.Log.Info("[ "+ Version + " ]");

            switch (args.Length)
            {
                case 2:

                    CcuUpgradeHandler.Singleton.RebootCCU(args[0], args[1]);
                    return;

                case 3:

                    CcuUpgradeHandler.Singleton.BeginUpgrade(
                        args[0], 
                        args[1], 
                        args[2], 
                        args, 
                        UpgradeFinish);

                    break;

                case 4:

                    CcuUpgradeHandler.Singleton.BeginUpgrade(
                        args[0], 
                        args[1], 
                        args[2], 
                        args[3], 
                        args, 
                        UpgradeFinish);

                    break;

                default:

                    CcuUpgradeHandler.Log.Error("Invalid argument count specified : "+args.Length);
                    return;
            }

            SafeThread signalingThread = SafeThread.StartThread(SignalWatchdog);
            CcuUpgradeHandler.Singleton.WaitForExit();

            if (signalingThread != null)
            {
                _signalWatchdog = false;
                signalingThread.Join(500);
            }
        }

        private static void UpgradeFinish(
            string dataPath, 
            string aplicationPath, 
            string fileToExecute, 
            bool success, 
            Exception ex)
        {
            CcuUpgradeHandler.Singleton.StopUpgrade();
        }

// ReSharper disable once NotAccessedField.Local
        private static bool _signalWatchdog = true;

        private static void SignalWatchdog()
        {
#if !DEBUG
            int thisProcessId = Process.GetCurrentProcess().Id;

            WatchdogControl.SetRebootTimeout(
                thisProcessId, 
                120000);

            while (_signalWatchdog)
            {
                WatchdogControl.SignalWatchdog();


                ASafeThreadBase.Sleep(thisProcessId, ref _signalWatchdog);
            }
#endif
        }

        /// <summary>
        /// should be invoked when exception would not be catch ; 
        /// in some cases this handler would not be invoked, therefore more for logging purpose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e == null)
                return;

            var exceptionExtracted = e.ExceptionObject as Exception;
            try
            {
                
                if (exceptionExtracted !=null)
                    HandledExceptionAdapter.Examine(exceptionExtracted);
            }
            catch
            {
            }

            try
            {
                Console.WriteLine("CCU UPGRADER UNHANDLED EXCEPTION\t" + (exceptionExtracted ?? e.ExceptionObject));
            }
            catch
            {
            }
        }
    }
}
