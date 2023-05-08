using Elastic.Clients.Elasticsearch;

namespace csElasticSearchQuichStart;


internal class Program
{
    static async Task Main(string[] args)
    {
        // 建立 ElasticsearchClient 物件
        var client = new ElasticsearchClient(new Uri("http://192.168.82.7:9200"));

        // 建立一個 Tweet 物件
        var tweet = new Tweet
        {
            User = "Vulcan Lee",
            PostDate = DateTime.Now,
            Message = "Elasticsearch 是一個開源的、分布式的、RESTful 風格的搜索和分析引擎。"
        };

        // 將 Tweet 物件存入 Elasticsearch
        var response = await client.IndexAsync(tweet, "my-index-name");

        // 檢查存入是否成功
        if (response.IsValidResponse)
        {
            // 取得存入的 ID
            Console.WriteLine($"Index document with ID {response.Id} succeeded.");
        }
    }
}