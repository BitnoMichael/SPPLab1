using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Tracer.Core
{
    public class MyTracer : ITracer
    {
        private readonly ConcurrentDictionary<int, ThreadTraceData> _threadData = new();
        private readonly object _lock = new();

        public void StartTrace()
        {
            var threadId = Environment.CurrentManagedThreadId;
            var threadData = _threadData.GetOrAdd(threadId, _ => new ThreadTraceData());

            // Получаем информацию о вызывающем методе
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1); // Пропускаем текущий метод
            var method = frame?.GetMethod();

            var methodName = method?.Name;
            var className = method?.DeclaringType?.Name ?? "Unknown";
            if (methodName == null)
                return;
            var methodTrace = new InternalMethodTrace(methodName, className);
            threadData.StartMethod(methodTrace);
        }

        public void StopTrace()
        {
            var threadId = Environment.CurrentManagedThreadId;

            if (!_threadData.TryGetValue(threadId, out var threadData))
            {
                throw new InvalidOperationException("StopTrace called without StartTrace");
            }

            threadData.StopMethod();
        }

        public TraceResult GetTraceResult()
        {
            var threadResults = new List<ThreadTraceResult>();

            foreach (var kvp in _threadData)
            {
                var threadId = kvp.Key;
                var threadData = kvp.Value;

                var totalTime = threadData.GetTotalTime();
                var methods = threadData.GetMethodTraceResults();

                var threadResult = new ThreadTraceResult(
                    threadId,
                    totalTime,
                    methods.AsReadOnly()
                );

                threadResults.Add(threadResult);
            }

            return new TraceResult(threadResults.AsReadOnly());
        }
    }
}