namespace Contal.Cgp.NCAS.Server
{
    public class EventOptions
    {
        public bool Active { get; private set; }

        public EventParameters.EventParameters EventParameters { get; private set; }

        public EventOptions(
            EventParameters.EventParameters eventParameters)
        {
            EventParameters = eventParameters;
            Active = true;
        }

        public void MarkNotActive()
        {
            Active = false;
        }
    }
}