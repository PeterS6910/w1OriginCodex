using System;

namespace Contal.Cgp.Server.Beans
{
    public class MaximumVersionForObjectType
    {
        public virtual Guid IdMaximumVersionForObjectType { get; set; }
        public virtual int ObjectType { get; set; }
        public virtual int Version { get; set; }

        public MaximumVersionForObjectType()
        {
        }
    }
}
