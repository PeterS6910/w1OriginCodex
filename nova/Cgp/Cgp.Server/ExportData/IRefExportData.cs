using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Server.ExportData
{

    public interface IRefExportData
    {
        ICollection<string> Columns { get; }
        bool UseSection { get; }
        void AddReferencesToDataTable(ref DataTable table, out Exception error);
    }

    public  interface  IRefExportData<T> 
                          where T : AOrmObject
    {
        ICollection<string> Columns { get; }
        bool UseSection { get; }
        void AddReferencesToDataTable(T ormObj, ref DataTable table, out Exception error);
    }
}
