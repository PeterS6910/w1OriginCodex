using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.NCAS.Server.DB
{
    // SB
    /// <summary>
    /// PersonAttributes
    /// </summary>
    public sealed class PersonAttributes : ANcasBaseOrmTable<PersonAttributes, PersonAttribute>, IPersonAttributes
    {
        private PersonAttributes() 
            : base(null)
        {
        }

        public ICollection<PersonAttributeInfo> GetAllPersonAttributes()
        {
            var personAttributeByIdPerson = new Dictionary<Guid, PersonAttribute>();

            foreach (var personAttribute in List())
            {
                personAttributeByIdPerson[personAttribute.Person.IdPerson] = personAttribute;
            }

            var persons = Persons.Singleton.List()
                    .OrderBy(p => p.Surname)
                    .ThenBy(p => p.FirstName);

            var allPersonAttributes = new List<PersonAttributeInfo>();

            foreach (var person in persons)
            {
                PersonAttribute personAttribute;
                if (personAttributeByIdPerson.TryGetValue(
                    person.IdPerson,
                    out personAttribute))
                {
                    allPersonAttributes.Add(PersonAttributeInfo.OfPersonAttribute(personAttribute));
                }
                else
                {
                    allPersonAttributes.Add(PersonAttributeInfo.OfPerson(person));
                }
            }

            return allPersonAttributes;
        }

        public void UpdatePersonAttributes(ICollection<PersonAttributeInfo> personAttributes)
        {
            foreach(var personAttributeInfo in personAttributes)
            {
                if (!personAttributeInfo.IsWatched)
                {
                    if (personAttributeInfo.IdPersonAttribute != Guid.Empty)
                        DeletePersonAttributes(personAttributeInfo.IdPerson);

                    continue;
                }

                if (personAttributeInfo.IdPersonAttribute != Guid.Empty)
                    continue;

                var personAttribute = new PersonAttribute
                {
                    Person = Persons.Singleton.GetById(personAttributeInfo.IdPerson)
                };

                InsertOnlyInDatabase(ref personAttribute);
            }
        }

        public bool GetIsPersonWatched(string cardNumber)
        {
            var card = Cards.Singleton.GetCardFromFullCardNumber(cardNumber);
            if (card == null || card.Person == null)
                return false;

            var idPerson = card.Person.IdPerson;

            var personAttributes = SelectLinq<PersonAttribute>(
                personAttribute =>
                    personAttribute.Person != null
                    && personAttribute.Person.IdPerson == idPerson);

            return personAttributes != null
                   && personAttributes.Count > 0;
        }

        public override bool HasAccessView(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.PersonAttribute; }
        }

        public void DeletePersonAttributes(Guid idPerson)
        {
            DeleteByCriteria(
                personAttribute =>
                    personAttribute.Person != null
                    && personAttribute.Person.IdPerson == idPerson,
                null);
        }
    }
}