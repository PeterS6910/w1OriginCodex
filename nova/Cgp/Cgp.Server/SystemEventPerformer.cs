using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.Server
{
    public class SystemEventPerformer : Contal.Cgp.RemotingCommon.ISystemEventPerformers
    {
        public bool ReportSystemEvent(string eventName, string message)
        {
            return ReportSystemEventStatic(eventName, message);
        }

        public static bool ReportSystemEventStatic(string eventName,string message)
        {
            try
            {
                if (eventName == SystemEvent.DATABASE_DISCONNECTED) //database disconnected
                {
                    if (PerformPresentationProcessor.Singleton.DatabaseDisconnected != null)
                    {
                        PerformPresentationProcessor msgProcessor = PerformPresentationProcessor.Singleton;
                        if (msgProcessor.DatabaseDisconnected.StoredPG != null)
                        {
                            foreach (NoDbsPG pGroup in msgProcessor.DatabaseDisconnected.StoredPG)
                            {
                                msgProcessor.ProcessSendMessageNoDbs(pGroup, eventName + message);
                            }
                        }
                    }
                }
                else
                {
                    var sysEvent = SystemEvents.Singleton.GetByName(eventName);
                    
                    if (sysEvent == null) 
                        return false;

                    PerformPresentationProcessor msgProcessor = PerformPresentationProcessor.Singleton;
                    foreach (PresentationGroup pGroup in sysEvent.PresentationGroups)
                    {
                        var performGroup = new PerformPresentationGroup(pGroup.IdGroup);
                        msgProcessor.ProcessSendMessage(performGroup, eventName + message);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
