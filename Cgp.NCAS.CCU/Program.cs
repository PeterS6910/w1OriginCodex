using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

using System.IO;

namespace Contal.Cgp.NCAS.CCU
{
    class Program
    {

        private static void GeneralError(object error)
        {
            try
            {
                Console.WriteLine("\r\n[" + CcuCore.LocalTime + "]\r\n\tERROR CCU MAIN : \r\n\t" + (error ?? "NULL") +
                                  "\r\n");
            }
            catch
            {
                
            }
        }

        
        private static bool _sdCardPresent = false;

        public static bool SDCardPresent
        {
            get { return _sdCardPresent; }
        }

        private const int RETRY_COUNT_FOR_CHECK_SD_CARD = 3;
        private const int DELAY_BETWEEN_CHECKING_SD_CARD = 1000;

        /// <summary>
        /// main routine
        /// </summary>
        static void Main()
        {
            // not needed, will be handled by HandledExceptionAdapter.UseLogClass = DebugLog in CcuCore.DebugLog singleton
            //ConfigHandledExceptionAdapter();
            try
            {
                try
                {
                    AppDomain.CurrentDomain.UnhandledException += OnCCUUnhadnledException;
                }
                catch
                {

                }

                try
                {
                    var ccuProcessInfos = ProcessInfo.GetAllProcessInfos()
                        .Where(info => info.Name.Equals("ccu.exe", StringComparison.InvariantCultureIgnoreCase));

                    var currentCcuProcessVersion =
                        IwQuick.Sys.Microsoft.FileVersionInfo.GetVersionInfo(QuickPath.ApplicationFilePath);

                    var currentProcessId = Process.GetCurrentProcess().Id;

                    foreach (var ccuProcessInfo in ccuProcessInfos)
                    {
                        if (ccuProcessInfo.PID == currentProcessId)
                            continue;

                        var ccuProcessVersion = IwQuick.Sys.Microsoft.FileVersionInfo.GetVersionInfo(ccuProcessInfo.Path);

                        if (ccuProcessVersion >=
                            currentCcuProcessVersion)
                        {
                            Console.WriteLine(
                                string.Format(
                                    "This ccu.exe with version '{0}' will be terminated because ccu.exe with version '{1}' already runs",
                                    currentCcuProcessVersion,
                                    ccuProcessVersion));

                            return;
                        }

                        Console.WriteLine(
                            string.Format(
                                "Ccu.exe with version '{0}' will be terminated because ccu.exe with version '{1}' is starting",
                                ccuProcessVersion,
                                currentCcuProcessVersion));

                        var oldCcuProcess = ccuProcessInfo.ProcessInstance;

                        oldCcuProcess.Kill();
                        oldCcuProcess.WaitForExit(15000);

                        CcuCore.DebugLog.Warning(
                            Log.CALM_LEVEL,
                            string.Format(
                                "Ccu.exe with version '{0}' was terminated because ccu.exe with version '{1}' is starting",
                                ccuProcessVersion,
                                currentCcuProcessVersion));
                    }

                    int i = RETRY_COUNT_FOR_CHECK_SD_CARD;
                    while (i > 0)
                    {
                        if (SystemUtility.SDcard.CardPresent)
                        {
                            _sdCardPresent = true;
                            break;
                        }

                        i--;
                        Thread.Sleep(DELAY_BETWEEN_CHECKING_SD_CARD);
                    }

                    if (_sdCardPresent)
                    {
                        try
                        {
                            if (SystemUtility.SDcard.PartitionType == SystemUtility.FatVersion.FAT32)
                                PatchedFileStream.ImplicitUseFlushPatch = false;
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {
                    Thread.Sleep(DELAY_BETWEEN_CHECKING_SD_CARD);

                    try
                    {
                        _sdCardPresent = Directory.Exists(CcuCore.STORAGE_CARD_PATH);
                    }
                    catch
                    {

                    }
                }

                //ThreadPool.SetMaxThreads(24, 16);
                try
                {
                    int tc1, tc2;
                    ThreadPool.GetMaxThreads(out tc1, out tc2);

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Starting with ThreadPool settings " + tc1 + " " + tc2);
                }
                catch
                {
                    
                }


                // ensure the singleton creation here
                var v = CcuCore.Singleton.GetHashCode();

                CcuCore.Singleton.WaitForExit();

                DebugHelper.Keep(v);
            }
            catch(Exception generalError)
            {
                GeneralError(generalError.Message);
            }
            finally
            {
                try
                {
                    IOControl.Close();
                }
                catch
                {
                    
                }
            }
        }

        

        /// <summary>
        /// should be invoked when exception would not be catch ; 
        /// in some cases this handler would not be invoked, therefore more for logging purpose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnCCUUnhadnledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception)
                    HandledExceptionAdapter.Examine((Exception)e.ExceptionObject);
            }
            catch
            {
            }

            GeneralError(e.ExceptionObject);
        }
    }
}
