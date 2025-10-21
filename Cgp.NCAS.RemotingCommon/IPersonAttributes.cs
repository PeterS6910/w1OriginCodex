using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    // SB
    /// <summary>
    /// PersonAttributeInfo
    /// </summary>
    [Serializable()]
    public class PersonAttributeInfo
    {
        public PersonAttributeInfo(Guid idPersonAttribute, Guid idPerson, string fullName, bool isWatched)
        {
            IdPersonAttribute = idPersonAttribute;
            IdPerson = idPerson;
            FullName = fullName;
            IsWatched = isWatched;
        }

        public Guid IdPersonAttribute { get; private set; }

        public Guid IdPerson { get; private set; }

        public string FullName { get; private set; }

        public bool IsWatched { get; set; }

        public override string ToString()
        {
            return FullName;
        }

        public static PersonAttributeInfo OfPerson(Person person)
        {
            return new PersonAttributeInfo(Guid.Empty, person.IdPerson, person.WholeName, false);
        }

        public static PersonAttributeInfo OfPersonAttribute(PersonAttribute personAttribute)
        {
            var person = personAttribute.Person;

            return new PersonAttributeInfo(personAttribute.IdPersonAttribute, person.IdPerson, person.WholeName, true);
        }
    }

    // SB
    /// <summary>
    /// IPersonAttributes
    /// </summary>
    public interface IPersonAttributes : IBaseOrmTable<PersonAttribute>
    {
        ICollection<PersonAttributeInfo> GetAllPersonAttributes();

        void UpdatePersonAttributes(ICollection<PersonAttributeInfo> personAttributes);

        bool GetIsPersonWatched(string cardNumber);
    }
}
