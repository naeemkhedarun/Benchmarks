using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
//            var competitions = new BenchmarkCompetitionSwitch(new[]
//            {
//                typeof (PropertyGetterReflectionBenchmark),
//                typeof (PropertySetterReflectionBenchmark)
//            });
//
//            competitions.Run(args);

//            new BenchmarkRunner().RunCompetition(new PropertyGetterReflectionBenchmark());

            new PropertySetterReflectionBenchmark().CachedExpression();

            Console.ReadKey();
        }
    }
}
