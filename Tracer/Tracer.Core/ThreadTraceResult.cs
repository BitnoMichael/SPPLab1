using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Core
{
    public class ThreadTraceResult
    {
        public int ThreadId { get; }
        public long Time { get; }
        public IReadOnlyList<MethodTraceResult> Methods { get; }

        public ThreadTraceResult(int threadId, long time, IReadOnlyList<MethodTraceResult> methods)
        {
            ThreadId = threadId;
            Time = time;
            Methods = methods;
        }
    }
}