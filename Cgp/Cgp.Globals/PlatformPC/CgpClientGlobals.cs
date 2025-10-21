using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Globals
{
    public class CgpClientGlobals
    {
        private const string RegPathRoot = @"HKCU\Software\Contal\Cgp\Client\";

        public static string GetClientPathHash()
        {
            return Crc32.ComputeChecksum(QuickApp.StartupPath).ToString("X8");
        }

        public static readonly string RegPathClientRoot = RegPathRoot + GetClientPathHash();

        public static readonly string RegPathRemotingSettings =  RegPathClientRoot + StringConstants.BACKSLASH + "Remoting";
                
        public static readonly string RegPathClientSettings = RegPathClientRoot + StringConstants.BACKSLASH + "Settings";

        public const string CGP_SERVER_HOST_IP = "HostIP";
        public const string CGP_SERVER_HOST_PORT = "HostPort";
        public const string CGP_CLIENT_FRIENDLY_NAME = "ClientFriendlyName";
        public const string CGP_AES_REMOTING_ENABLED = "AesRemotingEnabled";
        public const string CGP_AES_REMOTING_KEY = "AesRemotingKey";
        public const string CGP_DEFAULT_LANGUAGE = "DefaultLanguage";
        public const string CGP_ENABLED_SYSTEM_LANGUAGE = "EnabledSystemLanguage";
        public const string CGP_COM_PORT_NAME = "ComPortName";
        public const string CGP_AUTO_UPGRADE_CLIENT = "AutoUpgradeClient";
        public const string CGP_EVENTLOG_LIST_REPORTER = "EventlogListReporter";
        public const string CGP_ALARM_SOUND_NOTIFICATION = "AlarmSoundNotification";
        public const string CGP_ALARM_SOUND_INVOCATION_TYPE = "AlarmSoundInvocationType";
        public const string CGP_ALARM_SOUND_FILE = "AlarmSoundFile";
        public const string CGP_ALARM_SOUND_REPEAT_FREQUENCY = "AlarmSoundRepeatFrequency";

        public const string DEFAULT_LANGUAGE_SELECT = "English";

        public const string CNMP_INSTANCE_PREFIX = "/Cgp/Client/";
        public const string CNMP_TYPE = "/Cgp/Client";
    }
}
