using System;
using System.Threading;
using Tracer.Core;
using Tracer.Serialization;

namespace Tracer.Example
{
    public class Foo
    {
        private readonly ITracer _tracer;

        public Foo(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void M0()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            M1();
            M2();
            _tracer.StopTrace();
        }

        private void M1()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }

        private void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }
    }

    public class Bar
    {
        private readonly ITracer _tracer;

        public Bar(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void M1()
        {
            _tracer.StartTrace();
            Thread.Sleep(300);
            M2();
            _tracer.StopTrace();
        }

        private void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(400);
            _tracer.StopTrace();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tracer = new MyTracer();

            // Пример многопоточной трассировки
            var foo = new Foo(tracer);
            var bar = new Bar(tracer);

            Thread thread1 = new Thread(() =>
            {
                foo.M0();
            });

            Thread thread2 = new Thread(() =>
            {
                bar.M1();
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            // Получение результатов
            var traceResult = tracer.GetTraceResult();

            // Сериализация результатов
            var pluginsPath = AppDomain.CurrentDomain.BaseDirectory;
            var serializerFactory = new TraceResultSerializerFactory(pluginsPath);
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Results");

            serializerFactory.SerializeAll(traceResult, outputPath);

            Console.WriteLine("Tracing completed. Results saved to: " + outputPath);

            // Вывод информации о доступных сериализаторах
            var serializers = serializerFactory.GetAvailableSerializers();
            Console.WriteLine($"Available serializers: {serializers.Count()}");
            foreach (var serializer in serializers)
            {
                Console.WriteLine($"  - {serializer.Format}");
            }
        }
    }
}