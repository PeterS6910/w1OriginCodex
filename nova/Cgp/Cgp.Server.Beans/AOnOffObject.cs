using System;
using System.Reflection;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.Beans
{
    /// <summary>
    /// Class for all on off object types in database
    /// </summary>
    [Serializable()]
    public abstract class AOnOffObject : AOrmObjectWithVersion
    {
        /// <summary>
        /// Get state of object
        /// </summary>
        public abstract bool State { get; }

        /// <summary>
        /// Get type as string and id to save on off object to database
        /// </summary>
        /// <param name="type">Returns type of object in string</param>
        /// <param name="id">Returns object id</param>
        public virtual void SaveToDatabase(out string type, out object id)
        {
            type = Assembly.CreateQualifiedName(this.GetType().Assembly.GetName().Name, this.GetType().FullName);
            id = GetId();
        }

        /// <summary>
        /// Get type as ObjectType and id to save on off object to database
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public virtual void SaveToDatabase(out ObjectType type, out object id)
        {
            type = this.GetObjectType();
            id = GetId();
        }

        /// <summary>
        /// Gets type from string
        /// </summary>
        /// <param name="strtype">Type of object in string from function SaveToDatabase</param>
        /// <returns>Returns type</returns>
        public static Type GetOnOffObjectType(string strtype)
        {
            Type type = Type.GetType(strtype);
            if (type != null && type.IsSubclassOf(typeof(AOnOffObject)))
                return type;
            else
                return null;
        }
    }
}
