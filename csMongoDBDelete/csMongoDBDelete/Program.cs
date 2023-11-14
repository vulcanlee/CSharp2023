using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace csMongoDBDelete;


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
        var collectionName = "BlogForDelete";

        // 取得 MongoDB Collection
        database = client.GetDatabase(dbName);

        #region 先行刪除這個測試用的 Collection
        await database.DropCollectionAsync(collectionName);
        #endregion

        collection = database.GetCollection<Blog>(collectionName);

        Stopwatch stopwatch = new Stopwatch();
        #endregion

        #region 建立準備要進行刪除用的測試文件
        #region 一次新增 10 筆文件
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"建立準備要進行刪除用的測試文件");
        stopwatch.Restart();
        List<Blog> blogs = new List<Blog>();
        stopwatch.Restart();
        for (int i = 0; i < 10; i++)
        {
            // 宣告一個 Blog 物件
            Blog blog = new Blog
            {
                BlogId = i,
                Title = $"Hello MongoDB{i}",
                Tag = $"C#",
                Content = $"Hello MongoDB{i%3}",
                CreateAt = DateTime.Now.AddDays(i).Date,
                UpdateAt = DateTime.Now.AddDays(i).Date
            };
            blogs.Add(blog);
        }
        // 進行批次新增 Blog 資料
        collection.InsertMany(blogs);
        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"一次新增 10 筆文件需要 {stopwatch.ElapsedMilliseconds} ms");
        #endregion
        #endregion

        #region 找出符合刪除條件的文件，並進行刪除一筆文件
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"找出符合刪除條件的文件，並進行刪除一筆文件");
        await Console.Out.WriteLineAsync($"Collection 內的所有文件");
        var byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Content}");
        }

        stopwatch.Restart();

        var filter1 = Builders<Blog>.Filter.Eq(r => r.Title, "Hello MongoDB5");
        var updateResult = await collection.DeleteOneAsync(filter1);

        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"刪除花費 {stopwatch.ElapsedMilliseconds} ms");
        await Console.Out.WriteLineAsync($"Status : {updateResult.IsAcknowledged} / {updateResult.DeletedCount}");
        await Console.Out.WriteLineAsync($"重新列出 Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Content}");
        }
        #endregion

        #region 找出符合刪除條件的文件，並進行刪除多筆文件 使用 Builders.Filter
        Console.WriteLine();
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"找出符合刪除條件的文件，並進行刪除多筆文件 使用 Builders.Filter");
        await Console.Out.WriteLineAsync($"Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Content}");
        }

        stopwatch.Restart();

        var filter2 = Builders<Blog>.Filter.Eq(r => r.Content, "Hello MongoDB2");
        var updateResult2 = await collection.DeleteManyAsync(filter2);
        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"使用 Builders.Filter 刪除花費 {stopwatch.ElapsedMilliseconds} ms");
        await Console.Out.WriteLineAsync($"Status : {updateResult2.IsAcknowledged} / {updateResult2.DeletedCount}");
        await Console.Out.WriteLineAsync($"重新列出 Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Content}");
        }
        #endregion

        #region 找出符合刪除條件的文件，並進行刪除多筆文件 使用 LINQ
        Console.WriteLine();
        Console.WriteLine();
        await Console.Out.WriteLineAsync($"找出符合刪除條件的文件，並進行刪除多筆文件 使用 LINQ");
        await Console.Out.WriteLineAsync($"Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Content}");
        }

        stopwatch.Restart();

        var updateResult21 = await collection
            .DeleteManyAsync<Blog>(x => x.Content == "Hello MongoDB0" ||
            x.Title == "Hello MongoDB1");

        stopwatch.Stop();
        // 顯示需要耗費時間
        Console.WriteLine($"使用 Linq 刪除花費 {stopwatch.ElapsedMilliseconds} ms");
        //await Console.Out.WriteLineAsync($"Status : {updateResult21.IsAcknowledged} / {updateResult21.DeletedCount}");
        await Console.Out.WriteLineAsync($"重新列出 Collection 內的所有文件");
        byLinqCollectionWithClass = await collection.AsQueryable().ToListAsync();
        foreach (var item in byLinqCollectionWithClass)
        {
            Console.WriteLine($"  {item.Id} / {item.Title} / {item.Content}");
        }
        #endregion
        #endregion

    }
}