using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;

using NHibernate;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using System.Collections;

namespace Contal.Cgp.Server.DB
{
    public sealed class CardTemplates :
        ABaseOrmTable<CardTemplates, CardTemplate>, 
        ICardTemplates
    {
        private CardTemplates() : base(null)
        {
        }

        public string[] GetAllCardTemplateNames()
        {
            var result = new List<string>();
            var session = NhHelper.Singleton.GetSession();
            using (session.BeginTransaction())
            {
                IQuery query = session.CreateSQLQuery("select Name from CardTemplate");
                object queryResult = query.List();
                if (queryResult != null)
                {
                    foreach (string name in (IList)queryResult)
                    {
                        result.Add(name);
                    }
                }
            }

            return result.ToArray();
        }

        public override bool HasAccessView(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.IdManagementCardTemplatesAdmin),
                    login) ||
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.IdManagementCardTemplatesView),
                    login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.IdManagementCardTemplatesAdmin),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.IdManagementCardTemplatesAdmin),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.IdManagementCardTemplatesAdmin),
                login);
        }

        public ICollection<CardTemplateShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listTemplates = SelectByCriteria(filterSettings, out error);
            ICollection<CardTemplateShort> result = new List<CardTemplateShort>();
            if (listTemplates != null)
            {
                foreach (var template in listTemplates)
                {
                    result.Add(new CardTemplateShort(template));
                }
            }
            return result;
        }

        public CardTemplateShort ShortGetById(object id, out Exception ex)
        {
            ex = null;
            var template = GetObjectById(id);
            if (template == null)
                return null;

            return new CardTemplateShort(template);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CardTemplate; }
        }
    }
}
