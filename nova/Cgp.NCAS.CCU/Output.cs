using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Contal.Drivers.LPC3250;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuickCF.Threads;

namespace Contal.Cgp.NCAS.CCU
{
    class Output
    {
        Contal.IwQuickCF.Threads.TimerCarrier tc;
        private bool _stopFrequency = false;

        private static Output _singleton = null;
        public static Output Singleton
        {
            get
            {
                if (null == _singleton)
                    _singleton = new Output();

                return _singleton;
            }
        }


        public void SetOutputToOn(DB.OutputDCU output)
        {
            switch (output.OutputType)
            {
                case (byte)OutputType.level :
                    LevelOn(output);
                    break;
                case (byte)OutputType.pulsed:
                    PulsedOn(output);
                    break;
                case (byte)OutputType.frequency:
                    FrequencyOn(output);
                    break;
            }
        }

        public void SetOutputToOff(DB.OutputDCU output)
        {
            switch (output.OutputType)
            {
                case (byte)OutputType.level:
                    LevelOff(output);
                    break;
                case (byte)OutputType.pulsed:
                    PulsedOff(output);
                    break;
                case (byte)OutputType.frequency:
                    FrequencyOff(output);
                    break;
            }
        }


        //output level
        private void LevelOn(DB.OutputDCU output)
        {
            if (tc != null) tc.StopTimer();
            OutputOn();
        }

        private void LevelOff(DB.OutputDCU output)
        {
            if (tc != null) tc.StopTimer();
            if (output.ControlType != (byte)EnumOutputControlTypes.forcedOn)
            {
                OutputOff();
            }
        }

        //output impulse
        private void PulsedOn(DB.OutputDCU output)
        {
            if (tc != null) tc.StopTimer();
            tc = TimerManager.Static.StartTimer((long)output.SettingsDelayToOn , true, new DOnTimerEvent(OnImpulseOn), output);
        }

        private void PulsedOff(DB.OutputDCU output)
        {
            if (output.ControlType != (byte)EnumOutputControlTypes.forcedOn)
            {
                if ((bool)output.SettingsForcedToOff)
                {
                    if (tc != null) tc.StopTimer();
                    OutputOff();
                }
            }
        }

        private bool OnImpulseOn(Contal.IwQuickCF.Threads.TimerCarrier timerCarrier)
        {
            if (tc != null) tc.StopTimer();
            DB.OutputDCU output = timerCarrier.Data as DB.OutputDCU;
            OutputOn();
            tc = TimerManager.Static.StartTimer((long)output.SettingsDelayToOff, true, new DOnTimerEvent(OnImpulseOff), output);
            return true;
        }


        private bool OnImpulseOff(Contal.IwQuickCF.Threads.TimerCarrier timerCarrier)
        {
            if (tc != null) tc.StopTimer();
            OutputOff();
            return true;
        }


        //Frequency output
        private void FrequencyOn(DB.OutputDCU output)
        {
            _stopFrequency = false;
            if (tc != null) tc.StopTimer();
            tc = TimerManager.Static.StartTimer((long)output.SettingsDelayToOn, true, new DOnTimerEvent(OnFrequencyOn), output);
        }

        private void FrequencyOff(DB.OutputDCU output)
        {
            if (output.ControlType != (byte)EnumOutputControlTypes.forcedOn)
            {
                if ((bool)output.SettingsForcedToOff)
                {
                    if (tc != null) tc.StopTimer();
                    OutputOff();
                }
                else
                {
                    _stopFrequency = true;
                }
            }
        }

        private bool OnFrequencyOn(Contal.IwQuickCF.Threads.TimerCarrier timerCarrier)
        {
            if (tc != null) tc.StopTimer();
            DB.OutputDCU output = timerCarrier.Data as DB.OutputDCU;
            OutputOn();
            tc = TimerManager.Static.StartTimer((long)output.SettingsDelayToOff, true, new DOnTimerEvent(OnFrequencyDelayToOn), output);
            return true;
        }

        private bool OnFrequencyDelayToOn(Contal.IwQuickCF.Threads.TimerCarrier timerCarrier)
        {
            if (tc != null) tc.StopTimer();
            DB.OutputDCU output = timerCarrier.Data as DB.OutputDCU;
            OutputOff();
            if (!_stopFrequency)
            {
                tc = TimerManager.Static.StartTimer((long)output.SettingsDelayToOn, true, new DOnTimerEvent(OnFrequencyOn), output);
            }
            return true;
        }

        private void OutputOn()
        {
            Contal.Drivers.LPC3250.IOControl.Set(IOControl.PortType.GPIO, 1, false);
            //CCU.CcuCore.MainLog.Info(": Output ON");
        }

        private void OutputOff()
        {
            Contal.Drivers.LPC3250.IOControl.Set(IOControl.PortType.GPIO, 1, true);
            //CCU.CcuCore.MainLog.Info(": Output OFF");
            Contal.Drivers.LPC3250.IOControl.Set(IOControl.PortType.GPIO, 1, true);
        }

    }
}
