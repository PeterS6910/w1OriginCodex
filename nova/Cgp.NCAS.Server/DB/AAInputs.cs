using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AAInputs :
        ANcasBaseOrmTable<AAInputs, AAInput>, 
        IAAInputs
    {
        private AAInputs()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<AAInput>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.INPUTS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.InputsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.INPUTS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.InputsInsertDeletePerform), login);
        }

        protected override void LoadObjectsInRelationshipGetById(AAInput obj)
        {
            if (obj.Input != null)
                obj.Input = SelectLinq<Input>(
                    input =>
                        input.IdInput == obj.Input.IdInput)
                    .FirstOrDefault();

            if (obj.AlarmArea != null)
                obj.AlarmArea = SelectLinq<AlarmArea>(
                    alarmArea =>
                        alarmArea.IdAlarmArea == obj.AlarmArea.IdAlarmArea)
                    .FirstOrDefault();
        }

        public IList<AlarmArea> GetAlarmAreaByInput(Input input)
        {
            try
            {
                if (input == null) return null;

                ICollection<AAInput> aaInputList = SelectLinq<AAInput>(aaInput => aaInput.Input == input);
                if (aaInputList != null && aaInputList.Count > 0)
                {
                    IList<AlarmArea> result = new List<AlarmArea>();
                    foreach (AAInput aaInput in aaInputList)
                    {
                        result.Add(aaInput.AlarmArea);
                    }
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idAAInput)
        {
            AAInput aaInput = GetById(idAAInput);
            if (ccus != null && aaInput != null)
            {
                //if (aaInput.AlarmArea != null)
                //{
                //   AlarmAreas.Singleton.GetParentCCU(ccus, aaInput.AlarmArea.IdAlarmArea);
                //}
                if (aaInput.Input != null)
                {
                    Inputs.Singleton.GetParentCCU(ccus, aaInput.Input.IdInput);
                }
            }
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.AAInput; }
        }
    }
}
