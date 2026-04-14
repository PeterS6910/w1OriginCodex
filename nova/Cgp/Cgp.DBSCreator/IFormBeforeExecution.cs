namespace Contal.Cgp.DBSCreator
{
    public interface IFormBeforeExecution
    {
        void ShowCreateDatabaseSuccess();
        void ShowCreateTablesFailure();
        void ShowBeforeExecutionSuccess();
        void ShowCreateDatabaseFailure();
    }
}