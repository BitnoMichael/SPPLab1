using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tracer.Core
{
    internal class ThreadTraceData
    {
        private readonly Stack<InternalMethodTrace> _methodStack = new();
        private readonly List<InternalMethodTrace> _rootMethods = new();

        public void StartMethod(InternalMethodTrace method)
        {
            if (_methodStack.Count > 0)
            {
                var parent = _methodStack.Peek();
                parent.InnerMethods.Add(method);
            }
            else
            {
                _rootMethods.Add(method);
            }

            _methodStack.Push(method);
            method.Stopwatch.Start();
        }

        public void StopMethod()
        {
            if (_methodStack.Count == 0)
                throw new InvalidOperationException("No method to stop");

            var method = _methodStack.Pop();
            method.Stopwatch.Stop();
        }

        public long GetTotalTime()
        {
            return _rootMethods.Sum(m => m.Stopwatch.ElapsedMilliseconds);
        }

        public List<MethodTraceResult> GetMethodTraceResults()
        {
            return _rootMethods.Select(ConvertToMethodTraceResult).ToList();
        }

        private MethodTraceResult ConvertToMethodTraceResult(InternalMethodTrace internalMethod)
        {
            var innerResults = internalMethod.InnerMethods
                .Select(ConvertToMethodTraceResult)
                .ToList();

            return new MethodTraceResult(
                internalMethod.MethodName,
                internalMethod.ClassName,
                internalMethod.Stopwatch.ElapsedMilliseconds,
                innerResults.AsReadOnly()
            );
        }
    }
}