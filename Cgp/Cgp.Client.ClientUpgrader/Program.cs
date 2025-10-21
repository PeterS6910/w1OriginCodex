using Contal.IwQuick;

namespace Contal.Cgp.Client.ClientUpgrader
{
    class Program
    {

        private readonly Log _log = new Log("Contal Nova Client upgrader", true, true, false);

        private void Run(string[] args)
        {
            _log.PrependDateConsole = false;

            if (args.Length == 4)
            {
                int killProcessId;
                if (!int.TryParse(args[3], out killProcessId))
                {
                    _log.Error("Invalid process id parameter set");
                    return;
                }

                var cup = new Upgrader(_log);
                cup.StartUpgrade(args[0], args[1], args[2], killProcessId);
            }
            else
            {
                _log.Error("Client upgrader has invalid arguments count");
            } 
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run(args);
        }
    }
}
