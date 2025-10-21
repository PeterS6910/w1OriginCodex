using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Contal.IwQuick;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;

namespace Contal.Cgp.ORM
{
    /// <summary>
    /// abstract parent for all ORM classes to be used
    /// </summary>
    public abstract class ATableORM<TSingleton> : AMbrSingleton<TSingleton>
        where TSingleton : ATableORM<TSingleton>
    {
        private Assembly _thisAssembly = Assembly.GetExecutingAssembly();

        private Exception _error;

        protected ATableORM(TSingleton dummyParameter)
            : base(dummyParameter)
        {
        }

        /// <summary>
        /// returns last error of during transactional operations
        /// </summary>
        public Exception LastError
        {
            get { return _error; }
        }

        /// <summary>
        /// Get or set assembly
        /// </summary>
        public Assembly ThisAssembly
        {
            get { return _thisAssembly; }
            set { _thisAssembly = value; }
        }

        /// <summary>
        /// flushes the session
        /// </summary>
        protected virtual void SessionFlush()
        {
            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            session.Flush();
            //session.Close();
        }

        /// <summary>
        /// deletes obtained object from dbs
        /// </summary>
        /// <param name="ormObject">the class to delete</param>
        /// <param name="session"></param>
        /// <exception cref="ArgumentNullException">if the object is null</exception>
        private void DeleteCore(
            [NotNull] object ormObject,
            ISession session)
        {
            Validator.CheckForNull(ormObject,"ormObject");

            if (ReferenceEquals(session,null))
                session = NhHelper.Singleton.GetSession(_thisAssembly);

            DeleteSubObjects(ormObject, session);

            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                session.Delete(ormObject);
                transaction.Commit();
                _error = null;
            }
            catch (Exception error)
            {
                SafeRollback(error, transaction);
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        protected virtual void DeleteSubObjects(object ormObject, ISession session)
        {
        }

        /// <summary>
        /// deletes obtained object from dbs
        /// </summary>
        /// <param name="ormObject">the class to delete</param>
        /// <returns>returns true, if the transaction succeeded</returns>
        protected bool Delete(object ormObject)
        {
            return Delete(ormObject, null);
        }

        /// <summary>
        /// deletes obtained object from dbs
        /// </summary>
        /// <param name="ormObject">the class to delete</param>
        /// <param name="session"></param>
        /// <returns>returns true, if the transaction succeeded</returns>
        protected bool Delete(
            object ormObject,
            ISession session)
        {
            try
            {
                DeleteCore(ormObject, session);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// deletes obtained object from dbs
        /// </summary>
        /// <param name="ormObject">the class to delete</param>
        /// <param name="session"></param>
        /// <param name="deleteException">Returns the exception if the transaction failed else null</param>
        /// <returns>returns true, if the transaction succeeded</returns>
        protected bool Delete(
            object ormObject, 
            ISession session, 
            out Exception deleteException)
        {
            try
            {
                DeleteCore(ormObject, session);
                deleteException = null;
                return true;
            }
            catch (Exception ex)
            {
                deleteException = RetypeException(ex);
                return false;
            }
        }

        /// <summary>
        /// Retype the excepcions
        /// </summary>
        /// <param name="originalException">Original exception</param>
        /// <returns>Return the retyped exception or originalException</returns>
        private Exception RetypeException(Exception originalException)
        {
            if (ReferenceEquals(originalException,null))
                return null;

            try
            {
                var innerExceptionMessage = originalException.InnerException.Message;
                var innerExceptionMessageUpper = innerExceptionMessage.ToUpper();

                if (innerExceptionMessageUpper.IndexOf("DUPLICATE KEY", StringComparison.Ordinal) > 0 || 
                    innerExceptionMessageUpper.IndexOf(" UNIQUE KEY", StringComparison.Ordinal) > 0)
                {
                    string message = string.Empty;
                    string constrainName = string.Empty;

                    int startPos = innerExceptionMessage.IndexOf("constraint '", StringComparison.Ordinal);
                    if (startPos > 0)
                    {
                        startPos += 12;
                        int endPos = innerExceptionMessage.IndexOf("'", startPos, StringComparison.Ordinal);
                        if (endPos > 0)
                            constrainName = innerExceptionMessage.Substring(startPos, endPos - startPos);
                    }

                    if (constrainName != string.Empty)
                    {
                        var session = NhHelper.Singleton.GetSession(_thisAssembly);

                        try
                        {
                            string sqlQueryString = "select sys.columns.name from sys.index_columns, sys.indexes, sys.columns where sys.index_columns.object_id = sys.indexes.object_id and sys.columns.object_id = sys.index_columns.object_id and sys.index_columns.index_id = sys.indexes.index_id and sys.columns.column_id = sys.index_columns.column_id and sys.indexes.name = '" + constrainName + "'";

                            var query = session.CreateSQLQuery(sqlQueryString);
                            var listColumnsName = query.List<string>();

                            foreach (string columnName in listColumnsName)
                            {
                                if (message != string.Empty)
                                    message += ",";

                                message += columnName;
                            }
                        }
                        catch { }
                        {
                            //session.Close();
                        }
                    }

                    var uniqueError = new SqlUniqueException(message);
                    return uniqueError;
                }

                if (innerExceptionMessageUpper.IndexOf("REFERENCE CONSTRAINT", StringComparison.Ordinal) > 0)
                {
                    var constraintError = new SqlDeleteReferenceConstraintException();
                    return constraintError;
                }
            }
            catch
            {
            }

            return originalException;
        }

        /// <summary>
        /// Saves obtained object to the dbs; returns true, if the transaction succeeded
        /// </summary>
        /// <param name="ormObject">the class to save</param>
        /// <param name="objectId"></param>
        /// <exception cref="ArgumentNullException">if the object is null</exception>
        private void InsertCore(
            [NotNull] object ormObject, 
            object objectId)
        {
            Validator.CheckForNull(ormObject,"ormObject");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly); //(_asmHbm);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                
                if (ReferenceEquals(objectId , null))
                    session.Save(ormObject);
                else
                    session.Save(ormObject, objectId);

                transaction.Commit();
                _error = null;
                //session.Close();
            }
            catch (Exception error)
            {
                SafeRollback(error, transaction);
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        private void InsertCoreObjectsArray([NotNull] object[] ormObjects)
        {
            Validator.CheckForNull(ormObjects,"ormObjects");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly); //(_asmHbm);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();

                foreach (object ormObject in ormObjects)
                    session.Save(ormObject);

                transaction.Commit();
                _error = null;
                //session.Close();
            }
            catch (Exception error)
            {
                SafeRollback(error, transaction);
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Saves obtained object to the dbs
        /// </summary>
        /// <param name="ormObject">the class to save</param>
        /// <param name="objectId"></param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        protected virtual bool Insert(
            [NotNull] object ormObject, 
            object objectId)
        {
            try
            {
                InsertCore(ormObject, objectId);
                return true;
            }
            catch //(Exception e)
            {
                return false;
            }
        }

        protected virtual bool InsertObjectsArray(object[] ormObjects)
        {
            try
            {
                InsertCoreObjectsArray(ormObjects);
                return true;
            }
            catch //(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Saves obtained object to the dbs
        /// </summary>
        /// <param name="ormObject">the class to save</param>
        /// <param name="insertException">Returns the exception if the transaction failed else null</param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        protected virtual bool Insert(object ormObject, out Exception insertException)
        {
            try
            {
                InsertCore(ormObject, null);
                insertException = null;
                return true;
            }
            catch (Exception ex)
            {
                insertException = RetypeException(ex);
                return false;
            }
        }

        /// <summary>
        /// updates the ORM object; returns true, if the transaction succeeded
        /// </summary>
        /// <param name="ormObject">Class to update</param>.
        /// <exception cref="ArgumentNullException">if the object is null</exception>
        private void UpdateCore([NotNull] object ormObject)
        {
            Validator.CheckForNull(ormObject,"ormObject");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly); //(_asmHbm);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                try
                {
                    session.Update(ormObject);
                    transaction.Commit();
                }
                catch //(Exception e)
                {
                    session.Clear();
                    session.Merge(ormObject);
                    transaction.Commit();
                }
                _error = null;
                //session.Close();
            }
            catch (Exception error)
            {
                SafeRollback(error, transaction);
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Updates the ORM object
        /// </summary>
        /// <param name="ormObject">Class to update</param>.
        /// <returns>Returns true, if the transaction succeeded</returns>
        protected virtual bool Update(object ormObject)
        {
            try
            {
                UpdateCore(ormObject);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the ORM object
        /// </summary>
        /// <param name="ormObject">Class to update</param>
        /// <param name="updateException">Returns the exception if the transaction failed else null</param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        protected virtual bool Update(object ormObject, out Exception updateException)
        {
            try
            {
                UpdateCore(ormObject);
                updateException = null;
                return true;
            }
            catch (Exception ex)
            {
                updateException = RetypeException(ex);
                return false;
            }
        }

        /// <summary>
        /// Merges the ORM object
        /// </summary>
        /// <param name="ormObject">Class to marge</param>
        /// <returns>Returns true, if the transaction succeeded</returns>
        protected virtual bool Merge([NotNull] object ormObject)
        {
            Validator.CheckForNull(ormObject,"ormObject");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly); //(_asmHbm);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();

                session.Merge(ormObject);
                transaction.Commit();
                _error = null;
                //session.Close();
                return true;
            }
            catch (Exception error)
            {
                SafeRollback(error, transaction);
                //session.Close();
                return false;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Closes the session
        /// </summary>
        protected virtual void CloseSession()
        {
            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            try
            {
                session.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Trivial query by Id
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <param name="id">Id of the object</param>
        /// <exception cref="ArgumentNullException">If the id is null</exception>
        /// <returns>Returns object form database</returns>
        protected T GetById<T>(object id)
        {
            ISession session;
            return GetById<T>(id, out session);
        }

        /// <summary>
        /// Trivial query by Id
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <param name="id">Id of the object</param>
        /// <param name="session"></param>
        /// <exception cref="ArgumentNullException">If the id is null</exception>
        /// <returns>Returns object form database</returns>
        protected T GetById<T>([NotNull] object id, out ISession session)
        {
            Validator.CheckForNull(id,"id");

            session = NhHelper.Singleton.GetSession(_thisAssembly);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                var obj = session.Get<T>(id);
                return obj;
            }
            catch (Exception error)
            {
                _error = error;
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Trivial query by primary key
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <param name="pk">Id of the object</param>
        /// <exception cref="ArgumentNullException">If the id is null</exception>
        /// <returns>Returns object form database</returns>
        protected virtual T GetByPk<T>([NotNull] object pk)
        {
            Validator.CheckForNull(pk,"pk");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                var obj = session.Get<T>(pk);
                //session.Close();
                return obj;
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Generic listing "select * from table"
        /// </summary>
        /// <typeparam name="T">Type of the ORM parameter</typeparam>
        /// <returns>Returns collection of selected objects</returns>
        public virtual ICollection<T> List<T>()
        {
            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                ICollection<T> collection = session.CreateCriteria(typeof(T))
                    .List<T>();
                //session.Close();
                return collection;
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Prepares criteria for the ancestor ORM handling object
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <returns>Returns criteria</returns>
        protected virtual ICriteria PrepareCriteria<T>()
        {
            return PrepareCriteria<T>(string.Empty);
        }

        /// <summary>
        /// Set distinct property (for correct row count computing)
        /// </summary>
        /// <param name="c">ICriteria for NHibernate</param>
        protected virtual void SetProjection(ICriteria c)
        {
        }

        protected ICriteria PrepareCriteria<T>(string alias)
        {
            //ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            //ICriteria criteria = session.CreateCriteria(typeof(T));
            //session.Close();
            //return criteria;        
            return 
                string.IsNullOrEmpty(alias) 
                    ? NhHelper.Singleton.GetSession(_thisAssembly).CreateCriteria(typeof(T)) 
                    : NhHelper.Singleton.GetSession(_thisAssembly).CreateCriteria(typeof(T), alias);
        }

        private const int MAX_SELECT_REPEATING = 5;

        /// <summary>
        /// Generic select operation with criteria
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <param name="criteria">Criteria to select</param>
        /// <exception cref="ArgumentNullException">if criteria is null</exception>
        /// <returns>Returns collection of selected objects</returns>
        protected virtual ICollection<T> Select<T>([NotNull] ICriteria criteria)
        {
            Validator.CheckForNull(criteria,"criteria");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();

                int iteration = 0;
                do
                {
                    try
                    {
                        ICollection<T> select = criteria.List<T>();
                        //session.Close();
                        return select;
                    }
                    catch (Exception error)
                    {
                        iteration++;

                        if (iteration > MAX_SELECT_REPEATING ||
                            error.InnerException == null ||
                            !error.InnerException.Message.Contains("Rerun the transaction"))
                        {
                            throw;
                        }

                        System.Threading.Thread.Sleep(500);
                    }
                }
                while (iteration <= MAX_SELECT_REPEATING);

                throw new Exception();
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Generic select operation with linq
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <param name="func">Condition where</param>
        /// <returns>Returns collection of selected objects</returns>
        protected ICollection<T> SelectLinq<T>(System.Linq.Expressions.Expression<Func<T, bool>> func)
        {
            ISession session = null;
            return SelectLinq(func, ref session);
        }

        /// <summary>
        /// Generic select operation with linq
        /// </summary>
        /// <typeparam name="T">Type of the ORM object</typeparam>
        /// <param name="func">Condition where</param>
        /// <param name="session"></param>
        /// <returns>Returns collection of selected objects</returns>
        protected ICollection<T> SelectLinq<T>(
            System.Linq.Expressions.Expression<Func<T, bool>> func,
            ref ISession session)
        {
            if (ReferenceEquals(session,null))
                session = NhHelper.Singleton.GetSession(_thisAssembly);

            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                ICollection<T> select = session.Query<T>()
                    .Where(func)
                    .ToList();
                return select;
            }
            catch (Exception error)
            {
                _error = error;
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Selects int scalar from database by criteria
        /// </summary>
        /// <param name="criteria">Criteria to select</param>
        /// <exception cref="ArgumentNullException">if criteria is null</exception>
        /// <returns>Returns int scalar</returns>
        protected virtual int SelectIntScalar([NotNull] ICriteria criteria)
        {
            Validator.CheckForNull(criteria,"criteria");

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                int selectInt = criteria.FutureValue<Int32>()
                    .Value;
                //session.Close();
                return selectInt;
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Performs query on DBS
        /// </summary>
        /// <typeparam name="T">Type of selected objects</typeparam>
        /// <param name="queryString">the query compatible with nhibernate and ORM</param>
        /// <returns>Returns collection of selected objects</returns>
        protected virtual ICollection<T> SelectByQuery<T>(string queryString)
        {
            ISession session = NhHelper.Singleton.GetSession(_thisAssembly); // _asmHbm);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                IQuery query = session.CreateQuery(queryString);
                //session.Close();
                return query.List<T>();
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Selects from database by sql query string
        /// </summary>
        /// <typeparam name="T">Type of selected objects</typeparam>
        /// <param name="sqlQueryString">Sql query string</param>
        /// <returns>Returns collection of selected objects</returns>
        protected virtual ICollection<T> SelectBySQL<T>(string sqlQueryString)
        {
            ISession session = NhHelper.Singleton.GetSession(_thisAssembly); // _asmHbm);
            ITransaction transaction = null;
            try
            {
                transaction = session.BeginTransaction();
                IQuery query = session.CreateSQLQuery(sqlQueryString);
                //session.Close();
                return query.List<T>();
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)                
                    transaction.Dispose();
            }
        }

        // SB
        protected int ExecuteUpdate(string sqlQueryString)
        {
            try
            {
                using (ISession session = NhHelper.Singleton.GetSession(_thisAssembly))
                using (ITransaction transaction = session.BeginTransaction())
                {
                    IQuery query = session.CreateSQLQuery(sqlQueryString);

                    int result = query.ExecuteUpdate();
                    transaction.Commit();

                    return result;
                }
            }
            catch (Exception error)
            {
                _error = error;
                throw;
            }
        }

        /*
        protected virtual ICollection<T> SelectBySQL<T>(string what,string tableName,params string[] condition)
        {
            Validator.CheckNullString(tableName);

            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    string query = AssembleSqlSelect(what, tableName, condition);

                    IQuery queryResult = session.CreateSQLQuery(query.ToString());

                    object o = queryResult.UniqueResult();
                    return queryResult.List<T>();
                }
                catch (Exception error)
                {
                    TransactionFailed(error);
                    throw error;
                }
            }
        }*/

        /// <summary>
        /// Creates sql condition string
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="value">String value</param>
        /// <exception cref="ArgumentNullException">if column name is null</exception>
        /// <returns>Returns sql condition string</returns>
        protected string SqlEq(string columnName, string value)
        {
            Validator.CheckNullString(columnName);

            return 
                value != null 
                    ? columnName + "='" + value + "'" 
                    : columnName + "= NULL";
        }

        /// <summary>
        /// Creates sql condition string
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="value">Bool value</param>
        /// <exception cref="ArgumentNullException">if column name is null</exception>
        /// <returns>Returns sql condition string</returns>
        protected string SqlEq(string columnName, bool value)
        {
            Validator.CheckNullString(columnName);

            return columnName + "= " + (value ? "1" : "0");
        }

        /// <summary>
        /// Get string for select count of lines in select
        /// </summary>
        protected string SqlCountAll
        {
            get { return "COUNT(*)"; }
        }

        /// <summary>
        /// Get string for select all columns
        /// </summary>
        protected string SqlAll
        {
            get { return "*"; }
        }

        /// <summary>
        /// Get string for condition and
        /// </summary>
        protected string SqlAnd
        {
            get { return "AND"; }
        }

        /// <summary>
        /// Get string for condition or
        /// </summary>
        protected string SqlOr
        {
            get { return "OR"; }
        }

        /// <summary>
        /// Creates sql query string
        /// </summary>
        /// <param name="what">Columns to select, if null or empty * is used</param>
        /// <param name="tableName">Table name to select</param>
        /// <param name="condition">Conditions to select</param>
        /// <returns>Returns sql query string</returns>
        protected string AssembleSqlSelect(
            string what, 
            [NotNull] string tableName, 
            params string[] condition)
        {
            Validator.CheckNullString(tableName);

            var query = new StringBuilder();
            query.Append("SELECT ");

            query.Append(Validator.IsNullString(what) ? "*" : what);

            query.Append(" FROM ");
            query.Append(tableName);

            if (null == condition || condition.Length <= 0)
                return query.ToString();

            query.Append(" WHERE ");

            foreach (string c in condition)
            {
                query.Append(c);
                query.Append(" ");
            }

            return query.ToString();
        }

        /// <summary>
        /// Selects int scalar from database
        /// </summary>
        /// <param name="what">Columns to select</param>
        /// <param name="tableName">Table name to select</param>
        /// <param name="condition">Conditions to select</param>
        /// <returns>Returns int scalar</returns>
        protected virtual int SelectIntScalar(string what, string tableName, params string[] condition)
        {
            ISession session = NhHelper.Singleton.GetSession(_thisAssembly);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                string query = AssembleSqlSelect(what, tableName, condition);

                IQuery queryResult = session.CreateSQLQuery(query);
                ICollection<int> l = queryResult.List<int>();
                //session.Close();
                return l.Count > 0 ? l.ElementAt(0) : 0;
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Selects from database by sql query string
        /// </summary>
        /// <param name="sqlQueryString">String with sql query</param>
        /// <returns>Returns list of objects from database</returns>
        public virtual IList<object> SelectBySQLList(string sqlQueryString)
        {
            var session = NhHelper.Singleton.GetSession(_thisAssembly); // _asmHbm);
            ITransaction transaction = null;

            try
            {
                transaction = session.BeginTransaction();
                IQuery query = session.CreateSQLQuery(sqlQueryString);
                //session.Close();
                return query.List<object>();
            }
            catch (Exception error)
            {
                _error = error;
                //session.Close();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        /// <param name="becauseOfError">Exception that set to last error</param>
        /// <param name="transaction">The transaction to roll back</param>
        /// <exception cref="InvalidOperationException">if roll back failed</exception>
        protected void SafeRollback(Exception becauseOfError, ITransaction transaction)
        {
            if (ReferenceEquals(transaction,null))
                return;

            try
            {
                transaction.Rollback();
                _error = becauseOfError;
            }
            catch
            {
                _error = 
                    new InvalidOperationException(
                        "Transaction rollback failed", 
                        becauseOfError ?? _error);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
