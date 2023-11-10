using MongoDB.Driver;
using System.Diagnostics;

namespace csMongoDBCreate;

// MongoDB 的 Blog 文件資料結構
public class Blog
{
    public int BlogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
}

internal class Program
{
    public static void Main(string[] args)
    {
        #region 準備相關設定要進行與雲端 MongoDB 連線用的參數與物件
        // 使用 Environment 來抓取環境變數設定的 帳號與密碼
        string MongoDBAccount = Environment.GetEnvironmentVariable("MongoDBAccount");
        string MongoDBPassword = Environment.GetEnvironmentVariable("MongoDBPassword");

        // 使用 MongoDB Atlas 來連線
        var mongoUri = $"mongodb+srv://{MongoDBAccount}:{MongoDBPassword}@vulcanmongo.hptf95d.mongodb.net/?retryWrites=true&w=majority";

        // 宣告一個 MongoDB Client 變數
        IMongoClient client;

        // 宣告一個 MongoDB Database 變數
        IMongoDatabase database;

        // 宣告一個 MongoDB Collection 變數
        IMongoCollection<Blog> collection;

        try
        {
            // 連線到 MongoDB Atlas
            client = new MongoClient(mongoUri);
        }
        catch (Exception e)
        {
            Console.WriteLine("{e.Message}");
            Console.WriteLine(e);
            Console.WriteLine();
            return;
        }
        #endregion

        #region 列出所有的資料庫名稱
        Console.WriteLine($"列出所有存在的資料庫");
        var dbs = client.ListDatabases().ToList();
        foreach (var item in dbs)
        {
            Console.WriteLine(item);
        }
        #endregion

        #region 準備新增 Document 到資料庫的 Collection 內 (執行 100 次新增文件)
        // 宣告一個 Database Name 與 Collection Name
        var dbName = "MyCrud";
        var collectionName = "Blog";

        // 取得 MongoDB Collection
        collection = client.GetDatabase(dbName)
           .GetCollection<Blog>(collectionName);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Restart();

        #region 每次新增一筆文件
        for (int i = 0; i < 100; i++)
        {
            // 宣告一個 Blog 物件
            Blog blog = new Blog
            {
                BlogId = i,
                Title = $"Hello MongoDB{i}",
                Content = $"Hello MongoDB{i}",
                CreateAt = DateTime.Now.AddDays(i),
                UpdateAt = DateTime.Now.AddDays(i)
            };

            // 新增一筆 Blog 資料
            collection.InsertOne(blog);
        }
        #endregion

        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"新增 100 次文件需要 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

        #region 一次新增100筆文件
        List<Blog> blogs = new List<Blog>();
        stopwatch.Restart();
        for (int i = 0; i < 100; i++)
        {
            // 宣告一個 Blog 物件
            Blog blog = new Blog
            {
                BlogId = i,
                Title = $"Hello MongoDB{i}",
                Content = $"Hello MongoDB{i}",
                CreateAt = DateTime.Now.AddDays(i),
                UpdateAt = DateTime.Now.AddDays(i)
            };
            blogs.Add(blog);
            // 新增一筆 Blog 資料
        }
        collection.InsertMany(blogs);
        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"一次新增 100 筆文件需要 {stopwatch.ElapsedMilliseconds} ms");
        #endregion
    }
}