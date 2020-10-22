using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace BssomSerializers.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BssomFunction();
            //ComplexClass();
        }

        static void BssomFunction()
        {
            BenchmarkRunner.Run<BssomSerialize_MyImage>();
        }

        static void SingleType()
        {
            BenchmarkRunner.Run<GeneralSerialize<byte>>();
            BenchmarkRunner.Run<GeneralSerialize<int>>();
            BenchmarkRunner.Run<GeneralSerialize<long>>();
            BenchmarkRunner.Run<GeneralSerialize<float>>();
            BenchmarkRunner.Run<GeneralSerialize<double>>();
            BenchmarkRunner.Run<GeneralSerialize<string>>();
            BenchmarkRunner.Run<GeneralSerialize<DateTime>>();
            BenchmarkRunner.Run<GeneralSerialize<Guid>>();
            BenchmarkRunner.Run<GeneralSerialize<decimal>>();
            BenchmarkRunner.Run<GeneralSerialize<byte[]>>();
        }

        static void ComplexClass()
        {
            BenchmarkRunner.Run<GeneralSerialize<BenchmarkComplexClass>>();
        }

        static void EmptyClass()
        {
            BenchmarkRunner.Run<GeneralSerialize<EmptyClass>>();
        }
    }
}
