using System;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using System.Data;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICards : IBaseOrmTable<Card>
    {
        ICollection<CardShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
        IList<IModifyObject> ListModifyObjects(Guid[] except, bool allowRelatedCards, out Exception error);
        IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ModifyObjectsFormPersonAddCard(Guid idPerson, out Exception error);
        IList<Card> GetCardsByListGuids(IList<object> idCards);
        bool ImportCards(Guid formIdentification, List<ImportCardData> importCardsData, CSVImportType importType, Guid guidCardSystem, out List<CSVImportCard> csvImportCards, out bool licenceRestriction, out int importedCardsCount);
        Card GetCardByFullNumber(string cardNumber);
        bool SetCardsToPerson(IList<Guid> listIdCards, Guid idPerson);
        bool RemoveCardsFromPerson(IList<Guid> listIdCards, Guid idPerson);

        DataTable ExportCards(IList<FilterSettings> filterSettings,   out bool bFillSection);
    }
}
