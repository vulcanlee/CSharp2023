using Nest;
using System.Diagnostics;

namespace csElasticsearchNestDelete;

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

        #region 取得需要刪除的文件
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"取得需要刪除的文件");
        int needDeleteBlogId = 10006;
        stopwatch.Restart();
        var searchResponse3 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .Size(200)
               .Query(q => q
                      .Term(t => t.BlogId, needDeleteBlogId)
                 )
            );
        stopwatch.Stop();

        Blog needDeleteBlog = null;
        if (searchResponse3.IsValid)
        {
            var documents = searchResponse3.Documents;
            needDeleteBlog = documents.FirstOrDefault();
            if (needDeleteBlog != null)
            {
                Console.WriteLine($"查詢到的文件數量為：{documents.Count} / Title : {needDeleteBlog.Title}， 花費 {stopwatch.ElapsedMilliseconds} ms");
            }
            else
            {
                Console.WriteLine($"沒有搜尋到文件， 花費 {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        #endregion

        #region 進行刪除文件
        if (needDeleteBlog != null)
        {
            await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync($"進行刪除文件");
            stopwatch.Restart();
            var searchResponse4 = await client
                .DeleteAsync<Blog>(needDeleteBlog.BlogId,
                  d => d.Index(indexName)
            );
            if (searchResponse4.IsValid)
            {
                Console.WriteLine($"成功刪除文件 Title : {needDeleteBlog.Title}， 花費 {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        #endregion

        #region 再次查詢已經刪除的文件
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"稍後3秒鐘");
        await Task.Delay(3000);
        await Console.Out.WriteLineAsync($"查詢已經刪除的文件");
        stopwatch.Restart();
        var searchResponse5 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .Size(200)
               .Query(q => q
                      .Term(t => t.BlogId, needDeleteBlogId)
                 )
            );
        stopwatch.Stop();

        if (searchResponse5.IsValid)
        {
            var documents = searchResponse5.Documents;
            needDeleteBlog = documents.FirstOrDefault();
            if (needDeleteBlog != null)
            {
                Console.WriteLine($"查詢到的文件數量為：{documents.Count} / Title : {needDeleteBlog.Title}， 花費 {stopwatch.ElapsedMilliseconds} ms");
            }
            else
            {
                Console.WriteLine($"沒有搜尋到文件，該文件已經被刪除了， 花費 {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        #endregion

    }
}
