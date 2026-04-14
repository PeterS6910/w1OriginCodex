using System;
using System.Collections.Generic;
using Contal.IwQuick;
using JetBrains.Annotations;
#if COMPACT_FRAMEWORK
using System.Reflection;


#else
using System.Linq;
#endif

namespace  Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumHelper
    {
        private const string EnumTypeException = " is not an enumerated type";

        /// <summary>
        /// Enum.Parse variant
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum GetValue<TEnum>([NotNull] string name) 
            where TEnum : struct, IConvertible
        {
            return GetValue(name, default(TEnum));
        }

        /// <summary>
        /// Enum.Parse variant
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultForParsingFailure"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException">if name is null</exception>
        public static TEnum GetValue<TEnum>(
            [NotNull] string name, 
            TEnum defaultForParsingFailure
            ) 
            where TEnum : struct, IConvertible
        {
            Validator.CheckForNull(name,"name");

            var enumType = ValidateTypeToBeEnum<TEnum>();

#if !COMPACT_FRAMEWORK
            return (TEnum)Enum.Parse(enumType, name, true);
#else
            FieldInfo[] fieldsInfo = enumType.GetFields();
            if (fieldsInfo != null)
            {
                foreach (FieldInfo fieldInfo in fieldsInfo)
                {
                    if (fieldInfo.FieldType == enumType
                        && fieldInfo.Name == name)
                        return (TEnum)fieldInfo.GetValue(fieldInfo);
                }
            }

            return defaultForParsingFailure;
#endif
        }

        /// <summary>
        /// List all values in enum
        /// </summary>
        /// <typeparam name="TEnum">Enum to process</typeparam>
        /// <returns>Return List of TEnum</returns>
        public static IList<TEnum> ListAllValues<TEnum>() 
            where TEnum : struct, IConvertible
        {
            var enumType = ValidateTypeToBeEnum<TEnum>();

#if !COMPACT_FRAMEWORK
            return new List<TEnum>(Enum.GetValues(enumType).Cast<TEnum>());
#else
            List<TEnum> result = new List<TEnum>();

            FieldInfo[] fieldsInfo = enumType.GetFields();
            if (fieldsInfo != null)
            {
                foreach (FieldInfo fieldInfo in fieldsInfo)
                {
                    if (fieldInfo.FieldType == enumType)
                        result.Add((TEnum)fieldInfo.GetValue(fieldInfo));
                }
            }

            return result;
#endif
        }



        /// <summary>
        /// List values in enum which match condition in enumValueLambda
        /// </summary>
        /// <typeparam name="TEnum">Enum to process</typeparam>
        /// <param name="enumValueLambda">Selecting lambda</param>
        /// <returns>Return List of TEnum</returns>
        /// <exception cref="ArgumentException">if enumValueLambda is null</exception>
        public static IList<TEnum> ListAllValues<TEnum>([NotNull] Func<TEnum, bool> enumValueLambda) 
            where TEnum : struct, IConvertible
        {
            Validator.CheckForNull(enumValueLambda,"enumValueLambda");

            var type = ValidateTypeToBeEnum<TEnum>();

#if !COMPACT_FRAMEWORK
            return new List<TEnum>(Enum.GetValues(type).Cast<TEnum>().Where(enumValueLambda));
#else
            List<TEnum> result = new List<TEnum>();

            FieldInfo[] fieldsInfo = type.GetFields();
            if (fieldsInfo != null)
            {
                foreach (FieldInfo fieldInfo in fieldsInfo)
                {
                    if (fieldInfo.FieldType == type)
                    {
                        TEnum value = (TEnum)fieldInfo.GetValue(fieldInfo);
                        if (enumValueLambda(value))
                            result.Add(value);
                    }
                }
            }

            return result;
#endif
        }

        /// <summary>
        /// iterates through the members of the enumeration executing processEnumValueLambda over them
        /// </summary>
        /// <typeparam name="TEnum">Enum to process</typeparam>
        /// <param name="processEnumValueLambda">processing lambda</param>
        /// <exception cref="ArgumentNullException">if processEnumValueLambda is null</exception>
        public static void IterateAllValues<TEnum>([NotNull] Action<TEnum> processEnumValueLambda) 
            where TEnum : struct, IConvertible
        {
            Validator.CheckForNull(processEnumValueLambda,"processEnumValueLambda");

            var type = ValidateTypeToBeEnum<TEnum>();

            var fieldsInfo = type.GetFields();
            if (fieldsInfo == null) 
                return;

            foreach (var fieldInfo in fieldsInfo)
            {
                if (fieldInfo.FieldType == type)
                {
                    var value = (TEnum)fieldInfo.GetValue(fieldInfo);
                    processEnumValueLambda(value);

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        private static Type ValidateTypeToBeEnum<TEnum>() 
            where TEnum : struct, IConvertible
        {
            var type = typeof (TEnum);
            if (!type.IsEnum)
                throw new ArgumentException(type + EnumTypeException);
            return type;
        }

        /// <summary>
        /// List values in enum. Call processing lambda for output item creation. If created item is null it will non be added to output list.
        /// </summary>
        /// <typeparam name="TEnum">Enum to process</typeparam>
        /// <typeparam name="TOut">Output list type</typeparam>
        /// <param name="enumValueLambda">Enum processing lambda</param>
        /// <returns>Return List of TOut</returns>
        /// <exception cref="ArgumentNullException">if enumValueLambda is null</exception>
        public static IList<TOut> ListAllValuesWithProcessing<TEnum, TOut>([NotNull] Func<TEnum, TOut> enumValueLambda) 
            where TEnum : struct, IConvertible
        {
            Validator.CheckForNull(enumValueLambda,"enumValueLambda");

            var enumType = ValidateTypeToBeEnum<TEnum>();

            List<TOut> result = new List<TOut>();

#if !COMPACT_FRAMEWORK
            foreach (var value in Enum.GetValues(enumType).Cast<TEnum>())
            {
                TOut temp = enumValueLambda(value);
                if(!ReferenceEquals(temp,null))
                    result.Add(temp);
            }
#else
            FieldInfo[] fieldsInfo = enumType.GetFields();
            if (fieldsInfo != null)
            {
                foreach (FieldInfo fieldInfo in fieldsInfo)
                {
                    if (fieldInfo.FieldType == enumType)
                    {
                        TOut temp = enumValueLambda((TEnum)fieldInfo.GetValue(fieldInfo));
                        if(!ReferenceEquals(temp,null))
                            result.Add(temp);
                    }
                }
            }
#endif
            return result;
        }
    }
}
