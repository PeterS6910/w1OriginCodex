using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.ORM
{
    /// <summary>
    /// Interface for table ORM
    /// </summary>
    /// <typeparam name="T">Type of ORM object</typeparam>
    public interface ITableORM<T>
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
