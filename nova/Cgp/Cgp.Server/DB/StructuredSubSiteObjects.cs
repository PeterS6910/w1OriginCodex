using System.Linq;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

using NHibernate;

namespace Contal.Cgp.Server.DB
{
    public sealed class StructuredSubSiteObjects : ABaseOrmTable<StructuredSubSiteObjects, StructuredSubSiteObject>
    {
        public const string STRUCTURED_SUB_SITE_OBJECT_TABLE_NAME = "StructuredSubSiteObject";

        private StructuredSubSiteObjects() : base(null)
        {
        }

        public override object ParseId(string strObjectId)
        {
            int result;

            return int.TryParse(strObjectId, out result)
                ? (object)result
                : null;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.STRUCTURED_SUB_SITE), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.StructuredSubSiteAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.StructuredSubSiteAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.StructuredSubSiteAdmin), login);
        }

        public ICollection<StructuredSubSiteObject> GetStructuredSubSiteObjects(
            StructuredSubSite site,
            ObjectType objectType,
            string objectId,
            bool isReference)
        {
            return
                SelectLinq<StructuredSubSiteObject>(
                    structuredSubSiteObject =>
                        structuredSubSiteObject.StructuredSubSite == site &&
                        structuredSubSiteObject.ObjectType == objectType &&
                        structuredSubSiteObject.ObjectId == objectId &&
                        structuredSubSiteObject.IsReference == isReference);
        }

        public StructuredSubSiteObject FindStructuredSubSiteObject(
            ObjectType objectType,
            string objectIdString)
        {
            return
                SelectLinq<StructuredSubSiteObject>(
                    structuredSubSiteObject =>
                        structuredSubSiteObject.ObjectType == objectType &&
                        structuredSubSiteObject.ObjectId == objectIdString &&
                        structuredSubSiteObject.IsReference == false)
                    .FirstOrDefault();
        }

        public void DeleteStructuredSubSiteObjectReference(
            ObjectType objectType,
            string objectId,
            int idStructuredSubSite)
        {
            ISession session = null;

            var structuredSubSite =
                idStructuredSubSite != -1
                    ? StructuredSubSites.Singleton
                        .GetById(
                            idStructuredSubSite,
                            out session)
                    : null;

            DeleteByCriteria(
                structuredSubSiteObject =>
                    structuredSubSiteObject.ObjectType == objectType &&
                    structuredSubSiteObject.ObjectId == objectId &&
                    structuredSubSiteObject.IsReference == true &&
                    structuredSubSiteObject.StructuredSubSite == structuredSubSite,
                session);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.StructuredSubSiteObject; }
        }
    }
}
