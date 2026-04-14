using System;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IInputChangedListener : IEquatable<IInputChangedListener>
    {
        void OnInputChanged(
            Guid guidInput,
            State state);
    }

    internal class InputChangedEventHandlers
    {
        private readonly HashSet<IInputChangedListener> _eventHandlerGroup =
            new HashSet<IInputChangedListener>();

        private readonly InputStateAndSettings _inputStateAndSettings;

        public InputChangedEventHandlers(InputStateAndSettings inputStateAndSettings)
        {
            _inputStateAndSettings = inputStateAndSettings;
        }

        public void AddInputChangedListener(IInputChangedListener inputChangedListener)
        {
            lock (_eventHandlerGroup)
                _eventHandlerGroup.Add(inputChangedListener);
        }

        public void RemoveInputChangedListener(IInputChangedListener inputChangedListener)
        {
            lock (_eventHandlerGroup)
                _eventHandlerGroup.Remove(inputChangedListener);
        }

        public void DoRunInputChanged(State inputState)
        {
            lock (_eventHandlerGroup)
                foreach (var inputChangedListener in _eventHandlerGroup)
                    try
                    {
                        inputChangedListener.OnInputChanged(
                            _inputStateAndSettings.Id,
                            inputState);
                    }
                    catch (Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
                    }
        }
    }
}