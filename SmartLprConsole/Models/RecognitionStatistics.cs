using System.Runtime.Serialization;

namespace SmartLprConsole.Models
{
    [DataContract]
    internal sealed class RecognitionStatistics
    {
        [DataMember(Name = "recognitions")]
        public int Recognitions { get; set; }

        [DataMember(Name = "recognitionsWithLicense")]
        public int RecognitionsWithLicense { get; set; }

        [DataMember(Name = "recognitionsWithGrammarOk")]
        public int RecognitionsWithGrammarOk { get; set; }

        [DataMember(Name = "avgQuality")]
        public int AverageQuality { get; set; }

        [DataMember(Name = "numberOfUnknownChars")]
        public int NumberOfUnknownChars { get; set; }
    }
}
