using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace csThreadPoolTesting
{
    public partial class MainWindowViewModel : ObservableObject
    {
        int executionTime = 3000;
        int monitorInterval = 200;
        Stopwatch stopwatch = new Stopwatch();
        object lockObject = new object();

        [ObservableProperty]
        long completedWorkItemCount = 0;
        [ObservableProperty]
        long pendingWorkItemCount = 0;
        [ObservableProperty]
        int threadCount = 0;
        [ObservableProperty]
        string elapseTime = "00:00:00";
        [ObservableProperty]
        int currentRunningThreadCount = 0;
        [ObservableProperty]
        int logicalProcessorCount = 0;
        [ObservableProperty]
        int threadPoolMinWorker = 0;
        [ObservableProperty]
        int waitInQueue = 0;

        public MainWindowViewModel()
        {
            LogicalProcessorCount = Environment.ProcessorCount;
            ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
            ThreadPoolMinWorker = minWorker;
            StartMonitor();
        }

        [RelayCommand]
        public void StartMonitor()
        {
            Thread thread = new Thread(_ =>
            {
                stopwatch.Start();
                while (true)
                {
                    CompletedWorkItemCount = ThreadPool.CompletedWorkItemCount;
                    PendingWorkItemCount = ThreadPool.PendingWorkItemCount;
                    ThreadCount = ThreadPool.ThreadCount;
                    ElapseTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                    Thread.Sleep(monitorInterval);
                }
            });
            thread.Start();
        }

        [RelayCommand]
        void ResetTimer()
        {
            stopwatch.Restart();
        }

        [RelayCommand]
        void Get10Worker()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject) { WaitInQueue += 10; }
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    lock (lockObject) { CurrentRunningThreadCount++; WaitInQueue -= 1; }
                    Thread.Sleep(executionTime);
                    lock (lockObject) { CurrentRunningThreadCount--; }
                });
            }
        }

        [RelayCommand]
        void Get50Worker()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject) { WaitInQueue += 50; }
            for (int i = 0; i < 50; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    lock (lockObject) { CurrentRunningThreadCount++; WaitInQueue -= 1; }
                    Thread.Sleep(executionTime);
                    lock (lockObject) { CurrentRunningThreadCount--; }
                });
            }
        }

        [RelayCommand]
        void Get10AsyncWorker()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject) { WaitInQueue += 10; }
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    lock (lockObject) { CurrentRunningThreadCount++; WaitInQueue -= 1; }
                    await Task.Delay(executionTime);
                    lock (lockObject) { CurrentRunningThreadCount--; }
                });
            }
        }

        [RelayCommand]
        void Get50AsyncWorker()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObject) { WaitInQueue += 50; }
            for (int i = 0; i < 50; i++)
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    lock (lockObject) { CurrentRunningThreadCount++; WaitInQueue -= 1; }
                    await Task.Delay(executionTime);
                    lock (lockObject) { CurrentRunningThreadCount--; }
                });
            }
        }

        [RelayCommand]
        void AddMoreThreadPoolMinWorker()
        {
            ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
            minWorker += 100;
            minIOC += 100;
            ThreadPool.SetMinThreads(minWorker, minIOC);
            ThreadPoolMinWorker = minWorker;
        }

        [RelayCommand]
        void ResetThreadPoolMinWorker()
        {
            ThreadPool.SetMinThreads(LogicalProcessorCount, LogicalProcessorCount);
            ThreadPoolMinWorker = LogicalProcessorCount;
        }
    }
}
