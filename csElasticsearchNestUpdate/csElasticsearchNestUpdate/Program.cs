using Nest;
using System.Diagnostics;

namespace csElasticsearchNestUpdate;

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

        #region 取得需要更新的文件
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"取得需要更新的文件");
        stopwatch.Restart();
        var searchResponse3 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .Size(200)
               .Query(q => q
                      .Term(t => t.BlogId, 3)
                 )
            );
        stopwatch.Stop();

        Blog needUpdateBlog = null;
        if (searchResponse3.IsValid)
        {
            var documents = searchResponse3.Documents;
            needUpdateBlog = documents.FirstOrDefault();
            Console.WriteLine($"查詢到的文件數量為：{documents.Count} / Title : {needUpdateBlog.Title}， 花費 {stopwatch.ElapsedMilliseconds} ms");
        }
        #endregion

        #region 進行更新文件
        if (needUpdateBlog != null)
        {
            await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync($"進行更新文件");
            needUpdateBlog.Title = $"更新後的標題 {DateTime.Now}";
            stopwatch.Restart();
            var searchResponse4 = await client
                .UpdateAsync<Blog>(needUpdateBlog.BlogId, u => u
                    .Index(indexName)
                    .Doc(needUpdateBlog)
                );
            if (searchResponse4.IsValid)
            {
                Console.WriteLine($"成功更新文件 Title : {needUpdateBlog.Title}， 花費 {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        #endregion
    }
}
