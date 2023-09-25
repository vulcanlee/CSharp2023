namespace csWithDegreeOfParallelism;

internal class Program
{
    static void Main(string[] args)
    {
        ThreadPool.SetMinThreads(20, 20);
        var source = Enumerable.Range(1, 20);

        var evenNums = source.AsParallel()
            .WithDegreeOfParallelism(20)
            .Select(ShowInfo);

        Console.WriteLine($"Total {evenNums.Count()}");
    }

    static int ShowInfo(int n)
    {
        Console.WriteLine($"N={n:d6} {DateTime.Now:mm:ss} / Thread ID {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(5000);
        return n;
    }
}