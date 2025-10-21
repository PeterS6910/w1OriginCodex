using System;

namespace Contal.IwQuick
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetNamingForTypeof(this Type type)
        {
            if (ReferenceEquals(type, null))
                return string.Empty;

            var friendlyName = type.FullName;
            if (!type.IsGenericType)
                return friendlyName;

            var genericParamCount = type.GetGenericArguments().Length;

            string tmp = string.Empty;
            for (int i = 0; i < genericParamCount-1;i++)
                tmp += ",";

            friendlyName = 
                friendlyName.Replace("`" + genericParamCount, "<" + tmp + ">");

            // done this way, as CF does not know property IsNested
            if (type.IsNestedPublic ||
                type.IsNestedPrivate ||
                type.IsNestedFamily ||
                type.IsNestedAssembly ||
                type.IsNestedFamANDAssem ||
                type.IsNestedFamORAssem)
                friendlyName = friendlyName.Replace('+', '.');



            return friendlyName;
        }
    }
}
