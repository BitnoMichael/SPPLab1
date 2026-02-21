using Newtonsoft.Json;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json
{
    public class JsonTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "json";

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var jsonData = ConvertToSerializableFormat(traceResult);
            var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            using var writer = new StreamWriter(to);
            writer.Write(json);
        }

        private object ConvertToSerializableFormat(TraceResult traceResult)
        {
            var result = new
            {
                threads = traceResult.Threads.Select(ConvertThread)
            };
            return result;
        }

        private object ConvertThread(ThreadTraceResult thread)
        {
            return new
            {
                id = thread.ThreadId.ToString(),
                time = $"{thread.Time}ms",
                methods = thread.Methods.Select(ConvertMethod)
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