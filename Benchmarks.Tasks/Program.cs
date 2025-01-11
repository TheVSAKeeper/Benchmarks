using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarks.Tasks;

internal class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<TaskVsValueTaskBenchmark>();
    }
}

[MemoryDiagnoser]
public class TaskVsValueTaskBenchmark
{
    private const int Iterations = 10_000_000;

    [Benchmark]
    public async Task MeasureTaskPerformance()
    {
        List<Task> tasks = new(Iterations);

        for (int i = 0; i < Iterations; i++)
        {
            tasks.Add(Task.Run(TaskMethod));
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task MeasureValueTaskPerformance()
    {
        List<Task> tasks = new(Iterations);

        for (int i = 0; i < Iterations; i++)
        {
            tasks.Add(Task.Run(ValueTaskMethod));
        }

        await Task.WhenAll(tasks);
    }

    private async Task TaskMethod()
    {
        int result = Math.Abs(Random.Shared.Next(0, 100000));
        await Task.CompletedTask;
    }

    private async ValueTask ValueTaskMethod()
    {
        int result = Math.Abs(Random.Shared.Next(0, 100000));
        await ValueTask.CompletedTask;
    }
}
