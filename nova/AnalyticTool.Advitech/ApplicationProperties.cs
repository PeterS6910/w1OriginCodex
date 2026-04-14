using System;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Win32;

using Contal.IwQuick.Sys.Microsoft;

namespace AnalyticTool.Advitech
{
    public class ApplicationProperties
    {
        private string _aesKey = "56466456dfwefw545";
        private CultureInfo _dateTimeProvider = CultureInfo.InvariantCulture;
        private string _dateTimeFormat = "dd.MM.yyyy HH:mm:ss";

        public static string RegPathClientRoot = @"HKLM\Software\Contal\AdvitechToolClient\";
        private static ApplicationProperties _singleton;

        public static ApplicationProperties Singleton 
        {
            get { return _singleton ?? (_singleton = new ApplicationProperties()); }
        }

        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }

        public string SuperUserName { get; set; }
        public string SuperUserPassword { get; set; }
        public string SqlServerName { get; set; }
        public string DatabaseLogin { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseName { get; set; }
        public string TableName { get; set; }

        public Guid InputCardReader { get; set; }
        public Guid OutputCardReader { get; set; }
        public Guid Pump1Input { get; set; }
        public Guid Pump2Input { get; set; }
        public Guid Pump3Input { get; set; }
        public Guid Pump4Input { get; set; }

        public DateTime DefaultStartDate { get; set; }
        public int ScheduleType { get; set; }
        public int ScheduleValue { get; set; }

        public string GetConnectionStringForMaster()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = SqlServerName,
                InitialCatalog = "master",
                UserID = SuperUserName,
                Password = SuperUserPassword,
                IntegratedSecurity = false,
                ConnectTimeout = 60
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionString()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = SqlServerName,
                InitialCatalog = "master",
                UserID = DatabaseLogin,
                Password = DatabasePassword,
                IntegratedSecurity = false,
                ConnectTimeout = 60
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionStringToDatabase(string databaseName)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = SqlServerName,
                InitialCatalog = databaseName,
                UserID = DatabaseLogin,
                Password = DatabasePassword,
                IntegratedSecurity = false,
                ConnectTimeout = 60
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public SqlConnectionStringBuilder GetConnectionStringBuilderForDatabase()
        {
            return new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = SqlServerName,
                InitialCatalog = DatabaseName,
                UserID = DatabaseLogin,
                Password = DatabasePassword,
                IntegratedSecurity = false,
                ConnectTimeout = 60
            };
        }

        public void SaveProperties()
        {
            //save server settings
            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("ServerIp", ServerIp, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("ServerPort", ServerPort, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("ServerUserName", ServerUserName, RegistryValueKind.String);

            string serverPassword = Contal.IwQuick.Crypto.QuickCrypto.Encrypt(
                ServerPassword, _aesKey);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("ServerPassword", serverPassword, RegistryValueKind.String);

            //save database settings
            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("SqlServerName", SqlServerName, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("DatabaseLogin", DatabaseLogin, RegistryValueKind.String);

            string databasePassword = Contal.IwQuick.Crypto.QuickCrypto.Encrypt(
                DatabasePassword, _aesKey);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("DatabasePassword", databasePassword, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("DatabaseName", DatabaseName, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("TableName", TableName, RegistryValueKind.String);

            //save nova settings
            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("InputCardReader", InputCardReader, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("OutputCardReader", OutputCardReader, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("Pump1Input", Pump1Input, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("Pump2Input", Pump2Input, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("Pump3Input", Pump3Input, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("Pump4Input", Pump4Input, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("DefaultStartDate", DefaultStartDate.ToString(_dateTimeFormat), RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("ScheduleType", ScheduleType, RegistryValueKind.String);

            RegistryHelper.GetOrAddKey(
                    RegPathClientRoot).
                    SetValue("ScheduleValue", ScheduleValue, RegistryValueKind.String);
        }

        public bool LoadProperties()
        {
            RegistryKey registryKey;

            if (RegistryHelper.TryParseKey(RegPathClientRoot, true, out registryKey))
            {
                ServerIp = (string) registryKey.GetValue("ServerIp");

                int port;
                Int32.TryParse(registryKey.GetValue("ServerPort").ToString(), out port);
                ServerPort = port;

                ServerUserName = (string) registryKey.GetValue("ServerUserName");

                try
                {
                    ServerPassword =
                        Contal.IwQuick.Crypto.QuickCrypto.Decrypt((string) registryKey.GetValue("ServerPassword"),
                            _aesKey);
                }
                catch
                {
                    ServerPassword = string.Empty;
                }

                SqlServerName = (string) registryKey.GetValue("SqlServerName");
                DatabaseLogin = (string) registryKey.GetValue("DatabaseLogin");
                DatabaseName = (string) registryKey.GetValue("DatabaseName");

                try
                {
                    DatabasePassword =
                        Contal.IwQuick.Crypto.QuickCrypto.Decrypt((string) registryKey.GetValue("DatabasePassword"),
                            _aesKey);
                }
                catch
                {
                    DatabasePassword = string.Empty;
                }

                TableName = (string) registryKey.GetValue("TableName");

                InputCardReader = new Guid((string) registryKey.GetValue("InputCardReader"));
                OutputCardReader = new Guid((string) registryKey.GetValue("OutputCardReader"));
                Pump1Input = new Guid((string) registryKey.GetValue("Pump1Input"));
                Pump2Input = new Guid((string) registryKey.GetValue("Pump2Input"));
                Pump3Input = new Guid((string) registryKey.GetValue("Pump3Input"));
                Pump4Input = new Guid((string) registryKey.GetValue("Pump4Input"));

                var startDateTimeString = (string)registryKey.GetValue("DefaultStartDate");
                
                try
                {
                    DefaultStartDate = DateTime.ParseExact(startDateTimeString, _dateTimeFormat, _dateTimeProvider);
                }
                catch
                {
                    DefaultStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }

                int scheduleType;
                Int32.TryParse((string) registryKey.GetValue("ScheduleType"), out scheduleType);
                ScheduleType = scheduleType;

                int scheduleValue;
                Int32.TryParse((string)registryKey.GetValue("ScheduleValue"), out scheduleValue);
                ScheduleValue = scheduleValue;

                if (string.IsNullOrEmpty(ServerIp)
                    || ServerPort == 0
                    || string.IsNullOrEmpty(ServerUserName)
                    || string.IsNullOrEmpty(ServerPassword)
                    || string.IsNullOrEmpty(SqlServerName)
                    || string.IsNullOrEmpty(DatabaseName)
                    || string.IsNullOrEmpty(DatabaseLogin)
                    || string.IsNullOrEmpty(DatabasePassword)
                    || string.IsNullOrEmpty(TableName)
                    || ScheduleValue == 0
                    || InputCardReader == Guid.Empty
                    || OutputCardReader == Guid.Empty
                    || (Pump1Input == Guid.Empty
                        && Pump2Input == Guid.Empty
                        && Pump3Input == Guid.Empty
                        && Pump4Input == Guid.Empty))
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }
    }
}
