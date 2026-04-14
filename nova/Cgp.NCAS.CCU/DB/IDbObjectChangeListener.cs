using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    public interface IDbObjectChangeListener<TDbObject>
    {
        void PrepareObjectUpdate(
            Guid idObject,
            TDbObject newObject);

        void OnObjectSaved(
            Guid idObjec,
            TDbObject newObject);

        void PrepareObjectDelete(Guid idObject);
        
    }

    public interface IDbObjectRemovalListener
    {
        void PrepareObjectDelete(
            Guid idObject,
            ObjectType objectType);
    }

}