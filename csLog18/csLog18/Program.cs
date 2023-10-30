using NLog;

namespace csLog18;

internal class Program
{
    // 取得當前執行這個方法的類別對應的 Logger 物件
    public static Logger logger =
        LogManager.GetCurrentClassLogger();
    static void Main(string[] args)
    {
        // 觀察 Log File 封存的做法?
        Console.WriteLine($"每分鐘進行封存一次");

        while (true)
        {
            Console.WriteLine($"現在時間 {DateTime.Now:hh:mm:ss}");
            var read = Console.ReadKey();
            if (read.Key == ConsoleKey.Escape)
            {
                break;
            }
            for (int i = 0; i < 5; i++)
            {
                logger.Trace("我是追蹤:Trace");
                logger.Debug("我是偵錯:Debug");
                logger.Info("我是資訊:Info");
                logger.Warn("我是警告:Warn");
                logger.Error("我是錯誤:error");
                logger.Fatal("我是致命錯誤:Fatal");
            }
        }
    }
}
