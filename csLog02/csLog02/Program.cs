using NLog;

namespace csLog02
{
    internal class Program
    {
        // 取得當前執行這個方法的類別對應的 Logger 物件
        public static Logger logger =
            LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            // 請觀察 Console & Log File 所寫入的內容為何?
            Console.WriteLine($"寫入各種不同層級的 日誌項目");

            logger.Trace("我是追蹤:Trace");
            logger.Debug("我是偵錯:Debug");
            logger.Info("我是資訊:Info");
            logger.Warn("我是警告:Warn");
            logger.Error("我是錯誤:error");
            logger.Fatal("我是致命錯誤:Fatal");
        }
    }
}