using System;
using System.Collections.Generic;
using System.IO;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(307)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AlarmArea : IDbObject
    {
        [LwSerialize]
        public Guid IdAlarmArea { get; set; }
        [LwSerialize]
        public int Id { get; set; }
        [LwSerialize]
        public string Name { get; set; }
        [LwSerialize]
        public string ShortName { get; set; }

        [LwSerialize]
        public virtual SensorPurpose Purpose { get; set; }

        private List<Guid> _guidAAInputs = new List<Guid>();
        [LwSerialize]
        public List<Guid> GuidAAInputs { get { return _guidAAInputs; } set { _guidAAInputs = value; } }

        #region Object for automatic activation

        private ObjectType _objForAutomaticActObjectType;
        [LwSerialize]
        public ObjectType ObjForAutomaticActObjectType { get { return _objForAutomaticActObjectType; } set { _objForAutomaticActObjectType = value; } }
        [LwSerialize]
        public Guid? ObjForAutomaticActId { get; set; }
        [LwSerialize]
        public bool AutomaticDeactive { get; set; }
        [LwSerialize]
        public bool IsInvertedObjForAutomaticAct { get; set; }

        #endregion

        #region Object for forced time buying

        private ObjectType _objForForcedTimeBuyingObjectType;

        [LwSerialize]
        public ObjectType ObjForForcedTimeBuyingObjectType
        {
            get { return _objForForcedTimeBuyingObjectType; } 
            set { _objForForcedTimeBuyingObjectType = value; }
        }

        [LwSerialize]
        public Guid? ObjForForcedTimeBuyingId { get; set; }

        [LwSerialize]
        public bool IsInvertedObjForForcedTimeBuying { get; set; }

        [LwSerialize]
        public bool NotForcedTimeBuyingProvideOnlyUnset { get; set; }

        #endregion

        [LwSerialize]
        public bool PreAlarm { get; set; }
        [LwSerialize]
        public int? PreAlarmDuration { get; set; }
        [LwSerialize]
        public int TemporaryUnsetDuration { get; set; }
        [LwSerialize]
        public bool PreWarning { get; set; }
        [LwSerialize]
        public int? PreWarningDuration { get; set; }
        [LwSerializeAttribute]
        public bool TimeBuyingEnabled { get; set; }
        [LwSerializeAttribute]
        public int? TimeBuyingMaxDuration { get; set; }
        [LwSerializeAttribute]
        public int? TimeBuyingTotalMax { get; set; }
        [LwSerializeAttribute]
        public bool TimeBuyingOnlyInPrewarning { get; set; }
        [LwSerialize]
        public bool AcknowledgeOFF { get; set; }
        [LwSerialize]
        public bool ABAlarmHandling { get; set; }
        [LwSerialize]
        public int PercentageSensorsToAAlarm { get; set; }

        private Guid _guidOutputActivation = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputActivation { get { return _guidOutputActivation; } set { _guidOutputActivation = value; } }

        private Guid _guidOutputAlarmState = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputAlarmState { get { return _guidOutputAlarmState; } set { _guidOutputAlarmState = value; } }

        private Guid _guidOutputPrewarning = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputPrewarning { get { return _guidOutputPrewarning; } set { _guidOutputPrewarning = value; } }

        private Guid _guidOutputTmpUnsetEntry = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputTmpUnsetEntry { get { return _guidOutputTmpUnsetEntry; } set { _guidOutputTmpUnsetEntry = value; } }

        private Guid _guidOutputTmpUnsetExit = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputTmpUnsetExit { get { return _guidOutputTmpUnsetExit; } set { _guidOutputTmpUnsetExit = value; } }

        private Guid _guidOutputAAlarm = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputAAlarm { get { return _guidOutputAAlarm; } set { _guidOutputAAlarm = value; } }

        private Guid _guidOutputSiren = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputSiren { get { return _guidOutputSiren; } set { _guidOutputSiren = value; } }

        [LwSerialize]
        public int? SirenMaxOnPeriod { get; set; }

        private Guid _guidOutputSabotage = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputSabotage { get { return _guidOutputSabotage; } set { _guidOutputSabotage = value; } }

        private Guid _guidOutputNotAcknowledged = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputNotAcknowledged { get { return _guidOutputNotAcknowledged; } set { _guidOutputNotAcknowledged = value; } }

        private Guid _guidOutputMotion = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputMotion { get { return _guidOutputMotion; } set { _guidOutputMotion = value; } }

        [LwSerialize]
        public Guid IdOutputSetByObjectForAaFailed{ get; set; }

        [LwSerialize]
        public int OutputSetNotCalmAaByObjectForAaOnPeriod { get; set; }

        [LwSerialize]
        public virtual bool? AllowSendingStateToCRs { get; set; }

        [LwSerialize]
        public bool UseEIS { get; set; }
        [LwSerialize]
        public int? FilterTimeEIS { get; set; }
        [LwSerialize]
        public Guid? ActivationStateInputEIS { get; set; }
        [LwSerialize]
        public Guid? SetUnsetOutputEIS { get; set; }

        [LwSerialize]
        public bool EnableEventlogsInCR { get; set; }

        [LwSerialize]
        public bool AaSet { get; set; }
        [LwSerialize]
        public bool AaUnset { get; set; }
        [LwSerialize]
        public bool AaAlarm { get; set; }
        [LwSerialize]
        public bool AaNormal { get; set; }
        [LwSerialize]
        public bool AaAcknowledged { get; set; }
        [LwSerialize]
        public bool AaUnconditionalSet { get; set; }

        [LwSerialize]
        public bool SensorAlarm { get; set; }
        [LwSerialize]
        public bool SensorNormal { get; set; }
        [LwSerialize]
        public bool SensorAcknowledged { get; set; }
        [LwSerialize]
        public bool SensorUnblocked { get; set; }
        [LwSerialize]
        public bool SensorTemporarilyBlocked { get; set; }
        [LwSerialize]
        public bool SensorPermanentlyBlocked { get; set; }

        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        [LwSerialize]
        public virtual AlarmAreaAutomaticActivationMode AutomaticActivationMode { get; set; }
        [LwSerialize]
        public virtual bool? AlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmAreaSetByOnOffObjectFailedId { get; set; }

        [LwSerialize]
        public virtual bool AlwaysProvideUnsetForTimeBuying { get; set; }

        public Guid GetGuid()
        {
            return IdAlarmArea;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AlarmArea;
        }

        public override string ToString()
        {
            return string.Format(
                "{0} {1}",
                Id.ToString("D2"),
                !string.IsNullOrEmpty(ShortName)
                    ? ShortName
                    : Name);
        }
    }
}
