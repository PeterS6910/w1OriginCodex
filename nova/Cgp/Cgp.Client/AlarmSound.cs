using System;
using System.Threading;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Threads;
using WMPLib;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public class AlarmSound
    {
        private static volatile AlarmSound _singleton = null;
        private static object _syncRoot = new object();
        ActualAlarms _playSound = new ActualAlarms();
        bool _playBylogged = true;
        private volatile SafeThread _doPlaySound = null;

        public bool PlayByLogged()
        {
            return _playBylogged;
        }

        public static AlarmSound Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new AlarmSound();
                    }

                return _singleton;
            }
        }

        private bool PlaySoundByInvType(ActualAlarms isAlarm)
        {
            _playSound = isAlarm;
            switch (GeneralOptions.Singleton.AlarmSoundInvocationType)
            {
                case Contal.Cgp.Globals.AlarmTypeBuzzer.Alarm:
                    return _playSound._alarm;
                case Contal.Cgp.Globals.AlarmTypeBuzzer.AlarmNotAcknowledged:
                    return _playSound._alarmNotAck;
                case Contal.Cgp.Globals.AlarmTypeBuzzer.NotAcknowledged:
                    return _playSound._notAck;
            }
            return false;
        }

        public void PlayAlarmSound(ActualAlarms isAlarm)
        {
            if (PlaySoundByInvType(isAlarm) && GeneralOptions.Singleton.AlarmSoundNotification && _playBylogged)
            {
                DoStartPlaySound();
            }
            else
            {
                StopPlaySound();
            }
        }

        public void SettingChanged()
        {
            PlayAlarmSound(_playSound);
        }

        public void ChangedLogedUser(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                _playBylogged = true;
            }
            else
            {
                if (CgpClient.Singleton.MainServerProvider != null &&
                    CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS)))
                {
                    _playBylogged = true;
                }
                else
                {
                    _playBylogged = false;
                }
            }
            PlayAlarmSound(_playSound);
        }

        private void DoStartPlaySound()
        {
            try
            {
                lock (_lockStart)
                {
                    if (_doPlaySound == null)
                    {
                        StartPlaySound();
                    }
                    else if (_actualSoundFile != GetSoundFile())
                    {
                        StopPlaySound();
                        StartPlaySound();
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private object _lockStart = new object();
        private void StartPlaySound()
        {
            try
            {
                if (_doPlaySound == null)
                    lock (_lockStart)
                    {
                        if (_doPlaySound == null)
                        {
                            _doPlaySound = new SafeThread(WplayerPlaySound);
                            _doPlaySound.Start();
                        }
                    }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private object _lockStop = new object();
        private void StopPlaySound()
        {
            if (_doPlaySound != null)
            {
                lock (_lockStop)
                {
                    if (_doPlaySound != null)
                    try
                    {
                        WplayerStopPlayingSound();
                        _doPlaySound.Stop(0);
                    }
                    catch(Exception error) 
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                    finally
                    {
                        _doPlaySound = null;
                    }
                }
            }
        
        }

        WMPLib.WindowsMediaPlayer _wplayer;
        string _actualSoundFile = string.Empty;
        private void WplayerPlaySound()
        {
            try
            {
                _wplayer = new WMPLib.WindowsMediaPlayer();
                //_wplayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(_wplayer_PlayStateChange);
                string soundFile = GetSoundFile();
                if (_wplayer != null && System.IO.File.Exists(soundFile))
                {
                    _actualSoundFile = soundFile;
                    _wplayer.URL = soundFile;
                    while (true && _wplayer != null)
                    {
                        _wplayer.controls.play();
                        Thread.Sleep(GetAlarmSoundRepeatFrequency());
                    }
                }
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        //void _wplayer_PlayStateChange(int NewState)
        //{
        //    //Console.WriteLine(NewState.ToString());
        //}

        private void WplayerStopPlayingSound()
        {
            try
            {
                if (_wplayer != null)
                {
                    if (_wplayer.playState == WMPPlayState.wmppsPlaying)
                        _wplayer.controls.stop();
                    _wplayer.close();
                    _wplayer = null;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private string GetSoundFile()
        {
            string fileName = string.Empty;

            if (GeneralOptions.Singleton.AlarmSoundFile != string.Empty)
            {
                fileName = System.Windows.Forms.Application.StartupPath;
                fileName += @"\Sounds\";
                fileName += GeneralOptions.Singleton.AlarmSoundFile;
            }

            return fileName;
        }

        private int GetAlarmSoundRepeatFrequency()
        {
            return GeneralOptions.Singleton.AlarmSoundRepeatFrequency * 1000;
        }

        public void StopPlayingSoundServerDisconnected()
        {
            StopPlaySound();
        }
    }

    public class ActualAlarms
    {
        public bool _alarmNotAck = false;
        public bool _alarm = false;
        public bool _notAck = false;
    }
}
