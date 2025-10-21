using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using WMPLib;
using System.Threading;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    
    public class AlarmTypeSound
    {
        private static volatile AlarmTypeSound _singleton = null;
        private static object _syncRoot = new object();
        private Dictionary<AlarmType, AlarmSoundRegObj> _dicAlarmTypeSettings;
        private Dictionary<AlarmType, ActiveAlarm> _dicActiveAlarms = new Dictionary<AlarmType,ActiveAlarm>();
        private Dictionary<AlarmType, SoundPalyer> _dicSoundPlayers = new Dictionary<AlarmType,SoundPalyer>();

        public static AlarmTypeSound Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new AlarmTypeSound();
                            ReadRegAlarmSound regRead = new ReadRegAlarmSound();
                            _singleton.InitDicAlarmTypeSettings(regRead.LoadRegAlarmSoundFromRegisrty());
                        }
                    }
                return _singleton;
            }
        }

        public void ReinitDicAlarmTypeSettins()
        {
            ReadRegAlarmSound regRead = new ReadRegAlarmSound();
            InitDicAlarmTypeSettings(regRead.LoadRegAlarmSoundFromRegisrty());
            ProccessAlarmChanged();
        }

        private void InitDicAlarmTypeSettings(List<AlarmSoundRegObj> list)
        {
            if (list == null) return;
            _dicAlarmTypeSettings = new Dictionary<AlarmType, AlarmSoundRegObj>();
            foreach (AlarmSoundRegObj asr in list)
            {
                if (!_dicAlarmTypeSettings.ContainsKey((AlarmType)asr.AlarmType))
                {
                    _dicAlarmTypeSettings.Add((AlarmType)asr.AlarmType, asr);
                }
            }
        }

        private object _lockProcessing = new object();
        public void ProcessAllAlarmsFromServer(ICollection<ServerAlarmCore> serverAlarms)
        {
            lock (_lockProcessing)
            {
                if (serverAlarms == null) return;
            
                _dicActiveAlarms = new Dictionary<AlarmType, ActiveAlarm>();
                foreach (var serverAlarm in serverAlarms)
                {
                    var alarmType = serverAlarm.Alarm.AlarmKey.AlarmType;

                    if (_dicActiveAlarms.ContainsKey(alarmType))
                    {
                        _dicActiveAlarms[alarmType].UpdateByAnother(serverAlarm);
                    }
                    else
                    {
                        _dicActiveAlarms.Add(alarmType, new ActiveAlarm(serverAlarm));
                    }
                }
            }

            StopEndedAlarms();
            ProccessAlarmChanged();
        }

        private void ProccessAlarmChanged()
        {
            try
            {
                lock (_lockProcessing)
                {
                    if (_dicActiveAlarms == null) return;
                    foreach (KeyValuePair<AlarmType, ActiveAlarm> kvp in _dicActiveAlarms)
                    {
                        if (_dicSoundPlayers.ContainsKey(kvp.Key))
                        {
                            if (PlayAlarmByType(kvp.Key) && !_dicSoundPlayers[kvp.Key].HasGuidAlarmObject(kvp.Value.AlarmGuids))
                            {
                                _dicSoundPlayers[kvp.Key].DoPlaySound(kvp.Value.AlarmGuids);
                            }
                            else
                            {
                                _dicSoundPlayers[kvp.Key].SetGuidAlarmObject(kvp.Value.AlarmGuids);
                                _dicSoundPlayers[kvp.Key].DoStopPlaySound();
                            }
                        }
                        else
                        {
                            _dicSoundPlayers.Add(kvp.Key, new SoundPalyer(kvp.Key, kvp.Value.AlarmGuids));
                            if (PlayAlarmByType(kvp.Key))
                            {
                                _dicSoundPlayers[kvp.Key].DoPlaySound(kvp.Value.AlarmGuids);
                            }
                        }
                    }

                    foreach (KeyValuePair<AlarmType, SoundPalyer> kvp in _dicSoundPlayers)
                    {
                        if (!_dicActiveAlarms.ContainsKey(kvp.Key))
                        {
                            _dicSoundPlayers[kvp.Key].ClearGuidAlarmObject();
                        }
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void StopEndedAlarms()
        {
            lock (_lockProcessing)
            {
                if (_dicSoundPlayers == null) return;
                foreach (KeyValuePair<AlarmType, SoundPalyer> kvp in _dicSoundPlayers)
                {
                    if (!_dicActiveAlarms.ContainsKey(kvp.Key))
                    {
                        kvp.Value.DoStopPlaySound();
                    }
                }
            }
        }

        public string GetSoundFileByType(AlarmType alarmType)
        {
            AlarmSoundRegObj value;
            if (_dicAlarmTypeSettings != null && _dicAlarmTypeSettings.TryGetValue(alarmType, out value))
            {
                string fileName = System.Windows.Forms.Application.StartupPath;
                fileName += @"\Sounds\";
                fileName += value.SoundFileName;
                return fileName;
            }
            return string.Empty;
        }

        public bool StillPlaySoundByType(AlarmType alarmType)
        {
            AlarmSoundRegObj value;
            if (_dicAlarmTypeSettings != null && _dicAlarmTypeSettings.TryGetValue(alarmType, out value))
            {
                return value.TillExists;
            }
            return false;
        }

        public int PlaybackCountByType(AlarmType alarmType)
        {
            AlarmSoundRegObj value;
            if (_dicAlarmTypeSettings != null && _dicAlarmTypeSettings.TryGetValue(alarmType, out value))
            {
                return value.RepeatCount;
            }
            return 1;
        }

        public bool PlayAlarmByType(AlarmType alarmType)
        {
            if (!AlarmSound.Singleton.PlayByLogged()) return false;

            AlarmSoundRegObj value;
            if (_dicAlarmTypeSettings != null && _dicAlarmTypeSettings.TryGetValue(alarmType, out value))
            {
                if (string.IsNullOrEmpty(value.SoundFileName))
                    return false;

                ActiveAlarm activeAlarm;
                if (_dicActiveAlarms.TryGetValue(alarmType, out activeAlarm))
                {
                    switch ((AlarmTypeBuzzer)value.AlarmTriggerType)
                    {
                        case Contal.Cgp.Globals.AlarmTypeBuzzer.Alarm:
                            return activeAlarm.Alarm;
                        case Contal.Cgp.Globals.AlarmTypeBuzzer.AlarmNotAcknowledged:
                            return activeAlarm.AlarmNotAck;
                        case Contal.Cgp.Globals.AlarmTypeBuzzer.NotAcknowledged:
                            return activeAlarm.NotAck;
                    }
                }
            }
            return false;
        }
    }

    public class ActiveAlarm
    {
        bool _alarmNotAck = false;
        bool _alarm = false;
        bool _notAck = false;
        List<Guid> _alarmGuid = new List<Guid>();

        public ActiveAlarm(ServerAlarmCore serverAlarm)
        {
            var alarm = serverAlarm.Alarm;

            _notAck = !serverAlarm.IsAcknowledged;
            if (alarm.AlarmState == AlarmState.Alarm && !serverAlarm.IsBlocked)
            {
                _alarm = true;

                if (!serverAlarm.IsAcknowledged)
                    _alarmNotAck = true;
            }

            _alarmGuid.Add(alarm.AlarmKey.AlarmObject != null
                ? (Guid) alarm.AlarmKey.AlarmObject.Id
                : Guid.Empty);
        }

        public void UpdateByAnother(ServerAlarmCore serverAlarm)
        {
            var alarm = serverAlarm.Alarm;

            if (alarm.AlarmState == AlarmState.Alarm && !serverAlarm.IsBlocked)
            {
                _alarm = true;

                if (!serverAlarm.IsAcknowledged)
                    _alarmNotAck = true;
            }
            if (!serverAlarm.IsAcknowledged)
            {
                _notAck = true;
            }

            _alarmGuid.Add(alarm.AlarmKey.AlarmObject != null
                ? (Guid)alarm.AlarmKey.AlarmObject.Id
                : Guid.Empty);
        }

        public bool AlarmNotAck { get { return _alarmNotAck; } }
        public bool Alarm { get { return _alarm; } }
        public bool NotAck { get { return _notAck; } }
        public List<Guid> AlarmGuids { get { return _alarmGuid; } }
    }

    public class SoundPalyer
    {
        AlarmType _alarmType;
        WMPLib.WindowsMediaPlayer _wplayer;
        int _repeated = 0;
        private volatile SafeThread _doPlaySound = null;
        List<Guid> _guidsAlarmObject = new List<Guid>();

        public bool HasGuidAlarmObject(List<Guid> alarmObject)
        {
            if (_guidsAlarmObject == null) return false;

            foreach (Guid id in alarmObject)
            {
                if (_guidsAlarmObject.Contains(id))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetGuidAlarmObject(List<Guid> alarmObject)
        {
            _guidsAlarmObject = alarmObject;
        }

        public void ClearGuidAlarmObject()
        {
            _guidsAlarmObject = null;
        }

        public SoundPalyer(AlarmType alarmType, List<Guid> startAlarmGuids)
        {
            _alarmType = alarmType;
            _guidsAlarmObject = startAlarmGuids.ToList();
        }

        private object _lockStart = new object();
        public void DoPlaySound(List<Guid> alarmObject)
        {
            lock (_lockStart)
            {
                _guidsAlarmObject = alarmObject;
                if (_doPlaySound == null)
                {
                    _repeated = 0;
                    _doPlaySound = new SafeThread(WplayerPlaySound);
                    _doPlaySound.Start();
                }
                else
                {
                    _repeated = 0;
                    if (_wplayer == null)
                    {
                        DoStopPlaySound();
                        _doPlaySound = new SafeThread(WplayerPlaySound);
                        _doPlaySound.Start();
                    }
                }
            }
        }

        private object _lockStop = new object();
        public void DoStopPlaySound()
        {
            if (_doPlaySound != null)
                lock (_lockStop)
                {
                    if (_doPlaySound != null)
                        try
                        {
                            WplayerStopPlayingSound();
                            _doPlaySound.Stop(0);
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }
                        finally
                        {
                            _doPlaySound = null;
                        }
                }
        }

        public void WplayerPlaySound()
        {
            try
            {
                _wplayer = new WMPLib.WindowsMediaPlayer();
                _wplayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(_wplayer_PlayStateChange);
                string soundFile = AlarmTypeSound.Singleton.GetSoundFileByType(_alarmType);

                if (_wplayer != null && System.IO.File.Exists(soundFile))
                {
                    _repeated = 0;
                    _wplayer.URL = soundFile;
                    
                    while (AlarmTypeSound.Singleton.StillPlaySoundByType(_alarmType) ||
                           (_repeated < AlarmTypeSound.Singleton.PlaybackCountByType(_alarmType)))
                    {
                        if (!_isPlaying)
                        {
                            _isPlaying = true;
                            _wplayer.controls.play();
                            Thread.Sleep(1000);
                            _repeated++;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _wplayer = null;
            }
        }

        bool _isPlaying;
        void _wplayer_PlayStateChange(int NewState)
        {
            if (((WMPLib.WMPPlayState)NewState) == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                _isPlaying = false;
            }
        }

        public void WplayerStopPlayingSound()
        {
            try
            {
                if (_wplayer != null)
                {
                    if (_wplayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        _wplayer.controls.stop();
                    _wplayer.close();
                    
                }
            }
            catch(Exception error) 
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _wplayer = null;
            }
        }
    }
}
