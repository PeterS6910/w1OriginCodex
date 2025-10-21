using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.ORM;

using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class SystemVersions : ATableORM<SystemVersions>
    {
        private const string TABLENAME = "SystemVersion";
        private const string COLUMNIDVERSION = "IdVersion";
        private const string COLUMNVERSION = "Version";

        private SystemVersions() : base(null)
        {
            NhHelper.Singleton.ConnectionString = 
                ConnectionString.LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            ThisAssembly = typeof(SystemVersion).Assembly;
        }

        #region ITableORM<SystemVersions> Members

        public bool Insert(SystemVersion ormObject)
        {
            return base.Insert(ormObject, null);
        }

        public bool Insert(ref SystemVersion ormObject, out Exception error)
        {
            return base.Insert(ormObject, out error);
        }

        public bool Insert(SystemVersion obj, out Exception insertException, out string Version)
        {
            bool result = base.Insert(obj, out insertException);
            Version = obj.Version;
            return result;
        }

        public bool InsertUdate(SystemVersion obj, out Exception exception, out string Version)
        {
            IList<object> list = base.SelectBySQLList("select top 1 IdVersion from SystemVersion");
            if (list == null || list.Count == 0)
            {
                Version = obj.Version;
                return base.Insert(obj, out exception);
            }
            Version = obj.Version;
            obj.IdVersion = (Guid)list.First();
            return base.Update(obj, out exception);
        }

        public bool Update(SystemVersion ormObject)
        {
            return base.Update(ormObject);
        }

        public bool Delete(SystemVersion ormObject)
        {
            return base.Delete(ormObject);
        }

        public SystemVersion GetById(object id)
        {
            return base.GetById<SystemVersion>(id);
        }

        public SystemVersion GetByPk(string version)
        {
            return base.GetByPk<SystemVersion>(version);
        }

        public ICollection<SystemVersion> List()
        {
            return base.List<SystemVersion>();
        }

        public ICollection<SystemVersion> SelectByCriteria(IList<FilterSettings> filterSettings)
        {
            ICriteria c = PrepareCriteria<SystemVersion>();

            if (filterSettings != null)
            {
                foreach (FilterSettings filterSetting in filterSettings)
                {
                    if (filterSetting.Column == "ExpirationSet")
                    {
                        if ((bool)filterSetting.Value)
                        {
                            c = c.Add(Expression.IsNotNull("ExpirationDate"));
                        }
                        else
                        {
                            c = c.Add(Expression.IsNull("ExpirationDate"));
                        }
                    }
                    else
                    {
                        c = FilterSettingsToCriteria.AddCriteria(c, filterSetting);
                    }
                }
            }

            return base.Select<SystemVersion>(c);
        }

        #endregion

        public SystemVersions CreateNewSystemVersion()
        {
            return new SystemVersions(); 
        }
    }
}
