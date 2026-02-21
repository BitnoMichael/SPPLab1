using System.Xml.Serialization;

namespace Tracer.Serialization.Xml.Models
{
    [XmlRoot("root")]
    public class TraceResultXml
    {
        [XmlArray("threads")]
        [XmlArrayItem("thread")]
        public List<ThreadTraceXml> Threads { get; set; } = new();
    }

    public class ThreadTraceXml
    {
        [XmlElement("id")]
        public string Id { get; set; } = "";

        [XmlElement("time")]
        public string Time { get; set; } = "";

        [XmlArray("methods")]
        [XmlArrayItem("method")]
        public List<MethodTraceXml> Methods { get; set; } = new();
    }

    public class MethodTraceXml
    {
        [XmlElement("name")]
        public string Name { get; set; } = "";

        [XmlElement("class")]
        public string Class { get; set; } = "";

        [XmlElement("time")]
        public string Time { get; set; } = "";

        [XmlArray("methods")]
        [XmlArrayItem("method")]
        public List<MethodTraceXml> Methods { get; set; } = new();
    }
}