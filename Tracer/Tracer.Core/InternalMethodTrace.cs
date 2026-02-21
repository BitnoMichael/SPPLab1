using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tracer.Core
{
    internal class InternalMethodTrace
    {
        public string MethodName { get; }
        public string ClassName { get; }
        public Stopwatch Stopwatch { get; }
        public List<InternalMethodTrace> InnerMethods { get; }

        public InternalMethodTrace(string methodName, string className)
        {
            MethodName = methodName;
            ClassName = className;
            Stopwatch = new Stopwatch();
            InnerMethods = new List<InternalMethodTrace>();
        }
    }
}