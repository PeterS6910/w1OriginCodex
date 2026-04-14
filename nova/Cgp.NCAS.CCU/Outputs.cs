using System;

using System.Collections.Generic;
using Contal.Drivers.LPC3250;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal sealed class Outputs : AStateAndSettingsObjectCollection<
        Outputs,
        OutputStateAndSettings,
        DB.Output>
    {
        public class OutputStateGuid : IProcessingQueueRequest
        {
            private readonly Guid _outputGuid;
            private readonly State _outputState;

            public OutputStateGuid(Guid outputGuid, State outputState)
            {
                _outputGuid = outputGuid;
                _outputState = outputState;
            }

            public void Execute()
            {
                Action<Guid, State> events;

                if (Singleton._outputsChanged.TryGetValue(
                    _outputGuid,
                    out events) && events != null)
                {
                    events(
                        _outputGuid,
                        _outputState);
                }
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public class IdDcuAndOutputNumber : IEquatable<IdDcuAndOutputNumber>
        {
            public Guid IdDcu
            {
                get;
                private set;
            }

            public byte OutputNumber
            {
                get;
                private set;
            }

            public IdDcuAndOutputNumber(
                Guid idDcu,
                byte outputNumber)
            {
                IdDcu = idDcu;
                OutputNumber = outputNumber;
            }

            public override int GetHashCode()
            {
                return IdDcu.GetHashCode() ^ OutputNumber.GetHashCode();
            }

            public bool Equals(IdDcuAndOutputNumber other)
            {
                return
                    other != null
                    && other.IdDcu.Equals(IdDcu)
                    && other.OutputNumber.Equals(OutputNumber);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as IdDcuAndOutputNumber);
            }
        }


        private readonly SyncDictionary<IdDcuAndOutputNumber, OutputStateAndSettings> _outputsStateAndSettingsByDcuAndNumber =
            new SyncDictionary<IdDcuAndOutputNumber, OutputStateAndSettings>();

        private readonly Dictionary<Guid, Action<Guid, State>> _outputsChanged =
            new Dictionary<Guid, Action<Guid, State>>();

        private readonly ThreadPoolQueue<OutputStateGuid> _queueOutputChanged =
            new ThreadPoolQueue<OutputStateGuid>(ThreadPoolGetter.Get());

        public override ObjectType ObjectType
        {
            get { return ObjectType.Output; }
        }

        private Outputs()
            : base(null)
        {
            IOControl.OutputChanged += IOControl_OutputChanged;
        }

        void IOControl_OutputChanged(int index, bool isOn)
        {
            OutputStateAndSettings outputStateAndSettings;

            if (_outputsStateAndSettingsByDcuAndNumber.TryGetValue(
                new IdDcuAndOutputNumber(
                    Guid.Empty,
                    (byte)index),
                out outputStateAndSettings))
            {
                outputStateAndSettings.SetOutputRealState(
                    isOn
                        ? State.On
                        : State.Off);
            }
        }

        public void OnDcuOutputRealStateChanged(
            Guid idDcu,
            byte outputNumber,
            State outputState)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.OnDcuOutputRealStateChanged(Guid idDcu, byte outputNumber, State outputState): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        outputNumber,
                        outputState)));

            OutputStateAndSettings outputStateAndSettings;

            if (_outputsStateAndSettingsByDcuAndNumber.TryGetValue(
                new IdDcuAndOutputNumber(
                    idDcu,
                    outputNumber),
                out outputStateAndSettings))
            {
                outputStateAndSettings.SetOutputRealState(outputState);
            }
        }

        public void OnDcuOutputLogicalStateChanged(
            Guid idDcu,
            byte outputNumber,
            State outputState)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.OnDcuOutputLogicalStateChanged(Guid idDcu, byte outputNumber, State outputState): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        outputNumber,
                        outputState)));

            OutputStateAndSettings outputStateAndSettings;

            if (_outputsStateAndSettingsByDcuAndNumber.TryGetValue(
                new IdDcuAndOutputNumber(
                    idDcu,
                    outputNumber),
                out outputStateAndSettings))
            {
                outputStateAndSettings.SetOutputLogicalState(outputState);
            }
        }

        public Guid GetOutputIdByDcuAndNumber(
            Guid idDcu,
            byte outputNumber)
        {
            //faster than veryfying dictionaries
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "Guid Outputs.GetOutputIdByDcuAndNumber(Guid dcuId, byte outputNumber): [{0}]",
                    Log.GetStringFromParameters(
                        idDcu,
                        outputNumber)));

            OutputStateAndSettings outputStateAndSettings;

            return _outputsStateAndSettingsByDcuAndNumber.TryGetValue(
                    new IdDcuAndOutputNumber(
                        idDcu,
                        outputNumber),
                    out outputStateAndSettings)
                ? outputStateAndSettings.Id
                : Guid.Empty;
        }

        public static int GetOutputCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int Outputs.GetOutputCount()");
            try
            {
                var result = IOControl.GetOutputCount();
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int Outputs.GetOutputCount return {0}", Log.GetStringFromParameters(result)));
                return result;
            }
            //if (MainBoard.Type == MainBoardType.CCU40)
            //    return IOControl.GetOutputCount();
            //else if (MainBoard.Type == MainBoardType.CCU12)
            //    return 4;
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "int Outputs.GetOutputCount return 0");
                return 0;
            }
        }

        public void OnBoundInputBlocked(
            Guid idOutput,
            InputStateAndSettings inputStateAndSettings,
            bool requestedOutputState)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.OnBoundInputBlocked(InputStateAndSettings inputStateAndSettings, bool requestedOutputState): [{0}]",
                    Log.GetStringFromParameters(
                        inputStateAndSettings,
                        requestedOutputState)));

            OutputStateAndSettings outputStateAndSettings;

            if (_objects.TryGetValue(
                idOutput,
                out outputStateAndSettings))
            {
                outputStateAndSettings.OnBoundInputBlocked(
                    inputStateAndSettings,
                    requestedOutputState);
            }
        }

        public void OnBoundInputUnblocked(
            Guid idOutput,
            InputStateAndSettings inputStateAndSettings)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.OnBoundInputUnblocked(Guid idOutput, InputStateAndSettings inputStateAndSettings): [{0}]",
                    Log.GetStringFromParameters(idOutput, inputStateAndSettings)));

            OutputStateAndSettings outputStateAndSettings;

            if (_objects.TryGetValue(
                idOutput,
                out outputStateAndSettings))
            {
                outputStateAndSettings.OnBoundInputUnblocked(inputStateAndSettings);
            }
        }

        public Guid GetIdDcuForOutput(Guid idOutput)
        {
            OutputStateAndSettings outputStateAndSettings;

            return _objects.TryGetValue(
                    idOutput,
                    out outputStateAndSettings)
                ? outputStateAndSettings.IdDcu
                : Guid.Empty;
        }

        public IdDcuAndOutputNumber GetIdDcuAndOutputNumberForOutput(Guid idOutput)
        {
            OutputStateAndSettings outputStateAndSettings;

            return _objects.TryGetValue(
                    idOutput,
                    out outputStateAndSettings)
                ? new IdDcuAndOutputNumber(
                    outputStateAndSettings.IdDcu,
                    outputStateAndSettings.OutputNumber)
                : null;
        }

        public void AddOutputChanged(
            Guid outputGuid,
            Action<Guid, State> outputChanged)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.AddOutputChanged(Guid outputGuid, Action<Guid, State> outputChanged): [{0}]",
                    Log.GetStringFromParameters(outputGuid, outputChanged)));

            if (outputGuid == Guid.Empty
                || outputChanged == null)
            {
                return;
            }

            lock (_outputsChanged)
                if (_outputsChanged.ContainsKey(outputGuid))
                    _outputsChanged[outputGuid] += outputChanged;
                else
                    _outputsChanged.Add(
                        outputGuid,
                        outputChanged);
        }

        public void RemoveOutputChanged(
            Guid outputGuid,
            Action<Guid, State> outputChanged)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.RemoveOutputChanged(Guid outputGuid, Action<Guid, State> outputChanged): [{0}]",
                    Log.GetStringFromParameters(outputGuid, outputChanged)));

            if (outputGuid == Guid.Empty)
                return;

            lock (_outputsChanged)
            {
                if (!_outputsChanged.ContainsKey(outputGuid))
                    return;

                _outputsChanged[outputGuid] -= outputChanged;

                if (_outputsChanged[outputGuid] == null)
                    _outputsChanged.Remove(outputGuid);
            }
        }

        public State GetOutputState(Guid idOutput)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "State Outputs.GetOutputState(Guid idOutput): [{0}]",
                    Log.GetStringFromParameters(idOutput)));

            OutputStateAndSettings outputStateAndSettings;

            return
                _objects.TryGetValue(
                        idOutput,
                        out outputStateAndSettings)
                    ? outputStateAndSettings.OutputState
                    : State.Unknown;
        }

        public void RunOutputChanged(
            Guid idOutput,
            State outputState)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.RunOutputChanged(Guid idOutput, State outputState): [{0}]",
                    Log.GetStringFromParameters(
                        idOutput,
                        outputState)));

            _queueOutputChanged.Enqueue(
                new OutputStateGuid(
                    idOutput,
                    outputState));
        }


        public void ClearOutputsChanged()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void Outputs.ClearOutputsChanged()");

            _outputsChanged.Clear();
        }

        public void SendAllLogicalStates()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void Outputs.SendAllLogicalStates()");

            foreach (var outputStateAndSetting in _objects.ValuesSnapshot)
                if (outputStateAndSetting.OutputState != State.Unknown)
                {
                    outputStateAndSetting.SendOutputStateChanged();
                }
        }

        public void SendAllRealStates()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void Outputs.SendAllRealStates()");

            foreach (var outputStateAndSetting in _objects.ValuesSnapshot)
                if (outputStateAndSetting != null
                    && outputStateAndSetting.OutputRealState != State.Unknown)
                {
                    outputStateAndSetting.SendOutputRealStateChanged();
                }
        }

        public void OnBoundInputStateChanged(
            Guid idOutput,
            Guid idInput,
            Guid idDcu,
            State state)
        {
            OutputStateAndSettings outputStateAndSettings;

            if (_objects.TryGetValue(
                idOutput,
                out outputStateAndSettings))
            {
                outputStateAndSettings.OnBoundInputStateChanged(
                    idInput,
                    idDcu,
                    state);
            }
        }

        public void OnDcuDisconnected(
            [NotNull]
            DB.DCU dcu)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Outputs.OnDcuDisconnected(DB.DCU dcu): [{0}]",
                    Log.GetStringFromParameters(dcu)));

            var idOutputs = dcu.GuidOutputs;

            if (idOutputs == null)
                return;

            foreach (var idOutput in idOutputs)
            {
                OutputStateAndSettings outputStateAndSettings;

                if (_objects.TryGetValue(
                    idOutput,
                    out outputStateAndSettings))
                {
                    outputStateAndSettings.OnDcuDisconnected();
                }
            }
        }

        public void Off(
            string activatorKey,
            Guid outputId)
        {
            OutputStateAndSettings outputStateAndSettings;

            if (_objects.TryGetValue(
                outputId,
                out outputStateAndSettings))
            {
                outputStateAndSettings.Off(activatorKey);
            }
        }

        public void On(
            string activatorKey,
            Guid outputId)
        {
            OutputStateAndSettings outputStateAndSettings;

            if (_objects.TryGetValue(
                outputId,
                out outputStateAndSettings))
            {
                outputStateAndSettings.On(activatorKey);
            }
        }

        public bool HasOutputActivator(
            Guid idOutput,
            string activatorKey)
        {
            OutputStateAndSettings outputStateAndSettings;

            return
                _objects.TryGetValue(
                    idOutput,
                    out outputStateAndSettings)
                && outputStateAndSettings.HasOutputActivator(activatorKey);
        }

        public void ReRunOutputActivators(Guid idOutput)
        {
            OutputStateAndSettings outputStateAndSettings;

            if (_objects.TryGetValue(
                idOutput,
                out outputStateAndSettings))
            {
                outputStateAndSettings.ReRunOutputActivators();
            }
        }

        protected override OutputStateAndSettings CreateNewStateAndSettingsObject(DB.Output output)
        {
            var result =
                OutputStateAndSettings.CreateNewOutputStateAndSettings(output);

            _outputsStateAndSettingsByDcuAndNumber[
                new IdDcuAndOutputNumber(
                    result.IdDcu,
                    result.OutputNumber)] =
                result;

            return result;
        }

        protected override void OnRemoved(OutputStateAndSettings removedValue)
        {
            _outputsStateAndSettingsByDcuAndNumber.Remove(
                new IdDcuAndOutputNumber(
                    removedValue.IdDcu,
                    removedValue.OutputNumber));
        }
    }
}
