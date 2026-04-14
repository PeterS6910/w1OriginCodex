using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface ICardReaders : IBaseOrmTable<CardReader>
    {
        OnlineState GetOnlineStates(Guid guidCardReader);
        string GetLastCard(Guid guidCardReader);
        string GetProtocolVersion(CardReader cardReader);
        string GetFirmwareVersion(CardReader cardReader);
        string GetHardwareVersion(CardReader cardReader);
        bool? GetHasKeyboard(Guid cardReaderId);
        bool? GetHasDisplay(Guid cardReaderId);
        string GetProtocolMajor(CardReader cardReader);
        CardReaderSceneType GetCardReaderCommand(Guid guidCardReader);
        bool IsUsedInDoorEnvironment(CardReader cardReader);

        ICollection<CardReaderShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);

        ICollection<CardReaderShort> ShortSelectByCriteria(
            out Exception error, 
            LogicalOperators filterJoinOperator, 
            params ICollection<FilterSettings>[] filterSettings);

        bool IsUsedInDoorEnvironmentByGuid(Guid idCardReader);

        IList<IModifyObject> ListModifyObjects(bool allowedCardReadersAssignegToMultiDoors, out Exception error);

        IList<IModifyObject> ListModifyObjects(
            bool allowedCardReadersAssignegToMultiDoors,
            out Exception error,
            Guid guidImplicitCCU);

        IList<IModifyObject> GetSpecialOutputs(Guid idCCU);

        void Reset(Guid cardReaderId);
        Guid GetParentCCU(Guid idCardreader);

        void GetActualSecurityLevel(Guid cardReaderId, out SecurityLevel? actualSecurityLeve,
            out SecurityLevel4SLDP? securityLevelStzSdp);

        ICollection<IModifyObject> ModifyObjectsSelectByCriteria(
            ICollection<FilterSettings> filterSettings,
            out Exception err);

        ICollection<IModifyObject> GetAPBZAssignableCRModifyObjects(
            Guid guidCCU,
            out IDictionary<Guid, bool> isCardReaderFromMinimalDe,
            out Exception err);

        bool GetBlockedState(Guid idCardReader);

        void Unblock(
            Guid idCcu,
            Guid idCardReader);

        void Unblock(Guid idCcu);

        bool IsFromMinimalDe(Guid idCardReader);
    }
}


