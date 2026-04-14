using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;

namespace Contal.Cgp.Server.Beans
{
    public interface IBaseOrmTable<T>
        where T : AOrmObject
    {
        T GetObjectById(object id);
        bool Insert(ref T ormObject, out Exception error);
        bool Update(T ormObject, out Exception error);
        bool UpdateOnlyInDatabase(T ormObject, out Exception error);
        bool Delete(T ormObject, out Exception error);
        bool DeleteById(object id, out Exception error);
        T GetObjectForEdit(object id, out Exception error);
        void RenewObjectForEdit(object id, out Exception error);
        ICollection<T> List(out Exception error);

        ICollection<T> SelectByCriteria(
            ICollection<FilterSettings> filterSettings, 
            out Exception error);

        ICollection<T> SelectByCriteria(
            out Exception error, 
            LogicalOperators filterJoinOperator, 
            params ICollection<FilterSettings>[] filterSettings);

        void EditEnd(T ormObject);
        bool HasAccessView();
        bool HasAccessViewForObject(T ormObject);
        bool HasAccessInsert();
        bool HasAccessUpdate();
        bool HasAccessDelete();
        IList<AOrmObject> GetReferencedObjects(object idObj, List<string> plugins);
        T GetObjectForEditById(object id, out bool editAllowed);
    }
}