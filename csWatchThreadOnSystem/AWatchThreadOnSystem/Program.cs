using Microsoft.Diagnostics.Runtime;
using System.Diagnostics;

namespace AWatchThreadOnSystem
{
    internal class Program
    {
        private const int MaxSleepTime = 5000;
        private const int MaxThreads = 8;

        static void Main(string[] args)
        {
            #region 等候使用者指令，決定要執行哪個工作
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.S)
                {
                    ShowThreadInformation();
                }
                else if (key.Key == ConsoleKey.P)
                {
                    UsingThreadPool();
                }
                else if (key.Key == ConsoleKey.E)
                {
                    // 結束程式
                    Environment.Exit(0);
                }
            }
            #endregion
        }

        /// <summary>
        /// 使用執行緒集區來執行工作
        /// </summary>
        private static void UsingThreadPool()
        {
            for (int i = 0; i < MaxThreads; i++)
            {
                // 透過執行緒集區來執行工作，並且模擬會休息一段時間，讓我們可以觀察到執行緒的變化
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    Console.WriteLine($"  Pool Managed Id : {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(MaxSleepTime);
                    Console.WriteLine($"  Pool Managed Id : {Thread.CurrentThread.ManagedThreadId} Exit");
                });
            }
        }

        /// <summary>
        /// 顯示處理程序執行緒與 .NET Managed 執行緒資訊到螢幕上
        /// </summary>
        private static void ShowThreadInformation()
        {
            int threadCount = 1;

            #region Get Process's Threads - 使用 .NET 提供的 API 來抓取相關資訊
            Console.WriteLine($"顯示該處理程序內在 作業系統 上的所有執行緒資訊");
            var osThreads = System.Diagnostics.Process.GetCurrentProcess().Threads;
            threadCount = 1;
            foreach (ProcessThread itemProcessThread in osThreads)
            {
                Console.WriteLine($"OS Thread {threadCount++} : Id {itemProcessThread.Id} " +
                    $"{itemProcessThread.PriorityLevel.ToString()}");
            }
            #endregion

            #region Get CLR Managed Threads - 這裡是透過第三方套件來讀取到這些資訊
            Console.WriteLine($"顯示該處理程序內在 CLR Managed 上的所有執行緒資訊");
            threadCount = 1;
            using (DataTarget target = DataTarget.AttachToProcess(
                Process.GetCurrentProcess().Id, false))
            {
                ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();
                foreach (ClrThread itemClrManagedThread in runtime.Threads)
                {
                    Console.WriteLine($"Managed Thread {threadCount++} : Id" +
                        $"{itemClrManagedThread.ManagedThreadId} (OS Id {itemClrManagedThread.OSThreadId})");
                }
            }
            #endregion
        }
    }
}