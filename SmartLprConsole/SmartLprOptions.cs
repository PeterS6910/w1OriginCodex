using System;
using System.Configuration;

namespace SmartLprConsole
{
    internal sealed class SmartLprOptions
    {
        private SmartLprOptions(Uri baseAddress, string username, string password, bool ignoreSslErrors, TimeSpan timeout)
        {
            BaseAddress = baseAddress;
            Username = username;
            Password = password;
            IgnoreSslErrors = ignoreSslErrors;
            Timeout = timeout;
        }

        public Uri BaseAddress { get; }

        public string Username { get; }

        public string Password { get; }

        public bool IgnoreSslErrors { get; }

        public TimeSpan Timeout { get; }

        public static SmartLprOptions LoadFromConfig()
        {
            var baseUrl = ConfigurationManager.AppSettings["SmartLpr:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ConfigurationErrorsException("Nie je nastavený parameter SmartLpr:BaseUrl.");
            }

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseAddress))
            {
                throw new ConfigurationErrorsException("Parameter SmartLpr:BaseUrl nie je platná URL.");
            }

            var username = ConfigurationManager.AppSettings["SmartLpr:Username"];
            var password = ConfigurationManager.AppSettings["SmartLpr:Password"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ConfigurationErrorsException("Je potrebné zadať SmartLpr:Username a SmartLpr:Password.");
            }

            var ignoreSslValue = ConfigurationManager.AppSettings["SmartLpr:IgnoreSslErrors"];
            var timeoutValue = ConfigurationManager.AppSettings["SmartLpr:TimeoutSeconds"];

            bool ignoreSsl = false;
            if (!string.IsNullOrEmpty(ignoreSslValue))
            {
                bool.TryParse(ignoreSslValue, out ignoreSsl);
            }

            TimeSpan timeout = TimeSpan.FromSeconds(30);
            if (!string.IsNullOrEmpty(timeoutValue) && int.TryParse(timeoutValue, out var seconds) && seconds > 0)
            {
                timeout = TimeSpan.FromSeconds(seconds);
            }

            return new SmartLprOptions(baseAddress, username, password, ignoreSsl, timeout);
        }
    }
}
