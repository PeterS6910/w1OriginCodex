using System.Diagnostics;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Returns a System.Diagnostics.Process pointing to
        /// a pre-existing process with the same name as the
        /// current one, if any; or null if the current process
        /// is unique.
        /// </summary>
        /// <returns></returns>
        public static Process PreviousProcess()
        {
            return null;
            //try
            //{

            //    //Process aCurrentProcess = Process.GetCurrentProcess();
            //    //aCurrentProcess.Id
                    
            //    /*
            //    Process[] arProcesses = Process.GetProcessById(aCurrentProcess.Id);
            //    foreach (Process aP in arProcesses)
            //    {
            //        if ((aP.Id != aCurrentProcess.Id) &&
            //            (Path.GetFileName(aP.MainModule.FileName) == Path.GetFileName(aCurrentProcess.MainModule.FileName)))
            //            return aP;
            //    }*/
            //    return null;
            //}
            //catch
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="arguments"></param>
        /// <param name="noWindow"></param>
        /// <returns></returns>
        public static bool ShellExecute(string program, string arguments, bool noWindow)
        {
            Validator.CheckNullString(program);

            try
            {
                var process = new Process {StartInfo = {FileName = program, UseShellExecute = true}};
                if (Validator.IsNotNullString(arguments))
                    process.StartInfo.Arguments = arguments;
                
                return process.Start();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDir"></param>
        /// <param name="noWindow"></param>
        /// <returns></returns>
        public static int Execute(string program, string arguments,string workingDir,bool noWindow)
        {
            Validator.CheckNullString(program);

            var process = new Process {StartInfo = {FileName = program, UseShellExecute = false}};
            if (Validator.IsNotNullString(arguments))
                process.StartInfo.Arguments = arguments;
            
            if (null != workingDir)
                process.StartInfo.WorkingDirectory = workingDir;

            
            if (process.Start())
            {
                process.WaitForExit();
                return process.ExitCode;
            }
            
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDir"></param>
        /// <param name="noWindow"></param>
        /// <returns></returns>
        public static bool TryExecute(string program, string arguments, string workingDir, bool noWindow)
        {
            try
            {
                int exitCode = Execute(program, arguments, workingDir, noWindow);
                return (exitCode == 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="arguments"></param>
        /// <param name="noWindow"></param>
        /// <returns></returns>
        public static bool TryExecute(string program, string arguments,bool noWindow)
            {
                return TryExecute(program,arguments,null,noWindow);
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="noWindow"></param>
        /// <returns></returns>
        public static bool ShellExecute(string program,bool noWindow)
        {
            return ShellExecute(program, null, noWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static bool ShellExecute(string program)
        {
            return ShellExecute(program, null,false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static bool ShellExecute(string program, string arguments)
        {
            return ShellExecute(program, arguments, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static bool TryExecute(string program)
        {
            return TryExecute(program, null, null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="noWindow"></param>
        /// <returns></returns>
        public static bool TryExecute(string program,bool noWindow)
        {
            return TryExecute(program, null, null, noWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static bool TryExecute(string program, string arguments)
        {
            return TryExecute(program, arguments, null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public static string StartupPath
        {
            get { return QuickPath.AssemblyStartupPath; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string AssemblyPath
        {

            get
            {

                //Assembly aBinary = Assembly.GetExecutingAssembly();
                //if (null == aBinary)
                //    return null;
                //else
                //    return aBinary.Location;

                return null;
            }


        }
    }
}
