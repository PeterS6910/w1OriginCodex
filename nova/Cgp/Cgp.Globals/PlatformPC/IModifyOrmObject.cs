using System;

namespace Contal.Cgp.Globals
{
    public interface IModifyObject
    {
        string FullName { get; set; }
        bool Contains(string expression);
        ObjectType GetOrmObjectType { get; }        
        string GetObjectSubType(byte option);
        Guid GetId { get; }
    }
}

