namespace csAsParallel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var source = Enumerable.Range(1, 10000);

            var evenNums = source.AsParallel()
                .Select(ShowInfo);

            Console.WriteLine($"Total {evenNums.Count()}");
        }

        static int ShowInfo(int n)
        {
            Console.WriteLine($"N={n:d6} {DateTime.Now:mm:ss} - {n} / Thread ID {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(5000);
            return n;
        }
    }  
}