using System.Diagnostics;
using System;
using System.Linq;
using System.Threading;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;


namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public static class DebugHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variablePreserves"></param>
        [Conditional("DEBUG")]
// ReSharper disable once InconsistentNaming
        public static void NOP(params object[] variablePreserves)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <typeparam name="T"></typeparam>
        [Conditional("DEBUG")]
        public static void BreakIfTypeName<T>(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return;

            Type t = typeof (T);

            try
            {
                if ((t.Name.Equals(typeName) || t.FullName.Equals(typeName)) && Debugger.IsAttached)
                    Debugger.Break();
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                
            }
        }

        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void WaitForDebuggerToBreak()
        {
            WaitForDebuggerToBreak(-1);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void WaitForDebuggerToBreak(int waitingTime)
        {
            try
            {

#if COMPACT_FRAMEWORK

                if (waitingTime <= 0)
                    waitingTime = 60;


                Process p = Process.GetCurrentProcess();

                try
                {
                    throw new Exception();
                }
                catch (Exception errorForStackTrace)
                {
                    Console.WriteLine("ERROR : Process PID=" + p.Id + " requested breaking at\r\n" +
                                      errorForStackTrace.StackTrace);
                }

                try
                {
                    const string conman = "ConmanClient2.exe";

                    var processes = ProcessInfo.GetAllProcessInfos();
                    if (!processes.Any(pi =>
                        pi.Name.Equals(conman, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var px = new Process {StartInfo = new ProcessStartInfo {FileName = conman}};
                        px.Start();
                    }
                }
                catch
                {

                }

                int j = 0;
                while (j++ < waitingTime && !Debugger.IsAttached)
                {
                    Console.WriteLine("ERROR : Attach within " + (waitingTime - j) + " seconds");

                    Thread.Sleep(1000);
                }

                if (Debugger.IsAttached)
                {

                    Console.WriteLine("ERROR : Debugger attached");
                    Console.WriteLine("ERROR : Breaking ...");

                    Debugger.Break();
                }
                else
                    Console.WriteLine("ERROR : Waiting for debugger attach failed");
#else
                Debugger.Break();
#endif
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// same as NOP but also for release
        /// </summary>
        /// <param name="variablePreserves"></param>
        public static void Keep(params object[] variablePreserves)
        {
            
        }


        /// <summary>
        /// IF BREAKS, look into debugger output or into callstack for deeper info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="variablePreserves"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void TryBreak(string message, params object[] variablePreserves)
        {
            TryBreak(message,null,variablePreserves);
        }

        /// <summary>
        /// IF BREAKS, look into debugger output or into callstack for deeper info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        /// <param name="variablePreserves"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void TryBreak(
            string message, 
            string stackTrace, 
// ReSharper disable once UnusedParameter.Local
            params object[] variablePreserves)
        {
           if (null == stackTrace)

#if COMPACT_FRAMEWORK
            try
            {
                throw new Exception("Fake exception to get stack trace");
            }
            catch (Exception e)
            {
                try
                {
                    stackTrace = e.StackTrace;

                    int posSecondLine = stackTrace.IndexOf(StringConstants.CR_LF, StringComparison.Ordinal);

                    if (posSecondLine >= 0)
                        stackTrace = stackTrace.Substring(posSecondLine + StringConstants.CR_LF.Length); 
                    stackTrace = "\t" + stackTrace.Replace(StringConstants.CR_LF, "\r\n\t");
                }
// ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    
                }
            }
#else
            try
            {
                stackTrace = Environment.StackTrace;

                int secondLine = stackTrace.IndexOf(StringConstants.CR_LF, StringComparison.InvariantCulture);
                if (secondLine >= 0)
                {
                    int thirdLine = stackTrace.IndexOf(StringConstants.CR_LF, secondLine + 1,
                        StringComparison.InvariantCulture);

                    if (thirdLine >= 0)
                        stackTrace = stackTrace.Substring(thirdLine);
                }

                stackTrace = "\t" + stackTrace.Replace(StringConstants.CR_LF, "\r\n\t");
            }
            catch
            {
                
            }
#endif

            try
            {
                stackTrace = "\tCustom message : " + message +
                                    "\r\n\tStack trace :\r\n" + stackTrace;

                if (Debugger.IsAttached)
                {
                    stackTrace = "\tWARNING : Intentional application jump to breakpoint :\r\n" + stackTrace;
                    Debug.WriteLine(stackTrace);
                    Console.WriteLine(stackTrace);

                    // look into debug console or call-stack for broader info
                    Debugger.Break();
                }
                else
                    Console.WriteLine(
                        "\tWARNING : The application would jump to breakpoint, however no debugger attached :\r\n" + stackTrace
                        );
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                
            }
        }

        /// <summary>
        /// IF BREAKS, look into debugger output or into callstack for deeper info
        /// </summary>
        /// <param name="error"></param>
        /// <param name="variablePreserves"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void TryBreak(Exception error, params object[] variablePreserves)
        {
            TryBreak(error.Message, error.StackTrace, variablePreserves);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="variablePreserves"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void TryBreakAndThrow(Exception error, params object[] variablePreserves)
        {
            TryBreak(error,variablePreserves);

            throw error;
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void TryBreakAndThrow([NotNull] this Exception exception)
        {
            TryBreak(exception);

            throw exception;
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void TryBreak([NotNull] this Exception exception)
        {
            TryBreak(exception,exception);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            if (null == message)
                message = string.Empty;

            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
