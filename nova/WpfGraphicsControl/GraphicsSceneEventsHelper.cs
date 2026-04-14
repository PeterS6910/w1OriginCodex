using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsSceneEventsHelper
    {
        private Action<ServerAlarmCore> _eventAlarmStateChnaged;
        private Action<Guid, byte, Guid> _eventCRStateChanged;
        private Action<Guid, bool, Guid> _eventCRTamperChanged;
        private Action<Guid, byte> _eventAlarmStateChanged;
        private Action<Guid, byte> _eventActivationStateChanged;
        private Action<Guid, byte, Guid> _eventStateChanged;
        private Action<Guid, byte, Guid> _eventRealStateChanged;
        private Action<Guid, byte> _doorEnvironmentEventStateChanged;
        private Action<Guid, byte, Guid> _eventInputStateChanged;
        private Action<Guid, byte> _eventCCUStateChanged;
        private Action<Guid, OnlineState, Guid> _eventDCUOnlineStateChanged;
        private readonly ProcessingQueue<DVoid2Void> _eventsProcessingQueue = new ProcessingQueue<DVoid2Void>();
        private readonly LinkedList<GraphicsScene> _graphicsScenes = new LinkedList<GraphicsScene>();

        public delegate void DEditGraphicsSymbolClick(ObjectType objectType, Guid id);
        public DEditGraphicsSymbolClick EditGraphicsSymbolClick;

        public void AddGraphicsScene(GraphicsScene graphicsScene)
        {
            if (graphicsScene == null)
                return;

            lock (_graphicsScenes)
            {
                if (!_graphicsScenes.Contains(graphicsScene))
                {
                    _graphicsScenes.AddLast(graphicsScene);
                    graphicsScene.EditObjectClick += graphicsScene_EditObjectClick;
                }
            }
        }

        public void RemoveGraphicsScene(GraphicsScene graphicsScene)
        {
            if (graphicsScene == null)
                return;

            lock (_graphicsScenes)
            {
                _graphicsScenes.Remove(graphicsScene);
            }
        }

        public void RemoveAllGraphicsScenes()
        {
            lock (_graphicsScenes)
            {
                _graphicsScenes.Clear();
            }
        }

        void _eventsProcessingQueue_ItemProcessing(DVoid2Void eventExecutor)
        {
            eventExecutor();
        }

        private void graphicsScene_EditObjectClick(ObjectType objectType, Guid id)
        {
            if (EditGraphicsSymbolClick != null)
                EditGraphicsSymbolClick(objectType, id);
        }

        public void Initialize()
        {
            _eventsProcessingQueue.ItemProcessing += _eventsProcessingQueue_ItemProcessing;

            //alarms
            _eventAlarmStateChnaged = AlarmStateChanged;
            AlarmStateChangedHandler.Singleton.RegisterChangeAlarms(_eventAlarmStateChnaged);

            //Card readers events
            _eventCRStateChanged = ChangeState;
            StateChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCRStateChanged);
            _eventCRTamperChanged = ChangeTamperState;
            TamperChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCRTamperChanged);

            //Alarm area events
            _eventAlarmStateChanged = ChangeAlarmState;
            AlarmStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventAlarmStateChanged);
            _eventActivationStateChanged = ChangeActivationState;
            ActivationStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventActivationStateChanged);

            //output events
            _eventStateChanged = ChangeOutputState;
            StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventStateChanged);
            _eventRealStateChanged = ChangeOutputRealState;
            RealStateChangedOutputHandler.Singleton.RegisterStateChanged(_eventRealStateChanged);

            //doorenvironment events
            _doorEnvironmentEventStateChanged = DoorEnvironmentChangeState;
            DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(_doorEnvironmentEventStateChanged);

            //inputs
            _eventInputStateChanged = ChangeInputState;
            StateChangedInputHandler.Singleton.RegisterStateChanged(_eventInputStateChanged);

            //ccu
            _eventCCUStateChanged = ChangeCCUState;
            StateChangedCCUHandler.Singleton.RegisterStateChanged(_eventCCUStateChanged);
            

            //dcu
            _eventDCUOnlineStateChanged = DCUOnlineStateChanged;
            DCUOnlineStateChangedHandler.Singleton.RegisterStateChanged(_eventDCUOnlineStateChanged);
        }

        public void Deinitialize()
        {
            //alarms
            AlarmStateChangedHandler.Singleton.UnregisterChangeAlarm(_eventAlarmStateChnaged);

            //Card readers events
            StateChangedCardReaderHandler.Singleton.UnregisterStateChanged(_eventCRStateChanged);
            TamperChangedCardReaderHandler.Singleton.UnregisterStateChanged(_eventCRTamperChanged);

            //Alarm areas events
            AlarmStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventAlarmStateChanged);
            ActivationStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventActivationStateChanged);

            //output events
            StateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventStateChanged);
            RealStateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventRealStateChanged);

            //doorenvironment events
            DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(DoorEnvironmentChangeState);

            //inputs
            StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventInputStateChanged);

            //ccu
            StateChangedCCUHandler.Singleton.UnregisterStateChanged(_eventCCUStateChanged);

            //dcu
            DCUOnlineStateChangedHandler.Singleton.UnregisterStateChanged(_eventDCUOnlineStateChanged);

            _eventsProcessingQueue.ItemProcessing -= _eventsProcessingQueue_ItemProcessing;
            _eventsProcessingQueue.Dispose();
        }

        private void AlarmStateChanged(ServerAlarmCore serverAlarm)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeAlarmState(serverAlarm));
                }
            }
        }

        private void ChangeInputState(Guid inputGuid, byte state, Guid parent)
        {
            var inputState = (State)state;

            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeInputState(inputGuid, inputState));
                }
            }
        }

        private void DoorEnvironmentChangeState(Guid doorEnvironmentGuid, byte state)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    grScene.ChangeDoorEnvironmentState(doorEnvironmentGuid, (DoorEnvironmentState)state);
                }
            }
        }

        private void ChangeState(Guid idCardReader, byte state, Guid parent)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(
                        () => grScene.ChangeCardReaderOnlineState(idCardReader, (OnlineState)state));
                }
            }
        }

        private void ChangeTamperState(Guid idCardReader, bool isTamper, Guid parent)
        {
            var state = State.Normal;

            if (isTamper)
                state = State.Tamper;

            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeCardReaderState(idCardReader, state));
                }
            }
        }

        private void ChangeAlarmState(Guid alarmAreaGuid, byte state)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(
                        () => grScene.ChangeAlarmAreaAlarmState(alarmAreaGuid, (AlarmAreaAlarmState)state));
                }
            }
        }

        private void ChangeActivationState(Guid alarmAreaGuid, byte state)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(
                        () => grScene.ChangeAlarmAreaState(alarmAreaGuid, (ActivationState)state));
                }
            }
        }

        private void ChangeOutputState(Guid outputGuid, byte state, Guid parent)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeOutputState(outputGuid, (State)state));
                }
            }
        }

        private void ChangeOutputRealState(Guid outputGuid, byte state, Guid parent)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeOutputState(outputGuid, (State)state));
                }
            }
        }

        private void ChangeCCUState(Guid ccuGuid, byte state)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeCcuState(ccuGuid, (CCUOnlineState)state));
                }
            }
        }

        private void DCUOnlineStateChanged(Guid dcuGuid, OnlineState onlineState, Guid parentGuid)
        {
            lock (_graphicsScenes)
            {
                foreach (var graphicsScene in _graphicsScenes)
                {
                    var grScene = graphicsScene;
                    _eventsProcessingQueue.Enqueue(() => grScene.ChangeDcuState(dcuGuid, onlineState));
                }
            }
        }
    }
}
