using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Diagnostics;
using static MongoDB.Driver.WriteConcern;

namespace csMongoDBUpdate;

// MongoDB 的 Blog 文件資料結構
public class Blog
{
    public ObjectId Id { get; set; }
    public int BlogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
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
        var collectionName = "BlogForUpdate";

        // 取得 MongoDB Collection
        database = client.GetDatabase(dbName);

        #region 先行刪除這個測試用的 Collection
        await database.DropCollectionAsync(collectionName);
        #endregion

        collection = database.GetCollection<Blog>(collectionName);

        Stopwatch stopwatch = new Stopwatch();
        #endregion

        #region 建立準備要進行更新用的測試文件
        #region 一次新增 5 筆文件
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"建立準備要進行更新用的測試文件");
        stopwatch.Restart();
        List<Blog> blogs = new List<Blog>();
        stopwatch.Restart();
        for (int i = 0; i < 5; i++)
        {
            // 宣告一個 Blog 物件
            Blog blog = new Blog
            {
                BlogId = i,
                Title = $"Hello MongoDB{i}",
                Tag = $"C#",
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
        Console.WriteLine($"一次新增 5 筆文件需要 {stopwatch.ElapsedMilliseconds} ms");
        #endregion
        #endregion

        #region 找出符合更新條件的文件，並進行更新一筆文件
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"找出符合更新條件的文件，並進行更新一筆文件");
        await Console.Out.WriteLineAsync($"Collection 內的所有文件");
        var byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Tag}");
        }

        stopwatch.Restart();

        var filter1 = Builders<Blog>.Filter.Eq(r => r.Tag, "C#");
        var update1 = Builders<Blog>.Update.Set(x => x.Tag, "SQL");
        var updateResult = await collection.UpdateOneAsync(filter1, update1);

        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"更新花費 {stopwatch.ElapsedMilliseconds} ms");
        await Console.Out.WriteLineAsync($"Status : {updateResult.IsAcknowledged} / {updateResult.ModifiedCount}");
        await Console.Out.WriteLineAsync($"重新列出 Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Tag}");
        }
        #endregion

        #region 找出符合更新條件的文件，並進行更新多筆文件
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"找出符合更新條件的文件，並進行更新多筆文件");
        await Console.Out.WriteLineAsync($"Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Tag}");
        }

        stopwatch.Restart();

        var filter2 = Builders<Blog>.Filter.Eq(r => r.Tag, "C#");
        var update2 = Builders<Blog>.Update.Set(x => x.Tag, "XAML");
        var updateResult2 = await collection.UpdateManyAsync(filter2, update2);

        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"更新花費 {stopwatch.ElapsedMilliseconds} ms");
        await Console.Out.WriteLineAsync($"Status : {updateResult2.IsAcknowledged} / {updateResult2.ModifiedCount}");
        await Console.Out.WriteLineAsync($"重新列出 Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Tag}");
        }
        #endregion
        #endregion

    }
}