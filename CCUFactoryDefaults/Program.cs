using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace CCUFactoryDefaults
{
    class Program
    {
        private const string PREFIX = "CCUFactoryDefaults ";

        private void Run(string[] args)
        {
            try
            {
                if (args.Length >= 1)
                {
                    string cmd = args[0];
                    cmd = cmd.Trim().ToLower();

                    switch (cmd)
                    {
                        case "clear-config-pass":
                            ClearConfigurationPassword();
                            break;
                    }
                }
                else
                {
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(PREFIX+"Error : " + error.Message);
            }
        }

        void ClearConfigurationPassword()
        {
            RegistryKey rk = null;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"Software\Contal\CCU", true);
                if (rk == null)
                {
                    Console.WriteLine(PREFIX + " Error : unable to open key in writable mode");
                    return;
                }

                rk.DeleteValue("ConfigurePassword");
                Console.WriteLine(PREFIX + " Info : Password cleared successfuly");
            }
            catch(Exception e)
            {
                Console.WriteLine(PREFIX + " Error : unable to delete password value");
            }

            if (rk != null)
            {
                try { rk.Flush(); }
                catch { }

                try
                {
                    rk.Close();
                }
                catch
                {
                }
            }
            
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }
    }
}
