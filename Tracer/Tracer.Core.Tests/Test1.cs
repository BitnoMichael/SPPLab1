using Tracer.Core;
using Xunit;

namespace Tracer.Tests
{
    /// <summary>
    /// Набор тестов для проверки корректности работы трассировщика.
    /// </summary>
    public class TracingFunctionalityTests
    {
        private readonly ITracer _tracer;
        private readonly ScenarioRunner _runner;

        public TracingFunctionalityTests()
        {
            _tracer = new MyTracer();
            _runner = new ScenarioRunner(_tracer);
        }

        [Fact]
        public void VerifySingleThreadTracing()
        {
            // Arrange & Act
            _runner.RunSimpleOperation();

            var traceData = _tracer.GetTraceResult();

            // Assert
            Assert.NotNull(traceData);
            Assert.Single(traceData.Threads);

            var threadInfo = traceData.Threads[0];
            Assert.Single(threadInfo.Methods);

            var methodInfo = threadInfo.Methods[0];
            Assert.Equal(nameof(ScenarioRunner.RunSimpleOperation), methodInfo.MethodName);
            Assert.Equal(nameof(ScenarioRunner), methodInfo.ClassName);
            Assert.InRange(methodInfo.Time, 0, int.MaxValue); // время неотрицательное
        }

        [Fact]
        public void CheckNestedMethodHierarchy()
        {
            // Arrange & Act
            _runner.ExecuteOuterMethod();

            var traceData = _tracer.GetTraceResult();

            // Assert
            var root = traceData.Threads[0].Methods[0];
            Assert.Equal(nameof(ScenarioRunner.ExecuteOuterMethod), root.MethodName);
            Assert.Single(root.Methods); // должен быть ровно один вложенный метод

            var nested = root.Methods[0];
            Assert.Equal(nameof(ScenarioRunner.ExecuteInnerMethod), nested.MethodName);
        }

        [Fact]
        public void EnsureThreadSeparation()
        {
            // Arrange
            var tracer1 = new MyTracer();
            var tracer2 = new MyTracer();

            var runner1 = new ScenarioRunner(tracer1);
            var runner2 = new ScenarioRunner(tracer2);

            // Act
            var thread1 = new Thread(() => runner1.RunSimpleOperation());
            var thread2 = new Thread(() => runner2.ExecuteOuterMethod());

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            var result1 = tracer1.GetTraceResult();
            var result2 = tracer2.GetTraceResult();

            // Assert
            Assert.Single(result1.Threads);
            Assert.Single(result2.Threads);

            var id1 = result1.Threads[0].ThreadId;
            var id2 = result2.Threads[0].ThreadId;
            Assert.NotEqual(id1, id2);
        }
    }

    /// <summary>
    /// Вспомогательный класс, имитирующий выполнение различных сценариев с замером времени.
    /// </summary>
    public class ScenarioRunner
    {
        private readonly ITracer _tracer;

        public ScenarioRunner(ITracer tracer)
        {
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public void RunSimpleOperation()
        {
            _tracer.StartTrace();
            SimulateWork();
            _tracer.StopTrace();
        }

        public void ExecuteOuterMethod()
        {
            _tracer.StartTrace();
            ExecuteInnerMethod();
            _tracer.StopTrace();
        }

        public void ExecuteInnerMethod()
        {
            _tracer.StartTrace();
            SimulateWork();
            _tracer.StopTrace();
        }

        private void SimulateWork()
        {
            // Имитация полезной нагрузки
            Thread.Sleep(10);
        }
    }
}