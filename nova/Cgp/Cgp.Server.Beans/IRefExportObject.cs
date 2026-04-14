using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Server.Beans
{
    public  interface IRefExportObject
    {
        void FillDataRow(int ColumnIndex, ref DataRow row);
        bool Compare(IRefExportObject refPerson);
    }
}
