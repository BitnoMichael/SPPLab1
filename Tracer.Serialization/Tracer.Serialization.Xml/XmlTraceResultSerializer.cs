using System.IO;
using System.Xml.Serialization;
using Tracer.Core;
using Tracer.Serialization.Abstractions;
using Tracer.Serialization.Xml.Models;

namespace Tracer.Serialization.Xml
{
    public class XmlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "xml";

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var xmlResult = ConvertToXmlModel(traceResult);
            var serializer = new XmlSerializer(typeof(TraceResultXml));

            using var writer = new StreamWriter(to);
            serializer.Serialize(writer, xmlResult);
        }

        private TraceResultXml ConvertToXmlModel(TraceResult traceResult)
        {
            var result = new TraceResultXml();

            foreach (var thread in traceResult.Threads)
            {
                var threadXml = new ThreadTraceXml
                {
                    Id = thread.ThreadId.ToString(),
                    Time = $"{thread.Time}ms"
                };

                threadXml.Methods.AddRange(
                    thread.Methods.Select(ConvertMethod)
                );

                result.Threads.Add(threadXml);
            }

            return result;
        }

        private MethodTraceXml ConvertMethod(MethodTraceResult method)
        {
            var methodXml = new MethodTraceXml
            {
                Name = method.MethodName,
                Class = method.ClassName,
                Time = $"{method.Time}ms"
            };

            foreach (var innerMethod in method.Methods)
            {
                methodXml.Methods.Add(ConvertMethod(innerMethod));
            }

            return methodXml;
        }
    }
}