using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;

namespace Bssom.Serializer.Benchmark
{
    internal class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Job baseConfig = Job.ShortRun.WithIterationCount(1).WithWarmupCount(1);
            AddJob(baseConfig.WithRuntime(CoreRuntime.Core21).WithJit(Jit.RyuJit).WithPlatform(Platform.X64));
            //AddExporter(MarkdownExporter.GitHub);
            //AddExporter(JsonExporter.Full);
            //AddExporter(CsvExporter.Default);
            //AddExporter(CsvMeasurementsExporter.Default);
            //AddExporter(RPlotExporter.Default);
            AddDiagnoser(MemoryDiagnoser.Default);
        }
    }
}
