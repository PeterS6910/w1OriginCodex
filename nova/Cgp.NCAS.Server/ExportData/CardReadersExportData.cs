using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
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
    public class CardReadersExportData : IRefExportData<CardReader>
    {
        public ICollection<string> Columns => new List<string> {"CardReaders",
                                                                "ColumnPerson",
                                                                "ColumnPersonID",
                                                                "ColumnBD" ,
                                                                "ColumnDepartment",
                                                                "ColumnTelephone",
                                                                "ColumnEmail",
                                                                "ColumnEmploymentBeginningDate",
                                                                "ColumnEmploymentEndDate"};
        public bool UseSection => true;

        private IList<IRefExportObject> GetReferencedPersons(CardReader ormObj, out Exception error)
        {
            List<IRefExportObject> refPersons = new List<IRefExportObject>();
            error = null;
            try
            {
                var aclList = ACLSettings.Singleton.UsedLikeCardReaderObject(ormObj.IdCardReader, ObjectType.CardReader);

                if (aclList != null)
                {
                    foreach (var acl in aclList)
                    {
                        if (acl != null)
                        {
                            var persons = ACLPersons.Singleton.GetPersonsForACL(acl);
                            if (persons != null)
                            {
                                foreach (var per in persons)
                                {
                                    if (per.Cards != null && per.Cards.Where(c => c.IsValid).ToList().Count > 0)
                                    {
                                        var refPerson = new RefCRPerson(per);

                                        if (!refPersons.Exists(p => p.Compare(refPerson)))
                                            refPersons.Add(refPerson);
                                    }
                                }
                            }
                        }
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(ormObj.IdCardReader,
                    ObjectType.CardReader);
                if (usedInAz != null)
                {
                    foreach (var per in usedInAz)
                    {
                        if (per.Cards != null && per.Cards.Where(c => c.IsValid).ToList().Count > 0)
                        {
                            var refPerson = new RefCRPerson(per);
                            refPerson.Person.Department = UserFoldersStructures.Singleton.GetPersonDepartment(refPerson.Person.GetIdString());
                            if (!refPersons.Exists(p => p.Compare(refPerson)))
                                refPersons.Add(refPerson);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            return refPersons;
        }

        public void AddReferencesToDataTable(CardReader ormObj, ref DataTable table, out Exception error)
        {
            foreach (var refPerson in GetReferencedPersons(ormObj, out error))
            {
                DataRow dr = table.NewRow();
                dr[0]=ormObj.ToString();
                refPerson.FillDataRow(1, ref dr);
                table.Rows.Add(dr);
            }
        }
    }
}
