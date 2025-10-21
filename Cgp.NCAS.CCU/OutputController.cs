using System;

using Contal.Cgp.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    internal class OutputController
    {
        private class OutputActivator
        {
            public string ActivatorKey
            {
                get;
                private set;
            }

            public OutputActivator(string activatorKey)
            {
                ActivatorKey = activatorKey;
            }

            /// <summary>
            /// Stop timeout for max on delay
            /// </summary>
            public void Dispose()
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    "void Outputs.Close()");
            }
        }

        private volatile SyncDictionary<string, OutputActivator>
            _outputsActivators = new SyncDictionary<string, OutputActivator>();

        private readonly OutputStateAndSettings _outputStateAndSettings;

        public OutputController(OutputStateAndSettings outputStateAndSettings)
        {
            _outputStateAndSettings = outputStateAndSettings;
        }

        /// <summary>
        /// Turns the specific output to OFF but special output activators remains
        /// </summary>
        public void ManualOff()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void OutputManager.ManualOff()");

            // Close all activators for the output
            DisposeActivators();

            ForcedSetOutputToOff();
        }

        /// <summary>
        /// Turns the specific output to OFF and also removes its activators
        /// </summary>
        public void TotalOff()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                "void OutputManager.TotalOff()");

            ClearActivators();

            ForcedSetOutputToOff();
        }

        private void ForcedSetOutputToOff()
        {
            try
            {
                if (_outputStateAndSettings.IdDcu == Guid.Empty)
                {
                    IOControl.SetOutputTotalOff(_outputStateAndSettings.OutputNumber);
                    _outputStateAndSettings.SetOutputLogicalState(State.Off);
                }
                else
                {
                    DCUs.Singleton.ForcedSetOutputToOff(
                        _outputStateAndSettings.IdDcu,
                        _outputStateAndSettings.OutputNumber);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void Unconfigure()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void OutputManager.Unconfigure(OutputStateAndSettings outputStateAndSettings)");

            ClearActivators();

            try
            {
                if (_outputStateAndSettings.IdDcu == Guid.Empty)
                {
                    IOControl.ConfigureOutput(
                        _outputStateAndSettings.OutputNumber,
                        null);
                }
                else
                    TotalOff();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Removes specific output and its activators
        /// </summary>
        private void ClearActivators()
        {
            lock (_outputsActivators)
            {
                DisposeActivators();
                _outputsActivators.Clear();
            }
        }

        /// <summary>
        /// Close all activator for output
        /// </summary>
        private void DisposeActivators()
        {
            lock (_outputsActivators)
                foreach (var activator in _outputsActivators.Values)
                    activator.Dispose();
        }

        public void On(string activatorKey)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "void OutputManager.On(string activatorKey): [{0}]",
                    Log.GetStringFromParameters(
                        activatorKey)));

            lock (_outputsActivators)
            {
                bool setOutputToOn;
                //Add information about who has activated this output
                AddOutputActivator(activatorKey, out setOutputToOn);

                if (!setOutputToOn || _outputStateAndSettings.IsOutputBlocked)
                {
                    return;
                }

                On();
            }
        }

        private void On()
        {
            //ccu output
            if (_outputStateAndSettings.IdDcu == Guid.Empty)
            {
                try
                {
                    IOControl.SetOutput(
                        _outputStateAndSettings.OutputNumber,
                        true);

                    _outputStateAndSettings.SetOutputLogicalState(State.On);
                }
                //if turning output on failed, specific activator must be removed
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
            else
                DCUs.Singleton.SetOutputToOn(
                    _outputStateAndSettings.IdDcu,
                    _outputStateAndSettings.OutputNumber);
        }

        /// <summary>
        /// Adds information about who has activated the specific output
        /// </summary>
        /// <param name="activatorKey"></param>
        /// <param name="setOuptuToOn"></param>
        private void AddOutputActivator(string activatorKey, out bool setOuptuToOn)
        {
            setOuptuToOn = false;

            if (string.IsNullOrEmpty(activatorKey))
            {
                //even this activator will not be added, information that is was added will prevent mechanism from enabling/disabling the output by incorrectly identified activator
                return;
            }

            lock (_outputsActivators)
            {
                setOuptuToOn = _outputsActivators.Count == 0;

                if (!_outputsActivators.ContainsKey(activatorKey))
                {
                    _outputsActivators.Add(
                        activatorKey,
                        new OutputActivator(activatorKey));
                }
            }
        }

        public bool HasOutputActivator(string activatorKey)
        {
            return 
                !string.IsNullOrEmpty(activatorKey)
                && _outputsActivators.ContainsKey(activatorKey);
        }

        public void Off(string activatorKey)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void OutputManager.Off(string activatorKey): [{0}]",
                    Log.GetStringFromParameters(activatorKey)));

            lock (_outputsActivators)
            {
                bool setOutputToOff;
                //Remove information about who has activated this output
                RemoveOutputActivator(activatorKey, out setOutputToOff);

                if (!setOutputToOff || _outputStateAndSettings.IsOutputBlocked)
                    return;

                //ccu output
                if (_outputStateAndSettings.IdDcu == Guid.Empty)
                {
                    try
                    {
                        IOControl.SetOutput(
                            _outputStateAndSettings.OutputNumber,
                            false);

                        _outputStateAndSettings.SetOutputLogicalState(State.Off);
                    }
                    catch (Exception error)
                    {
                        //if turning off output failed, its activator must be added back
                        HandledExceptionAdapter.Examine(error);
                    }
                }
                else
                    DCUs.Singleton.SetOutputToOff(
                        _outputStateAndSettings.IdDcu,
                        _outputStateAndSettings.OutputNumber);
            }
        }

        /// <summary>
        /// Removes activator from objects, which has activated the specific output
        /// </summary>
        /// <param name="activatorKey"></param>
        /// <param name="setOutputToOff"></param>
        private void RemoveOutputActivator(string activatorKey, out bool setOutputToOff)
        {
            setOutputToOff = false;

            if (string.IsNullOrEmpty(activatorKey))
            {
                //even this activator will not be added, information that is was added will prevent mechanism from enabling/disabling the output by incorrectly identified activator
                return;
            }

            lock (_outputsActivators)
            {
                _outputsActivators.Remove(
                    activatorKey,
                    (key, removed, value) =>
                    {
                        if (removed)
                            value.Dispose();
                    });

                setOutputToOff = _outputsActivators.Count == 0;
            }
        }

        public void ApplyOutputHwSetup(OutputStateAndSettings outputStateAndSettings)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "void OutputManager.ApplyOutputHwSetup(OutputStateAndSettings outputStateAndSettings): [{0}]", 
                    Log.GetStringFromParameters(outputStateAndSettings)));

            if (outputStateAndSettings == null
                || outputStateAndSettings.IdDcu != Guid.Empty)
            {
                return;
            }

            

            IOControl.ConfigureOutput(
                outputStateAndSettings.OutputNumber,
                outputStateAndSettings);

            IOControl.SetOutputInversion(
                outputStateAndSettings.OutputNumber, 
                outputStateAndSettings.Inverted);

            outputStateAndSettings.SetOutputLogicalState(
                outputStateAndSettings.IsOutputBlocked || _outputsActivators.Count == 0
                    ? State.Off
                    : State.On);

            if (outputStateAndSettings.SendRealStateChanges)
            {
                IOControl.ReportRealStateOutput(outputStateAndSettings.OutputNumber, true);
                outputStateAndSettings.SetOutputRealState(outputStateAndSettings.OutputRealState);
            }
            else
            {
                IOControl.ReportRealStateOutput(outputStateAndSettings.OutputNumber, false);
            }
        }


        public void ReRunOutputActivators()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void OutputManager.ReRunOutputActivators()");

            lock (_outputsActivators)
            {
                var activators = _outputsActivators.ValuesSnapshot;
                _outputsActivators.Clear();

                foreach (var activator in activators)
                {
                    activator.Dispose();

                    On(activator.ActivatorKey);
                }
            }
        }

        public void ReEvaluateOutputActivators()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void OutputManager.ReEvaluateOutputActivators()");

            lock (_outputsActivators)
            {
                if (!_outputStateAndSettings.IsOutputBlocked && _outputsActivators.Count > 0)
                {
                    ForcedSetOutputToOff();
                    On();
                }
            }
        }

        public void AddOrReplaceOutputActivator(
            string oldActivator, 
            string newActivator)
        {
            bool setOutputToOff;
            RemoveOutputActivator(oldActivator, out setOutputToOff);

            bool setOutputToOn;
            AddOutputActivator(newActivator, out setOutputToOn);
        }
    }
}