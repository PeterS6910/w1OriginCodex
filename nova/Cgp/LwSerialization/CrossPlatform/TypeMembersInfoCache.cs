using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if COMPACT_FRAMEWORK
using Contal.IwQuickCF;
#else
using Contal.IwQuick;
#endif

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal class TypeMembersInfoCache : 
        ACache<
            TypeMembersInfoCache.SerializedMembersKey, 
            MemberInfo[]>
    {
        public class SerializedMembersKey : IEquatable<SerializedMembersKey>
        {
            private readonly Direction _direction;
            private readonly Type _instanceType;
            private readonly LwSerializationMode _mode;
            private readonly DirectMemberType _directMemberType;

            public SerializedMembersKey(
                Direction direction, 
                Type instanceType, 
                LwSerializationMode mode, 
                DirectMemberType directMemberType)
            {
                _direction = direction;
                _instanceType = instanceType;
                _mode = mode;
                _directMemberType = directMemberType;
            }

            public DirectMemberType DirectMemberType
            {
                get { return _directMemberType; }
            }

            public Direction Direction
            {
                get { return _direction; }
            }

            public Type InstanceType
            {
                get { return _instanceType; }
            }

            public LwSerializationMode Mode
            {
                get { return _mode; }
            }

            public override int GetHashCode()
            {
                int num = (int)_direction << 16;

                num |= (int)_mode << 8;
                num |= (int)_directMemberType;

                return _instanceType.GetHashCode() ^ num;
            }

            public bool Equals(SerializedMembersKey other)
            {
                return
                    _direction == other._direction &&
                    _instanceType == other._instanceType &&
                    _mode == other._mode &&
                    _directMemberType == other._directMemberType;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                var serializedMembersKey = obj as SerializedMembersKey;

                return 
                    serializedMembersKey != null && 
                    Equals(serializedMembersKey);
            }
        }

        private class MemberInfoSortingKey : IComparable<MemberInfoSortingKey>
        {
            private readonly string _memberInfoText;
            private readonly int _idx;

            public MemberInfoSortingKey(
                MemberInfo memberInfo,
                int idx)
            {
                MemberInfo = memberInfo;
                _memberInfoText = memberInfo.ToString();

                if (_memberInfoText.StartsWith("System."))
                    _memberInfoText = _memberInfoText.Substring(7);

                _idx = idx;
            }

            public MemberInfo MemberInfo { get; private set; }

            public int CompareTo(MemberInfoSortingKey other)
            {
                int result = _memberInfoText.CompareTo(other._memberInfoText);

                return
                    result == 0
                        ? _idx.CompareTo(other._idx)
                        : result;
            }
        }

        private volatile List<MemberInfoSortingKey> _membersInfoSortingBuffer =
            new List<MemberInfoSortingKey>();

        private IEnumerable<MemberInfo> GetSerializableMembersDirect(
            SerializedMembersKey key)
        {
            BindingFlags bindingFlags = 
                BindingFlags.Instance | 
                BindingFlags.Public | 
                BindingFlags.DeclaredOnly;

            DirectMemberType directMemberType = key.DirectMemberType;
            Type instanceType = key.InstanceType;

            if ((directMemberType & DirectMemberType.PrivateAndProtected) != 0)
                bindingFlags |= BindingFlags.NonPublic;

            if ((directMemberType & DirectMemberType.Static) != 0)
                bindingFlags |= BindingFlags.Static;

            IEnumerable<MemberInfo> result = 
                instanceType.GetFields(bindingFlags);

            if ((directMemberType & DirectMemberType.Properties) != 0)
            {
                bindingFlags |= BindingFlags.GetProperty;

                result = 
                    result.Concat(
                        instanceType
                            .GetProperties(bindingFlags)
                            .Where(propertyInfo => propertyInfo.CanWrite)
                            .Cast<MemberInfo>());
            }
            else
                if ((directMemberType & DirectMemberType.GetProperties) != 0)
                {
                    bindingFlags |= BindingFlags.GetProperty;

                    result = 
                        result.Concat(
                            instanceType.GetProperties(bindingFlags));
                }

            return result;
        }

        private static bool IsMemberSelectivelySerializable(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType != MemberTypes.Field &&
                    memberInfo.MemberType != MemberTypes.Property)
                return false;

            object[] customAttributes =
                memberInfo.GetCustomAttributes(
                    typeof(LwSerializeAttribute),
                    false);

            return customAttributes.Length > 0;
        }

        private IEnumerable<MemberInfo> GetSerializableMembersSelective(
            SerializedMembersKey key)
        {
            BindingFlags bindingFlags =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly;

            if (key.Direction == Direction.Serialization)
                bindingFlags |= BindingFlags.SetProperty;
            else
                bindingFlags |= BindingFlags.SetProperty;

            Type instanceType = key.InstanceType;

            return
                instanceType.GetMembers(bindingFlags)
                    .Where(IsMemberSelectivelySerializable);
        }

        protected override MemberInfo[] CreateValue(SerializedMembersKey key)
        {
            IEnumerable<MemberInfo> serializableMembers;

            switch (key.Mode)
            {
                case LwSerializationMode.Direct:

                    serializableMembers =
                        GetSerializableMembersDirect(key);
                    break;

                case LwSerializationMode.Selective:

                    serializableMembers =
                        GetSerializableMembersSelective(key);
                    break;

                default:
                    throw new ArgumentException("Invalid serialization mode");
                //break;
            }

            _membersInfoSortingBuffer.Clear();

            _membersInfoSortingBuffer.AddRange(
                serializableMembers
                    .Select((memberInfo, idx) =>
                        new MemberInfoSortingKey(
                            memberInfo,
                            idx)));

            _membersInfoSortingBuffer.Sort();

            var result = new MemberInfo[_membersInfoSortingBuffer.Count];

            for (int idx = 0; idx < result.Length; ++idx)
                result[idx] = _membersInfoSortingBuffer[idx].MemberInfo;

            return result;
        }
    }
}