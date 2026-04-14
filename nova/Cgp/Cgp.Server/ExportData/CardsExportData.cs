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
    public class CardsExportData : IRefExportData<Card>
    {
        public ICollection<string> Columns => new List<string>  {"ColumnCardNumber",
                                                                 "ColumnCardFullNumber",
                                                                  "ColumnPerson",
                                                                  "ColumnPersonID",
                                                                  "ColumnDateFrom",
                                                                  "ColumnDateTo",
                                                                  "ColumnState",
                                                                  "ColumnDescription"};
        public bool UseSection => false;

        public void AddReferencesToDataTable(Card ormObj, ref DataTable table, out Exception error)
        {
            error = null;
            DataRow dr = table.NewRow();
            Person person=null;
            if( ormObj.Person != null)
                person= Persons.Singleton.GetObjectById(ormObj.Person.IdPerson);

            var refPerson = new RefCardPerson( ormObj, person != null ? person.ToString() : " - ", person != null ? person.Identification : null);
            refPerson.FillDataRow(0, ref dr);
            table.Rows.Add(dr);
        }
    }
}
