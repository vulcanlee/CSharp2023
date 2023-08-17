using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace csLog03
{
    // 在 .NET Core 專案下，使用 與 NLog
    internal class Program
    {
        static void Main(string[] args)
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
                runner.MyAction("MyAction引數");

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
        private readonly ILogger<MyService> _logger;

        public MyService(ILogger<MyService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 該服務所提供的功能
        /// </summary>
        /// <param name="name"></param>
        public void MyAction(string name)
        {
            // 將指定的內容寫入到日誌系統內
            _logger.LogDebug(20, "正在進行指派工作處理! {Action}", name);
        }
    }
}