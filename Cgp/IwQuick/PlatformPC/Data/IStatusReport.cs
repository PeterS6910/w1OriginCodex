namespace Contal.IwQuick.Data
{
    public interface IStatusReport
    {
        void AddStatusMessage(StatusMessage data);

        void AddStatusMessage(string message);
    }
}