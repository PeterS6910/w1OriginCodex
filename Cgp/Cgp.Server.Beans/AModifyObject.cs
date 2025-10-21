using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public abstract class AModifyObject : IModifyObject
    {
        public object Id { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string[] ObjectSubTypes { get; set; }

        public abstract ObjectType GetOrmObjectType { get; }
        public Guid GetId { get { return (Guid)Id; } }

        public override string ToString()
        {
            return FullName;
        }

        public virtual bool Contains(string expression)
        {
            if (AOrmObject.RemoveDiacritism(FullName).ToLower().Contains(AOrmObject.RemoveDiacritism(expression).ToLower()))
                return true;

            return 
                Description != null &&
                AOrmObject.RemoveDiacritism(Description).ToLower().Contains(AOrmObject.RemoveDiacritism(expression).ToLower());
        }

        public virtual string GetObjectSubType(byte option)
        {
            return GetOrmObjectType.ToString();
        }
    }
}
