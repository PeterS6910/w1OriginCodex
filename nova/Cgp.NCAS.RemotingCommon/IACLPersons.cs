using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IACLPersons : IBaseOrmTable<ACLPerson>
    {
        ICollection<CardReader> LoadActiveCardReaders(Person person, out Exception error);

        bool HasAccess(
            Guid idPerson,
            ObjectType objectType,
            Guid idObject);

        Dictionary<Guid, AOrmObject> GetActualAccessAOrmObjects(Person person, out Dictionary<Guid, object> objectStates, out Exception error);
        ICollection<ACLPerson> GetAclPersonsByPerson(Guid idPerson, out Exception error);
        ICollection<Person> GetPersonsForACL(AccessControlList acl);
        IList<String> PersonAclAssignment(IList<Object> persons, IList<Guid> idAcls, DateTime? dateFrom, DateTime? dateTo);
        string GetAlarmAreaActivationRights(Guid guidCcu, Person person, AlarmArea alarmArea);
    }
}
