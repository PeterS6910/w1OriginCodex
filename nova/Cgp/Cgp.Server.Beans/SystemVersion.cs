using System;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class SystemVersion
    {
        public const string VERSION = "0";
        public const string COLUMNIDVERSION = "IdVersion";
        public const string COLUMNVERSION = "Version";
        public const string COLUMNDBSNAME = "DbsName";

        public virtual Guid IdVersion { get; set; }
        public virtual string DbsName { get; set; }
        public virtual string Version { get; set; }
     
        public override string ToString()
        {
            return Version;
        }

        public virtual bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is SystemVersion)
            {
                return (obj as SystemVersion).IdVersion == IdVersion;
            }
            else
            {
                return false;
            }
        }       
    }
}
