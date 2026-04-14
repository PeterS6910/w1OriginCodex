using System;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Contal.IwQuick.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class Thread2UI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="lambdaToInvokeInUI"></param>
        /// <param name="async"></param>
        public static void Invoke(
            [NotNull] Control invoker, 
            [NotNull] Action lambdaToInvokeInUI, 
            bool async)
        {
            if (null == lambdaToInvokeInUI)
                throw new ArgumentNullException("lambdaToInvokeInUI");

            if (ReferenceEquals(invoker, null))
                throw new ArgumentNullException("invoker");

            try
            {

                if (invoker.InvokeRequired)
                {
                    if (async)
                        invoker.BeginInvoke(lambdaToInvokeInUI);
                    else
                        invoker.Invoke(lambdaToInvokeInUI);
                }
                else
                    lambdaToInvokeInUI();
            }
            catch(Exception e)
            {
                Sys.HandledExceptionAdapter.Examine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="lambdaToInvokeInUI"></param>
        public static void Invoke(
            [NotNull] Control invoker, 
            [NotNull] Action lambdaToInvokeInUI)
        {
            Invoke(invoker, lambdaToInvokeInUI, false);
        }

        
        

    }

    /// <summary>
    /// 
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="lambdaToInvokeInUI"></param>
        public static void InvokeInUI(
            [NotNull] this Form invoker, 
            [NotNull] Action lambdaToInvokeInUI)
        {
            Thread2UI.Invoke(invoker, lambdaToInvokeInUI,false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="lambdaToInvokeInUI"></param>
        public static void BeginInvokeInUI(
            [NotNull] this Form invoker,
            [NotNull] Action lambdaToInvokeInUI)
        {
            Thread2UI.Invoke(invoker, lambdaToInvokeInUI, true);
        }

    }
}
