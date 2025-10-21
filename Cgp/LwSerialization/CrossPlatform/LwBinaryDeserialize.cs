using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal abstract class LwBinaryDeserialize :
        LwBinarySerializationFunctions,
        IDeserializationBuilder
    {
        // Property to get file version
        public Version FileVersion { get; private set; }

        // Property to check that file version and runtime version is the same
        public bool IsSameVersion { get; private set; }

        #region readers

        private delegate void DReadType(
            Stream inputStream,
            IDeserializationBuilder builder);

        private static void ReadNull(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
        }

        private static void ReadFixedLengthBuffer(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            int length;
            byte[] buffer = builder.GetFixedLengthBuffer(out length);

            if (inputStream.Read(buffer, 0, length) != length)
                throw new LwSerializationException();
        }

        private static void ReadLengthAndBuffer(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            byte[] buffer = builder.GetBuffer(sizeof(int));

            if (inputStream.Read(buffer, 0, sizeof(int)) != sizeof(int))
                throw new LwSerializationException();

            int length;
            buffer = builder.GetLengthAndBuffer(out length);

            if (length <= 0)
                return;

            if (inputStream.Read(buffer, 0, length) != length)
                throw new LwSerializationException();
        }

        private static void ReadSequence(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            byte[] buffer = builder.GetBuffer(sizeof(int));

            if (inputStream.Read(buffer, 0, sizeof(int)) != sizeof(int))
                throw new LwSerializationException();

            int length = builder.GetLength();

            for (int i = 0; i < length; ++i)
                DoDeserialize(inputStream, builder);
        }

        private static void ReadVersionAndSequence(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            byte[] buffer = builder.GetBuffer(sizeof(int));

            if (inputStream.Read(buffer, 0, sizeof(int)) != sizeof(int))
                throw new LwSerializationException();

            int length = builder.GetLength();

            if (builder.VersionRequired)
            {
                const int size = 4 * sizeof(ushort);
                buffer = builder.GetBuffer(size);

                if (inputStream.Read(buffer, 0, size) != size)
                    throw new LwSerializationException();

                builder.Version = new Version(
                    BitConverter.ToUInt16(buffer, 0),
                    BitConverter.ToUInt16(buffer, sizeof(ushort)),
                    BitConverter.ToUInt16(buffer, 2 * sizeof(ushort)),
                    BitConverter.ToUInt16(buffer, 3 * sizeof(ushort)));
            }

            for (int i = 0; i < length; ++i)
                DoDeserialize(inputStream, builder);
        }

        private static void ReadSequenceOfPairs(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            byte[] buffer = builder.GetBuffer(sizeof(int));

            if (inputStream.Read(buffer, 0, sizeof(int)) != sizeof(int))
                throw new LwSerializationException();

            int length = builder.GetLength();

            for (int i = 0; i < length; ++i)
            {
                DoDeserialize(inputStream, builder);
                DoDeserialize(inputStream, builder);
            }
        }

        private static void ReadPair(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            DoDeserialize(inputStream, builder);
            DoDeserialize(inputStream, builder);
        }

        private static void ReadException(
            Stream inputstream,
            IDeserializationBuilder builder)
        {
            DoDeserialize(inputstream, builder);
            DoDeserialize(inputstream, builder);
            DoDeserialize(inputstream, builder);
        }

        #endregion

        private delegate ABuilderContext DGetBuilderContext(
            LwBinaryDeserialize lwBinaryDeserialize,
            StreamTypeInfo streamTypeInfo,
            Type requestedType);

        private static StreamTypeInfo ReadStreamTypeInfo(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            // Read type
            byte[] readBuffer = builder.GetBuffer(2);

            if (inputStream.Read(readBuffer, 0, 2) != 2)
                throw new LwSerializationException();

            var type = (Types)((byte)(readBuffer[0] & 0x7F));
            // Read type hash code
            byte typeSecondByte = readBuffer[1];

            AssignMode assignMode;
            Int16 nameHashCode;

            // Check assign mode is strict or lenient and read name hash code
            if ((byte)(readBuffer[0] & 0x80) != 0)
            {
                assignMode = AssignMode.Lenient;

                if (inputStream.Read(readBuffer, 0, sizeof(short)) != sizeof(short))
                    throw new LwSerializationException();

                nameHashCode = BitConverter.ToInt16(readBuffer, 0);
            }
            else
            {
                assignMode = AssignMode.Strict;
                nameHashCode = 0;
            }

            return
                new StreamTypeInfo(
                    type,
                    typeSecondByte,
                    assignMode,
                    nameHashCode);
        }

        protected static void DoDeserialize(
            Stream inputStream,
            IDeserializationBuilder builder)
        {
            StreamTypeInfo streamTypeInfo =
                ReadStreamTypeInfo(
                    inputStream,
                    builder);

            builder = builder.SetupStreamTypeInfo(streamTypeInfo);

            DReadType reader =
                streamTypeInfo.TypeSecondByte == (byte)ETypesSecondByte.tArray &&
                PredefinedArrayTypes[(byte)streamTypeInfo.Type] != null
                    ? ReadSequence
                    : TypeReaders[(byte)streamTypeInfo.Type];

            reader(inputStream, builder);

            builder.Commit();
        }

        private static readonly Type[] PredefinedArrayTypes;
        internal static readonly int[] FixedBufferLengthByType;
        private static readonly DReadType[] TypeReaders;
        private static readonly DGetBuilderContext[] BuilderContextGetters;
        private static readonly ConstructorCache ConstructorCache;

        static LwBinaryDeserialize()
        {
            int maxTypesValue =
                typeof(Types)
                    .GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Select(fieldInfo => fieldInfo.GetValue(null))
                    .Where(o => o is Types)
                    .Cast<byte>()
                    .Max();

            PredefinedArrayTypes = new Type[maxTypesValue + 1];

            PredefinedArrayTypes[(byte)Types.tBool] = typeof(bool[]);
            PredefinedArrayTypes[(byte)Types.tByte] = typeof(byte[]);
            PredefinedArrayTypes[(byte)Types.tChar] = typeof(char[]);
            PredefinedArrayTypes[(byte)Types.tInt16] = typeof(short[]);
            PredefinedArrayTypes[(byte)Types.tInt32] = typeof(int[]);
            PredefinedArrayTypes[(byte)Types.tInt64] = typeof(long[]);
            PredefinedArrayTypes[(byte)Types.tUInt16] = typeof(ushort[]);
            PredefinedArrayTypes[(byte)Types.tUInt32] = typeof(uint[]);
            PredefinedArrayTypes[(byte)Types.tUInt64] = typeof(ulong[]);
            PredefinedArrayTypes[(byte)Types.tFloat] = typeof(float[]);
            PredefinedArrayTypes[(byte)Types.tDouble] = typeof(double[]);
            PredefinedArrayTypes[(byte)Types.tString] = typeof(string[]);
            PredefinedArrayTypes[(byte)Types.tGuid] = typeof(Guid[]);
            PredefinedArrayTypes[(byte)Types.tGuid2] = typeof(Guid[]);
            PredefinedArrayTypes[(byte)Types.tArrayObject] = typeof(object[]);

            FixedBufferLengthByType = new int[maxTypesValue + 1];

            FixedBufferLengthByType[(byte)Types.tBool] = sizeof(bool);
            FixedBufferLengthByType[(byte)Types.tByte] = sizeof(byte);
            FixedBufferLengthByType[(byte)Types.tChar] = sizeof(char);
            FixedBufferLengthByType[(byte)Types.tInt16] = sizeof(short);
            FixedBufferLengthByType[(byte)Types.tInt32] = sizeof(int);
            FixedBufferLengthByType[(byte)Types.tInt64] = sizeof(long);
            FixedBufferLengthByType[(byte)Types.tUInt16] = sizeof(ushort);
            FixedBufferLengthByType[(byte)Types.tUInt32] = sizeof(uint);
            FixedBufferLengthByType[(byte)Types.tUInt64] = sizeof(ulong);
            FixedBufferLengthByType[(byte)Types.tFloat] = sizeof(float);
            FixedBufferLengthByType[(byte)Types.tDouble] = sizeof(double);
            FixedBufferLengthByType[(byte)Types.tGuid2] = 16;

            TypeReaders = new DReadType[maxTypesValue + 1];

            TypeReaders[(byte)Types.tNull] = ReadNull;
            TypeReaders[(byte)Types.tBool] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tByte] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tChar] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tInt16] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tInt32] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tInt64] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tUInt16] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tUInt32] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tUInt64] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tFloat] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tDouble] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tString] = ReadLengthAndBuffer;
            TypeReaders[(byte)Types.tGuid] = ReadLengthAndBuffer;
            TypeReaders[(byte)Types.tGuid2] = ReadFixedLengthBuffer;
            TypeReaders[(byte)Types.tArray] = ReadSequence;
            TypeReaders[(byte)Types.tCollection] = ReadSequence;
            TypeReaders[(byte)Types.tArrayObject] = ReadSequence;
            TypeReaders[(byte)Types.tClass] = ReadVersionAndSequence;
            TypeReaders[(byte)Types.tStructEnum] = ReadVersionAndSequence;
            TypeReaders[(byte)Types.tDictionary] = ReadSequenceOfPairs;
            TypeReaders[(byte)Types.tKeyValuePair] = ReadPair;
            TypeReaders[(byte)Types.tException] = ReadException;

            BuilderContextGetters = new DGetBuilderContext[maxTypesValue + 1];

            BuilderContextGetters[(byte)Types.tNull] = NullBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tBool] = BooleanBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tByte] = ByteBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tChar] = CharBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tInt16] = Int16BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tInt32] = Int32BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tInt64] = Int64BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tUInt16] = UInt16BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tUInt32] = UInt32BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tUInt64] = UInt64BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tFloat] = SingleBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tDouble] = DoubleBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tString] = StringBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tGuid] = GuidBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tGuid2] = Guid2BuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tArray] = ArrayBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tCollection] = CollectionBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tArrayObject] = null;
            BuilderContextGetters[(byte)Types.tClass] = ClassStructEnumBuilderContext.GetInstanceForClass;
            BuilderContextGetters[(byte)Types.tStructEnum] = ClassStructEnumBuilderContext.GetInstanceForStructEnum;
            BuilderContextGetters[(byte)Types.tDictionary] = DictionaryBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tKeyValuePair] = KeyValuePairBuilderContext.GetInstance;
            BuilderContextGetters[(byte)Types.tException] = ExceptionBuilderContext.GetInstance;

            ConstructorCache = new ConstructorCache();
        }

        private readonly StringBuilderContext _stringBuilderContext =
            new StringBuilderContext();

        private readonly GuidBuilderContext _guidBuilderContext =
            new GuidBuilderContext();

        private readonly Guid2BuilderContext _guid2BuilderContext =
            new Guid2BuilderContext();

        protected LwBinaryDeserialize()
        {
            _serializationPropertiesStack.Push(
                new SerializationProperties());
        }

        protected TypeCache TypeCache;

        private static object CreateInstance(
            Type instanceType)
        {
            ConstructorInfo constructorInfo =
                ConstructorCache.GetValue(instanceType);

            return constructorInfo != null
                ? constructorInfo.Invoke(null) ?? Activator.CreateInstance(instanceType)
                : Activator.CreateInstance(instanceType);
        }

        /// <summary>
        /// Function for create intance
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="inputParams"></param>
        /// <returns></returns>
        private static object CreateInstance(
            Type instanceType,
            object[] inputParams)
        {
            ConstructorInfo constructorInfo =
                instanceType.GetConstructor(
                    inputParams
                        .Select(inputParam => inputParam.GetType())
                        .ToArray());

            if (constructorInfo != null)
            {
                object newInstance = constructorInfo.Invoke(inputParams);

                if (newInstance != null)
                    return newInstance;
            }

            throw new ArgumentException(
                string.Format(
                    "Can not create object of type '{0}'",
                    instanceType));
        }

        /// <summary>
        /// Function for check that class implement intarface with specific name
        /// </summary>
        /// <param name="typeClass"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        private static bool TypeImplementsInterface(
            Type typeClass,
            string interfaceName)
        {
            Type[] typesInterface = typeClass.GetInterfaces();

            return typesInterface.Any(t => t.Name == interfaceName);
        }

        private readonly Stack<SerializationProperties> _serializationPropertiesStack =
            new Stack<SerializationProperties>();

        private bool GetPropertiesForType(
            Type requestedType,
            out SerializationProperties serializationProperties)
        {
            serializationProperties = _serializationPropertiesStack.Peek();

            if (!GetPropertiesForType(
                    requestedType,
                    ref serializationProperties))
                return false;

            _serializationPropertiesStack.Push(serializationProperties);
            return true;
        }

        private void PopSerializationProperties()
        {
            _serializationPropertiesStack.Pop();
        }

        #region builder contexts

        internal abstract class ABuilderContext
        {
            protected static readonly Type ObjectType = typeof(object);
            protected static readonly Type NullableType = typeof(Nullable<>);

            public virtual int Length { set { } }

            public virtual void ChildCommited(object childResult)
            {
                throw
                    new NotSupportedException(
                        String.Format(
                            "No child builder contexts for {0}",
                            GetType()));
            }

            public abstract object Commit(byte[] buffer);

            public virtual ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                throw
                    new NotSupportedException(
                        String.Format(
                            "No child builder contexts for {0}",
                            GetType()));
            }
        }

        protected abstract class SimpleTypeBuilderContext<TBuilder, TValue> :
            ABuilderContext
            where TBuilder : SimpleTypeBuilderContext<TBuilder, TValue>
        {
            protected static readonly Type ContextType = typeof(TValue);
        }

        protected abstract class ValueTypeBuilderContext<TBuilder, TValue> :
            SimpleTypeBuilderContext<TBuilder, TValue>
            where TBuilder : ValueTypeBuilderContext<TBuilder, TValue>
        {
            protected static bool MatchesContextType(Type requestedType)
            {
                if (requestedType == ObjectType)
                    return true;

                if (requestedType.IsGenericType &&
                        requestedType.GetGenericTypeDefinition() == NullableType)
                    requestedType = requestedType.GetGenericArguments()[0];

                return requestedType == ContextType;
            }
        }

        protected abstract class PrimitiveTypeBuilderContext<TBuilder, TValue> :
            ValueTypeBuilderContext<TBuilder, TValue>
            where TBuilder : PrimitiveTypeBuilderContext<TBuilder, TValue>, new()
        {
            private static readonly TBuilder Instance = new TBuilder();

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                return
                    MatchesContextType(requestedType)
                        ? Instance
                        : null;
            }
        }

        private class NullBuilderContext :
            ABuilderContext
        {
            private static readonly NullBuilderContext Instance =
                new NullBuilderContext();

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                return Instance;
            }

            public override object Commit(byte[] buffer)
            {
                return null;
            }
        }

        private class BooleanBuilderContext :
            PrimitiveTypeBuilderContext<BooleanBuilderContext, bool>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToBoolean(buffer, 0);
            }
        }

        private class ByteBuilderContext :
            PrimitiveTypeBuilderContext<ByteBuilderContext, byte>
        {
            public override object Commit(byte[] buffer)
            {
                return buffer[0];
            }
        }

        private class CharBuilderContext :
            PrimitiveTypeBuilderContext<CharBuilderContext, char>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToChar(buffer, 0);
            }
        }

        private class Int16BuilderContext :
            PrimitiveTypeBuilderContext<Int16BuilderContext, short>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToInt16(buffer, 0);
            }
        }

        private class Int32BuilderContext :
            PrimitiveTypeBuilderContext<Int32BuilderContext, int>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToInt32(buffer, 0);
            }
        }

        private class Int64BuilderContext :
            PrimitiveTypeBuilderContext<Int64BuilderContext, long>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToInt64(buffer, 0);
            }
        }

        private class UInt16BuilderContext :
            PrimitiveTypeBuilderContext<UInt16BuilderContext, ushort>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToUInt16(buffer, 0);
            }
        }

        private class UInt32BuilderContext :
            PrimitiveTypeBuilderContext<UInt32BuilderContext, uint>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToUInt32(buffer, 0);
            }
        }

        private class UInt64BuilderContext :
            PrimitiveTypeBuilderContext<UInt64BuilderContext, ulong>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToUInt64(buffer, 0);
            }
        }

        private class SingleBuilderContext :
            PrimitiveTypeBuilderContext<SingleBuilderContext, float>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToSingle(buffer, 0);
            }
        }

        private class DoubleBuilderContext :
            PrimitiveTypeBuilderContext<DoubleBuilderContext, double>
        {
            public override object Commit(byte[] buffer)
            {
                return BitConverter.ToDouble(buffer, 0);
            }
        }

        private class StringBuilderContext :
            SimpleTypeBuilderContext<StringBuilderContext, string>
        {
            private int _length;

            public override int Length
            {
                set { _length = value; }
            }

            public override object Commit(byte[] buffer)
            {
                return Encoding.UTF8.GetString(buffer, 0, _length);
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                return
                    requestedType == ObjectType || requestedType == ContextType
                        ? lwBinaryDeserialize._stringBuilderContext
                        : null;
            }
        }

        private class GuidBuilderContext :
            ValueTypeBuilderContext<GuidBuilderContext, Guid>
        {
            private int _length;

            public override int Length
            {
                set { _length = value; }
            }

            public override object Commit(byte[] buffer)
            {
                return
                    new Guid(
                        Encoding.UTF8.GetString(
                            buffer,
                            0,
                            _length));
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                return MatchesContextType(requestedType)
                    ? lwBinaryDeserialize._guidBuilderContext
                    : null;
            }
        }

        private class Guid2BuilderContext :
            ValueTypeBuilderContext<Guid2BuilderContext, Guid>
        {
            private readonly byte[] _buffer = new byte[16];

            public override object Commit(byte[] buffer)
            {
                if (buffer.Length == 16)
                    return new Guid(buffer);

                Buffer.BlockCopy(buffer, 0, _buffer, 0, 16);

                return new Guid(_buffer);
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                return MatchesContextType(requestedType)
                    ? lwBinaryDeserialize._guid2BuilderContext
                    : null;
            }
        }

        protected abstract class CompoundBuilderContext :
            ABuilderContext
        {
            protected ABuilderContext GetChildForType(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (streamTypeInfo.TypeSecondByte ==
                    (byte)ETypesSecondByte.tArray)
                {
                    Type arrayType = PredefinedArrayTypes[(byte)streamTypeInfo.Type];

                    if (arrayType != null)
                        if (requestedType == ObjectType ||
                                requestedType == arrayType)
                            return
                                new ArrayBuilderContext(arrayType);
                }

                DGetBuilderContext builderContextGetter =
                    BuilderContextGetters[(byte)streamTypeInfo.Type];

                return
                    builderContextGetter(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        requestedType);
            }
        }

        private class ArrayBuilderContext : CompoundBuilderContext
        {
            private Array _result;
            private readonly Type _elementType;
            private int _currentIndex;

            public ArrayBuilderContext(
                Type requestedType)
            {
                _elementType = requestedType.GetElementType();
            }

            public override int Length
            {
                set
                {
                    _result = Array.CreateInstance(_elementType, value);
                }
            }

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                return
                    GetChildForType(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        _elementType);
            }

            public override void ChildCommited(object childResult)
            {
                _result.SetValue(
                    childResult,
                    _currentIndex++);
            }

            public override object Commit(byte[] buffer)
            {
                return _result;
            }

            // invoked if streamTypeInfo.Type == Types.tArray
            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                return requestedType.IsArray &&
                        streamTypeInfo.TypeSecondByte ==
                            GetTypeByteHashCode(requestedType)
                    ? new ArrayBuilderContext(requestedType)
                    : null;
            }
        }

        private class CollectionBuilderContext : CompoundBuilderContext
        {
            private readonly object _result;
            private readonly Type _elementType;

            private readonly MethodInfo _methodInfo;
            private readonly object[] _params = new object[1];

            private CollectionBuilderContext(
                Type requestedType)
            {
                _result = CreateInstance(requestedType);

                _elementType = requestedType.GetGenericArguments()[0];
                _methodInfo = requestedType.GetMethod("Add");
            }

            public override void ChildCommited(object childResult)
            {
                _params[0] = childResult;
                _methodInfo.Invoke(_result, _params);
            }

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                return
                    GetChildForType(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        _elementType);
            }

            public override object Commit(byte[] buffer)
            {
                return _result;
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (!requestedType.IsClass ||
                        !requestedType.IsGenericType ||
                        !TypeImplementsInterface(requestedType, "ICollection") ||
                        streamTypeInfo.TypeSecondByte != GetTypeByteHashCode(requestedType))
                    return null;

                return new CollectionBuilderContext(requestedType);
            }
        }

        private class DictionaryBuilderContext : CompoundBuilderContext
        {
            private readonly object _result;
            private readonly Type[] _genericArguments;

            private readonly MethodInfo _addMethodInfo;
            private readonly object[] _params = new object[2];

            private bool _expectingValue;

            private DictionaryBuilderContext(
                Type requestedType)
            {
                _result = CreateInstance(requestedType);

                _genericArguments = requestedType.GetGenericArguments();
                _addMethodInfo = requestedType.GetMethod("Add");
            }

            public override void ChildCommited(object childResult)
            {
                if (_expectingValue)
                {
                    _params[1] = childResult;
                    _addMethodInfo.Invoke(_result, _params);

                    _expectingValue = false;
                }
                else
                {
                    _params[0] = childResult;
                    _expectingValue = true;
                }
            }

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                return
                    GetChildForType(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        _genericArguments[_expectingValue ? 1 : 0]);
            }

            public override object Commit(byte[] buffer)
            {
                return _result;
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (!requestedType.IsClass ||
                        !requestedType.IsGenericType ||
                        !TypeImplementsInterface(requestedType, "IDictionary") ||
                        streamTypeInfo.TypeSecondByte != GetTypeByteHashCode(requestedType))
                    return null;

                return new DictionaryBuilderContext(requestedType);
            }
        }

        private class ClassStructEnumBuilderContext : CompoundBuilderContext
        {
            private readonly LwBinaryDeserialize _lwBinaryDeserialize;
            private readonly MemberInfo[] _membersInfo;
            private readonly bool _strict;
            private readonly object _result;

            private int _currentMemberIdx;

            private ClassStructEnumBuilderContext(
                LwBinaryDeserialize lwBinaryDeserialize,
                bool streamStrict,
                Type requestedType)
            {
                SerializationProperties serializationProperties;

                if (lwBinaryDeserialize.GetPropertiesForType(
                        requestedType,
                        out serializationProperties))
                    _lwBinaryDeserialize = lwBinaryDeserialize;

                _membersInfo =
                    TypeMembersInfoCache.GetValue(
                        new TypeMembersInfoCache.SerializedMembersKey(
                            Direction.Deserialization,
                            requestedType,
                            serializationProperties.Mode,
                            serializationProperties.DirectMemberType));

                _strict =
                    streamStrict ||
                    !IsAssignModeLenient(serializationProperties.AssignMode);

                lwBinaryDeserialize.VersionRequired =
                    requestedType ==
                        lwBinaryDeserialize.VersionedType;

                _result = CreateInstance(requestedType);
            }

            public override int Length
            {
                set
                {
                    if (_strict && _membersInfo.Length != value)
                        throw new LwSerializationException();
                }
            }

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                if (_strict)
                {
                    MemberInfo memberInfo = _membersInfo[_currentMemberIdx];

                    ABuilderContext childBuilderContext =
                        GetChildForType(
                            lwBinaryDeserialize,
                            streamTypeInfo,
                            ALwSerializable.GetTypeOfMember(memberInfo));

                    if (childBuilderContext == null)
                        throw new LwSerializationException();

                    return childBuilderContext;
                }

                for (int memberIdx = _currentMemberIdx;
                    memberIdx < _membersInfo.Length;
                    ++memberIdx)
                {
                    MemberInfo memberInfo = _membersInfo[memberIdx];

                    if (GetNameShortHashCode(memberInfo) !=
                            streamTypeInfo.NameHashCode)
                        continue;

                    ABuilderContext childBuilderContext =
                        GetChildForType(
                            lwBinaryDeserialize,
                            streamTypeInfo,
                            ALwSerializable.GetTypeOfMember(memberInfo));

                    if (childBuilderContext == null)
                        continue;

                    _currentMemberIdx = memberIdx;
                    return childBuilderContext;
                }

                return null;
            }

            public override void ChildCommited(object childResult)
            {
                ALwSerializable.SetValue(
                    _membersInfo[_currentMemberIdx++],
                    childResult,
                    _result);
            }

            public override object Commit(byte[] buffer)
            {
                if (_lwBinaryDeserialize != null)
                    _lwBinaryDeserialize.PopSerializationProperties();

                return _result;
            }

            public static ABuilderContext GetInstanceForClass(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (requestedType == ObjectType)
                    requestedType =
                        lwBinaryDeserialize.TypeCache
                            .GetClass(streamTypeInfo.TypeSecondByte);
                else
                {
                    if (!requestedType.IsClass)
                        return null;

                    if (streamTypeInfo.TypeSecondByte !=
                        GetTypeByteHashCode(requestedType))
                        return null;
                }

                return
                    new ClassStructEnumBuilderContext(
                        lwBinaryDeserialize,
                        !IsAssignModeLenient(streamTypeInfo.AssignMode),
                        requestedType);
            }

            public static ABuilderContext GetInstanceForStructEnum(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (requestedType == ObjectType)
                    requestedType =
                        lwBinaryDeserialize.TypeCache
                            .GetValueType(streamTypeInfo.TypeSecondByte);
                else
                {
                    if (requestedType.IsGenericType &&
                        requestedType.GetGenericTypeDefinition() == NullableType)
                    {
                        requestedType = requestedType.GetGenericArguments()[0];
                    }

                    if (requestedType.IsPrimitive ||
                        !requestedType.IsValueType ||
                        streamTypeInfo.TypeSecondByte !=
                        GetTypeByteHashCode(requestedType))
                    {
                        return null;
                    }
                }

                return
                    new ClassStructEnumBuilderContext(
                        lwBinaryDeserialize,
                        !IsAssignModeLenient(streamTypeInfo.AssignMode),
                        requestedType);
            }
        }

        private class KeyValuePairBuilderContext : CompoundBuilderContext
        {
            private static readonly Type KeyValuePairType =
                typeof(KeyValuePair<,>);

            private readonly object[] _params = new object[2];

            private readonly Type _requestedType;
            private readonly Type[] _genericArguments;

            private bool _expectingValue;

            private KeyValuePairBuilderContext(Type requestedType)
            {
                _requestedType = requestedType;
                _genericArguments = requestedType.GetGenericArguments();
            }

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                return
                    GetChildForType(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        _genericArguments[_expectingValue ? 1 : 0]);
            }

            public override void ChildCommited(object childResult)
            {
                _params[_expectingValue ? 1 : 0] = childResult;
                _expectingValue = !_expectingValue;
            }

            public override object Commit(byte[] buffer)
            {
                return CreateInstance(_requestedType, _params);
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (requestedType.IsGenericType &&
                        requestedType.GetGenericTypeDefinition() == NullableType)
                    requestedType = requestedType.GetGenericArguments()[0];

                return
                        requestedType.IsGenericType &&
                        requestedType.GetGenericTypeDefinition() == KeyValuePairType &&
                        streamTypeInfo.TypeSecondByte == GetTypeByteHashCode(requestedType)
                    ? new KeyValuePairBuilderContext(requestedType)
                    : null;
            }
        }

        private class ExceptionBuilderContext : CompoundBuilderContext
        {
            private static readonly Type[] ChildrenTypes =
            {
                typeof (string),
                typeof (string),
                typeof(Exception)
            };

            private static readonly Type ExceptionType = typeof(Exception);

            private int _phase;
            private readonly object[] _childrenResults = new object[3];

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                return
                    GetChildForType(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        ChildrenTypes[_phase]);
            }

            public override void ChildCommited(object childResult)
            {
                _childrenResults[_phase++] = childResult;
            }

            public override object Commit(byte[] buffer)
            {
                object[] params_ =
                    _childrenResults[2] != null
                        ? new[]
                        {
                            _childrenResults[1],
                            _childrenResults[2]
                        }
                        : new[] { _childrenResults[1] };

                return
                    CreateInstance(
                        Type.GetType((string)_childrenResults[0]) ?? typeof(Exception),
                        params_);
            }

            public static ABuilderContext GetInstance(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo,
                Type requestedType)
            {
                if (requestedType == ObjectType)
                    return new ExceptionBuilderContext();

                if (!requestedType.IsClass)
                    return null;

                if (!requestedType.IsSubclassOf(ExceptionType) &&
                        requestedType != ExceptionType)
                    return null;

                return new ExceptionBuilderContext();
            }
        }

        #endregion

        protected readonly Stack<ABuilderContext> BuilderContextStack =
            new Stack<ABuilderContext>();

        private byte _currentType;

        public IDeserializationBuilder SetupStreamTypeInfo(
            StreamTypeInfo streamTypeInfo)
        {
            _currentType = (byte)streamTypeInfo.Type;

            ABuilderContext builderContext =
                BuilderContextStack
                    .Peek()
                    .GetChild(
                        this,
                        streamTypeInfo);

            if (builderContext == null)
                return new LwBinaryDeserializeSkipper(this, streamTypeInfo);

            BuilderContextStack.Push(builderContext);
            return this;
        }

        private byte[] _buffer = new byte[4];
        private Version _versionedTypeVersion;

        public byte[] GetBuffer(int size)
        {
            if (size > _buffer.Length)
                _buffer = new byte[size];

            return _buffer;
        }

        public byte[] GetLengthAndBuffer(out int length)
        {
            length = BitConverter.ToInt32(_buffer, 0);

            BuilderContextStack.Peek().Length = length;

            return GetBuffer(length);
        }

        public int GetLength()
        {
            int length = BitConverter.ToInt32(_buffer, 0);

            BuilderContextStack.Peek().Length = length;

            return length;
        }

        public byte[] GetFixedLengthBuffer(out int length)
        {
            length = FixedBufferLengthByType[_currentType];
            return GetBuffer(length);
        }

        public void Commit()
        {
            ABuilderContext builderContext =
                BuilderContextStack.Pop();

            BuilderContextStack
                .Peek()
                .ChildCommited(builderContext.Commit(_buffer));
        }

        public bool VersionRequired
        {
            get;
            private set;
        }

        protected abstract Type VersionedType { get; }

        private Version VersionedTypeVersion
        {
            get
            {
                return
                    _versionedTypeVersion ??
                        (_versionedTypeVersion =
                            ALwSerializable.GetVersion(VersionedType));
            }
        }

        public Version Version
        {
            set
            {
                FileVersion = value;

                IsSameVersion = FileVersion.Equals(VersionedTypeVersion);
            }
        }
    }

    internal class LwBinaryDeserialize<T> :
        LwBinaryDeserialize,
        IDisposable
    {
        private class TopMostBuilderContext : CompoundBuilderContext
        {
            public T Result { get; private set; }

            public override void ChildCommited(object childResult)
            {
                Result = (T)childResult;
            }

            public override object Commit(byte[] buffer)
            {
                return null;
            }

            public override ABuilderContext GetChild(
                LwBinaryDeserialize lwBinaryDeserialize,
                StreamTypeInfo streamTypeInfo)
            {
                return
                    GetChildForType(
                        lwBinaryDeserialize,
                        streamTypeInfo,
                        typeof(T));
            }
        }

        private readonly TopMostBuilderContext _topMostBuilderContext;

        public LwBinaryDeserialize()
        {
            _topMostBuilderContext = new TopMostBuilderContext();
            BuilderContextStack.Push(_topMostBuilderContext);
        }

        /// <summary>
        /// Function for deserialization
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="typeCache"></param>
        /// <returns></returns>
        public T Deserialize(Stream inputStream, TypeCache typeCache)
        {
            TypeCache = typeCache;

            DoDeserialize(
                inputStream,
                this);

            TypeCache = null;

            // Return new instance after deserialization
            return _topMostBuilderContext.Result;
        }

        protected override Type VersionedType
        {
            get { return typeof(T); }
        }

        public void Dispose()
        {
        }
    }
}
