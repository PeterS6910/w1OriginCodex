using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.ExportData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.NCAS.Server.ExportData
{
    public class AccessControlListsExportData : IRefExportData<AccessControlList>
    {
        public ICollection<string> Columns =>
                new List<string> { "AccessControlLists",
                                    "CardReaders",
                                    "ColumnDateFrom",
                                    "ColumnDateTo",
                                    "ColumnPerson",
                                    "ColumnPersonID",
                                    "ColumnBD" ,
                                    "ColumnDepartment",
                                    "ColumnTelephone",
                                    "ColumnEmail",
                                    "ColumnEmploymentBeginningDate",
                                    "ColumnEmploymentEndDate"};

        public bool UseSection => true;

        public void AddReferencesToDataTable(AccessControlList ormObj, ref DataTable table, out Exception error)
        {
            error = null;
            var aclPersons = ACLPersons.Singleton.GetACLPersonForACL(ormObj).ToList();

            var crs = AccessControlLists.Singleton.GetCardReaderObjects(ormObj, DateTime.MinValue).ToList();

            try
            {
                DataRow dr = table.NewRow();
                dr[0] = ormObj.ToString();

                for (var r = 0; r < crs.Count; r++)
                {
                    dr[0] = ormObj.ToString();
                    var aclCR = new RefACLCR(crs[r].ToString());
                    aclCR.FillDataRow(1, ref dr);
                    int perAdded = 0;
                    if (crs[r].GetObjectType() == ObjectType.CardReader)
                    {
                        foreach (var aclPerson in aclPersons)
                        {
                            if (aclPerson.Person.Cards != null && aclPerson.Person.Cards.Where(c => c.IsValid).ToList().Count > 0)
                            {
                                var per = new RefACLPerson(aclPerson.Person, aclPerson.DateFrom, aclPerson.DateTo);
                                dr[0] = ormObj.ToString();
                                aclCR.FillDataRow(1, ref dr);
                                per.FillDataRow(2, ref dr);
                                table.Rows.Add(dr);
                                dr = table.NewRow();
                                perAdded++;
                            }
                        }
                    }
                    if (perAdded == 0) //if not person then add Card reader
                    {
                        table.Rows.Add(dr);
                        dr = table.NewRow();
                    }
                }
                if (crs.Count == 0) //if not person and no card reader then add just ACL
                {
                    table.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                error = e;
            }
        }

    }
}
