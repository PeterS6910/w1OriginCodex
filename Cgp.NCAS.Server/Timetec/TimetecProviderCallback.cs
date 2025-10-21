using System.Collections.Generic;

using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.Server.Timetec
{
    public class TimetecProviderCallback : IServiceNovaTimetecCallback
    {
        public void ProcessChanges(IEnumerable<ObjectChangeEvent> changeActions)
        {
            TimetecCore.Singleton.InsertObjectChangeEvents(changeActions);
        }
    }
}
