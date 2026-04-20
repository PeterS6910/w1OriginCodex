using System;
using System.Threading.Tasks;

namespace SmartLprConsole
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try
            {
                var options = SmartLprOptions.LoadFromConfig();
                using (var client = new SmartLprClient(options))
                {
                    await DisplayCameraStatusAsync(client).ConfigureAwait(false);
                    await DisplayRecognitionStatisticsAsync(client).ConfigureAwait(false);
                    await DisplayActiveAlarmsAsync(client).ConfigureAwait(false);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Chyba pri komunikácii so SmartLPR jednotkou: {0}", ex.Message);
                Console.ResetColor();
                return 1;
            }
        }

        private static async Task DisplayCameraStatusAsync(SmartLprClient client)
        {
            Console.WriteLine("Načítavam stav kamery...");
            var status = await client.GetCameraStatusAsync().ConfigureAwait(false);
            Console.WriteLine("  Globálny stav:   {0}", status.Global ? "OK" : "CHYBA");
            Console.WriteLine("  Stav osvetlenia: {0}", status.Lamp ? "zapnuté" : "vypnuté");
            Console.WriteLine("  Teplota:         {0}°C", status.Temperature);
            Console.WriteLine();
        }

        private static async Task DisplayRecognitionStatisticsAsync(SmartLprClient client)
        {
            Console.WriteLine("Načítavam štatistiky rozpoznávania SPZ...");
            var statistics = await client.GetRecognitionStatisticsAsync().ConfigureAwait(false);
            Console.WriteLine("  Počet rozpoznaní:              {0}", statistics.Recognitions);
            Console.WriteLine("  Rozpoznania s čitateľnou SPZ:  {0}", statistics.RecognitionsWithLicense);
            Console.WriteLine("  Rozpoznania s korektnou gramatikou: {0}", statistics.RecognitionsWithGrammarOk);
            Console.WriteLine("  Priemerná kvalita:             {0}", statistics.AverageQuality);
            Console.WriteLine("  Neznáme znaky:                 {0}", statistics.NumberOfUnknownChars);
            Console.WriteLine();
        }

        private static async Task DisplayActiveAlarmsAsync(SmartLprClient client)
        {
            Console.WriteLine("Načítavam alarmy kamery...");
            var alarms = await client.GetAlarmsAsync().ConfigureAwait(false);
            if (alarms.Alarms.Count == 0)
            {
                Console.WriteLine("  Žiadne aktívne alarmy.");
            }
            else
            {
                foreach (var alarm in alarms.Alarms)
                {
                    Console.WriteLine("  Alarm: {0}", alarm.Name);
                    Console.WriteLine("    Stav: {0}", alarm.State);
                    if (!string.IsNullOrEmpty(alarm.TriggeredTimestamp))
                    {
                        Console.WriteLine("    Spustené: {0}", alarm.TriggeredTimestamp);
                    }
                    if (!string.IsNullOrEmpty(alarm.LastCheckedTimestamp))
                    {
                        Console.WriteLine("    Naposledy kontrolované: {0}", alarm.LastCheckedTimestamp);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Hotovo. Stlačte ľubovoľnú klávesu pre ukončenie.");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey(true);
            }
        }
    }
}
