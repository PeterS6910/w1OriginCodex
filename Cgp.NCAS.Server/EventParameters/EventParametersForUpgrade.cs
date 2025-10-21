using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(560)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrUpgradePercentageSet : EventParameters
    {
        public int NodeLogicalAddress { get; private set; }
        public byte CardReaderAddress { get; private set; }
        public int Progress { get; private set; }

        protected EventCrUpgradePercentageSet()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            if (NodeLogicalAddress != -1)
                parameters.Append(
                    string.Format(
                        ", Node logical address: {0}",
                        NodeLogicalAddress));

            parameters.Append(
                string.Format(
                    ", Card reader address: {0}, Progress: {1}",
                    CardReaderAddress,
                    Progress));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CRUpgradePercentageSet(
                NodeLogicalAddress,
                CardReaderAddress,
                Progress,
                ccuSettings.IPAddressString);
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

    [LwSerialize(561)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrUpgradeResultSet : EventParameters
    {
        public int NodeLogicalAddress { get; private set; }
        public byte CardReaderAddress { get; private set; }
        public byte Result { get; private set; }

        public EventCrUpgradeResultSet(
            UInt64 eventId,
            int nodeLogicalAddress,
            byte cardReaderAddress,
            byte result)
            : base(
                eventId,
                EventType.CRUpgradeResultSet)
        {
            NodeLogicalAddress = nodeLogicalAddress;
            CardReaderAddress = cardReaderAddress;
            Result = result;
        }

        protected EventCrUpgradeResultSet()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            if (NodeLogicalAddress != -1)
                parameters.Append(
                    string.Format(
                        ", Node logical address: {0}",
                        NodeLogicalAddress));

            parameters.Append(
                string.Format(
                    ", Card reader address: {0}, Result: {1}",
                    CardReaderAddress,
                    Result));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CRUpgradeResultSet(
                NodeLogicalAddress,
                CardReaderAddress,
                Result,
                ccuSettings.IPAddressString);
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

    [LwSerialize(562)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCeUpgradeFinished : EventParameters
    {
        public int Action { get; private set; }
        public int Result { get; private set; }
        public string Version { get; private set; }

        public EventCeUpgradeFinished(
            UInt64 eventId,
            int action,
            int result,
            string version)
            : base(
                eventId,
                EventType.CEUpgradeFinished)
        {
            Action = action;
            Result = result;
            Version = version;
        }

        protected EventCeUpgradeFinished()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Action: {0}, Result: {1}, Version {2}",
                    Action,
                    Result,
                    Version));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CEUpgradeFinished(
                Action,
                Result,
                Version,
                ccuSettings.IPAddressString);
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

    [LwSerialize(563)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuUpgradeFileUnpackProgress : EventParameters
    {
        public int Percentage { get; private set; }

        public EventCcuUpgradeFileUnpackProgress(
            UInt64 eventId,
            int percentage)
            : base(
                eventId,
                EventType.CcuUpgradeFileUnpackProgress)
        {
            Percentage = percentage;
        }

        protected EventCcuUpgradeFileUnpackProgress()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Percentage: {0}",
                    Percentage));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CcuUpgradeFileUnpackProgress(
                Percentage,
                ccuSettings.IPAddressString);
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

    [LwSerialize(564)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventProcessDcuUpgradePackageFailed : EventParameters
    {
        public byte[] NodesWiating { get; private set; }
        public byte ErrorCode { get; private set; }

        public EventProcessDcuUpgradePackageFailed(
            UInt64 eventId,
            byte[] nodesWaiting,
            byte errorCode)
            : base(
                eventId,
                EventType.ProcessDCUUpgradePackageFailed)
        {
            NodesWiating = nodesWaiting;
            ErrorCode = errorCode;
        }

        protected EventProcessDcuUpgradePackageFailed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(string.Format(
                ", Nodes wiating: {0}, Error code: {1}",
                NodesWiatingToString(),
                ErrorCode));
        }

        private string NodesWiatingToString()
        {
            if (NodesWiating == null)
                return string.Empty;

            var nodesWiatingEnumerator = NodesWiating.GetEnumerator();
            
            if (!nodesWiatingEnumerator.MoveNext())
                return string.Empty;

            var nodesWaitingString = new StringBuilder(
                nodesWiatingEnumerator.Current.ToString());

            while (nodesWiatingEnumerator.MoveNext())
            {
                nodesWaitingString.Append(
                    string.Format(
                        ", {0}",
                        nodesWiatingEnumerator.Current));
            }

            return nodesWaitingString.ToString();
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.ProcessDCUUpgradePackageFailed(
                NodesWiating,
                ErrorCode,
                ccuSettings.IPAddressString);
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

    [LwSerialize(565)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventProcessCrUpgradePackageFailed : EventParameters
    {
        public byte[] CrsWiating { get; private set; }
        public byte ErrorCode { get; private set; }

        public EventProcessCrUpgradePackageFailed(
            UInt64 eventId,
            byte[] crsWiating,
            byte errorCode)
            : base(
                eventId,
                EventType.ProcessCRUpgradePackageFailed)
        {
            CrsWiating = crsWiating;
            ErrorCode = errorCode;
        }

        protected EventProcessCrUpgradePackageFailed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Card readers wiating: {0}, Error code: {1}",
                    CrsWiatingToString(),
                    ErrorCode));
        }

        private string CrsWiatingToString()
        {
            if (CrsWiating == null)
                return string.Empty;

            var crsWiatingEnumerator = CrsWiating.GetEnumerator();

            if (!crsWiatingEnumerator.MoveNext())
                return string.Empty;

            var crsWaitingString = new StringBuilder(
                crsWiatingEnumerator.Current.ToString());

            while (crsWiatingEnumerator.MoveNext())
            {
                crsWaitingString.Append(
                    string.Format(
                        ", {0}",
                        crsWiatingEnumerator.Current));
            }

            return crsWaitingString.ToString();
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.ProcessCRUpgradePackageFailed(
                ErrorCode,
                CrsWiating,
                ccuSettings.IPAddressString);
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

    [LwSerialize(566)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuUpgraderStartFailed : EventParameters
    {
        public EventCcuUpgraderStartFailed(
            UInt64 eventId)
            : base(
                eventId,
                EventType.CCUUpgraderStartFailed)
        {

        }

        protected EventCcuUpgraderStartFailed()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CCUUpgraderStartFailed(ccuSettings.IPAddressString);
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

    [LwSerialize(567)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuUpgradePercentageSet : EventParameters
    {
        public byte NodeLogicalAddress { get; private set; }
        public int Progress { get; private set; }

        public EventDcuUpgradePercentageSet(
            UInt64 eventId,
            byte nodeLogicalAddress,
            int progress)
            : base(
                eventId,
                EventType.DcuUpgradePercentageSet)
        {
            NodeLogicalAddress = nodeLogicalAddress;
            Progress = progress;
        }

        protected EventDcuUpgradePercentageSet()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Node logical address: {0}, Progress: {1}",
                    NodeLogicalAddress,
                    Progress));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DcuUpgradePercentageSet(
                NodeLogicalAddress,
                Progress,
                ccuSettings.IPAddressString);
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
}
