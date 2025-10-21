using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class RelationshipGlobalAlarmInstructionObjects :
        ABaseOrmTable<RelationshipGlobalAlarmInstructionObjects, RelationshipGlobalAlarmInstructionObject>
    {
        private readonly HashSet<IdAndObjectType> _objectsWithGlobalAlarmInstructions = new HashSet<IdAndObjectType>();
        private bool _objectsWithGlobalAlarmInstructionsWasInitialized;

        private RelationshipGlobalAlarmInstructionObjects() : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public IList<AOrmObject> GetReferencedObjects(Guid IdGlobalAlarmInstruction)
        {
            ICollection<RelationshipGlobalAlarmInstructionObject> relationshipObjects = SelectLinq<RelationshipGlobalAlarmInstructionObject>(ro => ro.IdGlobalAlarmInstruction == IdGlobalAlarmInstruction);
            if (relationshipObjects != null && relationshipObjects.Count > 0)
            {
                IList<AOrmObject> referencedObjects = new List<AOrmObject>();
                foreach (RelationshipGlobalAlarmInstructionObject relationshipObject in relationshipObjects)
                {
                    switch (relationshipObject.ObjectType)
                    {
                        case (byte)ObjectType.Person:
                            Person person = Persons.Singleton.GetById(new Guid(relationshipObject.ObjectId));
                            referencedObjects.Add(person);
                            break;
                        case (byte)ObjectType.Login:
                            Login login = Logins.Singleton.GetById(new Guid(relationshipObject.ObjectId));
                            referencedObjects.Add(login);
                            break;
                        case (byte)ObjectType.Card:
                            Card card = Cards.Singleton.GetById(new Guid(relationshipObject.ObjectId));
                            referencedObjects.Add(card);
                            break;
                        case (byte)ObjectType.CardSystem:
                            CardSystem cardSystem = CardSystems.Singleton.GetById(new Guid(relationshipObject.ObjectId));
                            referencedObjects.Add(cardSystem);
                            break;
                        case (byte)ObjectType.CisNG:
                            CisNG cisNG = CisNGs.Singleton.GetById(new Guid(relationshipObject.ObjectId));
                            referencedObjects.Add(cisNG);
                            break;
                        case (byte)ObjectType.CisNGGroup:
                            CisNGGroup cisNGGroup = CisNGGroups.Singleton.GetById(new Guid(relationshipObject.ObjectId));
                            referencedObjects.Add(cisNGGroup);
                            break;
                    }
                }

                return referencedObjects;
            }

            return null;
        }

        public bool AddReference(Guid IdGlobalAlarmInstruction, ObjectType objectType, string objectId, out Exception exception)
        {
            var relationshipObject = new RelationshipGlobalAlarmInstructionObject
            {
                IdGlobalAlarmInstruction = IdGlobalAlarmInstruction,
                ObjectType = (byte)objectType,
                ObjectId = objectId
            };

            var result = Insert(relationshipObject, out exception);

            if (result)
                lock (_objectsWithGlobalAlarmInstructions)
                    if (_objectsWithGlobalAlarmInstructionsWasInitialized)
                        _objectsWithGlobalAlarmInstructions.Add(
                            new IdAndObjectType(
                                objectId,
                                objectType));

            return result;
        }

        public void RemoveReference(Guid IdGlobalAlarmInstruction, ObjectType objectType, string objectId)
        {
            bool result = false;

            ICollection<RelationshipGlobalAlarmInstructionObject> relationshipObjects =
                SelectLinq<RelationshipGlobalAlarmInstructionObject>(
                    ro =>
                        ro.IdGlobalAlarmInstruction == IdGlobalAlarmInstruction && ro.ObjectType == (byte) objectType &&
                        ro.ObjectId == objectId);
            if (relationshipObjects != null && relationshipObjects.Count > 0)
            {
                foreach (RelationshipGlobalAlarmInstructionObject relationshipObject in relationshipObjects)
                {
                    if (relationshipObject != null)
                    {
                        result = Delete(relationshipObject);
                    }
                }
            }

            if (result)
                lock (_objectsWithGlobalAlarmInstructions)
                    if (_objectsWithGlobalAlarmInstructionsWasInitialized)
                    {
                        if (ExistsGlobalAlarmInstructionsForObjectInDatabase(
                            ObjectType,
                            objectId))
                        {
                            return;
                        }

                        _objectsWithGlobalAlarmInstructions.Remove(
                            new IdAndObjectType(
                                objectId,
                                objectType));
                    }
        }

        public List<GlobalAlarmInstruction> GetGlobalAlarmInstructionsForObject(ObjectType objectType, string objectId)
        {
            var byteObjectType = (byte) objectType;

            ICollection<RelationshipGlobalAlarmInstructionObject> relationshipObjects = SelectLinq<RelationshipGlobalAlarmInstructionObject>(ro => ro.ObjectType == byteObjectType && ro.ObjectId == objectId);
            if (relationshipObjects != null && relationshipObjects.Count > 0)
            {
                var globalAlarmInstructions = new List<GlobalAlarmInstruction>();
                foreach (RelationshipGlobalAlarmInstructionObject relationshipObject in relationshipObjects)
                {
                    if (relationshipObject != null)
                    {
                        GlobalAlarmInstruction globalAlarmInstruction = GlobalAlarmInstructions.Singleton.GetById(relationshipObject.IdGlobalAlarmInstruction);
                        if (globalAlarmInstruction != null)
                            globalAlarmInstructions.Add(globalAlarmInstruction);
                    }
                }

                return globalAlarmInstructions.OrderBy(globalAlarmInstruction => globalAlarmInstruction.ToString()).ToList(); ;
            }

            return null;
        }

        public bool ExistsGlobalAlarmInstructionsForObject(ObjectType objectType, string objectId)
        {
            lock (_objectsWithGlobalAlarmInstructions)
            {
                if (!_objectsWithGlobalAlarmInstructionsWasInitialized)
                    InitializeObjectsWithGlobalAlarmInstructions();

                return _objectsWithGlobalAlarmInstructions.Contains(
                    new IdAndObjectType(
                        objectId,
                        objectType));
            }
        }

        private void InitializeObjectsWithGlobalAlarmInstructions()
        {
            var globalAlarmInstructions = List();

            if (globalAlarmInstructions == null)
                return;

            foreach (var globalAlarmInstruction in globalAlarmInstructions)
            {
                _objectsWithGlobalAlarmInstructions.Add(
                    new IdAndObjectType(
                        globalAlarmInstruction.ObjectId,
                        (ObjectType)globalAlarmInstruction.ObjectType));
            }

            _objectsWithGlobalAlarmInstructionsWasInitialized = true;
        }

        private bool ExistsGlobalAlarmInstructionsForObjectInDatabase(ObjectType objectType, string objectId)
        {
            var byteObjectType = (byte)objectType;

            ICollection<RelationshipGlobalAlarmInstructionObject> relationshipObjects =
                SelectLinq<RelationshipGlobalAlarmInstructionObject>(
                    ro =>
                        ro.ObjectType == byteObjectType
                        && ro.ObjectId == objectId);

            return relationshipObjects != null && relationshipObjects.Count > 0;
        }

        public List<Guid> GetReferencedObjects(Guid IdGlobalAlarmInstruction, ObjectType objectType)
        {
            ICollection<RelationshipGlobalAlarmInstructionObject> relationshipObjects = SelectLinq<RelationshipGlobalAlarmInstructionObject>(ro => ro.IdGlobalAlarmInstruction == IdGlobalAlarmInstruction && ro.ObjectType == (byte)objectType);
            if (relationshipObjects != null && relationshipObjects.Count > 0)
            {
                var referencedObjects = new List<Guid>();
                foreach (RelationshipGlobalAlarmInstructionObject relationshipObject in relationshipObjects)
                {
                    if (relationshipObject != null)
                    {
                        string strId = relationshipObject.ObjectId;
                        var id = new Guid(strId);
                        referencedObjects.Add(id);
                    }
                }

                return referencedObjects;
            }

            return null;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.RelationshipGlobalAlarmInstructionObject; }
        }
    }
}
