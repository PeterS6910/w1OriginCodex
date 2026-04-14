using Cgp.Components;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;

namespace Contal.Cgp.Server.DB
{
    public sealed class Persons :
        ABaserOrmTableWithAlarmInstruction<Persons, Person>,
        IPersons
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<Person>
        {
            public override void BeforeDelete(Person obj)
            {
                base.BeforeDelete(obj);
                Singleton.BeforePersonDelete(obj);
            }
        }

        private Persons() : base(null, new CudPreparation())
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(Person person)
        {
            var cards = person.Cards;

            if (cards != null)
                foreach (var card in cards)
                    yield return card;

            var logins = person.Logins;

            if (logins != null)
                foreach (var login in logins)
                    yield return login;
        }

        public const string PERSON_CONVERSION_STRING = @"%PERSON_CONV1%";
        private const string PHOTOS_PATH = @"\Pictures\Persons";

        public bool InsertPerson(ref Person obj, out Exception insertException)
        {
            obj.WholeName = obj.FirstName + " " + obj.Surname;
            return Insert(ref obj, out insertException);
        }

        public bool UpdatePerson(Person obj, out Exception updateException)
        {
            obj.WholeName = obj.FirstName + " " + obj.Surname;
            return Update(obj, out updateException);
        }

        public new bool UpdateOnlyInDatabase(Person obj, out Exception updateException)
        {
            obj.WholeName = obj.FirstName + " " + obj.Surname;
            return Update(obj, out updateException, false);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.PERSONS),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.PERSONS),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsInsertDeletePerform),
                login);
        }

        private void BeforePersonDelete(Person person)
        {
            if (person != null)
            {
                if (person.Cards != null && person.Cards.Count > 0)
                {
                    foreach (var card in person.Cards)
                    {
                        var cardEdit = Cards.Singleton.GetObjectForEdit(card.IdCard);
                        if (cardEdit != null)
                        {
                            cardEdit.Person = null;
                            if (CardPairs.Singleton.IsCardRelated(card.IdCard))
                                cardEdit.State = (byte)CardState.HybridUnused;
                            else
                                cardEdit.State = (byte)CardState.Unused;


                            Cards.Singleton.Update(cardEdit);
                            Cards.Singleton.EditEnd(cardEdit);
                        }
                    }
                }

                DbWatcher.Singleton.DbPersonBeforeUD(person, ObjectDatabaseAction.Delete);
            }
        }

        public override void AfterUpdate(
            Person newPerson,
            Person oldPersonBeforUpdate)
        {
            DbWatcher.Singleton.DbPersonAfterUpdate(
                newPerson,
                oldPersonBeforUpdate);
        }

        public override void CUDSpecial(Person person, ObjectDatabaseAction objectDatabaseAction)
        {
            if (person != null)
            {
                DbWatcher.Singleton.DbPersonChanged(person, objectDatabaseAction);
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(Person.COLUMNSURNAME, true));
            c.AddOrder(new Order(Person.COLUMNFIRSTNAME, true));
        }

        protected override void LoadObjectsInRelationship(Person obj)
        {
            if (obj.Logins != null)
            {
                IList<Login> list = new List<Login>();

                foreach (var login in obj.Logins)
                {
                    list.Add(Logins.Singleton.GetById(login.IdLogin));
                }

                obj.Logins.Clear();
                foreach (var login in list)
                    obj.Logins.Add(login);
            }

            if (obj.Cards != null)
            {
                IList<Card> list = new List<Card>();

                foreach (var card in obj.Cards)
                {
                    list.Add(Cards.Singleton.GetById(card.IdCard));
                }

                obj.Cards.Clear();
                foreach (var card in list)
                    obj.Cards.Add(card);
            }
            
            obj.Department = UserFoldersStructures.Singleton.GetPersonDepartment(obj.GetIdString());
        }

        public override bool CheckData(Person ormObject, out Exception error)
        {
            if (GeneralOptions.Singleton.UniqueAndNotNullPersonalKey)
            {
                var persons = SelectLinq<Person>(person => person.IdPerson != ormObject.IdPerson && person.Identification == ormObject.Identification);

                if (persons != null && persons.Count > 0)
                {
                    error = new SqlUniqueException(Person.COLUMNIDENTIFICATION);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(ormObject.PersonalCodeHash))
            {
                if (CodeAlreadyUsed(
                        ormObject.PersonalCodeHash,
                        ormObject.IdPerson))
                {
                    error = new SqlUniqueException(Person.COLUMN_PERSONAL_CODE_HASH);
                    return false;
                }
            }

            return base.CheckData(ormObject, out error);
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<Person> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                // ReSharper disable StringIndexOfIsCultureSpecific.1
                linqResult = single
                    ? SelectLinq<Person>(p => (p.FirstName + " " + p.Surname).IndexOf(name) >= 0)
                    : SelectLinq<Person>(
                        p =>
                            (p.FirstName + " " + p.Surname).IndexOf(name) >= 0 ||
                            p.Description.IndexOf(name) >= 0);
                // ReSharper restore StringIndexOfIsCultureSpecific.1
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(person => person.Surname).ThenBy(person => person.FirstName).ToList();
                foreach (var person in linqResult)
                {
                    resultList.Add(person);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Person> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                var criteria = PrepareCriteria<Person>();
                criteria.Add(
                    Restrictions.Or(
                        Expression.Sql(
                            string.Format("([{0}] + ' ' + [{1}]) COLLATE LATIN1_GENERAL_CI_AI LIKE '%{2}%'",
                                Person.COLUMNFIRSTNAME, Person.COLUMNSURNAME, name)),
                        Restrictions.Like(Person.COLUMNDESCRIPTION, name, MatchMode.Anywhere)));

                linqResult = Select<Person>(criteria);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<Person> linqResult;

            if (string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                var criteria = PrepareCriteria<Person>();
                criteria.Add(
                    Expression.Sql(string.Format("([{0}] + ' ' + [{1}]) COLLATE LATIN1_GENERAL_CI_AI LIKE '%{2}%'",
                        Person.COLUMNFIRSTNAME, Person.COLUMNSURNAME, name)));

                linqResult = Select<Person>(criteria);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<Person> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(person => person.Surname).ThenBy(person => person.FirstName).ToList();
                foreach (var person in linqResult)
                {
                    resultList.Add(person);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<PersonShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listPerson = SelectByCriteria(filterSettings, out error);
            ICollection<PersonShort> result = new List<PersonShort>();
            if (listPerson != null)
            {
                foreach (var person in listPerson)
                {
                    var department = UserFoldersStructures.Singleton.GetPersonDepartment(person.GetIdString());
                    if (department != null)
                    {
                        person.Department = department;
                    }
                    result.Add(new PersonShort(person));
                }
            }
            return result;
        }

        public ICollection<PersonShort> ShortSelectByCriteria(
            out Exception error,
            LogicalOperators filterJoinOperator,
            params ICollection<FilterSettings>[] filterSettings)
        {
            var listPerson = SelectByCriteria(out error, filterJoinOperator, filterSettings);
            ICollection<PersonShort> result = new List<PersonShort>();
            if (listPerson != null)
            {
                foreach (var person in listPerson)
                {
                    var department = UserFoldersStructures.Singleton.GetPersonDepartment(person.GetIdString());
                    if (department != null)
                    {
                        person.Department = department;
                    }
                    result.Add(new PersonShort(person));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listPerson = List(out error);
            IList<IModifyObject> listPersonModifyObj = null;
            if (listPerson != null)
            {
                listPersonModifyObj = new List<IModifyObject>();
                foreach (var person in listPerson)
                {
                    listPersonModifyObj.Add(new PersonModifyObj(person));
                }
                listPersonModifyObj = listPersonModifyObj.OrderBy(person => person.ToString()).ToList();
            }
            return listPersonModifyObj;
        }

        public void SetPersonDepartment(UserFoldersStructure newDepartment, Guid personId)
        {
            UserFoldersStructureObjects.Singleton.SetPersonDepartment(newDepartment, personId);
        }

        public void SetPersonDepartment(UserFoldersStructure newDepartment, UserFoldersStructure oldDepartment, Guid personId)
        {
            UserFoldersStructureObjects.Singleton.SetPersonDepartment(newDepartment, oldDepartment, personId);
        }

        private readonly EventHandlerGroup<IImportEventHandler> _importEventHandlerGroup =
            new EventHandlerGroup<IImportEventHandler>();

        public void AddImportEventHandler(IImportEventHandler importEventHandler)
        {
            _importEventHandlerGroup.Add(importEventHandler);
        }

        public bool ImportPersons(Guid formIdentification, List<ImportPersonData> importPersonsData,
            CSVImportType importType, string departmentFolder, StructuredSubSite structuredSubSite,
            bool tryParseBirthDateFromPersonId, out List<CSVImportPerson> csvImportPersons, out bool licenceRestriction,
            out int importedPersonsCount)
        {
            csvImportPersons = new List<CSVImportPerson>();
            importedPersonsCount = 0;

            if (!EnableStartCSVImport(out licenceRestriction))
                return false;

            try
            {
                _importEventHandlerGroup.ForEach(
                    importEventHandler =>
                        importEventHandler.ImportStarted());

                if (importPersonsData != null && importPersonsData.Count > 0)
                {
                    var count = 0;
                    foreach (var importPersonData in importPersonsData)
                    {
                        if (importPersonData != null)
                        {
                            if (!string.IsNullOrEmpty(importPersonData.Identification))
                            {
                                var person = new Person(importPersonData);
                                var departmentUFS = CreateDepartmentUserFolderStructures(
                                    importPersonData.Department,
                                    departmentFolder,
                                    structuredSubSite);

                                person.Department = departmentUFS;
                                var pesonIdentificationOccurrence = 0;

                                var addBirthday = false;
                                if (tryParseBirthDateFromPersonId)
                                {
                                    try
                                    {
                                        if (person.Identification.Length >= 6)
                                        {
                                            var strYear = person.Identification.Substring(0, 2);
                                            var strMonth = person.Identification.Substring(2, 2);
                                            var strDay = person.Identification.Substring(4, 2);
                                            int year;
                                            int month;
                                            int day;

                                            if (Int32.TryParse(strYear, out year) && Int32.TryParse(strMonth, out month) &&
                                                Int32.TryParse(strDay, out day))
                                            {
                                                person.Birthday = new DateTime(1900 + year, month, day);
                                                addBirthday = true;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                var retValue = false;
                                var csvImportResult = CSVImportResult.Failed;
                                switch (importType)
                                {
                                    case CSVImportType.AddAll:
                                        retValue = Insert(ref person);
                                        if (retValue)
                                        {
                                            csvImportResult = CSVImportResult.Added;
                                            SetPersonDepartment(person.Department, person.IdPerson);

                                            StructuredSubSites.Singleton.AddObjectToSite(person, structuredSubSite);
                                        }
                                        break;
                                    case CSVImportType.IgnoreOnConflict:
                                        var oldPersonIgnoreOnConflict = GetPersonFromPersonId(person.Identification,
                                            ref pesonIdentificationOccurrence);
                                        if (oldPersonIgnoreOnConflict == null)
                                        {
                                            retValue = Insert(ref person);
                                            if (retValue)
                                            {
                                                csvImportResult = CSVImportResult.Added;
                                                SetPersonDepartment(person.Department, person.IdPerson);
                                                StructuredSubSites.Singleton.AddObjectToSite(person, structuredSubSite);
                                            }
                                        }
                                        else
                                        {
                                            person = oldPersonIgnoreOnConflict;
                                            retValue = true;
                                            csvImportResult = CSVImportResult.IgnoreConflict;
                                        }
                                        break;
                                    case CSVImportType.OverwriteOnConflict:
                                    case CSVImportType.OverwriteNonEmptyData:
                                        var oldPerson = GetPersonFromPersonId(person.Identification,
                                            ref pesonIdentificationOccurrence);
                                        if (oldPerson == null)
                                        {
                                            retValue = Insert(ref person);
                                            if (retValue)
                                            {
                                                csvImportResult = CSVImportResult.Added;
                                                SetPersonDepartment(person.Department, person.IdPerson);
                                                StructuredSubSites.Singleton.AddObjectToSite(person, structuredSubSite);
                                            }
                                        }
                                        else if (pesonIdentificationOccurrence == 1)
                                        {
                                            oldPerson = GetObjectForEdit(oldPerson.IdPerson);
                                            if (oldPerson != null)
                                            {
                                                retValue =
                                                    OverridePerson(importType == CSVImportType.OverwriteOnConflict,
                                                        addBirthday, person, oldPerson);
                                                person = oldPerson;
                                                csvImportResult = CSVImportResult.Overwritten;
                                            }
                                            else
                                            {
                                                csvImportResult = CSVImportResult.Failed;
                                            }
                                        }
                                        else
                                        {
                                            //more pesonIdentificationOccurrence (more persons with same ID)
                                            csvImportResult = CSVImportResult.MorePersonsWithSameID;
                                        }
                                        break;
                                }

                                if (retValue)
                                {
                                    csvImportPersons.Add(new CSVImportPerson(person.IdPerson, person.ToString(),
                                        csvImportResult));
                                    if (csvImportResult == CSVImportResult.Added ||
                                        csvImportResult == CSVImportResult.Overwritten)
                                    {
                                        importedPersonsCount++;
                                    }
                                }
                                else
                                    csvImportPersons.Add(new CSVImportPerson(person.IdPerson, person.ToString(),
                                        csvImportResult));

                                count++;
                                var param = new object[2];
                                param[0] = formIdentification;
                                param[1] = count;
                                CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                                    RunImportedPersonCountChanged, DelegateSequenceBlockingMode.Asynchronous, false,
                                    param);
                            }
                            else
                            {
                                csvImportPersons.Add(new CSVImportPerson(Guid.Empty,
                                    importPersonData.FirstName + importPersonData.Surname,
                                    CSVImportResult.PersonIdIsNotEntry));
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            _importEventHandlerGroup.ForEach(
                importEventHanlder =>
                    importEventHanlder.ImportDone());

            _csvImportInProgress = false;
            return true;
        }

        private bool OverridePerson(bool overrrideAllData, bool addBirthday, Person person, Person oldPerson)
        {
            if (overrrideAllData || person.Department != null)
            {
                SetPersonDepartment(
                    person.Department,
                    UserFoldersStructures.Singleton.GetPersonDepartment(oldPerson.GetIdString()),
                    oldPerson.IdPerson);

                oldPerson.Department = person.Department;
            }

            if (overrrideAllData || !string.IsNullOrEmpty(person.FirstName))
                oldPerson.FirstName = person.FirstName;

            if (overrrideAllData || !string.IsNullOrEmpty(person.Surname))
                oldPerson.Surname = person.Surname;

            if (overrrideAllData || !string.IsNullOrEmpty(person.MiddleName))
                oldPerson.MiddleName = person.MiddleName;

            if (overrrideAllData || !string.IsNullOrEmpty(person.EmployeeNumber))
                oldPerson.EmployeeNumber = person.EmployeeNumber;

            if (overrrideAllData || !string.IsNullOrEmpty(person.Company))
                oldPerson.Company = person.Company;

            if (overrrideAllData || !string.IsNullOrEmpty(person.Role))
                oldPerson.Role = person.Role;

            if (overrrideAllData || !string.IsNullOrEmpty(person.CostCenter))
                oldPerson.CostCenter = person.CostCenter;

            if (overrrideAllData || !string.IsNullOrEmpty(person.RelativeSuperior))
                oldPerson.RelativeSuperior = person.RelativeSuperior;

            if (overrrideAllData || !string.IsNullOrEmpty(person.RelativeSuperiorsPhoneNumber))
                oldPerson.RelativeSuperiorsPhoneNumber = person.RelativeSuperiorsPhoneNumber;

            if (overrrideAllData || person.EmploymentBeginningDate != null)
                oldPerson.EmploymentBeginningDate = person.EmploymentBeginningDate;

            if (overrrideAllData || person.EmploymentEndDate != null)
                oldPerson.EmploymentEndDate = person.EmploymentEndDate;

            if (addBirthday && (overrrideAllData || person.Birthday != null))
            {
                oldPerson.Birthday = person.Birthday;
            }

            var retValue = UpdateOnlyInDatabase(oldPerson);
            EditEnd(oldPerson);
            return retValue;
        }

        private Person GetPersonFromPersonId(string personId, ref int count)
        {
            var linqResult = SelectLinq<Person>(p => p.Identification == personId);
            if (linqResult != null && linqResult.Count > 0)
            {
                count = linqResult.Count;
                return linqResult.ToList()[0];
            }

            return null;
        }

        private UserFoldersStructure CreateDepartmentUserFolderStructures(string department,
            string departmentFolder, StructuredSubSite structuredSubSite)
        {
            if (string.IsNullOrEmpty(department) 
                && string.IsNullOrEmpty(departmentFolder))
                return null;

            UserFoldersStructure departmentFolderUFS = null;
            if (!string.IsNullOrEmpty(departmentFolder))
            {
                var departmentFolders = UserFoldersStructures.Singleton.FolderStructureSearch(departmentFolder);
                if (departmentFolders != null && departmentFolders.Count > 0)
                {
                    foreach (var actUFS in departmentFolders)
                    {
                        if (actUFS.FolderName == departmentFolder &&
                            actUFS.ParentFolder == null &&
                            IsUserFolderInSite(actUFS.GetIdString(), structuredSubSite))
                        {
                            departmentFolderUFS = actUFS;
                            break;
                        }
                    }
                }

                if (departmentFolderUFS == null)
                {
                    departmentFolderUFS = new UserFoldersStructure
                    {
                        FolderName = departmentFolder
                    };
                    
                    UserFoldersStructures.Singleton.Insert(ref departmentFolderUFS);
                    StructuredSubSites.Singleton.AddObjectToSite(departmentFolderUFS, structuredSubSite);
                }
            }

            if (!string.IsNullOrEmpty(department))
            {
                UserFoldersStructure departmentUFS = null;
                var departments = UserFoldersStructures.Singleton.FolderStructureSearch(department);
                if (departments != null && departments.Count > 0)
                {
                    foreach (var actUFS in departments)
                    {
                        if (actUFS.FolderName == department
                            && (actUFS.ParentFolder != null
                                ? actUFS.ParentFolder.Compare(departmentFolderUFS)
                                : departmentFolderUFS == null)
                            && IsUserFolderInSite(actUFS.GetIdString(), structuredSubSite))
                        {
                            departmentUFS = actUFS;
                            break;
                        }
                    }
                }

                if (departmentUFS == null)
                {
                    departmentUFS = new UserFoldersStructure
                    {
                        ParentFolder = departmentFolderUFS,
                        FolderName = department
                    };

                    UserFoldersStructures.Singleton.Insert(ref departmentUFS);

                    if (departmentFolderUFS == null)
                        StructuredSubSites.Singleton.AddObjectToSite(departmentUFS, structuredSubSite);
                }

                return departmentUFS;
            }

            return departmentFolderUFS;
        }

        private bool IsUserFolderInSite(string userFolderId, StructuredSubSite structuredSubSite)
        {
            if (structuredSubSite == null)
            {
                var userFolderInASubSite =
                    SelectLinq<StructuredSubSiteObject>(
                        structuredSubSiteObject =>
                            structuredSubSiteObject.ObjectType == ObjectType.UserFoldersStructure &&
                            structuredSubSiteObject.ObjectId == userFolderId &&
                            structuredSubSiteObject.IsReference == false);

                return userFolderInASubSite == null || userFolderInASubSite.Count == 0;
            }

            var userFolderInThisSubSite =
                    SelectLinq<StructuredSubSiteObject>(
                        structuredSubSiteObject =>
                            structuredSubSiteObject.ObjectType == ObjectType.UserFoldersStructure &&
                            structuredSubSiteObject.ObjectId == userFolderId &&
                            structuredSubSiteObject.StructuredSubSite == structuredSubSite &&
                            structuredSubSiteObject.IsReference == false);

            return userFolderInThisSubSite != null && userFolderInThisSubSite.Count > 0;
        }

        private bool _csvImportInProgress;
        private bool EnableStartCSVImport(out bool licenceRestriction)
        {
            licenceRestriction = false;

#if !DEBUG
            string localisedName = string.Empty;
            object value = null;
            if (CgpServer.Singleton.GetLicencePropertyInfo(RequiredLicenceProperties.OfflineImport.ToString(), out localisedName, out value))
            {
                if (value is bool && (bool)value == true)
                {
#endif
            if (!_csvImportInProgress)
            {
                _csvImportInProgress = true;
                return true;
            }
            return false;
#if !DEBUG
                }
            }

            licenceRestriction = true;
            return false;
#endif
        }

        private static void RunImportedPersonCountChanged(ARemotingCallbackHandler remoteHandler, object[] objInput)
        {
            if (objInput == null || objInput.Length != 2) return;

            if (remoteHandler is ImportedPersonCountChangedHandler)
                (remoteHandler as ImportedPersonCountChangedHandler).RunEvent((Guid)objInput[0], (int)objInput[1]);
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idPerson)
        {
            var objects = new List<AOrmObject>();

            var person = GetById(idPerson);
            if (person != null)
            {
                if (person.Cards != null)
                {
                    foreach (var item in person.Cards)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }
            }

            return objects;
        }

        public void RemoveLogins(Guid guidPerson)
        {
            var person = GetObjectForEdit(guidPerson);
            if (person != null)
            {
                person.Logins = null;
                UpdateOnlyInDatabase(person);
                EditEnd(person);
            }
        }

        private readonly Dictionary<Guid, PhotosCache> _photos = new Dictionary<Guid, PhotosCache>();
        public BinaryPhoto GetPhoto(Guid personId)
        {
            lock (_photos)
            {
                PhotosCache photosCache;
                if (_photos.TryGetValue(personId, out photosCache) && photosCache != null)
                    return new BinaryPhoto(photosCache.BinaryPhotoData, photosCache.Extension);
            }

            var person = GetById(personId);
            if (person != null)
            {
                var photoFileName = person.PhotoFileName;
                if (!string.IsNullOrEmpty(photoFileName))
                {
                    var photoFullPath = QuickPath.AssemblyStartupPath + PHOTOS_PATH + @"\" + photoFileName;
                    Stream photoStream = null;
                    try
                    {
                        photoStream = new FileStream(photoFullPath, FileMode.Open, FileAccess.Read);
                        var binaryPhotoData = new byte[photoStream.Length];
                        photoStream.Read(binaryPhotoData, 0, binaryPhotoData.Length);
                        var binaryPhoto = new BinaryPhoto(binaryPhotoData, Path.GetExtension(photoFullPath));

                        SavePhototCache(personId, new PhotosCache(binaryPhoto.BinaryData, binaryPhoto.Extension));

                        return binaryPhoto;
                    }
                    catch { }
                    finally
                    {
                        if (photoStream != null)
                        {
                            try
                            {
                                photoStream.Close();
                            }
                            catch { }
                        }
                    }
                }
            }

            return null;
        }

        public void SavePhoto(Guid personId, BinaryPhoto binaryPhoto)
        {
            if (binaryPhoto != null)
            {
                SavePhototCache(personId, new PhotosCache(binaryPhoto.BinaryData, binaryPhoto.Extension));

                var person = GetObjectForEdit(personId);
                if (person != null)
                {
                    if (!string.IsNullOrEmpty(person.PhotoFileName))
                    {
                        try
                        {
                            var photoToDelete = QuickPath.AssemblyStartupPath + PHOTOS_PATH + @"\" + person.PhotoFileName;
                            if (File.Exists(photoToDelete))
                            {
                                File.Delete(photoToDelete);
                            }
                        }
                        catch { }
                    }

                    var photoFileName = person.IdPerson + "-" + person + binaryPhoto.Extension;
                    person.PhotoFileName = photoFileName;
                    UpdateOnlyInDatabase(person);
                    EditEnd(person);

                    try
                    {
                        if (!Directory.Exists(QuickPath.AssemblyStartupPath + PHOTOS_PATH))
                        {
                            Directory.CreateDirectory(QuickPath.AssemblyStartupPath + PHOTOS_PATH);
                        }

                        var photoFullPath = QuickPath.AssemblyStartupPath + PHOTOS_PATH + @"\" + photoFileName;

                        if (File.Exists(photoFullPath))
                            File.Delete(photoFullPath);
                        using (var photoStream = new MemoryStream())
                        {
                            photoStream.Write(binaryPhoto.BinaryData, 0, binaryPhoto.BinaryData.Length);
                            using (var imageToSave = Image.FromStream(photoStream))
                            {
                                ImageResizeUtility.SaveAsJPG(imageToSave, photoFullPath);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void SavePhototCache(Guid personId, PhotosCache photoCache)
        {
            SafeThread<Guid, PhotosCache>.StartThread(DoSavePhototCache, personId, photoCache);
        }

        private void DoSavePhototCache(Guid personId, PhotosCache photoCache)
        {
            if (photoCache == null)
                return;

            lock (_photos)
            {
                if (_photos.ContainsKey(personId))
                    _photos.Remove(personId);

                _photos.Add(personId, photoCache);

                if (_photosTimeout == null)
                    OnPhotosTimeout(null);
            }
        }

        private ITimer _photosTimeout;
        public bool OnPhotosTimeout(TimerCarrier timer)
        {
            lock (_photos)
            {
                _photosTimeout = null;

                if (_photos.Count > 0)
                {
                    var dateTimeNow = DateTime.Now;
                    DateTime? minTime = null;
                    var photoCachesToDelete = new List<Guid>();
                    foreach (var kvp in _photos)
                    {
                        var IdPerson = kvp.Key;
                        var photoCache = kvp.Value;

                        if (photoCache != null)
                        {
                            var cacheEndTime = photoCache.CacheEndTime;

                            if (cacheEndTime <= dateTimeNow)
                            {
                                photoCachesToDelete.Add(IdPerson);
                            }
                            else if (minTime == null || cacheEndTime < minTime.Value)
                            {
                                minTime = cacheEndTime;
                            }
                        }
                    }

                    if (photoCachesToDelete.Count > 0)
                    {
                        foreach (var IdPersonToDelete in photoCachesToDelete)
                        {
                            _photos.Remove(IdPersonToDelete);
                        }
                    }

                    if (minTime != null)
                        _photosTimeout = TimerManager.Static.StartTimeout((long)(minTime.Value - dateTimeNow).TotalMilliseconds, OnPhotosTimeout);
                }
            }

            return true;
        }

        protected override bool AddCriteriaSpecial(ref ICriteria c, FilterSettings filterSetting)
        {
            if (filterSetting.Column == Person.COLUMNOTHERINFORMATIONFIELDS)
            {
                c = c.Add(Restrictions.Disjunction()
                    .Add(Restrictions.Like("Description", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("Company", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("Role", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("Email", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("PhoneNumber", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("EmployeeNumber", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("RelativeSuperior", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Restrictions.Like("RelativeSuperiorsPhoneNumber", filterSetting.Value as string,
                        MatchMode.Anywhere))
                    .Add(Restrictions.Like("CostCenter", filterSetting.Value as string, MatchMode.Anywhere))
                    .Add(Expression.Sql(string.Format(
                        " {0} in (select {1} from UserFoldersStructureObject join UserFoldersStructure on {2} = {3} where {4} = {5} and {6} like '%{7}%')",
                        Person.COLUMNIDPERSON, UserFoldersStructureObject.ColumnObjectId,
                        UserFoldersStructureObject.ColumnFolder,
                        UserFoldersStructure.COLUMN_ID_USER_FOLDERS_STRUCTURE,
                        UserFoldersStructureObject.ColumnObjectType,
                        (byte) ObjectType.Person,
                        UserFoldersStructure.ColumnFolderName,
                        filterSetting.Value))));
                return true;
            }

            if (filterSetting.Column == Person.COLUMNDEPARTMENT)
            {
                c = c.Add(Expression.Sql(string.Format(
                    " {0} in (select {1} from UserFoldersStructureObject ufo join UserFoldersStructure dep on ufo.{2} = dep.{3} where ufo.{4} = {5} and dep.{6} like '%{7}%')",
                    Person.COLUMNIDPERSON,
                    UserFoldersStructureObject.ColumnObjectId,
                    UserFoldersStructureObject.ColumnFolder,
                    UserFoldersStructure.COLUMN_ID_USER_FOLDERS_STRUCTURE,
                    UserFoldersStructureObject.ColumnObjectType,
                    (byte)ObjectType.Person,
                    UserFoldersStructure.ColumnFolderName,
                    filterSetting.Value)));
                return true;
            }

            if (filterSetting.Column == Person.COLUMNFIRSTNAME || filterSetting.Column == Person.COLUMNSURNAME)
            {
                c =
                    c.Add(
                        Expression.Sql(string.Format("[{0}] COLLATE LATIN1_GENERAL_CI_AI LIKE '%{1}%'",
                            filterSetting.Column, filterSetting.Value)));

                return true;
            }

            return false;
        }

        public IList<IModifyObject> ListModifyPersonsWithCards(out Exception error)
        {
            var listPerson = List(out error);
            IList<IModifyObject> listPersonModifyObj = null;
            if (listPerson != null)
            {
                listPersonModifyObj = new List<IModifyObject>();
                foreach (var person in listPerson)
                {
                    if (person != null && person.Cards != null && person.Cards.Count > 0)
                    {
                        listPersonModifyObj.Add(new PersonModifyObj(person));
                    }
                }

                listPersonModifyObj = listPersonModifyObj.OrderBy(person => person.ToString()).ToList();
            }
            return listPersonModifyObj;
        }

        public bool HasAssignedCards(Guid idPerson)
        {
            var person = GetById(idPerson);
            if (person != null && person.Cards != null && person.Cards.Count > 0)
            {
                return true;
            }

            return false;
        }

        protected override IModifyObject CreateModifyObject(Person person)
        {
            return new PersonModifyObj(person);
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.Person; }
        }

        protected override IEnumerable<Person> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<Person>(
                person =>
                    person.LocalAlarmInstruction != null
                    && person.LocalAlarmInstruction != string.Empty);
        }

        public bool PersonalCodeAlreadyUsed(string codeHashValue)
        {
            return PersonalCodeAlreadyUsed(
                       codeHashValue,
                       Guid.Empty);
        }

        private bool PersonalCodeAlreadyUsed(
            string codeHashValue,
            Guid idEditedPerson)
        {
            var persons = SelectLinq<Person>(
                person =>
                    idEditedPerson != person.IdPerson
                    && codeHashValue == person.PersonalCodeHash);

            return persons != null
                   && persons.Count > 0;
        }

        private bool CodeAlreadyUsed(
            string codeHashValue,
            Guid idEditedPerson)
        {
            return PersonalCodeAlreadyUsed(
                       codeHashValue,
                       idEditedPerson)
                   || CgpServer.Singleton.PluginManager
                           .GetLoadedPlugins().Any(
                                plugin =>
                                plugin.CodeAlreadyUsed(codeHashValue));
        }

        public string GetRandomPersonalCode(Guid idPerson)
        {
            var random = new Random();
            string personalCode;
            var personalCodeStringBuilder = new StringBuilder();

            do
            {
                personalCodeStringBuilder.Clear();

                var randomLength = random.Next(GeneralOptions.Singleton.MinimalCodeLength, GeneralOptions.Singleton.MaximalCodeLength);

                for (var i = 0; i < randomLength; i++)
                {
                    personalCodeStringBuilder.Append(random.Next(0, 9));
                }

                personalCode = personalCodeStringBuilder.ToString();
            }
            while (CodeAlreadyUsed(
                       QuickHashes.GetCRC32String(personalCode),
                       idPerson));

            return personalCode;
        }
    }

    public class PhotosCache
    {
        private const int CACHE_TIME = 120000;

        private DateTime _readintDateTime;
        private readonly byte[] _binaryPhotoData;
        private readonly string _extension;

        public byte[] BinaryPhotoData { get { return _binaryPhotoData; } }
        public string Extension { get { return _extension; } }

        public PhotosCache(byte[] binaryPhotoData, string extension)
        {
            _readintDateTime = DateTime.Now;
            _binaryPhotoData = binaryPhotoData;
            _extension = extension;
        }

        public DateTime CacheEndTime { get { return _readintDateTime.AddMilliseconds(CACHE_TIME); } }
    }

    public interface IImportEventHandler
    {
        void ImportStarted();
        void ImportDone();
    }
}
