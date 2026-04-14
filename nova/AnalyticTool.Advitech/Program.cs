using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace AnalyticTool.Advitech
{
    class Program
    {
        static void Main(string[] args)
        {
            HandledExceptionAdapter.UseLogClass = new Log("Advitech Tool", false, true, false) {IntoEventLog = true};

            bool needConfigure = !ApplicationProperties.Singleton.LoadProperties();

            if (IsRunningAsConsole())
            {
#if !DEBUG
                var serviceControler = new ServiceController("Contal Nova Analytic Tool for Advitech");
#endif
                if (needConfigure
                    || args.Any(param => param == "/config"))
                {
#if !DEBUG
                    if (serviceControler.Status == ServiceControllerStatus.Running)
                    {
                        if (!Dialog.Question("Do you want to stop the service?"))
                            return;

                        serviceControler.Stop();
                    }
#endif
                    Form dialog = new ServerLoginSettingsForm();

                    while (dialog != null)
                    {
                        dialog.ShowDialog();
                        dialog = ((IDialog) dialog).ResultDialog;
                    }
#if !DEBUG
                    if (Dialog.Question("Do you want to start the service?"))
                        serviceControler.Start();

                    return;
#endif
                }

                if (!ApplicationProperties.Singleton.LoadProperties())
                    return;

#if !DEBUG
                if (args.All(param => param != "/noservice"))
                {
                    if (serviceControler.Status == ServiceControllerStatus.Stopped)
                        serviceControler.Start();

                    return;
                }
#endif
                Console.WriteLine("Starting...");
                Core.Singleton.Start();
                Console.WriteLine("Running...");
                Console.WriteLine("\nPRESS ANY KEY TO EXIT");
                Console.ReadLine();

                return;
            }

            var servicesToRun = new ServiceBase[] { new AnalyticToolAdvitechService() };
            ServiceBase.Run(servicesToRun);

            Console.ReadLine();
        }

        private static int _isRunningAsConsole = -1;

        private static bool IsRunningAsConsole()
        {
            if (_isRunningAsConsole >= 0)
                return (_isRunningAsConsole != 0);

            bool isConsole = false;

            try
            {
                var tr = Console.In;
                if (StreamReader.Null != tr)
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
}
