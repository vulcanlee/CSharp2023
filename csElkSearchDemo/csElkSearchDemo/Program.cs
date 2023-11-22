//using Elastic.Clients.Elasticsearch;
//using Elastic.Clients.Elasticsearch.Aggregations;
//using Elastic.Transport;
using Nest;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace csElkSearchDemo
{
    public class SbuUserId
    {
        public long est_type { get; set; }
        public DateTime sbr_enddate { get; set; }
        public DateTime sbr_startdate { get; set; }
        public string sbru_userid { get; set; }
        public string sbu_userid { get; set; }
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new ConnectionSettings(new Uri("http://10.1.1.231:9200/"))
                //.DisableDirectStreaming()
                .BasicAuthentication("elastic", "elastic");

            var client = new ElasticClient(settings);

            string indexSbuName = "sbu_userid".ToLower();
            string indexReportCustHeaderName = "emr_report_cust_header".ToLower();

            #region 指定的固定參數
            string assignUserId = "admin";
            DateTime startDate = DateTime.Parse("2023/11/14 11:33:30");
            DateTime endDate = DateTime.Parse("2011/11/14 11:33:30");
            int estType = 3;
            string unique_sbu_userid_name = "unique_sbu_userid";
            #endregion

            //var resultMap = await client.MapAsync<SbuUserId>(m => m
            //    .Index(indexSbuName)
            //    .Properties( p=>p
            //        .Text(t=>t
            //            .Name(n=>n.sbu_userid)
            //            .Fielddata(true)
            //        )
            //    )
            //);

            Stopwatch stopwatch = new Stopwatch();

            #region 對 sbu_userid 做 DISTINCT  查詢
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync("對 sbu_userid 做 DISTINCT  查詢");
            stopwatch.Restart();

            var response6 = await client.SearchAsync<SbuUserId>(s => s
            .Index(indexSbuName)
            .From(0)
            .Size(100)
            .Query(q => q
                .Bool(b => b
                .Must(mt => mt
                    .DateRange(dr => dr
                        .Field(f => f.sbr_startdate)
                        .LessThanOrEquals(startDate)),
                    mte => mte
                    .DateRange(dr => dr
                        .Field(f => f.sbr_enddate)
                        .GreaterThanOrEquals(endDate)),
                    et => et
                    .Term(t => t.est_type, estType),
                    uid => uid
                    .Term(t => t.sbru_userid, assignUserId))
                    )
                )
            .Aggregations(agg => agg
                .Terms(unique_sbu_userid_name, te => te
                    .Field(f => f.sbu_userid))
                )
            );

            stopwatch.Stop();

            if (response6.IsValid)
            {
                await Console.Out.WriteLineAsync();
                await Console.Out.WriteLineAsync(response6.DebugInformation);
                await Console.Out.WriteLineAsync();
              
                var document = response6.Documents;

                if (response6.Aggregations.ContainsKey(unique_sbu_userid_name))
                {
                    var termsAggregation = response6.Aggregations.Terms(unique_sbu_userid_name);
                    foreach (var bucket in termsAggregation.Buckets)
                    {
                        Console.WriteLine($"Key: {bucket.Key}, Count: {bucket.DocCount}");
                    }
                }

            }
            // 顯示需要耗費時間
            Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            #endregion


            //#region 對 sbu_userid 做 DISTINCT  查詢
            //await Console.Out.WriteLineAsync();
            //await Console.Out.WriteLineAsync("對 sbu_userid 做 DISTINCT  查詢");
            //stopwatch.Restart();

            //var response6 = await client.SearchAsync<SbuUserId>(s => s
            //.Index(indexSbuName)
            //.From(0)
            //.Size(100)
            //.Query(q => q
            //    .Bool(b => b
            //    .Must(mt => mt
            //        .Range(r => r
            //        .DateRange(dr => dr.Field(f => f.sbr_enddate)
            //            .Gte(endDate).Lte(startDate)
            //        )),
            //        et => et
            //        .Term(t => t.est_type, estType),
            //        uid => uid
            //        .Term(t => t.sbru_userid, assignUserId))
            //        )
            //    )
            //.Aggregations(agg => agg
            //    .Terms("unique", te => te
            //        .Field(f => f.sbu_userid)
            //        .Size(10))
            //    )
            //);

            //stopwatch.Stop();

            //if (response6.IsValidResponse)
            //{
            //    var SbuUsers = response6.Documents;
            //    foreach (var SbuUser in SbuUsers)
            //    {
            //        await Console.Out.WriteLineAsync($"Id={SbuUser.sbu_userid} ");
            //    }
            //}
            //// 顯示需要耗費時間
            //Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            //#endregion

        }
    }
}
