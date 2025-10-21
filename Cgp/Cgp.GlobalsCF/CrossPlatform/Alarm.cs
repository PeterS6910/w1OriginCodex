using System;
using System.Collections.Generic;

using Contal.IwQuick.Data;

namespace Contal.Cgp.Globals
{
    [Serializable]
    [LwSerialize(700)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmParameter : IEquatable<AlarmParameter>
    {
        public ParameterType TypeParameter { get; private set; }
        public string Value { get; private set; }

        public AlarmParameter(ParameterType typeParameter, string value)
        {
            TypeParameter = typeParameter;
            Value = value;
        }

        public AlarmParameter()
        {

        }

        #region IEquatable<AlarmParameter> Members

        public bool Equals(AlarmParameter alarmParameter)
        {
            if (alarmParameter == null)
                return false;

            if (TypeParameter != alarmParameter.TypeParameter)
                return false;

            if (Value == null)
                return alarmParameter.Value == null;

            return Value.Equals(alarmParameter.Value);
        }

        #endregion
    }

    [Serializable]
    [LwSerialize(701)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmKey : IEquatable<AlarmKey>
    {
        public AlarmType AlarmType { get; private set; }
        public IdAndObjectType AlarmObject { get; private set; }

        public List<IdAndObjectType> ExtendedObjects { get; private set; }
        public List<AlarmParameter> Parameters { get; private set; }

        public Guid ReferencedAlarmId { get; set; }

        public AlarmKey(
            AlarmType alarmType,
            IdAndObjectType alarmObject,
            IEnumerable<IdAndObjectType> extendedObjects,
            IEnumerable<AlarmParameter> parameters)
        {
            AlarmType = alarmType;
            AlarmObject = alarmObject;
            ExtendedObjects = extendedObjects != null
                ? new List<IdAndObjectType>(extendedObjects)
                : null;

            Parameters = parameters != null
                ? new List<AlarmParameter>(parameters)
                : null;
        }

        public AlarmKey(
            AlarmType alarmType,
            IdAndObjectType alarmObject)
            : this(
                alarmType,
                alarmObject,
                null,
                null)
        {

        }

        public AlarmKey(
            AlarmType alarmType,
            IdAndObjectType alarmObject,
            IEnumerable<AlarmParameter> parameters
            )
            : this(
                alarmType,
                alarmObject,
                null,
                parameters)
        {

        }

        public AlarmKey(
            AlarmType alarmType,
            IdAndObjectType alarmObject,
            IEnumerable<IdAndObjectType> extendedObjects)
            : this(
                alarmType,
                alarmObject,
                extendedObjects,
                null)
        {

        }

        public AlarmKey()
        {
            
        }

        public override int GetHashCode()
        {
            var hashCode = (int) AlarmType;

            if (AlarmObject != null)
                hashCode = AlarmObject.GetHashCode() ^ hashCode;

            if (ReferencedAlarmId != Guid.Empty)
                hashCode = ReferencedAlarmId.GetHashCode() ^ hashCode;

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AlarmKey);
        }

        #region IEquatable<AlarmKey> Members

        public bool Equals(AlarmKey other)
        {
            if (other == null)
                return false;

            if (AlarmType != other.AlarmType)
                return false;

            if (AlarmObject == null)
            {
                if (other.AlarmObject != null)
                    return false;
            }
            else if (!AlarmObject.Equals(other.AlarmObject))
                return false;

            bool checkExtendedObjects;
            bool checkParameters;
            CheckExtendedObjectsParameters(out checkExtendedObjects, out checkParameters);

            if (checkExtendedObjects && !SameExtendedObjects(other))
                return false;

            if (checkParameters && !SameParameters(other))
                return false;

            return ReferencedAlarmId.Equals(other.ReferencedAlarmId);
        }

        #endregion

        private void CheckExtendedObjectsParameters(out bool checkExtendedObjects, out bool checkParameters)
        {
            switch (AlarmType)
            {
                //Temporary condition for alarm type Input_Alarm that allow alarm to be different in ParentObjectList and ParameterList
                case AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached:
                case AlarmType.Input_Alarm:
                case AlarmType.Input_Tamper:
                    checkExtendedObjects = false;
                    checkParameters = false;
                    break;
                case AlarmType.CCU_ClockUnsynchronized:
                    checkExtendedObjects = true;
                    checkParameters = false;
                    break;
                default:
                    checkExtendedObjects = true;
                    checkParameters = true;
                    break;
            }
        }

        public bool SameExtendedObjects(AlarmKey other)
        {
            try
            {
                if (ExtendedObjects == null && other.ExtendedObjects == null)
                {
                    return true;
                }

                if (ExtendedObjects == null || other.ExtendedObjects == null)
                    return false;

                var extendedObjectsEnumerator = ExtendedObjects.GetEnumerator();
                var otherExtendedObjectsEnumerator = other.ExtendedObjects.GetEnumerator();

                while (extendedObjectsEnumerator.MoveNext())
                {
                    if (!otherExtendedObjectsEnumerator.MoveNext())
                        return false;

                    if (!extendedObjectsEnumerator.Current.Equals(otherExtendedObjectsEnumerator.Current))
                        return false;
                }

                return !otherExtendedObjectsEnumerator.MoveNext();
            }
            catch
            {
                return false;
            }
        }

        public bool SameParameters(AlarmKey other)
        {
            try
            {
                if (Parameters == null && other.Parameters == null)
                {
                    return true;
                }

                if (Parameters == null || other.Parameters == null)
                    return false;

                var parametersEnumerator = Parameters.GetEnumerator();
                var otherParametersEnumerator = other.Parameters.GetEnumerator();

                while (parametersEnumerator.MoveNext())
                {
                    if (!otherParametersEnumerator.MoveNext())
                        return false;

                    if (!parametersEnumerator.Current.Equals(otherParametersEnumerator.Current))
                        return false;
                }

                return !otherParametersEnumerator.MoveNext();
            }
            catch
            {
                return false;
            }
        }

        public void SetExtendedObjects(IEnumerable<IdAndObjectType> extendedObjects)
        {
            ExtendedObjects = extendedObjects != null
                ? new List<IdAndObjectType>(extendedObjects)
                : null;
        }

        public void SetParameters(IEnumerable<AlarmParameter> parameters)
        {
            Parameters = parameters != null
                ? new List<AlarmParameter>(parameters)
                : null;
        }
    }

    [Serializable]
    [LwSerialize(702)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class Alarm : IEquatable<Alarm>
    {
        public Guid Id { get; private set; }
        public AlarmKey AlarmKey { get; private set; }
        public DateTime CreatedDateTime { get; set; }
        public AlarmState AlarmState { get; set; }
        public bool IsAcknowledged { get; set; }
        public bool IsBlocked { get { return IsBlockedGeneral || IsBlockedIndividual; } }
        public bool IsBlockedGeneral { get; private set; }
        public bool IsBlockedIndividual { get; private set; }
        public DateTime? LastIndividualBlockingChangeDateTime { get; private set; }

        public Alarm(
            AlarmKey alarmKey,
            AlarmState alarmState)
            : this(
                Guid.NewGuid(),
                alarmKey,
                DateTime.Now,
                alarmState,
                false,
                false,
                false)
        {

        }

        public Alarm(
            AlarmKey alarmKey,
            DateTime date,
            AlarmState alarmState)
            : this(
                Guid.NewGuid(),
                alarmKey,
                date,
                alarmState,
                false,
                false,
                false)
        {

        }

        public Alarm(
            Guid id,
            AlarmKey alarmKey,
            DateTime date,
            AlarmState alarmState,
            bool isAcnowledged,
            bool isBlockedGeneral,
            bool isBlockedIndividual)
        {
            Id = id;
            AlarmKey = alarmKey;
            CreatedDateTime = date;
            AlarmState = alarmState;
            IsAcknowledged = isAcnowledged;
            IsBlockedGeneral = isBlockedGeneral;
            IsBlockedIndividual = isBlockedIndividual;
        }

        public Alarm()
        {

        }

        public bool BlockAlarmGeneral()
        {
            if (!IsBlockedGeneral)
            {
                IsBlockedGeneral = true;
                return true;
            }

            return false;
        }

        public bool UnblockAlarmGeneral()
        {
            if (IsBlockedGeneral)
            {
                IsBlockedGeneral = false;
                return true;
            }

            return false;
        }

        public bool BlockAlarmIndividual(DateTime utcDateTime)
        {
            if (!IsBlockedIndividual
                && (LastIndividualBlockingChangeDateTime == null
                    || LastIndividualBlockingChangeDateTime < utcDateTime))
            {
                IsBlockedIndividual = true;
                LastIndividualBlockingChangeDateTime = utcDateTime;

                return true;
            }

            return false;
        }

        public bool UnblockAlarmIndividual(DateTime utcDateTime)
        {
            if (IsBlockedIndividual
                && (LastIndividualBlockingChangeDateTime == null
                    || LastIndividualBlockingChangeDateTime < utcDateTime))
            {
                IsBlockedIndividual = false;
                LastIndividualBlockingChangeDateTime = utcDateTime;

                return true;
            }

            return false;
        }

        public bool AcknowledgeState()
        {
            if (!IsAcknowledged)
            {
                IsAcknowledged = true;
                return true;
            }

            return false;
        }

        #region IEquatable<Alarm> Members

        public bool Equals(Alarm other)
        {
            if (other == null)
                return false;

            return AlarmKey.Equals(other.AlarmKey);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as Alarm);
        }

        public override int GetHashCode()
        {
            return AlarmKey.GetHashCode();
        }
    }
}
