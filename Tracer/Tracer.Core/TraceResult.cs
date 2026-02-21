using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Core
{
    public class TraceResult
    {
        public IReadOnlyList<ThreadTraceResult> Threads { get; }

        public TraceResult(IReadOnlyList<ThreadTraceResult> threads)
        {
            Threads = threads;
        }
    }
}