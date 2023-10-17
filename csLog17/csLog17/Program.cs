using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace csLog17;

// 當應用程式拋出例外異常，如何進行日誌紀錄寫入
internal class Program
{
    static async Task Main(string[] args)
    {
        // 取得 NLog 的日誌物件
        var logger = LogManager.GetCurrentClassLogger();
        // 建議接下來的程式碼，要捕捉起來，一旦發生例外，就可以寫入到日誌系統內
        try
        {
            // 建立一個設定檔案的建構式
            var config = new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();

            // 建立一個服務容器
            using var servicesProvider = new ServiceCollection()
                .AddTransient<MyService>() // 註冊一個具有短暫生命週期的服務
                .AddLogging(loggingBuilder => // 註冊日誌服務
                {
                    // 清除所有的日誌服務提供者
                    loggingBuilder.ClearProviders();
                    // 設定最低的日誌等級
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    // 設定日誌服務提供者為 NLog
                    loggingBuilder.AddNLog(config);
                }).BuildServiceProvider();

            // 取得服務容器內的 MyService 服務物件
            var runner = servicesProvider.GetRequiredService<MyService>();
            // 執行該服務物件的功能，該方法內會寫入一個 Debug 層級的日誌訊息
           await runner.MyAction("MyAction引數");

            Console.WriteLine("Press ANY key to exit");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            // 發生例外時，將例外訊息寫入到日誌系統內
            logger.Error(ex, "因為系統啟動時，發生不明例外異常，系統即將停止運行");
            // 重新拋出例外
            throw;
        }
        finally
        {
            // 將日誌系統內的資料寫入到目的地
            LogManager.Shutdown();
        }
    }
}

/// <summary>
/// 客製服務類別
/// </summary>
public class MyService
{
    /// <summary>
    /// 注入的日誌服務物件
    /// </summary>
    private readonly ILogger<MyService> logger;

    public MyService(ILogger<MyService> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// 該服務所提供的功能
    /// </summary>
    /// <param name="name"></param>
    public async Task MyAction(string name)
    {
        // 將指定的內容寫入到日誌系統內
        logger.LogDebug(20, "正在進行指派工作處理! {Action}", name);
        logger.LogTrace("-------------------------\n\n\n");
        try
        {
            // 這裡模擬在一個方法內，故意拋出一個例外異常
            throw new NullReferenceException("測試用，強制拋出例外異常");
        }
        catch (Exception ex)
        {
            // 將例外異常寫入到日誌系統內
            logger.LogError(ex, "已經捕捉到例外異常(Error)");
            logger.LogTrace("-------------------------\n\n\n");

            logger.LogCritical(ex, "已經捕捉到例外異常(Critical)");
            logger.LogTrace("-------------------------\n\n\n");
        }

        // 模擬聚合例外異常
        AggregationException();

        // 模擬內部例外異常
        InnerException();

        // 模擬非同步方法
        await MyAwaitAsync();

    }

    /// <summary>
    /// 模擬非同步方法
    /// </summary>
    public async Task MyAwaitAsync()
    {
        await Task.Run(() =>
        {
            logger.LogDebug(20, "正在進行指派工作處理! {Action}", "MyAwaitAsync");
            // 這裡模擬在一個非同步方法內，故意拋出一個例外異常
            throw new ArgumentException("測試用，強制拋出例外異常");
        });
    }

    /// <summary>
    /// 模擬聚合例外異常
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void AggregationException()
    {
        // 建立兩個會擲回例外狀況的工作
        Task task1 = Task.Run(() =>
        {
            throw new Exception("This is task 1 exception");
        });
        Task task2 = Task.Run(() =>
        {
            throw new Exception("This is task 2 exception");
        });
        Task task3 = Task.Run(() =>
        {
            Thread.Sleep(1000);
        });

        // 使用 Task.WaitAll 方法等待兩個工作完成
        try
        {
            Task.WaitAll(task1, task2, task3);
        }
        catch (AggregateException ex)
        {
            logger.LogError(ex, "已經捕捉到 AggregateException 例外異常(Error)");
            // 處理 Aggregation Exception 例外狀況
            //foreach (Exception innerException in ex.InnerExceptions)
            //{
            //    Console.WriteLine(innerException.Message);
            //}
            logger.LogTrace("-------------------------\n\n\n");
        }
    }

    /// <summary>
    /// 模擬內部例外異常
    /// </summary>
    public void InnerException()
    {
        try
        {
            try
            {
                // 產生一個外層異常
                throw new Exception("外層異常");
            }
            catch (Exception ex)
            {
                // 產生一個內層異常
                throw new Exception("內層異常", ex);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "已經捕捉到 InnerException 例外異常(Error)");
            logger.LogTrace("-------------------------\n\n\n");
        }
    }
}
