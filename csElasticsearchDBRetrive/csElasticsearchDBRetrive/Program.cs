using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Elastic.Clients.Elasticsearch.QueryDsl;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Text;

namespace csElasticsearchDBRetrive
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
                .Authentication(new BasicAuthentication("elastic", "elastic"))
                .OnRequestCompleted(apiCallDetails =>
                {
                    if (apiCallDetails.RequestBodyInBytes != null)
                    {
                        Console.WriteLine(
                            $"{apiCallDetails.HttpMethod} {apiCallDetails.Uri} " +
                            $"{Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes)}");
                    }
                    else
                    {
                        Console.WriteLine($"{apiCallDetails.HttpMethod} {apiCallDetails.Uri}");
                    }

                    // log out the response and the response body, if one exists for the type of response
                    if (apiCallDetails.ResponseBodyInBytes != null)
                    {
                        Console.WriteLine($"Status: {apiCallDetails.HttpStatusCode}" +
                                 $"{Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes)}");
                    }
                    else
                    {
                        Console.WriteLine($"Status: {apiCallDetails.HttpStatusCode}");
                    }
                });

            var client = new ElasticsearchClient(settings);

            string indexName = "blogs".ToLower();

            // 嘗試讓 client 物件與後端 Elasticsearch 來通訊，避免第一次的延遲
            await client.GetAsync<Blog>(1, idx => idx.Index(indexName));

            Stopwatch stopwatch = new Stopwatch();

            //#region Term 查詢
            //await Console.Out.WriteLineAsync("Term 查詢");
            //stopwatch.Restart();

            //var response = await client.SearchAsync<Blog>(s => s
            //.Index(indexName)
            //.From(0)
            //.Size(10)
            //.Query(q => q
            //.Term(t => t.BlogId, 1)));

            //stopwatch.Stop();

            //if (response.IsValidResponse)
            //{
            //    var blogs = response.Documents;
            //    foreach (var blog in blogs)
            //    {
            //        await Console.Out.WriteLineAsync($"Id={blog.BlogId} / Title={blog.Title}");
            //    }
            //}
            //// 顯示需要耗費時間
            //Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            //#endregion

            //#region Match 查詢
            //await Console.Out.WriteLineAsync();
            //await Console.Out.WriteLineAsync("Match 查詢");
            //stopwatch.Restart();

            //var response2 = await client.SearchAsync<Blog>(s => s
            //.Index(indexName)
            //.From(0)
            //.Size(10)
            //.Query(q => q
            //    .Match(m => m
            //        .Field(f => f.Title)
            //        .Query("meet"))));

            //stopwatch.Stop();

            //if (response2.IsValidResponse)
            //{
            //    var blogs = response2.Documents;
            //    foreach (var blog in blogs)
            //    {
            //        await Console.Out.WriteLineAsync($"Id={blog.BlogId} / Title={blog.Title}");
            //    }
            //}
            //// 顯示需要耗費時間
            //Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            //#endregion

            //#region Range (數值) 查詢
            //await Console.Out.WriteLineAsync();
            //await Console.Out.WriteLineAsync("Range (數值) 查詢");
            //stopwatch.Restart();

            //var response3 = await client.SearchAsync<Blog>(s => s
            //.Index(indexName)
            //.From(0)
            //.Size(10)
            //.Query(q => q
            //    .Range(r => r
            //        .NumberRange(dr=>dr.Field(f=>f.BlogId)
            //            .Gt(90).Lt(100)
            //        ))));

            //stopwatch.Stop();

            //if (response3.IsValidResponse)
            //{
            //    var blogs = response3.Documents;
            //    foreach (var blog in blogs)
            //    {
            //        await Console.Out.WriteLineAsync($"Id={blog.BlogId} / Title={blog.Title} / CreateAt={blog.CreateAt}");
            //    }
            //}
            //// 顯示需要耗費時間
            //Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            //#endregion

            #region Range (日期) 查詢
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync("Range (日期) 查詢");
            stopwatch.Restart();

            var response4 = await client.SearchAsync<Blog>(s => s
            .Index(indexName)
            .From(0)
            .Size(10)
            .Query(q => q
                .Range(r => r
                    .DateRange(dr => dr.Field(f => f.CreateAt)
                        .Gte(DateTime.Now.AddDays(20)).Lte(DateTime.Now.AddDays(24))
                    ))));

            stopwatch.Stop();

            if (response4.IsValidResponse)
            {
                var blogs = response4.Documents;
                foreach (var blog in blogs)
                {
                    await Console.Out.WriteLineAsync($"Id={blog.BlogId} / Title={blog.Title} / CreateAt={blog.CreateAt}");
                }
            }
            // 顯示需要耗費時間
            Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            #endregion
        }
    }
}
