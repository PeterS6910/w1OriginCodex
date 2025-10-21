using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class CSVImportColumns : ABaseOrmTable<CSVImportColumns, CSVImportColumn>
    {
        private CSVImportColumns() : base(null)
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

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CSVImportColumn; }
        }
    }
}

