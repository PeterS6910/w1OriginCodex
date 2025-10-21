using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Contal.Cgp.Globals;
using Contal.Cgp.ORM;
using Contal.IwQuick;
using Contal.IwQuick.Parsing;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.UI;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Contal.Cgp.Server
{
    /// <summary>
    /// class for process of Cgp.Server
    /// </summary>
    class Program
    {

        private static DllKernel32.DCtrlEventHandler _handler;

        /// <summary>
        /// handles Ctrl signals especially in console mode
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        private static bool CtrlHandler(DllKernel32.CtrlType signal)
        {
            switch (signal)
            {
                case DllKernel32.CtrlType.CTRL_C_EVENT:
                    return true;
                case DllKernel32.CtrlType.CTRL_LOGOFF_EVENT:
                case DllKernel32.CtrlType.CTRL_SHUTDOWN_EVENT:
                case DllKernel32.CtrlType.CTRL_CLOSE_EVENT:
                default:
                    CgpServer.Singleton.StopProcessing();
                    CgpServer.Singleton.StopServiceIfRunning();
                    return false;
            }
        }

        /// <summary>
        /// ensures, that the close button won't be visible in console mode
        /// </summary>
        private static void ConsoleUnclosable()
        {
            try
            {
                _handler += CtrlHandler;
                DllKernel32.SetConsoleCtrlHandler(_handler, true);
                // catch if anything goes wrong, but not that important
                IntPtr hMenu = Process.GetCurrentProcess().MainWindowHandle;
                IntPtr hSystemMenu = DllUser32.GetSystemMenu(hMenu, false);
                DllUser32.EnableMenuItem(hSystemMenu, DllUser32.SC_CLOSE, DllUser32.MF_GRAYED);
                DllUser32.RemoveMenu(hSystemMenu, DllUser32.SC_CLOSE, DllUser32.MF_BYCOMMAND);
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isConsole"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private static LicenseValidity CheckLicensing(bool isConsole, Log log)
        {
            if (LicenseHelper.Singleton.DemoLicence)
            {
                return RunDemoLicense(log);
            }

            LicenseHelper.Singleton.CheckLicence();

            if (LicenseHelper.Singleton.IsValid)
            {
                return LicenseValidity.Valid;
            }

            log.Error("Invalid or no license found");
            if (isConsole)
            {
                GeneralOptionsForm generalOptionsForm =
                    new GeneralOptionsForm(CgpServer.Singleton.LocalizationHelper, GeneralOptions.Singleton);

                DialogResult dialogResult = generalOptionsForm.ShowDialog();

                if (dialogResult != DialogResult.Cancel)
                {
                    if (LicenseHelper.Singleton.DemoLicence)
                    {
                        return RunDemoLicense(log);
                    }

                    LicenseHelper.Singleton.CheckLicence();

                    if (LicenseHelper.Singleton.IsValid)
                    {
                        return LicenseValidity.Valid;
                    }
                }
            }
            else
            {
                Thread.Sleep(5000);
            }

            return LicenseValidity.Invalid;
        }

        private static LicenseValidity RunDemoLicense(Log log)
        {
            CgpServer.Singleton.DemoLicense = true;
            CgpServer.Singleton.DemoStartTime = DateTime.Now;

            if (log != null)
                log.Warning("\r\nStarting under DEMO license.\r\nSystem will be running only for " +
                            LicenseHelper.DEMO_MINUTES_WORKING + " minutes.");

            Contal.IwQuick.Threads.TimerManager.Static.StartTimeout(1000 * 60 * LicenseHelper.DEMO_MINUTES_WORKING, null,
                TimerTick);

            return LicenseValidity.Demo;
        }

        /// <summary>
        /// VS installer does not make the registered service with Delayed flag,
        /// so this call makes it manually
        /// </summary>
        private static void FixDelayedAutomaticStart()
        {
            /// fix for delayed automatic start for OS Vista,7,...
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    RegistryKey key;
                    if (RegistryHelper.TryParseKey(@"HKLM\SYSTEM\CurrentControlSet\services\" + CgpServerGlobals.CGP_SERVICE_NAME, false, out key))
                    {
                        int startMode = (int)key.GetValue("Start");

                        if (startMode == 2)
                        {
                            int delayedAutostart;
                            try
                            {
                                delayedAutostart = (int)key.GetValue("DelayedAutoStart", 0);
                            }
                            catch(Exception error)
                            {
                                delayedAutostart = 0;
                                HandledExceptionAdapter.Examine(error);
                            }

                            if (delayedAutostart == 0)
                                key.SetValue("DelayedAutoStart", 1);
                        }

                        key.Close();
                    }
                }
            }
            catch (Exception error) { HandledExceptionAdapter.Examine(error); }
        }

        /// <returns>system account under which the service is running</returns>
        private static string GetServiceUser()
        {
            string serviceUser = CgpServerGlobals.CGP_SERVICE_USER;

            try
            {
                System.Management.SelectQuery query = new System.Management.SelectQuery(string.Format("select startname from Win32_Service where name = '{0}'", CgpServerGlobals.CGP_SERVICE_NAME));
                using (System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(query))
                {
                    foreach (System.Management.ManagementObject service in searcher.Get())
                    {
                        serviceUser = (string)service["StartName"];
                    }
                }
            }
            catch(Exception error)
            {
                serviceUser = CgpServerGlobals.CGP_SERVICE_USER;
                HandledExceptionAdapter.Examine(error);
            }

            int position = serviceUser.LastIndexOf('\\');
            if (position > 0)
            {
                serviceUser = serviceUser.Substring(position + 1);
            }

            return serviceUser;
        }

        private static int _isRunningAsConsole = -1;
        /// <summary>
        /// returns true, when process is run as usual, therefore console application
        /// </summary>
        /// <returns></returns>
        private static bool IsRunningAsConsole()
        {
            if (_isRunningAsConsole >= 0)
            {
                return (_isRunningAsConsole != 0);
            }
            else
            {
                bool isConsole = false;

                try
                {
                    TextReader tr = Console.In;
                    if (System.IO.StreamReader.Null != tr)
                        isConsole = true;
                }
                catch
                {
                    // no need to capture exception here
                }

                _isRunningAsConsole = isConsole ? 1 : 0;

                return isConsole;
            }
        }

        /// <summary>
        /// main service processing
        /// </summary>
        /// <param name="log"></param>
        private static void RunService(Log log)
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new CgpServerService() };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                log.Error("\r\nService starting failed with error :\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// main console processing
        /// </summary>
        /// <param name="log"></param>
        private static void RunConsole(Log log)
        {
            using (CgpServer server = CgpServer.Singleton)
            {
                server.StartProcessing(false, true);
                server.WaitForExit();
            }
        }

        /// <summary>
        /// initializes capturing of handled exceptions
        /// </summary>
        private static void SetupErrorLogging()
        {
            bool errorLoggingSet = false;
            try
            {
                string sysTemp = Environment.GetEnvironmentVariable("TEMP");

                if (Validator.IsNotNullString(sysTemp))
                {
                    IwQuick.Sys.HandledExceptionAdapter.OutputFilePath = sysTemp + @"\ContalNova.Server.error.log";
                    errorLoggingSet = true;
                }
            }
            catch
            {
                // no exception capturing here
            }

            if (!errorLoggingSet)
                try
                {
                    string sysTemp = Environment.GetEnvironmentVariable("SystemRoot");

                    if (Validator.IsNotNullString(sysTemp))
                    {
                        IwQuick.Sys.HandledExceptionAdapter.OutputFilePath = sysTemp + @"\Temp\ContalNova.Server.error.log";
                        errorLoggingSet = true;
                    }
                }
                catch
                {
                    // no exception capturing here
                }

            if (!errorLoggingSet)
                Console.WriteLine("\nError logging capabilities would not be present\n");
        }

        private const int ERROR_READING_GRACE_TIMEOUT = 5000;

        /// <summary>
        /// deallocates the global mutex, so other instances can be run
        /// </summary>
        /// <param name="globalMutex"></param>
        private static void FinalizeGlobalMutex(ref Mutex globalMutex)
        {
            try
            {
                if (null != globalMutex)
                    globalMutex.Close();
            }
            catch (Exception error) { HandledExceptionAdapter.Examine(error); }
            finally
            {
                globalMutex = null;
            }
        }

        //[STAThread] CANNOT BE USED WHEN SERVER RUN AS SERVICE DUE MEMORY CONSUMPTION 
        /// <summary>
        /// main initialisation
        /// </summary>
        static void Main(string[] args)
        {

            SetupErrorLogging();           


            bool isFirstInstance;
            Mutex globalMutex = null;
            LicenseValidity licenseValidity = LicenseValidity.Valid;    // later conditioned by DEBUG
                                                                        // in DEBUG the license should be always valid
            Contal.IwQuick.Log logCgpServer = new Contal.IwQuick.Log(
                IsRunningAsConsole()?CgpServerGlobals.CGP_CONSOLE_NAME :CgpServerGlobals.CGP_SERVICE_NAME,
                true,
                true, 
                false);

            try
            {
                globalMutex = new Mutex(true, @"Global\" + CgpServerGlobals.NOVA_SERVER, out isFirstInstance);
            }
            catch
            {
                isFirstInstance = false;
            }

            if (!isFirstInstance)
                CgpServer.Singleton.SetIsNotFirst();

            try
            {
                ConnectionString.EnsureEncryptedDatabaseLoginPassword();

#if DEBUG
                if (QuickParser.IsArgumentPresent(args, false, "/verifynh"))
                {
                    using (CgpServer server = CgpServer.Singleton)
                    {
                        server.StartProcessing(false, GeneralMode.Verification, true);
                        server.WaitForExit();
                    }
                }
#endif

                if (QuickParser.IsArgumentPresent(args, false, "/configure", "/maintenance", "/config"))
                {
                    ConsoleUnclosable();

                    FixDelayedAutomaticStart();

                    FinalizeGlobalMutex(ref globalMutex); // the mutex must be released before the service can be started
                    // otherwise it would block service's start

                    using (CgpServer server = CgpServer.Singleton)
                    {
                        server.StartProcessing(false, GeneralMode.Maintenance, true);
                        server.WaitForExit();
                    }
                }

                if (QuickParser.IsArgumentPresent(args, false, "/noservice", "/nosvc", "/console"))
                {
                    if (!isFirstInstance)
                    {
                        ExtendedConsole.WarningWriteLine(
                            "\"" + Cgp.Globals.CgpServerGlobals.NOVA_SERVER + "\"" +
                            " application is already running");
                        ExtendedConsole.InfoWriteLine("Exiting...");
                        Thread.Sleep(3000);

                    }
                    else
                    {
                        ConsoleUnclosable();
#if !DEBUG
                        licenseValidity = CheckLicensing(true, logCgpServer);
#endif
                        if (licenseValidity == LicenseValidity.Valid || licenseValidity == LicenseValidity.Demo)
                        {
                            RunConsole(logCgpServer);
                        }
                    }
                }

                //System.Diagnostics.Debugger.Break();

                if (args.Length == 0) // this can be either console or service
                {
                    if (!ConnectionString.IsDatabaseLoginPasswordEncrypted())
                    {
                        logCgpServer.Error("\r\nService starting failed with error :\r\n Not encrypted database password \r\n Please run regonfigure Contal Nova");
                        throw new InvalidDataException();
                    }

                    if (isFirstInstance)
                    {

#if !DEBUG
                        licenseValidity = CheckLicensing(false, logCgpServer);
#endif
                        if (licenseValidity == LicenseValidity.Valid || licenseValidity == LicenseValidity.Demo)
                        {
                            //string serviceUser = GetServiceUser();

                            //if (true)//System.Environment.UserName == serviceUser)
                            if (IsRunningAsConsole())
                            {
                                try
                                {
                                    ServiceController sc = new ServiceController(CgpServerGlobals.CGP_SERVICE_NAME);
                                    if (sc.Status == ServiceControllerStatus.Stopped)
                                    {
                                        try
                                        {
                                            logCgpServer.Info("\r\nFound installed service " + CgpServerGlobals.CGP_SERVICE_NAME + "\r\n\tTrying to start...\r\n");

                                            FinalizeGlobalMutex(ref globalMutex); // the mutex must be released before the service will be started
                                            // otherwise it would block service's start
                                            sc.Start();

                                            logCgpServer.Info("\r\nService " + CgpServerGlobals.CGP_SERVICE_NAME + " successfuly started\r\n");
                                        }
                                        catch (Exception error)
                                        {
                                            HandledExceptionAdapter.Examine(error);
                                            logCgpServer.Error("\r\nUnable to start " + CgpServerGlobals.NOVA_SERVER + " installed as service " + CgpServerGlobals.CGP_SERVICE_NAME + "\r\n\t" +
                                                error.Message + "\r\n");
                                        }
                                    }
                                    else
                                    {
                                        logCgpServer.Info("\r\nService " + CgpServerGlobals.CGP_SERVICE_NAME + " is already started\r\n\r\n"+
                                            "In case you want to use the console mode, run the application with /console argument\r\n");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    HandledExceptionAdapter.Examine(ex);
                                    string tmpText = "\r\n" + CgpServerGlobals.NOVA_SERVER + " is not installed as service " + CgpServerGlobals.CGP_SERVICE_NAME + "\r\n\t"
                                        + ex.Message+"\r\n";
                                    logCgpServer.Error(tmpText);
                                }

                            }
                            else
                            {
                                RunService(logCgpServer);
                            }



                            //try
                            //{
                            //    ServiceController sc = new ServiceController(CgpServerGlobals.CGP_SERVICE_NAME);
                            //    if (sc.Status == ServiceControllerStatus.Stopped)
                            //    {
                            //        sc.Start();
                            //    }
                            //}
                            //catch
                            //{
                            //    string tmpText = "\r\n" + CgpServerGlobals.CGP_SERVER + " is not installed as service " + CgpServerGlobals.CGP_SERVICE_NAME + " !!!\r\n" +
                            //        CgpServerGlobals.CGP_SERVER + " will run as console application now.";

                            //    logCgpServer.Error(tmpText);

                            //    ConsoleUnclosable();
                            //    using (CgpServer server = CgpServer.Singleton)
                            //    {
                            //        server.StartProcessing(false, true);
                            //        server.WaitForExit();
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        ExtendedConsole.WarningWriteLine(
                            Cgp.Globals.CgpServerGlobals.NOVA_SERVER +
                            " application is already running either as console or service\r\n"
                            +"In case you want to use the console mode, run the application with /console argument\r\n");
                    }
                }
            }
            catch(InvalidDataException idex){
                // if database password is not encrypted
                HandledExceptionAdapter.Examine(idex);
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                FinalizeGlobalMutex(ref globalMutex);
            }

            if (IsRunningAsConsole())
            {
                int count = ERROR_READING_GRACE_TIMEOUT / 1000;

                for (int i = 0; i < count; i++)
                {
                    ExtendedConsole.InfoWrite("\rExiting in "+(count - i) + "s");
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// ticks when demo period expires
        /// </summary>
        /// <param name="timerCarrier"></param>
        /// <returns></returns>
        private static bool TimerTick(IwQuick.Threads.TimerCarrier timerCarrier)
        {
            if (CgpServer.Singleton.DemoLicense)
            {
                CgpServer.Singleton.DemoPeriodHasExpired();
                CgpServer.Singleton.StopProcessing();
                CgpServer.Singleton.StopServiceIfRunning();
            }
            
            return true;
        }
    }
}
