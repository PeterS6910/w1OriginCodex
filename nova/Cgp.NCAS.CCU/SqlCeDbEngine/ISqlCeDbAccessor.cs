using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public interface ISqlCeDbAccessor : ISqlCeDbAccessorBase
    {
        ICollection<Guid> GetPrimaryKeys();

        bool ContainsAnyObjects();

        object GetFromDatabase(Guid idObject);

        bool Exists(Guid idObject);

        void SaveToDatabase(IEnumerable<DB.IDbObject> newObjects, bool executePrepareDeleteOrUpdate);

        void DeleteFromDatabase(Guid idObject);

        MaximumVersionAndIds GetMaximumVersionAndIds();

        void SaveVersion(int version);

        ICollection<Guid> GetIdsOfRecentlySavedObjects();

        void OnApplyChangesDone();
    }
}
