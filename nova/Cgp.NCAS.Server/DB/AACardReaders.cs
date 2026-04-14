using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AACardReaders :
        ANcasBaseOrmTable<AACardReaders, AACardReader>, 
        IAACardReaders
    {
        private AACardReaders()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<AACardReader>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.CARD_READERS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.CardReadersInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.CARD_READERS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.CardReadersInsertDeletePerform), login);
        }

        protected override void LoadObjectsInRelationship(AACardReader obj)
        {
            if (obj.AlarmArea != null)
                obj.AlarmArea = AlarmAreas.Singleton.GetById(obj.AlarmArea.IdAlarmArea);

            if (obj.CardReader != null)
                obj.CardReader = CardReaders.Singleton.GetById(obj.CardReader.IdCardReader);
        }

        public AlarmArea GetImplicitAlarmArea(CardReader cardReader)
        {
            ICollection<AACardReader> aaCardReaders = SelectLinq<AACardReader>(aaCardReader => aaCardReader.CardReader == cardReader && aaCardReader.PermanentlyUnlock == false);

            if (aaCardReaders != null && aaCardReaders.Count > 0 && aaCardReaders.ToList()[0].AlarmArea != null)
            {
                return AlarmAreas.Singleton.GetById(aaCardReaders.ToList()[0].AlarmArea.IdAlarmArea);
            }

            return null;
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idAAcardReader)
        {
            AACardReader aaCardreader = GetById(idAAcardReader);
            if (ccus != null && aaCardreader != null)
            {
                if (aaCardreader.CardReader != null)
                {
                    CardReaders.Singleton.GetParentCCU(ccus, aaCardreader.CardReader.IdCardReader);
                }
            }
        }

        public void SetImplicitAAToCardReader(Guid guidAlarmArea)
        {
            AlarmArea alarmArea = AlarmAreas.Singleton.GetById(guidAlarmArea);
            if (alarmArea == null) return;
            foreach (AACardReader aaCardReader in alarmArea.AACardReaders)
            {
                if (!aaCardReader.PermanentlyUnlock)
                {
                    CancelPermanentlyUnlockinCardReaders(aaCardReader.CardReader, guidAlarmArea);
                }
            }
        }

        private void CancelPermanentlyUnlockinCardReaders(CardReader cardReader, Guid guidAlarmArea)
        {
            ICollection<AACardReader> aaCardReaders = SelectLinq<AACardReader>(aaCardReader => aaCardReader.CardReader == cardReader);
            if (aaCardReaders == null) return;
            foreach (AACardReader aaCr in aaCardReaders)
            {
                if (!aaCr.PermanentlyUnlock && aaCr.AlarmArea.IdAlarmArea != guidAlarmArea)
                {
                    try
                    {
                        AACardReader editAaCardReader = GetObjectForEdit(aaCr.IdAACardReader);
                        if (editAaCardReader != null)
                        {
                            editAaCardReader.PermanentlyUnlock = true;
                            Update(editAaCardReader);
                        }
                    }
                    catch { }
                }
            }
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.AACardReader; }
        }
    }
}
