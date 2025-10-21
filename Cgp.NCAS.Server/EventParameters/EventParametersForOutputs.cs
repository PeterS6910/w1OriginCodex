using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(510)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventOutputRealStateChanged : EventParametersWithObjectIdAndState
    {
        public EventOutputRealStateChanged(
            UInt64 eventId,
            State outputRealState,
            Guid idOutput)
            : base(
                eventId,
                EventType.OutputRealStateChanged,
                idOutput,
                outputRealState)
        {

        }

        protected EventOutputRealStateChanged()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var realOutputState = OutputState.Unknown;

            switch (State)
            {
                case State.On:
                    realOutputState = OutputState.On;
                    break;

                case State.Off:
                    realOutputState = OutputState.Off;
                    break;
            }

            CCUConfigurationHandler.Singleton.OutputRealStateChanged(
                IdObject,
                DateTime,
                realOutputState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(511)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventOutputStateChanged : EventParametersWithObjectIdAndState
    {
        public EventOutputStateChanged(
            UInt64 eventId,
            State outputState,
            Guid idOutput)
            : base(
                eventId,
                EventType.OutputStateChanged,
                idOutput,
                outputState)
        {

        }

        protected EventOutputStateChanged()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var outputState = OutputState.Unknown;

            switch (State)
            {
                case State.On:
                    outputState = OutputState.On;
                    break;

                case State.Off:
                    outputState = OutputState.Off;
                    break;

                case State.UsedByAnotherAplication:
                    outputState = OutputState.UsedByAnotherAplication;
                    break;

                case State.OutOfRange:
                    outputState = OutputState.OutOfRange;
                    break;
            }

            CCUConfigurationHandler.Singleton.OutputStateChanged(
                IdObject,
                DateTime,
                outputState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            if (!GeneralOptions.Singleton.EventlogOutputStateChanged)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var output = Outputs.Singleton.GetById(IdObject);
            
            if (output == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = Enumerable.Repeat(
                output.IdOutput,
                1);

            if (output.DCU != null)
            {
                eventSourcesIds = eventSourcesIds.Concat(
                    Enumerable.Repeat(
                        output.DCU.IdDCU,
                        1));

                if (output.DCU.CCU != null)
                    eventSourcesIds = eventSourcesIds.Concat(
                        Enumerable.Repeat(
                            output.DCU.CCU.IdCCU,
                            1));
            }
            else if (output.CCU != null)
                eventSourcesIds = eventSourcesIds.Concat(
                    Enumerable.Repeat(
                        output.CCU.IdCCU,
                        1));

            var description =
                string.Format(
                    "Output {0} changed its state to {1}",
                    output,
                    State);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_OUTPUT_STATE_CHANGED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    description,
                    out eventlog,
                    out eventSources);
        }
    }
}
