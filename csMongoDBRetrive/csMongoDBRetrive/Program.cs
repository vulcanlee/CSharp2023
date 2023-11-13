using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Diagnostics;

namespace csMongoDBRetrive;


// MongoDB 的 Blog 文件資料結構
public class Blog
{
    public ObjectId Id { get; set; }
    public int BlogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
}

internal class Program
{
    public static async Task Main(string[] args)
    {
        #region 準備相關設定要進行與雲端 MongoDB 連線用的參數與物件
        // 使用 Environment 來抓取環境變數設定的 帳號與密碼
        string MongoDBAccount = Environment.GetEnvironmentVariable("MongoDBAccount");
        string MongoDBPassword = Environment.GetEnvironmentVariable("MongoDBPassword");

        // 使用 MongoDB Atlas 來連線
        //var mongoUri = $"mongodb+srv://{MongoDBAccount}:{MongoDBPassword}@vulcanmongo.hptf95d.mongodb.net/?retryWrites=true&w=majority";
        var mongoUri = $"mongodb://localhost:27017/?retryWrites=true&w=majority";

        // 宣告一個 MongoDB Client 變數
        IMongoClient client;

        // 宣告一個 MongoDB Database 變數
        IMongoDatabase database;

        // 宣告一個 MongoDB Collection 變數
        IMongoCollection<Blog> collection;

        // 連線到 MongoDB Atlas
        client = new MongoClient(mongoUri);
        #endregion

        #region 進行各種不同 MongoDB 資料庫的 Collection 查詢作法
        #region 建立操作 MogoDB 資料庫與Collection 物件
        // 宣告一個 Database Name 與 Collection Name
        var dbName = "MyCrud";
        var collectionName = "Blog";

        // 取得 MongoDB Collection
        collection = client.GetDatabase(dbName)
           .GetCollection<Blog>(collectionName);

        Stopwatch stopwatch = new Stopwatch();
        #endregion

        #region 使用 C# 類別最強型別 與 Builders 來進行所有文件查詢
        await Console.Out.WriteLineAsync($"用 C# 類別最強型別的查詢 與 Builders 來進行所有文件查詢");
        stopwatch.Restart();
        var filter = Builders<Blog>.Filter.Empty;
        var byFilterAllResultWithClass = await collection.Find(filter).ToListAsync();

        stopwatch.Stop();
        if (byFilterAllResultWithClass.Count > 0)
        {
            Console.WriteLine($"查詢到的文件數量為：{byFilterAllResultWithClass.Count}");
            foreach (var item in byFilterAllResultWithClass)
            {
                Console.WriteLine($"查詢到的文件內容為：{item.Id} / {item.Title}");
            }
        }
        else
        {
            Console.WriteLine($"查詢不到任何文件");
        }
        // 顯示需要耗費時間
        Console.WriteLine($"查詢花費 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

        #region 使用 C# 類別最強型別 與 LINQ 來進行所有文件查詢
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"用 C# 類別最強型別的查詢 與 LINQ 來進行所有文件查詢");
        stopwatch.Restart();
        var byLinqAllResultWithClass = await collection.AsQueryable()
            .ToListAsync();

        stopwatch.Stop();
        if (byLinqAllResultWithClass.Count > 0)
        {
            Console.WriteLine($"查詢到的文件數量為：{byLinqAllResultWithClass.Count}");
            foreach (var item in byLinqAllResultWithClass)
            {
                Console.WriteLine($"查詢到的文件內容為：{item.Id} / {item.Title}");
            }
        }
        else
        {
            Console.WriteLine($"查詢不到任何文件");
        }
        // 顯示需要耗費時間
        Console.WriteLine($"查詢花費 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

        #region 使用 C# 類別最強型別 與 Builders 來進行條件查詢
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"用 C# 類別最強型別的查詢 與 Builders 來進行條件查詢");
        stopwatch.Restart();
        var filter5 = Builders<Blog>.Filter
            .Eq(r => r.Title, "Hello MongoDB3");
        var byFilterResultWithClass = await collection.Find(filter5).FirstOrDefaultAsync();

        stopwatch.Stop();
        if (byFilterResultWithClass != null)
        {
            Console.WriteLine($"查詢到的文件內容為：{byFilterResultWithClass.Id} / {byFilterResultWithClass.Title}");
        }
        else
        {
            Console.WriteLine($"查詢不到任何文件");
        }
        // 顯示需要耗費時間
        Console.WriteLine($"查詢花費 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

        #region 使用 C# 類別最強型別 與 LINQ 來進行條件查詢
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"用 C# 類別最強型別的查詢 與 LINQ 來進行條件查詢");
        stopwatch.Restart();
        var byLinqResultWithClass = await collection.AsQueryable()
            .Where(r => r.Title == "Hello MongoDB3").FirstOrDefaultAsync();

        stopwatch.Stop();
        if (byLinqResultWithClass != null)
        {
            Console.WriteLine($"查詢到的文件數量為：{byLinqResultWithClass.Id} / {byLinqResultWithClass.Title}");
        }
        else
        {
            Console.WriteLine($"查詢不到任何文件");
        }
        // 顯示需要耗費時間
        Console.WriteLine($"查詢花費 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

        #region 使用 C# 類別最強型別 與 Builders 來進行多筆文件查詢
        await Console.Out.WriteLineAsync($"用 C# 類別最強型別 與 Builders 來進行多筆文件查詢");
        stopwatch.Restart();
        var filter3 = Builders<Blog>.Filter
            .Regex(x => x.Title, new BsonRegularExpression("MongoDB1"));
        var byFilterRegexResultWithClass = await collection.Find(filter3).ToListAsync();

        stopwatch.Stop();
        if (byFilterRegexResultWithClass.Count > 0)
        {
            Console.WriteLine($"查詢到的文件內容數量為：{byFilterRegexResultWithClass.Count}");
            foreach (var item in byFilterRegexResultWithClass)
            {
                Console.WriteLine($"查詢到的文件內容為：{item.Id} / {item.Title}");
            }
        }
        else
        {
            Console.WriteLine($"查詢不到任何文件");
        }
        // 顯示需要耗費時間
        Console.WriteLine($"查詢花費 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

        #region 使用 C# 類別最強型別 與 LINQ 來進行多筆文件查詢
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"用 C# 類別最強型別的查詢 與 LINQ 來進行多筆文件查詢");
        stopwatch.Restart();
        var byLinqMultipleResultWithClass = await collection.AsQueryable()
            .Where(r => r.Title.Contains("MongoDB1")).ToListAsync();

        stopwatch.Stop();
        if (byLinqMultipleResultWithClass.Count > 0)
        {
            Console.WriteLine($"查詢到的文件數量為：{byLinqMultipleResultWithClass.Count}");
            foreach (var item in byLinqMultipleResultWithClass)
            {
                Console.WriteLine($"查詢到的文件內容為：{item.Id} / {item.Title}");
            }
        }
        else
        {
            Console.WriteLine($"查詢不到任何文件");
        }
        // 顯示需要耗費時間
        Console.WriteLine($"查詢花費 {stopwatch.ElapsedMilliseconds} ms");
        #endregion
        #endregion

    }
}