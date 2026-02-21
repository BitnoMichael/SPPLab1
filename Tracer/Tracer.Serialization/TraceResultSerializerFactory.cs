using System.Collections.Generic;
using System.IO;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization
{
    public class TraceResultSerializerFactory
    {
        private readonly List<ITraceResultSerializer> _serializers;

        public TraceResultSerializerFactory(string pluginsPath)
        {
            var loader = new TraceResultSerializerLoader();
            _serializers = new List<ITraceResultSerializer>(loader.LoadSerializers(pluginsPath));
        }

        public void SerializeAll(TraceResult traceResult, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            foreach (var serializer in _serializers)
            {
                var fileName = Path.Combine(outputDirectory, $"result.{serializer.Format}");
                using (var fileStream = File.Create(fileName))
                {
                    serializer.Serialize(traceResult, fileStream);
                }
            }
        }

        public IEnumerable<ITraceResultSerializer> GetAvailableSerializers()
        {
            return _serializers.AsReadOnly();
        }
    }
}   