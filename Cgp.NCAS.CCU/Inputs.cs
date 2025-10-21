using System;
using System.Collections.Generic;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.Cgp.Globals;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal sealed class Inputs : AStateAndSettingsObjectCollection<
        Inputs,
        InputStateAndSettings,
        DB.Input>
    {
        public interface IAcknowledgeStateChangedListener
            : IEquatable<IAcknowledgeStateChangedListener>
        {
            void OnAcknowledgeStateChanged(
                Guid idObject,
                bool acknowledgeState);
        }

        internal class SendInputStatePqRequest : IProcessingQueueRequest
        {
            private readonly InputStateAndSettings _inputStateAndSettings;
            private readonly State _inputState;
            private readonly bool _inputChanged;
            private readonly bool _alarmWasGenerated;

            public SendInputStatePqRequest(
                InputStateAndSettings inputStateAndSettings,
                State inputState,
                bool inputChanged,
                bool alarmWasGenerated)
            {
                _inputState = inputState;
                _inputStateAndSettings = inputStateAndSettings;
                _inputChanged = inputChanged;
                _alarmWasGenerated = alarmWasGenerated;
            }

            public void Execute()
            {
                _inputStateAndSettings.SendEventInputStateChanged(
                   _inputState,
                   _inputChanged,
                   _alarmWasGenerated);
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public class InputChangedPqRequest : IProcessingQueueRequest
        {
            private readonly State _inputState;
            private readonly InputChangedEventHandlers _inputChangedEventHandler;

            public InputChangedPqRequest(
                InputChangedEventHandlers inputChangedEventHandler,
                State inputState)
            {
                _inputChangedEventHandler = inputChangedEventHandler;
                _inputState = inputState;
            }

            public void Execute()
            {
                _inputChangedEventHandler.DoRunInputChanged(_inputState);
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private class IdDcuAndInputNumber : IEquatable<IdDcuAndInputNumber>
        {
            private readonly Guid _idDcu;
            private readonly byte _inputNumber;

            public IdDcuAndInputNumber(
                Guid idDcu,
                byte inputNumber)
            {
                _idDcu = idDcu;
                _inputNumber = inputNumber;
            }

            public override int GetHashCode()
            {
                return _idDcu.GetHashCode() ^ _inputNumber.GetHashCode();
            }

            public bool Equals(IdDcuAndInputNumber other)
            {
                return
                    other != null
                    && other._idDcu.Equals(_idDcu)
                    && other._inputNumber.Equals(_inputNumber);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as IdDcuAndInputNumber);
            }
        }

        //private const string BLOCKED_INPUT_SETTINGS_PATH = "NandFlash\\CCU\\Temp\\";
        public const string PROCESSING_QUEUE_NAME_INPUT_CHANGED = "Inputs: input changed";
        public const string PROCESSING_QUEUE_NAME_SEND_STATE = "Inputs: send state";
        public const string PROCESSING_QUEUE_NAME_FUNCTION_TO_RUN = "Inputs: function to run";
 
        private readonly ThreadPoolQueue<InputChangedPqRequest> _queueInputChanged =
            new ThreadPoolQueue<InputChangedPqRequest>(ThreadPoolGetter.Get());

        private readonly ThreadPoolQueue<SendInputStatePqRequest>_queueSendInputState=
            new ThreadPoolQueue<SendInputStatePqRequest>(ThreadPoolGetter.Get());

 
        public override ObjectType ObjectType
        {
            get { return ObjectType.Input; }
        }

        private Inputs()
            : base (null)
        {
            IOControl.InputChangedExt2 += OnCcuInputhanged;
        }



        public IBoolExpression GetBoolExpresionInputTamper(Guid inputId)
        {
            InputStateAndSettings inputStateAndSettings;

            if (_objects.TryGetValue(inputId, out inputStateAndSettings))
            {
                return inputStateAndSettings.InputTamper;
            }

            return null;
        }

        public static int GetInputCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int Inputs.GetInputCount()");
            try
            {
                var result = IOControl.GetInputCount();
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int Inputs.GetInputCount return {0}", Log.GetStringFromParameters(result)));
                return result;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                //if (MainBoard.Type == MainBoardType.CCU40)
                //    return IOControl.GetInputCount();
                //else if (MainBoard.Type == MainBoardType.CCU12)
                //    return 8;

                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "int Inputs.GetInputCount return 0");
                return 0;
            }
        }

        private readonly Dictionary<IdDcuAndInputNumber, InputStateAndSettings> _inputsStateAndSettingsByDcuAndNumber =
            new Dictionary<IdDcuAndInputNumber, InputStateAndSettings>();

        public void SetCcuInputsBsiLevels(uint[] bsiLevels)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.SetCcuInputsBsiLevels(uint[] bsiLevels): [{0}]",
                    Log.GetStringFromParameter(bsiLevels)));

            _objects.ForEach(
                (key,
                    value) =>
                    value.SetBsiLevels(bsiLevels));
        }

        private void OnCcuInputhanged(
            int inputNumber,
            InputStateVariantG7 inputStateVariant,
            int voltage)
        {
            try
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => string.Format("InputLevelChanged - input: {0} level: {1}",
                        inputNumber,
                        inputStateVariant.InputState));

                InputStateAndSettings inputStateAndSettings;

                if (!_inputsStateAndSettingsByDcuAndNumber.TryGetValue(
                    new IdDcuAndInputNumber(
                        Guid.Empty,
                        (byte)inputNumber),
                    out inputStateAndSettings))
                {
                    return;
                }

                switch (inputStateVariant.InputState)
                {

                    case InputState.Alarm:

                        inputStateAndSettings.SetBaseInputState(State.Alarm);
                        break;

                    case InputState.Break:

                        inputStateAndSettings.SetBaseInputState(State.Break);
                        break;

                    case InputState.Normal:

                        inputStateAndSettings.SetBaseInputState(State.Normal);
                        break;

                    case InputState.Short:

                        inputStateAndSettings.SetBaseInputState(State.Short);
                        break;

                    case InputState.Unknown:

                        inputStateAndSettings.SetBaseInputState(State.Unknown);
                        break;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void OnDcuDisconnected(
            [NotNull]
            DB.DCU dcu)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.OnDcuDisconnected(DB.DCU dcu): [{0}]",
                    Log.GetStringFromParameters(dcu)));

            var idInputs = dcu.GuidInputs;

            if (idInputs == null)
                return;

            foreach (var idInput in idInputs)
            {
                InputStateAndSettings inputStateAndSettings;

                if (_objects.TryGetValue(
                    idInput,
                    out inputStateAndSettings))
                {
                    inputStateAndSettings.ResetInputState();
                }
            }
        }

        public void OnDcuInputChanged(Guid guidDCU, byte inputNumber, State inputState)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void Inputs.OnDcuInputChanged(Guid guidDCU, byte inputNumber, State inputState): [{0}]",
                        Log.GetStringFromParameters(guidDCU, inputNumber, inputState)));

            InputStateAndSettings inputStateAndSettings;

            if (_inputsStateAndSettingsByDcuAndNumber.TryGetValue(
                new IdDcuAndInputNumber(
                    guidDCU,
                    inputNumber),
                out inputStateAndSettings))
            {
                inputStateAndSettings.SetBaseInputState(inputState);
            }
        }

        public void EnqueueSendInputStateToServer(SendInputStatePqRequest sendInputStatePqRequest)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.EnqueueSendInputStateToServer(InputStateGuid inputStateGuid): [{0}]",
                    Log.GetStringFromParameters(sendInputStatePqRequest)));

            _queueSendInputState.Enqueue(sendInputStatePqRequest);
        }


        public State GetInputLogicalState(Guid inputGuid)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "State Inputs.GetInputLogicalState(Guid inputGuid): [{0}]",
                    Log.GetStringFromParameters(inputGuid)));

            var result = State.Unknown;

            InputStateAndSettings inputStateAndSettings;

            if (_objects.TryGetValue(
                inputGuid,
                out inputStateAndSettings))
            {
                result = inputStateAndSettings.InputLogicalState;
            }

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "State Inputs.GetInputLogicalState return {0}",
                    Log.GetStringFromParameters(result)));

            return result;
        }

        public void AddInputChangedListener(
            Guid inputGuid,
            IInputChangedListener inputChangedListener)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.AddInputChangedListener(Guid inputGuid, IInputChangedListener inputChanged): [{0}]",
                    Log.GetStringFromParameters(
                        inputGuid,
                        inputChangedListener)));

            AddInputChangedListenerInternal(
                inputGuid,
                inputChangedListener,
                Guid.Empty);
        }

        public void AddInputChangedListener(
            Guid inputGuid,
            IInputChangedListener inputChangedListener,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.AddInputChangedListener(Guid inputGuid, IInputChangedListener inputChanged, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        inputGuid,
                        inputChangedListener,
                        guidAlarmArea)));

            AddInputChangedListenerInternal(
                inputGuid,
                inputChangedListener,
                guidAlarmArea);
        }

        private void AddInputChangedListenerInternal(
            Guid inputGuid,
            IInputChangedListener inputChangedListener,
            Guid idAlarmArea)
        {
            InputStateAndSettings inputStateAndSettings;

            if (_objects.TryGetValue(
                inputGuid,
                out inputStateAndSettings))
            {
                inputStateAndSettings.AddInputChangedListener(
                    inputChangedListener,
                    idAlarmArea);
            }
        }

        public void RemoveInputChangedListener(
            Guid inputGuid,
            IInputChangedListener inputChangedListener)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.RemoveInputChanged(Guid inputGuid, IInputChangedListener inputChanged): [{0}]",
                    Log.GetStringFromParameters(inputGuid, inputChangedListener)));

            RemoveInputChangedListenerInternal(
                inputGuid,
                inputChangedListener,
                Guid.Empty);
        }

        private void RemoveInputChangedListenerInternal(
            Guid inputGuid,
            IInputChangedListener inputChangedListener,
            Guid idAlarmArea)
        {
            if (inputGuid == Guid.Empty
                 || inputChangedListener == null)
            {
                return;
            }

            InputStateAndSettings inputStateAndSettings;

            if (!_objects.TryGetValue(
                inputGuid,
                out inputStateAndSettings))
            {
                return;
            }

            inputStateAndSettings.RemoveInputChangedListener(
                inputChangedListener,
                idAlarmArea);
        }

        public void RemoveInputChangedListener(
            Guid inputGuid,
            IInputChangedListener inputChangedListener,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.RemoveInputChangedListener(Guid inputGuid, IInputChangedListener inputChanged, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        inputGuid,
                        inputChangedListener,
                        guidAlarmArea)));

            RemoveInputChangedListenerInternal(
                inputGuid,
                inputChangedListener,
                guidAlarmArea);
        }

        public void EnqueueInputLogicalStateChanged(InputChangedPqRequest inputChangedPqRequest)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.EnqueueInputLogicalStateChanged(InputChangedEventHandlers inputChangedEventHandler): [{0}]",
                    Log.GetStringFromParameters(inputChangedPqRequest)));

            _queueInputChanged.Enqueue(inputChangedPqRequest);
        }


        public void RemoveStateChangedEventsOnBlockingObject(Guid idInput)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void Inputs.RemoveStateChangedEventsOnBlockingObject(Guid idInput): [{0}]",
                    Log.GetStringFromParameters(idInput)));

            InputStateAndSettings inputStateAndSettings;

            if (_objects.TryGetValue(
                idInput,
                out inputStateAndSettings))
            {
                inputStateAndSettings.RemoveStateChangedEventsOnBlockingObject();
            }
        }

        public Guid GetInputDCU(Guid inputGuid)
        {
            InputStateAndSettings inputStateAndSettings;

            return _objects.TryGetValue(
                    inputGuid,
                    out inputStateAndSettings)
                ? inputStateAndSettings.IdDcu
                : Guid.Empty;
        }

        public bool IsInputBlocked(Guid idInput)
        {
            InputStateAndSettings inputStateAndSetting;

            return _objects.TryGetValue(
                idInput,
                out inputStateAndSetting) && inputStateAndSetting.IsBlocked;
        }

        public bool IsInputBlockedAndCurrentStateIsNotNormal(Guid idInput)
        {
            InputStateAndSettings inputStateAndSetting;

            return _objects.TryGetValue(
                idInput,
                out inputStateAndSetting) && inputStateAndSetting.IsBlockedAndCurrentStateIsNotNormal;
        }

        public void SendAllStates()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void Inputs.SendAllStates()");

            foreach (var inputStateAndSetting in _objects.ValuesSnapshot)
                if (inputStateAndSetting.InputLogicalState != State.Unknown)
                    EnqueueSendInputStateToServer(
                        new SendInputStatePqRequest(
                            inputStateAndSetting,
                            inputStateAndSetting.InputLogicalState,
                            false,
                            false));
        }

        public void BindOutputToInput(
            Guid idInput,
            OutputStateAndSettings outputStateAndSettings)
        {
            InputStateAndSettings inputStateAndSettings;

            if (!_objects.TryGetValue(
                idInput,
                out inputStateAndSettings))
            {
                InterCcuCommunication.Singleton.SetObjectActualState(
                    ObjectType.Output,
                    outputStateAndSettings.Id);

                return;
            }

            inputStateAndSettings.BindOutputToInput(outputStateAndSettings);
        }

        public void UnbindOutputFromInput(
            Guid idInput,
            OutputStateAndSettings outputStateAndSettings)
        {
            InputStateAndSettings inputStateAndSettings;

            if (_objects.TryGetValue(
                idInput,
                out inputStateAndSettings))
            {
                inputStateAndSettings.UnbindOutputFromInput(outputStateAndSettings);
            }
        }

        public bool TryDirectBindToInput(
            Guid idInput,
            OutputStateAndSettings outputStateAndSettings)
        {
            InputStateAndSettings inputStateAndSettings;

            if (!_objects.TryGetValue(
                idInput,
                out inputStateAndSettings))
            {
                return false;
            }

            if (inputStateAndSettings.IdDcu == Guid.Empty
                || inputStateAndSettings.IdDcu != outputStateAndSettings.IdDcu)
            {
                return false;
            }

            if (!inputStateAndSettings.IsBlocked)
            {
                DCUs.Singleton.DirectlyBindOutputToInput(
                    inputStateAndSettings.IdDcu,
                    outputStateAndSettings.OutputNumber,
                    inputStateAndSettings.InputNumber);
            }

            return true;
        }

        protected override InputStateAndSettings CreateNewStateAndSettingsObject(DB.Input dbObject)
        {
            var result =
                InputStateAndSettings.CreateNewInputStateAndSettings(dbObject);

            _inputsStateAndSettingsByDcuAndNumber.Add(
                new IdDcuAndInputNumber(
                    result.IdDcu,
                    result.InputNumber),
                result);

            return result;
        }

        protected override void OnRemoved(InputStateAndSettings removedValue)
        {
            _inputsStateAndSettingsByDcuAndNumber.Remove(
                new IdDcuAndInputNumber(
                    removedValue.IdDcu,
                    removedValue.InputNumber));
        }

        public byte GetInputNumber(Guid idInput)
        {
            InputStateAndSettings inputStateAndSettings;

            return _objects.TryGetValue(
                    idInput,
                    out inputStateAndSettings)
                ? inputStateAndSettings.InputNumber
                : (byte)0;
        }

        public void IccuInputOrOutputStateChanged(Guid idInput, bool state)
        {
            InputStateAndSettings inputStateAndSettings;

            if (_objects.TryGetValue(
                idInput,
                out inputStateAndSettings))
            {
                inputStateAndSettings.IccuInputOrOutputStateChanged(state);
            }
        }
    }
}
