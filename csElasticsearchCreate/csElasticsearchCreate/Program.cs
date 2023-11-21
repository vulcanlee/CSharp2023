using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Transport;
using System.Diagnostics;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace csElasticsearchCreate
{
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
            var settings = new ElasticsearchClientSettings(new Uri("http://10.1.1.231:9200/"))
                .Authentication(new BasicAuthentication("elastic", "elastic"));
            var client = new ElasticsearchClient(settings);

            string indexName = "blogs".ToLower();

            Stopwatch stopwatch = new Stopwatch();

            #region 每次新增一筆文件，共 100 次
            stopwatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                Blog blog = new Blog()
                {
                    BlogId = 1,
                    Title = $"Nice to meet your {i}",
                    Content = $"Hell Elasticsearch {i}",
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                };

                var response = await client.IndexAsync(blog, indexName);

                if (response.IsValidResponse)
                {
                    //Console.WriteLine($"Index document with ID {response.Id} succeeded.");
                }
                else
                {
                    Console.WriteLine($"Error Message : {response.DebugInformation}");
                }
            }

            stopwatch.Stop();
            // 顯示需要耗費時間
            Console.WriteLine($"新增 100 次文件需要 {stopwatch.ElapsedMilliseconds} ms");
            #endregion

            #region 一次新增 100 筆文件
            stopwatch.Restart();
            Console.WriteLine();
            List<Blog> list = new List<Blog>();
            for (int i = 0; i < 100; i++)
            {
                Blog blog = new Blog()
                {
                    BlogId = 1,
                    Title = $"Nice to meet your {i}",
                    Content = $"Hell Elasticsearch {i}",
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                };
                list.Add(blog);
            }

            var operations = new List<IBulkOperation>();

            foreach (var blog in list)
            {
                var indexOperation = new BulkIndexOperation<Blog>(blog) { Index = indexName };
                operations.Add(indexOperation);
            }

            var bulkRequest = new BulkRequest
            {
                Operations = operations
            };

            var response2 =  await client.BulkAsync(bulkRequest);

            if (response2.IsValidResponse)
            {
                //Console.WriteLine($"Index document with ID {response.Id} succeeded.");
            }
            else
            {
                Console.WriteLine($"Error Message : {response2.DebugInformation}");
            }

            stopwatch.Stop();
            // 顯示需要耗費時間
            Console.WriteLine($"新增 100 次文件需要 {stopwatch.ElapsedMilliseconds} ms");
            #endregion
        }
    }
}
