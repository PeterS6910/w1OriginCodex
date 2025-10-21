using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using System.Collections;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal abstract class LwBinarySerialize : LwBinarySerializationFunctions
    {
        private static readonly Type KeyValuePairType = typeof(KeyValuePair<,>);

        static LwBinarySerialize()
        {
            PredefinedArrayTypes[typeof(bool)] = Types.tBool;
            PredefinedArrayTypes[typeof(byte)] = Types.tByte;
            PredefinedArrayTypes[typeof(char)] = Types.tChar;
            PredefinedArrayTypes[typeof(Int16)] = Types.tInt16;
            PredefinedArrayTypes[typeof(Int32)] = Types.tInt32;
            PredefinedArrayTypes[typeof(Int64)] = Types.tInt64;
            PredefinedArrayTypes[typeof(UInt16)] = Types.tUInt16;
            PredefinedArrayTypes[typeof(UInt32)] = Types.tUInt32;
            PredefinedArrayTypes[typeof(UInt64)] = Types.tUInt64;
            PredefinedArrayTypes[typeof(float)] = Types.tFloat;
            PredefinedArrayTypes[typeof(double)] = Types.tDouble;
            PredefinedArrayTypes[typeof(string)] = Types.tString;
            PredefinedArrayTypes[typeof(Guid)] = Types.tGuid2;
            PredefinedArrayTypes[typeof(object)] = Types.tArrayObject;

            PrimitiveTypeConvertors[typeof(bool)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tBool,
                    WriterMethod =
                        (binaryWriter, obj) => 
                            binaryWriter.Write((bool) obj)
                };

            PrimitiveTypeConvertors[typeof(byte)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tByte,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((byte)obj)
                };

            PrimitiveTypeConvertors[typeof(char)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tChar,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((short)(char)obj)
                };

            PrimitiveTypeConvertors[typeof(short)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tInt16,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((short)obj)
                };

            PrimitiveTypeConvertors[typeof(int)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tInt32,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((int)obj)
                };

            PrimitiveTypeConvertors[typeof(long)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tInt64,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((long)obj)
                };

            PrimitiveTypeConvertors[typeof(ushort)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tUInt16,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((ushort)obj)
                };

            PrimitiveTypeConvertors[typeof(uint)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tUInt32,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((uint)obj)
                };

            PrimitiveTypeConvertors[typeof(ulong)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tUInt64,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((ulong)obj)
                };

            PrimitiveTypeConvertors[typeof(float)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tFloat,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((float)obj)
                };

            PrimitiveTypeConvertors[typeof(double)] =
                new PrimitiveTypeWriterInfo
                {
                    Type = Types.tDouble,
                    WriterMethod =
                        (binaryWriter, obj) =>
                            binaryWriter.Write((double)obj)
                };
        }

        private static readonly IDictionary<Type, Types> PredefinedArrayTypes =
            new Dictionary<Type, Types>();

        private static Types GetArrayType(Type type)
        {
            Type elementType = type.GetElementType();

            Debug.Assert(elementType != null, "elementType != null");

            Types t;

            return 
                PredefinedArrayTypes.TryGetValue(elementType, out t) 
                    ? t 
                    : Types.tArray;
        }

        private delegate void DPrimitiveTypeWriter(
            BinaryWriter binaryWriter,
            object obj);

        private struct PrimitiveTypeWriterInfo
        {
            public Types Type;
            public DPrimitiveTypeWriter WriterMethod;
        }

        private static readonly IDictionary<Type, PrimitiveTypeWriterInfo> PrimitiveTypeConvertors =
            new Dictionary<Type, PrimitiveTypeWriterInfo>();

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="o"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        protected void DoSerialize(
            BinaryWriter binaryWriter, 
            object o, 
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo)
        {
            // Check null objects
            if (o == null)
            {
                SerializeNull(
                    binaryWriter,
                    serializationProperties,
                    memberInfo);

                return;
            }

            Type objectType = o.GetType();

            if (objectType.IsPrimitive)
            {
                // Serialize primitive type and string
                SerializePrimitiveType(
                    binaryWriter,
                    serializationProperties,
                    memberInfo,
                    o);

                return;
            }

            // If member is struct or enum
            if (objectType.IsValueType)
            {
                if (objectType == typeof (Guid))
                {
                    SerializeGuid2(
                        binaryWriter,
                        serializationProperties,
                        memberInfo,
                        (Guid) o);

                    return;
                }

                if (objectType.IsGenericType &&
                    objectType.GetGenericTypeDefinition() == KeyValuePairType)
                {
                    SerializeKeyValuePair(
                        binaryWriter,
                        serializationProperties,
                        memberInfo, o);

                    return;
                }

                SerializeStructOrEnum(
                    binaryWriter,
                    serializationProperties,
                    memberInfo, o);

                return;
            }

            if (!objectType.IsClass)
                throw new InvalidOperationException(
                    string.Format(
                        "Unknown object type \"{0}\" for LwSerialization mechanism",
                        objectType.FullName));

            // If member is array
            if (objectType.IsArray)
            {
                SerializeArray(
                    binaryWriter, 
                    serializationProperties,
                    memberInfo, 
                    (Array)o);

                return;
            }

            var objectAsString = o as string;

            // Serialize string
            if (objectAsString != null)
            {
                SerializeString(
                    binaryWriter,
                    serializationProperties, 
                    memberInfo, 
                    objectAsString);

                return;
            }

            if (objectType.IsGenericType)
            {
                var dictionary = o as IDictionary;

                if (dictionary != null)
                {
                    SerializeDictionary(
                        binaryWriter,
                        serializationProperties,
                        memberInfo, 
                        dictionary);

                    return;
                }

                var collection = o as ICollection;

                if (collection != null)
                    SerializeCollection(
                        binaryWriter,
                        serializationProperties,
                        memberInfo, 
                        collection);

                return;
            }

            var exception = o as Exception;

            if (exception != null)
            {
                SerializeException(
                    binaryWriter,
                    serializationProperties,
                    memberInfo, 
                    exception);

                return;
            }

            SerializeClass(
                binaryWriter, 
                serializationProperties,
                memberInfo, 
                o);
        }

        private static void SerializeString(
            BinaryWriter binaryWriter,
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            string objectAsString)
        {
            // Write string as array of bytes to file
            WriteType(
                binaryWriter,
                Types.tString,
                (byte) ETypesSecondByte.tNoArray,
                serializationProperties,
                memberInfo);

            byte[] stringBytes = Encoding.UTF8.GetBytes(objectAsString);

            binaryWriter.Write(stringBytes.Length);
            binaryWriter.Write(stringBytes);
        }

        /// <summary>
        /// Serialize null objects
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        private static void SerializeNull(
            BinaryWriter binaryWriter,
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo)
        {
            WriteType(
                binaryWriter,
                Types.tNull,
                memberInfo != null
                    ? GetTypeByteHashCode(ALwSerializable.GetTypeOfMember(memberInfo))
                    : (byte)ETypesSecondByte.tNoArray,
                serializationProperties,
                memberInfo);
        }

        /// <summary>
        /// Function for serialize Guid
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="guid"></param>
        private static void SerializeGuid2(
            BinaryWriter binaryWriter,
            SerializationProperties serializationProperties,
            MemberInfo memberInfo,
            Guid guid)
        {
            // Write string as array of bytes to file
            WriteType(
                binaryWriter,
                Types.tGuid2,
                (byte) ETypesSecondByte.tNoArray,
                serializationProperties,
                memberInfo);

            byte[] guidBytes = guid.ToByteArray();

            binaryWriter.Write(guidBytes);
        }

        /// <summary>
        /// Function for serialize primitive type and string
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        private static void SerializePrimitiveType(
            BinaryWriter binaryWriter,
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            object obj)
        {
            PrimitiveTypeWriterInfo primitiveTypeWriterInfo;

            Type objectType = obj.GetType();

            if (!PrimitiveTypeConvertors.TryGetValue(
                    objectType,
                    out primitiveTypeWriterInfo))
                throw new ArgumentException(
                    string.Format(
                        "Type is not implemented. Type '{0}'  of object '{1}'",
                        objectType,
                        memberInfo.Name));

            WriteType(
                binaryWriter,
                primitiveTypeWriterInfo.Type,
                (byte)ETypesSecondByte.tNoArray,
                serializationProperties,
                memberInfo);

            primitiveTypeWriterInfo.WriterMethod(
                binaryWriter, 
                obj);
        }

        /// <summary>
        /// Function for serialize array
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="array"></param>
        private void SerializeArray(
            BinaryWriter binaryWriter, 
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            Array array)
        {
            // Write array length to array of bytes
            Types eType = GetArrayType(array.GetType());

            WriteType(
                binaryWriter,
                eType,
                eType == Types.tArray
                    ? GetTypeByteHashCode(array.GetType())
                    : (byte)ETypesSecondByte.tArray,
                serializationProperties,
                memberInfo);

            binaryWriter.Write(array.Length);

            // Serialize members of array
            foreach (object member in array)
            {
                if (eType == Types.tArrayObject || memberInfo == null)
                    if (member != null && member.GetType().IsGenericType)
                        throw new ArgumentException("Can not serialize generic type");

                DoSerialize(
                    binaryWriter, 
                    member, 
                    serializationProperties, 
                    null);
            }
        }

        protected abstract void WriteVersion(BinaryWriter binaryWriter, Type type);

        /// <summary>
        /// Serialize class
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        private void SerializeClass(
            BinaryWriter binaryWriter, 
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            object obj)
        {
            Type objectType = obj.GetType();

            CheckIsSupportedType(objectType);

            GetPropertiesForType(
                objectType, 
                ref serializationProperties);

            // Filter members of class
            ICollection<MemberInfo> classMembersInfo =
                TypeMembersInfoCache.GetValue(
                    new TypeMembersInfoCache.SerializedMembersKey(
                        Direction.Serialization,
                        objectType,
                        serializationProperties.Mode,
                        serializationProperties.DirectMemberType));

            classMembersInfo = 
                GetMembersInfoForAssignMode(
                    obj,
                    classMembersInfo,
                    serializationProperties.AssignMode);

            // Write class to file
            WriteType(
                binaryWriter,
                Types.tClass,
                GetTypeByteHashCode(objectType),
                serializationProperties,
                memberInfo);

            binaryWriter.Write(classMembersInfo.Count);

            // Write version
            WriteVersion(binaryWriter, objectType);

            foreach (MemberInfo classMemberInfo in classMembersInfo)
                DoSerialize(
                    binaryWriter, 
                    ALwSerializable.GetValue(
                        classMemberInfo,
                        obj),
                    serializationProperties,
                    classMemberInfo);
        }

        /// <summary>
        /// Serialize struct or enum
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        private void SerializeStructOrEnum(
            BinaryWriter binaryWriter, 
            SerializationProperties serializationProperties,
            MemberInfo memberInfo, 
            object obj)
        {
            Type objectType = obj.GetType();

            CheckIsSupportedType(objectType);

            GetPropertiesForType(
                objectType, 
                ref serializationProperties);

            // Filter members of class
            MemberInfo[] seMembersInfo =
                TypeMembersInfoCache.GetValue(
                    new TypeMembersInfoCache.SerializedMembersKey(
                        Direction.Serialization,
                        objectType,
                        serializationProperties.Mode,
                        serializationProperties.DirectMemberType));

            // Write members count to array of bytes

            // Write struct or enum to file
            WriteType(
                binaryWriter, 
                Types.tStructEnum, 
                GetTypeByteHashCode(objectType), 
                serializationProperties,
                memberInfo);

            binaryWriter.Write(seMembersInfo.Length);

            // Write version
            WriteVersion(binaryWriter, objectType);

            // Serialize members
            foreach (var seMemberInfo in seMembersInfo)
                try
                {
                    DoSerialize(
                        binaryWriter,
                        ALwSerializable.GetValue(
                            seMemberInfo,
                            obj),
                        serializationProperties,
                        seMemberInfo);
                }
                catch (Exception)
                {
                }
        }

        /// <summary>
        /// Serialize generic type dictionary
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="dictionary"></param>
        private void SerializeDictionary(
            BinaryWriter binaryWriter, 
            SerializationProperties serializationProperties,
            MemberInfo memberInfo, 
            IDictionary dictionary)
        {
            // Get members count to array of bytes

            // Write dictionary to file
            WriteType(
                binaryWriter,
                Types.tDictionary,
                GetTypeByteHashCode(dictionary.GetType()),
                serializationProperties,
                memberInfo);

            binaryWriter.Write(dictionary.Count);

            // Serialize members
            foreach (DictionaryEntry keyValuePair in dictionary)
            {
                // Read and serialize key

                DoSerialize(
                    binaryWriter,
                    keyValuePair.Key,
                    serializationProperties,
                    null);

                // Read and serialize value

                DoSerialize(
                    binaryWriter,
                    keyValuePair.Value,
                    serializationProperties,
                    null);
            }
        }

        /// <summary>
        /// Serialize generic type collection
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="collection"></param>
        private void SerializeCollection(
            BinaryWriter binaryWriter, 
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            ICollection collection)
        {
            // Write collection to file
            WriteType(
                binaryWriter,
                Types.tCollection,
                GetTypeByteHashCode(collection.GetType()),
                serializationProperties,
                memberInfo);

            binaryWriter.Write(collection.Count);

            // Serialize members
            foreach (object member in collection)
                DoSerialize(
                    binaryWriter,
                    member,
                    serializationProperties,
                    null);
        }

        /// <summary>
        /// Serialize generic type key value pair
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        private void SerializeKeyValuePair(
            BinaryWriter binaryWriter, 
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            object obj)
        {
            Type objectType = obj.GetType();

            // Write key value pair to file
            WriteType(
                binaryWriter, 
                Types.tKeyValuePair, 
                GetTypeByteHashCode(objectType), 
                serializationProperties,
                memberInfo);

            // Find property
            MemberInfo[] membersInfo =
                TypeMembersInfoCache.GetValue(
                    new TypeMembersInfoCache.SerializedMembersKey(
                        Direction.Serialization, 
                        objectType, 
                        LwSerializationMode.Direct, 
                        DirectMemberType.GetProperties));

            // Find property key
            MemberInfo keyMemberInfo = 
                membersInfo
                    .FirstOrDefault(
                        memberInfo1 => memberInfo1.Name == "Key");

            // Read key and serialize
            if (keyMemberInfo == null)
                throw new ArgumentException("Exception by read key from kayvaluepair");

            object key =
                ALwSerializable.GetValue(
                    keyMemberInfo,
                    obj);

            DoSerialize(
                binaryWriter, 
                key,
                serializationProperties,
                null);

            // Find property value
            MemberInfo valueMemberInfo = 
                membersInfo
                    .FirstOrDefault(
                        memberInfo2 => memberInfo2.Name == "Value");

            // Read and serialize value
            if (valueMemberInfo == null)
                throw new ArgumentException(
                    "Exception by read value from KeyValuePair");

            object value = ALwSerializable.GetValue(
                valueMemberInfo,
                obj);

            DoSerialize(
                binaryWriter,
                value,
                serializationProperties,
                null);
        }

        /// <summary>
        /// Serialize exception
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="serializationProperties"></param>
        /// <param name="memberInfo"></param>
        /// <param name="exception"></param>
        private void SerializeException(
            BinaryWriter binaryWriter,
            SerializationProperties serializationProperties, 
            MemberInfo memberInfo, 
            Exception exception)
        {
            // Write exception to file
            WriteType(
                binaryWriter,
                Types.tException,
                (byte)ETypesSecondByte.tNoArray,
                serializationProperties,
                memberInfo);

            // Serialize type
            SerializeString(
                binaryWriter,
                serializationProperties,
                memberInfo,
                exception.GetType().FullName);

            SerializeString(
                binaryWriter, 
                serializationProperties,
                null, 
                exception.Message);

            if (exception.InnerException == null)
                SerializeNull(
                    binaryWriter,
                    serializationProperties,
                    null);
            else
                SerializeException(
                    binaryWriter,
                    serializationProperties,
                    null,
                    exception.InnerException);
        }

        private static void WriteType(
            BinaryWriter binaryWriter,
            Types type,
            byte typeSecondByte,
            SerializationProperties serializationProperties,
            MemberInfo memberInfo)
        {
            if (IsAssignModeLenient(serializationProperties.AssignMode))
            {
                binaryWriter.Write((byte) ((byte) type | 128));
                binaryWriter.Write(typeSecondByte);
                binaryWriter.Write(GetNameShortHashCode(memberInfo));
            }
            else
            {
                binaryWriter.Write((byte) type);
                binaryWriter.Write(typeSecondByte);
            }
        }

        /// <summary>
        /// Function for check serialized type
        /// </summary>
        /// <param name="type"></param>
        private static void CheckIsSupportedType(Type type)
        {
            if (type.FullName.IndexOf("System.") != 0)
                return;

            if (type == typeof(DateTime))
                return;

            if (type == typeof(TimeSpan))
                return;

            if (type == typeof(Stopwatch))
                return;

            throw new NotSupportedException("Particular type \"" + type.FullName + "\" is not supported");
        }

        private static ICollection<MemberInfo> GetMembersInfoForAssignMode(
            object o,
            ICollection<MemberInfo> membersInfo,
            AssignMode assignMode)
        {
            var lwSerializable = o as ALwSerializable;

            switch (assignMode)
            {
                case AssignMode.LenientDynamic:

                    if (lwSerializable == null)
                        throw new ArgumentException(
                            "If assign mode is lenient dynamic then instance being serialized must be a subclass of ALwSerializable");

                    return
                        membersInfo
                            .Where(
                                memberInfo =>
                                    lwSerializable.IsAttributeMarked(
                                        memberInfo.Name))
                            .ToList();


                case AssignMode.LenientDynamicProperties:

                    if (lwSerializable == null)
                        throw new ArgumentException(
                            "If assign mode is lenient dynamic then instance being serialized must be a subclass of ALwSerializable");

                    return
                        membersInfo
                            .Where(
                                memberInfo =>
                                    memberInfo.MemberType != MemberTypes.Property ||
                                    lwSerializable.IsAttributeMarked(memberInfo.Name))
                            .ToList();

                default:
                    return membersInfo;
            }
        }
    }

    internal class LwBinarySerialize<T> : 
        LwBinarySerialize,
        IDisposable
    {
        /// <summary>
        /// Function for serialization.
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="instanceToSerialize"></param>
        public void Serialize(Stream outputStream, T instanceToSerialize)
        {
            // Serialize
            DoSerialize(
                // Note: the disposal of binary writer was ommited intentionally.
                // If a binary writer gets disposed, it closes the underlying stream as well.
                new BinaryWriter(outputStream),
                instanceToSerialize,
                new SerializationProperties(),
                null);
        }

        /// <summary>
        /// Write version if type of stuct or class is the same as T
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="type"></param>
        protected override void WriteVersion(
            BinaryWriter binaryWriter, 
            Type type)
        {
            // Compare types
            if (type != typeof(T))
                return;

            // Write version to file
            Version version = ALwSerializable.GetVersion(type);

            binaryWriter.Write((ushort)version.Major);
            binaryWriter.Write((ushort)version.Minor);
            binaryWriter.Write((ushort)version.Build);
            binaryWriter.Write((ushort)version.Revision);
        }

        public void Dispose()
        {
        }
    }
}
