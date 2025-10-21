namespace Contal.IwQuick.Threads
{
    public interface ISafeThreadContext
    {
        bool AimedToBeStopped { get; set; }
    }

    public class SafeThreadContext : ISafeThreadContext
    {
        private volatile bool _aimedToBeStopped = false;
        public bool AimedToBeStopped { get { return _aimedToBeStopped; } set { _aimedToBeStopped = value; } }
    }
}