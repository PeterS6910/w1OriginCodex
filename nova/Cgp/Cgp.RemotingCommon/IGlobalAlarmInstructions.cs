using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IGlobalAlarmInstructions : IBaseOrmTable<GlobalAlarmInstruction>
    {
        ICollection<GlobalAlarmInstructionShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);

        bool AddReference(Guid IdGlobalAlarmInstruction, ObjectType objectType, string objectId, out Exception exception);
        void RemoveReference(Guid IdGlobalAlarmInstruction, ObjectType objectType, string objectId);
        List<GlobalAlarmInstruction> GetGlobalAlarmInstructionsForObject(ObjectType objectType, string objectId);
        bool ExistsGlobalAlarmInstructionsForObject(ObjectType objectType, string objectId);
    }
}
