using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Yaml
{
    public class YamlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "yaml";

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var yamlData = ConvertToSerializableFormat(traceResult);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();

            var yaml = serializer.Serialize(yamlData);

            using var writer = new StreamWriter(to);
            writer.Write(yaml);
        }

        private object ConvertToSerializableFormat(TraceResult traceResult)
        {
            return new
            {
                threads = traceResult.Threads.Select(t => new
                {
                    id = t.ThreadId.ToString(),
                    time = $"{t.Time}ms",
                    methods = t.Methods.Select(ConvertMethod)
                })
            };
        }

        private object ConvertMethod(MethodTraceResult method)
        {
            return new
            {
                name = method.MethodName,
                @class = method.ClassName,
                time = $"{method.Time}ms",
                methods = method.Methods.Select(ConvertMethod)
            };
        }
    }
}