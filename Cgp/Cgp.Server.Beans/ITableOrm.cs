using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    public interface ITableORM
    {
        AOrmObject GetObjectParent(AOrmObject ormObject);

        AOrmObject GetObjectById(object objectId);

        object ParseId(string strObjectId);

        IList<AOrmObject> GetReferencedObjectsAllPlugins(object idObj);
        IEnumerable<AOrmObject> GetDirectReferences(object idObj);

        IEnumerable<AOrmObject> List();

        bool HasAccessViewForObject(AOrmObject ormObject, Login login);
        bool HasAccessView();
        bool HasAccessView(Login login);

        string GetPluginName();
        bool CanCreateObject();

        ICollection<AOrmObject> GetSearchList(
            string name,
            bool single);

        ICollection<AOrmObject> FulltextSearch(string name);
        ICollection<AOrmObject> ParametricSearch(string name);

        IEnumerable<IModifyObject> GetIModifyObjects();

        bool ImplementsSearchFunctions();
    }

    /// <summary>
    /// Interface for table ORM
    /// </summary>
    /// <typeparam name="T">Type of ORM object</typeparam>
    public interface ITableORM<T> : ITableORM
    {
        /// <summary>
        /// Inserts object to table
        /// </summary>
        /// <param name="ormObject">Object to insert</param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        bool Insert(ref T ormObject);

        /// <summary>
        /// Updates object in table
        /// </summary>
        /// <param name="ormObject">Object to update</param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        bool Update(T ormObject);

        /// <summary>
        /// Deletes object from table
        /// </summary>
        /// <param name="ormObject">Object to delete</param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        bool Delete(T ormObject);

        /// <summary>
        /// Gets object from table by id
        /// </summary>
        /// <param name="id">Object id</param>
        /// <returns>Returns object from table</returns>
        T GetById(object id);

        /// <summary>
        /// Selects all objects from table
        /// </summary>
        /// <returns>Returns collection of objects</returns>
        ICollection<T> List();
    }
}
