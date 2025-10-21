using System.Data.SqlServerCe;
using System.IO;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    class SqlCeDbAutonomousEventsEngine
        : ASqlCeDbEngine
        , IAutonomousEventsEngine
    {
        private readonly SqlCeDbAutonomousEventsAcessor _sqlCeDbAutonomousEventsAcessor;
        private readonly string _connectionString;

        public SqlCeDbAutonomousEventsAcessor SqlCeDbAutonomousEventsAcessor
        {
            get { return _sqlCeDbAutonomousEventsAcessor; }
        }

        public SqlCeDbAutonomousEventsEngine()
        {
            var databaseDirectoryPath = Path.Combine(
                CcuCore.Singleton.RootPath,
                Database.DATABASE_DIRECTORY_NAME);

            if (!Directory.Exists(databaseDirectoryPath))
                Directory.CreateDirectory(databaseDirectoryPath);

            var databaseFilePath = Path.Combine(
                databaseDirectoryPath,
                "SqlCeDbAutonomousEvents.sdf");

            _connectionString = 
                string.Format(
                    "Data source = {0}; Max Database Size = 2048;",
                    databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                var sqlCeEnginge = new SqlCeEngine(_connectionString);

                sqlCeEnginge.CreateDatabase();
                sqlCeEnginge.Dispose();
            }

            _sqlCeDbAutonomousEventsAcessor = new SqlCeDbAutonomousEventsAcessor(this);
        }

        protected override SqlCeConnection CreateConnection()
        {
            return new SqlCeConnection(_connectionString);
        }

        public void ShrinkDatabase()
        {
            var engine = new SqlCeEngine(_connectionString);

            engine.Shrink();
            engine.Dispose();
        }
    }
}
