using Nest;
using System.Diagnostics;

namespace csElasticsearchNestRetrive;

[ElasticsearchType(IdProperty = nameof(BlogId))]
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
    static async Task Main(string[] args)
    {
        var settings = new ConnectionSettings(new Uri("http://10.1.1.231:9200/"))
            .DisableDirectStreaming()
            .BasicAuthentication("elastic", "elastic");

        var client = new ElasticClient(settings);

        string indexName = "blogs".ToLower();

        Stopwatch stopwatch = new Stopwatch();

        #region 取得所有文件 (沒有指定頁面大小)
        await Console.Out.WriteLineAsync($"取得所有文件 (沒有指定頁面大小)");
        stopwatch.Restart();
        var searchResponse1 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
            );
        stopwatch.Stop();
        if (searchResponse1.IsValid)
        {
            var document1 = searchResponse1.Documents;
            foreach (var item in document1)
            {
                Console.WriteLine($"    BlogId:{item.BlogId} , Title:{item.Title}");
            }
            Console.WriteLine($"查詢到的文件數量為：{document1.Count} ， 花費 {stopwatch.ElapsedMilliseconds} ms");
        }
        #endregion

        #region 取得所有文件 (每頁20筆文件，指定第三頁)
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"取得所有文件 (沒有指定頁面大小)");
        stopwatch.Restart();
        var searchResponse2 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .From(20 * 3)
               .Size(20)
            );
        stopwatch.Stop();
        if (searchResponse2.IsValid)
        {
            var documents = searchResponse2.Documents;
            foreach (var item in documents)
            {
                Console.WriteLine($"    BlogId:{item.BlogId} , Title:{item.Title}");
            }
            Console.WriteLine($"查詢到的文件數量為：{documents.Count} ， 花費 {stopwatch.ElapsedMilliseconds} ms");
        }
        #endregion

        #region 取得 Title 有 3 文字之所有文件
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"取得 Title 有 1003 文字之所有文件");
        stopwatch.Restart();
        var searchResponse3 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .Size(200)
               .Query(q => q
                      .Match(m => m
                            .Field(f => f.Title)
                            .Query("33")
                      )
                 )
            );
        stopwatch.Stop();
        if (searchResponse3.IsValid)
        {
            var documents = searchResponse3.Documents;
            foreach (var item in documents)
            {
                Console.WriteLine($"    BlogId:{item.BlogId} , Title:{item.Title}");
            }
            Console.WriteLine($"查詢到的文件數量為：{documents.Count} ， 花費 {stopwatch.ElapsedMilliseconds} ms");
        }
        #endregion

        #region 取得 Title 有包含任何 3 文字之所有文件
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"取得 Title 有包含任何 3 文字之所有文件");
        stopwatch.Restart();
        var searchResponse4 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .Size(200)
               .Query(q => q
                        .Wildcard(m => m.Value("*33*").Field(f => f.Title))
                     )
            );
        stopwatch.Stop();
        if (searchResponse4.IsValid)
        {
            var documents = searchResponse4.Documents;
            foreach (var item in documents)
            {
                Console.WriteLine($"    BlogId:{item.BlogId} , Title:{item.Title}");
            }
            Console.WriteLine($"查詢到的文件數量為：{documents.Count} ， 花費 {stopwatch.ElapsedMilliseconds} ms");
        }
        #endregion
    }
}
