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
[RankColumn]
[SkewnessColumn]
[KurtosisColumn]
public class TaskVsValueTaskBenchmark
{
    private const int Iterations = 100_000;

    [Benchmark]
    public async Task MeasureTask()
    {
        List<Task> tasks = new(Iterations);

        for (int i = 0; i < Iterations; i++)
        {
            tasks.Add(Task.Run(TaskMethod));
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task MeasureValueTask()
    {
        List<Task> tasks = new(Iterations);

        for (int i = 0; i < Iterations; i++)
        {
            tasks.Add(Task.Run(ValueTaskMethod));
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureTask_WithoutAwait()
    {
        List<Task> tasks = new(Iterations);

        for (int i = 0; i < Iterations; i++)
        {
            tasks.Add(Task.Run(TaskMethodWithoutAwait));
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureValueTask_WithoutAwait()
    {
        List<Task> tasks = new(Iterations);

        for (int i = 0; i < Iterations; i++)
        {
            tasks.Add(Task.Run(ValueTaskMethodWithoutAwait));
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureTask_ArrayWithoutAwait()
    {
        Task[] tasks = new Task[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            tasks[i] = Task.Run(TaskMethodWithoutAwait);
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureValueTask_ArrayWithoutAwait()
    {
        Task[] tasks = new Task[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            tasks[i] = Task.Run(ValueTaskMethodWithoutAwait);
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureTask_ArrayFactoryWithoutAwait()
    {
        Task[] tasks = new Task[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            tasks[i] = Task.Factory.StartNew(TaskMethodWithoutAwait);
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureValueTask_ArrayFactoryWithoutAwait()
    {
        Task[] tasks = new Task[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            tasks[i] = Task.Factory.StartNew(ValueTaskMethodWithoutAwait);
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureTask_ArrayWithoutAwaitAndTaskRun()
    {
        Task[] tasks = new Task[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            tasks[i] = TaskMethodWithoutAwait();
        }

        return Task.WhenAll(tasks);
    }

    [Benchmark]
    public Task MeasureValueTask_ArrayWithoutAwaitAndTaskRun()
    {
        Task[] tasks = new Task[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            tasks[i] = ValueTaskMethodWithoutAwait().AsTask();
        }

        return Task.WhenAll(tasks);
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

    private Task TaskMethodWithoutAwait()
    {
        int result = Math.Abs(Random.Shared.Next(0, 100000));
        return Task.CompletedTask;
    }

    private ValueTask ValueTaskMethodWithoutAwait()
    {
        int result = Math.Abs(Random.Shared.Next(0, 100000));
        return ValueTask.CompletedTask;
    }
}
