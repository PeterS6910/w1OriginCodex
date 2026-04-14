//INSTALLTYPE<>1

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using Contal.IwQuick.Crypto;
using Contal.IwQuick.Sys.Microsoft;
using Contal.Cgp.Globals;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Security.Principal;
using System.DirectoryServices;
using Contal.IwQuick;


namespace Contal.Cgp.Server
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private static readonly string CgpServicePassword =
            "@H+#AzfJ!%10" + 
            QuickHashes.GetMD5String(DateTime.Today.ToString());

        private static DomainController _domainControler;

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private string CgpPath
        {
            get
            {
                DirectoryInfo directoryInfo;

                try
                {
                    var fileInfo = new FileInfo(Context.Parameters["assemblypath"]);
                    directoryInfo = fileInfo.Directory;
                }
                catch (Exception)
                {
                    return null;
                }

                return
                    directoryInfo != null
                        ? directoryInfo + @"\Contal Nova Server.exe"
                        : null;
            }
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            var selectAccountForm = new SelectServiceUserForm();

            if (selectAccountForm.ShowDialog() != DialogResult.OK)
                throw new InstallException(
                    "Service settings were cancelled or failed otherwise");

            switch (selectAccountForm.AccountType)
            {
                case AccountType.LocalSystem:

                    _serviceProcessInstallerCgp.Account =
                        ServiceAccount.LocalSystem;

                    break;

                case AccountType.NetworkService:

                    _serviceProcessInstallerCgp.Account =
                        ServiceAccount.NetworkService;

                    break;

                case AccountType.CustomAccount:

                    _serviceProcessInstallerCgp.Username =
                        (selectAccountForm.AccountDomain != string.Empty
                                ? selectAccountForm.AccountDomain
                                : StringConstants.DOT) +
                            StringConstants.BACKSLASH +
                            selectAccountForm.AccountName;

                    _serviceProcessInstallerCgp.Password =
                        selectAccountForm.AccountPassword;

                    break;

                default:

                    CreateServiceAccount();

                    string serviceUserName = _domainControler == null
                        ? StringConstants.DOT +
                          StringConstants.BACKSLASH +
                          CgpServerGlobals.CGP_SERVICE_USER
                        : _domainControler.Domain.Name + "\\" + CgpServerGlobals.CGP_SERVICE_USER;

                    _serviceProcessInstallerCgp.Username = serviceUserName;

                    _serviceProcessInstallerCgp.Password = CgpServicePassword;

                    break;
            }

            _serviceInstallerCgp.ServicesDependedOn = 
                CgpServerGlobals.SERVICE_DEPENDENCIES;

            _serviceInstallerCgp.StartType = 
                selectAccountForm.ServiceStartupType;
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);

            if (DoesServiceExist())
            {
                StopService();
                RemoveService();
            }

            var cgpPath = CgpPath;

            if (cgpPath != null)
                FirewallController.RemoveApplicationRule(cgpPath);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            var cgpPath = CgpPath;

            if (cgpPath ==  null)
                return;

            FirewallController.SetApplicationRule(cgpPath);

            StartNtpServer();

            ModifyServiceRecoveryArray();

            StartCgp(cgpPath);
        }

        //private void SetLogOnAsForService()
        //{
        //    string arguments = string.Format("config \"{0}\" obj=\"{1}\" password=\"{2}\" ",
        //        CgpServerGlobals.CGP_SERVICE_NAME,
        //        _domainControler == null
        //            ? CgpServerGlobals.CGP_SERVICE_USER
        //            : _domainControler.Domain.Name + "\\" + CgpServerGlobals.CGP_SERVICE_USER,
        //        CgpServicePassword);

        //    var proc = new Process
        //    {
        //        StartInfo =
        //        {
        //            FileName = "sc.exe",
        //            Arguments = arguments,
        //            UseShellExecute = false,
        //            RedirectStandardOutput = false,
        //            Verb = "runas",
        //            WindowStyle = ProcessWindowStyle.Hidden,
        //            CreateNoWindow = false
        //        }
        //    };

        //    proc.Start();
        //    proc.WaitForExit();
        //}

        private static void StartNtpServer()
        {
            try
            {
                var regKey =
                    RegistryHelper.GetOrAddKey(
                        @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\W32Time\TimeProviders\NtpServer");

                if (regKey == null)
                    return;

                var value = (int)regKey.GetValue("Enabled");

                if (value == 1)
                    return;

                regKey.SetValue("Enabled", 1);

                var controller = new ServiceController
                {
                    MachineName = ".",
                    ServiceName = "w32time"
                };

                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped);

                controller.Start();

                FirewallController.AddAllowUdpPortRule(123);
            }
            catch
            {
            }
        }

        private static byte[] CreateServiceRecoveryArray()
        {
            var result = new byte[44];

            result[12] = 3;
            result[16] = 14;

            return result;
        }

        private static void ModifyServiceRecoveryArray()
        {
            using (var oKey = Registry.LocalMachine.OpenSubKey(
                    String.Format(
                        @"SYSTEM\CurrentControlSet\Services\{0}",
                        CgpServerGlobals.CGP_SERVICE_NAME),
                    true))
            {
                if (oKey == null)
                    return;

                try
                {
                    var bArray =
                        (byte[])oKey.GetValue("FailureActions")
                        ?? CreateServiceRecoveryArray();

                    if (bArray.Length > 42)
                    {
                        bArray[20] = 1;
                        bArray[24] = 192;
                        bArray[25] = 212;
                        bArray[26] = 1;

                        bArray[28] = 1;
                        bArray[32] = 192;
                        bArray[33] = 212;
                        bArray[34] = 1;

                        bArray[36] = 1;
                        bArray[40] = 192;
                        bArray[41] = 212;
                        bArray[42] = 1;
                    }

                    oKey.SetValue(
                        "FailureActions",
                        bArray);
                }
                catch
                {
                }
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            if (!DoesServiceExist())
                Install();

            if (NovaServiceUserExists())
                RemoveNovaServiceUser();
        }

        private static bool NovaServiceUserExists()
        {
            try
            {
                var userEntry = new DirectoryEntry("WinNT://./NovaServiceUser,user");
                //this call wil throw exception if specific user does not exist
                var name = userEntry.Name;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to remove NovaServiceUser
        /// </summary>
        private static void RemoveNovaServiceUser()
        {
            try
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "net.exe",
                        Arguments = "users " + CgpServerGlobals.CGP_SERVICE_USER + " /delete",
                        UseShellExecute = false,
                        RedirectStandardOutput = false
                    }
                };

                proc.Start();
                proc.WaitForExit();
            }
            catch
            {
            }
        }

        private void Install()
        {
            var cgpPath = CgpPath;

            if (cgpPath == null)
                return;

            try
            {
                var serviceInstaller = 
                    new ServiceInstaller
                    {
                        Context = new InstallContext(
                            string.Empty,
                            new[]
                            {
                                string.Format(
                                    "/assemblypath={0}",
                                    cgpPath)
                            }),
                        DisplayName = _serviceInstallerCgp.DisplayName,
                        Description = string.Empty,
                        ServiceName = CgpServerGlobals.CGP_SERVICE_NAME,
                        StartType = _serviceInstallerCgp.StartType,
                        Parent = new ServiceProcessInstaller
                        {
                            Account = ServiceAccount.User
                        },
                        ServicesDependedOn = CgpServerGlobals.SERVICE_DEPENDENCIES
                    };

                serviceInstaller.Install(new ListDictionary());

                using (var oKey = Registry.LocalMachine.OpenSubKey(
                    string.Format(
                        @"SYSTEM\CurrentControlSet\Services\{0}",
                        CgpServerGlobals.CGP_SERVICE_NAME),
                    true))
                {
                    if (oKey == null)
                        return;

                    try
                    {
                        oKey.SetValue("ImagePath", oKey.GetValue("ImagePath"));
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private static void StartCgp(string program)
        {
            try
            {
                //string strExitCode;
                var proc = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo =
                    {
                        FileName = program,
                        Arguments = "/configure",
                        UseShellExecute = true
                    }
                };
                proc.Start();
            }
            catch
            {
            }
        }

        private static void RemoveService()
        {
            try
            {
                var proc = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo =
                    {
                        FileName = "sc",
                        Arguments = "delete " + CgpServerGlobals.CGP_SERVICE_NAME,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };

                proc.Start();

                while (!proc.StandardOutput.EndOfStream)
                {
                    MessageBox.Show(proc.StandardOutput.ReadLine());
                }
            }
            catch
            {
            }
        }

        private static void StopService()
        {
            try
            {
                var proc1 = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo =
                    {
                        FileName = "sc",
                        Arguments = "stop " + CgpServerGlobals.CGP_SERVICE_NAME,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };

                proc1.Start();

                while (!proc1.StandardOutput.EndOfStream)
                {
                    MessageBox.Show(proc1.StandardOutput.ReadLine());
                }
            }
            catch
            {
            }
        }

        private static DomainController IsNeededDomainAccount()
        {
            try
            {
                var domain = Domain.GetCurrentDomain();
                string thisPcName = String.Format("{0}.{1}", Environment.MachineName, domain.ToString()).ToLower();

                if (domain.DomainControllers != null
                    && domain.DomainControllers.Count > 0)
                {
                    return domain.DomainControllers.OfType<DomainController>()
                        .FirstOrDefault(domainControler => thisPcName == domainControler.Name.ToLower());
                }
            }
            catch
            {
            }

            return null;
        }

        private static void CreateServiceAccount()
        {
            try
            {
                if (!LocalUsers.UserExists(CgpServerGlobals.CGP_SERVICE_USER))
                    LocalUsers.CreateUser(
                        CgpServerGlobals.CGP_SERVICE_USER,
                        CgpServicePassword,
                        true,
                        true,
                        false);

                var administratorsName = 
                    new SecurityIdentifier(CgpServerGlobals.SID_ADMINISTRATORS)
                        .Translate(typeof(NTAccount))
                        .Value
                        .Split(StringConstants.BACKSLASH[0])[1];

                string arguments = "localgroup " + administratorsName + " " + CgpServerGlobals.CGP_SERVICE_USER + " /add";

                _domainControler = IsNeededDomainAccount();

                if (_domainControler != null
                    && !_domainControler.OSVersion.ToLower().Contains("essentials"))
                {
                    arguments += " /domain";
                }

                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "net.exe",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = false
                    }
                };

                proc.Start();
                proc.WaitForExit();
            }
            catch(Exception e)
            {
                throw new InstallException(
                    "Unable to create " + 
                        CgpServerGlobals.CGP_SERVICE_USER + 
                        " account or assign it administrator role", 
                    e);
            }
        }

        private static bool DoesServiceExist()
        {
            try
            {
                return
                    ServiceController
                        .GetServices()
                        .Any(serviceController => 
                            serviceController.ServiceName == 
                                CgpServerGlobals.CGP_SERVICE_NAME);
            }
            catch
            {
                return true;
            }
        }
    }
}
