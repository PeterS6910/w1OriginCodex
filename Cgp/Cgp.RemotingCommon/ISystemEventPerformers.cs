using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.RemotingCommon
{
    public interface ISystemEventPerformers

    {
        bool ReportSystemEvent(string eventName, string message);
    }
}
