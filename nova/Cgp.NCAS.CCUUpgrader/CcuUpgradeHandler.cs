using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;
using Contal.Drivers.LPC3250;
using System.Xml;
using Contal.LwRemoting2;
using Contal.LwSerialization;
using JetBrains.Annotations;

namespace Cgp.NCAS.CCUUpgrader
{
    public enum CcuApplication
    {
        CCU,
        CCUUpgrader,
        Unknown
    }

    public class CcuUpgradeHandler
    {
        private const int FIRST_BLOCK_IN_RTC_MEMORY_FOR_SAVING_EVENT_ID_TRESHOLD = 24;

        private static volatile CcuUpgradeHandler _singleton;
        private static readonly object _syncRoot = new object();
        private readonly string _serverHashCode = string.Empty;

        internal static readonly Log Log = new Log("CCUupgrader", true,
#if DEBUG
            true
#else
            false
#endif
        , null, 0);

        public string ServerHashCode
        {
            get { return _serverHashCode; }
        }

        public static CcuUpgradeHandler Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;

                lock (_syncRoot)
                    if (_singleton == null)
                        _singleton = new CcuUpgradeHandler();

                return _singleton;
            }
        }

        private CcuUpgradeHandler()
        {
            _serverHashCode = GetRegistryServerHashCode();

            Log.ShowThreadId = true;
        }

        private readonly AutoResetEvent _exiting = new AutoResetEvent(false);

        private readonly AutoResetEvent _finished = new AutoResetEvent(false);

        private readonly AutoResetEvent _bindCompleted = new AutoResetEvent(false);

        private RemotingServer _remotingServer;

        private string _applicationPath;
        private string _dataPath;
        private string _fileToExecute;
        private uint _killProcessID;
        

        public delegate void DOnUpgradeFinish(
            string dataPaht, 
            string aplicationPath, 
            string fileToExecute, 
            bool success, 
            Exception ex);

        private event DOnUpgradeFinish OnUpgradeFinish;

        public void BeginUpgrade(
            string dataPath, 
            string applicationPath,
            string fileToExecute,
            string[] mainArguments,
            DOnUpgradeFinish onUpgradeFinish)
        {
            BeginUpgrade(
                applicationPath, 
                dataPath,
                fileToExecute,
                "0", 
                mainArguments,
                onUpgradeFinish);
        }

        public void BeginUpgrade(
            string dataPath, 
            string applicationPath, 
            string fileToExecute, 
            string killProcessID,
            string[] mainArguments,
            DOnUpgradeFinish onUpgradeFinish)
        {
            if (onUpgradeFinish != null)
                OnUpgradeFinish += onUpgradeFinish;

            _applicationPath = applicationPath;
            _dataPath = dataPath;
            _fileToExecute = fileToExecute;

            try
            {
                _killProcessID = uint.Parse(killProcessID);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Log.Error("An exception occured: " + ex.Message + ". Program will end");

                return;
            }

            SafeThread.StartThread(() => DoUpgrade(mainArguments));
        }

        private void DoUpgrade(IEnumerable<string> mainArguments)
        {
            if (!Path.IsPathRooted(_fileToExecute))
                _fileToExecute = _applicationPath + @"\" + _fileToExecute;

            var starterApp = CcuApplication.Unknown;

            try
            {
                ExchangeApplicationOrder(
                    CcuApplication.CCUUpgrader,
                    mainArguments, 
                    out starterApp);

                if (starterApp == CcuApplication.CCU)
                    Thread.Sleep(5000);

                //Device upgrader old directory
                DeleteDirectory(@"\Nandflash\DeviceUpgrader");

                var stoppedCCU = true;

                if (starterApp == CcuApplication.CCU)
                    stoppedCCU = EnsureStoppedApplicationProcess();

                TypeCache.LoadAssemblies(
                    GetType().Assembly,
                    typeof (LwRemotingMessage).Assembly);

                _remotingServer = new RemotingServer();
                _remotingServer.InitRemoting();

                Log.Info("Remoting started");

                //continue after server has successfuly binded events
                _bindCompleted.WaitOne(30000, false);

                //could not stop CCU.exe process, remoting is working so it is time to process this error
                if (!stoppedCCU)
                    throw new Exception("Could not stop CCU.EXE");

                //CCU directory
                DeleteDirectory(_dataPath);
                DeleteDirectory(_applicationPath);

                ResetLastEventId();

                Directory.Move(
                    @"\Nandflash\TempUpgrade", 
                    @"\NandFlash\CCU");

                Log.Info("CCU application folder replaced by newer version");

                

                CcuApplication tmp;
                ExchangeApplicationOrder(CcuApplication.CCU, null,out tmp);

                PossiblyNewKickstartMatters();
            }
            catch (Exception exc)
            {
                HandledExceptionAdapter.Examine(exc);
                Log.Error("An exception occured: " + exc.Message);

                switch (starterApp)
                {
                    case CcuApplication.CCU:

                        WatchdogControl.Reboot();
                        break;

                    case CcuApplication.CCUUpgrader:

                        CcuCoreRemotingProvider.Singleton.DoCcuUpgradeFinished(
                            _serverHashCode, 
                            false);

                        _finished.WaitOne(10000, false);

                        if (OnUpgradeFinish != null)
                            OnUpgradeFinish(
                                _dataPath, 
                                _applicationPath, 
                                _fileToExecute, 
                                false, 
                                exc);

                        break;
                }

                return;
            }

            CcuCoreRemotingProvider.Singleton.DoCcuUpgradeFinished(
                _serverHashCode, 
                true);

            _finished.WaitOne(
                10000, 
                false);

            _remotingServer.Stop();
            _remotingServer = null;

            Log.Info("Starting executing file: " + _fileToExecute);

            Process.Start(
                _fileToExecute, 
                string.Empty);

            Log.Info("Successfully upgraded");

            if (OnUpgradeFinish != null)
                OnUpgradeFinish(
                    _dataPath,
                    _applicationPath, 
                    _fileToExecute, 
                    true, 
                    null);
        }

        private static void PossiblyNewKickstartMatters()
        {
            try
            {
                //DebugHelper.WaitForDebuggerToBreak();

                string carriedCKtoStart = null;
                Version carriedCKversion = null;
                var traceProcessInfos = new LinkedList<ProcessInfo>();

                // also covers condition for PathForCarriedKickstartDir
                if (File.Exists(PathForCarriedKickstartExe))
                {
                    carriedCKversion = FileVersionInfo.GetVersionInfo(PathForCarriedKickstartExe);

                    Version otherCeKickstartRunningVersion;

                    var otherCKpresence = CeKickstartControl.CheckOtherKickstartRunning(
                        -1,
                        carriedCKversion,
                        out otherCeKickstartRunningVersion,
                        false,
                        Log,
                        traceProcessInfos
                        );

                    if (otherCKpresence == CeKickstartControl.OtherCeKickstartPresence.NoneFound ||
                        otherCKpresence == CeKickstartControl.OtherCeKickstartPresence.FoundLowerVersion)
                    {
                        // other CK should be killed by now

                        DeleteDirectory(CeKickstartControl.PathForNandFlashKickstartDirectory);

                        Directory.Move(PathForCarriedKickstartDir, CeKickstartControl.PathForNandFlashKickstartDirectory);

                        carriedCKtoStart = CeKickstartControl.PathForNandFlashKickstartDirectory + CeKickstartControl.ExecutableName;
                    }
                }

                
                if (carriedCKtoStart == null)
                {
                    Version ceKickstart;
                    string possiblyNewerKickstartPath =
                        CeKickstartControl.FindNewestKickstartVersion(
                            // use backslash here, as it doesn't collide with path combination later
                            CeKickstartControl.PathForNandFlashKickstartDirectory,
                            out ceKickstart);

                    if (!string.IsNullOrEmpty(possiblyNewerKickstartPath)
                        && traceProcessInfos.All(
                            info =>
                                info.Path.IndexOf(possiblyNewerKickstartPath, StringComparison.OrdinalIgnoreCase) < 0 &&
                                possiblyNewerKickstartPath.IndexOf(info.Path, StringComparison.OrdinalIgnoreCase) < 0
                            ))
                        CeKickstartStart(possiblyNewerKickstartPath, ceKickstart);
                }
                else
                {
                    CeKickstartStart(carriedCKtoStart, carriedCKversion);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error during finding replacing CEKickstart : "+e.Message);
            }
            finally
            {
                // for all flows where this dir wasn't moved to PathForNandFlashKickstartDirectory
                DeleteDirectory(PathForCarriedKickstartDir);

            }
        }


        private static void CeKickstartStart([NotNull] string ckPath, [NotNull] Version ckVersion)
        {
            try
            {
                Log.Info("Found a kickstart at \"" + ckPath + "\" with version " + ckVersion +
                         " and trying to start it");
                var p = new Process {StartInfo = {FileName = ckPath, Arguments = "avoid-apploader"}};
                p.Start();
            }
            catch (Exception err)
            {
                Log.Warning("Unable to start kickstart at \"" + ckPath + "\" due error : " + err.Message);
            }
        }

        private const string PathApploader = @"\NandFlash\contal.apploader.xml";
        private const string PathCcu = "/NandFlash/CCU/ccu.exe";
        private const string PathCcuUpgrader = "/NandFlash/CCUUpgrader/CCUUpgrader.exe";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desiredCcuApplicationAfterExchange"></param>
        /// <param name="mainArguments"></param>
        /// <param name="ccuApplicationBeforeExchange">application that started CCUUpgrader</param>
        private static void ExchangeApplicationOrder(
            CcuApplication desiredCcuApplicationAfterExchange,
            IEnumerable<string> mainArguments, 
            out CcuApplication ccuApplicationBeforeExchange)
        {
            ccuApplicationBeforeExchange = CcuApplication.Unknown;
            
            var anyCcuFound = false;
            var anyCcuUpgraderFound = false;
            

            string ccuUpgraderArguments = null;

            if (mainArguments != null)
            {
                var mainArgsStringBuilder = StringBuilderPool.Implicit2k.Get();

                try
                {

                    foreach (var arg in mainArguments)
                    {
                        if (mainArgsStringBuilder.Length > 0)
                            mainArgsStringBuilder.Append(StringConstants.SPACE);

                        mainArgsStringBuilder.Append(StringConstants.QUOTE);
                        mainArgsStringBuilder.Append(arg);
                        mainArgsStringBuilder.Append(StringConstants.QUOTE);
                    }

                    ccuUpgraderArguments = mainArgsStringBuilder.ToString();
                }
                finally
                {
                    try {StringBuilderPool.Implicit2k.Return(mainArgsStringBuilder);}catch{}
                }

            }

            var xmlDoc = new XmlDocument();

            var isXmlEmpty = true;
            try
            {
                xmlDoc.Load(PathApploader);
                isXmlEmpty = false;
            }
            catch(Exception xmlException)
            {
                Log.Info("Unable to load "+PathApploader+" because of error :\r\n\t"+xmlException.Message);
            }

            if (!isXmlEmpty)
            {
                foreach (XmlNode node in xmlDoc.GetElementsByTagName(XmlElementProcess))
                {
                    var lastCcuApplication = CcuApplication.Unknown;
                    var lastRecordIsDuplicate = false;

                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        switch (childNode.Name)
                        {
                            case XmlElementFilepath:
                                if (childNode.InnerText == null)
                                    continue;

                                string childNodeInnerText = childNode.InnerText.ToLower();

                                if (childNodeInnerText.Contains("ccu.exe"))
                                {
                                    lastCcuApplication = CcuApplication.CCU;

                                    if (anyCcuFound)
                                        lastRecordIsDuplicate = true;
                                    else
                                    {
                                        anyCcuFound = true;

                                        if (ccuApplicationBeforeExchange == CcuApplication.Unknown)
                                            ccuApplicationBeforeExchange = CcuApplication.CCU;
                                    }
                                }
                                else if (childNodeInnerText.Contains("ccuupgrader.exe"))
                                {
                                    lastCcuApplication = CcuApplication.CCUUpgrader;

                                    if (anyCcuUpgraderFound)
                                        lastRecordIsDuplicate = true;
                                    else
                                    {

                                        anyCcuUpgraderFound = true;

                                        if (ccuApplicationBeforeExchange == CcuApplication.Unknown)
                                            ccuApplicationBeforeExchange = CcuApplication.CCUUpgrader;
                                    }
                                }
                                else
                                {
                                    lastCcuApplication = CcuApplication.Unknown;
                                    continue;
                                }



                                if (lastRecordIsDuplicate)
                                {
                                    // invalidates the startup path
                                    childNode.InnerText = String.Empty;
                                    lastCcuApplication= CcuApplication.Unknown;
                                    
                                    continue;
                                }


                                if (desiredCcuApplicationAfterExchange == CcuApplication.CCUUpgrader &&
                                    lastCcuApplication == CcuApplication.CCU)
                                {
                                    childNode.InnerText = PathCcuUpgrader;

                                    continue;
                                }

                                if (desiredCcuApplicationAfterExchange == CcuApplication.CCU &&
                                    lastCcuApplication == CcuApplication.CCUUpgrader)
                                {
                                    childNode.InnerText = PathCcu;

// ReSharper disable once RedundantJumpStatement
                                    continue;
                                }


                                break;

                            case XmlElementArguments:

                                if (lastCcuApplication != CcuApplication.Unknown)
                                    switch (desiredCcuApplicationAfterExchange)
                                    {
                                        case CcuApplication.CCU:

                                            childNode.InnerText = String.Empty;
                                            break;

                                        case CcuApplication.CCUUpgrader:

                                            childNode.InnerText = ccuUpgraderArguments;
                                            break;

                                        case CcuApplication.Unknown:
                                            break;
                                    }
                                else
                                {
                                    if (lastRecordIsDuplicate)
                                        childNode.InnerText = String.Empty;
                                }
                                break;

                        }

                    }
                }
            }

            //no CCU and no CCUUpgrader record was found in apploader - insert CCU record there
            if (!anyCcuFound && !anyCcuUpgraderFound)
            {
                try
                {
                    CreateNewApploaderElement(
                        xmlDoc, 
                        desiredCcuApplicationAfterExchange,
                        ccuUpgraderArguments);

                    
                }
                catch (Exception)
                {
                    return;
                }
            }

            try
            {
                xmlDoc.Save(PathApploader);
            }
            catch (Exception xmlException)
            {
                Log.Info("Unable to save " + PathApploader + " because of error :\r\n\t" + xmlException.Message);
            }
        }

        private const string XmlElementProcesses = "processes";
        private const string XmlElementFilepath = "filepath";
        private const string XmlElementUseShellExec = "use-shell-exec";
        private const string XmlElementArguments = "arguments";
        private const string XmlElementProcess = "process";

        private const string PathForCarriedKickstartDir = @"NandFlash\Ccu\cekickstart\";
        private const string PathForCarriedKickstartExe = PathForCarriedKickstartDir + CeKickstartControl.ExecutableName;
        

        private static void CreateNewApploaderElement(XmlDocument xmlDoc,CcuApplication ccuApplication,string ccuUpgraderArguments)
        {
            var elementsByTagName = xmlDoc.GetElementsByTagName(XmlElementProcesses);

            XmlElement processesElement;

            if (elementsByTagName.Count <= 0 ||
                elementsByTagName[0] == null)
            {
                processesElement = xmlDoc.CreateElement(XmlElementProcesses);
                xmlDoc.AppendChild(processesElement);
            }
            else
            {
                processesElement = elementsByTagName[0] as XmlElement;
            }

            if (processesElement == null)
            {
                DebugHelper.TryBreakAndThrow(
                    new SystemException("Xml Processes element not found neither created"),
                    xmlDoc);
                return;
            }

            var processElement = xmlDoc.CreateElement(XmlElementProcess);

            var filePathElement = xmlDoc.CreateElement(XmlElementFilepath);
            var argumentsElement = xmlDoc.CreateElement(XmlElementArguments);
            
            var shellElement = xmlDoc.CreateElement(XmlElementUseShellExec);

            switch (ccuApplication)
            {
                case CcuApplication.CCU:
                    filePathElement.InnerText = PathCcu;
                    argumentsElement.InnerText = String.Empty;
                    break;
                default:
                    filePathElement.InnerText = PathCcuUpgrader;
                    argumentsElement.InnerText = ccuUpgraderArguments;
                    break;
            }

            shellElement.InnerText = "False";

            processElement.AppendChild(filePathElement);
            processElement.AppendChild(argumentsElement);
            processElement.AppendChild(shellElement);

            processesElement.AppendChild(processElement);
        }

        private bool EnsureStoppedApplicationProcess()
        {
            try
            {
                if (_killProcessID == 0)
                {
                    var fileName = 
                        Path.GetFileName(_fileToExecute).ToLower();

                    _killProcessID =
                        ProcessInfo
                            .GetAllProcessInfos()
                            .Where(processInfo => processInfo.Name.ToLower() == fileName)
                            .Select(processInfo => processInfo.PID)
                            .DefaultIfEmpty((uint)0)
                            .First();
                }
                if (_killProcessID != 0)
                {
                    Process process;

                    try
                    {
                        process = Process.GetProcessById((int)_killProcessID);
                    }
                    catch (Exception ex)
                    {
                        HandledExceptionAdapter.Examine(ex);
                        process = null;
                    }
                    
                    if (process == null)
                        return true;

                    process.Kill();
                    if (process.WaitForExit(5000))
                    {
                        Log.Info("CCU process stopped");
                        return true;
                    }
                    Log.Error("CCU process stop timed out");
                }
                else
                {
                    Log.Error("Process id could not be found");
                    return true;
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return false;
        }

        private static bool RebootTick(TimerCarrier timerCarrier)
        {
            WatchdogControl.Reboot();
            return true;
        }

        private static void DeleteDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    Log.Info("Attempt " + (i + 1) + "/5 to remove directory: " + directory);
                    Directory.Delete(directory, true);

                    break;
                }
                catch
                {
                    Thread.Sleep(1000);
                }

                //in catch section, new file will be created in specific directory to check directory accessibility
                try
                {
                    if (i == 4)
                        Directory.Delete(directory, true);
                }
                catch
                {
                    const string testFile = "DirTest.tst";
                    try
                    {
                        using (new FileStream(
                            Path.Combine(
                                directory,
                                testFile),
                            FileMode.Create,
                            FileAccess.Write))
                        {
                        }
                            
                        Log.Info(string.Format("Directory '{0}' can not be removed, but the file '{1}' can be created there", directory, testFile));
                    }
                    catch
                    {
                        Log.Warning(string.Format("Directory '{0}' can not be removed and the file '{1}' can not be created there", directory, testFile));
                    }

                    throw;
                }
            }
        }

        private static void ResetLastEventId()
        {
            RTCMemory.Write(
                FIRST_BLOCK_IN_RTC_MEMORY_FOR_SAVING_EVENT_ID_TRESHOLD,
                new uint[]
                {
                    0,
                    0
                });
        }

        public void UpgradeFinishConfirmed()
        {
            Log.Info("Server confirmed upgrade finished");
            _finished.Set();
        }

        private void Exit()
        {
            if (_remotingServer != null)
                try
                {
                    _remotingServer.Stop();
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                    Log.Error("Exception when closing LwRemoting server. Exception:" + ex.Message);
                }

            _exiting.Set();
        }

        public bool StopUpgrade()
        {
            SafeThread.StartThread(Exit);
            return true;
        }

        internal void WaitForExit()
        {
            _exiting.WaitOne();
        }

        internal void BindCompleted()
        {
            _bindCompleted.Set();
        }

        public string GetRegistryServerHashCode()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(@"HKLM\Software\Contal\CCU");
                if (registryKey != null)
                {
                    return Convert.ToString(registryKey.GetValue("CcuServerHashCode"));
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public void RebootCCU(string ccuFilePath, string killProcessID)
        {

            try
            {
                _killProcessID = uint.Parse(killProcessID);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Log.Error("An exception occured: " + ex.Message + ". Program will end");
                return;
            }

            if (_killProcessID != 0)
            {
                if (EnsureStoppedApplicationProcess())
                {
                    Process.Start(ccuFilePath, string.Empty);
                    Log.Info("Successfully rebooted CCU");
                }
                else
                {
                    Log.Error("Failed to stop CCU process. The device will be rebooted");
                    TimerManager.Static.StartTimeout(1000, RebootTick);
                }
            }
            else
            {
                Log.Error("Process id could not be 0");
            }
        }
    }
}
