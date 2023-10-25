using Newtonsoft.Json;
using NLog;

namespace csLog04;

public class SomeClass
{
    public int Value { get; set; }
    public string Title { get; set; }
}

public class SomeWithToStringClass
{
    public int Value { get; set; }
    public string Title { get; set; }

    public override string ToString()
    {
        return $"Title: {Title}, Value: {Value}";
    }
}

public class SomeWithToJsonClass
{
    public int Value { get; set; }
    public string Title { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class Program
{
    // 取得當前執行這個方法的類別對應的 Logger 物件
    public static Logger logger =
        LogManager.GetCurrentClassLogger();
    static void Main(string[] args)
    {
        // 請觀察 Console & Log File 所寫入的內容為何?
        Console.WriteLine($"寫入各種不同層級的 日誌項目");

        var someObject = new SomeClass() { Title = "外部物件", Value = 168 };
        var someWithToStringObject = new SomeWithToStringClass() { Title = "內部物件", Value = 99 };
        var someWithToJsonClass = new SomeWithToJsonClass() { Title = "所有物件", Value = 87 };
        Thread.Sleep(1000);
        logger.Trace("我是追蹤:Trace {someObject}", JsonConvert.SerializeObject(someObject));
        logger.Debug("我是偵錯:Debug {someObject}", someObject);
        logger.Info("我是資訊:Info {someObject}", someWithToStringObject);
        logger.Warn("我是警告:Warn {someObject}", someWithToJsonClass);
        logger.Error("我是錯誤:error {someObject}", new { OrderId = 2, Status = "Processing" });
        logger.Fatal("我是致命錯誤:Fatal {someObject}", someObject);


        Console.WriteLine("Press any key for continuing...");
        Console.ReadKey();
    }
}