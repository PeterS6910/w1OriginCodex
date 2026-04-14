using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.ExportData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.NCAS.Server.ExportData
{
    public static class ExportTableNCASFactory
    {
        public enum Type { _AccessControlList, _CardReader, _PersonAttribute};

        public static DataTable Generate(Type type, IList<FilterSettings> filterSettings,
                                                    out bool bFillSection)
        {
            switch (type)
            {
                case Type._AccessControlList:
                    return new ExportTable<AccessControlLists, AccessControlList>(new AccessControlListsExportData())
                                        .ExportData(filterSettings, out bFillSection);
                case Type._CardReader:
                return new ExportTable<CardReaders, CardReader>(new CardReadersExportData())
                                    .ExportData(filterSettings, out bFillSection);
                case Type._PersonAttribute:
                    DateTime dtFrom = DateTime.Now.AddDays(-10);
                    return new ExportTable<Persons, Cgp.Server.Beans.Person>(new PersonAttributeExportData(dtFrom))
                                   .ExportData(null, out bFillSection);
                default:
                    bFillSection=false;
                    return new DataTable();
            }
        }
    }
}
