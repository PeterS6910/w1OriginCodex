namespace Contal.Cgp.DBSCreator
{
    internal interface IFormUpdateDatabase
    {
        void ShowStartDatabaseBackup();
        void ShowStopDatabaseBackup(bool createdDatabaseBackup);
        void StopProgress();
        void CloseSync();
        bool PerformVersionBasedDatabaseConversion(DatabaseCommandExecutor databaseCommandExecutor);
        void OnUpdateFinished();
    }
}