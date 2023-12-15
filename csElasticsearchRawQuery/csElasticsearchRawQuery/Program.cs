using Elasticsearch.Net;
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

        #region 使用 原始 DSL 查詢
        await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync($"使用 原始 DSL 查詢");
        string query = @"{
    ""wildcard"": {
      ""title"": {
        ""value"": ""*3*""
      }
    }
  }
";

        string query2 = @"";
        stopwatch.Restart();

        // https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/raw-query-usage.html
        var searchResponse2 = await client
            .SearchAsync<Blog>(s =>
              s.Index(indexName)
               .Query(q => q.Raw(query2))
               .Size(8)
                 )
              ;

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
    }
}











//using Nest;
//using Elasticsearch.Net;
//using System;

//public class ElasticsearchService
//{
//    private readonly ElasticClient _client;

//    public ElasticsearchService()
//    {
//        var settings = new ConnectionSettings(new Uri("http://localhost:9200")) // 將此替換為您的 Elasticsearch 實例地址
//                          .DefaultIndex("blogs"); // 將此替換為您的默認索引名稱

//        _client = new ElasticClient(settings);
//    }

//    public string SearchBlogsUsingRawQuery(string rawQuery)
//    {
//        var searchResponse = _client.LowLevel.Search<StringResponse>("blogs", rawQuery); // "blogs" 是目標索引的名稱
//        return searchResponse.Body;
//    }
//}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        var elasticsearchService = new ElasticsearchService();

//        // 假設這是您的 DSL 查詢
//        string rawQuery = @"
//        {
//            ""query"": {
//                ""match_all"": {}
//            }
//        }";

//        string searchResult = elasticsearchService.SearchBlogsUsingRawQuery(rawQuery);
//        Console.WriteLine(searchResult);
//    }
//}

