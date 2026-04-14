using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IPersons : IBaseOrmTable<Person>
    {
        bool InsertPerson(ref Person obj, out Exception insertException);
        bool UpdatePerson(Person obj, out Exception updateException);

        ICollection<PersonShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        ICollection<PersonShort> ShortSelectByCriteria(out Exception error, LogicalOperators filterJoinOperator,    params ICollection<FilterSettings>[] filterSettings);
        IList<IModifyObject> ListModifyObjects(out Exception error);
        void SetPersonDepartment(UserFoldersStructure newDepartment, Guid personId);
        void SetPersonDepartment(UserFoldersStructure newDepartment, UserFoldersStructure oldDepartment, Guid personId);

        bool ImportPersons(Guid formIdentification, List<ImportPersonData> importPersonsData, CSVImportType importType,
            string departmentFolder, StructuredSubSite structuredSubSite, bool tryParseBirthDateFromPersonId,
            out List<CSVImportPerson> csvImportPersons, out bool licenceRestriction, out int importedPersonsCount);

        void RemoveLogins(Guid guidPerson);
        BinaryPhoto GetPhoto(Guid personId);
        void SavePhoto(Guid personId, BinaryPhoto binaryPhoto);
        IList<IModifyObject> ListModifyPersonsWithCards(out Exception error);
        bool HasAssignedCards(Guid idPerson);

        string GetRandomPersonalCode(Guid idPerson);
    }
}
