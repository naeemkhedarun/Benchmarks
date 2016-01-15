using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            var competitions = new BenchmarkCompetitionSwitch(new[]
            {
                typeof (PropertyGetterReflectionBenchmark),
                typeof (PropertySetterReflectionBenchmark),
                typeof(DynamicProxyCallBenchmark),
                typeof(DynamicProxyCreationBenchmark)
            });

            competitions.Run(args);

            //            new BenchmarkRunner().RunCompetition(new PropertyGetterReflectionBenchmark());

//                        new DynamicProxyCreationBenchmark().NProxyCreation();

            Console.ReadKey();
        }
    }
}
