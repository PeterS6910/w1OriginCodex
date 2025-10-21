namespace Contal.Cgp.NCAS.CCU
{
    public interface IEventDispatcher
    {
        void ProcessEvent(EventParameters.EventParameters eventParameters);
    }
}
