using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server
{
    public abstract class ACgpServerPlugin : ACgpPlugin<ACgpServerPlugin>
    {
        public virtual void InitDatabaseDefaults()
        {
        }

        public virtual void EnsureUpgradeDirectories()
        {
        }

        public virtual void RunDBTest()
        {
        }

        public virtual IEnumerable<AOrmObject> GetDirectReferences(AOrmObject objRef)
        {
            return null;
        }

        public virtual ICollection<AOrmObject> GetReferencedByObject(AOrmObject objRef)
        {
            return null;
        }

        public virtual Dictionary<string, Type> GetRequiredLicenceProperties()
        {
            return null;
        }

        public virtual bool SetRequiredLicenceProperties(Dictionary<string, object> properties)
        {
            return false;
        }

        public virtual bool GetLicencePropertyInfo(
            string propertyName, 
            out string localisedName, 
            out object value)
        {
            return GetLicencePropertyInfo(
                propertyName, 
                null, 
                out localisedName, 
                out value);
        }

        public virtual bool GetLicencePropertyInfo(
            string propertyName, 
            string language, 
            out string localisedName,
            out object value)
        {
            localisedName = string.Empty;
            value = null;

            return false;
        }

        public virtual bool GetLocalisedLicencePropertyName(
            string propertyName,
            out string localisedName)
        {
            return 
                GetLocalisedLicencePropertyName(
                    propertyName, 
                    null, 
                    out localisedName);
        }

        public virtual bool GetLocalisedLicencePropertyName(
            string propertyName,
            string language,
            out string localisedName)
        {
            localisedName = string.Empty;
            return false;
        }

        public virtual void GeneralOptionsChanged()
        {
        }

        public virtual AOrmObject GetPluginTableObject(ObjectType objectType, Guid objectId)
        {
            return null;
        }

        public virtual ICollection<StructuredSiteObjectWithChildObjects> GetObjectsNotInStructuredSubSites(
            ObjectType objectType,
            ICollection<string> objectIdsInStructuredSubSites)
        {
            return null;
        }

        public abstract ITableORM GetTableOrmForObjectType(ObjectType objectType);

        public virtual bool ProcessPresentationGroup(PerformPresentationGroup performPresentationGroup, string msg)
        {
            return true;
        }

        public abstract bool CodeAlreadyUsed(string codeHashValue);
    }
}
