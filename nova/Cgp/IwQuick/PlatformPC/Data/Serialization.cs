using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// helper class for binary serialization
    /// </summary>
    public class Serialization
    {
        public static int GetThresholdVersion(
            [NotNull] Object serializedObject,
            [NotNull] Type attributeType)
        {
            Validator.CheckForNull(serializedObject,"serializedObject");
            Validator.CheckForNull(attributeType,"attributeType");

            int iVersion = 1;
            GetObjectFields(false, serializedObject, attributeType, ref iVersion);
            return iVersion;
        }

        private static List<FieldInfo> GetObjectFields(
            bool isLoad,
            [NotNull] Object serializedObject, 
            Type attributeType, 
            ref int io_iThresholdVersion)
        {
            var aRetList = new List<FieldInfo>();

            FieldInfo[] arFields = serializedObject.GetType().GetFields(BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            if (null != attributeType)// && io_iThresholdVersion > 0)
            {
                Validator.CheckInvalidOperation(
                    !typeof(IVersionedAttribute).IsAssignableFrom(attributeType)
                    );
            }

            foreach (FieldInfo aFieldInfo in arFields)
            {
                // Skip this field if it is marked NonSerialized.
                if (Attribute.IsDefined(aFieldInfo, typeof(NonSerializedAttribute)))
                    continue;

                if (null != attributeType)
                {
                    object[] arCustomAttributes = aFieldInfo.GetCustomAttributes(attributeType, false);
                    if (null == arCustomAttributes ||
                        0 == arCustomAttributes.Length)
                        continue;

                    
                    IVersionedAttribute aVersionedAttribute = (IVersionedAttribute)arCustomAttributes[0];
                    if (isLoad)
                    {

                        if (io_iThresholdVersion > 0 &&
                            io_iThresholdVersion < aVersionedAttribute.Version)
                        {
#if DEBUG
                            UI.ExtendedConsole.Warning(
                                "Property \"" + serializedObject.GetType() + "." + aFieldInfo.Name + "(" + aVersionedAttribute.Version + ")\"" +
                                " has higher version than threshold " + io_iThresholdVersion +
                                "\nVersion will be increased by serialization automatically"
                                );

#endif
                            continue;
                        }
                    }
                    else
                    {
                        if (aVersionedAttribute.Version > io_iThresholdVersion)
                            io_iThresholdVersion = aVersionedAttribute.Version;
                    }
                    
                }

                

                aRetList.Add(aFieldInfo);
            }

            return aRetList;
        }

        private static void SerializeDeserializeFields(
            bool isLoad, 
            [NotNull] Object serializedObject, 
            Type attributeType, 
            int thresholdVersion,
            [NotNull] System.IO.Stream dataStream,
            [NotNull] System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter
            )
        {
            Validator.CheckForNull(serializedObject,"serializedObject");
            Validator.CheckForNull(dataStream,"dataStream");
            Validator.CheckForNull(binaryFormatter,"binaryFormatter");

            List<FieldInfo> aFields = GetObjectFields(isLoad, serializedObject, attributeType, ref thresholdVersion);

            foreach(FieldInfo aFieldInfo in aFields) 
            {
                // Get the value of this field from the SerializationInfo object.
                if (isLoad)
                {
                    object aValue = binaryFormatter.Deserialize(dataStream);
                    if (aValue is byte &&
                        (byte)aValue == 0)
                        aFieldInfo.SetValue(serializedObject, null);
                    else
                        aFieldInfo.SetValue(serializedObject, aValue);
                }
                else
                {
                    object aValue = aFieldInfo.GetValue(serializedObject);

                    if (null != aValue)
                        binaryFormatter.Serialize(dataStream, aValue);
                    else
                        binaryFormatter.Serialize(dataStream, (byte)0);
                }
            }
        }

        private static void SerializeDeserializeFields(
            bool isLoad,
            [NotNull] Object serializedObject,
            Type attributeType,
            int thresholdVersion,
            [NotNull] SerializationInfo serialInfo,
            [UsedImplicitly] StreamingContext context)
        {
            Validator.CheckForNull(serializedObject,"serializedObject");

            Validator.CheckForNull(serialInfo,"serialInfo");

            List<FieldInfo> aFields = GetObjectFields(isLoad, serializedObject, attributeType, ref thresholdVersion);

            foreach (FieldInfo aFieldInfo in aFields)
            {
                // Get the value of this field from the SerializationInfo object.
                if (isLoad)
                    aFieldInfo.SetValue(serializedObject, serialInfo.GetValue(aFieldInfo.Name, aFieldInfo.FieldType));
                else
                    serialInfo.AddValue(aFieldInfo.Name, aFieldInfo.GetValue(serializedObject));
            }
        }

        public static void SerializeFields(Object serializedObject, Type attributeType, SerializationInfo serialInfo, StreamingContext context)
        {
            SerializeDeserializeFields(false,  serializedObject, attributeType, 0, serialInfo, context);
        }

        public static void DeserializeFields(Object  serializedObject, Type attributeType, SerializationInfo serialInfo, StreamingContext context)
        {
            SerializeDeserializeFields(true,  serializedObject, attributeType, 0, serialInfo, context);
        }

        // no filtering
        public static void SerializeFields(Object  serializedObject, SerializationInfo serialInfo, StreamingContext context)
        {
            SerializeDeserializeFields(false,  serializedObject, null,0, serialInfo, context);
        }

        public static void DeserializeFields(Object  serializedObject, SerializationInfo serialInfo, StreamingContext context)
        {
            SerializeDeserializeFields(true,  serializedObject, null, 0, serialInfo, context);
        }

        // filtering with version also

        public static void DeserializeFields(Object  serializedObject, Type attributeType, int i_iThresholdVersion, SerializationInfo serialInfo, StreamingContext context)
        {
            SerializeDeserializeFields(true,  serializedObject, attributeType, i_iThresholdVersion, serialInfo, context);
        }

        // direct by binary formatter

        public static void SerializeFields(Object  serializedObject, Type attributeType, 
            System.IO.Stream dataStream,
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter)
        {
            SerializeDeserializeFields(false,  serializedObject, attributeType, 0, dataStream, binaryFormatter);
        }

        public static void DeserializeFields(Object  serializedObject, Type attributeType,
            System.IO.Stream dataStream,
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter)
        {
            SerializeDeserializeFields(true,  serializedObject, attributeType, 0, dataStream, binaryFormatter);
        }

        // filtering with version also
        public static void DeserializeFields(Object  serializedObject, Type attributeType, int i_iThresholdVersion,
            System.IO.Stream dataStream,
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter)
        {
            SerializeDeserializeFields(true, serializedObject, attributeType, i_iThresholdVersion, dataStream, binaryFormatter);
        }
    }
}
