using System;
using System.Windows.Forms;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool errorLoggingSet = false;

            try
            {
                string sysTemp = Environment.GetEnvironmentVariable("TEMP");

                if (Validator.IsNotNullString(sysTemp))
                {
                    IwQuick.Sys.HandledExceptionAdapter.OutputFilePath = sysTemp + @"\ContalNova.Client.error.log";
                    errorLoggingSet = true;
                }
            }
            catch
            {
            }

            if (!errorLoggingSet)
                try
                {
                    string sysTemp = Environment.GetEnvironmentVariable("SystemRoot");

                    if (Validator.IsNotNullString(sysTemp))
                    {
                        IwQuick.Sys.HandledExceptionAdapter.OutputFilePath = sysTemp + @"\Temp\ContalNova.Client.error.log";
                        errorLoggingSet = true;
                    }
                }
                catch
                {
                }

            if (!errorLoggingSet)
                IwQuick.UI.Dialog.Warning("Logging capabilities will be disabled due failure");

            //Contal.IwQuick.Sys.HandledExceptionAdapter.Examine(new Exception("TEST"));

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CgpClient.Singleton.Run();
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                if (e.Exception is System.Runtime.InteropServices.SEHException)
                {
                    return;
                }
                ExceptionDialog excDialog = new ExceptionDialog();
                result = excDialog.ShowDialogExc(e.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort. 
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                Exception exception = (Exception)e.ExceptionObject;
                if (exception is System.Runtime.InteropServices.SEHException)
                {
                    return;
                }
                ExceptionDialog excDialog = new ExceptionDialog();
                result = excDialog.ShowDialogExc(exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort. 
            if (result == DialogResult.Abort)
                Application.Exit();
        }
    }
}
