using System.Runtime.Serialization;

namespace SmartLprConsole.Models
{
    [DataContract]
    internal sealed class DeviceStatus
    {
        [DataMember(Name = "global")]
        public bool Global { get; set; }

        [DataMember(Name = "lamp")]
        public bool Lamp { get; set; }

        [DataMember(Name = "temperature")]
        public int Temperature { get; set; }
    }
}
