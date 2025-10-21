using System;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(510)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventOutputRealStateChanged : EventParametersWithObjectIdAndState
    {
        public EventOutputRealStateChanged(
            State outputRealState,
            Guid idOutput)
            : base(
                EventType.OutputRealStateChanged,
                idOutput,
                outputRealState)
        {

        }

        public EventOutputRealStateChanged()
        {
            
        }
    }

    [LwSerialize(511)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventOutputStateChanged : EventParametersWithObjectIdAndState
    {
        public EventOutputStateChanged(
            State outputState,
            Guid idOutput)
            : base(
                EventType.OutputStateChanged,
                idOutput,
                outputState)
        {

        }

        public EventOutputStateChanged()
        {
            
        }
    }

    public static class TestOutputEvents
    {
        public static void EnqueueTestEvents(
            Guid idOutput)
        {
            Events.ProcessEvent(new EventOutputRealStateChanged(
                State.On,
                idOutput));

            Events.ProcessEvent(new EventOutputStateChanged(
                State.On,
                idOutput));
        }
    }
}
