using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Client
{
    public class ReadRegAlarmSound
    {
        public const string REGISTRY_CLIENT_SETTINGS_ALARM_TYPE_SOUND = @"HKCU\Software\Contal\Cgp\Client\Settings\AlarmTypeSound";
        public const string CGP_ALARM_TYPE_SOUND_ALARM_TYPE = "AlarmType";
        public const string CGP_ALARM_TYPE_SOUND_FILE_NAME = "SoundFileName";
        public const string CGP_ALARM_TYPE_SOUND_REPEAT_COUNT = "RepeatCount";
        public const string CGP_ALARM_TYPE_SOUND_TILL_EXISTS = "TillExists";
        public const string CGP_ALARM_TYPE_SOUND_BUZZER_TYPE = "AlarmTriggerType";

        public void SavegAlarmSoundDgvToRegistry(List<AlarmTypeSoundDgvObj> listAts)
        {
            foreach (AlarmTypeSoundDgvObj ats in listAts)
            {
                SaveRegAlarmSoundToRegistry(ats.AlarmType, ats.RegistrySoundFile, ats.SoundRepeatCount, ats.SoundRepeatTillDone, ats.RegAlarmBuzzerType);
            }
            AlarmTypeSound.Singleton.ReinitDicAlarmTypeSettins();
        }

        private void SaveRegAlarmSoundToRegistry(AlarmType alarmType, string soundFileName, int repeatCount, bool tillExists, int alarmTriggerType)
        {
            try
            {
                string registryKey = REGISTRY_CLIENT_SETTINGS_ALARM_TYPE_SOUND + @"\" + alarmType.ToString();

                RegistryKey rk = Contal.IwQuick.Sys.Microsoft.RegistryHelper.GetOrAddKey(registryKey, true);

                if (null == rk)
                    return;

                rk.SetValue(CGP_ALARM_TYPE_SOUND_ALARM_TYPE, (int)alarmType, RegistryValueKind.DWord);
                rk.SetValue(CGP_ALARM_TYPE_SOUND_FILE_NAME, soundFileName, RegistryValueKind.String);
                rk.SetValue(CGP_ALARM_TYPE_SOUND_REPEAT_COUNT, repeatCount, RegistryValueKind.DWord);
                rk.SetValue(CGP_ALARM_TYPE_SOUND_TILL_EXISTS, tillExists, RegistryValueKind.DWord);
                rk.SetValue(CGP_ALARM_TYPE_SOUND_BUZZER_TYPE, alarmTriggerType, RegistryValueKind.DWord);
            }
            catch { }
        }

        public List<AlarmSoundRegObj> LoadRegAlarmSoundFromRegisrty()
        {
            List<AlarmSoundRegObj> result = new List<AlarmSoundRegObj>();
            string registryKey = REGISTRY_CLIENT_SETTINGS_ALARM_TYPE_SOUND;
            RegistryKey rk = Contal.IwQuick.Sys.Microsoft.RegistryHelper.GetOrAddKey(registryKey, true);
            AlarmSoundRegObj tmpObj = null;

            foreach (string subKeyName in rk.GetSubKeyNames())
            {
                using (RegistryKey subkey = rk.OpenSubKey(subKeyName))
                {
                    tmpObj = GetFFF(subkey.Name);
                    if (tmpObj != null)
                    {
                        result.Add(tmpObj);
                    }
                }
            }

            if (result.Count != 0)
                return result;
            else
                return null;
        }

        private AlarmSoundRegObj GetFFF(string regKey)
        {
            byte alarmType;
            string soundFileName;
            int repeatCount;
            bool tillExists;
            byte alarmTriggerType;

            AlarmSoundRegObj atb = null;

            RegistryKey registryKey = null;
            if (Contal.IwQuick.Sys.Microsoft.RegistryHelper.TryParseKey(regKey, true, out registryKey))
            {
                try
                {
                    int type = (int)registryKey.GetValue(CGP_ALARM_TYPE_SOUND_ALARM_TYPE);
                    alarmType = (byte)type;
                }
                catch
                {
                    return null;
                }
                try
                {
                    soundFileName = (string)registryKey.GetValue(CGP_ALARM_TYPE_SOUND_FILE_NAME);
                }
                catch
                {
                    soundFileName = string.Empty;
                }
                try
                {
                    repeatCount = (int)registryKey.GetValue(CGP_ALARM_TYPE_SOUND_REPEAT_COUNT);
                }
                catch
                {
                    repeatCount = 1;
                }

                try
                {
                    int regNumber = (int)registryKey.GetValue(CGP_ALARM_TYPE_SOUND_TILL_EXISTS);
                    tillExists = (regNumber != 0);
                }
                catch
                {
                    tillExists = false;
                }
                try
                {
                    int buzzerType = (int)registryKey.GetValue(CGP_ALARM_TYPE_SOUND_BUZZER_TYPE);
                    alarmTriggerType = (byte)buzzerType;
                }
                catch
                {
                    alarmTriggerType = 1;
                }

                atb = new AlarmSoundRegObj(alarmType, soundFileName, repeatCount, tillExists, alarmTriggerType);
            }

            return atb;
        }
    }

    public class AlarmSoundRegObj
    {
        byte _alarmType;
        string _soundFileName;
        int _repeatCount;
        bool _tillExists;
        byte _alarmTriggerType;

        public AlarmSoundRegObj(byte alarmType, string soundFileName, int repeatCount, bool tillExists, byte alarmTriggerType)
        {
            _alarmType = alarmType;
            _soundFileName = soundFileName;
            _repeatCount = repeatCount;
            _tillExists = tillExists;
            _alarmTriggerType = alarmTriggerType;
        }

        public byte AlarmType { get { return _alarmType; } }
        public string SoundFileName { get { return _soundFileName; } }
        public int RepeatCount { get { return _repeatCount; } }
        public bool TillExists { get { return _tillExists; } }
        public byte AlarmTriggerType { get { return _alarmTriggerType; } }
    }
}
