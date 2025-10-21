using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.CardReader;
using Contal.Drivers.LPC3250;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(560)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrUpgradePercentageSet : EventParameters
    {
        public int NodeLogicalAddress { get; private set; }
        public byte CardReaderAddress { get; private set; }
        public int Progress { get; private set; }

        public EventCrUpgradePercentageSet(
            int nodeLogicalAddress,
            byte cardReaderAddress,
            int progress)
            : base(
                EventType.CRUpgradePercentageSet)
        {
            NodeLogicalAddress = nodeLogicalAddress;
            CardReaderAddress = cardReaderAddress;
            Progress = progress;
        }

        public EventCrUpgradePercentageSet()
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
    }

    [LwSerialize(561)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrUpgradeResultSet : EventParameters
    {
        public int NodeLogicalAddress { get; private set; }
        public byte CardReaderAddress { get; private set; }
        public byte Result { get; private set; }

        public EventCrUpgradeResultSet(
            int nodeLogicalAddress,
            byte cardReaderAddress,
            byte result)
            : base(
                EventType.CRUpgradeResultSet)
        {
            NodeLogicalAddress = nodeLogicalAddress;
            CardReaderAddress = cardReaderAddress;
            Result = result;
        }

        public EventCrUpgradeResultSet()
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
    }

    [LwSerialize(562)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCeUpgradeFinished : EventParameters
    {
        public int Action { get; private set; }
        public int Result { get; private set; }
        public string Version { get; private set; }

        public EventCeUpgradeFinished(
            int action,
            int result,
            string version)
            : base(
                EventType.CEUpgradeFinished)
        {
            Action = action;
            Result = result;
            Version = version;
        }

        public EventCeUpgradeFinished()
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
    }

    [LwSerialize(563)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuUpgradeFileUnpackProgress : EventParameters
    {
        public int Percentage { get; private set; }

        public EventCcuUpgradeFileUnpackProgress(
            int percentage)
            : base(
                EventType.CcuUpgradeFileUnpackProgress)
        {
            Percentage = percentage;
        }

        public EventCcuUpgradeFileUnpackProgress()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Percentage: {0}",
                    Percentage));
        }
    }

    [LwSerialize(564)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventProcessDcuUpgradePackageFailed : EventParameters
    {
        public byte[] NodesWiating { get; private set; }
        public byte ErrorCode { get; private set; }

        public EventProcessDcuUpgradePackageFailed(
            byte[] nodesWaiting,
            byte errorCode)
            : base(
                EventType.ProcessDCUUpgradePackageFailed)
        {
            NodesWiating = nodesWaiting;
            ErrorCode = errorCode;
        }

        public EventProcessDcuUpgradePackageFailed()
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
    }

    [LwSerialize(565)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventProcessCrUpgradePackageFailed : EventParameters
    {
        public byte[] CrsWiating { get; private set; }
        public byte ErrorCode { get; private set; }

        public EventProcessCrUpgradePackageFailed(
            byte[] crsWiating,
            byte errorCode)
            : base(
                EventType.ProcessCRUpgradePackageFailed)
        {
            CrsWiating = crsWiating;
            ErrorCode = errorCode;
        }

        public EventProcessCrUpgradePackageFailed()
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
    }

    [LwSerialize(566)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuUpgraderStartFailed : EventParameters
    {
        public EventCcuUpgraderStartFailed()
            : base(
                EventType.CCUUpgraderStartFailed)
        {

        }
    }

    [LwSerialize(567)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuUpgradePercentageSet : EventParameters
    {
        public byte NodeLogicalAddress { get; private set; }
        public int Progress { get; private set; }

        public EventDcuUpgradePercentageSet(
            byte nodeLogicalAddress,
            int progress)
            : base(
                EventType.DcuUpgradePercentageSet)
        {
            NodeLogicalAddress = nodeLogicalAddress;
            Progress = progress;
        }

        public EventDcuUpgradePercentageSet()
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
    }

    public static class TestUpgradeEvents
    {
        public static void EnqueueTestEvents()
        {
            Events.ProcessEvent(
                new EventCrUpgradePercentageSet(
                    8,
                    1,
                    10));

            Events.ProcessEvent(
                new EventCrUpgradeResultSet(
                    8,
                    1,
                    (byte) CRUpgradeResult.FailedToReadBinaryFile));

            Events.ProcessEvent(
                new EventCeUpgradeFinished(
                    (byte) CeUpgradeAction.BeginUpgrade,
                    (byte) ActionResultUpgrade.ChecksumValid,
                    "xxx.xx"));

            Events.ProcessEvent(
                new EventCcuUpgradeFileUnpackProgress(
                    10));

            Events.ProcessEvent(
                new EventProcessDcuUpgradePackageFailed(
                    new byte[]
                    {
                        8
                    },
                    (byte) UnpackPackageFailedCode.GetChecksumFailed));

            Events.ProcessEvent(
                new EventProcessCrUpgradePackageFailed(
                    new byte[]
                    {
                        8,
                        1
                    },
                    (byte) UnpackPackageFailedCode.GetChecksumFailed));

            Events.ProcessEvent(
                new EventCcuUpgraderStartFailed());

            Events.ProcessEvent(
                new EventDcuUpgradePercentageSet(
                    8,
                    10));
        }
    }
}
