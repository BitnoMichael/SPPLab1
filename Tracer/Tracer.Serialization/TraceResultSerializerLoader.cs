using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization
{
    public class TraceResultSerializerLoader
    {
        public IEnumerable<ITraceResultSerializer> LoadSerializers(string pluginsPath)
        {
            var serializers = new List<ITraceResultSerializer>();

            if (!Directory.Exists(pluginsPath))
                return serializers;

            var dllFiles = Directory.GetFiles(pluginsPath, "*.dll");

            foreach (var dllFile in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);
                    var serializerTypes = assembly.GetTypes()
                        .Where(t => typeof(ITraceResultSerializer).IsAssignableFrom(t)
                                 && !t.IsInterface
                                 && !t.IsAbstract);

                    foreach (var type in serializerTypes)
                    {
                        if (Activator.CreateInstance(type) is ITraceResultSerializer serializer)
                        {
                            serializers.Add(serializer);
                        }
                    }
                }
                catch
                {
                    // Игнорируем файлы, которые не могут быть загружены
                    continue;
                }
            }

            return serializers;
        }
    }
}