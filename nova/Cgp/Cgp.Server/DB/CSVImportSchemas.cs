using System.Collections.Generic;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class CSVImportSchemas : 
        ABaseOrmTable<CSVImportSchemas, CSVImportSchema>, 
        ICSVImportSchemas
    {
        private CSVImportSchemas() : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return true;
        }

        public override bool HasAccessInsert(Login login)
        {
            return true;
        }

        public override bool HasAccessUpdate(Login login)
        {
            return true;
        }

        public override bool HasAccessDelete(Login login)
        {
            return true;
        }

        protected override void LoadObjectsInRelationship(CSVImportSchema obj)
        {
            if (obj.CSVImportColumns != null)
            {
                IList<CSVImportColumn> list = new List<CSVImportColumn>();

                foreach (CSVImportColumn csvImportColumn in obj.CSVImportColumns)
                {
                    CSVImportColumn actCSVImportColumn = CSVImportColumns.Singleton.GetById(csvImportColumn.IdCSVImportColumn);
                    list.Add(actCSVImportColumn);
                }

                obj.CSVImportColumns.Clear();
                foreach (CSVImportColumn csvImportColumn in list)
                    obj.CSVImportColumns.Add(csvImportColumn);
            }
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CSVImportSchema; }
        }
    }
}
