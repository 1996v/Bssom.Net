using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace Bssom.Serializer.Benchmark
{
    internal class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Job baseConfig = Job.ShortRun.WithIterationCount(1).WithWarmupCount(1);
            AddJob(baseConfig.WithRuntime(CoreRuntime.Core21).WithJit(Jit.RyuJit).WithPlatform(Platform.X64));
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
        }
    }
}
