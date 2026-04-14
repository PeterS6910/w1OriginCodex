using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Contal.IwQuick.Sys
{
    public class QuickApp
    {
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
        public static Process PreviousProcess()
        {
            try
            {
                
                Process aCurrentProcess = Process.GetCurrentProcess();
                //aCurrentProcess.Id
                    
                Process[] arProcesses = Process.GetProcessesByName(aCurrentProcess.ProcessName);
                foreach (Process aP in arProcesses)
                {
                    if ((aP.Id != aCurrentProcess.Id) &&
                        (Path.GetFileName(aP.MainModule.FileName) == Path.GetFileName(aCurrentProcess.MainModule.FileName)))
                        return aP;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool ShellExecute(string applicationPath, string arguments, bool noWindow)
        {
            Validator.CheckNullString(applicationPath);

            try
            {
                Process aProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = applicationPath, 
                        UseShellExecute = true, 
                        CreateNoWindow = noWindow
                    }
                };

                if (Validator.IsNotNullString(arguments))
                    aProcess.StartInfo.Arguments = arguments;
                if (noWindow)
                    aProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                return aProcess.Start();
            }
            catch
            {
                return false;
            }
        }

        public static int Execute(string applicationPath, string arguments,string workingDirectory,bool noWindow)
        {
            Validator.CheckNullString(applicationPath);

            Process aProcess = new Process
            {
                StartInfo = {FileName = applicationPath, UseShellExecute = false, CreateNoWindow = noWindow}
            };

            if (Validator.IsNotNullString(arguments))
                aProcess.StartInfo.Arguments = arguments;
            if (noWindow)
                aProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;


            if (null != workingDirectory)
                aProcess.StartInfo.WorkingDirectory = workingDirectory;

            
            if (aProcess.Start())
            {
                aProcess.WaitForExit();
                return aProcess.ExitCode;
            }
            
            return -1;
        }

        public static bool TryExecute(string applicationPath, string arguments, string i_strWorkingDir, bool noWindow)
        {
            try
            {
                int iExitCode = Execute(applicationPath, arguments, i_strWorkingDir, noWindow);
                return (iExitCode == 0);
            }
            catch
            {
                return false;
            }
        }

        public static bool TryExecute(string applicationPath, string arguments,bool noWindow)
            {
                return TryExecute(applicationPath,arguments,null,noWindow);
            }

        public static bool ShellExecute(string applicationPath,bool noWindow)
        {
            return ShellExecute(applicationPath, null, noWindow);
        }

        public static bool ShellExecute(string applicationPath)
        {
            return ShellExecute(applicationPath, null,false);
        }

        public static bool ShellExecute(string applicationPath, string arguments)
        {
            return ShellExecute(applicationPath, arguments, false);
        }

        public static bool TryExecute(string applicationPath)
        {
            return TryExecute(applicationPath, null, null, false);
        }

        public static bool TryExecute(string applicationPath,bool noWindow)
        {
            return TryExecute(applicationPath, null, null, noWindow);
        }

        public static bool TryExecute(string applicationPath, string arguments)
        {
            return TryExecute(applicationPath, arguments, null, false);
        }

        public static string StartupPath
        {
            get { return QuickPath.AssemblyStartupPath; }
        }

        public static string AssemblyPath
        {
            get
            {
                Assembly execAssembly = Assembly.GetExecutingAssembly();
                // according to R#, aBinary will always be not-null
                return execAssembly.Location;
               
            }
        }

        public static bool InteractiveProcesOrService
        {
            get
            {
                return System.Environment.UserInteractive;
            }
        }
    }
}
