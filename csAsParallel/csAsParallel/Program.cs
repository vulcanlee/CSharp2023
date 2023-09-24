namespace csAsParallel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var source = Enumerable.Range(1, 10000);

            // Opt in to PLINQ with AsParallel.
            var evenNums = from num in source.AsParallel()
                           where num % 2 == 0
                           select ShowInfo(num);

            Console.WriteLine($"Total {evenNums.Count()}");
        }

        static int ShowInfo(int n)
        {
            Console.WriteLine($" {DateTime.Now:mm:ss} - {n} / Thread ID {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(5000);
            return n;
        }
    }  
}