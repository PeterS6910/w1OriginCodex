using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using NHibernate.Criterion;
using NHibernate;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class GlobalAlarmInstructions : 
        ABaseOrmTable<GlobalAlarmInstructions, GlobalAlarmInstruction>, 
        IGlobalAlarmInstructions
    {
        private GlobalAlarmInstructions() : base(null)
        {
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(GlobalAlarmInstruction.COLUMN_NAME, true));
        }

        public override bool HasAccessView(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccessesForGroup(LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.GlobalAlarmInstructionsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccessesForGroup(LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.GlobalAlarmInstructionsInsertDeletePerform), login);
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                if (idObj is Guid)
                {
                    var idGlobalAlarmInstruction = (Guid)idObj;
                    return RelationshipGlobalAlarmInstructionObjects.Singleton.GetReferencedObjects(idGlobalAlarmInstruction).OrderBy(orm => orm.ToString()).ToList();
                }
            }
            catch { }

            return null;
        }

        public bool AddReference(Guid IdGlobalAlarmInstruction, ObjectType objectType, string objectId, out Exception exception)
        {
            return RelationshipGlobalAlarmInstructionObjects.Singleton.AddReference(IdGlobalAlarmInstruction, objectType, objectId, out exception);
        }

        public void RemoveReference(Guid IdGlobalAlarmInstruction, ObjectType objectType, string objectId)
        {
            RelationshipGlobalAlarmInstructionObjects.Singleton.RemoveReference(IdGlobalAlarmInstruction, objectType, objectId);
        }

        public List<GlobalAlarmInstruction> GetGlobalAlarmInstructionsForObject(ObjectType objectType, string objectId)
        {
            return RelationshipGlobalAlarmInstructionObjects.Singleton.GetGlobalAlarmInstructionsForObject(objectType, objectId);
        }

        public bool ExistsGlobalAlarmInstructionsForObject(
            ObjectType objectType,
            string objectId)
        {
            return RelationshipGlobalAlarmInstructionObjects.Singleton.ExistsGlobalAlarmInstructionsForObject(
                objectType,
                objectId);
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<GlobalAlarmInstruction> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                linqResult =
                    single
                        ? SelectLinq<GlobalAlarmInstruction>(
                            globalAlarmInstruction =>
                                globalAlarmInstruction.Name.IndexOf(name) >= 0)
                        : SelectLinq<GlobalAlarmInstruction>(
                            globalAlarmInstruction =>
                                globalAlarmInstruction.Name.IndexOf(name) >= 0 ||
                                globalAlarmInstruction.Instructions.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(globalAlarmInstruction => globalAlarmInstruction.ToString()).ToList();
                foreach (var globalAlarmInstruction in linqResult)
                {
                    resultList.Add(globalAlarmInstruction);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<GlobalAlarmInstruction> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<GlobalAlarmInstruction>(
                        globalAlarmInstruction =>
                            globalAlarmInstruction.Name.IndexOf(name) >= 0 ||
                            globalAlarmInstruction.Instructions.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult = 
                string.IsNullOrEmpty(name)
                    ? List() 
                    : SelectLinq<GlobalAlarmInstruction>(
                        globalAlarmInstruction => globalAlarmInstruction.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<GlobalAlarmInstruction> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(globalAlarmInstruction => globalAlarmInstruction.Name).ToList();
                foreach (var globalAlarmInstruction in linqResult)
                {
                    resultList.Add(globalAlarmInstruction);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<GlobalAlarmInstructionShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listGlobalAlarmInstructions = SelectByCriteria(filterSettings, out error);
            ICollection<GlobalAlarmInstructionShort> result = new List<GlobalAlarmInstructionShort>();
            if (listGlobalAlarmInstructions != null)
            {
                foreach (var globalAlarmInstruction in listGlobalAlarmInstructions)
                {
                    if (globalAlarmInstruction != null)
                        result.Add(new GlobalAlarmInstructionShort(globalAlarmInstruction));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listGlobalAlarmInstructiond = List(out error);
            var listGlobalAlarmInstructiondModifyObj = new List<IModifyObject>();
            if (listGlobalAlarmInstructiond != null && listGlobalAlarmInstructiond.Count > 0)
            {
                foreach (var globalAlarmInstruction in listGlobalAlarmInstructiond)
                {
                    listGlobalAlarmInstructiondModifyObj.Add(new GlobalAlarmInstructionModifyObj(globalAlarmInstruction));
                }
                listGlobalAlarmInstructiondModifyObj = listGlobalAlarmInstructiondModifyObj.OrderBy(globalAlarmInstruction => globalAlarmInstruction.ToString()).ToList();
            }
            return listGlobalAlarmInstructiondModifyObj;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.GlobalAlarmInstruction; }
        }
    }
}
