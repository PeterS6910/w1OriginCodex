using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SmartLprConsole.Models
{
    [DataContract]
    internal sealed class AlarmCollection
    {
        [DataMember(Name = "alarms")]
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();
    }

    [DataContract]
    internal sealed class Alarm
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "triggeredTimestamp")]
        public string TriggeredTimestamp { get; set; }

        [DataMember(Name = "lastCheckedTimestamp")]
        public string LastCheckedTimestamp { get; set; }
    }
}
