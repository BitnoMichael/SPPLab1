using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Core
{
    public class MethodTraceResult
    {
        public string MethodName { get; }
        public string ClassName { get; }
        public long Time { get; }
        public IReadOnlyList<MethodTraceResult> Methods { get; }

        public MethodTraceResult(string methodName, string className, long time,
            IReadOnlyList<MethodTraceResult> methods)
        {
            MethodName = methodName;
            ClassName = className;
            Time = time;
            Methods = methods;
        }
    }
}