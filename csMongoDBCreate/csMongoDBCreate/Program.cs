using MongoDB.Driver;

namespace csMongoDBCreate;

// MongoDB 的 Blog 文件資料結構
public class Blog
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
}

internal class Program
{
    public static void Main(string[] args)
    {
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

        // 宣告一個 Database Name 與 Collection Name
        var dbName = "MyCrud";
        var collectionName = "Blog";

        // 取得 MongoDB Collection
        collection = client.GetDatabase(dbName)
           .GetCollection<Blog>(collectionName);

        // 宣告一個 Blog 物件
        Blog blog = new Blog
        {
            Title = "Hello MongoDB",
            Content = "Hello MongoDB",
            CreateAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };

        try
        {
            // 新增一筆 Blog 資料
            collection.InsertOne(blog);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
            Console.WriteLine(e);
            Console.WriteLine();
            return;
        }
    }
}