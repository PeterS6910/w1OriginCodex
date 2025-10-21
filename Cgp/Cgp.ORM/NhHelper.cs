using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using System.Data.SqlClient;

using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.ORM
{
    public class NhHelper
    {
        private ISessionFactory _sessionFactory;

        /// <summary>
        /// Get or set conection string to the database
        /// </summary>
        public ConnectionString ConnectionString { get; set; }

        private NhHelper()
        {
        }

        private static volatile NhHelper _singleton;
        private static readonly object SyncRoot = new object();

        public static NhHelper Singleton
        {
            get
            {
                if (null != _singleton)
                    return _singleton;

                lock (SyncRoot)
                    if (null == _singleton)
                        _singleton = new NhHelper();

                return _singleton;
            }
        }

        private void AddAssemblies(Configuration config, IEnumerable<Assembly> assemblies)
        {
            if (null == config || null == assemblies)
                return;
//#if DEBUG
//            var _assembly = Assembly.LoadFrom("Cgp.NCAS.Server.Beans.dll");
//            config.AddAssembly(_assembly);
//#endif
            foreach (Assembly assembly in assemblies)
            {
                config.AddAssembly(assembly);
            }
        }

        //private void 

        /// <summary>
        /// Create session to the DBS
        /// </summary>
        /// <param name="assemblies">the assembly that contains hbm.xml files</param>
        /// <returns>return ISession</returns>
        public ISession GetSession([NotNull] IEnumerable<Assembly> assemblies)
        {
// ReSharper disable PossibleMultipleEnumeration
            Validator.CheckForNull(assemblies,"assemblies");


            if (_sessionFactory == null)
            {
                Configuration cfg = ConfigureNHibernate();
                AddAssemblies(cfg, assemblies);
                _sessionFactory = cfg.BuildSessionFactory();
            }
// ReSharper restore PossibleMultipleEnumeration
            
            return _sessionFactory.OpenSession();
        }

        /// <summary>
        /// Create session to the DBS
        /// </summary>
        /// <returns>return session</returns>
        public ISession GetSession(Assembly assembly)
        {
            return GetSession(Enumerable.Repeat(assembly, 1));
        }

        /// <summary>
        /// Create session to the DBS. Add calling assembly.
        /// </summary>
        /// <returns>return session</returns>
        public ISession GetSession()
        {
            return GetSession(Assembly.GetCallingAssembly());
        }

        private Configuration ConfigureNHibernate()
        {
            var cfg = new Configuration();

            string connectString = ConnectionString.ToString();

            if (!String.IsNullOrEmpty(connectString))
                cfg.SetProperty("connection.connection_string", connectString);

            cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
            cfg.SetProperty("command_timeout", "90000");
/*#if DEBUG
            cfg.SetProperty("show_sql", "true");
#else
            cfg.SetProperty("show_sql", "false");
#endif*/
            //cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu");
            //cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            return cfg;
        }


        /// <summary>
        /// Update tables on DBS
        /// </summary>
        /// <param name="assemblies"></param>
        public void UpdateSchema([NotNull] IEnumerable<Assembly> assemblies)
        {
// ReSharper disable PossibleMultipleEnumeration
            Validator.CheckForNull(assemblies,"assemblies");

          
            Configuration cfg = ConfigureNHibernate();

            foreach (Assembly asm in assemblies)
                cfg.AddAssembly(asm);
// ReSharper restore PossibleMultipleEnumeration


            new SchemaUpdate(cfg).Execute(
#if DEBUG
                true
#else
                false
#endif
                ,true);

        }

        /// <summary>
        /// Create tables on BDS
        /// </summary>
        /// <param name="assemblies"></param>
        public void CreateSchema([NotNull] IEnumerable<Assembly> assemblies)
        {
// ReSharper disable PossibleMultipleEnumeration
            Validator.CheckForNull(assemblies,"assemblies");


            Configuration cfg = ConfigureNHibernate();
            //cfg.AddAssembly(Assembly.GetCallingAssembly());
            foreach (Assembly asm in assemblies)
                cfg.AddAssembly(asm);
// ReSharper restore PossibleMultipleEnumeration

            new SchemaExport(cfg).Create(false, true);
        }

        /// <summary>
        /// Add assemblies and run BuildSessionFactory.
        /// </summary>
        /// <param name="assemblies">Assemblies that contains hbm.xml files</param>
        /// <param name="connectString">Connect string to the database</param>
        public void ControlAssemblies(
            [NotNull] LinkedList<Assembly> assemblies, 
            ConnectionString connectString)
        {
            try
            {
                Validator.CheckForNull(assemblies,"assemblies");

                ConnectionString = connectString;

                Configuration config = ConfigureNHibernate();

               AddAssemblies(config, assemblies);
                _sessionFactory = config.BuildSessionFactory();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void Disconnect()
        {
            if (ConnectionString == null || !ConnectionString.IsValid())
                return;

            try
            {
                var sqlConnection = new SqlConnection
                {
                    ConnectionString = ConnectionString.ToString()
                };

                SqlConnection.ClearPool(sqlConnection);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Return version of database server
        /// </summary>
        /// <returns></returns>
        public string GetVersionOfDatabaseServer()
        {
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = 
                    new SqlConnection
                    {
                        ConnectionString = ConnectionString.ToString()
                    };

                sqlConnection.Open();

                var myCommand = new SqlCommand(@"Select @@version", sqlConnection);
                return (string)myCommand.ExecuteScalar();
            }
            catch
            {
            }
            finally
            {
                if (sqlConnection != null)
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch
                    {
                    }
            }

            return string.Empty;
        }
    }
    
}
