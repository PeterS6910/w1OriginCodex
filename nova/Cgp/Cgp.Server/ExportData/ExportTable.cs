using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using JetBrains.Annotations;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Server.ExportData
{

    public class ExportTable
    {
        private readonly IRefExportData refExportData;

        public ExportTable(IRefExportData ExportData)
        {
            refExportData = ExportData;
        }


        public DataTable ExportData(out bool bFillSection)
        {
            DataTable _dataTable = new DataTable();
            bFillSection = false;
            if (refExportData != null)
            {
                foreach (var col in refExportData.Columns)
                {
                    _dataTable.Columns.Add(ExportObjects.GetStringFromResources("Contal.Cgp.Server.Localization", col), typeof(string));
                }
                refExportData.AddReferencesToDataTable(ref _dataTable, out Exception error);

                bFillSection = refExportData.UseSection;
            }

            return _dataTable;
        }
    }
    public  class ExportTable<TSingleton, T>
                     where TSingleton : ABaseOrmTable<TSingleton, T>
                     where T : AOrmObject
    {
        public static ABaseOrmTable<TSingleton, T> Singleton => ABaseOrmTable<TSingleton, T>.Singleton;

        private readonly IRefExportData<T> refExportData;

        public ExportTable (IRefExportData<T> ExportData)
        {
            refExportData = ExportData;
        }
        private List<T>GetList(IList<FilterSettings> filterSettings)
        {
            if(filterSettings!=null && filterSettings.Count > 0)
            {
                return Singleton.SelectByCriteria(filterSettings, out Exception error).ToList(); 
            }
            return Singleton.List().ToList();
        }

        public  DataTable ExportData(IList<FilterSettings> filterSettings,
                                                   out bool bFillSection)
        {
            DataTable _dataTable = new DataTable();
            bFillSection = false;
            var ormTable = GetList(filterSettings);
            if (ormTable != null)
            {
                if (refExportData != null)
                {
                    foreach (var col in refExportData.Columns)
                    {
                        _dataTable.Columns.Add(ExportObjects.GetStringFromResources("Contal.Cgp.Server.Localization", col), typeof(string));
                    }

                    int table_count = 0;
                    foreach (var ormObj in ormTable)
                    {
                        table_count++;
                        if (ormObj != null)
                        {
                            refExportData.AddReferencesToDataTable(ormObj, ref _dataTable, out Exception error);
                        }
                        if (refExportData.UseSection && table_count < ormTable.Count)
                        {
                            DataRow dr = _dataTable.NewRow();
                            _dataTable.Rows.Add(dr);
                        }
                    }
                    bFillSection = refExportData.UseSection;
                }
            }

            return _dataTable;
        }
    }
}
