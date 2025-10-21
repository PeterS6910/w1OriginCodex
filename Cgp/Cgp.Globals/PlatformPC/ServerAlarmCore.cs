using System;
using System.Collections.Generic;
using System.Linq;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Globals.PlatformPC
{
    [Serializable]
    [LwSerialize(708)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class IdServerAlarm : IEquatable<IdServerAlarm>
    {
        public Guid IdOwner { get; private set; }
        public Guid Id { get; private set; }

        private IdServerAlarm()
        {
            
        }

        public IdServerAlarm(
            Guid idOwner,
            Guid id)
        {
            IdOwner = idOwner;
            Id = id;
        }

        #region IEquatable<AlarmKey> Members

        public bool Equals(IdServerAlarm other)
        {
            if (other == null)
                return false;

            return IdOwner.Equals(other.IdOwner)
                   && Id.Equals(other.Id);
        }

        #endregion

        public override int GetHashCode()
        {
            return IdOwner.GetHashCode() ^ Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdServerAlarm);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}{1}",
                IdOwner,
                Id);
        }
    }

    [Serializable]
    [LwSerialize(707)]
    [LwSerializeMode(LwSerializationMode.Selective)]
    public class ServerAlarmCore : IEquatable<ServerAlarmCore>
    {
        [LwSerialize]
        public IdServerAlarm IdServerAlarm { get; private set; }
        [LwSerialize]
        public Alarm Alarm { get; private set; }
        [LwSerialize]
        private readonly ICollection<IdAndObjectType> _parentObjects;
        [LwSerialize]
        private readonly ICollection<IdAndObjectType> _extendedObjects;
        public bool OwnerIsOffline { get; set; }
        [LwSerialize]
        public bool AcknowledgeInPending { get; set; }
        [LwSerialize]
        public DateTime? IndividualBlockinInPending { get; set; }
        [LwSerialize]
        public DateTime? IndividualUnblockinInPending { get; set; }

        [LwSerialize]
        public string Name { get; set; }
        [LwSerialize]
        public string ParentObject { get; private set; }
        [LwSerialize]
        public string Description { get; private set; }

        public ServerAlarmCore ReferencedServerAlarm { get; set; }
        public string ParentsString { get; set; }
        public string ParentsStringReversed { get; set; }
        public AlarmPriority AlarmPriority { get; set; }
        public Guid PresentationGruopId { get; set; }
        public bool HasAlarmInstructions { get; set; }

        [LwSerialize]
        private readonly Dictionary<IdAndObjectType, string> _specificObjectNames;

        public ICollection<IdAndObjectType> RelatedObjects
        {
            get
            {
                if (_parentObjects == null
                    && Alarm.AlarmKey.AlarmObject == null
                    && _extendedObjects == null
                    && Alarm.AlarmKey.ExtendedObjects == null)
                {
                    return null;
                }

                var relatedObjects = Enumerable.Empty<IdAndObjectType>();

                if (_parentObjects != null)
                    relatedObjects = relatedObjects.Concat(
                        _parentObjects);

                if (Alarm.AlarmKey.AlarmObject != null)
                    relatedObjects = relatedObjects.Concat(
                        Enumerable.Repeat(
                            Alarm.AlarmKey.AlarmObject,
                            1));

                if (_extendedObjects != null)
                    relatedObjects = relatedObjects.Concat(
                        _extendedObjects);

                if (Alarm.AlarmKey.ExtendedObjects != null)
                    relatedObjects = relatedObjects.Concat(
                        Alarm.AlarmKey.ExtendedObjects);

                return new LinkedList<IdAndObjectType>(relatedObjects);
            }
        }

        protected ServerAlarmCore()
        {
            
        }

        public ServerAlarmCore(
            Alarm alarm,
            string name,
            string parentObject,
            string description)
            : this(
                Guid.Empty,
                alarm,
                null,
                null,
                name,
                parentObject,
                description,
                null)
        {

        }

        public ServerAlarmCore(
            Guid idOwner,
            Alarm alarm,
            string name,
            string parentObject,
            string description)
            : this(
                idOwner,
                alarm,
                null,
                null,
                name,
                parentObject,
                description,
                null)
        {

        }

        public ServerAlarmCore(
            Alarm alarm,
            ICollection<IdAndObjectType> parentObjects,
            string name,
            string parentObject,
            string description)
            : this(
                Guid.Empty,
                alarm,
                parentObjects,
                null,
                name,
                parentObject,
                description,
                null)

        {

        }

        public ServerAlarmCore(
            Guid idOwner,
            Alarm alarm,
            ICollection<IdAndObjectType> parentObjects,
            string name,
            string parentObject,
            string description)
            : this(
                idOwner,
                alarm,
                parentObjects,
                null,
                name,
                parentObject,
                description,
                null)
        {

        }



        public ServerAlarmCore(
            Guid idOwner,
            Alarm alarm,
            ICollection<IdAndObjectType> parentObjects,
            string name,
            string parentObject,
            string description,
            IEnumerable<KeyValuePair<IdAndObjectType, string>> specificObjectNames)
            : this(
                idOwner,
                alarm,
                parentObjects,
                null,
                name,
                parentObject,
                description,
                specificObjectNames)
        {

        }

        public ServerAlarmCore(
            Alarm alarm,
            ICollection<IdAndObjectType> parentObjects,
            ICollection<IdAndObjectType> extendedObjects,
            string name,
            string parentObject,
            string description)
            : this(
                Guid.Empty,
                alarm,
                parentObjects,
                extendedObjects,
                name,
                parentObject,
                description,
                null)
        {

        }

        public ServerAlarmCore(
            Guid idOwner,
            Alarm alarm,
            ICollection<IdAndObjectType> parentObjects,
            ICollection<IdAndObjectType> extendedObjects,
            string name,
            string parentObject,
            string description)
            : this(
                idOwner,
                alarm,
                parentObjects,
                extendedObjects,
                name,
                parentObject,
                description,
                null)
        {

        }

        public ServerAlarmCore(
            Guid idOwner,
            Alarm alarm,
            ICollection<IdAndObjectType> parentObjects,
            ICollection<IdAndObjectType> extendedObjects,
            string name,
            string parentObject,
            string description,
            IEnumerable<KeyValuePair<IdAndObjectType, string>> specificObjectNames)
        {
            Alarm = alarm;

            IdServerAlarm = new IdServerAlarm(
                idOwner,
                alarm.Id);

            _parentObjects = parentObjects;
            _extendedObjects = extendedObjects;
            Name = name;
            ParentObject = parentObject;
            Description = description;

            if (specificObjectNames != null)
            {
                _specificObjectNames = new Dictionary<IdAndObjectType, string>();

                foreach (var specificObjectName in specificObjectNames)
                    _specificObjectNames.Add(
                        specificObjectName.Key,
                        specificObjectName.Value);
            }
        }

        #region IEquatable<ServerAlarm> Members

        public bool Equals(ServerAlarmCore other)
        {
            return Alarm.Equals(other.Alarm);
        }

        public static bool operator !=(ServerAlarmCore alarm1, ServerAlarmCore alarm2)
        {
            if (ReferenceEquals(alarm1, null))
                return (!ReferenceEquals(alarm2, null));

            if (ReferenceEquals(alarm2, null))
                return true; // expression (!ReferenceEquals(v1, null)) always true

            return !alarm1.Equals(alarm2);
        }

        public static bool operator ==(ServerAlarmCore alarm1, ServerAlarmCore alarm2)
        {
            if (ReferenceEquals(alarm1, null))
                return (ReferenceEquals(alarm2, null));

            if (ReferenceEquals(alarm2, null))
                return false; // expression(ReferenceEquals(v1, null)) always false if reached this point

            return alarm1.Equals(alarm2);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as ServerAlarmCore);
        }

        public override int GetHashCode()
        {
            return Alarm.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool IsBlockedIndividual
        {
            get
            {
                if (IndividualUnblockinInPending != null)
                    return false;

                return IndividualBlockinInPending != null
                       || Alarm.IsBlockedIndividual;
            }
        }

        public bool IsBlocked
        {
            get { return IsBlockedIndividual || Alarm.IsBlockedGeneral; }
        }

        public bool IsAcknowledged
        {
            get
            {
                return AcknowledgeInPending
                       || Alarm.IsAcknowledged;
            }
        }

        public string GetSpecificObjectName(IdAndObjectType idAndObjectType)
        {
            if (_specificObjectNames == null)
                return null;

            lock (_specificObjectNames)
            {
                string specificObjectName;

                if (!_specificObjectNames.TryGetValue(
                    idAndObjectType,
                    out specificObjectName))
                {
                    return null;
                }

                return specificObjectName;
            }
        }
    }
}
