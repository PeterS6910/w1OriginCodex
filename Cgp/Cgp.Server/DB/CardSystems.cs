using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;

using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.DB
{
    public sealed class CardSystems : 
        ABaserOrmTableWithAlarmInstruction<CardSystems, CardSystem>, 
        ICardSystems
    {
        private const string ProxySerialCardSystemName = "Proxy Serial";
        private const string MagneticCardSystemName = "Magnetic";
        private const string MifareSerial4bCardSystemName = "Mifare Serial 4B";
        private const string MifareSerial7bCardSystemName = "Mifare Serial 7B";
        private const string MifareSerial11bCardSystemName = "Mifare Serial 11B";
        private const string DirectSerialCardSystemName = "Direct serial (Proxy/Mifare)";

        private CardSystems()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<CardSystem>())
        {
        }

        public override void CUDSpecial(CardSystem cardSystem, ObjectDatabaseAction objectDatabaseAction)
        {
            if (cardSystem == null)
                return;

            DbWatcher.Singleton.DbObjectChanged(cardSystem, objectDatabaseAction);
        }

        private void DoAfterInsertUpdate(CardSystem cardSystem)
        {
            if (cardSystem.CardData != null)
            {
                if (cardSystem.CardData is MifareSectorData)
                {
                    if ((cardSystem.CardData as MifareSectorData).Id == Guid.Empty)
                        (cardSystem.CardData as MifareSectorData).Id = cardSystem.IdCardSystem;

                    Delete(cardSystem.CardData);
                    Insert(cardSystem.CardData as MifareSectorData, null);
                }
            }
        }

        public override void AfterUpdate(CardSystem newObject, CardSystem oldObjectBeforUpdate)
        {
            if (newObject == null)
                return;

            DoAfterInsertUpdate(newObject);

            base.AfterUpdate(newObject, oldObjectBeforUpdate);
        }

        public override void AfterInsert(CardSystem newObject)
        {
            if (newObject == null)
                return;

            DoAfterInsertUpdate(newObject);

            base.AfterInsert(newObject);
        }

        public override void AfterDelete(CardSystem deletedObject)
        {
            if (deletedObject == null)
                return;

            if (deletedObject.CardData != null)
            {
                if (deletedObject.CardData is MifareSectorData)
                {
                    Delete(deletedObject.CardData);
                }
            }

            base.AfterDelete(deletedObject);
        }

        protected override void LoadObjectsInRelationship(CardSystem obj)
        {
            if (obj.Cards != null)
                obj.Cards = obj.Cards.ToArray();
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(CardSystem.COLUMNNAME, true));
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARD_SYSTEMS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.CardSystemsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.CARD_SYSTEMS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.CardSystemsInsertDeletePerform), login);
        }

        public void CreateDefaultCardSystems()
        {
            try
            {
                var cardSystemNames = new HashSet<string>(List().Select(cardSystem => cardSystem.Name));

                if (!cardSystemNames.Contains(ProxySerialCardSystemName))
                {
                    var cardSystem = new CardSystem
                    {
                        Name = ProxySerialCardSystemName,
                        LengthCardData = 14,
                        LengthCompanyCode = 0,
                        CardType = (byte)CardType.Proxy,
                        CardSubType = (byte)CardSubType.ProxySerialNumber,
                        CompanyCode = string.Empty,
                        CardSystemNumber = GetCardSystemNumber()
                    };
                    Insert(ref cardSystem);
                }

                if (!cardSystemNames.Contains(MagneticCardSystemName))
                {
                    var cardSystem = new CardSystem
                    {
                        Name = MagneticCardSystemName,
                        LengthCardData = 38,
                        LengthCompanyCode = 0,
                        CardType = (byte)CardType.Magnetic,
                        CardSubType = (byte)CardSubType.None,
                        CompanyCode = string.Empty,
                        CardSystemNumber = GetCardSystemNumber()
                    };
                    Insert(ref cardSystem);
                }

                if (!cardSystemNames.Contains(MifareSerial4bCardSystemName))
                {
                    var cardSystem = new CardSystem
                    {
                        Name = MifareSerial4bCardSystemName,
                        LengthCardData = 12,
                        LengthCompanyCode = 0,
                        CardType = (byte)CardType.Mifare,
                        CardSubType = (byte)CardSubType.MifareSerialNumberWithoutPrefix,
                        CompanyCode = string.Empty,
                        CardSystemNumber = GetCardSystemNumber()
                    };
                    Insert(ref cardSystem);
                }

                if (!cardSystemNames.Contains(MifareSerial7bCardSystemName))
                {
                    var cardSystem = new CardSystem
                    {
                        Name = MifareSerial7bCardSystemName,
                        LengthCardData = 20,
                        LengthCompanyCode = 0,
                        CardType = (byte)CardType.Mifare,
                        CardSubType = (byte)CardSubType.MifareSerialNumberWithoutPrefix,
                        CompanyCode = string.Empty,
                        CardSystemNumber = GetCardSystemNumber()
                    };
                    Insert(ref cardSystem);
                }

                if (!cardSystemNames.Contains(MifareSerial11bCardSystemName))
                {
                    var cardSystem = new CardSystem
                    {
                        Name = MifareSerial11bCardSystemName,
                        LengthCardData = 30,
                        LengthCompanyCode = 0,
                        CardType = (byte)CardType.Mifare,
                        CardSubType = (byte)CardSubType.MifareSerialNumberWithoutPrefix,
                        CompanyCode = string.Empty,
                        CardSystemNumber = GetCardSystemNumber()
                    };
                    Insert(ref cardSystem);
                }

                if (!cardSystemNames.Contains(DirectSerialCardSystemName))
                {
                    var cardSystem = new CardSystem
                    {
                        Name = DirectSerialCardSystemName,
                        LengthCardData = 30,
                        LengthCompanyCode = 0,
                        CardType = (byte)CardType.DirectSerial,
                        CardSubType = (byte)CardSubType.None,
                        CompanyCode = string.Empty,
                        CardSystemNumber = GetCardSystemNumber()
                    };
                    Insert(ref cardSystem);
                }
            }
            catch
            { }
        }

        protected override void LoadObjectsInRelationshipGetById(CardSystem obj)
        {
            if (obj == null)
                return;

            if (obj.CardType == (byte)CardType.Mifare &&
                (obj.CardSubType == (byte)CardSubType.MifareSectorReadinWithMAD || 
                    obj.CardSubType == (byte)CardSubType.MifareStandardSectorReadin))
            {
                obj.CardData = base.GetById<MifareSectorData>(obj.IdCardSystem);
            }
        }

        public ICollection<MifareSectorData> ListMifareSectorSmartData()
        {
            return base.List<MifareSectorData>();
        }

        public bool IsMifareSectorDataInCollision(MifareSectorData sectorData)
        {
            if (sectorData == null || sectorData.SectorsInfo == null)
                return false;

            var aKeysUsed = new HashSet<string>();
            foreach (var sectorInfo in sectorData.SectorsInfo.Where(si => si.Bank != null))
            {
                aKeysUsed.Add(
                    sectorInfo.InheritAKey
                        ? ByteDataCarrier.HexDump(sectorData.GeneralAKey)
                        : ByteDataCarrier.HexDump(sectorInfo.AKey));
            }

            var sectorsData = ListMifareSectorSmartData();
            foreach (var sd in sectorsData.Where(sd => !sd.Id.Equals(sectorData.Id)))
            {
                if (sd.SectorsInfo == null)
                    continue;

                foreach (var si in sd.SectorsInfo.Where(secInfo => secInfo.Bank != null))
                {
                    if (si.InheritAKey)
                    {
                        if (aKeysUsed.Contains(ByteDataCarrier.HexDump(sd.GeneralAKey)))
                            return true;
                    }
                    else if (aKeysUsed.Contains(ByteDataCarrier.HexDump(si.AKey)))
                        return true;
                }
            }

            return false;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<CardSystem> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (single)
                {
                    linqResult =
                        SelectLinq<CardSystem>(
                            cardSystem => cardSystem.Name.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult =
                        SelectLinq<CardSystem>(
                            cardSystem =>
                                cardSystem.Name.IndexOf(name) >= 0 ||
                                cardSystem.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cs => cs.Name).ToList();
                foreach (var cardSystem in linqResult)
                {
                    resultList.Add(cardSystem);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<CardSystem> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = 
                    SelectLinq<CardSystem>(
                        cardSystem =>
                            cardSystem.Name.IndexOf(name) >= 0 ||
                            cardSystem.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<CardSystem> linqResult = 
                string.IsNullOrEmpty(name) 
                    ? List()
                    : SelectLinq<CardSystem>(
                        cardSystem => cardSystem.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<CardSystem> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cs => cs.Name).ToList();
                foreach (var cardSystem in linqResult)
                {
                    resultList.Add(cardSystem);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<CardSystemShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listCardSystem = SelectByCriteria(filterSettings, out error);
            ICollection<CardSystemShort> result = new List<CardSystemShort>();
            if (listCardSystem != null)
            {
                foreach (var cardSystem in listCardSystem)
                {
                    result.Add(new CardSystemShort(cardSystem));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listCardSystem = List(out error);
            IList<IModifyObject> listCardSystemModifyObj = null;
            if (listCardSystem != null)
            {
                listCardSystemModifyObj = new List<IModifyObject>();
                foreach (var cardSystem in listCardSystem)
                {
                    listCardSystemModifyObj.Add(new CardSystemModifyObj(cardSystem));
                }
                listCardSystemModifyObj = listCardSystemModifyObj.OrderBy(cardSystem => cardSystem.ToString()).ToList();
            }
            return listCardSystemModifyObj;
        }

        public byte GetCardSystemNumber()
        {
            var listCardSystem = List();
            var listInt = new List<int>();
            foreach (var cs in listCardSystem)
            {
                listInt.Add(cs.CardSystemNumber);
            }

            for (byte i = 1; i < 200; i++)
            {
                if (!listInt.Contains(i))
                    return i;
            }
            return 255;
        }

        public CardSystem GetCardSystemFromNumber(byte cardSystemNumber)
        {
            var listCardSystem = List();
            foreach (var cs in listCardSystem)
            {
                if (cs.CardSystemNumber == cardSystemNumber)
                    return cs;
            }
            return null;
        }

        public CardSystem GetCardSystemByCard(string fullCardNumber)
        {
            if (String.IsNullOrEmpty(fullCardNumber)) return null;
            var listCardSystems = List();
            foreach (var cs in listCardSystems)
            {
                if (cs.LengthCardData == fullCardNumber.Length)
                {
                    var initalCardString = cs.GetFullCompanyCode();
                    if (fullCardNumber.Substring(0, initalCardString.Length) == initalCardString)
                    {
                        var csOut = GetObjectById(cs.IdCardSystem);
                        return csOut;
                    }
                }
            }
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CardSystem; }
        }

        protected override IEnumerable<CardSystem> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<CardSystem>(
                cardSystem =>
                    cardSystem.LocalAlarmInstruction != null
                    && cardSystem.LocalAlarmInstruction != string.Empty);
        }
    }
}
