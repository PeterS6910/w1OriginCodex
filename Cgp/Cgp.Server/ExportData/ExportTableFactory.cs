using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Server.ExportData
{
    public  static class ExportTableFactory
    {
        public enum Type  { _Card } ;

        public static DataTable Generate(Type type, IList<FilterSettings> filterSettings,
                                                   out bool bFillSection)
        {
            switch(type)
            {
                case Type._Card:
                    return new  ExportTable<Cards, Card>(new CardsExportData())
                                      .ExportData(filterSettings, out bFillSection);
                default:
                    bFillSection = false;
                    return new DataTable();
            }
        }
    }
}
